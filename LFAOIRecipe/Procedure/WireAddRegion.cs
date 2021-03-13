using HalconDotNet;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LFAOIRecipe
{
    class WireAddRegion : ViewModelBase, IProcedure
    {
        //1123
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public static bool isRightClickWire = true;

        //
        //private HObject ChannelImage = null;

        private int imageIndex;
        public int ImageIndex
        {
            get => imageIndex;
            set
            {
                if (imageIndex != value)
                {
                    if (WireObject.DieImage == null)
                    {
                        MessageBox.Show("请先加载检测验证图像！");
                    }
                    WireParameter.ImageIndex = value;
                    HObject ChannelImageDisplay = Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex);
                    if (ChannelImageDisplay != null) htWindow.Display(ChannelImageDisplay, true);
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }


        //--设置金线检测模板区域内检测金线所使用的金线图像索引号 by wj
        private int imageIndex0;
        public int ImageIndex0
        {
            get => imageIndex0;
            set
            {
                if (imageIndex0 != value && SelectedUserRegion != null)
                {
                    if (value == -1)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, SelectedUserRegion.WireRegionWithPara.WireThresAlgoPara.ImageIndex + 1);

                        htWindow.Display(ChannelImageDisplay, false);
                    }
                    else if (value >= 0)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                        SelectedUserRegion.WireRegionWithPara.WireThresAlgoPara.ImageIndex = value;
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
                if (imageIndex1 != value && SelectedUserRegion != null)
                {
                    if (value == -1)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, SelectedUserRegion.WireRegionWithPara.WireLineGauseAlgoPara.ImageIndex + 1);
                        htWindow.Display(ChannelImageDisplay, false);
                    }
                    else if (value >= 0)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                        SelectedUserRegion.WireRegionWithPara.WireLineGauseAlgoPara.ImageIndex = value;
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
                if (imageIndex2 != value && SelectedUserRegion != null)
                {
                    if (value == -1)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, SelectedUserRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.ImageIndex + 1);
                        htWindow.Display(ChannelImageDisplay, false);
                    }
                    else if (value >= 0)
                    {
                        HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                        SelectedUserRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.ImageIndex = value;
                        WireParameter.ImageIndex = value;
                        htWindow.Display(ChannelImageDisplay, false);
                    }
                    imageIndex2 = value;
                    OnPropertyChanged();
                }
            }
        }




        private WireRegionsGroup currentGroup;
        public WireRegionsGroup CurrentGroup
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

        private bool? isCheckAll;//1028
        public bool? IsCheckAll//1028
        {
            get => isCheckAll;
            set => OnPropertyChanged(ref isCheckAll, value);
        }

        private bool? isCheckAllGroup;
        public bool? IsCheckAllGroup
        {
            get => isCheckAllGroup;
            set => OnPropertyChanged(ref isCheckAllGroup, value);
        }

        private bool? isCheckAllWire;
        public bool? IsCheckAllWire
        {
            get => isCheckAllWire;
            set => OnPropertyChanged(ref isCheckAllWire, value);
        }

        private UserRegion selectedUserRegion;
        public UserRegion SelectedUserRegion
        {
            get => selectedUserRegion;
            set => OnPropertyChanged(ref selectedUserRegion, value);
        }

        public ObservableCollection<WireRegionsGroup> Groups { get; private set; }

        //public ObservableCollection<WireRegionsGroup> Groups { get; private set; } = new ObservableCollection<WireRegionsGroup>();
        //public ObservableCollection<UserRegion> LineUserRegions { get; private set; }
        //private IEnumerable<HObject> LineRegions => LineUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion);

        //加载自动生成Wire检测
        public CommandBase LoadAutoWireRegionCommand { get; private set; }

        public CommandBase AddGroupCommand { get; private set; }
        public CommandBase RemoveGroupCommand { get; private set; }

        public CommandBase AddBondOnFrameUserRegionCommand { get; private set; }
        public CommandBase ModifyBondOnFrameRegionCommand { get; private set; }

        public CommandBase AddBondOnICUserRegionCommand { get; private set; }
        public CommandBase ModifyBondOnICRegionCommand { get; private set; }

        public CommandBase AddRefWireUserRegionCommand { get; private set; }

        public CommandBase DisplayAllRegionCommand { get; private set; }

        //精修每组金线检测区域
        public CommandBase AddMultiWireRegionsCommand { get; private set; }
        public CommandBase RemoveMultiWireRegionsCommand { get; private set; }
        public CommandBase ModifyMultiWireRegionsCommand { get; private set; }

        public CommandBase ClickResponseCommand { get; private set; }
        public CommandBase SelectAllLineRegionCommand { get; private set; }

        public CommandBase IsCheckCommand { get; private set; }//1028
        public CommandBase IsCheckAllCommand { get; private set; }//1028
        public CommandBase IsCheckGroupCommand { get; private set; }
        public CommandBase IsCheckAllGroupCommand { get; private set; }
        public CommandBase IsCheckWireCommand { get; private set; }
        public CommandBase IsCheckAllWireCommand { get; private set; }
        public CommandBase SetDefaultParasCommand { get; private set; }//1102
        public CommandBase ClearParasCommand { get; private set; }//1102

        private HTHalControlWPF htWindow;
        private WireObject WireObject;
        public WireParameter WireParameter { get; set; }
        public string ReferenceDirectory { get; set; }

        #region  参数设置
        private double[] threshGray = { 0, 120 };// 0, 120 
        public double[] ThreshGray
        {
            get => threshGray;
            set => OnPropertyChanged(ref threshGray, value);
        }

        private double closingSize = 2.5;//2.5
        public double ClosingSize
        {
            get => closingSize;
            set => OnPropertyChanged(ref closingSize, value);
        }

        private double[] wireWidth = { 3.5, 999 };//3.5, 999
        public double[] WireWidth
        {
            get => wireWidth;
            set => OnPropertyChanged(ref wireWidth, value);
        }

        private double[] wireLength = { 13, 999 };//13, 999
        public double[] WireLength
        {
            get => wireLength;
            set => OnPropertyChanged(ref wireLength, value);
        }

        private double[] wireArea = { 45, 999 };//45, 999
        public double[] WireArea
        {
            get => wireArea;
            set => OnPropertyChanged(ref wireArea, value);
        }

        /// <summary>
        /// 焊点至金线法线距离阈值
        /// </summary>
        private double distTh = 0;
        public double DistTh
        {
            get => distTh;
            set => OnPropertyChanged(ref distTh, value);
        }

        ///////////////////////////////////////////////////////
        private double wireWidth_Gause = 3.0;//3.0
        public double WireWidth_Gause
        {
            get => wireWidth_Gause;
            set => OnPropertyChanged(ref wireWidth_Gause, value);
        }

        private double wireContrast_Gause = 15.0;//15.0
        public double WireContrast_Gause
        {
            get => wireContrast_Gause;
            set => OnPropertyChanged(ref wireContrast_Gause, value);
        }

        private string lightOrDark_Gause = "dark";//dark
        public string LightOrDark_Gause
        {
            get => lightOrDark_Gause;
            set => OnPropertyChanged(ref lightOrDark_Gause, value);
        }

        private string[] selMetric_Gause = { "contour_length", "direction" };//"contour_length", "direction" 
        public string[] SelMetric_Gause
        {
            get => selMetric_Gause;
            set => OnPropertyChanged(ref selMetric_Gause, value);
        }

        private double[] selMin_Gause = { 3, -0.436 };//3, -0.436
        public double[] SelMin_Gause
        {
            get => selMin_Gause;
            set => OnPropertyChanged(ref selMin_Gause, value);
        }

        //25 / 180 * Math.PI
        private double[] selMax_Gause = { 999, 0.436 };//999, 0.436
        public double[] SelMax_Gause
        {
            get => selMax_Gause;
            set => OnPropertyChanged(ref selMax_Gause, value);
        }

        //25 / 180 * Math.PI
        private double linePhiDiff_Gause = 0.436;//0.436
        public double LinePhiDiff_Gause
        {
            get => linePhiDiff_Gause;
            set => OnPropertyChanged(ref linePhiDiff_Gause, value);
        }

        private double maxWireGap_Gause = 5;//5
        public double MaxWireGap_Gause
        {
            get => maxWireGap_Gause;
            set => OnPropertyChanged(ref maxWireGap_Gause, value);
        }

        /// <summary>
        /// 焊点至金线法线距离阈值
        /// </summary>
        private double distTh_Gause = 0;
        public double DistTh_Gause
        {
            get => distTh_Gause;
            set => OnPropertyChanged(ref distTh_Gause, value);
        }

        ////////////////////////////////////////////////////////////
        private double wireWidth_GauseAll = 3.0;//3.0
        public double WireWidth_GauseAll
        {
            get => wireWidth_GauseAll;
            set => OnPropertyChanged(ref wireWidth_GauseAll, value);
        }

        private double wireContrast_GauseAll = 15.0;//15.0
        public double WireContrast_GauseAll
        {
            get => wireContrast_GauseAll;
            set => OnPropertyChanged(ref wireContrast_GauseAll, value);
        }

        private string lightOrDark_GauseAll = "dark";//"dark"
        public string LightOrDark_GauseAll
        {
            get => lightOrDark_GauseAll;
            set => OnPropertyChanged(ref lightOrDark_GauseAll, value);
        }

        private string[] selMetric_GauseAll = { "contour_length", "direction" };//"contour_length", "direction"
        public string[] SelMetric_GauseAll
        {
            get => selMetric_GauseAll;
            set => OnPropertyChanged(ref selMetric_GauseAll, value);
        }

        private double[] selMin_GauseAll = { 3, -0.436 };//3, -0.436
        public double[] SelMin_GauseAll
        {
            get => selMin_GauseAll;
            set => OnPropertyChanged(ref selMin_GauseAll, value);
        }

        // 25/180 * Math.PI
        private double[] selMax_GauseAll = { 999, 0.436 };// 999, 0.436
        public double[] SelMax_GauseAll
        {
            get => selMax_GauseAll;
            set => OnPropertyChanged(ref selMax_GauseAll, value);
        }

        //25 / 180 * Math.PI
        private double linePhiDiff_GauseAll = 0.436;//0.436
        public double LinePhiDiff_GauseAll
        {
            get => linePhiDiff_GauseAll;
            set => OnPropertyChanged(ref linePhiDiff_GauseAll, value);
        }

        private double maxWireGap_GauseAll = 5;//5
        public double MaxWireGap_GauseAll
        {
            get => maxWireGap_GauseAll;
            set => OnPropertyChanged(ref maxWireGap_GauseAll, value);
        }

        /// <summary>
        /// 是否开启双线预处理 "是" "否"
        /// </summary>
        private bool isDoubleLines_GauseAll = false;
        public bool IsDoubleLines_GauseAll
        {
            get => isDoubleLines_GauseAll;
            set => OnPropertyChanged(ref isDoubleLines_GauseAll, value);
        }

        /// <summary>
        /// 亮暗因数 light_dark_light dark_light_dark
        /// </summary>
        private string doubleLinesType_GauseAll = "light_dark_light";
        public string DoubleLinesType_GauseAll
        {
            get => doubleLinesType_GauseAll;
            set => OnPropertyChanged(ref doubleLinesType_GauseAll, value);
        }

        /// <summary>
        /// 焊点至金线法线距离阈值
        /// </summary>
        private double middleLineWidth_GauseAll = 3;
        public double MiddleLineWidth_GauseAll
        {
            get => middleLineWidth_GauseAll;
            set => OnPropertyChanged(ref middleLineWidth_GauseAll, value);
        }

        /// <summary>
        /// 焊点至金线法线距离阈值
        /// </summary>
        private double distTh_GauseAll = 0;
        public double DistTh_GauseAll
        {
            get => distTh_GauseAll;
            set => OnPropertyChanged(ref distTh_GauseAll, value);
        }
        #endregion

        public CommandBase ModifyParametersModelBatchCommand { get; private set; }//1028
        public CommandBase ModifyParametersBatchCommand { get; private set; }//1028

        //
        public ObservableCollection<UserRegion> StartBallAutoUserRegion { get; private set; }
        public ObservableCollection<UserRegion> StopBallAutoUserRegion { get; private set; }
        public ObservableCollection<WireAutoRegionGroup> ModelGroups { get; private set; }


        public WireAddRegion(HTHalControlWPF htWindow,
                             string referenceDirectory,
                             WireObject wireObject,
                             WireParameter wireParameter,
                             ObservableCollection<WireRegionsGroup> groups,
                             ObservableCollection<UserRegion> startBallAutoUserRegion,
                             ObservableCollection<UserRegion> stopBallAutoUserRegion,
                             ObservableCollection<WireAutoRegionGroup> modelGroups)
        {
            DisplayName = "添加焊点金线区域";
            Content = new Page_WireAddRegion { DataContext = this };
            this.htWindow = htWindow;
            this.ReferenceDirectory = referenceDirectory;
            this.Groups = groups;
            this.StartBallAutoUserRegion = startBallAutoUserRegion;
            this.StopBallAutoUserRegion = stopBallAutoUserRegion;
            this.ModelGroups = modelGroups;
            this.WireObject = wireObject;
            this.WireParameter = wireParameter;

            LoadAutoWireRegionCommand = new CommandBase(ExecuteLoadAutoWireRegionCommand);

            AddGroupCommand = new CommandBase(ExecuteAddGroupCommand);
            RemoveGroupCommand = new CommandBase(ExecuteRemoveGroupCommand);

            AddBondOnFrameUserRegionCommand = new CommandBase(ExecuteAddBondOnFrameUserRegionCommand);
            ModifyBondOnFrameRegionCommand = new CommandBase(ExecuteModifyBondOnFrameRegionCommand);

            AddBondOnICUserRegionCommand = new CommandBase(ExecuteAddBondOnICUserRegionCommand);
            ModifyBondOnICRegionCommand = new CommandBase(ExecuteModifyBondOnICRegionCommand);

            AddRefWireUserRegionCommand = new CommandBase(ExecuteAddRefWireUserRegionCommand);

            AddMultiWireRegionsCommand = new CommandBase(ExecuteAddMultiWireRegionsCommand);
            RemoveMultiWireRegionsCommand = new CommandBase(ExecuteRemoveMultiWireRegionsCommand);
            ModifyMultiWireRegionsCommand = new CommandBase(ExecuteModifyMultiWireRegionsCommand);

            DisplayAllRegionCommand = new CommandBase(ExecuteDisplayAllRegionCommand);

            ClickResponseCommand = new CommandBase(ExecuteClickResponseCommand);
            SelectAllLineRegionCommand = new CommandBase(ExecuteSelectAllLineRegionCommand);

            ModifyParametersModelBatchCommand = new CommandBase(ExecuteModifyParametersModelBatchCommand);//1028
            ModifyParametersBatchCommand = new CommandBase(ExecuteModifyParametersBatchCommand);//1028

            IsCheckCommand = new CommandBase(ExecuteIsCheckCommand);//1028
            IsCheckAllCommand = new CommandBase(ExecuteIsCheckAllCommand);//1028
            IsCheckGroupCommand = new CommandBase(ExecuteIsCheckGroupCommand);
            IsCheckAllGroupCommand = new CommandBase(ExecuteIsCheckAllGroupCommand);
            IsCheckWireCommand = new CommandBase(ExecuteIsCheckWireCommand);
            IsCheckAllWireCommand = new CommandBase(ExecuteIsCheckAllWireCommand);
            SetDefaultParasCommand = new CommandBase(ExecuteSetDefaultParasCommand);//1102
            ClearParasCommand = new CommandBase(ExecuteClearParasCommand);//1102


            CurrentGroup = new WireRegionsGroup
            {
                Index = groups.Count + 1
            };
            groups.Add(CurrentGroup);
            GroupsCount = groups.Count;
        }

        private void ExecuteSetDefaultParasCommand(object parameter)//1102
        {
            ThreshGray =new double[]{ 0, 120 };
            ClosingSize = 2.5; 
            WireWidth = new double[] { 3.5, 999 };
            WireLength = new double[] { 13, 999 };
            WireArea = new double[] { 45, 999 };
            DistTh = 0;
            WireWidth_Gause = 3.0; 
            WireContrast_Gause = 15.0; 
            LightOrDark_Gause = "dark"; 
            SelMetric_Gause = new string[]{ "contour_length", "direction" };
            SelMin_Gause = new double[] { 3, -0.436 };
            SelMax_Gause = new double[] { 999, 0.436 }; 
            LinePhiDiff_Gause = 0.436;
            MaxWireGap_Gause = 5;
            DistTh_Gause = 0;
            WireWidth_GauseAll = 3.0;
            WireContrast_GauseAll = 15.0;
            LightOrDark_GauseAll = "dark";
            SelMetric_GauseAll = new string[] { "contour_length", "direction" };
            SelMin_GauseAll = new double[] { 3, -0.436 };
            SelMax_GauseAll = new double[] { 999, 0.436 };
            LinePhiDiff_GauseAll = 0.436; 
            MaxWireGap_GauseAll = 5;
            IsDoubleLines_GauseAll = false;
            DoubleLinesType_GauseAll = "light_dark_light";
            MiddleLineWidth_GauseAll = 3;
            DistTh_GauseAll = 0;
         }

        private void ExecuteClearParasCommand(object parameter)//1102
        {
            ThreshGray = new double[] { 0, 0 };
            ClosingSize = 0;
            WireWidth = new double[] { 0, 0 };
            WireLength = new double[] { 0, 0 };
            WireArea = new double[] { 0, 0 };
            DistTh = 0;
            WireWidth_Gause = 0;
            WireContrast_Gause = 0;
            LightOrDark_Gause = "dark";
            SelMetric_Gause = new string[] { "contour_length", "direction" };
            SelMin_Gause = new double[] { 0, 0 };
            SelMax_Gause = new double[] { 0, 0 };
            LinePhiDiff_Gause = 0;
            MaxWireGap_Gause = 0;
            DistTh_Gause = 0;
            WireWidth_GauseAll = 0;
            WireContrast_GauseAll = 0;
            LightOrDark_GauseAll = "dark";
            SelMetric_GauseAll = new string[] { "contour_length", "direction" };
            SelMin_GauseAll = new double[] { 0, 0 };
            SelMax_GauseAll = new double[] { 0, 0 };
            LinePhiDiff_GauseAll = 0;
            MaxWireGap_GauseAll = 0;
            IsDoubleLines_GauseAll = false;
            DoubleLinesType_GauseAll = "light_dark_light";
            MiddleLineWidth_GauseAll = 0;
            DistTh_GauseAll = 0;
        }

        private void ExecuteModifyParametersModelBatchCommand(object parameter)//1028
        {
            if (isRightClickWire != true) return;
            if (currentGroup == null || currentGroup.LineUserRegions.Count == 0 || Groups.Count == 0) return;
            try
            {
                if (MessageBox.Show("是否进行参数批量修改?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (WireParameter.IsWireBatchParasSet == true)
                    {
                        WireParameter.IsGroupBatchParasSet = false;
                        foreach (var item in Groups)
                        {
                            if (item.IsSelected == true)
                            {
                                if (item.LineUserRegions.Count != CurrentGroup.LineUserRegions.Count)
                                {
                                    MessageBox.Show("和模板金线区域不是同一类！");
                                    return;
                                }
                                for (int i = 0; i < item.LineUserRegions.Count; i++)
                                {
                                    if (item.LineUserRegions[i].AlgoParameterIndex == 0)
                                    {
                                        item.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ThreshGray = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ThreshGray;
                                        item.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ClosingSize = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ClosingSize;
                                        item.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireWidth = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireWidth;
                                        item.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireLength = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireLength;
                                        item.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireArea = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireArea;
                                        item.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.DistTh = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.DistTh;
                                    }
                                    else if (item.LineUserRegions[i].AlgoParameterIndex == 1)
                                    {
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.WireWidth = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.WireWidth;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.WireContrast = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.WireContrast;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.LightOrDark = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.LightOrDark;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMetric = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMetric;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMin = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMin;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMax = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMax;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.LinePhiDiff = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.LinePhiDiff;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.MaxWireGap = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.MaxWireGap;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.DistTh = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.DistTh;
                                    }
                                    else if (item.LineUserRegions[i].AlgoParameterIndex == 2)
                                    {
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.WireWidth = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.WireWidth;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.WireContrast = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.WireContrast;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.LightOrDark = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.LightOrDark;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMetric = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMetric;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMin = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMin;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMax = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMax;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.LinePhiDiff = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.LinePhiDiff;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.MaxWireGap = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.MaxWireGap;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.IsDoubleLines = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.IsDoubleLines;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.DoubleLinesType = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.DoubleLinesType;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.MiddleLineWidth = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.MiddleLineWidth;
                                        item.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.DistTh = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.DistTh;
                                    }
                                }
                            }
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

        private void ExecuteModifyParametersBatchCommand(object parameter)//1028
        {
            if (isRightClickWire != true) return;
            if (currentGroup == null || currentGroup.LineUserRegions.Count == 0 || Groups.Count == 0) return;
            try
            {
                if (MessageBox.Show("是否进行参数批量修改?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (WireParameter.IsGroupBatchParasSet == true)
                    {
                        WireParameter.IsWireBatchParasSet = false;
                        foreach (var item in Groups)
                        {
                            if (item.IsSelected == true)
                            {
                                foreach (var _item in item.LineUserRegions)
                                {
                                    if (_item.AlgoParameterIndex == 0)
                                    {
                                        if (ThreshGray[0]!=0 || ThreshGray[1] != 0)
                                        {
                                            _item.WireRegionWithPara.WireThresAlgoPara.ThreshGray = ThreshGray;
                                        }
                                        if (ClosingSize != 0)
                                        {
                                            _item.WireRegionWithPara.WireThresAlgoPara.ClosingSize = ClosingSize;
                                        }
                                        if (WireWidth[0] != 0 || WireWidth[1] != 0)
                                        {
                                            _item.WireRegionWithPara.WireThresAlgoPara.WireWidth = WireWidth;
                                        }
                                        if (WireLength[0] != 0 || WireLength[1] != 0)
                                        {
                                            _item.WireRegionWithPara.WireThresAlgoPara.WireLength = WireLength;
                                        }
                                        if (WireArea[0] != 0 || WireArea[1] != 0)
                                        {
                                            _item.WireRegionWithPara.WireThresAlgoPara.WireArea = WireArea;
                                        }
                                        if (DistTh != 0)
                                        {
                                            _item.WireRegionWithPara.WireThresAlgoPara.DistTh = DistTh;
                                        }
                                    }
                                    else if (_item.AlgoParameterIndex == 1)
                                    {
                                        if (WireWidth_Gause != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoPara.WireWidth = WireWidth_Gause;
                                        }
                                        if (WireContrast_Gause != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoPara.WireContrast = WireContrast_Gause;
                                        }
                                        _item.WireRegionWithPara.WireLineGauseAlgoPara.LightOrDark = LightOrDark_Gause;
                                        _item.WireRegionWithPara.WireLineGauseAlgoPara.SelMetric = SelMetric_Gause;
                                        if (SelMin_Gause[0] != 0 || SelMin_Gause[1] != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoPara.SelMin = SelMin_Gause;
                                        }
                                        if (SelMax_Gause[0] != 0 || SelMax_Gause[1] != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoPara.SelMax = SelMax_Gause;
                                        }
                                        if (LinePhiDiff_Gause != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoPara.LinePhiDiff = LinePhiDiff_Gause;
                                        }
                                        if (MaxWireGap_Gause != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoPara.MaxWireGap = MaxWireGap_Gause;
                                        }
                                        if (DistTh_Gause != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoPara.DistTh = DistTh_Gause;
                                        }
                                    }
                                    else if (_item.AlgoParameterIndex == 2)
                                    {
                                        if (WireWidth_GauseAll != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoParaAll.WireWidth = WireWidth_GauseAll;
                                        }
                                        if (WireContrast_GauseAll != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoParaAll.WireContrast = WireContrast_GauseAll;
                                        }
                                        _item.WireRegionWithPara.WireLineGauseAlgoParaAll.LightOrDark = LightOrDark_GauseAll;
                                        _item.WireRegionWithPara.WireLineGauseAlgoParaAll.SelMetric = SelMetric_GauseAll;
                                        if (SelMin_GauseAll[0] != 0 || SelMin_GauseAll[1] != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoParaAll.SelMin = SelMin_GauseAll;
                                        }
                                        if (SelMax_GauseAll[0] != 0 || SelMax_GauseAll[1] != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoParaAll.SelMax = SelMax_GauseAll;
                                        }
                                        if (LinePhiDiff_GauseAll != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoParaAll.LinePhiDiff = LinePhiDiff_GauseAll;
                                        }
                                        if (MaxWireGap_GauseAll != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoParaAll.MaxWireGap = MaxWireGap_GauseAll;
                                        }
                                        if (MiddleLineWidth_GauseAll != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoParaAll.MiddleLineWidth = MiddleLineWidth_GauseAll;
                                        }
                                        if (DistTh_GauseAll != 0)
                                        {
                                            _item.WireRegionWithPara.WireLineGauseAlgoParaAll.DistTh = DistTh_GauseAll;
                                        }
                                    }
                                }
                            }
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

        private void ExecuteIsCheckCommand(object parameter)//1028
        {
            if (CurrentGroup.LineUserRegions.All(x => x.IsSelected == true))
            { IsCheckAll = true; }
            else if (CurrentGroup.LineUserRegions.All(x => !x.IsSelected))
            { IsCheckAll = false; }
            else
            { IsCheckAll = null; }
        }

        private void ExecuteIsCheckAllCommand(object parameter)//1028
        {
            if (IsCheckAll == true)
            { CurrentGroup.LineUserRegions.ToList().ForEach(r => r.IsSelected = true); }
            else if (IsCheckAll == false)
            { CurrentGroup.LineUserRegions.ToList().ForEach(r => r.IsSelected = false); }
        }

        private void ExecuteIsCheckGroupCommand(object parameter)
        {
            if (Groups.All(x => x.IsSelected == true))
            { IsCheckAllGroup = true; }
            else if (Groups.All(x => !x.IsSelected))
            { IsCheckAllGroup = false; }
            else
            { IsCheckAllGroup = null; }
        }

        private void ExecuteIsCheckAllGroupCommand(object parameter)
        {
            if (IsCheckAllGroup == true)
            { Groups.ToList().ForEach(r => r.IsSelected = true); }
            else if (IsCheckAllGroup == false)
            { Groups.ToList().ForEach(r => r.IsSelected = false); }
        }

        private void ExecuteIsCheckWireCommand(object parameter)
        {
            if (Groups.All(x => x.IsSelected == true))
            { IsCheckAllWire = true; }
            else if (Groups.All(x => !x.IsSelected))
            { IsCheckAllWire = false; }
            else
            { IsCheckAllWire = null; }
        }

        private void ExecuteIsCheckAllWireCommand(object parameter)
        {
            if (IsCheckAllWire == true)
            { Groups.ToList().ForEach(r => r.IsSelected = true); }
            else if (IsCheckAllWire == false)
            { Groups.ToList().ForEach(r => r.IsSelected = false); }
        }

        private void ExecuteLoadAutoWireRegionCommand(object parameter)
        {
            if (StartBallAutoUserRegion.Count == 0)
            {
                MessageBox.Show("请先自动生成金线起始焊点！");
                return;
            }
            if (StopBallAutoUserRegion.Count == 0)
            {
                MessageBox.Show("请先自动生成金线结束焊点！");
                return;
            }
            if (ModelGroups.Count == 0)
            {
                MessageBox.Show("请生成金线检测区域模板！");
                return;
            }
            try
            {
                // 金线组清零
                Groups.Clear();

                for (int i = 0; i < StartBallAutoUserRegion.Count; i++)
                {
                    //创建自动金线检测区域
                    WireRegionsGroup WireRegion = new WireRegionsGroup()
                    {
                        Index = Groups.Count + 1
                    };

                    //区域目标模板类型
                    int regType = WireParameter.WireRegModelType[i];
                    //--------获取金线起始区域
                    UserRegion _StartUseBall = StartBallAutoUserRegion.ElementAt(i);
                    WireRegion.BondOnFrameUserRegions = new UserRegion();
                    WireRegion.BondOnFrameUserRegions = _StartUseBall;

                    //--------获取金线结束区域
                    UserRegion _StopUseBall = StopBallAutoUserRegion.ElementAt(i);
                    WireRegion.BondOnICUserRegions = new UserRegion();
                    WireRegion.BondOnICUserRegions = _StopUseBall;

                    //--------获取金线检测区域

                    //选择模板组进行金线检测区域模板创建
                    WireAutoRegionGroup wireModelGroup = ModelGroups.ElementAt(regType - 1);
                    //金线模板起始焊点
                    UserRegion _ModelStartBall = wireModelGroup.ModelStartUserRegions;
                    //金线模板结束焊点
                    UserRegion _ModelStopBall = wireModelGroup.ModelStopUserRegions;
                    //金线模板检测区域
                    ObservableCollection<UserRegion> modelInspectReg = new ObservableCollection<UserRegion>();
                    modelInspectReg = wireModelGroup.LineModelRegions;
                    //获取映射矩阵
                    Algorithm.Model_RegionAlg.affine_transformation_matrix(Algorithm.Region.ConcatRegion(_StartUseBall), Algorithm.Region.ConcatRegion(_StopUseBall),
                                                                           Algorithm.Region.ConcatRegion(_ModelStartBall), Algorithm.Region.ConcatRegion(_ModelStopBall), out HTuple _HomMat2D,
                                                                           out HTuple _RefDirection);
                    //获取映射后的金线检测区域
                    HOperatorSet.AffineTransRegion(Algorithm.Region.ConcatRegion(modelInspectReg), out HObject _wireInspectRegs,
                                                  _HomMat2D, "nearest_neighbor");
                    //生成金线检测区域带参数
                    ObservableCollection<UserRegion> WireInspectRegs = new ObservableCollection<UserRegion>();

                    //金线检测区域
                    WireRegion.LineUserRegions = new ObservableCollection<UserRegion>();

                    for (int j = 0; j < modelInspectReg.Count; j++)
                    {
                        switch (modelInspectReg[j].RegionType)
                        {
                            case RegionType.Point:
                                break;
                            case RegionType.Rectangle1:
                                //如果区域是rectangle1

                                break;
                            case RegionType.Rectangle2:

                                HOperatorSet.SelectObj(_wireInspectRegs, out HObject wireInspectReg, j + 1);

                                HOperatorSet.SmallestRectangle2(wireInspectReg, out HTuple row_Rectangle,
                                                                                out HTuple column_Rectangle,
                                                                                out HTuple phi_Rectangle2,
                                                                                out HTuple lenth1_Rectangle2,
                                                                                out HTuple lenth2_Rectangle2);
                                //生成需要的金线检测区域
                                HTuple row_Rectangle2 = row_Rectangle - WireParameter.DieImageRowOffset;
                                HTuple column_Rectangle2 = column_Rectangle - WireParameter.DieImageColumnOffset;

                                UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Rectangle2,
                                                                                               row_Rectangle2, column_Rectangle2,
                                                                                               lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                               WireParameter.DieImageRowOffset,
                                                                                               WireParameter.DieImageColumnOffset,
                                                                                               phi_Rectangle2);

                                if (userRegion_Rectangle2 == null) return;

                                userRegion_Rectangle2.Index = WireRegion.LineUserRegions.Count + 1;
                                userRegion_Rectangle2.ChannelNames = ChannelNames;
                                userRegion_Rectangle2.ImageIndex = WireParameter.ImageIndex;
                                //检测区域参数赋值
                                int ModelAlgoParameterIndex = modelInspectReg[j].AlgoParameterIndex;
                                userRegion_Rectangle2.AlgoParameterIndex = ModelAlgoParameterIndex;
                                //
                                WireRegionWithPara WireInspectPara = new WireRegionWithPara();
                                WireInspectPara = WireRegionWithPara.DeepCopyByReflection(modelInspectReg[j].WireRegionWithPara);
                                //modify by lht
                                //WireRegionWithPara WireInspectPara = modelInspectReg[j].WireRegionWithPara;

                                userRegion_Rectangle2.WireRegionWithPara = WireInspectPara;

                                WireRegion.LineUserRegions.Add(userRegion_Rectangle2);

                                break;
                            default: break;
                        }
                    }

                    //放入Groups中
                    Groups.Add(WireRegion);
                }

                GroupsCount = Groups.Count;

                DispalyGroupRegionAll(true);//1030 lw
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteAddGroupCommand(object parameter)
        {
            if (WireAddRegion.isRightClickWire != true) return;
            CurrentGroup = new WireRegionsGroup
            {
                Index = Groups.Count + 1
            };
            Groups.Add(CurrentGroup);
            GroupsCount = Groups.Count;
            DispalyGroupRegion();
            //MessageBox.Show($"新建了序号 {CurrentGroup.Index.ToString()} 的焊点金线组合");
        }

        private void ExecuteRemoveGroupCommand(object parameter)
        {
            if (isRightClickWire != true) return;
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

        //焊点startRegion
        private void ExecuteAddBondOnFrameUserRegionCommand(object parameter)
        {
            if (isRightClickWire != true) return;
            if (htWindow.RegionType != RegionType.Circle)
            {
                MessageBox.Show("请使用圆形画框工具！");
                return;
            }
            if (CurrentGroup == null)
            {
                MessageBox.Show("请选择或者新建一个焊点金线组合");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                                    WireParameter.DieImageRowOffset,
                                                                    WireParameter.DieImageColumnOffset);
                if (userRegion == null) return;
                CurrentGroup.BondOnFrameUserRegions = userRegion;
                DispalyGroupRegion(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteModifyBondOnFrameRegionCommand(object parameter)
        {
            if (currentGroup?.BondOnFrameUserRegions == null) return;
            if (isRightClickWire)
            {
                isRightClickWire = false;
                htWindow.RegionType = RegionType.Null;
                try
                {
                    switch (currentGroup.BondOnFrameUserRegions.RegionType)
                    {
                        case RegionType.Point:
                            break;

                        case RegionType.Rectangle1:
                            htWindow.InitialHWindowUpdate(Math.Floor(currentGroup.BondOnFrameUserRegions.RegionParameters[0]),
                                                          Math.Floor(currentGroup.BondOnFrameUserRegions.RegionParameters[1]),
                                                          Math.Ceiling(currentGroup.BondOnFrameUserRegions.RegionParameters[2]),
                                                          Math.Ceiling(currentGroup.BondOnFrameUserRegions.RegionParameters[3]), currentGroup.BondOnFrameUserRegions.RegionType,
                                                          WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");
                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (currentGroup.BondOnFrameUserRegions.RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                                                            (currentGroup.BondOnFrameUserRegions.RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                                                            (currentGroup.BondOnFrameUserRegions.RegionParameters[2] - WireParameter.DieImageRowOffset),
                                                                                           (currentGroup.BondOnFrameUserRegions.RegionParameters[3] - WireParameter.DieImageColumnOffset),
                                                                                        out HTuple row1_Rectangle,
                                                                                        out HTuple column1_Rectangle,
                                                                                        out HTuple row2_Rectangle,
                                                                                        out HTuple column2_Rectangle);

                            currentGroup.BondOnFrameUserRegions.RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.BondOnFrameUserRegions.RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset);
                            if (userRegion == null) return;
                            currentGroup.BondOnFrameUserRegions = userRegion;
                            DispalyGroupRegion(true);
                            break;

                        case RegionType.Rectangle2:
                            break;

                        case RegionType.Circle:
                            htWindow.InitialHWindowUpdate((currentGroup.BondOnFrameUserRegions.RegionParameters[0]),
                                                         (currentGroup.BondOnFrameUserRegions.RegionParameters[1]),
                                                         (currentGroup.BondOnFrameUserRegions.RegionParameters[2]),
                                                         0,
                                                         currentGroup.BondOnFrameUserRegions.RegionType,
                                                         WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                            HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                       (currentGroup.BondOnFrameUserRegions.RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                       (currentGroup.BondOnFrameUserRegions.RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                       currentGroup.BondOnFrameUserRegions.RegionParameters[2],
                                                   out HTuple row_Circle,
                                                   out HTuple column_Circle,
                                                   out HTuple radius_Circle);

                            currentGroup.BondOnFrameUserRegions.RegionType = RegionType.Circle;
                            UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                             currentGroup.BondOnFrameUserRegions.RegionType,
                                                                                             row_Circle, column_Circle,
                                                                                             radius_Circle, 0,
                                                                                             WireParameter.DieImageRowOffset,
                                                                                             WireParameter.DieImageColumnOffset,
                                                                                             0);
                            if (userRegion_Circle == null) return;
                            currentGroup.BondOnFrameUserRegions = userRegion_Circle;
                            DispalyGroupRegion(true);
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
                    isRightClickWire = true;
                }
            }
        }

        //焊点stopRegion
        private void ExecuteAddBondOnICUserRegionCommand(object parameter)
        {
            if (isRightClickWire != true) return;//
            if (htWindow.RegionType != RegionType.Circle)
            {
                MessageBox.Show("请使用圆形画框工具！");
                return;
            }
            if (CurrentGroup == null)
            {
                MessageBox.Show("请选择或者新建一个焊点金线组合");
                return;
            }
            UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                                WireParameter.DieImageRowOffset,
                                                                WireParameter.DieImageColumnOffset);
            if (userRegion == null) return;
            CurrentGroup.BondOnICUserRegions = userRegion;
            DispalyGroupRegion(true);
        }

        private void ExecuteModifyBondOnICRegionCommand(object parameter)//
        {
            try
            {
                if (currentGroup.BondOnICUserRegions == null) return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请添加第一焊点区域");
                return;
            }
            if (WireAddRegion.isRightClickWire)
            {
                WireAddRegion.isRightClickWire = false;
                htWindow.RegionType = RegionType.Null;
                try
                {
                    switch (currentGroup.BondOnICUserRegions.RegionType)
                    {
                        case RegionType.Point:
                            break;

                        case RegionType.Rectangle1:
                            htWindow.InitialHWindowUpdate(Math.Floor(currentGroup.BondOnICUserRegions.RegionParameters[0]),
                                                 Math.Floor(currentGroup.BondOnICUserRegions.RegionParameters[1]),
                                                 Math.Ceiling(currentGroup.BondOnICUserRegions.RegionParameters[2]),
                                                 Math.Ceiling(currentGroup.BondOnICUserRegions.RegionParameters[3]), currentGroup.BondOnICUserRegions.RegionType,
                                                 WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");
                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (currentGroup.BondOnICUserRegions.RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                        (currentGroup.BondOnICUserRegions.RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                        (currentGroup.BondOnICUserRegions.RegionParameters[2] - WireParameter.DieImageRowOffset),
                                                        (currentGroup.BondOnICUserRegions.RegionParameters[3] - WireParameter.DieImageColumnOffset),
                                               out HTuple row1_Rectangle,
                                               out HTuple column1_Rectangle,
                                               out HTuple row2_Rectangle,
                                               out HTuple column2_Rectangle);

                            currentGroup.BondOnICUserRegions.RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.BondOnICUserRegions.RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset);
                            if (userRegion == null) return;
                            currentGroup.BondOnICUserRegions = userRegion;
                            DispalyGroupRegion(true);
                            break;

                        case RegionType.Rectangle2:
                            break;

                        case RegionType.Circle:
                            htWindow.InitialHWindowUpdate((currentGroup.BondOnICUserRegions.RegionParameters[0]),
                            (currentGroup.BondOnICUserRegions.RegionParameters[1]),
                            (currentGroup.BondOnICUserRegions.RegionParameters[2]),
                            0,
                            currentGroup.BondOnICUserRegions.RegionType,
                            WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                            HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                       (currentGroup.BondOnICUserRegions.RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                       (currentGroup.BondOnICUserRegions.RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                       currentGroup.BondOnICUserRegions.RegionParameters[2],
                                                   out HTuple row_Circle,
                                                   out HTuple column_Circle,
                                                   out HTuple radius_Circle);

                            currentGroup.BondOnICUserRegions.RegionType = RegionType.Circle;
                            UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                             currentGroup.BondOnICUserRegions.RegionType,
                                                                                             row_Circle, column_Circle,
                                                                                             radius_Circle, 0,
                                                                                             WireParameter.DieImageRowOffset,
                                                                                             WireParameter.DieImageColumnOffset,
                                                                                             0);
                            if (userRegion_Circle == null) return;
                            currentGroup.BondOnICUserRegions = userRegion_Circle;
                            DispalyGroupRegion(true);
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
                    WireAddRegion.isRightClickWire = true;
                }
            }
        }

        // RefLine 
        private void ExecuteAddRefWireUserRegionCommand(object parameter)
        {
            if (WireAddRegion.isRightClickWire != true) return;//
            if (CurrentGroup == null)
            {
                MessageBox.Show("请选择或者新建一个焊点金线组合");
                return;
            }
            if (CurrentGroup.BondOnFrameUserRegions == null)
            {
                MessageBox.Show("请添加金线起始点区域！");
                return;
            }
            if (CurrentGroup.BondOnICUserRegions == null)
            {
                MessageBox.Show("请添加金线结束点区域！");
                return;
            }

            try
            {
                UserRegion userRegion_Line = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Line,
                                                                  CurrentGroup.BondOnFrameUserRegions.RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                                  CurrentGroup.BondOnFrameUserRegions.RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                                  CurrentGroup.BondOnICUserRegions.RegionParameters[0] - WireParameter.DieImageRowOffset,
                                                                  CurrentGroup.BondOnICUserRegions.RegionParameters[1] - WireParameter.DieImageColumnOffset,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset);
                if (userRegion_Line == null) return;
                userRegion_Line.RegionType = RegionType.Line;
                currentGroup.RefLineUserRegions = userRegion_Line;
                DispalyGroupRegion(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteDisplayAllRegionCommand(object parameter)
        {
            if (WireAddRegion.isRightClickWire != true) return;
            DispalyGroupRegionAll(true);//
        }

        private void DispalyGroupRegionAll(bool isHTWindowRegion = false)//
        {
            if (GroupsCount == 0)
            {
                htWindow.Display(WireObject.DieImage, true);//
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);

            for (int i = 0; i < groupsCount; i++)
            {
                if (Groups[i].BondOnFrameUserRegions != null)
                {
                    HOperatorSet.ConcatObj(concatGroupRegion, Groups[i].BondOnFrameUserRegions.DisplayRegion, out concatGroupRegion);
                }
                if (Groups[i].BondOnICUserRegions != null)
                {
                    HOperatorSet.ConcatObj(concatGroupRegion, Groups[i].BondOnICUserRegions.DisplayRegion, out concatGroupRegion);
                }
                if (Groups[i].LineUserRegions != null)
                {
                    foreach (var userRegions in Groups[i].LineUserRegions)
                    {
                        HOperatorSet.ConcatObj(concatGroupRegion, userRegions.DisplayRegion, out concatGroupRegion);
                    }
                }
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                //htWindow.DisplaySingleRegion(concatGroupRegion, WireObject.Image);
                htWindow.DisplayMultiRegion(concatGroupRegion, WireObject.DieImage);
            }
        }

        private void DispalyGroupRegion(bool isDisplayImage = false)
        {
            if (CurrentGroup == null)
            {
                htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex), true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            if (CurrentGroup.BondOnFrameUserRegions != null)
            {
                HOperatorSet.ConcatObj(CurrentGroup.BondOnFrameUserRegions.DisplayRegion, concatGroupRegion, out concatGroupRegion);
            }
            if (CurrentGroup.BondOnICUserRegions != null)
            {
                HOperatorSet.ConcatObj(CurrentGroup.BondOnICUserRegions.DisplayRegion, concatGroupRegion, out concatGroupRegion);
            }
            if (CurrentGroup.RefLineUserRegions != null)
            {
                HOperatorSet.ConcatObj(CurrentGroup.RefLineUserRegions.DisplayRegion, concatGroupRegion, out concatGroupRegion);
            }
            if (CurrentGroup.LineUserRegions.Count > 0)
            {
                foreach (var userRegions in CurrentGroup.LineUserRegions)
                {
                    HOperatorSet.ConcatObj(userRegions.DisplayRegion, concatGroupRegion, out concatGroupRegion);
                }
            }
            if (isDisplayImage)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                //htWindow.DisplaySingleRegion(concatGroupRegion, WireObject.Image);
                htWindow.DisplaySingleRegion(concatGroupRegion, Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex));
            }
        }

        //区域带多个套参数
        private void ExecuteAddMultiWireRegionsCommand(object parameter)
        {
            if (WireAddRegion.isRightClickWire != true) return;
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                                    WireParameter.DieImageRowOffset,
                                                                    WireParameter.DieImageColumnOffset);
                if (userRegion == null) return;
                userRegion.Index = currentGroup.LineUserRegions.Count + 1;
                userRegion.WireRegionWithPara = new WireRegionWithPara();
                currentGroup.LineUserRegions.Add(userRegion);
                DispalyGroupRegion(true);
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

        private void ExecuteSelectAllLineRegionCommand(object parameter)
        {
            if (WireAddRegion.isRightClickWire != true) return;
            if (currentGroup?.LineUserRegions.Count == 0) return;//改
            for (int i = 0; i < currentGroup.LineUserRegions.Count; i++)
            {
                currentGroup.LineUserRegions[i].IsSelected = true;
            }
        }

        //删除区域
        private void ExecuteRemoveMultiWireRegionsCommand(object parameter)
        {
            if (WireAddRegion.isRightClickWire != true) return;
            if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < currentGroup.LineUserRegions.Count; i++)
                {
                    if (currentGroup.LineUserRegions[i].IsSelected)
                    {
                        currentGroup.LineUserRegions.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        currentGroup.LineUserRegions[i].Index = i + 1;
                    }
                }
                DispalyGroupRegion(true);
            }
        }

        //修改区域
        private void ExecuteModifyMultiWireRegionsCommand(object parameter)
        {
            try
            {
                if (currentGroup.LineUserRegions == null) return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请添加金线检测区域！");
                return;
            }
            if (WireAddRegion.isRightClickWire)
            {
                WireAddRegion.isRightClickWire = false;
                htWindow.RegionType = RegionType.Null;
                try
                {
                    for (int i = 0; i < currentGroup.LineUserRegions.Count; i++)
                    {
                        if (currentGroup.LineUserRegions[i].IsSelected)
                        {
                            switch (currentGroup.LineUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(currentGroup.LineUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(currentGroup.LineUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(currentGroup.LineUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(currentGroup.LineUserRegions[i].RegionParameters[3]),
                                                                  currentGroup.LineUserRegions[i].RegionType,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (currentGroup.LineUserRegions[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                                                                   (currentGroup.LineUserRegions[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                                                                   (currentGroup.LineUserRegions[i].RegionParameters[2] - WireParameter.DieImageRowOffset),
                                                                                                   (currentGroup.LineUserRegions[i].RegionParameters[3] - WireParameter.DieImageColumnOffset),
                                                                                                out HTuple row1_Rectangle,
                                                                                                out HTuple column1_Rectangle,
                                                                                                out HTuple row2_Rectangle,
                                                                                                out HTuple column2_Rectangle);

                                    currentGroup.LineUserRegions[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.LineUserRegions[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;




                                    if (currentGroup.LineUserRegions[i].AlgoParameterIndex == 0)
                                    {
                                        userRegion.AlgoParameterIndex = 0;
                                        userRegion.WireRegionWithPara.WireThresAlgoPara.ThreshGray = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ThreshGray;
                                        userRegion.WireRegionWithPara.WireThresAlgoPara.ClosingSize = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ClosingSize;
                                        userRegion.WireRegionWithPara.WireThresAlgoPara.WireWidth = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireWidth;
                                        userRegion.WireRegionWithPara.WireThresAlgoPara.WireLength = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireLength;
                                        userRegion.WireRegionWithPara.WireThresAlgoPara.WireArea = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireArea;
                                    }
                                    else if (currentGroup.LineUserRegions[i].AlgoParameterIndex == 1)
                                    {
                                        userRegion.AlgoParameterIndex = 1;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoPara.WireWidth = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.WireWidth;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoPara.WireContrast = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.WireContrast;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoPara.LightOrDark = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.LightOrDark;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoPara.SelMetric = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMetric;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoPara.SelMin = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMin;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoPara.SelMax = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMax;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoPara.LinePhiDiff = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.LinePhiDiff;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoPara.MaxWireGap = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.MaxWireGap;
                                    }
                                    else if (currentGroup.LineUserRegions[i].AlgoParameterIndex == 2)
                                    {
                                        userRegion.AlgoParameterIndex = 2;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.WireWidth = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.WireWidth;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.WireContrast = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.WireContrast;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.LightOrDark = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.LightOrDark;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.SelMetric = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMetric;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.SelMin = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMin;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.SelMax = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMax;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.LinePhiDiff = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.LinePhiDiff;
                                        userRegion.WireRegionWithPara.WireLineGauseAlgoParaAll.MaxWireGap = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.MaxWireGap;
                                    }


                                    //// modify by wj 使用修改前设置的区域检测参数
                                    userRegion.ChannelNames = ChannelNames;
                                    userRegion.ImageIndex = WireParameter.ImageIndex;
                                    //检测区域参数赋值
                                    int ModelAlgoParameterIndex = currentGroup.LineUserRegions[i].AlgoParameterIndex;
                                    userRegion.AlgoParameterIndex = ModelAlgoParameterIndex;
                                    //
                                    WireRegionWithPara WireInspectPara = new WireRegionWithPara();
                                    WireInspectPara = WireRegionWithPara.DeepCopyByReflection(currentGroup.LineUserRegions[i].WireRegionWithPara);
                                    //modify by lht
                                    userRegion.WireRegionWithPara = WireInspectPara;
                                    //userRegion.WireRegionWithPara = new WireRegionWithPara();
                                    currentGroup.LineUserRegions[i] = userRegion;
                                    currentGroup.LineUserRegions[i].Index = i + 1;
                                    DispalyGroupRegion(true);
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(currentGroup.LineUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(currentGroup.LineUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(currentGroup.LineUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(currentGroup.LineUserRegions[i].RegionParameters[3]),
                                                                  currentGroup.LineUserRegions[i].RegionType,
                                                                  WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(currentGroup.LineUserRegions[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                                   Math.Floor(currentGroup.LineUserRegions[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                                   currentGroup.LineUserRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(currentGroup.LineUserRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(currentGroup.LineUserRegions[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    currentGroup.LineUserRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.LineUserRegions[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         WireParameter.DieImageRowOffset,
                                                                                                         WireParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;


                                    if (currentGroup.LineUserRegions[i].AlgoParameterIndex == 0)
                                    {
                                        userRegion_Rectangle2.AlgoParameterIndex = 0;
                                        userRegion_Rectangle2.WireRegionWithPara = new WireRegionWithPara();
                                        userRegion_Rectangle2.WireRegionWithPara.WireThresAlgoPara.ThreshGray = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ThreshGray;
                                        userRegion_Rectangle2.WireRegionWithPara.WireThresAlgoPara.ClosingSize = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ClosingSize;
                                        userRegion_Rectangle2.WireRegionWithPara.WireThresAlgoPara.WireWidth = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireWidth;
                                        userRegion_Rectangle2.WireRegionWithPara.WireThresAlgoPara.WireLength = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireLength;
                                        userRegion_Rectangle2.WireRegionWithPara.WireThresAlgoPara.WireArea = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireArea;
                                    }
                                    else if (currentGroup.LineUserRegions[i].AlgoParameterIndex == 1)
                                    {
                                        userRegion_Rectangle2.AlgoParameterIndex = 1;
                                        userRegion_Rectangle2.WireRegionWithPara = new WireRegionWithPara();
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoPara.WireWidth = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.WireWidth;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoPara.WireContrast = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.WireContrast;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoPara.LightOrDark = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.LightOrDark;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoPara.SelMetric = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMetric;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoPara.SelMin = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMin;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoPara.SelMax = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMax;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoPara.LinePhiDiff = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.LinePhiDiff;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoPara.MaxWireGap = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.MaxWireGap;
                                    }
                                    else if (currentGroup.LineUserRegions[i].AlgoParameterIndex == 2)
                                    {
                                        userRegion_Rectangle2.AlgoParameterIndex = 2;
                                        userRegion_Rectangle2.WireRegionWithPara = new WireRegionWithPara();
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoParaAll.WireWidth = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.WireWidth;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoParaAll.WireContrast = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.WireContrast;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoParaAll.LightOrDark = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.LightOrDark;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoParaAll.SelMetric = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMetric;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoParaAll.SelMin = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMin;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoParaAll.SelMax = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMax;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoParaAll.LinePhiDiff = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.LinePhiDiff;
                                        userRegion_Rectangle2.WireRegionWithPara.WireLineGauseAlgoParaAll.MaxWireGap = CurrentGroup.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.MaxWireGap;
                                    }

                                    //// modify by wj 使用修改前设置的区域检测参数
                                    userRegion_Rectangle2.ChannelNames = ChannelNames;
                                    userRegion_Rectangle2.ImageIndex = WireParameter.ImageIndex;
                                    //检测区域参数赋值
                                    int AlgoParameterIndex = currentGroup.LineUserRegions[i].AlgoParameterIndex;
                                    userRegion_Rectangle2.AlgoParameterIndex = AlgoParameterIndex;
                                    //
                                    WireRegionWithPara WireInspectParas = new WireRegionWithPara();
                                    WireInspectParas = WireRegionWithPara.DeepCopyByReflection(currentGroup.LineUserRegions[i].WireRegionWithPara);
                                    //modify by lht
                                    userRegion_Rectangle2.WireRegionWithPara = WireInspectParas;

                                    //userRegion_Rectangle2.WireRegionWithPara = new WireRegionWithPara();
                                    currentGroup.LineUserRegions[i] = userRegion_Rectangle2;
                                    currentGroup.LineUserRegions[i].Index = i + 1;
                                    DispalyGroupRegion(true);
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((currentGroup.LineUserRegions[i].RegionParameters[0]),
                                                                 (currentGroup.LineUserRegions[i].RegionParameters[1]),
                                                                 (currentGroup.LineUserRegions[i].RegionParameters[2]),
                                                                 0,
                                                                 currentGroup.LineUserRegions[i].RegionType,
                                                                 WireParameter.DieImageRowOffset, WireParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (currentGroup.LineUserRegions[i].RegionParameters[0] - WireParameter.DieImageRowOffset),
                                                               (currentGroup.LineUserRegions[i].RegionParameters[1] - WireParameter.DieImageColumnOffset),
                                                               currentGroup.LineUserRegions[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    currentGroup.LineUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     currentGroup.LineUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     WireParameter.DieImageRowOffset,
                                                                                                     WireParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    userRegion_Circle.WireRegionWithPara = new WireRegionWithPara();
                                    currentGroup.LineUserRegions[i] = userRegion_Circle;
                                    currentGroup.LineUserRegions[i].Index = i + 1;
                                    DispalyGroupRegion(true);
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
                    WireAddRegion.isRightClickWire = true;
                }
            }
        }

        private void ExecuteClickResponseCommand(object parameter)
        {
            UserRegion userRegion = parameter as UserRegion;
            SelectedUserRegion = userRegion;
        }
        private void OnItemMouseDoubleClick(object sender, RoutedEventArgs e)
        {

        }


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
            WireParameter.IsWireRegionPickUp = false;//1028
            WireParameter.IsStartPickUp = false;//1028
            WireParameter.IsStopPickUp = false;//1028
            WireParameter.IsWirePickUp = false;//1028
            htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(WireObject.DieImage, WireParameter.ImageIndex));
            //DispalyGroupRegion();
            DispalyGroupRegionAll(true);//

            //1123
            ChannelNames = new ObservableCollection<ChannelName>(WireParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = WireParameter.ImageIndex;
            OnPropertyChanged("ImageIndex");

        }

        public void Dispose()
        {
            (Content as Page_WireAddRegion).DataContext = null;
            (Content as Page_WireAddRegion).Close();
            Content = null;
            isRightClickWire = true;
            this.htWindow = null;
            this.Groups = null;
            this.WireObject = null;
            this.WireParameter = null;
            AddGroupCommand = null;
            RemoveGroupCommand = null;
            AddBondOnFrameUserRegionCommand = null;
            AddBondOnICUserRegionCommand = null;
            AddRefWireUserRegionCommand = null;
            ModifyBondOnFrameRegionCommand = null;
            ModifyBondOnICRegionCommand = null;
            DisplayAllRegionCommand = null;
        }
    }
}
