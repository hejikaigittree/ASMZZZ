using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace LFAOIRecipe
{
    public class CreateAutoBondMeasureModel : ViewModelBase, IProcedure
    {
        //1122
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public bool isRightClick = true;

        public string ReferenceDirectory { get; set; }

        private readonly string ModelsFile;

        private readonly string RecipeFile;

        public int ImageChannelIndex;

        private string imageIndex = "System.Windows.Controls.ComboBoxItem: 原图";
        public string ImageIndex 
        {
            get => imageIndex;
            set
            {
                if (imageIndex != value)
                {
                    BondMeasureModelObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex);
                    htWindow.Display(BondMeasureModelObject.ChannelImage);
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private UserRegion selectedUserRegion;
        public UserRegion SelectedUserRegion
        {
            get => selectedUserRegion;
            set => OnPropertyChanged(ref selectedUserRegion, value);
        }

        public BondMeasureModelObject BondMeasureModelObject { get; set; }

        public BondMeasureParameter BondMeasureParameter { get; private set; }

        public BondMeasureModelParameter BondMeasureModelParameter { get; private set; }

        public ObservableCollection<UserRegion> BondModelUserRegions { get; private set; }
        private IEnumerable<HObject> BondModelRegions => BondModelUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion);

        public ObservableCollection<UserRegion> Bond1AutoUserRegion { get; private set; } = new ObservableCollection<UserRegion>();
        private IEnumerable<HObject> Bond1AutoRegion => Bond1AutoUserRegion.Where(r => r.IsEnable).Select(r => r.CalculateRegion);

        public ObservableCollection<UserRegion> RotateLineUserRegion { get; private set; } = new ObservableCollection<UserRegion>();
        private IEnumerable<HObject> RotateLineRegion => RotateLineUserRegion.Where(r => r.IsEnable).Select(r => r.CalculateRegion);
        
        public Bond1AutoRegionsParameter Bond1AutoRegionsParameter { get; private set; }

        public ObservableCollection<UserRegion> Bond2UserRegion { get; private set; }

        public ObservableCollection<UserRegion> Bond2UserRegionDiff { get; private set; }

        HTuple ModelID = new HTuple();

        private HObject rotatedImage = null;

        private BondWireRegionGroup currentGroup;
        public BondWireRegionGroup CurrentGroup
        {
            get => currentGroup;
            set
            {
                if (currentGroup != value)
                {
                    currentGroup = value;
                    OnPropertyChanged();
                    DispalyGroupRegion();
                }
            }
        }

        private int groupsCount;
        public int GroupsCount
        {
            get => groupsCount;
            set => OnPropertyChanged(ref groupsCount, value);
        }

        private int switchImageComboBoxIndex;
        public int SwitchImageComboBoxIndex
        {
            get => switchImageComboBoxIndex;
            set
            {
                if (switchImageComboBoxIndex != value)
                {
                    if (!File.Exists(BondMeasureParameter.ImagePath) || !File.Exists($"{bondRecipeDirectory}\\RotatedImage.tiff"))
                    {
                        switchImageComboBoxIndex = value;
                        OnPropertyChanged();
                        return;
                    }
                    if (value == 0)
                    {
                        //htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex),true);
                        htWindow.DisplaySingleRegion(RotateLineUserRegion.Count == 0 ? null : RotateLineUserRegion[0].CalculateRegion, Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex));

                    }
                    else if (value == 1)
                    {
                        if (rotatedImage == null || !rotatedImage.IsInitialized())
                        {
                            if (File.Exists($"{bondRecipeDirectory}\\RotatedImage.tiff"))
                            {
                                HOperatorSet.ReadImage(out rotatedImage, $"{bondRecipeDirectory}\\RotatedImage.tiff");
                            }
                            else
                            {
                                MessageBox.Show("转正图像不存在!");
                                return;
                            }
                        }
                        //HOperatorSet.ReadImage(out rotatedImage, $"{bondRecipeDirectory}\\RotatedImage.tiff");
                        htWindow.DisplayMultiRegionTwo(Bond2UserRegion.Count==0?null: Bond2UserRegion[0].CalculateRegion, Bond2UserRegionDiff.Count == 0?null: Bond2UserRegionDiff[0].CalculateRegion, rotatedImage);
                    }
                    switchImageComboBoxIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<BondWireRegionGroup> Groups { get; private set; } = new ObservableCollection<BondWireRegionGroup>();

        private bool isSetAllParas = false;
        public bool IsSetAllParas
        {
            get => isSetAllParas;
            set => OnPropertyChanged(ref isSetAllParas, value);
        }

        private bool? isCheckAllAuto;
        public bool? IsCheckAllAuto
        {
            get => isCheckAllAuto;
            set => OnPropertyChanged(ref isCheckAllAuto, value);
        }

        private bool isMeasureRefine = false;
        public bool IsMeasureRefine
        {
            get => isMeasureRefine;
            set => OnPropertyChanged(ref isMeasureRefine, value);
        }

        private double measureContrast = 5;
        public double MeasureContrast
        {
            get => measureContrast;
            set => OnPropertyChanged(ref measureContrast, value);
        }

        private double mearsureLength = 4;
        public double MearsureLength
        {
            get => mearsureLength;
            set => OnPropertyChanged(ref mearsureLength, value);
        }

        private double mearsureWideth = 1;
        public double MearsureWideth
        {
            get => mearsureWideth;
            set => OnPropertyChanged(ref mearsureWideth, value);
        }

        private string mearsureTransition = "positive";
        /// 测量变换  ‘positive’,‘negative’
        public string MearsureTransition
        {
            get => mearsureTransition;
            set => OnPropertyChanged(ref mearsureTransition, value);
        }

        private double distanceThreshold = 3.5;
        /// <summary>
        /// 测量间隔宽度
        /// </summary>
        public double DistanceThreshold
        {
            get => distanceThreshold;
            set => OnPropertyChanged(ref distanceThreshold, value);
        }

        private string mearsureSelect = "all";
        /// <summary>
        /// 边缘点选择  ‘all’,‘first’,‘last’
        /// </summary>
        public string MearsureSelect
        {
            get => mearsureSelect;
            set => OnPropertyChanged(ref mearsureSelect, value);
        }

        public CommandBase LoadReferenceCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase IsCheckAutoCommand { get; private set; }
        public CommandBase IsCheckAllAutoCommand { get; private set; }
        public CommandBase AddBond2UserRegionCommand { get; private set; }
        public CommandBase ModifyBond2ModelRegionCommand { get; private set; }
        public CommandBase AddBond2UserRegionDiffCommand { get; private set; }
        public CommandBase ModifyBond2ModelRegionDiffCommand { get; private set; }
        public CommandBase AddBond2InspectUserRegionCommand { get; private set; }
        public CommandBase ModifyBond2InspectUserRegionCommand { get; private set; }
        public CommandBase AddWireInspectUserRegionCommand { get; private set; }
        public CommandBase ModifyWireInspectUserRegionCommand { get; private set; }
        public CommandBase DisplayAllInspectRegionsCommand { get; private set; }
        public CommandBase AddGroupCommand { get; private set; }
        public CommandBase RemoveGroupCommand { get; private set; }
        public CommandBase CreateAutoBondUserRegionCommand { get; private set; }
        public CommandBase TextChangedCommand { get; private set; }
        public CommandBase SortCommand { get; private set; }
        public CommandBase AddBond1AutoUserRegionCommand { get; private set; }
        public CommandBase ModifyBond1AutoRegionCommand { get; private set; }
        public CommandBase RemoveBond1AutoUserRegionCommand { get; private set; }
        public CommandBase DisplayBond1ModelRegionsCommand { get; private set; }
        public CommandBase DisplayAutoBond1RegionsCommand { get; private set; }
        public CommandBase CreateBond2ModelCommand { get; private set; }
        public CommandBase LoadAutoMeasureBondCommand { get; private set; }
        public CommandBase Rectangle1SelectCommand { get; private set; }
        public CommandBase RotateCommand { get; private set; }
        public CommandBase SelectedChangedImageCommand { get; private set; }
        public CommandBase OptimizeResultCommand { get; private set; }
        public CommandBase ModifyParametersBatchCommand { get; private set; }
        public CommandBase UpdateAutoMeasureBondCommand { get; private set; }

        private HTHalControlWPF htWindow;
        private readonly string bondRecipeDirectory;
        private readonly string ModelsRecipeDirectory;

        public CreateAutoBondMeasureModel(HTHalControlWPF htWindow,
                                     string modelsFile,
                                     string recipeFile,
                                     string referenceDirectory,
                                     ObservableCollection<BondWireRegionGroup> groups,
                                     BondMeasureModelObject BondMeasureModelObject,
                                     BondMeasureParameter BondMeasureParameter,
                                     Bond1AutoRegionsParameter Bond1AutoRegionsParameter,
                                     ObservableCollection<UserRegion> BondModelUserRegions,
                                     ObservableCollection<UserRegion> Bond1AutoUserRegion,
                                     ObservableCollection<UserRegion> RotateLineUserRegion,
                                     string bondRecipeDirctory,
                                     string modelsRecipeDirectory,
                                     ObservableCollection<UserRegion> Bond2UserRegion,
                                     ObservableCollection<UserRegion> Bond2UserRegionDiff)
        {
            DisplayName = "焊点测量区域自动生成";
            Content = new Page_CreateAutoBondMeasureModel { DataContext = this };
            this.htWindow = htWindow;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.ReferenceDirectory = referenceDirectory;
            this.Groups = groups;
            this.BondMeasureModelObject = BondMeasureModelObject;
            this.BondMeasureParameter = BondMeasureParameter;
            this.Bond1AutoRegionsParameter = Bond1AutoRegionsParameter;
            this.BondModelUserRegions = BondModelUserRegions;
            this.Bond1AutoUserRegion = Bond1AutoUserRegion;
            this.bondRecipeDirectory = bondRecipeDirctory;
            this.ModelsRecipeDirectory = modelsRecipeDirectory;
            this.RotateLineUserRegion = RotateLineUserRegion;
            this.Bond2UserRegion = Bond2UserRegion;
            this.Bond2UserRegionDiff = Bond2UserRegionDiff;

            LoadReferenceCommand = new CommandBase(ExecuteLoadReferenceCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            IsCheckAutoCommand = new CommandBase(ExecuteIsCheckAutoCommand);
            IsCheckAllAutoCommand = new CommandBase(ExecuteIsCheckAllAutoCommand);
            CreateBond2ModelCommand = new CommandBase(ExecuteCreateBond2ModelCommand);
            AddGroupCommand = new CommandBase(ExecuteAddGroupCommand);
            RemoveGroupCommand = new CommandBase(ExecuteRemoveGroupCommand);
            AddBond2UserRegionCommand = new CommandBase(ExecuteAddBond2UserRegionCommand);
            ModifyBond2ModelRegionCommand = new CommandBase(ExecuteModifyBond2ModelRegionCommand);
            AddBond2UserRegionDiffCommand = new CommandBase(ExecuteAddBond2UserRegionDiffCommand);
            ModifyBond2ModelRegionDiffCommand = new CommandBase(ExecuteModifyBond2ModelRegionDiffCommand);
            AddBond2InspectUserRegionCommand = new CommandBase(ExecuteAddBond2InspectUserRegionCommand);
            ModifyBond2InspectUserRegionCommand = new CommandBase(ExecuteModifyBond2InspectUserRegionCommand);
            AddWireInspectUserRegionCommand = new CommandBase(ExecuteAddWireInspectUserRegionCommand);
            ModifyWireInspectUserRegionCommand = new CommandBase(ExecuteModifyWireInspectUserRegionCommand);
            CreateAutoBondUserRegionCommand = new CommandBase(ExecuteCreateAutoBondUserRegionCommand);
            TextChangedCommand = new CommandBase(ExecuteTextChangedCommand);
            SortCommand = new CommandBase(ExecuteSortCommand);
            AddBond1AutoUserRegionCommand = new CommandBase(ExecuteAddBond1AutoUserRegionCommand);
            ModifyBond1AutoRegionCommand = new CommandBase(ExecuteModifyBond1AutoRegionCommand);
            RemoveBond1AutoUserRegionCommand = new CommandBase(ExecuteRemoveBond1AutoUserRegionCommand);
            DisplayAllInspectRegionsCommand = new CommandBase(ExecuteDisplayAllInspectRegionsCommand);
            DisplayBond1ModelRegionsCommand = new CommandBase(ExecuteDisplayBond1ModelRegionsCommand);
            DisplayAutoBond1RegionsCommand = new CommandBase(ExecuteDisplayAutoBond1RegionsCommand);
            LoadAutoMeasureBondCommand = new CommandBase(ExecuteLoadAutoMeasureBondCommand);
            Rectangle1SelectCommand = new CommandBase(ExecuteRectangle1SelectCommand);
            RotateCommand = new CommandBase(ExecuteRotateCommand);
            ModifyParametersBatchCommand = new CommandBase(ExecuteModifyParametersBatchCommand);
            OptimizeResultCommand = new CommandBase(ExecuteOptimizeResultCommand);
            UpdateAutoMeasureBondCommand = new CommandBase(ExecuteUpdateAutoMeasureBondCommand);
            // 1122-lw
            SelectedChangedImageCommand = new CommandBase(ExecuteSelectedChangedImageCommand);
        }

        //修正焊点自动生成结果
        private void ExecuteOptimizeResultCommand(object parameter)
        {
            if (isRightClick != true) return;//
            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载参考图像!");
                return;
            }
            if (BondModelUserRegions.Count() == 0)
            {
                MessageBox.Show("请添加测量模板区域！");
                return;
            }
            if (BondMeasureParameter.ImageCountChannels > 0 && BondMeasureParameter.ImageChannelIndex < 0)
            {
                MessageBox.Show("请先选择通道图像！");
                return;
            }

            HTuple _MetrologyType = new HTuple();
            HTuple MetrologyType = new HTuple();
            HTuple MetrologyContrast = new HTuple();
            HTuple MetrologyPara = new HTuple();
            HTuple MearsureLength = new HTuple();
            HTuple MearsureWideth = new HTuple();
            HTuple MearsureTransition = new HTuple();
            HTuple DistanceThreshold = new HTuple();
            HTuple MearsureSelect = new HTuple();

            try
            {
                // 创建测量模板
                foreach (var bondRegon in BondModelUserRegions)
                {
                    switch (bondRegon.RegionType)
                    {
                        case RegionType.Line:
                            _MetrologyType = 1;
                            break;
                        case RegionType.Rectangle2:
                            _MetrologyType = 2;
                            break;
                        case RegionType.Circle:
                            _MetrologyType = 3;
                            break;
                        case RegionType.Ellipse:
                            _MetrologyType = 4;
                            break;
                        default:
                            MessageBox.Show("请重新选择画区域工具！");
                            return;
                    }

                    MetrologyType = MetrologyType.TupleConcat(_MetrologyType);
                    MetrologyContrast = MetrologyContrast.TupleConcat(bondRegon.BondMeasureModelParameter.MeasureContrast);
                    MearsureLength = MearsureLength.TupleConcat(bondRegon.BondMeasureModelParameter.MearsureLength);
                    MearsureWideth = MearsureWideth.TupleConcat(bondRegon.BondMeasureModelParameter.MearsureWideth);
                    MearsureTransition = MearsureTransition.TupleConcat(bondRegon.BondMeasureModelParameter.MearsureTransition);
                    DistanceThreshold = DistanceThreshold.TupleConcat(bondRegon.BondMeasureModelParameter.DistanceThreshold);
                    MearsureSelect = MearsureSelect.TupleConcat(bondRegon.BondMeasureModelParameter.MearsureSelect);
                    MetrologyPara = MetrologyPara.TupleConcat(bondRegon.RegionParameters);
                }
             
                Algorithm.Model_RegionAlg.HTV_Create_Metrology_Model(MetrologyType,
                                                           MetrologyContrast,
                                                           MearsureLength,
                                                           MearsureWideth,
                                                           MearsureTransition,
                                                           MetrologyPara,
                                                           DistanceThreshold,
                                                           MearsureSelect,
                                                           out HTuple MetrologyHandle,
                                                           out HTuple ErrCode,
                                                           out HTuple ErrStr);

                BondMeasureModelObject.MetrologyHandle = MetrologyHandle;

                if (BondMeasureModelObject.MetrologyHandle.TupleLength() > 0)
                {
                    // 测量检测
                    HOperatorSet.GenEmptyObj(out HObject NewContour);
                    HOperatorSet.ApplyMetrologyModel(Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex),
                                                     BondMeasureModelObject.MetrologyHandle);
                    HOperatorSet.GetMetrologyObjectResultContour(out NewContour, BondMeasureModelObject.MetrologyHandle, "all", "all", 1.5);

                    HOperatorSet.GetMetrologyObjectIndices(BondMeasureModelObject.MetrologyHandle, out HTuple hv_Indices);
                    //HOperatorSet.GetMetrologyObjectResult(BondMeasureModelObject.MetrologyHandle, hv_Indices, "all", "result_type", "all_param", out HTuple hv_Parameter);
                    //htWindow.DisplayMultiRegion(NewContour);

                    // 将结果更新至Bond1AutoUserRegion
                    if (NewContour.CountObj() == 0)
                    {
                        MessageBox.Show("测量修正失败！");
                        return;
                    }

                    ObservableCollection<UserRegion> Bond1AutoUserRegionTmp = new ObservableCollection<UserRegion>(Bond1AutoUserRegion);
                    Bond1AutoUserRegion.Clear();

                    if (Bond1AutoRegionsParameter.IsCircleShape == true)
                    {
                        HTuple hv_toolNum = new HTuple(hv_Indices.TupleLength());
                        HTuple end_val140 = hv_toolNum - 1;
                        HTuple step_val140 = 1;
                        HTuple hv_idx = 0;
                        for (hv_idx = 0; hv_idx.Continue(end_val140, step_val140); hv_idx = hv_idx.TupleAdd(step_val140))
                        {
                            // 更新测量得到的精确区域
                            HOperatorSet.GetMetrologyObjectResult(BondMeasureModelObject.MetrologyHandle, hv_Indices.TupleSelect(
                                hv_idx), "all", "result_type", "all_param", out HTuple hv_Parameter);

                            if ((int)(new HTuple((new HTuple(hv_Parameter.TupleLength())).TupleLess(3))) != 0)
                            {
                                // 测量失败用原来的区域
                                UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Circle,
                                                                            Bond1AutoUserRegionTmp[hv_idx].RegionParameters[0],
                                                                            Bond1AutoUserRegionTmp[hv_idx].RegionParameters[1],
                                                                            Bond1AutoUserRegionTmp[hv_idx].RegionParameters[2],
                                                                            0,
                                                                            0, 0, 0);
                                if (userRegion_Circle == null) return;
                                userRegion_Circle.Index = Bond1AutoUserRegion.Count + 1;
                                Bond1AutoUserRegion.Add(userRegion_Circle);

                            }
                            else
                            { 
                                UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Circle,
                                                                            hv_Parameter.TupleSelect(0),
                                                                            hv_Parameter.TupleSelect(1),
                                                                            hv_Parameter.TupleSelect(2),
                                                                            0,
                                                                            0, 0, 0);
                                if (userRegion_Circle == null) return;
                                userRegion_Circle.Index = Bond1AutoUserRegion.Count + 1;
                                Bond1AutoUserRegion.Add(userRegion_Circle);
                            }
                        }
                    }
                    else
                    {
                        HTuple hv_toolNum = new HTuple(hv_Indices.TupleLength());
                        HTuple end_val140 = hv_toolNum - 1;
                        HTuple step_val140 = 1;
                        HTuple hv_idx = 0;
                        for (hv_idx = 0; hv_idx.Continue(end_val140, step_val140); hv_idx = hv_idx.TupleAdd(step_val140))
                        {
                            // 更新测量得到的精确区域
                            HOperatorSet.GetMetrologyObjectResult(BondMeasureModelObject.MetrologyHandle, hv_Indices.TupleSelect(
                                hv_idx), "all", "result_type", "all_param", out HTuple hv_Parameter);

                            if ((int)(new HTuple((new HTuple(hv_Parameter.TupleLength())).TupleLess(3))) != 0)
                            {
                                // 测量失败用原来的区域
                                UserRegion userRegion_Ellips = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Ellipse,
                                                                            Bond1AutoUserRegionTmp[hv_idx].RegionParameters[0],
                                                                            Bond1AutoUserRegionTmp[hv_idx].RegionParameters[1],
                                                                            Bond1AutoUserRegionTmp[hv_idx].RegionParameters[3],
                                                                            Bond1AutoUserRegionTmp[hv_idx].RegionParameters[4],
                                                                            0, 0, Bond1AutoUserRegionTmp[hv_idx].RegionParameters[2]);
                                if (userRegion_Ellips == null) return;
                                userRegion_Ellips.Index = Bond1AutoUserRegion.Count + 1;
                                Bond1AutoUserRegion.Add(userRegion_Ellips);

                            }
                            else
                            {
                                UserRegion userRegion_Ellips = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Ellipse,
                                                                            hv_Parameter.TupleSelect(0),
                                                                            hv_Parameter.TupleSelect(1),
                                                                            hv_Parameter.TupleSelect(3),
                                                                            hv_Parameter.TupleSelect(4),
                                                                            0, 0, hv_Parameter.TupleSelect(2));
                                if (userRegion_Ellips == null) return;
                                userRegion_Ellips.Index = Bond1AutoUserRegion.Count + 1;
                                Bond1AutoUserRegion.Add(userRegion_Ellips);
                            }
                        }
                    }
                    DispalyGroupsRegions();

                    HOperatorSet.ClearMetrologyModel(BondMeasureModelObject.MetrologyHandle);
                }
                else
                {
                    MessageBox.Show("创建测量模板失败！");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteModifyParametersBatchCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (BondModelUserRegions.Count() == 0)
            {
                MessageBox.Show("请添加测量模板区域！");
                return;
            }
            try
            {

                if (MessageBox.Show("是否进行参数批量修改?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    for (int i = 0; i < BondModelUserRegions.Count; i++)
                    {
                        if (BondModelUserRegions[i].IsSelected)
                        {
                            BondModelUserRegions[i].BondMeasureModelParameter.MeasureContrast = measureContrast;
                            BondModelUserRegions[i].BondMeasureModelParameter.MearsureLength = mearsureLength;
                            BondModelUserRegions[i].BondMeasureModelParameter.MearsureWideth = mearsureWideth;
                            BondModelUserRegions[i].BondMeasureModelParameter.MearsureTransition = mearsureTransition;
                            BondModelUserRegions[i].BondMeasureModelParameter.DistanceThreshold = distanceThreshold;
                            BondModelUserRegions[i].BondMeasureModelParameter.MearsureSelect = mearsureSelect;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void ExecuteDisplayBond1ModelRegionsCommand(object parameter)
        {
            DisplayBond1ModelRegions();
        }

        private void DisplayBond1ModelRegions(bool isHTWindowRegion = true)
        {
            try
            {
                if (rotatedImage == null || !rotatedImage.IsInitialized())
                {
                    if (File.Exists($"{bondRecipeDirectory}\\RotatedImage.tiff"))
                    {
                        HOperatorSet.ReadImage(out rotatedImage, $"{bondRecipeDirectory}\\RotatedImage.tiff");
                    }
                    else
                    {
                        MessageBox.Show("转正图像不存在!");
                        return;
                    }
                }
                SwitchToRotatedImage();

                if (Bond2UserRegion.Count == 0)
                {
                    //htWindow.Display(BondMeasureModelObject.Image, true);
                    return;
                }
                HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
                HOperatorSet.ConcatObj(concatGroupRegion, Bond2UserRegion[0].DisplayRegion, out concatGroupRegion);
                if (Bond2UserRegionDiff.Count > 0)
                {
                    HOperatorSet.ConcatObj(concatGroupRegion, Bond2UserRegionDiff[0].DisplayRegion, out concatGroupRegion);                    
                }

                if (isHTWindowRegion)
                {
                    htWindow.DisplaySingleRegion(concatGroupRegion);
                }
                else
                {
                    htWindow.DisplaySingleRegion(concatGroupRegion, rotatedImage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void ExecuteSelectedChangedImageCommand(object parameter)
        {
            // 1122
            if (BondMeasureParameter.ImageChannelIndex >= 0)
            {
                BondMeasureModelObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex);
                ImageChannelIndex = BondMeasureParameter.ImageChannelIndex;
                htWindow.Display(BondMeasureModelObject.ChannelImage);
                OnPropertyChanged();
            }
            else
            {
                BondMeasureParameter.ImageChannelIndex = ImageChannelIndex;
                OnPropertyChanged();
            }
        }

        //创建焊点匹配模板

        //添加区域
        private void ExecuteAddBond2UserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像！");
                return;
            }
            if (SwitchImageComboBoxIndex != 1)
            {
                SwitchToRotatedImage();
                MessageBox.Show("在转正图中画模板区域！");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow);
                if (userRegion == null) return;
                Bond2UserRegion.Clear();
                Bond2UserRegion.Add(userRegion);
                htWindow.DisplayMultiRegionTwo(Bond2UserRegion[0].CalculateRegion, Bond2UserRegionDiff.Count == 0 ? null : Bond2UserRegionDiff[0].CalculateRegion);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                htWindow.RegionType = RegionType.Null;
            }
        }

        private void ExecuteModifyBond2ModelRegionCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }
            if (SwitchImageComboBoxIndex != 1)
            {
                SwitchToRotatedImage();
            }
            if (Bond2UserRegion.Count == 0) return;
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    switch (Bond2UserRegion[0].RegionType)
                    {
                        case RegionType.Point:
                            break;

                        case RegionType.Rectangle1:
                            htWindow.InitialHWindowUpdate(Math.Floor(Bond2UserRegion[0].RegionParameters[0]),
                                                  Math.Floor(Bond2UserRegion[0].RegionParameters[1]),
                                                  Math.Ceiling(Bond2UserRegion[0].RegionParameters[2]),
                                                  Math.Ceiling(Bond2UserRegion[0].RegionParameters[3]),
                                                  Bond2UserRegion[0].RegionType, 0, 0, "yellow");
                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow,(Bond2UserRegion[0].RegionParameters[0]),
                                                                                          (Bond2UserRegion[0].RegionParameters[1]),
                                                                                          (Bond2UserRegion[0].RegionParameters[2]),
                                                                                          (Bond2UserRegion[0].RegionParameters[3]),
                                                                out HTuple row1_Rectangle,
                                                                out HTuple column1_Rectangle,
                                                                out HTuple row2_Rectangle,
                                                                out HTuple column2_Rectangle);

                            Bond2UserRegion[0].RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, Bond2UserRegion[0].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle);
                            if (userRegion == null) return;
                            Bond2UserRegion[0] = userRegion;
                            htWindow.DisplayMultiRegionTwo(Bond2UserRegion[0].CalculateRegion, Bond2UserRegionDiff.Count==0 ? null: Bond2UserRegionDiff[0].CalculateRegion);
                            break;

                        case RegionType.Rectangle2:
                            htWindow.InitialHWindowUpdate(Math.Floor(Bond2UserRegion[0].RegionParameters[0]),
                                                          Math.Floor(Bond2UserRegion[0].RegionParameters[1]),
                                                          Math.Ceiling(Bond2UserRegion[0].RegionParameters[2]),
                                                          Math.Ceiling(Bond2UserRegion[0].RegionParameters[3]),
                                                          Bond2UserRegion[0].RegionType,
                                                          0, 0, "yellow");

                            HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                           Math.Floor(Bond2UserRegion[0].RegionParameters[0]),
                                                           Math.Floor(Bond2UserRegion[0].RegionParameters[1]),
                                                           Bond2UserRegion[0].RegionParameters[2],
                                                           Math.Ceiling(Bond2UserRegion[0].RegionParameters[3]),
                                                           Math.Ceiling(Bond2UserRegion[0].RegionParameters[4]),
                                                        out HTuple row_Rectangle2,
                                                        out HTuple column_Rectangle2,
                                                        out HTuple phi_Rectangle2,
                                                        out HTuple lenth1_Rectangle2,
                                                        out HTuple lenth2_Rectangle2);

                            Bond2UserRegion[0].RegionType = RegionType.Rectangle2;
                            UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, Bond2UserRegion[0].RegionType,
                                                                                                 row_Rectangle2, column_Rectangle2,
                                                                                                 lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                 0,
                                                                                                 0,
                                                                                                 phi_Rectangle2);
                            if (userRegion_Rectangle2 == null) return;
                            Bond2UserRegion[0] = userRegion_Rectangle2;
                            htWindow.DisplayMultiRegionTwo(Bond2UserRegion[0].CalculateRegion, Bond2UserRegionDiff.Count == 0 ? null : Bond2UserRegionDiff[0].CalculateRegion);
                            break;

                        case RegionType.Circle:
                            htWindow.InitialHWindowUpdate((Bond2UserRegion[0].RegionParameters[0]),
                              (Bond2UserRegion[0].RegionParameters[1]),
                              (Bond2UserRegion[0].RegionParameters[2]),
                              0, Bond2UserRegion[0].RegionType, 0, 0, "yellow");

                            HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                       Bond2UserRegion[0].RegionParameters[0] - 0,
                                                       Bond2UserRegion[0].RegionParameters[1] - 0,
                                                       Bond2UserRegion[0].RegionParameters[2],
                                                   out HTuple row_Circle,
                                                   out HTuple column_Circle,
                                                   out HTuple radius_Circle);

                            Bond2UserRegion[0].RegionType = RegionType.Circle;
                            UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                             Bond2UserRegion[0].RegionType,
                                                                                             row_Circle, column_Circle, radius_Circle, 0, 0, 0, 0);
                            if (userRegion_Circle == null) return;
                            Bond2UserRegion[0] = userRegion_Circle;
                            htWindow.DisplayMultiRegionTwo(Bond2UserRegion[0].CalculateRegion, Bond2UserRegionDiff.Count == 0 ? null : Bond2UserRegionDiff[0].CalculateRegion);
                            break;

                        case RegionType.Ellipse:
                            htWindow.InitialHWindowUpdate(Math.Floor(Bond2UserRegion[0].RegionParameters[0]),
                                                          Math.Floor(Bond2UserRegion[0].RegionParameters[1]),
                                                          Bond2UserRegion[0].RegionParameters[2],
                                                          Math.Ceiling(Bond2UserRegion[0].RegionParameters[3]),
                                                           Bond2UserRegion[0].RegionType, 0, 0, "yellow");

                            HOperatorSet.DrawEllipseMod(htWindow.hTWindow.HalconWindow,
                                                       Math.Floor(Bond2UserRegion[0].RegionParameters[0] - 0),
                                                       Math.Floor(Bond2UserRegion[0].RegionParameters[1] - 0),
                                                       Bond2UserRegion[0].RegionParameters[2],
                                                       Math.Floor(Bond2UserRegion[0].RegionParameters[3] - 0),
                                                       Math.Floor(Bond2UserRegion[0].RegionParameters[4] - 0),
                                                   out HTuple row1,
                                                   out HTuple column1,
                                                   out HTuple phi,
                                                   out HTuple radius1,
                                                   out HTuple radius2);

                            Bond2UserRegion[0].RegionType = RegionType.Ellipse;
                            UserRegion userRegion_Ellipse = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                             Bond2UserRegion[0].RegionType,
                                                                                             row1, column1, radius1, radius2, 0, 0, phi);
                            if (userRegion_Ellipse == null) return;
                            Bond2UserRegion[0] = userRegion_Ellipse;
                            htWindow.DisplayMultiRegionTwo(Bond2UserRegion[0].CalculateRegion, Bond2UserRegionDiff.Count == 0 ? null : Bond2UserRegionDiff[0].CalculateRegion);
                            break;

                        default: break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        //添加拒绝区
        private void ExecuteAddBond2UserRegionDiffCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }
            if (SwitchImageComboBoxIndex != 1)
            {
                SwitchToRotatedImage();
                MessageBox.Show("在转正图中画模板拒绝区域！");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow);
                if (userRegion == null) return;
                userRegion.IsAccept = false;
                Bond2UserRegionDiff.Add(userRegion);
                htWindow.DisplayMultiRegionTwo(Bond2UserRegion[0]?.CalculateRegion, Bond2UserRegionDiff[0].CalculateRegion,rotatedImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                htWindow.RegionType = RegionType.Null;
            }
        }

        private void ExecuteModifyBond2ModelRegionDiffCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }
            if (Bond2UserRegionDiff.Count<1) return;
            if (SwitchImageComboBoxIndex != 1)
            {
                SwitchToRotatedImage();
            }
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    switch (Bond2UserRegionDiff[0].RegionType)
                    {
                        case RegionType.Point:
                            break;

                        case RegionType.Rectangle1:
                            htWindow.InitialHWindowUpdate(Math.Floor(Bond2UserRegionDiff[0].RegionParameters[0]),
                                                          Math.Floor(Bond2UserRegionDiff[0].RegionParameters[1]),
                                                          Math.Ceiling(Bond2UserRegionDiff[0].RegionParameters[2]),
                                                          Math.Ceiling(Bond2UserRegionDiff[0].RegionParameters[3]),
                                                          Bond2UserRegionDiff[0].RegionType, 0, 0, "yellow");
                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow,
                                                           (Bond2UserRegionDiff[0].RegionParameters[0]),
                                                           (Bond2UserRegionDiff[0].RegionParameters[1]),
                                                           (Bond2UserRegionDiff[0].RegionParameters[2]),
                                                           (Bond2UserRegionDiff[0].RegionParameters[3]),
                                                       out HTuple row1_Rectangle,
                                                       out HTuple column1_Rectangle,
                                                       out HTuple row2_Rectangle,
                                                       out HTuple column2_Rectangle);

                            Bond2UserRegionDiff[0].RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, Bond2UserRegionDiff[0].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle);
                            if (userRegion == null) return;
                            userRegion.IsAccept = false;
                            Bond2UserRegionDiff[0] = userRegion;
                            htWindow.DisplayMultiRegionTwo(Bond2UserRegion[0].CalculateRegion, Bond2UserRegionDiff[0]?.CalculateRegion);
                            break;

                        case RegionType.Rectangle2:
                            htWindow.InitialHWindowUpdate(Math.Floor(Bond2UserRegionDiff[0].RegionParameters[0]),
                                                          Math.Floor(Bond2UserRegionDiff[0].RegionParameters[1]),
                                                          Math.Ceiling(Bond2UserRegionDiff[0].RegionParameters[2]),
                                                          Math.Ceiling(Bond2UserRegionDiff[0].RegionParameters[3]),
                                                          Bond2UserRegionDiff[0].RegionType,
                                                          0, 0, "yellow");

                            HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                           Math.Floor(Bond2UserRegionDiff[0].RegionParameters[0]),
                                                           Math.Floor(Bond2UserRegionDiff[0].RegionParameters[1]),
                                                           Bond2UserRegionDiff[0].RegionParameters[2],
                                                           Math.Ceiling(Bond2UserRegionDiff[0].RegionParameters[3]),
                                                           Math.Ceiling(Bond2UserRegionDiff[0].RegionParameters[4]),
                                                        out HTuple row_Rectangle2,
                                                        out HTuple column_Rectangle2,
                                                        out HTuple phi_Rectangle2,
                                                        out HTuple lenth1_Rectangle2,
                                                        out HTuple lenth2_Rectangle2);

                            Bond2UserRegionDiff[0].RegionType = RegionType.Rectangle2;
                            UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, Bond2UserRegionDiff[0].RegionType,
                                                                                                 row_Rectangle2, column_Rectangle2,
                                                                                                 lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                 0,
                                                                                                 0,
                                                                                                 phi_Rectangle2);
                            if (userRegion_Rectangle2 == null) return;
                            Bond2UserRegionDiff[0] = userRegion_Rectangle2;
                            htWindow.DisplayMultiRegionTwo(Bond2UserRegion[0].CalculateRegion, Bond2UserRegionDiff[0]?.CalculateRegion);
                            break;

                        case RegionType.Circle:
                            htWindow.InitialHWindowUpdate((Bond2UserRegionDiff[0].RegionParameters[0]),
                               (Bond2UserRegionDiff[0].RegionParameters[1]),
                               (Bond2UserRegionDiff[0].RegionParameters[2]),
                               0, Bond2UserRegionDiff[0].RegionType, 0, 0, "yellow");

                            HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                      (Bond2UserRegionDiff[0].RegionParameters[0] - 0),
                                                      (Bond2UserRegionDiff[0].RegionParameters[1] - 0),
                                                       Bond2UserRegionDiff[0].RegionParameters[2],
                                                   out HTuple row_Circle,
                                                   out HTuple column_Circle,
                                                   out HTuple radius_Circle);

                            Bond2UserRegionDiff[0].RegionType = RegionType.Circle;
                            UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                             Bond2UserRegionDiff[0].RegionType,
                                                                                             row_Circle, column_Circle, radius_Circle, 0, 0, 0, 0);
                            if (userRegion_Circle == null) return;
                            userRegion_Circle.IsAccept = false;
                            Bond2UserRegionDiff[0] = userRegion_Circle;
                            htWindow.DisplayMultiRegionTwo(Bond2UserRegion[0].CalculateRegion, Bond2UserRegionDiff[0]?.CalculateRegion);
                            break;

                        case RegionType.Ellipse:

                            htWindow.InitialHWindowUpdate(Math.Floor(Bond2UserRegionDiff[0].RegionParameters[0]),
                                                          Math.Floor(Bond2UserRegionDiff[0].RegionParameters[1]),
                                                           Bond2UserRegionDiff[0].RegionParameters[2],
                                                          Math.Ceiling(Bond2UserRegionDiff[0].RegionParameters[3]),
                                                            Bond2UserRegionDiff[0].RegionType, 0, 0, "yellow");

                            HOperatorSet.DrawEllipseMod(htWindow.hTWindow.HalconWindow,
                                                       Math.Floor(Bond2UserRegionDiff[0].RegionParameters[0] - 0),
                                                       Math.Floor(Bond2UserRegionDiff[0].RegionParameters[1] - 0),
                                                        Bond2UserRegionDiff[0].RegionParameters[2],
                                                       Math.Floor(Bond2UserRegionDiff[0].RegionParameters[3] - 0),
                                                       Math.Floor(Bond2UserRegionDiff[0].RegionParameters[4] - 0),
                                                   out HTuple row1,
                                                   out HTuple column1,
                                                   out HTuple phi,
                                                   out HTuple radius1,
                                                   out HTuple radius2);

                            Bond2UserRegionDiff[0].RegionType = RegionType.Ellipse;
                            UserRegion userRegion_Ellipse = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                              Bond2UserRegionDiff[0].RegionType,
                                                                                             row1, column1, radius1, radius2, 0, 0, phi);
                            if (userRegion_Ellipse == null) return;
                            Bond2UserRegionDiff[0] = userRegion_Ellipse;
                            htWindow.DisplayMultiRegionTwo(Bond2UserRegion[0]?.CalculateRegion, Bond2UserRegionDiff[0].CalculateRegion);
                            break;

                        default: break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    isRightClick = true;
                }
            }

        }

        //转正图
        private void ExecuteRotateCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像!");
                return;
            }
            if (SwitchImageComboBoxIndex != 0)
            {
                MessageBox.Show("请切换到原图再进行操作");
                return;
            }
            if (htWindow.RegionType != RegionType.Line)
            {
                MessageBox.Show("请选择画直线按钮重新画一条直线！");
                return;
            }
            rotatedImage?.Dispose();
            rotatedImage = null;
            HOperatorSet.GenEmptyObj(out HObject displayRegion);

            try
            {
                Algorithm.File.RotateImage(Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex),
                                           out rotatedImage,
                                           htWindow.Row1_Line,
                                           htWindow.Column1_Line,
                                           htWindow.Row2_Line,
                                           htWindow.Column2_Line);

                UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, htWindow.RegionType, htWindow.Row1_Line, htWindow.Column1_Line, htWindow.Row2_Line, htWindow.Column2_Line);
                if (userRegion == null) return;
                RotateLineUserRegion.Clear();
                RotateLineUserRegion.Add(userRegion);

                HOperatorSet.LineOrientation(htWindow.Row1_Line, htWindow.Column1_Line, htWindow.Row2_Line, htWindow.Column2_Line, out HTuple AngleRotate);
                Bond1AutoRegionsParameter.RotatedImageAngel = AngleRotate.D;

                Bond1AutoRegionsParameter.RotatedImagePath = $"{bondRecipeDirectory}RotatedImage.tiff";
                HOperatorSet.WriteImage(rotatedImage, "tiff", 0, Bond1AutoRegionsParameter.RotatedImagePath);

                Bond2UserRegion.Clear();
                Bond2UserRegionDiff.Clear();

                htWindow.Display(rotatedImage, true);
                SwitchToRotatedImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                htWindow.RegionType = RegionType.Null;//
            }
        }

        //匹配焊点  模板创建
        private void ExecuteCreateBond2ModelCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("没有参考图像!");
                return;
            }
            if (BondMeasureParameter.ImageCountChannels > 0 && BondMeasureParameter.ImageChannelIndex < 0)
            {
                MessageBox.Show("请先选择通道图像！");
                return;
            }
            try
            {
                if (rotatedImage == null || !rotatedImage.IsInitialized())
                {
                    if (File.Exists($"{bondRecipeDirectory}\\RotatedImage.tiff"))
                    {
                        HOperatorSet.ReadImage(out rotatedImage, $"{bondRecipeDirectory}\\RotatedImage.tiff");
                    }
                    else
                    {
                        MessageBox.Show("转正图像不存在!");
                        return;
                    }
                }

                if (Bond2UserRegion.Count == 0)
                {
                    return;
                }
                //Window_Loading window_Loading = new Window_Loading("正在生成焊点模板");
                //window_Loading.Show();

                HTuple _modelId = new HTuple();

                HOperatorSet.GenEmptyObj(out HObject bond2UserRegionCalculation);
                HOperatorSet.GenEmptyObj(out HObject refineRegions);

                if (Bond2UserRegionDiff.Count > 0)
                {
                    HOperatorSet.ConcatObj(bond2UserRegionCalculation, Algorithm.Region.ConcatRegion(Bond2UserRegion[0], Bond2UserRegionDiff[0]), out bond2UserRegionCalculation);
                }
                else
                {
                    HOperatorSet.ConcatObj(bond2UserRegionCalculation, Bond2UserRegion[0].CalculateRegion, out bond2UserRegionCalculation);
                }

                //接口替换
                Algorithm.Model_RegionAlg.HTV_GenBond2Model_Recipe(Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex),
                                                         rotatedImage,//rotatedImages,concact
                                                         bond2UserRegionCalculation,//concact
                                                         refineRegions,//concact
                                                         0,//BondMeasureParameter.ModelType
                                                         1,//ModelsCount
                                                         0,// BondMeasureParameter.IsPreProcess == false ? 0 : 1,//
                                                         0,//BondMeasureParameter.Gamma
                                                         out _modelId);

                ModelID = _modelId;
                MessageBox.Show("焊点匹配模板已生成！");
            }
            catch (Exception ex)
            {
                //window_Loading.Close();
                MessageBox.Show(ex.ToString(), "模板没有生成");
            }
            finally
            {
                //HOperatorSet.ClearNccModel(_modelId);
            }
        }

        //添加焊点检测区域组
        private void ExecuteAddGroupCommand(object parameter)
        {
            if (isRightClick != true) return;
            CurrentGroup = new BondWireRegionGroup
            {
                Index = Groups.Count + 1
            };
            Groups.Add(CurrentGroup);
            GroupsCount = Groups.Count;
            DispalyGroupRegion();
            //MessageBox.Show($"新建了序号 {CurrentGroup.Index.ToString()} 的焊点金线组合");  //改
        }

        private void ExecuteRemoveGroupCommand(object parameter)
        {
            if (isRightClick != true) return;//
            if (CurrentGroup == null) return;
            if (MessageBox.Show($"是否删除序号 {CurrentGroup.Index.ToString()} 的焊点金线组合", "删除", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;
            for (int i = CurrentGroup.Index; i < Groups.Count; i++)
            {
                Groups[i].Index--;
            }
            Groups.Remove(CurrentGroup);
            GroupsCount = Groups.Count;
            CurrentGroup = null;
            DispalyGroupRegion();
        }

        private void ExecuteAddBond2InspectUserRegionCommand(object parameter)
        {
            if (htWindow.RegionType == RegionType.Null)
            {
                System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                return;
            }
            if (CurrentGroup==null)
            {
                MessageBox.Show("请选择或者新建一个焊点检测区域组！");
                return;
            }
            if (GroupsCount < 1)
            {
                MessageBox.Show("请先创建焊点检测区域组！");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow);
                if (userRegion == null) return;
                CurrentGroup.Bond2UserRegion = userRegion;
                DispalyGroupRegion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                isRightClick = true;
            }
        }

        private void ExecuteModifyBond2InspectUserRegionCommand(object parameter)
        {
            if (currentGroup?.Bond2UserRegion == null) return;
            if (isRightClick)
            {
                isRightClick = false;
                htWindow.RegionType = RegionType.Null;
                try
                {
                    switch (currentGroup.Bond2UserRegion.RegionType)
                    {
                        case RegionType.Point:
                            break;

                        case RegionType.Rectangle1:
                            htWindow.InitialHWindowUpdate(Math.Floor(currentGroup.Bond2UserRegion.RegionParameters[0]),
                                                          Math.Floor(currentGroup.Bond2UserRegion.RegionParameters[1]),
                                                          Math.Ceiling(currentGroup.Bond2UserRegion.RegionParameters[2]),
                                                          Math.Ceiling(currentGroup.Bond2UserRegion.RegionParameters[3]), currentGroup.Bond2UserRegion.RegionType,
                                                           0, 0, "yellow");

                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (currentGroup.Bond2UserRegion.RegionParameters[0]),
                                                                                          (currentGroup.Bond2UserRegion.RegionParameters[1]),
                                                                                          (currentGroup.Bond2UserRegion.RegionParameters[2]),
                                                                                          (currentGroup.Bond2UserRegion.RegionParameters[3]),
                                                                                        out HTuple row1_Rectangle,
                                                                                        out HTuple column1_Rectangle,
                                                                                        out HTuple row2_Rectangle,
                                                                                        out HTuple column2_Rectangle);

                            currentGroup.Bond2UserRegion.RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.Bond2UserRegion.RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, 0, 0);
                            if (userRegion == null) return;
                            currentGroup.Bond2UserRegion = userRegion;
                            DispalyGroupRegion(true);
                            break;

                        case RegionType.Rectangle2:
                            htWindow.InitialHWindowUpdate(Math.Floor(currentGroup.Bond2UserRegion.RegionParameters[0]),
                                                          Math.Floor(currentGroup.Bond2UserRegion.RegionParameters[1]),
                                                          Math.Ceiling(currentGroup.Bond2UserRegion.RegionParameters[2]),
                                                          Math.Ceiling(currentGroup.Bond2UserRegion.RegionParameters[3]),
                                                          currentGroup.Bond2UserRegion.RegionType,
                                                          BondMeasureParameter.DieImageRowOffset, BondMeasureParameter.DieImageColumnOffset, "yellow");

                            HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                           Math.Floor(currentGroup.Bond2UserRegion.RegionParameters[0]),
                                                           Math.Floor(currentGroup.Bond2UserRegion.RegionParameters[1]),
                                                           currentGroup.Bond2UserRegion.RegionParameters[2],
                                                           Math.Ceiling(currentGroup.Bond2UserRegion.RegionParameters[3]),
                                                           Math.Ceiling(currentGroup.Bond2UserRegion.RegionParameters[4]),
                                                        out HTuple row_Rectangle2,
                                                        out HTuple column_Rectangle2,
                                                        out HTuple phi_Rectangle2,
                                                        out HTuple lenth1_Rectangle2,
                                                        out HTuple lenth2_Rectangle2);

                            currentGroup.Bond2UserRegion.RegionType = RegionType.Rectangle2;
                            UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.Bond2UserRegion.RegionType,
                                                                                                 row_Rectangle2, column_Rectangle2,
                                                                                                 lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                 0,
                                                                                                 0,
                                                                                                 phi_Rectangle2);
                            if (userRegion_Rectangle2 == null) return;
                            currentGroup.Bond2UserRegion = userRegion_Rectangle2;
                            DispalyGroupRegion(true);
                            break;


                        case RegionType.Circle:
                            break;

                        case RegionType.Ellipse:
                            break;

                        default: break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void ExecuteAddWireInspectUserRegionCommand(object parameter)
        {
            if (CurrentGroup == null)
            {
                MessageBox.Show("请选择或者新建一个焊点检测区域组！");
                return;
            }
            if (CurrentGroup == null)
            {
                MessageBox.Show("请选择或者新建一个焊点检测区域组！");
                return;
            }
            if (htWindow.RegionType == RegionType.Null)
            {
                System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                return;
            }
            try
            {

                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow, 0, 0);
                if (userRegion == null) return;
                CurrentGroup.WireUserRegion = userRegion;
                DispalyGroupRegion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                isRightClick = true;
            }
        }

        private void ExecuteModifyWireInspectUserRegionCommand(object parameter)//
        {
            if (currentGroup?.WireUserRegion == null) return;

            if (isRightClick)
            {
                isRightClick = false;
                htWindow.RegionType = RegionType.Null;
                try
                {
                    switch (currentGroup.WireUserRegion.RegionType)
                    {
                        case RegionType.Point:
                            break;

                        case RegionType.Line:
                            htWindow.InitialHWindowUpdate(Math.Floor(currentGroup.WireUserRegion.RegionParameters[0]),
                            currentGroup.WireUserRegion.RegionParameters[1],
                            currentGroup.WireUserRegion.RegionParameters[2],
                            currentGroup.WireUserRegion.RegionParameters[3], currentGroup.WireUserRegion.RegionType,
                            0, 0, "yellow");
                            HOperatorSet.DrawLineMod(htWindow.hTWindow.HalconWindow, currentGroup.WireUserRegion.RegionParameters[0],
                                                                                           currentGroup.WireUserRegion.RegionParameters[1],
                                                                                           currentGroup.WireUserRegion.RegionParameters[2],
                                                                                           currentGroup.WireUserRegion.RegionParameters[3],
                                                       out HTuple row1_Line,
                                                       out HTuple column1_Line,
                                                       out HTuple row2_Line,
                                                       out HTuple column2_Line);

                            currentGroup.WireUserRegion.RegionType = RegionType.Line;
                            UserRegion userRegion_Line = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.WireUserRegion.RegionType, row1_Line, column1_Line, row2_Line, column2_Line, 0, 0);
                            if (userRegion_Line == null) return;
                            currentGroup.WireUserRegion = userRegion_Line;
                            DispalyGroupRegion(true);
                            break;

                        case RegionType.Rectangle1:
                            htWindow.InitialHWindowUpdate(Math.Floor(currentGroup.WireUserRegion.RegionParameters[0]),
                                                 Math.Floor(currentGroup.WireUserRegion.RegionParameters[1]),
                                                 Math.Ceiling(currentGroup.WireUserRegion.RegionParameters[2]),
                                                 Math.Ceiling(currentGroup.WireUserRegion.RegionParameters[3]), currentGroup.WireUserRegion.RegionType,
                                                 BondMeasureParameter.DieImageRowOffset, BondMeasureParameter.DieImageColumnOffset, "yellow");
                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (currentGroup.WireUserRegion.RegionParameters[0]),
                                                                                          (currentGroup.WireUserRegion.RegionParameters[1]),
                                                                                          (currentGroup.WireUserRegion.RegionParameters[2]),
                                                                                          (currentGroup.WireUserRegion.RegionParameters[3]),
                                                       out HTuple row1_Rectangle,
                                                       out HTuple column1_Rectangle,
                                                       out HTuple row2_Rectangle,
                                                       out HTuple column2_Rectangle);

                            currentGroup.WireUserRegion.RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.WireUserRegion.RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, BondMeasureParameter.DieImageRowOffset, BondMeasureParameter.DieImageColumnOffset);
                            if (userRegion == null) return;
                            currentGroup.WireUserRegion = userRegion;
                            DispalyGroupRegion(true);
                            break;

                        case RegionType.Rectangle2:
                            break;

                        case RegionType.Circle:
                            break;

                        case RegionType.Ellipse:
                            break;

                        default: break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void ExecuteDisplayAllInspectRegionsCommand(object parameter)
        {
            try
            {
                DisplayAllInspectRegions(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void DisplayAllInspectRegions(bool isHTWindowRegion = true)
        {
            if (GroupsCount == 0)
            {
                htWindow.Display(BondMeasureModelObject.Image, true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            for (int i = 0; i < GroupsCount; i++)
            {
                if (Groups[i].Bond2UserRegion != null)
                {
                    HOperatorSet.ConcatObj(concatGroupRegion, Groups[i].Bond2UserRegion.DisplayRegion, out concatGroupRegion);
                }
                if (Groups[i].WireUserRegion != null)
                {
                    HOperatorSet.ConcatObj(concatGroupRegion, Groups[i].WireUserRegion.DisplayRegion, out concatGroupRegion);
                }
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                htWindow.DisplaySingleRegion(concatGroupRegion, BondMeasureModelObject.Image);
            }
        }

        private void DisplayAllRegions(bool isHTWindowRegion = true)
        {
            if (groupsCount == 0)
            {
                htWindow.Display(BondMeasureModelObject.Image, true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            for (int i = 0; i < groupsCount; i++)
            {
                if (Groups[i].Bond2UserRegion != null)
                {
                    HOperatorSet.ConcatObj(concatGroupRegion, Groups[i].Bond2UserRegion.DisplayRegion, out concatGroupRegion);
                }
                if (Groups[i].WireUserRegion != null)
                {
                    HOperatorSet.ConcatObj(concatGroupRegion, Groups[i].WireUserRegion.DisplayRegion, out concatGroupRegion);
                }
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                htWindow.DisplaySingleRegion(concatGroupRegion, BondMeasureModelObject.Image);
            }
            for (int i = 0; i < groupsCount; i++)
            {
                if (Groups[i].Bond2UserRegion == null)
                {
                    MessageBox.Show("请先绘制区域！");
                    return;
                }
                HOperatorSet.AreaCenter(Groups[i].Bond2UserRegion.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, row_tmp, col_tmp);
                HOperatorSet.TupleString(Groups[i].Bond2UserRegion.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }

        //测量焊点  区域自动生成
        private void ExecuteCreateAutoBondUserRegionCommand(object parameter)
        {
            try
            {
                HOperatorSet.ReadRegion(out HObject DieRegion, $"{ReferenceDirectory}DieReference.reg");
                if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
                {
                    MessageBox.Show("没有参考图像!");
                    return;
                }
                if (BondMeasureParameter.ImageCountChannels > 0 && BondMeasureParameter.ImageChannelIndex < 0)
                {
                    MessageBox.Show("请先选择通道图像！");
                    return;
                }
                if (BondMeasureParameter.OnRecipesIndex == -1)
                {
                    MessageBox.Show("请先选择焊点所在位置！");
                    return;
                }

                if (ModelID.TupleLength() < 1)
                {
                    MessageBox.Show("请先创建匹配模板！");
                    return;
                }

                Algorithm.Model_RegionAlg.HTV_confirm_mapping_matrix(Algorithm.Region.GetChannnelImageConcact(BondMeasureModelObject.Image),
                       DieRegion,
                       ModelsFile,
                       RecipeFile,
                       BondMeasureParameter.OnRecipesIndexs[BondMeasureParameter.OnRecipesIndex],
                       out HTuple HomMat2D,
                       out HTuple _frameLocPara,
                       out HTuple ErrCode, out HTuple ErrStr);

                HTuple BondNum = new HTuple();
                HTuple LineAngle = new HTuple();
                foreach (var group in Groups)
                {
                    HOperatorSet.TupleConcat(BondNum, group.Bond2_BallNums, out BondNum);
                    HOperatorSet.AngleLx(group?.WireUserRegion.RegionParameters[0], group?.WireUserRegion.RegionParameters[1], group?.WireUserRegion.RegionParameters[2], group?.WireUserRegion.RegionParameters[3],
                                                 out HTuple lineAngle);
                    HOperatorSet.TupleConcat(LineAngle, lineAngle, out LineAngle);
                }

                Algorithm.Model_RegionAlg.HTV_Gen_Bond_Region(Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex),
                                Algorithm.Region.ConcatRegion(Groups.Select(r => r.Bond2UserRegion).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                out HObject BondContours,
                                HomMat2D,
                                BondNum,
                                Bond1AutoRegionsParameter.ModelType == 0 ? "ncc" : "shape",
                                ModelID,
                                Bond1AutoRegionsParameter.MinMatchScore,//0.65
                                LineAngle,
                                Bond1AutoRegionsParameter.AngleExt,
                                Bond1AutoRegionsParameter.BondSize,
                                out HTuple BondRows, out HTuple BondCols, out HTuple BondAngles,
                                out HTuple _ErrCode, out HTuple _ErrStr);


                if (BondContours.CountObj() == 0)
                {
                    MessageBox.Show("自动生成焊点数为0！");
                    return;
                }

                Bond1AutoUserRegion.Clear();
                //htWindow.DisplaySingleRegion(BondContours.ConcatObj(BondRegs).ConcatObj(BondLines));

                if (Bond1AutoRegionsParameter.IsCircleShape == true)
                {
                    for (int i = 0; i < BondContours.CountObj(); i++)
                    {
                        UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Circle,
                                BondRows[i],
                                BondCols[i],
                                Bond1AutoRegionsParameter.BondSize,
                                0,
                                0, 0, BondAngles[i]);
                        if (userRegion_Circle == null) return;
                        userRegion_Circle.Index = Bond1AutoUserRegion.Count + 1;
                        Bond1AutoUserRegion.Add(userRegion_Circle);
                    }
                }
                else
                {
                    for (int i = 0; i < BondContours.CountObj(); i++)
                    {
                        UserRegion userRegion_Ellips = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Ellipse,
                                BondRows[i],
                                BondCols[i],
                                Bond1AutoRegionsParameter.EllipsBondSize[0],
                                Bond1AutoRegionsParameter.EllipsBondSize[1],
                                0, 0, BondAngles[i].D);
                        if (userRegion_Ellips == null) return;
                        userRegion_Ellips.Index = Bond1AutoUserRegion.Count + 1;
                        Bond1AutoUserRegion.Add(userRegion_Ellips);
                    }
                }
                //htWindow.DisplayMultiRegion(Bond1AutoRegion);
                DispalyGroupsRegions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void ExecuteTextChangedCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    UserRegion userRegion = parameter as UserRegion;
                    if (userRegion == null || userRegion.RegionType == RegionType.Circle)
                    {
                        userRegion.RegionParameters[2] = 0;
                        return;
                    }
                    if (userRegion.RegionType == RegionType.Ellipse)
                    {
                        UserRegion userRegion_Ellipse = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Ellipse,
                        userRegion.RegionParameters[0],
                        userRegion.RegionParameters[1],
                        userRegion.RegionParameters[3],//Bond1AutoRegionsParameter.EllipsBondSize[0],
                        userRegion.RegionParameters[4],//Bond1AutoRegionsParameter.EllipsBondSize[1],
                        0, 0,
                        userRegion.RegionParameters[2]);
                        if (userRegion_Ellipse == null) return;
                        userRegion_Ellipse.Index = userRegion.Index;
                        Bond1AutoUserRegion[userRegion.Index - 1] = userRegion_Ellipse;
                        DispalyGroupsRegions();
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
        }

        //排序
        private void ExecuteSortCommand(object parameter)
        {
            if (Bond1AutoUserRegion.Count() == 0) return;
            try
            {
                switch (Bond1AutoRegionsParameter.SortMethod)
                {
                    case 0:
                        ObservableCollection<UserRegion> Bond1AutoUserRegionSort = new ObservableCollection<UserRegion>(Bond1AutoUserRegion.OrderBy(r => r.Index));
                        Bond1AutoUserRegion.Clear();
                        for (int i = 0; i < Bond1AutoUserRegionSort.Count(); i++)
                        {
                            Bond1AutoUserRegion.Add(Bond1AutoUserRegionSort[i]);
                        }
                        break;

                    case 1:
                        //列升序
                        ObservableCollection<UserRegion> Bond1AutoUserRegionSort1 = new ObservableCollection<UserRegion>(Bond1AutoUserRegion.OrderBy(r => r.RegionParameters[1]));
                        Bond1AutoUserRegion.Clear();
                        for (int i = 0; i < Bond1AutoUserRegionSort1.Count(); i++)
                        {
                            Bond1AutoUserRegion.Add(Bond1AutoUserRegionSort1[i]);
                            Bond1AutoUserRegionSort1[i].Index = Bond1AutoUserRegion.Count;
                        }
                        break;

                    case 2:
                        //列降序
                        ObservableCollection<UserRegion> Bond1AutoUserRegionSort2 = new ObservableCollection<UserRegion>(Bond1AutoUserRegion.OrderByDescending(r => r.RegionParameters[1]));
                        Bond1AutoUserRegion.Clear();
                        for (int i = 0; i < Bond1AutoUserRegionSort2.Count(); i++)
                        {
                            Bond1AutoUserRegion.Add(Bond1AutoUserRegionSort2[i]);
                            Bond1AutoUserRegionSort2[i].Index = Bond1AutoUserRegion.Count;
                        }
                        break;

                    case 3:
                        //行升序
                        ObservableCollection<UserRegion> Bond1AutoUserRegionSort3 = new ObservableCollection<UserRegion>(Bond1AutoUserRegion.OrderBy(r => r.RegionParameters[0]));
                        Bond1AutoUserRegion.Clear();
                        for (int i = 0; i < Bond1AutoUserRegionSort3.Count(); i++)
                        {
                            Bond1AutoUserRegion.Add(Bond1AutoUserRegionSort3[i]);
                            Bond1AutoUserRegionSort3[i].Index = Bond1AutoUserRegion.Count;
                        }
                        break;

                    case 4:
                        //行降序
                        ObservableCollection<UserRegion> Bond1AutoUserRegionSort4 = new ObservableCollection<UserRegion>(Bond1AutoUserRegion.OrderByDescending(r => r.RegionParameters[0]));
                        Bond1AutoUserRegion.Clear();
                        for (int i = 0; i < Bond1AutoUserRegionSort4.Count(); i++)
                        {
                            Bond1AutoUserRegion.Add(Bond1AutoUserRegionSort4[i]);
                            Bond1AutoUserRegionSort4[i].Index = Bond1AutoUserRegion.Count;
                        }
                        break;

                    case 5:
                        //顺时针
                        if (Bond1AutoRegionsParameter.FirstSortNumber == -1)
                        {
                            MessageBox.Show("请选择排序起始焊点！");
                            return;
                        }
                        HTuple BondRows = new HTuple();
                        HTuple BondCols = new HTuple();
                        HTuple BondAngles = new HTuple();

                        foreach (var item in Bond1AutoUserRegion)
                        {
                            BondRows = BondRows.TupleConcat(item.RegionParameters[0]);
                            BondCols = BondCols.TupleConcat(item.RegionParameters[1]);
                            BondAngles = BondAngles.TupleConcat(item.RegionParameters[2]);
                        }

                        Algorithm.Model_RegionAlg.HTV_Clockwise_Sort_Points_Angles(0,
                                        0,
                                        BondRows,
                                        BondCols,
                                        BondAngles,
                                        Bond1AutoRegionsParameter.FirstSortNumber,
                                        0,
                                        out HTuple SortRows,
                                        out HTuple SortCols,
                                        out HTuple SortAngles,
                                        out HTuple SortIndex);
                        Bond1AutoUserRegion.Clear();

                        for (int i = 0; i < SortRows.TupleLength(); i++)
                        {
                            UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                    Bond1AutoRegionsParameter.IsCircleShape == true ? RegionType.Circle : RegionType.Ellipse,
                                    SortRows[i],
                                    SortCols[i],
                                    Bond1AutoRegionsParameter.IsCircleShape == true ? Bond1AutoRegionsParameter.BondSize : Bond1AutoRegionsParameter.EllipsBondSize[0],
                                    Bond1AutoRegionsParameter.IsCircleShape == true ? 0 : Bond1AutoRegionsParameter.EllipsBondSize[1],
                                    0, 0,
                                    Bond1AutoRegionsParameter.IsCircleShape == true ? 0 : SortAngles[i].D);
                            if (userRegion_Circle == null) return;
                            userRegion_Circle.Index = Bond1AutoUserRegion.Count + 1;
                            Bond1AutoUserRegion.Add(userRegion_Circle);
                        }
                        break;

                    case 6:
                        //逆时针
                        if (Bond1AutoRegionsParameter.FirstSortNumber == -1)
                        {
                            MessageBox.Show("请选择排序起始焊点！");
                            return;
                        }
                        HTuple AntiBondRows = new HTuple();
                        HTuple AntiBondCols = new HTuple();
                        HTuple AntiBondAngles = new HTuple();

                        foreach (var item in Bond1AutoUserRegion)
                        {
                            AntiBondRows = AntiBondRows.TupleConcat(item.RegionParameters[0]);
                            AntiBondCols = AntiBondCols.TupleConcat(item.RegionParameters[1]);
                            AntiBondAngles = AntiBondAngles.TupleConcat(item.RegionParameters[2]);
                        }

                        Algorithm.Model_RegionAlg.HTV_Clockwise_Sort_Points_Angles(0,
                                        0,
                                        AntiBondRows,
                                        AntiBondCols,
                                        AntiBondAngles,
                                        Bond1AutoRegionsParameter.FirstSortNumber,
                                        1,
                                        out HTuple SortAntiRows,
                                        out HTuple SortAntiCols,
                                        out HTuple SortAntiAngles,
                                        out HTuple SortAntiIndex);
                        Bond1AutoUserRegion.Clear();
                        for (int i = 0; i < SortAntiRows.TupleLength(); i++)
                        {
                            UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                    Bond1AutoRegionsParameter.IsCircleShape == true ? RegionType.Circle : RegionType.Ellipse,
                                    SortAntiRows[i],
                                    SortAntiCols[i],
                                    Bond1AutoRegionsParameter.IsCircleShape == true ? Bond1AutoRegionsParameter.BondSize : Bond1AutoRegionsParameter.EllipsBondSize[0],
                                    Bond1AutoRegionsParameter.IsCircleShape == true ? 0 : Bond1AutoRegionsParameter.EllipsBondSize[1],
                                    0, 0,
                                    Bond1AutoRegionsParameter.IsCircleShape == true ? 0 : SortAntiAngles[i].D);
                            if (userRegion_Circle == null) return;
                            userRegion_Circle.Index = Bond1AutoUserRegion.Count + 1;
                            Bond1AutoUserRegion.Add(userRegion_Circle);
                        }
                        break;

                    default:
                        break;
                }
                Bond1AutoRegionsParameter.FirstSortNumber = 0; // 排序后默认FirstSortNumber为0
                DispalyGroupsRegions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        //拾取
        public void GetClickDownPoints()
        {
            if (Bond1AutoUserRegion.Count == 0 || BondMeasureParameter.IsPickUp == false) return;
            try
            {
                HOperatorSet.GenRegionPoints(out HObject Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
                foreach (var item in Bond1AutoUserRegion)
                {
                    HOperatorSet.SelectShapeProto(item.DisplayRegion, Point, out HObject overlapRegion, "overlaps_abs", 1.0, 5.0);
                    if (overlapRegion.CountObj() > 0)
                    {
                        if (item.IsSelected == false)
                        {
                            htWindow.hTWindow.HalconWindow.SetColor("yellow");
                            item.IsSelected = true;
                            htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
                        }
                        else
                        {
                            htWindow.hTWindow.HalconWindow.SetColor("green");
                            item.IsSelected = false;
                            htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        //框选 
        private void ExecuteRectangle1SelectCommand(object parameter)
        {
            if (Bond1AutoUserRegion.Count == 0 || htWindow.RegionType == RegionType.Null || htWindow.RegionType != RegionType.Rectangle1) return;
            try
            {
                HOperatorSet.GenRectangle1(out HObject rectangle1_sel, htWindow.Row1_Rectangle1, htWindow.Column1_Rectangle1, htWindow.Row2_Rectangle1, htWindow.Column2_Rectangle1);

                foreach (var item in Bond1AutoUserRegion)
                {
                    HOperatorSet.SelectShapeProto(item.DisplayRegion, rectangle1_sel, out HObject overlapRegion, "overlaps_abs", 1.0, 9999.0);
                    if (overlapRegion.CountObj() > 0)
                    {
                        if (item.IsSelected == false)
                        {
                            htWindow.hTWindow.HalconWindow.SetColor("yellow");
                            item.IsSelected = true;
                            htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
                        }
                        else
                        {
                            htWindow.hTWindow.HalconWindow.SetColor("green");
                            item.IsSelected = false;
                            htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        //添加自动生成焊点区域
        private void ExecuteAddBond1AutoUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (htWindow.RegionType == RegionType.Null)
            {
                System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow);
                if (userRegion == null) return;
                userRegion.Index = Bond1AutoUserRegion.Count + 1;
                Bond1AutoUserRegion.Add(userRegion);
                DispalyGroupsRegions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                htWindow.RegionType = RegionType.Null;
            }
        }

        //删除自动生成焊点区域
        private void ExecuteRemoveBond1AutoUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < Bond1AutoUserRegion.Count; i++)
                {
                    if (Bond1AutoUserRegion[i].IsSelected)
                    {
                        Bond1AutoUserRegion.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        Bond1AutoUserRegion[i].Index = i + 1;
                    }
                }
                DispalyGroupsRegions();
                if (Bond1AutoUserRegion.Count == 0)
                {
                    IsCheckAllAuto = false;
                }
            }
        }

        //修改自动生成焊点区域
        private void ExecuteModifyBond1AutoRegionCommand(object parameter)//
        {
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    for (int i = 0; i < Bond1AutoUserRegion.Count; i++)
                    {
                        if (Bond1AutoUserRegion[i].IsSelected)
                        {
                            switch (Bond1AutoUserRegion[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Line:

                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(Bond1AutoUserRegion[i].RegionParameters[0]),
                                                                  Math.Floor(Bond1AutoUserRegion[i].RegionParameters[1]),
                                                                  Math.Ceiling(Bond1AutoUserRegion[i].RegionParameters[2]),
                                                                  Math.Ceiling(Bond1AutoUserRegion[i].RegionParameters[3]),
                                                                  Bond1AutoUserRegion[i].RegionType,
                                                                  0, 0, "yellow");

                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (Bond1AutoUserRegion[i].RegionParameters[0]),
                                                                                                  (Bond1AutoUserRegion[i].RegionParameters[1]),
                                                                                                  (Bond1AutoUserRegion[i].RegionParameters[2]),
                                                                                                  (Bond1AutoUserRegion[i].RegionParameters[3]),
                                                        out HTuple row1_Rectangle,
                                                        out HTuple column1_Rectangle,
                                                        out HTuple row2_Rectangle,
                                                        out HTuple column2_Rectangle);

                                    Bond1AutoUserRegion[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, Bond1AutoUserRegion[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, 0, 0);
                                    if (userRegion == null) return;
                                    Bond1AutoUserRegion[i] = userRegion;
                                    Bond1AutoUserRegion[i].Index = i + 1;
                                    DispalyGroupsRegions();
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(Bond1AutoUserRegion[i].RegionParameters[0]),
                                                                  Math.Floor(Bond1AutoUserRegion[i].RegionParameters[1]),
                                                                  Math.Ceiling(Bond1AutoUserRegion[i].RegionParameters[2]),
                                                                  Math.Ceiling(Bond1AutoUserRegion[i].RegionParameters[3]),
                                                                  Bond1AutoUserRegion[i].RegionType,
                                                                  0, 0, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(Bond1AutoUserRegion[i].RegionParameters[0]),
                                                                   Math.Floor(Bond1AutoUserRegion[i].RegionParameters[1]),
                                                                   Bond1AutoUserRegion[i].RegionParameters[2],
                                                                   Math.Ceiling(Bond1AutoUserRegion[i].RegionParameters[3]),
                                                                   Math.Ceiling(Bond1AutoUserRegion[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    Bond1AutoUserRegion[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, Bond1AutoUserRegion[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         0,
                                                                                                         0,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    Bond1AutoUserRegion[i] = userRegion_Rectangle2;
                                    Bond1AutoUserRegion[i].Index = i + 1;
                                    DispalyGroupsRegions();
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((Bond1AutoUserRegion[i].RegionParameters[0]),
                                                                  (Bond1AutoUserRegion[i].RegionParameters[1]),
                                                                  (Bond1AutoUserRegion[i].RegionParameters[2]),
                                                                  0,
                                                                  Bond1AutoUserRegion[i].RegionType,
                                                                  0, 0, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (Bond1AutoUserRegion[i].RegionParameters[0]),
                                                               (Bond1AutoUserRegion[i].RegionParameters[1]),
                                                               Bond1AutoUserRegion[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    Bond1AutoUserRegion[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     Bond1AutoUserRegion[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    Bond1AutoUserRegion[i] = userRegion_Circle;
                                    Bond1AutoUserRegion[i].Index = i + 1;
                                    DispalyGroupsRegions();
                                    break;

                                case RegionType.Ellipse:
                                    htWindow.InitialHWindowUpdate(Math.Floor(Bond1AutoUserRegion[i].RegionParameters[0]),
                                    Math.Floor(Bond1AutoUserRegion[i].RegionParameters[1]),
                                    Bond1AutoUserRegion[i].RegionParameters[2],
                                    Math.Ceiling(Bond1AutoUserRegion[i].RegionParameters[3]),
                                    Bond1AutoUserRegion[i].RegionType, 0, 0, "yellow");

                                    HOperatorSet.DrawEllipseMod(htWindow.hTWindow.HalconWindow,
                                                               Math.Floor(Bond1AutoUserRegion[i].RegionParameters[0] - 0),
                                                               Math.Floor(Bond1AutoUserRegion[i].RegionParameters[1] - 0),
                                                               Bond1AutoUserRegion[i].RegionParameters[2],
                                                               Math.Floor(Bond1AutoUserRegion[i].RegionParameters[3] - 0),
                                                               Math.Floor(Bond1AutoUserRegion[i].RegionParameters[4] - 0),
                                                           out HTuple row1,
                                                           out HTuple column1,
                                                           out HTuple phi,
                                                           out HTuple radius1,
                                                           out HTuple radius2);

                                    Bond1AutoUserRegion[i].RegionType = RegionType.Ellipse;
                                    UserRegion userRegion_Ellipse = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     Bond1AutoUserRegion[i].RegionType,
                                                                                                     row1, column1, radius1, radius2, 0, 0, phi);
                                    if (userRegion_Ellipse == null) return;
                                    Bond1AutoUserRegion[i] = userRegion_Ellipse;
                                    Bond1AutoUserRegion[i].Index = i + 1;
                                    DispalyGroupsRegions();
                                    break;
                          

                                default: break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void ExecuteDisplayAutoBond1RegionsCommand(object parameter)
        {
            DispalyGroupsRegions();
        }

        public void DispalyGroupsRegions(bool isHTWindowRegion = true)
        {
            if (Bond1AutoUserRegion.Count == 0)
            {
                //htWindow.Display(BondMeasureModelObject.Image, true);
                return;
            }

            if (isHTWindowRegion)
            {
                htWindow.DisplayMultiRegion(Bond1AutoRegion);
            }
            else
            {
                htWindow.DisplayMultiRegion(Bond1AutoRegion, BondMeasureModelObject.Image);
            }

            foreach (var item in Bond1AutoUserRegion)
            {
                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, row_tmp, col_tmp);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }

        //加载参考数据
        private void ExecuteLoadReferenceCommand(object parameter)
        {
            if (!Directory.Exists(ReferenceDirectory))
            {
                MessageBox.Show("请先创建全局数据！");
                return;
            }
            try
            {
                LoadReferenceData();

                HTuple filesFrame = new HTuple();
                HTuple filesIC = new HTuple();
                filesFrame = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                            "Frame*.*", SearchOption.TopDirectoryOnly);
                filesIC = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                           "IC*.*", SearchOption.TopDirectoryOnly);
                string[] files = filesFrame.TupleConcat(filesIC);
                string[] OnRecipesIndexs = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    OnRecipesIndexs[i] = Path.GetFileName(files[i]);
                }
                BondMeasureParameter.OnRecipesIndexs = OnRecipesIndexs;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "全局数据文件不存在！");
                return;
            }
        }

        public void LoadReferenceData()
        {
            if (!(File.Exists($"{ReferenceDirectory}ReferenceImage.tiff")
             && File.Exists($"{ReferenceDirectory}DieReference.reg") && File.Exists($"{ReferenceDirectory}UserRegionForCutOutIndex.tup")
             && File.Exists($"{ReferenceDirectory}DieImageRowOffset.tup") && File.Exists($"{ReferenceDirectory}DieImageColumnOffset.tup")
             && File.Exists($"{ReferenceDirectory}CoarseReference.reg") && File.Exists($"{ReferenceDirectory}ReferenceImage.tiff")))
            {
                MessageBox.Show("全局数据不全，请创建！" + System.Environment.NewLine + System.Environment.NewLine + "提示：  "
                    + System.Environment.NewLine + "TrainningImagesDirectory.tup" + System.Environment.NewLine + "ImagePath.tup"
                     + System.Environment.NewLine + "DieReference.reg" + System.Environment.NewLine + "UserRegionForCutOutIndex.tup"
                      + System.Environment.NewLine + "DieImageRowOffset.tup" + System.Environment.NewLine + "DieImageColumnOffset.tup"
                       + System.Environment.NewLine + "CoarseReference.reg" + System.Environment.NewLine + "ReferenceImage.tiff");
                return;
            }

            HOperatorSet.ReadTuple(ReferenceDirectory + "TrainningImagesDirectory.tup", out HTuple TrainningImagesDirectoryTemp);
            BondMeasureParameter.VerifyImagesDirectory = TrainningImagesDirectoryTemp;

            BondMeasureParameter.ImagePath = $"{ReferenceDirectory}ReferenceImage.tiff";
            LoadImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageRowOffset.tup", out HTuple DieImageRowOffsetTemp);
            BondMeasureParameter.DieImageRowOffset = DieImageRowOffsetTemp;
            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageColumnOffset.tup", out HTuple DieImageColumnOffsetTemp);
            BondMeasureParameter.DieImageColumnOffset = DieImageColumnOffsetTemp;

            HOperatorSet.ReadRegion(out HObject UserRegionForCutOut_Region, ReferenceDirectory + "DieReference.reg");
            HOperatorSet.ReduceDomain(BondMeasureModelObject.Image, UserRegionForCutOut_Region, out HObject imageReduced);
            HOperatorSet.CropDomain(imageReduced, out HObject dieImage);
            BondMeasureModelObject.DieImage = dieImage;
            LoadDieImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "UserRegionForCutOutIndex.tup", out HTuple UserRegionForCutOutIndexTemp);
            BondMeasureParameter.UserRegionForCutOutIndex = UserRegionForCutOutIndexTemp;

            //1122 lw
            HOperatorSet.ReadTuple(ReferenceDirectory + "ImageChannelnumber.tup", out HTuple ImageChannelnumber);
            ChannelNames.Clear();
            for (int i = 0; i < ImageChannelnumber; i++)
            {
                ChannelName tmp_name = new ChannelName();
                tmp_name.Name = (i + 1).ToString();
                ChannelNames.Add(tmp_name);
            }

            ImageChannelIndex = BondMeasureParameter.ImageChannelIndex;

            BondMeasureParameter.ChannelNames = ChannelNames;
            OnPropertyChanged("BondMeasureParameter.ImageChannelIndex");

            //1201 lw
            HOperatorSet.TupleSplit(ReferenceDirectory, "\\", out HTuple hv_subStr);
            HTuple FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 3];
            BondMeasureParameter.CurFovName = FOV_Name;

            //htWindow.DisplaySingleRegion(UserRegionForCutOut_Region, BondMeasureModelObject.DieImage);
            htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex), true);
        }

        public void LoadImage()
        {
            HOperatorSet.GenEmptyObj(out HObject image);
            HOperatorSet.ReadImage(out image, BondMeasureParameter.ImagePath);
            BondMeasureModelObject.Image = image;
            //SwitchToImage();

            HOperatorSet.CountChannels((HObject)BondMeasureModelObject.Image, out HTuple channels);
            BondMeasureParameter.ImageCountChannels = channels;

            BondMeasureModelObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex);
            //BondMeasureModelObject.ImageR = Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, 1);
            //BondMeasureModelObject.ImageG = Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, 2);
            //BondMeasureModelObject.ImageB = Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, 3);
        }

        public void LoadDieImage()
        {
            BondMeasureModelObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.DieImage, BondMeasureParameter.ImageChannelIndex);
            //BondMeasureModelObject.DieImageR = Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.DieImage, 1);
            //BondMeasureModelObject.DieImageG = Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.DieImage, 2);
            //BondMeasureModelObject.DieImageB = Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.DieImage, 3);
        }

        private void ExecuteUserRegionEnableChangedCommand(object parameter)
        {
            htWindow.DisplayMultiRegion(Bond1AutoRegion);
        }

        private void ExecuteLoadAutoMeasureBondCommand(object parameter)
        {
            if (isRightClick != true) return;

            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }
            if (Bond1AutoUserRegion.Count < 1)
            {
                //MessageBox.Show("请先自动生成测量焊点区域！");
                return;
            }
            try
            {
                BondModelUserRegions.Clear();
                foreach (var item in Bond1AutoUserRegion)
                {
                    item.BondMeasureModelParameter = new BondMeasureModelParameter();
                    item.BondMeasureVerifyParameterSet = new BondMeasureVerifyParameterSet();
                    BondModelUserRegions.Add(item);
                    DispalyBondModelRegions();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void ExecuteUpdateAutoMeasureBondCommand(object parameter)
        {
            if (isRightClick != true) return;

            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }
            if (Bond1AutoUserRegion.Count < 1)
            {
                MessageBox.Show("请先自动生成测量焊点区域！");
                return;
            }
            try
            {
                BondModelUserRegions.Clear();
                foreach (var item in Bond1AutoUserRegion)
                {
                    item.BondMeasureModelParameter = new BondMeasureModelParameter();
                    item.BondMeasureVerifyParameterSet = new BondMeasureVerifyParameterSet();
                    BondModelUserRegions.Add(item);
                    DispalyBondModelRegions();
                }

                MessageBox.Show("请更新测量参数设置");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        public void DispalyBondModelRegions(bool isHTWindowRegion = true)
        {
            if (BondModelUserRegions.Count == 0)
            {
                htWindow.Display(BondMeasureModelObject.Image, true);
                return;
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplayMultiRegion(BondModelRegions);
            }
            else
            {
                htWindow.DisplayMultiRegion(BondModelRegions, BondMeasureModelObject.Image);
            }
            foreach (var item in BondModelUserRegions)
            {
                htWindow.hTWindow.HalconWindow.SetColor("orange");
                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, row_tmp, col_tmp);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }

        private void ExecuteIsCheckAutoCommand(object parameter)
        {
            if (Bond1AutoUserRegion.All(x => x.IsSelected == true))
            { IsCheckAllAuto = true; }
            else if (Bond1AutoUserRegion.All(x => !x.IsSelected))
            { IsCheckAllAuto = false; }
            else
            { IsCheckAllAuto = null; }
        }

        private void ExecuteIsCheckAllAutoCommand(object parameter)
        {
            if (IsCheckAllAuto == true)
            { Bond1AutoUserRegion.ToList().ForEach(r => r.IsSelected = true); }
            else if (IsCheckAllAuto == false)
            { Bond1AutoUserRegion.ToList().ForEach(r => r.IsSelected = false); }
        }

        private void SwitchToImage()
        {
            SwitchImageComboBoxIndex = 0;
            OnPropertyChanged("SwitchImageComboBoxIndex");
        }

        private void SwitchToRotatedImage()
        {
            SwitchImageComboBoxIndex = 1;
            OnPropertyChanged("SwitchImageComboBoxIndex");
        }

        private void DispalyGroupRegion(bool isHTWindowRegion = true)
        {
            if (CurrentGroup == null)
            {
                htWindow.Display(BondMeasureModelObject.Image, true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            if (CurrentGroup.Bond2UserRegion != null)
            {
                HOperatorSet.ConcatObj(CurrentGroup.Bond2UserRegion.CalculateRegion, concatGroupRegion, out concatGroupRegion);
            }
            if (CurrentGroup.WireUserRegion != null)
            {
                HOperatorSet.ConcatObj(CurrentGroup.WireUserRegion.CalculateRegion, concatGroupRegion, out concatGroupRegion);
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                htWindow.DisplaySingleRegion(concatGroupRegion, BondMeasureModelObject.Image);
            }
        }

        public bool CheckCompleted()
        {
            return true;
        }

        public void Initial()
        {
            htWindow.ClearSelection();
            BondMeasureParameter.IsPickUp = false;
            if (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff) return;
            htWindow.DisplayMultiRegion(Bond1AutoRegion, Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex));
        }

        public void Dispose()
        {
            (Content as Page_CreateAutoBondMeasureModel).DataContext = null;
            (Content as Page_CreateAutoBondMeasureModel).Close();
            this.Content = null;
            this.htWindow = null;
            this.LoadReferenceCommand = null;
            this.UserRegionEnableChangedCommand = null;
            this.BondMeasureModelObject = null;
            this.BondMeasureParameter = null;
            this.BondMeasureModelParameter = null;
            this.Bond1AutoRegionsParameter = null;
            this.BondModelUserRegions = null;
            this.Bond1AutoUserRegion = null;
            this.SelectedUserRegion = null;
            this.IsCheckAutoCommand = null;
            this.IsCheckAllAutoCommand = null;
            this.AddBond2UserRegionCommand = null;
            this.ModifyBond2ModelRegionCommand = null;
            this.AddBond2UserRegionDiffCommand = null; 
            this.ModifyBond2ModelRegionDiffCommand = null;
            this.AddBond2InspectUserRegionCommand = null;
            this.ModifyBond2InspectUserRegionCommand = null;
            this.AddWireInspectUserRegionCommand = null;
            this.ModifyWireInspectUserRegionCommand = null;
            this.DisplayAllInspectRegionsCommand = null;
            this.AddGroupCommand = null;
            this.RemoveGroupCommand = null;
            this.CreateAutoBondUserRegionCommand = null;
            this.TextChangedCommand = null;
            this.SortCommand = null;
            this.AddBond1AutoUserRegionCommand = null;
            this.ModifyBond1AutoRegionCommand = null;
            this.RemoveBond1AutoUserRegionCommand = null;
            this.DisplayBond1ModelRegionsCommand = null;
            this.DisplayAutoBond1RegionsCommand = null;
            this.CreateBond2ModelCommand = null;
            this.LoadAutoMeasureBondCommand = null;
            this.SelectedChangedImageCommand = null;
            this.OptimizeResultCommand = null;
            this.ModifyParametersBatchCommand = null;
            this.UpdateAutoMeasureBondCommand = null;
            this.Rectangle1SelectCommand = null;
            this.RotateCommand = null;
            this.Bond2UserRegion = null;
            this.Bond2UserRegionDiff = null;
        }
    }
}
