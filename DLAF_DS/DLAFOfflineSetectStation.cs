using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;
using DLAF;
using System.IO;
using LFAOIRecipe;
using HalconDotNet;

namespace DLAF_DS
{
    /// <summary>
    /// DLAF离线检测工站
    /// </summary>
    [JFDisplayName("DLAF离线检测工站")]
    public class DLAFOfflineSetectStation : JFStationBase
    {
        #region Station Customize Message
        public static string SCM_PieceStart = "PieceStart"; //开始检测料片
        public static string SCM_PieceEnd = "PieceEnd"; //料片检测结束
        public static string SCM_FovDetectResult = "FovDetectResult"; //一个FOV的检测结果
        #endregion

        public DLAFOfflineSetectStation()
        {
            DeclearCfgParam(JFParamDescribe.Create("TestPicFolder", typeof(string), JFValueLimit.FolderPath, null), "运行参数");
            DeclearCfgParam(JFParamDescribe.Create("RecipeID", typeof(string), JFValueLimit.NonLimit, null), "运行参数");
            DeclearCfgParam(JFParamDescribe.Create("LotID", typeof(string), JFValueLimit.NonLimit, null), "运行参数");


            DeclearCfgParam(JFParamDescribe.Create("相机标定文件", typeof(string), JFValueLimit.FilePath, null), "视觉定位参数");
            DeclearCfgParam(JFParamDescribe.Create("图像宽度/像素", typeof(int), JFValueLimit.MinLimit, new object[] { 0}), "视觉定位参数");
            DeclearCfgParam(JFParamDescribe.Create("图像高度/像素", typeof(int), JFValueLimit.MinLimit, new object[] { 0 }), "视觉定位参数");



            Array acs = Enum.GetValues(typeof(ODCS));
            _allCustomStatus = new int[acs.Length];
            for (int i = 0; i < acs.Length; i++)
                _allCustomStatus[i] = (int)acs.GetValue(i);
            _ucRT.SetStation(this);
        }
        public override JFStationRunMode RunMode { get { return JFStationRunMode.Manual; } }
        public override bool SetRunMode(JFStationRunMode runMode)
        {
            if (runMode == JFStationRunMode.Manual)
                return true;
            return false;
        }


        public override int[] AllCmds { get { return new int[] { }; } }
        public override string GetCmdName(int cmd)
        {
            throw new NotImplementedException(); //由于AllCmds不包含任何指令,此处可抛未实现异常
        }

        //offline detect custom status 
        enum ODCS
        {
            未运行,
            算法初始化,
            等待指令,
            正在检测,
        }
        int[] _allCustomStatus = null;
        //ODCS _currCS = ODCS.未运行;

        public override int[] AllCustomStatus
        {
            get { return _allCustomStatus; }
        }



        public override string GetCustomStatusName(int status)
        {
            if (status < (int)ODCS.未运行 || status > (int)ODCS.正在检测)
                return "Undefine Comstom Status";
            return ((ODCS)status).ToString();
        }

        void _ChangeCS(ODCS cs)
        {
            ChangeCustomStatus((int)cs);
        }

        //测试图片文件夹（LotID的上一级）
        internal string TestPicFolder { get { return GetCfgParamValue("TestPicFolder") as string; } }
        internal bool SetTestPidFolder(string folderPath, out string errorInfo)
        {
            //if(!Directory.Exists(folderPath))
            //{
            //    errorInfo = "文件夹:\"" + folderPath + "\"不存在";
            //    return false;
            //}
            if (IsWorkingStatus(CurrWorkStatus))
            {
                errorInfo = "离线检测正在运行，不能修改图片文件夹！";
                return false;
            }

            SetCfgParamValue("TestPicFolder", folderPath);
            SaveCfg();
            errorInfo = "Success";
            return true;

        }

        internal string RecipeID { get { return GetCfgParamValue("RecipeID") as string; } }
        internal bool SetRecipeID(string recipeID, out string errorInfo)
        {
            //IJFRecipeManager irm = JFHubCenter.Instance.RecipeManager;
            //if (null == irm)
            //    errorInfo = "配方管理器未设置"; //ExitWork(WorkExitCode.Error, "配方管理器未设置");
            //if (!(irm is JFDLAFRecipeManager))
            //    ExitWork(WorkExitCode.Error, "配方管理器类型错误:" + irm.GetType().Name);
            //JFDLAFRecipeManager rm = irm as JFDLAFRecipeManager;
            //if(!rm.IsInitOK)
            //{
            //    return false;
            //}

            if (IsWorkingStatus(CurrWorkStatus))
            {
                errorInfo = "离线检测正在运行，不能修改 产品ID！";
                return false;
            }

            SetCfgParamValue("RecipeID", recipeID);
            SaveCfg();
            errorInfo = "Success";
            return true;


        }



        internal string LotID { get { return GetCfgParamValue("LotID") as string; } }
        internal bool SetLotID(string lotID, out string errorInfo)
        {
            if (IsWorkingStatus(CurrWorkStatus))
            {
                errorInfo = "离线检测正在运行，不能修改批次ID！";
                return false;
            }

            SetCfgParamValue("LotID", lotID);
            SaveCfg();
            errorInfo = "Success";
            return true;
        }

        string _pieceID = null;
        internal string PieceID{ get { return _pieceID; } }

        internal bool SetPieceID(string pieceID,out string errorInfo)
        {
            if(IsWorking())
            {
                errorInfo = "正在离线检测料片，不能修改料片ID";
                return false;
            }

            _pieceID = pieceID;
            errorInfo = "Success";
            return true;
        }


        protected override void OnPause()
        {
            return;
        }

        protected override void OnResume()
        {
            return;
        }

        protected override void OnStop()
        {
            
        }



        protected override void ExecuteEndBatch()
        {
            return;
        }

        protected override void ExecuteReset()
        {
            return;
        }


        string _currPiecePicFolder = null; //保存一片整料的所有图片的文件夹
        JFDLAFProductRecipe _currRecipe = null;
        DLAFVisionFixer _visionFixer = new DLAFVisionFixer();


        protected override void PrepareWhenWorkStart()
        {
            if (string.IsNullOrEmpty(TestPicFolder))
                ExitWork(WorkExitCode.Error, "检测失败:图片文件路径未设置");
            if (!Directory.Exists(TestPicFolder))
                ExitWork(WorkExitCode.Error, "检测失败,图片文件夹不存在:" + TestPicFolder);
            IJFRecipeManager irm = JFHubCenter.Instance.RecipeManager;
            if (null == irm)
                ExitWork(WorkExitCode.Error, "检测失败，配方管理器未设置");
            if (!(irm is JFDLAFRecipeManager))
                ExitWork(WorkExitCode.Error, "检测失败，配方管理器类型错误:" + irm.GetType().Name);
            JFDLAFRecipeManager rm = irm as JFDLAFRecipeManager;
            if (!rm.IsInitOK)
                ExitWork(WorkExitCode.Error, "检测失败，配方管理器未初始化,ErrorInfo:" + rm.GetInitErrorInfo());



            if(string.IsNullOrEmpty(RecipeID))
                ExitWork(WorkExitCode.Error, "检测失败:RecipeID未设置");
            string[] allRecipeIDs = rm.AllRecipeIDsInCategoty("Product");
            if (null == allRecipeIDs || !allRecipeIDs.Contains(RecipeID))
                ExitWork(WorkExitCode.Error, "检测失败，RecipeID:\"" + RecipeID + "\"在配方管理器中不存在");
            
            if (string.IsNullOrEmpty(LotID))
                ExitWork(WorkExitCode.Error, "检测失败:LotID未设置");
            if (string.IsNullOrEmpty(_pieceID))
                ExitWork(WorkExitCode.Error, "检测失败:未选择料片号");
            _currPiecePicFolder = TestPicFolder + "\\" + RecipeID + "\\" + LotID + "\\" + _pieceID;
            if (!Directory.Exists(_currPiecePicFolder))
                ExitWork(WorkExitCode.Error, "检测失败,产品图片文件夹:\"" + _currPiecePicFolder + "\" 不存在");

            _currRecipe = rm.GetRecipe("Product", RecipeID) as JFDLAFProductRecipe;
            //初始化视觉算子
            string errorInfo;
            _ChangeCS(ODCS.算法初始化);
            JFDLAFInspectionManager.Instance.Clear(); //将所有已初始化的算子释放，重新初始化（适应外部修改配置）
            if (!JFDLAFInspectionManager.Instance.InitInspectNode(RecipeID, out errorInfo))
                ExitWork(WorkExitCode.Error, "检测失败,图像算子初始化失败:" + errorInfo);


            string cmrCalibDataFilePath = GetCfgParamValue("相机标定文件") as string;
            int imgWidth = (int)GetCfgParamValue("图像宽度/像素");
            int imgHeight = (int)GetCfgParamValue("图像高度/像素");
           
            if(!_visionFixer.Init(cmrCalibDataFilePath, _currRecipe, imgWidth,imgHeight,out errorInfo))
                ExitWork(WorkExitCode.Error, "检测失败,图像矫正算子初始化失败:" + errorInfo);



            _ChangeCS(ODCS.正在检测);

        }

        protected override void RunLoopInWork()
        {
            NotifyCustomizeMsg(SCM_PieceStart, new object[] { _currPiecePicFolder });
            //Inspect_Node inpect = JFDLAFInspectionManager.Instance.GetInspectNode
            int rows = _currRecipe.RowCount;
            int cols = _currRecipe.ColCount;

            string[] fovNames = _currRecipe.FovNames();
            for(int currRow = 0; currRow < rows; currRow ++)
                for(int currCol = 0; currCol < cols; currCol++)
                    for(int currFov = 0; currFov < fovNames.Length;currFov++ )
                    {
                        CheckCmd(CycleMilliseconds); //检测是否有暂停/恢复/退出 消息
                        string fovName = fovNames[currFov];
                        string fovSubFolder = "Row_" + currRow + "-" + "Col_" + currCol + "-Fov_" + fovName;
                        string fovFolder = _currPiecePicFolder + "\\" + fovSubFolder;//存储图片的文件夹
                        if (!Directory.Exists(fovFolder))
                            ExitWork(WorkExitCode.Error, "Fov图片文件夹:\"" + fovFolder + "\"不存在");
                        InspectNode fovInspNode = JFDLAFInspectionManager.Instance.GetInspectNode(RecipeID, fovName);
                        if(null == fovInspNode)
                        {
                            ExitWork(WorkExitCode.Error, "算子管理器中不存在 RecipeID = " + RecipeID + " Fov = " + fovName + "  的检测算子");
                        }

                        string[] filesInFovFolder = Directory.GetFiles(fovFolder);//Fov文件夹中现有的文件
                        if(null == filesInFovFolder || filesInFovFolder.Length == 0)
                            ExitWork(WorkExitCode.Error, "Fov图片文件夹:\"" + fovFolder + "\"中没有文件");


                        string[] taskNames = _currRecipe.TaskNames(fovName);
                        List<HObject> taskImgs = new List<HObject>();
                        foreach(string taskName in taskNames)
                        {
                            HObject hoImg;
                            HOperatorSet.GenEmptyObj(out hoImg);
                            string taskImgFile = null; 
                            foreach(string s in filesInFovFolder)
                            {
                                string exn = Path.GetExtension(s);
                                if (string.Compare(exn, ".bmp", true) != 0 &&
                                    string.Compare(exn, ".tiff", true) != 0 &&
                                    string.Compare(exn, ".tif", true) != 0 &&
                                    string.Compare(exn, ".jpg", true) != 0 &&
                                    string.Compare(exn, ".jpeg", true) != 0&&
                                    string.Compare(exn, ".png", true) != 0)
                                    continue;//先过滤掉非图像文件
                                string fnn = Path.GetFileNameWithoutExtension(s); //不带后缀的文件名
                                if (fnn.LastIndexOf(taskName) >= 0 &&
                                    fnn.Length == (fnn.LastIndexOf(taskName) + taskName.Length))
                                {
                                    taskImgFile = s;
                                    break;
                                }
                            }
                            if (null == taskImgFile)
                                ExitWork(WorkExitCode.Error, "Task = " + taskName + "  图片文件不存在");

                            HOperatorSet.ReadImage(out hoImg, taskImgFile);
                            taskImgs.Add(hoImg);
                        }

                        DlafFovDetectResult fdr = new DlafFovDetectResult();
                        fdr.FovName = fovName;
                        fdr.ICRow = currRow;
                        fdr.ICCol = currCol;
                        fdr.TaskNames = taskNames;
                        



                        string errorInfo;
                        //检测输入参数
                        HObject dieRegions;
                        int[] dieRows, dieCols;
                        int currDieRowCount, currDieColCount;
                        if (!_visionFixer.GetDetectRegionInFov(currRow, currCol, out dieRegions, out dieRows, out dieCols,out  currDieRowCount,out  currDieColCount, out errorInfo))
                            ExitWork(WorkExitCode.Error, "GetDetectRegionInFov failed,ErrorInfo:" + errorInfo);
                        fdr.DetectDiesRows = dieRows;
                        fdr.DetectDiesCols = dieCols;
                        fdr.CurrRowCount = currDieRowCount;
                        fdr.CurrColCount = currDieColCount;
                        
                        //fdr.DieInspectResults = new List<InspectResultItem>[currDieRowCount* currDieColCount];
                        //for (int i = 0; i < currDieRowCount * currDieColCount; i++)
                        //    fdr.DieInspectResults[i] = new List<InspectResultItem>();

                        //检测输出参数
                        string detectErrorInfo; //算子执行信息
                        List<int[]> diesErrorCodes; //= new List<int[]>();
                        List<string[]> diesErrorTaskNames;

                        List<HObject[]> diesFailedRegions;
                        List<string[]> diesErrorDetails; //错误的详细描述信息
                        Dictionary<string, HObject> detectItems; //算子中输出的检测项（BondContours/WireRegions 等等）
                        Dictionary<string, string> detectItemTaskNames;


                        //bool isDetectOK = fovInspNode.InspectImage(taskImgs.ToArray(), taskNames, dieRegions, 
                        //                                            out detectErrorInfo, out diesErrorCodes, out diesErrorTaskNames, out diesFailedRegions,out diesErrorDetails,
                        //                                            out detectItems,out detectItemTaskNames);

                        //用于测试新数据结构
                        List<InspectResultItem[]> inspectResults = null;
                        bool isDetectOK = fovInspNode.InspectImage(taskImgs.ToArray(),
                            dieRegions, out detectErrorInfo, out inspectResults);
                        fdr.DieInspectResults = inspectResults; //所有检测项


                        int dieCountInfov = dieRegions.CountObj();
                        //裁剪后的图象（按照Die的检测区域）
                        HObject[] dieImgs = new HObject[dieCountInfov];                        
                        HTuple _row1, _col1, _row2, _col2;
                        HTuple TWidth, THeight;
                        for (int i = 0; i < dieCountInfov;i++)
                        {
                            HOperatorSet.SmallestRectangle1(dieRegions.SelectObj(i+1), out _row1, out _col1, out _row2, out _col2);
                            if (_row1 < 0) _row1 = 0;
                            if (_col1 < 0) _col1 = 0;
                            HOperatorSet.GetImageSize(taskImgs[0], out  TWidth, out  THeight);
                            if (_row2 > THeight) _row2 = THeight;
                            if (_col2 > TWidth) _row2 = TWidth;

                            HObject hoImages = null; //一颗Die的多通道图
                            HOperatorSet.GenEmptyObj(out hoImages);
                            for (int j = 0; j < taskImgs.Count; j++)
                            {
                                HObject ho;
                                HOperatorSet.CropRectangle1(taskImgs[j], out ho, _row1, _col1, _row2, _col2);
                                HOperatorSet.ConcatObj(hoImages, ho, out hoImages);
                            }
                            dieImgs[i] = hoImages;

                        }
                        fdr.DetectDiesImages = dieImgs;
                        fdr.IsDetectSuccess = isDetectOK;
                        fdr.DetectErrorInfo = detectErrorInfo;
                        //if (null != diesErrorCodes)
                        //    fdr.DiesErrorCodes = diesErrorCodes.ToArray();
                        //else
                        //    fdr.DiesErrorCodes = null;

                        //if (null != diesErrorTaskNames)
                        //    fdr.DiesErrorTaskNames = diesErrorTaskNames.ToArray();
                        //else
                        //    fdr.DiesErrorTaskNames = null;

                        //if (null != diesFailedRegions)
                        //    fdr.DiesErrorRegions = diesFailedRegions.ToArray();
                        //else
                        //    fdr.DiesErrorRegions = null;

                        //if (diesErrorDetails != null)
                        //    fdr.DiesErrorDetails = diesErrorDetails.ToArray();
                        //else
                        //    fdr.DiesErrorDetails = null;


                        //fdr.DetectIterms = detectItems;
                        //fdr.DetectItemTaskNames = detectItemTaskNames;
                        //if (fdr.DetectIterms != null && fdr.DetectIterms.ContainsKey("WireRegions"))
                        //{
                        //    fdr.WireRegion = fdr.DetectIterms["WireRegions"];
                        //    fdr.WireRegionTaskName = fdr.DetectItemTaskNames["WireRegions"];
                        //    fdr.DetectIterms.Remove("WireRegions");
                        //    fdr.DetectItemTaskNames.Remove("WireRegions");
                        //}



                        NotifyCustomizeMsg(SCM_FovDetectResult, new object[] {
                            taskImgs, //FOV原图
                            fdr       //测试结果
                            });
                    
                    }
            NotifyCustomizeMsg(SCM_PieceEnd, null);
            ExitWork(WorkExitCode.Normal, "");
        }

        protected override void CleanupWhenWorkExit()
        {
            return;
        }

        UcOfflineDetectStation _ucRT = new UcOfflineDetectStation();

        public override JFRealtimeUI GetRealtimeUI()
        {
            //return base.GetRealtimeUI();
            return _ucRT;
        }
    }
}
