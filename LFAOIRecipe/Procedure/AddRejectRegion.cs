using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LFAOIRecipe
{
    class AddRejectRegion : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public GoldenModelParameter GoldenModelParameter { get; private set; }

        public ObservableCollection<UserRegion> RejectUserRegions { get; private set; }

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

        private IEnumerable<HObject> RejectRegions => RejectUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        private IEnumerable<HObject> InspectRegions => InspectUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public CommandBase AddUserRegionCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }
        public CommandBase LoadRejectRegionCommond { get; private set; }        

        public AddRejectRegion(HTHalControlWPF htWindow,
                               string recipeDirectory,
                               string modelsDirectory,
                               GoldenModelParameter goldenModelParameter,
                               GoldenModelObject goldenModelObject,
                               ObservableCollection<UserRegion> rejectUserRegions,
                               ObservableCollection<UserRegion> inspectUserRegions)
        {
            DisplayName = "添加免检区";
            Content = new Page_AddRejectRegion { DataContext = this };

            this.htWindow = htWindow;
            this.RecipeDirectory = recipeDirectory;
            this.ModelsDirectory = modelsDirectory;
            this.GoldenModelParameter = goldenModelParameter;
            this.goldenModelObject = goldenModelObject;
            this.RejectUserRegions = rejectUserRegions;
            this.InspectUserRegions = inspectUserRegions;

            AddUserRegionCommand = new CommandBase(ExecuteAddUserRegionCommand);
            RemoveUserRegionCommand = new CommandBase(ExecuteRemoveUserRegionCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);
            LoadRejectRegionCommond = new CommandBase(ExecuteLoadRejectRegionCommond);
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
                userRegion.Index = RejectUserRegions.Count + 1;
                RejectUserRegions.Add(userRegion);
                //htWindow.DisplayMultiRegion(RejectRegions, InspectRegions);
                htWindow.DisplayMultiRegion(RejectRegions);
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
                for (int i = 0; i < RejectUserRegions.Count; i++)
                {
                    if (RejectUserRegions[i].IsSelected)
                    {
                        RejectUserRegions.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        RejectUserRegions[i].Index = i + 1;
                    }
                }
                htWindow.DisplayMultiRegion(RejectRegions);
            }
        }

        private void ExecuteModifyRegionCommand(object parameter)
        {
            if (CutOutDie.isRightClick)
            {
                CutOutDie.isRightClick = false;
                try
                {
                    for (int i = 0; i < RejectUserRegions.Count; i++)
                    {
                        if (RejectUserRegions[i].IsSelected)
                        {
                            switch (RejectUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(RejectUserRegions[i].RegionParameters[0]),
                                                         Math.Floor(RejectUserRegions[i].RegionParameters[1]),
                                                         Math.Ceiling(RejectUserRegions[i].RegionParameters[2]),
                                                         Math.Ceiling(RejectUserRegions[i].RegionParameters[3]),
                                                         RejectUserRegions[i].RegionType,
                                                         GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (RejectUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                                                   (RejectUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                                                   (RejectUserRegions[i].RegionParameters[2] - GoldenModelParameter.DieImageRowOffset),
                                                                                                   (RejectUserRegions[i].RegionParameters[3] - GoldenModelParameter.DieImageColumnOffset),
                                                                out HTuple row1_Rectangle,
                                                                out HTuple column1_Rectangle,
                                                                out HTuple row2_Rectangle,
                                                                out HTuple column2_Rectangle);

                                    RejectUserRegions[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, 
                                                            RejectUserRegions[i].RegionType, 
                                                            row1_Rectangle, 
                                                            column1_Rectangle, 
                                                            row2_Rectangle, 
                                                            column2_Rectangle,
                                                            GoldenModelParameter.DieImageRowOffset,
                                                            GoldenModelParameter.DieImageColumnOffset);
                                    if (userRegion == null) return;
                                    RejectUserRegions[i] = userRegion;
                                    htWindow.DisplayMultiRegion(RejectRegions);//ok
                                    RejectUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(RejectUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(RejectUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(RejectUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(RejectUserRegions[i].RegionParameters[3]),
                                                                  RejectUserRegions[i].RegionType,
                                                                  GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(RejectUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                   Math.Floor(RejectUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                                   RejectUserRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(RejectUserRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(RejectUserRegions[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    RejectUserRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, RejectUserRegions[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         GoldenModelParameter.DieImageRowOffset,
                                                                                                         GoldenModelParameter.DieImageColumnOffset,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    RejectUserRegions[i] = userRegion_Rectangle2;
                                    htWindow.DisplayMultiRegion(RejectRegions);
                                    RejectUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate(Math.Floor(RejectUserRegions[i].RegionParameters[0]),
                                                             (RejectUserRegions[i].RegionParameters[1]),
                                                             (RejectUserRegions[i].RegionParameters[2]),
                                                            0,
                                                            RejectUserRegions[i].RegionType,
                                                            GoldenModelParameter.DieImageRowOffset, GoldenModelParameter.DieImageColumnOffset, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                                (RejectUserRegions[i].RegionParameters[0] - GoldenModelParameter.DieImageRowOffset),
                                                                (RejectUserRegions[i].RegionParameters[1] - GoldenModelParameter.DieImageColumnOffset),
                                                               RejectUserRegions[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    RejectUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     RejectUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle,
                                                                                                     radius_Circle, 0,
                                                                                                     GoldenModelParameter.DieImageRowOffset,
                                                                                                     GoldenModelParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    RejectUserRegions[i] = userRegion_Circle;
                                    htWindow.DisplayMultiRegion(RejectRegions);
                                    RejectUserRegions[i].Index = i + 1;
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
            //htWindow.DisplayMultiRegion(RejectRegions, InspectRegions, goldenModelObject.DieImage);
            htWindow.DisplayMultiRegion(RejectRegions, goldenModelObject.DieImage);
        }

        //1207 加载免检区域
        private void ExecuteLoadRejectRegionCommond(object parameter)
        {
            if (CutOutDie.isRightClick)
            {
                HOperatorSet.GenEmptyObj(out HObject rejectRegion);
                string regionPath;
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = "reg|*.reg";
                    ofd.Multiselect = false;
                    ofd.InitialDirectory = ModelsDirectory;
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    HOperatorSet.ReadRegion(out rejectRegion, ofd.FileName);
                    string regionName = System.IO.Path.GetFileName(ofd.FileName);
                    DirectoryInfo folder = new DirectoryInfo(ofd.FileName);
                    string folderPath = Directory.GetParent(folder.ToString()).FullName;
                    string parentPath = System.IO.Path.GetFileName(folderPath);
                    regionPath = $"Models\\{parentPath}\\{regionName}";
                }
                HOperatorSet.MoveRegion(rejectRegion, out HObject _rejectRegion, -GoldenModelParameter.DieImageRowOffset, -GoldenModelParameter.DieImageColumnOffset);
                UserRegion userRegion_Region = new UserRegion()
                {
                    DisplayRegion = _rejectRegion,
                    CalculateRegion = rejectRegion,
                    RegionType = RegionType.Region,
                    RegionPath = regionPath,
                    RecipeNames = "RejectReg",
                };
                if (userRegion_Region == null) return;
                userRegion_Region.Index = RejectUserRegions.Count + 1;
                RejectUserRegions.Add(userRegion_Region);
                htWindow.DisplayMultiRegion(RejectRegions);
                goldenModelObject.RejectRegion = rejectRegion;
            }
        }

        public bool CheckCompleted()
        {
            /*
            if (RejectUserRegions.Where(r => r.IsEnable).Count() == 0)
            {
                MessageBox.Show("请添加并启用至少一个免检区");
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
            htWindow.DisplayMultiRegion(RejectRegions, ChannelDieImageDisply);

            // 1121
            ChannelNames = new ObservableCollection<ChannelName>(GoldenModelParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = GoldenModelParameter.ImageChannelIndex;
            OnPropertyChanged("ImageIndex");
        }

        public void Dispose()
        {
            (Content as Page_AddRejectRegion).DataContext = null;
            (Content as Page_AddRejectRegion).Close();
            Content = null;

            this.htWindow = null;
            this.GoldenModelParameter = null;
            this.goldenModelObject = null;
            this.RejectUserRegions = null;
            this.InspectUserRegions = null;

            AddUserRegionCommand = null;
            RemoveUserRegionCommand = null;
            UserRegionEnableChangedCommand = null;
        }
    }
}
