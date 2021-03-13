using JFInterfaceDef;
using JFToolKits;
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
    public partial class FormSysCfg : Form
    {
        public FormSysCfg()
        {
            InitializeComponent();
            //dllForm = new FormDllMgr();
        }

        /// 
        private void FormSysCfg_Load(object sender, EventArgs e)
        {
            tabControlCF1.TabPages.Clear();
            FormDllMgr fmDll = new FormDllMgr();
            AddPage(fmDll); //Dll文件管理
            
            FormInitorMgr fmInitorMgr = new FormInitorMgr();
            fmInitorMgr.InitorCaption = "设备";
            fmInitorMgr.InitorType = typeof(IJFDevice);
            AddPage(fmInitorMgr); //设备管理

            FormInitorMgr fmStationMgr = new FormInitorMgr();
            fmStationMgr.InitorCaption = "工站";
            fmStationMgr.InitorType = typeof(IJFStation);
            AddPage(fmStationMgr); //工站管理



            //tabControlCF1.SelectedIndex = 0;
            fmDll.Visible = true;
        }



        //FormDllMgr dllForm;

        private void AddPage(Form frm,string pageName = null)
        {
            TabPage tabPage = new TabPage(); //创建一个TabControl 中的单个选项卡页。
            if (string.IsNullOrEmpty(pageName))
                pageName = frm.Text;
            tabPage.Text = pageName;
            tabPage.Name = pageName;
            tabPage.BackColor = frm.BackColor;
            tabPage.Font = frm.Font;

            tabControlCF1.TabPages.Add(tabPage);   //添加tabPage选项卡到tab控件

            frm.FormBorderStyle = FormBorderStyle.None;
            frm.TopLevel = false;
            frm.Dock = DockStyle.Fill;
            frm.Parent = tabPage;

            tabPage.Controls.Add(frm);  //tabPage选项卡添加一个窗体对象 
            frm.Visible = false;

        }

        private void tabControlCF1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null == tabControlCF1.SelectedTab)
                return;

            if (tabControlCF1.SelectedTab.HasChildren)
                foreach (Control item in tabControlCF1.SelectedTab.Controls)
                    if (item is Form)
                        item.Visible = true;
        }
    }
}
