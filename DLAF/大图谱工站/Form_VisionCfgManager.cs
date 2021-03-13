using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFToolKits;

namespace DLAF
{
    public partial class Form_VisionCfgManager : Form
    {
        public JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string, string>>> _visionCfgParams;
                            
        public Form_VisionCfgManager(JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string, string>>> visionCfgParams)
        {
            InitializeComponent();
            _visionCfgParams = new JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string, string>>>();
            _visionCfgParams = visionCfgParams;
        }

        //保存VisonCfgNames
        private void btSave_Click(object sender, EventArgs e)
        {
            if (!UpdateVisionCfgNames())
                return;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Form_VisionCfgManager_Load(object sender, EventArgs e)
        {
            int x = (System.Windows.Forms.SystemInformation.WorkingArea.Width - this.Size.Width) / 2;
            int y = (System.Windows.Forms.SystemInformation.WorkingArea.Height - this.Size.Height) / 2;
            this.Location = (Point)new Size(x, y); //窗体的起始位置为(x,y)

            dgvVisionCfg.AllowUserToAddRows = false;    //禁止添加行
            dgvVisionCfg.AllowUserToDeleteRows = false;   //禁止删除行

            DataGridViewLinkColumn fovNameColumn = new DataGridViewLinkColumn();
            fovNameColumn.HeaderText = "视野名称";
            dgvVisionCfg.Columns.Add(fovNameColumn);

            DataGridViewTextBoxColumn visionCfgNumColumn = new DataGridViewTextBoxColumn();
            visionCfgNumColumn.HeaderText = "光源配置序号";
            dgvVisionCfg.Columns.Add(visionCfgNumColumn);

            DataGridViewLinkColumn visionCfgNameColumn = new DataGridViewLinkColumn();
            visionCfgNameColumn.HeaderText = "光源配置名称";
            dgvVisionCfg.Columns.Add(visionCfgNameColumn);

            SetupUI();
        }

        /// <summary>
        /// 更新UI
        /// </summary>
        private void SetupUI()
        {
            if (_visionCfgParams != null)
            {
                int fovCount = _visionCfgParams.Keys.Count;
                for(int i=0;i<fovCount;i++)
                {
                    JFXmlDictionary<string, JFXmlDictionary<string, string>> fovVisionCfg = _visionCfgParams[i.ToString()];
                    foreach (string fovname in fovVisionCfg.Keys)
                    {
                        JFXmlDictionary<string, string> keyValuePairs = fovVisionCfg[fovname];
                        foreach(string taskname in keyValuePairs.Keys)
                        {
                            AddDataGridViewFunction(dgvVisionCfg, fovname, taskname, keyValuePairs[taskname].ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新VisionCfgName
        /// </summary>
        private bool UpdateVisionCfgNames()
        {
            int startIndex = 0;
            int endIndex = 0;
            int fovCount = _visionCfgParams.Keys.Count;
            for(int m=0;m<fovCount;m++)
            {
                JFXmlDictionary<string, JFXmlDictionary<string, string>> dicFovVisionCfgName = _visionCfgParams[m.ToString()];
                foreach (string fovname in dicFovVisionCfgName.Keys)
                {
                    JFXmlDictionary<string, string> visionCfgNames = dicFovVisionCfgName[fovname];
                    endIndex = startIndex + visionCfgNames.Keys.Count;

                    visionCfgNames.Clear();
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (dgvVisionCfg.Rows[i].Cells[1].Value.ToString() == "")
                        {
                            MessageBox.Show(string.Format("当前行{0}的Task名称不可为空", i));
                            return false;
                        }
                        if(visionCfgNames.ContainsKey(dgvVisionCfg.Rows[i].Cells[1].Value.ToString()))
                        {
                            MessageBox.Show(string.Format("视野名称为{0}的光源配置名{1}已存在", fovname, dgvVisionCfg.Rows[i].Cells[1].Value.ToString()));
                            return false;
                        }
                        visionCfgNames.Add(dgvVisionCfg.Rows[i].Cells[1].Value.ToString(), dgvVisionCfg.Rows[i].Cells[2].Value.ToString());
                    }
                    startIndex = endIndex;
                }
            }
            return true;
        }

        private delegate void AddDisplayDateGridView(System.Windows.Forms.DataGridView dataGridView, string fovName, string taskname, string visionCfgName);
        /// <summary>
        /// 新增DataGridView控件数据
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="dateTime"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        private void AddDataGridViewFunction(System.Windows.Forms.DataGridView dataGridView, string fovName, string taskname, string visionCfgName)
        {
            if (dataGridView.InvokeRequired)
            {
                AddDisplayDateGridView ss = new AddDisplayDateGridView(AddDataGridViewFunction);
                Invoke(ss, new object[] { dataGridView, fovName, taskname, visionCfgName });
            }
            else
            {
                int index = dataGridView.Rows.Add();
                dataGridView.Rows[index].Cells[0].Value = fovName;
                dataGridView.Rows[index].Cells[1].Value = taskname;
                dataGridView.Rows[index].Cells[2].Value = visionCfgName;
                dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.Rows.Count - 1;
                System.Windows.Forms.Application.DoEvents();
            }
        }
    }
}
