namespace DLAF
{
    partial class Form_BoxManage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resousrces should be disposed; otherwise, false.</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BoxManage));
            this.listBox_Box = new System.Windows.Forms.ListBox();
            this.propertyGrid_Box = new System.Windows.Forms.PropertyGrid();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btn_AddCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox_Box
            // 
            this.listBox_Box.FormattingEnabled = true;
            this.listBox_Box.ItemHeight = 12;
            this.listBox_Box.Location = new System.Drawing.Point(12, 12);
            this.listBox_Box.Name = "listBox_Box";
            this.listBox_Box.Size = new System.Drawing.Size(209, 280);
            this.listBox_Box.TabIndex = 0;
            this.listBox_Box.SelectedIndexChanged += new System.EventHandler(this.listBox_Box_SelectedIndexChanged);
            // 
            // propertyGrid_Box
            // 
            this.propertyGrid_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid_Box.Location = new System.Drawing.Point(227, 12);
            this.propertyGrid_Box.Name = "propertyGrid_Box";
            this.propertyGrid_Box.Size = new System.Drawing.Size(339, 282);
            this.propertyGrid_Box.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(515, 333);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 40);
            this.btnSave.TabIndex = 14;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(33, 299);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(61, 25);
            this.btnAdd.TabIndex = 85;
            this.btnAdd.Text = "增加";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDel
            // 
            this.btnDel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnDel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btnDel.FlatAppearance.BorderSize = 0;
            this.btnDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDel.ForeColor = System.Drawing.Color.White;
            this.btnDel.Location = new System.Drawing.Point(136, 299);
            this.btnDel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(61, 25);
            this.btnDel.TabIndex = 84;
            this.btnDel.Text = "删除";
            this.btnDel.UseVisualStyleBackColor = false;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btn_AddCopy
            // 
            this.btn_AddCopy.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btn_AddCopy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_AddCopy.FlatAppearance.BorderSize = 0;
            this.btn_AddCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_AddCopy.ForeColor = System.Drawing.Color.White;
            this.btn_AddCopy.Location = new System.Drawing.Point(227, 299);
            this.btn_AddCopy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_AddCopy.Name = "btn_AddCopy";
            this.btn_AddCopy.Size = new System.Drawing.Size(125, 25);
            this.btn_AddCopy.TabIndex = 86;
            this.btn_AddCopy.Text = "新增当前料盒的拷贝料盒";
            this.btn_AddCopy.UseVisualStyleBackColor = false;
            this.btn_AddCopy.Click += new System.EventHandler(this.btn_AddCopy_Click);
            // 
            // Form_BoxManage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 385);
            this.Controls.Add(this.btn_AddCopy);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.propertyGrid_Box);
            this.Controls.Add(this.listBox_Box);
            this.Name = "Form_BoxManage";
            this.Text = "料盒管理";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_Box;
        private System.Windows.Forms.PropertyGrid propertyGrid_Box;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btn_AddCopy;
    }
}