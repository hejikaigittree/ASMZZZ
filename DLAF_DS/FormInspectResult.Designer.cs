namespace DLAF_DS
{
    partial class FormInspectResult
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
            this.htWindowControl1 = new HTHalControl.HTWindowControl();
            this.label3 = new System.Windows.Forms.Label();
            this.lbErrorCode = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbErrorInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // htWindowControl1
            // 
            this.htWindowControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
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
            this.htWindowControl1.Location = new System.Drawing.Point(3, 2);
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
            this.htWindowControl1.Size = new System.Drawing.Size(795, 495);
            this.htWindowControl1.TabIndex = 0;
            this.htWindowControl1.UmPerPix = -1D;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 504);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "检测结果:";
            // 
            // lbErrorCode
            // 
            this.lbErrorCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbErrorCode.AutoSize = true;
            this.lbErrorCode.Location = new System.Drawing.Point(62, 504);
            this.lbErrorCode.Name = "lbErrorCode";
            this.lbErrorCode.Size = new System.Drawing.Size(53, 12);
            this.lbErrorCode.TabIndex = 4;
            this.lbErrorCode.Text = "None-Opt";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 523);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "检测信息:";
            // 
            // lbErrorInfo
            // 
            this.lbErrorInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbErrorInfo.AutoSize = true;
            this.lbErrorInfo.Location = new System.Drawing.Point(64, 523);
            this.lbErrorInfo.Name = "lbErrorInfo";
            this.lbErrorInfo.Size = new System.Drawing.Size(53, 12);
            this.lbErrorInfo.TabIndex = 6;
            this.lbErrorInfo.Text = "None-Opt";
            // 
            // FormInspectResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 546);
            this.Controls.Add(this.lbErrorInfo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lbErrorCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.htWindowControl1);
            this.Name = "FormInspectResult";
            this.Text = "测试结果显示";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormInspectResult_FormClosing);
            this.Load += new System.EventHandler(this.FormInspectResult_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HTHalControl.HTWindowControl htWindowControl1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbErrorCode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbErrorInfo;
    }
}