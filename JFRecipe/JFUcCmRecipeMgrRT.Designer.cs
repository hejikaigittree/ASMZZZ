namespace JFRecipe
{
    partial class JFUcCmRecipeMgrRT
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemRecipeMgr = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddRecipe = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDelRecipe = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDelCategoty = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripCbCategoty = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripCbRecipeIDs = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripMenuItemEditSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEditCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemRecipeMgr,
            this.toolStripTextBox2,
            this.toolStripCbCategoty,
            this.toolStripTextBox1,
            this.toolStripCbRecipeIDs,
            this.toolStripMenuItemEditSave,
            this.toolStripMenuItemEditCancel});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(146, 444);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItemRecipeMgr
            // 
            this.toolStripMenuItemRecipeMgr.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddRecipe,
            this.toolStripMenuItemDelRecipe,
            this.toolStripMenuItemDelCategoty});
            this.toolStripMenuItemRecipeMgr.Name = "toolStripMenuItemRecipeMgr";
            this.toolStripMenuItemRecipeMgr.Size = new System.Drawing.Size(133, 21);
            this.toolStripMenuItemRecipeMgr.Text = "Recipe/Categoty管理";
            // 
            // toolStripMenuItemAddRecipe
            // 
            this.toolStripMenuItemAddRecipe.Name = "toolStripMenuItemAddRecipe";
            this.toolStripMenuItemAddRecipe.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemAddRecipe.Text = "新增Recipe";
            this.toolStripMenuItemAddRecipe.Click += new System.EventHandler(this.toolStripMenuItemAddRecipe_Click);
            // 
            // toolStripMenuItemDelRecipe
            // 
            this.toolStripMenuItemDelRecipe.Name = "toolStripMenuItemDelRecipe";
            this.toolStripMenuItemDelRecipe.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemDelRecipe.Text = "删除Recipe";
            this.toolStripMenuItemDelRecipe.Click += new System.EventHandler(this.toolStripMenuItemDelRecipe_Click);
            // 
            // toolStripMenuItemDelCategoty
            // 
            this.toolStripMenuItemDelCategoty.Name = "toolStripMenuItemDelCategoty";
            this.toolStripMenuItemDelCategoty.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemDelCategoty.Text = "删除Categoty";
            this.toolStripMenuItemDelCategoty.Click += new System.EventHandler(this.toolStripMenuItemDelCategoty_Click);
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripTextBox2.Enabled = false;
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.ReadOnly = true;
            this.toolStripTextBox2.Size = new System.Drawing.Size(131, 23);
            this.toolStripTextBox2.Text = "Categoty:";
            // 
            // toolStripCbCategoty
            // 
            this.toolStripCbCategoty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripCbCategoty.Name = "toolStripCbCategoty";
            this.toolStripCbCategoty.Size = new System.Drawing.Size(131, 25);
            this.toolStripCbCategoty.SelectedIndexChanged += new System.EventHandler(this.toolStripCbCategoty_SelectedIndexChanged);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripTextBox1.Enabled = false;
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.ReadOnly = true;
            this.toolStripTextBox1.Size = new System.Drawing.Size(131, 23);
            this.toolStripTextBox1.Text = "RecipeID:";
            // 
            // toolStripCbRecipeIDs
            // 
            this.toolStripCbRecipeIDs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripCbRecipeIDs.Name = "toolStripCbRecipeIDs";
            this.toolStripCbRecipeIDs.Size = new System.Drawing.Size(131, 25);
            this.toolStripCbRecipeIDs.SelectedIndexChanged += new System.EventHandler(this.toolStripCbRecipeIDs_SelectedIndexChanged);
            // 
            // toolStripMenuItemEditSave
            // 
            this.toolStripMenuItemEditSave.Enabled = false;
            this.toolStripMenuItemEditSave.Name = "toolStripMenuItemEditSave";
            this.toolStripMenuItemEditSave.Size = new System.Drawing.Size(133, 21);
            this.toolStripMenuItemEditSave.Text = "编辑";
            this.toolStripMenuItemEditSave.Click += new System.EventHandler(this.toolStripMenuItemEditSave_Click);
            // 
            // toolStripMenuItemEditCancel
            // 
            this.toolStripMenuItemEditCancel.Enabled = false;
            this.toolStripMenuItemEditCancel.Name = "toolStripMenuItemEditCancel";
            this.toolStripMenuItemEditCancel.Size = new System.Drawing.Size(133, 21);
            this.toolStripMenuItemEditCancel.Text = "取消";
            this.toolStripMenuItemEditCancel.Click += new System.EventHandler(this.toolStripMenuItemEditCancel_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(149, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(693, 438);
            this.panel1.TabIndex = 1;
            // 
            // JFUcCmRecipeMgrRT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "JFUcCmRecipeMgrRT";
            this.Size = new System.Drawing.Size(845, 444);
            this.Load += new System.EventHandler(this.JFUcCmRecipeMgrRT_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRecipeMgr;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddRecipe;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelRecipe;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelCategoty;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripComboBox toolStripCbCategoty;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripComboBox toolStripCbRecipeIDs;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEditSave;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEditCancel;
        private System.Windows.Forms.Panel panel1;
    }
}
