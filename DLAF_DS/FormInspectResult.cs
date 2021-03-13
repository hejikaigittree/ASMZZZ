using HalconDotNet;
using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLAF_DS
{
    /// <summary>
    /// 用于显示测试结果的窗体
    /// </summary>
    public partial class FormInspectResult : Form
    {
        public FormInspectResult()
        {
            InitializeComponent();
        }

        private void FormInspectResult_Load(object sender, EventArgs e)
        {

        }

        public bool HideWhenClose { get; set; }


        delegate void dgShowInspectResult(object[] msgParams);
        public void ShowInspectResult(object[] msgParams)
        {
            if(InvokeRequired)
            {
                Invoke(new dgShowInspectResult(ShowInspectResult), new object[] { msgParams });
                return;
            }

            List<IJFImage> lstTaskImages = msgParams[0] as List<IJFImage>;
            string recipeID = msgParams[1] as string;
            int icRow = Convert.ToInt32(msgParams[2]);
            int icCol = Convert.ToInt32(msgParams[3]);
            string fovName = msgParams[4] as string;
            bool detectResult = Convert.ToBoolean(msgParams[5]);
            string detectErrorInfo = msgParams[6] as string;
            List<int[]> diesErrorCodes = msgParams[7] as List<int[]>;
            string[] dieErrorInfos = msgParams[8] as string[];
            HObject bondRegion = msgParams[9] as HObject;
            HObject wiresRegion = msgParams[10] as HObject;
            HObject failedRegion = msgParams[11] as HObject;

            List<HObject> rgs = new List<HObject>();
            if (null != bondRegion)
                rgs.Add(bondRegion);
            if (null != wiresRegion)
                rgs.Add(wiresRegion);
            if (null != failedRegion)
                rgs.Add(failedRegion);
            HObject hoRegion = null;
            if (rgs.Count() != 0)
            {
                hoRegion = rgs[0];
                if (rgs.Count > 1)
                {
                    hoRegion = rgs[0];
                    for (int i = 1; i < rgs.Count(); i++)
                        HOperatorSet.ConcatObj(rgs[i], hoRegion, out hoRegion);
                }
            }
            object objTmp;
            lstTaskImages[0].GenHalcon(out objTmp);
            HObject hoImg = objTmp as HObject;
            for (int i = 1; i < lstTaskImages.Count; i++)
            {
                object ot;
                lstTaskImages[i].GenHalcon(out ot);
                HOperatorSet.ConcatObj(hoImg, ot as HObject, out hoImg);
            }


            if (null == hoRegion)
                htWindowControl1.DispImage(hoImg);
            else
                htWindowControl1.RefreshWindow(hoImg, hoRegion, "fit");
            Text = "Recipe:" + recipeID + " Row=" + icRow + " Col=" + icCol + " Fov:" + fovName; 
            lbErrorCode.Text = detectResult?"视觉检测成功":"检测检测失败:"+detectErrorInfo;
            StringBuilder sbInfo = new StringBuilder();
            if (diesErrorCodes!= null)
            {
                for (int i = 0; i < diesErrorCodes.Count; i++)
                {
                    sbInfo.Append("Die-" + i + " ErrorCode:" + string.Join("|", diesErrorCodes[i]) + "\n");
                    sbInfo.Append("Die-" + i + " ErrorInfo:" + dieErrorInfos[i] + "\n");
                }
            }
            lbErrorInfo.Text = sbInfo.ToString();
        }

        private void FormInspectResult_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(HideWhenClose)
            {
                Hide();
                e.Cancel = true;
                return;
            }
        }
    }
}
