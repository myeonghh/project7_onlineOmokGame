namespace project_omok
{
    partial class OmokMain
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.파일ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.보기ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.수순ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.nickNameText = new System.Windows.Forms.TextBox();
            this.serverJoinBtn = new System.Windows.Forms.Button();
            this.userListView = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.matchingBtn = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Location = new System.Drawing.Point(0, -1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(963, 695);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.BackgroundImage = global::project_omok.Properties.Resources.oh木intro;
            this.tabPage1.Controls.Add(this.serverJoinBtn);
            this.tabPage1.Controls.Add(this.nickNameText);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(955, 669);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.matchingBtn);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.userListView);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(955, 669);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.panel1);
            this.tabPage3.Controls.Add(this.menuStrip1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(955, 669);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(2, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(669, 553);
            this.panel1.TabIndex = 4;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.파일ToolStripMenuItem1,
            this.보기ToolStripMenuItem1,
            this.수순ToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(3, 3);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(949, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip2";
            // 
            // 파일ToolStripMenuItem1
            // 
            this.파일ToolStripMenuItem1.Name = "파일ToolStripMenuItem1";
            this.파일ToolStripMenuItem1.Size = new System.Drawing.Size(43, 20);
            this.파일ToolStripMenuItem1.Text = "파일";
            // 
            // 보기ToolStripMenuItem1
            // 
            this.보기ToolStripMenuItem1.Name = "보기ToolStripMenuItem1";
            this.보기ToolStripMenuItem1.Size = new System.Drawing.Size(43, 20);
            this.보기ToolStripMenuItem1.Text = "보기";
            // 
            // 수순ToolStripMenuItem1
            // 
            this.수순ToolStripMenuItem1.Name = "수순ToolStripMenuItem1";
            this.수순ToolStripMenuItem1.Size = new System.Drawing.Size(43, 20);
            this.수순ToolStripMenuItem1.Text = "수순";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(293, 338);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "닉네임";
            // 
            // nickNameText
            // 
            this.nickNameText.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.nickNameText.Location = new System.Drawing.Point(385, 341);
            this.nickNameText.Name = "nickNameText";
            this.nickNameText.Size = new System.Drawing.Size(183, 29);
            this.nickNameText.TabIndex = 3;
            // 
            // serverJoinBtn
            // 
            this.serverJoinBtn.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.serverJoinBtn.Location = new System.Drawing.Point(427, 396);
            this.serverJoinBtn.Name = "serverJoinBtn";
            this.serverJoinBtn.Size = new System.Drawing.Size(98, 35);
            this.serverJoinBtn.TabIndex = 6;
            this.serverJoinBtn.Text = "접속하기";
            this.serverJoinBtn.UseVisualStyleBackColor = true;
            // 
            // userListView
            // 
            this.userListView.HideSelection = false;
            this.userListView.Location = new System.Drawing.Point(6, 50);
            this.userListView.Name = "userListView";
            this.userListView.Size = new System.Drawing.Size(941, 468);
            this.userListView.TabIndex = 0;
            this.userListView.UseCompatibleStateImageBehavior = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(23, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "유저목록";
            // 
            // matchingBtn
            // 
            this.matchingBtn.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.matchingBtn.Location = new System.Drawing.Point(384, 533);
            this.matchingBtn.Name = "matchingBtn";
            this.matchingBtn.Size = new System.Drawing.Size(177, 41);
            this.matchingBtn.TabIndex = 2;
            this.matchingBtn.Text = "대전 신청";
            this.matchingBtn.UseVisualStyleBackColor = true;
            this.matchingBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // OmokMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 698);
            this.Controls.Add(this.tabControl1);
            this.Name = "OmokMain";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 파일ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 보기ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 수순ToolStripMenuItem1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nickNameText;
        private System.Windows.Forms.Button serverJoinBtn;
        private System.Windows.Forms.ListView userListView;
        private System.Windows.Forms.Button matchingBtn;
        private System.Windows.Forms.Label label2;
    }
}