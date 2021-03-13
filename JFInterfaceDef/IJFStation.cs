using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFInterfaceDef
{

    public enum JFStationRunMode
    {
        Auto,   //自动(连续)运行
        Manual, //手动(单站)运行
    }

    /// <summary>
    /// 工站接口类
    /// </summary>
    public interface IJFStation : IJFCmdWork,IJFInitializable,IJFRealtimeUIProvider,IJFConfigUIProvider
    {
        JFStationRunMode RunMode { get; }
        bool SetRunMode(JFStationRunMode runMode);
        

        /// <summary>
        /// 生成本工站测试窗口，（可为空值） 
        /// 每调用一次本函数，将生成一个新的窗口对象
        /// </summary>
        Form GenForm();


        /// <summary>
        /// 向工站发送复位指令
        /// </summary>
        /// <returns></returns>
        JFWorkCmdResult Reset();

        /// <summary>
        /// 向工站发送结批指令
        /// </summary>
        /// <param name="milliSeconds"></param>
        /// <returns></returns>
        JFWorkCmdResult EndBatch(int milliSeconds = -1);//结批操作
    }
}
