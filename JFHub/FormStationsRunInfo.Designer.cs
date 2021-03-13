namespace JFHub
{
    partial class FormStationsRunInfo
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.rchGatherInfo = new System.Windows.Forms.RichTextBox();
            this.btGatherClear = new System.Windows.Forms.Button();
            this.btUpdateStations = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 450);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btUpdateStations);
            this.tabPage1.Controls.Add(this.rchGatherInfo);
            this.tabPage1.Controls.Add(this.btGatherClear);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 424);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "信息汇总";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // rchGatherInfo
            // 
            this.rchGatherInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rchGatherInfo.Location = new System.Drawing.Point(3, 26);
            this.rchGatherInfo.Name = "rchGatherInfo";
            this.rchGatherInfo.Size = new System.Drawing.Size(786, 398);
            this.rchGatherInfo.TabIndex = 1;
            this.rchGatherInfo.Text = "";
            // 
            // btGatherClear
            // 
            this.btGatherClear.Location = new System.Drawing.Point(715, 2);
            this.btGatherClear.Name = "btGatherClear";
            this.btGatherClear.Size = new System.Drawing.Size(75, 23);
            this.btGatherClear.TabIndex = 0;
            this.btGatherClear.Text = "清空信息";
            this.btGatherClear.UseVisualStyleBackColor = true;
            this.btGatherClear.Click += new System.EventHandler(this.btGatherClear_Click);
            // 
            // btUpdateStations
            // 
            this.btUpdateStations.Location = new System.Drawing.Point(3, 2);
            this.btUpdateStations.Name = "btUpdateStations";
            this.btUpdateStations.Size = new System.Drawing.Size(92, 23);
            this.btUpdateStations.TabIndex = 2;
            this.btUpdateStations.Text = "更新工站列表";
            this.btUpdateStations.UseVisualStyleBackColor = true;
            this.btUpdateStations.Click += new System.EventHandler(this.btUpdateStations_Click);
            // 
            // FormStationsRunInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormStationsRunInfo";
            this.Text = "工站运行信息";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormStationsRunInfo_FormClosing);
            this.Load += new System.EventHandler(this.FormStationsRunInfo_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btGatherClear;
        private System.Windows.Forms.RichTextBox rchGatherInfo;
        private System.Windows.Forms.Button btUpdateStations;
    }
}