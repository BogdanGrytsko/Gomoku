using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.PriorityChain
{
    public class DefenceThreatOfThreeHandler : PriorityHandlerBase
    {
        public DefenceThreatOfThreeHandler(List<Line> lines) : base(lines)
        {
        }

        public override PriorityCells GetCells()
        {
            return new PriorityCells(GetPriorityCells(lines, LineTypeExtensions.ThreatOfThree), false);
        }
    }
}