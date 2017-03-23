using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class LongBrokenThreeAnalyzer : ThreeCellAnalyzer
    {
        public LongBrokenThreeAnalyzer(Line line) : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            return LineType.LongBlockedThree;
        }

        public override LineType TwoSidesOpened()
        {
            return LineType.LongBrokenThree;
        }
    }
}