using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gomoku
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game(15, 15);
            bool skip = Console.ReadLine().Trim() == "First";
            int x, y;
            while (true)
            {
                if (skip == false)
                {
                    string[] input = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    x = Int32.Parse(input[0]);
                    y = Int32.Parse(input[1]);
                    game.DoOpponentMove(x, y);
                }
                skip = false;
                var move = game.DoMove();
                x = move.X;
                y = move.Y;
                Console.WriteLine("{0} {1}", x, y);
                Console.Out.Flush();
            }
        }
    }

    public enum BoardCell
    {
        None = 0,
        First = 1,
        Second = 2
    }

    public class BoardState
    {
        public List<Line> MyLines { get; private set; }

        public List<Line> OppLines { get; private set; }

        public BoardCell Type { get; private set; }

        public int Depth { get; private set; }

        public int MaxWidth { get; private set; }

        public BoardCell[,] Board { get; private set; }

        public BoardState(
            List<Line> myLines, List<Line> oppLines, BoardCell type, int depth, int minDepth, int maxWidth, BoardCell[,] board)
        {
            MyLines = myLines;
            OppLines = oppLines;
            Type = type;
            Depth = depth;
            Board = board;
            MaxWidth = maxWidth;
            MinDepth = minDepth;
        }

        public bool MovesFirst
        {
            get
            {
                return Type == BoardCell.First;
            }
        }

        public int StartEstimate
        {
            get
            {
                return MovesFirst ? int.MinValue : int.MaxValue;
            }
        }

        public BoardCell Opponent
        {
            get
            {
                return MovesFirst ? BoardCell.Second : BoardCell.First;
            }
        }

        public BoardState GetNextState(List<Line> myNewLines)
        {
            return new BoardState(OppLines, myNewLines, Opponent, Depth - 1, MinDepth, MaxWidth, Board);
        }

        public IEnumerable<Cell> GetNextCells(List<Cell> cellsToCheck = null)
        {
            IEnumerable<Cell> nextCells;
            var immidiateThreatCells = GetImmidiateThreatCells().ToList();
            if (immidiateThreatCells.Any())
            {
                nextCells = immidiateThreatCells;
                ////if (MinDepth >= -4)
                MinDepth--;
            }
            else nextCells = cellsToCheck ?? GetNearEmptyCells();
            return nextCells;
        }

        private int MinDepth { get; set; }

        public bool IsTerminal
        {
            get
            {
                return Depth == MinDepth;
            }
        }

        private IEnumerable<Cell> GetImmidiateThreatCells()
        {
            var threatOfFour = OppLines.Where(l => Game.ThreatOfFour(l.Estimate(Board, Opponent))).ToList();
            foreach (var cell in GetThreatCells(threatOfFour)) yield return cell;

            ////if (threatOfFour.Any()) yield break;
            ////var oppThreatOfThree = OppLines.Where(l => Game.ThreatOfThree(l.Estimate(Board, Opponent))).ToList();
            ////if (oppThreatOfThree.Any())
            ////{
            ////    foreach (var cell in GetThreatCells(oppThreatOfThree)) yield return cell;

            ////    var myThreatOfFour = MyLines.Where(l => l.Estimate(Board, Type) == LineType.BlokedThree);
            ////    foreach (var cell in GetThreatCells(myThreatOfFour)) yield return cell;

            ////    var myThreatOfThree = MyLines.Where(l => Game.ThreatOfThree(l.Estimate(Board, Type)));
            ////    foreach (var cell in GetThreatCells(myThreatOfThree)) yield return cell;
            ////}
        }

        private IEnumerable<Cell> GetThreatCells(IEnumerable<Line> lines)
        {
            foreach (var line in lines)
            {
                var moves = line.GetTwoNextCells(Board);
                if (moves.Item1 != null) yield return moves.Item1;
                if (moves.Item2 != null) yield return moves.Item2;
            }
        }

        public IEnumerable<Cell> GetNearEmptyCells()
        {
            var set = new HashSet<Cell>();
            for (int x = 0; x < 15; ++x)
            {
                for (int y = 0; y < 15; ++y)
                {
                    if (Board[x, y] != BoardCell.None)
                        set.UnionWith(Game.GetAdjustmentCells(Board, CellManager.Get(x, y)));
                }
            }
            set.UnionWith(GetNextNextCells(MyLines));
            set.UnionWith(GetNextNextCells(OppLines));
            return set.OrderBy(c => c.X * 15 + c.Y);
        }

        private IEnumerable<Cell> GetNextNextCells(List<Line> lines)
        {
            foreach (var line in lines)
            {
                var close = line.GetTwoNextCells(Board);
                var next = line.GetTwoNextNextCells(Board);
                if (close.Item1 != null && next.Item1 != null) yield return next.Item1;
                if (close.Item2 != null && next.Item2 != null) yield return next.Item2;
            }
        }

        public BoardState Clone()
        {
            var width = Board.GetLength(0);
            var height = Board.GetLength(1);
            var boardCopy = new BoardCell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    boardCopy[i, j] = Board[i, j];
                }
            }
            return new BoardState(MyLines, OppLines, Type, Depth, MinDepth, MaxWidth, boardCopy);
        }

        public BoardState Switch()
        {
            return new BoardState(OppLines, MyLines, Opponent, Depth, MinDepth, MaxWidth, Board);
        }
    }

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

        public Cell DoMove()
        {
            return DoMove(4, 16);
        }

        public Cell DoMove(int depth, int treeMaxWidth)
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

        public bool HasFiveInARow(BoardCell boardCell)
        {
            var lines = GetLines(boardCell);
            return lines.Any(l => l.Count >= 5);
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

    public class MoveResult
    {
        public MoveResult(Cell move, int lenght, bool found)
        {
            Move = move;
            Lenght = lenght;
            Found = found;
        }

        public Cell Move { get; set; }

        public int Lenght { get; set; }

        public bool Found { get; set; }

        public bool FoundFour
        {
            get
            {
                return Found && Lenght >= 4;
            }
        }

        public static MoveResult NotFound
        {
            get
            {
                return new MoveResult(null, 0, false);
            }
        }
    }

    public class Cell : IEquatable<Cell>
    {
        public Cell()
        {
        }

        public Cell(int i, int j)
        {
            X = i;
            Y = j;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public BoardCell BoardCell { get; set; }

        public int DistSqr(Cell other)
        {
            var xx = X - other.X;
            var yy = Y - other.Y;
            return xx * xx + yy * yy;
        }

        public Cell Normalize()
        {
            int nx = 0, ny = 0;
            if (X > 0) nx = 1;
            if (X < 0) nx = -1;
            if (Y > 0) ny = 1;
            if (Y < 0) ny = -1;
            return CellManager.Get(nx, ny);
        }

        public static Cell operator +(Cell first, Cell second)
        {
            return CellManager.Get(first.X + second.X, first.Y + second.Y);
        }

        public static Cell operator -(Cell first, Cell second)
        {
            return CellManager.Get(first.X - second.X, first.Y - second.Y);
        }

        public static Cell operator *(int a, Cell cell)
        {
            return CellManager.Get(cell.X * a, cell.Y * a);
        }

        public static bool operator ==(Cell first, Cell second)
        {
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }
            if (ReferenceEquals(first, null))
            {
                return false;
            }
            return first.Equals(second);
        }

        public static bool operator !=(Cell first, Cell second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Cell)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public bool IsEmpty(BoardCell[,] board)
        {
            return X >= 0 && X < 15 && Y >= 0 && Y < 15 && board[X, Y] == BoardCell.None;
        }

        public bool Equals(Cell other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", X, Y);
        }
    }

    public class CellManager
    {
        private static readonly Dictionary<int, Cell> cells = new Dictionary<int, Cell>();

        public static Cell Get(int x, int y)
        {
            Cell cell;
            if (cells.TryGetValue(x * 225 + y, out cell)) return cell;
            cell = new Cell(x, y);
            cells.Add(x * 225 + y, cell);
            return cell;
        }
    }

    public enum LineType
    {
        FiveInRow = 10000,
        StraightFour = 3000,
        DoubleThreat = 1000,
        FourInRow = 30,
        BrokenFourInRow = 23,
        ThreeInRow = 20,
        BrokenThree = 15,
        TwoInRow = 6,
        DeadFour = 5,
        BlokedThree = 4,
        BlockedTwo = 3,
        DeadThree = 3,
        DeadTwo = 2,
        SingleMark = 1,
        Useless = 0
    }

    public class Line : IComparable<Line>, IEnumerable<Cell>, IEquatable<Line>
    {
        private List<Cell> line = new List<Cell>();

        public Line()
        {
        }

        public Line(Cell cell)
        {
            AddCell(cell);
        }

        public Line(Cell cell1, Cell cell2)
        {
            line.Add(cell1);
            line.Add(cell2);
            CalcProps();
        }

        public void AddCell(Cell cell)
        {
            line.Add(cell);
            CalcProps();
        }

        private void CalcProps()
        {
            CalcStart();
            CalcEnd();
            CalcDirection();
        }

        private void CalcDirection()
        {
            var dir = Start - End;
            Direction = dir.Normalize();
        }

        private void CalcEnd()
        {
            var minY = line.Min(c => c.Y);
            var cells = line.Where(c => c.Y == minY).ToList();
            if (cells.Count() == 1)
            {
                End = cells[0];
                return;
            }
            var minX = line.Min(c => c.X);
            End = cells.First(c => c.X == minX);
        }

        private void CalcStart()
        {
            var maxY = line.Max(c => c.Y);
            var cells = line.Where(c => c.Y == maxY).ToList();
            if (cells.Count() == 1)
            {
                Start = cells[0];
                return;
            }
            var maxX = line.Max(c => c.X);
            Start = cells.First(c => c.X == maxX);
        }

        private Cell Start { get; set; }

        private Cell End { get; set; }

        public int Count
        {
            get
            {
                return line.Count;
            }
        }

        public LineType Estimate(BoardCell[,] board, BoardCell type)
        {
            int space;
            switch (Count)
            {
                case 5:
                    return LineType.FiveInRow;
                case 4:
                    space = OpenSpace(board);
                    if (space == 2) return LineType.StraightFour;
                    if (space == 1) return LineType.FourInRow;
                    return LineType.DeadFour;
                case 3:
                    space = OpenSpace(board);
                    var nextThreeSpace = NextSpace(board, type);
                    if (nextThreeSpace == 2) return LineType.StraightFour;
                    if (nextThreeSpace == 1) return LineType.BrokenFourInRow;
                    if (space == 2) return LineType.ThreeInRow;
                    if (space == 1) return LineType.BlokedThree;
                    return LineType.DeadThree;
                case 2:
                    space = OpenSpace(board);
                    if (space == 2)
                    {
                        var nextSpace = NextSpace(board, type);
                        if (nextSpace != 0)
                        {
                            return LineType.BrokenThree;
                        }
                        return LineType.TwoInRow;
                    }
                    if (space == 1) return LineType.BlockedTwo;
                    return LineType.DeadTwo;
                case 1:
                    return LineType.SingleMark;
            }
            return LineType.Useless;
        }

        private Cell Direction { get; set; }

        public Line Clone()
        {
            var newLine = new Line();
            newLine.line.AddRange(line);

            newLine.Start = Start;
            newLine.End = End;
            newLine.Direction = Direction;
            return newLine;
        }

        public bool HasSameDirection(Line other)
        {
            return Direction == other.Direction;
        }

        public bool JoinIfPossible(Cell cell)
        {
            if (line.Count == 1 && line[0].DistSqr(cell) <= 2 && line[0] != cell)
            {
                AddCell(cell);
                return true;
            }

            if (Start + Direction == cell)
            {
                line.Add(cell);
                Start = cell;
                return true;
            }
            if (End - Direction == cell)
            {
                line.Add(cell);
                End = cell;
                return true;
            }

            return false;
        }

        public Tuple<Cell, Cell> GetTwoNextCells(BoardCell[,] board)
        {
            if (Start == End)
            {
                Cell move;
                if (LineOfOneCase(board, out move)) return new Tuple<Cell, Cell>(move, move);
            }

            Cell first = null;
            var start = Start + Direction;
            if (start.IsEmpty(board))
            {
                first = start;
            }

            Cell second = null;
            var end = End - Direction;
            if (end.IsEmpty(board))
            {
                second = end;
            }

            return new Tuple<Cell, Cell>(first, second);
        }

        public Tuple<Cell, Cell> GetTwoNextNextCells(BoardCell[,] board)
        {
            if (Start == End)
            {
                return new Tuple<Cell, Cell>(null, null);
            }

            Cell first = null;
            var start = Start + 2 * Direction;
            if (start.IsEmpty(board))
            {
                first = start;
            }

            Cell second = null;
            var end = End - 2 * Direction;
            if (end.IsEmpty(board))
            {
                second = end;
            }

            return new Tuple<Cell, Cell>(first, second);
        }

        public int OpenSpace(BoardCell[,] board)
        {
            int opened = 0;
            if (CellIsDesiredType(board, Start.X + Direction.X, Start.Y + Direction.Y, BoardCell.None)) opened++;
            if (CellIsDesiredType(board, End.X - Direction.X, End.Y - Direction.Y, BoardCell.None)) opened++;
            return opened;
        }

        public int NextSpace(BoardCell[,] board, BoardCell type)
        {
            // redundant check for case of 2
            int space = 0;
            if (CellIsDesiredType(board, Start.X + Direction.X, Start.Y + Direction.Y, BoardCell.None))
            {
                if (CellIsDesiredType(board, Start.X + 2 * Direction.X, Start.Y + 2 * Direction.Y, type)) space++;
            }
            if (CellIsDesiredType(board, End.X - Direction.X, End.Y - Direction.Y, BoardCell.None))
            {
                if (CellIsDesiredType(board, End.X - 2 * Direction.X, End.Y - 2 * Direction.Y, type)) space++;
            }
            return space;
        }

        private static bool CellIsDesiredType(BoardCell[,] board, int x, int y, BoardCell type)
        {
            return x >= 0 && x < 15 && y >= 0 && y < 15 && board[x, y] == type;
        }

        private bool LineOfOneCase(BoardCell[,] board, out Cell findNextCell1)
        {
            findNextCell1 = null;
            var cell = Game.GetAdjustmentCells(board, Start).FirstOrDefault();
            if (cell != null)
            {
                findNextCell1 = cell;
                return true;
            }
            return false;
        }

        public int CompareTo(Line other)
        {
            return -Count.CompareTo(other.Count);
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            foreach (var cell in line)
            {
                yield return cell;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(Line other)
        {
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        bool IEquatable<Line>.Equals(Line other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            var other = (Line)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("S {0} E {1}", Start, End);
        }

        public Line GetMergedLine(Line otherLine)
        {
            var cells = new HashSet<Cell>();
            cells.UnionWith(line);
            cells.UnionWith(otherLine);
            var mergedLine = new Line();
            mergedLine.line.AddRange(cells);
            mergedLine.CalcProps();
            return mergedLine;
        }

        public bool Contains(Cell cell)
        {
            return line.Any(t => t == cell);
        }
    }
}
