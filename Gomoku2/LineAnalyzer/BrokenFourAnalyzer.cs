using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class BrokenFourAnalyzer : FourCellAnalyzer
    {
        public BrokenFourAnalyzer(Line line) : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            return LineType.BrokenFour;
        }

        public override LineType TwoSidesOpened()
        {
            return LineType.BrokenFour;
        }

        public override LineType Dead()
        {
            return LineType.BrokenFour;
        }

        public override IEnumerable<Cell> HighPriorityCells
        {
            get { yield return line.Middle1; }
        }
    }
}