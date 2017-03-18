using System.Collections.Generic;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class ThreeInRowAnalyzer : AnalyzerBase
    {
        public ThreeInRowAnalyzer(List<Cell> nextCells, List<Cell> prevCells, BoardCell owner) : base(nextCells, prevCells, owner)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells, ref List<Cell> priorityCells)
        {
            //OXXX  
            if (cells[1].IsEmpty)
                return LineType.BlokedThree;
            //OXXX X
            if (cells[1].BoardCell == owner)
            {
                priorityCells = new List<Cell> { cells[0] };
                return LineType.BrokenFourInRow;
            }
            //OXXX O
            return LineType.DeadThree;
        }

        public override LineType TwoSidesOpened(ref List<Cell> priorityCells)
        {
            var nextResult = NextOpened(ref priorityCells);
            var prevResult = PrevOpened(ref priorityCells);
            //X XXX X
            if (nextResult.IsBrokenFourInRow() && prevResult.IsBrokenFourInRow())
                return LineType.StraightFour;
            //* XXX X
            if (nextResult.IsBrokenFourInRow() || prevResult.IsBrokenFourInRow())
                return LineType.BrokenFourInRow;
            // XXX 
            return LineType.ThreeInRow;
        }
    }
}