namespace LFAOIRecipe
{
    partial class Form_Lv_Inf
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Lv_program = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(76, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "当前版本:";
            // 
            // textBox_Lv_program
            // 
            this.textBox_Lv_program.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.textBox_Lv_program.Enabled = false;
            this.textBox_Lv_program.ForeColor = System.Drawing.Color.Blue;
            this.textBox_Lv_program.Location = new System.Drawing.Point(181, 3);
            this.textBox_Lv_program.Name = "textBox_Lv_program";
            this.textBox_Lv_program.ReadOnly = true;
            this.textBox_Lv_program.Size = new System.Drawing.Size(302, 21);
            this.textBox_Lv_program.TabIndex = 1;
            this.textBox_Lv_program.Text = "Any CPU_1.00_2020XX.XX";
            this.textBox_Lv_program.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox1.ForeColor = System.Drawing.Color.DarkCyan;
            this.richTextBox1.Location = new System.Drawing.Point(12, 53);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBox1.Size = new System.Drawing.Size(613, 476);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "[V1.0.0.8 2020/12/15]\n1、完善Frame表面检测\n2、添加银胶自动区域生成\n\n[V1.0.0.8 2020/12/03]\n1、统一大软件和r" +
    "ecipe接口版本\n2、完善aroundBond检测\n\n[V1.0.0.7 2020/11/30]\n1、根据defectInfo输出修改算子和接口";
            this.richTextBox1.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "更新历史(History):";
            // 
            // Form_Lv_Inf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 533);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.textBox_Lv_program);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Form_Lv_Inf";
            this.Text = "版本信息";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Lv_program;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label2;
    }
}