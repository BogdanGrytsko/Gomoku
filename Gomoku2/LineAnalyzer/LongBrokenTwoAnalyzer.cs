using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class LongBrokenTwoAnalyzer : TwoCellAnalyzer
    {
        public LongBrokenTwoAnalyzer(Line line)
            : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            //OX  X O*
            if (cells[1].BoardCell == owner.Opponent())
                return LineType.DeadTwo;
            //OX  X  
            return LineType.BlockedTwo;
        }

        public override LineType TwoSidesOpened()
        {
            var nextResult = NextOpened();
            var prevResult = PrevOpened();
            // O X  X O 
            if (nextResult.IsDeadTwo() && prevResult.IsDeadTwo())
                return LineType.DeadTwo;

            //  X  X  
            return LineType.LongBrokenTwo;
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