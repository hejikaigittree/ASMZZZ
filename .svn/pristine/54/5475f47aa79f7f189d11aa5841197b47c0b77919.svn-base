using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using System.Threading;

namespace JFMethodCommonLib.轴控制
{
    [JFCategoryLevels(new string[] { "JF 设备控制", "Axis" })]
    [JFDisplayName("轴归零完成_S")]
    public class JFCM_WaitHome_D : JFMethodInitParamBase,IJFMethod_T
    {
        //Init Param's Name 
        static string PN_AxisID = "轴ID";
        static string PN_TimeoutMilliSeconds = "超时毫秒";
        static string PN_CycleMilliSeconds = "轮循间隔毫秒";
        //Output Param's Name 
        static string ON_Result = "执行结果";
        public JFCM_WaitHome_D()
        {
            string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            DeclearInitParam(JFParamDescribe.Create(PN_AxisID, typeof(string), JFValueLimit.Options, allAxisIDs), "");
            DeclearInitParam(JFParamDescribe.Create(PN_CycleMilliSeconds, typeof(int), JFValueLimit.MinLimit | JFValueLimit.MaxLimit, new object[] { 10, 1000 }), 50);
            DeclearInitParam(JFParamDescribe.Create(PN_TimeoutMilliSeconds, typeof(int), JFValueLimit.NonLimit, null), -1);
            DeclearOutput(ON_Result, typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            _isRunning = true;
            string axisID = GetInitParamValue(PN_AxisID) as string;
            if (!JFHubCenter.Instance.MDCellNameMgr.ContainAxisName(axisID))
            {
                errorInfo = "参数项:\"轴ID\" = " + axisID + " 在设备名称表中不存在";
                _workCmd = 0;
                _isRunning = false;
                SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                return false;
            }

            int timeoutMilSeconds = (int)GetInitParamValue(PN_TimeoutMilliSeconds);
            int cycleMilliSeconds = (int)GetInitParamValue(PN_CycleMilliSeconds);
            JFDevChannel axisChn = new JFDevChannel(JFDevCellType.Axis, axisID);
            if (!axisChn.CheckAvalid(out errorInfo))
            {
                _workCmd = 0;
                _isRunning = false;
                SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                return false;
            }

            IJFDevice_MotionDaq _dev = null;
            IJFModule_Motion _md = null;
            JFDevCellInfo _ci = null;

            _dev = axisChn.Device() as IJFDevice_MotionDaq;
            _ci = axisChn.CellInfo();
            _md = _dev.GetMc(_ci.ModuleIndex);

            DateTime startTime = DateTime.Now;
            while (true)
            {
                if (0 == _workCmd) //正常工作
                {
                    //归零完成检查
                    int errCode = 0;
                    bool isHomeDone = false;
                    errCode = _md.IsHomeDone(_ci.ChannelIndex, out isHomeDone);
                    if (errCode != 0)
                    {
                        errorInfo = "获取HomeDone状态失败!" + _md.GetErrorInfo(errCode);
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                        return false;
                    }
                    if (isHomeDone)
                    {
                        errorInfo = "Success";
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.Success);
                        return true;
                    }

                    // 其他状态检查
                    bool[] axisStatus;
                    errCode = _md.GetMotionStatus(_ci.ChannelIndex, out axisStatus);
                    if (errCode != 0)
                    {
                        errorInfo = "获取轴状态失败!" + _md.GetErrorInfo(errCode);
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                        return false;
                    }
                    if (axisStatus[_md.MSID_ALM])
                    {
                        errorInfo = "轴已报警!";
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                        return false;
                    }

                    if (_md.MSID_EMG > -1 && axisStatus[_md.MSID_EMG])
                    {
                        errorInfo = "轴已急停!";
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                        return false;
                    }

                    if (!axisStatus[_md.MSID_SVO])
                    {
                        errorInfo = "轴伺服已断电!";
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                        return false;
                    }

                    if (timeoutMilSeconds >= 0)
                    {
                        TimeSpan ts = DateTime.Now - startTime;
                        if (ts.TotalMilliseconds >= timeoutMilSeconds)
                        {
                            errorInfo = "超时未等到轴:\" " + axisID + "\"归零完成 " ;
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
                    int errCode = 0;
                    errCode = _md.StopAxis(_ci.ChannelIndex);
                    if (0 != errCode)
                    {
                        errorInfo = "停止轴归零运动失败:" + _md.GetErrorInfo(errCode);
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                        return false;
                    }
                    _workCmd = 0;
                    continue;
                }
                else if (2 == _workCmd)//当前为恢复状态
                {
                    int errCode = 0;
                    errCode = _md.Home(_ci.ChannelIndex);
                    if (0 != errCode)
                    {
                        errorInfo = "恢复轴归零运动失败:" + _md.GetErrorInfo(errCode);
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                        return false;
                    }
                    _workCmd = 0;
                    continue;
                }
                else if (-1 == _workCmd)//指令退出
                {
                    int errCode = 0;
                    errCode = _md.StopAxis(_ci.ChannelIndex);
                    if (0 != errCode)
                    {
                        errorInfo = "停止轴归零运动失败:" + _md.GetErrorInfo(errCode);
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                        return false;
                    }

                    errorInfo = "收到退出指令";
                    _workCmd = 0;
                    _isRunning = false;
                    SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                    return false;
                }
                else
                {
                    Thread.Sleep(cycleMilliSeconds);
                    continue;
                }
            }
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string axisID = GetInitParamValue("轴ID") as string;
            if (string.IsNullOrEmpty(axisID))
            {
                errorInfo = "参数项:\"轴ID\"未设置/空值";
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        int _workCmd = 0; //-1：指令退出 0:正常运行，1:暂停 ，2：恢复 ，3：等待指令
        bool _isRunning = false;

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
            _workCmd = 2;
        }

        public void Exit()
        {
            if (!_isRunning)
                return;
            _workCmd = -1;
        }
    }
}
