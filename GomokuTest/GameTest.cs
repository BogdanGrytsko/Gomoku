using System;
using System.Collections.Generic;
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
        public void Board14()
        {
            DoTest("board14.txt", new Cell(8, 11));
        }

        [TestMethod]
        public void Board16WinNotFound()
        {
            DoTest("board16WinNotFound.txt", new Cell(7, 10));
        }

        private static void DoTest(string boardName, params Cell[] correctMoves)
        {
            var board = BoardExportImport.Import(Path.Combine("BoardStates", boardName)).Board;
            var game = new Game(board);
            var move = game.DoMove(board.WhoMovesNext());
            var estimatedBoard = game.EstimatedBoard;
            Assert.IsTrue(correctMoves.Any(cm => cm == move));
            Assert.IsTrue(Game.StraightFour(estimatedBoard.Estimate));
        }
    }
}
