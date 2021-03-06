using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using JFHub;
using System.Threading;

namespace JFMethodCommonLib
{
    /// <summary>
    /// 等待轴运动完成
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 设备控制", "Axis" })]
    [JFDisplayName("等待轴运动完成_D")]
    public class JFMC_WaitSingleAxisMD_S : JFMethodInitParamBase,IJFMethod_T
    {
        public JFMC_WaitSingleAxisMD_S()
        {
            DeclearInitParam("轴名称", typeof(string), "");
            DeclearInitParam("等待超时毫秒", typeof(int), -1);
        }

        public override bool Initialize()
        {
            string axisID = GetInitParamValue("轴名称") as string;
            if(string.IsNullOrEmpty(axisID))
            {
                ActionErrorInfo = "初始化参数:\"轴名称\"未设置";
                return false;
            }
            ActionErrorInfo = "Success";
            return true;
        }

        TWAxisMD _thread = new TWAxisMD();

        protected override bool ActionGenuine()
        {
            if(!IsInitOK)
            {
                ActionErrorInfo = "初始化未完成:" + InitErrorInfo;
                return false;
            }
            string axisID = GetInitParamValue("轴名称") as string;
            int timeoutMilSeconds = (int)GetInitParamValue("等待超时毫秒");
            JFDevChannel axisChn = new JFDevChannel(JFDevCellType.Axis, axisID);
            string avalidInfo;
            if(!axisChn.CheckAvalid(out avalidInfo))
            {
                ActionErrorInfo = avalidInfo;
                return false;
            }
            if(!_thread.SetAxis(axisChn))
            {
                ActionErrorInfo = _thread.ActionErrorInfo;
                return false;
            }
            _thread.TimeoutMilliSeconds = timeoutMilSeconds;
            if (JFWorkCmdResult.Success != _thread.Start())
            {
                ActionErrorInfo = _thread.ActionErrorInfo;
                return false;
            }

            while(true)
            {
                JFWorkStatus status = _thread.CurrWorkStatus;
                if (status == JFWorkStatus.NormalEnd)
                {
                    ActionErrorInfo = "Success";
                    return true;
                }
                if(status== JFWorkStatus.CommandExit || status == JFWorkStatus.AbortExit)
                {
                    ActionErrorInfo = "收到退出指令";
                    return false;
                }
                if(status == JFWorkStatus.ErrorExit)
                {
                    ActionErrorInfo = _thread.ActionErrorInfo;
                    return false;
                }
                 if(status == JFWorkStatus.ExceptionExit)
                {
                    ActionErrorInfo = "线程内部异常！";
                    return false;
                }
                Thread.Sleep(100);
            }

            
        }


        public void Pause()
        {
            _thread.Pause();
        }

        public void Resume()
        {
            _thread.Resume();
        }

        public void Exit()
        {
            _thread.Stop();
        }


    }


    /// <summary>
    /// 等待轴运动完成的线程类
    /// </summary>
    class TWAxisMD : JFCmdWorkBase
    {
        public TWAxisMD()
        {
            ActionErrorInfo = "No-Option";
            TimeoutMilliSeconds = -1;
        }

        JFDevChannel _axisChn = null;
        public bool SetAxis(JFDevChannel axisChn)
        {
            if (CurrWorkStatus == JFWorkStatus.Running || CurrWorkStatus == JFWorkStatus.Pausing)
            {
                ActionErrorInfo = "内部线程当前状态:" + CurrWorkStatus;
                return false;
            }
            _axisChn = axisChn;
            return true;
        }

        public int TimeoutMilliSeconds { get; set; }


        public override int[] AllCmds { get{ return null; } }

        public override int[] AllCustomStatus { get { return null; } }

        public override string GetCmdName(int cmd)
        {
            return null;
        }

        public override string GetCustomStatusName(int status)
        {
            return null;
        }

        public string ActionErrorInfo { get; private set; }

        public override JFWorkCmdResult Start()
        {
            if (null == _axisChn)
            {
                ActionErrorInfo = "轴未设置";
                return JFWorkCmdResult.UnknownError;
            }
            string err;
            if(!_axisChn.CheckAvalid(out err))
            {
                ActionErrorInfo = err;
                return JFWorkCmdResult.UnknownError;
            }
            _dev = _axisChn.Device() as IJFDevice_MotionDaq;
            _md = _dev.GetMc(_ci.ModuleIndex);
            _ci = _axisChn.CellInfo();
            JFWorkCmdResult ret = base.Start();
            if (ret != JFWorkCmdResult.Success)
                ActionErrorInfo = "启动内部线程返回ErrorCode：" + ret;
            return ret;

        }

        IJFDevice_MotionDaq _dev = null;
        IJFModule_Motion _md = null;
        JFDevCellInfo _ci = null;
        DateTime _startTime = DateTime.Now;

        protected override void CleanupWhenWorkExit()
        {
            _md.StopAxis(_ci.ChannelIndex);
        }

        protected override void OnPause()
        {
            _md.StopAxis(_ci.ChannelIndex);

        }

        protected override void OnResume()
        {
            _md.Home(_ci.ChannelIndex);
        }

        protected override void OnStop()
        {
            _md.StopAxis(_ci.ChannelIndex);
        }

        protected override void PrepareWhenWorkStart()
        {
            _startTime = DateTime.Now;
        }

        protected override void RunLoopInWork()
        {
            int errCode = 0;
            //bool isHomeDone = false;
            //errCode = _md.IsHomeDone(_ci.ChannelIndex, out isHomeDone);
            //if (errCode != 0)
            //{
            //    ActionErrorInfo = "获取HomeDone状态失败!" + _md.GetErrorInfo(errCode);
            //    ExitWork(WorkExitCode.Error, "获取HomeDone 状态失败!");
            //}
            //if (isHomeDone)
            //{
            //    ActionErrorInfo = "Success";
            //    ExitWork(WorkExitCode.Normal, "Success");
            //}
            // 其他状态检查
            bool[] axisStatus;
            errCode = _md.GetMotionStatus(_ci.ChannelIndex, out axisStatus);
            if(errCode != 0)
            {
                ActionErrorInfo = "获取轴状态失败!" + _md.GetErrorInfo(errCode);
                ExitWork(WorkExitCode.Error, "获取轴状态失败!");
            }
            if(axisStatus[_md.MSID_ALM])
            {
                ActionErrorInfo = "轴已报警!";
                ExitWork(WorkExitCode.Error, "轴已报警!");
            }

            if (axisStatus[_md.MSID_EMG])
            {
                ActionErrorInfo = "轴已急停!";
                ExitWork(WorkExitCode.Error, "轴已急停!");
            }

            if(axisStatus[_md.MSID_SVO])
            {
                ActionErrorInfo = "轴伺服已断电!";
                ExitWork(WorkExitCode.Error, "轴伺服已断电!");
            }

            if(axisStatus[_md.MSID_MDN]) //等待归零完成，这一句去掉
            {
                ActionErrorInfo = "Success";
                ExitWork(WorkExitCode.Normal, "Success");
            }

            if(TimeoutMilliSeconds >=0)
            {
                TimeSpan ts = DateTime.Now - _startTime;
                if(ts.TotalMilliseconds >= TimeoutMilliSeconds)
                {
                    ActionErrorInfo = "超时";
                    ExitWork(WorkExitCode.Error, "超时");
                }
            }

        }
    }


}
