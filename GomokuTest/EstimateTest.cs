using System;
using System.IO;
using Gomoku2;
using Gomoku2.LineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class EstimateTest
    {
        private const string folder = "Estimations";

        [TestMethod]
        public void BrokenFourEstimatedTwice()
        {
            var estimate = TestEstimation("BrokenFourEstimatedTwice.txt");
            Assert.IsTrue(estimate < (int)LineType.DoubleThreat);
        }

        [TestMethod]
        public void DoubleThreatEstimatedAsLost()
        {
            var estimate = TestEstimation("DoubleThreatEstimatedAsLost.txt");
            Assert.IsTrue(estimate == -(int)LineType.FiveInRow);
        }

        [TestMethod]
        public void StraightFourEstimatedAsLost()
        {
            var estimate = TestEstimation("StraightFourEstimatedAsLost.txt");
            Assert.IsTrue(estimate == -(int)LineType.FiveInRow);
        }

        [TestMethod]
        public void BlockedFourEstimatedAsLost()
        {
            var estimate = TestEstimation("BlockedFourEstimatedAsLost.txt");
            Assert.IsTrue(estimate == -(int)LineType.FiveInRow);
        }

        [TestMethod]
        public void ThreeInRowEstimatedAsLost()
        {
            var estimate = TestEstimation("ThreeInRowEstimatedAsLost.txt");
            Assert.IsTrue(estimate == -(int)LineType.FiveInRow);
        }

        [TestMethod]
        public void FourInRowEstimatedAsWin()
        {
            var estimate = TestEstimation("FourInRowEstimatedAsWin.txt");
            Assert.IsTrue(estimate > (int)LineType.StraightFour / 2);
        }

        [TestMethod]
        public void BlockedThreeEstimatedAsBrokenThree()
        {
            var estimate = TestEstimation("BlockedThreeEstimatedAsBrokenThree.txt");
            Assert.IsTrue(estimate > 0);
        }

        [TestMethod]
        public void BlockedThreeAndThreeInRowEstimatedAsDoubleThreat()
        {
            var estimate = TestEstimation("BlockedThreeAndThreeInRowEstimatedAsDoubleThreat.txt");
            Assert.IsTrue(estimate < (int)LineType.DoubleThreat);
        }

        [TestMethod]
        public void BlockedThreeHighPriorityCellsWrong()
        {
            var estimate = TestEstimation("BlockedThreeHighPriorityCellsWrong.txt");
            Assert.IsTrue(estimate < (int)LineType.DoubleThreat);
        }

        [TestMethod]
        public void DoubleLongBrokenThree()
        {
            var estimate = TestEstimation("DoubleLongBrokenThree.txt");
            Assert.IsTrue(estimate < (int)LineType.DoubleThreat);
        }

        private static int TestEstimation(string fileName)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName)).Board;
            var game = new Game(board);
            return GetEstimate(game, board.WhoMovedLast());
        }

        private static int GetEstimate(Game game, BoardCell myCellType)
        {
            var oppCellType = myCellType.Opponent();
            var myLines = game.GetLines(myCellType);
            var oppLines = game.GetLines(oppCellType);
            return game.Estimate(myLines, oppLines);
        }
    }
}