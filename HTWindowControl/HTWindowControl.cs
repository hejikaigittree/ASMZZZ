using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HTHalControl
{
    public partial class HTWindowControl : UserControl
	{
		private HObject hv_image;

		private HObject hv_region;

		private HTuple hv_imageWidth;

		private HTuple hv_imageHeight;

		private double mposition_row;

		private double mposition_col;

		private int zoom_beginRow;

		private int zoom_beginCol;

		private int zoom_endRow;

		private int zoom_endCol;

		private int current_beginRow;

		private int current_beginCol;

		private int current_endRow;

		private int current_endCol;

		private string color = "red";

		private int colored = 0;

		private string regionType = "";

		private string str_imgSize;

		private HWindowControl bufferWindow;

		private ContextMenuStrip hv_MenuStrip;

		private ToolStripMenuItem fit_strip;

		private ToolStripMenuItem zoom_strip;

		private ToolStripMenuItem saveImg_strip;

		private ToolStripMenuItem barVisible_strip;

		private ToolStripMenuItem clearImg_strip;

		private ToolStripMenuItem panelVisible_strip;

        private ToolStripMenuItem changeBackColor_strip;

        private ToolStripMenuItem confirmReg_strip;

        private bool isRightClick = true;

        /// <summary>
        /// 测量模式
        /// </summary>
        private bool isMeasureMode = false;
        /// <summary>
        /// 测量起点坐标
        /// </summary>
        private PointF measureStartPoint = new PointF();
        /// <summary>
        /// 测量终点坐标
        /// </summary>
        private PointF measureEndPoint = new PointF();
        /// <summary>
        /// 测量起点是否点击确定
        /// </summary>
        private bool measureFristClickFlag = false;
        /// <summary>
        /// 每像素代表的实际物理距离  如果为-1，则未设置，不显示
        /// </summary>
        private double umPerPix = -1;

        private HTuple row;

		private HTuple column;

		private HTuple row1;

		private HTuple column1;

		private HTuple row2;

		private HTuple column2;

		private HTuple phi;

		private HTuple length1;

		private HTuple length2;

		private HTuple radius;

		private HTuple radius1;

		private HTuple radius2;

		private HObject region;

        


        /// <summary>
        /// 鼠标点击事件
        /// </summary>
        public event Action<PointF> MouseClickEvent;
        /// <summary>
        /// 画ROI区域完成事件
        /// </summary>
        public event Action<string> DrawRoiDownEvent;
        /// <summary>
        /// 用户按下 CTRL+A 事件
        /// </summary>
        public event Action SelectAllEvent;

        public HTuple Row
		{
			get
			{
				return this.row;
			}
			set
			{
				this.row = value;
			}
		}

		public HTuple Column
		{
			get
			{
				return this.column;
			}
			set
			{
				this.column = value;
			}
		}

		public HTuple Row1
		{
			get
			{
				return this.row1;
			}
			set
			{
				this.row1 = value;
			}
		}

		public HTuple Column1
		{
			get
			{
				return this.column1;
			}
			set
			{
				this.column1 = value;
			}
		}

		public HTuple Row2
		{
			get
			{
				return this.row2;
			}
			set
			{
				this.row2 = value;
			}
		}

		public HTuple Column2
		{
			get
			{
				return this.column2;
			}
			set
			{
				this.column2 = value;
			}
		}

		public HTuple Phi
		{
			get
			{
				return this.phi;
			}
			set
			{
				this.phi = value;
			}
		}

		public HTuple Length1
		{
			get
			{
				return this.length1;
			}
			set
			{
				this.length1 = value;
			}
		}

		public HTuple Length2
		{
			get
			{
				return this.length2;
			}
			set
			{
				this.length2 = value;
			}
		}

		public HTuple Radius
		{
			get
			{
				return this.radius;
			}
			set
			{
				this.radius = value;
			}
		}

		public HTuple Radius1
		{
			get
			{
				return this.radius1;
			}
			set
			{
				this.radius1 = value;
			}
		}

		public HTuple Radius2
		{
			get
			{
				return this.radius2;
			}
			set
			{
				this.radius2 = value;
			}
		}

		public new HObject Region
		{
			get
			{
				return this.region;
			}
			set
			{
				this.region = value;
			}
		}

		public string RegionType
		{
			get
			{
				return this.regionType;
			}
			set
			{
				this.regionType = value;
			}
		}

		public string ColorName
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				if (Created)
				{
					Invoke(new Action(() => {InitHWindow(HTWindow); }));
				}
			}
		}

		public int ColorType
		{
			get
			{
				return this.colored;
			}
			set
			{
				this.colored = value;
			}
		}

		public HWindowControl HTWindow
		{
			get
			{
				return this.mCtrl_HWindow;
			}
			set
			{
				this.mCtrl_HWindow = value;
			}
		}

		public HObject Image
		{
			get
			{
				return this.hv_image;
			}
			set
			{
				this.hv_image = value;
			}
		}
        /// <summary>
        /// 单像素实际物理距离
        /// </summary>
        public double UmPerPix
        {
            get
            {
                return this.umPerPix;
            }
            set
            {
                this.umPerPix = value;
            }
        }

		public HTWindowControl()
		{
			this.InitializeComponent();
			this.fit_strip = new ToolStripMenuItem("适应窗口");
			this.fit_strip.Click += delegate(object s, EventArgs e)
			{
				this.RefreshWindow(this.hv_image, this.hv_region, "fit");
			};
			this.zoom_strip = new ToolStripMenuItem("等比例适应窗口");
			this.zoom_strip.Click += delegate(object s, EventArgs e)
			{
				this.RefreshWindow(this.hv_image, this.hv_region, "zoom");
			};
			this.panelVisible_strip = new ToolStripMenuItem("显示ROI交互面板");
			this.panelVisible_strip.CheckOnClick = true;
			this.panelVisible_strip.Checked = true;
			this.panelVisible_strip.CheckedChanged += new EventHandler(this.panelVisible_strip_CheckedChanged);
			this.barVisible_strip = new ToolStripMenuItem("显示状态栏");
			this.barVisible_strip.CheckOnClick = true;
			this.barVisible_strip.Checked = true;
			this.barVisible_strip.CheckedChanged += new EventHandler(this.barVisible_strip_CheckedChanged);
			this.saveImg_strip = new ToolStripMenuItem("保存结果图像");
			this.saveImg_strip.Click += delegate(object s, EventArgs e)
			{
				this.SaveWindowDumpDialog();
			};
			this.clearImg_strip = new ToolStripMenuItem("清除窗口");
			this.clearImg_strip.Click += delegate(object s, EventArgs e)
			{
				this.ClearHWindow();
			};

            this.changeBackColor_strip = new ToolStripMenuItem("窗口背景色");
            this.changeBackColor_strip.Click += ChangeBackColor_strip_Click;

            this.confirmReg_strip = new ToolStripMenuItem("确认区域");
            confirmReg_strip.Enabled = false;
            this.confirmReg_strip.Click += confirmReg_strip_Click;


            this.hv_MenuStrip = new ContextMenuStrip();
			this.hv_MenuStrip.Items.Add(this.fit_strip);
			this.hv_MenuStrip.Items.Add(this.zoom_strip);
			this.hv_MenuStrip.Items.Add(this.panelVisible_strip);
			this.hv_MenuStrip.Items.Add(this.barVisible_strip);
			this.hv_MenuStrip.Items.Add(new ToolStripSeparator());
			this.hv_MenuStrip.Items.Add(this.saveImg_strip);
			this.hv_MenuStrip.Items.Add(this.clearImg_strip);
            this.hv_MenuStrip.Items.Add(this.changeBackColor_strip);
            this.hv_MenuStrip.Items.Add(this.confirmReg_strip);
            this.SetStripEnable(false);
			this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
			this.mCtrl_HWindow.SizeChanged += delegate(object s, EventArgs e)
			{
				this.RefreshWindow(this.hv_image, this.hv_region, "zoom");
			};
			this.hv_MenuStrip.MouseEnter += delegate(object s, EventArgs e)
			{
				this.mCtrl_HWindow.HMouseWheel -= new HMouseEventHandler(this.HWindowControl_HMouseWheel);
			};
			this.hv_MenuStrip.MouseLeave += delegate(object s, EventArgs e)
			{
				this.mCtrl_HWindow.HMouseWheel += new HMouseEventHandler(this.HWindowControl_HMouseWheel);
			};
			this.mCtrl_HWindow.HMouseMove += new HMouseEventHandler(this.HWindowControl_HMouseMove);
			this.mCtrl_HWindow.HMouseWheel += new HMouseEventHandler(this.HWindowControl_HMouseWheel);
            this.mCtrl_HWindow.HMouseUp += new HMouseEventHandler(this.HWindowControl_HMouseUp);
            this.mCtrl_HWindow.KeyDown += MCtrl_HWindow_KeyDown;
			this.bufferWindow = new HWindowControl();
		}

        private void MCtrl_HWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                if (SelectAllEvent != null)
                {
                    SelectAllEvent();
                }
            }
        }

        private void ChangeBackColor_strip_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.AllowFullOpen = false;
           
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    HOperatorSet.SetWindowParam(this.mCtrl_HWindow.HalconWindow, "background_color",dlg.Color.Name.ToLower());
                    this.mCtrl_HWindow.HalconWindow.ClearWindow();
                    this.RefreshWindow(this.hv_image, this.hv_region, "zoom");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("无效的颜色名称，请重新选择！");
                }
                
            }
        }

        private void RefreshWindow(HWindowControl hw_Ctrl)
		{
			if (this.hv_image != null)
			{
				if (this.hv_image.IsInitialized())
				{
					this.DispImageFit(hw_Ctrl);
					this.SetStripEnable(true);
				}
			}
			if (this.hv_region != null)
			{
				if (this.hv_region.IsInitialized())
				{
					this.InitHWindow(hw_Ctrl);
					hw_Ctrl.HalconWindow.DispObj(this.hv_region);
				}
			}
		}

		public void RefreshWindow(HObject image, HObject region, string dispMode)
		{
			if (image != null)
			{
				if (image.IsInitialized())
				{
					this.hv_image = image;
					HOperatorSet.GetImageSize(this.hv_image, out this.hv_imageWidth, out this.hv_imageHeight);
					this.str_imgSize = string.Format("{0}X{1}", this.hv_imageWidth, this.hv_imageHeight);
					if (dispMode == "fit")
					{
						this.DispImageFit(this.mCtrl_HWindow);
					}
					else if (dispMode == "zoom")
					{
						this.DispImageZoom(this.mCtrl_HWindow);
					}
					else
					{
						this.DispImage(this.mCtrl_HWindow);
					}
					this.SetStripEnable(true);
					this.hv_region = region;
				}
			}
			if (region != null)
			{
				if (region.IsInitialized())
				{
					this.hv_region = region;
					this.InitHWindow(this.mCtrl_HWindow);
					this.mCtrl_HWindow.HalconWindow.DispObj(this.hv_region);
				}
			}
		}

		public void RefreshWindow(HObject image, HObject region, System.Drawing.Rectangle Rect)
		{
			if (image != null)
			{
				if (image.IsInitialized())
				{
					this.hv_image = image;
					HOperatorSet.GetImageSize(this.hv_image, out this.hv_imageWidth, out this.hv_imageHeight);
					this.str_imgSize = string.Format("{0}X{1}", this.hv_imageWidth, this.hv_imageHeight);
					this.DispImage(image, region, Rect);
					this.SetStripEnable(true);
					this.hv_region = region;
				}
			}
			if (region != null)
			{
				if (region.IsInitialized())
				{
					this.hv_region = region;
					this.InitHWindow(this.mCtrl_HWindow);
					this.mCtrl_HWindow.HalconWindow.DispObj(this.hv_region);
				}
			}
		}

		public void DisposeAll()
		{
			this.hv_MenuStrip.Dispose();
			this.mCtrl_HWindow.HMouseMove -= new HMouseEventHandler(this.HWindowControl_HMouseMove);
			this.mCtrl_HWindow.HMouseWheel -= new HMouseEventHandler(this.HWindowControl_HMouseWheel);
			if (this.hv_image != null)
			{
				this.hv_image.Dispose();
			}
			if (this.hv_region != null)
			{
				this.hv_region.Dispose();
			}
		}

		public void SetStripEnable(bool flag)
		{
			this.saveImg_strip.Enabled = flag;
		}

		public void SetTablePanelVisible(bool flag)
		{
			this.tbPanel.Visible = flag;
			base.SuspendLayout();
			if (flag)
			{
				this.tableLayoutPanel1.ColumnStyles[0].Width = 23f;
			}
			else
			{
				this.tableLayoutPanel1.ColumnStyles[0].Width = 0f;
			}
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public void SetStatusStripVisible(bool flag)
		{
			this.m_CtrlHStatusLabelCtrl.Visible = flag;
			base.SuspendLayout();
			if (flag)
			{
				this.tableLayoutPanel1.RowStyles[1].Height = 21f;
				this.mCtrl_HWindow.HMouseMove += new HMouseEventHandler(this.HWindowControl_HMouseMove);
			}
			else
			{
				this.tableLayoutPanel1.RowStyles[1].Height = 0f;
				this.mCtrl_HWindow.HMouseMove -= new HMouseEventHandler(this.HWindowControl_HMouseMove);
			}
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public void SetInteractive(bool flag)
		{
			if (flag)
			{
				this.mCtrl_HWindow.HMouseMove += new HMouseEventHandler(this.HWindowControl_HMouseMove);
				this.mCtrl_HWindow.HMouseWheel += new HMouseEventHandler(this.HWindowControl_HMouseWheel);
			}
			else
			{
				this.mCtrl_HWindow.HMouseMove -= new HMouseEventHandler(this.HWindowControl_HMouseMove);
				this.mCtrl_HWindow.HMouseWheel -= new HMouseEventHandler(this.HWindowControl_HMouseWheel);
			}
		}

		public void SetMenuStrip(bool flag)
		{
			this.mCtrl_HWindow.ContextMenuStrip = (flag ? this.hv_MenuStrip : null);
		}

		public void SaveWindowDumpDialog()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "TIFF图像|*.tiff|PNG图像|*.png|BMP图像|*.bmp|JPEG图像|*.jpg|所有文件|*.*";
			string[] array = new string[]
			{
				"tiff",
				"png",
				"bmp",
				"jpeg"
			};
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(saveFileDialog.FileName))
				{
					string fileName = saveFileDialog.FileName;
					string imageType = (saveFileDialog.FilterIndex < 5) ? array[saveFileDialog.FilterIndex - 1] : array[0];
					this.SaveWindowDump(imageType, fileName, new System.Drawing.Size(this.hv_imageWidth, this.hv_imageHeight));
				}
			}
		}

		private void SaveWindowDump(string imageType, string fileName, System.Drawing.Size size)
		{
			try
			{
				if (this.hv_image != null)
				{
					if (this.hv_image.IsInitialized())
					{
						HWindowControl hWindowControl = new HWindowControl();
						hWindowControl.WindowSize = size;
						this.InitHWindow(hWindowControl);
						this.RefreshWindow(hWindowControl);
						hWindowControl.HalconWindow.DumpWindow(imageType, fileName);
						hWindowControl.Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public bool SaveWindowDump(HObject image, HObject region, string imageType, string fileName)
		{
			bool result;
			try
			{
				if (image != null)
				{
					if (image.IsInitialized())
					{
						HWindowControl hWindowControl = new HWindowControl();
						HTuple t;
						HTuple t2;
						HOperatorSet.GetImageSize(image, out t, out t2);
						hWindowControl.WindowSize = new System.Drawing.Size(t, t2);
						this.InitHWindow(hWindowControl);
						this.DispImageFit(image, hWindowControl);
						if (region != null || region.IsInitialized())
						{
							hWindowControl.HalconWindow.DispObj(region);
						}
						hWindowControl.HalconWindow.DumpWindow(imageType, fileName);
						hWindowControl.Dispose();
					}
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public bool SaveWindowDump(HObject image, HObject region, string imageType, string fileName, System.Drawing.Size size)
		{
			bool result;
			try
			{
				if (image != null)
				{
					if (image.IsInitialized())
					{
						HWindowControl hWindowControl = new HWindowControl();
						hWindowControl.WindowSize = size;
						this.InitHWindow(hWindowControl);
						this.DispImageFit(image, hWindowControl);
						if (region != null || region.IsInitialized())
						{
							hWindowControl.HalconWindow.DispObj(region);
						}
						hWindowControl.HalconWindow.DumpWindow(imageType, fileName);
						hWindowControl.Dispose();
					}
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public bool WindowDump(ref HObject image, HObject region)
		{
			bool result;
			try
			{
				if (image != null)
				{
					if (image.IsInitialized())
					{
						HWindowControl hWindowControl = new HWindowControl();
						HTuple t;
						HTuple t2;
						HOperatorSet.GetImageSize(image, out t, out t2);
						hWindowControl.WindowSize = new System.Drawing.Size(t, t2);
						this.InitHWindow(hWindowControl);
						this.DispImageFit(image.CopyObj(1, -1), hWindowControl);
						if (region != null || region.IsInitialized())
						{
							hWindowControl.HalconWindow.DispObj(region);
						}
						image.Dispose();
						HOperatorSet.DumpWindowImage(out image, hWindowControl.HalconWindow);
						hWindowControl.Dispose();
					}
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public bool WindowDump(ref HObject image, HObject region, System.Drawing.Size size)
		{
			bool result;
			try
			{
				if (image != null)
				{
					if (image.IsInitialized())
					{
						HWindowControl hWindowControl = new HWindowControl();
						hWindowControl.WindowSize = size;
						this.InitHWindow(hWindowControl);
						this.DispImageFit(image.CopyObj(1, -1), hWindowControl);
						if (region != null || region.IsInitialized())
						{
							hWindowControl.HalconWindow.DispObj(region);
						}
						image.Dispose();
						HOperatorSet.DumpWindowImage(out image, hWindowControl.HalconWindow);
						hWindowControl.Dispose();
					}
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public void SaveWindowDumpEx(HObject image, HObject region, string imageType, string filePath, System.Drawing.Size size)
		{
			try
			{
				if (image != null)
				{
					if (image.IsInitialized())
					{
						HWindowControl hWindowControl = new HWindowControl();
						hWindowControl.WindowSize = size;
						this.InitHWindow(hWindowControl);
						this.DispImageFit(image, hWindowControl);
						if (region != null)
						{
							if (region.IsInitialized())
							{
								hWindowControl.HalconWindow.DispObj(region);
							}
						}
						string fileName = string.Concat(new string[]
						{
							filePath,
							"\\",
							DateTime.Now.ToString("HH-mm-ss"),
							".",
							imageType
						});
						hWindowControl.HalconWindow.DumpWindow(imageType, fileName);
						hWindowControl.Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void SaveWindowDumpEx(HObject image, HObject region, string imageType, string filePath)
		{
			try
			{
				if (image != null)
				{
					if (image.IsInitialized())
					{
						HWindowControl hWindowControl = new HWindowControl();
						HTuple t;
						HTuple t2;
						HOperatorSet.GetImageSize(image, out t, out t2);
						hWindowControl.WindowSize = new System.Drawing.Size(t, t2);
						this.InitHWindow(hWindowControl);
						this.DispImageFit(image, hWindowControl);
						if (region != null)
						{
							if (region.IsInitialized())
							{
								hWindowControl.HalconWindow.DispObj(region);
							}
						}
						string fileName = string.Concat(new string[]
						{
							filePath,
							"\\",
							DateTime.Now.ToString("HH-mm-ss-ffff"),
							".",
							imageType
						});
						hWindowControl.HalconWindow.DumpWindow(imageType, fileName);
						hWindowControl.Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void DispImageFit(HWindowControl hw_Ctrl)
		{
			if (this.hv_image != null)
			{
				if (this.hv_image.IsInitialized())
				{
					this.SetStripEnable(true);
					HOperatorSet.GetImageSize(this.hv_image, out this.hv_imageWidth, out this.hv_imageHeight);
					hw_Ctrl.HalconWindow.SetPart(0, 0, this.hv_imageHeight - 1, this.hv_imageWidth - 1);
                   
					this.hv_image.DispObj(hw_Ctrl.HalconWindow);
				}
			}
		}

		public void DispImageFit(HObject image, HWindowControl hw_Ctrl)
		{
			if (image != null)
			{
				if (image.IsInitialized())
				{
					this.SetStripEnable(true);
					HOperatorSet.GetImageSize(image, out this.hv_imageWidth, out this.hv_imageHeight);
					hw_Ctrl.HalconWindow.SetPart(0, 0, this.hv_imageHeight - 1, this.hv_imageWidth - 1);
					image.DispObj(hw_Ctrl.HalconWindow);
				}
			}
		}

		public void DispImageZoom(HWindowControl hw_Ctrl)
		{
			if (this.hv_image != null)
			{
				if (this.hv_image.IsInitialized())
				{
					HOperatorSet.GetImageSize(this.hv_image, out this.hv_imageWidth, out this.hv_imageHeight);
					double num = this.hv_imageWidth.D / this.hv_imageHeight.D;
					double num2 = (double)hw_Ctrl.Width * 1.0 / (double)hw_Ctrl.Height;
					double num3 = this.hv_imageHeight.D - 1.0;
					double num4 = this.hv_imageWidth.D - 1.0;
					double num6;
					double num7;
					if (num2 > num)
					{
						double num5 = num2 * this.hv_imageHeight.D;
						num6 = ((this.hv_imageHeight.D > (double)hw_Ctrl.Height) ? 0.0 : (-((double)hw_Ctrl.Height - this.hv_imageHeight.D) / 2.0));
						num7 = ((this.hv_imageWidth.D > (double)hw_Ctrl.Width) ? (-(num5 - this.hv_imageWidth.D) / 2.0) : (-((double)hw_Ctrl.Width - this.hv_imageWidth.D) / 2.0));
						num3 = ((this.hv_imageHeight.D > (double)hw_Ctrl.Height) ? (this.hv_imageHeight.D - 1.0) : (this.hv_imageHeight.D - 1.0 + ((double)hw_Ctrl.Height - this.hv_imageHeight.D) / 2.0));
						num4 = ((this.hv_imageWidth.D > (double)hw_Ctrl.Width) ? (this.hv_imageWidth.D - 1.0 + (num5 - this.hv_imageWidth.D) / 2.0) : (this.hv_imageWidth.D - 1.0 + ((double)hw_Ctrl.Width - this.hv_imageWidth.D) / 2.0));
					}
					else
					{
						double num8 = this.hv_imageWidth / num2;
						num6 = ((this.hv_imageHeight.D > (double)hw_Ctrl.Height) ? (-(num8 - this.hv_imageHeight.D) / 2.0) : (-((double)hw_Ctrl.Height - this.hv_imageHeight.D) / 2.0));
						num7 = ((this.hv_imageWidth.D > (double)hw_Ctrl.Width) ? 0.0 : (-((double)hw_Ctrl.Width - this.hv_imageWidth.D) / 2.0));
						num3 = ((this.hv_imageHeight.D > (double)hw_Ctrl.Height) ? (this.hv_imageHeight.D - 1.0 + (num8 - this.hv_imageHeight.D) / 2.0) : (this.hv_imageHeight.D - 1.0 + ((double)hw_Ctrl.Height - this.hv_imageHeight.D) / 2.0));
						num4 = ((this.hv_imageWidth.D > (double)hw_Ctrl.Width) ? (this.hv_imageWidth.D - 1.0) : (this.hv_imageWidth.D - 1.0 + ((double)hw_Ctrl.Width - this.hv_imageWidth.D) / 2.0));
					}
					this.zoom_beginRow = (int)num6;
					this.zoom_beginCol = (int)num7;
					this.zoom_endRow = (int)num3;
					this.zoom_endCol = (int)num4;
					this.SetStripEnable(true);
					hw_Ctrl.HalconWindow.SetPart(this.zoom_beginRow, this.zoom_beginCol, this.zoom_endRow, this.zoom_endCol);
					this.hv_image.DispObj(hw_Ctrl.HalconWindow);
				}
			}
		}

		public void DispImage(HObject image)
		{
			if (image != null)
			{
				if (image.IsInitialized())
				{
					this.hv_image = image;
					this.SetStripEnable(true);
					this.mCtrl_HWindow.HalconWindow.GetPart(out this.current_beginRow, out this.current_beginCol, out this.current_endRow, out this.current_endCol);
					this.mCtrl_HWindow.HalconWindow.SetPart(this.current_beginRow, this.current_beginCol, this.current_endRow, this.current_endCol);
					image.DispObj(this.mCtrl_HWindow.HalconWindow);
				}
			}
		}



		public void DispRegion(HObject region)
        {
			if (region != null || region.IsInitialized())
			{
				this.hv_region = region;
				region.DispObj(this.mCtrl_HWindow.HalconWindow);
			}
		}

		public void DispImage(HWindowControl hw_Ctrl)
		{
			if (this.hv_image != null)
			{
				if (this.hv_image.IsInitialized())
				{
					this.SetStripEnable(true);
					this.hv_image.DispObj(hw_Ctrl.HalconWindow);
				}
			}
		}

		public void DispImage(HObject image, System.Drawing.Rectangle Rect)
		{
			if (image != null)
			{
				if (image.IsInitialized())
				{
					this.hv_image = image;
					this.SetStripEnable(true);
					this.mCtrl_HWindow.HalconWindow.SetPart(Rect.Top, Rect.Left, Rect.Bottom, Rect.Right);
					image.DispObj(this.mCtrl_HWindow.HalconWindow);
				}
			}
		}

		public void DispImage(HObject image, HObject region, System.Drawing.Rectangle Rect)
		{
			if (image != null)
			{
				if (image.IsInitialized())
				{
					this.hv_image = image;
					this.SetStripEnable(true);
					this.mCtrl_HWindow.HalconWindow.SetPart(Rect.Top, Rect.Left, Rect.Bottom, Rect.Right);
					image.DispObj(this.mCtrl_HWindow.HalconWindow);
					this.hv_region = region;
				}
			}
			if (region != null || region.IsInitialized())
			{
				this.hv_region = region;
				region.DispObj(this.mCtrl_HWindow.HalconWindow);
			}
		}

		public void DispImage(HObject image, System.Drawing.Rectangle Rect, HWindowControl hw_Ctrl)
		{
			if (image != null)
			{
				if (image.IsInitialized())
				{
					this.SetStripEnable(true);
					hw_Ctrl.HalconWindow.SetPart(Rect.Top, Rect.Left, Rect.Bottom, Rect.Right);
					image.DispObj(hw_Ctrl.HalconWindow);
				}
			}
		}

		public void ClearHWindow()
		{
			this.mCtrl_HWindow.HalconWindow.ClearWindow();
			this.SetStripEnable(false);
		}

		public void InitHWindow(HWindowControl hWindowControl)
		{
			hWindowControl.HalconWindow.SetDraw("margin");
			if (this.colored > 0)
			{
				hWindowControl.HalconWindow.SetColored(this.colored);
			}
			else
			{
				hWindowControl.HalconWindow.SetColor(this.color);
			}
		}

		private void panelVisible_strip_CheckedChanged(object sender, EventArgs e)
		{
			this.SetTablePanelVisible(this.panelVisible_strip.Checked);
		}

		private void barVisible_strip_CheckedChanged(object sender, EventArgs e)
		{
			this.SetStatusStripVisible(this.barVisible_strip.Checked);
		}

		private void HWindowControl_HMouseWheel(object sender, HMouseEventArgs e)
		{
			if (this.hv_image != null)
			{
				if (this.hv_image.IsInitialized())
				{
					try
					{
						int num;
						this.mCtrl_HWindow.HalconWindow.GetMpositionSubPix(out this.mposition_row, out this.mposition_col, out num);
						this.mCtrl_HWindow.HalconWindow.GetPart(out this.current_beginRow, out this.current_beginCol, out this.current_endRow, out this.current_endCol);
					}
					catch (Exception ex)
					{
						this.m_CtrlHStatusLabelCtrl.Text = ex.Message;
					}
					if (e.Delta > 0)
					{
						this.zoom_beginRow = (int)((double)this.current_beginRow + (this.mposition_row - (double)this.current_beginRow) * 0.3);
						this.zoom_beginCol = (int)((double)this.current_beginCol + (this.mposition_col - (double)this.current_beginCol) * 0.3);
						this.zoom_endRow = (int)((double)this.current_endRow - ((double)this.current_endRow - this.mposition_row) * 0.3);
						this.zoom_endCol = (int)((double)this.current_endCol - ((double)this.current_endCol - this.mposition_col) * 0.3);
					}
					else
					{
						this.zoom_beginRow = (int)(this.mposition_row - (this.mposition_row - (double)this.current_beginRow) / 0.7);
						this.zoom_beginCol = (int)(this.mposition_col - (this.mposition_col - (double)this.current_beginCol) / 0.7);
						this.zoom_endRow = (int)(this.mposition_row + ((double)this.current_endRow - this.mposition_row) / 0.7);
						this.zoom_endCol = (int)(this.mposition_col + ((double)this.current_endCol - this.mposition_col) / 0.7);
					}
					try
					{
						int width = this.mCtrl_HWindow.WindowSize.Width;
						int height = this.mCtrl_HWindow.WindowSize.Height;
						bool flag = this.zoom_beginRow >= this.hv_imageHeight || this.zoom_endRow <= 0 || this.zoom_beginCol >= this.hv_imageWidth || this.zoom_endCol < 0;
						bool flag2 = this.zoom_endRow - this.zoom_beginRow > this.hv_imageHeight * 500 || this.zoom_endCol - this.zoom_beginCol > this.hv_imageWidth * 500;
						bool flag3 = height / (this.zoom_endRow - this.zoom_beginRow) > 500 || width / (this.zoom_endCol - this.zoom_beginCol) > 500;
						if (!flag && !flag2)
						{
							if (!flag3)
							{
								this.mCtrl_HWindow.HalconWindow.ClearWindow();
								this.mCtrl_HWindow.HalconWindow.SetPaint(new HTuple("default"));
								this.mCtrl_HWindow.HalconWindow.SetPart(this.zoom_beginRow, this.zoom_beginCol, this.zoom_endRow, this.zoom_endCol);
								this.mCtrl_HWindow.HalconWindow.DispObj(this.hv_image);
								if (this.hv_region != null)
								{
									if (this.hv_region.IsInitialized())
									{
										this.mCtrl_HWindow.HalconWindow.DispObj(this.hv_region);
									}
								}
							}
						}
					}
					catch (Exception)
					{
						this.DispImageFit(this.mCtrl_HWindow);
						this.m_CtrlHStatusLabelCtrl.Text = "";
					}
				}
			}
		}

        private void HWindowControl_HMouseUp(object sender, HMouseEventArgs e)
        {
            
            if (this.hv_image != null)
            {
                if (this.hv_image.IsInitialized())
                {
                    if(this.isMeasureMode)
                    {
                        if(measureFristClickFlag == false)
                        {
                            measureStartPoint.X = (float)this.mposition_row;
                            measureStartPoint.Y = (float)this.mposition_col;
                            measureFristClickFlag = true;
                        }
                        else
                        {
                            measureEndPoint.X = (float)this.mposition_row;
                            measureEndPoint.Y = (float)this.mposition_col;
                            try
                            {
                                HTuple distance = null;
                                HOperatorSet.DistancePp(measureStartPoint.X, measureStartPoint.Y, measureEndPoint.X, measureEndPoint.Y, out distance);
                                if(umPerPix == -1)
                                {
                                    MessageBox.Show(string.Format("距离:{0}pix", distance.D.ToString("f2")), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("距离:{0}pix({1}um)", distance.D.ToString("f2"), (distance.D * umPerPix).ToString("f2")), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(string.Format("距离失败!详细信息:{0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            
                            measureFristClickFlag = false;
                            this.mCtrl_HWindow.Cursor = Cursors.Arrow;
                            this.isMeasureMode = false;
                            this.mCtrl_HWindow.HalconWindow.SetPaint(new HTuple("default"));
                            this.mCtrl_HWindow.HalconWindow.SetPart(this.zoom_beginRow, this.zoom_beginCol, this.zoom_endRow, this.zoom_endCol);
                            this.mCtrl_HWindow.HalconWindow.DispObj(this.hv_image);
                            if (this.hv_region != null)
                            {
                                if (this.hv_region.IsInitialized())
                                {
                                    this.mCtrl_HWindow.HalconWindow.DispObj(this.hv_region);
                                }
                            }
                        }  
                    }
                    else
                    {
                        if(MouseClickEvent != null)
                        {
                            if(isRightClick)
                                MouseClickEvent(new PointF((float)mposition_row, (float)mposition_col));
                        }
                    }
                }
            }
        }


        private void HWindowControl_HMouseMove(object sender, HMouseEventArgs e)
		{
			if (this.hv_image != null)
			{
				if (this.hv_image.IsInitialized())
				{
					try
					{
						HTuple channels;
						HOperatorSet.CountChannels(this.hv_image, out channels);
						double num;
						double num2;
						int button;
						this.mCtrl_HWindow.HalconWindow.GetMpositionSubPix(out num, out num2, out button);
						if (button == 17)
						{
							try
							{
								HOperatorSet.SetWindowExtents(this.bufferWindow.HalconWindow, 0, 0, this.mCtrl_HWindow.Width, this.mCtrl_HWindow.Height);
								this.mCtrl_HWindow.Cursor = Cursors.Hand;
								HTuple t2;
								HTuple t3;
								HTuple t4;
								HTuple t5;
								HOperatorSet.GetPart(this.mCtrl_HWindow.HalconWindow, out t2, out t3, out t4, out t5);
								double num4 = num - this.mposition_row;
								double num5 = num2 - this.mposition_col;
								if (Math.Abs(num4) < 1.0 && Math.Abs(num5) < 1.0)
								{
									this.mposition_row = num;
									this.mposition_col = num2;
								}
								else
								{
									HOperatorSet.SetPart(this.bufferWindow.HalconWindow, t2 - num4, t3 - num5, t4 - num4, t5 - num5);
									HOperatorSet.ClearWindow(this.bufferWindow.HalconWindow);
									HOperatorSet.DispObj(this.hv_image, this.bufferWindow.HalconWindow);
									if (this.hv_region != null)
									{
										if (this.hv_region.IsInitialized())
										{
											HOperatorSet.DispObj(this.hv_region, this.bufferWindow.HalconWindow);
										}
									}
									HOperatorSet.CopyRectangle(this.bufferWindow.HalconWindow, this.mCtrl_HWindow.HalconWindow, 0, 0, this.mCtrl_HWindow.Height - 1, this.mCtrl_HWindow.Width - 1, 0, 0);
									HOperatorSet.SetPart(this.mCtrl_HWindow.HalconWindow, t2 - num4, t3 - num5, t4 - num4, t5 - num5);
								}
							}
							catch (Exception)
							{
							}
						}
						else
						{
                            //if(!this.isMeasureMode)
                            //{
                            //    this.mCtrl_HWindow.Cursor = Cursors.Arrow;
                            //}
                            //else
                            //{
                            //    this.mCtrl_HWindow.Cursor = Cursors.Cross;
                            //    if (measureFristClickFlag)
                            //    {
                            //        if ((num < this.hv_imageHeight.D  && num > 0) && (num2 < this.hv_imageWidth.D && num2 > 0))
                            //        {
                            //            // this.mCtrl_HWindow.HalconWindow.ClearWindow();
                            //            this.mCtrl_HWindow.HalconWindow.SetPaint(new HTuple("default"));
                            //            this.mCtrl_HWindow.HalconWindow.SetPart(this.zoom_beginRow, this.zoom_beginCol, this.zoom_endRow, this.zoom_endCol);
                            //            this.mCtrl_HWindow.HalconWindow.DispObj(this.hv_image);
                            //            if (this.hv_region != null)
                            //            {
                            //                if (this.hv_region.IsInitialized())
                            //                {
                            //                    this.mCtrl_HWindow.HalconWindow.DispObj(this.hv_region);
                            //                }
                            //            }
                            //            this.mCtrl_HWindow.HalconWindow.SetColor("red");
                            //            this.mCtrl_HWindow.HalconWindow.DispLine(measureStartPoint.X, measureStartPoint.Y, num, num2);
                            //        }
                            //    }    
                            //}
							this.mposition_row = num;
							this.mposition_col = num2;
							string text = string.Format("X: {0:0000.0}, Y: {1:0000.0}", num2, num);

							HOperatorSet.GetImageSize(this.hv_image, out this.hv_imageWidth, out this.hv_imageHeight);

							bool flag = num2 < 0.0 || num2 >= this.hv_imageWidth;
							bool flag2 = num < 0.0 || num >= this.hv_imageHeight;
							if (!flag && !flag2)
							{
                                string text2 = "";
                                if (channels == 1)
                                {
                                    HTuple hTuple;
                                    HOperatorSet.GetGrayval(this.hv_image, (int)num, (int)num2, out hTuple);
                                    text2 = string.Format("Val: {0:000.0}", hTuple.D);
                                }
                                else if (channels == 3)
                                {
                                    HObject hObject;
                                    HOperatorSet.AccessChannel(this.hv_image, out hObject, 1);
                                    HObject hObject2;
                                    HOperatorSet.AccessChannel(this.hv_image, out hObject2, 2);
                                    HObject hObject3;
                                    HOperatorSet.AccessChannel(this.hv_image, out hObject3, 3);
                                    HTuple hTuple2;
                                    HOperatorSet.GetGrayval(hObject, (int)num, (int)num2, out hTuple2);
                                    HTuple hTuple3;
                                    HOperatorSet.GetGrayval(hObject2, (int)num, (int)num2, out hTuple3);
                                    HTuple hTuple4;
                                    HOperatorSet.GetGrayval(hObject3, (int)num, (int)num2, out hTuple4);
                                    hObject.Dispose();
                                    hObject2.Dispose();
                                    hObject3.Dispose();
                                    text2 = string.Format("Val: ({0:000.0}, {1:000.0}, {2:000.0})", hTuple2.D, hTuple3.D, hTuple4.D);
                                }
                            else
                            {
                                text2 = "";
                            }
                                this.m_CtrlHStatusLabelCtrl.Text = string.Concat(new string[]
                                {
                                    this.str_imgSize,
                                    "    ",
                                    text,
                                    "    ",
                                    text2
                                });
                            }
						}
					}
					catch (Exception ex)
					{
						this.m_CtrlHStatusLabelCtrl.Text = "";
					}
				}
			}
		}

		private void rbPoint_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.rbPoint.Checked && this.isRightClick && !isMeasureMode)
				{
					this.mCtrl_HWindow.Focus();
					this.InitHWindow(this.mCtrl_HWindow);
					this.isRightClick = false;
					this.mCtrl_HWindow.ContextMenuStrip = null;
					this.regionType = "";
					HOperatorSet.DrawPoint(this.mCtrl_HWindow.HalconWindow, out this.row, out this.column);
					this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
					this.regionType = "Point";
					this.isRightClick = true;

                    if(DrawRoiDownEvent != null)
                    {
                        DrawRoiDownEvent(this.regionType);
                    }
				}
			}
			catch (Exception)
			{
				this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
				this.isRightClick = true;
			}
		}

		private void rbLine_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.rbLine.Checked && this.isRightClick && !isMeasureMode)
				{
					this.mCtrl_HWindow.Focus();
					this.InitHWindow(this.mCtrl_HWindow);
					this.isRightClick = false;
					this.mCtrl_HWindow.ContextMenuStrip = null;
					this.regionType = "";
					HOperatorSet.DrawLine(this.mCtrl_HWindow.HalconWindow, out this.row1, out this.column1, out this.row2, out this.column2);
					this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
					this.regionType = "Line";
					this.isRightClick = true;

                    if (DrawRoiDownEvent != null)
                    {
                        DrawRoiDownEvent(this.regionType);
                    }
                }
			}
			catch (Exception)
			{
				this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
				this.isRightClick = true;
			}
		}

		private void rbRect1_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.rbRect1.Checked && this.isRightClick && !isMeasureMode)
				{
					this.mCtrl_HWindow.Focus();
					this.InitHWindow(this.mCtrl_HWindow);
					this.isRightClick = false;
					this.mCtrl_HWindow.ContextMenuStrip = null;
					this.regionType = "";
					HOperatorSet.DrawRectangle1(this.mCtrl_HWindow.HalconWindow, out this.row1, out this.column1, out this.row2, out this.column2);
					this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
					this.regionType = "Rectangle1";
					this.isRightClick = true;
                    if (DrawRoiDownEvent != null)
                    {
                        DrawRoiDownEvent(this.regionType);
                    }
                }
			}
			catch (Exception)
			{
				this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
				this.isRightClick = true;
			}
		}

		private void rbRect2_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.rbRect2.Checked && this.isRightClick && !isMeasureMode)
				{
					this.mCtrl_HWindow.Focus();
					this.InitHWindow(this.mCtrl_HWindow);
					this.isRightClick = false;
					this.mCtrl_HWindow.ContextMenuStrip = null;
					this.regionType = "";
					HOperatorSet.DrawRectangle2(this.mCtrl_HWindow.HalconWindow, out this.row, out this.column, out this.phi, out this.length1, out this.length2);
					this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
					this.regionType = "Rectangle2";
					this.isRightClick = true;
                    if (DrawRoiDownEvent != null)
                    {
                        DrawRoiDownEvent(this.regionType);
                    }
                }
			}
			catch (Exception)
			{
				this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
				this.isRightClick = true;
			}
		}

		private void rbCircle_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.rbCircle.Checked && this.isRightClick && !isMeasureMode)
				{
					this.mCtrl_HWindow.Focus();
					this.InitHWindow(this.mCtrl_HWindow);
					this.isRightClick = false;
					this.mCtrl_HWindow.ContextMenuStrip = null;
					this.regionType = "";
					HOperatorSet.DrawCircle(this.mCtrl_HWindow.HalconWindow, out this.row, out this.column, out this.radius);
					this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
					this.regionType = "Circle";
					this.isRightClick = true;
                    if (DrawRoiDownEvent != null)
                    {
                        DrawRoiDownEvent(this.regionType);
                    }
                }
			}
			catch (Exception)
			{
				this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
				this.isRightClick = true;
			}
		}

		private void rbEllipse_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.rbEllipse.Checked && this.isRightClick && !isMeasureMode)
				{
					this.mCtrl_HWindow.Focus();
					this.InitHWindow(this.mCtrl_HWindow);
					this.isRightClick = false;
					this.mCtrl_HWindow.ContextMenuStrip = null;
					this.regionType = "";
					HOperatorSet.DrawEllipse(this.mCtrl_HWindow.HalconWindow, out this.row, out this.column, out this.phi, out this.radius1, out this.radius2);
					this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
					this.regionType = "Ellipse";
					this.isRightClick = true;
                    if (DrawRoiDownEvent != null)
                    {
                        DrawRoiDownEvent(this.regionType);
                    }
                }
			}
			catch (Exception)
			{
				this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
				this.isRightClick = true;
			}
		}

		private void rbRegion_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.rbRegion.Checked && this.isRightClick && !isMeasureMode)
				{
					this.mCtrl_HWindow.Focus();
					this.InitHWindow(this.mCtrl_HWindow);
					this.isRightClick = false;
					this.mCtrl_HWindow.ContextMenuStrip = null;
					this.regionType = "";
					HOperatorSet.DrawRegion(out this.region, this.mCtrl_HWindow.HalconWindow);
					HOperatorSet.WaitSeconds(0.1);
					HTuple hTuple;
					HTuple hTuple2;
					HTuple hTuple3;
					HTuple hTuple4;
					HOperatorSet.GetPart(this.mCtrl_HWindow.HalconWindow, out hTuple, out hTuple2, out hTuple3, out hTuple4);
					HOperatorSet.SetPart(this.mCtrl_HWindow.HalconWindow, hTuple, hTuple2, hTuple3, hTuple4);
					HOperatorSet.DispObj(this.hv_image, this.mCtrl_HWindow.HalconWindow);
					this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
					this.regionType = "Region";
					this.isRightClick = true;
                    if (DrawRoiDownEvent != null)
                    {
                        DrawRoiDownEvent(this.regionType);
                    }
                }
			}
			catch (Exception)
			{
				this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
				this.isRightClick = true;
			}
		}

		private void rbXld_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.rbXld.Checked && this.isRightClick && !isMeasureMode)
				{
					this.mCtrl_HWindow.Focus();
					this.InitHWindow(this.mCtrl_HWindow);
					this.isRightClick = false;
					this.mCtrl_HWindow.ContextMenuStrip = null;
					this.regionType = "";
					HOperatorSet.DrawXld(out this.region, this.mCtrl_HWindow.HalconWindow, "true", "true", "true", "true");
					HTuple hTuple;
					HTuple hTuple2;
					HTuple hTuple3;
					HTuple hTuple4;
					HOperatorSet.GetPart(this.mCtrl_HWindow.HalconWindow, out hTuple, out hTuple2, out hTuple3, out hTuple4);
					HOperatorSet.SetPart(this.mCtrl_HWindow.HalconWindow, hTuple, hTuple2, hTuple3, hTuple4);
					HOperatorSet.DispObj(this.hv_image, this.mCtrl_HWindow.HalconWindow);
					this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
					this.regionType = "Xld";
					this.isRightClick = true;
                    if (DrawRoiDownEvent != null)
                    {
                        DrawRoiDownEvent(this.regionType);
                    }
                }
			}
			catch (Exception)
			{
				this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
				this.isRightClick = true;
			}
		}

        private void rbMeasure_Click(object sender, EventArgs e)
        {

            HDrawingObject circle = HDrawingObject.CreateDrawingObject(
             HDrawingObject.HDrawingObjectType.CIRCLE, 200, 200, 70);
            circle.SetDrawingObjectParams("color", "magenta");
            this.mCtrl_HWindow.HalconWindow.AttachDrawingObjectToWindow(circle);
            //if (this.rbMeasure.Checked && this.isRightClick && !this.isMeasureMode)
            //{
            //    this.isMeasureMode = true;

            //}
        }


        #region  新增可编辑区域按钮rect1Ex 用于图片放大缩小时编辑

        private HDrawingObject selected_drawing_object = null;

        private bool flag = true;

        private HImage _himage = null;

        private List<HDrawingObject> drawing_objects = new List<HDrawingObject>();

        public string DrawRectangle1Ex(string color = "red", int lineWidth = 2)
        {
            string err = "";
            if (this.rbRect1Ex.Checked && this.flag && !isMeasureMode)
            {
                try
                {
                    this.mCtrl_HWindow.Focus();
                    this.flag = false;

                    HTuple row1, col1, row2, col2;
                    HOperatorSet.SetColor(this.mCtrl_HWindow.HalconWindow, color);
                    HOperatorSet.SetLineWidth(this.mCtrl_HWindow.HalconWindow, lineWidth);
                    HOperatorSet.DrawRectangle1(this.mCtrl_HWindow.HalconWindow, out row1, out col1, out row2, out col2);
                    if (this.hv_image != null)
                    {
                        HDrawingObject rect1 = HDrawingObject.CreateDrawingObject(HDrawingObject.HDrawingObjectType.RECTANGLE1, row1, col1, row2, col2);
                        rect1.SetDrawingObjectParams("color", color);
                        rect1.SetDrawingObjectParams(new HTuple("line_width"), new HTuple(lineWidth));
                        AttachDrawObj(rect1);
                        

                    }
                    else
                    {
                        err = "图片为空";
                    }
                    this.InitHWindow(this.mCtrl_HWindow);
                    
                   
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                }
            }
                return err;
            
        }

        private void AttachDrawObj(HDrawingObject obj)
        {
            drawing_objects.Add(obj);
            obj.OnSelect(OnSelectHandle);
            obj.OnDrag(OnDragHandle);
            if (selected_drawing_object == null)
                selected_drawing_object = obj;
            this.mCtrl_HWindow.HalconWindow.AttachDrawingObjectToWindow(obj);
            if (this._himage != null)
                _himage.Dispose();
            _himage = new HImage(this.hv_image);
            this.mCtrl_HWindow.HalconWindow.AttachBackgroundToWindow(_himage);
        }

        #region ROI事件

        private void OnDragHandle(HDrawingObject drawid, HWindow window, string type)
        {

        }

        private void OnSelectHandle(HDrawingObject drawid, HWindow window, string type)
        {
            selected_drawing_object = drawid;
        }

        private void OnResizeHandle(HDrawingObject drawid, HWindow window, string type)
        {

        }



        #endregion

        #endregion

        private void rbRect1Ex_Click(object sender, EventArgs e)
        {
            if(rbRect1Ex.Checked&&this.flag)
            {
                confirmReg_strip.Enabled = true;
                DrawRectangle1Ex();
               
            }
            else
            {
                 MessageBox.Show("请先确认之前的区域！！！\n  右键确认区域");
            }
            
        }

        private void confirmReg_strip_Click(object sender, EventArgs e)
        {
            try
            {

                string err = DeleteSelectROI();
                if (err != "")
                {
                    MessageBox.Show("确定选中ROI失败：" + err);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "请选择一个region");
            }

            confirmReg_strip.Enabled = false;
            rbRect1Ex.Checked = false;
            this.flag = true;
        }

        public string DeleteSelectROI()
        {
            string err = "";
            try
            {
                
                //this.InitHWindow(this.mCtrl_HWindow);
                this.isRightClick = false;
                
                this.mCtrl_HWindow.ContextMenuStrip = null;
                HObject region = null;
                region = selected_drawing_object.GetDrawingObjectIconic();
                HOperatorSet.SmallestRectangle1(region, out this.row1, out this.column1, out this.row2, out this.column2);
                for (int i = 0; i < drawing_objects.Count; i++)
                {
                    drawing_objects[i].Dispose();
                }
                drawing_objects.Clear();
                selected_drawing_object = null;
                GC.Collect();

                this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
                this.regionType = "Rectangle1";
                this.isRightClick = true;
                if (DrawRoiDownEvent != null)
                {
                    DrawRoiDownEvent(this.regionType);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return err;
        }
    }
}
