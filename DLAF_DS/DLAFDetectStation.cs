using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using JFUI;
using JFHub;
using JFRecipe; //产品配方
using System.IO;
using DLAF;
using HalconDotNet;
using System.Threading;
using System.Collections.Concurrent;
using LFAOIRecipe;
using LFAOIReview;

namespace DLAF_DS
{

    /// <summary>
    /// 直线AOI设备的视觉检测工站
    /// </summary>
    [JFDisplayName("AOI检测工站")]
    [JFVersion("1.0.0.0")]
    public class DLAFDetectStation : JFStationBase, IJFRealtimeUIProvider, IDLAFProductFixReceiver, IDLAFInspectionReceiver
    {
        public static string SDN_CurrLotID = "DLAFCurrentLotID";
        #region 自定义消息类别   CMC = CustomizeMsgCategoty
        public static string CMC_ShowJFImage = "ShowJFImge"; //显示IJFImage对象
        public static string CMC_ShowHO = "ShowHObject";//显示Halcon对象
        public static string CMC_InspectResult = "InspectResult"; //Fov检测结果 , 后面的参数列表为 HObject/图像，FovResult
        public static string CMC_DieResults = "DieResult"; //Die检测结果 
        public static string CMC_StartNewPiece = "StartNewPiece"; //开始检测一片新料片，参数为PieceID(时间)
        public static string CMC_PieceFinished = "Piece Finished";//料片检测结束
        #endregion

        #region 工站自定义变量名称

        enum DSStatus   //Detect Station Status  检测工站的业务状态
        {
            已停止 = 0,
            初始化,
            开始时Z轴移动到避位, //开始运行时移动到最高位置
            开始时等待轨道避位完成, // 等待前后轨道都到达避位位置
            复位,                 //如需要复位，则进行复位动作
            开始运行,
            移动到待机位置,        //准备工作（复位）完成后 ，移动到待机位置
            等待进料,           //已移动到待机位置，等待上料工站送料完成
            定位矫正点1,         //向Mark1移动ing
            定位矫正点2,
            定位产品条码,
            检测中,
            移动到待机高度, ///检测流程完成后，先抬起一段距离避位
            返回待机位置//检测完成后，向待机位置移动
        }

        /// <summary>
        /// 改变当前业务逻辑状态
        /// </summary>
        /// <param name="status"></param>
        void _ChangeCS(DSStatus status)
        {
            if (CurrCustomStatus == (int)status)
                return;
            ChangeCustomStatus((int)status);
        }


        /// <summary>
        ///  当前业务逻辑状态
        /// </summary>
        DSStatus CurrCS { get { return (DSStatus)CurrCustomStatus; } }


        static string CategotyProduct = "Product";


        ///////////////工站声明的系统变量替身名 SPAN = System Pool-Data Item's Alias Name

        static string SPAN_TrackStationInAside0 = "开始时轨道工站避位完成"; //由轨道工站
        //static string SPAN_TrackDeliveryOK = "轨道工站送料完成";


        ///////////////////系统变量名称（非替身） SPAN = System Pool-Data Item's Name
        static string SPN_AxisZInAside0 = "开始运行时Z轴避位完成";
        static string SPN_WaitProductIn = "等待进料";
        //static string SPN_ProductReadyOut = "出料准备好";  改为单双轨工用工站后
        static string SPN_ProductOutCount = "出料个数";

        static string SPN_ReviewDone = "在线复判完成";


        /////工站自定义点位名称
        static string PosN_Standby = "待机位置"; //运行时相机的待机位置

        // 单轨AOI手动上料时使用的变量
        static string PosN_FeedStandby = "上料夹爪待机位置"; //半自动时
        static string PosN_FeedEnd = "上料夹爪送料到位位置";





        ////////////设备名称 Device Alias Name 
        static string DevAN_Cmr = "相机";
        static string DevAN_Light0 = "环光1";
        static string DevAN_Light1 = "环光2";
        static string DevAN_Light2 = "环光3";
        static string DevAN_Light3 = "环光4";
        static string DevAN_AxisX = "X轴";
        static string DevAN_AxisY = "Y轴";
        static string DevAN_AxisZ = "Z轴";
        static string DevAN_AxisFeed = "轨道上料轴"; //半自动上料时使用
        static string DevAN_DIFeedClose = "上料夹爪气缸闭合到位"; //无信号时表示夹紧
        static string DevAN_DOFeedCtrl = "上料夹爪气缸控制"; //False表示夹紧




        static string CfgName_VisionFixProduct = "使用视觉定位产品";
        static string CfgName_PictureFolder = "图片保存路径";
        static string CfgName_SavePicture = "保存测试图片";
        static string CfgName_PicSaveFormat = "图片保存格式";
        static string CfgName_InspectResultFolder = "检测结果保存路径";
        static string CfgName_ManualFeed = "人工上料模式";
        static string CfgName_DetectThreadCount = "并行检测任务数";

        static string CfgName_DeviceID = "设备ID"; //系统配置，设备当前ID


        static string CfgName_ImgWidth = "图像宽度/像素";   //任务参数
        static string CfgName_ImgHeight = "图像高度/像素";  //任务参数
        static string CfgName_CmrDataFilePath = "相机标定数据文件";

        static string CfgName_EnableReviewAtOnce = "检测后立即复判";
        static string CfgName_ReviewDBFolder = "Review数据库文件夹";
        static string CfgName_ReviewPicFolder = "Review图片文件夹";


        /////////////////单双轨配置
        static string CfgName_DeviceType = "设备类型"; //单轨/双轨
        
        static string PosN_TrackAMark1Snap = "轨道A_Mark1_拍照位置";
        static string PosN_TrackAMark2Snap = "轨道A_Mark2_拍照位置";
        static string PosN_TrackBMark1Snap = "轨道B_Mark1_拍照位置";
        static string PosN_TrackBMark2Snap = "轨道B_Mark2_拍照位置";

        //系统数据池变量
        static string SPN_Track1Ready = "轨道1工件已到位";
        static string SPN_Track2Ready = "轨道2工件已到位";

        static string SPN_Track1DetectDone = "轨道1检测完成";
        static string SPN_Track2DetectDone = "轨道2检测完成";
        #endregion


        public DLAFDetectStation()
        {
            PFRecipeID = null;
            PFErrorCode = -1;
            PFErrorInfo = "未定位";

            Array csVals = Enum.GetValues(typeof(DSStatus));
            _allCustomStatus = new int[csVals.Length];
            for (int i = 0; i < csVals.Length; i++)
                _allCustomStatus[i] = (int)csVals.GetValue(i);

            DeclearCfgParam(JFParamDescribe.Create(CfgName_DeviceType, typeof(string), JFValueLimit.Options,new object[] {"单轨","双轨" } ), "设备参数");//单轨/双轨






            DeclearCfgParam(JFParamDescribe.Create(CfgName_VisionFixProduct, typeof(bool), JFValueLimit.NonLimit, null), "任务参数");
            DeclearCfgParam(JFParamDescribe.Create(CfgName_PictureFolder, typeof(string), JFValueLimit.FolderPath, null), "任务参数");
            DeclearCfgParam(JFParamDescribe.Create(CfgName_SavePicture, typeof(string), JFValueLimit.Options, new string[] { "全部保存", "保存NG", "不保存" }), "任务参数");
            DeclearCfgParam(JFParamDescribe.Create(CfgName_InspectResultFolder, typeof(string), JFValueLimit.FolderPath, null), "任务参数");
            DeclearCfgParam(JFParamDescribe.Create(CfgName_PicSaveFormat, typeof(JFImageSaveFileType), JFValueLimit.NonLimit, null), "任务参数");
            DeclearCfgParam(JFParamDescribe.Create(CfgName_ManualFeed, typeof(bool), JFValueLimit.NonLimit, null), "任务参数");
            DeclearCfgParam(JFParamDescribe.Create(CfgName_DetectThreadCount, typeof(int), JFValueLimit.MinLimit| JFValueLimit.MaxLimit, new object[] { 1,10}), "任务参数");

            DeclearCfgParam(JFParamDescribe.Create(CfgName_ImgWidth, typeof(int), JFValueLimit.NonLimit, null), "产品定位参数");
            DeclearCfgParam(JFParamDescribe.Create(CfgName_ImgHeight, typeof(int), JFValueLimit.NonLimit, null), "产品定位参数");
            DeclearCfgParam(JFParamDescribe.Create(CfgName_CmrDataFilePath, typeof(string), JFValueLimit.FilePath, null), "产品定位参数");

            DeclearCfgParam(JFParamDescribe.Create(CfgName_EnableReviewAtOnce, typeof(bool), JFValueLimit.NonLimit, null), "Reviewe配置项");
            DeclearCfgParam(JFParamDescribe.Create(CfgName_ReviewDBFolder, typeof(string), JFValueLimit.FolderPath, null), "Reviewe配置项");
            DeclearCfgParam(JFParamDescribe.Create(CfgName_ReviewPicFolder, typeof(string), JFValueLimit.FolderPath, null), "Reviewe配置项");



            DeclearWorkPosition(PosN_Standby);
            
            DeclearWorkPosition(PosN_FeedStandby);//单轨手动上料时使用的点位参数
            DeclearWorkPosition(PosN_FeedEnd);//单轨手动上料时使用的点位参数

            DeclearWorkPosition(PosN_TrackAMark1Snap);//轨道1Mark1拍照位置
            DeclearWorkPosition(PosN_TrackAMark2Snap);//轨道1Mark2拍照位置
            DeclearWorkPosition(PosN_TrackBMark1Snap);
            DeclearWorkPosition(PosN_TrackBMark2Snap);

            DeclearDevChn(NamedChnType.Camera, DevAN_Cmr);//添加一个本地相机（替身名称）
            DeclearDevChn(NamedChnType.Light, DevAN_Light0);//添加一个本地光源（替身名称）
            DeclearDevChn(NamedChnType.Light, DevAN_Light1);//添加一个本地光源（替身名称）
            DeclearDevChn(NamedChnType.Light, DevAN_Light2);//添加一个本地光源（替身名称）
            DeclearDevChn(NamedChnType.Light, DevAN_Light3);//添加一个本地光源（替身名称）
            DeclearDevChn(NamedChnType.Axis, DevAN_AxisX);//添加一个本地设备（替身名称）
            DeclearDevChn(NamedChnType.Axis, DevAN_AxisY);//添加一个本地设备（替身名称）
            DeclearDevChn(NamedChnType.Axis, DevAN_AxisZ);//添加一个本地设备（替身名称）
            DeclearDevChn(NamedChnType.Axis, DevAN_AxisFeed);
            DeclearDevChn(NamedChnType.Di, DevAN_DIFeedClose);
            DeclearDevChn(NamedChnType.Do, DevAN_DOFeedCtrl);


            ///定义系统数据池变量
            DeclearSPItemAlias(SPN_AxisZInAside0, typeof(bool), false);
            DeclearSPItemAlias(SPAN_TrackStationInAside0, typeof(bool), false);
            DeclearSPItemAlias(SPN_WaitProductIn, typeof(bool), false);
            //DeclearSPItemAlias(SPN_ProductReadyOut, typeof(bool), false);
            //DeclearSPItemAlias(SPAN_TrackDeliveryOK, typeof(bool), false);
            DeclearSPItemAlias(SPN_ProductOutCount, typeof(int), 0);

            DeclearSPItemAlias(SPN_ReviewDone, typeof(bool), true);




            DeclearSPItemAlias(SPN_Track1Ready,typeof(bool),false);
            DeclearSPItemAlias(SPN_Track2Ready, typeof(bool), false);


            DeclearSPItemAlias(SPN_Track1DetectDone, typeof(bool), false);
            DeclearSPItemAlias(SPN_Track2DetectDone, typeof(bool), false);


            _ui.SetStation(this);
        }


        //打开并使能所有电机轴
        bool ServOnAllAxis(out string errorInfo)
        {
            if (!OpenChnDeviceAlias(NamedChnType.Axis, DevAN_AxisX, out errorInfo))
                return false;
            if (!OpenChnDeviceAlias(NamedChnType.Axis, DevAN_AxisY, out errorInfo))
                return false;
            if (!OpenChnDeviceAlias(NamedChnType.Axis, DevAN_AxisZ, out errorInfo))
                return false;
            if (!AxisServoByAlias(DevAN_AxisX, true, out errorInfo))
                return false;
            if (!AxisServoByAlias(DevAN_AxisY, true, out errorInfo))
                return false;
            if (!AxisServoByAlias(DevAN_AxisZ, true, out errorInfo))
                return false;

            bool isManualFeed = (bool)GetCfgParamValue(CfgName_ManualFeed); //采用人工上料模式
            if (isManualFeed)
            {
                if (!AxisServoByAlias(DevAN_AxisFeed, true, out errorInfo))
                    return false;
            }

            errorInfo = "Success";
            return true;
        }







        /// <summary>
        /// 所有可接收的自定义指令ID
        /// </summary>
        public override int[] AllCmds
        {
            get { return null; }
        }

        /// <summary>
        /// 获取自定义指令名称
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override string GetCmdName(int cmd)
        {
            return "Undefined";
        }



        int[] _allCustomStatus = null;
        /// <summary>
        /// 所有自定义状态ID
        /// </summary>
        public override int[] AllCustomStatus
        {
            get { return _allCustomStatus; }
        }

        JFStationRunMode _runMode = JFStationRunMode.Auto; //自动运行模式或单站调试模式
        public override JFStationRunMode RunMode { get { return _runMode; } }



        /// <summary>
        /// 获取工作状态名称
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public override string GetCustomStatusName(int status)
        {
            return ((DSStatus)status).ToString();
        }

        UcDetectStationRT _ui = new UcDetectStationRT();
        public override JFRealtimeUI GetRealtimeUI()
        {
            return _ui;
        }

        string _deviceType = null; //设备类型 “单轨”/“双轨”
        string _deviceID = null; //设备ID，用于向报表中记录检测数据

        string _gAxisNameX = null; //X轴真名
        string _gAxisNameY = null;
        string _gAxisNameZ = null;
        string _gCmrName = null; //相机全局名



        int _imgWidth = 0;
        int _imgHeight = 0;
        string _cmrCalibDataFilePath = null;


        InspectionDataEx _inspectDataEx = new InspectionDataEx();//用于向ReviewDB写数据的Key


        /// <summary>
        /// 检测Mark拍照点位是否被正确设置
        /// </summary>
        /// <param name="posName"></param>
        JFMultiAxisPosition _checkMarkSnapPos(string posName)
        {
            if (string.IsNullOrEmpty(posName))
                ExitWork(WorkExitCode.Exception, "Mark拍照位置点位未命名");

            string axisXName = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisX);
            string axisYName = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisY);
            if (string.IsNullOrEmpty(axisXName))
                ExitWork(WorkExitCode.Exception, "Mark拍照点X轴未定义");
            if (string.IsNullOrEmpty(axisYName))
                ExitWork(WorkExitCode.Exception, "Mark拍照点Y轴未定义");


            JFMultiAxisPosition ret= GetWorkPosition(posName);
            if(null == ret)
                ExitWork(WorkExitCode.Exception, "Mark拍照位置点位:" + posName + " 在点位配置中不存在");

            if (ret.Positions.Count != 2)
                ExitWork(WorkExitCode.Exception, "Mark拍照点轴数错误，AxisNumber = " + ret.Positions.Count);



            if (!ret.ContainAxis(axisXName))
                ExitWork(WorkExitCode.Exception, "Mark拍照点未包含X轴 =  " + axisXName);
            if(!ret.ContainAxis(axisYName))
                ExitWork(WorkExitCode.Exception, "Mark拍照点未包含Y轴 =  " + axisXName);

            return ret;
        }

        /// <summary>
        /// 生成任务运行参数 ， 根据产品ID，LotID，扫码模式 ， 视觉配置参数等 生成任务运行所需参数
        /// 如果失败，则退出工作流程
        /// </summary>
        void BuildTaskParam()
        {
            if(!JFHubCenter.Instance.SystemCfg.ContainsItem(CfgName_DeviceID))
            {
                JFHubCenter.Instance.SystemCfg.AddItem(CfgName_DeviceID, "", "设备参数");
                JFHubCenter.Instance.SystemCfg.Save();
            }



            _deviceID = JFHubCenter.Instance.SystemCfg.GetItemValue(CfgName_DeviceID) as string;
            if(string.IsNullOrEmpty(_deviceID))
                ExitWork(WorkExitCode.Error, "系统数据项:\"设备ID\" 未设置/空值");


            _deviceType = GetCfgParamValue(CfgName_DeviceType) as string;
            if(string.IsNullOrEmpty(_deviceType))
                ExitWork(WorkExitCode.Error, "数据项:\"设备类型\" 未设置/空值");

            _isManualFeed = (bool)GetCfgParamValue(CfgName_ManualFeed);//是否采用人工上料的方式


            //_currRecipeID = Convert.ToString(GetCfgParamValue(CfgName_CurrRecipeID));
            if (!JFHubCenter.Instance.SystemCfg.ContainsItem("CurrentID"))
            {
                ExitWork(WorkExitCode.Error, "数据项:\"CurrentID\"未定义");
                return;
            }
            _currRecipeID = JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID") as string;
            if (string.IsNullOrEmpty(_currRecipeID))
            {
                ExitWork(WorkExitCode.Error, "\"CurrentID\"未设置");
                return;
            }
            object obLotID;
            bool isOK = JFHubCenter.Instance.DataPool.GetItemValue(SDN_CurrLotID, out obLotID);
            _currLotID = obLotID as string;
            if (!isOK || string.IsNullOrEmpty(_currLotID))
            {
                ExitWork(WorkExitCode.Error, "产品批次号未设置");
            }

            //string errorInfo = "";
            IJFRecipeManager irm = JFHubCenter.Instance.RecipeManager;
            if (null == irm)
            {
                ExitWork(WorkExitCode.Error, "配方管理器未设置");
                return;
            }
            if (!irm.IsInitOK)
            {
                ExitWork(WorkExitCode.Error, "配方管理器未初始化");
                return;
            }
            JFDLAFRecipeManager recipeMgr = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            if (null == recipeMgr)
            {
                ExitWork(WorkExitCode.Error, "配方管理器类型错误:" + irm.GetType().Name);
                return;
            }
            string[] recipeIDs = recipeMgr.AllRecipeIDsInCategoty(CategotyProduct);
            if (null == recipeIDs || !recipeIDs.Contains(_currRecipeID))
            {
                ExitWork(WorkExitCode.Error, "CurrentID:" + _currRecipeID + " 在配方管理器中不存在");
                return;
            }

            string errorInfo;
            if (!_detectResultTransfer.SetRecipeLot(_currRecipeID,_currLotID,out errorInfo)) //Fov检测结果转化为Die结果的对象
                ExitWork(WorkExitCode.Error, errorInfo);

            //Review数据库初始化动作
            string _reviewDBFolder = GetCfgParamValue(CfgName_ReviewDBFolder) as string;
            if (string.IsNullOrEmpty(_reviewDBFolder))
                ExitWork(WorkExitCode.Error, CfgName_ReviewDBFolder + " 未设置！");
            if(!Directory.Exists(_reviewDBFolder))
                ExitWork(WorkExitCode.Error, CfgName_ReviewDBFolder + ":\"" + _reviewDBFolder + "\"文件夹不存在！");
            

            string _reviewPicFolder = GetCfgParamValue(CfgName_ReviewPicFolder) as string;
            if(string.IsNullOrEmpty(_reviewPicFolder))
                ExitWork(WorkExitCode.Error, CfgName_ReviewPicFolder + " 未设置！");
            if (!Directory.Exists(_reviewPicFolder))
                ExitWork(WorkExitCode.Error, CfgName_ReviewPicFolder + ":\"" + _reviewPicFolder + "\"文件夹不存在！");

            _currRecipe = recipeMgr.GetRecipe(CategotyProduct, _currRecipeID) as JFDLAFProductRecipe;

            DataManager.DataBaseDirectory = _reviewDBFolder;
            DataManager.FileSaveDirectory = _reviewPicFolder;
            LotInfo lotInfo = new LotInfo();
            lotInfo.ProductCode = _currRecipeID;
            lotInfo.LotName = _currLotID;
            lotInfo.Operator = "admin"; //临时代码，日后改为操作员ID
            lotInfo.Machine = _deviceID;
            lotInfo.TotalFrameCount = 10;    //??  数量存疑
            lotInfo.RowCount = _currRecipe.RowNumber;
            lotInfo.ColumnCount = _currRecipe.ColumnNumber*_currRecipe.BlockNumber;
            

            List<DefectTypeInfo> list_DefectType = new List<DefectTypeInfo>();
            int[] defectTypes = InspectNode.DieErrorTypes();
            foreach (int dt in defectTypes)
            {
                DefectTypeInfo di = new DefectTypeInfo();
                di.Index = dt;
                di.DefectType = InspectNode.DieErrorDescript(dt);
                list_DefectType.Add(di);
            }
            _inspectDataEx.LotIndex = DataManager.InitialNewLot(lotInfo, list_DefectType);
            DataManager.StartLot();

            ///获取Mark拍照参数
            _markSnapVcName1 = _currRecipe.GetMark1LightCfg();
            _markSnapVcName2 = _currRecipe.GetMark2LightCfg();
            if (string.IsNullOrEmpty(_markSnapVcName1))
                ExitWork(WorkExitCode.Error, "Mark1视觉参数未设置");
            if (string.IsNullOrEmpty(_markSnapVcName2))
                ExitWork(WorkExitCode.Error, "Mark2视觉参数未设置");


            


            _gAxisNameX = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisX);
            if (string.IsNullOrEmpty(_gAxisNameX))
                ExitWork(WorkExitCode.Error, "工站内X轴替身未指定全局名称");
            _gAxisNameY = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisY);
            if (string.IsNullOrEmpty(_gAxisNameY))
                ExitWork(WorkExitCode.Error, "工站内Y轴替身未指定全局名称");
            _gAxisNameZ = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisZ);
            if (string.IsNullOrEmpty(_gAxisNameZ))
                ExitWork(WorkExitCode.Error, "工站内Z轴替身未指定全局名称");

            _gCmrName = GetDecChnGlobName(NamedChnType.Camera, DevAN_Cmr);
            if (string.IsNullOrEmpty(_gCmrName))
                ExitWork(WorkExitCode.Error, "工站内相机替身未指定全局名称");






            JFVisionManager vm = JFHubCenter.Instance.VisionMgr;
            string[] vmNames = vm.AllSingleVisionCfgNames();

            //string errorInfo;


            bool isUserVisionFix = (bool)GetCfgParamValue(CfgName_VisionFixProduct);
            if(isUserVisionFix)
            {
                _imgWidth = (int)GetCfgParamValue(CfgName_ImgWidth);
                if (_imgWidth <= 0)
                    ExitWork(WorkExitCode.Error, "产品定位参数:图像宽度/像素值 未设置！");
                _imgHeight = (int)GetCfgParamValue(CfgName_ImgHeight);
                if(_imgHeight <=0)
                    ExitWork(WorkExitCode.Error, "产品定位参数:图像高度/像素值 未设置！");
                _cmrCalibDataFilePath = GetCfgParamValue(CfgName_CmrDataFilePath) as string;
                if (string.IsNullOrEmpty(_cmrCalibDataFilePath))
                    ExitWork(WorkExitCode.Error, "产品定位参数:相机标定数据文件 未设置！");
                if(!File.Exists(_cmrCalibDataFilePath))
                    ExitWork(WorkExitCode.Error, "产品定位参数:相机标定数据文件:\"" + _cmrCalibDataFilePath + "\"不存在！");
                if(!_prodVisionFixer.Init(_cmrCalibDataFilePath,_currRecipe, _imgWidth,_imgHeight,out errorInfo))
                    ExitWork(WorkExitCode.Error, "定位算子初始化失败:" + errorInfo);
            }
            else
            {

            }

            ///获取各Fov相对于ICCenter的偏移
            //_lstFovSnapPos.Clear();
            _taskVCNames.Clear();
            string[] fovNames = _currRecipe.FovNames();
            foreach (string s in fovNames)
            {
                _taskVCNames.Add(s, new Dictionary<string, string>());
                string[] taskNames = _currRecipe.TaskNames(s);
                foreach (string tn in taskNames)
                {
                    string vc = _currRecipe.VisionCfgName(s, tn);
                    if (string.IsNullOrEmpty(vc))
                        ExitWork(WorkExitCode.Error, "Fov:" + s + " Task:" + tn + " 视觉参数未设置");
                    if (null == vmNames || !vmNames.Contains(vc))
                        ExitWork(WorkExitCode.Error, "Fov:" + s + " Task:" + tn + " 视觉配置:" + vc + " 不存在");

                    _taskVCNames[s].Add(tn, vc);
                }

            }






            if (_lstICCenterSnapPos.Count() < _currRecipe.ICCount)
            {
                for (int i = _lstICCenterSnapPos.Count(); i < _currRecipe.ICCount; i++)
                    _lstICCenterSnapPos.Add(new JFMultiAxisPosition());
            }
            else if (_lstICCenterSnapPos.Count() > _currRecipe.ICCount)
            {
                int delCount = _lstICCenterSnapPos.Count() - _currRecipe.ICCount;
                _lstICCenterSnapPos.RemoveRange(_currRecipe.ICCount, delCount);
            }


            while (_lstFovOffset.Count() < _currRecipe.FovCount)
                _lstFovOffset.Add(new double[] { 0, 0 });
            if (_lstFovOffset.Count() > _currRecipe.FovCount)
                _lstFovOffset.RemoveRange(_lstFovOffset.Count(), _currRecipe.FovCount - _lstFovOffset.Count());



            _icRows = _currRecipe.RowCount; // 产品配方 IC行数
            _icCols = _currRecipe.ColCount; // 产品配方 IC列数
            _currRow = 0;
            _currCol = 0;
            _currFov = 0;
            _currTask = 0;
            _currFovName = "";
            _currTaskName = "";


            _pictureFolder = Convert.ToString(GetCfgParamValue(CfgName_PictureFolder));
            if (string.IsNullOrEmpty(_pictureFolder))
            {
                ExitWork(WorkExitCode.Error, "图片保存路径未设置");
                return;
            }

            _inspectResultFolder = Convert.ToString(GetCfgParamValue(CfgName_InspectResultFolder));
            if (string.IsNullOrEmpty(_inspectResultFolder))
            {
                _inspectResultFolder = System.Environment.CurrentDirectory + "\\检测结果";
            }



            //轨道A 或 单轨设备的Mark拍照点位
            _trackAMark1SnapPos = _checkMarkSnapPos(PosN_TrackAMark1Snap);
            _trackAMark2SnapPos = _checkMarkSnapPos(PosN_TrackAMark2Snap);

            if (_deviceType == "双轨")
            {
                _trackBMark1SnapPos = _checkMarkSnapPos(PosN_TrackBMark1Snap);
                _trackBMark2SnapPos = _checkMarkSnapPos(PosN_TrackBMark2Snap);
            }


        }


        /// <summary>
        /// 将工站所有信号置为安全状态，防止误触发其他工站流程
        /// </summary>
        void ResetMutualSignal()
        {
            SetSPAliasValue(SPN_WaitProductIn, true);
            //SetSPAliasValue(SPN_ProductReadyOut, false);
            SetSPAliasValue(SPN_ReviewDone, true);
            _isCurrPieceDetectResultSaved = true;

            SetSPAliasValue(SPN_Track1DetectDone, false); //单轨或轨道A
            if(_deviceType == "双轨")
                SetSPAliasValue(SPN_Track2DetectDone, false);//轨道B
        }

        /// <summary>
        /// 关闭所有光源
        /// </summary>
        void TurnoffAllLights()
        {
            if (null == LightChannelNames)
                return;
            foreach (string lightName in LightChannelNames)
                SetLightIntensity(lightName, 0, out string err);
        }

        /// <summary>
        /// 打开所有设备通到并使能
        /// 设备，光源，相机等
        /// </summary>
        void EnableAllDevices()
        {
            string errorInfo;
            if (!OpenAllDevices(out errorInfo)) //打开所有设备通道
                ExitWork(WorkExitCode.Error, errorInfo);
            ///使能所有轴
            if (!EnableAllAxis(out errorInfo))
                ExitWork(WorkExitCode.Error, errorInfo);

            //打开所有光源设备
            if (!OpenAllLightDev(out errorInfo))
                ExitWork(WorkExitCode.Error, errorInfo);

            //设置所有光源为开关模式
            string[] lightNames = LightChannelNames;
            if (null != lightNames)
                foreach (string ln in lightNames)
                    if (!SetLightTrigMode(ln, JFLightWithTrigWorkMode.TurnOnOff, out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);

            //打开所有相机设备
            if (!EnableAllCmrDev(true, out errorInfo))
                ExitWork(WorkExitCode.Error, errorInfo);

            //设置所有相机为软件采图模式
            string[] cmrNames = CameraNames;
            if (null != cmrNames)
                foreach (string cn in cmrNames)
                {
                    if (!SetCmrTrigMode(cn, JFCmrTrigMode.disable, out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);
                    if (!EnableCmrGrab(cn, true, out errorInfo))
                        ExitWork(WorkExitCode.Error, errorInfo);
                }
        }



        DLAFVisionFixer _prodVisionFixer = new DLAFVisionFixer(); //通过视觉定位产品的算子对象

        /// <summary>
        /// 工站主逻辑开始前的准备工作，只是在主流程开始前执行一次
        /// </summary>
        protected override void PrepareWhenWorkStart()
        {
            _ChangeCS(DSStatus.初始化);
            ResetMutualSignal();//先重置所有系统信号（防止误触发）
            BuildTaskParam();   //创建运行参数
            EnableAllDevices();
            TurnoffAllLights();


            _ChangeCS(DSStatus.开始时Z轴移动到避位);
            string errorInfo = "Unknown-Error";
            if (!AxisHomeByAlias(DevAN_AxisZ, out errorInfo))
                ExitWork(WorkExitCode.Error, CurrCS + "失败:" + errorInfo);
            if (JFWorkCmdResult.Success != WaitAxisHomeDoneByAlias(DevAN_AxisZ))
                ExitWork(WorkExitCode.Error, CurrCS + "失败:" + errorInfo);
            if (!AxisClearAlarmAlias(DevAN_AxisZ, out errorInfo))
                ExitWork(WorkExitCode.Error, "清除Z轴限位报警失败，ErrorInfo:" + errorInfo);

            if (!AxisServoByAlias(DevAN_AxisZ, true, out errorInfo))
                ExitWork(WorkExitCode.Error, "Z轴避位后重新上电失败，ErrorInfo:" + errorInfo);

            bool isManualFeed = (bool)GetCfgParamValue(CfgName_ManualFeed);
            if (isManualFeed) //如果是人工上料模式 ，
            {
                if (_deviceType == "单轨") //当前设备为单轨,打开送料夹手
                {


                    if (!IsNeedResetWhenStart())//如果不需要归零，将送料轴移动到准备位置
                    {
                        if (!MoveToWorkPosition(PosN_FeedStandby, out errorInfo))
                            ExitWork(WorkExitCode.Error, "送料夹手运动到待机位置失败，ErrorInfo:" + errorInfo);
                        SendMsg2Outter("送料夹手向待机位置移动");
                        if (!WaitToWorkPosition(PosN_FeedStandby, out errorInfo))
                            ExitWork(WorkExitCode.Error, "等待夹手到待机位置失败，ErrorInfo:" + errorInfo);

                    }
                }
                else
                {
                    //双轨的手动上料流程待添加
                }
            }
            DetectTaskParam dtp;
            while (_queDetectParams.TryDequeue(out dtp)) ;

            int threadCount = (int)GetCfgParamValue(CfgName_DetectThreadCount);
            
            _detectThreadRunningFlag = true;
            for (int i = 0; i < threadCount;i++)
            {
                Thread threadDetect = new Thread(_DetectThreadFunc);
                threadDetect.Start();
            }

            Thread threadTransData2Review = new Thread(_TransDieResultThreadFunc);
            threadTransData2Review.Start();

            if(!IsNeedResetWhenStart())
                _ChangeCS(DSStatus.开始运行);
            

            return;
        }

#region 任务参数 ， 每次运行前确定
        //bool _isSavePicture = false; //是否保存测试图片
        string _pictureFolder = null; //图片保存路径
        string _inspectResultFolder = null;


        string _currRecipeID = null;
        JFDLAFProductRecipe _currRecipe = null;

        string _currLotID = null;


        //////////////////////////轨道1Mark拍照位置
        /// Mark拍照参数
        JFMultiAxisPosition _trackAMark1SnapPos = null;//轨道A(或单轨) Mark1拍照位置
        JFMultiAxisPosition _trackAMark2SnapPos = null;//轨道A(或单轨) Mark2拍照位置


        JFMultiAxisPosition _trackBMark1SnapPos = null;//轨道A(或单轨) Mark1拍照位置
        JFMultiAxisPosition _trackBMark2SnapPos = null;//轨道A(或单轨) Mark2拍照位置







        /////////////////////////Mark拍照光学配置
        string _markSnapVcName1 = null;
        string _markSnapVcName2 = null;


        List<JFMultiAxisPosition> _lstICCenterSnapPos = new List<JFMultiAxisPosition>(); //通过计算获得的所有IC中心坐标，通过定位算法后确定
        //List<JFMultiAxisPosition> _lstFovSnapPos = new List<JFMultiAxisPosition>(); //各FOV拍照的实际坐标
        List<double[]> _lstFovOffset = new List<double[]>(); //各Fov相对于IC中心的偏移量（由定位算子计算得到）
        Dictionary<string, Dictionary<string, string>> _taskVCNames = new Dictionary<string, Dictionary<string, string>>();


        int _icRows = 0; // 产品配方 IC行数
        int _icCols = 0; // 产品配方 IC列数
#endregion



#region 任务当前状态
        int _currRow = 0;//当前正在检测的IC行
        int _currCol = 0;//当前正在检测的IC列
        int _currFov = 0;
        string _currFovName = "";
        int _currTask = 0;//当前Task
        string _currTaskName = "";
#endregion

        bool _isSavePicture(bool isInspectNG)
        {
            string saveMode = Convert.ToString(GetCfgParamValue(CfgName_SavePicture));
            if (string.IsNullOrEmpty(saveMode))
                return true;
            else if (saveMode == "全部保存")
                return true;
            else if (saveMode == "保存NG" && isInspectNG)
                return true;
            return false;
        }
        string _currPieceID = null;
        bool _isDetectTrack1 = false; //双轨设备,当前是否检测轨道A
        bool _isManualFeed = false;
        bool _isVisionFixProduct = false;//是否使用视觉定位



        IJFImage _markImg1, _markImg2; //Mark定位点图片
        /// <summary>
        /// 工站主流程 ，将会在内部循环中不断被调用，直到收到暂停/退出指令，或在RunLoopInWork函数内部调用退出函数时结束
        /// </summary>
        protected override void RunLoopInWork()
        {
            string errInfo = "Unknown";
            object obj = null;
            switch (CurrCS)
            {
                case DSStatus.开始运行: //只有在开始运行后进入一次
                    _ChangeCS(DSStatus.移动到待机位置); //
                    break;
                case DSStatus.移动到待机位置:       //准备工作（复位）完成后 ，移动到待机位置
                    if (!MoveToWorkPosition(PosN_Standby, out errInfo))
                        ExitWork(WorkExitCode.Error, "移动到:\"" + PosN_Standby + "\" 失败,错误信息:" + errInfo);
                    if (!WaitToWorkPosition(PosN_Standby, out errInfo))
                        ExitWork(WorkExitCode.Error, "等待移动到:\"" + PosN_Standby + "\" 失败,错误信息:" + errInfo);
                    if (!_isManualFeed)
                        SetSPAliasValue(SPN_WaitProductIn, true); //通知轨道工站可以进料
                    _ChangeCS(DSStatus.等待进料);
                    break;
                case DSStatus.等待进料:          //已移动到待机位置，等待上料工站送料完成
                    if (!_isCurrPieceDetectResultSaved) //等待上一料片的后端数据处理完成
                    {
                        SendMsg2Outter("等待保存料片检测结果完成");
                        while (!_isCurrPieceDetectResultSaved)
                            CheckCmd(CycleMilliseconds);
                        SendMsg2Outter("料片检测结果保存完成，开始检测");
                    }
                    _isCurrPieceDetectResultSaved = false;


                    if (_deviceType == "单轨")
                    {
                        if (!WaitSPBoolByAliasName(SPN_Track1Ready, true, out errInfo))
                            ExitWork(WorkExitCode.Error, errInfo);
                        if(_isManualFeed) //人工上料模式
                        {
                            if (!MoveToWorkPosition(PosN_FeedEnd, out errInfo))
                                ExitWork(WorkExitCode.Error, "送料轴运行到送料位置失败，ErrorInfo:" + errInfo);
                            SendMsg2Outter("送料轴开始向目标位置移动");
                            if (!WaitToWorkPosition(PosN_FeedEnd, out errInfo))
                                ExitWork(WorkExitCode.Error, "等待送料轴移动完成失败，ErrorInfo:" + errInfo);

                            if (!MoveToWorkPosition(PosN_FeedStandby, out errInfo))
                                ExitWork(WorkExitCode.Error, "送料轴运行到待机位置失败，ErrorInfo:" + errInfo);
                            SendMsg2Outter("送料轴开始向待机位置移动");
                            if (!WaitToWorkPosition(PosN_FeedEnd, out errInfo))
                                ExitWork(WorkExitCode.Error, "等待送料轴移动完成失败，ErrorInfo:" + errInfo);
                        }
                        if (_isVisionFixProduct) //使用视觉定位
                            _ChangeCS(DSStatus.定位矫正点1);
                        else
                            _ChangeCS(DSStatus.定位产品条码);
                    }
                    else //双轨设备 , 轮询AB轨道
                    {
                        while (true)
                        {
                            CheckCmd(CycleMilliseconds);
                            if (!GetSPAliasValue(SPN_Track1Ready, out obj))
                                ExitWork(WorkExitCode.Error, "获取系统变量失败:Name=" + SPN_Track1Ready);
                            if ((bool)obj)
                            {
                                _isDetectTrack1 = true;
                                break;
                            }

                            if (!GetSPAliasValue(SPN_Track2Ready, out obj))
                                ExitWork(WorkExitCode.Error, "获取系统变量失败:Name=" + SPN_Track2Ready);
                            if ((bool)obj)
                            {
                                _isDetectTrack1 = false;
                                break;
                            }
                        }
                        if (_isDetectTrack1)
                        {
                            SendMsg2Outter("轨道A工件已到位,准备检测轨道A产品");
                            //SetSPAliasValue(SPN_Track1Ready, false);
                        }
                        else
                        {
                            SendMsg2Outter("轨道B工件已到位,准备检测轨道B产品");
                            //SetSPAliasValue(SPN_Track2Ready, false);
                        }


                        if (_isManualFeed) //人工上料模式,此时应该是接到了界面发来的消息
                        {
                            //在此添加 添加轨道电机/气缸动作 ... ...
                        }

                        if (_isVisionFixProduct)
                            _ChangeCS(DSStatus.定位矫正点1);
                        else
                            _ChangeCS(DSStatus.定位产品条码);

                    }
                    break;
                case DSStatus.定位矫正点1:         //向Mark1移动ing
                    if (_deviceType == "单轨" || _isDetectTrack1) //
                    {
                        if (!MoveToPosition(_trackAMark1SnapPos, out errInfo))//if(!MoveToWorkPosition(PosN_MarkSnap1,out errInfo))
                            ExitWork(WorkExitCode.Error, errInfo);
                        if (!WaitToPosition(_trackAMark1SnapPos, out errInfo))//if(!WaitToWorkPosition(PosN_MarkSnap1, out errInfo))
                            ExitWork(WorkExitCode.Error, errInfo);
                    }
                    else
                    {
                        if (!MoveToPosition(_trackBMark1SnapPos, out errInfo))//if(!MoveToWorkPosition(PosN_MarkSnap1,out errInfo))
                            ExitWork(WorkExitCode.Error, errInfo);
                        if (!WaitToPosition(_trackBMark1SnapPos, out errInfo))//if(!WaitToWorkPosition(PosN_MarkSnap1, out errInfo))
                            ExitWork(WorkExitCode.Error, errInfo);
                    }

                    //调整Mark1视觉参数
                    if (!AdjustSingleVisionCfgAlias(DevAN_Cmr, _markSnapVcName1, out errInfo))
                        ExitWork(WorkExitCode.Error, errInfo);

                    
                    if (JFWorkCmdResult.Success != WaitSingleVisionCfgAdjustDone(_markSnapVcName1, out errInfo))
                        ExitWork(WorkExitCode.Error, errInfo);
                    

                    //Mark1拍照
                    if(null != _markImg1)
                    {
                        _markImg1.Dispose();
                        _markImg1 = null;
                    }
                    if (!SnapCmrImageAlias(DevAN_Cmr, out _markImg1, out errInfo))
                        ExitWork(WorkExitCode.Error, errInfo);
                    
                    NotifyCustomizeMsg(CMC_ShowJFImage, new object[] { _markImg1 });
                    _ChangeCS(DSStatus.定位矫正点2);
                    break;
                case DSStatus.定位矫正点2:
                    if (_deviceType == "单轨" || _isDetectTrack1) // 单轨设备 或双轨时检测A轨道
                    {
                        if (!MoveToPosition(_trackAMark2SnapPos, out errInfo))
                            ExitWork(WorkExitCode.Error, errInfo);
                        if (!WaitToPosition(_trackAMark2SnapPos, out errInfo))
                            ExitWork(WorkExitCode.Error, errInfo);
                    }
                    else
                    {
                        if (!MoveToPosition(_trackBMark2SnapPos, out errInfo))
                            ExitWork(WorkExitCode.Error, errInfo);
                        if (!WaitToPosition(_trackBMark2SnapPos, out errInfo))
                            ExitWork(WorkExitCode.Error, errInfo);
                    }

                    //调整视觉参数2
                    if (!AdjustSingleVisionCfgAlias(DevAN_Cmr, _markSnapVcName2, out errInfo))
                        ExitWork(WorkExitCode.Error, errInfo);

                    

                    if (JFWorkCmdResult.Success != WaitSingleVisionCfgAdjustDone(_markSnapVcName2, out errInfo))
                        ExitWork(WorkExitCode.Error, errInfo);
                    


                    //Mark2拍照
                    if(null != _markImg2)
                    {
                        _markImg2.Dispose();
                        _markImg2 = null;
                    }
                    if (!SnapCmrImageAlias(DevAN_Cmr, out _markImg2, out errInfo))
                        ExitWork(WorkExitCode.Error, errInfo);
                    
                    NotifyCustomizeMsg(CMC_ShowJFImage, new object[] { _markImg2 });
                    double markx1 = 0, marky1 = 0;
                    double markx2 = 0, marky2 = 0;
                    _currRecipe.GetMarkSnapPos1(out markx1, out marky1);
                    _currRecipe.GetMarkSnapPos2(out markx2, out marky2);

                    double[] icSnapCenterXs, icSnapCenterYs, fovOffsetXs, fovOffsetYs; //输出参数
                    if (!_prodVisionFixer.CalibProduct(new IJFImage[] { _markImg1, _markImg2 }, new double[] { markx1, markx2 }, new double[] { marky1, marky2 },
                                                      out icSnapCenterXs, out icSnapCenterYs, out fovOffsetXs, out fovOffsetYs, out errInfo))
                        ExitWork(WorkExitCode.Error, "产品定位失败:" + errInfo);

                    for (int i = 0; i < _icRows; i++)
                        for (int j = 0; j < _icCols; j++)
                        {
                            _lstICCenterSnapPos[i * _icCols + j].SetAxisPos(_gAxisNameX, icSnapCenterXs[i * _icCols + j]);
                            _lstICCenterSnapPos[i * _icCols + j].SetAxisPos(_gAxisNameY, icSnapCenterYs[i * _icCols + j]);
                        }
                    for (int i = 0; i < _currRecipe.FovCount; i++)
                    {
                        _lstFovOffset[i][0] = fovOffsetXs[i];
                        _lstFovOffset[i][1] = fovOffsetYs[i];
                    }

                    _ChangeCS(DSStatus.定位产品条码);
                    break;
                case DSStatus.定位产品条码:
                    //待添加扫码流程
                    _ChangeCS(DSStatus.检测中);
                    break;
                case DSStatus.检测中:
                    if (!_isVisionFixProduct)
                    {
                        if (_deviceType == "双轨")
                            ExitWork(WorkExitCode.Error, "当前为双轨设备,必须使用视觉定位产品坐标,请修改相关配置");
                        SendMsg2Outter("未使用视觉定位，将使用产品IC原始坐标拍照");
                        for (int i = 0; i < _icRows; i++)
                            for (int j = 0; j < _icCols; j++)
                            {
                                double x = 0, y = 0;
                                _currRecipe.GetICSnapCenter(i, j, out x, out y);
                                _lstICCenterSnapPos[i * _icCols + j].SetAxisPos(_gAxisNameX, x);
                                _lstICCenterSnapPos[i * _icCols + j].SetAxisPos(_gAxisNameY, y);
                            }
                        for (int i = 0; i < _currRecipe.FovCount; i++)
                        {
                            _lstFovOffset[i][0] = _currRecipe.ICFovOffsetX[i];
                            _lstFovOffset[i][1] = _currRecipe.ICFovOffsetY[i];
                        }
                    }



                    DateTime startTime = DateTime.Now; //开始时间
                    _currPieceID = startTime.ToString("yyyyMMdd-HH-mm-ss"); //日后可能改为扫码 ... 
                    _detectResultTransfer.SetPieceID(_currPieceID);
                    _inspectDataEx.FrameIndex = DataManager.InitialNewFrame(_currPieceID);
                    JFMultiAxisPosition fovSnapPos = new JFMultiAxisPosition();//当前Fov的拍照位置
                    NotifyCustomizeMsg(CMC_StartNewPiece, new object[] { _currPieceID });

                    _ChangeCS(DSStatus.检测中);
                    for (int i = 0; i < _icRows; i++) //行
                    {
                        CheckCmd(CycleMilliseconds);
                        _currRow = i;
                        for (int j = 0; j < _icCols; j++)//列
                        {
                            CheckCmd(CycleMilliseconds);
                            _currCol = j;
                            JFMultiAxisPosition currICPos = _lstICCenterSnapPos[i * _icCols + j];

                            for (int k = 0; k < _currRecipe.FovCount; k++) //Task拍照
                            {
                                CheckCmd(CycleMilliseconds);
                                _currFov = k;
                                _currFovName = _currRecipe.FovNames()[k];

                                fovSnapPos.SetAxisPos(_gAxisNameX, currICPos.GetAxisPos(_gAxisNameX) + _lstFovOffset[k][0]);
                                fovSnapPos.SetAxisPos(_gAxisNameY, currICPos.GetAxisPos(_gAxisNameY) + _lstFovOffset[k][1]);


                                SendMsg2Outter("移动到Row = " + i + "Col = " + j + " FovName:" + _currFovName);

                                string[] taskNames = _currRecipe.TaskNames(_currFovName);
                                if (!AdjustSingleVisionCfgAlias(DevAN_Cmr, _taskVCNames[_currFovName][taskNames[0]], out errInfo))
                                    ExitWork(WorkExitCode.Error, "调整视觉参数失败:VcName = " + _taskVCNames[_currFovName][taskNames[0]] + "ErrorInfo:" + errInfo);



                                if (!MoveToPosition(fovSnapPos, out errInfo))
                                {
                                    _ChangeCS(DSStatus.已停止);
                                    ExitWork(WorkExitCode.Error, errInfo);
                                }

                                if (!WaitToPosition(fovSnapPos, out errInfo))
                                {
                                    _ChangeCS(DSStatus.已停止);
                                    ExitWork(WorkExitCode.Error, errInfo);
                                }

                                List<IJFImage> lstTaskImages = new List<IJFImage>();
                                for (int h = 0; h < taskNames.Length; h++)
                                {
                                    CheckCmd(CycleMilliseconds);
                                    _currTask = h;
                                    _currTaskName = taskNames[_currTask];
                                    if (JFWorkCmdResult.Success != WaitSingleVisionCfgAdjustDone(_taskVCNames[_currFovName][_currTaskName], out errInfo))
                                        ExitWork(WorkExitCode.Error, "调整视觉参数失败:VcName = " + _taskVCNames[_currFovName][_currTaskName] + "ErrorInfo:" + errInfo);

                                    SendMsg2Outter("开始拍照 TaskName = " + _currTaskName);
                                    IJFImage img = null;
                                    if (!SnapCmrImageAlias(DevAN_Cmr, out img, out errInfo))
                                        ExitWork(WorkExitCode.Error, errInfo);
                                    NotifyCustomizeMsg(CMC_ShowJFImage, new object[] { img });
                                    lstTaskImages.Add(img);
                                    SendMsg2Outter(" 拍照完成");

                                    if (h < taskNames.Length - 1)
                                    {
                                        _currTaskName = taskNames[h + 1];
                                        SendMsg2Outter(" 开始调整视觉参数 Row = " + i + "Col = " + j + "FovName = " + _currFovName + "Task = " + _currTaskName);
                                        if (!AdjustSingleVisionCfgAlias(DevAN_Cmr, _taskVCNames[_currFovName][_currTaskName], out errInfo))
                                            ExitWork(WorkExitCode.Error, "调整视觉参数失败:VcName = " + _taskVCNames[_currFovName][_currTaskName] + "ErrorInfo:" + errInfo);
                                    }
                                }


                                DetectTaskParam dtp = new DetectTaskParam();
                                dtp.RecipeID = _currRecipeID;
                                dtp.ICRow = _currRow;
                                dtp.ICCol = _currCol;
                                dtp.FovName = _currFovName;
                                dtp.TaskNames = taskNames;
                                dtp.TaskImages = lstTaskImages.ToArray();
                                _queDetectParams.Enqueue(dtp);

                            }

                        }



                    }
                    bool isOnlineReview = (bool)JFHubCenter.Instance.SystemCfg.GetItemValue("在线复判");
                    if (isOnlineReview) //等待检测线程完成后,复盼完成
                    {
                        if (!WaitSPBoolByAliasName(SPN_ReviewDone, true, out errInfo))
                            ExitWork(WorkExitCode.Error, errInfo);
                        SetSPAliasValue(SPN_ReviewDone, false);
                    }
                    NotifyCustomizeMsg(CMC_PieceFinished, new object[] { _currRecipeID, _currLotID, _currPieceID, _inspectResultFolder, _pictureFolder });//料片检测结束

                    //移动到Z轴避位后通知出料工站
                    TurnoffAllLights();
                    _ChangeCS(DSStatus.移动到待机高度);
                    break;
                case DSStatus.移动到待机高度: ///检测流程完成后，先抬起一段距离避位
                    if (!MoveToWorkPosition(PosN_Standby, out errInfo))
                        ExitWork(WorkExitCode.Error, "出料时移动到:\"" + PosN_Standby + "\" 失败,错误信息:" + errInfo);
                    

                    if (!WaitToWorkPosition(PosN_Standby, out errInfo))
                        ExitWork(WorkExitCode.Error, "出料时等待移动到:\"" + PosN_Standby + "\" 失败,错误信息:" + errInfo);
                    
                    //SetSPAliasValue(SPN_ProductReadyOut, true);
                    //通知轨道工站开始出料
                    if (_deviceType == "单轨" || _isDetectTrack1)
                    {
                        SetSPAliasValue(SPN_Track1Ready, false);
                        SetSPAliasValue(SPN_Track1DetectDone, true);

                    }
                    else
                    {
                        SetSPAliasValue(SPN_Track2Ready, false);
                        SetSPAliasValue(SPN_Track2DetectDone, true);
                    }
                    _ChangeCS(DSStatus.返回待机位置);
                    break;
                case DSStatus.返回待机位置://检测完成后，向待机位置移动
                    if(_deviceType == "单轨")
                    {
                        if (!MoveToWorkPosition(PosN_Standby, out errInfo))
                            ExitWork(WorkExitCode.Error, "移动到:\"" + PosN_Standby + "\" 失败,错误信息:" + errInfo);
                        if (!WaitToWorkPosition(PosN_Standby, out errInfo))
                            ExitWork(WorkExitCode.Error, "等待移动到:\"" + PosN_Standby + "\" 失败,错误信息:" + errInfo);
                        if (!_isManualFeed)
                            SetSPAliasValue(SPN_WaitProductIn, true); //通知轨道工站可以进料
                        _ChangeCS(DSStatus.等待进料);
                        break;
                    }

                    _ChangeCS(DSStatus.等待进料); //双轨工站检测完成后没有动作,直接等轨道工站到位完成信号
                    break;
            }




       

           

            return;
        }


        /// <summary>
        /// 工站在退出主流程后会执行一次，然后工作线程关闭
        /// </summary>
        protected override void CleanupWhenWorkExit()
        {
            TurnoffAllLights();
            ResetMutualSignal();//置工站信号为安全状态
            _detectThreadRunningFlag = false;
            ///停止所有轴
            StopAxisAlias(DevAN_AxisX);
            StopAxisAlias(DevAN_AxisY);
            StopAxisAlias(DevAN_AxisZ);
            string errorInfo;
            string[] cmrNames = CameraNames;
            if (null != cmrNames)
                foreach (string cn in cmrNames)
                    if (!EnableCmrGrab(cn, false, out errorInfo))
                        SendMsg2Outter("关闭相机采集失败,CmrName:\"" + cn + "\" ErrorInfo:" + errorInfo);

            if (null != _markImg1)
            {
                _markImg1.Dispose();
                _markImg1 = null;
            }


            if(null != _markImg2)
            {
                _markImg2.Dispose();
                _markImg2 = null;
            }


            _ChangeCS(DSStatus.已停止);
            return;
        }

        /// <summary>
        /// 执行结批动作
        /// </summary>
        protected override void ExecuteEndBatch()
        {
            return;
        }

        /// <summary>
        /// 当工站线程运行时，受到暂停指令会执行OnPause(),然后进入暂停状态
        /// </summary>
        protected override void OnPause()
        {
            switch (CurrCS)
            {
                case DSStatus.开始时Z轴移动到避位://, //开始运行时移动到最高位置(正限位)
                    StopAxisAlias(DevAN_AxisZ);
                    break;
                case DSStatus.开始时等待轨道避位完成: // 等待前后轨道都到达避位位置
                    break;
                case DSStatus.复位:                 //如需要复位，则进行复位动作
                    break;
                case DSStatus.移动到待机位置:        //准备工作（复位）完成后 ，移动到待机位置
                    break;
                case DSStatus.等待进料:           //已移动到待机位置，等待上料工站送料完成
                    break;
                case DSStatus.定位矫正点1:         //向Mark1移动ing
                    break;
                case DSStatus.定位矫正点2:
                    break;
                case DSStatus.定位产品条码:
                    break;
                case DSStatus.检测中:
                    break;
                case DSStatus.移动到待机高度: ///检测流程完成后，先抬起一段距离避位
                    break;
                case DSStatus.返回待机位置://检测完成后，向待机位置移动
                    break;
            }
        }

        /// <summary>
        /// 当工站线程运行并且当前状态处于暂停时，受到恢复运行指令会执行OnResume()
        /// </summary>
        protected override void OnResume()
        {
            string errorInfo = "";
            switch (CurrCS)
            {
                case DSStatus.开始时Z轴移动到避位://, //开始运行时移动到最高位置(正限位)
                    if (!MoveVelAxisByAlias(DevAN_AxisZ, true, out errorInfo))
                        ExitWork(WorkExitCode.Error, "恢复运行失败:" + errorInfo);
                    break;
                case DSStatus.开始时等待轨道避位完成: // 等待前后轨道都到达避位位置
                    break;
                case DSStatus.复位:                 //如需要复位，则进行复位动作
                    break;
                case DSStatus.移动到待机位置:        //准备工作（复位）完成后 ，移动到待机位置
                    break;
                case DSStatus.等待进料:           //已移动到待机位置，等待上料工站送料完成
                    break;
                case DSStatus.定位矫正点1:         //向Mark1移动ing
                    break;
                case DSStatus.定位矫正点2:
                    break;
                case DSStatus.定位产品条码:
                    break;
                case DSStatus.检测中:
                    break;
                case DSStatus.移动到待机高度: ///检测流程完成后，先抬起一段距离避位
                    break;
                case DSStatus.返回待机位置://检测完成后，向待机位置移动
                    break;
            }
        }

        /// <summary>
        /// 当前工站正在运行（包括暂停状态）收到退出指令会执行OnStop()
        /// </summary>
        protected override void OnStop()
        {
            _detectThreadRunningFlag = false;
            TurnoffAllLights();
            ResetMutualSignal();//置工站信号为安全状态
            ///停止所有轴
            StopAxisAlias(DevAN_AxisX);
            StopAxisAlias(DevAN_AxisY);
            StopAxisAlias(DevAN_AxisZ);

            DataManager.EndLot();
        }

        protected override void ExecuteReset()
        {
            _ChangeCS(DSStatus.复位);
            string errorInfo = "Unknown-Error";
            if (!ServOnAllAxis(out errorInfo))
            {
                ExitWork(WorkExitCode.Error, "工站归零失败:" + errorInfo);
                return;
            }


            //Z轴归零
            if (!AxisHomeByAlias(DevAN_AxisZ, out errorInfo))
                ExitWork(WorkExitCode.Error, "工站归零失败: " + errorInfo);
            WaitAxisHomeDoneByAlias(DevAN_AxisZ);

            bool isManualFeed = (bool)GetCfgParamValue(CfgName_ManualFeed);
            if (isManualFeed)
            {
                ////先打开夹爪 ， 再归零送料轴
                //if (!SetDOAlias(DevAN_DOFeedCtrl, true, out errorInfo))
                //    ExitWork(WorkExitCode.Error, "松开送料夹手失败，ErrorInfo:" + errorInfo);
                //SendMsg2Outter("夹手气缸松开,等待松开到位信号");
                ////等待夹手气缸松开到位
                //if (JFWorkCmdResult.Success != WaitDIAlias(DevAN_DIFeedClose, true, out errorInfo, 5000))
                //    ExitWork(WorkExitCode.Error, "等待松开到位信号失败，ErrorInfo:" + errorInfo);
                //SendMsg2Outter("夹手气缸已松开到位");

                if (!AxisHomeByAlias(DevAN_AxisFeed, out errorInfo))
                    ExitWork(WorkExitCode.Error, "工站归零失败: " + errorInfo);
                WaitAxisHomeDoneByAlias(DevAN_AxisFeed);

            }



            //Y轴先归零，
            if (!AxisHomeByAlias(DevAN_AxisY, out errorInfo))
                ExitWork(WorkExitCode.Error, "工站归零失败: " + errorInfo);

            WaitAxisHomeDoneByAlias(DevAN_AxisY);



            //X轴归零
            if (!AxisHomeByAlias(DevAN_AxisX, out errorInfo))
                ExitWork(WorkExitCode.Error, "工站归零失败: " + errorInfo);
            WaitAxisHomeDoneByAlias(DevAN_AxisX);


            if (!MoveToWorkPosition(PosN_FeedStandby, out errorInfo))
                ExitWork(WorkExitCode.Error, "送料夹手运动到待机位置失败，ErrorInfo:" + errorInfo);
            SendMsg2Outter("送料夹手向待机位置移动");
            if (!WaitToWorkPosition(PosN_FeedStandby, out errorInfo))
                ExitWork(WorkExitCode.Error, "等待夹手到待机位置失败，ErrorInfo:" + errorInfo);
            //if (!SetDOAlias(DevAN_DOFeedCtrl, false, out errorInfo))
            //    ExitWork(WorkExitCode.Error, "闭合送料夹手失败，ErrorInfo:" + errorInfo);
            //SendMsg2Outter("等待夹手闭合到位");
            //if (JFWorkCmdResult.Success != WaitDIAlias(DevAN_DIFeedClose, false, out errorInfo, 5000))
            //    ExitWork(WorkExitCode.Error, "等待夹手闭合到位失败，ErrorInfo:" + errorInfo);
            //SendMsg2Outter("夹手已闭合到位");




            if (RunMode == JFStationRunMode.Auto) //自动连续运行
            {
            }
            else //手动调试运行
            {

            }

            if (STWorkMode == StationThreadWorkMode.Normal)//正常工作模式
                _ChangeCS(DSStatus.开始运行);


        }




        public override bool SetRunMode(JFStationRunMode runMode)
        {
            if (IsWorking())
                return false;
            _runMode = runMode;
            return true;
        }





#region IDLAFProductFixer's implement
        public string PFRecipeID { get; set; }
        public int PFErrorCode { get; set; }
        public string PFErrorInfo { get; set; }
        public double[] PFICCenterX { get; set; }
        public double[] PFICCenterY { get; set; }
        public double[] PFFovOffsetX { get; set; }
        public double[] PFFovOffsetY { get; set; }
#endregion


#region IDLAFInspectionReceiver's Implement
        /// <summary>
        /// 已检测的RecipeID
        /// </summary>
        public string InspectedRecipeID { get; set; }

        /// <summary>
        /// 已检测的IC行数
        /// </summary>
        public int InspectedICRow { get; set; }

        /// <summary>
        /// 已检测的IC列数
        /// </summary>
        public int InspectedICCol { get; set; }

        /// <summary>
        /// 已检测的FOV名称
        /// </summary>
        public string InspectedFovName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool InspectedResult { get; set; }


        public List<int[]> InspectedDiesErrorCodes { get; set; }

        public string[] InspectedDieErrorInfos { get; set; }

        /// <summary>
        ///  检测错误信息
        /// </summary>
        public string InspectedErrorInfo { get; set; }

        public HObject InspectedBondContours { get; set; }
        public HObject InspectedWires { get; set; }
        public HObject InspectedFailRegs { get; set; }


        /// <summary>
        /// 其他可能存在的检测结果
        /// </summary>
        public Dictionary<string, object> InspectedReverse { get; set; }
#endregion


         
        ConcurrentQueue<DetectTaskParam> _queDetectParams = new ConcurrentQueue<DetectTaskParam>(); //检测任务所需输入参数队列

        /// <summary>
        /// 当前料片的检测结果是否保存OK
        /// 采图线程必须等待当前料片结果保存完成才能进行下一片料的检测
        /// </summary>
        bool _isCurrPieceDetectResultSaved = true;
        /// <summary>
        /// 检测任务参数
        /// </summary>
        internal class DetectTaskParam
        {
            internal string RecipeID = null;
            internal int ICRow = -1; //扫描点位行数
            internal int ICCol = -1; //扫描点位列数
            internal string FovName = null;//Fov
            internal string[] TaskNames = null;
            internal IJFImage[] TaskImages = null;
            internal HObject[] TaskRegions = null; //Task图片中的检测区域
        }



        /// <summary>
        /// 将Fov结果保存为DieResut的转化器
        /// </summary>
        DLAFDetectResultTransfer _detectResultTransfer = new DLAFDetectResultTransfer();

        bool _detectThreadRunningFlag = false;
        void _DetectThreadFunc()
        {
            DetectTaskParam dtp = null;
            while (_detectThreadRunningFlag)
            {
                if (!_queDetectParams.TryDequeue(out dtp))
                {
                    Thread.Sleep(200);
                    continue;
                }
                DlafFovDetectResult fdr = new DlafFovDetectResult();
                //dtr.RecipeID = dtp.RecipeID;
                fdr.FovName = dtp.FovName;
                fdr.ICRow = dtp.ICRow;
                fdr.ICCol = dtp.ICCol;
                fdr.TaskNames = dtp.TaskNames;

                HObject[] hoImgs = new HObject[dtp.TaskImages.Length];
                for(int i = 0; i < hoImgs.Length;i++)
                {
                    object oi;
                    int nOpt = dtp.TaskImages[i].GenHalcon(out oi);
                    if (nOpt != 0)
                        throw new Exception("图像检测任务出错,IJFImage.GenHalcon() Error:" + dtp.TaskImages[i].GetErrorInfo(nOpt));
                    hoImgs[i] = oi as HObject;

                }

                do
                {
                    InspectNode inspNode = JFDLAFInspectionManager.Instance.GetInspectNode(dtp.RecipeID, dtp.FovName);
                    if (null == inspNode)
                    {
                        fdr.IsDetectSuccess = false;
                        fdr.DetectErrorInfo = "视觉算子未初始化";
                        SendMsg2Outter("图像检测任务出错:RecipeID = \"" + dtp.RecipeID + "\" Fov = \"" + dtp.FovName + "\" 视觉算子未初始化");
                        throw new Exception("视觉算子未初始化，视觉检测线程退出！");//_queDetectResult.Enqueue(fdr);
                    }

                    string errorInfo;
                    //检测输入参数
                    HObject dieRegions;
                    int currDieRowCount, currDieColCount;
                    int[] dieRows, dieCols;
                    if (!_prodVisionFixer.GetDetectRegionInFov(fdr.ICRow, fdr.ICCol, out dieRegions, out dieRows, out dieCols,out currDieRowCount,out currDieColCount,out errorInfo))
                        ExitWork(WorkExitCode.Error, "GetDetectRegionInFov failed,ErrorInfo:" + errorInfo);
                    fdr.DetectDiesRows = dieRows;
                    fdr.DetectDiesCols = dieCols;
                    fdr.CurrRowCount = currDieRowCount;
                    fdr.CurrColCount = currDieColCount;

                    //检测输出参数
                    string detectErrorInfo; //算子执行信息
#if true   //暂时保留，待数据库使用新数据结构后停止使用
                    List<int[]> diesErrorCodes; //= new List<int[]>();
                    List<string[]> diesErrorTaskNames;

                    List<HObject[]> diesFailedRegions;
                    List<string[]> diesErrorDetails; //错误的详细描述信息
                    Dictionary<string, HObject> detectItems; //算子中输出的检测项（BondContours/WireRegions 等等）
                    Dictionary<string, string> detectItemTaskNames;


                    bool isDetectOK = inspNode.InspectImage(hoImgs, dtp.TaskNames, dieRegions,
                                                                out detectErrorInfo, out diesErrorCodes, out diesErrorTaskNames, out diesFailedRegions, out diesErrorDetails,
                                                                out detectItems, out detectItemTaskNames);
#endif
                    List<InspectResultItem[]> diesResults = null;
                    isDetectOK = inspNode.InspectImage(hoImgs, dieRegions, out detectErrorInfo, out diesResults);



                    int dieCountInfov = dieRegions.CountObj();
                    //裁剪后的图象（按照Die的检测区域）
                    HObject[] dieImgs = new HObject[dieCountInfov];
                    HTuple _row1, _col1, _row2, _col2;
                    HTuple TWidth, THeight;
                    for (int i = 0; i < dieCountInfov; i++)
                    {
                        HOperatorSet.SmallestRectangle1(dieRegions.SelectObj(i+1), out _row1, out _col1, out _row2, out _col2);
                        if (_row1 < 0) _row1 = 0;
                        if (_col1 < 0) _col1 = 0;
                        HOperatorSet.GetImageSize(hoImgs[0], out TWidth, out THeight);
                        if (_row2 > THeight) _row2 = THeight;
                        if (_col2 > TWidth) _row2 = TWidth;

                        HObject hoImages = null; //一颗Die的多通道图
                        for (int j = 0; j < hoImgs.Length; j++)
                        {
                            HObject ho;
                            HOperatorSet.CropRectangle1(hoImgs[j], out ho, _row1, _col1, _row2, _col2);
                            if (null == hoImages)
                                hoImages = ho.Clone();
                            else
                            {
                                HOperatorSet.ConcatObj(hoImages, ho, out hoImages);
                                //ho.Dispose();
                            }
                        }
                        dieImgs[i] = hoImages;

                    }
                    fdr.DetectDiesImages = dieImgs;
                    fdr.IsDetectSuccess = isDetectOK;
                    fdr.DetectErrorInfo = detectErrorInfo;
                    fdr.DieInspectResults = diesResults;
#if true   //暂时保留，待数据库使用新数据结构后停止使用
                    if (null != diesErrorCodes)
                        fdr.DiesErrorCodes = diesErrorCodes.ToArray();
                    else
                        fdr.DiesErrorCodes = null;

                    if (null != diesErrorTaskNames)
                        fdr.DiesErrorTaskNames = diesErrorTaskNames.ToArray();
                    else
                        fdr.DiesErrorTaskNames = null;

                    if (null != diesFailedRegions)
                        fdr.DiesErrorRegions = diesFailedRegions.ToArray();
                    else
                        fdr.DiesErrorRegions = null;

                    if (diesErrorDetails != null)
                        fdr.DiesErrorDetails = diesErrorDetails.ToArray();
                    else
                        fdr.DiesErrorDetails = null;


                    fdr.DetectIterms = detectItems;
                    fdr.DetectItemTaskNames = detectItemTaskNames;
                    //将金线区域单独提取出来
                    if (null != fdr.DetectIterms && fdr.DetectIterms.ContainsKey("WireRegions"))
                    {
                        fdr.WireRegion = fdr.DetectIterms["WireRegions"];
                        fdr.WireRegionTaskName = fdr.DetectItemTaskNames["WireRegions"];
                        fdr.DetectIterms.Remove("WireRegions");
                        fdr.DetectItemTaskNames.Remove("WireRegions");
                    }
#endif
                    //向界面发送一个Fov的检测结果
                    NotifyCustomizeMsg(CMC_InspectResult, new object[]
                            {
                            dtp.TaskImages,
                            fdr.Clone()
                            }); 
                    _detectResultTransfer.EntryFovResult(fdr);//_queDetectResult.Enqueue(fdr);

                } while (false);


                if (_isSavePicture(fdr.IsDetectSuccess)) //保存图像
                {
                    JFImageSaveFileType imgSaveType = JFImageSaveFileType.Bmp;
                    object osft = GetCfgParamValue(CfgName_PicSaveFormat);
                    if (null != osft)
                        imgSaveType = (JFImageSaveFileType)osft;


                    string icPicFolder = _pictureFolder + "\\" + _currRecipeID + "\\" + _currLotID + "\\" + _currPieceID + "\\Row_" + dtp.ICRow + "-Col_" + dtp.ICCol + "-Fov_" + dtp.FovName;
                    Directory.CreateDirectory(icPicFolder);
                    for (int p = 0; p < dtp.TaskNames.Length; p++)
                    {
                        string picPath;
                        int ret = 0;
                        if (imgSaveType == JFImageSaveFileType.Jpg)
                        {
                            picPath = icPicFolder + "\\" + dtp.TaskNames[p] + ".JPG";
                            ret = dtp.TaskImages[p].Save(picPath, JFImageSaveFileType.Jpg);
                        }
                        else if (imgSaveType == JFImageSaveFileType.Bmp)
                        {
                            picPath = icPicFolder + "\\" + dtp.TaskNames[p] + ".BMP";
                            ret = dtp.TaskImages[p].Save(picPath, JFImageSaveFileType.Bmp);
                        }
                        else if (imgSaveType == JFImageSaveFileType.Tif)
                        {
                            picPath = icPicFolder + "\\" + dtp.TaskNames[p] + ".tiff";
                            ret = dtp.TaskImages[p].Save(picPath, JFImageSaveFileType.Tif);
                        }
                        else if (imgSaveType == JFImageSaveFileType.Png)
                        {
                            picPath = icPicFolder + "\\" + dtp.TaskNames[p] + ".png";
                            ret = dtp.TaskImages[p].Save(picPath, JFImageSaveFileType.Png);
                        }

                        //if (ret != 0)
                        //    ExitWork(WorkExitCode.Error, "图片保存失败,ErrorInfo:" + lstTaskImages[p].GetErrorInfo(ret));
                    }




                }
              
            }// end runFlag
        }

        //将检测结果存到Review数据库的线程
        void _TransDieResultThreadFunc()
        {
            while(_detectThreadRunningFlag)
            {
                InspectionData[] inspDatas = _detectResultTransfer.DisChargeDieResults();
                if (null != inspDatas)
                {
                    //向界面发送检测结果
                    NotifyCustomizeMsg(CMC_DieResults, new object[] { inspDatas });

                    DataManager.AddInspectionData(inspDatas.ToList(), _inspectDataEx); //向数据库发送检测结果
                    if(_detectResultTransfer.AllFovRecieved && _detectResultTransfer.AllFovDischarged)
                        _isCurrPieceDetectResultSaved = true;
                    
                }
                else
                    Thread.Sleep(CycleMilliseconds);
            }
        }

    }
}
