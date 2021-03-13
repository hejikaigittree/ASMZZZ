using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;
using HalconDotNet;

namespace JFVision
{
    /// <summary>
    ///  用于显示IJFImage对象的面板（使用Halcon）
    /// </summary>
    public partial class UcJFImagePanel : UserControl
    {
        public UcJFImagePanel()
        {
            InitializeComponent();
        }

        //HObject _hImage;

        private void UcJFImagePanel_Load(object sender, EventArgs e)
        {

        }

        private void UcJFImagePanel_VisibleChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 是否显示中心十字线
        /// </summary>
        public bool DrawCross { get; set; }

        /// <summary>
        /// 显示一幅JF图像
        /// </summary>
        /// <param name="img"></param>
        public void DisplayImage(IJFImage img)
        {

        }

        private void hcWndCtrl_HMouseDown(object sender, HalconDotNet.HMouseEventArgs e)
        {

        }

        private void hcWndCtrl_HMouseUp(object sender, HalconDotNet.HMouseEventArgs e)
        {

        }

        private void hcWndCtrl_HMouseMove(object sender, HalconDotNet.HMouseEventArgs e)
        {

        }

        private void hcWndCtrl_HMouseWheel(object sender, HalconDotNet.HMouseEventArgs e)
        {

        }
    }
}
