using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFHub
{
    public partial class FormAddMChnDev : Form
    {
        public FormAddMChnDev()
        {
            InitializeComponent();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(cbDevID.Text))
            {
                MessageBox.Show("设备ID不能为空");
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public string DeviceID 
        { 
            get { return cbDevID.Text; }
        }

        /// <summary>
        /// 设置可选的设备名称
        /// </summary>
        /// <param name="ids"></param>
        public void SetOptionalDeviceIDs(string[] ids)
        {
            cbDevID.Items.Clear();
            if (null != ids && ids.Length > 0)
            {
                cbDevID.Items.AddRange(ids);
                cbDevID.SelectedIndex = 0;
            }
        }
        
        List<NumericUpDown> numericUpDowns = new List<NumericUpDown>();

        /// <summary>
        /// 设置通道类型名称
        /// </summary>
        /// <param name="chnTypes"></param>
        public void SetChannelTypes(string[] chnTypes)
        {
            numericUpDowns.Clear();
            int top = 3;
            for(int i = 0; i < chnTypes.Length;i++)
            {
                TextBox tbChnType = new TextBox();
                tbChnType.ReadOnly = true;
                tbChnType.Location = new Point(3, top);
                tbChnType.Width = 170;
                tbChnType.Text = chnTypes[i];
                panel1.Controls.Add(tbChnType);
                NumericUpDown numChnCount = new NumericUpDown();
                numChnCount.Location = new Point(tbChnType.Width + 3, top);
                numChnCount.Width = panel1.Width - tbChnType.Width - 9;
                panel1.Controls.Add(numChnCount);
                numericUpDowns.Add(numChnCount);
                top = tbChnType.Bottom + 3;
            }
            //btOK.Top = panel1.Bottom + 3;
            //btCancel.Top = panel1.Bottom + 3;
            Height += (chnTypes.Length-1) * 26;//panel1.Top + (23+3)* chnTypes.Length + 3 + 30;// + 50;//btOK.Bottom + 3;

        }

        /// <summary>
        /// 各类型通道的数量
        /// </summary>
        public int[] ChannelCount 
        {
            get 
            {
                List<int> ret = new List<int>();
                foreach (NumericUpDown num in numericUpDowns)
                    ret.Add(Convert.ToInt32(num.Value));
                return ret.ToArray();
            }
            set
            {
                for (int i = 0; i < numericUpDowns.Count; i++)
                    numericUpDowns[i].Value = Convert.ToDecimal(value[i]);
            }
        }
        /// <summary>
        /// 编辑模式：添加新设备 /修改设备通道数量 /显示数量
        /// </summary>
        DevModuleSettingMode _settingMode = DevModuleSettingMode.Add;
        public DevModuleSettingMode SettingMode
        {
            get { return _settingMode; }
            set
            {
                _settingMode = value;
                switch (_settingMode)
                {
                    case DevModuleSettingMode.Add: //添加新设备
                        cbDevID.Enabled = true;
                        cbDevID.BackColor = Color.White;
                        foreach(NumericUpDown numChnCount in numericUpDowns)
                        {
                            numChnCount.Enabled = true;
                            numChnCount.BackColor = Color.White;
                        }
                        btOK.Visible = true;
                        btOK.Enabled = true;
                        btCancel.Visible = true;
                        btCancel.Enabled = true;
                        break;
                    case DevModuleSettingMode.Set://设置现有Dev
                        cbDevID.Enabled = false;
                        cbDevID.BackColor = SystemColors.Control;
                        foreach (NumericUpDown numChnCount in numericUpDowns)
                        {
                            numChnCount.Enabled = true;
                            numChnCount.BackColor = Color.White;
                        }
                        btOK.Visible = true;
                        btOK.Enabled = true;
                        btCancel.Visible = true;
                        btCancel.Enabled = true;
                        break;
                    case DevModuleSettingMode.Show://只是显示
                        cbDevID.Enabled = false;
                        cbDevID.BackColor = SystemColors.Control;
                        foreach (NumericUpDown numChnCount in numericUpDowns)
                        {
                            numChnCount.Enabled = false;
                            numChnCount.BackColor = SystemColors.Control;
                        }
                        btOK.Enabled = false;
                        btCancel.Enabled = false;

                        break;
                }
            }
        }




        delegate void dgSetDeviceID(string id);
        public void SetDeviceID(string devID)
        {
            if (InvokeRequired)
            {
                Invoke(new dgSetDeviceID(SetDeviceID), new object[] { devID });
                return;
            }
            cbDevID.Text = devID;
        }

        delegate void dgChannelCount(int chnIndex,int cnt);
        void SetChannelCount(int chnIndex,int cnt)
        {
            if (InvokeRequired)
            {
                Invoke(new dgChannelCount(SetChannelCount), new object[] { chnIndex, cnt });
                return;
            }

            numericUpDowns[chnIndex].Value = Convert.ToDecimal(cnt);//tbDeviceID.Text = devID;
        }






    }
}
