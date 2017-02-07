using System;
using System.Diagnostics;
using Gomoku2;

namespace GomokuSimulator
{
    public class GameRunner
    {
        //todo create this via reflection;
        private readonly Game game = new Game(15, 15);
        private readonly Stopwatch timer = new Stopwatch();

        public Cell DoMove()
        {
            timer.Start();
            var playerMove = game.DoMove();
            timer.Stop();
            return playerMove;
        }

        public TimeSpan Elapsed { get { return timer.Elapsed; } }
        public EstimatedBoard EstimatedBoard { get { return game.EstimatedBoard; } }

        public void DoOpponentMove(int x, int y)
        {
            game.DoOpponentMove(x, y);
        }

        public bool HasFiveInARow(BoardCell cellType)
        {
            return game.HasFiveInARow(cellType);
        }
    }
}