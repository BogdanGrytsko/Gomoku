using Gomoku2.CellObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class PlaysSecondTest : TestBase
    {
        protected override string Folder => "PlaysSecond";

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
    }
}