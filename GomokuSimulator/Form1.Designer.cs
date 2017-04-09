using Gomoku2;

namespace GomokuSimulator
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.moveNumberTxtBox = new System.Windows.Forms.TextBox();
            this.exportBoardBtn = new System.Windows.Forms.Button();
            this.importBoardBtn = new System.Windows.Forms.Button();
            this.analyzeBtn = new System.Windows.Forms.Button();
            this.analyzisTreeView = new System.Windows.Forms.TreeView();
            this.importBoardFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.depthTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.widthTxtBox = new System.Windows.Forms.TextBox();
            this.totalStateCountTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.exportBoardFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.minMaxTxtBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.elapsedTxtBox = new System.Windows.Forms.TextBox();
            this.totalElapsedPlayer1TxtBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.totalElapsedPlayer2TxtBox = new System.Windows.Forms.TextBox();
            this.player1Box = new System.Windows.Forms.ComboBox();
            this.player2Box = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(670, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Play";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.PlayButtonClick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(835, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = ">";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ForwardBtnClick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(754, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "<";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.BackBtnClick);
            // 
            // moveNumberTxtBox
            // 
            this.moveNumberTxtBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.moveNumberTxtBox.Location = new System.Drawing.Point(929, 15);
            this.moveNumberTxtBox.Name = "moveNumberTxtBox";
            this.moveNumberTxtBox.ReadOnly = true;
            this.moveNumberTxtBox.Size = new System.Drawing.Size(44, 20);
            this.moveNumberTxtBox.TabIndex = 5;
            // 
            // exportBoardBtn
            // 
            this.exportBoardBtn.Location = new System.Drawing.Point(670, 50);
            this.exportBoardBtn.Name = "exportBoardBtn";
            this.exportBoardBtn.Size = new System.Drawing.Size(75, 23);
            this.exportBoardBtn.TabIndex = 6;
            this.exportBoardBtn.Text = "ExportBoard";
            this.exportBoardBtn.UseVisualStyleBackColor = true;
            this.exportBoardBtn.Click += new System.EventHandler(this.ExportBoardBtnClick);
            // 
            // importBoardBtn
            // 
            this.importBoardBtn.Location = new System.Drawing.Point(754, 50);
            this.importBoardBtn.Name = "importBoardBtn";
            this.importBoardBtn.Size = new System.Drawing.Size(75, 23);
            this.importBoardBtn.TabIndex = 7;
            this.importBoardBtn.Text = "ImportBoard";
            this.importBoardBtn.UseVisualStyleBackColor = true;
            this.importBoardBtn.Click += new System.EventHandler(this.ImportBoardBtnClick);
            // 
            // analyzeBtn
            // 
            this.analyzeBtn.Location = new System.Drawing.Point(910, 156);
            this.analyzeBtn.Name = "analyzeBtn";
            this.analyzeBtn.Size = new System.Drawing.Size(75, 23);
            this.analyzeBtn.TabIndex = 8;
            this.analyzeBtn.Text = "Analyze";
            this.analyzeBtn.UseVisualStyleBackColor = true;
            this.analyzeBtn.Click += new System.EventHandler(this.AnalyzeBtnClick);
            // 
            // analyzisTreeView
            // 
            this.analyzisTreeView.Location = new System.Drawing.Point(673, 226);
            this.analyzisTreeView.Name = "analyzisTreeView";
            this.analyzisTreeView.Size = new System.Drawing.Size(566, 536);
            this.analyzisTreeView.TabIndex = 9;
            this.analyzisTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.AnalyzisTreeViewNodeClick);
            // 
            // importBoardFileDialog
            // 
            this.importBoardFileDialog.FileName = "openFileDialog1";
            // 
            // depthTextBox
            // 
            this.depthTextBox.Location = new System.Drawing.Point(712, 158);
            this.depthTextBox.Name = "depthTextBox";
            this.depthTextBox.Size = new System.Drawing.Size(33, 20);
            this.depthTextBox.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(670, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Depth";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(751, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Width";
            // 
            // widthTxtBox
            // 
            this.widthTxtBox.Location = new System.Drawing.Point(792, 158);
            this.widthTxtBox.Name = "widthTxtBox";
            this.widthTxtBox.Size = new System.Drawing.Size(37, 20);
            this.widthTxtBox.TabIndex = 13;
            // 
            // totalStateCountTxtBox
            // 
            this.totalStateCountTxtBox.Location = new System.Drawing.Point(929, 189);
            this.totalStateCountTxtBox.Name = "totalStateCountTxtBox";
            this.totalStateCountTxtBox.ReadOnly = true;
            this.totalStateCountTxtBox.Size = new System.Drawing.Size(56, 20);
            this.totalStateCountTxtBox.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(881, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "State #";
            // 
            // exportBoardFileDialog
            // 
            this.exportBoardFileDialog.DefaultExt = "txt";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(769, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "MinMax";
            // 
            // minMaxTxtBox
            // 
            this.minMaxTxtBox.Location = new System.Drawing.Point(819, 189);
            this.minMaxTxtBox.Name = "minMaxTxtBox";
            this.minMaxTxtBox.ReadOnly = true;
            this.minMaxTxtBox.Size = new System.Drawing.Size(56, 20);
            this.minMaxTxtBox.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(670, 192);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Elapsed";
            // 
            // elapsedTxtBox
            // 
            this.elapsedTxtBox.Location = new System.Drawing.Point(712, 189);
            this.elapsedTxtBox.Name = "elapsedTxtBox";
            this.elapsedTxtBox.ReadOnly = true;
            this.elapsedTxtBox.Size = new System.Drawing.Size(56, 20);
            this.elapsedTxtBox.TabIndex = 19;
            // 
            // totalElapsedPlayer1TxtBox
            // 
            this.totalElapsedPlayer1TxtBox.Location = new System.Drawing.Point(900, 82);
            this.totalElapsedPlayer1TxtBox.Name = "totalElapsedPlayer1TxtBox";
            this.totalElapsedPlayer1TxtBox.ReadOnly = true;
            this.totalElapsedPlayer1TxtBox.Size = new System.Drawing.Size(75, 20);
            this.totalElapsedPlayer1TxtBox.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(816, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "TotalP1Time";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(816, 122);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "TotalP2Time";
            // 
            // totalElapsedPlayer2TxtBox
            // 
            this.totalElapsedPlayer2TxtBox.Location = new System.Drawing.Point(900, 115);
            this.totalElapsedPlayer2TxtBox.Name = "totalElapsedPlayer2TxtBox";
            this.totalElapsedPlayer2TxtBox.ReadOnly = true;
            this.totalElapsedPlayer2TxtBox.Size = new System.Drawing.Size(75, 20);
            this.totalElapsedPlayer2TxtBox.TabIndex = 22;
            // 
            // player1Box
            // 
            this.player1Box.FormattingEnabled = true;
            this.player1Box.Location = new System.Drawing.Point(673, 86);
            this.player1Box.Name = "player1Box";
            this.player1Box.Size = new System.Drawing.Size(121, 21);
            this.player1Box.TabIndex = 24;
            this.player1Box.SelectedIndexChanged += new System.EventHandler(this.player1Box_SelectedIndexChanged);
            // 
            // player2Box
            // 
            this.player2Box.FormattingEnabled = true;
            this.player2Box.Location = new System.Drawing.Point(673, 119);
            this.player2Box.Name = "player2Box";
            this.player2Box.Size = new System.Drawing.Size(121, 21);
            this.player2Box.TabIndex = 25;
            this.player2Box.SelectedIndexChanged += new System.EventHandler(this.player2Box_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1251, 774);
            this.Controls.Add(this.player2Box);
            this.Controls.Add(this.player1Box);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.totalElapsedPlayer2TxtBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.totalElapsedPlayer1TxtBox);
            this.Controls.Add(this.elapsedTxtBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.minMaxTxtBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.totalStateCountTxtBox);
            this.Controls.Add(this.widthTxtBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.depthTextBox);
            this.Controls.Add(this.analyzisTreeView);
            this.Controls.Add(this.analyzeBtn);
            this.Controls.Add(this.importBoardBtn);
            this.Controls.Add(this.exportBoardBtn);
            this.Controls.Add(this.moveNumberTxtBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox moveNumberTxtBox;
        private System.Windows.Forms.Button exportBoardBtn;
        private System.Windows.Forms.Button importBoardBtn;
        private System.Windows.Forms.Button analyzeBtn;
        private System.Windows.Forms.TreeView analyzisTreeView;
        private System.Windows.Forms.OpenFileDialog importBoardFileDialog;
        private System.Windows.Forms.TextBox depthTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox widthTxtBox;
        private System.Windows.Forms.TextBox totalStateCountTxtBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog exportBoardFileDialog;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox minMaxTxtBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox elapsedTxtBox;
        private System.Windows.Forms.TextBox totalElapsedPlayer1TxtBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox totalElapsedPlayer2TxtBox;
        private System.Windows.Forms.ComboBox player1Box;
        private System.Windows.Forms.ComboBox player2Box;
    }
}

