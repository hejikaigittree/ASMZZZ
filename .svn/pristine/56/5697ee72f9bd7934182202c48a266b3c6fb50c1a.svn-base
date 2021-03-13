using System;
using System.Collections.Generic;
using System.IO;
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
    /// <summary>
    /// 景焱HTM光源触发板
    /// </summary>
    [JFVersion("1.0.0.0")]
    [JFDisplayName("景焱HTM光源触发板")]
    public class HtmLightTrig : IJFDevice_TrigController,IJFRealtimeUIProvider
    {
        internal HtmLightTrig()
        {
            IsDeviceOpen = false;
            IsInitOK = false;
            CreateInitParamDescribes();
        }

  

        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            Unsupported = -1,//设备不支持此功能
            ParamError = -2,//参数错误（不支持的参数）
            InvokeFailed = -3,//库函数调用出错
            Allowed = 1,//调用成功，但不是所有的参数都支持
            InitFailedWhenOpen = -4,//
            NotOpen = -5, //卡未打开
        }
        public string GetErrorInfo(int errorCode)
        {
            string ret = "Unknown-Error";
            switch (errorCode)
            {
                case (int)ErrorDef.Success://操作成功，无错误
                    ret = "Success";
                    break;
                case (int)ErrorDef.Unsupported://设备不支持此功能
                    ret = "Unsupported";
                    break;
                case (int)ErrorDef.ParamError://参数错误（不支持的参数）
                    ret = "Param Error";
                    break;
                case (int)ErrorDef.InvokeFailed://库函数调用出错
                    ret = "Inner API invoke failed";
                    break;
                case (int)ErrorDef.Allowed://调用成功，但不是所有的参数都支持
                    ret = "Allowed,Not all param are supported";
                    break;
                case (int)ErrorDef.NotOpen:
                    ret = "Device not open";
                    break;
                default://未定义的错误类型
                    break;
            }

            return ret;
        }



        /// <summary>获取初始化需要的所有参数的名称 </summary>
        //public string[] InitParamNames { get; } = new string[] {
        //                    "配置文件",
        //                    "使用凌华控制卡",
        //                    "使用HTM控制卡",
        //                    "板卡打开模式",
        //                    "最大轴数",
        //                    "最大IO数量",
        //                    "其他设备数量",
        //                    "模块数量",
        //                    "设备序号"};

        public string[] InitParamNames
        {
            get
            {
                if (null == initParamDescribes)
                    return null;
                return initParamDescribes.Keys.ToArray();
            }
        }

        
        /// 初始化参数描述信息
        SortedDictionary<string, JFParamDescribe> initParamDescribes = null;
        object[] yesnoRange = new object[2] { "是", "否" };
        //0 - 在线模式(初始化板卡 + 配置)，1 - 脱机模式，2 - 仅初始化板卡不配置参数
        object[] openModeRange = new object[3] { "在线模式(初始化板卡 + 配置)","脱机模式",  "仅初始化板卡不配置参数" };
        void CreateInitParamDescribes()
        {
            if (null == initParamDescribes)
                initParamDescribes = new SortedDictionary<string, JFParamDescribe>();
            initParamDescribes.Clear();
            JFParamDescribe pd = JFParamDescribe.Create("配置文件",typeof(string), JFValueLimit.FilePath, null, "HTM板卡库配置文件");
            initParamDescribes.Add("配置文件", pd);
            
            pd = JFParamDescribe.Create("使用凌华控制卡",typeof(string), JFValueLimit.Options, yesnoRange, "是否使用凌华控制卡");
            initParamDescribes.Add("使用凌华控制卡", pd);
            pd = JFParamDescribe.Create("使用HTM控制卡",typeof(string), JFValueLimit.Options, yesnoRange, "是否使用HTM控制卡");
            initParamDescribes.Add("使用HTM控制卡", pd);
            pd = JFParamDescribe.Create("板卡打开模式",typeof(string), JFValueLimit.Options, openModeRange);
            initParamDescribes.Add("板卡打开模式", pd);
            object[] minValueZero = new object[] { 0 };
            pd = JFParamDescribe.Create("最大轴数", typeof(int), JFValueLimit.MinLimit, minValueZero);
            initParamDescribes.Add("最大轴数", pd);
            pd = JFParamDescribe.Create("最大IO数量", typeof(int), JFValueLimit.MinLimit, minValueZero);
            initParamDescribes.Add("最大IO数量", pd);
            pd = JFParamDescribe.Create("其他设备数量", typeof(int), JFValueLimit.MinLimit, minValueZero);
            initParamDescribes.Add("其他设备数量", pd);
            pd = JFParamDescribe.Create("模块数量", typeof(int), JFValueLimit.MinLimit, minValueZero);
            initParamDescribes.Add("模块数量", pd);
            pd = JFParamDescribe.Create("设备序号", typeof(int), JFValueLimit.MinLimit, minValueZero);
            initParamDescribes.Add("设备序号", pd);

        }





        void _CheckInitName(string initName, string funcName)
        {
            if (null == initName)
                throw new ArgumentNullException(string.Format("{0} failed By: name = null! ", funcName));
            if (!InitParamNames.Contains(initName))
                throw new ArgumentException(string.Format("{0} failed By: name = {1} is not included by InitParamNames:{2}", funcName, initName, string.Join("|", InitParamNames)));
        }




        public JFParamDescribe GetInitParamDescribe(string name)
        {
            _CheckInitName(name, "GetInitParamDescribe(name)");
            return initParamDescribes[name];

        }


        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public object GetInitParamValue(string name)
        {
            _CheckInitName(name, "GetInitParamDescribe(name)");
            if (name == "配置文件")
                return config_file;
            else if (name == "使用凌华控制卡")
                return use_aps_card;
            else if (name == "使用HTM控制卡")
                return use_htnet_card;
            else if (name == "板卡打开模式")
                return offline_mode;
            else if (name == "最大轴数")
                return max_axis_num;
            else if (name == "最大IO数量")
                return max_io_num;
            else if (name == "其他设备数量")
                return max_dev_num;
            else if (name == "模块数量")
                return max_module_num;
            else if (name == "设备序号")
                return devIndex;
            else
                throw new Exception("GetInitParamValue(name) failed By: name = " + name);
        }

        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool SetInitParamValue(string name, object value)
        {
            _CheckInitName(name, "SetInitParamValue(name, value)");
            if (null == value)
                throw new Exception("SetInitParamValue(name, value) failed By: value = null");

            if (!GetInitParamDescribe(name).ParamType.IsAssignableFrom(value.GetType()))
                throw new ArgumentException(string.Format("SetInitParamValue(name = {0}, value) faile By: value's Type = {1} can't Assignable to InitParam's Type:{2}", name, value.GetType(), GetInitParamDescribe(name).ParamType.Name));

            if (name == "配置文件")
            {
                string tmp = value as string;
                if (!File.Exists(tmp))
                {
                    _initErrorInfo = string.Format("设置初始化参数\"配置文件\"失败，文件{0}不存在！", tmp);
                    config_file = null;
                    return false;
                }
                config_file = tmp;
                _initErrorInfo = "Success";
                return true;
            }
            else if (name == "使用凌华控制卡")
            {
                string tmp = value as string;
                if (!yesnoRange.Contains(tmp))
                {
                    _initErrorInfo = string.Format("设置初始化参数\"使用凌华控制卡\"失败，\"{0}\"不是合法参数：[{1},{2}]", tmp,yesnoRange[0],yesnoRange[1]);
                    use_aps_card = null;
                    return false;
                }
                use_aps_card = tmp;
                _initErrorInfo = "Success";
                return true;
            }
            else if (name == "使用HTM控制卡")
            {
                string tmp = value as string;
                if (!yesnoRange.Contains(tmp))
                {
                    _initErrorInfo = string.Format("设置初始化参数\"使用HTM控制卡\"失败，\"{0}\"不是合法参数：[{1},{2}]", tmp, yesnoRange[0], yesnoRange[1]);
                    use_htnet_card = null;
                    return false;
                }
                use_htnet_card = tmp;
                _initErrorInfo = "Success";
                return true;
            }
            else if (name == "板卡打开模式")
            {
                string tmp = value as string;
                if (!openModeRange.Contains(tmp))
                {
                    _initErrorInfo = string.Format("设置初始化参数\"板卡打开模式\"失败， \"{0}\"不是合法的参数！可选值:{1}|{2}|{3}", tmp,openModeRange[0],openModeRange[1],openModeRange[2]);
                    offline_mode = null;
                    return false;
                }
                _initErrorInfo = "Success";
                offline_mode = tmp;
                return true;
            }
            else if (name == "最大轴数")
            {
                int ma = (int)value;
                if (ma < 0)
                {
                    _initErrorInfo = string.Format("设置初始化参数\"最大轴数 = {0}\"失败，轴数量不能为负数", ma);
                    max_axis_num = null;
                    return false;
                }
                _initErrorInfo = "Success";
                max_axis_num = ma;
                return true;
            }
            else if (name == "最大IO数量")
            {
                int mo = (int)value;
                if (mo < 0)
                {
                    _initErrorInfo = string.Format("设置初始化参数\"最大IO数量 = {0}\"失败，IO数量不能为负数", mo);
                    max_io_num = null;
                    return false;
                }
                _initErrorInfo = "Success";
                max_io_num = mo;
                return true;
            }
            else if (name == "其他设备数量")
            {
                int md = (int)value;
                if (md < 0)
                {
                    _initErrorInfo = string.Format("设置初始化参数\"其他设备数量 = {0}\"失败，设备数量不能为负数", md);
                    max_dev_num = null;
                    return false;
                }
                _initErrorInfo = "Success";
                max_dev_num = md;
                return true;
            }
            else if (name == "模块数量")
            {
                int mm = (int)value;
                if (mm < 0)
                {
                    _initErrorInfo = string.Format("设置初始化参数\"模块数量 = {0}\"失败，模块数量不能为负数", mm);
                    max_module_num = null;
                    return false;
                }
                _initErrorInfo = "Success";
                max_module_num = mm;
                return true;
            }
            else if (name == "设备序号")
            {
                int mm = (int)value;
                if (mm < 0)
                {
                    _initErrorInfo = string.Format("设置初始化参数\"设备序号 = {0}\"失败，设备序号不能为负数", mm);
                    devIndex = null;
                    return false;
                }
                _initErrorInfo = "Success";
                devIndex = mm;
                return true;
            }
            else
                throw new Exception("GetInitParamValue(name) failed By: name = " + name);
        }

        bool _InitParamEqualExisted() //内外参数设置一样一样的
        {
            if (null == config_file || null == use_aps_card || use_htnet_card == null ||
                offline_mode == null || max_axis_num == null || max_io_num == null ||
                max_dev_num == null || max_module_num == null)
                return false;
            HTM.INIT_PARA currParam = new HTM.INIT_PARA();
            currParam.para_file = config_file;
            currParam.use_aps_card = (byte)(use_aps_card == yesnoRange[0]as string?1:0);
            currParam.use_htnet_card = (byte)(use_htnet_card == yesnoRange[0] as string ? 1 : 0);

            
            currParam.offline_mode = (byte)Array.IndexOf(openModeRange,offline_mode);

            currParam.max_axis_num = (ushort)max_axis_num;
            currParam.max_io_num = (ushort)max_io_num;

            currParam.max_dev_num = (ushort)max_dev_num;

            currParam.max_module_num = (ushort)max_module_num;
            return HtmDllManager.InitParamEqualExisted(currParam);
        }


        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool Initialize()
        {
            bool ret = false;
            //string errorInfo = "Unknown-Error!";
            lock (HtmDllManager.AsynLocker)
            {
                if (IsDeviceOpen)
                    CloseDevice();
                do
                {
                    if (null == config_file)
                    {
                        _initErrorInfo = "初始化参数:\"配置文件\"未设置！";
                        break;
                    }
                    if (use_aps_card == null)
                    {
                        _initErrorInfo = "初始化参数:\"使用凌华控制卡\"未设置！"; ;
                        break;
                    }
                    if (use_htnet_card == null)
                    {
                        _initErrorInfo = "初始化参数:\"使用HTM控制卡\"未设置！";
                        break;
                    }
                    if (offline_mode == null)
                    {
                        _initErrorInfo = "初始化参数:\"板卡打开模式\"未设置！";
                        break;
                    }
                    if (max_axis_num == null)
                    {
                        _initErrorInfo = "初始化参数:\"最大轴数\"未设置！";
                        break;
                    }
                    if (max_io_num == null)
                    {
                        _initErrorInfo = "初始化参数:\"最大IO数量\"未设置！";
                        break;
                    }
                    if (max_dev_num == null)
                    {
                        _initErrorInfo = "初始化参数:\"其他设备数量\"未设置！";
                        break;
                    }
                    if (max_module_num == null)
                    {
                        _initErrorInfo = "初始化参数:\"模块数量\"未设置！";
                        break;
                    }

                    if(null == devIndex)
                    {
                        _initErrorInfo = "初始化参数:\"设备序号\"未设置！";
                        break;
                    }
                    lock (HtmDllManager.AsynLocker)
                    {
                        if (HtmDllManager.IsHtmParamInited())//HTM控制卡已打开
                        {
                            if (!_InitParamEqualExisted())//当前预设参数和已打开的控制卡参数不同
                            {
                                _initErrorInfo = "HtmLightTrig初始化参数和已打开的HTM库中的初始化参数不同！";
                                break;
                            }
                        }
                        if(HtmDllManager.LightTrigInited.Contains((int)devIndex))
                        {
                            _initErrorInfo = "系统中已存在DevIndex = " + devIndex + " 的光源触发板对象！";
                            break;
                        }

                        HTM.INIT_PARA currParam = new HTM.INIT_PARA();
                        currParam.para_file = config_file;
                        currParam.use_aps_card = (byte)(use_aps_card == yesnoRange[0] as string ? 1 : 0);
                        currParam.use_htnet_card = (byte)(use_htnet_card == yesnoRange[0] as string ? 1 : 0);


                        currParam.offline_mode = (byte)Array.IndexOf(openModeRange, offline_mode);

                        currParam.max_axis_num = (ushort)max_axis_num;
                        currParam.max_io_num = (ushort)max_io_num;

                        currParam.max_dev_num = (ushort)max_dev_num;

                        currParam.max_module_num = (ushort)max_module_num;
                        HtmDllManager.SetInitParam(currParam);

                        HtmDllManager.LightTrigInited.Add((int)devIndex);
                    }
                    ret = true;
                    _initErrorInfo = "Success";
                } while (false);
            }
            IsInitOK = ret;
            return ret;
        }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get; private set; }

        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo()
        {
            return _initErrorInfo;
        }

        //INIT_PARA initParam;// = new INIT_PARA();
        ///HTM控制器初始化参数
        string config_file = null;
        string use_aps_card = null;
        string use_htnet_card = null;
        string offline_mode = null;
        int? max_axis_num = null;
        int? max_io_num = null;
        int? max_dev_num = null;
        int? max_module_num = null;

        ///设备序号 ， 光源触发板在HTM控制器配置中的序号
        int? devIndex = null;


        //int? language = null;
        string _initErrorInfo = "NO-OPS";


        object asynLocker = new object();//线程同步对象

        public string DeviceModel { get { return "HtmLightTriggerBoard"; } }
        /// <summary>
        /// 打开设备
        /// </summary>
        public int OpenDevice()
        {
            lock (asynLocker)
            {
                if (!IsInitOK)
                    return (int)ErrorDef.InitFailedWhenOpen;

                if (IsDeviceOpen)
                    return (int)ErrorDef.Success;
                HTM.INIT_PARA initParam = new HTM.INIT_PARA();
                initParam.para_file = config_file;
                initParam.use_aps_card = (byte)(Array.IndexOf(yesnoRange,use_aps_card)==0?1:0);
                initParam.use_htnet_card = (byte)(Array.IndexOf(yesnoRange, use_htnet_card) == 0 ? 1 : 0);
                initParam.offline_mode = (byte)(Array.IndexOf(openModeRange,offline_mode));
                initParam.max_axis_num = (ushort)max_axis_num;
                initParam.max_io_num = (ushort)max_io_num;
                initParam.max_dev_num = (ushort)max_dev_num;
                initParam.max_module_num = (ushort)max_module_num;
                initParam.language = 0;
                int nErr = 0;
                lock (HtmDllManager.AsynLocker)
                {
                    if (HtmDllManager.OpendDevCount == 0)
                    {
                        HTM.Discard();
                        nErr = HTM.Init(ref initParam);
                        if (nErr < 0)
                            return (int)ErrorDef.InvokeFailed;
                    }
                    HtmDllManager.OpendDevCount++;//HtmDllManager.LightTrigOpend++;
                    IsDeviceOpen = true;
                }
                
                nErr = HTM.SetLightTrigSrc((int)devIndex, 0) ;//关闭所有通道
                if (0 != nErr)
                    throw new Exception("Turn off all channels failed when open HtmLightTrig devIndex = " + devIndex);
                for (int i = 0; i < TrigChannelCount; i++)
                {
                    srcChannels[i] = 0;
                    channelEnables[i] = false;
                }
                

                nErr = HTM.SetLightTrigTime((int)devIndex, 0);
                if (0 != nErr)
                    throw new Exception("SetLightTrigTime(devIndex, 0) failed when open HtmLightTrig devIndex = " + devIndex);
                trigDuration = 0;
                IsDeviceOpen = true;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        public int CloseDevice()
        {
            lock (asynLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.Success;

                lock (HtmDllManager.AsynLocker)
                {
                    HTM.SetLightTrigSrc((int)devIndex, 0);//关闭所有通道
                    for (int i = 0; i < TrigChannelCount; i++)
                        channelEnables[i] = false;
                    HTM.SetLightTrigTime((int)devIndex, 0);
                    HtmDllManager.OpendDevCount--;
                    if (HtmDllManager.OpendDevCount == 0)
                        HTM.Discard();
                }
                IsDeviceOpen = false;
                return (int)ErrorDef.Success;

            }

        }

        /// <summary>
        /// 设备是否已经打开
        /// </summary>
        public bool IsDeviceOpen { get; private set; }




        public int TrigChannelCount { get { return 1; } } //景焱光源触发板，软件接口只支持一路 ， 每块触发板分为4个Device
        bool[] channelEnables = new bool[] { false };
        int[] srcChannels = new int[] { 0 }; //绑定的输入通道位掩码
        /// <summary>获取通道使能状态</summary>
        public int GetTrigChannelEnable(int channel, out bool isEnabled)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelEnable(channel ,...) failed By:channel = " + channel+"  is out of range 0~" + (TrigChannelCount-1));
            lock (asynLocker)
            {
                isEnabled = false;
                if (!IsDeviceOpen)
                    return (int)ErrorDef.Success;
                isEnabled = channelEnables[channel];
                return (int)ErrorDef.Success;
            }
        }
        /// <summary>设置通道使能状态</summary>
        public int SetTrigChannelEnable(int channel, bool isEnable)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("SetTrigChannelEnable(channel ,...) failed By:channel = " + channel + "  is out of range 0~" + (TrigChannelCount - 1));
            lock (asynLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                if (isEnable == channelEnables[channel])
                    return (int)ErrorDef.Success;
                int opt = 0;
                if(isEnable)
                    opt = HTM.SetLightTrigSrc((int)devIndex, (HTM.LightTrigSrc)srcChannels[channel]);
                else
                    opt = HTM.SetLightTrigSrc((int)devIndex, HTM.LightTrigSrc.NONE);
                if (0 != opt)
                        return (int)ErrorDef.InvokeFailed;
                    channelEnables[channel] = isEnable;
                
                return (int)ErrorDef.Success;
            }
        }

        public int GetTrigChannelEnables(out bool[] isEnables)
        {
            isEnables = new bool[] { false};
            lock (asynLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.Success;
                channelEnables.CopyTo(isEnables, 0);   
                return (int)ErrorDef.Success;
            }
        }

        public int SetTrigChannelEnables(int[] channels, bool[] isEnables)
        {
            if (null == channels || null == isEnables)
                throw new ArgumentNullException("SetTrigChannelEnables(int[] channels, bool[] isEnables) failed By: channels == null or isEnables = null");
            if(channels.Count() != isEnables.Count())
                throw new ArgumentException("SetTrigChannelEnables(int[] channels, bool[] isEnables) failed By: channels.Count:" +channels.Count() + " != isEnables.Count:" + isEnables.Count());
            if (channels.Count()==0 || channels.Count() > TrigChannelCount)
                throw new ArgumentOutOfRangeException("SetTrigChannelEnables(int[]channel ,...) failed By:channels.count = " + channels.Count() + "  is out of range 1~" + (TrigChannelCount - 1));

            return SetTrigChannelEnable(0, isEnables[0]);
        }



        /// <summary>
        /// 触发源输入通道数量
        /// </summary>
        public int TrigSrcChannelCount { get { return 4; } } //新添加

        /// <summary>
        /// 设置（触发）输出通道的输入源（通道）
        /// </summary>
        /// <param name="trigChannel">输出通道号</param>
        /// <param name="srcMsak">输入通道位掩码</param>
        /// <returns>如果设备不支持此功能，则返回Unsupport</returns>
        public int GetTrigChannelSrc(int trigChannel, out int srcMask)
        {
            if(trigChannel < 0 || trigChannel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelSrc(channel ,...) failed By:channel = " + trigChannel + "  is out of range 0~" + (TrigChannelCount - 1));
            srcMask = srcChannels[trigChannel];
            return (int)ErrorDef.Success;
        }

        public int SetTrigChannelSrc(int trigChannel, int srcMask)
        {
            if (trigChannel < 0 || trigChannel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("SetTrigChannelSrc(channel ,...) failed By:channel = " + trigChannel + "  is out of range 0~" + (TrigChannelCount - 1));
            
            int sm = srcMask & 0xf;
            if (IsDeviceOpen && channelEnables[trigChannel])
            {
                int opt = HTM.SetLightTrigSrc((int)devIndex, (HTM.LightTrigSrc)sm);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
            }
            
            srcChannels[trigChannel] = srcMask & 0xf;
            return (int)ErrorDef.Success;
        }

        /// <summary>获取通道触发强度</summary>
        public int GetTrigChannelIntensity(int channel, out int intensity)
        {
            intensity = 0;
            return (int)ErrorDef.Unsupported;
        }
        /// <summary>设置通道触发强度</summary>
        public int SetTrigChannelIntensity(int channel, int intensity)
        {
            return (int)ErrorDef.Unsupported;
        }

        int trigDuration = 0;//触发时长()

        /// <summary>获取通道触发时长</summary>
        public int GetTrigChannelDuration(int channel, out int duration)
        {
            duration = 0;
            if(channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelDuration(channel ,...) failed By:channel = " + channel + "  is out of range 0~" + (TrigChannelCount - 1));
            lock (asynLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                duration = trigDuration;
                return (int)ErrorDef.Success;
            }
        }
        /// <summary>设置通道触发时长</summary>
        public int SetTrigChannelDuration(int channel, int duration)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelDuration(channel ,...) failed By:channel = " + channel + "  is out of range 0~" + (TrigChannelCount - 1));
            if (duration < 0)
                return (int)ErrorDef.ParamError;
            lock (asynLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;

                int opt = HTM.SetLightTrigTime((int)devIndex, duration);
                if (0 == opt)
                {
                    trigDuration = duration;
                    return (int)ErrorDef.Success;
                }
                return (int)ErrorDef.InvokeFailed;
            }
        }


        /// <summary>设置所有通道触发强度</summary>
        public int SetTrigIntensity(int intensity)
        {
            return (int)ErrorDef.Unsupported;
        }
        /// <summary>设置所有通道触发时长</summary>
        public int SetTrigDuration(int duration)
        {
            if (duration < 0)
                return (int)ErrorDef.ParamError;
            lock (asynLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;

                int opt = HTM.SetLightTrigTime((int)devIndex, duration);
                if (0 == opt)
                {
                    trigDuration = duration;
                    return (int)ErrorDef.Success;
                }
                return (int)ErrorDef.InvokeFailed;
            }
        }

        public int SoftwareTrigAll()
        {
            //lock (asynLocker)
            //{
            //    if (!IsDeviceOpen)
            //        return (int)ErrorDef.NotOpen;

            //    int opt = HTM.SWLightTrig((int)devIndex);
            //    if (0 != opt)
            //        return (int)ErrorDef.InvokeFailed;
            //    return (int)ErrorDef.Success;
            //}
            return SoftwareTrigChannel(0);
        }

        public int SoftwareTrigChannel(int channel)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("SoftwareTrigChannel(int channel,...) failed by channel = " + channel + " is out of range 0~" + (TrigChannelCount - 1));
            lock (asynLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;

                int opt = HTM.SWLightTrig((int)devIndex);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
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

        public JFRealtimeUI GetRealtimeUI()
        {
            UcTrigCtrl ui = new UcTrigCtrl();
            ui.SetModuleInfo(this);
            return ui;
        }

        ~HtmLightTrig()
        {
            Dispose(false);
        }
    }
}
