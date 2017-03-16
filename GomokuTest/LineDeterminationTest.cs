using System;
using System.Collections.Generic;
using System.IO;
using Gomoku2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class LineDeterminationTest
    {
        private const string folder = "LineDetermination";

        [TestMethod]
        public void TwoInRow()
        {
            var lines = TestEstimation("TwoInRow.txt");

            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
        }

        private static List<Line> TestEstimation(string fileName)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName)).Board;
            var game = new Game(board);
            var opponent = board.WhoMovesNext();
            var linesOwner = opponent.Opponent();
            return game.GetLines(linesOwner);
        }
    }
}