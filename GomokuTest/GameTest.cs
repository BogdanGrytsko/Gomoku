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
            var move = game.DoMove(board.WhoMovesNext(), depth, 20);
            Assert.IsTrue(correctMoves.Any(cm => cm == move));
            return game;
        }

        private static Game TestMove(string boardName, params Cell[] correctMoves)
        {
            return TestMove(boardName, Game.DefaultDepth, correctMoves);
        }
    }
}
