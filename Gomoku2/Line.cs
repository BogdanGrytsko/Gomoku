﻿using System;
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
        //these cells are not null only if corresponding space is empty
        private Cell next, prev, nextNext, prevPrev;

        public Line()
        {
        }

        public Line(Cell cell)
        {
            line.Add(cell);
            Start = cell;
            End = cell;
            Direction = CellManager.Get(0, 0);
        }

        public Line(Cell cell1, Cell cell2)
        {
            line.Add(cell1);
            line.Add(cell2);
            CalcProps();
        }

        private void AddCell(Cell cell)
        {
            line.Add(cell);
            CalcProps();
        }

        private void CalcProps()
        {
            CalcStart();
            CalcEnd();
            CalcDirection();
            CalcNextCells();
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

        private void CalcNextCells()
        {
            //throw new NotImplementedException();
        }

        private Cell Start { get; set; }

        private Cell End { get; set; }

        public int Count { get { return line.Count; } }

        public LineType LineType { get { return lineType; } }

        public List<Cell> PriorityCells { get { return priorityCells; } } 

        public LineType Estimate(BoardCell[,] board, BoardCell type)
        {
            priorityCells.Clear();
            return lineType = GetEstimate(board, type);
        }

        private LineType GetEstimate(BoardCell[,] board, BoardCell type)
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
                    var nextThreeSpace = NextSpace(board, type, true);
                    if (nextThreeSpace == 2) return LineType.StraightFour;
                    if (nextThreeSpace == 1) return LineType.BrokenFourInRow;
                    if (space == 2) return LineType.ThreeInRow;
                    if (space == 1 && priorityCells.Count == 2) return LineType.BlokedThree;
                    return LineType.DeadThree;
                case 2:
                    space = OpenSpace(board);
                    var nextSpace = NextSpace(board, type, false);
                    if (HasBrokenFour(board, type))
                        return LineType.BrokenFourInRow;
                    if (space == 2)
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
            return newLine;
        }

        public bool HasSameDirection(Line other)
        {
            return Direction == other.Direction;
        }

        public bool JoinIfPossible(Cell cell)
        {
            if (line.Count == 1 && Start.DistSqr(cell) <= 2)
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
                return new Tuple<Cell, Cell>(null, null);
            }

            Cell first = null;
            var start = NextCell(1);
            if (start.IsEmpty(board))
            {
                first = start;
            }

            Cell second = null;
            var end = NextCell(-1);
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

        private int OpenSpace(BoardCell[,] board)
        {
            var nextCells = GetTwoNextCells(board);
            if (nextCells.Item1 != null) priorityCells.Add(nextCells.Item1);
            if (nextCells.Item2 != null) priorityCells.Add(nextCells.Item2);
            return priorityCells.Count;
        }

        private Cell NextCell(int i)
        {
            if (i >= 0)
                return Start + i*Direction;
            return End + i*Direction;
        }

        private int NextSpace(BoardCell[,] board, BoardCell type, bool addNextNextCellAsPriority)
        {
            int space = 0;
            if (CellIsDesiredType(board, NextCell(1), BoardCell.None))
            {
                var nextNextCell = NextCell(2);
                if (CellIsDesiredType(board, nextNextCell, type)) space++;
                else if (addNextNextCellAsPriority && CellIsDesiredType(board, nextNextCell, BoardCell.None))
                    priorityCells.Add(nextNextCell);
            }
            if (CellIsDesiredType(board, NextCell(-1), BoardCell.None))
            {
                var nextNextCell = NextCell(-2);
                if (CellIsDesiredType(board, nextNextCell, type)) space++;
                else if (addNextNextCellAsPriority && CellIsDesiredType(board, nextNextCell, BoardCell.None))
                    priorityCells.Add(nextNextCell);
            }
            return space;
        }

        private bool HasBrokenFour(BoardCell[,] board, BoardCell type)
        {
            if (CellIsDesiredType(board, NextCell(1), BoardCell.None))
            {
                if (CellIsDesiredType(board, NextCell(2), type) && CellIsDesiredType(board, NextCell(3), type))
                {
                    priorityCells.Clear();
                    priorityCells.Add(NextCell(1));
                    return true;
                }
            }
            if (CellIsDesiredType(board, NextCell(-1), BoardCell.None))
            {
                if (CellIsDesiredType(board, NextCell(-2), type) && CellIsDesiredType(board, NextCell(-3), type))
                {
                    priorityCells.Clear();
                    priorityCells.Add(NextCell(-1));
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

        //todo maybe compare using not length but LineType. Find where sorting is used.
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
            return string.Format("{2} S {0} E {1}", Start, End, LineType);
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