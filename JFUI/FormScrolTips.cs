using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFUI
{
    /// <summary>
    /// 显示消息的窗口
    /// </summary>
    public partial class FormScrolTips : Form
    {
        public FormScrolTips()
        {
            InitializeComponent();
            HideWhenClose = true;
        }

        private void FormScrolTips_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = 100000;
            chkShowTime.Checked = true;
            chkScrolLast.Checked = true;
        }

        int _currLine = 0; //当前行数
        int _maxLines = 100000;
        private void btClear_Click(object sender, EventArgs e)
        {
            rchTips.Text = "";
            _currLine = 0;
        }

        public bool HideWhenClose { get; set; }


        delegate void dgAddOneTips(string t);
        public void AddOneTips(string tips)
        {
            //if(InvokeRequired)
            //{
            //    Invoke(new dgAddOneTips(AddOneTips), new object[] { });
            //    return;
            //}

            BeginInvoke(new Action(() =>
            {
                if (null == tips)
                    return;

                int lineCount = tips.Split('\n').Length + 1;
                _currLine += lineCount;
                if (chkShowTime.Checked)
                    rchTips.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " ");

                rchTips.AppendText(tips + "\n");
                if (_currLine >= _maxLines)
                {
                    string[] lines = rchTips.Text.Split('\n');
                    int rmvChars = 0;
                    for (int i = 0; i < (_maxLines < 2 ? 1 : (_maxLines / 2)); i++)
                        rmvChars += lines[i].Length + 1;
                    rchTips.Text = rchTips.Text.Substring(rmvChars);
                }
                if (chkScrolLast.Checked)
                {
                    rchTips.Select(rchTips.TextLength, 0); //滚到最后一行
                    rchTips.ScrollToCaret();//滚动到控件光标处 
                }
            }));
          

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _maxLines = Convert.ToInt32(numericUpDown1.Value);
        }

        private void FormScrolTips_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HideWhenClose)
            {
                Hide();
                e.Cancel = true;
            }
        }
    }
}
