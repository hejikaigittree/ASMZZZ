namespace JFRecipe
{
    partial class FormGenCommonRecipe
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
            this.cbGenCate = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbGenID = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbCopyID = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkCopy = new System.Windows.Forms.CheckBox();
            this.cbCopyCate = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.labelTips = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Categoty:";
            // 
            // cbGenCate
            // 
            this.cbGenCate.FormattingEnabled = true;
            this.cbGenCate.Location = new System.Drawing.Point(75, 8);
            this.cbGenCate.Name = "cbGenCate";
            this.cbGenCate.Size = new System.Drawing.Size(171, 20);
            this.cbGenCate.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "RecipeID:";
            // 
            // tbGenID
            // 
            this.tbGenID.Location = new System.Drawing.Point(75, 34);
            this.tbGenID.Name = "tbGenID";
            this.tbGenID.Size = new System.Drawing.Size(171, 21);
            this.tbGenID.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbCopyID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.chkCopy);
            this.groupBox1.Controls.Add(this.cbCopyCate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 88);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // cbCopyID
            // 
            this.cbCopyID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCopyID.Enabled = false;
            this.cbCopyID.FormattingEnabled = true;
            this.cbCopyID.Location = new System.Drawing.Point(63, 58);
            this.cbCopyID.Name = "cbCopyID";
            this.cbCopyID.Size = new System.Drawing.Size(171, 20);
            this.cbCopyID.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "RecipeID:";
            // 
            // chkCopy
            // 
            this.chkCopy.AutoSize = true;
            this.chkCopy.Location = new System.Drawing.Point(8, 12);
            this.chkCopy.Name = "chkCopy";
            this.chkCopy.Size = new System.Drawing.Size(108, 16);
            this.chkCopy.TabIndex = 0;
            this.chkCopy.Text = "拷贝已有Recipe";
            this.chkCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCopy.UseVisualStyleBackColor = true;
            this.chkCopy.CheckedChanged += new System.EventHandler(this.chkCopy_CheckedChanged);
            // 
            // cbCopyCate
            // 
            this.cbCopyCate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCopyCate.Enabled = false;
            this.cbCopyCate.FormattingEnabled = true;
            this.cbCopyCate.Location = new System.Drawing.Point(63, 32);
            this.cbCopyCate.Name = "cbCopyCate";
            this.cbCopyCate.Size = new System.Drawing.Size(171, 20);
            this.cbCopyCate.TabIndex = 6;
            this.cbCopyCate.SelectedIndexChanged += new System.EventHandler(this.cbCopyCate_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "Categoty:";
            // 
            // btOK
            // 
            this.btOK.Enabled = false;
            this.btOK.Location = new System.Drawing.Point(33, 170);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 5;
            this.btOK.Text = "确定";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(158, 170);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 6;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // labelTips
            // 
            this.labelTips.AutoSize = true;
            this.labelTips.Location = new System.Drawing.Point(13, 152);
            this.labelTips.Name = "labelTips";
            this.labelTips.Size = new System.Drawing.Size(29, 12);
            this.labelTips.TabIndex = 7;
            this.labelTips.Text = "Tips";
            // 
            // FormGenCommonRecipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 202);
            this.Controls.Add(this.labelTips);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbGenID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbGenCate);
            this.Controls.Add(this.label1);
            this.Name = "FormGenCommonRecipe";
            this.Text = "创建 Common Recipe";
            this.Load += new System.EventHandler(this.FormGenCommonRecipe_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbGenCate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbGenID;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbCopyID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkCopy;
        private System.Windows.Forms.ComboBox cbCopyCate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label labelTips;
    }
}