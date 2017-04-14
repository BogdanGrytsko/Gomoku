using System.Collections.Generic;
using System.Linq;

namespace Gomoku2.CellObjects
{
    public class PriorityCells
    {
        public IList<Cell> Cells { get; private set; }

        public bool IncreasesDepth { get; private set; }

        public PriorityCells(IEnumerable<Cell> cells, bool increasesDepth = true)
        {
            if (cells == null)
                cells = new List<Cell>();
            Cells = cells.ToList();
            IncreasesDepth = increasesDepth;
        }

        public bool Any()
        {
            return Cells.Any();
        }
    }
}