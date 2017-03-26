using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;
using Gomoku2.StateCache;

namespace Gomoku2
{
    public class Game
    {
        public const int DefaultDepth = 4, DefaultWidth = 15;

        private readonly int width;
        private readonly int height;
        private readonly BoardCell[,] board;
        private readonly Stopwatch sw;
        private GameState currentState;

        public Game(BoardCell[,] board)
        {
            width = board.GetLength(0);
            height = board.GetLength(1);
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
            Cell move;
            //try
            //{
            //    move = DoMoveInternal(boardCell, depth, treeMaxWidth);
            //}
            //catch (Exception)
            //{
            //    BoardExportImport.Export(new EstimatedBoard { Board = board }, "ErrorBoard.txt");
            //    throw;
            //}
            move = DoMoveInternal(boardCell, depth, treeMaxWidth);
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
            Cell move;
            LastEstimate = AlphaBeta(state, int.MinValue, int.MaxValue, out move, null);
            sw.Stop();
            return move;
        }

        public BoardState GetBoardState(BoardCell boardCell, int depth, int maxWidth)
        {
            var myLines = GetLines(boardCell);
            var oppLines = GetLines(boardCell.Opponent());
            return new BoardState(myLines, oppLines, boardCell, depth, 0, maxWidth, board);
        }

        private int AlphaBeta(BoardState state, int alpha, int beta, out Cell move, GameState parent)
        {
            move = null;
            int bestEstim = state.StartEstimate;

            foreach (var estimatedCell in EstimateCells(state))
            {
                var cell = estimatedCell.Cell;
                Cell bestMove;

                board[cell.X, cell.Y] = state.MyCellType;
                var currEstim = estimatedCell.Estimate*state.Multiplier;

                var gameState = new GameState {BoardState = state.GetThisState(estimatedCell.MyLines, estimatedCell.OppLines), Cell = cell, Estimate = currEstim};
                OnStateChanged(gameState, parent);
                int minMax;
                if (state.IsTerminal || FiveInRow(estimatedCell.Estimate) ||
                    StraightFour(estimatedCell.Estimate) ||
                    DoubleThreat(estimatedCell.Estimate))
                    minMax = currEstim;
                else
                    minMax = AlphaBeta(state.GetNextState(estimatedCell.MyLines, estimatedCell.OppLines), alpha, beta, out bestMove, gameState);

                gameState.Estimate = minMax;

                if (state.ItIsFirstsTurn && minMax > bestEstim)
                {
                    bestEstim = minMax;
                    alpha = minMax;
                    move = cell;
                }
                if (!state.ItIsFirstsTurn && minMax < bestEstim)
                {
                    bestEstim = minMax;
                    beta = minMax;
                    move = cell;
                }
                board[cell.X, cell.Y] = BoardCell.None;
                if (BreakOnFive(state.ItIsFirstsTurn, minMax)
                    || BreakOnStraightFour(state.ItIsFirstsTurn, minMax)
                    || BreakOnDoubleThreat(state.ItIsFirstsTurn, minMax)
                    || beta <= alpha) break;
            }
            return bestEstim;
        }

        private static bool DoubleThreat(int estimate)
        {
            //todo consider also Double Threat as exit condition
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
            {
                GameStates.Add(gameState);
            }
            else
            {
                parentState.AddChild(gameState);
            }
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
            return LineFactory.GetLines(board, cellType);
        }

        private static BoardStateBase GetLinesByAddingCell(Cell cell, BoardStateBase state)
        {
            var clonedState = state.Clone();
            LineFactory.AddCellToLines(cell, clonedState);
            //todo remove
            var lines = LineFactory.GetLines(state.Board, state.MyCellType);
            if (lines.Count != clonedState.MyLines.Count)
            {
                //horrible mismatch
            }
            return clonedState;
        }
    }
}