using System;
using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.StateCache
{
    public class BoardState : BoardStateBase
    {
        public int Depth { get; private set; }

        public int MaxWidth { get; private set; }

        public BoardState(
            List<Line> myLines, List<Line> oppLines, BoardCell myCellType, int depth, int minDepth, int maxWidth, BoardCell[,] board)
            :base(myLines, oppLines, myCellType, board)
        {
            Depth = depth;
            MaxWidth = maxWidth;
            MinDepth = minDepth;
        }

        public bool ItIsFirstsTurn => MyCellType == BoardCell.First;

        public int Multiplier => ItIsFirstsTurn ? 1 : -1;

        public int StartEstimate => ItIsFirstsTurn ? int.MinValue : int.MaxValue;

        public BoardState GetNextState(List<Line> myNewLines, List<Line> oppNewLines)
        {
            return new BoardState(oppNewLines, myNewLines, OpponentCellType, Depth - 1, MinDepth, MaxWidth, Board);
        }

        public BoardState GetThisState(List<Line> myNewLines, List<Line> oppNewLines)
        {
            return new BoardState(myNewLines, oppNewLines, MyCellType, Depth, MinDepth, MaxWidth, (BoardCell[,])Board.Clone());
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

        public bool IsTerminal => Depth == MinDepth;

        private PriorityCells GetPriorityThreatCells()
        {
            var stratighFour = GetPriorityThreatCells(type => type.IsStraightFour());
            if (stratighFour.Any())
                return new PriorityCells(stratighFour);

            var threatOfFour = GetPriorityThreatCells(LineTypeExtensions.ThreatOfFour);
            if (threatOfFour.Any())
                return new PriorityCells(threatOfFour);

            //todo long broken 3 should be processed separatelly.
            var threatOfThree = GetPriorityThreatCells(type => type.ThreatOfThree() || type.IsLongBrokenThree());
            if (threatOfThree.Any())
                return new PriorityCells(threatOfThree, false);

            //find double threat
            var myDoubleThreat = DoubleThreatCells(MyLines);
            if (myDoubleThreat.Any())
                return new PriorityCells(myDoubleThreat, false);

            //this forces immidiate analyzis on blocked three cells.
            var myBlockedThree = MyLines.FirstOrDefault(l => l.LineType.IsBlokedThree());
            if (myBlockedThree != null)
                return new PriorityCells(myBlockedThree.PriorityCells);

            var oppDoubleThreat = DoubleThreatCells(OppLines);
            if (oppDoubleThreat.Any())
                return new PriorityCells(oppDoubleThreat, false);

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
            var doubleThreat = SelectManyPriorityCells(lines, type => type.IsBlokedThree() || type.IsTwoInRow() || type.IsLongBrokenTwo());
            return doubleThreat.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
        }

        private static IEnumerable<Cell> SelectManyHighPriorityCells(IEnumerable<Line> lines, Predicate<LineType> predicate)
        {
            return lines.Where(l => predicate(l.LineType)).SelectMany(l => l.HighPriorityCells);
        }

        private static IEnumerable<Cell> SelectManyPriorityCells(IEnumerable<Line> lines, Predicate<LineType> predicate)
        {
            return lines.Where(l => predicate(l.LineType)).SelectMany(l => l.PriorityCells);
        }

        public NextCells GetNearEmptyCells()
        {
            var set = new HashSet<Cell>();
            set.UnionWith(GetPriorityCells(MyLines));

            for (int x = 0; x < 15; ++x)
            {
                for (int y = 0; y < 15; ++y)
                {
                    //todo this is TOO MUCH. Decrease this number. At least for leafs
                    if (Board[x, y] == MyCellType)
                        set.UnionWith(CellManager.Get(x, y).GetAdjustmentEmptyCells(Board));
                }
            }
            return new NextCells(set, GetPriorityCells(OppLines));
        }

        private static IEnumerable<Cell> GetPriorityCells(IEnumerable<Line> lines)
        {
            return lines.SelectMany(line => line.PriorityCells);
        }
    }
}