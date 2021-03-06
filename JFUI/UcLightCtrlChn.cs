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
    public partial class UcLightCtrlChn : UserControl
    {
        public UcLightCtrlChn()
        {
            InitializeComponent();
        }

        private void UcLightCtrlChn_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
        }

        bool _isFormLoaded = false;
        IJFDevice_LightController _module = null;
        int _chn = 0;
        string _chnID = "ID";

        [Category("JF属性"), Description("亮度最大值"), Browsable(true)]
        public int MaxIntensity 
        { 
            get { return trkIntensity.Maximum; }
            set 
            { 
                trkIntensity.Maximum = value;
                lbMax.Text = value.ToString();
            }
        }

        public void SetChannelInfo(IJFDevice_LightController module,int chn,string chnID = null)
        {
            _module = module;
            _chn = chn;
            _chnID = chnID == null ? "通道_" + chn.ToString() : chnID;
            tbID.Text = _chnID;
            if (_isFormLoaded)
                UpdateChannelStatus();
        }
        bool isChkEnableSetting = false;

        bool IsChnValid { get { return null != _module && _chn >= 0 && _chn < _module.LightChannelCount; } }
        public void UpdateChannelStatus() 
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateChannelStatus));
                return;
            }
            if (!IsChnValid)
            {
                isChkEnableSetting = false;
                chkEnable.Checked = false;
                isChkEnableSetting = true;
                chkEnable.Enabled = false;
                isTrkSetting = true;
                trkIntensity.Value = 0;
                isTrkSetting = false;
                trkIntensity.Enabled = false;
                tbIntensity.Text = "Unvalid";
                tbIntensity.Enabled = false;
                return;
            }
            
            chkEnable.Enabled = true;
            bool isEnable = false;
            int err = _module.GetLightChannelEnable(_chn, out isEnable);
            isChkEnableSetting = true;
            if (0 != err)
                chkEnable.Checked = false;
            else
                chkEnable.Checked = isEnable;
            isChkEnableSetting = false;
            trkIntensity.Enabled = isEnable;
            tbIntensity.Enabled = isEnable;
            tbIntensity.BackColor = SystemColors.Control;
            int val = 0;
            err = _module.GetLightIntensity(_chn, out val);
            if (!isIntensityEditting)
            {
                
                if (err != 0)
                {
                    //trkIntensity.Value = 0;
                    tbIntensity.Text = "Error";
                }
                else
                {
                    //trkIntensity.Value = val;
                    tbIntensity.Text = val.ToString();
                }
            }
            if(!isTrkFocus)
            {
                isTrkSetting = true;
                if (err != 0)
                {
                    trkIntensity.Value = 0;
                    //tbIntensity.Text = "Error";
                }
                else
                {
                    trkIntensity.Value = val;
                    //tbIntensity.Text = val.ToString();
                }
                isTrkSetting = false;
            }
        }

        bool isIntensityEditting = false; //亮度控件正在编辑
        bool isTrkSetting = false; // 是否由Update函数设置
        private void tbIntensity_Enter(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            isIntensityEditting = true;
            tbIntensity.BackColor = Color.White;
        }

        private void tbIntensity_Leave(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            isIntensityEditting = false;
            UpdateChannelStatus();
        }

        private void tbIntensity_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsChnValid)
                return;
            if(e.KeyCode == Keys.Escape)
            {
                UpdateChannelStatus();
                return;
            }
            else if(e.KeyCode == Keys.Enter)
            {
                int curr = 0;
                if(!int.TryParse(tbIntensity.Text,out curr))
                {
                    MessageBox.Show("设置亮度失败,参数格式错误!");
                    return;
                }

                if(curr < 0 || curr > MaxIntensity)
                {
                    MessageBox.Show(string.Format("设置亮度失败,参数 = {0}超出可设置范围0~{1}!",curr,MaxIntensity));
                    return;
                }
                int err = _module.SetLightIntensity(_chn, curr);
                if(err != 0)
                {
                    MessageBox.Show("设置亮度失败，错误信息:" + _module.GetErrorInfo(err));
                    return;
                }
                UpdateChannelStatus();
            }
        }

        private void tbIntensity_TextChanged(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            int curr = 0;
            if(!int.TryParse(tbIntensity.Text,out curr))
            {
                tbIntensity.BackColor = Color.Red;
                return;
            }
            int val = 0;
            int err = _module.GetLightIntensity(_chn, out val);
            if(0 != err || val != curr)
            {
                tbIntensity.BackColor = Color.OrangeRed;
                return;
            }

            tbIntensity.BackColor = Color.White;
            return;
        }

        /// <summary>
        /// 用于在命名通道列表中编辑通道名称
        /// </summary>
        public string ChannelID { get { return tbID.Text; } }
        public bool IsChnIDEditting 
        { 
            get { return !tbID.ReadOnly; }
            set { tbID.ReadOnly = !value; }
        }

        private void UcLightCtrlChn_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                UpdateChannelStatus();
        }

        //使能或关闭通道
        private void chkEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (isChkEnableSetting)
            {
                if(chkEnable.Checked)
                {
                    tbIntensity.Enabled = true;
                    trkIntensity.Enabled = true;
                }
                return;
            }
            bool enabledAfterOpt = chkEnable.Checked;
            int err = _module.SetLightChannelEnable(_chn, chkEnable.Checked);
            if(err != 0)
            {
                MessageBox.Show((chkEnable.Checked ? "使能" : "禁用") + _chnID + " 失败，错误信息:" + _module.GetErrorInfo(err));
                isChkEnableSetting = true;
                chkEnable.Checked = !chkEnable.Checked;
                isChkEnableSetting = false;
                return;
            }
            else
            {
                tbIntensity.Enabled = enabledAfterOpt;
                trkIntensity.Enabled = enabledAfterOpt;
            }
            
        }

        bool isTrkFocus = false;
        private void trkIntensity_Enter(object sender, EventArgs e)
        {
            isTrkFocus = true;
        }

        private void trkIntensity_Leave(object sender, EventArgs e)
        {
            isTrkFocus = false;
        }

        private void trkIntensity_ValueChanged(object sender, EventArgs e)
        {
            if (_module == null)
                return;
            if (isTrkSetting)
                return;
            if (!_module.IsDeviceOpen)
                return;
            int intensity = trkIntensity.Value;
            _module.SetLightIntensity(_chn, intensity);
        }
    }
}
