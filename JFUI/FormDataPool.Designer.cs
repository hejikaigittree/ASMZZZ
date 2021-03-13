namespace JFUI
{
    partial class FormDataPool
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
            this.ucDataPoolEdit1 = new JFUI.UcDataPoolEdit();
            this.SuspendLayout();
            // 
            // ucDataPoolEdit1
            // 
            this.ucDataPoolEdit1.AutoUpdateDataPool = false;
            this.ucDataPoolEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucDataPoolEdit1.Location = new System.Drawing.Point(0, 0);
            this.ucDataPoolEdit1.Name = "ucDataPoolEdit1";
            this.ucDataPoolEdit1.Size = new System.Drawing.Size(540, 547);
            this.ucDataPoolEdit1.TabIndex = 0;
            // 
            // FormDataPool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 547);
            this.Controls.Add(this.ucDataPoolEdit1);
            this.Name = "FormDataPool";
            this.Text = "FormDataPool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDataPool_FormClosing);
            this.Load += new System.EventHandler(this.FormDataPool_Load);
            this.VisibleChanged += new System.EventHandler(this.FormDataPool_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private UcDataPoolEdit ucDataPoolEdit1;
    }
}