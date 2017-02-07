using System;
using System.Collections.Generic;
using Gomoku2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class SecondPlayerTest
    {
        [TestMethod]
        public void SecondHasWinningMove()
        {
            var board = BoardExportImport.Import("BoardStates\\SecondHasWinningMove.txt").Board;
            var game = new Game(board);
            var move = game.DoMove(board.WhoMovesNext());
            var possibleCells = new List<Cell> {new Cell(4, 10), new Cell(9,10)};

            Assert.IsTrue(possibleCells.Contains(move));
        }
    }
}
