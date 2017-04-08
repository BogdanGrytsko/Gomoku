using System.Collections.Generic;
using Gomoku2.CellObjects;

namespace Gomoku2.StateCache
{
    public class GameState
    {
        public GameState(BoardState state, EstimatedCell estimatedCell)
        {
            BoardState = state.GetThisState(estimatedCell.MyLines, estimatedCell.OppLines);
            Cell = estimatedCell.Cell;
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