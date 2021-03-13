using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFHub
{
    /// <summary>
    /// 设备通道类型
    /// </summary>
    public enum JFDevCellType
    {
        unknown = 0, //未定义的
        DI,
        DO,
        Axis,
        AI,
        AO,
        CmpTrig,
        Light,
        Trig
    }

    /// <summary>
    /// 设备通道对象 ， 包含设备对象/通道类型/通道信息
    /// </summary>
    public class JFDevChannel
    {
        ///// <summary>
        ///// 通道类型DI 数字量输入
        ///// </summary>
        //public static string CategoryDI = "DI";

        ///// <summary>
        ///// 通道类型DO 数字量输出
        ///// </summary>
        //public static string CategoryDO = "DO";
        //public static string CategoryAxis = "Axis";
        //public static string CategoryAI = "AI";//
        //public static string CategoryAO = "AO";

        //public static string CategoryCmpTrig = "CmpTrig";

        //public static string CategoryLightChn = "LightChn";
        //public static string CategoryTrigChn = "TrigChn";//触发控制器通道

        /// <summary>
        /// 检查DIO/轴...等设备通道是否存在
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <param name="dev"></param>
        /// <param name="ci"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public static bool CheckChannel(JFDevCellType cellType,string name,out IJFDevice dev,out JFDevCellInfo ci,out string errorInfo)
        {
            dev = null;
            ci = null;
            errorInfo = "Success";


            if (string.IsNullOrEmpty(name))
            {
                errorInfo = "参数项\"cellName\"为空";
                return false;
            }
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            JFInitorManager initorMgr = JFHubCenter.Instance.InitorManager;
            JFDevCellInfo cellInfo = null;
            if (cellType == JFDevCellType.DO) //获取数字量输出通道信息
            {
                cellInfo = nameMgr.GetDoCellInfo(name);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在DO：" + name;
                    return false;
                }

                dev = initorMgr.GetInitor(cellInfo.DeviceID) as IJFDevice;
                if (null == dev)
                {
                    errorInfo = "DO:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_MotionDaq).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "DO:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是Device_MotionDaq ";
                    return false;
                }

                IJFDevice_MotionDaq md = dev as IJFDevice_MotionDaq;
                if (!md.IsInitOK)
                {
                    errorInfo = "DO:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "DO:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ModuleIndex >= md.DioCount)
                {
                    errorInfo = "DO:\"" + name + "\" ModuleIndex = :" + cellInfo.ModuleIndex + "超出设备DIO模块数量: " + md.DioCount;
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.GetDio(cellInfo.ModuleIndex).DOCount)
                {
                    errorInfo = "DO:\"" + name + "\" Channel = :" + cellInfo.ChannelIndex + "超出模块DO通道数量: " + md.GetDio(cellInfo.ModuleIndex).DOCount;
                    return false;
                }
                ci = cellInfo;
                errorInfo = "Success";
                return true;

            }
            else if (cellType == JFDevCellType.DI) //获取数字量输入
            {
                cellInfo = nameMgr.GetDiCellInfo(name);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在DI：" + name;
                    return false;
                }

                dev = initorMgr.GetInitor(cellInfo.DeviceID) as IJFDevice;
                if (null == dev)
                {
                    errorInfo = "DI:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_MotionDaq).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "DI:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是Device_MotionDaq ";
                    return false;
                }

                IJFDevice_MotionDaq md = dev as IJFDevice_MotionDaq;
                if (!md.IsInitOK)
                {
                    errorInfo = "DI:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "DI:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ModuleIndex >= md.DioCount)
                {
                    errorInfo = "DI:\"" + name + "\" ModuleIndex = :" + cellInfo.ModuleIndex + "超出设备DIO模块数量: " + md.DioCount;
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.GetDio(cellInfo.ModuleIndex).DICount)
                {
                    errorInfo = "DI:\"" + name + "\" Channel = :" + cellInfo.ChannelIndex + "超出模块DI通道数量: " + md.GetDio(cellInfo.ModuleIndex).DOCount;
                    return false;
                }
                ci = cellInfo;
                errorInfo = "Success";
                return true;
            }
            else if (cellType == JFDevCellType.AI)
            {

            }
            else if (cellType == JFDevCellType.AO)
            {

            }
            else if (cellType == JFDevCellType.Axis)
            {
                cellInfo = nameMgr.GetAxisCellInfo(name);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在Axis：" + name;
                    return false;
                }

                dev = initorMgr.GetInitor(cellInfo.DeviceID) as IJFDevice;
                if (null == dev)
                {
                    errorInfo = "Axis:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_MotionDaq).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "Axis:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是Device_MotionDaq ";
                    return false;
                }

                IJFDevice_MotionDaq md = dev as IJFDevice_MotionDaq;
                if (!md.IsInitOK)
                {
                    errorInfo = "Axis:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "Axis:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ModuleIndex >= md.McCount)
                {
                    errorInfo = "Axis:\"" + name + "\" ModuleIndex = :" + cellInfo.ModuleIndex + "超出设备轴模块数量: " + md.McCount;
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.GetMc(cellInfo.ModuleIndex).AxisCount)
                {
                    errorInfo = "Axis:\"" + name + "\" Channel = :" + cellInfo.ModuleIndex + "超出模块轴通道数量: " + md.GetMc(cellInfo.ModuleIndex).AxisCount;
                    return false;
                }
                ci = cellInfo;
                errorInfo = "Success";
                return true;
            }
            else if (cellType == JFDevCellType.CmpTrig)
            {
                cellInfo = nameMgr.GetCmpTrigCellInfo(name);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在CmpTrig：" + name;
                    return false;
                }

                dev = initorMgr.GetInitor(cellInfo.DeviceID) as IJFDevice;
                if (null == dev)
                {
                    errorInfo = "CmpTrig:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_MotionDaq).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "CmpTrig:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是Device_MotionDaq ";
                    return false;
                }

                IJFDevice_MotionDaq md = dev as IJFDevice_MotionDaq;
                if (!md.IsInitOK)
                {
                    errorInfo = "CmpTrig:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "CmpTrig:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ModuleIndex >= md.CompareTriggerCount)
                {
                    errorInfo = "CmpTrig:\"" + name + "\" ModuleIndex = :" + cellInfo.ModuleIndex + "超出设备比较触发模块数量: " + md.CompareTriggerCount;
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.GetCompareTrigger(cellInfo.ModuleIndex).EncoderChannels)
                {
                    errorInfo = "CmpTrig:\"" + name + "\" Channel = :" + cellInfo.ModuleIndex + "超出模块比较触发通道数量: " + md.GetCompareTrigger(cellInfo.ModuleIndex).EncoderChannels;
                    return false;
                }
                ci = cellInfo;
                errorInfo = "Success";
                return true;
            }
            else if (cellType == JFDevCellType.Light)
            {
                cellInfo = nameMgr.GetLightCtrlChannelInfo(name);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在LightCtrl：" + name;
                    return false;
                }

                dev = initorMgr.GetInitor(cellInfo.DeviceID) as IJFDevice;
                if (null == dev)
                {
                    errorInfo = "LightCtrl:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_LightController).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "LightCtrl:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是IJFDevice_LightController ";
                    return false;
                }

                IJFDevice_LightController md = dev as IJFDevice_LightController;
                if (!md.IsInitOK)
                {
                    errorInfo = "LightCtrl:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "LightCtrl:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.LightChannelCount)
                {
                    errorInfo = "LightCtrl:\"" + name + "\" Channel = :" + cellInfo.ChannelIndex + "超出设备光源通道数量: " + md.LightChannelCount;
                    return false;
                }
                ci = cellInfo;
                errorInfo = "Success";
                return true;
            }
            else if (cellType == JFDevCellType.Trig)
            {
                cellInfo = nameMgr.GetTrigCtrlChannelInfo(name);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在LightTrig：" + name;
                    return false;
                }

                dev = initorMgr.GetInitor(cellInfo.DeviceID) as IJFDevice;
                if (null == dev)
                {
                    errorInfo = "LightTrig:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_TrigController).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "LightTrig:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是IJFDevice_TrigController ";
                    return false;
                }

                IJFDevice_TrigController md = dev as IJFDevice_TrigController;
                if (!md.IsInitOK)
                {
                    errorInfo = "LightTrig:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "LightTrig:\"" + name + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.TrigChannelCount)
                {
                    errorInfo = "LightTrig:\"" + name + "\" Channel = :" + cellInfo.ChannelIndex + "超出设备触发通道数量: " + md.TrigChannelCount;
                    return false;
                }
                ci = cellInfo;
                errorInfo = "Success";
                return true;
            }
            else
            {
                errorInfo = "不支持的参数项\"CellType\" = " + cellType.ToString();
            }
            return false;
        }


        public JFDevChannel(JFDevCellType cellType, string name)
        {
            CellType = cellType;
            Name = name;
            
        }
        public JFDevCellType CellType { get; private set; }

        public string Name { get; private set; }



        /// <summary>
        /// 只是获取通道对应的设备，不做安全性检查（如通道序号是否合法/设备是否已经打开...）
        /// </summary>
        /// <returns></returns>
        public IJFDevice Device()
        {
            JFDevCellInfo ci = CellInfo();
            if (null == ci)
                return null;
            IJFInitializable initor =  JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID);
            switch (CellType)
            {
                case JFDevCellType.DI:
                    return initor as IJFDevice_MotionDaq;
                case JFDevCellType.DO:
                    return initor as IJFDevice_MotionDaq;
                case JFDevCellType.Axis:
                    return initor as IJFDevice_MotionDaq;
                case JFDevCellType.AI:
                    return initor as IJFDevice_MotionDaq;
                case JFDevCellType.AO:
                    return initor as IJFDevice_MotionDaq;
                case JFDevCellType.CmpTrig:
                    return initor as IJFDevice_MotionDaq;
                case JFDevCellType.Light:
                    return initor as IJFDevice_LightController;
                case JFDevCellType.Trig:
                    return initor as IJFDevice_TrigController;
                default:
                    break;
            }
            return null;

        }

        public JFDevCellInfo CellInfo()
        {

            switch (CellType)
            {
                case JFDevCellType.DI:
                    return JFHubCenter.Instance.MDCellNameMgr.GetDiCellInfo(Name);
                case JFDevCellType.DO:
                    return JFHubCenter.Instance.MDCellNameMgr.GetDoCellInfo(Name);
                case JFDevCellType.Axis:
                    return JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(Name);
                case JFDevCellType.AI:
                    return JFHubCenter.Instance.MDCellNameMgr.GetAiCellInfo(Name);
                case JFDevCellType.AO:
                    return JFHubCenter.Instance.MDCellNameMgr.GetAoCellInfo(Name);
                case JFDevCellType.CmpTrig:
                    return JFHubCenter.Instance.MDCellNameMgr.GetCmpTrigCellInfo(Name);
                case JFDevCellType.Light:
                    return JFHubCenter.Instance.MDCellNameMgr.GetLightCtrlChannelInfo(Name);
                case JFDevCellType.Trig:
                    return JFHubCenter.Instance.MDCellNameMgr.GetTrigCtrlChannelInfo(Name);
                default:
                    break;
            }
            return null;

        }

        public bool IsDevOpen()
        {
            IJFDevice dev = Device();
            if (null == dev)
                return false;


            return dev.IsDeviceOpen;
        }

        /// <summary>
        /// 打开并使能设备通道
        /// </summary>
        /// <param name="errInfo"></param>
        /// <returns></returns>
        public bool OpenDev(out string errInfo)
        {
            errInfo = "Success";
            if(string.IsNullOrEmpty(Name))
            {
                errInfo = "名称未设置/空字串";
                return false;
            }

            JFDevCellInfo ci = CellInfo();
            if (null == ci)
            {
                errInfo = "通道名称:\"" + Name + "\"在设备名称表中不存在";
                return false;
            }

            IJFDevice dev = Device();
            if(dev == null)
            {
                errInfo = "通道:\"" + Name + "\"所属设备:\"" + ci.DeviceID + "\"在设备表中不存在";
                return false;
            }

            if(!dev.IsDeviceOpen)
            {
                int errCode = dev.OpenDevice();
                if(0!= errCode)
                {
                    errInfo = "通道:\"" + Name + "\"所属设备:\"" + ci.DeviceID + "\"打开失败:" + dev.GetErrorInfo(errCode);
                    return false;
                }
            }

            //switch(CellType)
            //{
            //    case JFDevCellType.Axis:
            //        {
            //            IJFDevice_MotionDaq devMD = dev as IJFDevice_MotionDaq;
            //            if(ci.ModuleIndex>= devMD.McCount)
            //            {
            //                errInfo = CellType + "通道:\"" + Name + "\"模块序号超过设备:\"" + ci.DeviceID + "\"允许范围0~" + (devMD.McCount - 1);
            //                return false;
            //            }
            //            IJFModule_Motion mm = devMD.GetMc(ci.ModuleIndex);
            //            if(ci.ChannelIndex >= mm.AxisCount)
            //            {
            //                errInfo = CellType + "通道:\"" + Name + "\"通道序号超过模块:\"" + ci.DeviceID + "-" + ci.ModuleIndex + "\"允许范围0~" + (mm.AxisCount - 1);
            //                return false;
            //            }

            //        }
            //        break;
            //    case JFDevCellType.Light:
            //        break;
            //    case JFDevCellType.Trig:
            //        break;
            //}


            return true;
        }


        ///// <summary>
        ///// 使通道可用（如伺服上电，光源/触发可用）
        ///// 建议在 打开设备->检查通道可用性 之后调用
        ///// </summary>
        ///// <param name="errorInfo"></param>
        ///// <returns></returns>
        public bool EnabledChannel(out string errorInfo)
        {
            errorInfo = "Unknown-Error";
            bool isOK = false;
            if (!CheckAvalid(out errorInfo))
                return false;
            int errorCode = 0;
            IJFDevice dev = Device();

            switch (CellType)
            {
                case JFDevCellType.DI:
                    isOK = true;
                    errorInfo = "Success";
                    break;
                case JFDevCellType.DO:
                    errorInfo = "Success";
                    isOK = true;
                    break;
                case JFDevCellType.Axis:
                    {

                        IJFDevice_MotionDaq devMD = Device() as IJFDevice_MotionDaq;
                        JFDevCellInfo ci = CellInfo();
                        IJFModule_Motion mm = devMD.GetMc(ci.ModuleIndex);
                        errorCode = mm.ServoOn(ci.ChannelIndex);
                        if (errorCode != 0)
                            errorInfo = mm.GetErrorInfo(errorCode);
                        else
                        {
                            isOK = true;
                            errorInfo = "Success";
                        }

                    }
                    break;
                case JFDevCellType.AI:
                    errorInfo = "Success";
                    isOK = true;
                    break;
                case JFDevCellType.AO:
                    errorInfo = "Success";
                    isOK = true;
                    break;
                case JFDevCellType.CmpTrig:
                    errorInfo = "Success";
                    isOK = true;
                    break;
                case JFDevCellType.Light:
                    dev = Device();
                    if(dev is IJFDevice_LightControllerWithTrig) //光源控制器
                    {
                        IJFDevice_LightControllerWithTrig devLT = dev as IJFDevice_LightControllerWithTrig;
                        errorCode = devLT.SetWorkMode(JFLightWithTrigWorkMode.TurnOnOff); //切换为开关模式
                        if(errorCode != 0)
                        {
                            errorInfo = "切换为开关模式失败:" + devLT.GetErrorInfo(errorCode);
                            break;
                        }
                    }
                    IJFDevice_LightController devl = dev as IJFDevice_LightController;
                    errorCode = devl.SetLightChannelEnable(CellInfo().ChannelIndex, true);
                    if(errorCode != 0)
                    {
                        errorInfo = "通道使能失败:" + devl.GetErrorInfo(errorCode);
                        break;
                    }
                    isOK = true;
                    errorInfo = "Success";
                    break;
                case JFDevCellType.Trig:
                    dev = Device();
                    if (dev is IJFDevice_LightControllerWithTrig) //光源控制器
                    {
                        IJFDevice_LightControllerWithTrig devLT = dev as IJFDevice_LightControllerWithTrig;
                        errorCode = devLT.SetWorkMode(JFLightWithTrigWorkMode.Trigger); //切换为触发模式
                        if (errorCode != 0)
                        {
                            errorInfo = "切换为触发模式失败:" + devLT.GetErrorInfo(errorCode);
                            break;
                        }
                    }
                    IJFDevice_TrigController devt = dev as IJFDevice_TrigController;
                    errorCode = devt.SetTrigChannelEnable(CellInfo().ChannelIndex, true);
                    if (errorCode != 0)
                    {
                        errorInfo = "通道使能失败:" + devt.GetErrorInfo(errorCode);
                        break;
                    }
                    isOK = true;
                    errorInfo = "Success";
                    break;
                default:
                    errorInfo = "未定义的通道类型";
                    break;
            }
            return isOK;
        }



        /// <summary>
        /// 检查通道信息是否有效
        /// 建议在打开设备成功后调用
        /// </summary>
        public bool CheckAvalid(out string invalidInfo)
        {
            IJFDevice dev = null;
            JFDevCellInfo ci = null;
            return CheckChannel(CellType, Name, out dev, out ci, out invalidInfo);
        }

        //string _invalidInfo = "";
        ///// <summary>
        ///// 通道无效信息
        ///// </summary>
        //public string InvalidInfo
        //{
        //    get { return _invalidInfo; }
        //}
    }
}
