using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Drawing;
using System.Collections.ObjectModel;

namespace LFAOIRecipe
{
    /// <summary>
    /// HTWindowControlWPF.xaml 的交互逻辑
    /// </summary>
    public  partial class HTHalControlWPF : UserControl, IDisposable
    {
        public HTHalControlWPF()
        {
            InitializeComponent();
            hTWindow.HMouseWheel += HTWindow_HMouseWheel;
            hTWindow.HMouseMove += HTWindow_HMouseMove;
            hTWindow.HMouseUp += HTWindow_HMouseUp;//
            hTWindow.HMouseDown += HTWindow_HMouseDown;//
            hTWindow.SizeChanged += HTWindow_SizeChanged;
            //框选初始化
            btn_Select.Opacity = 0.6;
            isSelectState = false;
            isBoxSelectState = false;
        }

        //1028
        public bool isdeleted = false;

        public HObject LightImage { get; set; }
        public HObject DarkImage { get; set; }
        HTuple Ic_mat = null;

        private HTuple row_Point;
        public HTuple Row_Point => row_Point;

        private HTuple column_Point;
        public HTuple Column_Point => column_Point;

        private HTuple row1_Line;
        public HTuple Row1_Line => row1_Line;

        private HTuple column1_Line;
        public HTuple Column1_Line => column1_Line;

        private HTuple row2_Line;
        public HTuple Row2_Line => row2_Line;

        private HTuple column2_Line;
        public HTuple Column2_Line => column2_Line;

        private HTuple row1_Rectangle1;
        public HTuple Row1_Rectangle1 => row1_Rectangle1;

        private HTuple column1_Rectangle1;
        public HTuple Column1_Rectangle1 => column1_Rectangle1;

        private HTuple row2_Rectangle1;
        public HTuple Row2_Rectangle1 => row2_Rectangle1;

        private HTuple column2_Rectangle1;
        public HTuple Column2_Rectangle1 => column2_Rectangle1;

        private HTuple row_Rectangle2;
        public HTuple Row_Rectangle2 => row_Rectangle2;

        private HTuple column_Rectangle2;
        public HTuple Column_Rectangle2 => column_Rectangle2;

        private HTuple phi_Rectangle2;
        public HTuple Phi_Rectangle2 => phi_Rectangle2;

        private HTuple length1_Rectangle2;
        public HTuple Length1_Rectangle2 => length1_Rectangle2;

        private HTuple length2_Rectangle2;
        public HTuple Length2_Rectangle2 => length2_Rectangle2;

        private HTuple row_Circle;
        public HTuple Row_Circle => row_Circle;

        private HTuple column_Circle;
        public HTuple Column_Circle => column_Circle;

        private HTuple radius_Circle;
        public HTuple Radius_Circle => radius_Circle;

        private HTuple row_Ellipse;
        public HTuple Row_Ellipse => row_Ellipse;

        private HTuple column_Ellipse;
        public HTuple Column_Ellipse => column_Ellipse;

        private HTuple phi_Ellipse;
        public HTuple Phi_Ellipse => phi_Ellipse;

        private HTuple radius1_Ellipse;
        public HTuple Radius1_Ellipse => radius1_Ellipse;

        private HTuple radius2_Ellipse;
        public HTuple Radius2_Ellipse => radius2_Ellipse;

        private BondMatchAutoRegions bondMatchAutoRegions;

        private CreateAutoBondMeasureModel createAutoBondMeasureModel;

        private BondMeasureVerify bondMeasureVerify;

        private BondMatchVerify bondMatchVerify;
        //金线自动生成
        private WireAddAutoRegion wireAddAutoRegion;

        private InspectNode inspectNode;

        private GoldenModelInspectVerify goldenModelInspectVerify;

        private CreateAroundBondRegionModel createAroundBondRegionModel;

        private HTuple StrIndex;

        //画任意形状
        private HObject region_Region;
        public HObject Region_Region => region_Region;

        public HObject Region { get; private set; }

        public RegionType RegionType { get; set; } = RegionType.Null;

        public HObject Image { get; private set; }

        public HWindowControlWPF HTWindow => hTWindow;

        private HTuple imageWidth;

        private HTuple imageHeight;

        private string imageSize;

        private int zoomBeginRow = 0;
        private int zoomBeginColumn = 0;
        private int zoomEndRow = 0;
        private int zoomEndColumn = 0;

        private bool isRightClick = true;

        //测量模式
        private bool isMeasureMode = false;

        //平移模式
        private bool isMoveMode = false;

        //测量起点坐标
        private PointF measureStartPoint = new PointF();

        //测量终点坐标
        private PointF measureEndPoint = new PointF();

        //测量起点是否点击确定
        private bool measureFristClickFlag = false;

        //每像素代表的实际物理距离  如果为-1，则未设置，不显示
        private double umPerPix = -1;

        //单像素实际物理距离
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

        /////////////////////////////////////////////////////连续画框模式
        private bool isMultiDraw = false;

        private bool draw_Mul = false;

        private bool draw_Mul_mousemove = false;

        //拾取模式
        private bool isSelectState = false;
        private bool isBoxSelectState = false;

        //记录当前话框的Type  lht test
        public RegionType Cur_type { get; set; } = RegionType.Null;

        //0817 画区域获取焦点次数，Halcon每执行一次画区域后就会创建一个消息队列，等待右键完成当前的画区域操作，
        //如果在等待画区域过程中又产生了画区域的命令，则会需要多次右键操作来结束连画区域的队列
        public int RightClick_buffer = 0;
        public string Region_str = "";

        protected ObservableCollection<UserRegion> MultiDrawRegions = new ObservableCollection<UserRegion>();
        private IEnumerable<HObject> ShowRegions => MultiDrawRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        private double mPositionRow;

        private double mPositionColumn;

        public double mPositionDownRow;

        public double mPositionDownColumn;

        public void useAutoBond(BondMatchAutoRegions _bondMatchAutoRegions)
        {
            this.bondMatchAutoRegions = _bondMatchAutoRegions;
        }
        //add by wj 2021-01-05
        public void useBondVerifyAndPad(BondMeasureVerify _bondMeasureVerify)
        {
            this.bondMeasureVerify = _bondMeasureVerify;
        }
        //add by wj 2021-01-07
        public void useBond2Verify(BondMatchVerify _bondMatchVerify)
        {
            this.bondMatchVerify = _bondMatchVerify;
        }
        public void useAutoBond1(CreateAutoBondMeasureModel _createAutoBondMeasureModel)
        {
            this.createAutoBondMeasureModel = _createAutoBondMeasureModel;
        }

        //金线自动生成
        public void useAutoBondAndWire(WireAddAutoRegion _wireAddAutoRegion)
        {
            this.wireAddAutoRegion = _wireAddAutoRegion;
        }

        public void useInspectNode(InspectNode _inspectNode)
        {
            this.inspectNode = _inspectNode;
        }

        public void useGoldenModelInspectVerify(GoldenModelInspectVerify _goldenModelInspectVerify)
        {
            this.goldenModelInspectVerify = _goldenModelInspectVerify;
        }

        public void useCreateAroundBondRegionModel(CreateAroundBondRegionModel _createAroundBondRegionModel)
        {
            this.createAroundBondRegionModel = _createAroundBondRegionModel;
        }

        private void Btn_Point_Click(object sender, RoutedEventArgs e)
        {
            if (isRightClick && !isMeasureMode)
            {
                isRightClick = false;
                InitialHWindow();
                hTWindow.Focus();
                try
                {
                    HOperatorSet.DrawPoint(hTWindow.HalconWindow,
                                           out row_Point,
                                           out column_Point);
                    RegionType = RegionType.Point;
                }
                catch { }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void Btn_Line_Click(object sender, RoutedEventArgs e)
        {
            if (isRightClick && !isMeasureMode)
            {
                isRightClick = false;
                InitialHWindow();
                hTWindow.Focus();
                try
                {
                    HOperatorSet.DrawLine(hTWindow.HalconWindow,
                                          out row1_Line,
                                          out column1_Line,
                                          out row2_Line,
                                          out column2_Line);
                    RegionType = RegionType.Line;
                }
                catch { }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void Btn_Rectangle1_Click(object sender, RoutedEventArgs e)
        {
            if (isRightClick && !isMeasureMode)
            {
                isRightClick = false;
                InitialHWindow();
                hTWindow.Focus();
                try
                {
                    HOperatorSet.DrawRectangle1(hTWindow.HalconWindow,
                                                out row1_Rectangle1,
                                                out column1_Rectangle1,
                                                out row2_Rectangle1,
                                                out column2_Rectangle1);
                    RegionType = RegionType.Rectangle1;

                    //1020 if in select state, 
                    if (isBoxSelectState == true)
                    {
                        //wireAddAutoRegion.GetClickDownPointsFromStartBall(); GetBoxSelectioinFromStartBall
                        wireAddAutoRegion.GetBoxSelectioinFromStartBall();

                    }
                }
                catch { }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void Btn_Rectangle2_Click(object sender, RoutedEventArgs e)
        {
            if (isRightClick && !isMeasureMode)
            {
                isRightClick = false;
                InitialHWindow();
                hTWindow.Focus();
                try
                {
                    HOperatorSet.DrawRectangle2(hTWindow.HalconWindow,
                                                out row_Rectangle2,
                                                out column_Rectangle2,
                                                out phi_Rectangle2,
                                                out length1_Rectangle2,
                                                out length2_Rectangle2);
                    RegionType = RegionType.Rectangle2;
                }
                catch { }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void Btn_Circle_Click(object sender, RoutedEventArgs e)
        {
            if (isRightClick && !isMeasureMode)
            {
                isRightClick = false;
                InitialHWindow();
                hTWindow.Focus();
                try
                {
                    HOperatorSet.DrawCircle(hTWindow.HalconWindow,
                                            out row_Circle,
                                            out column_Circle,
                                            out radius_Circle);
                    RegionType = RegionType.Circle;
                }
                catch { }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void Btn_Ellipse_Click(object sender, RoutedEventArgs e)
        {
            if (isRightClick && !isMeasureMode)
            {
                isRightClick = false;
                InitialHWindow();
                hTWindow.Focus();
                try
                {
                    HOperatorSet.DrawEllipse(hTWindow.HalconWindow,
                                                out row_Ellipse,
                                                out column_Ellipse,
                                                out phi_Ellipse,
                                                out radius1_Ellipse,
                                                out radius2_Ellipse);
                    RegionType = RegionType.Ellipse;
                }
                catch { }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void Btn_fresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshWindow();
            isMultiDraw = false;
        }

        public void InitialHWindow(string color = "", string setDraw = "margin")
        {
            //hTWindow.HalconWindow.SetDraw("margin");
            hTWindow.HalconWindow.SetDraw(setDraw);
            hTWindow.HalconWindow.SetLineWidth(2);
            if (color.Equals(string.Empty))
            {
                hTWindow.HalconWindow.SetColor("green");
            }
            else
            {
                hTWindow.HalconWindow.SetColor(color);
            }
        }

        //带优化，功能待添加
        public void InitialHWindowUpdate(HTuple Row1_Rectangle_Small, HTuple Column1_Rectangle_Small, HTuple Row2_Rectangle_Small,
                                        HTuple Column2_Rectangle_Small, RegionType regionType,
                                        double rowOffset = 0, double columnOffset = 0, string color = "")//
        {
            hTWindow.HalconWindow.SetDraw("margin");
            hTWindow.HalconWindow.SetLineWidth(2);
            if (color.Equals(string.Empty))
            {
                hTWindow.HalconWindow.SetColor("green");
            }
            else
            {
                hTWindow.HalconWindow.SetColor(color);
            }

            switch (regionType)
            {
                case RegionType.Point:
                    return;

                case RegionType.Rectangle1:
                    hTWindow.HalconWindow.DispCross(Row1_Rectangle_Small.D - rowOffset, Column1_Rectangle_Small.D - columnOffset, ((Row2_Rectangle_Small - Row1_Rectangle_Small) / 10 + (Column2_Rectangle_Small - Column1_Rectangle_Small) / 10) / 2, 0.785);
                    hTWindow.HalconWindow.DispCross(Row1_Rectangle_Small.D - rowOffset, Column2_Rectangle_Small.D - columnOffset, ((Row2_Rectangle_Small - Row1_Rectangle_Small) / 10 + (Column2_Rectangle_Small - Column1_Rectangle_Small) / 10) / 2, 0.785);
                    hTWindow.HalconWindow.DispCross(Row2_Rectangle_Small.D - rowOffset, Column1_Rectangle_Small.D - columnOffset, ((Row2_Rectangle_Small - Row1_Rectangle_Small) / 10 + (Column2_Rectangle_Small - Column1_Rectangle_Small) / 10) / 2, 0.785);
                    hTWindow.HalconWindow.DispCross(Row2_Rectangle_Small.D - rowOffset, Column2_Rectangle_Small.D - columnOffset, ((Row2_Rectangle_Small - Row1_Rectangle_Small) / 10 + (Column2_Rectangle_Small - Column1_Rectangle_Small) / 10) / 2, 0.785);
                    return;

                case RegionType.Rectangle2:
                    return;

                case RegionType.Circle:
                    return;

                case RegionType.Ellipse:
                    return;

                default:
                    return;
            }
        }

        private void RefreshWindow()
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (Image != null && Image.IsInitialized())
            {
                DisplayImage(Image);
            }
            if (Region != null && Region.IsInitialized())
            {
                InitialHWindow();
                if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.IsStopPickUp == true && wireAddAutoRegion.WireParameter.IsStartPickUp == true)
                {
                    //MessageBox.Show("拾取功能冲突，请确定唯一的拾取对象！");
                    //return;
                    //wireAddAutoRegion.GetClickDownPointsFromStartBall();
                    //wireAddAutoRegion.GetClickDownPointsFromStopBall();
                    //wireAddAutoRegion.DispalyGroupsStartandStopRegions();
                    wireAddAutoRegion.UpdateModelIndexChange();

                }
                else
                {
                    hTWindow.HalconWindow.DispObj(Region);
                }
                
            }
        }

        private void UpdateWindow()
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;

            hTWindow.HalconWindow.ClearWindow();
            if (Image != null && Image.IsInitialized())
            {
                //DisplayImage(Image);
                HOperatorSet.DispObj(Image, hTWindow.HalconWindow);
            }
            if (Region != null && Region.IsInitialized())
            {
                InitialHWindow();
                hTWindow.HalconWindow.DispObj(Region);
            }
        }

        public void DispImageFitWindow(HWindowControlWPF htWindow)//
        {
            if (Image != null && Image.IsInitialized())
            {
                HOperatorSet.GetImageSize(Image, out imageWidth, out imageHeight);
                htWindow.HalconWindow.SetPart(0, 0, imageHeight - 1, imageWidth - 1);
                Image.DispObj(htWindow.HalconWindow);
            }
        }

        private void DisplayImage(HObject image, bool isZoom = false)
        {
            if (image == null) return;
            hTWindow.HalconWindow.SetPaint(new HTuple("default"));

            HOperatorSet.GetImageSize(image, out imageWidth, out imageHeight);
            if (isZoom)
            {
                if (zoomBeginRow == 0
                 && zoomBeginColumn == 0
                 && zoomEndRow == 0
                 && zoomEndColumn == 0)
                {
                    zoomBeginRow = 0;
                    zoomBeginColumn = 0;
                    zoomEndRow = imageHeight - 1;
                    zoomEndColumn = imageWidth - 1;
                }
                hTWindow.HalconWindow.SetPart(zoomBeginRow, zoomBeginColumn, zoomEndRow, zoomEndColumn);
            }
            else
            {
                hTWindow.HalconWindow.SetPart(0, 0, imageHeight - 1, imageWidth - 1);
                zoomBeginRow = 0;
                zoomBeginColumn = 0;
                zoomEndRow = imageHeight - 1;
                zoomEndColumn = imageWidth - 1;
            }
            hTWindow.HalconWindow.DispObj(image);
            imageSize = $"{imageWidth}X{imageHeight}";
        }

        public void Display(HObject image, bool clearRegion = false)
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            this.Image = image;
            if (image == null || !image.IsInitialized())
            {
                Region = null;
                return;
            }
            DisplayImage(image);
            if (clearRegion)
            {
                this.Region = null;
                return;
            }
            if (Region != null && Region.IsInitialized())
            {
                InitialHWindow();
                hTWindow.HalconWindow.DispObj(Region);
            }
        }

        public void DisplaySingleRegion(HObject region, HObject image = null, string color = "")//看
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (image != null && image.IsInitialized())
            {
                this.Image = image;
                DisplayImage(image);
            }
            else
            {
                DisplayImage(Image, true);
            }
            this.Region = region;
            if (region != null && region.IsInitialized())
            {
                InitialHWindow(color);
                hTWindow.HalconWindow.DispObj(Region);
            }
        }

        public void DisplaySingleRegionSetDraw(HObject region, HObject image = null)
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (image != null && image.IsInitialized())
            {
                this.Image = image;
                DisplayImage(image);
            }
            else
            {
                DisplayImage(Image, true);
            }
            this.Region = region;
            if (region != null && region.IsInitialized())
            {
                hTWindow.HalconWindow.DispObj(Region);
            }
        }

        public void DisplaySingleRegionColor(HObject region, string color = "")
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            this.Region = region;
            if (region != null && region.IsInitialized())
            {
                InitialHWindow(color);
                hTWindow.HalconWindow.DispObj(Region);
            }
        }

        public void DisplaySingleRegionForCutOut(IEnumerable<UserRegion> userRegions, UserRegion userRegion, int idx, HObject image = null, string color = "")//
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (image != null && image.IsInitialized())
            {
                this.Image = image;
                DisplayImage(image);
            }
            else
            {
                DisplayImage(Image, true);
            }
            HObject DieRegionsRemain = null;
            HOperatorSet.GenEmptyObj(out DieRegionsRemain);
            IEnumerable<HObject> DieRegions;

            DieRegions = userRegions.Where(r => r.IsEnable).Where(r => r.Index != idx).Select(r => r.DisplayRegion);
            DieRegionsRemain = Algorithm.Region.ConcatRegion(DieRegions);

            this.Region = userRegion.DisplayRegion;
            if (userRegion.DisplayRegion != null && userRegion.DisplayRegion.IsInitialized())
            {
                InitialHWindow(color);
                hTWindow.HalconWindow.DispObj(userRegion.DisplayRegion);
            }

            this.Region = DieRegionsRemain;
            if (DieRegionsRemain != null && DieRegionsRemain.IsInitialized())
            {
                InitialHWindow();
                hTWindow.HalconWindow.DispObj(DieRegionsRemain);
            }
        }

        public void DisplayMultiRegion(IEnumerable<HObject> regions, HObject image = null, string color = "")
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (image != null && image.IsInitialized())
            {
                this.Image = image;
                DisplayImage(image);
            }
            else
            {
                DisplayImage(this.Image, true);
            }
            if (regions == null)
            {
                Region = null;
                return;
            }
            HObject concatRegion = null;
            HOperatorSet.GenEmptyObj(out concatRegion);
            foreach (var region in regions)
            {
                if (region != null && region.IsInitialized())
                {
                    HOperatorSet.ConcatObj(concatRegion, region, out concatRegion);
                }
            }
            this.Region = concatRegion;
            InitialHWindow(color);
            hTWindow.HalconWindow.DispObj(this.Region);
        }

        public void DisplayMultiRegionLine(IEnumerable<HObject> regions, HObject region, HObject image = null, string color = "")//
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (image != null && image.IsInitialized())
            {
                this.Image = image;
                DisplayImage(image);
            }
            else
            {
                DisplayImage(Image, true);
            }
            if (regions == null || region == null)
            {
                Region = null;
                return;
            }
            HObject concatRegion = null;
            HOperatorSet.GenEmptyObj(out concatRegion);
            foreach (var _region in regions)
            {
                if (_region != null && _region.IsInitialized())
                {
                    HOperatorSet.ConcatObj(concatRegion, _region, out concatRegion);
                }
            }
            HOperatorSet.ConcatObj(concatRegion, region, out concatRegion);

            this.Region = concatRegion;
            InitialHWindow(color);
            hTWindow.HalconWindow.DispObj(Region);
        }

        public void DisplayMultiRegion(IEnumerable<HObject> regions1, IEnumerable<HObject> regions2, HObject image = null, string color = "")
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (image != null && image.IsInitialized())
            {
                this.Image = image;
                DisplayImage(image);
            }
            else
            {
                DisplayImage(Image, true);
            }
            HObject concatRegion = null;
            HOperatorSet.GenEmptyObj(out concatRegion);
            foreach (var region in regions1)
            {
                if (region != null && region.IsInitialized())
                {
                    HOperatorSet.ConcatObj(concatRegion, region, out concatRegion);
                }
            }
            foreach (var region in regions2)
            {
                if (region != null && region.IsInitialized())
                {
                    HOperatorSet.ConcatObj(concatRegion, region, out concatRegion);
                }
            }
            this.Region = concatRegion;
            InitialHWindow(color);
            hTWindow.HalconWindow.DispObj(Region);
        }

        public void DisplayMultiRegionTwo(HObject region1, HObject region2, HObject image = null, string color = "")//
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (image != null && image.IsInitialized())
            {
                this.Image = image;
                DisplayImage(image);
            }
            else
            {
                DisplayImage(Image, true);
            }
            HObject concatRegion = null;
            HOperatorSet.GenEmptyObj(out concatRegion);
            if (region1 != null && region1.IsInitialized())
            {
                HOperatorSet.ConcatObj(concatRegion, region1, out concatRegion);
            }
            if (region2 != null && region2.IsInitialized())
            {
                HOperatorSet.ConcatObj(concatRegion, region2, out concatRegion);
            }
            this.Region = concatRegion;
            InitialHWindow(color);
            hTWindow.HalconWindow.DispObj(Region);
        }

        public void DisplayMultiRegion(HObject region1, IEnumerable<HObject> regions2, HObject image = null, string color = "")
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (image != null && image.IsInitialized())
            {
                this.Image = image;
                DisplayImage(image);
            }
            else
            {
                DisplayImage(Image, true);
            }
            HObject concatRegion = null;
            HOperatorSet.GenEmptyObj(out concatRegion);
            if (region1 != null && region1.IsInitialized())
            {
                HOperatorSet.ConcatObj(concatRegion, region1, out concatRegion);
            }
            foreach (var region in regions2)
            {
                if (region != null && region.IsInitialized())
                {
                    HOperatorSet.ConcatObj(concatRegion, region, out concatRegion);
                }
            }
            this.Region = concatRegion;
            InitialHWindow(color);
            hTWindow.HalconWindow.DispObj(Region);
        }

        public void DisplayMultiRegion(params HObject[] regions)//******
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (Image == null || !Image.IsInitialized())
            {
                Region = null;
                return;
            }
            DisplayImage(Image);
            HOperatorSet.GenEmptyObj(out HObject concatRegion);
            foreach (var region in regions)
            {
                if (region != null && region.IsInitialized())
                {
                    HOperatorSet.ConcatObj(concatRegion, region, out concatRegion);
                }
            }
            this.Region = concatRegion;
            InitialHWindow();
            hTWindow.HalconWindow.DispObj(this.Region);
        }

        public void DisplayMultiRegionWithIndex(ObservableCollection<UserRegion> userRegions, string color = "")
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            if (userRegions == null || userRegions.Count == 0) return;
            hTWindow.HalconWindow.ClearWindow();
            if (Image == null || !Image.IsInitialized())
            {
                Region = null;
                return;
            }
            DisplayImage(Image);
            HOperatorSet.GenEmptyObj(out HObject concatRegion);
            foreach (var region in userRegions)
            {
                if (region.IsEnable == true)
                {
                    if (region.DisplayRegion != null && region.DisplayRegion.IsInitialized())
                    {
                        HOperatorSet.ConcatObj(concatRegion, region.DisplayRegion, out concatRegion);
                    }
                    HOperatorSet.AreaCenter(region.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                    HOperatorSet.SetTposition(hTWindow.HalconWindow, row_tmp, col_tmp);
                    HOperatorSet.TupleString(region.Index, "0", out StrIndex);
                }
            }
            this.Region = concatRegion;
            InitialHWindow();
            hTWindow.HalconWindow.DispObj(this.Region);
            RefreshWindow();
        }

        public void DisplayMultiRegionSetDisplay(params HObject[] regions)//******
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (Image == null || !Image.IsInitialized())
            {
                Region = null;
                return;
            }
            DisplayImage(Image);
            HOperatorSet.GenEmptyObj(out HObject concatRegion);
            foreach (var region in regions)
            {
                if (region != null && region.IsInitialized())
                {
                    HOperatorSet.ConcatObj(concatRegion, region, out concatRegion);
                }
            }
            this.Region = concatRegion;
            hTWindow.HalconWindow.DispObj(this.Region);
        }

        private Timer refreshTimer = null;

        /// <summary>
        /// 为了避免改变控件大小时频繁刷新图像，使用计时器判断：如果在等待周期内持续触发，则刷新等待时间。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HTWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (refreshTimer != null)
            {
                refreshTimer.Change(500, Timeout.Infinite);
            }
            else
            {
                refreshTimer = new Timer(new TimerCallback(o =>
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        RefreshWindow();
                    }));
                    refreshTimer.Dispose();
                    refreshTimer = null;
                }), null, 500, Timeout.Infinite);
            }
        }

        private void HTWindow_HMouseWheel(object sender, HMouseEventArgsWPF e)
        {
            if (Image != null && Image.IsInitialized())
            {
                double mPositionRow;
                double mPositionColumn;
                int currentBeginRow;
                int currentBeginColumn;
                int currentEndRow;
                int currentEndColumn;

                try
                {
                    hTWindow.HalconWindow.GetMpositionSubPix(out mPositionRow, out mPositionColumn, out int button);
                    hTWindow.HalconWindow.GetPart(out currentBeginRow, out currentBeginColumn, out currentEndRow, out currentEndColumn);
                }
                catch (Exception ex)
                {
                    tb_Status.Text = ex.Message;
                    return;
                }
                if (e.Delta > 0)
                {
                    //1109 add for pickup sort funcion
                    if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.StartSortMethod == 7)
                    {
                        int pick_select_type;
                        pick_select_type = 2;

                        bool start_pint_pick = false;
                        start_pint_pick = wireAddAutoRegion.GetClickDownPointsFromStartBall_for_sort(pick_select_type, mPositionRow, mPositionColumn); //序号增加
                        if (start_pint_pick == true)
                        {
                            return;
                        }

                        bool stop_pint_pick = false;
                        stop_pint_pick = wireAddAutoRegion.GetClickDownPointsFromStopBall_for_sort(pick_select_type, mPositionRow, mPositionColumn); //序号增加
                        if (stop_pint_pick == true)
                        {
                            return;
                        }

                    }

                    zoomBeginRow = (int)(currentBeginRow + (mPositionRow - currentBeginRow) * 0.3);
                    zoomBeginColumn = (int)(currentBeginColumn + (mPositionColumn - currentBeginColumn) * 0.3);
                    zoomEndRow = (int)(currentEndRow - (currentEndRow - mPositionRow) * 0.3);
                    zoomEndColumn = (int)(currentEndColumn - (currentEndColumn - mPositionColumn) * 0.3);
                }
                else
                {
                    //1109 add for pickup sort funcion
                    if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.StartSortMethod == 7)
                    {
                        int pick_select_type;
                        pick_select_type = 3;

                        bool start_pint_pick = false;
                        start_pint_pick = wireAddAutoRegion.GetClickDownPointsFromStartBall_for_sort(pick_select_type, mPositionRow, mPositionColumn); //序号增加
                        if (start_pint_pick == true)
                        {
                            return;
                        }

                        bool stop_pint_pick = false;
                        stop_pint_pick = wireAddAutoRegion.GetClickDownPointsFromStopBall_for_sort(pick_select_type, mPositionRow, mPositionColumn); //序号增加
                        if (stop_pint_pick == true)
                        {
                            return;
                        }
                    }

                    zoomBeginRow = (int)(mPositionRow - (mPositionRow - currentBeginRow) / 0.7);
                    zoomBeginColumn = (int)(mPositionColumn - (mPositionColumn - currentBeginColumn) / 0.7);
                    zoomEndRow = (int)(mPositionRow + (currentEndRow - mPositionRow) / 0.7);
                    zoomEndColumn = (int)(mPositionColumn + (currentEndColumn - mPositionColumn) / 0.7);
                }
                try
                {
                    double width = hTWindow.Width;
                    double height = hTWindow.Height;
                    bool flag = zoomBeginRow >= imageHeight || zoomEndRow <= 0 || zoomBeginColumn >= imageWidth || zoomEndColumn < 0;
                    bool flag2 = zoomEndRow - zoomBeginRow > imageHeight * 20 || zoomEndColumn - zoomBeginColumn > imageWidth * 20;
                    bool flag3 = height / (zoomEndRow - zoomBeginRow) > 500 || width / (zoomEndColumn - zoomBeginColumn) > 500;
                    if (!flag && !flag2)
                    {
                        if (!flag3)
                        {
                            hTWindow.HalconWindow.ClearWindow();
                            hTWindow.HalconWindow.SetPaint(new HTuple("default"));
                            hTWindow.HalconWindow.SetPart(zoomBeginRow, zoomBeginColumn, zoomEndRow, zoomEndColumn);
                            hTWindow.HalconWindow.DispObj(Image);
                            if (Region != null && Region.IsInitialized())
                            {
                                //InitialHWindow();
                                hTWindow.HalconWindow.DispObj(Region);

                                //1012
                                if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.IsStartPickUp == true && wireAddAutoRegion.WireParameter.IsStopPickUp == false)
                                {
                                    //wireAddAutoRegion.GetClickDownPointsFromStartBall();
                                    wireAddAutoRegion.DispalyGroupsStartRegions();

                                }
                                else if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.IsStopPickUp == true && wireAddAutoRegion.WireParameter.IsStartPickUp == false)
                                {
                                    //wireAddAutoRegion.GetClickDownPointsFromStopBall();
                                    wireAddAutoRegion.DispalyGroupsStopRegions();

                                }
                                else if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.IsStopPickUp == true && wireAddAutoRegion.WireParameter.IsStartPickUp == true)
                                {
                                    //MessageBox.Show("拾取功能冲突，请确定唯一的拾取对象！");
                                    //return;
                                    //wireAddAutoRegion.GetClickDownPointsFromStartBall();
                                    //wireAddAutoRegion.GetClickDownPointsFromStopBall();
                                    //wireAddAutoRegion.DispalyGroupsStartandStopRegions();
                                    wireAddAutoRegion.UpdateModelIndexChange();

                                }
                                else
                                {
                                    hTWindow.HalconWindow.DispObj(Region);
                                }//1012

                            }

                            // 0118 lw
                            if (inspectNode != null && inspectNode.IniParameters.IsInspectNodeVerify == true)
                            {
                                inspectNode.DisplayNodeResultRegion();
                            }

                            if (goldenModelInspectVerify != null && goldenModelInspectVerify.GoldenModelParameter.IsInspectVerify == true)
                            {
                                goldenModelInspectVerify.DisplayIcResultRegion();
                            }

                            if (createAroundBondRegionModel != null)
                            {
                                createAroundBondRegionModel.DisplayAroundBondResultRegion();
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    DispImageFitWindow(hTWindow);
                    tb_Status.Text = "";
                }
            }
        }

        private void HTWindow_HMouseMove(object sender, HMouseEventArgsWPF e)
        {
            try
            {
                if (Image != null && Image.IsInitialized())
                {
                    HTuple channels;
                    HOperatorSet.CountChannels(Image, out channels);
                    //double mPositionRow;
                    //double mPositionColumn;
                    hTWindow.HalconWindow.GetMpositionSubPix(out mPositionRow, out mPositionColumn, out int button);

                    if (!this.isMeasureMode)
                    {
                        this.hTWindow.Cursor = Cursors.Arrow;
                    }
                    else
                    {
                        this.hTWindow.Cursor = Cursors.Cross;//
                        if (measureFristClickFlag)
                        {
                            if ((mPositionRow < this.imageHeight.D && mPositionRow > 0) && (mPositionColumn < this.imageWidth.D && mPositionColumn > 0))
                            {
                                // this.mCtrl_HWindow.HalconWindow.ClearWindow();
                                this.hTWindow.HalconWindow.SetPaint(new HTuple("default"));
                                this.hTWindow.HalconWindow.SetPart(this.zoomBeginRow, this.zoomBeginColumn, this.zoomEndRow, this.zoomEndColumn);
                                this.hTWindow.HalconWindow.DispObj(this.Image);
                                if (this.Region != null)
                                {
                                    if (this.Region.IsInitialized())
                                    {
                                        this.hTWindow.HalconWindow.DispObj(this.Region);
                                    }
                                }
                                this.hTWindow.HalconWindow.SetColor("red");
                                this.hTWindow.HalconWindow.DispLine(measureStartPoint.X, measureStartPoint.Y, mPositionRow, mPositionColumn);
                            }
                        }
                    }
                    //////////////////////
                    HTuple px, py;
                    px = mPositionRow;
                    py = mPositionColumn;
                    HTuple gray_light = null;
                    HTuple gray_dark = null;
                    HTuple qx = null;
                    HTuple qy = null;
                    if (Ic_mat != null && LightImage!=null && DarkImage!=null)
                    {
                        HOperatorSet.AffineTransPoint2d(Ic_mat, px, py, out qx, out qy);

                        HOperatorSet.GetGrayval(LightImage, qx, qy, out gray_light);
                        HOperatorSet.GetGrayval(DarkImage, qx, qy, out gray_dark);
                    }

                    StringBuilder sb = new StringBuilder();
                    if ((wireAddAutoRegion != null) 
                        && (wireAddAutoRegion.WireParameter.IsDrawStartVirtualBall == true || wireAddAutoRegion.WireParameter.IsDrawStopVirtualBall == true))
                    {
                        if (isMultiDraw == true)
                        {
                            sb.Append($"【连续画框模式】 ");
                        }
                        else
                        {
                            sb.Append($"【单独画框模式】 ");
                        }
                    }
                    sb.Append($"{imageSize}  X: {mPositionRow:0000.0}, Y: {mPositionColumn:0000.0}  ");
                    bool flag1 = mPositionColumn < 0.0 || mPositionColumn >= imageWidth;
                    bool flag2 = mPositionRow < 0.0 || mPositionRow >= imageHeight;
                    if (!flag1 && !flag2)
                    {
                        if (channels == 1)
                        {
                            try
                            {
                                HOperatorSet.GetGrayval(Image, mPositionRow, mPositionColumn, out HTuple grayval);
                                sb.Append($"Val: {grayval.D:000.0}");

                                if (Ic_mat != null && LightImage != null && DarkImage != null)
                                {
                                    sb.Append($"  M-X: {qx.D:0000.0}, M-Y: {qy.D:0000.0}  ");
                                    sb.Append($"--L-Val: {gray_light.D:000.0}");
                                    sb.Append($"--D-Val: {gray_dark.D:000.0}");
                                }
                            }
                            catch (HalconException ex)
                            {
                                sb.Append(ex.Message);
                            }
                        }
                        else if (channels == 3) // 暂不改
                        {
                            try
                            {
                                HOperatorSet.AccessChannel(Image, out HObject channelImage_1, 1);
                                HOperatorSet.AccessChannel(Image, out HObject channelImage_2, 2);
                                HOperatorSet.AccessChannel(Image, out HObject channelImage_3, 3);
                                HOperatorSet.GetGrayval(channelImage_1, mPositionRow, mPositionColumn, out HTuple grayval_1);
                                HOperatorSet.GetGrayval(channelImage_2, mPositionRow, mPositionColumn, out HTuple grayval_2);
                                HOperatorSet.GetGrayval(channelImage_3, mPositionRow, mPositionColumn, out HTuple grayval_3);
                                channelImage_1.Dispose();
                                channelImage_2.Dispose();
                                channelImage_3.Dispose();
                                sb.Append($"Val: ({grayval_1.D:000.0}, {grayval_2.D:000.0}, {grayval_3.D:000.0})");

                                if (Ic_mat != null && LightImage != null && DarkImage != null)
                                {
                                    sb.Append($"  M-X: {qx.D:0000.0}, M-Y: {qy.D:0000.0}  ");
                                    sb.Append($"--L-Val: {gray_light.D:000.0}");
                                    sb.Append($"--D-Val: {gray_dark.D:000.0}");
                                }
                            }
                            catch (HalconException ex)
                            {
                                sb.Append(ex.Message);
                            }
                        }
                    }
                    tb_Status.Text = sb.ToString();
                }
            }
            catch { }
        }

        private void HTWindow_HMouseUp(object sender, HMouseEventArgsWPF e)
        {
            if (Image != null && Image.IsInitialized())
            {
                if (this.isMeasureMode)
                {
                    if (measureFristClickFlag == false)
                    {
                        measureStartPoint.X = (float)this.mPositionRow;
                        measureStartPoint.Y = (float)this.mPositionColumn;
                        measureFristClickFlag = true;
                    }
                    else
                    {
                        measureEndPoint.X = (float)this.mPositionRow;
                        measureEndPoint.Y = (float)this.mPositionColumn;
                        try
                        {
                            HTuple distance = null;
                            HOperatorSet.DistancePp(measureStartPoint.X, measureStartPoint.Y, measureEndPoint.X, measureEndPoint.Y, out distance);
                            if (umPerPix == -1)
                            {
                                System.Windows.Forms.MessageBox.Show(string.Format("距离: {0}pix", distance.D.ToString("f0")), "距离测量", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show(string.Format("距离:{0}pix({1}um)", distance.D.ToString("f2"), (distance.D * umPerPix).ToString("f2")), "Info", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                            }

                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show(string.Format("距离失败!详细信息:{0}", ex.Message), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        }

                        measureFristClickFlag = false;
                        this.hTWindow.Cursor = System.Windows.Input.Cursors.Arrow;
                        this.isMeasureMode = false;
                        this.hTWindow.HalconWindow.SetPaint(new HTuple("default"));
                        this.hTWindow.HalconWindow.SetPart(zoomBeginRow, zoomBeginColumn, zoomEndRow, zoomEndColumn);
                        this.hTWindow.HalconWindow.DispObj(Image);
                        if (Region != null)
                        {
                            if (this.Region.IsInitialized())
                            {
                                this.hTWindow.HalconWindow.DispObj(this.Region);
                            }
                        }
                    }
                    //e.Button = System.Windows.Forms.MouseButtons.Left;
                }

                if (e.Button == MouseButton.Left && isMoveMode == true && !isMeasureMode)
                {
                    int row1, col1, row2, col2;
                    if (mPositionRow == 0 || mPositionColumn == 0)
                        return;
                    try
                    {
                        int tempNum = 0;
                        //hWindow.GetMposition(out mouse_X1, out mouse_Y1, out tempNum);

                        double dbRowMove, dbColMove;
                        dbRowMove = mPositionDownRow - mPositionRow;//计算光标在X轴拖动的距离
                        dbColMove = mPositionDownColumn - mPositionColumn;//计算光标在Y轴拖动的距离

                        hTWindow.HalconWindow.GetPart(out row1, out col1, out row2, out col2);//计算HWindow控件在当前状态下显示图像的位置
                        hTWindow.HalconWindow.SetPart((int)(row1 + dbRowMove), (int)(col1 + dbColMove), (int)(row2 + dbRowMove), (int)(col2 + dbColMove));//根据拖动距离调整HWindows控件显示图像的位置
                        HOperatorSet.ClearWindow(hTWindow.HalconWindow);
                        //HOperatorSet.DispObj(Image, hTWindow.HalconWindow);
                        UpdateWindow();//刷新图像
                        isMoveMode = false;
                    }
                    catch (HalconException HDevExpDefaultException)
                    {
                    }
                }
                else
                {
                    //if (MouseClickEvent != null)
                    //{
                    //if (isRightClick)
                    //MouseClickEvent(new PointF((float)mPositionRow, (float)mposition_col));
                    //}
                }
            }

        }

        private void HTWindow_HMouseDown(object sender, HMouseEventArgsWPF e)
        {
            mPositionDownRow = mPositionRow;
            mPositionDownColumn = mPositionColumn;

            //1109 add for pickup sort funcion
            int pick_select_type;
            pick_select_type = 0;
            if (e.Button == MouseButton.Left && isMoveMode == false && !isMeasureMode) // 点击序号从指定序号（旋转起始点）开始递增
            {
                pick_select_type = 1;
            }
            if (e.Button == MouseButton.Right && isMoveMode == false && !isMeasureMode)  // 右键点击序号增加
            {
                pick_select_type = 2;
            }
            if (e.Button == MouseButton.Middle && isMoveMode == false && !isMeasureMode)  // 中键点击序号减少
            {
                pick_select_type = 3;
            }

            if (bondMatchAutoRegions != null)
            {
                bondMatchAutoRegions.GetClickDownPoints();
            }

            if (createAutoBondMeasureModel != null)
            {
                createAutoBondMeasureModel.GetClickDownPoints();
            }
            //2021-01-05 add by wj
            if (bondMeasureVerify != null && bondMeasureVerify.BondMeasureParameter.IsPadRegionPickUp == true)
            {
                bondMeasureVerify.GetClickDownPointsFromPadRegion();
            }
            //add by wj 2021-01-11
            if (bondMatchVerify != null && bondMatchVerify.BondWireParameter.IsVerifyRegionPickUp == true)
            {
                bondMatchVerify.GetClickDownPointsFromBond2UserRegion();
            }

            // 虚拟焊点连续画圆 add_lw
            if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.IsDrawStartVirtualBall == true && isMultiDraw == true)
            {
                UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(null,
                                                                                 RegionType.Circle,
                                                                                 mPositionDownRow, mPositionDownColumn,
                                                                                 wireAddAutoRegion.WireParameter.GenStartBallSize, 0,
                                                                                 wireAddAutoRegion.WireParameter.DieImageRowOffset,
                                                                                 wireAddAutoRegion.WireParameter.DieImageColumnOffset,
                                                                                 0);
                if (userRegion_Circle == null) return;
                userRegion_Circle.Index = wireAddAutoRegion.StartBallAutoUserRegion.Count + 1;
                userRegion_Circle.Index_ini = wireAddAutoRegion.StartBallAutoUserRegion.Count + 1;
                wireAddAutoRegion.StartBallAutoUserRegion.Add(userRegion_Circle);

                //有模板线，但没有当前模板线，重置为第一条模板线
                if (wireAddAutoRegion.StartBallAutoUserRegion[wireAddAutoRegion.StartBallAutoUserRegion.Count-1].CurrentModelGroup == null && wireAddAutoRegion.ModelGroups.Count > 0)
                {
                    wireAddAutoRegion.StartBallAutoUserRegion[wireAddAutoRegion.StartBallAutoUserRegion.Count - 1].ModelGroups = wireAddAutoRegion.ModelGroups;
                    wireAddAutoRegion.StartBallAutoUserRegion[wireAddAutoRegion.StartBallAutoUserRegion.Count - 1].CurrentModelGroup = wireAddAutoRegion.ModelGroups.ElementAt(0);
                }

                //初始化 WireParameter.WireRegModelType
                if (wireAddAutoRegion.WireParameter.WireRegModelType.Count() >= 0 || wireAddAutoRegion.WireParameter.WireRegModelType.Count() != wireAddAutoRegion.StartBallAutoUserRegion.Count())
                {
                    //initilize wiremodeltype
                    Array.Clear(wireAddAutoRegion.WireParameter.WireRegModelType, 0, wireAddAutoRegion.WireParameter.WireRegModelType.Length);
                    wireAddAutoRegion.WireParameter.WireRegModelType = new int[wireAddAutoRegion.StartBallAutoUserRegion.Count];
                    for (int i = 0; i < wireAddAutoRegion.StartBallAutoUserRegion.Count(); i++)
                    {
                        if (wireAddAutoRegion.ModelGroups.Count > 0 && wireAddAutoRegion.StartBallAutoUserRegion[i].CurrentModelGroup != null)
                        {
                            wireAddAutoRegion.WireParameter.WireRegModelType[i] = wireAddAutoRegion.StartBallAutoUserRegion[i].CurrentModelGroup.Index;
                        }
                        else
                        {
                            wireAddAutoRegion.WireParameter.WireRegModelType[i] = 0;
                        }

                    }

                }

                wireAddAutoRegion.DispalyGroupsStartRegions(true);
            }

            if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.IsDrawStopVirtualBall == true && isMultiDraw == true)
            {
                UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(null,
                                                                                 RegionType.Circle,
                                                                                 mPositionDownRow, mPositionDownColumn,
                                                                                 wireAddAutoRegion.WireParameter.GenStopBallSize, 0,
                                                                                 wireAddAutoRegion.WireParameter.DieImageRowOffset,
                                                                                 wireAddAutoRegion.WireParameter.DieImageColumnOffset,
                                                                                 0);
                if (userRegion_Circle == null) return;
                userRegion_Circle.Index = wireAddAutoRegion.StopBallAutoUserRegion.Count + 1;
                userRegion_Circle.Index_ini = wireAddAutoRegion.StopBallAutoUserRegion.Count + 1;
                wireAddAutoRegion.StopBallAutoUserRegion.Add(userRegion_Circle);

                wireAddAutoRegion.DispalyGroupsStopRegions(true);
            }

            if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.IsStartPickUp == true && wireAddAutoRegion.WireParameter.IsWirePickUp == false)
            {
                //金线起始焊点拾取功能+1109
                wireAddAutoRegion.GetClickDownPointsFromStartBall(pick_select_type);               
            }
            if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.IsStopPickUp == true && wireAddAutoRegion.WireParameter.IsWirePickUp == false)
            {
                //金线结束焊点拾取功能+1109
                wireAddAutoRegion.GetClickDownPointsFromStopBall(pick_select_type);
            }
            if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.IsStopPickUp == false && wireAddAutoRegion.WireParameter.IsStartPickUp == false
               && wireAddAutoRegion.WireParameter.IsWirePickUp == true)
            {
                //金线检测区域拾取功能
                wireAddAutoRegion.GetClickDownPointsFromLineModelRegion();
            }
            if (wireAddAutoRegion != null && ((wireAddAutoRegion.WireParameter.IsStopPickUp == true && wireAddAutoRegion.WireParameter.IsWirePickUp == true)
                   || (wireAddAutoRegion.WireParameter.IsWirePickUp == true && wireAddAutoRegion.WireParameter.IsStartPickUp == true)))
            {
                MessageBox.Show("拾取功能冲突，请确定焊点区域与金线模板区域拾取对象！");
                return;
            }
            //拾取选择制作金线模板的序号
            if (wireAddAutoRegion != null && wireAddAutoRegion.WireParameter.IsWireRegionPickUp == true)
            {
                wireAddAutoRegion.GetClickDownPointsFromStartStopLine();
            }
        }

        public void ClearSelection()
        {
            RegionType = RegionType.Null;
        }

        public void Dispose()
        {
            this.Region?.Dispose();
            this.Image?.Dispose();
            if (refreshTimer != null)
            {
                refreshTimer.Dispose();
            }
            hTWindow.HMouseWheel -= HTWindow_HMouseWheel;
            hTWindow.HMouseMove -= HTWindow_HMouseMove;
            hTWindow.HMouseUp -= HTWindow_HMouseUp;//
            hTWindow.SizeChanged -= HTWindow_SizeChanged;
            if (HTWindow.HalconWindow.Handle != (IntPtr)0xffffffffffffffff || isdeleted == true)
            {
                if (isdeleted == true)
                {
                    MessageBox.Show("窗口释放");
                }
                
                hTWindow.Dispose();
            }
            dockPanel.Children.Clear();
        }


        public void SET_Light_Dark_image(HObject input_Light_Image, HObject input_Dark_Image, HTuple Input_Ic_mat)
        {
            LightImage = input_Light_Image;
            DarkImage = input_Dark_Image;
            HTuple inverse_mat;
            HOperatorSet.HomMat2dInvert(Input_Ic_mat, out inverse_mat);
            Ic_mat = inverse_mat;
        }

        //鼠标画区域
        private void Btn_Region_Click(object sender, RoutedEventArgs e)
        {
            if (isRightClick)
            {
                isRightClick = false;
                InitialHWindow();
                hTWindow.Focus();
                try
                {
                    HOperatorSet.DrawRegion(out region_Region, hTWindow.HalconWindow);
                    RegionType = RegionType.Region;
                }
                catch { }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void Btn_Measure_Click(object sender, RoutedEventArgs e)//
        {
            isMeasureMode = true;//
            try
            {
                if (isRightClick && isMeasureMode)
                {
                    this.hTWindow.Focus();
                    InitialHWindow();
                    this.isRightClick = false;
                    //this.regionType = "";
                    HOperatorSet.DrawLine(hTWindow.HalconWindow, out row1_Line, out column1_Line, out row2_Line, out column2_Line);
                    this.isRightClick = true;
                    this.isMeasureMode = false;
                }
            }
            catch (Exception)
            {
                this.isRightClick = true;
            }
        }

        private void Btn_Move_Click(object sender, RoutedEventArgs e)
        {
            isMoveMode = true;//
        }

        //鼠标拾取
        private void Btn_DrawMul_Click(object sender, RoutedEventArgs e)
        {
            // 1103 add_lw
            isMultiDraw = true;
            /*
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (Image != null && Image.IsInitialized())
            {
                DisplayImage(Image, true);  // 0817 ture is set for retain zoom state 
            }
            //if (Region != null && Region.IsInitialized())
            //{
            //    InitialHWindow();
            //    hTWindow.HalconWindow.DispObj(Region);
            //}

            if (isMultiDraw)
            {
                isMultiDraw = true;
            }
            else
            {
                isMultiDraw = true;
            }

            isSelectState = false;

            //isMultiDraw = true;
            //MultiDrawRegions.Clear();
            DisplayMultiRegion(MultiDrawRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
            //RegionType = RegionType.Null;
            */
        }

        private void Btn_Drawsingle_Click(object sender, RoutedEventArgs e)
        {
            // 1103 add_lw
            isMultiDraw = false;
            /*
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (Image != null && Image.IsInitialized())
            {
                DisplayImage(Image, true);  // 0817 ture is set for retain zoom state 
            }
            //if (Region != null && Region.IsInitialized())
            //{
            //    InitialHWindow();
            //    hTWindow.HalconWindow.DispObj(Region);
            //}
            if (isMultiDraw)
            {
                isMultiDraw = false;
            }
            else
            {
                isMultiDraw = false;
            }
            isSelectState = true;

            //isMultiDraw = true;
            MultiDrawRegions.Clear();
            DisplayMultiRegion(MultiDrawRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
            //RegionType = RegionType.Null;
            */
        }

        private void Btn_Select_Click(object sender, RoutedEventArgs e)
        {

            if (isSelectState)
            {
                //btn_Drawsingle.IsEnabled = false;
                btn_Select.Opacity = 0.4;
                isSelectState = false;
                isBoxSelectState = false;
            }
            else
            {
                //btn_Drawsingle.IsEnabled = true;
                btn_Select.Opacity = 1;
                isSelectState = true;
                isBoxSelectState = true;
            }

        }

        private void Btn_DeleteLastOne_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (Image != null && Image.IsInitialized())
            {
                DisplayImage(Image, true);  // 0817 ture is set for retain zoom state 
            }
            //if (Region != null && Region.IsInitialized())
            //{
            //    InitialHWindow();
            //    hTWindow.HalconWindow.DispObj(Region);
            //}

            //delete the last region
            if (MultiDrawRegions.Count() > 0)
            {
                MultiDrawRegions.RemoveAt(MultiDrawRegions.Count() - 1);
            }

            DisplayMultiRegion(MultiDrawRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
            */
        }

        private void Btn_DeleteSels_Click(object sender, RoutedEventArgs e)
        {
            if (wireAddAutoRegion.WireParameter.IsStartPickUp == true)
            {
                wireAddAutoRegion.StartBallAutoUserRegion.ToList().ForEach(r => r.IsSelected = false);
            }
            if (wireAddAutoRegion.WireParameter.IsStopPickUp == true)
            {
                wireAddAutoRegion.StopBallAutoUserRegion.ToList().ForEach(r => r.IsSelected = false);
            }
            wireAddAutoRegion.UpdateModelIndexChange();
            /*
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (Image != null && Image.IsInitialized())
            {
                DisplayImage(Image, true);  // 0817 ture is set for retain zoom state 
            }
            //if (Region != null && Region.IsInitialized())
            //{
            //    InitialHWindow();
            //    hTWindow.HalconWindow.DispObj(Region);
            //}

            // delete the selected regions
            for (int i = 0; i < MultiDrawRegions.Count; i++)
            {
                if (MultiDrawRegions[i].IsSelected)
                {
                    MultiDrawRegions.RemoveAt(i);
                    i--;
                }
                else
                {
                    MultiDrawRegions[i].Index = i + 1;
                }
            }

            DisplayMultiRegion(MultiDrawRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
            */
        }

        private void Btn_DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (Image != null && Image.IsInitialized())
            {
                DisplayImage(Image, true);  // 0817 ture is set for retain zoom state 
            }
            //if (Region != null && Region.IsInitialized())
            //{
            //    InitialHWindow();
            //    hTWindow.HalconWindow.DispObj(Region);
            //}
            // delete the selected regions
            //for (int i = 0; i < MultiDrawRegions.Count; i++)
            //{
            //        MultiDrawRegions.RemoveAt(i);
            //        i--;
            //}
            MultiDrawRegions.Clear();

            DisplayMultiRegion(MultiDrawRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
            */
        }

        public void SET_MultiRegions(ObservableCollection<UserRegion> Input_DrawRegions)
        {
            MultiDrawRegions = Input_DrawRegions;
        }

        public ObservableCollection<UserRegion> GET_MultiRegions(out ObservableCollection<UserRegion> Output_DrawRegions)
        {

            Output_DrawRegions = MultiDrawRegions;
            return Output_DrawRegions;

        }

        public void CLEAR_MultiRegions()
        {

            //MultiDrawRegions.Clear();

        }

        public void DisplayMultiRegion_Index_Highlight(IEnumerable<HObject> regions, HTuple sel_index, string color = "", string color_sel = "", HObject image = null)
        {
            if (hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || isdeleted == true) return;
            hTWindow.HalconWindow.ClearWindow();
            if (image != null && image.IsInitialized())
            {
                this.Image = image;
                DisplayImage(image);
            }
            else
            {
                DisplayImage(this.Image, true);
            }
            if (regions == null)
            {
                Region = null;
                return;
            }
            HObject concatRegion = null;
            HOperatorSet.GenEmptyObj(out concatRegion);
            HObject concatRegion_sel = null;
            HObject concatRegion_all = null;
            HOperatorSet.GenEmptyObj(out concatRegion_sel);
            HOperatorSet.GenEmptyObj(out concatRegion_all);
            HTuple tmp_n = 1, tmp_n_sel = 0;
            HTuple n_sel = sel_index.TupleLength();

            InitialHWindow("red");
            foreach (var region in regions)
            {

                if (region != null && region.IsInitialized())
                {

                    if (tmp_n_sel < n_sel)
                    {
                        HTuple cond1 = tmp_n.TupleEqual(sel_index[tmp_n_sel]);
                        if (cond1)
                        {
                            HOperatorSet.ConcatObj(concatRegion_sel, region, out concatRegion_sel);
                            tmp_n_sel++;
                        }
                        else
                        {
                            HOperatorSet.ConcatObj(concatRegion, region, out concatRegion);
                        }

                    }
                    else
                    {
                        HOperatorSet.ConcatObj(concatRegion, region, out concatRegion);
                    }
                    HOperatorSet.ConcatObj(concatRegion_all, region, out concatRegion_all);

                    HTuple odd_n = tmp_n.TupleMod(2);
                    if (odd_n)
                    {
                        HOperatorSet.AreaCenter(region, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                        HOperatorSet.SetTposition(hTWindow.HalconWindow, row_tmp, col_tmp);
                        HOperatorSet.TupleString((tmp_n + 1) / 2, "0", out HTuple str_n);
                        HOperatorSet.WriteString(hTWindow.HalconWindow, str_n);
                    }

                    tmp_n++;
                }
            }
            this.Region = concatRegion_all;
            InitialHWindow(color);
            hTWindow.HalconWindow.DispObj(concatRegion);
            InitialHWindow(color_sel);
            hTWindow.HalconWindow.DispObj(concatRegion_sel);

            concatRegion.Dispose();
            concatRegion_sel.Dispose();
        }
    }
}
