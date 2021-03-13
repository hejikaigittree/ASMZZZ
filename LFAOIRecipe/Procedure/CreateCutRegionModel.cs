using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace LFAOIRecipe
{
    class CreateCutRegionModel : ViewModelBase, IProcedure
    {
        //1123
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public static bool isRightClick = true;

        public event Action OnSaveXML;

        public string ReferenceDirectory { get; set; }

        private string ModelsRecipeDirectory;

        private readonly string ModelsFile;

        private readonly string RecipeFile;

        private EpoxyModelObject EpoxyModelObject;

        private HObject DieRegions = null;

        public ObservableCollection<UserRegion> CutRegionUserRegions { get; private set; }

        public ObservableCollection<UserRegion> OriRegionUserRegions { get; private set; }

        private IEnumerable<HObject> CutRegionRegions => CutRegionUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion);

        private HTHalControlWPF htWindow;

        private int imageIndex;
        public int ImageIndex
        {
            get => imageIndex;
            set
            {
                // 1123
                if (imageIndex != value)
                {
                    if (value == -1)
                    {
                        HOperatorSet.AccessChannel(EpoxyModelObject.DieImage, out HObject ChannelImage, CutRegionParameter.ImageChannelIndex + 1);
                        htWindow.Display(ChannelImage, false);
                    }
                    else if (value >= 0)
                    {
                        HOperatorSet.AccessChannel(EpoxyModelObject.DieImage, out HObject ChannelImage, value + 1);
                        CutRegionParameter.ImageChannelIndex = value;
                        htWindow.Display(ChannelImage, false);
                    }
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private int stepTime = 200;
        public int StepTime
        {
            get => stepTime;
            set => OnPropertyChanged(ref stepTime, value);
        }

        private bool? isCheckAll;
        public bool? IsCheckAll
        {
            get => isCheckAll;
            set => OnPropertyChanged(ref isCheckAll, value);
        }

        private bool isSetAllOper = true;
        public bool IsSetAllOper
        {
            get => isSetAllOper;
            set => OnPropertyChanged(ref isSetAllOper, value);
        }

        private string freeRegionName = "FreeRegion";
        public string FreeRegionName
        {
            get => freeRegionName;
            set => OnPropertyChanged(ref freeRegionName, value);
        }

        private bool isMargin = true;
        public bool IsMargin
        {
            get => isMargin;
            set => OnPropertyChanged(ref isMargin, value);
        }

        private double erosionRadius = 5;
        public double ErosionRadius
        {
            get => erosionRadius;
            set => OnPropertyChanged(ref erosionRadius, value);
        }

        private double dilationRadius = 1;//修改
        public double DilationRadius
        {
            get => dilationRadius;
            //set => OnPropertyChanged(ref dilationRadius, value);
            set
            {
                if (dilationRadius != value)
                {
                    if (value<=0)
                    {
                        MessageBox.Show("请设置膨胀因子！");
                        return;
                    }
                    dilationRadius = value;
                    OnPropertyChanged();
                }
            }
        }

        private UserRegion userRegionForCutOut;
        public UserRegion UserRegionForCutOut
        {
            get => userRegionForCutOut;
            set => OnPropertyChanged(ref userRegionForCutOut, value);
        }

        public CutRegionParameter CutRegionParameter { get; set; }

        public CommandBase LoadReferenceCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase LoadResultRegionsCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ErosionCommand { get; private set; }
        public CommandBase DilationCommand { get; private set; }
        public CommandBase SaveCommand { get; private set; }
        public CommandBase IsCheckCommand { get; private set; }
        public CommandBase IsCheckAllCommand { get; private set; }
        public CommandBase TextChangedCommand { get; private set; }

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

        //private HObjectVector IcsRegs = new HObjectVector(1);
        //private HObjectVector EpoxysRegs = new HObjectVector(1);
        //private HObjectVector BondsRegs = new HObjectVector(1);
        //private HObjectVector WiresRegs = new HObjectVector(1);

        private HTupleVector Con_FrameInspectParas = new HTupleVector(3), Con_IcInspectParas = new HTupleVector(3);
        private HTupleVector Con_EpoxyInspectParas = new HTupleVector(4), Con_BondInspectParas = new HTupleVector(3);
        private HTupleVector Con_WireInspectParas = new HTupleVector(4);

        public CreateCutRegionModel(HTHalControlWPF htWindow,
                              string referenceDirectory,
                              string modelsFile,
                              string recipeFile,
                              EpoxyModelObject epoxyModelObject,
                              CutRegionParameter cutRegionParameter,
                              ObservableCollection<UserRegion> CutRegionUserRegions,
                              ObservableCollection<UserRegion> OriRegionUserRegions,
                              string modelsRecipeDirectory)
        {
            DisplayName = "制作CutRegion模板";
            Content = new Page_CutRegionModel { DataContext = this };

            this.htWindow = htWindow;
            this.ReferenceDirectory = referenceDirectory;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.EpoxyModelObject = epoxyModelObject;
            this.CutRegionParameter = cutRegionParameter;
            this.CutRegionUserRegions = CutRegionUserRegions;
            this.OriRegionUserRegions = OriRegionUserRegions;
            this.ModelsRecipeDirectory = modelsRecipeDirectory;

            LoadResultRegionsCommand = new CommandBase(ExecuteLoadResultRegionsCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            ErosionCommand = new CommandBase(ExecuteErosionCommand);
            DilationCommand = new CommandBase(ExecuteDilationCommand);
            LoadReferenceCommand = new CommandBase(ExecuteLoadReferenceCommand);
            SaveCommand = new CommandBase(ExecuteSaveCommand);
            IsCheckCommand = new CommandBase(ExecuteIsCheckCommand);
            IsCheckAllCommand = new CommandBase(ExecuteIsCheckAllCommand);
            TextChangedCommand = new CommandBase(ExecuteTextChangedCommand);

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

                HTuple files = new HTuple();
                files = Directory.GetDirectories(Directory.GetParent(ModelsFile).ToString(),"Frame*.*", SearchOption.TopDirectoryOnly);
                string[] OnRecipesIndexs = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    OnRecipesIndexs[i] = Path.GetFileName(files[i]);
                }
                CutRegionParameter.OnRecipesIndexs = OnRecipesIndexs;
                htWindow.DisplayMultiRegion(CutRegionRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, CutRegionParameter.ImageChannelIndex));
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

            CutRegionParameter.ImagePath = $"{ReferenceDirectory}ReferenceImage.tiff";
            EpoxyModelObject.Image?.Dispose();
            LoadImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageRowOffset.tup", out HTuple dieImageRowOffset);
            CutRegionParameter.DieImageRowOffset = dieImageRowOffset;
            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageColumnOffset.tup", out HTuple dieImageColumnOffset);
            CutRegionParameter.DieImageColumnOffset = dieImageColumnOffset;

            HOperatorSet.ReadRegion(out HObject UserRegionForCutOut_Region, ReferenceDirectory + "DieReference.reg");
            HOperatorSet.ReduceDomain(EpoxyModelObject.Image, UserRegionForCutOut_Region, out HObject imageReduced);
            HOperatorSet.CropDomain(imageReduced, out HObject dieImage);
            EpoxyModelObject.DieImage = dieImage;
            LoadDieImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "UserRegionForCutOutIndex.tup", out HTuple UserRegionForCutOutIndexTemp);
            CutRegionParameter.UserRegionForCutOutIndex = UserRegionForCutOutIndexTemp;
            //UserRegionForCutOut = DieUserRegions.Where(u => u.Index == CutRegionParameter.UserRegionForCutOutIndex).FirstOrDefault();
            //EpoxyModelObject.UserRegionForCutOut = UserRegionForCutOut;

            //1121 lht
            HOperatorSet.ReadTuple(ReferenceDirectory + "ImageChannelnumber.tup", out HTuple ImageChannelnumber);
            ChannelNames.Clear();
            for (int i = 0; i < ImageChannelnumber; i++)
            {
                ChannelName tmp_name = new ChannelName();
                tmp_name.Name = (i + 1).ToString();
                ChannelNames.Add(tmp_name);
            }

            imageIndex = CutRegionParameter.ImageChannelIndex;
            OnPropertyChanged("imageIndex");

            HOperatorSet.AccessChannel(EpoxyModelObject.DieImage, out HObject ChannelImageDisplay, CutRegionParameter.ImageChannelIndex + 1);
            htWindow.Display(ChannelImageDisplay);
        }

        public void LoadImage()
        {
            HOperatorSet.GenEmptyObj(out HObject image);
            HOperatorSet.ReadImage(out image, CutRegionParameter.ImagePath);
            htWindow.Display(image, true);
            EpoxyModelObject.Image = image;

            HOperatorSet.CountChannels(EpoxyModelObject.Image, out HTuple channels);
            CutRegionParameter.ImageCountChannels = channels;
            EpoxyModelObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, CutRegionParameter.ImageChannelIndex);
            //EpoxyModelObject.ImageR = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, 1);
            //EpoxyModelObject.ImageG = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, 2);
            //EpoxyModelObject.ImageB = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.Image, 3);
        }

        public void LoadDieImage()
        {
            try
            {
                EpoxyModelObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, CutRegionParameter.ImageChannelIndex);
                //EpoxyModelObject.DieImageR = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, 1);
                //EpoxyModelObject.DieImageG = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, 2);
                //EpoxyModelObject.DieImageB = Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //加载区域
        private void ExecuteLoadResultRegionsCommand(object parameter)
        {
            try
            {
                LoadInspectCutRegions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void LoadInspectCutRegions()
        {
            HOperatorSet.GenEmptyObj(out DieRegions);
            HOperatorSet.ReadRegion(out DieRegions, $"{ReferenceDirectory}CoarseReference.reg");

            if (DieRegions == null || !DieRegions.IsInitialized())
            {
                MessageBox.Show("请先加载搜索区！");
                return;
            }

            if (EpoxyModelObject.Image == null && !EpoxyModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请先加载验证图像！");
            }


            Algorithm.Model_RegionAlg.JSCC_AOI_read_all_model(out  /*{eObjectVector,Dim=2}*/ FrameObjs,
                            out  /*{eObjectVector,Dim=2}*/ IcObjs,
                            out  /*{eObjectVector,Dim=2}*/ EpoxyObjs,
                            out  /*{eObjectVector,Dim=2}*/ BondObjs,
                            out  /*{eObjectVector,Dim=2}*/ WireObjs,
                            ModelsFile,
                            out HTuple InspectItemNum,
                            out  /*{eTupleVector,Dim=2}*/ FrameModels,
                            out  /*{eTupleVector,Dim=2}*/ IcModels,
                            out  /*{eTupleVector,Dim=2}*/ EpoxyModels,
                            out  /*{eTupleVector,Dim=2}*/ BondModels,
                            out  /*{eTupleVector,Dim=2}*/ WireModels,
                            out CutRegModels,
                            out HTuple modelErrCode, out HTuple modelErrStr);
            if (modelErrCode < 0)
            {
                MessageBox.Show("读Models文件异常或文件不存在！");
                return;

            }

            CutRegionParameter.InspectItemNum = InspectItemNum;

            ///*  Algorithm.Model_RegionAlg.JSCC_AOI_read_all_inspect_parametrer(RecipeFile,
            //              out HTupleVector/*{eTupleVector,Dim=2}*/  FrameParameters,
            //              out HTupleVector/*{eTupleVector,Dim=2}*/  IcParameters,
            //              out HTupleVector/*{eTupleVector,Dim=2}*/  EpoxyParameters,
            //              out HTupleVector/*{eTupleVector,Dim=3}*/  WireParameters,
            //              out HTuple paraErrCode,
            //              out HTuple paraErrStr);
            //  **/

            // 修改 by 王静 2020-10-22
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
                MessageBox.Show("读Recipe文件异常或文件不存在！");
                return;

            }

            Con_FrameInspectParas = ((new HTupleVector(3).Insert(0, FrameModels)).Insert(
                1, FrameParameters));
            Con_IcInspectParas = ((new HTupleVector(3).Insert(0, IcModels)).Insert(
                1, IcParameters));
            Con_EpoxyInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, EpoxyModels)))).Insert(
                1, EpoxyParameters));
            //modify by wj 2021-01-18
            Con_BondInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, BondModels)))).Insert(
                    1, BondParameters));
            Con_WireInspectParas = ((new HTupleVector(4).Insert(0, (new HTupleVector(3).Insert(0, WireModels)))).Insert(
                1, WireParameters));

            Algorithm.Model_RegionAlg.JSCC_AOI_Inspect_Gen_InspectReg_Unit(Algorithm.Region.GetChannnelImageConcact(EpoxyModelObject.Image),
                                            DieRegions.SelectObj(CutRegionParameter.UserRegionForCutOutIndex),
                                            FrameObjs,
                                            IcObjs,
                                            EpoxyObjs,
                                            BondObjs,
                                            WireObjs,
                                            out HObjectVector/*{eObjectVector,Dim=1}*/  IcsRegs,
                                            out HObjectVector /*{eObjectVector,Dim=1}*/  EpoxysRegs,
                                            out HObjectVector/*{eObjectVector,Dim=1}*/   BondRegs,
                                            out HObjectVector /*{eObjectVector,Dim=1}*/   WiresRegs,
                                            out HObject FailRegs,
                                            InspectItemNum,//frame ic epoxy bond wire
                                            Con_FrameInspectParas,
                                            Con_IcInspectParas,
                                            Con_EpoxyInspectParas,
                                            Con_BondInspectParas,
                                            Con_WireInspectParas,
                                             /*{eTupleVector,Dim=1}*/ CutRegModels,
                                            out HTuple hv_o_DefectType,
                                            out HTuple hv_o_ErrCode, out HTuple hv_o_ErrStr);

            // 保存初始cutReg, 即膨胀前参数 0125 lw
            OriRegionUserRegions.Clear();
            CutRegionUserRegions.Clear();

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
            //
            for (int i = 0; i < InspectItemNum.TupleSelect(1); i++)
            {
                //add by wj 2020-12-24
                string regionPathName;
                string regionName = $"Ic{i + 1}FreeRegion";
                string parentPath = "CutRegion";
                regionPathName = $"Models\\{parentPath}\\{regionName}.reg";
                //
                HOperatorSet.MoveRegion(IcsRegs.At(i).O, out HObject IcsReg, -CutRegionParameter.DieImageRowOffset, -CutRegionParameter.DieImageColumnOffset);
                HOperatorSet.Union1(IcsReg, out HObject _IcsReg);
                //
                UserRegion userRegion = new UserRegion
                {
                    DisplayRegion = _IcsReg,
                    CalculateRegion = IcsReg,
                    //
                    RegionType = RegionType.Region,
                    RegionParameters = new double[1],
                    RecipeNames = $"IC{i + 1}",
                    RegionPath = regionPathName,
                };
                if (userRegion == null) return;
                userRegion.Index = CutRegionUserRegions.Count + 1;
                CutRegionUserRegions.Add(userRegion);

                UserRegion userRegionOri = new UserRegion
                {
                    DisplayRegion = _IcsReg,
                    CalculateRegion = IcsReg,
                    //
                    RegionType = RegionType.Region,
                    RegionParameters = new double[1],
                    RecipeNames = $"IC{i + 1}",
                    RegionPath = regionPathName,
                };
                if (userRegionOri == null) return;
                userRegionOri.Index = OriRegionUserRegions.Count + 1;
                OriRegionUserRegions.Add(userRegionOri);
            }

            for (int i = 0; i < InspectItemNum.TupleSelect(2); i++)
            {
                //add by wj 2020-12-24
                string regionPathName;
                string regionName = $"Epoxy{i + 1}FreeRegion";
                string parentPath = "CutRegion";
                regionPathName = $"Models\\{parentPath}\\{regionName}.reg";
                //
                HOperatorSet.MoveRegion(EpoxysRegs.At(i).O, out HObject EpoxysReg, -CutRegionParameter.DieImageRowOffset, -CutRegionParameter.DieImageColumnOffset);
                HOperatorSet.Union1(EpoxysReg, out HObject _EpoxysReg);
                //
                UserRegion userRegion = new UserRegion
                {
                    DisplayRegion   = _EpoxysReg,
                    CalculateRegion = EpoxysReg,
                    //
                    RegionType = RegionType.Region,
                    RegionParameters = new double[1],
                    RecipeNames = $"Epoxys{i + 1}",
                    RegionPath = regionPathName,
                };
                if (userRegion == null) return;
                userRegion.Index = CutRegionUserRegions.Count + 1;
                CutRegionUserRegions.Add(userRegion);

                UserRegion userRegionOri = new UserRegion
                {
                    DisplayRegion = _EpoxysReg,
                    CalculateRegion = EpoxysReg,
                    //
                    RegionType = RegionType.Region,
                    RegionParameters = new double[1],
                    RecipeNames = $"Epoxys{i + 1}",
                    RegionPath = regionPathName,
                };
                if (userRegionOri == null) return;
                userRegionOri.Index = OriRegionUserRegions.Count + 1;
                OriRegionUserRegions.Add(userRegionOri);
            }

            for (int i = 0; i < InspectItemNum.TupleSelect(3); i++)
            {
                //add by wj 2020-12-24
                string regionPathName;
                string regionName = $"Bond{i + 1}FreeRegion";
                string parentPath = "CutRegion";
                regionPathName = $"Models\\{parentPath}\\{regionName}.reg";

                HOperatorSet.MoveRegion(BondRegs.At(i).O, out HObject BondReg, -CutRegionParameter.DieImageRowOffset, -CutRegionParameter.DieImageColumnOffset);
                HOperatorSet.Union1(BondReg, out HObject _BondReg);
                //
                HOperatorSet.TupleSelect(hv_Result, i, out HTuple bondRecipeName);
                UserRegion userRegion = new UserRegion
                {
                    DisplayRegion   = _BondReg,
                    CalculateRegion = BondReg,
                    //
                    RegionType = RegionType.Region,
                    RegionParameters = new double[1],
                    RecipeNames = bondRecipeName,
                    RegionPath = regionPathName,
                };
                if (userRegion == null) return;
                userRegion.Index = CutRegionUserRegions.Count + 1;
                CutRegionUserRegions.Add(userRegion);

                UserRegion userRegionOri = new UserRegion
                {
                    DisplayRegion = _BondReg,
                    CalculateRegion = BondReg,
                    //
                    RegionType = RegionType.Region,
                    RegionParameters = new double[1],
                    RecipeNames = bondRecipeName,
                    RegionPath = regionPathName,
                };
                if (userRegionOri == null) return;
                userRegionOri.Index = OriRegionUserRegions.Count + 1;
                OriRegionUserRegions.Add(userRegionOri);
            }

            for (int i = 0; i < InspectItemNum.TupleSelect(4); i++)
            {
                //add by wj 2020-12-24
                string regionPathName;
                string regionName = $"Wire{i + 1}FreeRegion";
                string parentPath = "CutRegion";
                regionPathName = $"Models\\{parentPath}\\{regionName}.reg";

                HOperatorSet.MoveRegion(WiresRegs.At(i).O, out HObject WiresReg, -CutRegionParameter.DieImageRowOffset, -CutRegionParameter.DieImageColumnOffset);
                HOperatorSet.Union1(WiresReg, out HObject _WiresReg);

                UserRegion userRegion = new UserRegion
                {
                    DisplayRegion   = _WiresReg,
                    CalculateRegion = WiresReg,
                    //
                    RegionType = RegionType.Region,
                    RegionParameters = new double[1],
                    RecipeNames = $"Wire{i + 1}",
                    RegionPath = regionPathName,
                };
                if (userRegion == null) return;
                userRegion.Index = CutRegionUserRegions.Count + 1;
                CutRegionUserRegions.Add(userRegion);

                UserRegion userRegionOri = new UserRegion
                {
                    DisplayRegion = _WiresReg,
                    CalculateRegion = WiresReg,
                    //
                    RegionType = RegionType.Region,
                    RegionParameters = new double[1],
                    RecipeNames = $"Wire{i + 1}",
                    RegionPath = regionPathName,
                };
                if (userRegionOri == null) return;
                userRegionOri.Index = OriRegionUserRegions.Count + 1;
                OriRegionUserRegions.Add(userRegionOri);
            }

            htWindow.DisplayMultiRegion(CutRegionUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));

            //清除模板 1130
            Algorithm.Model_RegionAlg.JSCC_AOI_clear_all_model(InspectItemNum, FrameModels, IcModels,
                                                               BondModels, out HTuple _clearErrcode, out HTuple _clearErrStr);
        }

        private void ExecuteUserRegionEnableChangedCommand(object parameter)
        {
            if (isRightClick)
            {
                UserRegion userRegion = parameter as UserRegion;
                if (userRegion == null) return;
                htWindow.DisplayMultiRegion(CutRegionRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, CutRegionParameter.ImageChannelIndex));
            }
        }


        //膨胀
        public void LoadDilationParameters()
        {
            try
            {
                for (int i = 0; i < OriRegionUserRegions.Count; i++)
                {                    
                    HOperatorSet.GenEmptyObj(out HObject resultRegion);   
                    HOperatorSet.DilationCircle(OriRegionUserRegions[i].DisplayRegion, out resultRegion, CutRegionParameter.CutRegionParameters[i]);

                    UserRegion userRegionOri = new UserRegion
                    {
                        DisplayRegion = OriRegionUserRegions[i].DisplayRegion,
                        CalculateRegion = resultRegion,
                        //
                        RegionType = RegionType.Region,
                        RegionOperatType = RegionOperatType.Dilation,
                        RegionParameters = new double[1] { CutRegionParameter.CutRegionParameters[i] },
                        RecipeNames = OriRegionUserRegions[i].RecipeNames,
                        RegionPath = OriRegionUserRegions[i].RegionPath,
                    };

                    if (userRegionOri == null) return;
                    userRegionOri.Index = CutRegionUserRegions.Count + 1;
                    CutRegionUserRegions.Add(userRegionOri);
                    htWindow.DisplayMultiRegion(CutRegionRegions);                        
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }           
        }

        //膨胀
        private void ExecuteDilationCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    for (int i = 0; i < CutRegionUserRegions.Count; i++)
                    {
                        HOperatorSet.GenEmptyObj(out HObject resultRegion);
                        if (CutRegionUserRegions[i].IsSelected)
                        {
                            if (isSetAllOper == true)
                            {
                                HOperatorSet.DilationCircle(CutRegionUserRegions[i].DisplayRegion, out resultRegion, dilationRadius);
                                CutRegionUserRegions[i].RegionParameters = new double[1] { dilationRadius };
                            }
                            else
                            {
                                if (CutRegionUserRegions[i].RegionParameters[0] <= 0)
                                {
                                    MessageBox.Show("请设置膨胀因子！");
                                    return;
                                }
                                HOperatorSet.DilationCircle(CutRegionUserRegions[i].DisplayRegion, out resultRegion, CutRegionUserRegions[i].RegionParameters);
                            }
                            CutRegionUserRegions[i].CalculateRegion = resultRegion;
                            CutRegionUserRegions[i].RegionOperatType = RegionOperatType.Dilation;
                        }
                        htWindow.DisplayMultiRegion(CutRegionRegions);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        //腐蚀
        private void ExecuteErosionCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    for (int i = 0; i < CutRegionUserRegions.Count; i++)
                    {
                        if (CutRegionUserRegions[i].IsSelected)
                        {
                            HOperatorSet.ErosionCircle(CutRegionUserRegions[i].DisplayRegion, out HObject _ResultRegion, erosionRadius);
                            CutRegionUserRegions[i].CalculateRegion = _ResultRegion;
                            CutRegionUserRegions[i].RegionOperatType = RegionOperatType.Erosion;
                            CutRegionUserRegions[i].RegionParameters = new double[1] { erosionRadius };
                            htWindow.DisplayMultiRegion(CutRegionRegions);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void ExecuteTextChangedCommand(object parameter)
        {
            if (isRightClick)
            {
                UserRegion userRegion = parameter as UserRegion;
                if (userRegion == null) return;
                if (userRegion.RegionParameters[0] <= 0)
                {
                    MessageBox.Show("请设置膨胀因子！");
                    return;
                }
                HOperatorSet.DilationCircle(userRegion.DisplayRegion, out HObject resultRegion, userRegion.RegionParameters[0]);

                userRegion.CalculateRegion = resultRegion;
                userRegion.RegionOperatType = RegionOperatType.Dilation;
                htWindow.DisplayMultiRegion(CutRegionRegions);
            }
        }
        

        private void ExecuteSaveCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    if (CutRegionParameter.OnRecipesIndex == -1)
                    {
                        MessageBox.Show("请先选择属于哪个框架！");
                        return;
                    }
                    OnSaveXML?.Invoke();                      
                    
                    MessageBox.Show("保存完成!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "参数保存失败");
                }
            }
        }

        private void ExecuteIsCheckCommand(object parameter)
        {
            if (CutRegionUserRegions.All(x => x.IsSelected==true))
            { IsCheckAll = true; }
            else if (CutRegionUserRegions.All(x =>!x.IsSelected))
            { IsCheckAll = false; }
            else 
            { IsCheckAll = null; }
        }

        private void ExecuteIsCheckAllCommand(object parameter)
        {
            if (IsCheckAll==true)
            { CutRegionUserRegions.ToList().ForEach(r => r.IsSelected = true); }
            else if(IsCheckAll == false)
            { CutRegionUserRegions.ToList().ForEach(r => r.IsSelected = false); }
        }

        public bool CheckCompleted()
        {
            /*
            if (string.IsNullOrEmpty(CutRegionParameter.ImagePath) || !File.Exists(CutRegionParameter.ImagePath))
            {
                System.Windows.MessageBox.Show("图片不存在，请重新选择");
                return false;
            }
            if (string.IsNullOrEmpty(CutRegionParameter.TrainningImagesDirectory) || !Directory.Exists(CutRegionParameter.TrainningImagesDirectory))
            {
                System.Windows.MessageBox.Show("训练图像文件夹不存在，请重新选择");
                return false;
            }
            if (CutRegionUserRegions.Count == 0)
            {
                System.Windows.MessageBox.Show("请选择一个Die区域");
                return false;
            }
            if (EpoxyModelObject.DieImage == null || !EpoxyModelObject.DieImage.IsInitialized())
            {
                System.Windows.MessageBox.Show("请裁剪一个Die区域");
                return false;
            }
            */
            return true;
        }

        public void Initial()
        {
            htWindow.ClearSelection();
            if (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff) return;            
            //htWindow.DisplayMultiRegion(CutRegionRegions, Algorithm.Region.GetChannnelImageUpdate(EpoxyModelObject.DieImage, CutRegionParameter.ImageChannelIndex));
        }

        public void Dispose()
        {
            (Content as Page_CutRegionModel).DataContext = null;
            (Content as Page_CutRegionModel).Close();
            Content = null;
            htWindow = null;
            CutRegionParameter = null;
            EpoxyModelObject = null;
            CutRegionUserRegions = null;
            LoadReferenceCommand = null;
            RemoveUserRegionCommand = null;
            UserRegionEnableChangedCommand = null;
            ErosionCommand = null;
            DilationCommand = null;
            SaveCommand = null;
            IsCheckCommand = null;
            IsCheckAllCommand = null;
        }
    }
}
