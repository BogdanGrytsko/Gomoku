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
            this.estimateTxtBox = new System.Windows.Forms.TextBox();
            this.moveNumberTxtBox = new System.Windows.Forms.TextBox();
            this.exportBoardBtn = new System.Windows.Forms.Button();
            this.importBoardBtn = new System.Windows.Forms.Button();
            this.analyzeBtn = new System.Windows.Forms.Button();
            this.analyzisTreeView = new System.Windows.Forms.TreeView();
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
            // estimateTxtBox
            // 
            this.estimateTxtBox.Location = new System.Drawing.Point(670, 100);
            this.estimateTxtBox.Name = "estimateTxtBox";
            this.estimateTxtBox.ReadOnly = true;
            this.estimateTxtBox.Size = new System.Drawing.Size(100, 20);
            this.estimateTxtBox.TabIndex = 4;
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
            this.analyzeBtn.Location = new System.Drawing.Point(777, 100);
            this.analyzeBtn.Name = "analyzeBtn";
            this.analyzeBtn.Size = new System.Drawing.Size(75, 23);
            this.analyzeBtn.TabIndex = 8;
            this.analyzeBtn.Text = "Analyze";
            this.analyzeBtn.UseVisualStyleBackColor = true;
            this.analyzeBtn.Click += new System.EventHandler(this.AnalyzeBtnClick);
            // 
            // analyzisTreeView
            // 
            this.analyzisTreeView.Location = new System.Drawing.Point(670, 129);
            this.analyzisTreeView.Name = "analyzisTreeView";
            this.analyzisTreeView.Size = new System.Drawing.Size(265, 479);
            this.analyzisTreeView.TabIndex = 9;
            this.analyzisTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.AnalyzisTreeViewNodeClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(947, 646);
            this.Controls.Add(this.analyzisTreeView);
            this.Controls.Add(this.analyzeBtn);
            this.Controls.Add(this.importBoardBtn);
            this.Controls.Add(this.exportBoardBtn);
            this.Controls.Add(this.moveNumberTxtBox);
            this.Controls.Add(this.estimateTxtBox);
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
        private System.Windows.Forms.TextBox estimateTxtBox;
        private System.Windows.Forms.TextBox moveNumberTxtBox;
        private System.Windows.Forms.Button exportBoardBtn;
        private System.Windows.Forms.Button importBoardBtn;
        private System.Windows.Forms.Button analyzeBtn;
        private System.Windows.Forms.TreeView analyzisTreeView;
    }
}

