using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFHub;
using JFInterfaceDef;
using JFUI;

namespace JFApp
{
    /// <summary>
    /// 视觉调试窗口
    /// </summary>
    public partial class FormVision : Form
    {
        public FormVision()
        {
            InitializeComponent();
        }

        private void FormVision_Load(object sender, EventArgs e)
        {
            AdjustSingleVisionAssistView();
        }

        private void FormVision_VisibleChanged(object sender, EventArgs e)
        {

        }

        //string _currSingleVisionAssistName = null; //当前所选的单目示教助手名称
        void AdjustSingleVisionAssistView()
        {
            string[] allSVANames = JFHubCenter.Instance.InitorManager.GetIDs(typeof(JFSingleVisionAssist));
            if (null != allSVANames)
                foreach (string nm in allSVANames)
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem(nm);
                    mi.Click += OnSelectSingleVisionAssist;
                    toolStripMenuItemSelectSingleVisionAssist.DropDownItems.Add(mi);
                }


        }


        
        /// <summary>
        /// 选择一个单视觉示教助手
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSelectSingleVisionAssist(object sender,EventArgs e)
        {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            JFSingleVisionAssist sva = JFHubCenter.Instance.VisionMgr.GetSVAssistByName(mi.Text);
            JFRealtimeUI newUc = sva.GetRealtimeUI();
            //if (panelSingleVA.Controls.Contains(newUc))
            //    return;
            if (tpSingleVisionTeach.Controls.Contains(newUc))
                return;
            foreach (Control ctrl in tpSingleVisionTeach.Controls)
            {
                if (ctrl is JFRealtimeUI)
                {
                    tpSingleVisionTeach.Controls.Remove(ctrl);
                    ctrl.Visible = false;
                    break;
                }
            }
            //tpSingleVisionTeach.Controls.Clear();
            //newUc.Dock = DockStyle.Fill;
            newUc.Top = menuStrip1.Bottom;
            tpSingleVisionTeach.Controls.Add(newUc);
            //panelSingleVA.Controls.Add(newUc);
            //newUc.Dock = DockStyle.Fill;
            newUc.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            newUc.Visible = true;

        }

        /// <summary>
        /// 添加一个新的单相机示教助手
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAddSingleVisionAssist_Click(object sender, EventArgs e)
        {
            FormCreateInitor fm = new FormCreateInitor();
            fm.MatchType = typeof(JFSingleVisionAssist);
            fm.ExistIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFInitializable));
            if(fm.ShowDialog() == DialogResult.OK)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(fm.ID);
                mi.Click += OnSelectSingleVisionAssist;
                toolStripMenuItemSelectSingleVisionAssist.DropDownItems.Add(mi);
                JFHubCenter.Instance.VisionMgr.AddSVAssist(fm.ID, fm.Initor as JFSingleVisionAssist);

            }

        }

        /// <summary>
        /// 删除一个已有的相机示教助手
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDelSingleVisionAssist_Click(object sender, EventArgs e)
        {
            string[] allSVANames = JFHubCenter.Instance.VisionMgr.AllSVAssistNames();
            if(null == allSVANames || 0 == allSVANames.Length)
            {
                MessageBox.Show("系统中不存在单视野相机示教助手");
                return;
            }
            FormSelectStrings fm = new FormSelectStrings();
            fm.Text = "请选择需要删除的助手名称";
            fm.SetSelectStrings(allSVANames);
            if(DialogResult.OK == fm.ShowDialog())
            {
                string[] dels = fm.GetSelectedStrings();
                if(MessageBox.Show("确定删除以下示教助手：\n" + string.Join("\n", dels),"警告",MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    for (int i = 0; i < dels.Length; i++)
                    {
                        JFHubCenter.Instance.VisionMgr.DelSVAssist(dels[i]);
                        foreach(ToolStripMenuItem mi in toolStripMenuItemSelectSingleVisionAssist.DropDownItems)
                        {
                            if(mi.Text == dels[i])
                            {
                                toolStripMenuItemSelectSingleVisionAssist.DropDownItems.Remove(mi);
                                break;
                            }
                        }
                        //toolStripMenuItemSelectSingleVisionAssist.DropDownItems.RemoveByKey(dels[i]);
                    }
                }
            }

        }

        
        private void tabControlCF1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(0 == tabControlCF1.SelectedIndex) //单相机示教助手界面
            {

            }
        }
    }
}
