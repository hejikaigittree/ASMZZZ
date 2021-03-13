namespace DLAF
{
    partial class FormUV2XY
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUV2XY));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvUVXY2D = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.htWindowCalibration = new HTHalControl.HTWindowControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSnap = new System.Windows.Forms.Button();
            this.btnTool = new System.Windows.Forms.Button();
            this.btnSaveUVXY2D = new System.Windows.Forms.Button();
            this.btnUVXYCalib2D = new System.Windows.Forms.Button();
            this.btnSaveCalibModel2D = new System.Windows.Forms.Button();
            this.btnCreateCalibModel2D = new System.Windows.Forms.Button();
            this.btnCmrAxisTool = new System.Windows.Forms.Button();
            this.btnSavePara = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUVXY2D)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 623F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 168F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.htWindowCalibration, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1444, 882);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(650, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(617, 870);
            this.panel2.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvUVXY2D);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(617, 193);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "当前相机标定矩阵";
            // 
            // dgvUVXY2D
            // 
            this.dgvUVXY2D.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUVXY2D.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.dgvUVXY2D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUVXY2D.Location = new System.Drawing.Point(3, 19);
            this.dgvUVXY2D.Margin = new System.Windows.Forms.Padding(4);
            this.dgvUVXY2D.Name = "dgvUVXY2D";
            this.dgvUVXY2D.RowTemplate.Height = 23;
            this.dgvUVXY2D.Size = new System.Drawing.Size(611, 171);
            this.dgvUVXY2D.TabIndex = 5;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.FillWeight = 300F;
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.FillWeight = 300F;
            this.Column2.HeaderText = "";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.FillWeight = 300F;
            this.Column3.HeaderText = "";
            this.Column3.Name = "Column3";
            // 
            // htWindowCalibration
            // 
            this.htWindowCalibration.BackColor = System.Drawing.Color.Transparent;
            this.htWindowCalibration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.htWindowCalibration.ColorName = "red";
            this.htWindowCalibration.ColorType = 0;
            this.htWindowCalibration.Column = null;
            this.htWindowCalibration.Column1 = null;
            this.htWindowCalibration.Column2 = null;
            this.htWindowCalibration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htWindowCalibration.Image = null;
            this.htWindowCalibration.Length1 = null;
            this.htWindowCalibration.Length2 = null;
            this.htWindowCalibration.Location = new System.Drawing.Point(8, 9);
            this.htWindowCalibration.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.htWindowCalibration.Name = "htWindowCalibration";
            this.htWindowCalibration.Phi = null;
            this.htWindowCalibration.Radius = null;
            this.htWindowCalibration.Radius1 = null;
            this.htWindowCalibration.Radius2 = null;
            this.htWindowCalibration.Region = null;
            this.htWindowCalibration.RegionType = null;
            this.htWindowCalibration.Row = null;
            this.htWindowCalibration.Row1 = null;
            this.htWindowCalibration.Row2 = null;
            this.htWindowCalibration.Size = new System.Drawing.Size(631, 864);
            this.htWindowCalibration.TabIndex = 2;
            this.htWindowCalibration.UmPerPix = -1D;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1277, 7);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(160, 868);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.btnSavePara, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.btnSnap, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnTool, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.btnSaveUVXY2D, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.btnUVXYCalib2D, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.btnSaveCalibModel2D, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.btnCreateCalibModel2D, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnCmrAxisTool, 0, 5);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 8;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49906F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49906F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49906F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49906F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49906F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49906F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.50281F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.50281F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(160, 868);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // btnSnap
            // 
            this.btnSnap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnSnap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSnap.FlatAppearance.BorderSize = 0;
            this.btnSnap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSnap.ForeColor = System.Drawing.Color.White;
            this.btnSnap.Location = new System.Drawing.Point(4, 4);
            this.btnSnap.Margin = new System.Windows.Forms.Padding(1);
            this.btnSnap.Name = "btnSnap";
            this.btnSnap.Size = new System.Drawing.Size(152, 103);
            this.btnSnap.TabIndex = 75;
            this.btnSnap.Text = "单张采集";
            this.btnSnap.UseVisualStyleBackColor = false;
            this.btnSnap.Click += new System.EventHandler(this.btnSnap_Click);
            // 
            // btnTool
            // 
            this.btnTool.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnTool.FlatAppearance.BorderSize = 0;
            this.btnTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTool.ForeColor = System.Drawing.Color.White;
            this.btnTool.Location = new System.Drawing.Point(4, 652);
            this.btnTool.Margin = new System.Windows.Forms.Padding(1);
            this.btnTool.Name = "btnTool";
            this.btnTool.Size = new System.Drawing.Size(152, 103);
            this.btnTool.TabIndex = 76;
            this.btnTool.Text = "打开轴调试助手";
            this.btnTool.UseVisualStyleBackColor = false;
            this.btnTool.Click += new System.EventHandler(this.btnTool_Click);
            // 
            // btnSaveUVXY2D
            // 
            this.btnSaveUVXY2D.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnSaveUVXY2D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSaveUVXY2D.FlatAppearance.BorderSize = 0;
            this.btnSaveUVXY2D.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveUVXY2D.ForeColor = System.Drawing.Color.White;
            this.btnSaveUVXY2D.Location = new System.Drawing.Point(4, 436);
            this.btnSaveUVXY2D.Margin = new System.Windows.Forms.Padding(1);
            this.btnSaveUVXY2D.Name = "btnSaveUVXY2D";
            this.btnSaveUVXY2D.Size = new System.Drawing.Size(152, 103);
            this.btnSaveUVXY2D.TabIndex = 74;
            this.btnSaveUVXY2D.Text = "保存标定结果";
            this.btnSaveUVXY2D.UseVisualStyleBackColor = false;
            this.btnSaveUVXY2D.Click += new System.EventHandler(this.btnSaveUVXY2D_Click);
            // 
            // btnUVXYCalib2D
            // 
            this.btnUVXYCalib2D.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnUVXYCalib2D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUVXYCalib2D.FlatAppearance.BorderSize = 0;
            this.btnUVXYCalib2D.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUVXYCalib2D.ForeColor = System.Drawing.Color.White;
            this.btnUVXYCalib2D.Location = new System.Drawing.Point(4, 328);
            this.btnUVXYCalib2D.Margin = new System.Windows.Forms.Padding(1);
            this.btnUVXYCalib2D.Name = "btnUVXYCalib2D";
            this.btnUVXYCalib2D.Size = new System.Drawing.Size(152, 103);
            this.btnUVXYCalib2D.TabIndex = 73;
            this.btnUVXYCalib2D.Text = "UV2XY标定";
            this.btnUVXYCalib2D.UseVisualStyleBackColor = false;
            this.btnUVXYCalib2D.Click += new System.EventHandler(this.btnUVXYCalib2D_Click);
            // 
            // btnSaveCalibModel2D
            // 
            this.btnSaveCalibModel2D.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnSaveCalibModel2D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSaveCalibModel2D.FlatAppearance.BorderSize = 0;
            this.btnSaveCalibModel2D.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCalibModel2D.ForeColor = System.Drawing.Color.White;
            this.btnSaveCalibModel2D.Location = new System.Drawing.Point(4, 220);
            this.btnSaveCalibModel2D.Margin = new System.Windows.Forms.Padding(1);
            this.btnSaveCalibModel2D.Name = "btnSaveCalibModel2D";
            this.btnSaveCalibModel2D.Size = new System.Drawing.Size(152, 103);
            this.btnSaveCalibModel2D.TabIndex = 72;
            this.btnSaveCalibModel2D.Text = "保存标定模板";
            this.btnSaveCalibModel2D.UseVisualStyleBackColor = false;
            this.btnSaveCalibModel2D.Click += new System.EventHandler(this.btnSaveCalibModel2D_Click);
            // 
            // btnCreateCalibModel2D
            // 
            this.btnCreateCalibModel2D.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnCreateCalibModel2D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCreateCalibModel2D.FlatAppearance.BorderSize = 0;
            this.btnCreateCalibModel2D.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateCalibModel2D.ForeColor = System.Drawing.Color.White;
            this.btnCreateCalibModel2D.Location = new System.Drawing.Point(4, 112);
            this.btnCreateCalibModel2D.Margin = new System.Windows.Forms.Padding(1);
            this.btnCreateCalibModel2D.Name = "btnCreateCalibModel2D";
            this.btnCreateCalibModel2D.Size = new System.Drawing.Size(152, 103);
            this.btnCreateCalibModel2D.TabIndex = 71;
            this.btnCreateCalibModel2D.Text = "创建标定模板";
            this.btnCreateCalibModel2D.UseVisualStyleBackColor = false;
            this.btnCreateCalibModel2D.Click += new System.EventHandler(this.btnCreateCalibModel2D_Click);
            // 
            // btnCmrAxisTool
            // 
            this.btnCmrAxisTool.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnCmrAxisTool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCmrAxisTool.FlatAppearance.BorderSize = 0;
            this.btnCmrAxisTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCmrAxisTool.ForeColor = System.Drawing.Color.White;
            this.btnCmrAxisTool.Location = new System.Drawing.Point(4, 544);
            this.btnCmrAxisTool.Margin = new System.Windows.Forms.Padding(1);
            this.btnCmrAxisTool.Name = "btnCmrAxisTool";
            this.btnCmrAxisTool.Size = new System.Drawing.Size(152, 103);
            this.btnCmrAxisTool.TabIndex = 84;
            this.btnCmrAxisTool.Text = "相机运动助手";
            this.btnCmrAxisTool.UseVisualStyleBackColor = false;
            this.btnCmrAxisTool.Click += new System.EventHandler(this.btnCmrAxisTool_Click);
            // 
            // btnSavePara
            // 
            this.btnSavePara.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSavePara.BackgroundImage")));
            this.btnSavePara.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSavePara.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSavePara.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSavePara.Location = new System.Drawing.Point(6, 762);
            this.btnSavePara.Name = "btnSavePara";
            this.btnSavePara.Size = new System.Drawing.Size(148, 100);
            this.btnSavePara.TabIndex = 83;
            this.btnSavePara.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSavePara.UseVisualStyleBackColor = true;
            this.btnSavePara.Click += new System.EventHandler(this.btnSavePara_Click);
            // 
            // FormUV2XY
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1444, 882);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormUV2XY";
            this.Text = "FormUV2XY";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUVXY2D)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public HTHalControl.HTWindowControl htWindowCalibration;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnSaveCalibModel2D;
        private System.Windows.Forms.Button btnCreateCalibModel2D;
        private System.Windows.Forms.Button btnUVXYCalib2D;
        private System.Windows.Forms.Button btnSaveUVXY2D;
        private System.Windows.Forms.DataGridView dgvUVXY2D;
        private System.Windows.Forms.Button btnSnap;
        private System.Windows.Forms.Button btnTool;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.Button btnCmrAxisTool;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnSavePara;
    }
}