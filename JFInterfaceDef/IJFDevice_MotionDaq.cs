using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace JFInterfaceDef
{

    /// <summary>
    /// IJFMotionDaqDevice： 运动控制/数据采集（设备）接口 JionFrame's MotionControl&Daq Device
    /// 每个IJFMotionDaqDevice可能连接有多个（运动控制模块）和（数字量采集模块）
    /// </summary>
    public interface IJFDevice_MotionDaq:IJFDevice,IJFRealtimeUIProvider,IJFConfigUIProvider, IDisposable
    {
       

        /// <summary>
        /// 设备上连接的MotionCtrl（运动控制模块）的数量
        /// </summary>
        int McCount { get; }

        /// <summary>
        /// 设备上连接的DIO（数字IO模块）的数量
        /// </summary>
        int DioCount { get; }

        /// <summary>
        /// 设备上连接的AIO（模拟量IO采集模块）的数量
        /// </summary>
        int AioCount { get; }


        int CompareTriggerCount { get; }



        /// <summary>
        /// 获取运动控制器模块
        /// </summary>
        /// <param name="index">序号，从0开始</param>
        /// <returns></returns>
        IJFModule_Motion GetMc(int index);

        /// <summary>
        /// 获取数字IO控制器模块
        /// </summary>
        /// <param name="index">序号，从0开始</param>
        /// <returns></returns>
        IJFModule_DIO GetDio(int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">序号，从0开始</param>
        /// <returns></returns>
        IJFModule_AIO GetAio(int index);

        IJFModule_CmprTrigger GetCompareTrigger(int index);

    }

   


 


}
