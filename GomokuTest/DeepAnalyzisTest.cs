using Gomoku2.CellObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class DeepAnalyzisTest : TestBase
    {
        protected override string Folder => "DeepAnalyzis";

        [TestMethod]
        public void Board05SecondLost()
        {
            DoTestWin("Board05SecondLost.txt", new Cell(9, 8), new Cell(10, 8));
        }
    }
}