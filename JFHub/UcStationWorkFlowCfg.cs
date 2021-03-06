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
    /// <summary>
    /// 用于配置StationBase中的工作流
    /// </summary>
    public partial class UcStationWorkFlowCfg : UserControl
    {
        public UcStationWorkFlowCfg()
        {
            InitializeComponent();
            dgvWorkFlows.CellContentClick += DataGridViewCellContentClick;
        }

        bool _isFormLoaded = false;
        JFStationBase _station = null;
        private void UcStationWorkFlowCfg_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            
            //UpdateStation2UI();
        }

        public void SetStation(JFStationBase station)
        {
            _station = station;
            if (_isFormLoaded)
                UpdateStation2UI();
        }


        void UpdateStation2UI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateStation2UI));
                return;
            }
            dgvWorkFlows.Rows.Clear();
            if(null == _station)
            {
                lbTital.Text = "工站:\"未设置\" - 工作流配置";
                btAdd.Enabled = false;
                btDel.Enabled = false;  
                return;
            }
            btAdd.Enabled = true;
            lbTital.Text = string.Format("工站:\"{0}\" - 工作流配置",_station.Name);
            string[] flowNames = _station.WorkFlowNames;
            if(null == flowNames || 0 == flowNames.Length)
            {
                btDel.Enabled = false;
                return;
            }
            foreach(string flowName in flowNames)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cellFlowName = new DataGridViewTextBoxCell();
                cellFlowName.Value = flowName;
                row.Cells.Add(cellFlowName);
                DataGridViewButtonCell cellTest = new DataGridViewButtonCell();
                cellTest.Value = "设置";
                row.Cells.Add(cellTest);
                DataGridViewButtonCell cellImport = new DataGridViewButtonCell();
                cellImport.Value = "导入";
                row.Cells.Add(cellImport);
                DataGridViewButtonCell cellExport = new DataGridViewButtonCell();
                cellExport.Value = "导出";
                row.Cells.Add(cellExport);
                dgvWorkFlows.Rows.Add(row);
            }
            

        }

        /// <summary>
        /// 新建工作流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAdd_Click(object sender, EventArgs e)
        {
            FormAddFlow2Station dlg = new FormAddFlow2Station();
            dlg.Text = _station.Name + " 添加工作流";
            if (DialogResult.OK != dlg.ShowDialog())
                return;
            string flowName2Add = dlg.WorkFlowName;
            string[] existNames = _station.WorkFlowNames;
            if(null != existNames)
                foreach(string nm in existNames)
                    if(nm == flowName2Add)
                    {
                        MessageBox.Show("添加工作流失败！已存在同名称对象:" + flowName2Add);
                        return;
                    }
            JFMethodFlow flow2Add = new JFMethodFlow();
            if (dlg.IsLoadedFromFile)
                flow2Add.Load(dlg.FilePath);
            flow2Add.Name = flowName2Add;
            _station.AddWorkFlow(flow2Add);
            _station.SaveCfg();
            UpdateStation2UI();

        }

        /// <summary>
        /// 删除工作流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDel_Click(object sender, EventArgs e)
        {
            if(null == dgvWorkFlows.SelectedRows || 0 == dgvWorkFlows.SelectedRows.Count)
            {
                MessageBox.Show("请先选择需要删除的对象！");
                return;
            }

            List<string> _delFlowNames = new List<string>();
            foreach (DataGridViewRow row in dgvWorkFlows.SelectedRows)
            {

                string wfName = row.Cells[0].Value.ToString();
                if(_station.IsMethodFlowDecleared(wfName))
                {
                    MessageBox.Show("工作流:\"" + wfName + "\"为工站固有属性，不可删除");
                    return;
                }
                _delFlowNames.Add(wfName);
            }

            if(DialogResult.OK == MessageBox.Show("确定要删除以下动作流？\n" + string.Join("\n", _delFlowNames)))
            {
                foreach (string delName in _delFlowNames)
                    _station.RemoveWorkFlow(delName);
                _station.SaveCfg();
                UpdateStation2UI();
            }
        }

        void DataGridViewCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1) //调试工作流
            {
                FormStationWorkFlowCfg dlg = new FormStationWorkFlowCfg();
                string workFlowName = dgvWorkFlows.Rows[e.RowIndex].Cells[0].Value as string;
                dlg.SetStationWorkFlow(_station, workFlowName);
                dlg.ShowDialog();
            }
            else if (e.ColumnIndex == 2) //从文件中导入
            {
                string workFlowName = dgvWorkFlows.Rows[e.RowIndex].Cells[0].Value as string;
                JFMethodFlow flow = _station.GetWorkFlow(workFlowName);
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "JF流程文件(*.jff) | *.jff";
                //ofd.InitialDirectory = Application.StartupPath;
                ofd.ValidateNames = true;
                ofd.CheckPathExists = true;
                ofd.CheckFileExists = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (DialogResult.OK == MessageBox.Show(string.Format("确定将文件:{0} 导入到工作流{1}?", ofd.FileName, workFlowName)))
                    {
                        try
                        {
                            flow.Load(ofd.FileName);
                            _station.SaveCfg();
                            MessageBox.Show("导入成功!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("载入失败！异常信息：" + ex.ToString());
                        }
                    }


                }
            }
            else if (e.ColumnIndex == 3) //导出工作流到文件
            {
                string workFlowName = dgvWorkFlows.Rows[e.RowIndex].Cells[0].Value as string;
                JFMethodFlow flow = _station.GetWorkFlow(workFlowName);
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "JF流程文件(*.jff) | *.jff";
                //ofd.InitialDirectory = Application.StartupPath;
                ofd.ValidateNames = true;
                ofd.CheckPathExists = true;
                ofd.CheckFileExists = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    flow.Save(ofd.FileName);
                    MessageBox.Show("导出文件成功!");
                }
            }
        }

        private void UcStationWorkFlowCfg_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                UpdateStation2UI();
            else
            {

            }
        }
    }
}
