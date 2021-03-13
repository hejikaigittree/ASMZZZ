using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HT_Lib;
using JFInterfaceDef;
using JFUI;

namespace JFHTMDevice
{
    public class HtmCompareTrigger:IJFModule_CmprTrigger,IJFRealtimeUIProvider
    {
        internal enum TriggerType
        {
            AxisSlave = 0, //伺服轴触发 每个一轴对应一路输出
            PosTrig, //位置触发板卡
        }
        internal HtmCompareTrigger(TriggerType trigType,int[] devs)
        {
            this.trigType = trigType;
            devIndexes = new List<int>();
            devIndexes.AddRange(devs);
            trigTables = new List<double[]>();
            trigLiners = new List<JFCompareTrigLinerParam>();
            trigModes = new List<JFCompareTrigMode>();
            //trigEnables = new List<bool>();
            IsOpen = false;
        }
        public int EncoderChannels 
        { 
            get
            {
                if (!IsOpen)
                    return 0;
                return devIndexes.Count();
            }
        }

        TriggerType trigType;

        List<int> devIndexes;
        List<double[]> trigTables;
        List<JFCompareTrigLinerParam> trigLiners;
        List<JFCompareTrigMode> trigModes;
        //List<bool> trigEnables;

        public int TrigChannels
        {
            get
            {
                //if (!IsOpen)
                //    return 0;
                return devIndexes.Count();
            }
        }

        internal void Open()
        {
            if (IsOpen)
                return;
            //devIndexes.Clear();
            trigTables.Clear();
            trigLiners.Clear();
            trigModes.Clear();
            //trigEnables.Clear();
            int err, i , j;
            //int devCount = HTM.GetDeviceNum();
            //for (i = 0; i < devCount; i++)
            //{
            //    HTM.DEVICE_INFO devInfo;
            //    if (0 == HTM.GetDeviceInfo(i, out devInfo))
            //        if (devInfo.devType == (byte)HTM.DeviceType.POSTRIG || devInfo.devType == (int)HTM.DeviceType.HTDHVD)
            //        {
            //            devIndexes.Add(i);
            //        }
            //}
            //if (devIndexes.Count == 0)
            //    return;

            
            for (i = 0; i < TrigChannels; i++)
            {
                trigTables.Add(new double[2] { 0, 0 });
                if (trigType == TriggerType.PosTrig) //目前只有位置触发板卡支持点表模式
                {
                    for (j = 0; j < 2; j++)
                    {
                        err = HTM.SetPtTrigPos(devIndexes[i], j, trigTables[i][j]);
                        if (err != 0)
                            throw new Exception(string.Format("HtmCompareTrigger.Open() failed By HTM.SetPtTrigPos(devIndexes = {0},ptIndex = {1},0) return Errorcode = {2}", devIndexes[i], j, err));

                    }
                    for (j = 0; j < 2; j++)
                    {
                        err = HTM.SetPtTrigEnable(devIndexes[i], j, 0);
                        if (0 != err)
                            throw new Exception(string.Format("HtmCompareTrigger.Open() failed By HTM.SetPtTrigEnable(devIndexes = {0},j = {1},0) return Errorcode = {2}", devIndexes[i], j, err));
                    }
                }

                HTM.TRIG_LINEAR tl = new HTM.TRIG_LINEAR() { startPos = 0, endPos = 0, interval = 1 };
                err = HTM.SetLinTrigPos(devIndexes[i], ref tl);
                //if (0 != err)
                    // throw new Exception(string.Format("HtmCompareTrigger.Open() failed By HTM.SetLinTrigPos(devIndexes = {0},TRIG_LINEAR =\"0,0,0\") return Errorcode = {1}", devIndexes[i], err));


                err = HTM.SetLinTrigEnable(devIndexes[i], 0);
                //if (0 != err)
                    // throw new Exception(string.Format("HtmCompareTrigger.Open() failed By HTM.SetLinTrigEnable(devIndexes = {0},0) return Errorcode = {1}", devIndexes[i], err));
              
                
                trigModes.Add(JFCompareTrigMode.disable);
                
                trigLiners.Add(new JFCompareTrigLinerParam() { start = 0, interval = 0, repeats = 0 });
                //trigEnables.Add(false);

            }
            IsOpen = true;

        }

        internal void Close()
        {
            if (!IsOpen)
                return;
            for(int i = 0; i < TrigChannels;i++)
                SetTrigEnable(i, false);
            //devIndexes.Clear();
            //trigModes.Clear();
            //trigTables.Clear();
            //trigLiners.Clear();
            //trigEnables.Clear();
            IsOpen = false;
        }

        public bool IsOpen { get; private set; }


        //不支持脉冲当量功能
         public int SetEncoderFactor(int encChn, double factor)
        {
            return (int)ErrorDef.Unsupported;
        }

        public int GetEncoderFactor(int encChn, out double factor)
        {
            factor = 0;
            return (int)ErrorDef.Unsupported;
        }

        public int SetEncoderTrigBind(int encChn, int[] trigChns)
        {
            return (int)ErrorDef.Unsupported;
        }

        public int GetEncoderTrigBind(int encChn, out int[] trigChns)
        {
            trigChns = null;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("GetEncoderTrigBind(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));
            trigChns = new int[] { encChn };
            return (int)ErrorDef.Success;
        }

        public int SetTrigMode(int encChn, JFCompareTrigMode mode)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("SetTrigMode(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));
            int err = 0;
            if (mode == trigModes[encChn])
                return (int)ErrorDef.Success;
          
            int ret = (int)ErrorDef.ParamError;
            switch(mode)
            {
                case JFCompareTrigMode.disable:
                    err = HTM.SetLinTrigEnable(devIndexes[devIndexes[encChn]], 0);
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;
                    if (trigType == TriggerType.PosTrig)
                    {
                        for (int i = 0; i < trigTables[encChn].Length; i++) //HTM点表触发只支持2个点
                        {
                            err = HTM.SetPtTrigEnable(devIndexes[encChn], i, 0);
                            if (err != 0)
                                return (int)ErrorDef.InvokeFailed;
                        }
                    }
                    ret =  (int)ErrorDef.Success;
                    trigModes[encChn] = mode;
                    break;
                case JFCompareTrigMode.liner:
                    err = HTM.SetLinTrigEnable(devIndexes[encChn], 1);
                    if (err != 0)
                    {
                        ret = (int)ErrorDef.InvokeFailed;
                        break;
                    }
                    if (trigType == TriggerType.PosTrig)
                    {
                        for (int i = 0; i < trigTables[encChn].Length; i++) //HTM点表触发只支持2个点
                        {
                            err = HTM.SetPtTrigEnable(devIndexes[encChn], i, 0);
                            if (err != 0)
                                return (int)ErrorDef.InvokeFailed;
                        }
                    }
                    ret = (int)ErrorDef.Success;
                    trigModes[encChn] = mode;
                    break;
                case JFCompareTrigMode.table:
                    err = HTM.SetLinTrigEnable(devIndexes[encChn], 0); //先禁用线性触发
                    if (err != 0)
                        return (int)ErrorDef.InvokeFailed;
                    //if (trigType == TriggerType.PosTrig)
                    {
                        for (int i = 0; i < trigTables[encChn].Length; i++) //HTM点表触发只支持2个点
                        {
                            err = HTM.SetPtTrigEnable(devIndexes[encChn], i, 1);
                            if (err != 0)
                                return (int)ErrorDef.InvokeFailed;
                        }
                    }
                    ret = (int)ErrorDef.Success;
                    trigModes[encChn] = mode;
                    break;
                default:
                    break;
            }
                
            return ret;

        }

        public int GetTrigMode(int encChn, out JFCompareTrigMode mode)
        {
            mode = JFCompareTrigMode.disable;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("GetTrigMode(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));
            mode = trigModes[encChn];
            return (int)ErrorDef.Success;
        }

        public int SetTrigLiner(int encChn, JFCompareTrigLinerParam linerParam)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("SetTrigMode(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            if (trigLiners[encChn] == linerParam)
                return (int)ErrorDef.Success;

            if (linerParam.repeats < 0)
                return (int)ErrorDef.ParamError;

            HTM.TRIG_LINEAR tl = new HTM.TRIG_LINEAR();
            tl.startPos = linerParam.start;
            tl.interval = linerParam.interval;
            if (linerParam.repeats == 0)
                tl.endPos = tl.startPos;
            else
            {
                if (linerParam.interval >= 0)
                    tl.endPos = tl.startPos + (linerParam.repeats + 0.5) * linerParam.interval;
                else
                    tl.endPos = tl.startPos + (linerParam.repeats - 0.5) * linerParam.interval;
            }
            int err = HTM.SetLinTrigPos(devIndexes[encChn], ref tl);
            if (0 != err)
                return (int)ErrorDef.InvokeFailed;
            trigLiners[encChn] = linerParam;
            return (int)ErrorDef.Success;
        }

        public int GetTrigLiner(int encChn, out JFCompareTrigLinerParam linerParam)
        {
            linerParam =  new JFCompareTrigLinerParam() { start = 0, interval = 0, repeats = 0 };
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("GetTrigLiner(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));
            linerParam = trigLiners[encChn];
            return (int)ErrorDef.Success;
        }

        public int SetTrigTable(int encChn, double[] posTable)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("SetTrigTable(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            //if (trigType == TriggerType.AxisSlave)
            //    return (int)ErrorDef.Unsupported;

            if (null == posTable)
                posTable = new double[] { };//throw new ArgumentNullException("SetTrigTable(int encChn, double[] posTable) failed By: posTable == null");

            //if (posTable.Length > 4)
            //    return (int)ErrorDef.ParamCountOutofRange;

            int err;
            for(int i = 0; i < posTable.Length; i++)
            {
                err = HTM.SetPtTrigEnable(devIndexes[encChn], i, 1);
                if (0 != err)
                    return (int)ErrorDef.InvokeFailed;
                err = HTM.SetPtTrigPos(devIndexes[encChn], i, posTable[i]);
                if (0 != err)
                    return (int)ErrorDef.InvokeFailed;
                //if(trigEnables[encChn] && trigModes[encChn] == JFCompareTrigMode.table)
                //{
                //    err = HTM.SetPtTrigEnable(devIndexes[encChn], i, 1);
                //    if(0!= err)
                //        return (int)ErrorDef.InvokeFailed;
                //}
            }
            trigTables[encChn] = posTable;
            return (int)ErrorDef.Success;
        }

        public int GetTrigTable(int encChn, out double[] posTable)
        {
            posTable = null;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("GetTrigTable(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));
            posTable = trigTables[encChn];
            return (int)ErrorDef.Success;


        }

        public int GetTrigEnable(int trigChn, out bool isEnabled)
        {
            isEnabled = false;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (trigChn < 0 || trigChn >= TrigChannels)
                throw new ArgumentOutOfRangeException("GetTrigEnable(trigChn ,...) fialed By:trigChn = " + trigChn + " is outof range:0~" + (EncoderChannels - 1));
            isEnabled = trigModes[trigChn] != JFCompareTrigMode.disable;//trigEnables[trigChn];
            return (int)ErrorDef.Success;

        }

        public int SetTrigEnable(int trigChn, bool isEnable)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (trigChn < 0 || trigChn >= TrigChannels)
                throw new ArgumentOutOfRangeException("SetTrigEnable(trigChn ,...) fialed By:trigChn = " + trigChn + " is outof range:0~" + (TrigChannels - 1));

            return (int)ErrorDef.Unsupported;
            //if (isEnable == trigEnables[trigChn])
            //    return (int)ErrorDef.Success;

            //int enbaleLine = isEnable ? (trigModes[trigChn] == JFCompareTrigMode.liner ? 1 : 0) : 0;
            //int enbaleTable = isEnable ? (trigModes[trigChn] == JFCompareTrigMode.table ? 1 : 0) : 0;

            //int err = HTM.SetLinTrigEnable(devIndexes[trigChn], enbaleLine);
            //if (0 != err)
            //    return (int)ErrorDef.InvokeFailed;
            //for(int i = 0; i < 4;i++)
            //{
            //    if(i < trigTables[trigChn].Length)
            //    {
            //        err = HTM.SetPtTrigEnable(devIndexes[trigChn], i, enbaleTable);
            //        if(0 != err)
            //            return (int)ErrorDef.InvokeFailed;
            //    }
            //    else
            //    {
            //        err = HTM.SetPtTrigEnable(devIndexes[trigChn], i, 0);
            //        if (0 != err)
            //            return (int)ErrorDef.InvokeFailed;
            //    }
            //}
            //trigEnables[trigChn] = isEnable;
            //return (int)ErrorDef.Success;
        }

        public int GetTriggedCount(int trigChn, out int count)
        {
            count = 0;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (trigChn < 0 || trigChn >= TrigChannels)
                throw new ArgumentOutOfRangeException("GetTriggedCount(trigChn ,...) fialed By:trigChn = " + trigChn + " is outof range:0~" + (TrigChannels - 1));

            count = HTM.GetTrigCnt(devIndexes[trigChn]);
            return (int)ErrorDef.Success;
        }

        public int ResetTriggedCount(int trigChn)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (trigChn < 0 || trigChn >= TrigChannels)
                throw new ArgumentOutOfRangeException("ResetTriggedCount(trigChn ,...) fialed By:trigChn = " + trigChn + " is outof range:0~" + (TrigChannels - 1));

            int err = HTM.ResetTrigCnt(devIndexes[trigChn]);
            if (0 != err)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        public int GetEncoderCurrPos(int encChn, out double pos)
        {
            pos = 0;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("ResetTriggedCount(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            if (0 != HTM.GetTrigCurPos(devIndexes[encChn], out pos))
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        public int SetEncoderCurrPos(int encChn, double pos)
        {
            return (int)ErrorDef.Unsupported;
        }

        public int SyncEncoderCurrPos(int encChn)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;

            if (encChn < 0 || encChn >= EncoderChannels)
                throw new ArgumentOutOfRangeException("SyncEncoderCurrPos(encChn ,...) fialed By:encChn = " + encChn + " is outof range:0~" + (EncoderChannels - 1));

            if (0 != HTM.SyncTrigCurPos(devIndexes[encChn]))
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;

        }

        public int SoftTrigge(int[] trigChns)
        {
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            List<int> swDev = new List<int>();
            if (null != trigChns)
            {
                if (0 == trigChns.Length)
                    return (int)ErrorDef.Success;
                for (int i = 0; i < trigChns.Length; i++)
                {
                    if (trigChns[i] < 0 || trigChns[i] >= EncoderChannels)
                        throw new ArgumentOutOfRangeException(string.Format("SoftTrigge(trigChns[]) fialed By:trigChns[{0}] = {1} is outof range:0~{2}", i, trigChns[i], TrigChannels - 1));
                    if (!swDev.Contains(devIndexes[trigChns[i]]))
                        swDev.Add(devIndexes[trigChns[i]]);
                }
            }
            else
                swDev.AddRange(devIndexes);

            if (swDev.Count == 0)
                return (int)ErrorDef.Success;
            foreach (int devIdx in swDev)
                if (0 != HTM.SWPosTrig(devIdx))
                    return (int)ErrorDef.InvokeFailed;

            return (int)ErrorDef.Success;
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
            ModeError = -6,//工作模式错误 ，比如当前为触发模式
            ParamCountOutofRange = -7, //参数个数超过限制
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
                case (int)ErrorDef.ParamCountOutofRange:
                    return "Param's count is out of range";
                default://未定义的错误类型
                    return "Unknown-ErrorCode:" + errorCode;
            }
        }

        public JFRealtimeUI GetRealtimeUI()
        {
            UcCmprTrig ui = new UcCmprTrig();
            ui.SetCmprTigger(this, null, null);
            return ui;
        }
    }
}
