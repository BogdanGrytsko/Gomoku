using System.Collections.Generic;
using System.Diagnostics;
using Gomoku2;

namespace GomokuSimulator
{
    public class GamePlayer
    {
        public IEnumerable<EstimatedBoard> PlayGame()
        {
            var player1 = new GameRunner();
            var player2 = new GameRunner();

            while (true)
            {
                var player1Move = player1.DoMove();

                player2.DoOpponentMove(player1Move.X, player1Move.Y);
                yield return player1.EstimatedBoard;
                if (player1.HasFiveInARow(Gomoku2.BoardCell.First))
                    break;

                var player2Move = player2.DoMove();
                player1.DoOpponentMove(player2Move.X, player2Move.Y);
                yield return player1.EstimatedBoard;
                if (player1.HasFiveInARow(Gomoku2.BoardCell.Second))
                    break;
            }
        }
    }
}