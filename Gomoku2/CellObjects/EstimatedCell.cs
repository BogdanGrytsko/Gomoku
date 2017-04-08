using Gomoku2.StateCache;

namespace Gomoku2.CellObjects
{
    public class EstimatedCell
    {
        public EstimatedCell(Cell cell, int estimate, BoardStateBase newState)
        {
            Cell = cell;
            Estimate = estimate;
            BoardState = newState;
        }

        public Cell Cell { get; }

        public int Estimate { get; }

        public BoardStateBase BoardState { get; }

        public override string ToString()
        {
            return $"{Cell} : {Estimate}";
        }
    }
}