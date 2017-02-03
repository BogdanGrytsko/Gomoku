using System;
using System.Drawing;
using Gomoku2;

namespace GomokuSimulator
{
    public static class VisualExtensions
    {
        public static Color GetButonColor(this BoardCell boardCell)
        {
            switch (boardCell)
            {
                case BoardCell.None:
                    return Color.White;
                case BoardCell.First:
                    return Color.Red;
                case BoardCell.Second:
                    return Color.Blue;
            }
            return Color.Black;
        }

        public static string GetCellText(this BoardCell boardCell)
        {
            switch (boardCell)
            {
                case BoardCell.None:
                    return " ";
                case BoardCell.First:
                    return "X";
                case BoardCell.Second:
                    return "O";
            }
            return string.Empty;
        }
    }
}