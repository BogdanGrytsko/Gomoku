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

        public NextCells GetNextCells()
        {
            const int minMinDepth = -16;
            var threatCells = GetPriorityThreatCells();
            if (threatCells.Cells.Any())
            {
                if (threatCells.IncreasesDepth && MinDepth > minMinDepth)
                    MinDepth--;
                return new NextCells(threatCells.Cells);
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

        private PriorityCells GetPriorityThreatCells()
        {
            foreach (var myLine in MyLines)
            {
                myLine.Estimate(Board);
            }
            foreach (var oppLine in OppLines)
            {
                oppLine.Estimate(Board);
            }
            var myStraightFour = SelectManyPriorityCells(MyLines, type => type.IsStraightFour());
            if (myStraightFour.Any())
                return new PriorityCells(myStraightFour);
            var oppStraightFour = SelectManyPriorityCells(OppLines, type => type.IsStraightFour());
            if (oppStraightFour.Any())
                return new PriorityCells(oppStraightFour);

            var myThreatOfFour = SelectManyPriorityCells(MyLines, LineTypeExtensions.ThreatOfFour);
            if (myThreatOfFour.Any())
                return new PriorityCells(myThreatOfFour);
            var oppThreatOfFour = SelectManyPriorityCells(OppLines, LineTypeExtensions.ThreatOfFour);
            if (oppThreatOfFour.Any())
                return new PriorityCells(oppThreatOfFour);

            //this forces immidiate analyzis on blocked three cells.
            var myBlockedThree = SelectManyPriorityCells(MyLines, type => type.IsBlokedThree());
            if (myBlockedThree.Any())
                return new PriorityCells(myBlockedThree);

            var myThreatOfThree = SelectManyPriorityCells(MyLines, type => type.ThreatOfThree());
            if (myThreatOfThree.Any())
                return new PriorityCells(myThreatOfThree, false);
            var oppThreatOfThree = SelectManyPriorityCells(OppLines, type => type.ThreatOfThree());
            if (oppThreatOfThree.Any())
                return new PriorityCells(oppThreatOfThree, false);
            //find double threat
            var myDoubleThreat = DoubleThreatCells(MyLines);
            if (myDoubleThreat.Any())
                return new PriorityCells(myDoubleThreat, false);

            var oppDoubleThreat = DoubleThreatCells(OppLines);
            if (oppDoubleThreat.Any())
                return new PriorityCells(oppDoubleThreat, false);

            return new PriorityCells(new List<Cell>());
        }

        private static IEnumerable<Cell> DoubleThreatCells(IEnumerable<Line> lines)
        {
            var oppDoubleThreat = SelectManyPriorityCells(lines, type => type.IsBlokedThree() || type.IsTwoInRow());
            return oppDoubleThreat.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
        }

        private static IEnumerable<Cell> SelectManyPriorityCells(IEnumerable<Line> lines, Predicate<LineType> predicate)
        {
            return lines.Where(l => predicate(l.LineType)).SelectMany(l => l.HighPriorityCells);
        }

        public NextCells GetNearEmptyCells()
        {
            var set = new HashSet<Cell>();
            set.UnionWith(GetPriorityCells(MyLines));

            for (int x = 0; x < 15; ++x)
            {
                for (int y = 0; y < 15; ++y)
                {
                    //todo this is TOO MUCH. Decrease this number
                    if (Board[x, y] == MyCellType)
                        set.UnionWith(CellManager.Get(x, y).GetAdjustmentEmptyCells(Board));
                }
            }
            return new NextCells(set, GetPriorityCells(OppLines));
        }

        private static IEnumerable<Cell> GetPriorityCells(IEnumerable<Line> lines)
        {
            return lines.SelectMany(line => line.GetNextCells(true));
        }

        public BoardState Clone()
        {
            return new BoardState(MyLines, OppLines, MyCellType, Depth, MinDepth, MaxWidth, (BoardCell[,])Board.Clone());
        }
    }
}