using System.Collections.Generic;
using System.Diagnostics;
using Gomoku2;

namespace GomokuSimulator
{
    public class GamePlayer
    {
        public IEnumerable<EstimatedBoard> PlayGame()
        {
            var player1 = new Game(15,15);
            var player2 = new Game(15,15);

            while (true)
            {
                var player1Move = player1.DoMove();

                player2.DoOpponentMove(player1Move.X, player1Move.Y);
                yield return player1.EstimatedBoard;
                if (player1.HasFiveInARow(Gomoku2.BoardCell.First))
                    break;

                var player2Move = player2.DoMove();
                player1.DoOpponentMove(player2Move.X, player2Move.Y);
                yield return
                    new EstimatedBoard
                    {
                        Board = player1.EstimatedBoard.Board,
                        Estimate = -player2.EstimatedBoard.Estimate
                    };

                if (player1.HasFiveInARow(Gomoku2.BoardCell.Second))
                    break;
            }
        }
    }
}