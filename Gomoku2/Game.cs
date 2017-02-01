using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gomoku2
{
    public class Game
    {
        private int width;
        private int height;
        private BoardCell[,] board;
        private Stopwatch sw;

        public event Action<BoardState, Cell, int> StateChanged;

        public Game(int width, int height)
        {
            this.width = width;
            this.height = height;
            board = new BoardCell[width, height];
            sw = new Stopwatch();
        }

        public void DoOpponentMove(int x, int y)
        {
            board[x, y] = BoardCell.Second;
        }

        public void DoMyMove(int x, int y)
        {
            board[x, y] = BoardCell.First;
        }

        public Cell DoMove(int depth = 4, int treeMaxWidth = 16)
        {
            var move = DoMoveInternal(depth, treeMaxWidth);
            board[move.X, move.Y] = BoardCell.First;
            return move;
        }

        public Cell DoMoveInternal(int depth, int maxWidth)
        {
            sw.Start();
            var myLines = GetLines(BoardCell.First);
            var oppLines = GetLines(BoardCell.Second);
            if (!myLines.Any()) return FirstMoveCase();

            Cell move;
            if (WinInThisMove(myLines, BoardCell.First, out move)) return move;
            if (WinInThisMove(oppLines, BoardCell.Second, out move)) return move;

            BoardState state;
            if (sw.Elapsed < TimeSpan.FromSeconds(5))
                state = new BoardState(myLines, oppLines, BoardCell.First, depth, 0, maxWidth, board);
            else if (sw.Elapsed < TimeSpan.FromSeconds(6.6))
                state = new BoardState(myLines, oppLines, BoardCell.First, depth - 1, 0, maxWidth, board);
            else if (sw.Elapsed < TimeSpan.FromSeconds(9.9))
                state = new BoardState(myLines, oppLines, BoardCell.First, depth - 2, 0, maxWidth, board);
            else
                state = new BoardState(myLines, oppLines, BoardCell.First, depth - 2, 0, maxWidth, board);

            if (GetBestFromNextMoves(state, out move)) return move;

            AlphaBeta(state, int.MinValue, int.MaxValue, out move);
            sw.Stop();
            return move;
        }

        private int AlphaBeta(BoardState state, int alpha, int beta, out Cell move, List<Cell> cellsToCheck = null)
        {
            if (state.IsTerminal) return LeafCase(state, alpha, beta, out move);

            move = null;
            int bestEstim = state.StartEstimate;

            var nextCells = state.GetNextCells(cellsToCheck);
            var toTake = state.Depth == 4 ? 10 : state.MaxWidth;

            foreach (var tuple in EstimateCells(state, nextCells).Take(toTake))
            {
                var cell = tuple.Item1;
                Cell bestMove;

                board[cell.X, cell.Y] = state.Type;
                var currEstim = tuple.Item3 * (state.MovesFirst ? 1 : -1);
                OnStateChanged(state, cell, currEstim);
                int minMax;
                if (FiveInRow(tuple.Item3) || StraightFour(tuple.Item3)) minMax = currEstim;
                else minMax = AlphaBeta(state.GetNextState(tuple.Item2), alpha, beta, out bestMove);

                if (state.MovesFirst && minMax > bestEstim)
                {
                    bestEstim = minMax;
                    alpha = minMax;
                    move = cell;
                }
                if (!state.MovesFirst && minMax < bestEstim)
                {
                    bestEstim = minMax;
                    beta = minMax;
                    move = cell;
                }
                board[cell.X, cell.Y] = BoardCell.None;
                if (BreakOnFive(state.MovesFirst, minMax) || BreakOnStraightFour(state.MovesFirst, minMax)
                    || beta <= alpha) break;
            }
            return bestEstim;
        }

        private static bool FiveInRow(int estim)
        {
            return Math.Abs(estim) >= (int)LineType.FiveInRow / 2;
        }

        private static bool StraightFour(int estim)
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

        private void OnStateChanged(BoardState newState, Cell move, int estimate)
        {
            if (StateChanged != null) StateChanged(newState, move, estimate);
        }

        private int LeafCase(BoardState state, int alpha, int beta, out Cell move)
        {
            move = null;
            int bestEstim = state.StartEstimate;
            foreach (var cell in state.GetNextCells())
            {
                board[cell.X, cell.Y] = state.Type;
                var newLines = GetLinesByAddingCell(cell, state.MyLines);
                var myEstim = SumLines(newLines, state.Type);

                var oppEstim = EstimateOtherMove(state.OppLines, state.Type);
                var estim = (myEstim - oppEstim) * (state.MovesFirst ? 1 : -1);

                if (state.MovesFirst && estim > bestEstim)
                {
                    bestEstim = estim;
                    alpha = estim;
                    move = cell;
                    OnStateChanged(state, cell, estim);
                }
                if (!state.MovesFirst && estim < bestEstim)
                {
                    bestEstim = estim;
                    beta = estim;
                    move = cell;
                    OnStateChanged(state, cell, estim);
                }
                board[cell.X, cell.Y] = BoardCell.None;
                if (BreakOnFive(state.MovesFirst, estim) || BreakOnStraightFour(state.MovesFirst, estim)
                    || beta <= alpha) break;
            }
            return bestEstim;
        }

        private int EstimateOtherMove(List<Line> lines, BoardCell type)
        {
            return SumLines(lines, type == BoardCell.First ? BoardCell.Second : BoardCell.First);
        }

        private Cell FirstMoveCase()
        {
            return board[7, 7] == BoardCell.None ? CellManager.Get(7, 7) : CellManager.Get(8, 8);
        }

        private bool GetBestFromNextMoves(BoardState state, out Cell move)
        {
            move = null;
            MoveResult myThree = MoveResult.NotFound, oppThree = MoveResult.NotFound;
            if (OneMoveForwardFour(state, ref myThree, ref move)) return true;
            if (DefendFromOpenedThree(state, out move)) return true;
            if (OneMoveForwardFour(state.Switch(), ref oppThree, ref move)) return true;
            if (myThree.Found)
            {
                move = myThree.Move;
                return true;
            }
            if (oppThree.Found)
            {
                move = oppThree.Move;
                return true;
            }
            return false;
        }

        private bool OneMoveForwardFour(
            BoardState state,
            ref MoveResult myThree,
            ref Cell move)
        {
            foreach (var line in state.MyLines)
            {
                var moves = line.GetTwoNextCells(board);
                var res = FindBestMove(state, moves.Item1);
                if (LineFour(ref move, res)) return true;
                if (res.Found) myThree = res;
                res = FindBestMove(state, moves.Item2);
                if (LineFour(ref move, res)) return true;
                if (res.Found) myThree = res;
            }
            foreach (var cell in state.GetNearEmptyCells())
            {
                var res = FindBestMove(state, cell);
                if (LineFour(ref move, res)) return true;
                if (res.Found) myThree = res;
            }
            return false;
        }

        private static bool LineFour(ref Cell move, MoveResult res)
        {
            if (res.FoundFour)
            {
                move = res.Move;
                return true;
            }
            return false;
        }

        private bool DefendFromOpenedThree(BoardState state, out Cell move)
        {
            move = null;
            var cellsToDefend = new List<Cell>();
            foreach (var oppLine in state.OppLines)
            {
                Tuple<Cell, Cell> moves;
                if (ThreeInRowWithTwoPossibleMovesCase(oppLine, out moves))
                {
                    cellsToDefend.Add(moves.Item1);
                    cellsToDefend.Add(moves.Item2);
                }
            }
            if (cellsToDefend.Any())
            {
                AlphaBeta(state, int.MinValue, int.MaxValue, out move, cellsToDefend);
                return true;
            }
            return false;
        }

        public int SumLines(List<Line> lines, BoardCell type)
        {
            var estims = lines.Select(l => l.Estimate(board, type)).ToList();
            int killerLines = 0;
            foreach (var lineType in estims)
            {
                if (ThreatOfThree(lineType) || ThreatOfFour(lineType)) killerLines++;
            }
            if (killerLines >= 2) return (int)LineType.DoubleThreat + estims.Sum(es => (int)es);
            return estims.Sum(es => (int)es);
        }

        public static bool ThreatOfFour(LineType lineType)
        {
            return lineType == LineType.FourInRow || lineType == LineType.BrokenFourInRow;
        }

        private bool WinInThisMove(List<Line> lines, BoardCell type, out Cell move)
        {
            move = null;
            foreach (var line in lines)
            {
                var next = line.GetTwoNextCells(board);
                var res1 = CanDoWinMove(lines, next.Item1, type);
                if (res1.Found)
                {
                    move = res1.Move;
                    return true;
                }
                var res2 = CanDoWinMove(lines, next.Item2, type);
                if (res2.Found)
                {
                    move = res2.Move;
                    return true;
                }
            }
            return false;
        }

        private MoveResult CanDoWinMove(List<Line> existingLines, Cell proposedMove, BoardCell type)
        {
            if (proposedMove == null) return MoveResult.NotFound;
            board[proposedMove.X, proposedMove.Y] = type;

            var lines = GetLinesByAddingCell(proposedMove, existingLines);
            if (lines.Any(l => l.Count >= 5)) return RevertAndReturn(proposedMove, 5);

            board[proposedMove.X, proposedMove.Y] = BoardCell.None;
            return MoveResult.NotFound;
        }

        private MoveResult FindBestMove(BoardState state, Cell move)
        {
            if (move == null) return MoveResult.NotFound;
            // do move
            board[move.X, move.Y] = state.Type;

            var lines = GetLinesByAddingCell(move, state.MyLines);
            int lineOfThree = 0, lineOfFour = 0;
            foreach (var line in lines)
            {
                var lineType = line.Estimate(board, state.Type);
                if (lineType == LineType.StraightFour) return RevertAndReturn(move, 4);
                if ((ThreatOfFour(lineType)) && line.Contains(move))
                {
                    lineOfFour++;
                    continue;
                }
                if (ThreatOfThree(lineType) && line.Contains(move)) lineOfThree++;
            }
            if (lineOfFour >= 2) return RevertAndReturn(move, 4);
            if (lineOfFour >= 1 && lineOfThree >= 1) return RevertAndReturn(move, 4);

            if (lineOfThree >= 2 && !state.OppLines.Any(l => ThreatOfThree(l.Estimate(board, state.Type))))
                return RevertAndReturn(move, 3);
            // revert
            board[move.X, move.Y] = BoardCell.None;
            return MoveResult.NotFound;
        }

        public static bool ThreatOfThree(LineType lineType)
        {
            return lineType == LineType.ThreeInRow || lineType == LineType.BrokenThree;
        }

        private MoveResult RevertAndReturn(Cell proposedMove, int lenght)
        {
            board[proposedMove.X, proposedMove.Y] = BoardCell.None;
            return new MoveResult(proposedMove, lenght, true);
        }

        private bool ThreeInRowWithTwoPossibleMovesCase(Line oppLine, out Tuple<Cell, Cell> moves)
        {
            moves = null;
            if (oppLine.Count != 3)
            {
                return false;
            }
            var oppBest = oppLine.GetTwoNextCells(board);
            if (oppBest.Item1 != null && oppBest.Item2 != null)
            {
                moves = oppBest;
                return true;
            }
            return false;
        }

        private List<Tuple<Cell, List<Line>, int>> EstimateCells(
            BoardState state, IEnumerable<Cell> cells)
        {
            var list = new List<Tuple<Cell, List<Line>, int>>();

            foreach (var cell in cells)
            {
                board[cell.X, cell.Y] = state.Type;
                var newLines = GetLinesByAddingCell(cell, state.MyLines);
                var oppEstim = EstimateOtherMove(state.OppLines, state.Type);
                list.Add(new Tuple<Cell, List<Line>, int>(
                    cell, newLines, SumLines(newLines, state.Type) - oppEstim));
                board[cell.X, cell.Y] = BoardCell.None;

            }
            list.Sort(Comparison);
            return list;
        }





        private int Comparison(Tuple<Cell, List<Line>, int> t1, Tuple<Cell, List<Line>, int> t2)
        {
            var comp = t2.Item3.CompareTo(t1.Item3);
            if (comp != 0) return comp;

            return (t1.Item1.X * 15 + t1.Item1.Y).CompareTo(t2.Item1.X * 15 + t2.Item1.Y);
        }

        public static IEnumerable<Cell> GetAdjustmentCells(BoardCell[,] board, Cell startCell)
        {
            var x = startCell.X;
            var y = startCell.Y;
            var cell = CellManager.Get(x + 1, y + 1);
            if (cell.IsEmpty(board))
            {
                yield return cell;
            }
            cell = CellManager.Get(x + 1, y - 1);
            if (cell.IsEmpty(board))
            {
                yield return cell;
            }
            cell = CellManager.Get(x - 1, y + 1);
            if (cell.IsEmpty(board))
            {
                yield return cell;
            }
            cell = CellManager.Get(x - 1, y - 1);
            if (cell.IsEmpty(board))
            {
                yield return cell;
            }
            //////////////////////

            cell = CellManager.Get(x + 1, y);
            if (cell.IsEmpty(board))
            {
                yield return cell;
            }
            cell = CellManager.Get(x - 1, y);
            if (cell.IsEmpty(board))
            {
                yield return cell;
            }
            cell = CellManager.Get(x, y + 1);
            if (cell.IsEmpty(board))
            {
                yield return cell;
            }
            cell = CellManager.Get(x, y - 1);
            if (cell.IsEmpty(board))
            {
                yield return cell;
            }
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

        private void FillLines(Cell cell, List<Line> lines)
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

        public void AppendBoardLine(int i, string line)
        {
            for (int j = 0; j < line.Length; j++)
            {
                board[i, j] = (BoardCell)Enum.Parse(typeof(BoardCell), line[j].ToString());
            }
        }

        public void AppendBoardLineXO(int i, string line)
        {
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j].ToString() == " ") board[i, j] = BoardCell.None;
                if (line[j].ToString() == "X") board[i, j] = BoardCell.First;
                if (line[j].ToString() == "O") board[i, j] = BoardCell.Second;
            }
        }

        public BoardState GetState()
        {
            return new BoardState(GetLines(BoardCell.First), GetLines(BoardCell.Second), BoardCell.First, 0, 0, 0, board);
        }
    }
}