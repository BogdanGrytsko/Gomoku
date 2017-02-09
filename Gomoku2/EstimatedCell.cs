using System.Collections.Generic;

namespace Gomoku2
{
    public class EstimatedCell
    {
        public EstimatedCell(Cell cell, List<Line> myLines, int estimate)
        {
            Cell = cell;
            MyLines = myLines;
            Estimate = estimate;
        }

        public Cell Cell { get; private set; }

        public List<Line> MyLines { get; private set; }

        public int Estimate { get; set; }
    }
}