using System.Collections.Generic;

namespace Gomoku2
{
    public class PriorityCells
    {
        public IEnumerable<Cell> Cells { get; private set; }

        public bool IncreasesDepth { get; private set; }

        public PriorityCells(IEnumerable<Cell> cells, bool increasesDepth = true)
        {
            Cells = cells;
            IncreasesDepth = increasesDepth;
        }
    }
}