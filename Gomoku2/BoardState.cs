using System;
using System.Collections.Generic;
using System.Linq;

namespace Gomoku2
{
    public class BoardState
    {
        public List<Line> MyLines { get; private set; }

        public List<Line> OppLines { get; private set; }

        public BoardCell MyCellType { get; private set; }

        public BoardCell OpponentCellType
        {
            get { return MyCellType.Opponent(); }
        }

        public int Depth { get; private set; }

        public int MaxWidth { get; private set; }

        public BoardCell[,] Board { get; private set; }

        public BoardState(
            List<Line> myLines, List<Line> oppLines, BoardCell myCellType, int depth, int minDepth, int maxWidth, BoardCell[,] board)
        {
            MyLines = myLines;
            OppLines = oppLines;
            MyCellType = myCellType;
            Depth = depth;
            Board = board;
            MaxWidth = maxWidth;
            MinDepth = minDepth;
        }

        public bool ItIsFirstsTurn
        {
            get
            {
                return MyCellType == BoardCell.First;
            }
        }

        public int Multiplier
        {
            get { return ItIsFirstsTurn ? 1 : -1; }
        }

        public int StartEstimate
        {
            get
            {
                return ItIsFirstsTurn ? int.MinValue : int.MaxValue;
            }
        }

        public BoardState GetNextState(List<Line> myNewLines)
        {
            return new BoardState(OppLines, myNewLines, OpponentCellType, Depth - 1, MinDepth, MaxWidth, Board);
        }

        public IEnumerable<Cell> GetNextCells()
        {
            var threatCells = GetPriorityThreatCells().ToList();
            if (threatCells.Any())
            {
                MinDepth--;
                return new HashSet<Cell>(threatCells);
            }
            return GetNearEmptyCells();
        }

        private int MinDepth { get; set; }

        public bool IsTerminal
        {
            get
            {
                return Depth == MinDepth;
            }
        }

        private IEnumerable<Cell> GetPriorityThreatCells()
        {
            foreach (var myLine in MyLines)
            {
                myLine.Estimate(Board, MyCellType);
            }
            foreach (var oppLine in OppLines)
            {
                oppLine.Estimate(Board, OpponentCellType);
            }
            foreach (var cell in SelectManyPriorityCells(MyLines, type => type == LineType.StraightFour)) yield return cell;
            foreach (var cell in SelectManyPriorityCells(OppLines, type => type == LineType.StraightFour)) yield return cell;

            foreach (var cell in SelectManyPriorityCells(MyLines, Game.ThreatOfFour)) yield return cell;
            foreach (var cell in SelectManyPriorityCells(OppLines, Game.ThreatOfFour)) yield return cell;
            foreach (var cell in SelectManyPriorityCells(MyLines, type => type == LineType.BlokedThree)) yield return cell;
        }

        private static IEnumerable<Cell> SelectManyPriorityCells(IEnumerable<Line> lines, Predicate<LineType> predicate)
        {
            return lines.Where(l => predicate(l.LineType)).SelectMany(l => l.PriorityCells);
        }

        public IEnumerable<Cell> GetNearEmptyCells()
        {
            var set = new HashSet<Cell>();
            set.UnionWith(GetPriorityCells(MyLines));
            set.UnionWith(GetPriorityCells(OppLines));
            set.UnionWith(SelectManyPriorityCells(MyLines, Game.ThreatOfThree));
            set.UnionWith(SelectManyPriorityCells(OppLines, Game.ThreatOfThree));

            for (int x = 0; x < 15; ++x)
            {
                for (int y = 0; y < 15; ++y)
                {
                    if (Board[x, y] != BoardCell.None)
                        set.UnionWith(CellManager.Get(x, y).GetAdjustmentEmptyCells(Board));
                }
            }
            return set;
        }

        private IEnumerable<Cell> GetPriorityCells(IEnumerable<Line> lines)
        {
            foreach (var line in lines)
            {
                var close = line.GetTwoNextCells(Board);
                var next = line.GetTwoNextNextCells(Board);
                if (close.Item1 != null)
                {
                    yield return close.Item1;
                    if (next.Item1 != null)
                        yield return next.Item1;
                }
                if (close.Item2 != null)
                {
                    yield return close.Item2;
                    if (next.Item2 != null)
                        yield return next.Item2;
                }
            }
        }

        public BoardState Clone()
        {
            return new BoardState(MyLines, OppLines, MyCellType, Depth, MinDepth, MaxWidth, (BoardCell[,])Board.Clone());
        }
    }
}