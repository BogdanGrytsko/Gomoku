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
            var sameDirLine = state.MyLines.FilterFirstOrDefault(cellDir.AnalyzedCell, cellDir.Direction);
            var mirrorDirLine = state.MyLines.FilterFirstOrDefault(mirrorCellDir.AnalyzedCell, mirrorCellDir.Direction);

            //if it is same line then add middle cell, reestimate and finish
            //| X *X|
            if (sameDirLine == mirrorDirLine && sameDirLine != null)
            {
                sameDirLine.AddMiddleCell(cellDir.Cell);
                return;
            }

            var isStrangeLongBrokenThree = StrangeLongBrokenThree(cellDir);
            if (isStrangeLongBrokenThree)
            {
                AddMyLine(CreateMaxCellLine(cellDir.Swap()));
                RemoveOneCellLine(sameDirLine);
                RemoveOneCellLine(mirrorDirLine);
            }

            ProcessSide(cellDir, sameDirLine, isStrangeLongBrokenThree);
            ProcessSide(mirrorCellDir, mirrorDirLine, isStrangeLongBrokenThree);
        }

        private void ProcessSide(CellDirection cellDir, Line line, bool isStrangeLongBrokenThree)
        {
            if (CanAddCell(line, cellDir))
                line.AddOuterCellAndEstimate(state.Board, cellDir);
            else if (!isStrangeLongBrokenThree)
                AddMyLine(CreateMaxCellLine(cellDir));
        }

        private bool StrangeLongBrokenThree(CellDirection cellDir)
        {
            return cellDir.Distance + mirrorCellDir.Distance == 4;
        }

        private void RemoveOneCellLine(Line line)
        {
            if (line != null && line.Count == 1)
            {
                state.MyLines.Remove(line);
            }
        }
    }
}