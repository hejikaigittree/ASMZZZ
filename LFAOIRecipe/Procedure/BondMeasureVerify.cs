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
    public class BondMeasureVerify : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public static bool isRightClick = true;

        public event Action OnSaveXML;

        private readonly string ReferenceDirectory;
        private readonly string bondRecipeDirectory;
        private readonly string ModelsRecipeDirectory;

        private readonly string ModelsFile;

        private readonly string RecipeFile;

        private int isFovTaskFlag = 0;

        private int imgIndex = 0;
        private int pImageIndex = -1;

        private string pImageIndexPath;
        public string PImageIndexPath
        {
            get => pImageIndexPath;
            set => OnPropertyChanged(ref pImageIndexPath, value);
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
                        //MessageBox.Show("请先加载检测验证图像！");
                    }
                    BondMeasureVerifyParameter.ImageChannelIndex = value;
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondMeasureVerifyParameter.ImageChannelIndex);
                    if (ChannelImageVerify != null) htWindow.Display(ChannelImageVerify, true);

                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private HObject ImageVerify,ChannelImageVerify, DieRegions = null;

        private bool isLoadCompleted = false;

        private HTHalControlWPF htWindow;
        public BondMeasureModelObject BondMeasureModelObject { get; set; }

        public BondMeasureParameter BondMeasureParameter { get; private set; }
        public BondMeasureVerifyParameter BondMeasureVerifyParameter { get; private set; }
        public BondMeasureVerifyParameterSet BondMeasureVerifyParameterSet { get; private set; }
        //add by wj 2021-01-07
        public BondVerifyModelPara BondVerifyModelPara { get; private set; }
        public ObservableCollection<UserRegion> PadUserRegions { get; private set; }//2021-01-05
        public ObservableCollection<UserRegion> BondModelUserRegions { get; private set; }
        private IEnumerable<HObject> PadRegions => PadUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);
        private IEnumerable<HObject> BondModelRegions => BondModelUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        private bool? isPadRegionCheckAll;
        public bool? IsPadRegionCheckAll
        {
            get => isPadRegionCheckAll;
            set => OnPropertyChanged(ref isPadRegionCheckAll, value);
        }
        //by wj 2021-01-05
        private UserRegion selectedPadRegion;

        public UserRegion SelectedPadRegion
        {
            get => selectedPadRegion;
            set => OnPropertyChanged(ref selectedPadRegion, value);
        }

        //
        //add by wj 2021-01-05  编辑生成焊点焊盘区域
        public CommandBase AutoGenPadUserRegionsCommand { get; private set; }
        public CommandBase DisplayPadRegionsCommand { get; private set; }//1227
        public CommandBase AddPadUserRegionCommand { get; private set; }
        public CommandBase RemovePadUserRegionCommand { get; private set; }
        public CommandBase ModifyPadUserRegionCommand { get; private set; }
        public CommandBase AlterPadUserRegionCommand { get; private set; }
        public CommandBase IsPadRegionCheckCommand { get; private set; }
        public CommandBase IsPadRegionCheckAllCommand { get; private set; }

        //public CommandBase PadRegionPickUpCommand { get; private set; }
        public CommandBase PadRegionEnableChangedCommand { get; private set; }
        //
        //add by wj bitch modify verify parameter
        public CommandBase BatchModifyVerifyParasCommand { get; private set; }
        //
        public CommandBase PreviousCommand { get; private set; }
        public CommandBase NextCommand { get; private set; }
        public CommandBase VerifyCommand { get; private set; }
        public CommandBase SaveCommand { get; private set; }
        public CommandBase ImagesSetVerifyCommand { get; private set; }
        public CommandBase RefreshImagesSet { get; private set; }


        public BondMeasureVerify(HTHalControlWPF htWindow,
                                 string modelsFile,
                                 string recipeFile,
                                 string referenceDirectory,
                                 BondMeasureModelObject BondMeasureModelObject,
                                 BondMeasureParameter BondMeasureParameter,
                                 BondMeasureVerifyParameter BondMeasureVerifyParameter,
                                 BondMeasureVerifyParameterSet BondMeasureVerifyParameterSet,
                                 BondVerifyModelPara BondVerifyModelPara,
                                 ObservableCollection<UserRegion> PadUserRegions,
                                 ObservableCollection<UserRegion> BondModelUserRegions, 
                                 string bondRecipeDirctory,
                                 string modelsRecipeDirectory)
        {
            DisplayName = "检测验证";
            Content = new Page_BondMeasureVerify { DataContext = this };
            this.htWindow = htWindow;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.ReferenceDirectory = referenceDirectory;
            this.BondMeasureModelObject = BondMeasureModelObject;
            this.BondMeasureParameter = BondMeasureParameter;           
            this.BondMeasureVerifyParameter = BondMeasureVerifyParameter;
            this.BondMeasureVerifyParameterSet = BondMeasureVerifyParameterSet;
            this.BondVerifyModelPara = BondVerifyModelPara;
            this.PadUserRegions = PadUserRegions;
            this.BondModelUserRegions = BondModelUserRegions;
            this.bondRecipeDirectory = bondRecipeDirctory;
            this.ModelsRecipeDirectory = modelsRecipeDirectory;

            //add by wj 2021-01-05
            AutoGenPadUserRegionsCommand = new CommandBase(ExecuteAutoGenPadUserRegionsCommand);
            DisplayPadRegionsCommand = new CommandBase(ExecuteDisplayPadRegionsCommand);
            AddPadUserRegionCommand = new CommandBase(ExecuteAddPadUserRegionCommand);
            RemovePadUserRegionCommand = new CommandBase(ExecuteRemovePadUserRegionCommand);
            ModifyPadUserRegionCommand = new CommandBase(ExecuteModifyPadUserRegionCommand);
            AlterPadUserRegionCommand = new CommandBase(ExecuteAlterPadUserRegionCommand);
            //
            IsPadRegionCheckCommand = new CommandBase(ExecuteIsPadRegionCheckCommand);
            IsPadRegionCheckAllCommand = new CommandBase(ExecuteIsPadRegionCheckAllCommand);
            //PadRegionPickUpCommand = new CommandBase(ExecutePadRegionPickUpCommand);
            PadRegionEnableChangedCommand = new CommandBase(ExecutePadRegionEnableChangedCommand);
            //
            BatchModifyVerifyParasCommand = new CommandBase(ExecuteBatchModifyVerifyParasCommand);

            PreviousCommand = new CommandBase(ExecutePreviousCommand);
            NextCommand = new CommandBase(ExecuteNextCommand);
            VerifyCommand = new CommandBase(ExecuteVerifyCommand);
            SaveCommand = new CommandBase(ExecuteSaveCommand);
            ImagesSetVerifyCommand = new CommandBase(ExecuteImagesSetVerifyCommand);
            RefreshImagesSet = new CommandBase(ExecuteRefreshImagesSet);

        }


        #region **********************编辑焊盘区域  2021-01-05 add by wj

        //add by wj 2021-0108
        private void ExecuteAutoGenPadUserRegionsCommand(object parameter)
        {
            if (isRightClick != true) return;
            try
            {
                GenPadUserRegions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        public void GenPadUserRegions()
        {
            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }
            if (BondModelUserRegions.Count < 1)
            {
                MessageBox.Show("请先生成焊点模板区域！");
                return;
            }
            PadUserRegions.Clear();
            for (int i = 0; i < BondModelUserRegions.Count; i++)
            {

                UserRegion _BondModelUseRegion = BondModelUserRegions.ElementAt(i);

                HOperatorSet.AreaCenter(Algorithm.Region.ConcatRegion(_BondModelUseRegion), out HTuple area, out HTuple row_Rectangle, out HTuple column_Rectangle);
                HOperatorSet.GenRectangle2(out HObject rect2Region, row_Rectangle, column_Rectangle, 0, BondMeasureParameter.GenPadRegionsSize[0], BondMeasureParameter.GenPadRegionsSize[1]);
                //获取正方形所需参数
                HOperatorSet.SmallestRectangle1(rect2Region, out HTuple row1_Rectangle,
                                                             out HTuple column1_Rectangle,
                                                             out HTuple row2_Rectangle,
                                                             out HTuple column2_Rectangle);
                //生成Bond检测区域
                HTuple row1_Rectangle1 = row1_Rectangle - BondMeasureParameter.DieImageRowOffset;
                HTuple column1_Rectangle1 = column1_Rectangle - BondMeasureParameter.DieImageColumnOffset;
                HTuple row2_Rectangle1 = row2_Rectangle - BondMeasureParameter.DieImageRowOffset;
                HTuple column2_Rectangle1 = column2_Rectangle - BondMeasureParameter.DieImageColumnOffset;

                UserRegion userRegion_Rectangle1 = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Rectangle1,
                                                                                row1_Rectangle1, column1_Rectangle1,
                                                                                row2_Rectangle1, column2_Rectangle1,
                                                                                BondMeasureParameter.DieImageRowOffset,
                                                                                BondMeasureParameter.DieImageColumnOffset,
                                                                                0);
                if (userRegion_Rectangle1 == null) return;
                userRegion_Rectangle1.Index = PadUserRegions.Count + 1;
                //
                userRegion_Rectangle1.BondVerifyRegionWithPara = new BondVerifyRegionWithPara();
                //add by wj 2021-0107
                userRegion_Rectangle1.ChannelNames = ChannelNames;
                userRegion_Rectangle1.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                userRegion_Rectangle1.BondVerifyRegionWithPara.BondThresAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                userRegion_Rectangle1.BondVerifyRegionWithPara.BondMeasureAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                userRegion_Rectangle1.BondVerifyRegionWithPara.BondMatchAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                userRegion_Rectangle1.AlgoParameterIndex = BondMeasureParameter.AlgoParameterIndex;

                PadUserRegions.Add(userRegion_Rectangle1);
                //htWindow.DisplayMultiRegion(BondVerifyRegions, BondMeasureModelObject.DieImage);
                //htWindow.DisplayMultiRegion(BondVerifyRegions);
                DispalyPadRegions();
            }

        }

        public void DispalyPadRegions(bool isHTWindowRegion = true)
        {
            if (PadUserRegions.Count == 0)
            {
                
                htWindow.Display(BondMeasureModelObject.DieImage, true);
                return;
            }

            if (isHTWindowRegion)
            {
                htWindow.DisplayMultiRegion(PadRegions);
            }
            else
            {
                htWindow.DisplayMultiRegion(PadRegions, Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.DieImage, BondMeasureParameter.ImageChannelIndex));
                //htWindow.DisplayMultiRegion(PadRegions, BondMeasureModelObject.DieImage);
            }

            foreach (var item in PadUserRegions)
            {
                htWindow.hTWindow.HalconWindow.SetColor("green");
                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, row_tmp, col_tmp);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }

        private void ExecuteDisplayPadRegionsCommand(object parameter)
        {
            DispalyPadRegions(false);
        }
        private void ExecuteIsPadRegionCheckCommand(object parameter)
        {
            if (PadUserRegions.All(x => x.IsSelected == true))
            { IsPadRegionCheckAll = true; }
            else if (PadUserRegions.All(x => !x.IsSelected))
            { IsPadRegionCheckAll = false; }
            else
            { IsPadRegionCheckAll = null; }
        }

        private void ExecuteIsPadRegionCheckAllCommand(object parameter)
        {
            if (IsPadRegionCheckAll == true)
            { PadUserRegions.ToList().ForEach(r => r.IsSelected = true); }
            else if (IsPadRegionCheckAll == false)
            { PadUserRegions.ToList().ForEach(r => r.IsSelected = false); }
        }

        //添加区域
        private void ExecuteAddPadUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;//

            if (BondMeasureModelObject.Image == null || !BondMeasureModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }

            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow, BondMeasureParameter.DieImageRowOffset, BondMeasureParameter.DieImageColumnOffset);
                if (userRegion == null) return;
                userRegion.Index = PadUserRegions.Count + 1;
                userRegion.BondVerifyRegionWithPara = new BondVerifyRegionWithPara();
                //add by wj 2021-0107
                userRegion.ChannelNames = ChannelNames;
                userRegion.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                userRegion.BondVerifyRegionWithPara.BondThresAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                userRegion.BondVerifyRegionWithPara.BondMeasureAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                userRegion.BondVerifyRegionWithPara.BondMatchAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                userRegion.AlgoParameterIndex = BondMeasureParameter.AlgoParameterIndex;

                PadUserRegions.Add(userRegion);
                //htWindow.DisplayMultiRegion(BondVerifyRegions, BondMeasureModelObject.DieImage);
                htWindow.DisplayMultiRegion(PadRegions);
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

        //删除区域
        private void ExecuteRemovePadUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;//

            //if (CurrentModel == null)
            //{
            //MessageBox.Show("请选择或者新建一个模板配置");
            //return;
            //}
            try
            {
                if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    for (int i = 0; i < PadUserRegions.Count; i++)
                    {
                        if (PadUserRegions[i].IsSelected)
                        {
                            PadUserRegions.RemoveAt(i);
                            i--;

                        }
                        else
                        {
                            PadUserRegions[i].Index = i + 1;
                        }
                    }
                    //htWindow.DisplayMultiRegion(RefineRegions, BondMeasureModelObject.Image);
                    htWindow.DisplayMultiRegion(PadRegions, BondMeasureModelObject.DieImage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }
        //修改区域
        private void ExecuteModifyPadUserRegionCommand(object parameter)//
        {
            if (isRightClick != true) return;//
            try
            {
                if (PadUserRegions == null) return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请添加精炼区域到列表");
                return;
            }
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    for (int i = 0; i < PadUserRegions.Count; i++)
                    {
                        if (PadUserRegions[i].IsSelected)
                        {
                            switch (PadUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(PadUserRegions[i].RegionParameters[0]),
                                                  Math.Floor(PadUserRegions[i].RegionParameters[1]),
                                                  Math.Ceiling(PadUserRegions[i].RegionParameters[2]),
                                                  Math.Ceiling(PadUserRegions[i].RegionParameters[3]),
                                                  PadUserRegions[i].RegionType,
                                                  BondMeasureParameter.DieImageRowOffset,
                                                  BondMeasureParameter.DieImageColumnOffset,
                                                  "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (PadUserRegions[i].RegionParameters[0] - BondMeasureParameter.DieImageRowOffset),
                                                                                                   (PadUserRegions[i].RegionParameters[1] - BondMeasureParameter.DieImageColumnOffset),
                                                                                                   (PadUserRegions[i].RegionParameters[2] - BondMeasureParameter.DieImageRowOffset),
                                                                                                   (PadUserRegions[i].RegionParameters[3] - BondMeasureParameter.DieImageColumnOffset),
                                                                out HTuple row1_Rectangle,
                                                                out HTuple column1_Rectangle,
                                                                out HTuple row2_Rectangle,
                                                                out HTuple column2_Rectangle);

                                    PadUserRegions[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion_Rectangle1 = UserRegion.GetHWindowRegionUpdate(htWindow, PadUserRegions[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle, BondMeasureParameter.DieImageRowOffset, BondMeasureParameter.DieImageColumnOffset);
                                    if (userRegion_Rectangle1 == null) return;

                                    userRegion_Rectangle1.BondVerifyRegionWithPara = new BondVerifyRegionWithPara();
                                    //add by wj 2021-0107
                                    userRegion_Rectangle1.ChannelNames = ChannelNames;
                                    userRegion_Rectangle1.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                                    userRegion_Rectangle1.BondVerifyRegionWithPara.BondThresAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                                    userRegion_Rectangle1.BondVerifyRegionWithPara.BondMeasureAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                                    userRegion_Rectangle1.BondVerifyRegionWithPara.BondMatchAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                                    userRegion_Rectangle1.AlgoParameterIndex = BondMeasureParameter.AlgoParameterIndex;
                                    //
                                    PadUserRegions[i] = userRegion_Rectangle1;
                                    //htWindow.DisplayMultiRegion(BondVerifyRegions, BondMeasureModelObject.DieImage);
                                    PadUserRegions[i].Index = i + 1;
                                    DispalyPadRegions(true);
                                    break;

                                case RegionType.Rectangle2:
                                    break;

                                case RegionType.Circle:
                                    htWindow.InitialHWindowUpdate((PadUserRegions[i].RegionParameters[0]),
                                                                  (PadUserRegions[i].RegionParameters[1]),
                                                                  (PadUserRegions[i].RegionParameters[2]),
                                                                  0, PadUserRegions[i].RegionType,
                                                                  BondMeasureParameter.DieImageRowOffset,
                                                                  BondMeasureParameter.DieImageColumnOffset,
                                                                  "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (PadUserRegions[i].RegionParameters[0] - BondMeasureParameter.DieImageRowOffset),
                                                               (PadUserRegions[i].RegionParameters[1] - BondMeasureParameter.DieImageColumnOffset),
                                                               PadUserRegions[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    PadUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     PadUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle, radius_Circle, 0,
                                                                                                     BondMeasureParameter.DieImageRowOffset,
                                                                                                     BondMeasureParameter.DieImageColumnOffset,
                                                                                                     0);
                                    if (userRegion_Circle == null) return;
                                    PadUserRegions[i] = userRegion_Circle;
                                    
                                    //htWindow.DisplayMultiRegion(BondVerifyRegions, BondMeasureModelObject.DieImage);
                                    PadUserRegions[i].Index = i + 1;
                                    DispalyPadRegions(true);
                                    break;

                                case RegionType.Ellipse:
                                    break;

                                case RegionType.Region:

                                    MessageBox.Show("请选择变更区域按钮进行自由区域修改！");
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

        //变更Pad区域
        private void ExecuteAlterPadUserRegionCommand(object parameter)//
        {
            if (isRightClick != true) return;//
            try
            {
                if (PadUserRegions == null) return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请添加Pad区域到列表");
                return;
            }

            if (htWindow.RegionType == RegionType.Null)
            {
                System.Windows.MessageBox.Show("没有有效区域，请选择需要变更的区域后，右键点击确认！");
                return ;
            }

            UserRegion userRegion = new UserRegion();
            userRegion = UserRegion.GetHWindowRegion(htWindow, BondMeasureParameter.DieImageRowOffset, BondMeasureParameter.DieImageColumnOffset);          
            if (userRegion == null) return;

            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    userRegion.BondVerifyRegionWithPara = new BondVerifyRegionWithPara();
                    //add by wj 2021-0107
                    userRegion.ChannelNames = ChannelNames;
                    userRegion.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                    userRegion.BondVerifyRegionWithPara.BondThresAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                    userRegion.BondVerifyRegionWithPara.BondMeasureAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                    userRegion.BondVerifyRegionWithPara.BondMatchAlgoPara.ImageIndex = BondMeasureParameter.ImageChannelIndex;
                    userRegion.AlgoParameterIndex = BondMeasureParameter.AlgoParameterIndex;

                    for (int i = 0; i < PadUserRegions.Count; i++)
                    {
                        if (PadUserRegions[i].IsSelected)
                        {
                            //
                            switch (PadUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:

                                    PadUserRegions[i].RegionType = userRegion.RegionType;
                                    PadUserRegions[i] = userRegion;                                    
                                    PadUserRegions[i].Index = i + 1;
                                    DispalyPadRegions(true);
                                    break;

                                case RegionType.Rectangle2:
                                    PadUserRegions[i].RegionType = userRegion.RegionType;
                                    PadUserRegions[i] = userRegion;
                                    PadUserRegions[i].Index = i + 1;
                                    DispalyPadRegions(true);
                                    break;

                                case RegionType.Circle:
                                    PadUserRegions[i].RegionType = userRegion.RegionType;
                                    PadUserRegions[i] = userRegion;
                                    PadUserRegions[i].Index = i + 1;
                                    DispalyPadRegions(true);
                                    break;

                                case RegionType.Ellipse:
                                    PadUserRegions[i].RegionType = userRegion.RegionType;
                                    PadUserRegions[i] = userRegion;
                                    PadUserRegions[i].Index = i + 1;
                                    DispalyPadRegions(true);
                                    break;

                                case RegionType.Region:
                                    PadUserRegions[i].RegionType = userRegion.RegionType;
                                    PadUserRegions[i] = userRegion;
                                    PadUserRegions[i].Index = i + 1;
                                    DispalyPadRegions(true);
                                    break;
                                default: break;
                            }
                        }
                    }
                    htWindow.RegionType = RegionType.Null;
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

        //public void ExecutePadRegionPickUpCommand(object parameter)
        //{
        //    if (BondMeasureParameter.IsPadRegionPickUp == true)
        //    {
        //        BondMeasureParameter.IsVerifyRegionPickUp = false;
        //    }
        //}

        public void GetClickDownPointsFromPadRegion()
        {
            if (PadUserRegions.Count == 0 || BondMeasureParameter.IsPadRegionPickUp == false) return;
            try
            {
                HOperatorSet.GenRegionPoints(out HObject Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
                foreach (var item in PadUserRegions)
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void ExecutePadRegionEnableChangedCommand(object parameter)
        {
            htWindow.DisplayMultiRegion(PadRegions, Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.DieImage, BondMeasureParameter.ImageChannelIndex));
            //htWindow.DisplayMultiRegion(PadRegions, BondMeasureModelObject.DieImage);
        }

        private void ExecuteBatchModifyVerifyParasCommand(object parameter)  //add by wj 2021-01-07
        {
            if (isRightClick != true) return;
            if (PadUserRegions.Count() == 0)
            {
                MessageBox.Show("请添加Pad焊盘区域！");
                return;
            }
            try
            {
                switch (BondMeasureParameter.AlgoParameterIndex)
                {
                    case 0:
                        //批量设置阈值分割方法参数
                        if (MessageBox.Show("是否进行参数批量修改?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            for (int i = 0; i < PadUserRegions.Count; i++)
                            {
                                if (PadUserRegions[i].IsSelected)
                                {
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ImageIndex = ImageIndex;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.IsCircleBond = BondVerifyModelPara.IsCircleBond;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondSize = BondVerifyModelPara.BondSize;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.EllipsBondSize = BondVerifyModelPara.EllipsBondSize;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ThreshGray = BondVerifyModelPara.ThreshGray;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ClosingSize = BondVerifyModelPara.ClosingSize;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondOverSizeFactor = BondVerifyModelPara.BondOverSizeFactor;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondUnderSizeFactor = BondVerifyModelPara.BondUnderSizeFactor;
                                }
                            }
                        }
                        break;
                    case 1:
                        //批量设置Measure方法参数
                        if (MessageBox.Show("是否进行参数批量修改?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            for (int i = 0; i < PadUserRegions.Count; i++)
                            {
                                if (PadUserRegions[i].IsSelected)
                                {
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.ImageIndex = ImageIndex;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.PreJudgeEnable = BondVerifyModelPara.PreJudgeEnable;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.SegThreshGray = BondVerifyModelPara.SegThreshGray;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.SegRegAreaFactor = BondVerifyModelPara.SegRegAreaFactor;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.BondOverSizeFactor = BondVerifyModelPara.BondOverSizeFactor;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.BondUnderSizeFactor = BondVerifyModelPara.BondUnderSizeFactor;
                                }
                            }
                        }
                        break;
                    case 2:
                        //批量设置Match方法参数
                        if (MessageBox.Show("是否进行参数批量修改?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            for (int i = 0; i < PadUserRegions.Count; i++)
                            {
                                if (PadUserRegions[i].IsSelected)
                                {
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.ImageIndex = ImageIndex;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.MinMatchScore = BondVerifyModelPara.MinMatchScore;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.AngleExt = BondVerifyModelPara.AngleExt;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.IsCircleBond = BondVerifyModelPara.IsCircleBond;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.BondSize = BondVerifyModelPara.BondSize;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.EllipsBondSize = BondVerifyModelPara.EllipsBondSize;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.IsBondRegRefine = BondVerifyModelPara.IsBondRegRefine;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.AddBallNum = BondVerifyModelPara.AddBallNum;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.MaxOverlap = BondVerifyModelPara.MaxOverlap;
                                    PadUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.MinHistScore = BondVerifyModelPara.MinHistScore;

                                }
                            }
                        }
                        break;
                    default:break;
                }
                   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        #endregion

        //保存
        private void ExecuteSaveCommand(object parameter)
        {
            if (BondMeasureParameter.ImageCountChannels > 0 && BondMeasureVerifyParameter.ImageChannelIndex < 0)
            {
                MessageBox.Show("请先选择验证图像通道！");
                return;
            }
            try
            {
                OnSaveXML?.Invoke();
                MessageBox.Show("参数保存完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "参数保存失败");
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
                        BondMeasureVerifyParameter.VerifyImagesDirectory = fbd.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                    // 1201 lw
                    if (MessageBox.Show("是否为指定Fov的task图集类型？", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        isFovTaskFlag = 1;

                        // 指定Fov合成多通道图并显示第一张图
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                BondMeasureVerifyParameter.VerifyImagesDirectory,
                                                                                BondMeasureParameter.CurFovName,
                                                                                0, out HTuple hv_o_ImageVerifyNum, out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        BondMeasureVerifyParameter.CurrentVerifySet = hv_o_ImageVerifyNum;
                        PImageIndexPath = imageFiles[BondMeasureVerifyParameter.ImageChannelIndex];
                        ImageVerify = ho_MutiImage;
                    }
                    else
                    {
                        isFovTaskFlag = 0;

                        Algorithm.File.list_image_files(BondMeasureVerifyParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        string[] folderList = imageFiles;
                        BondMeasureVerifyParameter.CurrentVerifySet = folderList.Count();
                        PImageIndexPath = imageFiles[0];
                        HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                        ImageVerify = image;
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondMeasureVerifyParameter.ImageChannelIndex);
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
            BondMeasureVerifyParameter.VerifyImagesDirectory = BondMeasureParameter.VerifyImagesDirectory;
            if (Directory.Exists(BondMeasureVerifyParameter.VerifyImagesDirectory))
            {
                Algorithm.File.list_image_files(BondMeasureVerifyParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                string[] folderList = imageFiles;
                BondMeasureVerifyParameter.CurrentVerifySet = folderList.Count();
                PImageIndexPath = imageFiles[0];
                HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                ImageVerify = image;
                ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondMeasureVerifyParameter.ImageChannelIndex);
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
                MessageBox.Show("请先加载检测验证图集！");
                return;
            }
            if (DieRegions == null || !DieRegions.IsInitialized())
            {
                MessageBox.Show("请先加载搜索区！");
                return;
            }
            if (PadUserRegions.Count == 0)
            {
                MessageBox.Show("请先画检测验证区域！");
                return;
            }
            if (BondMeasureParameter.ImageCountChannels > 0 && BondMeasureVerifyParameter.ImageChannelIndex < 0)
            {
                MessageBox.Show("请先选择通道图像！");
                return;
            }
            if (BondMeasureModelObject.MetrologyHandle == null)
            {
                if (File.Exists($"{bondRecipeDirectory}MetrologyHandle.mtr"))
                {
                    HOperatorSet.ReadMetrologyModel($"{bondRecipeDirectory}MetrologyHandle.mtr", out HTuple metrologyHandle);
                    BondMeasureModelObject.MetrologyHandle = metrologyHandle;
                }
                else
                {
                    MessageBox.Show("请先创建焊点测量模板！");
                    return;
                }
            }

            //Window_Loading window_Loading = new Window_Loading("正在检验");
            //window_Loading.Show();

            try
            {
                htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondMeasureVerifyParameter.ImageChannelIndex), true);

                Algorithm.Model_RegionAlg.HTV_confirm_mapping_matrix(Algorithm.Region.GetChannnelImageConcact(ImageVerify),//黑白图传1张；彩色图RGB三通道图concact在一起
                                           DieRegions.SelectObj(imgIndex + 1),
                                           ModelsFile,
                                           RecipeFile,
                                           BondMeasureParameter.OnRecipesIndexs[BondMeasureParameter.OnRecipesIndex],
                                           out HTuple HomMat2D,
                                           out HTuple _frameLocPara,
                                           out HTuple ErrCode, out HTuple ErrStr);

                if (ErrCode != 0)
                {
                    MessageBox.Show("模板没有定位到！");
                    imgIndex++;
                    if (imgIndex + 1 > DieRegions.CountObj())
                    {
                        imgIndex = 0;
                    }
                    return;
                }

                //初始化Bond检测输出
                HObject ho__BondContours = null;
                HObject ho_EmptyObj = null, ho__BondContour = null;
                HObject ho__FailBondReg = null;

                HObjectVector hvec__FailBondRegs = new HObjectVector(1);
                //
                HTuple hv__DefectTypeReg = new HTuple(), hv__DefectImgIdxReg = new HTuple();
                HTuple hv__ErrRegCode = new HTuple(), hv__ErrRegStr = new HTuple();
                //
                HTupleVector hvec__DefectBondImgIdx = new HTupleVector(1);
                HTupleVector hvec__DefectBondType = new HTupleVector(1), hvec__DefectBondValue = new HTupleVector(2);
                HTupleVector hvec__RefBondValue = new HTupleVector(2);
                HTupleVector hvec__DefectValueReg = new HTupleVector(1), hvec__RefValueReg = new HTupleVector(1);
                //
                HOperatorSet.GenEmptyObj(out ho__BondContours);
                HOperatorSet.GenEmptyObj(out ho_EmptyObj);
                HOperatorSet.GenEmptyObj(out ho__BondContour);
                HOperatorSet.GenEmptyObj(out ho__FailBondReg);


                hvec__DefectBondImgIdx = (new HTupleVector(1).Insert(0, new HTupleVector(new HTuple())));
                hvec__DefectBondType = (new HTupleVector(1).Insert(0, new HTupleVector(new HTuple())));
                hvec__DefectBondValue = (new HTupleVector(2).Insert(0, (new HTupleVector(1).Insert(0, new HTupleVector(new HTuple())))));
                hvec__RefBondValue = (new HTupleVector(2).Insert(0, (new HTupleVector(1).Insert(0, new HTupleVector(new HTuple())))));

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hvec__FailBondRegs = dh.Take((
                        dh.Add(new HObjectVector(1)).Insert(0, dh.Add(new HObjectVector(ho_EmptyObj)))));
                }
                //
                HTuple _MetrologyType = new HTuple();
                HTuple MetrologyType = new HTuple();
                foreach (var bondRegon in BondModelUserRegions)
                {
                    switch (bondRegon.RegionType)
                    {
                        case RegionType.Line:
                            _MetrologyType = 1;
                            break;
                        case RegionType.Rectangle2:
                            _MetrologyType = 2;
                            break;
                        case RegionType.Circle:
                            _MetrologyType = 3;
                            break;
                        case RegionType.Ellipse:
                            _MetrologyType = 4;
                            break;
                        default:
                            MessageBox.Show("请重新选择画区域工具！");
                            return;
                    }
                    MetrologyType = MetrologyType.TupleConcat(_MetrologyType);
                }
                //获取Bond检测区域
                HOperatorSet.AffineTransRegion(Algorithm.Region.ConcatRegion(PadUserRegions), out HObject _BondInspectRegs, HomMat2D, "nearest_neighbor");
                //
                for (int i = 0; i < PadUserRegions.Count(); i++)
                {
                    HOperatorSet.SelectObj(_BondInspectRegs, out HObject _BondInspectReg,i + 1);
                    //
                    switch (PadUserRegions[i].AlgoParameterIndex)
                    {
                        case 0:
                            //阈值分割                          
                            Algorithm.Model_RegionAlg.HTV_Bond_Inspect_threshold(Algorithm.Region.GetChannnelImageConcact(ImageVerify), _BondInspectReg, out ho__BondContour,
                                        out ho__FailBondReg,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ImageIndex+1,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.IsCircleBond ? new HTuple(PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondSize) : new HTuple(PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.EllipsBondSize[0]).TupleConcat(new HTuple(PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.EllipsBondSize[1])),
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ThreshGray,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ClosingSize,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondOverSizeFactor,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondUnderSizeFactor,
                                        out hv__DefectTypeReg,
                                        out hv__DefectImgIdxReg,
                                        out hvec__DefectValueReg,
                                        out hvec__RefValueReg,
                                        out hv__ErrRegCode,
                                        out hv__ErrRegStr);
                            break;
                        case 1:
                            //measure
                            Algorithm.Model_RegionAlg.HTV_Bond_Inspect_measure(Algorithm.Region.GetChannnelImageConcact(ImageVerify), _BondInspectReg, out ho__BondContour,
                                        out ho__FailBondReg, 
                                        HomMat2D, 
                                        i,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.ImageIndex+1,
                                        MetrologyType,
                                        BondMeasureModelObject.MetrologyHandle,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.BondOverSizeFactor,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.BondUnderSizeFactor,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.PreJudgeEnable,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.SegThreshGray,
                                        PadUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.SegRegAreaFactor, 
                                        out hv__DefectTypeReg,
                                        out hv__DefectImgIdxReg, 
                                        out hvec__DefectValueReg, 
                                        out hvec__RefValueReg,
                                        out hv__ErrRegCode, 
                                        out hv__ErrRegStr);
                            break;
                        case 2:
                            //match
                            MessageBox.Show("该检测区域暂不使用匹配进行定位！");
                            return;
                            
                        default:
                            break;
                    }

                    //
                    //整合区域检测结果
                    //
                    hvec__DefectBondType[i] = new HTupleVector(hv__DefectTypeReg).Clone();
                    hvec__DefectBondImgIdx[i] = new HTupleVector(hv__DefectImgIdxReg).Clone();
                    hvec__DefectBondValue[i] = hvec__DefectValueReg.Clone();
                    hvec__RefBondValue[i] = hvec__RefValueReg.Clone();
                    //
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho__BondContours, ho__BondContour, out ExpTmpOutVar_0
                            );
                        ho__BondContours.Dispose();
                        ho__BondContours = ExpTmpOutVar_0;
                    }
                    hvec__FailBondRegs[i] = new HObjectVector(ho__FailBondReg.CopyObj(1, -1));
                }



                if (ho__BondContours.CountObj() > 0)
                {
                    htWindow.DisplaySingleRegion(ho__BondContours.ConcatObj(DieRegions.SelectObj(imgIndex + 1)), Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondMeasureVerifyParameter.ImageChannelIndex));
                }

                Algorithm.Model_RegionAlg.HTV_Vector_to_ObjectConcat_1d(hvec__FailBondRegs, out HObject BondFailRegsConcat, out HTuple VerErrCode, out HTuple VerErrStr);
                Algorithm.Model_RegionAlg.HTV_Vector_to_Tuple_1d(hvec__DefectBondType, out HTuple BondDefectType, out HTuple _ErrCode, out HTuple _ErrStr);
                //寻找大于0的数组
                HOperatorSet.TupleSum(BondDefectType, out HTuple TypeSum);
                if (TypeSum > 0)
                {
                    htWindow.DisplaySingleRegion(BondFailRegsConcat.ConcatObj(ho__BondContours).ConcatObj(DieRegions.SelectObj(imgIndex + 1)), Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondMeasureVerifyParameter.ImageChannelIndex), "orange");
                }

                imgIndex++;
                if ((imgIndex + 1) > DieRegions.CountObj())
                {
                    imgIndex = 0;
                }

                ho__BondContours?.Dispose();
                hvec__FailBondRegs?.Dispose();
                BondFailRegsConcat?.Dispose();
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
                if (BondMeasureVerifyParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == 0 ? BondMeasureVerifyParameter.CurrentVerifySet - 1 : --pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                BondMeasureVerifyParameter.VerifyImagesDirectory,
                                                                                BondMeasureParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[BondMeasureVerifyParameter.ImageChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(BondMeasureVerifyParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondMeasureVerifyParameter.ImageChannelIndex);
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
                if (BondMeasureVerifyParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == BondMeasureVerifyParameter.CurrentVerifySet - 1 ? 0 : ++pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                BondMeasureVerifyParameter.VerifyImagesDirectory,
                                                                                BondMeasureParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[BondMeasureVerifyParameter.ImageChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(BondMeasureVerifyParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondMeasureVerifyParameter.ImageChannelIndex);
                    htWindow.Display(ChannelImageVerify, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
            if (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff) return;
            DispalyPadRegions(false);
            //htWindow.DisplayMultiRegion(PadRegions, Algorithm.Region.GetChannnelImageUpdate(BondMeasureModelObject.Image, BondMeasureParameter.ImageChannelIndex));
            //1122
            ChannelNames = new ObservableCollection<ChannelName>(BondMeasureParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = BondMeasureParameter.ImageChannelIndex;
            OnPropertyChanged("ImageIndex");
        }

        public void Dispose()
        {
            (Content as Page_BondMeasureVerify).DataContext = null;
            (Content as Page_BondMeasureVerify).Close();
            Content = null;
            this.htWindow = null;
            ImageVerify = null;
            ChannelImageVerify = null;
            PadRegionEnableChangedCommand = null;
            PreviousCommand = null;
            NextCommand = null;
            VerifyCommand = null;
            SaveCommand = null;
            ImagesSetVerifyCommand = null;
            BondMeasureModelObject = null;
            BondMeasureParameter = null;
            BondMeasureVerifyParameter = null;
            BondMeasureVerifyParameterSet = null;
            BondVerifyModelPara = null;
            BondModelUserRegions = null;
            // 2021-010-05
            PadUserRegions = null;
            AutoGenPadUserRegionsCommand = null;
            DisplayPadRegionsCommand = null;
            AddPadUserRegionCommand = null;
            RemovePadUserRegionCommand = null;
            AlterPadUserRegionCommand = null;
            IsPadRegionCheckCommand = null;
            IsPadRegionCheckAllCommand = null;
            //PadRegionPickUpCommand = null;
            BatchModifyVerifyParasCommand = null;
        }
    }
}
