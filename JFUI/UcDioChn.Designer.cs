namespace JFUI
{
    partial class UcDioChn
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
            this.Lamp = new JFUI.LampButton();
            this.tbEdit = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Lamp
            // 
            this.Lamp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Lamp.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Lamp.FlatAppearance.BorderSize = 0;
            this.Lamp.IconSize = new System.Drawing.Size(32, 32);
            this.Lamp.ImageIndex = 0;
            this.Lamp.LampColor = JFUI.LampButton.LColor.Gray;
            this.Lamp.Location = new System.Drawing.Point(0, 0);
            this.Lamp.Name = "Lamp";
            this.Lamp.Size = new System.Drawing.Size(240, 33);
            this.Lamp.TabIndex = 0;
            this.Lamp.Text = "lampButton1";
            this.Lamp.UseVisualStyleBackColor = true;
            this.Lamp.SizeChanged += new System.EventHandler(this.Lamp_SizeChanged);
            // 
            // tbEdit
            // 
            this.tbEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEdit.Location = new System.Drawing.Point(24, 6);
            this.tbEdit.Name = "tbEdit";
            this.tbEdit.Size = new System.Drawing.Size(213, 21);
            this.tbEdit.TabIndex = 1;
            this.tbEdit.TextChanged += new System.EventHandler(this.tbEdit_TextChanged);
            this.tbEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbEdit_KeyDown);
            // 
            // UcDioChn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbEdit);
            this.Controls.Add(this.Lamp);
            this.Name = "UcDioChn";
            this.Size = new System.Drawing.Size(240, 33);
            this.Load += new System.EventHandler(this.UcDIO_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LampButton Lamp;
        private System.Windows.Forms.TextBox tbEdit;
    }
}
