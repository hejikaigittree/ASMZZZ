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
    /// 位置比较触发器调试面板
    /// </summary>
    public partial class UcCmprTrig : JFRealtimeUI
    {
        public UcCmprTrig()
        {
            InitializeComponent();
        }

        
        private void UcCompareTrig_Load(object sender, EventArgs e)
        {
            _isLoaded = true;
            AdjustUI();
        }

        int maxTips = 100;
        bool _isLoaded = false;
        IJFModule_CmprTrigger _module = null;
        string[] _encChnIDs = null; //各通道名称
        string[] _trigChnIDs = null;
        //List<Label> lstTrigIdLbs = new List<Label>();
        List<CheckBox> lstTrigEnableChks = new List<CheckBox>();//触发通道使能控件
        List<TextBox> lstTrigTimeTbs = new List<TextBox>(); //触发通道的计数控件
        List<Button> lstSwTrigBts = new List<Button>();//软触发按钮
        List<Button> lstResetTimeBts = new List<Button>();//触发次数归零按钮

        delegate void dgShowTip(string txt);
        void ShowTips(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return;
            if (InvokeRequired)
            {
                Invoke(new dgShowTip(ShowTips), new object[] { txt });
                return;
            }

            rbTips.AppendText(txt + "\n");
            string[] lines = rbTips.Text.Split('\n');
            if (lines.Length >= maxTips)
            {
                int rmvChars = 0;
                for (int i = 0; i < lines.Length - maxTips; i++)
                    rmvChars += lines[i].Length + 1;
                rbTips.Text = rbTips.Text.Substring(rmvChars);
            }
            rbTips.Select(rbTips.TextLength, 0); //滚到最后一行
            rbTips.ScrollToCaret();//滚动到控件光标处 
        }

        /// <summary>设置模块</summary>
        public void SetCmprTigger(IJFModule_CmprTrigger module,string[] encChnIDs,string[] trgChnIDs)
        {
            _module = module;
            _encChnIDs = encChnIDs;
            _trigChnIDs = trgChnIDs;
            if (_isLoaded)
                AdjustUI();
        }


        bool isChkEnableSetting = false; //主动更新CheckBox控件值
        //根据通道数量布置界面
        void AdjustUI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AdjustUI));
                return;
            }
            gbChns.Controls.Clear();
            gbTrigs.Controls.Clear();
            //lstTrigIdLbs.Clear();
            lstTrigEnableChks.Clear();
            lstTrigTimeTbs.Clear();
            lstResetTimeBts.Clear();
            lstSwTrigBts.Clear();
            if (null == _module)
            {
                ShowTips("模块对象为空");
                return;
            }
            int err = 0;
            int locX = 3, locY = 30;
            for(int i = 0; i < _module.EncoderChannels;i++)
            {
                UcCmprTrgChn ucChn = new UcCmprTrgChn();
                ucChn.OnTxtMsg += ShowTips;
                ucChn.Location = new Point(locX, locY);
                string encName = _encChnIDs == null ? null : (_encChnIDs.Length > i ? _encChnIDs[i] : null);
                ucChn.SetModuleChn(_module, i, encName, _trigChnIDs);
                locY += ucChn.Height + 3;
                gbChns.Controls.Add(ucChn);
            }
            locY = 30;
            for(int i = 0; i < _module.TrigChannels;i++ )
            {
                ///通道ID标签
                Label lbTrigID = new Label();
                lbTrigID.Text = _trigChnIDs != null && (_trigChnIDs.Length > i) ? _trigChnIDs[i] : ("通道_" + i.ToString());
                lbTrigID.Location = new Point(3, locY + 5);
                lbTrigID.Width = 100;
                gbTrigs.Controls.Add(lbTrigID);
                ///通道使能
                CheckBox chkEnable = new CheckBox();
                chkEnable.Text = "使能";
                chkEnable.Location = new Point(lbTrigID.Right + 3, locY);
                gbTrigs.Controls.Add(chkEnable);
                lstTrigEnableChks.Add(chkEnable);
                chkEnable.CheckedChanged += OnTrigEnableCheckedChange;
                bool isEnable = false;
                err = _module.GetTrigEnable(i, out isEnable);
                isChkEnableSetting = true;
                if (err != 0)
                {
                    ShowTips("获取 " + lbTrigID.Text + " 使能状态失败,错误信息:" + _module.GetErrorInfo(err));
                    chkEnable.Checked = false;
                }
                else
                    chkEnable.Checked = isEnable;
                isChkEnableSetting = false;
                ///触发次数
                Label lbTrigTimes = new Label();
                lbTrigTimes.Text = "次数";
                lbTrigTimes.Location = new Point(chkEnable.Right + 3, locY + 5);
                gbTrigs.Controls.Add(lbTrigTimes);
                TextBox tbTrigTimes = new TextBox();
                tbTrigTimes.ReadOnly = true;
                tbTrigTimes.BackColor = SystemColors.Control;
                tbTrigTimes.Location = new Point(lbTrigTimes.Right + 3, locY);
                gbTrigs.Controls.Add(tbTrigTimes);
                lstTrigTimeTbs.Add(tbTrigTimes);
                Button btResetTimes = new Button();
                btResetTimes.Location = new Point(tbTrigTimes.Right + 3, locY);
                btResetTimes.Text = "置0";
                gbTrigs.Controls.Add(btResetTimes);
                lstResetTimeBts.Add(btResetTimes);
                btResetTimes.Click += OnResetTrigTimesButtonClick;
                ///软触发
                Button btSoftwareTrig = new Button();
                btSoftwareTrig.Location = new Point(btResetTimes.Right + 3, locY);
                btSoftwareTrig.Text = "软触发";
                gbTrigs.Controls.Add(btSoftwareTrig);
                lstSwTrigBts.Add(btSoftwareTrig);
                btSoftwareTrig.Click += OnSoftwareTrigButtonClick;
                locY += btSoftwareTrig.Height + 3;
            }
            UpdateModuleStatus();
        }


        /// <summary>触发输出通道使能属性改变</summary>
        void OnTrigEnableCheckedChange(object sender,EventArgs e)
        {
            if (null == _module)
            {
                MessageBox.Show("无效操作，模块对象未设置（空值）");
                return;
            }

            if (isChkEnableSetting)
                return;

            for(int i = 0; i < _module.TrigChannels;i++)
                if(sender == lstTrigEnableChks[i])
                {
                    bool isEnable = lstTrigEnableChks[i].Checked;
                    int err = _module.SetTrigEnable(i, isEnable);
                    if(0!= err)
                    {
                        //lstTrigEnableChks[i].Checked = !isEnable; //不能在此采取赋值操作，会重复调用
                        MessageBox.Show(string.Format("{0}触发通道{1}失败，错误信息：{2}", isEnable ? "使能" : "禁用", i, _module.GetErrorInfo(err)));
                        return;
                    }
                    else
                        ShowTips(string.Format("{0}触发通道{1}完成", isEnable ? "使能" : "禁用", i));
                }
            //UpdateModuleStatus();
        }

        /// <summary>触发次数置0</summary>
        void OnResetTrigTimesButtonClick(object sender, EventArgs e)
        {
            if (null == _module)
            {
                MessageBox.Show("无效操作，模块对象未设置（空值）");
                return;
            }

            for (int i = 0; i < _module.TrigChannels; i++)
                if (sender == lstResetTimeBts[i])
                {
                    int err = _module.ResetTriggedCount(i);
                    if (0 != err)
                    {
                        MessageBox.Show(string.Format("触发通道{0} 次数置0失败，错误信息：{1}", i, _module.GetErrorInfo(err)));
                        return;
                    }
                    else
                        ShowTips(string.Format("触发通道{0} 次数置0完成", i));
                }
            UpdateModuleStatus();
        }

        /// <summary>软触发按钮</summary>
        void OnSoftwareTrigButtonClick(object sender, EventArgs e)
        {
            if (null == _module)
            {
                MessageBox.Show("无效操作，模块对象未设置（空值）");
                return;
            }

            for (int i = 0; i < _module.TrigChannels; i++)
                if (sender == lstSwTrigBts[i])
                {
                    int err = _module.SoftTrigge(new int[] { i});
                    if (0 != err)
                    {
                        MessageBox.Show(string.Format("触发通道{0} 软触发失败，错误信息：{1}", i, _module.GetErrorInfo(err)));
                        return;
                    }
                    else
                        ShowTips(string.Format("触发通道{0} 软触发完成", i));
                }
            UpdateModuleStatus();
        }


        /// <summary>更新所有状态</summary>
        public void UpdateModuleStatus()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateModuleStatus));
                return;
            }
            if (null == _module)
                return;
            for(int i = 0; i < _module.EncoderChannels;i++ )
            {
                UcCmprTrgChn ucEnc = gbChns.Controls[i] as UcCmprTrgChn;
                ucEnc.UpdateChnStatus();
            }

            int err = 0;
            for(int i = 0; i < _module.TrigChannels;i++)
            {
                int count = 0;
                err = _module.GetTriggedCount(i, out count);
                if (0 != err)
                    lstTrigTimeTbs[i].Text = "Err";
                else
                    lstTrigTimeTbs[i].Text = count.ToString();
            }
        }

        public override void UpdateSrc2UI()
        {
            UpdateModuleStatus();
        }



        private void btClearTips_Click(object sender, EventArgs e)
        {
            rbTips.Text = "";
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            UpdateModuleStatus(); 
        }
    }
}
