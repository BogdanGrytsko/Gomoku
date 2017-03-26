using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.PriorityChain
{
    public class ThreatGeneratingTwoHandler : PriorityHandlerBase
    {
        public ThreatGeneratingTwoHandler(List<Line> lines) : base(lines)
        {
        }

        public override PriorityCells GetCells()
        {
            return new PriorityCells(new List<Cell>());
        }
    }
}