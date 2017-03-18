using System;
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
            var lines = TestEstimation("TwoInRow.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
        }

        [TestMethod]
        public void BlockedTwoInRow()
        {
            var lines = TestEstimation("BlockedTwoInRow.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
        }

        [TestMethod]
        public void BlockedTwoInRow2()
        {
            var lines = TestEstimation("BlockedTwoInRow2.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
        }

        [TestMethod]
        public void DeadTwo()
        {
            var lines = TestEstimation("DeadTwo.txt");
            Assert.AreEqual(LineType.DeadTwo, lines[0].LineType);
        }

        [TestMethod]
        public void DeadTwo2()
        {
            var lines = TestEstimation("DeadTwo2.txt");
            Assert.AreEqual(LineType.DeadTwo, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenFourInRow()
        {
            var lines = TestEstimation("BrokenFourInRow.txt");
            Assert.AreEqual(LineType.BrokenFourInRow, lines[0].LineType);
        }

        [TestMethod]
        public void LongBrokenThree()
        {
            var lines = TestEstimation("LongBrokenThree.txt");
            Assert.AreEqual(LineType.LongBrokenThree, lines[0].LineType);
        }

        [TestMethod]
        public void LongBrokenThreeInRowAndBrokenThreeInRow()
        {
            var lines = TestEstimation("LongBrokenThreeInRowAndBrokenThreeInRow.txt");
            Assert.AreEqual(LineType.BrokenThree, lines[0].LineType);
            Assert.AreEqual(LineType.LongBrokenThree, lines[1].LineType);
        }

        private static List<Line> TestEstimation(string fileName)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName)).Board;
            var game = new Game(board);
            return game.GetLines(board.WhoMovedLast());
        }
    }
}