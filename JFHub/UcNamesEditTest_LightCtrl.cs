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
using JFUI;

namespace JFHub
{
    /// <summary>
    /// 光源控制器(兼容带触发功能的光源控制器)
    /// </summary>
    public partial class UcNamesEditTest_LightCtrl : UserControl
    {
        public UcNamesEditTest_LightCtrl()
        {
            InitializeComponent();
        }

        private void UcNamesEditTest_LightCtrl_Load(object sender, EventArgs e)
        {

        }
        string _devID = null;
        
        public void UpdateChannelsInfo(string devID) //必须从名称表中的触发控制器和光源控制器提取信息
        {
            _devID = devID;
            panelLight.Controls.Clear();
            panelTrig.Controls.Clear();
            if (string.IsNullOrEmpty(_devID))
            {
                ShowTips("设备名称为空！");
                return;
            }

            if (!JFHubCenter.Instance.MDCellNameMgr.ContainLightCtrlDev(devID))
            {
                ShowTips("设备名称列表中未包含光源控制器设备:" + _devID);
                return;
            }

            IJFDevice_LightController dev = JFHubCenter.Instance.InitorManager.GetInitor(_devID) as IJFDevice_LightController;
            if (dev == null)
            {
                ShowTips("设备列表中不存在设备Name = " + _devID);
                //return;
            }
            else
            {
                if (!dev.IsDeviceOpen)
                    ShowTips("设备当前处于关闭状态");
            }

            int top = 3;
            string[] lightChnNames = JFHubCenter.Instance.MDCellNameMgr.AllChannelNamesInLightCtrlDev(devID);
            if (lightChnNames == null || 0 == lightChnNames.Length)
            {
                ShowTips("开关通道数量为0");
                return;
            }
            else
            {
                top = 3;
                for (int i = 0; i < lightChnNames.Count(); i++)
                {
                    UcLightCtrlChn ucChn = new UcLightCtrlChn();
                    ucChn.Location = new Point(3, top);
                    panelLight.Controls.Add(ucChn);
                    ucChn.SetChannelInfo(dev, i, lightChnNames[i]);//.SetChannelInfo(dev, i, trigChnNames[i]);
                    top = ucChn.Bottom + 5;
                }
            }

            if(dev != null && !typeof(IJFDevice_LightControllerWithTrig).IsAssignableFrom(dev.GetType())) //本设备不支持触发控制器接口
            {
                panelTrig.Visible = false;
                return;
            }

            if (!JFHubCenter.Instance.MDCellNameMgr.ContainTrigCtrlDev(devID)) //不包含触发功能
            {
                ShowTips("设备命名表中不包含触发通道名称信息");
                return;
            }
            //panelTrig.Top = panelLight.Bottom + 10;
                panelTrig.Visible = true;
                string[] trigChnNames = JFHubCenter.Instance.MDCellNameMgr.AllChannelNamesInTrigCtrlDev(devID);

                if (trigChnNames == null || 0 == trigChnNames.Length)
                {
                    ShowTips("命名通道数量为0");
                    return;
                }
               
                top = 3;
                for (int i = 0; i < trigChnNames.Count(); i++)
                {
                    UcTrigCtrlChn ucChn = new UcTrigCtrlChn();
                    ucChn.Location = new Point(3, top);
                    panelTrig.Controls.Add(ucChn);
                    ucChn.SetChannelInfo(dev as IJFDevice_TrigController, i, trigChnNames[i]);
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

        /// <summary>
        /// 用于编辑功能
        /// </summary>
        public void BeginEdit()
        {
            foreach (UcLightCtrlChn ucLight in panelLight.Controls)
                ucLight.IsChnIDEditting = true;
            foreach (UcTrigCtrlChn ucTrig in panelTrig.Controls)
                ucTrig.IsChnIDEditting = true;
        }

        public void EndEdit()
        {
            foreach (UcLightCtrlChn ucLight in panelLight.Controls)
                ucLight.IsChnIDEditting = false;
            foreach (UcTrigCtrlChn ucTrig in panelTrig.Controls)
                ucTrig.IsChnIDEditting = false;
        }


        public string[] LightChannelNames()
        {
            List<string> ret = new List<string>();
            foreach (UcLightCtrlChn ucLight in panelLight.Controls)
                ret.Add(ucLight.ChannelID);
                return ret.ToArray();
        }

        public string[] TrigChannelNames()
        {
            List<string> ret = new List<string>();
            foreach (UcTrigCtrlChn ucTrig in panelTrig.Controls)
                ret.Add(ucTrig.ChannelID);
            return ret.ToArray();
        }
    }
}
