using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.StateCache;

namespace Gomoku2.LineCore
{
    public class LineFactory
    {
        private readonly HashSet<Cell> usedCells = new HashSet<Cell>();
        private readonly BoardCell[,] board;
        private readonly BoardCell type;

        public LineFactory(BoardCell[,] board, BoardCell type)
        {
            this.board = board;
            this.type = type;
        }

        //todo change to return whole state
        private List<Line> GetState()
        {
            var state = new BoardStateBase(new List<Line>(), new List<Line>(), type, board);
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    var cell = CellManager.Get(i, j);
                    if (board[i, j] != type || usedCells.Contains(cell)) continue;
                    var factory = new LineModifier(cell, state, GetBackwardsDirections());
                    factory.AddCellToLines();
                    foreach (var lineCell in state.MyLines.SelectMany(l => l))
                        usedCells.Add(lineCell);
                }
            }
            state.MyLines.Sort();
            return state.MyLines;
        }

        private IEnumerable<Cell> GetBackwardsDirections()
        {
            yield return new Cell(-1, 0);
            yield return new Cell(-1, -1);
            yield return new Cell(0, -1);
            yield return new Cell(-1, 1);
        }

        public static List<Line> GetLines(BoardCell[,] board, BoardCell type)
        {
            var factory = new LineFactory(board, type);
            return factory.GetState();
        }

        public static void AddCellToLines(Cell cell, BoardStateBase state)
        {
            var factory = new LineModifier(cell, state);
            factory.AddCellToLines();
        }

        //public static void FillLines(Cell cell, BoardStateBase state)
        //{
        //    var lines = state.MyLines;
        //    var board = state.Board;
        //    var cellType = state.MyCellType;

        //    var cellsUsedInAdding = new HashSet<Cell>();
        //    var addedToSomeLine = false;
        //    var usedLines = new List<Line>();
        //    for (int i = lines.Count - 1; i >= 0; i--)
        //    {
        //        var line = lines[i];
        //        if (!line.JoinIfPossible(cell, board)) continue;

        //        cellsUsedInAdding.UnionWith(line);
        //        if (!line.IsBrokenTwo)
        //            addedToSomeLine = true;
        //        MergeLines(lines, usedLines, line, board);
        //    }
        //    for (int i = lines.Count - 1; i >= 0; i--)
        //    {
        //        var line = lines[i];
        //        foreach (var lineCell in line)
        //        {
        //            if (cellsUsedInAdding.Contains(lineCell)) continue;
        //            var dist = lineCell.DistSqr(cell);
        //            if (Line.IsBrokenTwoDistance(dist))
        //            {
        //                var brokenTwoLine = new Line(cell, cellType);
        //                //todo rewrite in a way that lines.Contains(brokenTwoLine) is never true
        //                if (brokenTwoLine.JoinIfPossible(lineCell, board) && !lines.Contains(brokenTwoLine))
        //                {
        //                    lines.Add(brokenTwoLine);
        //                    //todo not sure how much of this is needed
        //                    cellsUsedInAdding.Add(lineCell);
        //                    MergeLines(lines, usedLines, brokenTwoLine, board);
        //                    continue;
        //                }
        //            }

        //            //todo this shouldn't happen but it happens. Investigate and remove
        //            if (dist == 0) continue;
        //            if (dist > 2) continue;

        //            var newLine = new Line(cell, lineCell, board, cellType);
        //            if (!lines.Contains(newLine))
        //            {
        //                lines.Add(newLine);
        //                addedToSomeLine = true;
        //                cellsUsedInAdding.Add(lineCell);
        //                MergeLines(lines, usedLines, newLine, board);
        //            }
        //        }
        //    }
        //    if (!addedToSomeLine)
        //    {
        //        lines.Add(new Line(cell, cellType));
        //    }
        //}

        //private static void MergeLines(List<Line> lines, List<Line> usedLines, Line line, BoardCell[,] board)
        //{
        //    Line mergedLine = null;
        //    foreach (var usedLine in usedLines.ToList())
        //    {
        //        if (!usedLine.HasSameDirection(line)) continue;

        //        mergedLine = usedLine.GetMergedLine(line, board);
        //        usedLines.Remove(usedLine);
        //        lines.Remove(usedLine);
        //        lines.Remove(line);
        //        lines.Add(mergedLine);
        //    }
        //    if (mergedLine == null) usedLines.Add(line);
        //}
    }
}