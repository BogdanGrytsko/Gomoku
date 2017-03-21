using System.Collections.Generic;
using System.IO;
using Gomoku2;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;
using Gomoku2.StateCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class LineChangeTest
    {
        private const string folder = "LineChange";

        [TestMethod]
        public void BrokenFourIntoFive()
        {
            var lines = GetLines("BrokenFourIntoFive.txt", new Cell(6, 5));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.FiveInRow, lines[0].LineType);
        }

        private static List<Line> GetLines(string fileName, Cell cell)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName)).Board;
            var game = new Game(board);
            var lines = game.GetLines(board.WhoMovedLast());
            LineFactory.AddCellToLines(cell, new BoardStateBase(lines, new List<Line>(), board.WhoMovedLast(), board));
            return lines;
        }
    }
}