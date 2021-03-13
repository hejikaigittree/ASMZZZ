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
    public class JFAps168DIO : IJFModule_DIO
    {
        public bool IsOpen { get; private set; }

        public int DICount { get; private set; }

        public int DOCount { get; private set; }

        /// <summary>
        /// 凌华卡ID
        /// </summary>
        public int BoardID { get; private set; }

        private object asynLocker = new object();

        /// <summary>
        /// 自定义错误信息
        /// </summary>
        enum ErrorDef
        {
            Success = 0, //
            InvokeFailed = -3,//库函数调用出错
            NotOpen = -5, //卡未打开
        }

        internal JFAps168DIO(int boardID)
        {
            DICount = 0;
            DOCount = 0;
            IsOpen = false;
            BoardID = boardID;
        }

        /// <summary>运动控制卡DIO初始化 </summary>
        internal void Open()
        {
            int StartAxisId = 0, TotalAxis = 0, CardName = 0;
            APS168.APS_get_first_axisId(BoardID, ref StartAxisId, ref TotalAxis);
            APS168.APS_get_card_name(BoardID, ref CardName);
            //if (/*CardName != (Int32)APS_Define.DEVICE_NAME_PCI_825458 && */CardName != (Int32)APS_Define.DEVICE_NAME_AMP_20408C)
                //throw new Exception(string.Format("AMP204MC.Initialize Failed :运动控制是型号不是204C或208C！"));

            if (CardName == (Int32)APS_Define.DEVICE_NAME_AMP_20408C || CardName == (Int32)APS_Define.DEVICE_NAME_PCI_825458 /*&& TotalAxis == 4*/)
            {
                DICount = 16;
                DOCount = 16;
            }
            else
                throw new Exception(string.Format("未实现的运动控制DIO模块型号:" + CardName));
            //if (CardName == (Int32)APS_Define.DEVICE_NAME_AMP_20408C && TotalAxis == 8)
            //{
            //    DICount = 16;
            //    DOCount = 16;
            //}
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

            Int32 di_data = 0;

            lock (asynLocker)
            {
                if (!IsOpen)
                {
                    return (int)ErrorDef.NotOpen;
                }

                if (APS168.APS_read_d_input(BoardID, 0, ref di_data) != 0)
                    return (int)ErrorDef.InvokeFailed;

                if (((di_data >> (index + 8)) & 1) == 1)
                    isON = true;
                else
                    isON = false;
            }
           
            return (int)ErrorDef.Success;
        }

        public int GetAllDIs(out bool[] isONs)
        {
            isONs = new bool[DICount];

            Int32 digital_input_value = 0;
            Int32[] di_ch = new Int32[DICount];

            lock (asynLocker)
            {
                if (!IsOpen)
                {
                    return (int)ErrorDef.NotOpen;
                }

                if(APS168.APS_read_d_input(BoardID
                    , 0                     // I32 DI_Group
                    , ref digital_input_value // I32 *DI_Data
                )!=0)
                    return (int)ErrorDef.InvokeFailed;

                for (int i = 8; i < DICount+8; i++)
                {
                    di_ch[i-8] = ((digital_input_value >> i) & 1);
                    if (di_ch[i - 8] == 1)
                        isONs[i - 8] = true;
                    else
                        isONs[i - 8] = false;
                }
            }
            return (int)ErrorDef.Success;
        }

        public int GetDO(int index, out bool isON)
        {
            Int32 do_group = 0;
            Int32 do_data = 0;
            isON = false;
            if (index < 0 || index >= DOCount)
                throw new Exception(string.Format("GetDO(index = {0}, out bool isON) index is out of range:0~{1}", index, DOCount - 1));

            lock (asynLocker)
            {
                if (!IsOpen)
                {
                    return (int)ErrorDef.NotOpen;
                }
                if (APS168.APS_read_d_channel_output(BoardID, do_group, index+8, ref do_data) != 0)
                    return (int)ErrorDef.InvokeFailed;
                isON = (do_data == 1 ? true : false);
            }    
            return (int)ErrorDef.Success;
        }

        public int GetAllDOs(out bool[] isONs)
        {
            isONs = new bool[DOCount];

            Int32 digital_output_value = 0;
            Int32[] do_ch = new Int32[DOCount];

            lock (asynLocker)
            {
                if (!IsOpen)
                {
                    return (int)ErrorDef.NotOpen;
                }

                if(APS168.APS_read_d_output(BoardID
                    , 0                     // I32 DO_Group
                    , ref digital_output_value // I32 *DO_Data
                )!=0)
                    return (int)ErrorDef.InvokeFailed;

                for (int i = 8; i < DOCount+8; i++)
                {
                    do_ch[i-8] = ((digital_output_value >> i) & 1);
                    if (do_ch[i - 8] == 1)
                        isONs[i - 8] = true;
                    else
                        isONs[i - 8] = false;
                }
            }
            return (int)ErrorDef.Success;
        }

        public int SetDO(int index, bool isON)
        {
            Int32 do_group = 0;
            Int32 do_data = 0;
            
            if (index < 0 || index >= DOCount)
                throw new Exception(string.Format("SetDO(index = {0}, isON) index is out of range:0~{1}", index, DOCount - 1));
  
            lock (asynLocker)
            {
                do_data = (isON ? 1 : 0);
                if (!IsOpen)
                {
                    return (int)ErrorDef.NotOpen;
                }
                if (APS168.APS_write_d_channel_output(BoardID, do_group, index+8, do_data) != 0)
                    return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }

        public int SetDOs(bool[] isONs, int beginIndex, int count)
        {
            Int32 do_group = 0;
            Int32 do_data = 0;

            if (null == isONs)
                throw new ArgumentNullException("SetDOs(bool[] isONs ...) faied By:isONs = null");
            if (beginIndex < 0 || beginIndex >= DOCount)
                throw new ArgumentOutOfRangeException(string.Format(" SetDOs(bool[] isONs, int beginIndex = {0},int count) failed By:beginIndex is out of range 0~{1}", beginIndex, DOCount - 1));
            if (count < 0 || beginIndex + count > isONs.Count() || beginIndex + count > DOCount)
                throw new ArgumentOutOfRangeException(string.Format(" SetDOs(bool[] isONs, int beginIndex = {0},int count ={1}) failed : DOCount = {2},isONs.Count = {3}", beginIndex, count, DOCount, isONs.Length));
            
            lock (asynLocker)
            {
                if (isONs.Length != count || isONs.Length == 0)
                    return (int)ErrorDef.InvokeFailed;
                for (int i = 0; i < isONs.Length; i++)
                {
                    do_data = (isONs[i]?1:0);
                    if (!IsOpen)
                    {
                        return (int)ErrorDef.NotOpen;
                    }
                    if (APS168.APS_write_d_channel_output(BoardID, do_group, beginIndex + i+8, do_data) != 0)
                        return (int)ErrorDef.InvokeFailed;
                }
            }
            return (int)ErrorDef.Success;
        }

        public int SetDOs(bool[] isONs, int[] indexs)
        {
            Int32 do_group = 0;
            Int32 do_data = 0;
            if (null == isONs || indexs == null)
                throw new ArgumentNullException("SetDOs() failed by null == isONs or null == indexs");
            if (isONs.Length != indexs.Length)
                throw new ArgumentException("SetDOs() failed by isONs.Length != indexs.Length");
            foreach (int idx in indexs)
                if (idx < 0 || idx >= DOCount)
                    throw new ArgumentOutOfRangeException(string.Format(" SetDOs(bool[] isONs, int[] indexs) failed By: an index = {0} out of range0~{1}", idx, DOCount - 1));

            lock (asynLocker)
            {
                if (isONs.Length != indexs.Length || isONs.Length == 0)
                    return (int)ErrorDef.InvokeFailed;
                for (int i = 0; i < isONs.Length; i++)
                {
                    do_data = (isONs[i]?1:0);
                    if (!IsOpen)
                    {
                        return (int)ErrorDef.NotOpen;
                    }
                    if (APS168.APS_write_d_channel_output(BoardID, do_group, indexs[i]+8, do_data) != 0)
                        return (int)ErrorDef.InvokeFailed;
                }
            } 
            return (int)ErrorDef.Success;
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
                default:
                    break;
            }
            return ret;
        }
    }
}
