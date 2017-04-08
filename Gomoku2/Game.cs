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

            if (state.StartDepth - state.Depth <= -1)
            {
                Parallel.ForEach(EstimateCells(state), (estimatedCell, parallelLoopState) =>
                {
                    if (ProcessCell(state, parent, estimatedCell, result))
                        parallelLoopState.Stop();
                });
            }
            else
            {
                foreach (var estimatedCell in EstimateCells(state))
                {
                    if (ProcessCell(state, parent, estimatedCell, result)) break;
                }
            }
            return result;
        }

        private bool ProcessCell(BoardState state, GameState parent, EstimatedCell estimatedCell, AlphaBetaResult res)
        {
            var cell = estimatedCell.Cell;

            board[cell.X, cell.Y] = state.MyCellType;
            var currEstim = estimatedCell.Estimate*state.Multiplier;

            var gameState = new GameState(state, estimatedCell);
            OnStateChanged(gameState, parent);
            var cellResult = ShoudNotGoDeeper(state, estimatedCell)
                ? new AlphaBetaResult(currEstim)
                : AlphaBeta(state.GetNextState(estimatedCell.MyLines, estimatedCell.OppLines), res.Alpha, res.Beta, gameState);

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
            board[cell.X, cell.Y] = BoardCell.None;
            return ShouldBreak(state, res, cellResult.MinMax);
        }

        private static bool ShoudNotGoDeeper(BoardState state, EstimatedCell estimatedCell)
        {
            return state.IsTerminal
                   || FiveInRow(estimatedCell.Estimate)
                   || StraightFour(estimatedCell.Estimate)
                   || DoubleThreat(estimatedCell.Estimate);
        }

        private static bool ShouldBreak(BoardState state, AlphaBetaResult res, int minMax)
        {
            return BreakOnFive(state.ItIsFirstsTurn, minMax)
                   || BreakOnStraightFour(state.ItIsFirstsTurn, minMax)
                   || BreakOnDoubleThreat(state.ItIsFirstsTurn, minMax)
                   || res.Beta <= res.Alpha;
        }

        private static bool DoubleThreat(int estimate)
        {
            //todo consider also Double Threat as an exit condition
            return false;
            return Math.Abs(estimate) >= (int)LineType.DoubleThreat / 2;
        }

        private static bool FiveInRow(int estim)
        {
            return Math.Abs(estim) >= (int)LineType.FiveInRow / 2;
        }

        public static bool StraightFour(int estim)
        {
            return Math.Abs(estim) >= (int)LineType.StraightFour / 2;
        }

        private static bool BreakOnFive(bool movesFirst, int estim)
        {
            return (estim <= -(int)LineType.FiveInRow / 2 && !movesFirst) || (estim >= (int)LineType.FiveInRow / 2 && movesFirst);
        }

        private static bool BreakOnStraightFour(bool movesFirst, int estim)
        {
            return (estim <= -(int)LineType.StraightFour / 2 && !movesFirst) || (estim >= (int)LineType.StraightFour / 2 && movesFirst);
        }

        private static bool BreakOnDoubleThreat(bool movesFirst, int estim)
        {
            return (estim <= -(int)LineType.DoubleThreat / 2 && !movesFirst) || (estim >= (int)LineType.DoubleThreat / 2 && movesFirst);
        }

        private void OnStateChanged(GameState gameState, GameState parentState)
        {
            if (parentState == null)
                GameStates.Add(gameState);
            else
                parentState.AddChild(gameState);
        }

        private Cell FirstMoveCase()
        {
            return board[7, 7] == BoardCell.None ? CellManager.Get(7, 7) : CellManager.Get(8, 8);
        }

        private static int Sum(List<Line> lines)
        {
            var estims = lines.Select(l => l.LineType).ToList();
            var sum = estims.Sum(es => (int)es);
            if (HasDoubleThreat(lines))
                sum += (int)LineType.DoubleThreat;
            return sum;
        }

        private static bool HasDoubleThreat(List<Line> lines)
        {
            int killerLines = lines.Count(line => line.LineType.ThreatOfThree() || line.LineType.ThreatOfFour());
            return killerLines >= 2;
        }

        private IEnumerable<EstimatedCell> EstimateCells(BoardState state)
        {
            var cells = state.GetNextCells();
            //todo we estimate leaf position we may consider to take ONLY GetPriorityCells(MyLines), without all near empty cells
            var estimatedCells = GetEstimatedCells(state, cells);
            if (state.IsTerminal)
                return estimatedCells;

            return estimatedCells.OrderByDescending(ec => ec.Estimate).Take(state.MaxWidth);
        }

        private IEnumerable<EstimatedCell> GetEstimatedCells(BoardStateBase state, NextCells nextCells)
        {
            foreach (var cell in nextCells.MyNextCells)
            {
                board[cell.X, cell.Y] = state.MyCellType;
                var newState = GetLinesByAddingCell(cell, state);
                var estimate = Estimate(newState.MyLines, newState.OppLines);
                board[cell.X, cell.Y] = BoardCell.None;

                yield return new EstimatedCell(cell, newState.MyLines, newState.OppLines, estimate);
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

        public int Estimate(List<Line> myLines, List<Line> oppLines)
        {
            var myEstim = Sum(myLines);
            if (FiveInRow(myEstim)) return myEstim;

            var oppEstim = Sum(oppLines);

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

        private static BoardStateBase GetLinesByAddingCell(Cell cell, BoardStateBase state)
        {
            var clonedState = state.Clone();
            BoardFactory.AddCellToLines(cell, clonedState);
            //CheckConsistency(state, clonedState);
            return clonedState;
        }

        private static void CheckConsistency(BoardStateBase state, BoardStateBase clonedState)
        {
            var myLines = BoardFactory.GetLines(state.Board, state.MyCellType);
            if (myLines.Count != clonedState.MyLines.Count)
            {
                var line = myLines.Except(clonedState.MyLines);
                var line2 = clonedState.MyLines.Except(myLines);
                BoardExportImport.Export(state.Board, "HorribleMismatch.txt");
                throw new Exception("HorribleMismatch.txt");
            }

            var oppLines = BoardFactory.GetLines(state.Board, state.OpponentCellType);
            if (oppLines.Count != clonedState.OppLines.Count)
            {
                var line = oppLines.Except(clonedState.OppLines);
                var line2 = clonedState.OppLines.Except(oppLines);
                BoardExportImport.Export(state.Board, "HorribleMismatch.txt");
                throw new Exception("HorribleMismatch.txt");
            }
        }
    }
}