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
    /// 光源控制器面板
    /// </summary>
    public partial class UcLightCtrl : JFRealtimeUI//UserControl
    {
        public UcLightCtrl()
        {
            InitializeComponent();
        }

        private void UcLightCtrl_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustUI();
        }
        bool _isFormLoaded = false;
        IJFDevice_LightController _module = null;
        string[] _chnIDs = null;
        List<UcLightCtrlChn> _lstUc = new List<UcLightCtrlChn>(); //各通道界面
        List<CheckBox> _lstChk = new List<CheckBox>(); //多通道使能

        [Category("JF属性"), Description("显示打开设备按钮"), Browsable(true)]
        public bool IsShowOpenCloseButton
        {
            get { return btOpenClose.Visible; }
            set { btOpenClose.Visible = value; }
        }


        public void SetModuleInfo(IJFDevice_LightController module,string[] chnIDs = null)
        {
            _module = module;
            _chnIDs = chnIDs;
            if (_isFormLoaded)
                AdjustUI();
        }

        bool isChkEnableSetting = false;

        public void AdjustUI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustUI));
                return;
            }
            foreach (UcLightCtrlChn uc in _lstUc)
                Controls.Remove(uc);
            _lstUc.Clear();
            foreach (CheckBox chk in _lstChk)
                Controls.Remove(chk);
            _lstChk.Clear();

            if (null == _module)
            {
                lbInfo.Text = "设备状态:未设置";
                btOpenClose.Enabled = false;
                return;
            }
            btOpenClose.Enabled = true;
            if (_module.IsDeviceOpen)
            {
                lbInfo.Text = "设备状态:已打开";
                btOpenClose.Text = "关闭设备";
            }
            else
            {
                lbInfo.Text = "设备状态:已关闭";
                btOpenClose.Text = "打开设备";
            }

            int locY = btOpenClose.Bottom + 10;
            for(int i = 0;i < _module.LightChannelCount;i++)
            {
                UcLightCtrlChn ucChn = new UcLightCtrlChn();
                ucChn.Location = new Point(3, locY);
                string chnID = (null == _chnIDs || _chnIDs.Length <= i) ? "通道_" + i.ToString() : _chnIDs[i];
                ucChn.SetChannelInfo(_module, i, chnID);
                _lstUc.Add(ucChn);
                Controls.Add(ucChn);
                CheckBox chkEnable = new CheckBox();
                chkEnable.Text = "使能";
                chkEnable.Location = new Point(3 + ucChn.Width + 3, locY);
                _lstChk.Add(chkEnable);
                Controls.Add(chkEnable);
                chkEnable.CheckedChanged += OnMultiEnableButtonClick;
                locY += ucChn.Height + 3;
            }
            UpdateModuleStatus();
        }

        public void UpdateModuleStatus()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateModuleStatus));
                return;
            }
            if (null == _module)
                return;
            if (_module.IsDeviceOpen)
            {
                lbInfo.Text = "设备状态:已打开";
                btOpenClose.Text = "关闭设备";
            }
            else
            {
                lbInfo.Text = "设备状态:已关闭";
                btOpenClose.Text = "打开设备";
            }

            foreach (UcLightCtrlChn uc in _lstUc)
                uc.UpdateChannelStatus();

            bool[] enables;
            int err = _module.GetLightChannelEnables(out enables);
            for (int i = 0; i < _lstChk.Count; i++)
            {
                isChkEnableSetting = true;
                _lstChk[i].Checked = (err == 0) ? enables[i] : false;
                isChkEnableSetting = false;
            }

        }

        /// <summary>
        /// 多通道使能按钮被点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMultiEnableButtonClick(object sender, EventArgs e)
        {
            if (isChkEnableSetting)
                return;
            if (null == _module)
                return;
            List<int> chns = new List<int>();
            List<bool> enables = new List<bool>();
            for (int i = 0; i < _lstChk.Count();i++)
            {
                chns.Add(i);
                enables.Add(_lstChk[i].Checked);
            }
            int err = _module.SetLightChannelEnables(chns.ToArray(), enables.ToArray());
            if(err != 0)
            {
                MessageBox.Show("多通道使能失败，错误信息:" + _module.GetErrorInfo(err));
                UpdateModuleStatus();
                return;
            }
            UpdateModuleStatus();
        }

        public override void UpdateSrc2UI()
        {
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
            if (err != 0)
                MessageBox.Show((isCurrOpen ? "关闭" : "打开") + "设备失败，ErrorInfo ：" + _module.GetErrorInfo(err));
            UpdateModuleStatus();
        }
    }
}
