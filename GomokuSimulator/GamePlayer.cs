using System.Collections.Generic;
using Gomoku2;

namespace GomokuSimulator
{
    public class GamePlayer
    {
        private readonly PlayerWrapper player1;
        private readonly PlayerWrapper player2;
        private readonly EstimatedBoard estimatedBoard;

        public GamePlayer(string player1GameName, string player2GameName)
        {
            player1 = new PlayerWrapper(player1GameName);
            player2 = new PlayerWrapper(player2GameName);
            estimatedBoard = new EstimatedBoard();
        }

        public IEnumerable<EstimatedBoard> PlayGame()
        {
            while (true)
            {
                var player1Move = player1.DoMove();
                estimatedBoard.Board[player1Move.X, player1Move.Y] = BoardCell.First;
                estimatedBoard.Estimate = player1.LastEstimate ?? estimatedBoard.Estimate;
                player2.DoOpponentMove(player1Move.X, player1Move.Y);
                yield return estimatedBoard.Clone();
                if (estimatedBoard.Board.HasFiveInARow(BoardCell.First))
                    break;

                var player2Move = player2.DoMove();
                estimatedBoard.Board[player2Move.X, player2Move.Y] = BoardCell.Second;
                estimatedBoard.Estimate = player2.LastEstimate ?? estimatedBoard.Estimate;
                player1.DoOpponentMove(player2Move.X, player2Move.Y);
                yield return estimatedBoard.Clone();

                if (estimatedBoard.Board.HasFiveInARow(BoardCell.Second))
                    break;
            }
        }
    }
}