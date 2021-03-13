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
    public class BondMatchVerify : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public event Action OnSaveXML;

        public bool isRightClick = true;

        public CommandBase PreviousCommand { get; private set; }
        public CommandBase NextCommand { get; private set; }
        public CommandBase VerifyCommand { get; private set; }
        public CommandBase SaveCommand { get; private set; }
        public CommandBase ImagesSetVerifyCommand { get; private set; }
        public CommandBase RefreshModels { get; private set; }

        private HTHalControlWPF htWindow;

        private readonly string ModelsFile;

        private readonly string RecipeFile;

        private Bond2ModelObject bond2ModelObject;

        private readonly string ReferenceDirectory ;

        private readonly string BondDirectory ;

        private Bond2ModelParameter Bond2ModelParameter { get; set; }
        public BondVerifyModelPara BondVerifyModelPara { get; private set; }
        public BondWireParameter BondWireParameter { get; private set; }

        public CommandBase RefreshImagesSet { get; private set; }
        //add by wj 2021-01-11
        public CommandBase LoadBondVerifyRegionsCommand { get; private set; }
        public CommandBase DisplayBondVerifyRegionsCommand { get; private set; }
        public CommandBase IsBondVerifyRegionCheckCommand { get; private set; }
        public CommandBase IsBondVerifyRegionCheckAllCommand { get; private set; }
        public CommandBase BondVerifyRegionEnableChangedCommand { get; private set; }
        //
        //add by wj bitch modify verify parameter
        public CommandBase BatchModifyVerifyParasCommand { get; private set; }
        //
        public ObservableCollection<Bond2Model> BondModels { get; private set; }
        public ObservableCollection<UserRegion> BondVerifyUserRegions { get; private set; }//2021-01-11
         
        private IEnumerable<HObject> BondVerifyRegions => BondVerifyUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public ObservableCollection<BondWireRegionGroup> Groups { get; private set; }

        //add by wj
        private bool? isBondVerfyRegionCheckAll;
        public bool? IsBondVerfyRegionCheckAll
        {
            get => isBondVerfyRegionCheckAll;
            set => OnPropertyChanged(ref isBondVerfyRegionCheckAll, value);
        }
        //by wj 2021-01-05
        private UserRegion selectedBondVerifyRegion;

        public UserRegion SelectedBondVerifyRegion
        {
            get => selectedBondVerifyRegion;
            set => OnPropertyChanged(ref selectedBondVerifyRegion, value);
        }

        //
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
                    BondWireParameter.ImageChannelIndex = value;
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondWireParameter.ImageChannelIndex);
                    if (ChannelImageVerify != null) htWindow.Display(ChannelImageVerify, true);

                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        HTuple modelID = null;

        private bool isLoadCompleted = false;

        private HObject ImageVerify, ChannelImageVerify, DieRegions = null;

        public BondMatchVerify(HTHalControlWPF htWindow,
                               string modelsFile,
                               string recipeFile,
                               ObservableCollection<Bond2Model> bondModels,
                               ObservableCollection<UserRegion> BondVerifyUserRegions,
                               ObservableCollection<BondWireRegionGroup> groups,
                               Bond2ModelObject bond2ModelObject,
                               Bond2ModelParameter bond2ModelParameter,
                               BondWireParameter BondWireParameter,
                               BondVerifyModelPara BondVerifyModelPara,
                               string bondDirectory,
                               string referenceDirectory)
        {
            DisplayName = "检测验证";
            Content = new Page_BondMatchVerify { DataContext = this };
            this.htWindow = htWindow;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.BondModels = bondModels;
            this.BondVerifyUserRegions = BondVerifyUserRegions;
            this.Groups = groups;
            this.bond2ModelObject = bond2ModelObject;
            this.Bond2ModelParameter = bond2ModelParameter;
            this.BondWireParameter = BondWireParameter;
            this.BondVerifyModelPara = BondVerifyModelPara;
            this.BondDirectory = bondDirectory;
            this.ReferenceDirectory = referenceDirectory;

            //add by wj 2021-01-11
            LoadBondVerifyRegionsCommand = new CommandBase(ExecuteLoadBondVerifyRegionsCommand);
            DisplayBondVerifyRegionsCommand = new CommandBase(ExecuteDisplayBondVerifyRegionsCommand);
            IsBondVerifyRegionCheckCommand = new CommandBase(ExecuteIsBondVerifyRegionCheckCommand);
            IsBondVerifyRegionCheckAllCommand = new CommandBase(ExecuteIsBondVerifyRegionCheckAllCommand);
            BondVerifyRegionEnableChangedCommand = new CommandBase(ExecuteBondVerifyRegionEnableChangedCommand);
            //
            BatchModifyVerifyParasCommand = new CommandBase(ExecuteBatchModifyVerifyParasCommand);
            //-----------------------------------------------------------------------------------------------

            PreviousCommand = new CommandBase(ExecutePreviousCommand);
            NextCommand = new CommandBase(ExecuteNextCommand);
            VerifyCommand = new CommandBase(ExecuteVerifyCommand);
            SaveCommand = new CommandBase(ExecuteSaveCommand);
            ImagesSetVerifyCommand = new CommandBase(ExecuteImagesSetVerifyCommand);
            RefreshModels = new CommandBase(ExecuteRefreshModels);
            RefreshImagesSet = new CommandBase(ExecuteRefreshImagesSet);
        }



        public void ExecuteLoadBondVerifyRegionsCommand(object parameter)
        {
            if (isRightClick != true) return;
            try
            {
                LoadBondVerifyUserRegions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void LoadBondVerifyUserRegions()
        {
            if (bond2ModelObject.Image == null || !bond2ModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }
            if (Groups.Count < 1)
            {
                MessageBox.Show("请先生成焊点检测区域组！");
                return;
            }

            BondVerifyUserRegions.Clear();

            foreach (var group in Groups)
            {
                UserRegion userRegion = new UserRegion();

                userRegion = group.Bond2UserRegion;

                userRegion.Index = BondVerifyUserRegions.Count + 1;
                //
                userRegion.BondVerifyRegionWithPara = new BondVerifyRegionWithPara();
                //add by wj 2021-0107
                userRegion.ChannelNames = ChannelNames;
                userRegion.ImageIndex = BondWireParameter.ImageChannelIndex;
                userRegion.BondVerifyRegionWithPara.BondThresAlgoPara.ImageIndex = BondWireParameter.ImageChannelIndex;
                userRegion.BondVerifyRegionWithPara.BondMeasureAlgoPara.ImageIndex = BondWireParameter.ImageChannelIndex;
                userRegion.BondVerifyRegionWithPara.BondMatchAlgoPara.ImageIndex = BondWireParameter.ImageChannelIndex;
                //
                HOperatorSet.AngleLx(group.WireUserRegion.RegionParameters[0], group.WireUserRegion.RegionParameters[1], group.WireUserRegion.RegionParameters[2], group.WireUserRegion.RegionParameters[3],
                    out HTuple lineAngle);
                userRegion.BondVerifyRegionWithPara.BondMatchAlgoPara.AngleStart = lineAngle;
                //
                userRegion.BondVerifyRegionWithPara.BondMatchAlgoPara.BallNum_OnRegion = group.Bond2_BallNums;
                //
                userRegion.AlgoParameterIndex = BondWireParameter.AlgoParameterIndex;

                BondVerifyUserRegions.Add(userRegion);

                DisplayBondVerifyRegions(false);

            }

        }
        public void DisplayBondVerifyRegions(bool isHTWindowRegion = true)
        {
            if (BondVerifyUserRegions.Count == 0)
            {
                htWindow.Display(bond2ModelObject.DieImage, true);
                return;
            }

            if (isHTWindowRegion)
            {
                htWindow.DisplayMultiRegion(BondVerifyRegions);
            }
            else
            {
                htWindow.DisplayMultiRegion(BondVerifyRegions, Algorithm.Region.GetChannnelImageUpdate(bond2ModelObject.DieImage, BondWireParameter.ImageChannelIndex));
                //htWindow.DisplayMultiRegion(BondVerifyRegions, bond2ModelObject.DieImage);
            }

            foreach (var item in BondVerifyUserRegions)
            {
                htWindow.hTWindow.HalconWindow.SetColor("green");
                HOperatorSet.AreaCenter(item.DisplayRegion, out HTuple area_tmp, out HTuple row_tmp, out HTuple col_tmp);
                HOperatorSet.SetTposition(htWindow.hTWindow.HalconWindow, row_tmp, col_tmp);
                HOperatorSet.TupleString(item.Index, "0", out HTuple StrIndex);
                HOperatorSet.WriteString(htWindow.hTWindow.HalconWindow, StrIndex);
            }
        }

        private void ExecuteDisplayBondVerifyRegionsCommand(object parameter)
        {
            DisplayBondVerifyRegions(true);
        }

        private void ExecuteIsBondVerifyRegionCheckCommand(object parameter)
        {
            if (BondVerifyUserRegions.All(x => x.IsSelected == true))
            { IsBondVerfyRegionCheckAll = true; }
            else if (BondVerifyUserRegions.All(x => !x.IsSelected))
            { IsBondVerfyRegionCheckAll = false; }
            else
            { IsBondVerfyRegionCheckAll = null; }
        }

        private void ExecuteIsBondVerifyRegionCheckAllCommand(object parameter)
        {
            if (IsBondVerfyRegionCheckAll == true)
            { BondVerifyUserRegions.ToList().ForEach(r => r.IsSelected = true); }
            else if (IsBondVerfyRegionCheckAll == false)
            { BondVerifyUserRegions.ToList().ForEach(r => r.IsSelected = false); }
        }

        public void GetClickDownPointsFromBond2UserRegion()
        {
            if (BondVerifyUserRegions.Count() == 0 || BondWireParameter.IsVerifyRegionPickUp == false) return;
            try
            {
                HOperatorSet.GenRegionPoints(out HObject Point, htWindow.mPositionDownRow, htWindow.mPositionDownColumn);
                foreach (var item in BondVerifyUserRegions)
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

        private void ExecuteBondVerifyRegionEnableChangedCommand(object parameter)
        {
            htWindow.DisplayMultiRegion(BondVerifyRegions, bond2ModelObject.DieImage);
        }

        private void ExecuteBatchModifyVerifyParasCommand(object parameter)  //add by wj 2021-01-07
        {
            if (isRightClick != true) return;
            if (BondVerifyUserRegions.Count() == 0)
            {
                MessageBox.Show("请添加焊点检测区域！");
                return;
            }
            try
            {
                switch (BondWireParameter.AlgoParameterIndex)
                {
                    case 0:
                        //批量设置阈值分割方法参数
                        if (MessageBox.Show("是否进行参数批量修改?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            for (int i = 0; i < BondVerifyUserRegions.Count; i++)
                            {
                                if (BondVerifyUserRegions[i].IsSelected)
                                {
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ImageIndex = ImageIndex;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.IsCircleBond = BondVerifyModelPara.IsCircleBond;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondSize = BondVerifyModelPara.BondSize;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.EllipsBondSize = BondVerifyModelPara.EllipsBondSize;

                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ThreshGray = BondVerifyModelPara.ThreshGray;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ClosingSize = BondVerifyModelPara.ClosingSize;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondOverSizeFactor = BondVerifyModelPara.BondOverSizeFactor;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondUnderSizeFactor = BondVerifyModelPara.BondUnderSizeFactor;
                                }
                            }
                        }
                        break;
                    case 1:
                        //批量设置Measure方法参数
                        if (MessageBox.Show("是否进行参数批量修改?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            for (int i = 0; i < BondVerifyUserRegions.Count; i++)
                            {
                                if (BondVerifyUserRegions[i].IsSelected)
                                {
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.ImageIndex = ImageIndex;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.PreJudgeEnable = BondVerifyModelPara.PreJudgeEnable;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.SegThreshGray = BondVerifyModelPara.SegThreshGray;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.SegRegAreaFactor = BondVerifyModelPara.SegRegAreaFactor;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.BondOverSizeFactor = BondVerifyModelPara.BondOverSizeFactor;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMeasureAlgoPara.BondUnderSizeFactor = BondVerifyModelPara.BondUnderSizeFactor;
                                }
                            }
                        }
                        break;
                    case 2:
                        //批量设置Match方法参数
                        if (MessageBox.Show("是否进行参数批量修改?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            for (int i = 0; i < BondVerifyUserRegions.Count; i++)
                            {
                                if (BondVerifyUserRegions[i].IsSelected)
                                {
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.ImageIndex = ImageIndex;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.MinMatchScore = BondVerifyModelPara.MinMatchScore;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.AngleExt = BondVerifyModelPara.AngleExt;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.IsCircleBond = BondVerifyModelPara.IsCircleBond;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.BondSize = BondVerifyModelPara.BondSize;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.EllipsBondSize = BondVerifyModelPara.EllipsBondSize;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.IsBondRegRefine = BondVerifyModelPara.IsBondRegRefine;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.AddBallNum = BondVerifyModelPara.AddBallNum;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.MaxOverlap = BondVerifyModelPara.MaxOverlap;
                                    BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.MinHistScore = BondVerifyModelPara.MinHistScore;

                                }
                            }
                        }
                        break;
                    default: break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
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
                        BondWireParameter.VerifyImagesDirectory = fbd.SelectedPath;
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
                                                                                BondWireParameter.VerifyImagesDirectory,
                                                                                Bond2ModelParameter.CurFovName,
                                                                                0, out HTuple hv_o_ImageVerifyNum, out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        BondWireParameter.CurrentVerifySet = hv_o_ImageVerifyNum;
                        PImageIndexPath = imageFiles[BondWireParameter.ImageChannelIndex];
                        ImageVerify = ho_MutiImage;
                    }
                    else
                    {
                        isFovTaskFlag = 0;

                        Algorithm.File.list_image_files(BondWireParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        string[] folderList = imageFiles;
                        BondWireParameter.CurrentVerifySet = folderList.Count();
                        PImageIndexPath = imageFiles[0];
                        HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                        ImageVerify = image;
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondWireParameter.ImageChannelIndex);
                    htWindow.Display(ChannelImageVerify, true);
                    pImageIndex = 0;
                    imgIndex = 0;
                }
                LoadModels();
                isLoadCompleted = true;
            }
            catch (Exception ex)
            {
                isLoadCompleted = false;
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        // 加载默认图集
        private void ExecuteRefreshImagesSet(object parameter)
        {
            BondWireParameter.VerifyImagesDirectory = Bond2ModelParameter.VerifyImagesDirectory;
            if (Directory.Exists(BondWireParameter.VerifyImagesDirectory))
            {
                Algorithm.File.list_image_files(BondWireParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                string[] folderList = imageFiles;
                BondWireParameter.CurrentVerifySet = folderList.Count();
                PImageIndexPath = imageFiles[0];
                HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                ImageVerify = image;
                ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondWireParameter.ImageChannelIndex);
                htWindow.Display(ChannelImageVerify, true);
                pImageIndex = 0;
                imgIndex = 0;

                LoadModels();
                isLoadCompleted = true;
            }
        }

        private void ExecuteRefreshModels(object parameter)
        {
            try
            {
                LoadModels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void LoadModels()
        {
            if (isRightClick != true) return;
            HOperatorSet.ReadRegion(out DieRegions, $"{ReferenceDirectory}CoarseReference.reg");
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
        }

        //检测验证
        private void ExecuteVerifyCommand(object parameter)
        {
            if (isRightClick != true) return;

            if (!isLoadCompleted)
            {
                MessageBox.Show("请先加载图集！");
                return;
            }

            if (ImageVerify== null || !ImageVerify.IsInitialized())
            {
                MessageBox.Show("请先加载图集！");
                return;
            }

            if (DieRegions==null || !DieRegions.IsInitialized())
            {
                MessageBox.Show("请先加载搜索区！");
                return;
            }

            if (Groups.Count == 0)
            {
                MessageBox.Show("请先画焊点检测区域！");
                return;
            }
            
            if (Bond2ModelParameter.OnRecipesIndex == -1)
            {
                MessageBox.Show("请先选择焊点所在位置！");
                return;
            }
            if (Bond2ModelParameter.ImageCountChannels > 0 && BondWireParameter.ImageChannelIndex < 0) // 1122
            {
                MessageBox.Show("请先选择通道图像！");
                return;
            }
            if (modelID == null || modelID.TupleLength() < 1)
            {
                MessageBox.Show("请先创建定位模板或刷新模板！");
                return;
            }

            //Window_Loading window_Loading = new Window_Loading("正在检验");

            //window_Loading.Show();

            try
            {
                htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondWireParameter.ImageChannelIndex), true);

                Algorithm.Model_RegionAlg.HTV_confirm_mapping_matrix(Algorithm.Region.GetChannnelImageConcact(ImageVerify),//黑白图传1张；彩色图RGB三通道图concact在一起
                                           DieRegions.SelectObj(imgIndex + 1),
                                           ModelsFile,
                                           RecipeFile,
                                           Bond2ModelParameter.OnRecipesIndexs[Bond2ModelParameter.OnRecipesIndex].ToString(),
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
                HObject ho_EmptyObj = null,  ho__BondContour = null;
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
                HObject RotatedImage = null;
                HOperatorSet.ReadImage(out RotatedImage, $"{BondDirectory}RotatedImage1.tiff");
                //获取Bond检测区域
                HOperatorSet.AffineTransRegion(Algorithm.Region.ConcatRegion(BondVerifyUserRegions), out HObject _BondInspectRegs, HomMat2D, "nearest_neighbor");
                //

                for (int i = 0;i < BondVerifyUserRegions.Count();i++)
                {
                    HOperatorSet.SelectObj(_BondInspectRegs, out HObject _BondInspectReg, i + 1);

                    switch (BondVerifyUserRegions[i].AlgoParameterIndex)
                    {
                        case 0:
                            //阈值分割                          
                            Algorithm.Model_RegionAlg.HTV_Bond_Inspect_threshold(Algorithm.Region.GetChannnelImageConcact(ImageVerify), _BondInspectReg, out ho__BondContour,
                                        out ho__FailBondReg,
                                        BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ImageIndex+1,
                                        BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.IsCircleBond ? new HTuple(BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondSize) : new HTuple(BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.EllipsBondSize[0]).TupleConcat(new HTuple(BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.EllipsBondSize[1])),
                                        BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ThreshGray,
                                        BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.ClosingSize,
                                        BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondOverSizeFactor,
                                        BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondThresAlgoPara.BondUnderSizeFactor,
                                        out hv__DefectTypeReg,
                                        out hv__DefectImgIdxReg,
                                        out hvec__DefectValueReg,
                                        out hvec__RefValueReg,
                                        out hv__ErrRegCode,
                                        out hv__ErrRegStr);
                            break;
                        case 1:
                            //measure
                            MessageBox.Show("该检测区域暂不使用测量进行定位！");
                            return;
                        case 2:
                            //match
                            Algorithm.Model_RegionAlg.HTV_Bond_Inspect_model(Algorithm.Region.GetChannnelImageConcact(ImageVerify), RotatedImage,
                                            _BondInspectReg,
                                            out ho__BondContour,
                                            out ho__FailBondReg,
                                            BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.ImageIndex+1,
                                            BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.BallNum_OnRegion,
                                            Bond2ModelParameter.ModelType == 0 ? "ncc" : "shape",
                                            modelID,
                                            BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.MinMatchScore,
                                            BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.AngleStart,
                                            BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.AngleExt,
                                            BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.IsCircleBond ? new HTuple(BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.BondSize) : new HTuple(BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.EllipsBondSize[0]).TupleConcat(new HTuple(BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.EllipsBondSize[1])),
                                            BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.IsBondRegRefine == false ? 0 : 1,
                                            BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.AddBallNum,
                                            BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.MaxOverlap,
                                            BondVerifyUserRegions[i].BondVerifyRegionWithPara.BondMatchAlgoPara.MinHistScore,
                                            out hv__DefectTypeReg,
                                            out hv__DefectImgIdxReg,
                                            out hvec__DefectValueReg,
                                            out hvec__RefValueReg,
                                            out hv__ErrRegCode,
                                            out hv__ErrRegStr);
                            break;
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

                //检测结果显示
                if (ho__BondContours.CountObj() > 0)
                {
                    htWindow.DisplaySingleRegion(ho__BondContours.ConcatObj(DieRegions.SelectObj(imgIndex + 1)), Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondWireParameter.ImageChannelIndex));
                }

                Algorithm.Model_RegionAlg.HTV_Vector_to_ObjectConcat_1d(hvec__FailBondRegs, out HObject BondFailRegsConcat, out HTuple VerErrCode, out HTuple VerErrStr);
                Algorithm.Model_RegionAlg.HTV_Vector_to_Tuple_1d(hvec__DefectBondType, out HTuple BondDefectType, out HTuple _ErrCode, out HTuple _ErrStr);
                //寻找大于0的数组
                HOperatorSet.TupleSum(BondDefectType, out HTuple TypeSum);
                if (TypeSum > 0)
                {
                    htWindow.DisplaySingleRegion(BondFailRegsConcat.ConcatObj(ho__BondContours).ConcatObj(DieRegions.SelectObj(imgIndex + 1)), Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondWireParameter.ImageChannelIndex), "orange");
                }

                imgIndex++;
                if ((imgIndex + 1) > DieRegions.CountObj())
                {
                    imgIndex = 0;
                }
                ho__BondContours.Dispose();
                hvec__FailBondRegs.Dispose();
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
                if (BondWireParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == 0 ? BondWireParameter.CurrentVerifySet - 1 : --pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                BondWireParameter.VerifyImagesDirectory,
                                                                                Bond2ModelParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[BondWireParameter.ImageChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(BondWireParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondWireParameter.ImageChannelIndex);
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
                if (BondWireParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == BondWireParameter.CurrentVerifySet - 1 ? 0 : ++pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                BondWireParameter.VerifyImagesDirectory,
                                                                                Bond2ModelParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[BondWireParameter.ImageChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(BondWireParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondWireParameter.ImageChannelIndex);
                    htWindow.Display(ChannelImageVerify, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //保存
        private void ExecuteSaveCommand(object parameter)
        {
            if (isRightClick != true) return;
            try
            {
                if (Bond2ModelParameter.ImageCountChannels > 0 && BondWireParameter.ImageChannelIndex < 0)
                {
                    MessageBox.Show("请先选择验证图像通道！");
                    return;
                }
                OnSaveXML?.Invoke();
                MessageBox.Show("参数保存完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "参数保存失败");
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
            //1122
            ChannelNames = new ObservableCollection<ChannelName>(Bond2ModelParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = BondWireParameter.ImageChannelIndex;
            OnPropertyChanged("ImageIndex");
            //
            DisplayBondVerifyRegions(false);
            //htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BondWireParameter.ImageChannelIndex), true);
            

        }

        public void Dispose()
        {
            (Content as Page_BondMatchVerify).DataContext = null;
            (Content as Page_BondMatchVerify).Close();
            this.Content = null;
            this.htWindow = null;
            this.ImageVerify = null;
            this.ChannelImageVerify = null;
            this.Groups = null;
            this.bond2ModelObject = null;
            this.Bond2ModelParameter = null;
            this.BondWireParameter = null;
            this.ImageVerify = null;
            this.modelID = null;
            //add by wj 2021-01-11
            this.BondVerifyModelPara = null;
            BatchModifyVerifyParasCommand = null;
            IsBondVerifyRegionCheckCommand = null;
            IsBondVerifyRegionCheckAllCommand = null;
            BondVerifyRegionEnableChangedCommand = null;
            DisplayBondVerifyRegionsCommand = null;
            LoadBondVerifyRegionsCommand = null;
        }
    }
}
