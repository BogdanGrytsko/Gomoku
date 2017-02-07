using System.Collections.Generic;

namespace Gomoku2
{
    public class EstimatedCell
    {
        public EstimatedCell(Cell cell, List<Line> lines, int estimate)
        {
            Cell = cell;
            Lines = lines;
            Estimate = estimate;
        }

        public Cell Cell { get; private set; }

        public List<Line> Lines { get; private set; }

        public int Estimate { get; private set; }
    }
}