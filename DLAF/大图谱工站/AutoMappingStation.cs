using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFToolKits;
using JFInterfaceDef;
using HalconDotNet;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;

namespace DLAF
{
    [JFDisplayName("自动生成图谱工站")]
    public class AutoMappingStation:JFStationBase
    {
        public FrmAutoMapping autoMapping;
        public AutoMappingOperation operation;
        const bool use_current_dir = false;
        static string currentDir = Directory.GetCurrentDirectory();
        static string programDir = (use_current_dir ? Environment.CurrentDirectory : "D:\\ht'tech") + "\\AOI_LF";
        public string SystemUV2XYDir { get { return (programDir + "\\System\\UV2XY"); } }
        //public IniFiles scanIniConfig = null;
        public HObject frameMapImg = null;
        public HObject showRegion;
        public HObject Image;
        public List<HTuple> List_UV2XYResult = null;
        public List<ImagePosition> ScanMapPostions = null;
        public List<ImagePosition> ClipMapPostions = null;

        public JFDLAFProductRecipe jFDLAFProductRecipe;

        public IniFiles formIniConfig;

        public HTuple hv_xSnapPosLT;
        public HTuple hv_ySnapPosLT;
        public HTuple clipMapX = null;
        public HTuple clipMapY = null;
        public HTuple clipMapU = null;
        public HTuple clipMapV = null;
        public HTuple clipMapRow;
        public HTuple clipMapCol;
       
        public HTuple hv_updateMapX;
        public HTuple hv_updateMapY;

        public HTuple hv_foundU;
        public HTuple hv_foundV;

        public HTuple snapMapX;
        public HTuple snapMapY;
        public HTuple snapMapRow;
        public HTuple snapMapCol;

        public HTuple hv_defRow2Y = null;
        public HTuple hv_defCol2X = null;
        public HTuple hv_modelHmap = null;
        public HTuple hv_updateCoorX = null;
        public HTuple hv_updateCoorY = null;
        public HTuple hv_defRow2YR = null;
        public HTuple hv_defCol2XR = null;
        public HTuple hv_modelHmapR = null;
        public HTuple hv_updateCoorXR = null;
        public HTuple hv_updateCoorYR = null;
        public HTuple hv_updateSnapMapX = null;
        public HTuple hv_updateSnapMapY = null;

        #region System 参数/描述
        private string categoryName = "自动生成大图谱工站";
        private string mRunMode = "系统运行模式";
        private string mNum_Camera = "相机总数";
        private string mSelectIndex = "当前选定的相机";
        //private string mZFocus = "Z轴聚焦高度";
        private string mZ_safe = "相机Z轴安全位";
        //private string mImageFolder = "图像存储主目录";
        //private string mActivePdt = "当前加载的产品名称";
        private string mdieWidth = "Die Width";
        private string mdieHeight = "Die Height";
        private string mref_x = "参考原点X";
        private string mref_y = "参考原点Y";
        private string mref_z = "参考原点Z";
        private string mscanRowNum = "Scan Row Num";
        private string mscanColNum = "Scan Col Num";
        private string mclipPosNum = "Clip Pos Num";
        private string mscanPosNum = "Scan Pos Num";

        //private string mFrameLength = "料片长度（mm）";
        //private string mRowNumber = "Block内行数目";
        //private string mColumnNumber = "Block内列数目";
        //private string mBlockNumber = "Block数目";


        /// <summary>
        /// 图像存储主目录
        /// </summary>
        public String imageFolder = "";
        public int _RunMode = 0;
        public int Num_Camera = 0;
        public int SelectedIndex = 0;
        public double ZFocus = 0;
        public double Z_safe = 0;
        public string ActivePdt = "";//Active product
        public HTuple hv_dieWidth=0;
        public HTuple hv_dieHeight=0;
        public double ref_x = 0;
        public double ref_y = 0;
        public double ref_z = 0;
        public int scanRowNum = 0;
        public int scanColNum = 0;
        public int clipPosNum = 0;
        public int scanPosNum = 0;
        #endregion

        #region 打标
        public bool IsX = false;
        public double RXorY = 4;

        public double ref_Mark_x;                //参考原点
        public double ref_Mark_y;
        public double ref_Mark_z;
        public string ProductDir = "D:\\products";
        [CategoryAttribute("机构属性"), DisplayNameAttribute("喷印器对相机X偏移(mm)"), DescriptionAttribute("喷印器相对相机X偏移,负左正右")]
        public double Ref_Mark_x { get { return ref_Mark_x; } set { ref_Mark_x = value; } }
        [CategoryAttribute("机构属性"), DisplayNameAttribute("喷印器对相机Y偏移(mm)"), DescriptionAttribute("喷印器相对相机Y偏移,负下正上")]
        public double Ref_Mark_y { get { return ref_Mark_y; } set { ref_Mark_y = value; } }
        [CategoryAttribute("机构属性"), DisplayNameAttribute("喷印标定喷印高度Z(mm)"), DescriptionAttribute("喷印器标定时喷印高度Z, 负下正上")]
        public double Ref_Mark_z { get { return ref_Mark_z; } set { ref_Mark_z = value; } }
        #endregion

        public Model CheckPosModels;
        public Model CheckPosRModels; //右矫正点模板

        public JFXmlDictionary<string, object> Dict { get; internal set; }

        public JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string,string>>> visionCfgParams { get; internal set; }

        #region 工站专用参数/描述
        //private string mProductParams = "产品对应参数";
        
        private string mgenMapStartX = "图谱起点X(mm)";
        private string mgenMapStartY = "图谱起点Y(mm)";
        private string mgenMapEndX = "图谱终点X(mm)";
        private string mgenMapEndY = "图谱终点Y(mm)";
        private string msameSpace = "重复距离(mm)";
        private string mlctScoreThresh = "定位匹配分数(0-1)";
        private string mcheckMdlMethod = "模板制作方法";
        private string musedoubleCheck = "使用双校准点";
        private string mwidth = "图像Width";
        private string mheight = "图像Height";
        private string mVisionCfgName = "视觉光源名称配置";
        private string mFovRow = "视野Row坐标";
        private string mFovCol = "视野Col坐标";
 
        public double genMapStartX;
        public double genMapStartY;
        public double genMapEndX;
        public double genMapEndY;
        public double sameSpace = 3;
        public double lctScoreThresh = 0.6;
        public int checkMdlMethod = 0;// 矫正点模板制作方法
        public int usedoubleCheck = 0;//双矫正点
        public HTuple width = 2448, height = 2048;

        /// <summary>
        /// IC FOV Row坐标列表
        /// </summary>
        public List<double> ICFovRow;

        /// <summary>
        /// IC FOV Col坐标列表
        /// </summary>

        public List<double> ICFovCol;
        #endregion

        #region 料片属性
        private Int32 _columnNumber = 20;
        private Int32 _blockNumber = 1;
        private Int32 _rowNumber = 14;
        private Double _frameLength = 0;
        [CategoryAttribute("料片属性"), DisplayNameAttribute("①料片长度（mm）")]
        public Double FrameLength
        {
            get { return _frameLength; }
            set { _frameLength = value; }
        }

        [CategoryAttribute("料片属性"), DisplayNameAttribute("②Block内行数目")]
        public Int32 RowNumber
        {
            get { return _rowNumber; }
            set
            {
                _rowNumber = value;
            }
        }
        [CategoryAttribute("料片属性"), DisplayNameAttribute("③Block内列数目")]
        public Int32 ColumnNumber
        {
            get { return _columnNumber; }
            set
            {
                _columnNumber = value;
            }
        }

        [CategoryAttribute("料片属性"), DisplayNameAttribute("④Block数目")]
        public Int32 BlockNumber
        {
            get { return _blockNumber; }
            set
            {
                _blockNumber = value;
            }
        }

        [CategoryAttribute("料片属性"), DisplayNameAttribute("⑤使用打标器")]
        public Boolean UseMarker { get; set; }
        [CategoryAttribute("料片属性"), DisplayNameAttribute("⑥双芯片Block标记点x镜像")]
        public Boolean SymmetryMark { get; set; }
        [CategoryAttribute("料片属性"), DisplayNameAttribute("⑦打标点相对X（mm）"), DescriptionAttribute("打标位置相对芯片位置,偏左为负值,X方向")]
        public double RelativeMark_X { get; set; }
        [CategoryAttribute("料片属性"), DisplayNameAttribute("⑧打标点相对Y（mm）"), DescriptionAttribute("打标位置相对芯片位置,偏下为负值,Y方向")]
        public double RelativeMark_Y { get; set; }
        [ReadOnly(true), CategoryAttribute("料片属性"), DisplayNameAttribute("⑨打标高度Z（mm）"), DescriptionAttribute("打标位置高度,相机Z轴,Z方向")]
        public double RelativeMark_Z
        {
            get
            {
                return Ref_Mark_z;
                //return App.obj_Chuck.Ref_Mark_z+_zFocus-App.obj_Chuck.Mark_Focus_z;
            }
            set
            {
            }
        }
        #endregion

        #region 设备名称/通道描述
        public string[] AxisX = new string[] { "2D相机X轴" };
        public string[] AxisY = new string[] { "2D相机Y轴" };
        public string[] AxisZ = new string[] { "2D相机Z轴" };
        public string[] AxisXYZ = new string[] { "2D相机X轴", "2D相机Y轴", "2D相机Z轴" };
        public string[] CamereDev = new string[] { "海康相机", "海康相机" };
        #endregion 

        /// <summary>
        /// UV2XY模版存放路径
        /// </summary>
        private string _calibrUV2XYModelPath = string.Empty;
        [BrowsableAttribute(false)]
        public string CalibrUV2XYModelPath
        {
            get { return _calibrUV2XYModelPath; }
            set
            {
                _calibrUV2XYModelPath = value;
            }
        }
        
        public AutoMappingStation()
        {
            ICFovRow = new List<double>();
            ICFovCol = new List<double>();

            CheckPosModels = new Model();
            CheckPosRModels = new Model();

            operation = new AutoMappingOperation();
            operation.SetStation(this);
            DeclearStationCfgParam();

            _rtUi.SetStation(this);
        }

        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;
            //InitStationParams();
            return true;
        }

        private void DeclearStationCfgParam()
        {
            DeclearCfgParam(mwidth, typeof(double), categoryName);
            DeclearCfgParam(mheight, typeof(double), categoryName);
            DeclearCfgParam(mRunMode, typeof(int), categoryName);
            DeclearCfgParam(mNum_Camera, typeof(int), categoryName);
            DeclearCfgParam(mSelectIndex, typeof(int), categoryName);
            DeclearCfgParam(mZ_safe, typeof(double), categoryName);
        }

        public void InitStationParams()
        {
            try
            {
                Dict = new JFXmlDictionary<string, object>();
                visionCfgParams = new JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string, string>>>();
                //系统参数
                imageFolder = (string)JFHubCenter.Instance.RecipeManager.GetInitParamValue((string)JFHubCenter.Instance.RecipeManager.InitParamNames[2]);
                ActivePdt = (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID");

                formIniConfig = new IniFiles(imageFolder+"\\"+ ActivePdt + "\\scanPoint.ini");
                //工站参数
                width = (double)GetCfgParamValue(mwidth);
                height = (double)GetCfgParamValue(mheight);
                _RunMode = (int)GetCfgParamValue(mRunMode);
                Num_Camera = (int)GetCfgParamValue(mNum_Camera);
                SelectedIndex = (int)GetCfgParamValue(mSelectIndex);
                Z_safe = (double)GetCfgParamValue(mZ_safe);

                jFDLAFProductRecipe = ((JFDLAFProductRecipe)JFHubCenter.Instance.RecipeManager.GetRecipe("Product", (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID")));
                //Recipe参数
                if (jFDLAFProductRecipe != null)
                {
                    FrameLength = jFDLAFProductRecipe.FrameLength;
                    RowNumber = jFDLAFProductRecipe.RowNumber;
                    ColumnNumber = jFDLAFProductRecipe.ColumnNumber;
                    BlockNumber = jFDLAFProductRecipe.BlockNumber;
                    visionCfgParams = jFDLAFProductRecipe.visionCfgParams;
                    ZFocus = jFDLAFProductRecipe.ZFocus;
                }
                else
                {
                    return;
                }

                if (jFDLAFProductRecipe.AutoMappingStationProInf != "")
                {
                    string xmlTxt = jFDLAFProductRecipe.AutoMappingStationProInf;
                    Dict = JFFunctions.FromXTString(xmlTxt, Dict.GetType()) as JFXmlDictionary<string, object>;

                    xmlTxt = Dict.ContainsKey(mVisionCfgName) ? (string)Dict[mVisionCfgName] : "";
                    if (xmlTxt != "")
                        visionCfgParams = JFFunctions.FromXTString(xmlTxt, visionCfgParams.GetType()) as JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string, string>>>;

                    //工站中默认的参数
                    genMapStartX = Dict.ContainsKey(mgenMapStartX) ? (double)Dict[mgenMapStartX] : 0;
                    genMapStartY = Dict.ContainsKey(mgenMapStartY) ? (double)Dict[mgenMapStartY] : 0;
                    genMapEndX = Dict.ContainsKey(mgenMapEndX) ? (double)Dict[mgenMapEndX] : 0;
                    genMapEndY = Dict.ContainsKey(mgenMapEndY) ? (double)Dict[mgenMapEndY] : 0;
                    sameSpace = Dict.ContainsKey(msameSpace) ? (double)Dict[msameSpace] : 0;
                    lctScoreThresh = Dict.ContainsKey(mlctScoreThresh) ? (double)Dict[mlctScoreThresh] : 0;
                    checkMdlMethod = Dict.ContainsKey(mcheckMdlMethod) ? (int)Dict[mcheckMdlMethod] : 0;
                    usedoubleCheck = Dict.ContainsKey(musedoubleCheck) ? (int)Dict[musedoubleCheck] : 0;

                    //ZFocus = Dict.ContainsKey(mZFocus) ? (double)Dict[mZFocus] : 0;
                    hv_dieWidth = Dict.ContainsKey(mdieWidth) ? (double)Dict[mdieWidth] : 0;
                    hv_dieHeight = Dict.ContainsKey(mdieHeight) ? (double)Dict[mdieHeight] : 0;
                    ref_x = Dict.ContainsKey(mref_x) ? (double)Dict[mref_x] : 0;
                    ref_y = Dict.ContainsKey(mref_y) ? (double)Dict[mref_y] : 0;
                    ref_z = Dict.ContainsKey(mref_z) ? (double)Dict[mref_z] : 0;
                    scanRowNum = Dict.ContainsKey(mscanRowNum) ? (int)Dict[mscanRowNum] : 0;
                    scanColNum = Dict.ContainsKey(mscanColNum) ? (int)Dict[mscanColNum] : 0;
                    clipPosNum = Dict.ContainsKey(mclipPosNum) ? (int)Dict[mclipPosNum] : 0;
                    scanPosNum = Dict.ContainsKey(mscanPosNum) ? (int)Dict[mscanPosNum] : 0;

                    if (Dict.ContainsKey(mFovRow))
                    {
                        ICFovRow = (List<double>)Dict[mFovRow];
                    }
                    if (Dict.ContainsKey(mFovCol))
                    {
                        ICFovCol = (List<double>)Dict[mFovCol];
                    }
                }
                else
                {
                    if(Config.ContainsItem((string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID")))
                    {
                        string xmlTxt = (string)GetCfgParamValue((string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                        Dict = JFFunctions.FromXTString(xmlTxt, Dict.GetType()) as JFXmlDictionary<string, object>;

                        xmlTxt = Dict.ContainsKey(mVisionCfgName) ? (string)Dict[mVisionCfgName] : "";
                        if (xmlTxt != "")
                            visionCfgParams = JFFunctions.FromXTString(xmlTxt, visionCfgParams.GetType()) as JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string, string>>>;

                        //工站中默认的参数
                        genMapStartX = Dict.ContainsKey(mgenMapStartX) ? (double)Dict[mgenMapStartX] : 0;
                        genMapStartY = Dict.ContainsKey(mgenMapStartY) ? (double)Dict[mgenMapStartY] : 0;
                        genMapEndX = Dict.ContainsKey(mgenMapEndX) ? (double)Dict[mgenMapEndX] : 0;
                        genMapEndY = Dict.ContainsKey(mgenMapEndY) ? (double)Dict[mgenMapEndY] : 0;
                        sameSpace = Dict.ContainsKey(msameSpace) ? (double)Dict[msameSpace] : 0;
                        lctScoreThresh = Dict.ContainsKey(mlctScoreThresh) ? (double)Dict[mlctScoreThresh] : 0;
                        checkMdlMethod = Dict.ContainsKey(mcheckMdlMethod) ? (int)Dict[mcheckMdlMethod] : 0;
                        usedoubleCheck = Dict.ContainsKey(musedoubleCheck) ? (int)Dict[musedoubleCheck] : 0;
                        //ZFocus = Dict.ContainsKey(mZFocus) ? (double)Dict[mZFocus] : 0;
                        hv_dieWidth = Dict.ContainsKey(mdieWidth) ? (double)Dict[mdieWidth] : 0;
                        hv_dieHeight = Dict.ContainsKey(mdieHeight) ? (double)Dict[mdieHeight] : 0;
                        ref_x = Dict.ContainsKey(mref_x) ? (double)Dict[mref_x] : 0;
                        ref_y = Dict.ContainsKey(mref_y) ? (double)Dict[mref_y] : 0;
                        ref_z = Dict.ContainsKey(mref_z) ? (double)Dict[mref_z] : 0;
                        scanRowNum = Dict.ContainsKey(mscanRowNum) ? (int)Dict[mscanRowNum] : 0;
                        scanColNum = Dict.ContainsKey(mscanColNum) ? (int)Dict[mscanColNum] : 0;
                        clipPosNum = Dict.ContainsKey(mclipPosNum) ? (int)Dict[mclipPosNum] : 0;
                        scanPosNum = Dict.ContainsKey(mscanPosNum) ? (int)Dict[mscanPosNum] : 0;

                        if (Dict.ContainsKey(mFovRow))
                        {
                            ICFovRow = (List<double>)Dict[mFovRow];
                        }
                        if (Dict.ContainsKey(mFovCol))
                        {
                            ICFovCol = (List<double>)Dict[mFovCol];
                        }
                    }
                }

                //else if(File.Exists(imageFolder + "\\" + ActivePdt + "\\scanPoint.ini"))
                //{
                //    formIniConfig.ReadDouble("ScanPoints", "genMapStartX", out genMapStartX);
                //    formIniConfig.ReadDouble("ScanPoints", "genMapStartY", out genMapStartY);
                //    formIniConfig.ReadDouble("ScanPoints", "genMapEndX", out genMapEndX);
                //    formIniConfig.ReadDouble("ScanPoints", "genMapEndY", out genMapEndY);
                //    formIniConfig.ReadDouble("ScanPoints", "sameSpace", out sameSpace);
                //    formIniConfig.ReadDouble("ScanPoints", "scaleFactor", out scaleFactor);
                //    formIniConfig.ReadDouble("ScanPoints", "lctScoreThresh", out lctScoreThresh);
                //    formIniConfig.ReadDouble("ScanPoints", "checkPosX", out checkPosX);
                //    formIniConfig.ReadDouble("ScanPoints", "checkPosY", out checkPosY);
                //    formIniConfig.ReadDouble("ScanPoints", "checkPosScoreThresh", out checkPosScoreThresh);
                //    formIniConfig.ReadDouble("ScanPoints", "widthFactor", out widthFactor);
                //    formIniConfig.ReadDouble("ScanPoints", "heightFactor", out heightFactor);
                //    formIniConfig.ReadInteger("ScanPoints", "CheckModelMethod", out checkMdlMethod);
                //    formIniConfig.ReadDouble("ScanPoints", "checkPosRX", out checkPosRX);
                //    formIniConfig.ReadDouble("ScanPoints", "checkPosRY", out checkPosRY);
                //    formIniConfig.ReadDouble("ScanPoints", "checkPosRScoreThresh", out checkPosRScoreThresh);
                //    formIniConfig.ReadInteger("ScanPoints", "useDoubleCheck", out usedoubleCheck);

                //    ICFovRow = new List<double>();
                //    ICFovCol = new List<double>();
                //}
            }
            catch
            {

            }
        }

        public void SaveStationParams()
        {
            try
            {
                string recipeID = (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID");
                //工站参数
                SetCfgParamValue(mwidth, width.D);
                SetCfgParamValue(mheight, height.D);
                SetCfgParamValue(mRunMode, RunMode);
                SetCfgParamValue(mNum_Camera, Num_Camera);
                SetCfgParamValue(mSelectIndex, SelectedIndex);
                SetCfgParamValue(mZ_safe, Z_safe);

                //工站中默认的参数
                AddDictItem(mgenMapStartX, genMapStartX);
                AddDictItem(mgenMapStartY, genMapStartY);
                AddDictItem(mgenMapEndX, genMapEndX);
                AddDictItem(mgenMapEndY, genMapEndY);
                AddDictItem(msameSpace, sameSpace);
                AddDictItem(mlctScoreThresh, lctScoreThresh);
                AddDictItem(mcheckMdlMethod, checkMdlMethod);
                AddDictItem(musedoubleCheck, usedoubleCheck);
                //AddDictItem(mZFocus, ZFocus);

                double value = hv_dieWidth.D;
                AddDictItem(mdieWidth, value);
                value = hv_dieHeight.D;
                AddDictItem(mdieHeight, value);

                AddDictItem(mref_x, ref_x);
                AddDictItem(mref_y, ref_y);
                AddDictItem(mref_z, ref_z);
                AddDictItem(mscanRowNum, scanRowNum);
                AddDictItem(mscanColNum, scanColNum);
                AddDictItem(mclipPosNum, clipPosNum);
                AddDictItem(mscanPosNum, scanPosNum);

                string xmlTxt = null;
                string typeTxt = null;
                JFFunctions.ToXTString(visionCfgParams, out xmlTxt, out typeTxt);
                AddDictItem(mVisionCfgName, xmlTxt);

                AddDictItem(mFovRow, ICFovRow);
                AddDictItem(mFovCol, ICFovCol);

                JFFunctions.ToXTString(Dict, out xmlTxt, out typeTxt);

                //若工站xml配置中不需要保存参数，则注释该行即可，那么工站参数仅会保存在Recipe manager中；
                SetCfgParamValue(recipeID, xmlTxt);

                jFDLAFProductRecipe.AutoMappingStationProInf = xmlTxt;
                jFDLAFProductRecipe.SaveParamsToCfg();
                JFHubCenter.Instance.RecipeManager.Save();

            }
            catch
            {

            }
        }

        private void AddDictItem(string itemName,object value)
        {
            if(Dict.ContainsKey(itemName))
            {
                Dict[itemName] = value;
            }
            else
            {
                Dict.Add(itemName, value);
            }
        }

        public override Form GenForm()
        {
            autoMapping = new FrmAutoMapping();
            autoMapping.TopMost = false;
            autoMapping.SetStation(this);
            return autoMapping;
        }

        string _name = "";
        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }

        }
        public override int[] AllCmds { get { return null; } }

        protected override void ExecuteReset()
        {
            return;
        }

        public override int[] AllCustomStatus { get { return null; } }

        public override string GetCmdName(int cmd)
        {
            return "";
        }

        public override string GetCustomStatusName(int status)
        {
            return "";
        }

        protected override void CleanupWhenWorkExit()
        {
            return;
        }

        protected override void ExecuteEndBatch()
        {
            return;
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
            return;
        }

        protected override void PrepareWhenWorkStart()
        {
            //打开相机
            //打开电机，伺服使能
            return;
        }

        protected override void RunLoopInWork()
        {
            return;
        }

        public override JFStationRunMode RunMode { get { return JFStationRunMode.Auto; } }
        public override bool SetRunMode(JFStationRunMode runMode)
        {
            return false;
        }


        UcRtAutoMapping _rtUi = new UcRtAutoMapping();

        public override JFRealtimeUI GetRealtimeUI()
        {
            return _rtUi;
            //return base.GetRealtimeUI();
        }
    }

    /// <summary>
    /// 矫正点模板定位方式
    /// </summary>
    public enum CheckModelMethod
    {
        /// <summary>
        ///精确定位（画十字线）
        /// </summary>
        MODEL_EX = 0,
        /// <summary>
        /// 普通定位
        /// </summary>
        MODEL_ORD = 1
    }

    enum SystemRunMode
    {
        MODE_ONLINE = 0,
        MODE_OFFLINE = 1,
    }
}
