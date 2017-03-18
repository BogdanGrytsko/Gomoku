using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;

namespace Gomoku2.LineCore
{
    public class LineFactory
    {
        public static List<Line> GetLines(BoardCell[,] board, BoardCell type)
        {
            var lines = new List<Line>();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] != type) continue;
                    FillLines(CellManager.Get(i, j), lines, type, board);
                }
            }
            lines.ForEach(l => l.Estimate(board));
            lines.Sort();
            return lines;
        }

        public static void FillLines(Cell cell, List<Line> lines, BoardCell cellType, BoardCell[,] board)
        {
            //todo use for better join\merge algorithm;
            //var adjustmentCells = cell.GetAdjustmentCells(board, cellType);
            //foreach (var adjustmentCell in adjustmentCells)
            //{

            //}

            var cellsUsedInAdding = new HashSet<Cell>();
            var addedToSomeLine = false;
            var usedLines = new List<Line>();
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                if (!line.JoinIfPossible(cell, board)) continue;

                cellsUsedInAdding.UnionWith(line);
                addedToSomeLine = true;
                MergeLines(lines, usedLines, line, board);
            }
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                foreach (var lineCell in line)
                {
                    if (cellsUsedInAdding.Contains(lineCell)) continue;
                    var dist = lineCell.DistSqr(cell);
                    if (Line.IsBrokenTwoDistance(dist))
                    {
                        var brokenTwoLine = new Line(cell, cellType);
                        if (brokenTwoLine.JoinIfPossible(lineCell, board) && !lines.Contains(brokenTwoLine))
                        {
                            lines.Add(brokenTwoLine);
                            //todo not sure how much of this is needed
                            addedToSomeLine = true;
                            cellsUsedInAdding.Add(lineCell);
                            MergeLines(lines, usedLines, brokenTwoLine, board);
                            continue;
                        }
                    }

                    //todo this shouldn't happen but it happens. Investigate and remove
                    if (dist == 0) continue;
                    if (dist > 2) continue;

                    var newLine = new Line(cell, lineCell, board, cellType);
                    if (!lines.Contains(newLine))
                    {
                        lines.Add(newLine);
                        addedToSomeLine = true;
                        cellsUsedInAdding.Add(lineCell);
                        MergeLines(lines, usedLines, newLine, board);
                    }
                }
            }
            if (!addedToSomeLine)
            {
                lines.Add(new Line(cell, cellType));
            }
        }

        private static void MergeLines(List<Line> lines, List<Line> usedLines, Line line, BoardCell[,] board)
        {
            Line mergedLine = null;
            foreach (var usedLine in usedLines.ToList())
            {
                if (!usedLine.HasSameDirection(line)) continue;

                mergedLine = usedLine.GetMergedLine(line, board);
                usedLines.Remove(usedLine);
                lines.Remove(usedLine);
                lines.Remove(line);
                lines.Add(mergedLine);
            }
            if (mergedLine == null) usedLines.Add(line);
        }
    }
}