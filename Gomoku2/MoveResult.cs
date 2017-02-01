namespace Gomoku2
{
    public class MoveResult
    {
        public MoveResult(Cell move, int lenght, bool found)
        {
            Move = move;
            Lenght = lenght;
            Found = found;
        }

        public Cell Move { get; set; }

        public int Lenght { get; set; }

        public bool Found { get; set; }

        public bool FoundFour
        {
            get
            {
                return Found && Lenght >= 4;
            }
        }

        public static MoveResult NotFound
        {
            get
            {
                return new MoveResult(null, 0, false);
            }
        }
    }
}