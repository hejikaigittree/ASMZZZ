using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFHub;
using JFInterfaceDef;
using JFUI;

namespace JFApp
{
    /// <summary>
    /// 各种配置项管理
    /// </summary>
    public partial class FormCfg : Form
    {
        public FormCfg()
        {
            InitializeComponent();
            AllowedEdit = false;
        }

        //FormDllMgr _dllFm = new FormDllMgr(); //拓展动态库管理界面

        private void FormCfg_Load(object sender, EventArgs e)
        {
            tabCtrl.TabPages.Clear();
            FormStationEnables fse = new FormStationEnables();
            AddPage(fse, "工站使能管理");
            fse.Show();
            AddPage(new FormStationCfgMgr(), "工站配置管理"); // 可能不需要在此添加，每个工站的测试界面都有一个配置界面
            AddPage(new FormDllMgr(), "拓展Dll管理");
            
            FormInitorMgr fmDevMgr = new FormInitorMgr();
            fmDevMgr.InitorType = typeof(IJFDevice);
            AddPage(fmDevMgr,"设备管理"); //设备管理
            
            AddPage(new FormDeviceCellNameManager(), "设备通道命名");

            FormInitorMgr fmStationMgr = new FormInitorMgr();
            fmStationMgr.InitorCaption = "工站";
            fmStationMgr.InitorType = typeof(IJFStation);
            AddPage(fmStationMgr,"工站管理"); //工站管理



            AddPage(new FormRecipeMgr(), "产品配方管理");


            FormXCfgEdit fm = new FormXCfgEdit();
            fm.AllowAddTypes.Add(typeof(Point));
            fm.AllowAddTypes.Add(typeof(JFMotionParam));
            fm.AllowAddTypes.Add(typeof(JFHomeParam));
            fm.AllowAddTypes.Add(typeof(JFLinearCalibData));
            fm.SetCfg(JFHubCenter.Instance.SystemCfg);
            AddPage(fm, "系统配置");

            tabCtrl.SelectedIndex = 0;

        }

        void AddPage(Form frm,string pageName)
        {
            TabPage tabPage = new TabPage(); //创建一个TabControl 中的单个选项卡页。
            if (string.IsNullOrEmpty(pageName))
                pageName = frm.Text;
            tabPage.Text = pageName;
            tabPage.Name = pageName;
            tabPage.BackColor = frm.BackColor;
            tabPage.Font = frm.Font;

            tabCtrl.TabPages.Add(tabPage);   //添加tabPage选项卡到tab控件

            frm.FormBorderStyle = FormBorderStyle.None;
            frm.TopLevel = false;
            frm.Dock = DockStyle.Fill;
            frm.Parent = tabPage;

            tabPage.Controls.Add(frm);  //tabPage选项卡添加一个窗体对象 
            frm.Visible = false;
        }


        public bool AllowedEdit //待添加功能
        {
            get;set;
        }

        private void FormCfg_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {

            }
            else
            {

            }
        }

        private void tabCtrl_Selecting(object sender, TabControlCancelEventArgs e) 
        {
            ///待添加代码
            //当前页正在编辑时，取消变更...
            //e.Cancel = false;
        }

        private void tabCtrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null == tabCtrl.SelectedTab)
                return;

            if (tabCtrl.SelectedTab.HasChildren)
                foreach (Control item in tabCtrl.SelectedTab.Controls)
                    if (item is Form)
                        item.Visible = true;
        }
    }
}
