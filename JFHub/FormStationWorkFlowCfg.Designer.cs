namespace JFHub
{
    partial class FormStationWorkFlowCfg
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
            this.btReload = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.lbTital = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btReload
            // 
            this.btReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btReload.Location = new System.Drawing.Point(639, 5);
            this.btReload.Name = "btReload";
            this.btReload.Size = new System.Drawing.Size(75, 23);
            this.btReload.TabIndex = 0;
            this.btReload.Text = "取消变更";
            this.btReload.UseVisualStyleBackColor = true;
            this.btReload.Click += new System.EventHandler(this.btReload_Click);
            // 
            // btSave
            // 
            this.btSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSave.Location = new System.Drawing.Point(720, 4);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 1;
            this.btSave.Text = "保存变更";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // lbTital
            // 
            this.lbTital.AutoSize = true;
            this.lbTital.Location = new System.Drawing.Point(10, 11);
            this.lbTital.Name = "lbTital";
            this.lbTital.Size = new System.Drawing.Size(179, 12);
            this.lbTital.TabIndex = 2;
            this.lbTital.Text = "工站:未设置 工作流名称:未设置";
            // 
            // FormStationWorkFlowTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbTital);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.btReload);
            this.Name = "FormStationWorkFlowTest";
            this.Text = "工作流调试/设置";
            this.Load += new System.EventHandler(this.FormStationWorkFlowTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btReload;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Label lbTital;
    }
}