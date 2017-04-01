using System.Collections.Generic;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public abstract class AnalyzerBase
    {
        protected readonly BoardCell owner;
        protected readonly Line line;

        protected AnalyzerBase(Line line)
        {
            owner = line.owner;
            this.line = line;
        }

        public LineType PrevOpened()
        {
            return OneSideOpened(line.PrevCells);
        }

        public LineType NextOpened()
        {
            return OneSideOpened(line.NextCells);
        }

        protected abstract LineType OneSideOpened(List<Cell> cells);

        public abstract LineType TwoSidesOpened();
        public abstract LineType Dead();

        public LineType DoAnalysis()
        {
            if (IsDead)
                return Dead();
            if (!NextEmpty && PrevEmpty)
                return PrevOpened();
            if (NextEmpty && !PrevEmpty)
                return NextOpened();
            return TwoSidesOpened();
        }

        private bool IsDead => !NextEmpty && !PrevEmpty;

        private bool NextEmpty => line.next.IsEmpty || line.next.BoardCell == line.owner;

        private bool PrevEmpty => line.prev.IsEmpty || line.prev.BoardCell == line.owner;

        public virtual IEnumerable<Cell> HighPriorityCells
        {
            get { yield break; }
        }

        public virtual IEnumerable<Cell> PriorityCells => HighPriorityCells;
        public abstract int NextAnalyzeLength { get; }

        public abstract bool CanAddCell(CellDirection cellDir);
    }
}