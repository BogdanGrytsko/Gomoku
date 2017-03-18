using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineAnalyzer;

namespace Gomoku2.LineCore
{
    public class Line : IComparable<Line>, IEnumerable<Cell>, IEquatable<Line>
    {
        private readonly List<Cell> line = new List<Cell>();
        private LineType lineType;
        private BoardCell owner;
        private Cell next, prev, nextNext, prevPrev, nextNextNext, prevPrevPrev, middle1;
        private List<Cell> priorityCells;

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
            next = prev = CellManager.Get(-1, -1);
            next.BoardCell = BoardCell.Invalid;
            lineType = LineType.SingleMark;
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
            SetEstimate();
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
            next = NextCellWithType(1, board);
            nextNext = NextCellWithType(2, board);
            nextNextNext = NextCellWithType(3, board);
        }

        private void CalcPrev(BoardCell[,] board)
        {
            prev = NextCellWithType(-1, board);
            prevPrev = NextCellWithType(-2, board);
            prevPrevPrev = NextCellWithType(-3, board);
        }

        private Cell Start { get; set; }

        private Cell End { get; set; }

        public int Count { get { return line.Count; } }

        public LineType LineType { get { return lineType; } }

        public IEnumerable<Cell> HighPriorityCells
        {
            get
            {
                if (lineType.IsBrokenFourInRow())
                {
                    foreach (var cell in priorityCells) yield return cell;
                    yield break;
                }
                switch (Count)
                {
                    case 4:
                        foreach (var cell in GetNextCells(false)) yield return cell;
                        break;
                    case 3:
                        var includeNextNext = lineType.IsBlokedThree();
                        foreach (var cell in GetNextCells(includeNextNext)) yield return cell;
                        break;
                    case 2:
                        // XX  X OR OXX  X
                        if (lineType.IsLongBrokenThree() || lineType.IsLongBlockedThree())
                        {
                            foreach (var cell in priorityCells) yield return cell;
                            break;
                        }
                        //OXX X 
                        //todo investigate why need to have check on null and remove it.
                        if (lineType.IsBlokedThree() && priorityCells != null)
                        {
                            foreach (var cell in priorityCells) yield return cell;
                            break;
                        }
                        if (lineType.IsBrokenThree())
                        {
                            foreach (var cell in priorityCells) yield return cell;
                            break;
                        }
                        //TwoInRow should return next cell only of next next is open
                        if (lineType.IsTwoInRow())
                        {
                            if (priorityCells != null)
                            {
                                foreach (var cell in priorityCells) yield return cell;
                                //todo return middle cell if any
                                break;
                            }
                            foreach (var cell in GetNextCells(true)) yield return cell;
                            break;
                        }
                        break;
                    case 1:
                        if (lineType.IsTwoInRow())
                        {
                            foreach (var cell in GetNextCells(false)) yield return cell;
                            yield return middle1;
                            break;
                        }
                        break;
                }
            }
        }

        public IEnumerable<Cell> GetNextCells(bool includeNextNext)
        {
            if (next.IsEmpty)
            {
                yield return next;
                if (includeNextNext && nextNext.IsEmpty)
                    yield return nextNext;
            }
            if (prev.IsEmpty)
            {
                yield return prev;
                if (includeNextNext && prevPrev.IsEmpty)
                    yield return prevPrev;
            }
        }

        public LineType Estimate(BoardCell[,] board)
        {
            CalcNextAndPrev(board);
            return lineType = GetEstimate();
        }

        public void SetEstimate()
        {
            lineType = GetEstimate();
        }

        private bool IsOpenFromBothSides
        {
            get { return next.IsEmpty && prev.IsEmpty; }
        }

        private bool IsDead
        {
            get { return !next.IsEmpty && !prev.IsEmpty; }
        }

        private List<Cell> NextCells
        {
            get { return new List<Cell> {next, nextNext, nextNextNext}; }
        }

        private List<Cell> PrevCells
        {
            get { return new List<Cell> { prev, prevPrev, prevPrevPrev }; }
        }

        private LineType GetEstimate()
        {
            priorityCells = null;
            switch (Count)
            {
                case 5:
                    return LineType.FiveInRow;
                case 4:
                    if (IsDead) return LineType.DeadFour;
                    if (IsOpenFromBothSides) return LineType.StraightFour;
                    return LineType.FourInRow;
                case 3:
                    return ThreeInRowAnalysis();
                case 2:
                    return TwoInRowAnalysis();
                case 1:
                    if (middle1 != null)
                        return BrokenTwoInRowAnalsis();
                    return LineType.SingleMark;
            }
            return LineType.Useless;
        }

        private LineType BrokenTwoInRowAnalsis()
        {
            if (IsDead) return LineType.DeadTwo;
            var analyzer = new BrokenTwoInRowAnalyzer(NextCells, PrevCells, owner, middle1);
            return DoAnalysis(analyzer);
        }

        private LineType ThreeInRowAnalysis()
        {
            if (IsDead) return LineType.DeadThree;
            var analyzer = new ThreeInRowAnalyzer(NextCells, PrevCells, owner);
            return DoAnalysis(analyzer);
        }

        private LineType DoAnalysis(AnalyzerBase analyzer)
        {
            if (!next.IsEmpty && prev.IsEmpty)
                return analyzer.PrevOpened(ref priorityCells);
            if (next.IsEmpty && !prev.IsEmpty)
                return analyzer.NextOpened(ref priorityCells);
            return analyzer.TwoSidesOpened(ref priorityCells);
        }

        private LineType TwoInRowAnalysis()
        {
            if (IsDead) return LineType.DeadTwo;
            var analyzer = new TwoInRowAnalyzer(NextCells, PrevCells, owner);
            return DoAnalysis(analyzer);
        }

        private Cell Direction { get; set; }

        public Line Clone()
        {
            //todo MemberwiseClone doesn't work. Investigate
            //return (Line)MemberwiseClone();
            var newLine = new Line();
            newLine.line.AddRange(line);

            newLine.Start = Start;
            newLine.End = End;
            newLine.Direction = Direction;
            newLine.owner = owner;

            newLine.lineType = lineType;
            newLine.next = next;
            newLine.nextNext = nextNext;
            newLine.nextNextNext = nextNextNext;
            newLine.prev = prev;
            newLine.prevPrev = prevPrev;
            newLine.prevPrevPrev = prevPrevPrev;
            newLine.middle1 = middle1;

            newLine.priorityCells = priorityCells;

            return newLine;
        }

        public bool HasSameDirection(Line other)
        {
            return Direction == other.Direction;
        }

        public static bool IsBrokenTwoDistance(int dist)
        {
            return dist == 4 || dist == 16;
        }

        public bool JoinIfPossible(Cell cell, BoardCell[,] board)
        {
            if (line.Count == 1 && middle1 == null)
            {
                var dist = Start.DistSqr(cell);
                if (dist <= 2)
                {
                    AddCell(cell, board);
                    return true;
                }
                if (IsBrokenTwoDistance(dist))
                {
                    var dir = (Start - cell).Normalize();
                    var tmpNext = cell + dir;
                    var tmpNextNext = cell - dir;
                    if (tmpNext.IsEmptyWithBoard(board) && !tmpNextNext.IsType(board, owner))
                    {
                        middle1 = tmpNext;
                        AddCell(cell, board);
                        line.Remove(cell);
                        return true;
                    }
                }
            }

            if (NextCell(1) == cell)
            {
                if (middle1 != null)
                {
                    End = Start;
                    middle1 = null;
                }

                line.Add(cell);
                Start = cell;
                CalcNext(board);
                SetEstimate();
                return true;
            }
            if (NextCell(-1) == cell)
            {
                if (middle1 != null)
                {
                    Start = End;
                    middle1 = null;
                }

                line.Add(cell);
                End = cell;
                CalcPrev(board);
                SetEstimate();
                return true;
            }
            if (NextCell(2) == cell || NextCell(3) == cell)
            {
                CalcNext(board);
                SetEstimate();
            }
            if (NextCell(-2) == cell || NextCell(-3) == cell)
            {
                CalcPrev(board);
                SetEstimate();
            }

            return false;
        }

        private Cell NextCell(int i)
        {
            if (i >= 0)
                return Start + i*Direction;
            return End + i*Direction;
        }

        private Cell NextCellWithType(int i, BoardCell[,] board)
        {
            var cell = NextCell(i);
            cell.BoardCell = !cell.InTheBoard ? BoardCell.Invalid : board[cell.X, cell.Y];
            return cell;
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