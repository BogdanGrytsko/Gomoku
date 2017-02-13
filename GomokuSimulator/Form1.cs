using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Gomoku2;

namespace GomokuSimulator
{
    public partial class Form1 : Form
    {
        private readonly GamePlayer gamePlayer = new GamePlayer("Gomoku2.Game", "Gomoku2.Game");
        private readonly List<EstimatedBoard> boards = new List<EstimatedBoard>();
        private EstimatedBoard currentBoard;
        private int currState;

        public Form1()
        {
            InitializeComponent();
            widthTxtBox.Text = Game.DefaultWidth.ToString();
            depthTextBox.Text = Game.DefaultDepth.ToString();
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
            currentBoard = estimatedBoard;
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

            moveNumberTxtBox.Text = currState.ToString();
            //todo: this binding is made twice. Refactor - Remove.
            minMaxTxtBox.Text = estimatedBoard.Estimate.ToString();
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
            boards.Clear();
            currState = 0;
            foreach (var board in gamePlayer.PlayGame())
            {
                boards.Add(board);
                UpdateGrid(board);
                currState++;
                Application.DoEvents();
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
            if (exportBoardFileDialog.ShowDialog() != DialogResult.OK) return;

            BoardExportImport.Export(currentBoard, exportBoardFileDialog.FileName);
        }

        private void ImportBoardBtnClick(object sender, System.EventArgs e)
        {
            if (importBoardFileDialog.ShowDialog() != DialogResult.OK) return;

            var board = BoardExportImport.Import(importBoardFileDialog.FileName);
            UpdateGrid(board);
        }
        
        private void AnalyzeBtnClick(object sender, System.EventArgs e)
        {
            analyzisTreeView.Nodes.Clear();

            var board = currentBoard.Board;
            var game = new Game(board);
            game.DoMove(board.WhoMovesNext(), AnalyzeDepth, AnalyzeWidth);
            PopulateTree(analyzisTreeView.Nodes, game.GameStates);

            BindGameTextBoxValues(game);
        }

        private void BindGameTextBoxValues(Game game)
        {
            elapsedTxtBox.Text = game.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            minMaxTxtBox.Text = game.EstimatedBoard.Estimate.ToString();
            totalStateCountTxtBox.Text = game.GameStates.TotalStateCount().ToString();
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
                var childrenStateCount = gameState.Children.TotalStateCount();
                var nodeText = childrenStateCount == 0
                    ? string.Format("{0} {1}", gameState.Cell, gameState.Estimate)
                    : string.Format("{0} {1} ({2})", gameState.Cell, gameState.Estimate, childrenStateCount);
                var node = nodes.Add(nodeText);
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
