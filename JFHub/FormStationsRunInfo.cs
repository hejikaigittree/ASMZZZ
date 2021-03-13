using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFHub
{
    /// <summary>
    /// 显示工站的运行信息
    /// </summary>
    public partial class FormStationsRunInfo : Form
    {
        public FormStationsRunInfo()
        {
            InitializeComponent();
            _dctStationInfos = new Dictionary<IJFStation, RichTextBox>();
        }

        int _gatherTipsMaxBytes = 1000000; //汇总信息最大数量
        int _stationTipsMaxBytes = 100000;

        private void FormStationsRunInfo_Load(object sender, EventArgs e)
        {
            IsHideWhenFormClose = true;
            AdjustStationList();
            JFStationManager mgr = JFHubCenter.Instance.StationMgr;
            mgr.EventStationWorkStatusChanged += OnStationWorkStatusChanged;
            mgr.EventStationCustomStatusChanged += OnStationCustomStatusChanged;
            mgr.EventStationTxtMsg += OnStationTxtMsg;
            mgr.EventStationProductFinished += OnStationProductFinished;
            mgr.EventStationCustomizeMsg += OnStationCustomizeMsg;


        }

        Dictionary<IJFStation, RichTextBox> _dctStationInfos = new Dictionary<IJFStation, RichTextBox>();

        bool _isAdjusting = false;
        void AdjustStationList()
        {
            _isAdjusting = true;
            while (tabControl1.TabCount > 1)
                tabControl1.TabPages.RemoveAt(1);
            _dctStationInfos.Clear();
            JFStationManager mgr = JFHubCenter.Instance.StationMgr;
            string[] stationNames = mgr.AllStationNames();
            if (null == stationNames)
                return;
            foreach (string sn in stationNames)
            {
                TabPage tp = new TabPage(sn);
                tabControl1.TabPages.Add(tp);

                IJFStation station = mgr.GetStation(sn);
                RichTextBox rchInfos = new RichTextBox();
                rchInfos.Tag = sn;
                _dctStationInfos.Add(station, rchInfos);
                Button btClear = new Button();
                btClear.Text = "清空信息";
                btClear.Click += OnBtStationClear_Click;
                btClear.Tag = rchInfos;
                btClear.Location = new Point(tp.Width - 3 - btClear.Width, 3);
                btClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                rchInfos.Location = new Point(3, btClear.Bottom + 3);
                rchInfos.Width = tp.Width - 6;
                rchInfos.Height = tp.Height - 3 - rchInfos.Top;
                rchInfos.ReadOnly = true;
                rchInfos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
                tp.Controls.Add(btClear);
                tp.Controls.Add(rchInfos);


            }

            _isAdjusting = false;
        }





        delegate void dgOnStationWorkStatusChanged(object station, JFWorkStatus currWorkStatus);
        void OnStationWorkStatusChanged(object station, JFWorkStatus currWorkStatus)
        {
            if (_isAdjusting)
                return;
            if (!Created)
                return;
            if (!_dctStationInfos.ContainsKey(station as IJFStation))
                return;
            if (InvokeRequired)
            {
                Invoke(new dgOnStationWorkStatusChanged(OnStationWorkStatusChanged), new object[] { station, currWorkStatus });
                return;
            }

            string timeTxt = DateTime.Now.ToString("HH:mm:ss.fff");
            string info = "运行状态:" + JFStationBase.WorkStatusName(currWorkStatus);
            RichTextBox rchStation = _dctStationInfos[station as IJFStation];
            rchStation.AppendText( timeTxt + " " + info + "\n");
            if (rchStation.Text.Length >= _stationTipsMaxBytes)
                rchStation.Text = rchStation.Text.Substring(_stationTipsMaxBytes / 2);
            //     string[] lines = rbTips.Text.Split('\n');
            //if (lines.Length >= maxTips)
            //{
            //    int rmvChars = 0;
            //    for (int i = 0; i < lines.Length - maxTips; i++)
            //        rmvChars += lines[i].Length + 1;
            //    rbTips.Text = rbTips.Text.Substring(rmvChars);
            //}
            rchStation.Select(rchStation.TextLength, 0); //滚到最后一行
            rchStation.ScrollToCaret();//滚动到控件光标处 

        }

        delegate void dgStationCustomStatusChanged(object station, int currCustomStatus);
        void OnStationCustomStatusChanged(object station, int currCustomStatus)
        {
            if (_isAdjusting)
                return;
            if (!Created)
                return;
            if (!_dctStationInfos.ContainsKey(station as IJFStation))
                return;
        }

        delegate void dgStationTxtMsg(object station, string msgInfo);
        void OnStationTxtMsg(object station,string msgInfo)
        {
            if (_isAdjusting)
                return;
            if (!Created)
                return;
            if (!_dctStationInfos.ContainsKey(station as IJFStation))
                return;
        }

        delegate void dgStationCustomizeMsg(object station, string msgCategory, object[] msgParams);
        void OnStationCustomizeMsg(object station, string msgCategory, object[] msgParams)
        {
            if (_isAdjusting)
                return;
            if (!Created)
                return;
            if (!_dctStationInfos.ContainsKey(station as IJFStation))
                return;
        }

        delegate void dgStationProductFinished(object station, int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo);
        void OnStationProductFinished(object station, int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo)
        {
            if (_isAdjusting)
                return;
            if (!Created)
                return;
            if (!_dctStationInfos.ContainsKey(station as IJFStation))
                return;
        }




        /// <summary>
        /// 用隐藏代替关闭窗口
        /// </summary>
        public bool IsHideWhenFormClose { get; set; }

        ///清空汇总信息
        private void btGatherClear_Click(object sender, EventArgs e)
        {
            rchGatherInfo.Text = "";
        }


        /// <summary>
        /// 清空工站信息列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBtStationClear_Click(object sender, EventArgs e)
        {
            ((sender as Button).Tag as RichTextBox).Text = "";
        }

        private void FormStationsRunInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(IsHideWhenFormClose)
            {
                e.Cancel = true;
                Hide();
                return;
            }

            e.Cancel = false;
        }


        /// <summary>
        /// 更新当前工站列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUpdateStations_Click(object sender, EventArgs e)
        {
            AdjustStationList();
        }
    }
}
