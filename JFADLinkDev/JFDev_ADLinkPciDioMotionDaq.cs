using System.Runtime.InteropServices;
using System;
using JFInterfaceDef;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JFUI;
using System.Windows.Forms;
using JFToolKits;
namespace JFADLinkDevice
{
    [JFDisplayName("凌华PCI_DIO采集卡")]
    public class JFDev_ADLinkPciDioMotionDaq : IJFDevice_MotionDaq
    {

        internal enum CardType
        {
            PCI_Unknown = 0,
            PCI_6208V = 1,
            PCI_6208A = 2,
            PCI_6308V = 3,
            PCI_6308A = 4,
            PCI_7200 = 5,
            PCI_7230 = 6,
            PCI_7233 = 7,
            PCI_7234 = 8,
            PCI_7248 = 9,
            PCI_7249 = 10,
            PCI_7250 = 11,
            PCI_7252 = 12,
            PCI_7296 = 13,
            PCI_7300A_RevA = 14,
            PCI_7300A_RevB = 15,
            PCI_7432 = 16,
            PCI_7433 = 17,
            PCI_7434 = 18,
            PCI_8554 = 19,
            PCI_9111DG = 20,
            PCI_9111HR = 21,
            PCI_9112 = 22,
            PCI_9113 = 23,
            PCI_9114DG = 24,
            PCI_9114HG = 25,
            PCI_9118DG = 26,
            PCI_9118HG = 27,
            PCI_9118HR = 28,
            PCI_9810 = 29,
            PCI_9812 = 30,
            PCI_7396 = 31,
            PCI_9116 = 32,
            PCI_7256 = 33,
            PCI_7258 = 34,
            PCI_7260 = 35,
            PCI_7452 = 36,
            PCI_7442 = 37,
        }

        enum ErrorDef
        {
            Success = 0, //
            NotInitialed = -1, //打开卡时未初始化
            InvokeFailed = -3,//库函数调用出错
            NotOpen = -5, //卡未打开
        }


        public JFDev_ADLinkPciDioMotionDaq()
        {

            DeviceModel = "凌华PCI_DIO采集卡";
            IsDeviceOpen = false;
            IsInitOK = false;
        }

        JFAdlinkPciDio _dioMD = null;


        public int McCount { get { return 0; } }
        

        public int DioCount { get { return IsDeviceOpen ? 1 : 0; } }

        public int AioCount { get { return 0; } }

        public int CompareTriggerCount { get { return 0; } }

        public string DeviceModel { get; private set; }

        public bool IsDeviceOpen { get; private set; }

        string[] _initParamNames = new string[] { "采集卡型号", "板载拨码号" };
        public string[] InitParamNames { get { return _initParamNames; } }

        public bool IsInitOK { get; private set; }

        public void Dispose()
        {
            CloseDevice();
        }

        string _initErrorInfo = "No-Opt";

        CardType _cardType = CardType.PCI_Unknown;
        int _cardIndex = -1;
        public string GetInitErrorInfo()
        {
            return _initErrorInfo;
        }

        public JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == _initParamNames[0])
                return JFParamDescribe.Create(name, typeof(CardType), JFValueLimit.NonLimit, null);
            else if (name == _initParamNames[1])
                return JFParamDescribe.Create(name, typeof(int), JFValueLimit.MinLimit | JFValueLimit.MaxLimit, new object[] { 0, 31 });
            else
                throw new ArgumentException("不支持的初始化参数名:" + name);
        }

        public object GetInitParamValue(string name)
        {
            if (name == _initParamNames[0])
                return _cardType;
            else if (name == _initParamNames[1])
                return _cardIndex;
            else
                throw new ArgumentException("不支持的初始化参数名:" + name);
        }

        public bool SetInitParamValue(string name, object value)
        {
            if (name == _initParamNames[0])
            {
                CardType ct = (CardType)value;
                if(ct == CardType.PCI_Unknown)
                {
                    _initErrorInfo = "初始化参数 \"" + name + "\" 为无效值:" + ct.ToString();
                    return false;
                }
                _initErrorInfo = "Succes! setinitparam:" + name;
                _cardType = ct;
                return true;
            }
            else if (name == _initParamNames[1])
            {

                _cardIndex = (int)value;
                _initErrorInfo = "Succes! setinitparam:" + name;
                return true;
            }
            else
                throw new ArgumentException("不支持的初始化参数名:" + name);
        }

        public bool Initialize()
        {
            CloseDevice();
            IsInitOK = false;
            if(_cardType == CardType.PCI_Unknown)
            {
                _initErrorInfo = "初始化失败,参数:" + _initParamNames[0] + " = " + _cardType.ToString();
                return false;
            }

            if(_cardIndex < 0)
            {
                _initErrorInfo = "初始化失败,参数:" + _initParamNames[1] + " = " + _cardIndex;
                return false;
            }
            IsInitOK = true;
            return true;
        }

        short _devHandle = -1; //SDK打开卡时返回的句柄
        public int OpenDevice()
        {
            if (!IsInitOK)
                return (int)ErrorDef.NotInitialed;
            if (IsDeviceOpen)
                return 0 ;

            _devHandle = DASK.Register_Card((ushort)_cardType, (ushort)_cardIndex);
            if (_devHandle < 0)
                return (int)ErrorDef.InvokeFailed;

            _dioMD = new JFAdlinkPciDio(_cardType, _devHandle);
            _dioMD.Open();

            IsDeviceOpen = true;
            return (int)ErrorDef.Success;

        }


        public int CloseDevice()
        {
            if (!IsDeviceOpen)
                return (int)ErrorDef.Success;
            if (DASK.Release_Card((ushort)_cardIndex) != 0)
                return (int)ErrorDef.InvokeFailed;
            _dioMD.Close();
            _dioMD = null;
            
            IsDeviceOpen = false;
            return (int)ErrorDef.Success;

        }



        public IJFModule_AIO GetAio(int index)
        {
            return null;
        }

        public IJFModule_CmprTrigger GetCompareTrigger(int index)
        {
            return null;
        }

        public IJFModule_DIO GetDio(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException("Index = " + index + " 为非法值(此数值只能为0)");
            if (!IsDeviceOpen)
                return null;
            return _dioMD;
        }

        public string GetErrorInfo(int errorCode)
        {
            switch(errorCode)
            {
                case (int)ErrorDef.Success:
                    return "Success";
                case (int)ErrorDef.NotInitialed:// = -1, //打开卡时未初始化
                    return "未初始化";
                case (int)ErrorDef.InvokeFailed:// = -3,//库函数调用出错
                    return "凌华SDK调用出错";
                case (int)ErrorDef.NotOpen:
                    return "控制卡未打开";
                default:
                    return "未定义的ErrorCode = " + errorCode;
            }
        }

        

        public IJFModule_Motion GetMc(int index)
        {
            return null;
        }

        public JFRealtimeUI GetRealtimeUI()
        {
            JFRealtimeUI ret = new JFRealtimeUI();
            if (!IsDeviceOpen)
                return ret;
            ret.AutoScroll = true;

            UcMotionDaq ui = new UcMotionDaq();
            ui.AutoScroll = true;
            string cardType = DeviceModel;
            int cardIndex = 0;
            object obj = GetInitParamValue(_initParamNames[0]);
            if (null != obj && (CardType)obj != CardType.PCI_Unknown)
                cardType = ((CardType)obj).ToString();
            obj = GetInitParamValue(_initParamNames[0]);
            if (null != obj)
                cardIndex = (int)obj;
            ui.SetDevice(this, cardType + " ID:" + cardIndex);
            return ui;
        }






        public void ShowCfgDialog()
        {
            MessageBox.Show("暂未实现");
        }
    }
}
