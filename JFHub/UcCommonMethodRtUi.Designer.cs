namespace JFHub
{
    partial class UcCommonMethodRtUi
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
            this.btAction = new System.Windows.Forms.Button();
            this.btSetSave = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnInputParam = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnOutputParam = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btAction
            // 
            this.btAction.Location = new System.Drawing.Point(174, 3);
            this.btAction.Name = "btAction";
            this.btAction.Size = new System.Drawing.Size(42, 23);
            this.btAction.TabIndex = 0;
            this.btAction.Text = "执行";
            this.btAction.UseVisualStyleBackColor = true;
            this.btAction.Click += new System.EventHandler(this.btAction_Click);
            // 
            // btSetSave
            // 
            this.btSetSave.Location = new System.Drawing.Point(5, 3);
            this.btSetSave.Name = "btSetSave";
            this.btSetSave.Size = new System.Drawing.Size(90, 23);
            this.btSetSave.TabIndex = 1;
            this.btSetSave.Text = "设置输入参数";
            this.btSetSave.UseVisualStyleBackColor = true;
            this.btSetSave.Click += new System.EventHandler(this.btSetSave_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(101, 3);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(67, 23);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "取消设置";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbInfo);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.pnInputParam);
            this.splitContainer1.Panel1.Controls.Add(this.btSetSave);
            this.splitContainer1.Panel1.Controls.Add(this.btAction);
            this.splitContainer1.Panel1.Controls.Add(this.btCancel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.pnOutputParam);
            this.splitContainer1.Size = new System.Drawing.Size(636, 322);
            this.splitContainer1.SplitterDistance = 222;
            this.splitContainer1.TabIndex = 3;
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = true;
            this.lbInfo.Location = new System.Drawing.Point(259, 9);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(41, 12);
            this.lbInfo.TabIndex = 7;
            this.lbInfo.Text = "未执行";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(222, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "信息:";
            // 
            // pnInputParam
            // 
            this.pnInputParam.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnInputParam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnInputParam.ColumnCount = 1;
            this.pnInputParam.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnInputParam.Location = new System.Drawing.Point(5, 29);
            this.pnInputParam.Name = "pnInputParam";
            this.pnInputParam.RowCount = 1;
            this.pnInputParam.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnInputParam.Size = new System.Drawing.Size(628, 190);
            this.pnInputParam.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "输出参数列表";
            // 
            // pnOutputParam
            // 
            this.pnOutputParam.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnOutputParam.ColumnCount = 1;
            this.pnOutputParam.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnOutputParam.Location = new System.Drawing.Point(5, 21);
            this.pnOutputParam.Name = "pnOutputParam";
            this.pnOutputParam.RowCount = 1;
            this.pnOutputParam.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnOutputParam.Size = new System.Drawing.Size(628, 72);
            this.pnOutputParam.TabIndex = 4;
            // 
            // UcCommonMethodRtUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UcCommonMethodRtUi";
            this.Size = new System.Drawing.Size(636, 322);
            this.Load += new System.EventHandler(this.UcCommonMethodRtUi_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btAction;
        private System.Windows.Forms.Button btSetSave;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lbInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel pnInputParam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel pnOutputParam;
    }
}
