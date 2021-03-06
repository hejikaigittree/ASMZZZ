using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HT_Lib;
using JFInterfaceDef;

namespace JFHTMDevice
{

    public class HtmAio : IJFModule_AIO
    {

        internal HtmAio(/*MDD_Htm htmDev*/)
        {
            //dev = htmDev;
            IsOpen = false;
            AICount = 0;
            AOCount = 0;
            aiIndex = new List<int>();
            aoIndex = new List<int>();
        }


        //MDD_Htm dev = null; //IO模块所在的板卡
        List<int> aiIndex;//用于保存DIN 序号和HTM中的DIN序号的对应顺序
        List<int> aoIndex;
        object asynLocker = new object();

        internal void Open()
        {
            lock (asynLocker)
            {
                if (IsOpen)
                    Close();
                HTM.IO_INFO ioInfo;
                int ioCount = HTM.GetIoNum();
                for (int i = 0; i < ioCount; i++)
                {
                    int opt = HTM.GetIoInfo(i, out ioInfo);
                    if (0 != opt)
                        throw new Exception(string.Format("获取HTM-IO信息失败：HTM.GetIoInfo(i = {0},out ioInfo) return ErrorCode = {1}", i, opt));
                    if ((int)HTM.IoCardType.AIO_HTNET == ioInfo.cardType)
                    {
                        if (ioInfo.ioDir == 0) //AIn,
                        {
                            aiIndex.Add(i);
                        }
                        else//AOut
                        {
                            aoIndex.Add(i);
                        }
                    }
                }

                AICount = aiIndex.Count;
                AOCount = aoIndex.Count;
                IsOpen = true;
            }

        }

        internal void Close()
        {
            aiIndex.Clear();
            aoIndex.Clear();
            AICount = 0;
            AOCount = 0;
            IsOpen = false;
            return;
        }

        /// <summary>
        ///输入点数量 
        /// </summary>
        /// <returns></returns>
        public int AICount { get; private set; }


        /// <summary>
        /// 输出点数量 
        /// </summary>
        /// <returns></returns>
        public int AOCount { get; private set; }

        public bool IsOpen { get; private set; }


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
        public int GetAI(int index, out double volt)
        {
            if (index < 0 || index >= AICount)
                throw new Exception(string.Format("GetAI(index = {0}, volt) index is out of range:0~{1}", index, AICount - 1));
            volt = 0;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            int opt = HTM.ReadAI(aiIndex[index], out volt);
            if (opt != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取所有的输入点状态
        /// </summary>
        /// <returns>byte[0]的最低位表示第0个输入点的当前状态</returns>
        public int GetAllAIs(out double[] volts)
        {
            List<double> ret = new List<double>();
            lock (asynLocker)
            {
                if (!IsOpen)
                {
                    volts = new double[] { };
                    return (int)ErrorDef.NotOpen;
                }
                foreach (int index in aiIndex)
                {
                    double volt = 0;
                    if (0 != HTM.GetAI(aiIndex[index], out volt))
                    {
                        volts = new double[] { };
                        return (int)ErrorDef.InvokeFailed;
                    }
                    ret.Add(volt);
                }
                volts = ret.ToArray();
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 获取单个输出点状态
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="index">输出点序号，从0开始</param>
        /// <returns></returns>
        public int GetAO(int index, out double volt)
        {
            if (index < 0 || index >= AOCount)
                throw new Exception(string.Format("GetAO(index = {0}, isON) index is out of range:0~{1}", index, AOCount - 1));
            volt = 0;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            int opt = HTM.ReadAO(aoIndex[index], out volt);
            if (opt != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取所有的输出点状态
        /// </summary>
        /// <returns>byte[0]的最低位表示第0个输出点的当前状态</returns>
        public int GetAllAOs(out double[] volts)
        {
            List<double> ret = new List<double>();
            lock (asynLocker)
            {
                if (!IsOpen)
                {
                    volts = new double[] { };
                    return (int)ErrorDef.NotOpen;
                }
                foreach (int index in aoIndex)
                {
                    double volt = 0;
                    if (0 != HTM.GetAO(aoIndex[index], out volt))
                    {
                        volts = new double[] { };
                        return (int)ErrorDef.InvokeFailed;
                    }
                    ret.Add(volt);
                }
                volts = ret.ToArray();
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 设置单个输出点状态
        /// ArgumentOutofRange
        /// </summary>
        public int SetAO(int index, double volt)
        {

            if (index < 0 || index >= AOCount)
                throw new ArgumentOutOfRangeException(string.Format("SetAO(index = {0}, volt) index is out of range:0~{1}", index, AOCount - 1));
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;
            int opt = HTM.WriteAO(aoIndex[index], volt);
            if (opt != 0)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }
        /// <summary>
        /// 按顺序一次设置多个输出点状态
        /// ArgumentNull
        /// ArgumentOutofRange
        /// </summary>
        public int SetAOs(double[] volts, int beginIndex, int count)
        {
            if (null == volts)
                throw new ArgumentNullException("SetAOs(volts ...) faied By:volts = null");
            if (beginIndex < 0 || beginIndex >= AOCount)
                throw new ArgumentOutOfRangeException(string.Format(" SetAOs(double[] volts, int beginIndex = {0},int count) failed By:beginIndex is out of range 0~{1}", beginIndex, AOCount - 1));
            if (count < 0 || beginIndex + count > volts.Count() || beginIndex + count > AOCount)
                throw new ArgumentOutOfRangeException(string.Format(" SetAOs(volts, int beginIndex = {0},int count ={1}) failed : DOCount = {2},isONs.Count = {3}", beginIndex, count, AOCount, volts.Length));

            lock (asynLocker)
            {
                if (!IsOpen)
                    return (int)ErrorDef.NotOpen;
                for (int i = 0; i < count; i++)
                    if (0 != HTM.WriteAO(aoIndex[beginIndex + i], volts[i]))
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
        public int SetAOs(double[] volts, int[] indexs)
        {
            if (null == volts || indexs == null)
                throw new ArgumentNullException("SetAOs() failed by null == volts or null == indexs");
            if (volts.Length != indexs.Length)
                throw new ArgumentException("SetAOs() failed by volts.Length != indexs.Length");
            foreach (int idx in indexs)
                if (idx < 0 || idx >= AOCount)
                    throw new ArgumentOutOfRangeException(string.Format(" SetAOs(double[] volts, int[] indexs) failed By: an index = {0} out of range0~{1}", idx, AOCount - 1));
            lock (asynLocker)
            {
                for (int i = 0; i < volts.Count(); i++)
                    if (0 != HTM.WriteAO(aoIndex[i], volts[i]))
                        return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }

        }

        public string GetErrorInfo(int errorCode)
        {
            string ret = "UnDefined-Error";
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

        public int GetAIFactor(int index, out double factor)
        {
            factor = 1;
            return 0;
        }

        public int SetAIFactor(int index, double factor)
        {
            return (int)ErrorDef.InvokeFailed;
        }

        public int GetAOFactor(int index, out double factor)
        {
            factor = 1;
            return 0;
        }

        public int SetAOFactor(int index, double factor)
        {
            return (int)ErrorDef.InvokeFailed;
        }

        public int ResetAI(int index)
        {
            return (int)ErrorDef.InvokeFailed;//throw new NotImplementedException();
        }
    }
}
