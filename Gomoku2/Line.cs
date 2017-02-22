using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gomoku2
{
    public class Line : IComparable<Line>, IEnumerable<Cell>, IEquatable<Line>
    {
        private readonly List<Cell> line = new List<Cell>();
        private readonly List<Cell> priorityCells = new List<Cell>();
        private LineType lineType;
        private BoardCell owner;
        //these cells are not null only if corresponding space is empty
        private Cell next, prev;
        //these cells can be either empty or "My" type
        private Cell nextNext, prevPrev, nextNextNext, prevPrevPrev;

        public Line()
        {
        }

        public Line(Cell cell, BoardCell owner)
        {
            line.Add(cell);
            Start = cell;
            End = cell;
            this.owner = owner;
            Direction = CellManager.Get(0, 0);
        }

        public Line(Cell cell1, Cell cell2, BoardCell[,] board, BoardCell owner)
        {
            line.Add(cell1);
            line.Add(cell2);
            this.owner = owner;
            CalcProps(board);
        }

        private void AddCell(Cell cell, BoardCell[,] board)
        {
            line.Add(cell);
            CalcProps(board);
        }

        private void CalcProps(BoardCell[,] board)
        {
            CalcStart();
            CalcEnd();
            CalcDirection();
            CalcNextAndPrev(board);
        }

        private void CalcStart()
        {
            var maxY = line.Max(c => c.Y);
            var cells = line.Where(c => c.Y == maxY).ToList();
            if (cells.Count == 1)
            {
                Start = cells[0];
                return;
            }
            var maxX = line.Max(c => c.X);
            Start = cells.First(c => c.X == maxX);
        }

        private void CalcEnd()
        {
            var minY = line.Min(c => c.Y);
            var cells = line.Where(c => c.Y == minY).ToList();
            if (cells.Count == 1)
            {
                End = cells[0];
                return;
            }
            var minX = line.Min(c => c.X);
            End = cells.First(c => c.X == minX);
        }
        private void CalcDirection()
        {
            var dir = Start - End;
            Direction = dir.Normalize();
        }

        private void CalcNextAndPrev(BoardCell[,] board)
        {
            if (Start == End)
                return;

            CalcNext(board);
            CalcPrev(board);
        }

        private void CalcNext(BoardCell[,] board)
        {
            next = NextCell(1);
            if (next.IsEmpty(board))
            {
                nextNext = NextCell(2);
                if (!nextNext.IsEmpty(board)) nextNext = null;
            }
            else
                next = null;
        }

        private void CalcPrev(BoardCell[,] board)
        {
            prev = NextCell(-1);
            if (prev.IsEmpty(board))
            {
                prevPrev = NextCell(-2);
                if (!prevPrev.IsEmpty(board)) prevPrev = null;
            }
            else
                prev = null;
        }

        private Cell Start { get; set; }

        private Cell End { get; set; }

        public int Count { get { return line.Count; } }

        public LineType LineType { get { return lineType; } }

        public List<Cell> PriorityCells { get { return priorityCells; } }

        public IEnumerable<Cell> GetNextCells(bool includeNextNext)
        {
            if (next != null) yield return next;
            if (prev != null) yield return prev;
            if (!includeNextNext) yield break;

            if (nextNext != null) yield return nextNext;
            if (prevPrev != null) yield return prevPrev;
        }

        public LineType Estimate(BoardCell[,] board)
        {
            priorityCells.Clear();
            CalcNext(board);
            CalcPrev(board);
            return lineType = GetEstimate(board);
        }

        private bool IsOpenFromBothSides
        {
            get { return next != null && prev != null; }
        }

        private LineType GetEstimate(BoardCell[,] board)
        {
            int space;
            switch (Count)
            {
                case 5:
                    return LineType.FiveInRow;
                case 4:
                    space = OpenSpace(board);
                    if (IsOpenFromBothSides) return LineType.StraightFour;
                    if (space == 1) return LineType.FourInRow;
                    return LineType.DeadFour;
                case 3:
                    space = OpenSpace(board);
                    var nextThreeSpace = NextSpace(board, true);
                    if (nextThreeSpace == 2) return LineType.StraightFour;
                    if (nextThreeSpace == 1) return LineType.BrokenFourInRow;
                    if (IsOpenFromBothSides) return LineType.ThreeInRow;
                    if (space == 1 && priorityCells.Count == 2) return LineType.BlokedThree;
                    return LineType.DeadThree;
                case 2:
                    space = OpenSpace(board);
                    var nextSpace = NextSpace(board, false);
                    if (HasBrokenFour(board))
                        return LineType.BrokenFourInRow;
                    if (IsOpenFromBothSides)
                    {
                        if (nextSpace == 2)
                            return LineType.DoubleBrokenThree;
                        if (nextSpace == 1)
                            return LineType.BrokenThree;
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

        //todo update cloning accordingly
        public Line Clone()
        {
            var newLine = new Line();
            newLine.line.AddRange(line);

            newLine.Start = Start;
            newLine.End = End;
            newLine.Direction = Direction;
            newLine.owner = owner;
            return newLine;
        }

        public bool HasSameDirection(Line other)
        {
            return Direction == other.Direction;
        }

        public bool JoinIfPossible(Cell cell, BoardCell[,] board)
        {
            if (line.Count == 1 && Start.DistSqr(cell) <= 2)
            {
                AddCell(cell, board);
                return true;
            }

            if (NextCell(1) == cell)
            {
                line.Add(cell);
                Start = cell;
                CalcNext(board);
                return true;
            }
            if (NextCell(-1) == cell)
            {
                line.Add(cell);
                End = cell;
                CalcPrev(board);
                return true;
            }

            return false;
        }

        private int OpenSpace(BoardCell[,] board)
        {
            if (next != null) priorityCells.Add(next);
            if (prev != null) priorityCells.Add(prev);
            return priorityCells.Count;
        }

        private Cell NextCell(int i)
        {
            if (i >= 0)
                return Start + i*Direction;
            return End + i*Direction;
        }

        private int NextSpace(BoardCell[,] board, bool addNextNextCellAsPriority)
        {
            int space = 0;
            if (next != null)
            {
                //todo use nextnext
                var nextNextCell = NextCell(2);
                if (CellIsDesiredType(board, nextNextCell, owner)) space++;
                else if (addNextNextCellAsPriority && nextNext != null)
                    priorityCells.Add(nextNext);
            }
            if (prev != null)
            {
                var nextNextCell = NextCell(-2);
                if (CellIsDesiredType(board, nextNextCell, owner)) space++;
                else if (addNextNextCellAsPriority && prevPrev != null)
                    priorityCells.Add(prevPrev);
            }
            return space;
        }

        private bool HasBrokenFour(BoardCell[,] board)
        {
            if (next != null)
            {
                if (CellIsDesiredType(board, NextCell(2), owner) && CellIsDesiredType(board, NextCell(3), owner))
                {
                    priorityCells.Clear();
                    priorityCells.Add(next);
                    return true;
                }
            }
            if (prev != null)
            {
                if (CellIsDesiredType(board, NextCell(-2), owner) && CellIsDesiredType(board, NextCell(-3), owner))
                {
                    priorityCells.Clear();
                    priorityCells.Add(prev);
                    return true;
                }
            }
            return false;
        }

        private static bool CellIsDesiredType(BoardCell[,] board, Cell cell, BoardCell type)
        {
            return CellIsDesiredType(board, cell.X, cell.Y, type);
        }

        private static bool CellIsDesiredType(BoardCell[,] board, int x, int y, BoardCell cellType)
        {
            return x >= 0 && x < 15 && y >= 0 && y < 15 && board[x, y] == cellType;
        }

        public int CompareTo(Line other)
        {
            return -lineType.CompareTo(other.lineType);
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
            return string.Format("{2} S {0} E {1}", Start, End, LineType);
        }

        public Line GetMergedLine(Line otherLine, BoardCell[,] board)
        {
            var cells = new HashSet<Cell>();
            cells.UnionWith(line);
            cells.UnionWith(otherLine);
            var mergedLine = new Line();
            mergedLine.line.AddRange(cells);
            mergedLine.owner = owner;
            //TODO maybe we don't need to recalculate all props
            mergedLine.CalcProps(board);
            return mergedLine;
        }
    }
}