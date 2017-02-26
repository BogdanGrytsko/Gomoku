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
            DoTestStraightFour("SecondHasWinningMove.txt", new Cell(4, 10), new Cell(9, 10) );
        }

        [TestMethod]
        public void StraightFiveIsNotCountered()
        {
            DoTestStraightFour("StraightFiveIsNotCountered.txt", new Cell(8, 11));
        }

        [TestMethod]
        public void FourInLineNotPriorityForDef()
        {
            DoTestStraightFour("FourInLineNotPriorityForDef.txt", new Cell(5, 9));
        }

        [TestMethod]
        public void Board14HasWinningMove()
        {
            //todo analyze and refactor so total # of moves is lesser.
            DoTestStraightFour("Board14HasWinningMove.txt", new Cell(8, 10), new Cell(8, 11));
        }

        [TestMethod]
        public void Board16WinNotFound()
        {
            DoTestStraightFour("Board16WinNotFound.txt", new Cell(7, 10));
        }

        [TestMethod]
        public void Board07IsLost()
        {
            TestMove("Board07IsLost.txt", new Cell(8, 6));
        }

        [TestMethod]
        public void BlockedThreeIntoDeadFour()
        {
            DoTestStraightFour("BlockedThreeIntoDeadFour.txt", new Cell(7, 6));
        }

        [TestMethod]
        public void LongRunningComplexPosition()
        {
            TestMove("LongRunningComplexPosition.txt", new Cell(9, 7));
        }

        [TestMethod]
        public void Board34WrongDefensiveMove()
        {
            TestMove("Board34WrongDefensiveMove.txt", new Cell(11, 6), new Cell(8, 9));
        }

        [TestMethod]
        public void ThreeInRowDefensiveMove()
        {
            TestMove("ThreeInRowDefensiveMove.txt", new Cell(7, 5), new Cell(7, 9));
        }

        [TestMethod]
        public void LongBrokenThree()
        {
            TestMove("LongBrokenThree.txt", new Cell(7, 8), new Cell(5, 6));
        }

        [TestMethod]
        public void Board11FirstLost()
        {
            TestMove("Board11FirstLost.txt", new Cell(8, 10));
        }

        private static void DoTestStraightFour(string boardName, params Cell[] correctMoves)
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
