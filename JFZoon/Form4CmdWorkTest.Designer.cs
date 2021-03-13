namespace JFZoon
{
    partial class Form4CmdWorkTest
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
            this.components = new System.ComponentModel.Container();
            this.btStart = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.btPause = new System.Windows.Forms.Button();
            this.btResume = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbWorkStatus = new System.Windows.Forms.Label();
            this.lbCustomStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btSendCmd = new System.Windows.Forms.Button();
            this.cbCmds = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lbCurrFrame = new System.Windows.Forms.Label();
            this.rchTips = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btClearInfo = new System.Windows.Forms.Button();
            this.lbID = new System.Windows.Forms.Label();
            this.timerFlush = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(12, 38);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(75, 23);
            this.btStart.TabIndex = 0;
            this.btStart.Text = "开始";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // btStop
            // 
            this.btStop.Location = new System.Drawing.Point(12, 67);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(75, 23);
            this.btStop.TabIndex = 1;
            this.btStop.Text = "停止";
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // btPause
            // 
            this.btPause.Location = new System.Drawing.Point(12, 96);
            this.btPause.Name = "btPause";
            this.btPause.Size = new System.Drawing.Size(75, 23);
            this.btPause.TabIndex = 2;
            this.btPause.Text = "暂停";
            this.btPause.UseVisualStyleBackColor = true;
            this.btPause.Click += new System.EventHandler(this.btPause_Click);
            // 
            // btResume
            // 
            this.btResume.Location = new System.Drawing.Point(12, 125);
            this.btResume.Name = "btResume";
            this.btResume.Size = new System.Drawing.Size(75, 23);
            this.btResume.TabIndex = 3;
            this.btResume.Text = "恢复";
            this.btResume.UseVisualStyleBackColor = true;
            this.btResume.Click += new System.EventHandler(this.btResume_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(152, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "当前工作状态";
            // 
            // lbWorkStatus
            // 
            this.lbWorkStatus.AutoSize = true;
            this.lbWorkStatus.Location = new System.Drawing.Point(251, 9);
            this.lbWorkStatus.Name = "lbWorkStatus";
            this.lbWorkStatus.Size = new System.Drawing.Size(41, 12);
            this.lbWorkStatus.TabIndex = 5;
            this.lbWorkStatus.Text = "未开始";
            // 
            // lbCustomStatus
            // 
            this.lbCustomStatus.AutoSize = true;
            this.lbCustomStatus.Location = new System.Drawing.Point(400, 9);
            this.lbCustomStatus.Name = "lbCustomStatus";
            this.lbCustomStatus.Size = new System.Drawing.Size(41, 12);
            this.lbCustomStatus.TabIndex = 7;
            this.lbCustomStatus.Text = "未开始";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(314, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "自定义状态";
            // 
            // btSendCmd
            // 
            this.btSendCmd.Location = new System.Drawing.Point(151, 38);
            this.btSendCmd.Name = "btSendCmd";
            this.btSendCmd.Size = new System.Drawing.Size(75, 23);
            this.btSendCmd.TabIndex = 8;
            this.btSendCmd.Text = "发送指令";
            this.btSendCmd.UseVisualStyleBackColor = true;
            this.btSendCmd.Click += new System.EventHandler(this.btSendCmd_Click);
            // 
            // cbCmds
            // 
            this.cbCmds.FormattingEnabled = true;
            this.cbCmds.Location = new System.Drawing.Point(244, 39);
            this.cbCmds.Name = "cbCmds";
            this.cbCmds.Size = new System.Drawing.Size(121, 20);
            this.cbCmds.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "当前播放帧：";
            // 
            // lbCurrFrame
            // 
            this.lbCurrFrame.AutoSize = true;
            this.lbCurrFrame.Location = new System.Drawing.Point(244, 67);
            this.lbCurrFrame.Name = "lbCurrFrame";
            this.lbCurrFrame.Size = new System.Drawing.Size(11, 12);
            this.lbCurrFrame.TabIndex = 11;
            this.lbCurrFrame.Text = "0";
            // 
            // rchTips
            // 
            this.rchTips.Location = new System.Drawing.Point(1, 170);
            this.rchTips.Name = "rchTips";
            this.rchTips.ReadOnly = true;
            this.rchTips.Size = new System.Drawing.Size(507, 99);
            this.rchTips.TabIndex = 12;
            this.rchTips.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "信息";
            // 
            // btClearInfo
            // 
            this.btClearInfo.Location = new System.Drawing.Point(433, 145);
            this.btClearInfo.Name = "btClearInfo";
            this.btClearInfo.Size = new System.Drawing.Size(75, 23);
            this.btClearInfo.TabIndex = 14;
            this.btClearInfo.Text = "清空信息";
            this.btClearInfo.UseVisualStyleBackColor = true;
            this.btClearInfo.Click += new System.EventHandler(this.btClearInfo_Click);
            // 
            // lbID
            // 
            this.lbID.AutoSize = true;
            this.lbID.Location = new System.Drawing.Point(12, 9);
            this.lbID.Name = "lbID";
            this.lbID.Size = new System.Drawing.Size(17, 12);
            this.lbID.TabIndex = 15;
            this.lbID.Text = "ID";
            // 
            // timerFlush
            // 
            this.timerFlush.Enabled = true;
            this.timerFlush.Tick += new System.EventHandler(this.timerFlush_Tick);
            // 
            // Form4CmdWorkTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 271);
            this.Controls.Add(this.lbID);
            this.Controls.Add(this.btClearInfo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rchTips);
            this.Controls.Add(this.lbCurrFrame);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbCmds);
            this.Controls.Add(this.btSendCmd);
            this.Controls.Add(this.lbCustomStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbWorkStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btResume);
            this.Controls.Add(this.btPause);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.btStart);
            this.Name = "Form4CmdWorkTest";
            this.Text = "CmdWorkDemo测试窗口";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form4CmdWorkTest_FormClosed);
            this.Load += new System.EventHandler(this.Form4CmdWorkTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Button btPause;
        private System.Windows.Forms.Button btResume;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbWorkStatus;
        private System.Windows.Forms.Label lbCustomStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btSendCmd;
        private System.Windows.Forms.ComboBox cbCmds;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbCurrFrame;
        private System.Windows.Forms.RichTextBox rchTips;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btClearInfo;
        private System.Windows.Forms.Label lbID;
        private System.Windows.Forms.Timer timerFlush;
    }
}