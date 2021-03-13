namespace JFUI
{
    partial class FormXCfgEdit
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
            this.btEditSave = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.lbInfo = new System.Windows.Forms.Label();
            this.btAddItem = new System.Windows.Forms.Button();
            this.btDeletItem = new System.Windows.Forms.Button();
            this.btSaveAs = new System.Windows.Forms.Button();
            this.tabControlCF1 = new JFUI.TabControlCF();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControlCF1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btEditSave
            // 
            this.btEditSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btEditSave.Location = new System.Drawing.Point(4, 424);
            this.btEditSave.Name = "btEditSave";
            this.btEditSave.Size = new System.Drawing.Size(50, 23);
            this.btEditSave.TabIndex = 1;
            this.btEditSave.Text = "编辑";
            this.btEditSave.UseVisualStyleBackColor = true;
            this.btEditSave.Click += new System.EventHandler(this.btEditSave_Click);
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btCancel.Location = new System.Drawing.Point(56, 424);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(50, 23);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // lbInfo
            // 
            this.lbInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbInfo.AutoSize = true;
            this.lbInfo.Location = new System.Drawing.Point(314, 429);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(35, 12);
            this.lbInfo.TabIndex = 3;
            this.lbInfo.Text = "信息:";
            // 
            // btAddItem
            // 
            this.btAddItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btAddItem.Enabled = false;
            this.btAddItem.Location = new System.Drawing.Point(129, 424);
            this.btAddItem.Name = "btAddItem";
            this.btAddItem.Size = new System.Drawing.Size(50, 23);
            this.btAddItem.TabIndex = 4;
            this.btAddItem.Text = "添加";
            this.btAddItem.UseVisualStyleBackColor = true;
            this.btAddItem.Click += new System.EventHandler(this.btAddItem_Click);
            // 
            // btDeletItem
            // 
            this.btDeletItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btDeletItem.Enabled = false;
            this.btDeletItem.Location = new System.Drawing.Point(182, 424);
            this.btDeletItem.Name = "btDeletItem";
            this.btDeletItem.Size = new System.Drawing.Size(50, 23);
            this.btDeletItem.TabIndex = 5;
            this.btDeletItem.Text = "删除";
            this.btDeletItem.UseVisualStyleBackColor = true;
            this.btDeletItem.Click += new System.EventHandler(this.btDeletItem_Click);
            // 
            // btSaveAs
            // 
            this.btSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btSaveAs.Enabled = false;
            this.btSaveAs.Location = new System.Drawing.Point(261, 424);
            this.btSaveAs.Name = "btSaveAs";
            this.btSaveAs.Size = new System.Drawing.Size(50, 23);
            this.btSaveAs.TabIndex = 6;
            this.btSaveAs.Text = "另存为";
            this.btSaveAs.UseVisualStyleBackColor = true;
            this.btSaveAs.Click += new System.EventHandler(this.btSaveAs_Click);
            // 
            // tabControlCF1
            // 
            this.tabControlCF1.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabControlCF1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlCF1.Controls.Add(this.tabPage1);
            this.tabControlCF1.Controls.Add(this.tabPage2);
            this.tabControlCF1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlCF1.ItemSize = new System.Drawing.Size(25, 150);
            this.tabControlCF1.Location = new System.Drawing.Point(0, 3);
            this.tabControlCF1.Multiline = true;
            this.tabControlCF1.Name = "tabControlCF1";
            this.tabControlCF1.SelectedIndex = 0;
            this.tabControlCF1.Size = new System.Drawing.Size(798, 419);
            this.tabControlCF1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlCF1.TabColor = System.Drawing.SystemColors.ControlDark;
            this.tabControlCF1.TabIndex = 0;
            this.tabControlCF1.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControlCF1_Deselecting);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(640, 411);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(640, 411);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // FormXCfgEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btSaveAs);
            this.Controls.Add(this.btDeletItem);
            this.Controls.Add(this.btAddItem);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btEditSave);
            this.Controls.Add(this.tabControlCF1);
            this.Name = "FormXCfgEdit";
            this.Text = "XCfgEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormStationCfgParam_FormClosing);
            this.Load += new System.EventHandler(this.FormStationCfgParam_Load);
            this.VisibleChanged += new System.EventHandler(this.FormStationCfgParam_VisibleChanged);
            this.tabControlCF1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private JFUI.TabControlCF tabControlCF1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btEditSave;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label lbInfo;
        private System.Windows.Forms.Button btAddItem;
        private System.Windows.Forms.Button btDeletItem;
        private System.Windows.Forms.Button btSaveAs;
    }
}