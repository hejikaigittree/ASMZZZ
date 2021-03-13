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
    public partial class FormShowLctBuff : Form
    {
        public FormShowLctBuff()
        {
            InitializeComponent();
        }

        private void FormShowLctBuff_Load(object sender, EventArgs e)
        {

        }

        double[] _data = null;

        bool _IsEqual(double[] data)
        {
            if (_data == null || 0 == _data.Length)
            {
                if (data == null || 0 == data.Length)
                    return true;
                else
                    return false;
            }

            if (_data.Length != data.Length)
                return false;
            for (int i = 0; i < _data.Length; i++)
            {
                if (_data[i] != data[i])
                    return false;
            }
            return true;
        }

        delegate void dgSetData(double[] data);
        public void SetData(double[] data)
        {
            if(InvokeRequired)
            {
                Invoke(new dgSetData(SetData), new object[] { data });
                return;
            }
            if (_IsEqual(data))
                return;
            _data = data;
            dataGridView1.Rows.Clear();
            if (null == _data || 0 == _data.Length)
                return;
            for(int i = 0; i < _data.Length;i++)
            {
                DataGridViewRow row= new DataGridViewRow();
                DataGridViewTextBoxCell cellIndex = new DataGridViewTextBoxCell();
                cellIndex.Value = i;
                row.Cells.Add(cellIndex);
                DataGridViewTextBoxCell cellPos = new DataGridViewTextBoxCell();
                cellPos.Value = _data[i];
                row.Cells.Add(cellPos);
                dataGridView1.Rows.Add(row);
            }
        }

        private void FormShowLctBuff_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }
    }
}
