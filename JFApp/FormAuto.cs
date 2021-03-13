using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFHub;
using JFInterfaceDef;
using JFUI;

namespace JFApp
{
    /// <summary>
    /// 生产运行时的主界面
    /// </summary>
    public partial class FormAuto : Form
    {
        public FormAuto()
        {
            InitializeComponent();
        }

        private void FormAuto_Load(object sender, EventArgs e)
        {
            IJFMainStation mainStation = JFHubCenter.Instance.StationMgr.MainStation;
            if(null == mainStation)
            {
                MessageBox.Show("MainStation is not Regist,App will Exit");
                Application.Exit();
            }
            Control mainStationPanel = mainStation.UIPanel;
            mainStationPanel.Dock = DockStyle.Fill;
            //mainStationPanel.Parent = splitContainer1.Panel2;
            splitContainer1.Panel2.Controls.Add(mainStationPanel);
            mainStationPanel.Show();
            AdjustView();
        }


        string[] _currStations = null; //当前正在显示的工站


        /// <summary>
        /// 根据当前已激活的工站，布局界面
        /// </summary>
        public void AdjustView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustView));
                return;
            }
            JFStationManager stationMgr = JFHubCenter.Instance.StationMgr;
            string[] allEnabledStationName = stationMgr.AllEnabledStationNames();
            if (allEnabledStationName == null || allEnabledStationName.Length == 0)
            {
                _currStations = null;
                pnStations.Controls.Clear();
                return;
            }

            if(_currStations != null)
            {
                if(_currStations.Length == allEnabledStationName.Length)
                {
                    bool isSame = true;
                    for(int i = 0; i < _currStations.Length;i++)
                        if(_currStations[i] != allEnabledStationName[i])
                        {
                            isSame = false;
                            break;
                        }
                    if (isSame) //不需要更新界面
                        return;
                }
            }
            _currStations = allEnabledStationName;
            //将当前工站界面和消息回调解除绑定
            foreach (Control ui in pnStations.Controls)
                stationMgr.RemoveStationMsgReciever(ui as IJFStationMsgReceiver);
            pnStations.Controls.Clear();
            foreach (string enabledStationName in allEnabledStationName)
            {
                IJFStation station = stationMgr.GetStation(enabledStationName);
                //JFRealtimeUI ui = station.GetRealtimeUI(); //如果station不提供界面，则提供一个默认的
                //if(null == ui)
                UcStationRealtimeUI ui = new UcStationRealtimeUI();
                ui.JfDisplayMode = UcStationRealtimeUI.JFDisplayMode.simple;
                ui.SetStation(station);
                stationMgr.AppendStationMsgReceiver(station, ui);
                pnStations.Controls.Add(ui);
            }
        }

        private void FormAuto_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                AdjustView();
            }

        }
    }
}
