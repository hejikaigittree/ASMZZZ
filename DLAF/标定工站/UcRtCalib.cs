using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;

namespace DLAF
{
    public partial class UcRtCalib : JFRealtimeUI//UserControl
    {
        public UcRtCalib()
        {
            InitializeComponent();
        }

        public FormUV2XY _fm = new FormUV2XY();

        private void UcRtCalib_Load(object sender, EventArgs e)
        {
            _fm.TopMost = false;
            _fm.TopLevel = false;
            Controls.Add(_fm);
            _fm.Dock = DockStyle.Fill;
            _fm.Show();
        }

        public override void UpdateSrc2UI()
        {
            base.UpdateSrc2UI();
            //_fm.SetupUI();
        }

        public void SetStation(CalibStation station)
        {
            _fm.SetStation(station);
        }

        private void UcRtCalib_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                _fm.SetupUI();
        }
    }
}
