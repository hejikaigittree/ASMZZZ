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
using JFUI;
using JFToolKits;
using JFVision;
using System.Xml.Serialization;
using System.IO;
using System.Collections;

namespace JFHub
{
    /// <summary>
    /// 已创建设备的管理界面
    /// </summary>
    public partial class FormInitorMgr : Form
    {
        public FormInitorMgr()
        {
            InitializeComponent();

        }

        private void FormDevMgr_Load(object sender, EventArgs e)
        {
            isEditting = false;
            btCancel.Enabled = false;
            btInit.Enabled = false;
            btRemove.Enabled = false;
            btDebug.Enabled = false;
            btCfg.Enabled = false;
            chkSelfUI.Enabled = false;
            ///加载现有设备到列表中
            dgvDevs.Rows.Clear();
            string[] devIDs = JFHubCenter.Instance.InitorManager.GetIDs(InitorType);
            foreach(string devID in devIDs)
            {
                IJFInitializable dev = JFHubCenter.Instance.InitorManager[devID];
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cellID = new DataGridViewTextBoxCell();
                cellID.Value = devID;
                row.Cells.Add(cellID);

                DataGridViewTextBoxCell cellModel = new DataGridViewTextBoxCell();
                cellModel.Value = JFinitializerHelper.DispalyTypeName(dev.GetType());
                row.Cells.Add(cellModel);
                DataGridViewTextBoxCell cellType = new DataGridViewTextBoxCell();
                cellType.Value = dev.GetType().Name;
                row.Cells.Add(cellType);
                dgvDevs.Rows.Add(row);
                if (!dev.IsInitOK)
                    row.DefaultCellStyle.ForeColor = Color.Red;
                

            }

            dgvDevs.ClearSelection();
            RemoveAllPEs();

        }

        string _initorCaption = "设备"; //正在调试的接口类型名称 ,   如：设备/相机/工站
        public string InitorCaption
        {
            get { return _initorCaption; }
            set
            {
                _initorCaption = value;
                Text = _initorCaption + "模块管理";
                labelID.Text = _initorCaption + "ID";
                dgvDevs.Columns[0].HeaderText = _initorCaption + "ID";
                btAdd.Text = "添加新" + _initorCaption;
                btRemove.Text = "移除所选" + _initorCaption;
                btDebug.Text = "调试所选" + _initorCaption;
                btCfg.Text = "编辑" + _initorCaption +  "配置";

            }
        }


        Type _initorType = typeof(IJFDevice);
        public Type InitorType
        {
            get { return _initorType; }
            set
            {
                _initorType = value;
            }
        }



        /// <summary>移除所选设备</summary>
        private void btRemove_Click(object sender, EventArgs e)
        {
            if(dgvDevs.SelectedRows == null || 0 == dgvDevs.SelectedRows.Count)
            {
                MessageBox.Show("请先选择需要移除的" + _initorCaption);
                return;
            }

            DataGridViewRow row = dgvDevs.SelectedRows[0];
            if(JFHubCenter.Instance.StationMgr.DeclearedStationNames.Contains(row.Cells[0].Value.ToString()))
            {
                MessageBox.Show("注册工站不能被移除！");
                return;
            }
            if (DialogResult.Cancel == MessageBox.Show("确定要移除:" + row.Cells[0].Value.ToString() + "?", "移除设备警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                return;
            JFHubCenter.Instance.InitorManager.Remove(row.Cells[0].Value.ToString()); //从设备管理器中删除
            JFXmlSortedDictionary<string, List<object>> devCfg = JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_InitDevParams) as JFXmlSortedDictionary<string, List<object>>;
            devCfg.Remove(row.Cells[0].Value.ToString());//从设备配置文件中删除
            JFHubCenter.Instance.SystemCfg.NotifyItemChanged(JFHubCenter.CK_InitDevParams);
            JFHubCenter.Instance.SystemCfg.Save();
            MessageBox.Show(_initorCaption + row.Cells[0].Value.ToString() + "已从系统中移除！");
            dgvDevs.Rows.Remove(row);
            RemoveAllPEs();
            dgvDevs.ClearSelection();
        }

        /// <summary>添加新设备</summary>
        private void btAdd_Click(object sender, EventArgs e)
        {
            FormCreateInitor fm = new FormCreateInitor();
            fm.Text = "创建" + InitorCaption + "对象";
            fm.MatchType = InitorType;
            if (DialogResult.OK == fm.ShowDialog())
            {
                IJFInitializable newDevice = fm.Initor;
                string devID = fm.ID;
                JFHubCenter.Instance.InitorManager.Add(devID, newDevice);
               

                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cellID = new DataGridViewTextBoxCell();
                cellID.Value = devID;
                row.Cells.Add(cellID);

                DataGridViewTextBoxCell cellModel = new DataGridViewTextBoxCell();
                cellModel.Value = JFinitializerHelper.DispalyTypeName(newDevice.GetType());
                row.Cells.Add(cellModel);
                DataGridViewTextBoxCell cellType = new DataGridViewTextBoxCell();
                cellType.Value = newDevice.GetType().Name;
                row.Cells.Add(cellType);
                dgvDevs.Rows.Add(row);
                newDevice.Initialize();
                if (!newDevice.IsInitOK)
                {
                    btInit.Enabled = true;
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                    btInit.Enabled = false;



                /// 更新参数到界面
                dgvDevs.Rows[dgvDevs.Rows.Count - 1].Selected = true;
                RemoveAllPEs();
                string[] iniParamNames = newDevice.InitParamNames;
                if (null == iniParamNames)
                {
                    btEditSave.Enabled = false;
                    return;
                }
                btEditSave.Enabled = true;
                int locY = btInit.Location.Y + btInit.Size.Height + 5;
                foreach (string ipName in iniParamNames)
                {
                    UcJFParamEdit pe = new UcJFParamEdit();
                    pe.Width = gbParams.Width - 1;
                    pe.Location = new Point(4, locY);
                    pe.SetParamDesribe(newDevice.GetInitParamDescribe(ipName));
                    locY += pe.Height;
                    pe.SetParamValue(newDevice.GetInitParamValue(ipName));
                    pe.IsValueReadOnly = true;
                    gbParams.Controls.Add(pe);
                }

                Type devType = newDevice.GetType();
                if (typeof(IJFDevice_Camera).IsAssignableFrom(devType) ||
                    typeof(IJFDevice_LightController).IsAssignableFrom(devType) ||
                    typeof(IJFDevice_MotionDaq).IsAssignableFrom(devType) ||
                    typeof(IJFDevice_TrigController).IsAssignableFrom(devType) ||
                    typeof(IJFRealtimeUIProvider).IsAssignableFrom(devType)) //提供调试界面
                {
                    btDebug.Enabled = true;
                    if (typeof(IJFRealtimeUIProvider).IsAssignableFrom(devType))
                        chkSelfUI.Enabled = true;
                    else
                    {
                        chkSelfUI.Checked = false;
                        chkSelfUI.Enabled = false;
                    }
                }
                else
                    btDebug.Enabled = false;

                if (typeof(IJFConfigUIProvider).IsAssignableFrom(devType))
                    btCfg.Enabled = true;
                else
                    btCfg.Enabled = false;
            }
        
        }

        

        /// <summary>调试所选设备</summary>
        private void btDebug_Click(object sender, EventArgs e)
        {
            if(null == dgvDevs.SelectedRows || 0 == dgvDevs.SelectedRows.Count)
            {
                MessageBox.Show("请先选择需要调试的" + InitorCaption + "对象");
                return;
            }
            IJFInitializable initor = JFHubCenter.Instance.InitorManager[dgvDevs.SelectedRows[0].Cells[0].Value.ToString()];
            Type devType = initor.GetType();
            if (chkSelfUI.Checked)
            {
                if (!typeof(IJFRealtimeUIProvider).IsAssignableFrom(devType))
                {
                    MessageBox.Show(InitorCaption + "对象未提供自带调试界面，请取消\"自带界面\"后重试！");
                    return;
                }

                if(typeof(IJFStation).IsAssignableFrom(devType))
                {
                    Form fmStationDebug = (initor as IJFStation).GenForm();
                    if (null == fmStationDebug) //工站自身未提供界面
                    {
                        if (DialogResult.Cancel == MessageBox.Show("工站对象未提供调试界面,是否使用通用界面？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                            return;
                        if (typeof(JFStationBase).IsAssignableFrom(devType))
                        {
                            FormStationBaseDebug fmSB = new FormStationBaseDebug();
                            fmSB.Text = (initor as JFStationBase).Name + " Debug";
                            fmSB.SetStation(initor as JFStationBase);
                            fmSB.TopMost = true;
                            fmSB.Show();
                            return;
                        }
                        else
                        {
                            //JFRealtimeUI ucST = (initor as IJFRealtimeUIProvider).GetRealtimeUI();
                            //ucST.Dock = DockStyle.Fill;
                            UcStationRealtimeUI ucST = new UcStationRealtimeUI();
                            ucST.Dock = DockStyle.Fill;
                            ucST.AllowedStartStopCmd = true;
                            ucST.SetStation(initor as IJFStation);
                            FormJFRealTimeUI fmST = new FormJFRealTimeUI();
                            fmST.Text = dgvDevs.SelectedRows[0].Cells[0].Value.ToString() + " Debug";
                            fmST.Size = ucST.Size;
                            fmST.SetRTUI(ucST);
                            //fm.Controls.Add(uc);
                            fmST.Show();
                            return;
                        }
                    }
                    else
                    {
                        fmStationDebug.FormBorderStyle = FormBorderStyle.Sizable;
                        fmStationDebug.TopMost = true;
                        fmStationDebug.Show();
                        return;
                    }
                }

                JFRealtimeUI uc = (initor as IJFRealtimeUIProvider).GetRealtimeUI();
                uc.Dock = DockStyle.Fill;
                FormJFRealTimeUI fm = new FormJFRealTimeUI();
                fm.Text = dgvDevs.SelectedRows[0].Cells[0].Value.ToString() + " Debug";
                fm.Size = uc.Size;
                fm.SetRTUI(uc);
                //fm.Controls.Add(uc);
                fm.Show();
            }
            else
            {
                if (typeof(IJFDevice_Camera).IsAssignableFrom(devType) ||
                    typeof(IJFDevice_LightController).IsAssignableFrom(devType) ||
                    typeof(IJFDevice_MotionDaq).IsAssignableFrom(devType) ||
                    typeof(IJFDevice_TrigController).IsAssignableFrom(devType)||
                    typeof(IJFStation).IsAssignableFrom(devType) )
                {
                    if(typeof(JFStationBase).IsAssignableFrom(devType))
                    {
                        FormStationBaseDebug fmSB = new FormStationBaseDebug();
                        fmSB.Text = (initor as JFStationBase).Name + " Debug";
                        fmSB.SetStation(initor as JFStationBase);
                        fmSB.Show();
                        return;
                    }
                    if(typeof(IJFStation).IsAssignableFrom(devType))
                    {
                        UcStationRealtimeUI ucST = new UcStationRealtimeUI();
                        ucST.Dock = DockStyle.Fill;
                        ucST.AllowedStartStopCmd = true;
                        ucST.SetStation(initor as IJFStation);
                        FormJFRealTimeUI fmST = new FormJFRealTimeUI();
                        fmST.Text = dgvDevs.SelectedRows[0].Cells[0].Value.ToString() + " Debug";
                        fmST.Size = ucST.Size;
                        fmST.SetRTUI(ucST);
                        //fm.Controls.Add(uc);
                        fmST.Show();
                        return;
                    }


                    FormJFRealTimeUI fm = new FormJFRealTimeUI();
                    fm.Text = dgvDevs.SelectedRows[0].Cells[0].Value.ToString() + " Debug" + "待实现！！";
                    if (typeof(IJFDevice_MotionDaq).IsAssignableFrom(devType))
                    {
                        UcMotionDaq uc = new UcMotionDaq();
                        fm.Text = dgvDevs.SelectedRows[0].Cells[0].Value.ToString() + "调试窗口";
                        //uc.Parent = fm;
                        uc.SetDevice(initor as IJFDevice_MotionDaq, dgvDevs.SelectedRows[0].Cells[0].Value.ToString());
                        //fm.Size = new Size(uc.Width + 20, uc.Height + 20);
                        //fm.Controls.Add(uc);
                        fm.SetRTUI(uc);
                    }
                    else if(typeof(IJFDevice_TrigController).IsAssignableFrom(devType))//HTM触发板控制器，一进多出（控制触发强度/时间）
                    {
                        fm.Text = dgvDevs.SelectedRows[0].Cells[0].Value.ToString() + "调试窗口";
                        if (typeof(IJFDevice_LightController).IsAssignableFrom(devType))//带触发功能的光源控制器
                        {
                            UcLightCtrl_T uc = new UcLightCtrl_T();
                            uc.SetModule(initor as IJFDevice_LightControllerWithTrig, null, null);
                            uc.Dock = DockStyle.Fill;
                            fm.SetRTUI(uc);
                        } 
                        else //单纯的触发控制器
                        {
                            UcTrigCtrl uc = new UcTrigCtrl();
                            uc.SetModuleInfo(initor as IJFDevice_TrigController);
                            fm.SetRTUI(uc);
                        }
                    }
                    else if(typeof(IJFDevice_LightController).IsAssignableFrom(devType)) //光源控制器
                    {
                        fm.Text = dgvDevs.SelectedRows[0].Cells[0].Value.ToString() + "调试窗口";
                        UcLightCtrl uc = new UcLightCtrl();
                        uc.SetModuleInfo(initor as IJFDevice_LightController);
                        fm.SetRTUI(uc);
                    }
                    else if(typeof(IJFDevice_Camera).IsAssignableFrom(devType))
                    {
                        fm.Text = dgvDevs.SelectedRows[0].Cells[0].Value.ToString() + "调试窗口";
                        UcCmr uc = new UcCmr();
                        uc.SetCamera(initor as IJFDevice_Camera);
                        fm.SetRTUI(uc);
                    }
                    fm.Show();
                }
                else
                {
                    MessageBox.Show("无法为"+InitorCaption + "类型的对象提供调试界面");
                    return;
                }

            }
        }

        /// <summary>初始化设备</summary>
        private void btInit_Click(object sender, EventArgs e)
        {
            IJFInitializable initor = JFHubCenter.Instance.InitorManager[dgvDevs.SelectedRows[0].Cells[0].Value.ToString()];
            if(!initor.Initialize())
                MessageBox.Show("初始化失败:" + initor.GetInitErrorInfo());

        }

        bool isEditting; ///是否处于编辑初始化参数状态
        /// <summary>编辑/保存 初始化参数</summary>
        private void btEditSave_Click(object sender, EventArgs e)
        {
            if(!isEditting)//参数未编辑状态
            {
                if(dgvDevs.SelectedRows == null || 0 == dgvDevs.SelectedRows.Count)
                {
                    MessageBox.Show("请选择需要编辑参数的对象");
                    return;
                }

                if(gbParams.Controls.Count <=5) //没有初始化参数
                {
                    return;
                }

                for(int i = 5; i< gbParams.Controls.Count;i++)
                {
                    UcJFParamEdit pe = gbParams.Controls[i] as UcJFParamEdit;
                    pe.IsValueReadOnly = false;
                }

                dgvDevs.Enabled = false;
                btEditSave.Text = "保存";
                btCancel.Enabled = true;
                btInit.Enabled = false;
                btAdd.Enabled = false;
                btRemove.Enabled = false;
                btDebug.Enabled = false;
                isEditting = true;
            }
            else //编辑完成，需要存储编辑后的参数
            {
                IJFInitializable dev = JFHubCenter.Instance.InitorManager[dgvDevs.SelectedRows[0].Cells[0].Value.ToString()];
                List<object> initParams = new List<object>();
                initParams.Add(dev.GetType().AssemblyQualifiedName);//DevType
                for (int i = 5; i < gbParams.Controls.Count; i++)
                {
                    object paramVal = null;
                    UcJFParamEdit pe = gbParams.Controls[i] as UcJFParamEdit;
                    if(!pe.GetParamValue(out paramVal))
                    {
                        pe.Focus();
                        MessageBox.Show("参数\"" + dev.InitParamNames[i - 5] + "\"格式错误：" + pe.GetParamErrorInfo());
                        return;
                    }
                    if(!dev.SetInitParamValue(dev.InitParamNames[i - 5], paramVal))
                    {
                        pe.Focus();
                        MessageBox.Show("设置参数\"" + dev.InitParamNames[i - 5] + "\"失败：" + dev.GetInitErrorInfo() );
                        return;
                    }
                    //initParams.Add(paramVal); //替换为以下代码，适应数组/基类型混编的参数列表



                        //paramsInCfg.Add(dev.GetInitParamValue(dev.InitParamNames[i])); 本行代码用以下代码代替，以支持有限的 简单类型 + 数组（列表）类型的组合 
                        //暂时的解决方法：将数组类型转化为字符串存储
                        Type paramType = dev.GetInitParamDescribe(dev.InitParamNames[i-5]).ParamType;
                        SerializableAttribute[] sas = paramType.GetCustomAttributes(typeof(SerializableAttribute), false) as SerializableAttribute[];
                        if (sas != null && sas.Length > 0) //如果是可序列化的类型，直接保存序列化后的文本
                        {
                            StringBuilder buffer = new StringBuilder();
                            XmlSerializer serializer = new XmlSerializer(paramType);
                            using (TextWriter writer = new StringWriter(buffer))
                            {
                                serializer.Serialize(writer, paramVal);
                            }
                            string xmlTxt = buffer.ToString();
                        initParams.Add(xmlTxt); // paramsInCfg.Add(xmlTxt);
                        }
                        else
                        {
                            if (paramType.IsValueType || paramType == typeof(string)) //单值对象和字符串对象直接添加
                            initParams.Add(paramVal);
                            else //目前支持Array 和 List
                            {
                                if (paramType.IsArray) //参数类型是数组
                                {
                                    if (null == paramVal)
                                    {
                                    initParams.Add("");
                                    }
                                    else
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        string splitString = "$";
                                        for (int j = 0; j < (paramVal as Array).Length; j++)
                                        {
                                            sb.Append((paramVal as Array).GetValue(j).ToString());
                                            if (j < (paramVal as Array).Length - 1)
                                                sb.Append(splitString);
                                        }
                                    initParams.Add(sb.ToString());//paramsInCfg.Add(string.Join(splitString, paramVal)); //使用ASC 响铃符作为间隔
                                    }
                                }
                                else if (typeof(IList).IsAssignableFrom(paramType)) //除了数组之外的队列类型
                                {
                                    if (null == paramVal)
                                    {
                                    initParams.Add("");
                                    }
                                    else
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        string splitString = "$";
                                        for (int j = 0; j < (paramVal as IList).Count; j++)
                                        {
                                            sb.Append((paramVal as IList)[j].ToString());
                                            if (j < (paramVal as IList).Count - 1)
                                                sb.Append(splitString);
                                        }
                                    initParams.Add(sb.ToString());
                                        //paramsInCfg.Add(string.Join(System.Text.Encoding.ASCII.GetString(new byte[] { 0x07 }), paramVal)); //使用ASC 响铃符作为间隔
                                    }
                                }
                            }

                        }

                    





                }
                for (int i = 5; i < gbParams.Controls.Count; i++)
                {
                    UcJFParamEdit pe = gbParams.Controls[i] as UcJFParamEdit;
                    pe.IsValueReadOnly = true;
                }
                if (!dev.Initialize())
                {
                    MessageBox.Show("用当前参数初始化设备失败:" + dev.GetInitErrorInfo());
                }
                JFXmlSortedDictionary<string,List<object>> devCfg = JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_InitDevParams) as JFXmlSortedDictionary<string, List<object>>;
                devCfg[dgvDevs.SelectedRows[0].Cells[0].Value.ToString()] = initParams;
                JFHubCenter.Instance.SystemCfg.NotifyItemChanged(JFHubCenter.CK_InitDevParams);
                JFHubCenter.Instance.SystemCfg.Save();

                btInit.Enabled = true;
                btAdd.Enabled = true;
                btRemove.Enabled = true;
                //btDebug.Enabled = true;

                isEditting = false;
                btEditSave.Text = "编辑参数";
                btCancel.Enabled = false;
                dgvDevs.Enabled = true;


                Type devType = dev.GetType();
                if (typeof(IJFDevice_Camera).IsAssignableFrom(devType) ||
                    typeof(IJFDevice_LightController).IsAssignableFrom(devType) ||
                    typeof(IJFDevice_MotionDaq).IsAssignableFrom(devType) ||
                    typeof(IJFDevice_TrigController).IsAssignableFrom(devType) ||
                    typeof(IJFRealtimeUIProvider).IsAssignableFrom(devType)) //提供调试界面
                {
                    btDebug.Enabled = true;
                    if (typeof(IJFRealtimeUIProvider).IsAssignableFrom(devType))
                        chkSelfUI.Enabled = true;
                    else
                    {
                        chkSelfUI.Checked = false;
                        chkSelfUI.Enabled = false;
                    }
                }
                else
                    btDebug.Enabled = false;

                if (typeof(IJFConfigUIProvider).IsAssignableFrom(devType))
                    btCfg.Enabled = true;
                else
                    btCfg.Enabled = false;
            }
        }

        /// <summary>取消编辑，将配置中的参数恢复到界面和对象中</summary>
        private void btCancel_Click(object sender, EventArgs e)
        {
            IJFInitializable dev = JFHubCenter.Instance.InitorManager[dgvDevs.SelectedRows[0].Cells[0].Value.ToString()];
            JFXmlSortedDictionary<string, List<object>> devCfg = JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_InitDevParams) as JFXmlSortedDictionary<string, List<object>>;
            List<object> initParams = devCfg[dgvDevs.SelectedRows[0].Cells[0].Value.ToString()];
            for (int i = 1; i < initParams.Count; i++)
            {
                UcJFParamEdit pe = gbParams.Controls[i+4] as UcJFParamEdit;
                pe.SetParamValue(initParams[i]);
                pe.IsValueReadOnly = true;
                dev.SetInitParamValue(dev.InitParamNames[i - 1], initParams[i]);
                
            }
            btInit.Enabled = !dev.IsInitOK;
            btAdd.Enabled = true;
            btRemove.Enabled = true;
            //btDebug.Enabled = true;
            isEditting = false;
            btEditSave.Text = "编辑参数";
            btCancel.Enabled = false;
            dgvDevs.Enabled = true;

            Type devType = dev.GetType();
            if (typeof(IJFDevice_Camera).IsAssignableFrom(devType) ||
                typeof(IJFDevice_LightController).IsAssignableFrom(devType) ||
                typeof(IJFDevice_MotionDaq).IsAssignableFrom(devType) ||
                typeof(IJFDevice_TrigController).IsAssignableFrom(devType) ||
                typeof(IJFRealtimeUIProvider).IsAssignableFrom(devType)) //提供调试界面
            {
                btDebug.Enabled = true;
                if (typeof(IJFRealtimeUIProvider).IsAssignableFrom(devType))
                    chkSelfUI.Enabled = true;
                else
                {
                    chkSelfUI.Checked = false;
                    chkSelfUI.Enabled = false;
                }
            }
            else
                btDebug.Enabled = false;

            if (typeof(IJFConfigUIProvider).IsAssignableFrom(devType))
                btCfg.Enabled = true;
            else
                btCfg.Enabled = false;
        }

        /// <summary>移除GroupBox中的参数编辑控件</summary>
        void RemoveAllPEs()
        {
            while (gbParams.Controls.Count > 5)
                gbParams.Controls.RemoveAt(5);
        }



        /// <summary>选择设备</summary>
        private void dgvDevs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDevs.SelectedRows.Count == 0)
                return;
            btRemove.Enabled = true;
            RemoveAllPEs();

            DataGridViewRow row = dgvDevs.SelectedRows[0];
            IJFInitializable dev = JFHubCenter.Instance.InitorManager[row.Cells[0].Value.ToString()];
            Type devType = dev.GetType();
            if (typeof(IJFDevice_Camera).IsAssignableFrom(devType) ||
                typeof(IJFDevice_LightController).IsAssignableFrom(devType) ||
                typeof(IJFDevice_MotionDaq).IsAssignableFrom(devType) ||
                typeof(IJFDevice_TrigController).IsAssignableFrom(devType) ||
                typeof(IJFRealtimeUIProvider).IsAssignableFrom(devType)) //提供调试界面
            {
                btDebug.Enabled = true;
                if (typeof(IJFRealtimeUIProvider).IsAssignableFrom(devType))
                    chkSelfUI.Enabled = true;
                else
                {
                    chkSelfUI.Checked = false;
                    chkSelfUI.Enabled = false;
                }
            }
            else
                btDebug.Enabled = false;

            if (typeof(IJFConfigUIProvider).IsAssignableFrom(devType))
                btCfg.Enabled = true;
            else
                btCfg.Enabled = false;



            tbDevID.Text = row.Cells[0].Value.ToString();
            btInit.Enabled = !dev.IsInitOK;
            string[] iniParamNames = dev.InitParamNames;
            if (null == iniParamNames)
            {
                btEditSave.Enabled = false;
                return;
            }
            btEditSave.Enabled = true;
            int locY = btInit.Location.Y + btInit.Size.Height + 5;
            foreach(string ipName in iniParamNames)
            {
                UcJFParamEdit pe = new UcJFParamEdit();
                pe.Width = gbParams.Width - 1;
                pe.Location = new Point(4, locY);
                pe.SetParamDesribe(dev.GetInitParamDescribe(ipName));
                locY += pe.Height;
                pe.SetParamValue(dev.GetInitParamValue(ipName));
                pe.IsValueReadOnly = true;
                gbParams.Controls.Add(pe);
            }
        }

        private void btCfg_Click(object sender, EventArgs e)
        {
            if(dgvDevs.SelectedRows == null || dgvDevs.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择需要设置的" + InitorCaption + "对象！");
                return;
            }

            IJFInitializable initor = JFHubCenter.Instance.InitorManager[dgvDevs.SelectedRows[0].Cells[0].Value.ToString()];
            Type devType = initor.GetType();
            if (!typeof(IJFConfigUIProvider).IsAssignableFrom(devType))
                {
                    MessageBox.Show(InitorCaption + "对象未提供参数配置界面！");
                    return;
                }

                (initor as IJFConfigUIProvider).ShowCfgDialog();
            }
    }
}
