using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Gomoku2;

namespace GomokuSimulator
{
    public partial class Form1 : Form
    {
        private readonly GameRunner gameRunner = new GameRunner();
        private readonly List<EstimatedBoard> boards = new List<EstimatedBoard>();
        private readonly List<GameState> allGameStates = new List<GameState>();
        private readonly List<GameState> rootGameStates = new List<GameState>();
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

        private void UpdateGrid(EstimatedBoard estimatedBoard)
        {
            var board = estimatedBoard.Board;
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    var cell = Controls[GetName(x,y)];
                    cell.Text = GetCellText(board[x, y]);
                    cell.ForeColor = GetButonColor(board[x, y]);
                }
            }

            estimateTxtBox.Text = estimatedBoard.Estimate.ToString();
            moveNumberTxtBox.Text = currState.ToString();
        }

        private Color GetButonColor(BoardCell boardCell)
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

        private string GetCellText(BoardCell boardCell)
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
            foreach (var board in gameRunner.PlayGame())
            {
                boards.Add(board);
                UpdateGrid(board);
                currState++;
                Application.DoEvents();
                //Thread.Sleep(100);
            }
        }

        private void BackBtnClick(object sender, System.EventArgs e)
        {
            if (currState > 0)
            {
                currState--;
                UpdateGrid(boards[currState]);
            }
        }

        private void ForwardBtnClick(object sender, System.EventArgs e)
        {
            if (currState < boards.Count - 1)
            {
                currState++;
                UpdateGrid(boards[currState]);
            }
        }

        private void ExportBoardBtnClick(object sender, System.EventArgs e)
        {
            ExportBoard(boards[currState]);
        }

        private void ExportBoard(EstimatedBoard estimatedBoard)
        {
            using (var sw = new StreamWriter(string.Format("board{0}.txt", currState)))
            {
                var board = estimatedBoard.Board;
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    for (int y = 0; y < board.GetLength(1); y++)
                    {
                        sw.Write(GetCellText(board[x,y]));
                    }
                    sw.WriteLine();
                }
            }
        }

        private void ImportBoardBtnClick(object sender, System.EventArgs e)
        {
            if (importBoardFileDialog.ShowDialog() != DialogResult.OK) return;

            var board = GetBoard(importBoardFileDialog.FileName);
            boards.Add(board);
            UpdateGrid(board);
        }

        private static EstimatedBoard GetBoard(string fileName)
        {
            var board = new BoardCell[15, 15];
            using (var sw = new StreamReader(fileName))
            {
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    var line = sw.ReadLine();
                    for (int y = 0; y < board.GetLength(1); y++)
                    {
                        if (line[y].ToString() == " ") board[x, y] = BoardCell.None;
                        if (line[y].ToString() == "X") board[x, y] = BoardCell.First;
                        if (line[y].ToString() == "O") board[x, y] = BoardCell.Second;
                    }
                }
            }
            return new EstimatedBoard {Board = board};
        }

        private const int analyzeDepth = 5;

        private void AnalyzeBtnClick(object sender, System.EventArgs e)
        {
            analyzisTreeView.Nodes.Clear();
            rootGameStates.Clear();
            allGameStates.Clear();

            var game = new Game(boards[currState].Board);
            game.StateChanged += GameOnStateChanged;
            game.DoMove(analyzeDepth);
            PopulateTree(analyzisTreeView.Nodes, rootGameStates);
        }

        private static void PopulateTree(TreeNodeCollection nodes, IEnumerable<GameState> gameStates)
        {
            foreach (var gameState in gameStates)
            {
                var node = nodes.Add(gameState.Cell + " " + gameState.Estimate);
                node.Tag = gameState;
                PopulateTree(node.Nodes, gameState.Children);
            }
        }

        private void GameOnStateChanged(GameState gameState)
        {
            if (gameState.BoardState.Depth == analyzeDepth)
            {
                rootGameStates.Add(gameState);
                allGameStates.Add(gameState);
                return;
            }
            var parent = allGameStates.FindLast(gs => gs.BoardState.Depth == gameState.BoardState.Depth + 1);
            parent.Children.Add(gameState);
            allGameStates.Add(gameState);
        }

        private void AnalyzisTreeViewNodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var gameState = (GameState) e.Node.Tag;
            UpdateGrid(gameState.EstimatedBoard);
        }
    }
}
