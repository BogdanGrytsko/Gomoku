using System;
using System.Diagnostics;
using System.Reflection;
using Gomoku2;
using Gomoku2.CellObjects;

namespace GomokuSimulator
{
    public class PlayerWrapper
    {
        private readonly object player;
        private readonly MethodInfo doMove;
        private readonly MethodInfo doOpponentMove;
        private readonly PropertyInfo lastEstimate, depth;
        private readonly Stopwatch sw = new Stopwatch();

        public PlayerWrapper(string playerGameName)
        {
            var type = GetType(playerGameName);
            player = Activator.CreateInstance(type, 15, 15);
            doMove = type.GetMethod("DoMove", new Type[] { });
            doOpponentMove = type.GetMethod("DoOpponentMove", new [] { typeof(int), typeof(int) });
            lastEstimate = type.GetProperty("LastEstimate");
            depth = type.GetProperty("Depth");
        }

        public Cell DoMove()
        {
            sw.Start();
            var cell = doMove.Invoke(player, null);
            sw.Stop();
            return new Cell(GetPropertyValue(cell, "X"), GetPropertyValue(cell, "Y"));
        }

        public int Depth
        {
            set
            {
                depth?.SetValue(player, value);
            }
        }

        private static int GetPropertyValue(object obj, string propName)
        {
            return (int)obj.GetType().GetProperty(propName).GetValue(obj, null);
        }

        public int? LastEstimate => (int?) lastEstimate?.GetValue(player, null);

        public TimeSpan Elapsed => sw.Elapsed;

        public void DoOpponentMove(int x, int y)
        {
            doOpponentMove.Invoke(player, new object[] {x, y});
        }

        private static Type GetType(string typeWithAssembly)
        {
            var player1Assembly = typeWithAssembly.Split('.')[0];
            return Type.GetType(typeWithAssembly + "," + player1Assembly);
        }
    }
}