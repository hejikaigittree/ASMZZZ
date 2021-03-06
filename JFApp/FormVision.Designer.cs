namespace JFApp
{
    partial class FormVision
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
            this.tabControlCF1 = new JFUI.TabControlCF();
            this.tpSingleVisionTeach = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.示教助手ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSelectSingleVisionAssist = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddSingleVisionAssist = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDelSingleVisionAssist = new System.Windows.Forms.ToolStripMenuItem();
            this.tpSingleCmrCalib = new System.Windows.Forms.TabPage();
            this.tabControlCF1.SuspendLayout();
            this.tpSingleVisionTeach.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlCF1
            // 
            this.tabControlCF1.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabControlCF1.Controls.Add(this.tpSingleVisionTeach);
            this.tabControlCF1.Controls.Add(this.tpSingleCmrCalib);
            this.tabControlCF1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlCF1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlCF1.ItemSize = new System.Drawing.Size(35, 100);
            this.tabControlCF1.Location = new System.Drawing.Point(0, 0);
            this.tabControlCF1.Multiline = true;
            this.tabControlCF1.Name = "tabControlCF1";
            this.tabControlCF1.SelectedIndex = 0;
            this.tabControlCF1.Size = new System.Drawing.Size(911, 508);
            this.tabControlCF1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlCF1.TabColor = System.Drawing.SystemColors.ControlDark;
            this.tabControlCF1.TabIndex = 0;
            this.tabControlCF1.SelectedIndexChanged += new System.EventHandler(this.tabControlCF1_SelectedIndexChanged);
            // 
            // tpSingleVisionTeach
            // 
            this.tpSingleVisionTeach.AutoScroll = true;
            this.tpSingleVisionTeach.Controls.Add(this.menuStrip1);
            this.tpSingleVisionTeach.Location = new System.Drawing.Point(4, 4);
            this.tpSingleVisionTeach.Name = "tpSingleVisionTeach";
            this.tpSingleVisionTeach.Padding = new System.Windows.Forms.Padding(3);
            this.tpSingleVisionTeach.Size = new System.Drawing.Size(803, 500);
            this.tpSingleVisionTeach.TabIndex = 0;
            this.tpSingleVisionTeach.Text = "单相机示教";
            this.tpSingleVisionTeach.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.示教助手ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(3, 3);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(797, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 示教助手ToolStripMenuItem
            // 
            this.示教助手ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSelectSingleVisionAssist,
            this.toolStripMenuItemAddSingleVisionAssist,
            this.toolStripMenuItemDelSingleVisionAssist});
            this.示教助手ToolStripMenuItem.Name = "示教助手ToolStripMenuItem";
            this.示教助手ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.示教助手ToolStripMenuItem.Text = "示教助手";
            // 
            // toolStripMenuItemSelectSingleVisionAssist
            // 
            this.toolStripMenuItemSelectSingleVisionAssist.Name = "toolStripMenuItemSelectSingleVisionAssist";
            this.toolStripMenuItemSelectSingleVisionAssist.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemSelectSingleVisionAssist.Text = "选择";
            // 
            // toolStripMenuItemAddSingleVisionAssist
            // 
            this.toolStripMenuItemAddSingleVisionAssist.Name = "toolStripMenuItemAddSingleVisionAssist";
            this.toolStripMenuItemAddSingleVisionAssist.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemAddSingleVisionAssist.Text = "添加";
            this.toolStripMenuItemAddSingleVisionAssist.Click += new System.EventHandler(this.toolStripMenuItemAddSingleVisionAssist_Click);
            // 
            // toolStripMenuItemDelSingleVisionAssist
            // 
            this.toolStripMenuItemDelSingleVisionAssist.Name = "toolStripMenuItemDelSingleVisionAssist";
            this.toolStripMenuItemDelSingleVisionAssist.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemDelSingleVisionAssist.Text = "删除";
            this.toolStripMenuItemDelSingleVisionAssist.Click += new System.EventHandler(this.toolStripMenuItemDelSingleVisionAssist_Click);
            // 
            // tpSingleCmrCalib
            // 
            this.tpSingleCmrCalib.Location = new System.Drawing.Point(4, 4);
            this.tpSingleCmrCalib.Name = "tpSingleCmrCalib";
            this.tpSingleCmrCalib.Padding = new System.Windows.Forms.Padding(3);
            this.tpSingleCmrCalib.Size = new System.Drawing.Size(803, 500);
            this.tpSingleCmrCalib.TabIndex = 1;
            this.tpSingleCmrCalib.Text = "单相机标定";
            this.tpSingleCmrCalib.UseVisualStyleBackColor = true;
            // 
            // FormVision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 508);
            this.Controls.Add(this.tabControlCF1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormVision";
            this.Text = "FormVision";
            this.Load += new System.EventHandler(this.FormVision_Load);
            this.VisibleChanged += new System.EventHandler(this.FormVision_VisibleChanged);
            this.tabControlCF1.ResumeLayout(false);
            this.tpSingleVisionTeach.ResumeLayout(false);
            this.tpSingleVisionTeach.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private JFUI.TabControlCF tabControlCF1;
        private System.Windows.Forms.TabPage tpSingleVisionTeach;
        private System.Windows.Forms.TabPage tpSingleCmrCalib;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 示教助手ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddSingleVisionAssist;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelSingleVisionAssist;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectSingleVisionAssist;
    }
}