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
            //todo have to fix bug and finally comment this
            foreach (var myLine in MyLines)
            {
                myLine.Estimate(Board);
            }
            foreach (var oppLine in OppLines)
            {
                oppLine.Estimate(Board);
            }
            var stratighFour = GetPriorityThreatCells(type => type.IsStraightFour());
            if (stratighFour.Any())
                return new PriorityCells(stratighFour);

            var threatOfFour = GetPriorityThreatCells(LineTypeExtensions.ThreatOfFour);
            if (threatOfFour.Any())
                return new PriorityCells(threatOfFour);

            var threatOfThree = GetPriorityThreatCells(type => type.ThreatOfThree());
            if (threatOfThree.Any())
                return new PriorityCells(threatOfThree, false);

            //find double threat
            var myDoubleThreat = DoubleThreatCells(MyLines);
            if (myDoubleThreat.Any())
                return new PriorityCells(myDoubleThreat, false);

            var oppDoubleThreat = DoubleThreatCells(OppLines);
            if (oppDoubleThreat.Any())
                return new PriorityCells(oppDoubleThreat, false);

            //this forces immidiate analyzis on blocked three cells.
            var myBlockedThree = SelectManyHighPriorityCells(MyLines, type => type.IsBlokedThree());
            if (myBlockedThree.Any())
                return new PriorityCells(myBlockedThree);

            return new PriorityCells(new List<Cell>());
        }

        private IList<Cell> GetPriorityThreatCells(Predicate<LineType> func)
        {
            var myThreat = SelectManyHighPriorityCells(MyLines, func).ToList();
            if (myThreat.Any())
                return myThreat;
            var oppThreat = SelectManyHighPriorityCells(OppLines, func).ToList();
            if (oppThreat.Any())
                return oppThreat;

            return new List<Cell>();
        }

        private static IEnumerable<Cell> DoubleThreatCells(IEnumerable<Line> lines)
        {
            var oppDoubleThreat = SelectManyHighPriorityCells(lines, type => type.IsBlokedThree() || type.IsTwoInRow());
            return oppDoubleThreat.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
        }

        private static IEnumerable<Cell> SelectManyHighPriorityCells(IEnumerable<Line> lines, Predicate<LineType> predicate)
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