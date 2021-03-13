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
using HTHalControl;   //封装Halcon显示控件 dll
using LFAOIRecipe;    //封装检测算子dll
using HalconDotNet;

namespace DLAF_DS
{
    public partial class JFRtUiSingleImageInspection : JFRealtimeUI//UserControl
    {
        public JFRtUiSingleImageInspection()
        {
            InitializeComponent();
        }

        private void JFRtUiSingleImageInspection_Load(object sender, EventArgs e)
        {
            AdjustInspectView();
        }

        void AdjustInspectView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustInspectView));
                return;
            }
            if (null == _inspection)
            {
                lbActionErrorInfo.Text = "算子对象未设置/空值";
                label1.Visible = false;
                label2.Visible = false;
                label5.Visible = false;
                lbActionSeconds.Visible = false;
                lbInspectRet.Visible = false;
                lbInspectInfo.Visible = false;
                return;
            }

            label1.Visible = true;
            label2.Visible = true;
            lbInspectRet.Visible = true;
            lbInspectInfo.Visible = true;
            label5.Visible = true;
            lbActionSeconds.Visible = true;
        }

        DLAFMethod_Inspection _inspection = null; //算子对象
        /// <summary>
        /// 设置算子对象
        /// </summary>
        /// <param name="inspection"></param>
        public void SetInspection(DLAFMethod_Inspection inspection)
        {
            _inspection = inspection;
            AdjustInspectView();
            return;
        }

        public override void UpdateSrc2UI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateSrc2UI));
                return;
            }
            if (null == _inspection)
                return;
            lbActionErrorInfo.Text = _inspection.GetActionErrorInfo();
            lbInspectRet.Text = _inspection.GetMethodOutputValue("ErrorCode").ToString();
            lbInspectInfo.Text = _inspection.GetMethodOutputValue("ErrorInfo").ToString();
            lbActionSeconds.Text = _inspection.GetActionSeconds().ToString("F3");

            htWindowControl1.ClearHWindow();
            IJFImage imgInput = _inspection.GetMethodInputValue("Image") as IJFImage;
            if (null == imgInput)
                return;
            object hoImg = null;
            if (0 == imgInput.GenHalcon(out hoImg))
                htWindowControl1.DispImage(hoImg as HObject);

            HObject BondContours = _inspection.GetMethodOutputValue("焊点区域") as HObject;
            if (null != BondContours)
                htWindowControl1.DispImage(BondContours);

            HObject Wires = _inspection.GetMethodOutputValue("金线区域") as HObject;
            if (null != Wires)
                htWindowControl1.DispImage(Wires);


            HObject FailRegs = _inspection.GetMethodOutputValue("失败区域") as HObject;
            if (FailRegs != null)
                htWindowControl1.DispImage(FailRegs);
        }

        private void btAction_Click(object sender, EventArgs e)
        {
            if(null == _inspection)
            {
                MessageBox.Show("算子未设置");
                return;
            }

            _inspection.Action();
            UpdateSrc2UI();

        }
    }
}
