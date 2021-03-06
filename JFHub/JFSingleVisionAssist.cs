using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JFInterfaceDef;
namespace JFHub
{
    /// <summary>
    /// 单目视觉系统的相机+运动控制的示教类
    /// </summary>
    [JFVersion("1.0.0")]
    [JFDisplayName("单相机示教")]
    public class JFSingleVisionAssist:IJFInitializable,IJFRealtimeUIProvider
    {

        public JFSingleVisionAssist()
        {
            IsInitOK = false;
        }

        string _cmrName = "";//相机名称
        string[] _lightNames = new string[] { }; //光源通道名称
        string _teachAxisXName = null; //标准示教X轴 ,(非配置项轴)
        string _teachAxisYName = null;
        string _teachAxisZName = null;
        string _teachAxisRName = null;
        string[] _teachExpandAxisNames = new string[] { }; //其他示教轴

        string[] _lightTrigNames = new string[] { };//光源触发通道


        string[] _cfgAxisNames = new string[] { }; // 参与配置的轴数据项
        //double[] _cfgAxisPositions = new double[] { }; //参与配置的轴位置


        string[] _initParamNames = new string[] { "相机", "光源通道", "示教X轴", "示教Y轴", "示教Z轴", "示教R轴", "其他示教轴", "光源触发通道", "配置轴" };
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public string[] InitParamNames { get { return _initParamNames; } }


        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == _initParamNames[0]) //"相机",
            {
                string[] allCmrNames = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_Camera));
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.Options, allCmrNames);
            }
            else if(name == _initParamNames[1])// "光源通道",
            {
                string[] allLightNames = JFHubCenter.Instance.MDCellNameMgr.AllLightChannelNames();
                return JFParamDescribe.Create(name, typeof(string[]), JFValueLimit.NonLimit, allLightNames);
            }
            else if (name == _initParamNames[2])// "示教X轴", 
            {
                
                string[] allAxisNames = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
                List<string> optionAxisNames = new List<string>();
                optionAxisNames.Add("无");
                optionAxisNames.AddRange(allAxisNames);
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.Options, optionAxisNames.ToArray());
            }
            else if (name == _initParamNames[3])//"示教Y轴", 
            {
                string[] allAxisNames = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
                List<string> optionAxisNames = new List<string>();
                optionAxisNames.AddRange(allAxisNames);
                optionAxisNames.Add("无");
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.Options, optionAxisNames.ToArray());

            }
            else if (name == _initParamNames[4])//"示教Z轴", 
            {
                string[] allAxisNames = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
                List<string> optionAxisNames = new List<string>();
                optionAxisNames.AddRange(allAxisNames);
                optionAxisNames.Add("无");
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.Options, optionAxisNames.ToArray());
            }
            else if (name == _initParamNames[5])//"示教R轴", 
            {
                string[] allAxisNames = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
                List<string> optionAxisNames = new List<string>();
                optionAxisNames.AddRange(allAxisNames);
                optionAxisNames.Add("无");
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.Options, optionAxisNames.ToArray());
            }
            else if (name == _initParamNames[6])//"其他示教轴", 
            {
                string[] allAxisNames = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
                return JFParamDescribe.Create(name, typeof(string[]), JFValueLimit.NonLimit, allAxisNames);
            }
            else if (name == _initParamNames[7])//"光源触发通道",
            {
                string[] allTrigNames = JFHubCenter.Instance.MDCellNameMgr.AllTrigChannelNames();
                return JFParamDescribe.Create(name, typeof(string[]), JFValueLimit.NonLimit, allTrigNames);
            }
            else if (name == _initParamNames[8])// "配置轴"
            {
                string[] allAxisNames = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
                return JFParamDescribe.Create(name, typeof(string[]), JFValueLimit.NonLimit, allAxisNames);
            }
            else
                throw new ArgumentException("name = \"" + name + "\"不是有效的初始化参数名称 in JFSingleVisionAssist.GetInitParamDescribe");

        }

        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public object GetInitParamValue(string name)
        {
            if (name == _initParamNames[0]) //"相机",
            {
                return _cmrName;
            }
            else if (name == _initParamNames[1])// "光源通道",
            {
                return _lightNames;
            }
            else if (name == _initParamNames[2])// "示教X轴", 
            {
                return _teachAxisXName;
            }
            else if (name == _initParamNames[3])//"示教Y轴", 
            {
                return _teachAxisYName;

            }
            else if (name == _initParamNames[4])//"示教Z轴", 
            {
                return _teachAxisZName;
            }
            else if (name == _initParamNames[5])//"示教R轴", 
            {
                return _teachAxisRName;
            }
            else if (name == _initParamNames[6])//"其他示教轴", 
            {
                return _teachExpandAxisNames;
            }
            else if (name == _initParamNames[7])//"光源触发通道",
            {
                return _lightTrigNames;
            }
            else if (name == _initParamNames[8])// "配置轴"
            {
                return _cfgAxisNames;
            }
            else
                throw new ArgumentException("name = \"" + name + "\"不是有效的初始化参数名称 in JFSingleVisionAssist.GetInitParamValue");

        }

        string _initErrorInfo = "No-Opts";//无操作
        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool SetInitParamValue(string name, object value)
        {
            if (name == _initParamNames[0]) //"相机",
            {
                _cmrName = value as string;
            }
            else if (name == _initParamNames[1])// "光源通道",
            {
                _lightNames = value  as string[];
            }
            else if (name == _initParamNames[2])// "示教X轴", 
            {
                string strVal = value as string;
                if (null == strVal||"无" == strVal)
                    strVal = "";
                _teachAxisXName = strVal;
                

            }
            else if (name == _initParamNames[3])//"示教Y轴", 
            {
                string strVal = value as string;
                if (null == strVal || "无" == strVal)
                    strVal = "";
                _teachAxisYName = strVal;

            }
            else if (name == _initParamNames[4])//"示教Z轴", 
            {
                string strVal = value as string;
                if (null == strVal || "无" == strVal)
                    strVal = "";
                _teachAxisZName = strVal;
            }
            else if (name == _initParamNames[5])//"示教R轴", 
            {
                string strVal = value as string;
                if (null == strVal || "无" == strVal)
                    strVal = "";
                _teachAxisRName = strVal;
            }
            else if (name == _initParamNames[6])//"其他示教轴", 
            {
                _teachExpandAxisNames = value as string[];
            }
            else if (name == _initParamNames[7])//"光源触发通道",
            {
                _lightTrigNames = value as string[];
            }
            else if (name == _initParamNames[8])// "配置轴"
            {
                _cfgAxisNames = value as string[];
            }
            else
                throw new ArgumentException("name = \"" + name + "\"不是有效的初始化参数名称 in JFSingleVisionAssist.GetInitParamValue");
            return true;
        }


        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool Initialize()
        {
            _cmr = null;
            _lightChns.Clear();
            _teachAxisX = null;
            _teachAxisY = null;
            _teachAxisZ = null;
            _teachAxisR = null;
            _extendAxes.Clear(); //拓展示教轴
            _cfgAxes.Clear();    //配置项轴
            _trigChns.Clear();    //所有触发光源通道

            bool isOK = true;
            StringBuilder _sbError = new StringBuilder();
            ///判断相机名称合法性 if (name == _initParamNames[0]) //"相机",
            if (string.IsNullOrEmpty(_cmrName))   //参数的合法性检查统一放在Init函数中
            {
                _sbError.AppendLine("参数项:\"相机\" 的值未设置/为空值");
                isOK = false;
            }

            string[] allCmrName = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_Camera));
            if (null == allCmrName)
            {
                _sbError.AppendLine("参数项:\"相机\" = " + _cmrName + " 在设备列表中不存在（设备表中无相机设备）");
                isOK = false;
            }


            bool isCmrExisted = false;
            foreach (string existCmr in allCmrName)
                if (existCmr == _cmrName)
                {
                    isCmrExisted = true;
                    break;
                }
            if (!isCmrExisted)
            {
                _sbError.AppendLine("参数项:\"相机\" = " + _cmrName + " 在设备列表中不存在");
                isOK = false;
            }
            else
                _cmr = JFHubCenter.Instance.InitorManager.GetInitor(_cmrName) as IJFDevice_Camera;

            ///光源通道名称检查
            do
            {
                if (_lightNames != null && _lightNames.Length > 0)
                {
                    foreach(string ln in _lightNames)
                        _lightChns.Add(new JFDevChannel(JFDevCellType.Light, ln));
                    string[] allLightNames = JFHubCenter.Instance.MDCellNameMgr.AllLightChannelNames();
                    if (null == allLightNames)
                    {
                        _sbError.AppendLine("光源通道 = " + string.Join(",", _lightNames) + " 在设备表中不存在 (设备表中无光源通道)");
                        isOK = false;
                        break;
                    }
                    List<string> existedLightNames = allLightNames.ToList();

                    foreach (string lightName in _lightNames)
                    {
                        if (!existedLightNames.Contains(lightName))
                        {
                            isOK = false;
                            _sbError.AppendLine("光源通道 = " + lightName + " 在设备表中不存在");
                            
                        }
;
                    }

                }
            } while (false);

            string[] allAxisNames = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            List<string> existedAxisNames = allAxisNames == null?new List<string>(): allAxisNames.ToList();

            ///X轴名称检查
            if (null == _teachAxisXName)
            {
                isOK = false;
                _sbError.AppendLine("参数项:\"示教X轴\"未设置");
                
            }
            else
            {
                if (string.Empty != _teachAxisXName)
                {
                    _teachAxisX = new JFDevChannel(JFDevCellType.Axis, _teachAxisXName);
                    if (!existedAxisNames.Contains(_teachAxisXName))
                    {
                        isOK = false;
                        _sbError.AppendLine("参数项:\"示教X轴\" = " + _teachAxisXName + " 在设备列表中不存在" + (existedAxisNames.Count == 0 ? "(设备表中不存在轴通道)" : ""));
                    }
                        
                }
            }


            ///Y轴名称检查
            if (null == _teachAxisYName)
            {
                isOK = false;
                _sbError.AppendLine("参数项:\"示教Y轴\"未设置");
            }
            else
            {
                if (_teachAxisYName != string.Empty)
                {
                    _teachAxisY = new JFDevChannel(JFDevCellType.Axis, _teachAxisYName);
                    if (!existedAxisNames.Contains(_teachAxisYName))
                    {
                        isOK = false;
                        _sbError.AppendLine("参数项:\"示教Y轴\" = " + _teachAxisYName + " 在设备列表中不存在" + (existedAxisNames.Count == 0 ? "(设备表中不存在轴通道)" : ""));
                    }
                       
                }
            }
            ///Z轴名称检查else if (name == _initParamNames[4])//"示教Z轴", 
            if (null == _teachAxisZName)
            {
                isOK = false;
                _sbError.AppendLine("参数项:\"示教Z轴\"未设置");
            }
            else
            {
                if (string.Empty != _teachAxisZName)
                {
                    _teachAxisZ = new JFDevChannel(JFDevCellType.Axis, _teachAxisZName);
                    if (!existedAxisNames.Contains(_teachAxisZName))
                    {
                        isOK = false;
                        _sbError.AppendLine("参数项:\"示教Z轴\" = " + _teachAxisZName + " 在设备列表中不存在" + (existedAxisNames.Count == 0 ? "(设备表中不存在轴通道)" : ""));
                    }   
                }
            }
            ///R轴名称检查 else if (name == _initParamNames[5])//"示教R轴", 
            if (null == _teachAxisRName)
            {
                isOK = false;
                _sbError.AppendLine("参数项:\"示教R轴\"未设置");
            }
            else
            {
                if (_teachAxisRName != string.Empty)
                {
                    _teachAxisR = new JFDevChannel(JFDevCellType.Axis, _teachAxisRName);
                    if (!existedAxisNames.Contains(_teachAxisRName))
                    {
                        isOK = false;
                        _sbError.AppendLine("参数项:\"示教R轴\" = " + _teachAxisRName + " 在设备列表中不存在" + (existedAxisNames.Count == 0 ? "(设备表中不存在轴通道)" : ""));
                    }   
                }
            }
            ///其他示教轴名称检查else if (name == _initParamNames[6])//"其他示教轴", 
            if (null != _teachExpandAxisNames && _teachExpandAxisNames.Length > 0)
            {
                foreach (string an in _teachExpandAxisNames)
                    _extendAxes.Add(new JFDevChannel(JFDevCellType.Axis, an));
                if (existedAxisNames.Count == 0)
                {
                    isOK = false;
                    _sbError.AppendLine("参数项:\"示教轴s\" = " + string.Join(",", _teachExpandAxisNames) + " 在设备列表中不存在(设备表中不存在轴通道)");
                }
                else
                {
                    foreach (string an in _teachExpandAxisNames)
                    {
                        if (!existedAxisNames.Contains(an))
                        {
                            isOK = false;
                            _sbError.AppendLine("参数项:\"示教轴s\" = " + an + " 在设备列表中不存在");
                        }
                    }

                }
            }
            
            /// 光源触发通道名称检查   else if (name == _initParamNames[7])//"光源触发通道",
            string[] allTrigChnNames = JFHubCenter.Instance.MDCellNameMgr.AllTrigChannelNames();
            List<string> existedTrigChnNames = allTrigChnNames != null ? allTrigChnNames.ToList() : new List<string>();
            if(null != _lightTrigNames && _lightTrigNames.Length > 0)
            {
                foreach (string tn in _lightTrigNames)
                    _trigChns.Add(new JFDevChannel(JFDevCellType.Trig, tn));
                if(existedTrigChnNames.Count == 0 )
                {
                    isOK = false;
                    _sbError.AppendLine("参数项:\"光源触发通道\" = " + string.Join(",", _lightTrigNames) + " 在设备列表中不存在(设备表中无光源触发器)" );

                }
                else
                {
                    foreach(string tn in _lightTrigNames)
                    {
                        if(!existedTrigChnNames.Contains(tn))
                        {
                            isOK = false;
                            _sbError.AppendLine("参数项:\"光源触发通道\" = " + tn + " 在设备列表中不存在");

                        }
                    }
                }
            }
            ///配置轴名称检查 else if (name == _initParamNames[8])// "配置轴"
            if(null != _cfgAxisNames && _cfgAxisNames.Length >0)
            {
                foreach (string an in _cfgAxisNames)
                    _cfgAxes.Add(new JFDevChannel(JFDevCellType.Axis,an));
                if (existedAxisNames.Count == 0)
                {
                    isOK = false;
                    _sbError.AppendLine("参数项:\"配置轴s\" = " + string.Join(",", _cfgAxisNames) + " 在设备列表中不存在(设备表中不存在轴通道)");
                }
                else
                {
                    foreach (string an in _cfgAxisNames)
                        if (!existedAxisNames.Contains(an))
                        {
                            isOK = false;
                            _sbError.AppendLine("参数项:\"配置轴\" = " + an + " 在设备列表中不存在");

                        }
                }
            }
            IsInitOK = isOK;
            
            if (IsInitOK)
                _initErrorInfo = "Success";
            else
                _initErrorInfo = _sbError.ToString();
            return isOK;
        }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get; private set; }

        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo()
        {
            return _initErrorInfo;
        }

        UcSingleVisionAssist _ucRT = null;
        object lockRT = new object();
        /// <summary>
        /// 获取一个（新建的）调试界面
        /// </summary>
        /// <returns></returns>
        public JFRealtimeUI GetRealtimeUI()
        {
            if (null != _ucRT)
                return _ucRT;
            Monitor.Enter(lockRT);
            if (null != _ucRT)
            {
                Monitor.Exit(lockRT);
                return _ucRT;
            }
            _ucRT = new UcSingleVisionAssist();
            //_ucRT.Dock = System.Windows.Forms.DockStyle.Fill;
            _ucRT.SetAssist(this);
            Monitor.Exit(lockRT);
            return _ucRT;
        }


        /// <summary>
        /// 保存当前视觉配置
        /// </summary>
        /// <param name="filePath"></param>
        public bool SaveProgram(string programName,out string errorInfo)
        {
            errorInfo = "UnknownError";
            bool ret = false;
            do
            {
                if (string.IsNullOrEmpty(programName))
                {
                    errorInfo = "配置名称为空字串";
                    break;
                }

                JFVisionManager vm = JFHubCenter.Instance.VisionMgr;
                JFSingleVisionCfgParam vc = null;
                if (vm.ContainSingleVisionCfgByName(programName))
                    vc = vm.GetSingleVisionCfgByName(programName);
                else
                    vc = new JFSingleVisionCfgParam();
                string ei;
                if(!OpenEnableDevices(out ei))
                {
                    errorInfo = "设备未全部打开并使能:" + ei;
                    break;
                }
                ///相机参数
                double exposure = 0; //相机曝光值
                double gain = 0;
                bool isRevsX = false;
                bool isRevsY = false;

                int errCode = _cmr.GetExposure(out exposure);
                if(errCode != 0)
                {
                    errorInfo = "获取相机曝光值失败:" + _cmr.GetErrorInfo(errCode);
                    break;
                }

                errCode = _cmr.GetGain(out gain);
                if (errCode != 0)
                {
                    errorInfo = "获取相机增益值失败:" + _cmr.GetErrorInfo(errCode);
                    break;
                }

                errCode = _cmr.GetReverseX(out isRevsX);
                if (errCode != 0)
                {
                    errorInfo = "获取相机X镜像参数失败:" + _cmr.GetErrorInfo(errCode);
                    break;
                }

                errCode = _cmr.GetReverseY(out isRevsY);
                if (errCode != 0)
                {
                    errorInfo = "获取相机Y镜像参数失败:" + _cmr.GetErrorInfo(errCode);
                    break;
                }

                ///光源通道参数
                int[] intensities = null;  
                if(_lightNames != null && _lightNames.Length > 0)
                {
                    intensities = new int[_lightNames.Length];
                    for(int i = 0; i < _lightNames.Length;i++)
                    {
                        int nVal = 0;
                        IJFDevice_LightController dev = _lightChns[i].Device() as IJFDevice_LightController;
                        errCode = dev.GetLightIntensity(_lightChns[i].CellInfo().ChannelIndex, out nVal);
                        if(errCode != 0)
                        {
                            errorInfo = "获取光源:\"" + _lightNames[i] + "\"亮度值失败：" + dev.GetErrorInfo(errCode);
                            goto LoopExit;
                        }
                        intensities[i] = nVal;
                    }
                }


                double[] axisPos = null;
                ///配置轴参数
                if(_cfgAxisNames != null && _cfgAxisNames.Length > 0)
                {
                    double dVal = 0;
                    axisPos = new double[_cfgAxisNames.Length];
                    for(int i = 0; i < _cfgAxisNames.Length;i++)
                    {
                        IJFDevice_MotionDaq md = _cfgAxes[i].Device() as IJFDevice_MotionDaq;
                        JFDevCellInfo ci = _cfgAxes[i].CellInfo();
                        errCode = md.GetMc(ci.ModuleIndex).GetFbkPos(ci.ChannelIndex, out dVal);
                        if(errCode != 0)
                        {
                            errorInfo = "获取轴:\"" + _cfgAxisNames[i] + "\"位置失败:" + md.GetMc(ci.ModuleIndex).GetErrorInfo(errCode);
                            goto LoopExit;
                        }
                        axisPos[i] = dVal;
                    }
                }

                vc.CmrReverseX = isRevsX;
                vc.CmrReverseY = isRevsY;
                vc.CmrExposure = exposure;
                vc.CmrGain = gain;
                vc.LightChnNames = _lightNames == null ? new string[] { } : _lightNames;
                vc.LightIntensities = intensities == null ? new int[] { } : intensities;
                vc.AxisNames = _cfgAxisNames == null ? new string[] { } : _cfgAxisNames;
                vc.AxisPositions = axisPos == null ? new double[] { } : axisPos;
                if (!vm.ContainSingleVisionCfgByName(programName))
                {
                    vc.Name = programName;
                    vc.OwnerAssist = Name;
                    vm.AddSingleVisionCfg(programName, vc);
                }
                else
                    vm.Save();

                errorInfo = "Success";
                ret = true;
            } while (false);
            LoopExit:
            return ret;
        }

        public string Name
        {
            get 
            {
                return JFHubCenter.Instance.VisionMgr.GetSVAName(this) ;
            }
        }

        public string CameraName
        {
            get { return _cmrName; }
        }


        public string[] LightChnNames
        {
            get { return _lightNames; }
        }





        /// <summary>
        /// 是否存在标准示教4轴（可缺项）
        /// </summary>
        /// <returns></returns>
        public bool ExistTeachAxis4()
        {
            return (!string.IsNullOrEmpty(_teachAxisXName)||
                !string.IsNullOrEmpty(_teachAxisYName) ||
                !string.IsNullOrEmpty(_teachAxisZName) ||
                !string.IsNullOrEmpty(_teachAxisRName));
        }



        string[] _standardAxis4Names = new string[] { "", "", "", "" };
        /// <summary>
        /// 获取标准示教4轴的名称
        /// </summary>
        public string[] TeachAxis4Names
        {
            get
            {
                _standardAxis4Names[0] = _teachAxisXName == null?"": _teachAxisXName;
                _standardAxis4Names[1] = _teachAxisYName == null ? "" : _teachAxisYName;
                _standardAxis4Names[2] = _teachAxisZName == null ? "" : _teachAxisZName;
                _standardAxis4Names[3] = _teachAxisRName == null ? "" : _teachAxisRName;
                return _standardAxis4Names;
            }
        }

        /// <summary>
        /// 其他拓展示教轴
        /// </summary>
        public string[] ExtendTeachAxisNames
        {
            get { return _teachExpandAxisNames; }
        }


        /// <summary>
        /// 光源触发器通道
        /// </summary>
        public string[] TrigChnNames
        {
            get { return _lightTrigNames; }
        }


        IJFDevice_Camera _cmr = null;
        List<JFDevChannel> _lightChns = new List<JFDevChannel>();   //所有开关光源通道
        //JFDevChannel[] _axisTeach4 = new JFDevChannel[] { null,null,null,null};  //示教4轴
        JFDevChannel _teachAxisX = null;
        JFDevChannel _teachAxisY = null;
        JFDevChannel _teachAxisZ = null;
        JFDevChannel _teachAxisR = null;

        List<JFDevChannel> _extendAxes = new List<JFDevChannel>(); //拓展示教轴
        List<JFDevChannel> _cfgAxes = new List<JFDevChannel>();    //配置项轴
        List<JFDevChannel> _trigChns = new List<JFDevChannel>();    //所有触发光源通道



        public IJFDevice_Camera Camera 
        {
            get 
            {
                return _cmr;
            }
        }
        
        /// <summary>
        /// 开关式光源通道
        /// </summary>
        public JFDevChannel[] LightChns { get { return _lightChns.ToArray(); } }


        public JFDevChannel TeachAxisX { get{ return _teachAxisX; } }
        public JFDevChannel TeachAxisY { get { return _teachAxisY; } }
        public JFDevChannel TeachAxisZ { get { return _teachAxisZ; } }
        public JFDevChannel TeachAxisR { get { return _teachAxisR; } }


        /// <summary>
        /// 拓展示教轴
        /// </summary>
        public JFDevChannel[] ExtendTeachAxes { get { return _extendAxes.ToArray(); } }

        /// <summary>
        /// 参数配置轴
        /// </summary>
        public JFDevChannel[] ConfigAxes { get { return _cfgAxes.ToArray(); } }

        /// <summary>
        /// 光源触发通道
        /// </summary>
        public JFDevChannel[] TrigChns { get { return _trigChns.ToArray(); } }



        public bool OpenEnableDevices(out string errInfo)
        {
            errInfo = "Success";
            bool isOK = true;
            StringBuilder sbError = new StringBuilder();
            do
            {
                if (!IsInitOK)
                {
                    isOK = false;
                    sbError.AppendLine("初始化未完成：" + GetInitErrorInfo());
                    break;
                }
                int errCode = 0;

                ///尝试打开相机
                if(null == _cmr)
                {
                    sbError.AppendLine("相机无效,设备名称 = \"" + _cmrName + "\"");
                    isOK = false;
                }
                else
                {
                    if(!_cmr.IsDeviceOpen)
                    {
                        errCode = _cmr.OpenDevice();
                        if (errCode!=0)
                        {
                            isOK = false;
                            sbError.AppendLine("打开相机失败:" + _cmr.GetErrorInfo(errCode));
                        }
                    }
                    ///将相机切换到主动采图模式
                    if(_cmr.IsGrabbing)
                    {

                    }
                }

                ///尝试打开光源设备
                for(int i = 0; i < _lightChns.Count;i++)
                    do
                    {
                        string ei = null;
                        if (!_lightChns[i].OpenDev(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine(" 打开光源:\"" + _lightChns[i].Name + "\"失败：" + ei);
                            break;
                        }
                        if(!_lightChns[i].CheckAvalid(out ei)) //检查通道有效性
                        {
                            isOK = false;
                            sbError.AppendLine(" 打开光源:\"" + _lightChns[i].Name + "\"失败：" + ei);
                            break;
                        }

                        if(!_lightChns[i].EnabledChannel(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("使能光源:\"" + _lightChns[i].Name + "\"失败：" + ei);
                            break;
                        }
                    } while (false);


                ///尝试使能示教轴X
                if (!string.IsNullOrEmpty(_teachAxisXName))
                    do
                    {
                        string ei = "";
                        if (!_teachAxisX.OpenDev(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教X轴打开失败:\"" + _teachAxisX.Name + "\"，错误信息:" + ei);
                            break;
                        }
                        if (!_teachAxisX.CheckAvalid(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教X轴打开失败:\"" + _teachAxisX.Name + "\"，错误信息:" + ei);
                            break;
                        }
                        if(!_teachAxisX.EnabledChannel(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教X轴使能（伺服上电）失败:\"" + _teachAxisX.Name + "\"，错误信息:" + ei);
                            break;
                        }

                    } while (false);


                ///尝试使能示教轴Y
                if (!string.IsNullOrEmpty(_teachAxisYName))
                    do
                    {
                        string ei = "";
                        if (!_teachAxisY.OpenDev(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教Y轴打开失败:\"" + _teachAxisY.Name + "\"，错误信息:" + ei);
                            break;
                        }
                        if (!_teachAxisY.CheckAvalid(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教Y轴打开失败:\"" + _teachAxisY.Name + "\"，错误信息:" + ei);
                            break;
                        }
                        if (!_teachAxisY.EnabledChannel(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教Y轴使能（伺服上电）失败:\"" + _teachAxisY.Name + "\"，错误信息:" + ei);
                            break;
                        }

                    } while (false);


                ///尝试使能示教轴Z
                if (!string.IsNullOrEmpty(_teachAxisZName))
                    do
                    {
                        string ei = "";
                        if (!_teachAxisZ.OpenDev(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教Z轴打开失败:\"" + _teachAxisZ.Name + "\"，错误信息:" + ei);
                            break;
                        }
                        if (!_teachAxisZ.CheckAvalid(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教Z轴打开失败:\"" + _teachAxisZ.Name + "\"，错误信息:" + ei);
                            break;
                        }
                        if (!_teachAxisZ.EnabledChannel(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教Z轴使能（伺服上电）失败:\"" + _teachAxisZ.Name + "\"，错误信息:" + ei);
                            break;
                        }

                    } while (false);


                ///尝试使能示教轴R
                if (!string.IsNullOrEmpty(_teachAxisRName))
                    do
                    {
                        string ei = "";
                        if (!_teachAxisR.OpenDev(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教R轴打开失败:\"" + _teachAxisR.Name + "\"，错误信息:" + ei);
                            break;
                        }
                        if (!_teachAxisR.CheckAvalid(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教R轴打开失败:\"" + _teachAxisR.Name + "\"，错误信息:" + ei);
                            break;
                        }
                        if (!_teachAxisR.EnabledChannel(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("示教R轴使能（伺服上电）失败:\"" + _teachAxisR.Name + "\"，错误信息:" + ei);
                            break;
                        }

                    } while (false);


                ///尝试使能拓展示教轴
                foreach (JFDevChannel chn in _extendAxes)
                    do
                    {
                        string ei = null;
                        if (!chn.OpenDev(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine(" 打开拓展示教轴:\"" + chn.Name + "\"失败：" + ei);
                            break;
                        }
                        if (!chn.CheckAvalid(out ei)) //检查通道有效性
                        {
                            isOK = false;
                            sbError.AppendLine(" 打开拓展示教轴:\"" + chn.Name + "\"失败：" + ei);
                            break;
                        }

                        if (!chn.EnabledChannel(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("使能拓展示教轴:\"" + chn.Name + "\"失败：" + ei);
                            break;
                        }
                    } while (false);


                ///尝试打开/使能配置轴
                foreach (JFDevChannel chn in _cfgAxes)
                    do
                    {
                        string ei = null;
                        if (!chn.OpenDev(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine(" 打开配置参数轴:\"" + chn.Name + "\"失败：" + ei);
                            break;
                        }
                        if (!chn.CheckAvalid(out ei)) //检查通道有效性
                        {
                            isOK = false;
                            sbError.AppendLine(" 打开配置参数轴:\"" + chn.Name + "\"失败：" + ei);
                            break;
                        }

                        if (!chn.EnabledChannel(out ei))
                        {
                            isOK = false;
                            sbError.AppendLine("使能配置参数轴:\"" + chn.Name + "\"失败：" + ei);
                            break;
                        }
                    } while (false);


            } while (false);
            if (!isOK)
                errInfo = sbError.ToString();

            return isOK;



            
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~JFSingleCmrAssist()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
