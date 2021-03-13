using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;
using JFUI;

namespace JFHub
{
    /// <summary>
    /// 用于调试IJFMethod的窗口类，包含Init参数设置 ，RealtimeUI
    /// </summary>
    public partial class FormMethodUI : Form
    {
        public FormMethodUI()
        {
            InitializeComponent();
        }

        private void FormMethodUI_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustView();
            ContextMenuStrip = contextMenuStrip1;
        }
        bool _isFormLoaded = false;
        IJFMethod _method = null;
        //JFRealtimeUI _realtimeUI = null;
        List<UcJFParamEdit> _lstUcParam = new List<UcJFParamEdit>();//保存初始化参数编辑面板
        public void SetMethod(IJFMethod method)
        {
            _method = method;
            if (_isFormLoaded)
                AdjustView();
        }

        Button btSetSaveInit = null;
        Button btCancelInit = null;
        void AdjustView()
        {
            timerFlush.Enabled = false;
            tpRealtime.Controls.Clear();
            tpInitCfg.Controls.Clear();
            _lstUcParam.Clear();
            if (null == _method)
            {
                Label lb1 = new Label();
                lb1.Text = "方法对象未设置";
                tpRealtime.Controls.Add(lb1);
                Label lb2 = new Label();
                lb2.Text = "方法对象未设置";
                tpInitCfg.Controls.Add(lb1);
                return;
            }
            if(_method is IJFRealtimeUIProvider)
            {
                JFRealtimeUI ui = (_method as IJFRealtimeUIProvider).GetRealtimeUI();
                if (null != ui)
                {
                    ui.Dock = DockStyle.Fill;
                    tpRealtime.Controls.Add(ui);
                    timerFlush.Enabled = true;
                }
            }
            else
            {
                UcCommonMethodRtUi ui = new UcCommonMethodRtUi();
                ui.Dock = DockStyle.Fill;
                ui.SetMethod(_method);
                tpRealtime.Controls.Add(ui);
                timerFlush.Enabled = true;
            }

            int locX = 3, locY = 3;
            if(_method is IJFConfigUIProvider)//提供参数配置界面
            {
                Button btShowCfg = new Button();
                btShowCfg.Text = "参数配置";
                btShowCfg.Location = new Point(locX, locY);
                tpInitCfg.Controls.Add(btShowCfg);
                btShowCfg.Click += OnButtonClick_ShowCfgUI;
                locY = btShowCfg.Bottom + 2;
            }

            if(_method is IJFInitializable)//初始参数可序列化对象
            {
                IJFInitializable initor = _method as IJFInitializable;
                string[] initNames = initor.InitParamNames;
                if(null != initNames && initNames.Length > 0)
                {
                    isInitParamEditting = false;
                    btSetSaveInit = new Button();
                    btSetSaveInit.Text = "编辑初始化参数";
                    btSetSaveInit.Location = new Point(locX, locY);
                    btSetSaveInit.Click += OnButtonClick_InitParamSetSave;
                    tpInitCfg.Controls.Add(btSetSaveInit);

                    btCancelInit = new Button();
                    btCancelInit.Text = "取消";
                    btCancelInit.Location = new Point(btSetSaveInit.Right+2, locY);
                    btCancelInit.Click += OnButtonClick_InitParamCancel;
                    tpInitCfg.Controls.Add(btCancelInit);
                    btCancelInit.Enabled = false;
                    locY = btCancelInit.Bottom + 2;
                    for(int i = 0;i < initNames.Length;i++)
                    {
                        string initName = initNames[i];
                        JFParamDescribe pd = initor.GetInitParamDescribe(initName);
                        UcJFParamEdit ucParam = new UcJFParamEdit();
                        ucParam.Width = tpInitCfg.Width - 100;
                        ucParam.Location = new Point(locX, locY);
                        ucParam.SetParamDesribe(pd);
                        ucParam.SetParamValue(initor.GetInitParamValue(initName));
                        ucParam.IsValueReadOnly = true;
                        _lstUcParam.Add(ucParam);
                        tpInitCfg.Controls.Add(ucParam);
                        
                        ucParam.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                        locY = ucParam.Bottom + 2;
                    }

                    InitParam2UI();
                }
            }

        }

        /// <summary>
        /// 显示参数设置界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnButtonClick_ShowCfgUI(object sender,EventArgs e)
        {
            (_method as IJFConfigUIProvider).ShowCfgDialog();
        }


        bool isInitParamEditting = false;
        /// <summary>
        /// 设置/保存 初始化参数 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnButtonClick_InitParamSetSave(object sender, EventArgs e)
        {
            if(!isInitParamEditting) //开放参数编辑
            {
                isInitParamEditting = true;
                foreach (UcJFParamEdit pe in _lstUcParam)
                    pe.IsValueReadOnly = false;
                btSetSaveInit.Text = "保存初始化参数";
                btCancelInit.Enabled = true;
            }
            else //保存参数修改
            {
                if(UI2InitParam())
                {
                    foreach (UcJFParamEdit pe in _lstUcParam)
                        pe.IsValueReadOnly = true;
                    btSetSaveInit.Text = "编辑初始化参数";
                    btCancelInit.Enabled = false;
                    isInitParamEditting = false;
                    ///添加方法对象的（重新）初始化动作
                    IJFInitializable initor = _method as IJFInitializable;
                    foreach (UcJFParamEdit pe in _lstUcParam)
                    {
                        string initParamName = pe.GetParamDesribe().DisplayName;
                        object paramValue = null;
                        bool isOK = pe.GetParamValue(out paramValue);
                        if(!isOK)
                        {
                            MessageBox.Show("未能重新初始化方法对象\n请检查参数:\"" + initParamName + "\"");
                            return;
                        }
                        isOK = initor.SetInitParamValue(initParamName, paramValue);
                        if(!isOK)
                        {
                            MessageBox.Show("未能重新初始化方法对象\n设置参数:\"" + initParamName + "\"失败:" + initor.GetInitErrorInfo());
                            return;
                        }
                        isOK = initor.Initialize();
                        if(!isOK)
                        {
                            MessageBox.Show("重新初始化方法对象失败\n错误信息:" + initor.GetInitErrorInfo());
                            return;
                        }

                    }
                }
            }
        }


        /// <summary>
        /// 取消保存初始化参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnButtonClick_InitParamCancel(object sender, EventArgs e)
        {
            InitParam2UI();
            foreach (UcJFParamEdit pe in _lstUcParam)
                pe.IsValueReadOnly = true;
            btSetSaveInit.Text = "编辑初始化参数";
            btCancelInit.Enabled = false;
            isInitParamEditting = false;
        }

        /// <summary>
        /// 将初始化参数加载到界面上
        /// </summary>
        void InitParam2UI()
        {
            if (null == _method)
                return;
            if (!(_method is IJFInitializable))
                return;
            IJFInitializable initor = _method as IJFInitializable;
            string[] initName = initor.InitParamNames;
            if (null == initName)
                return;
            for (int i = 0; i < initName.Length; i++)
                _lstUcParam[i].SetParamValue(initor.GetInitParamValue(initName[i]));
        }

        /// <summary>
        /// 将界面上的初始化参数保存到Method中
        /// </summary>
        /// <returns></returns>
        bool UI2InitParam()
        {
            if (null == _method)
                return true;
            if (!(_method is IJFInitializable))
                return true;
            IJFInitializable initor = _method as IJFInitializable;
            string[] initName = initor.InitParamNames;
            if (null == initName)
                return true;
            for (int i = 0; i < initName.Length; i++)
            {
                object val;
                if(!_lstUcParam[i].GetParamValue(out val))
                {
                    MessageBox.Show("参数\"" + initName[i] + "\"设置失败，错误信息:\n" + _lstUcParam[i].GetParamErrorInfo());
                    return false;
                }
                if(!initor.SetInitParamValue(initName[i],val))
                {
                    MessageBox.Show("参数\"" + initName[i] + "\"设置失败，错误信息:\n" + initor.GetInitErrorInfo());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 刷新RealTime界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerFlush_Tick(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 0)
                return;
            JFRealtimeUI ui = tpRealtime.Controls[0] as JFRealtimeUI;
            if(null != ui)
                ui.UpdateSrc2UI();
        }

        private void tpRealtime_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void ToolStripMenuItem_Action_Click(object sender, EventArgs e)
        {
            if (null == _method)
                return;
            bool isActionOK = _method.Action();
            if(!isActionOK)
            {
                MessageBox.Show("执行失败！错误信息：" + _method.GetActionErrorInfo());
                return;
            }
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (/*e.Button == MouseButtons.Right &&*/ null != _method && tabControl1.SelectedIndex == 0)
                ContextMenuStrip = contextMenuStrip1;
            else
                ContextMenuStrip = null;


        }



        private void chkAutoFlush_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkAutoFlush.Checked)
                timerFlush.Enabled = true;
            else
                timerFlush.Enabled = false;
        }

        private void btUpdateRealtimeUI_Click(object sender, EventArgs e)
        {
            JFRealtimeUI ui = tpRealtime.Controls[0] as JFRealtimeUI;
            if (null != ui)
                ui.UpdateSrc2UI();
        }
    }
}
