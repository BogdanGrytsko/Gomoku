using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class BrokenTwoAnalyzer : TwoCellAnalyzer
    {
        public BrokenTwoAnalyzer(Line line)
            : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            //OX X O*
            if (cells[1].BoardCell == owner.Opponent())
                return LineType.DeadTwo;
            //OX X   
            return LineType.BlockedTwo;
        }

        public override LineType TwoSidesOpened()
        {
            var nextResult = NextOpened();
            var prevResult = PrevOpened();
            // O X X O 
            if (nextResult.IsDeadTwo() && prevResult.IsDeadTwo())
                return LineType.DeadTwo;
            //  X X  
            return LineType.TwoInRow;
        }
    }
}