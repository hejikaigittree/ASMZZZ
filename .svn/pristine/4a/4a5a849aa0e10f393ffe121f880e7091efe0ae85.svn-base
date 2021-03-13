using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;
using JFUI;

namespace JFHub
{
    /// <summary>
    /// StationBase内使用的工站测试界面
    /// </summary>
    public partial class FormStationBaseAxisPanel : Form
    {
        public FormStationBaseAxisPanel()
        {
            InitializeComponent();
        }

        bool _isFormLoaded = false;
        private void FormAxisInStationBase_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            LayoutStation();
            //UcStandard4AxisPanel sap = new UcStandard4AxisPanel();
            //panelAxisOpt.Controls.Add(sap);
        }

        public void FormStationBaseAxisPanel_VisibleChanged(object sender, EventArgs e)
        {
            timerFlush.Enabled = Visible;
            if(Visible)
            {

            }
            else
            {

            }
        }

        private void FormStationBaseAxisPanel_FormClosed(object sender, FormClosedEventArgs e)
        {


        }

        private void FormStationBaseAxisPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MdiParent != null && CloseReason.MdiFormClosing != e.CloseReason)//在MDI中不关闭窗口
            {
                Hide();
                e.Cancel = true;
            }
        }
        int _maxTips = 500;
        delegate void dgShowTips(string info);
        public void ShowTips(string info)
        {
            if (InvokeRequired)
            {
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

        private void btClearTips_Click(object sender, EventArgs e)
        {
            rchTips.Text = "";
        }

        private void timerFlush_Tick(object sender, EventArgs e)
        {
            UpdateStationUI();
        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
            if (_isFormLoaded)
                LayoutStation();
        }

        UcStandard4AxisPanel _ucS4AxisPanel = new UcStandard4AxisPanel(); //四轴操作界面
        List<UcSimpleAxisInStation> _lstSimpleAxisPanels = new List<UcSimpleAxisInStation>();
        //List<UcAxisStatus> _lstAxisStatus = new List<UcAxisStatus>(); //轴状态控件

        public void LayoutStation()
        {
            panelAxisOpt.Controls.Clear();
            panelAxisStatus.Controls.Clear();
            _lstSimpleAxisPanels.Clear();
            //_lstAxisStatus.Clear();
            if (null == _station)
            {
                ShowTips("工站未设置");
                btMoveToWorkPos.Enabled = false;
                btSaveCurr2WorkPos.Enabled = false;
                cbMoveWorkPos.Enabled = false;
                cbSaveWorkPos.Enabled = false;
                btOpenAllDev.Enabled = false;
                return;
            }
            btOpenAllDev.Enabled = true;
            btMoveToWorkPos.Enabled = true;
            btSaveCurr2WorkPos.Enabled = true;
            cbMoveWorkPos.Enabled = true;
            cbMoveWorkPos.Items.Clear();
            cbSaveWorkPos.Enabled = true;
            cbSaveWorkPos.Items.Clear();
            if (null != _station.WorkPositionNames)
            {
                cbMoveWorkPos.Items.AddRange(_station.WorkPositionNames);
                cbSaveWorkPos.Items.AddRange(_station.WorkPositionNames);
            }
            if (null != _station.AxisNames && _station.AxisNames.Length > 0)
            {
                if (_station.StandardAxisCount > 0) //添加一个标准4轴控制面板
                {
                    string[] standardAxisNames = _station.AxisNames.Take(_station.StandardAxisCount).ToArray();
                    _ucS4AxisPanel.Top = 0;
                    _ucS4AxisPanel.SetAxisNames(standardAxisNames);
                    panelAxisOpt.Controls.Add(_ucS4AxisPanel);
                }
                int bottom = _ucS4AxisPanel.Bottom;
                //添加剩下的单轴操作面板
                for (int i= _station.StandardAxisCount;i< _station.AxisNames.Length;i++)
                {
                    UcSimpleAxisInStation ucSAS = new UcSimpleAxisInStation();
                    ucSAS.SetAxisName(_station.AxisNames[i]);
                    _lstSimpleAxisPanels.Add(ucSAS);
                    ucSAS.Top = bottom + 3;
                    bottom = ucSAS.Bottom;
                    panelAxisOpt.Controls.Add(ucSAS);
                }
                bottom = 0;
                for (int i = 0; i < _station.AxisNames.Length;i++ )
                {
                    UcAxisStatusIS ucStatus = new UcAxisStatusIS();
                    ucStatus.Top = bottom;
                    ucStatus.SetAxisName(_station.AxisNames[i]);
                    //_lstAxisStatus.Add(ucStatus);
                    panelAxisStatus.Controls.Add(ucStatus);
                    bottom = ucStatus.Bottom;
                    //ucStatus.SetAxis()
                }
                
            }
        }

        /// <summary>
        /// 移动到指定工位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btMoveToWorkPos_Click(object sender, EventArgs e)
        {
            if(cbMoveWorkPos.SelectedIndex < 0)
            {
                MessageBox.Show("请选择目标工作点位");
                return;
            }
            
            string errorInfo;
            bool isOK = false;
            if (chkLine.Checked)
                isOK = _station.LineToWorkPosition(cbMoveWorkPos.Text, out errorInfo);
            else
                isOK= _station.MoveToWorkPosition(cbMoveWorkPos.Text, out errorInfo);
            if(!isOK)
            {
                MessageBox.Show("移动到工作点位:\"" + cbMoveWorkPos.Text + "\"失败，ErrorInfo:" + errorInfo);
                return;
            }
            ShowTips("开始移动到工作点位:" + cbMoveWorkPos.Text);

        }

        /// <summary>
        /// 将当前位置保存为指定工位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveCurr2WorkPos_Click(object sender, EventArgs e)
        {
            if (cbSaveWorkPos.SelectedIndex < 0)
            {
                MessageBox.Show("请选择需要存储的工作点位");
                return;
            }
            JFMultiAxisPosition pos = _station.GetWorkPosition(cbSaveWorkPos.Text);
            if(null == pos)
            {
                MessageBox.Show(" 未找到点位信息,Name = " + cbSaveWorkPos.Text);
                return;
            }
            string[] axisNames = pos.AxisNames;
            if (null == axisNames || 0 == axisNames.Length)
            {
                MessageBox.Show("未能保存，点位中不包含轴/电机！");
                return;
            }
            List<double> lstAxisPos = new List<double>();
            foreach(string axisName in axisNames)
            {
                double apos = 0;
                string errorInfo;
                if(!_station.GetAxisPosition(axisName,out apos,out errorInfo))
                {
                    MessageBox.Show("获取轴 = \"" + axisName + "\"位置失败，ErrorInfo ：" + errorInfo);
                    return;
                }
                lstAxisPos.Add(apos);
            }
            if (DialogResult.Cancel == MessageBox.Show("确定将当前位置保存为点位:" + cbSaveWorkPos.Text, "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                return;
            for (int i = 0; i < axisNames.Length; i++)
                pos.SetAxisPos(pos.AxisNames[i], lstAxisPos[i]);
            _station.SaveCfg();
            MessageBox.Show("点位坐标已保存");
        }

        /// <summary>
        /// 打开所有轴设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOpenAllDev_Click(object sender, EventArgs e)
        {
            if(null == _station)
                return;
            string[] axisNames = _station.AxisNames;
            if(null == axisNames || 0 == axisNames.Length)
            {
                ShowTips("工站 未提供轴/电机！");
                return;
            }
            bool isOK = true;
            foreach(string axisName in axisNames)
            {
                JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(axisName);
                if (null == ci)
                {
                    isOK = false;
                    ShowTips("未发现轴 = \"" + axisName + "\"所属 通道信息！");
                    continue;
                }
                IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
                if (null == dev)
                {
                    isOK = false;
                    ShowTips("未发现轴 = \"" + axisName + "\"所属 设备ID = \"" + ci.DeviceID + "\"!");
                    continue;
                }
                if (!dev.IsDeviceOpen)
                {
                    int errCode = dev.OpenDevice();
                    if (0 != errCode)
                    {
                        isOK = false;
                        ShowTips(string.Format("打开轴 = \"{0}\"所属设备=\"{1}\"失败，错误信息:{2}", axisName, ci.DeviceID, dev.GetErrorInfo(errCode)));
                        continue;
                    }
                    ShowTips(string.Format("轴 = \"{0}\"所属设备=\"{1}\"已打开 ", axisName, ci.DeviceID));
                }
            }
            if (isOK)
                ShowTips("所有轴设备已打开");
            LayoutStation();
        }

        void UpdateStationUI()
        {
            if (null == _station)
                return;
            _ucS4AxisPanel.UpdateStationUI();
            foreach (UcSimpleAxisInStation usa in _lstSimpleAxisPanels)
                usa.UpdateAxisUI();
            foreach(Control ctrl in panelAxisStatus.Controls)
            {
                UcAxisStatusIS uas = ctrl as UcAxisStatusIS;
                uas.UpdateAxisStatus();
            }


        }
    }
}
