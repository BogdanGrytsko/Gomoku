using System;

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
    }
}