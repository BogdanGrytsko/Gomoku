using Gomoku2.LineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class LineDeterminationTest : TestBase
    {
        protected override string Folder => "LineDetermination";

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
        public void BrokenFour()
        {
            var lines = GetLines("BrokenFour.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenFour, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenFour2()
        {
            var lines = GetLines("BrokenFour2.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenFour, lines[0].LineType);
        }

        [TestMethod]
        public void LongBrokenThree()
        {
            var lines = GetLines("LongBrokenThree.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.LongBrokenThree, lines[0].LineType);
            Assert.IsNotNull(lines[0].Middle1);
            Assert.IsNotNull(lines[0].Middle2);
        }

        [TestMethod]
        public void LongBrokenThreeAndBrokenThree()
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

        [TestMethod]
        public void BrokenThree()
        {
            var lines = GetLines("BrokenThree.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenThree, lines[0].LineType);
            Assert.IsNotNull(lines[0].Middle1);
        }

        [TestMethod]
        public void BrokenThree2()
        {
            var lines = GetLines("BrokenThree2.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenThree, lines[0].LineType);
            Assert.IsNotNull(lines[0].Middle1);
        }

        [TestMethod]
        public void LongBrokenThree2()
        {
            var lines = GetLines("LongBrokenThree2.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.LongBrokenThree, lines[0].LineType);
            Assert.IsNotNull(lines[0].Middle1);
            Assert.IsNotNull(lines[0].Middle2);
        }

        [TestMethod]
        public void LongBrokenThree3()
        {
            var lines = GetLines("LongBrokenThree2.txt");
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.LongBrokenThree, lines[0].LineType);
            Assert.IsNotNull(lines[0].Middle1);
            Assert.IsNotNull(lines[0].Middle2);
        }

        [TestMethod]
        public void DoubleLongBrokenTwo()
        {
            var lines = GetLines("DoubleLongBrokenTwo.txt");
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.LongBrokenTwo, lines[0].LineType);
            Assert.AreEqual(LineType.LongBrokenTwo, lines[1].LineType);
        }

        [TestMethod]
        public void LongBrokenTwoAndBrokenTwo()
        {
            var lines = GetLines("LongBrokenTwoAndBrokenTwo.txt");
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
            Assert.AreEqual(LineType.LongBrokenTwo, lines[1].LineType);
        }

        [TestMethod]
        public void BrokenTwoAndLongBrokenTwo()
        {
            var lines = GetLines("BrokenTwoAndLongBrokenTwo.txt");
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
            Assert.AreEqual(LineType.LongBrokenTwo, lines[1].LineType);
        }

        [TestMethod]
        public void ThreeInRowAndLongBrokenThree()
        {
            var lines = GetLines("ThreeInRowAndLongBrokenThree.txt");
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.ThreeInRow, lines[0].LineType);
            Assert.AreEqual(LineType.LongBrokenThree, lines[1].LineType);
        }
    }
}