namespace JFHub
{
    partial class UcSingleVisionAssist
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
            this.components = new System.ComponentModel.Container();
            this.lbAssistInfo = new System.Windows.Forms.Label();
            this.rchTips = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAllProgramNames = new System.Windows.Forms.ComboBox();
            this.gbCmrCfg = new System.Windows.Forms.GroupBox();
            this.numGain = new System.Windows.Forms.NumericUpDown();
            this.chkContinueSnap = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numExposure = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.chkReverseY = new System.Windows.Forms.CheckBox();
            this.chkReverseX = new System.Windows.Forms.CheckBox();
            this.PnDevs = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.btSnapByCurrDevParam = new System.Windows.Forms.Button();
            this.dgvCfgParam = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btSnapUseCfg = new System.Windows.Forms.Button();
            this.btSaveCurrDevParam2Cfg = new System.Windows.Forms.Button();
            this.btSaveAs = new System.Windows.Forms.Button();
            this.btShowTestMethodFlowDialog = new System.Windows.Forms.Button();
            this.btOpenAllDev = new System.Windows.Forms.Button();
            this.btInitialize = new System.Windows.Forms.Button();
            this.timerFlush = new System.Windows.Forms.Timer(this.components);
            this.btClearTips = new System.Windows.Forms.Button();
            this.btSaveCurrCfgAs = new System.Windows.Forms.Button();
            this.btSaveImgAs = new System.Windows.Forms.Button();
            this.cbImgFileFormat = new System.Windows.Forms.ComboBox();
            this.htWindowControl1 = new HTHalControl.HTWindowControl();
            this.gbCmrCfg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExposure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCfgParam)).BeginInit();
            this.SuspendLayout();
            // 
            // lbAssistInfo
            // 
            this.lbAssistInfo.AutoSize = true;
            this.lbAssistInfo.Location = new System.Drawing.Point(6, 6);
            this.lbAssistInfo.Name = "lbAssistInfo";
            this.lbAssistInfo.Size = new System.Drawing.Size(197, 12);
            this.lbAssistInfo.TabIndex = 0;
            this.lbAssistInfo.Text = "示教助手:未设置    配置名:未命名";
            // 
            // rchTips
            // 
            this.rchTips.BackColor = System.Drawing.SystemColors.Control;
            this.rchTips.Location = new System.Drawing.Point(3, 481);
            this.rchTips.Name = "rchTips";
            this.rchTips.ReadOnly = true;
            this.rchTips.Size = new System.Drawing.Size(617, 72);
            this.rchTips.TabIndex = 2;
            this.rchTips.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(807, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "选择配置项:";
            // 
            // cbAllProgramNames
            // 
            this.cbAllProgramNames.FormattingEnabled = true;
            this.cbAllProgramNames.Location = new System.Drawing.Point(884, 5);
            this.cbAllProgramNames.Name = "cbAllProgramNames";
            this.cbAllProgramNames.Size = new System.Drawing.Size(244, 20);
            this.cbAllProgramNames.TabIndex = 4;
            this.cbAllProgramNames.SelectedIndexChanged += new System.EventHandler(this.cbAllProgramNames_SelectedIndexChanged);
            // 
            // gbCmrCfg
            // 
            this.gbCmrCfg.Controls.Add(this.numGain);
            this.gbCmrCfg.Controls.Add(this.chkContinueSnap);
            this.gbCmrCfg.Controls.Add(this.label3);
            this.gbCmrCfg.Controls.Add(this.numExposure);
            this.gbCmrCfg.Controls.Add(this.label2);
            this.gbCmrCfg.Controls.Add(this.chkReverseY);
            this.gbCmrCfg.Controls.Add(this.chkReverseX);
            this.gbCmrCfg.Location = new System.Drawing.Point(623, 6);
            this.gbCmrCfg.Name = "gbCmrCfg";
            this.gbCmrCfg.Size = new System.Drawing.Size(182, 146);
            this.gbCmrCfg.TabIndex = 6;
            this.gbCmrCfg.TabStop = false;
            this.gbCmrCfg.Text = "当前相机参数设置";
            // 
            // numGain
            // 
            this.numGain.Location = new System.Drawing.Point(68, 93);
            this.numGain.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numGain.Name = "numGain";
            this.numGain.Size = new System.Drawing.Size(100, 21);
            this.numGain.TabIndex = 10;
            this.numGain.ValueChanged += new System.EventHandler(this.numGain_ValueChanged);
            // 
            // chkContinueSnap
            // 
            this.chkContinueSnap.AutoSize = true;
            this.chkContinueSnap.Location = new System.Drawing.Point(8, 124);
            this.chkContinueSnap.Name = "chkContinueSnap";
            this.chkContinueSnap.Size = new System.Drawing.Size(156, 16);
            this.chkContinueSnap.TabIndex = 12;
            this.chkContinueSnap.Text = "连续拍照模式(非配置项)";
            this.chkContinueSnap.UseVisualStyleBackColor = true;
            this.chkContinueSnap.CheckedChanged += new System.EventHandler(this.chkContinueSnap_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "增益参数";
            // 
            // numExposure
            // 
            this.numExposure.Location = new System.Drawing.Point(68, 68);
            this.numExposure.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numExposure.Name = "numExposure";
            this.numExposure.Size = new System.Drawing.Size(100, 21);
            this.numExposure.TabIndex = 8;
            this.numExposure.ValueChanged += new System.EventHandler(this.numExposure_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "曝光时长:";
            // 
            // chkReverseY
            // 
            this.chkReverseY.AutoSize = true;
            this.chkReverseY.Location = new System.Drawing.Point(7, 44);
            this.chkReverseY.Name = "chkReverseY";
            this.chkReverseY.Size = new System.Drawing.Size(90, 16);
            this.chkReverseY.TabIndex = 1;
            this.chkReverseY.Text = "Y轴镜像使能";
            this.chkReverseY.UseVisualStyleBackColor = true;
            this.chkReverseY.CheckedChanged += new System.EventHandler(this.chkReverseY_CheckedChanged);
            // 
            // chkReverseX
            // 
            this.chkReverseX.AutoSize = true;
            this.chkReverseX.Location = new System.Drawing.Point(7, 20);
            this.chkReverseX.Name = "chkReverseX";
            this.chkReverseX.Size = new System.Drawing.Size(90, 16);
            this.chkReverseX.TabIndex = 0;
            this.chkReverseX.Text = "X轴镜像使能";
            this.chkReverseX.UseVisualStyleBackColor = true;
            this.chkReverseX.CheckedChanged += new System.EventHandler(this.chkReverseX_CheckedChanged);
            // 
            // PnDevs
            // 
            this.PnDevs.AutoScroll = true;
            this.PnDevs.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.PnDevs.ColumnCount = 1;
            this.PnDevs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.PnDevs.Location = new System.Drawing.Point(622, 179);
            this.PnDevs.Name = "PnDevs";
            this.PnDevs.RowCount = 1;
            this.PnDevs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.PnDevs.Size = new System.Drawing.Size(507, 300);
            this.PnDevs.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(627, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "轴/光源/触发器操作面板";
            // 
            // btSnapByCurrDevParam
            // 
            this.btSnapByCurrDevParam.Location = new System.Drawing.Point(808, 154);
            this.btSnapByCurrDevParam.Name = "btSnapByCurrDevParam";
            this.btSnapByCurrDevParam.Size = new System.Drawing.Size(162, 23);
            this.btSnapByCurrDevParam.TabIndex = 11;
            this.btSnapByCurrDevParam.Text = "使用设备当前参数拍照";
            this.btSnapByCurrDevParam.UseVisualStyleBackColor = true;
            this.btSnapByCurrDevParam.Click += new System.EventHandler(this.btSnapByCurrDevParam_Click);
            // 
            // dgvCfgParam
            // 
            this.dgvCfgParam.AllowUserToAddRows = false;
            this.dgvCfgParam.AllowUserToDeleteRows = false;
            this.dgvCfgParam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCfgParam.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.dgvCfgParam.Location = new System.Drawing.Point(808, 31);
            this.dgvCfgParam.Name = "dgvCfgParam";
            this.dgvCfgParam.ReadOnly = true;
            this.dgvCfgParam.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvCfgParam.RowHeadersVisible = false;
            this.dgvCfgParam.RowTemplate.Height = 23;
            this.dgvCfgParam.Size = new System.Drawing.Size(320, 121);
            this.dgvCfgParam.TabIndex = 13;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "参数名称";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "预设值";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 80;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "当前值";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 80;
            // 
            // btSnapUseCfg
            // 
            this.btSnapUseCfg.Location = new System.Drawing.Point(976, 154);
            this.btSnapUseCfg.Name = "btSnapUseCfg";
            this.btSnapUseCfg.Size = new System.Drawing.Size(152, 23);
            this.btSnapUseCfg.TabIndex = 14;
            this.btSnapUseCfg.Text = "使用预设示教参数拍照";
            this.btSnapUseCfg.UseVisualStyleBackColor = true;
            this.btSnapUseCfg.Click += new System.EventHandler(this.btSnapUseCfg_Click);
            // 
            // btSaveCurrDevParam2Cfg
            // 
            this.btSaveCurrDevParam2Cfg.Location = new System.Drawing.Point(947, 481);
            this.btSaveCurrDevParam2Cfg.Name = "btSaveCurrDevParam2Cfg";
            this.btSaveCurrDevParam2Cfg.Size = new System.Drawing.Size(182, 23);
            this.btSaveCurrDevParam2Cfg.TabIndex = 15;
            this.btSaveCurrDevParam2Cfg.Text = "保存设备参数到配置项";
            this.btSaveCurrDevParam2Cfg.UseVisualStyleBackColor = true;
            this.btSaveCurrDevParam2Cfg.Click += new System.EventHandler(this.btSaveCurrDevParam2Cfg_Click);
            // 
            // btSaveAs
            // 
            this.btSaveAs.Location = new System.Drawing.Point(947, 505);
            this.btSaveAs.Name = "btSaveAs";
            this.btSaveAs.Size = new System.Drawing.Size(182, 23);
            this.btSaveAs.TabIndex = 16;
            this.btSaveAs.Text = "将当前设备参数另存为";
            this.btSaveAs.UseVisualStyleBackColor = true;
            this.btSaveAs.Click += new System.EventHandler(this.btSaveAs_Click);
            // 
            // btShowTestMethodFlowDialog
            // 
            this.btShowTestMethodFlowDialog.Location = new System.Drawing.Point(623, 505);
            this.btShowTestMethodFlowDialog.Name = "btShowTestMethodFlowDialog";
            this.btShowTestMethodFlowDialog.Size = new System.Drawing.Size(128, 23);
            this.btShowTestMethodFlowDialog.TabIndex = 17;
            this.btShowTestMethodFlowDialog.Text = "显示测试流程窗口";
            this.btShowTestMethodFlowDialog.UseVisualStyleBackColor = true;
            this.btShowTestMethodFlowDialog.Click += new System.EventHandler(this.btShowTestMethodFlowDialog_Click);
            // 
            // btOpenAllDev
            // 
            this.btOpenAllDev.Location = new System.Drawing.Point(623, 481);
            this.btOpenAllDev.Name = "btOpenAllDev";
            this.btOpenAllDev.Size = new System.Drawing.Size(128, 23);
            this.btOpenAllDev.TabIndex = 18;
            this.btOpenAllDev.Text = "打开/使能所有设备";
            this.btOpenAllDev.UseVisualStyleBackColor = true;
            this.btOpenAllDev.Click += new System.EventHandler(this.btOpenAllDev_Click);
            // 
            // btInitialize
            // 
            this.btInitialize.Location = new System.Drawing.Point(488, 2);
            this.btInitialize.Name = "btInitialize";
            this.btInitialize.Size = new System.Drawing.Size(132, 23);
            this.btInitialize.TabIndex = 20;
            this.btInitialize.Text = "初始化/参数设置";
            this.btInitialize.UseVisualStyleBackColor = true;
            this.btInitialize.Click += new System.EventHandler(this.btInitialize_Click);
            // 
            // timerFlush
            // 
            this.timerFlush.Enabled = true;
            this.timerFlush.Tick += new System.EventHandler(this.timerFlush_Tick);
            // 
            // btClearTips
            // 
            this.btClearTips.Location = new System.Drawing.Point(622, 530);
            this.btClearTips.Name = "btClearTips";
            this.btClearTips.Size = new System.Drawing.Size(129, 23);
            this.btClearTips.TabIndex = 21;
            this.btClearTips.Text = "清空提示信息";
            this.btClearTips.UseVisualStyleBackColor = true;
            this.btClearTips.Click += new System.EventHandler(this.btClearTips_Click);
            // 
            // btSaveCurrCfgAs
            // 
            this.btSaveCurrCfgAs.Location = new System.Drawing.Point(947, 530);
            this.btSaveCurrCfgAs.Name = "btSaveCurrCfgAs";
            this.btSaveCurrCfgAs.Size = new System.Drawing.Size(182, 23);
            this.btSaveCurrCfgAs.TabIndex = 22;
            this.btSaveCurrCfgAs.Text = "将当前配置参数另存为";
            this.btSaveCurrCfgAs.UseVisualStyleBackColor = true;
            this.btSaveCurrCfgAs.Click += new System.EventHandler(this.btSaveCurrCfgAs_Click);
            // 
            // btSaveImgAs
            // 
            this.btSaveImgAs.Location = new System.Drawing.Point(764, 481);
            this.btSaveImgAs.Name = "btSaveImgAs";
            this.btSaveImgAs.Size = new System.Drawing.Size(87, 23);
            this.btSaveImgAs.TabIndex = 23;
            this.btSaveImgAs.Text = "拍照并保存";
            this.btSaveImgAs.UseVisualStyleBackColor = true;
            this.btSaveImgAs.Click += new System.EventHandler(this.btSaveImgAs_Click);
            // 
            // cbImgFileFormat
            // 
            this.cbImgFileFormat.FormattingEnabled = true;
            this.cbImgFileFormat.Items.AddRange(new object[] {
            "BMP",
            "JPG",
            "PNG",
            "TIF"});
            this.cbImgFileFormat.Location = new System.Drawing.Point(858, 482);
            this.cbImgFileFormat.Name = "cbImgFileFormat";
            this.cbImgFileFormat.Size = new System.Drawing.Size(61, 20);
            this.cbImgFileFormat.TabIndex = 24;
            // 
            // htWindowControl1
            // 
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
            this.htWindowControl1.Location = new System.Drawing.Point(3, 21);
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
            this.htWindowControl1.Size = new System.Drawing.Size(617, 458);
            this.htWindowControl1.TabIndex = 25;
            this.htWindowControl1.UmPerPix = -1D;
            // 
            // UcSingleVisionAssist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.htWindowControl1);
            this.Controls.Add(this.cbImgFileFormat);
            this.Controls.Add(this.btSaveImgAs);
            this.Controls.Add(this.btSaveCurrCfgAs);
            this.Controls.Add(this.btClearTips);
            this.Controls.Add(this.btInitialize);
            this.Controls.Add(this.btOpenAllDev);
            this.Controls.Add(this.btShowTestMethodFlowDialog);
            this.Controls.Add(this.btSaveAs);
            this.Controls.Add(this.btSaveCurrDevParam2Cfg);
            this.Controls.Add(this.btSnapUseCfg);
            this.Controls.Add(this.dgvCfgParam);
            this.Controls.Add(this.btSnapByCurrDevParam);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PnDevs);
            this.Controls.Add(this.gbCmrCfg);
            this.Controls.Add(this.cbAllProgramNames);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rchTips);
            this.Controls.Add(this.lbAssistInfo);
            this.Name = "UcSingleVisionAssist";
            this.Size = new System.Drawing.Size(1135, 558);
            this.Load += new System.EventHandler(this.UcSingleVisionAssist_Load);
            this.VisibleChanged += new System.EventHandler(this.UcSingleVisionAssist_VisibleChanged);
            this.gbCmrCfg.ResumeLayout(false);
            this.gbCmrCfg.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExposure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCfgParam)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbAssistInfo;
        private System.Windows.Forms.RichTextBox rchTips;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbAllProgramNames;
        private System.Windows.Forms.GroupBox gbCmrCfg;
        private System.Windows.Forms.NumericUpDown numGain;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numExposure;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkReverseY;
        private System.Windows.Forms.CheckBox chkReverseX;
        private System.Windows.Forms.TableLayoutPanel PnDevs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btSnapByCurrDevParam;
        private System.Windows.Forms.CheckBox chkContinueSnap;
        private System.Windows.Forms.DataGridView dgvCfgParam;
        private System.Windows.Forms.Button btSnapUseCfg;
        private System.Windows.Forms.Button btSaveCurrDevParam2Cfg;
        private System.Windows.Forms.Button btSaveAs;
        private System.Windows.Forms.Button btShowTestMethodFlowDialog;
        private System.Windows.Forms.Button btOpenAllDev;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.Button btInitialize;
        private System.Windows.Forms.Timer timerFlush;
        private System.Windows.Forms.Button btClearTips;
        private System.Windows.Forms.Button btSaveCurrCfgAs;
        private System.Windows.Forms.Button btSaveImgAs;
        private System.Windows.Forms.ComboBox cbImgFileFormat;
        private HTHalControl.HTWindowControl htWindowControl1;
    }
}
