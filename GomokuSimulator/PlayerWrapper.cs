﻿using System;
using System.Reflection;
using Gomoku2;

namespace GomokuSimulator
{
    public class PlayerWrapper
    {
        private readonly object player;
        private readonly MethodInfo doMove;
        private readonly MethodInfo doOpponentMove;
        private readonly PropertyInfo lastEstimate;

        public PlayerWrapper(string playerGameName)
        {
            var type = GetType(playerGameName);
            player = Activator.CreateInstance(type, 15, 15);
            doMove = type.GetMethod("DoMove", new Type[] { });
            doOpponentMove = type.GetMethod("DoOpponentMove", new [] { typeof(int), typeof(int) });
            lastEstimate = type.GetProperty("LastEstimate");
        }

        public Cell DoMove()
        {
            var cell = doMove.Invoke(player, null);
            return new Cell(GetPropertyValue(cell, "X"), GetPropertyValue(cell, "Y"));
        }

        private static int GetPropertyValue(object obj, string propName)
        {
            return (int)obj.GetType().GetProperty(propName).GetValue(obj, null);
        }

        public int? LastEstimate
        {
            get
            {
                if (lastEstimate == null) return null;
                return (int)lastEstimate.GetValue(player, null);
            }
        }

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