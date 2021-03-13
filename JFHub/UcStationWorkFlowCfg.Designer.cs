namespace JFHub
{
    partial class UcStationWorkFlowCfg
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
            this.dgvWorkFlows = new System.Windows.Forms.DataGridView();
            this.btDel = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.lbTital = new System.Windows.Forms.Label();
            this.ColumnWorkFlowName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSetWorkFlow = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnImport = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnExport = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkFlows)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvWorkFlows
            // 
            this.dgvWorkFlows.AllowUserToAddRows = false;
            this.dgvWorkFlows.AllowUserToDeleteRows = false;
            this.dgvWorkFlows.AllowUserToResizeRows = false;
            this.dgvWorkFlows.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvWorkFlows.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWorkFlows.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnWorkFlowName,
            this.ColumnSetWorkFlow,
            this.ColumnImport,
            this.ColumnExport});
            this.dgvWorkFlows.Location = new System.Drawing.Point(4, 29);
            this.dgvWorkFlows.MultiSelect = false;
            this.dgvWorkFlows.Name = "dgvWorkFlows";
            this.dgvWorkFlows.ReadOnly = true;
            this.dgvWorkFlows.RowHeadersWidth = 30;
            this.dgvWorkFlows.RowTemplate.Height = 23;
            this.dgvWorkFlows.Size = new System.Drawing.Size(656, 400);
            this.dgvWorkFlows.TabIndex = 0;
            // 
            // btDel
            // 
            this.btDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDel.Location = new System.Drawing.Point(619, 3);
            this.btDel.Name = "btDel";
            this.btDel.Size = new System.Drawing.Size(40, 23);
            this.btDel.TabIndex = 2;
            this.btDel.Text = "删除";
            this.btDel.UseVisualStyleBackColor = true;
            this.btDel.Click += new System.EventHandler(this.btDel_Click);
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btAdd.Location = new System.Drawing.Point(575, 3);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(40, 23);
            this.btAdd.TabIndex = 3;
            this.btAdd.Text = "添加";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // lbTital
            // 
            this.lbTital.AutoSize = true;
            this.lbTital.Location = new System.Drawing.Point(4, 7);
            this.lbTital.Name = "lbTital";
            this.lbTital.Size = new System.Drawing.Size(137, 12);
            this.lbTital.TabIndex = 4;
            this.lbTital.Text = "工站:未设置 工作流配置";
            // 
            // ColumnWorkFlowName
            // 
            this.ColumnWorkFlowName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnWorkFlowName.HeaderText = "工作流名称";
            this.ColumnWorkFlowName.Name = "ColumnWorkFlowName";
            this.ColumnWorkFlowName.ReadOnly = true;
            // 
            // ColumnSetWorkFlow
            // 
            this.ColumnSetWorkFlow.HeaderText = "编辑/调试";
            this.ColumnSetWorkFlow.Name = "ColumnSetWorkFlow";
            this.ColumnSetWorkFlow.ReadOnly = true;
            this.ColumnSetWorkFlow.Width = 70;
            // 
            // ColumnImport
            // 
            this.ColumnImport.HeaderText = "从文件导入";
            this.ColumnImport.Name = "ColumnImport";
            this.ColumnImport.ReadOnly = true;
            this.ColumnImport.Width = 80;
            // 
            // ColumnExport
            // 
            this.ColumnExport.HeaderText = "导出到文件";
            this.ColumnExport.Name = "ColumnExport";
            this.ColumnExport.ReadOnly = true;
            this.ColumnExport.Width = 80;
            // 
            // UcStationWorkFlowCfg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbTital);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.btDel);
            this.Controls.Add(this.dgvWorkFlows);
            this.Name = "UcStationWorkFlowCfg";
            this.Size = new System.Drawing.Size(663, 435);
            this.Load += new System.EventHandler(this.UcStationWorkFlowCfg_Load);
            this.VisibleChanged += new System.EventHandler(this.UcStationWorkFlowCfg_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkFlows)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvWorkFlows;
        private System.Windows.Forms.Button btDel;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Label lbTital;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnWorkFlowName;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnSetWorkFlow;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnImport;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnExport;
    }
}
