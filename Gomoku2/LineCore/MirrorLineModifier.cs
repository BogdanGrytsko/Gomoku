using System.Collections.Generic;
using System.Linq;
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
            var sameDirLines = state.MyLines.Filter(cellDir.AnalyzedCell, cellDir.Direction).ToList();
            var mirrorDirLines = state.MyLines.Filter(mirrorCellDir.AnalyzedCell, mirrorCellDir.Direction).ToList();

            var intersect = sameDirLines.Intersect(mirrorDirLines).ToList();
            var singleIntersect = intersect.SingleOrDefault();

            if (singleIntersect != null)
            {
                singleIntersect.AddMiddleCell(cellDir.Cell);
            }

            var sameDirLine = sameDirLines.Except(intersect).FirstOrDefault();
            var mirrorDirLine = mirrorDirLines.Except(intersect).FirstOrDefault();

            var isStrangeLongBrokenThree = StrangeLongBrokenThree(cellDir);
            if (isStrangeLongBrokenThree)
            {
                AddMyLine(CreateMaxCellLine(cellDir.Swap()));
                RemoveOneCellLine(sameDirLine);
                RemoveOneCellLine(mirrorDirLine);
            }

            var shouldNotAddLine =  singleIntersect != null || isStrangeLongBrokenThree;
            ProcessSide(cellDir, sameDirLine, shouldNotAddLine);
            ProcessSide(mirrorCellDir, mirrorDirLine, shouldNotAddLine);
        }

        private void ProcessSide(CellDirection cellDir, Line line, bool shouldNotAddLine)
        {
            if (CanAddCell(line, cellDir))
                line.AddOuterCellAndEstimate(state.Board, cellDir);
            else if (!shouldNotAddLine)
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