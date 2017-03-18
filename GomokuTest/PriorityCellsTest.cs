using System.IO;
using System.Linq;
using Gomoku2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class PriorityCellsTest
    {
        private const string folder = "PriorityCells";

        [TestMethod]
        public void DoubleThreatDefenceMoves()
        {
            var cells = GetNextCells("DoubleThreatDefenceMoves.txt");
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(new Cell(5, 4), myCells[0]);
            Assert.AreEqual(new Cell(5, 3), myCells[1]);
        }

        private static NextCells GetNextCells(string fileName)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName)).Board;
            var game = new Game(board);
            var state = game.GetBoardState(board.WhoMovedLast(), Game.DefaultDepth, Game.DefaultWidth);
            return state.GetNextCells();
        }
    }
}