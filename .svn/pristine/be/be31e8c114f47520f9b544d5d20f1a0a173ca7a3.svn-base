using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFUI
{
    /// <summary>
    /// FormSelectStrings 提供一个string多选对话框
    /// </summary>
    public partial class FormSelectStrings : Form
    {
        public FormSelectStrings()
        {
            InitializeComponent();
        }

        bool _isFormLoaded = false;
        private void FormSelectStrings_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustView();

        }

        string[] _strings = null;


        void AdjustView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustView));
                return;
            }
            dataGridView1.Rows.Clear();
            if (_strings == null)
                return;
            foreach(string txt in _strings)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = txt;
                row.Cells.Add(cell);
                dataGridView1.Rows.Add(row);
            }
        }


        public void SetSelectStrings(string[] strings)
        {
            _strings = strings;
            if (_isFormLoaded)
                AdjustView();
        }

        public string[] GetSelectedStrings()
        {

            if (0 == dataGridView1.SelectedRows.Count)
                return null;
            List<string> ret = new List<string>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                ret.Add(row.Cells[0].Value as string);
            
            return ret.ToArray();
        }

        

        private void btOK_Click(object sender, EventArgs e)
        {
            if (null == GetSelectedStrings())
            {
                MessageBox.Show("未选择数据项！");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.Cancel;
        }
    }
}
