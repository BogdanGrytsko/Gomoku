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

        public List<GameState> Children { get; set; }

        public EstimatedBoard EstimatedBoard
        {
            get
            {
                return new EstimatedBoard
                {
                    Board = BoardState.Board,
                    Estimate = Estimate
                };
            }
        }
    }
}