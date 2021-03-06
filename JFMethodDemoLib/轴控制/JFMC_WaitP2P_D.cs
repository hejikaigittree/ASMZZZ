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
    [JFDisplayName("等待轴P2P完成_D")]
    public class JFMC_WaitP2P_D : JFMethodInitParamBase,IJFMethod_T
    {
        //Init Param's Name 
        static string PN_AxisID = "轴ID";
        static string PN_TimeoutMilliSeconds = "超时毫秒";
        static string PN_CycleMilliSeconds = "轮循间隔毫秒";
        //Output Param's Name 
        static string ON_Result = "执行结果";

        public JFMC_WaitP2P_D()
        {
            string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            DeclearInput(PN_AxisID, typeof(string), "");
            DeclearInput(PN_TimeoutMilliSeconds, typeof(int), -1);
            DeclearInitParam(JFParamDescribe.Create(PN_CycleMilliSeconds, typeof(int), JFValueLimit.MinLimit | JFValueLimit.MaxLimit, new object[] { 10, 1000 }), 50);
            DeclearOutput(ON_Result, typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            //string axisID = GetInitParamValue(PN_AxisID) as string;
            //if(string.IsNullOrEmpty(axisID))
            //{
            //    errorInfo = "初始化参数:\"轴名称\"未设置";
            //    return false;
            //}
            errorInfo = "Success";
            return true;
        }

        int _workCmd = 0; //-1：指令退出 0:正常运行，1:暂停 ，2：恢复，3：等待指令
        bool _isRunning = false;

        protected override bool ActionGenuine(out string errorInfo)
        {
            _isRunning = true;
            string axisID = GetMethodInputValue(PN_AxisID) as string;
            if (!JFHubCenter.Instance.MDCellNameMgr.ContainAxisName(axisID))
            {
                errorInfo = "参数项:\"轴ID\" = " + axisID + " 在设备名称表中不存在";
                _workCmd = 0;
                _isRunning = false;
                SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                return false;
            }

            int timeoutMilSeconds = (int)GetMethodInputValue(PN_TimeoutMilliSeconds);
            int cycleMilliSeconds= (int)GetInitParamValue(PN_CycleMilliSeconds); ;
            JFDevChannel axisChn = new JFDevChannel(JFDevCellType.Axis, axisID);
            if(!axisChn.CheckAvalid(out errorInfo))
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
                    int errCode = 0;
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

                    if (axisStatus[_md.MSID_MDN])
                    {
                        ActionErrorInfo = "Success";
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
                            errorInfo = "超时未等到轴:\" " + axisID + "\"归零完成 ";
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
                    double tgtPos = 0;
                    int errCode = _md.GetCmdPos(_ci.ChannelIndex, out tgtPos);
                    if (0 != errCode)
                    {
                        errorInfo = "恢复运行时获取目标位置失败:" + _md.GetErrorInfo(errCode);
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                        return false;
                    }
                    errCode = _md.AbsMove(_ci.ChannelIndex, tgtPos);
                    if (0 != errCode)
                    {
                        errorInfo = "恢复轴P2P运动失败:" + _md.GetErrorInfo(errCode);
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
