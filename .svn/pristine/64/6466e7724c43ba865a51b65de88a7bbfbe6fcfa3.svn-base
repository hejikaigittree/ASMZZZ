using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;

namespace JFUI
{
    public partial class FormJFRealTimeUI : Form
    {
        public FormJFRealTimeUI()
        {
            InitializeComponent();
        }

        private void FormJFRealTimeUI_Load(object sender, EventArgs e)
        {
           
        }

        JFRealtimeUI _ui = null;

        public void SetRTUI(JFRealtimeUI ui)
        {
            if (_ui == ui)
                return;
            timerUpdateUI.Enabled = false;
            Controls.Clear();
            _ui = ui;
            if (null != _ui)
            {
                Size = ui.Size;
                Controls.Add(ui);
                ui.Dock = DockStyle.Fill;
                //timerUpdateUI.Enabled = true;
            }

        }

        /// <summary>
        /// 更新模块状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerUpdateUI_Tick(object sender, EventArgs e)
        {
            if (_ui != null)
                _ui.UpdateSrc2UI();
        }

        private void FormJFRealTimeUI_VisibleChanged(object sender, EventArgs e)
        {
            if (null != _ui)
                timerUpdateUI.Enabled = Visible;
        }
    }
}
