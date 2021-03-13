namespace DLAF_DS
{
    partial class UcOfflineDetectStation
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
            this.rbTask5 = new System.Windows.Forms.RadioButton();
            this.rbTask4 = new System.Windows.Forms.RadioButton();
            this.rbTask3 = new System.Windows.Forms.RadioButton();
            this.rbTask2 = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.rbTask1 = new System.Windows.Forms.RadioButton();
            this.rbTask0 = new System.Windows.Forms.RadioButton();
            this.dgvInspectResultsInFov = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDescrib = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTaskName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDetail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label5 = new System.Windows.Forms.Label();
            this.btUpdateInspect = new System.Windows.Forms.Button();
            this.lbSelectedInfo = new System.Windows.Forms.Label();
            this.rchDetectInfo = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rchPicFolder = new System.Windows.Forms.RichTextBox();
            this.btEditCancel = new System.Windows.Forms.Button();
            this.btSetPicFolder = new System.Windows.Forms.Button();
            this.btEditSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbLotID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbRecipeID = new System.Windows.Forms.ComboBox();
            this.lstBoxPieceIDs = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.mapDetectCells = new HTMappingControl.MappingControl();
            this.htDetectImg = new HTHalControl.HTWindowControl();
            this.htFullImg = new HTHalControl.HTWindowControl();
            this.chkShowAllItems = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInspectResultsInFov)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chkShowAllItems);
            this.splitContainer1.Panel1.Controls.Add(this.rbTask5);
            this.splitContainer1.Panel1.Controls.Add(this.rbTask4);
            this.splitContainer1.Panel1.Controls.Add(this.rbTask3);
            this.splitContainer1.Panel1.Controls.Add(this.rbTask2);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.rbTask1);
            this.splitContainer1.Panel1.Controls.Add(this.rbTask0);
            this.splitContainer1.Panel1.Controls.Add(this.dgvInspectResultsInFov);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.htDetectImg);
            this.splitContainer1.Panel1.Controls.Add(this.btUpdateInspect);
            this.splitContainer1.Panel1.Controls.Add(this.lbSelectedInfo);
            this.splitContainer1.Panel1.Controls.Add(this.rchDetectInfo);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.lstBoxPieceIDs);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1086, 677);
            this.splitContainer1.SplitterDistance = 594;
            this.splitContainer1.TabIndex = 14;
            // 
            // rbTask5
            // 
            this.rbTask5.AutoSize = true;
            this.rbTask5.Location = new System.Drawing.Point(515, 9);
            this.rbTask5.Name = "rbTask5";
            this.rbTask5.Size = new System.Drawing.Size(35, 16);
            this.rbTask5.TabIndex = 37;
            this.rbTask5.TabStop = true;
            this.rbTask5.Tag = "5";
            this.rbTask5.Text = "T5";
            this.rbTask5.UseVisualStyleBackColor = true;
            this.rbTask5.CheckedChanged += new System.EventHandler(this.rbButton_ChangeTaskShow);
            // 
            // rbTask4
            // 
            this.rbTask4.AutoSize = true;
            this.rbTask4.Location = new System.Drawing.Point(461, 9);
            this.rbTask4.Name = "rbTask4";
            this.rbTask4.Size = new System.Drawing.Size(35, 16);
            this.rbTask4.TabIndex = 36;
            this.rbTask4.TabStop = true;
            this.rbTask4.Text = "T4";
            this.rbTask4.UseVisualStyleBackColor = true;
            // 
            // rbTask3
            // 
            this.rbTask3.AutoSize = true;
            this.rbTask3.Location = new System.Drawing.Point(409, 9);
            this.rbTask3.Name = "rbTask3";
            this.rbTask3.Size = new System.Drawing.Size(35, 16);
            this.rbTask3.TabIndex = 35;
            this.rbTask3.TabStop = true;
            this.rbTask3.Text = "T3";
            this.rbTask3.UseVisualStyleBackColor = true;
            // 
            // rbTask2
            // 
            this.rbTask2.AutoSize = true;
            this.rbTask2.Location = new System.Drawing.Point(361, 9);
            this.rbTask2.Name = "rbTask2";
            this.rbTask2.Size = new System.Drawing.Size(35, 16);
            this.rbTask2.TabIndex = 34;
            this.rbTask2.TabStop = true;
            this.rbTask2.Tag = "2";
            this.rbTask2.Text = "T2";
            this.rbTask2.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(213, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 33;
            this.label6.Text = "显示图层";
            // 
            // rbTask1
            // 
            this.rbTask1.AutoSize = true;
            this.rbTask1.Location = new System.Drawing.Point(319, 9);
            this.rbTask1.Name = "rbTask1";
            this.rbTask1.Size = new System.Drawing.Size(35, 16);
            this.rbTask1.TabIndex = 32;
            this.rbTask1.TabStop = true;
            this.rbTask1.Tag = "1";
            this.rbTask1.Text = "T1";
            this.rbTask1.UseVisualStyleBackColor = true;
            // 
            // rbTask0
            // 
            this.rbTask0.AutoSize = true;
            this.rbTask0.Location = new System.Drawing.Point(271, 9);
            this.rbTask0.Name = "rbTask0";
            this.rbTask0.Size = new System.Drawing.Size(35, 16);
            this.rbTask0.TabIndex = 31;
            this.rbTask0.TabStop = true;
            this.rbTask0.Tag = "0";
            this.rbTask0.Text = "T0";
            this.rbTask0.UseVisualStyleBackColor = true;
            // 
            // dgvInspectResultsInFov
            // 
            this.dgvInspectResultsInFov.AllowUserToAddRows = false;
            this.dgvInspectResultsInFov.AllowUserToDeleteRows = false;
            this.dgvInspectResultsInFov.AllowUserToResizeRows = false;
            this.dgvInspectResultsInFov.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvInspectResultsInFov.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInspectResultsInFov.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.ColumnIndex,
            this.ColumnCode,
            this.ColumnDescrib,
            this.ColumnTaskName,
            this.ColumnDetail});
            this.dgvInspectResultsInFov.Location = new System.Drawing.Point(550, 227);
            this.dgvInspectResultsInFov.Name = "dgvInspectResultsInFov";
            this.dgvInspectResultsInFov.RowHeadersVisible = false;
            this.dgvInspectResultsInFov.RowTemplate.Height = 23;
            this.dgvInspectResultsInFov.Size = new System.Drawing.Size(530, 340);
            this.dgvInspectResultsInFov.TabIndex = 30;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Die";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 30;
            // 
            // ColumnIndex
            // 
            this.ColumnIndex.HeaderText = "检测项";
            this.ColumnIndex.Name = "ColumnIndex";
            this.ColumnIndex.ReadOnly = true;
            this.ColumnIndex.Width = 80;
            // 
            // ColumnCode
            // 
            this.ColumnCode.HeaderText = "检测结果";
            this.ColumnCode.Name = "ColumnCode";
            this.ColumnCode.ReadOnly = true;
            this.ColumnCode.Width = 80;
            // 
            // ColumnDescrib
            // 
            this.ColumnDescrib.HeaderText = "检测标准数据";
            this.ColumnDescrib.Name = "ColumnDescrib";
            this.ColumnDescrib.ReadOnly = true;
            this.ColumnDescrib.Width = 110;
            // 
            // ColumnTaskName
            // 
            this.ColumnTaskName.HeaderText = "检测结果数据";
            this.ColumnTaskName.Name = "ColumnTaskName";
            this.ColumnTaskName.ReadOnly = true;
            this.ColumnTaskName.Width = 110;
            // 
            // ColumnDetail
            // 
            this.ColumnDetail.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnDetail.HeaderText = "备注";
            this.ColumnDetail.Name = "ColumnDetail";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(731, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 29;
            this.label5.Text = "运行信息";
            // 
            // btUpdateInspect
            // 
            this.btUpdateInspect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btUpdateInspect.Location = new System.Drawing.Point(550, 567);
            this.btUpdateInspect.Name = "btUpdateInspect";
            this.btUpdateInspect.Size = new System.Drawing.Size(132, 23);
            this.btUpdateInspect.TabIndex = 22;
            this.btUpdateInspect.Text = "更新视觉算子参数";
            this.btUpdateInspect.UseVisualStyleBackColor = true;
            this.btUpdateInspect.Click += new System.EventHandler(this.btUpdateInspect_Click);
            // 
            // lbSelectedInfo
            // 
            this.lbSelectedInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSelectedInfo.AutoSize = true;
            this.lbSelectedInfo.Location = new System.Drawing.Point(555, 212);
            this.lbSelectedInfo.Name = "lbSelectedInfo";
            this.lbSelectedInfo.Size = new System.Drawing.Size(101, 12);
            this.lbSelectedInfo.TabIndex = 20;
            this.lbSelectedInfo.Text = "选中单元检测结果";
            // 
            // rchDetectInfo
            // 
            this.rchDetectInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rchDetectInfo.Location = new System.Drawing.Point(550, 28);
            this.rchDetectInfo.Name = "rchDetectInfo";
            this.rchDetectInfo.ReadOnly = true;
            this.rchDetectInfo.Size = new System.Drawing.Size(530, 181);
            this.rchDetectInfo.TabIndex = 18;
            this.rchDetectInfo.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.rchPicFolder);
            this.groupBox1.Controls.Add(this.btEditCancel);
            this.groupBox1.Controls.Add(this.btSetPicFolder);
            this.groupBox1.Controls.Add(this.btEditSave);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cbLotID);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbRecipeID);
            this.groupBox1.Location = new System.Drawing.Point(5, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(157, 170);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "检测参数设置";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "图片文件夹";
            // 
            // rchPicFolder
            // 
            this.rchPicFolder.BackColor = System.Drawing.SystemColors.Control;
            this.rchPicFolder.Location = new System.Drawing.Point(8, 97);
            this.rchPicFolder.Name = "rchPicFolder";
            this.rchPicFolder.ReadOnly = true;
            this.rchPicFolder.Size = new System.Drawing.Size(149, 45);
            this.rchPicFolder.TabIndex = 10;
            this.rchPicFolder.Text = "";
            // 
            // btEditCancel
            // 
            this.btEditCancel.Enabled = false;
            this.btEditCancel.Location = new System.Drawing.Point(102, 144);
            this.btEditCancel.Name = "btEditCancel";
            this.btEditCancel.Size = new System.Drawing.Size(55, 23);
            this.btEditCancel.TabIndex = 9;
            this.btEditCancel.Text = "取消";
            this.btEditCancel.UseVisualStyleBackColor = true;
            this.btEditCancel.Click += new System.EventHandler(this.btEditCancel_Click);
            // 
            // btSetPicFolder
            // 
            this.btSetPicFolder.Enabled = false;
            this.btSetPicFolder.Location = new System.Drawing.Point(84, 73);
            this.btSetPicFolder.Name = "btSetPicFolder";
            this.btSetPicFolder.Size = new System.Drawing.Size(70, 23);
            this.btSetPicFolder.TabIndex = 8;
            this.btSetPicFolder.Text = "设置路径";
            this.btSetPicFolder.UseVisualStyleBackColor = true;
            this.btSetPicFolder.Click += new System.EventHandler(this.btSetPicFolder_Click);
            // 
            // btEditSave
            // 
            this.btEditSave.Location = new System.Drawing.Point(42, 144);
            this.btEditSave.Name = "btEditSave";
            this.btEditSave.Size = new System.Drawing.Size(54, 23);
            this.btEditSave.TabIndex = 7;
            this.btEditSave.Text = "编辑";
            this.btEditSave.UseVisualStyleBackColor = true;
            this.btEditSave.Click += new System.EventHandler(this.btEditSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "批次号";
            // 
            // cbLotID
            // 
            this.cbLotID.Enabled = false;
            this.cbLotID.FormattingEnabled = true;
            this.cbLotID.Location = new System.Drawing.Point(52, 47);
            this.cbLotID.Name = "cbLotID";
            this.cbLotID.Size = new System.Drawing.Size(102, 20);
            this.cbLotID.TabIndex = 2;
            this.cbLotID.SelectedIndexChanged += new System.EventHandler(this.cbLotID_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "产品ID";
            // 
            // cbRecipeID
            // 
            this.cbRecipeID.Enabled = false;
            this.cbRecipeID.FormattingEnabled = true;
            this.cbRecipeID.Location = new System.Drawing.Point(52, 21);
            this.cbRecipeID.Name = "cbRecipeID";
            this.cbRecipeID.Size = new System.Drawing.Size(102, 20);
            this.cbRecipeID.TabIndex = 1;
            this.cbRecipeID.SelectedIndexChanged += new System.EventHandler(this.cbRecipeID_SelectedIndexChanged);
            // 
            // lstBoxPieceIDs
            // 
            this.lstBoxPieceIDs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstBoxPieceIDs.BackColor = System.Drawing.SystemColors.Control;
            this.lstBoxPieceIDs.FormattingEnabled = true;
            this.lstBoxPieceIDs.ItemHeight = 12;
            this.lstBoxPieceIDs.Location = new System.Drawing.Point(5, 198);
            this.lstBoxPieceIDs.Name = "lstBoxPieceIDs";
            this.lstBoxPieceIDs.Size = new System.Drawing.Size(157, 388);
            this.lstBoxPieceIDs.TabIndex = 15;
            this.lstBoxPieceIDs.SelectedIndexChanged += new System.EventHandler(this.lstBoxPieceIDs_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "料片ID列表";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.htFullImg);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.mapDetectCells);
            this.splitContainer2.Size = new System.Drawing.Size(1086, 79);
            this.splitContainer2.SplitterDistance = 362;
            this.splitContainer2.TabIndex = 0;
            // 
            // mapDetectCells
            // 
            this.mapDetectCells.BackColor = System.Drawing.Color.Transparent;
            this.mapDetectCells.BorderThickness = 1D;
            this.mapDetectCells.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.mapDetectCells.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapDetectCells.Location = new System.Drawing.Point(0, 0);
            this.mapDetectCells.MinCellHeight = 20D;
            this.mapDetectCells.MinCellWidth = 20D;
            this.mapDetectCells.Name = "mapDetectCells";
            this.mapDetectCells.Size = new System.Drawing.Size(720, 79);
            this.mapDetectCells.TabIndex = 0;
            // 
            // htDetectImg
            // 
            this.htDetectImg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.htDetectImg.BackColor = System.Drawing.Color.Transparent;
            this.htDetectImg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.htDetectImg.ColorName = "red";
            this.htDetectImg.ColorType = 0;
            this.htDetectImg.Column = null;
            this.htDetectImg.Column1 = null;
            this.htDetectImg.Column2 = null;
            this.htDetectImg.Image = null;
            this.htDetectImg.Length1 = null;
            this.htDetectImg.Length2 = null;
            this.htDetectImg.Location = new System.Drawing.Point(165, 28);
            this.htDetectImg.Name = "htDetectImg";
            this.htDetectImg.Phi = null;
            this.htDetectImg.Radius = null;
            this.htDetectImg.Radius1 = null;
            this.htDetectImg.Radius2 = null;
            this.htDetectImg.Region = null;
            this.htDetectImg.RegionType = "";
            this.htDetectImg.Row = null;
            this.htDetectImg.Row1 = null;
            this.htDetectImg.Row2 = null;
            this.htDetectImg.Size = new System.Drawing.Size(385, 562);
            this.htDetectImg.TabIndex = 28;
            this.htDetectImg.UmPerPix = -1D;
            // 
            // htFullImg
            // 
            this.htFullImg.BackColor = System.Drawing.Color.Transparent;
            this.htFullImg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.htFullImg.ColorName = "red";
            this.htFullImg.ColorType = 0;
            this.htFullImg.Column = null;
            this.htFullImg.Column1 = null;
            this.htFullImg.Column2 = null;
            this.htFullImg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htFullImg.Image = null;
            this.htFullImg.Length1 = null;
            this.htFullImg.Length2 = null;
            this.htFullImg.Location = new System.Drawing.Point(0, 0);
            this.htFullImg.Name = "htFullImg";
            this.htFullImg.Phi = null;
            this.htFullImg.Radius = null;
            this.htFullImg.Radius1 = null;
            this.htFullImg.Radius2 = null;
            this.htFullImg.Region = null;
            this.htFullImg.RegionType = "";
            this.htFullImg.Row = null;
            this.htFullImg.Row1 = null;
            this.htFullImg.Row2 = null;
            this.htFullImg.Size = new System.Drawing.Size(362, 79);
            this.htFullImg.TabIndex = 16;
            this.htFullImg.UmPerPix = -1D;
            // 
            // chkShowAllItems
            // 
            this.chkShowAllItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShowAllItems.AutoSize = true;
            this.chkShowAllItems.Location = new System.Drawing.Point(970, 210);
            this.chkShowAllItems.Name = "chkShowAllItems";
            this.chkShowAllItems.Size = new System.Drawing.Size(108, 16);
            this.chkShowAllItems.TabIndex = 38;
            this.chkShowAllItems.Text = "显示所有检测项";
            this.chkShowAllItems.UseVisualStyleBackColor = true;
            this.chkShowAllItems.CheckedChanged += new System.EventHandler(this.chkShowAllItems_CheckedChanged);
            // 
            // UcOfflineDetectStation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UcOfflineDetectStation";
            this.Size = new System.Drawing.Size(1086, 677);
            this.Load += new System.EventHandler(this.UcOfflineDetectStation_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInspectResultsInFov)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btUpdateInspect;
        private System.Windows.Forms.Label lbSelectedInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox rchPicFolder;
        private System.Windows.Forms.Button btEditCancel;
        private System.Windows.Forms.Button btSetPicFolder;
        private System.Windows.Forms.Button btEditSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbLotID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbRecipeID;
        private System.Windows.Forms.ListBox lstBoxPieceIDs;
        private System.Windows.Forms.Label label1;
        private HTHalControl.HTWindowControl htDetectImg;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private HTHalControl.HTWindowControl htFullImg;
        private HTMappingControl.MappingControl mapDetectCells;
        private System.Windows.Forms.RichTextBox rchDetectInfo;
        private System.Windows.Forms.DataGridView dgvInspectResultsInFov;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbTask5;
        private System.Windows.Forms.RadioButton rbTask4;
        private System.Windows.Forms.RadioButton rbTask3;
        private System.Windows.Forms.RadioButton rbTask2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbTask1;
        private System.Windows.Forms.RadioButton rbTask0;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDescrib;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTaskName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDetail;
        private System.Windows.Forms.CheckBox chkShowAllItems;
    }
}
