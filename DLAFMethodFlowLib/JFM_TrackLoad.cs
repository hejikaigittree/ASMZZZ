using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using System.Threading;
using DLAF;
namespace DLAFMethodLib
{
    [JFCategoryLevels(new string[] { "JF 工站流程", "station" })]
    [JFDisplayName("轨道上料")]
    class JFM_TrackLoad : JFMethodInitParamBase, IJFMethod_T,IJFStationBaseAcq
    {
        static string CheckPos = "检测位";
        static string LeftClampWaitPos = "左夹爪等待位";
        static string LeftClampRightPos = "左夹爪右限位";
        static string LeftClampClosePos = "左夹爪闭合位";
        static string LeftClampOpenPos = "左夹爪张开位";
        static string FrameLongth = "基板长度";
        static string FrameInsertDis = "基板入口插入长度";
        static string ClampLongth = "夹爪长度";
        
        //Axis Name
        static string LeftClampName = "导轨左夹爪电机";      

        static string LeftPushName = "上料导轨夹爪X轴";     
        //DI Name
        static string LeftFrameCheckName = "上料基板检测信号";       
        static string CylUpCheck = "载台气缸上位检";
        static string CylDownCheck = "载台气缸上位检";

        static string CylClampUpcheck = "上料夹爪气缸上位置信号";
        static string CylClampDowncheck = "上料夹爪气缸下位置信号";
        //DO Name
        static string CyName = "载台气缸";
        static string CyClampName = "上料夹爪夹紧气缸电磁阀";
        public JFM_TrackLoad()
        { 

        }
        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }

        char step = 'a';

        double RightFactPos = 0;
        double LeftFactPos = 0;
        double NeedPos = 0;
        double MotorPos = 0;
        protected override bool ActionGenuine(out string errorInfo)
        {
            while (true)
            {
                if (1 == _workCmd)//当前为暂停状态
                {
                    Thread.Sleep(10);
                    continue;
                }
                else
                if (-1 == _workCmd)//指令退出
                {
                    char step = 'a';
                    errorInfo = "收到退出指令";
                    _workCmd = 0;
                    _isRunning = false;
                    return false;
                }
                else
                if (0 == _workCmd)//指令退出
                {
                    errorInfo = "收到回复指令";
                    _workCmd = 0;
                    _isRunning = true;                   

                    if (_station == null)
                    {
                        errorInfo = "工站未设置";
                        return false;
                    }
                    double InsetDis = (double)_station.GetCfgParamValue(FrameInsertDis);
                    double TrackTotalDis = (double)_station.GetCfgParamValue(LeftClampRightPos) - (double)_station.GetCfgParamValue(LeftClampWaitPos);
                   // double _FrameLong = (double)_station.GetCfgParamValue(FrameLongth);
                    double _CheckPos = (double)_station.GetCfgParamValue(CheckPos);
                    double RightEndPos = (double)_station.GetCfgParamValue(LeftClampRightPos);
                    double _ClampLongth = (double)_station.GetCfgParamValue(ClampLongth);

                    JFDLAFProductRecipe BoardRecipe = ((JFDLAFProductRecipe)JFHubCenter.Instance.RecipeManager.GetRecipe("Product",
                                        (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID")));
                    double _FrameLong = BoardRecipe.FrameLength;
                    int delay = 10000;
                    switch (step)
                    {
                        case 'a':
                            if (!JFWaitDi(LeftFrameCheckName, out errorInfo, true))
                            {
                                errorInfo = _station.Name + ":" + LeftFrameCheckName + "获取DI状态失败！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            RightFactPos = InsetDis;
                            step = 'b';
                            break;

                        case 'b':
                            //if (!Move(LeftClampName, LeftClampOpenPos, true, delay, out errorInfo))
                            //    return false;
                            if(!_station.SetDO(CyClampName, true, out errorInfo))
                               return false;
                            if (!JFWaitDi(CylClampUpcheck, out errorInfo, true, 4000))
                            {
                                errorInfo = _station.Name + ":" + CylClampUpcheck + "获取DI状态失败！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                                step = 'c';
                            break;

                        case 'c':
                            if (!Move(LeftPushName, LeftClampWaitPos, true, delay, out errorInfo))
                                return false;

                            step = 'd';
                            break;
                        case 'd':
                            if (!_station.SetDO(CyClampName, false, out errorInfo))
                                return false;
                            if (!JFWaitDi(CylClampUpcheck, out errorInfo, false, 2000))
                            {
                                errorInfo = _station.Name + ":" + CylClampDowncheck + "获取DI状态失败！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            step = 'e';
                            break;
                        case 'e':
                            if (!Move(LeftPushName, LeftClampRightPos, true, delay, out errorInfo))
                                return false;
                            RightFactPos += TrackTotalDis;
                            LeftFactPos = RightFactPos - _FrameLong;

                            NeedPos = _CheckPos + _FrameLong - RightFactPos;
                            step = 'f';
                            break;
                        case 'f':
                            //if (!Move(LeftClampName, LeftClampOpenPos, true, delay, out errorInfo))
                            //    return false;
                            if (!_station.SetDO(CyClampName, true, out errorInfo))
                                return false;
                            if (!JFWaitDi(CylClampUpcheck, out errorInfo, true, 2000))
                            {
                                errorInfo = _station.Name + ":" + CylClampUpcheck + "获取DI状态失败！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            //if (NeedPos > TrackTotalDis)
                            // {
                            //     if (LeftFactPos > 30)
                            //         step = 'g';

                            //     step = 'c';
                            // }
                            // else
                            // {
                            //     if (LeftFactPos > 30)
                            //         step = 'g';
                            // }
                            // step = 'c';

                            if (LeftFactPos > _ClampLongth)
                                step = 'g';
                            else
                                step = 'c';

                            break;
                        case 'g':
                            double MotorBack = NeedPos;
                            double clampRightPos = RightEndPos + _ClampLongth;
                            double ShoulBackPos = clampRightPos - LeftFactPos;

                            // MotorPos = (double)_station.GetCfgParamValue(LeftClampRightPos) - MotorBack;

                            MotorPos = (double)_station.GetCfgParamValue(LeftClampRightPos) - ShoulBackPos - 10;
                            if (!_station.MoveAxis(LeftPushName, MotorPos, true, out errorInfo))
                            {
                                errorInfo = _station.Name + ":" + LeftPushName + "运动超时未完成！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            if (_station.WaitMotionDone(LeftPushName, 10000) != JFWorkCmdResult.Success)
                            {
                                errorInfo = _station.Name + ":" + LeftClampName + "运动超时未完成！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            step = 'h';
                            break;
                        case 'h':
                            //if (!Move(LeftClampName, LeftClampClosePos, true, delay, out errorInfo))
                            //    return false;
                            if (!_station.SetDO(CyClampName, false, out errorInfo))
                                return false;
                            if (!JFWaitDi(CylClampUpcheck, out errorInfo, false, 2000))
                            {
                                errorInfo = _station.Name + ":" + CylClampDowncheck + "获取DI状态失败！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            step = 'i';
                            break;
                        case 'i':
                            //if (!_station.MoveAxis(LeftPushName, MotorPos + NeedPos + 5, true, out errorInfo))
                            if (!_station.MoveAxis(LeftPushName, _CheckPos, true, out errorInfo))
                            {
                                errorInfo = _station.Name + ":" + LeftPushName + "运动超时未完成！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            if (_station.WaitMotionDone(LeftPushName, 10000) != JFWorkCmdResult.Success)
                            {
                                errorInfo = _station.Name + ":" + LeftClampName + "运动超时未完成！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            step = 'j';
                            break;
                        case 'j':
                            if (!_station.SetDO(CyClampName, true, out errorInfo))
                                return false;
                            if (!JFWaitDi(CylClampUpcheck, out errorInfo, true, 2000))
                            {
                                errorInfo = _station.Name + ":" + CylClampUpcheck + "获取DI状态失败！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            if (!Move(LeftPushName, LeftClampWaitPos, true, delay, out errorInfo))
                                return false;
                            step = 'a';
                            RightFactPos = 0;
                            errorInfo = "Success";

                            return true;

                        default:
                            errorInfo = _station.Name + ":" + "Action 调用出错！";
                            return false;
                    }
                }
            }
               // errorInfo = "Success";
              //  return true;
            
        }
           
        protected bool Move(string AxisName, string CfgParm, bool isAbs, int WaitTime,out string errInfo)
        {
            
            if (!_station.MoveAxis(AxisName, (double)_station.GetCfgParamValue(CfgParm), isAbs, out errInfo))
            {
                errInfo = _station.Name + ":" + LeftClampName + "运动超时未完成！";
                _workCmd = 0;
                _isRunning = false;
                return false;
            }

            if (!JFMotionDone(AxisName,out errInfo,WaitTime))
            {
                //errInfo = _station.Name + ":" + LeftClampName + "运动超时未完成！";
                _workCmd = 0;
                _isRunning = false;
                return false;
            }

            return true;
        }        
       protected bool JFMotionDone(string AxisName, out string errorInfo,int delaytime)
        {
            errorInfo = "";
            JFDevChannel axisChn = new JFDevChannel(JFDevCellType.Axis, AxisName);
            if (!axisChn.CheckAvalid(out errorInfo))
            {
                _workCmd = 0;
                _isRunning = false;             
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
                        return false;
                    }
                    if (axisStatus[_md.MSID_ALM])
                    {
                        errorInfo = "轴已报警!";
                        _workCmd = 0;
                        _isRunning = false;                     
                        return false;
                    }

                    if (_md.MSID_EMG > -1 && axisStatus[_md.MSID_EMG])
                    {
                        errorInfo = "轴已急停!";
                        _workCmd = 0;
                        _isRunning = false;                       
                        return false;
                    }

                    //if (!axisStatus[_md.MSID_SVO])
                    //{
                    //    errorInfo = "轴伺服已断电!";
                    //    _workCmd = 0;
                    //    _isRunning = false;
                    //    SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                    //    return false;
                    //}

                    if (axisStatus[_md.MSID_MDN])
                    {
                        ActionErrorInfo = "Success";
                        _workCmd = 0;
                        _isRunning = false;                       
                        return true;
                    }                                    
                        TimeSpan ts = DateTime.Now - startTime;
                        if (ts.TotalMilliseconds >= delaytime)
                        {
                            errorInfo = "超时未等到轴:\" " + AxisName + "\"动作完成 ";
                            _workCmd = 0;
                            _isRunning = false;                          
                            return false;
                        }
                    
                    Thread.Sleep(10);
                }
                else if (1 == _workCmd)//当前为暂停状态
                {                   
                    _station.StopAxis(LeftClampName);
                    _station.StopAxis(LeftPushName);

                    _isRunning = false;
                    _workCmd = 0;
                    continue;
                }
                else if (2 == _workCmd)//当前为恢复状态
                {                    
                    _workCmd = 0;
                    continue;
                }
                else if (-1 == _workCmd)//指令退出
                {
                    _station.StopAxis(LeftClampName);
                    _station.StopAxis(LeftPushName);

                    errorInfo = "收到退出指令";
                    _workCmd = 0;
                    _isRunning = false;                    
                    return false;
                }
                else
                {
                    Thread.Sleep(10);
                    continue;
                }
            }
        }
        protected bool JFWaitDi(string diName, out string errorInfo,bool isTurnOn ,int delaytime = -1)
        {
            if (!JFHubCenter.Instance.MDCellNameMgr.ContainDiName(diName))
            {
                errorInfo = "参数项:\"DI通道名称\" = " + diName + " 在设备名称表中不存在";
                return false;
            }

            JFDevChannel diChn = new JFDevChannel(JFDevCellType.DI, diName);
            string avalidInfo;
            if (!diChn.CheckAvalid(out avalidInfo))
            {
                errorInfo = avalidInfo;
                return false;
            }
            IJFDevice_MotionDaq _dev = null;
            IJFModule_DIO _dio = null;
            JFDevCellInfo _ci = null;

            _dev = diChn.Device() as IJFDevice_MotionDaq;
            _ci = diChn.CellInfo();
            _dio = _dev.GetDio(_ci.ModuleIndex);

            DateTime startTime = DateTime.Now;
            while (true)
            {
                if (0 == _workCmd) //正常工作
                {
                    // DI状态检查
                    bool isON = false;
                    int errCode = _dio.GetDI(_ci.ChannelIndex, out isON);
                    if (errCode != 0)
                    {
                        errorInfo = "获取DI状态失败!" + _dio.GetErrorInfo(errCode);
                        _workCmd = 0;
                        _isRunning = false;                    
                        return false;
                    }

                    if (isTurnOn == isON)
                    {
                        errorInfo = "Success";
                        _workCmd = 0;
                        _isRunning = false;                        
                        return true;
                    }

                    if (delaytime >= 0)
                    {
                        TimeSpan ts = DateTime.Now - startTime;
                        if (ts.TotalMilliseconds >= delaytime)
                        {
                             errorInfo = "超时未等到DI:\" " + diName + "\"状态为" + isTurnOn.ToString();
                            _workCmd = 0;
                            _isRunning = false;                          
                            return false;
                        }
                    }
                    Thread.Sleep(10);
                }
                else if (1 == _workCmd)//当前为暂停状态
                {
                    Thread.Sleep(10);
                    continue;
                }
                else//指令退出
                {
                    errorInfo = "收到退出指令";
                    _workCmd = 0;
                    _isRunning = false;                   
                    return false;
                }
            }
        }
        protected override bool InitializeGenuine(out string errorInfo)
        {
           
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
