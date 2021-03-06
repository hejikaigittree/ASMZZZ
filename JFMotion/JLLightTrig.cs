using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using JFUI;
using System.IO.Ports;
using System.Threading;

namespace JFHTMDevice
{
    /// <summary>
    /// 嘉励(非增亮)光源触发控制器
    /// </summary>  
    public abstract class JLLightTrig : IJFDevice_LightControllerWithTrig, IJFRealtimeUIProvider
    {
        public abstract int TrigChannelCount { get; }

        public abstract int TrigSrcChannelCount { get; } //新添加

        /// <summary>
        /// 设置（触发）输出通道的输入源（通道）
        /// </summary>
        /// <param name="trigChannel">输出通道号</param>
        /// <param name="srcMask">输入通道位掩码</param>
        /// <returns>如果设备不支持此功能，则返回Unsupport</returns>
        public virtual int GetTrigChannelSrc(int trigChannel, out int srcMask) //新添加
        {
            if (trigChannel < 0 || trigChannel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelSrc(trigChannel,..) failed by trigChannel = " + trigChannel + " is out of range 0~" + (TrigChannelCount - 1));
            srcMask = 1 << trigChannel;
            return (int)ErrorDef.Success;
        }

        public virtual int SetTrigChannelSrc(int trigChannel, int srcMask)//
        {
            return (int)ErrorDef.Unsupported;
        }

        abstract  public string DeviceModel { get; }

        public bool IsDeviceOpen { get; private set; }
        //JFParamDescribe paramDescribe = null;
        object asyncLocker = new object();//线程同步锁
        SerialPort serialPort = null;
        int portIndex = 0;
        int baudRate = 9600;


        bool[] chnEnables = new bool[] { false, false, false, false };//各通道使能状态
        int[] chnIntensities = new int[] { 0, 0, 0, 0 };//各通道强度
        int[] chnDurations = new int[] { 0, 0, 0, 0 };//各通道曝光时长
        string _initErrorInfo = "NO-OPS";
        
        internal JLLightTrig()
        {
            IsInitOK = false;
            IsDeviceOpen = false;
            //paramDescribe = JFParamDescribe.Create("串口序号", typeof(int), JFValueLimit.MinLimit, new object[] { 0 }, "光源控制器与计算机连接的串口序号:0=\"COM0\"etc.");
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
            NotResponse = -9,//未收到串口回应数据
            ResponseError = -10,//收到回应，数据格式错误

        }
        public string[] InitParamNames
        {
            get
            {
                return new string[] { "串口序号","波特率"/*paramDescribe.DisplayName*/ };
            }
        }

        public bool IsInitOK { get; private set; }     

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

        public string GetInitErrorInfo()
        {
            return _initErrorInfo;
        }

        public JFParamDescribe GetInitParamDescribe(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("GetInitParamDescribe(name) failed By: name Is Null Or Empty");
            if (!InitParamNames.Contains(name))
                throw new ArgumentOutOfRangeException("GetInitParamDescribe(name) failed By: name = " + name + " Is does not exist in InitParam Names");
            if (name == "串口序号")
            {
                return JFParamDescribe.Create(name, typeof(int), JFValueLimit.MinLimit, new object[] { 0 });
            }
            else if(name == "波特率")
            {
                return JFParamDescribe.Create(name, typeof(int), JFValueLimit.Options, new object[] { 9600,115200 });
            }
            else
                throw new Exception("GetInitParamDescribe(name) failed by name = " + name);
        }

        public object GetInitParamValue(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("GetInitParamValue(name) failed By: name Is Null Or Empty");
            if (!InitParamNames.Contains(name))
                throw new ArgumentOutOfRangeException("GetInitParamValue(name) failed By: name = " + name + " Is does not exist in InitParam Names");
            if (name == "串口序号")
                return portIndex;
            else if (name == "波特率")
                return baudRate;
            else
                throw new Exception("GetInitParamValue(name) failed by name = " + name);
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

        public int GetTrigChannelEnable(int channel, out bool isEnabled)
        {
            isEnabled = false;
            return (int)ErrorDef.Unsupported;

            //if (channel < 0 || channel >= TrigChannelCount)
            //    throw new ArgumentOutOfRangeException("GetTrigChannelEnable(int channel = " + channel + " ...) faield By:channel is out of range 0~" + (TrigChannelCount - 1));
            //isEnabled = false;
            //lock (asyncLocker)
            //{
            //    if (!IsDeviceOpen)
            //        return (int)ErrorDef.NotOpen;
            //    isEnabled = chnEnables[channel];
            //}
            //return (int)ErrorDef.Success;
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

        public int GetTrigChannelIntensity(int channel, out int intensity)
        {
            return GetLightIntensity(channel, out intensity);
           
        }

        public bool Initialize()
        {
            if (portIndex == null)
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
                if (!SerialPort.GetPortNames().Contains(portName))
                    return (int)ErrorDef.PortNameUnexist;

                serialPort = new SerialPort(portName, baudRate, System.IO.Ports.Parity.None, 8);
                try
                {
                    serialPort.Open();
                }
                catch (Exception)
                {
                    return (int)ErrorDef.OpenPortFailed;
                }
                serialPort.WriteTimeout = 500;
                serialPort.ReadTimeout = 300;
                serialPort.DiscardOutBuffer();
                serialPort.DiscardInBuffer();
                IsDeviceOpen = true;
                for (int i = 0; i < LightChannelCount; i++)
                {
                    int iid = 0;
                    int err = GetLightIntensityInDev(i, out iid);
                    if(err != 0)
                    {
                        chnEnables[i] = false;
                        chnIntensities[i] = 0;
                    }
                    else
                    {
                        chnEnables[i] = iid != 0 ? true:false ;
                        chnIntensities[i] = iid;
                    }

                }
            }
            return (int)ErrorDef.Success;
        }
        public bool SetInitParamValue(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("SetInitParamValue(name,value) failed By: name Is Null Or Empty");
            if (!InitParamNames.Contains(name))
                throw new ArgumentOutOfRangeException("SetInitParamValue(name,value) failed By: name = " + name + " Is does not exist in InitParam Names");
            if (!GetInitParamDescribe(name).ParamType.IsAssignableFrom(value.GetType()))
                throw new ArgumentException(string.Format("SetInitParamValue(name = {0}, value) faile By: value's Type = {1} can't Assignable to InitParam's Type:{2}", name, value.GetType().Name, GetInitParamDescribe(name).ParamType.Name));

            if (name == "串口序号")
            {
                if (null == value)
                {
                    _initErrorInfo = string.Format("设置初始化参数\"串口序号\"失败，value = null");
                    return false;
                }
                int spIndex = (int)value;
                if (spIndex < 0)
                {
                    _initErrorInfo = string.Format("设置初始化参数\"串口序号\"失败，value = {0} 不是合法的串口序号(必须大于/等于0)！", spIndex);
                    return false;
                }
                portIndex = spIndex;
                _initErrorInfo = "Success";
                return true;
            }
            else if(name == "波特率")
            {
                baudRate = (int)value;
                _initErrorInfo = "Success";
                return true;
            }
            _initErrorInfo = "设置初始化参数失败：未知的初始化参数名：" + name;
            return false;
        }
        public int SendData(string str)
        {
            lock (asyncLocker)
            {
                
                serialPort.DiscardInBuffer();
                try
                {
                    string Msg = xorCheack(str);
                    serialPort.WriteLine(Msg);//发送数据                  
                }
                catch (Exception)
                {
                    return (int)ErrorDef.WriteFailed;
                }

                byte[] DataRec = new byte[8];
                serialPort.Read(DataRec, 0, 8);
                string RecDataBuffer = Encoding.ASCII.GetString(DataRec).Trim('\0');

                if (RecDataBuffer == "&")//设定失败 “&”；
                    return (int)ErrorDef.ResponseError;

                return (int)ErrorDef.Success;

            }
        }
       
        /// <summary>
        /// 设置通道曝光时间
        /// </summary>
        StringBuilder SettingMsg = new StringBuilder();//
        public int SetTrigChannelDuration(int channel, int duration)
        {
            return (int)ErrorDef.Unsupported;
        }
        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string xorCheack(string str)
        {
            //获取s应字节数组
            byte[] b = Encoding.ASCII.GetBytes(str);
            // xorResult 存放校验结注意：初值首元素值
            byte xorResult = b[0];
            // 求xor校验注意：XOR运算第二元素始
            for (int i = 1; i < b.Length; i++)
            {
                xorResult ^= b[i];
            }
            // 运算xorResultXOR校验结，^=为异或符号
            // MessageBox.Show();

            return str + xorResult.ToString("X");

        }
        public int SetTrigChannelEnable(int channel, bool isEnable)
        {
            return (int)ErrorDef.Unsupported;
           
        }

        public int SetTrigChannelEnables(int[] channels, bool[] isEnables)
        {
            return (int)ErrorDef.Unsupported;
            
        }

        public int SetTrigChannelIntensity(int channel, int intensity)
        {
            return SetLightIntensity(channel, intensity);

        }
        //
        public int SetTrigDuration(int duration)
        {
            return (int)ErrorDef.Unsupported;
        }

        public int SetTrigIntensity(int intensity)
        {
            return (int)ErrorDef.Unsupported;/////////                  
        }

        public int SoftwareTrigAll()
        {
            return (int)ErrorDef.Unsupported;
        }

        public int SoftwareTrigChannel(int channel)
        {
            return (int)ErrorDef.Unsupported;
        }

        JFRealtimeUI IJFRealtimeUIProvider.GetRealtimeUI()
        {
            UcLightCtrl ui = new UcLightCtrl();
            ui.SetModuleInfo(this);
            return ui;
        }
        public abstract int LightChannelCount { get; }

        //public int TrigSrcChannelCount { get { return 1; } }

        JFLightWithTrigWorkMode _workMode = JFLightWithTrigWorkMode.TurnOnOff;
        public int SetWorkMode(JFLightWithTrigWorkMode mode)
        {
            _workMode = mode;
            return (int)ErrorDef.Success;//return (int)ErrorDef.Unsupported;
        }

        public int GetWorkMode(out JFLightWithTrigWorkMode workMode)
        {
            workMode = _workMode;//JFLightWithTrigWorkMode.TurnOnOff;
            return (int)ErrorDef.Success;
        }

        public int GetLightChannelEnable(int channel, out bool isTurnOn)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("GetTrigChannelEnable(int channel = " + channel + " ...) faield By:channel is out of range 0~" + (TrigChannelCount - 1));
            isTurnOn = false;
            lock (asyncLocker)
            {
                if (!IsDeviceOpen)
                    return (int)ErrorDef.NotOpen;
                isTurnOn = chnEnables[channel];
            }
            return (int)ErrorDef.Success;
        }

        public int SetLightChannelEnable(int channel, bool isTurnOn)
        {
            if (!IsDeviceOpen)
                return (int)ErrorDef.NotOpen;          
            SettingMsg.Clear();
            SettingMsg.Append("$");
            int value = (isTurnOn ? 1 : 2);
            string st = value.ToString("X"); //1 使能 2 关闭
            SettingMsg.Append(st);
            string strnum = (channel + 1).ToString();
            SettingMsg.Append(strnum);
            string str;
            if (value == 2)
                str = "029";
            else
                str = "064";

            SettingMsg.Append(str);
            lock (asyncLocker)
            {
                try
                {
                    int ret = SendData(SettingMsg.ToString());
                    if (ret != (int)ErrorDef.Success)
                        return (int)ErrorDef.WriteFailed;

                    chnEnables[channel] = isTurnOn;
                    if (isTurnOn)
                        SetLightIntensity(channel,chnIntensities[channel]);
                }
                catch (Exception)
                {
                    return (int)ErrorDef.WriteFailed;

                }
            }
            return (int)ErrorDef.Success;
        }

        public int GetLightChannelEnables(out bool[] isTurnOns)
        {
            //bool[] arr = new bool[2];
            //isTurnOns = arr;
            //return (int)ErrorDef.Unsupported;
            isTurnOns = new bool[LightChannelCount];
            if (!IsDeviceOpen)
                return (int)ErrorDef.NotOpen;
            
            Array.Copy(chnEnables, 0, isTurnOns, 0, LightChannelCount);
            return (int)ErrorDef.Success;
        }

        public int SetLightChannelEnables(int[] channels, bool[] isTurnOns)
        {
            if (!IsDeviceOpen)
                return (int)ErrorDef.NotOpen;

            for (int i = 0; i < channels.Count(); i++)
            {
                SettingMsg.Clear();
                SettingMsg.Append("$");

                int value = (isTurnOns[i] ? 1 : 2);
                string st = value.ToString("X"); //1 使能 2 关闭
                SettingMsg.Append(st);
                SettingMsg.Append((i+1).ToString());
                string str;
                if (value == 2)
                    str = "029";//GS
                else
                    str = chnIntensities[i].ToString("X").PadLeft(3, '0');//"064";//@

                SettingMsg.Append(str);
                lock (asyncLocker)
                {
                    try
                    {
                        int ret = SendData(SettingMsg.ToString());
                        if (ret != (int)ErrorDef.Success)
                            return (int)ErrorDef.WriteFailed;

                        chnEnables[i] = isTurnOns[i];
                    }
                    catch (Exception)
                    {
                        return (int)ErrorDef.WriteFailed;

                    }
                }
            }
            return (int)ErrorDef.Success;
        }
        public int GetLightIntensity(int channel, out int intensity)
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

        public int SendData(string str, out int intensity)
        {
            intensity = 0;
            lock (asyncLocker)
            {

                serialPort.DiscardInBuffer();
                try
                {
                    string Msg = xorCheack(str);
                    serialPort.WriteLine(Msg);//发送数据                  
                }
                catch (Exception)
                {
                    return (int)ErrorDef.WriteFailed;
                }
                Thread.Sleep(50);
                byte[] DataRec = new byte[8];
                serialPort.Read(DataRec, 0, 8);
                string RecDataBuffer = Encoding.ASCII.GetString(DataRec).Trim('\0');

                if (RecDataBuffer == "&")//设定失败 “&”；
                    return (int)ErrorDef.ResponseError;

                //byte[] inity = new byte[3];
                //for (int i = 0; i < inity.Length; i++)
                //{
                //    inity[i] = DataRec[i + 4];
                //}

                string numString = RecDataBuffer.Substring(4, 3);

                intensity = Convert.ToInt32(numString, 16);//System.BitConverter.ToInt32(inity, 0);
                return (int)ErrorDef.Success;
            }
        }

        int GetLightIntensityInDev(int channel,out int intensity)
        {
            intensity = 0;
            if (!IsDeviceOpen)
                return (int)ErrorDef.NotOpen;

            SettingMsg.Clear();
            SettingMsg.Append("$");
            SettingMsg.Append("4");//
            SettingMsg.Append((channel + 1).ToString());
            SettingMsg.Append("064");

            lock (asyncLocker)
            {
                try
                {
                    int ret = SendData(SettingMsg.ToString(), out intensity);

                    if (ret != (int)ErrorDef.Success)
                        return (int)ErrorDef.WriteFailed;

                }
                catch (Exception)
                {
                    return (int)ErrorDef.WriteFailed;

                }
            }
            return (int)ErrorDef.Success;
        }

        public int SetLightIntensity(int channel, int intensity)
        {
            if (channel < 0 || channel >= TrigChannelCount)
                throw new ArgumentOutOfRangeException("SetTrigChannelDuration(int channel = " + channel + " ...) faield By:channel is out of range 0~" + (TrigChannelCount - 1));
            if (intensity < 0 || intensity > 255)
                return (int)ErrorDef.ParamError;

            if (!IsDeviceOpen)
                return (int)ErrorDef.NotOpen;

            if(!chnEnables[channel])
            {
                chnIntensities[channel] = intensity;
                return (int)ErrorDef.Success;
            }

            SettingMsg.Clear();
            SettingMsg.Append("$");
            SettingMsg.Append("3");
            string strnum = (channel + 1).ToString();
            SettingMsg.Append(strnum);

            string str = intensity.ToString("X");
            if (str.Length == 1)
            {
                SettingMsg.Append("00" + str);
            }
            else
            {
                SettingMsg.Append("0" + str);
            }
            lock (asyncLocker)
            {
                try
                {
                    int ret = SendData(SettingMsg.ToString());
                    if (ret != (int)ErrorDef.Success)
                        return (int)ErrorDef.WriteFailed;

                    chnIntensities[channel] = intensity;
                }
                catch
                {
                    return (int)ErrorDef.WriteFailed;
                }
            }
            return (int)ErrorDef.Success;
        }



        ~JLLightTrig()
        {
            Dispose(false);
        }
     
    }

    [JFVersion("1.0.0.0")]
    [JFDisplayName("嘉励4通道光源控制器")]
    public class JLLightCtrlE_4 : JLLightTrig
    {

        public JLLightCtrlE_4() : base()
        {

        }

        public override string DeviceModel { get { return "嘉励4通道光源控制器"; } }

        public override int LightChannelCount { get { return 4; } }

        public override int TrigChannelCount { get { return 4; } }

        public override int TrigSrcChannelCount { get { return 4; } }
    }

    [JFVersion("1.0.0.0")]
    [JFDisplayName("嘉励2通道光源控制器")]
    public class JLLightCtrlE_2 : JLLightTrig
    {

        internal JLLightCtrlE_2() : base()
        {

        }

        public override string DeviceModel { get { return "嘉励2通道光源控制器"; } }

        public override int LightChannelCount { get { return 2; } }

        public override int TrigChannelCount { get { return 2; } }

        public override int TrigSrcChannelCount { get { return 2; } }
    }
}
