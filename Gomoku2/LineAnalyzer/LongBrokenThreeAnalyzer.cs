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
            return LineType.BlokedThree;
        }

        public override LineType TwoSidesOpened()
        {
            return LineType.LongBrokenThree;
        }

        public override IEnumerable<Cell> PriorityCells
        {
            get
            {
                yield return line.Middle1;
                yield return line.Middle2;
            }
        }
    }
}