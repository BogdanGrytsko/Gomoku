using System.Collections.Generic;
using System.Linq;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;

namespace Gomoku2.StateCache
{
    public class BoardStateBase
    {
        public List<Line> MyLines { get; private set; }

        public List<Line> OppLines { get; private set; }

        public BoardCell MyCellType { get; private set; }

        public BoardCell OpponentCellType
        {
            get { return MyCellType.Opponent(); }
        }

        public BoardCell[,] Board { get; private set; }

        public BoardStateBase(List<Line> myLines, List<Line> oppLines, BoardCell myCellType, BoardCell[,] board)
        {
            MyLines = myLines;
            OppLines = oppLines;
            MyCellType = myCellType;
            Board = board;
        }

        public BoardStateBase Clone()
        {
            var myLinesClone = new List<Line>(MyLines.Select(l => l.Clone()));
            var oppLinesClone = new List<Line>(OppLines.Select(l => l.Clone()));
            return new BoardStateBase(myLinesClone, oppLinesClone, MyCellType, Board);
        }
    }
}