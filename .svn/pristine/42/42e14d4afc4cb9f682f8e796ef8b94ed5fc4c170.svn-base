using JFInterfaceDef;
using JFUI;
using JFVision;
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
    public partial class FormStationBaseCmrPanel : Form
    {
        public FormStationBaseCmrPanel()
        {
            InitializeComponent();
            _tabCtrl.Alignment = TabAlignment.Right;
            //_tabCtrl.Dock = DockStyle.Fill;
            _tabCtrl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
            _tabCtrl.DrawMode = TabDrawMode.OwnerDrawFixed;
            _tabCtrl.ItemSize = new Size(35, 150);
            _tabCtrl.Location = new Point(0, menuStrip2.Height + 2);
            _tabCtrl.Width = Width-3;
            _tabCtrl.Height = Height-3;
            _tabCtrl.Multiline = true;
            _tabCtrl.SelectedIndex = 0;
            _tabCtrl.SizeMode = TabSizeMode.Fixed;
            _tabCtrl.TabColor = SystemColors.ControlDark;
            _tabCtrl.TabIndex = 0;
            _tabCtrl.SelectedIndexChanged += new System.EventHandler(TabCtrl_SelectedIndexChanged);
        }

        public void FormStationBaseCmrPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                if(null != _station)
                    SetCmrNames(_station.CameraNames);
            }
            else
            {
                ///添加代码，关闭正在连续采集的相机
            }
        }

        string[] _cmrNames = null;
        bool isFormLoaded = false;
        private void FormStationBaseCmrPanel_Load(object sender, EventArgs e)
        {
            isFormLoaded = true;
            AdjustView();
        }

        private void FormStationBaseCmrPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MdiParent != null && CloseReason.MdiFormClosing != e.CloseReason)//在MDI中不关闭窗口
            {
                Hide();
                e.Cancel = true;
            }
        }
        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
            if (null == _station)
                SetCmrNames(null);
            else
                SetCmrNames(_station.CameraNames);
        }

        public void SetCmrNames(string[] cmrNames)
        {
            if(_cmrNames == null || _cmrNames.Length == 0)
            {
                if (cmrNames == null || 0 == cmrNames.Length)
                    return;
            }
            else
            {
                if (cmrNames != null && cmrNames.Length == _cmrNames.Length)
                {
                    bool isSame = true;
                    for (int i = 0; i < cmrNames.Length;i++)
                        if(cmrNames[i] != _cmrNames[i])
                        {
                            isSame = false;
                            break;
                        }
                    if (isSame)
                        return;
                }

            }
            _cmrNames = cmrNames;
            if (isFormLoaded)
                AdjustView();
        }
        #region 标签显示模式下需要的变量
        List<Button> _lstUpdateBts = new List<Button>(); //用于更新相机界面的按钮们
        List<Label> _lstCmrStatusLbs = new List<Label>(); //显示相机状态的按钮们
        List<UcCmr> _lstUcCmrs = new List<UcCmr>();
        TabControlCF _tabCtrl = new TabControlCF(); //标签显示模式的容器 ， 在MDI显示模式时需要隐藏（不使用）
        #endregion
        int _showMode = 0; //显示模式 0：标签式    1：子窗口模式
        /// <summary>
        /// 设置相机名称列表后，重新布局界面
        /// </summary>
        public void AdjustView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustView));
                return;
            }
            if (0 == _showMode)
                ShowTab();
            else if (1 == _showMode)
                ShowMdi();
        }
             
        /// <summary>
        /// 标签显示模式
        /// </summary>
        void ShowTab()
        {
            //先要清除Mdi模式，待添加代码
            if (!Controls.Contains(_tabCtrl))//Add(_tabCtrl);
                Controls.Add(_tabCtrl);

            cmrLstToolStripMenuItem.Visible = false; //MDI模式下使用
            _lstUpdateBts.Clear();
            _lstCmrStatusLbs.Clear();
            _lstUcCmrs.Clear();
            _tabCtrl.TabPages.Clear();
            if (null == _cmrNames)
                return;
            for(int i = 0; i < _cmrNames.Length;i++)
            {
                TabPage tp = new TabPage(_cmrNames[i]);
                Button btFlush = new Button();
                btFlush.Text = "更新";
                btFlush.Click += OnFlushButtonClick;
                _lstUpdateBts.Add(btFlush);
                Label lbInfo = new Label();
                //lbInfo.Left = btFlush.Right + 5;
                lbInfo.Location = new Point(btFlush.Right + 5, btFlush.Top + 5);
                lbInfo.Text = "相机:" + _cmrNames[i];
                lbInfo.AutoSize = true;
                IJFDevice_Camera cmr = JFHubCenter.Instance.InitorManager.GetInitor(_cmrNames[i]) as IJFDevice_Camera;
                if (null == cmr)
                    lbInfo.Text += " 在设备列表中不存在";
                _lstCmrStatusLbs.Add(lbInfo);
                UcCmr ucCmr = new UcCmr();
                ucCmr.SetCamera(cmr);
                ucCmr.Top = btFlush.Bottom + 5;
                ucCmr.Width = tp.Width - 5;
                ucCmr.Height = tp.Height - 5;
                ucCmr.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                _lstUcCmrs.Add(ucCmr);
                tp.Controls.Add(btFlush);
                tp.Controls.Add(lbInfo);
                tp.Controls.Add(ucCmr);
                _tabCtrl.TabPages.Add(tp);

            }


        }

        void OnFlushButtonClick(object sender,EventArgs e)
        {
            for (int i = 0; i < _lstUpdateBts.Count; i++)
                if (sender == _lstUpdateBts[i])
                {
                    _lstCmrStatusLbs[i].Text = "相机:" + _cmrNames[i];
                    IJFDevice_Camera cmr = JFHubCenter.Instance.InitorManager.GetInitor(_cmrNames[i]) as IJFDevice_Camera;
                    if(null == cmr)
                        _lstCmrStatusLbs[i].Text += " 在设备列表中不存在";
                    _lstUcCmrs[i].SetCamera(cmr);
                }
        }

        void TabCtrl_SelectedIndexChanged(object sender,EventArgs e)
        {
            if (_tabCtrl.SelectedIndex < 0)
                return;
            //for (int i = 0; i < _tabCtrl.TabCount; i++)
            //{
            //    if (i == _tabCtrl.SelectedIndex)
            //    {
            //        _lstCmrStatusLbs[i].Visible = true;
            //        _lstUpdateBts[i].Visible = true;
            //        _lstUcCmrs[i].Visible = true;
            //    }
            //    else
            //    {
            //        _lstCmrStatusLbs[i].Visible = false;
            //        _lstUpdateBts[i].Visible = false;
            //        _lstUcCmrs[i].Visible = false;
            //    }
            //}
        }

        /// <summary>
        /// 子窗口显示模式
        /// </summary>
        void ShowMdi()
        {
            //待实现 .....
            //先要清楚Tab模式，带添加代码
            cmrLstToolStripMenuItem.Visible = true;
        }

        /// <summary>
        /// 标签方式显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_showMode == 0)
                return;
            mDIToolStripMenuItem.Checked = false;
            tabToolStripMenuItem.Checked = true;
            _showMode = 0;
            ShowTab();

        }

        /// <summary>
        /// 子窗口显示模式（用于同时显示多个相机画面）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mDIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_showMode == 1)
                return;
            mDIToolStripMenuItem.Checked = true;
            tabToolStripMenuItem.Checked = false;
            _showMode = 1;
            ShowMdi();
        }
    }
}
