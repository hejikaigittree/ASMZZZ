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
    public partial class FormStationBaseLightPanel : Form
    {
        public FormStationBaseLightPanel()
        {
            InitializeComponent();
        }

        public void FormStationBaseLightPanel_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void FormStationBaseLightPanel_Load(object sender, EventArgs e)
        {

        }

        private void FormStationBaseLightPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MdiParent != null && CloseReason.MdiFormClosing != e.CloseReason)//在MDI中不关闭窗口
            {
                Hide();
                e.Cancel = true;
            }
        }

        public void SetStation(JFStationBase station)
        {

        }
    }
}
