using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JFHub
{
    /// <summary>
    /// 规则工站 ， 主要用于运行一些不需要编程的任务
    /// </summary>
    [JFDisplayName("JF规则工站")]
    [JFVersion("1.0.0.0")]
    public class JFRuleStation:JFStationBase
    {
        static string ResetWFName = "归零动作流程";
        static string PrepareWFName = "任务开始前准备流程";
        static string RunLoopWFName = "主工作流程";
        static string EndBatchWFName = "结批流程";
        static string CleanWFName = "任务退出前清理流程";

        public JFRuleStation()
        {
            DeclearMethodFlow(ResetWFName); //归零动作流程
            DeclearMethodFlow(PrepareWFName);//主流程运行前的准备
            DeclearMethodFlow(RunLoopWFName);//主流程
            DeclearMethodFlow(EndBatchWFName);//结批流程
            DeclearMethodFlow(CleanWFName);//退出主流程后的清理
         }


     
        /// <summary>
        /// 暂时不接受自定义指令
        /// 日后添加
        /// </summary>
        public override int[] AllCmds { get { return new int[] { }; } }
        public override string GetCmdName(int cmd)
        {
            throw new NotImplementedException();
        }

        static int CSID_Stop = 0;
        static int CSID_Resetting = 1;
        static int CSID_Preparing = 2;
        static int CSID_Running = 3;
        static int CSID_EndBatching = 4;
        static int[] _customStatus = new int[5] { CSID_Stop, CSID_Resetting, CSID_Preparing, CSID_Running, CSID_EndBatching };
        static string[] CustomStatusNames = new string[] { "已停止", "正在归零","正在准备", "正在运行", "正在结批" };
        /// <summary>
        /// 自定义状态 0,1,2,3
        /// </summary>
        public override int[] AllCustomStatus { get { return _customStatus; } }

        JFStationRunMode _runMode = JFStationRunMode.Auto;
        public override JFStationRunMode RunMode { get { return _runMode; } }

        public override string GetCustomStatusName(int status)
        {
            if (status > -1 && status < 5)
                return CustomStatusNames[status];
            return "未定义状态";
        }






        protected override void ExecuteEndBatch()
        {
            string errInfo;
            ChangeCustomStatus(CSID_EndBatching);
            if (!SynchRunWorkFlow(EndBatchWFName, out errInfo))
            {
                ChangeCustomStatus(CSID_Stop);
                JFHubCenter.Instance.StationMgr.OnStationLog(this, EndBatchWFName + "发生错误:" + errInfo, JFLogLevel.FATAL, LogMode.Record);
                ExitWork(WorkExitCode.Error, EndBatchWFName + "发生错误:" + errInfo);
            }
        }

        protected override void OnPause() //此函数调用时已在工作线程中
        {
            string errorInfo;
            if (!PauseAllAsyncWorkFlow(out errorInfo))
            {
                ChangeCustomStatus(CSID_Stop);
                JFHubCenter.Instance.StationMgr.OnStationLog(this, "工站暂停运行失败->" + errorInfo, JFLogLevel.FATAL, LogMode.Record);
                ExitWork(WorkExitCode.Error, "工站暂停运行失败->" + errorInfo);
            }
        }

        protected override void OnResume()
        {
            string errorInfo;
            if (!ResumeAllAsyncWorkFlow(out errorInfo))
            {
                ChangeCustomStatus(CSID_Stop);
                JFHubCenter.Instance.StationMgr.OnStationLog(this, "工站恢复运行失败->" + errorInfo, JFLogLevel.FATAL, LogMode.Record);
                ExitWork(WorkExitCode.Error, "工站恢复运行失败->" + errorInfo);
            }
        }

        protected override void OnStop()
        {
            ChangeCustomStatus(CSID_Stop);
            StopAllAsyncWorkFlow();

        }


        protected override void PrepareWhenWorkStart()
        {
            
                ChangeCustomStatus(CSID_Preparing);
                string errInfo;
                if (!SynchRunWorkFlow(PrepareWFName, out errInfo))
                {
                    ChangeCustomStatus(CSID_Stop);
                    JFHubCenter.Instance.StationMgr.OnStationLog(this, PrepareWFName + "发生错误:" + errInfo, JFLogLevel.FATAL, LogMode.ShowRecord);
                    ExitWork(WorkExitCode.Error, PrepareWFName + "发生错误:" + errInfo);
                }
                ChangeCustomStatus(CSID_Running);


        }

        

        protected override void RunLoopInWork()
        {
            string errInfo;

            //正常工作流程
            if(!SynchRunWorkFlow(RunLoopWFName, out errInfo))
            {
                ChangeCustomStatus(CSID_Stop);
                JFHubCenter.Instance.StationMgr.OnStationLog(this, RunLoopWFName + "发生错误:" + errInfo, JFLogLevel.FATAL, LogMode.ShowRecord);
                ExitWork(WorkExitCode.Error, RunLoopWFName + "发生错误:" + errInfo);
            }
        }

        protected override void CleanupWhenWorkExit()
        {
            string errInfo;
            if (!SynchRunWorkFlow(CleanWFName, out errInfo))
                JFHubCenter.Instance.StationMgr.OnStationLog(this, CleanWFName + "发生错误:" + errInfo, JFLogLevel.FATAL, LogMode.ShowRecord);
            ChangeCustomStatus(CSID_Stop);
        }

        protected override void ExecuteReset()
        {
            string errInfo;
            ChangeCustomStatus(CSID_Resetting);
            if (!SynchRunWorkFlow(ResetWFName, out errInfo))
            {
                ChangeCustomStatus(CSID_Stop);
                JFHubCenter.Instance.StationMgr.OnStationLog(this, ResetWFName + "发生错误:" + errInfo, JFLogLevel.FATAL, LogMode.ShowRecord);
                ExitWork(WorkExitCode.Error, ResetWFName + "发生错误:" + errInfo);
            }
        }

        public override bool SetRunMode(JFStationRunMode runMode)
        {
            _runMode = runMode;
            return true;
        }
    }
}
