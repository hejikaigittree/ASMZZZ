using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;
using JFToolKits;
using JFUI;

namespace JFHTMDevice
{
    /// <summary>
    /// 楷威(高亮/频闪)光源触发控制器
    /// </summary>
    [JFVersion("1.0.0.0")]
    [JFDisplayName("楷威高亮/频闪光源触发控制器")]
    public class KWLightTrig:IJFDevice_TrigController,IJFRealtimeUIProvider
    {
        internal KWLightTrig()
        {
            IsInitOK = false;
            IsDeviceOpen = false;
            paramDescribe = JFParamDescribe.Create("串口序号", typeof(int), JFValueLimit.MinLimit, new object[] { 0 }, "光源控制器与计算机连接的串口序号:0=\"COM0\"etc.");
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
            PortNameUnexist = -6, //本机不存在命名的串口
            OpenPortFailed = -7, //打开端口失败，端口被占用
            WriteFailed = -8, //向串口写入数据失败（掉线）
            NotResponse = -9 ,//未收到串口回应数据
            ResponseError = -10,//收到回应，数据格式错误

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
                case (int)ErrorDef.PortNameUnexist:
                    ret = "Port's name unexisted";
                    break;
                case (int)ErrorDef.OpenPortFailed:
                    ret = "Open Port failed!";
                    break;
                case (int)ErrorDef.WriteFailed:// = -8, //向串口写入数据失败（掉线）
                    ret = "Write data to serialport failed";
                    break;
            case (int)ErrorDef.NotResponse:// ,//未收到串口回应数据
                    ret = "Not response after write data to serialport";
                    break;
                case (int)ErrorDef.ResponseError:// = -10,//收到回应，数据格式错误
                    ret = "Writed data's format error";
                    break;
                default://未定义的错误类型
                    break;
            }

            return ret;
        }


        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public string[] InitParamNames
        {
            get 
            {
                return new string[] { paramDescribe.DisplayName };
            }
        }
        JFParamDescribe paramDescribe = null;
        /// <summary>
        /// 获取指定名称的初始化参数的类型
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数类型</returns>
        public JFParamDescribe GetInitParamDescribe(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("GetInitParamDescribe(name) failed By: name Is Null Or Empty");
                if (!InitParamNames.Contains(name))
                    throw new ArgumentOutOfRangeException("GetInitParamDescribe(name) failed By: name = " + name + " Is does not exist in InitParam Names");
            if (name == paramDescribe.DisplayName)
                return paramDescribe;
            else
                throw new Exception("GetInitParamDescribe(name) failed by name = " + name);
        }


        int? portIndex = null;//串口名称
        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public object GetInitParamValue(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("GetInitParamValue(name) failed By: name Is Null Or Empty");
            if (!InitParamNames.Contains(name))
                throw new ArgumentOutOfRangeException("GetInitParamValue(name) failed By: name = " + name + " Is does not exist in InitParam Names");
            if (name == paramDescribe.DisplayName)
                return portIndex;
            else
                throw new Exception("GetInitParamValue(name) failed by name = " + name);
        }

        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool SetInitParamValue(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("SetInitParamValue(name,value) failed By: name Is Null Or Empty");
            if (!InitParamNames.Contains(name))
                throw new ArgumentOutOfRangeException("SetInitParamValue(name,value) failed By: name = " + name + " Is does not exist in InitParam Names");
            if (!GetInitParamDescribe(name).ParamType.IsAssignableFrom(value.GetType()))
                throw new ArgumentException(string.Format("SetInitParamValue(name = {0}, value) faile By: value's Type = {1} can't Assignable to InitParam's Type:{2}", name, value.GetType().Name, GetInitParamDescribe(name).ParamType.Name));

            if (name == paramDescribe.DisplayName)
            {
                if(null == value)
                {
                    _initErrorInfo = string.Format("设置初始化参数\"串口序号\"失败，value = null");
                    return false;
                }
                int spIndex = (int)value;
                if(spIndex < 0)
                {
                    _initErrorInfo = string.Format("设置初始化参数\"串口序号\"失败，value = {0} 不是合法的串口序号(必须大于/等于0)！", spIndex);
                    return false;
                }
                portIndex = spIndex;
                _initErrorInfo = "Success";
                return true;
            }
            _initErrorInfo = "设置初始化参数失败：未知的初始化参数名：" + name;
            return false;
        }

        string _initErrorInfo = "NO-OPS";

        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool Initialize()
        {
            if(portIndex == null)
            {
                _initErrorInfo = string.Format("初始化失败，参数\"串口序号\" 未设置");
                return false;
            }
            if (portIndex < 0)
            {

                _initErrorInfo = string.Format("初始化失败，参数\"串口序号\" = {0} 不合法(〉=0)", portIndex);
                return false;
            }

            IsInitOK = true;
            _initErrorInfo = "Success";
            return true;
        }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get; private set; }

        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo()
        {
            return _initErrorInfo;
        }




        public string DeviceModel { get { return "楷威4通道光源触发器"; } }


        object asyncLocker = new object();//线程同步锁
        SerialPort serialPort = null;
        /// <summary>
        /// 打开设备
        /// </summary>
        public int OpenDevice()
        {
            if (!IsInitOK)
                return (int)ErrorDef.InitFailedWhenOpen;
            lock (asyncLocker)
            {
                string portName = "COM" + portIndex;
                if (IsDeviceOpen)
                {
                    if (serialPort.PortName == portName)
                        return (int)ErrorDef.Success;
                    else
                        CloseDevice();
                }
                if(!SerialPort.GetPortNames().Contains(portName))
                    return (int)ErrorDef.PortNameUnexist;

               
                
                serialPort = new SerialPort(portName, 19200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                try
                {
                    serialPort.Open();
                }
                catch(Exception )
                {
                    return (int)ErrorDef.OpenPortFailed;
                }
                serialPort.WriteTimeout = 500;
                serialPort.ReadTimeout = 300;

                ///打开所有通道输出
                byte[] cmdEnableAll= new byte[4] { 1,0xff, 0, 1 };//使能所有通道
                byte[] cmdIntensityAll = new byte[4] { 2, 0xff, 0x1, 0x90 };//所有通道强度设为400
                byte[] cmdDisableTrigAll = new byte[4] { 0x10, 0xff, 0, 0 };//关闭所有触发使能
                byte[] cmdTrigModeAll = new byte[4] { 0x12, 0xff, 1, 0 };//触发方式，边沿，上升沿
                byte[] cmdTrigDelayAll = new byte[4] { 0x14, 0xff, 0, 0 };//所有延迟为0
                byte[] cmdDurationUnitAll = new byte[4] { 0x15, 0xff, 0, 1 };//时间单位：毫秒
                byte[] cmdDurationAll = new byte[4] { 0x16, 0xff, 0, 0x64 };//时间宽度：100毫秒
                //int opt = SetTrigChannelEnables(new int[] { 0, 1, 2, 3 }, new bool[] { true, true, false, false });
                byte[] pack = new byte[28];
                Array.Copy(cmdEnableAll, 0, pack, 0, 4);
                Array.Copy(cmdIntensityAll, 0, pack, 4, 4);
                Array.Copy(cmdDisableTrigAll, 0, pack, 8, 4);
                Array.Copy(cmdTrigModeAll, 0, pack, 12, 4);
                Array.Copy(cmdTrigDelayAll, 0, pack, 16, 4);
                Array.Copy(cmdDurationUnitAll, 0, pack, 20, 4);
                Array.Copy(cmdDurationAll, 0, pack, 24, 4);
                int opt = SendData(pack);
                if (opt != (int)ErrorDef.Success)
                    throw new Exception("Init All channel failed when open! errorInfo = " + GetErrorInfo(opt));
                for(int i = 0; i < TrigChannelCount;i++)
                {
                    chnEnables[i] = false;
                    chnDurations[i] = 100;
                    chnIntensities[i] = 400;
                }

                serialPort.DiscardOutBuffer();
                serialPort.DiscardInBuffer();
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
                if (!IsInitOK || !IsDeviceOpen)
                    return (int)ErrorDef.Success;
                serialPort.DiscardOutBuffer();
                serialPort.DiscardInBuffer();
                serialPort.Close();
                serialPort.Dispose();
                serialPort = null;
                IsDeviceOpen = false;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 设备是否已经打开
        /// </summary>
        public bool IsDeviceOpen { get; private set; }

        public int TrigChannelCount { get { return 4; } }

        /// <summary>数据头</summary>
        byte[] head = { 0X64, 6, 0X10, 0X01 };//[0]:特征字 [1]：数据包长度 [2]：设备型号 [3]：设备编号
        /// <summary> 尾</summary>
        byte[] tail = { 0X84 };//数据尾

        /// <summary> CRC16 x16+x15+x2+1/// </summary>
         byte[] CRC16(byte[] data)
        {
            int len = data.Length;
            if (len > 0)
            {
                ushort crc = 0xFFFF;

                for (int i = 0; i < len; i++)
                {
                    crc = (ushort)(crc ^ (data[i]));
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                    }
                }
                byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
                byte lo = (byte)(crc & 0x00FF);         //低位置

                return new byte[] { lo, hi };
            }
            return new byte[] { 0, 0 };
        }

        public int SendData(byte[] data)
        {
            lock (asyncLocker)
            {
                byte[] pack = new byte[7 + data.Length]; //4个数据头
                Array.Copy(head, 0, pack, 0, head.Length);
                pack[1] = (byte)(data.Length + 3);
                Array.Copy(data, 0, pack, head.Length, data.Length);
                /// <summary> 用于校验的字 </summary>
                /// <summary> 校验后的字 </summary>
                byte[] checkCode = CRC16(pack.Skip(1).Take(data.Length + 3).ToArray());
                Array.Copy(checkCode, 0, pack, pack.Length-3, 2);
                pack[pack.Length - 1] = tail[0];
                //byte[] Cmd = { head[0], head[1], head[2], head[3], order[0], order[1], num[0], num[1], checkCode[0], checkCode[1], tail[0] };
                ////100 6 16 1 21 1 0 100 119 22 132
                ////64 06 10 01 16 01 00 64 16 33 84
                serialPort.DiscardInBuffer();
                try
                {
                    serialPort.Write(pack, 0, pack.Length);
                }
                catch(Exception)
                {
                    return (int)ErrorDef.WriteFailed;
                }
                byte[] rcvBytes = new byte[] { 0 };
                int a = serialPort.Read(rcvBytes,0,1);
                if (a == 0)
                    return (int)ErrorDef.NotResponse;
                if (rcvBytes[0] != 0x64)
                    return (int)ErrorDef.ResponseError;
                return (int)ErrorDef.Success;
                
            }
        }

        bool[] chnEnables = new bool[] { false, false, false, false };//各通道使能状态
        int[] chnIntensities = new int[] { 0, 0, 0, 0 };//各通道强度
        int[] chnDurations = new int[] { 0, 0, 0, 0 };//各通道曝光时长

        /// <summary>获取通道使能状态</summary>
        public int GetTrigChannelEnable(int channel, out bool isEnabled)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelBandInputs(int channel = " + channel + " ...) faield By:channel is out of range 0~" + (TrigChannelCount - 1));
            isEnabled = false;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                isEnabled = chnEnables[channel];
            }
            return (int)ErrorDef.Success;

        }
        /// <summary>设置通道使能状态</summary>
        public int SetTrigChannelEnable(int channel, bool isEnable)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("SetTrigChannelBandInputs(int channel = " + channel + " ...) faield By:channel is out of range 0~" + (TrigChannelCount - 1));
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                byte[] pack = new byte[] { 0x10, (byte)(0x1 << channel), 0, (byte)(isEnable ? 1 : 0) };
                int opt = SendData(pack);
                if (opt == (int)ErrorDef.Success)
                    chnEnables[channel] = isEnable;
                return opt;
            }
        }

        public int GetTrigChannelEnables(out bool[] isEnables)
        {
            isEnables = new bool[4];
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                Array.Copy(chnEnables, 0, isEnables, 0, 4);
            }
            return (int)ErrorDef.Success;
        }

        public int SetTrigChannelEnables(int[] channels, bool[] isEnables)
        {
            if (null == channels || null == isEnables)
                throw new ArgumentNullException("SetTrigChannelEnables Failed by Param:(int[] channels = null, or bool[] isEnables = null)");
            if (channels.Length == 0 || channels.Length > TrigChannelCount || channels.Length != isEnables.Length)
                throw new ArgumentNullException(string.Format("SetTrigChannelEnables Failed by Param:(int[] channels.Length = {0},bool[] isEnables.Length = {1}) and the TrigChannelCount = {2}", channels.Length, isEnables.Length, TrigChannelCount));
            List<int> needEnableIndex = new List<int>();//需要打开的端口
            List<int> needDisableIndex = new List<int>();//需要关闭的端口
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                for (int i = 0; i < channels.Count(); i++)
                {
                    if (channels[i] < 0 || channels[i] >= TrigChannelCount)
                        throw new ArgumentOutOfRangeException(string.Format("SetTrigChannelEnables Failed by Param:channels[{0}] = {1} is out of range 0~{2}", i, channels[i], TrigChannelCount - 1));
                    if (isEnables[i] != chnEnables[channels[i]])
                    {
                        if (isEnables[i])
                        {
                            if (!needEnableIndex.Contains(channels[i]))
                                needEnableIndex.Add(channels[i]);
                        }
                        else
                        {
                            if (!needDisableIndex.Contains(channels[i]))
                                needDisableIndex.Add(channels[i]);
                        }
                    }
                }
                if (needEnableIndex.Count() == 0 && needDisableIndex.Count() == 0) //状态一致，不需要操作
                    return (int)ErrorDef.Success;
                List<byte> cmd = new List<byte>();
                if (needEnableIndex.Count > 0)
                {
                    byte enableChnNum = 0;
                    foreach (int i in needEnableIndex)
                        enableChnNum += (byte)(1 << i);
                    cmd.AddRange(new byte[] { 0x10, enableChnNum, 0, 1 });
                }

                if (needDisableIndex.Count > 0)
                {
                    byte disableChnNum = 0;
                    foreach (int i in needDisableIndex)
                        disableChnNum += (byte)(1 << i);
                    cmd.AddRange(new byte[] { 0x10, disableChnNum, 0, 0 });
                }

                int opt = SendData(cmd.ToArray());
                if ((int)ErrorDef.Success == opt)
                {
                    foreach (int i in needEnableIndex)
                        chnEnables[i] = true;
                    foreach (int i in needDisableIndex)
                        chnEnables[i] = false;
                }
                return opt;
            }
        }

        public int TrigSrcChannelCount { get { return 4; } } //新添加

        /// <summary>
        /// 设置（触发）输出通道的输入源（通道）
        /// </summary>
        /// <param name="trigChannel">输出通道号</param>
        /// <param name="srcMask">输入通道位掩码</param>
        /// <returns>如果设备不支持此功能，则返回Unsupport</returns>
        public int GetTrigChannelSrc(int trigChannel, out int srcMask) //新添加
        {
            srcMask = 1 << trigChannel;
            return (int)ErrorDef.Success;
        }

        public int SetTrigChannelSrc(int trigChannel, int srcMask)//
        {
            return (int)ErrorDef.Unsupported;
        }

        /// <summary>获取通道触发强度</summary>
        public int GetTrigChannelIntensity(int channel, out int intensity)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelIntensity(int channel = " + channel + " ...) faield By:channel is out of range 0~" + (TrigChannelCount - 1));
            intensity = 0;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                intensity = chnIntensities[channel];
            }
            return (int)ErrorDef.Success;
        }
        /// <summary>设置通道触发强度</summary>
        public int SetTrigChannelIntensity(int channel, int intensity)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("SetTrigChannelIntensity(int channel = " + channel + " ...) faield By:channel is out of range 0~" + (TrigChannelCount - 1));
            if (intensity < 0 || intensity > ushort.MaxValue)
                return (int)ErrorDef.ParamError;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                byte[] pack = new byte[] { 2, (byte)(1<<channel), (byte)(intensity>>8), (byte)(intensity & 0XFF) };
                int opt = SendData(pack);
                if (opt == (int)ErrorDef.Success)
                    chnIntensities[channel] = intensity;
                return opt;
            }
        }

        /// <summary>获取通道触发时长</summary>
        public int GetTrigChannelDuration(int channel, out int duration)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelDuration(int channel = " + channel + " ...) faield By:channel is out of range 0~" + (TrigChannelCount - 1));
            duration = 0;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                duration = chnDurations[channel];
            }
            return (int)ErrorDef.Success;
        }
        /// <summary>设置通道触发时长</summary>
        public int SetTrigChannelDuration(int channel, int duration)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("SetTrigChannelDuration(int channel = " + channel + " ...) faield By:channel is out of range 0~" + (TrigChannelCount - 1));
            if (duration < 0 || duration > 1000)
                return (int)ErrorDef.ParamError;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                byte[] pack = new byte[] { 0x16, (byte)(1 << channel), (byte)(duration >> 8), (byte)(duration & 0XFF) };
                int opt = SendData(pack);
                if (opt == (int)ErrorDef.Success)
                    chnDurations[channel] = duration;
                return opt;
            }
        }


        /// <summary>设置所有通道触发强度</summary>
        public int SetTrigIntensity(int intensity)
        {
            if (intensity < 0 || intensity > ushort.MaxValue)
                return (int)ErrorDef.ParamError;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                byte[] pack = new byte[] { 2, 0xff, (byte)(intensity >> 8), (byte)(intensity & 0XFF) };
                int opt = SendData(pack);
                if (opt == (int)ErrorDef.Success)
                    for(int i = 0; i < 4;i++)
                    chnIntensities[i] = intensity;
                return opt;
            }
        }
        /// <summary>设置所有通道触发时长</summary>
        public int SetTrigDuration(int duration)
        {
            if (duration < 0 || duration > 1000)
                return (int)ErrorDef.ParamError;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                byte[] pack = new byte[] { 0x16, 0xff, (byte)(duration >> 8), (byte)(duration & 0Xff) };
                int opt = SendData(pack);
                if (opt == (int)ErrorDef.Success)
                    for (int i = 0; i < 4; i++)
                        chnDurations[i] = duration;
                return opt;
            }
        }

        public int SoftwareTrigAll()//不支持
        {
            return (int)ErrorDef.Unsupported;
        }

        public int SoftwareTrigChannel(int channel)
        {
            return (int)ErrorDef.Unsupported;
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

        ~KWLightTrig()
        {
            Dispose(false);
        }

    }
}
