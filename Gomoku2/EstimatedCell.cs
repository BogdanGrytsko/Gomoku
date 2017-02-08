using System.Collections.Generic;

namespace Gomoku2
{
    public class EstimatedCell
    {
        public EstimatedCell(Cell cell, List<Line> myLines, int myEstimate, int oppEstimate)
        {
            Cell = cell;
            MyLines = myLines;
            MyEstimate = myEstimate;
            OppEstimate = oppEstimate;
        }

        public Cell Cell { get; private set; }

        public List<Line> MyLines { get; private set; }

        public int MyEstimate { get; private set; }

        public int OppEstimate { get; private set; }

        public int Estimate { get { return MyEstimate - OppEstimate; } }
    }
}