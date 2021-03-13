using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HalconDotNet;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Interactivity;

namespace LFAOIRecipe
{
    public class CreateAroundBondRegionModel : ViewModelBase, IProcedure
    {
        //1202
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public static bool isRightClick = true;

        public event Action OnSaveXML;

        public bool IsRefModel = false;

        public string ReferenceDirectory { get; set; }

        private string ModelsRecipeDirectory;

        private readonly string ModelsFile;

        private readonly string RecipeFile;
        //----区域
        private EpoxyModelObject AroundBondDetectionObject;

        private HObject DieRegions = null;

        public ObservableCollection<UserRegion> AroundBondRegUserRegions { get; private set; }
        private IEnumerable<HObject> AroundBondRegRegions => AroundBondRegUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion);
        private HTHalControlWPF htWindow;
        //
        private UserRegion selectedUserRegion;
        public UserRegion SelectedUserRegion
        {
            get => selectedUserRegion;
            set => OnPropertyChanged(ref selectedUserRegion, value);
        }

        private int isFovTaskFlag = 0;

        private int imageIndex0;
        public int ImageIndex0
        {
            get => imageIndex0;
            set
            {
                if (imageIndex0 != value && SelectedUserRegion != null)
                {
                    if (ImageVerify == null || !ImageVerify.IsInitialized())
                    {
                        if (value == -1)
                        {
                            HOperatorSet.AccessChannel(AroundBondDetectionObject.DieImage, out HObject ChannelImageDisplay, SelectedUserRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara.SurfImageIndex + 1);
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        else if (value >= 0)
                        {
                            HOperatorSet.AccessChannel(AroundBondDetectionObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                            SelectedUserRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara.SurfImageIndex = value;
                            AroundBondRegionModelInspectParameter.ImageChannelIndex = value;
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        imageIndex0 = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        // 1202
                        if (imageIndex0 != value && SelectedUserRegion != null)
                        {
                            if (value == -1)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify, SelectedUserRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara.SurfImageIndex + 1);
                                htWindow.Display(ChannelImageVerify, true);
                            }
                            else if (value >= 0)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify, value + 1);
                                SelectedUserRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara.SurfImageIndex = value;
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = value;
                                htWindow.Display(ChannelImageVerify, true);
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
                if (imageIndex1 != value && SelectedUserRegion != null)
                {
                    if (ImageVerify == null || !ImageVerify.IsInitialized())
                    {
                        if (value == -1)
                        {
                            HOperatorSet.AccessChannel(AroundBondDetectionObject.DieImage, out HObject ChannelImageDisplay, SelectedUserRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara.SurfImageIndex + 1);
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        else if (value >= 0)
                        {
                            HOperatorSet.AccessChannel(AroundBondDetectionObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                            SelectedUserRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara.SurfImageIndex = value;
                            AroundBondRegionModelInspectParameter.ImageChannelIndex = value;
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        imageIndex1 = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        // 1202
                        if (imageIndex1 != value && SelectedUserRegion != null)
                        {
                            if (value == -1)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify, SelectedUserRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara.SurfImageIndex + 1);
                                htWindow.Display(ChannelImageVerify, true);
                            }
                            else if (value >= 0)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify, value + 1);
                                SelectedUserRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara.SurfImageIndex = value;
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = value;
                                htWindow.Display(ChannelImageVerify, true);
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
                if (imageIndex2 != value && SelectedUserRegion != null)
                {
                    if (ImageVerify == null || !ImageVerify.IsInitialized())
                    {
                        if (value == -1)
                        {
                            HOperatorSet.AccessChannel(AroundBondDetectionObject.DieImage, out HObject ChannelImageDisplay, SelectedUserRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara.TailImageIndex + 1);
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        else if (value >= 0)
                        {
                            HOperatorSet.AccessChannel(AroundBondDetectionObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                            SelectedUserRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara.TailImageIndex = value;
                            AroundBondRegionModelInspectParameter.ImageChannelIndex = value;
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        imageIndex2 = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        // 1202
                        if (imageIndex2 != value && SelectedUserRegion != null)
                        {
                            if (value == -1)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify, SelectedUserRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara.TailImageIndex + 1);
                                htWindow.Display(ChannelImageVerify, true);
                            }
                            else if (value >= 0)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify, value + 1);
                                SelectedUserRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara.TailImageIndex = value;
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = value;
                                htWindow.Display(ChannelImageVerify, true);
                            }
                            imageIndex2 = value;
                            OnPropertyChanged();
                        }
                    }
                }
            }
        }

        private int imageIndex3;
        public int ImageIndex3
        {
            get => imageIndex3;
            set
            {
                if (imageIndex3 != value && SelectedUserRegion != null)
                {
                    if (ImageVerify == null || !ImageVerify.IsInitialized())
                    {
                        if (value == -1)
                        {
                            HOperatorSet.AccessChannel(AroundBondDetectionObject.DieImage, out HObject ChannelImageDisplay, SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.ShiftImageIndex + 1);
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        else if (value >= 0)
                        {
                            HOperatorSet.AccessChannel(AroundBondDetectionObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                            SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.ShiftImageIndex = value;
                            AroundBondRegionModelInspectParameter.ImageChannelIndex = value;
                            htWindow.Display(ChannelImageDisplay, false);
                        }
                        imageIndex3 = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        // 1202
                        if (imageIndex3 != value && SelectedUserRegion != null)
                        {
                            if (value == -1)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify, SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.ShiftImageIndex + 1);
                                htWindow.Display(ChannelImageVerify, true);
                            }
                            else if (value >= 0)
                            {
                                HOperatorSet.AccessChannel(ImageVerify, out ChannelImageVerify, value + 1);
                                SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.ShiftImageIndex = value;
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = value;
                                htWindow.Display(ChannelImageVerify, true);
                            }
                            imageIndex3 = value;
                            OnPropertyChanged();
                        }
                    }
                }
            }
        }

        //参数
        public AroundBondRegionModelInspectParameter AroundBondRegionModelInspectParameter { get; set; }
        public AroundBondRegionWithPara AroundBondRegionWithPara { get; set; }
        private UserRegion userRegionForCutOut;
        public UserRegion UserRegionForCutOut
        {
            get => userRegionForCutOut;
            set => OnPropertyChanged(ref userRegionForCutOut, value);
        }
        //----加载图像集
        private string pImageIndexPath;
        public string PImageIndexPath
        {
            get => pImageIndexPath;
            set => OnPropertyChanged(ref pImageIndexPath, value);
        }

        //---命令----参考区域
        public CommandBase LoadReferenceCommand { get; private set; }
        public CommandBase LoadResultRegionsCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ClickResponseCommand { get; private set; }

        public CommandBase SaveCommand { get; private set; }
        //---命令-----检测验证
        public CommandBase PreviousCommand { get; private set; }
        public CommandBase NextCommand { get; private set; }
        public CommandBase VerifyCommand { get; private set; }
        //----命令------加载图像集
        public CommandBase ImagesSetVerifyCommand { get; private set; }
        public CommandBase RefreshImagesSetCommand { get; private set; }

        //-----命令------检测验证     

        private HObject ImageVerify = null;
        private HObject ChannelImageVerify = null;

        private int imgIndex = 0;
        private int pImageIndex = -1;
        public HTuple InspectItemNum = null;


        //---------------------加载验证Bond结果
        //****Models读取结果
        private HObjectVector FrameObjs = new HObjectVector(2);
        private HObjectVector IcObjs = new HObjectVector(2);
        private HObjectVector EpoxyObjs = new HObjectVector(2);
        private HObjectVector BondObjs = new HObjectVector(2);
        private HObjectVector WireObjs = new HObjectVector(2);

        //
        public HTupleVector FrameModels = new HTupleVector(2);
        public HTupleVector IcModels = new HTupleVector(2);
        private HTupleVector EpoxyModels = new HTupleVector(2);
        public HTupleVector BondModels = new HTupleVector(2);
        private HTupleVector WireModels = new HTupleVector(2);
        private HTupleVector CutRegModels = new HTupleVector(1);

        //****Recipe读取结果
        private HTupleVector Con_FrameInspectParas = new HTupleVector(3), Con_IcInspectParas = new HTupleVector(3);
        private HTupleVector Con_EpoxyInspectParas = new HTupleVector(4), Con_BondInspectParas = new HTupleVector(3);
        private HTupleVector Con_WireInspectParas = new HTupleVector(4);
        private HTupleVector Con_AroundBallInspectParas = new HTupleVector(2);

        private HTupleVector AroundBallInspectParas = new HTupleVector(2);

        //检测验证输出初始化

        //--------------------创建AroundBondRegModel
        public CreateAroundBondRegionModel(HTHalControlWPF htWindow,
                                            string referenceDirectory,
                                            string modelsFile,
                                            string recipeFile,
                                            EpoxyModelObject aroundBondDetectionObject,
                                            AroundBondRegionModelInspectParameter aroundBondRegionModelInspectParameter,
                                            ObservableCollection<UserRegion> aroundBondRegUserRegions,
                                            string modelsRecipeDirectory)
        {
            DisplayName = "制作AroundBondRegion模板";
            Content = new Page_AroundBondRegionModel { DataContext = this };
            this.htWindow = htWindow;
            this.ReferenceDirectory = referenceDirectory;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.AroundBondDetectionObject = aroundBondDetectionObject;
            this.AroundBondRegionModelInspectParameter = aroundBondRegionModelInspectParameter;
            this.AroundBondRegUserRegions = aroundBondRegUserRegions;
            this.ModelsRecipeDirectory = modelsRecipeDirectory;

            //加载参考命令
            LoadReferenceCommand = new CommandBase(ExecuteLoadReferenceCommand);
            //加载检测验证Bond区域
            LoadResultRegionsCommand = new CommandBase(ExecuteLoadResultRegionsCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            //图集加载
            ImagesSetVerifyCommand = new CommandBase(ExecuteImagesSetVerifyCommand);
            RefreshImagesSetCommand = new CommandBase(ExecuteRefreshImagesSetCommand);
            //检测验证命令
            VerifyCommand = new CommandBase(ExecuteVerifyCommand);

            PreviousCommand = new CommandBase(ExecutePreviousCommand);

            NextCommand = new CommandBase(ExecuteNextCommand);
            //保存参数XML
            SaveCommand = new CommandBase(ExecuteSaveCommand);

            ImageVerify = aroundBondDetectionObject?.Image;
        }



        //----------------------加载参考
        private void ExecuteLoadReferenceCommand(object parameter)
        {
            if (!Directory.Exists(ReferenceDirectory))
            {
                MessageBox.Show("请先创建全局数据！");
                return;
            }
            try
            {
                //加载全局数据
                LoadReferenceData();
                htWindow.DisplayMultiRegion(AroundBondRegUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "全局数据文件不存在！");
                return;
            }
        }
        public void LoadReferenceData()
        {
            if (!(File.Exists($"{ReferenceDirectory}ReferenceImage.tiff")
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

            // 1202 lw
            if (AroundBondRegUserRegions.Count() > 0)
            {
                SelectedUserRegion = AroundBondRegUserRegions[0];
              
                if (SelectedUserRegion.IsAroundBondRegInspect == 1)
                {
                    //该Ball周围需要检测
                    if(SelectedUserRegion.AroundBondRegionWithPara.IsBallShiftInspect)
                    {
                        //进行焊点偏移检测
                        switch(SelectedUserRegion.AroundBondRegionWithPara.ShiftInspectMethodIndex)
                        {
                            case 0:
                                //选择Match_Measure方法
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.ShiftImageIndex;
                                break;
                            default:
                                break;
                        }
                    }
                    //该Ball进行尾丝检测
                    if (SelectedUserRegion.AroundBondRegionWithPara.IsTailInspect)
                    {
                        //进行焊点偏移检测
                        switch (SelectedUserRegion.AroundBondRegionWithPara.TailInspectMethodIndex)
                        {
                            case 0:
                                //选择Line_Gauss方法
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara.TailImageIndex;
                                break;
                            default:
                                break;
                        }
                    }
                    //该Ball进行焊盘表面检测
                    if (SelectedUserRegion.AroundBondRegionWithPara.IsSurfaceInspect)
                    {
                        //进行焊点偏移检测
                        switch (SelectedUserRegion.AroundBondRegionWithPara.SurfaceInspectMethodIndex)
                        {
                            case 0:
                                //选择Adaptive方法
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara.SurfImageIndex;
                                break;
                            case 1:
                                //选择Global方法
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara.SurfImageIndex;
                                break;
                            default:
                                break;
                        }
                    }
                }

                //switch (SelectedUserRegion.RegAlgParameterIndex)
                //{
                //    case 0:
                //        AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara.ImageChannelIndex;
                //        break;
                //    case 1:
                //        AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara.ImageChannelIndex;
                //        break;
                //    case 2:
                //        AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara.ImageChannelIndex;
                //        break;
                //    case 3:
                //        AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.ImageChannelIndex;
                //        break;
                //    default:
                //        break;
                //}
            }
            else
            {
                AroundBondRegionModelInspectParameter.ImageChannelIndex = 0;
            }

            //默认图集
            HOperatorSet.ReadTuple(ReferenceDirectory + "TrainningImagesDirectory.tup", out HTuple TrainningImagesDirectoryTemp);
            AroundBondRegionModelInspectParameter.VerifyImagesDirectory = TrainningImagesDirectoryTemp;

            AroundBondRegionModelInspectParameter.ImagePath = $"{ReferenceDirectory}ReferenceImage.tiff";
            AroundBondDetectionObject.Image?.Dispose();
            LoadImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageRowOffset.tup", out HTuple dieImageRowOffset);
            AroundBondRegionModelInspectParameter.DieImageRowOffset = dieImageRowOffset;
            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageColumnOffset.tup", out HTuple dieImageColumnOffset);
            AroundBondRegionModelInspectParameter.DieImageColumnOffset = dieImageColumnOffset;

            HOperatorSet.ReadRegion(out HObject UserRegionForCutOut_Region, ReferenceDirectory + "DieReference.reg");
            HOperatorSet.ReduceDomain(AroundBondDetectionObject.Image, UserRegionForCutOut_Region, out HObject imageReduced);
            HOperatorSet.CropDomain(imageReduced, out HObject dieImage);
            AroundBondDetectionObject.DieImage = dieImage;
            LoadDieImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "UserRegionForCutOutIndex.tup", out HTuple UserRegionForCutOutIndexTemp);
            AroundBondRegionModelInspectParameter.UserRegionForCutOutIndex = UserRegionForCutOutIndexTemp;
            //UserRegionForCutOut = DieUserRegions.Where(u => u.Index == CutRegionParameter.UserRegionForCutOutIndex).FirstOrDefault();
            //EpoxyModelObject.UserRegionForCutOut = UserRegionForCutOut;

            //1202 lw
            HOperatorSet.ReadTuple(ReferenceDirectory + "ImageChannelnumber.tup", out HTuple ImageChannelnumber);
            ChannelNames.Clear();
            for (int i = 0; i < ImageChannelnumber; i++)
            {
                ChannelName tmp_name = new ChannelName();
                tmp_name.Name = (i + 1).ToString();
                ChannelNames.Add(tmp_name);
            }

            AroundBondRegionModelInspectParameter.ChannelNames = ChannelNames;

            if (AroundBondRegUserRegions.Count() > 0)
            {
                SelectedUserRegion = AroundBondRegUserRegions[0];

                imageIndex0 = SelectedUserRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara.SurfImageIndex;
                OnPropertyChanged("imageIndex0");

                imageIndex1 = SelectedUserRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara.SurfImageIndex;
                OnPropertyChanged("imageIndex1");

                imageIndex2 = SelectedUserRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara.TailImageIndex;
                OnPropertyChanged("imageIndex2");

                imageIndex3 = SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.ShiftImageIndex;
                OnPropertyChanged("imageIndex3");
            }

            //1202 lw
            HOperatorSet.TupleSplit(ReferenceDirectory, "\\", out HTuple hv_subStr);
            HTuple FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 3];
            AroundBondRegionModelInspectParameter.CurFovName = FOV_Name;

            htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(AroundBondDetectionObject.DieImage, AroundBondRegionModelInspectParameter.ImageChannelIndex));
        }

        private void LoadImage()
        {
            HOperatorSet.GenEmptyObj(out HObject image);
            HOperatorSet.ReadImage(out image, AroundBondRegionModelInspectParameter.ImagePath);
            htWindow.Display(image, true);
            AroundBondDetectionObject.Image = image;

            HOperatorSet.CountChannels(AroundBondDetectionObject.Image, out HTuple channels);
            AroundBondRegionModelInspectParameter.ImageCountChannels = channels;

        }
        public void LoadDieImage()
        {
            try
            {
                AroundBondDetectionObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(AroundBondDetectionObject.DieImage, AroundBondRegionModelInspectParameter.ImageChannelIndex);
                //AroundBondDetectionObject.DieImageR = Algorithm.Region.GetChannnelImageUpdate(AroundBondDetectionObject.DieImage, 1);
                //AroundBondDetectionObject.DieImageG = Algorithm.Region.GetChannnelImageUpdate(AroundBondDetectionObject.DieImage, 2);
                //AroundBondDetectionObject.DieImageB = Algorithm.Region.GetChannnelImageUpdate(AroundBondDetectionObject.DieImage, 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //加载
        private void LoadModels()
        {
            try
            {
                Algorithm.Model_RegionAlg.JSCC_AOI_read_all_model(out  /*{eObjectVector,Dim=2}*/ FrameObjs,
                                                out  /*{eObjectVector,Dim=2}*/ IcObjs,
                                                out  /*{eObjectVector,Dim=2}*/ EpoxyObjs,
                                                out  /*{eObjectVector,Dim=2}*/ BondObjs,
                                                out  /*{eObjectVector,Dim=2}*/ WireObjs,
                                                ModelsFile,
                                                out  InspectItemNum,
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

                // 修改 by 王静
                Algorithm.Model_RegionAlg.JSCC_AOI_read_all_inspect_parametrer(RecipeFile,
                         out HTupleVector/*{eTupleVector,Dim=2}*/  FrameParameters,
                         out HTupleVector/*{eTupleVector,Dim=2}*/  IcParameters,
                         out HTupleVector/*{eTupleVector,Dim=2}*/  EpoxyParameters,
                         out HTupleVector/*{eTupleVector,Dim=3}*/  BondParameters,
                         out HTupleVector/*{eTupleVector,Dim=3}*/  WireParameters,
                         out HTupleVector/*{eTupleVector,Dim=3}*/  AroundBallInspectParas,
                         out HTuple paraErrCode,
                         out HTuple paraErrStr);

                if (paraErrCode < 0)
                {
                    MessageBox.Show("读Recipe文件异常！");
                    return;
                }
                //---------整合参数
                Con_FrameInspectParas = ((new HTupleVector(3).Insert(0, FrameModels)).Insert(
                    1, FrameParameters));
                Con_IcInspectParas = ((new HTupleVector(3).Insert(0, IcModels)).Insert(
                    1, IcParameters));
                Con_EpoxyInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, EpoxyModels)))).Insert(
                    1, EpoxyParameters));
                //modify by wj 2021-01-18
                Con_BondInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, BondModels)))).Insert(
                    1, BondParameters));
                // xiugai by wj 2020-09-04
                Con_WireInspectParas = (((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, WireModels)))).Insert(
                    1, WireParameters)));
                Con_AroundBallInspectParas = AroundBallInspectParas;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //--------------------------------------------------------------------------------

        //------------------------------加载区域------------------------------------------------
        private void ExecuteLoadResultRegionsCommand(object parameter)
        {
            try
            {
                LoadInspectBondRegions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void LoadInspectBondRegions()
        {
            HOperatorSet.GenEmptyObj(out DieRegions);
            HOperatorSet.ReadRegion(out DieRegions, $"{ReferenceDirectory}CoarseReference.reg");

            if (DieRegions == null || !DieRegions.IsInitialized())
            {
                MessageBox.Show("请先加载搜索区！");
                return;
            }

            if (AroundBondDetectionObject.Image == null && !AroundBondDetectionObject.Image.IsInitialized())
            {
                MessageBox.Show("请先加载验证图像！");
            }
            //加载模板
            LoadModels();

            AroundBondRegionModelInspectParameter.InspectItemNum = InspectItemNum;

            Algorithm.Model_RegionAlg.JSCC_AOI_Inspect_Gen_AroundBallReg_Unit(Algorithm.Region.GetChannnelImageConcact(AroundBondDetectionObject.Image),
                                            DieRegions.SelectObj(AroundBondRegionModelInspectParameter.UserRegionForCutOutIndex),
                                            FrameObjs,
                                            IcObjs,
                                            EpoxyObjs,
                                            BondObjs,
                                            WireObjs,
                                            out HObjectVector/*{eObjectVector,Dim=1}*/ RefBondRegs,
                                            out HObjectVector/*{eObjectVector,Dim=1}*/ RefPadRegs,
                                            out HObject RejectReg,
                                            InspectItemNum,//frame ic epoxy bond wire
                                            Con_FrameInspectParas,
                                            Con_IcInspectParas,
                                            Con_EpoxyInspectParas,
                                            Con_BondInspectParas,
                                            Con_WireInspectParas,
                                            CutRegModels,
                                            out HTuple hv_o_ErrCode, out HTuple hv_o_ErrStr);
            AroundBondRegUserRegions.Clear();

            //------------add by wj
            //读物i_RecipePath下所有的模板文件
            HOperatorSet.ListFiles(RecipeFile, "directories", out HTuple hv_Files);
            //
            //获取Recipe下的模板项目数组
            HOperatorSet.TupleRegexpMatch(hv_Files, "Recipe.*", out HTuple hv_Matches);
            HOperatorSet.TupleRegexpReplace(hv_Matches, new HTuple("Recipe/") + "*", "", out HTuple hv_ItemFiles);
            //
            HOperatorSet.TupleRegexpSelect(hv_ItemFiles, "Bond.*", out HTuple hv_BondName);
            //
            HOperatorSet.TupleRegexpReplace(hv_BondName, "\\\\", "", out HTuple hv_Result);
            HOperatorSet.TupleRegexpReplace(hv_Result, "\\\\", "", out hv_Result);
            //----------------------------------------------------------------------------------------------------

            //获取检测到的Bond区域
            for (int i = 0; i < InspectItemNum.TupleSelect(3); i++)
            {

                string regionPathName;
                string regionName = $"Bond{i + 1}FreeRegion";
                string parentPath = "AroundBallRegion";
                //regionPathName = $"{RecipeFile}\\{parentPath}\\{regionName}.reg";
                //mod by lht 2020-12-23
                regionPathName = $"Models\\{parentPath}\\{regionName}.reg";

                //regionPath = RecipeFile;

                HOperatorSet.MoveRegion(RefBondRegs.At(i).O, out HObject BondReg, -AroundBondRegionModelInspectParameter.DieImageRowOffset, -AroundBondRegionModelInspectParameter.DieImageColumnOffset);
                HOperatorSet.Union1(BondReg, out HObject _BondReg);
                HOperatorSet.TupleSelect(hv_Result, i, out HTuple bondRecipeName);

                UserRegion userRegion = new UserRegion
                {
                    DisplayRegion = _BondReg,
                    CalculateRegion = RefBondRegs.At(i).O,
                    //
                    RegionType = RegionType.Region,
                    RegionParameters = new double[1],
                    RecipeNames = bondRecipeName,
                    RegionPath = regionPathName,
                };
                if (userRegion == null) return;
                userRegion.Index = AroundBondRegUserRegions.Count + 1;
                userRegion.AroundBondRegionWithPara = new AroundBondRegionWithPara();
                AroundBondRegUserRegions.Add(userRegion);

                //HOperatorSet.MoveRegion(rejectRegion, out HObject _rejectRegion, -GoldenModelParameter.DieImageRowOffset, -GoldenModelParameter.DieImageColumnOffset);
                //UserRegion userRegion_Region = new UserRegion()
                //{
                //    DisplayRegion = _rejectRegion,
                //    CalculateRegion = rejectRegion,
                //    RegionType = RegionType.Region,
                //    RegionPath = regionPath,
                //};
                //if (userRegion_Region == null) return;
                //userRegion_Region.Index = RejectUserRegions.Count + 1;
                //RejectUserRegions.Add(userRegion_Region);
                //htWindow.DisplayMultiRegion(RejectRegions);
                //goldenModelObject.RejectRegion = rejectRegion;

                //if (userRegion == null) return;
                //userRegion.Index = AroundBondRegUserRegions.Count + 1;
                //userRegion.AroundBondRegionWithPara = new AroundBondRegionWithPara();
            }

            htWindow.DisplayMultiRegion(AroundBondRegUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
  
            //清除模板
            Algorithm.Model_RegionAlg.JSCC_AOI_clear_all_model(InspectItemNum, FrameModels, IcModels,
                                                         BondModels, out HTuple _clearErrcode, out HTuple _clearErrStr);
        }
        //显示应用
        private void ExecuteUserRegionEnableChangedCommand(object parameter)
        {
            if (isRightClick)
            {
                UserRegion userRegion = parameter as UserRegion;
                if (userRegion == null) return;
                htWindow.DisplayMultiRegion(AroundBondRegUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion), Algorithm.Region.GetChannnelImageUpdate(AroundBondDetectionObject.DieImage, AroundBondRegionModelInspectParameter.ImageChannelIndex));
            }
        }

        //------------加载检测验证图像集
        private void ExecuteImagesSetVerifyCommand(object parameter)
        {
            try
            {
                using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        AroundBondRegionModelInspectParameter.VerifyImagesDirectory = fbd.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                    // 1202
                    if (MessageBox.Show("是否为指定Fov的task图集类型？", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        isFovTaskFlag = 1;

                        // 指定Fov合成多通道图并显示第一张图
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                AroundBondRegionModelInspectParameter.VerifyImagesDirectory,
                                                                                AroundBondRegionModelInspectParameter.CurFovName,
                                                                                0, out HTuple hv_o_ImageVerifyNum, out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        AroundBondRegionModelInspectParameter.CurrentVerifySet = hv_o_ImageVerifyNum;
                        PImageIndexPath = imageFiles[AroundBondRegionModelInspectParameter.ImageChannelIndex];
                        ImageVerify = ho_MutiImage;
                    }
                    else
                    {
                        isFovTaskFlag = 0;

                        Algorithm.File.list_image_files(AroundBondRegionModelInspectParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        string[] folderList = imageFiles;
                        AroundBondRegionModelInspectParameter.CurrentVerifySet = folderList.Count();
                        PImageIndexPath = imageFiles[0];
                        HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                        ImageVerify = image;
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, AroundBondRegionModelInspectParameter.ImageChannelIndex);
                    //显示原图
                    htWindow.Display(ChannelImageVerify, true);
                    pImageIndex = 0;
                    imgIndex = 0;
                    HOperatorSet.GenEmptyObj(out DieRegions);
                    HOperatorSet.ReadRegion(out DieRegions, $"{ReferenceDirectory}CoarseReference.reg");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //---------------加载默认图集
        private void ExecuteRefreshImagesSetCommand(object parameter)
        {
            if (Directory.Exists(AroundBondRegionModelInspectParameter.VerifyImagesDirectory))
            {
                Algorithm.File.list_image_files(AroundBondRegionModelInspectParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                string[] folderList = imageFiles;
                AroundBondRegionModelInspectParameter.CurrentVerifySet = folderList.Count();
                PImageIndexPath = imageFiles[0];
                HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                ImageVerify = image;
                ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, AroundBondRegionModelInspectParameter.ImageChannelIndex);
                htWindow.Display(ChannelImageVerify, true);
                pImageIndex = 0;
                imgIndex = 0;
                HOperatorSet.GenEmptyObj(out DieRegions);
                HOperatorSet.ReadRegion(out DieRegions, $"{ReferenceDirectory}CoarseReference.reg");
            }
        }

        //---------------区域内检测验证
        HObjectVector hvec__FailRegs = new HObjectVector(2);
        HObject ho_o_FailRegs = null;
        HTupleVector hvec_BondDefectValue = new HTupleVector(4);

        public HObject FailRegResult = null;
        public HObject BondRegResult = null;

        private void ExecuteVerifyCommand(object obj)

        {
            HOperatorSet.GenEmptyObj(out DieRegions);
            HOperatorSet.ReadRegion(out DieRegions, $"{ReferenceDirectory}CoarseReference.reg");

            if (DieRegions == null || !DieRegions.IsInitialized())
            {
                MessageBox.Show("请先加载搜索区！");
                return;
            }
            if (CreateAroundBondRegionModel.isRightClick != true) return;

            if (ImageVerify == null || !ImageVerify.IsInitialized())
            {
                MessageBox.Show("请先加载图集！");
                return;
            }

            if (SelectedUserRegion == null)
            {
                MessageBox.Show("请先选择焊点区域！");
                return;
            }

            try
            {
                //add by wj 2020-12-11
                if (SelectedUserRegion.IsAroundBondRegInspect == 1)
                {
                    //该Ball周围需要检测
                    if (SelectedUserRegion.AroundBondRegionWithPara.IsBallShiftInspect)
                    {
                        //进行焊点偏移检测
                        switch (SelectedUserRegion.AroundBondRegionWithPara.ShiftInspectMethodIndex)
                        {
                            case 0:
                                //选择Match_Measure方法
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.ShiftImageIndex;
                                break;
                            default:
                                break;
                        }
                    }
                    //该Ball进行尾丝检测
                    if (SelectedUserRegion.AroundBondRegionWithPara.IsTailInspect)
                    {
                        //进行焊点偏移检测
                        switch (SelectedUserRegion.AroundBondRegionWithPara.TailInspectMethodIndex)
                        {
                            case 0:
                                //选择Line_Gauss方法
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara.TailImageIndex;
                                break;
                            default:
                                break;
                        }
                    }
                    //该Ball进行焊盘表面检测
                    if (SelectedUserRegion.AroundBondRegionWithPara.IsSurfaceInspect)
                    {
                        //进行焊点偏移检测
                        switch (SelectedUserRegion.AroundBondRegionWithPara.SurfaceInspectMethodIndex)
                        {
                            case 0:
                                //选择Adaptive方法
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara.SurfImageIndex;
                                break;
                            case 1:
                                //选择Global方法
                                AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara.SurfImageIndex;
                                break;
                            default:
                                break;
                        }
                    }
                }

                // 1201
                //switch (SelectedUserRegion.RegAlgParameterIndex)
                //{
                //    case 0:
                //        AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara.ImageChannelIndex;
                //        break;
                //    case 1:
                //        AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara.ImageChannelIndex;
                //        break;
                //    case 2:
                //        AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara.ImageChannelIndex;
                //        break;
                //    case 3:
                //        AroundBondRegionModelInspectParameter.ImageChannelIndex = SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.ImageChannelIndex;
                //        break;
                //    default:
                //        break;
                //}


                htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, AroundBondRegionModelInspectParameter.ImageChannelIndex), true);
                //加载模板区域与参数
                LoadModels();

                Algorithm.Model_RegionAlg.JSCC_AOI_Inspect_Gen_AroundBallReg_Unit(Algorithm.Region.GetChannnelImageConcact(ImageVerify),
                                                DieRegions.SelectObj(AroundBondRegionModelInspectParameter.UserRegionForCutOutIndex),
                                                FrameObjs,
                                                IcObjs,
                                                EpoxyObjs,
                                                BondObjs,
                                                WireObjs,
                                                out HObjectVector/*{eObjectVector,Dim=1}*/ RefBondRegs,
                                                out HObjectVector/*{eObjectVector,Dim=1}*/ RefPadRegs,
                                                out HObject RejectReg,
                                                InspectItemNum,//frame ic epoxy bond wire
                                                Con_FrameInspectParas,
                                                Con_IcInspectParas,
                                                Con_EpoxyInspectParas,
                                                Con_BondInspectParas,
                                                Con_WireInspectParas,
                                                 /*{eTupleVector,Dim=1}*/ CutRegModels,
                                                out HTuple hv_o_ErrCode, out HTuple hv_o_ErrStr);

                //-----AroundBall周围检测
                //AroundBond检测个数
                HTuple aroundBallNum = InspectItemNum[3];
                Algorithm.Model_RegionAlg.HTV_aroundBalls_inspect(Algorithm.Region.GetChannnelImageConcact(ImageVerify),
                                          RefBondRegs, RefPadRegs, RejectReg,
                                          out hvec__FailRegs,
                                          out HObjectVector/*{eTupleVector,Dim=1}*/ hvec_o_BondContours,
                                          out HObjectVector/*{eTupleVector,Dim=1}*/ hvec_o_PadContours,
                                          aroundBallNum,
                                          Con_AroundBallInspectParas,
                                          hvec_BondDefectValue,
                                          out HTupleVector/*{eTupleVector,Dim=2}*/ hvec__DefectTypes,
                                          out HTupleVector/*{eTupleVector,Dim=4}*/ hvec_o_DefectValue, 
                                          out HTupleVector/*{eTupleVector,Dim=2}*/ hvec__aroundBallDefectImgIdx,
                                          out HTupleVector/*{eTupleVector,Dim=4}*/ hvec_o_RefValue,
                                          out HTuple hv__aroundBallErrCode,
                                          out HTuple hv__aroundBallErrStr);

                //--------整合AroundBall检测结果
                //     
                Algorithm.Model_RegionAlg.HTV_Vector_to_ObjectConcat_1d(hvec_o_BondContours, out HObject _BondContours, out HTuple VecErrCode, out HTuple VecErrStr);
                Algorithm.Model_RegionAlg.HTV_Vector_to_ObjectConcat_1d(hvec_o_PadContours, out HObject _PadContours, out HTuple VecErrCode0, out HTuple VecErrStr0);
                Algorithm.Model_RegionAlg.HTV_Vector_to_ObjectConcat_2d(hvec__FailRegs, out ho_o_FailRegs, out HTuple VerErrCode, out HTuple VerErrStr);

                //-------检测结果展示
                if (hv__aroundBallErrCode > 0)
                {
                    // 分颜色显示 0118 lw
                    HOperatorSet.GenEmptyObj(out FailRegResult);
                    HOperatorSet.GenEmptyObj(out BondRegResult);
                    HOperatorSet.ConcatObj(FailRegResult, ho_o_FailRegs, out FailRegResult);
                    HOperatorSet.ConcatObj(BondRegResult, DieRegions.SelectObj(imgIndex + 1).ConcatObj(_BondContours), out BondRegResult);

                    DisplayAroundBondResultRegion();
                    //htWindow.DisplaySingleRegion(ho_o_FailRegs.ConcatObj(DieRegions.SelectObj(imgIndex + 1)), ChannelImageVerify, "orange");
                }
                else
                {
                    FailRegResult = null;
                    BondRegResult = null;
                    htWindow.DisplaySingleRegion(DieRegions.SelectObj(imgIndex + 1).ConcatObj(_BondContours), ChannelImageVerify);
                }
                hvec__FailRegs.Dispose();
                ho_o_FailRegs = null;
                
                //清除模板
                Algorithm.Model_RegionAlg.JSCC_AOI_clear_all_model(InspectItemNum, FrameModels, IcModels,
                                                                   BondModels, out HTuple _clearErrcode, out HTuple _clearErrStr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "检测验证出错");
                imgIndex++;
                if ((imgIndex + 1) > DieRegions.CountObj())
                {
                    imgIndex = 0;
                }
            }
        }

        public void DisplayAroundBondResultRegion()
        {
            if ((FailRegResult == null) || (BondRegResult == null))
            {
                return;
            }

            htWindow.InitialHWindow("orange");
            htWindow.hTWindow.HalconWindow.DispObj(BondRegResult);

            htWindow.InitialHWindow("red");
            htWindow.hTWindow.HalconWindow.DispObj(FailRegResult);
        }


        //--------------检测验证前一张图
        private void ExecutePreviousCommand(object parameter)
        {
            try
            {
                imgIndex = 0;
                if (AroundBondRegionModelInspectParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == 0 ? AroundBondRegionModelInspectParameter.CurrentVerifySet - 1 : --pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                AroundBondRegionModelInspectParameter.VerifyImagesDirectory,
                                                                                AroundBondRegionModelInspectParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[AroundBondRegionModelInspectParameter.ImageChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(AroundBondRegionModelInspectParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, AroundBondRegionModelInspectParameter.ImageChannelIndex);                   
                    htWindow.Display(ChannelImageVerify, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        //--------------验证验证后一张图
        private void ExecuteNextCommand(object parameter)
        {
            try
            {
                imgIndex = 0;
                if (AroundBondRegionModelInspectParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == AroundBondRegionModelInspectParameter.CurrentVerifySet - 1 ? 0 : ++pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                AroundBondRegionModelInspectParameter.VerifyImagesDirectory,
                                                                                AroundBondRegionModelInspectParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[AroundBondRegionModelInspectParameter.ImageChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(AroundBondRegionModelInspectParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, AroundBondRegionModelInspectParameter.ImageChannelIndex);
                    htWindow.Display(ChannelImageVerify, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //----保存区域检测参数
        private void ExecuteSaveCommand(object parameter)
        {
            try
            {
                if (AroundBondRegionModelInspectParameter.ImageCountChannels > 0 && AroundBondRegionModelInspectParameter.ImageChannelIndex < 0)
                {
                    MessageBox.Show("请选择图像通道！");
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
            return true;
        }

        public void Initial()
        {
            htWindow.ClearSelection();
            if (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff) return;
            htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(AroundBondDetectionObject.DieImage, AroundBondRegionModelInspectParameter.ImageChannelIndex), true);
        }

        public void Dispose()
        {
           (Content as Page_AroundBondRegionModel).DataContext = null;
           (Content as Page_AroundBondRegionModel).Close();
            Content = null;
            htWindow = null;
            ImageVerify = null;
            ChannelImageVerify = null;
            AroundBondDetectionObject = null;
            AroundBondRegionModelInspectParameter = null;
            AroundBondDetectionObject = null;
            VerifyCommand = null;
            SaveCommand = null;
            PreviousCommand = null;
            NextCommand = null;
            AroundBondRegUserRegions = null;
            LoadReferenceCommand = null;
            LoadResultRegionsCommand = null;
            UserRegionEnableChangedCommand = null;
            ImagesSetVerifyCommand = null;
            ClickResponseCommand = null;
            hvec__FailRegs = null;
            ho_o_FailRegs = null;
        }

    }

}
