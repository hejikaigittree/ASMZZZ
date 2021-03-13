using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace LFAOIRecipe
{
    class CreateRegionModel : ViewModelBase, IProcedure
    {
        //1210
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public static bool isRightClick = true;

        public event Action OnSaveXML;

        public string ReferenceDirectory { get; set; }

        private string ModelsRecipeDirectory;

        private readonly string ModelsFile;

        private readonly string RecipeFile;

        private EpoxyModelObject EpoxyModelObject;

        public ObservableCollection<UserRegion> DieUserRegions { get; private set; } = new ObservableCollection<UserRegion>();

        public ObservableCollection<UserRegion> OperRegionUserRegions { get; private set; }
        private IEnumerable<HObject> OperRegionRegions => OperRegionUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        private HTHalControlWPF htWindow;

        private int imageIndex;
        public int ImageIndex
        {
            get => imageIndex;
            set
            {
                if (imageIndex != value)
                {
                    // 1123
                    if (imageIndex != value)
                    {
                        if (value == -1)
                        {
                            HOperatorSet.AccessChannel(EpoxyModelObject.DieImage, out HObject ChannelImageVerify, 1);
                            htWindow.Display(ChannelImageVerify, true);
                        }
                        else if (value >= 0)
                        {
                            HOperatorSet.AccessChannel(EpoxyModelObject.DieImage, out HObject ChannelImageVerify, value + 1);
                            EpoxyParameter.ImageChannelIndex = value;
                            EpoxyModelObject.DieChannelImage = ChannelImageVerify;
                            htWindow.Display(ChannelImageVerify, true);
                        }
                        imageIndex = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        private int stepTime = 10;
        public int StepTime
        {
            get => stepTime;
            set => OnPropertyChanged(ref stepTime, value);
        }

        private bool? isCheckAll;
        public bool? IsCheckAll
        {
            get => isCheckAll;
            set => OnPropertyChanged(ref isCheckAll, value);
        }

        private string freeRegionName = "FreeRegionModel";
        public string FreeRegionName
        {
            get => freeRegionName;
            set => OnPropertyChanged(ref freeRegionName, value);
        }

        private bool isMargin = true;
        public bool IsMargin
        {
            get => isMargin;
            set => OnPropertyChanged(ref isMargin, value);
        }

        private double[] erosionRadius = { 5 };
        public double[] ErosionRadius
        {
            get => erosionRadius;
            set => OnPropertyChanged(ref erosionRadius, value);
        }

        private double[] dilationRadius = { 5 };
        public double[] DilationRadius
        {
            get => dilationRadius;
            set => OnPropertyChanged(ref dilationRadius, value);
        }

        private double[] maxFillUp = { 500 };
        public double[] MaxFillUp
        {
            get => maxFillUp;
            set => OnPropertyChanged(ref maxFillUp, value);
        }

        private double[] openingCircleSize = { 5 };
        public double[] OpeningCircleSize
        {
            get => openingCircleSize;
            set => OnPropertyChanged(ref openingCircleSize, value);
        }

        private double[] closingCircleSize = { 5 };
        public double[] ClosingCircleSize
        {
            get => closingCircleSize;
            set => OnPropertyChanged(ref closingCircleSize, value);
        }

        private double[] thresValue = { 50, 150 };
        public double[] ThresValue
        {
            get => thresValue;
            set => OnPropertyChanged(ref thresValue, value);
        }

        private double[] areaValue = { 100, 9999 };
        public double[] AreaValue
        {
            get => areaValue;
            set => OnPropertyChanged(ref areaValue, value);
        }

        private int[] regionIdx = { 1, 2 };
        public int[] RegionIdx
        {
            get => regionIdx;
            set => OnPropertyChanged(ref regionIdx, value);
        }

        internal enum RegionTransType
        {
            inner_circle,
            circum_circle,
            smallest_rectangle1,
            smallest_rectangle2,
        }

        private int transType = 0;
        public int TransType
        {
            get => transType;
            set => OnPropertyChanged(ref transType, value);
        }

        private int arrayRegNum = 8 ;
        public int ArrayRegNum
        {
            get => arrayRegNum;
            set => OnPropertyChanged(ref arrayRegNum, value);
        }

        private UserRegion userRegionForCutOut;
        public UserRegion UserRegionForCutOut
        {
            get => userRegionForCutOut;
            set => OnPropertyChanged(ref userRegionForCutOut, value);
        }

        //********** 特征提取 ************       
        internal enum FeatureType
        {
            Center,
            Area,
            Rect2Len,
            Rect2Phi,
            GrayMean,
            GrayDev,
            GrayMaxMin,
        }

        private int featureTypeIndex = 0;
        public int FeatureTypeIndex
        {
            get => featureTypeIndex;
            set => OnPropertyChanged(ref featureTypeIndex, value);
        }

        private HObject _ThresResultRegion;

        public EpoxyParameter EpoxyParameter { get; set; }

        public CommandBase LoadReferenceCommand { get; private set; }
        public CommandBase AddUserRegionCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase UserRegionStepChangedCommand { get; private set; }
        public CommandBase UnionCommand { get; private set; }
        public CommandBase DifferenceCommand { get; private set; }
        public CommandBase ConnectionCommand { get; private set; }
        public CommandBase ErosionCommand { get; private set; }
        public CommandBase DilationCommand { get; private set; }
        public CommandBase FillupCommand { get; private set; }
        public CommandBase OpeningCommand { get; private set; }
        public CommandBase ClosingCommand { get; private set; }
        public CommandBase ThresCommand { get; private set; }
        public CommandBase SelectShapeCommand { get; private set; }
        public CommandBase RegionTransCommand { get; private set; }
        public CommandBase RegionArrayCommand { get; private set; }
        public CommandBase SaveCommand { get; private set; }
        public CommandBase DisplayResultCommand { get; private set; }
        public CommandBase DisplayThresCommand { get; private set; }
        public CommandBase LoadFreeRegionCommond { get; private set; }
        public CommandBase RegionSaveCommand { get; private set; }
        public CommandBase IsCheckCommand { get; private set; }
        public CommandBase IsCheckAllCommand { get; private set; }

        public CreateRegionModel(HTHalControlWPF htWindow,
                              string modelsFile,
                              string recipeFile,
                              string referenceDirectory,
                              EpoxyModelObject epoxyModelObject,
                              EpoxyParameter epoxyParameter,
                              ObservableCollection<UserRegion> operRegionUserRegions,
                              string modelsRecipeDirectory)
        {
            DisplayName = "制作区域模板";
            Content = new Page_RegionModel { DataContext = this };

            this.htWindow = htWindow;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.ReferenceDirectory = referenceDirectory;
            this.EpoxyModelObject = epoxyModelObject;
            this.EpoxyParameter = epoxyParameter;
            this.OperRegionUserRegions = operRegionUserRegions;
            this.ModelsRecipeDirectory = modelsRecipeDirectory;
            AddUserRegionCommand = new CommandBase(ExecuteAddUserRegionCommand);
            RemoveUserRegionCommand = new CommandBase(ExecuteRemoveUserRegionCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            UserRegionStepChangedCommand = new CommandBase(ExecuteUserRegionStepChangedCommand);
            DisplayThresCommand = new CommandBase(ExecuteDisplayThresCommand);
            UnionCommand = new CommandBase(ExecuteUnionCommand);
            DifferenceCommand = new CommandBase(ExecuteDifferenceCommand);
            ConnectionCommand = new CommandBase(ExecuteConnectionCommand);
            ErosionCommand = new CommandBase(ExecuteErosionCommand);
            DilationCommand = new CommandBase(ExecuteDilationCommand);
            FillupCommand = new CommandBase(ExecuteFillupCommand);
            OpeningCommand = new CommandBase(ExecuteOpeningCommand);
            ClosingCommand = new CommandBase(ExecuteClosingCommand);
            ThresCommand = new CommandBase(ExecuteThresCommand);
            SelectShapeCommand = new CommandBase(ExecuteSelectShapeCommand);
            RegionTransCommand = new CommandBase(ExecuteRegionTransCommand);
            RegionArrayCommand = new CommandBase(ExecuteRegionArrayCommand);
            LoadReferenceCommand = new CommandBase(ExecuteLoadReferenceCommand);
            SaveCommand = new CommandBase(ExecuteSaveCommand);
            DisplayResultCommand = new CommandBase(ExecuteDisplayResultCommand);
            LoadFreeRegionCommond = new CommandBase(ExecuteLoadFreeRegionCommond);
            RegionSaveCommand = new CommandBase(ExecuteRegionSaveCommand);
            IsCheckCommand = new CommandBase(ExecuteIsCheckCommand);
            IsCheckAllCommand = new CommandBase(ExecuteIsCheckAllCommand);
        }

        //加载参考
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

                HTuple files = new HTuple();
                files = Directory.GetDirectories(Directory.GetParent(ModelsFile).ToString(), "Frame*.*", SearchOption.TopDirectoryOnly);
                string[] OnRecipesIndexs = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    OnRecipesIndexs[i] = Path.GetFileName(files[i]);
                }
                EpoxyParameter.OnRecipesIndexs = OnRecipesIndexs;

                if (OperRegionUserRegions.Count() != 0)
                { 
                    RunNewSteps(OperRegionUserRegions.Count);
                }
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

            EpoxyParameter.ImagePath = $"{ReferenceDirectory}ReferenceImage.tiff";
            EpoxyModelObject.Image?.Dispose();
            LoadImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageRowOffset.tup", out HTuple dieImageRowOffset);
            EpoxyParameter.DieImageRowOffset = dieImageRowOffset;
            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageColumnOffset.tup", out HTuple dieImageColumnOffset);
            EpoxyParameter.DieImageColumnOffset = dieImageColumnOffset;

            HOperatorSet.ReadRegion(out HObject UserRegionForCutOut_Region, ReferenceDirectory + "DieReference.reg");
            HOperatorSet.ReduceDomain(EpoxyModelObject.Image, UserRegionForCutOut_Region, out HObject imageReduced);
            HOperatorSet.CropDomain(imageReduced, out HObject dieImage);
            EpoxyModelObject.DieImage = dieImage;
            LoadDieImage();

            //1121 lht
            HOperatorSet.ReadTuple(ReferenceDirectory + "ImageChannelnumber.tup", out HTuple ImageChannelnumber);
            ChannelNames.Clear();
            for (int i = 0; i < ImageChannelnumber; i++)
            {
                ChannelName tmp_name = new ChannelName();
                tmp_name.Name = (i + 1).ToString();
                ChannelNames.Add(tmp_name);
            }

            imageIndex = EpoxyParameter.ImageChannelIndex;
            OnPropertyChanged("imageIndex");

            HOperatorSet.ReadTuple(ReferenceDirectory + "UserRegionForCutOutIndex.tup", out HTuple UserRegionForCutOutIndexTemp);
            EpoxyParameter.UserRegionForCutOutIndex = UserRegionForCutOutIndexTemp;
            UserRegionForCutOut = DieUserRegions.Where(u => u.Index == EpoxyParameter.UserRegionForCutOutIndex).FirstOrDefault();
            EpoxyModelObject.UserRegionForCutOut = UserRegionForCutOut;

            htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyParameter.ImageChannelIndex));
        }

        public void LoadImage()
        {
            HOperatorSet.GenEmptyObj(out HObject image);
            HOperatorSet.ReadImage(out image, EpoxyParameter.ImagePath);
            htWindow.Display(image, true);
            EpoxyModelObject.Image = image;

            HOperatorSet.CountChannels(EpoxyModelObject.Image, out HTuple channels);
            EpoxyParameter.ImageCountChannels = channels;
            EpoxyModelObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, EpoxyParameter.ImageChannelIndex);
            //EpoxyModelObject.ImageR = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, 1);
            //EpoxyModelObject.ImageG = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, 2);
            //EpoxyModelObject.ImageB = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, 3);
        }

        public void LoadDieImage()
        {
            try
            {
                EpoxyModelObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyParameter.ImageChannelIndex);
                //EpoxyModelObject.DieImageR = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, 1);
                //EpoxyModelObject.DieImageG = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, 2);
                //EpoxyModelObject.DieImageB = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //结果显示
        private void ExecuteDisplayResultCommand(object parameter)
        {
            if (_ThresResultRegion == null) return;
            if (OperRegionUserRegions.Count() == 0) return;
            HObject resultRegion = RunNewSteps(OperRegionUserRegions.Last().Index);
            htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
            htWindow.DisplaySingleRegionSetDraw(resultRegion);
        }

        private void ExecuteDisplayThresCommand(object parameter)
        {
            if (OperRegionUserRegions.Count() == 0) return;
            UserRegion userRegion = parameter as UserRegion;
            if (userRegion == null) return;
            htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
            htWindow.DisplaySingleRegionSetDraw(userRegion.DisplayRegion);
        }

        //添加
        private void ExecuteAddUserRegionCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    if (htWindow.RegionType == RegionType.Null)
                    {
                        System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                        return;
                    }
                    UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                             EpoxyParameter.DieImageRowOffset,
                                             EpoxyParameter.DieImageColumnOffset);
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.DisplayMultiRegion(OperRegionRegions);
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
        }

        //删除
        private void ExecuteRemoveUserRegionCommand(object parameter)
        {
            if (isRightClick)
            {
                if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    for (int i = 0; i < OperRegionUserRegions.Count; i++)
                    {
                        if (OperRegionUserRegions[i].IsSelected)
                        {
                            OperRegionUserRegions.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            OperRegionUserRegions[i].Index = i + 1;
                        }
                    }
                    if (OperRegionUserRegions.Count==0)
                    {
                        IsCheckAll = false;
                    }
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegion(OperRegionRegions);
                }
            }
        }

        //修改
        private void ExecuteModifyRegionCommand(object parameter)//
        {
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    for (int i = 0; i < OperRegionUserRegions.Count; i++)
                    {
                        if (OperRegionUserRegions[i].IsSelected)
                        {
                            switch (OperRegionUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(OperRegionUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(OperRegionUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(OperRegionUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(OperRegionUserRegions[i].RegionParameters[3]),
                                         OperRegionUserRegions[i].RegionType, EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, 
                                                     (OperRegionUserRegions[i].RegionParameters[0] - EpoxyParameter.DieImageRowOffset),
                                                     (OperRegionUserRegions[i].RegionParameters[1] - EpoxyParameter.DieImageColumnOffset),
                                                     (OperRegionUserRegions[i].RegionParameters[2] - EpoxyParameter.DieImageRowOffset),
                                                     (OperRegionUserRegions[i].RegionParameters[3] - EpoxyParameter.DieImageColumnOffset),
                                                        out HTuple row1_Rectangle,
                                                        out HTuple column1_Rectangle,
                                                        out HTuple row2_Rectangle,
                                                        out HTuple column2_Rectangle);

                                    OperRegionUserRegions[i].RegionType = RegionType.Rectangle1;
                                    //Die图
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, OperRegionUserRegions[i].RegionType, 
                                                            row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle,
                                                            EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    userRegion.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    OperRegionUserRegions[i] = userRegion;
                                    htWindow.DisplayMultiRegion(OperRegionRegions);
                                    OperRegionUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(OperRegionUserRegions[i].RegionParameters[0]),
                                          Math.Floor(OperRegionUserRegions[i].RegionParameters[1]),
                                          Math.Ceiling(OperRegionUserRegions[i].RegionParameters[2]),
                                          Math.Ceiling(OperRegionUserRegions[i].RegionParameters[3]),
                                          OperRegionUserRegions[i].RegionType,EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, "yellow");
                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(OperRegionUserRegions[i].RegionParameters[0] - EpoxyParameter.DieImageRowOffset),
                                                                   Math.Floor(OperRegionUserRegions[i].RegionParameters[1] - EpoxyParameter.DieImageColumnOffset),
                                                                   OperRegionUserRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(OperRegionUserRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(OperRegionUserRegions[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    OperRegionUserRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, OperRegionUserRegions[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         EpoxyParameter.DieImageRowOffset, 
                                                                                                         EpoxyParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    userRegion_Rectangle2.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_Rectangle2.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    OperRegionUserRegions[i] = userRegion_Rectangle2;
                                    OperRegionUserRegions[i].Index = i + 1;
                                    htWindow.DisplayMultiRegion(OperRegionRegions);
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((OperRegionUserRegions[i].RegionParameters[0]),
                                     (OperRegionUserRegions[i].RegionParameters[1]),
                                     (OperRegionUserRegions[i].RegionParameters[2]),
                                     0, OperRegionUserRegions[i].RegionType, EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               OperRegionUserRegions[i].RegionParameters[0] -EpoxyParameter.DieImageRowOffset,
                                                               OperRegionUserRegions[i].RegionParameters[1] -EpoxyParameter.DieImageColumnOffset,
                                                               OperRegionUserRegions[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    OperRegionUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     OperRegionUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle, radius_Circle, 0, EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, 0);
                                    if (userRegion_Circle == null) return;
                                    userRegion_Circle.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_Circle.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    OperRegionUserRegions[i] = userRegion_Circle;
                                    htWindow.DisplayMultiRegion(OperRegionRegions);
                                    OperRegionUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Ellipse:
                                    htWindow.InitialHWindowUpdate(Math.Floor(OperRegionUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(OperRegionUserRegions[i].RegionParameters[1]),
                                                                  OperRegionUserRegions[i].RegionParameters[2],
                                                                  Math.Ceiling(OperRegionUserRegions[i].RegionParameters[3]),
                                                                   OperRegionUserRegions[i].RegionType, EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawEllipseMod(htWindow.hTWindow.HalconWindow,
                                                               Math.Floor(OperRegionUserRegions[i].RegionParameters[0] - EpoxyParameter.DieImageRowOffset),
                                                               Math.Floor(OperRegionUserRegions[i].RegionParameters[1] -EpoxyParameter.DieImageColumnOffset),
                                                               OperRegionUserRegions[i].RegionParameters[2],
                                                               Math.Floor(OperRegionUserRegions[i].RegionParameters[3]),
                                                               Math.Floor(OperRegionUserRegions[i].RegionParameters[4]),
                                                           out HTuple row1,
                                                           out HTuple column1,
                                                           out HTuple phi,
                                                           out HTuple radius1,
                                                           out HTuple radius2);

                                    OperRegionUserRegions[i].RegionType = RegionType.Ellipse;
                                    UserRegion userRegion_Ellipse = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     OperRegionUserRegions[i].RegionType,
                                                                                                     row1, column1, radius1, radius2, EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, phi);
                                    if (userRegion_Ellipse == null) return;
                                    userRegion_Ellipse.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_Ellipse.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    OperRegionUserRegions[i] = userRegion_Ellipse;
                                    htWindow.DisplayMultiRegion(OperRegionRegions);
                                    OperRegionUserRegions[i].Index = i + 1;
                                    break;



                                default:
                                    break;
                            }

                            switch(OperRegionUserRegions[i].RegionOperatType)
                            {
                                case RegionOperatType.Union:
                                    HOperatorSet.Union2(RunNewSteps(regionIdx[0]), RunNewSteps(regionIdx[1]), out HObject regionUnion);

                                    UserRegion userRegion_Union = new UserRegion
                                    {
                                        DisplayRegion = regionUnion,
                                        CalculateRegion = regionUnion,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.Union,
                                        RegionParameters = new double[2] { regionIdx[0], regionIdx[1] },
                                    };
                                    if (userRegion_Union == null) return;
                                    userRegion_Union.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_Union.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_Union.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_Union;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(regionUnion);
                                    break;

                                case RegionOperatType.Difference:
                                    HOperatorSet.Difference(RunNewSteps(regionIdx[0]), RunNewSteps(regionIdx[1]), out HObject regionDifference);

                                    UserRegion userRegion_Difference = new UserRegion
                                    {
                                        DisplayRegion = regionDifference,
                                        CalculateRegion = regionDifference,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.Difference,
                                        RegionParameters = new double[2] { regionIdx[0], regionIdx[1] },
                                    };
                                    if (userRegion_Difference == null) return;
                                    userRegion_Difference.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_Difference.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_Difference.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_Difference;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(regionDifference);
                                    break;

                                case RegionOperatType.Connection:
                                    HOperatorSet.Connection(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HObject regionConnection);

                                    UserRegion userRegion_Connection = new UserRegion
                                    {
                                        DisplayRegion = regionConnection,
                                        CalculateRegion = regionConnection,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.Connection,
                                        RegionParameters = new double[] { },
                                    };
                                    if (userRegion_Connection == null) return;
                                    userRegion_Connection.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_Connection.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_Connection.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_Connection;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(regionConnection);
                                    break;

                                case RegionOperatType.Threshold:
                                    HOperatorSet.GenEmptyObj(out HObject imageReduced);
                                    HOperatorSet.ReduceDomain(EpoxyModelObject.DieChannelImage, RunNewSteps(OperRegionUserRegions.Last().Index - 1), out imageReduced);
                                    HOperatorSet.Threshold(imageReduced, out HObject _ResultRegion, thresValue[0], thresValue[1]);
                                    imageReduced.Dispose();

                                    UserRegion userRegion_Region = new UserRegion
                                    {
                                        DisplayRegion = _ResultRegion,
                                        CalculateRegion = _ResultRegion,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.Threshold,
                                        RegionParameters = new double[2] { thresValue[0], thresValue[1] },
                                    };
                                    if (userRegion_Region == null) return;
                                    userRegion_Region.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_Region.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    OperRegionUserRegions[i] = userRegion_Region;
                                    OperRegionUserRegions[i].Index = i + 1;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplaySingleRegionSetDraw(_ResultRegion);
                                    break;

                                case RegionOperatType.Erosion:
                                    HOperatorSet.ErosionCircle(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HObject erosion, erosionRadius[0]);
                                    UserRegion userRegion_erosion = new UserRegion
                                    {
                                        DisplayRegion = erosion,
                                        CalculateRegion = erosion,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.Erosion,
                                        RegionParameters = new double[1] { erosionRadius[0] },

                                    };
                                    if (userRegion_erosion == null) return;
                                    userRegion_erosion.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_erosion.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_erosion.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_erosion;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(erosion);
                                    break;

                                case RegionOperatType.Dilation:
                                    HOperatorSet.DilationCircle(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HObject dilation, dilationRadius[0]);
                                    UserRegion userRegion_dilation = new UserRegion
                                    {
                                        DisplayRegion = dilation,
                                        CalculateRegion = dilation,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.Dilation,
                                        RegionParameters = new double[1] { erosionRadius[0] },

                                    };
                                    if (userRegion_dilation == null) return;
                                    userRegion_dilation.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_dilation.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_dilation.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_dilation;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(dilation);
                                    break;

                                case RegionOperatType.Fillup:
                                    HOperatorSet.FillUpShape(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HObject regionFillUp, "area", 0, maxFillUp[0]);
                                    UserRegion userRegion_fillup = new UserRegion
                                    {
                                        DisplayRegion = regionFillUp,
                                        CalculateRegion = regionFillUp,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.Fillup,
                                        RegionParameters = new double[1] { maxFillUp[0] },
                                    };
                                    if (userRegion_fillup == null) return;
                                    userRegion_fillup.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_fillup.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_fillup.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_fillup;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(regionFillUp);
                                    break;

                                case RegionOperatType.OpeningCircle:
                                    HOperatorSet.OpeningCircle(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HObject regionOpeningCircle, openingCircleSize[0]);
                                    UserRegion userRegion_OpeningCircle = new UserRegion
                                    {
                                        DisplayRegion = regionOpeningCircle,
                                        CalculateRegion = regionOpeningCircle,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.OpeningCircle,
                                        RegionParameters = new double[1] { openingCircleSize[0] },
                                    };
                                    if (userRegion_OpeningCircle == null) return;
                                    userRegion_OpeningCircle.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_OpeningCircle.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_OpeningCircle.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_OpeningCircle;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(regionOpeningCircle);
                                    break;

                                case RegionOperatType.ClosingCircle:
                                    HOperatorSet.ClosingCircle(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HObject regionClosingCircle, closingCircleSize[0]);
                                    UserRegion userRegion_ClosingCircle = new UserRegion
                                    {
                                        DisplayRegion = regionClosingCircle,
                                        CalculateRegion = regionClosingCircle,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.ClosingCircle,
                                        RegionParameters = new double[1] { closingCircleSize[0] },
                                    };
                                    if (userRegion_ClosingCircle == null) return;
                                    userRegion_ClosingCircle.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_ClosingCircle.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_ClosingCircle.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_ClosingCircle;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(regionClosingCircle);
                                    break;

                                case RegionOperatType.SelectShape:
                                    HOperatorSet.Connection(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HObject _RegionConnection);
                                    HOperatorSet.SelectShape(_RegionConnection, out HObject _ResultRegionSelect, "area", "and", areaValue[0], areaValue[1]);
                                    HOperatorSet.Union1(_ResultRegionSelect, out HObject regionSelectShape);
                                    UserRegion userRegion_SelectShape = new UserRegion
                                    {
                                        DisplayRegion = regionSelectShape,
                                        CalculateRegion = regionSelectShape,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.SelectShape,
                                        RegionParameters = new double[2] { areaValue[0], areaValue[1] },
                                    };
                                    if (userRegion_SelectShape == null) return;
                                    userRegion_SelectShape.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_SelectShape.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_SelectShape.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_SelectShape;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(regionSelectShape);
                                    break;

                                case RegionOperatType.RegionTrans:
                                    HObject regionTrans = null;
                                    switch (transType)
                                    {
                                        case 0:
                                            HOperatorSet.InnerCircle(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HTuple RowIn, out HTuple ColIn, out HTuple RadiusIn);
                                            HOperatorSet.GenCircle(out regionTrans, RowIn, ColIn, RadiusIn);
                                            break;

                                        case 1:
                                            HOperatorSet.SmallestCircle(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HTuple RowOut, out HTuple ColOut, out HTuple RadiusOut);
                                            HOperatorSet.GenCircle(out regionTrans, RowOut, ColOut, RadiusOut);
                                            break;

                                        case 2:
                                            HOperatorSet.SmallestRectangle1(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HTuple Row01, out HTuple Col02, out HTuple Row11, out HTuple Col12);
                                            HOperatorSet.GenRectangle1(out regionTrans, Row01, Col02, Row11, Col12);
                                            break;

                                        case 3:
                                            HOperatorSet.SmallestRectangle2(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out HTuple RowCenter, out HTuple ColCenter, out HTuple Phi, out HTuple Len1, out HTuple Len2);
                                            HOperatorSet.GenRectangle2(out regionTrans, RowCenter, ColCenter, Phi, Len1, Len2);
                                            break;

                                        default:
                                            break;
                                    }
                                    UserRegion userRegion_RegionTrans = new UserRegion
                                    {
                                        DisplayRegion = regionTrans,
                                        CalculateRegion = regionTrans,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.RegionTrans,
                                        RegionParameters = new double[1] { transType },
                                    };
                                    if (userRegion_RegionTrans == null) return;
                                    userRegion_RegionTrans.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_RegionTrans.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_RegionTrans.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_RegionTrans;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(regionTrans);
                                    break;

                                case RegionOperatType.RegionArray:

                                    HTuple hv_row = null, hv_col = null, hv_phi = null, hv_len1 = null;
                                    HTuple hv_len2 = null, hv_RegNum = null, hv_Radius = null;
                                    HTuple hv_startPtRow = new HTuple(), hv_startPtCol = new HTuple();
                                    HTuple hv_endPtRow = new HTuple(), hv_endPtCol = new HTuple();
                                    HTuple hv_gapRow = new HTuple(), hv_gapCol = new HTuple();
                                    HTuple hv_RowTup = new HTuple(), hv_ColTup = new HTuple();
                                    HTuple hv_RadiusTup = new HTuple();
                                    HObject regionArray = null;

                                    HOperatorSet.SmallestRectangle2(RunNewSteps(OperRegionUserRegions.Last().Index - 1), out hv_row, out hv_col,
                                                                             out hv_phi, out hv_len1, out hv_len2);

                                    hv_RegNum = arrayRegNum;
                                    hv_Radius = 12;  // 默认值

                                    if ((int)(new HTuple(hv_RegNum.TupleGreater(1))) != 0)
                                    {
                                        //起始点
                                        hv_startPtRow = hv_row + ((hv_len1 - hv_Radius) * (hv_phi.TupleSin()));
                                        hv_startPtCol = hv_col - ((hv_len1 - hv_Radius) * (hv_phi.TupleCos()));
                                        //终止点
                                        hv_endPtRow = hv_row - ((hv_len1 - hv_Radius) * (hv_phi.TupleSin()));
                                        hv_endPtCol = hv_col + ((hv_len1 - hv_Radius) * (hv_phi.TupleCos()));
                                        //间距
                                        hv_gapRow = (hv_endPtRow - hv_startPtRow) / (hv_RegNum - 1);
                                        hv_gapCol = (hv_endPtCol - hv_startPtCol) / (hv_RegNum - 1);
                                        if ((int)(new HTuple(hv_gapRow.TupleNotEqual(0))) != 0)
                                        {
                                            hv_RowTup = HTuple.TupleGenSequence(hv_startPtRow, hv_endPtRow, hv_gapRow);
                                        }
                                        else
                                        {
                                            HOperatorSet.TupleGenConst(hv_RegNum, hv_startPtRow, out hv_RowTup);
                                        }
                                        if ((int)(new HTuple(hv_gapCol.TupleNotEqual(0))) != 0)
                                        {
                                            hv_ColTup = HTuple.TupleGenSequence(hv_startPtCol, hv_endPtCol, hv_gapCol);
                                        }
                                        else
                                        {
                                            HOperatorSet.TupleGenConst(hv_RegNum, hv_startPtCol, out hv_ColTup);
                                        }
                                        HOperatorSet.TupleGenConst(new HTuple(hv_RowTup.TupleLength()), hv_Radius, out hv_RadiusTup);
                                        //生成阵列圆
                                        HOperatorSet.GenCircle(out regionArray, hv_RowTup, hv_ColTup, hv_RadiusTup);
                                    }
                                    else
                                    {
                                        //生成单个圆
                                        HOperatorSet.GenCircle(out regionArray, hv_row, hv_col, hv_Radius);
                                    }

                                    HOperatorSet.Union1(regionArray, out regionArray);

                                    UserRegion userRegion_RegionArray = new UserRegion
                                    {
                                        DisplayRegion = regionArray,
                                        CalculateRegion = regionArray,
                                        RegionType = RegionType.Operation,
                                        RegionOperatType = RegionOperatType.RegionArray,
                                        RegionParameters = new double[1] { arrayRegNum },
                                    };
                                    if (userRegion_RegionArray == null) return;
                                    userRegion_RegionArray.LastIndex = OperRegionUserRegions[i].LastIndex;
                                    userRegion_RegionArray.IsAccept = OperRegionUserRegions[i].IsAccept;
                                    userRegion_RegionArray.Index = i + 1;
                                    OperRegionUserRegions[i] = userRegion_RegionArray;
                                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                                    htWindow.DisplayMultiRegionSetDisplay(regionArray);
                                    break;
                                default:
                                    break;
                            }
                         }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void ExecuteUserRegionEnableChangedCommand(object parameter)
        {
            if (isRightClick)
            {
                UserRegion userRegion = parameter as UserRegion;
                if (userRegion == null) return;
                htWindow.DisplayMultiRegion(OperRegionRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyParameter.ImageChannelIndex));
            }
        }
         
        //运行
        private void ExecuteUserRegionStepChangedCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    UserRegion _userRegion = parameter as UserRegion;
                    if (_userRegion == null) return;
                    RunNewSteps(_userRegion.Index);
                    foreach (var item in OperRegionUserRegions)
                    {
                        item.IsCurrentStep = false;
                    }
                    OperRegionUserRegions[_userRegion.Index - 1].IsCurrentStep = true;
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplaySingleRegionSetDraw(RunNewSteps(_userRegion.Index));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }               
            }
        }

        public HObject RunSteps(int runToIndex)
        {
            if (EpoxyModelObject.DieChannelImage == null) return null;
            if (OperRegionUserRegions.Count == 0) return null;
            HOperatorSet.GenEmptyObj(out _ThresResultRegion);

            for (int i = 0; i < runToIndex; i++)
            {
                switch (OperRegionUserRegions[i].RegionType)
                {
                    case RegionType.Rectangle1:
                        _ThresResultRegion = Algorithm.Region.Union1DisplayRegion(_ThresResultRegion, OperRegionUserRegions[i]);
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionType.Rectangle2:
                        _ThresResultRegion = Algorithm.Region.Union1DisplayRegion(_ThresResultRegion, OperRegionUserRegions[i]);
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionType.Circle:
                        _ThresResultRegion = Algorithm.Region.Union1DisplayRegion(_ThresResultRegion, OperRegionUserRegions[i]);
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionType.Ellipse:
                        _ThresResultRegion = Algorithm.Region.Union1DisplayRegion(_ThresResultRegion, OperRegionUserRegions[i]);
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionType.Region:
                        _ThresResultRegion = Algorithm.Region.Union1DisplayRegion(_ThresResultRegion, OperRegionUserRegions[i]);
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionType.Operation:

                        break;
                    default:
                        MessageBox.Show("请重新选择画区域工具！");
                        break;
                }
                //add by wj 12-24
                switch (OperRegionUserRegions[i].RegionOperatType)
                {
                    case RegionOperatType.Union:
                        HOperatorSet.Union2(OperRegionUserRegions[(Int32)(OperRegionUserRegions[i].RegionParameters[0]) - 1].DisplayRegion,
                                            OperRegionUserRegions[(Int32)(OperRegionUserRegions[i].RegionParameters[1]) - 1].DisplayRegion,
                                            out _ThresResultRegion);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Difference:
                        HOperatorSet.Difference(OperRegionUserRegions[(Int32)(OperRegionUserRegions[i].RegionParameters[0]) - 1].DisplayRegion,
                                                OperRegionUserRegions[(Int32)(OperRegionUserRegions[i].RegionParameters[1]) - 1].DisplayRegion,
                                                out _ThresResultRegion);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Connection:
                        HOperatorSet.Connection(_ThresResultRegion, out _ThresResultRegion);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Threshold:
                        HOperatorSet.ReduceDomain(EpoxyModelObject.DieChannelImage, _ThresResultRegion, out HObject imageReduced);
                        HOperatorSet.Threshold(imageReduced, out _ThresResultRegion, OperRegionUserRegions[i].RegionParameters[0], OperRegionUserRegions[i].RegionParameters[1]);
                        imageReduced.Dispose();
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Erosion:
                        HOperatorSet.ErosionCircle(_ThresResultRegion, out _ThresResultRegion, OperRegionUserRegions[i].RegionParameters[0]);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Dilation:
                        HOperatorSet.DilationCircle(_ThresResultRegion, out _ThresResultRegion, OperRegionUserRegions[i].RegionParameters[0]);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Fillup:
                        HOperatorSet.FillUpShape(_ThresResultRegion, out _ThresResultRegion, "area", 0, OperRegionUserRegions[i].RegionParameters[0]);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.OpeningCircle:
                        HOperatorSet.OpeningCircle(_ThresResultRegion, out _ThresResultRegion, OperRegionUserRegions[i].RegionParameters[0]);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.ClosingCircle:
                        HOperatorSet.ClosingCircle(_ThresResultRegion, out _ThresResultRegion, OperRegionUserRegions[i].RegionParameters[0]);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.SelectShape:
                        HOperatorSet.Connection(_ThresResultRegion, out HObject _RegionConnection);
                        HOperatorSet.SelectShape(_RegionConnection, out HObject _ResultRegionSelect, "area", "and", OperRegionUserRegions[i].RegionParameters[0], OperRegionUserRegions[i].RegionParameters[1]);
                        HOperatorSet.Union1(_ResultRegionSelect, out _ThresResultRegion);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.RegionTrans:

                        switch (OperRegionUserRegions[i].RegionParameters[0])
                        {
                            case 0:
                                HOperatorSet.InnerCircle(_ThresResultRegion, out HTuple RowIn, out HTuple ColIn, out HTuple RadiusIn);
                                HOperatorSet.GenCircle(out _ThresResultRegion, RowIn, ColIn, RadiusIn);
                                break;

                            case 1:
                                HOperatorSet.SmallestCircle(_ThresResultRegion, out HTuple RowOut, out HTuple ColOut, out HTuple RadiusOut);
                                HOperatorSet.GenCircle(out _ThresResultRegion, RowOut, ColOut, RadiusOut);
                                break;

                            case 2:
                                HOperatorSet.SmallestRectangle1(_ThresResultRegion, out HTuple Row01, out HTuple Col02, out HTuple Row11, out HTuple Col12);
                                HOperatorSet.GenRectangle1(out _ThresResultRegion, Row01, Col02, Row11, Col12);
                                break;

                            case 3:
                                HOperatorSet.SmallestRectangle2(_ThresResultRegion, out HTuple RowCenter, out HTuple ColCenter, out HTuple Phi, out HTuple Len1, out HTuple Len2);
                                HOperatorSet.GenRectangle2(out _ThresResultRegion, RowCenter, ColCenter, Phi, Len1, Len2);
                                break;

                            default:
                                break;
                        }
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.RegionArray:

                        HTuple hv_row = null, hv_col = null, hv_phi = null, hv_len1 = null;
                        HTuple hv_len2 = null, hv_RegNum = null, hv_Radius = null;
                        HTuple hv_startPtRow = new HTuple(), hv_startPtCol = new HTuple();
                        HTuple hv_endPtRow = new HTuple(), hv_endPtCol = new HTuple();
                        HTuple hv_gapRow = new HTuple(), hv_gapCol = new HTuple();
                        HTuple hv_RowTup = new HTuple(), hv_ColTup = new HTuple();
                        HTuple hv_RadiusTup = new HTuple();

                        HOperatorSet.SmallestRectangle2(_ThresResultRegion, out hv_row, out hv_col, out hv_phi, out hv_len1, out hv_len2);

                        hv_RegNum = OperRegionUserRegions[i].RegionParameters[0];
                        hv_Radius = 12;  // 默认值

                        if ((int)(new HTuple(hv_RegNum.TupleGreater(1))) != 0)
                        {
                            //起始点
                            hv_startPtRow = hv_row + ((hv_len1 - hv_Radius) * (hv_phi.TupleSin()));
                            hv_startPtCol = hv_col - ((hv_len1 - hv_Radius) * (hv_phi.TupleCos()));
                            //终止点
                            hv_endPtRow = hv_row - ((hv_len1 - hv_Radius) * (hv_phi.TupleSin()));
                            hv_endPtCol = hv_col + ((hv_len1 - hv_Radius) * (hv_phi.TupleCos()));
                            //间距
                            hv_gapRow = (hv_endPtRow - hv_startPtRow) / (hv_RegNum - 1);
                            hv_gapCol = (hv_endPtCol - hv_startPtCol) / (hv_RegNum - 1);
                            if ((int)(new HTuple(hv_gapRow.TupleNotEqual(0))) != 0)
                            {
                                hv_RowTup = HTuple.TupleGenSequence(hv_startPtRow, hv_endPtRow, hv_gapRow);
                            }
                            else
                            {
                                HOperatorSet.TupleGenConst(hv_RegNum, hv_startPtRow, out hv_RowTup);
                            }
                            if ((int)(new HTuple(hv_gapCol.TupleNotEqual(0))) != 0)
                            {
                                hv_ColTup = HTuple.TupleGenSequence(hv_startPtCol, hv_endPtCol, hv_gapCol);
                            }
                            else
                            {
                                HOperatorSet.TupleGenConst(hv_RegNum, hv_startPtCol, out hv_ColTup);
                            }
                            HOperatorSet.TupleGenConst(new HTuple(hv_RowTup.TupleLength()), hv_Radius, out hv_RadiusTup);
                            //生成阵列圆
                            HOperatorSet.GenCircle(out _ThresResultRegion, hv_RowTup, hv_ColTup, hv_RadiusTup);
                        }
                        else
                        {
                            //生成单个圆
                            HOperatorSet.GenCircle(out _ThresResultRegion, hv_row, hv_col, hv_Radius);
                        }

                        HOperatorSet.Union1(_ThresResultRegion, out _ThresResultRegion);

                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);

                        break;
                    case RegionOperatType.Null:

                        break;
                    default:
                        MessageBox.Show("请重新选择操作区域工具！");
                        break;
                }
            }
            return _ThresResultRegion;
        }


        public HObject RunNewSteps(int runToIndex)
        {
            if (EpoxyModelObject.DieChannelImage == null) return null;
            if (OperRegionUserRegions.Count == 0) return null;
            HOperatorSet.GenEmptyObj(out _ThresResultRegion);

            // mod lw 按前节点路径生成
            List<int> LastIdxPath = new List<int>();

            for (int i = runToIndex-1; i >= 0; i--)
            {
                int PathEndFlag = 0;
                int lastIdx = OperRegionUserRegions[i].LastIndex;

                if ((lastIdx == 0) && (OperRegionUserRegions[i].Index != 1))
                {
                    i = OperRegionUserRegions[i - 1].Index;
                    PathEndFlag = 1;
                }
                else
                { 
                    i = lastIdx;
                }

                LastIdxPath.Add(lastIdx);

                if(PathEndFlag == 1)
                { 
                    LastIdxPath.Add(i);
                }
            }

            LastIdxPath.Reverse();
            LastIdxPath.Add(runToIndex);

            for (int j = 0; j < LastIdxPath.Count; j++)
            {
                int i = LastIdxPath[j] - 1;

                if(LastIdxPath[j] == 0)
                {
                    HOperatorSet.GenEmptyObj(out _ThresResultRegion);
                    continue;
                }

                switch (OperRegionUserRegions[i].RegionType)
                {
                    case RegionType.Rectangle1:
                        _ThresResultRegion = Algorithm.Region.Union1DisplayRegion(_ThresResultRegion, OperRegionUserRegions[i]);
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionType.Rectangle2:
                        _ThresResultRegion = Algorithm.Region.Union1DisplayRegion(_ThresResultRegion, OperRegionUserRegions[i]);
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionType.Circle:
                        _ThresResultRegion = Algorithm.Region.Union1DisplayRegion(_ThresResultRegion, OperRegionUserRegions[i]);
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionType.Ellipse:
                        _ThresResultRegion = Algorithm.Region.Union1DisplayRegion(_ThresResultRegion, OperRegionUserRegions[i]);
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionType.Region:
                        _ThresResultRegion = Algorithm.Region.Union1DisplayRegion(_ThresResultRegion, OperRegionUserRegions[i]);
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionType.Operation:

                        break;

                    default:
                        MessageBox.Show("请重新选择画区域工具！");
                        break;
                }

                switch (OperRegionUserRegions[i].RegionOperatType)
                {
                    case RegionOperatType.Union:
                        HOperatorSet.Union2(OperRegionUserRegions[(Int32)(OperRegionUserRegions[i].RegionParameters[0]) - 1].DisplayRegion,
                                            OperRegionUserRegions[(Int32)(OperRegionUserRegions[i].RegionParameters[1]) - 1].DisplayRegion,
                                            out _ThresResultRegion);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Difference:
                        HOperatorSet.Difference(OperRegionUserRegions[(Int32)(OperRegionUserRegions[i].RegionParameters[0]) - 1].DisplayRegion,
                                                OperRegionUserRegions[(Int32)(OperRegionUserRegions[i].RegionParameters[1]) - 1].DisplayRegion,
                                                out _ThresResultRegion);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Connection:
                        HOperatorSet.Connection(_ThresResultRegion, out _ThresResultRegion);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Threshold:
                        HOperatorSet.ReduceDomain(EpoxyModelObject.DieChannelImage, _ThresResultRegion, out HObject imageReduced);
                        HOperatorSet.Threshold(imageReduced, out _ThresResultRegion, OperRegionUserRegions[i].RegionParameters[0], OperRegionUserRegions[i].RegionParameters[1]);
                        imageReduced.Dispose();
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Erosion:
                        HOperatorSet.ErosionCircle(_ThresResultRegion, out _ThresResultRegion, OperRegionUserRegions[i].RegionParameters[0]);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Dilation:
                        HOperatorSet.DilationCircle(_ThresResultRegion, out _ThresResultRegion, OperRegionUserRegions[i].RegionParameters[0]);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.Fillup:
                        HOperatorSet.FillUpShape(_ThresResultRegion, out _ThresResultRegion, "area", 0, OperRegionUserRegions[i].RegionParameters[0]);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.OpeningCircle:
                        HOperatorSet.OpeningCircle(_ThresResultRegion, out _ThresResultRegion, OperRegionUserRegions[i].RegionParameters[0]);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.ClosingCircle:
                        HOperatorSet.ClosingCircle(_ThresResultRegion, out _ThresResultRegion, OperRegionUserRegions[i].RegionParameters[0]);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(OperRegionUserRegions[i].DisplayRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.SelectShape:

                        HOperatorSet.Connection(_ThresResultRegion, out HObject _RegionConnection);
                        HOperatorSet.SelectShape(_RegionConnection, out HObject _ResultRegionSelect, "area", "and", OperRegionUserRegions[i].RegionParameters[0], OperRegionUserRegions[i].RegionParameters[1]);
                        HOperatorSet.Union1(_ResultRegionSelect, out _ThresResultRegion);
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.RegionTrans:

                        switch (OperRegionUserRegions[i].RegionParameters[0])
                        {
                            case 0:
                                HOperatorSet.InnerCircle(_ThresResultRegion, out HTuple RowIn, out HTuple ColIn, out HTuple RadiusIn);
                                HOperatorSet.GenCircle(out _ThresResultRegion, RowIn, ColIn, RadiusIn);
                                break;

                            case 1:
                                HOperatorSet.SmallestCircle(_ThresResultRegion, out HTuple RowOut, out HTuple ColOut, out HTuple RadiusOut);
                                HOperatorSet.GenCircle(out _ThresResultRegion, RowOut, ColOut, RadiusOut);
                                break;

                            case 2:
                                HOperatorSet.SmallestRectangle1(_ThresResultRegion, out HTuple Row01, out HTuple Col02, out HTuple Row11, out HTuple Col12);
                                HOperatorSet.GenRectangle1(out _ThresResultRegion, Row01, Col02, Row11, Col12);
                                break;

                            case 3:
                                HOperatorSet.SmallestRectangle2(_ThresResultRegion, out HTuple RowCenter, out HTuple ColCenter, out HTuple Phi, out HTuple Len1, out HTuple Len2);
                                HOperatorSet.GenRectangle2(out _ThresResultRegion, RowCenter, ColCenter, Phi, Len1, Len2);
                                break;

                            default:
                                break;
                        }
                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);
                        break;

                    case RegionOperatType.RegionArray:

                        HTuple hv_row = null, hv_col = null, hv_phi = null, hv_len1 = null;
                        HTuple hv_len2 = null, hv_RegNum = null, hv_Radius = null;
                        HTuple hv_startPtRow = new HTuple(), hv_startPtCol = new HTuple();
                        HTuple hv_endPtRow = new HTuple(), hv_endPtCol = new HTuple();
                        HTuple hv_gapRow = new HTuple(), hv_gapCol = new HTuple();
                        HTuple hv_RowTup = new HTuple(), hv_ColTup = new HTuple();
                        HTuple hv_RadiusTup = new HTuple();

                        HOperatorSet.SmallestRectangle2(_ThresResultRegion, out hv_row, out hv_col, out hv_phi, out hv_len1, out hv_len2);

                        hv_RegNum = OperRegionUserRegions[i].RegionParameters[0];
                        hv_Radius = 12;  // 默认值

                        if ((int)(new HTuple(hv_RegNum.TupleGreater(1))) != 0)
                        {
                            //起始点
                            hv_startPtRow = hv_row + ((hv_len1 - hv_Radius) * (hv_phi.TupleSin()));
                            hv_startPtCol = hv_col - ((hv_len1 - hv_Radius) * (hv_phi.TupleCos()));
                            //终止点
                            hv_endPtRow = hv_row - ((hv_len1 - hv_Radius) * (hv_phi.TupleSin()));
                            hv_endPtCol = hv_col + ((hv_len1 - hv_Radius) * (hv_phi.TupleCos()));
                            //间距
                            hv_gapRow = (hv_endPtRow - hv_startPtRow) / (hv_RegNum - 1);
                            hv_gapCol = (hv_endPtCol - hv_startPtCol) / (hv_RegNum - 1);
                            if ((int)(new HTuple(hv_gapRow.TupleNotEqual(0))) != 0)
                            {
                                hv_RowTup = HTuple.TupleGenSequence(hv_startPtRow, hv_endPtRow, hv_gapRow);
                            }
                            else
                            {
                                HOperatorSet.TupleGenConst(hv_RegNum, hv_startPtRow, out hv_RowTup);
                            }
                            if ((int)(new HTuple(hv_gapCol.TupleNotEqual(0))) != 0)
                            {
                                hv_ColTup = HTuple.TupleGenSequence(hv_startPtCol, hv_endPtCol, hv_gapCol);
                            }
                            else
                            {
                                HOperatorSet.TupleGenConst(hv_RegNum, hv_startPtCol, out hv_ColTup);
                            }
                            HOperatorSet.TupleGenConst(new HTuple(hv_RowTup.TupleLength()), hv_Radius, out hv_RadiusTup);
                            //生成阵列圆
                            HOperatorSet.GenCircle(out _ThresResultRegion, hv_RowTup, hv_ColTup, hv_RadiusTup);
                        }
                        else
                        {
                            //生成单个圆
                            HOperatorSet.GenCircle(out _ThresResultRegion, hv_row, hv_col, hv_Radius);
                        }

                        HOperatorSet.Union1(_ThresResultRegion, out _ThresResultRegion);

                        OperRegionUserRegions[i].DisplayRegion = _ThresResultRegion;
                        OperRegionUserRegions[i].CalculateRegion = _ThresResultRegion;
                        htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                        htWindow.DisplaySingleRegionSetDraw(_ThresResultRegion);
                        Thread.Sleep(stepTime);

                        break;

                    case RegionOperatType.Null:

                        break;

                    default:
                        MessageBox.Show("请重新选择操作区域工具！");
                        break;


                }
            }
            return _ThresResultRegion;
        }

        //加载FreeRegion
        private void ExecuteLoadFreeRegionCommond(object parameter)
        {
            if (isRightClick)
            {
                HOperatorSet.GenEmptyObj(out HObject freeRegion);
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = "reg|*.reg";
                    ofd.Multiselect = false;
                    ofd.InitialDirectory = ModelsRecipeDirectory;
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    HOperatorSet.ReadRegion(out freeRegion, ofd.FileName);
                }
                HOperatorSet.MoveRegion(freeRegion, out HObject _freeRegion, -EpoxyParameter.DieImageRowOffset, -EpoxyParameter.DieImageColumnOffset);
                UserRegion userRegion_Region = new UserRegion()
                {
                    DisplayRegion = _freeRegion,
                    CalculateRegion = _freeRegion,
                    RegionType = RegionType.Region,
                    //RegionParameters = regionParameters,
                };
                if (userRegion_Region == null) return;
                userRegion_Region.Index = OperRegionUserRegions.Count + 1;
                userRegion_Region.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                OperRegionUserRegions.Add(userRegion_Region);
                htWindow.DisplayMultiRegion(userRegion_Region.DisplayRegion);
            }
        }

        //阈值分割
        private void ExecuteThresCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    if (OperRegionUserRegions.Count == 0) return;
                    if (EpoxyModelObject.DieChannelImage == null || !EpoxyModelObject.DieChannelImage.IsInitialized()) return;
                    HOperatorSet.GenEmptyObj(out HObject imageReduced);
                    HOperatorSet.ReduceDomain(EpoxyModelObject.DieChannelImage, RunSteps(OperRegionUserRegions.Last().Index), out imageReduced);
                    HOperatorSet.Threshold(imageReduced, out HObject _ResultRegion, ThresValue[0], ThresValue[1]);
                    imageReduced.Dispose();
                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.Threshold,
                        RegionParameters = new double[2] { thresValue[0], thresValue[1] },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplaySingleRegionSetDraw(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        //腐蚀
        private void ExecuteErosionCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HOperatorSet.ErosionCircle(RunSteps(OperRegionUserRegions.Last().Index), out HObject _ResultRegion, ErosionRadius[0]);
                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.Erosion,
                        RegionParameters = new double[1] { erosionRadius[0] },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        //膨胀
        private void ExecuteDilationCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HOperatorSet.DilationCircle(RunSteps(OperRegionUserRegions.Last().Index), out HObject _ResultRegion, DilationRadius[0]);
                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.Dilation,
                        RegionParameters = new double[1] { dilationRadius[0] },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        //填孔
        private void ExecuteFillupCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HOperatorSet.FillUpShape(RunSteps(OperRegionUserRegions.Last().Index), out HObject _ResultRegion,"area",0,maxFillUp[0]);
                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.Fillup,
                        RegionParameters = new double[1] { maxFillUp[0] },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        //开操作
        private void ExecuteOpeningCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HOperatorSet.OpeningCircle(RunSteps(OperRegionUserRegions.Last().Index), out HObject _ResultRegion, openingCircleSize[0]);
                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.OpeningCircle,
                        RegionParameters = new double[1] { openingCircleSize[0] },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        //闭操作
        private void ExecuteClosingCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HOperatorSet.ClosingCircle(RunSteps(OperRegionUserRegions.Last().Index), out HObject _ResultRegion, closingCircleSize[0]);
                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.ClosingCircle,
                        RegionParameters = new double[1] { closingCircleSize[0] },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        // 根据面积选择区域
        private void ExecuteSelectShapeCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HOperatorSet.Connection(RunSteps(OperRegionUserRegions.Last().Index), out HObject _RegionConnection);
                    HOperatorSet.SelectShape(_RegionConnection, out HObject _ResultRegionSelect, "area", "and", areaValue[0], areaValue[1]);
                    HOperatorSet.Union1(_ResultRegionSelect, out HObject _ResultRegion);
                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.SelectShape,
                        RegionParameters = new double[2] { areaValue[0], areaValue[1] },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        // 区域合并
        private void ExecuteUnionCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HOperatorSet.Union2(RunSteps(regionIdx[0]), RunSteps(regionIdx[1]), out HObject _ResultRegion);
                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.Union,
                        RegionParameters = new double[2] { regionIdx[0], regionIdx[1] },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        // 区域求差
        private void ExecuteDifferenceCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HOperatorSet.Difference(RunSteps(regionIdx[0]), RunSteps(regionIdx[1]), out HObject _ResultRegion);
                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.Difference,
                        RegionParameters = new double[2] { regionIdx[0], regionIdx[1] },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        // 区域连通域
        private void ExecuteConnectionCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HOperatorSet.Connection(RunSteps(OperRegionUserRegions.Last().Index), out HObject _ResultRegion);
                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.Connection,
                        RegionParameters = new double[] { },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        // 区域转换
        private void ExecuteRegionTransCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HObject _ResultRegion = null;
                    switch (transType)
                    {
                        case 0:
                            HOperatorSet.InnerCircle(RunSteps(OperRegionUserRegions.Last().Index), out HTuple RowIn, out HTuple ColIn, out HTuple RadiusIn);
                            HOperatorSet.GenCircle(out _ResultRegion, RowIn, ColIn, RadiusIn);
                            break;

                        case 1:
                            HOperatorSet.SmallestCircle(RunSteps(OperRegionUserRegions.Last().Index), out HTuple RowOut, out HTuple ColOut, out HTuple RadiusOut);
                            HOperatorSet.GenCircle(out _ResultRegion, RowOut, ColOut, RadiusOut);
                            break;

                        case 2:
                            HOperatorSet.SmallestRectangle1(RunSteps(OperRegionUserRegions.Last().Index), out HTuple Row01, out HTuple Col02, out HTuple Row11, out HTuple Col12);
                            HOperatorSet.GenRectangle1(out _ResultRegion, Row01, Col02, Row11, Col12);
                            break;

                        case 3:
                            HOperatorSet.SmallestRectangle2(RunSteps(OperRegionUserRegions.Last().Index), out HTuple RowCenter, out HTuple ColCenter, out HTuple Phi,out HTuple Len1, out HTuple Len2);
                            HOperatorSet.GenRectangle2(out _ResultRegion, RowCenter, ColCenter, Phi, Len1, Len2);
                            break;

                        default:
                            break;
                    }

                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.RegionTrans,
                        RegionParameters = new double[1] { transType },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        //区域阵列
        private void ExecuteRegionArrayCommand(object parameter)
        {
            if (isRightClick)
            {
                if (OperRegionUserRegions.Count == 0) return;
                try
                {
                    HTuple hv_row = null, hv_col = null, hv_phi = null, hv_len1 = null;
                    HTuple hv_len2 = null, hv_RegNum = null, hv_Radius = null;
                    HTuple hv_startPtRow = new HTuple(), hv_startPtCol = new HTuple();
                    HTuple hv_endPtRow = new HTuple(), hv_endPtCol = new HTuple();
                    HTuple hv_gapRow = new HTuple(), hv_gapCol = new HTuple();
                    HTuple hv_RowTup = new HTuple(), hv_ColTup = new HTuple();
                    HTuple hv_RadiusTup = new HTuple();
                    // Initialize local and output iconic variables 
                    HObject _ResultRegion = null;
                    HOperatorSet.GenEmptyObj(out _ResultRegion);

                    HOperatorSet.SmallestRectangle2(RunSteps(OperRegionUserRegions.Last().Index), out hv_row, out hv_col, 
                                                             out hv_phi, out hv_len1, out hv_len2);

                    hv_RegNum = arrayRegNum;
                    hv_Radius = 12;  // 默认值

                    if ((int)(new HTuple(hv_RegNum.TupleGreater(1))) != 0)
                    {
                        //起始点
                        hv_startPtRow = hv_row + ((hv_len1 - hv_Radius) * (hv_phi.TupleSin()));
                        hv_startPtCol = hv_col - ((hv_len1 - hv_Radius) * (hv_phi.TupleCos()));
                        //终止点
                        hv_endPtRow = hv_row - ((hv_len1 - hv_Radius) * (hv_phi.TupleSin()));
                        hv_endPtCol = hv_col + ((hv_len1 - hv_Radius) * (hv_phi.TupleCos()));
                        //间距
                        hv_gapRow = (hv_endPtRow - hv_startPtRow) / (hv_RegNum - 1);
                        hv_gapCol = (hv_endPtCol - hv_startPtCol) / (hv_RegNum - 1);
                        if ((int)(new HTuple(hv_gapRow.TupleNotEqual(0))) != 0)
                        {
                            hv_RowTup = HTuple.TupleGenSequence(hv_startPtRow, hv_endPtRow, hv_gapRow);
                        }
                        else
                        {
                            HOperatorSet.TupleGenConst(hv_RegNum, hv_startPtRow, out hv_RowTup);
                        }
                        if ((int)(new HTuple(hv_gapCol.TupleNotEqual(0))) != 0)
                        {
                            hv_ColTup = HTuple.TupleGenSequence(hv_startPtCol, hv_endPtCol, hv_gapCol);
                        }
                        else
                        {
                            HOperatorSet.TupleGenConst(hv_RegNum, hv_startPtCol, out hv_ColTup);
                        }
                        HOperatorSet.TupleGenConst(new HTuple(hv_RowTup.TupleLength()), hv_Radius, out hv_RadiusTup);
                        //生成阵列圆
                        _ResultRegion.Dispose();
                        HOperatorSet.GenCircle(out _ResultRegion, hv_RowTup, hv_ColTup, hv_RadiusTup);
                    }
                    else
                    {
                        //生成单个圆
                        _ResultRegion.Dispose();
                        HOperatorSet.GenCircle(out _ResultRegion, hv_row, hv_col, hv_Radius);
                    }

                    HOperatorSet.Union1(_ResultRegion, out _ResultRegion);

                    UserRegion userRegion = new UserRegion
                    {
                        DisplayRegion = _ResultRegion,
                        //CalculateRegion = _ResultRegion,
                        RegionType = RegionType.Operation,
                        RegionOperatType = RegionOperatType.RegionArray,
                        RegionParameters = new double[] { arrayRegNum },
                    };
                    if (userRegion == null) return;
                    userRegion.Index = OperRegionUserRegions.Count + 1;
                    userRegion.LastIndex = OperRegionUserRegions.Count; //默认前节点为上一次
                    OperRegionUserRegions.Add(userRegion);
                    htWindow.InitialHWindow("cyan", isMargin ? "margin" : "fill");
                    htWindow.DisplayMultiRegionSetDisplay(_ResultRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        //区域保存
        private void ExecuteRegionSaveCommand(object parameter)
        {
            if (isRightClick)
            {
                UserRegion userRegion = parameter as UserRegion;
                if (userRegion == null) return;
                HOperatorSet.MoveRegion(userRegion.DisplayRegion, out HObject _thresResultRegion, EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset);
                HOperatorSet.WriteRegion(_thresResultRegion, ModelsRecipeDirectory + userRegion.RegionType + userRegion.Index + ".reg");
            }
        }

        private void ExecuteSaveCommand(object parameter)
        {
            if (isRightClick)
            {
                if (_ThresResultRegion == null) return;
                try
                {
                    if (EpoxyParameter.OnRecipesIndex == -1)
                    {
                        MessageBox.Show("请先选择属于哪个框架！");
                        return;
                    }
                    OnSaveXML?.Invoke();  
                    HObject resultRegion=RunNewSteps(OperRegionUserRegions.Last().Index);
                    HOperatorSet.MoveRegion(resultRegion, out HObject _resultRegion, EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset);
                    HTuple file = new HTuple();
                    //file = $"{ModelsFile}{ EpoxyParameter.OnRecipesIndexs[EpoxyParameter.OnRecipesIndex]}\\";
                    //HOperatorSet.WriteRegion(_resultRegion,file+FreeRegionName+".reg");
                    HOperatorSet.WriteRegion(_resultRegion, ModelsRecipeDirectory + FreeRegionName + ".reg");
                    MessageBox.Show("结果区域保存完成!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "参数保存失败");
                }
            }
        }

        private void ExecuteIsCheckCommand(object parameter)
        {
            if (OperRegionUserRegions.All(x => x.IsSelected==true))
            { IsCheckAll = true; }
            else if (OperRegionUserRegions.All(x =>!x.IsSelected))
            { IsCheckAll = false; }
            else 
            { IsCheckAll = null; }
        }

        private void ExecuteIsCheckAllCommand(object parameter)
        {
            if (IsCheckAll==true)
            { OperRegionUserRegions.ToList().ForEach(r => r.IsSelected = true); }
            else if(IsCheckAll == false)
            { OperRegionUserRegions.ToList().ForEach(r => r.IsSelected = false); }
        }

        public bool CheckCompleted()
        {
            /*
            if (string.IsNullOrEmpty(EpoxyParameter.ImagePath) || !File.Exists(EpoxyParameter.ImagePath))
            {
                System.Windows.MessageBox.Show("图片不存在，请重新选择");
                return false;
            }
            if (string.IsNullOrEmpty(EpoxyParameter.TrainningImagesDirectory) || !Directory.Exists(EpoxyParameter.TrainningImagesDirectory))
            {
                System.Windows.MessageBox.Show("训练图像文件夹不存在，请重新选择");
                return false;
            }
            if (OperRegionUserRegions.Count == 0)
            {
                System.Windows.MessageBox.Show("请选择一个Die区域");
                return false;
            }
            if (EpoxyModelObject.DieImage == null || !EpoxyModelObject.DieImage.IsInitialized())
            {
                System.Windows.MessageBox.Show("请裁剪一个Die区域");
                return false;
            }
            */
            return true;
        }

        public void Initial()
        {

        }

        public void Dispose()
        {
            (Content as Page_RegionModel).DataContext = null;
            (Content as Page_RegionModel).Close();
            Content = null;
            htWindow = null;
            EpoxyParameter = null;
            EpoxyModelObject = null;
            OperRegionUserRegions = null;
            LoadReferenceCommand = null;
            AddUserRegionCommand = null;
            RemoveUserRegionCommand = null;
            ModifyRegionCommand = null;
            UserRegionEnableChangedCommand = null;
            UserRegionStepChangedCommand = null;
            UnionCommand = null;
            DifferenceCommand = null;
            ConnectionCommand = null;
            ErosionCommand = null;
            DilationCommand = null;
            FillupCommand = null;
            OpeningCommand = null;
            ClosingCommand = null;
            ThresCommand = null;
            SelectShapeCommand = null;
            RegionTransCommand = null;
            SaveCommand = null;
            DisplayResultCommand = null;
            DisplayThresCommand = null;
            LoadFreeRegionCommond = null;
            RegionSaveCommand = null;
            IsCheckCommand = null;
            IsCheckAllCommand = null;
        }
    }
}
