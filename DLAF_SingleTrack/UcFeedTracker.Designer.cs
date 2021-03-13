namespace DLAF_SingleTrack
{
    partial class UcFeedTracker
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
            this.ucStationRealtimeUI1 = new JFUI.UcStationRealtimeUI();
            this.btOpenClamp = new System.Windows.Forms.Button();
            this.btCloseClamp = new System.Windows.Forms.Button();
            this.btFeed = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // ucStationRealtimeUI1
            // 
            this.ucStationRealtimeUI1.AllowedStartStopCmd = false;
            this.ucStationRealtimeUI1.JfDisplayMode = JFUI.UcStationRealtimeUI.JFDisplayMode.full;
            this.ucStationRealtimeUI1.Location = new System.Drawing.Point(3, 3);
            this.ucStationRealtimeUI1.Name = "ucStationRealtimeUI1";
            this.ucStationRealtimeUI1.Size = new System.Drawing.Size(370, 177);
            this.ucStationRealtimeUI1.TabIndex = 0;
            // 
            // btOpenClamp
            // 
            this.btOpenClamp.Location = new System.Drawing.Point(4, 187);
            this.btOpenClamp.Name = "btOpenClamp";
            this.btOpenClamp.Size = new System.Drawing.Size(75, 23);
            this.btOpenClamp.TabIndex = 1;
            this.btOpenClamp.Text = "夹手-松开";
            this.btOpenClamp.UseVisualStyleBackColor = true;
            this.btOpenClamp.Click += new System.EventHandler(this.btOpenClamp_Click);
            // 
            // btCloseClamp
            // 
            this.btCloseClamp.Location = new System.Drawing.Point(4, 216);
            this.btCloseClamp.Name = "btCloseClamp";
            this.btCloseClamp.Size = new System.Drawing.Size(75, 23);
            this.btCloseClamp.TabIndex = 2;
            this.btCloseClamp.Text = "夹手闭合";
            this.btCloseClamp.UseVisualStyleBackColor = true;
            this.btCloseClamp.Click += new System.EventHandler(this.btCloseClamp_Click);
            // 
            // btFeed
            // 
            this.btFeed.Location = new System.Drawing.Point(4, 251);
            this.btFeed.Name = "btFeed";
            this.btFeed.Size = new System.Drawing.Size(75, 23);
            this.btFeed.TabIndex = 3;
            this.btFeed.Text = "夹料移动";
            this.btFeed.UseVisualStyleBackColor = true;
            this.btFeed.Click += new System.EventHandler(this.btFeed_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(112, 253);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(80, 257);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "行程";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(112, 187);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(43, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "<-X";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(210, 187);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(41, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "X->";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(157, 189);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(0, 21);
            this.textBox2.TabIndex = 8;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(157, 218);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(0, 21);
            this.textBox3.TabIndex = 11;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(210, 216);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(43, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "轨道-";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(112, 216);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(43, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "轨道+";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(155, 188);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(55, 21);
            this.numericUpDown1.TabIndex = 12;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(155, 217);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(55, 21);
            this.numericUpDown2.TabIndex = 13;
            // 
            // UcFeedTracker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btFeed);
            this.Controls.Add(this.btCloseClamp);
            this.Controls.Add(this.btOpenClamp);
            this.Controls.Add(this.ucStationRealtimeUI1);
            this.Name = "UcFeedTracker";
            this.Size = new System.Drawing.Size(566, 321);
            this.Load += new System.EventHandler(this.UcFeedTracker_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private JFUI.UcStationRealtimeUI ucStationRealtimeUI1;
        private System.Windows.Forms.Button btOpenClamp;
        private System.Windows.Forms.Button btCloseClamp;
        private System.Windows.Forms.Button btFeed;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
    }
}
