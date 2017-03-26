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

        public override IEnumerable<Cell> PriorityCells
        {
            get
            {
                foreach (var cell in GetCellForSide(line.NextCells)) yield return cell;
                foreach (var cell in GetCellForSide(line.PrevCells)) yield return cell;
            }
        }

        public override bool CanAddCell(CellDirection cellDir)
        {
            return true;
        }

        private static IEnumerable<Cell> GetCellForSide(List<Cell> side)
        {
            if (side[0].IsEmpty && side[1].IsEmpty)
            {
                yield return side[0];
                if (side[2].IsEmpty)
                    yield return side[1];
            }
        }
    }
}