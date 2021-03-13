namespace DLAF
{
    partial class FrmCamAxisMotion
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
            this.components = new System.ComponentModel.Container();
            this.numDistance = new System.Windows.Forms.NumericUpDown();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnZfocus = new System.Windows.Forms.Button();
            this.btnZsafe = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.htWindow = new HTHalControl.HTWindowControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOpenDir = new System.Windows.Forms.Button();
            this.btnSaveSnap = new System.Windows.Forms.Button();
            this.btnConfig_Cam = new System.Windows.Forms.Button();
            this.cbb_Cam = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnZDown = new System.Windows.Forms.Button();
            this.btnZUp = new System.Windows.Forms.Button();
            this.checkBox_MoveByCapture = new System.Windows.Forms.CheckBox();
            this.btnCapture = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numDistance)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // numDistance
            // 
            this.numDistance.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numDistance.DecimalPlaces = 3;
            this.numDistance.Location = new System.Drawing.Point(64, 41);
            this.numDistance.Name = "numDistance";
            this.numDistance.Size = new System.Drawing.Size(60, 21);
            this.numDistance.TabIndex = 0;
            this.numDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numDistance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDistance.ValueChanged += new System.EventHandler(this.numDistance_ValueChanged);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnUp.Location = new System.Drawing.Point(187, 14);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(41, 22);
            this.btnUp.TabIndex = 1;
            this.btnUp.Text = "上移";
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnDown.FlatAppearance.BorderSize = 0;
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDown.Location = new System.Drawing.Point(187, 39);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(41, 21);
            this.btnDown.TabIndex = 2;
            this.btnDown.Text = "下移";
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnLeft.FlatAppearance.BorderSize = 0;
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnLeft.Location = new System.Drawing.Point(144, 39);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(41, 21);
            this.btnLeft.TabIndex = 3;
            this.btnLeft.Text = "左移";
            this.btnLeft.UseVisualStyleBackColor = false;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnRight.FlatAppearance.BorderSize = 0;
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnRight.Location = new System.Drawing.Point(231, 39);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(41, 21);
            this.btnRight.TabIndex = 4;
            this.btnRight.Text = "右移";
            this.btnRight.UseVisualStyleBackColor = false;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "距离(mm):";
            // 
            // btnZfocus
            // 
            this.btnZfocus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnZfocus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnZfocus.FlatAppearance.BorderSize = 0;
            this.btnZfocus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZfocus.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnZfocus.Location = new System.Drawing.Point(6, 14);
            this.btnZfocus.Name = "btnZfocus";
            this.btnZfocus.Size = new System.Drawing.Size(67, 23);
            this.btnZfocus.TabIndex = 6;
            this.btnZfocus.Text = "Z轴聚焦位";
            this.btnZfocus.UseVisualStyleBackColor = false;
            this.btnZfocus.Click += new System.EventHandler(this.btnZfocus_Click);
            // 
            // btnZsafe
            // 
            this.btnZsafe.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnZsafe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnZsafe.FlatAppearance.BorderSize = 0;
            this.btnZsafe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZsafe.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnZsafe.Location = new System.Drawing.Point(74, 14);
            this.btnZsafe.Name = "btnZsafe";
            this.btnZsafe.Size = new System.Drawing.Size(67, 23);
            this.btnZsafe.TabIndex = 7;
            this.btnZsafe.Text = "Z轴安全位";
            this.btnZsafe.UseVisualStyleBackColor = false;
            this.btnZsafe.Click += new System.EventHandler(this.btnZsafe_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "X(mm):0.000";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(95, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "Y(mm):0.000";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "Z(mm):0.000";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // htWindow
            // 
            this.htWindow.BackColor = System.Drawing.Color.Transparent;
            this.htWindow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.htWindow.ColorName = "red";
            this.htWindow.ColorType = 0;
            this.htWindow.Column = null;
            this.htWindow.Column1 = null;
            this.htWindow.Column2 = null;
            this.htWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htWindow.Image = null;
            this.htWindow.Length1 = null;
            this.htWindow.Length2 = null;
            this.htWindow.Location = new System.Drawing.Point(4, 4);
            this.htWindow.Margin = new System.Windows.Forms.Padding(4);
            this.htWindow.Name = "htWindow";
            this.htWindow.Phi = null;
            this.htWindow.Radius = null;
            this.htWindow.Radius1 = null;
            this.htWindow.Radius2 = null;
            this.htWindow.Region = null;
            this.htWindow.RegionType = null;
            this.htWindow.Row = null;
            this.htWindow.Row1 = null;
            this.htWindow.Row2 = null;
            this.htWindow.Size = new System.Drawing.Size(642, 340);
            this.htWindow.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOpenDir);
            this.panel1.Controls.Add(this.btnSaveSnap);
            this.panel1.Controls.Add(this.btnConfig_Cam);
            this.panel1.Controls.Add(this.cbb_Cam);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.btnZDown);
            this.panel1.Controls.Add(this.btnZUp);
            this.panel1.Controls.Add(this.checkBox_MoveByCapture);
            this.panel1.Controls.Add(this.btnCapture);
            this.panel1.Controls.Add(this.btnZsafe);
            this.panel1.Controls.Add(this.btnUp);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.btnDown);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnLeft);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnRight);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numDistance);
            this.panel1.Controls.Add(this.btnZfocus);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(279, 170);
            this.panel1.TabIndex = 12;
            // 
            // btnOpenDir
            // 
            this.btnOpenDir.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOpenDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnOpenDir.FlatAppearance.BorderSize = 0;
            this.btnOpenDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenDir.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnOpenDir.Location = new System.Drawing.Point(179, 87);
            this.btnOpenDir.Name = "btnOpenDir";
            this.btnOpenDir.Size = new System.Drawing.Size(93, 22);
            this.btnOpenDir.TabIndex = 20;
            this.btnOpenDir.Text = "打开存图目录";
            this.btnOpenDir.UseVisualStyleBackColor = false;
            this.btnOpenDir.Click += new System.EventHandler(this.btnOpenDir_Click);
            // 
            // btnSaveSnap
            // 
            this.btnSaveSnap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSaveSnap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnSaveSnap.FlatAppearance.BorderSize = 0;
            this.btnSaveSnap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveSnap.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSaveSnap.Location = new System.Drawing.Point(231, 64);
            this.btnSaveSnap.Name = "btnSaveSnap";
            this.btnSaveSnap.Size = new System.Drawing.Size(41, 21);
            this.btnSaveSnap.TabIndex = 19;
            this.btnSaveSnap.Text = "存图";
            this.btnSaveSnap.UseVisualStyleBackColor = false;
            this.btnSaveSnap.Click += new System.EventHandler(this.btnSaveSnap_Click);
            // 
            // btnConfig_Cam
            // 
            this.btnConfig_Cam.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnConfig_Cam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnConfig_Cam.FlatAppearance.BorderSize = 0;
            this.btnConfig_Cam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfig_Cam.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnConfig_Cam.Location = new System.Drawing.Point(230, 123);
            this.btnConfig_Cam.Name = "btnConfig_Cam";
            this.btnConfig_Cam.Size = new System.Drawing.Size(41, 23);
            this.btnConfig_Cam.TabIndex = 18;
            this.btnConfig_Cam.Text = "配置";
            this.btnConfig_Cam.UseVisualStyleBackColor = false;
            this.btnConfig_Cam.Click += new System.EventHandler(this.btnConfig_Cam_Click);
            // 
            // cbb_Cam
            // 
            this.cbb_Cam.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbb_Cam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.cbb_Cam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_Cam.FormattingEnabled = true;
            this.cbb_Cam.Location = new System.Drawing.Point(78, 125);
            this.cbb_Cam.Name = "cbb_Cam";
            this.cbb_Cam.Size = new System.Drawing.Size(121, 20);
            this.cbb_Cam.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "当前相机:";
            // 
            // btnZDown
            // 
            this.btnZDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnZDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnZDown.FlatAppearance.BorderSize = 0;
            this.btnZDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZDown.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnZDown.Location = new System.Drawing.Point(231, 14);
            this.btnZDown.Name = "btnZDown";
            this.btnZDown.Size = new System.Drawing.Size(41, 22);
            this.btnZDown.TabIndex = 14;
            this.btnZDown.Text = "下降";
            this.btnZDown.UseVisualStyleBackColor = false;
            this.btnZDown.Click += new System.EventHandler(this.btnZDown_Click);
            // 
            // btnZUp
            // 
            this.btnZUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnZUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnZUp.FlatAppearance.BorderSize = 0;
            this.btnZUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZUp.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnZUp.Location = new System.Drawing.Point(144, 14);
            this.btnZUp.Name = "btnZUp";
            this.btnZUp.Size = new System.Drawing.Size(41, 22);
            this.btnZUp.TabIndex = 13;
            this.btnZUp.Text = "上升";
            this.btnZUp.UseVisualStyleBackColor = false;
            this.btnZUp.Click += new System.EventHandler(this.btnZUp_Click);
            // 
            // checkBox_MoveByCapture
            // 
            this.checkBox_MoveByCapture.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox_MoveByCapture.AutoSize = true;
            this.checkBox_MoveByCapture.Checked = true;
            this.checkBox_MoveByCapture.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_MoveByCapture.Location = new System.Drawing.Point(95, 88);
            this.checkBox_MoveByCapture.Name = "checkBox_MoveByCapture";
            this.checkBox_MoveByCapture.Size = new System.Drawing.Size(84, 16);
            this.checkBox_MoveByCapture.TabIndex = 12;
            this.checkBox_MoveByCapture.Text = "移动并拍照";
            this.checkBox_MoveByCapture.UseVisualStyleBackColor = true;
            // 
            // btnCapture
            // 
            this.btnCapture.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCapture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnCapture.FlatAppearance.BorderSize = 0;
            this.btnCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCapture.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCapture.Location = new System.Drawing.Point(187, 64);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(41, 21);
            this.btnCapture.TabIndex = 11;
            this.btnCapture.Text = "拍照";
            this.btnCapture.UseVisualStyleBackColor = false;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.18172F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.81828F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.htWindow, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(941, 348);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.richTextBox1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(653, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.46199F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.53801F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(285, 342);
            this.tableLayoutPanel2.TabIndex = 14;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 179);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox1.Size = new System.Drawing.Size(279, 160);
            this.richTextBox1.TabIndex = 13;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = "帮助:\n激活窗体后可按键快捷操作.\nW键视野上移,S键视野下移,A键视野左移,D键视野右移,Q上升,E下降,空格键拍照.\n【配置】会配置当前相机为图谱用相机。\n【" +
    "存图】会保存当前图片至图像目录下Snap目录。";
            // 
            // FrmCamAxisMotion
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(941, 348);
            this.Controls.Add(this.tableLayoutPanel1);
            this.KeyPreview = true;
            this.Name = "FrmCamAxisMotion";
            this.Text = "相机载轴运动助手";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCamAxisMotion_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.numDistance)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numDistance;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnZfocus;
        private System.Windows.Forms.Button btnZsafe;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer timer1;
        public HTHalControl.HTWindowControl htWindow;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox checkBox_MoveByCapture;
        private System.Windows.Forms.Button btnZDown;
        private System.Windows.Forms.Button btnZUp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbb_Cam;
        private System.Windows.Forms.Button btnConfig_Cam;
        private System.Windows.Forms.Button btnSaveSnap;
        private System.Windows.Forms.Button btnOpenDir;
    }
}