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
using JFHub;
using HalconDotNet;

namespace DLAF_DS
{
    public partial class UcDetectStationRT : JFRealtimeUI,IJFStationMsgReceiver//UserControl ,
    {
        public UcDetectStationRT()
        {
            InitializeComponent();
        }

        private void UcDetectStationRT_Load(object sender, EventArgs e)
        {

        }

        DLAFDetectStation _station = null;
        public void SetStation(DLAFDetectStation station)
        {
            _station = station;
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            if(_station == null)
            {
                MessageBox.Show("归零操作失败！工站未设置/空值");
                return;
            }
            JFWorkStatus ws = _station.CurrWorkStatus;
            if(ws == JFWorkStatus.Running || ws == JFWorkStatus.Pausing || ws == JFWorkStatus.Interactiving)
            {
                MessageBox.Show("归零操作失败！工站当前正在运行");
                return;
            }
            JFWorkCmdResult ret = _station.Reset();


        }


        private void btStopRun_Click(object sender, EventArgs e)
        {
            if (null == _station)
                return;
            _station.Stop();
        }


        public void OnWorkStatusChanged(JFWorkStatus currWorkStatus) //工作状态变化
        {

        }

        public void OnCustomStatusChanged(int currCustomStatus) //业务逻辑变化
        {

        }
        public void OnTxtMsg(string txt) //接受一条文本消息
        {
        }

        public void OnProductFinished(int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo) //产品加工完成
        {

        }

        delegate void dgCustomizeMsg(string msgCategory, object[] msgParams);
        public void OnCustomizeMsg(string msgCategory, object[] msgParams) //其他自定义消息
        {
            if(InvokeRequired)
            {
                Invoke(new dgCustomizeMsg(OnCustomizeMsg), new object[] { msgCategory, msgParams });
                return;
            }
            if(msgCategory == DLAFDetectStation.CMC_ShowJFImage)
            {
                if (msgParams == null || 0 == msgParams.Length)
                    return;
                IJFImage ji = msgParams[0] as IJFImage;
                if (null == ji)
                    return;
                object hoImg = null;
                if (0 != ji.GenHalcon(out hoImg))
                    return;
                htWindowControl1.DispImage(hoImg as HObject);
            }
            else if(msgCategory == DLAFDetectStation.CMC_ShowHO)
            {
                if (msgParams == null || 0 == msgParams.Length)
                    return;
                HObject hoImg = msgParams[0] as HObject;
                if (null== hoImg)
                    return;
                htWindowControl1.DispImage(hoImg);
            }
            else if(msgCategory == DLAFDetectStation.CMC_InspectResult)////检测结果 , 后面的参数列表为 HObject/图像，RecipeID，ICRow,ICCol,FovName,ErrorCode,ErrorInfo,Hobject-Region/Bond,Hobject-Region/Wires,Hobject-FailedRegs
            {
                //IJFImage[] lstTaskImages = msgParams[0] as IJFImage[];
                //string recipeID = msgParams[1] as string;
                //int icRow = Convert.ToInt32(msgParams[2]);
                //int icCol = Convert.ToInt32(msgParams[3]);
                //string fovName = msgParams[4] as string;
                //bool detectResult = Convert.ToBoolean(msgParams[5]);
                //string detectErrorInfo = msgParams[6] as string;
                //List<int[]> diesErrorCodes = msgParams[7] as List<int[]>;
                //string[] dieErrorInfos = msgParams[8] as string[];
                //HObject bondRegion = msgParams[9] as HObject;
                //HObject wiresRegion = msgParams[10] as HObject;
                //HObject failedRegion = msgParams[11] as HObject;


                //object objTmp;
                //lstTaskImages[0].GenHalcon(out objTmp);
                //HObject hoImg = objTmp as HObject;
                //for (int i = 1; i < lstTaskImages.Count; i++)
                //{
                //    object ot;
                //    lstTaskImages[i].GenHalcon(out ot);
                //    HOperatorSet.ConcatObj(hoImg, ot as HObject, out hoImg);
                //}

                //List<HObject> rgs = new List<HObject>();
                //if (null != bondRegion)
                //    rgs.Add(bondRegion);
                //if (null != wiresRegion)
                //    rgs.Add(wiresRegion);
                //if(null != failedRegion)
                //    rgs.Add(failedRegion);
                //if (rgs.Count() == 0)
                //    return;
                //HObject hoRegion = rgs[0];
                //if(rgs.Count > 1)
                //{
                //    hoRegion = rgs[0];
                //    for (int i = 1; i< rgs.Count();i++)
                //        HOperatorSet.ConcatObj(rgs[i], hoRegion, out hoRegion);
                //}
                
                //htWindowControl1.RefreshWindow(hoImg, hoRegion, "fit");

                //StringBuilder sbInfo = new StringBuilder();
                //sbInfo.AppendLine("-----------------------------");
                //sbInfo.AppendLine("IcRow:" + icRow + " IcCol:" + icCol + "FovName:" + fovName + " 检测结果:");
                //if(!detectResult)
                //{
                //    sbInfo.AppendLine("视觉检测失败:" + detectErrorInfo);
                //}
                //else
                //{
                //    for (int i = 0; i < diesErrorCodes.Count(); i++)
                //    {
                //        sbInfo.AppendLine("Die-" + i + " ErrorCodes:" + string.Join("|", diesErrorCodes[i]));
                //        sbInfo.AppendLine( "     ErrorInfo:" + dieErrorInfos[i]);
                //    }
                //}
                //ShowDetectResultTxt(sbInfo.ToString());
            }


        }


        int _maxLineInDetectResult = 2000;
        delegate void dgShowDetectResultTxt(string info);
        void ShowDetectResultTxt(string info)
        {
            if(InvokeRequired)
            {
                Invoke(new dgShowDetectResultTxt(ShowDetectResultTxt),new object[] { info });
                return;
            }
            richRetectResult.AppendText(info + "\n");
            string[] lines = richRetectResult.Text.Split('\n');
            if (lines.Length >= _maxLineInDetectResult)
            {
                int rmvChars = 0;
                for (int i = 0; i < lines.Length - _maxLineInDetectResult; i++)
                    rmvChars += lines[i].Length + 1;
                richRetectResult.Text = richRetectResult.Text.Substring(rmvChars);
            }
            richRetectResult.Select(richRetectResult.TextLength, 0); //滚到最后一行
            richRetectResult.ScrollToCaret();//滚动到控件光标处 
        }

        /// <summary>
        /// 清空视觉检测结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClearDetectResult_Click(object sender, EventArgs e)
        {
            richRetectResult.Text = "";
        }
    }
}
