using System;
using Gomoku2;
using Gomoku2.CellObjects;

namespace GomokuSimulator
{
    public class HumanComputerGame
    {
        private readonly Game computerPlayer;
        private readonly EstimatedBoard estimatedBoard;
        private readonly BoardCell humanMove;
        
        public HumanComputerGame(Game computerPlayer, BoardCell humanMove)
        {
            this.computerPlayer = computerPlayer;
            this.humanMove = humanMove;
            estimatedBoard = new EstimatedBoard();
        }

        public EstimatedBoard HumanMove(Cell move)
        {
            if (estimatedBoard.Board[move.X, move.Y] != BoardCell.None) 
                throw new Exception("Cell is not empty!");
            estimatedBoard.Board[move.X, move.Y] = humanMove;
            computerPlayer.DoOpponentMove(move.X, move.Y, HumanMoveType);
            return estimatedBoard.Clone();
        }

        public bool GameIsFinished => estimatedBoard.Board.HasFiveInARow(BoardCell.First) ||
                                       estimatedBoard.Board.HasFiveInARow(BoardCell.Second);

        public EstimatedBoard ComputerMove()
        {
            var move = computerPlayer.DoMove(humanMove.Opponent(), 4, 15);
            estimatedBoard.Board[move.X, move.Y] = humanMove.Opponent();
            return estimatedBoard.Clone();
        }

        public BoardCell HumanMoveType => humanMove;
    }
}