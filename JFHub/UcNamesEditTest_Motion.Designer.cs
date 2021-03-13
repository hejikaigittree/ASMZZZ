namespace JFHub
{
    partial class UcNamesEditTest_Motion
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
            this.label1 = new System.Windows.Forms.Label();
            this.pnAxes = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "轴名称列表";
            // 
            // pnAxes
            // 
            this.pnAxes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnAxes.AutoScroll = true;
            this.pnAxes.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnAxes.Location = new System.Drawing.Point(6, 20);
            this.pnAxes.Name = "pnAxes";
            this.pnAxes.Size = new System.Drawing.Size(582, 315);
            this.pnAxes.TabIndex = 1;
            // 
            // UcNamesEditTest_Motion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnAxes);
            this.Controls.Add(this.label1);
            this.Name = "UcNamesEditTest_Motion";
            this.Size = new System.Drawing.Size(591, 338);
            this.Load += new System.EventHandler(this.UcNamesEditTest_Motion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnAxes;
    }
}
