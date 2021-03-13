using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLAF
{
    public partial class CalibSearchRange : Form
    {
        public CalibSearchRange()
        {
            InitializeComponent();
        }
        public double Range { get; private set; }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Range = Convert.ToDouble(nudRange.Value);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
