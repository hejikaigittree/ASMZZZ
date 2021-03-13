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
    class AddMatchRegion : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public GoldenModelParameter GoldenModelParameter { get; private set; }

        public ObservableCollection<UserRegion> MatchUserRegions { get; private set; }

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

        private IEnumerable<HObject> MatchRegions => MatchUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public CommandBase AddUserRegionCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }

        public AddMatchRegion(HTHalControlWPF htWindow,
                              GoldenModelParameter goldenModelParameter,
                              GoldenModelObject goldenModelObject,
                              ObservableCollection<UserRegion> matchUserRegions)
        {
            DisplayName = "添加定位模板区";
            Content = new Page_AddMatchRegion { DataContext = this };

            this.htWindow = htWindow;
            this.GoldenModelParameter = goldenModelParameter;
            this.goldenModelObject = goldenModelObject;
            this.MatchUserRegions = matchUserRegions;

            AddUserRegionCommand = new CommandBase(ExecuteAddUserRegionCommand);
            RemoveUserRegionCommand = new CommandBase(ExecuteRemoveUserRegionCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);
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
                userRegion.Index = MatchUserRegions.Count + 1;
                MatchUserRegions.Add(userRegion);
                htWindow.DisplayMultiRegion(MatchRegions);
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
                for (int i = 0; i < MatchUserRegions.Count; i++)
                {
                    if (MatchUserRegions[i].IsSelected)
                    {
                        MatchUserRegions.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        MatchUserRegions[i].Index = i + 1;
                    }
                }
                htWindow.DisplayMultiRegion(MatchRegions, Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, GoldenModelParameter.ImageChannelIndex));
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
                    for (int i = 0; i < MatchUserRegions.Count; i++)
                    {
                        if (MatchUserRegions[i].IsSelected)
                        {
                            switch (MatchUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                     break;

                                case RegionType.Line:

                                    break;

                                case RegionType.Rectangle1:
                                    //                            原图5120*5125
                                    htWindow.InitialHWindowUpdate(Math.Floor(MatchUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(MatchUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(MatchUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(MatchUserRegions[i].RegionParameters[3]), 
                                                                  MatchUserRegions[i].RegionType,
                                                                  GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow,(MatchUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                                                  (MatchUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                                                  (MatchUserRegions[i].RegionParameters[2] - GoldenModelParameter.DieImageRowOffset),
                                                                                                  (MatchUserRegions[i].RegionParameters[3] - GoldenModelParameter.DieImageColumnOffset),
                                                        out HTuple row1_Rectangle,
                                                        out HTuple column1_Rectangle,
                                                        out HTuple row2_Rectangle,
                                                        out HTuple column2_Rectangle);

                                    MatchUserRegions[i].RegionType = RegionType.Rectangle1;
                                    //                           Die图
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, MatchUserRegions[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    MatchUserRegions[i] = userRegion;
                                    htWindow.DisplayMultiRegion(MatchRegions);//ok
                                    MatchUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(MatchUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(MatchUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(MatchUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(MatchUserRegions[i].RegionParameters[3]),
                                                                  MatchUserRegions[i].RegionType,
                                                                  GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow, 
                                                                   Math.Floor(MatchUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                   Math.Floor(MatchUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                   MatchUserRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(MatchUserRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(MatchUserRegions[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    MatchUserRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, MatchUserRegions[i].RegionType, 
                                                                                                         row_Rectangle2, column_Rectangle2, 
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         GoldenModelParameter.DieImageRowOffset,
                                                                                                         GoldenModelParameter.DieImageColumnOffset, 
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    MatchUserRegions[i] = userRegion_Rectangle2;
                                    htWindow.DisplayMultiRegion(MatchRegions);
                                    MatchUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((MatchUserRegions[i].RegionParameters[0]),
                                                                  (MatchUserRegions[i].RegionParameters[1]),
                                                                  (MatchUserRegions[i].RegionParameters[2]),
                                                                  0,
                                                                  MatchUserRegions[i].RegionType,
                                                                  GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (MatchUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                               (MatchUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                               MatchUserRegions[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    MatchUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow, 
                                                                                                     MatchUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     GoldenModelParameter.DieImageRowOffset,
                                                                                                     GoldenModelParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    MatchUserRegions[i] = userRegion_Circle;
                                    htWindow.DisplayMultiRegion(MatchRegions);
                                    MatchUserRegions[i].Index = i + 1;
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

        private void ExecuteUserRegionEnableChangedCommand(object parameter)
        {
            htWindow.DisplayMultiRegion(MatchRegions, Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, GoldenModelParameter.ImageChannelIndex));
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
            htWindow.DisplayMultiRegion(MatchRegions, ChannelDieImageDisply);
            
            //1121
            //ChannelNames = GoldenModelParameter.ChannelNames;
            ChannelNames = new ObservableCollection<ChannelName>(GoldenModelParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = GoldenModelParameter.ImageChannelIndex;
            OnPropertyChanged("ImageIndex");

        }

    public void Dispose()
        {
            (Content as Page_AddMatchRegion).DataContext = null;
            (Content as Page_AddMatchRegion).Close();
            Content = null;

            this.htWindow = null;
            this.GoldenModelParameter = null;
            this.goldenModelObject = null;
            this.MatchUserRegions = null;

            AddUserRegionCommand = null;
            RemoveUserRegionCommand = null;
            ModifyRegionCommand = null;
            UserRegionEnableChangedCommand = null;
        }
    }
}
