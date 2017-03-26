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
                AddMyLine(new Line(cell, state.MyCellType));
        }

        private bool AnalyzeCell(CellDirection cellDir)
        {
            var analyzedCell = cellDir.AnalyzedCell;
            if (analyzedCell.IsType(state.Board, state.OpponentCellType))
            {
                var sameDirOppLine = state.OppLines.Filter(analyzedCell, cellDir.Direction);
                if (sameDirOppLine == null) return true;
                if (sameDirOppLine.IsCellMiddle(cellDir.Cell))
                    SplitCase(cellDir, sameDirOppLine);
                else
                    sameDirOppLine.Estimate(state.Board);
                return true;
            }
            if (!analyzedCell.IsType(state.Board, state.MyCellType)) return false;

            addedToSomeLine = true;
            var sameDirLine = state.MyLines.Filter(analyzedCell, cellDir.Direction);
            //??*X?
            //merge case is possible
            if (cellDir.Distance == 1)
                SolidCase(cellDir, sameDirLine);
            //merge is also possible : X   X => XX  X
            //now cry fcking fuck
            else
                BrokenCase(cellDir, sameDirLine);
            return true;
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
                SolidLineDoesntExist(cellDir);
            else
                SolidLineExistsCase(cellDir, sameDirLine);
        }

        private void SolidLineExistsCase(CellDirection cellDir, Line sameDirLine)
        {
            if (sameDirLine.IsCellMiddle(cellDir.Cell)) skipDirections.Add(cellDir.MirrorDirection);


            sameDirLine.AddCells(state.Board, cellDir.Cell);
        }

        private void SolidLineDoesntExist(CellDirection cellDir)
        {
            var line = new Line(cellDir.Cell, cellDir.AnalyzedCell, state.Board, state.MyCellType);
            var maybeBrokenCell = cellDir.AnalyzedCell + 2*cellDir.Direction;
            if (maybeBrokenCell.IsType(state.Board, state.MyCellType)
                && (cellDir.AnalyzedCell + cellDir.Direction).IsEmptyWithBoard(state.Board))
            {
                var additionalDir = new CellDirection(maybeBrokenCell, -cellDir.Direction, 2);
                line.AddLonelyCell(additionalDir, state.Board);
            }
            AddMyLine(line);
        }

        private void SolidMirrorCase(CellDirection cellDir, Line sameDirLine)
        {
            skipDirections.Add(cellDir.MirrorDirection);
            sameDirLine.AddMissingCell(cellDir.Cell);
        }

        private void BrokenCase(CellDirection cellDir, Line sameDirLine)
        {
            //add new BrokenTwo or LongBrokenTwo
            if (sameDirLine == null || !sameDirLine.CanAddCell(cellDir))
                BrokenLineDoesntExistCase(cellDir);
            else
                BrokenLineExistsCase(cellDir, sameDirLine);
        }

        private void BrokenLineExistsCase(CellDirection cellDir, Line sameDirLine)
        {
            if (sameDirLine.IsCellMiddle(cellDir.Cell)) skipDirections.Add(cellDir.MirrorDirection);

            var mirrorAnalyzedCell = GetMirrorCellForBrokenCase(cellDir);
            // X   X case
            if (mirrorAnalyzedCell.IsType(state.Board, state.MyCellType))
                BrokenMirrorCase(cellDir, sameDirLine, mirrorAnalyzedCell);
            else
                sameDirLine.AddLonelyCell(cellDir, state.Board);
        }

        private void BrokenMirrorCase(CellDirection cellDir, Line sameDirLine, Cell mirrorAnalyzedCell)
        {
            //todo create a separate method for this
            sameDirLine.AddLonelyCell(cellDir, state.Board);
            sameDirLine.AddCells(state.Board, mirrorAnalyzedCell);
            skipDirections.Add(cellDir.MirrorDirection);
            var mirrorLine = state.MyLines
                .FilterByCell(mirrorAnalyzedCell)
                .FirstOrDefault(l => l.Count == 1);
            //remove line if it was lonely cell
            if (mirrorLine != null)
                state.MyLines.Remove(mirrorLine);
        }

        private Cell GetMirrorCellForBrokenCase(CellDirection cellDir)
        {
            var doesntExist = new Cell(-1, -1);
            if (!directions.Contains(cellDir.MirrorDirection))
                return doesntExist;
            if (cellDir.Distance == 3)
                return cellDir.Cell + cellDir.MirrorDirection;
            if (cellDir.Distance == 2)
                return cellDir.Cell + 2*cellDir.MirrorDirection;
            return doesntExist;
        }

        private void BrokenLineDoesntExistCase(CellDirection cellDir)
        {
            var line = new Line(cellDir, state.Board, state.MyCellType);
            AddMyLine(line);
        }

        private void AddMyLine(Line line)
        {
            state.MyLines.Add(line);
        }
    }
}