using JFHub;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFUI;
using JFInterfaceDef;
using System.Runtime.Remoting.Channels;

namespace JFApp
{
    /// <summary>
    /// 手动测试各工站的界面
    /// </summary>
    public partial class FormManual : Form
    {
        public FormManual()
        {
            InitializeComponent();
        }

        private void FormManual_Load(object sender, EventArgs e)
        {
            fmSysDP.SetDataPool(JFHubCenter.Instance.DataPool);
            fmSysDP.HideWhenClose = true;
            fmSysDP.TopMost = true;
            fmSysDP.VisibleChanged += SysDataPoolFormDisabled;
            AdjustStationList();

            //StationManager 消息挂靠
            JFStationManager mgr = JFHubCenter.Instance.StationMgr;
            mgr.EventStationWorkStatusChanged += OnStationWorkStatusChanged;
            mgr.EventStationCustomStatusChanged += OnStationCustomStatusChanged;
            mgr.EventStationProductFinished += OnStationProductFinished;
            mgr.EventStationCustomizeMsg += OnStationCustomizeMsg;
            mgr.EventStationTxtMsg += OnStationTxtMsg;
        }



        delegate void dgWorkStatusChanged(object sender, JFWorkStatus currWorkStatus);
        void OnStationWorkStatusChanged(object sender, JFWorkStatus currWorkStatus)
        {
            if (InvokeRequired)
            {
                Invoke(new dgWorkStatusChanged(OnStationWorkStatusChanged), new object[] { sender, currWorkStatus });
                return;
            }

            IJFStation station = sender as IJFStation;
            _dctStationForms[station].OnWorkStatusChanged(sender, currWorkStatus);
            if (_dctTestingForms.ContainsKey(station))
            {
                JFWorkStatus ws = station.CurrWorkStatus;
                if (!JFStationBase.IsWorkingStatus(ws) &&
                    ws != JFWorkStatus.CommandExit &&
                    ws != JFWorkStatus.AbortExit)
                {
                    _dctTestingForms.Remove(station);
                    StopTest();
                    JFTipsDelayClose.Show("工站:" + station.Name + " " + JFStationBase.WorkStatusName(ws) + "  ,停止测试运行!",-1);
                    return;
                }
            }



            
        }


        void OnStationCustomStatusChanged(object sender, int currCustomStatus)
        {
            _dctStationForms[sender as IJFStation].OnCustomStatusChanged(sender, currCustomStatus);
        }

        void OnStationTxtMsg(object sender, string txt)
        {
            _dctStationForms[sender as IJFStation].OnTxtMsg(sender, txt);
        }


         void OnStationProductFinished(object sender, int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo)
        {
            _dctStationForms[sender as IJFStation].OnProductFinished(sender, passCount, passIDs, ngCount, ngIDs, ngInfo);
        }


        void OnStationCustomizeMsg(object sender, string msgCategory, object[] msgParams)
        {
            //string info = "专属消息:Category = " + msgCategory + " ParamCount = " + (msgParams == null ? 0 : msgParams.Length);
            _dctStationForms[sender as IJFStation].OnCustomizeMsg(sender, msgCategory, msgParams);
        }




        /// <summary>
        /// 停止测试
        /// </summary>
        void StopTest()
        {
            foreach(KeyValuePair<IJFStation,FormSingleStationTest> kv in _dctTestingForms)
            {
                if (JFWorkCmdResult.Success != kv.Key.Stop(500))
                    kv.Key.Abort();
            }
            _isTesting = false;
            _dctTestingForms.Clear();
            btReset.Enabled = true;
            btResume.Enabled = false;
            btPause.Enabled = false;
        }




        void SysDataPoolFormDisabled(object sender,EventArgs e)
        {
            toolStripMenuItemSysTemDataPool.Checked = false;
        }

        string[] _currStationNames = null; //当前正在显示的工站列表


        /// <summary>
        /// 检查启动工站列表项
        /// </summary>
        void _CheckStationNamesInMenu() //
        {
            JFStationManager mgr = JFHubCenter.Instance.StationMgr;
            string[] allEnabledStaions = mgr.AllEnabledStationNames();
            if (allEnabledStaions == null || allEnabledStaions.Length == 0)
            {
                toolStripMenuItemStations.DropDownItems.Clear();
                return;
            }
            List<string> currStations = new List<string>(); //当前列表中的工站
            List<string> chkdStations = new List<string>();
            foreach (ToolStripMenuItem mi in toolStripMenuItemStations.DropDownItems)
            {
                currStations.Add(mi.Text);
                if (mi.Checked)
                    chkdStations.Add(mi.Text);
            }
            List<string> checkedStatios = new List<string>(); //当前已选中参与测试的工站
            if(allEnabledStaions.Length == currStations.Count)
            {
                bool isSame = true;
                for (int i = 0; i < allEnabledStaions.Length; i++)
                {
                    if (allEnabledStaions[i] != currStations[i])
                    {
                        isSame = false;
                        break;
                    }
                }
                if (isSame)
                    return;
            }
            toolStripMenuItemStations.DropDownItems.Clear();
            foreach(string s in allEnabledStaions)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(s, null, onToolStipItemStationClicked);
                toolStripMenuItemStations.DropDownItems.Add(mi);
                if (chkdStations.Contains(s))
                    mi.Checked = true;
            }




        }


        Dictionary<IJFStation,FormSingleStationTest> _dctStationForms = new Dictionary<IJFStation, FormSingleStationTest>();
        void AdjustStationList() //
        {
            JFStationManager mgr = JFHubCenter.Instance.StationMgr;
            string[] allStationNames = mgr.AllStationNames();
            if(null == allStationNames || 0 == allStationNames.Length)
            {
                _currStationNames = null;
                tabControlCF1.TabPages.Clear();
                toolStripMenuItemStations.DropDownItems.Clear();
                return;
            }

            if(null != _currStationNames && _currStationNames.Length == allStationNames.Length)
            {
                bool isSame = true;
                for(int i = 0; i < _currStationNames.Length;i++)
                    if(_currStationNames[i] != allStationNames[i])
                    {
                        isSame = false;
                        break;
                    }
                if (isSame)
                {
                    _CheckStationNamesInMenu();
                    return;
                }
            }
            _currStationNames = allStationNames;
           
            tabControlCF1.TabPages.Clear();
            _dctStationForms.Clear();
            foreach (string stationName in allStationNames)
            {
                TabPage tp = new TabPage(stationName);
                FormSingleStationTest fmStationTest = new FormSingleStationTest();
                fmStationTest.SetStation(mgr.GetStation(stationName));
                fmStationTest.FormBorderStyle = FormBorderStyle.None;
                fmStationTest.TopLevel = false;
                fmStationTest.ShowInTaskbar = false;
                fmStationTest.Dock = DockStyle.Fill;
                fmStationTest.Parent = tp;
                fmStationTest.Show();
                tp.Controls.Add(fmStationTest);
                tabControlCF1.TabPages.Add(tp);
                ToolStripMenuItem tsi = new ToolStripMenuItem(stationName, null, onToolStipItemStationClicked);
                toolStripMenuItemStations.DropDownItems.Add(tsi);
                _dctStationForms.Add(mgr.GetStation(stationName), fmStationTest);
            }
            _CheckStationNamesInMenu();

        }

        void onToolStipItemStationClicked(object sender,EventArgs e)
        {
            if(_isTesting)
            {
                MessageBox.Show("当前正在测试运行中，不能修改启动设置");
                return;
            }
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            string stationName = mi.Text;
            mi.Checked = !mi.Checked;



        }

        private void FormManual_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                AdjustStationList();
                if (toolStripMenuItemSysTemDataPool.Checked)
                    fmSysDP.Show();


                ///开始单工站界面刷新
                if (tabControlCF1.SelectedIndex > 0)
                    (tabControlCF1.SelectedTab.Controls[0] as FormSingleStationTest).EnableFlushStationUi = true;

            }
            else
            {
                JFStationManager mgr = JFHubCenter.Instance.StationMgr;
                fmSysDP.Hide(); //隐藏系统数据池显示
                if (tabControlCF1.SelectedIndex > 0)
                    (tabControlCF1.SelectedTab.Controls[0] as FormSingleStationTest).EnableFlushStationUi = false;

            }
        }

        FormDataPool fmSysDP = new FormDataPool();



   

        //所有选中参与测试的工站
        string[] TestStationNames()
        {
            List<string> ret = new List<string>();
            foreach (ToolStripMenuItem mi in toolStripMenuItemStations.DropDownItems)
                if (mi.Checked)
                    ret.Add(mi.Text);
            return ret.ToArray();
        }

        bool _isTesting = false; // 当前是否处于测试状态
        Dictionary<IJFStation, FormSingleStationTest> _dctTestingForms = new Dictionary<IJFStation, FormSingleStationTest>(); //参与当前测试的

        /// <summary>
        /// 启动所有选中的工站
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStart_Click(object sender, EventArgs e)
        {
            if(_isTesting)
            {
                MessageBox.Show("无效操作，当前测试正在运行");
                return;
            }

            string[] willTestStationNames = TestStationNames();
            if(null == willTestStationNames || 0 == willTestStationNames.Length)
            {
                MessageBox.Show("请先选择参与测试运行的工站！\n菜单->启动测试");
                return;
            }

            JFStationManager mgr = JFHubCenter.Instance.StationMgr;
            string[] allStationNames = mgr.AllStationNames();
            List<string> runningStationNames = new List<string>(); //当前正在运行的工站
            foreach (string s in allStationNames)
                if (JFStationBase.IsWorkingStatus(mgr.GetStation(s).CurrWorkStatus))
                    runningStationNames.Add(s);
            if(runningStationNames.Count > 0)
            {
                string info = "以下工站正在运行:\n";
                foreach(string s in runningStationNames)
                    info += s + "\n";
                info += "是否继续？";
                if (DialogResult.Cancel == MessageBox.Show(info, "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    return;
            }
            _isTesting = true;
            //mgr.EventStationWorkStatusChanged += OnStationWorkStatusChanged;
            _dctTestingForms.Clear();

            string Tipsinfo = "已启动工站:\n";
            foreach (string s in willTestStationNames)
            {
                IJFStation station = mgr.GetStation(s);
                _dctTestingForms.Add(station, _dctStationForms[station]);
                //_dctStationForms[station].EnableStationMsgReceive = true;
                JFWorkCmdResult ret = station.Start();
                if(ret != JFWorkCmdResult.Success) //启动工站失败
                {
                    //_dctStationForms[station].EnableStationMsgReceive = false;
                    foreach (KeyValuePair<IJFStation,FormSingleStationTest> kv in _dctTestingForms)
                    {
                        if (kv.Key.Stop(500) != JFWorkCmdResult.Success)
                            kv.Key.Abort();
                    }
                    _isTesting = false;
                    MessageBox.Show("启动工站:" + station.Name + "失败,ErrorCode = " + ret);
                    return;
                }

                Tipsinfo += s + "\n";

                JFTipsDelayClose.Show(Tipsinfo, 2);
                btReset.Enabled = false;
                btResume.Enabled = true;
                btPause.Enabled = true;
            }
        }

        /// <summary>
        /// 停止所有启动的工站
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStop_Click(object sender, EventArgs e)
        {
            StopTest();

        }

        private void tabControlCF1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage tpSel = tabControlCF1.SelectedTab;
            if (tpSel == null)
                return;
            
            foreach (TabPage tp in tabControlCF1.TabPages)
            {
                FormSingleStationTest fmStation = tp.Controls[0] as FormSingleStationTest;
                if (tp == tpSel)
                    fmStation.EnableFlushStationUi = true; //;//fmStation.Visible = true;
                else
                    fmStation.EnableFlushStationUi = false;
            }
        }

        /// <summary>
        /// 显示系统数据池界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemSysData_Click(object sender, EventArgs e)
        {
            if (toolStripMenuItemSysTemDataPool.Checked)
            {
                fmSysDP.Hide();
                toolStripMenuItemSysTemDataPool.Checked = false;
            }
            else
            {
                toolStripMenuItemSysTemDataPool.Checked = true;
                fmSysDP.Show();
            }
        }


        /// <summary>
        /// 暂停正在测试的工站
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPause_Click(object sender, EventArgs e)
        {

        }

        private void btResume_Click(object sender, EventArgs e)
        {

        }

        private void btEndBatch_Click(object sender, EventArgs e)
        {

        }

        private void btReset_Click(object sender, EventArgs e)
        {

        }
    }
}
