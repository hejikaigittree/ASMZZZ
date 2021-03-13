namespace JFUI
{
    partial class FormObtainBarcode
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
            this.lbCaption = new System.Windows.Forms.Label();
            this.cbBarcode = new System.Windows.Forms.ComboBox();
            this.btClear = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.cbOBMode = new System.Windows.Forms.ComboBox();
            this.tbBarcode = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbCaption
            // 
            this.lbCaption.AutoSize = true;
            this.lbCaption.Location = new System.Drawing.Point(20, 17);
            this.lbCaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbCaption.Name = "lbCaption";
            this.lbCaption.Size = new System.Drawing.Size(0, 16);
            this.lbCaption.TabIndex = 0;
            // 
            // cbBarcode
            // 
            this.cbBarcode.Font = new System.Drawing.Font("幼圆", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbBarcode.FormattingEnabled = true;
            this.cbBarcode.Location = new System.Drawing.Point(99, 9);
            this.cbBarcode.Margin = new System.Windows.Forms.Padding(4);
            this.cbBarcode.Name = "cbBarcode";
            this.cbBarcode.Size = new System.Drawing.Size(299, 27);
            this.cbBarcode.TabIndex = 1;
            // 
            // btClear
            // 
            this.btClear.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btClear.Location = new System.Drawing.Point(392, 8);
            this.btClear.Margin = new System.Windows.Forms.Padding(4);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(75, 28);
            this.btClear.TabIndex = 2;
            this.btClear.Text = "清除";
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // btOK
            // 
            this.btOK.Font = new System.Drawing.Font("幼圆", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btOK.Location = new System.Drawing.Point(88, 44);
            this.btOK.Margin = new System.Windows.Forms.Padding(4);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(147, 35);
            this.btOK.TabIndex = 4;
            this.btOK.Text = "确定";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.Font = new System.Drawing.Font("幼圆", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btCancel.Location = new System.Drawing.Point(243, 44);
            this.btCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(144, 35);
            this.btCancel.TabIndex = 5;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // cbOBMode
            // 
            this.cbOBMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOBMode.Font = new System.Drawing.Font("幼圆", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbOBMode.FormattingEnabled = true;
            this.cbOBMode.Items.AddRange(new object[] {
            "扫码",
            "输入",
            "选择"});
            this.cbOBMode.Location = new System.Drawing.Point(2, 9);
            this.cbOBMode.Margin = new System.Windows.Forms.Padding(4);
            this.cbOBMode.Name = "cbOBMode";
            this.cbOBMode.Size = new System.Drawing.Size(82, 27);
            this.cbOBMode.TabIndex = 6;
            this.cbOBMode.SelectedIndexChanged += new System.EventHandler(this.cbOBMode_SelectedIndexChanged);
            // 
            // tbBarcode
            // 
            this.tbBarcode.Location = new System.Drawing.Point(88, 10);
            this.tbBarcode.Name = "tbBarcode";
            this.tbBarcode.Size = new System.Drawing.Size(299, 25);
            this.tbBarcode.TabIndex = 7;
            this.tbBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbBarcode_KeyDown);
            // 
            // FormObtainBarcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 90);
            this.Controls.Add(this.tbBarcode);
            this.Controls.Add(this.cbOBMode);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.btClear);
            this.Controls.Add(this.cbBarcode);
            this.Controls.Add(this.lbCaption);
            this.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormObtainBarcode";
            this.Text = "FormObtainBarcode";
            this.Activated += new System.EventHandler(this.FormObtainBarcode_Activated);
            this.Load += new System.EventHandler(this.FormObtainBarcode_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormObtainBarcode_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbCaption;
        private System.Windows.Forms.ComboBox cbBarcode;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.ComboBox cbOBMode;
        private System.Windows.Forms.TextBox tbBarcode;
    }
}