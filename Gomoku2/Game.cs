using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            var move = DoMoveInternal(boardCell, depth, treeMaxWidth);
            board[move.X, move.Y] = boardCell;
            currentState = GameStates.FirstOrDefault(gs => gs.Cell == move);
            return move;
        }

        private Cell DoMoveInternal(BoardCell boardCell, int depth, int maxWidth)
        {
            sw.Start();
            BoardState state;
            if (currentState != null)
            {
                var bs = currentState.BoardState;
                //lines are swaped - analyzis was made from 2nd viewpoint.
                state = new BoardState(bs.OppLines, bs.MyLines, boardCell, depth, 0, maxWidth, board);
            }
            else
            {
                var myLines = GetLines(boardCell);
                var oppLines = GetLines(boardCell.Opponent());
                if (!myLines.Any()) return FirstMoveCase();

                state = new BoardState(myLines, oppLines, boardCell, depth, 0, maxWidth, board);
            }

            //todo we may want to remember history for perf improvement
            GameStates.Clear();
            Cell move;
            LastEstimate = AlphaBeta(state, int.MinValue, int.MaxValue, out move, null);
            sw.Stop();
            return move;
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
                    StraightFourAndOpponentDoesntHaveThreatOfFour(estimatedCell.Estimate))
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
                if (BreakOnFive(state.ItIsFirstsTurn, minMax) || BreakOnStraightFour(state.ItIsFirstsTurn, minMax)
                    || beta <= alpha) break;
            }
            return bestEstim;
        }

        private static bool StraightFourAndOpponentDoesntHaveThreatOfFour(int estimate)
        {
            return StraightFour(estimate);
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

        private int EstimateAndSum(List<Line> lines)
        {
            lines.ForEach(l => l.Estimate(board));
            return Sum(lines);
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
            int killerLines = lines.Count(line => line.LineType.ThreatOfThree() || (line.LineType.ThreatOfFour() && line.Count > 2));
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

        private IEnumerable<EstimatedCell> GetEstimatedCells(BoardState state, NextCells nextCells)
        {
            foreach (var cell in nextCells.MyNextCells)
            {
                board[cell.X, cell.Y] = state.MyCellType;
                var myNewLines = GetLinesByAddingCell(cell, state.MyLines, state.MyCellType);
                var oppClonedLines = new List<Line>(state.OppLines.Select(l => l.Clone()));
                var estimate = Estimate(myNewLines, oppClonedLines);
                board[cell.X, cell.Y] = BoardCell.None;

                yield return new EstimatedCell(cell, myNewLines, oppClonedLines, estimate);
            }

            if (nextCells.OppNextCells == null)
                yield break;
            //todo consider if need to use it at all
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

            var oppEstim = EstimateAndSum(oppLines);

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
            return GetLines(board, cellType);
        }

        public static List<Line> GetLines(BoardCell[,] board, BoardCell type)
        {
            var lines = new List<Line>();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] != type) continue;
                    FillLines(CellManager.Get(i, j), lines, type, board);
                }
            }
            lines.ForEach(l => l.Estimate(board));
            lines.Sort();
            return lines;
        }

        private List<Line> GetLinesByAddingCell(Cell cell, List<Line> existingLines, BoardCell cellType)
        {
            var lines = new List<Line>(existingLines.Select(existingLine => existingLine.Clone()));
            FillLines(cell, lines, cellType, board);
            lines.Sort();
            return lines;
        }

        private static void FillLines(Cell cell, List<Line> lines, BoardCell cellType, BoardCell[,] board)
        {
            //todo use for better join\merge algorithm;
            //var adjustmentCells = cell.GetAdjustmentCells(board, cellType);
            //foreach (var adjustmentCell in adjustmentCells)
            //{
                
            //}

            var cellsUsedInAdding = new HashSet<Cell>();
            var addedToSomeLine = false;
            var usedLines = new List<Line>();
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                if (!line.JoinIfPossible(cell, board)) continue;

                cellsUsedInAdding.UnionWith(line);
                addedToSomeLine = true;
                MergeLines(lines, usedLines, line, board);
            }
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                foreach (var lineCell in line)
                {
                    if (lineCell.DistSqr(cell) > 2 || cellsUsedInAdding.Contains(lineCell)) continue;

                    var newLine = new Line(cell, lineCell, board, cellType);
                    if (!lines.Contains(newLine))
                    {
                        lines.Add(newLine);
                        addedToSomeLine = true;
                        cellsUsedInAdding.Add(lineCell);
                        MergeLines(lines, usedLines, newLine, board);
                    }
                }
            }
            if (!addedToSomeLine)
            {
                lines.Add(new Line(cell, cellType));
            }
        }

        private static void MergeLines(List<Line> lines, List<Line> usedLines, Line line, BoardCell[,] board)
        {
            Line mergedLine = null;
            foreach (var usedLine in usedLines.ToList())
            {
                if (!usedLine.HasSameDirection(line)) continue;

                mergedLine = usedLine.GetMergedLine(line, board);
                usedLines.Remove(usedLine);
                lines.Remove(usedLine);
                lines.Remove(line);
                lines.Add(mergedLine);
            }
            if (mergedLine == null) usedLines.Add(line);
        }
    }
}