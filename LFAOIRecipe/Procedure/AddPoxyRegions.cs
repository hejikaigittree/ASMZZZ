using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HalconDotNet;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace LFAOIRecipe
{
    class AddPoxyRegions : ViewModelBase, IProcedure
    {
        //1123
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public bool isRightClick = true;//

        public event Action OnSaveXML;

        public string ReferenceDirectory { get; set; }

        private string ModelsRecipeDirectory;

        private EpoxyModelObject EpoxyModelObject;

        public ObservableCollection<UserRegion> DieUserRegions { get; private set; } = new ObservableCollection<UserRegion>();

        public ObservableCollection<UserRegion> EpoxyUserRegions { get; private set; }
        private IEnumerable<HObject> EpoxyRegions => EpoxyUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public ObservableCollection<UserRegion> EpoxyReferenceUserRegions { get; private set; }
        private IEnumerable<HObject> EpoxyReferenceRegions => EpoxyReferenceUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        private bool isLoadCompleted = false;

        private HTHalControlWPF htWindow;

        private readonly string ModelsFile;

        private readonly string RecipeFile;

        // 1215 lw
        private double epoxyRegionHigh = 50;
        public double EpoxyRegionHigh
        {
            get => epoxyRegionHigh;
            set => OnPropertyChanged(ref epoxyRegionHigh, value);
        }

        private double epoxyLenExpand = 10;
        public double EpoxyLenExpand
        {
            get => epoxyLenExpand;
            set => OnPropertyChanged(ref epoxyLenExpand, value);
        }

        private double epoxyRegionOffset = 5;
        public double EpoxyRegionOffset
        {
            get => epoxyRegionOffset;
            set => OnPropertyChanged(ref epoxyRegionOffset, value);
        }

        private int imageIndex;
        public int ImageIndex
        {
            get => imageIndex;
            set
            {
                if (imageIndex != value)
                {
                    if (ImageVerify == null || !ImageVerify.IsInitialized())
                    {
                        if (value == -1)
                        {
                            HOperatorSet.AccessChannel(EpoxyModelObject.DieImage, out HObject ChannelImageDisplay, EpoxyModelVerifyParameter.ImageChannelIndex + 1);
                            htWindow.Display(ChannelImageDisplay, true);
                        }
                        else if (value >= 0)
                        {
                            HOperatorSet.AccessChannel(EpoxyModelObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                            EpoxyModelVerifyParameter.ImageChannelIndex = value;
                            htWindow.Display(ChannelImageDisplay, true);
                        }
                        imageIndex = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        // 1123
                        if (imageIndex != value)
                        {
                            if (value == -1)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify, EpoxyModelVerifyParameter.ImageChannelIndex + 1);
                                htWindow.Display(ChannelImageVerify, true);
                            }
                            else if (value >= 0)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify, value + 1);
                                EpoxyModelVerifyParameter.ImageChannelIndex = value;
                                htWindow.Display(ChannelImageVerify, true);
                            }
                            imageIndex = value;
                            OnPropertyChanged();
                        }
                    }
                }
            }
        }

        private HObject ImageVerify,ChannelImageVerify, DieRegions = null;

        private UserRegion selectedUserRegion;
        public UserRegion SelectedUserRegion
        {
            get => selectedUserRegion;
            set => OnPropertyChanged(ref selectedUserRegion, value);
        }

        private UserRegion userRegionForCutOut;
        public UserRegion UserRegionForCutOut
        {
            get => userRegionForCutOut;
            set => OnPropertyChanged(ref userRegionForCutOut, value);
        }

        public EpoxyParameter EpoxyParameter { get; set; }
        public EpoxyModelVerifyParameter EpoxyModelVerifyParameter { get; set; }
        //1221
        public EpoxyModelVerifyParameterSet EpoxyModelVerifyParameterSet { get; set; }

        public CommandBase LoadReferenceCommand { get; private set; }
        public CommandBase AddUserRegionCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase AddReferenceUserRegionCommand { get; private set; }
        public CommandBase RemoveReferenceUserRegionCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }
        public CommandBase ModifyReferenceUserRegionCommand { get; private set; }
        public CommandBase ReferenceUserRegionEnableChangedCommand { get; private set; }

        public CommandBase PreviousCommand { get; private set; }
        public CommandBase NextCommand { get; private set; }
        public CommandBase VerifyCommand { get; private set; }
        public CommandBase SaveCommand { get; private set; }
        public CommandBase ImagesSetVerifyCommand { get; private set; }
        public CommandBase RefreshImagesSet { get; private set; }
        public CommandBase AutoGenEpoxyRegionCommand { get; private set; }
        // add by wj 2020-12-25
        public CommandBase DisplayAutoEpoxyRegionCommand { get; private set; }
        public CommandBase LoadReferenceUserRegionCommond { get; private set; }

        private int isFovTaskFlag = 0;

        private int imgIndex = 0;
        private int pImageIndex = -1;
        private string pImageIndexPath;
        public string PImageIndexPath
        {
            get => pImageIndexPath;
            set => OnPropertyChanged(ref pImageIndexPath, value);
        }

        public AddPoxyRegions(HTHalControlWPF htWindow,
                              string modelsFile,
                              string recipeFile,
                              string referenceDirectory,
                              EpoxyModelObject epoxyModelObject,
                              EpoxyParameter epoxyParameter,
                              EpoxyModelVerifyParameter epoxyModelVerifyParameter,
                              EpoxyModelVerifyParameterSet epoxyModelVerifyParameterSet,
                              ObservableCollection<UserRegion> epoxyUserRegions,
                              string modelsRecipeDirectory,
                              ObservableCollection<UserRegion> epoxyReferenceUserRegions)
        {
            DisplayName = "银胶检测";
            Content = new Page_AddEpoxyRegion { DataContext = this };

            this.htWindow = htWindow;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.ReferenceDirectory = referenceDirectory;
            this.EpoxyModelObject = epoxyModelObject;
            this.EpoxyParameter = epoxyParameter;
            this.EpoxyModelVerifyParameter = epoxyModelVerifyParameter;
            this.EpoxyModelVerifyParameterSet = epoxyModelVerifyParameterSet;
            this.EpoxyUserRegions = epoxyUserRegions;
            this.ModelsRecipeDirectory = modelsRecipeDirectory;
            this.EpoxyReferenceUserRegions = epoxyReferenceUserRegions;
            AddUserRegionCommand = new CommandBase(ExecuteAddUserRegionCommand); 
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            RemoveUserRegionCommand = new CommandBase(ExecuteRemoveUserRegionCommand);
            AddReferenceUserRegionCommand = new CommandBase(ExecuteAddReferenceUserRegionCommand);
            RemoveReferenceUserRegionCommand = new CommandBase(ExecuteRemoveReferenceUserRegionCommand);
            ModifyReferenceUserRegionCommand = new CommandBase(ExecuteModifyReferenceUserRegionCommand);//
            //CutOutCommand = new CommandBase(ExecuteCutOutCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);//
            LoadReferenceCommand = new CommandBase(ExecuteLoadReferenceCommand);//
            ReferenceUserRegionEnableChangedCommand= new CommandBase(ExecuteReferenceUserRegionEnableChangedCommand);//

            PreviousCommand = new CommandBase(ExecutePreviousCommand);
            NextCommand = new CommandBase(ExecuteNextCommand);
            VerifyCommand = new CommandBase(ExecuteVerifyCommand);
            SaveCommand = new CommandBase(ExecuteSaveCommand);
            ImagesSetVerifyCommand = new CommandBase(ExecuteImagesSetVerifyCommand);
            RefreshImagesSet = new CommandBase(ExecuteRefreshImagesSet);
            //add by lw
            AutoGenEpoxyRegionCommand = new CommandBase(ExecuteAutoGenEpoxyRegionCommand);
            //add by wj 2020-12-25
            DisplayAutoEpoxyRegionCommand = new CommandBase(ExecuteDisplayAutoEpoxyRegionCommand);
            LoadReferenceUserRegionCommond = new CommandBase(ExecuteLoadReferenceUserRegionCommond);
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
                files = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                            "IC*.*", SearchOption.TopDirectoryOnly);

                string[] OnRecipesIndexs = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    OnRecipesIndexs[i] = Path.GetFileName(files[i]);
                }
                EpoxyParameter.OnRecipesIndexs = OnRecipesIndexs;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "全局数据文件不存在！");
                return;
            }
        }

        public void LoadReferenceData()
        {
            if (!( File.Exists($"{ReferenceDirectory}ReferenceImage.tiff")
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

            //待优化
            HOperatorSet.ReadTuple(ReferenceDirectory + "TrainningImagesDirectory.tup", out HTuple TrainningImagesDirectoryTemp);
            EpoxyModelVerifyParameter.VerifyImagesDirectory = TrainningImagesDirectoryTemp;

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

            HOperatorSet.ReadTuple(ReferenceDirectory + "UserRegionForCutOutIndex.tup", out HTuple UserRegionForCutOutIndexTemp);
            EpoxyParameter.UserRegionForCutOutIndex = UserRegionForCutOutIndexTemp;
            UserRegionForCutOut = DieUserRegions.Where(u => u.Index == EpoxyParameter.UserRegionForCutOutIndex).FirstOrDefault();
            EpoxyModelObject.UserRegionForCutOut = UserRegionForCutOut;

            //1121 lht
            HOperatorSet.ReadTuple(ReferenceDirectory + "ImageChannelnumber.tup", out HTuple ImageChannelnumber);
            ChannelNames.Clear();
            for (int i = 0; i < ImageChannelnumber; i++)
            {
                ChannelName tmp_name = new ChannelName();
                tmp_name.Name = (i + 1).ToString();
                ChannelNames.Add(tmp_name);
            }
            EpoxyModelVerifyParameter.ChannelNames = ChannelNames;
            imageIndex = EpoxyModelVerifyParameter.ImageChannelIndex;
            OnPropertyChanged("imageIndex");

            //1201 lw
            HOperatorSet.TupleSplit(ReferenceDirectory, "\\", out HTuple hv_subStr);
            HTuple FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 3];
            EpoxyModelVerifyParameter.CurFovName = FOV_Name;

            //HOperatorSet.AccessChannel(EpoxyModelObject.DieImage, out HObject ChannelImageDisplay, EpoxyModelVerifyParameter.ImageChannelIndex + 1);
            HObject ChannelImageDisplay = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex);
            htWindow.Display(ChannelImageDisplay);

            //备用
            /*
            DieUserRegions.Clear();
            HOperatorSet.ReadRegion(out HObject _dieUserRegions, ReferenceDirectory + "CoarseReference.reg");
            HOperatorSet.CountObj(_dieUserRegions, out HTuple DieNumbers);
            for (int i = 0; i < DieNumbers; i++)
            {
                HOperatorSet.SmallestRectangle1(_dieUserRegions.SelectObj(i + 1), out HTuple row1_Rectangle, out HTuple column1_Rectangle, out HTuple row2_Rectangle, out HTuple column2_Rectangle);
                UserRegion _userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Rectangle1, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle);
                if (_userRegion == null) return;
                _userRegion.RegionType = RegionType.Rectangle1;
                DieUserRegions.Add(_userRegion);
                DieUserRegions[i].Index = i + 1;
            }
            */
        }

        public void LoadImage()
        {
            HOperatorSet.GenEmptyObj(out HObject image);
            HOperatorSet.ReadImage(out image, EpoxyParameter.ImagePath);
            htWindow.Display(image, true);
            EpoxyModelObject.Image = image;

            HOperatorSet.CountChannels(EpoxyModelObject.Image, out HTuple channels);
            EpoxyParameter.ImageCountChannels = channels;
            EpoxyModelObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, EpoxyModelVerifyParameter.ImageChannelIndex);
            //EpoxyModelObject.ImageR = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, 1);
            //EpoxyModelObject.ImageG = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, 2);
            //EpoxyModelObject.ImageB = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, 3);
        }

        public void LoadDieImage()
        {
            try
            {
                EpoxyModelObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex);
                //EpoxyModelObject.DieImageR = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, 1);
                //EpoxyModelObject.DieImageG = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, 2);
                //EpoxyModelObject.DieImageB = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

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
                    //if (htWindow.RegionType != RegionType.Rectangle1)
                    //{
                    //    MessageBox.Show("请使用矩形框选区域");
                    //    return;
                    //}
                    UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                         EpoxyParameter.DieImageRowOffset,
                                                         EpoxyParameter.DieImageColumnOffset);
                    if (userRegion == null) return;
                    userRegion.Index = EpoxyUserRegions.Count + 1;
                    //1221
                    userRegion.EpoxyModelVerifyParameterSet = new EpoxyModelVerifyParameterSet();
                    EpoxyUserRegions.Add(userRegion);
                    //htWindow.DisplayMultiRegion(EpoxyRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyParameter.ImageChannelIndex));
                    htWindow.DisplayMultiRegion(EpoxyRegions);
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

        private void ExecuteRemoveUserRegionCommand(object parameter)
        {
            if (isRightClick)
            {
                if (System.Windows.MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    for (int i = 0; i < EpoxyUserRegions.Count; i++)
                    {
                        if (EpoxyUserRegions[i].IsSelected)
                        {
                            EpoxyUserRegions.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            EpoxyUserRegions[i].Index = i + 1;
                        }
                    }
                    htWindow.DisplayMultiRegion(EpoxyRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));
                }
            }
        }

        private void ExecuteModifyRegionCommand(object parameter)//
        {
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    for (int i = 0; i < EpoxyUserRegions.Count; i++)
                    {
                        if (EpoxyUserRegions[i].IsSelected)
                        {
                            switch (EpoxyUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(EpoxyUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(EpoxyUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(EpoxyUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(EpoxyUserRegions[i].RegionParameters[3]),
                                                                  EpoxyUserRegions[i].RegionType, 
                                                                  EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, 
                                                                  (EpoxyUserRegions[i].RegionParameters[0] - EpoxyParameter.DieImageRowOffset),
                                                                  (EpoxyUserRegions[i].RegionParameters[1] - EpoxyParameter.DieImageColumnOffset),
                                                                  (EpoxyUserRegions[i].RegionParameters[2] - EpoxyParameter.DieImageRowOffset),
                                                                  (EpoxyUserRegions[i].RegionParameters[3] - EpoxyParameter.DieImageColumnOffset),
                                                                  out HTuple row1_Rectangle,
                                                                  out HTuple column1_Rectangle,
                                                                  out HTuple row2_Rectangle,
                                                                  out HTuple column2_Rectangle);

                                    EpoxyUserRegions[i].RegionType = RegionType.Rectangle1;
                                    //Die图
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, EpoxyUserRegions[i].RegionType, 
                                                            row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle,
                                                            EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    //1221
                                    userRegion.EpoxyModelVerifyParameterSet = new EpoxyModelVerifyParameterSet();
                                    EpoxyUserRegions[i] = userRegion;
                                    //htWindow.DisplayMultiRegion(EpoxyRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));//ok
                                    htWindow.DisplayMultiRegion(EpoxyRegions);
                                    EpoxyUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:

                                    htWindow.InitialHWindowUpdate(Math.Floor(EpoxyUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(EpoxyUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(EpoxyUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(EpoxyUserRegions[i].RegionParameters[3]),
                                                                  EpoxyUserRegions[i].RegionType,
                                                                  EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(EpoxyUserRegions[i].RegionParameters[0] - EpoxyParameter.DieImageRowOffset),
                                                                   Math.Floor(EpoxyUserRegions[i].RegionParameters[1] - EpoxyParameter.DieImageColumnOffset),
                                                                   EpoxyUserRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(EpoxyUserRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(EpoxyUserRegions[i].RegionParameters[4]),
                                                                   out HTuple row_Rectangle2,
                                                                   out HTuple column_Rectangle2,
                                                                   out HTuple phi_Rectangle2,
                                                                   out HTuple lenth1_Rectangle2,
                                                                   out HTuple lenth2_Rectangle2);

                                    EpoxyUserRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, EpoxyUserRegions[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         EpoxyParameter.DieImageRowOffset,
                                                                                                         EpoxyParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    //1221
                                    userRegion_Rectangle2.EpoxyModelVerifyParameterSet = new EpoxyModelVerifyParameterSet();
                                    EpoxyUserRegions[i] = userRegion_Rectangle2;
                                    //htWindow.DisplayMultiRegion(EpoxyRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));//ok
                                    htWindow.DisplayMultiRegion(EpoxyRegions);
                                    EpoxyUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((EpoxyUserRegions[i].RegionParameters[0]),
                                                                  (EpoxyUserRegions[i].RegionParameters[1]),
                                                                  (EpoxyUserRegions[i].RegionParameters[2]),
                                                                   0,
                                                                   EpoxyUserRegions[i].RegionType,
                                                                   EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                              (EpoxyUserRegions[i].RegionParameters[0] - EpoxyParameter.DieImageRowOffset),
                                                              (EpoxyUserRegions[i].RegionParameters[1] - EpoxyParameter.DieImageColumnOffset),
                                                               EpoxyUserRegions[i].RegionParameters[2],
                                                               out HTuple row_Circle,
                                                               out HTuple column_Circle,
                                                               out HTuple radius_Circle);

                                    EpoxyUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     EpoxyUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     EpoxyParameter.DieImageRowOffset,
                                                                                                     EpoxyParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    //1221
                                    userRegion_Circle.EpoxyModelVerifyParameterSet = new EpoxyModelVerifyParameterSet();
                                    EpoxyUserRegions[i] = userRegion_Circle;
                                    //htWindow.DisplayMultiRegion(EpoxyRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));//ok
                                    htWindow.DisplayMultiRegion(EpoxyRegions);
                                    EpoxyUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Ellipse:
                                    break;

                                default: break;
                            }
                         }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                    return;
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
                htWindow.DisplayMultiRegion(EpoxyRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));
            }
        }

        private void ExecuteAutoGenEpoxyRegionCommand(object parameter)
        {
            if (EpoxyReferenceUserRegions.Count == 0)
            {
                MessageBox.Show("请加载银胶参考区域！");
                return;
            }

            Algorithm.Model_RegionAlg.HTV_Gen_Epoxy_InspectReg(Algorithm.Region.ConcatRegion(EpoxyReferenceUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                                               out HObject EpoxyInspectRegs,
                                                               EpoxyRegionHigh, EpoxyLenExpand, EpoxyRegionOffset, 
                                                               out HTuple ErrRegCode,
                                                               out HTuple ErrRegStr);

            EpoxyUserRegions.Clear();
            for (int i = 0; i < EpoxyInspectRegs.CountObj(); i++)
            {
                HOperatorSet.SelectObj(EpoxyInspectRegs, out HObject _EpoxyInspectReg, i + 1);
                HOperatorSet.SmallestRectangle2(_EpoxyInspectReg, 
                                            out HTuple row_Rectangle2,
                                            out HTuple column_Rectangle2,
                                            out HTuple phi_Rectangle2,
                                            out HTuple lenth1_Rectangle2,
                                            out HTuple lenth2_Rectangle2);

                HOperatorSet.MoveRegion(_EpoxyInspectReg, out HObject _DieEpoxyInspectReg, -EpoxyParameter.DieImageRowOffset, -EpoxyParameter.DieImageColumnOffset);

                UserRegion userRegion = new UserRegion
                {
                    DisplayRegion = _DieEpoxyInspectReg,
                    CalculateRegion = _EpoxyInspectReg,
                    RegionType = RegionType.Rectangle2,
                    RegionParameters = new double[5] { row_Rectangle2, column_Rectangle2, phi_Rectangle2, lenth1_Rectangle2, lenth2_Rectangle2 },
                };
                if (userRegion == null) return;
                userRegion.Index = EpoxyUserRegions.Count + 1;
                //1221
                userRegion.EpoxyModelVerifyParameterSet = new EpoxyModelVerifyParameterSet();
                EpoxyUserRegions.Add(userRegion);
            }

            //htWindow.DisplayMultiRegion(EpoxyRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));
            //htWindow.DisplayMultiRegion(EpoxyRegions);
            DispalyEpoxyRegions(true);
        }

        //add by wj 显示自动生成银胶区域序号12-25
        //显示所有区域和序号
        private void ExecuteDisplayAutoEpoxyRegionCommand(object parameter)
        {
            DispalyEpoxyRegions();
        }
        public void DispalyEpoxyRegions(bool isHTWindowRegion = true)
        {
            //1028
            if (htWindow.hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || htWindow.isdeleted == true) return;

            if (EpoxyUserRegions.Count == 0)
            {
                htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex), true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            foreach (var item in EpoxyUserRegions)
            {
                HOperatorSet.ConcatObj(concatGroupRegion, item.DisplayRegion, out concatGroupRegion);
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                htWindow.DisplaySingleRegion(concatGroupRegion, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));
            }
            HTuple Length1 = 10;
            foreach (var item in EpoxyUserRegions)
            {
                HOperatorSet.TupleSin(item.RegionParameters[2], out HTuple sin_out_line);
                HTuple line_r = item.RegionParameters[0] - EpoxyParameter.DieImageRowOffset - Length1 * sin_out_line;
                HOperatorSet.TupleCos(item.RegionParameters[2], out HTuple cos_out_line);
                HTuple line_c = item.RegionParameters[1] - EpoxyParameter.DieImageColumnOffset + Length1 * cos_out_line;

                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, line_r - 20, line_c + 5);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }
        //1215 加载IC检测区
        private void ExecuteLoadReferenceUserRegionCommond(object parameter)
        {
            if (CutOutDie.isRightClick)
            {
                HOperatorSet.GenEmptyObj(out HObject IcRefRegion);
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = "reg|*.reg";
                    ofd.Multiselect = false;
                    ofd.InitialDirectory = ModelsFile;//初始路径
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    HOperatorSet.ReadRegion(out IcRefRegion, ofd.FileName);
                }
                HOperatorSet.SmallestRectangle2(IcRefRegion,
                                            out HTuple row_Rectangle2,
                                            out HTuple column_Rectangle2,
                                            out HTuple phi_Rectangle2,
                                            out HTuple lenth1_Rectangle2,
                                            out HTuple lenth2_Rectangle2);
                HOperatorSet.MoveRegion(IcRefRegion, out HObject _IcRefRegion, -EpoxyParameter.DieImageRowOffset, -EpoxyParameter.DieImageColumnOffset);

                EpoxyReferenceUserRegions.Clear();
                UserRegion userRegion_Region = new UserRegion()
                {
                    DisplayRegion = _IcRefRegion,
                    CalculateRegion = IcRefRegion,
                    RegionType = RegionType.Rectangle2,
                    RegionParameters = new double[5] { row_Rectangle2, column_Rectangle2, phi_Rectangle2, lenth1_Rectangle2, lenth2_Rectangle2 },
                };
                if (userRegion_Region == null) return;
                userRegion_Region.Index = EpoxyReferenceUserRegions.Count + 1;
                EpoxyReferenceUserRegions.Add(userRegion_Region);
                htWindow.DisplayMultiRegion(EpoxyReferenceRegions);
            }
        }

        //添加银胶参考区域
        private void ExecuteAddReferenceUserRegionCommand(object parameter)
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
                    //if (htWindow.RegionType != RegionType.Rectangle1)
                    //{
                    //    MessageBox.Show("请使用矩形框选区域");
                    //    return;
                    //}
                    UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                         EpoxyParameter.DieImageRowOffset,
                                                         EpoxyParameter.DieImageColumnOffset);
                    if (userRegion == null) return;
                    userRegion.Index = EpoxyReferenceUserRegions.Count + 1;
                    EpoxyReferenceUserRegions.Add(userRegion);
                    //htWindow.DisplayMultiRegion(EpoxyReferenceRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyParameter.ImageChannelIndex));
                    htWindow.DisplayMultiRegion(EpoxyReferenceRegions);
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

        private void ExecuteRemoveReferenceUserRegionCommand(object parameter)
        {
            if (isRightClick)
            {
                if (System.Windows.MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    for (int i = 0; i < EpoxyReferenceUserRegions.Count; i++)
                    {
                        if (EpoxyReferenceUserRegions[i].IsSelected)
                        {
                            EpoxyReferenceUserRegions.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            EpoxyReferenceUserRegions[i].Index = i + 1;
                        }
                    }
                    htWindow.DisplayMultiRegion(EpoxyReferenceRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));
                }
            }
        }

        private void ExecuteModifyReferenceUserRegionCommand(object parameter)// 1213 lw
        {
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    for (int i = 0; i < EpoxyReferenceUserRegions.Count; i++)
                    {
                        if (EpoxyReferenceUserRegions[i].IsSelected)
                        {
                            switch (EpoxyReferenceUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(EpoxyReferenceUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(EpoxyReferenceUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(EpoxyReferenceUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(EpoxyReferenceUserRegions[i].RegionParameters[3]),
                                                                  EpoxyReferenceUserRegions[i].RegionType,
                                                                  EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow,
                                                                  (EpoxyReferenceUserRegions[i].RegionParameters[0] - EpoxyParameter.DieImageRowOffset),
                                                                  (EpoxyReferenceUserRegions[i].RegionParameters[1] - EpoxyParameter.DieImageColumnOffset),
                                                                  (EpoxyReferenceUserRegions[i].RegionParameters[2] - EpoxyParameter.DieImageRowOffset),
                                                                  (EpoxyReferenceUserRegions[i].RegionParameters[3] - EpoxyParameter.DieImageColumnOffset),
                                                                  out HTuple row1_Rectangle,
                                                                  out HTuple column1_Rectangle,
                                                                  out HTuple row2_Rectangle,
                                                                  out HTuple column2_Rectangle);

                                    EpoxyReferenceUserRegions[i].RegionType = RegionType.Rectangle1;
                                    //Die图
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, EpoxyReferenceUserRegions[i].RegionType,
                                                            row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle,
                                                            EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    EpoxyReferenceUserRegions[i] = userRegion;
                                    //htWindow.DisplayMultiRegion(EpoxyReferenceRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));//ok
                                    htWindow.DisplayMultiRegion(EpoxyReferenceRegions);
                                    EpoxyReferenceUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:

                                    htWindow.InitialHWindowUpdate(Math.Floor(EpoxyReferenceUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(EpoxyReferenceUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(EpoxyReferenceUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(EpoxyReferenceUserRegions[i].RegionParameters[3]),
                                                                  EpoxyReferenceUserRegions[i].RegionType,
                                                                  EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(EpoxyReferenceUserRegions[i].RegionParameters[0] - EpoxyParameter.DieImageRowOffset),
                                                                   Math.Floor(EpoxyReferenceUserRegions[i].RegionParameters[1] - EpoxyParameter.DieImageColumnOffset),
                                                                   EpoxyReferenceUserRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(EpoxyReferenceUserRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(EpoxyReferenceUserRegions[i].RegionParameters[4]),
                                                                   out HTuple row_Rectangle2,
                                                                   out HTuple column_Rectangle2,
                                                                   out HTuple phi_Rectangle2,
                                                                   out HTuple lenth1_Rectangle2,
                                                                   out HTuple lenth2_Rectangle2);

                                    EpoxyReferenceUserRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, EpoxyReferenceUserRegions[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         EpoxyParameter.DieImageRowOffset,
                                                                                                         EpoxyParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    EpoxyReferenceUserRegions[i] = userRegion_Rectangle2;
                                    //htWindow.DisplayMultiRegion(EpoxyReferenceRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));//ok
                                    htWindow.DisplayMultiRegion(EpoxyReferenceRegions);
                                    EpoxyReferenceUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((EpoxyReferenceUserRegions[i].RegionParameters[0]),
                                                                  (EpoxyReferenceUserRegions[i].RegionParameters[1]),
                                                                  (EpoxyReferenceUserRegions[i].RegionParameters[2]),
                                                                   0,
                                                                   EpoxyReferenceUserRegions[i].RegionType,
                                                                   EpoxyParameter.DieImageRowOffset, EpoxyParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                              (EpoxyReferenceUserRegions[i].RegionParameters[0] - EpoxyParameter.DieImageRowOffset),
                                                              (EpoxyReferenceUserRegions[i].RegionParameters[1] - EpoxyParameter.DieImageColumnOffset),
                                                               EpoxyReferenceUserRegions[i].RegionParameters[2],
                                                               out HTuple row_Circle,
                                                               out HTuple column_Circle,
                                                               out HTuple radius_Circle);

                                    EpoxyReferenceUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     EpoxyReferenceUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     EpoxyParameter.DieImageRowOffset,
                                                                                                     EpoxyParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    EpoxyReferenceUserRegions[i] = userRegion_Circle;
                                    //htWindow.DisplayMultiRegion(EpoxyReferenceRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));//ok
                                    htWindow.DisplayMultiRegion(EpoxyReferenceRegions);
                                    EpoxyReferenceUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Ellipse:
                                    break;

                                default: break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void ExecuteReferenceUserRegionEnableChangedCommand(object parameter)
        {
            if (isRightClick)
            {
                UserRegion userRegion = parameter as UserRegion;
                if (userRegion == null) return;
                htWindow.DisplayMultiRegion(EpoxyReferenceRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));
            }
        }

        //加载图集
        private void ExecuteImagesSetVerifyCommand(object parameter)
        {
            if (isRightClick != true) return;
            try
            {
                using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        EpoxyModelVerifyParameter.VerifyImagesDirectory = fbd.SelectedPath;
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
                                                                                EpoxyModelVerifyParameter.VerifyImagesDirectory,
                                                                                EpoxyModelVerifyParameter.CurFovName,
                                                                                0, out HTuple hv_o_ImageVerifyNum, out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        EpoxyModelVerifyParameter.CurrentVerifySet = hv_o_ImageVerifyNum;
                        PImageIndexPath = imageFiles[EpoxyModelVerifyParameter.ImageChannelIndex];
                        ImageVerify = ho_MutiImage;
                    }
                    else
                    {
                        isFovTaskFlag = 0;

                        Algorithm.File.list_image_files(EpoxyModelVerifyParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        string[] folderList = imageFiles;
                        EpoxyModelVerifyParameter.CurrentVerifySet = folderList.Count();
                        PImageIndexPath = imageFiles[0];
                        HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                        ImageVerify = image;
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, EpoxyModelVerifyParameter.ImageChannelIndex);
                    htWindow.Display(ChannelImageVerify, true);
                    pImageIndex = 0;
                    imgIndex = 0;
                }
                HOperatorSet.ReadRegion(out DieRegions, $"{ReferenceDirectory}CoarseReference.reg");
                isLoadCompleted = true;
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // 加载默认图集
        private void ExecuteRefreshImagesSet(object parameter)
        {
            if (Directory.Exists(EpoxyModelVerifyParameter.VerifyImagesDirectory))
            {
                Algorithm.File.list_image_files(EpoxyModelVerifyParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                string[] folderList = imageFiles;
                EpoxyModelVerifyParameter.CurrentVerifySet = folderList.Count();
                PImageIndexPath = imageFiles[0];
                HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                ImageVerify = image;
                ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, EpoxyModelVerifyParameter.ImageChannelIndex);
                htWindow.Display(ChannelImageVerify, true);
                pImageIndex = 0;
                imgIndex = 0;

                HOperatorSet.ReadRegion(out DieRegions, $"{ReferenceDirectory}CoarseReference.reg");
                isLoadCompleted = true;
            }
        }

        //检测验证
        private void ExecuteVerifyCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (!isLoadCompleted)
            {
                MessageBox.Show("请先加载图集！");
                return;
            }
            if (EpoxyUserRegions.Count == 0)
            {
                MessageBox.Show("请先画银胶区域！");
                return;
            }
            if (EpoxyReferenceUserRegions.Count == 0)
            {
                MessageBox.Show("请先画银胶参考区域！");
                return;
            }
            if (EpoxyParameter.ImageCountChannels > 0 && EpoxyModelVerifyParameter.ImageChannelIndex < 0)
            {
                MessageBox.Show("请先选择通道图像！");
                return;
            }
            if (DieRegions == null || !DieRegions.IsInitialized())
            {
                MessageBox.Show("请先加载搜索区！");
                return;
            }
            //Window_Loading window_Loading = new Window_Loading("正在检验");

            try
            {
                htWindow.Display(ChannelImageVerify, true);

                Algorithm.Model_RegionAlg.HTV_confirm_mapping_matrix(Algorithm.Region.GetChannnelImageConcact(ImageVerify),//图像通道图反推 黑白图传1张；彩色图RGB三通道图concact在一起
                                                     DieRegions.SelectObj(imgIndex + 1),
                                                     ModelsFile,
                                                     RecipeFile,
                                                     EpoxyParameter.OnRecipesIndexs[EpoxyParameter.OnRecipesIndex],
                                                     out HTuple HomMat2D,
                                                     out HTuple _frameLocPara, out HTuple ErrCode, out HTuple ErrStr);

                if (ErrCode != 0 )
                {
                    MessageBox.Show("芯片没有定位到！");
                    imgIndex++;
                    if (imgIndex + 1 > DieRegions.CountObj())
                    {
                        imgIndex = 0;
                    }
                    return;
                }

                HTupleVector hvec_i_DefectValue = new HTupleVector(3);
                hvec_i_DefectValue = (new HTupleVector(3).Insert(0, (new HTupleVector(2).Insert(0, (
                                      new HTupleVector(1).Insert(0, new HTupleVector(new HTuple())))))));

                //1221多套参数
                HTupleVector ThreshGray = new HTupleVector(1);
                HTuple LightOrDark = new HTuple();
                HTuple OpeningSize = new HTuple();
                HTuple EpoxyLenTh = new HTuple();
                HTuple EpoxyHeiTh = new HTuple();
                HTuple hv_idx = 0;
                foreach (var epoxyRegon in EpoxyUserRegions)
                {
                    ThreshGray[hv_idx] = new HTupleVector(epoxyRegon.EpoxyModelVerifyParameterSet.ThreshGray).Clone();
                    LightOrDark = LightOrDark.TupleConcat(epoxyRegon.EpoxyModelVerifyParameterSet.LightOrDark);
                    OpeningSize = OpeningSize.TupleConcat(epoxyRegon.EpoxyModelVerifyParameterSet.OpeningSize);
                    EpoxyLenTh = EpoxyLenTh.TupleConcat(epoxyRegon.EpoxyModelVerifyParameterSet.EpoxyLenTh);
                    EpoxyHeiTh = EpoxyHeiTh.TupleConcat(epoxyRegon.EpoxyModelVerifyParameterSet.EpoxyHeiTh);
                    hv_idx = hv_idx + 1;
                }


                Algorithm.Model_RegionAlg.HTV_Epoxy_Inspect_Threshold(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, EpoxyModelVerifyParameter.ImageChannelIndex),
                                        Algorithm.Region.ConcatRegion(EpoxyUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                        Algorithm.Region.ConcatRegion(EpoxyReferenceUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                        out HObject EpoxyRegs,//CutRegion
                                        out HObjectVector FailReg,
                                        HomMat2D,
                                        ThreshGray,  //多套参数
                                        LightOrDark, //多套参数
                                        OpeningSize, //多套参数
                                        EpoxyLenTh,  //多套参数
                                        EpoxyHeiTh,  //多套参数
                                        hvec_i_DefectValue,
                                        EpoxyModelVerifyParameter.ImageChannelIndex + 1,
                                        out HTupleVector EpoxyDefectType,
                                        out HTupleVector hv__DefectImgIdx, 
                                        out HTupleVector hvec__DefectValue,
                                        out HTuple EpoxyErrCode, out HTuple EpoxyErrStr);

                if (EpoxyErrCode==0)
                {
                    htWindow.DisplaySingleRegion(EpoxyRegs.ConcatObj( DieRegions.SelectObj(imgIndex + 1)), ChannelImageVerify);
                }

                if (EpoxyErrCode > 0)
                {
                    Algorithm.Model_RegionAlg.HTV_Vector_to_ObjectConcat_1d(FailReg, out HObject FailRegsConcat, out HTuple VerErrCode, out HTuple VerErrStr);
                    htWindow.DisplaySingleRegion(FailRegsConcat.ConcatObj(EpoxyRegs).ConcatObj(DieRegions.SelectObj(imgIndex + 1)), ChannelImageVerify, "orange");
                }

                imgIndex++;
                if ((imgIndex + 1) > DieRegions.CountObj())
                {
                    imgIndex = 0;
                }

                EpoxyRegs?.Dispose();
                FailReg?.Dispose();
                //window_Loading.Close();
            }

            catch (Exception ex)
            {
                //window_Loading.Close();
                MessageBox.Show(ex.ToString(), "检测验证出错");
                imgIndex++;
                if ((imgIndex + 1) > DieRegions.CountObj())
                {
                    imgIndex = 0;
                }
            }
        }

        //前一页
        private void ExecutePreviousCommand(object parameter)
        {
            if (isRightClick != true) return;
            try
            {
                imgIndex = 0;
                if (EpoxyModelVerifyParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == 0 ? EpoxyModelVerifyParameter.CurrentVerifySet - 1 : --pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                EpoxyModelVerifyParameter.VerifyImagesDirectory,
                                                                                EpoxyModelVerifyParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[EpoxyModelVerifyParameter.ImageChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(EpoxyModelVerifyParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, EpoxyModelVerifyParameter.ImageChannelIndex);
                    htWindow.Display(ChannelImageVerify, true);
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
            if (isRightClick != true) return;
            try
            {
                imgIndex = 0;
                if (EpoxyModelVerifyParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == EpoxyModelVerifyParameter.CurrentVerifySet - 1 ? 0 : ++pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                EpoxyModelVerifyParameter.VerifyImagesDirectory,
                                                                                EpoxyModelVerifyParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[EpoxyModelVerifyParameter.ImageChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(EpoxyModelVerifyParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, EpoxyModelVerifyParameter.ImageChannelIndex);
                    htWindow.Display(ChannelImageVerify, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteSaveCommand(object parameter)
        {
            if (isRightClick != true) return;
            try
            {
                if (EpoxyParameter.ImageCountChannels > 0 && EpoxyModelVerifyParameter.ImageChannelIndex < 0)
                {
                    MessageBox.Show("请先选择验证图像通道！");
                    return;
                }
                OnSaveXML?.Invoke();
                MessageBox.Show("参数保存完成!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "参数保存失败!");
            }
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
            if (EpoxyUserRegions.Count == 0)
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
            htWindow.ClearSelection();
            if (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff) return;
            htWindow.DisplayMultiRegion(EpoxyRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, EpoxyModelVerifyParameter.ImageChannelIndex));
        }

        public void Dispose()
        {
            (Content as Page_AddEpoxyRegion).DataContext = null;
            (Content as Page_AddEpoxyRegion).Close();
            Content = null;
            htWindow = null;
            ImageVerify = null;
            ChannelImageVerify = null;
            EpoxyParameter = null;
            EpoxyModelObject = null;
            EpoxyUserRegions = null;
            EpoxyModelVerifyParameter = null;
            EpoxyModelVerifyParameterSet = null;
            AddUserRegionCommand = null;
            AddReferenceUserRegionCommand = null;
            RemoveUserRegionCommand = null;
            RemoveReferenceUserRegionCommand = null;
            UserRegionEnableChangedCommand = null;
            LoadReferenceCommand = null;
            ModifyRegionCommand = null;
            ModifyReferenceUserRegionCommand = null;
            PreviousCommand = null;
            NextCommand = null;
            VerifyCommand = null;
            SaveCommand = null;
            ImagesSetVerifyCommand = null;
            ReferenceUserRegionEnableChangedCommand = null;
            RefreshImagesSet = null;
            AutoGenEpoxyRegionCommand = null;
            LoadReferenceUserRegionCommond = null;
        }
    }
}
