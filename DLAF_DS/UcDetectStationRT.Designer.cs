namespace DLAF_DS
{
    partial class UcDetectStationRT
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
            this.htWindowControl1 = new HTHalControl.HTWindowControl();
            this.richRetectResult = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btClearDetectResult = new System.Windows.Forms.Button();
            this.SuspendLayout();
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
            this.htWindowControl1.Location = new System.Drawing.Point(4, 4);
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
            this.htWindowControl1.Size = new System.Drawing.Size(476, 409);
            this.htWindowControl1.TabIndex = 0;
            this.htWindowControl1.UmPerPix = -1D;
            // 
            // richRetectResult
            // 
            this.richRetectResult.BackColor = System.Drawing.SystemColors.Control;
            this.richRetectResult.Location = new System.Drawing.Point(486, 28);
            this.richRetectResult.Name = "richRetectResult";
            this.richRetectResult.ReadOnly = true;
            this.richRetectResult.Size = new System.Drawing.Size(458, 363);
            this.richRetectResult.TabIndex = 1;
            this.richRetectResult.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(487, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "视觉检测结果";
            // 
            // btClearDetectResult
            // 
            this.btClearDetectResult.Location = new System.Drawing.Point(589, 2);
            this.btClearDetectResult.Name = "btClearDetectResult";
            this.btClearDetectResult.Size = new System.Drawing.Size(75, 23);
            this.btClearDetectResult.TabIndex = 3;
            this.btClearDetectResult.Text = "清空";
            this.btClearDetectResult.UseVisualStyleBackColor = true;
            this.btClearDetectResult.Click += new System.EventHandler(this.btClearDetectResult_Click);
            // 
            // UcDetectStationRT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btClearDetectResult);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richRetectResult);
            this.Controls.Add(this.htWindowControl1);
            this.Name = "UcDetectStationRT";
            this.Size = new System.Drawing.Size(959, 416);
            this.Load += new System.EventHandler(this.UcDetectStationRT_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HTHalControl.HTWindowControl htWindowControl1;
        private System.Windows.Forms.RichTextBox richRetectResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btClearDetectResult;
    }
}
