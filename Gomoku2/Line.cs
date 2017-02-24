﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gomoku2
{
    public class Line : IComparable<Line>, IEnumerable<Cell>, IEquatable<Line>
    {
        private readonly List<Cell> line = new List<Cell>();
        private LineType lineType;
        private BoardCell owner;
        private Cell next, prev, nextNext, prevPrev, nextNextNext, prevPrevPrev, brokenFourCell;

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
                if (lineType == LineType.BrokenFourInRow)
                {
                    yield return brokenFourCell;
                    yield break;
                }
                switch (Count)
                {
                    case 3:
                        foreach (var cell in GetNextCells(true))
                        {
                            yield return cell;
                        }
                        break;
                    case 4:
                    case 2:
                        foreach (var cell in GetNextCells(false))
                        {
                            yield return cell;
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
            //todo WHY THE FUCK this method doesn't work????
            //CalcNextAndPrev(board);
            CalcNext(board);
            CalcPrev(board);
            return lineType = GetEstimate();
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
            switch (Count)
            {
                case 5:
                    return LineType.FiveInRow;
                case 4:
                    if (IsDead) return LineType.DeadFour;
                    if (IsOpenFromBothSides) return LineType.StraightFour;
                    return LineType.FourInRow;
                case 3:
                    if (IsDead) return LineType.DeadThree;
                    return ThreeInRowAnalysis();
                case 2:
                    if (IsDead) return LineType.DeadTwo;
                    return TwoInRowAnalysis();
                case 1:
                    return LineType.SingleMark;
            }
            return LineType.Useless;
        }

        private LineType ThreeInRowAnalysis()
        {
            if (!next.IsEmpty && prev.IsEmpty)
                return ThreeInRowOneSideOpened(PrevCells);
            if (next.IsEmpty && !prev.IsEmpty)
                return ThreeInRowOneSideOpened(NextCells);
            return ThreeInRowTwoSidesOpened();
        }

        private LineType ThreeInRowOneSideOpened(List<Cell> cells)
        {
            //OXXX O
            if (cells[1].BoardCell == owner.Opponent())
                return LineType.DeadThree;
            //OXXX X
            if (cells[1].BoardCell == owner)
            {
                brokenFourCell = cells[0];
                return LineType.BrokenFourInRow;
            }
            //OXXX  
            return LineType.BlokedThree;
        }

        private LineType ThreeInRowTwoSidesOpened()
        {
            var nextResult = ThreeInRowOneSideOpened(NextCells);
            var prevResult = ThreeInRowOneSideOpened(PrevCells);
            //X XXX X
            if (nextResult.IsBrokenFourInRow() && prevResult.IsBrokenFourInRow())
                return LineType.StraightFour;
            //* XXX X
            if (nextResult.IsBrokenFourInRow()|| prevResult.IsBrokenFourInRow())
                return LineType.BrokenFourInRow;
            // XXX 
            return LineType.ThreeInRow;
        }

        private LineType TwoInRowAnalysis()
        {
            if (!next.IsEmpty && prev.IsEmpty)
                return TwoInRowOneSideOpened(PrevCells);
            if (next.IsEmpty && !prev.IsEmpty)
                return TwoInRowOneSideOpened(NextCells);
            return TwoInRowTwoSidesOpened();
        }

        private LineType TwoInRowOneSideOpened(List<Cell> cells)
        {
            //OXX O*
            if (cells[1].BoardCell == owner.Opponent())
                return LineType.DeadTwo;
            //OXX X*
            if (cells[1].BoardCell == owner)
            {
                //OXX XX
                if (cells[2].BoardCell == owner)
                {
                    brokenFourCell = cells[0];
                    return LineType.BrokenFourInRow;
                }
                //OXX XO
                if (cells[2].BoardCell == owner.Opponent())
                    return LineType.DeadThree;
                //OXX X 
                return LineType.BlokedThree;
            }
            //OXX  X
            if (cells[2].BoardCell == owner)
                return LineType.BlokedThree;
            //OXX   
            return LineType.BlockedTwo;
        }

        private LineType TwoInRowTwoSidesOpened()
        {
            var nextResult = TwoInRowOneSideOpened(NextCells);
            var prevResult = TwoInRowOneSideOpened(PrevCells);
            // O XX O 
            if (nextResult.IsDeadTwo() && prevResult.IsDeadTwo())
                return LineType.DeadTwo;
            //XX XX XX
            if (nextResult.IsBrokenFourInRow() && prevResult.IsBrokenFourInRow())
                return LineType.StraightFour;
            //  XX XX
            if (nextResult.IsBrokenFourInRow() || prevResult.IsBrokenFourInRow())
                return LineType.BrokenFourInRow;
            // X XX  X
            if (nextResult.IsBlokedThree() && prevResult.IsBlokedThree())
                return LineType.DoubleBrokenThree;
            // XX X 
            if (nextResult.IsBlokedThree() || prevResult.IsBlokedThree())
                return LineType.BrokenThree;
            //OX XX  
            if (nextResult.IsDeadThree() || prevResult.IsDeadThree())
                return LineType.BlokedThree;

            return LineType.TwoInRow;
        }

        private Cell Direction { get; set; }

        public Line Clone()
        {
            var newLine = new Line();
            newLine.line.AddRange(line);

            newLine.Start = Start;
            newLine.End = End;
            newLine.Direction = Direction;
            newLine.owner = owner;

            newLine.next = next;
            newLine.nextNext = nextNext;
            newLine.nextNextNext = nextNextNext;
            newLine.prev = prev;
            newLine.prevPrev = prevPrev;
            newLine.prevPrevPrev = prevPrevPrev;

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