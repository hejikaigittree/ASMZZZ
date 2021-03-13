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
    class CreateGoldenModel : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        //显示图像尺寸设置
        private HTuple imgWidth;//

        private HTuple imgHeight;//     

        public GoldenModelParameter GoldenModelParameter { get; private set; }

        public GoldenModelInspectParameter GoldenModelInspectParameter { get; private set; }        

        public ObservableCollection<UserRegion> DieUserRegions { get; private set; }

        public ObservableCollection<UserRegion> MatchUserRegions { get; private set; }

        public ObservableCollection<UserRegion> InspectUserRegions { get; private set; }

        public ObservableCollection<UserRegion> RejectUserRegions { get; private set; }

        public ObservableCollection<UserRegion> SubUserRegions { get; private set; }

        private IEnumerable<HObject> InspectRegions => InspectUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        private HTHalControlWPF htWindow;

        private GoldenModelObject goldenModelObject;

        private readonly string RecipeDirectory;

        private readonly string ReferenceDirectory;

        private readonly string ModelsFile;

        private readonly string RecipeFile;

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

        public CommandBase CreateGoldenModelCommand { get; private set; }
        public CommandBase DisplayLightDarkImageCommand { get; private set; }
        public CommandBase CreatePosModelCommand { get; private set; }
        public CommandBase ICOnCommand { get; private set; }

        public CreateGoldenModel(HTHalControlWPF htWindow,
                                 string modelsFile,
                                 string recipeFile,
                                 GoldenModelParameter goldenModelParameter,
                                 GoldenModelInspectParameter goldenModelInspectParameter,
                                 GoldenModelObject goldenModelObject,
                                 ObservableCollection<UserRegion> dieUserRegions,
                                 ObservableCollection<UserRegion> matchUserRegions,
                                 ObservableCollection<UserRegion> inspectUserRegions,
                                 ObservableCollection<UserRegion> rejectUserRegions,
                                 ObservableCollection<UserRegion> subUserRegions,
                                 string recipeDirectory,
                                 string referenceDirectory)
        {
            DisplayName = "创建定位模板、黄金模板";
            Content = new Page_CreateGoldenModel { DataContext = this };

            this.htWindow = htWindow;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.GoldenModelParameter = goldenModelParameter;
            this.GoldenModelInspectParameter = goldenModelInspectParameter;
            this.goldenModelObject = goldenModelObject;
            this.DieUserRegions = dieUserRegions;
            this.MatchUserRegions = matchUserRegions;
            this.InspectUserRegions = inspectUserRegions;
            this.RejectUserRegions = rejectUserRegions;
            this.SubUserRegions = subUserRegions;
            this.RecipeDirectory = recipeDirectory;
            this.ReferenceDirectory = referenceDirectory;
            ICOnCommand = new CommandBase(ExecuteICOnCommand);

            CreateGoldenModelCommand = new CommandBase(ExecuteCreateGoldenModelCommand);
            CreatePosModelCommand = new CommandBase(ExecuteCreatePosModelCommand);
            DisplayLightDarkImageCommand=new CommandBase(ExecuteDisplayLightDarkImageCommand);
        }

        private void ExecuteICOnCommand(object parameter)
        {
            try
            {
                HTuple filesFrame = new HTuple();
                filesFrame = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                                      "Frame*.*", SearchOption.TopDirectoryOnly);
                GoldenModelParameter.OnRecipesIndexs1 = new string[filesFrame.Length];

                for (int i = 0; i < filesFrame.Length; i++)
                {
                    GoldenModelParameter.OnRecipesIndexs1[i] = Path.GetFileName(filesFrame[i]);
                }
                AddBondOnItem();
                (Content as Page_CreateGoldenModel).ICOnWhat.IsDropDownOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void AddBondOnItem()
        {
            (Content as Page_CreateGoldenModel).ICOnWhat.Items.Clear();
            for (int j = 0; j < GoldenModelParameter.OnRecipesIndexs1.Length; j++)
            {
                ComboBoxItem cboxItem = new ComboBoxItem();
                cboxItem.Content = GoldenModelParameter.OnRecipesIndexs1[j];
                (Content as Page_CreateGoldenModel).ICOnWhat.Items.Add(cboxItem);
            }
        }

        //创建黄金模板
        private void ExecuteCreateGoldenModelCommand(object parameter)
        {
            if (GoldenModelParameter.ModelType.Equals(string.Empty))
            {
                MessageBox.Show("请选择模板类型");
                return;
            }
            if (goldenModelObject.Image == null || !goldenModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载参考图像");
                return;
            }
            if (GoldenModelParameter.ImageChannelIndex < 0)
            {
                MessageBox.Show("请选择创建模板图像索引");
                return;
            }
            if (GoldenModelParameter.MinTrainSet < 0)
            {
                MessageBox.Show("训练集最小大小不能小于0");
                return;
            }
            if (GoldenModelParameter.CurrentTrainSet < GoldenModelParameter.MinTrainSet)
            {
                MessageBox.Show("当前图集数目小于最少训练集");
                return;
            }
            if (GoldenModelParameter.RefineThresh < 0)
            {
                MessageBox.Show("精炼阈值不能小于0");
                return;
            }
            if (GoldenModelParameter.ImageCountChannels < 0 && GoldenModelParameter.ImageChannelIndex == 0) //1121
            {
                MessageBox.Show("请先选择创建定位模板图像通道！");
                return;
            }
            if (GoldenModelParameter.ImageCountChannels < 0 && GoldenModelParameter.ImageGoldChannelIndex == 0) //1121
            {
                MessageBox.Show("请先选择创建黄金模板图像通道！");
                return;
            }
            if (!File.Exists($"{ReferenceDirectory}TrainningImagesDirectory.tup"))
            {
                MessageBox.Show("请先到全局加载训练图集并保存！");
                return;
            }

            Window_Loading window_Loading = new Window_Loading("正在生成黄金模板");

            try
            {
                HOperatorSet.ReadTuple($"{ReferenceDirectory}TrainningImagesDirectory.tup", out HTuple TrainningImagesDirectoryTemp);
                GoldenModelParameter.TrainningImagesDirectory = TrainningImagesDirectoryTemp;

                HTuple imageFiles = null;
                Algorithm.File.list_image_files(GoldenModelParameter.TrainningImagesDirectory, "default", "recursive", out imageFiles);
                HObject meanImage = null;
                HObject stdImage = null;

                window_Loading.Show();

                // 1122 - 暂定黄金模板和定位用一张图
                if (GoldenModelParameter.ImageChannelIndex != GoldenModelParameter.ImageGoldChannelIndex)
                {
                    GoldenModelParameter.ImageGoldChannelIndex = GoldenModelParameter.ImageChannelIndex;
                }

                //HOperatorSet.AccessChannel(goldenModelObject.Image, out HObject ChannelPosImage, GoldenModelParameter.ImageChannelIndex + 1);
                HObject ChannelPosImage = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.Image, GoldenModelParameter.ImageChannelIndex);

                Algorithm.Model_RegionAlg.HTV_GenModels_TargetInspect_Recipe(ChannelPosImage,//定位模板通道图 , RGB->123 gray->1
                                        Algorithm.Region.ConcatRegion(DieUserRegions),
                                        GoldenModelParameter.IsMultiModelPosMode ? Algorithm.Region.ConcatRegion(MatchUserRegions) : Algorithm.Region.Union1Region(MatchUserRegions),
                                        Algorithm.Region.ConcatRegion(InspectUserRegions),
                                        Algorithm.Region.ConcatRegion(RejectUserRegions),
                                        Algorithm.Region.ConcatRegion(SubUserRegions),
                                        out meanImage,
                                        out stdImage,
                                        imageFiles,
                                        ModelsFile,
                                        RecipeFile,
                                        GoldenModelParameter.OnRecipesIndexs1[GoldenModelParameter.OnRecipesIndex1],
                                        GoldenModelParameter.ModelType,//0
                                        GoldenModelParameter.IsMultiModelMode ? 1 : 0,  //0
                                        GoldenModelParameter.ImageCountChannels == 1 ? 1 : GoldenModelParameter.ImageChannelIndex + 1,//HTuple hv_i_FindIcImgIdx, RGB->123 gray->1
                                        GoldenModelParameter.ImageCountChannels == 1 ? 1 : GoldenModelParameter.ImageGoldChannelIndex + 1,//HTuple hv_i_IcGoldenImgIdx,   RGB->123 gray->1
                                        GoldenModelParameter.MinTrainSet,
                                        GoldenModelParameter.IsRefine ? 1 : 0,  //0
                                        GoldenModelParameter.RefineThresh,
                                        GoldenModelParameter.IsGoldenMatchModel ? 1 : 0,
                                        out HTuple modelID);//此模板modelID不保存

                // 黄金模板里的定位模板不使用，此处clear 0121 lw 
                if (modelID != null && modelID.TupleLength() > 0)
                {
                    HTuple ModelTypeStr = null;
                    ModelTypeStr = GoldenModelParameter.ModelType == 0 ? "ncc" : "shape";
                    Algorithm.Model_RegionAlg.HTV_clear_model(modelID, ModelTypeStr);
                    modelID = null;
                }
				
                goldenModelObject.MeadImage = meanImage;
                goldenModelObject.StdImage = stdImage;
                try
                {
                    //保存均值图、方差图、模板
                    GoldenModelParameter.MeanImagePath = $"{RecipeDirectory}Mean_Image.tiff";
                    //GoldenModelParameter.StdImagePath = $"{RecipeDirectory}Std_Image.tiff";
                    HOperatorSet.WriteImage(goldenModelObject.MeadImage, "tiff", 0, GoldenModelParameter.MeanImagePath);
                    //HOperatorSet.WriteImage(goldenModelObject.StdImage, "tiff", 0, GoldenModelParameter.StdImagePath);
                }
                catch (Exception ex)
                {
                    window_Loading.Close();
                    MessageBox.Show(ex.ToString(),"创建黄金模板的图集中可能有离焦图像，请剔除!");
                    return;
                }

                //modelID和定位模板相同，只保存定位模板ID
                //goldenModelObject.ModelID = modelID;
                /*
                if (modelID.TupleLength() == 1)
                {
                    GoldenModelParameter.ModelIdPath = $"{recipeDirectory}PosModel.dat";
                    Algorithm.File.SaveModel(GoldenModelParameter.ModelIdPath, GoldenModelParameter.ModelType, goldenModelObject.ModelID);
                }
                else if(modelID.TupleLength() > 1)
                {
                    String[] ModelIdPathArry = new string[goldenModelObject.ModelID.TupleLength()];
                    for (int i = 0; i < goldenModelObject.ModelID.TupleLength(); i++)
                    {
                        ModelIdPathArry[i] = $"{recipeDirectory}PosModel" + i + ".dat";
                    }
                    GoldenModelParameter.ModelIdPath = String.Join(",", ModelIdPathArry);
                    Algorithm.File.SaveModel(ModelIdPathArry, GoldenModelParameter.ModelType, goldenModelObject.ModelID);//
                }     
                */
                //isCompleted = true;
                window_Loading.Close();
                MessageBox.Show("黄金模板已生成");
            }
            catch (Exception ex)
            {
                window_Loading.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        //创建定位模板 
        private void ExecuteCreatePosModelCommand(object parameter)
        {
            try
            {
                if (GoldenModelParameter.ImageCountChannels > 0 && GoldenModelParameter.ImageChannelIndex < 0)
                {
                    MessageBox.Show("请先选择通道图像！");
                    return;
                }
                if (MatchUserRegions.Count == 0)
                {
                    MessageBox.Show("请先添加匹配区域！");
                    return;
                }
                if (goldenModelObject.Image == null || !goldenModelObject.Image.IsInitialized())
                {
                    MessageBox.Show("请加载参考图像");
                    return;
                }

                // 0115 lw
                if (goldenModelObject.PosModelID != null && goldenModelObject.PosModelID.TupleLength() > 0)
                {
                    HTuple ModelTypeStr = null;
                    ModelTypeStr = GoldenModelParameter.ModelType == 0 ? "ncc" : "shape";
                    Algorithm.Model_RegionAlg.HTV_clear_model(goldenModelObject.PosModelID, ModelTypeStr);
                    goldenModelObject.PosModelID = null;
                }

                //12-05  
                Algorithm.Model_RegionAlg.HTV_CreateLocModel(Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.Image, GoldenModelParameter.ImageChannelIndex),
                                                            Algorithm.Region.Union1Region(MatchUserRegions),//芯片没有用多等位模板，还是之前的
                                                            GoldenModelParameter.ModelType,
                                                            GoldenModelParameter.AngleStart,//传入接口都是角度值
                                                            GoldenModelParameter.AngleExt,//传入接口都是角度值
                                                            out HTuple PosModelID);

                //Algorithm.Model_RegionAlg.HTV_CreateLocModel(Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.Image, GoldenModelParameter.ImageChannelIndex),
                                                   //GoldenModelParameter.IsMultiModelPosMode? Algorithm.Region.ConcatRegion(MatchUserRegions): Algorithm.Region.Union1Region(MatchUserRegions),
                                                   //Algorithm.Region.Union1Region(MatchUserRegions),
                                                   //GoldenModelParameter.ModelType,
                                                   //out HTuple PosModelID);

                goldenModelObject.PosModelID = PosModelID;

                //保存模板
                if (PosModelID.TupleLength() == 1 )
                {
                    GoldenModelParameter.PosModelIdPath = $"{RecipeDirectory}PosModel.dat";
                    Algorithm.File.SaveModel(GoldenModelParameter.PosModelIdPath, GoldenModelParameter.ModelType, goldenModelObject.PosModelID);
                    MessageBox.Show("定位模板已生成!");
                }
                else if (PosModelID.TupleLength() > 1)
                {
                    String[] ModelIdPathArry = new string[goldenModelObject.PosModelID.TupleLength()];
                    for (int i = 0; i < goldenModelObject.PosModelID.TupleLength(); i++)
                    {
                        ModelIdPathArry[i] = $"{RecipeDirectory}PosModel_" + i + ".dat";
                    }
                    GoldenModelParameter.PosModelIdPath = String.Join(",", ModelIdPathArry);
                    Algorithm.File.SaveModel(ModelIdPathArry, GoldenModelParameter.ModelType, goldenModelObject.PosModelID);//
                    MessageBox.Show("定位模板已生成!");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //显示亮暗图像
        private void ExecuteDisplayLightDarkImageCommand(object parameter)
        {
            try
            {
                HOperatorSet.GenEmptyObj(out HObject Mean_Image);
                HOperatorSet.GenEmptyObj(out HObject Std_Image);
                (Content as Page_CreateGoldenModel).hTWindow1.HalconWindow.ClearWindow();
                (Content as Page_CreateGoldenModel).hTWindow2.HalconWindow.ClearWindow();

                if (goldenModelObject.MeadImage != null)
                {
                    Mean_Image = goldenModelObject.MeadImage;
                }
                else if (File.Exists($"{RecipeDirectory}Mean_Image.tiff"))
                {
                    HOperatorSet.ReadImage(out Mean_Image, $"{RecipeDirectory}Mean_Image.tiff");
                }
                else
                {
                    MessageBox.Show("请先生成均值图！");
                    return;
                }
                if (goldenModelObject.StdImage != null)
                {
                    Std_Image = goldenModelObject.StdImage;
                }
                else if (File.Exists($"{RecipeDirectory}Std_Image.tiff"))
                {
                    HOperatorSet.ReadImage(out Std_Image, $"{RecipeDirectory}Std_Image.tiff");
                }
                else
                {
                    MessageBox.Show("请先生成方差图！");
                    return;
                }

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

                HOperatorSet.ReduceDomain(LightImage, 
                                          Algorithm.Region.ConcatRegion(InspectUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)), 
                                          out HObject Light_Image_Reduced);
                HOperatorSet.ReduceDomain(DarkImage,
                                          Algorithm.Region.ConcatRegion(InspectUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                          out HObject Dark_Image_Reduced);
                HOperatorSet.CropDomain(Light_Image_Reduced, out HObject Light_Image_Croped);
                HOperatorSet.CropDomain(Dark_Image_Reduced, out HObject Dark_Image_Croped); 
                HOperatorSet.GetImageSize(Light_Image_Croped, out imgWidth, out imgHeight);
                setImagePart((Content as Page_CreateGoldenModel).hTWindow1, 0, 0, imgHeight, imgWidth);
                setImagePart((Content as Page_CreateGoldenModel).hTWindow2, 0, 0, imgHeight, imgWidth);
                (Content as Page_CreateGoldenModel).hTWindow1.HalconWindow.DispObj(Light_Image_Croped);
                (Content as Page_CreateGoldenModel).hTWindow2.HalconWindow.DispObj(Dark_Image_Croped);                
                GoldenModelParameter.LightImagePath  = $"{RecipeDirectory}Light_Image.tiff";
                GoldenModelParameter.DarkImagePath = $"{RecipeDirectory}Dark_Image.tiff";
                HOperatorSet.WriteImage(goldenModelObject.LightImage, "tiff", 0, GoldenModelParameter.LightImagePath);
                HOperatorSet.WriteImage(goldenModelObject.DarkImage, "tiff", 0, GoldenModelParameter.DarkImagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void setImagePart(HWindowControlWPF hwindow, int r1, int c1, int r2, int c2)//
        {
            double ImgRow1, ImgCol1, ImgRow2, ImgCol2;
            ImgRow1 = r1;
            ImgCol1 = c1;
            ImgRow2 = imgHeight.D;
            ImgCol2 = imgWidth.D;
            System.Windows.Thickness rect = hwindow.ImagePart;
            rect.Left = (int)ImgCol1;
            rect.Top = (int)ImgRow1;
            rect.Bottom = (int)imgHeight;
            rect.Right = (int)imgWidth;
            hwindow.ImagePart = rect;
        }

        public bool CheckCompleted()
        {
            /*
            if (!isCompleted)
            {
                MessageBox.Show("请创建黄金模板");
            }           
            return isCompleted;
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


            // 避免检测验证滚轮区域显示 lw 0127
            GoldenModelParameter.IsInspectVerify = false;
        }

        public void Dispose()
        {
            (Content as Page_CreateGoldenModel).DataContext = null;
            (Content as Page_CreateGoldenModel).Close();
            Content = null;

            this.htWindow = null;
            this.GoldenModelParameter = null;
            GoldenModelInspectParameter = null;
            this.goldenModelObject = null;
            this.DieUserRegions = null;
            this.MatchUserRegions = null;
            this.InspectUserRegions = null;
            this.RejectUserRegions = null;
            this.SubUserRegions = null;
            CreateGoldenModelCommand = null;
            CreatePosModelCommand = null;
            DisplayLightDarkImageCommand = null;
        }
    }
}
