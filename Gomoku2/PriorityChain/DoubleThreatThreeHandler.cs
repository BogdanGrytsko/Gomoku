using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.PriorityChain
{
    public class DoubleThreatThreeHandler : PriorityHandlerBase
    {
        public DoubleThreatThreeHandler(List<Line> lines) : base(lines)
        {
        }

        public override PriorityCells GetCells()
        {
            return new PriorityCells(DoubleThreatCells(lines));
        }

        private static IEnumerable<Cell> DoubleThreatCells(IEnumerable<Line> lines)
        {
            var threeCellLinePriority = GetPriorityCells(lines, type => type.IsLongBrokenThree() || type.IsBlokedThree()).ToList();
            var twoCellLinePriority = GetPriorityCells(lines, type => type.IsTwoInRow() || type.IsLongBrokenTwo());
            var combinedCells = new List<Cell>(threeCellLinePriority);
            combinedCells.AddRange(twoCellLinePriority);
            var doubleThreat = combinedCells.GroupBy(s => s).SelectMany(grp => grp.Skip(1)).ToList();

            var intersect = threeCellLinePriority.Intersect(doubleThreat);
            return intersect.Any() ? doubleThreat : new List<Cell>();
        }
    }
}