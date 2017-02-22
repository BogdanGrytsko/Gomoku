using System;
using System.Collections.Generic;
using System.Linq;

namespace Gomoku2
{
    public static class StaticExtensions
    {
        public static BoardCell Opponent(this BoardCell boardCell)
        {
            switch (boardCell)
            {
                case BoardCell.First:
                    return BoardCell.Second;
                case BoardCell.Second:
                    return BoardCell.First;
            }
            throw new Exception("No opponent for this type of cell");
        }

        public static BoardCell WhoMovesNext(this BoardCell[,] board)
        {
            var firstMoves = 0;
            var secondMoves = 0;
            foreach (var boardCell in board)
            {
                if (boardCell == BoardCell.First)
                    firstMoves++;
                if (boardCell == BoardCell.Second)
                    secondMoves++;
            }
            return firstMoves > secondMoves ? BoardCell.Second : BoardCell.First;
        }

        private static IEnumerable<Cell> GetAdjustmentCells(this Cell startCell)
        {
            var x = startCell.X;
            var y = startCell.Y;
            var cell = CellManager.Get(x + 1, y + 1);
            yield return cell;
            cell = CellManager.Get(x + 1, y - 1);
            yield return cell;
            cell = CellManager.Get(x - 1, y + 1);
            yield return cell;
            cell = CellManager.Get(x - 1, y - 1);
            yield return cell;

            cell = CellManager.Get(x + 1, y);
            yield return cell;
            cell = CellManager.Get(x - 1, y);
            yield return cell;
            cell = CellManager.Get(x, y + 1);
            yield return cell;
            cell = CellManager.Get(x, y - 1);
            yield return cell;
        }

        public static IEnumerable<Cell> GetAdjustmentEmptyCells(this Cell startCell, BoardCell[,] board)
        {
            return GetAdjustmentCells(startCell).Where(cell => cell.IsEmpty(board));
        }

        public static IEnumerable<Cell> GetAdjustmentCells(this Cell startCell, BoardCell[,] board, BoardCell cellType)
        {
            return GetAdjustmentCells(startCell).Where(cell => cell.IsType(board, cellType));
        }

        public static int TotalStateCount(this IEnumerable<GameState> gameStates)
        {
            return gameStates.Sum(gs => gs.Children.Any() ? TotalStateCount(gs.Children) + 1 : 1);
        }

        public static string GetCellText(this BoardCell boardCell)
        {
            switch (boardCell)
            {
                case BoardCell.None:
                    return " ";
                case BoardCell.First:
                    return "X";
                case BoardCell.Second:
                    return "O";
            }
            return String.Empty;
        }

        public static bool HasFiveInARow(this BoardCell[,] board, BoardCell cellType)
        {
            var lines = Game.GetLines(board, cellType);
            return lines.Any(l => l.Count >= 5);
        }
    }
}