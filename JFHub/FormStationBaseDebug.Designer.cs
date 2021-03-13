namespace JFHub
{
    partial class FormStationBaseDebug
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
            this.显示模式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAxis = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDio = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAio = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCmpTrig = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCmr = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemWorkFlow = new System.Windows.Forms.ToolStripMenuItem();
            this.显示模式ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewModeMDI = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewModeTab = new System.Windows.Forms.ToolStripMenuItem();
            this.CfgParamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.AllowMerge = false;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.显示模式ToolStripMenuItem,
            this.显示模式ToolStripMenuItem1,
            this.CfgParamToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(845, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 显示模式ToolStripMenuItem
            // 
            this.显示模式ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAxis,
            this.menuItemDio,
            this.menuItemAio,
            this.menuItemCmpTrig,
            this.menuItemLight,
            this.menuItemCmr,
            this.menuItemWorkFlow});
            this.显示模式ToolStripMenuItem.Name = "显示模式ToolStripMenuItem";
            this.显示模式ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.显示模式ToolStripMenuItem.Text = "功能模块";
            // 
            // menuItemAxis
            // 
            this.menuItemAxis.Name = "menuItemAxis";
            this.menuItemAxis.Size = new System.Drawing.Size(180, 22);
            this.menuItemAxis.Text = "轴/电机";
            this.menuItemAxis.Click += new System.EventHandler(this.menuItemAxis_Click);
            // 
            // menuItemDio
            // 
            this.menuItemDio.Name = "menuItemDio";
            this.menuItemDio.Size = new System.Drawing.Size(180, 22);
            this.menuItemDio.Text = "DI/DO";
            this.menuItemDio.Click += new System.EventHandler(this.menuItemDIO_Click);
            // 
            // menuItemAio
            // 
            this.menuItemAio.Name = "menuItemAio";
            this.menuItemAio.Size = new System.Drawing.Size(180, 22);
            this.menuItemAio.Text = "AI/AO";
            this.menuItemAio.Click += new System.EventHandler(this.menuItemAIO_Click);
            // 
            // menuItemCmpTrig
            // 
            this.menuItemCmpTrig.Name = "menuItemCmpTrig";
            this.menuItemCmpTrig.Size = new System.Drawing.Size(180, 22);
            this.menuItemCmpTrig.Text = "位置比较触发器";
            this.menuItemCmpTrig.Click += new System.EventHandler(this.menuItemCmpTrig_Click);
            // 
            // menuItemLight
            // 
            this.menuItemLight.Name = "menuItemLight";
            this.menuItemLight.Size = new System.Drawing.Size(180, 22);
            this.menuItemLight.Text = "光源控制器";
            this.menuItemLight.Click += new System.EventHandler(this.menuItemLightCtrl_Click);
            // 
            // menuItemCmr
            // 
            this.menuItemCmr.Name = "menuItemCmr";
            this.menuItemCmr.Size = new System.Drawing.Size(180, 22);
            this.menuItemCmr.Text = "相机设备";
            this.menuItemCmr.Click += new System.EventHandler(this.menuItemCmr_Click);
            // 
            // menuItemWorkFlow
            // 
            this.menuItemWorkFlow.Name = "menuItemWorkFlow";
            this.menuItemWorkFlow.Size = new System.Drawing.Size(180, 22);
            this.menuItemWorkFlow.Text = "动作流";
            this.menuItemWorkFlow.Click += new System.EventHandler(this.menuItemWorkFlow_Click);
            // 
            // 显示模式ToolStripMenuItem1
            // 
            this.显示模式ToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemViewModeMDI,
            this.menuItemViewModeTab});
            this.显示模式ToolStripMenuItem1.Name = "显示模式ToolStripMenuItem1";
            this.显示模式ToolStripMenuItem1.Size = new System.Drawing.Size(68, 21);
            this.显示模式ToolStripMenuItem1.Text = "显示模式";
            // 
            // menuItemViewModeMDI
            // 
            this.menuItemViewModeMDI.Checked = true;
            this.menuItemViewModeMDI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItemViewModeMDI.Name = "menuItemViewModeMDI";
            this.menuItemViewModeMDI.Size = new System.Drawing.Size(101, 22);
            this.menuItemViewModeMDI.Text = "MDI";
            this.menuItemViewModeMDI.Click += new System.EventHandler(this.menuItemViewModeMDI_Click);
            // 
            // menuItemViewModeTab
            // 
            this.menuItemViewModeTab.Enabled = false;
            this.menuItemViewModeTab.Name = "menuItemViewModeTab";
            this.menuItemViewModeTab.Size = new System.Drawing.Size(101, 22);
            this.menuItemViewModeTab.Text = "TAB";
            // 
            // CfgParamToolStripMenuItem
            // 
            this.CfgParamToolStripMenuItem.Name = "CfgParamToolStripMenuItem";
            this.CfgParamToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.CfgParamToolStripMenuItem.Text = "配置参数";
            this.CfgParamToolStripMenuItem.Click += new System.EventHandler(this.CfgParamToolStripMenuItem_Click);
            // 
            // FormStationBaseDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(845, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormStationBaseDebug";
            this.Text = "FormStationBase";
            this.Load += new System.EventHandler(this.FormStationBase_Load);
            this.VisibleChanged += new System.EventHandler(this.FormStationBase_VisibleChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 显示模式ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemAxis;
        private System.Windows.Forms.ToolStripMenuItem menuItemDio;
        private System.Windows.Forms.ToolStripMenuItem menuItemCmr;
        private System.Windows.Forms.ToolStripMenuItem menuItemLight;
        private System.Windows.Forms.ToolStripMenuItem menuItemCmpTrig;
        private System.Windows.Forms.ToolStripMenuItem 显示模式ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewModeMDI;
        private System.Windows.Forms.ToolStripMenuItem menuItemWorkFlow;
        private System.Windows.Forms.ToolStripMenuItem menuItemAio;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewModeTab;
        private System.Windows.Forms.ToolStripMenuItem CfgParamToolStripMenuItem;
    }
}