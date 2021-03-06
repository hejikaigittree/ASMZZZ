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
namespace JFHTMDevice
{

    //Motion and Daq Device HTM ,景焱运动控制/IO采集卡系列
    [JFVersion("1.0.0.0")]
    [JFDisplayName("景焱HTM运动控制器")]
    public class HtmMotionDaq : IJFDevice_MotionDaq
    {
        /// <summary>
        /// 景焱运动控制卡类
        /// </summary>
        /// <param name="cfgFile">配置文件(paras.db)路径</param>
        /// <param name="axisCount">轴数量</param>
        /// <param name="ioCount">DIO数量</param>
        /// <param name="mode">模式 1-脱机模式，0-在线模式(初始化板卡+配置)，2-仅初始化板卡不配置参数 </param>
        internal HtmMotionDaq()
        {
            IsInitOK = false;
            CreateInitParamDescribes();
            McCount = 0;
            DioCount = 0;
            AioCount = 0;
            //CompareTriggerCount = 0;
            dio = new HtmDio(/*this*/);
            aio = new HtmAio();
            mc = new HtmMC();
            //cmpTrig = new HtmCompareTrigger();
            cmpTrigs = new List<HtmCompareTrigger>();
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
                dio = null;
                mc = null;
            }
        }

        ~HtmMotionDaq()
        {
            Dispose(false);
        }


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
        object[] openModeRange = new object[3] { "在线模式(初始化板卡 + 配置)", "脱机模式", "仅初始化板卡不配置参数" };
        void CreateInitParamDescribes()
        {
            if (null == initParamDescribes)
                initParamDescribes = new SortedDictionary<string, JFParamDescribe>();
            initParamDescribes.Clear();
            JFParamDescribe pd = JFParamDescribe.Create("配置文件", typeof(string), JFValueLimit.FilePath, null, "HTM板卡库配置文件");
            initParamDescribes.Add("配置文件", pd);

            pd = JFParamDescribe.Create("使用凌华控制卡", typeof(string), JFValueLimit.Options, yesnoRange, "是否使用凌华控制卡");
            initParamDescribes.Add("使用凌华控制卡", pd);
            pd = JFParamDescribe.Create("使用HTM控制卡", typeof(string), JFValueLimit.Options, yesnoRange, "是否使用HTM控制卡");
            initParamDescribes.Add("使用HTM控制卡", pd);
            pd = JFParamDescribe.Create("板卡打开模式", typeof(string), JFValueLimit.Options, openModeRange);
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
            //pd = JFParamDescribe.Create("设备序号", typeof(int), JFValueLimit.MinLimit, minValueZero);
            //initParamDescribes.Add("设备序号", pd);

        }

        public bool IsInitOK { get; private set; }


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
                    _initErrorInfo = string.Format("设置初始化参数\"使用凌华控制卡\"失败，\"{0}\"不是合法参数：[{1},{2}]", tmp, yesnoRange[0], yesnoRange[1]);
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
                    _initErrorInfo = string.Format("设置初始化参数\"板卡打开模式\"失败， \"{0}\"不是合法的参数！可选值:{1}|{2}|{3}", tmp, openModeRange[0], openModeRange[1], openModeRange[2]);
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
            currParam.use_aps_card = (byte)(use_aps_card == yesnoRange[0] as string ? 1 : 0);
            currParam.use_htnet_card = (byte)(use_htnet_card == yesnoRange[0] as string ? 1 : 0);


            currParam.offline_mode = (byte)Array.IndexOf(openModeRange, offline_mode);

            currParam.max_axis_num = (ushort)max_axis_num;
            currParam.max_io_num = (ushort)max_io_num;

            currParam.max_dev_num = (ushort)max_dev_num;

            currParam.max_module_num = (ushort)max_module_num;
            return HtmDllManager.InitParamEqualExisted(currParam);
        }

        /// <summary>
        /// 初始化动作，并不打开卡，只是检查参数的合法性，打开卡的动作在Open（）函数中
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            bool ret = false;
            //string errorInfo = "Unknown-Error!";
            lock (HtmDllManager.AsynLocker)
            {
                //if (IsDeviceOpen)
                //    CloseDevice();
                do
                {
                    //if (HtmDllManager.MotionDaqCount > 0)
                    //{
                    //    _initErrorInfo = "系统中已存在HTM类型的MDD_Htm设备实例对象，不能初始化多个MDD_Htm设备对象！";
                    //    break;
                    //}
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
                    lock (HtmDllManager.AsynLocker)
                    {
                        if (HtmDllManager.IsHtmParamInited())//HTM控制卡参数已设置
                        {
                            if (!_InitParamEqualExisted())//当前预设参数和已打开的控制卡参数不同
                            {
                                _initErrorInfo = "MDD_Htm初始化参数和已打开的HTM库中的初始化参数不同！";
                                break;
                            }

                        }
                        //else
                        if(HtmDllManager.OpendDevCount > 0)
                        {
                            IsDeviceOpen = true;
                            HtmDllManager.OpendDevCount++;
                        }
                        else
                        {
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
                        }

                        //HtmDllManager.MotionDaqCount++;
                    }
                    ret = true;
                    _initErrorInfo = "Success";
                    //errorInfo = "Success";
                } while (false);
            }
            //_initErrorInfo = errorInfo;
            IsInitOK = ret;
            return ret;

        }


        public string GetInitErrorInfo() //已实现，未测试
        {
            return _initErrorInfo;
        }



        //INIT_PARA initParam;// = new INIT_PARA();
          string config_file = null;
          string use_aps_card = null;
          string use_htnet_card = null;
          string offline_mode = null;
          int? max_axis_num = null;
          int? max_io_num = null;
          int? max_dev_num = null;
          int? max_module_num = null;


        //int? language = null;
        string _initErrorInfo = "NO-OPS";

        HtmDio dio;// = new DIO_Htm();//DIO模块
        HtmMC mc;// = new MC_Htm(); //运动控制模块
        HtmAio aio;
        //HtmCompareTrigger cmpTrig;
        List<HtmCompareTrigger> cmpTrigs;        
        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            InvokeFailed = -3,//库函数调用出错
            InitFailedWhenOpenCard = -4,//初始化失败（初始化参数未设置）
            NotOpen = -5, //卡未打开
        }

        bool HasAxis()
        {
            int AxisCount = HTM.GetAxisNum();
            if (AxisCount > 0)
                return true;
            return false;
        }

        bool HasDio()
        {
            bool isDioExisted = false;
            HTM.IO_INFO ioInfo;
            //int? ioMaxCount = dev.GetInitParamValue("最大IO数量") as int?;
            //if (null == ioMaxCount)
            //    throw new Exception("HTM控制器初始化参数\"最大IO数量\"未设置！ ");
            int ioCount = HTM.GetIoNum();
            for (int i = 0; i < ioCount; i++)
            {
                int opt = HTM.GetIoInfo(i, out ioInfo);
                if (0 == opt)
                    if ((int)HTM.IoCardType.DIO_HTNET == ioInfo.cardType)
                    {
                        isDioExisted = true;
                        break;
                    }

            }
            return isDioExisted;
        }

        bool HasAio()
        {
            bool isAioExisted = false;
            HTM.IO_INFO ioInfo;
            //int? ioMaxCount = dev.GetInitParamValue("最大IO数量") as int?;
            //if (null == ioMaxCount)
            //    throw new Exception("HTM控制器初始化参数\"最大IO数量\"未设置！ ");
            int ioCount = HTM.GetIoNum();
            for (int i = 0; i < ioCount; i++)
            {
                int opt = HTM.GetIoInfo(i, out ioInfo);
                if (0 == opt)
                    if ((int)HTM.IoCardType.AIO_HTNET == ioInfo.cardType)
                    {
                        isAioExisted = true;
                        break;
                    }

            }
            return isAioExisted;
        }

        bool HasCompareTrigger()
        {
            int devCount = HTM.GetDeviceNum();
            bool ret = false;
            for(int i = 0; i <devCount;i++)
            {
                HTM.DEVICE_INFO devInfo;
                if(0 == HTM.GetDeviceInfo(i, out devInfo))
                    if(devInfo.devType == (byte)HTM.DeviceType.POSTRIG|| devInfo.devType == (int)HTM.DeviceType.HTDHVD)
                    {
                        ret = true;
                        break;
                    }
            }
            return ret;
        }

        /// <summary>
        /// 根据轴信息和设备信息
        /// </summary>
        void GenCompareTrigs()
        {
            //int err = 0;
            cmpTrigs.Clear();
            ////先生成轴对应的触发模块  //轴的对应触发器，也是通过在HTM——BSM配置软件中添加其他设备完成
            //List<int> axesIDs = new List<int>();
            //int axisNum = HTM.GetAxisNum();
            //for(int i = 0; i < axisNum;i++)
            //{
            //    HTM.AXIS_INFO ai;
            //    HTM.GetAxisInfo(i, out ai);
            //    if(ai.driveType == (byte)HTM.DeviceType.HTDHVD) //伺服轴
            //    //if(ai.axisType == (byte)HTM.AxisType.LINE || ai.axisType == (byte)HTM.AxisType.SERVO)   //上面一行代码无法确定轴是否有触发功能，暂时使用直线电机代替 
            //        axesIDs.Add(i);
            //}
            //if (axesIDs.Count > 0)
            //{
            //    HtmCompareTrigger trig = new HtmCompareTrigger(HtmCompareTrigger.TriggerType.AxisSlave, axesIDs.ToArray());
            //    cmpTrigs.Add(trig);
            //    trig.Open();
            //}
            //生成位置触发板对应的
            int devNum = HTM.GetDeviceNum();//所有其他设备的数量，包括光源驱动板/位置触发板/虚拟位置触发板/串口设备等
            List<int> axisTrigDevIDs = new List<int>();//int trigBoardCount = 0;
            for(int i = 0; i < devNum;i++)
            {
                HTM.DEVICE_INFO di;
                HTM.GetDeviceInfo(i, out di);
                if(di.devType == (byte)HTM.DeviceType.HTDHVD )//轴自带位置比较触发器（虚拟位置触发板）
                    //|| di.devType ==(byte)HTM.DeviceType.POSTRIG)//位置触发板
                {
                    axisTrigDevIDs.Add(i);
                    //trigBoardCount ++；
                }
            }
            if (axisTrigDevIDs.Count > 0)
            {
                HtmCompareTrigger cmpTrigger = new HtmCompareTrigger(HtmCompareTrigger.TriggerType.AxisSlave, axisTrigDevIDs.ToArray());
                cmpTrigs.Add(cmpTrigger);
                cmpTrigger.Open();
            }
        }

        #region IMcDaq's API Begin
        public int OpenDevice()
        {
            if (!IsInitOK)
                return (int)ErrorDef.InitFailedWhenOpenCard;

            if (IsDeviceOpen)
                return (int)ErrorDef.Success;
            HTM.INIT_PARA initParam = new HTM.INIT_PARA();
            initParam.para_file = config_file;
            initParam.use_aps_card = (byte)(Array.IndexOf(yesnoRange, use_aps_card) == 0 ? 1 : 0);
            initParam.use_htnet_card = (byte)(Array.IndexOf(yesnoRange, use_htnet_card) == 0 ? 1 : 0);
            initParam.offline_mode = (byte)(Array.IndexOf(openModeRange, offline_mode));
            initParam.max_axis_num = (ushort)max_axis_num;
            initParam.max_io_num = (ushort)max_io_num;
            initParam.max_dev_num = (ushort)max_dev_num;
            initParam.max_module_num = (ushort)max_module_num;
            initParam.language = 0;
            lock (HtmDllManager.AsynLocker)
            {
                if (0 != HTM.Init(ref initParam))
                {
                    HTM.Discard();
                    int nErr = 0;// HTM.Init(ref initParam);

                    for (int i = 0; i < 3; i++)
                    {
                        System.Threading.Thread.Sleep(500);
                        nErr = HTM.Init(ref initParam);
                        if (0 == nErr)
                            break;
                    }
                    if (nErr < 0)
                        return (int)ErrorDef.InvokeFailed;
                }
                
                HtmDllManager.OpendDevCount ++;
                IsDeviceOpen = true;
            }

            
            if (HasAxis())
            {
                McCount = 1;
                mc.Open();
            }
            else
                McCount = 0;


            if (HasDio())
            {
                DioCount = 1;
                dio.Open();
            }
            else
                DioCount = 0;

            if (HasAio())
            {
                AioCount = 1;
                aio.Open();
            }
            else
                AioCount = 0;

            //if (HasCompareTrigger())
            //{
            //    cmpTrig.Open();
            //    CompareTriggerCount = 1;
            //}
            //else
            //    CompareTriggerCount = 0;
            GenCompareTrigs();
            return (int)ErrorDef.Success;
        }

        public int CloseDevice()
        {
            if (!IsDeviceOpen)
                return (int)ErrorDef.Success;
            mc.Close();
            dio.Close();
            aio.Close();
            foreach (HtmCompareTrigger trig in cmpTrigs)
                trig.Close();
            cmpTrigs.Clear();
            McCount = 0;
            DioCount = 0;
            AioCount = 0;
            //CompareTriggerCount = 0;
            lock (HtmDllManager.AsynLocker)
            {
                HtmDllManager.OpendDevCount--;//HtmDllManager.IsMotionDaqOpened = false;
                if (HtmDllManager.OpendDevCount == 0)
                    HTM.Discard();
                IsDeviceOpen = false;
            }
            
            return (int)ErrorDef.Success;
        }

        public bool IsDeviceOpen { get; private set; }

        /// <summary>设备类型/// </summary>
        public string DeviceModel { get { return "景焱HTM运动控制器"; } }

        /// <summary>
        /// 设备上连接的MotionCtrl（运动控制模块）的数量
        /// </summary>
        public int McCount { get; private set; }

        /// <summary>
        /// 设备上连接的DIO（数字IO模块）的数量
        /// </summary>
        public int DioCount { get; private set; }

        public int AioCount { get; private set; }

        public int CompareTriggerCount { get { return cmpTrigs.Count; } }




        /// <summary>
        /// 获取运动控制器模块
        /// </summary>
        /// <param name="index">序号，从0开始</param>
        /// <returns></returns>
        public IJFModule_Motion GetMc(int index) //编写OK，未测试
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(string.Format("IJFMotionCtrl.GetMc failed by index ={0} (MC's Count = {1})", index, McCount));
            //if (!IsOpen)
            //    return null;
            return mc;
        }

        /// <summary>
        /// 获取数字IO控制器模块
        /// </summary>
        /// <param name="index">序号，从0开始</param>
        /// <returns></returns>
        public IJFModule_DIO GetDio(int index) //编写OK，未测试
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(string.Format("IJFMotionCtrl.GetDio failed by index ={0} (Dio's Count = {1})", index, DioCount));
            //if (!IsOpen)
            //    return null;

            return dio;

        }

        public IJFModule_AIO GetAio(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(string.Format("IJFMotionCtrl.GetAio failed by index ={0} (Dio's Count = {1})", index, DioCount));
            //if (!IsOpen)
            //    return null;

            return aio;
        }

        public IJFModule_CmprTrigger GetCompareTrigger(int index) //编写OK，未测试
        {
            if (index < 0 || index >= CompareTriggerCount)
                throw new ArgumentOutOfRangeException(string.Format("IJFMotionCtrl.GetCompareTrigger failed by index ={0} (CompareTrigger's Count = {1})", index, CompareTriggerCount));
            //if (!IsOpen)
            //    return null;
            return cmpTrigs[index];
        }
        #endregion IMcDaq's API End



        public JFRealtimeUI GetRealtimeUI()
        {
            JFRealtimeUI ret = new JFRealtimeUI();
            if (!IsDeviceOpen)
                return ret;
            ret.AutoScroll = true;
            int opt = HTM.LoadUI(ret);
            if (opt != 0)
                throw new Exception("GetRealtimeUI() failed by HTM.LoadUI() return Errorcode = " + opt);
            return ret;

        }


        /// <summary>显示一个对话框窗口</summary>
        public void ShowCfgDialog()
        {
            if (!IsDeviceOpen)
            {
                MessageBox.Show("打开配置界面失败，Error：运动控制卡未打开！");
                return;
            }
            int  opt = HTM.LoadUI();
            if (opt != 0)
                throw new Exception("ShowCfgDialog() failed By:HTM.LoadUI(true) return Errorcode = " + opt);
            return ;
        }

        public string GetErrorInfo(int errorCode)
        {
            string ret = "ErrorCode:" + errorCode + " Undefined";
            switch(errorCode)
            {
                case (int)ErrorDef.Success:
                    ret = "Success";
                    break;
                case (int)ErrorDef.InvokeFailed:
                    ret = "Inner API invoke failed";
                    break;
                case (int)ErrorDef.InitFailedWhenOpenCard:
                    ret = "Init Failed When Open";
                    break;
                case (int)ErrorDef.NotOpen:
                    ret = "Device not open";
                    break;
                default:
                    break;
            }
            return ret;
        }


    }





}
