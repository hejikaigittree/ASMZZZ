using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using JFHub;
using JFUI;
using DLAF; // 包含DLAFRecipe定义

namespace DLAF_SingleTrack
{
    [JFDisplayName("单轨AOI上料轨道工站")]
    [JFVersion("1.0.0.0")]
    public class DLAF_FeedTracker:JFStationBase
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
            调节导轨宽度,
            移动到待机位置,
            等待进料信号, //等待料仓向轨道送料完成信号
            进料, //轨道（自身）向检测位置送料
            //等待检测完成,
            //等待出料信号,//等待下料仓准备好
            //出料,

        }

        public DLAF_FeedTracker()
        {

            Array acs = Enum.GetValues(typeof(STStatus));
            _allCS = new int[acs.Length];
            for (int i = 0; i < acs.Length; i++)
                _allCS[i] = (int)acs.GetValue(i);




            DeclearDevChn(NamedChnType.Axis, DevAN_AxisIntervel);
            DeclearDevChn(NamedChnType.Axis, DevAN_AxisFeed);
            DeclearDevChn(NamedChnType.Di, DevAN_DIFeedCldOpen);
            DeclearDevChn(NamedChnType.Do, DevAN_DOFeedCldCtrl);
            DeclearDevChn(NamedChnType.Di, DevAN_DIPieceInFeed);
            //DeclearDevChn(NamedChnType.Di, DevAN_DIPieceInDetect);
            DeclearDevChn(NamedChnType.Di, DevAN_DIPieceInEject); 

            DeclearCfgParam(JFParamDescribe.Create(SCN_FeedDistance,typeof(double),JFValueLimit.MinLimit,new object[] { 0}), "运行参数");
            DeclearCfgParam(JFParamDescribe.Create(SCN_PushLength, typeof(double), JFValueLimit.MinLimit, new object[] { 0 }), "运行参数");
            DeclearCfgParam(JFParamDescribe.Create(SCN_TrackOrgWidth, typeof(double), JFValueLimit.MinLimit, new object[] { 0 }), "运行参数");
            DeclearCfgParam(JFParamDescribe.Create(SCN_Piece2Hand, typeof(double), JFValueLimit.MinLimit, new object[] { 0 }), "运行参数"); 


            DeclearSPItemAlias(GPN_LoadReady, typeof(bool), false);
            DeclearSPItemAlias(GPN_DetectReady, typeof(bool), false);
            DeclearSPItemAlias(GPN_EjectReady, typeof(bool), false);
            DeclearSPItemAlias(GPN_DetectAvoid, typeof(bool), false);
            DeclearSPItemAlias(GPN_FeedOK, typeof(bool), false);
            DeclearSPItemAlias(GPN_FeedReady, typeof(bool), false); 
            DeclearSPItemAlias(GPN_PickDone, typeof(bool), false); 


            DeclearWorkPosition(WPN_Standby);
        }


        //Device Alias Name
        static string DevAN_AxisIntervel = "导轨间距Y轴";

        static string DevAN_AxisFeed = "进料X轴"; //半自动上料时使用
        static string DevAN_DIFeedCldOpen = "进料夹爪气缸打开到位"; //无信号时表示夹紧
        static string DevAN_DOFeedCldCtrl = "进料夹爪气缸控制"; //False表示夹紧
        static string DevAN_DIPieceInFeed = "进料口感应器";
        //static string DevAN_DIPieceInDetect = "检测位感应器";
        static string DevAN_DIPieceInEject = "出料口感应器";

        //Station config name
        static string SCN_FeedDistance = "上料总行程"; //料片从进料口移动到检测位置的总长
        static string SCN_PushLength = "单次夹料最大行程"; //上料电机单次推料的最大长度
        static string SCN_TrackOrgWidth = "轨道归零后宽度";
        static string SCN_Piece2Hand = "料片-推手距离"; //料仓上料完成后,料片

        //globle(system) data_pool variable 's Name
        static string GPN_LoadReady = "料仓上料完成";
        static string GPN_DetectReady = "检测站等待进料";
        static string GPN_EjectReady = "下料轨道允许上料";
        static string GPN_DetectAvoid = "检测Z轴避位完成";

        static string GPN_FeedReady= "准备好收料"; //当前工站已准备好接受料片
        static string GPN_PickDone = "从上料位取走工件";// 通知料仓已从上料位置取走工件

        static string GPN_FeedOK = " 上料完成"; //本工站上料完成信号 ，用于通知检测工站开始检测
        
        


        //Work Position's Name
        static string WPN_Standby = "待机位置"; //进料轴待机位置


        /// <summary>
        /// 系统配置项名称 
        /// </summary>
        string SYS_CurrentRecipeID = "CurrentID";//当前加工的产品配方ID 在系统配置中的项名称


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



        //执行结批动作
        protected override void ExecuteEndBatch()
        {
            
        }

        /// <summary>
        /// 打开/关闭 夹爪气缸
        /// </summary>
        /// <param name="isClamp"></param>
        /// <returns></returns>
        public bool Clamp(bool isClamp,out string errorInfo,int timeoutMilliseconds = 5000)
        {
            string errInfo;
            if (!SetDOAlias(DevAN_DOFeedCldCtrl, !isClamp, out errInfo))
            {
                errorInfo =  isClamp ? "夹紧" : "松开" + "夹爪气缸失败,ErrorInfo:" + errInfo;
                return false;
            }
            JFWorkCmdResult wcr = WaitDIAlias(DevAN_DIFeedCldOpen,  !isClamp, out errInfo, timeoutMilliseconds);
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

        /// <summary>
        /// 夹住工件送料（单次）
        /// </summary>
        /// <param name="dis">送料行程</param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool ClampFeedPiece(double dis,out string errorInfo,int timeoutMilliSeconds = -1)
        {
            if (!Clamp(true, out errorInfo))
                return false;
            if (!MoveAxisByAlias(DevAN_AxisFeed, dis, false, out errorInfo))
                return false;
            JFWorkCmdResult wcr = WaitMotionDoneByAlias(DevAN_AxisFeed, timeoutMilliSeconds);
            if (wcr != JFWorkCmdResult.Success)
                errorInfo = "等待轴送料运动完成失败，ret = " + wcr;

            if (!Clamp(false, out errorInfo))
                return false;

            //if (!MoveAxisByAlias(DevAN_AxisFeed, -dis, false, out errorInfo))
            //    return false;
            if (!MoveToWorkPosition(WPN_Standby, out errorInfo))
                return false;
            wcr = WaitMotionDoneByAlias(DevAN_AxisFeed, timeoutMilliSeconds);
            if (wcr != JFWorkCmdResult.Success)
            {
                errorInfo = "等待轴回程运动完成失败，ret = " + wcr;
                return false;
            }
            errorInfo = "Success";
            return true;
        }

        //任务流程中使用
        public void _ClampFeedPiece(double dis)
        {
            string errorInfo;
            if (!ClampFeedPiece(dis, out errorInfo))
                ExitWork(WorkExitCode.Error, "单程夹料失败，ErrorInfo:" + errorInfo);
            
        }


        /// <summary>
        /// 执行复位动作
        /// </summary>
        protected override void ExecuteReset()
        {
            ChangeCS(STStatus.复位);
            bool diSigON = false;
            string errorInfo;
            if (!OpenAllDevices(out errorInfo))
                ExitWork(WorkExitCode.Exception, errorInfo);
            if (!EnableAllAxis(out errorInfo))
                ExitWork(WorkExitCode.Exception, errorInfo);

            _Clamp(false);// 打开夹手

            //等待检测Z轴电机到达避位
            if (!WaitSPBoolByAliasName(GPN_DetectAvoid, true, out errorInfo))
                ExitWork(WorkExitCode.Error, "等待系统变量:" + GPN_DetectAvoid + " = true 失败:ErrorInfo:" + errorInfo);

            JFWorkCmdResult wcr = JFWorkCmdResult.UnknownError;
            if (!_isAxisIntervelHomed) //轴距电机归零
            {
                //先检测轨道上是否有残留工件
                if (!GetDIByAlias(DevAN_DIPieceInFeed, out diSigON, out errorInfo))
                    ExitWork(WorkExitCode.Exception, "获取进料口感应信号失败,Error:" + errorInfo);
                if(diSigON)
                    ExitWork(WorkExitCode.Exception, "工站复位失败:进料口有工件");


                //if (!GetDIByAlias(DevAN_DIPieceInDetect, out diSigON, out errorInfo))
                //    ExitWork(WorkExitCode.Exception, "获取检测位感应信号失败,Error:" + errorInfo);
                //if (diSigON)
                //    ExitWork(WorkExitCode.Exception, "工站复位失败:检测位置有工件");

                if (!GetDIByAlias(DevAN_DIPieceInEject, out diSigON, out errorInfo))
                    ExitWork(WorkExitCode.Exception, "获取出料口感应信号失败,Error:" + errorInfo);
                if (diSigON)
                    ExitWork(WorkExitCode.Exception, "工站复位失败:出料口有工件");




                if (!AxisHomeByAlias(DevAN_AxisIntervel, out errorInfo))
                    ExitWork(WorkExitCode.Error, "轨道间距轴Y归零失败:" + errorInfo);

                wcr = WaitAxisHomeDoneByAlias(DevAN_AxisIntervel);
                if(wcr != JFWorkCmdResult.Success)
                    ExitWork(WorkExitCode.Error, "等待轨道间距轴Y归零完成失败:ret = " + wcr);
                _isAxisIntervelHomed = true;
            }

            

            
            if (!AxisHomeByAlias(DevAN_AxisFeed, out errorInfo))
                ExitWork(WorkExitCode.Error, DevAN_AxisFeed + "归零失败:" + errorInfo);
            wcr = WaitAxisHomeDoneByAlias(DevAN_AxisFeed);
            if (wcr != JFWorkCmdResult.Success)
                ExitWork(WorkExitCode.Error, "等待" + DevAN_AxisFeed + "归零完成失败:ret = " + wcr);

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
            ChangeCS(STStatus.已停止);
            _ResetSig2Safe();
        }

        bool _isAxisIntervelHomed = false; //导轨间距Y轴是否归零过(程序开启后)
        bool _isAxisIntervelFitted = false;//已调整过轴间距
        //任务运行所需参数
        string _currPieceID = null; //当前Recipe
        double _trackIntervel = 0;//Recipe轨道宽度

        double _feedDistance = 0;//进料总行程
        double _singleTripLength = 0;//单次夹料的最大行程
        double _trackOrg = 0; //轨宽轴归零后的原始距离
        double _piece2HandEdge = 0;//料仓送料完成后，料片左边缘到夹手右边缘的距离

        /// <summary>
        /// 准备运行所需要的参数
        /// </summary>
        void _BuildTaskParam()
        {
            _currPieceID = JFHubCenter.Instance.SystemCfg.GetItemValue(SYS_CurrentRecipeID) as string;
            if (string.IsNullOrEmpty(_currPieceID))
                ExitWork(WorkExitCode.Exception, " 系统数据项:" + SYS_CurrentRecipeID + " 未设置");

            JFDLAFProductRecipe productRecipe = JFHubCenter.Instance.RecipeManager.GetRecipe("Product", _currPieceID) as JFDLAFProductRecipe;
            if (null == productRecipe)
                ExitWork(WorkExitCode.Exception, "产品配方:" + _currPieceID + " 不存在");

            if (string.IsNullOrEmpty(productRecipe.magezineBox))
                ExitWork(WorkExitCode.Exception, "产品配方:" + _currPieceID + " 未包含BoxID配置项");

            JFDLAFBoxRecipe boxRecipe = JFHubCenter.Instance.RecipeManager.GetRecipe("Box", /*productRecipe.magezineBox*/_currPieceID) as JFDLAFBoxRecipe;
            if (null == boxRecipe)
                ExitWork(WorkExitCode.Exception, "产品配方:" + _currPieceID + " BoxID:" + productRecipe.magezineBox + " 在配方管理器中不存在");

            //_trackIntervel = boxRecipe.FrameWidth;
            if (boxRecipe.FrameWidth <= 0)
                ExitWork(WorkExitCode.Exception, " BoxID:" + productRecipe.magezineBox + " 参数项FrameWidth = " + _trackIntervel + " 为非法值!");


            _feedDistance = (double)GetCfgParamValue(SCN_FeedDistance);
            if (_feedDistance <= 0)
                ExitWork(WorkExitCode.Exception, "运行参数:" + SCN_FeedDistance + " =" + _feedDistance + " 为非法值");

            _singleTripLength = (double)GetCfgParamValue(SCN_PushLength);
            if (_singleTripLength <= 0)
                ExitWork(WorkExitCode.Exception, "运行参数:" + SCN_PushLength + " =" + _singleTripLength + " 为非法值");

            _piece2HandEdge = (double)GetCfgParamValue(SCN_Piece2Hand);
            if (_piece2HandEdge <= 0)
                ExitWork(WorkExitCode.Exception, "运行参数:" + SCN_Piece2Hand + " =" + _piece2HandEdge + " 为非法值");

            _trackOrg = (double)GetCfgParamValue(SCN_TrackOrgWidth);
            if (_trackOrg <= 0)
                ExitWork(WorkExitCode.Exception, "运行参数:" + SCN_TrackOrgWidth + " =" + _trackOrg + " 为非法值");

            //检查待机点位是否设置
            JFMultiAxisPosition standbyPos = GetWorkPosition(WPN_Standby);
            string axisXName = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisFeed);
            if (string.IsNullOrEmpty(axisXName))
                ExitWork(WorkExitCode.Exception, "轴替身:" + DevAN_AxisFeed + " 未邦定全局轴名称");

            if (standbyPos.AxisNames.Length != 1 || !standbyPos.ContainAxis(axisXName))
                ExitWork(WorkExitCode.Exception, "进料待机点位设置错误!只能包含进料X轴");




        }

        /// <summary>
        /// 将所有信号置为安全状态
        /// </summary>
        void _ResetSig2Safe()
        {
            SetSPAliasValue(GPN_FeedOK, false); //将送料完成信号置为false
            SetSPAliasValue(GPN_PickDone, false);
            SetSPAliasValue(GPN_FeedReady, false);
        }

        
        //动作流程开始前的准备
        protected override void PrepareWhenWorkStart()
        {
            string errorInfo;
            if (!OpenAllDevices(out errorInfo))
                ExitWork(WorkExitCode.Exception, errorInfo);
            if(!EnableAllAxis(out errorInfo))
                ExitWork(WorkExitCode.Exception, errorInfo);
            _BuildTaskParam();

            if (!IsNeedResetWhenStart())
                ChangeCS(STStatus.等待检测Z轴避位);
        }

        //主动做流程
        protected override void RunLoopInWork()
        {
            string errorInfo;
            JFWorkCmdResult wcr = JFWorkCmdResult.UnknownError;
            bool diSigON = false;
            object obj = null;
            switch (CurrCS)
            {
                //case STStatus.已停止: 
                //    break;
                case STStatus.开始运行:
                    SendMsg2Outter("开始运行任务主流程");
                    _Clamp(false);//打开夹手气缸

                    //先检测轨道上是否有残留工件
                    if (!GetDIByAlias(DevAN_DIPieceInFeed, out diSigON, out errorInfo))
                        ExitWork(WorkExitCode.Exception, "获取进料口感应信号失败,Error:" + errorInfo);
                    if (diSigON)
                        ExitWork(WorkExitCode.Exception, "工站复位失败:进料口有工件");


                    //if (!GetDIByAlias(DevAN_DIPieceInDetect, out diSigON, out errorInfo))
                    //    ExitWork(WorkExitCode.Exception, "获取检测位感应信号失败,Error:" + errorInfo);
                    //if (diSigON)
                    //    ExitWork(WorkExitCode.Exception, "工站复位失败:检测位置有工件");

                    if (!GetDIByAlias(DevAN_DIPieceInEject, out diSigON, out errorInfo))
                        ExitWork(WorkExitCode.Exception, "获取出料口感应信号失败,Error:" + errorInfo);
                    if (diSigON)
                        ExitWork(WorkExitCode.Exception, "工站复位失败:出料口有工件");


                    ChangeCS(STStatus.等待检测Z轴避位);
                    break;
                case STStatus.等待检测Z轴避位: //归零时等待Z轴抬起后，再进行下一步动作
                    if (!WaitSPBoolByAliasName(GPN_DetectAvoid, true, out errorInfo))
                        ExitWork(WorkExitCode.Exception, errorInfo);
                    ChangeCS(STStatus.调节导轨宽度);
                    break;
                //case STStatus.复位: //主流程运行时不会进入
                //    break;


                case STStatus.调节导轨宽度:
                    string currRecipeIncfg = JFHubCenter.Instance.SystemCfg.GetItemValue(SYS_CurrentRecipeID) as string;
                    JFDLAFProductRecipe productRecipe = JFHubCenter.Instance.RecipeManager.GetRecipe("Product", currRecipeIncfg) as JFDLAFProductRecipe;
                    JFDLAFBoxRecipe boxRecipe = JFHubCenter.Instance.RecipeManager.GetRecipe("Box", /*productRecipe.magezineBox*/_currPieceID) as JFDLAFBoxRecipe;
                    if (_isAxisIntervelFitted) //已经调节过轨道宽度
                    {
                        if (currRecipeIncfg == _currPieceID)
                        {
                             if(_trackIntervel ==  boxRecipe.FrameWidth) //防止参数被修改
                            {
                                
                                ChangeCS(STStatus.移动到待机位置);
                                break;
                            }

                        }
                        
                    }
                    _trackIntervel = boxRecipe.FrameWidth;
                    _isAxisIntervelFitted = false;
                    SendMsg2Outter("开始调节轨道宽度到:" + _trackIntervel);
                    //开始调解轨道宽度
                    double axisTargetPos = _trackIntervel - _trackOrg; //轴实际位置为产品宽度-轴归零后的间距
                    if (!MoveAxisByAlias(DevAN_AxisIntervel, axisTargetPos, true, out errorInfo))
                        ExitWork(WorkExitCode.Exception, "移动轴:" + DevAN_AxisIntervel + " 失败,ErrorInfo:" + errorInfo);
                    wcr = WaitMotionDoneByAlias(DevAN_AxisIntervel);
                    if (JFWorkCmdResult.Success != wcr)
                        ExitWork(WorkExitCode.Exception, "等待轴运动完成失败,Ret = " + wcr);
                    _isAxisIntervelFitted = true;
                    //SetSPAliasValue(GPN_PickDone, false);
                    ChangeCS(STStatus.移动到待机位置);
                    break;

                case STStatus.移动到待机位置:
                    if (!MoveToWorkPosition(WPN_Standby, out errorInfo))
                        ExitWork(WorkExitCode.Exception, "进料X轴移动到待机位置失败:" + errorInfo);

                    if (!WaitToWorkPosition(WPN_Standby, out errorInfo))
                        ExitWork(WorkExitCode.Exception, "等待移动到待机位置失败，errorInfo:" + errorInfo);
                    
                    SetSPAliasValue(GPN_FeedReady, true);
                    ChangeCS(STStatus.等待进料信号);
                    break;
                case STStatus.等待进料信号: //等待料仓向轨道送料完成信号 && 检测工站等待进料 && 出料工站允许进料
                    
                    while(true)
                    {
                        CheckCmd(CycleMilliseconds);

                        //本站的送料完成信号必须为false，（防止送料完成后重复送料）
                        if (!GetSPAliasValue(GPN_FeedOK, out obj)) //上料仓工件已准备好
                            ExitWork(WorkExitCode.Exception, "获取变量=" + GPN_FeedOK + " 失败！");
                        if ((bool)obj)
                        {
                            CheckCmd(100);
                            continue;
                        }

                        if (!GetSPAliasValue(GPN_LoadReady, out obj)) //上料仓工件已准备好
                            ExitWork(WorkExitCode.Exception, "获取变量=" + GPN_LoadReady + " 失败！");
                        if(!(bool)obj)
                        {
                            CheckCmd(100);
                            continue;
                        }
                        
                        if (!GetSPAliasValue(GPN_DetectAvoid, out obj)) //Z轴是否以避位
                            ExitWork(WorkExitCode.Exception, "获取变量=" + GPN_DetectAvoid + " 失败！");
                        if (!(bool)obj)
                        {
                            CheckCmd(100);
                            continue;
                        }


                        if (!GetSPAliasValue(GPN_DetectReady, out obj)) //工站准备好检测
                            ExitWork(WorkExitCode.Exception, "获取变量=" + GPN_DetectReady + " 失败！");
                        if (!(bool)obj)
                        {
                            CheckCmd(100);
                            continue;
                        }

                        if (!GetSPAliasValue(GPN_EjectReady, out obj)) //出料站允许送料
                            ExitWork(WorkExitCode.Exception, "获取变量=" + GPN_EjectReady + " 失败！");
                        if (!(bool)obj)
                        {
                            CheckCmd(100);
                            continue;
                        }

                        break;

                    }

                    ChangeCS(STStatus.进料);
                    break;
                case STStatus.进料: //轨道（自身）向检测位置送料
                    SetSPAliasValue(GPN_FeedReady, false);
                    _Clamp(true); //下压夹爪
                    double clampDis = 0; //需要夹住工件往返运动的距离
                    double pushDis = 0; //最后一次退料的距离;
                    if(_feedDistance < _singleTripLength) //总行程
                    {
                        clampDis = _feedDistance; //只需要夹持一次就好
                    }
                    else
                    {
                        clampDis = _piece2HandEdge + 5; //预留退料间隙5mm
                        pushDis = _feedDistance - _piece2HandEdge;
                    }

                    while(clampDis > _singleTripLength)
                    {
                        //往返运动一次
                        SendMsg2Outter("夹料距离:" + _singleTripLength);
                        _ClampFeedPiece(_singleTripLength);
                        clampDis -= _singleTripLength;
                    }
                    if(clampDis >0) //最后一节不足一个行程的部分
                    {
                        SendMsg2Outter("夹料距离:" + clampDis);
                        _ClampFeedPiece(clampDis);

                    }


                    if(pushDis >0) //闭合夹爪，推料到位
                    {
                        SendMsg2Outter("推料距离:" + pushDis);
                        _Clamp(true);
                        if (!MoveAxisByAlias(DevAN_AxisFeed, pushDis, false, out errorInfo))
                            ExitWork(WorkExitCode.Exception, "推料失败，ErrorInfo:" + errorInfo);
                        wcr = WaitMotionDoneByAlias(DevAN_AxisFeed);
                        if (wcr != JFWorkCmdResult.Success)
                            ExitWork(WorkExitCode.Exception, "等待推料到位失败,ErrorInfo:" + errorInfo);

                    }




                    SetSPAliasValue(GPN_PickDone, true); //通知上料仓 ，工件已经取走
                    //打开夹爪
                    _Clamp(false);
                    //返回待机位置
                    if (!MoveToWorkPosition(WPN_Standby, out errorInfo))
                        ExitWork(WorkExitCode.Exception, "进料完成返回待机位置失败,ErrorInfo:" + errorInfo);
                    if(!WaitToWorkPosition(WPN_Standby,out errorInfo))
                        ExitWork(WorkExitCode.Exception, "进料完成后，等待返回待机位置失败,ErrorInfo:" + errorInfo);
                    //将送料完成信号置为true
                    SetSPAliasValue(GPN_FeedOK, true);
                    
                    SetSPAliasValue(GPN_FeedReady, true);//通知料仓，本站开始等待送料完成
                    ChangeCS(STStatus.等待进料信号);
                    break;
                default:
                    ExitWork(WorkExitCode.Exception, "工作流程中未命中的Custom-Status:" + CurrCS.ToString());
                    break;
            }
        }

        //动作流程推出前的清理
        protected override void CleanupWhenWorkExit()
        {
            StopAxisAlias(DevAN_AxisIntervel);
            StopAxisAlias(DevAN_AxisFeed);
            _ResetSig2Safe();            
        }


        public override JFRealtimeUI GetRealtimeUI()
        {
            return base.GetRealtimeUI();
        }

    }
}
