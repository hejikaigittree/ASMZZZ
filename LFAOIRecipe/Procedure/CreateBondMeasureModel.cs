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
    class CreateBondMeasureModel : ViewModelBase, IProcedure
    {
        //1122
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public bool isRightClick = true;

        public string ReferenceDirectory { get; set; }

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

        public int ImageChannelIndex;

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
        private IEnumerable<HObject> BondModelRegions => BondModelUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public ObservableCollection<UserRegion> Bond1AutoUserRegion { get; private set; } = new ObservableCollection<UserRegion>();
        private IEnumerable<HObject> Bond1AutoRegion => Bond1AutoUserRegion.Where(r => r.IsEnable).Select(r => r.CalculateRegion);

        //MeasureContrast MearsureLength MearsureWideth MearsureTransition 

        private bool isSetAllParas = false;
        public bool IsSetAllParas
        {
            get => isSetAllParas;
            set => OnPropertyChanged(ref isSetAllParas, value);
        }

        private bool? isCheckAll;
        public bool? IsCheckAll
        {
            get => isCheckAll;
            set => OnPropertyChanged(ref isCheckAll, value);
        }

        private double measureContrast = 5;
        public double MeasureContrast
        {
            get => measureContrast;
            set => OnPropertyChanged(ref measureContrast, value);
        }

        private double mearsureLength = 2;
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
        public CommandBase AddUserRegionCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }
        public CommandBase CreateBondMeasureModelCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase SaveBondModelCommand { get; private set; }
        public CommandBase BondMeasureInspectCommand { get; private set; }
        public CommandBase ModifyParametersBatchCommand { get; private set; }
        public CommandBase IsCheckCommand { get; private set; }
        public CommandBase IsCheckAllCommand { get; private set; }
        public CommandBase LoadAutoMeasureBondCommand { get; private set; }
        public CommandBase DisplayBondMeasureRegionsCommand { get; private set; }
        public CommandBase Rectangle1SelectCommand { get; private set; }
        public CommandBase SelectedChangedImageCommand { get; private set; }

        private HTHalControlWPF htWindow;

        private readonly string bondRecipeDirectory;

        private readonly string ModelsRecipeDirectory;

        public CreateBondMeasureModel(HTHalControlWPF htWindow,
                                     string referenceDirectory,
                                     BondMeasureModelObject BondMeasureModelObject,
                                     BondMeasureParameter BondMeasureParameter,
                                     ObservableCollection<UserRegion> BondModelUserRegions,
                                     ObservableCollection<UserRegion> Bond1AutoUserRegion,
                                     string bondRecipeDirctory,
                                     string modelsRecipeDirectory)

        {
            DisplayName = "创建焊点测量模板";
            Content = new Page_CreateBondMeasureModel { DataContext = this };
            this.htWindow = htWindow;
            this.ReferenceDirectory = referenceDirectory;
            this.BondMeasureModelObject = BondMeasureModelObject;
            this.BondMeasureParameter = BondMeasureParameter;
            this.BondModelUserRegions = BondModelUserRegions;
            this.Bond1AutoUserRegion = Bond1AutoUserRegion;
            this.bondRecipeDirectory = bondRecipeDirctory;
            this.ModelsRecipeDirectory = modelsRecipeDirectory;

            LoadReferenceCommand = new CommandBase(ExecuteLoadReferenceCommand);
            AddUserRegionCommand = new CommandBase(ExecuteAddUserRegionCommand);
            RemoveUserRegionCommand = new CommandBase(ExecuteRemoveUserRegionCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);
            CreateBondMeasureModelCommand = new CommandBase(ExecuteCreateBondMeasureModelCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            SaveBondModelCommand = new CommandBase(ExecuteSaveBondModelCommand);
            BondMeasureInspectCommand = new CommandBase(ExecuteBondMeasureInspectCommand);
            ModifyParametersBatchCommand = new CommandBase(ExecuteModifyParametersBatchCommand);
            IsCheckCommand = new CommandBase(ExecuteIsCheckCommand);
            IsCheckAllCommand = new CommandBase(ExecuteIsCheckAllCommand);
            LoadAutoMeasureBondCommand = new CommandBase(ExecuteLoadAutoMeasureBondCommand);
            DisplayBondMeasureRegionsCommand = new CommandBase(ExecuteDisplayBondMeasureRegionsCommand);
            Rectangle1SelectCommand = new CommandBase(ExecuteRectangle1SelectCommand);
            // 1122-lw
            SelectedChangedImageCommand = new CommandBase(ExecuteSelectedChangedImageCommand);
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
            }
        }

        //加载自动生成焊点区域
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
                MessageBox.Show("请先自动生成测量焊点区域！");
                return;
            }
            BondModelUserRegions.Clear();
            foreach (var item in Bond1AutoUserRegion)
            {
                item.BondMeasureModelParameter = new BondMeasureModelParameter();
                item.BondMeasureVerifyParameterSet = new BondMeasureVerifyParameterSet();
                BondModelUserRegions.Add(item);
                DispalyBondModelRegions();
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
                htWindow.hTWindow.HalconWindow.SetColor("green");
                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, row_tmp, col_tmp);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }

        private void ExecuteDisplayBondMeasureRegionsCommand(object parameter)
        {
            DispalyBondModelRegions();
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

            htWindow.DisplaySingleRegion(UserRegionForCutOut_Region, Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex));
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
            htWindow.DisplayMultiRegion(BondModelRegions);
        }

        private void ExecuteAddUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;

            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }

            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow);
                if (userRegion == null) return;
                userRegion.Index = BondModelUserRegions.Count + 1;
                userRegion.BondMeasureModelParameter = new BondMeasureModelParameter();
                userRegion.BondMeasureVerifyParameterSet = new BondMeasureVerifyParameterSet();
                BondModelUserRegions.Add(userRegion);
                htWindow.DisplayMultiRegion(BondModelRegions);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                htWindow.RegionType = RegionType.Null;//
            }
        }

        private void ExecuteRemoveUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;//

            //if (CurrentModel == null)
            //{
            //MessageBox.Show("请选择或者新建一个模板配置");
            //return;
            //}
            try
            {
                if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    for (int i = 0; i < BondModelUserRegions.Count; i++)
                    {
                        if (BondModelUserRegions[i].IsSelected)
                        {
                            BondModelUserRegions.RemoveAt(i);
                            i--;

                        }
                        else
                        {
                            BondModelUserRegions[i].Index = i + 1;
                        }
                    }
                    //htWindow.DisplayMultiRegion(RefineRegions, BondMeasureModelObject.Image);
                    htWindow.DisplayMultiRegion(BondModelRegions);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void ExecuteModifyRegionCommand(object parameter)//
        {
            if (isRightClick != true) return;//
            try
            {
                if (BondModelUserRegions == null) return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请添加精炼区域到列表");
                return;
            }
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    for (int i = 0; i < BondModelUserRegions.Count; i++)
                    {
                        if (BondModelUserRegions[i].IsSelected)
                        {
                            switch (BondModelUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(BondModelUserRegions[i].RegionParameters[0]),
                                                  Math.Floor(BondModelUserRegions[i].RegionParameters[1]),
                                                  Math.Ceiling(BondModelUserRegions[i].RegionParameters[2]),
                                                  Math.Ceiling(BondModelUserRegions[i].RegionParameters[3]),
                                                  BondModelUserRegions[i].RegionType, 0, 0, "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (BondModelUserRegions[i].RegionParameters[0]),
                                                                                                    (BondModelUserRegions[i].RegionParameters[1]),
                                                                                                    (BondModelUserRegions[i].RegionParameters[2]),
                                                                                                    (BondModelUserRegions[i].RegionParameters[3]),
                                                                out HTuple row1_Rectangle,
                                                                out HTuple column1_Rectangle,
                                                                out HTuple row2_Rectangle,
                                                                out HTuple column2_Rectangle);

                                    BondModelUserRegions[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, BondModelUserRegions[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle);
                                    if (userRegion == null) return;
                                    userRegion.BondMeasureModelParameter = new BondMeasureModelParameter()
                                    {
                                        MeasureContrast = BondModelUserRegions[i].BondMeasureModelParameter.MeasureContrast,
                                        MearsureLength = BondModelUserRegions[i].BondMeasureModelParameter.MearsureLength,
                                        MearsureWideth = BondModelUserRegions[i].BondMeasureModelParameter.MearsureWideth,
                                        MearsureTransition = BondModelUserRegions[i].BondMeasureModelParameter.MearsureTransition
                                    };

                                    userRegion.BondMeasureVerifyParameterSet = new BondMeasureVerifyParameterSet()
                                    {
                                        //BondOffsetFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.BondOffsetFactor,
                                        BondOverSizeFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.BondOverSizeFactor,
                                        BondUnderSizeFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.BondUnderSizeFactor,
                                        PreJudgeEnable = BondModelUserRegions[i].BondMeasureVerifyParameterSet.PreJudgeEnable,
                                        SegThreshGray = BondModelUserRegions[i].BondMeasureVerifyParameterSet.SegThreshGray,
                                        SegRegAreaFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.SegRegAreaFactor
                                    };
                                    BondModelUserRegions[i] = userRegion;
                                    htWindow.DisplayMultiRegion(BondModelRegions);
                                    BondModelUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((BondModelUserRegions[i].RegionParameters[0]),
                                     (BondModelUserRegions[i].RegionParameters[1]),
                                     (BondModelUserRegions[i].RegionParameters[2]),
                                     0, BondModelUserRegions[i].RegionType, 0, 0, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               BondModelUserRegions[i].RegionParameters[0] - 0,
                                                               BondModelUserRegions[i].RegionParameters[1] - 0,
                                                               BondModelUserRegions[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    BondModelUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     BondModelUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle, radius_Circle, 0, 0, 0, 0);
                                    if (userRegion_Circle == null) return;
                                    userRegion_Circle.BondMeasureModelParameter = new BondMeasureModelParameter()
                                    {
                                        MeasureContrast = BondModelUserRegions[i].BondMeasureModelParameter.MeasureContrast,
                                        MearsureLength = BondModelUserRegions[i].BondMeasureModelParameter.MearsureLength,
                                        MearsureWideth = BondModelUserRegions[i].BondMeasureModelParameter.MearsureWideth,
                                        MearsureTransition = BondModelUserRegions[i].BondMeasureModelParameter.MearsureTransition
                                    };

                                    userRegion_Circle.BondMeasureVerifyParameterSet = new BondMeasureVerifyParameterSet()
                                    {
                                        //BondOffsetFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.BondOffsetFactor,
                                        BondOverSizeFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.BondOverSizeFactor,
                                        BondUnderSizeFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.BondUnderSizeFactor,
                                        PreJudgeEnable = BondModelUserRegions[i].BondMeasureVerifyParameterSet.PreJudgeEnable,
                                        SegThreshGray = BondModelUserRegions[i].BondMeasureVerifyParameterSet.SegThreshGray,
                                        SegRegAreaFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.SegRegAreaFactor
                                    };
                                    BondModelUserRegions[i] = userRegion_Circle;
                                    htWindow.DisplayMultiRegion(BondModelRegions);
                                    BondModelUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Ellipse:

                                    htWindow.InitialHWindowUpdate(Math.Floor(BondModelUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(BondModelUserRegions[i].RegionParameters[1]),
                                                                  BondModelUserRegions[i].RegionParameters[2],
                                                                  Math.Ceiling(BondModelUserRegions[i].RegionParameters[3]),
                                                                   BondModelUserRegions[i].RegionType, 0, 0, "yellow");

                                    HOperatorSet.DrawEllipseMod(htWindow.hTWindow.HalconWindow,
                                                               Math.Floor(BondModelUserRegions[i].RegionParameters[0] - 0),
                                                               Math.Floor(BondModelUserRegions[i].RegionParameters[1] - 0),
                                                               BondModelUserRegions[i].RegionParameters[2],
                                                               Math.Floor(BondModelUserRegions[i].RegionParameters[3] - 0),
                                                               Math.Floor(BondModelUserRegions[i].RegionParameters[4] - 0),
                                                           out HTuple row1,
                                                           out HTuple column1,
                                                           out HTuple phi,
                                                           out HTuple radius1,
                                                           out HTuple radius2);

                                    BondModelUserRegions[i].RegionType = RegionType.Ellipse;
                                    UserRegion userRegion_Ellipse = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     BondModelUserRegions[i].RegionType,
                                                                                                     row1, column1, radius1, radius2, 0, 0, phi);
                                    if (userRegion_Ellipse == null) return;
                                    userRegion_Ellipse.BondMeasureModelParameter = new BondMeasureModelParameter()
                                    {
                                        MeasureContrast = BondModelUserRegions[i].BondMeasureModelParameter.MeasureContrast,
                                        MearsureLength = BondModelUserRegions[i].BondMeasureModelParameter.MearsureLength,
                                        MearsureWideth = BondModelUserRegions[i].BondMeasureModelParameter.MearsureWideth,
                                        MearsureTransition = BondModelUserRegions[i].BondMeasureModelParameter.MearsureTransition
                                    };

                                    userRegion_Ellipse.BondMeasureVerifyParameterSet = new BondMeasureVerifyParameterSet()
                                    {
                                        //BondOffsetFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.BondOffsetFactor,
                                        BondOverSizeFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.BondOverSizeFactor,
                                        BondUnderSizeFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.BondUnderSizeFactor,
                                        PreJudgeEnable = BondModelUserRegions[i].BondMeasureVerifyParameterSet.PreJudgeEnable,
                                        SegThreshGray = BondModelUserRegions[i].BondMeasureVerifyParameterSet.SegThreshGray,
                                        SegRegAreaFactor = BondModelUserRegions[i].BondMeasureVerifyParameterSet.SegRegAreaFactor
                                    };
                                    BondModelUserRegions[i] = userRegion_Ellipse;
                                    htWindow.DisplayMultiRegion(BondModelRegions);
                                    BondModelUserRegions[i].Index = i + 1;
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

        private void ExecuteModifyParametersBatchCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (BondModelUserRegions.Count() == 0)
            {
                MessageBox.Show("请添加创建测量模板区域！");
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

        //创建焊点测量模板
        private void ExecuteCreateBondMeasureModelCommand(object parameter)
        {
            if (isRightClick != true) return;//
            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载参考图像!");
                return;
            }
            if (BondModelUserRegions.Count() == 0)
            {
                MessageBox.Show("请添加创建测量模板区域！");
                return;
            }
            if (BondMeasureParameter.ImageCountChannels > 0 && BondMeasureParameter.ImageChannelIndex < 0)
            {
                MessageBox.Show("请先选择通道图像！");
                return;
            }
            //Window_Loading window_Loading = new Window_Loading("正在生成焊点模板");

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

                    //BondMeasureModelParameter.MearsurePara = MetrologyPara;
                }

                //window_Loading.Show();

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
                    BondMeasureParameter.ModelIdPath = $"{bondRecipeDirectory}MetrologyHandle.mtr";
                    HOperatorSet.WriteMetrologyModel(BondMeasureModelObject.MetrologyHandle, BondMeasureParameter.ModelIdPath);
                }
                //window_Loading.Close();
                MessageBox.Show("创建模板完成!");
            }
            catch (Exception ex)
            {
                //window_Loading.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        //检测
        private void ExecuteBondMeasureInspectCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载参考图像！");
                return;
            }
            if (BondMeasureModelObject.MetrologyHandle == null)
            {
                if (File.Exists(BondMeasureParameter.ModelIdPath))
                {
                    HOperatorSet.ReadMetrologyModel(BondMeasureParameter.ModelIdPath, out HTuple metrologyHandle);
                    BondMeasureModelObject.MetrologyHandle = metrologyHandle;
                }
                else
                {
                    MessageBox.Show("请先创建焊点测量模板！");
                    return;
                }
            }

            HOperatorSet.GenEmptyObj(out HObject ImageReduced);
            HOperatorSet.GenEmptyObj(out HObject Contour);
            try
            {
                HOperatorSet.ApplyMetrologyModel(Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex), BondMeasureModelObject.MetrologyHandle);
                HOperatorSet.GetMetrologyObjectResultContour(out Contour, BondMeasureModelObject.MetrologyHandle, "all", "all", 1.5);
                htWindow.DisplayMultiRegion(Contour);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ImageReduced.Dispose();
                Contour.Dispose();
            }
        }

        //框选 
        private void ExecuteRectangle1SelectCommand(object parameter)
        {
            if (BondModelUserRegions.Count == 0 || htWindow.RegionType == RegionType.Null || htWindow.RegionType != RegionType.Rectangle1) return;
            try
            {
                HOperatorSet.GenRectangle1(out HObject rectangle1_sel, htWindow.Row1_Rectangle1, htWindow.Column1_Rectangle1, htWindow.Row2_Rectangle1, htWindow.Column2_Rectangle1);

                foreach (var item in BondModelUserRegions)
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

        private void ExecuteIsCheckCommand(object parameter)
        {
            if (BondModelUserRegions.All(x => x.IsSelected == true))
            { IsCheckAll = true; }
            else if (BondModelUserRegions.All(x => !x.IsSelected))
            { IsCheckAll = false; }
            else
            { IsCheckAll = null; }
        }

        private void ExecuteIsCheckAllCommand(object parameter)
        {
            if (IsCheckAll == true)
            { BondModelUserRegions.ToList().ForEach(r => r.IsSelected = true); }
            else if (IsCheckAll == false)
            { BondModelUserRegions.ToList().ForEach(r => r.IsSelected = false); }
        }

        /*
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
        */

        //保存模板
        private void ExecuteSaveBondModelCommand(object parameter)
        {
            if (BondMeasureModelObject.MetrologyHandle == null)
            {
                MessageBox.Show("请先创建测量模板！");
                return;
            }
            if (BondModelUserRegions.Count() == 0)
            {
                MessageBox.Show("请添加创建测量模板区域！");
                return;
            }
            try
            {
                HOperatorSet.WriteMetrologyModel(BondMeasureModelObject.MetrologyHandle, FilePath.EnsureDirectoryExisted($"{ModelsRecipeDirectory}") + "MetrologyHandle.mtr");
                HOperatorSet.WriteTuple(Algorithm.Region.MetrologyType(BondModelUserRegions), $"{ModelsRecipeDirectory}MetrologyType.tup");
                MessageBox.Show("保存模板完成!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "保存失败");
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
            BondMeasureParameter.IsPickUp = false;
            htWindow.DisplayMultiRegion(BondModelRegions, Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex));

            ChannelNames = new ObservableCollection<ChannelName>(BondMeasureParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            OnPropertyChanged("BondMeasureParameter.ImageChannelIndex");
        }

        public void Dispose()
        {
            (Content as Page_CreateBondMeasureModel).DataContext = null;
            (Content as Page_CreateBondMeasureModel).Close();
            this.Content = null;
            this.htWindow = null;
            this.Bond1AutoUserRegion = null;
            this.ModifyRegionCommand = null;
            this.UserRegionEnableChangedCommand = null;
            this.LoadReferenceCommand = null;
            this.AddUserRegionCommand = null;
            this.RemoveUserRegionCommand = null;
            this.CreateBondMeasureModelCommand = null;
            this.SaveBondModelCommand = null;
            this.BondMeasureInspectCommand = null;
            this.BondMeasureModelObject = null;
            this.BondMeasureParameter = null;
            this.BondMeasureModelParameter = null;
            this.BondModelUserRegions = null;
            this.SelectedUserRegion = null;
            this.ModifyParametersBatchCommand = null;
            this.IsCheckCommand = null;
            this.IsCheckAllCommand = null;
            this.LoadAutoMeasureBondCommand = null;
            this.DisplayBondMeasureRegionsCommand = null;
            this.Rectangle1SelectCommand = null;
        }
    }
}
