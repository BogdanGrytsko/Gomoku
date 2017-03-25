using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.PriorityChain
{
    public class PriorityAlgorithm
    {
        private readonly List<Line> myLines, oppLines;
        private readonly List<PriorityHandlerBase> handlers;

        public PriorityAlgorithm(List<Line> myLines, List<Line> oppLines)
        {
            this.myLines = myLines;
            this.oppLines = oppLines;
            handlers = GetHandlers().ToList();
        }

        private IEnumerable<PriorityHandlerBase> GetHandlers()
        {
            //Analyzis is done based on fact that it is MY move right now, yet to be done.
            //this leads to different attack and defence priority cells
            //if I have 4 cell line -> finish it. I Win (StraightFour, FourInRow, BrokenFour)
            //if opponent has 4 cell line -> block, otherwise I loose (StraightFour, FourInRow, BrokenFour)
            //if I have Winnable line than do win with it. (ThreeInRow, BrokenThree)
            //TODO if opponent has winnable 3 cell line vs I have Threat-Generating 3 cell line (LongBrokenThree, BlockedThree) ???
            //if I have 2 cell Threat-Generating line (Even Double-Threat) (TwoInRow, LongBrokenTwo)
            //if opponent has Double-threat possibility.

            yield return new FourCellHandler(myLines);
            yield return new FourCellHandler(oppLines);
            yield return new ThreatOfThreeHandler(myLines);
            yield return new ThreatOfThreeHandler(oppLines);
            yield return new DoubleThreatHandler(myLines);
            yield return new BlockedThreeHandler(myLines);
            yield return new DoubleThreatHandler(oppLines);
        }

        public PriorityCells GetPriorityCells()
        {
            foreach (var handler in handlers)
            {
                var cells = handler.GetCells();
                if (cells.Any()) return cells;
            }
            return new PriorityCells(new List<Cell>());
        }
    }
}