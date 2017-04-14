using Gomoku2.CellObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class BoardStateTest : TestBase
    {
        protected override string Folder => "BoardState";

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
        public void Board11FirstWon()
        {
            DoTestWin("Board11FirstWon.txt", new Cell(5, 8));
        }

        [TestMethod]
        public void Board06WrongDefence()
        {
            TestMove("Board06WrongDefence.txt", new Cell(5, 9));
        }

        [TestMethod]
        public void Board09Unknown()
        {
            DoTestWin("Board09Unknown.txt", new Cell(6, 9));
        }

        [TestMethod]
        public void Board13FirstDontDefend()
        {
            TestMove("Board13FirstDontDefend.txt", new Cell(5, 5));
        }

        [TestMethod]
        public void LongBrokenTwoNotModifiedCorrectly()
        {
            TestMove("LongBrokenTwoNotModifiedCorrectly.txt", new Cell(7, 5), new Cell(10, 5));
        }

        [TestMethod]
        public void FirstWrongDefenceMove()
        {
            DoTestWin("FirstWrongDefenceMove.txt", new Cell(4, 7));
        }
    }
}
