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
    /// 工站使能管理
    /// </summary>
    public partial class FormStationEnables : Form
    {
        public FormStationEnables()
        {
            InitializeComponent();
            IsEditting = false;
        }

        private void FormStationEnables_Load(object sender, EventArgs e)
        {

        }

        List<string> _locStationNames = new List<string>();

        private void FormStationEnables_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                UpdateStations();
            }
            else
            {

            }
        }

        bool IsNeedReload(List<string> stations)
        {
            if (stations.Count != _locStationNames.Count)
                return true;
            for (int i = 0; i < _locStationNames.Count; i++)
                if (_locStationNames[i] != stations[i])
                    return true;
            return false;
        }

        void AdjustStations(List<string> stations)
        {
            tableLayoutPanel1.Controls.Clear();
            _locStationNames = stations;
            if(_locStationNames.Count == 0)
            {
                IsEditting = false;
                btCancel.Enabled = false;
                btSetSave.Enabled = false;
                label1.Text = "工站列表:系统中无工站！";
                return;
            }
            else
            {
                label1.Text = "工站列表:";
                if (!IsEditting)
                {
                    btCancel.Enabled = false;
                    btSetSave.Enabled = true;
                    btSetSave.Text = "编辑";
                }
                //tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                foreach (string stationName in _locStationNames)
                {
                    //tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); //无效
                    CheckBox cb = new CheckBox();
                    cb.Height = 25;
                    cb.Text = stationName;
                    cb.Enabled = IsEditting;
                    tableLayoutPanel1.Controls.Add(cb);
                }
            }
        }

        /// <summary>
        /// 更新工站列表
        /// </summary>
        public void UpdateStations()
        {
            List<string> allStationNames = JFHubCenter.Instance.StationMgr.AllStationNames().ToList();
            if (!IsNeedReload(allStationNames))
                return;
            AdjustStations(allStationNames);
            LoadCfg();
        }

        void SaveCfg()
        {
            JFStationManager mgr = JFHubCenter.Instance.StationMgr;
            foreach(CheckBox cb in tableLayoutPanel1.Controls)
                mgr.SetStationEnabled(cb.Text, cb.Checked);
            

        }

        void LoadCfg()
        {
            JFStationManager mgr = JFHubCenter.Instance.StationMgr;
            foreach (CheckBox cb in tableLayoutPanel1.Controls)
                cb.Checked = mgr.GetStationEnabled(cb.Text);
        }

        public bool IsEditting { get; private set; }

        private void btSetSave_Click(object sender, EventArgs e)
        {
            if(!IsEditting)
            {
                IsEditting = true;
                foreach (CheckBox cb in tableLayoutPanel1.Controls)
                    cb.Enabled = true;
                btSetSave.Text = "保存";
                btCancel.Enabled = true;
            }
            else
            {
                SaveCfg();
                IsEditting = false;
                foreach (CheckBox cb in tableLayoutPanel1.Controls)
                    cb.Enabled = false;
                btSetSave.Text = "编辑";
                btCancel.Enabled = false;
            }
        }



        private void btCancel_Click(object sender, EventArgs e)
        {
            LoadCfg();
            IsEditting = false;
            foreach (CheckBox cb in tableLayoutPanel1.Controls)
                cb.Enabled = false;
            btSetSave.Text = "编辑";
            btCancel.Enabled = false;
        }
    }
}
