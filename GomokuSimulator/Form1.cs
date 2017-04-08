using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Gomoku2;
using Gomoku2.CellObjects;
using Gomoku2.StateCache;

namespace GomokuSimulator
{
    public partial class Form1 : Form
    {
        private const string Human = "Human";
        private GamePlayer gamePlayer = new GamePlayer("Gomoku2.Game", "Gomoku.Game");
        private HumanComputerGame humanGame;
        private readonly List<EstimatedBoard> boards = new List<EstimatedBoard>();
        private EstimatedBoard currentBoard;
        private int currState;

        public Form1()
        {
            InitializeComponent();
            widthTxtBox.Text = Game.DefaultWidth.ToString();
            depthTextBox.Text = Game.DefaultDepth.ToString();
            DrawGrid(16,16);
            var players = PossiblePlayers().ToArray();
            SetComboBox(player1Box, players);
            SetComboBox(player2Box, players);
            player1Box.SelectedItem = players[0];
            player2Box.SelectedItem = players[1];
        }

        private static void SetComboBox(ComboBox comboBox, object[] players)
        {
            comboBox.Items.AddRange(players);
            comboBox.DisplayMember = "Text";
            comboBox.ValueMember = "Value";
        }

        private IEnumerable<object> PossiblePlayers()
        {
            yield return new ComboBoxItem {Text = "Gomoku2", Value = "Gomoku2.Game"};
            yield return new ComboBoxItem { Text = "Gomoku", Value = "Gomoku.Game"};
            yield return new ComboBoxItem { Text = Human, Value = Human };
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
                    SetButtonProps(cell, board[x,y]);
                }
            }

            moveNumberTxtBox.Text = currState.ToString();
            minMaxTxtBox.Text = estimatedBoard.Estimate.ToString();
            if (estimatedBoard.PlayerType == BoardCell.First)
                totalElapsedPlayer1TxtBox.Text = estimatedBoard.Elapsed.ToString();
            if (estimatedBoard.PlayerType == BoardCell.Second)
                totalElapsedPlayer2TxtBox.Text = estimatedBoard.Elapsed.ToString();
        }

        private static void SetButtonProps(Control cell, BoardCell boardCell)
        {
            cell.Text = boardCell.GetCellText();
            cell.ForeColor = boardCell.GetButonColor();
        }

        private Button CreateButton(int x, int y)
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
                Name = GetName(x,y),
                Tag = new Cell(x,y)
            };
            tmpButton.Click += CellClick;
            if (x == 15)
                tmpButton.Text = y.ToString();
            if (y == 15)
                tmpButton.Text = x.ToString();
            return tmpButton;
        }

        private void CellClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var button = (Button)sender;
                var board = humanGame.HumanMove((Cell)button.Tag);
                boards.Add(board);
                currState++;
                SetButtonProps(button, humanGame.HumanMoveType);
                Application.DoEvents();
                if (humanGame.GameIsFinished)
                    MessageBox.Show("Game is finished!");
                DoComputerMove();
                Application.DoEvents();
                if (humanGame.GameIsFinished)
                    MessageBox.Show("Game is finished!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static string GetName(int x, int y)
        {
            return string.Format("ButtonX{0}Y{1}", x, y);
        }

        private void PlayButtonClick(object sender, EventArgs e)
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
            OutPutEstimations();
        }

        private void OutPutEstimations()
        {
            using (var sw = new StreamWriter("Estimations.txt"))
            {
                for (int i = 0; i < boards.Count; i++)
                {
                    sw.WriteLine("{0};{1}", i, boards[i].Estimate);
                }
            }
        }

        private void BackBtnClick(object sender, EventArgs e)
        {
            if (currState > 0)
            {
                currState--;
                UpdateGrid(boards[currState]);
            }
        }

        private void ForwardBtnClick(object sender, EventArgs e)
        {
            if (currState < boards.Count - 1)
            {
                currState++;
                UpdateGrid(boards[currState]);
            }
        }

        private void ExportBoardBtnClick(object sender, EventArgs e)
        {
            if (exportBoardFileDialog.ShowDialog() != DialogResult.OK) return;

            BoardExportImport.Export(currentBoard.Board, exportBoardFileDialog.FileName);
        }

        private void ImportBoardBtnClick(object sender, EventArgs e)
        {
            if (importBoardFileDialog.ShowDialog() != DialogResult.OK) return;

            var board = BoardExportImport.Import(importBoardFileDialog.FileName);
            UpdateGrid(new EstimatedBoard {Board = board});
        }
        
        private void AnalyzeBtnClick(object sender, EventArgs e)
        {
            analyzisTreeView.Nodes.Clear();

            var board = currentBoard.Board;
            var game = new Game(board) { AnalyzeModeOn = true };
            game.DoMove(board.WhoMovesNext(), AnalyzeDepth, AnalyzeWidth);
            PopulateTree(analyzisTreeView.Nodes, game.GameStates);

            BindGameTextBoxValues(game);
        }

        private void BindGameTextBoxValues(Game game)
        {
            elapsedTxtBox.Text = game.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            minMaxTxtBox.Text = game.LastEstimate.ToString();
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

        private void player1Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item1 = (ComboBoxItem) player1Box.SelectedItem;
            var item2 = (ComboBoxItem) player2Box.SelectedItem;
            if (item1 == null || item2 == null) return;

            UpdateGrid(new EstimatedBoard());
            boards.Clear();
            currState = 0;
            
            Application.DoEvents();
            //human makes first move
            if (item1.Value == Human)
            {
                humanGame = new HumanComputerGame(new Game(15, 15), BoardCell.First);
                return;
            }
            if (item2.Value == Human)
            {
                humanGame = new HumanComputerGame(new Game(15, 15), BoardCell.Second);
                DoComputerMove();
                return;
            }
            gamePlayer = new GamePlayer(item1.Value, item2.Value);
        }

        private void DoComputerMove()
        {
            var board = humanGame.ComputerMove();
            boards.Add(board);
            UpdateGrid(board);
            currState++;
        }

        private void player2Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            player1Box_SelectedIndexChanged(sender, e);
        }
    }
}
