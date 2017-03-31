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
        private readonly LineModifierBase opponentLineModifier, singleLineModifier;

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
            opponentLineModifier = new OpponentLineModifier(state, skipDirections);
            singleLineModifier = new SingleLineModifier(state, skipDirections);
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
            foreach (var direction in directions)
            {
                if (skipDirections.Contains(direction)) continue;
                for (int distance = 1; distance <= 3; distance++)
                {
                    var cellDir = new CellDirection(cell, direction, distance);
                    if (AnalyzeCellDir(cellDir)) break;
                }
            }
            if (!addedToSomeLine)
                AddMyLine(new Line(cell, state.MyCellType));
        }

        private bool AnalyzeCellDir(CellDirection cellDir)
        {
            var analyzedCell = cellDir.AnalyzedCell;
            if (analyzedCell.IsType(state.Board, state.OpponentCellType))
            {
                opponentLineModifier.Modify(cellDir);
                return true;
            }
            if (!analyzedCell.IsType(state.Board, state.MyCellType)) return false;

            //My cell was found
            addedToSomeLine = true;

            if (directions.Contains(cellDir.MirrorDirection))
            {
                //spawn search in mirror direction,
                for (int distance = 1; distance <= 3; distance++)
                {
                    var mirrorCellDir = new CellDirection(cell, cellDir.MirrorDirection, distance);
                    if (mirrorCellDir.AnalyzedCell.IsType(state.Board, state.OpponentCellType))
                    {
                        // === to just reestimate, since it can't be split case
                        opponentLineModifier.Modify(mirrorCellDir);
                        singleLineModifier.Modify(cellDir);
                        skipDirections.Add(cellDir.MirrorDirection);
                        return true;
                    }
                    if (mirrorCellDir.AnalyzedCell.IsType(state.Board, state.MyCellType))
                    {
                        var mirrorModifier = new MirrorLineModifier(state, skipDirections, mirrorCellDir);
                        mirrorModifier.Modify(cellDir);
                        skipDirections.Add(cellDir.MirrorDirection);
                        return true;
                    }
                }
            }

            singleLineModifier.Modify(cellDir);
            return true;
        }

        private void AddMyLine(Line line)
        {
            state.MyLines.Add(line);
        }
    }
}