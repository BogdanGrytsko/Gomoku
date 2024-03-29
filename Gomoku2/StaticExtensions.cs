﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;
using Gomoku2.StateCache;

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

        public static BoardCell WhoMovedLast(this BoardCell[,] board)
        {
            return WhoMovesNext(board).Opponent();
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
            return GetAdjustmentCells(startCell).Where(cell => cell.IsEmptyWithBoard(board));
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
            var lines = BoardFactory.GetLines(board, cellType);
            return lines.Any(l => l.Count >= 5);
        }

        public static Line FilterFirstOrDefault(this List<Line> lines, Cell analyzedCell, Cell direction)
        {
            return Filter(lines, analyzedCell, direction).FirstOrDefault();
        }

        public static IEnumerable<Line> Filter(this List<Line> lines, Cell analyzedCell, Cell direction)
        {
            return lines
               .FilterByCell(analyzedCell)
               .Where(line => line.Count == 1 || line.Direction == direction || line.Direction == -direction);
        }

        public static IEnumerable<Line> FilterByCell(this List<Line> lines, Cell analyzedCell)
        {
            foreach (var line in lines)
            {
                foreach (var lineCell in line)
                {
                    if (lineCell == analyzedCell)
                        yield return line;
                }
            }
        }

        public static int Sum(this List<Line> lines)
        {
            var estims = lines.Select(l => l.LineType).ToList();
            var sum = estims.Sum(es => (int)es);
            if (HasDoubleThreat(lines))
                sum += (int)LineType.DoubleThreat;
            return sum;
        }

        public static bool HasDoubleThreat(this List<Line> lines)
        {
            int killerLines = lines.Count(line => line.LineType.ThreatOfThree() || line.LineType.ThreatOfFour());
            return killerLines >= 2;
        }
    }
}