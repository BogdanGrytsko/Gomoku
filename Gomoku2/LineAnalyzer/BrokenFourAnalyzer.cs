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

        protected override LineType OneSideOpened(List<Cell> cells, ref List<Cell> priorityCells)
        {
            return LineType.BrokenFour;
        }

        public override LineType TwoSidesOpened(ref List<Cell> priorityCells)
        {
            return LineType.BrokenFour;
        }

        public override LineType Dead()
        {
            return LineType.BrokenFour;
        }
    }
}