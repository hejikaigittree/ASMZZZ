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

namespace JFUI
{
    /// <summary>
    /// 为Station基类提供一个RealtimeUI
    /// </summary>
    public partial class UcStationRealtimeUI : JFRealtimeUI/*UserControl*/, IJFStationMsgReceiver
    {
        public enum JFDisplayMode
        {
            full = 0, //控件全部显示
            middling,
            simple, //简单界面 ，显示位置/速度操作面板
        }
        public UcStationRealtimeUI()
        {
            InitializeComponent();
        }

        JFDisplayMode _displayMode = JFDisplayMode.full;
        [Category("JF属性"), Description("显示模式"), Browsable(true)]
        public JFDisplayMode JfDisplayMode
        {
            get { return _displayMode; }
            set
            {
                _displayMode = value;
                switch(_displayMode)
                {
                    case JFDisplayMode.full:
                        Height = 130;
                        break;
                    case JFDisplayMode.middling:
                        Height = 101;
                        break;
                    case JFDisplayMode.simple:
                        Height = 55;
                        break;

                }
            }
        }

                bool _isFormLoaded = false;
        IJFStation _station = null;
        private void UcStationBaseRealtimeUI_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            tbPFCount.Text = "0";
            tbPassCount.Text = "0";
            tbNGCount.Text = "0";
            tbStartTime.Text = "未开始";
            tbRunTime.Text = "00:00:00";
            AdjustStationView();
        }

        bool _isAllowedStartStopCmd = false;
        /// <summary>
        /// 是否在指令栏中包含 开始/停止/结批指令
        /// </summary>
        public bool AllowedStartStopCmd
        {
            get { return _isAllowedStartStopCmd; }
            set
            {
                _isAllowedStartStopCmd = value;
                if (_isFormLoaded)
                    AdjustStationView();
            }
        }


        /// <summary>
        /// 接收一条Log，
        /// </summary>
        /// <param name="info"></param>
        public void ShowLog(string info)
        {
            ShowTips(info);
        }

        /// <summary>
        /// 工站是否已启动运行
        /// </summary>
        bool _isStationStartRunning = false;
        DateTime _startTime = DateTime.Now;

        delegate void dgWorkStatusChanged(JFWorkStatus currWorkStatus);
        /// <summary>
        /// 处理工站状态改变
        /// </summary>
        /// <param name="station"></param>
        /// <param name="currWorkStatus"></param>
        public void OnWorkStatusChanged(JFWorkStatus currWorkStatus)
        {
            if (!Created)
                return;
            if (InvokeRequired)
            {
                Invoke(new dgWorkStatusChanged(OnWorkStatusChanged), new object[] { currWorkStatus });
                return;
            }
            switch(currWorkStatus)
            {
                case JFWorkStatus.UnStart:// = 0,    //线程未开始运行
                    lampWorkStatus.LampColor = JFUI.LampButton.LColor.Gray;
                    lampWorkStatus.Text = "未运行";
                    break;
                case JFWorkStatus.Running://,        //线程正在运行，未退出
                    if(!_isStationStartRunning)
                    {
                        _isStationStartRunning = true;
                        _startTime = DateTime.Now;
                        tbStartTime.Text = _startTime.ToString("yy-MM-dd HH:mm:ss");
                        timer1.Start(); //启动计时器
                    }
                    lampWorkStatus.LampColor = JFUI.LampButton.LColor.Green;
                    lampWorkStatus.Text = "运行中";
                    break;
                case JFWorkStatus.Pausing://,        //线程暂停中
                    lampWorkStatus.LampColor = JFUI.LampButton.LColor.Yellow;
                    lampWorkStatus.Text = "暂停中";
                    break;
                case JFWorkStatus.Interactiving: //人机交互 ，需要人工干预的情况
                    lampWorkStatus.LampColor = JFUI.LampButton.LColor.Yellow;
                    lampWorkStatus.Text = "人工干预";
                    break;
                case JFWorkStatus.NormalEnd://,     //线程正常完成后退出
                    _isStationStartRunning = false;
                    timer1.Stop();
                    lampWorkStatus.LampColor = JFUI.LampButton.LColor.Gray;
                    lampWorkStatus.Text = "正常结束";
                    break;
                case JFWorkStatus.CommandExit://,    //收到退出指令
                    _isStationStartRunning = false;
                    timer1.Stop();
                    lampWorkStatus.LampColor = JFUI.LampButton.LColor.Gray;
                    lampWorkStatus.Text = "命令结束";
                    break;
                case JFWorkStatus.ErrorExit://,      //发生错误退出
                    _isStationStartRunning = false;
                    timer1.Stop();
                    lampWorkStatus.LampColor = JFUI.LampButton.LColor.Red;
                    lampWorkStatus.Text = "错误退出";
                    break;
                case JFWorkStatus.ExceptionExit://,  //发生异常退出
                    _isStationStartRunning = false;
                    timer1.Stop();
                    lampWorkStatus.LampColor = JFUI.LampButton.LColor.Red;
                    lampWorkStatus.Text = "异常退出";
                    break;
                case JFWorkStatus.AbortExit:      //由调用者强制退出
                    _isStationStartRunning = false;
                    timer1.Stop();
                    lampWorkStatus.LampColor = JFUI.LampButton.LColor.Red;
                    lampWorkStatus.Text = "强制退出";
                    break;
                default:
                    lampWorkStatus.LampColor = JFUI.LampButton.LColor.Red;
                    lampWorkStatus.Text = "未知状态";
                    break;
            }
        }
        //void HandleStationMsg(IJFStation station,object msg )


        delegate void dgCustomStatusChanged(int currCustomStatus);
        /// <summary>
        ///  处理工站的业务状态发生改变
        /// </summary>
        /// <param name="station"></param>
        /// <param name="currCustomStatus"></param>
        public void OnCustomStatusChanged( int currCustomStatus)
        {
            if (!Created)
                return;
            if (InvokeRequired)
            {
                Invoke(new dgCustomStatusChanged(OnCustomStatusChanged), new object[] { currCustomStatus });
                return;
            }
            int[] allStatusIDs = _station.AllCustomStatus;
            for(int i = 0; i < allStatusIDs.Length;i++)
                if(currCustomStatus == allStatusIDs[i])
                {
                    lstBoxCustomStatus.SelectedIndex = i;
                    if (i == 0)
                        lstBoxCustomStatus.TopIndex = 0;
                    else
                        lstBoxCustomStatus.TopIndex = i - 1;
                }
        }

        int _passTotal = 0; //Pass总数
        int _ngTotal = 0; //NG总数
        int _productFinishedIndex = 0; //批次
        delegate void dgProductFinished(int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo);
        /// <summary>
        /// 产品加工完成消息
        /// </summary>
        /// <param name="station">消息发送者</param>
        /// <param name="PassCount">本次生产完成的成品数量</param>
        /// <param name="NGCount">本次生产的次品数量</param>
        /// <param name="NGInfo">次品信息</param>
        public void OnProductFinished(int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo)
        {
            if (!Created)
                return;
            if (InvokeRequired)
            {
                Invoke(new dgProductFinished(OnProductFinished), new object[] { passCount, passIDs, ngCount, ngIDs, ngInfo });
                return;
            }
            _productFinishedIndex++;
            _passTotal += passCount;
            _ngTotal += ngCount;
            tbPFCount.Text = _productFinishedIndex.ToString();
            tbNGCount.Text = _ngTotal.ToString();
            tbPassCount.Text = _passTotal.ToString();
            ShowTips("第" + _productFinishedIndex + "次产品统计:Pass:" + passCount + " NG:" + ngCount);

        }

        delegate void dgCustomizeMsg(string msgCategory, object[] msgParams);
        /// <summary>
        /// 处理工站发来的其他定制化的消息
        /// </summary>
        /// <param name="station"></param>
        /// <param name="msg"></param>
        public virtual void OnCustomizeMsg( string msgCategory, object[] msgParams)
        {
            if (!Created)
                return;
            if (InvokeRequired)
            {
                Invoke(new dgCustomizeMsg(OnCustomizeMsg), new object[] { msgCategory, msgParams });
                return;
            }
            ShowTips("CustomizeMsg:" + msgCategory + " msgParams" + (msgParams == null ? ":null" : ("params count = " + msgParams.Length)));
        }


        public void SetStation(IJFStation station)
        {
            _station = station;
            if (_isFormLoaded)
                AdjustStationView();
        }

        void AdjustStationView()
        {
            lstBoxCustomStatus.Items.Clear();
            cbCmds.Items.Clear();
            if (null == _station)
            {
                gbStationName.Text = "工站:未设置";
                btResetProductInfo.Enabled = false;
                btSendCmd.Enabled = false;
                cbCmds.Enabled = false;
                btShowLog.Enabled = false;
                btClearTips.Enabled = false;
                return;
            }
            
            gbStationName.Text = "工站:" + _station.Name;
            btResetProductInfo.Enabled = true;
            //btSendCmd.Enabled = true;
            //cbCmds.Enabled = true;
            btShowLog.Enabled = true;
            btClearTips.Enabled = true;

            
            int[] allCustomStatusIDs = _station.AllCustomStatus;
            if(null != allCustomStatusIDs)
            {
                foreach(int customStatusID in allCustomStatusIDs)
                {
                    string statusName = _station.GetCustomStatusName(customStatusID);
                    if (string.IsNullOrEmpty(statusName))
                        statusName = "CStatus_ID:" + customStatusID;
                    lstBoxCustomStatus.Items.Add(statusName);
                }
            }

            
            btSendCmd.Enabled = true;
            cbCmds.Enabled = true;
            if(AllowedStartStopCmd)
                cbCmds.Items.Add("开始");
            cbCmds.Items.Add("暂停");
            cbCmds.Items.Add("恢复");
            if (AllowedStartStopCmd)
            {
                cbCmds.Items.Add("结批");
                cbCmds.Items.Add("停止");
            }
            int[] allCmds = _station.AllCmds;
            if (null != allCmds)
                foreach (int cmd in allCmds)
                {
                    string cmdTxt = _station.GetCmdName(cmd);
                    if (string.IsNullOrEmpty(cmdTxt))
                        cmdTxt = "CmdID:" + cmd;
                    cbCmds.Items.Add(cmdTxt);
                }
            
        }

        int _maxTips = 2000; //最多显示2000条信息
        delegate void dgShowTips(string info);
        public void ShowTips(string info)
        {
            if (InvokeRequired)
            {
                if (!Created)
                    return;
                Invoke(new dgShowTips(ShowTips), new object[] { info });
                return;
            }
            rchTips.AppendText(info + "\n");
            string[] lines = rchTips.Text.Split('\n');
            if (lines.Length >= _maxTips)
            {
                int rmvChars = 0;
                for (int i = 0; i < lines.Length - _maxTips; i++)
                    rmvChars += lines[i].Length + 1;
                rchTips.Text = rchTips.Text.Substring(rmvChars);
            }
            rchTips.Select(rchTips.TextLength, 0); //滚到最后一行
            rchTips.ScrollToCaret();//滚动到控件光标处 
        }

        private void btResetProductInfo_Click(object sender, EventArgs e)
        {
            if(!_isStationStartRunning)
            {
                tbStartTime.Text = "未运行";
                tbRunTime.Text = "";
                tbPFCount.Text = "0";
                tbPassCount.Text = "0";
                tbNGCount.Text = "0";
            }
            else
            {
                _startTime = DateTime.Now;
                tbStartTime.Text = _startTime.ToString("yy-MM-dd HH:mm:ss") ;
                tbPFCount.Text = "0";
                tbPassCount.Text = "0";
                tbNGCount.Text = "0";
            }
        }

        /// <summary>
        /// 向工站发送一条指令（包含 开始/停止/暂停/恢复/结批 和用户自定义指令）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSendCmd_Click(object sender, EventArgs e)
        {
            int selIndex = cbCmds.SelectedIndex;
            if(selIndex < 0)
            {
                JFTipsDelayClose.Show("请选择需要发送的指令", 2);//MessageBox.Show("请选择需要发送的指令");
                return;
            }
            JFWorkCmdResult cmdRet = JFWorkCmdResult.UnknownError;
            if(AllowedStartStopCmd)
            {
                if (selIndex < 5) //开始/暂停/恢复/结批/停止
                    switch (selIndex)
                    {
                        case 0: cmdRet = _station.Start(); break;
                        case 1: cmdRet = _station.Pause(1000); break;
                        case 2: cmdRet = _station.Resume(1000); break;
                        case 3: cmdRet = _station.EndBatch(1000); break;
                        case 4: cmdRet = _station.Stop(1000); break;
                    }
                else
                {
                    int cmdIndex = selIndex - 5;
                    cmdRet = _station.SendCmd(_station.AllCmds[cmdIndex], 1000);
                }
            }
            else
            {
                if (selIndex < 2) //暂停/恢复
                    switch (selIndex)
                    {
                        case 0: cmdRet = _station.Pause(1000); break;
                        case 1: cmdRet = _station.Resume(1000); break;
                    }
                else
                {
                    int cmdIndex = selIndex - 2;
                    cmdRet = _station.SendCmd(_station.AllCmds[cmdIndex], 1000);
                }
            }
            string errInfo = "";
            switch(cmdRet)
            {
                case JFWorkCmdResult.UnknownError:// = -1, //发生未定义的错误
                    errInfo = "未知错误";
                    break;
                case JFWorkCmdResult.Success:// = 0, //指令执行成功
                    errInfo = "指令执行成功";
                    break;
                case JFWorkCmdResult.IllegalCmd://,//不支持的非法指令
                    errInfo = "不支持的非法指令";
                    break;
                case JFWorkCmdResult.StatusError://, //工作状态（包括用户自定义状态）不支持当前指令 ，（向未运行的线程发送Resume指令）
                    errInfo = "当前状态不支持该指令";
                    break;
                case JFWorkCmdResult.ActionError://, //指令执行失败
                    errInfo = "执行失败";
                    break;
                case JFWorkCmdResult.Timeout://,//线程超时未响应
                    errInfo = "指令执行超时";
                    break;
            }

            if (cmdRet != JFWorkCmdResult.Success)
                JFTipsDelayClose.Show("指令:" + cbCmds.Text + " 发送失败:" + errInfo,3);
            else
                ShowTips("指令:" + cbCmds.Text + "发送完成");

                
        }

        private void btClearTips_Click(object sender, EventArgs e)
        {
            rchTips.Text = "";
        }

        /// <summary>
        /// 显示日志窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btShowLog_Click(object sender, EventArgs e)
        {
            //string[] items = new string[] { "hehe", "a---", "Yi????????" };
            //int selectIndex = JFSelectTipsUI.ShowDialog("选择功能演示", items, 2,5); //已测试OK
            //MessageBox.Show("Select: " + (selectIndex < 0 ? "null" : items[selectIndex]));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_isStationStartRunning)
            {
                TimeSpan ts = DateTime.Now - _startTime;
                tbRunTime.Text = ts.ToString(@"dd\.hh\:mm\:ss");
            }
                

        }

        public void OnTxtMsg(string txt)
        {
            ShowTips(txt);
        }
    }
}
