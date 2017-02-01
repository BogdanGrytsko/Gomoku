using System.Collections.Generic;
using System.Linq;

namespace Gomoku2
{
    public class BoardState
    {
        public List<Line> MyLines { get; private set; }

        public List<Line> OppLines { get; private set; }

        public BoardCell Type { get; private set; }

        public int Depth { get; private set; }

        public int MaxWidth { get; private set; }

        public BoardCell[,] Board { get; private set; }

        public BoardState(
            List<Line> myLines, List<Line> oppLines, BoardCell type, int depth, int minDepth, int maxWidth, BoardCell[,] board)
        {
            MyLines = myLines;
            OppLines = oppLines;
            Type = type;
            Depth = depth;
            Board = board;
            MaxWidth = maxWidth;
            MinDepth = minDepth;
        }

        public bool MovesFirst
        {
            get
            {
                return Type == BoardCell.First;
            }
        }

        public int StartEstimate
        {
            get
            {
                return MovesFirst ? int.MinValue : int.MaxValue;
            }
        }

        public BoardCell Opponent
        {
            get
            {
                return MovesFirst ? BoardCell.Second : BoardCell.First;
            }
        }

        public BoardState GetNextState(List<Line> myNewLines)
        {
            return new BoardState(OppLines, myNewLines, Opponent, Depth - 1, MinDepth, MaxWidth, Board);
        }

        public IEnumerable<Cell> GetNextCells(List<Cell> cellsToCheck = null)
        {
            IEnumerable<Cell> nextCells;
            var immidiateThreatCells = GetImmidiateThreatCells().ToList();
            if (immidiateThreatCells.Any())
            {
                nextCells = immidiateThreatCells;
                ////if (MinDepth >= -4)
                MinDepth--;
            }
            else nextCells = cellsToCheck ?? GetNearEmptyCells();
            return nextCells;
        }

        private int MinDepth { get; set; }

        public bool IsTerminal
        {
            get
            {
                return Depth == MinDepth;
            }
        }

        private IEnumerable<Cell> GetImmidiateThreatCells()
        {
            var threatOfFour = OppLines.Where(l => Game.ThreatOfFour(l.Estimate(Board, Opponent))).ToList();
            foreach (var cell in GetThreatCells(threatOfFour)) yield return cell;

            ////if (threatOfFour.Any()) yield break;
            ////var oppThreatOfThree = OppLines.Where(l => Game.ThreatOfThree(l.Estimate(Board, Opponent))).ToList();
            ////if (oppThreatOfThree.Any())
            ////{
            ////    foreach (var cell in GetThreatCells(oppThreatOfThree)) yield return cell;

            ////    var myThreatOfFour = MyLines.Where(l => l.Estimate(Board, Type) == LineType.BlokedThree);
            ////    foreach (var cell in GetThreatCells(myThreatOfFour)) yield return cell;

            ////    var myThreatOfThree = MyLines.Where(l => Game.ThreatOfThree(l.Estimate(Board, Type)));
            ////    foreach (var cell in GetThreatCells(myThreatOfThree)) yield return cell;
            ////}
        }

        private IEnumerable<Cell> GetThreatCells(IEnumerable<Line> lines)
        {
            foreach (var line in lines)
            {
                var moves = line.GetTwoNextCells(Board);
                if (moves.Item1 != null) yield return moves.Item1;
                if (moves.Item2 != null) yield return moves.Item2;
            }
        }

        public IEnumerable<Cell> GetNearEmptyCells()
        {
            var set = new HashSet<Cell>();
            for (int x = 0; x < 15; ++x)
            {
                for (int y = 0; y < 15; ++y)
                {
                    if (Board[x, y] != BoardCell.None)
                        set.UnionWith(Game.GetAdjustmentCells(Board, CellManager.Get(x, y)));
                }
            }
            set.UnionWith(GetNextNextCells(MyLines));
            set.UnionWith(GetNextNextCells(OppLines));
            return set.OrderBy(c => c.X * 15 + c.Y);
        }

        private IEnumerable<Cell> GetNextNextCells(List<Line> lines)
        {
            foreach (var line in lines)
            {
                var close = line.GetTwoNextCells(Board);
                var next = line.GetTwoNextNextCells(Board);
                if (close.Item1 != null && next.Item1 != null) yield return next.Item1;
                if (close.Item2 != null && next.Item2 != null) yield return next.Item2;
            }
        }

        public BoardState Clone()
        {
            var width = Board.GetLength(0);
            var height = Board.GetLength(1);
            var boardCopy = new BoardCell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    boardCopy[i, j] = Board[i, j];
                }
            }
            return new BoardState(MyLines, OppLines, Type, Depth, MinDepth, MaxWidth, boardCopy);
        }

        public BoardState Switch()
        {
            return new BoardState(OppLines, MyLines, Opponent, Depth, MinDepth, MaxWidth, Board);
        }
    }
}