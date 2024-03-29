﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gomoku2;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;
using Gomoku2.StateCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    [TestClass]
    public class OpponentMoveLineChange
    {
        private const string folder = "OpponentMoveLineChange";

        [TestMethod]
        public void StraightFourIntoBlockedFour()
        {
            var lines = GetLines("StraightFourIntoBlockedFour.txt", new Cell(4, 6));
            Assert.AreEqual(1, lines.Count);
            Assert.AreEqual(LineType.FourInRow, lines[0].LineType);
        }

        [TestMethod]
        public void BrokenFourIntoBlockedThreeAndSingle()
        {
            var lines = GetLines("BrokenFourIntoBlockedThreeAndSingle.txt", new Cell(5, 6));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlokedThree, lines[0].LineType);
            Assert.IsNull(lines[0].Middle1);
            Assert.AreEqual(LineType.SingleMark, lines[1].LineType);
            Assert.IsNull(lines[1].Middle1);
        }

        [TestMethod]
        public void BrokenFourIntoDoubleBlockedTwo()
        {
            var lines = GetLines("BrokenFourIntoDoubleBlockedTwo.txt", new Cell(6, 6));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
            Assert.IsNull(lines[0].Middle1);
            Assert.IsNull(lines[1].Middle1);
        }

        [TestMethod]
        public void LongBrokenThreeIntoBlockedTwoAndSingle()
        {
            var lines = GetLines("LongBrokenThreeIntoBlockedTwoAndSingle.txt", new Cell(6, 6));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
            Assert.IsNotNull(lines[0].Middle1);
            Assert.IsNull(lines[0].Middle2);
            Assert.AreEqual(LineType.SingleMark, lines[1].LineType);
            Assert.IsNull(lines[1].Middle1);
            Assert.IsNull(lines[1].Middle2);
        }

        [TestMethod]
        public void LongBrokenThreeIntoTwoAndSingle()
        {
            var lines = GetLines("LongBrokenThreeIntoTwoAndSingle.txt", new Cell(6, 6));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
            Assert.IsNull(lines[0].Middle1);
            Assert.IsNull(lines[0].Middle2);
            Assert.AreEqual(LineType.SingleMark, lines[1].LineType);
            Assert.IsNull(lines[1].Middle1);
            Assert.IsNull(lines[1].Middle2);
        }

        [TestMethod]
        public void BrokenThreeIntoBlockedTwoAndSingle()
        {
            var lines = GetLines("BrokenThreeIntoBlockedTwoAndSingle.txt", new Cell(7, 6));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
            Assert.IsNull(lines[0].Middle1);
            Assert.IsNull(lines[0].Middle2);
            Assert.AreEqual(LineType.SingleMark, lines[1].LineType);
            Assert.IsNull(lines[1].Middle1);
            Assert.IsNull(lines[1].Middle2);
        }

        [TestMethod]
        public void LongBrokenThreeIntoBlockedTwoAndSingle2()
        {
            var lines = GetLines("LongBrokenThreeIntoBlockedTwoAndSingle.txt", new Cell(8, 6));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
            Assert.IsNotNull(lines[0].Middle1);
            Assert.IsNull(lines[0].Middle2);
            Assert.AreEqual(LineType.SingleMark, lines[1].LineType);
            Assert.IsNull(lines[1].Middle1);
            Assert.IsNull(lines[1].Middle2);
        }

        [TestMethod]
        public void LongBrokenTwoIntoNoSingleLines()
        {
            var lines = GetLines("LongBrokenTwoIntoNoSingleLines.txt", new Cell(5, 8));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
            Assert.AreEqual(LineType.BlockedTwo, lines[1].LineType);
        }

        [TestMethod]
        public void SingleMarkAfterLongBrokenTwoSplit()
        {
            var lines = GetLines("SingleMarkAfterLongBrokenTwoSplit.txt", new Cell(8, 4));
            var singleMarks = lines.Where(l => l.LineType == LineType.SingleMark).ToList();
            Assert.AreEqual(2, singleMarks.Count);
        }

        [TestMethod]
        public void SplitLongBrokenThree()
        {
            var lines = GetLines("SplitLongBrokenThree.txt", new Cell(5, 8));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlokedThree, lines[0].LineType);
            Assert.AreEqual(LineType.SingleMark, lines[1].LineType);
        }

        [TestMethod]
        public void SplitBrokenThree()
        {
            var lines = GetLines("SplitBrokenThree.txt", new Cell(9, 8));
            Assert.AreEqual(4, lines.Count);
            Assert.AreEqual(LineType.TwoInRow, lines[0].LineType);
            Assert.AreEqual(LineType.TwoInRow, lines[1].LineType);
            Assert.AreEqual(LineType.DeadTwo, lines[2].LineType);
            Assert.AreEqual(LineType.SingleMark, lines[3].LineType);
        }

        [TestMethod]
        public void DoubleBrokenFourIntoBlockedLines()
        {
            var lines = GetLines("DoubleBrokenFourIntoBlockedLines.txt", new Cell(5, 7));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlokedThree, lines[0].LineType);
            Assert.AreEqual(LineType.BlockedTwo, lines[1].LineType);
        }

        [TestMethod]
        public void DoubleBrokenThreeSplit()
        {
            var lines = GetLines("DoubleBrokenThreeSplit.txt", new Cell(5, 7));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
            Assert.AreEqual(LineType.BlockedTwo, lines[1].LineType);
        }

        [TestMethod]
        public void BrokenTwoSplit()
        {
            var lines = GetLines("BrokenTwoSplit.txt", new Cell(6, 9));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlockedTwo, lines[0].LineType);
            Assert.AreEqual(LineType.DeadFour, lines[1].LineType);
        }

        [TestMethod]
        public void BrokenFourAndBrokenThreeSplit()
        {
            var lines = GetLines("BrokenFourAndBrokenThreeSplit.txt", new Cell(5, 8));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlokedThree, lines[0].LineType);
            Assert.AreEqual(LineType.BlockedTwo, lines[1].LineType);
        }

        [TestMethod]
        public void BrokenThreeAndLongBrokenThree()
        {
            var lines = GetLines("BrokenThreeAndLongBrokenThree.txt", new Cell(6, 6));
            Assert.AreEqual(2, lines.Count);
            Assert.AreEqual(LineType.BlokedThree, lines[0].LineType);
            Assert.AreEqual(LineType.SingleMark, lines[1].LineType);
        }

        [TestMethod]
        public void VeryLongBrokenTwoNotAllowed()
        {
            var lines = GetLines("VeryLongBrokenTwoNotAllowed.txt", new Cell(7, 6));
            Assert.AreEqual(3, lines.Count);
            Assert.AreEqual(LineType.SingleMark, lines[0].LineType);
            Assert.AreEqual(LineType.SingleMark, lines[1].LineType);
            Assert.AreEqual(LineType.SingleMark, lines[2].LineType);
        }

        private static List<Line> GetLines(string fileName, Cell cell)
        {
            var board = BoardExportImport.Import(Path.Combine(folder, fileName));
            var game = new Game(board);
            var owner = board.WhoMovedLast();
            var lines = game.GetLines(owner);
            board[cell.X, cell.Y] = owner.Opponent();
            BoardFactory.AddCellToLines(cell, new BoardStateBase(new List<Line>(), lines, owner.Opponent(), board));
            return lines;
        }
    }
}