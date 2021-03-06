namespace JFHub
{
    partial class FormCreateInitor
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
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.lbTips = new System.Windows.Forms.Label();
            this.dgvTypes = new System.Windows.Forms.DataGridView();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBrief = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.tbID = new System.Windows.Forms.TextBox();
            this.gbParams = new System.Windows.Forms.GroupBox();
            this.panelParams = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTypes)).BeginInit();
            this.gbParams.SuspendLayout();
            this.SuspendLayout();
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.Location = new System.Drawing.Point(573, 384);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(72, 23);
            this.btOK.TabIndex = 1;
            this.btOK.Text = "创建";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.Location = new System.Drawing.Point(647, 383);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // lbTips
            // 
            this.lbTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbTips.AutoSize = true;
            this.lbTips.Location = new System.Drawing.Point(4, 390);
            this.lbTips.Name = "lbTips";
            this.lbTips.Size = new System.Drawing.Size(35, 12);
            this.lbTips.TabIndex = 3;
            this.lbTips.Text = "信息:";
            // 
            // dgvTypes
            // 
            this.dgvTypes.AllowUserToAddRows = false;
            this.dgvTypes.AllowUserToDeleteRows = false;
            this.dgvTypes.AllowUserToResizeRows = false;
            this.dgvTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvTypes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTypes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnType,
            this.ColumnBrief});
            this.dgvTypes.Location = new System.Drawing.Point(1, 1);
            this.dgvTypes.Name = "dgvTypes";
            this.dgvTypes.ReadOnly = true;
            this.dgvTypes.RowHeadersVisible = false;
            this.dgvTypes.RowTemplate.Height = 23;
            this.dgvTypes.Size = new System.Drawing.Size(342, 377);
            this.dgvTypes.TabIndex = 4;
            this.dgvTypes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTypes_CellClick);
            // 
            // ColumnType
            // 
            this.ColumnType.HeaderText = "类型";
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.ReadOnly = true;
            // 
            // ColumnBrief
            // 
            this.ColumnBrief.HeaderText = "简介";
            this.ColumnBrief.Name = "ColumnBrief";
            this.ColumnBrief.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "全局ID";
            // 
            // tbID
            // 
            this.tbID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbID.Location = new System.Drawing.Point(52, 14);
            this.tbID.Name = "tbID";
            this.tbID.Size = new System.Drawing.Size(316, 21);
            this.tbID.TabIndex = 6;
            // 
            // gbParams
            // 
            this.gbParams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbParams.Controls.Add(this.panelParams);
            this.gbParams.Controls.Add(this.tbID);
            this.gbParams.Controls.Add(this.label1);
            this.gbParams.Location = new System.Drawing.Point(349, 1);
            this.gbParams.Name = "gbParams";
            this.gbParams.Size = new System.Drawing.Size(373, 377);
            this.gbParams.TabIndex = 7;
            this.gbParams.TabStop = false;
            this.gbParams.Text = "Initor参数";
            // 
            // panelParams
            // 
            this.panelParams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelParams.AutoScroll = true;
            this.panelParams.ColumnCount = 1;
            this.panelParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelParams.Location = new System.Drawing.Point(0, 41);
            this.panelParams.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.panelParams.Name = "panelParams";
            this.panelParams.RowCount = 1;
            this.panelParams.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelParams.Size = new System.Drawing.Size(373, 336);
            this.panelParams.TabIndex = 7;
            // 
            // FormCreateInitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 417);
            this.Controls.Add(this.gbParams);
            this.Controls.Add(this.dgvTypes);
            this.Controls.Add(this.lbTips);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Name = "FormCreateInitor";
            this.Text = "创建Initor对象";
            this.Load += new System.EventHandler(this.FormSelectInitor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTypes)).EndInit();
            this.gbParams.ResumeLayout(false);
            this.gbParams.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label lbTips;
        private System.Windows.Forms.DataGridView dgvTypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbID;
        private System.Windows.Forms.GroupBox gbParams;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBrief;
        private System.Windows.Forms.TableLayoutPanel panelParams;
    }
}