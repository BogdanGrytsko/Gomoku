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
            var sameDirOppLines = state.OppLines.Filter(cellDir.AnalyzedCell, cellDir.Direction).ToList();
            if (!sameDirOppLines.Any()) return;
            if (sameDirOppLines.Count >= 2 && sameDirOppLines.All(l => l.IsCellMiddle(cellDir.Cell)))
            {
                SplitCase(cellDir, sameDirOppLines);
                return;
            }
            foreach (var sameDirOppLine in sameDirOppLines)
            {
                if (sameDirOppLine.IsCellMiddle(cellDir.Cell))
                    SplitCase(cellDir, new List<Line> { sameDirOppLine });
                else
                    sameDirOppLine.CalcPropsAndEstimate(state.Board);
            }
        }

        private void SplitCase(CellDirection cellDir, List<Line> sameDirOppLines)
        {
            skipDirections.Add(cellDir.MirrorDirection);
            foreach (var sameDirOppLine in sameDirOppLines)
                state.OppLines.Remove(sameDirOppLine);
            AddOpponentLine(GetMaxLine(cellDir.AnalyzedCell, cellDir.Direction));
            AddOpponentLine(GetMaxLine(cellDir.Cell + cellDir.MirrorDirection, cellDir.MirrorDirection));
        }

        private Line GetMaxLine(Cell startCell, Cell direction)
        {
            Line line = null;
            var space = 1;
            for (int i = 0; i <= 4; i++)
            {
                var cell = startCell + i*direction;
                if (cell.IsEmptyWithBoard(state.Board)) space++;
                if (cell.IsType(state.Board, state.MyCellType)) break;
                if (cell.IsType(state.Board, state.OpponentCellType))
                {
                    if (line == null)
                    {
                        line = new Line(cell, state.OpponentCellType);
                        space = 1;
                    }
                    else
                    {
                        //don't allow very long broken two
                        if (space == 4) break;
                        var cellDir = new CellDirection(cell, -direction, space);
                        //todo this is a bit too much. reduce maybe. no need to estimate etc
                        line.AddOuterCellAndEstimate(state.Board, cellDir);
                        space = 1;
                    }
                }
            }
            return line;
        }

        private void AddOpponentLine(Line line)
        {
            if (state.OppLines.Contains(line))
            {
                //swap old line with new fresh estimated
                state.OppLines.Remove(line);
                state.OppLines.Add(line);
                return;
            }
            if (line.Count == 1 && state.OppLines.FilterByCell(line.Start).Any())return;

            state.OppLines.Add(line);
        }
    }
}