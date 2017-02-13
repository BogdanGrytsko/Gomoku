namespace Gomoku2
{
    public class EstimatedBoard
    {
        public EstimatedBoard()
        {
            Board = new BoardCell[15, 15];
        }

        public BoardCell[,] Board { get; set; }
        public int Estimate { get; set; }

        public EstimatedBoard Clone()
        {
            return new EstimatedBoard
            {
                Board = (BoardCell[,])Board.Clone(),
                Estimate = Estimate
            };
        }
    }
}