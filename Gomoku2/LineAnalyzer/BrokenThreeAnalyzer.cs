using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public class BrokenThreeAnalyzer : ThreeCellAnalyzer
    {
        public BrokenThreeAnalyzer(Line line) : base(line)
        {
        }

        protected override LineType OneSideOpened(List<Cell> cells)
        {
            return LineType.BlokedThree;
        }

        public override LineType TwoSidesOpened()
        {
            return LineType.BrokenThree;
        }

        public override IEnumerable<Cell> HighPriorityCells
        {
            get { yield return line.Middle1; }
        }

        public override IEnumerable<Cell> PriorityCells
        {
            get
            {
                yield return line.Middle1;
                foreach (var nextCell in line.GetNextCells(false))
                {
                    yield return nextCell;
                }
            }
        }

        //todo do better priority cells for this line
    }
}