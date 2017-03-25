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
            state.OppLines.Sort();
            return state.MyLines;
        }

        private static IEnumerable<Cell> GetBackwardsDirections()
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
            state.MyLines.Sort();
            state.OppLines.Sort();
        }
    }
}