namespace DLAF
{
    partial class UcDLAFMainStation
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvDetectItems = new System.Windows.Forms.DataGridView();
            this.colDieIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetectItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colError = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetectValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRefValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRemarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkJustShowNG = new System.Windows.Forms.CheckBox();
            this.cbTaskImgShow = new System.Windows.Forms.ComboBox();
            this.lbRunStatus = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbLotID = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btSwitchRTImage = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbProdID = new System.Windows.Forms.Label();
            this.btSetLotID = new System.Windows.Forms.Button();
            this.btSetProdID = new System.Windows.Forms.Button();
            this.htWindowControl1 = new HTHalControl.HTWindowControl();
            this.rchDieInfo = new System.Windows.Forms.RichTextBox();
            this.btReview = new System.Windows.Forms.Button();
            this.btShowRunInfo = new System.Windows.Forms.Button();
            this.btManualFeed = new System.Windows.Forms.Button();
            this.rchRDetectInfo = new System.Windows.Forms.RichTextBox();
            this.btClearDetectInfo = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mappCtrl = new HTMappingControl.MappingControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetectItems)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvDetectItems);
            this.splitContainer1.Panel1.Controls.Add(this.chkJustShowNG);
            this.splitContainer1.Panel1.Controls.Add(this.cbTaskImgShow);
            this.splitContainer1.Panel1.Controls.Add(this.lbRunStatus);
            this.splitContainer1.Panel1.Controls.Add(this.label8);
            this.splitContainer1.Panel1.Controls.Add(this.lbLotID);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btSwitchRTImage);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.lbProdID);
            this.splitContainer1.Panel1.Controls.Add(this.btSetLotID);
            this.splitContainer1.Panel1.Controls.Add(this.btSetProdID);
            this.splitContainer1.Panel1.Controls.Add(this.htWindowControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rchDieInfo);
            this.splitContainer1.Panel2.Controls.Add(this.btReview);
            this.splitContainer1.Panel2.Controls.Add(this.btShowRunInfo);
            this.splitContainer1.Panel2.Controls.Add(this.btManualFeed);
            this.splitContainer1.Panel2.Controls.Add(this.rchRDetectInfo);
            this.splitContainer1.Panel2.Controls.Add(this.btClearDetectInfo);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.mappCtrl);
            this.splitContainer1.Size = new System.Drawing.Size(1160, 665);
            this.splitContainer1.SplitterDistance = 813;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgvDetectItems
            // 
            this.dgvDetectItems.AllowUserToAddRows = false;
            this.dgvDetectItems.AllowUserToDeleteRows = false;
            this.dgvDetectItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetectItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetectItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDieIndex,
            this.colDetectItem,
            this.colError,
            this.colDetectValue,
            this.colRefValue,
            this.colRemarks});
            this.dgvDetectItems.Location = new System.Drawing.Point(6, 357);
            this.dgvDetectItems.Name = "dgvDetectItems";
            this.dgvDetectItems.ReadOnly = true;
            this.dgvDetectItems.RowTemplate.Height = 23;
            this.dgvDetectItems.Size = new System.Drawing.Size(804, 305);
            this.dgvDetectItems.TabIndex = 23;
            // 
            // colDieIndex
            // 
            this.colDieIndex.HeaderText = "Die";
            this.colDieIndex.Name = "colDieIndex";
            this.colDieIndex.ReadOnly = true;
            this.colDieIndex.Width = 50;
            // 
            // colDetectItem
            // 
            this.colDetectItem.HeaderText = "检测项";
            this.colDetectItem.Name = "colDetectItem";
            this.colDetectItem.ReadOnly = true;
            // 
            // colError
            // 
            this.colError.HeaderText = "检测结果";
            this.colError.Name = "colError";
            this.colError.ReadOnly = true;
            this.colError.Width = 120;
            // 
            // colDetectValue
            // 
            this.colDetectValue.HeaderText = "检测值";
            this.colDetectValue.Name = "colDetectValue";
            this.colDetectValue.ReadOnly = true;
            this.colDetectValue.Width = 200;
            // 
            // colRefValue
            // 
            this.colRefValue.HeaderText = "标准值";
            this.colRefValue.Name = "colRefValue";
            this.colRefValue.ReadOnly = true;
            this.colRefValue.Width = 300;
            // 
            // colRemarks
            // 
            this.colRemarks.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRemarks.HeaderText = "备注";
            this.colRemarks.Name = "colRemarks";
            this.colRemarks.ReadOnly = true;
            // 
            // chkJustShowNG
            // 
            this.chkJustShowNG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkJustShowNG.AutoSize = true;
            this.chkJustShowNG.Location = new System.Drawing.Point(687, 335);
            this.chkJustShowNG.Name = "chkJustShowNG";
            this.chkJustShowNG.Size = new System.Drawing.Size(120, 16);
            this.chkJustShowNG.TabIndex = 22;
            this.chkJustShowNG.Text = "只显示不良检测项";
            this.chkJustShowNG.UseVisualStyleBackColor = true;
            // 
            // cbTaskImgShow
            // 
            this.cbTaskImgShow.FormattingEnabled = true;
            this.cbTaskImgShow.Location = new System.Drawing.Point(212, 334);
            this.cbTaskImgShow.Name = "cbTaskImgShow";
            this.cbTaskImgShow.Size = new System.Drawing.Size(154, 20);
            this.cbTaskImgShow.TabIndex = 19;
            this.cbTaskImgShow.SelectedIndexChanged += new System.EventHandler(this.cbTaskImgShow_SelectedIndexChanged);
            // 
            // lbRunStatus
            // 
            this.lbRunStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbRunStatus.AutoSize = true;
            this.lbRunStatus.Font = new System.Drawing.Font("YouYuan", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbRunStatus.ForeColor = System.Drawing.Color.Black;
            this.lbRunStatus.Location = new System.Drawing.Point(465, 9);
            this.lbRunStatus.Name = "lbRunStatus";
            this.lbRunStatus.Size = new System.Drawing.Size(59, 16);
            this.lbRunStatus.TabIndex = 18;
            this.lbRunStatus.Text = "未开始";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("YouYuan", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(412, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 16);
            this.label8.TabIndex = 17;
            this.label8.Text = "状态:";
            // 
            // lbLotID
            // 
            this.lbLotID.AutoSize = true;
            this.lbLotID.Font = new System.Drawing.Font("YouYuan", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbLotID.ForeColor = System.Drawing.Color.Red;
            this.lbLotID.Location = new System.Drawing.Point(255, 8);
            this.lbLotID.Name = "lbLotID";
            this.lbLotID.Size = new System.Drawing.Size(59, 16);
            this.lbLotID.TabIndex = 16;
            this.lbLotID.Text = "未设置";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("YouYuan", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(202, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "批次:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("YouYuan", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 16);
            this.label1.TabIndex = 14;
            this.label1.Text = "产品:";
            // 
            // btSwitchRTImage
            // 
            this.btSwitchRTImage.Location = new System.Drawing.Point(4, 331);
            this.btSwitchRTImage.Name = "btSwitchRTImage";
            this.btSwitchRTImage.Size = new System.Drawing.Size(100, 23);
            this.btSwitchRTImage.TabIndex = 13;
            this.btSwitchRTImage.Text = "显示实时图像";
            this.btSwitchRTImage.UseVisualStyleBackColor = true;
            this.btSwitchRTImage.Click += new System.EventHandler(this.btSwitchRTImage_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(106, 338);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "Task通道图像显示";
            // 
            // lbProdID
            // 
            this.lbProdID.AutoSize = true;
            this.lbProdID.Font = new System.Drawing.Font("YouYuan", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbProdID.ForeColor = System.Drawing.Color.Red;
            this.lbProdID.Location = new System.Drawing.Point(49, 8);
            this.lbProdID.Name = "lbProdID";
            this.lbProdID.Size = new System.Drawing.Size(59, 16);
            this.lbProdID.TabIndex = 10;
            this.lbProdID.Text = "未设置";
            // 
            // btSetLotID
            // 
            this.btSetLotID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSetLotID.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btSetLotID.Location = new System.Drawing.Point(718, 3);
            this.btSetLotID.Name = "btSetLotID";
            this.btSetLotID.Size = new System.Drawing.Size(92, 28);
            this.btSetLotID.TabIndex = 9;
            this.btSetLotID.Text = "变更批次";
            this.btSetLotID.UseVisualStyleBackColor = true;
            this.btSetLotID.Click += new System.EventHandler(this.btSetLotID_Click);
            // 
            // btSetProdID
            // 
            this.btSetProdID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSetProdID.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btSetProdID.Location = new System.Drawing.Point(624, 3);
            this.btSetProdID.Name = "btSetProdID";
            this.btSetProdID.Size = new System.Drawing.Size(92, 28);
            this.btSetProdID.TabIndex = 8;
            this.btSetProdID.Text = "切换产品";
            this.btSetProdID.UseVisualStyleBackColor = true;
            this.btSetProdID.Click += new System.EventHandler(this.btSetProdID_Click);
            // 
            // htWindowControl1
            // 
            this.htWindowControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.htWindowControl1.BackColor = System.Drawing.Color.Transparent;
            this.htWindowControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.htWindowControl1.ColorName = "red";
            this.htWindowControl1.ColorType = 0;
            this.htWindowControl1.Column = null;
            this.htWindowControl1.Column1 = null;
            this.htWindowControl1.Column2 = null;
            this.htWindowControl1.Image = null;
            this.htWindowControl1.Length1 = null;
            this.htWindowControl1.Length2 = null;
            this.htWindowControl1.Location = new System.Drawing.Point(4, 33);
            this.htWindowControl1.Name = "htWindowControl1";
            this.htWindowControl1.Phi = null;
            this.htWindowControl1.Radius = null;
            this.htWindowControl1.Radius1 = null;
            this.htWindowControl1.Radius2 = null;
            this.htWindowControl1.Region = null;
            this.htWindowControl1.RegionType = "";
            this.htWindowControl1.Row = null;
            this.htWindowControl1.Row1 = null;
            this.htWindowControl1.Row2 = null;
            this.htWindowControl1.Size = new System.Drawing.Size(806, 298);
            this.htWindowControl1.TabIndex = 0;
            this.htWindowControl1.UmPerPix = -1D;
            // 
            // rchDieInfo
            // 
            this.rchDieInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rchDieInfo.Location = new System.Drawing.Point(3, 335);
            this.rchDieInfo.Name = "rchDieInfo";
            this.rchDieInfo.ReadOnly = true;
            this.rchDieInfo.Size = new System.Drawing.Size(334, 77);
            this.rchDieInfo.TabIndex = 21;
            this.rchDieInfo.Text = "";
            // 
            // btReview
            // 
            this.btReview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btReview.Location = new System.Drawing.Point(111, 627);
            this.btReview.Name = "btReview";
            this.btReview.Size = new System.Drawing.Size(100, 23);
            this.btReview.TabIndex = 20;
            this.btReview.Text = "Review";
            this.btReview.UseVisualStyleBackColor = true;
            this.btReview.Click += new System.EventHandler(this.btReview_Click);
            // 
            // btShowRunInfo
            // 
            this.btShowRunInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btShowRunInfo.Location = new System.Drawing.Point(5, 627);
            this.btShowRunInfo.Name = "btShowRunInfo";
            this.btShowRunInfo.Size = new System.Drawing.Size(100, 23);
            this.btShowRunInfo.TabIndex = 19;
            this.btShowRunInfo.Text = "显示运行信息";
            this.btShowRunInfo.UseVisualStyleBackColor = true;
            this.btShowRunInfo.Click += new System.EventHandler(this.btShowRunInfo_Click);
            // 
            // btManualFeed
            // 
            this.btManualFeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btManualFeed.Location = new System.Drawing.Point(262, 627);
            this.btManualFeed.Name = "btManualFeed";
            this.btManualFeed.Size = new System.Drawing.Size(75, 23);
            this.btManualFeed.TabIndex = 6;
            this.btManualFeed.Text = "手动上料";
            this.btManualFeed.UseVisualStyleBackColor = true;
            this.btManualFeed.Click += new System.EventHandler(this.btManualFeed_Click);
            // 
            // rchRDetectInfo
            // 
            this.rchRDetectInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rchRDetectInfo.BackColor = System.Drawing.SystemColors.Control;
            this.rchRDetectInfo.Location = new System.Drawing.Point(3, 437);
            this.rchRDetectInfo.Name = "rchRDetectInfo";
            this.rchRDetectInfo.ReadOnly = true;
            this.rchRDetectInfo.Size = new System.Drawing.Size(334, 183);
            this.rchRDetectInfo.TabIndex = 5;
            this.rchRDetectInfo.Text = "";
            // 
            // btClearDetectInfo
            // 
            this.btClearDetectInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btClearDetectInfo.Location = new System.Drawing.Point(298, 414);
            this.btClearDetectInfo.Name = "btClearDetectInfo";
            this.btClearDetectInfo.Size = new System.Drawing.Size(40, 23);
            this.btClearDetectInfo.TabIndex = 4;
            this.btClearDetectInfo.Text = "清空";
            this.btClearDetectInfo.UseVisualStyleBackColor = true;
            this.btClearDetectInfo.Click += new System.EventHandler(this.btClearDetectInfo_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 421);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "当前料片检测信息";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "检测结果";
            // 
            // mappCtrl
            // 
            this.mappCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mappCtrl.BackColor = System.Drawing.Color.WhiteSmoke;
            this.mappCtrl.BorderThickness = 1D;
            this.mappCtrl.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.mappCtrl.Location = new System.Drawing.Point(5, 22);
            this.mappCtrl.MinCellHeight = 10D;
            this.mappCtrl.MinCellWidth = 10D;
            this.mappCtrl.Name = "mappCtrl";
            this.mappCtrl.Size = new System.Drawing.Size(332, 309);
            this.mappCtrl.TabIndex = 0;
            // 
            // UcDLAFMainStation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UcDLAFMainStation";
            this.Size = new System.Drawing.Size(1160, 665);
            this.Load += new System.EventHandler(this.UcDLAFMainStation_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetectItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btSetProdID;
        private HTHalControl.HTWindowControl htWindowControl1;
        private System.Windows.Forms.Button btSetLotID;
        private System.Windows.Forms.Label lbProdID;
        private System.Windows.Forms.Label label2;
        private HTMappingControl.MappingControl mappCtrl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox rchRDetectInfo;
        private System.Windows.Forms.Button btClearDetectInfo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btManualFeed;
        private System.Windows.Forms.Button btSwitchRTImage;
        private System.Windows.Forms.Label lbLotID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbRunStatus;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btShowRunInfo;
        private System.Windows.Forms.ComboBox cbTaskImgShow;
        private System.Windows.Forms.Button btReview;
        private System.Windows.Forms.RichTextBox rchDieInfo;
        private System.Windows.Forms.DataGridView dgvDetectItems;
        private System.Windows.Forms.CheckBox chkJustShowNG;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDieIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetectItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colError;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetectValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRefValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRemarks;
    }
}
