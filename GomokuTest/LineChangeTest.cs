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
            var lines = GetLines("BrokenFourIntoFive.txt", new Cell(5, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.FiveInRow, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenFourIntoFive2()
        {
            var lines = GetLines("BrokenFourIntoFive2.txt", new Cell(6, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.FiveInRow, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenTwoIntoThree()
        {
            var lines = GetLines("BrokenTwoIntoThree.txt", new Cell(6, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.ThreeInRow, lines[0].LineType);
        }

        [TestMethod]
        public void TwoIntoBrokenThree()
        {
            var lines = GetLines("TwoIntoBrokenThree.txt", new Cell(6, 8));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenThree, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenThreeIntoBrokenFour()
        {
            var lines = GetLines("BrokenThreeIntoBrokenFour.txt", new Cell(6, 4));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenFour, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenThreeIntoStraightFour()
        {
            var lines = GetLines("BrokenThreeIntoStraightFour.txt", new Cell(6, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.StraightFour, lines[0].LineType);
        }

        private static List<Line> GetLines(string fileName, Cell cell)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName)).Board;
            var game = new Game(board);
            var owner = board.WhoMovedLast();
            var lines = game.GetLines(owner);
            board[cell.X, cell.Y] = owner;
            LineFactory.AddCellToLines(cell, new BoardStateBase(lines, new List<Line>(), owner, board));
            return lines;
        }
    }
}