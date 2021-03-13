using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LFAOIRecipe
{
    [Serializable]
    public class UserRegion : ViewModelBase
    {
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        //检测焊点周围区域图层选择
        private int imageIndex = 0;
        public int ImageIndex
        {
            get => imageIndex;
            set => OnPropertyChanged(ref imageIndex, value);
        }


        public HObject DisplayRegion { get; set; }

        public HObject CalculateRegion { get; set; }

        //1211
        public string RegionPath { get; set; }

        private RegionType regionType;
        public RegionType RegionType
        {
            get => regionType;
            set => OnPropertyChanged(ref regionType, value);
        }
        //add by wj 自定义区域操作类别
        private RegionOperatType regionOperatType;
        public RegionOperatType RegionOperatType
        {
            get => regionOperatType;
            set => OnPropertyChanged(ref regionOperatType, value);
        }

        //by wj
        public ObservableCollection<WireAutoRegionGroup> modelGroups;
        public ObservableCollection<WireAutoRegionGroup> ModelGroups
        {
            get => modelGroups;
            set => OnPropertyChanged(ref modelGroups, value);
        }

        public WireAutoRegionGroup currentModelGroup;
        public WireAutoRegionGroup CurrentModelGroup
        {
            get => currentModelGroup;
            set
            {
                if (currentModelGroup != value)
                {
                    currentModelGroup = value;
                    OnPropertyChanged();
                }
            }
        }


        public int currentWireModelind;
        public int CurrentWireModelind
        {
            get => currentWireModelind;
            set => OnPropertyChanged(ref currentWireModelind, value);
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set => OnPropertyChanged(ref isSelected, value);
        }

        private int index;
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value);
        }

        //1022 mark the initial index
        private int index_ini;
        public int Index_ini
        {
            get => index_ini;
            set => OnPropertyChanged(ref index_ini, value);
        }

        private int lastIndex;
        public int LastIndex
        {
            get => lastIndex;
            set => OnPropertyChanged(ref lastIndex, value);
        }

        private bool isEnable = true;
        public bool IsEnable
        {
            get => isEnable;
            set => OnPropertyChanged(ref isEnable, value);
        }

        private bool isAccept = true;
        public bool IsAccept
        {
            get => isAccept;
            set => OnPropertyChanged(ref isAccept, value);
        }

        private int algoParameterIndex = 0;
        public int AlgoParameterIndex
        {
            get => algoParameterIndex;
            set => OnPropertyChanged(ref algoParameterIndex, value);
        }

        //add by wj 2020-10-22
        private int regAlgParameterIndex = 1;
        public int RegAlgParameterIndex
        {
            get => regAlgParameterIndex;
            set => OnPropertyChanged(ref regAlgParameterIndex, value);
        }
        //默认检测
        private int isAroundBondRegInspect = 1;
        // <summary>
        // 是否Bond周围检测
        //</summary>
        public int IsAroundBondRegInspect
        {
            get => isAroundBondRegInspect;
            set => OnPropertyChanged(ref isAroundBondRegInspect, value);
        }

        private double[] regionParameters;
        public double[] RegionParameters
        {
            get => regionParameters;
            set => OnPropertyChanged(ref regionParameters, value);
        }

        private bool isCurrentStep = false;
        public bool IsCurrentStep
        {
            get => isCurrentStep;
            set => OnPropertyChanged(ref isCurrentStep, value);
        }

        private string recipeNames;
        public string RecipeNames
        {
            get => recipeNames;
            set => OnPropertyChanged(ref recipeNames, value);
        }

        private double bondMatchAutoAngle;
        public double BondMatchAutoAngle
        {
            get => bondMatchAutoAngle;
            set => OnPropertyChanged(ref bondMatchAutoAngle, value);
        }

        public GoldenModelInspectParameter SubParameter { get; set; }

        public GoldenModelInspectParameter UnsubParameter { get; set; }

        public BondMeasureModelParameter BondMeasureModelParameter { get; set; }

        public BondMeasureVerifyParameterSet BondMeasureVerifyParameterSet { get; set; }

        public EpoxyModelVerifyParameterSet EpoxyModelVerifyParameterSet { get; set; }        

        public WireRegionWithPara WireRegionWithPara { get; set; }
        //add by  wj 2020-10-22
        public AroundBondRegionWithPara AroundBondRegionWithPara { get; set; }

        //add by wj 2021-01-06
        public BondVerifyRegionWithPara BondVerifyRegionWithPara { get; set; }


        public override string ToString()
        {
            //区域操作类型显示  add by wj
            switch (this.RegionOperatType)
            {
                case RegionOperatType.Union:
                    if (this.RegionParameters.Count() < 2) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"FirstReg: {RegionParameters[0]:#} SecondReg: {RegionParameters[1]:#}";

                case RegionOperatType.Difference:
                    if (this.RegionParameters.Count() < 2) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"FirstReg: {RegionParameters[0]:#} SecondReg: {RegionParameters[1]:#}";

                case RegionOperatType.Threshold:
                    if (this.RegionParameters.Count() < 2) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"MinGray: {RegionParameters[0]:#} MaxGray: {RegionParameters[1]:#}";

                case RegionOperatType.Erosion:
                    if (this.RegionParameters.Count() < 1) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"Radius: {RegionParameters[0]:#.0}";

                case RegionOperatType.Dilation:
                    if (this.RegionParameters.Count() < 1) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"Radius: {RegionParameters[0]:#.0}";

                case RegionOperatType.Fillup:
                    if (this.RegionParameters.Count() < 1) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"MaxArea: {RegionParameters[0]:#}";

                case RegionOperatType.OpeningCircle:
                    if (this.RegionParameters.Count() < 1) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"OpenSize: {RegionParameters[0]:#.0}";

                case RegionOperatType.ClosingCircle:
                    if (this.RegionParameters.Count() < 1) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"CloseSize: {RegionParameters[0]:#.0}";

                case RegionOperatType.SelectShape:
                    if (this.RegionParameters.Count() < 2) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"MinArea: {RegionParameters[0]:#} MaxArea: {RegionParameters[1]:#}";

                case RegionOperatType.RegionTrans:
                    if (this.RegionParameters.Count() < 1) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"TransType: {RegionParameters[0]:#}";

                case RegionOperatType.RegionArray:
                    if (this.RegionParameters.Count() < 1) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"RegionArrayNum: {RegionParameters[0]:#}";

                //default: return string.Empty;
            }

            //区域外形显示   add by wj
            switch (this.RegionType)
            {
                case RegionType.Point:
                    if (this.RegionParameters.Count() < 2) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0}";
                    return $"Row:{Math.Floor(RegionParameters[0])} Col:{Math.Floor(RegionParameters[1])}";

                case RegionType.Line:
                    if (this.RegionParameters.Count() < 2) return string.Empty;//
                    //return $"row1:{RegionParameters[0]:#.0} col1:{RegionParameters[1]:#.0} row2:{RegionParameters[2]:#.0} col2:{RegionParameters[3]:#.0} ";//
                    return $"Row1:{Math.Floor(RegionParameters[0])} Col1:{Math.Floor(RegionParameters[1])} Row2:{Math.Floor(RegionParameters[2])} Col2:{Math.Floor(RegionParameters[3])} ";//

                case RegionType.Rectangle1:
                    if (this.RegionParameters.Count() < 4) return string.Empty;
                    //return $"row1:{RegionParameters[0]:#.0} col1:{RegionParameters[1]:#.0} row2:{RegionParameters[2]:#.0} col2:{RegionParameters[3]:#.0} ";
                    return $"Row1:{Math.Floor(RegionParameters[0])} Col1:{Math.Floor(RegionParameters[1])} Row2:{Math.Ceiling(RegionParameters[2])} Col2:{Math.Ceiling(RegionParameters[3])} ";

                case RegionType.Rectangle2:
                    if (this.RegionParameters.Count() < 5) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} len1:{RegionParameters[3]:#.0} len2:{RegionParameters[4]:#.0}";
                    return $"Row:{Math.Floor(RegionParameters[0])} Col:{Math.Floor(RegionParameters[1])} Phi:{RegionParameters[2]:#.0} Len1:{Math.Ceiling(RegionParameters[3])} Len2:{Math.Ceiling(RegionParameters[4])}";

                case RegionType.Circle:
                    if (this.RegionParameters.Count() < 3) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} rad:{RegionParameters[2]:#.0}";
                    return $"Row:{Math.Floor(RegionParameters[0])} Col:{Math.Floor(RegionParameters[1])} Radius:{RegionParameters[2]:#.00}";

                case RegionType.Ellipse:
                    if (this.RegionParameters.Count() < 5) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    return $"Row:{Math.Floor(RegionParameters[0])} Col:{Math.Floor(RegionParameters[1])} Phi:{RegionParameters[2]:#.0} Rad1:{Math.Ceiling(RegionParameters[3])} Rad2:{Math.Ceiling(RegionParameters[4])}";

                case RegionType.Region:

                    if(this.RegionPath == null)
                    {
                        RegionPath = "freeRegion";
                    }
                    //if (this.RegionParameters.Count() < 1 ) return string.Empty;
                    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                    //return $"Name: Free{this.RegionType}{this.Index}";
                    //1211
                    return $"Path: {this.RegionPath.ToString()}";

                //case RegionType.Union:
                //    if (this.RegionParameters.Count() < 2) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"FirstReg: {RegionParameters[0]:#} SecondReg: {RegionParameters[1]:#}";

                //case RegionType.Difference:
                //    if (this.RegionParameters.Count() < 2) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"FirstReg: {RegionParameters[0]:#} SecondReg: {RegionParameters[1]:#}";

                //case RegionType.Threshold:
                //    if (this.RegionParameters.Count() < 2) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"MinGray: {RegionParameters[0]:#} MaxGray: {RegionParameters[1]:#}";

                //case RegionType.Erosion:
                //    if (this.RegionParameters.Count() < 1) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"Radius: {RegionParameters[0]:#.0}";

                //case RegionType.Dilation:
                //    if (this.RegionParameters.Count() < 1) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"Radius: {RegionParameters[0]:#.0}";

                //case RegionType.Fillup:
                //    if (this.RegionParameters.Count() < 1) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"MaxArea: {RegionParameters[0]:#}";

                //case RegionType.OpeningCircle:
                //    if (this.RegionParameters.Count() < 1) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"OpenSize: {RegionParameters[0]:#.0}";

                //case RegionType.ClosingCircle:
                //    if (this.RegionParameters.Count() < 1) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"CloseSize: {RegionParameters[0]:#.0}";

                //case RegionType.SelectShape:
                //    if (this.RegionParameters.Count() < 2) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"MinArea: {RegionParameters[0]:#} MaxArea: {RegionParameters[1]:#}";

                //case RegionType.RegionTrans:
                //    if (this.RegionParameters.Count() < 1) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"TransType: {RegionParameters[0]:#}";

                //case RegionType.RegionArray:
                //    if (this.RegionParameters.Count() < 1) return string.Empty;
                //    //return $"row:{RegionParameters[0]:#.0} col:{RegionParameters[1]:#.0} phi:{RegionParameters[2]:#.0} rad:{RegionParameters[3]:#.0} rad:{RegionParameters[4]:#.0}";
                //    return $"RegionArrayNum: {RegionParameters[0]:#}";

                default: return string.Empty;
            }  
            
        }

        public static UserRegion GetHWindowRegion(HTHalControlWPF htWindow, double rowOffset = 0, double columnOffset = 0, double Width_Rectangle2 = -1)
        {
            if (htWindow.RegionType == RegionType.Null)
            {
                System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                return null;
            }
            HObject displayRegion = null;
            HObject calculateRegion = null;
            HOperatorSet.GenEmptyObj(out displayRegion);
            HOperatorSet.GenEmptyObj(out calculateRegion);
            double[] regionParameters;
            switch (htWindow.RegionType)
            {
                case RegionType.Point:
                    HOperatorSet.GenRegionPoints(out displayRegion, htWindow.Row_Point, htWindow.Column_Point);
                    regionParameters = new double[2]
                    {
                        htWindow.Row_Point.D + rowOffset,
                        htWindow.Column_Point.D + columnOffset,
                    };
                    HOperatorSet.GenRegionPoints(out calculateRegion, regionParameters[0], regionParameters[1]);
                    break;
                case RegionType.Line:
                    HOperatorSet.GenRegionLine(out displayRegion, htWindow.Row1_Line, htWindow.Column1_Line, htWindow.Row2_Line, htWindow.Column2_Line);
                    regionParameters = new double[4]
                    {
                        htWindow.Row1_Line.D + rowOffset,
                        htWindow.Column1_Line.D + columnOffset,
                        htWindow.Row2_Line.D + rowOffset,
                        htWindow.Column2_Line.D + columnOffset
                    };
                    HOperatorSet.GenRegionLine(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3]);
                    break;

                case RegionType.Rectangle1:
                    HTuple row1 = Math.Floor(htWindow.Row1_Rectangle1.D);//
                    HTuple col1 = Math.Floor(htWindow.Column1_Rectangle1.D);//
                    HTuple row2 = Math.Ceiling(htWindow.Row2_Rectangle1.D);//
                    HTuple col2 = Math.Ceiling(htWindow.Column2_Rectangle1.D);//
                    HOperatorSet.GenRectangle1(out displayRegion, row1, col1, row2, col2);//
                    //HOperatorSet.GenRectangle1(out displayRegion, htWindow.Row1_Rectangle1, htWindow.Column1_Rectangle1, htWindow.Row2_Rectangle1, htWindow.Column2_Rectangle1);//
                    regionParameters = new double[4]
                    {
                        htWindow.Row1_Rectangle1.D + rowOffset,
                        htWindow.Column1_Rectangle1.D + columnOffset,
                        htWindow.Row2_Rectangle1.D + rowOffset,
                        htWindow.Column2_Rectangle1.D + columnOffset
                    };
                    HOperatorSet.GenRectangle1(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3]);
                    break;

                case RegionType.Rectangle2:
                    if (Width_Rectangle2 > 0)
                    {
                        HOperatorSet.GenRectangle2(out displayRegion, htWindow.Row_Rectangle2, htWindow.Column_Rectangle2, htWindow.Phi_Rectangle2, htWindow.Length1_Rectangle2, Width_Rectangle2);
                        regionParameters = new double[5]
                        {
                        htWindow.Row_Rectangle2.D + rowOffset,
                        htWindow.Column_Rectangle2.D + columnOffset,
                        htWindow.Phi_Rectangle2.D,
                        htWindow.Length1_Rectangle2.D,
                        Width_Rectangle2
                        };
                        HOperatorSet.GenRectangle2(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3], regionParameters[4]);
                    }
                    else
                    {
                        HOperatorSet.GenRectangle2(out displayRegion, htWindow.Row_Rectangle2, htWindow.Column_Rectangle2, htWindow.Phi_Rectangle2, htWindow.Length1_Rectangle2, htWindow.Length2_Rectangle2);
                        regionParameters = new double[5]
                        {
                        htWindow.Row_Rectangle2.D + rowOffset,
                        htWindow.Column_Rectangle2.D + columnOffset,
                        htWindow.Phi_Rectangle2.D,
                        htWindow.Length1_Rectangle2.D,
                        htWindow.Length2_Rectangle2.D
                        };
                        HOperatorSet.GenRectangle2(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3], regionParameters[4]);
                    }
                    break;

                case RegionType.Circle:
                    HOperatorSet.GenCircle(out displayRegion, htWindow.Row_Circle, htWindow.Column_Circle, htWindow.Radius_Circle);
                    regionParameters = new double[3]
                    {
                        htWindow.Row_Circle.D + rowOffset,
                        htWindow.Column_Circle.D + columnOffset,
                        htWindow.Radius_Circle.D
                    };
                    HOperatorSet.GenCircle(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2]);
                    break;

                case RegionType.Ellipse:
                    HOperatorSet.GenEllipse(out displayRegion, htWindow.Row_Ellipse, htWindow.Column_Ellipse, htWindow.Phi_Ellipse, htWindow.Radius1_Ellipse, htWindow.Radius2_Ellipse);
                    regionParameters = new double[5]
                    {
                        htWindow.Row_Ellipse.D + rowOffset,
                        htWindow.Column_Ellipse.D + columnOffset,
                        htWindow.Phi_Ellipse.D,
                        htWindow.Radius1_Ellipse.D,
                        htWindow.Radius2_Ellipse.D
                    };
                    HOperatorSet.GenEllipse(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3], regionParameters[4]);
                    break;

                //鼠标画任意形状的区域
                case RegionType.Region://
                    HOperatorSet.CopyObj(htWindow.Region_Region, out displayRegion, 1, -1);
                    
                    HOperatorSet.AreaCenter(displayRegion, out HTuple area, out HTuple row, out HTuple column);
                    regionParameters = new double[2]
                    {
                        row.D + rowOffset,
                        column.D + columnOffset,
                    };
                    HOperatorSet.MoveRegion(displayRegion, out calculateRegion, rowOffset, columnOffset);
                    break;

                default: return null;
            }

            UserRegion userRegion = new UserRegion
            {
                DisplayRegion = displayRegion,
                CalculateRegion = calculateRegion,
                RegionType = htWindow.RegionType,
                RegionParameters = regionParameters,
                RecipeNames = "Pad",
                RegionPath = "freeRegion",
            };
            return userRegion;
        }

        //更新区域坐标
        public static UserRegion GetHWindowRegionUpdate(HTHalControlWPF htWindow, RegionType regionTypeUpdate, HTuple Row1_Rectangle1, HTuple Column1_Rectangle1,
                                                        HTuple Row2_Rectangle1, HTuple Column2_Rectangle1,
                                                        double rowOffset = 0, double columnOffset = 0, HTuple Phi_Rectangle2 = null)//
        {
            if (regionTypeUpdate == RegionType.Null)
            {
                System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                return null;
            }

            HObject displayRegion = null;
            HObject calculateRegion = null;
            HOperatorSet.GenEmptyObj(out displayRegion);
            HOperatorSet.GenEmptyObj(out calculateRegion);
            double[] regionParameters;
            UserRegion userRegion;

            switch (regionTypeUpdate)
            {
                case RegionType.Point:
                    return null;

                case RegionType.Line:
                    HTuple row1_Line = Row1_Rectangle1.D;
                    HTuple col1_Line = Column1_Rectangle1.D;
                    HTuple row2_Line = Row2_Rectangle1.D;
                    HTuple col2_Line = Column2_Rectangle1.D;

                    HOperatorSet.GenRegionLine(out displayRegion, row1_Line, col1_Line, row2_Line, col2_Line);
                    regionParameters = new double[4]
                    {
                        Row1_Rectangle1.D + rowOffset,
                        Column1_Rectangle1.D + columnOffset,
                        Row2_Rectangle1.D + rowOffset,
                        Column2_Rectangle1.D + columnOffset
                    };
                    HOperatorSet.GenRegionLine(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3]);

                    userRegion = new UserRegion
                    {
                        DisplayRegion = displayRegion,
                        CalculateRegion = calculateRegion,
                        RegionType = regionTypeUpdate,
                        RegionParameters = regionParameters,
                    };
                    return userRegion;

                case RegionType.Rectangle1:
                    HTuple row1_Rectangle1 = Math.Floor(Row1_Rectangle1.D);
                    HTuple col1_Rectangle1 = Math.Floor(Column1_Rectangle1.D);
                    HTuple row2_Rectangle1 = Math.Ceiling(Row2_Rectangle1.D);
                    HTuple col2_Rectangle1 = Math.Ceiling(Column2_Rectangle1.D);

                    HOperatorSet.GenRectangle1(out displayRegion, row1_Rectangle1, col1_Rectangle1, row2_Rectangle1, col2_Rectangle1);
                    regionParameters = new double[4]
                    {
                        Row1_Rectangle1.D + rowOffset,
                        Column1_Rectangle1.D + columnOffset,
                        Row2_Rectangle1.D + rowOffset,
                        Column2_Rectangle1.D + columnOffset
                    };
                    HOperatorSet.GenRectangle1(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3]);

                    userRegion = new UserRegion
                    {
                        DisplayRegion = displayRegion,
                        CalculateRegion = calculateRegion,
                        RegionType = regionTypeUpdate,
                        RegionParameters = regionParameters,
                    };
                    return userRegion;

                case RegionType.Rectangle2:

                    HTuple row_Rectangle2 = Math.Floor(Row1_Rectangle1.D);
                    HTuple column_Rectangle2 = Math.Floor(Column1_Rectangle1.D);
                    HTuple lenth1_Rectangle2 = Math.Ceiling(Row2_Rectangle1.D);
                    HTuple lenth2_Rectangle2 = Math.Ceiling(Column2_Rectangle1.D);
                    HTuple phi_Rectangle2 = Phi_Rectangle2.D;

                    HOperatorSet.GenRectangle2(out displayRegion, row_Rectangle2, column_Rectangle2, phi_Rectangle2, lenth1_Rectangle2, lenth2_Rectangle2);
                    regionParameters = new double[5]
                    {
                        row_Rectangle2.D + rowOffset,
                        column_Rectangle2.D + columnOffset,
                        phi_Rectangle2.D,
                        lenth1_Rectangle2.D,
                        lenth2_Rectangle2
                    };
                    HOperatorSet.GenRectangle2(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3], regionParameters[4]);

                    userRegion = new UserRegion
                    {
                        DisplayRegion = displayRegion,
                        CalculateRegion = calculateRegion,
                        RegionType = regionTypeUpdate,
                        RegionParameters = regionParameters,
                    };
                    return userRegion;

                case RegionType.Circle:

                    HTuple row_Circle = Row1_Rectangle1.D;
                    HTuple column_Circle = Column1_Rectangle1.D;
                    HTuple radius_Circle = Row2_Rectangle1.D;

                    HOperatorSet.GenCircle(out displayRegion, row_Circle, column_Circle, radius_Circle);
                    regionParameters = new double[3]
                    {
                        row_Circle.D + rowOffset,
                        column_Circle.D + columnOffset,
                        radius_Circle.D,
                    };
                    HOperatorSet.GenCircle(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2]);

                    userRegion = new UserRegion
                    {
                        DisplayRegion = displayRegion,
                        CalculateRegion = calculateRegion,
                        RegionType = regionTypeUpdate,
                        RegionParameters = regionParameters,
                    };
                    return userRegion;

                case RegionType.Ellipse:

                    HTuple row_Ellipse = Row1_Rectangle1.D;
                    HTuple column_Ellipse = Column1_Rectangle1.D;
                    HTuple radius1_Ellipse = Row2_Rectangle1.D;
                    HTuple radius2_Ellipse = Column2_Rectangle1.D;
                    HTuple phi_Ellipse = Phi_Rectangle2.D;

                    HOperatorSet.GenEllipse(out displayRegion, row_Ellipse, column_Ellipse, phi_Ellipse, radius1_Ellipse, radius2_Ellipse);
                    regionParameters = new double[5]
                    {
                        row_Ellipse.D + rowOffset,
                        column_Ellipse.D + columnOffset,
                        phi_Ellipse,
                        radius1_Ellipse.D,
                        radius2_Ellipse.D
                    };
                    HOperatorSet.GenEllipse(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3], regionParameters[4]);

                    userRegion = new UserRegion
                    {
                        DisplayRegion = displayRegion,
                        CalculateRegion = calculateRegion,
                        RegionType = regionTypeUpdate,
                        RegionParameters = regionParameters,
                    };
                    return userRegion;
                case RegionType.Region:

                default: return null;
            }
        }

        public XElement ToXElement(string name, 
                                   bool IsSaveParameter = false, //其它
                                   bool IsSaveAlgoParameter = false,//金线检测区域内参数保存
                                   bool IsSaveModelAgloParameter = false,//自动生成金线检测区域模板区域内参数
                                   bool isSaveRegAlgoParameter = false,//ArondBond周围区域检测参数
                                   bool isSaveBondAlgoParameter = false//bond 检测区域内检测参数
                                   )
        {
            XElement xElement = new XElement(name);
            xElement.Add(new XAttribute("Index", this.Index.ToString()));
            xElement.Add(new XAttribute("Index_ini", this.Index_ini.ToString()));//1109
            xElement.Add(new XAttribute("IsEnable", this.isEnable ? "1" : "0"));
            xElement.Add(new XAttribute("IsAccept", this.IsAccept ? "1" : "0"));

            //区域操作类型显示  add by wj
            if (this.RegionOperatType.ToString() != "Null")
            {
                xElement.Add(new XAttribute("OperatType", this.RegionOperatType.ToString()));
            }
            //
            if (IsSaveAlgoParameter)
            {
                xElement.Add(new XAttribute("AlgoIndex", this.AlgoParameterIndex));
            }
            //
            if(IsSaveModelAgloParameter)
            {
                xElement.Add(new XAttribute("AlgoIndex", this.AlgoParameterIndex));
            }
            //add by wj 2020-10-22
            if (isSaveRegAlgoParameter)
            {
                xElement.Add(new XAttribute("IsInspect", this.IsAroundBondRegInspect));               
                //是否焊点偏移、焊盘异物检测
                xElement.Add(new XAttribute("IsBallShiftInspect", this.AroundBondRegionWithPara.IsBallShiftInspect ? "1" : "0"));
                //是否焊点尾丝检测
                xElement.Add(new XAttribute("IsTailInspect", this.AroundBondRegionWithPara.IsTailInspect ? "1" : "0"));
                //是否焊盘表面异物检测
                xElement.Add(new XAttribute("IsSurfaceInspect", this.AroundBondRegionWithPara.IsSurfaceInspect ? "1" : "0"));

            }
            //add by wj 2021-0107 save bond verify parameter index

            if(isSaveBondAlgoParameter)
            {
                xElement.Add(new XAttribute("AlgoIndex", this.AlgoParameterIndex));
            }

            xElement.Add(new XAttribute("Type", this.RegionType.ToString()));
            switch (this.RegionType)
            {
                case RegionType.Point:
                    xElement.Add(new XAttribute("Row", this.RegionParameters[0].ToString()));
                    xElement.Add(new XAttribute("Col", this.RegionParameters[1].ToString()));
                    break;
                case RegionType.Line:
                    xElement.Add(new XAttribute("Row1", this.RegionParameters[0].ToString()));
                    xElement.Add(new XAttribute("Col1", this.RegionParameters[1].ToString()));
                    xElement.Add(new XAttribute("Row2", this.RegionParameters[2].ToString()));
                    xElement.Add(new XAttribute("Col2", this.RegionParameters[3].ToString()));
                    break;
                case RegionType.Rectangle1:
                    xElement.Add(new XAttribute("Row1", this.RegionParameters[0].ToString()));
                    xElement.Add(new XAttribute("Col1", this.RegionParameters[1].ToString()));
                    xElement.Add(new XAttribute("Row2", this.RegionParameters[2].ToString()));
                    xElement.Add(new XAttribute("Col2", this.RegionParameters[3].ToString()));
                    break;
                case RegionType.Rectangle2:
                    xElement.Add(new XAttribute("Row", this.RegionParameters[0].ToString()));
                    xElement.Add(new XAttribute("Col", this.RegionParameters[1].ToString()));
                    xElement.Add(new XAttribute("Phi", this.RegionParameters[2].ToString()));
                    xElement.Add(new XAttribute("Len1", this.RegionParameters[3].ToString()));
                    xElement.Add(new XAttribute("Len2", this.RegionParameters[4].ToString()));
                    break;
                case RegionType.Circle:
                    xElement.Add(new XAttribute("Row", this.RegionParameters[0].ToString()));
                    xElement.Add(new XAttribute("Col", this.RegionParameters[1].ToString()));
                    xElement.Add(new XAttribute("Rad", this.RegionParameters[2].ToString()));
                    break;
                case RegionType.Ellipse:
                    xElement.Add(new XAttribute("Row", this.RegionParameters[0].ToString()));
                    xElement.Add(new XAttribute("Col", this.RegionParameters[1].ToString()));
                    xElement.Add(new XAttribute("Phi", this.RegionParameters[2].ToString()));
                    xElement.Add(new XAttribute("Rad1", this.RegionParameters[3].ToString()));
                    xElement.Add(new XAttribute("Rad2", this.RegionParameters[4].ToString()));
                    break;
                //1211
                case RegionType.Region:
                    xElement.Add(new XAttribute("RegionType", this.RecipeNames));
                    xElement.Add(new XAttribute("RegionPath", this.RegionPath));
                    break;

                default: break;
            }

            if (IsSaveParameter)
            {
                if (this.BondMeasureModelParameter != null)
                {
                    XElement paraElement = new XElement("Parameter");
                    XMLHelper.AddParameters(paraElement, BondMeasureModelParameter);
                    xElement.Add(paraElement);
                }

                //if (this.BondMeasureVerifyParameterSet != null)
                //{
                //    XElement paraElement = new XElement("VerifyParameterSet");
                //    XMLHelper.AddParameters(paraElement, BondMeasureVerifyParameterSet);
                //    xElement.Add(paraElement);
                //}

                if (this.EpoxyModelVerifyParameterSet != null)
                {
                    XElement paraElement = new XElement("EpoxyVerifyParameterSet");
                    XMLHelper.AddParameters(paraElement, EpoxyModelVerifyParameterSet);
                    xElement.Add(paraElement);
                }
            }
            //-----------保存金线检测区域带参数
            if (IsSaveAlgoParameter == true)
            {
                XElement paraElement = new XElement("Parameter");
                switch (this.AlgoParameterIndex)
                {
                    case 0:
                        XMLHelper.AddParameters(paraElement, this.WireRegionWithPara.WireThresAlgoPara);
                        break;

                    case 1:
                        XMLHelper.AddParameters(paraElement, this.WireRegionWithPara.WireLineGauseAlgoPara);
                        break;


                    case 2:
                        XMLHelper.AddParameters(paraElement, this.WireRegionWithPara.WireLineGauseAlgoParaAll);
                        break;

                    default:
                        break;
                }
                xElement.Add(paraElement);
            }
            //------------保存金线模板检测区域参数
            if (IsSaveModelAgloParameter == true)
            {
                XElement paraElement = new XElement("ModelParameter");
                switch (this.AlgoParameterIndex)
                {
                    case 0:
                        XMLHelper.AddParameters(paraElement, this.WireRegionWithPara.WireThresAlgoPara);
                        break;

                    case 1:
                        XMLHelper.AddParameters(paraElement, this.WireRegionWithPara.WireLineGauseAlgoPara);
                        break;

                    case 2:
                        XMLHelper.AddParameters(paraElement, this.WireRegionWithPara.WireLineGauseAlgoParaAll);
                        break;

                    default:
                        break;
                }
                xElement.Add(paraElement);
            }
            //---------------保存AroundBond区域内检测参数   add by wj 2020-10-22
            if (isSaveRegAlgoParameter == true)
            {
                //bondshift检测参数保存
                if(this.AroundBondRegionWithPara.IsBallShiftInspect)
                {

                    //焊点偏移检测算法索引
                    xElement.Add(new XAttribute("ShiftInspectMethodIndex", this.AroundBondRegionWithPara.ShiftInspectMethodIndex));

                    XElement paraElement = new XElement("BallShiftInspectParameter");

                    switch (this.AroundBondRegionWithPara.ShiftInspectMethodIndex)
                    {
                        case 0:
                            XMLHelper.AddParameters(paraElement, this.AroundBondRegionWithPara.AroundBallMeasureAlgoPara);
                            break;
                        default:
                            break;
                    }
                    xElement.Add(paraElement);
                }
                //tail检测参数保存
                if (this.AroundBondRegionWithPara.IsTailInspect)
                {
                    //尾丝检测算法索引
                    xElement.Add(new XAttribute("TailInspectMethodIndex", this.AroundBondRegionWithPara.TailInspectMethodIndex));

                    XElement paraElement = new XElement("TailInspectParameter");
                    switch (this.AroundBondRegionWithPara.TailInspectMethodIndex)
                    {
                        case 0:
                            XMLHelper.AddParameters(paraElement, this.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara);
                            break;
                        default:
                            break;
                    }
                    xElement.Add(paraElement);
                }
                //Surface检测参数保存
                if (this.AroundBondRegionWithPara.IsSurfaceInspect)
                {
                    //焊盘表面检测算法索引
                    xElement.Add(new XAttribute("SurfaceInspectMethodIndex", this.AroundBondRegionWithPara.SurfaceInspectMethodIndex));

                    XElement paraElement = new XElement("SurfaceInspectParameter");

                    switch (this.AroundBondRegionWithPara.SurfaceInspectMethodIndex)
                    {
                        case 0:
                            XMLHelper.AddParameters(paraElement, this.AroundBondRegionWithPara.AroundBondAdativeAlgoPara);
                            break;

                        case 1:
                            XMLHelper.AddParameters(paraElement, this.AroundBondRegionWithPara.AroundBondGlobalAlgoPara);
                            break;
                        default:
                            break;
                    }
                    xElement.Add(paraElement);
                }

                if((this.AroundBondRegionWithPara.IsBallShiftInspect == false) &&(this.AroundBondRegionWithPara.IsTailInspect == false)
                    && (this.AroundBondRegionWithPara.IsSurfaceInspect==false))
                {
                    XElement paraElement = new XElement("Parameter");
                    XMLHelper.AddParameters(paraElement, this.AroundBondRegionWithPara.AroundBondAdativeAlgoPara);
                    xElement.Add(paraElement);
                }
            }
            //add by wj save bond verify parameter 2021-0107
            if(isSaveBondAlgoParameter == true)
            {
                XElement paraElement = new XElement("BondVerifyParameter");
                switch (this.AlgoParameterIndex)
                {
                    case 0:
                        XMLHelper.AddParameters(paraElement, this.BondVerifyRegionWithPara.BondThresAlgoPara);
                        break;

                    case 1:
                        XMLHelper.AddParameters(paraElement, this.BondVerifyRegionWithPara.BondMeasureAlgoPara);
                        break;

                    case 2:
                        XMLHelper.AddParameters(paraElement, this.BondVerifyRegionWithPara.BondMatchAlgoPara);
                        break;

                    default:
                        break;
                }
                xElement.Add(paraElement);
            }

            return xElement;
        }

        public static UserRegion FromXElement(XElement xElement, bool IsReadParameter = false,
                                              double rowOffset = 0, double columnOffset = 0, 
                                              bool IsReadAlgoParameter = false,
                                              bool IsReadModelAlgoParameter = false,
                                              bool IsReadRegAlgoParameter = false,//读取AroundBond检测参数
                                              bool IsReadBondAlgoParameter = false//读取bond检测参数
                                              )
        {
            if (xElement == null) return null;
            HObject displayRegion = null;
            HObject calculateRegion = null;
            HOperatorSet.GenEmptyObj(out displayRegion);
            HOperatorSet.GenEmptyObj(out calculateRegion);
            try
            {
                UserRegion userRegion = new UserRegion
                {
                    Index = int.Parse(xElement.Attribute("Index").Value),
                    RegionType = (RegionType)Enum.Parse(typeof(RegionType), xElement.Attribute("Type").Value),
                    isEnable = xElement.Attribute("IsEnable").Value.Equals("1") ? true : false,
                    isAccept = xElement.Attribute("IsAccept").Value.Equals("1") ? true : false,//  
                };
 
                //区域操作类型显示  add by wj
                if (userRegion.RegionOperatType.ToString() != "Null")
                {
                    userRegion.RegionOperatType = (RegionOperatType)Enum.Parse(typeof(RegionOperatType), xElement.Attribute("OperatType").Value);
                    
                }

                double[] regionParameters;
                string _regionPath = string.Empty;
                string _recipeNames = string.Empty;

                switch (userRegion.RegionType)
                {
                    case RegionType.Point:
                        double row_Point = double.Parse(xElement.Attribute("Row").Value);
                        double column_Point = double.Parse(xElement.Attribute("Col").Value);
                        HOperatorSet.GenRegionPoints(out displayRegion, row_Point - rowOffset, column_Point - columnOffset);
                        regionParameters = new double[2]
                        {
                            row_Point,
                            column_Point
                        };
                        HOperatorSet.GenRegionPoints(out calculateRegion, regionParameters[0], regionParameters[1]);
                        break;

                    case RegionType.Line:
                        double row1_Line = double.Parse(xElement.Attribute("Row1").Value);
                        double column1_Line = double.Parse(xElement.Attribute("Col1").Value);
                        double row2_Line = double.Parse(xElement.Attribute("Row2").Value);
                        double column2_Line = double.Parse(xElement.Attribute("Col2").Value);
                        HOperatorSet.GenRegionLine(out displayRegion, row1_Line - rowOffset, column1_Line - columnOffset, row2_Line - rowOffset, column2_Line - columnOffset);
                        regionParameters = new double[4]
                        {
                            row1_Line,
                            column1_Line,
                            row2_Line,
                            column2_Line
                        };
                        HOperatorSet.GenRegionLine(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3]);
                        break;

                    case RegionType.Rectangle1:
                        double row1_Rectangle1 = new HTuple(double.Parse(xElement.Attribute("Row1").Value));
                        double column1_Rectangle1 = new HTuple(double.Parse(xElement.Attribute("Col1").Value));
                        double row2_Rectangle1 = new HTuple(double.Parse(xElement.Attribute("Row2").Value));
                        double column2_Rectangle1 = new HTuple(double.Parse(xElement.Attribute("Col2").Value));
                        HOperatorSet.GenRectangle1(out displayRegion, row1_Rectangle1 - rowOffset, column1_Rectangle1 - columnOffset, row2_Rectangle1 - rowOffset, column2_Rectangle1 - columnOffset);
                        regionParameters = new double[4]
                        {
                            row1_Rectangle1,
                            column1_Rectangle1,
                            row2_Rectangle1,
                            column2_Rectangle1
                        };
                        HOperatorSet.GenRectangle1(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3]);
                        break;

                    case RegionType.Rectangle2:
                        double row_Rectangle2 = double.Parse(xElement.Attribute("Row").Value);
                        double column_Rectangle2 = double.Parse(xElement.Attribute("Col").Value);
                        double phi_Rectangle2 = double.Parse(xElement.Attribute("Phi").Value);
                        double length1_Rectangle2 = double.Parse(xElement.Attribute("Len1").Value);
                        double length2_Rectangle2 = double.Parse(xElement.Attribute("Len2").Value);
                        HOperatorSet.GenRectangle2(out displayRegion, row_Rectangle2 - rowOffset, column_Rectangle2 - columnOffset, phi_Rectangle2, length1_Rectangle2, length2_Rectangle2);
                        regionParameters = new double[5]
                        {
                            row_Rectangle2,
                            column_Rectangle2,
                            phi_Rectangle2,
                            length1_Rectangle2,
                            length2_Rectangle2
                        };
                        HOperatorSet.GenRectangle2(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3], regionParameters[4]);
                        break;

                    case RegionType.Circle:
                        double row_Circle = double.Parse(xElement.Attribute("Row").Value);
                        double column_Circle = double.Parse(xElement.Attribute("Col").Value);
                        double radius_Circle = double.Parse(xElement.Attribute("Rad").Value);
                        HOperatorSet.GenCircle(out displayRegion, row_Circle - rowOffset, column_Circle - columnOffset, radius_Circle);
                        regionParameters = new double[3]
                        {
                            row_Circle,
                            column_Circle,
                            radius_Circle
                        };
                        HOperatorSet.GenCircle(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2]);
                        break;

                    case RegionType.Ellipse:
                        double row_Ellipse = double.Parse(xElement.Attribute("Row").Value);
                        double column_Ellipse = double.Parse(xElement.Attribute("Col").Value);
                        double phi_Ellipse = double.Parse(xElement.Attribute("Phi").Value);
                        double radius1_Ellipse = double.Parse(xElement.Attribute("Rad1").Value);
                        double radius2_Ellipse = double.Parse(xElement.Attribute("Rad2").Value);
                        HOperatorSet.GenEllipse(out displayRegion, row_Ellipse - rowOffset, column_Ellipse - columnOffset, phi_Ellipse, radius1_Ellipse, radius2_Ellipse);
                        regionParameters = new double[5]
                        {
                            row_Ellipse,
                            column_Ellipse,
                            phi_Ellipse,
                            radius1_Ellipse,
                            radius2_Ellipse
                        };
                        HOperatorSet.GenEllipse(out calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3], regionParameters[4]);
                        break;
                    //1211
                    case RegionType.Region:
                        regionParameters = new double[1]
                       {
                            1,
                       };
                        _regionPath = xElement.Attribute("RegionPath").Value.ToString();
                        _recipeNames = xElement.Attribute("RegionType").Value;
                        break;
                    default:
                        regionParameters = new double[1]
                           {
                            1,
                           };
                        break;
                }
                //add by wj 2020-12-24
                //switch(userRegion.RegionOperatType)
                //{
                //    case RegionOperatType.Dilation:
                //        regionParameters = new double[1]
                //       {
                //            1,
                //       };
                //        break;
                //    default:
                //        regionParameters = new double[1]
                //           {
                //            1,
                //           };
                //        break;
                //}

                //修改by wj   2020-10-22
                //1211
                if (RegionType.Region != userRegion.RegionType)
                {
                    userRegion.DisplayRegion = displayRegion;
                    userRegion.CalculateRegion = calculateRegion;
                    userRegion.RegionParameters = regionParameters;
                }
                else if(RegionType.Region == userRegion.RegionType)
                {
                    userRegion.RegionPath = _regionPath;
                    userRegion.RecipeNames = _recipeNames;
                }
                //userRegion.DisplayRegion = displayRegion;
                //userRegion.CalculateRegion = calculateRegion;
                //userRegion.RegionParameters = regionParameters;

                if (IsReadParameter)
                {
                    userRegion.BondMeasureModelParameter = new BondMeasureModelParameter();
                    //userRegion.BondMeasureVerifyParameterSet = new BondMeasureVerifyParameterSet();
                    userRegion.EpoxyModelVerifyParameterSet = new EpoxyModelVerifyParameterSet();

                    XElement paraElement = xElement.Element("Parameter");
                    //XElement _paraElement = xElement.Element("VerifyParameterSet");
                    XElement paraElementEpoxy = xElement.Element("EpoxyVerifyParameterSet");
                    if (paraElement != null)
                    {
                        XMLHelper.ReadParameters(paraElement, userRegion.BondMeasureModelParameter);
                    }
                    //if (_paraElement != null)
                    //{
                    //    XMLHelper.ReadParameters(_paraElement, userRegion.BondMeasureVerifyParameterSet);
                    //}
                    if (paraElementEpoxy != null)
                    {
                        XMLHelper.ReadParameters(paraElementEpoxy, userRegion.EpoxyModelVerifyParameterSet);
                    }
                }

                //-------读取金线检测区域带参数
                if (IsReadAlgoParameter)
                {
                    userRegion.AlgoParameterIndex = int.Parse(xElement.Attribute("AlgoIndex").Value);

                    userRegion.WireRegionWithPara = new WireRegionWithPara();

                    XElement paraElement = xElement.Element("Parameter");

                    if (paraElement == null) return userRegion;

                    switch (userRegion.AlgoParameterIndex)
                    {
                        case 0:
                            XMLHelper.ReadParameters(paraElement, userRegion.WireRegionWithPara.WireThresAlgoPara);
                            break;

                        case 1:
                            XMLHelper.ReadParameters(paraElement, userRegion.WireRegionWithPara.WireLineGauseAlgoPara);
                            break;

                        case 2:
                            XMLHelper.ReadParameters(paraElement, userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll);
                            break;

                        default:
                            break;
                    }
                }

                //-------读取金线模板检测区域带参数
                if (IsReadModelAlgoParameter)
                {
                    userRegion.AlgoParameterIndex = int.Parse(xElement.Attribute("AlgoIndex").Value);

                    userRegion.WireRegionWithPara = new WireRegionWithPara();

                    XElement paraElement = xElement.Element("ModelParameter");

                    if (paraElement == null) return userRegion;

                    switch (userRegion.AlgoParameterIndex)
                    {
                        case 0:
                            XMLHelper.ReadParameters(paraElement, userRegion.WireRegionWithPara.WireThresAlgoPara);
                            break;

                        case 1:
                            XMLHelper.ReadParameters(paraElement, userRegion.WireRegionWithPara.WireLineGauseAlgoPara);
                            break;

                        case 2:
                            XMLHelper.ReadParameters(paraElement, userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll);
                            break;

                        default:
                            break;
                    }
                }

                // add by wj 2020-09-11 读取AroundBond区域检测参数加载参数
                if (IsReadRegAlgoParameter)
                {
                    userRegion.AroundBondRegionWithPara = new AroundBondRegionWithPara();                  
                    userRegion.isAroundBondRegInspect = int.Parse(xElement.Attribute("IsInspect").Value);
                    //是否焊点偏移检测
                    userRegion.AroundBondRegionWithPara.IsBallShiftInspect = xElement.Attribute("IsBallShiftInspect").Value.Equals("1") ? true : false;                  
                    //是否尾丝检测
                    userRegion.AroundBondRegionWithPara.IsTailInspect = xElement.Attribute("IsTailInspect").Value.Equals("1") ? true : false;
                    //userRegion.AroundBondRegionWithPara.IsTailInspect = bool.Parse(xElement.Attribute("IsTailInspect").Value);
                    //是否焊盘表面检测
                    userRegion.AroundBondRegionWithPara.IsSurfaceInspect = xElement.Attribute("IsSurfaceInspect").Value.Equals("1") ? true : false;
                    //userRegion.AroundBondRegionWithPara.IsSurfaceInspect = bool.Parse(xElement.Attribute("IsSurfaceInspect").Value);                   
                    if (userRegion.AroundBondRegionWithPara.IsBallShiftInspect)
                    {
                        XElement shiftparaElement = xElement.Element("BallShiftInspectParameter");

                        if (shiftparaElement == null) return userRegion;
                        switch(userRegion.AroundBondRegionWithPara.ShiftInspectMethodIndex)
                        {
                            case 0:
                                XMLHelper.ReadParameters(shiftparaElement, userRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara);
                                break;
                            default:
                                break;
                        }
                    }

                    if(userRegion.AroundBondRegionWithPara.IsTailInspect)
                    {
                        XElement tailparaElement = xElement.Element("TailInspectParameter");

                        if (tailparaElement == null) return userRegion;

                        switch (userRegion.AroundBondRegionWithPara.TailInspectMethodIndex)
                        {
                            case 0:
                                XMLHelper.ReadParameters(tailparaElement, userRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara);
                                break;
                            default:
                                break;
                        }
                    }
                    if (userRegion.AroundBondRegionWithPara.IsSurfaceInspect)
                    {

                        XElement surfaceparaElement = xElement.Element("SurfaceInspectParameter");

                        if (surfaceparaElement == null) return userRegion;

                        switch (userRegion.AroundBondRegionWithPara.SurfaceInspectMethodIndex)
                        {
                            case 0:
                                XMLHelper.ReadParameters(surfaceparaElement, userRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara);
                                break;

                            case 1:
                                XMLHelper.ReadParameters(surfaceparaElement, userRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara);
                                break;
                            default:
                                break;
                        }
                    }

                }

                //add by wj read bond verify parameter 2021-0107
                if(IsReadBondAlgoParameter == true)
                {
                    userRegion.AlgoParameterIndex = int.Parse(xElement.Attribute("AlgoIndex").Value);
                    userRegion.BondVerifyRegionWithPara = new BondVerifyRegionWithPara();

                    XElement paraElement = xElement.Element("BondVerifyParameter");

                    if (paraElement == null) return userRegion;

                    switch (userRegion.AlgoParameterIndex)
                    {
                        case 0:
                            XMLHelper.ReadParameters(paraElement, userRegion.BondVerifyRegionWithPara.BondThresAlgoPara);
                            break;

                        case 1:
                            XMLHelper.ReadParameters(paraElement, userRegion.BondVerifyRegionWithPara.BondMeasureAlgoPara);
                            break;

                        case 2:
                            XMLHelper.ReadParameters(paraElement, userRegion.BondVerifyRegionWithPara.BondMatchAlgoPara);
                            break;

                        default:
                            break;
                    }


                }

                return userRegion;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        public UserRegion Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (this.GetType().IsSerializable)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    return formatter.Deserialize(stream)as UserRegion;
                }
                return null;
            }
        }

        public UserRegion Clone1()

        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream) as UserRegion;
        }



    }
}
