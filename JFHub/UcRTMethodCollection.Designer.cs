namespace JFHub
{
    partial class UcRTMethodCollection
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
            this.dgvDataPool = new System.Windows.Forms.DataGridView();
            this.ColumnID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOutterDataPool = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataPool)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutterDataPool)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvDataPool
            // 
            this.dgvDataPool.AllowUserToAddRows = false;
            this.dgvDataPool.AllowUserToDeleteRows = false;
            this.dgvDataPool.AllowUserToResizeRows = false;
            this.dgvDataPool.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDataPool.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnID,
            this.ColumnValue});
            this.dgvDataPool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDataPool.Location = new System.Drawing.Point(0, 0);
            this.dgvDataPool.Name = "dgvDataPool";
            this.dgvDataPool.ReadOnly = true;
            this.dgvDataPool.RowHeadersVisible = false;
            this.dgvDataPool.RowTemplate.Height = 23;
            this.dgvDataPool.Size = new System.Drawing.Size(291, 460);
            this.dgvDataPool.TabIndex = 7;
            // 
            // ColumnID
            // 
            this.ColumnID.HeaderText = "内部池:ID";
            this.ColumnID.Name = "ColumnID";
            this.ColumnID.ReadOnly = true;
            // 
            // ColumnValue
            // 
            this.ColumnValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnValue.HeaderText = "Value";
            this.ColumnValue.Name = "ColumnValue";
            this.ColumnValue.ReadOnly = true;
            this.ColumnValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvOutterDataPool
            // 
            this.dgvOutterDataPool.AllowUserToAddRows = false;
            this.dgvOutterDataPool.AllowUserToDeleteRows = false;
            this.dgvOutterDataPool.AllowUserToResizeRows = false;
            this.dgvOutterDataPool.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOutterDataPool.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgvOutterDataPool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOutterDataPool.Location = new System.Drawing.Point(0, 0);
            this.dgvOutterDataPool.Name = "dgvOutterDataPool";
            this.dgvOutterDataPool.ReadOnly = true;
            this.dgvOutterDataPool.RowHeadersVisible = false;
            this.dgvOutterDataPool.RowTemplate.Height = 23;
            this.dgvOutterDataPool.Size = new System.Drawing.Size(264, 460);
            this.dgvOutterDataPool.TabIndex = 8;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "外部池:ID";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvOutterDataPool);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvDataPool);
            this.splitContainer1.Size = new System.Drawing.Size(559, 460);
            this.splitContainer1.SplitterDistance = 264;
            this.splitContainer1.TabIndex = 9;
            // 
            // UcRTMethodCollection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UcRTMethodCollection";
            this.Size = new System.Drawing.Size(559, 460);
            this.Load += new System.EventHandler(this.UcMethodCollection_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataPool)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutterDataPool)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDataPool;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnValue;
        private System.Windows.Forms.DataGridView dgvOutterDataPool;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
