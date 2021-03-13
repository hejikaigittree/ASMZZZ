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
    /// UcMainStationPanelBase 供JFMainStation使用的简单界面，只显示收到的消息
    /// </summary>
    public partial class UcMainStationPanelBase : UserControl
    {
        public UcMainStationPanelBase()
        {
            InitializeComponent();
        }

        private void UcMainStationPanelBase_Load(object sender, EventArgs e)
        {

        }

        IJFMainStation _mainStation = null;

        public void SetMainStation(IJFMainStation ms)
        {
            _mainStation = ms;
        }



    }
}
