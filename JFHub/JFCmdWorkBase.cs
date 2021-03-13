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
    /// 工作线程向外界发送消息的代理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    public delegate void WorkMsgInfo(object sender, string msg);

    /// <summary>
    /// 运行中可接收用户指令的线程类
    /// </summary>
    public abstract class JFCmdWorkBase:IJFCmdWork//,IJFWorkCmdChecker
    {
        public JFCmdWorkBase()
        {
            cmdEvent = new ManualResetEvent(false);
            rspEvent = new AutoResetEvent(false);
            command = CommandUnknown;
            cmdResult = JFWorkCmdResult.UnknownError;
            accessLocker = new object();
            thread = new Thread(ThreadFunc);
            CurrWorkStatus = JFWorkStatus.UnStart;
            CycleMilliseconds = 10;
        }
        public event WorkStatusChange WorkStatusChanged;
        public event CustomStatusChange CustomStatusChanged;
        public event WorkMsgInfo WorkMsg2Outter;



        public virtual string Name { get; set; }





        protected object WorkStatusLocker = new object();
        public JFWorkStatus CurrWorkStatus { get; protected set; }

        /// <summary>线程内部轮询周期</summary>
        public int CycleMilliseconds { get; set; }

        public bool SetWorkThreadParam(object param)
        {
            lock (WorkStatusLocker)
            {
            if (IsWorking())
                return false;
            threadParam = param;
            return true;
            }
        }

        /// <summary>
        /// 供继承类调用，获取线程运行所需的参数
        /// </summary>
        /// <returns></returns>
        protected object GetWorkThreadParam()
        {
            return threadParam;
        }

        /// <summary>
        /// 用于判断当前函数是否在主线程中运行
        /// </summary>
        /// <returns></returns>
        protected bool IsInWorkThread()
        {
            if (null == thread)
                return false;
            if (IsWorking() && Thread.CurrentThread.ManagedThreadId == thread.ManagedThreadId)
                return true;
            return false;
        }

        public static bool IsWorkingStatus(JFWorkStatus ws)
        {
            return ws == JFWorkStatus.Running || ws == JFWorkStatus.Pausing || ws == JFWorkStatus.Interactiving;

        }

        public bool IsWorking()
        {
            return IsWorkingStatus(CurrWorkStatus);
        }


        public virtual JFWorkCmdResult Start()
        {
            lock (accessLocker)
            {
                lock (WorkStatusLocker)
                {
                    if (IsWorking())
                        return JFWorkCmdResult.Success;
                    //if (null == thread)
                    thread = new Thread(ThreadFunc);
                    cmdEvent.Reset();
                    rspEvent.Reset();
                    thread.Start();
                }
                
                return _SendCmd(CommandStart);
                
                
            }
        }

        public virtual JFWorkCmdResult Stop(int timeoutMilliseconds = -1)
        {
            lock (accessLocker)
            {
                JFWorkCmdResult ret = JFWorkCmdResult.UnknownError;
                Monitor.Enter(WorkStatusLocker);
                {
                    if (!IsWorking())
                    {
                        Monitor.Exit(WorkStatusLocker);
                        return JFWorkCmdResult.Success;
                    }
                    ret = _SendCmd(CommandStop, timeoutMilliseconds);
                    
                }
                Monitor.Exit(WorkStatusLocker);
                //if (ret == JFWorkCmdResult.Success)
                //    thread.Join();

                return ret;
            }
        }
        public virtual JFWorkCmdResult Pause(int timeoutMilliseconds = -1)
        {
            lock(accessLocker)
            {
                lock (WorkStatusLocker)
                {
                    if (CurrWorkStatus == JFWorkStatus.Pausing)
                        return JFWorkCmdResult.Success;
                    if (CurrWorkStatus != JFWorkStatus.Running)
                        return JFWorkCmdResult.StatusError;
                }
                return _SendCmd(CommandPause, timeoutMilliseconds);
            }
        }

        public virtual JFWorkCmdResult Resume(int timeoutMilliseconds = -1)
        {
            lock (accessLocker)
            {
                lock (WorkStatusLocker)
                {
                    if (CurrWorkStatus == JFWorkStatus.Running)
                        return JFWorkCmdResult.Success;
                    if (CurrWorkStatus != JFWorkStatus.Pausing)
                        return JFWorkCmdResult.StatusError;
                }
                    return _SendCmd(CommandResume, timeoutMilliseconds);
                
            }
        }

        public void Abort()
        {
            lock(accessLocker)
            {
                if (!thread.IsAlive)
                    return;
                thread.Abort();
                //thread.Join();
                thread = null;
                ChangeWorkStatus(JFWorkStatus.AbortExit);
            }
        }

        /// <summary>
        /// 线程函数内部调用
        /// 工作状态变化时，需要调用此函数
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="info"></param>
        protected void ChangeWorkStatus(JFWorkStatus ws)
        {
            lock (WorkStatusLocker)
            {
                if (CurrWorkStatus == ws)
                    return;
                CurrWorkStatus = ws;
            }
            WorkStatusChanged?.Invoke(this, CurrWorkStatus);
        }

        /// <summary>
        /// 业务状态发生改变时，调用此函数
        /// </summary>
        /// <param name="customStatus"></param>
        /// <param name="info"></param>
        /// <param name="param"></param>
        public void ChangeCustomStatus(int customStatus)
        {
            if (AllCustomStatus == null || AllCustomStatus.Length == 0)
                throw new ArgumentException("ChangeCustomStatus(int customStatus) failed by: " + customStatus + " is not defined!");
            CurrCustomStatus = customStatus;
            CustomStatusChanged?.Invoke(this, customStatus);
        }




        /// <summary>
        /// 通过注册的消息回调函数向外界发送一条文本消息
        /// </summary>
        /// <param name="info"></param>
        protected void SendMsg2Outter(string info)
        {
            if (string.IsNullOrEmpty(info))
                return;
            WorkMsg2Outter?.Invoke(this, info);
        }

        /// <summary>
        /// 工作线程开始时的准备工作，在线程开始时只执行一次
        /// 线程函数内部在调用PrepareWhenWorkStart后，进入While循环调用RunLoopInWork
        /// </summary>
        protected abstract void PrepareWhenWorkStart();

        /// <summary>
        /// 一个工作循环步骤，在线程的While循环中被重复调用，直到
        /// </summary>
        protected abstract void RunLoopInWork();

        /// <summary>
        /// 线程结束前的清理步骤
        /// </summary>
        protected abstract void CleanupWhenWorkExit();


        /// <summary>
        /// 工作线程内部收到暂停指令时，会先调用OnPause函数，继承类可重写此函数
        /// </summary>
        protected abstract void OnPause();

        /// <summary>
        /// 工作线程内部处于暂停状态时，收到恢复运行指令时，会调用OnResume函数，继承类可重写此函数
        /// </summary>
        protected abstract void OnResume();

        /// <summary>
        /// 工作线程内部收到推出指令时，会先调用OnStop函数，然后进入退出流程
        /// </summary>
        protected abstract void OnStop();


        /// <summary>
        /// 线程内部调用，退出线程
        /// </summary>
        /// <param name="wec"></param>
        /// <param name="info"></param>
        /// <param name="param"></param>
        protected void ExitWork(WorkExitCode wec, string info, object param = null)
        {
            //SendMsg2Outter(info);
            switch (wec)
            {
                case WorkExitCode.Command:
                    //ChangeWorkStatus(JFWorkStatus.CommandExit);
                    break;
                case WorkExitCode.Error:
                    //ChangeWorkStatus(JFWorkStatus.ErrorExit);
                    break;
                case WorkExitCode.Exception:
                    //ChangeWorkStatus(JFWorkStatus.ExceptionExit);
                    break;
                case WorkExitCode.Normal:
                    //ChangeWorkStatus(JFWorkStatus.NormalEnd);
                    break;
            }
            throw new JFWorkExitException(wec, info, param);
        }

        
        protected bool WaitCmd(out int cmd,int timeoutMilliSeconds = -1)
        {
            cmd = 0;
            CheckCmd(CycleMilliseconds < 0 ? -1 : timeoutMilliSeconds);//处理系统指令 ， 暂停/恢复/退出
            if (!cmdEvent.WaitOne(0))
                return false;
            if (command < int.MinValue || command > int.MaxValue) // 非用户定义指令
                return false;
            cmd = (int)command;
            return true;
        }


        ///// <summary>
        ///// JFWorkCmdChecker接口
        ///// 检查有没有工作指令（Start/Stop/Pause/Resume）到来
        ///// 不Response
        ///// </summary>
        ///// <param name="cmdCode"></param>
        ///// <param name="timeoutMiliSeconds"></param>
        ///// <returns></returns>
        //public JFWorkCmdChkCode CheckedWorkCmd( int timeoutMiliSeconds = -1)
        //{
        //    //if (!IsInWorkThread())
        //    //{ 
        //    //    return false;
        //    DateTime start = DateTime.Now;
        //    int milSecRemained = timeoutMiliSeconds;
        //    while (true)
        //    {
        //        if (!cmdEvent.WaitOne(milSecRemained))
        //            return JFWorkCmdChkCode.None;
        //        if (command == CommandStop)
        //            return JFWorkCmdChkCode.Stop;
        //        else if (command == CommandPause)
        //            return JFWorkCmdChkCode.Pause;
        //        if (command == CommandResume)
        //            return JFWorkCmdChkCode.Resume;
        //        else if (command == CommandStart)
        //            return JFWorkCmdChkCode.Run;



        //        if(timeoutMiliSeconds >= 0)
        //        {
        //            TimeSpan ts = DateTime.Now - start;
        //            milSecRemained -= (int)ts.TotalMilliseconds;
        //            if (milSecRemained <= 0)
        //                return JFWorkCmdChkCode.None;
        //        }

        //    }




        //}






        #region 用户指令

        List<int> _lstCommands = new List<int>();
        Dictionary<int,string> _dctCommandNames = new Dictionary<int, string>();
        public virtual int[] AllCmds { get { return _lstCommands.ToArray(); } }

        public virtual  string GetCmdName(int cmd)
        {
            if (!_lstCommands.Contains(cmd))
                throw new ArgumentOutOfRangeException("cmd = " + cmd + "is not contained in command list");
            if (_dctCommandNames.ContainsKey(cmd))
                return _dctCommandNames[cmd];
            return cmd.ToString();
        }


        /// <summary>
        /// 声明一个工作命令 , 在集成类的构造函数中调用
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        protected void DelearCommand(int cmd,string name)
        {
            if (null == name)
                throw new ArgumentNullException();

            if (_lstCommands.Contains(cmd))
                throw new ArgumentException("cmd = " + cmd + " is already decleared!");
                
            _lstCommands.Add(cmd);


            if (_dctCommandNames.ContainsKey(cmd))
                _dctCommandNames[cmd] = name;
            else
                _dctCommandNames.Add(cmd, name);


        }

        /// <summary>
        /// 用枚举类型申明所有的工作命令
        /// </summary>
        /// <param name="cmdEnum"></param>
        protected void DeclearAllCommands(Type cmdEnum)
        {
            _lstCommands.Clear();
            _dctCommandNames.Clear();
            Array cmds = Enum.GetValues(cmdEnum);
            string[] cmdNames = Enum.GetNames(cmdEnum);
            for (int i = 0; i < cmds.Length; i++)
                DelearCommand((int)cmds.GetValue(i), cmdNames[i]);
        }





        public JFWorkCmdResult SendCmd(int cmd, int timeoutMilliseconds = -1)
        {
            return _SendCmd((long)cmd, timeoutMilliseconds);
        }
        #endregion

        #region 用户自定义工作状态
        public  int CurrCustomStatus { get; private set; }



        List<int> _lstCustomStatus = new List<int>();
        Dictionary<int, string> _dctCSNames = new Dictionary<int, string>();
        /// <summary>
        /// 在继承类中枚举所有的自定义工作状态（与业务逻辑相关的）
        /// 建议0为默认状态
        /// </summary>
        public virtual int[] AllCustomStatus { get { return _lstCustomStatus.ToArray(); } }


        public virtual string GetCustomStatusName(int status)
        {
            if (!_lstCustomStatus.Contains(status))
                throw new ArgumentOutOfRangeException();
            if (!_dctCSNames.ContainsKey(status))
                return status.ToString();
            return _dctCSNames[status];

        }

        /// <summary>
        /// 声明一个自定义状态 ,在继承类的构造函数中使用
        /// </summary>
        /// <param name="status"></param>
        /// <param name="name"></param>
        protected void DeclearCustomStatus(int status,string name)
        {
            if (null == name)
                throw new ArgumentNullException();
            if (_lstCustomStatus.Contains(status))
                throw new ArgumentException("status = " + status + " is already decleared!");
            _lstCustomStatus.Add(status);
            _dctCSNames.Add(status, name);
        }

        /// <summary>
        /// 用结构体类型定义所有的自定义状态,在继承类的构造函数中使用
        /// </summary>
        /// <param name="csEnum"></param>
        protected void DeclearAllCustomStatus(Type csEnum)
        {
            _lstCustomStatus.Clear();
            _dctCSNames.Clear();
            Array arCS = Enum.GetValues(csEnum);
            string[] csNames = Enum.GetNames(csEnum);
            for(int i = 0; i < arCS.Length;i++)
            {
                DeclearCustomStatus((int)arCS.GetValue(i), csNames[i]);
            }
        }
        #endregion




        #region 工作线程函数内使用
        /// <summary>
        /// 基类内部调用，向线程发送包括开始/停止/暂停/恢复指令和用户指令在内的所有指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        protected JFWorkCmdResult _SendCmd(long cmd, int timeoutMilliseconds = -1)
        {
            lock(accessLocker)
            {
                rspEvent.Reset();
                command = cmd;
                cmdEvent.Set();
                if (!rspEvent.WaitOne(timeoutMilliseconds))
                {
                    cmdEvent.Reset();
                    return JFWorkCmdResult.Timeout;
                }
                cmdEvent.Reset();
                return cmdResult;
            }
        }

        /// <summary>
        /// 继承类线程函数内部使用
        /// </summary>
        protected void CheckCmd(int timeoutMilliseconds)
        {
            //int timeMilliSeconds = CycleMilliseconds;
            //if (timeMilliSeconds < 0)
            //    timeMilliSeconds = 0;
            while(true)
            {
                if(CurrWorkStatus == JFWorkStatus.Pausing) //当前处于暂停状态
                {
                    if(!cmdEvent.WaitOne(timeoutMilliseconds))
                        continue;
                    if(command == CommandResume) //收到恢复运行指令
                    {
                        OnResume();
                        RespCmd(JFWorkCmdResult.Success);
                        ChangeWorkStatus(JFWorkStatus.Running);
                        return;
                    }
                    else if(command == CommandStop)
                    {
                        OnStop();
                        RespCmd(JFWorkCmdResult.Success);
                        ExitWork(WorkExitCode.Command,"");
                        return;
                    }
                    else //不接受其他指令
                    {
                        RespCmd(JFWorkCmdResult.StatusError);
                        continue;
                    }
                }
                 

                if(CurrWorkStatus != JFWorkStatus.Running)
                {
                    RespCmd(JFWorkCmdResult.StatusError);
                    return;
                }

                //线程当前处于运行状态
                if (!cmdEvent.WaitOne(timeoutMilliseconds))
                    return;
                if (command == CommandStop)
                {
                    RespCmd(JFWorkCmdResult.Success);
                    ExitWork(WorkExitCode.Command, "收到退出指令" + DateTime.Now.ToString("yy.MM.dd.hh:mm:ss"));
                    return;
                }
                else if(command == CommandPause)
                {
                    OnPause();
                    RespCmd(JFWorkCmdResult.Success);
                    ChangeWorkStatus(JFWorkStatus.Pausing);
                    continue;
                }
                else if(command == CommandResume)
                {
                    RespCmd(JFWorkCmdResult.StatusError);
                    return;
                }
                else if(command >= int.MinValue && command <= int.MaxValue) //收到用户指令
                {
                    return;
                }
                else //不支持的指令
                {
                    RespCmd(JFWorkCmdResult.IllegalCmd);
                    return;
                }

            }
         
           
        }

        protected virtual void ThreadFunc()
        {
            WorkExitCode exitCode = WorkExitCode.Normal;
            long cmdWaited = CommandUnknown;
            try
            {
                cmdEvent.WaitOne();
                if (command != CommandStart)
                    ExitWork(WorkExitCode.Exception, "WorkThread receive first command is not CommandStart,command = " + command);
                RespCmd(JFWorkCmdResult.Success);
                ChangeWorkStatus(JFWorkStatus.Running);
                

                PrepareWhenWorkStart();
                while (true)
                {
                    CheckCmd(CycleMilliseconds < 0?-1: CycleMilliseconds);
                    RunLoopInWork();
                    
                }
            }
            catch(JFWorkExitException wee) //工作线程退出流程
            {
                //Monitor.Exit(workStatusLocker);
                exitCode = wee.ExitCode;
                SendMsg2Outter("任务即将退出:" + exitCode + " 信息:" + wee.ExitInfo);
                
            }
            catch(Exception ex)
            {
                exitCode = WorkExitCode.Exception;
                SendMsg2Outter("任务发生未知的程序异常:" + ex.Message);
                ChangeWorkStatus(JFWorkStatus.ExceptionExit);
            }
            finally
            {
                try
                {
                    OnStop();
                }
                catch(JFWorkExitException eeStop)
                {
                    switch (eeStop.ExitCode)
                    {
                        case WorkExitCode.Command://此时不会再接受指令
                            ;// ChangeWorkStatus(JFWorkStatus.CommandExit);
                            break;
                        case WorkExitCode.Normal:
                            ;// SendMsg2Outter("任务清理完成，退出运行");
                            break;
                        case WorkExitCode.Error:
                            SendMsg2Outter("OnStop() 发生错误:" + eeStop.Message); 
                            break;
                        case WorkExitCode.Exception:
                            SendMsg2Outter("OnStop() 发生异常:" + eeStop.Message); 
                            break;

                    }

                }
                catch(Exception exOnStop)
                {
                    SendMsg2Outter("OnStop() 发生程序异常:" + exOnStop.Message);
                }
                SendMsg2Outter("任务开始退出前清理");
                try
                {
                    CleanupWhenWorkExit();
                    SendMsg2Outter("任务清理完成，退出运行");
                }
                catch (JFWorkExitException exCleanWEE)
                {
                    switch (exCleanWEE.ExitCode)
                    {
                        case WorkExitCode.Command://此时不会再接受指令
                            ;// ChangeWorkStatus(JFWorkStatus.CommandExit);
                            break;
                        case WorkExitCode.Normal:
                            SendMsg2Outter("任务清理完成，退出运行");
                            break;
                        case WorkExitCode.Error:
                            SendMsg2Outter("任务清理发生错误:" + exCleanWEE.Message);
                            break;
                        case WorkExitCode.Exception:
                            SendMsg2Outter("任务清理发生异常:" + exCleanWEE.Message); 
                            break;
                    }
                }
                catch (Exception exCleanup)
                {
                    SendMsg2Outter("任务清理发生未定义程序异常:" + exCleanup.Message);
                }


                switch(exitCode)
                {
                    case WorkExitCode.Command:
                        ChangeWorkStatus(JFWorkStatus.CommandExit);
                        break;
                    case WorkExitCode.Error:
                        ChangeWorkStatus(JFWorkStatus.ErrorExit);
                        break;
                    case WorkExitCode.Exception:
                        ChangeWorkStatus(JFWorkStatus.ExceptionExit);
                        break;
                    case WorkExitCode.Normal:
                        ChangeWorkStatus(JFWorkStatus.NormalEnd);
                        break;

                }
                
            }

        }



        protected void RespCmd(JFWorkCmdResult cmdRst)
        {
            cmdEvent.Reset();
            cmdResult = cmdRst;
            rspEvent.Set();

        }









        #endregion

        /// <summary>线程访问同步锁</summary>
        protected object accessLocker;
        /// <summary> 用于保存调用者向线程发送的指令</summary>
        protected long command = 0;
        /// <summary>用于保存线程返回的执行指令（结果）</summary>
        protected JFWorkCmdResult cmdResult = JFWorkCmdResult.UnknownError;//long response = 0;
        /// <summary>调用者向线程发送指令的触发对象</summary>
        protected EventWaitHandle cmdEvent = null;
        /// <summary>线程返回执行结果的触发对象</summary>
        protected EventWaitHandle rspEvent = null;
        /// <summary>线程参数</summary>
        object threadParam = null;
        /// <summary>工作线程对象</summary>
        Thread thread = null;



        /// <summary>
        /// 线程退出代码
        /// </summary>
        protected  enum WorkExitCode
        {
            Normal, //线程正常完成后退出
            Command,    //收到退出指令
            Error,      //发生错误退出
            Exception,  //发生(程序)异常退出
        }

        /// <summary>
        /// 用于程序流控制的异常，不建议使用Try...Catch捕获
        /// </summary>
        protected class JFWorkExitException : Exception
        {
            public JFWorkExitException(WorkExitCode exitCode, string info, object param) : base()
            {
                ExitCode = exitCode;
                ExitInfo = info;
                ExitParam = param;
            }

            public WorkExitCode ExitCode { get; private set; }
            public string ExitInfo { get; private set; }
            protected object ExitParam { get; private set; }
        }

        /// <summary>
        /// 代理类:工作事件发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currStatus"></param>
        /// <param name="info"></param>

        protected static long CommandUnknown = long.MinValue; //
        protected static long CommandStart = long.MinValue + 1;
        protected static long CommandStop = long.MinValue + 2;
        protected static long CommandPause = long.MinValue + 3;
        protected static long CommandResume = long.MinValue + 4;
        
    }
}
