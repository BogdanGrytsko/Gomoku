using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.StateCache;

namespace Gomoku2.LineCore
{
    public class OpponentLineModifier : LineModifierBase
    {
        public OpponentLineModifier(BoardStateBase state, List<Cell> skipDirections) : base(state, skipDirections)
        {
        }

        public override void Modify(CellDirection cellDir)
        {
            var sameDirOppLine = state.OppLines.Filter(cellDir.AnalyzedCell, cellDir.Direction);
            if (sameDirOppLine == null) return;
            if (sameDirOppLine.IsCellMiddle(cellDir.Cell))
                SplitCase(cellDir, sameDirOppLine);
            else
                sameDirOppLine.Estimate(state.Board);
        }

        private void SplitCase(CellDirection cellDir, Line sameDirOppLine)
        {
            skipDirections.Add(cellDir.MirrorDirection);
            var cells = sameDirOppLine.ExtractCells(cellDir.Cell, state.Board).ToList();
            if (sameDirOppLine.Count == 1 && state.OppLines.FilterByCell(sameDirOppLine.Start).Count() >= 2)
                state.OppLines.Remove(sameDirOppLine);
            if (cells.Count == 1 && state.OppLines.FilterByCell(cells[0]).Any())
                return;

            var line = new Line(cells, state.OpponentCellType, state.Board);
            state.OppLines.Add(line);
        }
    }
}