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

namespace JFHub
{
    public partial class FormStationCfgMgr : Form
    {
        public FormStationCfgMgr()
        {
            InitializeComponent();
        }

        private void FormStationCfgMgr_Load(object sender, EventArgs e)
        {
        }

        List<string> _locStationNames = new List<string>();
        string _selectStationName = null;

        bool IsNeedReLoad(List<string> stationNames)
        {
            if (_locStationNames.Count != stationNames.Count)
                return true;
            for (int i = 0; i < _locStationNames.Count; i++)
                if (_locStationNames[i] != stationNames[i])
                    return true;
            return false;
        }

        void UpdateStations()
        {
            List<string> currStationNames = JFHubCenter.Instance.StationMgr.AllStationNames().ToList();
            if (!IsNeedReLoad(currStationNames))
                return;
            _locStationNames = currStationNames;
            tabControlCF1.TabPages.Clear();
            foreach(string sn in _locStationNames)
            {
                IJFStation station = JFHubCenter.Instance.StationMgr.GetStation(sn);
                if(typeof(JFStationBase).IsAssignableFrom(station.GetType()))
                {
                    FormStationBaseCfg fm = new FormStationBaseCfg();
                    fm.SetStation(station as JFStationBase);
                    AddPage(fm, sn);
                }
                else //使用IJFStaion的ConfigUIProvider
                {
                    TabPage tabPage = new TabPage(); //创建一个TabControl 中的单个选项卡页。
                    tabPage.Text = sn;
                    tabPage.Name = sn;
                    tabPage.BackColor = BackColor;
                    tabPage.Font = Font;
                    tabControlCF1.TabPages.Add(tabPage);   //添加tabPage选项卡到tab控件

                    Button bt = new Button();
                    bt.Text = "显示工站配置窗口";
                    bt.Tag = sn;
                    bt.Click += OnBtShowStationCfgUI;
                    tabPage.Controls.Add(bt);  //tabPage选项卡添加一个窗体对象 
                }

            }
            if(_selectStationName == null)
            {
                if(tabControlCF1.TabPages.Count > 0)
                {
                    tabControlCF1.SelectedIndex = -1;
                    tabControlCF1.SelectedIndex = 0;
                    _selectStationName = tabControlCF1.TabPages[0].Text;
                }
            }
            else
            {
                for(int i = 0; i < _locStationNames.Count;i++)
                {
                    if(_selectStationName == _locStationNames[i])
                    {
                        tabControlCF1.SelectedIndex = i;
                        return;
                    }
                }
                if(0 == _locStationNames.Count)
                {
                    _selectStationName = null;
                }
                else
                {
                    _selectStationName = _locStationNames[0];
                    tabControlCF1.SelectedIndex = 0;
                }


            }
        }

        void OnBtShowStationCfgUI(object sender,EventArgs e)
        {
            Button bt = sender as Button;
            string stationName = bt.Tag as string;
            IJFStation  station= JFHubCenter.Instance.StationMgr.GetStation(stationName);
            station.ShowCfgDialog();
        }


        void AddPage(Form frm, string pageName)
        {
            TabPage tabPage = new TabPage(); //创建一个TabControl 中的单个选项卡页。
            if (string.IsNullOrEmpty(pageName))
                pageName = frm.Text;
            tabPage.Text = pageName;
            tabPage.Name = pageName;
            tabPage.BackColor = frm.BackColor;
            tabPage.Font = frm.Font;

            tabControlCF1.TabPages.Add(tabPage);   //添加tabPage选项卡到tab控件

            frm.FormBorderStyle = FormBorderStyle.None;
            frm.TopLevel = false;
            frm.Dock = DockStyle.Fill;
            frm.Parent = tabPage;

            tabPage.Controls.Add(frm);  //tabPage选项卡添加一个窗体对象 
            frm.Visible = false;
        }

        private void FormStationCfgMgr_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                UpdateStations();
            }
            else
            {

            }
        }

        private void tabControlCF1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //当前页正在编辑时，取消页面变更
            //待添加代码
        }

        private void tabControlCF1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null == tabControlCF1.SelectedTab)
                return;

            if (tabControlCF1.SelectedTab.HasChildren)
                foreach (Control item in tabControlCF1.SelectedTab.Controls)
                    if (item is Form)
                        item.Visible = true;
        }


    }
}
