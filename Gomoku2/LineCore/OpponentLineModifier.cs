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
                SuperSplitCase(cellDir, sameDirOppLines);
                return;
            }
            foreach (var sameDirOppLine in sameDirOppLines)
            {
                //todo : use super split case
                if (sameDirOppLine.IsCellMiddle(cellDir.Cell))
                    SplitCase(cellDir, sameDirOppLine);
                else
                    sameDirOppLine.CalcPropsAndEstimate(state.Board);
            }
        }

        private void SuperSplitCase(CellDirection cellDir, List<Line> sameDirOppLines)
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
            for (int i = 0; i <= 3; i++)
            {
                var cell = startCell + i*direction;
                if (cell.IsEmptyWithBoard(state.Board)) space++;
                if (cell.IsType(state.Board, state.MyCellType)) break;
                if (cell.IsType(state.Board, state.OpponentCellType))
                {
                    if (line == null)
                        line = new Line(cell, state.OpponentCellType);
                    else
                    {
                        //todo this is a bit too much. reduce maybe.
                        var cellDir = new CellDirection(cell, -direction, space);
                        line.AddOuterCellAndEstimate(state.Board, cellDir);
                        space = 1;
                    }
                }
            }
            return line;
        }

        private void AddOpponentLine(Line line)
        {
            state.OppLines.Add(line);
        }

        private void SplitCase(CellDirection cellDir, Line sameDirOppLine)
        {
            skipDirections.Add(cellDir.MirrorDirection);
            var cells = sameDirOppLine.ExtractCells(cellDir.Cell, state.Board).ToList();
            if (LineIsOneCellAndOtherLineExists(sameDirOppLine)
                || TwoOrMoreLinesExistInCell(sameDirOppLine.Start, cellDir.Direction)
                || TwoOrMoreLinesExistInCell(sameDirOppLine.End, cellDir.Direction))
                state.OppLines.Remove(sameDirOppLine);
            if (cells.Count == 1 && state.OppLines.FilterByCell(cells[0]).Any())
                return;

            var line = new Line(cells, state.OpponentCellType, state.Board);
            state.OppLines.Add(line);
        }

        private bool LineIsOneCellAndOtherLineExists(Line sameDirOppLine)
        {
            return sameDirOppLine.Count == 1 && state.OppLines.FilterByCell(sameDirOppLine.Start).Count() >= 2;
        }

        private bool TwoOrMoreLinesExistInCell(Cell cell, Cell direction)
        {
            return state.OppLines.Filter(cell, direction).Count() >= 2;
        }
    }
}