using System;
using System.IO;
using Gomoku2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class EstimateTest
    {
        [TestMethod]
        public void BrokenFourEstimatedTwice()
        {
            TestEstimation("BrokenFourEstimatedTwice.txt", est => est < (int)LineType.DoubleThreat);
        }

        [TestMethod]
        public void DoubleThreatEstimatedAsLost()
        {
            //todo fix
            TestEstimation("DoubleThreatEstimatedAsLost.txt", est => est < 0 && est < (int)LineType.DoubleThreat);
        }

        [TestMethod]
        public void StraightFourEstimatedAsLost()
        {
            TestEstimation("StraightFourEstimatedAsLost.txt", est => est == -(int)LineType.FiveInRow);
        }

        [TestMethod]
        public void BlockedFourEstimatedAsLost()
        {
            TestEstimation("BlockedFourEstimatedAsLost.txt", est => est == -(int)LineType.FiveInRow);
        }

        [TestMethod]
        public void ThreeInRowEstimatedAsLost()
        {
            TestEstimation("ThreeInRowEstimatedAsLost.txt", est => est == -(int)LineType.FiveInRow);
        }

        [TestMethod]
        public void FourInRowEstimatedAsWin()
        {
            TestEstimation("FourInRowEstimatedAsWin.txt", est => est > (int)LineType.StraightFour / 2);
        }

        private static void TestEstimation(string fileName, Predicate<int> predicate)
        {
            var board = BoardExportImport.Import(Path.Combine("Estimations", fileName)).Board;
            var game = new Game(board);
            var opponent = board.WhoMovesNext();
            var linesOwner = opponent.Opponent();
            var estimate = GetEstimate(game, linesOwner);
            Assert.IsTrue(predicate(estimate));
        }

        private static int GetEstimate(Game game, BoardCell myCellType)
        {
            var oppCellType = myCellType.Opponent();
            var myLines = game.GetLines(myCellType);
            var oppLines = game.GetLines(oppCellType);
            return game.Estimate(myLines, myCellType, oppLines, oppCellType);
        }
    }
}