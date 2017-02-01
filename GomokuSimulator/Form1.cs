using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Gomoku2;

namespace GomokuSimulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var game = new Gomoku2.Game(15, 15);
            DrawGrid(game.Board);
        }

        private void DrawGrid(BoardCell[,] board)
        {
            Controls.Clear();


            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    var tmpButton = CreateButton(x,y);
                    tmpButton.Text = GetButtonText(board[x, y]);
                    tmpButton.ForeColor = GetButonColor(board[x, y]);
                    Controls.Add(tmpButton);
                }
            }
        }

        private Color GetButonColor(BoardCell boardCell)
        {
            switch (boardCell)
            {
                case BoardCell.None:
                    return Color.Aqua;
                case BoardCell.First:
                    return Color.Red;
                case BoardCell.Second:
                    return Color.Blue;
            }
            return Color.Black;
        }

        private string GetButtonText(BoardCell boardCell)
        {
            switch (boardCell)
            {
                case BoardCell.None:
                    return string.Empty + "1";
                case BoardCell.First:
                    return "X";
                case BoardCell.Second:
                    return "O";
            }
            return string.Empty;
        }

        private static Button CreateButton(int x, int y)
        {
            const int buttonWidth = 40;
            const int buttonHeight = 40;
            const int distance = 0;
            const int startX = 0;
            const int startY = 0;

            var tmpButton = new Button
            {
                Top = startX + (x*buttonHeight + distance),
                Left = startY + (y*buttonWidth + distance),
                Width = buttonWidth,
                Height = buttonHeight
            };
            return tmpButton;
        }
    }
}
