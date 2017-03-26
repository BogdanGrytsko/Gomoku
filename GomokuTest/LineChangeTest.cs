﻿using System.Collections.Generic;
using System.IO;
using Gomoku2;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;
using Gomoku2.StateCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class LineChangeTest
    {
        private const string folder = "LineChange";

        [TestMethod]
        public void BrokenFourIntoFive()
        {
            var lines = GetChangedLines("BrokenFourIntoFive.txt", new Cell(5, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.FiveInRow, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenFourIntoFive2()
        {
            var lines = GetChangedLines("BrokenFourIntoFive2.txt", new Cell(6, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.FiveInRow, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenTwoIntoThree()
        {
            var lines = GetChangedLines("BrokenTwoIntoThree.txt", new Cell(6, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.ThreeInRow, lines[0].LineType);
        }

        [TestMethod]
        public void TwoIntoBrokenThree()
        {
            var lines = GetChangedLines("TwoIntoBrokenThree.txt", new Cell(6, 8));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenThree, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenThreeIntoBrokenFour()
        {
            var lines = GetChangedLines("BrokenThreeIntoBrokenFour.txt", new Cell(6, 4));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenFour, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenThreeIntoStraightFour()
        {
            var lines = GetChangedLines("BrokenThreeIntoStraightFour.txt", new Cell(6, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.StraightFour, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenTwoIntoBrokenThree()
        {
            var lines = GetChangedLines("BrokenTwoIntoBrokenThree.txt", new Cell(6, 8));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenThree, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenTwoIntoBrokenThree2()
        {
            var lines = GetChangedLines("BrokenTwoIntoLongBrokenThree.txt", new Cell(6, 9));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.LongBrokenThree, lines[0].LineType);
        }

        [TestMethod]
        public void LongBrokenTwoIntoBrokenThree()
        {
            var lines = GetChangedLines("LongBrokenTwoIntoBrokenThree.txt", new Cell(6, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.BrokenThree, lines[0].LineType);
        }

        [TestMethod]
        public void LongBrokenTwoIntoLongBrokenThree()
        {
            var lines = GetChangedLines("LongBrokenTwoIntoLongBrokenThree.txt", new Cell(6, 9));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.LongBrokenThree, lines[0].LineType);
        }

        [TestMethod]
        public void LongBrokenThreeInto2Lines()
        {
            var lines = GetChangedLines("LongBrokenThreeInto2Lines.txt", new Cell(6, 10));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.LongBrokenThree, lines[0].LineType);
            Assert.AreEqual(LineType.TwoInRow, lines[1].LineType);
        }

        [TestMethod]
        public void LongBrokenTwoNotModifiedCorrectly()
        {
            var lines = GetChangedLines("LongBrokenTwoNotModifiedCorrectly.txt", new Cell(8, 5), true);
            Assert.AreEqual(8, lines.Count);
            Assert.AreEqual(LineType.BrokenThree, lines[0].LineType);
            Assert.AreEqual(LineType.TwoInRow, lines[1].LineType);
        }

        private static List<Line> GetChangedLines(string fileName, Cell cell, bool movesInOrder = false)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName));
            var game = new Game(board);
            var owner = movesInOrder ? board.WhoMovesNext() : board.WhoMovedLast();
            var lines = game.GetLines(owner);
            board[cell.X, cell.Y] = owner;
            LineFactory.AddCellToLines(cell, new BoardStateBase(lines, new List<Line>(), owner, board));
            return lines;
        }
    }
}