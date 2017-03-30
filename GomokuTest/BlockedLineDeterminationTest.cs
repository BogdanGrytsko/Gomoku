using Gomoku2.LineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class BlockedLineDeterminationTest : TestBase
    {
        protected override string Folder => "BlockedLineDetermination";

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
        public void BlockedThreeAndBlockedThree()
        {
            var lines = GetLines("BlockedThreeAndBlockedThree.txt");
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlokedThree, lines[0].LineType);
            Assert.AreEqual(LineType.BlokedThree, lines[1].LineType);
        }

        [TestMethod]
        public void BlockedLongBrokenThree()
        {
            var lines = GetLines("BlockedLongBrokenThree.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BlokedThree, lines[0].LineType);
        }

        [TestMethod]
        public void BlockedThreeAndLongBrokenThree()
        {
            var lines = GetLines("BlockedThreeAndLongBrokenThree.txt");
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.LongBrokenThree, lines[0].LineType);
            Assert.AreEqual(LineType.BlokedThree, lines[1].LineType);
        }

        [TestMethod]
        public void BlockedLongBrokenTwoAndBrokenTwo()
        {
            var lines = GetLines("BlockedLongBrokenTwoAndBrokenTwo.txt");
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
            Assert.AreEqual(LineType.BlockedTwo, lines[1].LineType);
        }
    }
}