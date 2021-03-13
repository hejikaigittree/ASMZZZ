using System;
using System.Windows.Forms;
namespace HTHalControl
{
    partial class HTWindowControl
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
            if (disposing && this.components != null)
            {
                this.components.Dispose();
                this.hv_MenuStrip.Dispose();
                mCtrl_HWindow.HMouseMove -= HWindowControl_HMouseMove;
                mCtrl_HWindow.HMouseWheel -= HWindowControl_HMouseWheel;
            }
            if (disposing && this.hv_image != null)
            {
                this.hv_image.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HTWindowControl));
            this.mCtrl_HWindow = new HalconDotNet.HWindowControl();
            this.m_CtrlHStatusLabelCtrl = new System.Windows.Forms.Label();
            this.tbPanel = new System.Windows.Forms.TableLayoutPanel();
            this.rbPoint = new System.Windows.Forms.RadioButton();
            this.rbLine = new System.Windows.Forms.RadioButton();
            this.rbRect1 = new System.Windows.Forms.RadioButton();
            this.rbRect2 = new System.Windows.Forms.RadioButton();
            this.rbCircle = new System.Windows.Forms.RadioButton();
            this.rbEllipse = new System.Windows.Forms.RadioButton();
            this.rbRegion = new System.Windows.Forms.RadioButton();
            this.rbXld = new System.Windows.Forms.RadioButton();
            this.rbMeasure = new System.Windows.Forms.RadioButton();
            this.rbRect1Ex = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolTip_Info = new System.Windows.Forms.ToolTip(this.components);
            this.tbPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mCtrl_HWindow
            // 
            this.mCtrl_HWindow.BackColor = System.Drawing.Color.Black;
            this.mCtrl_HWindow.BorderColor = System.Drawing.Color.Black;
            this.mCtrl_HWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mCtrl_HWindow.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.mCtrl_HWindow.Location = new System.Drawing.Point(29, 0);
            this.mCtrl_HWindow.Margin = new System.Windows.Forms.Padding(0);
            this.mCtrl_HWindow.Name = "mCtrl_HWindow";
            this.mCtrl_HWindow.Size = new System.Drawing.Size(393, 333);
            this.mCtrl_HWindow.TabIndex = 0;
            this.mCtrl_HWindow.WindowSize = new System.Drawing.Size(393, 333);
            // 
            // m_CtrlHStatusLabelCtrl
            // 
            this.m_CtrlHStatusLabelCtrl.AutoSize = true;
            this.m_CtrlHStatusLabelCtrl.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.m_CtrlHStatusLabelCtrl.ForeColor = System.Drawing.SystemColors.WindowText;
            this.m_CtrlHStatusLabelCtrl.Location = new System.Drawing.Point(32, 336);
            this.m_CtrlHStatusLabelCtrl.Margin = new System.Windows.Forms.Padding(3);
            this.m_CtrlHStatusLabelCtrl.Name = "m_CtrlHStatusLabelCtrl";
            this.m_CtrlHStatusLabelCtrl.Size = new System.Drawing.Size(112, 15);
            this.m_CtrlHStatusLabelCtrl.TabIndex = 1;
            this.m_CtrlHStatusLabelCtrl.Text = "NO INPUTIMAGE";
            // 
            // tbPanel
            // 
            this.tbPanel.AutoSize = true;
            this.tbPanel.ColumnCount = 1;
            this.tbPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbPanel.Controls.Add(this.rbPoint, 0, 0);
            this.tbPanel.Controls.Add(this.rbLine, 0, 1);
            this.tbPanel.Controls.Add(this.rbRect1, 0, 2);
            this.tbPanel.Controls.Add(this.rbRect2, 0, 3);
            this.tbPanel.Controls.Add(this.rbCircle, 0, 4);
            this.tbPanel.Controls.Add(this.rbEllipse, 0, 5);
            this.tbPanel.Controls.Add(this.rbRegion, 0, 6);
            this.tbPanel.Controls.Add(this.rbXld, 0, 7);
            this.tbPanel.Controls.Add(this.rbMeasure, 0, 8);
            this.tbPanel.Controls.Add(this.rbRect1Ex, 0, 9);
            this.tbPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbPanel.Location = new System.Drawing.Point(0, 0);
            this.tbPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tbPanel.Name = "tbPanel";
            this.tbPanel.RowCount = 11;
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tbPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tbPanel.Size = new System.Drawing.Size(29, 333);
            this.tbPanel.TabIndex = 2;
            // 
            // rbPoint
            // 
            this.rbPoint.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPoint.BackgroundImage = global::HTHalControl.Properties.Resources.point;
            this.rbPoint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rbPoint.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbPoint.Location = new System.Drawing.Point(2, 2);
            this.rbPoint.Margin = new System.Windows.Forms.Padding(2);
            this.rbPoint.Name = "rbPoint";
            this.rbPoint.Size = new System.Drawing.Size(26, 26);
            this.rbPoint.TabIndex = 4;
            this.rbPoint.TabStop = true;
            this.rbPoint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Info.SetToolTip(this.rbPoint, "画点");
            this.rbPoint.UseVisualStyleBackColor = true;
            this.rbPoint.Click += new System.EventHandler(this.rbPoint_Click);
            // 
            // rbLine
            // 
            this.rbLine.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbLine.BackgroundImage = global::HTHalControl.Properties.Resources.line;
            this.rbLine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rbLine.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbLine.Location = new System.Drawing.Point(2, 32);
            this.rbLine.Margin = new System.Windows.Forms.Padding(2);
            this.rbLine.Name = "rbLine";
            this.rbLine.Size = new System.Drawing.Size(26, 26);
            this.rbLine.TabIndex = 3;
            this.rbLine.TabStop = true;
            this.rbLine.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Info.SetToolTip(this.rbLine, "画直线");
            this.rbLine.UseVisualStyleBackColor = true;
            this.rbLine.Click += new System.EventHandler(this.rbLine_Click);
            // 
            // rbRect1
            // 
            this.rbRect1.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbRect1.BackgroundImage = global::HTHalControl.Properties.Resources.rect1;
            this.rbRect1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rbRect1.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbRect1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbRect1.Location = new System.Drawing.Point(2, 62);
            this.rbRect1.Margin = new System.Windows.Forms.Padding(2);
            this.rbRect1.Name = "rbRect1";
            this.rbRect1.Size = new System.Drawing.Size(26, 26);
            this.rbRect1.TabIndex = 3;
            this.rbRect1.TabStop = true;
            this.rbRect1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Info.SetToolTip(this.rbRect1, "画水平矩形");
            this.rbRect1.UseVisualStyleBackColor = true;
            this.rbRect1.Click += new System.EventHandler(this.rbRect1_Click);
            // 
            // rbRect2
            // 
            this.rbRect2.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbRect2.BackgroundImage = global::HTHalControl.Properties.Resources.rect2;
            this.rbRect2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rbRect2.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbRect2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbRect2.Location = new System.Drawing.Point(2, 92);
            this.rbRect2.Margin = new System.Windows.Forms.Padding(2);
            this.rbRect2.Name = "rbRect2";
            this.rbRect2.Size = new System.Drawing.Size(26, 26);
            this.rbRect2.TabIndex = 3;
            this.rbRect2.TabStop = true;
            this.rbRect2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Info.SetToolTip(this.rbRect2, "画任意矩形");
            this.rbRect2.UseVisualStyleBackColor = true;
            this.rbRect2.Click += new System.EventHandler(this.rbRect2_Click);
            // 
            // rbCircle
            // 
            this.rbCircle.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbCircle.BackgroundImage = global::HTHalControl.Properties.Resources.circle;
            this.rbCircle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rbCircle.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbCircle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbCircle.Location = new System.Drawing.Point(2, 122);
            this.rbCircle.Margin = new System.Windows.Forms.Padding(2);
            this.rbCircle.Name = "rbCircle";
            this.rbCircle.Size = new System.Drawing.Size(26, 26);
            this.rbCircle.TabIndex = 3;
            this.rbCircle.TabStop = true;
            this.rbCircle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Info.SetToolTip(this.rbCircle, "画圆");
            this.rbCircle.UseVisualStyleBackColor = true;
            this.rbCircle.Click += new System.EventHandler(this.rbCircle_Click);
            // 
            // rbEllipse
            // 
            this.rbEllipse.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbEllipse.BackgroundImage = global::HTHalControl.Properties.Resources.ellipse;
            this.rbEllipse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rbEllipse.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbEllipse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbEllipse.Location = new System.Drawing.Point(2, 152);
            this.rbEllipse.Margin = new System.Windows.Forms.Padding(2);
            this.rbEllipse.Name = "rbEllipse";
            this.rbEllipse.Size = new System.Drawing.Size(26, 26);
            this.rbEllipse.TabIndex = 3;
            this.rbEllipse.TabStop = true;
            this.rbEllipse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Info.SetToolTip(this.rbEllipse, "画椭圆");
            this.rbEllipse.UseVisualStyleBackColor = true;
            this.rbEllipse.Click += new System.EventHandler(this.rbEllipse_Click);
            // 
            // rbRegion
            // 
            this.rbRegion.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbRegion.BackgroundImage = global::HTHalControl.Properties.Resources.region;
            this.rbRegion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rbRegion.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbRegion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbRegion.Location = new System.Drawing.Point(2, 182);
            this.rbRegion.Margin = new System.Windows.Forms.Padding(2);
            this.rbRegion.Name = "rbRegion";
            this.rbRegion.Size = new System.Drawing.Size(26, 26);
            this.rbRegion.TabIndex = 3;
            this.rbRegion.TabStop = true;
            this.rbRegion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Info.SetToolTip(this.rbRegion, "画任意曲线");
            this.rbRegion.UseVisualStyleBackColor = true;
            this.rbRegion.Click += new System.EventHandler(this.rbRegion_Click);
            // 
            // rbXld
            // 
            this.rbXld.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbXld.BackgroundImage = global::HTHalControl.Properties.Resources.polygon;
            this.rbXld.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rbXld.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbXld.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbXld.Location = new System.Drawing.Point(2, 212);
            this.rbXld.Margin = new System.Windows.Forms.Padding(2);
            this.rbXld.Name = "rbXld";
            this.rbXld.Size = new System.Drawing.Size(26, 26);
            this.rbXld.TabIndex = 3;
            this.rbXld.TabStop = true;
            this.rbXld.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Info.SetToolTip(this.rbXld, "画封闭曲线");
            this.rbXld.UseVisualStyleBackColor = false;
            this.rbXld.Click += new System.EventHandler(this.rbXld_Click);
            // 
            // rbMeasure
            // 
            this.rbMeasure.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMeasure.BackgroundImage = global::HTHalControl.Properties.Resources.measure;
            this.rbMeasure.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rbMeasure.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMeasure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbMeasure.Location = new System.Drawing.Point(2, 242);
            this.rbMeasure.Margin = new System.Windows.Forms.Padding(2);
            this.rbMeasure.Name = "rbMeasure";
            this.rbMeasure.Size = new System.Drawing.Size(26, 26);
            this.rbMeasure.TabIndex = 6;
            this.rbMeasure.TabStop = true;
            this.rbMeasure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Info.SetToolTip(this.rbMeasure, "测量距离");
            this.rbMeasure.UseVisualStyleBackColor = false;
            this.rbMeasure.Click += new System.EventHandler(this.rbMeasure_Click);
            // 
            // rbRect1Ex
            // 
            this.rbRect1Ex.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbRect1Ex.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbRect1Ex.BackgroundImage")));
            this.rbRect1Ex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rbRect1Ex.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbRect1Ex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbRect1Ex.Location = new System.Drawing.Point(2, 272);
            this.rbRect1Ex.Margin = new System.Windows.Forms.Padding(2);
            this.rbRect1Ex.Name = "rbRect1Ex";
            this.rbRect1Ex.Size = new System.Drawing.Size(26, 26);
            this.rbRect1Ex.TabIndex = 7;
            this.rbRect1Ex.TabStop = true;
            this.rbRect1Ex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip_Info.SetToolTip(this.rbRect1Ex, "可编辑矩形框");
            this.rbRect1Ex.UseVisualStyleBackColor = false;
            this.rbRect1Ex.Click += new System.EventHandler(this.rbRect1Ex_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tbPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.mCtrl_HWindow, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_CtrlHStatusLabelCtrl, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(422, 354);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // HTWindowControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HTWindowControl";
            this.Size = new System.Drawing.Size(422, 354);
            this.tbPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private HalconDotNet.HWindowControl mCtrl_HWindow;
        private System.Windows.Forms.Label m_CtrlHStatusLabelCtrl;
      
        private System.Windows.Forms.TableLayoutPanel tbPanel;
        private System.Windows.Forms.RadioButton rbPoint;
        private System.Windows.Forms.RadioButton rbEllipse;
        private System.Windows.Forms.RadioButton rbCircle;
        private System.Windows.Forms.RadioButton rbRect2;
        private System.Windows.Forms.RadioButton rbRect1;
        private System.Windows.Forms.RadioButton rbXld;
        private System.Windows.Forms.RadioButton rbRegion;
        private System.Windows.Forms.RadioButton rbLine;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private RadioButton rbMeasure;
        private ToolTip toolTip_Info;
        private RadioButton rbRect1Ex;
    }
}
