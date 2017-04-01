using System;
using System.IO;
using System.Linq;
using Gomoku2;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class BoardStateTest
    {
        private const string folder = "BoardState";

        [TestMethod]
        public void SecondHasWinningMove()
        {
            DoTestWin("SecondHasWinningMove.txt", new Cell(4, 10), new Cell(9, 10) );
        }

        [TestMethod]
        public void StraightFiveIsNotCountered()
        {
            DoTestWin("StraightFiveIsNotCountered.txt", new Cell(8, 11));
        }

        [TestMethod]
        public void FourInLineNotPriorityForDef()
        {
            DoTestWin("FourInLineNotPriorityForDef.txt", new Cell(5, 9));
        }

        [TestMethod]
        public void Board14HasWinningMove()
        {
            DoTestWin("Board14HasWinningMove.txt", new Cell(8, 10), new Cell(8, 11));
        }

        [TestMethod]
        public void Board16WinNotFound()
        {
            DoTestWin("Board16WinNotFound.txt", new Cell(7, 10));
        }

        [TestMethod]
        public void BlockedThreeIntoDeadFour()
        {
            DoTestWin("BlockedThreeIntoDeadFour.txt", new Cell(7, 6), new Cell(3, 9));
        }

        [TestMethod]
        public void LongRunningComplexPosition()
        {
            TestMove("LongRunningComplexPosition.txt", new Cell(10, 6));
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
            DoTestWin("Board11FirstLost.txt", new Cell(8, 10));
        }

        [TestMethod]
        public void Board06WrongDefence()
        {
            //second has lost anyway
            DoTestWin("Board06WrongDefence.txt", 5, new Cell(5, 9), new Cell(9, 5));
        }

        [TestMethod]
        public void Board09Unknown()
        {
            TestMove("Board09Unknown.txt", new Cell(5, 7), new Cell(7, 9));
        }

        [TestMethod]
        public void Board05CorrectMove()
        {
            TestMove("Board05CorrectMove.txt", new Cell(7, 6), new Cell(6, 8));
        }

        [TestMethod]
        public void Board13FirstDontDefend()
        {
            TestMove("Board13FirstDontDefend.txt", new Cell(6, 9));
        }

        [TestMethod]
        public void LongBrokenTwoNotModifiedCorrectly()
        {
            TestMove("LongBrokenTwoNotModifiedCorrectly.txt", new Cell(7, 4));
        }

        private static void DoTestWin(string boardName, int depth, params Cell[] correctMoves)
        {
            var game = TestMove(boardName, depth, correctMoves);
            Assert.IsTrue(Win(game.LastEstimate));
        }

        private static void DoTestWin(string boardName, params Cell[] correctMoves)
        {
            var game = TestMove(boardName, correctMoves);
            Assert.IsTrue(Win(game.LastEstimate));
        }

        public static bool Win(int estim)
        {
            return Math.Abs(estim) >= (int)LineType.DoubleThreat / 2;
        }

        public static Game TestMove(string boardName, int depth, params Cell[] correctMoves)
        {
            return TestMove(folder, boardName, depth, correctMoves);
        }

        public static Game TestMove(string folderPath, string boardName, int depth, params Cell[] correctMoves)
        {
            var board = BoardExportImport.Import(Path.Combine(folderPath, boardName));
            var game = new Game(board);
            var move = game.DoMove(board.WhoMovesNext(), depth, Game.DefaultWidth);
            Assert.IsTrue(correctMoves.Any(cm => cm == move));
            return game;
        }

        public static Game TestMove(string boardName, params Cell[] correctMoves)
        {
            return TestMove(boardName, Game.DefaultDepth, correctMoves);
        }
    }
}
