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
    public partial class UcCmprTrgChn : UserControl
    {
        public UcCmprTrgChn()
        {
            InitializeComponent();
        }

        public delegate void TxtMessage(string msg);
        public TxtMessage OnTxtMsg;


        private void UcCmprTrgChn_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            cbMode.Items.AddRange(new string[] { "禁用","线性","点表" });
            AdjustView();
        }

        
        bool _isFormLoaded = false;
        IJFModule_CmprTrigger _module = null; //触发模块
        int _encChn = 0;//编码器通道号
        string _id = "";
        string[] _trigChnIDs = null;//触发器输出通道的名称
        /// <summary>设置模块和编码器通道号</summary>
        public void SetModuleChn(IJFModule_CmprTrigger module,int encChn,string id ,string[] trigChnIDs)
        {
            _module = module;
            _encChn = encChn;
            _trigChnIDs = trigChnIDs;
            if (null == _module)
            {
                id = "";
            }
            else
            {
                if (id == null)
                    _id = "Enc_" + _encChn.ToString("D2");
                else
                _id = id;
            }
            if (_isFormLoaded)
                AdjustView();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AdjustView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustView));
                return;
            }
            if(null == _module)
            {
                lbID.Text = "空对象";
                tbEnc.Text = "";
                tbTrigCount.Text = "";
                btResetCnt.Enabled = false;
                cbMode.Text = "";//cbMode.SelectedIndex = 0;
                cbMode.Enabled = false;
                btSoftTrig.Enabled = false;
                btCfg.Enabled = false;
                btSync.Enabled = false;
                return;
            }
            lbID.Text = _id;
            double dVal = 0;
            int err = _module.GetEncoderCurrPos(_encChn, out dVal);
            if (err != 0)
                tbEnc.Text = "Err:" + _module.GetErrorInfo(err);
            else
                tbEnc.Text = dVal.ToString();
            int nVal = 0;
            int[] trigChns = null;
            err = _module.GetEncoderTrigBind(_encChn, out trigChns);//一个编码器输入可能会绑定多个触发输出
            if (err != 0)
                tbTrigCount.Text = "Err:" + _module.GetErrorInfo(err);
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0 ; i < trigChns.Length;i++)
                {
                    int trigChn = trigChns[i];
                    err = _module.GetTriggedCount(trigChn, out nVal);
                    if (err != 0)
                        sb.Append("Err");
                    else
                        sb.Append(nVal.ToString());
                    if (i < trigChns.Length - 1)
                        sb.Append("|");

                }
                tbTrigCount.Text = sb.ToString();
            }
            btSync.Enabled = true;
            btResetCnt.Enabled = true;
            cbMode.Enabled = true;
            JFCompareTrigMode trigMode = JFCompareTrigMode.disable;
            err = _module.GetTrigMode(_encChn, out trigMode);
            if (err != 0)
                cbMode.Text = "Err";
            else
                cbMode.SelectedIndex = (int)trigMode;
            btSoftTrig.Enabled = true;
            btCfg.Enabled = true;
            
        }

        public void UpdateChnStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateChnStatus));
                return;
            }
            if (null == _module)
                return;

            lbID.Text = _id;
            double dVal = 0;
            int err = _module.GetEncoderCurrPos(_encChn, out dVal);
            if (err != 0)
                tbEnc.Text = "Err:" + _module.GetErrorInfo(err);
            else
                tbEnc.Text = dVal.ToString();
            int nVal = 0;
            int[] trigChns = null;
            err = _module.GetEncoderTrigBind(_encChn, out trigChns);//一个编码器输入可能会绑定多个触发输出
            if (err != 0)
                tbTrigCount.Text = "Err:" + _module.GetErrorInfo(err);
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < trigChns.Length; i++)
                {
                    int trigChn = trigChns[i];
                    err = _module.GetTriggedCount(trigChn, out nVal);
                    if (err != 0)
                        sb.Append("Err");
                    else
                        sb.Append(nVal.ToString());
                    if (i < trigChns.Length - 1)
                        sb.Append("|");

                }
                tbTrigCount.Text = sb.ToString();
            }

        }


        /// <summary>软触发按钮</summary>
        private void btSoftTrig_Click(object sender, EventArgs e)
        {
            if(null == _module)
            {
                MessageBox.Show("软触发失败！触发器对象未设置（空值）");
                return;
            }
            if(_encChn < 0 || _encChn >= _module.EncoderChannels)
            {
                MessageBox.Show(string.Format("软触发失败！编码器通道序号 ={0} 超出范围0~{1}", _encChn,_module.EncoderChannels-1));
                return;
            }
            string errInfo = "";
            int[] trigChn = null;
            int err = _module.GetEncoderTrigBind(_encChn, out trigChn);
            if(err != 0)
            {
                errInfo = _id + " 软触发失败,Error:" + _module.GetErrorInfo(err);
                if (null != OnTxtMsg)
                    OnTxtMsg(errInfo);
                else
                    MessageBox.Show(errInfo);
                return;
            }
            err = _module.SoftTrigge(trigChn);
            if (err != 0)
            {
                errInfo = _id + " 软触发失败,Error:" + _module.GetErrorInfo(err);
                if (null != OnTxtMsg)
                    OnTxtMsg(errInfo);
                else
                    MessageBox.Show(errInfo);
                return;
            }
            errInfo = _id + " 软触发OK!输出通道：";
            for (int i = 0; i < trigChn.Length; i++)
            {
                errInfo = errInfo + trigChn[i].ToString();
                if (i < trigChn.Length - 1)
                    errInfo = errInfo + "|";
            }
            OnTxtMsg?.Invoke(errInfo);
            UpdateChnStatus();

        }

        /// <summary>参数设置按钮</summary>
        private void btCfg_Click(object sender, EventArgs e)
        {
            if (null == _module)
            {
                MessageBox.Show("未能显示设置窗口！触发器对象未设置（空值）");
                return;
            }
            if (_encChn < 0 || _encChn >= _module.EncoderChannels)
            {
                MessageBox.Show(string.Format("未能显示设置窗口！编码器通道序号 ={0} 超出范围0~{1}", _encChn, _module.EncoderChannels - 1));
                return;
            }

            FormCmprTrigChnCfg fm = new FormCmprTrigChnCfg();
            fm.SetChnInfo(_module, _encChn, _id, _trigChnIDs) ;
            fm.ShowDialog();
            AdjustView();
        }

        /// <summary>将触发次数置0</summary>
        private void btResetCnt_Click(object sender, EventArgs e)
        {
            if (null == _module)
            {
                MessageBox.Show("触发次数归0失败！触发器对象未设置（空值）");
                return;
            }
            if (_encChn < 0 || _encChn >= _module.EncoderChannels)
            {
                MessageBox.Show(string.Format("触发次数归0失败！编码器通道序号 ={0} 超出范围0~{1}", _encChn, _module.EncoderChannels - 1));
                return;
            }
            string errInfo = "";
            int[] trigChns = null;
            int err = _module.GetEncoderTrigBind(_encChn, out trigChns);//一个编码器输入可能会绑定多个触发输出
            if (err != 0)
            {
                errInfo = _id + " 触发次数归0失败,Error:" + _module.GetErrorInfo(err);
                if (null != OnTxtMsg)
                    OnTxtMsg(errInfo);
                else
                    MessageBox.Show(errInfo);
                return;
            }
            bool isErrorHappend = false;
            for (int i = 0; i < trigChns.Length; i++)
            {
                err = _module.ResetTriggedCount(trigChns[i]);
                if(err!=0)
                {
                    isErrorHappend = true;
                    errInfo += string.Format("TrigChan = {0} 次数置0失败：{1}", trigChns[i], _module.GetErrorInfo(err));
                }
            }
            UpdateChnStatus();
            if (!isErrorHappend)
            {
                OnTxtMsg?.Invoke(_id + "置0 完成");
                return;
            }
            else
            {
                if (null != OnTxtMsg)
                    OnTxtMsg(_id + "置0失败！ " + errInfo);
                else
                    MessageBox.Show(_id + "置0失败！ " + errInfo);
            }

        }

        /// <summary>
        /// HTM触发板卡功能，同步内部编码器和外部编码器数值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSync_Click(object sender, EventArgs e)
        {
            if (null == _module)
            {
                MessageBox.Show("编码器同步失败！触发器对象未设置（空值）");
                return;
            }
            if (_encChn < 0 || _encChn >= _module.EncoderChannels)
            {
                MessageBox.Show(string.Format("编码器同步失败！编码器通道序号 ={0} 超出范围0~{1}", _encChn, _module.EncoderChannels - 1));
                return;
            }
            string errInfo = "";
            
            int err = _module.SyncEncoderCurrPos(_encChn);
            if (err != 0)
            {
                errInfo = _id + " 编码器同步失败,Error:" + _module.GetErrorInfo(err);
                if (null != OnTxtMsg)
                    OnTxtMsg(errInfo);
                else
                    MessageBox.Show(errInfo);
                return;
            }
            
            OnTxtMsg?.Invoke(_id + " 编码器同步完成");
            UpdateChnStatus();
        }

        private void UcCmprTrgChn_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                AdjustView();
        }

        //触发模式改变
        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMode.SelectedIndex < 0)
                return;
            if (_module == null)
                return;
            int errCode = 0;
            if(0 == cbMode.SelectedIndex) //禁用
            {
                errCode = _module.SetTrigMode(_encChn, JFCompareTrigMode.disable);
                if(errCode != 0)
                {
                    string err = "禁用触发功能失败，EncChn = " + _encChn + ",ErrorInfo:" + _module.GetErrorInfo(errCode);
                    if (null != OnTxtMsg)
                        OnTxtMsg(err);
                    else
                        MessageBox.Show(err);

                }
                else
                {
                    if (null != OnTxtMsg)
                        OnTxtMsg("通道：" + _encChn + " 已禁用触发");
                }

            }
            else if(1 == cbMode.SelectedIndex)//线性模式
            {
                errCode = _module.SetTrigMode(_encChn, JFCompareTrigMode.liner);
                if (errCode != 0)
                {
                    string err = "设置线性触发失败，EncChn = " + _encChn + ",ErrorInfo:" + _module.GetErrorInfo(errCode);
                    if (null != OnTxtMsg)
                        OnTxtMsg(err);
                    else
                        MessageBox.Show(err);

                }
                else
                {
                    if (null != OnTxtMsg)
                        OnTxtMsg("通道：" + _encChn + " 已设置为线性触发");
                }
            }
            else //点表模式
            {
                errCode = _module.SetTrigMode(_encChn, JFCompareTrigMode.table);
                if (errCode != 0)
                {
                    string err = "设置点表触发失败，EncChn = " + _encChn + ",ErrorInfo:" + _module.GetErrorInfo(errCode);
                    if (null != OnTxtMsg)
                        OnTxtMsg(err);
                    else
                        MessageBox.Show(err);

                }
                else
                {
                    if (null != OnTxtMsg)
                        OnTxtMsg("通道：" + _encChn + " 已设置为点表触发");
                }
            }

            if (errCode != 0)
            {
                JFCompareTrigMode currMode;
                int errCodeAgain = _module.GetTrigMode(_encChn, out currMode);
                if (0 != errCodeAgain)
                {
                    cbMode.Text = "未知";
                    cbMode.BackColor = Color.Red;
                }
                else
                {
                    cbMode.BackColor = Color.White;
                    if (currMode == JFCompareTrigMode.disable)
                        cbMode.Text = "禁用";
                    else if (currMode == JFCompareTrigMode.liner)
                        cbMode.Text = "线性";
                    else
                        cbMode.Text = "点表";
                }
            }
        }
    }
}
