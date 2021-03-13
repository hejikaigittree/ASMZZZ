using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFHub
{
    /// <summary>
    /// （模态）窗口类，用于选择/创建系统配置文件
    /// </summary>
    public partial class FormSelCfg : Form
    {
        public FormSelCfg()
        {
            InitializeComponent();
            SysCfgFilePath = null;
            IsCreateNewFile = false;
        }

        public string SysCfgFilePath { get; private set; }
        public bool IsCreateNewFile { get; private set; }

        private void FormInitHub_Load(object sender, EventArgs e)
        {
            ControlBox = false;
        }


        OpenFileDialog ofd = null;
        private void btSelectFile_Click(object sender, EventArgs e)//选择已有配置/重新创建文件
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "XML文件(*.xml;*.XML)|*.xml;*.XML";
            ofd.InitialDirectory = Application.StartupPath;
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = false;
            ofd.FileOk += OnFileOK;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                 SysCfgFilePath =  ofd.FileName;
                DialogResult = DialogResult.OK;
                IsCreateNewFile = false;
            }
        }

        void OnFileOK(object sender, CancelEventArgs e)
        {
            if(File.Exists(ofd.FileName)) //配置文件已存在
            {
                e.Cancel = false;
            }
            else //文件不存在，新创建
            {
                if(DialogResult.Yes != MessageBox.Show("确定使用新文件:\"" +ofd.FileName +"\"作为系统配置文件？" ,"警告",MessageBoxButtons.YesNo,MessageBoxIcon.Warning))
                    e.Cancel = true;
            }
            
        }



        private void btCancle_Click(object sender, EventArgs e) //退出程序
        {
            Application.Exit();
        }

        public string Tips { get { return richTextBox1.Text; }set { richTextBox1.Text = value; } }
    }
}
