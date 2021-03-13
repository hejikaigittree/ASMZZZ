namespace JFUI
{
    partial class UcAIO
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ucAiPanel = new JFUI.UcAIOPanel();
            this.ucAoPanel = new JFUI.UcAIOPanel();
            this.tbInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ucAiPanel
            // 
            this.ucAiPanel.AutoScroll = true;
            this.ucAiPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ucAiPanel.IsAnalogOut = false;
            this.ucAiPanel.IsNamesEditting = false;
            this.ucAiPanel.Location = new System.Drawing.Point(24, 23);
            this.ucAiPanel.Name = "ucAiPanel";
            this.ucAiPanel.Size = new System.Drawing.Size(189, 193);
            this.ucAiPanel.TabIndex = 0;
            // 
            // ucAoPanel
            // 
            this.ucAoPanel.AutoScroll = true;
            this.ucAoPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ucAoPanel.IsAnalogOut = true;
            this.ucAoPanel.IsNamesEditting = false;
            this.ucAoPanel.Location = new System.Drawing.Point(219, 23);
            this.ucAoPanel.Name = "ucAoPanel";
            this.ucAoPanel.Size = new System.Drawing.Size(187, 193);
            this.ucAoPanel.TabIndex = 1;
            // 
            // tbInfo
            // 
            this.tbInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInfo.BackColor = System.Drawing.SystemColors.Control;
            this.tbInfo.Location = new System.Drawing.Point(0, 281);
            this.tbInfo.Name = "tbInfo";
            this.tbInfo.ReadOnly = true;
            this.tbInfo.Size = new System.Drawing.Size(501, 21);
            this.tbInfo.TabIndex = 2;
            // 
            // UcAIO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbInfo);
            this.Controls.Add(this.ucAoPanel);
            this.Controls.Add(this.ucAiPanel);
            this.Name = "UcAIO";
            this.Size = new System.Drawing.Size(501, 305);
            this.Load += new System.EventHandler(this.UcAIO_Load);
            this.SizeChanged += new System.EventHandler(this.UcAIO_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UcAIOPanel ucAiPanel;
        private UcAIOPanel ucAoPanel;
        private System.Windows.Forms.TextBox tbInfo;
    }
}
