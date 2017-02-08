using System.IO;
using System.Linq;
using Gomoku2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void SecondHasWinningMove()
        {
            DoTest("SecondHasWinningMove.txt", new Cell(4, 10), new Cell(9, 10) );
        }

        [TestMethod]
        public void StraightFiveIsNotCountered()
        {
            DoTest("StraightFiveIsNotCountered.txt", new Cell(8, 11));
        }

        [TestMethod]
        public void FourInLineNotPriorityForDef()
        {
            DoTest("FourInLineNotPriorityForDef.txt", new Cell(5, 9));
        }

        [TestMethod]
        public void Board14HasWinningMove()
        {
            DoTest("Board14HasWinningMove.txt", new Cell(8, 10), new Cell(8, 11));
        }

        [TestMethod]
        public void Board16WinNotFound()
        {
            DoTest("Board16WinNotFound.txt", new Cell(7, 10));
        }

        [TestMethod]
        public void Board07IsLost()
        {
            TestMove("Board07IsLost.txt", new Cell(8, 6));
        }

        [TestMethod]
        public void BlockedThreeIntoDeadFour()
        {
            DoTest("BlockedThreeIntoDeadFour.txt", new Cell(7, 6));
        }

        [TestMethod]
        public void ZeroDepthReturnsWrongEstimate()
        {
            TestMove("ComplexPosition2.txt", 0, new Cell(9, 7), new Cell(5, 11));
        }

        [TestMethod]
        public void BrokenFourEstimatedTwice()
        {
            var board = BoardExportImport.Import(Path.Combine("BoardStates", "BrokenFourEstimatedTwice.txt")).Board;
            var game = new Game(board);
            var linesOwner = board.WhoMovesNext().Opponent();
            var lines = game.GetLines(linesOwner);
            var estimate = game.SumLines(lines, linesOwner);
            Assert.IsTrue(estimate < (int) LineType.DoubleThreat);
        }

        private static void DoTest(string boardName, params Cell[] correctMoves)
        {
            var game = TestMove(boardName, correctMoves);
            var estimatedBoard = game.EstimatedBoard;
            Assert.IsTrue(Game.StraightFour(estimatedBoard.Estimate));
        }

        private static Game TestMove(string boardName, int depth, params Cell[] correctMoves)
        {
            var board = BoardExportImport.Import(Path.Combine("BoardStates", boardName)).Board;
            var game = new Game(board);
            var move = game.DoMove(board.WhoMovesNext(), depth);
            Assert.IsTrue(correctMoves.Any(cm => cm == move));
            return game;
        }

        private static Game TestMove(string boardName, params Cell[] correctMoves)
        {
            return TestMove(boardName, Game.DefaultDepth, correctMoves);
        }
    }
}
