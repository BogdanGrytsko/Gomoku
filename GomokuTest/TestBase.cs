using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gomoku2;
using Gomoku2.CellObjects;
using Gomoku2.LineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GomokuTest
{
    public abstract class TestBase
    {
        protected abstract string Folder { get; }

        protected List<Line> GetLines(string fileName)
        {
            var board = BoardExportImport.Import(Path.Combine(Folder, fileName));
            var game = new Game(board);
            return game.GetLines(board.WhoMovedLast());
        }

        protected Game TestMove(string boardName, int depth, params Cell[] correctMoves)
        {
            return TestMove(Folder, boardName, depth, correctMoves);
        }

        protected void DoTestWin(string boardName, params Cell[] correctMoves)
        {
            var game = TestMove(boardName, correctMoves);
            Assert.IsTrue(Win(game.LastEstimate));
        }

        protected void DoTestWin(string boardName, int depth, params Cell[] correctMoves)
        {
            var game = TestMove(boardName, depth, correctMoves);
            Assert.IsTrue(Win(game.LastEstimate));
        }

        protected static bool Win(int estim)
        {
            return Math.Abs(estim) >= (int)LineType.DoubleThreat / 2;
        }

        protected Game TestMove(string boardName, params Cell[] correctMoves)
        {
            return TestMove(boardName, Game.DefaultDepth, correctMoves);
        }

        public static Game TestMove(string folderPath, string boardName, int depth, params Cell[] correctMoves)
        {
            var board = BoardExportImport.Import(Path.Combine(folderPath, boardName));
            var game = new Game(board);
            var move = game.DoMove(board.WhoMovesNext(), depth, Game.DefaultWidth);
            Assert.IsTrue(correctMoves.Any(cm => cm == move));
            return game;
        }
    }
}