namespace JFUI
{
    partial class UcTrigCtrlChn
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
            this.chkEnable = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbIntensity = new System.Windows.Forms.TextBox();
            this.tbDuration = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btSoftwareTrig = new System.Windows.Forms.Button();
            this.tbID = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // chkEnable
            // 
            this.chkEnable.AutoSize = true;
            this.chkEnable.Location = new System.Drawing.Point(165, 2);
            this.chkEnable.Name = "chkEnable";
            this.chkEnable.Size = new System.Drawing.Size(48, 16);
            this.chkEnable.TabIndex = 1;
            this.chkEnable.Text = "使能";
            this.chkEnable.UseVisualStyleBackColor = true;
            this.chkEnable.CheckedChanged += new System.EventHandler(this.chkEnable_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(212, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "强度:";
            // 
            // tbIntensity
            // 
            this.tbIntensity.Location = new System.Drawing.Point(248, 0);
            this.tbIntensity.Name = "tbIntensity";
            this.tbIntensity.Size = new System.Drawing.Size(58, 21);
            this.tbIntensity.TabIndex = 3;
            this.tbIntensity.TextChanged += new System.EventHandler(this.tbIntensity_TextChanged);
            this.tbIntensity.Enter += new System.EventHandler(this.tbIntensity_Enter);
            this.tbIntensity.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbIntensity_KeyDown);
            this.tbIntensity.Leave += new System.EventHandler(this.tbIntensity_Leave);
            // 
            // tbDuration
            // 
            this.tbDuration.Location = new System.Drawing.Point(351, 0);
            this.tbDuration.Name = "tbDuration";
            this.tbDuration.Size = new System.Drawing.Size(58, 21);
            this.tbDuration.TabIndex = 5;
            this.tbDuration.TextChanged += new System.EventHandler(this.tbDuration_TextChanged);
            this.tbDuration.Enter += new System.EventHandler(this.tbDuration_Enter);
            this.tbDuration.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbDuration_KeyDown);
            this.tbDuration.Leave += new System.EventHandler(this.tbDuration_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(314, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "时长:";
            // 
            // btSoftwareTrig
            // 
            this.btSoftwareTrig.Location = new System.Drawing.Point(1, 22);
            this.btSoftwareTrig.Name = "btSoftwareTrig";
            this.btSoftwareTrig.Size = new System.Drawing.Size(50, 21);
            this.btSoftwareTrig.TabIndex = 6;
            this.btSoftwareTrig.Text = "软触发";
            this.btSoftwareTrig.UseVisualStyleBackColor = true;
            this.btSoftwareTrig.Click += new System.EventHandler(this.btSoftwareTrig_Click);
            // 
            // tbID
            // 
            this.tbID.Location = new System.Drawing.Point(1, 0);
            this.tbID.Name = "tbID";
            this.tbID.ReadOnly = true;
            this.tbID.Size = new System.Drawing.Size(163, 21);
            this.tbID.TabIndex = 7;
            // 
            // UcTrigCtrlChn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbID);
            this.Controls.Add(this.btSoftwareTrig);
            this.Controls.Add(this.tbDuration);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbIntensity);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkEnable);
            this.Name = "UcTrigCtrlChn";
            this.Size = new System.Drawing.Size(412, 45);
            this.Load += new System.EventHandler(this.UcTrigCtrlChn_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkEnable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbIntensity;
        private System.Windows.Forms.TextBox tbDuration;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btSoftwareTrig;
        private System.Windows.Forms.TextBox tbID;
    }
}
