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
    public class GoldenModelInspectVerify : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; } 

        public event Action OnSaveXML;

        private HTHalControlWPF htWindow;

        private readonly string ModelsFile;

        private readonly string RecipeFile;

        private GoldenModelObject goldenModelObject;

        public ObservableCollection<UserRegion> DieUserRegions { get; private set; }

        public ObservableCollection<UserRegion> MatchUserRegions { get; private set; }

        public ObservableCollection<UserRegion> InspectUserRegions { get; private set; }

        public ObservableCollection<UserRegion> RejectUserRegions { get; private set; }

        public ObservableCollection<UserRegion> SubUserRegions { get; private set; }

        private IEnumerable<HObject> DieRegions => DieUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public GoldenModelParameter GoldenModelParameter { get; set; }

        public GoldenModelInspectParameter GoldenModelInspectParameter { get; set; }

        private int isFovTaskFlag = 0;

        private int imgIndex = 0;
        private int pImageIndex = -1;

        private int imageIndex;
        public int ImageIndex
        {
            get => imageIndex;
            set
            {
                if (imageIndex!= value)
                {
                    GoldenModelInspectParameter.ImageChannelIndex_IcExist = value;
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, GoldenModelInspectParameter.ImageChannelIndex_IcExist);
                    htWindow.Display(ChannelImageVerify, true);
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private string pImageIndexPath;
        public string PImageIndexPath
        {
            get => pImageIndexPath;
            set => OnPropertyChanged(ref pImageIndexPath, value);
        }

        private HObject ImageVerify = null;
        private HObject ChannelImageVerify = null;

        HTuple PosModelID = null;

        private readonly string ReferenceDirectory;
        private readonly string RecipeDirectory;
        private readonly string ModelsRecipeDirectory;

        public HObject FailRegResult = null;
        public HObject IcRegResult = null;

        public CommandBase VerifyCommand { get; private set; }
        public CommandBase SaveCommand { get; private set; }
        public CommandBase DisplayLightDarkImageCommand { get; private set; }
        public CommandBase ImagesSetVerifyCommand { get; private set; }
        public CommandBase PreviousCommand { get; private set; }
        public CommandBase NextCommand { get; private set; }
        public CommandBase ICOnCommand { get; private set; }
        public CommandBase RefreshModels { get; private set; }
        public CommandBase RefreshImagesSetModels { get; private set; }

        public GoldenModelInspectVerify(HTHalControlWPF htWindow,
                                        string modelsFile,
                                        string recipeFile,
                                        string referenceDirectory,
                                        GoldenModelObject goldenModelObject,
                                        GoldenModelParameter goldenModelParameter,
                                        GoldenModelInspectParameter goldenModelInspectParameter,
                                        ObservableCollection<UserRegion> dieUserRegions,
                                        ObservableCollection<UserRegion> matchUserRegions,
                                        ObservableCollection<UserRegion> inspectUserRegions,
                                        ObservableCollection<UserRegion> rejectUserRegions,
                                        ObservableCollection<UserRegion> subUserRegions,
                                        string recipeDirectory,
                                        string modelsRecipeDirectory)
        {
            DisplayName = "检测验证";
            Content = new Page_GoldenModelInspectVerify { DataContext = this };

            this.htWindow = htWindow;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.ReferenceDirectory = referenceDirectory;
            this.goldenModelObject = goldenModelObject;
            this.GoldenModelParameter = goldenModelParameter;
            this.GoldenModelInspectParameter = goldenModelInspectParameter;
            this.DieUserRegions = dieUserRegions;
            this.MatchUserRegions = matchUserRegions;
            this.InspectUserRegions = inspectUserRegions;
            this.RejectUserRegions = rejectUserRegions;
            this.SubUserRegions = subUserRegions;
            this.ModelsRecipeDirectory=modelsRecipeDirectory;
            this.RecipeDirectory = recipeDirectory;

            ICOnCommand = new CommandBase(ExecuteICOnCommand);
            VerifyCommand = new CommandBase(ExecuteVerifyCommand);
            SaveCommand = new CommandBase(ExecuteSaveCommand);
            ImagesSetVerifyCommand = new CommandBase(ExecuteImagesSetVerifyCommand);
            PreviousCommand = new CommandBase(ExecutePreviousCommand);
            NextCommand = new CommandBase(ExecuteNextCommand);
            ImageVerify = goldenModelObject?.Image;//改
            RefreshModels = new CommandBase(ExecuteRefreshModels);
            RefreshImagesSetModels = new CommandBase(ExecuteRefreshImagesSetModels);
        }


        private void ExecuteICOnCommand(object parameter)
        {
            try
            {
                HTuple filesFrame = new HTuple();
                filesFrame = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                                      "Frame*.*", SearchOption.TopDirectoryOnly);
                GoldenModelParameter.OnRecipesIndexs = new string[filesFrame.Length];

                for (int i = 0; i < filesFrame.Length; i++)
                {
                    GoldenModelParameter.OnRecipesIndexs[i] = Path.GetFileName(filesFrame[i]);
                }
                AddBondOnItem();
                (Content as Page_GoldenModelInspectVerify).ICOnWhat.IsDropDownOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void AddBondOnItem()
        {
            (Content as Page_GoldenModelInspectVerify).ICOnWhat.Items.Clear();
            for (int j = 0; j < GoldenModelParameter.OnRecipesIndexs.Length; j++)
            {
                ComboBoxItem cboxItem = new ComboBoxItem();
                cboxItem.Content = GoldenModelParameter.OnRecipesIndexs[j];
                (Content as Page_GoldenModelInspectVerify).ICOnWhat.Items.Add(cboxItem);
            }
        }

        //加载图集
        private void ExecuteImagesSetVerifyCommand(object parameter)
        {
            try
            {
                using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        GoldenModelParameter.VerifyImagesDirectory = fbd.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                }

                // 1201 lw
                if (MessageBox.Show("是否为指定Fov的task图集类型？", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    isFovTaskFlag = 1;

                    // 指定Fov合成多通道图并显示第一张图
                    Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                            GoldenModelParameter.VerifyImagesDirectory,
                                                                            GoldenModelParameter.CurFovName,
                                                                            0, out HTuple hv_o_ImageVerifyNum, out HTuple imageFiles,
                                                                            out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                    if ((int)(hv_o_ImgErrorCode) < 0)
                    {
                        MessageBox.Show(hv_o_ImgErrorStr.ToString());

                        return;
                    }

                    GoldenModelParameter.CurrentVerifySet = hv_o_ImageVerifyNum;
                    PImageIndexPath = imageFiles[GoldenModelParameter.ImageChannelIndex];
                    ImageVerify = ho_MutiImage;
                }
                else
                {
                    isFovTaskFlag = 0;

                    Algorithm.File.list_image_files(GoldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                    string[] folderList = imageFiles;
                    GoldenModelParameter.CurrentVerifySet = folderList.Count();
                    PImageIndexPath = imageFiles[0];
                    HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                    ImageVerify = image;
                }
                ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, GoldenModelInspectParameter.ImageChannelIndex_IcExist);
                htWindow.Display(ChannelImageVerify, true); // 1122
                pImageIndex = 0;
                imgIndex = 0;
                LoadModels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
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

        private void ExecuteRefreshImagesSetModels(object parameter)
        {
            if (Directory.Exists(GoldenModelParameter.VerifyImagesDirectory))
            {
                if (GoldenModelInspectParameter.ImageChannelIndex_IcExist < 0)
                {
                    MessageBox.Show("请选择图像通道！");
                    return;
                }

                Algorithm.File.list_image_files(GoldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                string[] folderList = imageFiles;
                GoldenModelParameter.CurrentVerifySet = folderList.Count();
                PImageIndexPath = imageFiles[0];
                HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                ImageVerify = image;
                ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, GoldenModelInspectParameter.ImageChannelIndex_IcExist);
                htWindow.Display(ChannelImageVerify, true); // 1122
                pImageIndex = 0;
                imgIndex = 0;
                LoadModels();
            }
        }

        private void LoadModels()
        {
            if (goldenModelObject.PosModelID != null)
            {
                PosModelID = goldenModelObject.PosModelID;
            }
            else if (File.Exists($"{RecipeDirectory}PosModel.dat"))
            {
                // 清除已有模板 0115 -lw
                if (PosModelID != null && PosModelID.TupleLength() > 0)
                {
                    HTuple ModelTypeStr = null;
                    ModelTypeStr = GoldenModelParameter.ModelType == 0 ? "ncc" : "shape";
                    Algorithm.Model_RegionAlg.HTV_clear_model(PosModelID, ModelTypeStr);
                }

                PosModelID = Algorithm.File.ReadModel($"{RecipeDirectory}PosModel.dat", GoldenModelParameter.ModelType);
            }
            else
            {
                MessageBox.Show("没有定位模板，请返回上一步重新生成模板！");
                return;
            }
            int IsStdImageExist = 1;
            HOperatorSet.GenEmptyObj(out HObject Mean_Image);
            HOperatorSet.GenEmptyObj(out HObject Std_Image);
            if (goldenModelObject.MeadImage != null && goldenModelObject.MeadImage.IsInitialized())
            {
                Mean_Image = goldenModelObject.MeadImage;
            }
            else if (File.Exists($"{RecipeDirectory}Mean_Image.tiff"))
            {
                HOperatorSet.ReadImage(out Mean_Image, $"{RecipeDirectory}Mean_Image.tiff");
            }
            else
            {
                MessageBox.Show("没有均值图像，请返回上一步重新生成模板！");
                return;
            }
            if (goldenModelObject.StdImage != null && goldenModelObject.StdImage.IsInitialized())
            {
                Std_Image = goldenModelObject.StdImage;
            }
            else if (File.Exists($"{RecipeDirectory}Std_Image.tiff"))
            {
                HOperatorSet.ReadImage(out Std_Image, $"{RecipeDirectory}Std_Image.tiff");
            }
            else
            {
                IsStdImageExist = 0;
                //MessageBox.Show("没有方差图像，请返回上一步重新生成模板！");
                //return;
            }

            if (IsStdImageExist == 1)
            { 
                Algorithm.Model_RegionAlg.HTV_CreateGoldenImage(Mean_Image,
                                                          Std_Image,
                                                          //Algorithm.Region.ConcatRegion(InspectUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                                          //Algorithm.Region.ConcatRegion(SubUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                                          Algorithm.Region.Union1Region(InspectUserRegions),
                                                          Algorithm.Region.Union1Region(SubUserRegions),
                                                          out HObject LightImage,
                                                          out HObject DarkImage,
                                                          GoldenModelInspectParameter.LightScaleFactors,
                                                          GoldenModelInspectParameter.DarkScaleFactors,
                                                          GoldenModelInspectParameter.SobelScaleFactors,
                                                          out HTuple hv_o_ErrCode, out HTuple hv_o_ErrStr);
                goldenModelObject.LightImage = LightImage;
                goldenModelObject.DarkImage = DarkImage;
                goldenModelObject.MeadImage = Mean_Image;
            }
            else
            {
                // 针对未保存方差图分支 1221 lw
                if (File.Exists($"{RecipeDirectory}Light_Image.tiff"))
                {
                    HOperatorSet.ReadImage(out HObject LightImage, $"{RecipeDirectory}Light_Image.tiff");
                    goldenModelObject.LightImage = LightImage;
                }
                else
                {
                    MessageBox.Show("未保存亮图像！");
                    return;
                }

                if (File.Exists($"{RecipeDirectory}Dark_Image.tiff"))
                {
                    HOperatorSet.ReadImage(out HObject DarkImage, $"{RecipeDirectory}Dark_Image.tiff");
                    goldenModelObject.DarkImage = DarkImage;
                }
                else
                {
                    MessageBox.Show("未保存暗图像！");
                    return;
                }

                goldenModelObject.MeadImage = Mean_Image;
            }
        }

        //检测验证
        private void ExecuteVerifyCommand(object parameter)
        {
            try
            {
                //Window_Loading window_Loading = new Window_Loading("正在检验");

                if (ImageVerify == null || !ImageVerify.IsInitialized())
                {
                    MessageBox.Show("请先加载图集 或刷新！");
                    return;
                }
                if (goldenModelObject.LightImage == null || !goldenModelObject.LightImage.IsInitialized())
                {
                    MessageBox.Show("没有亮图像！");
                    return;
                }
                if (goldenModelObject.DarkImage == null || !goldenModelObject.DarkImage.IsInitialized())
                {
                    MessageBox.Show("没有暗图像！");
                    return;
                }
                if (goldenModelObject.MeadImage == null || !goldenModelObject.MeadImage.IsInitialized())
                {
                    MessageBox.Show("没有均值图像！");
                    return;
                }
                if (GoldenModelParameter.ImageCountChannels > 1 && GoldenModelParameter.ImageChannelIndex < 0) // 1122
                {
                    MessageBox.Show("请先选择创建定位模板图像通道！");
                    return;
                }
                if (GoldenModelParameter.ImageCountChannels > 1 && GoldenModelParameter.ImageGoldChannelIndex < 0) // 1122
                {
                    MessageBox.Show("请先选择创建黄金模板图像通道！");
                    return;
                }
                if (GoldenModelParameter.ImageCountChannels > 1 && GoldenModelInspectParameter.ImageChannelIndex_IcExist < 0) // 1122
                {
                    MessageBox.Show("请先选择IC有无检测通道图像！");
                    return;
                }
                if (GoldenModelParameter.OnRecipesIndex==-1)
                {
                    MessageBox.Show("请先选择芯片On！");
                    return;
                }
                if (PosModelID == null || PosModelID.TupleLength() < 1)
                {
                    MessageBox.Show("请先创建定位模板或刷新模板！");
                    return;
                }

                htWindow.Display(ChannelImageVerify, true);

                Algorithm.Model_RegionAlg.HTV_confirm_mapping_matrix(Algorithm.Region.GetChannnelImageConcact(ImageVerify),//图像通道图反推 黑白图传1张；彩色图RGB三通道图concact在一起
                                                                     DieUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion).ElementAt(imgIndex),
                                                                     ModelsFile,
                                                                     RecipeFile,
                                                                     GoldenModelParameter.OnRecipesIndexs[GoldenModelParameter.OnRecipesIndex],
                                                                     out HTuple HomMat2D, 
                                                                     out HTuple _frameLocPara,
                                                                     out HTuple ErrCode, out HTuple ErrStr);
                if (ErrCode != 0)
                {
                    MessageBox.Show("框架没有定位到！");
                    imgIndex++;
                    if (imgIndex + 1 > DieUserRegions.Count)
                    {
                        imgIndex = 0;
                    }
                    return;
                }

                //window_Loading.Show();

                HTupleVector hvec_i_DefectValue = new HTupleVector(2);
                //单个IC_ID的DefectValue初始化：定位Score, [deltaX, deltaY], deltaAng, 芯片区异物信息, 崩边区异物信息
                hvec_i_DefectValue = (((((new HTupleVector(2).Insert(0, (new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(-2)))))).Insert(
                    1, (new HTupleVector(1).Insert(0, new HTupleVector((new HTuple(-2)).TupleConcat(
                    -2)))))).Insert(2, (new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(-2)))))).Insert(
                    3, (((new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(0)))).Insert(
                    1, new HTupleVector(new HTuple(0)))).Insert(2, new HTupleVector(new HTuple(0)))))).Insert(
                    4, (((new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(0)))).Insert(
                    1, new HTupleVector(new HTuple(0)))).Insert(2, new HTupleVector(new HTuple(0))))));

                //检测验证   修改2020-12-05
                Algorithm.Model_RegionAlg.HTV_Ic_Inspect_GoldenModel(Algorithm.Region.GetChannnelImageConcact(ImageVerify),//图像通道图反推 黑白图传1张；彩色图RGB三通道图concact在一起
                                    goldenModelObject.LightImage,
                                    goldenModelObject.DarkImage,
                                    goldenModelObject.MeadImage,
                                    //Algorithm.Region.ConcatRegion(InspectUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                    //Algorithm.Region.ConcatRegion(MatchUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                    //Algorithm.Region.ConcatRegion(RejectUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                    //Algorithm.Region.ConcatRegion(SubUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                    Algorithm.Region.Union1Region(InspectUserRegions),
                                    Algorithm.Region.Union1Region(MatchUserRegions),
                                    Algorithm.Region.Union1Region(RejectUserRegions),
                                    Algorithm.Region.Union1Region(SubUserRegions),
                                    out HObject ICFailRegs,
                                    out HObject IcReg,//cutRegion
                                    (new HTuple(GoldenModelParameter.ImageChannelIndex==0?1: GoldenModelParameter.ImageChannelIndex+1))//定位模板通道
                                            .TupleConcat(new HTuple(GoldenModelParameter.ImageChannelIndex == 0?1: GoldenModelParameter.ImageGoldChannelIndex+1))//黄金模板通道
                                            .TupleConcat(new HTuple(GoldenModelInspectParameter.ImageChannelIndex_IcExist == 0 ? 1 : GoldenModelInspectParameter.ImageChannelIndex_IcExist+1)),//ic有无检测通道
                                    new HTuple(GoldenModelInspectParameter.IsICExist == false ? 0 : 1)
                                            .TupleConcat(new HTuple(GoldenModelInspectParameter.IsICLocate == false ? 0 : 1))
                                            .TupleConcat(new HTuple(GoldenModelInspectParameter.IsICOffSet == false ? 0 : 1))
                                            .TupleConcat(new HTuple(GoldenModelInspectParameter.IsICSurfaceInspect == false ? 0 : 1)),//hv_i_TaskEnable,
                                    HomMat2D,
                                    _frameLocPara,
                                    GoldenModelInspectParameter.ThreshGray, GoldenModelInspectParameter.LightOrDark,
                                    GoldenModelInspectParameter.CloseSize, GoldenModelInspectParameter.IcSizeTh,
                                    GoldenModelInspectParameter.DilationSize,
                                    (GoldenModelParameter.ModelType == 0) ? "ncc" : "shape",
                                    PosModelID,
                                    GoldenModelInspectParameter.MinMatchScore,
                                    GoldenModelInspectParameter.AngleStart,
                                    GoldenModelInspectParameter.AngleExt,
                                    GoldenModelInspectParameter.MatchNum,
                                    GoldenModelInspectParameter.RowDiffTh,
                                    GoldenModelInspectParameter.ColDiffTh,
                                    GoldenModelInspectParameter.AngleDiffTh,//150,150,45
                                    GoldenModelInspectParameter.GrayContrast,//5,5
                                    GoldenModelInspectParameter.MinLength,
                                    GoldenModelInspectParameter.MinWidth,
                                    GoldenModelInspectParameter.MinArea,
                                    GoldenModelInspectParameter.SelOperation,
                                    new HTuple(GoldenModelInspectParameter.IsChromatismProcess == false ? 0 : 1),
                                    new HTuple(GoldenModelInspectParameter.IsGlobalChromatism == false ? 0 : 1),
                                    hvec_i_DefectValue,
                                    out HTuple ICLocPara,
                                    out HTuple HomMatMod2Img,
                                    out HTuple ICDefectType,
                                    out HTupleVector DefectValue,
                                    out HTuple DefectImgIdx,
                                    out HTuple ICErrCode, out HTuple ICErrStr);

                //对Ic定位结果进行位置映射
                Algorithm.Model_RegionAlg.HTV_Arrange_Pos(PosModelID,
                                                        (GoldenModelParameter.ModelType == 0) ? "ncc" : "shape",
                                                        ICLocPara[0],
                                                        ICLocPara[1],
                                                        ICLocPara[2],
                                                        out HTuple Rows,
                                                        out HTuple Cols);
                HOperatorSet.AreaCenter(IcReg, out HTuple Area0, out HTuple Rows0, out HTuple Cols0);
                HOperatorSet.GenCrossContourXld(out HObject cross, Rows0, Cols0, 40, ICLocPara[2]);

                //显示框架所在区域
                HObject FrameRegion = null;
                HObject cross1 = null;
                if (File.Exists($"{ModelsFile+ GoldenModelParameter.OnRecipesIndexs[GoldenModelParameter.OnRecipesIndex]}\\FrameRegion.reg"))
                {
                    HOperatorSet.ReadRegion(out HObject ho_FrameRegion, $"{ModelsFile+ GoldenModelParameter.OnRecipesIndexs[GoldenModelParameter.OnRecipesIndex]}\\FrameRegion.reg");

                    HOperatorSet.SmallestRectangle2(ho_FrameRegion, out HTuple Row, out HTuple Col, out HTuple Phi, out HTuple Len1, out HTuple Len2);

                    if(Len1 < 5 && Len2 < 5)
                    {
                        HOperatorSet.GenEmptyObj(out FrameRegion);

                        HOperatorSet.AffineTransRegion(Algorithm.Region.Union1Region(InspectUserRegions), out HObject RefIcRegion, HomMat2D, "nearest_neighbor");
                        HOperatorSet.SmallestRectangle2(RefIcRegion, out HTuple hv_Row, out HTuple hv_Col, out HTuple hv_Phi, out HTuple hv_Len1, out HTuple hv_Len2);
                        HOperatorSet.GenCrossContourXld(out cross1, hv_Row, hv_Col, 40, hv_Phi);
                    }
                    else
                    {
                        HOperatorSet.AffineTransRegion(ho_FrameRegion, out FrameRegion, HomMat2D, "nearest_neighbor");
                        HOperatorSet.SmallestRectangle2(FrameRegion, out HTuple hv_Row, out HTuple hv_Col, out HTuple hv_Phi, out HTuple hv_Len1, out HTuple hv_Len2);
                        HOperatorSet.GenCrossContourXld(out cross1, hv_Row, hv_Col, 40, hv_Phi);
                    }
                }
                else
                {
                    HOperatorSet.GenEmptyObj(out FrameRegion);
                    HOperatorSet.GenEmptyObj(out cross1);
                }

                if (IcReg.CountObj() > 0)
                {
                    FailRegResult = null;
                    IcRegResult = null;
                    htWindow.DisplaySingleRegion(IcReg.ConcatObj(DieRegions.ElementAt(imgIndex)).ConcatObj(cross).ConcatObj(cross1), ChannelImageVerify,"green");
                }

                if (ICErrCode > 0)
                {
                    // 分颜色显示 0118 lw
                    HOperatorSet.GenEmptyObj(out FailRegResult);
                    HOperatorSet.GenEmptyObj(out IcRegResult);
                    HOperatorSet.ConcatObj(FailRegResult, ICFailRegs.ConcatObj(cross), out FailRegResult);
                    HOperatorSet.ConcatObj(IcRegResult, IcReg.ConcatObj(DieRegions.ElementAt(imgIndex)).ConcatObj(FrameRegion).ConcatObj(cross1), out IcRegResult);

                    DisplayIcResultRegion();
                    //htWindow.DisplaySingleRegion(ICFailRegs.ConcatObj(IcReg).ConcatObj(DieRegions.ElementAt(imgIndex)).ConcatObj(cross), ChannelImageVerify, "orange");
                }

                // 显示偏移量
                HTuple XYDiff = DefectValue[1][0].T.Clone(); ;
                HTuple XDiff = XYDiff[0];
                HTuple YDiff = XYDiff[1];

                HTuple PhiDiff = DefectValue[2][0].T.Clone()[0];
                HTuple AngDiff = PhiDiff.TupleDeg();

                HOperatorSet.GetImageSize(ChannelImageVerify, out HTuple imageWidth, out HTuple imageHeight);             
                HalconDisp.DisplayMessage(htWindow.HTWindow.HalconWindow, string.Format("{0}  Row: {1:F2} pixel  Col: {2:F2} pixel  Ang: {3:F2}°", "芯片偏移量", (double)XDiff, (double)YDiff, (double)AngDiff), 
                                         "image", imageWidth * 1 / 16, imageHeight * 1 / 32, "blue", "true");

                // add lw 1119 判断映射矩阵有效性
                HOperatorSet.TupleSum(HomMatMod2Img, out HTuple hv_HomMatSum);
                if ((int)(new HTuple(hv_HomMatSum.TupleEqual(-12))) == 0)
                {            
                    htWindow.SET_Light_Dark_image(goldenModelObject.LightImage, goldenModelObject.DarkImage, HomMatMod2Img);
                }
                else
                {
                    MessageBox.Show("芯片没有定位到！");
                }

                imgIndex++;
                if (imgIndex + 1 > DieUserRegions.Count)
                {
                    imgIndex = 0;
                }

                ICFailRegs?.Dispose();
                IcReg?.Dispose();
                //window_Loading.Close();
            }
            catch (Exception ex)
            {
                //window_Loading.Close();
                MessageBox.Show(ex.ToString(), "检测验证出错,可能缺少模板数据！");
                imgIndex++;
                if (imgIndex + 1 > DieUserRegions.Count)
                {
                    imgIndex = 0;
                }
            }
        }

        // 0125 lw
        public void DisplayIcResultRegion()
        {
            if ((FailRegResult == null) || (IcRegResult == null))
            {
                return;
            }

            htWindow.InitialHWindow("orange");
            htWindow.hTWindow.HalconWindow.DispObj(IcRegResult);

            htWindow.InitialHWindow("red");
            htWindow.hTWindow.HalconWindow.DispObj(FailRegResult);

        }

        //前一页
        private void ExecutePreviousCommand(object parameter)
        {
            try
            {
                imgIndex = 0;
                if (GoldenModelParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == 0 ? GoldenModelParameter.CurrentVerifySet - 1 : --pImageIndex;

                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                GoldenModelParameter.VerifyImagesDirectory,
                                                                                GoldenModelParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[GoldenModelInspectParameter.ImageChannelIndex_IcExist];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(GoldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, GoldenModelInspectParameter.ImageChannelIndex_IcExist);
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
            try
            {
                imgIndex = 0;
                if (GoldenModelParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == GoldenModelParameter.CurrentVerifySet - 1 ? 0 : ++pImageIndex;

                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                GoldenModelParameter.VerifyImagesDirectory,
                                                                                GoldenModelParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[GoldenModelInspectParameter.ImageChannelIndex_IcExist];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(GoldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, GoldenModelInspectParameter.ImageChannelIndex_IcExist);                   
                    htWindow.Display(ChannelImageVerify, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteSaveCommand(object parameter)
        {
            try
            {
                if (GoldenModelParameter.ImageCountChannels > 0 && GoldenModelParameter.ImageChannelIndex < 0)
                {
                    MessageBox.Show("请先选择图像通道！");
                    return;
                }
                OnSaveXML?.Invoke();
                MessageBox.Show("参数保存完成");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "参数保存失败");
            }
        }

        public bool CheckCompleted()
        {
            return true;
        }

        public void Initial()
        {
            htWindow.ClearSelection();
            if (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff) return;
            htWindow.Display(ChannelImageVerify,true);

            //1121
            ChannelNames = new ObservableCollection<ChannelName>(GoldenModelParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = GoldenModelInspectParameter.ImageChannelIndex_IcExist;
            OnPropertyChanged("ImageIndex");

            // 0127
            GoldenModelParameter.IsInspectVerify = true;
        }

        public void Dispose()
        {
            (Content as Page_GoldenModelInspectVerify).DataContext = null;
            (Content as Page_GoldenModelInspectVerify).Close();
            Content = null;
            htWindow = null;
            ImageVerify = null;
            ChannelImageVerify = null;
            goldenModelObject = null;
            GoldenModelParameter = null;
            MatchUserRegions = null;
            InspectUserRegions = null;
            RejectUserRegions = null;
            SubUserRegions = null;
            VerifyCommand = null;
            SaveCommand = null;
            DisplayLightDarkImageCommand = null;
            DieUserRegions = null;
            GoldenModelInspectParameter = null;
        }
    }
}
