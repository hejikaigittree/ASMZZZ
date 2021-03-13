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
    /// 多个DIO模块的测试窗口 ，用于测试同一个MotionDaq中的多个DIO模块
    /// </summary>
    public partial class FormDios : Form
    {
        public FormDios()
        {
            InitializeComponent();
        }

        private void FormDios_Load(object sender, EventArgs e)
        {
            UpdateModleStatus();
        }

        //bool _isFormLoaded = false;
        List<IJFModule_DIO> _lstModules = new List<IJFModule_DIO>();
        //List<string> _lstNames = new List<string>();


        public void ClearModules()
        {
            _lstModules.Clear();
            tabCtrl.TabPages.Clear();
        }
        public void AddModule(IJFModule_DIO module,string moduleName)
        {
            if (null == module)
                return;
            if (_lstModules.Contains(module))
                return;

            if (null == moduleName)
                moduleName = "DIO";
            TabPage tp = new TabPage();
            tabCtrl.TabPages.Add(tp);
            UcDIO uc = new UcDIO();
            uc.Dock = DockStyle.Fill;
            uc.Parent = tp;
            uc.Visible = true;
            uc.SetDioModule(module, null, null);
            tp.Text = moduleName;
            tp.Name = moduleName;
            tp.Controls.Add(uc);
            _lstModules.Add(module);

        }

        public void UpdateModleStatus()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateModleStatus));
                return;
            }
            if (_lstModules.Count == 0)
                return;
            if (tabCtrl.SelectedIndex < 0)
                return;
            UcDIO uc = tabCtrl.TabPages[tabCtrl.SelectedIndex].Controls[0] as UcDIO;
            uc.UpdateSrc2UI();
        }
    }
}
