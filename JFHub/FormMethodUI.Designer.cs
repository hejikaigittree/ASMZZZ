namespace JFHub
{
    partial class FormMethodUI
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpRealtime = new System.Windows.Forms.TabPage();
            this.tpInitCfg = new System.Windows.Forms.TabPage();
            this.timerFlush = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_Action = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkAutoFlush = new System.Windows.Forms.CheckBox();
            this.btUpdateRealtimeUI = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpRealtime);
            this.tabControl1.Controls.Add(this.tpInitCfg);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(734, 512);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseDown);
            // 
            // tpRealtime
            // 
            this.tpRealtime.AutoScroll = true;
            this.tpRealtime.Location = new System.Drawing.Point(4, 22);
            this.tpRealtime.Name = "tpRealtime";
            this.tpRealtime.Padding = new System.Windows.Forms.Padding(3);
            this.tpRealtime.Size = new System.Drawing.Size(726, 486);
            this.tpRealtime.TabIndex = 0;
            this.tpRealtime.Text = "状态显示/实时调试";
            this.tpRealtime.UseVisualStyleBackColor = true;
            // 
            // tpInitCfg
            // 
            this.tpInitCfg.AutoScroll = true;
            this.tpInitCfg.Location = new System.Drawing.Point(4, 22);
            this.tpInitCfg.Name = "tpInitCfg";
            this.tpInitCfg.Padding = new System.Windows.Forms.Padding(3);
            this.tpInitCfg.Size = new System.Drawing.Size(726, 486);
            this.tpInitCfg.TabIndex = 1;
            this.tpInitCfg.Text = "初始化参数/配置";
            this.tpInitCfg.UseVisualStyleBackColor = true;
            // 
            // timerFlush
            // 
            this.timerFlush.Tick += new System.EventHandler(this.timerFlush_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Action});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 26);
            // 
            // ToolStripMenuItem_Action
            // 
            this.ToolStripMenuItem_Action.Name = "ToolStripMenuItem_Action";
            this.ToolStripMenuItem_Action.Size = new System.Drawing.Size(100, 22);
            this.ToolStripMenuItem_Action.Text = "执行";
            this.ToolStripMenuItem_Action.Click += new System.EventHandler(this.ToolStripMenuItem_Action_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chkAutoFlush);
            this.tabPage1.Controls.Add(this.btUpdateRealtimeUI);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(726, 486);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "刷新设置";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chkAutoFlush
            // 
            this.chkAutoFlush.AutoSize = true;
            this.chkAutoFlush.Checked = true;
            this.chkAutoFlush.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoFlush.Location = new System.Drawing.Point(291, 221);
            this.chkAutoFlush.Name = "chkAutoFlush";
            this.chkAutoFlush.Size = new System.Drawing.Size(144, 16);
            this.chkAutoFlush.TabIndex = 5;
            this.chkAutoFlush.Text = "自动刷新Realtime界面";
            this.chkAutoFlush.UseVisualStyleBackColor = true;
            this.chkAutoFlush.CheckedChanged += new System.EventHandler(this.chkAutoFlush_CheckedChanged_1);
            // 
            // btUpdateRealtimeUI
            // 
            this.btUpdateRealtimeUI.Location = new System.Drawing.Point(291, 243);
            this.btUpdateRealtimeUI.Name = "btUpdateRealtimeUI";
            this.btUpdateRealtimeUI.Size = new System.Drawing.Size(144, 23);
            this.btUpdateRealtimeUI.TabIndex = 4;
            this.btUpdateRealtimeUI.Text = "手动刷新Realtime界面";
            this.btUpdateRealtimeUI.UseVisualStyleBackColor = true;
            this.btUpdateRealtimeUI.Click += new System.EventHandler(this.btUpdateRealtimeUI_Click);
            // 
            // FormMethodUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 512);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormMethodUI";
            this.Text = "FormMethodUI";
            this.Load += new System.EventHandler(this.FormMethodUI_Load);
            this.tabControl1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpRealtime;
        private System.Windows.Forms.TabPage tpInitCfg;
        private System.Windows.Forms.Timer timerFlush;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Action;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox chkAutoFlush;
        private System.Windows.Forms.Button btUpdateRealtimeUI;
    }
}