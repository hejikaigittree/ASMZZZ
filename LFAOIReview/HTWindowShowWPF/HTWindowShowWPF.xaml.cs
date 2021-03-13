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

namespace LFAOIReview
{
    /// <summary>
    /// HTWindowShowWPF.xaml 的交互逻辑
    /// </summary>
    public partial class HTWindowShowWPF : UserControl
    {
        public HTWindowShowWPF()
        {
            InitializeComponent();
            hTWindow.HMouseWheel += HTWindow_HMouseWheel;
            hTWindow.HMouseDown += HTWindow_HMouseDown;
            hTWindow.HMouseUp += HTWindow_HMouseUp;
            hTWindow.HMouseMove += HTWindow_HMouseMove;
        }

        public HObject Region { get;  set; }

        public HObject Image { get;  set; }

        public HWindowControlWPF HTWindow => hTWindow;

        private HTuple imageWidth;

        private HTuple imageHeight;

        int mouse_X0, mouse_X1, mouse_Y0, mouse_Y1;//用来记录按下/抬起鼠标时的坐标位置

        private bool flag = false;  

        private int zoomBeginRow = 0;
        private int zoomBeginColumn = 0;
        private int zoomEndRow = 0;
        private int zoomEndColumn = 0;

        public void InitialHWindow(string color = "")
        {
            hTWindow.HalconWindow.SetDraw("margin");
            if (color.Equals(string.Empty))
            {
                hTWindow.HalconWindow.SetColor("yellow");
            }
            else
            {
                hTWindow.HalconWindow.SetColor(color);
            }
        }

        /// <summary>
        /// 初始化函数 第一张图显示会有问题
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Region"></param>
        /// <param name="color"></param>
        public void InitialHWindow(HObject Image,HObject Region = null,string color = "")
        {
            if (color.Equals(string.Empty))
            {
                hTWindow.HalconWindow.SetColor("yellow");
            }
            else
            {
                hTWindow.HalconWindow.SetColor(color);
            }


            if (this.Image!=null)
            {
                this.Image.Dispose();
            }
            this.Image = Image.CopyObj(1,-1);
            hTWindow.HalconWindow.DispObj(this.Image);


            if (this.Region !=null)
            {
                this.Region.Dispose();
            }
            if(Region !=null)
            {
                this.Region = Region.CopyObj(1, -1);
                hTWindow.HalconWindow.DispObj(this.Region);
            }

            HOperatorSet.GetImageSize(Image, out imageWidth, out imageHeight);
            hTWindow.HalconWindow.SetPart(0, 0, imageHeight - 1, imageWidth - 1);
            hTWindow.HalconWindow.SetDraw("margin");
     

        }

        public void SetImage(HObject image)
        {
            Image = image.CopyObj(1, -1);
            HOperatorSet.GetImageSize(image, out imageWidth, out imageHeight);
        }

        /// <summary>
        /// 鼠标滚轮放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    zoomBeginRow = (int)(currentBeginRow + (mPositionRow - currentBeginRow) * 0.3);
                    zoomBeginColumn = (int)(currentBeginColumn + (mPositionColumn - currentBeginColumn) * 0.3);
                    zoomEndRow = (int)(currentEndRow - (currentEndRow - mPositionRow) * 0.3);
                    zoomEndColumn = (int)(currentEndColumn - (currentEndColumn - mPositionColumn) * 0.3);
                }
                else
                {
                    zoomBeginRow = (int)(mPositionRow - (mPositionRow - currentBeginRow) / 0.7);
                    zoomBeginColumn = (int)(mPositionColumn - (mPositionColumn - currentBeginColumn) / 0.7);
                    zoomEndRow = (int)(mPositionRow + (currentEndRow - mPositionRow) / 0.7);
                    zoomEndColumn = (int)(mPositionColumn + (currentEndColumn - mPositionColumn) / 0.7);
                }
                double width = hTWindow.Width;
                double height = hTWindow.Height;
                
                //double width = 800;
                //double height = 400;
                bool flag = zoomBeginRow >= imageHeight || zoomEndRow <= 0 || zoomBeginColumn >= imageWidth || zoomEndColumn < 0;
                bool flag2 = zoomEndRow - zoomBeginRow > imageHeight * 10 || zoomEndColumn - zoomBeginColumn > imageWidth * 10;
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
                            InitialHWindow();
                            hTWindow.HalconWindow.DispObj(Region);
                        }
                    }
                }

            }
        }

        private void HTWindow_HMouseDown(object sender, HMouseEventArgsWPF e)
        {
            if (e.Button == MouseButton.Left)
            {
                try
                {
                    int tempNum = 0;
                    hTWindow.HalconWindow.GetMposition(out mouse_X0, out mouse_Y0, out tempNum);//记录按下鼠标时的位置
                    flag = true;
                }
                catch (Exception ex)
                {
                    tb_Status.Text = ex.Message;
                    return;
                }
            }
        }
        private void HTWindow_HMouseUp(object sender, HMouseEventArgsWPF e)
        {

            if (e.Button == MouseButton.Left)
            {
                  
               flag = false;
              
            }

        }

        private void HTWindow_HMouseMove(object sender, HMouseEventArgsWPF e)
        {

            if (e.Button == MouseButton.Left)
            {
                if(flag)
                {  
                    try
                    {
                       
                        int row1, col1, row2, col2;
                        int tempNum = 0;
                        hTWindow.HalconWindow.GetMposition(out mouse_X1, out mouse_Y1, out tempNum);

                        double dbRowMove, dbColMove;
                        dbRowMove = mouse_X0 - mouse_X1;//计算光标在X轴拖动的距离
                        dbColMove = mouse_Y0 - mouse_Y1;//计算光标在Y轴拖动的距离

                        hTWindow.HalconWindow.GetPart(out row1, out col1, out row2, out col2);//计算HWindow控件在当前状态下显示图像的位置
                        hTWindow.HalconWindow.SetPart((int)(row1 + dbRowMove), (int)(col1 + dbColMove), (int)(row2 + dbRowMove), (int)(col2 + dbColMove));//根据拖动距离调整HWindows控件显示图像的位置
                        RefreshImage();//刷新图像
                    }
                    catch (Exception ex)
                    {
                        tb_Status.Text = ex.Message;
                        return;
                    }
                    
                }
            }
        }

        private void RefreshImage()
        {
            hTWindow.HalconWindow.ClearWindow();
            hTWindow.HalconWindow.DispObj(Image);
            if(Region!=null)
            {
                hTWindow.HalconWindow.DispObj(Region);
            }
        }

    }
}
