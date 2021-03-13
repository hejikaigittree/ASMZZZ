using JFInterfaceDef;
using JFUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFHub
{

    /// <summary>
    /// 单工站手动调试界面
    /// </summary>
    public partial class FormSingleStationTest : Form
    {
        public FormSingleStationTest()
        {
            InitializeComponent();
        }

        bool _isFormLoaded = false;
        private void FormSingleStationTest_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustStationView();
            splitContainer1.SplitterDistance = 0;
            splitContainer1.IsSplitterFixed = false;
            toolStripMenuItemDebugInfo.Checked = true;
        }

        JFRealtimeUI _stationUI = null;
        void AdjustStationView()
        {
            Text = _station.Name;
            if (_station == null)
            {
                toolStripStationName.Text = "工站:未设置";
                menuStrip1.Enabled = false;
                return;
            }
            menuStrip1.Enabled = true;
            toolStripStationName.Text = "工站:" + _station.Name;

            if (_station.RunMode == JFStationRunMode.Manual)
                toolStripMenuItemManual.Checked = true;
            else
                toolStripMenuItemAuto.Checked = true;

            _stationUI = _station.GetRealtimeUI();
                if (_stationUI == null)
                {
                    UcStationRealtimeUI sbUi = new UcStationRealtimeUI();
                    sbUi.AllowedStartStopCmd = true;
                    sbUi.SetStation(_station);
                _stationUI = sbUi;
                }
            _stationUI.Dock = DockStyle.Fill;
            splitContainer2.Panel1.Controls.Add(_stationUI);



            if (typeof(JFStationBase).IsAssignableFrom(_station.GetType()))
            {
                toolStripMenuItemCfg.Enabled = true;

                splitContainer1.IsSplitterFixed = false;
                splitContainer1.SplitterDistance = 200;
                toolStripMenuItemCfg.Visible = true;

                UcDataPoolEdit ucDP = new UcDataPoolEdit();
                ucDP.SetDataPool((_station as JFStationBase).DataPool);
                ucDP.Dock = DockStyle.Fill;
                splitContainer1.Panel1.Controls.Add(ucDP);
                ucDP.Show();

            }
            else //
            {
                splitContainer1.IsSplitterFixed = true;
                splitContainer1.Panel1.Width = 0;
            }

        }

        IJFStation _station = null;
        public void SetStation(IJFStation station)
        {
            _station = station;
            if (_isFormLoaded)
                AdjustStationView();
        }

        bool _isReceiveStationMsg = false;
        
        /// <summary>
        /// 使能/禁用 接受工站消息
        /// </summary>
        public bool EnableStationMsgReceive
        {
            get 
            {
                if (null == _station)
                    return false;
                return _isReceiveStationMsg; 
            }
            set
            {
                if (null == _station)
                    return;
                _isReceiveStationMsg = value;
                if(_isReceiveStationMsg)
                {
                    _station.WorkStatusChanged += OnWorkStatusChanged;
                    _station.CustomStatusChanged += OnCustomStatusChanged;
                    if(_station is JFCmdWorkBase)
                    {
                        (_station as JFCmdWorkBase).WorkMsg2Outter += OnTxtMsg;
                    }

                    if(_station is JFStationBase)
                    {
                        (_station as JFStationBase).EventProductFinished += OnProductFinished;
                        (_station as JFStationBase).EventCustomizeMsg += OnCustomizeMsg;
                    }
                }
                else
                {
                    _station.WorkStatusChanged -= OnWorkStatusChanged;
                    _station.CustomStatusChanged -= OnCustomStatusChanged;
                    if (_station is JFCmdWorkBase)
                    {
                        (_station as JFCmdWorkBase).WorkMsg2Outter -= OnTxtMsg;
                    }

                    if (_station is JFStationBase)
                    {
                        (_station as JFStationBase).EventProductFinished -= OnProductFinished;
                        (_station as JFStationBase).EventCustomizeMsg -= OnCustomizeMsg;
                    }
                }

            }
        }


        /// <summary>
        /// 使能/禁用 开始按钮 
        /// 当手动测试界面开始
        /// </summary>
        public bool EnableStartRun
        {
            get { return toolStripMenuItemStart.Enabled; }
            set 
            { 
                toolStripMenuItemStart.Enabled = value;
                toolStripMenuItemReset.Enabled = value;
            }
        }


        /// <summary>
        /// 使能
        /// </summary>
        public bool EnableFlushStationUi
        {
            get { return timer1.Enabled; }
            set { timer1.Enabled = value; }
        }


        private void FormSingleStationTest_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                EnableFlushStationUi = true; 
            }
            else
            {
                EnableFlushStationUi = false;
            }
        }


        delegate void dgWorkStatusChanged(object station, JFWorkStatus currWorkStatus);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currWorkStatus"></param>
        public void OnWorkStatusChanged(object station,JFWorkStatus currWorkStatus)
        {
            if (!Created)
                return;
            if(InvokeRequired)
            {
                Invoke(new dgWorkStatusChanged(OnWorkStatusChanged), new object[] { station, currWorkStatus });
                return;
            }
            string wsName = "";
            switch(currWorkStatus)
            {
                case JFWorkStatus.UnStart:// = 0,    //线程未开始运行
                    wsName = "未开始";
                    break;
                case JFWorkStatus.Running://,        //线程正在运行，未退出
                    wsName = "运行中";
                    break;
                case JFWorkStatus.Pausing://,        //线程暂停中
                    wsName = "已暂停";
                    break;
                case JFWorkStatus.Interactiving://,  //人机交互 ， 等待人工干预指令
                    wsName = "人工干预";
                    break;
                case JFWorkStatus.NormalEnd://     //线程正常完成后退出
                    wsName = "正常结束";
                    break;
                case JFWorkStatus.CommandExit://,    //收到退出指令
                    wsName = "指令结束";
                    break;
                case JFWorkStatus.ErrorExit://,      //发生错误退出，（重启或人工消除错误后可恢复）
                    wsName = "错误退出";
                    break;
                case JFWorkStatus.ExceptionExit://,  //发生异常退出 ,  (不可恢复的错误)
                    wsName = "异常退出";
                    break;
                case JFWorkStatus.AbortExit://,      //由调用者强制退出
                    wsName = "强制终止";
                    break;
                default:
                    wsName = currWorkStatus.ToString();
                    break;
            }
            if (_stationUI is IJFStationMsgReceiver)
                (_stationUI as IJFStationMsgReceiver).OnWorkStatusChanged(currWorkStatus);
            toolStripTextBoxWorkStatus.Text = "运行状态:" + wsName;
            ShowTips("运行状态:" + wsName);
        }


        delegate void dgCustomStatusChanged(IJFWork station, int currCustomStatus);
        public void OnCustomStatusChanged(object station,int currCustomStatus)
        {
            if (!Created)
                return;
            if (InvokeRequired)
            {
                Invoke(new dgCustomStatusChanged(OnCustomStatusChanged), new object[] { station, currCustomStatus });
                return;
            }
            string csName = _station.GetCustomStatusName(currCustomStatus);
            if (string.IsNullOrEmpty(csName))
                csName = "CsCode = " + currCustomStatus;
            toolStripTextBoxCustomStatus.Text = "任务状态:" + csName;
            if (_stationUI is IJFStationMsgReceiver)
                (_stationUI as IJFStationMsgReceiver).OnCustomStatusChanged(currCustomStatus);
            ShowTips("任务状态:" + csName);
        }

        public void OnTxtMsg(object station, string txt)
        {
            if (!Created)
                return;
            ShowTips("文本信息:" + txt);
            if (_stationUI is IJFStationMsgReceiver)
                (_stationUI as IJFStationMsgReceiver).OnTxtMsg(txt);
        }


        delegate void dgProductFinished(object station, int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo);
        public void OnProductFinished(object station, int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo)
        {
            if (!Created)
                return;
            if (InvokeRequired)
            {
                Invoke(new dgProductFinished(OnProductFinished), new object[] { station, passCount, passIDs, ngCount, ngIDs, ngInfo });
                return;
            }
            string info = "加工完成:PassCount = " + passCount + ", NGCount = " + ngCount;
            if (null != passIDs && passIDs.Length > 0)
                info += "\n     PassIDs:" + String.Join(",", passIDs);
            if (null != ngIDs && ngIDs.Length > 0)
                info += "\n     NGIDs:" + String.Join(",", ngIDs);
            if (_stationUI is IJFStationMsgReceiver)
                (_stationUI as IJFStationMsgReceiver).OnProductFinished(passCount, passIDs, ngCount, ngIDs, ngInfo);
            ShowTips(info);
        }


        delegate void dgCustomizeMsg(object station, string msgCategory, object[] msgParams);
        public void OnCustomizeMsg(object station, string msgCategory, object[] msgParams)
        {
            if (!Created)
                return;
            if (InvokeRequired)
            {
                Invoke(new dgCustomizeMsg(OnCustomizeMsg),new object[] { station, msgCategory , msgParams });
                return;
            }
            string info = "专属消息:Category = " + msgCategory + " ParamCount = " + (msgParams == null ? 0 : msgParams.Length);
            if (_stationUI is IJFStationMsgReceiver)
                (_stationUI as IJFStationMsgReceiver).OnCustomizeMsg( msgCategory,  msgParams);
            ShowTips(info);
        }


        /// <summary>
        /// 工站配置菜单按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemCfg_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 工站自动运行模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAuto_Click(object sender, EventArgs e)
        {
            JFWorkStatus ws = _station.CurrWorkStatus;
            if (ws == JFWorkStatus.Running || ws == JFWorkStatus.Pausing || ws == JFWorkStatus.Interactiving)
            {
                MessageBox.Show("工站正在运行，不能修改运行模式");
                return;
            }
            bool isOK = _station.SetRunMode(JFStationRunMode.Auto);
            if (isOK)
            {
                toolStripMenuItemAuto.Checked = true;
                toolStripMenuItemManual.Checked = false;
            }
            else
            {
                MessageBox.Show("设置自动/连续模式 失败");
            }
        }

        /// <summary>
        /// 工站手动运行模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemManual_Click(object sender, EventArgs e)
        {
            JFWorkStatus ws = _station.CurrWorkStatus;
            if (ws == JFWorkStatus.Running || ws == JFWorkStatus.Pausing || ws == JFWorkStatus.Interactiving)
            {
                MessageBox.Show("工站正在运行，不能修改运行模式");
                return;
            }
            bool isOK = _station.SetRunMode(JFStationRunMode.Manual);
            if (isOK)
            {
                toolStripMenuItemAuto.Checked = false;
                toolStripMenuItemManual.Checked = true;
            }
            else
            {
                MessageBox.Show("设置手动/调试模式 失败");
            }
        }

        /// <summary>
        /// 开始单工站运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemStart_Click(object sender, EventArgs e)
        {
            if(null == _station)
            {
                MessageBox.Show("无效操作，工站未设置");
                return;
            }
            JFWorkStatus ws = _station.CurrWorkStatus;
            if(ws == JFWorkStatus.Running || ws == JFWorkStatus.Pausing || ws == JFWorkStatus.Interactiving)
            {
                JFTipsDelayClose.Show("无效操作，工站当前正在运行:" + ws, 1);
                return;
            }

            //EnableStationMsgReceive = true;
            JFWorkCmdResult ret = _station.Start();
            if (ret != JFWorkCmdResult.Success)
            {
                MessageBox.Show("启动工站失败,错误代码:" + ret);
                //EnableStationMsgReceive = false;
            }
            else
            {


                JFTipsDelayClose.Show("操作信息:工站已启动", 1);

            }

        }

        /// <summary>
        /// 停止单工站运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemStop_Click(object sender, EventArgs e)
        {
            if(null == _station)
            {
                MessageBox.Show("无效操作，工站未设置");
                return;
            }
            
            JFWorkCmdResult ret = _station.Stop(2000);
            if (ret != JFWorkCmdResult.Success)
            {
                if(DialogResult.OK == MessageBox.Show(" 停止操作失败，错误代码:" + ret + "\n是否强制终止？","警告",MessageBoxButtons.OKCancel,MessageBoxIcon.Error))
                {
                    _station.Abort();
                    JFTipsDelayClose.Show("工站已强制停止！", 2);
                }
            }
            else
                JFTipsDelayClose.Show("工站已停止！", 2); 
        }

        /// <summary>
        /// 显示工站配置界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == _station)
                return;
            _station.ShowCfgDialog();
        }

        private void baseTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == _station)
                return;
            Form fm = _station.GenForm();
            fm.ShowDialog();

            //if(typeof(JFStationBase).IsAssignableFrom(_station.GetType()))
            //{
            //    FormStationBaseDebug fm = new FormStationBaseDebug();
            //    fm.SetStation(_station as JFStationBase);
            //    fm.ShowDialog();
            //}
        }


        int _maxTips = 1000;
        delegate void dgShowTips(string info);
        void ShowTips(string info)
        {
            if (!Created)
                return;
            if (InvokeRequired)
            {
                if (!Created)
                    return;
                Invoke(new dgShowTips(ShowTips), new object[] { info });
                return;
            }
            if (string.IsNullOrEmpty(info))
                return;
            rchTips.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " " + info + "\n");
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

        private void btClearTips_Click(object sender, EventArgs e)
        {
            rchTips.Text = "";
        }


        /// <summary>
        /// 工站归零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemReset_Click(object sender, EventArgs e)
        {
            if (null == _station)
            {
                MessageBox.Show("无效操作，工站未设置");
                return;
            }
            JFWorkStatus ws = _station.CurrWorkStatus;
            if (ws == JFWorkStatus.Running || ws == JFWorkStatus.Pausing || ws == JFWorkStatus.Interactiving)
            {
                JFTipsDelayClose.Show("无效操作，工站当前正在运行:" + ws, 1);
                return;
            }


            JFWorkCmdResult ret = _station.Reset();
            if (ret != JFWorkCmdResult.Success)
                MessageBox.Show("启动复位失败,错误代码:" + ret);
            else
            {

                ShowTips("操作信息:复位已启动");
                JFTipsDelayClose.Show("复位已启动", 1);

            }
        }


        /// <summary>
        /// 工站暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemPause_Click(object sender, EventArgs e)
        {
            if (null == _station)
            {
                MessageBox.Show("无效操作，工站未设置");
                return;
            }
            JFWorkStatus ws = _station.CurrWorkStatus;
            if(!JFStationBase.IsWorkingStatus(ws))
            {
                JFTipsDelayClose.Show("无效操作，工站当前未运行:" + ws, 1);
                return ;
            }

            if(ws == JFWorkStatus.Pausing)
            {
                JFTipsDelayClose.Show("无效操作，工站当前已暂停", 1);
                return;
            }


            JFWorkCmdResult ret = _station.Pause(2000);
            if (ret != JFWorkCmdResult.Success)
                MessageBox.Show("工站暂停失败,错误代码:" + ret);
            else
            {

                ShowTips("操作信息:工站已暂停");
                JFTipsDelayClose.Show("工站已暂停", 1);

            }
        }


        /// <summary>
        /// 工站恢复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemResume_Click(object sender, EventArgs e)
        {
            if (null == _station)
            {
                MessageBox.Show("无效操作，工站未设置");
                return;
            }
            JFWorkStatus ws = _station.CurrWorkStatus;
            if (!JFStationBase.IsWorkingStatus(ws))
            {
                JFTipsDelayClose.Show("无效操作，工站当前未运行:" + ws, 1);
                return;
            }

            if (ws  != JFWorkStatus.Pausing)
            {
                JFTipsDelayClose.Show("无效操作，工站当前非暂停状态:" + ws, 1);
                return;
            }


            JFWorkCmdResult ret = _station.Resume(2000);
            if (ret != JFWorkCmdResult.Success)
                MessageBox.Show("恢复运行失败,错误代码:" + ret);
            else
            {

                ShowTips("操作信息:工站已恢复运行");
                JFTipsDelayClose.Show("工站已恢复运行", 1);

            }
        }

        /// <summary>
        /// 显示工站数据池
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDataPool_Click(object sender, EventArgs e)
        {
            if(_station == null)
            {
                MessageBox.Show("无效操作，工站未设置");
                return;
            }

            if (!typeof(JFStationBase).IsAssignableFrom(_station.GetType()))
            {
                MessageBox.Show("无效操作，当前工站基类型不是JFStationBase！");
                return;
            }

            if(!toolStripMenuItemDataPool.Checked)
            {
                splitContainer1.IsSplitterFixed = false;
                splitContainer1.SplitterDistance = 150;
                toolStripMenuItemDataPool.Checked = true;
            }
            else
            {
                splitContainer1.IsSplitterFixed = true;
                splitContainer1.SplitterDistance = 0;
                toolStripMenuItemDataPool.Checked = false;
            }
        }

        /// <summary>
        /// 显示调试信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDebugInfo_Click(object sender, EventArgs e)
        {
            if(!toolStripMenuItemDebugInfo.Checked)
            {
                splitContainer2.SplitterDistance = Height - 200;
                splitContainer2.IsSplitterFixed = false;
                toolStripMenuItemDebugInfo.Checked = true;
            }
            else
            {
                splitContainer2.SplitterDistance = Height-2;
                splitContainer2.IsSplitterFixed = true;
                toolStripMenuItemDebugInfo.Checked = false;
            }
        }


        /// <summary>
        /// 结批操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemEndBatch_Click(object sender, EventArgs e)
        {
            if (null == _station)
            {
                MessageBox.Show("无效操作，工站未设置");
                return;
            }
            JFWorkStatus ws = _station.CurrWorkStatus;
            if (!JFStationBase.IsWorkingStatus(ws))
            {
                JFTipsDelayClose.Show("无效操作，工站当前未运行:" + ws, 1);
                return;
            }

            JFWorkCmdResult ret = _station.EndBatch(2000);
            if (ret != JFWorkCmdResult.Success)
                MessageBox.Show("发送结批指令失败,错误代码:" + ret);
            else
            {

                ShowTips("操作信息:结批指令已发送");
                JFTipsDelayClose.Show("结批指令已发送", 1);

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //刷新界面
            if(null == _stationUI)
            {
                timer1.Enabled = false;
                return;
            }

            _stationUI.UpdateSrc2UI();//将工站状态更新到界面上

        }
    }
}
