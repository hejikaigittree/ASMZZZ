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
    /// <summary>
    /// 用于同一个测试运动控制器中的多个模拟量采集模块
    /// </summary>
    public partial class FormAios : Form
    {
        public FormAios()
        {
            InitializeComponent();
        }

        private void FormAios_Load(object sender, EventArgs e)
        {

        }

        List<IJFModule_AIO> _lstModules = new List<IJFModule_AIO>();

        public void ClearModules()
        {
            _lstModules.Clear();
            tabCtrl.TabPages.Clear();
        }
        public void AddModule(IJFModule_AIO module, string moduleName)
        {
            if (null == module)
                return;
            if (_lstModules.Contains(module))
                return;

            if (null == moduleName)
                moduleName = "AIO";
            TabPage tp = new TabPage();
            tabCtrl.TabPages.Add(tp);
            UcAIO uc = new UcAIO();
            uc.Dock = DockStyle.Fill;
            uc.Parent = tp;
            uc.Visible = true;
            uc.SetModuleInfo(module, null, null);//uc.SetDioModule(module, null, null);
            tp.Text = moduleName;
            tp.Name = moduleName;
            tp.Controls.Add(uc);
            _lstModules.Add(module);
        }

        public void UpdateModuleStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateModuleStatus));
                return;
            }
            if (_lstModules.Count == 0)
                return;
            if (tabCtrl.SelectedIndex < 0)
                return;
            UcAIO uc = tabCtrl.TabPages[tabCtrl.SelectedIndex].Controls[0] as UcAIO;
            uc.UpdateSrc2UI();
        }
    }
}
