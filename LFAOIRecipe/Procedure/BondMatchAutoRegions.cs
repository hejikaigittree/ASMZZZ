using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace LFAOIRecipe
{
    public class BondMatchAutoRegions : ViewModelBase, IProcedure
    {
        public string DisplayName { get; }

        public object Content { get; private set; }

        public bool isRightClick = true;

        private readonly string ModelsFile;

        private readonly string RecipeFile;

        private readonly string BondDirectory;

        public ObservableCollection<UserRegion> Bond2AutoUserRegion { get; private set; }

        private BondMatchAutoRegionGroup currentGroup;
        public BondMatchAutoRegionGroup CurrentGroup
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

        private bool? isCheckAll;
        public bool? IsCheckAll
        {
            get => isCheckAll;
            set => OnPropertyChanged(ref isCheckAll, value);
        }

        public ObservableCollection<BondMatchAutoRegionGroup> Groups { get; private set; }

        public ObservableCollection<UserRegion> SortBond2AutoUserRegion { get; private set; } = new ObservableCollection<UserRegion>();

        public CommandBase AddGroupCommand { get; private set; }
        public CommandBase RemoveGroupCommand { get; private set; }
        public CommandBase AddBond2UserRegionCommand { get; private set; }
        public CommandBase AddWireUserRegionCommand { get; private set; }
        public CommandBase ModifyBond2RegionCommand { get; private set; }
        public CommandBase ModifyWireRegionCommand { get; private set; }
        public CommandBase DisplayAllRegionCommand { get; private set; }
        public CommandBase CreateAutoBondUserRegionCommand { get; private set; }
        public CommandBase TextChangedCommand { get; private set; }
        public CommandBase SortCommand { get; private set; }
        public CommandBase AddBond2AutoUserRegionCommand { get; private set; }
        public CommandBase ModifyBond2AutoRegionCommand { get; private set; }
        public CommandBase RemoveBond2AutoUserRegionCommand { get; private set; }
        public CommandBase DisplayGroupsRegionsCommand { get; private set; }
        public CommandBase IsCheckCommand { get; private set; }
        public CommandBase IsCheckAllCommand { get; private set; }

        private HTHalControlWPF htWindow;

        HTuple BondRows, BondCols, BondAngles = null;

        private Bond2ModelObject bond2ModelObject;

        private string ReferenceDirectory { get; set; }

        public Bond2ModelParameter Bond2ModelParameter { get; set; }

        public BondWireParameter BondWireParameter { get; set; }

        public BondAutoRegionsParameter BondAutoRegionsParameter { get; set; }

        public BondMatchAutoRegions(HTHalControlWPF htWindow,
                               string modelsFile,
                               string recipeFile,
                               ObservableCollection<BondMatchAutoRegionGroup> groups,
                               Bond2ModelObject bond2ModelObject,
                               Bond2ModelParameter bond2ModelParameter,
                               BondWireParameter bondWireParameter,
                               BondAutoRegionsParameter bondAutoRegionsParameter,
                               ObservableCollection<UserRegion> bond2AutoUserRegion,
                               string bondDirectory,
                               string referenceDirectory)
        {
            DisplayName = "自动生成焊点区域";
            Content = new Page_AddBondMatchAutoRegions { DataContext = this };
            this.htWindow = htWindow;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            Groups = groups;
            this.bond2ModelObject = bond2ModelObject;
            this.Bond2ModelParameter = bond2ModelParameter;
            this.BondWireParameter = bondWireParameter;
            this.BondAutoRegionsParameter = bondAutoRegionsParameter;
            this.Bond2AutoUserRegion = bond2AutoUserRegion;
            this.BondDirectory = bondDirectory;
            this.ReferenceDirectory = referenceDirectory;
            AddGroupCommand = new CommandBase(ExecuteAddGroupCommand);
            RemoveGroupCommand = new CommandBase(ExecuteRemoveGroupCommand);
            AddBond2UserRegionCommand = new CommandBase(ExecuteAddBond2UserRegionCommand);
            AddWireUserRegionCommand = new CommandBase(ExecuteAddWireUserRegionCommand);
            ModifyBond2RegionCommand = new CommandBase(ExecuteModifyBond2RegionCommand);
            ModifyWireRegionCommand = new CommandBase(ExecuteModifyWireRegionCommand);
            DisplayAllRegionCommand = new CommandBase(ExecuteDisplayAllRegionCommand);
            CreateAutoBondUserRegionCommand = new CommandBase(ExecuteCreateAutoBondUserRegionCommand);
            TextChangedCommand = new CommandBase(ExecuteTextChangedCommand);
            SortCommand = new CommandBase(ExecuteSortCommand);
            AddBond2AutoUserRegionCommand = new CommandBase(ExecuteAddBond2AutoUserRegionCommand);
            ModifyBond2AutoRegionCommand = new CommandBase(ExecuteModifyBond2AutoRegionCommand);
            RemoveBond2AutoUserRegionCommand = new CommandBase(ExecuteRemoveBond2AutoUserRegionCommand);
            DisplayGroupsRegionsCommand = new CommandBase(ExecuteDisplayGroupsRegionsCommand);
            IsCheckAllCommand = new CommandBase(ExecuteIsCheckAllCommand);
            IsCheckCommand = new CommandBase(ExecuteIsCheckCommand);

            CurrentGroup = new BondMatchAutoRegionGroup
            {
                Index = Groups.Count + 1,
                Bond2_BallNums = 15
            };
            Groups.Add(CurrentGroup);
            GroupsCount = Groups.Count;
        }

        private void ExecuteCreateAutoBondUserRegionCommand(object parameter)
        {
            HOperatorSet.ReadRegion(out HObject DieRegion, $"{ReferenceDirectory}DieReference.reg");
            HTuple modelID = new HTuple();

            if (bond2ModelObject.ModelID != null)
            {
                modelID = bond2ModelObject.ModelID;
            }
            else if (File.Exists($"{BondDirectory}PosModel.dat"))
            {
                modelID = Algorithm.File.ReadModel($"{BondDirectory}PosModel.dat", Bond2ModelParameter.ModelType);
            }
            else
            {
                MessageBox.Show("请先生成匹配模板！");
                return;
            }
            if (modelID.TupleLength() > 1)
            {
                MessageBox.Show("暂时不支持多套模板的检测验证！");
                return;
            }

            Algorithm.Model_RegionAlg.HTV_confirm_mapping_matrix(Algorithm.Region.GetChannnelImageConcact(bond2ModelObject.Image),
                   DieRegion,
                   ModelsFile,
                   RecipeFile,
                   Bond2ModelParameter.OnRecipesIndexs[Bond2ModelParameter.OnRecipesIndex],
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

            Algorithm.Model_RegionAlg.HTV_Gen_Bond_Region(Algorithm.Region.GetChannnelImageUpdate(bond2ModelObject.Image, Bond2ModelParameter.ImageChannelIndex), // 1203 mod lw
                            Algorithm.Region.ConcatRegion(Groups.Select(r => r.Bond2UserRegion).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                            out HObject BondContours,
                            HomMat2D,
                            BondNum,
                            Bond2ModelParameter.ModelType == 0 ? "ncc" : "shape",
                            modelID,
                            BondAutoRegionsParameter.MinMatchScore,//0.65
                            LineAngle,
                            BondAutoRegionsParameter.AngleExt,//界面设置一个值  
                            BondAutoRegionsParameter.BondSize,//18  
                            out BondRows,
                            out BondCols,
                            out BondAngles,
                            out HTuple hv_o_ErrCode, out HTuple hv_o_ErrStr);

            if (BondContours.CountObj() == 0)
            {
                MessageBox.Show("自动生成焊点数为0！");
                return;
            }

            Bond2AutoUserRegion.Clear();
            //htWindow.DisplaySingleRegion(BondContours.ConcatObj(BondRegs).ConcatObj(BondLines));
            for (int i = 0; i < BondContours.CountObj(); i++)
            {
                UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Rectangle2,
                        BondRows[i] - Bond2ModelParameter.DieImageRowOffset,
                        BondCols[i] - Bond2ModelParameter.DieImageColumnOffset,
                        BondAutoRegionsParameter.Length1,
                        BondAutoRegionsParameter.Length2,
                        Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, BondAngles[i]);
                if (userRegion_Rectangle2 == null) return;
                userRegion_Rectangle2.Index = Bond2AutoUserRegion.Count + 1;
                Bond2AutoUserRegion.Add(userRegion_Rectangle2);
            }

            //htWindow.DisplayMultiRegionWithIndex(Bond2AutoUserRegion);

            DispalyGroupsRegions(Bond2ModelParameter.ImageChannelIndex);
        }

        public void GetClickDownPoints()
        {
            if (Bond2AutoUserRegion.Count == 0 || Bond2ModelParameter.IsPickUp == false) return;

            HOperatorSet.GenRegionPoints(out HObject Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
            foreach (var item in Bond2AutoUserRegion)
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

        private void ExecuteTextChangedCommand(object parameter)
        {
            if (isRightClick)
            {
                UserRegion userRegion = parameter as UserRegion;
                if (userRegion == null) return;
                UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Rectangle2,
                            userRegion.RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset,
                            userRegion.RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset,
                            BondAutoRegionsParameter.Length1, BondAutoRegionsParameter.Length2,
                            Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, userRegion.RegionParameters[2]);
                if (userRegion_Rectangle2 == null) return;
                userRegion_Rectangle2.Index = userRegion.Index;
                Bond2AutoUserRegion[userRegion.Index - 1] = userRegion_Rectangle2;
                DispalyGroupsRegions(Bond2ModelParameter.ImageChannelIndex);
            }
        }

        private void DispalyGroupsRegions(int ImageIndex, bool isHTWindowRegion = true)
        {
            HObject ChannelDieImageDisply = null;

            if (bond2ModelObject.DieImage == null) return;

            // 1122-lw
            if (ImageIndex < 0)
            {
                htWindow.Display(bond2ModelObject.DieImage, true);
                return;
            }

            if (Bond2AutoUserRegion.Count == 0)
            {
                HOperatorSet.AccessChannel(bond2ModelObject.DieImage, out ChannelDieImageDisply, ImageIndex + 1);
                htWindow.Display(ChannelDieImageDisply, true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            foreach (var item in Bond2AutoUserRegion)
            {
                HOperatorSet.ConcatObj(concatGroupRegion, item.DisplayRegion, out concatGroupRegion);
                HOperatorSet.TupleSin(item.RegionParameters[2], out HTuple sin_out_line);
                HTuple line_r = item.RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset - 2 * BondAutoRegionsParameter.Length1 * sin_out_line;
                HOperatorSet.TupleCos(item.RegionParameters[2], out HTuple cos_out_line);
                HTuple line_c = item.RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset + 2 * BondAutoRegionsParameter.Length1 * cos_out_line;
                UserRegion userRegion_line = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Line,
                        item.RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset,
                        item.RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset,
                        line_r, line_c, Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, item.RegionParameters[2]);
                HOperatorSet.ConcatObj(concatGroupRegion, userRegion_line.DisplayRegion, out concatGroupRegion);
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                HOperatorSet.AccessChannel(bond2ModelObject.DieImage, out ChannelDieImageDisply, ImageIndex + 1);
                htWindow.DisplaySingleRegion(concatGroupRegion, ChannelDieImageDisply);
            }

            foreach (var item in Bond2AutoUserRegion)
            {
                HOperatorSet.TupleSin(item.RegionParameters[2], out HTuple sin_out_line);
                HTuple line_r = item.RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset - 2 * BondAutoRegionsParameter.Length1 * sin_out_line;
                HOperatorSet.TupleCos(item.RegionParameters[2], out HTuple cos_out_line);
                HTuple line_c = item.RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset + 2 * BondAutoRegionsParameter.Length1 * cos_out_line;

                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, line_r - 20, line_c + 5);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }

        private void ExecuteDisplayGroupsRegionsCommand(object parameter)
        {
            if (isRightClick != true) return;
            DispalyGroupsRegions(Bond2ModelParameter.ImageChannelIndex);
        }

        //排序
        private void ExecuteSortCommand(object parameter)
        {
            if (Bond2AutoUserRegion.Count() == 0) return;

            switch (BondAutoRegionsParameter.SortMethod)
            {
                case 0:
                    ObservableCollection<UserRegion> Bond2AutoUserRegionSort = new ObservableCollection<UserRegion>(Bond2AutoUserRegion.OrderBy(r => r.Index));
                    Bond2AutoUserRegion.Clear();
                    for (int i = 0; i < Bond2AutoUserRegionSort.Count(); i++)
                    {
                        Bond2AutoUserRegion.Add(Bond2AutoUserRegionSort[i]);
                    }
                    break;

                case 1:
                    //列升序
                    ObservableCollection<UserRegion> Bond2AutoUserRegionSort1 = new ObservableCollection<UserRegion>(Bond2AutoUserRegion.OrderBy(r => r.RegionParameters[1]));
                    Bond2AutoUserRegion.Clear();
                    for (int i = 0; i < Bond2AutoUserRegionSort1.Count(); i++)
                    {
                        Bond2AutoUserRegion.Add(Bond2AutoUserRegionSort1[i]);
                        Bond2AutoUserRegionSort1[i].Index = Bond2AutoUserRegion.Count;
                    }
                    break;

                case 2:
                    //列降序
                    ObservableCollection<UserRegion> Bond2AutoUserRegionSort2 = new ObservableCollection<UserRegion>(Bond2AutoUserRegion.OrderByDescending(r => r.RegionParameters[1]));
                    Bond2AutoUserRegion.Clear();
                    for (int i = 0; i < Bond2AutoUserRegionSort2.Count(); i++)
                    {
                        Bond2AutoUserRegion.Add(Bond2AutoUserRegionSort2[i]);
                        Bond2AutoUserRegionSort2[i].Index = Bond2AutoUserRegion.Count;
                    }
                    break;

                case 3:
                    //行升序
                    ObservableCollection<UserRegion> Bond2AutoUserRegionSort3 = new ObservableCollection<UserRegion>(Bond2AutoUserRegion.OrderBy(r => r.RegionParameters[0]));
                    Bond2AutoUserRegion.Clear();
                    for (int i = 0; i < Bond2AutoUserRegionSort3.Count(); i++)
                    {
                        Bond2AutoUserRegion.Add(Bond2AutoUserRegionSort3[i]);
                        Bond2AutoUserRegionSort3[i].Index = Bond2AutoUserRegion.Count;
                    }
                    break;

                case 4:
                    //行降序
                    ObservableCollection<UserRegion> Bond2AutoUserRegionSort4 = new ObservableCollection<UserRegion>(Bond2AutoUserRegion.OrderByDescending(r => r.RegionParameters[0]));
                    Bond2AutoUserRegion.Clear();
                    for (int i = 0; i < Bond2AutoUserRegionSort4.Count(); i++)
                    {
                        Bond2AutoUserRegion.Add(Bond2AutoUserRegionSort4[i]);
                        Bond2AutoUserRegionSort4[i].Index = Bond2AutoUserRegion.Count;
                    }
                    break;

                case 5:
                    //顺时针
                    if (BondAutoRegionsParameter.FirstSortNumber == -1)
                    {
                        MessageBox.Show("请选择排序起始焊点！");
                        return;
                    }
                    HTuple BondRows = new HTuple();
                    HTuple BondCols = new HTuple();
                    HTuple BondAngles = new HTuple();

                    foreach (var item in Bond2AutoUserRegion)
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
                                    BondAutoRegionsParameter.FirstSortNumber,
                                    0,
                                    out HTuple SortRows,
                                    out HTuple SortCols,
                                    out HTuple SortAngles,
                                    out HTuple SortIndex);
                    Bond2AutoUserRegion.Clear();

                    for (int i = 0; i < SortRows.TupleLength(); i++)
                    {
                        UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Rectangle2,
                                SortRows[i] - Bond2ModelParameter.DieImageRowOffset,
                                SortCols[i] - Bond2ModelParameter.DieImageColumnOffset,
                                BondAutoRegionsParameter.Length1,
                                BondAutoRegionsParameter.Length2, Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset,
                                SortAngles[i]);
                        if (userRegion_Rectangle2 == null) return;
                        userRegion_Rectangle2.Index = Bond2AutoUserRegion.Count + 1;
                        Bond2AutoUserRegion.Add(userRegion_Rectangle2);
                    }
                    break;

                case 6:
                    //逆时针
                    if (BondAutoRegionsParameter.FirstSortNumber == -1)
                    {
                        MessageBox.Show("请选择排序起始焊点！");
                        return;
                    }
                    HTuple AntiBondRows = new HTuple();
                    HTuple AntiBondCols = new HTuple();
                    HTuple AntiBondAngles = new HTuple();

                    foreach (var item in Bond2AutoUserRegion)
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
                                    BondAutoRegionsParameter.FirstSortNumber,
                                    1,
                                    out HTuple SortAntiRows,
                                    out HTuple SortAntiCols,
                                    out HTuple SortAntiAngles,
                                    out HTuple SortAntiIndex);
                    Bond2AutoUserRegion.Clear();
                    for (int i = 0; i < SortAntiRows.TupleLength(); i++)
                    {
                        UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Rectangle2,
                                SortAntiRows[i] - Bond2ModelParameter.DieImageRowOffset,
                                SortAntiCols[i] - Bond2ModelParameter.DieImageColumnOffset,
                                BondAutoRegionsParameter.Length1,
                                BondAutoRegionsParameter.Length2, Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset,
                                SortAntiAngles[i]);
                        if (userRegion_Rectangle2 == null) return;
                        userRegion_Rectangle2.Index = Bond2AutoUserRegion.Count + 1;
                        Bond2AutoUserRegion.Add(userRegion_Rectangle2);
                    }
                    break;

                default:
                    break;
            }
            BondAutoRegionsParameter.FirstSortNumber = 0; // 刷新排序后FirstSortNumber默认为1
            DispalyGroupsRegions(Bond2ModelParameter.ImageChannelIndex);
        }

        private void ExecuteAddGroupCommand(object parameter)
        {
            if (isRightClick != true) return;
            CurrentGroup = new BondMatchAutoRegionGroup
            {
                Index = Groups.Count + 1,
                Bond2_BallNums = 15
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

        //添加
        private void ExecuteAddBond2UserRegionCommand(object parameter)
        {
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
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                            Bond2ModelParameter.DieImageRowOffset,
                                                            Bond2ModelParameter.DieImageColumnOffset);
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

        //修改
        private void ExecuteModifyBond2RegionCommand(object parameter)
        {
            if (currentGroup.Bond2UserRegion == null) return;
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
                                                          Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, "yellow");

                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (currentGroup.Bond2UserRegion.RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset),
                                                                                          (currentGroup.Bond2UserRegion.RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset),
                                                                                          (currentGroup.Bond2UserRegion.RegionParameters[2] - Bond2ModelParameter.DieImageRowOffset),
                                                                                          (currentGroup.Bond2UserRegion.RegionParameters[3] - Bond2ModelParameter.DieImageColumnOffset),
                                                                                        out HTuple row1_Rectangle,
                                                                                        out HTuple column1_Rectangle,
                                                                                        out HTuple row2_Rectangle,
                                                                                        out HTuple column2_Rectangle);

                            currentGroup.Bond2UserRegion.RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.Bond2UserRegion.RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset);
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
                                                          Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, "yellow");

                            HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                           Math.Floor(currentGroup.Bond2UserRegion.RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset),
                                                           Math.Floor(currentGroup.Bond2UserRegion.RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset),
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
                                                                                                 Bond2ModelParameter.DieImageRowOffset,
                                                                                                 Bond2ModelParameter.DieImageColumnOffset,
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

        private void ExecuteAddWireUserRegionCommand(object parameter)
        {
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

                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                                Bond2ModelParameter.DieImageRowOffset,
                                                                Bond2ModelParameter.DieImageColumnOffset);
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

        private void ExecuteModifyWireRegionCommand(object parameter)//
        {
            try
            {
                if (currentGroup.WireUserRegion == null) return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请添加中继点区域");
                return;
            }
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
                            Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, "yellow");
                            HOperatorSet.DrawLineMod(htWindow.hTWindow.HalconWindow, currentGroup.WireUserRegion.RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset,
                                                                                           currentGroup.WireUserRegion.RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset,
                                                                                           currentGroup.WireUserRegion.RegionParameters[2] - Bond2ModelParameter.DieImageRowOffset,
                                                                                           currentGroup.WireUserRegion.RegionParameters[3] - Bond2ModelParameter.DieImageColumnOffset,
                                                       out HTuple row1_Line,
                                                       out HTuple column1_Line,
                                                       out HTuple row2_Line,
                                                       out HTuple column2_Line);

                            currentGroup.WireUserRegion.RegionType = RegionType.Line;
                            UserRegion userRegion_Line = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.WireUserRegion.RegionType, row1_Line, column1_Line, row2_Line, column2_Line, Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset);
                            if (userRegion_Line == null) return;
                            currentGroup.WireUserRegion = userRegion_Line;
                            DispalyGroupRegion(true);
                            break;

                        case RegionType.Rectangle1:
                            htWindow.InitialHWindowUpdate(Math.Floor(currentGroup.WireUserRegion.RegionParameters[0]),
                                                 Math.Floor(currentGroup.WireUserRegion.RegionParameters[1]),
                                                 Math.Ceiling(currentGroup.WireUserRegion.RegionParameters[2]),
                                                 Math.Ceiling(currentGroup.WireUserRegion.RegionParameters[3]), currentGroup.WireUserRegion.RegionType,
                                                 Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, "yellow");
                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (currentGroup.WireUserRegion.RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset),
                                                                                          (currentGroup.WireUserRegion.RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset),
                                                                                          (currentGroup.WireUserRegion.RegionParameters[2] - Bond2ModelParameter.DieImageRowOffset),
                                                                                          (currentGroup.WireUserRegion.RegionParameters[3] - Bond2ModelParameter.DieImageColumnOffset),
                                                       out HTuple row1_Rectangle,
                                                       out HTuple column1_Rectangle,
                                                       out HTuple row2_Rectangle,
                                                       out HTuple column2_Rectangle);

                            currentGroup.WireUserRegion.RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, currentGroup.WireUserRegion.RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset);
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

        //添加
        private void ExecuteAddBond2AutoUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (htWindow.RegionType == RegionType.Null)
            {
                System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                         Bond2ModelParameter.DieImageRowOffset,
                                                         Bond2ModelParameter.DieImageColumnOffset);
                if (userRegion == null) return;
                userRegion.Index = Bond2AutoUserRegion.Count + 1;
                Bond2AutoUserRegion.Add(userRegion);
                DispalyGroupsRegions(Bond2ModelParameter.ImageChannelIndex);
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

        //删除
        private void ExecuteRemoveBond2AutoUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < Bond2AutoUserRegion.Count; i++)
                {
                    if (Bond2AutoUserRegion[i].IsSelected)
                    {
                        Bond2AutoUserRegion.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        Bond2AutoUserRegion[i].Index = i + 1;
                    }
                }
                DispalyGroupsRegions(Bond2ModelParameter.ImageChannelIndex);
                if (Bond2AutoUserRegion.Count == 0)
                {
                    IsCheckAll = false;
                }
            }
        }

        //修改
        private void ExecuteModifyBond2AutoRegionCommand(object parameter)//
        {
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    for (int i = 0; i < Bond2AutoUserRegion.Count; i++)
                    {
                        if (Bond2AutoUserRegion[i].IsSelected)
                        {
                            switch (Bond2AutoUserRegion[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Line:

                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(Bond2AutoUserRegion[i].RegionParameters[0]),
                                                                  Math.Floor(Bond2AutoUserRegion[i].RegionParameters[1]),
                                                                  Math.Ceiling(Bond2AutoUserRegion[i].RegionParameters[2]),
                                                                  Math.Ceiling(Bond2AutoUserRegion[i].RegionParameters[3]),
                                                                  Bond2AutoUserRegion[i].RegionType,
                                                                  Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (Bond2AutoUserRegion[i].RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset),
                                                                                                  (Bond2AutoUserRegion[i].RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset),
                                                                                                  (Bond2AutoUserRegion[i].RegionParameters[2] - Bond2ModelParameter.DieImageRowOffset),
                                                                                                  (Bond2AutoUserRegion[i].RegionParameters[3] - Bond2ModelParameter.DieImageColumnOffset),
                                                        out HTuple row1_Rectangle,
                                                        out HTuple column1_Rectangle,
                                                        out HTuple row2_Rectangle,
                                                        out HTuple column2_Rectangle);

                                    Bond2AutoUserRegion[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, Bond2AutoUserRegion[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    Bond2AutoUserRegion[i] = userRegion;
                                    Bond2AutoUserRegion[i].Index = i + 1;
                                    DispalyGroupsRegions(Bond2ModelParameter.ImageChannelIndex);
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(Bond2AutoUserRegion[i].RegionParameters[0]),
                                                                  Math.Floor(Bond2AutoUserRegion[i].RegionParameters[1]),
                                                                  Math.Ceiling(Bond2AutoUserRegion[i].RegionParameters[2]),
                                                                  Math.Ceiling(Bond2AutoUserRegion[i].RegionParameters[3]),
                                                                  Bond2AutoUserRegion[i].RegionType,
                                                                  Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(Bond2AutoUserRegion[i].RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset),
                                                                   Math.Floor(Bond2AutoUserRegion[i].RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset),
                                                                   Bond2AutoUserRegion[i].RegionParameters[2],
                                                                   Math.Ceiling(Bond2AutoUserRegion[i].RegionParameters[3]),
                                                                   Math.Ceiling(Bond2AutoUserRegion[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    Bond2AutoUserRegion[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, Bond2AutoUserRegion[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         Bond2ModelParameter.DieImageRowOffset,
                                                                                                         Bond2ModelParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    Bond2AutoUserRegion[i] = userRegion_Rectangle2;
                                    Bond2AutoUserRegion[i].Index = i + 1;
                                    DispalyGroupsRegions(Bond2ModelParameter.ImageChannelIndex);
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((Bond2AutoUserRegion[i].RegionParameters[0]),
                                                                  (Bond2AutoUserRegion[i].RegionParameters[1]),
                                                                  (Bond2AutoUserRegion[i].RegionParameters[2]),
                                                                  0,
                                                                  Bond2AutoUserRegion[i].RegionType,
                                                                  Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (Bond2AutoUserRegion[i].RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset),
                                                               (Bond2AutoUserRegion[i].RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset),
                                                               Bond2AutoUserRegion[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    Bond2AutoUserRegion[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     Bond2AutoUserRegion[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     Bond2ModelParameter.DieImageRowOffset,
                                                                                                     Bond2ModelParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    Bond2AutoUserRegion[i] = userRegion_Circle;
                                    Bond2AutoUserRegion[i].Index = i + 1;
                                    DispalyGroupsRegions(Bond2ModelParameter.ImageChannelIndex);
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
                    isRightClick = true;
                }
            }
        }

        private void ExecuteDisplayAllRegionCommand(object parameter)
        {
            if (isRightClick != true) return;
            DispalyGroupRegionAll();
        }

        private void DispalyGroupRegionAll(bool isHTWindowRegion = true)
        {
            if (GroupsCount == 0)
            {
                htWindow.Display(bond2ModelObject.DieImage, true);
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
                htWindow.DisplayMultiRegion(concatGroupRegion, bond2ModelObject.DieImage);
            }
        }

        private void DispalyGroupRegion(bool isHTWindowRegion = true)
        {
            if (CurrentGroup == null)
            {
                htWindow.Display(bond2ModelObject.DieImage, true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            if (CurrentGroup.Bond2UserRegion != null)
            {
                HOperatorSet.ConcatObj(CurrentGroup.Bond2UserRegion.DisplayRegion, concatGroupRegion, out concatGroupRegion);
            }
            if (CurrentGroup.WireUserRegion != null)
            {
                HOperatorSet.ConcatObj(CurrentGroup.WireUserRegion.DisplayRegion, concatGroupRegion, out concatGroupRegion);
            }
            if (isHTWindowRegion)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                htWindow.DisplaySingleRegion(concatGroupRegion, bond2ModelObject.DieImage);
            }
        }

        private void ExecuteIsCheckCommand(object parameter)
        {
            if (Bond2AutoUserRegion.All(x => x.IsSelected == true))
            { IsCheckAll = true; }
            else if (Bond2AutoUserRegion.All(x => !x.IsSelected))
            { IsCheckAll = false; }
            else
            { IsCheckAll = null; }
        }

        private void ExecuteIsCheckAllCommand(object parameter)
        {
            if (IsCheckAll == true)
            { Bond2AutoUserRegion.ToList().ForEach(r => r.IsSelected = true); }
            else if (IsCheckAll == false)
            { Bond2AutoUserRegion.ToList().ForEach(r => r.IsSelected = false); }
        }

        public bool CheckCompleted()
        {
            //foreach (var group in Groups)
            //{
            //    if (group.Bond2UserRegion != null)
            //    {
            //        MessageBox.Show($"序号 {group.Index.ToString()} 的焊点金线组合的第二焊点区域为空，请选择");
            //        return false;
            //    }
            //    if (group.Bond1UserRegion != null)
            //    {
            //        MessageBox.Show($"序号 {group.Index.ToString()} 的焊点金线组合的第一焊点区域为空，请选择");
            //        return false;
            //    }
            //    if (group.WireUserRegion != null)
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
            DispalyGroupsRegions(Bond2ModelParameter.ImageChannelIndex, false);
        }

        public void Dispose()
        {
            (Content as Page_AddBondMatchAutoRegions).DataContext = null;
            (Content as Page_AddBondMatchAutoRegions).Close();
            Content = null;
            this.htWindow = null;
            this.Groups = null;
            this.bond2ModelObject = null;
            AddGroupCommand = null;
            RemoveGroupCommand = null;
            Bond2AutoUserRegion = null;
            SortBond2AutoUserRegion = null;
            BondAutoRegionsParameter = null;
            Bond2ModelParameter = null;
            BondWireParameter = null;
            AddBond2UserRegionCommand = null;
            AddWireUserRegionCommand = null;
            ModifyBond2RegionCommand = null;
            ModifyWireRegionCommand = null;
            DisplayAllRegionCommand = null;
            CreateAutoBondUserRegionCommand = null;
            TextChangedCommand = null;
            SortCommand = null;
            AddBond2AutoUserRegionCommand = null;
            ModifyBond2AutoRegionCommand = null;
            RemoveBond2AutoUserRegionCommand = null;
            DisplayGroupsRegionsCommand = null;
            IsCheckCommand = null;
            IsCheckAllCommand = null;
        }
    }
}
