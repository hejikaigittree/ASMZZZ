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
    class AddInspectRegion : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public GoldenModelParameter GoldenModelParameter { get; private set; }

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

        private IEnumerable<HObject> InspectRegions => InspectUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public CommandBase AddUserRegionCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }
        public CommandBase LoadUserRegionCommond { get; private set; }

        public AddInspectRegion(HTHalControlWPF htWindow,
                                string recipeDirectory,
                                string modelsDirectory,
                                GoldenModelParameter goldenModelParameter,
                                GoldenModelObject goldenModelObject,
                                ObservableCollection<UserRegion> inspectUserRegions)
        {
            DisplayName = "添加检测区";
            Content = new Page_AddInspectRegion { DataContext = this };

            this.htWindow = htWindow;
            this.RecipeDirectory = recipeDirectory;
            this.ModelsDirectory = modelsDirectory;
            this.GoldenModelParameter = goldenModelParameter;
            this.goldenModelObject = goldenModelObject;
            this.InspectUserRegions = inspectUserRegions;

            AddUserRegionCommand = new CommandBase(ExecuteAddUserRegionCommand);
            RemoveUserRegionCommand = new CommandBase(ExecuteRemoveUserRegionCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);
            LoadUserRegionCommond = new CommandBase(ExecuteUserRegionCommond);
        }

        //加载检测区
        private void ExecuteUserRegionCommond(object parameter)
        {
            if (CutOutDie.isRightClick)
            {
                HOperatorSet.GenEmptyObj(out HObject IcRefRegion);
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = "reg|*.reg";
                    ofd.Multiselect = false;
                    ofd.InitialDirectory = ModelsDirectory;//初始路径
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    HOperatorSet.ReadRegion(out IcRefRegion, ofd.FileName);
                }
                HOperatorSet.SmallestRectangle2(IcRefRegion,
                                            out HTuple row_Rectangle2,
                                            out HTuple column_Rectangle2,
                                            out HTuple phi_Rectangle2,
                                            out HTuple lenth1_Rectangle2,
                                            out HTuple lenth2_Rectangle2);
                HOperatorSet.MoveRegion(IcRefRegion, out HObject _IcRefRegion, -GoldenModelParameter.DieImageRowOffset, -GoldenModelParameter.DieImageColumnOffset);

                InspectUserRegions.Clear();
                UserRegion userRegion_Region = new UserRegion()
                {
                    DisplayRegion = _IcRefRegion,
                    CalculateRegion = IcRefRegion,
                    RegionType = RegionType.Rectangle2,
                    RegionParameters = new double[5] { row_Rectangle2, column_Rectangle2, phi_Rectangle2, lenth1_Rectangle2, lenth2_Rectangle2 },
                };
                if (userRegion_Region == null) return;
                userRegion_Region.Index = InspectUserRegions.Count + 1;
                InspectUserRegions.Add(userRegion_Region);
                htWindow.DisplayMultiRegion(InspectRegions);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            if (userRegion == null) return;
            userRegion.Index = InspectUserRegions.Count + 1;
            InspectUserRegions.Add(userRegion);
            htWindow.DisplayMultiRegion(InspectRegions);
        }

        private void ExecuteRemoveUserRegionCommand(object parameter)
        {
            if (CutOutDie.isRightClick != true) return;//
            if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < InspectUserRegions.Count; i++)
                {
                    if (InspectUserRegions[i].IsSelected)
                    {
                        InspectUserRegions.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        InspectUserRegions[i].Index = i + 1;
                    }
                }
                htWindow.DisplayMultiRegion(InspectRegions, Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, GoldenModelParameter.ImageChannelIndex));
            }
        }

        private void ExecuteModifyRegionCommand(object parameter)
        {
            if (CutOutDie.isRightClick)
            {
                CutOutDie.isRightClick = false;
                try
                {
                    for (int i = 0; i < InspectUserRegions.Count; i++)
                    {
                        if (InspectUserRegions[i].IsSelected)
                        {
                            switch (InspectUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(InspectUserRegions[i].RegionParameters[0]),
                                                          Math.Floor(InspectUserRegions[i].RegionParameters[1]),
                                                          Math.Ceiling(InspectUserRegions[i].RegionParameters[2]),
                                                          Math.Ceiling(InspectUserRegions[i].RegionParameters[3]), 
                                                          InspectUserRegions[i].RegionType,
                                                          GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow,(InspectUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                                           (InspectUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                                           (InspectUserRegions[i].RegionParameters[2] - GoldenModelParameter.DieImageRowOffset),
                                                                                           (InspectUserRegions[i].RegionParameters[3] - GoldenModelParameter.DieImageColumnOffset),
                                                                                        out HTuple row1_Rectangle,
                                                                                        out HTuple column1_Rectangle,
                                                                                        out HTuple row2_Rectangle,
                                                                                        out HTuple column2_Rectangle);

                                    InspectUserRegions[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, InspectUserRegions[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    InspectUserRegions[i] = userRegion;
                                    htWindow.DisplayMultiRegion(InspectRegions);//ok
                                    InspectUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(InspectUserRegions[i].RegionParameters[0]),
                                    Math.Floor(InspectUserRegions[i].RegionParameters[1]),
                                    Math.Ceiling(InspectUserRegions[i].RegionParameters[2]),
                                    Math.Ceiling(InspectUserRegions[i].RegionParameters[3]),
                                    InspectUserRegions[i].RegionType,
                                    GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(InspectUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                   Math.Floor(InspectUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                   InspectUserRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(InspectUserRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(InspectUserRegions[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    InspectUserRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, InspectUserRegions[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         GoldenModelParameter.DieImageRowOffset,
                                                                                                         GoldenModelParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    InspectUserRegions[i] = userRegion_Rectangle2;
                                    htWindow.DisplayMultiRegion(InspectRegions);
                                    InspectUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((InspectUserRegions[i].RegionParameters[0]),
                                                                 (InspectUserRegions[i].RegionParameters[1]),
                                                                 (InspectUserRegions[i].RegionParameters[2]),
                                                                 0,
                                                                 InspectUserRegions[i].RegionType,
                                                                 GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (InspectUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                               (InspectUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                               InspectUserRegions[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    InspectUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     InspectUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     GoldenModelParameter.DieImageRowOffset,
                                                                                                     GoldenModelParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    InspectUserRegions[i] = userRegion_Circle;
                                    htWindow.DisplayMultiRegion(InspectRegions);
                                    InspectUserRegions[i].Index = i + 1;
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
            htWindow.DisplayMultiRegion(InspectRegions, Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, GoldenModelParameter.ImageChannelIndex));
        }

        public bool CheckCompleted()
        {
            /*
            if (InspectUserRegions.Where(r => r.IsEnable).Count() == 0)
            {
                MessageBox.Show("请添加并启用至少一个检测区");
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
            htWindow.DisplayMultiRegion(InspectRegions, ChannelDieImageDisply);

            // 1121
            ChannelNames = new ObservableCollection<ChannelName>(GoldenModelParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = GoldenModelParameter.ImageChannelIndex;
            OnPropertyChanged("ImageIndex");
        }

        public void Dispose()
        {
            (Content as Page_AddInspectRegion).DataContext = null;
            (Content as Page_AddInspectRegion).Close();
            Content = null;
            htWindow = null;
            GoldenModelParameter = null;
            goldenModelObject = null;
            InspectUserRegions = null;
            AddUserRegionCommand = null;
            RemoveUserRegionCommand = null;
            ModifyRegionCommand = null;
            UserRegionEnableChangedCommand = null;
            LoadUserRegionCommond = null;
        }
    }
}
