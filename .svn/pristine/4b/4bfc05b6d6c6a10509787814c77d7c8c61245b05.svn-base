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
    public partial class FormAddFlow2Station : Form
    {
        public FormAddFlow2Station()
        {
            InitializeComponent();
        }

        /// <summary>
        ///  CheckBox 是否从配置文件中加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkLoaded_CheckedChanged(object sender, EventArgs e)
        {
            btLoadFile.Enabled = chkLoaded.Checked;
        }

        /// <summary>
        /// 选择配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog(); ;
            ofd.Filter = "JF流程文件(*.jff) | *.jff";
            ofd.InitialDirectory = Application.StartupPath;
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = false;
            if (ofd.ShowDialog() == DialogResult.OK)
                tbFilePath.Text = ofd.FileName;
                
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("工作流名称不能为空");
                tbName.Focus();
                return;
            }
            if(chkLoaded.Checked)
            {
                if(string.IsNullOrEmpty(tbFilePath.Text))
                {
                    MessageBox.Show("文件路径不能为空");
                    return;
                }
            }
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            tbName.Text = "";
            chkLoaded.Checked = false;
            tbFilePath.Text = "";
            DialogResult = DialogResult.Cancel;
        }

        public string WorkFlowName { get { return tbName.Text; } }
        public bool IsLoadedFromFile { get { return chkLoaded.Checked; } }

        public string FilePath { get { return tbFilePath.Text; } }
    }
}
