using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using JFHub;
using JFUI;

namespace DLAF_SingleTrack
{
    [JFDisplayName("单轨AOI下料轨道工站")]
    [JFVersion("1.0.0.0")]
    public class DLAF_EjectTracker:JFStationBase
    {
        /// <summary>
        /// 工站工作状态
        /// </summary>
        enum STStatus //single tracker's status
        {
            已停止,
            等待检测Z轴避位, //归零时等待Z轴抬起后，再进行下一步动作
            复位,
            开始运行,
            移动到待机位置,
            等待检测完成,
            等待下料仓准备好,//等待下料仓准备好
            出料, //轨道（自身）向检测位置送料

            
            //出料,

        }

        public DLAF_EjectTracker()
        {
            Array acs = Enum.GetValues(typeof(STStatus));
            _allCS = new int[acs.Length];
            for (int i = 0; i < acs.Length; i++)
                _allCS[i] = (int)acs.GetValue(i);


            DeclearDevChn(NamedChnType.Axis, DevAN_AxisEject);
            DeclearDevChn(NamedChnType.Di, DevAN_DIEjectCldOpen);
            DeclearDevChn(NamedChnType.Do, DevAN_DOEjectCldCtrl);
            DeclearDevChn(NamedChnType.Di, DevAN_DIPieceInEject);

            DeclearDevChn(NamedChnType.Do, DevAN_DOPushCldCtrl);
            DeclearDevChn(NamedChnType.Di, DevAN_DIPushCldUp);
            DeclearDevChn(NamedChnType.Di, DevAN_DIPushObstruct);





            DeclearCfgParam(JFParamDescribe.Create(SCN_EjectDistance, typeof(double), JFValueLimit.MinLimit, new object[] { 0 }), "运行参数");
            DeclearCfgParam(JFParamDescribe.Create(SCN_ClampLength, typeof(double), JFValueLimit.MinLimit, new object[] { 0 }), "运行参数");
            DeclearCfgParam(JFParamDescribe.Create(SCN_Piece2HandEdge, typeof(double), JFValueLimit.MinLimit, new object[] { 0 }), "运行参数");
            DeclearCfgParam(JFParamDescribe.Create(SCN_NormalSpd, typeof(double), JFValueLimit.MinLimit, new object[] { 0 }), "运行参数");
            DeclearCfgParam(JFParamDescribe.Create(SCN_PushSpd, typeof(double), JFValueLimit.MinLimit, new object[] { 0 }), "运行参数");

            


            DeclearSPItemAlias(GPN_UnloadReady, typeof(bool), false);
            DeclearSPItemAlias(GPN_DetectDone, typeof(bool), false);
            DeclearSPItemAlias(GPN_AllowedLoad, typeof(bool), false);
            DeclearSPItemAlias(GPN_EjectReady, typeof(bool), false);
            DeclearSPItemAlias(GPN_EjectDone, typeof(bool), false);
            DeclearSPItemAlias(GPN_DetectAvoid, typeof(bool), false); 

            DeclearWorkPosition(WPN_Standby);
            DeclearWorkPosition(WPN_EjectStart);

        }


        static string DevAN_AxisEject = "出料轴"; //半自动上料时使用
        static string DevAN_DIEjectCldOpen = "出料夹爪气缸张开到位"; //无信号时表示夹紧
        static string DevAN_DOEjectCldCtrl = "出料夹爪气缸控制"; //False表示夹紧
        static string DevAN_DIPieceInEject = "出料口感应器";

        static string DevAN_DOPushCldCtrl = "推杆顶升气缸";
        static string DevAN_DIPushCldUp = "推杆气缸上升到位";
        static string DevAN_DIPushObstruct = "推杆阻塞信号"; //此信号被触发表示下料时工件卡料

        static string SCN_EjectDistance = "下料总行程"; //料片从检测位置移动到料仓的总长
        static string SCN_ClampLength = "单次夹料最大行程"; //下料电机单次推料的最大长度
        static string SCN_Piece2HandEdge = "料手距离"; //料片左边缘到夹手右边缘的距离

        static string SCN_NormalSpd = "夹料运动速度";
        static string SCN_PushSpd = "推料进仓速度";


        //globle(system) data_pool variable 's Name
        static string GPN_UnloadReady = "下料仓准备收料";
        static string GPN_DetectDone = "视觉检测完成";
        static string GPN_AllowedLoad = "允许上料"; //与轨道上料工站的通讯信号
        static string GPN_DetectAvoid = "检测Z轴已到达";

        static string GPN_EjectReady = "准备出料";
        static string GPN_EjectDone = "出料完成";


        static string WPN_Standby = "待机位置";
        static string WPN_EjectStart = "送料起始位置";




        int[] _allCS = null;

        public override int[] AllCustomStatus
        {
            get
            {
                return _allCS;//return Enum.GetValues(typeof(STStatus)).
            }
        }

        public override string GetCustomStatusName(int status)
        {
            return ((STStatus)status).ToString();
        }



        /// <summary>
        /// 工站当前状态
        /// </summary>
        STStatus CurrCS
        {
            get { return (STStatus)CurrCustomStatus; }
        }

        void ChangeCS(STStatus cs)
        {
            if (CurrCustomStatus == (int)cs)
                return;
            ChangeCustomStatus((int)cs);
        }



        /// <summary>
        /// 打开/关闭 夹爪气缸
        /// </summary>
        /// <param name="isClamp"></param>
        /// <returns></returns>
        public bool Clamp(bool isClamp, out string errorInfo, int timeoutMilliseconds = 5000)
        {
            string errInfo;
            if (!SetDOAlias(DevAN_DOEjectCldCtrl, !isClamp, out errInfo))
            {
                errorInfo = isClamp ? "夹紧" : "松开" + "夹爪气缸失败,ErrorInfo:" + errInfo;
                return false;
            }
            JFWorkCmdResult wcr = WaitDIAlias(DevAN_DIEjectCldOpen, !isClamp, out errInfo, timeoutMilliseconds);
            if (wcr != JFWorkCmdResult.Success)
            {
                errorInfo = isClamp ? "夹紧" : "松开" + "夹爪气缸失败,ErrorInfo:" + errInfo;
                return false;
            }
            errorInfo = "Success";
            return true;
        }


        /// <summary>
        /// 打开/关闭 夹爪 工作线程中使用
        /// </summary>
        /// <returns></returns>
        void _Clamp(bool isClamp)
        {
            string errorInfo;
            if (!Clamp(isClamp, out errorInfo))
                ExitWork(WorkExitCode.Exception, errorInfo);

        }

        //上升/下降 推料气缸
        public bool OptPushCylinder(bool isUp,out string errorInfo,int timeoutMilliseconds = -1)
        {
            if (!SetDOAlias(DevAN_DOPushCldCtrl, isUp, out errorInfo))
                ExitWork(WorkExitCode.Exception, errorInfo);

            JFWorkCmdResult wcr = WaitDIAlias(DevAN_DIPushCldUp, isUp, out errorInfo, timeoutMilliseconds);
            if(wcr != JFWorkCmdResult.Success)
                ExitWork(WorkExitCode.Exception, errorInfo);
            errorInfo = "Success";
            return true;
        }

        //上升/下降 推料气缸 (在任务线程中使用)
        void _OptPushCylinder(bool isUp)
        {
            string errorInfo;
            if (!OptPushCylinder(isUp, out errorInfo, 5000))
                ExitWork(WorkExitCode.Exception, errorInfo);
        }


        /// <summary>
        /// 夹住工件送料（单次）
        /// </summary>
        /// <param name="dis">送料行程</param>
        /// <param name="errorInfo"></param先打开夹爪
        /// <returns></returns>
        public bool ClampEjectPiece(double dis, out string errorInfo, int timeoutMilliSeconds = -1)
        {
            if (!Clamp(false, out errorInfo)) //打开夹爪，
                return false;
            //先移动到送料起始位置
            if (!MoveToWorkPosition(WPN_EjectStart, out errorInfo))
                return false;
            JFWorkCmdResult wcr = WaitMotionDoneByAlias(DevAN_AxisEject, timeoutMilliSeconds);
            if (wcr != JFWorkCmdResult.Success)
            {
                errorInfo = "等待轴运动到抓料起始位置失败，ret = " + wcr;
                return false;
            }

            if (!Clamp(true, out errorInfo)) //闭合夹爪，
                return false;
            if (!MoveAxisByAlias(DevAN_AxisEject, dis, false, out errorInfo)) //移动
                return false;
            wcr = WaitMotionDoneByAlias(DevAN_AxisEject, timeoutMilliSeconds);
            if (wcr != JFWorkCmdResult.Success)
                errorInfo = "等待轴送料运动完成失败，ret = " + wcr;

            //打开夹爪
            if (!Clamp(false, out errorInfo))
                return false;

            //返回待机位置
            //if (!MoveToWorkPosition(WPN_Standby, out errorInfo))
            //    return false;
            //wcr = WaitMotionDoneByAlias(DevAN_AxisEject, timeoutMilliSeconds);
            //if (wcr != JFWorkCmdResult.Success)
            //{
            //    errorInfo = "等待轴运动到待机位置失败，ret = " + wcr;
            //    return false;
            //}

            errorInfo = "Success";
            return true;
        }

        //任务流程中使用
        public void _ClampEjectPiece(double dis)
        {
            string errorInfo;
            if (!ClampEjectPiece(dis, out errorInfo,5000))
                ExitWork(WorkExitCode.Error, "单程夹料失败，ErrorInfo:" + errorInfo);

        }




        //执行结批动作
        protected override void ExecuteEndBatch()
        {
            
        }


        /// <summary>
        /// 执行复位动作
        /// </summary>
        protected override void ExecuteReset()
        {
            string errorInfo;
            _Clamp(false); //打开夹爪
            _OptPushCylinder(false); //降下推料气缸
            //等待检测Z轴避位完成
            if (!WaitSPBoolByAliasName(GPN_DetectAvoid, true, out errorInfo))
                ExitWork(WorkExitCode.Exception, "等待检测Z轴避位信号失败:" + errorInfo);
            if (!AxisHomeByAlias(DevAN_AxisEject, out errorInfo))
                ExitWork(WorkExitCode.Exception, errorInfo);
            JFWorkCmdResult wcr = WaitAxisHomeDoneByAlias(DevAN_AxisEject);
            if (wcr != JFWorkCmdResult.Success)
                ExitWork(WorkExitCode.Exception, "等待轴：" + DevAN_AxisEject + " 归零运动失败，Ret = " + wcr);
            if (STWorkMode == StationThreadWorkMode.Normal)
                ChangeCS(STStatus.开始运行);
        }

        protected override void OnPause()
        {
            
        }

        protected override void OnResume()
        {
            
        }

        protected override void OnStop()
        {
            _ResetSig2Safe();
        }


        double _ejectDis = 0; //送料总行程
        double _clampLength = 0; //单次夹持距离
        double _piece2HandEdge = 0; //料片左边缘到夹手右边缘的距离
        double _normalSpd = 0; //正常运行速度
        double _pushSpd = 0; //推料入仓速度
        //载入并检查任务参数
        void _BuildTaskParam()
        {
            _ejectDis = (double)GetCfgParamValue(SCN_EjectDistance);
            if (_ejectDis <= 0)
                ExitWork(WorkExitCode.Exception, "运行参数:" + SCN_EjectDistance + " 值非法/未设置，Value = " + _ejectDis);

            _clampLength = (double)GetCfgParamValue(SCN_ClampLength);
            if (_clampLength <= 0)
                ExitWork(WorkExitCode.Exception, "运行参数:" + SCN_ClampLength + " 值非法/未设置，Value = " + _clampLength);

            _piece2HandEdge = (double)GetCfgParamValue(SCN_Piece2HandEdge);
            if (_piece2HandEdge <= 0)
                ExitWork(WorkExitCode.Exception, "运行参数:" + SCN_Piece2HandEdge + " 值非法/未设置，Value = " + _piece2HandEdge);

            _normalSpd = (double)GetCfgParamValue(SCN_NormalSpd);
            if (_normalSpd <= 0)
                ExitWork(WorkExitCode.Exception, "运行参数:" + SCN_NormalSpd + " 值非法/未设置，Value = " + _normalSpd);

            _pushSpd = (double)GetCfgParamValue(SCN_PushSpd);
            if (_pushSpd <= 0)
                ExitWork(WorkExitCode.Exception, "运行参数:" + SCN_PushSpd + " 值非法/未设置，Value = " + _pushSpd);



            string gAxisName = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisEject);
            //检查点位是否被正确设置
            if (!GetWorkPosition(WPN_EjectStart).ContainAxis(gAxisName)) //工站只包含一个轴
                ExitWork(WorkExitCode.Exception, "点位:" + WPN_EjectStart + " 坐标未设置");

            if (!GetWorkPosition(WPN_Standby).ContainAxis(gAxisName)) //工站只包含一个轴
                ExitWork(WorkExitCode.Exception, "点位:" + WPN_Standby + " 坐标未设置");


        }

        //将所有信号置为安全状态
        void _ResetSig2Safe()
        {
            SetSPAliasValue(GPN_AllowedLoad, false);
            SetSPAliasValue(GPN_EjectDone, false);
            SetSPAliasValue(GPN_EjectReady, false);
        }



        //动作流程开始前的准备
        protected override void PrepareWhenWorkStart()
        {
            string errorInfo;
            //打开所有设备
            if (!OpenAllDevices(out errorInfo))
                ExitWork(WorkExitCode.Exception, errorInfo);
            if(!EnableAllAxis(out errorInfo))
                ExitWork(WorkExitCode.Exception, errorInfo);
            //将信号置为安全状态
            _ResetSig2Safe();
            //检查运行参数
            _BuildTaskParam();
            if (!IsNeedResetWhenStart())
                ChangeCS(STStatus.开始运行);
            else
                ChangeCS(STStatus.复位);
        }

        //主动做流程
        protected override void RunLoopInWork()
        {
            string errorInfo;
            bool isSigON = false;
            JFWorkCmdResult wcr = JFWorkCmdResult.UnknownError;
            object obj = null;
            switch(CurrCS)
            {
                //case STStatus.已停止: //不会出现在此处的状态
                //    break;
                //case STStatus.复位://不会出现在此处的状态

                //    break;
                case STStatus.开始运行:
                    _Clamp(false); //打开夹手
                    _OptPushCylinder(false); //降下推料气缸
                    //检测出料口是否有工件
                    if (!GetDIByAlias(DevAN_DIPieceInEject, out isSigON, out errorInfo))
                        ExitWork(WorkExitCode.Exception, errorInfo);
                    if (isSigON)
                        ExitWork(WorkExitCode.Exception, "出料口感应器检测到有信号，请清理残留料片！");
                    ChangeCS(STStatus.等待检测Z轴避位);

                    break;
                case STStatus.等待检测Z轴避位: //归零时等待Z轴抬起后，再进行下一步动作
                    if (!WaitSPBoolByAliasName(GPN_DetectAvoid, true, out errorInfo))
                        ExitWork(WorkExitCode.Exception, errorInfo);
                    ChangeCS(STStatus.移动到待机位置);
                    break;
                case STStatus.移动到待机位置:
                    _Clamp(false); //打开夹手
                    _OptPushCylinder(false); //降下推料气缸
                    if (!SetAxisMPSpeedByAlias(DevAN_AxisEject, _normalSpd, out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);
                    if (!MoveToWorkPosition(WPN_Standby, out errorInfo))
                        ExitWork(WorkExitCode.Exception, errorInfo);
                    if (WaitToWorkPosition(WPN_Standby, out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);
                    SetSPAliasValue(GPN_AllowedLoad, true); //告诉轨道上料站可以上料
                    SetSPAliasValue(GPN_EjectReady, true); //告诉下料仓准备接收
                    ChangeCS(STStatus.等待检测完成);
                    break;
                case STStatus.等待检测完成:
                    SendMsg2Outter("等待检测工站视觉检测完成...");
                    if (!WaitSPBoolByAliasName(GPN_DetectDone, true, out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);
                    SetSPAliasValue(GPN_DetectDone, false);
                    SendMsg2Outter("等待检测Z轴到达避位...");
                    if(!WaitSPBoolByAliasName(GPN_DetectAvoid,true,out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);
                    ChangeCS(STStatus.等待下料仓准备好);
                    break;
                case STStatus.等待下料仓准备好:
                    SendMsg2Outter("等待下料仓准备好...");
                    if (!WaitSPBoolByAliasName(GPN_UnloadReady, true, out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);
                    ChangeCS(STStatus.出料);
                    break;
                case STStatus.出料: //轨道（自身）向检测位置送料
                    double clampDis = 0; //需要夹住工件往返运动的距离
                    double pushDis = 0; //最后一次退料的距离;
                    if (_ejectDis < _clampLength) //总行程
                    {
                        clampDis = _ejectDis; //只需要夹持一次就好
                    }
                    else
                    {
                        clampDis = _piece2HandEdge + 5; //预留退料间隙5mm
                        pushDis = _ejectDis - _piece2HandEdge;
                    }
                    if (!SetAxisMPSpeedByAlias(DevAN_AxisEject, _pushSpd, out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);
                    while (clampDis > _clampLength)
                    {
                        //往返运动一次
                        SendMsg2Outter("夹料距离:" + _clampLength);
                        _ClampEjectPiece(_clampLength);
                        clampDis -= _clampLength;
                    }
                    if (clampDis > 0) //最后一节不足一个行程的部分
                    {
                        SendMsg2Outter("夹料距离:" + clampDis);
                        _ClampEjectPiece(clampDis);

                    }


                    if (pushDis > 0) //闭合夹爪，推料到位
                    {
                        SendMsg2Outter("推料距离:" + pushDis);
                        _Clamp(false);
                        // 移动到推料起始位置
                        if (!MoveToWorkPosition(WPN_EjectStart, out errorInfo))
                            ExitWork(WorkExitCode.Error, errorInfo);
                        if(!WaitToWorkPosition(WPN_EjectStart,out errorInfo))
                            ExitWork(WorkExitCode.Error, errorInfo);
                        _OptPushCylinder(true); //抬起推料气缸
                        //设置推料速度
                        if(!SetAxisMPSpeedByAlias(DevAN_AxisEject,_pushSpd,out errorInfo))
                            ExitWork(WorkExitCode.Error, errorInfo);
                        if (!MoveAxisByAlias(DevAN_AxisEject, pushDis, false, out errorInfo))
                            ExitWork(WorkExitCode.Error, errorInfo);
                        SendMsg2Outter("开始推料入仓...");
                        while(true)
                        {
                            CheckCmd(CycleMilliseconds);
                            if(!GetDIByAlias(DevAN_DIPushObstruct,out isSigON,out errorInfo))
                                ExitWork(WorkExitCode.Error, errorInfo);
                            if (isSigON)
                                ExitWork(WorkExitCode.Exception, "推料入仓时卡料信号被触发...");
                            wcr = WaitMotionDoneByAlias(DevAN_AxisEject, CycleMilliseconds);
                            if (wcr == JFWorkCmdResult.Timeout)
                                continue;
                            else if (wcr == JFWorkCmdResult.Success)
                            {
                                SendMsg2Outter("推料入仓OK");
                                break;
                            }
                            else
                                ExitWork(WorkExitCode.Exception, "等待推料完成出错:ret = " + wcr);
                        }

                    }
                    _OptPushCylinder(false); //下降推料气缸
                    //回到待机位置
                    if (!MoveToWorkPosition(WPN_Standby, out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);
                    if(!WaitToWorkPosition(WPN_Standby,out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);

                    SetSPAliasValue(GPN_EjectDone,true); //通知下料仓 ， 送料完成
                    SetSPAliasValue(GPN_DetectDone, false);
                    SetSPAliasValue(GPN_AllowedLoad, true);
                    SetSPAliasValue(GPN_EjectReady, true);
                    ChangeCS(STStatus.等待检测完成);
                    break;
                default:
                    ExitWork(WorkExitCode.Exception, "工作流程中未命中的Custom-Status:" + CurrCS.ToString());
                    break;
            }
        }

        //动作流程推出前的清理
        protected override void CleanupWhenWorkExit()
        {
           
        }

    }
}
