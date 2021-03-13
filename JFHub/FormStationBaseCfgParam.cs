using JFToolKits;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFUI;

namespace JFHub
{
    /// <summary>
    ///  用于显示
    /// </summary>
    public partial class FormStationBaseCfgParam : Form
    {
        public FormStationBaseCfgParam()
        {
            InitializeComponent();
        }

        public void SetStation(JFStationBase station)
        {
            _station = station;
            if(_isFormLoaded)
                AdjustStationView();
        }

        JFStationBase _station = null;
        bool _isFormLoaded = false;
        private void FormStationCfgParam_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustStationView();
        }

        bool isEditting = false;
        void AdjustStationView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustStationView));
                return;
            }
            tabControlCF1.TabPages.Clear();
            isEditting = false;
            btEditSave.Text = "编辑";
            btCancel.Enabled = false;
            if(null == _station)
            {
                lbInfo.Text = "工站未设置！";
                btEditSave.Enabled = false;
                return;
            }
            lbInfo.Text = "工站:" + _station.Name;
            JFXCfg cfg = _station.Config;
            string[] categorys = cfg.AllTags;
            if(null == categorys || categorys.Length < 2) //只有一个无名称Tag，由于保存私有配置
            {
                lbInfo.Text += " 无定制化参数";
                btEditSave.Enabled = false;
                return;
            }
            btEditSave.Enabled = true;
            foreach (string category in categorys)
            {
                if (string.IsNullOrEmpty(category))
                    continue;
                TabPage tp = new TabPage(category);
                tabControlCF1.TabPages.Add(tp);
                string[] itemNames = cfg.ItemNamesInTag(category);
                if (null == itemNames)
                    continue;
                TableLayoutPanel panel = new TableLayoutPanel();
                panel.ColumnCount = 1;
                panel.AutoScroll = true;
                panel.Dock = DockStyle.Fill;
                tp.Controls.Add(panel);
                foreach (string itemName in itemNames)
                {
                    UcJFParamEdit ucParam = new UcJFParamEdit();
                    ucParam.IsHelpVisible = false;
                    ucParam.Height = 23;
                    ucParam.SetParamDesribe(_station.GetCfgParamDescribe(itemName));
                    ucParam.SetParamValue(_station.GetCfgParamValue(itemName));
                    ucParam.IsValueReadOnly = false;
                    panel.Controls.Add(ucParam);
                    ucParam.IsValueReadOnly = true;
                }
            }
            //if (tabControlCF1.TabCount > 0)
            //    tabControlCF1.SelectedIndex = 0;


        }

        private void FormStationCfgParam_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MdiParent != null && CloseReason.MdiFormClosing != e.CloseReason)//在MDI中不关闭窗口
            {
                Hide();
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 编辑/保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btEditSave_Click(object sender, EventArgs e)
        {
            if (tabControlCF1.SelectedIndex < 0)
                return;
            if(!isEditting)
            {
                TableLayoutPanel panel = tabControlCF1.SelectedTab.Controls[0] as TableLayoutPanel;
                if(0 == panel.Controls.Count)
                {
                    MessageBox.Show("没有可供编辑的参数项");
                    return;
                }
                foreach (UcJFParamEdit ucParam in panel.Controls)
                    ucParam.IsValueReadOnly = false;
                isEditting = true;
                btEditSave.Text = "保存";
                btCancel.Enabled = true;
            }
            else
            {
                TableLayoutPanel panel = tabControlCF1.SelectedTab.Controls[0] as TableLayoutPanel;
                foreach (UcJFParamEdit ucParam in panel.Controls)
                {
                    string paramName = ucParam.GetParamDesribe().DisplayName;
                    object paramVal;
                    if(!ucParam.GetParamValue(out paramVal))
                    {
                        MessageBox.Show("未能获取参数 Name = " + paramName);
                        return;
                    }
                    //_station.SetCfgParamValue(paramName, paramVal);
                    //ucParam.IsValueReadOnly = false;
                }

                foreach (UcJFParamEdit ucParam in panel.Controls)
                {
                    string paramName = ucParam.GetParamDesribe().DisplayName;
                    object paramVal;
                    ucParam.GetParamValue(out paramVal) ;
                    _station.SetCfgParamValue(paramName, paramVal);
                    ucParam.IsValueReadOnly = true;
                }
                _station.SaveCfg();

                isEditting = false;
                btEditSave.Text = "编辑";
                btCancel.Enabled = false;
            }
        }

        /// <summary>
        /// 将当前所选页的工站配置更新到界面上
        /// </summary>
        void UpdateCurrPage(bool enabled = false)
        {
            TabPage currTP = tabControlCF1.SelectedTab;
            if (null == currTP)
                return;
            TableLayoutPanel currPanel = currTP.Controls[0] as TableLayoutPanel;
            foreach(UcJFParamEdit ucParam in currPanel.Controls)
            {
                object paramVal = _station.GetCfgParamValue(ucParam.GetParamDesribe().DisplayName);
                ucParam.SetParamValue(paramVal);
                ucParam.IsValueReadOnly = !enabled;
            }
        }

        /// <summary>
        /// 取消编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, EventArgs e)
        {
            _station.LoadCfg();
            //AdjustStationView();
            UpdateCurrPage();
            isEditting = false;
            btCancel.Enabled = false;
            btEditSave.Text = "编辑";
        }

        public void FormStationCfgParam_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {

            }
            else
            {
                if(isEditting)
                {
                    if (DialogResult.OK == MessageBox.Show("退出窗口前是否保存当前变更？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    {
                        TableLayoutPanel panelEditted = tabControlCF1.SelectedTab.Controls[0] as TableLayoutPanel;
                        foreach (UcJFParamEdit ucParam in panelEditted.Controls)
                        {
                            string paramName = ucParam.GetParamDesribe().DisplayName;
                            object paramVal;
                            ucParam.GetParamValue(out paramVal);
                            _station.SetCfgParamValue(paramName, paramVal);
                            ucParam.IsValueReadOnly = true;
                        }
                        _station.SaveCfg();
                        btEditSave.Text = "编辑";
                        btCancel.Enabled = false;
                        //isEditting = false;
                    }
                    else
                        UpdateCurrPage();

                    isEditting = false;
                }
            }
        }

        private void tabControlCF1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (isEditting)
            {
                MessageBox.Show("当前页正处于编辑状态，请先保存或取消");
                e.Cancel = true;
                return;
            }
        }
    }
}
