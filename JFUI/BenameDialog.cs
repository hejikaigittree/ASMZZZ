using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFUI
{
    public partial class BenameDialog : Form
    {
        public BenameDialog()
        {
            InitializeComponent();
        }

        

        private void BenameDialog_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 设置名称初始值
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            tbNameTxt.Text = name;
        }

        /// <summary>
        /// 获取新名称
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return tbNameTxt.Text;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbNameTxt.Text))
            {
                MessageBox.Show("名称不能为空字符串！");
                return; 
            }
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            //DialogResult = DialogResult.Cancel;
        }
    }
}
