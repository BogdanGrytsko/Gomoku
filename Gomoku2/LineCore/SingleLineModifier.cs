using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.StateCache;

namespace Gomoku2.LineCore
{
    public class SingleLineModifier : LineModifierBase
    {
        public SingleLineModifier(BoardStateBase state, List<Cell> skipDirections) : base(state, skipDirections)
        {
        }

        //means that this cell is outer cell to My line
        public override void Modify(CellDirection cellDir)
        {
            var sameDirLine = state.MyLines.Filter(cellDir.AnalyzedCell, cellDir.Direction);
            if (sameDirLine == null || !sameDirLine.CanAddCell(cellDir))
            {
                var line = CreateMaxCellLine(cellDir);
                AddMyLine(line);
            }
            else
                sameDirLine.AddOuterCellAndEstimate(state.Board, cellDir);
        }
    }
}