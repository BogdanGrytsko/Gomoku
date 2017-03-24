using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.StateCache;

namespace Gomoku2.LineCore
{
    public class LineModifier
    {
        private bool addedToSomeLine;
        private readonly Cell cell;
        private readonly BoardStateBase state;
        private readonly List<Cell> skipDirections = new List<Cell>();
        private readonly IList<Cell> directions; 
        
        public LineModifier(Cell cell, BoardStateBase state)
            : this(cell, state, GetDirections())
        {
        }

        public LineModifier(Cell cell, BoardStateBase state, IEnumerable<Cell> directions)
        {
            addedToSomeLine = false;
            this.cell = cell;
            this.state = state;
            this.directions = directions.ToList();
        }

        private static IEnumerable<Cell> GetDirections()
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    yield return new Cell(i, j);
                }
            }
        }

        public void AddCellToLines()
        {
            //handle cases:
            //1. Cell is totally lonelly  | X |
            //2. Cell Creates new line. This can be only 2-cell line |X  X|X X| XX |
            //3. Cell Makes line longer | X => XX| XX => XXX| X X => X XX| X X => X X X|
            //4. Cell combines 2 lines into one | X XX => XXXX | XX XX => XXXXX| X X => XXX|
            //5. Cell Creates 2 new 2-cell lines. "Triangle"

            foreach (var direction in directions)
            {
                if (skipDirections.Contains(direction)) continue;
                for (int distance = 1; distance <= 3; distance++)
                {
                    var cellDir = new CellDirection(cell, direction, distance);
                    if (AnalyzeCell(cellDir)) break;
                }
            }
            if (!addedToSomeLine)
                AddLine(new Line(cell, state.MyCellType));
        }

        private bool AnalyzeCell(CellDirection cellDir)
        {
            var analyzedCell = cellDir.AnalyzedCell;
            if (analyzedCell.IsType(state.Board, state.OpponentCellType))
            {
                //todo hanle case when my cell breaks opp line. Remove cells from it
                var sameDirOppLine = state.OppLines.Filter(analyzedCell, cellDir.Direction);
                sameDirOppLine?.Estimate(state.Board);
                return true;
            }
            if (!analyzedCell.IsType(state.Board, state.MyCellType)) return false;

            addedToSomeLine = true;
            var sameDirLine = state.MyLines.Filter(analyzedCell, cellDir.Direction);
            //??*X?
            //merge case is possible
            if (cellDir.Distance == 1)
                SolidCase(cellDir, sameDirLine);
            else
                BrokenCase(cellDir, sameDirLine);
            return true;
        }

        private void SolidCase(CellDirection cellDir, Line sameDirLine)
        {
            //do mirror analyzis only when asked to.
            var mirrorAnalyzedCell = directions.Contains(cellDir.MirrorDirection)
                ? cellDir.MirrorAnalyzedCell
                : new Cell(-1, -1);
            //?X*X?
            if (mirrorAnalyzedCell.IsType(state.Board, state.MyCellType))
                SolidMirrorCase(cellDir, sameDirLine);
            else
                SolidSingleCase(cellDir, sameDirLine);
        }

        private void SolidSingleCase(CellDirection cellDir, Line sameDirLine)
        {
            if (sameDirLine == null || !sameDirLine.CanAddCell(cellDir))
            {
                var line = new Line(cellDir.Cell, cellDir.AnalyzedCell, state.Board, state.MyCellType);
                var maybeBrokenCell = cellDir.AnalyzedCell + 2*cellDir.Direction;
                if (maybeBrokenCell.IsType(state.Board, state.MyCellType))
                {
                    var additionalDir = new CellDirection(maybeBrokenCell, -cellDir.Direction, 2);
                    line.AddLonelyCell(additionalDir, state.Board);
                }
                AddLine(line);
            }
            else
                sameDirLine.AddCells(state.Board, cellDir.Cell);
        }

        private void SolidMirrorCase(CellDirection cellDir, Line sameDirLine)
        {
            skipDirections.Add(-cellDir.Direction);
            sameDirLine.AddMissingCell(cellDir.Cell);
        }

        private void BrokenCase(CellDirection cellDir, Line sameDirLine)
        {
            //add new BrokenTwo or LongBrokenTwo
            if (sameDirLine == null || !sameDirLine.CanAddCell(cellDir))
                BrokenLineDoesntExistCase(cellDir);
            // reestimate line. add lonely cell to it.
            else
                BrokenLineExistsCase(cellDir, sameDirLine);
        }

        private void BrokenLineExistsCase(CellDirection cellDir, Line sameDirLine)
        {
            sameDirLine.AddLonelyCell(cellDir, state.Board);
        }

        private void BrokenLineDoesntExistCase(CellDirection cellDir)
        {
            var line = new Line(cellDir, state.Board, state.MyCellType);
            AddLine(line);
        }

        private void AddLine(Line line)
        {
            state.MyLines.Add(line);
        }
    }
}