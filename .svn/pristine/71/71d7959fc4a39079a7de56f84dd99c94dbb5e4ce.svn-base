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

namespace JFZoon
{
    public partial class Form4CmdWorkTest : Form
    {
        public Form4CmdWorkTest()
        {
            InitializeComponent();
            cmdWorker = new CmdWorkDemo();
            ((CmdWorkDemo)cmdWorker).Name = "倍速播放器Demo";
            cmdWorker.WorkStatusChanged += OnWorkStatusChange;
            cmdWorker.CustomStatusChanged += OnCustomStatusChange;
        }

        IJFCmdWork cmdWorker = null;
        

        private void Form4CmdWorkTest_Load(object sender, EventArgs e)
        {
            lbID.Text = cmdWorker.Name;
            int[] cmds = cmdWorker.AllCmds;
            if (null != cmds)
                foreach (int cmd in cmds)
                    cbCmds.Items.Add(cmdWorker.GetCmdName(cmd));

        }

        int maxTips = 100;
        delegate void dgShowTips(string info);
        void ShowTips(string info)
        {
            if (InvokeRequired)
            {
                Invoke(new dgShowTips(ShowTips), new object[] { info });
                return;
            }
            rchTips.AppendText(info + "\n");
            string[] lines = rchTips.Text.Split('\n');
            if (lines.Length >= maxTips)
            {
                int rmvChars = 0;
                for (int i = 0; i < lines.Length - maxTips; i++)
                    rmvChars += lines[i].Length + 1;
                rchTips.Text = rchTips.Text.Substring(rmvChars);
            }
            rchTips.Select(rchTips.TextLength, 0); //滚到最后一行
            rchTips.ScrollToCaret();//滚动到控件光标处 
        }

        delegate void dgWorkStatusChange(IJFCmdWork worker, JFWorkStatus status, string info);
        void OnWorkStatusChange(IJFWork worker,JFWorkStatus status,string info)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new dgWorkStatusChange(OnWorkStatusChange), new object[] { worker, status, info });
                //Invoke(new dgWorkStatusChange(OnWorkStatusChange), new object[] { worker, status, info });
                return;
            }
            lbWorkStatus.Text = status.ToString();
            ShowTips(string.Format("Worker: \"{0}\" 工作状态变化: \"{1}\" info:{2}", worker.Name, status.ToString(), info));
        }

        delegate void dgCustomStatusChange(IJFCmdWork worker, int status, string info, object param);
        void OnCustomStatusChange(IJFCmdWork worker, int status, string info,object param)
        {
            if (InvokeRequired)
            {
                Invoke(new dgCustomStatusChange(OnCustomStatusChange), new object[] { worker, status, info ,param});
                return;
            }
            lbCustomStatus.Text = cmdWorker.GetCustomStatusName(status);
            ShowTips(string.Format("Worker: \"{0}\" 自定义状态变化: \"{1}\" info:{2}", worker.Name, cmdWorker.GetCustomStatusName(status), info));
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            JFWorkCmdResult ret = cmdWorker.Start();
            if (ret != JFWorkCmdResult.Success)
                MessageBox.Show("启动失败，ret = " + ret.ToString());
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            JFWorkCmdResult ret = cmdWorker.Stop();
            if (ret != JFWorkCmdResult.Success)
                MessageBox.Show("停止失败，ret = " + ret.ToString());
        }

        private void btPause_Click(object sender, EventArgs e)
        {
            JFWorkCmdResult ret = cmdWorker.Pause();
            if (ret != JFWorkCmdResult.Success)
                MessageBox.Show("暂停失败，ret = " + ret.ToString());
        }

        private void btResume_Click(object sender, EventArgs e)
        {
            JFWorkCmdResult ret = cmdWorker.Resume();
            if (ret != JFWorkCmdResult.Success)
                MessageBox.Show("恢复失败，ret = " + ret.ToString());
        }

        private void btSendCmd_Click(object sender, EventArgs e)
        {
            if (cbCmds.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择需要发送的指令");
                return;
            }
            JFWorkCmdResult ret = cmdWorker.SendCmd(cmdWorker.AllCmds[cbCmds.SelectedIndex],1000);
            if (ret != JFWorkCmdResult.Success)
                MessageBox.Show("发送指令失败，ret = " + ret.ToString());
        }

        private void btClearInfo_Click(object sender, EventArgs e)
        {
            rchTips.Text = "";
        }

        private void timerFlush_Tick(object sender, EventArgs e)
        {
            int frameIndex = ((CmdWorkDemo)cmdWorker).CurrFrame;
            lbCurrFrame.Text = frameIndex.ToString();
        }

        private void Form4CmdWorkTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            cmdWorker.Stop();
        }
    }
}
