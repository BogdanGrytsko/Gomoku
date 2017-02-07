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
            this.button2.Location = new System.Drawing.Point(767, 51);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = ">";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ForwardBtnClick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(670, 51);
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
            this.moveNumberTxtBox.Location = new System.Drawing.Point(848, 53);
            this.moveNumberTxtBox.Name = "moveNumberTxtBox";
            this.moveNumberTxtBox.ReadOnly = true;
            this.moveNumberTxtBox.Size = new System.Drawing.Size(44, 20);
            this.moveNumberTxtBox.TabIndex = 5;
            // 
            // exportBoardBtn
            // 
            this.exportBoardBtn.Location = new System.Drawing.Point(767, 12);
            this.exportBoardBtn.Name = "exportBoardBtn";
            this.exportBoardBtn.Size = new System.Drawing.Size(75, 23);
            this.exportBoardBtn.TabIndex = 6;
            this.exportBoardBtn.Text = "ExportBoard";
            this.exportBoardBtn.UseVisualStyleBackColor = true;
            this.exportBoardBtn.Click += new System.EventHandler(this.ExportBoardBtnClick);
            // 
            // importBoardBtn
            // 
            this.importBoardBtn.Location = new System.Drawing.Point(849, 12);
            this.importBoardBtn.Name = "importBoardBtn";
            this.importBoardBtn.Size = new System.Drawing.Size(75, 23);
            this.importBoardBtn.TabIndex = 7;
            this.importBoardBtn.Text = "ImportBoard";
            this.importBoardBtn.UseVisualStyleBackColor = true;
            this.importBoardBtn.Click += new System.EventHandler(this.ImportBoardBtnClick);
            // 
            // analyzeBtn
            // 
            this.analyzeBtn.Location = new System.Drawing.Point(910, 127);
            this.analyzeBtn.Name = "analyzeBtn";
            this.analyzeBtn.Size = new System.Drawing.Size(75, 23);
            this.analyzeBtn.TabIndex = 8;
            this.analyzeBtn.Text = "Analyze";
            this.analyzeBtn.UseVisualStyleBackColor = true;
            this.analyzeBtn.Click += new System.EventHandler(this.AnalyzeBtnClick);
            // 
            // analyzisTreeView
            // 
            this.analyzisTreeView.Location = new System.Drawing.Point(673, 197);
            this.analyzisTreeView.Name = "analyzisTreeView";
            this.analyzisTreeView.Size = new System.Drawing.Size(312, 437);
            this.analyzisTreeView.TabIndex = 9;
            this.analyzisTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.AnalyzisTreeViewNodeClick);
            // 
            // importBoardFileDialog
            // 
            this.importBoardFileDialog.FileName = "openFileDialog1";
            // 
            // depthTextBox
            // 
            this.depthTextBox.Location = new System.Drawing.Point(712, 129);
            this.depthTextBox.Name = "depthTextBox";
            this.depthTextBox.Size = new System.Drawing.Size(33, 20);
            this.depthTextBox.TabIndex = 10;
            this.depthTextBox.Text = "4";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(670, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Depth";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(751, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Width";
            // 
            // widthTxtBox
            // 
            this.widthTxtBox.Location = new System.Drawing.Point(792, 129);
            this.widthTxtBox.Name = "widthTxtBox";
            this.widthTxtBox.Size = new System.Drawing.Size(37, 20);
            this.widthTxtBox.TabIndex = 13;
            this.widthTxtBox.Text = "20";
            // 
            // totalStateCountTxtBox
            // 
            this.totalStateCountTxtBox.Location = new System.Drawing.Point(929, 160);
            this.totalStateCountTxtBox.Name = "totalStateCountTxtBox";
            this.totalStateCountTxtBox.ReadOnly = true;
            this.totalStateCountTxtBox.Size = new System.Drawing.Size(56, 20);
            this.totalStateCountTxtBox.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(857, 163);
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
            this.label4.Location = new System.Drawing.Point(733, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "MinMax";
            // 
            // minMaxTxtBox
            // 
            this.minMaxTxtBox.Location = new System.Drawing.Point(783, 160);
            this.minMaxTxtBox.Name = "minMaxTxtBox";
            this.minMaxTxtBox.ReadOnly = true;
            this.minMaxTxtBox.Size = new System.Drawing.Size(56, 20);
            this.minMaxTxtBox.TabIndex = 17;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 646);
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
    }
}

