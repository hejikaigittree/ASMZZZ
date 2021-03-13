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

namespace JFUI
{
    public partial class UcAIOChn : UserControl
    {
        public UcAIOChn()
        {
            InitializeComponent();
        }

        private void UcAIO_Load(object sender, EventArgs e)
        {

        }

        [Category("JF属性"), Description("IO名称"), Browsable(true)]
        public string IOName 
        {
            get { return _ioName; }
            set { _ioName = value; tbIOName.Text = _ioName; } 
        }

        public string IONameEditting
        {
            get { return tbIOName.Text; }
        }

        [Category("JF属性"), Description("IO数值"), Browsable(true)]
        public double IOValue
        {
            get;
            set;
        }

        [Category("JF属性"), Description("编辑IO名称使能"), Browsable(true)]
        public bool IsEditting
        {
            get { return !tbIOName.ReadOnly; }
            set
            {
                tbIOName.ReadOnly = !value;
                if (!value)
                    tbIOName.Text = _ioName;
                tbIOName.BackColor = tbIOName.ReadOnly ? SystemColors.Control : Color.White;
            }
        }


        private void tbIOValue_TextChanged(object sender, EventArgs e)
        {
            double currValue = 0;
            if (!double.TryParse(tbIOValue.Text, out currValue))
            {
                tbIOValue.BackColor = Color.Red;
                return;
            }
            else
            {
                if (null == _aio)
                {
                    tbIOValue.BackColor = Color.Red;
                    return;
                }
                if (_ioIndex < 0|| _ioIndex >= _aio.AOCount )
                {
                    tbIOValue.BackColor = Color.Red;
                    return;
                }

                double ioValue = 0;
                int err =  _aio.GetAO(_ioIndex, out ioValue) ;
                if (0 != err)
                {
                    tbIOValue.BackColor = Color.OrangeRed;
                    return;
                }

                if(ioValue == currValue)
                {
                    tbIOValue.BackColor = Color.White;
                    return;
                }
                else
                {
                    tbIOValue.BackColor = Color.OrangeRed;
                    return;
                }
            }
        }

        /// <summary>
        /// AO时捕捉按键事件，回车键=确定输入 Esc退出输入
        /// </summary>
        private void tbIOValue_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                UpdateIO();
                tbIOValue.BackColor = Color.White;
                return;
            }
            else
            {
                if(e.KeyCode == Keys.Enter)
                {
                    double val2Set = 0;
                    if(!double.TryParse(tbIOValue.Text,out val2Set))
                    {
                        MessageBox.Show("参数格式错误");
                        return;
                    }
                    if(null == _aio)
                    {
                        MessageBox.Show("AIO模块未设置");
                        return;
                    }
                    if (!_isAo)
                        return;
                    if(_ioIndex < 0 || _ioIndex >= _aio.AOCount)
                    {
                        MessageBox.Show(string.Format("IO序号={0}超出范围：0~{1}", _ioIndex, _aio.AOCount - 1));
                        return;
                    }
                    int err = _aio.SetAO(_ioIndex, val2Set);
                    if(0!=err)
                    {
                        MessageBox.Show("设置AO值发生错误:" + _aio.GetErrorInfo(err));
                        return;
                    }

                    tbIOValue.BackColor = Color.White;
                    return;
                }
            }
        }

        IJFModule_AIO _aio = null;
        int _ioIndex = 0;
        bool _isAo = false;
        string _ioName = "";
        delegate void DgSetIOInfo(IJFModule_AIO aio, int ioIndex, bool isAo, string name);
        public void SetIOInfo(IJFModule_AIO aio,int ioIndex,bool isAo,string name)
        {
            if(InvokeRequired)
            {
                Invoke(new DgSetIOInfo(SetIOInfo), new object[] { aio, ioIndex, isAo, name });
                return;
            }
            IOName = name;
            _isAo = isAo;
            tbIOValue.Enabled = _isAo;
            _aio = aio;
            if(null == _aio)
                tbIOValue.Enabled = false;
            _ioIndex = ioIndex;
        }

        public IJFModule_AIO AIOModule { get { return _aio; } }
        public int IOIndex { get { return _ioIndex; } }

        public void UpdateIO()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateIO));
                return;
            }
            if(null == _aio)
            {
                //IOName = "ID";
                tbIOValue.Text = "AIO unset";
                return;
            }
            if(_ioIndex < 0 ||(_isAo? _ioIndex >= _aio.AOCount: _ioIndex >= _aio.AICount))
            {
                tbIOValue.Text = "Idx Err";
                return;
            }

            if(_isAo && tbIOValue.Focused)//用户正在输入AO值
                return;

            double ioValue = 0;
            int err = _isAo ? _aio.GetAO(_ioIndex, out ioValue) : _aio.GetAI(_ioIndex, out ioValue);
            if (0!= err)
            {
                tbIOValue.Text = "Upd-Err:" + _aio.GetErrorInfo(err);
                return;
            }

            tbIOValue.Text = ioValue.ToString();


        }
        //int _valMinWidth = 60;//输入框控件最小宽度
        int _valMaxWidth = 80; //输入框控件最大宽度
        int _idMinWidth = 37; //名称输入框最小宽度 120（控件最小宽度）-23（label宽度）-_valMinWidth

        private void UcAIO_SizeChanged(object sender, EventArgs e)
        {
            tbIOValue.Width = Width < (_idMinWidth + _valMaxWidth + 23) ? Width - _idMinWidth - 23 : _valMaxWidth;
            int iDWidth = Width - tbIOValue.Width - 23; //IO名称控件宽度
            lbVal.Location = new Point(iDWidth < 0 ? 0 : iDWidth, (Height - lbVal.Height) / 2);
            tbIOValue.Location = new Point(iDWidth<0?23: iDWidth + 23, (Height - tbIOValue.Height) / 2);
            tbIOName.Location = new Point(0, (Height - tbIOName.Height) / 2);
            tbIOName.Width = iDWidth < 0 ? 0 : iDWidth;
        }

        private void tbIOValue_Leave(object sender, EventArgs e)
        {
            UpdateIO();
            tbIOValue.BackColor = Color.White;
        }

        private void tbIOName_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsEditting)
                return;
            if(e.KeyCode == Keys.Enter) //确认修改名称
            {
                _ioName = tbIOName.Text;
                tbIOName.BackColor = Color.White;
            }
            else if(e.KeyCode == Keys.Escape)//取消修改名称
            {
                tbIOName.Text = _ioName;
            }
        }

        private void tbIOName_TextChanged(object sender, EventArgs e)
        {
            if (!IsEditting)
                return;
            if(tbIOName.Text != _ioName)
                tbIOName.BackColor = Color.OrangeRed;
            else
                tbIOName.BackColor = Color.White;
        }
    }
}
