using System.Collections.Generic;
using System.Diagnostics;
using Gomoku2;

namespace GomokuSimulator
{
    public class GamePlayer
    {
        private readonly Stopwatch player1Timer = new Stopwatch();
        private readonly Stopwatch player2Timer = new Stopwatch();

        public IEnumerable<EstimatedBoard> PlayGame()
        {
            var player1 = new Gomoku2.Game(15, 15);
            var player2 = new Gomoku2.Game(15, 15);

            while (true)
            {
                player1Timer.Start();
                var player1Move = player1.DoMove();
                player1Timer.Stop();

                player2.DoOpponentMove(player1Move.X, player1Move.Y);
                yield return player1.EstimatedBoard;
                if (player1.HasFiveInARow(Gomoku2.BoardCell.First))
                    break;

                player2Timer.Start();
                var player2Move = player2.DoMove();
                player2Timer.Stop();

                player1.DoOpponentMove(player2Move.X, player2Move.Y);
                yield return player1.EstimatedBoard;
                if (player1.HasFiveInARow(Gomoku2.BoardCell.Second))
                    break;
            }
        }
    }
}