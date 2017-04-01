using System;
using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class FiveCellAnalyzer : AnalyzerBase
    {
        public FiveCellAnalyzer(Line line) : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            return LineType.FiveInRow;
        }

        public override LineType TwoSidesOpened()
        {
            return LineType.FiveInRow;
        }

        public override LineType Dead()
        {
            return LineType.FiveInRow;
        }

        public override int NextAnalyzeLength => 0;

        public override bool CanAddCell(CellDirection cellDir)
        {
           throw new Exception("Should never happen");
        }
    }
}