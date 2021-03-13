using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;

namespace JFHub
{
    public enum DevModuleSettingMode
    {
        Add, //添加新设备
        Set, //设置当前模块数量
        Show, //只是显示信息
    }
    public partial class FormDevModuleInfo : Form
    {
        public FormDevModuleInfo()
        {
            InitializeComponent();
        }

        private void FormGenDevID_Load(object sender, EventArgs e)
        {
            string[] devIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_MotionDaq));//设备表中存在的设备ID（不是命名表）
            string[] allNamedDevIDs = JFHubCenter.Instance.MDCellNameMgr.AllMotionDaqDevices();//已在命名表中存在的DevID
            List<string> unNamedDevIds = new List<string>();
            if(null != devIDs)
            {
                if (null == allNamedDevIDs || 0 == allNamedDevIDs.Length)
                    unNamedDevIds.AddRange(devIDs);
                else
                {
                    foreach (string devID in devIDs)
                        if (!allNamedDevIDs.Contains(devID))
                            unNamedDevIds.Add(devID);
                }
            }
            cbDevIDs.Items.Clear();
            cbDevIDs.Items.AddRange(unNamedDevIds.ToArray());
            //SettingMode = DevModuleSettingMode.Add;
        }
        DevModuleSettingMode _settingMode = DevModuleSettingMode.Add;
        public DevModuleSettingMode SettingMode
        {
            get { return _settingMode; }
            set
            {
                _settingMode = value;
                switch(_settingMode)
                {
                    case DevModuleSettingMode.Add: //添加新设备
                        cbDevIDs.Enabled = true;
                        cbDevIDs.BackColor = Color.White;
                        numAioCount.Enabled = true;
                        numDioCount.Enabled = true;
                        numMotionCount.Enabled = true;
                        numTrigCount.Enabled = true;
                        btOK.Visible = true;
                        btOK.Enabled = true;
                        btCancel.Visible = true;
                        btCancel.Enabled = true;
                        break;
                    case DevModuleSettingMode.Set://设置现有Dev
                        cbDevIDs.Enabled = false;
                        cbDevIDs.BackColor = SystemColors.Control;
                        numAioCount.Enabled = true;
                        numDioCount.Enabled = true;
                        numMotionCount.Enabled = true;
                        numTrigCount.Enabled = true;
                        btOK.Visible = true;
                        btOK.Enabled = true;
                        btCancel.Visible = true;
                        btCancel.Enabled = true;
                        break;
                    case DevModuleSettingMode.Show://只是显示
                        cbDevIDs.Enabled = false;
                        numAioCount.Enabled = false;
                        numDioCount.Enabled = false;
                        numMotionCount.Enabled = false;
                        numTrigCount.Enabled = false;
                        btOK.Visible = false;
                        btCancel.Visible = false;
                        Height = btOK.Top - 1;
                        break;
                }
            }
        }

        public string DevID { get { return cbDevIDs.Text; } set { cbDevIDs.Text = value; } }
        public int DioCount { get { return Convert.ToInt32(numDioCount.Value); }set { numDioCount.Value = Convert.ToDecimal(value); } }
        public int AioCount { get { return Convert.ToInt32(numAioCount.Value); } set { numAioCount.Value = Convert.ToDecimal(value); } }
        public int MotionCount { get { return Convert.ToInt32(numMotionCount.Value); } set { numMotionCount.Value = Convert.ToDecimal(value); } }
        public int TrigCount { get { return Convert.ToInt32(numTrigCount.Value); } set { numTrigCount.Value = Convert.ToDecimal(value); } }


        private void btOK_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(DevID))
            {
                MessageBox.Show("添加设备失败\n设备ID 参数不能为空！");
                return;
            }
            if(SettingMode == DevModuleSettingMode.Add)
            {
                string[] allExistDevIDs = JFHubCenter.Instance.MDCellNameMgr.AllMotionDaqDevices();
                if(allExistDevIDs != null && allExistDevIDs.Contains(DevID))
                {
                    MessageBox.Show("添加设备失败\n设备名称配置中已存在相同的设备ID！");
                    return;
                }
            }
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


    }
}
