using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HT_Lib;

namespace DLAF
{
    public partial class Form_Wait : Form
    {
        public Form_Wait()
        {
            InitializeComponent();
            labelTest = label1.Text;
            PointNum = 0;
            Instance = this;
        }
        string labelTest = null;
        public static Form_Wait Instance = null;
        int PointNum=0;
        int HelfSecond = 5;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < progressBar1.Maximum - progressBar1.Step)
            {
                progressBar1.Value = progressBar1.Value + progressBar1.Step;
            }
            PointNum= (PointNum+1)%6;
            label1.Text = labelTest;
            for (int i=0;i< PointNum;i++)
            {
                label1.Text = label1.Text + ".";
            }
        }
        /// <summary>
        /// 显示窗体
        /// </summary>
        public static void ShowForm()
        {

            if (Instance == null || Instance.IsDisposed)
            {
                Instance = new Form_Wait();
                Instance.TopMost = false;
                int SH = Screen.PrimaryScreen.Bounds.Height;
                int SW = Screen.PrimaryScreen.Bounds.Width;
                Instance.HelfSecond = 10;
                Instance.progressBar1.Step = Instance.progressBar1.Maximum / (Instance.HelfSecond*2 * 1000 / Instance.timer1.Interval);
                //App.mainWin.Enabled = false;
                Instance.Show();
                Instance.Location = new Point((SW - Instance.Size.Width) / 2, (SH - Instance.Size.Height) / 2);
            }
            else
            {
                Instance.Activate();
            }
        }
        /// <summary>
        /// 指定一半进度条的时间
        /// </summary>
        /// <param name="helfSecond"></param>
        public static void ShowForm(int helfSecond)
        {

            if (Instance == null || Instance.IsDisposed)
            {
                Instance = new Form_Wait();
                Instance.TopMost = true;
                int SH = Screen.PrimaryScreen.Bounds.Height;
                int SW = Screen.PrimaryScreen.Bounds.Width;
                Instance.HelfSecond = helfSecond;
                Instance.progressBar1.Step = Instance.progressBar1.Maximum / (Instance.HelfSecond * 1000 / Instance.timer1.Interval);
                //App.mainWin.Enabled = false;
                Instance.Show();
                Instance.Location = new Point((SW - Instance.Size.Width) / 2, (SH - Instance.Size.Height) / 2);
            }
            else
            {
                Instance.Activate();
            }
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        public static void CloseForm()
        {
            if (Instance == null || Instance.IsDisposed)
            {
                return;
            }
            else
            {
                Instance.progressBar1.Value = Instance.progressBar1.Maximum;
                //App.mainWin.Enabled = true;

                Instance.Close();

            }
        }
    }
}
