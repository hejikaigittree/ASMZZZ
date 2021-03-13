namespace JFHub
{
    partial class FormAddFlow2Station
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
            this.tbName = new System.Windows.Forms.TextBox();
            this.chkLoaded = new System.Windows.Forms.CheckBox();
            this.btLoadFile = new System.Windows.Forms.Button();
            this.tbFilePath = new System.Windows.Forms.TextBox();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名称:";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(48, 9);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(249, 21);
            this.tbName.TabIndex = 1;
            // 
            // chkLoaded
            // 
            this.chkLoaded.AutoSize = true;
            this.chkLoaded.Location = new System.Drawing.Point(13, 41);
            this.chkLoaded.Name = "chkLoaded";
            this.chkLoaded.Size = new System.Drawing.Size(96, 16);
            this.chkLoaded.TabIndex = 2;
            this.chkLoaded.Text = "从文件中加载";
            this.chkLoaded.UseVisualStyleBackColor = true;
            this.chkLoaded.CheckedChanged += new System.EventHandler(this.chkLoaded_CheckedChanged);
            // 
            // btLoadFile
            // 
            this.btLoadFile.Enabled = false;
            this.btLoadFile.Location = new System.Drawing.Point(106, 37);
            this.btLoadFile.Name = "btLoadFile";
            this.btLoadFile.Size = new System.Drawing.Size(25, 23);
            this.btLoadFile.TabIndex = 3;
            this.btLoadFile.Text = "...";
            this.btLoadFile.UseVisualStyleBackColor = true;
            this.btLoadFile.Click += new System.EventHandler(this.btLoadFile_Click);
            // 
            // tbFilePath
            // 
            this.tbFilePath.Location = new System.Drawing.Point(138, 38);
            this.tbFilePath.Name = "tbFilePath";
            this.tbFilePath.ReadOnly = true;
            this.tbFilePath.Size = new System.Drawing.Size(159, 21);
            this.tbFilePath.TabIndex = 4;
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(56, 66);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 5;
            this.btOK.Text = "确定";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(176, 65);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 6;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // FormAddFlow2Station
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 99);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.tbFilePath);
            this.Controls.Add(this.btLoadFile);
            this.Controls.Add(this.chkLoaded);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddFlow2Station";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormAddFlow2Station";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.CheckBox chkLoaded;
        private System.Windows.Forms.Button btLoadFile;
        private System.Windows.Forms.TextBox tbFilePath;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
    }
}