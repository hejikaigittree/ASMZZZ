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
using JFUI;

namespace JFHub
{
    public partial class UcNamesEditTest_Aio : UserControl
    {
        public UcNamesEditTest_Aio()
        {
            InitializeComponent();
        }

        private void UcNamesEditTest_Aio_Load(object sender, EventArgs e)
        {

        }
        public void UpdateChannelsInfo(string devID, int moduleIndex)
        {
            //int nBottom = 0;
            pnAi.Controls.Clear();
            pnAo.Controls.Clear();
            JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            IJFModule_AIO md = null;
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(devID) as IJFDevice_MotionDaq;
            if (dev != null && dev.DioCount > moduleIndex)
                md = dev.GetAio(moduleIndex);
            int AiCount = mgr.GetAiChannelCount(devID, moduleIndex);
            for (int i = 0; i < AiCount; i++)
            {
                Label lbIndex = new Label();
                lbIndex.Text = i.ToString("D2");
                lbIndex.Location = new Point(2, 10 + i * 33 + 2);
                lbIndex.Width = 30;
                pnAi.Controls.Add(lbIndex);
                UcAIOChn ucAi = new UcAIOChn();
                pnAi.Controls.Add(ucAi);
                ucAi.Location = new Point(32, 2 + i * 33);
                ucAi.Width = pnAi.Width - 34;
                ucAi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                ucAi.SetIOInfo(md, i, false, mgr.GetAiName(devID, moduleIndex, i));
                //nBottom = ucAi.Bottom;
            }
            int aoCount = mgr.GetAoChannelCount(devID, moduleIndex);
            for (int i = 0; i < aoCount; i++)
            {
                Label lbIndex = new Label();
                lbIndex.Text = i.ToString("D2");
                lbIndex.Location = new Point(2, 10 + i * 33 + 2);
                lbIndex.Width = 30;
                pnAo.Controls.Add(lbIndex);
                UcAIOChn ucAo = new UcAIOChn();
                pnAo.Controls.Add(ucAo);
                ucAo.Location = new Point(32, 2 + i * 33);
                ucAo.Width = pnAi.Width - 34;
                ucAo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                ucAo.SetIOInfo(md, i, true, mgr.GetAoName(devID, moduleIndex, i));
                //if (nBottom < ucAo.Bottom)
                //    nBottom = ucAo.Bottom;
            }

            //rtTips.Top = nBottom + 10;
        }

        private void UcNamesEditTest_Aio_Resize(object sender, EventArgs e)
        {
            lbAi.Location = new Point(2, 2);
            lbAo.Location = new Point((Width - 6) / 2 + 4, 2);
            pnAi.Location = new Point(lbAi.Left, lbAi.Bottom + 2);
            pnAi.Size = new Size((Width - 6) / 2, Height - 20 - rtTips.Height);
            pnAo.Location = new Point(lbAo.Left, lbAo.Bottom + 2);
            pnAo.Size = new Size((Width - 6) / 2, Height - 20 - rtTips.Height);
        }

        int maxTips = 1000;
        delegate void dgShowTips(string info);
        /// <summary>
        /// 显示一条信息
        /// </summary>
        /// <param name="info"></param>
        public void ShowTips(string info)
        {
            if (InvokeRequired)
            {
                Invoke(new dgShowTips(ShowTips), new object[] { info });
                return;
            }
            if (null == info)
                return;

            rtTips.AppendText(info + "\n");
            string[] lines = rtTips.Text.Split('\n');
            if (lines.Length >= maxTips)
            {
                int rmvChars = 0;
                for (int i = 0; i < lines.Length - maxTips; i++)
                    rmvChars += lines[i].Length + 1;
                rtTips.Text = rtTips.Text.Substring(rmvChars);
            }
            rtTips.Select(rtTips.TextLength, 0); //滚到最后一行
            rtTips.ScrollToCaret();//滚动到控件光标处 
        }

        public void BeginEdit()
        {
            foreach (Control ctrl in pnAi.Controls)
                if (ctrl is UcAIOChn)
                    (ctrl as UcAIOChn).IsEditting = true;
            foreach (Control ctrl in pnAo.Controls)
                if (ctrl is UcAIOChn)
                    (ctrl as UcAIOChn).IsEditting = true;
        }

        public void EndEdit()
        {
            foreach (Control ctrl in pnAi.Controls)
                if (ctrl is UcAIOChn)
                {
                    (ctrl as UcAIOChn).IOName = (ctrl as UcAIOChn).IONameEditting;
                    (ctrl as UcAIOChn).IsEditting = false;
                }
            foreach (Control ctrl in pnAo.Controls)
                if (ctrl is UcAIOChn)
                {
                    (ctrl as UcAIOChn).IOName = (ctrl as UcAIOChn).IONameEditting;
                    (ctrl as UcAIOChn).IsEditting = false;
                }
        }

        public string[] AiNames
        {
            get 
            {
                List<string> ret = new List<string>();
                foreach(Control ctrl in pnAi.Controls)
                {
                    if (ctrl is UcAIOChn)
                        ret.Add((ctrl as UcAIOChn).IONameEditting);
                }
                return ret.ToArray();
            }
        }

        public string[] AoNames
        {
            get 
            {
                List<string> ret = new List<string>();
                foreach (Control ctrl in pnAo.Controls)
                {
                    if (ctrl is UcAIOChn)
                        ret.Add((ctrl as UcAIOChn).IONameEditting);
                }
                return ret.ToArray();
            }
        }

        public void UpdateIOStatus2UI()
        {
            foreach (Control ctrl in pnAi.Controls)
                if (ctrl is UcAIOChn)
                    (ctrl as UcAIOChn).UpdateIO();
            foreach (Control ctrl in pnAo.Controls)
                if (ctrl is UcAIOChn)
                    (ctrl as UcAIOChn).UpdateIO();
        }
    }
}
