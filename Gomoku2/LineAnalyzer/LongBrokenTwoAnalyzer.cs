using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class LongBrokenTwoAnalyzer : AnalyzerBase
    {
        private readonly Cell middle1, middle2;

        public LongBrokenTwoAnalyzer(
            List<Cell> nextCells, List<Cell> prevCells, BoardCell owner, Cell middle1, Cell middle2)
            : base(nextCells, prevCells, owner)
        {
            this.middle1 = middle1;
            this.middle2 = middle2;
        }

        protected override LineType OneSideOpened(List<Cell> cells, ref List<Cell> priorityCells)
        {
            //OX  X O*
            if (cells[1].BoardCell == owner.Opponent())
                return LineType.DeadTwo;
            //OX  X  
            priorityCells = new List<Cell> { middle1, middle2 };
            return LineType.BlockedTwo;
        }

        public override LineType TwoSidesOpened(ref List<Cell> priorityCells)
        {
            var nextResult = NextOpened(ref priorityCells);
            var prevResult = PrevOpened(ref priorityCells);
            // O X  X O 
            if (nextResult.IsDeadTwo() && prevResult.IsDeadTwo())
                return LineType.DeadTwo;

            //  X  X  
            return LineType.LongBrokenTwo;
        }
    }
}