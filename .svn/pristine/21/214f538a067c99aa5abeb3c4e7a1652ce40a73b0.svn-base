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

namespace JFMethodCommonLib.相机控制
{
    //用于显示相机抓到的一幅图片
    public partial class UcRTSnapOne : JFRealtimeUI//UserControl
    {
        public UcRTSnapOne()
        {
            InitializeComponent();
        }

        JFCM_CmrSnapOne_S _method = null; //方法对象
        bool _isFormLoaded = false;
        private void UcRTSnapOne_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustUCView();
        }

        public void SetMethod(JFCM_CmrSnapOne_S method)
        {
            _method = method;
        }

        public void  AdjustUCView()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AdjustUCView));
                return;
            }
            if (!_isFormLoaded)
                return;
            UpdateSrc2UI();
        }


        public override void UpdateSrc2UI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateSrc2UI));
                return;
            }

            if (null == _method)
            {
                return;
            }
            IJFImage img = _method.GetMethodOutputValue(JFCM_CmrSnapOne_S.ON_Image) as IJFImage;
            if (null == img)
            {
                
                return;
            }
            else
            {
                ucImageView1.LoadImage(img);
                ucImageView1.UpdateView();
            }
        }
    }
}
