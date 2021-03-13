#define DemoMode  //定义则代表整机离线模式
//#define CameraOnline //定义则表示相机在整机离线模式中可在线

using System;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using HT_Lib;
using IniDll;
using System.Collections.Concurrent;
using HalconDotNet;
using HTHalControl;
using JFMethodCommonLib;
using JFInterfaceDef;
using JFHub;

namespace DLAF 
{
    enum SystemRunMode
    {
        MODE_ONLINE = 0,
        MODE_OFFLINE = 1,
    }
    static class App2
    {
        #region 路径
        const bool use_current_dir = false;

        public static string ActivePdt = "";

        public static double ZFocus = 0;
        public static double Z_safe = 0;
        public static double ref_x;                //参考原点
        public static double ref_y;               //参考原点

        public static string currentDir = Directory.GetCurrentDirectory();

        public static string programDir = (use_current_dir ? Environment.CurrentDirectory : "D:\\ht'tech") + "\\AOI_LF";
        public static string LogFile { get { return ("D:\\Log"); } }
        public static string UserFile { get { return (programDir + "\\User\\User.db"); } }
        public static string SystemDir { get { return (programDir + "\\System"); } }
        public static string SystemUV2XYDir { get { return (programDir + "\\System\\UV2XY"); } }
        public static string SystemUV2XYResultFile { get { return (programDir + "\\System\\UV2XY" + "\\" + "UV2XY" + ".dat"); } }
        public static string BspParaFile { get { return (programDir + "\\System\\htm_config.db"); } }
        public static string SystemMatFile { get { return (programDir + "\\System\\matUV2XY.tup"); } }
        public static string SystemConfig { get { return (programDir + "\\SystemConfig"); } }
        public static string ProductDir { get { return ("D:\\Products"); } }
        public static string ProductPath { get { return ProductDir + "\\" + ActivePdt; } }
        public static string ModelPath { get { return ProductPath + "\\Models\\"; } }
        public static string AlgParamsPath { get { return ProductPath + "\\AlgParams.ini"; } }
        public static string RecipePath { get { return ProductPath + "\\Recipe\\"; } }
        public static string Fuc_LibPath { get { return Application.StartupPath + "\\VisionFlow_Funclib.dll"; } }
        public static string Json_Path { get { return ProductPath + "\\AlgorithmConfig.json"; } }
        public static string SystemParaFile { get { return (programDir + "\\System\\mechanism_paras.db"); } }
        public static string EnhanLightPath { get { return ("LightCtrl_0310.exe"); } }
        #endregion
        //public static Operations obj_Operations = new Operations(SystemParaFile, "CalibrationPara");
        public static Vision obj_Vision = new Vision(SystemParaFile, "Vision");
        public static ChuckModule obj_Chuck = new ChuckModule(SystemParaFile, "Chuck");

        public static MotionCtrl motionCtrl = new MotionCtrl();
        public static CameraCtrl cameraCtrl = new CameraCtrl();
    }

    public class MotionCtrl
    {
        public MotionCtrl()
        {

        }

        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            Unsupported = -1,//设备不支持此功能
            ParamError = -2,//参数错误（不支持的参数）
            InvokeFailed = -3,//库函数调用出错
            Allowed = 1,//调用成功，但不是所有的参数都支持
            InitFailedWhenOpenCard = -4,//
            Timeout = -5,//
        }

        /// <summary>
        /// 轴动作完成
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="errMsg"></param>
        /// <param name="timeoutSelSecond"></param>
        /// <returns></returns>
        private int MotionDone(string[] AxisName, out string errMsg, int timeoutSelSecond = 60000)
        {
            DateTime starttime = new DateTime();
            errMsg = "";
            IJFInitializable dev;
            JFDevCellInfo ci;
            for (int i = 0; i < AxisName.Length; i++)
            {
                if (!JFCMFunction.CheckDevCellName(JFCMFunction.Axis, AxisName[i], out dev, out ci, out errMsg))
                {
                    return (int)ErrorDef.InvokeFailed;
                }
                IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
                while (true)
                {
                    if (md.IsMDN(ci.ChannelIndex))
                        break;
                    DateTime nowtime = new DateTime();
                    if(nowtime.Subtract(starttime).TotalMilliseconds> timeoutSelSecond)
                        return (int)ErrorDef.Timeout;
                    Thread.Sleep(10);
                }
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 轴动作完成
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="errMsg"></param>
        /// <param name="timeoutSelSecond"></param>
        /// <returns></returns>
        private int MotionDone(string AxisName, out string errMsg, int timeoutSelSecond = 60000)
        {
            DateTime starttime = new DateTime();
            errMsg = "";
            IJFInitializable dev;
            JFDevCellInfo ci;
            
            if (!JFCMFunction.CheckDevCellName(JFCMFunction.Axis, AxisName, out dev, out ci, out errMsg))
            {
                return (int)ErrorDef.InvokeFailed;
            }
            IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            while (true)
            {
                if (md.IsMDN(ci.ChannelIndex))
                    break;
                DateTime nowtime = new DateTime();
                if (nowtime.Subtract(starttime).TotalMilliseconds > timeoutSelSecond)
                    return (int)ErrorDef.Timeout;
                Thread.Sleep(10);
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 获取轴反馈坐标
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="pos"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int GetAxisFbkPos(string AxisName,out double pos,out string errMsg)
        {
            pos = 0;
            errMsg = "";
            IJFInitializable dev;
            JFDevCellInfo ci;
            double UV2xy = 0;

            if (!JFCMFunction.CheckDevCellName(JFCMFunction.Axis, AxisName, out dev, out ci, out errMsg))
            {
                return (int)ErrorDef.InvokeFailed;
            }
            IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.GetFbkPos(ci.ChannelIndex, out UV2xy);
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = md.GetErrorInfo(errCode);
                return errCode;
            }
            pos = UV2xy;
            return (int)ErrorDef.Success; 
        }

        /// <summary>
        /// 软触发
        /// </summary>
        /// <param name="TrigChnName"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int SWPosTrig(string[] TrigChnName, out string errMsg)
        {
            IJFInitializable dev=null;
            JFDevCellInfo ci =null ;
            errMsg = "";

            if (TrigChnName.Length==0)
            {
                errMsg = "触发通道名称为空";
                return (int)ErrorDef.InvokeFailed;
            }

            int[] trigChns = new int[TrigChnName.Length];
            for (int i = 0; i < TrigChnName.Length; i++)
            {
                if (!JFCMFunction.CheckDevCellName(JFCMFunction.CmpTrig, TrigChnName[i], out dev, out ci, out errMsg))
                    return (int)ErrorDef.InvokeFailed;
                trigChns[i] = ci.ChannelIndex;
            }

            IJFModule_CmprTrigger md = (dev as IJFDevice_MotionDaq).GetCompareTrigger(ci.ModuleIndex);
            int errCode = md.SoftTrigge(trigChns);
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = md.GetErrorInfo(errCode);
                return errCode;
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        ///单轴相对运动
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="distance"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int AxisRelMove(string AxisName, double distance, out string errMsg)
        {
            errMsg = "";
            IJFInitializable dev;
            JFDevCellInfo ci;
            string errInfo = "";

            if (!JFCMFunction.CheckDevCellName(JFCMFunction.Axis, AxisName, out dev, out ci, out errInfo))
            {
                return (int)ErrorDef.InvokeFailed;
            }
            IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.RelMove(ci.ChannelIndex, distance);
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = md.GetErrorInfo(errCode);
                return errCode;
            }

            errCode = MotionDone(AxisName, out errMsg);
            if (errCode != 0)
            {
                return errCode;
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 单轴绝对运动
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="distance"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int AxisAbsMove(string AxisName, double distance, out string errMsg)
        {
            errMsg = "";
            IJFInitializable dev;
            JFDevCellInfo ci;
            string errInfo = "";

            if (!JFCMFunction.CheckDevCellName(JFCMFunction.Axis, AxisName, out dev, out ci, out errInfo))
            {
                return (int)ErrorDef.InvokeFailed;
            }
            IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.AbsMove(ci.ChannelIndex, distance);
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = md.GetErrorInfo(errCode);
                return errCode;
            }

            errCode = MotionDone(AxisName, out errMsg);
            if (errCode != 0)
            {
                return errCode;
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 多轴绝对运动(非插补运动)
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="distance"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int MultiAxisAbsMove(string[] AxisName, double[] distance, out string errMsg)
        {
            errMsg = "";

            if (AxisName.Length == 0)
            {
                errMsg = "轴通道名称为空";
                return (int)ErrorDef.InvokeFailed;
            }

            if (AxisName.Length != distance.Length)
            {
                errMsg = "轴通道名称数量与位置坐标数量不匹配";
                return (int)ErrorDef.InvokeFailed;
            }

            for (int i = 0; i < AxisName.Length; i++)
            {
                if(AxisAbsMove(AxisName[i],distance[i],out errMsg) !=0)
                {
                    return (int)ErrorDef.InvokeFailed;
                }
            }

            int errCode = 0;
            errCode = MotionDone(AxisName, out errMsg);
            if (errCode != 0)
            {
                return errCode;
            }
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 多轴相对运动(非插补运动)
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="distance"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int MultiAxisRelMove(string[] AxisName, double[] distance, out string errMsg)
        {
            errMsg = "";

            if (AxisName.Length == 0)
            {
                errMsg = "轴通道名称为空";
                return (int)ErrorDef.InvokeFailed;
            }

            if (AxisName.Length != distance.Length)
            {
                errMsg = "轴通道名称数量与位置坐标数量不匹配";
                return (int)ErrorDef.InvokeFailed;
            }

            for (int i = 0; i < AxisName.Length; i++)
            {
                if (AxisRelMove(AxisName[i], distance[i], out errMsg) != 0)
                {
                    return (int)ErrorDef.InvokeFailed;
                }
            }

            int errCode = 0;
            errCode = MotionDone(AxisName, out errMsg);
            if (errCode != 0)
            {
                return errCode;
            }
            return (int)ErrorDef.Success;
        }
    }

    public class CameraCtrl
    {
        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            Unsupported = -1,//设备不支持此功能
            ParamError = -2,//参数错误（不支持的参数）
            InvokeFailed = -3,//库函数调用出错
            Allowed = 1,//调用成功，但不是所有的参数都支持
            InitFailedWhenOpenCard = -4,//
        }

        public CameraCtrl()
        {

        }

        public int ClearImageQueue(string cmrName,out string errMsg)
        {
            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            errMsg = "";

            if (!JFCMFunction.CheckDevCellName(JFCMFunction.Cmr, cmrName, out dev, out ci, out errMsg))
                return (int)ErrorDef.InvokeFailed;

            IJFDevice_Camera cmr = (dev as IJFDevice_Camera);
            int errCode = cmr.ClearBuff();
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = cmr.GetErrorInfo(errCode);
                return errCode;
            }
            return (int)ErrorDef.Success;
        }

        public int CaputreOneImage(string cmrName, string imageType, out HObject hObject, out string errMsg, int timeoutMilSwconds = -1)
        {
            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            IJFImage image = null;
            hObject = null;
            errMsg = "";

            if (!JFCMFunction.CheckDevCellName(JFCMFunction.Cmr, cmrName, out dev, out ci, out errMsg))
                return (int)ErrorDef.InvokeFailed;

            IJFDevice_Camera cmr = (dev as IJFDevice_Camera);
            int errCode = cmr.GrabOne(out image, timeoutMilSwconds);
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = cmr.GetErrorInfo(errCode);
                return errCode;
            }

            if (GenImgObject(imageType, image, out hObject, out errMsg) != 0)
            {
                return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }

        private int GenImgObject(string imageType,IJFImage image,out HObject hObject,out string errMsg)
        {
            hObject = null;
            errMsg = "";
            object ImgObject;
            int errCode = image.GenImgObject(out ImgObject, imageType);
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = image.GetErrorInfo(errCode);
                return errCode;
            }
            hObject = (HObject)ImgObject;

            return (int)ErrorDef.Success;
        }
    }

}