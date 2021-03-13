namespace JFHub
{
    partial class UcSimpleAxisInStation
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
            this.ucAxisTest = new JFUI.UcAxisTest();
            this.gbAxisName = new System.Windows.Forms.GroupBox();
            this.btCfg = new System.Windows.Forms.Button();
            this.cbMode = new System.Windows.Forms.ComboBox();
            this.gbAxisName.SuspendLayout();
            this.SuspendLayout();
            // 
            // ucAxisTest
            // 
            this.ucAxisTest.DisplayMode = JFUI.UcAxisTest.JFDisplayMode.simplest_pos;
            this.ucAxisTest.IsBoxShowError = false;
            this.ucAxisTest.IsRepeating = false;
            this.ucAxisTest.Location = new System.Drawing.Point(3, 15);
            this.ucAxisTest.Name = "ucAxisTest";
            this.ucAxisTest.Size = new System.Drawing.Size(292, 50);
            this.ucAxisTest.TabIndex = 0;
            // 
            // gbAxisName
            // 
            this.gbAxisName.Controls.Add(this.btCfg);
            this.gbAxisName.Controls.Add(this.cbMode);
            this.gbAxisName.Controls.Add(this.ucAxisTest);
            this.gbAxisName.Location = new System.Drawing.Point(0, 1);
            this.gbAxisName.Name = "gbAxisName";
            this.gbAxisName.Size = new System.Drawing.Size(380, 67);
            this.gbAxisName.TabIndex = 1;
            this.gbAxisName.TabStop = false;
            this.gbAxisName.Text = "轴名称";
            // 
            // btCfg
            // 
            this.btCfg.Location = new System.Drawing.Point(301, 39);
            this.btCfg.Name = "btCfg";
            this.btCfg.Size = new System.Drawing.Size(73, 23);
            this.btCfg.TabIndex = 3;
            this.btCfg.Text = "配置";
            this.btCfg.UseVisualStyleBackColor = true;
            this.btCfg.Click += new System.EventHandler(this.btCfg_Click);
            // 
            // cbMode
            // 
            this.cbMode.FormattingEnabled = true;
            this.cbMode.Items.AddRange(new object[] {
            "位置模式",
            "速度模式"});
            this.cbMode.Location = new System.Drawing.Point(301, 16);
            this.cbMode.Name = "cbMode";
            this.cbMode.Size = new System.Drawing.Size(73, 20);
            this.cbMode.TabIndex = 2;
            this.cbMode.SelectedIndexChanged += new System.EventHandler(this.cbMode_SelectedIndexChanged);
            // 
            // UcSimpleAxisInStation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbAxisName);
            this.Name = "UcSimpleAxisInStation";
            this.Size = new System.Drawing.Size(380, 71);
            this.Load += new System.EventHandler(this.UcAxisInStation_Load);
            this.gbAxisName.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private JFUI.UcAxisTest ucAxisTest;
        private System.Windows.Forms.GroupBox gbAxisName;
        private System.Windows.Forms.Button btCfg;
        private System.Windows.Forms.ComboBox cbMode;
    }
}
