using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;
using Gomoku2.StateCache;

namespace Gomoku2
{
    public class Game
    {
        public const int DefaultDepth = 4, DefaultWidth = 15;

        private readonly BoardCell[,] board;
        private readonly Stopwatch sw;
        private GameState currentState;

        public Game(BoardCell[,] board)
        {
            this.board = (BoardCell[,])board.Clone();
            sw = new Stopwatch();
            GameStates = new List<GameState>();
            Depth = DefaultDepth;
        }

        public Game(int width, int height)
            :this(new BoardCell[width, height])
        {
        }

        public TimeSpan Elapsed => sw.Elapsed;

        public int LastEstimate { get; private set; }

        public List<GameState> GameStates { get; }

        public int Depth { private get; set; }
        public bool AnalyzeModeOn { get; set; }

        public void DoOpponentMove(int x, int y)
        {
            DoOpponentMove(x, y, BoardCell.Second);
        }

        public void DoOpponentMove(int x, int y, BoardCell boardCell)
        {
            board[x, y] = boardCell;
            if (currentState != null)
                currentState = currentState.Children.FirstOrDefault(gs => gs.Cell == new Cell(x, y));
        }

        public Cell DoMove()
        {
            return DoMove(BoardCell.First, Depth, DefaultWidth);
        }

        public Cell DoMove(BoardCell boardCell, int depth, int treeMaxWidth)
        {
            var move = DoMoveInternal(boardCell, depth, treeMaxWidth);
            board[move.X, move.Y] = boardCell;
            currentState = GameStates.FirstOrDefault(gs => gs.Cell == move);
            return move;
        }

        private Cell DoMoveInternal(BoardCell boardCell, int depth, int maxWidth)
        {
            sw.Start();
            BoardState state;
            //todo make it work.
            if (currentState != null && false)
            {
                var bs = currentState.BoardState;
                //lines are swaped - analyzis was made from 2nd viewpoint.
                state = new BoardState(bs.OppLines, bs.MyLines, boardCell, depth, 0, maxWidth, board);
            }
            else
            {
                state = GetBoardState(boardCell, depth, maxWidth);
            }
            if (!state.MyLines.Any())
                return FirstMoveCase();

            //todo we may want to remember history for perf improvement
            GameStates.Clear();
            var move = AlphaBeta(state, int.MinValue, int.MaxValue, null);
            LastEstimate = move.MinMax;
            sw.Stop();
            return move.Move;
        }

        public BoardState GetBoardState(BoardCell boardCell, int depth, int maxWidth)
        {
            var myLines = GetLines(boardCell);
            var oppLines = GetLines(boardCell.Opponent());
            return new BoardState(myLines, oppLines, boardCell, depth, 0, maxWidth, board) { StartDepth = depth };
        }

        private AlphaBetaResult AlphaBeta(BoardState state, int alpha, int beta, GameState parent)
        {
            var result = new AlphaBetaResult(state.StartEstimate, alpha, beta);
            var estimatedCells = EstimateCells(state);

            //double enumeration doesn't happen - we parallelize only non-terminal states
            if (state.AllowParallelize(estimatedCells) && !AnalyzeModeOn)
            {
                Parallel.ForEach(estimatedCells, (estimatedCell, parallelLoopState) =>
                {
                    if (ProcessCell(state, parent, estimatedCell, result))
                        parallelLoopState.Stop();
                });
            }
            else
            {
                foreach (var estimatedCell in estimatedCells)
                {
                    if (ProcessCell(state, parent, estimatedCell, result)) break;
                }
            }
            return result;
        }

        private bool ProcessCell(BoardState state, GameState parent, EstimatedCell estimatedCell, AlphaBetaResult res)
        {
            var cell = estimatedCell.Cell;
            var currEstim = estimatedCell.Estimate*state.Multiplier;

            var gameState = new GameState(estimatedCell);
            OnStateChanged(gameState, parent);
            var cellResult = ShoudNotGoDeeper(state, estimatedCell)
                ? new AlphaBetaResult(currEstim)
                : AlphaBeta(state.GetNextState(estimatedCell.BoardState), res.Alpha, res.Beta, gameState);

            gameState.Estimate = cellResult.MinMax;
            if (state.ItIsFirstsTurn && cellResult.MinMax > res.MinMax)
            {
                res.MinMax = cellResult.MinMax;
                res.Alpha = cellResult.MinMax;
                res.Move = cell;
            }
            if (!state.ItIsFirstsTurn && cellResult.MinMax < res.MinMax)
            {
                res.MinMax = cellResult.MinMax;
                res.Beta = cellResult.MinMax;
                res.Move = cell;
            }
            return ShouldBreak(state, res, cellResult.MinMax);
        }

        private static bool ShoudNotGoDeeper(BoardState state, EstimatedCell estimatedCell)
        {
            return state.IsTerminal || Math.Abs(estimatedCell.Estimate) >= (int) LineType.DoubleThreat/2;
        }

        private static bool ShouldBreak(BoardState state, AlphaBetaResult res, int minMax)
        {
            return BreakOnDoubleThreat(state.ItIsFirstsTurn, minMax)
                   || res.Beta <= res.Alpha;
        }

        private static bool FiveInRow(int estim)
        {
            return Math.Abs(estim) >= (int)LineType.FiveInRow / 2;
        }

        private static bool BreakOnDoubleThreat(bool movesFirst, int estim)
        {
            return (estim <= -(int)LineType.DoubleThreat / 2 && !movesFirst) || (estim >= (int)LineType.DoubleThreat / 2 && movesFirst);
        }

        private void OnStateChanged(GameState gameState, GameState parentState)
        {
            if (AnalyzeModeOn)
            {
                if (parentState == null)
                    GameStates.Add(gameState);
                else
                    parentState.AddChild(gameState);
            }
        }

        private Cell FirstMoveCase()
        {
            return board[7, 7] == BoardCell.None ? CellManager.Get(7, 7) : CellManager.Get(8, 8);
        }

        private static IEnumerable<EstimatedCell> EstimateCells(BoardState state)
        {
            var cells = state.GetNextCells();
            //todo we estimate leaf position we may consider to take ONLY GetPriorityCells(MyLines), without all near empty cells
            var estimatedCells = GetEstimatedCells(state, cells);
            if (state.IsTerminal)
                return estimatedCells;

            return estimatedCells.OrderByDescending(ec => ec.Estimate).Take(state.MaxWidth);
        }

        private static IEnumerable<EstimatedCell> GetEstimatedCells(BoardStateBase state, NextCells nextCells)
        {
            foreach (var cell in nextCells.MyNextCells)
            {
                state.Board[cell.X, cell.Y] = state.MyCellType;
                var newState = state.GetNewState(cell);
                var estimate = Estimate(newState.MyLines, newState.OppLines);
                state.Board[cell.X, cell.Y] = BoardCell.None;

                yield return new EstimatedCell(cell, estimate, newState);
            }

            if (nextCells.OppNextCells == null)
                yield break;
            //foreach (var cell in nextCells.OppNextCells)
            //{
            //    board[cell.X, cell.Y] = state.OpponentCellType;
            //    var oppNewLines = GetLinesByAddingCell(cell, state.OppLines, state.OpponentCellType);
            //    var estimate = Estimate(oppNewLines, state.MyLines);
            //    board[cell.X, cell.Y] = BoardCell.None;
            //}
        }

        public static int Estimate(List<Line> myLines, List<Line> oppLines)
        {
            var myEstim = myLines.Sum();
            if (FiveInRow(myEstim)) return myEstim;

            var oppEstim = oppLines.Sum();

            if (oppLines.Any(line => line.LineType.FourCellLine()))
                return -(int)LineType.FiveInRow;

            if (myLines.Any(line => line.LineType.FourCellLine()))
                return myEstim - oppEstim;

            if (oppLines.Any(line => line.LineType.ThreatOfThree()))
                return -(int)LineType.FiveInRow;

            return myEstim - oppEstim;
        }

        public List<Line> GetLines(BoardCell cellType)
        {
            return BoardFactory.GetLines(board, cellType);
        }
    }
}