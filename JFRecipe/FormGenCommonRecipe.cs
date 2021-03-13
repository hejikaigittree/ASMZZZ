using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using JFInterfaceDef;
using JFToolKits;

namespace JFRecipe
{
    /// <summary>
    /// 用于创建一个通用Recipe的窗口
    /// </summary>
    public partial class FormGenCommonRecipe : Form
    {
        public FormGenCommonRecipe()
        {
            InitializeComponent();
        }


        private void FormGenCommonRecipe_Load(object sender, EventArgs e)
        {
            AdjustMgr2View();
        }


        /// <summary>
        /// 当前系统中正在使用的RecipeManager
        /// </summary>
        IJFRecipeManager _mgr = null;
        public void SetRecipeManager(IJFRecipeManager mgr)
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

            if(null == _mgr)
            {
                cbGenCate.Enabled = false;
                tbGenID.Enabled = false;
                chkCopy.Enabled = false;
                cbCopyCate.Enabled = false;
                cbCopyID.Enabled = false;
                labelTips.Text = "RecipeManager未设置！";
                btOK.Enabled = false;
                return;
            }

            if(!_mgr.IsInitOK)
            {
                cbGenCate.Enabled = false;
                tbGenID.Enabled = false;
                chkCopy.Enabled = false;
                cbCopyCate.Enabled = false;
                cbCopyID.Enabled = false;
                labelTips.Text = "RecipeManager未初始化！";
                btOK.Enabled = false;
                return;
            }

            cbGenCate.Enabled = true;
            tbGenID.Enabled = true;
            chkCopy.Enabled = true;
            chkCopy.Checked = false;
            cbCopyCate.Enabled = false;
            cbCopyID.Enabled = false;
            labelTips.Text = "";
            btOK.Enabled = true;

            cbGenCate.Items.Clear();
            cbCopyCate.Items.Clear();
            string[] categotys = _mgr.AllCategoties();
            if(null != categotys && categotys.Length > 0)
                foreach (string cate in categotys)
                {
                    cbGenCate.Items.Add(cate);
                    cbCopyCate.Items.Add(cate);
                }
            else //当前不存在产品类别
                chkCopy.Enabled = false;
            



        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if(null == _mgr )
            {
                MessageBox.Show("无效操作，RecipeManager未设置");
                return;
            }

            if(!_mgr.IsInitOK)
            {
                MessageBox.Show("无效操作，RecipeManager未初始化，ErrorInfo:" + _mgr.GetInitErrorInfo());
                return;
            }

            if(string.IsNullOrWhiteSpace(cbGenCate.Text))
            {
                MessageBox.Show("参数项 Categoty 不能为空值！");
                return;
            }

            if(string.IsNullOrWhiteSpace(tbGenID.Text))
            {
                MessageBox.Show("参数项 RecipeID 不能为空值！");
                return;
            }

            string[] existIDs = _mgr.AllRecipeIDsInCategoty(cbGenCate.Text);
            if(null != existIDs && existIDs.Contains(tbGenID.Text))
            {
                MessageBox.Show("添加Recipe失败，Categoty = " + cbGenCate.Text + "已包含当前RecipeID = " + tbGenID.Text);
                return;
            }

            JFCommonRecipe newRecipe = new JFCommonRecipe();
            newRecipe.Categoty = cbGenCate.Text;
            newRecipe.ID = tbGenID.Text;
            if (chkCopy.Checked) //以拷贝的方式创建新的Recipe
            {
                if(string.IsNullOrEmpty(cbCopyCate.Text))
                {
                    MessageBox.Show("请选择待拷贝的Categoty!");
                    return;
                }

                if (string.IsNullOrEmpty(cbCopyID.Text))
                {
                    MessageBox.Show("请选择待拷贝的RecipeID!");
                    return;
                }

                JFCommonRecipe recipe = _mgr.GetRecipe(cbCopyCate.Text, cbCopyID.Text) as JFCommonRecipe;
                string xmlTxt;
                string typeTxt;
                JFFunctions.ToXTString(recipe.Dict, out xmlTxt, out typeTxt);
                newRecipe.Dict = JFFunctions.FromXTString(xmlTxt, typeTxt) as JFXmlDictionary<string, object>;

            }


            _mgr.AddRecipe(cbGenCate.Text, tbGenID.Text, newRecipe);
            _mgr.Save();
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


        //bool _isCheckedSetting = false;
        /// <summary>
        /// 事都选择以Copy的方式创建新Recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkCopy_CheckedChanged(object sender, EventArgs e)
        {
            //if (_isCheckedSetting)
            //    return;
            if (null == _mgr || !_mgr.IsInitOK)
                return;
            if(!chkCopy.Checked)
            {
                cbCopyCate.Enabled = false;
                cbCopyID.Enabled = false;
                return;
            }
            string[] existCategoties = _mgr.AllCategoties();
            if (null == existCategoties || existCategoties.Length == 0)
            {
                chkCopy.Checked = false;
                MessageBox.Show("不能进行拷贝操作，系统中不存在Recipe");
                cbCopyCate.Enabled = false;
                cbCopyID.Enabled = false;
                return;
            }

            cbCopyCate.Enabled = true;
            cbCopyID.Enabled = true;
            cbCopyCate.Items.Clear();
            cbCopyID.Items.Clear();

            foreach(string s in existCategoties)
                cbCopyCate.Items.Add(s);
        }

        /// <summary>
        /// 拷贝Categoty选择改变，将已选Categoty下的CommonRecipe添加到cbCopyID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCopyCate_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbCopyID.Items.Clear();
            string[] ids = _mgr.AllRecipeIDsInCategoty(cbCopyCate.Text);
            if (null == ids || 0 == ids.Length)
            {
                cbCopyID.Enabled = false;
                return;
            }
            List<string> commonIDs = new List<string>();
            foreach (string id in ids)
                if (_mgr.GetRecipe(cbCopyCate.Text, id) is JFCommonRecipe)
                    commonIDs.Add(id);
            if(ids.Count() == 0)
            {
                cbCopyID.Enabled = false;
                return;
            }

            cbCopyID.Enabled = true;
            cbCopyID.Items.AddRange(commonIDs.ToArray());
        }
    }
}
