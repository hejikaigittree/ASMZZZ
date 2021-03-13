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
    [JFDisplayName("轨道下料")]
    class JFM_TrackUnLoad : JFMethodInitParamBase, IJFMethod_T, IJFStationBaseAcq
    {
        static string CheckPos = "检测位";
        
        static string FrameLongth = "基板长度";        

        static string RighClampWaitPos = "右夹爪等待位";
        static string RighClampRightPos = "右夹爪右限位";
        static string RighClampClosePos = "右夹爪闭合位";
        static string RighClampOpenPos = "右夹爪张开位";
        //Axis Name   
        static string RighClampName = "导轨右夹爪电机";       
        static string RighPushName = "下料导轨夹爪X轴";
        //DI Name   
        static string RightFrameCheckName = "下料基板检测信号";
        // static string RighTrackCheck = "基板上翘检测";
        static string CyClampName = "下料夹爪夹紧气缸电磁阀";
        static string CylPushUp = "下料推杆抬升电磁阀";

        static string CylClampUpcheck = "下料夹爪气缸上位置信号";
        static string CylClampDowncheck = "下料夹爪气缸下位置信号";

        static string CylPushUpcheck = "下料夹爪缓冲气缸上位置信号";
        static string CylPushDowncheck = "下料夹爪缓冲气缸下位置信号";
        static string CylProtect = "下料推杆缓冲防撞信号";
        public JFM_TrackUnLoad()
        {

        }
        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }
        char step = 'a';
        int PushCount = 0;
        double InStopPos = 0;
        double NeedPos = 0;
        double BackPos = 0;
        double MotorBackPos = 0;
        double ShouldBack = 0;
        double RightFactPos = 0;
       
        protected override bool ActionGenuine(out string errorInfo)
        {
            while (true)
            {
                if (1 == _workCmd)//当前为暂停状态
                {
                    _station.StopAxis(RighClampName);
                    _station.StopAxis(RighPushName);
                    Thread.Sleep(10);
                    continue;
                }
                else
                if (-1 == _workCmd)//指令退出
                {
                    _station.StopAxis(RighClampName);
                    _station.StopAxis(RighPushName);
                    step = 'a';
                    errorInfo = "收到退出指令";
                    _workCmd = 0;
                    _isRunning = false;
                    return false;
                }
                else
                if (0 == _workCmd)//指令退出
                {
                    double TrackTotalDis = (double)_station.GetCfgParamValue(RighClampWaitPos) - (double)_station.GetCfgParamValue(RighClampRightPos);
                    double LeftPos = (double)_station.GetCfgParamValue(RighClampWaitPos);
                    double RightPos = (double)_station.GetCfgParamValue(RighClampRightPos);
                    //double _FrameLongth = (double)_station.GetCfgParamValue(FrameLongth);

                    JFDLAFProductRecipe BoardRecipe = ((JFDLAFProductRecipe)JFHubCenter.Instance.RecipeManager.GetRecipe("Product",
                                        (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID")));
                    double _FrameLongth = BoardRecipe.FrameLength;
                    int delay = 10000;
                    errorInfo = "";
                    switch (step)
                    {
                        case 'a':
                            PushCount = 0;
                            step = 'b';
                            break;

                        case 'b':
                            //if (!Move(RighClampName, RighClampOpenPos, true, delay, out errorInfo))
                            //    return false;
                            if (!_station.SetDO(CyClampName, true, out errorInfo))
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
                            if (!Move(RighPushName, RighClampWaitPos, true, delay, out errorInfo))
                                return false;                            
                            step = 'd';
                            break;
                        case 'd':
                            //if (!Move(RighClampName, RighClampClosePos, true, delay, out errorInfo))
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
                            step = 'e';
                            break;
                        case 'e':
                            if (!Move(RighPushName, RighClampRightPos, true, delay, out errorInfo))
                                return false;
                            step = 'f';
                            break;
                        case 'f':
                            //if (!Move(RighClampName, RighClampOpenPos, true, delay, out errorInfo))
                            //    return false;
                            if (!_station.SetDO(CyClampName, true, out errorInfo))
                                return false;
                            if (!JFWaitDi(CylClampUpcheck, out errorInfo, true, 4000))
                            {
                                errorInfo = _station.Name + ":" + CylClampUpcheck + "获取DI状态失败！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            step = 'g';
                            break;
                        case 'g':
                            double MotorBack = RightPos - _FrameLongth;
                            if (!_station.MoveAxis(RighPushName, MotorBack, true, out errorInfo))
                                return false;
                            if (!JFMotionDone(RighPushName, out errorInfo, delay))
                                return false;
                            PushCount++;
                            step = 'h';
                            break;
                        case 'h':
                            //if (!Move(RighClampName, RighClampClosePos, true, delay, out errorInfo))
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

                            if (!_station.MoveAxis(RighPushName, RightPos, true, out errorInfo))
                                return false;
                            if (JFWaitDi(RightFrameCheckName, out errorInfo, true, 6000))
                            {
                                _station.StopAxis(RighPushName);
                                step = 'j';
                                break;
                            }
                          
                        // step = 'j';
                          break;

                        case 'j':
                            //if (!Move(RighClampName, RighClampOpenPos, true, delay, out errorInfo))
                            //    return false;
                            if (!_station.SetDO(CyClampName, true, out errorInfo))
                                return false;
                            if (!JFWaitDi(CylClampUpcheck, out errorInfo, true, 4000))
                            {
                                errorInfo = _station.Name + ":" + CylClampUpcheck + "获取DI状态失败！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            step = 'k';
                            break;

                        case 'k':
                            //_station.StopAxis(RighPushName);
                            if (!_station.GetAxisPosition(RighPushName, out InStopPos, out errorInfo))
                                return false;

                            // NeedPos = RightPos - InStopPos;
                            // ShouldBack = NeedPos + 185;
                            // BackPos = _FrameLongth - NeedPos;
                            // MotorBackPos = InStopPos- BackPos  - 5;
                            //MotorBackPos = InStopPos - _FrameLongth;
                            MotorBackPos = RightPos - _FrameLongth -50;
                            step = 'l';
                            break;
                        case 'l':
                            if (!_station.MoveAxis(RighPushName, MotorBackPos, true, out errorInfo))
                            {
                                errorInfo = _station.Name + ":" + RighPushName + "运动超时未完成！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            if (_station.WaitMotionDone(RighPushName, 10000) != JFWorkCmdResult.Success)
                            {
                                errorInfo = _station.Name + ":" + RighPushName + "运动超时未完成！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            step = 'm';
                            break;
                        case 'm':
                            //if (!Move(RighClampName, RighClampClosePos, true, delay, out errorInfo))
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
                            step = 'n';
                            break;
                        case 'n':
                            if (!_station.SetDO(CylPushUp, true, out errorInfo))
                                return false;
                            if (!JFWaitDi(CylPushUpcheck, out errorInfo, true, 4000))
                            {
                                errorInfo = _station.Name + ":" + CylPushUpcheck + "获取DI状态失败！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }

                            //if (!Move(RighPushName, RighClampRightPos, true, delay, out errorInfo))
                            //    return false;
                            if (!_station.MoveAxis(RighPushName, RightPos, true, out errorInfo))
                            {
                                errorInfo = _station.Name + ":" + RighPushName + "运动超时未完成！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            DateTime startTime = DateTime.Now;
                            do
                            {
                              if(JFWaitDi(CylProtect,out errorInfo,true,1))
                                {
                                    _station.StopAxis(RighPushName);
                                    errorInfo = _station.Name + ":" + CylProtect + "发生碰撞！";
                                    return false;
                                }
                               TimeSpan ts = DateTime.Now - startTime;
                               if (ts.TotalMilliseconds >= 5000)
                                {
                                    errorInfo = _station.Name + ":" + RighPushName + "运动超时未完成！"; ;
                                    return false;
                                }                                   
                               
                            } while (_station.WaitMotionDone(RighPushName, 1) != JFWorkCmdResult.Success);
                         
                            step = 'o';
                            
                            break;

                        case 'o':
                           
                            double backposition = (double)_station.GetCfgParamValue(RighClampRightPos) - 100;
                            if (!_station.MoveAxis(RighPushName, backposition, true, out errorInfo))
                            {
                                errorInfo = _station.Name + ":" + RighPushName + "运动超时未完成！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            if (_station.WaitMotionDone(RighPushName, 10000) != JFWorkCmdResult.Success)
                            {
                                errorInfo = _station.Name + ":" + RighPushName + "运动超时未完成！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }

                            if (!_station.SetDO(CylPushUp, false, out errorInfo))
                                return false;
                            if (!JFWaitDi(CylPushUpcheck, out errorInfo, false, 4000))
                            {
                                errorInfo = _station.Name + ":" + CylPushUpcheck + "获取DI状态失败！";
                                _workCmd = 0;
                                _isRunning = false;
                                return false;
                            }
                            step = 'a';
                            errorInfo = "Success";
                            return true;
                        default:
                            errorInfo = _station.Name + ":" + "Action 调用出错！";
                            return false;
                    }
                }
            }
            
          //  return true;
        }

        protected bool Move(string AxisName, string CfgParm, bool isAbs, int WaitTime, out string errInfo)
        {

            if (!_station.MoveAxis(AxisName, (double)_station.GetCfgParamValue(CfgParm), isAbs, out errInfo))
            {
                errInfo = _station.Name + ":" + AxisName + "运动超时未完成！";
                _workCmd = 0;
                _isRunning = false;
                return false;
            }

            if (!JFMotionDone(AxisName, out errInfo, WaitTime))
            {
                //errInfo = _station.Name + ":" + LeftClampName + "运动超时未完成！";
                _workCmd = 0;
                _isRunning = false;
                return false;
            }

            return true;
        }
        protected bool JFMotionDone(string AxisName, out string errorInfo, int delaytime)
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
                    _station.StopAxis(RighClampName);
                    _station.StopAxis(RighPushName);

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
                    _station.StopAxis(RighClampName);
                    _station.StopAxis(RighPushName);

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
        protected bool JFWaitDi(string diName, out string errorInfo, bool isTurnOn, int delaytime = -1)
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
                    Thread.Sleep(2);
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
