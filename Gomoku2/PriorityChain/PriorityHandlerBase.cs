using System;
using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.PriorityChain
{
    public abstract class PriorityHandlerBase
    {
        protected readonly List<Line> lines;

        protected PriorityHandlerBase(List<Line> lines)
        {
            this.lines = lines;
        }

        public abstract PriorityCells GetCells();

        protected static IEnumerable<Cell> GetHighPriorityCells(IEnumerable<Line> lines, Predicate<LineType> predicate)
        {
            return lines.Where(l => predicate(l.LineType)).SelectMany(l => l.HighPriorityCells);
        }

        public static IEnumerable<Cell> GetPriorityCells(IEnumerable<Line> lines, Predicate<LineType> predicate)
        {
            return lines.Where(l => predicate(l.LineType)).SelectMany(l => l.PriorityCells);
        }
    }
}