using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFHub
{
    public partial class UcStationBaseSPAliasEdit : UserControl
    {
        public UcStationBaseSPAliasEdit()
        {
            InitializeComponent();
        }

        private void UcStationBaseSPAliasEdit_Load(object sender, EventArgs e)
        {
            if (Parent != null)
                Parent.VisibleChanged += new System.EventHandler(this.UcStationBaseSPAliasEdit_VisibleChanged);//将当前的VisibalChange绑定到父控件上


            _isFormLoaded = true;
            AdjustStationView();
        }

        void AdjustStationView()
        {
            dgv.Rows.Clear();
            _isEditting = false;
            btEditCancel.Enabled = false;
            if(_station == null)
            {
                btEditSave.Enabled = false;
                label1.Text = "工站:未设置 系统变量映射表编辑界面";
                return;
            }
            string[] allAliasSPNames = _station.AllSPAliasNames;
            if(null == allAliasSPNames || 0 == allAliasSPNames.Length)
            {
                btEditSave.Enabled = false;
                label1.Text = "工站:" + _station.Name + " 系统变量映射表编辑界面:无系统变量替身";
                return;
            }
            label1.Text = "工站:" + _station.Name + " 系统变量映射表编辑界面";
            btEditSave.Text = "编辑";
            btEditSave.Enabled = true;
            foreach(string aliasName in allAliasSPNames)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cellAlias = new DataGridViewTextBoxCell();
                cellAlias.Value = aliasName;
                row.Cells.Add(cellAlias);
                DataGridViewTextBoxCell cellRealName = new DataGridViewTextBoxCell();
                //string realName = _station.GetSPAliasRealName(aliasName);
                //if (null != realName)
                    cellRealName.Value = _station.GetSPAliasRealName(aliasName);
                row.Cells.Add(cellRealName);
                dgv.Rows.Add(row);
            }
            dgv.Columns[1].ReadOnly = true;
        }


        public void SetStation(JFStationBase station)
        {
            _station = station;
            if(_isFormLoaded)
                AdjustStationView();
        }
        bool _isFormLoaded = false;
        JFStationBase _station = null;
        bool _isEditting = false;


        //编辑/保存
        private void btEditSave_Click(object sender, EventArgs e)
        {
            if(!_isEditting)
            {
                _isEditting = true;
                btEditCancel.Enabled = true;
                btEditSave.Text = "保存";
                dgv.Columns[1].ReadOnly = false;
            }
            else
            {
                if(Save2Station())
                {
                    _isEditting = false;
                    btEditCancel.Enabled = false;
                    btEditSave.Text = "编辑";
                    dgv.Columns[1].ReadOnly = true;
                }

                _station.SaveCfg();
                MessageBox.Show("已保存变更！");
            }
        }

        /// <summary>
        /// 取消编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btEditCancel_Click(object sender, EventArgs e)
        {
            _isEditting = false;
            btEditCancel.Enabled = false;
            btEditSave.Text = "编辑";

            string[] allAliasSPNames = _station.AllSPAliasNames;
            for(int i = 0; i < allAliasSPNames.Length;i++)//foreach (string aliasName in allAliasSPNames)
            {
                string aliasName = allAliasSPNames[i];
                    dgv.Rows[i].Cells[1].Value = _station.GetSPAliasRealName(aliasName);
            }
            dgv.Columns[1].ReadOnly = true;
        }

        /// <summary>
        /// 将界面变更保存到工站配置
        /// </summary>
        /// <returns></returns>
        public bool Save2Station()
        {
            ///先检查所有参数是否合法
            bool isOK = true;
            StringBuilder sbError = new StringBuilder();
            foreach(DataGridViewRow row in dgv.Rows)
            {
                string aliasName = row.Cells[0].Value as string;
                string realName = row.Cells[1].Value as string;
                if(string.IsNullOrEmpty(realName))
                {
                    isOK = false;
                    sbError.AppendLine("替代名:\"" + aliasName + "\" 未设置真名参数");
                }

            }
            if(!isOK)
            {
                MessageBox.Show("保存失败!错误信息:\n" + sbError.ToString());
                return false;
            }
            isOK = true;
            sbError.Clear();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                string aliasName = row.Cells[0].Value as string;
                string realName = row.Cells[1].Value as string;
                if (!_station.SetSPAliasRealName(aliasName, realName))
                {
                    isOK = false;
                    sbError.AppendLine("替代名:\"" + aliasName + "\" 设置真名:\"" + realName + "\"失败");
                }
            }
            if (!isOK)
            {
                MessageBox.Show("保存失败!请在此确认修改参数\n错误信息:\n" + sbError.ToString());
                return false;
            }
            return true;
        }

        private void UcStationBaseSPAliasEdit_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                AdjustStationView();

            }
            else
            {
                if (_station == null)
                    return;
                if (_isEditting)
                {
                    if (DialogResult.OK == MessageBox.Show("即将离开正在编辑的系统数据映射界面\n是否保存当前变更？", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    {
                        if (!Save2Station())
                            MessageBox.Show("请再次确认需要修改的参数");
                    }
                    AdjustStationView();
                    return;

                }
            }
        }
    }
}
