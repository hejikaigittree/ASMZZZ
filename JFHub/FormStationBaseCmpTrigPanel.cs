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
    public partial class FormStationBaseCmpTrigPanel : Form
    {
        public FormStationBaseCmpTrigPanel()
        {
            InitializeComponent();
        }

        public void FormStationBaseCmpTrigPanel_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                if (null != _station)
                    SetTrigNames(_station.CmpTrigNames);
            }
            else
            {

            }
        }

        bool isFormLoaded = false;
        private void FormStationBaseCmpTrigPanel_Load(object sender, EventArgs e)
        {
            isFormLoaded = true;
            AdjustView();
        }

        private void FormStationBaseCmpTrigPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MdiParent != null && CloseReason.MdiFormClosing != e.CloseReason)//在MDI中不关闭窗口
            {
                Hide();
                e.Cancel = true;
            }
        }
        List<Label> _lstCmpTrigInfos = new List<Label>();
        List<UcCmprTrgChn> _lstCmpTrigUcs = new List<UcCmprTrgChn>();
        string[] _trigNames = null;
        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
            if (null == _station)
                SetTrigNames(null);
            else
                SetTrigNames(_station.CmpTrigNames);
        }

        public void SetTrigNames(string[] cmpTrigNames)
        {
            if (_trigNames == null || _trigNames.Length == 0)
            {
                if (cmpTrigNames == null || 0 == cmpTrigNames.Length)
                    return;
            }
            else
            {
                if (cmpTrigNames != null && cmpTrigNames.Length == _trigNames.Length)
                {
                    bool isSame = true;
                    for (int i = 0; i < cmpTrigNames.Length; i++)
                        if (cmpTrigNames[i] != _trigNames[i])
                        {
                            isSame = false;
                            break;
                        }
                    if (isSame)
                        return;
                }
            }
            _trigNames = cmpTrigNames;
            if (isFormLoaded)
                AdjustView();
        }

        JFDevCellInfo CheckTrigDevInfo(string trigName, out string errorInfo)
        {

            JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetCmpTrigCellInfo(trigName); //在命名表中的通道信息
            if (null == ci)
            {
                errorInfo = "未找到设备信息";
                return null;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            if (null == dev)
            {
                errorInfo = "未找到所属设备:\"" + ci.DeviceID + "\"";
                return null;
            }
            if (!dev.IsDeviceOpen)
            {
                errorInfo = "所属设备:\"" + ci.DeviceID + "\"未打开";
                return null;
            }
            if (ci.ModuleIndex >= dev.McCount)
            {
                errorInfo = "模块序号:\"" + ci.ModuleIndex + "\"超出限制!";
                return null;
            }
            IJFModule_CmprTrigger md = dev.GetCompareTrigger(ci.ModuleIndex);
            if (ci.ChannelIndex >= md.EncoderChannels)
            {
                errorInfo ="通道序号:\"" + ci.ChannelIndex + "\"超出限制!";
                return null;
            }

            errorInfo = "";
            return ci;
        }

        /// <summary>
        /// 更新界面
        /// </summary>
        void AdjustView()
        {
            Controls.Clear();
            _lstCmpTrigInfos.Clear();
            _lstCmpTrigUcs.Clear();
            if (null == _trigNames || 0 == _trigNames.Length)
                return;
            Point loc = new Point(5, 5);
            for(int i = 0; i < _trigNames.Length;i++)
            {
                Label lbInfo = new Label();
                lbInfo.AutoSize = true;
                lbInfo.Text = _trigNames[i];
                string errInfo;
                JFDevCellInfo ci = CheckTrigDevInfo(_trigNames[i], out errInfo);
                if(null == ci)
                    lbInfo.Text += " ,ErrorInfo:" + errInfo;
                lbInfo.Location = loc;//lbInfo.Top = bottom;
                loc.Y = lbInfo.Bottom;//bottom = lbInfo.Bottom +5;
                Controls.Add(lbInfo);
                _lstCmpTrigInfos.Add(lbInfo);

                UcCmprTrgChn ucTrig = new UcCmprTrgChn();
                if (null == ci)
                {
                    ci = JFHubCenter.Instance.MDCellNameMgr.GetCmpTrigCellInfo(_trigNames[i]); //尝试使用命名表中的名称
                    if(ci == null)
                        ucTrig.SetModuleChn(null, 0, _trigNames[i] + ":无效通道名", null);
                    else
                        ucTrig.SetModuleChn(null, ci.ChannelIndex, _trigNames[i], null);
                }
                else
                {
                    IJFModule_CmprTrigger md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetCompareTrigger(ci.ModuleIndex);
                    ucTrig.SetModuleChn(md, ci.ChannelIndex, _trigNames[i], null);
                }
                ucTrig.Location = loc;
                ucTrig.BackColor = SystemColors.ActiveBorder;
                Controls.Add(ucTrig);
                _lstCmpTrigUcs.Add(ucTrig);
                loc.Y = ucTrig.Bottom + 20;//bottom = ucTrig.Bottom + 10;
            }
        }
    }
}
