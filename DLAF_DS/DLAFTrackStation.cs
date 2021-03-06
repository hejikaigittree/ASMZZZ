using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using JFUI;
using JFVision;
using JFHub;

namespace DLAF_DS
{
    /// <summary>
    /// 直线AOI设备的轨道工站
    /// </summary>
    public class DLAFTrackStation:JFStationBase
    {
        /// <summary>
        /// 所有可接收的自定义指令ID
        /// </summary>
        public override int[] AllCmds
        {
            get { return new int[] { }; }
        }

        /// <summary>
        /// 获取自定义指令名称
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override string GetCmdName(int cmd)
        {
            return "Undefined";
        }

        /// <summary>
        /// 所有自定义状态ID
        /// </summary>
        public override int[] AllCustomStatus
        {
            get { return new int[] { }; }
        }

        /// <summary>
        /// 获取自定义状态名称
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public override string GetCustomStatusName(int status)
        {
            return "Undefined";
        }







        /// <summary>
        /// 工站主逻辑开始前的准备工作，只是在主流程开始前执行一次
        /// </summary>
        protected override void PrepareWhenWorkStart()
        {
            return;
        }

        /// <summary>
        /// 工站主流程 ，将会在内部循环中不断被调用，直到收到暂停/退出指令，或在RunLoopInWork函数内部调用退出函数时结束
        /// </summary>
        protected override void RunLoopInWork()
        {
            return;
        }


        /// <summary>
        /// 工站在退出主流程后会执行一次，然后工作线程关闭
        /// </summary>
        protected override void CleanupWhenWorkExit()
        {
            return;
        }

        /// <summary>
        /// 执行结批动作
        /// </summary>
        protected override void ExecuteEndBatch()
        {
            return;
        }

        /// <summary>
        /// 当工站线程运行时，受到暂停指令会执行OnPause(),然后进入暂停状态
        /// </summary>
        protected override void OnPause()
        {
            return;
        }

        /// <summary>
        /// 当工站线程运行并且当前状态处于暂停时，受到恢复运行指令会执行OnResume()
        /// </summary>
        protected override void OnResume()
        {
            return;
        }

        /// <summary>
        /// 当前工站正在运行（包括暂停状态）收到退出指令会执行OnStop()
        /// </summary>
        protected override void OnStop()
        {
            return;
        }


        /// <summary>
        /// 执行工站归零动作
        /// </summary>
        protected override void ExecuteReset()
        {
            
        }
    }
}
