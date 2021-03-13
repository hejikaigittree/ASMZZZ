using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using HalconDotNet;
using System.IO;
using System.Windows.Controls;

namespace LFAOIRecipe
{
    public class AddBondMatchRegions : ViewModelBase, IProcedure
    {
        public string DisplayName { get; }

        public object Content { get; private set; }

        public bool isRightClick = true;

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
                    DispalyGroupRegion(Bond2ModelParameter.ImageChannelIndex);
                }
            }
        }

        private int groupsCount;
        public int GroupsCount
        {
            get => groupsCount;
            set => OnPropertyChanged(ref groupsCount, value);
        }

        public ObservableCollection<BondWireRegionGroup> Groups { get; private set; }

        public CommandBase AddGroupCommand { get; private set; }
        public CommandBase RemoveGroupCommand { get; private set; }
        public CommandBase AddBond2UserRegionCommand { get; private set; }
        public CommandBase AddWireUserRegionCommand { get; private set; }
        public CommandBase ModifyBond2RegionCommand { get; private set; }
        public CommandBase ModifyWireRegionCommand { get; private set; }
        public CommandBase DisplayAllRegionCommand { get; private set; }
        public CommandBase LoadAutoMatchBondCommand { get; private set; }

        private HTHalControlWPF htWindow;

        private Bond2ModelObject bond2ModelObject;

        private string ReferenceDirectory { get; set; }

        private Bond2ModelParameter Bond2ModelParameter { get; set; }

        private bool? isCheckAll;
        public bool? IsCheckAll
        {
            get => isCheckAll;
            set => OnPropertyChanged(ref isCheckAll, value);
        }

        public ObservableCollection<UserRegion> Bond2AutoUserRegion { get; private set; }

        public AddBondMatchRegions(HTHalControlWPF htWindow,
                               ObservableCollection<BondWireRegionGroup> groups,
                               Bond2ModelObject bond2ModelObject,
                               Bond2ModelParameter bond2ModelParameter,
                               ObservableCollection<UserRegion> bond2AutoUserRegion,
                               string referenceDirectory)
        {
            DisplayName = "添加焊点检测区域";
            Content = new Page_AddBondMatchRegions { DataContext = this };
            this.htWindow = htWindow;
            this.Groups = groups;
            this.bond2ModelObject = bond2ModelObject;
            this.Bond2ModelParameter = bond2ModelParameter;
            this.Bond2AutoUserRegion = bond2AutoUserRegion;
            this.ReferenceDirectory = referenceDirectory;
            AddGroupCommand = new CommandBase(ExecuteAddGroupCommand);
            RemoveGroupCommand = new CommandBase(ExecuteRemoveGroupCommand);
            AddBond2UserRegionCommand = new CommandBase(ExecuteAddBond2UserRegionCommand);
            AddWireUserRegionCommand = new CommandBase(ExecuteAddWireUserRegionCommand);
            ModifyBond2RegionCommand = new CommandBase(ExecuteModifyBond2RegionCommand);//
            ModifyWireRegionCommand = new CommandBase(ExecuteModifyWireRegionCommand);//
            DisplayAllRegionCommand = new CommandBase(ExecuteDisplayAllRegionCommand);//
            LoadAutoMatchBondCommand = new CommandBase(ExecuteLoadAutoMatchBondCommand);

            CurrentGroup = new BondWireRegionGroup
            {
                Index = groups.Count + 1
            };
            groups.Add(CurrentGroup);
            GroupsCount = groups.Count;
        }

        private void ExecuteLoadAutoMatchBondCommand(object parameter)
        {
            if (Bond2AutoUserRegion.Count == 0)
            {
                MessageBox.Show("请先自动生成焊点！");
                return;
            }
            try
            {
                Groups.Clear();
                for (int i = 0; i < Bond2AutoUserRegion.Count; i++)
                {
                    // 加载顺序应为排序顺序 add_lw 1104 
                    int idx = Bond2AutoUserRegion[i].Index - 1;
                    HOperatorSet.TupleSin(Bond2AutoUserRegion[idx].RegionParameters[2], out HTuple sin_out);
                    HTuple delta_r = Bond2AutoUserRegion[idx].RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset - 2 * 50 * sin_out;
                    HOperatorSet.TupleCos(Bond2AutoUserRegion[idx].RegionParameters[2], out HTuple cos_out);
                    HTuple delta_c = Bond2AutoUserRegion[idx].RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset + 2 * 50 * cos_out;
                    UserRegion userRegion_line = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Line,
                            Bond2AutoUserRegion[idx].RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset,
                            Bond2AutoUserRegion[idx].RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset,
                            delta_r, delta_c, Bond2ModelParameter.DieImageRowOffset, Bond2ModelParameter.DieImageColumnOffset, Bond2AutoUserRegion[idx].RegionParameters[2]);
                    if (userRegion_line == null) return;

                    BondWireRegionGroup bondWireRegion = new BondWireRegionGroup()
                    {
                        Index = Groups.Count + 1
                    };
                    bondWireRegion.Bond2UserRegion = new UserRegion();
                    bondWireRegion.Bond2UserRegion = Bond2AutoUserRegion.ElementAt(idx);
                    bondWireRegion.WireUserRegion = new UserRegion();
                    bondWireRegion.WireUserRegion = userRegion_line;
                    Groups.Add(bondWireRegion);
                    GroupsCount = Groups.Count;
                }
                DispalyGroupRegionAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteAddGroupCommand(object parameter)
        {
            if (isRightClick != true) return;
            CurrentGroup = new BondWireRegionGroup
            {
                Index = Groups.Count + 1
            };
            Groups.Add(CurrentGroup);
            GroupsCount = Groups.Count;
            DispalyGroupRegion(Bond2ModelParameter.ImageChannelIndex);
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
            DispalyGroupRegion(Bond2ModelParameter.ImageChannelIndex);
        }

        private void ExecuteAddBond2UserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (CurrentGroup == null)
            {
                MessageBox.Show("请选择或者新建一个焊点金线组合");
                return;
            }
            UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                                Bond2ModelParameter.DieImageRowOffset,
                                                                Bond2ModelParameter.DieImageColumnOffset);
            if (userRegion == null) return;
            CurrentGroup.Bond2UserRegion = userRegion;
            DispalyGroupRegion(Bond2ModelParameter.ImageChannelIndex);
        }

        private void ExecuteAddWireUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (CurrentGroup == null)
            {
                MessageBox.Show("请选择或者新建一个焊点金线组合");
                return;
            }
            UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                                Bond2ModelParameter.DieImageRowOffset,
                                                                Bond2ModelParameter.DieImageColumnOffset);
            if (userRegion == null) return;
            CurrentGroup.WireUserRegion = userRegion;
            DispalyGroupRegion(Bond2ModelParameter.ImageChannelIndex);
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
                htWindow.DisplaySingleRegion(concatGroupRegion, bond2ModelObject.DieImage);
            }
            for (int i = 0; i < groupsCount; i++)
            {
                if (Groups[i].Bond2UserRegion == null)
                {
                    MessageBox.Show("请先绘制区域！");
                    return;
                }
                HOperatorSet.TupleSin(Groups[i].Bond2UserRegion.RegionParameters[2], out HTuple sin_out_line);
                HTuple line_r = Groups[i].Bond2UserRegion.RegionParameters[0] - Bond2ModelParameter.DieImageRowOffset - 2 * 50 * sin_out_line;
                HOperatorSet.TupleCos(Groups[i].Bond2UserRegion.RegionParameters[2], out HTuple cos_out_line);
                HTuple line_c = Groups[i].Bond2UserRegion.RegionParameters[1] - Bond2ModelParameter.DieImageColumnOffset + 2 * 50 * cos_out_line;

                HOperatorSet.AreaCenter(Groups[i].Bond2UserRegion.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, line_r - 20, line_c + 5);
                HOperatorSet.TupleString(Groups[i].Bond2UserRegion.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }

        private void DispalyGroupRegion(int ImageIndex, bool isHTWindowRegion = true)
        {
            HObject ChannelDieImageDisply = null;

            if (bond2ModelObject.DieImage == null) return;

            // 1122-lw
            if (ImageIndex < 0)
            {
                htWindow.Display(bond2ModelObject.DieImage, true);
                return;
            }

            if (CurrentGroup == null)
            {
                HOperatorSet.AccessChannel(bond2ModelObject.DieImage, out ChannelDieImageDisply, ImageIndex + 1);
                htWindow.Display(ChannelDieImageDisply, true);
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
                HOperatorSet.AccessChannel(bond2ModelObject.DieImage, out ChannelDieImageDisply, ImageIndex + 1);
                htWindow.DisplaySingleRegion(concatGroupRegion, ChannelDieImageDisply);
            }
        }

        private void ExecuteModifyBond2RegionCommand(object parameter)//
        {
            try
            {
                if (currentGroup.Bond2UserRegion == null) return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请添加第二焊点区域");
                return;
            }
            if (isRightClick)
            {
                isRightClick = false;
                htWindow.RegionType = RegionType.Null;
                try
                {
                    int index = currentGroup.Index;
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
                            currentGroup.Bond2UserRegion.Index = index;
                            DispalyGroupRegionAll();
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
                            currentGroup.Bond2UserRegion.Index = index;
                            DispalyGroupRegionAll();
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
                            DispalyGroupRegionAll();
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
                            DispalyGroupRegionAll();
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
            DispalyGroupRegion(Bond2ModelParameter.ImageChannelIndex, false);
            Bond2ModelParameter.IsPickUp = false;
        }

        public void Dispose()
        {
            (Content as Page_AddBondMatchRegions).DataContext = null;
            (Content as Page_AddBondMatchRegions).Close();
            Content = null;
            this.htWindow = null;
            this.Groups = null;
            this.bond2ModelObject = null;
            AddGroupCommand = null;
            RemoveGroupCommand = null;
            AddBond2UserRegionCommand = null;
            AddWireUserRegionCommand = null;
            AddGroupCommand = null;
            RemoveGroupCommand = null;
            AddBond2UserRegionCommand = null;
            AddWireUserRegionCommand = null;
            ModifyBond2RegionCommand = null;
            ModifyWireRegionCommand = null;
            DisplayAllRegionCommand = null;
            LoadAutoMatchBondCommand = null;
        }
    }
}
