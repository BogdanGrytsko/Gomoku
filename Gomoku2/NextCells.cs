using System.Collections.Generic;
using System.Linq.Expressions;

namespace Gomoku2
{
    public class NextCells
    {
        public NextCells(IEnumerable<Cell> threatCells)
        {
            MyNextCells = new HashSet<Cell>(threatCells);
        }

        public NextCells(HashSet<Cell> myCells, IEnumerable<Cell> oppCells)
        {
            MyNextCells = myCells;
            OppNextCells = oppCells;
        }

        public IEnumerable<Cell> MyNextCells { get; set; }

        public IEnumerable<Cell> OppNextCells { get; set; }
    }
}