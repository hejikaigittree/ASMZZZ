using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DLAF;
using JFHub;
using JFInterfaceDef;
using JFUI;
using JFVision;
using HalconDotNet;
using System.IO;

namespace DLAF_DS
{
    [JFDisplayName("光学配置示教工站")]
    [JFVersion("1.0.0.0")]
    public class DLAFVisionTeachStation : JFStationBase, IDLAFProductFixReceiver, IDLAFInspectionReceiver
    {
        enum TVStatus   //Detect Station Status  检测工站的业务状态
        {
            已停止 = 0,
            Z轴避位,
            复位,                 //如需要复位，则进行复位动作
            等待确认轨道避位完成,     //复位完成后，需要人工确认前后轨道拨片已到达避位位置
            移动到待机位置,       //准备工作（复位）完成后 ，向待机位置移动
            等待指令,             //等待用户指令(定位Mark / 定位到FOV)
            定位产品,           //定位Mark1 + Mark2（包含移动，拍照，调用定位算法）
            向指定IC移动,
            向指定FOV移动,
            调整Task视觉配置,
            定位Mark1,
            定位Mark2,
            采集模板图像,
            Fov视觉检测,
        }

        static string CategotyProduct = "Product";

        ////////////设备名称 Device Alias Name 
        static string DevAN_Cmr = "相机";
        //static string DevAN_CoAxialLight = "同轴光";
        //static string DevAN_RingLight = "环形光";
        static string DevAN_Light0 = "环光1";
        static string DevAN_Light1 = "环光2";
        static string DevAN_Light2 = "环光3";
        static string DevAN_Light3 = "环光4";
        static string DevAN_AxisX = "X轴";
        static string DevAN_AxisY = "Y轴";
        static string DevAN_AxisZ = "Z轴";

        static string PosAN_Standby = "待机位置";

        /// <summary>
        /// 工站方法流定义
        /// </summary>
        static string MFName_Prepare = "任务参数初始化";
        static string MFName_FixProduct = "产品定位";
        static string MFName_Inspect = "视觉检测";


        static string MFDP_MarkImage1 = "MarkImage1";
        static string MFDP_MarkSnapX1 = "MarkSnapX1";
        static string MFDP_MarkSnapY1 = "MarkSnapY1";

        static string MFDP_MarkImage2 = "MarkImage2";
        static string MFDP_MarkSnapX2 = "MarkSnapX2";
        static string MFDP_MarkSnapY2 = "MarkSnapY2";

        static string MFDP_RecipeID = "RecipeID"; //数据池对象名称


        //Station Cfg Item Name's
        public static string SCItemName_ModelPicPath = "模板图像保存路径";

        public static string SCItemName_CmrCalibDataFile = "相机标定数据文件";
        public static string SCItemName_ImageWidth = "相机图象宽度/像素";
        public static string SCItemName_ImageHeight = "相机图象高度/像素";







        public DLAFVisionTeachStation()
        {
            PFRecipeID = null;
            PFErrorCode = -1;
            PFErrorInfo = "未定位";

            Array csVals = Enum.GetValues(typeof(TVStatus));
            _customStatus = new int[csVals.Length];
            for (int i = 0; i < csVals.Length; i++)
                _customStatus[i] = (int)csVals.GetValue(i);

            csVals = Enum.GetValues(typeof(TVCommand));
            _allCmds = new int[csVals.Length];
            for (int i = 0; i < csVals.Length; i++)
                _allCmds[i] = (int)csVals.GetValue(i);


            DeclearDevChn(NamedChnType.Camera, DevAN_Cmr);//添加一个本地相机（替身名称）
            DeclearDevChn(NamedChnType.Light, DevAN_Light0);//添加一个本地光源（替身名称）
            DeclearDevChn(NamedChnType.Light, DevAN_Light1);//添加一个本地光源（替身名称）
            DeclearDevChn(NamedChnType.Light, DevAN_Light2);//添加一个本地光源（替身名称）
            DeclearDevChn(NamedChnType.Light, DevAN_Light3);//添加一个本地光源（替身名称）
            DeclearDevChn(NamedChnType.Axis, DevAN_AxisX);//添加一个本地轴设备（替身名称）
            DeclearDevChn(NamedChnType.Axis, DevAN_AxisY);//添加一个本地轴设备（替身名称）
            DeclearDevChn(NamedChnType.Axis, DevAN_AxisZ);//添加一个本地设备（替身名称）

            DeclearWorkPosition(PosAN_Standby);


            DeclearMethodFlow(MFName_Prepare);
            DeclearMethodFlow(MFName_FixProduct);
            DeclearMethodFlow(MFName_Inspect);


            DeclearCfgParam(JFParamDescribe.Create(SCItemName_ModelPicPath, typeof(string), JFValueLimit.FolderPath, null), "任务参数");
            DeclearCfgParam(JFParamDescribe.Create(SCItemName_CmrCalibDataFile, typeof(string), JFValueLimit.FilePath, null), "任务参数");
            DeclearCfgParam(JFParamDescribe.Create(SCItemName_ImageWidth, typeof(int), JFValueLimit.MinLimit, new object[] { 0}), "任务参数");
            DeclearCfgParam(JFParamDescribe.Create(SCItemName_ImageHeight, typeof(int), JFValueLimit.MinLimit, new object[] { 0 }), "任务参数");
            

            _rtUi.SetStation(this);
        }



        string _recipeID = null; //当前示教的产品型号
        bool _isProductFixed = false; //当前产品是否定位成功
        int _currIcRow = -1; //当前定位的IC行数
        int _currIcCol = -1; //当前定位的IC列数
        bool _isIcFixed = false; //当前IC是否已定位

        string _currFovName = "";//当前定位的Fov名称
        bool _isFovFixed = false;//当前Fov是否定位成功


        string _currTaskName = ""; //当前视觉任务名称


        DLAFVisionFixer _prodVisionFixer = new DLAFVisionFixer(); //视觉定位算子


        public string CurrRecipeID()
        {
            return _recipeID;
        }

        /// <summary>
        /// 设置当前示教的产品型号
        /// </summary>
        /// <param name="recipeID"></param>
        public bool SetRecipeID(string recipeID, out string errorInfo)
        {
            if (_recipeID == recipeID)
            {
                errorInfo = "Success";
                return true;
            }


            if (!IsWorkingStatus(CurrWorkStatus))
            {

                _recipeID = recipeID;
                _currRecipe = JFHubCenter.Instance.RecipeManager.GetRecipe(CategotyProduct, _recipeID) as JFDLAFProductRecipe;
                _isProductFixed = false;

                _currIcRow = -1; //当前定位的IC行数
                _currIcCol = -1; //当前定位的IC列数
                _isIcFixed = false; //当前IC是否已定位

                _currFovName = null;
                _isFovFixed = false;
                errorInfo = "Success";
                return true;
            }

            if (CurrCS == TVStatus.向指定FOV移动 || 
                CurrCS == TVStatus.定位产品 || 
                CurrCS == TVStatus.向指定IC移动 ||
                CurrCS == TVStatus.定位Mark1 ||
                CurrCS == TVStatus.定位Mark2 ||
                CurrCS == TVStatus.调整Task视觉配置||
                CurrCS == TVStatus.采集模板图像 ||
                CurrCS == TVStatus.Fov视觉检测)
            {
                errorInfo = "设置RecipeID失败，当前任务状态:" + CurrCS.ToString();
                return false;
            }



            _recipeID = recipeID;
            _isProductFixed = false;

            _currIcRow = -1; //当前定位的IC行数
            _currIcCol = -1; //当前定位的IC列数
            _isIcFixed = false; //当前IC是否已定位

            _currFovName = null;
            _isFovFixed = false;
            _currRecipe = JFHubCenter.Instance.RecipeManager.GetRecipe(CategotyProduct, _recipeID) as JFDLAFProductRecipe;


            JFMethodFlow mfPrepare = GetWorkFlow(MFName_Prepare);
            mfPrepare.Reset();
            mfPrepare.SetDP("RecipeID", _recipeID);
            if (!mfPrepare.Action())
                SendMsg2Outter("工站初始化流程失败,可能影响后继的视觉检测，错误信息:" + mfPrepare.ActionErrorInfo());

            errorInfo = "Success";
            return true;

        }

        public override bool Initialize()
        {
            bool ret = base.Initialize();
            JFMethodFlow mfPrepare = GetWorkFlow(MFName_Prepare);
            mfPrepare.DeclearDPItem("RecipeID", typeof(string), "");


            JFMethodFlow mfFixProduct = GetWorkFlow(MFName_FixProduct);
            mfFixProduct.DeclearDPItem("定位结果接收者", typeof(IDLAFProductFixReceiver), this);
            mfFixProduct.DeclearDPItem(MFDP_RecipeID, typeof(string), "");
            mfFixProduct.DeclearDPItem(MFDP_MarkSnapX1, typeof(double), 0f);
            mfFixProduct.DeclearDPItem(MFDP_MarkSnapY1, typeof(double), 0f);
            mfFixProduct.DeclearDPItem(MFDP_MarkImage1, typeof(IJFImage), null);
            mfFixProduct.DeclearDPItem(MFDP_MarkSnapX2, typeof(double), 0f);
            mfFixProduct.DeclearDPItem(MFDP_MarkSnapY2, typeof(double), 0f);
            mfFixProduct.DeclearDPItem(MFDP_MarkImage2, typeof(IJFImage), null);


            JFMethodFlow mfInspection = GetWorkFlow(MFName_Inspect);
            mfInspection.DeclearDPItem("RecipeID", typeof(string), "");
            mfInspection.DeclearDPItem("ICRow", typeof(int), 0);
            mfInspection.DeclearDPItem("ICCol", typeof(int), 0);
            mfInspection.DeclearDPItem("FovName", typeof(string), "");
            mfInspection.DeclearDPItem("TaskNames", typeof(string[]), null);
            mfInspection.DeclearDPItem("Images", typeof(IJFImage[]), null);
            mfInspection.DeclearDPItem("ResultHandle", typeof(IDLAFInspectionReceiver), this); //处理检测结果




            return ret;
        }


        bool _isFixProductByVision = true;
        /// <summary>
        /// 向工站下达产品定位指令
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool FixProduct(bool isFixByVision,out string errorInfo)
        {
            if (!IsWorkingStatus(CurrWorkStatus))
            {
                errorInfo = "定位产品失败，工站当前未运行";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Interactiving)
            {
                errorInfo = "定位产品失败，工站当前状态:等待用户确认操作！";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Pausing)
            {
                errorInfo = "定位产品失败，工站当前状态:已暂停！";
                return false;
            }

            if (CurrCS != TVStatus.等待指令)
            {
                errorInfo = "定位产品失败，当前任务状态:" + CurrCS;
                return false;
            }

            if (_recipeID == null)
            {
                errorInfo = "定位产品失败，未设置RecipeID";
                return false;
            }

            string cmrDataFile = GetCfgParamValue(SCItemName_CmrCalibDataFile) as string;
            if(string.IsNullOrEmpty(cmrDataFile))
            {
                errorInfo = "相机标定数据文件未设置";
                return false;
            }
            if(!File.Exists(cmrDataFile))
            {
                errorInfo = "相机标定数据文件不存在:" + cmrDataFile;
                return false;
            }

            int picWidth = (int)GetCfgParamValue(SCItemName_ImageWidth);
            if (picWidth <= 0)
            {
                errorInfo = "相机图象宽度未设置";
                return false;
            }

            int picHeight = (int)GetCfgParamValue(SCItemName_ImageHeight);
            if (picHeight <= 0)
            {
                errorInfo = "相机图象高度未设置";
                return false;
            }


            _isProductFixed = false;
            _isIcFixed = false;
            _isFovFixed = false;
            _isFixProductByVision = isFixByVision;

            JFWorkCmdResult cmdRet = SendCmd((int)TVCommand.产品定位, CycleMilliseconds * 5);
            if (cmdRet != JFWorkCmdResult.Success)
            {
                errorInfo = "向工站发送产品定位指令失败：ErrorCode = " + cmdRet;
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 向工站发送Mark定位指令
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool FixMark(int markIndex,out string errorInfo)
        {
            if (!IsWorkingStatus(CurrWorkStatus))
            {
                errorInfo = "定位Mark失败，工站当前未运行";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Interactiving)
            {
                errorInfo = "定位Mark失败，工站当前状态:等待用户确认操作！";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Pausing)
            {
                errorInfo = "定位Mark失败，工站当前状态:已暂停！";
                return false;
            }

            if (CurrCS != TVStatus.等待指令)
            {
                errorInfo = "定位Mark失败，当前任务状态:" + CurrCS;
                return false;
            }

            if (_recipeID == null)
            {
                errorInfo = "定位Mark失败，未设置RecipeID";
                return false;
            }
            if(markIndex < 0 || markIndex > 1)
            {
                errorInfo = "定位Mark失败，Mark序号(范围:0~1)非法:" + markIndex;
                return false;
            }
            _markIndexWillFix = markIndex;


            JFWorkCmdResult cmdRet = SendCmd((int)TVCommand.定位Mark, CycleMilliseconds * 5);
            if (cmdRet != JFWorkCmdResult.Success)
            {
                errorInfo = "向工站发送Mark定位指令失败：ErrorCode = " + cmdRet;
                return false;
            }

            errorInfo = "Success";
            return true;
        }


        /// <summary>
        /// 向工站下达定位IC指令
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool FixIC(int row, int col, out string errorInfo)
        {
            if (!IsWorkingStatus(CurrWorkStatus))
            {
                errorInfo = "定位IC失败，工站当前未运行";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Interactiving)
            {
                errorInfo = "定位IC失败，工站当前状态:等待用户确认操作！";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Pausing)
            {
                errorInfo = "定位IC失败，工站当前状态:已暂停！";
                return false;
            }

            if (CurrCS != TVStatus.等待指令)
            {
                errorInfo = "定位IC失败，当前任务状态:" + CurrCS;
                return false;
            }

            if (_recipeID == null)
            {
                errorInfo = "定位IC失败，未设置RecipeID";
                return false;
            }

            if (!_isProductFixed)
            {
                errorInfo = "定位IC失败,当前产品未定位";
                return false;
            }

            IJFRecipeManager mgr = JFHubCenter.Instance.RecipeManager;
            JFDLAFProductRecipe recipe = mgr.GetRecipe(CategotyProduct, _recipeID) as JFDLAFProductRecipe;
            if (row >= recipe.RowCount)
            {
                errorInfo = "定位IC失败，Row超限，最大值：" + (recipe.RowCount - 1);
                return false;
            }

            if (col >= recipe.ColCount)
            {
                errorInfo = "定位IC失败，Col超限，最大值：" + (recipe.ColCount - 1);
                return false;
            }
            //if(null == recipe)
            //{
            //    errorInfo = "定位IC失败，配方管理器中不存在RecipeID = " + _recipeID;
            //    return false;
            //}



            _currIcRow = row;
            _currIcCol = col;
            _isIcFixed = false;
            JFWorkCmdResult cmdRet = SendCmd((int)TVCommand.IC定位, CycleMilliseconds * 5);
            if (cmdRet != JFWorkCmdResult.Success)
            {
                errorInfo = "向工站发送IC定位指令失败：ErrorCode = " + cmdRet;
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 定位FOV
        /// </summary>
        /// <param name="fovName"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool FixFOV(string fovName, out string errorInfo)
        {
            if (!IsWorkingStatus(CurrWorkStatus))
            {
                errorInfo = "定位FOV失败，工站当前未运行";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Interactiving)
            {
                errorInfo = "定位FOV失败，工站当前状态:等待用户确认操作！";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Pausing)
            {
                errorInfo = "定位FOV失败，工站当前状态:已暂停！";
                return false;
            }

            if (_recipeID == null)
            {
                errorInfo = "定位FOV失败，未设置RecipeID";
                return false;
            }


            //if(null == recipe)
            //{
            //    errorInfo = "定位IC失败，配方管理器中不存在RecipeID = " + _recipeID;
            //    return false;
            //}

            if (CurrCS != TVStatus.等待指令)
            {
                errorInfo = "定位FOV失败，当前任务状态:" + CurrCS;
                return false;
            }

            if (!_isIcFixed)
            {
                errorInfo = "定位FOV失败，IC定位未完成！";
                return false;
            }
            _currFovName = fovName;
            _isFovFixed = false;
            JFWorkCmdResult cmdRet = SendCmd((int)TVCommand.FOV定位, CycleMilliseconds * 5);
            if (cmdRet != JFWorkCmdResult.Success)
            {
                errorInfo = "向工站发送FOV定位指令失败：ErrorCode = " + cmdRet;
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        
        /// <summary>
        /// 向工站发送调整Task视觉参数指令
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool FixTaskVisionCfg(string taskName,out string errorInfo)
        {
            if (!IsWorkingStatus(CurrWorkStatus))
            {
                errorInfo = "调整Task视觉参数失败，工站当前未运行";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Interactiving)
            {
                errorInfo = "调整Task视觉参数失败，工站当前状态:等待用户确认操作！";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Pausing)
            {
                errorInfo = "调整Task视觉参数失败，工站当前状态:已暂停！";
                return false;
            }

            if (_recipeID == null)
            {
                errorInfo = "调整Task视觉参数失败，未设置RecipeID";
                return false;
            }


            if (CurrCS != TVStatus.等待指令)
            {
                errorInfo = "调整Task视觉参数失败，当前任务状态:" + CurrCS;
                return false;
            }

            if (!_isFovFixed)
            {
                errorInfo = "调整Task视觉参数失败，Fov定位未完成！";
                return false;
            }
            _currTaskName = taskName;
            
            JFWorkCmdResult cmdRet = SendCmd((int)TVCommand.Task视觉配置, CycleMilliseconds * 5);
            if (cmdRet != JFWorkCmdResult.Success)
            {
                errorInfo = "向工站发送Task视觉配置指令失败：ErrorCode = " + cmdRet;
                return false;
            }

            errorInfo = "Success";
            return true;
        }


        string _modelPicFolder = null; //木板图像保存路径
        /// <summary>
        /// 采集产品模板图像，采集所有IC的Fov的Task
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool GrabModelPicture(out string errorInfo)
        {
            if (!IsWorkingStatus(CurrWorkStatus))
            {
                errorInfo = "采集模板图像失败，工站当前未运行";
                return false;
            }

            _modelPicFolder = GetCfgParamValue(SCItemName_ModelPicPath) as string;
            if(string.IsNullOrEmpty(_modelPicFolder))
            {
                errorInfo = "采集模板图像失败，保存路径未设置";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Interactiving)
            {
                errorInfo = "采集模板图像失败，工站当前状态:等待用户确认操作！";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Pausing)
            {
                errorInfo = "采集模板图像失败，工站当前状态:已暂停！";
                return false;
            }

            if (_recipeID == null)
            {
                errorInfo = "采集模板图像失败，未设置RecipeID";
                return false;
            }

            if(!_isProductFixed)
            {
                errorInfo = "采集模板图像失败，当前产品未定位";
                return false;
            }


            if (CurrCS != TVStatus.等待指令)
            {
                errorInfo = "采集模板图像失败，当前任务状态:" + CurrCS;
                return false;
            }

            

            JFWorkCmdResult cmdRet = SendCmd((int)TVCommand.采集模板图像, CycleMilliseconds * 5);
            if (cmdRet != JFWorkCmdResult.Success)
            {
                errorInfo = "采集模板图像失败：ErrorCode = " + cmdRet;
                return false;
            }

            errorInfo = "Success";
            return true;
        }



        int _inspectRow = -1; //需要检测的ICRow
        int _inspectCol = -1;
        string _inspectFovName = ""; //需要检测的Fov

        /// <summary>
        /// 对当前FOV进行视觉检测
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="fovName"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool InspectFov(int row,int col,string fovName,out string errorInfo)
        {
            if (!IsWorkingStatus(CurrWorkStatus))
            {
                errorInfo = "Fov视觉测试失败，工站当前未运行";
                return false;
            }


            if (CurrWorkStatus == JFWorkStatus.Interactiving)
            {
                errorInfo = "Fov视觉测试失败，工站当前状态:等待用户确认操作！";
                return false;
            }

            if (CurrWorkStatus == JFWorkStatus.Pausing)
            {
                errorInfo = "Fov视觉测试失败，工站当前状态:已暂停！";
                return false;
            }

            if (_recipeID == null)
            {
                errorInfo = "Fov视觉测试失败，未设置RecipeID";
                return false;
            }

            if (!_isProductFixed)
            {
                errorInfo = "Fov视觉测试失败，当前产品未定位";
                return false;
            }


            if (CurrCS != TVStatus.等待指令)
            {
                errorInfo = "Fov视觉测试失败，当前任务状态:" + CurrCS;
                return false;
            }

            _inspectRow = row;
            _inspectCol = col;
            _inspectFovName = fovName;

            JFWorkCmdResult cmdRet = SendCmd((int)TVCommand.FOV视觉测试, CycleMilliseconds * 5);
            if (cmdRet != JFWorkCmdResult.Success)
            {
                errorInfo = "Fov视觉测试失败：ErrorCode = " + cmdRet;
                return false;
            }

            errorInfo = "Success";
            return true;
        }



        public override JFStationRunMode RunMode { get { return JFStationRunMode.Manual; } } //本工站只支持手动模式
        public override bool SetRunMode(JFStationRunMode runMode)
        {
            if (runMode == JFStationRunMode.Auto)
                return false;
            return true;
        }

        /// <summary>
        /// 改变当前业务逻辑状态
        /// </summary>
        /// <param name="status"></param>
        void _ChangeCS(TVStatus status)
        {
            if (CurrCustomStatus == (int)status)
                return;
            ChangeCustomStatus((int)status);
        }


        /// <summary>
        ///  当前业务逻辑状态
        /// </summary>
        TVStatus CurrCS { get { return (TVStatus)CurrCustomStatus; } }


        int[] _customStatus = null;
        public override int[] AllCustomStatus { get { return _customStatus; } }
        public override string GetCustomStatusName(int status)
        {
            return ((TVStatus)status).ToString();
        }


        public enum TVCommand
        {
            定位Mark,
            产品定位,
            IC定位,
            FOV定位,
            Task视觉配置,
            采集模板图像,
            FOV视觉测试
        }


        int[] _allCmds = null;
        public override int[] AllCmds { get { return _allCmds; } }

        public string PFRecipeID { get ; set; }
        public int PFErrorCode { get ; set; }
        public string PFErrorInfo { get ; set ; }
        public double[] PFICCenterX { get ; set; }
        public double[] PFICCenterY { get ; set; }
        public double[] PFFovOffsetX { get ; set; }
        public double[] PFFovOffsetY { get ; set ; }

        public override string GetCmdName(int cmd)
        {
            return ((TVCommand)cmd).ToString();
        }






        

        UcRtTeachStation _rtUi = new UcRtTeachStation();
        public override JFRealtimeUI GetRealtimeUI()
        {
            return _rtUi;
           // return base.GetRealtimeUI();
        }





        /// <summary>
        /// 打开所有设备通到并使能
        /// 设备，光源，相机等
        /// </summary>
        void EnableAllDevices()
        {
            string errorInfo;
            if(!OpenAllDevices(out errorInfo))
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



        string _gNameAxisX = null; //X轴名称
        string _gNameAxisY = null;
        string _gNameAxisZ = null;
        protected override void PrepareWhenWorkStart()
        {
            _gNameAxisX = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisX);
            if(string.IsNullOrEmpty(_gNameAxisX))
            {
                ExitWork(WorkExitCode.Error, "Alias:\"" + DevAN_AxisX + "\" 未指定全局通道名称");
            }
            _gNameAxisY = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisY);
            if (string.IsNullOrEmpty(_gNameAxisY))
            {
                ExitWork(WorkExitCode.Error, "Alias:\"" + DevAN_AxisY + "\" 未指定全局通道名称");
            }
            _gNameAxisZ = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisZ);
            if (string.IsNullOrEmpty(_gNameAxisZ))
            {
                ExitWork(WorkExitCode.Error, "Alias:\"" + DevAN_AxisZ + "\" 未指定全局通道名称");
            }
            EnableAllDevices();
            Thread.Sleep(4000);
            _ChangeCS(TVStatus.Z轴避位);
            string errorInfo = "Unknown-Error";
            JFWorkCmdResult waitRet = JFWorkCmdResult.UnknownError;
            if (!MoveVelAxisByAlias(DevAN_AxisZ, true, out errorInfo))
                ExitWork(WorkExitCode.Error, CurrCS + "失败:" + errorInfo);
            waitRet = WaitAxis2LimitByAlias(DevAN_AxisZ, true, -1);
            if (JFWorkCmdResult.Success != waitRet)
            {
                ExitWork(WorkExitCode.Error, "等待:" + TVStatus.Z轴避位 + "失败，ErrorCode = " + waitRet);
                return;
            }
            _ChangeCS(TVStatus.等待确认轨道避位完成);
            //通知用户确认左右轨道电机已到达安全位置
            string[] tipInfos = new string[] { "左右轨道电机干涉，工站退出运行", "已确认左右轨道安全，继续运行" };
            int ret = JFSelectTipsUI.ShowDialog("请确认操作安全！", tipInfos, 0);
            if (ret <= 0)
                ExitWork(WorkExitCode.Command, "左右轨道干涉运行，已确认退出");


            ///////创建两个Mark点(坐标为0)
            _markPositions[0].Positions.Clear();
            _markPositions[0].Positions.Add(JFAxisPos.Create(_gNameAxisX, 0));
            _markPositions[0].Positions.Add(JFAxisPos.Create(_gNameAxisY, 0));
            _markPositions[1].Positions.Clear();
            _markPositions[1].Positions.Add(JFAxisPos.Create(_gNameAxisX, 0));
            _markPositions[1].Positions.Add(JFAxisPos.Create(_gNameAxisY, 0));

            if (!IsNeedResetWhenStart()) //不需要归零动作
            {
                _ChangeCS(TVStatus.移动到待机位置);
                if (!MoveToWorkPosition(PosAN_Standby,out errorInfo))
                    ExitWork(WorkExitCode.Error, CurrCS + "失败:" + errorInfo);
                if(!WaitToWorkPosition(PosAN_Standby, out errorInfo))
                {
                    ExitWork(WorkExitCode.Error, "等待:" + TVStatus.移动到待机位置 + "失败，Error:" + errorInfo);
                    return;
                }
                _ChangeCS(TVStatus.等待指令);
            }
            else
                _ChangeCS(TVStatus.复位);
            return;
        }

        /// <summary>
        /// 工站归零
        /// </summary>
        protected override void ExecuteReset()
        {
            _ChangeCS(TVStatus.复位);
            string errorInfo = "Unknown-Error";
            if (!EnableAllAxis(out errorInfo))
            {
                ExitWork(WorkExitCode.Error, "工站归零失败:" + errorInfo);
                return;
            }

            //Z轴先上升到最高
            if (!MoveVelAxisByAlias(DevAN_AxisZ, true, out errorInfo))
                ExitWork(WorkExitCode.Error, "工站归零失败:" + errorInfo);
            JFWorkCmdResult ret = WaitAxis2LimitByAlias(DevAN_AxisZ, true);
            if (ret != JFWorkCmdResult.Success)
                ExitWork(WorkExitCode.Error, "工站归零失败: 等待Z轴向正限位返回ErrorCode = " + ret);

            //Y轴先归零，
            if (!AxisHomeByAlias(DevAN_AxisY, out errorInfo))
                ExitWork(WorkExitCode.Error, "工站归零失败: " + errorInfo);

            WaitAxisHomeDoneByAlias(DevAN_AxisY);



            //X轴归零
            AxisHomeByAlias(DevAN_AxisX, out errorInfo);
            WaitAxisHomeDoneByAlias(DevAN_AxisX);

            //Z轴归零
            AxisHomeByAlias(DevAN_AxisZ, out errorInfo);
            WaitAxisHomeDoneByAlias(DevAN_AxisZ);
            //if(!IsNeedResetWhenStart()) //正常运行模式
            {
                //向待机位置移动
                _ChangeCS(TVStatus.移动到待机位置);
                if (!MoveToWorkPosition(PosAN_Standby, out errorInfo))
                    ExitWork(WorkExitCode.Error, CurrCS + "失败:" + errorInfo);
                if (!WaitToWorkPosition(PosAN_Standby, out errorInfo))
                {
                    ExitWork(WorkExitCode.Error, "等待:" + TVStatus.移动到待机位置 + "失败，Error:" + errorInfo);
                    return;
                }
                _ChangeCS(TVStatus.等待指令);
            }



        }

        protected override void OnPause()
        {
            
        }

        protected override void OnResume()
        {
            
        }

        protected override void OnStop()
        {
            StopAxisAlias(DevAN_AxisX);
            StopAxisAlias(DevAN_AxisY);
            StopAxisAlias(DevAN_AxisZ);
        }

        /// <summary>
        /// 产品定位子步骤
        /// </summary>
        enum FixProductSubStep
        {
            Move2Mark1, // 正在向Mark1移动(XY  位置)
            VisionCfg1, //正在调整视觉参数1（包含Z轴在内）
            SnapMark1, //Mark1拍照
            Move2Mark2,//正在向Mark2移动（XY位置）
            VisionCfg2,
            SnapMark2, //Mark2拍照
        }

        protected override void RunLoopInWork()
        {
            _ChangeCS(TVStatus.等待指令);
            int customCmd = 0;
            if(!WaitCmd(out customCmd,CycleMilliseconds))
            {
                Thread.Sleep(CycleMilliseconds * 2);
                return;
            }
            if(customCmd == (int)TVCommand.产品定位)
            {
                _FixProduct();
                
            }
            else if(customCmd == (int)TVCommand.IC定位)
            {
                _FixIC();
            }
            else if(customCmd == (int)TVCommand.FOV定位)
            {
                _FixFOV();
            }
            else if(customCmd == (int)TVCommand.Task视觉配置)
            {
                _FixTaskVisionCfg();
            }
            else if(customCmd == (int)TVCommand.定位Mark)
            {
                _FixMark();
            }
            else if(customCmd == (int)TVCommand.采集模板图像)
            {
                _GrabModelPicture();
            }
            else if(customCmd == (int)TVCommand.FOV视觉测试)
            {
                _InspectFov();
            }
            else
            {
                RespCmd(JFWorkCmdResult.IllegalCmd);
                SendMsg2Outter("未定义的工站指令:" + customCmd);
            }

        }
        JFDLAFProductRecipe _currRecipe = null;
        FixProductSubStep _fpSubStep = FixProductSubStep.Move2Mark1;
        IJFImage[] _markImages = new IJFImage[] { null, null };//两个Mark点的拍照图片
        JFMultiAxisPosition[] _markPositions = new JFMultiAxisPosition[] { new JFMultiAxisPosition(), new JFMultiAxisPosition() };

        List<JFMultiAxisPosition> _lstICCenterSnapPos = new List<JFMultiAxisPosition>();
        List<double[]> _lstFovOffset = new List<double[]>();
        /// <summary>
        /// 执行产品定位动作
        /// </summary>
        void _FixProduct()
        {
            if (string.IsNullOrEmpty(_recipeID))
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("产品定位失败:RecipeID未设置");
                return;
            }
            //IJFRecipe iRecipe = JFHubCenter.Instance.RecipeManager.GetRecipe(CategotyProduct, _recipeID);
            if (null == _currRecipe)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("产品定位失败:RecipeID = \"" + _recipeID + "\" 产品配置不存在！");
                return;
            }
            RespCmd(JFWorkCmdResult.Success);
            _ChangeCS(TVStatus.定位产品);
            _isProductFixed = false;
            _isIcFixed = false;
            _currIcRow = -1; //当前定位的IC行数
            _currIcCol = -1; //当前定位的IC列数
            _isFovFixed = false;

            if (!_isFixProductByVision) //不使用视觉定位
            {
                SendMsg2Outter("未使用视觉定位，将使用产品IC原始坐标拍照");
                _lstICCenterSnapPos.Clear();
                for (int i = 0; i < _currRecipe.RowCount; i++)
                    for (int j = 0; j < _currRecipe.ColCount; j++)
                    {
                        double x = 0, y = 0;
                        _currRecipe.GetICSnapCenter(i, j, out x, out y);
                        JFMultiAxisPosition mp = new JFMultiAxisPosition();
                        mp.SetAxisPos(_gNameAxisX, x);
                        mp.SetAxisPos(_gNameAxisY, y);
                        _lstICCenterSnapPos.Add(mp);
                    }
                _lstFovOffset.Clear();
                for(int i = 0; i <_currRecipe.FovCount;i++)
                {
                    string[] fovNames = _currRecipe.FovNames();
                    double[] fovOffset = new double[2] { 0, 0 };
                    _currRecipe.GetFovOffset(_currRecipe.FovNames()[i], out fovOffset[0], out fovOffset[1]);
                    _lstFovOffset.Add(fovOffset);
                }

            }
            else
            {
                _lstICCenterSnapPos.Clear();
                _lstFovOffset.Clear();
                string errorInfo;
                string cmrDataFile = GetCfgParamValue(SCItemName_CmrCalibDataFile) as string;
                int imgWidth = (int)GetCfgParamValue(SCItemName_ImageWidth);
                int imgHeight = (int)GetCfgParamValue(SCItemName_ImageHeight);
                if(!_prodVisionFixer.Init(cmrDataFile,_currRecipe,imgWidth,imgHeight,out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("定位算子初始化失败，ErrorInfo:" + errorInfo );
                    return;
                }


                double x = 0, y = 0;
                _currRecipe.GetMarkSnapPos1(out x, out y);
                _markPositions[0].SetAxisPos(_gNameAxisX, x);
                _markPositions[0].SetAxisPos(_gNameAxisY, y);

                _currRecipe.GetMarkSnapPos2(out x, out y);
                _markPositions[1].SetAxisPos(_gNameAxisX, x);
                _markPositions[1].SetAxisPos(_gNameAxisY, y);


                
                if (!MoveToPosition(_markPositions[0], out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("向Mark1移动失败");
                    return;
                }
                _fpSubStep = FixProductSubStep.Move2Mark1;
                if (!WaitToPosition(_markPositions[0], out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("等待移动到Mark1失败");
                    return;
                }
                string mark1VisionCfgName = _currRecipe.GetMark1LightCfg();
                if (string.IsNullOrEmpty(mark1VisionCfgName))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("Mark1视觉参数未设置！");
                    return;
                }
                JFVisionManager vm = JFHubCenter.Instance.VisionMgr;
                if (!vm.ContainSingleVisionCfgByName(mark1VisionCfgName))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("Mark1视觉参数配置名:" + mark1VisionCfgName + " 在视觉配置表中不存在");
                    return;
                }
                _fpSubStep = FixProductSubStep.SnapMark1;
                if (!AdjustSingleVisionCfgAlias(DevAN_Cmr, mark1VisionCfgName, out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("调整Mark1视觉参数配置:\"" + mark1VisionCfgName + "\"失败，Error:" + errorInfo);
                    return;
                }
                if (JFWorkCmdResult.Success != WaitSingleVisionCfgAdjustDone(mark1VisionCfgName, out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("等待调整Mark1视觉参数配置:\"" + mark1VisionCfgName + "\"失败，Error:" + errorInfo);
                    return;
                }
                IJFImage img;
                if (!SnapCmrImageAlias(DevAN_Cmr, out img, out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("Mark1拍照失败:" + errorInfo);
                    return;
                }
                _markImages[0] = img;
                NotifyCustomizeMsg("ShowJFImage", new object[] { img });//


                _fpSubStep = FixProductSubStep.Move2Mark2;
                if (!MoveToPosition(_markPositions[1], out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("向Mark2移动失败");
                    return;
                }

                if (!WaitToPosition(_markPositions[1], out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("等待移动到Mark2失败");
                    return;
                }
                string mark2VisionCfgName = _currRecipe.GetMark2LightCfg();
                if (string.IsNullOrEmpty(mark2VisionCfgName))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("Mark2视觉参数未设置！");
                    return;
                }
                if (!vm.ContainSingleVisionCfgByName(mark2VisionCfgName))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("Mark2视觉参数配置名:" + mark1VisionCfgName + " 在视觉配置表中不存在");
                    return;
                }
                _fpSubStep = FixProductSubStep.SnapMark2;
                if (!AdjustSingleVisionCfgAlias(DevAN_Cmr, mark2VisionCfgName, out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("调整Mark2视觉参数配置:\"" + mark2VisionCfgName + "\"失败，Error:" + errorInfo);
                    return;
                }
                if (JFWorkCmdResult.Success != WaitSingleVisionCfgAdjustDone(mark2VisionCfgName, out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("等待调整Mark2视觉参数配置:\"" + mark2VisionCfgName + "\"失败，Error:" + errorInfo);
                    return;
                }
                if (!SnapCmrImageAlias(DevAN_Cmr, out img, out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("Mark1拍照失败:" + errorInfo);
                    return;
                }
                _markImages[1] = img;
                NotifyCustomizeMsg("ShowJFImage", new object[] { img });//
                                                                        ///////////////////////////添加Mark定位算法
                                                                        //JFMethodFlow mfFixPostion = GetWorkFlow(MFName_FixProduct);
                                                                        //mfFixPostion.Reset();
                                                                        //mfFixPostion.SetDP(MFDP_RecipeID, _recipeID);
                                                                        //mfFixPostion.SetDP(MFDP_MarkSnapX1, _markPositions[0].GetAxisPos(_gNameAxisX));
                                                                        //mfFixPostion.SetDP(MFDP_MarkSnapY1, _markPositions[0].GetAxisPos(_gNameAxisY));
                                                                        //mfFixPostion.SetDP(MFDP_MarkImage1, _markImages[0]);

                //mfFixPostion.SetDP(MFDP_MarkSnapX2, _markPositions[1].GetAxisPos(_gNameAxisX));
                //mfFixPostion.SetDP(MFDP_MarkSnapY2, _markPositions[1].GetAxisPos(_gNameAxisY));
                //mfFixPostion.SetDP(MFDP_MarkImage2, _markImages[1]);
                //// 调用定位方法流
                //if (!mfFixPostion.Action())
                //{
                //    _ChangeCS(TVStatus.等待指令);
                //    SendMsg2Outter( "产品定位失败,动作流执行出错:" +mfFixPostion.ActionErrorInfo());
                //}


                //if (PFErrorCode != 0) //方法流定位结果
                //{
                //    _ChangeCS(TVStatus.等待指令);
                //    SendMsg2Outter("产品定位失败，视觉定位算法:" + PFErrorInfo);
                //}
                //_lstICCenterSnapPos.Clear();

                //for (int i = 0; i < PFICCenterX.Length; i++)
                //{
                //    JFMultiAxisPosition mp = new JFMultiAxisPosition();
                //    mp.SetAxisPos(_gNameAxisX, PFICCenterX[i]);
                //    mp.SetAxisPos(_gNameAxisY, PFICCenterY[i]);
                //    _lstICCenterSnapPos.Add(mp);
                //}


                //_lstFovOffset.Clear();
                //for (int i = 0; i < _currRecipe.FovCount; i++)
                //{
                //    double[] fovOffset = new double[2] { PFFovOffsetX[i], PFFovOffsetY[i] };
                //    _lstFovOffset.Add(fovOffset);
                //}


                double[] icCenterXs;
                double[] icCenterYs;
                double[] fovOffsetXs;
                double[] fovOffsetYs;
                if(!_prodVisionFixer.CalibProduct(_markImages,
                                            new double[] { _markPositions[0].Positions[0].Position, _markPositions[1].Positions[0].Position, },
                                            new double[] { _markPositions[0].Positions[1].Position, _markPositions[1].Positions[1].Position, },
                                            out icCenterXs, out icCenterYs,out fovOffsetXs,out fovOffsetYs,out errorInfo))
                {
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("产品定位失败，定位算子ErrorInfo:" + errorInfo);
                }
                for(int i = 0; i < icCenterXs.Length;i++)
                {
                    JFMultiAxisPosition mp = new JFMultiAxisPosition();
                        mp.SetAxisPos(_gNameAxisX, icCenterXs[i]);
                        mp.SetAxisPos(_gNameAxisY, icCenterYs[i]);
                        _lstICCenterSnapPos.Add(mp);
                }

                for(int i = 0; i < fovOffsetXs.Length;i++)
                {
                    double[] fovOffset = new double[2] { fovOffsetXs[i], fovOffsetYs[i] };
                    _lstFovOffset.Add(fovOffset);
                }

            }
            
        
            _isProductFixed = true;
            _ChangeCS(TVStatus.等待指令);
            SendMsg2Outter("产品定位完成");
        }


        
        /// <summary>
        /// 定位IC
        /// </summary>
        void _FixIC()
        {
            if(!_isProductFixed)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("定位IC失败:产品定位未完成");
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            if(_currIcRow < 0 || _currIcRow >= _currRecipe.RowCount)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("定位IC失败:ICRow = " + _currIcRow + " 超出允许范围0~" + (_currRecipe.RowCount - 1));
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            if(_currIcCol < 0 || _currIcCol >= _currRecipe.ColCount)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("定位IC失败:ICCol = " + _currIcRow + " 超出允许范围0~" + (_currRecipe.ColCount - 1));
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            RespCmd(JFWorkCmdResult.Success);
            SendMsg2Outter("开始定位IC:Row = " + _currIcRow + " Col=" + _currIcCol);

            
           
            JFMultiAxisPosition icPos = _lstICCenterSnapPos[_currIcRow*_currRecipe.ColCount + _currIcCol];
            icPos.Name = "IC-Row:" + _currIcRow + "-Col:" + _currIcCol;
            string errorInfo;
            if(!MoveToPosition(icPos,out errorInfo))
            {
                SendMsg2Outter("定位IC失败:" + errorInfo);
                _ChangeCS(TVStatus.等待指令);
                return;
            }
            _ChangeCS(TVStatus.向指定IC移动);
            if (!WaitToPosition(icPos,out errorInfo))
            {
                SendMsg2Outter("定位IC失败:" + errorInfo);
                _ChangeCS(TVStatus.等待指令);
                return;
            }
            _isIcFixed = true;
            SendMsg2Outter("定位IC成功");
            NotifyCustomizeMsg("SnapShow", null);//
            _ChangeCS(TVStatus.等待指令);
           


        }

        /// <summary>
        /// 
        /// </summary>
        void _FixFOV()
        {
            if (!_isProductFixed)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("定位FOV失败:产品定位未完成");
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            if(!_isIcFixed)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("定位FOV失败:产品定位未完成");
                _ChangeCS(TVStatus.等待指令);
                return;
            }
            if(string.IsNullOrEmpty(_currFovName))
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("定位FOV失败:FovName未设定");
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            string[] fovNames = _currRecipe.FovNames();
            if(null == fovNames || fovNames.Length == 0)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("定位FOV失败:Recipe = " +_recipeID + " 未包含FOV信息");
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            if(!fovNames.Contains(_currFovName))
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("定位FOV失败,无效的FovName:" + _currFovName);
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            RespCmd(JFWorkCmdResult.Success);
            SendMsg2Outter("开始定位FovName = " + _currFovName);


            JFMultiAxisPosition fovPos = new JFMultiAxisPosition();
            fovPos.Name = _currFovName;
            int fovIndex = 0;
            string[] allFovNames = _currRecipe.FovNames();

            for(int i = 0; i < allFovNames.Length;i++)
                if(_currFovName == allFovNames[i])
                {
                    fovIndex = i;
                    break;
                }

            fovPos.SetAxisPos(_gNameAxisX, _lstICCenterSnapPos[_currIcRow * _currRecipe.ColCount + _currIcCol].GetAxisPos(_gNameAxisX) + _lstFovOffset[fovIndex][0]);
            fovPos.SetAxisPos(_gNameAxisY, _lstICCenterSnapPos[_currIcRow * _currRecipe.ColCount + _currIcCol].GetAxisPos(_gNameAxisY) + _lstFovOffset[fovIndex][1]);
            string errorInfo;
            if (!MoveToPosition(fovPos, out errorInfo))
            {
                SendMsg2Outter("定位Fov失败:" + errorInfo);
                _ChangeCS(TVStatus.等待指令);
                return;
            }
            _ChangeCS(TVStatus.向指定FOV移动);
            if (!WaitToPosition(fovPos, out errorInfo))
            {
                SendMsg2Outter("定位Fov失败:" + errorInfo);
                _ChangeCS(TVStatus.等待指令);
                return;
            }
            SendMsg2Outter("定位Fov成功");
            NotifyCustomizeMsg("SnapShow", null);//
            _isFovFixed = true;
            _ChangeCS(TVStatus.等待指令);


        }


        /// <summary>
        /// 根据Task名称调整视觉参数
        /// </summary>
        void _FixTaskVisionCfg()
        {
            if(!_isFovFixed)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("调整Task视觉配置失败:Fov定位未完成");
                _ChangeCS(TVStatus.等待指令);
            }

            if(string.IsNullOrEmpty(_currTaskName))
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("调整Task视觉配置失败:Task Name未设置");
                _ChangeCS(TVStatus.等待指令);
            }

            string[] taskNames = _currRecipe.TaskNames(_currFovName);
            if(null == taskNames || 0 == taskNames.Length)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("调整Task视觉配置失败:当前FOV:" + _currFovName + " 未包含任何TaskName");
                _ChangeCS(TVStatus.等待指令);
            }


            if(!taskNames.Contains(_currTaskName))
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("调整Task视觉配置失败:当前FOV:" + _currFovName + " 未包含TaskName = " + _currTaskName);
                _ChangeCS(TVStatus.等待指令);
            }

            string vcName = _currRecipe.VisionCfgName(_currFovName, _currTaskName);
            if(string.IsNullOrEmpty(vcName))
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("调整Task视觉配置失败:当前Task:" +  _currTaskName + "视觉参数为设置");
                _ChangeCS(TVStatus.等待指令);
            }


            JFVisionManager vm = JFHubCenter.Instance.VisionMgr;
            if(!vm.ContainSingleVisionCfgByName(vcName))
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("调整Task视觉配置失败:当前Task:" + _currTaskName + "视觉参数未设置");
                _ChangeCS(TVStatus.等待指令);
            }
            
            RespCmd(JFWorkCmdResult.Success);
            _ChangeCS(TVStatus.调整Task视觉配置);
            SendMsg2Outter("开始调整视觉参数...");
            string errorInfo;
            if(!AdjustSingleVisionCfgAlias(DevAN_Cmr,vcName,out errorInfo))
            {
                SendMsg2Outter("调整Task视觉配置失败:" + errorInfo);
                _ChangeCS(TVStatus.等待指令);
            }
            if(JFWorkCmdResult.Success !=WaitSingleVisionCfgAdjustDone(vcName,out errorInfo))
            {
                SendMsg2Outter("调整Task视觉配置失败:" + errorInfo);
                _ChangeCS(TVStatus.等待指令);
            }
            SendMsg2Outter("调整Task视觉配置完成！");
            NotifyCustomizeMsg("SnapShow", null);//
            _ChangeCS(TVStatus.等待指令);

        }


        int _markIndexWillFix = 0; //需要定位的Mark点序号
        /// <summary>
        /// 定位Mark点
        /// 线程函数内部调用
        /// </summary>
        void _FixMark()
        {
            if (string.IsNullOrEmpty(_recipeID))
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("定位Mark" + (_markIndexWillFix + 1) +  " 失败:RecipeID未设置");
                return;
            }
            if (null == _currRecipe)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("定位Mark" + (_markIndexWillFix + 1) + " 失败:产品配置不存在！");
                return;
            }
            RespCmd(JFWorkCmdResult.Success);
            SendMsg2Outter("开始定位Mark" + (_markIndexWillFix + 1) + "...");
            double x = 0, y = 0;
            if (0 == _markIndexWillFix)
            {
                _currRecipe.GetMarkSnapPos1(out x, out y);
                _ChangeCS(TVStatus.定位Mark1);
            }
            else
            {
                _currRecipe.GetMarkSnapPos2(out x, out y);
                _ChangeCS(TVStatus.定位Mark2);
            }
            JFMultiAxisPosition markPos = new JFMultiAxisPosition();
            markPos.Name = "Mark" + (_markIndexWillFix + 1);
            markPos.Positions.Add(JFAxisPos.Create(_gNameAxisX, x));
            markPos.Positions.Add(JFAxisPos.Create(_gNameAxisY, y));
            string errorInfo;
            if(!MoveToPosition(markPos,out errorInfo))
            {
                SendMsg2Outter("Mark定位失败:" + errorInfo);
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            if(!WaitToPosition(markPos,out errorInfo))
            {
                SendMsg2Outter("Mark定位失败:" + errorInfo);
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            SendMsg2Outter("Mark定位完成");
            string markVCName = "";
            if (_markIndexWillFix == 0)
                markVCName = _currRecipe.GetMark1LightCfg();
            else
                markVCName = _currRecipe.GetMark2LightCfg();
            if(string.IsNullOrEmpty(markVCName))
            {
                SendMsg2Outter("调整Mark视觉参数失败:Mark视觉参数未设置！");
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            if(!AdjustSingleVisionCfgAlias(DevAN_Cmr, markVCName,out errorInfo))
            {
                SendMsg2Outter("调整Mark视觉参数失败" + errorInfo);
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            if(JFWorkCmdResult.Success != WaitSingleVisionCfgAdjustDone(markVCName,out errorInfo))
            {
                SendMsg2Outter("调整Mark视觉参数失败" + errorInfo);
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            SendMsg2Outter("调整Mark视觉参数完成:VisionConfig = " + markVCName);
            _ChangeCS(TVStatus.等待指令);

        }


        void _GrabModelPicture()
        {
            if (!_isProductFixed)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("采集产品图像木板失败:产品定位未完成");
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            RespCmd(JFWorkCmdResult.Success);
            _ChangeCS(TVStatus.采集模板图像);


            if (!Directory.Exists(_modelPicFolder)) //当前
            {
                ChangeWorkStatus(JFWorkStatus.Interactiving);
                string[] selInfo = new[] { "创建文件夹并继续", "取消当前操作（模板图像采集）" };
                if (1 == JFSelectTipsUI.ShowDialog("模板保存路径:\"" + _modelPicFolder + "\"不存在",selInfo,1))
                {
                    ChangeWorkStatus(JFWorkStatus.Running);
                    _ChangeCS(TVStatus.等待指令);
                    SendMsg2Outter("模板图像采集任务 已取消！");
                    return;
                }
                ChangeWorkStatus(JFWorkStatus.Running);
                DirectoryInfo di = Directory.CreateDirectory(_modelPicFolder);
                if (di == null || !di.Exists)
                {
                    SendMsg2Outter("创建模板图像保存路径:\"" + _modelPicFolder + "\"失败，采集任务取消");
                    _ChangeCS(TVStatus.等待指令);
                    return;
                }
            }

            string errorInfo;
            DateTime startTime = DateTime.Now;
            //string productPath = _modelPicFolder + "\\" + _recipeID + "\\"; //模板图像的Recipe文件夹
            JFMultiAxisPosition fovPos = new JFMultiAxisPosition();
            int rowCount = _currRecipe.RowCount;
            int colCount = _currRecipe.ColCount;
            SendMsg2Outter("开始采集模板图像");
            for(int i = 0; i < _currRecipe.RowCount;i++)
                for(int j = 0; j < _currRecipe.ColCount;j++)
                    for(int k = 0; k < _currRecipe.FovCount;k++)
                    {
                        string fovName = _currRecipe.FovNames()[k];
                        fovPos.SetAxisPos(_gNameAxisX, _lstICCenterSnapPos[i * colCount + j].GetAxisPos(_gNameAxisX) + _lstFovOffset[k][0]);
                        fovPos.SetAxisPos(_gNameAxisY, _lstICCenterSnapPos[i * colCount + j].GetAxisPos(_gNameAxisY) + _lstFovOffset[k][1]);
                        fovPos.Name = fovName;


                        string[] taskNames = _currRecipe.TaskNames(fovName);
                        string vcName = _currRecipe.VisionCfgName(fovName, taskNames[0]);
                        if (string.IsNullOrEmpty(vcName))
                        {
                            SendMsg2Outter(string.Format("采集模板图像失败, Fov:{0} Task:{1} 视觉参数未设置", fovName, taskNames[0]));
                            _ChangeCS(TVStatus.等待指令);
                            return;
                        }
                        if (!AdjustSingleVisionCfgAlias(DevAN_Cmr, vcName, out errorInfo))
                        {
                            SendMsg2Outter(string.Format("采集模板图像失败, Fov:{0} Task:{1} Vc:{2} 调整视觉参数出错，Error：{3}", fovName, taskNames[0], vcName, errorInfo));
                            _ChangeCS(TVStatus.等待指令);
                            return;
                        }





                        if (!MoveToPosition(fovPos,out errorInfo))
                        {
                            SendMsg2Outter(string.Format("采集模板图像失败，未能移动到Row:{0}-Col:{1} Fov:{2},ErrorInfo:{3}", i, j, fovName, errorInfo));
                            _ChangeCS(TVStatus.等待指令);
                            return;
                        }

                        string.Format("开始移动到Row:{0}-Col:{1} Fov:{2}", i, j, fovName);
                        if(!WaitToPosition(fovPos,out errorInfo))
                        {
                            SendMsg2Outter(string.Format("采集模板图像失败，等待移动到Row:{0}-Col:{1} Fov:{2},ErrorInfo:{3}", i, j, fovName, errorInfo));
                            _ChangeCS(TVStatus.等待指令);
                            return;
                        }
                        
                        for(int h = 0; h < taskNames.Length;h++)
                        {
                            CheckCmd(CycleMilliseconds);
                            //string vcName = _currRecipe.VisionCfgName(fovName,taskNames[h]);
                            //if(string.IsNullOrEmpty(vcName))
                            //{
                            //    SendMsg2Outter(string.Format("采集模板图像失败, Fov:{0} Task:{1} 视觉参数未设置",fovName, taskNames[h]));
                            //    _ChangeCS(TVStatus.等待指令);
                            //    return;
                            //}
                            //if(!AdjustSingleVisionCfgAlias(DevAN_Cmr,vcName,out errorInfo))
                            //{
                            //    SendMsg2Outter(string.Format("采集模板图像失败, Fov:{0} Task:{1} Vc:{2} 调整视觉参数出错，Error：{3}", fovName, taskNames[h],vcName,errorInfo));
                            //    _ChangeCS(TVStatus.等待指令);
                            //    return;
                            //}
                            SendMsg2Outter("开始调整视觉参数:" + vcName);
                            if(JFWorkCmdResult.Success != WaitSingleVisionCfgAdjustDone(vcName,out errorInfo))
                            {
                                SendMsg2Outter(string.Format("采集模板图像失败, Fov:{0} Task:{1} Vc:{2} 等待调整视觉参数完成出错，Error：{3}", fovName, taskNames[h], vcName, errorInfo));
                                _ChangeCS(TVStatus.等待指令);
                                return;
                            }

                            //Thread.Sleep(100);

                            IJFImage img = null;
                            if(!SnapCmrImageAlias(DevAN_Cmr,out img,out errorInfo))
                            {
                                SendMsg2Outter("采集模板图像失败，相机采图出错:" + errorInfo);
                                _ChangeCS(TVStatus.等待指令);
                                return;
                            }
                            NotifyCustomizeMsg("ShowJFImage", new object[] { img });
                            string fp = _modelPicFolder + "\\" + _recipeID + "\\" + startTime.ToString("yyyyMMdd-HH-mm-ss") + "\\Row_" + i + "-Col_" + j  +"-Fov_" + fovName + "\\" + taskNames[h] + ".tiff"; //模板图像的Recipe文件夹
                            int errCode = img.Save(fp, JFImageSaveFileType.Tif);
                            if (0 != errCode)
                                SendMsg2Outter("保存图像到:" + fp + " 失败，ErrorInfo:" + img.GetErrorInfo(errCode));
                            else
                                SendMsg2Outter("图像已保存到:" + fp);

                            if (h < taskNames.Length - 1)
                            {
                                vcName = _currRecipe.VisionCfgName(fovName, taskNames[h + 1]);
                                if (string.IsNullOrEmpty(vcName))
                                {
                                    SendMsg2Outter(string.Format("采集模板图像失败, Fov:{0} Task:{1} 视觉参数未设置", fovName, taskNames[h + 1]));
                                    _ChangeCS(TVStatus.等待指令);
                                    return;
                                }
                                if (!AdjustSingleVisionCfgAlias(DevAN_Cmr, vcName, out errorInfo))
                                {
                                    SendMsg2Outter(string.Format("采集模板图像失败, Fov:{0} Task:{1} Vc:{2} 调整视觉参数出错，Error：{3}", fovName, taskNames[h + 1], vcName, errorInfo));
                                    _ChangeCS(TVStatus.等待指令);
                                    return;
                                }
                            }


                        }
                    }
            NotifyCustomizeMsg("SnapShow", null);//
            _isFovFixed = true;
            _ChangeCS(TVStatus.等待指令);
        }


        void _InspectFov()
        {
            if (!_isProductFixed)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("Fov视觉检测失败:产品定位未完成");
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            if (_inspectRow < 0 || _inspectRow >= _currRecipe.RowCount)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("Fov视觉检测失败:非法的Row = " + _inspectRow);
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            if (_inspectCol < 0 || _inspectCol >= _currRecipe.ColCount)
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("Fov视觉检测失败:非法的Col = " + _inspectCol);
                _ChangeCS(TVStatus.等待指令);
                return;
            }


            if (string.IsNullOrEmpty(_inspectFovName))
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("Fov视觉检测失败:FovName 为空字串");
                _ChangeCS(TVStatus.等待指令);
                return;
            }
            string[] allFovNames = _currRecipe.FovNames();
            if (!allFovNames.Contains(_inspectFovName))
            {
                RespCmd(JFWorkCmdResult.ActionError);
                SendMsg2Outter("Fov视觉检测失败:非法的FovName = " + _inspectFovName);
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            int fovIndex = 0;
            for (int i = 0; i < allFovNames.Length; i++)
                if (_inspectFovName == allFovNames[i])
                {
                    fovIndex = i;
                    break;
                }


            RespCmd(JFWorkCmdResult.Success);
            _ChangeCS(TVStatus.Fov视觉检测);
            DateTime startTime = DateTime.Now;

            List<IJFImage> lstTaskImages = new List<IJFImage>();

            string errorInfo;
            JFMultiAxisPosition fovPos = new JFMultiAxisPosition();
            fovPos.SetAxisPos(_gNameAxisX, _lstICCenterSnapPos[_inspectRow * _currRecipe.ColCount + _inspectCol].GetAxisPos(_gNameAxisX) + _lstFovOffset[fovIndex][0]);
            fovPos.SetAxisPos(_gNameAxisY, _lstICCenterSnapPos[_inspectRow * _currRecipe.ColCount + _inspectCol].GetAxisPos(_gNameAxisY) + _lstFovOffset[fovIndex][1]);
            fovPos.Name = _inspectFovName;
            if (!MoveToPosition(fovPos, out errorInfo))
            {
                SendMsg2Outter(string.Format("Fov图像检测失败，未能移动到Row:{0}-Col:{1} Fov:{2},ErrorInfo:{3}", _inspectRow, _inspectCol, _inspectFovName, errorInfo));
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            string.Format("开始移动到Row:{0}-Col:{1} Fov:{2}", _inspectRow, _inspectCol, _inspectFovName);
            if (!WaitToPosition(fovPos, out errorInfo))
            {
                SendMsg2Outter(string.Format("Fov图像检测失败，等待移动到Row:{0}-Col:{1} Fov:{2},ErrorInfo:{3}", _inspectRow, _inspectCol, _inspectFovName, errorInfo));
                _ChangeCS(TVStatus.等待指令);
                return;
            }
            string[] taskNames = _currRecipe.TaskNames(_inspectFovName);
            for (int h = 0; h < taskNames.Length; h++)
            {
                CheckCmd(CycleMilliseconds);
                string vcName = _currRecipe.VisionCfgName(_inspectFovName, taskNames[h]);
                if (string.IsNullOrEmpty(vcName))
                {
                    SendMsg2Outter(string.Format("Fov图像检测失败, Fov:{0} Task:{1} 视觉参数未设置", _inspectFovName, taskNames[h]));
                    _ChangeCS(TVStatus.等待指令);
                    return;
                }
                if (!AdjustSingleVisionCfgAlias(DevAN_Cmr, vcName, out errorInfo))
                {
                    SendMsg2Outter(string.Format("Fov图像检测失败, Fov:{0} Task:{1} Vc:{2} 调整视觉参数出错，Error：{3}", _inspectFovName, taskNames[h], vcName, errorInfo));
                    _ChangeCS(TVStatus.等待指令);
                    return;
                }
                SendMsg2Outter("开始调整视觉参数:" + vcName);
                if (JFWorkCmdResult.Success != WaitSingleVisionCfgAdjustDone(vcName, out errorInfo))
                {
                    SendMsg2Outter(string.Format("Fov图像检测失败, Fov:{0} Task:{1} Vc:{2} 等待调整视觉参数完成出错，Error：{3}", _inspectFovName, taskNames[h], vcName, errorInfo));
                    _ChangeCS(TVStatus.等待指令);
                    return;
                }

                IJFImage img = null;
                if (!SnapCmrImageAlias(DevAN_Cmr, out img, out errorInfo))
                {
                    SendMsg2Outter("Fov图像检测失败，相机采图出错:" + errorInfo);
                    _ChangeCS(TVStatus.等待指令);
                    return;
                }
                NotifyCustomizeMsg("ShowJFImage", new object[] { img });
                string fp = _modelPicFolder + "\\示教测试图片\\" + _recipeID + "\\" + startTime.ToString("yyyyMMdd-HH-mm-ss") + "\\Row_" + _inspectRow + "-Col_" + _inspectCol + "-Fov_" + _inspectFovName + "\\" + taskNames[h] + ".BMP"; //模板图像的Recipe文件夹
                int errCode = img.Save(fp);
                if (0 != errCode)
                {
                    SendMsg2Outter("保存图像到:" + fp + " 失败，ErrorInfo:" + img.GetErrorInfo(errCode));
                }
                else
                    SendMsg2Outter("图像已保存到:" + fp);
                lstTaskImages.Add(img);
                
            }

            JFMethodFlow mfInspect = GetWorkFlow(MFName_Inspect);
            mfInspect.Reset();
            mfInspect.SetDP("RecipeID", _recipeID);
            mfInspect.SetDP("ICRow", _inspectRow);
            mfInspect.SetDP("ICCol", _inspectCol);
            mfInspect.SetDP("FovName", _inspectFovName);
            mfInspect.SetDP("TaskNames", taskNames);
            mfInspect.SetDP("Images", lstTaskImages.ToArray());
            mfInspect.SetDP("ResultHandle", this); //处理检测结果
            if(!mfInspect.Action())
            {
                SendMsg2Outter("Fov图像检测失败:" + mfInspect.ActionErrorInfo());
                _ChangeCS(TVStatus.等待指令);
                return;
            }

            SendMsg2Outter("图像检测完成!");
            //添加显示检测结果
            object hoImg;
            lstTaskImages[0].GenHalcon(out hoImg);
            NotifyCustomizeMsg("ShowInspectResult", new object[]
            {
                lstTaskImages.ToArray(), //Images
                InspectedRecipeID,
                InspectedICRow,
                InspectedICCol,
                InspectedFovName,
                InspectedResult, //Bool
                InspectedErrorInfo,//string
                InspectedDiesErrorCodes,
                InspectedDieErrorInfos,
                InspectedBondContours,
                InspectedWires,
                InspectedFailRegs
            }) ;

            _ChangeCS(TVStatus.等待指令);

        }


        /// <summary>
        /// 工站结批
        /// </summary>
        protected override void ExecuteEndBatch()
        {
            return;
        }

        /// <summary>
        /// 退出前清理
        /// </summary>
        protected override void CleanupWhenWorkExit()
        {
            StopAxisAlias(DevAN_AxisX);
            StopAxisAlias(DevAN_AxisY);
            StopAxisAlias(DevAN_AxisZ);
            //string errorInfo;
            //string[] cmrNames = CameraNames;
            //if (null != cmrNames)
            //    foreach (string cn in cmrNames)
            //        if (!EnableCmrGrab(cn, false, out errorInfo))
            //            ;//SendMsg2Outter("关闭相机采集失败,CmrName:\"" + cn + "\" ErrorInfo:" + errorInfo);


            //string axisXGlobName = GetDecChnGlobName(NamedChnType.Axis, DevAN_AxisX);


            return;
        }


        #region IDLAFInspectResultHandle's Implement
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

        //视觉算子是否运行出错
        public bool InspectedResult { get; set; }

        /// <summary>
        ///  视觉检测流程错误信息
        /// </summary>
        public string InspectedErrorInfo { get; set; }


        /// <summary>
        /// 检测结果 0表示OK
        /// </summary>
        public List<int[]> InspectedDiesErrorCodes { get; set; }

        /// <summary>
        ///  检测错误信息
        /// </summary>
        public string[] InspectedDieErrorInfos { get; set; }

        public HObject InspectedBondContours { get; set; }
        public HObject InspectedWires { get; set; }
        public HObject InspectedFailRegs { get; set; }


        /// <summary>
        /// 其他可能存在的检测结果
        /// </summary>
        public Dictionary<string, object> InspectedReverse { get; set; }

        #endregion
    }
}
