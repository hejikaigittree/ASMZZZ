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
    public partial class FormStationBaseWorkFlowPanel : Form
    {
        public FormStationBaseWorkFlowPanel()
        {
            InitializeComponent();
        }

        public void FormStationBaseWorkFlowPanel_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                AdjustStaionUI();
            }
            else
            {

            }
        }

        bool _isFormLoaded = false;
        private void FormStationBaseWorkFlowPanel_Load(object sender, EventArgs e)
        {
            ucSystemDataPool.SetDataPool(JFHubCenter.Instance.DataPool);
            _isFormLoaded = true;
            AdjustStaionUI();
        }

        private void FormStationBaseWorkFlowPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MdiParent != null && CloseReason.MdiFormClosing != e.CloseReason)//在MDI中不关闭窗口
            {
                Hide();
                e.Cancel = true;
            }
        }
        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
            if (null != _station)
                ucStationDataPool.SetDataPool(_station.DataPool);
            if(_isFormLoaded)
                AdjustStaionUI();
        }

        void AdjustStaionUI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustStaionUI));
                return;
            }
            tabControlCF1.TabPages.Clear();
            if(null == _station)
                return;
            string[] flowNames = _station.WorkFlowNames;
            if (null == flowNames)
                return;
            foreach(string flowName in flowNames)
            {
                TabPage tp = new TabPage(flowName);
                FormStationWorkFlowCfg fm = new FormStationWorkFlowCfg();
                fm.FormBorderStyle = FormBorderStyle.None;
                tabControlCF1.TabPages.Add(tp);
                fm.TopLevel = false;
                tp.Controls.Add(fm);
                fm.Parent = tp;
                fm.Dock = DockStyle.Fill;
                fm.SetStationWorkFlow(_station, flowName);
                fm.Show();
            }
        }
    }
}
