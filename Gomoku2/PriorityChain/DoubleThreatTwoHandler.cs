using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.PriorityChain
{
    public class DoubleThreatTwoHandler : PriorityHandlerBase
    {
        public DoubleThreatTwoHandler(List<Line> lines) : base(lines)
        {
        }

        public override PriorityCells GetCells()
        {
            return new PriorityCells(DoubleThreatCells(lines));
        }

        private static IEnumerable<Cell> DoubleThreatCells(IEnumerable<Line> lines)
        {
            var doubleThreat = GetPriorityCells(lines, type => type.IsTwoInRow() || type.IsLongBrokenTwo());
            return doubleThreat.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
        }
    }
}