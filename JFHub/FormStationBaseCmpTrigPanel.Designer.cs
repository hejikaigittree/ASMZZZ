namespace JFHub
{
    partial class FormStationBaseCmpTrigPanel
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
            this.SuspendLayout();
            // 
            // FormStationBaseCmpTrigPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "FormStationBaseCmpTrigPanel";
            this.Text = "工站:比较触发器面板";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormStationBaseCmpTrigPanel_FormClosing);
            this.Load += new System.EventHandler(this.FormStationBaseCmpTrigPanel_Load);
            this.VisibleChanged += new System.EventHandler(this.FormStationBaseCmpTrigPanel_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion
    }
}