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
        public Cell next, prev, nextNext, prevPrev, nextNextNext, prevPrevPrev;
        private Cell middle1, middle2;
        private AnalyzerBase analyzer;

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

        public Line(IEnumerable<Cell> newCells, BoardCell owner, BoardCell[,] board)
        {
            cells.AddRange(newCells);
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
            foreach (var cell1 in cell)
            {
                if (cells.Contains(cell1)) continue;
                AddCellAndMaybeNullMiddle(cell1);
            }
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

        public IEnumerable<Cell> HighPriorityCells => analyzer.HighPriorityCells;

        public IEnumerable<Cell> PriorityCells => analyzer.PriorityCells;

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
            analyzer = GetAnalyzer();
            if (analyzer != null)
                return analyzer.DoAnalysis();
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

        public Cell Direction { get; set; }

        public Line Clone()
        {
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

        private void RemoveCell(CellDirection cellDir)
        {
            cells.Remove(cellDir.Cell);
            SetLonelyAndMiddleCells(cellDir);
            SetEstimate();
        }

        private void SetLonelyAndMiddleCells(CellDirection cellDir)
        {
            if (cells.Contains(cellDir.Cell)) return;;
            AddCellAndMaybeNullMiddle(cellDir.Cell);

            if (middle1 == null)
                middle1 = cellDir.Cell + cellDir.Direction;
            // X X X case
            else
                middle2 = cellDir.Cell + cellDir.Direction;

            if (cellDir.Distance == 3)
                middle2 = cellDir.Cell + 2*cellDir.Direction;
        }

        private void AddCellAndMaybeNullMiddle(Cell cell)
        {
            cells.Add(cell);
            NullMiddle(cell);
        }

        private void NullMiddle(Cell cell)
        {
            if (cell == middle1)
                middle1 = null;
            if (cell == middle2)
                middle2 = null;
            if (middle2 != null && middle1 == null)
            {
                middle1 = middle2;
                middle2 = null;
            }
        }

        public void AddLonelyCell(CellDirection cellDir, BoardCell[,] board)
        {
            SetLonelyAndMiddleCells(cellDir);
            CalcPropsAndEstimate(board);
        }

        public void AddMissingCell(Cell cell)
        {
            cells.Add(cell);
            if (middle1 == cell)
                middle1 = null;
            SetEstimate();
        }

        public bool IsCellMiddle(Cell cell)
        {
            return middle1 == cell || middle2 == cell;
        }

        public bool CanAddCell(CellDirection cellDir)
        {
            // X X X
            if (lineType.IsTwoInRow() && middle1 != null)
                return IsCellMiddle(cellDir.Cell) || cellDir.Distance <= 2;

            if (lineType.IsLongBrokenThree() || lineType.IsLongBlockedThree())
                return IsCellMiddle(cellDir.Cell);
            if (lineType.IsLongBrokenTwo())
                return IsCellMiddle(cellDir.Cell) || cellDir.Distance == 1;
            return true;
        }

        public IEnumerable<Cell> ExtractCells(Cell cell, BoardCell[,] board)
        {
            var sameDirCell = new List<Cell>();
            var oppDirCell = new List<Cell>();
            foreach (var cell1 in cells.ToList())
            {
                if ((cell1 - cell).Normalize() == Direction)
                    sameDirCell.Add(cell1);
                else
                    oppDirCell.Add(cell1);
            }
            var shortest = GetShortest(sameDirCell, oppDirCell);
            foreach (var cell1 in shortest)
            {
                cells.Remove(cell1);
                yield return cell1;
            }
            //additional null for all that is not X X X
            if (!IsStrangeBrokenThree)
            {
                middle1 = null;
                middle2 = null;
            }
            NullMiddle(cell);
            CalcPropsAndEstimate(board);
        }

        private bool IsStrangeBrokenThree
        {
            get
            {
                if (middle1 == null || middle2 == null)
                    return false;
                return middle1.DistSqr(middle2) == 4;
            }
        }

        private static List<Cell> GetShortest(List<Cell> sameDirCell, List<Cell> oppDirCell)
        {
            if (sameDirCell.Count >= oppDirCell.Count)
                return oppDirCell;
            return sameDirCell;
        }
    }
}