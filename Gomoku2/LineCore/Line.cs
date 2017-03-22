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
        private readonly List<Cell> cells = new List<Cell>();
        private LineType lineType;
        //todo fix public => private
        public BoardCell owner;
        public Cell next, prev, nextNext, prevPrev, nextNextNext, prevPrevPrev, middle1, middle2;
        //is used for broken lines
        //private Cell lonelyCell;
        private List<Cell> priorityCells;

        public Line()
        {
        }

        public Line(Cell cell, BoardCell owner)
        {
            cells.Add(cell);
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
            cells.Add(cell1);
            cells.Add(cell2);
            this.owner = owner;
            CalcPropsAndEstimate(board);
        }

        public Line(Cell cell1, Cell cell2, Cell cell3, BoardCell owner, BoardCell[,] board)
        {
            cells.Add(cell1);
            cells.Add(cell2);
            cells.Add(cell3);
            this.owner = owner;
            CalcPropsAndEstimate(board);
        }

        public Line(CellDirection cellDir, BoardCell[,] board, BoardCell owner)
        {
            cells.Add(cellDir.Cell);
            cells.Add(cellDir.AnalyzedCell);
            this.owner = owner;
            CalcProps(board);
            RemoveCell(cellDir);
        }

        public void AddCells(BoardCell[,] board, params Cell[] cell)
        {
            cells.AddRange(cell);
            CalcPropsAndEstimate(board);
        }

        private void CalcPropsAndEstimate(BoardCell[,] board)
        {
            CalcProps(board);
            SetEstimate();
        }

        private void CalcProps(BoardCell[,] board)
        {
            CalcStart();
            CalcEnd();
            CalcDirection();
            CalcNextAndPrev(board);
        }

        public Cell Middle1 => middle1;
        public Cell Middle2 => middle2;

        private void CalcStart()
        {
            var maxY = this.cells.Max(c => c.Y);
            var cells = this.cells.Where(c => c.Y == maxY).ToList();
            if (cells.Count == 1)
            {
                Start = cells[0];
                return;
            }
            var maxX = this.cells.Max(c => c.X);
            Start = cells.First(c => c.X == maxX);
        }

        private void CalcEnd()
        {
            var minY = this.cells.Min(c => c.Y);
            var cells = this.cells.Where(c => c.Y == minY).ToList();
            if (cells.Count == 1)
            {
                End = cells[0];
                return;
            }
            var minX = this.cells.Min(c => c.X);
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

        public int Count { get { return cells.Count; } }

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
                            //todo uncomment
                            //foreach (var cell in GetNextCells(false)) yield return cell;
                            //yield return middle1;
                            break;
                        }
                        break;
                }
            }
        }

        public IEnumerable<Cell> GetNextCells(bool includeNextNext)
        {
            //todo for some reason next was null for single mark. investigate.
            if (lineType.IsSingleMark())
                yield break;
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

        public bool IsDead
        {
            get { return !next.IsEmpty && !prev.IsEmpty; }
        }

        public List<Cell> NextCells
        {
            get { return new List<Cell> {next, nextNext, nextNextNext}; }
        }

        public List<Cell> PrevCells
        {
            get { return new List<Cell> { prev, prevPrev, prevPrevPrev }; }
        }

        private LineType GetEstimate()
        {
            priorityCells = null;
            var analyzer = GetAnalyzer();
            if (analyzer != null)
                return DoAnalysis(analyzer);
            switch (Count)
            {
                case 5:
                    return LineType.FiveInRow;
                case 1:
                    return LineType.SingleMark;
            }
            return LineType.Useless;
        }

        private AnalyzerBase GetAnalyzer()
        {
            switch (Count)
            {
                case 4:
                    if (middle1 != null)
                        return new BrokenFourAnalyzer(this);
                    return new FourInRowAnalyzer(this);
                case 3:
                    if (middle2 != null)
                        return new LongBrokenThreeAnalyzer(this);
                    if (middle1 != null)
                        return new BrokenThreeAnalyzer(this);
                    return new ThreeInRowAnalyzer(this);
                case 2:
                    if (middle2 != null)
                        return new LongBrokenTwoAnalyzer(this);
                    if (middle1 != null)
                        return new BrokenTwoAnalyzer(this);
                    return new TwoInRowAnalyzer(this);
            }
            return null;
        }

        private LineType DoAnalysis(AnalyzerBase analyzer)
        {
            if (IsDead)
                return analyzer.Dead();
            if (!next.IsEmpty && prev.IsEmpty)
                return analyzer.PrevOpened(ref priorityCells);
            if (next.IsEmpty && !prev.IsEmpty)
                return analyzer.NextOpened(ref priorityCells);
            return analyzer.TwoSidesOpened(ref priorityCells);
        }

        public Cell Direction { get; set; }

        public Line Clone()
        {
            //todo MemberwiseClone doesn't work. Investigate
            //return (Line)MemberwiseClone();
            var newLine = new Line();
            newLine.cells.AddRange(cells);

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
            newLine.middle2 = middle2;
            //newLine.lonelyCell = lonelyCell;

            newLine.priorityCells = priorityCells;

            return newLine;
        }

        public bool HasSameDirection(Line other)
        {
            return Direction == other.Direction;
        }

        public static bool IsBrokenTwoDistance(int dist)
        {
            return dist == 4 || dist == 8;
        }

        public static bool IsLongBrokenTwoDistance(int dist)
        {
            return dist == 9 || dist == 18;
        }

        public bool IsBrokenTwo
        {
            get { return middle1 != null; }
        }

        public bool JoinIfPossible(Cell cell, BoardCell[,] board)
        {
            if (cells.Count == 1 && middle1 == null)
            {
                var dist = Start.DistSqr(cell);
                if (dist <= 2)
                {
                    AddCells(board, cell);
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
                        AddCells(board, cell);
                        cells.Remove(cell);
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

                cells.Add(cell);
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

                cells.Add(cell);
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
            foreach (var cell in cells)
            {
                yield return cell;
            }
            //if (lonelyCell != null) yield return lonelyCell;
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
            cells.UnionWith(this.cells);
            cells.UnionWith(otherLine);
            var mergedLine = new Line();
            mergedLine.cells.AddRange(cells);
            mergedLine.owner = owner;
            //TODO maybe we don't need to recalculate all props
            mergedLine.CalcPropsAndEstimate(board);
            return mergedLine;
        }

        private void RemoveCell(CellDirection cellDir)
        {
            cells.Remove(cellDir.Cell);
            SetLonelyAndMiddleCells(cellDir);
            SetEstimate();
        }

        private void SetLonelyAndMiddleCells(CellDirection cellDir)
        {
            cells.Add(cellDir.Cell);
            //lonelyCell = cellDir.Cell;
            middle1 = cellDir.Cell + cellDir.Direction;
            if (cellDir.Distance == 3)
                middle2 = cellDir.Cell + 2*cellDir.Direction;
        }

        public void AddLonelyCell(CellDirection cellDir)
        {
            SetLonelyAndMiddleCells(cellDir);
        }

        public void AddMissingCell(Cell cell)
        {
            cells.Add(cell);
            if (middle1 == cell)
            {
                //if (lonelyCell != null)
                //    cells.Add(lonelyCell);
                //lonelyCell = null;
                middle1 = null;
            }
            SetEstimate();
        }
    }
}