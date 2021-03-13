using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APS168_W64;
using APS_Define_W32;

namespace JFADLinkDevice
{
    public class JFAdlinkPciDio : IJFModule_DIO
    {
        public bool IsOpen { get; private set; }

        public int DICount { get; private set; }

        public int DOCount { get; private set; }

        /// <summary>
        /// 凌华卡ID
        /// </summary>
        // public int BoardID { get; private set; }

        private object asynLocker = new object();

        /// <summary>
        /// 自定义错误信息
        /// </summary>
        enum ErrorDef
        {
            Success = 0, //
            InvokeFailed = -3,//库函数调用出错
            NotOpen = -5, //卡未打开
            Unsupport = -6,//
        }

        internal JFAdlinkPciDio(JFDev_ADLinkPciDioMotionDaq.CardType cardType ,short devHandle)
        {
            DICount = 0;
            DOCount = 0;
            IsOpen = true; //已经在MD中打开
            _cardType = cardType;
            _devHandle = (ushort)devHandle;
        }

        JFDev_ADLinkPciDioMotionDaq.CardType _cardType = JFDev_ADLinkPciDioMotionDaq.CardType.PCI_Unknown;
        ushort _devHandle = 0;

        /// <summary>运动控制卡DIO初始化 </summary>
        internal void Open()
        {
           //确定IO数量
           switch(_cardType)
            {
                case JFDev_ADLinkPciDioMotionDaq.CardType.PCI_Unknown:
                    throw new ArgumentException("错误的卡类型参数:" + JFDev_ADLinkPciDioMotionDaq.CardType.PCI_Unknown.ToString());
                    break;
                //PCI_6208V = 1,
                //PCI_6208A = 2,
                //PCI_6308V = 3,
                //PCI_6308A = 4,
                //PCI_7200 = 5,
                //PCI_7230 = 6,
                //PCI_7233 = 7,
                //PCI_7234 = 8,
                //PCI_7248 = 9,
                //PCI_7249 = 10,
                //PCI_7250 = 11,
                //PCI_7252 = 12,
                //PCI_7296 = 13,
                //PCI_7300A_RevA = 14,
                //PCI_7300A_RevB = 15,
                //PCI_7432 = 16,
                case JFDev_ADLinkPciDioMotionDaq.CardType.PCI_7433:// = 17,
                    DOCount = 0;
                    DICount = 64;
                    break;
            //PCI_7434 = 18,
            //PCI_8554 = 19,
            //PCI_9111DG = 20,
            //PCI_9111HR = 21,
            //PCI_9112 = 22,
            //PCI_9113 = 23,
            //PCI_9114DG = 24,
            //PCI_9114HG = 25,
            //PCI_9118DG = 26,
            //PCI_9118HG = 27,
            //PCI_9118HR = 28,
            //PCI_9810 = 29,
            //PCI_9812 = 30,
            //PCI_7396 = 31,
            //PCI_9116 = 32,
            //PCI_7256 = 33,
            //PCI_7258 = 34,
            //PCI_7260 = 35,
            //PCI_7452 = 36,
            //PCI_7442 = 37,
                default:
                    throw new ArgumentException("暂不支持的卡型号:" + _cardType.ToString());
                    //break;
            }
            IsOpen = true;
        }

        internal void Close()
        {
            DICount = 0;
            DOCount = 0;
            IsOpen = false;
            return;
        }

        public int GetDI(int index, out bool isON)
        {
            if (index < 0 || index >= DICount)
                throw new Exception(string.Format("GetDI(index = {0}, isON) index is out of range:0~{1}", index, DICount - 1));
            isON = false;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;


            //lock (asynLocker)
            {
                if (!IsOpen)
                {
                    return (int)ErrorDef.NotOpen;
                }
                ushort diVals = 0;
                if (0 != DASK.DI_ReadLine(_devHandle, (ushort)(index / 32), (ushort)(index%32),out diVals))
                    return (int)ErrorDef.InvokeFailed;

                isON = diVals !=0;
            }
           
            return (int)ErrorDef.Success;
        }

        public int GetAllDIs(out bool[] isONs)
        {
            isONs = new bool[DICount];

            //lock (asynLocker)
            {
                if (!IsOpen)
                {
                    return (int)ErrorDef.NotOpen;
                }
                uint diVals = 0;
                ushort ports = (ushort)((DICount + 31) / 32);
                for(ushort i = 0; i < ports;i++)
                {
                    if (0 != DASK.DI_ReadPort(_devHandle, i, out diVals))
                        return (int)ErrorDef.InvokeFailed;
                    for(int j = 0;j < 32;j++)
                    {
                        if (i * 32 + j == DICount)
                            break;
                        isONs[i*32 + j] = (diVals & (1 << j )) != 0;
                    }
                }


                //DASK.DO_ReadLine()


            }
            return (int)ErrorDef.Success;
        }

        public int GetDO(int index, out bool isON)
        {
            if (index < 0 || index >= DOCount)
                throw new Exception(string.Format("GetDO(index = {0}, isON) index is out of range:0~{1}", index, DOCount - 1));
            isON = false;
            if (!IsOpen)
                return (int)ErrorDef.NotOpen;


            //lock (asynLocker)
            {

                ushort doVal = 0;
                if (0 != DASK.DO_ReadLine(_devHandle, (ushort)(index / 32), (ushort)(index % 32), out doVal))
                    return (int)ErrorDef.InvokeFailed;

                isON = doVal != 0;
            }

            return (int)ErrorDef.Success;
        }

        public int GetAllDOs(out bool[] isONs)
        {
            isONs = new bool[DOCount];

            //lock (asynLocker)
            {
                if (!IsOpen)
                {
                    return (int)ErrorDef.NotOpen;
                }
                uint doVals = 0;
                ushort ports = (ushort)((DOCount + 31) / 32);
                for (ushort i = 0; i < ports; i++)
                {
                    if (0 != DASK.DO_ReadPort(_devHandle, i, out doVals))
                        return (int)ErrorDef.InvokeFailed;
                    for (int j = 0; j < 32; j++)
                    {
                        if (i * 32 + j == DOCount)
                            break;
                        isONs[i * 32 + j] = (doVals & (1 << j)) != 0;
                    }
                }
            }
            return (int)ErrorDef.Success;
        }

        public int SetDO(int index, bool isON)
        {
                
            
            if (index < 0 || index >= DOCount)
                throw new Exception(string.Format("SetDO(index = {0}, isON) index is out of range:0~{1}", index, DOCount - 1));
  
            //lock (asynLocker)
            {

                    if (DASK.DO_WriteLine(_devHandle, (ushort)(index / 32), (ushort)(index % 32), (ushort)(isON ? 1 : 0)) != 0)
                        return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }

        public int SetDOs(bool[] isONs, int beginIndex, int count)
        {
            return (int)ErrorDef.Unsupport;
            //Int32 do_group = 0;
            //Int32 do_data = 0;

            //if (null == isONs)
            //    throw new ArgumentNullException("SetDOs(bool[] isONs ...) faied By:isONs = null");
            //if (beginIndex < 0 || beginIndex >= DOCount)
            //    throw new ArgumentOutOfRangeException(string.Format(" SetDOs(bool[] isONs, int beginIndex = {0},int count) failed By:beginIndex is out of range 0~{1}", beginIndex, DOCount - 1));
            //if (count < 0 || beginIndex + count > isONs.Count() || beginIndex + count > DOCount)
            //    throw new ArgumentOutOfRangeException(string.Format(" SetDOs(bool[] isONs, int beginIndex = {0},int count ={1}) failed : DOCount = {2},isONs.Count = {3}", beginIndex, count, DOCount, isONs.Length));
            
            //lock (asynLocker)
            //{
            //    if (isONs.Length != count || isONs.Length == 0)
            //        return (int)ErrorDef.InvokeFailed;
            //    for (int i = 0; i < isONs.Length; i++)
            //    {
            //        do_data = (isONs[i]?1:0);
            //        if (!IsOpen)
            //        {
            //            return (int)ErrorDef.NotOpen;
            //        }
            //        if (APS168.APS_write_d_channel_output(BoardID, do_group, beginIndex + i+8, do_data) != 0)
            //            return (int)ErrorDef.InvokeFailed;
            //    }
            //}
            //return (int)ErrorDef.Success;
        }

        public int SetDOs(bool[] isONs, int[] indexs)
        {
            return (int)ErrorDef.Unsupport;
            //Int32 do_group = 0;
            //Int32 do_data = 0;
            //if (null == isONs || indexs == null)
            //    throw new ArgumentNullException("SetDOs() failed by null == isONs or null == indexs");
            //if (isONs.Length != indexs.Length)
            //    throw new ArgumentException("SetDOs() failed by isONs.Length != indexs.Length");
            //foreach (int idx in indexs)
            //    if (idx < 0 || idx >= DOCount)
            //        throw new ArgumentOutOfRangeException(string.Format(" SetDOs(bool[] isONs, int[] indexs) failed By: an index = {0} out of range0~{1}", idx, DOCount - 1));

            //lock (asynLocker)
            //{
            //    if (isONs.Length != indexs.Length || isONs.Length == 0)
            //        return (int)ErrorDef.InvokeFailed;
            //    for (int i = 0; i < isONs.Length; i++)
            //    {
            //        do_data = (isONs[i]?1:0);
            //        if (!IsOpen)
            //        {
            //            return (int)ErrorDef.NotOpen;
            //        }
            //        if (APS168.APS_write_d_channel_output(BoardID, do_group, indexs[i]+8, do_data) != 0)
            //            return (int)ErrorDef.InvokeFailed;
            //    }
            //} 
            //return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取当前异常信息
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
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
                case (int)ErrorDef.Unsupport:
                    ret = "Unsupported this function!";
                    break;
                default:
                    break;
            }
            return ret;
        }
    }
}
