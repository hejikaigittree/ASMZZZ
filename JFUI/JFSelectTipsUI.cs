using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFUI
{
    internal class FormSelectTextItem : Form
    {

        internal FormSelectTextItem(string[] items,int defaultIndex, int closeAfterSeconds)
        {
            InitializeComponent();
            _items2Choise = items;
            _defaultIndex = defaultIndex;
            if (closeAfterSeconds >= 0)
            {
                timer1 = new Timer(/*this.Container*/);
                timer1.Enabled = true;
                timer1.Interval = closeAfterSeconds * 1000;
                timer1.Tick += timer1_Tick;
                timer1.Start();
            }

        }
        string[] Items
        {
            set
            {
                _items2Choise = value;
            }
        }

        int _defaultIndex = 0;

        public int SelectIndex{ get { return _selectIndex; } }

        string[] _items2Choise = null;
        Timer timer1 = null;

        private TableLayoutPanel tableLayoutPanel1;
        private Button btOK;
        private Button btCancel;

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(339, 199);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btOK.Location = new System.Drawing.Point(86, 206);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 1;
            this.btOK.Text = " 确定";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.Location = new System.Drawing.Point(193, 206);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // FormSelectTextItem
            // 
            this.ClientSize = new System.Drawing.Size(344, 232);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectTextItem";
            this.Load += new System.EventHandler(this.FormSelectTextItem_Load);
            this.ResumeLayout(false);

        }

        int _selectIndex = -1;

        private void btOK_Click(object sender, EventArgs e)
        {
            if (timer1 != null)
                timer1.Stop();
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            if (timer1 != null)
                timer1.Stop();
            _selectIndex = -1;
            DialogResult = DialogResult.Cancel;
        }


        private void FormSelectTextItem_Load(object sender, EventArgs e)
        {
            if(null != _items2Choise)
                for(int i = 0; i < _items2Choise.Length;i++)
                {
                    RadioButton rb = new RadioButton();
                    tableLayoutPanel1.Controls.Add(rb, 0, i);
                    rb.Tag = i;
                    rb.Click += OnRadioButtonClick;
                    RichTextBox tb = new RichTextBox();
                    tb.Height = 30;
                    tb.BorderStyle = BorderStyle.None;
                    tb.BackColor = SystemColors.Control;
                    //tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    tb.Text = _items2Choise[i];
                    tb.Dock = DockStyle.Fill;
                    tableLayoutPanel1.Controls.Add(tb, 1, i);
                    if (i == _defaultIndex)
                    {
                        rb.Checked = true;
                        _selectIndex = _defaultIndex;
                    }
                    
                }
        }

        void OnRadioButtonClick(object sender,EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if(rb.Checked)
            {
                _selectIndex = (int)rb.Tag;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public static class JFSelectTipsUI
    {
        public static int ShowDialog(string caption,string[] items2Choise,int defaultIndex,int closeAfterSeconds = -1)
        {
            FormSelectTextItem fm = new FormSelectTextItem(items2Choise, defaultIndex, closeAfterSeconds);
            fm.Text = caption;
            if (fm.ShowDialog() == DialogResult.OK)
                return fm.SelectIndex;
            return -1;
        }
    }
}
