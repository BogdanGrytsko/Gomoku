using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class OneCellAnalyzer : AnalyzerBase
    {
        public OneCellAnalyzer(Line line) : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            throw new System.NotImplementedException();
        }

        public override LineType TwoSidesOpened()
        {
            throw new System.NotImplementedException();
        }

        public override LineType Dead()
        {
            return LineType.SingleMark;
        }
    }
}