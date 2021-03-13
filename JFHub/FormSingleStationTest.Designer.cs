namespace JFHub
{
    partial class FormSingleStationTest
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.运行模式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAuto = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemManual = new System.Windows.Forms.ToolStripMenuItem();
            this.调试运行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemReset = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPause = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemResume = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEndBatch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCfg = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBaseConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBaseTest = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDataPool = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDebugInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStationName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripTextBoxWorkStatus = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripTextBoxCustomStatus = new System.Windows.Forms.ToolStripTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.rchTips = new System.Windows.Forms.RichTextBox();
            this.btClearTips = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.运行模式ToolStripMenuItem,
            this.调试运行ToolStripMenuItem,
            this.toolStripMenuItemCfg,
            this.toolStripStationName,
            this.toolStripTextBoxWorkStatus,
            this.toolStripTextBoxCustomStatus});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 27);
            this.menuStrip1.TabIndex = 0;
            // 
            // 运行模式ToolStripMenuItem
            // 
            this.运行模式ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAuto,
            this.toolStripMenuItemManual});
            this.运行模式ToolStripMenuItem.Name = "运行模式ToolStripMenuItem";
            this.运行模式ToolStripMenuItem.Size = new System.Drawing.Size(68, 23);
            this.运行模式ToolStripMenuItem.Text = "运行模式";
            // 
            // toolStripMenuItemAuto
            // 
            this.toolStripMenuItemAuto.Name = "toolStripMenuItemAuto";
            this.toolStripMenuItemAuto.Size = new System.Drawing.Size(129, 22);
            this.toolStripMenuItemAuto.Text = "自动/连续";
            this.toolStripMenuItemAuto.Click += new System.EventHandler(this.toolStripMenuItemAuto_Click);
            // 
            // toolStripMenuItemManual
            // 
            this.toolStripMenuItemManual.Name = "toolStripMenuItemManual";
            this.toolStripMenuItemManual.Size = new System.Drawing.Size(129, 22);
            this.toolStripMenuItemManual.Text = "手动/调试";
            this.toolStripMenuItemManual.Click += new System.EventHandler(this.toolStripMenuItemManual_Click);
            // 
            // 调试运行ToolStripMenuItem
            // 
            this.调试运行ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemStop,
            this.toolStripMenuItemStart,
            this.toolStripMenuItemReset,
            this.toolStripMenuItemPause,
            this.toolStripMenuItemResume,
            this.toolStripMenuItemEndBatch});
            this.调试运行ToolStripMenuItem.Name = "调试运行ToolStripMenuItem";
            this.调试运行ToolStripMenuItem.Size = new System.Drawing.Size(73, 23);
            this.调试运行ToolStripMenuItem.Text = "调试/运行";
            // 
            // toolStripMenuItemStop
            // 
            this.toolStripMenuItemStop.Name = "toolStripMenuItemStop";
            this.toolStripMenuItemStop.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemStop.Text = "停止";
            this.toolStripMenuItemStop.Click += new System.EventHandler(this.toolStripMenuItemStop_Click);
            // 
            // toolStripMenuItemStart
            // 
            this.toolStripMenuItemStart.Name = "toolStripMenuItemStart";
            this.toolStripMenuItemStart.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemStart.Text = "开始";
            this.toolStripMenuItemStart.Click += new System.EventHandler(this.toolStripMenuItemStart_Click);
            // 
            // toolStripMenuItemReset
            // 
            this.toolStripMenuItemReset.Name = "toolStripMenuItemReset";
            this.toolStripMenuItemReset.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemReset.Text = "归零";
            this.toolStripMenuItemReset.Click += new System.EventHandler(this.toolStripMenuItemReset_Click);
            // 
            // toolStripMenuItemPause
            // 
            this.toolStripMenuItemPause.Name = "toolStripMenuItemPause";
            this.toolStripMenuItemPause.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemPause.Text = "暂停";
            this.toolStripMenuItemPause.Click += new System.EventHandler(this.toolStripMenuItemPause_Click);
            // 
            // toolStripMenuItemResume
            // 
            this.toolStripMenuItemResume.Name = "toolStripMenuItemResume";
            this.toolStripMenuItemResume.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemResume.Text = "恢复";
            this.toolStripMenuItemResume.Click += new System.EventHandler(this.toolStripMenuItemResume_Click);
            // 
            // toolStripMenuItemEndBatch
            // 
            this.toolStripMenuItemEndBatch.Name = "toolStripMenuItemEndBatch";
            this.toolStripMenuItemEndBatch.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemEndBatch.Text = "结批";
            this.toolStripMenuItemEndBatch.Click += new System.EventHandler(this.toolStripMenuItemEndBatch_Click);
            // 
            // toolStripMenuItemCfg
            // 
            this.toolStripMenuItemCfg.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemBaseConfig,
            this.toolStripMenuItemBaseTest,
            this.toolStripMenuItemDataPool,
            this.toolStripMenuItemDebugInfo});
            this.toolStripMenuItemCfg.Name = "toolStripMenuItemCfg";
            this.toolStripMenuItemCfg.Size = new System.Drawing.Size(73, 23);
            this.toolStripMenuItemCfg.Text = "配置/数据";
            this.toolStripMenuItemCfg.Click += new System.EventHandler(this.toolStripMenuItemCfg_Click);
            // 
            // toolStripMenuItemBaseConfig
            // 
            this.toolStripMenuItemBaseConfig.Name = "toolStripMenuItemBaseConfig";
            this.toolStripMenuItemBaseConfig.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItemBaseConfig.Text = "Base Config";
            this.toolStripMenuItemBaseConfig.Click += new System.EventHandler(this.baseConfigToolStripMenuItem_Click);
            // 
            // toolStripMenuItemBaseTest
            // 
            this.toolStripMenuItemBaseTest.Name = "toolStripMenuItemBaseTest";
            this.toolStripMenuItemBaseTest.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItemBaseTest.Text = "Base Test";
            this.toolStripMenuItemBaseTest.Click += new System.EventHandler(this.baseTestToolStripMenuItem_Click);
            // 
            // toolStripMenuItemDataPool
            // 
            this.toolStripMenuItemDataPool.Name = "toolStripMenuItemDataPool";
            this.toolStripMenuItemDataPool.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItemDataPool.Text = "Data Pool";
            this.toolStripMenuItemDataPool.Click += new System.EventHandler(this.toolStripMenuItemDataPool_Click);
            // 
            // toolStripMenuItemDebugInfo
            // 
            this.toolStripMenuItemDebugInfo.Name = "toolStripMenuItemDebugInfo";
            this.toolStripMenuItemDebugInfo.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItemDebugInfo.Text = "Debug Info";
            this.toolStripMenuItemDebugInfo.Click += new System.EventHandler(this.toolStripMenuItemDebugInfo_Click);
            // 
            // toolStripStationName
            // 
            this.toolStripStationName.Name = "toolStripStationName";
            this.toolStripStationName.ReadOnly = true;
            this.toolStripStationName.Size = new System.Drawing.Size(200, 23);
            this.toolStripStationName.Text = "工站:未设置";
            // 
            // toolStripTextBoxWorkStatus
            // 
            this.toolStripTextBoxWorkStatus.Name = "toolStripTextBoxWorkStatus";
            this.toolStripTextBoxWorkStatus.ReadOnly = true;
            this.toolStripTextBoxWorkStatus.Size = new System.Drawing.Size(130, 23);
            this.toolStripTextBoxWorkStatus.Text = "运行状态:未运行";
            // 
            // toolStripTextBoxCustomStatus
            // 
            this.toolStripTextBoxCustomStatus.Name = "toolStripTextBoxCustomStatus";
            this.toolStripTextBoxCustomStatus.ReadOnly = true;
            this.toolStripTextBoxCustomStatus.Size = new System.Drawing.Size(200, 23);
            this.toolStripTextBoxCustomStatus.Text = "任务状态:已停止";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(800, 423);
            this.splitContainer1.SplitterDistance = 25;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.rchTips);
            this.splitContainer2.Panel2.Controls.Add(this.btClearTips);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2MinSize = 0;
            this.splitContainer2.Size = new System.Drawing.Size(771, 423);
            this.splitContainer2.SplitterDistance = 265;
            this.splitContainer2.TabIndex = 0;
            // 
            // rchTips
            // 
            this.rchTips.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rchTips.Location = new System.Drawing.Point(2, 21);
            this.rchTips.Name = "rchTips";
            this.rchTips.ReadOnly = true;
            this.rchTips.Size = new System.Drawing.Size(765, 129);
            this.rchTips.TabIndex = 2;
            this.rchTips.Text = "";
            // 
            // btClearTips
            // 
            this.btClearTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btClearTips.Location = new System.Drawing.Point(693, 1);
            this.btClearTips.Name = "btClearTips";
            this.btClearTips.Size = new System.Drawing.Size(75, 20);
            this.btClearTips.TabIndex = 1;
            this.btClearTips.Text = "清空";
            this.btClearTips.UseVisualStyleBackColor = true;
            this.btClearTips.Click += new System.EventHandler(this.btClearTips_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "调试信息";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormSingleStationTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormSingleStationTest";
            this.Text = "单工站测试";
            this.Load += new System.EventHandler(this.FormSingleStationTest_Load);
            this.VisibleChanged += new System.EventHandler(this.FormSingleStationTest_VisibleChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 运行模式ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAuto;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemManual;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCfg;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripTextBox toolStripStationName;
        private System.Windows.Forms.ToolStripMenuItem 调试运行ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStart;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStop;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBaseConfig;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBaseTest;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxWorkStatus;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxCustomStatus;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox rchTips;
        private System.Windows.Forms.Button btClearTips;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemReset;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPause;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemResume;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDataPool;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDebugInfo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEndBatch;
        private System.Windows.Forms.Timer timer1;
    }
}