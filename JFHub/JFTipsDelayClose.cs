using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;

namespace JFHub
{

    /// <summary>
    /// 显示一条文本信息，并在指定的时间后关闭
    /// </summary>
    public static class JFTipsDelayClose
    {
        /// <summary>
        /// 以阻塞（UI）线程的方式定时关闭一条显示信息
        /// </summary>
        /// <param name="tips"></param>
        /// <param name="closeAfterSeconds"></param>
        public static void ShowDialog(string tips,int closeAfterSeconds) //这种方式会阻塞UI线程
        {
            MessageBox.Show(new FormDelayClosed(closeAfterSeconds), tips);//这种方式会
        }

        /// <summary>
        /// 以不阻赛线程的方式显示一条信息
        /// </summary>
        /// <param name="tips"></param>
        /// <param name="closeAfterSeconds"></param>
        public static void Show(string tips, int closeAfterSeconds) //不阻塞UI线程
        {
            FormDelayClosed  fm= new FormDelayClosed(closeAfterSeconds);
            fm.Tips = tips;
            fm.TopMost = true;
            fm.Show();
            //MessageBox.Show(new FormDelayClosed(closeAfterSeconds), tips);//这种方式会
        }
    }


    /// <summary>
    /// 一个定时关闭的窗口
    /// 
    /// </summary>
    internal class FormDelayClosed : Form
    {
        private RichTextBox richTextBox1;
        private Button button1;
        Timer timer1 = null;
        internal FormDelayClosed(int delaySecondsClose)
        {
            InitializeComponent();
            //计时器
            timer1 = new Timer(/*this.Container*/);
            timer1.Enabled = true;
            timer1.Interval = delaySecondsClose * 1000;
            timer1.Tick += timer1_Tick;
            timer1.Start();
        }
        private void InitializeComponent()
        {
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(12, 7);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(259, 45);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(102, 58);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormDelayClosed
            // 
            this.ClientSize = new System.Drawing.Size(284, 82);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDelayClosed";
            this.Load += new System.EventHandler(this.FormDelayClosed_Load);
            this.ResumeLayout(false);

        }

        internal string Tips
        {
            set
            {
                richTextBox1.Text = value;
            }
        }

        private void FormDelayClosed_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Close();
        }
    }
}
