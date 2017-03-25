using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.PriorityChain
{
    public class FourCellHandler : PriorityHandlerBase
    {
        public FourCellHandler(List<Line> lines) : base(lines)
        {
        }

        public override PriorityCells GetCells()
        {
            return new PriorityCells(GetHighPriorityCells(lines, LineTypeExtensions.FourCellLine));
        }
    }
}