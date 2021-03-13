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
    /// 如果JFMethod方法未提供Realtime接口，则使用UcCommonMethodRtUi对象代替
    /// 未完成，待实现 ... 
    /// </summary>
    public partial class UcCommonMethodRtUi : JFRealtimeUI
    {
        public UcCommonMethodRtUi()
        {
            InitializeComponent();
        }

        bool _isFormLoaded = false;
        private void UcCommonMethodRtUi_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustView();
        }

        void AdjustView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustView));
                return;
            }
            pnInputParam.Controls.Clear();
            pnOutputParam.Controls.Clear();
            if(null == _method)
            {
                lbInfo.Text = " 方法未设置";
                btSetSave.Enabled = false;
                btCancel.Enabled = false;
                btAction.Enabled = false;
                return;
            }
            lbInfo.Text = " ";
            _isEditting = false;
            btSetSave.Enabled = true;
            btCancel.Enabled = true;
            btAction.Enabled = true;
            string[] inputNames = _method.MethodInputNames;
            
            if (inputNames != null)
            {
                foreach (string inputName in inputNames)
                {
                    UcJFParamEdit ucParam = new UcJFParamEdit();
                    ucParam.IsHelpVisible = false;
                    ucParam.Height = 50;

                    //ucParam.SetParamType(_cfg.GetItemValue(itemName).GetType());//ucParam.SetParamDesribe(_station.GetCfgParamDescribe(itemName));
                    ucParam.SetParamDesribe(JFParamDescribe.Create(inputName, _method.GetMethodInputType(inputName), JFValueLimit.NonLimit, null));
                    ucParam.SetParamValue(_method.GetMethodInputValue(inputName));
                    pnInputParam.Controls.Add(ucParam);
                    ucParam.IsValueReadOnly = true;
                }
            }

            string[] outputNames = _method.MethodOutputNames;
            if (null != outputNames)
            {
                foreach (string outputName in outputNames)
                {
                    UcJFParamEdit ucParam = new UcJFParamEdit();
                    ucParam.IsHelpVisible = false;
                    ucParam.Height = 50;

                    //ucParam.SetParamType(_cfg.GetItemValue(itemName).GetType());//ucParam.SetParamDesribe(_station.GetCfgParamDescribe(itemName));
                    ucParam.SetParamDesribe(JFParamDescribe.Create(outputName, _method.GetMethodOutputType(outputName), JFValueLimit.NonLimit, null));
                    ucParam.SetParamValue(_method.GetMethodOutputValue(outputName));
                    pnOutputParam.Controls.Add(ucParam);
                    ucParam.IsValueReadOnly = false;
                }
            }
        }
        IJFMethod _method = null;
        public void SetMethod(IJFMethod method)
        {
            _method = method;
            if (_isFormLoaded)
                AdjustView();
        }

        public override void UpdateSrc2UI()
        {
            if (null == _method)
                return;
            string[] outputNames = _method.MethodOutputNames;
            if (null != outputNames)
            {
                for(int i = 0; i < outputNames.Length;i++)//foreach (string outputName in outputNames)
                {
                    string outputName = outputNames[i];
                    UcJFParamEdit ucParam = pnOutputParam.Controls[i] as UcJFParamEdit;

                    ucParam.SetParamValue(_method.GetMethodOutputValue(outputName));
                }
            }

        }

        bool _isEditting = false;
        /// <summary>
        ///  编辑输入参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSetSave_Click(object sender, EventArgs e)
        {
            if(!_isEditting)
            {
                _isEditting = true;
                btCancel.Enabled = true;
                btSetSave.Text = "保存输入参数项";
                btAction.Enabled = false;
                foreach (UcJFParamEdit pe in pnInputParam.Controls)
                    pe.IsValueReadOnly = false;
            }
            else
            {
                foreach(UcJFParamEdit pe in pnInputParam.Controls)
                {
                    string inputName = pe.GetParamDesribe().DisplayName;

                    object inputVal = null;
                    if(!pe.GetParamValue(out inputVal))
                    {
                        MessageBox.Show("未能保存输入参数:\"" + inputName + "\",请检查参数格式");
                        return;
                    }
                    _method.SetMethodInputValue(inputName, inputVal);
                }

                foreach (UcJFParamEdit pe in pnInputParam.Controls)
                    pe.IsValueReadOnly = true;

                _isEditting = false;
                btCancel.Enabled = false;
                btSetSave.Text = "设置输入参数项";
                btAction.Enabled = true;
                MessageBox.Show("参数已保存");
            }
        }

        /// <summary>
        /// 取消编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, EventArgs e)
        {
            string[] inputNames = _method.MethodInputNames;
            if (null != inputNames)
            {
                for (int i = 0; i < inputNames.Length; i++)//foreach (string outputName in outputNames)
                {
                    string inputName = inputNames[i];
                    UcJFParamEdit ucParam = pnInputParam.Controls[i] as UcJFParamEdit;

                    ucParam.SetParamValue(_method.GetMethodInputValue(inputName));
                    ucParam.IsValueReadOnly = true;
                }
            }
            _isEditting = false;
            btCancel.Enabled = false;
            btSetSave.Text = "设置输入参数项";
            btAction.Enabled = true;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAction_Click(object sender, EventArgs e)
        {
            if(!_method.Action())
            {
                MessageBox.Show("运行失败:" + _method.GetActionErrorInfo());
                return;
            }
            lbInfo.Text = " 运行成功，耗时：" + _method.GetActionSeconds().ToString("F3") + "秒";
        }
    }
}
