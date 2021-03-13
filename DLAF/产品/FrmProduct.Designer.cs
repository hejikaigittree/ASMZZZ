namespace DLAF
{
    partial class FrmProduct
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmProduct));
            this.prgProductMagzine = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlMagazineSschematic = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.htWindow1 = new HTHalControl.HTWindowControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.htWindow2 = new HTHalControl.HTWindowControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnBoxManager = new System.Windows.Forms.Button();
            this.btnFixTrack = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnLoadProduct = new System.Windows.Forms.Button();
            this.btnCreateProduct = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbxProductCategory = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.lblCurrentProd = new System.Windows.Forms.Label();
            this.lblSelectProd = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // prgProductMagzine
            // 
            this.prgProductMagzine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prgProductMagzine.LineColor = System.Drawing.SystemColors.ControlDark;
            this.prgProductMagzine.Location = new System.Drawing.Point(0, 100);
            this.prgProductMagzine.Margin = new System.Windows.Forms.Padding(0);
            this.prgProductMagzine.Name = "prgProductMagzine";
            this.prgProductMagzine.Size = new System.Drawing.Size(334, 623);
            this.prgProductMagzine.TabIndex = 5;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.pnlMagazineSschematic, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(626, 723);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // pnlMagazineSschematic
            // 
            this.pnlMagazineSschematic.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlMagazineSschematic.BackgroundImage")));
            this.pnlMagazineSschematic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pnlMagazineSschematic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMagazineSschematic.Location = new System.Drawing.Point(3, 3);
            this.pnlMagazineSschematic.Name = "pnlMagazineSschematic";
            this.pnlMagazineSschematic.Size = new System.Drawing.Size(620, 355);
            this.pnlMagazineSschematic.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.htWindow1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 361);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(626, 180);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "当前料片";
            // 
            // htWindow1
            // 
            this.htWindow1.BackColor = System.Drawing.Color.Transparent;
            this.htWindow1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.htWindow1.ColorName = "red";
            this.htWindow1.ColorType = 0;
            this.htWindow1.Column = null;
            this.htWindow1.Column1 = null;
            this.htWindow1.Column2 = null;
            this.htWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htWindow1.Image = null;
            this.htWindow1.Length1 = null;
            this.htWindow1.Length2 = null;
            this.htWindow1.Location = new System.Drawing.Point(3, 17);
            this.htWindow1.Margin = new System.Windows.Forms.Padding(0);
            this.htWindow1.Name = "htWindow1";
            this.htWindow1.Phi = null;
            this.htWindow1.Radius = null;
            this.htWindow1.Radius1 = null;
            this.htWindow1.Radius2 = null;
            this.htWindow1.Region = null;
            this.htWindow1.RegionType = null;
            this.htWindow1.Row = null;
            this.htWindow1.Row1 = null;
            this.htWindow1.Row2 = null;
            this.htWindow1.Size = new System.Drawing.Size(620, 160);
            this.htWindow1.TabIndex = 4;
            this.htWindow1.UmPerPix = -1D;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.htWindow2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 541);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(626, 182);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "选中料片";
            // 
            // htWindow2
            // 
            this.htWindow2.BackColor = System.Drawing.Color.Transparent;
            this.htWindow2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.htWindow2.ColorName = "red";
            this.htWindow2.ColorType = 0;
            this.htWindow2.Column = null;
            this.htWindow2.Column1 = null;
            this.htWindow2.Column2 = null;
            this.htWindow2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htWindow2.Image = null;
            this.htWindow2.Length1 = null;
            this.htWindow2.Length2 = null;
            this.htWindow2.Location = new System.Drawing.Point(3, 17);
            this.htWindow2.Margin = new System.Windows.Forms.Padding(0);
            this.htWindow2.Name = "htWindow2";
            this.htWindow2.Phi = null;
            this.htWindow2.Radius = null;
            this.htWindow2.Radius1 = null;
            this.htWindow2.Radius2 = null;
            this.htWindow2.Region = null;
            this.htWindow2.RegionType = null;
            this.htWindow2.Row = null;
            this.htWindow2.Row1 = null;
            this.htWindow2.Row2 = null;
            this.htWindow2.Size = new System.Drawing.Size(620, 162);
            this.htWindow2.TabIndex = 5;
            this.htWindow2.UmPerPix = -1D;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 340F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1292, 729);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.btnBoxManager, 0, 5);
            this.tableLayoutPanel4.Controls.Add(this.btnFixTrack, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.btnDelete, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.btnRename, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.btnLoadProduct, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.btnCreateProduct, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnSave, 0, 6);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(1175, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 7;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(114, 723);
            this.tableLayoutPanel4.TabIndex = 9;
            // 
            // btnBoxManager
            // 
            this.btnBoxManager.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnBoxManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBoxManager.FlatAppearance.BorderSize = 0;
            this.btnBoxManager.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBoxManager.ForeColor = System.Drawing.Color.White;
            this.btnBoxManager.Location = new System.Drawing.Point(1, 516);
            this.btnBoxManager.Margin = new System.Windows.Forms.Padding(1);
            this.btnBoxManager.Name = "btnBoxManager";
            this.btnBoxManager.Size = new System.Drawing.Size(112, 101);
            this.btnBoxManager.TabIndex = 85;
            this.btnBoxManager.Text = "料盒管理";
            this.btnBoxManager.UseVisualStyleBackColor = false;
            this.btnBoxManager.Click += new System.EventHandler(this.btnBoxManager_Click);
            // 
            // btnFixTrack
            // 
            this.btnFixTrack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnFixTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFixTrack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFixTrack.ForeColor = System.Drawing.Color.White;
            this.btnFixTrack.Location = new System.Drawing.Point(1, 413);
            this.btnFixTrack.Margin = new System.Windows.Forms.Padding(1);
            this.btnFixTrack.Name = "btnFixTrack";
            this.btnFixTrack.Size = new System.Drawing.Size(112, 101);
            this.btnFixTrack.TabIndex = 4;
            this.btnFixTrack.Text = "配置产品轨宽";
            this.btnFixTrack.UseVisualStyleBackColor = false;
            this.btnFixTrack.Click += new System.EventHandler(this.btnFixTrack_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(1, 310);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(1);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(112, 101);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "删除产品";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRename
            // 
            this.btnRename.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnRename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRename.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRename.ForeColor = System.Drawing.Color.White;
            this.btnRename.Location = new System.Drawing.Point(1, 207);
            this.btnRename.Margin = new System.Windows.Forms.Padding(1);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(112, 101);
            this.btnRename.TabIndex = 2;
            this.btnRename.Text = "重命名产品";
            this.btnRename.UseVisualStyleBackColor = false;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // btnLoadProduct
            // 
            this.btnLoadProduct.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnLoadProduct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoadProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadProduct.ForeColor = System.Drawing.Color.White;
            this.btnLoadProduct.Location = new System.Drawing.Point(1, 104);
            this.btnLoadProduct.Margin = new System.Windows.Forms.Padding(1);
            this.btnLoadProduct.Name = "btnLoadProduct";
            this.btnLoadProduct.Size = new System.Drawing.Size(112, 101);
            this.btnLoadProduct.TabIndex = 1;
            this.btnLoadProduct.Text = "加载产品";
            this.btnLoadProduct.UseVisualStyleBackColor = false;
            this.btnLoadProduct.Click += new System.EventHandler(this.btnLoadProduct_Click);
            // 
            // btnCreateProduct
            // 
            this.btnCreateProduct.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnCreateProduct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCreateProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateProduct.ForeColor = System.Drawing.Color.White;
            this.btnCreateProduct.Location = new System.Drawing.Point(1, 1);
            this.btnCreateProduct.Margin = new System.Windows.Forms.Padding(1);
            this.btnCreateProduct.Name = "btnCreateProduct";
            this.btnCreateProduct.Size = new System.Drawing.Size(112, 101);
            this.btnCreateProduct.TabIndex = 0;
            this.btnCreateProduct.Text = "创建产品";
            this.btnCreateProduct.UseVisualStyleBackColor = false;
            this.btnCreateProduct.Click += new System.EventHandler(this.btnCreateProduct_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(3, 622);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(108, 97);
            this.btnSave.TabIndex = 14;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbxProductCategory);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(975, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(194, 723);
            this.panel1.TabIndex = 6;
            // 
            // lbxProductCategory
            // 
            this.lbxProductCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxProductCategory.FormattingEnabled = true;
            this.lbxProductCategory.ItemHeight = 12;
            this.lbxProductCategory.Location = new System.Drawing.Point(0, 0);
            this.lbxProductCategory.Margin = new System.Windows.Forms.Padding(0);
            this.lbxProductCategory.Name = "lbxProductCategory";
            this.lbxProductCategory.Size = new System.Drawing.Size(194, 723);
            this.lbxProductCategory.TabIndex = 0;
            this.lbxProductCategory.SelectedIndexChanged += new System.EventHandler(this.lbxProductCategory_SelectedIndexChanged);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.prgProductMagzine, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(635, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(334, 723);
            this.tableLayoutPanel3.TabIndex = 8;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.lblCurrentProd, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.lblSelectProd, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.pictureBox1, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.pictureBox2, 1, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(328, 94);
            this.tableLayoutPanel5.TabIndex = 6;
            // 
            // lblCurrentProd
            // 
            this.lblCurrentProd.AutoSize = true;
            this.lblCurrentProd.Location = new System.Drawing.Point(3, 0);
            this.lblCurrentProd.Name = "lblCurrentProd";
            this.lblCurrentProd.Size = new System.Drawing.Size(71, 12);
            this.lblCurrentProd.TabIndex = 6;
            this.lblCurrentProd.Text = "当前产品:无";
            // 
            // lblSelectProd
            // 
            this.lblSelectProd.AutoSize = true;
            this.lblSelectProd.Location = new System.Drawing.Point(3, 47);
            this.lblSelectProd.Name = "lblSelectProd";
            this.lblSelectProd.Size = new System.Drawing.Size(71, 12);
            this.lblSelectProd.TabIndex = 0;
            this.lblSelectProd.Text = "选中产品:无";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(167, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(158, 41);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Location = new System.Drawing.Point(167, 50);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(158, 41);
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            // 
            // FrmProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FrmProduct";
            this.Size = new System.Drawing.Size(1292, 729);
            this.Load += new System.EventHandler(this.FrmProduct_Load);
            this.Enter += new System.EventHandler(this.frmProduct_Enter);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMagazineSschematic;
        public System.Windows.Forms.PropertyGrid prgProductMagzine;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lbxProductCategory;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lblCurrentProd;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnLoadProduct;
        private System.Windows.Forms.Button btnCreateProduct;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnFixTrack;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label lblSelectProd;
        public HTHalControl.HTWindowControl htWindow2;
        public HTHalControl.HTWindowControl htWindow1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnBoxManager;
    }
}