using System.Collections.Generic;
using System.Linq;
using Gomoku2.LineCore;

namespace Gomoku2.CellObjects
{
    public class EstimatedCell
    {
        public EstimatedCell(Cell cell, List<Line> myLines, List<Line> oppLines, int estimate, BoardCell[,] board)
        {
            Cell = cell;
            MyLines = myLines;
            Estimate = estimate;
            OppLines = oppLines;
            var fourSeven = MyLines.Where(l => l.Contains(new Cell(4, 7))).ToList();
            if (fourSeven.Count == 2 && (fourSeven[0].Count == 1 || fourSeven[1].Count == 1))
            {
                BoardExportImport.Export(new EstimatedBoard { Board = board }, "Before(4,7)breaks.txt");
            }
        }

        public Cell Cell { get; }

        public List<Line> MyLines { get; }

        public List<Line> OppLines { get; }

        public int Estimate { get; }

        public override string ToString()
        {
            return $"{Cell} : {Estimate}";
        }
    }
}