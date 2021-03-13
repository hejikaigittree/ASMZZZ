namespace DLAF_DS
{
    partial class UcRtTeachStation
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbVisionGrab = new System.Windows.Forms.GroupBox();
            this.tbModelPicPath = new System.Windows.Forms.TextBox();
            this.btSetMdelSavePath = new System.Windows.Forms.Button();
            this.btGrabModelPics = new System.Windows.Forms.Button();
            this.btChkCfg = new System.Windows.Forms.Button();
            this.btFlushVc = new System.Windows.Forms.Button();
            this.gbTaskSave = new System.Windows.Forms.GroupBox();
            this.btFixTask = new System.Windows.Forms.Button();
            this.btTaskVcEditCancel = new System.Windows.Forms.Button();
            this.btTaskVcEditSave = new System.Windows.Forms.Button();
            this.cbTaskVc = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cbTaskName = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.gbFixProduct = new System.Windows.Forms.GroupBox();
            this.chkVisionFixProduct = new System.Windows.Forms.CheckBox();
            this.lbMarPos = new System.Windows.Forms.Label();
            this.cbMarkVc2 = new System.Windows.Forms.ComboBox();
            this.cbMarkVc1 = new System.Windows.Forms.ComboBox();
            this.btMark2EditSave = new System.Windows.Forms.Button();
            this.btMark1EditSave = new System.Windows.Forms.Button();
            this.btFixMark2 = new System.Windows.Forms.Button();
            this.btFixMark1 = new System.Windows.Forms.Button();
            this.btFixProduct = new System.Windows.Forms.Button();
            this.gbFixFov = new System.Windows.Forms.GroupBox();
            this.lbFovOffset = new System.Windows.Forms.Label();
            this.btInspectCurrFov = new System.Windows.Forms.Button();
            this.cbFovName = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btFixFov = new System.Windows.Forms.Button();
            this.gbFixIC = new System.Windows.Forms.GroupBox();
            this.lbICPos = new System.Windows.Forms.Label();
            this.btFixIC = new System.Windows.Forms.Button();
            this.cbIcCol = new System.Windows.Forms.ComboBox();
            this.cbIcRow = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbVAName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbRecipeID = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbVisionGrab.SuspendLayout();
            this.gbTaskSave.SuspendLayout();
            this.gbFixProduct.SuspendLayout();
            this.gbFixFov.SuspendLayout();
            this.gbFixIC.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.splitContainer1.Panel1.Controls.Add(this.gbVisionGrab);
            this.splitContainer1.Panel1.Controls.Add(this.btChkCfg);
            this.splitContainer1.Panel1.Controls.Add(this.btFlushVc);
            this.splitContainer1.Panel1.Controls.Add(this.gbTaskSave);
            this.splitContainer1.Panel1.Controls.Add(this.gbFixProduct);
            this.splitContainer1.Panel1.Controls.Add(this.gbFixFov);
            this.splitContainer1.Panel1.Controls.Add(this.gbFixIC);
            this.splitContainer1.Panel1.Controls.Add(this.cbVAName);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.cbRecipeID);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(1303, 630);
            this.splitContainer1.SplitterDistance = 221;
            this.splitContainer1.TabIndex = 0;
            // 
            // gbVisionGrab
            // 
            this.gbVisionGrab.Controls.Add(this.tbModelPicPath);
            this.gbVisionGrab.Controls.Add(this.btSetMdelSavePath);
            this.gbVisionGrab.Controls.Add(this.btGrabModelPics);
            this.gbVisionGrab.Enabled = false;
            this.gbVisionGrab.Location = new System.Drawing.Point(8, 454);
            this.gbVisionGrab.Name = "gbVisionGrab";
            this.gbVisionGrab.Size = new System.Drawing.Size(206, 70);
            this.gbVisionGrab.TabIndex = 12;
            this.gbVisionGrab.TabStop = false;
            this.gbVisionGrab.Text = "图像模板采集/测试";
            // 
            // tbModelPicPath
            // 
            this.tbModelPicPath.BackColor = System.Drawing.SystemColors.Control;
            this.tbModelPicPath.Location = new System.Drawing.Point(83, 15);
            this.tbModelPicPath.Name = "tbModelPicPath";
            this.tbModelPicPath.ReadOnly = true;
            this.tbModelPicPath.Size = new System.Drawing.Size(117, 21);
            this.tbModelPicPath.TabIndex = 3;
            // 
            // btSetMdelSavePath
            // 
            this.btSetMdelSavePath.Location = new System.Drawing.Point(11, 15);
            this.btSetMdelSavePath.Name = "btSetMdelSavePath";
            this.btSetMdelSavePath.Size = new System.Drawing.Size(70, 23);
            this.btSetMdelSavePath.TabIndex = 2;
            this.btSetMdelSavePath.Text = "保存路径";
            this.btSetMdelSavePath.UseVisualStyleBackColor = true;
            this.btSetMdelSavePath.Click += new System.EventHandler(this.btSetMdelSavePath_Click);
            // 
            // btGrabModelPics
            // 
            this.btGrabModelPics.Location = new System.Drawing.Point(11, 40);
            this.btGrabModelPics.Name = "btGrabModelPics";
            this.btGrabModelPics.Size = new System.Drawing.Size(190, 23);
            this.btGrabModelPics.TabIndex = 0;
            this.btGrabModelPics.Text = "采集图像模板";
            this.btGrabModelPics.UseVisualStyleBackColor = true;
            this.btGrabModelPics.Click += new System.EventHandler(this.btGrabModelPics_Click);
            // 
            // btChkCfg
            // 
            this.btChkCfg.Enabled = false;
            this.btChkCfg.Location = new System.Drawing.Point(19, 528);
            this.btChkCfg.Name = "btChkCfg";
            this.btChkCfg.Size = new System.Drawing.Size(130, 23);
            this.btChkCfg.TabIndex = 11;
            this.btChkCfg.Text = "检查Recipe参数配置";
            this.btChkCfg.UseVisualStyleBackColor = true;
            this.btChkCfg.Click += new System.EventHandler(this.btChkCfg_Click);
            // 
            // btFlushVc
            // 
            this.btFlushVc.Enabled = false;
            this.btFlushVc.Location = new System.Drawing.Point(19, 554);
            this.btFlushVc.Name = "btFlushVc";
            this.btFlushVc.Size = new System.Drawing.Size(130, 23);
            this.btFlushVc.TabIndex = 8;
            this.btFlushVc.Text = "更新视觉参数列表";
            this.btFlushVc.UseVisualStyleBackColor = true;
            this.btFlushVc.Click += new System.EventHandler(this.btFlushVc_Click);
            // 
            // gbTaskSave
            // 
            this.gbTaskSave.Controls.Add(this.btFixTask);
            this.gbTaskSave.Controls.Add(this.btTaskVcEditCancel);
            this.gbTaskSave.Controls.Add(this.btTaskVcEditSave);
            this.gbTaskSave.Controls.Add(this.cbTaskVc);
            this.gbTaskSave.Controls.Add(this.label10);
            this.gbTaskSave.Controls.Add(this.cbTaskName);
            this.gbTaskSave.Controls.Add(this.label6);
            this.gbTaskSave.Enabled = false;
            this.gbTaskSave.Location = new System.Drawing.Point(8, 350);
            this.gbTaskSave.Name = "gbTaskSave";
            this.gbTaskSave.Size = new System.Drawing.Size(206, 101);
            this.gbTaskSave.TabIndex = 7;
            this.gbTaskSave.TabStop = false;
            this.gbTaskSave.Text = "Task";
            // 
            // btFixTask
            // 
            this.btFixTask.Location = new System.Drawing.Point(9, 73);
            this.btFixTask.Name = "btFixTask";
            this.btFixTask.Size = new System.Drawing.Size(103, 21);
            this.btFixTask.TabIndex = 16;
            this.btFixTask.Text = "调整当前VC";
            this.btFixTask.UseVisualStyleBackColor = true;
            this.btFixTask.Click += new System.EventHandler(this.btFixTask_Click);
            // 
            // btTaskVcEditCancel
            // 
            this.btTaskVcEditCancel.Enabled = false;
            this.btTaskVcEditCancel.Location = new System.Drawing.Point(162, 72);
            this.btTaskVcEditCancel.Name = "btTaskVcEditCancel";
            this.btTaskVcEditCancel.Size = new System.Drawing.Size(39, 23);
            this.btTaskVcEditCancel.TabIndex = 10;
            this.btTaskVcEditCancel.Text = "取消";
            this.btTaskVcEditCancel.UseVisualStyleBackColor = true;
            this.btTaskVcEditCancel.Click += new System.EventHandler(this.btTaskVcEditCancel_Click);
            // 
            // btTaskVcEditSave
            // 
            this.btTaskVcEditSave.Location = new System.Drawing.Point(121, 72);
            this.btTaskVcEditSave.Name = "btTaskVcEditSave";
            this.btTaskVcEditSave.Size = new System.Drawing.Size(39, 23);
            this.btTaskVcEditSave.TabIndex = 9;
            this.btTaskVcEditSave.Text = "设置";
            this.btTaskVcEditSave.UseVisualStyleBackColor = true;
            this.btTaskVcEditSave.Click += new System.EventHandler(this.btTaskVcEditSave_Click);
            // 
            // cbTaskVc
            // 
            this.cbTaskVc.Enabled = false;
            this.cbTaskVc.FormattingEnabled = true;
            this.cbTaskVc.Location = new System.Drawing.Point(67, 46);
            this.cbTaskVc.Name = "cbTaskVc";
            this.cbTaskVc.Size = new System.Drawing.Size(133, 20);
            this.cbTaskVc.TabIndex = 8;
            this.cbTaskVc.SelectedIndexChanged += new System.EventHandler(this.cbTaskVc_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 50);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 12);
            this.label10.TabIndex = 7;
            this.label10.Text = "视觉配置:";
            // 
            // cbTaskName
            // 
            this.cbTaskName.FormattingEnabled = true;
            this.cbTaskName.Location = new System.Drawing.Point(66, 20);
            this.cbTaskName.Name = "cbTaskName";
            this.cbTaskName.Size = new System.Drawing.Size(133, 20);
            this.cbTaskName.TabIndex = 6;
            this.cbTaskName.SelectedIndexChanged += new System.EventHandler(this.cbTaskName_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "Task Name:";
            // 
            // gbFixProduct
            // 
            this.gbFixProduct.Controls.Add(this.chkVisionFixProduct);
            this.gbFixProduct.Controls.Add(this.lbMarPos);
            this.gbFixProduct.Controls.Add(this.cbMarkVc2);
            this.gbFixProduct.Controls.Add(this.cbMarkVc1);
            this.gbFixProduct.Controls.Add(this.btMark2EditSave);
            this.gbFixProduct.Controls.Add(this.btMark1EditSave);
            this.gbFixProduct.Controls.Add(this.btFixMark2);
            this.gbFixProduct.Controls.Add(this.btFixMark1);
            this.gbFixProduct.Controls.Add(this.btFixProduct);
            this.gbFixProduct.Enabled = false;
            this.gbFixProduct.Location = new System.Drawing.Point(8, 60);
            this.gbFixProduct.Name = "gbFixProduct";
            this.gbFixProduct.Size = new System.Drawing.Size(206, 118);
            this.gbFixProduct.TabIndex = 6;
            this.gbFixProduct.TabStop = false;
            this.gbFixProduct.Text = "产品";
            // 
            // chkVisionFixProduct
            // 
            this.chkVisionFixProduct.AutoSize = true;
            this.chkVisionFixProduct.Location = new System.Drawing.Point(9, 90);
            this.chkVisionFixProduct.Name = "chkVisionFixProduct";
            this.chkVisionFixProduct.Size = new System.Drawing.Size(96, 16);
            this.chkVisionFixProduct.TabIndex = 12;
            this.chkVisionFixProduct.Text = "使用视觉矫正";
            this.chkVisionFixProduct.UseVisualStyleBackColor = true;
            // 
            // lbMarPos
            // 
            this.lbMarPos.AutoSize = true;
            this.lbMarPos.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbMarPos.Location = new System.Drawing.Point(7, 62);
            this.lbMarPos.Name = "lbMarPos";
            this.lbMarPos.Size = new System.Drawing.Size(45, 10);
            this.lbMarPos.TabIndex = 11;
            this.lbMarPos.Text = "Mark坐标";
            // 
            // cbMarkVc2
            // 
            this.cbMarkVc2.FormattingEnabled = true;
            this.cbMarkVc2.Location = new System.Drawing.Point(34, 40);
            this.cbMarkVc2.Name = "cbMarkVc2";
            this.cbMarkVc2.Size = new System.Drawing.Size(101, 20);
            this.cbMarkVc2.TabIndex = 10;
            this.cbMarkVc2.SelectedIndexChanged += new System.EventHandler(this.cbMarkVc2_SelectedIndexChanged);
            // 
            // cbMarkVc1
            // 
            this.cbMarkVc1.FormattingEnabled = true;
            this.cbMarkVc1.Location = new System.Drawing.Point(34, 15);
            this.cbMarkVc1.Name = "cbMarkVc1";
            this.cbMarkVc1.Size = new System.Drawing.Size(101, 20);
            this.cbMarkVc1.TabIndex = 9;
            this.cbMarkVc1.SelectedIndexChanged += new System.EventHandler(this.cbMarkVc1_SelectedIndexChanged);
            // 
            // btMark2EditSave
            // 
            this.btMark2EditSave.Location = new System.Drawing.Point(7, 38);
            this.btMark2EditSave.Margin = new System.Windows.Forms.Padding(0);
            this.btMark2EditSave.Name = "btMark2EditSave";
            this.btMark2EditSave.Size = new System.Drawing.Size(25, 23);
            this.btMark2EditSave.TabIndex = 8;
            this.btMark2EditSave.Text = "Vc";
            this.btMark2EditSave.UseVisualStyleBackColor = true;
            this.btMark2EditSave.Click += new System.EventHandler(this.btMark2EditSave_Click);
            // 
            // btMark1EditSave
            // 
            this.btMark1EditSave.Location = new System.Drawing.Point(7, 14);
            this.btMark1EditSave.Margin = new System.Windows.Forms.Padding(0);
            this.btMark1EditSave.Name = "btMark1EditSave";
            this.btMark1EditSave.Size = new System.Drawing.Size(25, 23);
            this.btMark1EditSave.TabIndex = 7;
            this.btMark1EditSave.Text = "Vc";
            this.btMark1EditSave.UseVisualStyleBackColor = true;
            this.btMark1EditSave.Click += new System.EventHandler(this.btMark1EditSave_Click);
            // 
            // btFixMark2
            // 
            this.btFixMark2.Location = new System.Drawing.Point(135, 38);
            this.btFixMark2.Name = "btFixMark2";
            this.btFixMark2.Size = new System.Drawing.Size(67, 23);
            this.btFixMark2.TabIndex = 6;
            this.btFixMark2.Text = "定位Mark2";
            this.btFixMark2.UseVisualStyleBackColor = true;
            this.btFixMark2.Click += new System.EventHandler(this.btFixMark2_Click);
            // 
            // btFixMark1
            // 
            this.btFixMark1.Location = new System.Drawing.Point(135, 13);
            this.btFixMark1.Name = "btFixMark1";
            this.btFixMark1.Size = new System.Drawing.Size(67, 23);
            this.btFixMark1.TabIndex = 5;
            this.btFixMark1.Text = "定位Mark1";
            this.btFixMark1.UseVisualStyleBackColor = true;
            this.btFixMark1.Click += new System.EventHandler(this.btFixMark1_Click);
            // 
            // btFixProduct
            // 
            this.btFixProduct.Location = new System.Drawing.Point(135, 86);
            this.btFixProduct.Name = "btFixProduct";
            this.btFixProduct.Size = new System.Drawing.Size(67, 23);
            this.btFixProduct.TabIndex = 4;
            this.btFixProduct.Text = "定位产品";
            this.btFixProduct.UseVisualStyleBackColor = true;
            this.btFixProduct.Click += new System.EventHandler(this.btFixMark_Click);
            // 
            // gbFixFov
            // 
            this.gbFixFov.Controls.Add(this.lbFovOffset);
            this.gbFixFov.Controls.Add(this.btInspectCurrFov);
            this.gbFixFov.Controls.Add(this.cbFovName);
            this.gbFixFov.Controls.Add(this.label5);
            this.gbFixFov.Controls.Add(this.btFixFov);
            this.gbFixFov.Location = new System.Drawing.Point(8, 253);
            this.gbFixFov.Name = "gbFixFov";
            this.gbFixFov.Size = new System.Drawing.Size(206, 93);
            this.gbFixFov.TabIndex = 6;
            this.gbFixFov.TabStop = false;
            this.gbFixFov.Text = "FOV";
            // 
            // lbFovOffset
            // 
            this.lbFovOffset.AutoSize = true;
            this.lbFovOffset.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbFovOffset.Location = new System.Drawing.Point(9, 45);
            this.lbFovOffset.Name = "lbFovOffset";
            this.lbFovOffset.Size = new System.Drawing.Size(55, 10);
            this.lbFovOffset.TabIndex = 14;
            this.lbFovOffset.Text = "FovOffset:";
            // 
            // btInspectCurrFov
            // 
            this.btInspectCurrFov.Location = new System.Drawing.Point(9, 67);
            this.btInspectCurrFov.Name = "btInspectCurrFov";
            this.btInspectCurrFov.Size = new System.Drawing.Size(192, 23);
            this.btInspectCurrFov.TabIndex = 1;
            this.btInspectCurrFov.Text = "Fov视觉检测";
            this.btInspectCurrFov.UseVisualStyleBackColor = true;
            this.btInspectCurrFov.Click += new System.EventHandler(this.btInspectCurrFov_Click);
            // 
            // cbFovName
            // 
            this.cbFovName.FormattingEnabled = true;
            this.cbFovName.Location = new System.Drawing.Point(66, 16);
            this.cbFovName.Name = "cbFovName";
            this.cbFovName.Size = new System.Drawing.Size(134, 20);
            this.cbFovName.TabIndex = 6;
            this.cbFovName.SelectedIndexChanged += new System.EventHandler(this.cbFovName_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "Fov Name:";
            // 
            // btFixFov
            // 
            this.btFixFov.Location = new System.Drawing.Point(135, 41);
            this.btFixFov.Name = "btFixFov";
            this.btFixFov.Size = new System.Drawing.Size(64, 21);
            this.btFixFov.TabIndex = 0;
            this.btFixFov.Text = "定位Fov";
            this.btFixFov.UseVisualStyleBackColor = true;
            this.btFixFov.Click += new System.EventHandler(this.btFixFov_Click);
            // 
            // gbFixIC
            // 
            this.gbFixIC.Controls.Add(this.lbICPos);
            this.gbFixIC.Controls.Add(this.btFixIC);
            this.gbFixIC.Controls.Add(this.cbIcCol);
            this.gbFixIC.Controls.Add(this.cbIcRow);
            this.gbFixIC.Controls.Add(this.label4);
            this.gbFixIC.Controls.Add(this.label3);
            this.gbFixIC.Enabled = false;
            this.gbFixIC.Location = new System.Drawing.Point(8, 181);
            this.gbFixIC.Name = "gbFixIC";
            this.gbFixIC.Size = new System.Drawing.Size(206, 70);
            this.gbFixIC.TabIndex = 5;
            this.gbFixIC.TabStop = false;
            this.gbFixIC.Text = "IC中心";
            // 
            // lbICPos
            // 
            this.lbICPos.AutoSize = true;
            this.lbICPos.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbICPos.Location = new System.Drawing.Point(7, 46);
            this.lbICPos.Name = "lbICPos";
            this.lbICPos.Size = new System.Drawing.Size(35, 10);
            this.lbICPos.TabIndex = 12;
            this.lbICPos.Text = "IC坐标";
            // 
            // btFixIC
            // 
            this.btFixIC.Location = new System.Drawing.Point(135, 41);
            this.btFixIC.Name = "btFixIC";
            this.btFixIC.Size = new System.Drawing.Size(65, 23);
            this.btFixIC.TabIndex = 4;
            this.btFixIC.Text = "定位IC";
            this.btFixIC.UseVisualStyleBackColor = true;
            this.btFixIC.Click += new System.EventHandler(this.btFixIC_Click);
            // 
            // cbIcCol
            // 
            this.cbIcCol.FormattingEnabled = true;
            this.cbIcCol.Location = new System.Drawing.Point(139, 17);
            this.cbIcCol.Name = "cbIcCol";
            this.cbIcCol.Size = new System.Drawing.Size(60, 20);
            this.cbIcCol.TabIndex = 3;
            this.cbIcCol.SelectedIndexChanged += new System.EventHandler(this.cbIcCol_SelectedIndexChanged);
            // 
            // cbIcRow
            // 
            this.cbIcRow.FormattingEnabled = true;
            this.cbIcRow.Location = new System.Drawing.Point(39, 17);
            this.cbIcRow.Name = "cbIcRow";
            this.cbIcRow.Size = new System.Drawing.Size(60, 20);
            this.cbIcRow.TabIndex = 2;
            this.cbIcRow.SelectedIndexChanged += new System.EventHandler(this.cbIcRow_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(108, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Col:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Row:";
            // 
            // cbVAName
            // 
            this.cbVAName.FormattingEnabled = true;
            this.cbVAName.Location = new System.Drawing.Point(87, 36);
            this.cbVAName.Name = "cbVAName";
            this.cbVAName.Size = new System.Drawing.Size(127, 20);
            this.cbVAName.TabIndex = 3;
            this.cbVAName.SelectedIndexChanged += new System.EventHandler(this.cbVAName_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "选择示教助手:";
            // 
            // cbRecipeID
            // 
            this.cbRecipeID.FormattingEnabled = true;
            this.cbRecipeID.Location = new System.Drawing.Point(87, 9);
            this.cbRecipeID.Name = "cbRecipeID";
            this.cbRecipeID.Size = new System.Drawing.Size(127, 20);
            this.cbRecipeID.TabIndex = 1;
            this.cbRecipeID.SelectedIndexChanged += new System.EventHandler(this.cbRecipeID_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "选择RecipeID:";
            // 
            // UcRtTeachStation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UcRtTeachStation";
            this.Size = new System.Drawing.Size(1303, 630);
            this.Load += new System.EventHandler(this.UcRtTeachStation_Load);
            this.VisibleChanged += new System.EventHandler(this.UcRtTeachStation_VisibleChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gbVisionGrab.ResumeLayout(false);
            this.gbVisionGrab.PerformLayout();
            this.gbTaskSave.ResumeLayout(false);
            this.gbTaskSave.PerformLayout();
            this.gbFixProduct.ResumeLayout(false);
            this.gbFixProduct.PerformLayout();
            this.gbFixFov.ResumeLayout(false);
            this.gbFixFov.PerformLayout();
            this.gbFixIC.ResumeLayout(false);
            this.gbFixIC.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbTaskSave;
        private System.Windows.Forms.ComboBox cbTaskName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox gbFixProduct;
        private System.Windows.Forms.Button btFixProduct;
        private System.Windows.Forms.GroupBox gbFixFov;
        private System.Windows.Forms.ComboBox cbFovName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btFixFov;
        private System.Windows.Forms.GroupBox gbFixIC;
        private System.Windows.Forms.Button btFixIC;
        private System.Windows.Forms.ComboBox cbIcCol;
        private System.Windows.Forms.ComboBox cbIcRow;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbVAName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbRecipeID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btFixMark2;
        private System.Windows.Forms.Button btFixMark1;
        private System.Windows.Forms.Button btTaskVcEditCancel;
        private System.Windows.Forms.Button btTaskVcEditSave;
        private System.Windows.Forms.ComboBox cbTaskVc;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbMarPos;
        private System.Windows.Forms.ComboBox cbMarkVc2;
        private System.Windows.Forms.ComboBox cbMarkVc1;
        private System.Windows.Forms.Button btMark2EditSave;
        private System.Windows.Forms.Button btMark1EditSave;
        private System.Windows.Forms.Label lbFovOffset;
        private System.Windows.Forms.Button btFlushVc;
        private System.Windows.Forms.Button btFixTask;
        private System.Windows.Forms.Label lbICPos;
        private System.Windows.Forms.Button btChkCfg;
        private System.Windows.Forms.CheckBox chkVisionFixProduct;
        private System.Windows.Forms.GroupBox gbVisionGrab;
        private System.Windows.Forms.Button btInspectCurrFov;
        private System.Windows.Forms.Button btGrabModelPics;
        private System.Windows.Forms.TextBox tbModelPicPath;
        private System.Windows.Forms.Button btSetMdelSavePath;
    }
}
