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

namespace JFUI
{
    /// <summary>
    /// 带触发功能的光源控制器 ，比如（OPT光源控制器）
    /// </summary>
    public partial class UcLightCtrl_T : JFRealtimeUI//UserControl
    {
        public UcLightCtrl_T()
        {
            InitializeComponent();
        }

        private void UcLightCtrl_T_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            cbMode.Items.AddRange(new string[] { "开关模式", "触发模式" });
            AdjustUI();
        }
        bool _isFormLoaded = false;
        IJFDevice_LightControllerWithTrig _module = null;


        public void SetModule(IJFDevice_LightControllerWithTrig module,string[] lightChnIDs,string[] trigChnIDs)
        {
            _module = module;
            ucLight.SetModuleInfo(_module, lightChnIDs);
            ucTrig.SetModuleInfo(_module,null, trigChnIDs);
            if (_isFormLoaded)
                AdjustUI();
        }

        void AdjustUI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustUI));
                return;
            }

            ucLight.AutoSize = true;
            ucTrig.AutoSize = true;
            UpdateModuleStatus();
        }

        public void UpdateModuleStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateModuleStatus));
                return;
            }
            if(null == _module)
            {
                cbMode.Text = "";
                cbMode.Enabled = false;
                ucLight.Enabled = false;
                ucTrig.Enabled = false;
                btOpenClose.Enabled = false;
                cbMode.Enabled = false;
                return;
            }
            btOpenClose.Enabled = true;
            if (_module.IsDeviceOpen)
                btOpenClose.Text = "关闭设备";
            else
                btOpenClose.Text = "打开设备";

            cbMode.Enabled = true;
            ucLight.Enabled = true;
            ucTrig.Enabled = true;
            JFLightWithTrigWorkMode mode;
            int err = _module.GetWorkMode(out mode);
            if (err != 0)
                cbMode.Text = _module.GetErrorInfo(err);
            else
            {
                if (mode == JFLightWithTrigWorkMode.TurnOnOff)
                    cbMode.Text = "开关模式";
                else if(mode == JFLightWithTrigWorkMode.Trigger)
                    cbMode.Text = "触发模式";
            }
            ucLight.UpdateModuleStatus();
            ucTrig.UpdateModuleStatus();
        }

        public override void  UpdateSrc2UI()
        {
            UpdateModuleStatus();
        }

        private void UcLightCtrl_T_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                UpdateModuleStatus();
        }

        private void btOpenClose_Click(object sender, EventArgs e)
        {
            if (null == _module)
                return;
            int err = 0;
            bool isCurrOpen = _module.IsDeviceOpen;
            if (isCurrOpen)
                err = _module.CloseDevice();
            else
                err = _module.OpenDevice();
            if(err != 0)
            {
                MessageBox.Show((isCurrOpen ? "关闭" : "打开") + "设备失败，ErrorInfo:" + _module.GetErrorInfo(err));
            }
            UpdateModuleStatus();
        }
    }
}
