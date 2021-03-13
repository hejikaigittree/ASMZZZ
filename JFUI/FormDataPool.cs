using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef ;

namespace JFUI
{
    public partial class FormDataPool : Form
    {
        public FormDataPool()
        {
            InitializeComponent();
            HideWhenClose = false;
        }

        private void FormDataPool_Load(object sender, EventArgs e)
        {
           
        }

        public void SetDataPool(IJFDataPool dp)
        {
            ucDataPoolEdit1.SetDataPool(dp);
        }


        /// <summary>
        /// 点击关闭按钮时，只是隐藏界面
        /// </summary>
        public bool HideWhenClose { get; set; }

        private void FormDataPool_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                ucDataPoolEdit1.AutoUpdateDataPool = true;
            }
            else
            {
                ucDataPoolEdit1.AutoUpdateDataPool = false;
            }
        }

        private void FormDataPool_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HideWhenClose)
            {
                Hide();
                e.Cancel = true;
            }
            else
                e.Cancel = false;
        }
    }
}
