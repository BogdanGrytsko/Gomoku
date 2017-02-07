using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Gomoku2;

namespace GomokuSimulator
{
    public partial class Form1 : Form
    {
        private readonly GameRunner gameRunner = new GameRunner();
        private readonly List<EstimatedBoard> boards = new List<EstimatedBoard>();
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
                    cell.Text = board[x, y].GetCellText();
                    cell.ForeColor = board[x, y].GetButonColor();
                }
            }

            estimateTxtBox.Text = estimatedBoard.Estimate.ToString();
            moveNumberTxtBox.Text = currState.ToString();
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
                        sw.Write(board[x, y].GetCellText());
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

        private void AnalyzeBtnClick(object sender, System.EventArgs e)
        {
            analyzisTreeView.Nodes.Clear();

            var board = boards[currState].Board;
            var game = new Game(board);
            game.DoMove(board.WhoMovesNext(), AnalyzeDepth, AnalyzeWidth);
            PopulateTree(analyzisTreeView.Nodes, game.GameStates);
        }

        private int AnalyzeDepth
        {
            get { return int.Parse(depthTextBox.Text); }
        }

        private int AnalyzeWidth
        {
            get { return int.Parse(widthTxtBox.Text); }
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

        private void AnalyzisTreeViewNodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var gameState = (GameState) e.Node.Tag;
            UpdateGrid(gameState.EstimatedBoard);
        }
    }
}
