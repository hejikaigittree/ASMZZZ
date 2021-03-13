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
    public partial class UcRtAutoMapping : JFRealtimeUI//UserControl
    {
        public UcRtAutoMapping()
        {
            InitializeComponent();
        }

        FrmAutoMapping _fm = new FrmAutoMapping();

        private void UcRtAutoMapping_Load(object sender, EventArgs e)
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

        public void SetStation(AutoMappingStation station)
        {
            _fm.SetStation(station);
        }

        private void UcRtAutoMapping_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                _fm.SetupUI();
        }
    }
}
