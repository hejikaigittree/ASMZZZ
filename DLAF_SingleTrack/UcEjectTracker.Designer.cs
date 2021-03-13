namespace DLAF_SingleTrack
{
    partial class UcEjectTracker
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
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btFeed = new System.Windows.Forms.Button();
            this.btCloseClamp = new System.Windows.Forms.Button();
            this.btOpenClamp = new System.Windows.Forms.Button();
            this.ucStationRealtimeUI1 = new JFUI.UcStationRealtimeUI();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(155, 188);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(55, 21);
            this.numericUpDown1.TabIndex = 26;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(157, 218);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(0, 21);
            this.textBox3.TabIndex = 25;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(157, 189);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(0, 21);
            this.textBox2.TabIndex = 22;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(210, 187);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(41, 23);
            this.button2.TabIndex = 21;
            this.button2.Text = "X->";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(112, 187);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(43, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "<-X";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(80, 257);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 19;
            this.label1.Text = "行程";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(112, 253);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 18;
            // 
            // btFeed
            // 
            this.btFeed.Location = new System.Drawing.Point(4, 251);
            this.btFeed.Name = "btFeed";
            this.btFeed.Size = new System.Drawing.Size(75, 23);
            this.btFeed.TabIndex = 17;
            this.btFeed.Text = "夹料移动";
            this.btFeed.UseVisualStyleBackColor = true;
            // 
            // btCloseClamp
            // 
            this.btCloseClamp.Location = new System.Drawing.Point(4, 216);
            this.btCloseClamp.Name = "btCloseClamp";
            this.btCloseClamp.Size = new System.Drawing.Size(75, 23);
            this.btCloseClamp.TabIndex = 16;
            this.btCloseClamp.Text = "夹手闭合";
            this.btCloseClamp.UseVisualStyleBackColor = true;
            // 
            // btOpenClamp
            // 
            this.btOpenClamp.Location = new System.Drawing.Point(4, 187);
            this.btOpenClamp.Name = "btOpenClamp";
            this.btOpenClamp.Size = new System.Drawing.Size(75, 23);
            this.btOpenClamp.TabIndex = 15;
            this.btOpenClamp.Text = "夹手-松开";
            this.btOpenClamp.UseVisualStyleBackColor = true;
            // 
            // ucStationRealtimeUI1
            // 
            this.ucStationRealtimeUI1.AllowedStartStopCmd = false;
            this.ucStationRealtimeUI1.JfDisplayMode = JFUI.UcStationRealtimeUI.JFDisplayMode.full;
            this.ucStationRealtimeUI1.Location = new System.Drawing.Point(3, 3);
            this.ucStationRealtimeUI1.Name = "ucStationRealtimeUI1";
            this.ucStationRealtimeUI1.Size = new System.Drawing.Size(370, 177);
            this.ucStationRealtimeUI1.TabIndex = 14;
            // 
            // UcEjectTracker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btFeed);
            this.Controls.Add(this.btCloseClamp);
            this.Controls.Add(this.btOpenClamp);
            this.Controls.Add(this.ucStationRealtimeUI1);
            this.Name = "UcEjectTracker";
            this.Size = new System.Drawing.Size(554, 395);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btFeed;
        private System.Windows.Forms.Button btCloseClamp;
        private System.Windows.Forms.Button btOpenClamp;
        private JFUI.UcStationRealtimeUI ucStationRealtimeUI1;
    }
}
