using System.IO;
using System.Linq;
using Gomoku2;
using Gomoku2.CellObjects;
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
            var cells = GetNextCells("DoubleThreatDefenceMoves.txt", true);
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(2, myCells.Count);
            Assert.AreEqual(new Cell(5, 4), myCells[0]);
            Assert.AreEqual(new Cell(5, 3), myCells[1]);
        }

        [TestMethod]
        public void BrokenTwoAndBrokenTwo()
        {
            var cells = GetNextCells("BrokenTwoAndBrokenTwo.txt");
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(1, myCells.Count);
            Assert.AreEqual(new Cell(6, 8), myCells[0]);
        }

        [TestMethod]
        public void BrokenTwoAndBrokenTwo2()
        {
            var cells = GetNextCells("BrokenTwoAndBrokenTwo2.txt");
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(1, myCells.Count);
            Assert.AreEqual(new Cell(6, 8), myCells[0]);
        }

        [TestMethod]
        public void BrokenTwoAndLongBrokenTwo()
        {
            var cells = GetNextCells("BrokenTwoAndLongBrokenTwo.txt");
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(1, myCells.Count);
            Assert.AreEqual(new Cell(6, 8), myCells[0]);
        }

        [TestMethod]
        public void LongBrokenTwoIntoDoubleThreat()
        {
            var cells = GetNextCells("LongBrokenTwoIntoDoubleThreat.txt", true);
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(1, myCells.Count);
            Assert.AreEqual(new Cell(6, 9), myCells[0]);
        }

        [TestMethod]
        public void BlockedFour()
        {
            var cells = GetNextCells("BlockedFour.txt");
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(1, myCells.Count);
            Assert.AreEqual(new Cell(6, 9), myCells[0]);
        }

        [TestMethod]
        public void StraightFour()
        {
            var cells = GetNextCells("StraightFour.txt");
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(2, myCells.Count);
            Assert.AreEqual(new Cell(6, 9), myCells[0]);
            Assert.AreEqual(new Cell(6, 4), myCells[1]);
        }

        [TestMethod]
        public void BrokenFour()
        {
            var cells = GetNextCells("BrokenFour.txt");
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(1, myCells.Count);
            Assert.AreEqual(new Cell(6, 7), myCells[0]);
        }

        [TestMethod]
        public void BrokenThreeAttack()
        {
            var cells = GetNextCells("BrokenThree.txt");
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(1, myCells.Count);
            Assert.AreEqual(new Cell(6, 7), myCells[0]);
        }

        [TestMethod]
        public void BrokenThreeDeffence()
        {
            var cells = GetNextCells("BrokenThree.txt", true);
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(3, myCells.Count);
            Assert.AreEqual(new Cell(6, 7), myCells[0]);
            Assert.AreEqual(new Cell(6, 10), myCells[1]);
            Assert.AreEqual(new Cell(6, 5), myCells[2]);
        }

        [TestMethod]
        public void ThreeInRow()
        {
            var cells = GetNextCells("ThreeInRow.txt");
            var myCells = cells.MyNextCells.ToList();
            Assert.AreEqual(2, myCells.Count);
            Assert.AreEqual(new Cell(6, 9), myCells[0]);
            Assert.AreEqual(new Cell(6, 5), myCells[1]);
        }

        [TestMethod]
        public void TwoInRow()
        {
            var cells = GetNextCells("TwoInRow.txt", true);
            var myCells = cells.MyNextCells.ToList();
            //maybe even more since it is starting position. 9 is all surronding
            Assert.AreEqual(9 + 2, myCells.Count);
        }

        private static NextCells GetNextCells(string fileName, bool next = false)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName));
            var game = new Game(board);
            var owner = next ? board.WhoMovesNext() : board.WhoMovedLast();
            var state = game.GetBoardState(owner, Game.DefaultDepth, Game.DefaultWidth);
            return state.GetNextCells();
        }
    }
}