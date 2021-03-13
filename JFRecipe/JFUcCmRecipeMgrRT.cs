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

namespace JFRecipe
{
    /// <summary>
    /// JFCommonRecipeManager的实时界面
    /// </summary>
    public partial class JFUcCmRecipeMgrRT : JFRealtimeUI//UserControl
    {
        public JFUcCmRecipeMgrRT()
        {
            InitializeComponent();
        }

        UcCommonRecipeEdit _recipeEdit = new UcCommonRecipeEdit();
        private void JFUcCmRecipeMgrRT_Load(object sender, EventArgs e)
        {
            _recipeEdit.Dock = DockStyle.Fill;
            panel1.Controls.Add(_recipeEdit);
            _recipeEdit.Show();
            AdjustMgr2View();

        }

        JFCommonRecipeManager _mgr = null;
        bool _isEditting = false; //当前是否正在编辑
        public void SetManager(JFCommonRecipeManager mgr)
        {
            _mgr = mgr;
            if (Created)
                AdjustMgr2View();
        }


        void AdjustMgr2View()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustMgr2View));
                return;
            }

            toolStripCbCategoty.Items.Clear();
            toolStripCbRecipeIDs.Items.Clear();

            if(_mgr == null || !_mgr.IsInitOK)
            {
                menuStrip1.Enabled = false;
                return;
            }

            menuStrip1.Enabled = true;
            string[] categoties = _mgr.AllCategoties();
            if (null != categoties && categoties.Length > 0)
                foreach (string s in categoties)
                    toolStripCbCategoty.Items.Add(s);
        }



        /// <summary>
        /// 新增一个Recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAddRecipe_Click(object sender, EventArgs e)
        {
            FormGenCommonRecipe fm = new FormGenCommonRecipe();
            fm.SetRecipeManager(_mgr);
            if (DialogResult.OK == fm.ShowDialog())
                AdjustMgr2View();
        }

        /// <summary>
        /// 删除当前所选Recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDelRecipe_Click(object sender, EventArgs e)
        {
            if(_mgr == null )
            {
                MessageBox.Show("无效操作:RecipeManager未设置");
                return;
            }

            if(!_mgr.IsInitOK)
            {
                MessageBox.Show("无效操作:RecipeManager未初始化，Error:" + _mgr.GetInitErrorInfo());
                return;
            }


            if(toolStripCbRecipeIDs.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择一个需要删除的RecipeID");
                return;
            }
            string categoty = toolStripCbCategoty.Text;
            string recipeID = toolStripCbRecipeIDs.Text;
            if (DialogResult.OK != MessageBox.Show("确定要删除Categoty: " + categoty + " RecipeID: " + recipeID + "?","删除警告",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning))
                return;


            IJFRecipe deled = _mgr.RemoveRecipe(categoty, recipeID);
            if(deled != null)
            {
                _mgr.Save();
                MessageBox.Show("Categoty:" + categoty + " RecipeID:" + recipeID + " 已删除");
                toolStripCbRecipeIDs.SelectedIndex = -1;
                return;
            }



        }

        /// <summary>
        /// 删除一个Categoty（下属所有的Recipe）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDelCategoty_Click(object sender, EventArgs e)
        {
            //if (_mgr == null)
            //{
            //    MessageBox.Show("无效操作:RecipeManager未设置");
            //    return;
            //}

            //if (!_mgr.IsInitOK)
            //{
            //    MessageBox.Show("无效操作:RecipeManager未初始化，Error:" + _mgr.GetInitErrorInfo());
            //    return;
            //}

            if(toolStripCbCategoty.SelectedIndex < 0)
            {
                MessageBox.Show("请选择需要删除的Categoty");
                return;
            }
            string delCate = toolStripCbCategoty.Text;
            if (DialogResult.OK == MessageBox.Show("确定删除Categoty:" + delCate + "\n及所属的所有Recipe？","删除警告",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning))
            {
                _mgr.RemoveCategoty(toolStripCbCategoty.Text);
                toolStripCbCategoty.SelectedIndex = -1;
                toolStripCbRecipeIDs.SelectedIndex = -1;
                _mgr.Save();
                AdjustMgr2View();
                MessageBox.Show("Categoty:" + delCate + " 已删除！");
            }
        }

        /// <summary>
        /// Categoty选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripCbCategoty_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (_mgr == null)
            //{
            //    MessageBox.Show("无效操作:RecipeManager未设置");
            //    return;
            //}
            //if (!_mgr.IsInitOK)
            //{
            //    MessageBox.Show("无效操作:RecipeManager未初始化，Error:" + _mgr.GetInitErrorInfo());
            //    return;
            //}


            toolStripCbRecipeIDs.Items.Clear();
            if (toolStripCbCategoty.SelectedIndex < 0)
            {
                toolStripCbRecipeIDs.SelectedIndex = -1;
                return;
            }

            string[] recipeIds = _mgr.AllRecipeIDsInCategoty(toolStripCbCategoty.Text);
            if(null == recipeIds || 0 == recipeIds.Length)
            {
                toolStripCbRecipeIDs.SelectedIndex = -1;
                return;
            }

            foreach (string s in recipeIds)
                toolStripCbRecipeIDs.Items.Add(s);


            toolStripCbRecipeIDs.SelectedIndex = -1;
            return;

        }

        /// <summary>
        /// RecipeID选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripCbRecipeIDs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(toolStripCbRecipeIDs.SelectedIndex == -1)
            {
                toolStripMenuItemEditSave.Enabled = false;
                _recipeEdit.SetRecipe(null);
                return;
            }
            toolStripMenuItemEditSave.Enabled = true;
            JFCommonRecipe recipe = _mgr.GetRecipe(toolStripCbCategoty.Text, toolStripCbRecipeIDs.Text) as JFCommonRecipe;
            _recipeEdit.SetRecipe(recipe);
            return;


        }


        /// <summary>
        /// recipe Item 编辑/保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemEditSave_Click(object sender, EventArgs e)
        {
            if(!_isEditting)
            {
                if(toolStripCbRecipeIDs.SelectedIndex < 0)
                {
                    MessageBox.Show("请选择需要编辑的Recipe");
                    return;
                }

                _recipeEdit.EditRecipe = true;
                _isEditting = true;
                toolStripMenuItemEditSave.Text = "保存";
                toolStripMenuItemRecipeMgr.Enabled = false;
                toolStripCbCategoty.Enabled = false;
                toolStripCbRecipeIDs.Enabled = false;
                toolStripMenuItemEditCancel.Enabled = true;
                return;
            }
            else
            {
                _recipeEdit.EditRecipe = false;
                _isEditting = false;
                toolStripMenuItemEditSave.Text = "编辑";
                toolStripMenuItemRecipeMgr.Enabled = true;
                toolStripCbCategoty.Enabled = true;
                toolStripCbRecipeIDs.Enabled = true;
                toolStripMenuItemEditCancel.Enabled = false;
                _mgr.Save();
            }
        }


        /// <summary>
        /// 取消 Recipe item编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemEditCancel_Click(object sender, EventArgs e)
        {
            _isEditting = false;
            _mgr.Load();
            _recipeEdit.SetRecipe(_mgr.GetRecipe(toolStripCbCategoty.Text, toolStripCbRecipeIDs.Text) as JFCommonRecipe);
            //_recipeEdit.AdjustRecipe2View();
            toolStripMenuItemEditSave.Text = "编辑";
            toolStripMenuItemRecipeMgr.Enabled = true;
            toolStripCbCategoty.Enabled = true;
            toolStripCbRecipeIDs.Enabled = true;
            toolStripMenuItemEditCancel.Enabled = false;
        }
    }
}
