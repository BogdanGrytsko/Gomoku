using System.Collections.Generic;

namespace Gomoku2
{
    public class GameState
    {
        public GameState()
        {
            Children = new List<GameState>();
        }

        public BoardState BoardState { get; set; }
        public Cell Cell { get; set; }
        public int Estimate { get; set; }

        public GameState Parent { get; set; }
        public List<GameState> Children { get; set; }

        public void AddChild(GameState gameState)
        {
            Children.Add(gameState);
            gameState.Parent = this;
        }

        public EstimatedBoard EstimatedBoard => new EstimatedBoard
        {
            Board = BoardState.Board,
            Estimate = Estimate
        };
    }
}