namespace JFVision
{
    partial class UcCmr
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
            this.picBox = new System.Windows.Forms.PictureBox();
            this.rbTips = new System.Windows.Forms.RichTextBox();
            this.btClearTips = new System.Windows.Forms.Button();
            this.btDev = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbImgDispMode = new System.Windows.Forms.ComboBox();
            this.btGrab = new System.Windows.Forms.Button();
            this.btGrabOne = new System.Windows.Forms.Button();
            this.btSoftwareTrig = new System.Windows.Forms.Button();
            this.chkContinueGrab = new System.Windows.Forms.CheckBox();
            this.btSave = new System.Windows.Forms.Button();
            this.cbImgFileFormat = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkCallBack = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbTrigSrc = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numBuffSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numExposure = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numGain = new System.Windows.Forms.NumericUpDown();
            this.chkReverseX = new System.Windows.Forms.CheckBox();
            this.chkReverseY = new System.Windows.Forms.CheckBox();
            this.gbParam = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuffSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExposure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGain)).BeginInit();
            this.gbParam.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picBox
            // 
            this.picBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.picBox.Location = new System.Drawing.Point(0, 0);
            this.picBox.Name = "picBox";
            this.picBox.Size = new System.Drawing.Size(640, 480);
            this.picBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBox.TabIndex = 0;
            this.picBox.TabStop = false;
            this.picBox.SizeChanged += new System.EventHandler(this.picBox_SizeChanged);
            this.picBox.Paint += new System.Windows.Forms.PaintEventHandler(this.picBox_Paint);
            // 
            // rbTips
            // 
            this.rbTips.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbTips.BackColor = System.Drawing.SystemColors.Control;
            this.rbTips.Location = new System.Drawing.Point(0, 481);
            this.rbTips.Name = "rbTips";
            this.rbTips.ReadOnly = true;
            this.rbTips.Size = new System.Drawing.Size(830, 47);
            this.rbTips.TabIndex = 1;
            this.rbTips.Text = "";
            // 
            // btClearTips
            // 
            this.btClearTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btClearTips.Location = new System.Drawing.Point(756, 457);
            this.btClearTips.Name = "btClearTips";
            this.btClearTips.Size = new System.Drawing.Size(74, 23);
            this.btClearTips.TabIndex = 2;
            this.btClearTips.Text = "清空信息";
            this.btClearTips.UseVisualStyleBackColor = true;
            this.btClearTips.Click += new System.EventHandler(this.btClearTips_Click);
            // 
            // btDev
            // 
            this.btDev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDev.Location = new System.Drawing.Point(646, 3);
            this.btDev.Name = "btDev";
            this.btDev.Size = new System.Drawing.Size(63, 23);
            this.btDev.TabIndex = 3;
            this.btDev.Text = "打开相机";
            this.btDev.UseVisualStyleBackColor = true;
            this.btDev.Click += new System.EventHandler(this.btDev_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(718, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "显示模式";
            // 
            // cbImgDispMode
            // 
            this.cbImgDispMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbImgDispMode.FormattingEnabled = true;
            this.cbImgDispMode.Items.AddRange(new object[] {
            "sdk",
            "halcon",
            "bitmap"});
            this.cbImgDispMode.Location = new System.Drawing.Point(772, 6);
            this.cbImgDispMode.Name = "cbImgDispMode";
            this.cbImgDispMode.Size = new System.Drawing.Size(56, 20);
            this.cbImgDispMode.TabIndex = 7;
            // 
            // btGrab
            // 
            this.btGrab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btGrab.Location = new System.Drawing.Point(92, 14);
            this.btGrab.Name = "btGrab";
            this.btGrab.Size = new System.Drawing.Size(86, 23);
            this.btGrab.TabIndex = 8;
            this.btGrab.Text = "开始采集";
            this.btGrab.UseVisualStyleBackColor = true;
            this.btGrab.Click += new System.EventHandler(this.btGrab_Click);
            // 
            // btGrabOne
            // 
            this.btGrabOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btGrabOne.Location = new System.Drawing.Point(3, 53);
            this.btGrabOne.Name = "btGrabOne";
            this.btGrabOne.Size = new System.Drawing.Size(88, 23);
            this.btGrabOne.TabIndex = 9;
            this.btGrabOne.Text = "采集一张图片";
            this.btGrabOne.UseVisualStyleBackColor = true;
            this.btGrabOne.Click += new System.EventHandler(this.btGrabOne_Click);
            // 
            // btSoftwareTrig
            // 
            this.btSoftwareTrig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSoftwareTrig.Location = new System.Drawing.Point(92, 53);
            this.btSoftwareTrig.Name = "btSoftwareTrig";
            this.btSoftwareTrig.Size = new System.Drawing.Size(88, 23);
            this.btSoftwareTrig.TabIndex = 10;
            this.btSoftwareTrig.Text = "软触发一次";
            this.btSoftwareTrig.UseVisualStyleBackColor = true;
            this.btSoftwareTrig.Click += new System.EventHandler(this.btSoftwareTrig_Click);
            // 
            // chkContinueGrab
            // 
            this.chkContinueGrab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkContinueGrab.AutoSize = true;
            this.chkContinueGrab.Location = new System.Drawing.Point(11, 12);
            this.chkContinueGrab.Name = "chkContinueGrab";
            this.chkContinueGrab.Size = new System.Drawing.Size(72, 16);
            this.chkContinueGrab.TabIndex = 11;
            this.chkContinueGrab.Text = "连续模式";
            this.chkContinueGrab.UseVisualStyleBackColor = true;
            // 
            // btSave
            // 
            this.btSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSave.Location = new System.Drawing.Point(791, 297);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(37, 23);
            this.btSave.TabIndex = 13;
            this.btSave.Text = "保存";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSaveImage_Click);
            // 
            // cbImgFileFormat
            // 
            this.cbImgFileFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbImgFileFormat.FormattingEnabled = true;
            this.cbImgFileFormat.Items.AddRange(new object[] {
            "Bmp",
            "Jpg",
            "Png",
            "Tif"});
            this.cbImgFileFormat.Location = new System.Drawing.Point(701, 299);
            this.cbImgFileFormat.Name = "cbImgFileFormat";
            this.cbImgFileFormat.Size = new System.Drawing.Size(84, 20);
            this.cbImgFileFormat.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(646, 303);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "图片格式";
            // 
            // chkCallBack
            // 
            this.chkCallBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCallBack.AutoSize = true;
            this.chkCallBack.Location = new System.Drawing.Point(11, 28);
            this.chkCallBack.Name = "chkCallBack";
            this.chkCallBack.Size = new System.Drawing.Size(72, 16);
            this.chkCallBack.TabIndex = 14;
            this.chkCallBack.Text = "使用回调";
            this.chkCallBack.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "触发模式";
            // 
            // cbTrigSrc
            // 
            this.cbTrigSrc.Enabled = false;
            this.cbTrigSrc.FormattingEnabled = true;
            this.cbTrigSrc.Items.AddRange(new object[] {
            "禁用触发",
            "软件触发",
            "硬触发Line0",
            "硬触发Line1",
            "硬触发Line2",
            "硬触发Line3"});
            this.cbTrigSrc.Location = new System.Drawing.Point(69, 20);
            this.cbTrigSrc.Name = "cbTrigSrc";
            this.cbTrigSrc.Size = new System.Drawing.Size(104, 20);
            this.cbTrigSrc.TabIndex = 5;
            this.cbTrigSrc.SelectedIndexChanged += new System.EventHandler(this.cbTrigSrc_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "缓冲帧数";
            // 
            // numBuffSize
            // 
            this.numBuffSize.Enabled = false;
            this.numBuffSize.Location = new System.Drawing.Point(70, 46);
            this.numBuffSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numBuffSize.Name = "numBuffSize";
            this.numBuffSize.Size = new System.Drawing.Size(103, 21);
            this.numBuffSize.TabIndex = 7;
            this.numBuffSize.ValueChanged += new System.EventHandler(this.numBuffSize_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "曝光";
            // 
            // numExposure
            // 
            this.numExposure.Enabled = false;
            this.numExposure.Location = new System.Drawing.Point(70, 74);
            this.numExposure.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numExposure.Name = "numExposure";
            this.numExposure.Size = new System.Drawing.Size(103, 21);
            this.numExposure.TabIndex = 9;
            this.numExposure.ValueChanged += new System.EventHandler(this.numExposure_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "增益";
            // 
            // numGain
            // 
            this.numGain.Enabled = false;
            this.numGain.Location = new System.Drawing.Point(69, 104);
            this.numGain.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numGain.Name = "numGain";
            this.numGain.Size = new System.Drawing.Size(103, 21);
            this.numGain.TabIndex = 11;
            this.numGain.ValueChanged += new System.EventHandler(this.numGain_ValueChanged);
            // 
            // chkReverseX
            // 
            this.chkReverseX.AutoSize = true;
            this.chkReverseX.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkReverseX.Enabled = false;
            this.chkReverseX.Location = new System.Drawing.Point(34, 140);
            this.chkReverseX.Name = "chkReverseX";
            this.chkReverseX.Size = new System.Drawing.Size(54, 16);
            this.chkReverseX.TabIndex = 14;
            this.chkReverseX.Text = "X镜像";
            this.chkReverseX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkReverseX.UseVisualStyleBackColor = true;
            this.chkReverseX.CheckedChanged += new System.EventHandler(this.chkReverseX_CheckedChanged);
            // 
            // chkReverseY
            // 
            this.chkReverseY.AutoSize = true;
            this.chkReverseY.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkReverseY.Enabled = false;
            this.chkReverseY.Location = new System.Drawing.Point(118, 140);
            this.chkReverseY.Name = "chkReverseY";
            this.chkReverseY.Size = new System.Drawing.Size(54, 16);
            this.chkReverseY.TabIndex = 15;
            this.chkReverseY.Text = "Y镜像";
            this.chkReverseY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkReverseY.UseVisualStyleBackColor = true;
            this.chkReverseY.CheckedChanged += new System.EventHandler(this.chkReverseY_CheckedChanged);
            // 
            // gbParam
            // 
            this.gbParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbParam.Controls.Add(this.chkReverseY);
            this.gbParam.Controls.Add(this.chkReverseX);
            this.gbParam.Controls.Add(this.numGain);
            this.gbParam.Controls.Add(this.label5);
            this.gbParam.Controls.Add(this.numExposure);
            this.gbParam.Controls.Add(this.label4);
            this.gbParam.Controls.Add(this.numBuffSize);
            this.gbParam.Controls.Add(this.label3);
            this.gbParam.Controls.Add(this.cbTrigSrc);
            this.gbParam.Controls.Add(this.label1);
            this.gbParam.Location = new System.Drawing.Point(646, 34);
            this.gbParam.Name = "gbParam";
            this.gbParam.Size = new System.Drawing.Size(182, 166);
            this.gbParam.TabIndex = 12;
            this.gbParam.TabStop = false;
            this.gbParam.Text = "相机参数";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btGrab);
            this.groupBox1.Controls.Add(this.chkCallBack);
            this.groupBox1.Controls.Add(this.chkContinueGrab);
            this.groupBox1.Controls.Add(this.btGrabOne);
            this.groupBox1.Controls.Add(this.btSoftwareTrig);
            this.groupBox1.Location = new System.Drawing.Point(646, 201);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 84);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // UcCmr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbImgFileFormat);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.gbParam);
            this.Controls.Add(this.cbImgDispMode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btDev);
            this.Controls.Add(this.btClearTips);
            this.Controls.Add(this.rbTips);
            this.Controls.Add(this.picBox);
            this.Name = "UcCmr";
            this.Size = new System.Drawing.Size(833, 528);
            this.Load += new System.EventHandler(this.UcCmr_Load);
            this.SizeChanged += new System.EventHandler(this.UcCmr_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.UcCmr_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBuffSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExposure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGain)).EndInit();
            this.gbParam.ResumeLayout(false);
            this.gbParam.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picBox;
        private System.Windows.Forms.RichTextBox rbTips;
        private System.Windows.Forms.Button btClearTips;
        private System.Windows.Forms.Button btDev;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbImgDispMode;
        private System.Windows.Forms.Button btGrab;
        private System.Windows.Forms.Button btGrabOne;
        private System.Windows.Forms.Button btSoftwareTrig;
        private System.Windows.Forms.CheckBox chkContinueGrab;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.ComboBox cbImgFileFormat;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkCallBack;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbTrigSrc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numBuffSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numExposure;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numGain;
        private System.Windows.Forms.CheckBox chkReverseX;
        private System.Windows.Forms.CheckBox chkReverseY;
        private System.Windows.Forms.GroupBox gbParam;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
