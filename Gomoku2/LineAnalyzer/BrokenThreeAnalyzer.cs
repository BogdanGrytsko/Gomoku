using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class BrokenThreeAnalyzer : ThreeCellAnalyzer
    {
        public BrokenThreeAnalyzer(Line line) : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            return LineType.BlokedThree;
        }

        public override LineType TwoSidesOpened()
        {
            return LineType.BrokenThree;
        }
    }
}