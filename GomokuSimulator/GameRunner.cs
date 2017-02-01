using System.Collections.Generic;

namespace GomokuSimulator
{
    public class GameRunner
    {
        public IEnumerable<Gomoku2.BoardCell[,]> PlayGame()
        {
            var player1 = new Gomoku2.Game(15, 15);
            var player2 = new Gomoku.Game(15, 15);

            bool shouldContinuePlaying = true;
            while (shouldContinuePlaying)
            {
                var player1Move = player1.DoMove();
                player2.DoOpponentMove(player1Move.X, player1Move.Y);
                yield return CloneBoard(player1.Board);
                if (player1.HasFiveInARow(Gomoku2.BoardCell.First))
                    shouldContinuePlaying = false;

                var player2Move = player2.DoMove();
                player1.DoOpponentMove(player2Move.X, player2Move.Y);
                yield return CloneBoard(player1.Board);
                if (player1.HasFiveInARow(Gomoku2.BoardCell.Second))
                    shouldContinuePlaying = false;
            }
        }

        private static Gomoku2.BoardCell[,] CloneBoard(Gomoku2.BoardCell[,] board)
        {
            return (Gomoku2.BoardCell[,])board.Clone();
        }
    }
}