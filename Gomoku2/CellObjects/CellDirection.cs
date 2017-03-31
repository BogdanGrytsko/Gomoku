namespace Gomoku2.CellObjects
{
    public class CellDirection
    {
        private Cell analyzedCell, mirrorAnalyzedCell;

        public Cell Cell { get; private set; }

        public Cell Direction { get; private set; }

        public int Distance { get; private set; }

        public CellDirection(Cell cell, Cell direction, int distance)
        {
            Cell = cell;
            Direction = direction;
            Distance = distance;
        }

        public Cell MirrorDirection => -Direction;

        public Cell AnalyzedCell => analyzedCell ?? (analyzedCell = Cell + Distance*Direction);

        public Cell MirrorAnalyzedCell => mirrorAnalyzedCell ?? (mirrorAnalyzedCell = Cell + Distance*MirrorDirection);

        public Cell Analyzed1 => Cell + Direction;

        public Cell Analyzed2 => Cell + 2*Direction;

        public Cell Analyzed(int i)
        {
            return Cell + i*Direction;
        }

        public CellDirection Swap()
        {
            return new CellDirection(AnalyzedCell, MirrorDirection, Distance);
        }
    }
}