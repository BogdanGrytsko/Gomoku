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

        public Cell AnalyzedCell => analyzedCell ?? (analyzedCell = Cell + Distance*Direction);

        public Cell MirrorAnalyzedCell => mirrorAnalyzedCell ?? (mirrorAnalyzedCell = Cell - Distance*Direction);
    }
}