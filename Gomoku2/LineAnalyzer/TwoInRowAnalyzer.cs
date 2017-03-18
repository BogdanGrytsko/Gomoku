using System.Collections.Generic;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class TwoInRowAnalyzer : AnalyzerBase
    {
        public TwoInRowAnalyzer(List<Cell> nextCells, List<Cell> prevCells, BoardCell owner) : base(nextCells, prevCells, owner)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells, ref List<Cell> priorityCells)
        {
            //OXX O*
            if (cells[1].BoardCell == owner.Opponent())
                return LineType.DeadTwo;
            //OXX X*
            if (cells[1].BoardCell == owner)
            {
                //OXX XX
                if (cells[2].BoardCell == owner)
                {
                    priorityCells = new List<Cell> { cells[0] };
                    return LineType.BrokenFourInRow;
                }
                //OXX XO
                if (cells[2].BoardCell == owner.Opponent())
                    return LineType.DeadThree;
                //OXX X 
                return LineType.BlokedThree;
            }
            //OXX  X
            if (cells[2].BoardCell == owner)
            {
                priorityCells = new List<Cell> { cells[0], cells[1] };
                return LineType.LongBlockedThree;
            }
            //OXX   
            return LineType.BlockedTwo;
        }

        public override LineType TwoSidesOpened(ref List<Cell> priorityCells)
        {
            var nextResult = NextOpened(ref priorityCells);
            var prevResult = PrevOpened(ref priorityCells);
            // O XX O 
            if (nextResult.IsDeadTwo() && prevResult.IsDeadTwo())
                return LineType.DeadTwo;
            if (nextResult.IsDeadTwo())
                priorityCells = new List<Cell> { prevCells[0], prevCells[1] };
            if (prevResult.IsDeadTwo())
                priorityCells = new List<Cell> { nextCells[0], nextCells[1] };

            //XX XX XX
            if (nextResult.IsBrokenFourInRow() && prevResult.IsBrokenFourInRow())
                return LineType.StraightFour;
            //  XX XX
            if (nextResult.IsBrokenFourInRow() || prevResult.IsBrokenFourInRow())
                return LineType.BrokenFourInRow;
            // X XX  X
            if (nextResult.IsBlokedThree() && prevResult.IsBlokedThree())
                return LineType.DoubleBrokenThree;
            // XX X 
            if (nextResult.IsBlokedThree())
            {
                priorityCells = new List<Cell> { nextCells[0], prevCells[0], nextCells[2]};
                return LineType.BrokenThree;
            }
            if (prevResult.IsBlokedThree())
            {
                priorityCells = new List<Cell> { nextCells[0], prevCells[0], prevCells[2] };
                return LineType.BrokenThree;
            }
            // XX  X
            if (nextResult.IsLongBlockedThree() || prevResult.IsLongBlockedThree())
                return LineType.LongBrokenThree;
            //OX XX  
            if (nextResult.IsDeadThree() || prevResult.IsDeadThree())
            {
                priorityCells = new List<Cell> { nextCells[0], prevCells[0] };
                return LineType.BlokedThree;
            }
            //  XX  
            return LineType.TwoInRow;
        }
    }
}