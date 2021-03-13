using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LFAOIRecipe
{

    //InspectResultItem类：InspectNode提供输出数据的封装类型(Die中的单个检测项数据)
    public class InspectResultItem
    {

        public enum Categoty
        {
            Unknown = 0,
            Frame,
            IC,
            Bond,
            Wire,
            Epoxy
        }

        public InspectResultItem()
        {
            DieIndex = -1;
        }


        /// <summary>
        /// 判断检测结果是否OK
        /// </summary>
        /// <returns></returns>
        public bool IsDetectOK()
        {
            if (ErrorCodes == null || 0 == ErrorCodes.Length)
                return true;
            foreach (int err in ErrorCodes)
                if (0 != err)
                    return false;
            return true;

        }

        /// <summary>
        /// Die序号，Base 0
        /// </summary>
        public int DieIndex { get; set; }


        /// <summary>
        /// 检测项对应的图层序号
        /// </summary>
        public int ImageIndex { get; set; }

        /// <summary>
        /// 检测项类别：焊点/金线/银胶/框架/IC
        /// </summary>
        public Categoty InspectCategoty { get; set; }

        /// <summary>
        /// 检测项名称   焊点_1，_2 .../金线/银胶/框架/IC
        /// </summary>
        public string InspectID { get; set; }

        /// <summary>
        /// 错误码 0表示Pass ，其他值同DieErrorTypes
        /// </summary>
        public int[] ErrorCodes { get; set; }

        public string[] ErrorTexts 
        {
            get 
            {
                if (null == ErrorCodes || 0 == ErrorCodes.Length)
                    return new string[] { "None" };
                if (IsDetectOK())
                    return new string[] { "OK" };
                string[] ret = new string[ErrorCodes.Length];
                for (int i = 0; i < ErrorCodes.Length; i++)
                    ret[i] = InspectNode.DieErrorDescript(ErrorCodes[i]);
                return ret;
                    
            }
        }



        /// <summary>
        /// 检测标准描述信息 ，比如:灰度阈值，面积大小，长宽比等等
        /// </summary>
        public string QualifiedDescript { get; set; }

        /// <summary>
        /// 检测结果描述信息
        /// </summary>
        public string ResultDescript { get; set; }


        /// <summary>
        /// 需要检测的区域
        /// </summary>
        public HObject DetectRegion { get; set; }

        /// <summary>
        /// 结果区域，如果检测失败，此对象表示错误区域，
        /// </summary>
        public HObject ResultRegion { get; set; }

    }


    public class InspectNode : ViewModelBase, IProcedure
    {
        //1123
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        private int imageIndex = 0;
        public int ImageIndex
        {
            get => imageIndex;
            set
            {
                if (ImageVerify == null || !ImageVerify.IsInitialized())
                {
                    MessageBox.Show("请先加载检测验证图像！");
                }
                if (imageIndex != value)
                {
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, value);
                    if (ChannelImageVerify != null) htWindow.Display(ChannelImageVerify, true);
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        //1120
        private int switchImageChannelIndex = -1;
        /// <summary>
        /// 切换图像通道数
        /// </summary>
        public int SwitchImageChannelIndex
        {
            get => switchImageChannelIndex;
            set
            {
                if (ImageVerify == null || !ImageVerify.IsInitialized())
                {
                    //MessageBox.Show("请先加载检测验证图像！");
                }
                else
                { 
                    if (switchImageChannelIndex != value)
                    {
                        if (value == 0)
                        {
                            HOperatorSet.AccessChannel(ImageVerify, out HObject ChannelImage1, 1);
                            IniParameters.ImageIndex = 0;
                            htWindow.Display(ChannelImage1);
                        }
                        else if (value == 1)
                        {
                            HOperatorSet.AccessChannel(ImageVerify, out HObject ChannelImage2, 2);
                            IniParameters.ImageIndex = 1;
                            htWindow.Display(ChannelImage2);
                        }
                        else if (value == 2)
                        {
                            HOperatorSet.AccessChannel(ImageVerify, out HObject ChannelImage3, 3);
                            IniParameters.ImageIndex = 2;
                            htWindow.Display(ChannelImage3);
                        }
                        else if (value == 3)
                        {
                            HOperatorSet.AccessChannel(ImageVerify, out HObject ChannelImage4, 4);
                            IniParameters.ImageIndex = 3;
                            htWindow.Display(ChannelImage4);
                        }
                        else if (value > 4)
                        {
                            if (value > IniParameters.ImageCountChannels)
                            {
                                System.Windows.MessageBox.Show("通道数异常");
                                return;
                            }
                            HOperatorSet.AccessChannel(ImageVerify, out HObject ChannelImagen, value + 1);
                            IniParameters.ImageIndex = value;
                            htWindow.Display(ChannelImagen);

                        }
                        switchImageChannelIndex = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        private int isFovTaskFlag = 0;

        private int pImageIndex = -1;

        private bool IsLoadModel=false;

        private string pImageIndexPath;
        public string PImageIndexPath
        {
            get => pImageIndexPath;
            set => OnPropertyChanged(ref pImageIndexPath, value);
        }

        private bool isDisplayBonds;
        public bool IsDisplayBonds
        {
            get => isDisplayBonds;
            set => OnPropertyChanged(ref isDisplayBonds, value);
        }

        private HObject ImageVerifyConcat = null;  // 1124
        private HObject ImageVerify = null;
        private HObject ChannelImageVerify = null;

        private string verifyImagesDirectory = string.Empty;
        public string VerifyImagesDirectory
        {
            get => verifyImagesDirectory;
            set => OnPropertyChanged(ref verifyImagesDirectory, value);
        }

        private int currentVerifySet;
        public int CurrentVerifySet
        {
            get => currentVerifySet;
            set => OnPropertyChanged(ref currentVerifySet, value);
        }

        public CommandBase VerifyCommand { get; private set; }
        public CommandBase SaveCommand { get; private set; }
        public CommandBase ImagesSetVerifyCommand { get; private set; }
        public CommandBase RefreshImagesSetCommand { get; private set; }
        public CommandBase PreviousCommand { get; private set; }
        public CommandBase NextCommand { get; private set; }
        public CommandBase ClearModelCommand { get; private set; }

        private HTHalControlWPF htWindow;
 
        private readonly string ModelsFile;

        private readonly string RecipeFile;

        public IniParameters IniParameters { get; private set; }

        //private int imgIndex=0; 

        private HObject DieRegions = null;
        private HTuple InspectItemNum = new HTuple();

        private HObjectVector FrameObjs = new HObjectVector(2);
        private HObjectVector IcObjs = new HObjectVector(2);
        private HObjectVector EpoxyObjs = new HObjectVector(2);
        private HObjectVector BondObjs = new HObjectVector(2);
        private HObjectVector WireObjs = new HObjectVector(2);

        private HTupleVector FrameModels = new HTupleVector(2);
        private HTupleVector IcModels = new HTupleVector(2);
        private HTupleVector EpoxyModels = new HTupleVector(2);
        private HTupleVector BondModels = new HTupleVector(2);
        private HTupleVector WireModels = new HTupleVector(2);
        private HTupleVector CutRegModels = new HTupleVector(1);

        private HTupleVector Con_FrameInspectParas = new HTupleVector(3), Con_IcInspectParas = new HTupleVector(3);
        private HTupleVector Con_EpoxyInspectParas = new HTupleVector(4), Con_BondInspectParas = new HTupleVector(3);
        private HTupleVector Con_WireInspectParas = new HTupleVector(4);
        private HTupleVector Con_AroundBallInspectParas = new HTupleVector(2);
        //add by wj 12-22
        private HTuple imageWidth, imageHeight;
        public InspectNode(HTHalControlWPF htWindow,
                           IniParameters iniParameters,
                           string modelsFile, 
                           string recipeFile)
        {
            DisplayName = "单节点检测";
            if (null != iniParameters)
                Content = new Page_InspectNode { DataContext = this };
            else
                Content = null;
            this.htWindow = htWindow;
            this.IniParameters = iniParameters;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
 
            SaveCommand = new CommandBase(ExecuteSaveCommand);
            VerifyCommand = new CommandBase(ExecuteVerifyCommand);
            ImagesSetVerifyCommand = new CommandBase(ExecuteImagesSetVerifyCommand);
            RefreshImagesSetCommand = new CommandBase(ExecuteRefreshImagesSetCommand);
            PreviousCommand = new CommandBase(ExecutePreviousCommand);
            NextCommand = new CommandBase(ExecuteNextCommand);
            ClearModelCommand = new CommandBase(ExecuteClearModelCommand);
        }

        //所有参数加载
        private void ExecuteSaveCommand(object parameter)
        {
            try
             {            

                Algorithm.Model_RegionAlg.JSCC_AOI_read_all_model(out  /*{eObjectVector,Dim=2}*/ FrameObjs,
                                out  /*{eObjectVector,Dim=2}*/ IcObjs, 
                                out  /*{eObjectVector,Dim=2}*/ EpoxyObjs,
                                out  /*{eObjectVector,Dim=2}*/ BondObjs, 
                                out  /*{eObjectVector,Dim=2}*/ WireObjs,
                                ModelsFile, 
                                out InspectItemNum, 
                                out  /*{eTupleVector,Dim=2}*/ FrameModels,
                                out  /*{eTupleVector,Dim=2}*/ IcModels, 
                                out  /*{eTupleVector,Dim=2}*/ EpoxyModels,
                                out  /*{eTupleVector,Dim=2}*/ BondModels, 
                                out  /*{eTupleVector,Dim=2}*/ WireModels,
                                out CutRegModels,
                                out HTuple modelErrCode, out HTuple modelErrStr);

                if (modelErrCode < 0)
                {
                    MessageBox.Show("读Models文件异常！");
                    return;

                }

                // 修改 by 王静 2020-10-22
                Algorithm.Model_RegionAlg.JSCC_AOI_read_all_inspect_parametrer(RecipeFile,
                         out HTupleVector/*{eTupleVector,Dim=2}*/  FrameParameters,
                         out HTupleVector/*{eTupleVector,Dim=2}*/  IcParameters,
                         out HTupleVector/*{eTupleVector,Dim=2}*/  EpoxyParameters,
                         out HTupleVector/*{eTupleVector,Dim=3}*/  BondParameters,
                         out HTupleVector/*{eTupleVector,Dim=3}*/  WireParameters,
                         out /*{eTupleVector,Dim=2}*/  Con_AroundBallInspectParas,
                         out HTuple paraErrCode,
                         out HTuple paraErrStr);

                if (paraErrCode < 0)
                {
                    MessageBox.Show("读Recipe文件异常！");
                    return;

                }

                Con_FrameInspectParas = ((new HTupleVector(3).Insert(0, FrameModels)).Insert(
                    1, FrameParameters));
                Con_IcInspectParas = ((new HTupleVector(3).Insert(0, IcModels)).Insert(
                    1, IcParameters));
                Con_EpoxyInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, EpoxyModels)))).Insert(
                    1, EpoxyParameters));
                //modify by wj 2021-01-18
                Con_BondInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, BondModels)))).Insert(
                        1, BondParameters));
                Con_WireInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, WireModels)))).Insert(
                    1, WireParameters));

                //o_BondModels.at(fileidx) := { OnWhat,_InspectMethod,\  Model_Type,PosModel,BallNum_OnRegion,\
                //Match_StartAngle,Match_AngleExt,Match_MinScore,BallRadius}
                //o_BondModels.at(fileidx) := { OnWhat,_InspectMethod,MetrologyType,\ MetrologyHandle,BondOffsetFactor}

                IsLoadModel = true;
                MessageBox.Show("加载参数完成！");
            }
            catch (Exception ex)
            {
                 MessageBox.Show(ex.ToString());
            }
        }

        //清理内存
        private void ExecuteClearModelCommand(object parameter)
        {
            try
            {
                if (IsLoadModel != true)
                {
                    MessageBox.Show("未加载模板参数！");
                    return;
                }

                //清除模板 
                Algorithm.Model_RegionAlg.JSCC_AOI_clear_all_model(InspectItemNum, FrameModels, IcModels,
                                                                   BondModels, out HTuple _clearErrcode, out HTuple _clearErrStr);

                if ((int)(new HTuple(_clearErrcode.TupleLess(0))) != 0)
                {
                    MessageBox.Show("清理模板异常！");
                    return;
                }

                IsLoadModel = false;
                MessageBox.Show("清理模板完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region    /// 大软件调用函数接口 ///

        public InspectNode(string modelsFile, string recipeFile) : this(null, null, modelsFile, recipeFile)
        {

        }
        bool _isInitOK = false;
        string _initErrorInfo = "None-Opt"; //检测算子初始化错误信息
        /// <summary>
        /// 初始化视觉检测参数
        /// </summary>
        /// <returns></returns>
        public bool InitInspectParam(out string errorInfo)
        {
            ClearAllModels(out errorInfo);
            _isInitOK = false;
            try
            {
                // 避免图像外区域影响
                HOperatorSet.SetSystem("clip_region", "false");

                Algorithm.Model_RegionAlg.JSCC_AOI_read_all_model(out  /*{eObjectVector,Dim=2}*/ FrameObjs,
                                out  /*{eObjectVector,Dim=2}*/ IcObjs,
                                out  /*{eObjectVector,Dim=2}*/ EpoxyObjs,
                                out  /*{eObjectVector,Dim=2}*/ BondObjs,
                                out  /*{eObjectVector,Dim=2}*/ WireObjs,
                                ModelsFile,
                                out InspectItemNum,
                                out  /*{eTupleVector,Dim=2}*/ FrameModels,
                                out  /*{eTupleVector,Dim=2}*/ IcModels,
                                out  /*{eTupleVector,Dim=2}*/ EpoxyModels,
                                out  /*{eTupleVector,Dim=2}*/ BondModels,
                                out  /*{eTupleVector,Dim=2}*/ WireModels,
                                out CutRegModels,
                                out HTuple modelErrCode, out HTuple modelErrStr);

                if (modelErrCode < 0)
                {
                    //MessageBox.Show("读Models文件异常！");
                    //return;
                    _initErrorInfo = "Inspect_Node:读Models文件异常:ErrorCode = " + modelErrCode.I + " ErrorInfo:" + modelErrStr.S;
                    errorInfo = _initErrorInfo;
                    return false;
                }

                // 修改 by 王静 2020-10-22
                Algorithm.Model_RegionAlg.JSCC_AOI_read_all_inspect_parametrer(RecipeFile,
                         out HTupleVector/*{eTupleVector,Dim=2}*/  FrameParameters,
                         out HTupleVector/*{eTupleVector,Dim=2}*/  IcParameters,
                         out HTupleVector/*{eTupleVector,Dim=2}*/  EpoxyParameters,
                         out HTupleVector/*{eTupleVector,Dim=3}*/  BondParameters,
                         out HTupleVector/*{eTupleVector,Dim=3}*/  WireParameters,
                         out /*{eTupleVector,Dim=2}*/  Con_AroundBallInspectParas,
                         out HTuple paraErrCode,
                         out HTuple paraErrStr);

                if (paraErrCode < 0)
                {
                    //MessageBox.Show("读Recipe文件异常！");
                    //return;
                    _initErrorInfo = "Inspect_Node:读Models文件异常:ErrorCode = " + paraErrCode.I + " ErrorInfo:" + paraErrStr.S;
                    errorInfo = _initErrorInfo;
                    return false;
                }

                Con_FrameInspectParas = ((new HTupleVector(3).Insert(0, FrameModels)).Insert(
                    1, FrameParameters));
                Con_IcInspectParas = ((new HTupleVector(3).Insert(0, IcModels)).Insert(
                    1, IcParameters));
                Con_EpoxyInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, EpoxyModels)))).Insert(
                    1, EpoxyParameters));
                //modify by wj 2021-01-18
                Con_BondInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, BondModels)))).Insert(
                        1, BondParameters));
                Con_WireInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, WireModels)))).Insert(
                    1, WireParameters));

                IsLoadModel = true;
                //MessageBox.Show("加载参数完成！");
                _isInitOK = true;
                errorInfo = "Success";
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                _initErrorInfo = "Inspect_Node InitInspectParam  异常:" + ex.Message;
                errorInfo = ex.Message;
                return false;
            }
        }


        static int[] _dieErrorTypes = new int[] { 1,  2,  3,  4,  5,  6,  7,  8,  9,  10,
                                                  11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                                                  21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31};
        public static int[] DieErrorTypes() { return _dieErrorTypes; }
        public static string DieErrorDescript(int errType)
        {
            switch (errType)
            {
                case 1: return "芯片脱落";
                case 2: return "芯片偏移";
                case 3: return "芯片转角";
                case 4: return "芯片反向";
                case 5: return "错误芯片";
                case 6: return "墨点芯片";
                case 7: return "芯片外来物";
                case 8: return "芯片崩角";
                case 9: return "银胶异常";
                case 10: return "金球大小异常";
                case 11: return "金（铜）球偏移";
                case 12:
                    return "断线";
                case 13:
                    return "弯曲";
                case 14:
                    return "第二焊点脱落";
                case 15:
                    return "第二焊点偏移";
                case 16:
                    return "##16##";
                case 17:
                    return "钉架异物";
                case 18:
                    return "框架错误";
                case 19:
                    return "##19##";
                case 20:
                    return "##20##";
                case 21:
                    return "##21##";
                case 22:
                    return "第一焊点脱落";
                case 23:
                    return "第一焊点高尔夫球";
                case 24:
                    return "尾丝长";
                case 25:
                    return "双丝";
                case 26:
                    return "##26##";
                case 27:
                    return "框架变形";
                case 28:
                    return "##28##";
                case 29:
                    return "桥接缺陷";
                case 30:
                    return "框架异物";
                case 31:
                    return "焊点周围异物";
                default: return "ErrorCode:" + errType + "Undefine";
            }
        }

        /// <summary>
        /// DieBond图象检测
        /// </summary>
        /// <param name="imgs">多通道图象</param>
        /// <param name="taskNames">图象通道名</param>
        /// <param name="dieRegions">需要检测的区域</param>
        /// <param name="detectErrorInfo">检测算法失败时的错误信息</param>
        /// <param name="diesErrorCodes">错误码，一维长度为Die的数量，每颗Die包含多个（1~N）错误码，如果错误码数量为1，码值为0表示无错误</param>
        /// <param name="diesErrorTaskNames">错误码对应的图象通道名称</param>
        /// <param name="diesFailRegions">错误码发生的区域</param>
        /// <param name="diesErrorDetails">每个错误码的详细描述信息错误码</param>
        /// <param name="detectItems">检测项（多个，可扩展）</param>
        /// <returns></returns>
        public bool InspectImage(HObject[] imgs,                              // 输入的Task图像
                                 string[] taskNames,                          // 输入的Task图层名称
                                 HObject dieRegions,                          // 需要检测的Die的区域(Concact后的一维数组)
                                 out string detectErrorInfo,                  // 检测流程出错时的信息
                                 out List<int[]> diesErrorCodes,              // Fov中每颗die的错误码 , 每颗die可能有多个错误码
                                 out List<string[]> diesErrorTaskNames,       // 每颗Die中的errorCode对应的Task图层
                                 out List<HObject[]> diesFailRegions,         // 错误区域，List的数量为Fov中Die的数量 ，每个die中的错误区域数量等以检测出的ErrorCode的数量
                                 out List<string[]> diesErrorDetails,         // 各个错误的详细描述信息
                                 out Dictionary<string, HObject> detectItems, // 所有输出的检测项(OK项)区域
                                 out Dictionary<string, string> detectItemTaskNames // 所有OK项对应的图层名称
                                 )
        {
            if (!_isInitOK)
            {
                detectErrorInfo = "Inspect_Node 未完成初始化,ErrorInfo:" + _initErrorInfo;
                diesErrorCodes = null;
                diesErrorTaskNames = null;
                detectItems = null;
                diesFailRegions = null;
                diesErrorDetails = null;
                detectItemTaskNames = null;
                return false;
            }
            try
            {
                HTuple ErrCode, ErrStr;
                ImageVerifyConcat = imgs[0].Clone();

                for (int i = 1; i < imgs.Length; i++)
                    HOperatorSet.ConcatObj(ImageVerifyConcat, imgs[i], out ImageVerifyConcat);

                HObject BondContours;
                HObject Wires;
                HObjectVector FailRegs = new HObjectVector(4); //算子输出的Regions

                HTupleVector hvDetectErrorType = new HTupleVector(4);
                HTupleVector hvDetectDefectImgIdx = new HTupleVector(4);
                HTupleVector hvDefectValueFrames = new HTupleVector(5);
                HTupleVector hvDefectValueIcs = new HTupleVector(5);
                HTupleVector hvDefectValueEpoxys = new HTupleVector(5);
                HTupleVector hvDefectValueBonds = new HTupleVector(5);
                HTupleVector hvDefectValueWires = new HTupleVector(5);
                HTupleVector hvec_RefValue = new HTupleVector(5);

                Algorithm.Model_RegionAlg.JSCC_AOI_Inspect(ImageVerifyConcat,
                                                           dieRegions,
                                                           FrameObjs,
                                                           IcObjs,
                                                           EpoxyObjs,
                                                           BondObjs,
                                                           WireObjs,
                                                           out BondContours,
                                                           out Wires,
                                                           out FailRegs,
                                                           InspectItemNum,
                                                           Con_FrameInspectParas,
                                                           Con_IcInspectParas,
                                                           Con_EpoxyInspectParas,
                                                           Con_BondInspectParas,
                                                           Con_WireInspectParas,
                                                           CutRegModels,
                                                           Con_AroundBallInspectParas,
                                                           out hvDefectValueFrames,     // 框架缺陷信息
                                                           out hvDefectValueIcs,        // 芯片缺陷信息
                                                           out hvDefectValueEpoxys,     // 银胶缺陷信息
                                                           out hvDefectValueBonds,      // 焊球缺陷信息
                                                           out hvDefectValueWires,      // 金线缺陷信息
                                                           out hvDetectDefectImgIdx,    // 检测缺陷所检图像索引
                                                           out hvDetectErrorType,       // 检测结果错误码
                                                           out hvec_RefValue,           // 参数设置参考值
                                                           out ErrCode,                 // 1维数组, 数组长度为Die数量, 0表示检测结果OK 1表示检测NG
                                                           out ErrStr);

                Algorithm.Model_RegionAlg.HTV_Vector_to_ObjectConcat_4d(FailRegs, out HObject FailRegsConcat, out HTuple VerErrCode, out HTuple VerErrStr);

                // 结果输出
                long[] la = ErrCode.LArr;
                int failedRegIndex = 0; //用于统计1维变量FiledRegs的
                diesErrorCodes = new List<int[]>();
                diesErrorTaskNames = new List<string[]>();
                diesFailRegions = new List<HObject[]>();
                diesErrorDetails = new List<string[]>();
                for (int i = 0; i < la.Length; i++)
                {
                    if (la[i] == 0) //当前Die检测成功
                    {
                        diesErrorCodes.Add(new int[] { 0 });
                        diesErrorTaskNames.Add(new string[] { });
                        diesFailRegions.Add(new HObject[] { });
                        diesErrorDetails.Add(new string[] { "Success" });
                    }
                    else
                    {

                        long[] laErrorCode = hvDetectErrorType.At(i).ConvertVectorToTuple().LArr;
                        long[] laErrorTask = hvDetectDefectImgIdx.At(i).ConvertVectorToTuple().LArr;
                        int[] dieErrorCodes = new int[laErrorCode.Length]; //每颗Die
                        string[] dieErrorTasks = new string[laErrorCode.Length];
                        HObject[] dieFailedRegions = new HObject[laErrorCode.Length];
                        string[] dieErrorDetails = new string[laErrorCode.Length];
                        for (int j = 0; j < laErrorCode.Length; j++)
                        {
                            dieErrorCodes[j] = Convert.ToInt32(laErrorCode[j]);
                            dieErrorTasks[j] = taskNames[Convert.ToInt32(laErrorTask[j]) - 1];
                            dieFailedRegions[j] = FailRegsConcat.SelectObj(failedRegIndex + 1);
                            dieErrorDetails[j] = "Unknown";
                            failedRegIndex++;
                        }
                        diesErrorCodes.Add(dieErrorCodes);
                        diesErrorTaskNames.Add(dieErrorTasks);
                        diesFailRegions.Add(dieFailedRegions);
                        diesErrorDetails.Add(dieErrorDetails);

                    }
                }
                detectErrorInfo = "Success";

                detectItems = new Dictionary<string, HObject>();
                detectItems.Add("BondContours", BondContours);
                detectItems.Add("WireRegions", Wires);

                detectItemTaskNames = new Dictionary<string, string>();
                detectItemTaskNames.Add("BondContours", taskNames[0]); //当前先将图层置为第一个，日后等视觉添加算法支持
                detectItemTaskNames.Add("WireRegions", taskNames[0]);



                //在此添加其他可能的输出项


                return true;
            }
            catch (Exception ex)
            {
                detectErrorInfo = "Inspect_Node.InspectImage() 发生异常,ErrorInfo:" + ex.Message;
                diesErrorCodes = null;
                diesErrorTaskNames = null;
                detectItems = null;
                detectItemTaskNames = null;
                diesFailRegions = null;
                diesErrorDetails = null;
                return false;
            }
        }

        public bool InspectImage(HObject[] imgs,                              // 输入的Task图像 
                                HObject dieRegions,                          // 需要检测的Die的区域(Concact后的一维数组)
                                out string detectErrorInfo,                  // 检测流程出错时的信息
                                out List<InspectResultItem[]> dieDefectResults   //die的检测结果(List中的每一个item表示一颗Die,也可能是Die的一部分)
                                )
        {


            if (!_isInitOK)
            {
                dieDefectResults = null;
                detectErrorInfo = "Inspect_Node 未完成初始化,ErrorInfo:" + _initErrorInfo;
                return false;
            }
            dieDefectResults = new List<InspectResultItem[]>();



            try
            {
                HTuple ErrCode, ErrStr;
                ImageVerifyConcat = imgs[0].Clone();

                for (int i = 1; i < imgs.Length; i++)
                    HOperatorSet.ConcatObj(ImageVerifyConcat, imgs[i], out ImageVerifyConcat);

                HObject BondContours;
                HObject Wires;
                HObjectVector FailRegs = new HObjectVector(4); //算子输出的Regions

                HTupleVector hvDetectErrorType = new HTupleVector(4);
                HTupleVector hvDetectDefectImgIdx = new HTupleVector(4);
                HTupleVector hvDefectValueFrames = new HTupleVector(5);
                HTupleVector hvDefectValueIcs = new HTupleVector(5);
                HTupleVector hvDefectValueEpoxys = new HTupleVector(5);
                HTupleVector hvDefectValueBonds = new HTupleVector(5);
                HTupleVector hvDefectValueWires = new HTupleVector(5);
                HTupleVector hvec_RefValue = new HTupleVector(5);

                Algorithm.Model_RegionAlg.JSCC_AOI_Inspect(ImageVerifyConcat,
                                                           dieRegions,
                                                           FrameObjs,
                                                           IcObjs,
                                                           EpoxyObjs,
                                                           BondObjs,
                                                           WireObjs,
                                                           out BondContours,
                                                           out Wires,
                                                           out FailRegs,
                                                           InspectItemNum,
                                                           Con_FrameInspectParas,
                                                           Con_IcInspectParas,
                                                           Con_EpoxyInspectParas,
                                                           Con_BondInspectParas,
                                                           Con_WireInspectParas,
                                                           CutRegModels,
                                                           Con_AroundBallInspectParas,
                                                           out hvDefectValueFrames,     // 框架缺陷信息
                                                           out hvDefectValueIcs,        // 芯片缺陷信息
                                                           out hvDefectValueEpoxys,     // 银胶缺陷信息
                                                           out hvDefectValueBonds,      // 焊球缺陷信息
                                                           out hvDefectValueWires,      // 金线缺陷信息
                                                           out hvDetectDefectImgIdx,    // 检测缺陷所检图像索引
                                                           out hvDetectErrorType,       // 检测结果错误码
                                                           out hvec_RefValue,           // 参数设置参考值
                                                           out ErrCode,                 // 1维数组, 数组长度为Die数量, 0表示检测结果OK 1表示检测NG
                                                           out ErrStr);

                Algorithm.Model_RegionAlg.HTV_Vector_to_ObjectConcat_4d(FailRegs, out HObject FailRegsConcat, out HTuple VerErrCode, out HTuple VerErrStr);

                // 结果输出
                long[] la = ErrCode.LArr;
                for (int i = 0; i < la.Length; i++)
                {
                    List<InspectResultItem> dieResult = new List<InspectResultItem>();
                    HTupleVector hvDieDefectType = hvDetectErrorType.At(i);//一整颗Die的所有检测结果
                    HTupleVector hvFramesDt = hvDieDefectType.At(0); //Frames DetectType
                    HTupleVector hvICsDt = hvDieDefectType.At(1);//IC's
                    HTupleVector hvEPoxysDt = hvDieDefectType.At(2); //银胶
                    HTupleVector hvBondsDt = hvDieDefectType.At(3); //焊点
                    HTupleVector hvWiresDt = hvDieDefectType.At(4); //金线
                    

                    //图层序号
                    HTupleVector hvImgeIndex = hvDetectDefectImgIdx.At(i);
                    HTupleVector hvFramesImageID = hvImgeIndex.At(0); //Frames DetectType
                    HTupleVector hvICsImageID = hvImgeIndex.At(1);//IC's
                    HTupleVector hvEPoxysImageID = hvImgeIndex.At(2); //银胶
                    HTupleVector hvBoundsImageID = hvImgeIndex.At(3); //焊点
                    HTupleVector hvWiresImageID = hvImgeIndex.At(4); //金线


                    //各检测项的检测标准
                    HTupleVector hvDetectRefVal = hvec_RefValue.At(i);
                    HTupleVector hvFramesRV = hvec_RefValue.At(0); //Frames DetectType
                    HTupleVector hvICsRV = hvec_RefValue.At(1);//IC's
                    HTupleVector hvEPoxysRV = hvec_RefValue.At(2); //银胶
                    HTupleVector hvBondsRV = hvec_RefValue.At(3); //焊点
                    HTupleVector hvWiresRV = hvec_RefValue.At(4); //金线



                    //检测结果区域
                    HObjectVector ovFramesRetRegs = FailRegs.At(i).At(0);
                    HObjectVector ovICsRetRegs = FailRegs.At(i).At(1);
                    HObjectVector ovEpoxysRetRegs = FailRegs.At(i).At(2);
                    HObjectVector ovBondsRetRegs = FailRegs.At(i).At(3);
                    HObjectVector ovWiresRetRegs = FailRegs.At(i).At(4);
                    

                    //框架检测信息
                    int frameCount = hvFramesDt.At(0).Length;
                    for (int j = 0; j < frameCount; j++)
                    {
                        InspectResultItem frameResultItem = new InspectResultItem();
                        frameResultItem.DieIndex = i;
                        frameResultItem.InspectCategoty = InspectResultItem.Categoty.Frame;
                        frameResultItem.InspectID = "Frame_" + (j + 1);
                        frameResultItem.ImageIndex = hvFramesImageID.At(0).At(j).ConvertVectorToTuple().I-1;
                        //HTuple htp = hvFramesDt.At(0).At(j).ConvertVectorToTuple();
                        long[] laErrs = hvFramesDt.At(0).At(j).ConvertVectorToTuple().LArr;
                        frameResultItem.ErrorCodes = new int[laErrs.Length];
                        for (int k = 0; k < laErrs.Length; k++)
                            frameResultItem.ErrorCodes[k] = (int)laErrs[k];

                        frameResultItem.QualifiedDescript = hvFramesRV.ToString();  //先将数据输出，日后添加文字说明
                        frameResultItem.ResultDescript = hvDefectValueFrames.At(i).At(j).ToString();//先将数据输出，日后添加文字说明

                        frameResultItem.DetectRegion = null; //理论检测区,日后添加
                        frameResultItem.ResultRegion = ovFramesRetRegs.At(0).At(j).O; //实际检出的区域

                        dieResult.Add(frameResultItem);
                    }

                    //IC检测信息
                    int icCount = hvICsDt.Length;//已确认,IC的数量为向量第一层的长度
                    if(1 == icCount)//再次确认是否为空向量
                    {
                        if (0 == hvICsDt.At(0).Length)
                            icCount = 0;
                    }
                    for (int j = 0; j < icCount; j++)
                    {
                        InspectResultItem icResultItem = new InspectResultItem();
                        icResultItem.DieIndex = i;
                        icResultItem.InspectCategoty = InspectResultItem.Categoty.IC;
                        icResultItem.InspectID = "IC_" + (j + 1);
                        icResultItem.ImageIndex = hvICsImageID.At(j).At(0).ConvertVectorToTuple().I-1;
                        long[] laErrs = hvICsDt.At(j).At(0).ConvertVectorToTuple().LArr;
                        icResultItem.ErrorCodes = new int[laErrs.Length];
                        for (int k = 0; k < laErrs.Length; k++)
                            icResultItem.ErrorCodes[k] = (int)laErrs[k];

                        icResultItem.QualifiedDescript = hvICsRV.At(j).ToString();  //先将数据输出，日后添加文字说明
                        icResultItem.ResultDescript = hvDefectValueIcs.At(i).At(j).ToString();

                        ////frameResultItem.DetectRegion = null; //理论检测区
                        icResultItem.ResultRegion = ovICsRetRegs.At(j).At(0).O; //实际检出的区域

                        dieResult.Add(icResultItem);
                    }

                    //焊点监测信息
                    int bondIndex = 0;
                    int bondGroupsCount = hvBondsDt.Length; //焊点是分组的, 每组分别有若干个焊点（项）
                    if(1 == bondGroupsCount)
                    {
                        if (0 == hvBondsDt.At(0).Length)
                            bondGroupsCount = 0;
                    }
                    for (int j = 0; j < bondGroupsCount; j++)
                    {
                        HTupleVector hvBondsGrp = hvBondsDt.At(j);
                        int bondCountInGrp = hvBondsGrp.Length;
                        for (int k = 0; k < bondCountInGrp; k++)
                        {
                            InspectResultItem bondResultItem = new InspectResultItem();
                            bondResultItem.DieIndex = i;
                            bondResultItem.InspectCategoty = InspectResultItem.Categoty.Bond;
                            bondResultItem.InspectID = "Bond_" + (++bondIndex);
                            bondResultItem.ImageIndex = hvICsImageID.At(j).ConvertVectorToTuple().I-1;
                            long[] laErrs = hvBondsDt.At(j).At(k).ConvertVectorToTuple().LArr;
                            bondResultItem.ErrorCodes = new int[laErrs.Length];
                            for (int h = 0; h < laErrs.Length; h++)
                                bondResultItem.ErrorCodes[h] = (int)laErrs[h];

                            bondResultItem.QualifiedDescript = hvBondsRV.At(j).At(k).ToString();  //先将数据输出，日后添加文字说明
                            bondResultItem.ResultDescript = hvDefectValueBonds.At(i).At(j).At(k).ToString();

                            //bondResultItem.DetectRegion = null; //理论检测区
                            bondResultItem.ResultRegion = ovBondsRetRegs.At(j).At(k).O; //实际检出的区域

                            dieResult.Add(bondResultItem);
                        }
                    }

                    //焊线检测
                    int wiresCount = hvWiresDt.At(0).Length; //焊线的组数
                    for(int j = 0;j< wiresCount; j++)
                    {
    
                        InspectResultItem wireResultItem = new InspectResultItem();
                        wireResultItem.DieIndex = i;
                        wireResultItem.InspectCategoty = InspectResultItem.Categoty.Wire;
                        wireResultItem.InspectID = "Wire_" + (j+1);
                        wireResultItem.ImageIndex = hvWiresImageID.At(0).At(j).ConvertVectorToTuple().I - 1;
                            //long[] laErrs = hvWiresDt.At(0).At(j).ConvertVectorToTuple().LArr;
                        wireResultItem.ErrorCodes = hvWiresDt.At(0).At(j).ConvertVectorToTuple().IArr;//new int[laErrs.Length];
                            //for (int h = 0; h < laErrs.Length; h++)
                            //    wireResultItem.ErrorCodes[h] = (int)laErrs[h];

                        wireResultItem.QualifiedDescript = hvWiresRV.At(0).At(j).ToString();  //先将数据输出，日后添加文字说明
                        wireResultItem.ResultDescript = hvDefectValueWires.At(i).At(0).At(j).ToString();

                        //wireResultItem.DetectRegion = null; //理论检测区
                        wireResultItem.ResultRegion = ovWiresRetRegs.At(0).At(j).O; ; //实际检出的区域

                        dieResult.Add(wireResultItem);
                        

                    }


                    //银胶检测
                    int eproxyGrps = hvEPoxysDt.Length;
                    if(1 == eproxyGrps)
                    {
                        if (hvEPoxysDt.At(0).Length == 0)
                            eproxyGrps = 0;
                    }
                    int eproxyIndex = 0;
                    for (int j = 0; j < eproxyGrps; j++)
                    {
                        HTupleVector hvEpoxyGrp = hvEPoxysDt.At(j);
                        for (int k = 0; k < hvEpoxyGrp.Length; k++)
                        {
                            InspectResultItem epoxyResultItem = new InspectResultItem();
                            epoxyResultItem.DieIndex = i;
                            epoxyResultItem.InspectCategoty = InspectResultItem.Categoty.Epoxy;
                            epoxyResultItem.InspectID = "Epoxy_" + (++eproxyIndex);
                            epoxyResultItem.ImageIndex = hvEPoxysImageID.At(j).At(k).ConvertVectorToTuple().I - 1;
                            epoxyResultItem.ErrorCodes = hvEPoxysDt.At(j).At(k).ConvertVectorToTuple().IArr;//new int[laErrs.Length];
                                                                                                            //for (int h = 0; h < laErrs.Length; h++)
                                                                                                            //    wireResultItem.ErrorCodes[h] = (int)laErrs[h];

                            epoxyResultItem.QualifiedDescript = hvEPoxysRV.At(j).At(k).ToString();  //先将数据输出，日后添加文字说明
                            epoxyResultItem.ResultDescript = hvDefectValueEpoxys.At(i).At(j).At(k).ToString();

                            //epoxyResultItem.DetectRegion = null; //理论检测区
                            epoxyResultItem.ResultRegion = ovEpoxysRetRegs.At(j).At(k).O; //实际检出的区域

                            dieResult.Add(epoxyResultItem);
                        }
                    }




                    dieDefectResults.Add(dieResult.ToArray());

                }
                detectErrorInfo = "Success";




                return true;
            }
            catch (Exception ex)
            {
                detectErrorInfo = "Inspect_Node.InspectImage() 发生异常,ErrorInfo:" + ex.Message;
                return false;
            }
        }


        public bool ClearAllModels(out string errorInfo)
        {
            try
            {
                if (_isInitOK)
                {
                    //清除模板 1130
                    Algorithm.Model_RegionAlg.JSCC_AOI_clear_all_model(InspectItemNum, FrameModels, IcModels,
                                                                       BondModels, out HTuple _clearErrcode, out HTuple _clearErrStr);

                    if ((int)(new HTuple(_clearErrcode.TupleLess(0))) != 0)
                    {
                        errorInfo = _clearErrStr.S;
                        return false;
                    }
                    _isInitOK = false;
                    errorInfo = "ClearAllModels Success";
                    return true;
                }
                else
                {
                    errorInfo = "Success";
                    return true;
                    //errorInfo = "Inspect_Node 未完成初始化,ErrorInfo:" + _initErrorInfo;
                    //return false;
                }

            }
            catch (Exception ex)
            {
                errorInfo = "Inspect_Node.ClearAllModels() 发生异常,ErrorInfo:" + ex.Message;
                return false;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //单节点检测验证
        public HObject FailRegResult = null;
        public HObject BondWireRegResult = null;

        private void ExecuteVerifyCommand(object parameter)
        {
            if (IsLoadModel != true)
            {
                MessageBox.Show("请先加载模板参数！");
                return;
            }
            if (ImageVerify == null || !ImageVerify.IsInitialized())
            {
                MessageBox.Show("请先加检测图集)！");
                return;
            }

            try
            {
                HOperatorSet.GenEmptyObj(out DieRegions);
                HOperatorSet.ReadRegion(out DieRegions, $"{RecipeFile}Reference\\CoarseReference.reg");

                HTupleVector hvDetectErrorType = new HTupleVector(4);
                HTupleVector hvDetectDefectImgIdx = new HTupleVector(4);
                HTupleVector hvDefectValueFrames = new HTupleVector(5);
                HTupleVector hvDefectValueIcs = new HTupleVector(5);
                HTupleVector hvDefectValueEpoxys = new HTupleVector(5);
                HTupleVector hvDefectValueBonds = new HTupleVector(5);
                HTupleVector hvDefectValueWires = new HTupleVector(5);
                HTupleVector hvec_RefValue = new HTupleVector(5);

                Algorithm.Model_RegionAlg.JSCC_AOI_Inspect(Algorithm.Region.GetChannnelImageConcact(ImageVerify),
                                        DieRegions,
                                        FrameObjs,
                                        IcObjs,
                                        EpoxyObjs,
                                        BondObjs,
                                        WireObjs,
                                        out HObject BondContours,
                                        out HObject Wires,
                                        out HObjectVector ResultRegs,
                                        InspectItemNum,
                                        Con_FrameInspectParas,
                                        Con_IcInspectParas,
                                        Con_EpoxyInspectParas,
                                        Con_BondInspectParas,
                                        Con_WireInspectParas,
                                        CutRegModels,
                                        Con_AroundBallInspectParas,
                                        out hvDefectValueFrames,     // 框架缺陷信息
                                        out hvDefectValueIcs,        // 芯片缺陷信息
                                        out hvDefectValueEpoxys,     // 银胶缺陷信息
                                        out hvDefectValueBonds,      // 焊球缺陷信息
                                        out hvDefectValueWires,      // 金线缺陷信息
                                        out hvDetectDefectImgIdx,    // 检测缺陷所检图像索引
                                        out hvDetectErrorType,       // 检测结果错误码
                                        out hvec_RefValue,           // 参数设置参考值
                                        out HTuple ErrCode,
                                        out HTuple ErrStr);

                //单节点定位不到提示
                HOperatorSet.GetImageSize(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, IniParameters.ImageIndex), out imageWidth, out imageHeight);

                HOperatorSet.TupleSum(ErrCode, out HTuple ErrCodeSum);
                if (ErrCodeSum > 0)
                {
                    HTuple hv__ErrorTypes = null;
                    HObject ho__FailRegs = null, ho__NormalRegs = null;
                    
                    Algorithm.Model_RegionAlg.HTV_Analysis_InspectResult(ResultRegs, out ho__FailRegs, out ho__NormalRegs, hvDetectErrorType,
                                                                     out hv__ErrorTypes, out HTuple ResultErrCode, out HTuple ResultErrStr);
                    
                    // 分颜色显示 0118 lw
                    HOperatorSet.GenEmptyObj(out FailRegResult);
                    HOperatorSet.GenEmptyObj(out BondWireRegResult);
                    HOperatorSet.ConcatObj(FailRegResult, ho__FailRegs, out FailRegResult);
                    HOperatorSet.ConcatObj(BondWireRegResult, Wires.ConcatObj(BondContours), out BondWireRegResult);

                    DisplayNodeResultRegion();
                    HalconDisp.DisplayMessage(htWindow.HTWindow.HalconWindow, string.Format("{0}", "该视野检测NG"), "image", imageWidth * 1 / 16, imageHeight * 1 / 16, "orange red", "true");

                    ho__FailRegs.Dispose();
                    ho__NormalRegs.Dispose();
                }
                else
                {
                    FailRegResult = null;
                    BondWireRegResult = null;
                    htWindow.DisplaySingleRegion(Wires.ConcatObj(BondContours), Algorithm.Region.GetChannnelImageUpdate(ImageVerify, IniParameters.ImageIndex), "green");
                    HalconDisp.DisplayMessage(htWindow.HTWindow.HalconWindow, string.Format("{0}", "该视野检测OK"), "image", imageWidth * 1 / 16, imageHeight*1 / 16, "green", "true");
                }

                /*
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //结果解析
                //【1】. o_DefectTypes/o_DefectImgIdxs
                //4维向量：1- die循环  2- 固定检测任务：Frame, Ic, Epoxy, Bond, Wire  3- 检测项组  4-检测项ID
                //> 0：错误项； =0：正常项; 为空：没有这个检测项

                //例：解析第i个die
                HTuple hv_idx = new HTuple(), hv_i = new HTuple(), hv_j = new HTuple();
                HTupleVector hvec_DieDefectType = new HTupleVector(3);
                HTupleVector hvec_FramesDefectType = new HTupleVector(2), hvec_IcsDefectType = new HTupleVector(2);
                HTupleVector hvec_EpoxysDefectType = new HTupleVector(2), hvec_BondsDefectType = new HTupleVector(2);
                HTupleVector hvec_WiresDefectType = new HTupleVector(2), hvec_frameDefectType = new HTupleVector(1);
                HTupleVector hvec_icDefectType = new HTupleVector(1), hvec_epoxyDefectType = new HTupleVector(1);
                HTupleVector hvec_bondDefectType = new HTupleVector(1), hvec_wireDefectType = new HTupleVector(1);
                HTuple hv_FrameIdType = new HTuple();
                HTuple hv_IcIdType = new HTuple(), hv_EpoxyIdType = new HTuple();
                HTuple hv_BondIdType = new HTuple(), hv_WireIdType = new HTuple();

                HTuple end_val215 = new HTuple(hvDetectErrorType.Length) - 1;
                HTuple step_val215 = 1;
                for (hv_idx = 0; hv_idx.Continue(end_val215, step_val215); hv_idx = hv_idx.TupleAdd(step_val215))
                {
                    //1d
                    hvec_DieDefectType = hvDetectErrorType[hv_idx];
                    //2d: 固定5个检测任务
                    hvec_FramesDefectType = hvec_DieDefectType[0];
                    hvec_IcsDefectType = hvec_DieDefectType[1];
                    hvec_EpoxysDefectType = hvec_DieDefectType[2];
                    hvec_BondsDefectType = hvec_DieDefectType[3];
                    hvec_WiresDefectType = hvec_DieDefectType[4];
                    //Frame
                    HTuple end_val225 = new HTuple(hvec_FramesDefectType.Length) - 1;
                    HTuple step_val225 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val225, step_val225); hv_i = hv_i.TupleAdd(step_val225))
                    {
                        //3d
                        hvec_frameDefectType = hvec_FramesDefectType[hv_i];
                        HTuple end_val228 = new HTuple(hvec_frameDefectType.Length) - 1;
                        HTuple step_val228 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val228, step_val228); hv_j = hv_j.TupleAdd(step_val228))
                        {
                            //4d : Frame 一组只有一个ID
                            hv_FrameIdType = hvec_frameDefectType[hv_j].T.Clone();
                        }
                    }
                    //IC
                    HTuple end_val234 = new HTuple(hvec_IcsDefectType.Length) - 1;
                    HTuple step_val234 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val234, step_val234); hv_i = hv_i.TupleAdd(step_val234))
                    {
                        //3d
                        hvec_icDefectType = hvec_IcsDefectType[hv_i];
                        HTuple end_val237 = new HTuple(hvec_icDefectType.Length) - 1;
                        HTuple step_val237 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val237, step_val237); hv_j = hv_j.TupleAdd(step_val237))
                        {
                            //4d : IC 一组只有一个ID
                            hv_IcIdType = hvec_icDefectType[hv_j].T.Clone();
                        }
                    }
                    //Epoxy
                    HTuple end_val243 = new HTuple(hvec_EpoxysDefectType.Length) - 1;
                    HTuple step_val243 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val243, step_val243); hv_i = hv_i.TupleAdd(step_val243))
                    {
                        //3d
                        hvec_epoxyDefectType = hvec_EpoxysDefectType[hv_i];
                        HTuple end_val246 = new HTuple(hvec_epoxyDefectType.Length) - 1;
                        HTuple step_val246 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val246, step_val246); hv_j = hv_j.TupleAdd(step_val246))
                        {
                            //4d : Epoxy 一个区域对应一个ID
                            hv_EpoxyIdType = hvec_epoxyDefectType[hv_j].T.Clone();
                        }
                    }
                    //Bond
                    HTuple end_val252 = new HTuple(hvec_BondsDefectType.Length) - 1;
                    HTuple step_val252 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val252, step_val252); hv_i = hv_i.TupleAdd(step_val252))
                    {
                        //3d
                        hvec_bondDefectType = hvec_BondsDefectType[hv_i];
                        HTuple end_val255 = new HTuple(hvec_bondDefectType.Length) - 1;
                        HTuple step_val255 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val255, step_val255); hv_j = hv_j.TupleAdd(step_val255))
                        {
                            //4d : Bond 一个焊点对应一个ID
                            hv_BondIdType = hvec_bondDefectType[hv_j].T.Clone();
                        }
                    }
                    //wire
                    HTuple end_val261 = new HTuple(hvec_WiresDefectType.Length) - 1;
                    HTuple step_val261 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val261, step_val261); hv_i = hv_i.TupleAdd(step_val261))
                    {
                        //3d
                        hvec_wireDefectType = hvec_WiresDefectType[hv_i];
                        HTuple end_val264 = new HTuple(hvec_wireDefectType.Length) - 1;
                        HTuple step_val264 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val264, step_val264); hv_j = hv_j.TupleAdd(step_val264))
                        {
                            //4d : Wire 一根金线对应一个ID
                            hv_WireIdType = hvec_wireDefectType[hv_j].T.Clone();
                        }
                    }
                }

                //【2】. o_FailRegs
                //FailRegion：错误项； EmptyRegion：正常项; EmptyObject：没有这个检测项
                //例：解析第i个die
                HObject ho_FrameIdFailReg = null, ho_IcIdFailReg = null, ho_EpoxyIdFailReg = null;
                HObject ho_BondIdFailReg = null, ho_WireIdFailReg = null;
                HObjectVector hvec__FailRegs = new HObjectVector(4), hvec_DieFailRegs = new HObjectVector(3);
                HObjectVector hvec_FramesFailRegs = new HObjectVector(2), hvec_IcsFailRegs = new HObjectVector(2);
                HObjectVector hvec_EpoxysFailRegs = new HObjectVector(2), hvec_BondsFailRegs = new HObjectVector(2);
                HObjectVector hvec_WiresFailRegs = new HObjectVector(2), hvec_frameFailRegs = new HObjectVector(1);
                HObjectVector hvec_icFailRegs = new HObjectVector(1), hvec_epoxyFailRegs = new HObjectVector(1);
                HObjectVector hvec_bondFailRegs = new HObjectVector(1), hvec_wireFailRegs = new HObjectVector(1);
                HTuple end_val275 = new HTuple(ResultRegs.Length) - 1;
                HTuple step_val275 = 1;
                // Initialize local and output iconic variables 
                HOperatorSet.GenEmptyObj(out ho_FrameIdFailReg);
                HOperatorSet.GenEmptyObj(out ho_IcIdFailReg);
                HOperatorSet.GenEmptyObj(out ho_EpoxyIdFailReg);
                HOperatorSet.GenEmptyObj(out ho_BondIdFailReg);
                HOperatorSet.GenEmptyObj(out ho_WireIdFailReg);
                for (hv_idx = 0; hv_idx.Continue(end_val275, step_val275); hv_idx = hv_idx.TupleAdd(step_val275))
                {
                    //1d
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hvec_DieFailRegs = dh.Take(ResultRegs[hv_idx]);
                    }
                    //2d: 固定5个检测任务
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hvec_FramesFailRegs = dh.Take(hvec_DieFailRegs[0]);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hvec_IcsFailRegs = dh.Take(hvec_DieFailRegs[1]);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hvec_EpoxysFailRegs = dh.Take(hvec_DieFailRegs[2]);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hvec_BondsFailRegs = dh.Take(hvec_DieFailRegs[3]);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hvec_WiresFailRegs = dh.Take(hvec_DieFailRegs[4]);
                    }
                    //Frame
                    HTuple end_val285 = new HTuple(hvec_FramesFailRegs.Length) - 1;
                    HTuple step_val285 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val285, step_val285); hv_i = hv_i.TupleAdd(step_val285))
                    {
                        //3d
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hvec_frameFailRegs = dh.Take(hvec_FramesFailRegs[hv_i]);
                        }
                        HTuple end_val288 = new HTuple(hvec_frameFailRegs.Length) - 1;
                        HTuple step_val288 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val288, step_val288); hv_j = hv_j.TupleAdd(step_val288))
                        {
                            //4d : Frame 一组只有一个ID
                            ho_FrameIdFailReg.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_FrameIdFailReg = hvec_frameFailRegs[hv_j].O.CopyObj(1, -1);
                            }
                        }
                    }
                    //IC
                    HTuple end_val294 = new HTuple(hvec_IcsFailRegs.Length) - 1;
                    HTuple step_val294 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val294, step_val294); hv_i = hv_i.TupleAdd(step_val294))
                    {
                        //3d
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hvec_icFailRegs = dh.Take(hvec_IcsFailRegs[hv_i]);
                        }
                        HTuple end_val297 = new HTuple(hvec_icFailRegs.Length) - 1;
                        HTuple step_val297 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val297, step_val297); hv_j = hv_j.TupleAdd(step_val297))
                        {
                            //4d : IC 一组只有一个ID
                            ho_IcIdFailReg.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_IcIdFailReg = hvec_icFailRegs[hv_j].O.CopyObj(1, -1);
                            }
                        }
                    }
                    //Epoxy
                    HTuple end_val303 = new HTuple(hvec_EpoxysFailRegs.Length) - 1;
                    HTuple step_val303 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val303, step_val303); hv_i = hv_i.TupleAdd(step_val303))
                    {
                        //3d
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hvec_epoxyFailRegs = dh.Take(hvec_EpoxysFailRegs[hv_i]);
                        }
                        HTuple end_val306 = new HTuple(hvec_epoxyFailRegs.Length) - 1;
                        HTuple step_val306 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val306, step_val306); hv_j = hv_j.TupleAdd(step_val306))
                        {
                            //4d : Epoxy 一个区域对应一个ID
                            ho_EpoxyIdFailReg.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_EpoxyIdFailReg = hvec_epoxyFailRegs[hv_j].O.CopyObj(1, -1);
                            }
                        }
                    }
                    //Bond
                    HTuple end_val312 = new HTuple(hvec_BondsFailRegs.Length) - 1;
                    HTuple step_val312 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val312, step_val312); hv_i = hv_i.TupleAdd(step_val312))
                    {
                        //3d
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hvec_bondFailRegs = dh.Take(hvec_BondsFailRegs[hv_i]);
                        }
                        HTuple end_val315 = new HTuple(hvec_bondFailRegs.Length) - 1;
                        HTuple step_val315 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val315, step_val315); hv_j = hv_j.TupleAdd(step_val315))
                        {
                            //4d : Bond 一个焊点对应一个ID
                            ho_BondIdFailReg.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_BondIdFailReg = hvec_bondFailRegs[hv_j].O.CopyObj(1, -1);
                            }
                        }
                    }
                    //wire
                    HTuple end_val321 = new HTuple(hvec_WiresFailRegs.Length) - 1;
                    HTuple step_val321 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val321, step_val321); hv_i = hv_i.TupleAdd(step_val321))
                    {
                        //3d
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hvec_wireFailRegs = dh.Take(hvec_WiresFailRegs[hv_i]);
                        }
                        HTuple end_val324 = new HTuple(hvec_wireFailRegs.Length) - 1;
                        HTuple step_val324 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val324, step_val324); hv_j = hv_j.TupleAdd(step_val324))
                        {
                            //4d : Wire 一根金线对应一个ID
                            ho_WireIdFailReg.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_WireIdFailReg = hvec_wireFailRegs[hv_j].O.CopyObj(1, -1);
                            }
                        }
                    }
                }

                //【2】. o_DefectValue/o_RefValue：
                //包括o_DefectValueFrames,o_DefectValueIcs,o_DefectValueEpoxys,o_DefectValueBonds,o_DefectValueWires
                //5维向量：1- die循环 2- 检测项组 3- 检测项ID 4/5 -defectValue值
                
                //例：解析第0个die Frame
                HTupleVector hvec_DieDefectValueFrames = new HTupleVector(4);
                HTupleVector hvec_FramesDefectValue = new HTupleVector(3), hvec_FrameId1DefectValue = new HTupleVector(2);
                HTupleVector hvec_FrameDefectValue_Socre = new HTupleVector(1);
                HTupleVector hvec_FrameDefectValue_FrameArea = new HTupleVector(1);
                HTupleVector hvec_FrameDefectValue_PegRackArea = new HTupleVector(1);
                HTupleVector hvec_FrameDefectValue_BridgeArea = new HTupleVector(1);
                HTuple hv_FrameModelSocreTup = new HTuple(), hv_FrameAreaTup = new HTuple();
                HTuple hv_FrameRowTup = new HTuple(), hv_FrameColTup = new HTuple();
                HTuple hv_PegRackAreaTup = new HTuple(), hv_PegRackRowTup = new HTuple();
                HTuple hv_PegRackColTup = new HTuple(), hv_BridgeAreaTup = new HTuple();
                HTuple hv_BridgeRowTup = new HTuple(), hv_BridgeColTup = new HTuple();
                //1d
                hvec_DieDefectValueFrames = hvDefectValueFrames[0];
                //2d
                hvec_FramesDefectValue = hvec_DieDefectValueFrames[0];
                //3d
                hvec_FrameId1DefectValue = hvec_FramesDefectValue[0];
                //4d: 定位分数, 框架异物信息, 钉架异物信息, 桥接异物信息
                if ((int)(new HTuple((new HTuple(hvec_FrameId1DefectValue.Length)).TupleLess(
                    2))) != 0)
                {
                    hv_FrameModelSocreTup = new HTuple();
                    hv_FrameAreaTup = new HTuple();
                    hv_FrameRowTup = new HTuple();
                    hv_FrameColTup = new HTuple();
                    hv_PegRackAreaTup = new HTuple();
                    hv_PegRackRowTup = new HTuple();
                    hv_PegRackColTup = new HTuple();
                    hv_BridgeAreaTup = new HTuple();
                    hv_BridgeRowTup = new HTuple();
                    hv_BridgeColTup = new HTuple();
                }
                else
                {
                    hvec_FrameDefectValue_Socre = hvec_FrameId1DefectValue[0];
                    hvec_FrameDefectValue_FrameArea = hvec_FrameId1DefectValue[1];
                    hvec_FrameDefectValue_PegRackArea = hvec_FrameId1DefectValue[2];
                    hvec_FrameDefectValue_BridgeArea = hvec_FrameId1DefectValue[3];
                    //5d: 各模板定位分数，各异物面积及其对应坐标信息
                    hv_FrameModelSocreTup = hvec_FrameDefectValue_Socre[0].T.Clone();
                    hv_FrameAreaTup = hvec_FrameDefectValue_FrameArea[0].T.Clone();
                    hv_FrameRowTup = hvec_FrameDefectValue_FrameArea[1].T.Clone();
                    hv_FrameColTup = hvec_FrameDefectValue_FrameArea[2].T.Clone();
                    hv_PegRackAreaTup = hvec_FrameDefectValue_PegRackArea[0].T.Clone();
                    hv_PegRackRowTup = hvec_FrameDefectValue_PegRackArea[1].T.Clone();
                    hv_PegRackColTup = hvec_FrameDefectValue_PegRackArea[2].T.Clone();
                    hv_BridgeAreaTup = hvec_FrameDefectValue_BridgeArea[0].T.Clone();
                    hv_BridgeRowTup = hvec_FrameDefectValue_BridgeArea[1].T.Clone();
                    hv_BridgeColTup = hvec_FrameDefectValue_BridgeArea[2].T.Clone();
                }

                //例：解析第0个die IC
                HTupleVector hvec_DieDefectValueIcs = new HTupleVector(4), hvec_IcsDefectValue = new HTupleVector(3);
                HTupleVector hvec_IcIdDefectValue = new HTupleVector(2);
                HTupleVector hvec_IcDefectValue_SubRegArea = new HTupleVector(1);
                HTupleVector hvec_IcDefectValue_ChippingRegArea = new HTupleVector(1);
                HTuple hv_IcDefectValue_Socre = new HTuple(), hv_IcDefectValue_DeltaXY = new HTuple();
                HTuple hv_IcDefectValue_DeltaPhi = new HTuple();
                HTuple hv_SubRegAreaTup = new HTuple(), hv_SubRegRowTup = new HTuple(), hv_SubRegColTup = new HTuple();
                HTuple hv_ChippingRegAreaTup = new HTuple(), hv_ChippingRegRowTup = new HTuple(), hv_ChippingRegColTup = new HTuple();
                //1d
                hvec_DieDefectValueIcs = hvDefectValueIcs[0];
                //2d
                HTuple end_val375 = new HTuple(hvec_DieDefectValueIcs.Length) - 1;
                HTuple step_val375 = 1;
                for (hv_idx = 0; hv_idx.Continue(end_val375, step_val375); hv_idx = hv_idx.TupleAdd(step_val375))
                {
                    hvec_IcsDefectValue = hvec_DieDefectValueIcs[hv_idx];
                    //3d
                    HTuple end_val378 = new HTuple(hvec_IcsDefectValue.Length) - 1;
                    HTuple step_val378 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val378, step_val378); hv_i = hv_i.TupleAdd(step_val378))
                    {
                        hvec_IcIdDefectValue = hvec_IcsDefectValue[hv_i];
                        //4d
                        if ((int)(new HTuple((new HTuple(hvec_IcIdDefectValue.Length)).TupleLess(
                            2))) != 0)
                        {
                            //空向量
                            hv_IcDefectValue_Socre = new HTuple();
                            hv_IcDefectValue_DeltaXY = new HTuple();
                            hv_IcDefectValue_DeltaPhi = new HTuple();
                            hv_SubRegAreaTup = new HTuple();
                            hv_SubRegRowTup = new HTuple();
                            hv_SubRegColTup = new HTuple();
                            hv_ChippingRegAreaTup = new HTuple();
                            hv_ChippingRegRowTup = new HTuple();
                            hv_ChippingRegColTup = new HTuple();
                        }
                        else
                        {
                            //定位模板分数
                            hv_IcDefectValue_Socre = hvec_IcIdDefectValue[0][0].T.Clone();
                            //XY偏移
                            hv_IcDefectValue_DeltaXY = hvec_IcIdDefectValue[1][0].T.Clone();
                            //旋转角度
                            hv_IcDefectValue_DeltaPhi = hvec_IcIdDefectValue[2][0].T.Clone();
                            //芯片区异物信息
                            hvec_IcDefectValue_SubRegArea = hvec_IcIdDefectValue[3];
                            //崩边区异物信息
                            hvec_IcDefectValue_ChippingRegArea = hvec_IcIdDefectValue[4];
                            //5d
                            //各异物信息: > 0 为有异物，Tup长度对应异物个数；=0 为无异物
                            hv_SubRegAreaTup = hvec_IcDefectValue_SubRegArea[0].T.Clone();
                            hv_SubRegRowTup = hvec_IcDefectValue_SubRegArea[1].T.Clone();
                            hv_SubRegColTup = hvec_IcDefectValue_SubRegArea[2].T.Clone();
                            hv_ChippingRegAreaTup = hvec_IcDefectValue_ChippingRegArea[0].T.Clone();
                            hv_ChippingRegRowTup = hvec_IcDefectValue_ChippingRegArea[1].T.Clone();
                            hv_ChippingRegColTup = hvec_IcDefectValue_ChippingRegArea[2].T.Clone();
                        }
                    }
                }

                //例：解析第0个die Epoxy
                HTupleVector hvec_DieDefectValueEpoxys = new HTupleVector(4);
                HTupleVector hvec_EpoxysDefectValue = new HTupleVector(3), hvec_EpoxyIdDefectValue = new HTupleVector(2);
                HTuple hv_BondDefectValue_LenR = new HTuple(), hv_BondDefectValue_High = new HTuple();
                //1d
                hvec_DieDefectValueEpoxys = hvDefectValueEpoxys[0];
                //2d
                HTuple end_val419 = new HTuple(hvec_DieDefectValueEpoxys.Length) - 1;
                HTuple step_val419 = 1;
                for (hv_idx = 0; hv_idx.Continue(end_val419, step_val419); hv_idx = hv_idx.TupleAdd(step_val419))
                {
                    hvec_EpoxysDefectValue = hvec_DieDefectValueEpoxys[hv_idx];
                    //3d
                    HTuple end_val422 = new HTuple(hvec_EpoxysDefectValue.Length) - 1;
                    HTuple step_val422 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val422, step_val422); hv_i = hv_i.TupleAdd(step_val422))
                    {
                        hvec_EpoxyIdDefectValue = hvec_EpoxysDefectValue[hv_i];
                        //4d/5d
                        if ((int)(new HTuple((new HTuple(hvec_EpoxyIdDefectValue.Length)).TupleLess(
                            2))) != 0)
                        {
                            //空向量
                            hv_BondDefectValue_LenR = new HTuple();
                            hv_BondDefectValue_High = new HTuple();
                        }
                        else
                        {
                            //银胶长度比
                            hv_BondDefectValue_LenR = hvec_EpoxyIdDefectValue[0][0].T.Clone();
                            //银胶高度
                            hv_BondDefectValue_High = hvec_EpoxyIdDefectValue[1][0].T.Clone();
                        }
                    }
                }

                //例：解析第0个die Bond
                HTupleVector hvec_DieDefectValueBonds = new HTupleVector(4), hvec_BondsDefectValue = new HTupleVector(3);
                HTupleVector hvec_BondIdDefectValue = new HTupleVector(2), hvec_BondDefectValue_PadArea = new HTupleVector(1);
                HTuple hv_BondDefectValue_Radius = new HTuple(), hv_BondDefectValue_Diff = new HTuple();
                HTuple hv_BondDefectValue_Tail = new HTuple(), hv_PadAreaTup = new HTuple();
                HTuple hv_PadRowTup = new HTuple(), hv_PadColTup = new HTuple();
                //1d
                hvec_DieDefectValueBonds = hvDefectValueBonds[0];
                //2d
                HTuple end_val442 = new HTuple(hvec_DieDefectValueBonds.Length) - 1;
                HTuple step_val442 = 1;
                for (hv_idx = 0; hv_idx.Continue(end_val442, step_val442); hv_idx = hv_idx.TupleAdd(step_val442))
                {
                    hvec_BondsDefectValue = hvec_DieDefectValueBonds[hv_idx];
                    //3d
                    HTuple end_val445 = new HTuple(hvec_BondsDefectValue.Length) - 1;
                    HTuple step_val445 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val445, step_val445); hv_i = hv_i.TupleAdd(step_val445))
                    {
                        hvec_BondIdDefectValue = hvec_BondsDefectValue[hv_i];
                        //4d/5d
                        if ((int)(new HTuple((new HTuple(hvec_BondIdDefectValue.Length)).TupleLess(
                            2))) != 0)
                        {
                            //空向量
                            hv_BondDefectValue_Radius = new HTuple();
                            hv_BondDefectValue_Diff = new HTuple();
                            hv_BondDefectValue_Tail = new HTuple();
                            hv_PadAreaTup = new HTuple();
                            hv_PadRowTup = new HTuple();
                            hv_PadColTup = new HTuple();
                        }
                        else
                        {
                            //焊球半径
                            hv_BondDefectValue_Radius = hvec_BondIdDefectValue[0][0].T.Clone();
                            //焊点偏移 < 0：为无该检测项
                            hv_BondDefectValue_Diff = hvec_BondIdDefectValue[1][0].T.Clone();
                            //尾丝长 < 0：为无该检测项
                            hv_BondDefectValue_Tail = hvec_BondIdDefectValue[2][0].T.Clone();
                            //焊盘异物
                            hvec_BondDefectValue_PadArea = hvec_BondIdDefectValue[3];
                            //5d: 异物面积及对应RowCol坐标
                            hv_PadAreaTup = hvec_BondDefectValue_PadArea[0].T.Clone();
                            hv_PadRowTup = hvec_BondDefectValue_PadArea[1].T.Clone();
                            hv_PadColTup = hvec_BondDefectValue_PadArea[2].T.Clone();
                        }
                    }
                }

                //例：解析第0个die Wire
                HTupleVector hvec_DieDefectValueWires = new HTupleVector(4), hvec_WiresDefectValue = new HTupleVector(3);
                HTupleVector hvec_WireIdDefectValue = new HTupleVector(2);
                HTuple hv_WireDefectValue_Gap = new HTuple();
                HTuple end_val475 = new HTuple(hvDefectValueWires.Length) - 1;
                HTuple step_val475 = 1;
                for (hv_idx = 0; hv_idx.Continue(end_val475, step_val475); hv_idx = hv_idx.TupleAdd(step_val475))
                {
                    hvec_DieDefectValueWires = hvDefectValueWires[hv_idx];
                    //2d
                    HTuple end_val478 = new HTuple(hvec_DieDefectValueWires.Length) - 1;
                    HTuple step_val478 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val478, step_val478); hv_i = hv_i.TupleAdd(step_val478))
                    {
                        hvec_WiresDefectValue = hvec_DieDefectValueWires[hv_i];
                        //3d
                        HTuple end_val481 = new HTuple(hvec_WiresDefectValue.Length) - 1;
                        HTuple step_val481 = 1;
                        for (hv_j = 0; hv_j.Continue(end_val481, step_val481); hv_j = hv_j.TupleAdd(step_val481))
                        {
                            hvec_WireIdDefectValue = hvec_WiresDefectValue[hv_j];
                            //4d/5d
                            //断线距离: <0 为断线  > 0 为正常
                            hv_WireDefectValue_Gap = hvec_WireIdDefectValue[0][0].T.Clone();
                        }
                    }
                }
                */
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //imgIndex++;
                //if (imgIndex + 1 > DieRegions.CountObj())
                //{
                //imgIndex = 0;
                //}
                //return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                //imgIndex++;
                //if (imgIndex + 1 > DieRegions.CountObj())
                //{
                    //imgIndex = 0;
                //}
                return;
            }       

        }

        // 0118 lw
        public void DisplayNodeResultRegion()
        {
            if ((FailRegResult == null) || (BondWireRegResult == null))
            {
                return;
            }

            htWindow.InitialHWindow("red");
            htWindow.hTWindow.HalconWindow.DispObj(FailRegResult);

            htWindow.InitialHWindow("orange");
            htWindow.hTWindow.HalconWindow.DispObj(BondWireRegResult);
        }

        //加载图集
        private void ExecuteImagesSetVerifyCommand(object parameter)
        {
            try
            {
                using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        VerifyImagesDirectory = fbd.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                    if (MessageBox.Show("是否为指定Fov的task图集类型？", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        isFovTaskFlag = 1;

                        // 指定Fov合成多通道图并显示第一张图
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                VerifyImagesDirectory,
                                                                                IniParameters.CurFovName,
                                                                                0, out HTuple hv_o_ImageVerifyNum, out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        CurrentVerifySet = hv_o_ImageVerifyNum;
                        PImageIndexPath = imageFiles[IniParameters.ImageIndex];
                        ImageVerify = ho_MutiImage;
                    }
                    else
                    {
                        isFovTaskFlag = 0;

                        Algorithm.File.list_image_files(VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        string[] folderList = imageFiles;
                        CurrentVerifySet = folderList.Count();
                        PImageIndexPath = imageFiles[0];           
                        HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                        ImageVerify = image;
                    }
                    //ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, imageIndex);
                    htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, IniParameters.ImageIndex), true);
                    pImageIndex = 0;
                    //imgIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteRefreshImagesSetCommand(object parameter)
        {
            try
            {
                VerifyImagesDirectory = IniParameters.TrainningImagesDirectory;
                if (Directory.Exists(VerifyImagesDirectory))
                {
                    Algorithm.File.list_image_files(VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                    string[] folderList = imageFiles;
                    CurrentVerifySet = folderList.Count();
                    PImageIndexPath = imageFiles[0];
                    HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                    ImageVerify = image;
                    //ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, imageIndex);
                    htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, IniParameters.ImageIndex), true);
                    pImageIndex = 0;
                    //imgIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //前一页
        private void ExecutePreviousCommand(object parameter)
        {
            try
            {
                //imgIndex = 0;
                if (CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == 0 ? CurrentVerifySet - 1 : --pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                VerifyImagesDirectory,
                                                                                IniParameters.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[IniParameters.ImageIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image; 
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    //ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, imageIndex);
                    htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, IniParameters.ImageIndex), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        //后一页
        private void ExecuteNextCommand(object parameter)
        {
            try
            {
                //imgIndex = 0;
                if (CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == CurrentVerifySet - 1 ? 0 : ++pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                VerifyImagesDirectory,
                                                                                IniParameters.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[IniParameters.ImageIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    //ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, imageIndex);
                    htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, IniParameters.ImageIndex), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public bool CheckCompleted()
        {
            return true;
        }

        public void Initial()
        {
            htWindow.ClearSelection();
            if (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff) return;
            htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, IniParameters.ImageIndex), true);
            
            //1123
            ChannelNames = new ObservableCollection<ChannelName>(IniParameters.ChannelNames);
            OnPropertyChanged("ChannelNames");

            switchImageChannelIndex = IniParameters.ImageIndex;
            OnPropertyChanged("SwitchImageChannelIndex");

            //1201 lw
            if (IniParameters.CurFovName == "" && IniParameters.IniDirectory != null)
            {
                HOperatorSet.TupleSplit(IniParameters.IniDirectory, "\\", out HTuple hv_subStr);
                HTuple FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 3];
                IniParameters.CurFovName = FOV_Name;
            }

            // 避免其他页滚轮显示检测区域
            IniParameters.IsInspectNodeVerify = true;
        }

        public void Dispose()
        {
            (Content as Page_InspectNode).DataContext = null;
            (Content as Page_InspectNode).Close();
            Content = null;
            htWindow = null;
            ChannelImageVerify = null;
            ImageVerify = null;
            ImageVerifyConcat = null;
            VerifyCommand = null;
            SaveCommand = null;
            ImagesSetVerifyCommand = null;
            PreviousCommand = null;
            NextCommand = null;
            RefreshImagesSetCommand = null;
            ClearModelCommand = null;
        }
    }
}
