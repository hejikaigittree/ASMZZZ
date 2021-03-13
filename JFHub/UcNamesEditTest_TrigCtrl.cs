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
using JFHub;
using JFUI;

namespace JFHub
{
    /// <summary>
    /// 触发控制器（HTM光源触发板，不是位置比较触发器）
    /// </summary>
    public partial class UcNamesEditTest_TrigCtrl : UserControl
    {
        public UcNamesEditTest_TrigCtrl()
        {
            InitializeComponent();
        }

        private void UcNamesEditTest_TrigCtrl_Load(object sender, EventArgs e)
        {

        }
        string _devID = null;
        public void UpdateChannelsInfo(string devID)
        {
            _devID = devID;
            panel1.Controls.Clear();
            if (string.IsNullOrEmpty(_devID))
            {
                ShowTips("设备名称为空！");
                return;
            }
            if(!JFHubCenter.Instance.MDCellNameMgr.ContainTrigCtrlDev(devID))
            {
                ShowTips("设备名称列表中未包含触发控制器设备:" + _devID);
                return;
            }
            string[] trigChnNames = JFHubCenter.Instance.MDCellNameMgr.AllChannelNamesInTrigCtrlDev(devID);
            
            if(trigChnNames == null || 0 == trigChnNames.Length)
            {
                ShowTips("命名通道数量为0");
                return;
            }
            IJFDevice_TrigController dev = JFHubCenter.Instance.InitorManager.GetInitor(_devID) as IJFDevice_TrigController;
            if (dev == null)
                ShowTips("设备列表中不存在设备Name = " + _devID);
            if(!dev.IsDeviceOpen)
                ShowTips("设备当前处于关闭状态");
            int top = 3;
            for(int i = 0; i < trigChnNames.Count();i++)
            {
                UcTrigCtrlChn ucChn = new UcTrigCtrlChn();
                ucChn.Location = new Point(3, top);
                panel1.Controls.Add(ucChn);
                ucChn.SetChannelInfo(dev, i, trigChnNames[i]);
                top = ucChn.Bottom + 5;
            }
        }

        private void btFlushChannel_Click(object sender, EventArgs e)
        {
            UpdateChannelsInfo(_devID);
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

        public void BeginEdit()
        {
            foreach (UcTrigCtrlChn uc in panel1.Controls)
                uc.IsChnIDEditting = true;
        }

        public void EndEdit()
        {
            foreach (UcTrigCtrlChn uc in panel1.Controls)
                uc.IsChnIDEditting = false;
        }

        public string[] ChannelNames()
        {
            List<string> ret = new List<string>();
            foreach (UcTrigCtrlChn uc in panel1.Controls)
                ret.Add(uc.ChannelID);
            return ret.ToArray();
        }
    }
}
