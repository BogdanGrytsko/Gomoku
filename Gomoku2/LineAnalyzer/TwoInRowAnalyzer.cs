using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class TwoInRowAnalyzer : TwoCellAnalyzer
    {
        public TwoInRowAnalyzer(Line line) : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            //OXX O*
            if (cells[1].BoardCell == owner.Opponent())
                return LineType.DeadTwo;
            //OXX  O
            if (cells[2].BoardCell == owner.Opponent())
                return LineType.DeadTwo;
            //OXX   
            return LineType.BlockedTwo;
        }

        public override LineType TwoSidesOpened()
        {
            //todo : estimate open space
            // XX 
            return LineType.TwoInRow;
        }
    }
}