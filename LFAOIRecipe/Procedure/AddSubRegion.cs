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
    class AddSubRegion : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public GoldenModelParameter GoldenModelParameter { get; private set; }

        public ObservableCollection<UserRegion> SubUserRegions { get; private set; }

        public ObservableCollection<UserRegion> InspectUserRegions { get; private set; }

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
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private HTHalControlWPF htWindow;

        private readonly string RecipeDirectory;

        private readonly string ModelsDirectory;

        private GoldenModelObject goldenModelObject;
        private IEnumerable<HObject> SubRegions => SubUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);
        private IEnumerable<HObject> InspectRegions => InspectUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public CommandBase AddUserRegionCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }
        public CommandBase LoadUserRegionCommond { get; private set; }
        public CommandBase DefaultRegionCommand { get; private set; }

        public AddSubRegion(HTHalControlWPF htWindow,
                            string recipeDirectory,
                            string modelsDirectory,
                            GoldenModelParameter goldenModelParameter,
                            GoldenModelObject goldenModelObject,
                            ObservableCollection<UserRegion> subUserRegions, 
                            ObservableCollection<UserRegion> inspectUserRegions)
        {
            DisplayName = "添加重点检测区";
            Content = new Page_AddSubRegion { DataContext = this };
            this.htWindow = htWindow;
            this.RecipeDirectory = recipeDirectory;
            this.ModelsDirectory = modelsDirectory;
            this.GoldenModelParameter = goldenModelParameter;
            this.goldenModelObject = goldenModelObject;
            this.SubUserRegions = subUserRegions;
            this.InspectUserRegions = inspectUserRegions;

            AddUserRegionCommand = new CommandBase(ExecuteAddUserRegionCommand);
            RemoveUserRegionCommand = new CommandBase(ExecuteRemoveUserRegionCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);
            LoadUserRegionCommond = new CommandBase(ExecuteUserRegionCommond);
            DefaultRegionCommand = new CommandBase(ExecuteDefaultRegionCommand);
        }

        //加载已有区
        private void ExecuteUserRegionCommond(object parameter)
        {
            if (CutOutDie.isRightClick)
            {
                HOperatorSet.GenEmptyObj(out HObject SubRefRegion);
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = "reg|*.reg";
                    ofd.Multiselect = false;
                    ofd.InitialDirectory = ModelsDirectory;//初始路径
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    HOperatorSet.ReadRegion(out SubRefRegion, ofd.FileName);
                }
                HOperatorSet.SmallestRectangle2(SubRefRegion,
                                            out HTuple row_Rectangle2,
                                            out HTuple column_Rectangle2,
                                            out HTuple phi_Rectangle2,
                                            out HTuple lenth1_Rectangle2,
                                            out HTuple lenth2_Rectangle2);
                HOperatorSet.MoveRegion(SubRefRegion, out HObject _SubRefRegion, -GoldenModelParameter.DieImageRowOffset, -GoldenModelParameter.DieImageColumnOffset);

                SubUserRegions.Clear();
                UserRegion userRegion_Region = new UserRegion()
                {
                    DisplayRegion = _SubRefRegion,
                    CalculateRegion = SubRefRegion,
                    RegionType = RegionType.Rectangle2,
                    RegionParameters = new double[5] { row_Rectangle2, column_Rectangle2, phi_Rectangle2, lenth1_Rectangle2, lenth2_Rectangle2 },
                };
                if (userRegion_Region == null) return;
                userRegion_Region.Index = SubUserRegions.Count + 1;
                SubUserRegions.Add(userRegion_Region);
                htWindow.DisplayMultiRegion(SubRegions, InspectRegions);
            }
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
                userRegion.Index = SubUserRegions.Count + 1;
                SubUserRegions.Add(userRegion);
                htWindow.DisplayMultiRegion(SubRegions, InspectRegions);
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
                for (int i = 0; i < SubUserRegions.Count; i++)
                {
                    if (SubUserRegions[i].IsSelected)
                    {
                        SubUserRegions.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        SubUserRegions[i].Index = i + 1;
                    }
                }
                htWindow.DisplayMultiRegion(SubRegions, InspectRegions, Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, GoldenModelParameter.ImageChannelIndex));
            }
        }

        private void ExecuteModifyRegionCommand(object parameter)
        {
            if (CutOutDie.isRightClick)
            {
                CutOutDie.isRightClick = false;
                HTuple temp = new HTuple();
                try
                {
                    for (int i = 0; i < SubUserRegions.Count; i++)
                    {
                        if (SubUserRegions[i].IsSelected)
                        {
                            switch (SubUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(SubUserRegions[i].RegionParameters[0]),
                                                         Math.Floor(SubUserRegions[i].RegionParameters[1]),
                                                         Math.Ceiling(SubUserRegions[i].RegionParameters[2]),
                                                         Math.Ceiling(SubUserRegions[i].RegionParameters[3]),
                                                         SubUserRegions[i].RegionType,
                                                         GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (SubUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                                                   (SubUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                                                   (SubUserRegions[i].RegionParameters[2] - GoldenModelParameter.DieImageRowOffset),
                                                                                                   (SubUserRegions[i].RegionParameters[3] - GoldenModelParameter.DieImageColumnOffset),
                                                                                                out HTuple row1_Rectangle,
                                                                                                out HTuple column1_Rectangle,
                                                                                                out HTuple row2_Rectangle,
                                                                                                out HTuple column2_Rectangle);

                                    SubUserRegions[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, SubUserRegions[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    SubUserRegions[i] = userRegion;
                                    htWindow.DisplayMultiRegion(SubRegions);//ok
                                    SubUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(SubUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(SubUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(SubUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(SubUserRegions[i].RegionParameters[3]),
                                                                  SubUserRegions[i].RegionType,
                                                                  GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(SubUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                   Math.Floor(SubUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                   SubUserRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(SubUserRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(SubUserRegions[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    SubUserRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, SubUserRegions[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         GoldenModelParameter.DieImageRowOffset,
                                                                                                         GoldenModelParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    SubUserRegions[i] = userRegion_Rectangle2;
                                    htWindow.DisplayMultiRegion(SubRegions);
                                    SubUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((SubUserRegions[i].RegionParameters[0]),
                                                             (SubUserRegions[i].RegionParameters[1]),
                                                             (SubUserRegions[i].RegionParameters[2]),
                                                            0,
                                                            SubUserRegions[i].RegionType,
                                                            GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                                (SubUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                (SubUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                 SubUserRegions[i].RegionParameters[2],
                                                             out HTuple row_Circle,
                                                             out HTuple column_Circle,
                                                             out HTuple radius_Circle);

                                    SubUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     SubUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     GoldenModelParameter.DieImageRowOffset,
                                                                                                     GoldenModelParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    SubUserRegions[i] = userRegion_Circle;
                                    htWindow.DisplayMultiRegion(SubRegions);
                                    SubUserRegions[i].Index = i + 1;
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

        private void ExecuteDefaultRegionCommand(object parameter)
        {
            if (CutOutDie.isRightClick)
            {
                try
                {
                    for (int i = 0; i < InspectUserRegions.Count; i++)
                    {
                            switch (InspectUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    HOperatorSet.GenRectangle1(out HObject Rectangle1_Reg, InspectUserRegions[i].RegionParameters[0], InspectUserRegions[i].RegionParameters[1],
                                                               InspectUserRegions[i].RegionParameters[2], InspectUserRegions[i].RegionParameters[3]);

                                    HOperatorSet.MoveRegion(Rectangle1_Reg, out HObject _SubRefRegion, -GoldenModelParameter.DieImageRowOffset, -GoldenModelParameter.DieImageColumnOffset);

                                    SubUserRegions.Clear();
                                    UserRegion userRegion_Region = new UserRegion()
                                    {
                                        DisplayRegion = _SubRefRegion,
                                        CalculateRegion = Rectangle1_Reg,
                                        RegionType = RegionType.Rectangle1,
                                        RegionParameters = new double[4] { InspectUserRegions[i].RegionParameters[0], InspectUserRegions[i].RegionParameters[1],
                                                                           InspectUserRegions[i].RegionParameters[2], InspectUserRegions[i].RegionParameters[3] },
                                    };
                                    if (userRegion_Region == null) return;
                                    userRegion_Region.Index = SubUserRegions.Count + 1;
                                    SubUserRegions.Add(userRegion_Region);
                                    htWindow.DisplayMultiRegion(SubRegions);
                                    break;

                                case RegionType.Rectangle2:
                                    HOperatorSet.GenRectangle2(out HObject Rectangle2_Reg, InspectUserRegions[i].RegionParameters[0], InspectUserRegions[i].RegionParameters[1],
                                                               InspectUserRegions[i].RegionParameters[2], InspectUserRegions[i].RegionParameters[3], InspectUserRegions[i].RegionParameters[4]);

                                    HOperatorSet.MoveRegion(Rectangle2_Reg, out HObject _SubRefRegion2, -GoldenModelParameter.DieImageRowOffset, -GoldenModelParameter.DieImageColumnOffset);

                                    SubUserRegions.Clear();
                                    UserRegion userRegion_Region2 = new UserRegion()
                                    {
                                        DisplayRegion = _SubRefRegion2,
                                        CalculateRegion = Rectangle2_Reg,
                                        RegionType = RegionType.Rectangle2,
                                        RegionParameters = new double[5] { InspectUserRegions[i].RegionParameters[0], InspectUserRegions[i].RegionParameters[1],
                                                                               InspectUserRegions[i].RegionParameters[2], InspectUserRegions[i].RegionParameters[3], InspectUserRegions[i].RegionParameters[4]},
                                    };
                                    if (userRegion_Region2 == null) return;
                                    userRegion_Region2.Index = SubUserRegions.Count + 1;
                                    SubUserRegions.Add(userRegion_Region2);
                                    htWindow.DisplayMultiRegion(SubRegions);
                                    break;
                              
                                case RegionType.Circle:
                                    HOperatorSet.GenCircle(out HObject Cilrcle_Reg, InspectUserRegions[i].RegionParameters[0], InspectUserRegions[i].RegionParameters[1],
                                                           InspectUserRegions[i].RegionParameters[2]);

                                    HOperatorSet.MoveRegion(Cilrcle_Reg, out HObject _SubRefRegionCilrcle, -GoldenModelParameter.DieImageRowOffset, -GoldenModelParameter.DieImageColumnOffset);

                                    SubUserRegions.Clear();
                                    UserRegion userRegion_Cilrcle = new UserRegion()
                                    {
                                        DisplayRegion = _SubRefRegionCilrcle,
                                        CalculateRegion = Cilrcle_Reg,
                                        RegionType = RegionType.Circle,
                                        RegionParameters = new double[3] { InspectUserRegions[i].RegionParameters[0], InspectUserRegions[i].RegionParameters[1],
                                                                           InspectUserRegions[i].RegionParameters[2]},
                                    };
                                    if (userRegion_Cilrcle == null) return;
                                    userRegion_Cilrcle.Index = SubUserRegions.Count + 1;
                                    SubUserRegions.Add(userRegion_Cilrcle);
                                    htWindow.DisplayMultiRegion(SubRegions);
                                    break;

                                case RegionType.Ellipse:
                                    break;

                                default: break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
        }

        private void ExecuteUserRegionEnableChangedCommand(object parameter)
        {
            htWindow.DisplayMultiRegion(SubRegions, InspectRegions, Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, GoldenModelParameter.ImageChannelIndex));
        }

        public bool CheckCompleted()
        {
            /*
            if (SubUserRegions.Where(r => r.IsEnable).Count() == 0)
            {
                MessageBox.Show("请添加并启用至少一个重点检测区");
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
            htWindow.DisplayMultiRegion(SubRegions, InspectRegions, ChannelDieImageDisply);

            //1121
            ChannelNames = new ObservableCollection<ChannelName>(GoldenModelParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = GoldenModelParameter.ImageChannelIndex;
            OnPropertyChanged("ImageIndex");
        }

        public void Dispose()
        {
            (Content as Page_AddSubRegion).DataContext = null;
            (Content as Page_AddSubRegion).Close();
            Content = null;

            this.htWindow = null;
            this.GoldenModelParameter = null;
            this.goldenModelObject = null;
            this.SubUserRegions = null;
            this.InspectUserRegions = null;

            AddUserRegionCommand = null;
            RemoveUserRegionCommand = null;
            ModifyRegionCommand = null;
            UserRegionEnableChangedCommand = null;
            LoadUserRegionCommond = null;
            DefaultRegionCommand = null;
        }
    }
}
