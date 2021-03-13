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
using JFUI;
using System.Threading;
using HalconDotNet;

namespace JFHub
{
    /// <summary>
    /// UcSingleVisionAssist  单视野视觉配置参数示教界面
    /// </summary>
    public partial class UcSingleVisionAssist : JFRealtimeUI//UserControl
    {
        public UcSingleVisionAssist()
        {
            InitializeComponent();
        }


        public override  void UpdateSrc2UI() 
        {

        }

        bool _isFormLoaded = false;
        JFSingleVisionAssist _sva = null;
        JFSingleVisionCfgParam _svCfg = null; //当前所选的预设配置对象

        IJFDevice_Camera _cmr = null; //相机设备
        bool _isAutoSnapping = false;


        UcStandard4AxisPanel _pnAxis4 = new UcStandard4AxisPanel(); //标准四轴操作面板
        List<UcSimpleAxisInStation> _lstExtendTeachAxis = new List<UcSimpleAxisInStation>(); //其他拓展的示教轴
        List<UcLightCtrlChn> _lstLightChn = new List<UcLightCtrlChn>(); //所有光源控制器通道面板


        JFMethodFlow _testMethodFlow = new JFMethodFlow(); //用于测试当前图像的方法流对象（会转化成文本保存到配置数据中）
        



        #region 是否主动更改控件值
        bool _isSettingChkReversX = false; //修改相机ReversX
        bool _isSettingChkReversY = false;
        bool _isSettingExposure = false;
        bool _isSettingGain = false;
        bool _isSettingContinueSnap = false;
        #endregion





        int _maxTips = 500;
        delegate void dgShowTips(string info);
        void ShowTips(string info)
        {
            if (!Created)
                return;
            if (InvokeRequired)
            {
                Invoke(new dgShowTips(ShowTips), new object[] { info });
                return;
            }
            rchTips.AppendText(info + "\n");
            string[] lines = rchTips.Text.Split('\n');
            if (lines.Length >= _maxTips)
            {
                int rmvChars = 0;
                for (int i = 0; i < lines.Length - _maxTips; i++)
                    rmvChars += lines[i].Length + 1;
                rchTips.Text = rchTips.Text.Substring(rmvChars);
            }
            rchTips.Select(rchTips.TextLength, 0); //滚到最后一行
            rchTips.ScrollToCaret();//滚动到控件光标处 
        }

        


        private void UcSingleVisionAssist_Load(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            AdjustAssistView();
        }

        /// <summary>
        /// 布置界面
        /// </summary>
        void AdjustAssistView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(AdjustAssistView));
                return;
            }

            if(null == _sva)
            {
                lbAssistInfo.Text = "示教助手:未设置    配置名:未命名";
                Enabled = false;
                return;
            }
            Enabled = true;
            if(!_sva.IsInitOK) // 对象未完成初始化
            {
                ShowTips("对象初始化未完成,错误信息:" + _sva.GetInitErrorInfo());
                btInitialize.Enabled = true;
                /* 日后添加其他控件的使能/禁用功能
                gbCmrCfg.Enabled = false;
                */
                return;
            }
            PnDevs.Controls.Clear();
            btInitialize.Enabled = false;
            lbAssistInfo.Text = "示教助手:" +_sva.Name + " 相机:" +(string.IsNullOrEmpty(_sva.CameraName)?"未设置": _sva.CameraName) +  "    配置:未选择";

            ///检查相机是否可用
            if(string.IsNullOrEmpty(_sva.CameraName))
            {
                ShowTips("示教助手:相机未设置/名称为空字串");
                gbCmrCfg.Enabled = false;
                _cmr = null;
            }
            else
            {
                string[] allCmrNames = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_Camera));
                if(null == allCmrNames || allCmrNames.Length == 0)
                {
                    ShowTips("示教助手相机名称:\"" + _sva.CameraName + "\" 在设备表中不存在（设备表中无相机设备）");
                    gbCmrCfg.Enabled = false;
                    _cmr = null;
                }
                else
                {
                    if(!allCmrNames.ToList().Contains(_sva.CameraName))
                    {
                        ShowTips("示教助手相机名称:\"" + _sva.CameraName + "\" 在设备表中不存在");
                        gbCmrCfg.Enabled = false;
                        _cmr = null;
                    }
                    _cmr = JFHubCenter.Instance.InitorManager.GetInitor(_sva.CameraName) as IJFDevice_Camera;
                    if (!_cmr.IsDeviceOpen)
                    {
                        gbCmrCfg.Enabled = false;
                        ShowTips("相机:\"" + _sva.CameraName + "\" 当前状态 :未打开");
                    }
                    else //获取相机参数显示在界面上
                    {
                        gbCmrCfg.Enabled = true;
                        ShowTips("相机:\"" + _sva.CameraName + "\"   已打开");
                    }
                }
            }

            ///检查光源通道
            string[] lightChnNames = _sva.LightChnNames;
            if(null != lightChnNames && lightChnNames.Length > 0)
            {
                for (int i = 0 ; i < lightChnNames.Length;i++)
                {
                    UcLightCtrlChn uclc = new UcLightCtrlChn();
                    string lightChnName = lightChnNames[i];
                    if(string.IsNullOrEmpty(lightChnName))
                    {
                        ShowTips("第" + i + "光源通道名称未设置/空字串");
                        PnDevs.Controls.Add(uclc);
                        continue;
                    }

                    JFDevCellInfo devci = JFHubCenter.Instance.MDCellNameMgr.GetLightCtrlChannelInfo(lightChnName);
                    if(devci == null)
                    {
                        ShowTips("第" + i + "光源通道名称 = \"" +lightChnName + "\"在设备名称表中不存在");
                        uclc.SetChannelInfo(null, 0, lightChnName);
                        PnDevs.Controls.Add(uclc);
                        continue;
                    }

                    IJFDevice_LightController dev = JFHubCenter.Instance.InitorManager.GetInitor(devci.DeviceID) as IJFDevice_LightController;
                    if(dev == null)
                    {
                        ShowTips("第" + i + "光源通道名称 = \"" + lightChnName + "\"所属设备:\"" + devci.DeviceID + "在设备表中不存在");
                        uclc.SetChannelInfo(null, devci.ChannelIndex, lightChnName);
                        PnDevs.Controls.Add(uclc);
                        continue;
                    }

                    uclc.SetChannelInfo(dev, devci.ChannelIndex, lightChnName);
                    PnDevs.Controls.Add(uclc);
                    _lstLightChn.Add(uclc);
                }
            }

            //检查是否需要标准4轴示教
            if (_sva.ExistTeachAxis4())
            {
                //_pnAxis4 = new UcStandard4AxisPanel();
                _pnAxis4.Size = new Size(380, 200);
                _pnAxis4.EventShowInfo += ShowTips;
                _pnAxis4.SetAxisNames(_sva.TeachAxis4Names);
                if(!PnDevs.Controls.Contains(_pnAxis4))
                PnDevs.Controls.Add(_pnAxis4);
            }

            //拓展示教轴
            string[] extendTeachAxisNames = _sva.ExtendTeachAxisNames;
            if(null != extendTeachAxisNames && extendTeachAxisNames.Length > 0)
                for(int i = 0; i < extendTeachAxisNames.Length;i++)
                {
                    string axisName = extendTeachAxisNames[i];
                    UcSimpleAxisInStation ucAxis = new UcSimpleAxisInStation();
                    ucAxis.SetAxisName(axisName);
                    PnDevs.Controls.Add(ucAxis);
                    _lstExtendTeachAxis.Add(ucAxis);
                }

            //检查光源触发器通道
            string[] trigChnNames = _sva.TrigChnNames;
            if(null != trigChnNames && trigChnNames.Length > 0)
            {
                //日后添加光源触发器通道示教功能
            }


            cbAllProgramNames.Items.Clear();
            string[] allProgramNames = JFHubCenter.Instance.VisionMgr.SingleVisionCfgNameByOwner(_sva.Name); //由示教助手创建的所有配置项
            if (null != allProgramNames && allProgramNames.Length > 0)
            {
                foreach (string pn in allProgramNames)
                    cbAllProgramNames.Items.Add(pn);
                cbAllProgramNames.Enabled = true;
            }
            else
            {
                cbAllProgramNames.Enabled = false;

            }


            UpdateCfg2GridView();///将当前配置项显示在表格
            UpdateCmr2View();

        }
        
        /// <summary>
        /// 更新列表中的当前设备参数项(单项)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void UpdateDevParamInGrid(string name,string value)
        {
            foreach(DataGridViewRow row in dgvCfgParam.Rows)
                if((row.Cells[0].Value as string) == name)
                {
                    row.Cells[2].Value = value;
                    return;
                }
        }

        /// <summary>
        /// 将相机参数显示到界面上(包括列表)
        /// </summary>
        void UpdateCmr2View()
        {
            if(null == _cmr || !_cmr.IsDeviceOpen)
            {
                _isSettingChkReversX = true;
                chkReverseX.Checked = false;
                _isSettingChkReversX = false;
                UpdateDevParamInGrid("X轴镜像使能", "未知");

                _isSettingChkReversY = true;
                chkReverseY.Checked = false;
                _isSettingChkReversY = false;
                UpdateDevParamInGrid("Y轴镜像使能", "未知");

                _isSettingExposure = true;
                numExposure.Value = 0;
                _isSettingExposure = false;
                UpdateDevParamInGrid("相机曝光时长", "未知");

                _isSettingGain = true;
                numGain.Value = 0;
                _isSettingGain = false;
                UpdateDevParamInGrid("相机增益参数", "未知");


                _isSettingContinueSnap = true;
                chkContinueSnap.Checked = false;
                StopContinueSnap();
                _isSettingContinueSnap = false;
            }
            else
            {
                bool bVal = false;
                int errCode = _cmr.GetReverseX(out bVal);
                if(errCode != 0)
                {
                    UpdateDevParamInGrid("X轴镜像使能", "未知");
                    ShowTips(" 获取相机X镜像使能参数失败：" + _cmr.GetErrorInfo(errCode));
                }
                else
                    UpdateDevParamInGrid("X轴镜像使能", bVal?"是":"否");


                errCode = _cmr.GetReverseY(out bVal);
                if (errCode != 0)
                {
                    UpdateDevParamInGrid("Y轴镜像使能", "未知");
                    ShowTips(" 获取相机Y镜像使能参数失败：" + _cmr.GetErrorInfo(errCode));
                }
                else
                    UpdateDevParamInGrid("Y轴镜像使能", bVal ? "是" : "否");


                double dVal = 0;
                errCode = _cmr.GetExposure(out dVal);
                if(errCode != 0)
                {
                    UpdateDevParamInGrid("相机曝光时长", "未知");
                    ShowTips(" 获取相机曝光参数失败：" + _cmr.GetErrorInfo(errCode));
                }
                else
                    UpdateDevParamInGrid("相机曝光时长", dVal.ToString("F3"));


                errCode = _cmr.GetGain(out dVal);
                if (errCode != 0)
                {
                    UpdateDevParamInGrid("相机增益参数", "未知");
                    ShowTips(" 获取相机增益参数失败：" + _cmr.GetErrorInfo(errCode));
                }
                else
                    UpdateDevParamInGrid("相机增益参数", dVal.ToString("F3"));

            }
        }


        void UpdateCmr2Grid()
        {
            if (null == _cmr || !_cmr.IsDeviceOpen)
            {
                UpdateDevParamInGrid("X轴镜像使能", "未知");
                UpdateDevParamInGrid("Y轴镜像使能", "未知");
                UpdateDevParamInGrid("相机曝光时长", "未知");
                UpdateDevParamInGrid("相机增益参数", "未知");
            }
            else
            {
                bool bVal = false;
                int errCode = _cmr.GetReverseX(out bVal);
                if (errCode != 0)
                {
                    UpdateDevParamInGrid("X轴镜像使能", "未知");
                }
                else
                    UpdateDevParamInGrid("X轴镜像使能", bVal ? "是" : "否");


                errCode = _cmr.GetReverseY(out bVal);
                if (errCode != 0)
                {
                    UpdateDevParamInGrid("Y轴镜像使能", "未知");
                }
                else
                    UpdateDevParamInGrid("Y轴镜像使能", bVal ? "是" : "否");


                double dVal = 0;
                errCode = _cmr.GetExposure(out dVal);
                if (errCode != 0)
                {
                    UpdateDevParamInGrid("相机曝光时长", "未知");
                }
                else
                    UpdateDevParamInGrid("相机曝光时长", dVal.ToString("F3"));


                errCode = _cmr.GetGain(out dVal);
                if (errCode != 0)
                {
                    UpdateDevParamInGrid("相机增益参数", "未知");
                }
                else
                    UpdateDevParamInGrid("相机增益参数", dVal.ToString("F3"));

            }
        }

        /// <summary>
        /// 开始连续采图
        /// </summary>
        /// <returns></returns>
        bool StartContinueSnap()
        {
            return false;
        }

        void StopContinueSnap()
        {

        }




        /// <summary>
        /// 设置调试助手
        /// </summary>
        /// <param name="assist"></param>
        public void SetAssist(JFSingleVisionAssist assist)
        {
            _sva = assist;
            if (_isFormLoaded)
                AdjustAssistView();
        }


        /// <summary>
        /// Initor对象初始化/参数修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btInitialize_Click(object sender, EventArgs e)
        {
            if (_sva == null)
                return;


            bool isOK = _sva.Initialize();
            if(!isOK)
            {
                MessageBox.Show("视觉助手初始化失败：" + _sva.GetInitErrorInfo());
                return;
            }

            AdjustAssistView();

        }



        private void UcSingleVisionAssist_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                timerFlush.Enabled = true;
            }
            else
            {
                timerFlush.Enabled = false;
                _isAutoSnapping = false;
                _isSettingContinueSnap = true;
                chkContinueSnap.Checked = false;
                _isSettingContinueSnap = false;
                StopSnapWithCurrCfg();
                StopAutoSnap();
            }

        }

        /// <summary>
        /// 将从combo中所选的配置载入到界面上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btLoadProgram_Click(object sender, EventArgs e)
        {
            if(cbAllProgramNames.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择配置项！");
                return;
            }
            string pn = cbAllProgramNames.SelectedItem.ToString();
            JFSingleVisionCfgParam svCfg = JFHubCenter.Instance.VisionMgr.GetSingleVisionCfgByName(pn);
            if(svCfg == null)
            {
                MessageBox.Show("系统配置中不存在名称为 " + pn + " 的视觉配置参数项");
                return;
            }

           // _testMethodFlow.FromTxt()
            

            _svCfg = svCfg;
            _testMethodFlow.FromTxt(_svCfg.TestMethodFlowTxt);
            
            lbAssistInfo.Text = "示教助手:" + _sva.Name + "相机:" + _sva.CameraName + "    配置名:" + pn;// == _svCfg.Name;
            UpdateCfg2GridView();

        }

        /// <summary>
        /// 将视觉配置参数更新到表格中
        /// </summary>
        void UpdateCfg2GridView()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(UpdateCfg2GridView));
                return;
            }
            dgvCfgParam.Rows.Clear();
            if (null == _svCfg)
                return;
            DataGridViewRow rowRevsX = new DataGridViewRow();
            DataGridViewTextBoxCell cn_RevsX = new DataGridViewTextBoxCell();
            cn_RevsX.Value = "X轴镜像使能";
            rowRevsX.Cells.Add(cn_RevsX);
            DataGridViewTextBoxCell cv_CfgRevsX = new DataGridViewTextBoxCell();
            cv_CfgRevsX.Value = _svCfg.CmrReverseX ? "是" : "否";
            rowRevsX.Cells.Add(cv_CfgRevsX);
            DataGridViewTextBoxCell cv_CurrRevsX = new DataGridViewTextBoxCell();
            rowRevsX.Cells.Add(cv_CurrRevsX);
            dgvCfgParam.Rows.Add(rowRevsX);

            DataGridViewRow rowRevsY = new DataGridViewRow();
            DataGridViewTextBoxCell cn_RevsY = new DataGridViewTextBoxCell();
            cn_RevsY.Value = "Y轴镜像使能";
            rowRevsY.Cells.Add(cn_RevsY);
            DataGridViewTextBoxCell cv_CfgRevsY = new DataGridViewTextBoxCell();
            cv_CfgRevsY.Value = _svCfg.CmrReverseY ? "是" : "否";
            rowRevsY.Cells.Add(cv_CfgRevsY);
            DataGridViewTextBoxCell cv_CurrRevsY = new DataGridViewTextBoxCell();
            rowRevsY.Cells.Add(cv_CurrRevsY);
            dgvCfgParam.Rows.Add(rowRevsY);


            DataGridViewRow rowExposure = new DataGridViewRow();
            DataGridViewTextBoxCell cn_Exposure = new DataGridViewTextBoxCell();
            cn_Exposure.Value = "相机曝光时长";
            rowExposure.Cells.Add(cn_Exposure);
            DataGridViewTextBoxCell cv_CfgExposure = new DataGridViewTextBoxCell();
            cv_CfgExposure.Value = _svCfg.CmrExposure.ToString();
            rowExposure.Cells.Add(cv_CfgExposure);
            DataGridViewTextBoxCell cv_CurrExposure = new DataGridViewTextBoxCell();
            rowExposure.Cells.Add(cv_CurrExposure);
            dgvCfgParam.Rows.Add(rowExposure);


            DataGridViewRow rowGain = new DataGridViewRow();
            DataGridViewTextBoxCell cn_Gain = new DataGridViewTextBoxCell();
            cn_Gain.Value = "相机增益参数";
            rowGain.Cells.Add(cn_Gain);
            DataGridViewTextBoxCell cv_CfgGain = new DataGridViewTextBoxCell();
            cv_CfgGain.Value = _svCfg.CmrGain.ToString();
            rowGain.Cells.Add(cv_CfgGain);
            DataGridViewTextBoxCell cv_CurrGain = new DataGridViewTextBoxCell();
            rowGain.Cells.Add(cv_CurrGain);
            dgvCfgParam.Rows.Add(rowGain);


            string[] lightNames = _svCfg.LightChnNames; //参与配置的光源通道选项
            if(null != lightNames && lightNames.Length > 0)
                for(int i = 0; i < lightNames.Length;i++)//foreach(string ln in lightNames)
                {
                    string ln = lightNames[i];
                    DataGridViewRow rowLight = new DataGridViewRow();
                    DataGridViewTextBoxCell cn_Light = new DataGridViewTextBoxCell();
                    cn_Light.Value = ln;
                    rowLight.Cells.Add(cn_Light);
                    DataGridViewTextBoxCell cv_CfgLight = new DataGridViewTextBoxCell();
                    cv_CfgLight.Value = _svCfg.LightIntensities[i].ToString();
                    rowLight.Cells.Add(cv_CfgLight);
                    DataGridViewTextBoxCell cv_CurrLight = new DataGridViewTextBoxCell();
                    rowLight.Cells.Add(cv_CurrLight);
                    dgvCfgParam.Rows.Add(rowLight);
                }

            string[] trigNames = _svCfg.TrigChnNames;
            if(null != trigNames && trigNames.Length > 0)
            {
                //触发控制器的功能日后再添加，暂时用不到 ... 
            }

            string[] axisNames = _svCfg.AxisNames;
            if (axisNames != null && axisNames.Length > 0) //参与配置项的轴
                for(int i = 0; i < axisNames.Length;i++)
                {
                    string an = axisNames[i];
                    DataGridViewRow rowAxis = new DataGridViewRow();
                    DataGridViewTextBoxCell cn_Axis = new DataGridViewTextBoxCell();
                    cn_Axis.Value = an;
                    rowAxis.Cells.Add(cn_Axis);
                    DataGridViewTextBoxCell cv_CfgAxis = new DataGridViewTextBoxCell();
                    cv_CfgAxis.Value = _svCfg.AxisPositions[i].ToString();
                    rowAxis.Cells.Add(cv_CfgAxis);
                    DataGridViewTextBoxCell cv_CurrAxis = new DataGridViewTextBoxCell();
                    rowAxis.Cells.Add(cv_CurrAxis);
                    dgvCfgParam.Rows.Add(rowAxis);
                }
            
            UpdateCurrDev2GridView();
        }

        /// <summary>
        /// 更新各轴控制面板
        /// </summary>
        void UpdateAxisPanel()
        {
            _pnAxis4.UpdateStationUI(); //标准四轴操作面板
            foreach (UcSimpleAxisInStation sa in _lstExtendTeachAxis)//其他拓展的示教轴
                sa.UpdateAxisUI();
        }


        void UpdateAxis2Grid()
        {
            if (_sva == null)
                return;
            JFDevChannel[] axes = _sva.ConfigAxes;
            if(null != axes && axes.Length > 0)
            {
                int errCode = 0;
                double pos = 0;
                foreach(JFDevChannel axis in axes)
                {
                    string errInfo;
                    if (!axis.CheckAvalid(out errInfo))
                        continue;
                    JFDevCellInfo ci = axis.CellInfo();
                    IJFModule_Motion md = (axis.Device() as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
                    errCode = md.GetFbkPos(ci.ChannelIndex, out pos);
                    if (errCode != 0)
                        UpdateDevParamInGrid(axis.Name, "未知");//UpdateCfg2GridView(axis.Name,)
                    else
                        UpdateDevParamInGrid(axis.Name, pos.ToString());
                }
            }
        }

        /// <summary>
        /// 将光源设备参数更新到数据表中
        /// </summary>
        void UpdateLight2Grid()
        {
            if (_sva == null)
                return;
            if (_svCfg == null)
                return;
            string errorInfo = null;
            JFDevChannel[] lightChns = _sva.LightChns;
            if(null != lightChns && lightChns.Length > 0)
            {
                foreach(JFDevChannel lightChn in lightChns)
                {
                    if (lightChn.CheckAvalid(out errorInfo))
                    {
                        int intensity = 0;
                        if(0!=(lightChn.Device() as IJFDevice_LightController).GetLightIntensity(lightChn.CellInfo().ChannelIndex, out intensity))
                            UpdateDevParamInGrid(lightChn.Name, "未知");
                        else
                            UpdateDevParamInGrid(lightChn.Name, intensity.ToString());
                    }
                    else
                        UpdateDevParamInGrid(lightChn.Name, "未知");

                }
            }
        }

        void UpdateCfgAxis2Grid() //将配置轴参数更新到表格中
        {

        }




        /// <summary>
        /// 将设备当前参数更新到显示列表中
        /// </summary>
        void UpdateCurrDev2GridView()
        {
            UpdateCmr2Grid();
            
        }

        bool _isSnapWithCurrCfgRunning = false;
        //Task _taskSnapWithCurrCfg = null; 
        Thread _threadSnapWithCurrCfg = null;//使用预设参数拍照的工作线程


        void ThreadFuncSnapWithCurrCfg()
        {
                //等待轴调整到预设位置后拍照
                ShowTips("等待配置轴移动到预设位置");
                Thread.Sleep(100);
                JFDevChannel[] axes = _sva.ConfigAxes;
            while (_isSnapWithCurrCfgRunning)
            {
                bool isAllInp = true;
                for (int i = 0; i < axes.Length; i++)
                {
                    JFDevChannel axis = axes[i];
                    IJFModule_Motion md = (axis.Device() as IJFDevice_MotionDaq).GetMc(axis.CellInfo().ModuleIndex);
                    int axisIndex = axis.CellInfo().ChannelIndex;
                    if (!md.IsINP(axisIndex))
                        isAllInp = false;
                }
                if (isAllInp)
                    break;
            }
            if(_isSnapWithCurrCfgRunning)
            {
                if(_isAutoSnapping)
                {
                    ShowTips("当前为自动拍照模式，设备参数已调整为预设值");
                    _isSnapWithCurrCfgRunning = false;
                    return;
                }
                else
                {
                    if (null != _cmr)
                    {
                        IJFImage img = null;
                        int errCode = _cmr.GrabOne(out img, 1000);
                        if (errCode != 0)
                            MessageBox.Show("相机拍照失败：" + _cmr.GetErrorInfo(errCode));
                        else
                        {
                            ShowTips("相机拍照完成");
                            ShowImage(img);//ucImage.LoadImage(img);
                        }
                    }
                }
            }
            _isSnapWithCurrCfgRunning = false;
        }

        void StopSnapWithCurrCfg()
        {
            if (null == _threadSnapWithCurrCfg)
                return;
            if (_isSnapWithCurrCfgRunning)
                return;
            _isSnapWithCurrCfgRunning = false;
            if (!_threadSnapWithCurrCfg.Join(1000))
                _threadSnapWithCurrCfg.Abort();

        }



        /// <summary>
        /// 使用已载入的预设参数拍照
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSnapUseCfg_Click(object sender, EventArgs e)
        {
            if(_svCfg == null)
            {
                MessageBox.Show("请先选择参数配置项");
                return;
            }

            if(_threadSnapWithCurrCfg != null && _isSnapWithCurrCfgRunning)
            {
                if(DialogResult.OK == MessageBox.Show("当前拍照动作未完成,是否取消当前拍照"))
                {
                    StopSnapWithCurrCfg();
                    return;
                }
            }
            string errInfo;
            if(!_sva.OpenEnableDevices(out errInfo))
            {
                MessageBox.Show("操作失败，未能打开并使能示教器所有设备通道，ErrorInfo:" + errInfo);
                return;
            }

            DateTime startTime = DateTime.Now;
            int errCode = 0;
            //将相机调整为预设参数
            if (null != _cmr)
            {
                bool currRevX = false;
                _cmr.GetReverseX(out currRevX);
                if (currRevX != _svCfg.CmrReverseX)
                {
                    errCode = _cmr.SetReverseX(_svCfg.CmrReverseX);
                    if (errCode != 0)
                    {
                        MessageBox.Show("设置相机X轴镜像 = " + _svCfg.CmrReverseX + " 失败:" + _cmr.GetErrorInfo(errCode));
                        return;
                    }
                }

                bool currRevY = false;

                _cmr.GetReverseY(out currRevY);
                if (currRevY != _svCfg.CmrReverseY)
                {
                    errCode = _cmr.SetReverseY(_svCfg.CmrReverseY);
                    if (errCode != 0)
                    {
                        MessageBox.Show("设置相机Y轴镜像 = " + _svCfg.CmrReverseY + " 失败:" + _cmr.GetErrorInfo(errCode));
                        return;
                    }
                }

                errCode = _cmr.SetExposure(_svCfg.CmrExposure);
                if (errCode != 0)
                {
                    MessageBox.Show("设置相机曝光值 = " + _svCfg.CmrExposure + " 失败:" + _cmr.GetErrorInfo(errCode));
                    return;
                }

                errCode = _cmr.SetGain(_svCfg.CmrGain);
                if (errCode != 0)
                {
                    MessageBox.Show("设置相机增益值 = " + _svCfg.CmrGain + " 失败:" + _cmr.GetErrorInfo(errCode));
                    return;
                }
            }

            JFDevChannel[] lightChns = _sva.LightChns;
            //将光源亮度调整为预设值
            if(null != lightChns && lightChns.Length > 0)
                for(int i = 0; i < lightChns.Length;i++)//(JFDevChannel lightChn in lightChns)
                {
                    JFDevChannel lightChn = lightChns[i];
                    errCode = (lightChn.Device() as IJFDevice_LightController).SetLightIntensity(lightChn.CellInfo().ChannelIndex, _svCfg.LightIntensities[i]);
                    if(errCode!= 0)
                    {
                        MessageBox.Show("设置光源: " + lightChn.Name + " 亮度值" + _svCfg.LightIntensities[i] + " 失败:" + _cmr.GetErrorInfo(errCode));
                        return;
                    }
                }

            JFDevChannel[] cfgAxes = _sva.ConfigAxes;
            bool isNeedWaitAxesINP = false;//是否需要运动配置轴
            ///移动配置轴到预设位置
            if (null != cfgAxes && cfgAxes.Length > 0)
                for(int i = 0; i < cfgAxes.Length;i++)
                {
                    JFDevChannel axis = cfgAxes[i];
                    IJFModule_Motion md = (axis.Device() as IJFDevice_MotionDaq).GetMc(axis.CellInfo().ModuleIndex);
                    int axisIndex = axis.CellInfo().ChannelIndex;
                    double currPos = 0;//轴当前位置
                    errCode = md.GetFbkPos(axisIndex, out currPos);
                    if(errCode!=0)
                    {
                        MessageBox.Show("获取轴:" + axis.Name + " 当前位置失败:" + md.GetErrorInfo(errCode));
                        return;
                    }
                    if (currPos != _svCfg.AxisPositions[i])
                    {
                        isNeedWaitAxesINP = true;
                        errCode = md.AbsMove(axisIndex, _svCfg.AxisPositions[i]);
                        if(errCode != 0)
                        {
                            MessageBox.Show("移动轴:" + axis.Name + " 到预设位置:" + _svCfg.AxisPositions[i] +  "失败:" + md.GetErrorInfo(errCode));
                            return;
                        }
                    }
                
                }

            if (isNeedWaitAxesINP)
            {

                _threadSnapWithCurrCfg = new Thread(ThreadFuncSnapWithCurrCfg);
                _isSnapWithCurrCfgRunning = true;
                _threadSnapWithCurrCfg.Start();
            }
            else
            {
                if (!_isAutoSnapping)
                {
                    if (null != _cmr)
                    {
                        IJFImage img = null;
                        errCode = _cmr.GrabOne(out img, 1000);
                        if(errCode !=0)
                            MessageBox.Show("相机拍照失败：" + _cmr.GetErrorInfo(errCode));
                        else
                        {
                            ShowTips("相机拍照完成");
                            ShowImage(img);//ucImage.LoadImage(img);
                            TimeSpan ts = DateTime.Now - startTime;
                            ShowTips("耗时:" + ts.TotalMilliseconds + "毫秒");
                        }
                    }
                }
                else
                    ShowTips("当前处于自动采图，已将设备调整为预设参数");

            }
            


        }

        /// <summary>
        /// 检查相机是否可以拍照
        /// </summary>
        /// <returns></returns>
        bool CheckCmrCanSnap()
        {
            if (_cmr == null)
            {
                MessageBox.Show("连续拍照失败，相机未设置！");
                return false;
            }
            int errCode = 0;
            if (!_cmr.IsDeviceOpen)
            {
                if (DialogResult.OK != MessageBox.Show("当前相机未打开，是否打开设备？", "操作提示", MessageBoxButtons.OKCancel))
                    return false;
                errCode = _cmr.OpenDevice();
                if (errCode != 0)
                {
                    MessageBox.Show("打开相机失败：" + _cmr.GetErrorInfo(errCode));
                    return false;
                }
            }
            JFCmrTrigMode tm = JFCmrTrigMode.disable;
            errCode = _cmr.GetTrigMode(out tm);
            if(errCode != 0)
            {
                MessageBox.Show("打开相机失败,未能获取相机触发模式：" + _cmr.GetErrorInfo(errCode));
                return false;
            }

            if(tm != JFCmrTrigMode.disable)
            {
                if (DialogResult.OK != MessageBox.Show("是否将相机工作模式转为软件采图？", "操作提示", MessageBoxButtons.OKCancel))
                    return false;
                if (_cmr.IsGrabbing)
                    _cmr.StopGrab();
                errCode = _cmr.SetTrigMode(JFCmrTrigMode.disable);
                if(errCode != 0)
                {
                    MessageBox.Show("将相机触发模式转为软件采图失败：" + _cmr.GetErrorInfo(errCode));
                    return false;
                }

            }

            if(_cmr.IsRegistAcqFrameCallback)
            {
                if (DialogResult.OK != MessageBox.Show("是否关闭相机回调模式？", "操作提示", MessageBoxButtons.OKCancel))
                    return false;
                if (_cmr.IsGrabbing)
                    _cmr.StopGrab();
                _cmr.ClearAcqFrameCallback();
            }

            if (!_cmr.IsGrabbing)
            {
                errCode = _cmr.StartGrab();
                if(0 != errCode)
                {
                    MessageBox.Show("开启相机采图失败：" + _cmr.GetErrorInfo(errCode));
                    return false;
                }
                
            }

          
            return true;
        }

        /// <summary>
        /// 使用设备当前状态参数拍照
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSnapByCurrDevParam_Click(object sender, EventArgs e)
        {
            if (!CheckCmrCanSnap())
                return;

            IJFImage img = null;
            int errCode = _cmr.GrabOne(out img, 1000);
            if (errCode != 0)
            {
                JFTipsDelayClose.Show("拍照失败，错误信息:" + _cmr.GetErrorInfo(errCode), 2);
                return;
            }
            else
                ShowTips("拍照OK");

            ShowImage(img);//ucImage.LoadImage(img);
        }

        /// <summary>
        /// 图像X镜像使能选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkReverseX_CheckedChanged(object sender, EventArgs e)
        {
            if (_isSettingChkReversX)
                return;
            if (_cmr == null)
            {
                JFTipsDelayClose.Show("操作失败，相机对象未设置", 2);
                return;
            }
            bool isRevsX = chkReverseX.Checked;
            int errCode = _cmr.SetReverseX(isRevsX);
            if(errCode != 0)
            {
                MessageBox.Show((isRevsX ? "使能" : "禁用") + "相机X轴镜像 失败:" + _cmr.GetErrorInfo(errCode));
                _isSettingChkReversX = true;
                chkReverseX.Checked = !isRevsX;
                _isSettingChkReversX = false;
                return;
            }
            ShowTips("相机X轴镜像使能已设为:" + (isRevsX ? "使能" : "禁用"));
            UpdateCmr2Grid();

        }

        /// <summary>
        /// 图像Y镜像使能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkReverseY_CheckedChanged(object sender, EventArgs e)
        {
            if (_isSettingChkReversY)
                return;

            if (_cmr == null)
            {
                JFTipsDelayClose.Show("操作失败，相机对象未设置", 2);
                return;
            }
            bool isRevsY = chkReverseY.Checked;
            int errCode = _cmr.SetReverseY(isRevsY);
            if (errCode != 0)
            {
                MessageBox.Show((isRevsY ? "使能" : "禁用") + "相机Y轴镜像 失败:" + _cmr.GetErrorInfo(errCode));
                _isSettingChkReversY = true;
                chkReverseY.Checked = !isRevsY;
                _isSettingChkReversY = false;
                return;
            }
            ShowTips("相机Y轴镜像使能已设为:" + (isRevsY ? "使能" : "禁用"));
            UpdateCmr2Grid();
        }

        /// <summary>
        /// 相机曝光参数值发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numExposure_ValueChanged(object sender, EventArgs e)
        {
            if (_isSettingExposure)
                return;

            if (_cmr == null)
            {
                JFTipsDelayClose.Show("操作失败，相机对象未设置", 2);
                return;
            }
            double dVal = Convert.ToDouble(numExposure.Value);
            int errCode = _cmr.SetExposure(dVal);
            if(errCode != 0)
            {
                MessageBox.Show("设置相机曝光参数失败：" + _cmr.GetErrorInfo(errCode));
                errCode = _cmr.GetExposure(out dVal);
                if (0 == errCode)
                {
                    _isSettingExposure = true;
                    numExposure.Value = Convert.ToDecimal(dVal);
                    _isSettingExposure = false;
                }
                return;
            }
            ShowTips("相机曝光值已设为:" + dVal);
            ///将相机参数更新到GridView
            UpdateCmr2Grid();


        }

        /// <summary>
        /// 相机增益参数值发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numGain_ValueChanged(object sender, EventArgs e)
        {
            if (_isSettingGain)
                return;
            if (null == _cmr)
            {
                JFTipsDelayClose.Show("操作失败，相机对象未设置", 2);
                return;
            }


            double dVal = Convert.ToDouble(numGain.Value);
            int errCode = _cmr.SetGain(dVal);
            if (errCode != 0)
            {
                MessageBox.Show("设置相机增益参数失败：" + _cmr.GetErrorInfo(errCode));
                errCode = _cmr.GetGain(out dVal);
                if (0 == errCode)
                {
                    _isSettingGain = true;
                    numGain.Value = Convert.ToDecimal(dVal);
                    _isSettingGain = false;
                }
                return;
            }
            ShowTips("相机增益参数已设为:" + dVal);
            ///将相机参数更新到GridView
            UpdateCmr2Grid();

        }


        bool _isAutoSnapThreadRunning = false;
        Thread _threasdAutoSnap = null;
        void StartAutoSnap()
        {
            if (_isAutoSnapThreadRunning)
                return;
            _isAutoSnapThreadRunning = true;
            _threasdAutoSnap = new Thread(ThreadFuncAutoSnap);
            _threasdAutoSnap.Start();
        }


        void StopAutoSnap()
        {
            if (!_isAutoSnapThreadRunning)
                return;
            _isAutoSnapThreadRunning = false;
            if (!_threasdAutoSnap.Join(1000))
                _threasdAutoSnap.Abort();
            _isAutoSnapping = false;
            _isSettingContinueSnap = true;
            chkContinueSnap.Checked = false;
            _isSettingContinueSnap = false;

        }

        void ThreadFuncAutoSnap()
        {
            int errCode = 0;
            while (_isAutoSnapThreadRunning)
            {
                IJFImage img;
                errCode = _cmr.GrabOne(out img);
                if (errCode != 0)
                {
                    ShowTips("采图失败！ErrorInfo:" + _cmr.GetErrorInfo(errCode));
                    Thread.Sleep(500);
                    continue;
                }
                //ShowTips("拍照OK");
                ShowImage(img);//ucImage.LoadImage(img);

            }
            ShowTips("自动采图已停止");
        }


        /// <summary>
        /// 连续拍照模式发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkContinueSnap_CheckedChanged(object sender, EventArgs e)
        {
            if (_isSettingContinueSnap)
                return;
            if (chkContinueSnap.Checked)
            {
                if (!CheckCmrCanSnap())
                {
                    _isAutoSnapping = false;
                    _isSettingContinueSnap = true;
                    chkContinueSnap.Checked = false;
                    _isSettingContinueSnap = false;
                    return;
                }
                ShowTips("自动采图已开启");
                StartAutoSnap();
            }
            else
            {
                
                StopAutoSnap();
                _isAutoSnapping = false;
            }
            //_isAutoSnapping = chkContinueSnap.Checked;

        }

        /// <summary>
        /// 打开轴所属设备并使能伺服
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="dev"></param>
        /// <param name="ci"></param>
        /// <param name="errInfo"></param>
        /// <returns></returns>
        bool OpenAndEbnablAxis(string axisName, out IJFDevice_MotionDaq dev, out JFDevCellInfo ci, out string errInfo)
        {
            dev = null;
            ci = null;
            errInfo = "Unknown-Error";
            bool ret = false;
            do
            {
                if (string.IsNullOrEmpty(axisName))
                {
                    errInfo = "轴名称为空/字串";
                    break;
                }

                ci = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(axisName);
                if(null == ci)
                {
                    errInfo = "轴:\"" + axisName + "\"在设备命名表中不存在";
                    break;
                }

                dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
                if(null == dev)
                {
                    errInfo = "轴:\"" + axisName + "\"所属设备:\"" + ci.DeviceID + "\"在设备表中不存在";
                    break;
                }

                int errCode = 0;
                if(!dev.IsDeviceOpen)
                {
                    errCode = dev.OpenDevice();
                    if(0!= errCode)
                    {
                        errInfo = "打开轴\"" + axisName + "\"所属设备:\"" + ci.DeviceID + "\"失败:" + dev.GetErrorInfo(errCode);
                        break;
                    }
                }

                if(ci.ModuleIndex >= dev.McCount)
                {
                    errInfo = "轴\"" + axisName + "\"模块序号=" +ci.ModuleIndex + " 超出设备:\"" +  ci.DeviceID + "\"最大序号:" + (dev.McCount-1);
                    break;
                }

                if(ci.ChannelIndex >= dev.GetMc(ci.ModuleIndex).AxisCount)
                {
                    errInfo = "轴\"" + axisName + "\"轴序号=" + ci.ModuleIndex + " 所属模块最大轴序号:" + (dev.GetMc(ci.ModuleIndex).AxisCount - 1);
                    break;
                }

                if(!dev.GetMc(ci.ModuleIndex).IsSVO(ci.ChannelIndex))
                {
                    errCode = dev.GetMc(ci.ModuleIndex).ServoOn(ci.ChannelIndex);
                    if(errCode != 0)
                    {
                        errInfo = "轴\"" + axisName + "\"使能失败:" + dev.GetMc(ci.ModuleIndex).GetErrorInfo(errCode);
                        break;
                    }
                }

                errInfo = "Success";
                ret = true;
            } while (false);
            return ret;
        }



        /// <summary>
        /// 打开并使能所有设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOpenAllDev_Click(object sender, EventArgs e)
        {
            if(null == _sva)
            {
                MessageBox.Show("助手未设置！");
                return;
            }

            bool isOK = true;
            StringBuilder sbErrorInfo = new StringBuilder();
            int errCode = 0;
            ///尝试打开相机
            do
            {
                string cmrName = _sva.CameraName;
                if (string.IsNullOrEmpty(cmrName))
                {
                    isOK = false;
                    sbErrorInfo.AppendLine("打开相机失败，相机名称未设置/空字串");
                    break;
                }
                IJFDevice_Camera cmr = JFHubCenter.Instance.InitorManager.GetInitor(cmrName) as IJFDevice_Camera;
                if (null == cmr)
                {
                    gbCmrCfg.Enabled = false;
                    isOK = false;
                    sbErrorInfo.AppendLine("打开相机失败，设备表中不存在名称为\"" + cmrName + "\"的相机设备");
                    break;
                }
                _cmr = cmr;
                gbCmrCfg.Enabled = true;
                if (!_cmr.IsDeviceOpen)
                {
                    errCode = _cmr.OpenDevice();
                    if (0 != errCode)
                    {
                        isOK = false;
                        sbErrorInfo.AppendLine("打开相机失败:" + _cmr.GetErrorInfo(errCode));
                        break;
                    }
                }

                UpdateCmr2View();
               if(_cmr.IsRegistAcqFrameCallback) //当前相机正在使用回调模式
               {
                    if (_cmr.IsGrabbing)
                    {
                        if(DialogResult.Yes != MessageBox.Show("相机当前为回调模式并正在采集图像，\n需要停止采集并取消回调模式","提示信息",MessageBoxButtons.YesNo))
                        {
                            isOK = false;
                            sbErrorInfo.AppendLine("相机正在使用回调模式采集图像");
                            break;
                        }
                        _cmr.StopGrab();
                    }
                    _cmr.ClearAcqFrameCallback(); //改为取图模式
                }

                _cmr.ClearBuff();
                if (!_cmr.IsGrabbing)
                {
                    errCode = _cmr.StartGrab();
                    if(0!= errCode)
                    {
                        isOK = false;
                        sbErrorInfo.AppendLine("开启相机抓图失败:" + _cmr.GetErrorInfo(errCode));
                        break;
                    }
                }
               
                
            } while (false);


            ///尝试打开所有的光源控制器通道
            string[] lightChnNames = _sva.LightChnNames;
            if (null != lightChnNames && lightChnNames.Length > 0)
                for (int i = 0; i < lightChnNames.Length; i++)//(string lcn in lightChnNames)
                    do
                    {
                        string lcn = lightChnNames[i];
                        if(string.IsNullOrEmpty(lcn))
                        {
                            isOK = false;
                            sbErrorInfo.AppendLine("第0号光源通道打开失败:名称为空");
                            break;
                        }

                        JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetLightCtrlChannelInfo(lcn);
                        if(null == ci)
                        {
                            isOK = false;
                            sbErrorInfo.AppendLine("光源通道:\"" + lcn + "\"打开失败：设备命名表中不存在");
                            break;
                        }

                        IJFDevice_LightController dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_LightController;
                        if(null == dev)
                        {
                            isOK = false;
                            sbErrorInfo.AppendLine("光源通道:\"" + lcn + "\"打开失败,所属设备:\"" + ci.DeviceID + "\"在设备表中不存在");
                            break;
                        }

                        if(!dev.IsDeviceOpen)
                        {
                            errCode = dev.OpenDevice();
                            if(0 != errCode)
                            {
                                isOK = false;
                                sbErrorInfo.AppendLine("光源通道:\"" + lcn + "\"打开失败:" + dev.GetErrorInfo(errCode));
                                break;
                            }
                        }

                        if(ci.ChannelIndex >= dev.LightChannelCount)
                        {
                            isOK = false;
                            sbErrorInfo.AppendLine("光源通道:\"" + lcn + "\"打开失败,通道序号 = " + ci.ChannelIndex + "超出设备最大通道号:" + (dev.LightChannelCount-1));
                            break;
                        }

                        if(dev is IJFDevice_LightControllerWithTrig) //带触发功能的光源控制器
                        {
                            IJFDevice_LightControllerWithTrig devT = dev as IJFDevice_LightControllerWithTrig;
                            JFLightWithTrigWorkMode wm = JFLightWithTrigWorkMode.TurnOnOff;
                            errCode = devT.GetWorkMode(out wm);
                            if(errCode != 0)
                            {
                                isOK = false;
                                sbErrorInfo.AppendLine("未能获取光源通道:\"" + lcn + "\"的工作模式:" + devT.GetErrorInfo(errCode));
                                //break;
                            }
                            if(wm != JFLightWithTrigWorkMode.TurnOnOff)
                            {
                                errCode = devT.SetWorkMode(JFLightWithTrigWorkMode.TurnOnOff);
                                if(errCode != 0)
                                {
                                    //isOK = false; //忽略不支持模式切换的设备
                                    //sbErrorInfo.AppendLine("未能将光源通道:\"" + lcn + "\"切换到开关模式:" + devT.GetErrorInfo(errCode));
                                    //break;
                                }
                            }
                        }
                        bool chnEnabled = false;
                        errCode = dev.GetLightChannelEnable(ci.ChannelIndex, out chnEnabled);
                        if (errCode != 0)
                        {
                            isOK = false;
                            sbErrorInfo.AppendLine("未能获取光源通道:\"" + lcn + "\"的使能状态");
                        }

                        if(!chnEnabled)
                        {
                            errCode = dev.SetLightChannelEnable(ci.ChannelIndex,true);
                            if(errCode != 0 )
                            {
                                isOK = false;
                                sbErrorInfo.AppendLine("使能光源通道:\"" + lcn + "\"失败:" + dev.GetErrorInfo(errCode));
                                break;
                            }
                        }

                    } while (false);

                
            ///尝试打开/使能 标准示教4轴
            if(_sva.ExistTeachAxis4())
            {
                string[] aaName = new string[] { "X", "Y", "Z", "R" };
                string[] axis4Names= _sva.TeachAxis4Names;
                for(int i = 0; i < axis4Names.Length;i++)//foreach(string axisName in axis4Names)
                {
                    string axisName = axis4Names[i];
                    if (string.IsNullOrEmpty(axisName)) //未设置的轴
                        continue;
                    IJFDevice_MotionDaq dev = null;
                    JFDevCellInfo ci = null;
                    string errInfo = null;
                    if(!OpenAndEbnablAxis(axisName,out dev,out ci,out errInfo))
                    {
                        isOK = false;
                        sbErrorInfo.AppendLine("打开/使能 示教4-" + aaName[i] + "失败:" + errInfo);
                    }

                }
            }


            ///尝试打开拓展示教轴
            string[] extendTeachAxisNames = _sva.ExtendTeachAxisNames;
            if (null != extendTeachAxisNames && extendTeachAxisNames.Length > 0)
                foreach (string tan in extendTeachAxisNames)
                {
                    IJFDevice_MotionDaq dev = null;
                    JFDevCellInfo ci = null;
                    string errInfo = null;
                    if (!OpenAndEbnablAxis(tan, out dev, out ci, out errInfo))
                    {
                        isOK = false;
                        sbErrorInfo.AppendLine(errInfo);
                    }
                }

           
            _pnAxis4.UpdateAxisEnabled(); //更新四轴面板的可用状态
            foreach (UcSimpleAxisInStation ucSA in _lstExtendTeachAxis) //更新拓展示教轴可用状态
                ucSA.UpdateAxisUI();
            foreach (UcLightCtrlChn uclc in _lstLightChn) //更新光源控制器通道可用状态
                uclc.UpdateChannelStatus();

            if (isOK)
            {
                ShowTips("示教助手所需设备已全部打开/使能");
                JFTipsDelayClose.Show("示教助手所需设备已全部打开/使能", 2);
                timerFlush.Enabled = true;
            }
            else
            {
                string errInfo = "打开示教器所属设备失败,错误信息:\n" + sbErrorInfo.ToString();
                ShowTips(errInfo);
                MessageBox.Show(errInfo);
            }
        }

        /// <summary>
        /// 保存当前设备参数到当前所选配置项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveCurrDevParam2Cfg_Click(object sender, EventArgs e)
        {
            if(cbAllProgramNames.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择配置项名称！");
                return;
            }

            string progName = cbAllProgramNames.Text;
            if(DialogResult.OK == MessageBox.Show("确定将当前示教状态保存为参数:" + progName + "?","保存参数警告",MessageBoxButtons.OKCancel))
            {
                string errInfo;
                bool isOK = _sva.SaveProgram(progName, out errInfo);
                if (isOK)
                {
                    JFTipsDelayClose.Show("参数已保存！", 2);
                    ShowTips("当前示教状态已保存为:" + progName);
                    UpdateCfg2GridView();


                    ///保存测试动作流文本
                    JFSingleVisionCfgParam vc = JFHubCenter.Instance.VisionMgr.GetSingleVisionCfgByName(progName);
                    vc.TestMethodFlowTxt = _testMethodFlow.ToTxt();
                    JFHubCenter.Instance.VisionMgr.Save();
                }
                else
                {
                    MessageBox.Show("保存参数失败，错误信息:\n" + errInfo);
                    ShowTips("保存参数到配置:" + progName + " 失败，错误信息：" + errInfo);
                }
            }


        }

        /// <summary>
        /// 将当前设备参数另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveAs_Click(object sender, EventArgs e)
        {
            BenameDialog dlg = new BenameDialog();
            dlg.Text = "将设备参数保存为:";
            if (DialogResult.OK != dlg.ShowDialog())
                return;
            string progName = dlg.GetName();
            if(JFHubCenter.Instance.VisionMgr.ContainSingleVisionCfgByName(progName))
            {
                if (DialogResult.OK != MessageBox.Show("已存在同名配置，是否覆盖？", "警告", MessageBoxButtons.OKCancel))
                    return;
            }
            string errInfo;
            bool isOK = _sva.SaveProgram(progName, out errInfo);
            if (isOK)
            {
                JFTipsDelayClose.Show("参数已保存！", 2);
                ShowTips("当前示教状态已保存为:" + progName);
                UpdateCfg2GridView();
                if(!cbAllProgramNames.Items.Contains(progName))//if (!JFHubCenter.Instance.VisionMgr.ContainSingleVisionCfgByName(progName))
                {
                    cbAllProgramNames.Items.Add(progName);
                    if (!cbAllProgramNames.Enabled)
                        cbAllProgramNames.Enabled = true;
                }

                JFSingleVisionCfgParam vc = JFHubCenter.Instance.VisionMgr.GetSingleVisionCfgByName(progName);
                vc.TestMethodFlowTxt = _testMethodFlow.ToTxt();
                JFHubCenter.Instance.VisionMgr.Save();
            }
            else
            {
                MessageBox.Show("保存参数失败，错误信息:\n" + errInfo);
                ShowTips("保存参数到配置:" + progName + " 失败，错误信息：" + errInfo);
            }
            
        }

        /// <summary>
        /// 显示测试流程对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btShowTestMethodFlowDialog_Click(object sender, EventArgs e)
        {
            if(_svCfg == null)
            {
                MessageBox.Show("配置参数项未设置，请先选择参数配置项");
                return;
            }
            FormMethodFlowTest fm = new FormMethodFlowTest();
            
            fm.Text = "示教助手" + _sva.Name + "  测试动作流";
            fm.SetMethodFlow(_testMethodFlow);
            fm.ShowDialog();
        }


        /// <summary>
        /// 更新当前界面，并提供自动拍照
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerFlush_Tick(object sender, EventArgs e)
        {
            if(_sva == null)
            {
                timerFlush.Enabled = false;
                return;
            }
            if(_svCfg == null)
            {

            }

            //更新轴控制面板状态
            UpdateAxisPanel();
            UpdateAxis2Grid();
            //更新光源参数到表格
            UpdateLight2Grid();
            //GridView
            // UpdateCurrDev2GridView();
        }

        private void btClearTips_Click(object sender, EventArgs e)
        {
            rchTips.Text = "";
        }

        private void cbAllProgramNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAllProgramNames.SelectedIndex < 0)
                return;
            if (_isOutterSetVcName)
                return;
            
            string pn = cbAllProgramNames.SelectedItem.ToString();
            JFSingleVisionCfgParam svCfg = JFHubCenter.Instance.VisionMgr.GetSingleVisionCfgByName(pn);
            if (svCfg == null)
            {
                MessageBox.Show("系统配置中不存在名称为 " + pn + " 的视觉配置参数项");
                return;
            }

            // _testMethodFlow.FromTxt()


            _svCfg = svCfg;
            _testMethodFlow.FromTxt(_svCfg.TestMethodFlowTxt);

            lbAssistInfo.Text = "示教助手:" + _sva.Name + "相机:" + _sva.CameraName + "    配置名:" + pn;// == _svCfg.Name;
            UpdateCfg2GridView();
        }

        /// <summary>
        /// 将当前配置参数另存为（拷贝到另一个配置）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveCurrCfgAs_Click(object sender, EventArgs e)
        {
            if (null == _svCfg)
            {
                MessageBox.Show("请先选择配置项");
                return;
            }

            BenameDialog dlg = new BenameDialog();
            dlg.Text = "将设备参数保存为:";
            if (DialogResult.OK != dlg.ShowDialog())
                return;
            JFSingleVisionCfgParam vc = null;
            string progName = dlg.GetName();
            if (JFHubCenter.Instance.VisionMgr.ContainSingleVisionCfgByName(progName))
            {
                if (DialogResult.OK != MessageBox.Show("已存在同名配置，是否覆盖？", "警告", MessageBoxButtons.OKCancel))
                    return;
                vc = JFHubCenter.Instance.VisionMgr.GetSingleVisionCfgByName(progName);
            }
            else
                vc = new JFSingleVisionCfgParam();

            vc.Name = progName;
            vc.OwnerAssist = _svCfg.OwnerAssist;
            vc.CmrReverseX = _svCfg.CmrReverseX;
            vc.CmrReverseY = _svCfg.CmrReverseY;
            vc.CmrExposure = _svCfg.CmrExposure;
            vc.CmrGain = _svCfg.CmrGain;
            vc.LightChnNames = _svCfg.LightChnNames;
            vc.LightIntensities = _svCfg.LightIntensities;
            vc.AxisNames = _svCfg.AxisNames;
            vc.AxisPositions = _svCfg.AxisPositions;
            vc.TrigChnNames = _svCfg.TrigChnNames;
            vc.TrigIntensities = _svCfg.TrigIntensities;
            vc.TrigDurations = _svCfg.TrigDurations;
            vc.TestMethodFlowTxt = _svCfg.TestMethodFlowTxt;
            vc.CfgMethodFlowTxt = _svCfg.CfgMethodFlowTxt;

            if (!JFHubCenter.Instance.VisionMgr.ContainSingleVisionCfgByName(progName))
                JFHubCenter.Instance.VisionMgr.AddSingleVisionCfg(progName, vc);

            JFHubCenter.Instance.VisionMgr.Save();


            JFTipsDelayClose.Show("参数已保存！", 2);
            ShowTips("当前配置已已存为:" + progName);
            UpdateCfg2GridView();
            if(!cbAllProgramNames.Items.Contains(progName))//if (!JFHubCenter.Instance.VisionMgr.ContainSingleVisionCfgByName(progName))
            {
                cbAllProgramNames.Items.Add(progName);
                if (!cbAllProgramNames.Enabled)
                    cbAllProgramNames.Enabled = true;
            }

        }

        /// <summary>
        /// 相机拍照并存图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveImgAs_Click(object sender, EventArgs e)
        {
            if (null == _cmr)
            {
                ShowTips("操作失败，相机未设置");
                return;
            }
            IJFImage img = null;
            int errCode = _cmr.GrabOne(out img);


            if (0!= errCode)
            {

                MessageBox.Show("未能保存图像，相机采图失败:"+_cmr.GetErrorInfo(errCode));
                return;
            }
            if (cbImgFileFormat.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择文件格式！");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            switch (cbImgFileFormat.SelectedIndex)
            {
                case 0:
                    sfd.Filter = "BMP files(*.BMP)| *.BMP ";
                    break;
                case 1:
                    sfd.Filter = "JPG files(*.JPG) | *.JPG ";
                    break;
                case 2:
                    sfd.Filter = " PNG files(*.PNG) | *.PNG ";
                    break;
                case 3:
                    sfd.Filter = "TIF files(*.TIF) | *.TIF";
                    break;
                default:
                    throw new Exception("ImgFileFormat is not selected!");
                    //break;
            }

            sfd.FileName = "保存";//设置默认文件名
            sfd.DefaultExt = "BMP";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名
            sfd.CheckFileExists = false;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                int err = img.Save(sfd.FileName, (JFImageSaveFileType)cbImgFileFormat.SelectedIndex);
                if (err != 0)
                    MessageBox.Show("保存文件失败，ErrorInfo:" + img.GetErrorInfo(err));
                else
                    ShowTips("图片已保存至文件:" + sfd.FileName);


            }
        }

        HObject hoImgShowing = null;

        /// <summary>
        /// 显示一张图片
        /// </summary>
        /// <param name="img"></param>
        public void ShowImage(IJFImage img)
        {

            object obImg = null;
            img.GenHalcon(out obImg);

            htWindowControl1.DispImage(obImg as HObject);
            if(null != hoImgShowing)
            {
                hoImgShowing.Dispose();
                hoImgShowing = obImg as HObject;
            }
            //ucImage.LoadImage(img);
        }

        /// <summary>
        /// 相机抓图并显示
        /// </summary>
        public void SnapOneAndShow()
        {
            if(_cmr == null)
            {
                ShowTips("相机为设置/空值");
                return;
            }

            if (chkContinueSnap.Checked) //当前正处于连续拍照模式
                return;
            IJFImage img = null;
            int ret = _cmr.GrabOne(out img, 1000);
            if(ret != 0)
            {
                ShowTips("相机抓图失败:" + _cmr.GetErrorInfo(ret));
                return;
            }

            ShowImage(img);//ucImage.LoadImage(img);
            ShowTips("拍照OK");

        }


        bool _isOutterSetVcName = false;
        delegate void dgSetVcName(string vcName);
        /// <summary>
        /// 设置视觉配置名称(外部程序调用)
        /// </summary>
        /// <param name="vcName"></param>
        public void SetVcName(string vcName)
        {
            if(InvokeRequired)
            {
                Invoke(new dgSetVcName(SetVcName), new object[] { vcName });
                return;
            }
            JFSingleVisionCfgParam svCfg = JFHubCenter.Instance.VisionMgr.GetSingleVisionCfgByName(vcName);
            if (svCfg == null)
            {
                MessageBox.Show("系统配置中不存在名称为 " + vcName + " 的视觉配置参数项");
                return;
            }
            _isOutterSetVcName = true;
            cbAllProgramNames.Text = vcName;
            _isOutterSetVcName = false;
            // _testMethodFlow.FromTxt()


            _svCfg = svCfg;
            _testMethodFlow.FromTxt(_svCfg.TestMethodFlowTxt);

            lbAssistInfo.Text = "示教助手:" + _sva.Name + "相机:" + _sva.CameraName + "    配置名:" + vcName;// == _svCfg.Name;
            UpdateCfg2GridView();
        }
    }
}
