namespace JFUI
{
    partial class UcMotionDaq
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
            this.tabControlCF1 = new JFUI.TabControlCF();
            this.DevStatus = new System.Windows.Forms.TabPage();
            this.lbID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gbModuleInfo = new System.Windows.Forms.GroupBox();
            this.lbCmprTrigCount = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbAioCount = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbMotionCount = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbDioCount = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btOpenClose = new System.Windows.Forms.Button();
            this.lbOpend = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbModel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gbInitParams = new System.Windows.Forms.GroupBox();
            this.tabControlCF1.SuspendLayout();
            this.DevStatus.SuspendLayout();
            this.gbModuleInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlCF1
            // 
            this.tabControlCF1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControlCF1.Controls.Add(this.DevStatus);
            this.tabControlCF1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlCF1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlCF1.ItemSize = new System.Drawing.Size(20, 100);
            this.tabControlCF1.Location = new System.Drawing.Point(0, 0);
            this.tabControlCF1.Multiline = true;
            this.tabControlCF1.Name = "tabControlCF1";
            this.tabControlCF1.SelectedIndex = 0;
            this.tabControlCF1.Size = new System.Drawing.Size(1038, 564);
            this.tabControlCF1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlCF1.TabColor = System.Drawing.SystemColors.ControlDark;
            this.tabControlCF1.TabIndex = 0;
            // 
            // DevStatus
            // 
            this.DevStatus.BackColor = System.Drawing.SystemColors.Control;
            this.DevStatus.Controls.Add(this.lbID);
            this.DevStatus.Controls.Add(this.label2);
            this.DevStatus.Controls.Add(this.gbModuleInfo);
            this.DevStatus.Controls.Add(this.btOpenClose);
            this.DevStatus.Controls.Add(this.lbOpend);
            this.DevStatus.Controls.Add(this.label3);
            this.DevStatus.Controls.Add(this.lbModel);
            this.DevStatus.Controls.Add(this.label1);
            this.DevStatus.Controls.Add(this.gbInitParams);
            this.DevStatus.Location = new System.Drawing.Point(104, 4);
            this.DevStatus.Name = "DevStatus";
            this.DevStatus.Padding = new System.Windows.Forms.Padding(3);
            this.DevStatus.Size = new System.Drawing.Size(930, 556);
            this.DevStatus.TabIndex = 0;
            this.DevStatus.Text = "控制器状态";
            // 
            // lbID
            // 
            this.lbID.AutoSize = true;
            this.lbID.Location = new System.Drawing.Point(275, 7);
            this.lbID.Name = "lbID";
            this.lbID.Size = new System.Drawing.Size(29, 12);
            this.lbID.TabIndex = 7;
            this.lbID.Text = "未知";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(246, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "ID:";
            // 
            // gbModuleInfo
            // 
            this.gbModuleInfo.Controls.Add(this.lbCmprTrigCount);
            this.gbModuleInfo.Controls.Add(this.label7);
            this.gbModuleInfo.Controls.Add(this.lbAioCount);
            this.gbModuleInfo.Controls.Add(this.label8);
            this.gbModuleInfo.Controls.Add(this.lbMotionCount);
            this.gbModuleInfo.Controls.Add(this.label6);
            this.gbModuleInfo.Controls.Add(this.lbDioCount);
            this.gbModuleInfo.Controls.Add(this.label5);
            this.gbModuleInfo.Location = new System.Drawing.Point(702, 27);
            this.gbModuleInfo.Name = "gbModuleInfo";
            this.gbModuleInfo.Size = new System.Drawing.Size(221, 520);
            this.gbModuleInfo.TabIndex = 1;
            this.gbModuleInfo.TabStop = false;
            this.gbModuleInfo.Text = "模块信息";
            // 
            // lbCmprTrigCount
            // 
            this.lbCmprTrigCount.AutoSize = true;
            this.lbCmprTrigCount.Location = new System.Drawing.Point(111, 106);
            this.lbCmprTrigCount.Name = "lbCmprTrigCount";
            this.lbCmprTrigCount.Size = new System.Drawing.Size(11, 12);
            this.lbCmprTrigCount.TabIndex = 15;
            this.lbCmprTrigCount.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "比较触发模块数量:";
            // 
            // lbAioCount
            // 
            this.lbAioCount.AutoSize = true;
            this.lbAioCount.Location = new System.Drawing.Point(85, 78);
            this.lbAioCount.Name = "lbAioCount";
            this.lbAioCount.Size = new System.Drawing.Size(11, 12);
            this.lbAioCount.TabIndex = 13;
            this.lbAioCount.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "AIO模块数量:";
            // 
            // lbMotionCount
            // 
            this.lbMotionCount.AutoSize = true;
            this.lbMotionCount.Location = new System.Drawing.Point(85, 51);
            this.lbMotionCount.Name = "lbMotionCount";
            this.lbMotionCount.Size = new System.Drawing.Size(11, 12);
            this.lbMotionCount.TabIndex = 11;
            this.lbMotionCount.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "轴模块数量:";
            // 
            // lbDioCount
            // 
            this.lbDioCount.AutoSize = true;
            this.lbDioCount.Location = new System.Drawing.Point(85, 26);
            this.lbDioCount.Name = "lbDioCount";
            this.lbDioCount.Size = new System.Drawing.Size(11, 12);
            this.lbDioCount.TabIndex = 9;
            this.lbDioCount.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "DIO模块数量:";
            // 
            // btOpenClose
            // 
            this.btOpenClose.Location = new System.Drawing.Point(850, 2);
            this.btOpenClose.Name = "btOpenClose";
            this.btOpenClose.Size = new System.Drawing.Size(75, 23);
            this.btOpenClose.TabIndex = 5;
            this.btOpenClose.Text = "打开设备";
            this.btOpenClose.UseVisualStyleBackColor = true;
            this.btOpenClose.Click += new System.EventHandler(this.btOpenClose_Click);
            // 
            // lbOpend
            // 
            this.lbOpend.AutoSize = true;
            this.lbOpend.Location = new System.Drawing.Point(770, 7);
            this.lbOpend.Name = "lbOpend";
            this.lbOpend.Size = new System.Drawing.Size(41, 12);
            this.lbOpend.TabIndex = 4;
            this.lbOpend.Text = "已关闭";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(707, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "设备状态:";
            // 
            // lbModel
            // 
            this.lbModel.AutoSize = true;
            this.lbModel.Location = new System.Drawing.Point(87, 7);
            this.lbModel.Name = "lbModel";
            this.lbModel.Size = new System.Drawing.Size(29, 12);
            this.lbModel.TabIndex = 2;
            this.lbModel.Text = "未知";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "控制器型号:";
            // 
            // gbInitParams
            // 
            this.gbInitParams.Location = new System.Drawing.Point(4, 27);
            this.gbInitParams.Name = "gbInitParams";
            this.gbInitParams.Size = new System.Drawing.Size(692, 520);
            this.gbInitParams.TabIndex = 0;
            this.gbInitParams.TabStop = false;
            this.gbInitParams.Text = "初始化参数";
            // 
            // UcMotionDaq
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlCF1);
            this.Name = "UcMotionDaq";
            this.Size = new System.Drawing.Size(1038, 564);
            this.Load += new System.EventHandler(this.UcMotionDaq_Load);
            this.tabControlCF1.ResumeLayout(false);
            this.DevStatus.ResumeLayout(false);
            this.DevStatus.PerformLayout();
            this.gbModuleInfo.ResumeLayout(false);
            this.gbModuleInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TabControlCF tabControlCF1;
        private System.Windows.Forms.TabPage DevStatus;
        private System.Windows.Forms.GroupBox gbInitParams;
        private System.Windows.Forms.GroupBox gbModuleInfo;
        private System.Windows.Forms.Button btOpenClose;
        private System.Windows.Forms.Label lbOpend;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbModel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbMotionCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbDioCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbAioCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lbCmprTrigCount;
        private System.Windows.Forms.Label label7;
    }
}
