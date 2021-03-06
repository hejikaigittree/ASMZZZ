using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HT_Lib;
using JFInterfaceDef;

namespace JFHTMDevice
{


    public class HtmDio : IJFModule_DIO
    {

        internal HtmDio(/*MDD_Htm htmDev*/)
        {
            //dev = htmDev;
            IsOpen = false;
            DICount = 0;
            DOCount = 0;
            diIndex = new List<int>();
            doIndex = new List<int>();
        }


        //MDD_Htm dev = null; //IO模块所在的板卡
        List<int> diIndex;//用于保存DIN 序号和HTM中的DIN序号的对应顺序
        List<int> doIndex;
        object asynLocker = new object();

        internal void Open()
        {
            lock (asynLocker)
            {
                if (IsOpen)
                    Close();
                HTM.IO_INFO ioInfo;
                //int? ioMaxCount = dev.GetInitParamValue("最大IO数量") as int?;
                //if (null == ioMaxCount)
                //    throw new Exception("HTM控制器初始化参数\"最大IO数量\"未设置！ ");
                int ioCount = HTM.GetIoNum();
                for (int i = 0; i < ioCount; i++)
                {
                    int opt = HTM.GetIoInfo(i, out ioInfo);
                    if (0 != opt)
                        throw new Exception(string.Format("获取HTM-IO信息失败：HTM.GetIoInfo(i = {0},out ioInfo) return ErrorCode = {1}", i, opt));
                    if ((int)HTM.IoCardType.DIO_HTNET == ioInfo.cardType)
                    {
                        if (ioInfo.ioDir == 0) //DIn,
                        {
                            diIndex.Add(i);
                        }
                        else//DOut
                        {
                            doIndex.Add(i);
                        }
                    }
                }

                DICount = diIndex.Count;
                DOCount = doIndex.Count;
                IsOpen = true;
            }

        }

        internal void Close()
        {
            diIndex.Clear();
            doIndex.Clear();
            DICount = 0;
            DOCount = 0;
            IsOpen = false;
            return;
        }

        /// <summary>
        ///输入点数量 
        /// </summary>
        /// <returns></returns>
        public int DICount { get; private set; }


        /// <summary>
        /// 输出点数量 
        /// </summary>
        /// <returns></returns>
        public int DOCount { get; private set; }

        public bool IsOpen { get; private set; }

        object _ioLocker = new object();


        enum ErrorDef
        {
            Success = 0, //
            InvokeFailed = -3,//库函数调用出错
            NotOpen = -5, //卡未打开
        }
        /// <summary>
        /// 获取单个输入点状态
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="index">输入点序号，从0开始</param>
        /// <returns></returns>
        public int GetDI(int index, out bool isON)
        {
            if (index < 0 || index >= DICount)
                throw new Exception(string.Format("GetDI(index = {0}, isON) index is out of range:0~{1}", index, DICount - 1));

                isON = false;
                if (!IsOpen)
                    return (int)ErrorDef.NotOpen;

                int val = HTM.ReadDI(diIndex[index]);
                //int val = HTM.GetDI(diIndex[index]);
                isON = val != 0;
                return (int)ErrorDef.Success;
            
        }

        /// <summary>
        /// 获取所有的输入点状态
        /// </summary>
        /// <returns>byte[0]的最低位表示第0个输入点的当前状态</returns>
        public int GetAllDIs(out bool[] isONs)
        {
            List<bool> ret = new List<bool>();
            lock (asynLocker)
            {
                if (!IsOpen)
                {
                    isONs = new bool[] { };
                    return (int)ErrorDef.NotOpen;
                }
                foreach (int index in diIndex)
                {
                    int sig = 0;
                    //if (0 != HTM.GetDio(diIndex[index], out sig))
                    //{
                    //    isONs = new bool[] { };
                    //    return (int)ErrorDef.InvokeFailed;
                    //}
                    sig = HTM.GetDI(index);
                    ret.Add(sig != 0);
                }
                isONs = ret.ToArray();
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 获取单个输出点状态
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="index">输出点序号，从0开始</param>
        /// <returns></returns>
        public int GetDO(int index, out bool isON)
        {
            if (index < 0 || index >= DOCount)
                throw new Exception(string.Format("GetDO(index = {0}, isON) index is out of range:0~{1}", index, DOCount - 1));
            isON = false;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            int val;
            val = HTM.ReadDO(doIndex[index]);
            //val = HTM.GetDO(doIndex[index]);
            isON = val != 0;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取所有的输出点状态
        /// </summary>
        /// <returns>byte[0]的最低位表示第0个输出点的当前状态</returns>
        public int GetAllDOs(out bool[] isONs)
        {
            List<bool> ret = new List<bool>();
            lock (asynLocker)
            {
                if (!IsOpen)
                {
                    isONs = new bool[] { };
                    return (int)ErrorDef.NotOpen;
                }
                foreach (int index in doIndex)
                {
                    int sig = 0;
                    //if (0 != HTM.GetDio(doIndex[index], out sig))
                    //{
                    //    isONs = new bool[] { };
                    //    return (int)ErrorDef.InvokeFailed;
                    //}
                    sig = HTM.GetDO(index);
                    ret.Add(sig != 0);
                }
                isONs = ret.ToArray();
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 设置单个输出点状态
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="index">输出点序号，从0开始</param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public int SetDO(int index, bool isON)
        {

            if (index < 0 || index >= DOCount)
                throw new ArgumentOutOfRangeException(string.Format("SetDO(index = {0}, isON) index is out of range:0~{1}", index, DOCount - 1));
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            int opt = 0;
            lock (_ioLocker)
            {
                opt = HTM.WriteDO(doIndex[index], isON ? 1 : 0);
            }
            if (opt != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }
        /// <summary>
        /// 按顺序一次设置多个输出点状态
        /// ArgumentNull
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="bits">状态值</param>
        /// <param name="beginDOIndex">待设置输出点的起始序号（从0开始）</param>
        /// <returns></returns>
        public int SetDOs(bool[] isONs, int beginIndex, int count)
        {
            if (null == isONs)
                throw new ArgumentNullException("SetDOs(bool[] isONs ...) faied By:isONs = null");
            if (beginIndex < 0 || beginIndex >= DOCount)
                throw new ArgumentOutOfRangeException(string.Format(" SetDOs(bool[] isONs, int beginIndex = {0},int count) failed By:beginIndex is out of range 0~{1}", beginIndex, DOCount - 1));
            if (count < 0 || beginIndex + count > isONs.Count() || beginIndex + count > DOCount)
                throw new ArgumentOutOfRangeException(string.Format(" SetDOs(bool[] isONs, int beginIndex = {0},int count ={1}) failed : DOCount = {2},isONs.Count = {3}", beginIndex, count, DOCount, isONs.Length));

            lock (asynLocker)
            {
                if (!IsOpen)
                    return (int)ErrorDef.NotOpen;
                for (int i = 0; i < count; i++)
                    if (0 != HTM.WriteDO(doIndex[beginIndex + i], isONs[i] ? 1 : 0))
                        return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }


        /// <summary>
        /// 一次设置多个输出点状态
        /// ArgumentNull
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="bits">待设置的状态</param>
        /// <param name="doIndexs">待设置的点位序号</param>
        /// <returns></returns>
        public int SetDOs(bool[] isONs, int[] indexs)
        {
            if (null == isONs || indexs == null)
                throw new ArgumentNullException("SetDOs() failed by null == isONs or null == indexs");
            if (isONs.Length != indexs.Length)
                throw new ArgumentException("SetDOs() failed by isONs.Length != indexs.Length");
            foreach (int idx in indexs)
                if (idx < 0 || idx >= DOCount)
                    throw new ArgumentOutOfRangeException(string.Format(" SetDOs(bool[] isONs, int[] indexs) failed By: an index = {0} out of range0~{1}", idx, DOCount - 1));
            lock (asynLocker)
            {
                for (int i = 0; i < isONs.Count(); i++)
                    if (0 != HTM.WriteDO(doIndex[indexs[i]], isONs[i] ? 1 : 0))
                        return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }

        }

        public string GetErrorInfo(int errorCode)
        {
            string ret = "UnDefined-Error:" + errorCode;
            switch (errorCode)
            {
                case (int)ErrorDef.Success:
                    ret = "Success";
                    break;
                case (int)ErrorDef.NotOpen://卡未打开
                    ret = "Device is not open ";
                    break;
                case (int)ErrorDef.InvokeFailed: //调用库函数出错
                    ret = "Invoke Failed ";
                    break;
                default:
                    break;
            }


            return ret;
        }
    }
}
