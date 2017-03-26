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

        public override IEnumerable<Cell> PriorityCells
        {
            get
            {
                yield return line.Middle1;
                foreach (var cell in GetCellForSide(line.NextCells)) yield return cell;
                foreach (var cell in GetCellForSide(line.PrevCells)) yield return cell;
            }
        }

        public override bool CanAddCell(CellDirection cellDir)
        {
            return line.IsCellMiddle(cellDir.Cell) || cellDir.Distance <= 2;
        }

        private static IEnumerable<Cell> GetCellForSide(List<Cell> side)
        {
            if (side[0].IsEmpty && side[1].IsEmpty)
            {
                yield return side[0];
            }
        }
    }
}