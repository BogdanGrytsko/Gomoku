using System;

namespace Gomoku2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game(15, 15);
            bool skip = Console.ReadLine().Trim() == "First";
            int x, y;
            while (true)
            {
                if (skip == false)
                {
                    string[] input = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    x = Int32.Parse(input[0]);
                    y = Int32.Parse(input[1]);
                    game.DoOpponentMove(x, y);
                }
                skip = false;
                var move = game.DoMove();
                x = move.X;
                y = move.Y;
                Console.WriteLine("{0} {1}", x, y);
                Console.Out.Flush();
            }
        }
    }
}
