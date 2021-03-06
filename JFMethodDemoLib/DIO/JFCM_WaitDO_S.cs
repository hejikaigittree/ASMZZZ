using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using System.Threading;

namespace JFMethodCommonLib.DIO
{
    /// <summary>
    /// JFCM_WaitDO_S 等待一个DO通道的状态
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 设备控制", "DIO" })]
    [JFDisplayName("等待DO状态_S")]

    public class JFCM_WaitDO_S : JFMethodInitParamBase, IJFMethod_T
    {
        //Init Param's Name 
        static string PN_DOID = "DO通道名称";
        static string PN_TimeoutMilliSeconds = "超时毫秒";
        static string PN_CycleMilliSeconds = "轮循间隔毫秒";
        static string PN_WaitDOStatus = "等待DO状态";
        //Output Param's Name 
        static string ON_Result = "执行结果";

        public JFCM_WaitDO_S()
        {
            string[] allDoNames = JFHubCenter.Instance.MDCellNameMgr.AllDoNames();
            DeclearInitParam(JFParamDescribe.Create(PN_DOID, typeof(string), JFValueLimit.Options, allDoNames), "");
            DeclearInitParam(JFParamDescribe.Create(PN_TimeoutMilliSeconds, typeof(int), JFValueLimit.NonLimit, null), -1);
            DeclearInitParam(JFParamDescribe.Create(PN_CycleMilliSeconds, typeof(int), JFValueLimit.MinLimit | JFValueLimit.MaxLimit, new object[] { 10, 1000 }), 50);
            DeclearInitParam(JFParamDescribe.Create(PN_WaitDOStatus, typeof(bool), JFValueLimit.NonLimit, null), true);
            DeclearOutput(ON_Result, typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string doName = GetInitParamValue(PN_DOID) as string;
            if (string.IsNullOrEmpty(doName))
            {
                errorInfo = "参数项\"DO通道名称\"未设置/空字串";
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        IJFDevice_MotionDaq _dev = null;
        IJFModule_DIO _dio = null;
        JFDevCellInfo _ci = null;
        int _workCmd = 0; //-1：指令退出 0:正常运行，1:暂停
        bool _isRunning = false;

        protected override bool ActionGenuine(out string errorInfo)
        {
            _isRunning = true;
            string doName = GetInitParamValue(PN_DOID) as string;
            if (!JFHubCenter.Instance.MDCellNameMgr.ContainDoName(doName))
            {
                errorInfo = "参数项:\"DO通道名称\" = " + doName + " 在设备名称表中不存在";
                return false;
            }

            bool isDOEnable = (bool)GetInitParamValue(PN_WaitDOStatus);
            int timeoutMilSeconds = (int)GetInitParamValue(PN_TimeoutMilliSeconds);
            int cycleMilliSeconds = (int)GetInitParamValue(PN_CycleMilliSeconds);

            JFDevChannel doChn = new JFDevChannel(JFDevCellType.DO, doName);
            string avalidInfo;
            if (!doChn.CheckAvalid(out avalidInfo))
            {
                errorInfo = avalidInfo;
                return false;
            }

            _dev = doChn.Device() as IJFDevice_MotionDaq;
            _ci = doChn.CellInfo();
            _dio = _dev.GetDio(_ci.ModuleIndex);

            DateTime startTime = DateTime.Now;
            while (true)
            {
                if (0 == _workCmd) //正常工作
                {
                    // DI状态检查
                    bool isON = false;
                    int errCode = _dio.GetDO(_ci.ChannelIndex, out isON);
                    if (errCode != 0)
                    {
                        errorInfo = "获取DO状态失败!" + _dio.GetErrorInfo(errCode);
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.Timeout);
                        return false;
                    }

                    if (isDOEnable == isON)
                    {
                        errorInfo = "Success";
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.Success);
                        return true;
                    }

                    if (timeoutMilSeconds >= 0)
                    {
                        TimeSpan ts = DateTime.Now - startTime;
                        if (ts.TotalMilliseconds >= timeoutMilSeconds)
                        {
                            errorInfo = "超时未等到DO:\" " + doName + "\"状态为" + isDOEnable.ToString();
                            _workCmd = 0;
                            _isRunning = false;
                            SetOutputParamValue(ON_Result, JFWorkCmdResult.Timeout);
                            return true;
                        }
                    }
                    Thread.Sleep(cycleMilliSeconds);
                }
                else if (1 == _workCmd)//当前为暂停状态
                {
                    Thread.Sleep(cycleMilliSeconds);
                    continue;
                }
                else//指令退出
                {
                    errorInfo = "收到退出指令";
                    _workCmd = 0;
                    _isRunning = false;
                    SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                    return false;
                }
            }
        }

        public void Pause()
        {
            if (!_isRunning)
                return;
            _workCmd = 1;
        }

        public void Resume()
        {
            if (!_isRunning)
                return;
            _workCmd = 0;
        }

        public void Exit()
        {
            if (!_isRunning)
                return;
            _workCmd = -1;
        }
    }
}