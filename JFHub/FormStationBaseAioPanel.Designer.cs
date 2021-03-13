namespace JFHub
{
    partial class FormStationBaseAioPanel
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
            // FormStationBaseAioPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "FormStationBaseAioPanel";
            this.Text = "工站:AIO面板";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormStationBaseAioPanel_FormClosing);
            this.Load += new System.EventHandler(this.FormStationBaseAioPanel_Load);
            this.VisibleChanged += new System.EventHandler(this.FormStationBaseAioPanel_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion
    }
}