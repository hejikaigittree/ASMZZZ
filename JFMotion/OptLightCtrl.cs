using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HT_Lib;
using JFInterfaceDef;
using JFToolKits;
using JFUI;

namespace JFHTMDevice
{
    /// <summary>OPT-DPA1024E-X光源控制器基类</summary>
    public abstract class OptLightCtrlBase : IJFDevice_LightControllerWithTrig,IJFRealtimeUIProvider
    {

        internal OptLightCtrlBase()
        {
            IsInitOK = false;
            IsDeviceOpen = false;
            CreateInitParamDescribes();
            chnEnables = new bool[LightChannelCount];
            int i = 0;
            for (i = 0; i < LightChannelCount; i++)
                chnEnables[i] = false;

        }

        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            Unsupported = -1,//设备不支持此功能
            ParamError = -2,//参数错误（不支持的参数）
            InvokeFailed = -3,//库函数调用出错
            Allowed = 1,//调用成功，但不是所有的参数都支持
            InitFailedWhenOpen = -4,//
            NotOpen = -5, //设备未打开
            ModeError = -6,//工作模式错误 ，比如当前为触发模式，调用LightTurnOn/Off函数时会返回此值
        }
        public string GetErrorInfo(int errorCode)
        {
            switch (errorCode)
            {
                case (int)ErrorDef.Success://操作成功，无错误
                    return "Success";
                case (int)ErrorDef.Unsupported://设备不支持此功能
                    return "Unsupported";
                case (int)ErrorDef.ParamError://参数错误（不支持的参数）
                    return "Param Error";
                case (int)ErrorDef.InvokeFailed://库函数调用出错
                    return "Inner API invoke failed";
                case (int)ErrorDef.Allowed://调用成功，但不是所有的参数都支持
                    return "Allowed,Not all param are supported";
                case (int)ErrorDef.InitFailedWhenOpen:
                    return "Not initialized when open ";
                case (int)ErrorDef.NotOpen:
                    return "Device dose not open";
                case (int)ErrorDef.ModeError:
                    return "WorkMode Mismatching"; //工作模式不匹配
                default://未定义的错误类型
                    return "Unknown-ErrorCode:" + errorCode;
            }
        }

        #region  IJFInitializable'API
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public string[] InitParamNames => new string[] { "连接方式", "连接参数" };


        string linkMode = null;   //0:使用串口方式连接 1：TCP    2：使用SN连接
        string linkParam = null; //串口名 或者 IP 或者SN

        SortedDictionary<string, JFParamDescribe> paramDescribes = null;
        object[] linkModeParamRange = new object[] { "串口", "TCP", "序列号" };

        void CreateInitParamDescribes()
        {
            if (paramDescribes == null)
                paramDescribes = new SortedDictionary<string, JFParamDescribe>();
            paramDescribes.Clear();
            JFParamDescribe pd = JFParamDescribe.Create("连接方式", typeof(string), JFValueLimit.Options, linkModeParamRange, "计算机与光源控制器的连接方式");
            paramDescribes.Add("连接方式", pd);
            pd = JFParamDescribe.Create("连接参数", typeof(string), JFValueLimit.NonLimit, null, "在指定的连接方式下，使用此参数与相机建立连接");
            paramDescribes.Add("连接参数", pd);
        }
        


        /// <summary>
        /// 获取指定名称的初始化参数的类型
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数类型</returns>
        public JFParamDescribe GetInitParamDescribe(string name)
        {
            if (null == name)
                throw new ArgumentNullException("GetInitParamDescribe(name) failed by name = null");
            if(!InitParamNames.Contains(name))
                throw new ArgumentOutOfRangeException("GetInitParamDescribe(name) failed by name = " + name + " is not included in {\"连接方式\",\"连接参数\"}");

            return paramDescribes[name];


        }

        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public object GetInitParamValue(string name)
        {
            if (null == name)
                throw new ArgumentNullException("GetInitParamValue(name) failed by name = null");
            if (name == "连接方式")
                return linkMode;
            else if (name == "连接参数")
                return linkParam;
            throw new ArgumentOutOfRangeException("GetInitParamValue(name) failed by name = " + name + " is not included in {\"连接方式\",\"连接参数\"}");

        }

        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool SetInitParamValue(string name, object value)
        {
            if (null == name)
                throw new ArgumentNullException("SetInitParamValue(name) failed by name = null");
            if (!GetInitParamDescribe(name).ParamType.IsAssignableFrom(value.GetType()))
                throw new ArgumentException(string.Format("SetInitParamValue(name = {0}, value) faile By: value's Type = {1} can't Assignable to InitParam's Type:{2}", name, value.GetType(), GetInitParamDescribe(name).ParamType.Name));

            if (name == "连接方式")
            {
                if (null == value)
                {
                    linkMode = null;
                    _initError = "SetInitParamValue(name = \"连接方式\",value) failed by value = null";
                    return false;
                }
                string tmp = value as string;
                if(!linkModeParamRange.Contains(tmp))
                {
                    linkMode = null;
                    _initError = string.Format("SetInitParamValue(name = \"连接方式\",value = {0}) failed by value isnot included by legal params:\"{1}\"|\"{2}\"|\"{3}\"",tmp,linkModeParamRange[0], linkModeParamRange[1], linkModeParamRange[2]);
                    return false;
                }
                linkMode = tmp;
                _initError = "Success";
                return true;
            }
            else if (name == "连接参数")
            {
                if (string.IsNullOrEmpty((string)value))
                {
                    _initError = string.Format("SetInitParamValue(name = \"连接参数\",value) failed By:value is null or empty string");
                    return false;
                }

                linkParam = (string)value;
                _initError = "Success";
                return true;
            }
            throw new ArgumentOutOfRangeException("SetInitParamValue(name) failed by name = " + name + " is not included in {\"连接方式\",\"连接参数\"}");

        }

        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool Initialize()
        {
            if (IsDeviceOpen)
                CloseDevice();
            if (linkMode == null)
            {
                _initError = "Initialize Failed By:LinkMode is not set";
                IsInitOK = false;
                return false;
            }
            
            if (linkMode == linkModeParamRange[0] as string) //串口连接
            {
                if (JFFunctions.IsSerialPortName(linkParam))
                {
                    _initError = "Success";
                    IsInitOK = true;
                    return true;
                }
                _initError = "LinkParam = " + linkParam + " is not a legal serial-port name";
                IsInitOK = false;
                return false;
            }
            else if (linkMode == linkModeParamRange[1] as string)//TCP/IP
            {
                if (JFFunctions.IsIPAddress(linkParam))
                {
                    _initError = "Success";
                    IsInitOK = true;
                    return true;
                }
                _initError = "LinkParam = " + linkParam + " is not a legal ip address";
                IsInitOK = false;
                return false;


            }
            else if (linkMode == linkModeParamRange[2] as string) //SN
            {
                if (!string.IsNullOrWhiteSpace(linkParam))
                {
                    _initError = "Success";
                    IsInitOK = true;
                    return true;
                }
                _initError = "LinkParam =  null or whitespace when linkmode = SN";
                IsInitOK = false;
                return false;
            }
            _initError = "Initialize Failed By:LinkMode = " + linkMode + " is not included in the values{0:使用串口 1：使用TCP 2：使用SN}";
            IsInitOK = false;
            return false;
        }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get; set; }

        string _initError = "NO-OPS";
        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo()
        {
            return _initError;
        }
        #endregion //  IJFInitializable'API


        #region IJFDevice's API
        public abstract string DeviceModel { get; }//{ get { return "OPT-DPA1024E-X"; } }

        object asyncLocker = new object();
        OPTControllerAPI opt = new OPTControllerAPI();

        string linkedMode = null; //处于已打开状态的控制器的连接模式 串口/TCP/SN
        string linkedParam = null;

        /// <summary>
        /// 打开设备
        /// </summary>
        public int OpenDevice()
        {
            if (!IsInitOK)
                return (int)ErrorDef.InitFailedWhenOpen;
            lock (asyncLocker)
            {
                if (IsDeviceOpen)
                {
                    if (linkedMode == linkMode && linkedParam == linkParam)
                        return (int)ErrorDef.Success;
                    CloseDevice();
                }
                opt = new OPTControllerAPI();
                int errCode = 0;
                if (linkMode == linkModeParamRange[0] as string)
                {
                    errCode = opt.InitSerialPort(linkedParam);

                }
                else if (linkMode == linkModeParamRange[1] as string)
                {
                    errCode = opt.CreateEtheConnectionByIP(linkParam);
                }
                else// if(linkMode == linkModeParamRange[2] as string)
                    errCode = opt.CreateEtheConnectionBySN(linkParam);

                if (errCode != OPTControllerAPI.OPT_SUCCEED)
                {
                    //关闭打开的设备
                    if (linkMode == linkModeParamRange[0] as string)
                        opt.ReleaseSerialPort(); 
                    else// if (linkMode == linkModeParamRange[1] as string)
                        opt.DestoryEtheConnect();
                    //else// if(linkMode == linkModeParamRange[2] as string)
                    //    errCode = opt.CreateEtheConnectionBySN(linkParam);
                    return (int)ErrorDef.InvokeFailed;
                }
                errCode = opt.TurnOffChannel(0);//关闭所有通道
                if (OPTControllerAPI.OPT_SUCCEED != errCode)
                {
                    if (linkMode == linkModeParamRange[0] as string)
                        opt.ReleaseSerialPort();
                    else// if (linkMode == linkModeParamRange[1] as string)
                        opt.DestoryEtheConnect();
                    //else// if(linkMode == linkModeParamRange[2] as string)
                    //    errCode = opt.CreateEtheConnectionBySN(linkParam);
                    return (int)ErrorDef.InvokeFailed;
                }
                for (int i = 0; i < LightChannelCount; i++)
                    chnEnables[i] = false;

                //errCode = SetWorkMode(0);//将控制器设为开关模式
                errCode = opt.SetWorkMode(0);
                if (errCode != OPTControllerAPI.OPT_SUCCEED)
                {
                    if (linkMode == linkModeParamRange[0] as string)
                        opt.ReleaseSerialPort();
                    else// if (linkMode == linkModeParamRange[1] as string)
                        opt.DestoryEtheConnect();
                    //else// if(linkMode == linkModeParamRange[2] as string)
                    //    errCode = opt.CreateEtheConnectionBySN(linkParam);
                    return (int)ErrorDef.InvokeFailed;
                }
                linkedMode = linkMode;
                linkedParam = linkParam;
                IsDeviceOpen = true;
                return (int)ErrorDef.Success;

            }
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        public int CloseDevice()
        {
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.Success;
                int errCode = opt.TurnOffChannel(0);//关闭所有通道
                if (OPTControllerAPI.OPT_SUCCEED != errCode)
                    throw new Exception("Close all channel failed when close device");
                for (int i = 0; i < LightChannelCount; i++)
                    chnEnables[i] = false;
                if (linkedMode == linkModeParamRange[0] as string) //Linked by serialport
                    errCode = opt.ReleaseSerialPort();
                else //if (linkedMode == 1 || linkedMode == 2) //Linked by IP or SN
                    errCode = opt.DestoryEtheConnect();
                if (errCode != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;

                IsDeviceOpen = false;
                opt = null;
                return (int)ErrorDef.Success;

            }
        }

        /// <summary>
        /// 设备是否已经打开
        /// </summary>
        public bool IsDeviceOpen { get; private set; }

        #endregion //IJFDevice's API

        #region IJFUIProvider's API
        /// <summary>获取一个（新创建的）UI控件</summary>
        public JFRealtimeUI GetRealtimeUI()
        {
            UcLightCtrl_T ui = new UcLightCtrl_T();
            ui.SetModule(this, null, null);
            return ui;
        }

        /// <summary>显示一个对话框窗口</summary>
        public int ShowCfgDialog(bool isModalDialog)
        {
            return (int)ErrorDef.Unsupported;
        }
        #endregion //IJFUIProvider's API

        #region LightControl's API
        public abstract int LightChannelCount { get; } //{ get { return 16; } }
        bool[] chnEnables = null;// = new bool[16];

        /// <summary>获取通道开关状态</summary>
        public int GetLightChannelEnable(int channel, out bool isTurnOn)
        {
            isTurnOn = false;

            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                JFLightWithTrigWorkMode workMode = 0;
                if ((int)ErrorDef.Success != GetWorkMode(out workMode))
                    return (int)ErrorDef.InvokeFailed;
                if (workMode != JFLightWithTrigWorkMode.TurnOnOff) //0：开关模式
                    return (int)ErrorDef.ModeError;
                isTurnOn = chnEnables[channel];
                return (int)ErrorDef.Success;

            }

        }
        /// <summary>设置通道开关</summary>
        public int SetLightChannelEnable(int channel, bool isTurnOn)
        {
            if (channel < 0 || channel >= LightChannelCount)
                throw new ArgumentOutOfRangeException("SetLightChannelEnable(int channel = " + channel + " is out of range 0~" + (LightChannelCount - 1));
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                JFLightWithTrigWorkMode workMode = 0;
                if ((int)ErrorDef.Success != GetWorkMode(out workMode))
                    return (int)ErrorDef.InvokeFailed;
                if (workMode != JFLightWithTrigWorkMode.TurnOnOff) //0:开关模式 1:触发模式
                    return (int)ErrorDef.ModeError;

                int errCode = 0;
                if (isTurnOn)
                    errCode = opt.TurnOnChannel(channel + 1);
                else
                    errCode = opt.TurnOffChannel(channel + 1);
                if (errCode != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                chnEnables[channel] = isTurnOn;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>获取所有通道的开关状态</summary>
        public int GetLightChannelEnables(out bool[] isTurnOns)
        {
            isTurnOns = null;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                JFLightWithTrigWorkMode workMode = 0;
                if ((int)ErrorDef.Success != GetWorkMode(out workMode))
                    return (int)ErrorDef.InvokeFailed;
                if (workMode != JFLightWithTrigWorkMode.TurnOnOff) //0:开关模式 1:触发模式
                    return (int)ErrorDef.ModeError;

                isTurnOns = new bool[LightChannelCount];
                Array.Copy(chnEnables, 0, isTurnOns, 0, LightChannelCount);
                return (int)ErrorDef.Success;
            }

        }
        /// <summary>一次设置多个通道的开关状态</summary>
        public int SetLightChannelEnables(int[] channels, bool[] isTurnOns)
        {
            if (null == channels || isTurnOns == null)
                throw new ArgumentNullException("SetLightChannelEnables(channels,isTurnOns) failed By: null == channels || isTurnOns == null");
            if (channels.Length == 0 || channels.Length != isTurnOns.Length)
                throw new ArgumentException("SetLightChannelEnables(channels,isTurnOns) failed By:channels.Length == 0 || channels.Length != isTurnOns.Length");
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                JFLightWithTrigWorkMode workMode = 0;
                if ((int)ErrorDef.Success != GetWorkMode(out workMode))
                    return (int)ErrorDef.InvokeFailed;
                if (workMode != JFLightWithTrigWorkMode.TurnOnOff) //0:开关模式 1:触发模式
                    return (int)ErrorDef.ModeError;

                List<int> needEnableChannels = new List<int>();
                List<int> needDisableChannels = new List<int>();
                int i = 0;
                for (i = 0; i < channels.Count(); i++)
                    if (isTurnOns[i] != chnEnables[channels[i]])
                    {
                        if (isTurnOns[i])
                            needEnableChannels.Add(channels[i] + 1); //OPT的通道号从1开始
                        else
                            needDisableChannels.Add(channels[i] + 1);

                    }

                int errRet = 0;
                if (needEnableChannels.Count > 0)
                {
                    errRet = opt.TurnOnMultiChannel(needEnableChannels.ToArray(), needEnableChannels.Count);
                    if (errRet != OPTControllerAPI.OPT_SUCCEED)
                        return (int)ErrorDef.InvokeFailed;
                    for (i = 0; i < needEnableChannels.Count; i++)
                        chnEnables[needEnableChannels[i] - 1] = true;
                }
                if (needDisableChannels.Count > 0)
                {
                    errRet = opt.TurnOffMultiChannel(needDisableChannels.ToArray(), needDisableChannels.Count);
                    if (errRet != OPTControllerAPI.OPT_SUCCEED)
                        return (int)ErrorDef.InvokeFailed;
                    for (i = 0; i < needDisableChannels.Count; i++)
                        chnEnables[needDisableChannels[i] - 1] = false;
                }
                return (int)ErrorDef.Success;
            }
        }



        /// <summary>获取通道的亮度值</summary>
        public int GetLightIntensity(int channel, out int intensity)
        {
            if (channel < 0 || channel >= LightChannelCount)
                throw new ArgumentOutOfRangeException("GetLightIntensity(int channel...) failed By: channel = " + channel + " out of range");
            intensity = 0;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                int errCode = opt.ReadIntensity(channel + 1, ref intensity);
                if (errCode != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }

        }
        /// <summary>设置通道的亮度值</summary>
        public int SetLightIntensity(int channel, int intensity)
        {
            if (channel < 0 || channel >= LightChannelCount)
                throw new ArgumentOutOfRangeException("SetLightIntensity(int channel...) failed By: channel = " + channel + " out of range");
            if (intensity < 0 || intensity > 255)
                return (int)ErrorDef.ParamError;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;

                int errCode = opt.SetIntensity(channel + 1, intensity);
                if (errCode != OPTControllerAPI.OPT_SUCCEED)
                {
                    if (errCode != OPTControllerAPI.OPT_ERR_PARAM_OUTRANGE)
                        return (int)ErrorDef.ParamError;
                    return (int)ErrorDef.InvokeFailed;
                }
                return (int)ErrorDef.Success;
            }
        }

        #endregion //LightControl's API

        public abstract int TrigChannelCount { get; }//{ get { return 4; } } //触发端口只有4路

        public int TrigSrcChannelCount { get { return TrigChannelCount; } } //新添加

        /// <summary>
        /// 设置（触发）输出通道的输入源（通道）
        /// </summary>
        /// <param name="trigChannel">输出通道号</param>
        /// <param name="srcMask">输入通道位掩码</param>
        /// <returns>如果设备不支持此功能，则返回Unsupport</returns>
        public int GetTrigChannelSrc(int trigChannel, out int srcMask) //新添加
        {
            if (trigChannel < 0 || trigChannel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelSrc(trigChannel,...) failed by trigChannel = " + trigChannel + " is out of range:0~" + (TrigChannelCount - 1));
            srcMask = 1 << trigChannel;
            return (int)ErrorDef.Success;
        }

        public int SetTrigChannelSrc(int trigChannel, int srcMask)//
        {
            return (int)ErrorDef.Unsupported;
        }

        /// <summary>获取通道使能状态</summary>
        public int GetTrigChannelEnable(int channel, out bool isEnabled)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelBandInputs(int channel...) failed by channel = " + channel + " is out of range 0~" + (TrigChannelCount - 1));
            isEnabled = false;

            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                JFLightWithTrigWorkMode workMode = 0;
                if ((int)ErrorDef.Success != GetWorkMode(out workMode))
                    return (int)ErrorDef.InvokeFailed;
                if (workMode != JFLightWithTrigWorkMode.Trigger) //0:开关模式 1:触发模式
                    return (int)ErrorDef.ModeError;

                isEnabled = chnEnables[channel];
                return (int)ErrorDef.Success;

            }
        }
        /// <summary>设置通道使能状态</summary>
        public int SetTrigChannelEnable(int channel, bool isEnable)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("SetTrigChannelBandInputs(int channel = " + channel + " is out of range 0~" + (TrigChannelCount - 1));
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                JFLightWithTrigWorkMode workMode = 0;
                if ((int)ErrorDef.Success != GetWorkMode(out workMode))
                    return (int)ErrorDef.InvokeFailed;
                if (workMode != JFLightWithTrigWorkMode.Trigger) //0:开关模式 1:触发模式
                    return (int)ErrorDef.ModeError;

                int errCode = 0;
                if (isEnable)
                    errCode = opt.TurnOnChannel(channel + 1);
                else
                    errCode = opt.TurnOffChannel(channel + 1);
                if (errCode != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                chnEnables[channel] = isEnable;
                return (int)ErrorDef.Success;
            }
        }

        public int GetTrigChannelEnables(out bool[] isEnables)
        {
            isEnables = null;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                JFLightWithTrigWorkMode workMode = 0;
                if ((int)ErrorDef.Success != GetWorkMode(out workMode))
                    return (int)ErrorDef.InvokeFailed;
                if (workMode != JFLightWithTrigWorkMode.Trigger) //0:开关模式 1:触发模式
                    return (int)ErrorDef.ModeError;

                isEnables = new bool[TrigChannelCount];
                Array.Copy(chnEnables, 0, isEnables, 0, TrigChannelCount);
                return (int)ErrorDef.Success;
            }
        }

        public int SetTrigChannelEnables(int[] channels, bool[] isEnables)
        {
            if (null == channels || isEnables == null)
                throw new ArgumentNullException("SetTrigChannelEnables(channels,isEnables) failed By: null == channels || isEnables == null");
            if (channels.Length == 0 || channels.Length != isEnables.Length)
                throw new ArgumentException("SetTrigChannelEnables(channels,isEnables) failed By:channels.Length == 0 || channels.Length != isEnables.Length");
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                JFLightWithTrigWorkMode workMode = 0;
                if ((int)ErrorDef.Success != GetWorkMode(out workMode))
                    return (int)ErrorDef.InvokeFailed;
                if (workMode != JFLightWithTrigWorkMode.Trigger) //0:开关模式 1:触发模式
                    return (int)ErrorDef.ModeError;

                List<int> needEnableChannels = new List<int>();
                List<int> needDisableChannels = new List<int>();
                int i = 0;
                for (i = 0; i < channels.Count(); i++)
                    if (isEnables[i] != chnEnables[channels[i]])
                    {
                        if (isEnables[i])
                            needEnableChannels.Add(channels[i] + 1); //OPT的通道号从1开始
                        else
                            needDisableChannels.Add(channels[i] + 1);

                    }

                int errRet = 0;
                if (needEnableChannels.Count > 0)
                {
                    errRet = opt.TurnOnMultiChannel(needEnableChannels.ToArray(), needEnableChannels.Count);
                    if (errRet != OPTControllerAPI.OPT_SUCCEED)
                        return (int)ErrorDef.InvokeFailed;
                    for (i = 0; i < needEnableChannels.Count; i++)
                        chnEnables[needEnableChannels[i] - 1] = true;
                }
                if (needDisableChannels.Count > 0)
                {
                    errRet = opt.TurnOffMultiChannel(needDisableChannels.ToArray(), needDisableChannels.Count);
                    if (errRet != OPTControllerAPI.OPT_SUCCEED)
                        return (int)ErrorDef.InvokeFailed;
                    for (i = 0; i < needDisableChannels.Count; i++)
                        chnEnables[needDisableChannels[i] - 1] = false;
                }
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>获取通道触发强度</summary>
        public int GetTrigChannelIntensity(int channel, out int intensity)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelIntensity(channel ...) failed by channel = " + channel + " is out of range 0~" + (TrigChannelCount - 1));
            intensity = 0;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                int err = opt.ReadIntensity(channel + 1, ref intensity);
                if (err != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        /// <summary>设置通道触发强度</summary>
        public int SetTrigChannelIntensity(int channel, int intensity)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("SetTrigChannelIntensity(channel ...) failed by channel = " + channel + " is out of range 0~" + (TrigChannelCount - 1));
            if (intensity < 0 || intensity > 255)
                return (int)ErrorDef.ParamError;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                int err = opt.SetIntensity(channel + 1, intensity);
                if (err != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>获取通道触发时长</summary>
        public int GetTrigChannelDuration(int channel, out int duration)
        {

            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelIntensity(channel ...) failed by channel = " + channel + " is out of range 0~" + (TrigChannelCount - 1));
            duration = 0;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                int err = opt.ReadHBTriggerWidth(channel + 1, ref duration);
                if (err != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        /// <summary>设置通道触发时长</summary>
        public int SetTrigChannelDuration(int channel, int duration)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("SetTrigChannelDuration(channel ...) failed by channel = " + channel + " is out of range 0~" + (TrigChannelCount - 1));
            if (duration < 1 || duration > 500)
                return (int)ErrorDef.ParamError;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                int err = opt.SetHBTriggerWidth(channel + 1, duration);
                if (err != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }


        /// <summary>设置所有通道触发强度</summary>
        public int SetTrigIntensity(int intensity)
        {
            if (intensity < 0 || intensity > 255)
                return (int)ErrorDef.ParamError;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                int err = opt.SetIntensity(0, intensity);
                if (err != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        /// <summary>设置所有通道触发时长</summary>
        public int SetTrigDuration(int duration)
        {
            if (duration < 1 || duration > 500)
                return (int)ErrorDef.ParamError;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                int err = opt.SetHBTriggerWidth(0, duration);
                if (err != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        public int SoftwareTrigAll() //使用软触发，控制其固件必须3.3.1以上
        {
            lock (asyncLocker)
            {
                if (IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                int err = opt.SoftwareTrigger(0, 10);//时间单位为10毫秒，此处使用固定的100毫秒触发时间
                if (OPTControllerAPI.OPT_SUCCEED != err)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        public int SoftwareTrigChannel(int channel)//使用软触发，控制其固件必须3.3.1以上
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException(" SoftwareTrigChannel(int channel) failed by channel = " + channel + " is out of range 0~" + (TrigChannelCount - 1));
            lock (asyncLocker)
            {
                if (IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                int err = opt.SoftwareTrigger(channel + 1, 10);//时间单位为10毫秒，此处使用固定的100毫秒触发时间
                if (OPTControllerAPI.OPT_SUCCEED != err)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        public int SetWorkMode(JFLightWithTrigWorkMode mode)
        {
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                int modeParam = 0;
                if (mode == JFLightWithTrigWorkMode.Trigger) //opt光源控制器： 0-〉开关模式 1-〉常规亮度的触发模式 2-〉高亮度触发模式
                    modeParam = 2;

                int modeInner = 0;
                int errCode = opt.ReadWorkMode(ref modeInner);


                errCode = opt.SetWorkMode(modeParam);
                if (errCode != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        public int GetWorkMode(out JFLightWithTrigWorkMode workMode)
        {
            workMode = JFLightWithTrigWorkMode.TurnOnOff;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                //if (workMode == 1) //opt光源控制器： 0-〉开关模式 1-〉常规亮度的触发模式 2-〉高亮度触发模式
                int modeInner = 0;
                int errCode = opt.ReadWorkMode(ref modeInner);
                if (errCode != OPTControllerAPI.OPT_SUCCEED)
                    return (int)ErrorDef.InvokeFailed;
                if (modeInner == 0)
                    workMode = (int)JFLightWithTrigWorkMode.TurnOnOff;
                else// if(modeInner == 2)
                    workMode = JFLightWithTrigWorkMode.Trigger;
                return (int)ErrorDef.Success;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            ////////////释放非托管资源
            if (disposing)//////////////释放其他托管资源
            {
                CloseDevice();
            }
        }



        ~OptLightCtrlBase()
        {
            Dispose(false);
        }

    }


    [JFVersion("1.0.0.0")]
    [JFDisplayName("OPT-DPA1024E-4光源控制器")]
    public class OptLightCtrlE_4 : OptLightCtrlBase
    {

        public OptLightCtrlE_4():base()
        {

        }

        public override string DeviceModel { get { return "OPT-DPA1024E-4"; } }

        public override int LightChannelCount { get { return 4; } }

        public override int TrigChannelCount { get { return 4; } }
    }

    [JFVersion("1.0.0.0")]
    [JFDisplayName("OPT-DPA1024E-8光源控制器")]
    public class OptLightCtrlE_8 : OptLightCtrlBase
    {

        internal OptLightCtrlE_8() : base()
        {

        }

        public override string DeviceModel { get { return "OPT-DPA1024E-8"; } }

        public override int LightChannelCount { get { return 8; } }

        public override int TrigChannelCount { get { return 8; } }
    }


    [JFVersion("1.0.0.0")]
    [JFDisplayName("OPT-DPA1024E-16光源控制器")]
    public class OptLightCtrlE_16 : OptLightCtrlBase
    {

        internal OptLightCtrlE_16() : base()
        {

        }

        public override string DeviceModel { get { return "OPT-DPA1024E-16"; } }

        public override int LightChannelCount { get { return 16; } }

        public override int TrigChannelCount { get { return 16; } }
    }


}
