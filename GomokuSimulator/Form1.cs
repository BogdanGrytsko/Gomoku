using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Gomoku2;

namespace GomokuSimulator
{
    public partial class Form1 : Form
    {
        private readonly GameRunner gameRunner = new GameRunner();
        private List<BoardCell[,]> boardStates;
        private int currState;

        public Form1()
        {
            InitializeComponent();
            DrawGrid(15,15);
        }

        private void DrawGrid(int xcount, int ycount)
        {
            for (int x = 0; x < xcount; x++)
            {
                for (int y = 0; y < ycount; y++)
                {
                    var tmpButton = CreateButton(x,y);
                    Controls.Add(tmpButton);
                }
            }
        }

        private void UpdateGrid(BoardCell[,] board)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    var cell = Controls[GetName(x,y)];
                    cell.Text = GetButtonText(board[x, y]);
                    cell.ForeColor = GetButonColor(board[x, y]);
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
                    return string.Empty;
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
                Height = buttonHeight,
                Name = GetName(x,y)
            };
            return tmpButton;
        }

        private static string GetName(int x, int y)
        {
            return string.Format("ButtonX{0}Y{1}", x, y);
        }

        private void PlayButtonClick(object sender, System.EventArgs e)
        {
            boardStates = gameRunner.PlayGame().ToList();
            foreach (var board in boardStates)
            {
                UpdateGrid(board);
                currState++;
                Application.DoEvents();
                Thread.Sleep(100);
            }
        }

        private void BackBtnClick(object sender, System.EventArgs e)
        {
            if (currState > 1)
            {
                currState--;
                UpdateGrid(boardStates[currState]);
            }
        }

        private void ForwardBtnClick(object sender, System.EventArgs e)
        {
            if (currState < boardStates.Count - 1)
            {
                currState++;
                UpdateGrid(boardStates[currState]);
            }
        }
    }
}
