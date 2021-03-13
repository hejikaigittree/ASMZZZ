namespace JFUI
{
    partial class UcJFParamEdit
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
            this.pGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // pGrid
            // 
            this.pGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pGrid.Location = new System.Drawing.Point(0, 0);
            this.pGrid.Name = "pGrid";
            this.pGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pGrid.Size = new System.Drawing.Size(350, 49);
            this.pGrid.TabIndex = 0;
            this.pGrid.ToolbarVisible = false;
            this.pGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pGrid_PropertyValueChanged);
            // 
            // UcJFParamEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pGrid);
            this.Name = "UcJFParamEdit";
            this.Size = new System.Drawing.Size(350, 49);
            this.Load += new System.EventHandler(this.UcJFInitorParam_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UcJFParamEdit_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pGrid;
    }
}
