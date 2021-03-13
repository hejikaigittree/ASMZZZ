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
    /// 用于显示/调试运动控制器中的多个比较触发模块
    /// </summary>
    public partial class FormCmprTrigs : Form
    {
        public FormCmprTrigs()
        {
            InitializeComponent();
        }

        private void FormCmprTrigs_Load(object sender, EventArgs e)
        {
            UpdateModleStatus();
        }

        List<IJFModule_CmprTrigger> _lstModules = new List<IJFModule_CmprTrigger>();

        public void ClearModules()
        {
            _lstModules.Clear();
            tabCtrl.TabPages.Clear();
        }

        public void AddModule(IJFModule_CmprTrigger module, string moduleName)
        {
            if (null == module)
                return;
            if (_lstModules.Contains(module))
                return;

            if (null == moduleName)
                moduleName = "CmprTrig";
            TabPage tp = new TabPage();
            tabCtrl.TabPages.Add(tp);
            UcCmprTrig uc = new UcCmprTrig();
            uc.Dock = DockStyle.Fill;
            uc.Parent = tp;
            uc.Visible = true;
            uc.SetCmprTigger(module, null,null);
            tp.Text = moduleName;
            tp.Name = moduleName;
            tp.Controls.Add(uc);
            _lstModules.Add(module);
        }

        public void UpdateModleStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateModleStatus));
                return;
            }
            if (_lstModules.Count == 0)
                return;
            if (tabCtrl.SelectedIndex < 0)
                return;
            UcCmprTrig uc = tabCtrl.TabPages[tabCtrl.SelectedIndex].Controls[0] as UcCmprTrig;
            uc.UpdateSrc2UI();
        }

    }
}
