using System.Collections.Generic;
using Gomoku2;

namespace GomokuSimulator
{
    public class GameRunner
    {
        public IEnumerable<EstimatedBoard> PlayGame()
        {
            var player1 = new Gomoku2.Game(15, 15);
            var player2 = new Gomoku.Game(15, 15);

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