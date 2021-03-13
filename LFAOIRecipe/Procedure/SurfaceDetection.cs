using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace LFAOIRecipe
{
    class SurfaceDetection : ViewModelBase, IProcedure
    {
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public event Action OnSaveXML;

        private HTHalControlWPF htWindow;

        private string referenceDirectory;
        public string ReferenceDirectory
        {
            get => referenceDirectory;
            set => OnPropertyChanged(ref referenceDirectory, value);
        }

        private bool isImageSetVerifyFrame = true;
        public bool IsImageSetVerifyFrame
        {
            get => isImageSetVerifyFrame;
            set => OnPropertyChanged(ref isImageSetVerifyFrame, value);
        }

        private bool isImageSetVerifyPegRack = true;
        public bool IsImageSetVerifyPegRack
        {
            get => isImageSetVerifyPegRack;
            set => OnPropertyChanged(ref isImageSetVerifyPegRack, value);
        }

        private bool isImageSetVerifyBridge = true;
        public bool IsImageSetVerifyBridge
        {
            get => isImageSetVerifyBridge;
            set => OnPropertyChanged(ref isImageSetVerifyBridge, value);
        }

        private readonly string ModelsFile;
        private readonly string RecipeFile;

        // 1211
        private int imageIndex0;
        public int ImageIndex0
        {
            get => imageIndex0;
            set
            {
                if (imageIndex0 != value)
                {
                    if (ImageVerify == null || !ImageVerify.IsInitialized())
                    {
                        if (value == -1)
                        {
                            HOperatorSet.AccessChannel(SurfaceDetectionObject.DieImage, out HObject ChannelImageDisplay, FrameModelInspectParameter.ImageFrameVerifyChannelIndex + 1);
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        else if (value >= 0)
                        {
                            HOperatorSet.AccessChannel(SurfaceDetectionObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                            FrameModelInspectParameter.ImageFrameVerifyChannelIndex = value;
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        imageIndex0 = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        if (imageIndex0 != value)
                        {
                            if (value == -1)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify0, FrameModelInspectParameter.ImageFrameVerifyChannelIndex + 1);
                                htWindow.Display(ChannelImageVerify0, true);
                            }
                            else if (value >= 0)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify0, value + 1);
                                FrameModelInspectParameter.ImageFrameVerifyChannelIndex = value;
                                htWindow.Display(ChannelImageVerify0, true);
                            }
                            imageIndex0 = value;
                            OnPropertyChanged();
                        }
                    }
                }
            }
        }

        private int imageIndex1;
        public int ImageIndex1
        {
            get => imageIndex1;
            set
            {
                if (imageIndex1 != value)
                {
                    if (ImageVerify == null || !ImageVerify.IsInitialized())
                    {
                        if (value == -1)
                        {
                            HOperatorSet.AccessChannel(SurfaceDetectionObject.DieImage, out HObject ChannelImageDisplay, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex + 1);
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        else if (value >= 0)
                        {
                            HOperatorSet.AccessChannel(SurfaceDetectionObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                            PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex = value;
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        imageIndex1 = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        if (imageIndex1 != value)
                        {
                            if (value == -1)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify1, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex + 1);
                                htWindow.Display(ChannelImageVerify1, true);
                            }
                            else if (value >= 0)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify1, value + 1);
                                PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex = value;
                                htWindow.Display(ChannelImageVerify1, true);
                            }
                            imageIndex1 = value;
                            OnPropertyChanged();
                        }
                    }
                }
            }
        }

        private int imageIndex2;
        public int ImageIndex2
        {
            get => imageIndex2;
            set
            {
                if (imageIndex2 != value)
                {
                    if (ImageVerify == null || !ImageVerify.IsInitialized())
                    {
                        if (value == -1)
                        {
                            HOperatorSet.AccessChannel(SurfaceDetectionObject.DieImage, out HObject ChannelImageDisplay, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex + 1);
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        else if (value >= 0)
                        {
                            HOperatorSet.AccessChannel(SurfaceDetectionObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                            BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex = value;
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        imageIndex2 = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        if (imageIndex2 != value)
                        {
                            if (value == -1)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify2, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex + 1);
                                htWindow.Display(ChannelImageVerify2, true);
                            }
                            else if (value >= 0)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify2, value + 1);
                                BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex = value;
                                htWindow.Display(ChannelImageVerify2, true);
                            }
                            imageIndex2 = value;
                            OnPropertyChanged();
                        }
                    }
                }
            }
        }

        private int isFovTaskFlag = 0;

        private bool IsLoadModel = false;

        private EpoxyModelObject SurfaceDetectionObject;

        public GoldenModelParameter goldenModelParameter { get; set; }

        public FrameModelInspectParameter FrameModelInspectParameter { get; set; }

        public PegRackModelInspectParameter PegRackModelInspectParameter { get; set; }

        public BridgeModelInspectParameter BridgeModelInspectParameter { get; set; }

        private string pImageIndexPath;
        public string PImageIndexPath
        {
            get => pImageIndexPath;
            set => OnPropertyChanged(ref pImageIndexPath, value);
        }

        private bool isLoadFreeRegionFrame=false;
        public bool IsLoadFreeRegionFrame
        {
            get => isLoadFreeRegionFrame;
            set => OnPropertyChanged(ref isLoadFreeRegionFrame, value);
        }

        private bool isLoadFreeRegionPegRack = false;
        public bool IsLoadFreeRegionPegRack
        {
            get => isLoadFreeRegionPegRack;
            set => OnPropertyChanged(ref isLoadFreeRegionPegRack, value);
        }

        private bool isLoadFreeRegionBridge = false;
        public bool IsLoadFreeRegionBridge
        {
            get => isLoadFreeRegionBridge;
            set => OnPropertyChanged(ref isLoadFreeRegionBridge, value);
        }

        private bool isLoadCutRegion=false;
        public bool IsLoadCutRegion
        {
            get => isLoadCutRegion;
            set => OnPropertyChanged(ref isLoadCutRegion, value);
        }

        public ObservableCollection<UserRegion> LoadRegionUserRegions { get; private set; }
        private IEnumerable<HObject> LoadRegionRegions => LoadRegionUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion);

        public CommandBase ImagesSetVerifyCommand { get; private set; }

        public CommandBase FrameVerifyCommand { get; private set; }
        public CommandBase PegRackVerifyCommand { get; private set; }
        public CommandBase BridgeVerifyCommand { get; private set; }

        public CommandBase SaveCommand { get; private set; }

        public CommandBase FramePreviousCommand { get; private set; }
        public CommandBase FrameNextCommand { get; private set; }
        public CommandBase PegRackPreviousCommand { get; private set; }
        public CommandBase PegRackNextCommand { get; private set; }
        public CommandBase BridgePreviousCommand { get; private set; }
        public CommandBase BridgeNextCommand { get; private set; }

        //public CommandBase RefreshModels { get; private set; }

        public CommandBase LoadReferenceCommand { get; private set; }

        public CommandBase LoadFreeRegionFrameCommand { get; private set; }
        public CommandBase LoadFreeRegionPegRackCommand { get; private set; }
        public CommandBase LoadFreeRegionBridgeCommand { get; private set; }
        public CommandBase LoadCutRegionCommand { get; private set; }
        public CommandBase LoadModelsCommand { get; private set; }
        public CommandBase RefreshImagesSetFrame{ get; private set; }
        public CommandBase RefreshImagesSetPegRack { get; private set; }
        public CommandBase RefreshImagesSetBridge { get; private set; }

        private double[] frameHomMat2D;
        public double[] FrameHomMat2D
        {
            get => frameHomMat2D;
            set => OnPropertyChanged(ref frameHomMat2D, value);
        }

        private HObject ImageVerify = null;
        private HObject ChannelImageVerify0, ChannelImageVerify1, ChannelImageVerify2 = null;

        private int imgIndex0 = 0;
        private int imgIndex1 = 0;
        private int imgIndex2 = 0;

        private int pImageIndex = -1;

        private readonly string ModelsRecipeDirectory;

        HTuple hv_CurLen = new HTuple();

        private HObject DieRegions, CutRegion = null;

        private HObject FreeRegionFrame, FreeRegionPegRack, FreeRegionBridge = null;

        private HTuple InspectItemNum = new HTuple();

        private HObjectVector FrameObjs = new HObjectVector(2);
        private HObjectVector IcObjs = new HObjectVector(2);
        private HObjectVector EpoxyObjs = new HObjectVector(2);
        private HObjectVector BondObjs = new HObjectVector(2);
        private HObjectVector WireObjs = new HObjectVector(2);

        private HTupleVector FrameModels = new HTupleVector(2);
        private HTupleVector IcModels = new HTupleVector(2);
        private HTupleVector EpoxyModels = new HTupleVector(2);
        private HTupleVector BondModels = new HTupleVector(2);
        private HTupleVector WireModels = new HTupleVector(2);
        private HTupleVector CutRegModels = new HTupleVector(1);
        //private HTupleVector FrameHomMat2DVector = new HTupleVector(1); 

        private HTupleVector Con_FrameInspectParas = new HTupleVector(3), Con_IcInspectParas = new HTupleVector(3);
        private HTupleVector Con_EpoxyInspectParas = new HTupleVector(4), Con_BondInspectParas = new HTupleVector(3);
        private HTupleVector Con_WireInspectParas = new HTupleVector(4);

        public SurfaceDetection(HTHalControlWPF htWindow,
                                        EpoxyModelObject SurfaceDetectionObject,
                                        GoldenModelParameter goldenModelParameter,
                                        string referenceDirectory,
                                        string modelsFile,
                                        string recipeFile,
                                        FrameModelInspectParameter frameModelInspectParameter,
                                        PegRackModelInspectParameter pegRackModelInspectParameter,
                                        BridgeModelInspectParameter bridgeModelInspectParameter,
                                        ObservableCollection<UserRegion> loadRegionUserRegions,
                                        string modelsRecipeDirectory)
        {
            DisplayName = "表面检测验证";
            Content = new Page_SurfaceDetection { DataContext = this };
            this.FrameModelInspectParameter = frameModelInspectParameter;
            this.PegRackModelInspectParameter = pegRackModelInspectParameter;
            this.BridgeModelInspectParameter = bridgeModelInspectParameter;
            this.htWindow = htWindow;
            this.SurfaceDetectionObject = SurfaceDetectionObject;
            this.goldenModelParameter = goldenModelParameter;
            this.ReferenceDirectory = referenceDirectory;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.LoadRegionUserRegions = loadRegionUserRegions;
            this.ModelsRecipeDirectory = modelsRecipeDirectory;

            ImagesSetVerifyCommand = new CommandBase(ExecuteImagesSetVerifyCommand);
            FrameVerifyCommand = new CommandBase(ExecuteFrameVerifyCommand);
            PegRackVerifyCommand = new CommandBase(ExecutePegRackVerifyCommand);
            BridgeVerifyCommand = new CommandBase(ExecuteBridgeVerifyCommand);

            SaveCommand = new CommandBase(ExecuteSaveCommand);

            FramePreviousCommand = new CommandBase(ExecuteFramePreviousCommand);
            FrameNextCommand = new CommandBase(ExecuteFrameNextCommand);
            PegRackPreviousCommand = new CommandBase(ExecutePegRackPreviousCommand);
            PegRackNextCommand = new CommandBase(ExecutePegRackNextCommand);
            BridgePreviousCommand = new CommandBase(ExecuteBridgePreviousCommand);
            BridgeNextCommand = new CommandBase(ExecuteBridgeNextCommand);
            //RefreshModels = new CommandBase(ExecuteRefreshModels);

            LoadReferenceCommand = new CommandBase(ExecuteLoadReferenceCommand);
            LoadFreeRegionFrameCommand = new CommandBase(ExecuteLoadFreeRegionFrameCommand);
            LoadFreeRegionPegRackCommand = new CommandBase(ExecuteLoadFreeRegionPegRackCommand);
            LoadFreeRegionBridgeCommand = new CommandBase(ExecuteLoadFreeRegionBridgeCommand);
            LoadCutRegionCommand = new CommandBase(ExecuteLoadCutRegionCommand);
            LoadModelsCommand = new CommandBase(ExecuteLoadModelsCommand);

            RefreshImagesSetFrame = new CommandBase(ExecuteRefreshImagesSetFrame);
            RefreshImagesSetPegRack = new CommandBase(ExecuteRefreshImagesSetPegRack);
            RefreshImagesSetBridge = new CommandBase(ExecuteRefreshImagesSetBridge);
        }

        //加载参考
        private void ExecuteLoadReferenceCommand(object parameter)
        {
            if (!Directory.Exists(ReferenceDirectory))
            {
                MessageBox.Show("请先创建全局数据！");
                return;
            }
            try
            {
                LoadReferenceData();

                HTuple filesFrame = new HTuple();
                HTuple filesIC = new HTuple();
                filesFrame = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                            "Frame*.*", SearchOption.TopDirectoryOnly);
                string[] files = filesFrame;
                string[] OnRecipesIndexs = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    OnRecipesIndexs[i] = Path.GetFileName(files[i]);
                }
                goldenModelParameter.OnRecipesIndexs = OnRecipesIndexs;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "全局数据文件不存在！");
                return;
            }
        }

        public void LoadReferenceData()
        {
            if (!(File.Exists($"{ReferenceDirectory}TrainningImagesDirectory.tup") && File.Exists($"{ReferenceDirectory}ReferenceImage.tiff")
            && File.Exists($"{ReferenceDirectory}DieReference.reg") && File.Exists($"{ReferenceDirectory}UserRegionForCutOutIndex.tup")
            && File.Exists($"{ReferenceDirectory}DieImageRowOffset.tup") && File.Exists($"{ReferenceDirectory}DieImageColumnOffset.tup")
            && File.Exists($"{ReferenceDirectory}CoarseReference.reg") && File.Exists($"{ReferenceDirectory}ReferenceImage.tiff")))
            {
                MessageBox.Show("全局数据不全，请创建！" + System.Environment.NewLine + System.Environment.NewLine + "提示：  "
                    + System.Environment.NewLine + "TrainningImagesDirectory.tup" + System.Environment.NewLine + "ImagePath.tup"
                     + System.Environment.NewLine + "DieReference.reg" + System.Environment.NewLine + "UserRegionForCutOutIndex.tup"
                      + System.Environment.NewLine + "DieImageRowOffset.tup" + System.Environment.NewLine + "DieImageColumnOffset.tup"
                       + System.Environment.NewLine + "CoarseReference.reg" + System.Environment.NewLine + "ReferenceImage.tiff");
                return;
            }

            HOperatorSet.ReadTuple(ReferenceDirectory + "TrainningImagesDirectory.tup", out HTuple TrainningImagesDirectoryTemp);
            goldenModelParameter.VerifyImagesDirectory = TrainningImagesDirectoryTemp;

            goldenModelParameter.ImagePath = $"{ReferenceDirectory}ReferenceImage.tiff";
            SurfaceDetectionObject.Image?.Dispose();
            LoadImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageRowOffset.tup", out HTuple dieImageRowOffset);
            goldenModelParameter.DieImageRowOffset = dieImageRowOffset;
            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageColumnOffset.tup", out HTuple dieImageColumnOffset);
            goldenModelParameter.DieImageColumnOffset = dieImageColumnOffset;

            HOperatorSet.ReadRegion(out HObject UserRegionForCutOut_Region, ReferenceDirectory + "DieReference.reg");
            HOperatorSet.ReduceDomain(SurfaceDetectionObject.Image, UserRegionForCutOut_Region, out HObject imageReduced);
            HOperatorSet.CropDomain(imageReduced, out HObject dieImage);
            SurfaceDetectionObject.DieImage = dieImage;
            //LoadDieImage();

            //1211 lw
            HOperatorSet.ReadTuple(ReferenceDirectory + "ImageChannelnumber.tup", out HTuple ImageChannelnumber);
            ChannelNames.Clear();
            for (int i = 0; i < ImageChannelnumber; i++)
            {
                ChannelName tmp_name = new ChannelName();
                tmp_name.Name = (i + 1).ToString();
                ChannelNames.Add(tmp_name);
            }

            imageIndex0 = FrameModelInspectParameter.ImageFrameVerifyChannelIndex;
            OnPropertyChanged("imageIndex0");

            imageIndex1 = PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex;
            OnPropertyChanged("imageIndex1");

            imageIndex2 = BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex;
            OnPropertyChanged("imageIndex2");

            //1214 lw
            HOperatorSet.TupleSplit(ReferenceDirectory, "\\", out HTuple hv_subStr);
            HTuple FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 3];
            goldenModelParameter.CurFovName = FOV_Name;

            HOperatorSet.ReadTuple(ReferenceDirectory + "UserRegionForCutOutIndex.tup", out HTuple UserRegionForCutOutIndexTemp);
            goldenModelParameter.UserRegionForCutOutIndex = UserRegionForCutOutIndexTemp;
            // UserRegionForCutOut = DieUserRegions.Where(u => u.Index == goldenModelParameter.UserRegionForCutOutIndex).FirstOrDefault();
            // SurfaceDetectionObject.UserRegionForCutOut = UserRegionForCutOut;
            htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, FrameModelInspectParameter.ImageFrameVerifyChannelIndex));

            //备用
            /*
            DieUserRegions.Clear();
            HOperatorSet.ReadRegion(out HObject _dieUserRegions, ReferenceDirectory + "CoarseReference.reg");
            HOperatorSet.CountObj(_dieUserRegions, out HTuple DieNumbers);
            for (int i = 0; i < DieNumbers; i++)
            {
                HOperatorSet.SmallestRectangle1(_dieUserRegions.SelectObj(i + 1), out HTuple row1_Rectangle, out HTuple column1_Rectangle, out HTuple row2_Rectangle, out HTuple column2_Rectangle);
                UserRegion _userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Rectangle1, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle);
                if (_userRegion == null) return;
                _userRegion.RegionType = RegionType.Rectangle1;
                DieUserRegions.Add(_userRegion);
                DieUserRegions[i].Index = i + 1;
            }
            */
        }

        public void LoadImage()
        {
            HOperatorSet.GenEmptyObj(out HObject image);
            HOperatorSet.ReadImage(out image, goldenModelParameter.ImagePath);
            htWindow.Display(image, true);
            SurfaceDetectionObject.Image = image;

            HOperatorSet.CountChannels(SurfaceDetectionObject.Image, out HTuple channels);
            goldenModelParameter.ImageCountChannels = channels;
            SurfaceDetectionObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.Image, goldenModelParameter.ImageChannelIndex);
            //SurfaceDetectionObject.ImageR = Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.Image, 1);
            //SurfaceDetectionObject.ImageG = Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.Image, 2);
            //SurfaceDetectionObject.ImageB = Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.Image, 3);
        }

        public void LoadDieImage()
        {
            try
            {
                SurfaceDetectionObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, goldenModelParameter.ImageChannelIndex);
                //SurfaceDetectionObject.DieImageR = Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, 1);
                //SurfaceDetectionObject.DieImageG = Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, 2);
                //SurfaceDetectionObject.DieImageB = Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteImagesSetVerifyCommand(object parameter)
        {
            try
            {
                using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        goldenModelParameter.VerifyImagesDirectory = fbd.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                    if (MessageBox.Show("是否为指定Fov的task图集类型？", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        isFovTaskFlag = 1;

                        // 指定Fov合成多通道图并显示第一张图
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                goldenModelParameter.VerifyImagesDirectory,
                                                                                goldenModelParameter.CurFovName,
                                                                                0, out HTuple hv_o_ImageVerifyNum, out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        goldenModelParameter.CurrentVerifySet = hv_o_ImageVerifyNum;
                        PImageIndexPath = imageFiles[FrameModelInspectParameter.ImageFrameVerifyChannelIndex];
                        ImageVerify = ho_MutiImage;
                    }
                    else
                    {
                        Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        string[] folderList = imageFiles;
                        goldenModelParameter.CurrentVerifySet = folderList.Count();
                        PImageIndexPath = imageFiles[0];
                        HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                        ImageVerify = image;
                    }

                    ChannelImageVerify0 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, FrameModelInspectParameter.ImageFrameVerifyChannelIndex);
                    ChannelImageVerify1 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex);
                    ChannelImageVerify2 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex);
                    htWindow.Display(ChannelImageVerify0, true);
                    pImageIndex = 0;
                    imgIndex0 = 0;
                    imgIndex1 = 0;
                    imgIndex2 = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // 加载默认图集
        private void ExecuteRefreshImagesSetFrame(object parameter)
        {
            if (Directory.Exists(goldenModelParameter.VerifyImagesDirectory))
            {
                Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                string[] folderList = imageFiles;
                goldenModelParameter.CurrentVerifySet = folderList.Count();
                PImageIndexPath = imageFiles[0];
                HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                ImageVerify = image;
                ChannelImageVerify0 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, FrameModelInspectParameter.ImageFrameVerifyChannelIndex);
                htWindow.Display(ChannelImageVerify0, true);
                pImageIndex = 0;
                imgIndex0 = 0;
            }
        }

        private void ExecuteRefreshImagesSetPegRack(object parameter)
        {
            if (Directory.Exists(goldenModelParameter.VerifyImagesDirectory))
            {
                Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                string[] folderList = imageFiles;
                goldenModelParameter.CurrentVerifySet = folderList.Count();
                PImageIndexPath = imageFiles[0];
                HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                ImageVerify = image;
                ChannelImageVerify1 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex);
                htWindow.Display(ChannelImageVerify1, true);
                pImageIndex = 0;
                imgIndex1 = 0;
            }
        }

        private void ExecuteRefreshImagesSetBridge(object parameter)
        {
            if (Directory.Exists(goldenModelParameter.VerifyImagesDirectory))
            {
                Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                string[] folderList = imageFiles;
                goldenModelParameter.CurrentVerifySet = folderList.Count();
                PImageIndexPath = imageFiles[0];
                HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                ImageVerify = image;
                ChannelImageVerify2 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex);
                htWindow.Display(ChannelImageVerify2, true);
                pImageIndex = 0;
                imgIndex2 = 0;
            }
        }

        /*
        private void ExecuteRefreshModels(object parameter)
        {
            try
            {
 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }        
        }
        */

        //加载FreeRegion
        private void ExecuteLoadFreeRegionFrameCommand(object parameter)
        {
            try
            {
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = "reg|*.reg";
                    ofd.Multiselect = false;
                    ofd.InitialDirectory = $"{ModelsFile}";
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    string freeRegionName = ofd.FileName;
                    HOperatorSet.GenEmptyObj(out FreeRegionFrame);
                    HOperatorSet.ReadRegion(out FreeRegionFrame, new HTuple(freeRegionName));
                    HOperatorSet.MoveRegion(FreeRegionFrame, out HObject DieFreeRegionFrame, -goldenModelParameter.DieImageRowOffset, -goldenModelParameter.DieImageColumnOffset);//calculation
                    //htWindow.DisplaySingleRegion(isImageSetVerifyFrame == false ? DieFreeRegionFrame : FreeRegionFrame, 
                    //                             isImageSetVerifyFrame == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, FrameModelInspectParameter.ImageFrameVerifyChannelIndex) 
                    //                             : Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.Image, FrameModelInspectParameter.ImageFrameVerifyChannelIndex));
                    htWindow.DisplaySingleRegion(DieFreeRegionFrame, Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, FrameModelInspectParameter.ImageFrameVerifyChannelIndex));
                    IsLoadFreeRegionFrame = true;

                    // 保存Frame区域
                    HOperatorSet.WriteRegion(FreeRegionFrame, $"{ModelsFile}{goldenModelParameter.OnRecipesIndexs[goldenModelParameter.OnRecipesIndex]}\\Frame_Region.reg");               
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void ExecuteLoadFreeRegionPegRackCommand(object parameter)
        {
            try
            {
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = "reg|*.reg";
                    ofd.Multiselect = false;
                    ofd.InitialDirectory = $"{ModelsFile}";
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    string freeRegionName = ofd.FileName;
                    HOperatorSet.GenEmptyObj(out FreeRegionPegRack);
                    HOperatorSet.ReadRegion(out FreeRegionPegRack, new HTuple(freeRegionName));
                    HOperatorSet.MoveRegion(FreeRegionPegRack, out HObject DieFreeRegionPegRack, -goldenModelParameter.DieImageRowOffset, -goldenModelParameter.DieImageColumnOffset);//calculation
                    //htWindow.DisplaySingleRegion(isImageSetVerifyPegRack == false ? DieFreeRegionPegRack : FreeRegionPegRack, 
                    //                             isImageSetVerifyPegRack == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex)
                    //                             : Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.Image, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex));
                    htWindow.DisplaySingleRegion(DieFreeRegionPegRack, Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex));
                    IsLoadFreeRegionPegRack = true;

                    // 保存PegRack区域
                    HOperatorSet.WriteRegion(FreeRegionPegRack, $"{ModelsFile}{goldenModelParameter.OnRecipesIndexs[goldenModelParameter.OnRecipesIndex]}\\PegRack_Region.reg");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void ExecuteLoadFreeRegionBridgeCommand(object parameter)
        {
            try
            {
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = "reg|*.reg";
                    ofd.Multiselect = false;
                    ofd.InitialDirectory = $"{ModelsFile}";
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    string freeRegionName = ofd.FileName;
                    HOperatorSet.GenEmptyObj(out FreeRegionBridge);
                    HOperatorSet.ReadRegion(out FreeRegionBridge, new HTuple(freeRegionName));
                    HOperatorSet.MoveRegion(FreeRegionBridge, out HObject DieFreeRegionBridge, -goldenModelParameter.DieImageRowOffset, -goldenModelParameter.DieImageColumnOffset);//calculation
                    //htWindow.DisplaySingleRegion(isImageSetVerifyBridge == false ? DieFreeRegionBridge : FreeRegionBridge, 
                    //                             isImageSetVerifyBridge == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex)
                    //                             : Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.Image, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex));
                    htWindow.DisplaySingleRegion(DieFreeRegionBridge, Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex));
                    IsLoadFreeRegionBridge = true;

                    // 保存Bridge区域
                    HOperatorSet.WriteRegion(FreeRegionBridge, $"{ModelsFile}{goldenModelParameter.OnRecipesIndexs[goldenModelParameter.OnRecipesIndex]}\\Bridge_Region.reg");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        //加载CutRegion
        private void ExecuteLoadCutRegionCommand(object parameter)
        {
            try
            {
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = "reg|*.reg";
                    ofd.Multiselect = false;
                    ofd.InitialDirectory = $"{ModelsFile}";
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    string cutRegionName = ofd.FileName;
                    HOperatorSet.GenEmptyObj(out CutRegion);
                    HOperatorSet.ReadRegion(out CutRegion, new HTuple(cutRegionName));
                    HOperatorSet.MoveRegion(CutRegion, out HObject DieCutRegion, -goldenModelParameter.DieImageRowOffset, -goldenModelParameter.DieImageColumnOffset);//calculation
                    htWindow.DisplaySingleRegion(DieCutRegion, Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, FrameModelInspectParameter.ImageFrameVerifyChannelIndex));
                    IsLoadCutRegion = true;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        //加载
        private void ExecuteLoadModelsCommand(object parameter)
        {
            try
            {
                Algorithm.Model_RegionAlg.JSCC_AOI_read_all_model(out  /*{eObjectVector,Dim=2}*/ FrameObjs,
                                                out  /*{eObjectVector,Dim=2}*/ IcObjs,
                                                out  /*{eObjectVector,Dim=2}*/ EpoxyObjs,
                                                out  /*{eObjectVector,Dim=2}*/ BondObjs,
                                                out  /*{eObjectVector,Dim=2}*/ WireObjs,
                                                ModelsFile,
                                                out InspectItemNum,
                                                out  /*{eTupleVector,Dim=2}*/ FrameModels,
                                                out  /*{eTupleVector,Dim=2}*/ IcModels,
                                                out  /*{eTupleVector,Dim=2}*/ EpoxyModels,
                                                out  /*{eTupleVector,Dim=2}*/ BondModels,
                                                out  /*{eTupleVector,Dim=2}*/ WireModels,
                                                out CutRegModels,
                                                out HTuple modelErrCode, out HTuple modelErrStr);
                if (modelErrCode < 0)
                {
                    MessageBox.Show("读Models文件异常！");
                    return;
                }

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_CurLen = new HTuple(FrameObjs[0].Length);
                }
                Algorithm.Model_RegionAlg.JSCC_AOI_read_all_inspect_parametrer(RecipeFile,
                                      out HTupleVector/*{eTupleVector,Dim=2}*/  FrameParameters,
                                      out HTupleVector/*{eTupleVector,Dim=2}*/  IcParameters,
                                      out HTupleVector/*{eTupleVector,Dim=2}*/  EpoxyParameters,
                                      out HTupleVector/*{eTupleVector,Dim=3}*/  BondParameters,
                                      out HTupleVector/*{eTupleVector,Dim=3}*/  WireParameters,
                                      out HTupleVector/*{eTupleVector,Dim=2}*/  Con_AroundBallInspectParas,
                                      out HTuple paraErrCode,
                                      out HTuple paraErrStr);

                if (paraErrCode < 0)
                {
                    MessageBox.Show("读Recipe文件异常！");
                    return;
                }

                Con_FrameInspectParas = ((new HTupleVector(3).Insert(0, FrameModels)).Insert(
                    1, FrameParameters));
                Con_IcInspectParas = ((new HTupleVector(3).Insert(0, IcModels)).Insert(
                    1, IcParameters));
                Con_EpoxyInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, EpoxyModels)))).Insert(
                    1, EpoxyParameters));
                Con_BondInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, BondModels)))).Insert(
                        1, BondParameters));
                Con_WireInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, WireModels)))).Insert(
                    1, WireParameters));

                //o_BondModels.at(fileidx) := { OnWhat,_InspectMethod,\  Model_Type,PosModel,BallNum_OnRegion,\
                //Match_StartAngle,Match_AngleExt,Match_MinScore,BallRadius}
                //o_BondModels.at(fileidx) := { OnWhat,_InspectMethod,MetrologyType,\ MetrologyHandle,BondOffsetFactor}
                HOperatorSet.GenEmptyObj(out DieRegions);
                HOperatorSet.ReadRegion(out DieRegions, $"{ReferenceDirectory}CoarseReference.reg");

                IsLoadModel = true;
                MessageBox.Show("加载参数完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //框架检测验证
        private void ExecuteFrameVerifyCommand(object parameter)
        {
            if (IsLoadModel != true)
            {
                MessageBox.Show("请先加载模板参数！");
                return;
            }
            if (isLoadFreeRegionFrame != true)
            {
                MessageBox.Show("请先加载框架FreeRegion！");
                return;
            }            
            if (goldenModelParameter.OnRecipesIndex == -1)
            {
                MessageBox.Show("请选择所在哪个框架上！");
                return;
            }
            if (FrameModelInspectParameter.IsFrameSurfaceInspect == false)
            {
                MessageBox.Show("框架表面检测没使能！");
                return;
            }
            if (DieRegions == null || !DieRegions.IsInitialized())
            {
                MessageBox.Show("请先加载模板(Die区域不存在)！");
                return;
            }
            if (SurfaceDetectionObject.Image == null || !SurfaceDetectionObject.Image.IsInitialized())
            {
                MessageBox.Show("请先加载全局！");
                return;
            }
            if (isImageSetVerifyFrame == true && (ImageVerify == null || !ImageVerify.IsInitialized()))
            {
                MessageBox.Show("请先加载图集！");
                return;
            }
            if (FreeRegionFrame == null || !FreeRegionFrame.IsInitialized())
            {
                MessageBox.Show("请先加载FreeRegion！");
                return;
            }
            if (CutRegion == null || !CutRegion.IsInitialized())
            {
                if (isImageSetVerifyFrame == false)
                {
                    MessageBox.Show("请先加载CutRegion！");
                    return;
                }
            }
            if (hv_CurLen.TupleLength() <= 0)
            {
                MessageBox.Show("请先加载全局！");
                return;
            }

            //CutRegModels

            try
            {
                if (goldenModelParameter.ImageCountChannels > 0 && FrameModelInspectParameter.ImageFrameVerifyChannelIndex < 0)
                {
                    MessageBox.Show("请先选择通道图像！");
                    return;
                }

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    FrameObjs[0].Insert(hv_CurLen, dh.Add(new HObjectVector(FreeRegionFrame)));
                }

                htWindow.Display(isImageSetVerifyFrame == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, FrameModelInspectParameter.ImageFrameVerifyChannelIndex) :
                                 Algorithm.Region.GetChannnelImageUpdate(ImageVerify, FrameModelInspectParameter.ImageFrameVerifyChannelIndex), true);

                Algorithm.Model_RegionAlg.JSCC_AOI_Inspect_Gen_CutReg_Unit(isImageSetVerifyFrame == false ? Algorithm.Region.GetChannnelImageConcact(SurfaceDetectionObject.Image)
                                                                           : Algorithm.Region.GetChannnelImageConcact(ImageVerify),
                                                                           DieRegions.SelectObj(IsImageSetVerifyFrame == false ? goldenModelParameter.UserRegionForCutOutIndex : imgIndex0 + 1),
                                                                           FrameObjs,
                                                                           IcObjs,
                                                                           EpoxyObjs,
                                                                           BondObjs,
                                                                           WireObjs,
                                                                           out HObject RejectReg,
                                                                           InspectItemNum,
                                                                           Con_FrameInspectParas,
                                                                           Con_IcInspectParas,
                                                                           Con_EpoxyInspectParas,
                                                                           Con_BondInspectParas,
                                                                           Con_WireInspectParas,
                                                                           CutRegModels,
                                                                           out HTupleVector FrameHomMat2DVector,
                                                                           out HTupleVector IcHomMat2D,
                                                                           out HTuple FrameSurfaceDefectType,
                                                                           out HTuple FrameSurfaceErrCode,
                                                                           out HTuple FrameSurfaceErrStr);

                //映射矩阵有效性判断
                HOperatorSet.TupleSum(FrameHomMat2DVector[goldenModelParameter.OnRecipesIndex].T, out HTuple _FrameHomMat);
                if (_FrameHomMat == -12.0)
                {
                    MessageBox.Show("框架没定位到！");
                    imgIndex0++;
                    if (imgIndex0 + 1 > DieRegions.CountObj())
                    {
                        imgIndex0 = 0;
                    }
                    return;
                }

                HTupleVector hvec__RegSegParas = new HTupleVector(1);

                //*方法一：Adaptive
                //_RegSegMethod  := 0   // 自适应阈值分割
                //_RegSegParas:= { _RegSegMethod, _AdaptiveMethod, _BlockSize, _Contrast, _LightOrDark}

                //*方法二：Global
                //_RegSegMethod  :=1   // 
                //* _RegSegParas   := { _RegSegMethod, _ThreshGray,ThreshGrayInOrOut, _MorphSize}


                if (FrameModelInspectParameter.RegSegMethodIndex == 0)
                {
                    hvec__RegSegParas = (((((new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(0)))).Insert(
                    1, new HTupleVector(FrameModelInspectParameter.AdaptiveMethod))).Insert(2, new HTupleVector(FrameModelInspectParameter.BlockSize))).
                    Insert(3, new HTupleVector(FrameModelInspectParameter.Contrast))).Insert(4, new HTupleVector(FrameModelInspectParameter.LightOrDark)));
                }
                else
                {
                    hvec__RegSegParas = (((((new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(1)))).Insert(
                    1, new HTupleVector(FrameModelInspectParameter.ThreshGray)))
                    .Insert(2, new HTupleVector(new HTuple(FrameModelInspectParameter.ThreshGrayInOrOut)))).
                    Insert(3, new HTupleVector(FrameModelInspectParameter.MorphSize))));
                }

                Algorithm.Model_RegionAlg.HTV_Frame_Inspect(isImageSetVerifyFrame == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.Image, FrameModelInspectParameter.ImageFrameVerifyChannelIndex)
                                                            : Algorithm.Region.GetChannnelImageUpdate(ImageVerify, FrameModelInspectParameter.ImageFrameVerifyChannelIndex),
                                                            FreeRegionFrame,
                                                            IsImageSetVerifyFrame == false ? CutRegion : RejectReg,// 加载Die:CutRegion 图集：动态生成CutRegModels,
                                                            out HObject FailRegs,
                                                            FrameHomMat2DVector[goldenModelParameter.OnRecipesIndex].T,
                                                            hvec__RegSegParas,
                                                            FrameModelInspectParameter.CloseSize,
                                                            FrameModelInspectParameter.MinLength,
                                                            FrameModelInspectParameter.MinWidth,
                                                            FrameModelInspectParameter.MinArea,
                                                            FrameModelInspectParameter.SelOperation,
                                                            FrameModelInspectParameter.ImageFrameVerifyChannelIndex + 1,
                                                            out HTuple FrameDefectType,
                                                            out HTuple DefectFrameImgIdx, 
                                                            out HTuple DefectFrameInsValue, 
                                                            out HTuple FrameErrCode, out HTuple FrameErrStr);

                FrameHomMat2D = FrameHomMat2DVector[goldenModelParameter.OnRecipesIndex].T;

                if (FailRegs.CountObj() > 0)
                {
                    HOperatorSet.MoveRegion(FailRegs, out HObject _FailRegs, -goldenModelParameter.DieImageRowOffset, -goldenModelParameter.DieImageColumnOffset);//
                    htWindow.DisplaySingleRegion(isImageSetVerifyFrame == false ? _FailRegs : FailRegs, 
                                                 isImageSetVerifyFrame == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, FrameModelInspectParameter.ImageFrameVerifyChannelIndex)
                                                 : Algorithm.Region.GetChannnelImageUpdate(ImageVerify, FrameModelInspectParameter.ImageFrameVerifyChannelIndex));
                }

                imgIndex0++;
                if (imgIndex0 + 1 > DieRegions.CountObj())
                {
                    imgIndex0 = 0;
                }

                //Algorithm.Model_RegionAlg.HTV_Arrange_Pos(modelID, (goldenModelParameter.ModelType == 0) ? "ncc" : "shape",
                //PosLocPara[0],
                //PosLocPara[1],
                //[2],
                //out HTuple PosRow, out HTuple PosCol);

                //htWindow.hTWindow.HalconWindow.SetColor("green");
                //HOperatorSet.DispCross(htWindow.hTWindow.HalconWindow, PosRow, PosCol, 40, PosLocPara[2]);
            }
            catch (Exception ex)
            {
                //window_Loading.Close();
                MessageBox.Show(ex.ToString(), "检测验证异常！");
                imgIndex0++;
                if (imgIndex0 + 1 > DieRegions.CountObj())
                {
                    imgIndex0 = 0;
                }
            }
        }

        //钉架检测验证
        private void ExecutePegRackVerifyCommand(object parameter)
        {
            if (IsLoadModel != true)
            {
                MessageBox.Show("请先加载模板参数！");
                return;
            }
            if (isLoadFreeRegionPegRack != true)
            {
                MessageBox.Show("请先加载钉架FreeRegion！");
                return;
            }
            if (goldenModelParameter.OnRecipesIndex == -1)
            {
                MessageBox.Show("请选择所在哪个框架上！");
                return;
            }
            if (PegRackModelInspectParameter.IsPegRackSurfaceInspect == false)
            {
                MessageBox.Show("钉架表面检测没使能！");
                return;
            }
            if (DieRegions == null || !DieRegions.IsInitialized())
            {
                MessageBox.Show("请先加载模板(Die区域不存在)！");
                return;
            }
            if (SurfaceDetectionObject.Image == null || !SurfaceDetectionObject.Image.IsInitialized())
            {
                MessageBox.Show("请先加载全局！");
                return;
            }
            if (isImageSetVerifyPegRack == true && (ImageVerify == null || !ImageVerify.IsInitialized()))
            {
                MessageBox.Show("请先加载图集！");
                return;
            }
            if (FreeRegionPegRack == null || !FreeRegionPegRack.IsInitialized())
            {
                MessageBox.Show("请先加载FreeRegion！");
                return;
            }
            if (CutRegion == null || !CutRegion.IsInitialized())
            {
                if (IsImageSetVerifyPegRack == false)
                {
                    MessageBox.Show("请先加载CutRegion！");
                    return;
                }
            }

            try
            {
                if (goldenModelParameter.ImageCountChannels > 0 && PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex < 0)
                {
                    MessageBox.Show("请先选择通道图像！");
                    return;
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    FrameObjs[0].Insert(hv_CurLen + 1, dh.Add(new HObjectVector(FreeRegionPegRack)));
                }

                htWindow.Display(isImageSetVerifyPegRack == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex) :
                                 Algorithm.Region.GetChannnelImageUpdate(ImageVerify, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex), true);

                Algorithm.Model_RegionAlg.JSCC_AOI_Inspect_Gen_CutReg_Unit(isImageSetVerifyPegRack == false ? Algorithm.Region.GetChannnelImageConcact(SurfaceDetectionObject.Image)
                                                                           : Algorithm.Region.GetChannnelImageConcact(ImageVerify),
                                                                           DieRegions.SelectObj(IsImageSetVerifyPegRack == false ? goldenModelParameter.UserRegionForCutOutIndex : imgIndex1 + 1),
                                                                           FrameObjs,
                                                                           IcObjs,
                                                                           EpoxyObjs,
                                                                           BondObjs,
                                                                           WireObjs,
                                                                           out HObject RejectReg,
                                                                           InspectItemNum,
                                                                           Con_FrameInspectParas,
                                                                           Con_IcInspectParas,
                                                                           Con_EpoxyInspectParas,
                                                                           Con_BondInspectParas,
                                                                           Con_WireInspectParas,
                                                                           CutRegModels,
                                                                           out HTupleVector PegRackHomMat2DVector,
                                                                           out HTupleVector IcHomMat2D,
                                                                           out HTuple PegRackSurfaceDefectType,
                                                                           out HTuple PegRackSurfaceErrCode,
                                                                           out HTuple PegRackSurfaceErrStr);

                HOperatorSet.TupleSum(PegRackHomMat2DVector[goldenModelParameter.OnRecipesIndex].T, out HTuple _FrameHomMat);

                if (_FrameHomMat == -12.0)
                {
                    MessageBox.Show("框架没定位到！");
                    imgIndex1++;
                    if (imgIndex1 + 1 > DieRegions.CountObj())
                    {
                        imgIndex1 = 0;
                    }
                    return;
                }

                HTupleVector hvec__RegSegParas = new HTupleVector(1);

                if (PegRackModelInspectParameter.RegSegMethodIndex == 0)
                {
                    hvec__RegSegParas = (((((new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(0)))).Insert(
                    1, new HTupleVector(PegRackModelInspectParameter.AdaptiveMethod))).Insert(2, new HTupleVector(PegRackModelInspectParameter.BlockSize))).
                    Insert(3, new HTupleVector(PegRackModelInspectParameter.Contrast))).Insert(4, new HTupleVector(PegRackModelInspectParameter.LightOrDark)));

                }
                else
                {
                    hvec__RegSegParas = (((((new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(1)))).Insert(
                    1, new HTupleVector(PegRackModelInspectParameter.ThreshGray)))
                    .Insert(2, new HTupleVector(new HTuple(PegRackModelInspectParameter.ThreshGrayInOrOut)))).
                    Insert(3, new HTupleVector(PegRackModelInspectParameter.MorphSize))));
                }

                Algorithm.Model_RegionAlg.HTV_PegRack_Inspect(isImageSetVerifyPegRack == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.Image, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex)
                                                            : Algorithm.Region.GetChannnelImageUpdate(ImageVerify, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex),
                                                            FreeRegionPegRack,
                                                            IsImageSetVerifyPegRack == false ? CutRegion : RejectReg,
                                                            out HObject FailRegs,
                                                            PegRackHomMat2DVector[goldenModelParameter.OnRecipesIndex].T,
                                                            hvec__RegSegParas,
                                                            PegRackModelInspectParameter.CloseSize,
                                                            PegRackModelInspectParameter.MinLength,
                                                            PegRackModelInspectParameter.MinWidth,
                                                            PegRackModelInspectParameter.MinArea,
                                                            PegRackModelInspectParameter.SelOperation,
                                                            PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex + 1,
                                                            out HTuple PegRackDefectType,
                                                            out HTuple DefectPegRackImgIdx,
                                                            out HTupleVector DefectPegRackInsValue,
                                                            out HTuple PegRackErrCode, out HTuple PegRackErrStr);

                FrameHomMat2D = PegRackHomMat2DVector[goldenModelParameter.OnRecipesIndex].T;

                if (FailRegs.CountObj() > 0)
                {
                    HOperatorSet.MoveRegion(FailRegs, out HObject _FailRegs, -goldenModelParameter.DieImageRowOffset, -goldenModelParameter.DieImageColumnOffset);//
                    htWindow.DisplaySingleRegion(isImageSetVerifyPegRack == false ? _FailRegs : FailRegs, 
                                                 isImageSetVerifyPegRack == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex) 
                                                 : Algorithm.Region.GetChannnelImageUpdate(ImageVerify, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex));
                    //Window.DisplayMultiRegions(FailRegs);
                }
                imgIndex1++;
                if (imgIndex1 + 1 > DieRegions.CountObj())
                {
                    imgIndex1 = 0;
                }
            }
            catch (Exception ex)
            {
                //window_Loading.Close();
                MessageBox.Show(ex.ToString(), "检测验证异常！");
                imgIndex1++;
                if (imgIndex1 + 1 > DieRegions.CountObj())
                {
                    imgIndex1 = 0;
                }
            }
        }

        //桥接检测验证
        private void ExecuteBridgeVerifyCommand(object parameter)
        {
            if (IsLoadModel != true)
            {
                MessageBox.Show("请先加载模板参数！");
                return;
            }
            if (isLoadFreeRegionBridge != true)
            {
                MessageBox.Show("请先加载桥接FreeRegion！");
                return;
            }
            if (goldenModelParameter.OnRecipesIndex == -1)
            {
                MessageBox.Show("请选择所在哪个框架上！");
                return;
            }
            if (BridgeModelInspectParameter.IsBridgeSurfaceInspect == false)
            {
                MessageBox.Show("桥接表面检测没使能！");
                return;
            }
            if (DieRegions == null || !DieRegions.IsInitialized())
            {
                MessageBox.Show("请先加载模板(Die区域不存在)！");
                return;
            }
            if (SurfaceDetectionObject.Image == null || !SurfaceDetectionObject.Image.IsInitialized())
            {
                MessageBox.Show("请先加载全局！");
                return;
            }
            if (isImageSetVerifyBridge == true && (ImageVerify == null || !ImageVerify.IsInitialized()))
            {
                MessageBox.Show("请先加载图集！");
                return;
            }
            if (FreeRegionBridge == null || !FreeRegionBridge.IsInitialized())
            {
                MessageBox.Show("请先加载FreeRegion！");
                return;
            }
            if (CutRegion == null || !CutRegion.IsInitialized())
            {
                if (IsImageSetVerifyBridge == false)
                {
                    MessageBox.Show("请先加载CutRegion！");
                    return;
                }
            }

            try
            {
                if (goldenModelParameter.ImageCountChannels > 0 && BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex < 0)
                {
                    MessageBox.Show("请先选择通道图像！");
                    return;
                }

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    FrameObjs[0].Insert(hv_CurLen + 2, dh.Add(new HObjectVector(FreeRegionBridge)));
                }

                htWindow.Display(isImageSetVerifyBridge == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, FrameModelInspectParameter.ImageFrameVerifyChannelIndex) :
                                 Algorithm.Region.GetChannnelImageUpdate(ImageVerify, FrameModelInspectParameter.ImageFrameVerifyChannelIndex), true);

                Algorithm.Model_RegionAlg.JSCC_AOI_Inspect_Gen_CutReg_Unit(isImageSetVerifyBridge == false ? Algorithm.Region.GetChannnelImageConcact(SurfaceDetectionObject.Image)
                                                                           : Algorithm.Region.GetChannnelImageConcact(ImageVerify),
                                                                            DieRegions.SelectObj(isImageSetVerifyBridge == false ? goldenModelParameter.UserRegionForCutOutIndex : imgIndex2 + 1),
                                                                            FrameObjs,
                                                                            IcObjs,
                                                                            EpoxyObjs,
                                                                            BondObjs,
                                                                            WireObjs,
                                                                            out HObject RejectReg,
                                                                            InspectItemNum,
                                                                            Con_FrameInspectParas,
                                                                            Con_IcInspectParas,
                                                                            Con_EpoxyInspectParas,
                                                                            Con_BondInspectParas,
                                                                            Con_WireInspectParas,
                                                                            CutRegModels,
                                                                            out HTupleVector BridgeHomMat2DVector,
                                                                            out HTupleVector IcHomMat2D,
                                                                            out HTuple BridgeSurfaceDefectType,
                                                                            out HTuple BridgeSurfaceErrCode,
                                                                            out HTuple BridgeSurfaceErrStr);

                HOperatorSet.TupleSum(BridgeHomMat2DVector[goldenModelParameter.OnRecipesIndex].T, out HTuple _FrameHomMat);

                if (_FrameHomMat == -12.0)
                {
                    MessageBox.Show("框架没定位到！");
                    imgIndex2++;
                    if (imgIndex2 + 1 > DieRegions.CountObj())
                    {
                        imgIndex2 = 0;
                    }
                    return;
                }

                HTupleVector hvec__RegSegParas = new HTupleVector(1);

                if (BridgeModelInspectParameter.RegSegMethodIndex == 0)
                {
                    hvec__RegSegParas = (((((new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(0)))).Insert(
                    1, new HTupleVector(BridgeModelInspectParameter.AdaptiveMethod))).Insert(2, new HTupleVector(BridgeModelInspectParameter.BlockSize))).
                    Insert(3, new HTupleVector(BridgeModelInspectParameter.Contrast))).Insert(4, new HTupleVector(BridgeModelInspectParameter.LightOrDark)));

                }
                else
                {
                    hvec__RegSegParas = (((((new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(1)))).Insert(
                    1, new HTupleVector(BridgeModelInspectParameter.ThreshGray)))
                    .Insert(2, new HTupleVector(new HTuple(BridgeModelInspectParameter.ThreshGrayInOrOut)))).
                    Insert(3, new HTupleVector(BridgeModelInspectParameter.MorphSize))));
                }

                Algorithm.Model_RegionAlg.HTV_Bridge_Inspect(IsImageSetVerifyBridge == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.Image, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex)
                                                            : Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex),
                                                            FreeRegionBridge,
                                                            IsImageSetVerifyBridge == false ? CutRegion : RejectReg,
                                                            out HObject FailRegs,
                                                            BridgeHomMat2DVector[goldenModelParameter.OnRecipesIndex].T,
                                                            hvec__RegSegParas,
                                                            BridgeModelInspectParameter.CloseSize,
                                                            BridgeModelInspectParameter.MinLength,
                                                            BridgeModelInspectParameter.MinWidth,
                                                            BridgeModelInspectParameter.MinArea,
                                                            BridgeModelInspectParameter.SelOperation,
                                                            BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex + 1,
                                                            out HTuple BridgeDefectType,
                                                            out HTuple DefectBridgeImgIdx,
                                                            out HTupleVector DefectBridgeInsValue,
                                                            out HTuple BridgeErrCode, out HTuple BridgeErrStr);


                FrameHomMat2D = BridgeHomMat2DVector[goldenModelParameter.OnRecipesIndex].T;

                if (FailRegs.CountObj() > 0)
                {
                    HOperatorSet.MoveRegion(FailRegs, out HObject _FailRegs, -goldenModelParameter.DieImageRowOffset, -goldenModelParameter.DieImageColumnOffset);//
                    htWindow.DisplaySingleRegion(isImageSetVerifyBridge == false ? _FailRegs : FailRegs, 
                                                 isImageSetVerifyBridge == false ? Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex) 
                                                 : Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex));
                    //Window.DisplayMultiRegions(FailRegs);
                }
                imgIndex2++;
                if (imgIndex2 + 1 > DieRegions.CountObj())
                {
                    imgIndex2 = 0;
                }

                //Algorithm.Model_RegionAlg.HTV_Arrange_Pos(modelID, (goldenModelParameter.ModelType == 0) ? "ncc" : "shape",
                //PosLocPara[0],
                //PosLocPara[1],
                //[2],
                //out HTuple PosRow, out HTuple PosCol);

                //htWindow.hTWindow.HalconWindow.SetColor("green");
                //HOperatorSet.DispCross(htWindow.hTWindow.HalconWindow, PosRow, PosCol, 40, PosLocPara[2]);
            }
            catch (Exception ex)
            {
                //window_Loading.Close();
                MessageBox.Show(ex.ToString(), "检测验证异常！");
                imgIndex2++;
                if (imgIndex2 + 1 > DieRegions.CountObj())
                {
                    imgIndex2 = 0;
                }
            }
        }

        //保存
        private void ExecuteSaveCommand(object parameter)
        {
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

        private void ExecuteFramePreviousCommand(object parameter)
        {
            try
            {
                //imgIndex0 = 0;
                if (goldenModelParameter.CurrentVerifySet > 0 && pImageIndex >= 0)//改
                {
                    pImageIndex = pImageIndex == 0 ? goldenModelParameter.CurrentVerifySet - 1 : --pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                goldenModelParameter.VerifyImagesDirectory,
                                                                                goldenModelParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[FrameModelInspectParameter.ImageFrameVerifyChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify0 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, FrameModelInspectParameter.ImageFrameVerifyChannelIndex);
                    htWindow.Display(ChannelImageVerify0, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteFrameNextCommand(object parameter)
        {
            try
            {
                //imgIndex0 = 0;
                if (goldenModelParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == goldenModelParameter.CurrentVerifySet - 1 ? 0 : ++pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                goldenModelParameter.VerifyImagesDirectory,
                                                                                goldenModelParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[FrameModelInspectParameter.ImageFrameVerifyChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify0 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, FrameModelInspectParameter.ImageFrameVerifyChannelIndex);
                    htWindow.Display(ChannelImageVerify0, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecutePegRackPreviousCommand(object parameter)
        {
            try
            {
                //imgIndex1 = 0;
                if (goldenModelParameter.CurrentVerifySet > 0 && pImageIndex >= 0)//改
                {
                    pImageIndex = pImageIndex == 0 ? goldenModelParameter.CurrentVerifySet - 1 : --pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                goldenModelParameter.VerifyImagesDirectory,
                                                                                goldenModelParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify1 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex);
                    htWindow.Display(ChannelImageVerify1, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecutePegRackNextCommand(object parameter)
        {
            try
            {
                //imgIndex1 = 0;
                if (goldenModelParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == goldenModelParameter.CurrentVerifySet - 1 ? 0 : ++pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                goldenModelParameter.VerifyImagesDirectory,
                                                                                goldenModelParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify1 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex);
                    htWindow.Display(ChannelImageVerify1, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteBridgePreviousCommand(object parameter)
        {
            try
            {
                imgIndex2 = 0;
                if (goldenModelParameter.CurrentVerifySet > 0 && pImageIndex >= 0)//改
                {
                    pImageIndex = pImageIndex == 0 ? goldenModelParameter.CurrentVerifySet - 1 : --pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                goldenModelParameter.VerifyImagesDirectory,
                                                                                goldenModelParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify2 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex);
                    htWindow.Display(ChannelImageVerify2, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteBridgeNextCommand(object parameter)
        {
            try
            {
                imgIndex2 = 0;
                if (goldenModelParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == goldenModelParameter.CurrentVerifySet - 1 ? 0 : ++pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                goldenModelParameter.VerifyImagesDirectory,
                                                                                goldenModelParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify2 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex);
                    htWindow.Display(ChannelImageVerify2, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
            htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(SurfaceDetectionObject.DieImage, goldenModelParameter.ImageChannelIndex), true);
            //htWindow.Display(SurfaceDetectionObject.DieImage, true);
        }

        public void Dispose()
        {
            (Content as Page_SurfaceDetection).DataContext = null;
            (Content as Page_SurfaceDetection).Close();
            Content = null;
            htWindow = null;
            ImageVerify = null;
            ChannelImageVerify0 = null;
            ChannelImageVerify1 = null;
            ChannelImageVerify2 = null;
            SurfaceDetectionObject = null;
            goldenModelParameter = null;
            FrameModelInspectParameter = null;
            PegRackModelInspectParameter = null;
            BridgeModelInspectParameter = null;
            ImagesSetVerifyCommand = null;
            FrameVerifyCommand = null;
            PegRackVerifyCommand = null;
            BridgeVerifyCommand = null;
            SaveCommand = null;
            FramePreviousCommand = null;
            FrameNextCommand = null;
            PegRackPreviousCommand = null;
            PegRackNextCommand = null;
            BridgePreviousCommand = null;
            BridgeNextCommand = null;
            FrameHomMat2D = null;
            LoadRegionUserRegions = null;
            LoadReferenceCommand = null;
            LoadFreeRegionFrameCommand = null;
            LoadFreeRegionPegRackCommand = null;
            LoadFreeRegionBridgeCommand = null;
            LoadCutRegionCommand = null;
            LoadModelsCommand = null;
            RefreshImagesSetFrame = null;
            RefreshImagesSetPegRack = null;
            RefreshImagesSetBridge = null;
        }
    }
}
