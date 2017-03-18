using System.Collections.Generic;

namespace Gomoku2.CellObjects
{
    public class CellManager
    {
        private static readonly Dictionary<int, Cell> cells = new Dictionary<int, Cell>();

        public static Cell Get(int x, int y)
        {
            return new Cell(x, y);
            //Cell cell;
            //if (cells.TryGetValue(x * 225 + y, out cell)) return cell;
            //cell = new Cell(x, y);
            //cells.Add(x * 225 + y, cell);
            //return cell;
        }
    }
}