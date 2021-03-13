namespace JFApp
{
    partial class FormManual
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemSysTemDataPool = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSysData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStations = new System.Windows.Forms.ToolStripMenuItem();
            this.btStart = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.tabControlCF1 = new JFUI.TabControlCF();
            this.btPause = new System.Windows.Forms.Button();
            this.btResume = new System.Windows.Forms.Button();
            this.btEndBatch = new System.Windows.Forms.Button();
            this.btReset = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSysTemDataPool,
            this.toolStripMenuItemStations});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItemSysTemDataPool
            // 
            this.toolStripMenuItemSysTemDataPool.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSysData});
            this.toolStripMenuItemSysTemDataPool.Name = "toolStripMenuItemSysTemDataPool";
            this.toolStripMenuItemSysTemDataPool.Size = new System.Drawing.Size(68, 21);
            this.toolStripMenuItemSysTemDataPool.Text = "调试数据";
            // 
            // toolStripMenuItemSysData
            // 
            this.toolStripMenuItemSysData.Name = "toolStripMenuItemSysData";
            this.toolStripMenuItemSysData.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemSysData.Text = "系统数据";
            this.toolStripMenuItemSysData.Click += new System.EventHandler(this.toolStripMenuItemSysData_Click);
            // 
            // toolStripMenuItemStations
            // 
            this.toolStripMenuItemStations.Name = "toolStripMenuItemStations";
            this.toolStripMenuItemStations.Size = new System.Drawing.Size(68, 21);
            this.toolStripMenuItemStations.Text = "启动设置";
            // 
            // btStart
            // 
            this.btStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btStart.Location = new System.Drawing.Point(476, 2);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(75, 23);
            this.btStart.TabIndex = 2;
            this.btStart.Text = "开始";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // btStop
            // 
            this.btStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btStop.Location = new System.Drawing.Point(720, 2);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(75, 23);
            this.btStop.TabIndex = 3;
            this.btStop.Text = "停止";
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // tabControlCF1
            // 
            this.tabControlCF1.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabControlCF1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlCF1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlCF1.ItemSize = new System.Drawing.Size(35, 100);
            this.tabControlCF1.Location = new System.Drawing.Point(0, 25);
            this.tabControlCF1.Multiline = true;
            this.tabControlCF1.Name = "tabControlCF1";
            this.tabControlCF1.SelectedIndex = 0;
            this.tabControlCF1.Size = new System.Drawing.Size(800, 425);
            this.tabControlCF1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlCF1.TabColor = System.Drawing.SystemColors.ControlDark;
            this.tabControlCF1.TabIndex = 0;
            this.tabControlCF1.SelectedIndexChanged += new System.EventHandler(this.tabControlCF1_SelectedIndexChanged);
            // 
            // btPause
            // 
            this.btPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btPause.Enabled = false;
            this.btPause.Location = new System.Drawing.Point(554, 2);
            this.btPause.Name = "btPause";
            this.btPause.Size = new System.Drawing.Size(45, 23);
            this.btPause.TabIndex = 4;
            this.btPause.Text = "暂停";
            this.btPause.UseVisualStyleBackColor = true;
            this.btPause.Click += new System.EventHandler(this.btPause_Click);
            // 
            // btResume
            // 
            this.btResume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btResume.Enabled = false;
            this.btResume.Location = new System.Drawing.Point(602, 2);
            this.btResume.Name = "btResume";
            this.btResume.Size = new System.Drawing.Size(45, 23);
            this.btResume.TabIndex = 5;
            this.btResume.Text = "恢复";
            this.btResume.UseVisualStyleBackColor = true;
            this.btResume.Click += new System.EventHandler(this.btResume_Click);
            // 
            // btEndBatch
            // 
            this.btEndBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btEndBatch.Enabled = false;
            this.btEndBatch.Location = new System.Drawing.Point(649, 2);
            this.btEndBatch.Name = "btEndBatch";
            this.btEndBatch.Size = new System.Drawing.Size(45, 23);
            this.btEndBatch.TabIndex = 6;
            this.btEndBatch.Text = "结批";
            this.btEndBatch.UseVisualStyleBackColor = true;
            this.btEndBatch.Click += new System.EventHandler(this.btEndBatch_Click);
            // 
            // btReset
            // 
            this.btReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btReset.Location = new System.Drawing.Point(374, 2);
            this.btReset.Name = "btReset";
            this.btReset.Size = new System.Drawing.Size(75, 23);
            this.btReset.TabIndex = 7;
            this.btReset.Text = "复位";
            this.btReset.UseVisualStyleBackColor = true;
            this.btReset.Click += new System.EventHandler(this.btReset_Click);
            // 
            // FormManual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btReset);
            this.Controls.Add(this.btEndBatch);
            this.Controls.Add(this.btResume);
            this.Controls.Add(this.btPause);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.tabControlCF1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormManual";
            this.Text = "FormManual";
            this.Load += new System.EventHandler(this.FormManual_Load);
            this.VisibleChanged += new System.EventHandler(this.FormManual_VisibleChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private JFUI.TabControlCF tabControlCF1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSysTemDataPool;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStations;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSysData;
        private System.Windows.Forms.Button btPause;
        private System.Windows.Forms.Button btResume;
        private System.Windows.Forms.Button btEndBatch;
        private System.Windows.Forms.Button btReset;
    }
}