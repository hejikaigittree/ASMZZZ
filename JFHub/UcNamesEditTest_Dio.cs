using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFUI;
using JFInterfaceDef;

namespace JFHub
{
    /// <summary>
    /// UcDioCellNames类： DIO名称修改/DIO功能测试
    /// </summary>
    public partial class UcNamesEditTest_Dio : UserControl
    {
        public UcNamesEditTest_Dio()
        {
            InitializeComponent();
        }

        private void UcDioCellNames_Load(object sender, EventArgs e)
        {
            
        }

        private void UcDioCellNames_Resize(object sender, EventArgs e)
        {
            lbDi.Location = new Point(2, 2);
            lbDo.Location = new Point((Width - 6) / 2 + 4, 2);
            pnDi.Location = new Point(lbDi.Left, lbDi.Bottom + 2);
            pnDi.Size = new Size((Width - 6) / 2, Bottom-50);
            //pnDi.Width = (Width - 6) / 2;
            pnDo.Location = new Point(lbDo.Left, lbDo.Bottom + 2);
            pnDo.Size = new Size((Width - 6) / 2, Bottom - 50);//pnDo.Size = new Size((Width - 6) / 2, rtTips.Top - 4);
        }

        public void UpdateChannelsInfo(string devID,int moduleIndex)
        {
            pnDi.Controls.Clear();
            pnDo.Controls.Clear();
            JFDevCellNameManeger mgr = JFHubCenter.Instance.MDCellNameMgr;
            IJFModule_DIO md = null;
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(devID) as IJFDevice_MotionDaq;
            if (dev != null && dev.DioCount > moduleIndex)
                md = dev.GetDio(moduleIndex);
            int diCount = mgr.GetDiChannelCount(devID, moduleIndex);
            for(int i = 0; i < diCount;i++)
            {
                Label lbIndex = new Label();
                lbIndex.Text = i.ToString("D2");
                lbIndex.Location = new Point(2,10 + i * 33 + 2);
                lbIndex.Width = 30;
                pnDi.Controls.Add(lbIndex);
                UcDioChn ucDi = new UcDioChn();
                pnDi.Controls.Add(ucDi);
                ucDi.Location = new Point(32, 2 + i * 33);
                ucDi.Width = pnDi.Width - 34;
                ucDi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                ucDi.SetDioInfo(md, i, false, mgr.GetDiName(devID, moduleIndex, i));
            }
            int doCount = mgr.GetDoChannelCount(devID, moduleIndex);
            for (int i = 0; i < doCount; i++)
            {
                Label lbIndex = new Label();
                lbIndex.Text = i.ToString("D2");
                lbIndex.Location = new Point(2, 10 + i * 33 + 2);
                lbIndex.Width = 30;
                pnDo.Controls.Add(lbIndex);
                UcDioChn ucDo = new UcDioChn();
                pnDo.Controls.Add(ucDo);
                ucDo.Location = new Point(32, 2 + i * 33);
                ucDo.Width = pnDi.Width - 34;
                ucDo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                ucDo.SetDioInfo(md, i, true, mgr.GetDoName(devID, moduleIndex, i));
                
            }

        }

        int maxTips = 1000;
        delegate void dgShowTips(string info);
        /// <summary>
        /// 显示一条信息
        /// </summary>
        /// <param name="info"></param>
        public void ShowTips(string info)
        {
            if(InvokeRequired)
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

        public void UpdateIO2UI()
        {
            foreach(Control ctrl in pnDi.Controls)
                if (ctrl is UcDioChn)
                    (ctrl as UcDioChn).UpdateIO();
            foreach (Control ctrl in pnDo.Controls)
                if (ctrl is UcDioChn)
                    (ctrl as UcDioChn).UpdateIO();
        }

        /// <summary>
        /// 获取Di的编辑名称
        /// </summary>
        public string[] DiNames 
        {
            get 
            {
                List<string> ret = new List<string>();
                foreach (Control ctrl in pnDi.Controls)
                    if (ctrl is UcDioChn)
                        ret.Add((ctrl as UcDioChn).IONameEditting);
                return ret.ToArray();
            }
        }

        public string[] DoNames
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (Control ctrl in pnDo.Controls)
                    if (ctrl is UcDioChn)
                        ret.Add((ctrl as UcDioChn).IONameEditting);
                return ret.ToArray();
            }
        }

        public void BeginEdit()
        {
            foreach (Control ctrl in pnDi.Controls)
                if (ctrl is UcDioChn)
                    (ctrl as UcDioChn).IsEditting = true;
            foreach (Control ctrl in pnDo.Controls)
                if (ctrl is UcDioChn)
                    (ctrl as UcDioChn).IsEditting = true;
        }

        public void EndEdit()
        {
            foreach (Control ctrl in pnDi.Controls)
                if (ctrl is UcDioChn)
                {
                    UcDioChn ucc = ctrl as UcDioChn;
                    ucc.IOName = ucc.IONameEditting;
                    ucc.IsEditting = false;

                }
            foreach (Control ctrl in pnDo.Controls)
                if (ctrl is UcDioChn)
                {
                    UcDioChn ucc = ctrl as UcDioChn;
                    ucc.IOName = ucc.IONameEditting;
                    ucc.IsEditting = false;

                }
        }


    }
}
