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

namespace JFHub
{
    public enum NamedChnType //需要设置的通道（名称）类型
    {
        None = 0,
        Di,     //数字输入
        Do,     //数字输出
        Axis,   //轴
        CmpTrig, //比较触发器
        Ai, //模拟量输入
        Ao,//模拟量输出
        Camera, //相机
        Light, //开关式光源通道
        Trig, //触发式光源通道
    }

    /// <summary>
    /// 面板，用于在Station中 增/删 DIO/AIO/Axis/Trig命名通道               
    /// </summary>
    public partial class UcChnNamesInStation : UserControl
    {
        public UcChnNamesInStation()
        {
            InitializeComponent();
        }

        private void UcMDChnInStation_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            //UpdateStation2UI();
        }
        bool _isFormLoaded = false;
        JFStationBase _station = null;
        NamedChnType _chnType = NamedChnType.None;

        public void SetStationChnType(JFStationBase station, NamedChnType chnType)
        {
            _station = station;
            _chnType = chnType;
            if (_isFormLoaded)
                UpdateStation2UI();
        }

        int maxTips = 20;
        delegate void dgShowTips(string info);
        void ShowTips(string info)
        {
            if (InvokeRequired)
            {
                Invoke(new dgShowTips(ShowTips), new object[] { info });
                return;
            }
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

        void UpdateStation2UI()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateStation2UI));
                return;
            }
            dgvNameInfos.Rows.Clear();
            if (null == _station)
            {
                btAdd.Enabled = false;
                btDel.Enabled = false;
                lbTital.Text = "工站未设置";
                ShowTips("工站未设置，不能编辑命名通道！");
                return;

            }
            lbTital.Text = "工站:" + _station.Name;
            if(_chnType == NamedChnType.None)
            {
                btAdd.Enabled = false;
                btDel.Enabled = false;
                lbTital.Text += " 通道类型未设置";
                ShowTips("通道类型未设置，不能编辑！");
                return;
            }

            btAdd.Enabled = true;
            btDel.Enabled = true;
            lbTital.Text += (" " + _chnType.ToString() + "通道名称列表");

            switch (_chnType)
            {
                case NamedChnType.Ai:
                    _UpdateAI();
                    break;
                case NamedChnType.Ao:
                    _UpDateAO();
                    break;
                case NamedChnType.Axis:
                    _UpdateAxis();
                    break;
                case NamedChnType.Di:
                    _UpdateDI();
                    break;
                case NamedChnType.Do:
                    _UpdateDO();
                    break;
                case NamedChnType.CmpTrig:
                    _UpdateCmpTrig();
                    break;
                case NamedChnType.Camera:
                    _UpdateCmr();
                    break;
                case NamedChnType.Light:
                    _UpdateLight();
                    break;
                case NamedChnType.Trig:
                    _UpdateTrig();
                    break;
                default:
                    btAdd.Enabled = false;
                    btDel.Enabled = false;
                    ShowTips("暂不支持的通道类型:" + _chnType.ToString());
                    break;
            }
           
        }

        /// <summary>
        /// 向UI更新工站的AI命名通道
        /// </summary>
        void _UpdateAI()
        {
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            string[] allAIs = nameMgr.AllAiNames();//系统中所有可用的AI命名通道
            string[] aiNames = _station.AINames; //工站中已配置的AI通道
            if (null == allAIs || 0 == allAIs.Length) 
            {
                ShowTips("系统配置中不存在已命名的AI通道！");
                btAdd.Enabled = false;
            }
            else
                btAdd.Enabled = true;

            if (null == aiNames || 0 == aiNames.Length)
            {
                btDel.Enabled = false;
                return;
            }
            else//if(null != chnNames)
            {
                btDel.Enabled = true;
                foreach(string aiName in aiNames)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = aiName;
                    DataGridViewTextBoxCell cellInfo = new DataGridViewTextBoxCell();
                    JFDevCellInfo devInfo = nameMgr.GetAiCellInfo(aiName);
                    if (null == devInfo)
                    {
                        cellName.Style.ForeColor = Color.Red;
                        cellInfo.Value = "无效的通道名称，在配置中不存在！";
                        cellInfo.Style.ForeColor = Color.Red;
                    }
                    else
                        cellInfo.Value = string.Format("控制器:{0} - 模块:{1} - 通道:{2}", devInfo.DeviceID, devInfo.ModuleIndex, devInfo.ChannelIndex);
                    row.Cells.Add(cellName);
                    row.Cells.Add(cellInfo);
                    dgvNameInfos.Rows.Add(row);
                }
            }
        }

        void _UpDateAO()
        {
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            if (null == nameMgr.AllAoNames() || 0 == nameMgr.AllAoNames().Length) //系统中不存在已命名的AI通道
            {
                ShowTips("系统配置中不存在已命名的AO通道！");
                btAdd.Enabled = false;
            }
            else
                btAdd.Enabled = true;

            string[] aoNames = _station.AONames;
            if (null == aoNames || 0 == aoNames.Length)
            {
                btDel.Enabled = false;
                return;
            }
            else//if(null != chnNames)
            {
                btDel.Enabled = true;
                foreach (string aoName in aoNames)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = aoName;
                    DataGridViewTextBoxCell cellInfo = new DataGridViewTextBoxCell();
                    JFDevCellInfo devInfo = nameMgr.GetAoCellInfo(aoName);
                    if (null == devInfo)
                    {
                        cellName.Style.ForeColor = Color.Red;
                        cellInfo.Value = "无效的通道名称，在配置中不存在！";
                        cellInfo.Style.ForeColor = Color.Red;
                    }
                    else
                        cellInfo.Value = string.Format("控制器:{0} - 模块:{1} - 通道:{2}", devInfo.DeviceID, devInfo.ModuleIndex, devInfo.ChannelIndex);


                    row.Cells.Add(cellName);
                    row.Cells.Add(cellInfo);
                    dgvNameInfos.Rows.Add(row);
                }
            }
        }
        
        void _UpdateAxis()
        {
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            if (null == nameMgr.AllAxisNames() || 0 == nameMgr.AllAxisNames().Length) //系统中不存在已命名的AI通道
            {
                ShowTips("系统配置中不存在已命名的Axis通道！");
                btAdd.Enabled = false;
            }
            else
                btAdd.Enabled = true;

            string[] axisNames = _station.AxisNames;
            if (null == axisNames || 0 == axisNames.Length)
            {
                btDel.Enabled = false;
                return;
            }
            else//if(null != chnNames)
            {
                btDel.Enabled = true;
                foreach (string axisName in axisNames)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = axisName;
                    DataGridViewTextBoxCell cellInfo = new DataGridViewTextBoxCell();
                    JFDevCellInfo devInfo = nameMgr.GetAxisCellInfo(axisName);
                    if (null == devInfo)
                    {
                        cellName.Style.ForeColor = Color.Red;
                        cellInfo.Value = "无效的通道名称，在配置中不存在！";
                        cellInfo.Style.ForeColor = Color.Red;
                    }
                    else
                        cellInfo.Value = string.Format("控制器:{0} - 模块:{1} - 通道:{2}", devInfo.DeviceID, devInfo.ModuleIndex, devInfo.ChannelIndex);


                    row.Cells.Add(cellName);
                    row.Cells.Add(cellInfo);
                    dgvNameInfos.Rows.Add(row);
                }
            }
        }


        void _UpdateDI()
        {
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            if (null == nameMgr.AllDiNames() || 0 == nameMgr.AllDiNames().Length) //系统中不存在已命名的AI通道
            {
                ShowTips("系统配置中不存在已命名的Di通道！");
                btAdd.Enabled = false;
            }
            else
                btAdd.Enabled = true;

            string[] diNames = _station.DINames;
            if (null == diNames || 0 == diNames.Length)
            {
                btDel.Enabled = false;
                return;
            }
            else//if(null != chnNames)
            {
                btDel.Enabled = true;
                foreach (string diName in diNames)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = diName;
                    DataGridViewTextBoxCell cellInfo = new DataGridViewTextBoxCell();
                    JFDevCellInfo devInfo = nameMgr.GetDiCellInfo(diName);
                    if (null == devInfo)
                    {
                        cellName.Style.ForeColor = Color.Red;
                        cellInfo.Value = "无效的通道名称，在配置中不存在！";
                        cellInfo.Style.ForeColor = Color.Red;
                    }
                    else
                        cellInfo.Value = string.Format("控制器:{0} - 模块:{1} - 通道:{2}", devInfo.DeviceID, devInfo.ModuleIndex, devInfo.ChannelIndex);


                    row.Cells.Add(cellName);
                    row.Cells.Add(cellInfo);
                    dgvNameInfos.Rows.Add(row);
                }
            }
        }

        void _UpdateDO()
        {
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            if (null == nameMgr.AllDoNames() || 0 == nameMgr.AllDoNames().Length) //系统中不存在已命名的AI通道
            {
                ShowTips("系统配置中不存在已命名的Do通道！");
                btAdd.Enabled = false;
            }
            else
                btAdd.Enabled = true;

            string[] doNames = _station.DONames;
            if (null == doNames || 0 == doNames.Length)
            {
                btDel.Enabled = false;
                return;
            }
            else//if(null != chnNames)
            {
                btDel.Enabled = true;
                foreach (string doName in doNames)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = doName;
                    DataGridViewTextBoxCell cellInfo = new DataGridViewTextBoxCell();
                    JFDevCellInfo devInfo = nameMgr.GetDoCellInfo(doName);
                    if (null == devInfo)
                    {
                        cellName.Style.ForeColor = Color.Red;
                        cellInfo.Value = "无效的通道名称，在配置中不存在！";
                        cellInfo.Style.ForeColor = Color.Red;
                    }
                    else
                        cellInfo.Value = string.Format("控制器:{0} - 模块:{1} - 通道:{2}", devInfo.DeviceID, devInfo.ModuleIndex, devInfo.ChannelIndex);


                    row.Cells.Add(cellName);
                    row.Cells.Add(cellInfo);
                    dgvNameInfos.Rows.Add(row);
                }
            }
        }

        void _UpdateCmpTrig()
        {
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            if (null == nameMgr.AllCmpTrigNames() || 0 == nameMgr.AllCmpTrigNames().Length) //系统中不存在已命名的AI通道
            {
                ShowTips("系统配置中不存在已命名的CmpTrig通道！");
                btAdd.Enabled = false;
            }
            else
                btAdd.Enabled = true;

            string[] cmpTrigNames = _station.CmpTrigNames;
            if (null == cmpTrigNames || 0 == cmpTrigNames.Length)
            {
                btDel.Enabled = false;
                return;
            }
            else//if(null != chnNames)
            {
                btDel.Enabled = true;
                foreach (string cmpTrigName in cmpTrigNames)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = cmpTrigName;
                    DataGridViewTextBoxCell cellInfo = new DataGridViewTextBoxCell();
                    JFDevCellInfo devInfo = nameMgr.GetCmpTrigCellInfo(cmpTrigName);
                    if (null == devInfo)
                    {
                        cellName.Style.ForeColor = Color.Red;
                        cellInfo.Value = "无效的通道名称，在配置中不存在！";
                        cellInfo.Style.ForeColor = Color.Red;
                    }
                    else
                        cellInfo.Value = string.Format("控制器:{0} - 模块:{1} - 通道:{2}", devInfo.DeviceID, devInfo.ModuleIndex, devInfo.ChannelIndex);


                    row.Cells.Add(cellName);
                    row.Cells.Add(cellInfo);
                    dgvNameInfos.Rows.Add(row);
                }
            }
        }

        void _UpdateCmr()
        {
            string[] allCmrDevs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_Camera));
            if (null == allCmrDevs || 0 == allCmrDevs.Length) //系统中不存在已命名的AI通道
            {
                ShowTips("设备列表中不存在Camera通道！");
                btAdd.Enabled = false;
            }
            else
                btAdd.Enabled = true;

            string[] cmrNames = _station.CameraNames;
            if (null == cmrNames || 0 == cmrNames.Length)
            {
                btDel.Enabled = false;
                return;
            }
            else//if(null != chnNames)
            {
                btDel.Enabled = true;
                foreach (string cmrName in cmrNames)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = cmrName;
                    DataGridViewTextBoxCell cellInfo = new DataGridViewTextBoxCell();
                    IJFDevice_Camera cmr = JFHubCenter.Instance.InitorManager.GetInitor(cmrName) as IJFDevice_Camera;
                    if (null == cmr)
                    {
                        cellName.Style.ForeColor = Color.Red;
                        cellInfo.Value = "无效的通道名称，在配置中不存在！";
                        cellInfo.Style.ForeColor = Color.Red;
                    }
                    else
                    {
                        string[] paramNames = cmr.InitParamNames;
                        if (null == paramNames)
                            cellInfo.Value = "";
                        else
                        {
                            StringBuilder sbDevInfo = new StringBuilder();
                            foreach(string paramName in paramNames)
                                sbDevInfo.Append(paramName + ":" + cmr.GetInitParamValue(paramName).ToString());
                            
                            cellInfo.Value = sbDevInfo.ToString();
                        }
                        //cellInfo.Value = string.Format("控制器:{0} - 模块:{1} - 通道:{2}", devInfo.DeviceID, devInfo.ModuleIndex, devInfo.ChannelIndex);
                    }

                    row.Cells.Add(cellName);
                    row.Cells.Add(cellInfo);
                    dgvNameInfos.Rows.Add(row);
                }
            }
        }

        void _UpdateLight()
        {
            string[] allLightDevs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_LightController));
            if (null == allLightDevs || 0 == allLightDevs.Length) //系统中不存在已命名的AI通道
            {
                ShowTips("设备列表中不存在开关光源通道！");
                btAdd.Enabled = false;
            }
            else
                btAdd.Enabled = true;

            string[] chnNames = _station.LightChannelNames;
            if (null == chnNames || 0 == chnNames.Length)
            {
                btDel.Enabled = false;
                return;
            }
            else//if(null != chnNames)
            {
                btDel.Enabled = true;
                foreach (string chnName in chnNames)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = chnName;
                    DataGridViewTextBoxCell cellInfo = new DataGridViewTextBoxCell();
                    JFDevCellInfo chnCellInfo = JFHubCenter.Instance.MDCellNameMgr.GetLightCtrlChannelInfo(chnName);
                    if (null == chnCellInfo)
                    {
                        cellName.Style.ForeColor = Color.Red;
                        cellInfo.Value = "无效的通道名称，在名称配置表中不存在！";
                        cellInfo.Style.ForeColor = Color.Red;
                    }
                    else
                    {
                        IJFDevice_LightController lightDev = JFHubCenter.Instance.InitorManager.GetInitor(chnCellInfo.DeviceID) as IJFDevice_LightController;
                        if (null == lightDev)
                        {
                            cellName.Style.ForeColor = Color.Red;
                            cellInfo.Value = "控制器在设备列表中不存在！";
                            cellInfo.Style.ForeColor = Color.Red;
                        }
                        else
                        {
                             cellInfo.Value = string.Format("控制器:{0} - 通道:{1}", chnCellInfo.DeviceID, chnCellInfo.ChannelIndex);
                        }
                        //cellInfo.Value = string.Format("控制器:{0} - 模块:{1} - 通道:{2}", devInfo.DeviceID, devInfo.ModuleIndex, devInfo.ChannelIndex);
                    }
                    row.Cells.Add(cellName);
                    row.Cells.Add(cellInfo);
                    dgvNameInfos.Rows.Add(row);
                } 
                
            }
        }

        void _UpdateTrig()
        {
            string[] allTrigDevs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_TrigController));
            if (null == allTrigDevs || 0 == allTrigDevs.Length) //系统中不存在已命名的AI通道
            {
                ShowTips("设备列表中不存在触发设备！");
                btAdd.Enabled = false;
            }
            else
                btAdd.Enabled = true;

            string[] chnNames = _station.TrigChannelNames;
            if (null == chnNames || 0 == chnNames.Length)
            {
                btDel.Enabled = false;
                return;
            }
            else//if(null != chnNames)
            {
                btDel.Enabled = true;
                foreach (string chnName in chnNames)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cellName = new DataGridViewTextBoxCell();
                    cellName.Value = chnName;
                    DataGridViewTextBoxCell cellInfo = new DataGridViewTextBoxCell();
                    JFDevCellInfo chnCellInfo = JFHubCenter.Instance.MDCellNameMgr.GetTrigCtrlChannelInfo(chnName);
                    if (null == chnCellInfo)
                    {
                        cellName.Style.ForeColor = Color.Red;
                        cellInfo.Value = "无效的通道名称，在名称配置表中不存在！";
                        cellInfo.Style.ForeColor = Color.Red;
                    }
                    else
                    {
                        IJFDevice_TrigController trigDev = JFHubCenter.Instance.InitorManager.GetInitor(chnCellInfo.DeviceID) as IJFDevice_TrigController;
                        if (null == trigDev)
                        {
                            cellName.Style.ForeColor = Color.Red;
                            cellInfo.Value = "控制器在设备列表中不存在！";
                            cellInfo.Style.ForeColor = Color.Red;
                        }
                        else
                        {
                            cellInfo.Value = string.Format("控制器:{0} - 通道:{1}", chnCellInfo.DeviceID, chnCellInfo.ChannelIndex);
                        }
                        //cellInfo.Value = string.Format("控制器:{0} - 模块:{1} - 通道:{2}", devInfo.DeviceID, devInfo.ModuleIndex, devInfo.ChannelIndex);
                    }
                    row.Cells.Add(cellName);
                    row.Cells.Add(cellInfo);
                    dgvNameInfos.Rows.Add(row);
                }

            }
        }




        //删除所选通道
        private void btDel_Click(object sender, EventArgs e)
        {
            if(dgvNameInfos.SelectedRows == null || dgvNameInfos.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择需要删除的通道!");
                return;
            }
            List<string> delNames = new List<string>();
            StringBuilder sbShow = new StringBuilder("确定删除 " + _chnType.ToString() + "以下命名通道？");
            foreach (DataGridViewRow row in dgvNameInfos.SelectedRows)
            {
                delNames.Add(row.Cells[0].Value.ToString());
                sbShow.AppendLine();
                sbShow.Append(row.Cells[0].Value.ToString());
            }
            if (DialogResult.OK == MessageBox.Show(sbShow.ToString()))
            {
                foreach(string delName in delNames)
                {
                    switch(_chnType)
                    {
                        case NamedChnType.Ai:
                            _station.RemoveAI(delName);
                            break;
                        case NamedChnType.Ao:
                            _station.RemoveAO(delName);
                            break;
                        case NamedChnType.Axis:
                            _station.RemoveAxis(delName);
                            break;
                        case NamedChnType.Di:
                            _station.RemoveDI(delName);
                            break;
                        case NamedChnType.Do:
                            _station.RemoveDO(delName);
                            break;
                        case NamedChnType.CmpTrig:
                            _station.RemoveCmpTrig(delName);
                            break;
                        case NamedChnType.Camera:
                            _station.RemoveCamera(delName);
                            break;
                        case NamedChnType.Light:
                            _station.RemoveLightChannel(delName);
                            break;
                        case NamedChnType.Trig:
                            _station.RemoveTrigChannel(delName);
                            break;
                        default:
                            ShowTips("暂不支持的命名通道类型:" + _chnType.ToString());
                            return;
                    }
                }
                UpdateStation2UI();
                _station.SaveCfg();
            }
        }

        //添加通道
        private void btAdd_Click(object sender, EventArgs e)
        {
            switch(_chnType)
            {
                case NamedChnType.Ai:
                    _AddAINames();
                    break;
                case NamedChnType.Ao:
                    _AddAONames();
                    break;
                case NamedChnType.Axis:
                    _AddAxisNames();
                    break;
                case NamedChnType.Di:
                    _AddDINames();
                    break;
                case NamedChnType.Do:
                    _AddDONames();
                    break;
                case NamedChnType.CmpTrig:
                    _AddCmpTrigNames();
                    break;
                case NamedChnType.Camera:
                    _AddCmrNames();
                    break;
                case NamedChnType.Light:
                    _AddLightNames();
                    break;
                case NamedChnType.Trig:
                    _AddTrigNames();
                    break;
                default:
                    ShowTips("暂不支持添加功能的通道类型:" + _chnType.ToString());
                    break;
            }

        }

        /// <summary>
        /// 添加AI命名通道
        /// </summary>
        void _AddAONames()
        {
            string[] allChnNames = null; //系统中所有已命名的通道 
            string[] existChnNames = null; //已存在的通道名称
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            allChnNames = nameMgr.AllAoNames(); //系统名称表中所有可用的DI通道名称
            existChnNames = _station.AONames; //工站中已有的DI通道名称

            if (null == allChnNames || 0 == allChnNames.Length)
            {
                MessageBox.Show("不存在" + _chnType.ToString() + "命名通道");
                return;
            }

            List<string> lstAddNames = new List<string>(); //可供添加的DI通道名称
            if (null == existChnNames || 0 == existChnNames.Length)
                lstAddNames.AddRange(allChnNames);
            else
            {
                foreach (string chnName in allChnNames)
                {
                    if (!existChnNames.Contains(chnName))
                        lstAddNames.Add(chnName);
                }
            }

            if (lstAddNames.Count == 0)
            {
                MessageBox.Show("没有可供添加的" + _chnType.ToString() + "命名通道");
                return;
            }
            List<string> lstAddInfos = new List<string>();
            foreach (string addName in lstAddNames)
            {
                JFDevCellInfo chnInfo = nameMgr.GetAoCellInfo(addName);
                lstAddInfos.Add(string.Format("控制器:{0} - 模块:{1} - 通道:{2}", chnInfo.DeviceID, chnInfo.ModuleIndex, chnInfo.ChannelIndex));
            }
            FormAddNames frm = new FormAddNames();
            frm.Text = "为工站:" + _station.Name + " 添加Ao命名通道";
            frm.SetAvailedNames(lstAddNames.ToArray(), lstAddInfos.ToArray());
            if (DialogResult.OK == frm.ShowDialog())
            {
                string[] chnNamesWillAdd = null;
                string[] chnInfosWillAdd = null;
                frm.GetSelectNameInfos(out chnNamesWillAdd, out chnInfosWillAdd);
                for (int i = 0; i < chnNamesWillAdd.Length; i++)
                {
                    string chnName = chnNamesWillAdd[i];
                    _station.AddAO(chnName);
                }
                _station.SaveCfg();
                UpdateStation2UI();
            }
        }

        void _AddAINames()
        {
            string[] allChnNames = null; //系统中所有已命名的通道 
            string[] existChnNames = null; //已存在的通道名称
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            allChnNames = nameMgr.AllAiNames(); //系统名称表中所有可用的DI通道名称
            existChnNames = _station.AINames; //工站中已有的DI通道名称

            if (null == allChnNames || 0 == allChnNames.Length)
            {
                MessageBox.Show("不存在" + _chnType.ToString() + "命名通道");
                return;
            }

            List<string> lstAddNames = new List<string>(); //可供添加的DI通道名称
            if (null == existChnNames || 0 == existChnNames.Length)
                lstAddNames.AddRange(allChnNames);
            else
            {
                foreach (string chnName in allChnNames)
                {
                    if (!existChnNames.Contains(chnName))
                        lstAddNames.Add(chnName);
                }
            }

            if (lstAddNames.Count == 0)
            {
                MessageBox.Show("没有可供添加的" + _chnType.ToString() + "命名通道");
                return;
            }
            List<string> lstAddInfos = new List<string>();
            foreach (string addName in lstAddNames)
            {
                JFDevCellInfo chnInfo = nameMgr.GetAiCellInfo(addName);
                lstAddInfos.Add(string.Format("控制器:{0} - 模块:{1} - 通道:{2}", chnInfo.DeviceID, chnInfo.ModuleIndex, chnInfo.ChannelIndex));
            }
            FormAddNames frm = new FormAddNames();
            frm.Text = "为工站:" + _station.Name + " 添加Ai命名通道";
            frm.SetAvailedNames(lstAddNames.ToArray(), lstAddInfos.ToArray());
            if (DialogResult.OK == frm.ShowDialog())
            {
                string[] chnNamesWillAdd = null;
                string[] chnInfosWillAdd = null;
                frm.GetSelectNameInfos(out chnNamesWillAdd, out chnInfosWillAdd);
                for (int i = 0; i < chnNamesWillAdd.Length; i++)
                {
                    string chnName = chnNamesWillAdd[i];
                    _station.AddAI(chnName);
                }
                _station.SaveCfg();
                UpdateStation2UI();
            }
        }

        void _AddAxisNames()
        {
            string[] allChnNames = null; //系统中所有已命名的通道 
            string[] existChnNames = null; //已存在的通道名称
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            allChnNames = nameMgr.AllAxisNames(); //系统名称表中所有可用的DI通道名称
            existChnNames = _station.AxisNames; //工站中已有的DI通道名称

            if (null == allChnNames || 0 == allChnNames.Length)
            {
                MessageBox.Show("不存在" + _chnType.ToString() + "命名通道");
                return;
            }

            List<string> lstAddNames = new List<string>(); //可供添加的DI通道名称
            if (null == existChnNames || 0 == existChnNames.Length)
                lstAddNames.AddRange(allChnNames);
            else
            {
                foreach (string chnName in allChnNames)
                {
                    if (!existChnNames.Contains(chnName))
                        lstAddNames.Add(chnName);
                }
            }

            if (lstAddNames.Count == 0)
            {
                MessageBox.Show("没有可供添加的" + _chnType.ToString() + "命名通道");
                return;
            }
            List<string> lstAddInfos = new List<string>();
            foreach (string addName in lstAddNames)
            {
                JFDevCellInfo chnInfo = nameMgr.GetAxisCellInfo(addName);
                lstAddInfos.Add(string.Format("控制器:{0} - 模块:{1} - 通道:{2}", chnInfo.DeviceID, chnInfo.ModuleIndex, chnInfo.ChannelIndex));
            }
            FormAddNames frm = new FormAddNames();
            frm.Text = "为工站:" + _station.Name + " 添加Axis命名通道";
            frm.SetAvailedNames(lstAddNames.ToArray(), lstAddInfos.ToArray());
            if (DialogResult.OK == frm.ShowDialog())
            {
                string[] chnNamesWillAdd = null;
                string[] chnInfosWillAdd = null;
                frm.GetSelectNameInfos(out chnNamesWillAdd, out chnInfosWillAdd);
                for (int i = 0; i < chnNamesWillAdd.Length; i++)
                {
                    string chnName = chnNamesWillAdd[i];
                    _station.AddAxis(chnName);
                }
                _station.SaveCfg();
                UpdateStation2UI();
            }
        }

        void _AddDINames()
        {
            string[] allChnNames = null; //系统中所有已命名的通道 
            string[] existChnNames = null; //已存在的通道名称
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            allChnNames = nameMgr.AllDiNames(); //系统名称表中所有可用的DI通道名称
            existChnNames = _station.DINames; //工站中已有的DI通道名称

            if (null == allChnNames || 0 == allChnNames.Length)
            {
                MessageBox.Show("不存在" + _chnType.ToString() + "命名通道");
                return;
            }

            List<string> lstAddNames = new List<string>(); //可供添加的DI通道名称
            if (null == existChnNames || 0 == existChnNames.Length)
                lstAddNames.AddRange(allChnNames);
            else
            {
                foreach (string chnName in allChnNames)
                {
                    if (!existChnNames.Contains(chnName))
                        lstAddNames.Add(chnName);
                }
            }

            if (lstAddNames.Count == 0)
            {
                MessageBox.Show("没有可供添加的" + _chnType.ToString() + "命名通道");
                return;
            }
            List<string> lstAddInfos = new List<string>();
            foreach (string addName in lstAddNames)
            {
                JFDevCellInfo chnInfo = nameMgr.GetDiCellInfo(addName);
                lstAddInfos.Add(string.Format("控制器:{0} - 模块:{1} - 通道:{2}", chnInfo.DeviceID, chnInfo.ModuleIndex, chnInfo.ChannelIndex));
            }
            FormAddNames frm = new FormAddNames();
            frm.Text = "为工站:" + _station.Name + " 添加Di命名通道";
            frm.SetAvailedNames(lstAddNames.ToArray(), lstAddInfos.ToArray());
            if (DialogResult.OK == frm.ShowDialog())
            {
                string[] chnNamesWillAdd = null;
                string[] chnInfosWillAdd = null;
                frm.GetSelectNameInfos(out chnNamesWillAdd, out chnInfosWillAdd);
                for (int i = 0; i < chnNamesWillAdd.Length; i++)
                {
                    string chnName = chnNamesWillAdd[i];
                    _station.AddDI(chnName);
                }
                _station.SaveCfg();
                UpdateStation2UI();
            }

        }

        void _AddDONames()
        {
            string[] allChnNames = null; //系统中所有已命名的通道 
            string[] existChnNames = null; //已存在的通道名称
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            allChnNames = nameMgr.AllDoNames(); //系统名称表中所有可用的DI通道名称
            existChnNames = _station.DONames; //工站中已有的DI通道名称

            if (null == allChnNames || 0 == allChnNames.Length)
            {
                MessageBox.Show("不存在" + _chnType.ToString() + "命名通道");
                return;
            }

            List<string> lstAddNames = new List<string>(); //可供添加的DI通道名称
            if (null == existChnNames || 0 == existChnNames.Length)
                lstAddNames.AddRange(allChnNames);
            else
            {
                foreach (string chnName in allChnNames)
                {
                    if (!existChnNames.Contains(chnName))
                        lstAddNames.Add(chnName);
                }
            }

            if (lstAddNames.Count == 0)
            {
                MessageBox.Show("没有可供添加的" + _chnType.ToString() + "命名通道");
                return;
            }
            List<string> lstAddInfos = new List<string>();
            foreach (string addName in lstAddNames)
            {
                JFDevCellInfo chnInfo = nameMgr.GetDoCellInfo(addName);
                lstAddInfos.Add(string.Format("控制器:{0} - 模块:{1} - 通道:{2}", chnInfo.DeviceID, chnInfo.ModuleIndex, chnInfo.ChannelIndex));
            }
            FormAddNames frm = new FormAddNames();
            frm.Text = "为工站:" + _station.Name + " 添加Do命名通道";
            frm.SetAvailedNames(lstAddNames.ToArray(), lstAddInfos.ToArray());
            if (DialogResult.OK == frm.ShowDialog())
            {
                string[] chnNamesWillAdd = null;
                string[] chnInfosWillAdd = null;
                frm.GetSelectNameInfos(out chnNamesWillAdd, out chnInfosWillAdd);
                for (int i = 0; i < chnNamesWillAdd.Length; i++)
                {
                    string chnName = chnNamesWillAdd[i];
                    _station.AddDO(chnName);
                }
                _station.SaveCfg();
                UpdateStation2UI();
            }
        }

        void _AddCmpTrigNames()
        {
            string[] allChnNames = null; //系统中所有已命名的通道 
            string[] existChnNames = null; //已存在的通道名称
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            allChnNames = nameMgr.AllCmpTrigNames(); //系统名称表中所有可用的DI通道名称
            existChnNames = _station.CmpTrigNames; //工站中已有的DI通道名称

            if (null == allChnNames || 0 == allChnNames.Length)
            {
                MessageBox.Show("不存在" + _chnType.ToString() + "命名通道");
                return;
            }

            List<string> lstAddNames = new List<string>(); //可供添加的DI通道名称
            if (null == existChnNames || 0 == existChnNames.Length)
                lstAddNames.AddRange(allChnNames);
            else
            {
                foreach (string chnName in allChnNames)
                {
                    if (!existChnNames.Contains(chnName))
                        lstAddNames.Add(chnName);
                }
            }

            if (lstAddNames.Count == 0)
            {
                MessageBox.Show("没有可供添加的" + _chnType.ToString() + "命名通道");
                return;
            }
            List<string> lstAddInfos = new List<string>();
            foreach (string addName in lstAddNames)
            {
                JFDevCellInfo chnInfo = nameMgr.GetCmpTrigCellInfo(addName);
                lstAddInfos.Add(string.Format("控制器:{0} - 模块:{1} - 通道:{2}", chnInfo.DeviceID, chnInfo.ModuleIndex, chnInfo.ChannelIndex));
            }
            FormAddNames frm = new FormAddNames();
            frm.Text = "为工站:" + _station.Name + " 添加CmpTrig命名通道";
            frm.SetAvailedNames(lstAddNames.ToArray(), lstAddInfos.ToArray());
            if (DialogResult.OK == frm.ShowDialog())
            {
                string[] chnNamesWillAdd = null;
                string[] chnInfosWillAdd = null;
                frm.GetSelectNameInfos(out chnNamesWillAdd, out chnInfosWillAdd);
                for (int i = 0; i < chnNamesWillAdd.Length; i++)
                {
                    string chnName = chnNamesWillAdd[i];
                    _station.AddCmpTrig(chnName);
                }
                _station.SaveCfg();
                UpdateStation2UI();
            }
        }

        void _AddCmrNames()
        {
            string[] allChnNames = null; //系统中所有已命名的通道 
            string[] existChnNames = null; //已存在的通道名称
            JFInitorManager initorMgr = JFHubCenter.Instance.InitorManager;
            allChnNames = initorMgr.GetIDs(typeof(IJFDevice_Camera));
            existChnNames = _station.CameraNames; //工站中已有的DI通道名称

            if (null == allChnNames || 0 == allChnNames.Length)
            {
                MessageBox.Show("不存在" + _chnType.ToString() + "命名通道");
                return;
            }

            List<string> lstAddNames = new List<string>(); //可供添加的DI通道名称
            if (null == existChnNames || 0 == existChnNames.Length)
                lstAddNames.AddRange(allChnNames);
            else
            {
                foreach (string chnName in allChnNames)
                {
                    if (!existChnNames.Contains(chnName))
                        lstAddNames.Add(chnName);
                }
            }

            if (lstAddNames.Count == 0)
            {
                MessageBox.Show("没有可供添加的" + _chnType.ToString() + "命名通道");
                return;
            }
            List<string> lstAddInfos = new List<string>();
            foreach (string addName in lstAddNames)
            {
                StringBuilder sbDevInfo = new StringBuilder();
                IJFDevice_Camera cmr = initorMgr.GetInitor(addName) as IJFDevice_Camera;
                string[] paramNames = cmr.InitParamNames;
                if(null != paramNames)
                    foreach(string paramName in paramNames)
                        sbDevInfo.Append(paramName + ":" + cmr.GetInitParamValue(paramName).ToString() + " ");
                    
                lstAddInfos.Add(sbDevInfo.ToString());
            }
            FormAddNames frm = new FormAddNames();
            frm.Text = "为工站:" + _station.Name + " 添加Camera命名通道";
            frm.SetAvailedNames(lstAddNames.ToArray(), lstAddInfos.ToArray());
            if (DialogResult.OK == frm.ShowDialog())
            {
                string[] chnNamesWillAdd = null;
                string[] chnInfosWillAdd = null;
                frm.GetSelectNameInfos(out chnNamesWillAdd, out chnInfosWillAdd);
                for (int i = 0; i < chnNamesWillAdd.Length; i++)
                {
                    string chnName = chnNamesWillAdd[i];
                    _station.AddCamera(chnName);
                }
                _station.SaveCfg();
                UpdateStation2UI();
            }
        }

        void _AddLightNames()
        {
            string[] allChnNames = null; //系统中所有已命名的通道 
            string[] existChnNames = null; //已存在的通道名称
            //JFInitorManager initorMgr = JFHubCenter.Instance.InitorManager;
            allChnNames = JFHubCenter.Instance.MDCellNameMgr.AllLightChannelNames();
            existChnNames = _station.LightChannelNames; //工站中已有的DI通道名称

            if (null == allChnNames || 0 == allChnNames.Length)
            {
                MessageBox.Show("不存在" + _chnType.ToString() + "命名通道");
                return;
            }

            List<string> lstAddNames = new List<string>(); //可供添加的DI通道名称
            if (null == existChnNames || 0 == existChnNames.Length)
                lstAddNames.AddRange(allChnNames);
            else
            {
                foreach (string chnName in allChnNames)
                {
                    if (!existChnNames.Contains(chnName))
                        lstAddNames.Add(chnName);
                }
            }

            if (lstAddNames.Count == 0)
            {
                MessageBox.Show("没有可供添加的" + _chnType.ToString() + "命名通道");
                return;
            }
            List<string> lstAddInfos = new List<string>();
            foreach (string addName in lstAddNames)
            {
                JFDevCellInfo cellInfo = JFHubCenter.Instance.MDCellNameMgr.GetLightCtrlChannelInfo(addName);
                if (null == cellInfo)
                    lstAddInfos.Add("无设备通道信息");
                else
                    lstAddInfos.Add("设备:" + cellInfo.DeviceID + " 通道:" + cellInfo.ChannelIndex);
            }
            FormAddNames frm = new FormAddNames();
            frm.Text = "为工站:" + _station.Name + " 添加开关光源命名通道";
            frm.SetAvailedNames(lstAddNames.ToArray(), lstAddInfos.ToArray());
            if (DialogResult.OK == frm.ShowDialog())
            {
                string[] chnNamesWillAdd = null;
                string[] chnInfosWillAdd = null;
                frm.GetSelectNameInfos(out chnNamesWillAdd, out chnInfosWillAdd);
                for (int i = 0; i < chnNamesWillAdd.Length; i++)
                {
                    string chnName = chnNamesWillAdd[i];
                    _station.AddLightChannel(chnName);
                }
                _station.SaveCfg();
                UpdateStation2UI();
            }
        }

        void _AddTrigNames()
        {
            string[] allChnNames = null; //系统中所有已命名的通道 
            string[] existChnNames = null; //已存在的通道名称
            //JFInitorManager initorMgr = JFHubCenter.Instance.InitorManager;
            allChnNames = JFHubCenter.Instance.MDCellNameMgr.AllTrigChannelNames();
            existChnNames = _station.TrigChannelNames; //工站中已有的DI通道名称

            if (null == allChnNames || 0 == allChnNames.Length)
            {
                MessageBox.Show("不存在" + _chnType.ToString() + "命名通道");
                return;
            }

            List<string> lstAddNames = new List<string>(); //可供添加的DI通道名称
            if (null == existChnNames || 0 == existChnNames.Length)
                lstAddNames.AddRange(allChnNames);
            else
            {
                foreach (string chnName in allChnNames)
                {
                    if (!existChnNames.Contains(chnName))
                        lstAddNames.Add(chnName);
                }
            }

            if (lstAddNames.Count == 0)
            {
                MessageBox.Show("没有可供添加的" + _chnType.ToString() + "命名通道");
                return;
            }
            List<string> lstAddInfos = new List<string>();
            foreach (string addName in lstAddNames)
            {
                JFDevCellInfo cellInfo = JFHubCenter.Instance.MDCellNameMgr.GetTrigCtrlChannelInfo(addName);
                if (null == cellInfo)
                    lstAddInfos.Add("无设备通道信息");
                else
                    lstAddInfos.Add("设备:" + cellInfo.DeviceID + " 通道:" + cellInfo.ChannelIndex);
            }
            FormAddNames frm = new FormAddNames();
            frm.Text = "为工站:" + _station.Name + " 添加触发光源命名通道";
            frm.SetAvailedNames(lstAddNames.ToArray(), lstAddInfos.ToArray());
            if (DialogResult.OK == frm.ShowDialog())
            {
                string[] chnNamesWillAdd = null;
                string[] chnInfosWillAdd = null;
                frm.GetSelectNameInfos(out chnNamesWillAdd, out chnInfosWillAdd);
                for (int i = 0; i < chnNamesWillAdd.Length; i++)
                {
                    string chnName = chnNamesWillAdd[i];
                    _station.AddTrigChannel(chnName);
                }
                _station.SaveCfg();
                UpdateStation2UI();
            }
        }

        private void UcChnNamesInStation_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                UpdateStation2UI();
            }
            else
            {
                ;
            }
        }
    }
}
