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
    public partial class FormStationBaseAioPanel : Form
    {
        public FormStationBaseAioPanel()
        {
            InitializeComponent();
        }

        private void FormStationBaseAioPanel_Load(object sender, EventArgs e)
        {

        }

        public void SetStation(JFStationBase station)
        {

        }

        public  void FormStationBaseAioPanel_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {

            }
            else
            {

            }
        }

        private void FormStationBaseAioPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MdiParent != null && CloseReason.MdiFormClosing != e.CloseReason)//在MDI中不关闭窗口
            {
                Hide();
                e.Cancel = true;
            }
        }


    }
}
