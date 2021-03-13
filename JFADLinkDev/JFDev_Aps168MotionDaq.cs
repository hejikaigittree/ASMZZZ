using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JFUI;
using APS168_W64;
using APS_Define_W32;
using System.Windows.Forms;
using JFToolKits;

namespace JFADLinkDevice
{
    [JFDisplayName("凌华APS168运动控制卡")]
    public class JFDev_Aps168MotionDaq : IJFDevice_MotionDaq
    {

        static bool IsSDKInitialized = false; //是否已调用过SDK的init函数
        static int BoardBitsLinked = 0; // PC连接的所有卡的bits
        static int BoardBitsLoaded = 0; // 已经从Flash中加载过参数的
        static int BoardOpened = 0; //已经打开的卡的数量
        static object SDKLocker = new object();
        


        string _initErrorInfo = "NO-OPS";




        JFAps168DIO dio;//DIO模块
        JFAps168MC mc;//运动控制模块
        JFAps168CompareTrigger cmpTrigs;

        public int McCount { get; private set; }

        public int DioCount { get; private set; }

        public int AioCount { get; private set; }

        public int CompareTriggerCount { get; private set; }

        public string DeviceModel { get { return "凌华APS168运动控制卡"; } }

        public bool IsDeviceOpen { get; private set; }

        string[] _initParamNames = new string[] { /*"ADLink配置文件",*/ "BoardID" ,"控制器参数配置文件"};

        //public string factorKeyName = "PulseFactor";
        //public double[] factors = new double[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        public JFXCfg mdCfg = new JFXCfg();//MotionDaq's Cfg  用于保存各卡的定制化参数(比如脉冲当量...)

        public string[] InitParamNames
        {
            get
            {
                return _initParamNames;
            }
        }

        public bool IsInitOK { get; private set; }


        string _cfgFilePath = null;
        //private string ConfigPath { get; private set; }

        //private int BoardId { get; set; }  //凌华控制卡卡号,如果卡载拨号为bit0,此处的值应为1 , bit1 = 2 ...
        int _boardID = 0;
        //private string ADLinkCfgPath { get; set; }

        public JFDev_Aps168MotionDaq()
        {
            IsInitOK = false;
            McCount = 0;
            DioCount = 0;
            AioCount = 0;
            CompareTriggerCount = 0;
            //ConfigPath = null;
            _boardID = -1;
        }

        ~JFDev_Aps168MotionDaq()
        {
            Dispose(false);
        }

        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            InvokeFailed = -3,//库函数调用出错
            InitFailedWhenOpenCard = -4,//初始化失败（初始化参数未设置）
            NotOpen = -5, //卡未打开
            BoardIDNotExist = -6 //已连接的卡不存在指定的BoardID
        }

        public bool Initialize()
        {
            bool ret = false;
            if (IsDeviceOpen)
                CloseDevice();
            IsInitOK = false;
            do
            {
                if (string.IsNullOrEmpty(_cfgFilePath))
                {
                    _initErrorInfo = "初始化参数:\"控制器参数配置文件\" 未设置";
                    return false;
                }

                mdCfg = new JFXCfg();
                try
                {
                    mdCfg.Load(_cfgFilePath, true);
                }
                catch(Exception ex)
                {
                    _initErrorInfo = "\"控制器参数配置文件\" 加载失败：Path = " + _cfgFilePath + "ErrorInfo:" + ex.Message;
                    return false;
                }
                
                if (_boardID < 0)
                {
                    IsInitOK = false;
                    _initErrorInfo = "初始化参数:BoardID = " + _boardID + " 非法值/未设置";
                    return false;
                }


                //dio = new JFAps168DIO(BoardId);
                //mc = new JFAps168MC(BoardId, ConfigPath,this);
                //cmpTrigs = new JFAps168CompareTrigger(BoardId, ConfigPath,this);

                ret = true;
                IsInitOK = true;
                _initErrorInfo = "Success";
            } while (false);
            return ret;
        }

        public int OpenDevice()
        {
            if (!IsInitOK)
                return (int)ErrorDef.InitFailedWhenOpenCard;

            if (IsDeviceOpen)
                return (int)ErrorDef.Success;


            
            // Card(Board) initial,mode bit0(0:By system assigned, 1:By dip switch)  
            int ret = 0;
            lock (SDKLocker)
            {
                if (!IsSDKInitialized)
                {
                    ret = APS168.APS_initial(ref BoardBitsLinked, 0); //可以获取已连接计算机的控制卡的bits
                    if (ret != 0)
                        return (int)ErrorDef.InvokeFailed;

                    IsSDKInitialized = true;
                }

                bool isBoardIDExist = false; //检查总线中是否连接有指定BoardID的卡
                for (int i = 0; i < 32; i++)
                    if ((BoardBitsLinked & (1 << i)) != 0 && _boardID == (i))
                    {
                        isBoardIDExist = true;
                        break;
                    }
                if (!isBoardIDExist)
                    return (int)ErrorDef.BoardIDNotExist;
                

                if ((BoardBitsLoaded & (1 << (_boardID ))) == 0) //从Flash中载入参数
                {
                    ret = APS168.APS_load_parameter_from_flash(_boardID); //从Flash中载入参数
                    if (0 != ret)
                        return (int)ErrorDef.InvokeFailed;
                    BoardBitsLoaded |= (1 << (_boardID ));
                }
                BoardOpened++;
                //if(!mdCfg.ContainsItem("Card_" + _boardID))
                //    mdCfg.AddItem("Card"+ _boardID ,new JFXM)


                dio = new JFAps168DIO(_boardID);
                mc = new JFAps168MC(_boardID, mdCfg,this);
                cmpTrigs = new JFAps168CompareTrigger(_boardID, mdCfg, this);



                dio.Open();
                mc.Open();
                cmpTrigs.Open();

                AioCount = 0;
                McCount = 1;
                DioCount = 1;
                CompareTriggerCount = 1;

                IsInitOK = true;
                IsDeviceOpen = true;

            }
            return (int)ErrorDef.Success;
        }

        public int CloseDevice()
        {
            if (!IsDeviceOpen)
                return (int)ErrorDef.Success;
            mc.Close();
            dio.Close();
            cmpTrigs.Close();

            McCount = 0;
            DioCount = 0;
            AioCount = 0;
            CompareTriggerCount = 0;

            BoardOpened--;
            if (BoardOpened == 0)
            {
                lock (SDKLocker)
                {
                    int ret = APS168.APS_close();
                    if (ret != 0)
                        return (int)ErrorDef.InvokeFailed;
                    IsSDKInitialized = false;
                    BoardBitsLinked = 0;
                    BoardBitsLoaded = 0;
                }
            }
            IsDeviceOpen = false;
            return (int)ErrorDef.Success;
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
                cmpTrigs = null;
            }
        }

        public IJFModule_AIO GetAio(int index)
        {
            throw new NotImplementedException();
        }

        public IJFModule_CmprTrigger GetCompareTrigger(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(string.Format("IJFMotionCtrl.GetCompareTrigger failed by index ={0} (CompareTrigger's Count = {1})", index, CompareTriggerCount));
            //if (!IsOpen)
            //    return null;
            return cmpTrigs;
        }

        public IJFModule_DIO GetDio(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(string.Format("IJFMotionCtrl.GetDio failed by index ={0} (Dio's Count = {1})", index, DioCount));
            //if (!IsOpen)
            //    return null;

            return dio;
        }

        public IJFModule_Motion GetMc(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(string.Format("IJFMotionCtrl.GetMc failed by index ={0} (MC's Count = {1})", index, McCount));
            //if (!IsOpen)
            //    return null;
            return mc;
        }

        public JFRealtimeUI GetRealtimeUI()
        {
            JFRealtimeUI ret = new JFRealtimeUI();
            if (!IsDeviceOpen)
                return ret;
            ret.AutoScroll = true;

            UcMotionDaq ui = new UcMotionDaq();
            ui.AutoScroll = true;
            ui.SetDevice(this, "APS168MD BoardID = " + _boardID.ToString());
            return ui;

        }

        public void ShowCfgDialog()
        {
            if (!IsDeviceOpen)
            {
                MessageBox.Show("打开配置界面失败，Error：运动控制卡未打开！");
                return;
            }
            //UcMotionDaq ui = new UcMotionDaq();
            //ui.AutoScroll = true;
            //ui.SetDevice(this, BoardId.ToString());
            //return;
            MessageBox.Show("暂未实现");
        }

        public string GetInitErrorInfo()
        {
            return _initErrorInfo;
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
            if (name == InitParamNames[0])
            {
                return JFParamDescribe.Create(name, typeof(int), JFValueLimit.MinLimit, new object[] { 0 });
                //return JFParamDescribe.Create(name, typeof(string), JFValueLimit.FilePath, null);
            }
            else if (name == InitParamNames[1])
            {
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.FilePath, null);
            }
            //else if(name == InitParamNames[2])
            //{
                
            //}
            throw new ArgumentException("未知的初始化参数名:" + name);
        }

        public bool SetInitParamValue(string name, object value)
        {
            _CheckInitName(name, "SetInitParamValue(name, value)");
            if (null == value)
                throw new Exception("SetInitParamValue(name, value) failed By: value = null");

            if (!GetInitParamDescribe(name).ParamType.IsAssignableFrom(value.GetType()))
                throw new ArgumentException(string.Format("SetInitParamValue(name = {0}, value) faile By: value's Type = {1} can't Assignable to InitParam's Type:{2}", name, value.GetType(), GetInitParamDescribe(name).ParamType.Name));

            if (name == InitParamNames[0]) //BoardID
            {
                _boardID = (int)value;
                _initErrorInfo = "Success";
                return true;

            }
            else if (name == InitParamNames[1]) //cfg File Path
            {
                
                if(string.IsNullOrEmpty(value as string))
                {
                    _initErrorInfo = "初始化参数:" + InitParamNames[1] + " 为空字串";
                    return false;
                }
                _cfgFilePath = value as string;
                return true;
            }
            else
                throw new Exception("SetInitParamValue(name) failed By: name = " + name);
        }

        public object GetInitParamValue(string name)
        {
            _CheckInitName(name, "GetInitParamDescribe(name)");
            if (name == InitParamNames[0])
                return _boardID;
            else if (name == InitParamNames[1])
                return _cfgFilePath;
            throw new Exception();
        }

        public string GetErrorInfo(int errorCode)
        {
            string ret = "ErrorCode:" + errorCode + " Undefined";
            switch (errorCode)
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
                case (int)ErrorDef.BoardIDNotExist:
                    ret = "已连接的控制卡不存在BoradID = " + _boardID;
                    break;
                default:
                    break;
            }
            return ret;
        }
    }
}
