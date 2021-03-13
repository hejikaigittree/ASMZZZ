namespace JFHub
{
    partial class FormStationBaseAxisPanel
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelAxisOpt = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btOpenAllDev = new System.Windows.Forms.Button();
            this.cbSaveWorkPos = new System.Windows.Forms.ComboBox();
            this.cbMoveWorkPos = new System.Windows.Forms.ComboBox();
            this.btSaveCurr2WorkPos = new System.Windows.Forms.Button();
            this.btMoveToWorkPos = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panelAxisStatus = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btClearTips = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.rchTips = new System.Windows.Forms.RichTextBox();
            this.timerFlush = new System.Windows.Forms.Timer(this.components);
            this.chkLine = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelAxisOpt);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(881, 450);
            this.splitContainer1.SplitterDistance = 390;
            this.splitContainer1.TabIndex = 0;
            // 
            // panelAxisOpt
            // 
            this.panelAxisOpt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelAxisOpt.AutoScroll = true;
            this.panelAxisOpt.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panelAxisOpt.Location = new System.Drawing.Point(2, 1);
            this.panelAxisOpt.Name = "panelAxisOpt";
            this.panelAxisOpt.Size = new System.Drawing.Size(386, 332);
            this.panelAxisOpt.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel1.Controls.Add(this.chkLine);
            this.panel1.Controls.Add(this.btOpenAllDev);
            this.panel1.Controls.Add(this.cbSaveWorkPos);
            this.panel1.Controls.Add(this.cbMoveWorkPos);
            this.panel1.Controls.Add(this.btSaveCurr2WorkPos);
            this.panel1.Controls.Add(this.btMoveToWorkPos);
            this.panel1.Location = new System.Drawing.Point(2, 337);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(386, 110);
            this.panel1.TabIndex = 0;
            // 
            // btOpenAllDev
            // 
            this.btOpenAllDev.Location = new System.Drawing.Point(9, 4);
            this.btOpenAllDev.Name = "btOpenAllDev";
            this.btOpenAllDev.Size = new System.Drawing.Size(114, 23);
            this.btOpenAllDev.TabIndex = 4;
            this.btOpenAllDev.Text = "打开所有轴设备";
            this.btOpenAllDev.UseVisualStyleBackColor = true;
            this.btOpenAllDev.Click += new System.EventHandler(this.btOpenAllDev_Click);
            // 
            // cbSaveWorkPos
            // 
            this.cbSaveWorkPos.FormattingEnabled = true;
            this.cbSaveWorkPos.Location = new System.Drawing.Point(119, 83);
            this.cbSaveWorkPos.Name = "cbSaveWorkPos";
            this.cbSaveWorkPos.Size = new System.Drawing.Size(213, 20);
            this.cbSaveWorkPos.TabIndex = 3;
            // 
            // cbMoveWorkPos
            // 
            this.cbMoveWorkPos.FormattingEnabled = true;
            this.cbMoveWorkPos.Location = new System.Drawing.Point(119, 54);
            this.cbMoveWorkPos.Name = "cbMoveWorkPos";
            this.cbMoveWorkPos.Size = new System.Drawing.Size(213, 20);
            this.cbMoveWorkPos.TabIndex = 2;
            // 
            // btSaveCurr2WorkPos
            // 
            this.btSaveCurr2WorkPos.Location = new System.Drawing.Point(2, 82);
            this.btSaveCurr2WorkPos.Name = "btSaveCurr2WorkPos";
            this.btSaveCurr2WorkPos.Size = new System.Drawing.Size(114, 23);
            this.btSaveCurr2WorkPos.TabIndex = 1;
            this.btSaveCurr2WorkPos.Text = "当前位置保存为";
            this.btSaveCurr2WorkPos.UseVisualStyleBackColor = true;
            this.btSaveCurr2WorkPos.Click += new System.EventHandler(this.btSaveCurr2WorkPos_Click);
            // 
            // btMoveToWorkPos
            // 
            this.btMoveToWorkPos.Location = new System.Drawing.Point(2, 53);
            this.btMoveToWorkPos.Name = "btMoveToWorkPos";
            this.btMoveToWorkPos.Size = new System.Drawing.Size(114, 23);
            this.btMoveToWorkPos.TabIndex = 0;
            this.btMoveToWorkPos.Text = "移动到指定位置";
            this.btMoveToWorkPos.UseVisualStyleBackColor = true;
            this.btMoveToWorkPos.Click += new System.EventHandler(this.btMoveToWorkPos_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panelAxisStatus);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel2);
            this.splitContainer2.Size = new System.Drawing.Size(487, 450);
            this.splitContainer2.SplitterDistance = 333;
            this.splitContainer2.TabIndex = 0;
            // 
            // panelAxisStatus
            // 
            this.panelAxisStatus.AutoScroll = true;
            this.panelAxisStatus.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panelAxisStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAxisStatus.Location = new System.Drawing.Point(0, 0);
            this.panelAxisStatus.Name = "panelAxisStatus";
            this.panelAxisStatus.Size = new System.Drawing.Size(487, 333);
            this.panelAxisStatus.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel2.Controls.Add(this.btClearTips);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.rchTips);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(487, 113);
            this.panel2.TabIndex = 1;
            // 
            // btClearTips
            // 
            this.btClearTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btClearTips.Location = new System.Drawing.Point(442, -1);
            this.btClearTips.Name = "btClearTips";
            this.btClearTips.Size = new System.Drawing.Size(44, 21);
            this.btClearTips.TabIndex = 2;
            this.btClearTips.Text = "清空";
            this.btClearTips.UseVisualStyleBackColor = true;
            this.btClearTips.Click += new System.EventHandler(this.btClearTips_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "信息:";
            // 
            // rchTips
            // 
            this.rchTips.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rchTips.BackColor = System.Drawing.SystemColors.Control;
            this.rchTips.Location = new System.Drawing.Point(0, 19);
            this.rchTips.Name = "rchTips";
            this.rchTips.Size = new System.Drawing.Size(487, 91);
            this.rchTips.TabIndex = 0;
            this.rchTips.Text = "";
            // 
            // timerFlush
            // 
            this.timerFlush.Tick += new System.EventHandler(this.timerFlush_Tick);
            // 
            // chkLine
            // 
            this.chkLine.AutoSize = true;
            this.chkLine.Location = new System.Drawing.Point(336, 56);
            this.chkLine.Name = "chkLine";
            this.chkLine.Size = new System.Drawing.Size(48, 16);
            this.chkLine.TabIndex = 5;
            this.chkLine.Text = "插补";
            this.chkLine.UseVisualStyleBackColor = true;
            // 
            // FormStationBaseAxisPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormStationBaseAxisPanel";
            this.Text = "工站:轴面板";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormStationBaseAxisPanel_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormStationBaseAxisPanel_FormClosed);
            this.Load += new System.EventHandler(this.FormAxisInStationBase_Load);
            this.VisibleChanged += new System.EventHandler(this.FormStationBaseAxisPanel_VisibleChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelAxisOpt;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panelAxisStatus;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cbSaveWorkPos;
        private System.Windows.Forms.ComboBox cbMoveWorkPos;
        private System.Windows.Forms.Button btSaveCurr2WorkPos;
        private System.Windows.Forms.Button btMoveToWorkPos;
        private System.Windows.Forms.Button btClearTips;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rchTips;
        private System.Windows.Forms.Timer timerFlush;
        private System.Windows.Forms.Button btOpenAllDev;
        private System.Windows.Forms.CheckBox chkLine;
    }
}