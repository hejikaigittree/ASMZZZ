using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace LFAOIRecipe
{
    class CreateBond2Model : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public bool isRightClick = true;

        public string ReferenceDirectory { get; set; } 
        
        private string imageIndex;
        public string ImageIndex
        {
            get => imageIndex;
            set
            {
                if (imageIndex != value)
                {
                    Bond2ModelObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, Bond2ModelParameter.ImageChannelIndex);
                    htWindow.Display(Bond2ModelObject.ChannelImage);
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        // 1122
        public int ImageChannelIndex;

        private Bond2Model currentModel;
        public Bond2Model CurrentModel
        {
            get => currentModel;
            set
            {
                //if (currentModel != value && isRightClick==true)
                if (currentModel != value )
                {
                    currentModel = value;
                    OnPropertyChanged();

                    rotatedImage?.Dispose();
                    rotatedImage = null;
                    htWindow.DisplayMultiRegion(RefineRegions, Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, Bond2ModelParameter.ImageChannelIndex));
                    if (currentModel == null) return;
                    if (File.Exists(currentModel.RotatedImagePath))
                    {
                        try
                        {
                            HOperatorSet.ReadImage(out rotatedImage, currentModel.RotatedImagePath);
                        }
                        catch { }
                    }
                    //htWindow.DisplayMultiRegion(RefineRegions, Bond2ModelObject.Image);  //original
                }
            }
        }

        public Bond2ModelObject Bond2ModelObject { get; set; }

        private int modelsCount;
        public int ModelsCount
        {
            get => modelsCount;
            set => OnPropertyChanged(ref modelsCount, value);
        }

        private int switchImageComboBoxIndex;
        public int SwitchImageComboBoxIndex
        {
            get => switchImageComboBoxIndex;
            set
            {
                if (switchImageComboBoxIndex != value)
                {
                    if (!File.Exists(Bond2ModelParameter.ImagePath) || !File.Exists($"{bondRecipeDirectory}\\RotatedImage{CurrentModel?.Index.ToString()}.tiff")) 
                    {
                        switchImageComboBoxIndex = value;
                        OnPropertyChanged();
                        return;
                    }
                    if (value == 0)
                    {
                        htWindow.DisplayMultiRegionLine(RefineRegions, currentModel.RotateLineUserRegion?.DisplayRegion , Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, Bond2ModelParameter.ImageChannelIndex)); 
                    }
                    else if (value == 1)
                    {                        
                        HOperatorSet.ReadImage(out rotatedImage, $"{bondRecipeDirectory}\\RotatedImage{CurrentModel?.Index.ToString()}.tiff");
                        //htWindow.DisplayMultiRegionTwo(CurrentModel?.Bond2UserRegion?.CalculateRegion, CurrentModel?.Bond2UserRegionDiff?.CalculateRegion, Algorithm.Region.GetChannnelImageUpdate(rotatedImage, Bond2ModelParameter.ImageChannelIndex));
                        htWindow.DisplayMultiRegionTwo(CurrentModel?.Bond2UserRegion?.CalculateRegion, CurrentModel?.Bond2UserRegionDiff?.CalculateRegion,rotatedImage);
                    }
                     switchImageComboBoxIndex = value;
                     OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Bond2Model> Models { get; private set; }

        public IEnumerable<HObject> RefineRegions => CurrentModel?.RefineUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public Bond2ModelParameter Bond2ModelParameter { get; private set; }       

        public CommandBase AddModelCommand { get; private set; }
        public CommandBase RemoveModelCommand { get; private set; }
        public CommandBase RotateCommand { get; private set; }
        public CommandBase AddRefineUserRegionCommand { get; private set; }
        public CommandBase RemoveRefineUserRegionCommand { get; private set; }
        public CommandBase AddBond2UserRegionCommand { get; private set; }
        public CommandBase AddBond2UserRegionDiffCommand { get; private set; }
        public CommandBase CreateBond2ModelCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }
        public CommandBase ModifyBond2ModelRegionCommand { get; private set; }
        public CommandBase ModifyBond2ModelRegionDiffCommand { get; private set; }
        public CommandBase SaveBond2ModelCommand { get; private set; }
        public CommandBase LoadReferenceCommand { get; private set; }
        public CommandBase SelectedChangedImageCommand { get; private set; }

        private HTHalControlWPF htWindow;

        private HObject rotatedImage=null;//

        private HObject bond2ModelImage=null;//

        private readonly string bondRecipeDirectory;

        private readonly string ModelsDirectory;

        public CreateBond2Model(HTHalControlWPF htWindow, 
                                string modelsDirectory,
                                string referenceDirectory,
                                Bond2ModelObject bond2ModelObject,
                                ObservableCollection<Bond2Model> bond2Models,
                                Bond2ModelParameter parameter,
                                string bondRecipeDirctory)
        {
            DisplayName = "创建焊点匹配模板";
            Content = new Page_CreateBond2Model { DataContext = this };
            this.htWindow = htWindow;
            ReferenceDirectory = referenceDirectory;
            Bond2ModelObject = bond2ModelObject;
            Models = bond2Models;
            Bond2ModelParameter = parameter;
            this.bondRecipeDirectory = bondRecipeDirctory;
            this.ModelsDirectory = modelsDirectory;

            AddModelCommand = new CommandBase(ExecuteAddModelCommand);
            RemoveModelCommand = new CommandBase(ExecuteRemoveModelCommand);
            RotateCommand = new CommandBase(ExecuteRotateCommand);
            AddRefineUserRegionCommand = new CommandBase(ExecuteAddRefineUserRegionCommand);
            RemoveRefineUserRegionCommand = new CommandBase(ExecuteRemoveRefineUserRegionCommand);
            AddBond2UserRegionCommand = new CommandBase(ExecuteAddBond2UserRegionCommand);
            AddBond2UserRegionDiffCommand = new CommandBase(ExecuteAddBond2UserRegionDiffCommand);
            CreateBond2ModelCommand = new CommandBase(ExecuteCreateBond2ModelCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);
            ModifyBond2ModelRegionCommand = new CommandBase(ExecuteModifyBond2ModelRegionCommand);
            ModifyBond2ModelRegionDiffCommand = new CommandBase(ExecuteModifyBond2ModelRegionDiffCommand);
            SaveBond2ModelCommand = new CommandBase(ExecuteSaveBond2ModelCommand);
            LoadReferenceCommand = new CommandBase(ExecuteLoadReferenceCommand);
            // 1122-lw
            SelectedChangedImageCommand = new CommandBase(ExecuteSelectedChangedImageCommand);

            CurrentModel = new Bond2Model
            {
                Index = 1
            };
            Models.Add(CurrentModel);
            ModelsCount = Models.Count;
        }

        private void ExecuteAddModelCommand(object parameter)
        {
            if (isRightClick != true) return;
            CurrentModel = new Bond2Model
            {
                Index = Models.Count + 1
            };
            Models.Add(CurrentModel);
            ModelsCount = Models.Count;
            SwitchToImage();
            MessageBox.Show($"新建了序号 {CurrentModel.Index.ToString()} 的模板配置");
        }

        private void ExecuteRemoveModelCommand(object parameter)
        {
            if (isRightClick != true) return;

            if (CurrentModel == null) return;
            if (MessageBox.Show($"是否删除序号 {CurrentModel.Index.ToString()} 的模板配置", "删除", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;
            if (File.Exists(CurrentModel.RotatedImagePath))
            {
                File.Delete(CurrentModel.RotatedImagePath);
            }
            for (int i = CurrentModel.Index; i < Models.Count; i++)
            {
                Models[i].Index--;
                if (File.Exists(Models[i].RotatedImagePath))
                {
                    string newRotatedImagePath = $"{bondRecipeDirectory}\\RotatedImage{Models[i].Index.ToString()}.tiff";
                    File.Move(Models[i].RotatedImagePath, newRotatedImagePath);
                    Models[i].RotatedImagePath = newRotatedImagePath;
                }
            }            
            Models.Remove(CurrentModel);
            ModelsCount = Models.Count;
        }

        //加载参考数据
        private void ExecuteLoadReferenceCommand(object parameter)
        {
            if (isRightClick != true) return;
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
                filesIC = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                           "IC*.*", SearchOption.TopDirectoryOnly);
                string[] files = filesFrame.TupleConcat(filesIC);
                string[] OnRecipesIndexs = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    OnRecipesIndexs[i] = Path.GetFileName(files[i]);
                }
                Bond2ModelParameter.OnRecipesIndexs = OnRecipesIndexs;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "全局数据文件不存在！");
                return;
            }
        }

        public void LoadReferenceData()
        {
            //if (isRightClick != true) return;
            if (!( File.Exists($"{ReferenceDirectory}ReferenceImage.tiff")
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
            Bond2ModelParameter.VerifyImagesDirectory = TrainningImagesDirectoryTemp;

            Bond2ModelParameter.ImagePath = $"{ReferenceDirectory}ReferenceImage.tiff";
            LoadImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageRowOffset.tup", out HTuple DieImageRowOffsetTemp);
            Bond2ModelParameter.DieImageRowOffset = DieImageRowOffsetTemp;
            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageColumnOffset.tup", out HTuple DieImageColumnOffsetTemp);
            Bond2ModelParameter.DieImageColumnOffset = DieImageColumnOffsetTemp;

            HOperatorSet.ReadRegion(out HObject UserRegionForCutOut_Region, ReferenceDirectory + "DieReference.reg");
            HOperatorSet.ReduceDomain((HObject)Bond2ModelObject.Image, UserRegionForCutOut_Region, out HObject imageReduced);
            HOperatorSet.CropDomain(imageReduced, out HObject dieImage);
            Bond2ModelObject.DieImage = dieImage;
            LoadDieImage();
            HOperatorSet.ReadTuple(ReferenceDirectory + "UserRegionForCutOutIndex.tup", out HTuple UserRegionForCutOutIndexTemp);
            Bond2ModelParameter.UserRegionForCutOutIndex = UserRegionForCutOutIndexTemp;

            //1122 lw
            HOperatorSet.ReadTuple(ReferenceDirectory + "ImageChannelnumber.tup", out HTuple ImageChannelnumber);
            ChannelNames.Clear();
            for (int i = 0; i < ImageChannelnumber; i++)
            {
                ChannelName tmp_name = new ChannelName();
                tmp_name.Name = (i + 1).ToString();
                ChannelNames.Add(tmp_name);
            }

            ImageChannelIndex = Bond2ModelParameter.ImageChannelIndex;

            Bond2ModelParameter.ChannelNames = ChannelNames;
            OnPropertyChanged("Bond2ModelParameter.ImageChannelIndex");

            //1201 lw
            HOperatorSet.TupleSplit(ReferenceDirectory, "\\", out HTuple hv_subStr);
            HTuple FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 3];
            Bond2ModelParameter.CurFovName = FOV_Name;

            htWindow.DisplaySingleRegion(UserRegionForCutOut_Region, Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, Bond2ModelParameter.ImageChannelIndex));

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
            HOperatorSet.ReadImage(out image, Bond2ModelParameter.ImagePath);
            Bond2ModelObject.Image = image;
            //SwitchToImage();

            HOperatorSet.CountChannels((HObject)Bond2ModelObject.Image, out HTuple channels);
            Bond2ModelParameter.ImageCountChannels = channels;

            Bond2ModelObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, Bond2ModelParameter.ImageChannelIndex);
            //Bond2ModelObject.ImageR = Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, 1);
            //Bond2ModelObject.ImageG = Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, 2);
            //Bond2ModelObject.ImageB = Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, 3);

            //if(Bond2ModelParameter.ImageCountChannels == 3)
            //{
            //    HOperatorSet.GenEmptyObj(out HObject images);
            //    HOperatorSet.ConcatObj(images, Bond2ModelObject.ImageR, out images);
            //    HOperatorSet.ConcatObj(images, Bond2ModelObject.ImageG, out images);
            //    HOperatorSet.ConcatObj(images, Bond2ModelObject.ImageB, out images);
            //    Bond2ModelObject.Images = images;
            //}
        }

        public void LoadDieImage()
        {
            try
            {
                Bond2ModelObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.DieImage, Bond2ModelParameter.ImageChannelIndex);
                //Bond2ModelObject.DieImageR = Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.DieImage, 1);
                //Bond2ModelObject.DieImageG = Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.DieImage, 2);
                //Bond2ModelObject.DieImageB = Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.DieImage, 3);
                //if (Bond2ModelParameter.ImageCountChannels == 3)
                //{
                //    HOperatorSet.GenEmptyObj(out HObject dieimages);
                //    HOperatorSet.ConcatObj(dieimages, Bond2ModelObject.DieImageR, out dieimages);
                //    HOperatorSet.ConcatObj(dieimages, Bond2ModelObject.DieImageG, out dieimages);
                //    HOperatorSet.ConcatObj(dieimages, Bond2ModelObject.DieImageB, out dieimages);
                //    Bond2ModelObject.DieImages = dieimages;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteSelectedChangedImageCommand(object parameter)
        {
            // 1122
            if (Bond2ModelParameter.ImageChannelIndex >= 0)
            { 
                Bond2ModelObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, Bond2ModelParameter.ImageChannelIndex);
                ImageChannelIndex = Bond2ModelParameter.ImageChannelIndex;
                htWindow.Display(Bond2ModelObject.ChannelImage);
                OnPropertyChanged();
            }
            else
            {
                Bond2ModelParameter.ImageChannelIndex = ImageChannelIndex;
            }
        }

        private void ExecuteRotateCommand(object parameter)
        {
            if (isRightClick != true) return;
            if (Bond2ModelObject.Image == null || !Bond2ModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像!");
                return;
            }
            if (CurrentModel == null)
            {
                MessageBox.Show("请选择或者新建一个模板配置");
                return;
            }
            if (SwitchImageComboBoxIndex != 0)
            {
                MessageBox.Show("请切换到原图再进行操作");
                return;
            }
            if (htWindow.RegionType != RegionType.Line)
            {
                MessageBox.Show("请选择画直线按钮重新画一条直线！");
                return;
            }
            rotatedImage?.Dispose();
            rotatedImage = null;
            HOperatorSet.GenEmptyObj(out HObject displayRegion);

            try
            {
                Algorithm.File.RotateImage(Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, Bond2ModelParameter.ImageChannelIndex),
                                       out rotatedImage,
                                           htWindow.Row1_Line,
                                           htWindow.Column1_Line,
                                           htWindow.Row2_Line,
                                           htWindow.Column2_Line);
                
                UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, htWindow.RegionType, htWindow.Row1_Line, htWindow.Column1_Line, htWindow.Row2_Line, htWindow.Column2_Line);
                if (userRegion == null) return;
                CurrentModel.RotateLineUserRegion = userRegion;

                HOperatorSet.LineOrientation(htWindow.Row1_Line, htWindow.Column1_Line, htWindow.Row2_Line, htWindow.Column2_Line, out HTuple AngleRotate);
                CurrentModel.RotatedImageAngel = AngleRotate.D;

                CurrentModel.RotatedImagePath = $"{bondRecipeDirectory}RotatedImage{CurrentModel.Index.ToString()}.tiff";
                HOperatorSet.WriteImage(rotatedImage, "tiff", 0, CurrentModel.RotatedImagePath);

                CurrentModel.Bond2UserRegion = null;
                CurrentModel.Bond2UserRegionDiff = null;

                htWindow.Display(rotatedImage, true); 
                SwitchToRotatedImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                htWindow.RegionType = RegionType.Null;//
            }
        }

        private void ExecuteAddBond2UserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;//
            if (Bond2ModelObject.Image == null || !Bond2ModelObject.Image.IsInitialized())
            {
                MessageBox.Show("没有参考图像！");
                return;
            }
            if (CurrentModel == null)
            {
                MessageBox.Show("请选择或者新建一个模板配置！");
                return;
            }
            if (SwitchImageComboBoxIndex != 1)
            {
                MessageBox.Show("请切换到转正图再进行操作！");
                return;
            }
            if (!File.Exists($"{bondRecipeDirectory}\\RotatedImage{CurrentModel.Index.ToString()}.tiff"))
            {
                MessageBox.Show("转正图像不存在，请对原图进行转正！");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow);
                if (userRegion == null) return;
                CurrentModel.Bond2UserRegion = userRegion;
                htWindow.DisplayMultiRegionTwo(CurrentModel.Bond2UserRegion.CalculateRegion, CurrentModel.Bond2UserRegionDiff?.CalculateRegion);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                htWindow.RegionType = RegionType.Null;
            }

        }

        private void ExecuteAddBond2UserRegionDiffCommand(object parameter)
        {
            if (isRightClick != true) return;//
            if (Bond2ModelObject.Image == null || !Bond2ModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }
            if (CurrentModel == null)
            {
                MessageBox.Show("请选择或者新建一个模板配置");
                return;
            }
            if (SwitchImageComboBoxIndex != 1)
            {
                MessageBox.Show("请切换到转正图再进行操作");
                return;
            }
            if (rotatedImage == null || !rotatedImage.IsInitialized())
            {
                MessageBox.Show("转正图像不存在，请对原图进行转正");
                return;
            }
            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow);
                if (userRegion == null) return;
                userRegion.IsAccept = false;
                CurrentModel.Bond2UserRegionDiff = userRegion;
                htWindow.DisplayMultiRegionTwo(CurrentModel.Bond2UserRegion?.CalculateRegion, CurrentModel.Bond2UserRegionDiff.CalculateRegion);
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

        private void ExecuteUserRegionEnableChangedCommand(object parameter)
        {
            if (isRightClick != true) return;
            //htWindow.DisplayMultiRegion(RefineRegions, Bond2ModelObject.Image);
            htWindow.DisplayMultiRegion(RefineRegions);
        }

        private void ExecuteAddRefineUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;//
            if (CurrentModel == null)
            {
                MessageBox.Show("请选择或者新建一个模板配置");
                return;
            }
            if (Bond2ModelObject.Image == null || !Bond2ModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }

            if (SwitchImageComboBoxIndex != 0)
            {
                MessageBox.Show("请切换到原图再进行操作");
                return;
            }

            try
            {
                UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow);
                if (userRegion == null) return;
                userRegion.Index = CurrentModel.RefineUserRegions.Count + 1;
                CurrentModel.RefineUserRegions.Add(userRegion);
                htWindow.DisplayMultiRegion(RefineRegions);
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

        private void ExecuteRemoveRefineUserRegionCommand(object parameter)
        {
            if (isRightClick != true) return;//

            if (CurrentModel == null)
            {
                MessageBox.Show("请选择或者新建一个模板配置");
                return;
            }
            try
            {
                if (MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    for (int i = 0; i < CurrentModel.RefineUserRegions.Count; i++)
                    {
                        if (CurrentModel.RefineUserRegions[i].IsSelected)
                        {
                            CurrentModel.RefineUserRegions.RemoveAt(i);
                            i--;

                        }
                        else
                        {
                            CurrentModel.RefineUserRegions[i].Index = i + 1;
                        }
                    }
                    //htWindow.DisplayMultiRegion(RefineRegions, Bond2ModelObject.Image);
                    htWindow.DisplayMultiRegion(RefineRegions);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void ExecuteModifyBond2ModelRegionCommand(object parameter)//
        {
            if (isRightClick != true) return;//
            if (Bond2ModelObject.Image == null || !Bond2ModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }
            if (CurrentModel == null)
            {
                MessageBox.Show("请选择或者新建一个模板配置");
                return;
            }
            if (CurrentModel.Bond2UserRegion == null) return;
            if (SwitchImageComboBoxIndex != 1)
            {
                MessageBox.Show("请切换到转正图再进行操作");
                return;
            }
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    switch (CurrentModel.Bond2UserRegion.RegionType)
                    {
                        case RegionType.Point:
                            break;

                        case RegionType.Rectangle1:
                            htWindow.InitialHWindowUpdate(Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[0]),
                                                  Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[1]),
                                                  Math.Ceiling(CurrentModel.Bond2UserRegion.RegionParameters[2]),
                                                  Math.Ceiling(CurrentModel.Bond2UserRegion.RegionParameters[3]),
                                                  CurrentModel.Bond2UserRegion.RegionType,0, 0, "yellow");
                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow,(CurrentModel.Bond2UserRegion.RegionParameters[0]),
                                                                                          (CurrentModel.Bond2UserRegion.RegionParameters[1]),
                                                                                          (CurrentModel.Bond2UserRegion.RegionParameters[2]),
                                                                                          (CurrentModel.Bond2UserRegion.RegionParameters[3]),
                                                                out HTuple row1_Rectangle,
                                                                out HTuple column1_Rectangle,
                                                                out HTuple row2_Rectangle,
                                                                out HTuple column2_Rectangle);

                            CurrentModel.Bond2UserRegion.RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, CurrentModel.Bond2UserRegion.RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle);
                            if (userRegion == null) return;
                            CurrentModel.Bond2UserRegion = userRegion;
                            htWindow.DisplayMultiRegionTwo(CurrentModel.Bond2UserRegion.CalculateRegion, CurrentModel.Bond2UserRegionDiff?.CalculateRegion);
                            break;

                        case RegionType.Rectangle2:
                            htWindow.InitialHWindowUpdate(Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[0]),
                                                          Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[1]),
                                                          Math.Ceiling(CurrentModel.Bond2UserRegion.RegionParameters[2]),
                                                          Math.Ceiling(CurrentModel.Bond2UserRegion.RegionParameters[3]),
                                                          CurrentModel.Bond2UserRegion.RegionType,
                                                          0, 0, "yellow");

                            HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                           Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[0]),
                                                           Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[1]),
                                                           CurrentModel.Bond2UserRegion.RegionParameters[2],
                                                           Math.Ceiling(CurrentModel.Bond2UserRegion.RegionParameters[3]),
                                                           Math.Ceiling(CurrentModel.Bond2UserRegion.RegionParameters[4]),
                                                        out HTuple row_Rectangle2,
                                                        out HTuple column_Rectangle2,
                                                        out HTuple phi_Rectangle2,
                                                        out HTuple lenth1_Rectangle2,
                                                        out HTuple lenth2_Rectangle2);

                            CurrentModel.Bond2UserRegion.RegionType = RegionType.Rectangle2;
                            UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, CurrentModel.Bond2UserRegion.RegionType,
                                                                                                 row_Rectangle2, column_Rectangle2,
                                                                                                 lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                 0,
                                                                                                 0,
                                                                                                 phi_Rectangle2);
                            if (userRegion_Rectangle2 == null) return;
                            CurrentModel.Bond2UserRegion = userRegion_Rectangle2;
                            htWindow.DisplayMultiRegion(CurrentModel.Bond2UserRegion.CalculateRegion, CurrentModel.Bond2UserRegionDiff?.CalculateRegion);
                            break;

                        case RegionType.Circle:
                            htWindow.InitialHWindowUpdate((CurrentModel.Bond2UserRegion.RegionParameters[0]),
                              (CurrentModel.Bond2UserRegion.RegionParameters[1]),
                              (CurrentModel.Bond2UserRegion.RegionParameters[2]),
                              0,CurrentModel.Bond2UserRegion.RegionType,0, 0, "yellow");

                            HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                       CurrentModel.Bond2UserRegion.RegionParameters[0] - 0,
                                                       CurrentModel.Bond2UserRegion.RegionParameters[1] - 0,
                                                       CurrentModel.Bond2UserRegion.RegionParameters[2],
                                                   out HTuple row_Circle,
                                                   out HTuple column_Circle,
                                                   out HTuple radius_Circle);

                            CurrentModel.Bond2UserRegion.RegionType = RegionType.Circle;
                            UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                             CurrentModel.Bond2UserRegion.RegionType,
                                                                                             row_Circle, column_Circle,radius_Circle, 0,0,0,0);
                            if (userRegion_Circle == null) return;
                            CurrentModel.Bond2UserRegion = userRegion_Circle;
                            htWindow.DisplayMultiRegionTwo(CurrentModel.Bond2UserRegion.CalculateRegion, CurrentModel.Bond2UserRegionDiff?.CalculateRegion);
                            break;

                        case RegionType.Ellipse:
                            htWindow.InitialHWindowUpdate(Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[0]),
                                                          Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[1]),
                                                          CurrentModel.Bond2UserRegion.RegionParameters[2],
                                                          Math.Ceiling(CurrentModel.Bond2UserRegion.RegionParameters[3]),
                                                           CurrentModel.Bond2UserRegion.RegionType, 0, 0, "yellow");

                            HOperatorSet.DrawEllipseMod(htWindow.hTWindow.HalconWindow,
                                                       Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[0] - 0),
                                                       Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[1] - 0),
                                                       CurrentModel.Bond2UserRegion.RegionParameters[2],
                                                       Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[3] - 0),
                                                       Math.Floor(CurrentModel.Bond2UserRegion.RegionParameters[4] - 0),
                                                   out HTuple row1,
                                                   out HTuple column1,
                                                   out HTuple phi,
                                                   out HTuple radius1,
                                                   out HTuple radius2);

                            CurrentModel.Bond2UserRegion.RegionType = RegionType.Ellipse;
                            UserRegion userRegion_Ellipse = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                             CurrentModel.Bond2UserRegion.RegionType,
                                                                                             row1, column1, radius1, radius2, 0, 0, phi);
                            if (userRegion_Ellipse == null) return;
                            CurrentModel.Bond2UserRegion = userRegion_Ellipse;
                            htWindow.DisplayMultiRegionTwo(CurrentModel.Bond2UserRegion.CalculateRegion, CurrentModel.Bond2UserRegionDiff?.CalculateRegion);
                            break;

                        default: break;
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

        private void ExecuteModifyBond2ModelRegionDiffCommand(object parameter)
        {
            if (isRightClick != true) return;//
            if (Bond2ModelObject.Image == null || !Bond2ModelObject.Image.IsInitialized())
            {
                MessageBox.Show("请加载图像");
                return;
            }
            if (CurrentModel == null)
            {
                MessageBox.Show("请选择或者新建一个模板配置");
                return;
            }
            if (CurrentModel.Bond2UserRegionDiff == null) return;
            if (SwitchImageComboBoxIndex != 1)
            {
                MessageBox.Show("请切换到转正图再进行操作");
                return;
            }
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    switch (CurrentModel.Bond2UserRegionDiff.RegionType)
                    {
                        case RegionType.Point:
                            break;

                        case RegionType.Rectangle1:
                            htWindow.InitialHWindowUpdate(Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[0]),
                                                          Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[1]),
                                                          Math.Ceiling(CurrentModel.Bond2UserRegionDiff.RegionParameters[2]),
                                                          Math.Ceiling(CurrentModel.Bond2UserRegionDiff.RegionParameters[3]),
                                                          CurrentModel.Bond2UserRegionDiff.RegionType, 0, 0, "yellow");
                            HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, 
                                                           (CurrentModel.Bond2UserRegionDiff.RegionParameters[0]),
                                                           (CurrentModel.Bond2UserRegionDiff.RegionParameters[1]),
                                                           (CurrentModel.Bond2UserRegionDiff.RegionParameters[2]),
                                                           (CurrentModel.Bond2UserRegionDiff.RegionParameters[3]),
                                                       out HTuple row1_Rectangle,
                                                       out HTuple column1_Rectangle,
                                                       out HTuple row2_Rectangle,
                                                       out HTuple column2_Rectangle);

                            CurrentModel.Bond2UserRegionDiff.RegionType = RegionType.Rectangle1;
                            UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, CurrentModel.Bond2UserRegionDiff.RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle);
                            if (userRegion == null) return;
                            userRegion.IsAccept = false;
                            CurrentModel.Bond2UserRegionDiff = userRegion;
                            htWindow.DisplayMultiRegionTwo(CurrentModel.Bond2UserRegion?.CalculateRegion, CurrentModel.Bond2UserRegionDiff.CalculateRegion);
                            break;

                        case RegionType.Rectangle2:
                            htWindow.InitialHWindowUpdate(Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[0]),
                                                          Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[1]),
                                                          Math.Ceiling(CurrentModel.Bond2UserRegionDiff.RegionParameters[2]),
                                                          Math.Ceiling(CurrentModel.Bond2UserRegionDiff.RegionParameters[3]),
                                                          CurrentModel.Bond2UserRegionDiff.RegionType,
                                                          0, 0, "yellow");

                            HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                           Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[0]),
                                                           Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[1]),
                                                           CurrentModel.Bond2UserRegionDiff.RegionParameters[2],
                                                           Math.Ceiling(CurrentModel.Bond2UserRegionDiff.RegionParameters[3]),
                                                           Math.Ceiling(CurrentModel.Bond2UserRegionDiff.RegionParameters[4]),
                                                        out HTuple row_Rectangle2,
                                                        out HTuple column_Rectangle2,
                                                        out HTuple phi_Rectangle2,
                                                        out HTuple lenth1_Rectangle2,
                                                        out HTuple lenth2_Rectangle2);

                            CurrentModel.Bond2UserRegionDiff.RegionType = RegionType.Rectangle2;
                            UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, CurrentModel.Bond2UserRegionDiff.RegionType,
                                                                                                 row_Rectangle2, column_Rectangle2,
                                                                                                 lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                 0,
                                                                                                 0,
                                                                                                 phi_Rectangle2);
                            if (userRegion_Rectangle2 == null) return;
                            CurrentModel.Bond2UserRegionDiff = userRegion_Rectangle2;
                            htWindow.DisplayMultiRegion(CurrentModel.Bond2UserRegion.CalculateRegion, CurrentModel.Bond2UserRegionDiff?.CalculateRegion);
                            break;

                        case RegionType.Circle:
                            htWindow.InitialHWindowUpdate((CurrentModel.Bond2UserRegionDiff.RegionParameters[0]),
                               (CurrentModel.Bond2UserRegionDiff.RegionParameters[1]),
                               (CurrentModel.Bond2UserRegionDiff.RegionParameters[2]),
                               0, CurrentModel.Bond2UserRegionDiff.RegionType, 0, 0, "yellow");

                            HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                      (CurrentModel.Bond2UserRegionDiff.RegionParameters[0] - 0),
                                                      (CurrentModel.Bond2UserRegionDiff.RegionParameters[1] - 0),
                                                       CurrentModel.Bond2UserRegionDiff.RegionParameters[2],
                                                   out HTuple row_Circle,
                                                   out HTuple column_Circle,
                                                   out HTuple radius_Circle);

                            CurrentModel.Bond2UserRegionDiff.RegionType = RegionType.Circle;
                            UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                             CurrentModel.Bond2UserRegionDiff.RegionType,
                                                                                             row_Circle, column_Circle, radius_Circle, 0, 0, 0, 0);
                            if (userRegion_Circle == null) return;
                            userRegion_Circle.IsAccept = false;
                            CurrentModel.Bond2UserRegionDiff = userRegion_Circle;
                            htWindow.DisplayMultiRegionTwo(CurrentModel?.Bond2UserRegion.CalculateRegion, CurrentModel.Bond2UserRegionDiff.CalculateRegion);
                            break;

                        case RegionType.Ellipse:

                            htWindow.InitialHWindowUpdate(Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[0]),
                                                          Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[1]),
                                                          CurrentModel.Bond2UserRegionDiff.RegionParameters[2],
                                                          Math.Ceiling(CurrentModel.Bond2UserRegionDiff.RegionParameters[3]),
                                                           CurrentModel.Bond2UserRegionDiff.RegionType, 0, 0, "yellow");

                            HOperatorSet.DrawEllipseMod(htWindow.hTWindow.HalconWindow,
                                                       Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[0] - 0),
                                                       Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[1] - 0),
                                                       CurrentModel.Bond2UserRegionDiff.RegionParameters[2],
                                                       Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[3] - 0),
                                                       Math.Floor(CurrentModel.Bond2UserRegionDiff.RegionParameters[4] - 0),
                                                   out HTuple row1,
                                                   out HTuple column1,
                                                   out HTuple phi,
                                                   out HTuple radius1,
                                                   out HTuple radius2);

                            CurrentModel.Bond2UserRegionDiff.RegionType = RegionType.Ellipse;
                            UserRegion userRegion_Ellipse = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                             CurrentModel.Bond2UserRegionDiff.RegionType,
                                                                                             row1, column1, radius1, radius2, 0, 0, phi);
                            if (userRegion_Ellipse == null) return;
                            CurrentModel.Bond2UserRegionDiff = userRegion_Ellipse;
                            htWindow.DisplayMultiRegionTwo(CurrentModel.Bond2UserRegion?.CalculateRegion, CurrentModel.Bond2UserRegionDiff.CalculateRegion);
                            break;

                        default: break;
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

        private void ExecuteModifyRegionCommand(object parameter)//
        {
            if (isRightClick != true) return;//
            try
            {
                if (CurrentModel.RefineUserRegions == null) return;
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
                    for (int i = 0; i < CurrentModel.RefineUserRegions.Count; i++)
                    {
                        if (CurrentModel.RefineUserRegions[i].IsSelected)
                        {
                            switch (CurrentModel.RefineUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[0]),
                                                  Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[1]),
                                                  Math.Ceiling(CurrentModel.RefineUserRegions[i].RegionParameters[2]),
                                                  Math.Ceiling(CurrentModel.RefineUserRegions[i].RegionParameters[3]),
                                                  CurrentModel.RefineUserRegions[i].RegionType, 0, 0, "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow,  (CurrentModel.RefineUserRegions[i].RegionParameters[0]),
                                                                                                    (CurrentModel.RefineUserRegions[i].RegionParameters[1]),
                                                                                                    (CurrentModel.RefineUserRegions[i].RegionParameters[2]),
                                                                                                    (CurrentModel.RefineUserRegions[i].RegionParameters[3]),
                                                                out HTuple row1_Rectangle,
                                                                out HTuple column1_Rectangle,
                                                                out HTuple row2_Rectangle,
                                                                out HTuple column2_Rectangle);

                                    CurrentModel.RefineUserRegions[i].RegionType = RegionType.Rectangle1;
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, CurrentModel.RefineUserRegions[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle);
                                    if (userRegion == null) return;
                                    CurrentModel.RefineUserRegions[i] = userRegion;
                                    htWindow.DisplayMultiRegion(RefineRegions);
                                    CurrentModel.RefineUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:
                                    htWindow.InitialHWindowUpdate(Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[1]),
                                                                  Math.Ceiling(CurrentModel.RefineUserRegions[i].RegionParameters[2]),
                                                                  Math.Ceiling(CurrentModel.RefineUserRegions[i].RegionParameters[3]),
                                                                  CurrentModel.RefineUserRegions[i].RegionType,
                                                                  0, 0, "yellow");

                                    HOperatorSet.DrawRectangle2Mod(htWindow.hTWindow.HalconWindow,
                                                                   Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[0]),
                                                                   Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[1]),
                                                                   CurrentModel.RefineUserRegions[i].RegionParameters[2],
                                                                   Math.Ceiling(CurrentModel.RefineUserRegions[i].RegionParameters[3]),
                                                                   Math.Ceiling(CurrentModel.RefineUserRegions[i].RegionParameters[4]),
                                                                out HTuple row_Rectangle2,
                                                                out HTuple column_Rectangle2,
                                                                out HTuple phi_Rectangle2,
                                                                out HTuple lenth1_Rectangle2,
                                                                out HTuple lenth2_Rectangle2);

                                    CurrentModel.RefineUserRegions[i].RegionType = RegionType.Rectangle2;
                                    UserRegion userRegion_Rectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, CurrentModel.RefineUserRegions[i].RegionType,
                                                                                                         row_Rectangle2, column_Rectangle2,
                                                                                                         lenth1_Rectangle2, lenth2_Rectangle2,
                                                                                                         0,
                                                                                                         0,
                                                                                                         phi_Rectangle2);
                                    if (userRegion_Rectangle2 == null) return;
                                    CurrentModel.RefineUserRegions[i] = userRegion_Rectangle2;
                                    htWindow.DisplayMultiRegion(RefineRegions);
                                    CurrentModel.RefineUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Circle:
                                     htWindow.InitialHWindowUpdate((CurrentModel.RefineUserRegions[i].RegionParameters[0]),
                                      (CurrentModel.RefineUserRegions[i].RegionParameters[1]),
                                      (CurrentModel.RefineUserRegions[i].RegionParameters[2]),
                                      0, CurrentModel.RefineUserRegions[i].RegionType, 0, 0, "yellow");

                                    HOperatorSet.DrawCircleMod(htWindow.hTWindow.HalconWindow,
                                                               (CurrentModel.RefineUserRegions[i].RegionParameters[0] - 0),
                                                               (CurrentModel.RefineUserRegions[i].RegionParameters[1] - 0),
                                                               CurrentModel.RefineUserRegions[i].RegionParameters[2],
                                                           out HTuple row_Circle,
                                                           out HTuple column_Circle,
                                                           out HTuple radius_Circle);

                                    CurrentModel.RefineUserRegions[i].RegionType = RegionType.Circle;
                                    UserRegion userRegion_Circle = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     CurrentModel.RefineUserRegions[i].RegionType,
                                                                                                     row_Circle, column_Circle, radius_Circle, 0, 0, 0, 0);
                                    if (userRegion_Circle == null) return;
                                    userRegion_Circle.IsAccept = false;
                                    CurrentModel.RefineUserRegions[i] = userRegion_Circle;
                                    htWindow.DisplayMultiRegion(RefineRegions);
                                    CurrentModel.RefineUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Ellipse:

                                    htWindow.InitialHWindowUpdate(Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[0]),
                                                                  Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[1]),
                                                                  CurrentModel.RefineUserRegions[i].RegionParameters[2],
                                                                  Math.Ceiling(CurrentModel.RefineUserRegions[i].RegionParameters[3]),
                                                                   CurrentModel.RefineUserRegions[i].RegionType, 0, 0, "yellow");

                                    HOperatorSet.DrawEllipseMod(htWindow.hTWindow.HalconWindow,
                                                               Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[0] - 0),
                                                               Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[1] - 0),
                                                               CurrentModel.RefineUserRegions[i].RegionParameters[2],
                                                               Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[3] - 0),
                                                               Math.Floor(CurrentModel.RefineUserRegions[i].RegionParameters[4] - 0),
                                                           out HTuple row1,
                                                           out HTuple column1,
                                                           out HTuple phi,
                                                           out HTuple radius1,
                                                           out HTuple radius2);

                                    CurrentModel.RefineUserRegions[i].RegionType = RegionType.Ellipse;
                                    UserRegion userRegion_Ellipse = UserRegion.GetHWindowRegionUpdate(htWindow,
                                                                                                     CurrentModel.RefineUserRegions[i].RegionType,
                                                                                                     row1, column1, radius1, radius2, 0, 0, phi);
                                    if (userRegion_Ellipse == null) return;
                                    CurrentModel.RefineUserRegions[i] = userRegion_Ellipse;
                                    htWindow.DisplayMultiRegion(RefineRegions);
                                    CurrentModel.RefineUserRegions[i].Index = i + 1;
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

        //创建第二焊点模板
        private void ExecuteCreateBond2ModelCommand(object parameter)
        {
            if (isRightClick != true) return;//
            if (Bond2ModelObject.Image == null || !Bond2ModelObject.Image.IsInitialized())
            {
                MessageBox.Show("没有参考图像!");
                return;
            }
            if (Bond2ModelParameter.ImageCountChannels > 0 && Bond2ModelParameter.ImageChannelIndex < 0)
            {
                MessageBox.Show("请先选择通道图像！");
                return;
            }
            //Window_Loading window_Loading = new Window_Loading("正在生成焊点模板");
            //window_Loading.Show();
            try
            {
                HOperatorSet.GenEmptyObj(out HObject rotatedImages);
                HOperatorSet.GenEmptyObj(out HObject bond2UserRegionCalculation);
                HOperatorSet.GenEmptyObj(out HObject refineRegions);
                //HTuple ModelType = new HTuple();
                //HTuple IsPreProcess = new HTuple();
                //HTuple Gamma = new HTuple();

                foreach (var model in Models)
                {
                    if (model.Bond2UserRegion == null || model.Bond2UserRegion.CalculateRegion == null || !model.Bond2UserRegion.CalculateRegion.IsInitialized())
                    {
                        MessageBox.Show($"序号 {model.Index.ToString()} 的模板配置的焊点模板区域为空，请在转正图中选择");
                        return;
                    }
                    if (!File.Exists($"{bondRecipeDirectory}RotatedImage{model.Index.ToString()}.tiff")) 
                    {
                        MessageBox.Show($"序号 {model.Index.ToString()} 的模板配置的转正图不存在");
                        return;
                    }
                    HOperatorSet.ReadImage(out HObject rotatedImage, $"{bondRecipeDirectory}RotatedImage{model.Index.ToString()}.tiff");
                    HOperatorSet.ConcatObj(rotatedImages, rotatedImage, out rotatedImages);

                    if (model.Bond2UserRegionDiff!=null)
                    {
                        HOperatorSet.ConcatObj(bond2UserRegionCalculation, Algorithm.Region.ConcatRegion(model.Bond2UserRegion, model.Bond2UserRegionDiff), out bond2UserRegionCalculation);
                    }
                    else
                    {
                        HOperatorSet.ConcatObj(bond2UserRegionCalculation, model.Bond2UserRegion.CalculateRegion, out bond2UserRegionCalculation);
                    }
                    HOperatorSet.ConcatObj(refineRegions, Algorithm.Region.ConcatRegion(model.RefineUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion)), out refineRegions);//DisplayRegion
                    //HOperatorSet.TupleConcat(IsPreProcess, new HTuple(model.IsPreProcess), out IsPreProcess);//
                    //HOperatorSet.TupleConcat(Gamma, model.Gamma, out Gamma);//
                }

                bond2ModelImage?.Dispose();
                bond2ModelImage = null;

                //接口替换
                Algorithm.Model_RegionAlg.HTV_GenBond2Model_Recipe(Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, Bond2ModelParameter.ImageChannelIndex),
                                                         rotatedImages,//concact
                                                         bond2UserRegionCalculation,//concact
                                                         refineRegions,//concact
                                                         Bond2ModelParameter.ModelType,//
                                                         ModelsCount,
                                                         Bond2ModelParameter.IsPreProcess==false?0:1,//
                                                         Bond2ModelParameter.Gamma,//
                                                     out HTuple _modelId);          

                /*
                Algorithm.Model.HTV_GenBond2Model_Recipe(Bond2ModelObject.DieImages,
                                    RotateImages[i],
                                    Models[i].Bond2UserRegion.CalculateRegion,
                                    refineRegions[i],
                                    Models[i].ImageIndex,//012
                                    Models[i].ModelType,
                                    new HTuple(Models[i].AngleStart).TupleRad(),
                                    new HTuple(Models[i].AngleExt).TupleRad(),
                                    Models[i].IsPreProcess ? 1 : 0,
                                    Models[i].Gamma,
                                out HTuple _modelId);
                */

                //保存并且显示第二焊点模板图像
                //CurrentModel.Bond2ModelImagePath = $"{bondRecipeDirectory}\\bond2ModelImagebefore{CurrentModel.Index.ToString()}.tiff";//
                //HOperatorSet.WriteImage(bond2ModelImage, "tiff", 0, CurrentModel.Bond2ModelImagePath);

                //CurrentModel.Bond2ModelImagePath = $"{bondRecipeDirectory}\\bond2ModelImage{CurrentModel.Index.ToString()}.tiff";//
                //HOperatorSet.CropDomain(bond2ModelImage, out bond2ModelImage);
                //HOperatorSet.WriteImage(bond2ModelImage, "tiff", 0, CurrentModel.Bond2ModelImagePath);

                //HOperatorSet.GetImageSize(bond2ModelImage, out imgWidth, out imgHeight);
                //setImagePart((Content as Page_CreateBond2Model).hTWindow5, 0, 0, imgHeight, imgWidth);
                //(Content as Page_CreateBond2Model).hTWindow5.HalconWindow.ClearWindow();
                //(Content as Page_CreateBond2Model).hTWindow5.HalconWindow.DispObj(bond2ModelImage);

                //CurrentModel.RotatedImagePath = $"{bondRecipeDirectory}\\RotatedImage{CurrentModel.Index.ToString()}.tiff"; //RotatedImage
                //HOperatorSet.WriteImage(rotatedImage, "tiff", 0, CurrentModel.RotatedImagePath);

                //CurrentModel.Bond2ModelImagePath =FilePath.EnsureDirectoryExisted($"{bondRecipeDirectory}\\bond2ModelImage" + i + ".tiff");//不对

                /*
                //Algorithm.File.SaveModel($"{PathBondWire}\\Model_" + i + ".dat", Models[i].ModelType, _modelId);//
                //临时使用 与之前算法模板保存一致
                HOperatorSet.WriteTuple(new HTuple(Models[i].ImageIndex), $"{bondRecipeDirectory}\\Bond2_ModelImgIdx_" + i + ".tup");
                //HOperatorSet.WriteTuple(new HTuple(Models[i].ImageIndex), $"{PathBondWire}\\Bond2_ModelImgIdx_" + i + ".tup");//
                if (Models[i].ModelType == 0)
                {
                    HOperatorSet.WriteTuple(new HTuple("ncc"), $"{bondRecipeDirectory}\\Model_Type_" + i + ".tup");
                }
                else
                {
                    HOperatorSet.WriteTuple(new HTuple("shape"), $"{bondRecipeDirectory}\\Model_Type_" + i + ".tup");
                }
                */
                //modelId.Append(_modelId); 原来有
                //}

                Bond2ModelObject.ModelID = _modelId;

                if (Bond2ModelObject.ModelID.TupleLength() > 1)
                {
                    String[] ModelIdPathArry = new string[_modelId.TupleLength()];
                    for (int i = 0; i < _modelId.TupleLength(); i++)
                    {
                        ModelIdPathArry[i] = $"{bondRecipeDirectory}PosModel_" + i + ".dat";
                        Models[i].ModelIdPath = ModelIdPathArry[i];
                        Algorithm.File.SaveModel(Models[i].ModelIdPath, Bond2ModelParameter.ModelType, Bond2ModelObject.ModelID[i]);//Bond2ModelParameter.ModelType               
                    }
                    Bond2ModelParameter.ModelIdPath = String.Join(",", ModelIdPathArry);
                    //window_Loading.Close();
                    MessageBox.Show("焊点模板已生成");
                }
                else if(Bond2ModelObject.ModelID.TupleLength() == 1)
                {
                    Models[0].ModelIdPath = $"{bondRecipeDirectory}PosModel" + ".dat";
                    Algorithm.File.SaveModel(Models[0].ModelIdPath, Bond2ModelParameter.ModelType, Bond2ModelObject.ModelID[0]);
                    Bond2ModelParameter.ModelIdPath = Models[0].ModelIdPath;
                    //window_Loading.Close();
                    MessageBox.Show("焊点模板已生成");
                }
            }
            catch (Exception ex)
            {
                //window_Loading.Close();
                MessageBox.Show(ex.ToString());
            }
        }
       
        private void SwitchToImage()
        {
            SwitchImageComboBoxIndex = 0;
            OnPropertyChanged("SwitchImageComboBoxIndex");
        }

        private void SwitchToRotatedImage()
        {
            SwitchImageComboBoxIndex = 1;
            OnPropertyChanged("SwitchImageComboBoxIndex");
        }

        //保存焊点模板到Models文件夹
        private void ExecuteSaveBond2ModelCommand(object parameter)//
        {
            if (isRightClick != true) return;
            if (Bond2ModelObject.ModelID==null)
            {
                MessageBox.Show("请先创建焊点模板！");
                return;
            }
            try
            {
                if (Bond2ModelObject.ModelID?.TupleLength() > 1)
                {
                    String[] ModelIdPathArry = new string[Bond2ModelObject.ModelID.TupleLength()];
                    for (int i = 0; i < Bond2ModelObject.ModelID.TupleLength(); i++)
                    {
                        ModelIdPathArry[i] = $"{ModelsDirectory}PosModel_" + i + ".dat";
                        Algorithm.File.SaveModel(ModelIdPathArry[i], Bond2ModelParameter.ModelType, Bond2ModelObject.ModelID[i]);
                    }
                }
                else
                {
                    Algorithm.File.SaveModel($"{ModelsDirectory}PosModel" + ".dat", Bond2ModelParameter.ModelType, Bond2ModelObject.ModelID[0]);
                }
                MessageBox.Show("模板保存成功！");              
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                MessageBox.Show("请先创建焊点模板！");
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
            htWindow.DisplayMultiRegion(RefineRegions, Algorithm.Region.GetChannnelImageUpdate(Bond2ModelObject.Image, Bond2ModelParameter.ImageChannelIndex));
            SwitchToImage();
            Bond2ModelParameter.IsPickUp = false;
        }

        public void Dispose()
        {
            (Content as Page_CreateBond2Model).DataContext = null;
            (Content as Page_CreateBond2Model).Close();
            Content = null;            
            this.htWindow = null;
            Bond2ModelObject = null;
            Models = null;
            Bond2ModelParameter = null;
            AddModelCommand = null;
            RemoveModelCommand = null;
            RotateCommand = null;
            AddBond2UserRegionCommand = null;//
            AddBond2UserRegionDiffCommand = null;//
            ModifyRegionCommand = null;//
            ModifyBond2ModelRegionCommand = null;//
            AddRefineUserRegionCommand = null;
            RemoveRefineUserRegionCommand = null;
            CreateBond2ModelCommand = null;
            UserRegionEnableChangedCommand = null;
            rotatedImage = null;
            bond2ModelImage = null;
        }
    }
}
