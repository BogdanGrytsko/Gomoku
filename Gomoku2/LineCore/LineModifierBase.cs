using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.StateCache;

namespace Gomoku2.LineCore
{
    public abstract class LineModifierBase
    {
        protected readonly BoardStateBase state;
        protected readonly List<Cell> skipDirections;

        protected LineModifierBase(BoardStateBase state, List<Cell> skipDirections)
        {
            this.state = state;
            this.skipDirections = skipDirections;
        }

        public abstract void Modify(CellDirection cellDir);

        protected void AddMyLine(Line line)
        {
            state.MyLines.Add(line);
        }

        protected Line CreateMaxCellLine(CellDirection cellDir)
        {
            //create line with maximum possible number of cells. Distance <= 4 | X X*| X  X*| X X *|
            var cells = new List<Cell> { cellDir.AnalyzedCell };
            var line = new Line(cells, state.MyCellType);
            for (int i = cellDir.Distance + 1; i <= 4; i++)
            {
                var cell = cellDir.Analyzed(i);
                if (cell.IsType(state.Board, state.OpponentCellType)) break;
                if (cell.IsType(state.Board, state.MyCellType))
                {
                    var tmpCellDir = new CellDirection(cell, cellDir.MirrorDirection, i -  cellDir.Distance);
                    line.AddOuterCell(tmpCellDir);
                }
            }
            
            line.AddOuterCellAndEstimate(state.Board, cellDir);
            return line;
        }
    }
}