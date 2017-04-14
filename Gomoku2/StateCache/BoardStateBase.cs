using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private BoardStateBase Clone()
        {
            var myLinesClone = new List<Line>(MyLines.Select(l => l.Clone()));
            var oppLinesClone = new List<Line>(OppLines.Select(l => l.Clone()));
            return new BoardStateBase(myLinesClone, oppLinesClone, MyCellType, (BoardCell[,])Board.Clone());
        }

        public BoardStateBase GetNewState(Cell cell)
        {
            var clonedState = Clone();
            BoardFactory.AddCellToLines(cell, clonedState);
            return clonedState;
        }

        //public string Export()
        //{
        //    var sb = new StringBuilder();
        //    bool lineEncountered = false;
        //    for (int x = 0; x < Board.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < Board.GetLength(1); y++)
        //        {
        //            if (!lineEncountered && Board[x, y] != BoardCell.None)
        //            {
        //                lineEncountered = true;
        //                sb.Append($"{x};{y};");

        //            }
        //            if 
                        
                    
        //            sw.Write(board[x, y].GetCellText());
        //        }
        //        sw.WriteLine();
        //    }
        //}

        private Tuple<int, int> TopLeftCorner()
        {
            for (int x = 0; x < Board.GetLength(0); x++)
            {
                for (int y = 0; y < Board.GetLength(1); y++)
                {
                    if (Board[x, y] != BoardCell.None)
                        return Tuple.Create(x, y);
                }
            }
            throw new Exception("Not found");
        }

        private Tuple<int, int> BottomRightCorner()
        {
            for (int x = Board.GetLength(0) - 1; x >= 0; x--)
            {
                for (int y = Board.GetLength(1) - 1; y >= 0; y--)
                {
                    if (Board[x, y] != BoardCell.None)
                        return Tuple.Create(x, y);
                }
            }
            throw new Exception("Not found");
        } 
    }
}