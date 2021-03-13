using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFUI;
using APS168_W64;
using APS_Define_W32;
using JFToolKits;

namespace JFADLinkDevice
{
    class JFAps168CompareTrigger : IJFModule_CmprTrigger, IJFRealtimeUIProvider
    {
        /// <summary>
        /// 触发通道对应的点表初始化数据
        /// </summary>
        JFXmlDictionary<int, double[]> trigTables;
        /// <summary>
        /// 触发通道对应线性初始化数据
        /// </summary>
        JFXmlDictionary<int, JFCompareTrigLinerParam> trigLiners ;
        /// <summary>
        /// 触发通道的触发模式
        /// </summary>
        JFXmlDictionary<int, JFCompareTrigMode> trigModes ;
        /// <summary>
        /// 轴号与触发通道的对应关系
        /// </summary>
        JFXmlDictionary<int, int[]> chnTrig ;
        /// <summary>
        /// 触发通道的使能状态
        /// </summary>
        JFXmlDictionary<int, bool> trigEnables;
        /// <summary>
        /// 线性触发源
        /// </summary>
        JFXmlDictionary<int, int> lcmpSource;
        /// <summary>
        /// 点表触发源
        /// </summary>
        JFXmlDictionary<int, int> tcmpSource;
        JFXmlDictionary<int, int> tcmpDir;
        /// <summary>
        /// 线性触发源使用状态
        /// </summary>
        JFXmlDictionary<int, bool> lcmprUsed;
        /// <summary>
        /// 点表触发源使用状态
        /// </summary>
        JFXmlDictionary<int, bool> tcmprUsed;
        /// <summary>
        /// 轴号与线性触发源的绑定关系
        /// </summary>
        JFXmlDictionary<int, int> chnLcmpr;
        /// <summary>
        /// 轴号与点表触发源的绑定关系
        /// </summary>
        JFXmlDictionary<int, int> chnTcmpr;
        /// <summary>
        /// 触发通道与线性触发源的对应关系
        /// </summary>
        JFXmlDictionary<int, List<int>> trigLCmprSource;
        /// <summary>
        /// 触发通道与点表触发源的对应关系
        /// </summary>
        JFXmlDictionary<int, List<int>> trigTCmprSource;

        /// <summary>
        /// 触发通道与点表初始化对应关系关键字
        /// </summary>
        private string TrigTablesKeyName="TrigTables";
        /// <summary>
        /// 触发通道与线性初始化对应关系关键字
        /// </summary>
        private string TrigLinersKeyName = "TrigLiners";
        /// <summary>
        /// 触发通道对应的触发模式关键字
        /// </summary>
        private string TrigModesKeyName = "TrigModes";
        /// <summary>
        /// 轴号与触发通道对应关键字
        /// </summary>
        private string ChnTrigKeyName = "ChnTrig";
        /// <summary>
        /// 触发通道使能状态对应关键字
        /// </summary>
        private string TrigEnableKeyName = "TrigEnables";
        /// <summary>
        /// 线性触发源使用状态对应关键字
        /// </summary>
        private string LCmprUsedKeyName = "LCmprUsed";
        /// <summary>
        /// 点表触发源使用状态对应关键字
        /// </summary>
        private string TCmprUsedKeyName = "TCmprUsed";
        /// <summary>
        /// 触发通道与线性触发源对应关系关键字
        /// </summary>
        private string TrigLCmprKeyName = "TrigLCmpr";
        /// <summary>
        /// 触发通道与点表触发源对应关系关键字
        /// </summary>
        private string TrigTCmprKeyName = "TrigTCmpr";
        /// <summary>
        /// 轴号与线性触发源对应关系关键字
        /// </summary>
        private string ChnLCmprKeyName = "ChnLCmpr";
        /// <summary>
        /// 轴号与点表触发源对应关系关键字
        /// </summary>
        private string ChnTCmprKeyName = "ChnTCmpr";
        /// <summary>
        /// 脉冲当量对应关键字
        /// </summary>
        public string factorKeyName = "PulseFactor";

        public bool IsOpen { get; private set; }

        public int EncoderChannels { get; private set; }

        public int TrigChannels { get; private set; }

        /// <summary>
        /// 凌华卡ID
        /// </summary>
        public int BoardID { get; private set; }
        /// <summary>
        /// 凌华运动参数配置文件路径
        /// </summary>
        //public string ConfigPath { get; private set; }
        JFXCfg _jf168Cfg = null;
        JFXmlDictionary<string,object> _dictCT = null;
        double[] pulseFactors = null;

        //private JFDev_Aps168MotionDaq AmpMotionDaq;

        internal JFAps168CompareTrigger(int board_id, JFXCfg jf168Cfg, JFDev_Aps168MotionDaq _AmpMotionDaq)
        {
            trigTables = new JFXmlDictionary<int, double[]>();
            trigLiners = new JFXmlDictionary<int, JFCompareTrigLinerParam>();
            trigModes = new JFXmlDictionary<int, JFCompareTrigMode>();
            chnTrig = new JFXmlDictionary<int, int[]>();
            trigEnables = new JFXmlDictionary<int, bool>();
            lcmpSource = new JFXmlDictionary<int, int>();
            tcmpSource = new JFXmlDictionary<int, int>();
            tcmpDir = new JFXmlDictionary<int, int>();
            lcmprUsed = new JFXmlDictionary<int, bool>();
            tcmprUsed = new JFXmlDictionary<int, bool>();
            trigLCmprSource = new JFXmlDictionary<int, List<int>>();
            trigTCmprSource = new JFXmlDictionary<int, List<int>>();
            chnLcmpr = new JFXmlDictionary<int, int>();
            chnTcmpr = new JFXmlDictionary<int, int>();

            BoardID = board_id;
            TrigChannels = 0;
            EncoderChannels = 0;
            IsOpen = false;
            _jf168Cfg = jf168Cfg;
            //AmpMotionDaq = _AmpMotionDaq;
        }

        /// <summary>运动控制卡Compare Trigger初始化 </summary>
        internal void Open()
        {
            if (IsOpen)
                return;

            int StartAxisId = 0, TotalAxis = 0, CardName = 0;
            APS168.APS_get_first_axisId(BoardID, ref StartAxisId, ref TotalAxis);
            APS168.APS_get_card_name(BoardID, ref CardName);
            if (/*CardName != (Int32)APS_Define.DEVICE_NAME_PCI_825458 && */CardName != (Int32)APS_Define.DEVICE_NAME_AMP_20408C)
                throw new Exception(string.Format("AMP204MC.Initialize Failed :运动控制卡型号不是204C或208C！"));

            if (CardName == (Int32)APS_Define.DEVICE_NAME_AMP_20408C && TotalAxis == 4)
            {
                TrigChannels = 2;
                EncoderChannels = 4;
                for(int i=0;i<TrigChannels;i++)
                {
                    if(!lcmpSource.ContainsKey(i))
                    {
                        lcmpSource.Add(i, (Int32)APS_Define.TGR_LCMP0_SRC + i);
                        tcmpSource.Add(i, (Int32)APS_Define.TGR_TCMP0_SRC + i);
                        tcmpDir.Add(i, (Int32)APS_Define.TGR_TCMP0_DIR + i);
                    }
                }
            }
            if (CardName == (Int32)APS_Define.DEVICE_NAME_AMP_20408C && TotalAxis == 8)
            {
                TrigChannels = 4;
                EncoderChannels = 8;
                for(int i=0;i<TrigChannels;i++)
                {
                    if(i<2)
                    {
                        if (!lcmpSource.ContainsKey(i))
                        {
                            lcmpSource.Add(i, (Int32)APS_Define.TGR_LCMP0_SRC + i);
                            tcmpSource.Add(i, (Int32)APS_Define.TGR_TCMP0_SRC + i);
                            tcmpDir.Add(i, (Int32)APS_Define.TGR_TCMP0_DIR + i);
                        }
                    }
                    else
                    {
                        if (!lcmpSource.ContainsKey(i))
                        {
                            lcmpSource.Add(i, (Int32)APS_Define.TGR_LCMP2_SRC + i);
                            tcmpSource.Add(i, (Int32)APS_Define.TGR_TCMP2_SRC + i);
                            tcmpDir.Add(i, (Int32)APS_Define.TGR_TCMP2_DIR + i);
                        }
                    }
                }
            }

            #region Dictionary 初始化
            lock (_jf168Cfg)
            {
                if (!_jf168Cfg.ContainsItem("Card_" + BoardID))
                    _jf168Cfg.AddItem("Card_" + BoardID, new JFXmlDictionary<string, object>());
                _dictCT = _jf168Cfg.GetItemValue("Card_" + BoardID) as JFXmlDictionary<string, object>;

                if (_dictCT.ContainsKey( TrigTablesKeyName))
                {
                    trigTables = _dictCT[TrigTablesKeyName] as JFXmlDictionary<int, double[]>;
                }
                else
                {
                    for (int i = 0; i < EncoderChannels; i++)
                    {
                        if (trigTables.ContainsKey(i))
                            trigTables[i] = new double[0];
                        else
                            trigTables.Add(i, new double[0]);
                    }
                    _dictCT.Add(TrigTablesKeyName, trigTables);
                }


                if (_dictCT.ContainsKey( TrigLinersKeyName))
                {
                    trigLiners = _dictCT[TrigLinersKeyName] as JFXmlDictionary<int, JFCompareTrigLinerParam>;
                }
                else
                {
                    for (int i = 0; i < EncoderChannels; i++)
                    {
                        if (trigLiners.ContainsKey(i))
                            trigLiners[i] = new JFCompareTrigLinerParam();
                        else
                            trigLiners.Add(i, new JFCompareTrigLinerParam());
                    }
                    _dictCT.Add(TrigLinersKeyName, trigLiners);
                }

                if (_dictCT.ContainsKey(TrigModesKeyName))
                {
                    trigModes = _dictCT[TrigModesKeyName] as JFXmlDictionary<int, JFCompareTrigMode>;
                }
                else
                {
                    for (int i = 0; i < EncoderChannels; i++)
                    {
                        if (trigModes.ContainsKey(i))
                            trigModes[i] = JFCompareTrigMode.disable;
                        else
                            trigModes.Add(i, JFCompareTrigMode.disable);
                    }
                    _dictCT.Add(TrigModesKeyName, trigModes);
                }

                if (_dictCT.ContainsKey(ChnTrigKeyName))
                {
                    chnTrig = _dictCT[ChnTrigKeyName] as JFXmlDictionary<int, int[]>;
                }


                if (_dictCT.ContainsKey(TrigEnableKeyName))
                {
                    trigEnables = _dictCT[TrigEnableKeyName] as JFXmlDictionary<int, bool>;
                }
                else
                {
                    for (int i = 0; i < TrigChannels; i++)
                    {
                        if (trigEnables.ContainsKey(i))
                            trigEnables[i] = false;
                        else
                            trigEnables.Add(i, false);
                    }
                    _dictCT.Add(TrigEnableKeyName, trigEnables);
                }

                if (_dictCT.ContainsKey( LCmprUsedKeyName))
                {
                    lcmprUsed = _dictCT[LCmprUsedKeyName] as JFXmlDictionary<int, bool>;
                }
                else
                {
                    for (int i = 0; i < TrigChannels; i++)
                    {
                        if (lcmprUsed.ContainsKey(i))
                            lcmprUsed[i] = false;
                        else
                            lcmprUsed.Add(i, false);
                    }
                    _dictCT.Add(LCmprUsedKeyName, lcmprUsed);
                }

                if (_dictCT.ContainsKey(TCmprUsedKeyName))
                {
                    tcmprUsed = _dictCT[TCmprUsedKeyName] as JFXmlDictionary<int, bool>;
                }
                else
                {
                    for (int i = 0; i < TrigChannels; i++)
                    {
                        if (tcmprUsed.ContainsKey(i))
                            tcmprUsed[i] = false;
                        else
                            tcmprUsed.Add(i, false);
                    }
                    _dictCT.Add(TCmprUsedKeyName, tcmprUsed);
                }

                if (_dictCT.ContainsKey(TrigLCmprKeyName))
                {
                    trigLCmprSource = _dictCT[TrigLCmprKeyName] as JFXmlDictionary<int, List<int>>;
                }

                if (_dictCT.ContainsKey(TrigTCmprKeyName))
                {
                    trigTCmprSource = _dictCT[TrigTCmprKeyName] as JFXmlDictionary<int, List<int>>;
                }


                if (_dictCT.ContainsKey(ChnTCmprKeyName))
                {
                    chnTcmpr = _dictCT[ChnTCmprKeyName] as JFXmlDictionary<int, int>;
                }

                if (_dictCT.ContainsKey(ChnLCmprKeyName))
                {
                    chnLcmpr = _dictCT[ChnLCmprKeyName] as JFXmlDictionary<int, int>;
                }

                if(!_dictCT.ContainsKey("PulseFactor"))
                {
                    pulseFactors = new double[TotalAxis];
                    for (int i = 0; i < TotalAxis;i++)
                        pulseFactors[i] = 1;
                }
                else
                {
                    pulseFactors = _dictCT["PulseFactor"] as double[];
                    if(pulseFactors.Length < TotalAxis)
                    {
                        _dictCT.Remove("PulseFactor");
                        pulseFactors = new double[TotalAxis];
                        for (int i = 0; i < TotalAxis; i++)
                            pulseFactors[i] = 1;
                        _dictCT.Add("PulseFactor", pulseFactors);
                    }
                }



                _jf168Cfg.Save();
            }

            #endregion

            for (int i = 0; i < TrigChannels; i++)
            {
                if(APS168.APS_reset_trigger_count(BoardID, i)!=0)//reset count
                    throw new Exception(string.Format("AMP204MC.APS_reset_trigger_count Failed :重置触发通道{0}计数器失败！", i));


                if (APS168.APS_set_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG0_SRC + i, 0)!=0)//Trig source bind
                    throw new Exception(string.Format("AMP204MC.APS_set_trigger_param Failed :接触触发通道{0}绑定关系！", i));

                //if (SetTrigEnable(i, trigEnables[i]) != (int)ErrorDef.Success)//enable or disable trig    //调用慢，先屏蔽   remarked by Boby
                //    throw new Exception(string.Format("AMP204MC.SetTrigEnable Failed :设置触发通道{0}使能状态失败！", i));
            }

            for (int i = 0; i < EncoderChannels; i++)
            {
                if(!chnTrig.ContainsKey(i))
                    continue;
                if (chnTrig[i].Length <= 0)
                    continue;
                int[] TrigChns = new int[chnTrig[i].Length];
                TrigChns = chnTrig[i];
                //if(SetEncoderTrigBind(i, TrigChns)!=(int)ErrorDef.Success)    //调用慢，先屏蔽 remarked by Boby
                //    throw new Exception(string.Format("AMP204MC.SetEncoderTrigBind Failed :绑定编码器{0}和触发通道{1}失败！",i, chnTrig[i]));
            }
            IsOpen = true;
        }

        internal void Close()
        {
            if (!IsOpen)
                return;
            TrigChannels = 0;
            EncoderChannels = 0;

            for (int i = 0; i < TrigChannels; i++)
                SetTrigEnable(i, false);
            
            IsOpen = false;
        }

        /// <summary>
        /// 保存轴卡参数到文件
        /// </summary>
        /// <returns></returns>
        internal int SaveParamsToFlash()
        {
            if (APS168.APS_save_parameter_to_flash(BoardID) != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        public int SetEncoderFactor(int encChn, double factor)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("SetEncoderFactor(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));
            lock (_jf168Cfg)
            {
                pulseFactors[encChn] = factor;
                _jf168Cfg.Save();
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取编码器脉冲当量
        /// </summary>
        /// <param name="encChn"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public int GetEncoderFactor(int encChn, out double factor)
        {
            factor = 0;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("GetEncoderFactor(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            factor = pulseFactors[encChn];
            return (int)ErrorDef.Success;
        }

        public int SetEncoderTrigBind(int encChn, int[] trigChns)
        {
            int Param_Value = 0;
            int err = 0;

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("SetEncoderTrigBind(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));
            if (trigChns == null)
                return (int)ErrorDef.InvokeFailed;

            for (int i = 0; i < chnTrig[encChn].Length; i++)
            {
                if (trigChns[i] < 0 || trigChns[i] >= TrigChannels)
                    throw new ArgumentOutOfRangeException("SetEncoderTrigBind(encChn ,...) fialed By:trigChns = " + trigChns[i] + " is outof range:0~" + (TrigChannels - 1));
            }

            bool exist = false;
            bool equal = false;

            if (chnTrig.ContainsKey(encChn))
            {
                exist = true;
                if (chnTrig[encChn] == trigChns)
                    equal = true;
            }

            if(exist && !equal)
            { 
                if (chnTrig.ContainsKey(encChn))
                    chnTrig[encChn] = trigChns;
                else
                    chnTrig.Add(encChn, trigChns);

                

                for (int i = 0; i < TrigChannels; i++)
                {
                    if (APS168.APS_reset_trigger_count(BoardID, i) != 0)//reset count
                        return (int)ErrorDef.InvokeFailed;

                    if (SetTrigEnable(i, false) != (int)ErrorDef.Success)//disable trig
                        return (int)ErrorDef.InvokeFailed;

                    if (APS168.APS_set_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG0_SRC + i, 0) != 0)//Trig source bind
                        return (int)ErrorDef.InvokeFailed;

                    if (SetTrigEnable(i, trigEnables[i]) != (int)ErrorDef.Success)//enable or disable trig
                        return (int)ErrorDef.InvokeFailed;
                }

                if (SaveParamsToFlash() != 0)
                    return (int)ErrorDef.InvokeFailed;

                for (int i = 0; i < EncoderChannels; i++)
                {
                    if (!chnTrig.ContainsKey(i))
                        continue;
                    if (chnTrig[i].Length <= 0)
                        continue;
                    int[] TrigChns = new int[chnTrig[i].Length];
                    TrigChns = chnTrig[i];
                    if (SetEncoderTrigBind(i, TrigChns) != (int)ErrorDef.Success) //循环调用？？？？？？？？？？？？？？？？？？？？ BobY
                        return (int)ErrorDef.InvokeFailed;
                }
                _jf168Cfg.Save();
                return (int)ErrorDef.Success;
            }
            
            //encoder和trig源进行绑定
            Param_Value = 0;
            for (int i = 0; i < trigChns.Length; i++)
            {
                JFCompareTrigMode jFCompareTrigMode;
                if (GetTrigMode(encChn, out jFCompareTrigMode) != (int)ErrorDef.Success)
                    return (int)ErrorDef.InvokeFailed;

                int value = 0;
                int index = 0;

                #region JFCompareTrigMode.liner
                if (jFCompareTrigMode == JFCompareTrigMode.liner)
                {
                    if (GetLCmprIdleSource(encChn,out value,out index) != (int)ErrorDef.Success)//get Ideal LCMP source
                    {
                        return (int)ErrorDef.TrgLcmpSrcError;
                    }
                    err = APS168.APS_set_trigger_param(BoardID, value, encChn);//Param:TGR_LCMP0_SRC
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;

                    //Trigger output 0 (TRG0) source Bit( 1:On, 0:Off)  Bit 0: Manual  Bit 1:Reserved Bit  2:TCMP0  Bit 3:TCMP1  Bit 4:LCMP0  Bit 5:LCMP1 Bit 6:MCMP
                    //Bit 7:TCMP2 Bit 8:TCMP3 Bit 9:LCMP2 Bit 10:LCMP3  
                    Param_Value = 0;
                    //获取TRGSRC与TCMP已建立的绑定关系
                    List<int> triglcmpsrc = new List<int>();
                    if (trigLCmprSource.ContainsKey(trigChns[i]))
                    {
                        triglcmpsrc = trigLCmprSource[trigChns[i]] as List<int>;
                        for(int m=0;m< triglcmpsrc.Count;m++)
                        {
                            if (triglcmpsrc[m] < 2)
                                Param_Value = Param_Value+(1 << (4 + triglcmpsrc[m]));
                            else
                                Param_Value = Param_Value+(1 << (9 + triglcmpsrc[m] - 2));
                        }
                    }
                    if (!triglcmpsrc.Contains(index))
                    {
                        if (index < 2)
                            Param_Value = Param_Value + (1 << (4 + trigChns[i]));
                        else
                            Param_Value = Param_Value + (1 << (9 + trigChns[i] - 2));
                    }

                    err = APS168.APS_set_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG0_SRC + trigChns[i], Param_Value);
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;

                    err = APS168.APS_set_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG0_PWD + trigChns[i], 250000); //  pulse width=  value * 0.02 us
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;
                    //TRG 1 logic  0: Not inverse  1:Inverse
                    err = APS168.APS_set_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG0_LOGIC + trigChns[i], 0);
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;

                    //TrigSRC与LCMPSRC进行绑定信息保存
                    triglcmpsrc = new List<int>();
                    if (trigLCmprSource.ContainsKey(trigChns[i]))
                    {
                        triglcmpsrc = trigTCmprSource[trigChns[i]] as List<int>;
                        if(!triglcmpsrc.Contains(index))
                            triglcmpsrc.Add(index);
                        trigLCmprSource[trigChns[i]] = triglcmpsrc;
                    }
                    else
                    {
                        triglcmpsrc.Add(index);
                        trigLCmprSource.Add(trigChns[i], triglcmpsrc);
                    }

                    if (lcmprUsed.ContainsKey(index))
                        lcmprUsed[index] = true;
                    else
                        lcmprUsed.Add(index, true);
                }
                #endregion

                #region JFCompareTrigMode.table
                else if (jFCompareTrigMode == JFCompareTrigMode.table)
                {
                    if (GetTCmprIdleSource(encChn,out value, out index) != (int)ErrorDef.Success)//get Ideal TCMP source
                    {
                        return (int)ErrorDef.TrgLcmpSrcError;
                    }
                    err = APS168.APS_set_trigger_param(BoardID, value, encChn);//Param:TGR_TCMP0_SRC
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;

                    err = APS168.APS_set_trigger_param(BoardID, tcmpDir[index], 2);//Param:TGR_TCMP0_DIR
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;

                    //Trigger output 0 (TRG0) source Bit( 1:On, 0:Off)    Bit 0: Manual  Bit 1:Reserved Bit  2:TCMP0  Bit 3:TCMP1  Bit 4:LCMP0  Bit 5:LCMP1 Bit 6:MCMP
                    //Bit 7:TCMP2 Bit 8:TCMP3 Bit 9:LCMP2 Bit 10:LCMP3   
                    Param_Value = 0;
                    //获取TRGSRC与TCMP已建立的绑定关系
                    List<int> trigtcmpsrc = new List<int>();
                    if (trigTCmprSource.ContainsKey(trigChns[i]))
                    {
                        trigtcmpsrc = trigTCmprSource[trigChns[i]] as List<int>;
                        for (int m = 0; m < trigtcmpsrc.Count; m++)
                        {
                            if (trigtcmpsrc[m] < 2)
                                Param_Value = Param_Value + (1 << (2 + trigtcmpsrc[m]));
                            else
                                Param_Value = Param_Value + (1 << (7 + trigtcmpsrc[m] - 2));
                        }
                    }
                    if (!trigtcmpsrc.Contains(index))
                    {
                        if (index < 2)
                            Param_Value = (1 << (2 + trigChns[i]));
                        else
                            Param_Value = (1 << (7 + trigChns[i] - 2));
                    }
                    err = APS168.APS_set_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG0_SRC + trigChns[i], Param_Value);
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;

                    err = APS168.APS_set_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG0_PWD + trigChns[i], 250000); //  pulse width=  value * 0.02 us
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;
                    //TRG 1 logic  0: Not inverse  1:Inverse
                    err = APS168.APS_set_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG0_LOGIC + trigChns[i], 0);
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;

                    //TrigSRC与TCMPSRC进行绑定信息保存
                    trigtcmpsrc = new List<int>();
                    if (trigTCmprSource.ContainsKey(trigChns[i]))
                    {
                        trigtcmpsrc = trigTCmprSource[trigChns[i]] as List<int>;
                        if(!trigtcmpsrc.Contains(index))
                            trigtcmpsrc.Add(index);
                        trigTCmprSource[trigChns[i]] = trigtcmpsrc;
                    }
                    else
                    {
                        trigtcmpsrc.Add(index);
                        trigTCmprSource.Add(trigChns[i], trigtcmpsrc);
                    }

                    if (tcmprUsed.ContainsKey(index))
                        tcmprUsed[index] = true;
                    else
                        tcmprUsed.Add(index, true);
                }
            }
            #endregion

            if (chnTrig.ContainsKey(encChn))
                chnTrig[encChn] = trigChns;
            else
                chnTrig.Add(encChn, trigChns);

            if (trigModes[encChn] == JFCompareTrigMode.liner)
            {
                if (SetTrigLiner(encChn, trigLiners[encChn]) != (int)ErrorDef.Success)
                    return (int)ErrorDef.InvokeFailed;
            }
            else if (trigModes[encChn] == JFCompareTrigMode.table)
            {
                if (SetTrigTable(encChn, trigTables[encChn]) != (int)ErrorDef.Success)
                    return (int)ErrorDef.InvokeFailed;
            }



            _jf168Cfg.Save();
            if (SaveParamsToFlash() != 0)
                return (int)ErrorDef.InvokeFailed;

            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取编码器和触发通道的绑定信息
        /// </summary>
        /// <param name="encChn"></param>
        /// <param name="trigChns"></param>
        /// <returns></returns>
        public int GetEncoderTrigBind(int encChn, out int[] trigChns)
        {
            trigChns = new int[1]{0};
            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("GetEncoderTrigBind(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            if (chnTrig.ContainsKey(encChn))
                trigChns = chnTrig[encChn];
  
            return (int)ErrorDef.Success;
        }

        public int SetTrigMode(int encChn, JFCompareTrigMode mode)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("SetTrigMode(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            if (mode == trigModes[encChn])
                return (int)ErrorDef.Success;

            if (trigModes.ContainsKey(encChn))
                trigModes[encChn] = mode;
            else
                trigModes.Add(encChn, mode);


            _jf168Cfg.Save();
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取编码器的脉冲触发方式
        /// </summary>
        /// <param name="encChn"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int GetTrigMode(int encChn, out JFCompareTrigMode mode)
        {
            mode = JFCompareTrigMode.disable;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("GetTrigMode(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));
            if (trigModes.ContainsKey(encChn))
                mode = trigModes[encChn];

            return (int)ErrorDef.Success;
        }

        public int SetTrigLiner(int encChn, JFCompareTrigLinerParam linerParam)
        {
            if (encChn < 0 || encChn >= TrigChannels)
                throw new ArgumentOutOfRangeException("SetTrigMode(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            //if (trigLiners[encChn] == linerParam)
            //    return (int)ErrorDef.Success;

            if (linerParam.repeats < 0)
                return (int)ErrorDef.ParamError;

            int[] trigchans;
            if (GetEncoderTrigBind(encChn, out trigchans) != 0)
                return (int)ErrorDef.InvokeFailed;

            for (int i = 0; i < trigchans.Length; i++)
            {
                int err = APS168.APS_set_trigger_linear(BoardID, trigchans[i], (int)(linerParam.start * pulseFactors[encChn]),
                    (int)linerParam.repeats, (int)(linerParam.interval * pulseFactors[encChn]));
                if (err != 0)
                    return (int)ErrorDef.InvokeFailed;
            }

            if (trigLiners.ContainsKey(encChn))
                trigLiners[encChn] = linerParam;
            else
                trigLiners.Add(encChn, linerParam);

            _jf168Cfg.Save();
            if (SaveParamsToFlash() != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取线性触发参数
        /// </summary>
        /// <param name="encChn"></param>
        /// <param name="linerParam"></param>
        /// <returns></returns>
        public int GetTrigLiner(int encChn, out JFCompareTrigLinerParam linerParam)
        {
            linerParam = new JFCompareTrigLinerParam() { start = 0, interval = 0, repeats = 0 };

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("GetTrigLiner(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            if (trigLiners.ContainsKey(encChn))
                linerParam = trigLiners[encChn];
            return (int)ErrorDef.Success;
        }

        public int SetTrigTable(int encChn, double[] posTable)
        {
            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("SetTrigTable(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            if (null == posTable)
                throw new ArgumentNullException("SetTrigTable(int encChn, double[] posTable) failed By: posTable == null");

            int[] iposTable = new int[] { };
            for (int i = 0; i < posTable.Length; i++)
            {
                iposTable[i] = Convert.ToInt32(posTable[i]* pulseFactors[encChn]);
            }

            int[] trigchans;
            if (GetEncoderTrigBind(encChn, out trigchans) != 0)
                return (int)ErrorDef.InvokeFailed;

 
            for (int i = 0; i < trigchans.Length; i++)
            {
                int err = APS168.APS_set_trigger_table(BoardID, trigchans[i], iposTable, iposTable.Length);
                if (err != 0)
                    return (int)ErrorDef.InvokeFailed;
            }

            if (trigTables.ContainsKey(encChn))
                trigTables[encChn] = posTable;
            else
                trigTables.Add(encChn, posTable);

            _jf168Cfg.Save();
            if (SaveParamsToFlash() != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取点表触发参数
        /// </summary>
        /// <param name="encChn"></param>
        /// <param name="posTable"></param>
        /// <returns></returns>
        public int GetTrigTable(int encChn, out double[] posTable)
        {
            posTable = null;

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("GetTrigTable(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            if (trigTables.ContainsKey(encChn))
                posTable = trigTables[encChn];
            return (int)ErrorDef.Success;
        }

        public int SetTrigEnable(int trigChn, bool isEnable)
        {
            if (trigChn < 0 || trigChn >= TrigChannels)
                throw new ArgumentOutOfRangeException("SetTrigEnable(trigChn ,...) fialed By:trigChn = " + trigChn + " is outof range:0~" + (TrigChannels - 1));

            int Param_Val = 0;
            int ret = APS168.APS_get_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG_EN, ref Param_Val);
            if(ret!=0)
                return (int)ErrorDef.InvokeFailed;

            int currentstatus = (Param_Val >> trigChn) & 1;
            if(currentstatus==1)
            {
                if (!isEnable)
                    Param_Val = Param_Val - (1 << trigChn);
            }
            else
            {
                if (isEnable)
                    Param_Val = Param_Val + (1 << trigChn);
            }

            ret = APS168.APS_set_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG_EN, Param_Val);
            if (ret != 0)
                return (int)ErrorDef.InvokeFailed;

            if (trigEnables.ContainsKey(trigChn))
                trigEnables[trigChn] = isEnable;
            else
                trigEnables.Add(trigChn, isEnable);

            _jf168Cfg.Save();

            if (SaveParamsToFlash() != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        public int GetTrigEnable(int trigChn, out bool isEnabled)
        {
            isEnabled = false;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (trigChn < 0 || trigChn >= TrigChannels)
                throw new ArgumentOutOfRangeException("GetTrigEnable(trigChn ,...) fialed By:trigChn = " + trigChn + " is outof range:0~" + (EncoderChannels - 1));

            int Param_Val = 0;
            int ret = APS168.APS_get_trigger_param(BoardID, (Int32)APS_Define.TGR_TRG_EN, ref Param_Val);
            if (ret != 0)
                return (int)ErrorDef.InvokeFailed;

            if (((Param_Val>>trigChn)&1)==1)
                isEnabled = true;
            else
                isEnabled = false;

            return (int)ErrorDef.Success;
        }

        public int GetTriggedCount(int trigChn, out int count)
        {
            count = 0;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (trigChn < 0 || trigChn >= TrigChannels)
                throw new ArgumentOutOfRangeException("GetTriggedCount(trigChn ,...) fialed By:trigChn = " + trigChn + " is outof range:0~" + (TrigChannels - 1));

            int ret = APS168.APS_get_trigger_count(BoardID, trigChn,ref count);
            if (ret != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        public int ResetTriggedCount(int trigChn)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (trigChn < 0 || trigChn >= TrigChannels)
                throw new ArgumentOutOfRangeException("ResetTriggedCount(trigChn ,...) fialed By:trigChn = " + trigChn + " is outof range:0~" + (TrigChannels - 1));

            int ret = APS168.APS_reset_trigger_count(BoardID, trigChn);
            if(ret!=0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        public int SetEncoderCurrPos(int encChn, double pos)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("SetEncoderCurrPos(trigChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            if (APS168.APS_set_position_f(encChn, pos* pulseFactors[encChn]) != 0)
                return (int)ErrorDef.InvokeFailed;

            return (int)ErrorDef.Success;
        }

        public int GetEncoderCurrPos(int encChn, out double pos)
        {
            pos = 0;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("GetEncoderCurrPos(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            if((pulseFactors[encChn])<=0)
                throw new ArgumentOutOfRangeException("GetEncoderCurrPos(encChn ,...) fialed By:factor<=0,factor="+ pulseFactors[encChn]);

            int ret = APS168.APS_get_position_f(encChn,ref pos);
            if (ret != 0)
                return (int)ErrorDef.InvokeFailed;

            pos = pos / ((double)pulseFactors[encChn]);
            return (int)ErrorDef.Success;
        }

        public int SoftTrigge(int[] trigChns)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            if (null != trigChns)
            {
                if (0 == trigChns.Length)
                    return (int)ErrorDef.Success;
                for (int i = 0; i < trigChns.Length; i++)
                {
                    if (trigChns[i] < 0 || trigChns[i] >= TrigChannels)
                        throw new ArgumentOutOfRangeException(string.Format("SoftTrigge(trigChns[]) fialed By:trigChns[{0}] = {1} is outof range:0~{2}", i, trigChns[i], TrigChannels - 1));
                    int err = SetTrigEnable(trigChns[i], true);
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;
                            
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;
                    err = APS168.APS_set_trigger_manual(BoardID, trigChns[i]);
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;
                }
            }
            else
                return (int)ErrorDef.Success;

            return (int)ErrorDef.Success;
        }

        public int SyncEncoderCurrPos(int encChn)
        {
            return (int)ErrorDef.Unsupported;
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
            InitFailedWhenOpen = -4,//
            NotOpen = -5, //设备未打开
            ModeError = -6,//工作模式错误 ，比如当前为触发模式
            ParamCountOutofRange = -7, //参数个数超过限制
            TrgLcmpSrcError = -8, //线性触发源已用完
            TrgTcmpSrcError = -9, //点表触发源已用完
        }

        /// <summary>
        /// 获取错误信息
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
                case (int)ErrorDef.InitFailedWhenOpen:
                    return "Not initialized when open ";
                case (int)ErrorDef.NotOpen:
                    return "Device dose not open";
                case (int)ErrorDef.ModeError:
                    return "WorkMode Mismatching"; //工作模式不匹配
                case (int)ErrorDef.ParamCountOutofRange:
                    return "Param's count is out of range";
                case (int)ErrorDef.TrgLcmpSrcError:
                    return "No lcmp source can be used";//没有可用的LCMP能进行绑定
                case (int)ErrorDef.TrgTcmpSrcError:
                    return "No tcmp source can be used";//没有可用的TCMP能进行绑定
                default://未定义的错误类型
                    return "Unknown-ErrorCode:" + errorCode;
            }
        }

        /// <summary>
        /// 查询线性触发源
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetLCmprIdleSource(int encChn,out int value,out int index)
        {
            value = 0;
            index = 0;
            if (chnLcmpr.ContainsKey(encChn))
            {
                value = chnLcmpr[encChn];
                foreach (int outindex in lcmpSource.Keys)
                {
                    if (lcmpSource[outindex] == value)
                    {
                        index = outindex;
                        return (int)ErrorDef.Success;
                    }
                }
                return (int)ErrorDef.TrgTcmpSrcError;
            }
            else
            {
                for (int i = 0; i < TrigChannels; i++)
                {
                    if (!lcmprUsed[i])
                    {
                        index = i;
                        value = lcmpSource[i];
                        chnLcmpr.Add(encChn, value);
                        return (int)ErrorDef.Success;
                    }
                }
            }
            return (int)ErrorDef.TrgLcmpSrcError;
        }

        /// <summary>
        /// 查询点表触发源
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetTCmprIdleSource(int encChn,out int value, out int index)
        {
            value = 0;
            index = 0;
            if (chnTcmpr.ContainsKey(encChn))
            {
                value = chnTcmpr[encChn];
                foreach(int outindex in tcmpSource.Keys)
                {
                    if(tcmpSource[outindex]==value)
                    {
                        index = outindex;
                        return (int)ErrorDef.Success;
                    }
                }
                return (int)ErrorDef.TrgTcmpSrcError;
            }
            else
            {
                for (int i = 0; i < TrigChannels; i++)
                {
                    if (!tcmprUsed[i])
                    {
                        index = i;
                        value = tcmpSource[i];
                        chnTcmpr.Add(encChn, value);
                        return (int)ErrorDef.Success;
                    }
                }
            }
            return (int)ErrorDef.TrgTcmpSrcError;
        }

        public JFRealtimeUI GetRealtimeUI()
        {
            UcCmprTrig ui = new UcCmprTrig();
            ui.SetCmprTigger(this, null, null);
            return ui;
        }
    }
    
}

