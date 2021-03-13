namespace JFHub
{
    partial class FormSysCfg
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
            this.tabControlCF1 = new JFUI.TabControlCF();
            this.SuspendLayout();
            // 
            // tabControlCF1
            // 
            this.tabControlCF1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControlCF1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlCF1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlCF1.ItemSize = new System.Drawing.Size(35, 150);
            this.tabControlCF1.Location = new System.Drawing.Point(0, 0);
            this.tabControlCF1.Multiline = true;
            this.tabControlCF1.Name = "tabControlCF1";
            this.tabControlCF1.SelectedIndex = 0;
            this.tabControlCF1.Size = new System.Drawing.Size(938, 533);
            this.tabControlCF1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlCF1.TabColor = System.Drawing.SystemColors.ControlDark;
            this.tabControlCF1.TabIndex = 0;
            this.tabControlCF1.SelectedIndexChanged += new System.EventHandler(this.tabControlCF1_SelectedIndexChanged);
            // 
            // FormSysCfg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(938, 533);
            this.Controls.Add(this.tabControlCF1);
            this.Name = "FormSysCfg";
            this.Text = "系统配置";
            this.Load += new System.EventHandler(this.FormSysCfg_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private JFUI.TabControlCF tabControlCF1;
    }
}