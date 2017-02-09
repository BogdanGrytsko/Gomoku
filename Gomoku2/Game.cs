using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gomoku2
{
    public class Game
    {
        public const int DefaultDepth = 4, DefaultWidth = 20;

        private readonly int width;
        private readonly int height;
        private readonly BoardCell[,] board;
        private readonly Stopwatch sw;
        private readonly List<GameState> gameStates;
        private int lastEstimate;

        public Game(BoardCell[,] board)
        {
            width = board.GetLength(0);
            height = board.GetLength(1);
            this.board = (BoardCell[,])board.Clone();
            sw = new Stopwatch();
            gameStates = new List<GameState>();
        }

        public Game(int width, int height)
        {
            this.width = width;
            this.height = height;
            board = new BoardCell[width, height];
            sw = new Stopwatch();
            gameStates = new List<GameState>();
        }

        public EstimatedBoard EstimatedBoard
        {
            get
            {
                return new EstimatedBoard
                {
                    Board = (BoardCell[,]) board.Clone(),
                    Estimate = lastEstimate
                };
            }
        }
        
        public TimeSpan Elapsed { get { return sw.Elapsed; } }

        public List<GameState> GameStates
        {
            get { return gameStates; }
        } 

        public void DoOpponentMove(int x, int y, BoardCell boardCell = BoardCell.Second)
        {
            board[x, y] = boardCell;
        }

        public Cell DoMove(BoardCell boardCell = BoardCell.First, int depth = DefaultDepth, int treeMaxWidth = DefaultWidth)
        {
            var move = DoMoveInternal(boardCell, depth, treeMaxWidth);
            board[move.X, move.Y] = boardCell;
            return move;
        }

        private Cell DoMoveInternal(BoardCell boardCell, int depth, int maxWidth)
        {
            sw.Start();
            var myLines = GetLines(boardCell);
            var oppLines = GetLines(boardCell.Opponent());
            if (!myLines.Any()) return FirstMoveCase();
            
            BoardState state;
            if (sw.Elapsed < TimeSpan.FromSeconds(5))
                state = new BoardState(myLines, oppLines, boardCell, depth, 0, maxWidth, board);
            else if (sw.Elapsed < TimeSpan.FromSeconds(6.6))
                state = new BoardState(myLines, oppLines, boardCell, depth - 1, 0, maxWidth, board);
            else
                state = new BoardState(myLines, oppLines, boardCell, depth - 2, 0, maxWidth, board);

            //if (GetBestFromNextMoves(state, out move)) return move;

            //todo we may want to remember history for perf improvement
            gameStates.Clear();
            Cell move;
            lastEstimate = AlphaBeta(state, int.MinValue, int.MaxValue, out move, null);
            sw.Stop();
            return move;
        }

        private int AlphaBeta(BoardState state, int alpha, int beta, out Cell move, GameState parent)
        {
            var nextCells = state.GetNextCells();
            move = null;
            int bestEstim = state.StartEstimate;
           
            foreach (var estimatedCell in EstimateCells(state, nextCells).Take(state.MaxWidth))
            {
                //TODO
                //for leaf case we probably don't even need to iterate - first (or last) one will be the result, since we order them.
                var cell = estimatedCell.Cell;
                Cell bestMove;

                board[cell.X, cell.Y] = state.MyCellType;
                var currEstim = estimatedCell.Estimate * state.Multiplier;

                var gameState = new GameState {BoardState = state.Clone(), Cell = cell, Estimate = currEstim};
                OnStateChanged(gameState, parent);
                int minMax;
                //make sure we terminate in case of win\loose
                //StraightFour comparison leds to invalid analysis. Can break on it only if oppent doesn't have broken/Blocked 4
                //but after bug fixed it seems to work.
                //TODO invesigate further
                if (state.IsTerminal || FiveInRow(estimatedCell.Estimate) || StraightFourAndOpponentDoesntHaveThreatOfFour(estimatedCell.Estimate))
                    minMax = currEstim;
                else
                    minMax = AlphaBeta(state.GetNextState(estimatedCell.MyLines), alpha, beta, out bestMove, gameState);

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
                gameStates.Add(gameState);
            }
            else
            {
                parentState.Children.Add(gameState);
            }
        }

        private Cell FirstMoveCase()
        {
            return board[7, 7] == BoardCell.None ? CellManager.Get(7, 7) : CellManager.Get(8, 8);
        }

        private int SumLines(List<Line> lines, BoardCell type)
        {
            var estims = lines.Select(l => l.Estimate(board, type)).ToList();
            var sum = estims.Sum(es => (int) es);
            int killerLines = 0;
            foreach (var line in lines)
            {
                if (ThreatOfThree(line.LineType) || (ThreatOfFour(line.LineType) && line.Count > 2)) killerLines++;
            }
            if (killerLines >= 2) return sum + (int)LineType.DoubleThreat;
            return sum;
        }

        private static bool FourCellLine(LineType lineType)
        {
            return ThreatOfFour(lineType) || lineType == LineType.StraightFour;
        }

        public static bool ThreatOfFour(LineType lineType)
        {
            return lineType == LineType.FourInRow || lineType == LineType.BrokenFourInRow;
        }

        public static bool ThreatOfThree(LineType lineType)
        {
            return lineType == LineType.ThreeInRow || lineType == LineType.BrokenThree || lineType == LineType.DoubleBrokenThree;
        }

        private IEnumerable<EstimatedCell> EstimateCells(BoardState state, IEnumerable<Cell> cells)
        {
            var list = new List<EstimatedCell>();

            foreach (var cell in cells)
            {
                board[cell.X, cell.Y] = state.MyCellType;
                var myNewLines = GetLinesByAddingCell(cell, state.MyLines);
               
                list.Add(new EstimatedCell(cell, myNewLines, Estimate(myNewLines, state.MyCellType, state.OppLines, state.OpponentCellType)));
                board[cell.X, cell.Y] = BoardCell.None;
            }
            return list.OrderByDescending(ec => ec.Estimate);
        }

        public int Estimate(List<Line> myLines, BoardCell myCellType, List<Line> oppLines, BoardCell opponentCellType)
        {
            var myEstim = SumLines(myLines, myCellType);
            if (FiveInRow(myEstim)) return myEstim;

            var oppEstim = SumLines(oppLines, opponentCellType);

            if (oppLines.Any(line => FourCellLine(line.LineType)))
                return -(int)LineType.FiveInRow;

            if (myLines.Any(line => FourCellLine(line.LineType)))
                return myEstim - oppEstim;

            if (oppLines.Any(line => ThreatOfThree(line.LineType)))
                return -(int)LineType.FiveInRow;

            return myEstim - oppEstim;
        }

        public List<Line> GetLines(BoardCell type)
        {
            var lines = new List<Line>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (board[i, j] != type)
                    {
                        continue;
                    }
                    FillLines(CellManager.Get(i, j), lines);
                }
            }
            var sorted = lines.ToList();
            sorted.Sort();
            return sorted;
        }

        public List<Line> GetLinesByAddingCell(Cell cell, List<Line> existingLines)
        {
            var lines = new List<Line>(existingLines.Select(existingLine => existingLine.Clone()));
            FillLines(cell, lines);
            return lines;
        }

        private static void FillLines(Cell cell, List<Line> lines)
        {
            var cellsUsedInAdding = new HashSet<Cell>();
            var addedToSomeLine = false;
            var usedLines = new List<Line>();
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                if (!line.JoinIfPossible(cell)) continue;
                cellsUsedInAdding.UnionWith(line);
                addedToSomeLine = true;
                MergeLines(lines, usedLines, line);
            }
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                foreach (var lineCell in line)
                {
                    if (lineCell.DistSqr(cell) > 2 || cellsUsedInAdding.Contains(lineCell)) continue;
                    var newLine = new Line(cell, lineCell);
                    if (!lines.Contains(newLine))
                    {
                        lines.Add(newLine);
                        addedToSomeLine = true;
                        cellsUsedInAdding.Add(lineCell);
                        MergeLines(lines, usedLines, newLine);
                    }
                }
            }
            if (!addedToSomeLine)
            {
                lines.Add(new Line(cell));
            }
        }

        private static void MergeLines(List<Line> lines, List<Line> usedLines, Line line)
        {
            Line mergedLine = null;
            foreach (var usedLine in usedLines.ToList())
            {
                if (!usedLine.HasSameDirection(line)) continue;

                mergedLine = usedLine.GetMergedLine(line);
                usedLines.Remove(usedLine);
                lines.Remove(usedLine);
                lines.Remove(line);
                lines.Add(mergedLine);
            }
            if (mergedLine == null) usedLines.Add(line);
        }

        public bool HasFiveInARow(BoardCell boardCell)
        {
            var lines = GetLines(boardCell);
            return lines.Any(l => l.Count >= 5);
        }
    }
}