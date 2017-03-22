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

        protected override LineType OneSideOpened(List<Cell> cells, ref List<Cell> priorityCells)
        {
            //OX X O*
            if (cells[1].BoardCell == owner.Opponent())
                return LineType.DeadTwo;
            //OX X X*
            if (cells[1].BoardCell == owner)
            {
                //priorityCells = new List<Cell> {middle1, cells[0]};
                return LineType.BlokedThree;
            }
            //OX X   
            return LineType.BlockedTwo;
        }

        public override LineType TwoSidesOpened(ref List<Cell> priorityCells)
        {
            var nextResult = NextOpened(ref priorityCells);
            var prevResult = PrevOpened(ref priorityCells);
            // O X X O 
            if (nextResult.IsDeadTwo() && prevResult.IsDeadTwo())
                return LineType.DeadTwo;
            if (nextResult.IsDeadTwo())
                //priorityCells = new List<Cell> { prevCells[0], prevCells[1], middle1 };
            if (prevResult.IsDeadTwo())
                //priorityCells = new List<Cell> { nextCells[0], nextCells[1], middle1 };

            // X X X 
            if (nextResult.IsBlokedThree() || prevResult.IsBlokedThree())
                return LineType.LongBrokenThree;
            //  X X  
            return LineType.TwoInRow;
        }
    }
}