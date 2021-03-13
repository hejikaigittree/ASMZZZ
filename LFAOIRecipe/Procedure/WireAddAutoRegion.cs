using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LFAOIRecipe
{
    public class WireAddAutoRegion : ViewModelBase, IProcedure
    {
        //1122
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public static bool isRightClickWire = true;

        private readonly string ModelsFile;

        private readonly string RecipeFile;
        //add by wj
        public static bool IsLoadIniData = false;

        public static bool IsAutoLoadStartBallReg = true;

        public static bool IsAutoLoadStopBallReg = true;

        //1109 add for pickup sort function
        public int Start_first_index = 0;
        public int Stop_first_index = 0;
        public ObservableCollection<OnRecipe> StartBondOnRecipes { get; set; } = new ObservableCollection<OnRecipe>();//起始焊点
        public ObservableCollection<OnRecipe> EndBondOnRecipes { get; set; } = new ObservableCollection<OnRecipe>();//结束焊点
        public ObservableCollection<UserRegion> StartBallAutoUserRegion { get; private set; }
        public ObservableCollection<UserRegion> StopBallAutoUserRegion { get; private set; }
        public ObservableCollection<UserRegion> StartStopLineAutoUserRegion { get; private set; }
        public ObservableCollection<UserRegion> LineModelRegions { get; private set; }

        //
        HTuple startBondRows, startBondCols = null;

        HTuple stopBondRows, stopBondCols = null;

        HTuple StartRegToWhat = null;

        HTuple StopRegToWhat = null;

        HObject UserRegions = new HObject();

        //private HObject ChannelImage = null;

        public int ImageChannelIndex;

        private string imageIndex = "System.Windows.Controls.ComboBoxItem: 原图";
        public string ImageIndex
        {
            get => imageIndex;
            set
            {
                if (imageIndex != value)
                {
                    WireObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex);
                    htWindow.Display(WireObject.DieChannelImage);
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
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
                    //DispalyModelGroupRegion();
                    UpdateModelIndexChange();
                }
            }
        }

        //2020-10-21 add wj
        private bool? isStartBondCheckAll;
        public bool? IsStartBondCheckAll
        {
            get => isStartBondCheckAll;
            set => OnPropertyChanged(ref isStartBondCheckAll, value);
        }

        private bool? isStopBondCheckAll;
        public bool? IsStopBondCheckAll
        {
            get => isStopBondCheckAll;
            set => OnPropertyChanged(ref isStopBondCheckAll, value);
        }

        private bool? isWireRegionCheckAll;
        public bool? IsWireRegionCheckAll
        {
            get => isWireRegionCheckAll;
            set => OnPropertyChanged(ref isWireRegionCheckAll, value);
        }


        //by wj
        private int modelGroupsCount;
        public int ModelGroupsCount
        {
            get => modelGroupsCount;
            set => OnPropertyChanged(ref modelGroupsCount, value);
        }

        private UserRegion selectedModelRegion;
        public UserRegion SelectedModelRegion
        {
            get => selectedModelRegion;
            set => OnPropertyChanged(ref selectedModelRegion, value);
        }

        //--设置金线检测模板区域内检测金线所使用的金线图像索引号 by wj
        private int imageIndex0;
        public int ImageIndex0
        {
            get => imageIndex0;
            set
            {
                if (imageIndex0 != value && SelectedModelRegion != null)
                {
                    if (value == -1)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, SelectedModelRegion.WireRegionWithPara.WireThresAlgoPara.ImageIndex + 1);

                        htWindow.Display(ChannelImageDisplay, false);
                    }
                    else if (value >= 0)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                        SelectedModelRegion.WireRegionWithPara.WireThresAlgoPara.ImageIndex = value;
                        WireParameter.ImageIndex = value;
                        htWindow.Display(ChannelImageDisplay, false);
                    }
                    imageIndex0 = value;
                    OnPropertyChanged();
                }
            }
        }

        private int imageIndex1;
        public int ImageIndex1
        {
            get => imageIndex1;
            set
            {
                if (imageIndex1 != value && SelectedModelRegion != null)
                {
                    if (value == -1)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, SelectedModelRegion.WireRegionWithPara.WireLineGauseAlgoPara.ImageIndex + 1);
                        htWindow.Display(ChannelImageDisplay, false);
                    }
                    else if (value >= 0)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                        SelectedModelRegion.WireRegionWithPara.WireLineGauseAlgoPara.ImageIndex = value;
                        WireParameter.ImageIndex = value;
                        htWindow.Display(ChannelImageDisplay, false);
                    }
                    imageIndex1 = value;
                    OnPropertyChanged();

                }
            }
        }

        private int imageIndex2;
        public int ImageIndex2
        {
            get => imageIndex2;
            set
            {
                if (imageIndex2 != value && SelectedModelRegion != null)
                {
                    if (value == -1)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, SelectedModelRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.ImageIndex + 1);
                        htWindow.Display(ChannelImageDisplay, false);
                    }
                    else if (value >= 0)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                        SelectedModelRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.ImageIndex = value;
                        WireParameter.ImageIndex = value;
                        htWindow.Display(ChannelImageDisplay, false);
                    }
                    imageIndex2 = value;
                    OnPropertyChanged();
                }
            }
        }


        //by lht
        private UserRegion selectedStartRegion;

        public UserRegion SelectedStartRegion
        {
            get => selectedStartRegion;
            set => OnPropertyChanged(ref selectedStartRegion, value);
        }

        public ObservableCollection<WireAutoRegionGroup> ModelGroups { get; private set; }

        public CommandBase AddModelGroupCommand { get; private set; }
        public CommandBase RemoveModelGroupCommand { get; private set; }
        public CommandBase LoadReferenceCommand { get; private set; }
        public CommandBase DisplayAllRegionCommand { get; private set; }

        //StartRegion Conmand
        public CommandBase StartBondOnWhatCommand { get; private set; }
        public CommandBase GenStartBondUserRegionCommand { get; private set; }
        public CommandBase DisplayAutoStartBondRegionsCommand { get; private set; }
        public CommandBase AddAutoStartBondUserRegionCommand { get; private set; }
        public CommandBase RemoveAutoStartBondUserRegionCommand { get; private set; }
        public CommandBase ModifyAutoStartBondRegionCommand { get; private set; }
        public CommandBase StartBallSortCommand { get; private set; }
        public CommandBase IsCheckStartCommand { get; private set; }
        public CommandBase IsCheckAllStartCommand { get; private set; }
        public CommandBase BindingWireModelCommand { get; private set; }
        public CommandBase StartPickUpCommand { get; private set; }
        public CommandBase DrawStartVirtualBallCommand { get; private set; }
        public CommandBase SelectedChangedImageCommand { get; private set; }

        //1109 add for pickup sort function
        public CommandBase SelectionChangedStartIndexCommand { get; private set; }
        public CommandBase SelectionChangedStopIndexCommand { get; private set; }
        //12-17 add for sort core point
        public CommandBase AddCoreUserPointCommand { get; private set; }

        //EndRegion Conmand
        public CommandBase EndBondOnWhatCommand { get; private set; }
        public CommandBase GenEndBondUserRegionCommand { get; private set; }
        public CommandBase DisplayAutoStopBondRegionsCommand { get; private set; }
        public CommandBase AddAutoEndBondUserRegionCommand { get; private set; }
        public CommandBase RemoveAutoEndBondUserRegionCommand { get; private set; }
        public CommandBase ModifyAutoEndBondRegionCommand { get; private set; }
        public CommandBase StopBallSortCommand { get; private set; }
        public CommandBase IsCheckStopCommand { get; private set; }
        public CommandBase IsCheckAllStopCommand { get; private set; }
        public CommandBase StopPickUpCommand { get; private set; }
        public CommandBase DrawStopVirtualBallCommand { get; private set; }

        //---WireRegion
        public CommandBase SelectionChangedModelIndexCommand { get; private set; }
        public CommandBase DisplayAllBondRegionCommand { get; private set; }
        //public CommandBase GenAutoWireUserRegionCommand { get; private set; }
        public CommandBase AddModelWireUserRegionCommand { get; private set; }
        public CommandBase RemoveModelWireUserRegionCommand { get; private set; }
        public CommandBase ModifyModelWireRegionCommand { get; private set; }

        public CommandBase SelectLineModelRegionCommand { get; private set; }
        public CommandBase SelectAllLineModelRegionCommand { get; private set; }

        public CommandBase WireRegionPickUpCommand { get; private set; }
        public CommandBase WirePickUpCommand { get; private set; }

        private OnRecipe selectedOnRecipe;

        public ListBox listboxstart;

        private HTHalControlWPF htWindow;

        private WireObject WireObject;
        public WireParameter WireParameter { get; set; }

        public string ReferenceDirectory { get; set; }

        public WireAddAutoRegion(HTHalControlWPF htWindow,
                             string modelsFile,
                             string recipeFile,
                             string referenceDirectory,
                             WireObject wireObject,
                             WireParameter wireParameter,
                             ObservableCollection<OnRecipe> startBondOnRecipes,
                             ObservableCollection<OnRecipe> endBondOnRecipes,
                             ObservableCollection<UserRegion> startBallAutoUserRegion,
                             ObservableCollection<UserRegion> stopBallAutoUserRegion,
                             ObservableCollection<WireAutoRegionGroup> modelGroups )
        {
            DisplayName = "金线自动生成";
            Content = new Page_WireAddAutoRegion { DataContext = this };
            this.htWindow = htWindow;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.ReferenceDirectory = referenceDirectory;
            this.StartBondOnRecipes = startBondOnRecipes;
            this.EndBondOnRecipes = endBondOnRecipes;
            this.StartBallAutoUserRegion = startBallAutoUserRegion;
            this.StopBallAutoUserRegion = stopBallAutoUserRegion; 
            this.ModelGroups = modelGroups;
            this.WireObject = wireObject;
            this.WireParameter = wireParameter;

            IsLoadIniData = false;
            IsAutoLoadStartBallReg = false;
            IsAutoLoadStopBallReg = false;
            LoadReferenceData();

            this.StartStopLineAutoUserRegion = new ObservableCollection<UserRegion>();

            AddModelGroupCommand = new CommandBase(ExecuteAddModelGroupCommand);
            RemoveModelGroupCommand = new CommandBase(ExecuteRemoveModelGroupCommand);
            LoadReferenceCommand = new CommandBase(ExecuteLoadReferenceCommand);
            // 1122-lw
            SelectedChangedImageCommand = new CommandBase(ExecuteSelectedChangedImageCommand);

            //----StartRegion 
            StartBondOnWhatCommand = new CommandBase(ExecuteStartBondOnWhatCommand);
            GenStartBondUserRegionCommand = new CommandBase(ExecuteGenStartBondUserRegionCommand);
            DisplayAutoStartBondRegionsCommand = new CommandBase(ExecuteDisplayAutoStartBondRegionsCommand);
            AddAutoStartBondUserRegionCommand = new CommandBase(ExecuteAddAutoStartBondUserRegionCommand);
            RemoveAutoStartBondUserRegionCommand = new CommandBase(ExecuteRemoveAutoStartBondUserRegionCommand);
            ModifyAutoStartBondRegionCommand = new CommandBase(ExecuteModifyAutoStartBondRegionCommand);
            StartBallSortCommand = new CommandBase(ExecuteStartBallSortCommand);
            IsCheckStartCommand = new CommandBase(ExecuteIsCheckStartCommand);
            IsCheckAllStartCommand = new CommandBase(ExecuteIsCheckAllStartCommand);
            BindingWireModelCommand = new CommandBase(ExecuteBindingWireModelCommand);
            StartPickUpCommand = new CommandBase(ExecuteStartPickUpCommand);
            DrawStartVirtualBallCommand = new CommandBase(ExecuteDrawStartVirtualBallCommand);

            //1109 add for pickup sort function
            SelectionChangedStartIndexCommand = new CommandBase(ExecuteSelectionChangedStartIndexCommand);
            SelectionChangedStopIndexCommand = new CommandBase(ExecuteSelectionChangedStopIndexCommand);
            //12-17 add for sort core point
            AddCoreUserPointCommand = new CommandBase(ExecuteAddCoreUserPointCommand);

            //-----EndRegion
            EndBondOnWhatCommand = new CommandBase(ExecuteEndBondOnWhatCommand);
            GenEndBondUserRegionCommand = new CommandBase(ExecuteGenEndBondUserRegionCommand);
            DisplayAutoStopBondRegionsCommand = new CommandBase(ExecuteDisplayAutoStopBondRegionsCommand);
            AddAutoEndBondUserRegionCommand = new CommandBase(ExecuteAddAutoEndBondUserRegionCommand);
            RemoveAutoEndBondUserRegionCommand = new CommandBase(ExecuteRemoveAutoEndBondUserRegionCommand);
            ModifyAutoEndBondRegionCommand = new CommandBase(ExecuteModifyAutoEndBondRegionCommand);
            StopBallSortCommand = new CommandBase(ExecuteStopBallSortCommand);
            IsCheckStopCommand = new CommandBase(ExecuteIsCheckStopCommand);
            IsCheckAllStopCommand = new CommandBase(ExecuteIsCheckAllStopCommand);
            StopPickUpCommand = new CommandBase(ExecuteStopPickUpCommand);
            DrawStopVirtualBallCommand = new CommandBase(ExecuteDrawStopVirtualBallCommand);

            //-----ModelWireRegion

            //GenAutoWireUserRegionCommand = new CommandBase(ExecuteGenAutoWireUserRegionCommand);

            DisplayAllBondRegionCommand = new CommandBase(ExecuteDisplayAllBondRegionCommand);
            SelectionChangedModelIndexCommand = new CommandBase(ExecuteSelectionChangedModelIndexCommand);

            AddModelWireUserRegionCommand = new CommandBase(ExecuteAddModelWireUserRegionCommand);
            RemoveModelWireUserRegionCommand = new CommandBase(ExecuteRemoveModelWireUserRegionCommand);
            ModifyModelWireRegionCommand = new CommandBase(ExecuteModifyModelWireRegionCommand);

            SelectLineModelRegionCommand = new CommandBase(ExecuteSelectLineModelRegionCommand);
            SelectAllLineModelRegionCommand = new CommandBase(ExecuteSelectAllLineModelRegionCommand);

            WireRegionPickUpCommand = new CommandBase(ExecuteWireRegionPickUpCommand);
            WirePickUpCommand = new CommandBase(ExecuteWirePickUpCommand);

            //CurrentModelGroup = new WireAutoRegionGroup
            //{
            //    Index = modelGroups.Count + 1
            //};
            //modelGroups.Add(CurrentModelGroup);
            //ModelGroupsCount = modelGroups.Count;

        }

        private void ExecuteSelectedChangedImageCommand(object parameter)
        {
            // 1123
            if (WireParameter.ImageIndex >= 0)
            {
                WireObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex);
                ImageChannelIndex = WireParameter.ImageIndex;
                htWindow.Display(WireObject.ChannelImage);
                OnPropertyChanged();
            }
            else
            {
                WireParameter.ImageIndex = ImageChannelIndex;
            }
        }

        //----------------加载参考
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
                WireRecipe.IsLoadXML = false;
                IsLoadIniData = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "全局数据文件不存在！");
                return;
            }
        }
        public void LoadReferenceData()
        {
            IsLoadIniData = true;
            IsAutoLoadStartBallReg = true;
            IsAutoLoadStopBallReg = true;

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

            //后续优化
            HOperatorSet.ReadTuple(ReferenceDirectory + "TrainningImagesDirectory.tup", out HTuple TrainningImagesDirectoryTemp);
            WireParameter.VerifyImagesDirectory = TrainningImagesDirectoryTemp;

            WireParameter.ImagePath = $"{ReferenceDirectory}ReferenceImage.tiff";
            WireObject.Image?.Dispose();
            LoadImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageRowOffset.tup", out HTuple dieImageRowOffset);
            WireParameter.DieImageRowOffset = dieImageRowOffset;
            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageColumnOffset.tup", out HTuple dieImageColumnOffset);
            WireParameter.DieImageColumnOffset = dieImageColumnOffset;

            HOperatorSet.ReadRegion(out HObject UserRegionForCutOut_Region, ReferenceDirectory + "DieReference.reg");
            HOperatorSet.ReduceDomain(WireObject.Image, UserRegionForCutOut_Region, out HObject imageReduced);
            HOperatorSet.CropDomain(imageReduced, out HObject dieImage);
            WireObject.DieImage = dieImage;
            LoadDieImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "UserRegionForCutOutIndex.tup", out HTuple UserRegionForCutOutIndexTemp);
            WireParameter.UserRegionForCutOutIndex = UserRegionForCutOutIndexTemp;

            //1122 lw
            HOperatorSet.ReadTuple(ReferenceDirectory + "ImageChannelnumber.tup", out HTuple ImageChannelnumber);
            ChannelNames.Clear();
            for (int i = 0; i < ImageChannelnumber; i++)
            {
                ChannelName tmp_name = new ChannelName();
                tmp_name.Name = (i + 1).ToString();
                ChannelNames.Add(tmp_name);
            }

            ImageChannelIndex = WireParameter.ImageIndex;

            WireParameter.ChannelNames = ChannelNames;
            OnPropertyChanged("WireParameter.ImageIndex");

            //1201 lw
            HOperatorSet.TupleSplit(ReferenceDirectory, "\\", out HTuple hv_subStr);
            HTuple FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 3];
            WireParameter.CurFovName = FOV_Name;

            htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex));

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
            HOperatorSet.ReadImage(out image, WireParameter.ImagePath);
            WireObject.Image = image;

            //通道分离
            HOperatorSet.CountChannels(WireObject.Image, out HTuple channels);
            WireParameter.ImageCountChannels = channels;
            WireObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(WireObject.Image, WireParameter.ImageIndex);
            //WireObject.ImageB = Algorithm.Region.GetChannnelImageUpdate(WireObject.Image, 1);
            //WireObject.ImageR = Algorithm.Region.GetChannnelImageUpdate(WireObject.Image, 2);
            //WireObject.ImageG = Algorithm.Region.GetChannnelImageUpdate(WireObject.Image, 3);
        }
        public void LoadDieImage()//
        {
            WireObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex);
            //WireObject.DieImageB = Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, 1);
            //WireObject.DieImageR = Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, 2);
            //WireObject.DieImageG = Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, 3);
        }

        //******************************************************************************
        #region 金线检测起始焊点自动生成以及排序
        //获取金线起始焊点所有归属
        private void ExecuteStartBondOnWhatCommand(object parameter)
        {

            string msg = string.Empty;
            HTuple msg_htuple;
            List<OnRecipe> list = new List<OnRecipe>();
            if (listboxstart.SelectedItems != null && listboxstart.SelectedItems.Count > 1 && StartRegToWhat != null)
            {
                //多选
                foreach (OnRecipe lbi in listboxstart.SelectedItems)
                {
                    msg = lbi.Name;
                    list.Add(lbi);
                }
                msg_htuple = StartRegToWhat.TupleConcat(msg);
            }


            //标记当前点击的序号
            int current_index;
            current_index = -1;
            //赋值上步操作的选中标记
            for (int i = 0; i < StartBondOnRecipes.Count; i++)
            {
                if (StartBondOnRecipes.ElementAt(i).IsSelected_pre != StartBondOnRecipes.ElementAt(i).IsSelected)
                {
                    current_index = i;
                    StartBondOnRecipes.ElementAt(i).IsSelected_pre = StartBondOnRecipes.ElementAt(i).IsSelected;
                    break;
                }               
            }


            var OnRecipesResult = StartBondOnRecipes.Where(x => x.IsSelected).ToList();

            ObservableCollection<OnRecipe> OnRecipesResult_Sort;

            // Listbox 新单击产生
            if (current_index > -1)
            {
                selectedOnRecipe = StartBondOnRecipes.ElementAt(current_index);

                if (selectedOnRecipe.IsSelected == false)  //当前条目被取消选择，排序号设为-1，并根据其取消的前的序号，调整其它被选择条目的序号
                {
                    //step-1: 调整其它被选择条目的序号，小于当前序号的条目不用被调整，大于当前序号的条目的对应序号 减去 1 
                    for (int i = 0; i < OnRecipesResult.Count; i++)
                    {
                        if (OnRecipesResult.ElementAt(i).Selected_ind > selectedOnRecipe.Selected_ind)
                        {
                            OnRecipesResult.ElementAt(i).Selected_ind--;
                        }
                    }

                    //step-2: 当前条目排序号设为 -1
                    selectedOnRecipe.Selected_ind = 0;
                }
                else                                    //当前条目被选择，其排序号 = 已经有的被选择的条目数+1，
                {
                    selectedOnRecipe.Selected_ind = OnRecipesResult.Count + 0;
                }

                OnRecipesResult = StartBondOnRecipes.Where(x => x.Selected_ind > 0).ToList();  //选取那些序号大于1的条目

                OnRecipesResult_Sort = new ObservableCollection<OnRecipe>(OnRecipesResult.OrderBy(r => r.Selected_ind));

                OnRecipesResult.OrderBy(r => r.Selected_ind);
            }
            else
            {
                return;
            }
            //0929-上

            StartRegToWhat = new HTuple();

            for (int i = 0; i < OnRecipesResult_Sort.Count ; i++)
            {
                string  _startToWhat = OnRecipesResult_Sort.ElementAt(i).Name;
                StartRegToWhat =StartRegToWhat.TupleConcat(_startToWhat);
            }
            WireParameter.StartBondonRecipesIndexs = StartRegToWhat;
        }

        //生成自动起始焊点
        private void ExecuteGenStartBondUserRegionCommand(object parameter)
        {
            //新添加部分
            if (WireParameter.IsEnableStartVirtualBond == true)
            {
                // mod lw 1113
                HOperatorSet.GenEmptyObj(out HObject StartBallRegs);
                if (MessageBox.Show("是否从文件加载虚拟焊点区域？", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                { 
                    using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                    {
                        ofd.Filter = "reg|*.reg";
                        ofd.Multiselect = false;
                        ofd.InitialDirectory = $"{ModelsFile}FreeRegion";
                        if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                        HOperatorSet.ReadRegion(out StartBallRegs, ofd.FileName);
                    }
                }
                HOperatorSet.MoveRegion(StartBallRegs, out HObject _StartBallRegs, -WireParameter.DieImageRowOffset, -WireParameter.DieImageColumnOffset);
                htWindow.DisplayMultiRegion(_StartBallRegs);

                //1028 重新排序，线模板的线序号保持不变
                int tmp_SelectModelNumber = 0;
                if (CurrentModelGroup == null)
                {
                    tmp_SelectModelNumber = -1;
                }
                else
                {
                    tmp_SelectModelNumber = CurrentModelGroup.SelectModelNumber;
                }

                StartBallAutoUserRegion.Clear();

                for (int i = 0; i < _StartBallRegs.CountObj(); i++)
                {
                    HOperatorSet.AreaCenter(_StartBallRegs.SelectObj(i + 1), out HTuple area, out HTuple row, out HTuple column);
                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                    RegionType.Circle,
                                                                    row,
                                                                    column,
                                                                    WireParameter.GenStartBallSize, 0,
                                                                    WireParameter.DieImageRowOffset,
                                                                    WireParameter.DieImageColumnOffset,
                                                                    0);
                    if (userRegion_Circle == null) return;

                    userRegion_Circle.Index = StartBallAutoUserRegion.Count + 1;
                    userRegion_Circle.Index_ini = StartBallAutoUserRegion.Count + 1;
                    StartBallAutoUserRegion.Add(userRegion_Circle);                  
                }

                //1028 重新排序后，线模板的线序号保持不变
                if (CurrentModelGroup != null)
                {
                    CurrentModelGroup.SelectModelNumber = tmp_SelectModelNumber;
                }

                //htWindow.DisplayMultiRegionWithIndex(StartBallAutoUserRegion);
                DispalyGroupsStartRegions();

                //1109 add for pickup sort function
                WireParameter.StartFirstSortNumber = 0;
                Start_first_index = 1;
                Stop_first_index = 1;

                //1109
                if (StartBallAutoUserRegion.Count() != StopBallAutoUserRegion.Count() && StopBallAutoUserRegion.Count() > 0)//1109
                {
                    MessageBox.Show("起始点与结束点区域数量不一致，请检查焊点对应情况");
                }
                else
                {
                    Array.Clear(WireParameter.WireAutoIndex_sorted_start, 0, WireParameter.WireAutoIndex_sorted_start.Length);
                    WireParameter.WireAutoIndex_sorted_start = new int[StartBallAutoUserRegion.Count];
                    for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                    {
                        WireParameter.WireAutoIndex_sorted_start[i] = i + 1;
                    }
                }

                //初始化 WireParameter.WireRegModelType
                if (WireParameter.WireRegModelType.Count() >= 0 || WireParameter.WireRegModelType.Count() != StartBallAutoUserRegion.Count())
                {
                    //initilize wiremodeltype
                    Array.Clear(WireParameter.WireRegModelType, 0, WireParameter.WireRegModelType.Length);
                    WireParameter.WireRegModelType = new int[StartBallAutoUserRegion.Count];
                    for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                    {
                        if (ModelGroups.Count > 0)
                        {
                            WireParameter.WireRegModelType[i] = 1;
                        }
                        else
                        {
                            WireParameter.WireRegModelType[i] = 0;
                        }

                    }

                }

                if (CurrentModelGroup != null && ModelGroups.Count > 0 && WireParameter.WireRegModelType.Length == StartBallAutoUserRegion.Count())
                {
                    for (int i = 0; i < StartBallAutoUserRegion.Count; i++)
                    {
                        StartBallAutoUserRegion[i].ModelGroups = ModelGroups;
                        StartBallAutoUserRegion[i].CurrentModelGroup = ModelGroups.ElementAt(WireParameter.WireRegModelType[i] - 1);

                    }

                }
            }
            else
            {
                if (IsAutoLoadStartBallReg && IsLoadIniData)
                {
                    StartRegToWhat = WireParameter.StartBondonRecipesIndexs;//1029 读取XML时赋值防呆

                    if (StartRegToWhat == null)
                    {
                        MessageBox.Show("未选定起始焊点归属，请选择！");
                        return;
                    }
                    else if (StartRegToWhat.TupleLength() == 0)
                    {
                        MessageBox.Show("未选定起始焊点归属，请选择！");
                        return;
                    }
                    //生成需要编辑的起始焊点区域
                    HOperatorSet.ReadRegion(out HObject DieRegion, $"{ReferenceDirectory}DieReference.reg");

                    Algorithm.Model_RegionAlg.HTV_Gen_Ball_UserRegion(Algorithm.Region.GetChannnelImageConcact(WireObject.Image),
                                                                      DieRegion, out HObject ho_o_UserBallRegions, StartRegToWhat,
                                                                      ModelsFile,
                                                                      RecipeFile,
                                                                      WireParameter.GenStartBallSize,
                                                                      out startBondRows,
                                                                      out startBondCols,
                                                                      out HTuple hv__genBallErrCode,
                                                                      out HTuple hv__genBallErrStr);
                    if ((int)(new HTuple((new HTuple(startBondRows.TupleLength())).TupleLess(0))) != 0)
                    {
                        MessageBox.Show("自动生成焊点数为0！");
                        return;
                    }

                    //1028 重新排序，线模板的线序号保持不变
                    int tmp_SelectModelNumber = 0;
                    if (CurrentModelGroup == null)
                    {
                        tmp_SelectModelNumber = -1;
                    }
                    else
                    {
                        tmp_SelectModelNumber = CurrentModelGroup.SelectModelNumber;
                    }

                    StartBallAutoUserRegion.Clear();

                    for (int i = 0; i < startBondRows.TupleLength(); i++)
                    {
                        UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                        RegionType.Circle,
                                                                        startBondRows[i] - WireParameter.DieImageRowOffset,
                                                                        startBondCols[i] - WireParameter.DieImageColumnOffset,
                                                                        WireParameter.GenStartBallSize, 0,
                                                                        WireParameter.DieImageRowOffset,
                                                                        WireParameter.DieImageColumnOffset,
                                                                        0);
                        if (userRegion_Circle == null) return;

                        userRegion_Circle.Index = StartBallAutoUserRegion.Count + 1;
                        userRegion_Circle.Index_ini = StartBallAutoUserRegion.Count + 1; //1022
                        StartBallAutoUserRegion.Add(userRegion_Circle);
                    }

                    //1028 重新排序后，线模板的线序号保持不变
                    if (CurrentModelGroup != null)
                    {
                        CurrentModelGroup.SelectModelNumber = tmp_SelectModelNumber;
                    }

                    //htWindow.DisplayMultiRegionWithIndex(StartBallAutoUserRegion);
                    DispalyGroupsStartRegions();

                    //1109 add for pickup sort function
                    WireParameter.StartFirstSortNumber = 0;
                    Start_first_index = 1;
                    Stop_first_index = 1;

                    //1109
                    if (StartBallAutoUserRegion.Count() != StopBallAutoUserRegion.Count() && StopBallAutoUserRegion.Count() > 0)//1109
                    {
                        MessageBox.Show("起始点与结束点区域数量不一致，请检查焊点对应情况");
                    }
                    else
                    {
                        Array.Clear(WireParameter.WireAutoIndex_sorted_start, 0, WireParameter.WireAutoIndex_sorted_start.Length);
                        WireParameter.WireAutoIndex_sorted_start = new int[StartBallAutoUserRegion.Count];
                        for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                        {
                            WireParameter.WireAutoIndex_sorted_start[i] = i + 1;
                        }
                    }

                    //初始化 WireParameter.WireRegModelType
                    if (WireParameter.WireRegModelType.Count() >= 0 || WireParameter.WireRegModelType.Count() != StartBallAutoUserRegion.Count())
                    {
                        //initilize wiremodeltype
                        Array.Clear(WireParameter.WireRegModelType, 0, WireParameter.WireRegModelType.Length);
                        WireParameter.WireRegModelType = new int[StartBallAutoUserRegion.Count];
                        for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                        {
                            if (ModelGroups.Count > 0)
                            {
                                WireParameter.WireRegModelType[i] = 1;
                            }
                            else
                            {
                                WireParameter.WireRegModelType[i] = 0;
                            }

                        }

                    }

                    if (CurrentModelGroup != null && ModelGroups.Count > 0 && WireParameter.WireRegModelType.Length == StartBallAutoUserRegion.Count())
                    {
                        for (int i = 0; i < StartBallAutoUserRegion.Count; i++)
                        {
                            StartBallAutoUserRegion[i].ModelGroups = ModelGroups;
                            StartBallAutoUserRegion[i].CurrentModelGroup = ModelGroups.ElementAt(WireParameter.WireRegModelType[i] - 1);

                        }

                    }
                    //1012               
                    if (StartBallAutoUserRegion.Count() > 0 && StopBallAutoUserRegion.Count() == StartBallAutoUserRegion.Count())  //如果有结束焊点，则重新生成焊点连线
                    {
                        ////生成每条线的模板线参数，默认都是 0                  
                        //if (WireParameter.WireRegModelType.Length > 0 && WireParameter.WireRegModelType.Length == StartBallAutoUserRegion.Count()) //如果有已有参数，则保持不变
                        //{
                        //    //int a;
                        //    int a = 0;
                        //}
                        //else  // 如果没有参数，重新置为1 或 0
                        //{
                        //    Array.Clear(WireParameter.WireRegModelType, 0, WireParameter.WireRegModelType.Length);
                        //    WireParameter.WireRegModelType = new int[StartBallAutoUserRegion.Count];
                        //    for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                        //    {
                        //        if (ModelGroups.Count > 0)
                        //        {
                        //            WireParameter.WireRegModelType[i] = 1;
                        //        }
                        //        else
                        //        {
                        //            WireParameter.WireRegModelType[i] = 0;
                        //        }

                        //    }
                        //}
                        //重新生成起始--结束焊点之间的连线
                        GenStartStopLineAutoUserRegion();
                        UpdateStartandStopLineRegions();

                    }

                }
                else
                {

                    MessageBox.Show("请手动画金线检测起始焊点虚拟区域或者检查是否加载全局数据！");
                    return;

                }

		    }
        }

        public void GenStartStopLineAutoUserRegion()
        {
            //重新生成一二焊点连线数据
            StartStopLineAutoUserRegion.Clear();

            HOperatorSet.GenEmptyObj(out HObject concatBondWireRegion);

            //----------获取起始焊点区域
            HTuple modelStartRow = new HTuple();
            HTuple modelStartCol = new HTuple();
            //
            HObject modelStartBall = new HObject();

            //----------------------选取结束焊点区域
            HTuple modelStopRow = new HTuple();
            HTuple modelStopCol = new HTuple();
            //
            HObject modelStopBall = new HObject();

            HObject modelLine = new HObject();

            UserRegion userRegion_Line;

            for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
            {
                //起始焊点
                modelStartRow = StartBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset;
                modelStartCol = StartBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                //结束焊点
                modelStopRow = StopBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset;
                modelStopCol = StopBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                //生成焊点连线
                HOperatorSet.GenRegionLine(out modelLine, modelStartRow, modelStartCol, modelStopRow, modelStopCol);

                userRegion_Line = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                           RegionType.Line,
                                                           StartBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                           StartBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                           StopBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                           StopBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                           WireParameter.DieImageRowOffset,
                                                           WireParameter.DieImageColumnOffset,
                                                           0);
                userRegion_Line.Index = StartStopLineAutoUserRegion.Count + 1;
                StartStopLineAutoUserRegion.Add(userRegion_Line);

            }
        }

        //显示所有区域和序号
        private void ExecuteDisplayAutoStartBondRegionsCommand(object parameter)
        {
            DispalyGroupsStartRegions();
        }

        //public void GetClickDownPointsFromStartBall()
        //{
        //    if (StartBallAutoUserRegion.Count == 0 || WireParameter.IsStartPickUp == false) return;

        //    HOperatorSet.GenRegionPoints(out HObject Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
        //    foreach (var item in StartBallAutoUserRegion)
        //    {
        //        HOperatorSet.SelectShapeProto(item.DisplayRegion, Point, out HObject overlapRegion, "overlaps_abs", 1.0, 5.0);


        //        if (overlapRegion.CountObj() > 0)
        //        {
        //            if (item.IsSelected == false)
        //            {
        //                htWindow.hTWindow.HalconWindow.SetColor("yellow");
        //                item.IsSelected = true;
        //                htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
        //            }
        //            else
        //            {
        //                htWindow.hTWindow.HalconWindow.SetColor("green");
        //                item.IsSelected = false;
        //                htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
        //            }
        //        }
        //    }
        //}

        //1109 add for pickup sort function
        public void GetClickDownPointsFromStartBall(int pick_select_type = 1, double get_mousepoint_r = 0.0, double get_mousepoint_c = 0.0)
        {
            if (StartBallAutoUserRegion.Count == 0 || WireParameter.IsStartPickUp == false) return;

            HObject Point = new HObject();

            if (get_mousepoint_r == 0.0)
            {
                HOperatorSet.GenRegionPoints(out Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
            }
            else
            {
                HOperatorSet.GenRegionPoints(out Point, get_mousepoint_r, get_mousepoint_c);
            }


            int i;
            i = 0;
            foreach (var item in StartBallAutoUserRegion)
            {
                i++;

                HOperatorSet.SelectShapeProto(item.DisplayRegion, Point, out HObject overlapRegion, "overlaps_abs", 1.0, 5.0);


                if (overlapRegion.CountObj() == 1)
                {
                    if (pick_select_type == 1) // left key to change selection state
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


                    //1022 拾取设置序号，从时针圆周排序号开始
                    if (WireParameter.StartSortMethod == 7 && item.IsSelected == true)
                    {
                        if (WireParameter.StartFirstSortNumber == -1)
                        {
                            MessageBox.Show("请选择排序起始焊点！");
                            htWindow.hTWindow.HalconWindow.SetColor("green");
                            item.IsSelected = false;
                            htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
                            return;
                        }
                        if (pick_select_type == 1)// 点击序号从指定序号（旋转起始点）开始递增
                        {
                            StartBallAutoUserRegion[i - 1].Index = Start_first_index;
                            Start_first_index++;
                            //WireParameter.StartFirstSortNumber++;
                            UpdateModelIndexChange();
                        }

                        if (pick_select_type == 2)  // 点击序号增加
                        {
                            StartBallAutoUserRegion[i - 1].Index = StartBallAutoUserRegion[i - 1].Index + 1;
                            Start_first_index = StartBallAutoUserRegion[i - 1].Index + 1;
                            WireParameter.StartFirstSortNumber = StartBallAutoUserRegion[i - 1].Index + 1;
                            UpdateModelIndexChange();
                        }
                        if (pick_select_type == 3) // 点击序号减少
                        {
                            StartBallAutoUserRegion[i - 1].Index = StartBallAutoUserRegion[i - 1].Index - 1;
                            Start_first_index = StartBallAutoUserRegion[i - 1].Index + 1;
                            WireParameter.StartFirstSortNumber = StartBallAutoUserRegion[i - 1].Index + 1;
                            UpdateModelIndexChange();
                        }

                    }

                    break;
                }

            }
        }
        //1109 add for pickup sort function
        public bool GetClickDownPointsFromStartBall_for_sort(int pick_select_type = 1, double get_mousepoint_r = 0.0, double get_mousepoint_c = 0.0)
        {
            if (StartBallAutoUserRegion.Count == 0 || WireParameter.IsStartPickUp == false) return false;

            HObject Point = new HObject();
            if (get_mousepoint_r == 0.0)
            {
                HOperatorSet.GenRegionPoints(out Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
            }
            else
            {
                HOperatorSet.GenRegionPoints(out Point, get_mousepoint_r, get_mousepoint_c);
            }


            int i;
            i = 0;
            foreach (var item in StartBallAutoUserRegion)
            {
                i++;
                HOperatorSet.SelectShapeProto(item.DisplayRegion, Point, out HObject overlapRegion, "overlaps_abs", 1.0, 5.0);


                if (overlapRegion.CountObj() > 0)
                {
                    if (item.IsSelected == true)
                    {
                        //htWindow.hTWindow.HalconWindow.SetColor("yellow");
                        //item.IsSelected = true;
                        //htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);

                        //1022 拾取设置序号，从时针圆周排序号开始
                        if (WireParameter.StartSortMethod == 7)
                        {
                            if (WireParameter.StartFirstSortNumber == -1)
                            {
                                MessageBox.Show("请选择排序起始焊点！");
                                htWindow.hTWindow.HalconWindow.SetColor("green");
                                item.IsSelected = false;
                                htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
                                return false;
                            }
                            if (pick_select_type == 1)// 点击序号从指定序号（界面的旋转起始点）开始递增
                            {
                                StartBallAutoUserRegion[i - 1].Index = WireParameter.StartFirstSortNumber + 1;
                                WireParameter.StartFirstSortNumber++;
                                UpdateModelIndexChange();
                            }

                            if (pick_select_type == 2)  // 点击序号增加
                            {
                                StartBallAutoUserRegion[i - 1].Index = StartBallAutoUserRegion[i - 1].Index + 1;
                                Start_first_index = StartBallAutoUserRegion[i - 1].Index + 1;
                                UpdateModelIndexChange();
                            }
                            if (pick_select_type == 3) // 点击序号减少
                            {
                                StartBallAutoUserRegion[i - 1].Index = StartBallAutoUserRegion[i - 1].Index - 1;
                                Start_first_index = StartBallAutoUserRegion[i - 1].Index + 1;
                                UpdateModelIndexChange();
                            }


                        }

                        return true;

                    }
                    else
                    {
                        return false;
                    }

                }

            }

            return false;

        }


        //1109 add for pickup sort function
        public void GetClickDownPointsFromStopBall(int pick_select_type = 1, double get_mousepoint_r = 0.0, double get_mousepoint_c = 0.0)
        {
            if (StopBallAutoUserRegion.Count == 0 || WireParameter.IsStopPickUp == false) return;

            HObject Point = new HObject();

            if (get_mousepoint_r == 0.0)
            {
                HOperatorSet.GenRegionPoints(out Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
            }
            else
            {
                HOperatorSet.GenRegionPoints(out Point, get_mousepoint_r, get_mousepoint_c);
            }


            int i;
            i = 0;
            foreach (var item in StopBallAutoUserRegion)
            {
                i++;

                HOperatorSet.SelectShapeProto(item.DisplayRegion, Point, out HObject overlapRegion, "overlaps_abs", 1.0, 5.0);


                if (overlapRegion.CountObj() == 1)
                {
                    if (pick_select_type == 1) // left key to change selection state
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


                    //1022 拾取设置序号，从时针圆周排序号开始
                    if (WireParameter.StopSortMethod == 7 && item.IsSelected == true)
                    {
                        if (WireParameter.StopFirstSortNumber == -1)
                        {
                            MessageBox.Show("请选择排序起始焊点！");
                            htWindow.hTWindow.HalconWindow.SetColor("green");
                            item.IsSelected = false;
                            htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
                            return;
                        }
                        if (pick_select_type == 1)// 点击序号从指定序号（旋转起始点）开始递增
                        {
                            StopBallAutoUserRegion[i - 1].Index = Stop_first_index;
                            Stop_first_index++;
                            //WireParameter.StopFirstSortNumber++;
                            UpdateModelIndexChange();
                        }

                        if (pick_select_type == 2)  // 点击序号增加
                        {
                            StopBallAutoUserRegion[i - 1].Index = StopBallAutoUserRegion[i - 1].Index + 1;
                            Stop_first_index = StopBallAutoUserRegion[i - 1].Index + 1;
                            WireParameter.StopFirstSortNumber = StopBallAutoUserRegion[i - 1].Index + 1;
                            UpdateModelIndexChange();
                        }
                        if (pick_select_type == 3) // 点击序号减少
                        {
                            StopBallAutoUserRegion[i - 1].Index = StopBallAutoUserRegion[i - 1].Index - 1;
                            Stop_first_index = StopBallAutoUserRegion[i - 1].Index + 1;
                            WireParameter.StopFirstSortNumber = StopBallAutoUserRegion[i - 1].Index + 1;
                            UpdateModelIndexChange();
                        }

                    }

                    break;
                }

            }
        }
        //1109 add for pickup sort function
        public bool GetClickDownPointsFromStopBall_for_sort(int pick_select_type = 1, double get_mousepoint_r = 0.0, double get_mousepoint_c = 0.0)
        {
            if (StopBallAutoUserRegion.Count == 0 || WireParameter.IsStopPickUp == false) return false;

            HObject Point = new HObject();
            if (get_mousepoint_r == 0.0)
            {
                HOperatorSet.GenRegionPoints(out Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
            }
            else
            {
                HOperatorSet.GenRegionPoints(out Point, get_mousepoint_r, get_mousepoint_c);
            }


            int i;
            i = 0;
            foreach (var item in StopBallAutoUserRegion)
            {
                i++;
                HOperatorSet.SelectShapeProto(item.DisplayRegion, Point, out HObject overlapRegion, "overlaps_abs", 1.0, 5.0);


                if (overlapRegion.CountObj() > 0)
                {
                    if (item.IsSelected == true)
                    {
                        //htWindow.hTWindow.HalconWindow.SetColor("yellow");
                        //item.IsSelected = true;
                        //htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);

                        //1022 拾取设置序号，从时针圆周排序号开始
                        if (WireParameter.StopSortMethod == 7)
                        {
                            if (WireParameter.StopFirstSortNumber == -1)
                            {
                                MessageBox.Show("请选择排序起始焊点！");
                                htWindow.hTWindow.HalconWindow.SetColor("green");
                                item.IsSelected = false;
                                htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
                                return false;
                            }
                            if (pick_select_type == 1)// 点击序号从指定序号（界面的旋转起始点）开始递增
                            {
                                StopBallAutoUserRegion[i - 1].Index = WireParameter.StopFirstSortNumber + 1;
                                WireParameter.StopFirstSortNumber++;
                                UpdateModelIndexChange();
                            }

                            if (pick_select_type == 2)  // 点击序号增加
                            {
                                StopBallAutoUserRegion[i - 1].Index = StopBallAutoUserRegion[i - 1].Index + 1;
                                Stop_first_index = StopBallAutoUserRegion[i - 1].Index + 1;
                                UpdateModelIndexChange();
                            }
                            if (pick_select_type == 3) // 点击序号减少
                            {
                                StopBallAutoUserRegion[i - 1].Index = StopBallAutoUserRegion[i - 1].Index - 1;
                                Stop_first_index = StopBallAutoUserRegion[i - 1].Index + 1;
                                UpdateModelIndexChange();
                            }


                        }

                        return true;

                    }
                    else
                    {
                        return false;
                    }

                }

            }

            return false;

        }

        ////1109 add for pickup sort function选取起始区域号码
        private void ExecuteSelectionChangedStartIndexCommand(object obj)
        {
            //int Start_first_index;
            if (WireParameter.StartFirstSortNumber == -1)
            {
                Start_first_index = 1;
            }
            else
            {
                Start_first_index = StartBallAutoUserRegion.ElementAt(WireParameter.StartFirstSortNumber).Index + 0;
            }

        }
        ////1109 add for pickup sort function选取结束区域号码
        private void ExecuteSelectionChangedStopIndexCommand(object obj)
        {
            //int Stop_first_index;
            if (WireParameter.StopFirstSortNumber == -1)
            {
                Stop_first_index = 1;
            }
            else
            {
                Stop_first_index = StopBallAutoUserRegion.ElementAt(WireParameter.StopFirstSortNumber).Index + 0;
            }

        }


        // start region selection
        public void GetBoxSelectioinFromStartBall()
        {
            if (StartBallAutoUserRegion.Count == 0 || WireParameter.IsStartPickUp == false) return;

            //HOperatorSet.GenRegionPoints(out HObject Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
            // get selection box
            HOperatorSet.GenRectangle1(out HObject rectangle1_sel, htWindow.Row1_Rectangle1, htWindow.Column1_Rectangle1, htWindow.Row2_Rectangle1, htWindow.Column2_Rectangle1);

            foreach (var item in StartBallAutoUserRegion)
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
        public void DispalyGroupsStartRegions(bool isHTWindowRegion = true)
        {
            //1028
            if (htWindow.hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || htWindow.isdeleted == true) return;

            if (StartBallAutoUserRegion.Count == 0)
            {
                htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex), true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            foreach (var item in StartBallAutoUserRegion)
            {
                HOperatorSet.ConcatObj(concatGroupRegion, item.DisplayRegion, out concatGroupRegion);
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                htWindow.DisplaySingleRegion(concatGroupRegion, Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex));
            }
            HTuple Length1 = 10;
            foreach (var item in StartBallAutoUserRegion)
            {
                HOperatorSet.TupleSin(item.RegionParameters[2], out HTuple sin_out_line);
                HTuple line_r = item.RegionParameters[0] - WireParameter.DieImageRowOffset - Length1 * sin_out_line;
                HOperatorSet.TupleCos(item.RegionParameters[2], out HTuple cos_out_line);
                HTuple line_c = item.RegionParameters[1] - WireParameter.DieImageColumnOffset + Length1 * cos_out_line;

                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, line_r - 20, line_c + 5);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }
        //增加需要添加的金线起始焊点区域
        private void ExecuteAddAutoStartBondUserRegionCommand(object parameter)
        {
            if (isRightClickWire != true) return;
            if (htWindow.RegionType == RegionType.Null)
            {
                System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                         WireParameter.DieImageRowOffset,
                                                         WireParameter.DieImageColumnOffset);
                if (userRegion == null) return;
                userRegion.Index = StartBallAutoUserRegion.Count + 1;
                StartBallAutoUserRegion.Add(userRegion);

                DispalyGroupsStartRegions(true);
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
        // 12-17 add for sort core point 
        private void ExecuteAddCoreUserPointCommand(object parameter)
        {
            if (isRightClickWire != true) return;
            if (htWindow.RegionType != RegionType.Point)
            {
                System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                         WireParameter.DieImageRowOffset,
                                                         WireParameter.DieImageColumnOffset);
                if (userRegion == null) return;
                double center_row, center_column;
                center_row = (double)htWindow.Row_Point;
                center_column = (double)htWindow.Column_Point;
                WireParameter.CoreRow = center_row;
                WireParameter.CoreColumn = center_column;
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


        //删除自动生成的起始焊点区域
        private void ExecuteRemoveAutoStartBondUserRegionCommand(object parameter)
        {
            if (isRightClickWire != true) return;
            if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < StartBallAutoUserRegion.Count; i++)
                {
                    if (StartBallAutoUserRegion[i].IsSelected)
                    {
                        StartBallAutoUserRegion.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        StartBallAutoUserRegion[i].Index = i + 1;
                    }
                }

                //初始化 WireParameter.WireRegModelType
                if (WireParameter.WireRegModelType.Count() >= 0 || WireParameter.WireRegModelType.Count() != StartBallAutoUserRegion.Count())
                {
                    //initilize wiremodeltype
                    Array.Clear(WireParameter.WireRegModelType, 0, WireParameter.WireRegModelType.Length);
                    WireParameter.WireRegModelType = new int[StartBallAutoUserRegion.Count];
                    for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                    {
                        if (ModelGroups.Count > 0 && StartBallAutoUserRegion[i].CurrentModelGroup != null)
                        {
                            WireParameter.WireRegModelType[i] = StartBallAutoUserRegion[i].CurrentModelGroup.Index;
                        }
                        else
                        {
                            WireParameter.WireRegModelType[i] = 0;
                        }

                    }

                }

                DispalyGroupsStartRegions(false);

                if (StartBallAutoUserRegion.Count == 0)
                {
                    IsStartBondCheckAll = false;
                }
            }
        }
        //修改自动生成的起始焊点区域
        private void ExecuteModifyAutoStartBondRegionCommand(object parameter)
        {
            if (isRightClickWire)
            {

                isRightClickWire = false;
                try
                {
                    for (int i = 0; i < StartBallAutoUserRegion.Count; i++)
                    {
                        if (StartBallAutoUserRegion[i].IsSelected)
                        {
                            int IndexIni = StartBallAutoUserRegion[i].Index_ini;
                            switch (StartBallAutoUserRegion[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Line:

                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(StartBallAutoUserRegion[i].RegionParameters[0]),
                                                                  Math.Floor(StartBallAutoUserRegion[i].RegionParameters[1]),
                                                                  Math.Ceiling(StartBallAutoUserRegion[i].RegionParameters[2]),
                                                                  Math.Ceiling(StartBallAutoUserRegion[i].RegionParameters[3]),
                                                                  StartBallAutoUserRegion[i].RegionType,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (StartBallAutoUserRegion[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                                                                  (StartBallAutoUserRegion[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                                                                  (StartBallAutoUserRegion[i].RegionParameters[2] - WireParameter.DieImageRowOffset),
                                                                                                  (StartBallAutoUserRegion[i].RegionParameters[3] - WireParameter.DieImageColumnOffset),
                                                        out HTuple row1_Rectangle,
                                                        out HTuple column1_Rectangle,
                                                        out HTuple row2_Rectangle,
                                                        out HTuple column2_Rectangle);

                                    StartBallAutoUserRegion[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, StartBallAutoUserRegion[i].RegionType, row1_Rectangle, column1_Rectangle,
                                                           row2_Rectangle, column2_Rectangle, WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    StartBallAutoUserRegion[i] = userRegion;
                                    StartBallAutoUserRegion[i].Index = i + 1;
                                    StartBallAutoUserRegion[i].Index_ini = IndexIni;
                                    DispalyGroupsStartRegions(true);
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(StartBallAutoUserRegion[i].RegionParameters[0]),
                                                                  Math.Floor(StartBallAutoUserRegion[i].RegionParameters[1]),
                                                                  Math.Ceiling(StartBallAutoUserRegion[i].RegionParameters[2]),
                                                                  Math.Ceiling(StartBallAutoUserRegion[i].RegionParameters[3]),
                                                                  StartBallAutoUserRegion[i].RegionType,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(StartBallAutoUserRegion[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                                   Math.Floor(StartBallAutoUserRegion[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                                   StartBallAutoUserRegion[i].RegionParameters[2],
                                                                   Math.Ceiling(StartBallAutoUserRegion[i].RegionParameters[3]),
                                                                   Math.Ceiling(StartBallAutoUserRegion[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    StartBallAutoUserRegion[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, StartBallAutoUserRegion[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         WireParameter.DieImageRowOffset,
                                                                                                         WireParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    StartBallAutoUserRegion[i] = userRegion_Rectangle2;
                                    StartBallAutoUserRegion[i].Index = i + 1;
                                    StartBallAutoUserRegion[i].Index_ini = IndexIni;
                                    DispalyGroupsStartRegions(true);
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((StartBallAutoUserRegion[i].RegionParameters[0]),
                                                                  (StartBallAutoUserRegion[i].RegionParameters[1]),
                                                                  (StartBallAutoUserRegion[i].RegionParameters[2]),
                                                                  0,
                                                                  StartBallAutoUserRegion[i].RegionType,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (StartBallAutoUserRegion[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                               (StartBallAutoUserRegion[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                               StartBallAutoUserRegion[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    StartBallAutoUserRegion[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     StartBallAutoUserRegion[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     WireParameter.DieImageRowOffset,
                                                                                                     WireParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;

                                    //防呆
                                    StartBallAutoUserRegion[i] = userRegion_Circle;
                                    StartBallAutoUserRegion[i].Index = i + 1;
                                    StartBallAutoUserRegion[i].Index_ini = IndexIni;
                                    StartBallAutoUserRegion[i].ModelGroups = ModelGroups;
                                    if(CurrentModelGroup != null)
                                    {
                                        int PreModelType = WireParameter.WireRegModelType[i];

                                        //WireParameter.WireRegModelType[i] = CurrentModelGroup.Index;
                                        StartBallAutoUserRegion[i].CurrentModelGroup = ModelGroups.ElementAt(PreModelType - 1);
                                    }
                                    
                                    DispalyGroupsStartRegions(true);
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
                    MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    isRightClickWire = true;
                }

                //重新生成起始--结束焊点之间的金线并显示
                if (StartBallAutoUserRegion.Count() > 0 && StopBallAutoUserRegion.Count() == StartBallAutoUserRegion.Count())  //如果有结束焊点，则重新生成焊点连线
                {
                    GenStartStopLineAutoUserRegion();

                    UpdateStartandStopLineRegions();

                }

            }

        }
        //起始焊点区域排序
        private void ExecuteStartBallSortCommand(object parameter)
        {
            if (StartBallAutoUserRegion.Count() == 0) return;

            //1028 重新排序，线模板的线序号保持不变
            int tmp_SelectModelNumber = 0;
            if (CurrentModelGroup == null)
            {
                tmp_SelectModelNumber = -1;
            }
            else
            {
                tmp_SelectModelNumber = CurrentModelGroup.SelectModelNumber;
            }

            switch (WireParameter.StartSortMethod)
            {
                case 0:
                    //1022 手动设置序号排序的 有效性检查
                    bool manual_index_err = false;
                    int[] input_index = new int[StartBallAutoUserRegion.Count()];
                    for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                    {
                        input_index[i] = StartBallAutoUserRegion[i].Index;
                    }
                    Array.Sort(input_index);
                    for (int i = 0; i < StartBallAutoUserRegion.Count() - 1; i++)
                    {
                        if (input_index[i] != i + 1)
                        {
                            MessageBox.Show(string.Format("起始区域序号{0}不存在，请检查序号设置", i + 1));
                            manual_index_err = true;
                            break;
                        }
                        if (input_index[i] == input_index[i + 1])
                        {
                            MessageBox.Show(string.Format("起始区域序号{0}有重复，请检查序号设置", i + 1));
                            manual_index_err = true;
                            break;
                        }


                    }
                    if (manual_index_err)
                    {
                        //MessageBox.Show("起始区域手动填写序号有误，请检查");
                        return;
                    }
                    ObservableCollection<UserRegion> StartBallAutoUserRegionSort_0 = new ObservableCollection<UserRegion>(StartBallAutoUserRegion.OrderBy(r => r.Index));
                    StartBallAutoUserRegion.Clear();
                    for (int i = 0; i < StartBallAutoUserRegionSort_0.Count(); i++)
                    {
                        StartBallAutoUserRegion.Add(StartBallAutoUserRegionSort_0[i]);
                    }
                    break;

                case 1:
                    //列升序
                    ObservableCollection<UserRegion> StartBallAutoUserRegionSort_1 = new ObservableCollection<UserRegion>(StartBallAutoUserRegion.OrderBy(r => r.RegionParameters[1]));
                    StartBallAutoUserRegion.Clear();
                    for (int i = 0; i < StartBallAutoUserRegionSort_1.Count(); i++)
                    {
                        StartBallAutoUserRegion.Add(StartBallAutoUserRegionSort_1[i]);
                        StartBallAutoUserRegionSort_1[i].Index = StartBallAutoUserRegion.Count;
                    }
                    break;

                case 2:
                    //列降序
                    ObservableCollection<UserRegion> StartBallAutoUserRegionSort_2 = new ObservableCollection<UserRegion>(StartBallAutoUserRegion.OrderByDescending(r => r.RegionParameters[1]));
                    StartBallAutoUserRegion.Clear();
                    for (int i = 0; i < StartBallAutoUserRegionSort_2.Count(); i++)
                    {
                        StartBallAutoUserRegion.Add(StartBallAutoUserRegionSort_2[i]);
                        StartBallAutoUserRegionSort_2[i].Index = StartBallAutoUserRegion.Count;
                    }
                    break;

                case 3:
                    //行升序
                    ObservableCollection<UserRegion> StartBallAutoUserRegionSort_3 = new ObservableCollection<UserRegion>(StartBallAutoUserRegion.OrderBy(r => r.RegionParameters[0]));
                    StartBallAutoUserRegion.Clear();
                    for (int i = 0; i < StartBallAutoUserRegionSort_3.Count(); i++)
                    {
                        StartBallAutoUserRegion.Add(StartBallAutoUserRegionSort_3[i]);
                        StartBallAutoUserRegionSort_3[i].Index = StartBallAutoUserRegion.Count;
                    }
                    break;

                case 4:
                    //行降序
                    ObservableCollection<UserRegion> StartBallAutoUserRegionSort_4 = new ObservableCollection<UserRegion>(StartBallAutoUserRegion.OrderByDescending(r => r.RegionParameters[0]));
                    StartBallAutoUserRegion.Clear();
                    for (int i = 0; i < StartBallAutoUserRegionSort_4.Count(); i++)
                    {
                        StartBallAutoUserRegion.Add(StartBallAutoUserRegionSort_4[i]);
                        StartBallAutoUserRegionSort_4[i].Index = StartBallAutoUserRegion.Count;
                    }
                    break;

                case 5:
                    //顺时针
                    if (WireParameter.StartFirstSortNumber == -1)
                    {
                        MessageBox.Show("请选择排序起始焊点！");
                        return;
                    }
                    HTuple BondRows = new HTuple();
                    HTuple BondCols = new HTuple();
                    HTuple BondAngles = new HTuple();

                    foreach (var item in StartBallAutoUserRegion)
                    {
                        BondRows = BondRows.TupleConcat(item.RegionParameters[0]);
                        BondCols = BondCols.TupleConcat(item.RegionParameters[1]);
                        //生成固定值的角度
                        //HOperatorSet.TupleGenConst(new HTuple(BondRows.TupleLength()), 0, out BondAngles); //1217 此距拿到循环外面，只调用一次就可以
                        // BondAngles = BondAngles.TupleConcat(item.RegionParameters[2]);
                    }

                    HOperatorSet.TupleGenConst(new HTuple(BondRows.TupleLength()), 0, out BondAngles);//1217

                    Algorithm.Model_RegionAlg.HTV_Clockwise_Sort_Points_Angles(WireParameter.CoreRow,
                                    WireParameter.CoreColumn,
                                    BondRows,
                                    BondCols,
                                    BondAngles,
                                    WireParameter.StartFirstSortNumber,
                                    0,
                                    out HTuple SortRows,
                                    out HTuple SortCols,
                                    out HTuple SortAngles,
                                    out HTuple SortIndex);


                    //1217 start
                    HTuple sort_ind_ini = new HTuple();
                    HOperatorSet.TupleGenConst(new HTuple(BondRows.TupleLength()), 0, out sort_ind_ini);//1217
                    for(int i = 0; i < SortRows.TupleLength(); i++)
                    {
                        HTuple cur_sort_ind;
                        cur_sort_ind = SortIndex[i];
                        sort_ind_ini[i] = StartBallAutoUserRegion[cur_sort_ind].Index_ini + 0;
                    }
                    //1217 stop

                    StartBallAutoUserRegion.Clear();

                    for (int i = 0; i < SortRows.TupleLength(); i++)
                    {
                        UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                        RegionType.Circle,
                                                                        SortRows[i] - WireParameter.DieImageRowOffset,
                                                                        SortCols[i] - WireParameter.DieImageColumnOffset,
                                                                        WireParameter.GenStartBallSize, 0,
                                                                        WireParameter.DieImageRowOffset,
                                                                        WireParameter.DieImageColumnOffset,
                                                                        SortAngles[i]);

                        if (userRegion_Circle == null) return;

                        //1217 修正顺时针排序的初始序号问题
                        if (false)
                        {
                            userRegion_Circle.Index = StartBallAutoUserRegion.Count + 1;
                            userRegion_Circle.Index_ini = StartBallAutoUserRegion.Count + 1;
                            StartBallAutoUserRegion.Add(userRegion_Circle);
                        }
                        else
                        {
                            userRegion_Circle.Index = StartBallAutoUserRegion.Count + 1;
                            userRegion_Circle.Index_ini = sort_ind_ini[i];
                            StartBallAutoUserRegion.Add(userRegion_Circle);
                        }
                        
                    }
                    break;

                case 6:
                    //逆时针
                    if (WireParameter.StartFirstSortNumber == -1)
                    {
                        MessageBox.Show("请选择排序起始焊点！");
                        return;
                    }
                    HTuple AntiBondRows = new HTuple();
                    HTuple AntiBondCols = new HTuple();
                    HTuple AntiBondAngles = new HTuple();

                    foreach (var item in StartBallAutoUserRegion)
                    {
                        AntiBondRows = AntiBondRows.TupleConcat(item.RegionParameters[0]);
                        AntiBondCols = AntiBondCols.TupleConcat(item.RegionParameters[1]);
                        //生成固定值的角度
                        //HOperatorSet.TupleGenConst(new HTuple(AntiBondRows.TupleLength()), 0, out AntiBondAngles); // 1217 move out this loop
                    }
                    HOperatorSet.TupleGenConst(new HTuple(AntiBondRows.TupleLength()), 0, out AntiBondAngles); //1217

                    Algorithm.Model_RegionAlg.HTV_Clockwise_Sort_Points_Angles(WireParameter.CoreRow,
                                    WireParameter.CoreColumn,
                                    AntiBondRows,
                                    AntiBondCols,
                                    AntiBondAngles,
                                    WireParameter.StartFirstSortNumber,
                                    1,
                                    out HTuple SortAntiRows,
                                    out HTuple SortAntiCols,
                                    out HTuple SortAntiAngles,
                                    out HTuple SortAntiIndex);

                    //1217 start
                    HTuple sort_ind_ini2 = new HTuple();
                    HOperatorSet.TupleGenConst(new HTuple(AntiBondRows.TupleLength()), 0, out sort_ind_ini);//1217
                    for (int i = 0; i < AntiBondRows.TupleLength(); i++)
                    {
                        HTuple cur_sort_ind;
                        cur_sort_ind = SortAntiIndex[i];
                        sort_ind_ini2[i] = StartBallAutoUserRegion[cur_sort_ind].Index_ini + 0;
                    }
                    //1217 stop

                    StartBallAutoUserRegion.Clear();

                    for (int i = 0; i < SortAntiRows.TupleLength(); i++)
                    {
                        UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                        RegionType.Circle,
                                                                        SortAntiRows[i] - WireParameter.DieImageRowOffset,
                                                                        SortAntiCols[i] - WireParameter.DieImageColumnOffset,
                                                                        WireParameter.GenStartBallSize, 0,
                                                                        WireParameter.DieImageRowOffset,
                                                                        WireParameter.DieImageColumnOffset,
                                                                        SortAntiAngles[i]);
                        if (userRegion_Circle == null) return;
                        
                        //1217 修正顺时针排序的初始序号问题
                        if (false)
                        {
                            userRegion_Circle.Index = StartBallAutoUserRegion.Count + 1;
                            userRegion_Circle.Index_ini = StartBallAutoUserRegion.Count + 1;
                            StartBallAutoUserRegion.Add(userRegion_Circle);
                        }
                        else
                        {
                            userRegion_Circle.Index = StartBallAutoUserRegion.Count + 1;
                            userRegion_Circle.Index_ini = sort_ind_ini2[i];
                            StartBallAutoUserRegion.Add(userRegion_Circle);
                        }
                    }
                    break;
                case 7:
                    //1022 手动设置序号排序的 有效性检查
                    bool manual_index_err7 = false;
                    int[] input_index7 = new int[StartBallAutoUserRegion.Count()];
                    for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                    {
                        input_index7[i] = StartBallAutoUserRegion[i].Index;
                    }
                    Array.Sort(input_index7);
                    for (int i = 0; i < StartBallAutoUserRegion.Count() - 1; i++)
                    {
                        if (input_index7[i] != i + 1)
                        {
                            MessageBox.Show(string.Format("起始区域序号{0}不存在，请检查序号设置", i + 1));
                            manual_index_err7 = true;
                            break;
                        }
                        if (input_index7[i] == input_index7[i + 1])
                        {
                            MessageBox.Show(string.Format("起始区域序号{0}有重复，请检查序号设置", i + 1));
                            manual_index_err7 = true;
                            break;
                        }

                    }
                    if (manual_index_err7)
                    {
                        //MessageBox.Show("起始区域手动填写序号有误，请检查");
                        return;
                    }

                    ObservableCollection<UserRegion> StartBallAutoUserRegionSort_7 = new ObservableCollection<UserRegion>(StartBallAutoUserRegion.OrderBy(r => r.Index));


                    StartBallAutoUserRegion.Clear();
                    for (int i = 0; i < StartBallAutoUserRegionSort_7.Count(); i++)
                    {
                        StartBallAutoUserRegion.Add(StartBallAutoUserRegionSort_7[i]);
                    }

                    break;
                default:
                    break;
            }

            //1028 重新排序后，线模板的线序号保持不变
            if (CurrentModelGroup != null)
            {
                CurrentModelGroup.SelectModelNumber = tmp_SelectModelNumber;
            }

            DispalyGroupsStartRegions();

            //1109 add for pickup sort function
            Array.Clear(WireParameter.WireAutoIndex_sorted_start, 0, WireParameter.WireAutoIndex_sorted_start.Length);
            WireParameter.WireAutoIndex_sorted_start = new int[StartBallAutoUserRegion.Count];
            for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
            {
                WireParameter.WireAutoIndex_sorted_start[i] = StartBallAutoUserRegion[i].Index_ini;
            }

            //1111
            if (CurrentModelGroup != null && ModelGroups.Count > 0 && WireParameter.WireRegModelType.Length == StartBallAutoUserRegion.Count())
            {
                for (int i = 0; i < StartBallAutoUserRegion.Count; i++)
                {
                    StartBallAutoUserRegion[i].ModelGroups = ModelGroups;
                    StartBallAutoUserRegion[i].CurrentModelGroup = ModelGroups.ElementAt(WireParameter.WireRegModelType[i] - 1);

                }
            }

            //1012
            if (StartStopLineAutoUserRegion.Count() == 0)  // mod
            {
                return;
            }
            else
            {
                StartStopLineAutoUserRegion.Clear();
                if (StopBallAutoUserRegion.Count() != StartBallAutoUserRegion.Count())
                {
                    MessageBox.Show("起始焊点数目与结束焊点数量不一致，请确保焊点数目对应");
                }
                else  //一二焊点数量对应相等时，生成金线区域
                {
                    HOperatorSet.GenEmptyObj(out HObject concatBondWireRegion);

                    //----------获取起始焊点区域
                    HTuple modelStartRow = new HTuple();
                    HTuple modelStartCol = new HTuple();
                    //
                    HObject modelStartBall = new HObject();

                    //----------------------选取结束焊点区域
                    HTuple modelStopRow = new HTuple();
                    HTuple modelStopCol = new HTuple();
                    //
                    HObject modelStopBall = new HObject();

                    HObject modelLine = new HObject();

                    UserRegion userRegion_Line;

                    for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                    {
                        //起始焊点
                        modelStartRow = StartBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset;
                        modelStartCol = StartBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                        //结束焊点
                        modelStopRow = StopBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset;
                        modelStopCol = StopBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                        //生成焊点连线
                        HOperatorSet.GenRegionLine(out modelLine, modelStartRow, modelStartCol, modelStopRow, modelStopCol);

                        userRegion_Line = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                   RegionType.Line,
                                                                   StartBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                                   StartBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                                   StopBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                                   StopBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                                   WireParameter.DieImageRowOffset,
                                                                   WireParameter.DieImageColumnOffset,
                                                                   0);

                        userRegion_Line.Index = StartStopLineAutoUserRegion.Count + 1;
                        StartStopLineAutoUserRegion.Add(userRegion_Line);

                    }

                }
            }

        }
        //起始焊点操作
        private void ExecuteIsCheckStartCommand(object parameter)
        {
            if (StartBallAutoUserRegion.All(x => x.IsSelected == true))
            { IsStartBondCheckAll = true; }
            else if (StartBallAutoUserRegion.All(x => !x.IsSelected))
            { IsStartBondCheckAll = false; }
            else
            { IsStartBondCheckAll = null; }
        }
        private void ExecuteIsCheckAllStartCommand(object parameter)
        {
            if (IsStartBondCheckAll == true)
            { StartBallAutoUserRegion.ToList().ForEach(r => r.IsSelected = true); }
            else if (IsStartBondCheckAll == false)
            { StartBallAutoUserRegion.ToList().ForEach(r => r.IsSelected = false); }
        }
        private void ExecuteBindingWireModelCommand(object parameter)
        {          

            if (CurrentModelGroup != null)
            {
                for (int i = 0; i < StartBallAutoUserRegion.Count; i++)
                {

                    //StartBallAutoUserRegion[i].WireModelindex = WireModelindex;
                    //StartBallAutoUserRegion[i].ModelGroups = ModelGroups;                 

                    if (StartBallAutoUserRegion[i].IsSelected)
                    {
                        StartBallAutoUserRegion[i].CurrentModelGroup = ModelGroups.ElementAt(CurrentModelGroup.Index - 1);

                        WireParameter.WireRegModelType[i] = CurrentModelGroup.Index;

                    }

                }
            }
            IsStartBondCheckAll = null;
        }
        //起始焊点拾取
        public void ExecuteStartPickUpCommand(object parameter)
        {
            if (WireParameter.IsStartPickUp == true)
            {
                WireParameter.IsWirePickUp = false;
                WireParameter.IsWireRegionPickUp = false;

                WireParameter.IsDrawStartVirtualBall = false;
                WireParameter.IsDrawStopVirtualBall = false;
            }
        }
        //起始虚拟焊点开关
        public void ExecuteDrawStartVirtualBallCommand(object parameter)
        {
            if (WireParameter.IsEnableStartVirtualBond == false && WireParameter.IsDrawStartVirtualBall == true)
            {
                WireParameter.IsDrawStartVirtualBall = false;
                MessageBox.Show("请确认是否使用虚拟焊点！");
                return;
            }
            if (WireParameter.IsDrawStartVirtualBall == true)
            {
                MessageBox.Show("请点击【连续画框】模式开始绘制！");

                WireParameter.IsDrawStopVirtualBall = false;

                WireParameter.IsStartPickUp = false;
                WireParameter.IsStopPickUp = false;
                WireParameter.IsWirePickUp = false;
                WireParameter.IsWireRegionPickUp = false;
            }
        }
        #endregion

        //********************************************************************************************************
        #region 金线检测结束焊点自动生成以及排序

        //获取结束焊点属于
        private void ExecuteEndBondOnWhatCommand(object parameter)
        {
            //var OnRecipesResult = EndBondOnRecipes.Where(x => x.IsSelected).ToList();

            //StopRegToWhat = new HTuple();

            //for (int i = 0; i < OnRecipesResult.Count; i++)
            //{
            //    string _stopToWhat = OnRecipesResult.ElementAt(i).Name;
            //    StopRegToWhat = StopRegToWhat.TupleConcat(_stopToWhat);
            //}

            int current_index = -1;
            //赋值上步操作的选中标记
            for (int i = 0; i < EndBondOnRecipes.Count; i++)
            {
                if (EndBondOnRecipes.ElementAt(i).IsSelected_pre != EndBondOnRecipes.ElementAt(i).IsSelected)
                {
                    current_index = i;
                    EndBondOnRecipes.ElementAt(i).IsSelected_pre = EndBondOnRecipes.ElementAt(i).IsSelected;
                    break;
                }

                //EndBondOnRecipes.ElementAt(i).IsSelected_pre = EndBondOnRecipes.ElementAt(i).IsSelected;
            }
            var OnRecipesResult = EndBondOnRecipes.Where(x => x.IsSelected).ToList();

            ObservableCollection<OnRecipe> OnRecipesResult_Sort;

            // Listbox 新单击产生
            if (current_index > -1)
            {
                selectedOnRecipe = EndBondOnRecipes.ElementAt(current_index);

                if (selectedOnRecipe.IsSelected == false)  //当前条目被取消选择，排序号设为-1，并根据其取消的前的序号，调整其它被选择条目的序号
                {
                    //step-1: 调整其它被选择条目的序号，小于当前序号的条目不用被调整，大于当前序号的条目的对应序号 减去 1 
                    for (int i = 0; i < OnRecipesResult.Count; i++)
                    {
                        if (OnRecipesResult.ElementAt(i).Selected_ind > selectedOnRecipe.Selected_ind)
                        {
                            OnRecipesResult.ElementAt(i).Selected_ind--;
                        }
                    }

                    //step-2: 当前条目排序号设为 -1
                    selectedOnRecipe.Selected_ind = 0;
                }
                else                                    //当前条目被选择，其排序号 = 已经有的被选择的条目数+1，
                {
                    selectedOnRecipe.Selected_ind = OnRecipesResult.Count + 0;
                }

                OnRecipesResult = EndBondOnRecipes.Where(x => x.Selected_ind > 0).ToList();  //选取那些序号大于1的条目

                OnRecipesResult_Sort = new ObservableCollection<OnRecipe>(OnRecipesResult.OrderBy(r => r.Selected_ind));

                OnRecipesResult.OrderBy(r => r.Selected_ind);
            }
            else
            {
                return;
            }

            //----------使用排好序的数组
            StopRegToWhat = new HTuple();

            for (int i = 0; i < OnRecipesResult_Sort.Count; i++)
            {
                string _stopToWhat = OnRecipesResult_Sort.ElementAt(i).Name;
                StopRegToWhat = StopRegToWhat.TupleConcat(_stopToWhat);
            }

            WireParameter.StopBondonRecipesIndexs = StopRegToWhat;


        }
        //生成自动结束焊点
        private void ExecuteGenEndBondUserRegionCommand(object parameter)
        {
            //新添加部分
            if (WireParameter.IsEnableEndVirtualBond == true)
            {
                // mod lw 1113
                HOperatorSet.GenEmptyObj(out HObject EndBallRegs);

                if (MessageBox.Show("是否从文件加载虚拟焊点区域？", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                    {
                        ofd.Filter = "reg|*.reg";
                        ofd.Multiselect = false;
                        ofd.InitialDirectory = $"{ModelsFile}FreeRegion";
                        if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                        HOperatorSet.ReadRegion(out EndBallRegs, ofd.FileName);
                    }
                }
                HOperatorSet.MoveRegion(EndBallRegs, out HObject _EndBallRegs, -WireParameter.DieImageRowOffset, -WireParameter.DieImageColumnOffset);
                htWindow.DisplayMultiRegion(_EndBallRegs);
                StopBallAutoUserRegion.Clear();

                for (int i = 0; i < _EndBallRegs.CountObj(); i++)
                {
                    HOperatorSet.AreaCenter(_EndBallRegs.SelectObj(i + 1), out HTuple area, out HTuple row, out HTuple column);
                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                    RegionType.Circle,
                                                                    row,
                                                                    column,
                                                                    WireParameter.GenStopBallSize, 0,
                                                                    WireParameter.DieImageRowOffset,
                                                                    WireParameter.DieImageColumnOffset,
                                                                    0);
                    if (userRegion_Circle == null) return;

                    userRegion_Circle.Index = StopBallAutoUserRegion.Count + 1;
                    userRegion_Circle.Index_ini = StopBallAutoUserRegion.Count + 1;
                    StopBallAutoUserRegion.Add(userRegion_Circle);
                }
            }
            else
            {
                if (IsAutoLoadStopBallReg)
                {
                    StopRegToWhat = WireParameter.StopBondonRecipesIndexs;//1029 读取XML时赋值防呆
                    if (StopRegToWhat == null)
                    {
                        MessageBox.Show("未选定结束焊点归属，请选择！");
                        return;
                    }else if(StopRegToWhat.TupleLength() == 0)
                    {
                        MessageBox.Show("未选定结束焊点归属，请选择！");
                        return;
                    }
                    //生成需要编辑的起始焊点区域
                    HOperatorSet.ReadRegion(out HObject DieRegion, $"{ReferenceDirectory}DieReference.reg");


                    Algorithm.Model_RegionAlg.HTV_Gen_Ball_UserRegion(Algorithm.Region.GetChannnelImageConcact(WireObject.Image),
                                                                      DieRegion, out HObject ho_o_UserBallRegions, StopRegToWhat,
                                                                      ModelsFile,
                                                                      RecipeFile,
                                                                      WireParameter.GenStopBallSize,
                                                                      out stopBondRows,
                                                                      out stopBondCols,
                                                                      out HTuple hv__genBallErrCode,
                                                                      out HTuple hv__genBallErrStr);
                    if ((int)(new HTuple((new HTuple(stopBondRows.TupleLength())).TupleLess(0))) != 0)
                    {
                        MessageBox.Show("自动生成焊点数为0！");
                        return;
                    }
                    StopBallAutoUserRegion.Clear();

                    for (int i = 0; i < stopBondRows.TupleLength(); i++)
                    {
                        UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                        RegionType.Circle,
                                                                        stopBondRows[i] - WireParameter.DieImageRowOffset,
                                                                        stopBondCols[i] - WireParameter.DieImageColumnOffset,
                                                                        WireParameter.GenStopBallSize, 0,
                                                                        WireParameter.DieImageRowOffset,
                                                                        WireParameter.DieImageColumnOffset,
                                                                        0);
                        if (userRegion_Circle == null) return;

                        userRegion_Circle.Index = StopBallAutoUserRegion.Count + 1;
                        userRegion_Circle.Index_ini = StopBallAutoUserRegion.Count + 1;
                        StopBallAutoUserRegion.Add(userRegion_Circle);
                    }
                    DispalyGroupsStopRegions();

                    //1109 add for pickup sort function
                    if (StopBallAutoUserRegion.Count() != StartBallAutoUserRegion.Count())
                    {
                        MessageBox.Show("起始焊点数目与结束焊点数量不一致，请确保焊点数目对应");
                    }
                    else
                    {                 
                        Array.Clear(WireParameter.WireAutoIndex_sorted_stop, 0, WireParameter.WireAutoIndex_sorted_stop.Length);
                        WireParameter.WireAutoIndex_sorted_stop = new int[StopBallAutoUserRegion.Count];
                        for (int i = 0; i < StopBallAutoUserRegion.Count(); i++)
                        {
                            WireParameter.WireAutoIndex_sorted_stop[i] = i + 1;
                        }
                    }

                    //1012
                    if (StartStopLineAutoUserRegion == null || StartBallAutoUserRegion.Count()== 0)
                    {
                        MessageBox.Show("请确保起始焊点先被创建");//1109
                        return;
                    }
                    else
                    {
                        StartStopLineAutoUserRegion.Clear();
                        if (StopBallAutoUserRegion.Count() != StartBallAutoUserRegion.Count())
                        {
                            MessageBox.Show("起始焊点数目与结束焊点数量不一致，请确保焊点数目对应");
                        }
                        else  //一二焊点数量对应相等时，生成金线区域
                        {
                            HOperatorSet.GenEmptyObj(out HObject concatBondWireRegion);

                            //----------获取起始焊点区域
                            HTuple modelStartRow = new HTuple();
                            HTuple modelStartCol = new HTuple();
                            //
                            HObject modelStartBall = new HObject();

                            //----------------------选取结束焊点区域
                            HTuple modelStopRow = new HTuple();
                            HTuple modelStopCol = new HTuple();
                            //
                            HObject modelStopBall = new HObject();

                            HObject modelLine = new HObject();

                            UserRegion userRegion_Line;

                            for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                            {
                                //起始焊点
                                modelStartRow = StartBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset;
                                modelStartCol = StartBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                                //结束焊点
                                modelStopRow = StopBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset;
                                modelStopCol = StopBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                                //生成焊点连线
                                HOperatorSet.GenRegionLine(out modelLine, modelStartRow, modelStartCol, modelStopRow, modelStopCol);

                                userRegion_Line = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                           RegionType.Line,
                                                                           StartBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                                           StartBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                                           StopBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                                           StopBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                                           WireParameter.DieImageRowOffset,
                                                                           WireParameter.DieImageColumnOffset,
                                                                           0);

                                userRegion_Line.Index = StartStopLineAutoUserRegion.Count + 1;
                                StartStopLineAutoUserRegion.Add(userRegion_Line);

                            }

                        }
                    }
               
                   
                    //htWindow.DisplayMultiRegionWithIndex(StopBallAutoUserRegion);

                }
                else
                {
                    MessageBox.Show("请手动画金线检测结束焊点虚拟区域！");
                    return;
                }

            }
        }

        private void ExecuteDisplayAutoStopBondRegionsCommand(object parameter)
        {
            DispalyGroupsStopRegions();
        }

        //public void GetClickDownPointsFromStopBall()
        //{
        //    if (StopBallAutoUserRegion.Count == 0 || WireParameter.IsStopPickUp == false) return;

        //    HOperatorSet.GenRegionPoints(out HObject Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
        //    foreach (var item in StopBallAutoUserRegion)
        //    {
        //        HOperatorSet.SelectShapeProto(item.DisplayRegion, Point, out HObject overlapRegion, "overlaps_abs", 1.0, 5.0);
        //        if (overlapRegion.CountObj() > 0)
        //        {
        //            if (item.IsSelected == false)
        //            {
        //                htWindow.hTWindow.HalconWindow.SetColor("yellow");
        //                item.IsSelected = true;
        //                htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
        //            }
        //            else
        //            {
        //                htWindow.hTWindow.HalconWindow.SetColor("green");
        //                item.IsSelected = false;
        //                htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
        //            }
        //        }
        //    }
        //}

        public void DispalyGroupsStopRegions(bool isHTWindowRegion = true)
        {
            //1028
            if (htWindow.hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || htWindow.isdeleted == true) return;

            if (StopBallAutoUserRegion.Count == 0)
            {
                htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex), true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            foreach (var item in StopBallAutoUserRegion)
            {
                HOperatorSet.ConcatObj(concatGroupRegion, item.DisplayRegion, out concatGroupRegion);
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                htWindow.DisplaySingleRegion(concatGroupRegion, Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex));
            }
            HTuple Length1 = 10;
            foreach (var item in StopBallAutoUserRegion)
            {
                HOperatorSet.TupleSin(item.RegionParameters[2], out HTuple sin_out_line);
                HTuple line_r = item.RegionParameters[0] - WireParameter.DieImageRowOffset - Length1 * sin_out_line;
                HOperatorSet.TupleCos(item.RegionParameters[2], out HTuple cos_out_line);
                HTuple line_c = item.RegionParameters[1] - WireParameter.DieImageColumnOffset + Length1 * cos_out_line;

                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, line_r - 20, line_c + 5);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }

        public void DispalyGroupsStartandStopRegions(bool isHTWindowRegion = true)
        {
            //1028
            if (htWindow.hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || htWindow.isdeleted == true) return;

            if (StopBallAutoUserRegion.Count == 0)
            {
                //htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex), true);
                //return;
            }

            // 如果有起始结束焊点且无连线，则生成焊点连线  add_lw
            if (StartStopLineAutoUserRegion.Count() == 0 && StartBallAutoUserRegion.Count() > 0 && StopBallAutoUserRegion.Count() == StartBallAutoUserRegion.Count()) 
            {
                GenStartStopLineAutoUserRegion();
                //UpdateStartandStopLineRegions();
            }

            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);

            //1-起始焊点圆
            foreach (var item in StartBallAutoUserRegion)
            {
                HOperatorSet.ConcatObj(concatGroupRegion, item.DisplayRegion, out concatGroupRegion);
            }
            //2-结束焊点圆
            foreach (var item in StopBallAutoUserRegion)
            {
                HOperatorSet.ConcatObj(concatGroupRegion, item.DisplayRegion, out concatGroupRegion);
            }
            //3-起始结束焊点之间的连线
            foreach (var item in StartStopLineAutoUserRegion)
            {
                HOperatorSet.ConcatObj(concatGroupRegion, item.DisplayRegion, out concatGroupRegion);
            }

            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                htWindow.DisplaySingleRegion(concatGroupRegion, Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex));
            }

            //序号显示
            HTuple Length1 = 10;
            foreach (var item in StartBallAutoUserRegion)  // 起始焊点序号
            {
                HOperatorSet.TupleSin(item.RegionParameters[2], out HTuple sin_out_line);
                HTuple line_r = item.RegionParameters[0] - WireParameter.DieImageRowOffset - Length1 * sin_out_line;
                HOperatorSet.TupleCos(item.RegionParameters[2], out HTuple cos_out_line);
                HTuple line_c = item.RegionParameters[1] - WireParameter.DieImageColumnOffset + Length1 * cos_out_line;

                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, line_r - 20, line_c + 5);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }  
            foreach (var item in StopBallAutoUserRegion)   // 结束焊点序号
            {
                HOperatorSet.TupleSin(item.RegionParameters[2], out HTuple sin_out_line);
                HTuple line_r = item.RegionParameters[0] - WireParameter.DieImageRowOffset - Length1 * sin_out_line;
                HOperatorSet.TupleCos(item.RegionParameters[2], out HTuple cos_out_line);
                HTuple line_c = item.RegionParameters[1] - WireParameter.DieImageColumnOffset + Length1 * cos_out_line;

                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, line_r - 20, line_c + 5);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
            foreach (var item in StartStopLineAutoUserRegion)   // 焊点连线序号
            {
                HOperatorSet.TupleSin(item.RegionParameters[2], out HTuple sin_out_line);
                HTuple line_r = (item.RegionParameters[0] + item.RegionParameters[2] - 2 * WireParameter.DieImageRowOffset) / 2  - Length1 * sin_out_line;
                HOperatorSet.TupleCos(item.RegionParameters[2], out HTuple cos_out_line);
                HTuple line_c = (item.RegionParameters[1] + item.RegionParameters[3] - 2 * WireParameter.DieImageColumnOffset) / 2 + Length1 * cos_out_line;

                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, line_r - 20, line_c + 5);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }

        }

        public void UpdateStartandStopLineRegions(bool isHTWindowRegion = true)
        {
            //1028
            if (htWindow.hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || htWindow.isdeleted == true) return;

            //1012
            if (StartStopLineAutoUserRegion == null)
            {
                return;
            }
            else
            {
                StartStopLineAutoUserRegion.Clear();
                if (StopBallAutoUserRegion.Count() != StartBallAutoUserRegion.Count())
                {
                    MessageBox.Show("起始焊点数目与结束焊点数量不一致，请确保焊点数目对应");
                }
                else  //一二焊点数量对应相等时，生成金线区域
                {
                    HOperatorSet.GenEmptyObj(out HObject concatBondWireRegion);

                    //----------获取起始焊点区域
                    HTuple modelStartRow = new HTuple();
                    HTuple modelStartCol = new HTuple();
                    //
                    HObject modelStartBall = new HObject();

                    //----------------------选取结束焊点区域
                    HTuple modelStopRow = new HTuple();
                    HTuple modelStopCol = new HTuple();
                    //
                    HObject modelStopBall = new HObject();

                    HObject modelLine = new HObject();

                    UserRegion userRegion_Line;

                    for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                    {
                        //起始焊点
                        modelStartRow = StartBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset;
                        modelStartCol = StartBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                        //结束焊点
                        modelStopRow = StopBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset;
                        modelStopCol = StopBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                        //生成焊点连线
                        HOperatorSet.GenRegionLine(out modelLine, modelStartRow, modelStartCol, modelStopRow, modelStopCol);

                        userRegion_Line = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                   RegionType.Line,
                                                                   StartBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                                   StartBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                                   StopBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                                   StopBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                                   WireParameter.DieImageRowOffset,
                                                                   WireParameter.DieImageColumnOffset,
                                                                   0);

                        userRegion_Line.Index = StartStopLineAutoUserRegion.Count + 1;
                        StartStopLineAutoUserRegion.Add(userRegion_Line);

                    }

                }
            }
        }

        //手动画金线结束区域Or增加焊点区域
        private void ExecuteAddAutoEndBondUserRegionCommand(object parameter)
        {
            if (isRightClickWire != true) return;
            if (htWindow.RegionType == RegionType.Null)
            {
                System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                         WireParameter.DieImageRowOffset,
                                                         WireParameter.DieImageColumnOffset);
                if (userRegion == null) return;
                userRegion.Index = StopBallAutoUserRegion.Count + 1;
                StopBallAutoUserRegion.Add(userRegion);

                DispalyGroupsStopRegions(true);
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
        //删除金线结束焊点区域
        private void ExecuteRemoveAutoEndBondUserRegionCommand(object parameter)
        {
            if (isRightClickWire != true) return;
            if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < StopBallAutoUserRegion.Count; i++)
                {
                    if (StopBallAutoUserRegion[i].IsSelected)
                    {
                        StopBallAutoUserRegion.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        StopBallAutoUserRegion[i].Index = i + 1;
                    }
                }

                DispalyGroupsStopRegions(false);

                if (StopBallAutoUserRegion.Count == 0)
                {
                    IsStopBondCheckAll = false;
                }
            }

        }
        //修改不合格的结束焊点区域
        private void ExecuteModifyAutoEndBondRegionCommand(object parameter)
        {

            if (isRightClickWire)
            {
                isRightClickWire = false;
                try
                {
                    for (int i = 0; i < StopBallAutoUserRegion.Count; i++)
                    {
                        if (StopBallAutoUserRegion[i].IsSelected)
                        {
                            int IndexIni = StopBallAutoUserRegion[i].Index_ini;
                            switch (StopBallAutoUserRegion[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Line:

                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(StopBallAutoUserRegion[i].RegionParameters[0]),
                                                                  Math.Floor(StopBallAutoUserRegion[i].RegionParameters[1]),
                                                                  Math.Ceiling(StopBallAutoUserRegion[i].RegionParameters[2]),
                                                                  Math.Ceiling(StopBallAutoUserRegion[i].RegionParameters[3]),
                                                                  StopBallAutoUserRegion[i].RegionType,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (StopBallAutoUserRegion[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                                                                  (StopBallAutoUserRegion[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                                                                  (StopBallAutoUserRegion[i].RegionParameters[2] - WireParameter.DieImageRowOffset),
                                                                                                  (StopBallAutoUserRegion[i].RegionParameters[3] - WireParameter.DieImageColumnOffset),
                                                        out HTuple row1_Rectangle,
                                                        out HTuple column1_Rectangle,
                                                        out HTuple row2_Rectangle,
                                                        out HTuple column2_Rectangle);

                                    StopBallAutoUserRegion[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, StopBallAutoUserRegion[i].RegionType, row1_Rectangle, column1_Rectangle,
                                                           row2_Rectangle, column2_Rectangle, WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    StopBallAutoUserRegion[i] = userRegion;
                                    StopBallAutoUserRegion[i].Index = i + 1;
                                    StopBallAutoUserRegion[i].Index_ini = IndexIni;
                                    DispalyGroupsStopRegions(true);
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(StopBallAutoUserRegion[i].RegionParameters[0]),
                                                                  Math.Floor(StopBallAutoUserRegion[i].RegionParameters[1]),
                                                                  Math.Ceiling(StopBallAutoUserRegion[i].RegionParameters[2]),
                                                                  Math.Ceiling(StopBallAutoUserRegion[i].RegionParameters[3]),
                                                                  StopBallAutoUserRegion[i].RegionType,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(StopBallAutoUserRegion[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                                   Math.Floor(StopBallAutoUserRegion[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                                   StopBallAutoUserRegion[i].RegionParameters[2],
                                                                   Math.Ceiling(StopBallAutoUserRegion[i].RegionParameters[3]),
                                                                   Math.Ceiling(StopBallAutoUserRegion[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    StopBallAutoUserRegion[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, StopBallAutoUserRegion[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         WireParameter.DieImageRowOffset,
                                                                                                         WireParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    StopBallAutoUserRegion[i] = userRegion_Rectangle2;
                                    StopBallAutoUserRegion[i].Index = i + 1;
                                    StopBallAutoUserRegion[i].Index_ini = IndexIni;
                                    DispalyGroupsStopRegions(true);
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((StopBallAutoUserRegion[i].RegionParameters[0]),
                                                                  (StopBallAutoUserRegion[i].RegionParameters[1]),
                                                                  (StopBallAutoUserRegion[i].RegionParameters[2]),
                                                                  0,
                                                                  StopBallAutoUserRegion[i].RegionType,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (StopBallAutoUserRegion[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                               (StopBallAutoUserRegion[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                               StopBallAutoUserRegion[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    StopBallAutoUserRegion[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     StopBallAutoUserRegion[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     WireParameter.DieImageRowOffset,
                                                                                                     WireParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    StopBallAutoUserRegion[i] = userRegion_Circle;
                                    StopBallAutoUserRegion[i].Index = i + 1;
                                    StopBallAutoUserRegion[i].Index_ini = IndexIni;
                                    DispalyGroupsStopRegions(true);
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
                    MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    isRightClickWire = true;
                }
            }
            //重新生成起始--结束焊点之间的金线并显示
            if (StartBallAutoUserRegion.Count() > 0 && StopBallAutoUserRegion.Count() == StartBallAutoUserRegion.Count())  //如果有结束焊点，则重新生成焊点连线
            {
                GenStartStopLineAutoUserRegion();

                UpdateStartandStopLineRegions();

            }
            //UpdateStartandStopLineRegions();

        }

        private void ExecuteStopBallSortCommand(object parameter)
        {
            if (StopBallAutoUserRegion.Count() == 0) return;

            switch (WireParameter.StopSortMethod)
            {
                case 0:
                    ObservableCollection<UserRegion> StopBallAutoUserRegionSort_0 = new ObservableCollection<UserRegion>(StopBallAutoUserRegion.OrderBy(r => r.Index));
                    StopBallAutoUserRegion.Clear();
                    for (int i = 0; i < StopBallAutoUserRegionSort_0.Count(); i++)
                    {
                        StopBallAutoUserRegion.Add(StopBallAutoUserRegionSort_0[i]);
                    }
                    break;

                case 1:
                    //列升序
                    ObservableCollection<UserRegion> StopBallAutoUserRegionSort_1 = new ObservableCollection<UserRegion>(StopBallAutoUserRegion.OrderBy(r => r.RegionParameters[1]));
                    StopBallAutoUserRegion.Clear();
                    for (int i = 0; i < StopBallAutoUserRegionSort_1.Count(); i++)
                    {
                        StopBallAutoUserRegion.Add(StopBallAutoUserRegionSort_1[i]);
                        StopBallAutoUserRegionSort_1[i].Index = StopBallAutoUserRegion.Count;
                    }
                    break;

                case 2:
                    //列降序
                    ObservableCollection<UserRegion> StopBallAutoUserRegionSort_2 = new ObservableCollection<UserRegion>(StopBallAutoUserRegion.OrderByDescending(r => r.RegionParameters[1]));
                    StopBallAutoUserRegion.Clear();
                    for (int i = 0; i < StopBallAutoUserRegionSort_2.Count(); i++)
                    {
                        StopBallAutoUserRegion.Add(StopBallAutoUserRegionSort_2[i]);
                        StopBallAutoUserRegionSort_2[i].Index = StopBallAutoUserRegion.Count;
                    }
                    break;

                case 3:
                    //行升序
                    ObservableCollection<UserRegion> StopBallAutoUserRegionSort_3 = new ObservableCollection<UserRegion>(StopBallAutoUserRegion.OrderBy(r => r.RegionParameters[0]));
                    StopBallAutoUserRegion.Clear();
                    for (int i = 0; i < StopBallAutoUserRegionSort_3.Count(); i++)
                    {
                        StopBallAutoUserRegion.Add(StopBallAutoUserRegionSort_3[i]);
                        StopBallAutoUserRegionSort_3[i].Index = StopBallAutoUserRegion.Count;
                    }
                    break;

                case 4:
                    //行降序
                    ObservableCollection<UserRegion> StopBallAutoUserRegionSort_4 = new ObservableCollection<UserRegion>(StopBallAutoUserRegion.OrderByDescending(r => r.RegionParameters[0]));
                    StopBallAutoUserRegion.Clear();
                    for (int i = 0; i < StopBallAutoUserRegionSort_4.Count(); i++)
                    {
                        StopBallAutoUserRegion.Add(StopBallAutoUserRegionSort_4[i]);
                        StopBallAutoUserRegionSort_4[i].Index = StopBallAutoUserRegion.Count;
                    }
                    break;

                case 5:
                    //顺时针
                    if (WireParameter.StopFirstSortNumber == -1)
                    {
                        MessageBox.Show("请选择排序起始焊点！");
                        return;
                    }
                    HTuple BondRows = new HTuple();
                    HTuple BondCols = new HTuple();
                    HTuple BondAngles = new HTuple();

                    foreach (var item in StopBallAutoUserRegion)
                    {
                        BondRows = BondRows.TupleConcat(item.RegionParameters[0]);
                        BondCols = BondCols.TupleConcat(item.RegionParameters[1]);
                        // BondAngles = BondAngles.TupleConcat(item.RegionParameters[2]);
                    }
                    //生成固定值的角度
                    HOperatorSet.TupleGenConst(new HTuple(BondRows.TupleLength()), 0, out BondAngles);

                    Algorithm.Model_RegionAlg.HTV_Clockwise_Sort_Points_Angles(WireParameter.CoreRow,
                                    WireParameter.CoreColumn,
                                    BondRows,
                                    BondCols,
                                    BondAngles,
                                    WireParameter.StopFirstSortNumber,
                                    0,
                                    out HTuple SortRows,
                                    out HTuple SortCols,
                                    out HTuple SortAngles,
                                    out HTuple SortIndex);

                    //1217 start
                    HTuple sort_ind_ini = new HTuple();
                    HOperatorSet.TupleGenConst(new HTuple(BondRows.TupleLength()), 0, out sort_ind_ini);//1217
                    for (int i = 0; i < SortRows.TupleLength(); i++)
                    {
                        HTuple cur_sort_ind;
                        cur_sort_ind = SortIndex[i];
                        sort_ind_ini[i] = StopBallAutoUserRegion[cur_sort_ind].Index_ini + 0;
                    }
                    //1217 stop

                    StopBallAutoUserRegion.Clear();

                    for (int i = 0; i < SortRows.TupleLength(); i++)
                    {
                        UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                        RegionType.Circle,
                                                                        SortRows[i] - WireParameter.DieImageRowOffset,
                                                                        SortCols[i] - WireParameter.DieImageColumnOffset,
                                                                        WireParameter.GenStartBallSize, 0,
                                                                        WireParameter.DieImageRowOffset,
                                                                        WireParameter.DieImageColumnOffset,
                                                                        SortAngles[i]);

                        if (userRegion_Circle == null) return;

                        //1217 修正顺时针排序的初始序号问题
                        if (false)
                        {
                            userRegion_Circle.Index = StopBallAutoUserRegion.Count + 1;
                            userRegion_Circle.Index_ini = StopBallAutoUserRegion.Count + 1;
                            StopBallAutoUserRegion.Add(userRegion_Circle);
                        }
                        else
                        {
                            userRegion_Circle.Index = StopBallAutoUserRegion.Count + 1;
                            userRegion_Circle.Index_ini = sort_ind_ini[i];
                            StopBallAutoUserRegion.Add(userRegion_Circle);
                        }
                    }
                    break;

                case 6:
                    //逆时针
                    if (WireParameter.StopFirstSortNumber == -1)
                    {
                        MessageBox.Show("请选择排序起始焊点！");
                        return;
                    }
                    HTuple AntiBondRows = new HTuple();
                    HTuple AntiBondCols = new HTuple();
                    HTuple AntiBondAngles = new HTuple();

                    foreach (var item in StopBallAutoUserRegion)
                    {
                        AntiBondRows = AntiBondRows.TupleConcat(item.RegionParameters[0]);
                        AntiBondCols = AntiBondCols.TupleConcat(item.RegionParameters[1]);
                    }
                    //生成固定值的角度
                    HOperatorSet.TupleGenConst(new HTuple(AntiBondRows.TupleLength()), 0, out AntiBondAngles);

                    Algorithm.Model_RegionAlg.HTV_Clockwise_Sort_Points_Angles(WireParameter.CoreRow,
                                    WireParameter.CoreColumn,
                                    AntiBondRows,
                                    AntiBondCols,
                                    AntiBondAngles,
                                    WireParameter.StopFirstSortNumber,
                                    1,
                                    out HTuple SortAntiRows,
                                    out HTuple SortAntiCols,
                                    out HTuple SortAntiAngles,
                                    out HTuple SortAntiIndex);

                    //1217 start
                    HTuple sort_ind_ini2 = new HTuple();
                    HOperatorSet.TupleGenConst(new HTuple(AntiBondRows.TupleLength()), 0, out sort_ind_ini);//1217
                    for (int i = 0; i < AntiBondRows.TupleLength(); i++)
                    {
                        HTuple cur_sort_ind;
                        cur_sort_ind = SortAntiIndex[i];
                        sort_ind_ini[i] = StopBallAutoUserRegion[cur_sort_ind].Index_ini + 0;
                    }
                    //1217 stop

                    StopBallAutoUserRegion.Clear();

                    for (int i = 0; i < SortAntiRows.TupleLength(); i++)
                    {
                        UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                        RegionType.Circle,
                                                                        SortAntiRows[i] - WireParameter.DieImageRowOffset,
                                                                        SortAntiCols[i] - WireParameter.DieImageColumnOffset,
                                                                        WireParameter.GenStartBallSize, 0,
                                                                        WireParameter.DieImageRowOffset,
                                                                        WireParameter.DieImageColumnOffset,
                                                                        SortAntiAngles[i]);
                        if (userRegion_Circle == null) return;

                        //1217 修正顺时针排序的初始序号问题
                        if (false)
                        {
                            userRegion_Circle.Index = StopBallAutoUserRegion.Count + 1;
                            userRegion_Circle.Index_ini = StopBallAutoUserRegion.Count + 1;
                            StopBallAutoUserRegion.Add(userRegion_Circle);
                        }
                        else
                        {
                            userRegion_Circle.Index = StopBallAutoUserRegion.Count + 1;
                            userRegion_Circle.Index_ini = sort_ind_ini2[i];
                            StopBallAutoUserRegion.Add(userRegion_Circle);
                        }
                    }
                    break;
                case 7:
                    //1022 手动设置序号排序的 有效性检查
                    bool manual_index_err7 = false;
                    int[] input_index7 = new int[StopBallAutoUserRegion.Count()];
                    for (int i = 0; i < StopBallAutoUserRegion.Count(); i++)
                    {
                        input_index7[i] = StopBallAutoUserRegion[i].Index;
                    }
                    Array.Sort(input_index7);
                    for (int i = 0; i < StopBallAutoUserRegion.Count() - 1; i++)
                    {
                        if (input_index7[i] != i + 1)
                        {
                            MessageBox.Show(string.Format("起始区域序号{0}不存在，请检查序号设置", i + 1));
                            manual_index_err7 = true;
                            break;
                        }
                        if (input_index7[i] == input_index7[i + 1])
                        {
                            MessageBox.Show(string.Format("起始区域序号{0}有重复，请检查序号设置", i + 1));
                            manual_index_err7 = true;
                            break;
                        }

                    }
                    if (manual_index_err7)
                    {
                        //MessageBox.Show("起始区域手动填写序号有误，请检查");
                        return;
                    }

                    ObservableCollection<UserRegion> StopBallAutoUserRegionSort_7 = new ObservableCollection<UserRegion>(StopBallAutoUserRegion.OrderBy(r => r.Index));


                    StopBallAutoUserRegion.Clear();
                    for (int i = 0; i < StopBallAutoUserRegionSort_7.Count(); i++)
                    {
                        StopBallAutoUserRegion.Add(StopBallAutoUserRegionSort_7[i]);
                    }

                    break;
                default:
                    break;
            }
            DispalyGroupsStopRegions();

            WireParameter.StopFirstSortNumber = 0;

            //1109 add for pickup sort function
            if (StopBallAutoUserRegion.Count() != StartBallAutoUserRegion.Count())
            {
                MessageBox.Show("起始焊点数目与结束焊点数量不一致，请确保焊点数目对应");
            }
            else
            {
                Array.Clear(WireParameter.WireAutoIndex_sorted_stop, 0, WireParameter.WireAutoIndex_sorted_stop.Length);
                WireParameter.WireAutoIndex_sorted_stop = new int[StopBallAutoUserRegion.Count];
                for (int i = 0; i < StopBallAutoUserRegion.Count(); i++)
                {
                    WireParameter.WireAutoIndex_sorted_stop[i] = StopBallAutoUserRegion[i].Index_ini;
                }
            }

            //1012
            if (StartStopLineAutoUserRegion == null)
            {
                return;
            }
            else
            {
                StartStopLineAutoUserRegion.Clear();
                if (StopBallAutoUserRegion.Count() != StartBallAutoUserRegion.Count())
                {
                    MessageBox.Show("起始焊点数目与结束焊点数量不一致，请确保焊点数目对应");
                }
                else  //一二焊点数量对应相等时，生成金线区域
                {
                    HOperatorSet.GenEmptyObj(out HObject concatBondWireRegion);

                    //----------获取起始焊点区域
                    HTuple modelStartRow = new HTuple();
                    HTuple modelStartCol = new HTuple();
                    //
                    HObject modelStartBall = new HObject();

                    //----------------------选取结束焊点区域
                    HTuple modelStopRow = new HTuple();
                    HTuple modelStopCol = new HTuple();
                    //
                    HObject modelStopBall = new HObject();

                    HObject modelLine = new HObject();

                    UserRegion userRegion_Line;

                    for (int i = 0; i < StartBallAutoUserRegion.Count(); i++)
                    {
                        //起始焊点
                        modelStartRow = StartBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset;
                        modelStartCol = StartBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                        //结束焊点
                        modelStopRow = StopBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset;
                        modelStopCol = StopBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                        //生成焊点连线
                        HOperatorSet.GenRegionLine(out modelLine, modelStartRow, modelStartCol, modelStopRow, modelStopCol);

                        userRegion_Line = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                   RegionType.Line,
                                                                   StartBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                                   StartBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                                   StopBallAutoUserRegion.ElementAt(i).RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                                   StopBallAutoUserRegion.ElementAt(i).RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                                   WireParameter.DieImageRowOffset,
                                                                   WireParameter.DieImageColumnOffset,
                                                                   0);

                        userRegion_Line.Index = StartStopLineAutoUserRegion.Count + 1;
                        StartStopLineAutoUserRegion.Add(userRegion_Line);

                    }

                }
            }
        }
        //起始焊点操作
        private void ExecuteIsCheckStopCommand(object parameter)
        {
            if (StopBallAutoUserRegion.All(x => x.IsSelected == true))
            { IsStopBondCheckAll = true; }
            else if (StopBallAutoUserRegion.All(x => !x.IsSelected))
            { IsStopBondCheckAll = false; }
            else
            { IsStopBondCheckAll = null; }
        }

        private void ExecuteIsCheckAllStopCommand(object parameter)
        {
            if (IsStopBondCheckAll == true)
            { StopBallAutoUserRegion.ToList().ForEach(r => r.IsSelected = true); }
            else if (IsStopBondCheckAll == false)
            { StopBallAutoUserRegion.ToList().ForEach(r => r.IsSelected = false); }
        }
        //显示所有区域
        private void ExecuteDisplayAllBondRegionCommand(object parameter)
        {
            // add lw 1119
            ExecuteStopBallSortCommand(null);

            DispalyGroupsStartandStopRegions();
            //UpdateModelIndexChange();

            WireParameter.IsDrawStartVirtualBall = false;
            WireParameter.IsDrawStopVirtualBall = false;
        }
        //结束焊点拾取
        public void ExecuteStopPickUpCommand(object parameter)
        {
            if (WireParameter.IsStopPickUp == true)
            {
                WireParameter.IsWirePickUp = false;
                WireParameter.IsWireRegionPickUp = false;

                WireParameter.IsDrawStartVirtualBall = false;
                WireParameter.IsDrawStopVirtualBall = false;
            }
        }
        //结束虚拟焊点开关
        public void ExecuteDrawStopVirtualBallCommand(object parameter)
        {
            if (WireParameter.IsEnableEndVirtualBond == false && WireParameter.IsDrawStopVirtualBall == true)
            {
                WireParameter.IsDrawStopVirtualBall = false;
                MessageBox.Show("请确认是否使用虚拟焊点！");
                return;
            }
            if (WireParameter.IsDrawStopVirtualBall == true)
            {
                MessageBox.Show("请点击【连续画框】模式开始绘制！");

                WireParameter.IsDrawStartVirtualBall = false;

                WireParameter.IsStartPickUp = false;
                WireParameter.IsStopPickUp = false;
                WireParameter.IsWirePickUp = false;
                WireParameter.IsWireRegionPickUp = false;
            }
        }
        #endregion

        //****************************************************************************************************
        #region 自动生成金线检测区域模板

        private void ExecuteAddModelGroupCommand(object parameter)
        {
            if (WireAddAutoRegion.isRightClickWire != true) return;

            CurrentModelGroup = new WireAutoRegionGroup
            {
                Index = ModelGroups.Count + 1
            };
            ModelGroups.Add(CurrentModelGroup);
            ModelGroupsCount = ModelGroups.Count;
            //DispalyModelGroupRegion();
            UpdateModelIndexChange();

            MessageBox.Show($"新建了序号 {CurrentModelGroup.Index.ToString()} 的焊点金线模板组合");

            //WireModelindex.Add(ModelGroups.Count + 0);

            //set the modelwire=1 for each each line
            if (ModelGroups.Count() == 1)
            {
                Array.Clear(WireParameter.WireRegModelType, 0, WireParameter.WireRegModelType.Length);
                WireParameter.WireRegModelType = new int[StartBallAutoUserRegion.Count];

                for (int i = 0; i < StartBallAutoUserRegion.Count; i++)
                {
                    StartBallAutoUserRegion[i].ModelGroups = ModelGroups;
                    WireParameter.WireRegModelType[i] = CurrentModelGroup.Index;
                    StartBallAutoUserRegion[i].CurrentModelGroup = ModelGroups.ElementAt(ModelGroups.Count - 1);
                }
            }
            

        }

        private void ExecuteRemoveModelGroupCommand(object parameter)
        {
            if (WireAddAutoRegion.isRightClickWire != true) return;
            if (CurrentModelGroup == null) return;
            if (MessageBox.Show($"是否删除序号 {CurrentModelGroup.Index.ToString()} 的焊点金线模板组合", "删除", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;
            for (int i = CurrentModelGroup.Index; i < ModelGroups.Count; i++)
            {
                if (ModelGroups[i].Index > i)
                {
                    ModelGroups[i].Index--;
                }
                
            }
            int tmp_index;
            tmp_index = CurrentModelGroup.Index;
            ModelGroups.Remove(CurrentModelGroup);
            

            ModelGroupsCount = ModelGroups.Count;
            CurrentModelGroup = null;
            //DispalyModelGroupRegion();
            UpdateModelIndexChange();

            //update the modelwire when some modelwire been deleted
            for (int i = 0; i < StartBallAutoUserRegion.Count; i++)
            {

                StartBallAutoUserRegion[i].ModelGroups = ModelGroups;

                if (StartBallAutoUserRegion[i].CurrentModelGroup != null)
                { 
                    if(StartBallAutoUserRegion[i].CurrentModelGroup.Index == tmp_index)
                    {
                        StartBallAutoUserRegion[i].CurrentModelGroup = null;
                    }
                }

             }


        }

        //金线拾取功能
        public void GetClickDownPointsFromStartStopLine()
        {
            if (StartStopLineAutoUserRegion.Count == 0 || WireParameter.IsWireRegionPickUp == false) return;
            //add by wj 防呆
            if (CurrentModelGroup == null)
            {
                MessageBox.Show("请点击【添加焊点金线组+】开始创建金线检测区域模板！");
                return;
            }
            HOperatorSet.GenRegionPoints(out HObject Point1, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);

            HTuple dia_r = 10;
            HOperatorSet.DilationCircle(Point1, out HObject Point, dia_r);

            HOperatorSet.AreaCenter(Point1, out HTuple a1, out HTuple r1, out HTuple c1);
            HOperatorSet.AreaCenter(Point, out HTuple a, out HTuple r, out HTuple c);

            int line_ind = 0;
            foreach (var item in StartStopLineAutoUserRegion)
            {


                HOperatorSet.SelectShapeProto(item.DisplayRegion, Point, out HObject overlapRegion, "overlaps_abs", 1.0, 150.0);
                if (overlapRegion.CountObj() > 0)
                {
                    if (item.IsSelected == false)
                    {
                        htWindow.hTWindow.HalconWindow.SetColor("yellow");
                        item.IsSelected = true;
                        htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
                        CurrentModelGroup.SelectModelNumber = line_ind;
                    }
                    else
                    {
                        htWindow.hTWindow.HalconWindow.SetColor("green");
                        item.IsSelected = false;
                        htWindow.hTWindow.HalconWindow.DispObj(item.DisplayRegion);
                    }
                }

                line_ind++;

            }
        }

        //金线检测区域内拾取功能
        public void GetClickDownPointsFromLineModelRegion()
        {
            if (CurrentModelGroup == null)
            {
                MessageBox.Show("请点击【添加焊点金线组+】开始创建金线检测区域模板！");
                return;
            }
            if (currentModelGroup.LineModelRegions.Count == 0 || WireParameter.IsWirePickUp == false) return;

            HOperatorSet.GenRegionPoints(out HObject Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
            foreach (var item in currentModelGroup.LineModelRegions)
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

        public void ExecuteWireRegionPickUpCommand(object parameter)
        {
            if (WireParameter.IsWireRegionPickUp == true)
            {
                WireParameter.IsStartPickUp = false;
                WireParameter.IsStopPickUp = false;
                WireParameter.IsWirePickUp = false;
            }
            else
            {
                WireParameter.IsStartPickUp = true;
                WireParameter.IsStopPickUp = true;
            }
        }

        public void ExecuteWirePickUpCommand(object parameter)
        {
            if (WireParameter.IsWirePickUp == true)
            {
                WireParameter.IsStartPickUp = false;
                WireParameter.IsStopPickUp = false;
                WireParameter.IsWireRegionPickUp = false;
            }
            else
            {
                WireParameter.IsStartPickUp = true;
                WireParameter.IsStopPickUp = true;
            }
        }

        //选取创建金线模板的索引号
        private void ExecuteSelectionChangedModelIndexCommand(object obj)
        {
            //显示所有焊点连线
            //DispalyGroupsStartandStopRegions();
            UpdateModelIndexChange();
            return;
        }
        public void UpdateModelIndexChange()
        {
            //1028
            if (htWindow.hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || htWindow.isdeleted == true) return;

            //显示所有焊点连线
            DispalyGroupsStartandStopRegions();

            int SelectIndex;
            SelectIndex = 0;

            //没有任何模板线，没有当前模板线
            if (CurrentModelGroup == null && ModelGroups.Count < 1)
            {
                //htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex), false);
                return;
            }
            //有模板线，但没有当前模板线，此时是在删除某模板时发生，重置为第一条模板线
            if (CurrentModelGroup == null && ModelGroups.Count > 0)
            {
                SelectIndex = 1;
                CurrentModelGroup = ModelGroups.ElementAt(0);
            }
            //有模板线，有当前模板线，但是模板没有选择线的来源，此情况发生在新建模板线时
            if (CurrentModelGroup != null)
            {
                if (CurrentModelGroup.SelectModelNumber == -1)
                {
                    //htWindow.Display(WireObject.DieImage, true);
                    return;
                }
            }

            //将选定的索引号+1
            if (CurrentModelGroup != null)
            {
                SelectIndex = CurrentModelGroup.SelectModelNumber + 1;
            }


            HOperatorSet.GenEmptyObj(out HObject concatBondWireRegion);

            //----------获取起始焊点区域
            HTuple modelStartRow = new HTuple();
            HTuple modelStartCol = new HTuple();
            //
            HObject modelStartBall = new HObject();

            UserRegion UserregionStartBall = new UserRegion();

            HOperatorSet.GenEmptyObj(out HObject concatSelectionRegion);


            foreach (var item in StartBallAutoUserRegion)
            {
                if (item.Index == SelectIndex)
                {
                    modelStartRow = item.RegionParameters[0] - WireParameter.DieImageRowOffset;
                    modelStartCol = item.RegionParameters[1] - WireParameter.DieImageColumnOffset;
                    //生成圆
                    HOperatorSet.GenCircle(out modelStartBall, modelStartRow, modelStartCol, WireParameter.GenStartBallSize);

                    UserregionStartBall = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Circle, modelStartRow, modelStartCol,
                                                                 WireParameter.GenStartBallSize, 0,
                                                                 WireParameter.DieImageRowOffset,
                                                                 WireParameter.DieImageColumnOffset,
                                                                 0);
                    if (modelStartBall == null) return;
                    CurrentModelGroup.ModelStartUserRegions = UserregionStartBall;
                    //break;
                }
                if (item.IsSelected)
                {
                    HOperatorSet.ConcatObj(concatSelectionRegion, item.DisplayRegion, out concatSelectionRegion);
                }

            }

            //1109 add for stop region selection display
            foreach (var item in StopBallAutoUserRegion)
            {               
                if (item.IsSelected)
                {
                    HOperatorSet.ConcatObj(concatSelectionRegion, item.DisplayRegion, out concatSelectionRegion);
                }
            }
            //显示选中区域
            htWindow.InitialHWindow("yellow");
            htWindow.hTWindow.HalconWindow.DispObj(concatSelectionRegion);

            //----------------------选取结束焊点区域

            //----------------------选取结束焊点区域
            HTuple modelStopRow = new HTuple();
            HTuple modelStopCol = new HTuple();
            HObject modelStopBall = new HObject();
            UserRegion UserregionEndBall = new UserRegion();

            if (SelectIndex > 0)
            {
                modelStopRow = StopBallAutoUserRegion.ElementAt(SelectIndex-1).RegionParameters[0] - WireParameter.DieImageRowOffset;
                modelStopCol = StopBallAutoUserRegion.ElementAt(SelectIndex - 1).RegionParameters[1] - WireParameter.DieImageColumnOffset;
                //生成圆
                HOperatorSet.GenCircle(out modelStopBall, modelStopRow, modelStopCol, WireParameter.GenStopBallSize);
                UserregionEndBall = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Circle, modelStopRow, modelStopCol,
                                                                 WireParameter.GenStartBallSize, 0,
                                                                 WireParameter.DieImageRowOffset,
                                                                 WireParameter.DieImageColumnOffset,
                                                                 0);
                if (modelStartBall == null) return;
                CurrentModelGroup.ModelStopUserRegions = UserregionEndBall;
            }


            HOperatorSet.ConcatObj(modelStartBall, modelStopBall, out concatBondWireRegion);
            //生成直线
            HOperatorSet.GenRegionLine(out HObject modelLine, modelStartRow, modelStartCol, modelStopRow, modelStopCol);
            //将起始焊点，结束焊点，两点之间直线显示
            HOperatorSet.ConcatObj(concatBondWireRegion, modelLine, out concatBondWireRegion);


            //生成起始焊点，结束焊点之间的连线
            UserRegion UserregionmodelLine = new UserRegion(); ;
            UserregionmodelLine = UserRegion.GetHWindowRegionUpdate(htWindow,
                                              RegionType.Line,
                                              modelStartRow, modelStartCol,
                                              modelStopRow, modelStopCol,
                                              WireParameter.DieImageRowOffset,
                                              WireParameter.DieImageColumnOffset,
                                              0);


            if (modelLine == null) return;
            CurrentModelGroup.RefLineModelRegions = UserregionmodelLine;

            //2--显示-模板线以红色显示
            //htWindow.DisplaySingleRegion(concatBondWireRegion, WireObject.DieImage,"red");
            //'black', 'white', 'red', 'green', 'blue', 'cyan', 'magenta', 'yellow', 'dim gray', 'gray', 
            //'light gray', 'medium slate blue', 'coral', 'slate blue', 'spring green', 'orange red', 'orange', 
            //'dark olive green', 'pink', 'cadet blue', '#003075', '#e53019', '#ffb529'
            htWindow.InitialHWindow("red");
            htWindow.hTWindow.HalconWindow.DispObj(concatBondWireRegion);

            //return;

            //3-- 显示模板线上的金线检测区域，绿色显示
            HOperatorSet.GenEmptyObj(out HObject concatModelGroupRegion);
            if (CurrentModelGroup.ModelStartUserRegions != null)
            {
                //HOperatorSet.ConcatObj(CurrentModelGroup.ModelStartUserRegions.DisplayRegion, concatModelGroupRegion, out concatModelGroupRegion);
            }
            if (CurrentModelGroup.ModelStopUserRegions != null)
            {
                //HOperatorSet.ConcatObj(CurrentModelGroup.ModelStopUserRegions.DisplayRegion, concatModelGroupRegion, out concatModelGroupRegion);
            }
            if (CurrentModelGroup.RefLineModelRegions != null)
            {
                //HOperatorSet.ConcatObj(CurrentModelGroup.RefLineModelRegions.DisplayRegion, concatModelGroupRegion, out concatModelGroupRegion);
            }
            if (CurrentModelGroup.LineModelRegions.Count > 0)
            {
                foreach (var userRegions in CurrentModelGroup.LineModelRegions)
                {
                    HOperatorSet.ConcatObj(userRegions.DisplayRegion, concatModelGroupRegion, out concatModelGroupRegion);
                }
            }
            //'black', 'white', 'red', 'green', 'blue', 'cyan', 'magenta', 'yellow', 'dim gray', 'gray', 
            //'light gray', 'medium slate blue', 'coral', 'slate blue', 'spring green', 'orange red', 'orange', 
            //'dark olive green', 'pink', 'cadet blue', '#003075', '#e53019', '#ffb529'
            htWindow.InitialHWindow("cyan");
            htWindow.hTWindow.HalconWindow.DispObj(concatModelGroupRegion);

        }

        //手动画金线模板检测区域并带多套检测参数
        private void ExecuteAddModelWireUserRegionCommand(object parameter)
        {
            if (CurrentModelGroup == null)
            {
                MessageBox.Show("请点击【添加焊点金线组+】开始创建金线检测区域模板！");
                return;
            }

            if (isRightClickWire != true) return;
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                                    WireParameter.DieImageRowOffset,
                                                                    WireParameter.DieImageColumnOffset);
                if (userRegion == null) return;
                userRegion.Index = currentModelGroup.LineModelRegions.Count + 1;
                userRegion.WireRegionWithPara = new WireRegionWithPara();
                //add by lht
                userRegion.ChannelNames = ChannelNames;
                userRegion.ImageIndex = WireParameter.ImageIndex;
                userRegion.WireRegionWithPara.WireThresAlgoPara.ImageIndex = WireParameter.ImageIndex;
                userRegion.WireRegionWithPara.WireLineGauseAlgoPara.ImageIndex = WireParameter.ImageIndex;
                userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.ImageIndex = WireParameter.ImageIndex;
                //
                currentModelGroup.LineModelRegions.Add(userRegion);

                LineModelRegions = currentModelGroup.LineModelRegions;//1012

                DispalyModelGroupRegion(true);
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

        //删除金线检测区域
        private void ExecuteRemoveModelWireUserRegionCommand(object parameter)
        {
            if (WireAddAutoRegion.isRightClickWire != true) return;
            if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < currentModelGroup.LineModelRegions.Count; i++)
                {
                    if (currentModelGroup.LineModelRegions[i].IsSelected)
                    {
                        currentModelGroup.LineModelRegions.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        currentModelGroup.LineModelRegions[i].Index = i + 1;
                    }
                }
                DispalyModelGroupRegion(true);
            }
        }
        //修改金线检测区域
        private void ExecuteModifyModelWireRegionCommand(object parameter)
        {
            try
            {
                if (currentModelGroup.LineModelRegions == null) return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请添加金线模板检测区域！");
                return;
            }
            if (isRightClickWire)
            {
                isRightClickWire = false;
                htWindow.RegionType = RegionType.Null;
                try
                {
                    for (int i = 0; i < currentModelGroup.LineModelRegions.Count; i++)
                    {
                        if (currentModelGroup.LineModelRegions[i].IsSelected)
                        {
                            switch (currentModelGroup.LineModelRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(currentModelGroup.LineModelRegions[i].RegionParameters[0]),
                                                                  Math.Floor(currentModelGroup.LineModelRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(currentModelGroup.LineModelRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(currentModelGroup.LineModelRegions[i].RegionParameters[3]),
                                                                  currentModelGroup.LineModelRegions[i].RegionType,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (currentModelGroup.LineModelRegions[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                                                                   (currentModelGroup.LineModelRegions[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                                                                   (currentModelGroup.LineModelRegions[i].RegionParameters[2] - WireParameter.DieImageRowOffset),
                                                                                                   (currentModelGroup.LineModelRegions[i].RegionParameters[3] - WireParameter.DieImageColumnOffset),
                                                                                                out HTuple row1_Rectangle,
                                                                                                out HTuple column1_Rectangle,
                                                                                                out HTuple row2_Rectangle,
                                                                                                out HTuple column2_Rectangle);

                                    currentModelGroup.LineModelRegions[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, currentModelGroup.LineModelRegions[i].RegionType, row1_Rectangle, column1_Rectangle,
                                                                                             row2_Rectangle, column2_Rectangle, WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    //// modify by wj 使用修改前设置的区域检测参数
                                    userRegion.ChannelNames = ChannelNames;
                                    userRegion.ImageIndex = WireParameter.ImageIndex;
                                    //检测区域参数赋值
                                    int ModelAlgoParameterIndex = currentModelGroup.LineModelRegions[i].AlgoParameterIndex;
                                    userRegion.AlgoParameterIndex = ModelAlgoParameterIndex;
                                    //
                                    WireRegionWithPara WireInspectPara = new WireRegionWithPara();
                                    WireInspectPara = WireRegionWithPara.DeepCopyByReflection(currentModelGroup.LineModelRegions[i].WireRegionWithPara);
                                    //modify by lht
                                    userRegion.WireRegionWithPara = WireInspectPara;
                                    //
                                    currentModelGroup.LineModelRegions[i] = userRegion;
                                    currentModelGroup.LineModelRegions[i].Index = i + 1;
                                    DispalyModelGroupRegion(true);
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(currentModelGroup.LineModelRegions[i].RegionParameters[0]),
                                                                  Math.Floor(currentModelGroup.LineModelRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(currentModelGroup.LineModelRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(currentModelGroup.LineModelRegions[i].RegionParameters[3]),
                                                                  currentModelGroup.LineModelRegions[i].RegionType,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(currentModelGroup.LineModelRegions[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                                   Math.Floor(currentModelGroup.LineModelRegions[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                                   currentModelGroup.LineModelRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(currentModelGroup.LineModelRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(currentModelGroup.LineModelRegions[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    currentModelGroup.LineModelRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, currentModelGroup.LineModelRegions[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         WireParameter.DieImageRowOffset,
                                                                                                         WireParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;

                                    //// modify by wj 使用修改前设置的区域检测参数
                                    userRegion_Rectangle2.ChannelNames = ChannelNames;
                                    userRegion_Rectangle2.ImageIndex = WireParameter.ImageIndex;
                                    //检测区域参数赋值
                                    int AlgoParameterIndex = currentModelGroup.LineModelRegions[i].AlgoParameterIndex;
                                    userRegion_Rectangle2.AlgoParameterIndex = AlgoParameterIndex;
                                    //
                                    WireRegionWithPara WireInspectParas = new WireRegionWithPara();
                                    WireInspectParas = WireRegionWithPara.DeepCopyByReflection(currentModelGroup.LineModelRegions[i].WireRegionWithPara);
                                    //modify by lht
                                    userRegion_Rectangle2.WireRegionWithPara = WireInspectParas;
                                    //
                                    currentModelGroup.LineModelRegions[i] = userRegion_Rectangle2;
                                    currentModelGroup.LineModelRegions[i].Index = i + 1;
                                    DispalyModelGroupRegion(true);
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((currentModelGroup.LineModelRegions[i].RegionParameters[0]),
                                                                 (currentModelGroup.LineModelRegions[i].RegionParameters[1]),
                                                                 (currentModelGroup.LineModelRegions[i].RegionParameters[2]),
                                                                 0,
                                                                 currentModelGroup.LineModelRegions[i].RegionType,
                                                                 WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (currentModelGroup.LineModelRegions[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                               (currentModelGroup.LineModelRegions[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                               currentModelGroup.LineModelRegions[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    currentModelGroup.LineModelRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     currentModelGroup.LineModelRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     WireParameter.DieImageRowOffset,
                                                                                                     WireParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    userRegion_Circle.WireRegionWithPara = new WireRegionWithPara();
                                    currentModelGroup.LineModelRegions[i] = userRegion_Circle;
                                    currentModelGroup.LineModelRegions[i].Index = i + 1;
                                    DispalyModelGroupRegion(true);
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
                    MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    isRightClickWire = true;
                }
            }
        }


        private void ExecuteSelectLineModelRegionCommand(object parameter)
        {
            if (currentModelGroup.LineModelRegions.All(x => x.IsSelected == true))
            { IsWireRegionCheckAll = true; }
            else if (currentModelGroup.LineModelRegions.All(x => !x.IsSelected))
            { IsWireRegionCheckAll = false; }
            else
            { IsWireRegionCheckAll = null; }
        }
        
        private void ExecuteSelectAllLineModelRegionCommand(object parameter)
        {
            if (WireAddRegion.isRightClickWire != true) return;
            if (currentModelGroup?.LineModelRegions.Count == 0) return;//改
            for (int i = 0; i < currentModelGroup.LineModelRegions.Count; i++)
            {
                currentModelGroup.LineModelRegions[i].IsSelected = true;
            }
        }

        //显示线Model组
        private void DispalyModelGroupRegion(bool isHTWindowRegion = true)
        {
            //1028
            if (htWindow.hTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff || htWindow.isdeleted == true) return;

            if (CurrentModelGroup == null)
            {
                htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex), true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatModelGroupRegion);
            if (CurrentModelGroup.ModelStartUserRegions != null)
            {
                HOperatorSet.ConcatObj(CurrentModelGroup.ModelStartUserRegions.DisplayRegion, concatModelGroupRegion, out concatModelGroupRegion);
            }
            if (CurrentModelGroup.ModelStopUserRegions != null)
            {
                HOperatorSet.ConcatObj(CurrentModelGroup.ModelStopUserRegions.DisplayRegion, concatModelGroupRegion, out concatModelGroupRegion);
            }
            if (CurrentModelGroup.RefLineModelRegions != null)
            {
                HOperatorSet.ConcatObj(CurrentModelGroup.RefLineModelRegions.DisplayRegion, concatModelGroupRegion, out concatModelGroupRegion);
            }
            if (CurrentModelGroup.LineModelRegions.Count > 0)
            {
                foreach (var userRegions in CurrentModelGroup.LineModelRegions)
                {
                    HOperatorSet.ConcatObj(userRegions.DisplayRegion, concatModelGroupRegion, out concatModelGroupRegion);
                }
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatModelGroupRegion);
            }
            else
            {
                htWindow.DisplaySingleRegion(concatModelGroupRegion, Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex));
            }
        }
        #endregion

        public bool CheckCompleted()
        {
            //foreach (var group in Groups)
            //{
            //    if (group.BondOnFrameUserRegions != null)
            //    {
            //        MessageBox.Show($"序号 {group.Index.ToString()} 的焊点金线组合的第二焊点区域为空，请选择");
            //        return false;
            //    }
            //    if (group.BondOnICUserRegions != null)
            //    {
            //        MessageBox.Show($"序号 {group.Index.ToString()} 的焊点金线组合的第一焊点区域为空，请选择");
            //        return false;
            //    }
            //    if (group.RefLineUserRegions != null)
            //    {
            //        MessageBox.Show($"序号 {group.Index.ToString()} 的焊点金线组合的中继点区域为空，请选择");
            //        return false;
            //    }
            //}
            return true;
        }

        public void Initial()
        {
            htWindow.ClearSelection();
            WireParameter.IsStartPickUp = true;
            WireParameter.IsStopPickUp = true;
            WireParameter.IsWirePickUp = false;
            WireParameter.IsWireRegionPickUp = false;
            UpdateModelIndexChange();
            //htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex));
        }

        public void Dispose()
        {
            htWindow.isdeleted = true;//1028 for recipe delete err debug

            (Content as Page_WireAddAutoRegion).DataContext = null;
            (Content as Page_WireAddAutoRegion).Close();
            Content = null;
            isRightClickWire = true;
            this.htWindow = null;
            this.selectedOnRecipe = null;
            this.listboxstart = null;
            this.ModelGroups = null;
            this.WireObject = null;
            this.WireParameter = null;

            AddModelGroupCommand = null;
            RemoveModelGroupCommand = null;
            DisplayAllRegionCommand = null;
            LoadReferenceCommand = null;
            //
            StartBondOnWhatCommand = null;
            GenStartBondUserRegionCommand = null;
            DisplayAutoStopBondRegionsCommand = null;
            AddAutoStartBondUserRegionCommand = null;
            RemoveAutoStartBondUserRegionCommand = null;
            ModifyAutoStartBondRegionCommand = null;
            StartBallSortCommand = null;
            IsCheckStartCommand = null;
            IsCheckAllStartCommand = null;
            BindingWireModelCommand = null;
            StartPickUpCommand = null;
            DrawStartVirtualBallCommand = null;
            //     
            EndBondOnWhatCommand = null;
            GenEndBondUserRegionCommand = null;
            DisplayAutoStopBondRegionsCommand = null;
            AddAutoEndBondUserRegionCommand = null;
            RemoveAutoEndBondUserRegionCommand = null;
            ModifyAutoEndBondRegionCommand = null;
            StopBallSortCommand = null;
            IsCheckStopCommand = null;
            IsCheckAllStopCommand = null;
            StopPickUpCommand = null;
            DrawStopVirtualBallCommand = null;
            //    
            SelectionChangedModelIndexCommand = null;
            DisplayAllBondRegionCommand = null;
            AddModelWireUserRegionCommand = null;
            RemoveModelWireUserRegionCommand = null;
            ModifyModelWireRegionCommand = null;
            SelectLineModelRegionCommand = null;
            SelectAllLineModelRegionCommand = null;
            WireRegionPickUpCommand = null;
            WirePickUpCommand = null;
        }
    }

    //多选择
    public class OnRecipe : ViewModelBase
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        //0929
        private bool isSelected_pre;
        public bool IsSelected_pre
        {
            get => isSelected_pre;
            set
            {
                isSelected_pre = value;
                OnPropertyChanged();
            }
        }

        private int selected_ind; //被选择的顺序
        public int Selected_ind
        {
            get => selected_ind;
            set

            {
                selected_ind = value;
                OnPropertyChanged();
            }
        }

        public XElement ToXElement(string name)
        {
            XElement xElement = new XElement(name);
            xElement.Add(new XAttribute("Name", this.Name.ToString()));
            xElement.Add(new XAttribute("IsSelected", this.IsSelected ? "1" : "0"));
            xElement.Add(new XAttribute("IsSelected_pre", this.IsSelected_pre ? "1" : "0"));
            xElement.Add(new XAttribute("Selected_ind", this.Selected_ind.ToString()));
            return xElement;
        }

        public static OnRecipe FromXElement(XElement xElement)
        {
            if (xElement == null) return null;
            HObject displayRegion = null;
            HObject calculateRegion = null;
            HOperatorSet.GenEmptyObj(out displayRegion);
            HOperatorSet.GenEmptyObj(out calculateRegion);
            try
            {
                OnRecipe onRecipe = new OnRecipe
                {
                    Name = xElement.Attribute("Name").Value,
                    IsSelected = xElement.Attribute("IsSelected").Value.Equals("1") ? true : false,
                    IsSelected_pre = xElement.Attribute("IsSelected_pre").Value.Equals("1") ? true : false,//   
                    Selected_ind = int.Parse(xElement.Attribute("Selected_ind").Value)
                };

                return onRecipe;
            }
             
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return null;
            }
        }

    }
}
