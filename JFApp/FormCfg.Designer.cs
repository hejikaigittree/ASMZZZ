namespace JFApp
{
    partial class FormCfg
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
            this.tabCtrl = new System.Windows.Forms.TabControl();
            this.tpStationEnableCfg = new System.Windows.Forms.TabPage();
            this.tpStationCfg = new System.Windows.Forms.TabPage();
            this.tpDllMgr = new System.Windows.Forms.TabPage();
            this.tpDevCfg = new System.Windows.Forms.TabPage();
            this.tpDevNameCfg = new System.Windows.Forms.TabPage();
            this.tpStationMgr = new System.Windows.Forms.TabPage();
            this.tpSystemCfg = new System.Windows.Forms.TabPage();
            this.tabCtrl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabCtrl
            // 
            this.tabCtrl.Controls.Add(this.tpStationEnableCfg);
            this.tabCtrl.Controls.Add(this.tpStationCfg);
            this.tabCtrl.Controls.Add(this.tpDllMgr);
            this.tabCtrl.Controls.Add(this.tpDevCfg);
            this.tabCtrl.Controls.Add(this.tpDevNameCfg);
            this.tabCtrl.Controls.Add(this.tpStationMgr);
            this.tabCtrl.Controls.Add(this.tpSystemCfg);
            this.tabCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrl.Location = new System.Drawing.Point(0, 0);
            this.tabCtrl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.Padding = new System.Drawing.Point(10, 3);
            this.tabCtrl.SelectedIndex = 0;
            this.tabCtrl.Size = new System.Drawing.Size(1332, 787);
            this.tabCtrl.TabIndex = 0;
            this.tabCtrl.SelectedIndexChanged += new System.EventHandler(this.tabCtrl_SelectedIndexChanged);
            this.tabCtrl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabCtrl_Selecting);
            // 
            // tpStationEnableCfg
            // 
            this.tpStationEnableCfg.Location = new System.Drawing.Point(4, 30);
            this.tpStationEnableCfg.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpStationEnableCfg.Name = "tpStationEnableCfg";
            this.tpStationEnableCfg.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpStationEnableCfg.Size = new System.Drawing.Size(1324, 753);
            this.tpStationEnableCfg.TabIndex = 0;
            this.tpStationEnableCfg.Text = "工站使能列表";
            this.tpStationEnableCfg.UseVisualStyleBackColor = true;
            // 
            // tpStationCfg
            // 
            this.tpStationCfg.Location = new System.Drawing.Point(4, 30);
            this.tpStationCfg.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpStationCfg.Name = "tpStationCfg";
            this.tpStationCfg.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpStationCfg.Size = new System.Drawing.Size(1324, 753);
            this.tpStationCfg.TabIndex = 6;
            this.tpStationCfg.Text = "工站配置管理";
            this.tpStationCfg.UseVisualStyleBackColor = true;
            // 
            // tpDllMgr
            // 
            this.tpDllMgr.Location = new System.Drawing.Point(4, 30);
            this.tpDllMgr.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpDllMgr.Name = "tpDllMgr";
            this.tpDllMgr.Size = new System.Drawing.Size(1324, 753);
            this.tpDllMgr.TabIndex = 2;
            this.tpDllMgr.Text = "拓展软件库";
            this.tpDllMgr.UseVisualStyleBackColor = true;
            // 
            // tpDevCfg
            // 
            this.tpDevCfg.Location = new System.Drawing.Point(4, 30);
            this.tpDevCfg.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpDevCfg.Name = "tpDevCfg";
            this.tpDevCfg.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpDevCfg.Size = new System.Drawing.Size(1324, 753);
            this.tpDevCfg.TabIndex = 1;
            this.tpDevCfg.Text = "设备管理";
            this.tpDevCfg.UseVisualStyleBackColor = true;
            // 
            // tpDevNameCfg
            // 
            this.tpDevNameCfg.Location = new System.Drawing.Point(4, 30);
            this.tpDevNameCfg.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpDevNameCfg.Name = "tpDevNameCfg";
            this.tpDevNameCfg.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpDevNameCfg.Size = new System.Drawing.Size(1324, 753);
            this.tpDevNameCfg.TabIndex = 3;
            this.tpDevNameCfg.Text = "设备通道命名";
            this.tpDevNameCfg.UseVisualStyleBackColor = true;
            // 
            // tpStationMgr
            // 
            this.tpStationMgr.Location = new System.Drawing.Point(4, 30);
            this.tpStationMgr.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpStationMgr.Name = "tpStationMgr";
            this.tpStationMgr.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpStationMgr.Size = new System.Drawing.Size(1324, 753);
            this.tpStationMgr.TabIndex = 4;
            this.tpStationMgr.Text = "工站管理";
            this.tpStationMgr.UseVisualStyleBackColor = true;
            // 
            // tpSystemCfg
            // 
            this.tpSystemCfg.Location = new System.Drawing.Point(4, 30);
            this.tpSystemCfg.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpSystemCfg.Name = "tpSystemCfg";
            this.tpSystemCfg.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tpSystemCfg.Size = new System.Drawing.Size(1324, 753);
            this.tpSystemCfg.TabIndex = 5;
            this.tpSystemCfg.Text = "系统配置";
            this.tpSystemCfg.UseVisualStyleBackColor = true;
            // 
            // FormCfg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1332, 787);
            this.Controls.Add(this.tabCtrl);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "FormCfg";
            this.Text = "FormCfg";
            this.Load += new System.EventHandler(this.FormCfg_Load);
            this.VisibleChanged += new System.EventHandler(this.FormCfg_VisibleChanged);
            this.tabCtrl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabCtrl;
        private System.Windows.Forms.TabPage tpStationEnableCfg;
        private System.Windows.Forms.TabPage tpDevCfg;
        private System.Windows.Forms.TabPage tpStationCfg;
        private System.Windows.Forms.TabPage tpDllMgr;
        private System.Windows.Forms.TabPage tpDevNameCfg;
        private System.Windows.Forms.TabPage tpStationMgr;
        private System.Windows.Forms.TabPage tpSystemCfg;
    }
}