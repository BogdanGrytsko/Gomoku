﻿using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class FourInRowAnalyzer : FourCellAnalyzer
    {
        public FourInRowAnalyzer(Line line) : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            return LineType.FourInRow;
        }

        public override LineType TwoSidesOpened()
        {
            return LineType.StraightFour;
        }

        public override LineType Dead()
        {
            return LineType.DeadFour;
        }

        public override IEnumerable<Cell> HighPriorityCells => line.GetNextCells(false);
    }
}