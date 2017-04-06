using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;
using Gomoku2.PriorityChain;

namespace Gomoku2.StateCache
{
    public class BoardState : BoardStateBase
    {
        public int Depth { get; private set; }

        public int MaxWidth { get; private set; }

        private int MinDepth { get; set; }

        public int StartDepth { get; set; }

        public BoardState(
            List<Line> myLines, List<Line> oppLines, BoardCell myCellType, int depth, int minDepth, int maxWidth, BoardCell[,] board)
            :base(myLines, oppLines, myCellType, board)
        {
            Depth = depth;
            MaxWidth = maxWidth;
            MinDepth = minDepth;
        }

        public bool ItIsFirstsTurn => MyCellType == BoardCell.First;

        public int Multiplier => ItIsFirstsTurn ? 1 : -1;

        public int StartEstimate => ItIsFirstsTurn ? int.MinValue : int.MaxValue;

        public BoardState GetNextState(List<Line> myNewLines, List<Line> oppNewLines)
        {
            return new BoardState(oppNewLines, myNewLines, OpponentCellType, Depth - 1, MinDepth, MaxWidth, Board) { StartDepth = StartDepth };
        }

        public BoardState GetThisState(List<Line> myNewLines, List<Line> oppNewLines)
        {
            return new BoardState(myNewLines, oppNewLines, MyCellType, Depth, MinDepth, MaxWidth, (BoardCell[,])Board.Clone());
        }

        public NextCells GetNextCells()
        {
            const int minMinDepth = -16;
            var threatCells = GetPriorityThreatCells();
            if (threatCells.Any())
            {
                if (threatCells.IncreasesDepth && MinDepth > minMinDepth)
                    MinDepth--;
                return new NextCells(threatCells.Cells);
            }
            return GetNearEmptyCells();
        }

        public bool IsTerminal => Depth == MinDepth;

        private PriorityCells GetPriorityThreatCells()
        {
            var algorithm = new PriorityAlgorithm(MyLines, OppLines);
            return algorithm.GetPriorityCells();
        }

        public NextCells GetNearEmptyCells()
        {
            var set = new HashSet<Cell>();
            set.UnionWith(GetPriorityCells(MyLines));

            for (int x = 0; x < 15; ++x)
            {
                for (int y = 0; y < 15; ++y)
                {
                    //todo this is TOO MUCH. Decrease this number. At least for leafs
                    if (Board[x, y] == MyCellType)
                        set.UnionWith(CellManager.Get(x, y).GetAdjustmentEmptyCells(Board));
                }
            }
            return new NextCells(set, GetPriorityCells(OppLines));
        }

        private static IEnumerable<Cell> GetPriorityCells(IEnumerable<Line> lines)
        {
            return lines.SelectMany(line => line.PriorityCells);
        }
    }
}