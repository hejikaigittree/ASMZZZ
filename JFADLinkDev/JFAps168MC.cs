using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using JFInterfaceDef;
using APS168_W64;
using APS_Define_W32;
using JFToolKits;

namespace JFADLinkDevice
{
    public class JFAps168MC : IJFModule_Motion
    {
        public bool IsOpen { get; private set; }

        public int AxisCount { get; private set; }
        /// <summary>
        /// 报警信号
        /// </summary>
        public int MSID_ALM { get { return 0; } }
        /// <summary>
        /// 正限位
        /// </summary>
        public int MSID_PL { get { return 1; } }
        /// <summary>
        /// 负限位
        /// </summary>
        public int MSID_NL { get { return 2; } }
        /// <summary>
        /// 原点信号
        /// </summary>
        public int MSID_ORG { get { return 3; } }
        /// <summary>
        /// 急停信号
        /// </summary>
        public int MSID_EMG { get { return 4; } }
        /// <summary>
        /// EZ信号
        /// </summary>
        public int MSID_EZ { get { return 5; } }
        /// <summary>
        /// 到位/零速度检出信号
        /// </summary>
        public int MSID_INP { get { return 6; } }
        /// <summary>
        /// 伺服激磁信号
        /// </summary>
        public int MSID_SVO { get { return 7; } }
        /// <summary>
        /// 软正极限信号
        /// </summary>
        public int MSID_SPL { get { return 11; } }
        /// <summary>
        /// 软负极限信号
        /// </summary>
        public int MSID_SNL { get { return 12; } }
        /// <summary>
        /// motion done信号
        /// </summary>
        public int MSID_MDN { get { return 13; } }//APS_motion_status () 
        /// <summary>
        /// 异常停止信号
        /// </summary>
        public int MSID_ASTP { get { return 14; } }//APS_motion_status () 

        /// <summary>
        /// 凌华卡ID
        /// </summary>
        public int BoardID { get; private set; }
        /// <summary>
        /// 凌华运动参数配置文件路径
        /// </summary>
        //public string ConfigPath { get; private set; }
        JFXCfg _jfAPS168Cfg = null; //APS168系列控制卡全局配置对象
        JFXmlDictionary<string, object> _dictMC = null;//用于存储本控制卡的相关拓展参数（脉冲当量）


        private bool bufferAbsMoveStart = false;
        private bool bufferRelMoveStart = false;
        /// <summary>
        /// 各轴的运动参数
        /// </summary>
        JFMotionParam[] _motionParams;
        readonly object ml = new object();//Motion Lock (线程锁)

        /// <summary>
        /// 各轴锁存使能状态
        /// </summary>
        bool[] ltcEnable = null;//private JFXmlDictionary<int, bool> ltcEnable;
        /// <summary>
        /// 各轴锁存触发高低电平状态
        /// </summary>
        bool[] ltcLogic = null;//private JFXmlDictionary<int, bool> ltcLogic;
        /// <summary>
        /// 锁存通道使用状态
        /// </summary>
        bool[] ltcChUsed = null;//private JFXmlDictionary<int, bool> ltcChUsed;
        /// <summary>
        /// 轴与锁存通道绑定关系
        /// </summary>
        int[] chnLtcCh = null;//private JFXmlDictionary<int, int> chnLtcCh;

        /// <summary>
        /// 锁存状态关键字
        /// </summary>
        private string ltcEnableKeyName = "LTCEnable";
        /// <summary>
        /// 触发高低电平关键字
        /// </summary>
        private string ltcLogicKeyName = "LTCLogic";
        /// <summary>
        /// 锁存通道使用状态
        /// </summary>
        private string ltcUsedKeyName = "LTCUsed";
        /// <summary>
        /// 轴与锁存通道的绑定关系
        /// </summary>
        private string chnltcChKeyName = "ChnLTCCh";
        /// <summary>
        /// 脉冲当量关键字
        /// </summary>
        string factorKeyName = "PulseFactor";
        double[] pulseFactors = null; 

        //private JFDev_Aps168MotionDaq AmpMotionDaq;

        internal JFAps168MC(int boardID,JFXCfg/*string _configpath*/jfCfg, JFDev_Aps168MotionDaq _AmpMotionDaq)
        {
            AxisCount = 0;
            IsOpen = false;
            BoardID = boardID;
            //ConfigPath = _configpath;
            _jfAPS168Cfg = jfCfg;

            //AmpMotionDaq = _AmpMotionDaq;
        }

        /// <summary>
        /// 自定义错误信息
        /// </summary>
        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            Unsupported = -1,//设备不支持此功能
            ParamError = -2,//参数错误（不支持的参数）
            InvokeFailed = -3,//库函数调用出错
            Allowed = 1,//调用成功，但不是所有的参数都支持
            InitFailedWhenOpenCard = -4,//
            LtcCHNoIdel = -5,//锁存通道已用完
            NotOpen = -6,
        }
        int StartAxisId = 0;
        /// <summary>运动控制卡MC初始化 </summary>
        internal void Open()
        {
            double Axs_Param = 0;
            int TotalAxis = 0, CardName = 0;
            APS168.APS_get_first_axisId(BoardID, ref StartAxisId, ref TotalAxis);
            APS168.APS_get_card_name(BoardID, ref CardName);
            if (/*CardName != (Int32)APS_Define.DEVICE_NAME_PCI_825458 && */CardName != (Int32)APS_Define.DEVICE_NAME_AMP_20408C)
                throw new Exception(string.Format("AMP204MC.Initialize Failed :运动控制是型号不是204C或208C！"));

            AxisCount = TotalAxis;
            if(AxisCount == 0)
            {
                IsOpen = true;
                return;
            }

            lock(_jfAPS168Cfg)
            {
                if (!_jfAPS168Cfg.ContainsItem("Card_" + BoardID))
                    _jfAPS168Cfg.AddItem("Card_" + BoardID, new JFXmlDictionary<string, object>());
                _dictMC = _jfAPS168Cfg.GetItemValue("Card_" + BoardID) as JFXmlDictionary<string, object>;

                if (!_dictMC.ContainsKey(ltcEnableKeyName))
                    _dictMC.Add(ltcEnableKeyName, new bool[AxisCount]) ;
                else //检查数组长度
                {
                    bool[] baTmp = _dictMC[ltcEnableKeyName] as bool[];
                    if(baTmp.Length  < AxisCount)
                    {
                        _dictMC.Remove(ltcEnableKeyName);
                        _dictMC.Add(ltcEnableKeyName, new bool[AxisCount]);
                    }
                }
                ltcEnable = _dictMC[ltcEnableKeyName] as bool[];
                

                if(!_dictMC.ContainsKey(ltcLogicKeyName))
                    _dictMC.Add(ltcLogicKeyName, new bool[AxisCount]);
                else
                {
                    bool[] baTmp = _dictMC[ltcLogicKeyName] as bool[];
                    if (baTmp.Length < AxisCount)
                    {
                        _dictMC.Remove(ltcLogicKeyName);
                        _dictMC.Add(ltcLogicKeyName, new bool[AxisCount]);
                    }
                }
                ltcLogic = _dictMC[ltcLogicKeyName] as bool[];

                if(!_dictMC.ContainsKey(chnltcChKeyName))
                    _dictMC.Add(chnltcChKeyName, new int[AxisCount]);
                else
                {
                    int[] naTmp = _dictMC[chnltcChKeyName] as int[];
                    if(naTmp.Length < AxisCount)
                    {
                        _dictMC.Remove(chnltcChKeyName);
                        _dictMC.Add(chnltcChKeyName, new int[AxisCount]);
                    }
                }
                chnLtcCh = _dictMC[chnltcChKeyName] as int[];

                if(!_dictMC.ContainsKey(factorKeyName))
                {
                    pulseFactors = new double[AxisCount];
                    for (int i = 0; i < AxisCount; i++)
                        pulseFactors[i] = 1;
                    _dictMC.Add(factorKeyName, pulseFactors);
                }
                else
                {
                    double[] daTmp = _dictMC[factorKeyName] as double[];
                    if (daTmp.Length < AxisCount)
                    {
                        _dictMC.Remove(factorKeyName);
                        pulseFactors = new double[AxisCount];
                        for (int i = 0; i < AxisCount; i++)
                            pulseFactors[i] = 1;
                        _dictMC.Add(factorKeyName, pulseFactors);
                    }
                    else
                        pulseFactors = _dictMC[factorKeyName] as double[];
                }





                _jfAPS168Cfg.Save();
            }


            _motionParams = new JFMotionParam[AxisCount];
            for (int i = 0; i < AxisCount; i++)
            { 
                    _motionParams[i] = new JFMotionParam();
                    if(APS168.APS_get_axis_param_f(i+ StartAxisId,  (Int32)APS_Define.PRA_VS, ref Axs_Param) != 0)
                        throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_get_axis_param_f(AxisID = {0},(Int32)APS_Define.PRA_VS,Axs_Param={1}", i, Axs_Param));
                    _motionParams[i].vs = Axs_Param/ pulseFactors[i];
                    if (APS168.APS_get_axis_param_f(i + StartAxisId, (Int32)APS_Define.PRA_VM, ref Axs_Param) != 0)
                        throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_get_axis_param_f(AxisID = {0},(Int32)APS_Define.PRA_VM,Axs_Param={1}", i, Axs_Param));
                    _motionParams[i].vm = Axs_Param / pulseFactors[i]; ;
                    if (APS168.APS_get_axis_param_f(i+ StartAxisId, (Int32)APS_Define.PRA_VE, ref Axs_Param) != 0)
                        throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_get_axis_param_f(AxisID = {0},(Int32)APS_Define.PRA_VE,Axs_Param={1}", i, Axs_Param));
                    _motionParams[i].ve = Axs_Param / pulseFactors[i]; ;
                    if (APS168.APS_get_axis_param_f(i+ StartAxisId, (Int32)APS_Define.PRA_ACC, ref Axs_Param) != 0)
                        throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_get_axis_param_f(AxisID = {0},(Int32)APS_Define.PRA_ACC,Axs_Param={1}", i, Axs_Param));
                    _motionParams[i].acc = Axs_Param / pulseFactors[i]; ;
                    if (APS168.APS_get_axis_param_f(i + StartAxisId, (Int32)APS_Define.PRA_DEC, ref Axs_Param) != 0)
                        throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_get_axis_param_f(AxisID = {0},(Int32)APS_Define.PRA_DEC,Axs_Param={1}", i, Axs_Param));
                    _motionParams[i].dec = Axs_Param / pulseFactors[i]; ;
                    if (APS168.APS_get_axis_param_f(i + StartAxisId, (Int32)APS_Define.PRA_CURVE, ref Axs_Param) != 0)
                        throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_get_axis_param_f(AxisID = {0},(Int32)APS_Define.PRA_CURVE,Axs_Param={1}", i, Axs_Param));
                    _motionParams[i].curve = Axs_Param;
                    //if (APS168.APS_get_axis_param_f(i, (Int32)APS_Define.PRA_PSR_JERK, ref Axs_Param) != 0)
                    //    throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_get_axis_param_f(AxisID = {0},(Int32)APS_Define.PRA_PSR_JERK,Axs_Param={1}", i, Axs_Param));
                    //_motionParams[i].jerk = Axs_Param;   
                
            }

            int index = 0;
            for (int i = 0; i < FLtcChNum; i++)
            {
                if (APS168.APS_enable_ltc_fifo(BoardID, i, 0) != 0)
                    throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_enable_ltc_fifo(BoardID = {0},FlcCh={1},false", BoardID, i));

                if (APS168.APS_reset_ltc_fifo(BoardID, i) != 0)
                    throw new Exception(string.Format("AMP204MC.Open Failed :APS_reset_ltc_fifo(BoardID = {0},FlcCh={1}", BoardID, i));

                if (APS168.APS_set_ltc_fifo_param(BoardID, i, (Int32)APS_Define.LTC_IPT, TransBitDataToInt(i)) != 0)
                    throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_set_ltc_fifo_param(BoardID = {0},FlcCh={1},(Int32)APS_Define.LTC_ENC,Param_Val={2}", BoardID, i, TransBitDataToInt(i)));

                if (GetEncChnByFLtcChn(i, out index) != 0)
                    continue;

                if (APS168.APS_set_ltc_fifo_param(BoardID, i, (Int32)APS_Define.LTC_ENC, index) != 0)
                    throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_set_ltc_fifo_param(BoardID = {0},FlcCh={1},(Int32)APS_Define.LTC_ENC,Param_Val={2}", BoardID, i, index));

                if (APS168.APS_set_ltc_fifo_param(BoardID, i, (Int32)APS_Define.LTC_LOGIC, ltcLogic[index] ? 0 : 1) != (int)ErrorDef.Success)
                    throw new Exception(string.Format("AMP204MC.Open Failed :APS_set_ltc_fifo_param(BoardID = {0},FlcCh={1},(Int32)APS_Define.LTC_LOGIC,Param_Val={2}", BoardID, i, ltcLogic[index]));

                if (APS168.APS_enable_ltc_fifo(BoardID, i, ltcEnable[index] ? 1 : 0) != 0)
                    throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_enable_ltc_fifo(BoardID = {0},FlcCh={1},{2}", BoardID, i, ltcEnable[index] ? 1 : 0));
            }

            IsOpen = true;
        }

        internal void Close()
        {
            AxisCount = 0;
            _motionParams = null;
            IsOpen = false;
        }

        /// <summary>
        /// 保存轴卡参数到板载Flash
        /// </summary>
        /// <returns></returns>
        internal int SaveParamsToFlash()
        {
            //if (APS168.APS_save_param_to_file(BoardID, ConfigPath) != 0)
            if(0!=APS168.APS_save_parameter_to_flash(BoardID))
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        #region 获取（指定轴的）单个运动状态(IO)
        public bool IsALM(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsAlarm");
            motion_io_value = APS168.APS_motion_io_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> MSID_ALM) & 1);
        }

        public bool IsEMG(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsEMG");
            motion_io_value = APS168.APS_motion_io_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> MSID_EMG) & 1);
        }

        public bool IsINP(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsINP");
            motion_io_value = APS168.APS_motion_io_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> MSID_INP) & 1);
        }

        public bool IsNL(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsNL");
            motion_io_value = APS168.APS_motion_io_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> MSID_NL) & 1);
        }

        public bool IsORG(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsORG");
            motion_io_value = APS168.APS_motion_io_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> MSID_ORG) & 1);
        }

        public bool IsPL(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsPL");
            motion_io_value = APS168.APS_motion_io_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> MSID_PL) & 1);
        }

        public bool IsSNL(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsSNL");
            motion_io_value = APS168.APS_motion_io_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> MSID_SNL) & 1);
        }

        public bool IsSPL(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsSPL");
            motion_io_value = APS168.APS_motion_io_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> MSID_SPL) & 1);
        }

        public bool IsSVO(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsSVO");
            motion_io_value = APS168.APS_motion_io_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> MSID_SVO) & 1);
        }

        public bool IsMDN(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsMDN");
            motion_io_value = APS168.APS_motion_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> 5) & 1);//5对应motion status中的MDN
        }

        public bool IsASTP(int axis)
        {
            Int32 motion_io_value = 0;
            _CheckAxisEnable(axis, "IsASTP");
            motion_io_value = APS168.APS_motion_status(axis + StartAxisId);
            return Convert.ToBoolean((motion_io_value >> 16) & 1);//16对应motion status中的MDN
        }

        public int IsHomeDone(int axis, out bool isDone)
        {
            isDone = IsMDN(axis);
            return (int)ErrorDef.Success; ;
        }
        #endregion

        #region 获取（指定轴的）多个运动状态(IO)
        public int GetMotionStatus(int axis, out bool[] ret)
        {
            Int32 motion_io_value = 0;
            Int32 motion_value = 0;
            _CheckAxisEnable(axis, "GetMotionStatus");
            ret = new bool[15];
           
            motion_io_value = APS168.APS_motion_io_status(axis + StartAxisId);

            ret[MSID_ALM] = Convert.ToBoolean((motion_io_value >> MSID_ALM) & 1);
            ret[MSID_SVO] = Convert.ToBoolean((motion_io_value >> MSID_SVO) & 1);
            ret[MSID_INP] = Convert.ToBoolean((motion_io_value >> MSID_INP) & 1);
            ret[MSID_EMG] = Convert.ToBoolean((motion_io_value >> MSID_EMG) & 1);
            ret[MSID_PL] = Convert.ToBoolean((motion_io_value >> MSID_PL) & 1);
            ret[MSID_NL] = Convert.ToBoolean((motion_io_value >> MSID_NL) & 1);
            ret[MSID_ORG] = Convert.ToBoolean((motion_io_value >> MSID_ORG) & 1);
            ret[MSID_SPL] = Convert.ToBoolean((motion_io_value >> MSID_SPL) & 1);
            ret[MSID_SNL] = Convert.ToBoolean((motion_io_value >> MSID_SNL) & 1);

            motion_value = APS168.APS_motion_status(axis + StartAxisId);
            ret[MSID_MDN] = Convert.ToBoolean((motion_value >> 5) & 1);//5对应MDN
            ret[MSID_ASTP] = Convert.ToBoolean((motion_value >> 16) & 1);//16对应ASTP
           
            return (int)ErrorDef.Success;
        }
        #endregion

        #region 轴参数 
        public int GetPulseFactor(int axis, out double fact)
        {
            fact = 0;
            _CheckAxisEnable(axis, "GetPulseFactor");
            fact = pulseFactors[axis];

            return (int)ErrorDef.Success;
        }

        public int SetPulseFactor(int axis, double plsFactor)
        {
            _CheckAxisEnable(axis, "SetPulseFactor");
            pulseFactors[axis] = plsFactor;
            lock (_jfAPS168Cfg)
                _jfAPS168Cfg.Save();


            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取正软极限
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="enable"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int GetSPLimit(int axis, out bool enable, out double pos)
        {
            Int32 enableSPEL = 0;
            enable = false;
            pos = 0;
            _CheckAxisEnable(axis, "GetSPLimit");

            if ((pulseFactors[axis]) <= 0)
                throw new ArgumentOutOfRangeException("GetSPLimit(axis ,...) fialed By:factor<=0,factor=" + pulseFactors[axis]);

            if (APS168.APS_get_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_SPEL_POS, ref pos) != 0)
                return (int)ErrorDef.InvokeFailed;
            pos = pos / ((double)pulseFactors[axis]);

            if (APS168.APS_get_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_SPEL_EN, ref enableSPEL) != 0)
                return (int)ErrorDef.InvokeFailed;
            enable = (enableSPEL == 2 ? true : false);
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 设置正软极限
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="enable"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int SetSPLimit(int axis, bool enable, double pos)
        {
            _CheckAxisEnable(axis, "SetSPLimit");
            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_SPEL_POS, pos * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;
            if (APS168.APS_set_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_SPEL_EN, enable?2:0) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (SaveParamsToFlash() != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取负软极限
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="enable"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int GetSNLimit(int axis, out bool enable, out double pos)
        {
            Int32 enableSPEL = 0;
            enable = false;
            pos = 0;
            _CheckAxisEnable(axis, "GetSPLimit");

            if ((pulseFactors[ axis]) <= 0)
                throw new ArgumentOutOfRangeException("GetSNLimit(axis ,...) fialed By:factor<=0,factor=" + pulseFactors[axis]);


            if (APS168.APS_get_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_SMEL_POS, ref pos) != 0)
                return (int)ErrorDef.InvokeFailed;
            pos = pos / ((double)pulseFactors[axis]);

            if (APS168.APS_get_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_SMEL_EN, ref enableSPEL) != 0)
                return (int)ErrorDef.InvokeFailed;
            enable = (enableSPEL==2?true:false);
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 设置负软极限
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="enable"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int SetSNLimit(int axis, bool enable, double pos)
        {
            _CheckAxisEnable(axis, "SetSNLimit");
            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_SMEL_POS, pos * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;
            if (APS168.APS_set_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_SMEL_EN, enable?2:0) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (SaveParamsToFlash() != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        public int GetMotionParam(int axis, out JFMotionParam mp)
        {
            _CheckAxisEnable(axis, "GetMotionParam");
            mp = _motionParams[axis];
            return (int)ErrorDef.Success;
        }
        
        /// <summary>
        /// 设置单轴运动参数
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int SetMotionParam(int axis, JFMotionParam mp)
        {
            _CheckAxisEnable(axis, "GetMotionParam");

            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_VS, mp.vs * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_VM, mp.vm * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_VE, mp.ve * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_ACC, mp.acc * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_DEC, mp.dec * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_CURVE, mp.curve) != 0)
                return (int)ErrorDef.InvokeFailed;

            //  if (APS168.APS_set_axis_param_f(axis, (Int32)APS_Define.PRA_PSR_JERK, mp.jerk) != 0)
            //      throw new Exception(string.Format("AMP204MC.Open Failed :APS168.APS_set_axis_param_f(AxisID = {0},(Int32)APS_Define.PRA_PSR_JERK,Axs_Param={1}", axis, mp.jerk));

            _motionParams[axis] = mp;

            if (SaveParamsToFlash() != 0)
                return (int)ErrorDef.InvokeFailed;

            //临时代码 ... 
            //for(int i = 0; i < 32;i++)
            //{
            //    if(0 != (BoardID & 1<<i))
            //    {
            //        APS168.APS_save_parameter_to_flash(i);
            //    }
            //}


            return (int)ErrorDef.Success;
        }

        public int GetHomeParam(int axis, out JFHomeParam pm)
        {
            int iAxs_Param = 0;
            double dAxs_Param = 0;
            pm = new JFHomeParam();
            if (APS168.APS_get_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_MODE, ref iAxs_Param) != 0)
                return (int)ErrorDef.InvokeFailed;
            pm.mode = iAxs_Param;

            if (APS168.APS_get_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_DIR, ref iAxs_Param) != 0)
                return (int)ErrorDef.InvokeFailed;
            pm.dir = (iAxs_Param==0?true:false);

            if (APS168.APS_get_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_EZA, ref iAxs_Param) != 0)
                return (int)ErrorDef.InvokeFailed;
            pm.eza = (iAxs_Param==1?true:false);

            if (APS168.APS_get_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_ACC, ref dAxs_Param) != 0)
                return (int)ErrorDef.InvokeFailed;
            pm.acc = dAxs_Param/ pulseFactors[axis];

            if (APS168.APS_get_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_VM, ref dAxs_Param) != 0)
                return (int)ErrorDef.InvokeFailed;
            pm.vm = dAxs_Param/ pulseFactors[axis];

            if (APS168.APS_get_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_VO, ref dAxs_Param) != 0)
                return (int)ErrorDef.InvokeFailed;
            pm.vo = dAxs_Param / pulseFactors[axis];

            if (APS168.APS_get_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_VO, ref dAxs_Param) != 0)
                return (int)ErrorDef.InvokeFailed;
            pm.va = dAxs_Param / pulseFactors[axis];

            if (APS168.APS_get_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_SHIFT, ref dAxs_Param) != 0)
                return (int)ErrorDef.InvokeFailed;
            pm.shift = dAxs_Param;

            pm.offset = 0;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 设置单轴回零参数
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="hp"></param>
        /// <returns></returns>
        public int SetHomeParam(int axis, JFHomeParam hp)
        {
            _CheckAxisEnable(axis, "SetHomeParam");

            if (APS168.APS_set_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_MODE, (int)hp.mode) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_DIR, hp.dir?0:1) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_EZA, hp.eza?1:0) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_ACC, (double)hp.acc * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_VM, (double)hp.vm * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_VO, (double)hp.vo * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            //if (APS168.APS_set_axis_param_f(axis, (Int32)APS_Define.PRA_HOME_VA, (double)hp.va) != 0)
            //    return (int)ErrorDef.InvokeFailed;

            if (APS168.APS_set_axis_param_f(axis + StartAxisId, (Int32)APS_Define.PRA_HOME_SHIFT, (double)hp.shift * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            //if (APS168.APS_set_axis_param_f(axis, (Int32)APS_Define.PRA_HOME_OFFSET, (double)hp.offset) != 0)
            //    return (int)ErrorDef.InvokeFailed;

            if (SaveParamsToFlash() != 0)
                return (int)ErrorDef.InvokeFailed;

            //临时代码 ... 
            //for(int i = 0; i < 32;i++)
            //{
            //    if(0 != (BoardID & 1<<i))
            //    {
            //        APS168.APS_save_parameter_to_flash(i);
            //    }
            //}
            

            return (int)ErrorDef.Success;
        }
        #endregion

        #region 设置/获取轴位置数据
        /// <summary>
        /// 获取单轴指令位置
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="cmdPos"></param>
        /// <returns></returns>
        public int GetCmdPos(int axis, out double cmdPos)
        {
            cmdPos = 0;
            _CheckAxisEnable(axis, "GetCmdPos");

            if ((pulseFactors[axis]) <= 0)
                throw new ArgumentOutOfRangeException("GetCmdPos(axis ,...) fialed By:factor<=0,factor=" + pulseFactors[axis]);


            //if (APS168.APS_get_command_f(axis, ref cmdPos) != 0)
            if (APS168.APS_get_target_position_f(axis + StartAxisId, ref cmdPos) != 0)
                return (int)ErrorDef.InvokeFailed;

            cmdPos = cmdPos / ((double)pulseFactors[axis]);
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 设置单轴指令位置
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="cmdPos"></param>
        /// <returns></returns>
        public int SetCmdPos(int axis, double cmdPos)
        {
            _CheckAxisEnable(axis, "SetCmdPos");

            if (APS168.APS_set_command_f(axis + StartAxisId, cmdPos * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取单轴反馈位置
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="fbkPos"></param>
        /// <returns></returns>
        public int GetFbkPos(int axis, out double fbkPos)
        {
            fbkPos = 0;
            _CheckAxisEnable(axis, "GetFbkPos");

            if ((pulseFactors[axis]) <= 0)
                throw new ArgumentOutOfRangeException("GetFbkPos(axis ,...) fialed By:factor<=0,factor=" + pulseFactors[axis]);


            if (APS168.APS_get_position_f(axis + StartAxisId, ref fbkPos) != 0)
                return (int)ErrorDef.InvokeFailed;

            fbkPos = fbkPos / ((double)pulseFactors[axis]);
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 设置单轴指令位置
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="fbkPos"></param>
        /// <returns></returns>
        public int SetFbkPos(int axis, double fbkPos)
        {
            _CheckAxisEnable(axis, "SetFbkPos");

            if (APS168.APS_set_position_f(axis + StartAxisId, fbkPos * pulseFactors[axis]) != 0)
                return (int)ErrorDef.InvokeFailed;

            return (int)ErrorDef.Success;
        }
        #endregion

        #region 启动/停止 清除报警 归零
        public int ClearAlarm(int axis)//204、208不可以软件直接清除轴报错，若要实现需占用一个DO，作为输出来清除驱动器报错
        {
            _CheckAxisEnable(axis, "ClearAlarm");
            Int32 do_group = 0;
            Int32 do_data = 1;
            if (!IsOpen)
            {
                return (int)ErrorDef.InvokeFailed;
            }
            if (APS168.APS_write_d_channel_output(BoardID, do_group, axis, do_data) != 0)
                return (int)ErrorDef.InvokeFailed;

            return (int)ErrorDef.Success;
        }

        const Int32 ON = 1;
        const Int32 OFF = 0;

        /// <summary>
        /// 励磁
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public int ServoOn(int axis)
        {
            _CheckAxisEnable(axis, "ServoOn");
            if (APS168.APS_set_servo_on(axis + StartAxisId, ON) != (int)APS_Define.ERR_NoError)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 励磁OFF
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public int ServoOff(int axis)
        {
            _CheckAxisEnable(axis, "ServoOff");
            if (APS168.APS_set_servo_on(axis + StartAxisId, OFF) != (int)APS_Define.ERR_NoError)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        public void Stop()
        {
            for (int i = 0; i < AxisCount; i++)
            {
                APS168.APS_stop_move(i + StartAxisId) ;
            }
        }

        /// <summary>
        /// 单轴停止
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public int StopAxis(int axis)
        {
            _CheckAxisEnable(axis, "StopAxis");
            if (APS168.APS_stop_move(axis + StartAxisId) != (int)APS_Define.ERR_NoError)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 单轴急停
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public int StopAxisEmg(int axis)
        {
            _CheckAxisEnable(axis, "StopAxisEmg");
            if (APS168.APS_emg_stop(axis + StartAxisId) != (int)APS_Define.ERR_NoError)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 所有轴急停
        /// </summary>
        public void StopEmg()
        {
            for (int i = 0; i < AxisCount; i++)
            {
                APS168.APS_emg_stop(i + StartAxisId);
            }
        }

        /// <summary>
        /// 单轴回原点
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public int Home(int axis)
        {
            _CheckAxisEnable(axis, "Home");
            if (APS168.APS_home_move(axis + StartAxisId) != (int)APS_Define.ERR_NoError)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }
        #endregion

        #region 单轴运动
        public int AbsMove(int axis, double position)
        {
            _CheckAxisEnable(axis, "AbsMove");
            lock (ml)
            {
                //int a = APS168.APS_absolute_move(axis, (Int32)(position * pulseFactors[axis]), (int)_motionParams[axis].vm * pulseFactors[axis]);
                if (APS168.APS_absolute_move(axis + StartAxisId, (Int32)(position * pulseFactors[axis]), (int)(_motionParams[axis].vm * pulseFactors[axis])) != 0)
                    return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }

        public int RelMove(int axis, double distance)
        {
            _CheckAxisEnable(axis, "RelMove");
            lock (ml)
            {
                if (APS168.APS_relative_move(axis + StartAxisId, (Int32)(distance * pulseFactors[axis]), (int)(_motionParams[axis].vm * pulseFactors[axis])) != 0)
                    return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 单轴jog运动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="isPositive"></param>
        /// <returns></returns>
        public int Jog(int axis, bool isPositive)
        {
            int STA_On=1;
            int STA_Off = 0;
            _CheckAxisEnable(axis, "Jog");
            lock (ml)
            {
                if (APS168.APS_set_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_JG_MODE, 0) != 0)  // Set jog mode
                    return (int)ErrorDef.InvokeFailed;
                if (APS168.APS_set_axis_param(axis + StartAxisId, (Int32)APS_Define.PRA_JG_DIR, (isPositive?0:1)) != 0)  // Set jog direction
                    return (int)ErrorDef.InvokeFailed;
                if (APS168.APS_jog_start(axis + StartAxisId, STA_Off) != 0)
                    return (int)ErrorDef.InvokeFailed;
                if (APS168.APS_jog_start(axis + StartAxisId, STA_On) != 0)
                    return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }

        public int VelMove(int axis, double velocity, bool isPositive)
        {
            _CheckAxisEnable(axis, "VelMove");
            lock (ml)
            {
                ASYNCALL p = new ASYNCALL();
                if (APS168.APS_vel(axis + StartAxisId, (isPositive?0:1), velocity * pulseFactors[axis], ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }

        public int VelMove_P(int axis, JFMotionParam mp, bool isPositive)
        {
            _CheckAxisEnable(axis, "VelMove_P");
            lock (ml)
            {
                ASYNCALL p = new ASYNCALL();
                if (APS168.APS_vel_all(axis + StartAxisId, (isPositive?0:1),mp.vs * pulseFactors[axis], mp.vm * pulseFactors[axis], mp.ve * pulseFactors[axis], mp.acc * pulseFactors[axis], mp.dec * pulseFactors[axis], mp.curve, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }
        #endregion

        #region 插补运动     
        public int AbsLine(int[] axisList, double[] posList)
        {
            _CheckAxisEnable(axisList, "AbsLine");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("AbsLine(int[] axisList, double[] posList) failed By: posList.Count() != axisList.Count()");
            lock (ml)
            {
                int[] axisListMap = new int[axisList.Length];
                for (int i=0;i<axisList.Length;i++)
                {
                    posList[i] = posList[i] * pulseFactors[axisList[i]];
                    axisListMap[i] = axisList[i] + StartAxisId;
                }
                double TransPara = 0;
                ASYNCALL p = new ASYNCALL();

                
                    

                if (APS168.APS_line(axisList.Count(), axisListMap, (Int32)APS_Define.OPT_ABSOLUTE, posList, ref TransPara, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 以制定的运动参数作多轴直线插补（绝对方式）
        /// </summary>
        /// <param name="axisList"></param>
        /// <param name="posList"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int AbsLine_P(int[] axisList, double[] posList, JFMotionParam mp)
        {
            _CheckAxisEnable(axisList, "AbsLine_P");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("AbsLine_P(int[] axisList, JFMotionParam mp) failed By: posList.Count() != axisList.Count()");
            lock (ml)
            {
                int[] axisListMap = new int[axisList.Length];
                for (int i = 0; i < axisList.Length; i++)
                {
                    posList[i] = posList[i] * pulseFactors[axisList[i]];
                        axisListMap[i] = axisList[i] + StartAxisId;
                    }

                double TransPara = 0;
                ASYNCALL p = new ASYNCALL();
                if (APS168.APS_line_all(axisList.Count(), axisListMap, (Int32)APS_Define.OPT_ABSOLUTE, posList, ref TransPara,mp.vs * pulseFactors[axisList[0]], mp.vm * pulseFactors[axisList[0]], mp.ve * pulseFactors[axisList[0]], mp.acc * pulseFactors[axisList[0]], mp.dec * pulseFactors[axisList[0]], mp.curve, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        public int RelLine(int[] axisList, double[] posList)
        {
            _CheckAxisEnable(axisList, "RelLine");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("RelLine(int[] axisList, double[] posList) failed By: posList.Count() != axisList.Count()");
            lock (ml)
            {
                int[] axisListMap = new int[axisList.Length];
                for (int i = 0; i < axisList.Length; i++)
                {
                    posList[i] = posList[i] * pulseFactors[axisList[i]];
                    axisListMap[i] = axisList[i] + StartAxisId;
                }

                double TransPara = 0;
                ASYNCALL p = new ASYNCALL();
                if (APS168.APS_line(axisList.Count(), axisListMap, (Int32)APS_Define.OPT_RELATIVE, posList, ref TransPara, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 以制定的运动参数作多轴直线插补（相对方式）
        /// </summary>
        /// <param name="axisList"></param>
        /// <param name="posList"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int RelLine_P(int[] axisList, double[] posList, JFMotionParam mp)
        {
            _CheckAxisEnable(axisList, "RelLine_P");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("RelLine_P(int[] axisList, JFMotionParam mp) failed By: posList.Count() != axisList.Count()");
            lock (ml)
            {
                int[] axisListMap = new int[axisList.Length];
                for (int i = 0; i < axisList.Length; i++)
                {
                    posList[i] = posList[i] * pulseFactors[axisList[i]];
                    axisListMap[i] = axisList[i] + StartAxisId;
                }

                double TransPara = 0;
                ASYNCALL p = new ASYNCALL();
                if (APS168.APS_line_all(axisList.Count(), axisListMap, (Int32)APS_Define.OPT_RELATIVE, posList, ref TransPara, mp.vs, mp.vm, mp.ve, mp.acc, mp.dec, mp.curve, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        public int AbsArc2CA(int axis1, int axis2, double center1, double center2, double angle)
        {
            _CheckAxisEnable(axis1, "AbsArc2CA");
            _CheckAxisEnable(axis2, "AbsArc2CA");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1 + StartAxisId;
            Axis_ID_Array[2] = axis2 + StartAxisId;

            double[] CenterArray = new double[2];
            CenterArray[0] = center1 * pulseFactors[axis1];
            CenterArray[1] = center2 * pulseFactors[axis2];

            double TransPara = 0;
            ASYNCALL p = new ASYNCALL();
            lock (ml)
            {
                if (APS168.APS_arc2_ca(Axis_ID_Array, (Int32)APS_Define.OPT_ABSOLUTE, CenterArray,angle, ref TransPara, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 以指定的运动参数做圆弧插补运动（绝对方式，CA）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="angle"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int AbsArc2CA_P(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "AbsArc2CA_P");
            _CheckAxisEnable(axis2, "AbsArc2CA_P");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1 + StartAxisId;
            Axis_ID_Array[2] = axis2 + StartAxisId;

            double[] CenterArray = new double[2];
            CenterArray[0] = center1 * pulseFactors[axis1];
            CenterArray[1] = center2 * pulseFactors[axis2];

            double TransPara = 0;
            ASYNCALL p = new ASYNCALL();
            lock (ml)
            {
                if (APS168.APS_arc2_ca_all(Axis_ID_Array, (Int32)APS_Define.OPT_ABSOLUTE, CenterArray, angle, ref TransPara,mp.vs,mp.vm,mp.ve,mp.acc,mp.dec,mp.curve, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 以绝对方式做圆弧插补运动（CE）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="isPositive"></param>
        /// <returns></returns>
        public int AbsArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive)
        {
            _CheckAxisEnable(axis1, "AbsArc2CE");
            _CheckAxisEnable(axis2, "AbsArc2CE");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1 + StartAxisId;
            Axis_ID_Array[2] = axis2 + StartAxisId;

            double[] CenterArray = new double[2];
            CenterArray[0] = center1 * pulseFactors[axis1];
            CenterArray[1] = center2 * pulseFactors[axis2];

            double[] EndArray = new double[2];
            EndArray[0] = pos1 * pulseFactors[axis1];
            EndArray[1] = pos2 * pulseFactors[axis2];

            double TransPara = 0;
            ASYNCALL p = new ASYNCALL();
            lock (ml)
            {
                if (APS168.APS_arc2_ce(Axis_ID_Array, (Int32)APS_Define.OPT_ABSOLUTE, CenterArray, EndArray,(short)(isPositive?0:1), ref TransPara, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            };
        }

        /// <summary>
        /// 以指定的运动参数做圆弧插补运动（绝对方式，CE）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="isPositive"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int AbsArc2CE_P(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "AbsArc2CE_P");
            _CheckAxisEnable(axis2, "AbsArc2CE_P");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1 + StartAxisId;
            Axis_ID_Array[2] = axis2 + StartAxisId;

            double[] CenterArray = new double[2];
            CenterArray[0] = center1 * pulseFactors[axis1];
            CenterArray[1] = center2 * pulseFactors[axis2];

            double[] EndArray = new double[2];
            EndArray[0] = pos1 * pulseFactors[axis1];
            EndArray[1] = pos2 * pulseFactors[axis2];

            double TransPara = 0;
            ASYNCALL p = new ASYNCALL();
            lock (ml)
            {
                if (APS168.APS_arc2_ce_all(Axis_ID_Array, (Int32)APS_Define.OPT_ABSOLUTE, CenterArray, EndArray, (short)(isPositive?0:1), ref TransPara, mp.vs, mp.vm, mp.ve, mp.acc, mp.dec, mp.curve, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 以相对方式做圆弧插补运动（CA）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public int RelArc2CA(int axis1, int axis2, double center1, double center2, double angle)
        {
            _CheckAxisEnable(axis1, "RelArc2CA");
            _CheckAxisEnable(axis2, "RelArc2CA");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1 + StartAxisId;
            Axis_ID_Array[2] = axis2 + StartAxisId;

            double[] CenterArray = new double[2];
            CenterArray[0] = center1 * pulseFactors[axis1];
            CenterArray[1] = center2 * pulseFactors[axis2];

            double TransPara = 0;
            ASYNCALL p = new ASYNCALL();
            lock (ml)
            {
                if (APS168.APS_arc2_ca(Axis_ID_Array, (Int32)APS_Define.OPT_RELATIVE, CenterArray, angle, ref TransPara, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 以指定的运动参数做圆弧插补运动（相对方式，CA）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="angle"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int RelArc2CA_P(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "RelArc2CA_P");
            _CheckAxisEnable(axis2, "RelArc2CA_P");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1 + StartAxisId;
            Axis_ID_Array[2] = axis2 + StartAxisId;

            double[] CenterArray = new double[2];
            CenterArray[0] = center1 * pulseFactors[axis1];
            CenterArray[1] = center2 * pulseFactors[axis2];

            double TransPara = 0;
            ASYNCALL p = new ASYNCALL();
            lock (ml)
            {
                if (APS168.APS_arc2_ca_all(Axis_ID_Array, (Int32)APS_Define.OPT_RELATIVE, CenterArray, angle, ref TransPara, mp.vs, mp.vm, mp.ve, mp.acc, mp.dec, mp.curve, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 以相对方式做圆弧插补运动（CE）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="isPositive"></param>
        /// <returns></returns>
        public int RelArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive)
        {
            _CheckAxisEnable(axis1, "RelArc2CE");
            _CheckAxisEnable(axis2, "RelArc2CE");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1 + StartAxisId;
            Axis_ID_Array[2] = axis2 + StartAxisId;

            double[] CenterArray = new double[2];
            CenterArray[0] = center1 * pulseFactors[axis1];
            CenterArray[1] = center2 * pulseFactors[axis2];

            double[] EndArray = new double[2];
            EndArray[0] = pos1 * pulseFactors[axis1];
            EndArray[1] = pos2 * pulseFactors[axis2];

            double TransPara = 0;
            ASYNCALL p = new ASYNCALL();
            lock (ml)
            {
                if (APS168.APS_arc2_ce(Axis_ID_Array, (Int32)APS_Define.OPT_RELATIVE, CenterArray, EndArray, (short)(isPositive?0:1), ref TransPara, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            };
        }

        /// <summary>
        /// 以指定的运动参数做圆弧插补运动（相对方式，CE）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="isPositive"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int RelArc2CE_P(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "RelArc2CE_P");
            _CheckAxisEnable(axis2, "RelArc2CE_P");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1 + StartAxisId;
            Axis_ID_Array[2] = axis2 + StartAxisId;

            double[] CenterArray = new double[2];
            CenterArray[0] = center1 * pulseFactors[axis1];
            CenterArray[1] = center2 * pulseFactors[axis2];

            double[] EndArray = new double[2];
            EndArray[0] = pos1 * pulseFactors[axis1];
            EndArray[1] = pos2 * pulseFactors[axis2];

            double TransPara = 0;
            ASYNCALL p = new ASYNCALL();
            lock (ml)
            {
                if (APS168.APS_arc2_ce_all(Axis_ID_Array, (Int32)APS_Define.OPT_RELATIVE, CenterArray, EndArray, (short)(isPositive?0:1), ref TransPara, mp.vs, mp.vm, mp.ve, mp.acc, mp.dec, mp.curve, ref p) != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        #endregion

        #region 缓存式运动
        /// <summary>
        /// 以绝对方式做线性缓存式运动
        /// </summary>
        /// <param name="axisList"></param>
        /// <param name="posList"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int BuffAbsLine(int[] axisList, double[] posList, JFMotionParam mp)
        {
            _CheckAxisEnable(axisList, "BuffAbsLine");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("BuffAbsLine(int[] axisList, double[] posList) failed By: posList.Count() != axisList.Count()");

            PTLINE Line = new PTLINE();
            PTSTS Status = new PTSTS();
            int dimension = axisList.Length;//?D point table
            int ret = 0;
            int ptbId = 0;//Point table id 0

            if (!bufferAbsMoveStart)
            {
                //Enable point table
                ret = APS168.APS_pt_disable(BoardID, ptbId);    if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_enable(BoardID, ptbId, dimension, axisList);    if (ret != 0) return (int)ErrorDef.InvokeFailed;
                //Configuration
                ret = APS168.APS_pt_set_absolute(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_trans_buffered(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }

            //Get status
            //BitSts;	//b0: Is PTB work? [1:working, 0:Stopped]
            //b1: Is point buffer full? [1:full, 0:not full]
            //b2: Is point buffer empty? [1:empty, 0:not empty]
            //b3~b5: reserved

            ret = APS168.APS_get_pt_status(BoardID, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 2nd move profile
                ret = APS168.APS_pt_set_acc(BoardID, ptbId, mp.acc); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_dec(BoardID, ptbId, mp.dec); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_vm(BoardID, ptbId, mp.vm); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_ve(BoardID, ptbId, mp.ve); if (ret != 0) return (int)ErrorDef.InvokeFailed;

                //Set pt line data
                Line.Dim = dimension;
                Line.Pos = new Double[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                for(int i=0;i<dimension;i++)
                {
                    Line.Pos[i] = posList[i] * pulseFactors[axisList[i]];
                }

                //Push 2nd point to buffer
                ret = APS168.APS_pt_line(BoardID, ptbId, ref Line, ref Status); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }
            else
                return (int)ErrorDef.InvokeFailed;

            if (!bufferAbsMoveStart)
            {
                ret = APS168.APS_pt_start(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                bufferAbsMoveStart = true;
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 以相对方式做线性缓存式运动
        /// </summary>
        /// <param name="axisList"></param>
        /// <param name="posList"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int BuffRelLine(int[] axisList, double[] posList, JFMotionParam mp)
        {
            _CheckAxisEnable(axisList, "BuffRelLine");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("BuffRelLine(int[] axisList, double[] posList) failed By: posList.Count() != axisList.Count()");

            PTLINE Line = new PTLINE();
            PTSTS Status = new PTSTS();
            int dimension = axisList.Length;//?D point table
            int ret = 0;
            int ptbId = 1;//Point table id 1

            if (dimension > 6)
                throw new ArgumentException("BuffAbsLine(int[] axisList, double[] posList) failed By: dimension>6");

            if (!bufferRelMoveStart)
            {
                //Enable point table
                ret = APS168.APS_pt_disable(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_enable(BoardID, ptbId, dimension, axisList); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                //Configuration
                ret = APS168.APS_pt_set_relative(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_trans_buffered(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }

            //Get status
            //BitSts;	//b0: Is PTB work? [1:working, 0:Stopped]
            //b1: Is point buffer full? [1:full, 0:not full]
            //b2: Is point buffer empty? [1:empty, 0:not empty]
            //b3~b5: reserved

            ret = APS168.APS_get_pt_status(BoardID, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 2nd move profile
                ret = APS168.APS_pt_set_acc(BoardID, ptbId, mp.acc); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_dec(BoardID, ptbId, mp.dec); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_vm(BoardID, ptbId, mp.vm); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_ve(BoardID, ptbId, mp.ve); if (ret != 0) return (int)ErrorDef.InvokeFailed;

                //Set pt line data
                Line.Dim = dimension;
                Line.Pos = new Double[] { 0, 0, 0, 0, 0, 0,0,0 };
                for (int i = 0; i < dimension; i++)
                {
                    Line.Pos[i] = posList[i] * pulseFactors[axisList[i]];
                }

                //Push 2nd point to buffer
                ret = APS168.APS_pt_line(BoardID, ptbId, ref Line, ref Status); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }
            else
                return (int)ErrorDef.InvokeFailed;

            if (!bufferRelMoveStart)
            {
                ret = APS168.APS_pt_start(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                bufferRelMoveStart = true;
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 以绝对方式做圆弧缓存式运动（CA）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="angle"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int BuffAbsArc2CA(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "BuffAbsArc2CA");
            _CheckAxisEnable(axis2, "BuffAbsArc2CA");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1;
            Axis_ID_Array[1] = axis2;

            PTA2CA Arc2 = new PTA2CA();
            PTSTS Status = new PTSTS();
            int dimension = 2;//2D point table
            int ret = 0;
            int ptbId = 0;//Point table id 0

            if (!bufferAbsMoveStart)
            {
                //Enable point table
                ret = APS168.APS_pt_disable(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_enable(BoardID, ptbId, dimension, Axis_ID_Array); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                //Configuration
                ret = APS168.APS_pt_set_absolute(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_trans_buffered(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }

            //Get status
            //BitSts;	//b0: Is PTB work? [1:working, 0:Stopped]
            //b1: Is point buffer full? [1:full, 0:not full]
            //b2: Is point buffer empty? [1:empty, 0:not empty]
            //b3~b5: reserved

            ret = APS168.APS_get_pt_status(BoardID, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 2nd move profile
                ret = APS168.APS_pt_set_acc(BoardID, ptbId, mp.acc); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_dec(BoardID, ptbId, mp.dec); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_vm(BoardID, ptbId, mp.vm); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_ve(BoardID, ptbId, mp.ve); if (ret != 0) return (int)ErrorDef.InvokeFailed;

                //Set pt arc data
                Arc2.Center = new double[] { center1 * pulseFactors[axis1], center2 * pulseFactors[axis2] };
                Arc2.Angle = (angle) * 3.14159265 / 180.0;   //180 degree((180) * 3.14159265 / 180.0)
                Arc2.Index = new Byte[] { (Byte)axis1, (Byte)axis2 };

                //Push 2nd point to buffer
                ret = APS168.APS_pt_arc2_ca(BoardID, ptbId, ref Arc2, ref Status);  if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }
            else
                return (int)ErrorDef.InvokeFailed;

            if (!bufferAbsMoveStart)
            {
                ret = APS168.APS_pt_start(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                bufferAbsMoveStart = true;
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 以相对方式做圆弧缓存式运动（CA）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="angle"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int BuffRelArc2CA(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "BuffRelArc2CA");
            _CheckAxisEnable(axis2, "BuffRelArc2CA");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1;
            Axis_ID_Array[1] = axis2;

            PTA2CA Arc2 = new PTA2CA();
            PTSTS Status = new PTSTS();
            int dimension = 2;//2D point table
            int ret = 0;
            int ptbId = 1;//Point table id 0

            if (!bufferRelMoveStart)
            {
                //Enable point table
                ret = APS168.APS_pt_disable(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_enable(BoardID, ptbId, dimension, Axis_ID_Array); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                //Configuration
                ret = APS168.APS_pt_set_relative(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_trans_buffered(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }

            //Get status
            //BitSts;	//b0: Is PTB work? [1:working, 0:Stopped]
            //b1: Is point buffer full? [1:full, 0:not full]
            //b2: Is point buffer empty? [1:empty, 0:not empty]
            //b3~b5: reserved

            ret = APS168.APS_get_pt_status(BoardID, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 2nd move profile
                ret = APS168.APS_pt_set_acc(BoardID, ptbId, mp.acc); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_dec(BoardID, ptbId, mp.dec); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_vm(BoardID, ptbId, mp.vm); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_ve(BoardID, ptbId, mp.ve); if (ret != 0) return (int)ErrorDef.InvokeFailed;

                //Set pt arc data
                Arc2.Center = new double[] { center1 * pulseFactors[axis1], center2 * pulseFactors[axis2] };
                Arc2.Angle = (angle) * 3.14159265 / 180.0;   //180 degree((180) * 3.14159265 / 180.0)
                Arc2.Index = new Byte[] { (Byte)axis1, (Byte)axis2 };

                //Push 2nd point to buffer
                ret = APS168.APS_pt_arc2_ca(BoardID, ptbId, ref Arc2, ref Status); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }
            else
                return (int)ErrorDef.InvokeFailed;

            if (!bufferRelMoveStart)
            {
                ret = APS168.APS_pt_start(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                bufferRelMoveStart = true;
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 以绝对方式做圆弧缓存式运动（CE）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="isPositive"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int BuffAbsArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "BuffAbsArc2CE");
            _CheckAxisEnable(axis2, "BuffAbsArc2CE");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1;
            Axis_ID_Array[1] = axis2;

            PTA2CE Arc2 = new PTA2CE();
            PTSTS Status = new PTSTS();
            int dimension = 2;//2D point table
            int ret = 0;
            int ptbId = 0;//Point table id 0

            if (!bufferAbsMoveStart)
            {
                //Enable point table
                ret = APS168.APS_pt_disable(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_enable(BoardID, ptbId, dimension, Axis_ID_Array); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                //Configuration
                ret = APS168.APS_pt_set_absolute(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_trans_buffered(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }

            //Get status
            //BitSts;	//b0: Is PTB work? [1:working, 0:Stopped]
            //b1: Is point buffer full? [1:full, 0:not full]
            //b2: Is point buffer empty? [1:empty, 0:not empty]
            //b3~b5: reserved

            ret = APS168.APS_get_pt_status(BoardID, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 2nd move profile
                ret = APS168.APS_pt_set_acc(BoardID, ptbId, mp.acc); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_dec(BoardID, ptbId, mp.dec); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_vm(BoardID, ptbId, mp.vm); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_ve(BoardID, ptbId, mp.ve); if (ret != 0) return (int)ErrorDef.InvokeFailed;

                //Set pt arc data
                Arc2.Center = new double[] { center1 * pulseFactors[axis1], center2 * pulseFactors[axis2] };
                Arc2.End = new double[] { pos1 * pulseFactors[axis1], pos2 * pulseFactors[axis2] };
                Arc2.Index = new Byte[] { (Byte)axis1, (Byte)axis2 };

                //Push 2nd point to buffer
                ret = APS168.APS_pt_arc2_ce(BoardID, ptbId, ref Arc2, ref Status); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }
            else
                return (int)ErrorDef.InvokeFailed;

            if (!bufferAbsMoveStart)
            {
                ret = APS168.APS_pt_start(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                bufferAbsMoveStart = true;
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 以相对方式做圆弧缓存式运动（CE）
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="isPositive"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int BuffRelArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "BuffRelArc2CE");
            _CheckAxisEnable(axis2, "BuffRelArc2CE");

            int[] Axis_ID_Array = new int[2];
            Axis_ID_Array[0] = axis1;
            Axis_ID_Array[1] = axis2;

            PTA2CE Arc2 = new PTA2CE();
            PTSTS Status = new PTSTS();
            int dimension = 2;//2D point table
            int ret = 0;
            int ptbId = 1;//Point table id 0

            if (!bufferRelMoveStart)
            {
                //Enable point table
                ret = APS168.APS_pt_disable(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_enable(BoardID, ptbId, dimension, Axis_ID_Array); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                //Configuration
                ret = APS168.APS_pt_set_relative(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_trans_buffered(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }

            //Get status
            //BitSts;	//b0: Is PTB work? [1:working, 0:Stopped]
            //b1: Is point buffer full? [1:full, 0:not full]
            //b2: Is point buffer empty? [1:empty, 0:not empty]
            //b3~b5: reserved

            ret = APS168.APS_get_pt_status(BoardID, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 2nd move profile
                ret = APS168.APS_pt_set_acc(BoardID, ptbId, mp.acc); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_dec(BoardID, ptbId, mp.dec); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_vm(BoardID, ptbId, mp.vm); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                ret = APS168.APS_pt_set_ve(BoardID, ptbId, mp.ve); if (ret != 0) return (int)ErrorDef.InvokeFailed;

                //Set pt arc data
                Arc2.Center = new double[] { center1 * pulseFactors[axis1], center2 * pulseFactors[axis2] };
                Arc2.End = new double[] { pos1 * pulseFactors[axis1], pos2 * pulseFactors[axis2] };
                Arc2.Index = new Byte[] { (Byte)axis1, (Byte)axis2 };

                //Push 2nd point to buffer
                ret = APS168.APS_pt_arc2_ce(BoardID, ptbId, ref Arc2, ref Status); if (ret != 0) return (int)ErrorDef.InvokeFailed;
            }
            else
                return (int)ErrorDef.InvokeFailed;

            if (!bufferRelMoveStart)
            {
                ret = APS168.APS_pt_start(BoardID, ptbId); if (ret != 0) return (int)ErrorDef.InvokeFailed;
                bufferRelMoveStart = true;
            }
            return (int)ErrorDef.Success;
        }
        #endregion

        #region 轴位置锁存
        /// <summary>
        /// 获取轴锁存使能状态
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public int GetLtcEnabled(int axis, out bool enabled)
        {
            enabled = false;
            _CheckAxisEnable(axis, "GetLtcEnabled");
            if (!IsOpen)
                return (int)ErrorDef.InitFailedWhenOpenCard;

                enabled = ltcEnable[axis];
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 设置轴锁存使能状态
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public int SetLtcEnabled(int axis, bool enabled)
        {
            int index = 0;
            _CheckAxisEnable(axis, "SetLtcEnabled");
 
                if (!IsOpen)
                    return (int)ErrorDef.NotOpen;

                if (GetLtcCHIdleSource(axis, out index) != (int)ErrorDef.Success)
                    return (int)ErrorDef.LtcCHNoIdel;

                if (APS168.APS_set_ltc_fifo_param(BoardID, index, (Int32)APS_Define.LTC_ENC, axis) != 0)
                    return (int)ErrorDef.InvokeFailed;

                if (APS168.APS_enable_ltc_fifo(BoardID, index, (enabled ? 1 : 0)) != 0)
                    return (int)ErrorDef.InvokeFailed;
                if(APS168.APS_save_parameter_to_flash(BoardID) != 0)
                return (int)ErrorDef.InvokeFailed;
            lock (_jfAPS168Cfg)
            {

                if (enabled)
                {
                    ltcChUsed[index] = true;
                    chnLtcCh[axis] = index;
                }
                else
                {
                    if (RemoveChnLtcBind(axis) != (int)ErrorDef.Success)//解除轴号与锁存通道的绑定关系
                        return (int)ErrorDef.LtcCHNoIdel;
                }


                ltcEnable[axis] = enabled;
                _jfAPS168Cfg.Save();
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 获取轴锁存触发电平
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="isHighLevel"></param>
        /// <returns></returns>
        public int GetLtcLogic(int axis, out bool isHighLevel)
        {
            isHighLevel = false;
            _CheckAxisEnable(axis, "GetLtcLogic");
            if (!IsOpen)
                return (int)ErrorDef.InitFailedWhenOpenCard;

            int iLtcLogic = 0;
            if (APS168.APS_get_ltc_fifo_param(BoardID, axis, (Int32)APS_Define.LTC_LOGIC, ref iLtcLogic) != 0)
                return (int)ErrorDef.InvokeFailed;
            isHighLevel = (iLtcLogic==0?true:false);
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 设置轴锁存触发电平
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="isHighLevel"></param>
        /// <returns></returns>
        public int SetLtcLogic(int axis, bool isHighLevel)
        {
            _CheckAxisEnable(axis, "SetLtcLogic");
            if (!IsOpen)
                return (int)ErrorDef.InitFailedWhenOpenCard;

            if (APS168.APS_set_ltc_fifo_param(BoardID, axis, (Int32)APS_Define.LTC_LOGIC, isHighLevel?0:1) != 0)
                return (int)ErrorDef.InvokeFailed;


                ltcLogic[axis] = isHighLevel;

            _jfAPS168Cfg.Save();
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 清空锁存缓存数据
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public int ClearLtcBuff(int axis)
        {
            _CheckAxisEnable(axis, "ClearLtcBuff");
            if (!IsOpen)
                return (int)ErrorDef.InitFailedWhenOpenCard;

            if (APS168.APS_reset_ltc_fifo(BoardID,axis) != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }
        
        /// <summary>
        /// 获取锁存位置数据
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="buff"></param>
        /// <returns></returns>
        public int GetLtcBuff(int axis, out double[] buff)
        {
            buff = null;
            _CheckAxisEnable(axis, "ClearLtcBuff");
            if (!IsOpen)
                return (int)ErrorDef.InitFailedWhenOpenCard;

            int ArraySize = 0;

            LATCH_POINT[] lATCH_POINT = new LATCH_POINT[5000];
            if (APS168.APS_get_ltc_fifo_point(BoardID, axis, ref ArraySize ,ref lATCH_POINT[0]) != 0)
                return (int)ErrorDef.InvokeFailed;

            buff = new double[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                buff[i] = lATCH_POINT[i].position / ((double)pulseFactors[axis]);
            }
            return (int)ErrorDef.Success;
        }

        int FLtcChNum = 4;
        /// <summary>
        /// 查询锁存通道
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetLtcCHIdleSource(int encChn, out int index)
        {
            index = 0;
            if (chnLtcCh.Contains(encChn))
            {
                index = chnLtcCh[encChn];
                return (int)ErrorDef.LtcCHNoIdel;
            }
            else
            {
                for (int i = 0; i < FLtcChNum; i++)
                {
                    if (!ltcChUsed[i])
                    {
                        index = i;
                        return (int)ErrorDef.Success;
                    }
                }
            }
            return (int)ErrorDef.LtcCHNoIdel;
        }

        /// <summary>
        /// 解除轴与锁存通道的绑定关系
        /// </summary>
        /// <param name="encChn"></param>
        /// <returns></returns>
        private int RemoveChnLtcBind(int encChn)
        {
  
                    ltcChUsed[chnLtcCh[encChn]] = false;
 
            
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 通过锁存通道获得对应的轴号
        /// </summary>
        /// <param name="ltcChn"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetEncChnByFLtcChn(int ltcChn, out int index)
        {
            index = 0;
            if (chnLtcCh == null)
                return (int)ErrorDef.LtcCHNoIdel;
            for (int i = 0; i < chnLtcCh.Length; i++)
            {
                if (chnLtcCh[i] == ltcChn)
                {
                    index = i;
                    return (int)ErrorDef.Success;
                }
            }
            return (int)ErrorDef.LtcCHNoIdel;
        }
        #endregion

        /// <summary>
        /// 获取当前异常信息
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
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
                case (int)ErrorDef.InitFailedWhenOpenCard:
                    return "Not initialized when open ";
                case (int)ErrorDef.LtcCHNoIdel:
                    return "No Idel LtcCh can be used";//没有闲置的锁存通道
                case (int)ErrorDef.NotOpen:
                    return "Card is not Open";
                default://未定义的错误类型
                    return "Unknown-Error";
            }
        }

        /// <summary>检查轴序号是否合法，内部函数使用</summary>
        void _CheckAxisEnable(int axis, string funcName)
        {
            if (!IsOpen)
                throw new Exception("MC_Htm." + funcName + "() failed: MC is not Open!");
            if (axis < 0 || axis >= AxisCount)
                throw new ArgumentOutOfRangeException(string.Format("MC_Htm.{0}() failed:axis={1} is out of range(Axis Count = {2})", funcName, axis, AxisCount));
        }

        /// <summary>检查轴序号是否合法，内部函数使用</summary>
        void _CheckAxisEnable(int[] axes, string funcName)
        {
            if (!IsOpen)
                throw new Exception("MC_Htm." + funcName + "() failed: MC is not open!");
            if (null == axes || 0 == axes.Length)
                throw new ArgumentOutOfRangeException(string.Format("MC_Htm.{0}() failed: param axes is null or empty)", funcName));
            foreach (int axis in axes)
                if (axis < 0 || axis >= AxisCount)
                    throw new ArgumentOutOfRangeException(string.Format("MC_Htm.{0}() failed:axis={1} is out of range(Axis Count = {2})", funcName, axis, AxisCount));
        }

        int TransBitDataToInt(int FlcCh)
        {
            int iReturnValue = 1;
            for(int i=0;i<FlcCh;i++)
            {
                iReturnValue = iReturnValue * 2;
            }
            return iReturnValue;
        }
    }
}
