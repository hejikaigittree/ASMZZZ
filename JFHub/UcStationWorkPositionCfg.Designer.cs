namespace JFHub
{
    partial class UcStationWorkPositionCfg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UcStationWorkPositionCfg));
            this.lbTips = new System.Windows.Forms.Label();
            this.btAdd = new System.Windows.Forms.Button();
            this.btDel = new System.Windows.Forms.Button();
            this.btEdit = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.dgvPos = new JFUI.RowMergeView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPos)).BeginInit();
            this.SuspendLayout();
            // 
            // lbTips
            // 
            this.lbTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbTips.AutoSize = true;
            this.lbTips.Location = new System.Drawing.Point(4, 374);
            this.lbTips.Name = "lbTips";
            this.lbTips.Size = new System.Drawing.Size(29, 12);
            this.lbTips.TabIndex = 3;
            this.lbTips.Text = "Tips";
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btAdd.Location = new System.Drawing.Point(305, 371);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(75, 23);
            this.btAdd.TabIndex = 4;
            this.btAdd.Text = "添加";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // btDel
            // 
            this.btDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btDel.Location = new System.Drawing.Point(382, 371);
            this.btDel.Name = "btDel";
            this.btDel.Size = new System.Drawing.Size(75, 23);
            this.btDel.TabIndex = 5;
            this.btDel.Text = "删除";
            this.btDel.UseVisualStyleBackColor = true;
            this.btDel.Click += new System.EventHandler(this.btDel_Click);
            // 
            // btEdit
            // 
            this.btEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btEdit.Location = new System.Drawing.Point(556, 371);
            this.btEdit.Name = "btEdit";
            this.btEdit.Size = new System.Drawing.Size(75, 23);
            this.btEdit.TabIndex = 7;
            this.btEdit.Text = "编辑";
            this.btEdit.UseVisualStyleBackColor = true;
            this.btEdit.Click += new System.EventHandler(this.btEdit_Click);
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.Location = new System.Drawing.Point(479, 371);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 6;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // dgvPos
            // 
            this.dgvPos.AllowUserToAddRows = false;
            this.dgvPos.AllowUserToDeleteRows = false;
            this.dgvPos.AllowUserToResizeRows = false;
            this.dgvPos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPos.Location = new System.Drawing.Point(0, 0);
            this.dgvPos.MergeColumnHeaderBackColor = System.Drawing.SystemColors.Control;
            this.dgvPos.MergeColumnNames = ((System.Collections.Generic.List<string>)(resources.GetObject("dgvPos.MergeColumnNames")));
            this.dgvPos.Name = "dgvPos";
            this.dgvPos.RowHeadersWidth = 30;
            this.dgvPos.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvPos.RowTemplate.Height = 23;
            this.dgvPos.Size = new System.Drawing.Size(630, 371);
            this.dgvPos.TabIndex = 8;
            this.dgvPos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPos_CellContentClick);
            this.dgvPos.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvPos_EditingControlShowing);
            // 
            // UcStationWorkPositionCfg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvPos);
            this.Controls.Add(this.btEdit);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btDel);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.lbTips);
            this.Name = "UcStationWorkPositionCfg";
            this.Size = new System.Drawing.Size(633, 398);
            this.Load += new System.EventHandler(this.UcStationWorkPositionCfg_Load);
            this.VisibleChanged += new System.EventHandler(this.UcStationWorkPositionCfg_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbTips;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btDel;
        private System.Windows.Forms.Button btEdit;
        private System.Windows.Forms.Button btCancel;
        private JFUI.RowMergeView dgvPos;
    }
}
