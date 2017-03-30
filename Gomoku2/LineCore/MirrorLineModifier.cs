using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.StateCache;

namespace Gomoku2.LineCore
{
    public class MirrorLineModifier : LineModifierBase
    {
        private readonly CellDirection mirrorCellDir;

        public MirrorLineModifier(BoardStateBase state, List<Cell> skipDirections, CellDirection mirrorCellDir)
            : base(state, skipDirections)
        {
            this.mirrorCellDir = mirrorCellDir;
        }

        public override void Modify(CellDirection cellDir)
        {
            var sameDirLine = state.MyLines.Filter(cellDir.AnalyzedCell, cellDir.Direction);
            var mirrorDirLine = state.MyLines.Filter(mirrorCellDir.AnalyzedCell, mirrorCellDir.Direction);
            if (sameDirLine == null && mirrorDirLine == null)
            {
                SameDirLinesNullCase(cellDir, mirrorCellDir);
                return;
            }
            //if it is same line then add middle cell, reestimate and finish
            if (sameDirLine == mirrorDirLine)
                sameDirLine.AddMiddleCell(cellDir.Cell);

            if (sameDirLine == null)
            {
                // X        |X      |
                // X * XX   |X *  XX|
                //create line with maximum possible number of cells. Distance <= 5
            }
            if (mirrorDirLine == null)
            {

            }
            //if both are not null we have  | XX * XX | X * X | | X*X |
            //if distance is 5 then add new LongBrokenThree. Remove single marks if there were any
            //if can add to line, than add to line
        }

        private void SameDirLinesNullCase(CellDirection cellDir, CellDirection mirrorCellDir)
        {
            if (cellDir.AnalyzedCell.DistSqr(mirrorCellDir.AnalyzedCell) <= 16)
                AddMyLine(CreateThreeCellLine(cellDir, mirrorCellDir));
            else
            {
                AddMyLine(CreateTwoCellLine(cellDir));
                AddMyLine(CreateTwoCellLine(mirrorCellDir));
            }
        }

        private Line CreateTwoCellLine(CellDirection cellDir)
        {
            throw new System.NotImplementedException();
        }

        private Line CreateThreeCellLine(CellDirection cellDir, CellDirection mirrorCellDir)
        {
            throw new System.NotImplementedException();
        }
    }
}