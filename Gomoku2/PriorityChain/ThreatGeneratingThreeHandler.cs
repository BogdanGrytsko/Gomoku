using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.PriorityChain
{
    public class ThreatGeneratingThreeHandler : PriorityHandlerBase
    {
        public ThreatGeneratingThreeHandler(List<Line> lines) : base(lines)
        {
        }

        public override PriorityCells GetCells()
        {
            var threatGenerating = lines.FirstOrDefault(l => l.LineType.IsBlokedThree() || l.LineType.IsLongBrokenThree());
            return new PriorityCells(threatGenerating?.PriorityCells);
        }
    }
}