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
    /// 用于添加名称列表的窗口
    /// </summary>
    public partial class FormAddNames : Form
    {
        public FormAddNames()
        {
            InitializeComponent();
        }

        private void FormAddNames_Load(object sender, EventArgs e)
        {

        }

        private void btOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        delegate void dgSetAvailedNames(string[] names, string[] infos);
        public void SetAvailedNames(string[] names,string[] infos = null)
        {
            if(InvokeRequired)
            {
                Invoke(new dgSetAvailedNames(SetAvailedNames), new object[] { names, infos });
                return;
            }
            dgvAvailedNames.Rows.Clear();
            if (null == names)
                return;
            for(int i = 0; i < names.Length;i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                cellName.Value = names[i];
                row.Cells.Add(cellName);
                DataGridViewTextBoxCell cellInfo = new DataGridViewTextBoxCell();
                if (null != infos)
                    cellInfo.Value = infos[i];
                else
                    cellInfo.Value = "";
                row.Cells.Add(cellInfo);
                dgvAvailedNames.Rows.Add(row);
            }
        }


        public void GetSelectNameInfos(out string[] names,out string[] infos) 
        {
            List<string> retNames = new List<string>();
            List<string> retInfos = new List<string>();
            
            if(dgvAvailedNames.SelectedRows != null )
                for (int i = dgvAvailedNames.SelectedRows.Count-1; i >-1; i--)
                {
                    DataGridViewRow row = dgvAvailedNames.SelectedRows[i];
                    retNames.Add(row.Cells[0].Value as string);
                    retInfos.Add(row.Cells[1].Value as string);
                }
            
            names = retNames.ToArray();
            infos = retInfos.ToArray();
        }

        

    }
}
