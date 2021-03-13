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
    /// 触发控制器的单通道
    /// </summary>
    public partial class UcTrigCtrlChn : UserControl
    {
        public UcTrigCtrlChn()
        {
            InitializeComponent();
        }

        private void UcTrigCtrlChn_Load(object sender, EventArgs e)
        {
            isFormLoaded = true;
            AdjustChannelView();
        }

        bool isFormLoaded = false;
        IJFDevice_TrigController _module = null;
        int _chn = 0;
        string _chnID = "ID";

        bool isIntensityEditting = false;
        bool isDurationEditting = false;

        public void SetChannelInfo(IJFDevice_TrigController module,int chn,string chnID)
        {
            _module = module;
            _chn = chn;
            _chnID = string.IsNullOrEmpty(chnID) ? "通道_" + chn : chnID;
            if (isFormLoaded)
                AdjustChannelView();//UpdateChannelStatus();
        }

        bool isChnEnableChkSetting = false;
        bool isSrcChkSetting = false;
        bool isAdjusting = false;
        List<CheckBox> lstSrcChannelEnables = new List<CheckBox>();
        void AdjustChannelView() //界面布局
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AdjustChannelView));
                return;
            }
            isAdjusting = true;
            tbID.Text = _chnID;

            chkEnable.Enabled = IsChnValid;
            btSoftwareTrig.Enabled = IsChnValid;
            tbDuration.Enabled = IsChnValid;
            tbIntensity.Enabled = IsChnValid;
            if (!IsChnValid)
            {
                isAdjusting = false;
                return;
            }
            foreach (CheckBox chk in lstSrcChannelEnables)
                Controls.Remove(chk);
            lstSrcChannelEnables.Clear();
            int chkLeft = btSoftwareTrig.Right+2;
            int chkTop = btSoftwareTrig.Top+3 ;
            for (int i = 0;i < _module.TrigSrcChannelCount;i++)
            {
                CheckBox chk = new CheckBox();
                chk.Text = "Src" + i;
                chk.Location = new Point(chkLeft, chkTop);
                chk.AutoSize = true;
                lstSrcChannelEnables.Add(chk);
                Controls.Add(chk);
                chkLeft = chk.Right ;
                chk.CheckedChanged += chkTrigSrc_CheckedChanged;
            }
            UpdateChannelStatus();
            isAdjusting = false;
        }

        public void UpdateChannelStatus()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateChannelStatus));
                return;
            }
           
            bool isEnable = false;
            int err = _module.GetTrigChannelEnable(_chn, out isEnable);
            isChnEnableChkSetting = true;
            if (0 != err)
                chkEnable.Checked = false;
            else
                chkEnable.Checked = isEnable;
            isChnEnableChkSetting = false;
            int srcMask = 0;
            err = _module.GetTrigChannelSrc(_chn, out srcMask);
            //if (err != 0)
            //{
            //    foreach (CheckBox chk in lstSrcChannelEnables)
            //        chk.Enabled = false;
            //}
            //else
            //{
            for (int i = 0; i < _module.TrigSrcChannelCount; i++)
            {
                lstSrcChannelEnables[i].Enabled = err == 0;
                isSrcChkSetting = true;
                lstSrcChannelEnables[i].Checked = (srcMask & 1 << i) != 0 ? true : false;
                isSrcChkSetting = false;
            }
            //}
            int nVal = 0;
            if (!isIntensityEditting)
            {
                tbIntensity.BackColor = SystemColors.Control;
                err = _module.GetTrigChannelIntensity(_chn, out nVal);
                if (0 != err)
                {
                    tbIntensity.Text = _module.GetErrorInfo(err);
                }
                else
                    tbIntensity.Text = nVal.ToString();
            }
            if (!isDurationEditting)
            {
                tbDuration.BackColor = SystemColors.Control;
                err = _module.GetTrigChannelDuration(_chn, out nVal);
                if (0 != err)
                    tbDuration.Text = "Error";
                else
                    tbDuration.Text = nVal.ToString();
            }
        }

        bool IsChnValid { get { return null != _module && _chn >= 0 && _chn < _module.TrigChannelCount; } }

        /// <summary>
        /// 使能属性改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (isChnEnableChkSetting)
                return;
            if (!IsChnValid)
            {
                MessageBox.Show("操作失败:模块对象未设置/通道号超出范围!");
                return;
            }

            
            int err = _module.SetTrigChannelEnable(_chn, chkEnable.Checked);
            if(0!= err)
            {
                isChnEnableChkSetting = true;
                chkEnable.Checked = !chkEnable.Checked;
                isChnEnableChkSetting = false;
                
                    MessageBox.Show(chkEnable.Checked ?  "使能": "禁用"  + _chnID + "失败，错误信息:" + _module.GetErrorInfo(err));
                return;
            }
     
        }

        /// <summary>
        /// 软触发按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSoftwareTrig_Click(object sender, EventArgs e)
        {
            if (!IsChnValid)
            {
                MessageBox.Show("操作失败:模块对象未设置/通道号超出范围!");
                return;
            }

            int err = _module.SoftwareTrigChannel(_chn);
            if (0 != err)
            {
                MessageBox.Show("软触发" + _chnID + "失败，错误信息:" + _module.GetErrorInfo(err));
                return;
            }
        }

        private void tbIntensity_Enter(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            tbIntensity.BackColor = Color.White;
            isIntensityEditting = true;
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
            if (e.KeyCode == Keys.Escape)
            {
                isIntensityEditting = false;
                UpdateChannelStatus();
                return;
            }
            else if(e.KeyCode == Keys.Enter) //
            {
                int curr = 0;
                if(!int.TryParse(tbIntensity.Text,out curr))
                {
                    MessageBox.Show("操作失败:参数格式错误！");
                    return;
                }
                int err = _module.SetTrigChannelIntensity(_chn, curr);
                if(0 != err)
                {
                    MessageBox.Show("操作失败，错误信息：" + _module.GetErrorInfo(err));
                    return;
                }
                isIntensityEditting = false;
                tbIntensity.BackColor = Color.White;
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
            int nVal = 0;
            int err = _module.GetTrigChannelIntensity(_chn, out nVal);
            if (0 != err)
            {
                tbIntensity.BackColor = Color.OrangeRed;
                return;
            }
            else
            {
                if(nVal == curr)
                    tbIntensity.BackColor = Color.White;
                else
                    tbIntensity.BackColor = Color.OrangeRed;
            }
        }

        private void tbDuration_Enter(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            isDurationEditting = true;
            tbDuration.BackColor = Color.White;
        }

        private void tbDuration_Leave(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            isDurationEditting = false;
            UpdateChannelStatus();
        }


        private void tbDuration_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsChnValid)
                return;
            if (e.KeyCode == Keys.Escape)
            {
                isDurationEditting = false;
                UpdateChannelStatus();
                return;
            }
            else if (e.KeyCode == Keys.Enter) //
            {
                int curr = 0;
                if (!int.TryParse(tbDuration.Text, out curr))
                {
                    MessageBox.Show("操作失败:参数格式错误！");
                    return;
                }
                int err = _module.SetTrigChannelDuration(_chn, curr);
                if (0 != err)
                {
                    MessageBox.Show("操作失败，错误信息：" + _module.GetErrorInfo(err));
                    return;
                }
                isDurationEditting = false;
                tbDuration.BackColor = Color.White;
            }
        }

        private void tbDuration_TextChanged(object sender, EventArgs e)
        {
            if (!IsChnValid)
                return;
            int curr = 0;
            if (!int.TryParse(tbDuration.Text, out curr))
            {
                tbDuration.BackColor = Color.Red;
                return;
            }
            int nVal = 0;
            int err = _module.GetTrigChannelDuration(_chn, out nVal);
            if (0 != err)
            {
                tbDuration.BackColor = Color.OrangeRed;
                return;
            }
            else
            {
                if (nVal == curr)
                    tbDuration.BackColor = Color.White;
                else
                    tbDuration.BackColor = Color.OrangeRed;
            }
        }

        /// <summary>
        /// 用于在命名表编辑窗口中获取通道名称
        /// </summary>
        public string ChannelID { get { return tbID.Text; } }

        public bool IsChnIDEditting {
            get { return !tbID.ReadOnly; }
            set { tbID.ReadOnly = !value; }
        }


        /// <summary>
        /// 触发源通道选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTrigSrc_CheckedChanged(object sender, EventArgs e)
        {
            if (isSrcChkSetting)
                return;
            int srcMask = 0;
            for (int i = 0; i < _module.TrigSrcChannelCount; i++)
                srcMask += lstSrcChannelEnables[i].Checked ? 1 << i  : 0;

            int err = _module.SetTrigChannelSrc(_chn, srcMask);
            if (err != 0)
            {
                if (!isAdjusting)
                    MessageBox.Show("设置触发源通道失败，Erro" + _module.GetErrorInfo(err));
            }
            UpdateChannelStatus();
             
        }
    }
}
