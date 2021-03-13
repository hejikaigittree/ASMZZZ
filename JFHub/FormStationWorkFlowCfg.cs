using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFHub
{
    /// <summary>
    /// FormStationWorkFlowTest 用于测试StationBase中的WorkFlow（MethodFlow）
    /// </summary>
    public partial class FormStationWorkFlowCfg : Form
    {
        public FormStationWorkFlowCfg()
        {
            InitializeComponent();
        }

        private void FormStationWorkFlowTest_Load(object sender, EventArgs e)
        {
            _ucMethodFlow.Top = btSave.Bottom + 3;
            _ucMethodFlow.Left =  3;
            _ucMethodFlow.Width = Width - 6;
            _ucMethodFlow.Height = Height - _ucMethodFlow.Top - 3;
            _ucMethodFlow.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(_ucMethodFlow);
            _ucMethodFlow.Parent = this;
            _isFormLoaded = true;
            UpdateFlow2UI();
        }
        bool _isFormLoaded = false;
        UcMethodFlow _ucMethodFlow = new UcMethodFlow();
        JFStationBase _station = null;
        string _workFlowName = null;

        /// <summary>
        /// 保存变更 ， 将编辑后的内容存入工站配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSave_Click(object sender, EventArgs e)
        {
            if (_station == null)
                return;
            if (DialogResult.OK == MessageBox.Show("是否将当前工作流保存到工站配置文件?","提示信息",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning))
            {
                _station.SaveCfg();
                _ucMethodFlow.UpdateFlow2UI();
                MessageBox.Show("已将当前工作流保存到工站配置文件！");
            }
            
        }

        /// <summary>
        /// 取消变更，从工站配置文件中重新载入动作流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReload_Click(object sender, EventArgs e)
        {
            if (_station == null)
                return;
            if (DialogResult.OK == MessageBox.Show("是否从配置文件中加载工作流，并覆盖当前变更?", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
            {
                _station.LoadCfg();
                _ucMethodFlow.UpdateFlow2UI();
                MessageBox.Show("已重新加载！");
            }
        }

        public void SetStationWorkFlow(JFStationBase station,string workFlowName)
        {
            _station = station;
            _workFlowName = workFlowName;
            if (_isFormLoaded)
                UpdateFlow2UI();
        }

        void UpdateFlow2UI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateFlow2UI));
                return;
            }

            if(null == _station)
            {
                lbTital.Text = "工站:未设置";
                btReload.Enabled = false;
                btSave.Enabled = false;
                _ucMethodFlow.Enabled = false;
                return;
            }
            lbTital.Text = "工站:\"" + _station.Name + "\"";
            if(string.IsNullOrEmpty(_workFlowName))
            {
                lbTital.Text += " 工作流名称为空！";
                btReload.Enabled = false;
                btSave.Enabled = false;
                _ucMethodFlow.Enabled = false;
                return;
            }
            JFMethodFlow mf = _station.GetWorkFlow(_workFlowName);
            if(null == mf)
            {
                lbTital.Text += " 工作流名称:\"" + _workFlowName + "\"在工站中不存在！";
                btReload.Enabled = false;
                btSave.Enabled = false;
                _ucMethodFlow.Enabled = false;
                return;
            }
            lbTital.Text += " 工作流名称:\"" + _workFlowName + "\" ";
            btReload.Enabled = true;
            btSave.Enabled = true;
            _ucMethodFlow.Enabled = true;
            _ucMethodFlow.SetMethodFlow(mf);
        }
    }
}
