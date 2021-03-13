namespace DLAF
{
    partial class Form_VisionCfgManager
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
            this.dgvVisionCfg = new System.Windows.Forms.DataGridView();
            this.btExit = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVisionCfg)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvVisionCfg
            // 
            this.dgvVisionCfg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVisionCfg.Location = new System.Drawing.Point(1, 0);
            this.dgvVisionCfg.Name = "dgvVisionCfg";
            this.dgvVisionCfg.RowTemplate.Height = 23;
            this.dgvVisionCfg.Size = new System.Drawing.Size(427, 269);
            this.dgvVisionCfg.TabIndex = 0;
            // 
            // btExit
            // 
            this.btExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btExit.FlatAppearance.BorderSize = 0;
            this.btExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btExit.ForeColor = System.Drawing.Color.White;
            this.btExit.Location = new System.Drawing.Point(305, 270);
            this.btExit.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btExit.Name = "btExit";
            this.btExit.Size = new System.Drawing.Size(112, 41);
            this.btExit.TabIndex = 82;
            this.btExit.Text = "取消";
            this.btExit.UseVisualStyleBackColor = false;
            this.btExit.Click += new System.EventHandler(this.btExit_Click);
            // 
            // btSave
            // 
            this.btSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btSave.FlatAppearance.BorderSize = 0;
            this.btSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSave.ForeColor = System.Drawing.Color.White;
            this.btSave.Location = new System.Drawing.Point(172, 270);
            this.btSave.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(112, 41);
            this.btSave.TabIndex = 82;
            this.btSave.Text = "保存配置";
            this.btSave.UseVisualStyleBackColor = false;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // Form_VisionCfgManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 313);
            this.Controls.Add(this.btExit);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.dgvVisionCfg);
            this.Name = "Form_VisionCfgManager";
            this.Text = "Form_VisionCfgManager";
            this.Load += new System.EventHandler(this.Form_VisionCfgManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVisionCfg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvVisionCfg;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.Button btSave;
    }
}