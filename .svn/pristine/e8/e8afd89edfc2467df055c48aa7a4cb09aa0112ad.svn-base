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
    [JFDisplayName("等待轴直线插补完成_D")]

    public class JFCM_WaitLineMove_D : JFMethodInitParamBase, IJFMethod_T
    {
        //Init Param's Name 
        static string PN_AxisID = "轴名称列表";
        static string PN_TimeoutMilliSeconds = "超时毫秒";
        static string PN_CycleMilliSeconds = "轮循间隔毫秒";
        static string PN_AbsMode = "绝对位置模式";
        //Output Param's Name 
        static string ON_Result = "执行结果";

        public JFCM_WaitLineMove_D()
        {
            DeclearInitParam(PN_AxisID, typeof(string[]), new string[] { });
            DeclearInput(PN_TimeoutMilliSeconds, typeof(int),  -1);
            DeclearInitParam(JFParamDescribe.Create(PN_CycleMilliSeconds, typeof(int), JFValueLimit.MinLimit | JFValueLimit.MaxLimit, new object[] { 10, 1000 }), 50);
            DeclearInput(PN_AbsMode, typeof(bool), true);
            DeclearOutput(ON_Result, typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            _isRunning = true;
            string[] axisIDList = GetInitParamValue(PN_AxisID) as string[];
            if (axisIDList == null)
            {
                errorInfo = "轴ID列表长度为空";
                return false;
            }

            int[] iAxisIDList = new int[axisIDList.Length];
            if (axisIDList.Length <= 0)
            {
                errorInfo = "轴ID列表长度<=0";
                return false;
            }

            IJFModule_Motion _md = null;
            JFDevCellInfo _ci = null;
            IJFDevice_MotionDaq _dev = null;
            IJFDevice_MotionDaq _devBuff = null;
            List<JFDevCellInfo> cibuffList = new List<JFDevCellInfo>();

            _mdList = new List<IJFModule_Motion>();
            _ciList = new List<JFDevCellInfo>();

            //检查轴参数合法性
            for (int i=0;i<axisIDList.Length;i++)
            {
                if (!JFHubCenter.Instance.MDCellNameMgr.ContainAxisName(axisIDList[i]))
                {
                    errorInfo = "参数项:\"轴ID\" = " + axisIDList[i] + " 在设备名称表中不存在";
                    return false;
                }
                JFDevChannel axisChn = new JFDevChannel(JFDevCellType.Axis, axisIDList[1]);

                if (!axisChn.CheckAvalid(out errorInfo))
                {
                    return false;
                }

                _dev = axisChn.Device() as IJFDevice_MotionDaq;
                _ci = axisChn.CellInfo();
                _md = _dev.GetMc(_ci.ModuleIndex);

                if (_devBuff != null)
                {
                    if (_devBuff != _dev)
                    {
                        errorInfo = "轴ID列表中的所有轴并不来源于同一个设备";
                        return false;
                    }
                }
                _devBuff = _dev;

                foreach (JFDevCellInfo jFDevCellInfo in cibuffList)
                {
                    if (jFDevCellInfo.ChannelIndex == _ci.ChannelIndex)
                    {
                        errorInfo = "轴ID列表中存在重复的轴名称";
                        return false;
                    }
                }
                cibuffList.Add(_ci);

                _ciList.Add(_ci);
                _mdList.Add(_md);
                iAxisIDList[i] = _ci.ChannelIndex;
            }

            int timeoutMilSeconds = (int)GetMethodInputValue(PN_TimeoutMilliSeconds);
            int cycleMilliSeconds = (int)GetInitParamValue(PN_CycleMilliSeconds);
            bool isAbs = (bool)GetMethodInputValue(PN_AbsMode);

            DateTime startTime = DateTime.Now;
            while (true)
            {
                if (0 == _workCmd) //正常工作
                {
                    int errCode = 0;
                    // 其他状态检查
                    bool[] axisStatus;
                    bool isMotionDone = true;
                    for (int i = 0; i < _ciList.Count; i++)
                    {
                        errCode = _mdList[i].GetMotionStatus(_ciList[i].ChannelIndex, out axisStatus);
                        if (errCode != 0)
                        {
                            errorInfo = "获取轴状态失败!" + _mdList[i].GetErrorInfo(errCode);
                            _workCmd = 0;
                            _isRunning = false;
                            SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                            return false;
                        }
                        if (axisStatus[_mdList[i].MSID_ALM])
                        {
                            errorInfo = "轴已报警!";
                            _workCmd = 0;
                            _isRunning = false;
                            SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                            return false;
                        }

                        if (_mdList[i].MSID_EMG > -1 && axisStatus[_mdList[i].MSID_EMG])
                        {
                            errorInfo = "轴已急停!";
                            _workCmd = 0;
                            _isRunning = false;
                            SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                            return false;
                        }

                        if (!axisStatus[_mdList[i].MSID_SVO])
                        {
                            errorInfo = "轴伺服已断电!";
                            _workCmd = 0;
                            _isRunning = false;
                            SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                            return false;
                        }

                        if (!axisStatus[_mdList[i].MSID_MDN]) 
                        {
                            isMotionDone = false;
                            break;
                        }
                    }

                    if(isMotionDone)
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
                            errorInfo = "超时未等到直线插补运动完成 ";
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
                    for (int i = 0; i < _ciList.Count; i++)
                    {
                        errCode = _mdList[i].StopAxis(_ciList[i].ChannelIndex);
                        if (0 != errCode)
                        {
                            errorInfo = "停止轴归零运动失败:" + _mdList[i].GetErrorInfo(errCode);
                            _workCmd = 0;
                            _isRunning = false;
                            SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                            return false;
                        }
                    }
                    _workCmd = 0;
                    continue;
                }
                else if (2 == _workCmd)//当前为恢复状态
                {
                    double[] dPosList = new double[_ciList.Count];
                    double tgtPos = 0;
                    int errCode = 0;
                    for (int i = 0; i < _ciList.Count; i++)
                    {
                        errCode = _mdList[i].GetCmdPos(_ciList[i].ChannelIndex, out tgtPos);
                        if (0 != errCode)
                        {
                            errorInfo = "恢复直线插补运行时获取目标位置失败:" + _mdList[i].GetErrorInfo(errCode);
                            _workCmd = 0;
                            _isRunning = false;
                            SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                            return false;
                        }
                        dPosList[i] = tgtPos;
                    }
                    if (isAbs)
                        errCode = _mdList[0].AbsLine(iAxisIDList, dPosList);
                    else
                        errCode = _mdList[0].RelLine(iAxisIDList, dPosList);
                    if (0 != errCode)
                    {
                        errorInfo = "恢复轴" + (isAbs ? "绝对式" : "相对式") + "直线插补运动失败:" + _mdList[0].GetErrorInfo(errCode);
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
                    for (int i = 0; i < _ciList.Count; i++)
                    {
                        errCode = _mdList[i].StopAxis(_ciList[i].ChannelIndex);
                        if (0 != errCode)
                        {
                            errorInfo = "停止轴归零运动失败:" + _mdList[i].GetErrorInfo(errCode);
                            _workCmd = 0;
                            _isRunning = false;
                            SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                            return false;
                        }
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

        List<IJFModule_Motion> _mdList = null;
        List<JFDevCellInfo> _ciList = null;
        int _workCmd = 0; //-1：指令退出 0:正常运行，1:暂停 ，2：恢复，3：等待指令
        bool _isRunning = false;

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string[] axisIDList = GetInitParamValue(PN_AxisID) as string[];

            if (axisIDList == null)
            {
                errorInfo = "参数项:\"轴ID\"未设置/空值";
                return false;
            }

            if (axisIDList.Length == 0)
            {
                errorInfo = "参数项:\"轴ID\"未设置/空值";
                return false;
            }

            foreach (string axisID in axisIDList)
            {
                if (string.IsNullOrEmpty(axisID))
                {
                    errorInfo = "参数项:\"轴ID\"未设置/空值";
                    return false;
                }
            }

            errorInfo = "Success";
            return true;
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
