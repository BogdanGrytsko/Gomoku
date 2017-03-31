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

        public override void Modify(CellDirection cellDir)
        {
            var sameDirLine = state.MyLines.FilterFirstOrDefault(cellDir.AnalyzedCell, cellDir.Direction);
            if (CanAddCell(sameDirLine, cellDir))
                sameDirLine.AddOuterCellAndEstimate(state.Board, cellDir);
            else
                AddMyLine(CreateMaxCellLine(cellDir));
        }
    }
}