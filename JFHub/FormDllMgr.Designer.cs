namespace JFHub
{
    partial class FormDllMgr
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
            this.lbTips = new System.Windows.Forms.Label();
            this.btDel = new System.Windows.Forms.Button();
            this.btAddFile = new System.Windows.Forms.Button();
            this.dgvInitor = new System.Windows.Forms.DataGridView();
            this.trvDlls = new System.Windows.Forms.TreeView();
            this.ColumnInitorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnReadableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnInitorVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInitor)).BeginInit();
            this.SuspendLayout();
            // 
            // lbTips
            // 
            this.lbTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbTips.AutoSize = true;
            this.lbTips.Location = new System.Drawing.Point(148, 7);
            this.lbTips.Name = "lbTips";
            this.lbTips.Size = new System.Drawing.Size(35, 12);
            this.lbTips.TabIndex = 2;
            this.lbTips.Text = "Tips:";
            // 
            // btDel
            // 
            this.btDel.Location = new System.Drawing.Point(66, 2);
            this.btDel.Name = "btDel";
            this.btDel.Size = new System.Drawing.Size(76, 23);
            this.btDel.TabIndex = 7;
            this.btDel.Text = "移除所选";
            this.btDel.UseVisualStyleBackColor = true;
            this.btDel.Click += new System.EventHandler(this.btDel_Click);
            // 
            // btAddFile
            // 
            this.btAddFile.Location = new System.Drawing.Point(1, 2);
            this.btAddFile.Name = "btAddFile";
            this.btAddFile.Size = new System.Drawing.Size(59, 23);
            this.btAddFile.TabIndex = 6;
            this.btAddFile.Text = "添加Dll";
            this.btAddFile.UseVisualStyleBackColor = true;
            this.btAddFile.Click += new System.EventHandler(this.btAddFile_Click);
            // 
            // dgvInitor
            // 
            this.dgvInitor.AllowUserToAddRows = false;
            this.dgvInitor.AllowUserToDeleteRows = false;
            this.dgvInitor.AllowUserToResizeColumns = false;
            this.dgvInitor.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInitor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInitor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnInitorName,
            this.ColumnReadableName,
            this.ColumnInitorVersion});
            this.dgvInitor.Location = new System.Drawing.Point(358, 31);
            this.dgvInitor.Name = "dgvInitor";
            this.dgvInitor.ReadOnly = true;
            this.dgvInitor.RowHeadersVisible = false;
            this.dgvInitor.RowTemplate.Height = 23;
            this.dgvInitor.Size = new System.Drawing.Size(513, 472);
            this.dgvInitor.TabIndex = 9;
            // 
            // trvDlls
            // 
            this.trvDlls.Location = new System.Drawing.Point(3, 31);
            this.trvDlls.Name = "trvDlls";
            this.trvDlls.Size = new System.Drawing.Size(352, 472);
            this.trvDlls.TabIndex = 10;
            this.trvDlls.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvDlls_AfterSelect);
            // 
            // ColumnInitorName
            // 
            this.ColumnInitorName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnInitorName.HeaderText = "JFInitor名称";
            this.ColumnInitorName.Name = "ColumnInitorName";
            this.ColumnInitorName.ReadOnly = true;
            this.ColumnInitorName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnInitorName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnInitorName.Width = 197;
            // 
            // ColumnReadableName
            // 
            this.ColumnReadableName.HeaderText = "简介";
            this.ColumnReadableName.Name = "ColumnReadableName";
            this.ColumnReadableName.ReadOnly = true;
            // 
            // ColumnInitorVersion
            // 
            this.ColumnInitorVersion.HeaderText = "版本信息";
            this.ColumnInitorVersion.Name = "ColumnInitorVersion";
            this.ColumnInitorVersion.ReadOnly = true;
            // 
            // FormDllMgr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(873, 503);
            this.Controls.Add(this.trvDlls);
            this.Controls.Add(this.dgvInitor);
            this.Controls.Add(this.btDel);
            this.Controls.Add(this.btAddFile);
            this.Controls.Add(this.lbTips);
            this.Name = "FormDllMgr";
            this.Text = "JF拓展Dll管理";
            this.Load += new System.EventHandler(this.FormDllMgr_Load);
            this.VisibleChanged += new System.EventHandler(this.FormDllMgr_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInitor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbTips;
        private System.Windows.Forms.Button btDel;
        private System.Windows.Forms.Button btAddFile;
        private System.Windows.Forms.DataGridView dgvInitor;
        private System.Windows.Forms.TreeView trvDlls;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnInitorName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnReadableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnInitorVersion;
    }
}