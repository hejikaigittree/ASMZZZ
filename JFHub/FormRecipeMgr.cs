using JFInterfaceDef;
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
    public partial class FormRecipeMgr : Form
    {
        public FormRecipeMgr()
        {
            InitializeComponent();
        }


        private void FormRecipeMgr_Load(object sender, EventArgs e)
        {
            AdjustManagerView2UI();
        }

        void AdjustManagerView2UI()
        {
            panel1.Controls.Clear();
            IJFRecipeManager recipeMgr = JFHubCenter.Instance.RecipeManager;
            if (null == recipeMgr)
            {
                toolStripMenuItemReset.Text = "新建配方管理器";
                toolStripTbTips.Text = "系统不存在配方管理器";
                toolStripMenuItemInitMgr.Visible = false;
                toolStripMenuItemInitParam.Visible = false;
                toolStripMenuItemDialog.Visible = false;
                return;
            }
            else
            {
                toolStripMenuItemReset.Text = "重置配方管理器";
                toolStripMenuItemInitMgr.Visible = true;
                toolStripMenuItemInitParam.Visible = true;
                if (!recipeMgr.IsInitOK)
                {
                    toolStripTbTips.Text = "管理器初始化错误:" + recipeMgr.GetInitErrorInfo();
                    return;
                }
                toolStripTbTips.Text = "管理器已初始化！";
                if (recipeMgr is IJFConfigUIProvider)
                    toolStripMenuItemDialog.Visible = true;
                else
                    toolStripMenuItemDialog.Visible = false;

                if(recipeMgr is IJFRealtimeUIProvider)
                {
                    JFRealtimeUI ui = (recipeMgr as IJFRealtimeUIProvider).GetRealtimeUI();
                    ui.Dock = DockStyle.Fill;
                    panel1.Controls.Add(ui);
                    ui.Show();
                    ui.UpdateSrc2UI();
                }


            }
        }



        /// <summary>
        /// 新建/重置配方管理器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemReset_Click(object sender, EventArgs e)
        {
            IJFRecipeManager recipeMgr = JFHubCenter.Instance.RecipeManager;
            FormCreateInitor fm = new FormCreateInitor();
            fm.MatchType = typeof(IJFRecipeManager);
            fm.SetFixedID("JF配方管理器");
            if (null == recipeMgr)
                fm.Text = "新建配方管理器";
            else
                fm.Text = "重建配方管理器";
            if (DialogResult.OK != fm.ShowDialog())
                return;
            if (null != recipeMgr)
                JFHubCenter.Instance.InitorManager.Remove("JF配方管理器");//先删除当前RecipeMgr

            recipeMgr = fm.Initor as IJFRecipeManager;
            JFHubCenter.Instance.InitorManager.Add(fm.ID, recipeMgr);
            AdjustManagerView2UI();
            
        }


        /// <summary>
        /// 配方管理器初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemInitMgr_Click(object sender, EventArgs e)
        {
            IJFRecipeManager recipeMgr = JFHubCenter.Instance.RecipeManager;
            if(null == recipeMgr)
            {
                MessageBox.Show("系统配置中不存在配方管理器");
                return;
            }

            bool isOK = recipeMgr.Initialize();
            if(!isOK)
            {
                MessageBox.Show("配方管理器初始化失败,ErrorInfo:" + recipeMgr.GetInitErrorInfo());
                return;
            }

            AdjustManagerView2UI();


        }

        /// <summary>
        /// 设置配方管理器初始化参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemInitParam_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 显示配方管理器自定义窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDialog_Click(object sender, EventArgs e)
        {
            IJFRecipeManager recipeMgr = JFHubCenter.Instance.RecipeManager;
            if (null == recipeMgr)
            {
                MessageBox.Show("系统配置中不存在配方管理器");
                return;
            }

            if(!(recipeMgr is IJFConfigUIProvider))
            {
                MessageBox.Show("当前配方管理器类型:" + recipeMgr.GetType().Name + " 未提供配置窗口");
                return;
            }
            (recipeMgr as IJFConfigUIProvider).ShowCfgDialog();


        }
    }
}
