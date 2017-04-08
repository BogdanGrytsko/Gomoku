namespace Gomoku2.CellObjects
{
    public class AlphaBetaResult
    {
        public int MinMax { get; set; }

        public Cell Move { get; set; }

        public int Alpha { get; set; }

        public int Beta { get; set; }

        public AlphaBetaResult(int minMax)
        {
            MinMax = minMax;
        }

        public AlphaBetaResult(int minMax, int alpha, int beta)
        {
            MinMax = minMax;
            Alpha = alpha;
            Beta = beta;
        }
    }
}