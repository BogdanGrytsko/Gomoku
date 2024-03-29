﻿using System.IO;
using Gomoku2.CellObjects;

namespace Gomoku2
{
    public class BoardExportImport
    {
        public static BoardCell[,] Import(string fileName)
        {
            var board = new BoardCell[15, 15];
            using (var sw = new StreamReader(fileName))
            {
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    var line = sw.ReadLine();
                    for (int y = 0; y < board.GetLength(1); y++)
                    {
                        if (line[y].ToString() == " ") board[x, y] = BoardCell.None;
                        if (line[y].ToString() == "X") board[x, y] = BoardCell.First;
                        if (line[y].ToString() == "O") board[x, y] = BoardCell.Second;
                    }
                }
            }
            return board;
        }

        public static void Export(BoardCell[,] board, string fileName)
        {
            using (var sw = new StreamWriter(fileName))
            {
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    for (int y = 0; y < board.GetLength(1); y++)
                    {
                        sw.Write(board[x, y].GetCellText());
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}