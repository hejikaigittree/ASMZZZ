using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LFAOIRecipe
{
    class AddMatchRegionFrame : ViewModelBase, IProcedure
    {

        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        private int groupsCount;
        public int GroupsCount
        {
            get => groupsCount;
            set => OnPropertyChanged(ref groupsCount, value);
        }

        public GoldenModelParameter GoldenModelParameter { get; private set; }

        public ObservableCollection<UserRegion> FrameUserRegions { get; private set; }

        //1204
        public ObservableCollection<MatchRegionsGroup> Groups { get; private set; }

        private int imageIndex;
        /// <summary>
        /// 切换原图或通道图
        /// </summary>
        public int ImageIndex
        {
            get => imageIndex;
            set
            {
                if (imageIndex != value)
                {
                    if (value == -1)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImage0, GoldenModelParameter.ImageChannelIndex + 1);
                        goldenModelObject.ChannelImage = ChannelImage0;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 0)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImage1, 1);
                        goldenModelObject.ChannelImage = ChannelImage1;
                        GoldenModelParameter.ImageChannelIndex = 0;

                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 1)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImage2, 2);
                        goldenModelObject.ChannelImage = ChannelImage2;
                        GoldenModelParameter.ImageChannelIndex = 1;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 2)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImage3, 3);
                        goldenModelObject.ChannelImage = ChannelImage3;
                        GoldenModelParameter.ImageChannelIndex = 2;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 3)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImage4, 4);
                        goldenModelObject.ChannelImage = ChannelImage4;
                        GoldenModelParameter.ImageChannelIndex = 3;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelImagen, value + 1);
                        goldenModelObject.ChannelImage = ChannelImagen;
                        GoldenModelParameter.ImageChannelIndex = value;
                    }
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private HTHalControlWPF htWindow;
        private GoldenModelObject goldenModelObject;

        private MatchRegionsGroup currentGroup;
        public MatchRegionsGroup CurrentGroup
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

        //private IEnumerable<HObject> MatchRegions => MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public CommandBase AddUserRegionCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }
        public CommandBase AddGroupCommand { get; private set; }
        public CommandBase RemoveGroupCommand { get; private set; }
        public CommandBase DispalyAllGroupRegionsCommand { get; private set; }
        public CommandBase AddFrameUserRegionCommand { get; private set; }
        public CommandBase ModifyFrameRegionCommand { get; private set; }

        

        public AddMatchRegionFrame(HTHalControlWPF htWindow,
                              GoldenModelParameter goldenModelParameter,
                              GoldenModelObject goldenModelObject,
                              ObservableCollection<MatchRegionsGroup> groups,
                              ObservableCollection<UserRegion> frameUserRegions)
        {
            DisplayName = "添加定位模板区";
            Content = new Page_AddMatchRegionFrame { DataContext = this };

            this.htWindow = htWindow;
            this.GoldenModelParameter = goldenModelParameter;
            this.goldenModelObject = goldenModelObject;
            this.Groups = groups;
            this.FrameUserRegions = frameUserRegions;

            AddUserRegionCommand = new CommandBase(ExecuteAddUserRegionCommand);
            RemoveUserRegionCommand = new CommandBase(ExecuteRemoveUserRegionCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);
            AddGroupCommand = new CommandBase(ExecuteAddGroupCommand);
            RemoveGroupCommand = new CommandBase(ExecuteRemoveGroupCommand);
            DispalyAllGroupRegionsCommand = new CommandBase(ExecuteDispalyAllGroupRegionsCommand);
            AddFrameUserRegionCommand = new CommandBase(ExecuteAddFrameUserRegionCommand);
            ModifyFrameRegionCommand = new CommandBase(ExecuteModifyFrameRegionCommand);


            CurrentGroup = new MatchRegionsGroup
            {
                Index = groups.Count + 1
            };
            groups.Add(CurrentGroup);
            GroupsCount = groups.Count;
        }

        private void ExecuteAddFrameUserRegionCommand(object parameter)
        {
            if (CutOutDie.isRightClick != true) return;
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                                    GoldenModelParameter.DieImageRowOffset,
                                                                    GoldenModelParameter.DieImageColumnOffset);
                if (userRegion == null) return;
                FrameUserRegions.Clear();
                FrameUserRegions.Add(userRegion);
                htWindow.DisplaySingleRegion(FrameUserRegions[0].DisplayRegion);
             }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteModifyFrameRegionCommand(object parameter)
        {
            if (FrameUserRegions.Count==0) return;
            if (CutOutDie.isRightClick)
            {
                CutOutDie.isRightClick = false;
                htWindow.RegionType = RegionType.Null;
                try
                {
                    switch (FrameUserRegions[0].RegionType)
                    {
                        case RegionType.Point:
                            break;

                        case RegionType.Rectangle1:
                            htWindow.InitialHWindowUpdate(Math.Floor(FrameUserRegions[0].RegionParameters[0]),
                                                          Math.Floor(FrameUserRegions[0].RegionParameters[1]),
                                                          Math.Ceiling(FrameUserRegions[0].RegionParameters[2]),
                                                          Math.Ceiling(FrameUserRegions[0].RegionParameters[3]), FrameUserRegions[0].RegionType,
                                                          GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");
                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (FrameUserRegions[0].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                                            (FrameUserRegions[0].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                                            (FrameUserRegions[0].RegionParameters[2] - GoldenModelParameter.DieImageRowOffset),
                                                                                           (FrameUserRegions[0].RegionParameters[3] - GoldenModelParameter.DieImageColumnOffset),
                                                                                        out HTuple row1_Rectangle,
                                                                                        out HTuple column1_Rectangle,
                                                                                        out HTuple row2_Rectangle,
                                                                                        out HTuple column2_Rectangle);

                            FrameUserRegions[0].RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, FrameUserRegions[0].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset);
                            if (userRegion == null) return;
                            FrameUserRegions[0] = userRegion;
                            htWindow.DisplaySingleRegion(FrameUserRegions[0].DisplayRegion);
                            break;

                        case RegionType.Rectangle2:
                            htWindow.InitialHWindowUpdate(Math.Floor(FrameUserRegions[0].RegionParameters[0]),
                                                          Math.Floor(FrameUserRegions[0].RegionParameters[1]),
                                                          Math.Ceiling(FrameUserRegions[0].RegionParameters[2]),
                                                          Math.Ceiling(FrameUserRegions[0].RegionParameters[3]),
                                                          FrameUserRegions[0].RegionType,
                                                          GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                            HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                           Math.Floor(FrameUserRegions[0].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                           Math.Floor(FrameUserRegions[0].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                           FrameUserRegions[0].RegionParameters[2],
                                                           Math.Ceiling(FrameUserRegions[0].RegionParameters[3]),
                                                           Math.Ceiling(FrameUserRegions[0].RegionParameters[4]),
                                                        out HTuple row_Rectangle2,
                                                        out HTuple column_Rectangle2,
                                                        out HTuple phi_Rectangle2,
                                                        out HTuple lenth1_Rectangle2,
                                                        out HTuple lenth2_Rectangle2);

                            FrameUserRegions[0].RegionType = RegionType.Rectangle2;
                            UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, FrameUserRegions[0].RegionType,
                                                                                                 row_Rectangle2, column_Rectangle2,
                                                                                                 lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                 GoldenModelParameter.DieImageRowOffset,
                                                                                                 GoldenModelParameter.DieImageColumnOffset,
                                                                                                 phi_Rectangle2);
                            if (userRegion_Rectangle2 == null) return;
                            FrameUserRegions[0] = userRegion_Rectangle2;
                            htWindow.DisplaySingleRegion(FrameUserRegions[0].DisplayRegion);
                            break;

                        case RegionType.Circle:
                            htWindow.InitialHWindowUpdate((FrameUserRegions[0].RegionParameters[0]),
                                                         (FrameUserRegions[0].RegionParameters[1]),
                                                         (FrameUserRegions[0].RegionParameters[2]),
                                                         0,
                                                         FrameUserRegions[0].RegionType,
                                                         GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                            HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                       (FrameUserRegions[0].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                       (FrameUserRegions[0].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                       FrameUserRegions[0].RegionParameters[2],
                                                   out HTuple row_Circle,
                                                   out HTuple column_Circle,
                                                   out HTuple radius_Circle);

                            FrameUserRegions[0].RegionType = RegionType.Circle;
                            UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                             FrameUserRegions[0].RegionType,
                                                                                             row_Circle, column_Circle,
                                                                                             radius_Circle, 0,
                                                                                             GoldenModelParameter.DieImageRowOffset,
                                                                                             GoldenModelParameter.DieImageColumnOffset,
                                                                                             0);
                            if (userRegion_Circle == null) return;
                            FrameUserRegions[0] = userRegion_Circle;
                            htWindow.DisplaySingleRegion(FrameUserRegions[0].DisplayRegion);
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
                    CutOutDie.isRightClick = true;
                }
            }
        }

        private void ExecuteAddGroupCommand(object parameter)
        {
            if (CutOutDie.isRightClick != true) return;
            CurrentGroup = new MatchRegionsGroup
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
            if (CutOutDie.isRightClick != true) return;
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

        private void ExecuteAddUserRegionCommand(object parameter)
        {
            if (CutOutDie.isRightClick != true) return;
            UserRegion userRegion;
            try
            {
                userRegion = UserRegion.GetHWindowRegion(htWindow,
                                                         GoldenModelParameter.DieImageRowOffset,
                                                         GoldenModelParameter.DieImageColumnOffset);
                if (userRegion == null) return;
                userRegion.Index =CurrentGroup.MatchUserRegions.Count + 1;
                CurrentGroup.MatchUserRegions.Add(userRegion);
                htWindow.DisplayMultiRegion(CurrentGroup.MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
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
            if (CutOutDie.isRightClick != true) return;
            if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < CurrentGroup.MatchUserRegions.Count; i++)
                {
                    if (CurrentGroup.MatchUserRegions[i].IsSelected)
                    {
                        CurrentGroup.MatchUserRegions.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        CurrentGroup.MatchUserRegions[i].Index = i + 1;
                    }
                }
                htWindow.DisplayMultiRegion(CurrentGroup.MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion), goldenModelObject.DieImage);
            }
        }

        /// <summary>
        /// 修改ROI区域
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteModifyRegionCommand(object parameter)//
        {
            if (CutOutDie.isRightClick)
            {
                CutOutDie.isRightClick = false;
                try
                {
                    for (int i = 0; i < CurrentGroup.MatchUserRegions.Count; i++)
                    {
                        if (CurrentGroup.MatchUserRegions[i].IsSelected)
                        {
                            switch (CurrentGroup.MatchUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                     break;

                                case RegionType.Line:

                                    break;

                                case RegionType.Rectangle1:
                                    //                            原图5120*5125
                                    htWindow.InitialHWindowUpdate(Math.Floor(CurrentGroup.MatchUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(CurrentGroup.MatchUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(CurrentGroup.MatchUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(CurrentGroup.MatchUserRegions[i].RegionParameters[3]),
                                                                  CurrentGroup.MatchUserRegions[i].RegionType,
                                                                  GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow,(CurrentGroup.MatchUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                                                  (CurrentGroup.MatchUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                                                  (CurrentGroup.MatchUserRegions[i].RegionParameters[2] - GoldenModelParameter.DieImageRowOffset),
                                                                                                  (CurrentGroup.MatchUserRegions[i].RegionParameters[3] - GoldenModelParameter.DieImageColumnOffset),
                                                        out HTuple row1_Rectangle,
                                                        out HTuple column1_Rectangle,
                                                        out HTuple row2_Rectangle,
                                                        out HTuple column2_Rectangle);

                                    CurrentGroup.MatchUserRegions[i].RegionType = RegionType.Rectangle1;
                                    //                           Die图
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, CurrentGroup.MatchUserRegions[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    CurrentGroup.MatchUserRegions[i] = userRegion;
                                    htWindow.DisplayMultiRegion(CurrentGroup.MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
                                    CurrentGroup.MatchUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(CurrentGroup.MatchUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(CurrentGroup.MatchUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(CurrentGroup.MatchUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(CurrentGroup.MatchUserRegions[i].RegionParameters[3]),
                                                                  CurrentGroup.MatchUserRegions[i].RegionType,
                                                                  GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow, 
                                                                   Math.Floor(CurrentGroup.MatchUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                   Math.Floor(CurrentGroup.MatchUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                   CurrentGroup.MatchUserRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(CurrentGroup.MatchUserRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(CurrentGroup.MatchUserRegions[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    CurrentGroup.MatchUserRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, CurrentGroup.MatchUserRegions[i].RegionType, 
                                                                                                         row_Rectangle2, column_Rectangle2, 
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         GoldenModelParameter.DieImageRowOffset,
                                                                                                         GoldenModelParameter.DieImageColumnOffset, 
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    CurrentGroup.MatchUserRegions[i] = userRegion_Rectangle2;
                                    htWindow.DisplayMultiRegion(CurrentGroup.MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
                                    CurrentGroup.MatchUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((CurrentGroup.MatchUserRegions[i].RegionParameters[0]),
                                                                  (CurrentGroup.MatchUserRegions[i].RegionParameters[1]),
                                                                  (CurrentGroup.MatchUserRegions[i].RegionParameters[2]),
                                                                  0,
                                                                  CurrentGroup.MatchUserRegions[i].RegionType,
                                                                  GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (CurrentGroup.MatchUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                               (CurrentGroup.MatchUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                               CurrentGroup.MatchUserRegions[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    CurrentGroup.MatchUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     CurrentGroup.MatchUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     GoldenModelParameter.DieImageRowOffset,
                                                                                                     GoldenModelParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    CurrentGroup.MatchUserRegions[i] = userRegion_Circle;
                                    htWindow.DisplayMultiRegion(CurrentGroup.MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
                                    CurrentGroup.MatchUserRegions[i].Index = i + 1;
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
                    CutOutDie.isRightClick = true;
                }
            }
        }        
        //
        private void DispalyGroupRegion(bool isDisplayImage = false)
        {
            if (CurrentGroup == null || CurrentGroup.MatchUserRegions.Count==0)
            {
                htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, ImageIndex), true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            foreach (var item in CurrentGroup.MatchUserRegions)
            {
                HOperatorSet.ConcatObj(item.DisplayRegion, concatGroupRegion, out concatGroupRegion);
            }

            if (isDisplayImage)
            {
                htWindow.DisplaySingleRegion(concatGroupRegion);
            }
            else
            {
                //htWindow.DisplaySingleRegion(concatGroupRegion, WireObject.Image);
                htWindow.DisplaySingleRegion(concatGroupRegion, Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, ImageIndex));
            }
        }

        private void ExecuteDispalyAllGroupRegionsCommand(object parameter)
        {
            if (Groups == null || Groups.Count == 0)
            {
                htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, ImageIndex), true);
                return;
            }
            HOperatorSet.GenEmptyObj(out HObject concatGroupRegion);
            foreach (var group in Groups)
            {
                foreach (var item in group.MatchUserRegions)
                {
                    HOperatorSet.ConcatObj(item.DisplayRegion, concatGroupRegion, out concatGroupRegion);
                }
            }
            //htWindow.DisplaySingleRegion(concatGroupRegion);
            htWindow.DisplaySingleRegion(concatGroupRegion, Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, ImageIndex));
        }        

        private void ExecuteUserRegionEnableChangedCommand(object parameter)
        {
            htWindow.DisplayMultiRegion(CurrentGroup.MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion), goldenModelObject.DieImage);
        }

        public bool CheckCompleted()
        {
            /*
            if (MatchUserRegions.Where(r=>r.IsEnable).Count() == 0)
            {
                MessageBox.Show("请添加并启用至少一个定位模板区");
                return false;
            }
            */
            return true;
        }

        public void Initial()
        {
            htWindow.ClearSelection();

            // 1122
            //HOperatorSet.AccessChannel(goldenModelObject.DieImage, out HObject ChannelDieImageDisply, GoldenModelParameter.ImageChannelIndex + 1);
            HObject ChannelDieImageDisply = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, GoldenModelParameter.ImageChannelIndex);
            htWindow.DisplayMultiRegion(CurrentGroup.MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion), ChannelDieImageDisply);
            
            //1121
            //ChannelNames = GoldenModelParameter.ChannelNames;
            ChannelNames = new ObservableCollection<ChannelName>(GoldenModelParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = GoldenModelParameter.ImageChannelIndex;
            OnPropertyChanged("ImageIndex");

            //1225 自动生成3x3框架区域，默认以模板芯片为参考位置
            if (FrameUserRegions.Count() == 0) 
            {
                HOperatorSet.GetImageSize(goldenModelObject.DieImage, out HTuple width, out HTuple height);

                HTuple row1 = Math.Floor(width.D / 2 - 1.0);//
                HTuple col1 = Math.Floor(height.D / 2 - 1.0);//
                HTuple row2 = Math.Ceiling(width.D / 2 + 1.0);//
                HTuple col2 = Math.Ceiling(height.D / 2 + 1.0);//
                HOperatorSet.GenRectangle1(out HObject displayRegion, row1, col1, row2, col2);//
                //HOperatorSet.GenRectangle1(out displayRegion, htWindow.Row1_Rectangle1, htWindow.Column1_Rectangle1, htWindow.Row2_Rectangle1, htWindow.Column2_Rectangle1);//
                double[] regionParameters;
                regionParameters = new double[4]
                {
                    row1 + GoldenModelParameter.DieImageRowOffset,
                    col1 + GoldenModelParameter.DieImageColumnOffset,
                    row2 + GoldenModelParameter.DieImageRowOffset,
                    col2 + GoldenModelParameter.DieImageColumnOffset
                };
                HOperatorSet.GenRectangle1(out HObject calculateRegion, regionParameters[0], regionParameters[1], regionParameters[2], regionParameters[3]);
                UserRegion userRegion = new UserRegion
                {
                    DisplayRegion = displayRegion,
                    CalculateRegion = calculateRegion,
                    RegionType = RegionType.Rectangle1,
                    RegionParameters = regionParameters,
                };

                FrameUserRegions.Add(userRegion);
            }
        }

    public void Dispose()
        {
            (Content as Page_AddMatchRegionFrame).DataContext = null;
            (Content as Page_AddMatchRegionFrame).Close();
            Content = null;
            this.htWindow = null;
            this.GoldenModelParameter = null;
            this.goldenModelObject = null;
            this.Groups = null;
            AddUserRegionCommand = null;
            RemoveUserRegionCommand = null;
            UserRegionEnableChangedCommand = null;
            ModifyRegionCommand = null;
            AddGroupCommand = null;
            RemoveGroupCommand = null;
            DispalyAllGroupRegionsCommand = null;
            AddFrameUserRegionCommand = null;
            ModifyFrameRegionCommand = null;
        }
    }
}
