using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using HTHalControl;
using HalconDotNet;
using JFInterfaceDef;
using JFToolKits;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Windows.Documents;
using System.Web.UI.MobileControls;
using JFHub;

namespace DLAF
{
    [Serializable]
    public class JFDLAFProductRecipe:IJFRecipe
    {
        public HTuple snapMapX;
        public HTuple snapMapY;
        public HTuple snapMapRow;
        public HTuple snapMapCol;

        public HTuple icMapX;
        public HTuple icMapY;
        public HTuple icMapRow;
        public HTuple icMapCol;

        private HObject AllDieMatchRegion;

        public int mgzIdx = 0;
        public string magezineBox = "";
        private Double _ref_zFocus = 0;
        private Double Mark_Focus_z = 0;

        /// <summary>
        /// 产品/配方 类别 比如：托盘 产品 等
        /// </summary>
        [BrowsableAttribute(false)]
        public string Categoty { get; internal set; }

        /// <summary>
        /// 产品/配方 名称
        /// </summary>
        [BrowsableAttribute(false)]
        public string ID { get; internal set; }

        /// <summary>
        /// Mark1点名称
        /// </summary>
        [BrowsableAttribute(false)]
        public string Mark1VisionCfgName { get; internal set; }

        /// <summary>
        /// Mark2点名称
        /// </summary>
        [BrowsableAttribute(false)]
        public string Mark2VisionCfgName { get; internal set; }

        /// <summary>
        /// Mark1点拍照X坐标
        /// </summary>
        [BrowsableAttribute(false)]
        private double Mark1SnapPosX { get; set; }

        /// <summary>
        /// Mark1点拍照Y坐标
        /// </summary>
        [BrowsableAttribute(false)]
        private double Mark1SnapPosY { get; set; }

        /// <summary>
        /// Mark2点拍照X坐标
        /// </summary>
        [BrowsableAttribute(false)]
        private double Mark2SnapPosX { get; set; }

        /// <summary>
        /// Mark2点拍照Y坐标
        /// </summary>
        [BrowsableAttribute(false)]
        private double Mark2SnapPosY { get; set; }

        /// <summary>
        /// Mark1点实际X坐标
        /// </summary>
        [BrowsableAttribute(false)]
        private double Mark1RWPosX { get; set; }

        /// <summary>
        /// Mark1点实际Y坐标
        /// </summary>
        [BrowsableAttribute(false)]
        private double Mark1RWPosY { get; set; }

        /// <summary>
        /// Mark2点实际X坐标
        /// </summary>
        [BrowsableAttribute(false)]
        private double Mark2RWPosX { get; set; }

        /// <summary>
        /// Mark2点实际Y坐标
        /// </summary>
        [BrowsableAttribute(false)]
        private double Mark2RWPosY { get; set; }

        /// <summary>
        /// IC FOV X坐标列表
        /// </summary>
        public List<double> ICFovOffsetX;

        /// <summary>
        /// IC FOV Y坐标列表
        /// </summary>
        public List<double> ICFovOffsetY;

        /// <summary>
        /// Fov中Row中的Die的最大数量
        /// </summary>
        [BrowsableAttribute(false)]
        public int DieRowInFov { get; set; }

        /// <summary>
        /// Fov中Col中的Die的最大数量
        /// </summary>
        [BrowsableAttribute(false)]
        public int DieColInFov { get; set; }

        /// <summary>
        /// Die Width
        /// </summary>
        [BrowsableAttribute(false)]
        public double DieWidth { get; set; }

        /// <summary>
        /// Die Height
        /// </summary>
        [BrowsableAttribute(false)]
        public double DieHeight { get; set; }

        /// <summary>
        /// 有效宽比例
        /// </summary>
        [BrowsableAttribute(false)]
        public double WidthFactor { get; set; }

        /// <summary>
        /// 有效高比例
        /// </summary>
        [BrowsableAttribute(false)]
        public double HeightFactor { get; set; }

        /// <summary>
        /// 降采样比例
        /// </summary>
        [BrowsableAttribute(false)]
        public double ScaleFactor { get; set; }

        /// <summary>
        /// 左矫正点X
        /// </summary>
        [BrowsableAttribute(false)]
        public double CheckPosX { get; set; }

        /// <summary>
        /// 左矫正点Y
        /// </summary>
        [BrowsableAttribute(false)]
        public double CheckPosY { get; set; }

        /// <summary>
        /// 右矫正点X
        /// </summary>
        [BrowsableAttribute(false)]
        public double CheckPosRX { get; set; }

        /// <summary>
        /// 右矫正点Y
        /// </summary>
        [BrowsableAttribute(false)]
        public double CheckPosRY { get; set; }

        private double _checkPosScoreThresh = 0.6;
        /// <summary>
        /// 左矫正点匹配分数
        /// </summary>
        [BrowsableAttribute(false)]
        public double CheckPosScoreThresh
        {
            get { return _checkPosScoreThresh; }
            set { value = _checkPosScoreThresh; }
        }

        private double _checkPosRScoreThresh = 0.6;
        /// <summary>
        /// 右矫正点匹配分数
        /// </summary>
        [BrowsableAttribute(false)]
        public double CheckPosRScoreThresh
        {
            get { return _checkPosRScoreThresh; }
            set { value = _checkPosRScoreThresh; }
        }

        /// <summary>
        /// 大图谱工站产品参数
        /// </summary>
        [BrowsableAttribute(false)]
        public string AutoMappingStationProInf { get; set; }

        [BrowsableAttribute(false)]
        public JFXmlDictionary<string, object> Dict { get; internal set; }

        [BrowsableAttribute(false)]
        public JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string,string>>> visionCfgParams { get; internal set; }//Index / FovName / TaskName / LightCfgName

        [CategoryAttribute("料盒属性"), DisplayNameAttribute("①对应料盒"), TypeConverter(typeof(BoxManager))]
        public string MagezineBox
        {
            get
            {
                try
                {
                    return magezineBox = BoxManager.Dir_Boxes[mgzIdx].Name;
                }
                catch
                {
                    mgzIdx = -1;
                    magezineBox = "";
                    return magezineBox;
                }
            }
            set
            {
                foreach (var pair in BoxManager.Dir_Boxes)
                {
                    if (value == pair.Value.Name)
                    {
                        mgzIdx = pair.Key;
                        magezineBox = value;
                        return;
                    }
                }
                mgzIdx = -1;
                magezineBox = "";
            }
        }

        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("②料盒索引")]
        public Int32 MgzIdx
        {
            get
            {
                return mgzIdx;
            }
            set
            {
                mgzIdx = value;
            }
        }

        #region 料片属性
        [CategoryAttribute("料片属性"), DisplayNameAttribute("①料片长度（mm）")]
        public double FrameLength { get; set; }

        [CategoryAttribute("料片属性"), DisplayNameAttribute("②Block内行数目")]
        public Int32 RowNumber { get; set; }

        [CategoryAttribute("料片属性"), DisplayNameAttribute("③Block内列数目")]
        public Int32 ColumnNumber { get; set; }

        [CategoryAttribute("料片属性"), DisplayNameAttribute("④Block数目")]
        public Int32 BlockNumber { get; set; }

        [CategoryAttribute("料片属性"), DisplayNameAttribute("⑤使用打标器")]
        public Boolean UseMarker { get; set; }
        [CategoryAttribute("料片属性"), DisplayNameAttribute("⑥双芯片Block标记点x镜像")]
        public Boolean SymmetryMark { get; set; }
        [CategoryAttribute("料片属性"), DisplayNameAttribute("⑦打标点相对X（mm）"), DescriptionAttribute("打标位置相对芯片位置,偏左为负值,X方向")]
        public Double RelativeMark_X { get; set; }
        [CategoryAttribute("料片属性"), DisplayNameAttribute("⑧打标点相对Y（mm）"), DescriptionAttribute("打标位置相对芯片位置,偏下为负值,Y方向")]
        public Double RelativeMark_Y { get; set; }
        [ReadOnly(true), CategoryAttribute("料片属性"), DisplayNameAttribute("⑨打标高度Z（mm）"), DescriptionAttribute("打标位置高度,相机Z轴,Z方向")]
        public Double RelativeMark_Z { get; set; }

        [CategoryAttribute("产品位置信息"), DisplayNameAttribute("①Z轴聚焦高度（mm）")]
        public double ZFocus /*{ get; set; }*/
        {
            get { return Mark_Focus_z + _ref_zFocus; }
            set { _ref_zFocus = value - Mark_Focus_z; }
        }
        [CategoryAttribute("产品位置信息"), ReadOnly(true), DisplayNameAttribute("①Z轴聚焦偏移（mm）")]
        public double RefZFocus
        {
            get { return _ref_zFocus; }
            set { _ref_zFocus = value; }
        }
        [CategoryAttribute("产品位置信息"), DisplayNameAttribute("②二维码X坐标（mm）")]
        public double X_Code2D { get; set; }

        [CategoryAttribute("产品位置信息"), DisplayNameAttribute("③二维码Y坐标（mm）")]
        public double Y_Code2D { get; set; }
        #endregion

        #region 属性名称
        public string mMagezineBox = "①对应料盒";
        public string mMgzIdx = "②料盒索引";
        public string mFrameLength = "①料片长度";
        public string mRowNumber = "②Block内行数目";
        public string mColumnNumber = "③Block内列数目";
        public string mBlockNumber = "④Block数目";
        public string mUseMarker = "⑤使用打标器";
        public string mSymmetryMark = "⑥双芯片Block标记点x镜像";
        public string mRelativeMark_X = "⑦打标点相对X";
        public string mRelativeMark_Y = "⑧打标点相对Y";
        public string mRelativeMark_Z = "⑨打标高度Z";
        public string mZFocus = "①Z轴聚焦高度";
        public string mRefZFocus = "①Z轴聚焦偏移";
        public string mX_Code2D = "②二维码X坐标";
        public string mY_Code2D = "③二维码Y坐标";

        public string mMark1LightCfg = "Mark1光源配置名称";
        public string mMark2LightCfg = "Mark2光源配置名称";
        public string mVisionLightCfg = "视觉配置名称";

        public string mMark1SnapXName = "Mark1拍照X坐标";
        public string mMark1SnapYName = "Mark1拍照Y坐标";
        public string mMark2SnapXName = "Mark2拍照X坐标";
        public string mMark2SnapYName = "Mark2拍照Y坐标";

        public string mMark1RealWorldXName = "Mark1实际X坐标";
        public string mMark1RealWorldYName = "Mark1实际Y坐标";
        public string mMark2RealWorldXName = "Mark2实际X坐标";
        public string mMark2RealWorldYName = "Mark2实际Y坐标";

        public string mICSnapFovXName = "视野Region坐标X";
        public string mICSnapFovYName = "视野Region坐标Y";

        public string mDieRowCountInFov = "视野中Die的Row数目";
        public string mDieColCountInFov = "视野中Die的Col数目";

        public string mDieWidth = "Die宽度";
        public string mDieHeight = "Die高度";

        public string mAutomappingProInf = "大图谱工站产品参数";
        public string mWidthFactorName = "有效宽比例";
        public string mHeightFactorName = "有效高比例";
        public string mZoomFactorName = "降采样比例";
        private string mcheckPosX = "矫正点X";
        private string mcheckPosY = "矫正点Y";
        private string mcheckPosRX = "右矫正点X";
        private string mcheckPosRY = "右矫正点Y";
        private string mcheckPosScoreThresh = "左矫正点匹配分数";
        private string mcheckPosRScoreThresh = "右矫正点匹配分数";
        #endregion

        /// <summary>
        /// 属性赋值
        /// </summary>
        /// <returns></returns>
        public bool LoadParamsFromCfg()
        {
            MagezineBox = Dict.ContainsKey(mMagezineBox) ?(string)(GetItemValue(mMagezineBox)):"";
            MgzIdx = Dict.ContainsKey(mMgzIdx) ? (int)(GetItemValue(mMgzIdx)):-1;

            FrameLength = Dict.ContainsKey(mFrameLength) ? (double)(GetItemValue(mFrameLength)):0;
            RowNumber = Dict.ContainsKey(mRowNumber) ? (int)(GetItemValue(mRowNumber)):0;
            ColumnNumber = Dict.ContainsKey(mColumnNumber) ? (int)(GetItemValue(mColumnNumber)):0;
            BlockNumber = Dict.ContainsKey(mBlockNumber) ? (int)(GetItemValue(mBlockNumber)):0;
            UseMarker = Dict.ContainsKey(mUseMarker) ? (bool)(GetItemValue(mUseMarker)):false;
            SymmetryMark = Dict.ContainsKey(mSymmetryMark) ? (bool)(GetItemValue(mSymmetryMark)):false;

            RelativeMark_X = Dict.ContainsKey(mRelativeMark_X) ? (double)(GetItemValue(mRelativeMark_X)):0;
            RelativeMark_Y = Dict.ContainsKey(mRelativeMark_Y) ? (double)(GetItemValue(mRelativeMark_Y)):0;
            RelativeMark_Z = Dict.ContainsKey(mRelativeMark_Z) ? (double)(GetItemValue(mRelativeMark_Z)):0;

            ZFocus = Dict.ContainsKey(mZFocus) ? (double)(GetItemValue(mZFocus)):0;
            RefZFocus = Dict.ContainsKey(mRefZFocus) ? (double)(GetItemValue(mRefZFocus)):0;
            X_Code2D = Dict.ContainsKey(mX_Code2D) ? (double)(GetItemValue(mX_Code2D)):0;
            Y_Code2D = Dict.ContainsKey(mY_Code2D) ? (double)(GetItemValue(mY_Code2D)):0;

            Mark1LightCfg= Dict.ContainsKey(mMark1LightCfg) ? (string)(GetItemValue(mMark1LightCfg)):"";
            Mark2LightCfg = Dict.ContainsKey(mMark2LightCfg) ? (string)(GetItemValue(mMark2LightCfg)):"";

            Mark1SnapPosX = Dict.ContainsKey(mMark1SnapXName) ? (double)(GetItemValue(mMark1SnapXName)):0;
            Mark1SnapPosY = Dict.ContainsKey(mMark1SnapYName) ? (double)(GetItemValue(mMark1SnapYName)):0;

            Mark2SnapPosX = Dict.ContainsKey(mMark2SnapXName) ? (double)(GetItemValue(mMark2SnapXName)):0;
            Mark2SnapPosY = Dict.ContainsKey(mMark2SnapYName) ? (double)(GetItemValue(mMark2SnapYName)):0;

            Mark1RWPosX = Dict.ContainsKey(mMark1RealWorldXName) ? (double)(GetItemValue(mMark1RealWorldXName)):0;
            Mark1RWPosY = Dict.ContainsKey(mMark1RealWorldYName) ? (double)(GetItemValue(mMark1RealWorldYName)):0;

            Mark2RWPosX = Dict.ContainsKey(mMark2RealWorldXName) ? (double)(GetItemValue(mMark2RealWorldXName)):0;
            Mark2RWPosY = Dict.ContainsKey(mMark2RealWorldYName) ? (double)(GetItemValue(mMark2RealWorldYName)):0;

            ICFovOffsetX = Dict.ContainsKey(mICSnapFovXName) ? (List<double>)(GetItemValue(mICSnapFovXName)):new List<double>();
            ICFovOffsetY = Dict.ContainsKey(mICSnapFovYName) ? (List<double>)(GetItemValue(mICSnapFovYName)):new List<double>();

            DieRowInFov = Dict.ContainsKey(mDieRowCountInFov) ? (int)(GetItemValue(mDieRowCountInFov)) : 0;
            DieColInFov = Dict.ContainsKey(mDieColCountInFov) ? (int)(GetItemValue(mDieColCountInFov)) : 0;

            DieWidth = Dict.ContainsKey(mDieWidth) ? (double)(GetItemValue(mDieWidth)) : 0;
            DieHeight = Dict.ContainsKey(mDieHeight) ? (double)(GetItemValue(mDieHeight)) : 0;
            WidthFactor= Dict.ContainsKey(mWidthFactorName) ? (double)(GetItemValue(mWidthFactorName)) : 1;
            HeightFactor = Dict.ContainsKey(mHeightFactorName) ? (double)(GetItemValue(mHeightFactorName)) : 1;
            ScaleFactor = Dict.ContainsKey(mZoomFactorName) ? (double)(GetItemValue(mZoomFactorName)) : 0.125;

            CheckPosX = Dict.ContainsKey(mcheckPosX) ? (double)(GetItemValue(mcheckPosX)) : 0;
            CheckPosY = Dict.ContainsKey(mcheckPosY) ? (double)(GetItemValue(mcheckPosY)) : 0;
            CheckPosRX= Dict.ContainsKey(mcheckPosRX) ? (double)(GetItemValue(mcheckPosRX)) : 0;
            CheckPosRY = Dict.ContainsKey(mcheckPosRY) ? (double)(GetItemValue(mcheckPosRY)) : 0;
            CheckPosScoreThresh= Dict.ContainsKey(mcheckPosScoreThresh) ? (double)(GetItemValue(mcheckPosScoreThresh)) : 0.6;
            CheckPosRScoreThresh = Dict.ContainsKey(mcheckPosRScoreThresh) ? (double)(GetItemValue(mcheckPosRScoreThresh)) : 0.6;


            AutoMappingStationProInf = Dict.ContainsKey(mAutomappingProInf) ? (string)(GetItemValue(mAutomappingProInf)) : "";

            string xmlTxt = (string)GetItemValue(mVisionLightCfg);
            if(xmlTxt!="")
                visionCfgParams = JFFunctions.FromXTString(xmlTxt, visionCfgParams.GetType()) as JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string, string>>>;
            return true;
        }

        /// <summary>
        /// 属性写入到Dictionary中
        /// </summary>
        /// <returns></returns>
        public bool SaveParamsToCfg()
        {
            AddItem(mMagezineBox, MagezineBox);
            AddItem(mMgzIdx, MgzIdx);
            AddItem(mFrameLength, FrameLength);

            AddItem(mRowNumber, RowNumber);
            AddItem(mColumnNumber, ColumnNumber);
            AddItem(mBlockNumber, BlockNumber);

            AddItem(mUseMarker, UseMarker);
            AddItem(mSymmetryMark, SymmetryMark);

            AddItem(mRelativeMark_X, RelativeMark_X);
            AddItem(mRelativeMark_Y, RelativeMark_Y);
            AddItem(mRelativeMark_Z, RelativeMark_Z);

            AddItem(mZFocus, ZFocus);
            AddItem(mRefZFocus, RefZFocus);
            AddItem(mX_Code2D, X_Code2D);
            AddItem(mY_Code2D, Y_Code2D);

            AddItem(mMark1LightCfg, Mark1LightCfg);
            AddItem(mMark2LightCfg, Mark2LightCfg);

            AddItem(mMark1SnapXName, Mark1SnapPosX);
            AddItem(mMark1SnapYName, Mark1SnapPosY);
            AddItem(mMark2SnapXName, Mark2SnapPosX);
            AddItem(mMark2SnapYName, Mark2SnapPosY);

            AddItem(mMark1RealWorldXName, Mark1RWPosX);
            AddItem(mMark1RealWorldYName, Mark1RWPosY);
            AddItem(mMark2RealWorldXName, Mark2RWPosX);
            AddItem(mMark2RealWorldYName, Mark2RWPosY);

            AddItem(mDieRowCountInFov, DieRowInFov);
            AddItem(mDieColCountInFov, DieColInFov);

            AddItem(mDieWidth, DieWidth);
            AddItem(mDieHeight, DieHeight);

            AddItem(mICSnapFovXName, ICFovOffsetX);
            AddItem(mICSnapFovYName, ICFovOffsetY);
            AddItem(mWidthFactorName, WidthFactor);
            AddItem(mHeightFactorName, HeightFactor);
            AddItem(mZoomFactorName, ScaleFactor);

            AddItem(mcheckPosX, CheckPosX);
            AddItem(mcheckPosY, CheckPosY);
            AddItem(mcheckPosRX, CheckPosRX);
            AddItem(mcheckPosRY, CheckPosRY);
            AddItem(mcheckPosScoreThresh, CheckPosScoreThresh);
            AddItem(mcheckPosRScoreThresh, CheckPosRScoreThresh);

            AddItem(mAutomappingProInf, AutoMappingStationProInf);

            string xmlTxt = null;
            string typeTxt = null;
            JFFunctions.ToXTString(visionCfgParams, out xmlTxt, out typeTxt);

            AddItem(mVisionLightCfg, xmlTxt);
            return true;
        }

        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            Unsupported = -1,//设备不支持此功能
            ParamError = -2,//参数错误（不支持的参数）
            InvokeFailed = -3,//库函数调用出错//
        }

        public JFDLAFProductRecipe()
        {
            Dict = new JFXmlDictionary<string, object>();
            visionCfgParams = new JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string, string>>>();
            snapMapX = new HTuple();
            snapMapY = new HTuple();
            snapMapRow = new HTuple();
            snapMapCol = new HTuple();
            icMapX = new HTuple();
            icMapY = new HTuple();
            icMapRow = new HTuple();
            icMapCol = new HTuple();
            ICFovOffsetX = new List<double>();
            ICFovOffsetY = new List<double>();

            if(AllDieMatchRegion !=null)
                AllDieMatchRegion.Dispose();
            HOperatorSet.GenEmptyObj(out AllDieMatchRegion);
        }

        public JFDLAFProductRecipe Clone()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream) as JFDLAFProductRecipe;
        }

        /// <summary>
        /// IC扫描点总数目
        /// </summary>
        [BrowsableAttribute(false)]
        public int ICCount { get; private set;}

        /// <summary>
        /// IC扫描点行数目
        /// </summary>
        [BrowsableAttribute(false)]
        public int RowCount { get; private set; }

        /// <summary>
        /// IC扫描点列数目
        /// </summary>
        [BrowsableAttribute(false)]
        public int ColCount { get; private set; }

        /// <summary>
        /// Mark1点的光源配置参数
        /// </summary>
        [BrowsableAttribute(false)]
        public string Mark1LightCfg { get; set; }

        /// <summary>
        /// Mark2点的光源配置参数
        /// </summary>
        [BrowsableAttribute(false)]
        public string Mark2LightCfg { get; set; }

        /// <summary>
        /// 获取Mark1光源配置
        /// </summary>
        /// <returns></returns>
        public string GetMark1LightCfg()
        {
            return Mark1LightCfg;
        }

        /// <summary>
        /// 设置Mark1光源配置
        /// </summary>
        /// <param name="value"></param>
        public void SetMark1LightCfg(string value)
        {
            Mark1LightCfg = value;
            SaveParamsToCfg();
            JFHub.JFHubCenter.Instance.RecipeManager.Save();
        }

        /// <summary>
        /// 获取Mark2光源配置
        /// </summary>
        /// <returns></returns>
        public string GetMark2LightCfg()
        {
            return Mark2LightCfg;
        }

        /// <summary>
        /// 设置Mark2光源配置
        /// </summary>
        /// <param name="value"></param>
        public void SetMark2LightCfg(string value)
        {
            Mark2LightCfg = value;
            SaveParamsToCfg();
            JFHub.JFHubCenter.Instance.RecipeManager.Save();
        }

        /// <summary>
        /// 获取Mark1的Snap坐标信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GetMarkSnapPos1(out double x, out double y)
        {
            x = Mark1SnapPosX;
            y = Mark1SnapPosY;
        }

        /// <summary>
        /// 设置Mark1的Snap坐标信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetMarkSnapPos1(double x, double y)
        {
            Mark1SnapPosX = x;
            Mark1SnapPosY = y;
        }

        /// <summary>
        /// 获取Mark2的Snap坐标信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GetMarkSnapPos2(out double x, out double y)
        {
            x = Mark2SnapPosX;
            y = Mark2SnapPosY;
        }

        /// <summary>
        /// 设置Mark2的Snap坐标信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetMarkSnapPos2(double x, double y)
        {
            Mark2SnapPosX = x;
            Mark2SnapPosY = y;
        }

        /// <summary>
        /// 获取Mark1的实际坐标信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GetMarkRealWorldPos1(out double x, out double y)
        {
            x = Mark1RWPosX;
            y = Mark1RWPosY;
        }

        /// <summary>
        /// 设置Mark1的实际坐标信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetMarkRealWorldPos1(double x, double y)
        {
            Mark1RWPosX = x;
            Mark1RWPosY = y;
        }

        /// <summary>
        /// 设置Mark2的实际坐标信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetMarkRealWorldPos2(double x, double y)
        {
            Mark2RWPosX = x;
            Mark2RWPosY = y;
        }

        /// <summary>
        /// 获取IC拍照点的中心坐标信息
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GetICSnapCenter(double row, double col, out double x, out double y)
        {
            int index = 0;

            if(row%2==0)
            {
                index = (int)(row * ColCount + col);
            }
            else
            {
                index = (int)(row * ColCount + (ColCount-1-col));
            }
 
            GetICSnapCenter(index, out row, out col, out x, out y);
        }

        /// <summary>
        /// 获取IC拍照点的中心坐标信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GetICSnapCenter(int index, out double r, out double c,out double x, out double y)
        {
            r = snapMapRow.TupleSelect(index).D;
            c = snapMapCol.TupleSelect(index).D;
            x = snapMapX.TupleSelect(index).D;
            y = snapMapY.TupleSelect(index).D;
        }

        /// <summary>
        /// 获取IC的中心坐标信息
        /// </summary>
        /// <param name="row">芯片行数</param>
        /// <param name="col">芯片列数</param>
        /// <param name="x">芯片X坐标</param>
        /// <param name="y">芯片Y坐标</param>
        public void GetICCenter(double row, double col, out double x, out double y)
        {
            x = 0;
            y = 0;
            if ((icMapCol.TupleMax().I+1)!=ColumnNumber*BlockNumber || (icMapRow.TupleMax().I+1)!=RowNumber)
            {
                x = 0;
                y = 0;
                return;
            }
            int index = (int)(row * ColumnNumber * BlockNumber + col);
            x = icMapX.TupleSelect(index).D;
            y = icMapY.TupleSelect(index).D;
        }

        /// <summary>
        /// 根据拍照点的行列信息获取Die Match Region
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="dieMatchRegion"></param>
        /// <returns></returns>
        private bool GetDieMatchRegion(int row,int col,out HObject dieMatchRegion)
        {
            HOperatorSet.GenEmptyObj(out dieMatchRegion);
            int index = 0;

            if (row % 2 == 0)
            {
                index = (int)(row * ColCount + col);
            }
            else
            {
                index = (int)(row * ColCount + (ColCount - 1 - col));
            }

            if(index>= AllDieMatchRegion.CountObj())
            {
                return false;
            }

            HOperatorSet.SelectObj(AllDieMatchRegion, out dieMatchRegion, (HTuple)(index+1));
            return true;
        }

        [BrowsableAttribute(false)]
        public int FovCount { get; private set; }

        /// <summary>
        /// 获取FOV的相对IC中心的offset值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GetFovOffset(string fovName, out double x, out double y)
        {
            int index = 0;
            string[] FovNmaes = FovNames();
            for (int i=0;i<FovCount;i++)
            {
                if(FovNmaes[i]== fovName)
                {
                    index = i;
                    break;
                }
            }
            x = ICFovOffsetX[index];
            y = ICFovOffsetY[index];
        }

        /// <summary>
        /// 获取大图谱图片绝对路径
        /// </summary>
        /// <param name="recipeID"></param>
        /// <returns></returns>
        public string GetFrameMapImgFullPath(string recipeID)
        {
            return (string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue((string)JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2])
                + "\\" + recipeID + "\\frameMapImg.tiff";
        }

        /// <summary>
        /// 加载Recipe中IC的扫描点位
        /// </summary>
        /// <param name="errInf"></param>
        /// <returns></returns>
        public int LoadRecipeScanPositonFile(out string errInf)
        {
            string RecipeFolderPath = (string)JFHub.JFHubCenter.Instance.RecipeManager.GetInitParamValue(JFHub.JFHubCenter.Instance.RecipeManager.InitParamNames[2]);
            errInf = "Success";
            try
            {
                //值初始化
                ICCount = 0;
                RowCount = 0;
                ColCount = 0;
                FovCount = 0;

                //加载IC扫描点文件
                if (!File.Exists(RecipeFolderPath + "\\" + ID + "\\snapMapX.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + ID + "\\snapMapX.dat" + "不存在";
                    //MessageBox.Show(errInf);
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + ID + "\\snapMapY.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + ID + "\\snapMapY.dat" + "不存在";
                    //MessageBox.Show(errInf);
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + ID + "\\snapMapRow.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + ID + "\\snapMapRow.dat" + "不存在";
                    //MessageBox.Show(errInf);
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + ID + "\\snapMapCol.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + ID + "\\snapMapCol.dat" + "不存在";
                    //MessageBox.Show(errInf);
                    return (int)ErrorDef.InvokeFailed;
                }
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + ID + "\\snapMapX.dat", out snapMapX);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + ID + "\\snapMapY.dat", out snapMapY);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + ID + "\\snapMapRow.dat", out snapMapRow);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + ID + "\\snapMapCol.dat", out snapMapCol);

                if (snapMapX.Length != snapMapY.Length || snapMapRow.Length != snapMapCol.Length || snapMapX.Length != snapMapCol.Length)
                {
                    errInf = "文件snapMapX.dat、snapMapY.dat、snapMapRow.dat、snapMapCol.dat中点位数目不匹配";
                    //MessageBox.Show(errInf);
                    return (int)ErrorDef.InvokeFailed;
                }
                ICCount = snapMapX.Length;
                RowCount = snapMapRow.TupleMax().I + 1;
                ColCount = snapMapCol.TupleMax().I + 1;
                FovCount = ICFovOffsetX.Count;

                //加载IC扫描点文件
                if (!File.Exists(RecipeFolderPath + "\\" + ID + "\\clipMapX.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + ID + "\\clipMapX.dat" + "不存在";
                    //MessageBox.Show(errInf);
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + ID + "\\clipMapY.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + ID + "\\clipMapY.dat" + "不存在";
                    //MessageBox.Show(errInf);
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + ID + "\\clipMapRow.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + ID + "\\clipMapRow.dat" + "不存在";
                    //MessageBox.Show(errInf);
                    return (int)ErrorDef.InvokeFailed;
                }
                if (!File.Exists(RecipeFolderPath + "\\" + ID + "\\clipMapCol.dat"))
                {
                    errInf = "文件" + RecipeFolderPath + "\\" + ID + "\\clipMapCol.dat" + "不存在";
                    //MessageBox.Show(errInf);
                    return (int)ErrorDef.InvokeFailed;
                }
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + ID + "\\clipMapX.dat", out icMapX);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + ID + "\\clipMapY.dat", out icMapY);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + ID + "\\clipMapRow.dat", out icMapRow);
                HOperatorSet.ReadTuple(RecipeFolderPath + "\\" + ID + "\\clipMapCol.dat", out icMapCol);

                ////加载DieMatchRegion文件
                //if (!File.Exists(RecipeFolderPath + "\\" + ID + "\\dieMatchRegion.reg"))
                //{
                //    errInf = "文件" + RecipeFolderPath + "\\" + ID + "\\dieMatchRegion.reg" + "不存在";
                //    //MessageBox.Show(errInf);
                //    return (int)ErrorDef.InvokeFailed;
                //}
                //HOperatorSet.ReadRegion(out AllDieMatchRegion, RecipeFolderPath + "\\" + ID + "\\dieMatchRegion.reg");

                return (int)ErrorDef.Success;
            }
            catch(Exception ex)
            {
                errInf = ex.ToString();
                MessageBox.Show(errInf);
                return (int)ErrorDef.InvokeFailed;
            }
        }

        public string[] AllItemNames()
        {
            return null;
        }

        public Type ItemType(string itemName)
        {
            return null;
        }

        private void AddItem(string itemName, object value)
        {
            if (Dict.ContainsKey(itemName))
            {
                Dict[itemName] = value;
            }
            else
            {
                Dict.Add(itemName, value);
            }
        }

        public object GetItemValue(string itemName)
        {
            return Dict[itemName];
        }

        public void SetItemValue(string itemName, object itemValue)
        {
            
        }

        #region Fov/Task
        public string[] FovNames()
        {
            List<string> fovnameList = new List<string>();
            if (visionCfgParams != null)
            {
                int fovcount = visionCfgParams.Keys.Count;
                for(int i=0;i<fovcount;i++)
                {
                    foreach (string fovname in visionCfgParams[i.ToString()].Keys)
                    {
                        fovnameList.Add(fovname);
                    }
                }
            }
            return fovnameList.ToArray();
        }

        public string GetFovTempleteName(string fovName)
        {
            return null;
        }

        public bool SetFovTempleteName(string fovName,string templeteName)
        {
            return false;
        }

        public string[] TaskNames(string fovName)
        {
            List<string> tasknameList = new List<string>();
            foreach (string index in visionCfgParams.Keys)
            {
                foreach (string _fovname in visionCfgParams[index].Keys)
                {
                    if(_fovname==fovName)
                    {
                        foreach (string taskname in visionCfgParams[index][_fovname].Keys)
                        {
                            tasknameList.Add(taskname);
                        }
                        break;
                    }
                }
            }
            return tasknameList.ToArray();
        }

        /// <summary>
        /// 获取视觉光源名称
        /// </summary>
        /// <param name="fovName"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public string VisionCfgName(string fovName,string taskName)
        {
            foreach (string index in visionCfgParams.Keys)
            {
                JFXmlDictionary<string, JFXmlDictionary<string, string>> dicFovVisonName = visionCfgParams[index];
                if (dicFovVisonName.ContainsKey(fovName))
                {
                    JFXmlDictionary<string, string> keyValuePairs = dicFovVisonName[fovName];
                    if (keyValuePairs.ContainsKey(taskName))
                    {
                        return keyValuePairs[taskName];
                    }
                    else
                        return "";
                }
            }
            return "";
        }

        /// <summary>
        /// 设置视觉光源名称
        /// </summary>
        /// <param name="fovName"></param>
        /// <param name="taskName"></param>
        /// <param name="visionCfgName"></param>
        /// <returns></returns>
        public bool SetVisionCfgName(string fovName, string taskName, string visionCfgName)
        {
            string fovIndex = "";
            foreach (string index in visionCfgParams.Keys)
            {
                JFXmlDictionary<string, JFXmlDictionary<string, string>> dicFovVisonName = visionCfgParams[index];
                if (dicFovVisonName.ContainsKey(fovName))
                {
                    fovIndex = index;
                    break;
                }
            }

            if (fovIndex != "")
            {
                JFXmlDictionary<string, JFXmlDictionary<string, string>> dicFovVisonName = visionCfgParams[fovIndex];
                if (dicFovVisonName.ContainsKey(fovName))
                {
                    JFXmlDictionary<string, string> keyValuePairs = dicFovVisonName[fovName];
                    if (keyValuePairs.ContainsKey(taskName))
                    {
                        keyValuePairs[taskName] = visionCfgName;
                    }
                    else
                    {
                        keyValuePairs.Add(taskName, visionCfgName);
                    }
                }
                else
                {
                    visionCfgParams.Remove(fovIndex);
                    dicFovVisonName = new JFXmlDictionary<string, JFXmlDictionary<string, string>>();
                    JFXmlDictionary<string, string> keyValuePairs = new JFXmlDictionary<string, string>();
                    keyValuePairs.Add(taskName, visionCfgName);
                    dicFovVisonName.Add(fovName, keyValuePairs);
                    visionCfgParams.Add(fovIndex, dicFovVisonName);
                }
            }
            else
            {
                fovIndex = visionCfgParams.Keys.Count.ToString();
                JFXmlDictionary<string, JFXmlDictionary<string, string>> dicFovVisonName = new JFXmlDictionary<string, JFXmlDictionary<string, string>>();
                JFXmlDictionary<string, string> keyValuePairs = new JFXmlDictionary<string, string>();
                keyValuePairs.Add(taskName, visionCfgName);
                dicFovVisonName.Add(fovName, keyValuePairs);
                visionCfgParams.Add(fovIndex, dicFovVisonName);
            }

            SaveParamsToCfg();
            JFHub.JFHubCenter.Instance.RecipeManager.Save();
            return true;
        }

        #endregion

    }

    [Serializable]
    public class JFDLAFBoxRecipe : IJFRecipe
    {
        private Double _heightLastSlot_y = 0;
        private Double _heightLastSlot = 0;
        private Double _heightFirstSlot = 0;
        private Double _heightFirstSlot_y = 0;
        private Double _heightLastSlot_Unload = 0;
        private Double _heightLastSlot_Unload_y = 0;
        private Double _heightFirstSlot_Unload = 0;
        private Double _heightFirstSlot_Unload_y = 0;
        private Double z_LoadUnLoadFramePos = 0;
        private int _slotNumber = 0;
        private int _blankNumber_Unload = 0;
        private Double _frameWidth = 0;
        private Double x_PushRodOverPos = 0;
        private Double x_ChuckLoadFramePos = 0;
        private Double x_PushRodWaitPos = 0;
        private Double y_LoadUnLoadFramePos = 0;

        /// <summary>
        /// 产品/配方 类别 比如：托盘 产品 等
        /// </summary>
        public string Categoty { get; internal set; }

        /// <summary>
        /// 产品/配方 名称 
        /// </summary>
        public string ID { get; internal set; }

        public JFXmlDictionary<string, object> Dict { get; internal set; }

        #region 料盒属性
        [CategoryAttribute("料盒属性"), DisplayNameAttribute("①对应料盒")]
        public string MagezineBox { get; set; }

        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("②料盒索引")]
        public Int32 MgzIdx { get; set; }

        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("③上料最后槽上料位Z坐标（mm）")]
        public Double HeightLastSlot
        {
            get
            {
                try
                {
                    return (MgzIdx != -1 ? (_heightLastSlot = BoxManager.Dir_Boxes[MgzIdx].HeightLastSlot) : _heightLastSlot);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _heightLastSlot = value;
            }
        }


        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("③上料最后槽上料位Y坐标（mm）")]
        public Double HeightLastSlot_y
        {
            get
            {
                try
                {
                    return (MgzIdx != -1 ? (_heightLastSlot_y = BoxManager.Dir_Boxes[MgzIdx].HeightLastSlot_y) : _heightLastSlot_y);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _heightLastSlot_y = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("④上料第一槽上料位Z坐标（mm）")]
        public Double HeightFirstSlot
        {
            get
            {
                try
                {
                    if (MgzIdx != -1) z_LoadUnLoadFramePos = BoxManager.Dir_Boxes[MgzIdx].HeightFirstSlot;
                    return (MgzIdx != -1 ? (_heightFirstSlot = BoxManager.Dir_Boxes[MgzIdx].HeightFirstSlot) : _heightFirstSlot);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _heightFirstSlot = value;
                z_LoadUnLoadFramePos = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("④上料第一槽上料位Y坐标（mm）")]
        public Double HeightFirstSlot_y
        {
            get
            {
                try
                {
                    if (MgzIdx != -1) y_LoadUnLoadFramePos = BoxManager.Dir_Boxes[MgzIdx].HeightFirstSlot_y;
                    return (MgzIdx != -1 ? (_heightFirstSlot_y = BoxManager.Dir_Boxes[MgzIdx].HeightFirstSlot_y) : _heightFirstSlot_y);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _heightFirstSlot_y = value;
                y_LoadUnLoadFramePos = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑤下料最后槽上料位Z坐标（mm）")]
        public Double HeightLastSlot_Unload
        {
            get
            {
                try
                {
                    return (MgzIdx != -1 ? (_heightLastSlot_Unload = BoxManager.Dir_Boxes[MgzIdx].HeightLastSlot_Unload) : _heightLastSlot_Unload);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _heightLastSlot_Unload = value;
            }
        }

        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑤下料最后槽上料位Y坐标（mm）")]
        public Double HeightLastSlot_Unload_y
        {
            get
            {
                try
                {
                    return (MgzIdx != -1 ? (_heightLastSlot_Unload_y = BoxManager.Dir_Boxes[MgzIdx].HeightLastSlot_Unload_y) : _heightLastSlot_Unload_y);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _heightLastSlot_Unload_y = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑥下料第一槽上料位Z坐标（mm）")]
        public Double HeightFirstSlot_Unload
        {
            get
            {
                try
                {
                    if (MgzIdx != -1) z_LoadUnLoadFramePos = BoxManager.Dir_Boxes[MgzIdx].HeightFirstSlot_Unload;
                    return (MgzIdx != -1 ? (_heightLastSlot_Unload = BoxManager.Dir_Boxes[MgzIdx].HeightFirstSlot_Unload) : _heightFirstSlot_Unload);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _heightLastSlot_Unload = value;
                z_LoadUnLoadFramePos = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑥下料第一槽上料位Y坐标（mm）")]
        public Double HeightFirstSlot_Unload_y
        {
            get
            {
                try
                {
                    if (MgzIdx != -1) y_LoadUnLoadFramePos = BoxManager.Dir_Boxes[MgzIdx].HeightFirstSlot_Unload_y;
                    return (MgzIdx != -1 ? (_heightLastSlot_Unload_y = BoxManager.Dir_Boxes[MgzIdx].HeightFirstSlot_Unload_y) : _heightFirstSlot_Unload_y);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _heightLastSlot_Unload_y = value;
                y_LoadUnLoadFramePos = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑦料盒内槽数目")]
        public int SlotNumber
        {
            get
            {
                try
                {
                    return (MgzIdx != -1 ? (_slotNumber = BoxManager.Dir_Boxes[MgzIdx].SlotNumber) : _slotNumber);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _slotNumber = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑧顺序下料间隔槽数目"), DescriptionAttribute("顺序下料间隔槽数目，同层时无效")]
        public int BlankNumber_Unload
        {

            get
            {
                try
                {
                    return (MgzIdx != -1 ? (_blankNumber_Unload = BoxManager.Dir_Boxes[MgzIdx].BlankNumber_Unload) : _blankNumber_Unload);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _blankNumber_Unload = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑨导轨Y轴位（mm）")]
        public Double FrameWidth
        {
            get { return (MgzIdx != -1 ? (_frameWidth = BoxManager.Dir_Boxes[MgzIdx].FrameWidth) : _frameWidth); }
            set { _frameWidth = value; }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑩上料Y轴上片位(mm)"), DescriptionAttribute("上料料盒搬运机构Y轴,Y方向")]
        public Double Load_y_LoadUnLoadFramePos
        {
            get
            {
                return (MgzIdx != -1 ? (y_LoadUnLoadFramePos = BoxManager.Dir_Boxes[MgzIdx].y_LoadFramePos) : y_LoadUnLoadFramePos);
            }
            set
            {
                y_LoadUnLoadFramePos = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑫下料Y轴下片位(mm)"), DescriptionAttribute("下料料盒搬运机构Z轴,Y方向")]
        public Double UnLoad_y_LoadUnLoadFramePos
        {
            get
            {
                return (MgzIdx != -1 ? (y_LoadUnLoadFramePos = BoxManager.Dir_Boxes[MgzIdx].y_UnloadFramePos) : y_LoadUnLoadFramePos);
            }
            set
            {
                y_LoadUnLoadFramePos = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑭上料推杆等待位(mm)"), DescriptionAttribute("上料推杆位置,X方向")]
        public Double Load_x_PushRodWaitPos
        {
            get { return (MgzIdx != -1 ? (x_PushRodWaitPos = BoxManager.Dir_Boxes[MgzIdx].x_LoadPushRodWaitPos) : x_PushRodWaitPos); }
            set
            {
                x_PushRodWaitPos = value;
            }
        }
        [ReadOnly(true), CategoryAttribute("料盒属性"), DisplayNameAttribute("⑮上料推杆结束位(mm)"), DescriptionAttribute("上料推杆位置,X方向")]
        public Double Load_x_PushRodOverPos
        {
            get { return (MgzIdx != -1 ? (x_PushRodOverPos = BoxManager.Dir_Boxes[MgzIdx].x_LoadPushRodOverPos) : x_PushRodOverPos); }
            set
            {
                x_PushRodOverPos = value;
            }
        }

        [ReadOnly(true), CategoryAttribute("流程属性"), DisplayNameAttribute("①产品检测位(mm)"), DescriptionAttribute("载台左侧上料夹爪的位置, X方向")]
        public Double Load_x_ChuckLoadFramePos
        {
            get { return (MgzIdx != -1 ? (x_ChuckLoadFramePos = BoxManager.Dir_Boxes[MgzIdx].x_ChuckLoadFramePos) : x_ChuckLoadFramePos); }
            set
            {
                x_ChuckLoadFramePos = value;
            }
        }

        [BrowsableAttribute(false)]
        public double XFirstDie { get; set; }

        [BrowsableAttribute(false)]
        public double YFirstDie { get; set; }

        [BrowsableAttribute(false)]
        public int EquivalentRowNumber { get; set; }

        [BrowsableAttribute(false)]
        public int EquivalentColumnNumbe { get; set; }

        [BrowsableAttribute(false)]
        public Double DeltaColumnBlock { get; set; }
        #endregion

        #region 属性名称
        private string mMagezineBox = "①对应料盒";
        private string mMgzIdx = "②料盒索引";
        private string mHeightLastSlot = "③上料最后槽上料位Z坐标";
        private string mHeightLastSlot_y = "③上料最后槽上料位Y坐标";
        private string mHeightFirstSlot = "④上料第一槽上料位Z坐标";
        private string mHeightFirstSlot_y = "④上料第一槽上料位Y坐标";
        private string mHeightLastSlot_Unload = "⑤下料最后槽上料位Z坐标";
        private string mHeightLastSlot_Unload_y = "⑤下料最后槽上料位Y坐标";
        private string mHeightFirstSlot_Unload = "⑥下料第一槽上料位Z坐标";
        private string mHeightFirstSlot_Unload_y = "⑥下料第一槽上料位Z坐标";
        private string mSlotNumber = "⑦料盒内槽数目";
        private string mBlankNumber_Unload = "⑧顺序下料间隔槽数目";
        private string mFrameWidth = "⑨导轨Y轴位";
        private string mLoad_y_LoadUnLoadFramePos = "⑩上料Y轴上片位";
        private string mUnLoad_y_LoadUnLoadFramePos = "⑫下料Y轴下片位";
        private string mLoad_x_PushRodWaitPos = "⑭上料推杆等待位";
        private string mLoad_x_PushRodOverPos = "⑮上料推杆结束位";
        private string mLoad_x_ChuckLoadFramePos = "①产品检测位";
        #endregion

        public bool LoadParamsFromCfg()
        {
            MagezineBox = (string)(GetItemValue(mMagezineBox));
            MgzIdx = (int)(GetItemValue(mMgzIdx));
            HeightLastSlot = (double)(GetItemValue(mHeightLastSlot));
            HeightLastSlot_y = (double)(GetItemValue(mHeightLastSlot_y));
            HeightFirstSlot = (double)(GetItemValue(mHeightFirstSlot));
            HeightFirstSlot_y = (double)(GetItemValue(mHeightFirstSlot_y));
            HeightLastSlot_Unload = (double)(GetItemValue(mHeightLastSlot_Unload));
            HeightLastSlot_Unload_y = (double)(GetItemValue(mHeightLastSlot_Unload_y));
            HeightFirstSlot_Unload = (double)(GetItemValue(mHeightFirstSlot_Unload));
            HeightFirstSlot_Unload_y = (double)(GetItemValue(mHeightFirstSlot_Unload_y));
            SlotNumber = (int)(GetItemValue(mSlotNumber));
            BlankNumber_Unload = (int)(GetItemValue(mBlankNumber_Unload));
            FrameWidth = (double)(GetItemValue(mFrameWidth));
            Load_y_LoadUnLoadFramePos = (double)(GetItemValue(mLoad_y_LoadUnLoadFramePos));
            UnLoad_y_LoadUnLoadFramePos = (double)(GetItemValue(mUnLoad_y_LoadUnLoadFramePos));
            Load_x_PushRodWaitPos = (double)(GetItemValue(mLoad_x_PushRodWaitPos));
            Load_x_PushRodOverPos = (double)(GetItemValue(mLoad_x_PushRodOverPos));
            Load_x_ChuckLoadFramePos = (double)(GetItemValue(mLoad_x_ChuckLoadFramePos));

            return true;
        }

        public bool SaveParamsToCfg()
        {
            AddItem(mMagezineBox, MagezineBox);
            AddItem(mMgzIdx, MgzIdx);
            AddItem(mHeightLastSlot, HeightLastSlot);
            AddItem(mHeightLastSlot_y, HeightLastSlot_y);
            AddItem(mHeightFirstSlot, HeightFirstSlot);
            AddItem(mHeightFirstSlot_y, HeightFirstSlot_y);
            AddItem(mHeightLastSlot_Unload, HeightLastSlot_Unload);
            AddItem(mHeightLastSlot_Unload_y, HeightLastSlot_Unload_y);
            AddItem(mHeightFirstSlot_Unload, HeightFirstSlot_Unload);
            AddItem(mHeightFirstSlot_Unload_y, HeightFirstSlot_Unload_y);
            AddItem(mSlotNumber, SlotNumber);
            AddItem(mBlankNumber_Unload, BlankNumber_Unload);
            AddItem(mFrameWidth, FrameWidth);
            AddItem(mLoad_y_LoadUnLoadFramePos, Load_y_LoadUnLoadFramePos);
            AddItem(mUnLoad_y_LoadUnLoadFramePos, UnLoad_y_LoadUnLoadFramePos);
            AddItem(mLoad_x_PushRodWaitPos, Load_x_PushRodWaitPos);
            AddItem(mLoad_x_PushRodOverPos, Load_x_PushRodOverPos);
            AddItem(mLoad_x_ChuckLoadFramePos, Load_x_ChuckLoadFramePos);

            return true;
        }

        public JFDLAFBoxRecipe()
        {
            Dict = new JFXmlDictionary<string, object>();
        }

        public JFDLAFBoxRecipe Clone()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream) as JFDLAFBoxRecipe;
        }
        public string[] AllItemNames()
        {
            return null;
        }

        public Type ItemType(string itemName)
        {
            return null;
        }

        private void AddItem(string itemName, object value)
        {
            if (Dict.ContainsKey(itemName))
            {
                Dict[itemName] = value;
            }
            else
            {
                Dict.Add(itemName, value);
            }
        }

        public object GetItemValue(string itemName)
        {
            return Dict[itemName];
        }

        public void SetItemValue(string itemName, object itemValue)
        {
        }
    }


    /// <summary>
    /// 配方管理类
    /// </summary>
    [JFDisplayName("DLAF配方管理器")]
    [JFVersion("1.0.0.0")]
    public class JFDLAFRecipeManager:IJFRecipeManager,IJFRealtimeUIProvider
    {
        public JFDLAFRecipeManager()
        {
            if(!JFHub.JFHubCenter.Instance.SystemCfg.ContainsItem("CurrentID"))
            {
                JFHub.JFHubCenter.Instance.SystemCfg.AddItem("CurrentID", "");
                JFHub.JFHubCenter.Instance.SystemCfg.Save();
            }
        }

        JFXCfg _cfg = new JFXCfg(); //用于读取/保存配置的对象
        Dictionary<string, Dictionary<string, IJFRecipe>> _dctRecipes = new Dictionary<string, Dictionary<string, IJFRecipe>>();

        #region IJFInitializable's API
        static string[] _initParamNames = new string[] { "配置文件路径", "文件不存在时" ,"配方保存路径"};
        string[] _initParamValues = new string[] { "", "", ""};
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public string[] InitParamNames { get { return _initParamNames; } }


        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == _initParamNames[0])
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.FilePath, null);
            else if (name == _initParamNames[1])
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.Options, new string[] { "新创建", "报错" });
            else if (name == _initParamNames[2])
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.FolderPath, null);

            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public object GetInitParamValue(string name)
        {
            if (name == _initParamNames[0])
                return _initParamValues[0];
            else if (name == _initParamNames[1])
                return _initParamValues[1];
            else if (name == _initParamNames[2])
                return _initParamValues[2];
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool SetInitParamValue(string name, object value)
        {
            if (name == _initParamNames[0])
            {
                _initParamValues[0] = Convert.ToString(value);
                return true;
            }
            else if (name == _initParamNames[1])
            {
                string sv = Convert.ToString(value);
                if (sv == "新创建" || sv == "报错")
                {
                    _initParamValues[1] = sv;
                    return true;
                }
                return false;
            }
            else if (name == _initParamNames[2])
            {
                _initParamValues[2] = Convert.ToString(value);
                return true;
            }
            throw new ArgumentOutOfRangeException();
        }


        bool _isInitOK = false;
        string _initErrorInfo = "None-Opt";
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool Initialize()
        {
            _isInitOK = false;
            _initErrorInfo = "Unknown-Error";
            do
            {
                if (string.IsNullOrEmpty(_initParamValues[0]))
                {
                    _initErrorInfo = _initParamNames[0] + " 未设置/空值";
                    break;
                }

                if (string.IsNullOrEmpty(_initParamValues[1]))
                {
                    _initErrorInfo = _initParamNames[1] + " 未设置/空值";
                    break;
                }

                if (string.IsNullOrEmpty(_initParamValues[2]))
                {
                    _initErrorInfo = _initParamNames[2] + " 未设置/空值";
                    break;
                }

                bool isCreateWhenFileNotExist = false;
                if (_initParamValues[1] == "新创建")
                    isCreateWhenFileNotExist = true;
                else if (_initParamValues[1] == " 报错")
                    isCreateWhenFileNotExist = false;
                else
                {
                    _initErrorInfo = _initParamNames[1] + " 参数错误,Value = " + _initParamValues[1] + "不存在于可选值列表[\"新创建\",\"报错\"]";
                    break;
                }

                if (!File.Exists(_initParamValues[0]))
                {
                    if (!isCreateWhenFileNotExist)
                    {
                        _initErrorInfo = _initParamNames[0] + " = \"" + _initParamValues[0] + "\"文件不存在";
                        break;
                    }
                }

                try
                {
                    //加载料盒配置文件
                    if(!BoxManager.Instance.Load())
                    {
                        _initErrorInfo = "加载料盒配置文件出错";
                        break;
                    }

                    _cfg.Load(_initParamValues[0], isCreateWhenFileNotExist);


                    if (!_cfg.ContainsItem("Categoties"))///保存所有的产品类别()
                        _cfg.AddItem("Categoties", new List<string>());

                    if (!_cfg.ContainsItem("Cate-Recipes")) ///
                        _cfg.AddItem("Cate-Recipes", new JFXmlDictionary<string, List<string[]>>());
                    //.............................................  类别->Recipe[ID, innerTxt]      
                    string errInfo;
                    if (!_load(out errInfo))
                    {
                        _initErrorInfo = "加载配置文件出错:" + errInfo;
                        break;
                    }

                    JFXmlDictionary<string, List<string[]>> dctCateRecipes = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;


                }
                catch (Exception ex)
                {
                    _initErrorInfo = "加载配置文件发生异常:" + ex.Message;
                    break;
                }


                _isInitOK = true;
                _initErrorInfo = "Success";
            } while (false);
            return _isInitOK;
        }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get { return _isInitOK; } }

        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo()
        {
            return _initErrorInfo;
        }
        #endregion

        /// <summary>
        /// 将配置数据转化为内存对象
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        bool _load(out string errorInfo)
        {
            _dctRecipes.Clear();
            JFXmlDictionary<string, List<string[]>> cateRecipeInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
            foreach (KeyValuePair<string, List<string[]>> cr in cateRecipeInCfg)
            {
                if (!_dctRecipes.ContainsKey(cr.Key))
                    _dctRecipes.Add(cr.Key, new Dictionary<string, IJFRecipe>());
                Dictionary<string, IJFRecipe> dctInCate = _dctRecipes[cr.Key];
                foreach (string[] idAndTxt in cr.Value)
                {
                    if (cr.Key == "Product")
                    {
                        JFDLAFProductRecipe recipe = new JFDLAFProductRecipe();
                        recipe.Categoty = cr.Key;
                        recipe.ID = idAndTxt[0];
                        try
                        {
                            recipe.Dict = JFFunctions.FromXTString(idAndTxt[1], recipe.Dict.GetType()) as JFXmlDictionary<string, object>;
                            recipe.LoadParamsFromCfg();
                        }
                        catch (Exception ex)
                        {
                            errorInfo = "Categoty = " + cr.Key + ", RecipeID = " + idAndTxt[0] + " FromXTString() Exception:" + ex.Message;
                            return false;
                        }
                        dctInCate.Add(idAndTxt[0], recipe as IJFRecipe);
                    }
                    else
                    {

                        JFDLAFBoxRecipe recipe = new JFDLAFBoxRecipe();
                        recipe.Categoty = cr.Key;
                        recipe.ID = idAndTxt[0];
                        try
                        {
                            recipe.Dict = JFFunctions.FromXTString(idAndTxt[1], recipe.Dict.GetType()) as JFXmlDictionary<string, object>;
                            recipe.LoadParamsFromCfg();
                        }
                        catch (Exception ex)
                        {
                            errorInfo = "Categoty = " + cr.Key + ", RecipeID = " + idAndTxt[0] + " FromXTString() Exception:" + ex.Message;
                            return false;
                        }
                        dctInCate.Add(idAndTxt[0], recipe as IJFRecipe);
                        
                    }
                }

            }
            errorInfo = "Success";
            return true;

        }

        #region   IJFRecipeManager's API
        /// <summary>
        /// 载入(/重新载入)所有产品,配方
        /// </summary>
        public bool Load()
        {
            if (!IsInitOK)
                return false;

            _cfg.Load();
            string error;
            if (!_load(out error))
                return false;
            return true;

        }

        public void Save()
        {
            if (!IsInitOK)
                return;
            JFXmlDictionary<string, List<string[]>> dctRecipesInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;

            foreach (string categoty in _dctRecipes.Keys)
            {
                Dictionary<string, IJFRecipe> dctMmry = _dctRecipes[categoty]; //所有recipe内存对象
                List<string[]> lstInCfg = dctRecipesInCfg[categoty];
                foreach (KeyValuePair<string, IJFRecipe> kv in dctMmry)
                {
                    foreach (string[] idAndTxt in lstInCfg)
                    {
                        if (idAndTxt[0] == kv.Key)
                        {
                            if (categoty == "Product")
                            {
                                string xmlTxt = null;
                                string typeTxt = null;
                                JFDLAFProductRecipe jFDLAFProductRecipe = new JFDLAFProductRecipe();
                                jFDLAFProductRecipe = kv.Value as JFDLAFProductRecipe;
                                jFDLAFProductRecipe.SaveParamsToCfg();
                                JFFunctions.ToXTString(jFDLAFProductRecipe.Dict, out xmlTxt, out typeTxt);
                                idAndTxt[1] = xmlTxt;
                            }
                            else
                            {
                                string xmlTxt = null;
                                string typeTxt = null;
                                JFDLAFBoxRecipe jFDLAFProductRecipe = new JFDLAFBoxRecipe();
                                jFDLAFProductRecipe = kv.Value as JFDLAFBoxRecipe;
                                jFDLAFProductRecipe.SaveParamsToCfg();
                                JFFunctions.ToXTString(jFDLAFProductRecipe.Dict, out xmlTxt, out typeTxt);
                                idAndTxt[1] = xmlTxt;
                            }
                            break;
                        }
                    }
                }

            }
            _cfg.Save();
        }

        /// <summary>
        /// 获取所有产品/配方 类别
        /// </summary>
        /// <returns></returns>
        public string[] AllCategoties()
        {
            if (!IsInitOK)
                return null;


            return (_cfg.GetItemValue("Categoties") as List<string>).ToArray();

        }

        ///// <summary>
        ///// 添加一个产品类别
        ///// </summary>
        ///// <param name="recipeCategoty"></param>
        //void AddRecipeCategoty(string categoty);

        /// <summary>
        /// 移除一个类别(下的所有RecipeID)
        /// </summary>
        /// <param name="recipeCategoty"></param>
        public void RemoveCategoty(string categoty)
        {
            if (!_cfg.ContainsItem("Categoties"))
                return;
            //移除配置参数
            List<string> categoties = _cfg.GetItemValue("Categoties") as List<string>;
            if (!categoties.Contains(categoty))
                return;
            categoties.Remove(categoty);
            JFXmlDictionary<string, List<string[]>> recipesInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
            if (recipesInCfg.ContainsKey(categoty))
                recipesInCfg.Remove(categoty);

            //移除内存对象
            if (_dctRecipes.ContainsKey(categoty))
                _dctRecipes.Remove(categoty);


        }



        /// <summary>
        /// 获取指定类别下的所有产品/配方 ID
        /// </summary>
        /// <returns></returns>
        public string[] AllRecipeIDsInCategoty(string categoty)
        {
            JFXmlDictionary<string, List<string[]>> cateRecipes = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
            if (!cateRecipes.ContainsKey(categoty))
                return null;
            List<string[]> recipes = cateRecipes[categoty];
            List<string> ret = new List<string>();
            foreach (string[] sa in recipes)
                ret.Add(sa[0]);
            return ret.ToArray();
        }

        /// <summary>
        /// 获取一个产品/配方
        /// </summary>
        /// <param name="categoty"></param>
        /// <param name="recipeID"></param>
        /// <returns></returns>
        public IJFRecipe GetRecipe(string categoty, string recipeID)
        {
            string ErrInf = "";
            if (!_dctRecipes.ContainsKey(categoty))
                return null;
            Dictionary<string, IJFRecipe> dct = _dctRecipes[categoty];
             if (!dct.ContainsKey(recipeID))
                return null;
            if(categoty=="Product")
            {
                ((JFDLAFProductRecipe)dct[recipeID]).LoadRecipeScanPositonFile(out ErrInf);
                if (ErrInf != "Success")
                    return null;
            }
            return dct[recipeID];
        }

        /// <summary>
        /// 添加一个产品/配方
        /// </summary>
        /// <param name="categoty"></param>
        /// <param name="recipeID"></param>
        /// <param name="recipe"></param>
        public bool AddRecipe(string categoty, string recipeID, IJFRecipe recipe = null)
        {
            if (string.IsNullOrEmpty(categoty))
                return false;
            if (string.IsNullOrEmpty(recipeID))
                return false;
            if (recipe != null && (recipe.GetType() != typeof(JFDLAFProductRecipe) && recipe.GetType() != typeof(JFDLAFBoxRecipe)))
                return false;
            if (GetRecipe(categoty, recipeID) != null) //已存在同名Recipe
                return false;

            if (categoty == "Product")
            {
                JFDLAFProductRecipe cmRecipe = recipe as JFDLAFProductRecipe;
                if (null == cmRecipe)
                    cmRecipe = new JFDLAFProductRecipe();
                cmRecipe.ID = recipeID;
                cmRecipe.Categoty = categoty;

                List<string> lstCatesInCfg = _cfg.GetItemValue("Categoties") as List<string>;
                if (!lstCatesInCfg.Contains(categoty))
                    lstCatesInCfg.Add(categoty);

                JFXmlDictionary<string, List<string[]>> dctRecipesInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
                if (!dctRecipesInCfg.ContainsKey(categoty))
                    dctRecipesInCfg.Add(categoty, new List<string[]>());
                List<string[]> lstIDAndTxt = dctRecipesInCfg[categoty];
                lstIDAndTxt.Add(new string[] { recipeID, cmRecipe.Dict.ToString() });

                if (!_dctRecipes.ContainsKey(categoty))
                    _dctRecipes.Add(categoty, new Dictionary<string, IJFRecipe>());
                Dictionary<string, IJFRecipe> dctInMmry = _dctRecipes[categoty];
                dctInMmry.Add(recipeID, cmRecipe);
            }
            else
            {
                JFDLAFBoxRecipe cmRecipe = recipe as JFDLAFBoxRecipe;
                if (null == cmRecipe)
                    cmRecipe = new JFDLAFBoxRecipe();
                cmRecipe.ID = recipeID;
                cmRecipe.Categoty = categoty;

                List<string> lstCatesInCfg = _cfg.GetItemValue("Categoties") as List<string>;
                if (!lstCatesInCfg.Contains(categoty))
                    lstCatesInCfg.Add(categoty);

                JFXmlDictionary<string, List<string[]>> dctRecipesInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
                if (!dctRecipesInCfg.ContainsKey(categoty))
                    dctRecipesInCfg.Add(categoty, new List<string[]>());
                List<string[]> lstIDAndTxt = dctRecipesInCfg[categoty];
                lstIDAndTxt.Add(new string[] { recipeID, cmRecipe.Dict.ToString() });

                if (!_dctRecipes.ContainsKey(categoty))
                    _dctRecipes.Add(categoty, new Dictionary<string, IJFRecipe>());
                Dictionary<string, IJFRecipe> dctInMmry = _dctRecipes[categoty];
                dctInMmry.Add(recipeID, cmRecipe);
            }
            return true;

        }

        /// <summary>
        /// 移出一个产品配方
        /// </summary>
        /// <param name="categoty"></param>
        /// <param name="recipeID"></param>
        /// <returns></returns>
        public IJFRecipe RemoveRecipe(string categoty, string recipeID)
        {
            if (string.IsNullOrEmpty(categoty))
                return null;
            if (string.IsNullOrEmpty(recipeID))
                return null;

            IJFRecipe ret = GetRecipe(categoty, recipeID);
            if (ret == null) //不存在同名Recipe
                return ret;

            Dictionary<string, IJFRecipe> dctInMmry = _dctRecipes[categoty];
            dctInMmry.Remove(recipeID);
            if (dctInMmry.Count == 0)
                _dctRecipes.Remove(categoty);


            JFXmlDictionary<string, List<string[]>> dctRecipesInCfg = _cfg.GetItemValue("Cate-Recipes") as JFXmlDictionary<string, List<string[]>>;
            List<string[]> lstIDAndTxt = dctRecipesInCfg[categoty];
            for (int i = 0; i < lstIDAndTxt.Count; i++)
            {
                if (lstIDAndTxt[i][0] == recipeID)
                {
                    lstIDAndTxt.RemoveAt(i);
                    break;
                }
            }
            if (lstIDAndTxt.Count == 0)
            {
                dctRecipesInCfg.Remove(categoty);
                List<string> lstCatesInCfg = _cfg.GetItemValue("Categoties") as List<string>;
                lstCatesInCfg.Remove(categoty);
            }
            return ret;

        }

        public void Dispose()
        {
            return;
        }


        /// <summary>
        /// </summary>
        /// <returns>包含目录的指针list</returns>
        public List<String> GetProductList()
        {
            List<String> list = new List<string>();
            DirectoryInfo Path = new DirectoryInfo((string)GetInitParamValue(InitParamNames[2]));
            DirectoryInfo[] Dir = Path.GetDirectories();
            foreach (DirectoryInfo d in Dir)
            {
                list.Add(d.Name);
            }
            //read product directory to fetch the list
            return list;
        }

        FrmProduct _ui = new FrmProduct();
        public JFRealtimeUI GetRealtimeUI()
        {
            JFDLAFProductRecipe recipe = JFHub.JFHubCenter.Instance.RecipeManager.GetRecipe("Product", (string)JFHub.JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID")) as JFDLAFProductRecipe;

            _ui.SetRecipe(recipe);
            return _ui;
        }

        #endregion
    }
}
