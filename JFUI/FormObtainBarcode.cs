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
    /// 获取输入字串（条码）
    /// </summary>
    public partial class FormObtainBarcode : Form
    {
        public enum OBMode
        {
            Scanner, //扫码枪
            Manual,//手动输入
            Selectabel, //选择
        }

        public FormObtainBarcode()
        {
            InitializeComponent();

        }


        private void FormObtainBarcode_Load(object sender, EventArgs e)
        {
            if (_obMode != OBMode.Selectabel)
                cbBarcode.Focus();
            else
                tbBarcode.Focus();
            //cbOBMode.Items.Add("码枪");
            //cbOBMode.Items.Add("手动");
            //cbOBMode.Items.Add("选择");
            cbBarcode.DropDownStyle = ComboBoxStyle.DropDownList;
            AdjustOBView();
            if(!string.IsNullOrEmpty(_currBacode))
            {
                if (_obMode != OBMode.Selectabel)
                    tbBarcode.Text = _currBacode;
            }
            StartPosition = FormStartPosition.CenterScreen;

        }
        bool _isSettingCbOB = false;

        string _currBacode = ""; //初始值
        string[] _barcodeOptions = new string[] { }; //可选值列表，由外界输入 
        /// <summary>
        /// 更新窗口布局
        /// </summary>
        void AdjustOBView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustOBView));
                return;
            }
            _isSettingCbOB = true;
            cbOBMode.SelectedIndex = (int)_obMode;
            _isSettingCbOB = false;

            cbBarcode.Items.Clear();
            if (null != _barcodeOptions)
            {
                foreach (string s in _barcodeOptions)
                    cbBarcode.Items.Add(s);
                if (_barcodeOptions.Contains(_currBacode))
                    cbBarcode.Text = _currBacode;
            }

            if (_obMode == OBMode.Selectabel)
            {
                cbBarcode.Visible = true;
                tbBarcode.Visible = false;
                cbBarcode.Focus();
               
            }
            else
            {
                cbBarcode.Visible = false;
                tbBarcode.Visible = true;
                tbBarcode.Focus();
            }

        }

        private void FormObtainBarcode_MouseDown(object sender, MouseEventArgs e)
        {
            if (_obMode == OBMode.Selectabel)
                cbBarcode.Focus();
            else
                tbBarcode.Focus();
        }


        private void btOK_Click(object sender, EventArgs e)
        {
            string bc = null;
            if (_obMode == OBMode.Selectabel)
                bc = cbBarcode.Text;
            else
                bc = tbBarcode.Text;
            if (string.IsNullOrEmpty(bc))
                if (DialogResult.OK != MessageBox.Show("当前输入为空字串，确定继续？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    return;
                DialogResult = DialogResult.OK;
        }


        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            if (_obMode == OBMode.Selectabel)
            {
                cbBarcode.SelectedIndex = -1;
                cbBarcode.Focus();
            }
            else
            {
                tbBarcode.Text = "";
                tbBarcode.Focus();
            }
        }
        DateTime _lastInputTime = DateTime.MinValue; //未输入
        private void tbBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if(tbBarcode.Text.Length > 0)
                {
                    DialogResult = DialogResult.OK;
                    return;
                }
                else
                {
                    if (DialogResult.OK != MessageBox.Show("当前输入为空字串，确定继续？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                        return;
                    DialogResult = DialogResult.OK;
                    return;
                }


            }

            if (_obMode == OBMode.Scanner) //扫码枪输入
            {
                if (_lastInputTime == DateTime.MinValue)
                {
                    _lastInputTime = DateTime.Now;
                    return;
                }
                DateTime currTime = DateTime.Now;
                TimeSpan ts = currTime - _lastInputTime;
                _lastInputTime = DateTime.Now;
                if(ts.TotalMilliseconds > 1000) //相邻输入大于1秒，判定为新条码
                {
                    tbBarcode.Text = "";
                    //tbBarcode.Text = e.KeyCode.ToString();
                }
            }
        }

        OBMode _obMode = OBMode.Scanner;
        /// <summary>
        /// 条码输入模式
        /// </summary>
        public OBMode ObtainMode
        {
            get { return _obMode; }
            set 
            {
                _obMode = value;
                if (Created)
                    AdjustOBView();
            }

        }

        public void SetBarcodeOptions(string[] options)
        {
            _barcodeOptions = options;
            AdjustOBView();
        }

        /// <summary>
        /// 获取用户输入的条码
        /// </summary>
        public string Barcode 
        {
            get 
            {
                if (DialogResult != DialogResult.OK)
                    return string.Empty;
                if(_obMode == OBMode.Selectabel)
                    return cbBarcode.Text;
                return tbBarcode.Text;
            }
        }

        private void cbOBMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isSettingCbOB)
                return;
            if (cbOBMode.SelectedIndex < 0)
                return;
            _obMode = (OBMode)cbOBMode.SelectedIndex;
            if (_obMode == OBMode.Selectabel)
            {
                cbBarcode.Visible = true;
                tbBarcode.Visible = false;
                cbBarcode.Focus();
            }
            else
            {
                cbBarcode.Visible = false;
                tbBarcode.Visible = true;
                tbBarcode.Focus();
            }
        }


        /// <summary>
        /// 初始值
        /// </summary>
        /// <param name="bc"></param>
        public void  SetInitBarcode(string bc)
        {
            _currBacode = bc;
            return;
        }

        private void FormObtainBarcode_Activated(object sender, EventArgs e)
        {
            if (_obMode == OBMode.Selectabel)
            {
                cbBarcode.Focus();
            }
            else
            {
                tbBarcode.Focus();
            }
            StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
