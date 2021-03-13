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

namespace JFHub
{
    public partial class UcAxisStatusIS : UserControl
    {
        public UcAxisStatusIS()
        {
            InitializeComponent();
        }

        private void UcAxisStatusIS_Load(object sender, EventArgs e)
        {

        }
        //bool _isAxisEnabled = false;
        string _axisName = null;
        public void SetAxisName(string axisName)
        {
            //_isAxisEnabled = false;
            _axisName = axisName;
            gbAxisName.Text = _axisName;
            JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(_axisName);
            if(ci == null)
            {
                gbAxisName.Text += "  轴名无效";
                ucAxisStatus1.Enabled = false;
                return;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            if(null == dev)
            {
                gbAxisName.Text += "  设备无效";
                ucAxisStatus1.Enabled = false;
                return;
            }
            if(!dev.IsDeviceOpen)
            {
                gbAxisName.Text += "  设备未打开";
                ucAxisStatus1.Enabled = false;
                return;
            }
            if(ci.ModuleIndex >=dev.McCount)
            {
                gbAxisName.Text += "  模块号无效";
                ucAxisStatus1.Enabled = false;
                return;
            }

            IJFModule_Motion mm = dev.GetMc(ci.ModuleIndex);

            if(ci.ChannelIndex >= mm.AxisCount)
            {
                gbAxisName.Text += "  轴序号无效";
                ucAxisStatus1.Enabled = false;
                return;
            }
            ucAxisStatus1.Enabled = true;
            ucAxisStatus1.SetAxis(mm, ci.ChannelIndex);
            //_isAxisEnabled = true;

        }

        public  void UpdateAxisStatus()
        {
            ucAxisStatus1.UpdateAxisStatus();
        }
    }
}
