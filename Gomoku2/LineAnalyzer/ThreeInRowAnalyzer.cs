using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class ThreeInRowAnalyzer : ThreeCellAnalyzer
    {
        public ThreeInRowAnalyzer(Line line) : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            //OXXX  
            if (cells[1].IsEmpty)
                return LineType.BlokedThree;
            //OXXX O
            return LineType.DeadThree;
        }

        public override LineType TwoSidesOpened()
        {
            // XXX 
            return LineType.ThreeInRow;
        }

        public override IEnumerable<Cell> HighPriorityCells => line.GetNextCells(false);

        public override IEnumerable<Cell> PriorityCells => line.GetNextCells(line.LineType.IsBlokedThree());
    }
}