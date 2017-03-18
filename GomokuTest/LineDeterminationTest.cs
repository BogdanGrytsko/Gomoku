using System.Collections.Generic;
using System.IO;
using Gomoku2;
using Gomoku2.LineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class LineDeterminationTest
    {
        private const string folder = "LineDetermination";

        [TestMethod]
        public void TwoInRow()
        {
            var lines = GetLines("TwoInRow.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
        }

        [TestMethod]
        public void BlockedTwoInRow()
        {
            var lines = GetLines("BlockedTwoInRow.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
        }

        [TestMethod]
        public void BlockedTwoInRow2()
        {
            var lines = GetLines("BlockedTwoInRow2.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
        }

        [TestMethod]
        public void DeadTwo()
        {
            var lines = GetLines("DeadTwo.txt");
            Assert.AreEqual(LineType.DeadTwo, lines[0].LineType);
        }

        [TestMethod]
        public void DeadTwo2()
        {
            var lines = GetLines("DeadTwo2.txt");
            Assert.AreEqual(LineType.DeadTwo, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenFourInRow()
        {
            var lines = GetLines("BrokenFourInRow.txt");
            Assert.AreEqual(LineType.BrokenFourInRow, lines[0].LineType);
        }

        [TestMethod]
        public void LongBrokenThree()
        {
            var lines = GetLines("LongBrokenThree.txt");
            Assert.AreEqual(LineType.LongBrokenThree, lines[0].LineType);
        }

        [TestMethod]
        public void LongBrokenThreeInRowAndBrokenThreeInRow()
        {
            var lines = GetLines("LongBrokenThreeInRowAndBrokenThreeInRow.txt");
            Assert.AreEqual(LineType.BrokenThree, lines[0].LineType);
            Assert.AreEqual(LineType.LongBrokenThree, lines[1].LineType);
        }

        [TestMethod]
        public void BrokenTwoAndBrokenTwo2()
        {
            var lines = GetLines("BrokenTwoAndBrokenTwo2.txt");
            Assert.AreEqual(4, lines.Count);
            foreach (var line in lines)
            {
                Assert.AreEqual(LineType.TwoInRow, line.LineType);
            }
        }

        [TestMethod]
        public void LongBrokenTwo()
        {
            var lines = GetLines("LongBrokenTwo.txt");
            Assert.AreEqual(LineType.LongBrokenTwo, lines[0].LineType);
        }

        [TestMethod]
        public void BlockedTwoAnd2TwoInRow()
        {
            var lines = GetLines("BlockedTwoAnd2TwoInRow.txt");
            Assert.AreEqual(3, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
            Assert.AreEqual(LineType.TwoInRow, lines[1].LineType);
            Assert.AreEqual(LineType.BlockedTwo, lines[2].LineType);
        }

        private static List<Line> GetLines(string fileName)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName)).Board;
            var game = new Game(board);
            return game.GetLines(board.WhoMovedLast());
        }
    }
}