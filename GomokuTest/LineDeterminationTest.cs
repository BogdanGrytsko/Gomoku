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
        public void ThreeInRow()
        {
            var lines = GetLines("ThreeInRow.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.ThreeInRow, lines[0].LineType);
        }

        [TestMethod]
        public void StraightFour()
        {
            var lines = GetLines("StraightFour.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.StraightFour, lines[0].LineType);
        }

        [TestMethod]
        public void TripleTwoInRow()
        {
            var lines = GetLines("TripleTwoInRow.txt");
            Assert.AreEqual(3, lines.Count);
            foreach (var line in lines)
            {
                Assert.AreEqual(LineType.TwoInRow, line.LineType);
            }
        }

        [TestMethod]
        public void BrokenTwoInRow()
        {
            var lines = GetLines("BrokenTwoInRow.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenTwoInRow2()
        {
            var lines = GetLines("BrokenTwoInRow2.txt");
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
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.DeadTwo, lines[0].LineType);
        }

        [TestMethod]
        public void DeadTwo2()
        {
            var lines = GetLines("DeadTwo2.txt");
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.DeadTwo, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenFourInRow()
        {
            var lines = GetLines("BrokenFourInRow.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenFourInRow, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenFourInRow2()
        {
            var lines = GetLines("BrokenFourInRow2.txt");
            //todo fix - there are 2 lines like that. leads to bad estimation
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenFourInRow, lines[0].LineType);
        }

        [TestMethod]
        public void LongBrokenThree()
        {
            var lines = GetLines("LongBrokenThree.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.LongBrokenThree, lines[0].LineType);
        }

        [TestMethod]
        public void LongBrokenThreeInRowAndBrokenThreeInRow()
        {
            var lines = GetLines("LongBrokenThreeAndBrokenThree.txt");
            Assert.AreEqual(2, lines.Count);
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
            Assert.AreEqual(1, lines.Count);
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

        [TestMethod]
        public void BrokenTwoAndBlockedThree()
        {
            var lines = GetLines("BrokenTwoAndBlockedThree.txt");
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
            Assert.AreEqual(LineType.BlokedThree, lines[1].LineType);
        }

        [TestMethod]
        public void TripleBrokenTwo()
        {
            var lines = GetLines("TripleBrokenTwo.txt");
            Assert.AreEqual(3, lines.Count);
            foreach (var line in lines)
            {
                Assert.AreEqual(LineType.TwoInRow, line.LineType);
            }
        }

        [TestMethod]
        public void TripleLongBrokenTwo()
        {
            var lines = GetLines("TripleLongBrokenTwo.txt");
            Assert.AreEqual(3, lines.Count);
            foreach (var line in lines)
            {
                Assert.AreEqual(LineType.LongBrokenTwo, line.LineType);
            }
        }

        [TestMethod]
        public void DoubleBrokenTwo()
        {
            var lines = GetLines("DoubleBrokenTwo.txt");
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
            Assert.AreEqual(LineType.TwoInRow, lines[1].LineType);
        }

        private static List<Line> GetLines(string fileName)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName)).Board;
            var game = new Game(board);
            return game.GetLines(board.WhoMovedLast());
        }
    }
}