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

namespace JFVision
{
    public partial class UcRTLoadImage1 : JFRealtimeUI//UserControl
    {
        public UcRTLoadImage1()
        {
            InitializeComponent();
        }
        JFVM_LoadImage _method = null;
        bool _isFormLoaded = false; 
        private void UcRTLoadImage1_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
                AdjustView();
        }

        public void SetMethodObj(JFVM_LoadImage method)
        {
            _method = method;
            if (_isFormLoaded)
                AdjustView();
        }

        public void AdjustView()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AdjustView));
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
            IJFImage img = _method.GetMethodOutputValue("Image") as IJFImage;
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
