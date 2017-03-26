using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.PriorityChain
{
    public class DoubleThreatHandler : PriorityHandlerBase
    {
        public DoubleThreatHandler(List<Line> lines) : base(lines)
        {
        }

        public override PriorityCells GetCells()
        {
            return new PriorityCells(DoubleThreatCells(lines));
        }

        private static IEnumerable<Cell> DoubleThreatCells(IEnumerable<Line> lines)
        {
            var doubleThreat = GetPriorityCells(lines, type => type.IsBlokedThree() || type.IsTwoInRow() || type.IsLongBrokenTwo());
            return doubleThreat.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
        }
    }
}