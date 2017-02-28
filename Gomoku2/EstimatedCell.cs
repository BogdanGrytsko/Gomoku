using System.Collections.Generic;

namespace Gomoku2
{
    public class EstimatedCell
    {
        public EstimatedCell(Cell cell, List<Line> myLines, List<Line> oppLines, int estimate)
        {
            Cell = cell;
            MyLines = myLines;
            Estimate = estimate;
            OppLines = oppLines;
        }

        public Cell Cell { get; }

        public List<Line> MyLines { get; }

        public List<Line> OppLines { get; }

        public int Estimate { get; }

        public override string ToString()
        {
            return $"{Cell} : {Estimate}";
        }
    }
}