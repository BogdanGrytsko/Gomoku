using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gomoku2
{
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
            var cell = Start.GetAdjustmentEmptyCells(board).FirstOrDefault();
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