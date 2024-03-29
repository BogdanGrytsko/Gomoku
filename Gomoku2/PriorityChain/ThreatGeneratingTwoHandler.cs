﻿using System.Collections.Generic;
using System.Linq;
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
            var threatGenerating = GetPriorityCells(lines, l => l.IsTwoInRow() || l.IsLongBrokenTwo());
            return new PriorityCells(threatGenerating);
        }
    }
}