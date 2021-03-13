using JFToolKits;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;
using System.IO;

namespace JFHub
{
    public partial class FormDllMgr : Form
    {
        public FormDllMgr()
        {
            InitializeComponent();
        }

        private void FormDllMgr_Load(object sender, EventArgs e)
        {
            
            /////配置文件中的dll路径
            List<string> dllPaths = JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_ExpandDllFiles) as List<string>;
            JFinitializerHelper initorHelper = JFHubCenter.Instance.InitorHelp;
            trvDlls.Nodes.Clear();
            foreach(string dllPath in dllPaths)
            {
                
                TreeNode dllNode = new TreeNode(dllPath);
                trvDlls.Nodes.Add(dllNode);
                if (!initorHelper.AllApendDllPaths().Contains(dllPath)) //InitorHelp未加载
                {
                    dllNode.ForeColor = Color.Red;
                    continue;
                }
                Type[] types = initorHelper.InstantiatedClassesInDll(dllPath);
                foreach (Type type in types)
                    dllNode.Nodes.Add(type.Name);

            }
            UpdateInitorsView();
        }





        private void FormDllMgr_VisibleChanged(object sender, EventArgs e)
        {
            //if (Visible)
                //UpdateUI();
            //else
              //  ;//从未被调用
        }

        void ShowTips(string info)
        {
            if (string.IsNullOrEmpty(info))
                lbTips.Text = "";
            else
                lbTips.Text = "Tips:" + info;
        }

        /// <summary>添加一个dll文件路径</summary>
        private void btAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "dll文件(*.dll)|*.dll";
            ofd.InitialDirectory = Application.StartupPath;
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //string fn = ofd.FileName;
                if(JFHubCenter.Instance.InitorHelp.AllApendDllPaths().Contains(ofd.FileName))
                {
                    MessageBox.Show("\" " + ofd.FileName + "\"已经被导入程序，不能重复添加");
                    return;
                }
                try
                {
                    JFHubCenter.Instance.InitorHelp.AppendDll(ofd.FileName);

                    TreeNode dllNode = new TreeNode(ofd.FileName);
                    trvDlls.Nodes.Add(dllNode);
                    Type[] types = JFHubCenter.Instance.InitorHelp.InstantiatedClassesInDll(ofd.FileName);
                    foreach (Type type in types)
                        dllNode.Nodes.Add(type.Name);

                    (JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_ExpandDllFiles) as List<string>).Add(ofd.FileName);
                    JFHubCenter.Instance.SystemCfg.Save();
                    lbTips.Text = "导入dll文件\" " + ofd.FileName + "\" 成功！";
                    UpdateInitorsView();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("导入dll文件\" " + ofd.FileName + "\"发生异常:" + ex);
                    return;
                }
            }

        }


        /// <summary>
        /// 删除一个Dll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDel_Click(object sender, EventArgs e)
        {
            TreeNode node = trvDlls.SelectedNode;
            if(null == node)
            {
                MessageBox.Show("请先选择一个需要移除的dll文件");
                return;
            }
            if(!trvDlls.Nodes.Contains(node))
            {
                MessageBox.Show("请选择一个需要移除的dll文件(一级子节点)");
                return;
            }
            if (DialogResult.Cancel == MessageBox.Show("确定从系统中移除附加的Dll文件\"" + node.Text + "\"吗？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                return;
            try
            {
                JFHubCenter.Instance.InitorHelp.RemoveDll(node.Text);
                (JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_ExpandDllFiles) as List<string>).Remove(node.Text);
                JFHubCenter.Instance.SystemCfg.Save();
                trvDlls.Nodes.Remove(node);
                lbTips.Text = "Dll\"" + node.Text + "\"已从系统中移除";
                UpdateInitorsView();
            }
            catch(Exception ex)
            {
                MessageBox.Show("移除Dll发生异常:" + ex);
            }

            


        }

        private void trvDlls_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //MessageBox.Show(e.Node.Text);
            UpdateInitorsView();
        }

        /// <summary>
        /// 更新Initors信息列表
        /// </summary>
        void UpdateInitorsView()
        {

            dgvInitor.Rows.Clear();
            foreach(TreeNode dllNode in trvDlls.Nodes)
                if(dllNode.Nodes.Count >0)
                    foreach(TreeNode initorNode in dllNode.Nodes)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                        cellName.Value = initorNode.Text;
                        DataGridViewTextBoxCell cellVersion = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell cellBrief = new DataGridViewTextBoxCell();
                        cellVersion.Value = "未知";
                        //获取版本信息
                        Type[] types = JFHubCenter.Instance.InitorHelp.InstantiatedClassesInDll(dllNode.Text);
                        foreach(Type type in types)
                            if(type.Name == initorNode.Text)
                            {
                                JFVersionAttribute[] va = type.GetCustomAttributes(typeof(JFVersionAttribute), false) as JFVersionAttribute[];
                                if (null != va && va.Length > 0)
                                    cellVersion.Value = va[0].Info;
                                JFDisplayNameAttribute[] vn = type.GetCustomAttributes(typeof(JFDisplayNameAttribute), false) as JFDisplayNameAttribute[];
                                if (null != vn && vn.Length > 0)
                                    cellBrief.Value = vn[0].Name;

                            }
                        row.Cells.Add(cellName);
                        row.Cells.Add(cellBrief);
                        row.Cells.Add(cellVersion);
                        dgvInitor.Rows.Add(row);

                    }

            dgvInitor.ClearSelection();
            if (trvDlls.SelectedNode == null)
                return;
            
            ////根据tree控件中所选节点 ， 选中DatagridView控件内容
            if(trvDlls.Nodes.Contains(trvDlls.SelectedNode)) //选中的是dll文件
            {
                if(!File.Exists(trvDlls.SelectedNode.Text))
                {
                    if(DialogResult.OK == MessageBox.Show("文件路径不存在:" +trvDlls.SelectedNode.Text + "\n是否从系统中删除？","异常信息",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning))
                    {
                        JFHubCenter.Instance.InitorHelp.RemoveDll(trvDlls.SelectedNode.Text);
                        (JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_ExpandDllFiles) as List<string>).Remove(trvDlls.SelectedNode.Text);
                        JFHubCenter.Instance.SystemCfg.Save();
                        trvDlls.Nodes.Remove(trvDlls.SelectedNode);
                        //lbTips.Text = "Dll\"" + node.Text + "\"已从系统中移除";
                        //UpdateInitorsView();
                        return;
                    }

                    return;
                }
                Type[] types = JFHubCenter.Instance.InitorHelp.InstantiatedClassesInDll(trvDlls.SelectedNode.Text);
                if (null == types || types.Length == 0)
                    return;
                List<string> initTypeNames = new List<string>();
                foreach (Type t in types)
                    initTypeNames.Add(t.Name);
                foreach (DataGridViewRow row in dgvInitor.Rows)
                    if (initTypeNames.Contains(row.Cells[0].Value as string))
                        row.Selected = true;
            }
            else //选中的是Initor类型
            {
                foreach(DataGridViewRow row in dgvInitor.Rows)
                    if((row.Cells[0].Value as string)== trvDlls.SelectedNode.Text)
                        row.Selected = true;
                        
            }
        }
    }
}
