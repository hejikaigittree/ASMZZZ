namespace JFHub
{
    partial class FormRecipeMgr
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
            this.toolStripMenuItemSetMgr = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemReset = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemInitMgr = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemInitParam = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDialog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTbTips = new System.Windows.Forms.ToolStripTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSetMgr,
            this.toolStripTbTips});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItemSetMgr
            // 
            this.toolStripMenuItemSetMgr.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemReset,
            this.toolStripMenuItemInitMgr,
            this.toolStripMenuItemInitParam,
            this.toolStripMenuItemDialog});
            this.toolStripMenuItemSetMgr.Name = "toolStripMenuItemSetMgr";
            this.toolStripMenuItemSetMgr.Size = new System.Drawing.Size(104, 23);
            this.toolStripMenuItemSetMgr.Text = "配方管理器设置";
            // 
            // toolStripMenuItemReset
            // 
            this.toolStripMenuItemReset.Name = "toolStripMenuItemReset";
            this.toolStripMenuItemReset.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItemReset.Text = "新建/重置配方管理器";
            this.toolStripMenuItemReset.Click += new System.EventHandler(this.toolStripMenuItemReset_Click);
            // 
            // toolStripMenuItemInitMgr
            // 
            this.toolStripMenuItemInitMgr.Name = "toolStripMenuItemInitMgr";
            this.toolStripMenuItemInitMgr.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItemInitMgr.Text = "管理器初始化";
            this.toolStripMenuItemInitMgr.Click += new System.EventHandler(this.toolStripMenuItemInitMgr_Click);
            // 
            // toolStripMenuItemInitParam
            // 
            this.toolStripMenuItemInitParam.Name = "toolStripMenuItemInitParam";
            this.toolStripMenuItemInitParam.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItemInitParam.Text = "管理器初始参数";
            this.toolStripMenuItemInitParam.Click += new System.EventHandler(this.toolStripMenuItemInitParam_Click);
            // 
            // toolStripMenuItemDialog
            // 
            this.toolStripMenuItemDialog.Name = "toolStripMenuItemDialog";
            this.toolStripMenuItemDialog.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItemDialog.Text = "管理器视窗";
            this.toolStripMenuItemDialog.Click += new System.EventHandler(this.toolStripMenuItemDialog_Click);
            // 
            // toolStripTbTips
            // 
            this.toolStripTbTips.Name = "toolStripTbTips";
            this.toolStripTbTips.ReadOnly = true;
            this.toolStripTbTips.Size = new System.Drawing.Size(300, 23);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(0, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 421);
            this.panel1.TabIndex = 1;
            // 
            // FormRecipeMgr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormRecipeMgr";
            this.Text = "产品配方管理";
            this.Load += new System.EventHandler(this.FormRecipeMgr_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetMgr;
        private System.Windows.Forms.ToolStripTextBox toolStripTbTips;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemReset;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInitMgr;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInitParam;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDialog;
        private System.Windows.Forms.Panel panel1;
    }
}