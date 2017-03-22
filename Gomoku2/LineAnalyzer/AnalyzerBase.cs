using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public abstract class AnalyzerBase
    {
        protected readonly List<Cell> nextCells, prevCells;
        protected readonly BoardCell owner;

        protected AnalyzerBase(Line line)
        {
            nextCells = line.NextCells;
            prevCells = line.PrevCells;
            owner = line.owner;
        }

        public LineType PrevOpened(ref List<Cell> priorityCells)
        {
            return OneSideOpened(prevCells, ref priorityCells);
        }

        public LineType NextOpened(ref List<Cell> priorityCells)
        {
            return OneSideOpened(nextCells, ref priorityCells);
        }

        protected abstract LineType OneSideOpened(List<Cell> cells, ref List<Cell> priorityCells);

        public abstract LineType TwoSidesOpened(ref List<Cell> priorityCells);
        public abstract LineType Dead();
    }
}