namespace JFLog
{
    partial class UCLogger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCLogger));
            this.tcLog = new System.Windows.Forms.TabControl();
            this.tcLogShow = new System.Windows.Forms.TabPage();
            this.dgvLogShow = new System.Windows.Forms.DataGridView();
            this.tcLogSelect = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bdnPageManager = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorMoveLastPage = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstPage = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousPage = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorStartPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextPage = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelPreviousPage = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBoxCurrentPage = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripTextBoxTotalPage = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelNextPage = new System.Windows.Forms.ToolStripLabel();
            this.dgvSelect = new System.Windows.Forms.DataGridView();
            this.btSelect = new System.Windows.Forms.Button();
            this.btOut = new System.Windows.Forms.Button();
            this.cbMaxLevel = new System.Windows.Forms.ComboBox();
            this.cbMinLevel = new System.Windows.Forms.ComboBox();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.bdsInfo = new System.Windows.Forms.BindingSource(this.components);
            this.tcLog.SuspendLayout();
            this.tcLogShow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogShow)).BeginInit();
            this.tcLogSelect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bdnPageManager)).BeginInit();
            this.bdnPageManager.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // tcLog
            // 
            this.tcLog.Controls.Add(this.tcLogShow);
            this.tcLog.Controls.Add(this.tcLogSelect);
            this.tcLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcLog.Location = new System.Drawing.Point(0, 0);
            this.tcLog.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tcLog.Multiline = true;
            this.tcLog.Name = "tcLog";
            this.tcLog.SelectedIndex = 0;
            this.tcLog.Size = new System.Drawing.Size(559, 429);
            this.tcLog.TabIndex = 0;
            // 
            // tcLogShow
            // 
            this.tcLogShow.Controls.Add(this.dgvLogShow);
            this.tcLogShow.Location = new System.Drawing.Point(4, 22);
            this.tcLogShow.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tcLogShow.Name = "tcLogShow";
            this.tcLogShow.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tcLogShow.Size = new System.Drawing.Size(551, 403);
            this.tcLogShow.TabIndex = 0;
            this.tcLogShow.Text = "日志显示";
            this.tcLogShow.UseVisualStyleBackColor = true;
            // 
            // dgvLogShow
            // 
            this.dgvLogShow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLogShow.Location = new System.Drawing.Point(5, 9);
            this.dgvLogShow.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvLogShow.Name = "dgvLogShow";
            this.dgvLogShow.RowHeadersWidth = 51;
            this.dgvLogShow.RowTemplate.Height = 27;
            this.dgvLogShow.Size = new System.Drawing.Size(542, 388);
            this.dgvLogShow.TabIndex = 0;
            // 
            // tcLogSelect
            // 
            this.tcLogSelect.Controls.Add(this.label4);
            this.tcLogSelect.Controls.Add(this.label3);
            this.tcLogSelect.Controls.Add(this.label2);
            this.tcLogSelect.Controls.Add(this.label1);
            this.tcLogSelect.Controls.Add(this.bdnPageManager);
            this.tcLogSelect.Controls.Add(this.dgvSelect);
            this.tcLogSelect.Controls.Add(this.btSelect);
            this.tcLogSelect.Controls.Add(this.btOut);
            this.tcLogSelect.Controls.Add(this.cbMaxLevel);
            this.tcLogSelect.Controls.Add(this.cbMinLevel);
            this.tcLogSelect.Controls.Add(this.dtpEnd);
            this.tcLogSelect.Controls.Add(this.dtpStart);
            this.tcLogSelect.Location = new System.Drawing.Point(4, 22);
            this.tcLogSelect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tcLogSelect.Name = "tcLogSelect";
            this.tcLogSelect.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tcLogSelect.Size = new System.Drawing.Size(551, 403);
            this.tcLogSelect.TabIndex = 1;
            this.tcLogSelect.Text = "日志查询";
            this.tcLogSelect.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(339, 366);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "日志高等级：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 366);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "日志低等级：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 366);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "结束时间：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 366);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "起始时间：";
            // 
            // bdnPageManager
            // 
            this.bdnPageManager.AddNewItem = null;
            this.bdnPageManager.CountItem = this.bindingNavigatorMoveLastPage;
            this.bdnPageManager.DeleteItem = null;
            this.bdnPageManager.Dock = System.Windows.Forms.DockStyle.None;
            this.bdnPageManager.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.bdnPageManager.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstPage,
            this.bindingNavigatorMovePreviousPage,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorStartPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextPage,
            this.bindingNavigatorMoveLastPage,
            this.bindingNavigatorSeparator2,
            this.toolStripLabelPreviousPage,
            this.toolStripSeparator1,
            this.toolStripTextBoxCurrentPage,
            this.toolStripTextBoxTotalPage,
            this.toolStripSeparator2,
            this.toolStripLabelNextPage});
            this.bdnPageManager.Location = new System.Drawing.Point(2, 338);
            this.bdnPageManager.MoveFirstItem = this.bindingNavigatorMoveFirstPage;
            this.bdnPageManager.MoveLastItem = this.bindingNavigatorMoveLastPage;
            this.bdnPageManager.MoveNextItem = this.bindingNavigatorMoveNextPage;
            this.bdnPageManager.MovePreviousItem = this.bindingNavigatorMovePreviousPage;
            this.bdnPageManager.Name = "bdnPageManager";
            this.bdnPageManager.PositionItem = this.bindingNavigatorStartPositionItem;
            this.bdnPageManager.Size = new System.Drawing.Size(437, 27);
            this.bdnPageManager.TabIndex = 4;
            this.bdnPageManager.Text = "bindingNavigator1";
            this.bdnPageManager.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.BdnPageManager_ItemClicked);
            // 
            // bindingNavigatorMoveLastPage
            // 
            this.bindingNavigatorMoveLastPage.Checked = true;
            this.bindingNavigatorMoveLastPage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bindingNavigatorMoveLastPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastPage.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastPage.Image")));
            this.bindingNavigatorMoveLastPage.Name = "bindingNavigatorMoveLastPage";
            this.bindingNavigatorMoveLastPage.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastPage.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMoveLastPage.Text = "/ {0}";
            // 
            // bindingNavigatorMoveFirstPage
            // 
            this.bindingNavigatorMoveFirstPage.Checked = true;
            this.bindingNavigatorMoveFirstPage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bindingNavigatorMoveFirstPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstPage.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstPage.Image")));
            this.bindingNavigatorMoveFirstPage.Name = "bindingNavigatorMoveFirstPage";
            this.bindingNavigatorMoveFirstPage.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstPage.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMoveFirstPage.Text = "/ {0}";
            // 
            // bindingNavigatorMovePreviousPage
            // 
            this.bindingNavigatorMovePreviousPage.Checked = true;
            this.bindingNavigatorMovePreviousPage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bindingNavigatorMovePreviousPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousPage.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousPage.Image")));
            this.bindingNavigatorMovePreviousPage.Name = "bindingNavigatorMovePreviousPage";
            this.bindingNavigatorMovePreviousPage.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousPage.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMovePreviousPage.Text = "移到上一条记录";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorStartPositionItem
            // 
            this.bindingNavigatorStartPositionItem.AccessibleName = "位置";
            this.bindingNavigatorStartPositionItem.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.bindingNavigatorStartPositionItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.bindingNavigatorStartPositionItem.Name = "bindingNavigatorStartPositionItem";
            this.bindingNavigatorStartPositionItem.Size = new System.Drawing.Size(38, 27);
            this.bindingNavigatorStartPositionItem.Text = "0";
            this.bindingNavigatorStartPositionItem.ToolTipText = "当前位置";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.BackColor = System.Drawing.SystemColors.Control;
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(20, 24);
            this.bindingNavigatorCountItem.Text = "/0";
            this.bindingNavigatorCountItem.ToolTipText = "总项数";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorMoveNextPage
            // 
            this.bindingNavigatorMoveNextPage.Checked = true;
            this.bindingNavigatorMoveNextPage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bindingNavigatorMoveNextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextPage.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextPage.Image")));
            this.bindingNavigatorMoveNextPage.Name = "bindingNavigatorMoveNextPage";
            this.bindingNavigatorMoveNextPage.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextPage.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMoveNextPage.Text = "移到下一条记录";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripLabelPreviousPage
            // 
            this.toolStripLabelPreviousPage.AutoSize = false;
            this.toolStripLabelPreviousPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripLabelPreviousPage.ForeColor = System.Drawing.Color.Blue;
            this.toolStripLabelPreviousPage.Name = "toolStripLabelPreviousPage";
            this.toolStripLabelPreviousPage.Size = new System.Drawing.Size(84, 24);
            this.toolStripLabelPreviousPage.Text = "Previous Page";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripTextBoxCurrentPage
            // 
            this.toolStripTextBoxCurrentPage.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.toolStripTextBoxCurrentPage.Enabled = false;
            this.toolStripTextBoxCurrentPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.toolStripTextBoxCurrentPage.Name = "toolStripTextBoxCurrentPage";
            this.toolStripTextBoxCurrentPage.Size = new System.Drawing.Size(38, 27);
            // 
            // toolStripTextBoxTotalPage
            // 
            this.toolStripTextBoxTotalPage.Name = "toolStripTextBoxTotalPage";
            this.toolStripTextBoxTotalPage.Size = new System.Drawing.Size(20, 24);
            this.toolStripTextBoxTotalPage.Text = "/0";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripLabelNextPage
            // 
            this.toolStripLabelNextPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripLabelNextPage.ForeColor = System.Drawing.Color.Blue;
            this.toolStripLabelNextPage.Name = "toolStripLabelNextPage";
            this.toolStripLabelNextPage.Size = new System.Drawing.Size(64, 24);
            this.toolStripLabelNextPage.Text = "Next Page";
            // 
            // dgvSelect
            // 
            this.dgvSelect.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvSelect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSelect.Location = new System.Drawing.Point(5, 4);
            this.dgvSelect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvSelect.Name = "dgvSelect";
            this.dgvSelect.RowHeadersWidth = 51;
            this.dgvSelect.RowTemplate.Height = 27;
            this.dgvSelect.Size = new System.Drawing.Size(542, 326);
            this.dgvSelect.TabIndex = 3;
            // 
            // btSelect
            // 
            this.btSelect.Location = new System.Drawing.Point(484, 378);
            this.btSelect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btSelect.Name = "btSelect";
            this.btSelect.Size = new System.Drawing.Size(62, 22);
            this.btSelect.TabIndex = 2;
            this.btSelect.Text = "查询";
            this.btSelect.UseVisualStyleBackColor = true;
            this.btSelect.Click += new System.EventHandler(this.BtSelect_Click);
            // 
            // btOut
            // 
            this.btOut.Location = new System.Drawing.Point(484, 343);
            this.btOut.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btOut.Name = "btOut";
            this.btOut.Size = new System.Drawing.Size(62, 22);
            this.btOut.TabIndex = 2;
            this.btOut.Text = "导出";
            this.btOut.UseVisualStyleBackColor = true;
            this.btOut.Click += new System.EventHandler(this.BtOut_Click);
            // 
            // cbMaxLevel
            // 
            this.cbMaxLevel.FormattingEnabled = true;
            this.cbMaxLevel.Location = new System.Drawing.Point(341, 382);
            this.cbMaxLevel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbMaxLevel.Name = "cbMaxLevel";
            this.cbMaxLevel.Size = new System.Drawing.Size(92, 20);
            this.cbMaxLevel.TabIndex = 1;
            // 
            // cbMinLevel
            // 
            this.cbMinLevel.FormattingEnabled = true;
            this.cbMinLevel.Location = new System.Drawing.Point(233, 382);
            this.cbMinLevel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbMinLevel.Name = "cbMinLevel";
            this.cbMinLevel.Size = new System.Drawing.Size(92, 20);
            this.cbMinLevel.TabIndex = 1;
            // 
            // dtpEnd
            // 
            this.dtpEnd.Location = new System.Drawing.Point(117, 382);
            this.dtpEnd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(103, 21);
            this.dtpEnd.TabIndex = 0;
            // 
            // dtpStart
            // 
            this.dtpStart.Location = new System.Drawing.Point(5, 382);
            this.dtpStart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(103, 21);
            this.dtpStart.TabIndex = 0;
            this.dtpStart.Value = new System.DateTime(2020, 1, 12, 15, 47, 58, 0);
            // 
            // UCLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcLog);
            this.Name = "UCLogger";
            this.Size = new System.Drawing.Size(559, 429);
            this.Load += new System.EventHandler(this.UCLogger_Load);
            this.tcLog.ResumeLayout(false);
            this.tcLogShow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogShow)).EndInit();
            this.tcLogSelect.ResumeLayout(false);
            this.tcLogSelect.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bdnPageManager)).EndInit();
            this.bdnPageManager.ResumeLayout(false);
            this.bdnPageManager.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcLog;
        private System.Windows.Forms.TabPage tcLogShow;
        private System.Windows.Forms.DataGridView dgvLogShow;
        private System.Windows.Forms.TabPage tcLogSelect;
        private System.Windows.Forms.Button btOut;
        private System.Windows.Forms.ComboBox cbMinLevel;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstPage;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousPage;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorStartPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextPage;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastPage;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.DataGridView dgvSelect;
        private System.Windows.Forms.ToolStripLabel toolStripLabelPreviousPage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxCurrentPage;
        private System.Windows.Forms.ToolStripLabel toolStripTextBoxTotalPage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabelNextPage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btSelect;
        private System.Windows.Forms.ComboBox cbMaxLevel;
        private System.Windows.Forms.BindingNavigator bdnPageManager;
        private System.Windows.Forms.BindingSource bdsInfo;
    }
}
