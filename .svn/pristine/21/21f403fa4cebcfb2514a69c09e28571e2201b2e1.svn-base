using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;
using JFToolKits;
namespace JFMethodCommonLib
{
    public class JFCMFunction
    {
        ///设备模块类别
        public static string DO = "DO";
        public static string DI = "DI";
        public static string AO = "AO";
        public static string AI = "AI";
        public static string Axis = "Axis";
        public static string CmpTrig = "CmpTrig";//位置比较触发
        public static string LightCtrl = "LightCtrl"; // 开关式光源控制器通道
        public static string LightTrig = "LightTrig"; //触发式光源控制器通道
        public static string Cmr = "Camera";

        /// <summary>
        /// 检查设备通道是否存在并且可用
        /// </summary>
        /// <param name="category"></param>
        /// <param name="cellName"></param>
        /// <param name="initor"></param>
        /// <param name="cellInfo"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public static bool CheckDevCellName(string category,string cellName,out IJFInitializable initor,out JFDevCellInfo ci,out string errorInfo)
        {
            initor = null;
            ci = null;
            errorInfo = null;

            if(string.IsNullOrEmpty(cellName))
            {
                errorInfo = "参数项\"cellName\"为空";
                return false;
            }
            JFDevCellNameManeger nameMgr = JFHubCenter.Instance.MDCellNameMgr;
            JFInitorManager initorMgr = JFHubCenter.Instance.InitorManager;
            JFDevCellInfo cellInfo = null;
            if (category == DO) //获取数字量输出通道信息
            {
                cellInfo = nameMgr.GetDoCellInfo(cellName);
                if(null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在DO：" + cellName;
                    return false;
                }

                IJFInitializable dev = initorMgr.GetInitor(cellInfo.DeviceID);
                if (null == dev)
                {
                    errorInfo = "DO:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if(!typeof(IJFDevice_MotionDaq).IsAssignableFrom( dev.GetType()))
                {
                    errorInfo = "DO:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是Device_MotionDaq ";
                    return false;
                }

                IJFDevice_MotionDaq md = dev as IJFDevice_MotionDaq;
                if(!md.IsInitOK)
                {
                    errorInfo = "DO:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if(!md.IsDeviceOpen)
                {
                    errorInfo = "DO:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if(cellInfo.ModuleIndex >= md.DioCount)
                {
                    errorInfo = "DO:\"" + cellName + "\" ModuleIndex = :" + cellInfo.ModuleIndex + "超出设备DIO模块数量: " + md.DioCount;
                    return false;
                }

                if(cellInfo.ChannelIndex >= md.GetDio(cellInfo.ModuleIndex).DOCount)
                {
                    errorInfo = "DO:\"" + cellName + "\" Channel = :" + cellInfo.ChannelIndex + "超出模块DO通道数量: " + md.GetDio(cellInfo.ModuleIndex).DOCount;
                    return false;
                }
                initor = dev;
                ci = cellInfo;
                errorInfo = "Success";
                return true;

            }
            else if(category == DI) //获取数字量输入
            {
                cellInfo = nameMgr.GetDiCellInfo(cellName);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在DI：" + cellName;
                    return false;
                }

                IJFInitializable dev = initorMgr.GetInitor(cellInfo.DeviceID);
                if (null == dev)
                {
                    errorInfo = "DI:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_MotionDaq).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "DI:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是Device_MotionDaq ";
                    return false;
                }

                IJFDevice_MotionDaq md = dev as IJFDevice_MotionDaq;
                if (!md.IsInitOK)
                {
                    errorInfo = "DI:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "DI:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ModuleIndex >= md.DioCount)
                {
                    errorInfo = "DI:\"" + cellName + "\" ModuleIndex = :" + cellInfo.ModuleIndex + "超出设备DIO模块数量: " + md.DioCount;
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.GetDio(cellInfo.ModuleIndex).DICount)
                {
                    errorInfo = "DI:\"" + cellName + "\" Channel = :" + cellInfo.ChannelIndex + "超出模块DI通道数量: " + md.GetDio(cellInfo.ModuleIndex).DOCount;
                    return false;
                }
                initor = dev;
                ci = cellInfo;
                errorInfo = "Success";
                return true;
            }
            else if (category == AI) 
            {

            }
            else if (category == AO) 
            {

            }
            else if (category == Axis)
            {
                cellInfo = nameMgr.GetAxisCellInfo(cellName);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在Axis：" + cellName;
                    return false;
                }

                IJFInitializable dev = initorMgr.GetInitor(cellInfo.DeviceID);
                if (null == dev)
                {
                    errorInfo = "Axis:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_MotionDaq).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "Axis:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是Device_MotionDaq ";
                    return false;
                }

                IJFDevice_MotionDaq md = dev as IJFDevice_MotionDaq;
                if (!md.IsInitOK)
                {
                    errorInfo = "Axis:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "Axis:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ModuleIndex >= md.McCount)
                {
                    errorInfo = "Axis:\"" + cellName + "\" ModuleIndex = :" + cellInfo.ModuleIndex + "超出设备轴模块数量: " + md.McCount;
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.GetMc(cellInfo.ModuleIndex).AxisCount)
                {
                    errorInfo = "Axis:\"" + cellName + "\" Channel = :" + cellInfo.ModuleIndex + "超出模块轴通道数量: " + md.GetMc(cellInfo.ModuleIndex).AxisCount;
                    return false;
                }
                initor = dev;
                ci = cellInfo;
                errorInfo = "Success";
                return true;
            }
            else if (category == CmpTrig) 
            {
                cellInfo = nameMgr.GetCmpTrigCellInfo(cellName);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在CmpTrig：" + cellName;
                    return false;
                }

                IJFInitializable dev = initorMgr.GetInitor(cellInfo.DeviceID);
                if (null == dev)
                {
                    errorInfo = "CmpTrig:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_MotionDaq).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "CmpTrig:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是Device_MotionDaq ";
                    return false;
                }

                IJFDevice_MotionDaq md = dev as IJFDevice_MotionDaq;
                if (!md.IsInitOK)
                {
                    errorInfo = "CmpTrig:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "CmpTrig:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ModuleIndex >= md.CompareTriggerCount)
                {
                    errorInfo = "CmpTrig:\"" + cellName + "\" ModuleIndex = :" + cellInfo.ModuleIndex + "超出设备比较触发模块数量: " + md.CompareTriggerCount;
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.GetCompareTrigger(cellInfo.ModuleIndex).EncoderChannels)
                {
                    errorInfo = "CmpTrig:\"" + cellName + "\" Channel = :" + cellInfo.ModuleIndex + "超出模块比较触发通道数量: " + md.GetCompareTrigger(cellInfo.ModuleIndex).EncoderChannels;
                    return false;
                }
                initor = dev;
                ci = cellInfo;
                errorInfo = "Success";
                return true;
            }
            else if (category == LightCtrl)
            {
                cellInfo = nameMgr.GetLightCtrlChannelInfo(cellName);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在LightCtrl：" + cellName;
                    return false;
                }

                IJFInitializable dev = initorMgr.GetInitor(cellInfo.DeviceID);
                if (null == dev)
                {
                    errorInfo = "LightCtrl:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_LightController).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "LightCtrl:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是IJFDevice_LightController ";
                    return false;
                }

                IJFDevice_LightController md = dev as IJFDevice_LightController;
                if (!md.IsInitOK)
                {
                    errorInfo = "LightCtrl:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "LightCtrl:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.LightChannelCount)
                {
                    errorInfo = "LightCtrl:\"" + cellName + "\" Channel = :" + cellInfo.ChannelIndex + "超出设备光源通道数量: " + md.LightChannelCount;
                    return false;
                }
                initor = dev;
                ci = cellInfo;
                errorInfo = "Success";
                return true;
            }
            else if (category == LightTrig)
            {
                cellInfo = nameMgr.GetTrigCtrlChannelInfo(cellName);
                if (null == cellInfo)
                {
                    errorInfo = "设备命名表中不存在LightTrig：" + cellName;
                    return false;
                }

                IJFInitializable dev = initorMgr.GetInitor(cellInfo.DeviceID);
                if (null == dev)
                {
                    errorInfo = "LightTrig:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_TrigController).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "LightTrig:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"类型不是IJFDevice_TrigController ";
                    return false;
                }

                IJFDevice_TrigController md = dev as IJFDevice_TrigController;
                if (!md.IsInitOK)
                {
                    errorInfo = "LightTrig:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未完成初始化动作 ";
                    return false;
                }

                if (!md.IsDeviceOpen)
                {
                    errorInfo = "LightTrig:\"" + cellName + "\" 所属设备:\"" + cellInfo.DeviceID + "\"未打开 ";
                    return false;
                }

                if (cellInfo.ChannelIndex >= md.TrigChannelCount)
                {
                    errorInfo = "LightTrig:\"" + cellName + "\" Channel = :" + cellInfo.ChannelIndex + "超出设备触发通道数量: " + md.TrigChannelCount;
                    return false;
                }
                initor = dev;
                ci = cellInfo;
                errorInfo = "Success";
                return true;
            }
            else if (category == Cmr)
            {
               

                IJFInitializable dev = initorMgr.GetInitor(cellName);
                if (null == dev)
                {
                    errorInfo = "Camera:\"" + cellName  +  "\" 在设备列表中不存在";
                    return false;
                }

                if (!typeof(IJFDevice_Camera).IsAssignableFrom(dev.GetType()))
                {
                    errorInfo = "Camera:\"" + cellName + "\" 设备类型不是IJFDevice_Camera ";
                    return false;
                }
                initor = dev;
                errorInfo = "Success";
                return true;
            }
            else
            {
                errorInfo = "不支持的参数项\"category\" = " + category;
            }
            return false;
        }
    }
}
