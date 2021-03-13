namespace JFHub
{
    partial class FormDeviceCellNameManager
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
            this.tvDevs = new System.Windows.Forms.TreeView();
            this.contextMenuDevMgr = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemLoadCfg = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemSaveCfg = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemAddDev = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemAddMotionDaq = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddTrigCtrl = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddLightTrig = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuDev = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemResetDevsModuleCount = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemDelectDev = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemOpenCloseDev = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuModule = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemResetChannelCount = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contextMenuStripEditName = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemEditSave = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemEditCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuDevMgr.SuspendLayout();
            this.contextMenuDev.SuspendLayout();
            this.contextMenuModule.SuspendLayout();
            this.contextMenuStripEditName.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvDevs
            // 
            this.tvDevs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvDevs.Location = new System.Drawing.Point(2, 1);
            this.tvDevs.Name = "tvDevs";
            this.tvDevs.Size = new System.Drawing.Size(233, 545);
            this.tvDevs.TabIndex = 0;
            this.tvDevs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvDevs_MouseDown);
            // 
            // contextMenuDevMgr
            // 
            this.contextMenuDevMgr.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemLoadCfg,
            this.ToolStripMenuItemSaveCfg,
            this.ToolStripMenuItemAddDev});
            this.contextMenuDevMgr.Name = "contextMenuDevMgr";
            this.contextMenuDevMgr.Size = new System.Drawing.Size(125, 70);
            // 
            // ToolStripMenuItemLoadCfg
            // 
            this.ToolStripMenuItemLoadCfg.Name = "ToolStripMenuItemLoadCfg";
            this.ToolStripMenuItemLoadCfg.Size = new System.Drawing.Size(124, 22);
            this.ToolStripMenuItemLoadCfg.Text = "加载配置";
            this.ToolStripMenuItemLoadCfg.Click += new System.EventHandler(this.ToolStripMenuItemLoadCfg_Click);
            // 
            // ToolStripMenuItemSaveCfg
            // 
            this.ToolStripMenuItemSaveCfg.Name = "ToolStripMenuItemSaveCfg";
            this.ToolStripMenuItemSaveCfg.Size = new System.Drawing.Size(124, 22);
            this.ToolStripMenuItemSaveCfg.Text = "保存配置";
            this.ToolStripMenuItemSaveCfg.Click += new System.EventHandler(this.ToolStripMenuItemSaveCfg_Click);
            // 
            // ToolStripMenuItemAddDev
            // 
            this.ToolStripMenuItemAddDev.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemAddMotionDaq,
            this.toolStripMenuItemAddTrigCtrl,
            this.toolStripMenuItemAddLightTrig});
            this.ToolStripMenuItemAddDev.Name = "ToolStripMenuItemAddDev";
            this.ToolStripMenuItemAddDev.Size = new System.Drawing.Size(124, 22);
            this.ToolStripMenuItemAddDev.Text = "添加设备";
            // 
            // ToolStripMenuItemAddMotionDaq
            // 
            this.ToolStripMenuItemAddMotionDaq.Name = "ToolStripMenuItemAddMotionDaq";
            this.ToolStripMenuItemAddMotionDaq.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItemAddMotionDaq.Text = "运动控制器";
            this.ToolStripMenuItemAddMotionDaq.Click += new System.EventHandler(this.ToolStripMenuItemAddMotionDaq_Click);
            // 
            // toolStripMenuItemAddTrigCtrl
            // 
            this.toolStripMenuItemAddTrigCtrl.Name = "toolStripMenuItemAddTrigCtrl";
            this.toolStripMenuItemAddTrigCtrl.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemAddTrigCtrl.Text = "触发控制器";
            this.toolStripMenuItemAddTrigCtrl.Click += new System.EventHandler(this.toolStripMenuItemAddTrigCtrl_Click);
            // 
            // toolStripMenuItemAddLightTrig
            // 
            this.toolStripMenuItemAddLightTrig.Name = "toolStripMenuItemAddLightTrig";
            this.toolStripMenuItemAddLightTrig.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemAddLightTrig.Text = "光源控制器_T";
            this.toolStripMenuItemAddLightTrig.Click += new System.EventHandler(this.toolStripMenuItemAddLightTrig_Click);
            // 
            // contextMenuDev
            // 
            this.contextMenuDev.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemResetDevsModuleCount,
            this.ToolStripMenuItemDelectDev,
            this.ToolStripMenuItemOpenCloseDev});
            this.contextMenuDev.Name = "contextMenuDev";
            this.contextMenuDev.Size = new System.Drawing.Size(149, 70);
            // 
            // ToolStripMenuItemResetDevsModuleCount
            // 
            this.ToolStripMenuItemResetDevsModuleCount.Name = "ToolStripMenuItemResetDevsModuleCount";
            this.ToolStripMenuItemResetDevsModuleCount.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItemResetDevsModuleCount.Text = "修改模块数量";
            this.ToolStripMenuItemResetDevsModuleCount.Click += new System.EventHandler(this.ToolStripMenuItemResetDevsModuleCount_Click);
            // 
            // ToolStripMenuItemDelectDev
            // 
            this.ToolStripMenuItemDelectDev.Name = "ToolStripMenuItemDelectDev";
            this.ToolStripMenuItemDelectDev.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItemDelectDev.Text = "删除";
            this.ToolStripMenuItemDelectDev.Click += new System.EventHandler(this.ToolStripMenuItemDelectDev_Click);
            // 
            // ToolStripMenuItemOpenCloseDev
            // 
            this.ToolStripMenuItemOpenCloseDev.Name = "ToolStripMenuItemOpenCloseDev";
            this.ToolStripMenuItemOpenCloseDev.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItemOpenCloseDev.Text = "打开设备";
            this.ToolStripMenuItemOpenCloseDev.Click += new System.EventHandler(this.ToolStripMenuItemOpenCloseDev_Click);
            // 
            // contextMenuModule
            // 
            this.contextMenuModule.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemResetChannelCount});
            this.contextMenuModule.Name = "contextMenuModule";
            this.contextMenuModule.Size = new System.Drawing.Size(149, 26);
            // 
            // ToolStripMenuItemResetChannelCount
            // 
            this.ToolStripMenuItemResetChannelCount.Name = "ToolStripMenuItemResetChannelCount";
            this.ToolStripMenuItemResetChannelCount.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItemResetChannelCount.Text = "修改通道数量";
            this.ToolStripMenuItemResetChannelCount.Click += new System.EventHandler(this.ToolStripMenuItemResetChannelCount_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Location = new System.Drawing.Point(236, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(639, 545);
            this.panel1.TabIndex = 3;
            // 
            // contextMenuStripEditName
            // 
            this.contextMenuStripEditName.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemEditSave,
            this.ToolStripMenuItemEditCancel});
            this.contextMenuStripEditName.Name = "contextMenuStripEditName";
            this.contextMenuStripEditName.Size = new System.Drawing.Size(125, 48);
            // 
            // ToolStripMenuItemEditSave
            // 
            this.ToolStripMenuItemEditSave.Name = "ToolStripMenuItemEditSave";
            this.ToolStripMenuItemEditSave.Size = new System.Drawing.Size(124, 22);
            this.ToolStripMenuItemEditSave.Text = "编辑名称";
            this.ToolStripMenuItemEditSave.Click += new System.EventHandler(this.ToolStripMenuItemEditSave_Click);
            // 
            // ToolStripMenuItemEditCancel
            // 
            this.ToolStripMenuItemEditCancel.Enabled = false;
            this.ToolStripMenuItemEditCancel.Name = "ToolStripMenuItemEditCancel";
            this.ToolStripMenuItemEditCancel.Size = new System.Drawing.Size(124, 22);
            this.ToolStripMenuItemEditCancel.Text = "取消编辑";
            this.ToolStripMenuItemEditCancel.Click += new System.EventHandler(this.ToolStripMenuItemEditCancel_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormDeviceCellNameManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 548);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tvDevs);
            this.Name = "FormDeviceCellNameManager";
            this.Text = "设备单元命名管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDeviceCellNameManager_FormClosing);
            this.Load += new System.EventHandler(this.FormDeviceNameManager_Load);
            this.VisibleChanged += new System.EventHandler(this.FormDeviceCellNameManager_VisibleChanged);
            this.contextMenuDevMgr.ResumeLayout(false);
            this.contextMenuDev.ResumeLayout(false);
            this.contextMenuModule.ResumeLayout(false);
            this.contextMenuStripEditName.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvDevs;
        private System.Windows.Forms.ContextMenuStrip contextMenuDevMgr;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemLoadCfg;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemSaveCfg;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemAddDev;
        private System.Windows.Forms.ContextMenuStrip contextMenuDev;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemResetDevsModuleCount;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemDelectDev;
        private System.Windows.Forms.ContextMenuStrip contextMenuModule;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemResetChannelCount;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemOpenCloseDev;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEditName;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemEditSave;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemEditCancel;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemAddMotionDaq;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddTrigCtrl;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddLightTrig;
        private System.Windows.Forms.Timer timer1;
    }
}