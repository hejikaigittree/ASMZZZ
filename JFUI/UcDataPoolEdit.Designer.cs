namespace JFUI
{
    partial class UcDataPoolEdit
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
            this.components = new System.ComponentModel.Container();
            this.btAdjust = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelSingleItems = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelCollectionItems = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.btRead = new System.Windows.Forms.Button();
            this.btWrite = new System.Windows.Forms.Button();
            this.timerFlush = new System.Windows.Forms.Timer(this.components);
            this.lbInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btAdjust
            // 
            this.btAdjust.Location = new System.Drawing.Point(3, 3);
            this.btAdjust.Name = "btAdjust";
            this.btAdjust.Size = new System.Drawing.Size(75, 23);
            this.btAdjust.TabIndex = 0;
            this.btAdjust.Text = "更新数据项";
            this.btAdjust.UseVisualStyleBackColor = true;
            this.btAdjust.Click += new System.EventHandler(this.btAdjust_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Location = new System.Drawing.Point(2, 28);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelSingleItems);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelCollectionItems);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(379, 351);
            this.splitContainer1.SplitterDistance = 175;
            this.splitContainer1.TabIndex = 1;
            // 
            // panelSingleItems
            // 
            this.panelSingleItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSingleItems.AutoScroll = true;
            this.panelSingleItems.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelSingleItems.ColumnCount = 3;
            this.panelSingleItems.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelSingleItems.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.panelSingleItems.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.panelSingleItems.Location = new System.Drawing.Point(3, 20);
            this.panelSingleItems.Name = "panelSingleItems";
            this.panelSingleItems.RowCount = 1;
            this.panelSingleItems.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 152F));
            this.panelSingleItems.Size = new System.Drawing.Size(371, 150);
            this.panelSingleItems.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "单值数据项";
            // 
            // panelCollectionItems
            // 
            this.panelCollectionItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCollectionItems.AutoScroll = true;
            this.panelCollectionItems.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelCollectionItems.ColumnCount = 3;
            this.panelCollectionItems.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelCollectionItems.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.panelCollectionItems.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.panelCollectionItems.Location = new System.Drawing.Point(3, 18);
            this.panelCollectionItems.Name = "panelCollectionItems";
            this.panelCollectionItems.RowCount = 1;
            this.panelCollectionItems.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 151F));
            this.panelCollectionItems.Size = new System.Drawing.Size(371, 149);
            this.panelCollectionItems.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "集合数据项";
            // 
            // chkAutoUpdate
            // 
            this.chkAutoUpdate.AutoSize = true;
            this.chkAutoUpdate.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkAutoUpdate.Location = new System.Drawing.Point(218, 6);
            this.chkAutoUpdate.Name = "chkAutoUpdate";
            this.chkAutoUpdate.Size = new System.Drawing.Size(54, 18);
            this.chkAutoUpdate.TabIndex = 2;
            this.chkAutoUpdate.Text = "自动";
            this.chkAutoUpdate.UseVisualStyleBackColor = true;
            this.chkAutoUpdate.CheckedChanged += new System.EventHandler(this.chkAutoUpdate_CheckedChanged);
            // 
            // btRead
            // 
            this.btRead.Location = new System.Drawing.Point(271, 3);
            this.btRead.Name = "btRead";
            this.btRead.Size = new System.Drawing.Size(52, 23);
            this.btRead.TabIndex = 3;
            this.btRead.Text = "读取";
            this.btRead.UseVisualStyleBackColor = true;
            this.btRead.Click += new System.EventHandler(this.btRead_Click);
            // 
            // btWrite
            // 
            this.btWrite.Location = new System.Drawing.Point(326, 3);
            this.btWrite.Name = "btWrite";
            this.btWrite.Size = new System.Drawing.Size(52, 23);
            this.btWrite.TabIndex = 4;
            this.btWrite.Text = "写入";
            this.btWrite.UseVisualStyleBackColor = true;
            this.btWrite.Click += new System.EventHandler(this.btWrite_Click);
            // 
            // timerFlush
            // 
            this.timerFlush.Tick += new System.EventHandler(this.timerFlush_Tick);
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = true;
            this.lbInfo.Location = new System.Drawing.Point(85, 9);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(29, 12);
            this.lbInfo.TabIndex = 5;
            this.lbInfo.Text = "Tips";
            // 
            // UcDataPoolEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.btWrite);
            this.Controls.Add(this.btRead);
            this.Controls.Add(this.chkAutoUpdate);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btAdjust);
            this.Name = "UcDataPoolEdit";
            this.Size = new System.Drawing.Size(381, 379);
            this.Load += new System.EventHandler(this.UcDataPoolEdit_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btAdjust;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.Button btRead;
        private System.Windows.Forms.Button btWrite;
        private System.Windows.Forms.TableLayoutPanel panelSingleItems;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel panelCollectionItems;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timerFlush;
        private System.Windows.Forms.Label lbInfo;
    }
}
