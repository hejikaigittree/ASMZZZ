using JFHub;
using JFUI;
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
    public partial class FormMethodFlowTest : Form
    {
        public FormMethodFlowTest()
        {
            InitializeComponent();
        }

        private void FormMethodFlowTest_Load(object sender, EventArgs e)
        {
            ToolStripMenuItem_Save.Enabled = false;
            ToolStripMenuItem_SaveAs.Enabled = false;
        }

        public void SetMethodFlow(JFMethodFlow methodFlow)
        {
            this.methodFlow = methodFlow;
            ucMethodFlow.SetMethodFlow(methodFlow);
            if (null != methodFlow)
                ToolStripMenuItem_SaveAs.Enabled = true;
        }

        string currFilePath = null;//当前MethodFlow对象的载入路径
        JFMethodFlow methodFlow = null;


        /// <summary>新建动作流</summary>
        private void ToolStripMenuItem_Create_Click(object sender, EventArgs e)
        {
            if(null != methodFlow)
            {
                if(null != currFilePath) //当前对象是从文件中载入的
                {
                    if (DialogResult.Yes == MessageBox.Show("建立新流程之前，是否保存当前动作流程？", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        methodFlow.Save(currFilePath);
                }
                else //当前对象还没有保存到文件中
                {
                    if (DialogResult.Yes == MessageBox.Show("是否将当前动作流程保存到文件中？", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "JF流程文件(*.jff)|*.jff";//设置文件类型
                        sfd.FileName = "保存";//设置默认文件名
                        sfd.DefaultExt = "jff";//设置默认格式（可以不设）
                        sfd.AddExtension = true;//设置自动在文件名中添加扩展名
                        if (sfd.ShowDialog() == DialogResult.OK)
                            methodFlow.Save(sfd.FileName);
                    }

                }
            }
            BenameDialog bnd = new BenameDialog();
            bnd.SetName("未命名方法流");
            if (DialogResult.OK != bnd.ShowDialog())
                return;
            currFilePath = null;
            methodFlow = new JFMethodFlow();
            methodFlow.Name = bnd.GetName();
            ucMethodFlow.SetMethodFlow(methodFlow);
            tstbFilePath.Text = "";
            ToolStripMenuItem_Save.Enabled = false;
            ToolStripMenuItem_SaveAs.Enabled = true;
        }

        /// <summary>
        /// 从文件中加载动作流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_LoadFromFile_Click(object sender, EventArgs e)
        {
            if (null != methodFlow)//保存现有对象
            {
                if (null != currFilePath) //当前对象是从文件中载入的
                {
                    if (DialogResult.Yes == MessageBox.Show("载入新流程之前，是否保存当前动作流程？", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        methodFlow.Save(currFilePath);
                }
                else //当前对象还没有保存到文件中
                {
                    if (DialogResult.Yes == MessageBox.Show("是否将当前动作流程保存到文件中？", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "JF流程文件(*.jff)|*.jff";//设置文件类型
                        sfd.FileName = "保存";//设置默认文件名
                        sfd.DefaultExt = "jff";//设置默认格式（可以不设）
                        sfd.AddExtension = true;//设置自动在文件名中添加扩展名
                        if (sfd.ShowDialog() == DialogResult.OK)
                            methodFlow.Save(sfd.FileName);
                    }

                }
            }

            OpenFileDialog ofd = new OpenFileDialog();;
            ofd.Filter = "JF流程文件(*.jff) | *.jff";
            ofd.InitialDirectory = Application.StartupPath;
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                currFilePath = ofd.FileName;
                methodFlow = new JFMethodFlow();
                methodFlow.Load(currFilePath);
                tstbFilePath.Text = currFilePath;
                ucMethodFlow.SetMethodFlow(methodFlow);
                ToolStripMenuItem_Save.Enabled = true;
                ToolStripMenuItem_SaveAs.Enabled = true;
            }

        }

        /// <summary>
        /// 保存当前动作流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_Save_Click(object sender, EventArgs e)
        {
            if(null == methodFlow)
            {
                MessageBox.Show("操作失败！当前流程对象为空");
                return;
            }
            if(currFilePath == null) //保存到文件中
            {
                ToolStripMenuItem_SaveAs_Click(sender, e);
                return;
            }

            methodFlow.Save(currFilePath);
            MessageBox.Show("当前流程已保存成功！");
        }

        /// <summary>
        /// 将当前动作流另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_SaveAs_Click(object sender, EventArgs e)
        {
            if(null == methodFlow)
            {
                MessageBox.Show("操作失败！当前流程对象为空");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JF流程文件(*.jff)|*.jff";//设置文件类型
            sfd.FileName = methodFlow.Name;//设置默认文件名
            sfd.DefaultExt = "jff";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                methodFlow.Save(sfd.FileName);
                if(null == currFilePath)
                {
                    currFilePath = sfd.FileName;
                    tstbFilePath.Text = currFilePath;
                    ToolStripMenuItem_Save.Enabled = true;
                }
            }
        }
    }
}
