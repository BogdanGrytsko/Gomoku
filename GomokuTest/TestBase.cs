using System.Collections.Generic;
using System.IO;
using Gomoku2;
using Gomoku2.LineCore;

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
    }
}