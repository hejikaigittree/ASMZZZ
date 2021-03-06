using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JFInterfaceDef;

namespace JFHub
{
    /// <summary>
    /// JFMethodFlowBox： 提供方法流的容器，可以执行同步/异步运行
    /// </summary>
    //public class JFMethodFlowBox:JFCmdWorkBase
    //{
    //    public JFMethodFlowBox()
    //    {
    //        Name = "";
    //        ActionErrorInfo = "Inner methodflow is unsetted";
    //        RunLoops = 1;
    //    }

    //    object _accessLocker = new object();
    //    JFMethodFlow _methodFlow = null;
    //    //bool _isSyncRunning = false; //当前是否处于同步运行模式
        
        
    //    public bool SetMethodFlow(JFMethodFlow mf)
    //    {
    //        lock (_accessLocker)
    //        {
    //            if (CurrWorkStatus == JFWorkStatus.Running || CurrWorkStatus == JFWorkStatus.Pausing || CurrWorkStatus == JFWorkStatus.Interactiving )
    //                return false;
    //            _methodFlow = mf;
    //            _methodFlow.SetWorkCmdChecker(this);
    //            ChangeWorkStatus(JFWorkStatus.UnStart);//CurrWorkStatus = JFWorkStatus.UnStart;
    //            _actionErrorInfo = "No-Ops";
    //            return true;
    //        }
    //    }

    //    public JFMethodFlow MethodFlow { get { return _methodFlow; } }

    //    string _actionErrorInfo = "No-Ops";
        
    //    public string ActionErrorInfo
    //    {
    //        get { return _actionErrorInfo; }
    //        private set
    //        {
    //            lock(_accessLocker)
    //            {
    //                _actionErrorInfo = value;
    //            }
    //        }
            
    //    }

    //    public override int[] AllCmds { get { return new int[] {1 }; } } //
    //    public override string GetCmdName(int cmd) 
    //    {
    //        if (1 == cmd)
    //            return "单步执行";
    //        return "未知指令";
    //    }


    //    public override int[] AllCustomStatus
    //    {
    //        get
    //        {
    //            List<int> ret = new List<int>();
    //            ret.Add(-1);
    //            if(null != _methodFlow)
    //            {
    //                int steps= _methodFlow.Count;
    //                for (int i = 0; i < steps+1; i++)
    //                    ret.Add(i);
    //            }
    //            return ret.ToArray();
    //        }
    //    }

    //    public override string GetCustomStatusName(int status)
    //    {
    //        if (null == _methodFlow || status < -1 || status > _methodFlow.Count)
    //            return "Unknown-status";
    //        if (status == -1)
    //            return "未运行";
    //        else if (status == _methodFlow.Count)
    //            return "已结束";
    //        else 
    //            return "当前步:" + status + " 方法名:" + _methodFlow.AllMethodNames[status];
    //    }
    //    /// <summary>
    //    /// 异步运行循环次数 ， 如果RunLoops < 1 表示无限循环，直到接收到用户的退出指令
    //    /// </summary>
    //    public int RunLoops
    //    {
    //        get;set;
    //    }

    //    internal enum RunMode
    //    {
    //        Normal = 0, //正常运行
    //        StepByStep, //单步调试模式
    //    }

    //    RunMode _runMode = RunMode.Normal;
    //    /// <summary>
    //    /// 以RunLoops 指定的次数循环运行
    //    /// </summary>
    //    /// <returns></returns>
    //    public override JFWorkCmdResult Start()
    //    {
    //        if (_methodFlow == null)
    //        {
    //            ActionErrorInfo = "方法流对象未设置/空值";
    //            return JFWorkCmdResult.UnknownError;
    //        }
    //        JFWorkStatus currStatus = CurrWorkStatus;
    //        if(currStatus == JFWorkStatus.Running ||
    //            currStatus == JFWorkStatus.Pausing ||
    //            currStatus == JFWorkStatus.Interactiving)
    //        {
    //            ActionErrorInfo = "启动工作流失败,当前状态:" + currStatus;
    //            return JFWorkCmdResult.StatusError;
    //        }
    //        _runMode = RunMode.Normal;
    //        JFWorkCmdResult ret= base.Start();
    //        if (ret != JFWorkCmdResult.Success)
    //            ActionErrorInfo = "启动工作流失败:" + ret;
    //        return ret;
    //    }

    //    /// <summary>
    //    /// 同步运行方法流( 一次)
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool Action()
    //    {
    //        Monitor.Enter(_accessLocker);
    //        if (null == _methodFlow)
    //        {
    //            _actionErrorInfo = "Action Failed by: Inner methodflow is null";
    //            Monitor.Exit(_accessLocker);
    //            return false;
    //        }
    //        if (CurrWorkStatus == JFWorkStatus.Running || CurrWorkStatus == JFWorkStatus.Pausing)
    //        {
    //            _actionErrorInfo = "Action Failed by: Current Work status  = " + CurrWorkStatus.ToString();
    //            Monitor.Exit(_accessLocker);
    //            return false;
    //        }
    //        ChangeWorkStatus(JFWorkStatus.Running);//CurrWorkStatus = JFWorkStatus.Running;
    //        //_isSyncRunning = true;
    //        _actionErrorInfo = "Action is running";
    //        _methodFlow.Reset();
    //        Monitor.Exit(_accessLocker);
    //        bool isOK = _methodFlow.Action();
    //        Monitor.Enter(_accessLocker);
    //        if (isOK)
    //        {
    //            Monitor.Exit(_accessLocker);
    //            _actionErrorInfo = "Success";
    //            ChangeWorkStatus(JFWorkStatus.NormalEnd);//CurrWorkStatus = JFWorkStatus.NormalEnd;
    //            return true; 
    //        }

    //        _actionErrorInfo = _methodFlow.ActionErrorInfo();
    //        ChangeWorkStatus(JFWorkStatus.ErrorExit);//CurrWorkStatus = JFWorkStatus.ErrorExit;
    //        Monitor.Exit(_accessLocker);
    //        return false;

    //    }

    //    int currLoops = 0; //当前循环次数
    //    protected override void PrepareWhenWorkStart()
    //    {
    //        _methodFlow.Reset();
    //        currLoops = 0;
    //        if (_runMode == RunMode.StepByStep)
    //        {
    //            int cmd = 0;
    //            while(true)
    //            {
    //                if (!WaitCmd(out cmd))
    //                    continue;
    //                if (!AllCmds.Contains(cmd))
    //                    RespCmd(JFWorkCmdResult.IllegalCmd);
    //                else
    //                {
    //                    if (AllCmds[0] == cmd)
    //                    {
    //                        RespCmd(JFWorkCmdResult.Success);
    //                        return; //单步执行下一条动作
    //                    }
    //                    else
    //                        RespCmd(JFWorkCmdResult.IllegalCmd);
    //                }
    //            }

    //        }
    //        else //正常运行模式下，不接受用户自定义指令
    //            RespCmd(JFWorkCmdResult.IllegalCmd);

    //    }

    //    protected override void RunLoopInWork()
    //    {
    //        if (_runMode == RunMode.Normal)
    //        {
    //            //if (!_methodFlow.Action())
    //            //{
    //            //    _actionErrorInfo = _methodFlow.ActionErrorInfo();
    //            //    ExitWork(WorkExitCode.Error, _actionErrorInfo);
    //            //}
    //            for(int i = 0; i < _methodFlow.Count;i++)
    //            {
    //                ChangeCustomStatus(i);
    //                if(!_methodFlow.Step())
    //                    ExitWork(WorkExitCode.Error, "Step = " + i + " Error:" + _methodFlow.ActionErrorInfo());
    //                CheckCmd(CycleMilliseconds);
    //            }
                
    //            if (RunLoops > 0)
    //            {
    //                if (++currLoops >= RunLoops)
    //                    ExitWork(WorkExitCode.Normal, "Completed");
    //                else
    //                    _methodFlow.Reset();
    //            }
    //        }
    //        else
    //        {
    //            if(_methodFlow.CurrStep >= _methodFlow.Count)
    //                ExitWork(WorkExitCode.Normal, "Completed，已运行到最后一步 StepIndex = " + _methodFlow.CurrStep);

    //            ChangeCustomStatus(_methodFlow.CurrStep + 1);
    //            bool isOK = _methodFlow.Step();
    //            if(!isOK)
    //            {
    //                SendMsg2Outter("运行出错，当前步骤:" + _methodFlow.CurrStep + "\" 错误信息:" + _methodFlow.ActionErrorInfo());
    //                ExitWork(WorkExitCode.Error, _methodFlow.ActionErrorInfo());
    //            }
    //            SendMsg2Outter("当前步骤:" + _methodFlow.CurrStep + "执行成功！");
    //            int cmd = 0;
    //            while (true)
    //            {
    //                if (!WaitCmd(out cmd))
    //                    continue;
    //                if (!AllCmds.Contains(cmd))
    //                    RespCmd(JFWorkCmdResult.IllegalCmd);
    //                else
    //                {
    //                    if (AllCmds[0] == cmd)
    //                    {
    //                        RespCmd(JFWorkCmdResult.Success);
    //                        return; //单步执行下一条动作
    //                    }
    //                    else
    //                        RespCmd(JFWorkCmdResult.IllegalCmd);
    //                }
    //            }
    //        }
            
            
            
    //    }

    //    protected override void CleanupWhenWorkExit()
    //    {
    //        return;
    //    }

    //    protected override void OnPause()
    //    {
    //        //if (typeof(IJFMethod_T).IsAssignableFrom(_methodFlow.GetItem(_methodFlow.CurrStep).Value.GetType()))
    //        //    (_methodFlow.GetItem(_methodFlow.CurrStep).Value as IJFMethod_T).Pause();
    //        _methodFlow.Pause();
    //    }

    //    protected override void OnResume()
    //    {
    //        //if (typeof(IJFMethod_T).IsAssignableFrom(_methodFlow.GetItem(_methodFlow.CurrStep).Value.GetType()))
    //        //    (_methodFlow.GetItem(_methodFlow.CurrStep).Value as IJFMethod_T).Resume();
    //        _methodFlow.Resume();
    //    }

    //    protected override void OnStop()
    //    {
    //        //if (typeof(IJFMethod_T).IsAssignableFrom(_methodFlow.GetItem(_methodFlow.CurrStep).Value.GetType()))
    //        //    (_methodFlow.GetItem(_methodFlow.CurrStep).Value as IJFMethod_T).Exit();
    //        _methodFlow.Exit();
    //    }

        

    //    //提供异步的单步调试功能(供UcMethodFlow调用)
    //    internal JFWorkCmdResult StartStepByStep()
    //    {
    //        if (_methodFlow == null)
    //        {
    //            ActionErrorInfo = "方法流对象未设置/空值";
    //            return JFWorkCmdResult.UnknownError;
    //        }
    //        JFWorkStatus currStatus = CurrWorkStatus;
    //        if (currStatus == JFWorkStatus.Running ||
    //            currStatus == JFWorkStatus.Pausing ||
    //            currStatus == JFWorkStatus.Interactiving)
    //        {
    //            ActionErrorInfo = "启动工作流失败,当前状态:" + currStatus;
    //            return JFWorkCmdResult.StatusError;
    //        }
    //        _runMode = RunMode.StepByStep;
    //        JFWorkCmdResult ret = base.Start();
    //        if (ret != JFWorkCmdResult.Success)
    //            ActionErrorInfo = "启动工作流失败:" + ret;
    //        return ret;
    //    }

    //    internal JFMethodFlowBox.RunMode WorkMode { get { return _runMode; } }





    //}
}
