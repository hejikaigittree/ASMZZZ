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
    public partial class UcTrigCtrl : JFRealtimeUI//UserControl
    {
        public UcTrigCtrl()
        {
            InitializeComponent();
        }

        private void UcTrigCtrl_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustUI();
        }


        [Category("JF属性"), Description("显示打开设备按钮"), Browsable(true)]
        public bool IsShowOpenCloseButton
        {
            get { return btOpenClose.Visible; }
            set { btOpenClose.Visible = value; }
        }

        bool _isFormLoaded = false;
        IJFDevice_TrigController _module = null;
        int[] _chns = null;
        string[] _chnIDs = null;
        List<UcTrigCtrlChn> _lstUcs = new List<UcTrigCtrlChn>();
        /// <summary>
        /// 设置模块和通道序号
        /// </summary>
        /// <param name="module"></param>
        /// <param name="chns">当此值为null时，表示显示所有通道</param>
        public void SetModuleInfo(IJFDevice_TrigController module,int[] chns = null,string[] chnIDs = null)
        {
            _module = module;
            List<int> lstChn = new List<int>();
            List<string> lstIDs = new List<string>();
            if (null != _module)
            {
                if (null == chns) //添加所有通道
                {
                    for(int i = 0; i < _module.TrigChannelCount;i++)
                    {
                        lstChn.Add(i);
                        if (null == chnIDs || chnIDs.Length > i)
                            lstIDs.Add("通道_" + i.ToString());
                        else
                            lstIDs.Add(chnIDs[i]);
                    }
                }
                else
                {
                    lstChn.AddRange(chns);
                    for (int i = 0; i < chns.Length; i++)
                    {
                        if (null == chnIDs || chnIDs.Length > i)
                            lstIDs.Add("通道_" + i.ToString());
                        else
                            lstIDs.Add(chnIDs[i]);
                    }
                }
            }
            _chns = lstChn.ToArray();
            _chnIDs = lstIDs.ToArray();
            if (_isFormLoaded)
                AdjustUI();
        }

        

        public void AdjustUI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustUI));
                return;
            }
            foreach (UcTrigCtrlChn uc in _lstUcs)
                Controls.Remove(uc);
            _lstUcs.Clear();

            
            if (null == _module)
            {
                lbInitStatus.Text = "设备对象未设置(空值)";
                lbOpened.Text = "设备未打开/已关闭";
                btSoftwareTrig.Enabled = false;
                btOpenClose.Enabled = false;
                return;
            }
            if (!_module.IsInitOK)
                lbInitStatus.Text = "设备初始化失败！";
            else
                lbInitStatus.Text = "设备初始化成功";
            if (null == _chns)
                return;

            //btSoftwareTrig.Enabled = true;
            int locY = lbInitStatus.Bottom + 10;
            for(int i = 0; i<_chns.Length;i++)
            {
                UcTrigCtrlChn ucChn = new UcTrigCtrlChn();
                ucChn.Location = new Point(0, locY);
                ucChn.SetChannelInfo(_module, _chns[i], _chnIDs[i]);
                Controls.Add(ucChn);
                _lstUcs.Add(ucChn);
                locY += ucChn.Height + 20;
            }
            Height = locY;
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
                btOpenClose.Enabled = false;
                btSoftwareTrig.Enabled = false;
                return;
            }
            btOpenClose.Enabled = true;
            if (_module.IsDeviceOpen)
            {
                lbOpened.Text = "设备已打开";
                btOpenClose.Text = "关闭设备";
            }
            else
            {
                lbOpened.Text = "设备未打开/已关闭";
                btOpenClose.Text = "打开设备";
            }
            btSoftwareTrig.Enabled = _module.IsDeviceOpen;

            foreach (UcTrigCtrlChn uc in _lstUcs)
            {
                uc.Enabled = _module.IsDeviceOpen;
                if (_module.IsDeviceOpen)
                    uc.UpdateChannelStatus();
            }
        }

        // 软触发所有通道
        private void btSoftwareTrig_Click(object sender, EventArgs e)
        {
            if(null == _module)
            {
                MessageBox.Show("模块对象未设置（空值）！");
                return;
            }
            int err = _module.SoftwareTrigAll();
            if(err != 0)
            {
                MessageBox.Show("软触发所有通道失败,错误信息:" + _module.GetErrorInfo(err));
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
            if(null == _module)
                return;
            int err = 0;
            if (_module.IsDeviceOpen)
            {
                err = _module.CloseDevice();
                if (0 != err)
                {
                    MessageBox.Show("关闭设备失败，错误信息：" + _module.GetErrorInfo(err));
                    return;
                }
            }
            else
            {
                err = _module.OpenDevice();
                if (0 != err)
                {
                    MessageBox.Show("打开设备失败，错误信息：" + _module.GetErrorInfo(err));
                    return;
                }
            }
            UpdateModuleStatus();
        }
    }
}
