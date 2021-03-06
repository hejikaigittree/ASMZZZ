namespace JFApp
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelUserName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelUserLevel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelProductInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelRunMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel6 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelDevStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel7 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelAlarming = new System.Windows.Forms.ToolStripStatusLabel();
            this.btAuto = new JFUI.RoundButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btManual = new JFUI.RoundButton();
            this.btAlarm = new JFUI.RoundButton();
            this.btCfg = new JFUI.RoundButton();
            this.btVision = new JFUI.RoundButton();
            this.btLog = new JFUI.RoundButton();
            this.btUser = new JFUI.RoundButton();
            this.btBrief = new JFUI.RoundButton();
            this.btStop = new JFUI.RoundButton();
            this.btPause = new JFUI.RoundButton();
            this.btStart = new JFUI.RoundButton();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.btBatch = new JFUI.RoundButton();
            this.subFormPanel = new System.Windows.Forms.Panel();
            this.timerFlush = new System.Windows.Forms.Timer(this.components);
            this.btReset = new JFUI.RoundButton();
            this.btStationRunInfo = new JFUI.RoundButton();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.statusLabelUserName,
            this.toolStripStatusLabel3,
            this.statusLabelUserLevel,
            this.toolStripStatusLabel4,
            this.statusLabelProductInfo,
            this.toolStripStatusLabel5,
            this.statusLabelRunMode,
            this.toolStripStatusLabel6,
            this.statusLabelDevStatus,
            this.toolStripStatusLabel7,
            this.statusLabelAlarming});
            this.statusStrip1.Location = new System.Drawing.Point(0, 728);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1364, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(80, 17);
            this.toolStripStatusLabel1.Text = "景焱智能装备";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(35, 17);
            this.toolStripStatusLabel2.Text = "用户:";
            // 
            // statusLabelUserName
            // 
            this.statusLabelUserName.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.statusLabelUserName.Name = "statusLabelUserName";
            this.statusLabelUserName.Size = new System.Drawing.Size(60, 17);
            this.statusLabelUserName.Text = "unknown";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(35, 17);
            this.toolStripStatusLabel3.Text = "权限:";
            // 
            // statusLabelUserLevel
            // 
            this.statusLabelUserLevel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.statusLabelUserLevel.Name = "statusLabelUserLevel";
            this.statusLabelUserLevel.Size = new System.Drawing.Size(60, 17);
            this.statusLabelUserLevel.Text = "unknown";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(83, 17);
            this.toolStripStatusLabel4.Text = "当前加工产品:";
            // 
            // statusLabelProductInfo
            // 
            this.statusLabelProductInfo.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.statusLabelProductInfo.Name = "statusLabelProductInfo";
            this.statusLabelProductInfo.Size = new System.Drawing.Size(39, 17);
            this.statusLabelProductInfo.Text = "unset";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(59, 17);
            this.toolStripStatusLabel5.Text = "运行模式:";
            // 
            // statusLabelRunMode
            // 
            this.statusLabelRunMode.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.statusLabelRunMode.Name = "statusLabelRunMode";
            this.statusLabelRunMode.Size = new System.Drawing.Size(56, 17);
            this.statusLabelRunMode.Text = "自动运行";
            // 
            // toolStripStatusLabel6
            // 
            this.toolStripStatusLabel6.Name = "toolStripStatusLabel6";
            this.toolStripStatusLabel6.Size = new System.Drawing.Size(59, 17);
            this.toolStripStatusLabel6.Text = "设备状态:";
            // 
            // statusLabelDevStatus
            // 
            this.statusLabelDevStatus.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.statusLabelDevStatus.Name = "statusLabelDevStatus";
            this.statusLabelDevStatus.Size = new System.Drawing.Size(44, 17);
            this.statusLabelDevStatus.Text = "未运行";
            // 
            // toolStripStatusLabel7
            // 
            this.toolStripStatusLabel7.Name = "toolStripStatusLabel7";
            this.toolStripStatusLabel7.Size = new System.Drawing.Size(59, 17);
            this.toolStripStatusLabel7.Text = "报警状态:";
            // 
            // statusLabelAlarming
            // 
            this.statusLabelAlarming.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.statusLabelAlarming.Name = "statusLabelAlarming";
            this.statusLabelAlarming.Size = new System.Drawing.Size(44, 17);
            this.statusLabelAlarming.Text = "未报警";
            // 
            // btAuto
            // 
            this.btAuto.BackColor = System.Drawing.Color.Transparent;
            this.btAuto.BaseColor = System.Drawing.Color.White;
            this.btAuto.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btAuto.FlatAppearance.BorderSize = 0;
            this.btAuto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btAuto.ImageHeight = 60;
            this.btAuto.ImageKey = "Home_Off.png";
            this.btAuto.ImageList = this.imageList1;
            this.btAuto.ImageTextSpace = 0;
            this.btAuto.ImageWidth = 60;
            this.btAuto.Location = new System.Drawing.Point(1, 1);
            this.btAuto.Name = "btAuto";
            this.btAuto.Radius = 24;
            this.btAuto.Size = new System.Drawing.Size(60, 60);
            this.btAuto.SpliteButtonWidth = 18;
            this.btAuto.TabIndex = 3;
            this.btAuto.UseVisualStyleBackColor = false;
            this.btAuto.Click += new System.EventHandler(this.btAuto_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Alarm_Off.png");
            this.imageList1.Images.SetKeyName(1, "Alarm_On.png");
            this.imageList1.Images.SetKeyName(2, "chart_line.png");
            this.imageList1.Images.SetKeyName(3, "Data_Information_Off.png");
            this.imageList1.Images.SetKeyName(4, "Data_Information_On.png");
            this.imageList1.Images.SetKeyName(5, "File_Excel_Off.png");
            this.imageList1.Images.SetKeyName(6, "File_Excel_On.png");
            this.imageList1.Images.SetKeyName(7, "half_pie.png");
            this.imageList1.Images.SetKeyName(8, "Home_Off.png");
            this.imageList1.Images.SetKeyName(9, "Home_on.png");
            this.imageList1.Images.SetKeyName(10, "ico_folder.png");
            this.imageList1.Images.SetKeyName(11, "ico_save.png");
            this.imageList1.Images.SetKeyName(12, "light_gray.png");
            this.imageList1.Images.SetKeyName(13, "light_green.png");
            this.imageList1.Images.SetKeyName(14, "light_red.png");
            this.imageList1.Images.SetKeyName(15, "Login_Off.png");
            this.imageList1.Images.SetKeyName(16, "Login_On.png");
            this.imageList1.Images.SetKeyName(17, "logo.ico");
            this.imageList1.Images.SetKeyName(18, "Mannual_Off.png");
            this.imageList1.Images.SetKeyName(19, "Mannual_On.png");
            this.imageList1.Images.SetKeyName(20, "menu_save.png");
            this.imageList1.Images.SetKeyName(21, "msdn.ico");
            this.imageList1.Images.SetKeyName(22, "Pause_Off.png");
            this.imageList1.Images.SetKeyName(23, "Pause_On.png");
            this.imageList1.Images.SetKeyName(24, "servo_back.png");
            this.imageList1.Images.SetKeyName(25, "servo_down.png");
            this.imageList1.Images.SetKeyName(26, "servo_front.png");
            this.imageList1.Images.SetKeyName(27, "servo_Left.png");
            this.imageList1.Images.SetKeyName(28, "servo_right.png");
            this.imageList1.Images.SetKeyName(29, "servo_Stop.png");
            this.imageList1.Images.SetKeyName(30, "servo_turn_left.png");
            this.imageList1.Images.SetKeyName(31, "servo_turn_right.png");
            this.imageList1.Images.SetKeyName(32, "servo_up.png");
            this.imageList1.Images.SetKeyName(33, "Start_Off.png");
            this.imageList1.Images.SetKeyName(34, "Start_On.png");
            this.imageList1.Images.SetKeyName(35, "Stop_Off.png");
            this.imageList1.Images.SetKeyName(36, "Stop_On.png");
            this.imageList1.Images.SetKeyName(37, "Vision_Off.png");
            this.imageList1.Images.SetKeyName(38, "Vision_On.png");
            // 
            // btManual
            // 
            this.btManual.BackColor = System.Drawing.Color.Transparent;
            this.btManual.BaseColor = System.Drawing.Color.White;
            this.btManual.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btManual.FlatAppearance.BorderSize = 0;
            this.btManual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btManual.ImageHeight = 60;
            this.btManual.ImageKey = "Mannual_Off.png";
            this.btManual.ImageList = this.imageList1;
            this.btManual.ImageWidth = 60;
            this.btManual.Location = new System.Drawing.Point(63, 1);
            this.btManual.Name = "btManual";
            this.btManual.Radius = 24;
            this.btManual.Size = new System.Drawing.Size(60, 60);
            this.btManual.SpliteButtonWidth = 18;
            this.btManual.TabIndex = 4;
            this.btManual.UseVisualStyleBackColor = false;
            this.btManual.Click += new System.EventHandler(this.btManual_Click);
            // 
            // btAlarm
            // 
            this.btAlarm.BackColor = System.Drawing.Color.Transparent;
            this.btAlarm.BaseColor = System.Drawing.Color.White;
            this.btAlarm.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btAlarm.FlatAppearance.BorderSize = 0;
            this.btAlarm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btAlarm.ImageHeight = 60;
            this.btAlarm.ImageKey = "Alarm_Off.png";
            this.btAlarm.ImageList = this.imageList1;
            this.btAlarm.ImageTextSpace = 0;
            this.btAlarm.ImageWidth = 60;
            this.btAlarm.Location = new System.Drawing.Point(125, 1);
            this.btAlarm.Name = "btAlarm";
            this.btAlarm.Radius = 24;
            this.btAlarm.Size = new System.Drawing.Size(60, 60);
            this.btAlarm.SpliteButtonWidth = 18;
            this.btAlarm.TabIndex = 5;
            this.btAlarm.UseVisualStyleBackColor = false;
            this.btAlarm.Click += new System.EventHandler(this.btAlarm_Click);
            // 
            // btCfg
            // 
            this.btCfg.BackColor = System.Drawing.Color.Transparent;
            this.btCfg.BaseColor = System.Drawing.Color.White;
            this.btCfg.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btCfg.FlatAppearance.BorderSize = 0;
            this.btCfg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btCfg.ImageHeight = 60;
            this.btCfg.ImageKey = "ico_folder.png";
            this.btCfg.ImageList = this.imageList1;
            this.btCfg.ImageTextSpace = 0;
            this.btCfg.ImageWidth = 60;
            this.btCfg.Location = new System.Drawing.Point(897, 1);
            this.btCfg.Name = "btCfg";
            this.btCfg.Radius = 24;
            this.btCfg.Size = new System.Drawing.Size(60, 60);
            this.btCfg.SpliteButtonWidth = 18;
            this.btCfg.TabIndex = 6;
            this.btCfg.UseVisualStyleBackColor = false;
            this.btCfg.Click += new System.EventHandler(this.btCfg_Click);
            // 
            // btVision
            // 
            this.btVision.BackColor = System.Drawing.Color.Transparent;
            this.btVision.BaseColor = System.Drawing.Color.White;
            this.btVision.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btVision.FlatAppearance.BorderSize = 0;
            this.btVision.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btVision.Image = ((System.Drawing.Image)(resources.GetObject("btVision.Image")));
            this.btVision.ImageHeight = 60;
            this.btVision.ImageTextSpace = 0;
            this.btVision.ImageWidth = 60;
            this.btVision.Location = new System.Drawing.Point(957, 1);
            this.btVision.Name = "btVision";
            this.btVision.Radius = 24;
            this.btVision.Size = new System.Drawing.Size(60, 60);
            this.btVision.SpliteButtonWidth = 18;
            this.btVision.TabIndex = 7;
            this.btVision.UseVisualStyleBackColor = false;
            this.btVision.Click += new System.EventHandler(this.btVision_Click);
            // 
            // btLog
            // 
            this.btLog.BackColor = System.Drawing.Color.Transparent;
            this.btLog.BaseColor = System.Drawing.Color.White;
            this.btLog.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btLog.FlatAppearance.BorderSize = 0;
            this.btLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btLog.ImageHeight = 60;
            this.btLog.ImageKey = "File_Excel_Off.png";
            this.btLog.ImageList = this.imageList1;
            this.btLog.ImageTextSpace = 0;
            this.btLog.ImageWidth = 60;
            this.btLog.Location = new System.Drawing.Point(832, 1);
            this.btLog.Name = "btLog";
            this.btLog.Radius = 24;
            this.btLog.Size = new System.Drawing.Size(60, 60);
            this.btLog.SpliteButtonWidth = 18;
            this.btLog.TabIndex = 8;
            this.btLog.UseVisualStyleBackColor = false;
            this.btLog.Click += new System.EventHandler(this.btLog_Click);
            // 
            // btUser
            // 
            this.btUser.BackColor = System.Drawing.Color.Transparent;
            this.btUser.BaseColor = System.Drawing.Color.White;
            this.btUser.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btUser.FlatAppearance.BorderSize = 0;
            this.btUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btUser.ImageHeight = 60;
            this.btUser.ImageKey = "Login_Off.png";
            this.btUser.ImageList = this.imageList1;
            this.btUser.ImageTextSpace = 0;
            this.btUser.ImageWidth = 60;
            this.btUser.Location = new System.Drawing.Point(1020, 1);
            this.btUser.Name = "btUser";
            this.btUser.Radius = 24;
            this.btUser.Size = new System.Drawing.Size(60, 60);
            this.btUser.SpliteButtonWidth = 18;
            this.btUser.TabIndex = 9;
            this.btUser.UseVisualStyleBackColor = false;
            this.btUser.Click += new System.EventHandler(this.btUser_Click);
            // 
            // btBrief
            // 
            this.btBrief.AutoSize = true;
            this.btBrief.BackColor = System.Drawing.Color.Transparent;
            this.btBrief.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btBrief.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btBrief.FlatAppearance.BorderSize = 0;
            this.btBrief.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btBrief.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btBrief.ImageHeight = 60;
            this.btBrief.ImageKey = "(无)";
            this.btBrief.ImageList = this.imageList1;
            this.btBrief.ImageTextSpace = 0;
            this.btBrief.ImageWidth = 60;
            this.btBrief.Location = new System.Drawing.Point(577, 1);
            this.btBrief.Name = "btBrief";
            this.btBrief.Radius = 24;
            this.btBrief.Size = new System.Drawing.Size(226, 60);
            this.btBrief.SpliteButtonWidth = 18;
            this.btBrief.TabIndex = 10;
            this.btBrief.Text = "直线双轨AOI";
            this.btBrief.UseVisualStyleBackColor = false;
            this.btBrief.Click += new System.EventHandler(this.btBrief_Click);
            // 
            // btStop
            // 
            this.btStop.BackColor = System.Drawing.Color.Transparent;
            this.btStop.BaseColor = System.Drawing.Color.White;
            this.btStop.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btStop.FlatAppearance.BorderSize = 0;
            this.btStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btStop.ImageHeight = 60;
            this.btStop.ImageKey = "Stop_Off.png";
            this.btStop.ImageList = this.imageList1;
            this.btStop.ImageTextSpace = 0;
            this.btStop.ImageWidth = 60;
            this.btStop.Location = new System.Drawing.Point(345, 1);
            this.btStop.Name = "btStop";
            this.btStop.Radius = 24;
            this.btStop.Size = new System.Drawing.Size(60, 60);
            this.btStop.SpliteButtonWidth = 18;
            this.btStop.TabIndex = 13;
            this.btStop.UseVisualStyleBackColor = false;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // btPause
            // 
            this.btPause.BackColor = System.Drawing.Color.Transparent;
            this.btPause.BaseColor = System.Drawing.Color.White;
            this.btPause.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btPause.FlatAppearance.BorderSize = 0;
            this.btPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btPause.ImageHeight = 60;
            this.btPause.ImageKey = "Pause_Off.png";
            this.btPause.ImageList = this.imageList1;
            this.btPause.ImageTextSpace = 0;
            this.btPause.ImageWidth = 60;
            this.btPause.Location = new System.Drawing.Point(281, 1);
            this.btPause.Name = "btPause";
            this.btPause.Radius = 24;
            this.btPause.Size = new System.Drawing.Size(60, 60);
            this.btPause.SpliteButtonWidth = 18;
            this.btPause.TabIndex = 12;
            this.btPause.UseVisualStyleBackColor = false;
            this.btPause.Click += new System.EventHandler(this.btPause_Click);
            // 
            // btStart
            // 
            this.btStart.BackColor = System.Drawing.Color.Transparent;
            this.btStart.BaseColor = System.Drawing.Color.White;
            this.btStart.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btStart.FlatAppearance.BorderSize = 0;
            this.btStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btStart.ImageHeight = 60;
            this.btStart.ImageKey = "Start_On.png";
            this.btStart.ImageList = this.imageList1;
            this.btStart.ImageTextSpace = 0;
            this.btStart.ImageWidth = 60;
            this.btStart.Location = new System.Drawing.Point(218, 1);
            this.btStart.Name = "btStart";
            this.btStart.Radius = 24;
            this.btStart.Size = new System.Drawing.Size(60, 60);
            this.btStart.SpliteButtonWidth = 18;
            this.btStart.TabIndex = 11;
            this.btStart.UseVisualStyleBackColor = false;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // picLogo
            // 
            this.picLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picLogo.Image = ((System.Drawing.Image)(resources.GetObject("picLogo.Image")));
            this.picLogo.Location = new System.Drawing.Point(1183, 1);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(180, 60);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 14;
            this.picLogo.TabStop = false;
            // 
            // btBatch
            // 
            this.btBatch.BackColor = System.Drawing.Color.Transparent;
            this.btBatch.BaseColor = System.Drawing.SystemColors.Control;
            this.btBatch.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btBatch.FlatAppearance.BorderSize = 0;
            this.btBatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btBatch.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btBatch.ImageHeight = 60;
            this.btBatch.ImageKey = "(无)";
            this.btBatch.ImageList = this.imageList1;
            this.btBatch.ImageTextSpace = 0;
            this.btBatch.ImageWidth = 60;
            this.btBatch.Location = new System.Drawing.Point(410, 1);
            this.btBatch.Name = "btBatch";
            this.btBatch.Radius = 24;
            this.btBatch.Size = new System.Drawing.Size(60, 60);
            this.btBatch.SpliteButtonWidth = 18;
            this.btBatch.TabIndex = 15;
            this.btBatch.Text = "结批";
            this.btBatch.UseVisualStyleBackColor = false;
            this.btBatch.Click += new System.EventHandler(this.btBatch_Click);
            // 
            // subFormPanel
            // 
            this.subFormPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subFormPanel.Location = new System.Drawing.Point(0, 64);
            this.subFormPanel.Name = "subFormPanel";
            this.subFormPanel.Size = new System.Drawing.Size(1363, 661);
            this.subFormPanel.TabIndex = 16;
            // 
            // timerFlush
            // 
            this.timerFlush.Enabled = true;
            this.timerFlush.Interval = 200;
            this.timerFlush.Tick += new System.EventHandler(this.timerFlush_Tick);
            // 
            // btReset
            // 
            this.btReset.BackColor = System.Drawing.Color.Transparent;
            this.btReset.BaseColor = System.Drawing.SystemColors.Control;
            this.btReset.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btReset.FlatAppearance.BorderSize = 0;
            this.btReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btReset.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btReset.ImageHeight = 60;
            this.btReset.ImageKey = "(无)";
            this.btReset.ImageList = this.imageList1;
            this.btReset.ImageTextSpace = 0;
            this.btReset.ImageWidth = 60;
            this.btReset.Location = new System.Drawing.Point(475, 1);
            this.btReset.Name = "btReset";
            this.btReset.Radius = 24;
            this.btReset.Size = new System.Drawing.Size(60, 60);
            this.btReset.SpliteButtonWidth = 18;
            this.btReset.TabIndex = 17;
            this.btReset.Text = "复位";
            this.btReset.UseVisualStyleBackColor = false;
            this.btReset.Click += new System.EventHandler(this.btReset_Click);
            // 
            // btStationRunInfo
            // 
            this.btStationRunInfo.BackColor = System.Drawing.Color.DimGray;
            this.btStationRunInfo.BaseColor = System.Drawing.Color.White;
            this.btStationRunInfo.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.btStationRunInfo.FlatAppearance.BorderSize = 0;
            this.btStationRunInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btStationRunInfo.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btStationRunInfo.ImageHeight = 60;
            this.btStationRunInfo.ImageKey = "(none)";
            this.btStationRunInfo.ImageList = this.imageList1;
            this.btStationRunInfo.ImageTextSpace = 0;
            this.btStationRunInfo.ImageWidth = 60;
            this.btStationRunInfo.Location = new System.Drawing.Point(1086, 1);
            this.btStationRunInfo.Name = "btStationRunInfo";
            this.btStationRunInfo.Radius = 24;
            this.btStationRunInfo.Size = new System.Drawing.Size(60, 60);
            this.btStationRunInfo.SpliteButtonWidth = 18;
            this.btStationRunInfo.TabIndex = 18;
            this.btStationRunInfo.Text = "SRI";
            this.btStationRunInfo.UseVisualStyleBackColor = false;
            this.btStationRunInfo.Click += new System.EventHandler(this.btStationRunInfo_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1364, 750);
            this.Controls.Add(this.btStationRunInfo);
            this.Controls.Add(this.btReset);
            this.Controls.Add(this.subFormPanel);
            this.Controls.Add(this.btBatch);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.btPause);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.btBrief);
            this.Controls.Add(this.btUser);
            this.Controls.Add(this.btLog);
            this.Controls.Add(this.btVision);
            this.Controls.Add(this.btCfg);
            this.Controls.Add(this.btAlarm);
            this.Controls.Add(this.btManual);
            this.Controls.Add(this.btAuto);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "JFA2020-直线双轨AOI";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelUserName;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelUserLevel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelProductInfo;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelRunMode;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel6;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelDevStatus;
        private JFUI.RoundButton btAuto;
        private JFUI.RoundButton btManual;
        private JFUI.RoundButton btAlarm;
        private JFUI.RoundButton btCfg;
        private System.Windows.Forms.ImageList imageList1;
        private JFUI.RoundButton btVision;
        private JFUI.RoundButton btLog;
        private JFUI.RoundButton btUser;
        private JFUI.RoundButton btBrief;
        private JFUI.RoundButton btStop;
        private JFUI.RoundButton btPause;
        private JFUI.RoundButton btStart;
        private System.Windows.Forms.PictureBox picLogo;
        private JFUI.RoundButton btBatch;
        private System.Windows.Forms.Panel subFormPanel;
        private System.Windows.Forms.Timer timerFlush;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel7;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelAlarming;
        private JFUI.RoundButton btReset;
        private JFUI.RoundButton btStationRunInfo;
    }
}