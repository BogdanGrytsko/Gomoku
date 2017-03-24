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
    public class OpponentMoveLineChange
    {
        private const string folder = "OpponentMoveLineChange";

        [TestMethod]
        public void StraightFourIntoBlockedFour()
        {
            var lines = GetLines("StraightFourIntoBlockedFour.txt", new Cell(4, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.FourInRow, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenFourIntoBlockedThreeAndSingle()
        {
            var lines = GetLines("BrokenFourIntoBlockedThreeAndSingle.txt", new Cell(5, 6));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlokedThree, lines[0].LineType);
            Assert.AreEqual(LineType.SingleMark, lines[1].LineType);
        }

        [TestMethod]
        public void BrokenFourIntoDoubleBlockedTwo()
        {
            var lines = GetLines("BrokenFourIntoDoubleBlockedTwo.txt", new Cell(6, 6));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
        }

        private static List<Line> GetLines(string fileName, Cell cell)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName)).Board;
            var game = new Game(board);
            var owner = board.WhoMovedLast();
            var lines = game.GetLines(owner);
            board[cell.X, cell.Y] = owner.Opponent();
            LineFactory.AddCellToLines(cell, new BoardStateBase(new List<Line>(), lines, owner.Opponent(), board));
            return lines;
        }
    }
}