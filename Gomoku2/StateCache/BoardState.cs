﻿using System.Collections.Generic;
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

        public bool AllowParallelize(IEnumerable<EstimatedCell> cells)
        {
            if (IsTerminal)
                return false;
            return StartDepth - Depth <= 4 && cells.Count() >= 2;
        }

        private PriorityCells GetPriorityThreatCells()
        {
            var algorithm = new PriorityAlgorithm(MyLines, OppLines);
            return algorithm.GetPriorityCells();
        }

        private NextCells GetNearEmptyCells()
        {
            var set = new HashSet<Cell>();
            set.UnionWith(PriorityHandlerBase.GetPriorityCells(MyLines, t => true));

            for (int x = 0; x < 15; ++x)
            {
                for (int y = 0; y < 15; ++y)
                {
                    //todo this is TOO MUCH. Decrease this number. At least for leafs
                    if (Board[x, y] == MyCellType)
                        set.UnionWith(CellManager.Get(x, y).GetAdjustmentEmptyCells(Board));
                }
            }
            return new NextCells(set, PriorityHandlerBase.GetPriorityCells(OppLines, t => true));
        }

        public BoardState GetNextState(BoardStateBase newState)
        {
            return new BoardState(newState.OppLines, newState.MyLines, OpponentCellType, Depth - 1, MinDepth, MaxWidth, newState.Board) { StartDepth = StartDepth };
        }
    }
}