using Gomoku2;
using Gomoku2.CellObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class PlaysSecondTest
    {
        private const string folder = "PlaysSecond";

        [TestMethod]
        public void Board07Unknown()
        {
            TestMove("Board07Unknown.txt", new Cell(8, 6));
        }

        [TestMethod]
        public void Board10IsWon()
        {
            TestMove("Board10(6,9)IsUnknown.txt", new Cell(8, 11));
        }

        [TestMethod]
        public void Board11IsWon()
        {
            DoTestWin("Board11IsWon.txt", new Cell(6, 6), new Cell(6, 5));
        }

        private static Game TestMove(string boardName, int depth, params Cell[] correctMoves)
        {
            return GameTest.TestMove(folder, boardName, depth, correctMoves);
        }

        public static Game TestMove(string boardName, params Cell[] correctMoves)
        {
            return TestMove(boardName, Game.DefaultDepth, correctMoves);
        }

        private static void DoTestWin(string boardName, params Cell[] correctMoves)
        {
            var game = TestMove(boardName, correctMoves);
            Assert.IsTrue(GameTest.Win(game.LastEstimate));
        }
    }
}