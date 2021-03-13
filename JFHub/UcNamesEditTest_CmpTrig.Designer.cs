namespace JFHub
{
    partial class UcNamesEditTest_CmpTrig
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
            this.pnTrigs = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "位置比较触发器名称列表";
            // 
            // pnTrigs
            // 
            this.pnTrigs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnTrigs.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnTrigs.Location = new System.Drawing.Point(6, 20);
            this.pnTrigs.Name = "pnTrigs";
            this.pnTrigs.Size = new System.Drawing.Size(663, 323);
            this.pnTrigs.TabIndex = 1;
            // 
            // UcNamesEditTest_Trig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnTrigs);
            this.Controls.Add(this.label1);
            this.Name = "UcNamesEditTest_Trig";
            this.Size = new System.Drawing.Size(672, 346);
            this.Load += new System.EventHandler(this.UcNamesEditTest_Trig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnTrigs;
    }
}
