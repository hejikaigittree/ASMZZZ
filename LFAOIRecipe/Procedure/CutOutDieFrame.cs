using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HalconDotNet;
using System.IO;
using System.Windows;

namespace LFAOIRecipe
{
    class CutOutDieFrame : ViewModelBase, IProcedure
    {
        //1121
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public static bool isRightClick = true;//

        private int imageIndex;
        /// <summary>
        /// 切换原图或通道图
        /// </summary>
        public int ImageIndex
        {
            get => imageIndex;
            set
            {
                // 1122
                if (imageIndex != value)
                {
                    if (value == -1)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.Image, out HObject ChannelImage0, GoldenModelParameter.ImageChannelIndex + 1);
                        goldenModelObject.ChannelImage = ChannelImage0;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 0)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.Image, out HObject ChannelImage1, 1);
                        goldenModelObject.ChannelImage = ChannelImage1;
                        GoldenModelParameter.ImageChannelIndex = 0;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 1)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.Image, out HObject ChannelImage2, 2);
                        goldenModelObject.ChannelImage = ChannelImage2;
                        GoldenModelParameter.ImageChannelIndex = 1;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 2)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.Image, out HObject ChannelImage3, 3);
                        goldenModelObject.ChannelImage = ChannelImage3;
                        GoldenModelParameter.ImageChannelIndex = 2;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else if (value == 3)
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.Image, out HObject ChannelImage4, 4);
                        goldenModelObject.ChannelImage = ChannelImage4;
                        GoldenModelParameter.ImageChannelIndex = 3;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    else
                    {
                        HOperatorSet.AccessChannel(goldenModelObject.Image, out HObject ChannelImagen, value+1);
                        goldenModelObject.ChannelImage = ChannelImagen;
                        GoldenModelParameter.ImageChannelIndex = value;
                        htWindow.Display(goldenModelObject.ChannelImage);
                    }
                    imageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private string referenceDirectory;
        public string ReferenceDirectory
        {
            get => referenceDirectory;
            set => OnPropertyChanged(ref referenceDirectory, value);
        }

        public GoldenModelParameter GoldenModelParameter { get; private set; }

        public ObservableCollection<UserRegion> DieUserRegions { get; private set; }

        private UserRegion selectedUserRegion;
        public UserRegion SelectedUserRegion
        {
            get => selectedUserRegion;
            set => OnPropertyChanged(ref selectedUserRegion, value);
            /*
            set
            {
                if (selectedUserRegion!=value)
                {
                    selectedUserRegion = value;
                    htWindow.DisplaySingleRegionForCutOut(DieUserRegions,selectedUserRegion,selectedUserRegion.Index, null, "orange");
                }
            }
            */
        }

        private UserRegion userRegionForCutOut;
        public UserRegion UserRegionForCutOut
        {
            get => userRegionForCutOut;
            set => OnPropertyChanged(ref userRegionForCutOut, value);
        }

        private int switchImageComboBoxIndex = -1;
        /// <summary>
        /// 切换整图和Die区域
        /// </summary>
        public int SwitchImageComboBoxIndex
        {
            get => switchImageComboBoxIndex;
            set
            {
                if (switchImageComboBoxIndex != value)
                {
                    if (value == 0)
                    {
                        htWindow.DisplayMultiRegion(DieRegions, goldenModelObject.Image);
                    }
                    else if (value == 1)
                    {
                        htWindow.Display(goldenModelObject.DieImage, true);
                    }
                    switchImageComboBoxIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public event Action OnResetDieRegion;

        private HTHalControlWPF htWindow;

        private GoldenModelObject goldenModelObject;

        private IEnumerable<HObject> DieRegions => DieUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        public CommandBase LoadImageCommand { get; private set; }
        public CommandBase SelectedTrainningImageDirectoryCommand { get; private set; }
        public CommandBase AddUserRegionCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase CutOutCommand { get; private set; }
        public CommandBase LoadXMLCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }//    
        public CommandBase LoadReferenceCommand { get; private set; }//
        public CommandBase SelectReferenceCommand { get; private set; }

        public CutOutDieFrame(HTHalControlWPF htWindow,
                         string ReferenceDirectory,
                         GoldenModelParameter goldenModelParameter,
                         GoldenModelObject goldenModelObject,
                         ObservableCollection<UserRegion> dieUserRegions)
        {
            DisplayName = "搜索区";
            Content = new Page_CutOutDieFrame { DataContext = this };
            this.htWindow = htWindow;
            this.ReferenceDirectory = ReferenceDirectory;
            this.GoldenModelParameter = goldenModelParameter;
            this.goldenModelObject = goldenModelObject;
            this.DieUserRegions = dieUserRegions;

            LoadImageCommand = new CommandBase(ExecuteLoadImageCommand);
            SelectReferenceCommand = new CommandBase(ExecuteSelectReferenceCommand);
            SelectedTrainningImageDirectoryCommand = new CommandBase(ExecuteSelectedTrainningImageDirectoryCommand);
            AddUserRegionCommand = new CommandBase(ExecuteAddUserRegionCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            RemoveUserRegionCommand = new CommandBase(ExecuteRemoveUserRegionCommand);
            CutOutCommand = new CommandBase(ExecuteCutOutCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);//
            LoadReferenceCommand = new CommandBase(ExecuteLoadReferenceCommand);//
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

            GoldenModelParameter.ImagePath = $"{ReferenceDirectory}ReferenceImage.tiff";
            goldenModelObject.Image?.Dispose();
            LoadImage();

            HOperatorSet.ReadTuple(ReferenceDirectory + "TrainningImagesDirectory.tup", out HTuple TrainningImagesDirectoryTemp);
            GoldenModelParameter.TrainningImagesDirectory = TrainningImagesDirectoryTemp;
            GoldenModelParameter.VerifyImagesDirectory = GoldenModelParameter.TrainningImagesDirectory;

            HOperatorSet.ReadRegion(out HObject UserRegionForCutOut_Region, ReferenceDirectory + "DieReference.reg");
            HOperatorSet.ReduceDomain(goldenModelObject.Image, UserRegionForCutOut_Region, out HObject imageReduced);
            HOperatorSet.CropDomain(imageReduced, out HObject dieImage);
            goldenModelObject.DieImage = dieImage;

            HOperatorSet.ReadTuple(ReferenceDirectory + "UserRegionForCutOutIndex.tup", out HTuple UserRegionForCutOutIndexTemp);
            GoldenModelParameter.UserRegionForCutOutIndex = UserRegionForCutOutIndexTemp;

            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageRowOffset.tup", out HTuple dieImageRowOffset);
            GoldenModelParameter.DieImageRowOffset = dieImageRowOffset;
            HOperatorSet.ReadTuple(ReferenceDirectory + "DieImageColumnOffset.tup", out HTuple dieImageColumnOffset);
            GoldenModelParameter.DieImageColumnOffset = dieImageColumnOffset;

            LoadDieImage(false);
            GoldenModelParameter.ReferencePath = ReferenceDirectory;

            //1121 lht
            HOperatorSet.ReadTuple(ReferenceDirectory + "ImageChannelnumber.tup", out HTuple ImageChannelnumber);
            ChannelNames.Clear();
            for (int i = 0; i < ImageChannelnumber; i++)
            {
                ChannelName tmp_name = new ChannelName();
                tmp_name.Name = (i + 1).ToString();
                ChannelNames.Add(tmp_name);
            }
            //1121
            GoldenModelParameter.ChannelNames = ChannelNames;
            //GoldenModelParameter.ImageChannelIndex = 1;
            imageIndex = GoldenModelParameter.ImageChannelIndex;
            OnPropertyChanged("imageIndex");

            //1201 lw
            HOperatorSet.TupleSplit(ReferenceDirectory, "\\", out HTuple hv_subStr);
            HTuple FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 3];
            GoldenModelParameter.CurFovName = FOV_Name;

            //备用
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

            UserRegionForCutOut = DieUserRegions.Where(u => u.Index == GoldenModelParameter.UserRegionForCutOutIndex).FirstOrDefault();
            goldenModelObject.UserRegionForCutOut = UserRegionForCutOut;

            // 1122
            HOperatorSet.AccessChannel(goldenModelObject.Image, out HObject ChannelImageDisply, GoldenModelParameter.ImageChannelIndex + 1);
            htWindow.DisplaySingleRegion(goldenModelObject.UserRegionForCutOut.CalculateRegion, ChannelImageDisply);
        }

        public void LoadImage()
        {
            HObject image;
            HOperatorSet.GenEmptyObj(out image);
            HOperatorSet.ReadImage(out image, GoldenModelParameter.ImagePath);
            htWindow.Display(image, true);
            //SwitchToImage();
            goldenModelObject.Image = image;

            HOperatorSet.CountChannels(goldenModelObject.Image, out HTuple channels);
            GoldenModelParameter.ImageCountChannels = channels;
            goldenModelObject.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.Image, GoldenModelParameter.ImageChannelIndex);
            //goldenModelObject.ImageR = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.Image, 1);
            //goldenModelObject.ImageG = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.Image, 2);
            //goldenModelObject.ImageB = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.Image, 3);
        }

        /*
        public void GenImageListIndexBegin()
        {
            HTuple imageFiles = null;
            Algorithm.File.list_image_files(GoldenModelParameter.TrainningImagesDirectory, "default", "recursive", out imageFiles);
            string[] folderList = imageFiles;
            GoldenModelParameter.CurrentTrainSet = folderList.Count();
            for (int i = 0; i < folderList.Length; i++)
            {
                if (folderList[i].Replace(@"/", @"\") == (GoldenModelParameter.ImagePath))
                {
                    GoldenModelParameter.ImageListIndexBegin = i + 1;//从1起计数
                }
            }
            if (GoldenModelParameter.ImageListIndexBegin == 0)
            {
                MessageBox.Show("加载图像从选择训练文件夹中选择！");
                return;
            }
        }
        */

        /// <summary>
        /// 加载图像
        /// </summary>
        private void ExecuteLoadImageCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    bool isRegionExsited = DieUserRegions.Count > 0;
                    if (isRegionExsited)
                    {
                        if (System.Windows.MessageBox.Show("更换图像将清空现有区域，是否继续？", "", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        {
                            return;
                        }
                    }
                    
                    if (!Directory.Exists(GoldenModelParameter.TrainningImagesDirectory))
                    {
                        MessageBox.Show("请先选择训练图像文件的路径！");
                        return;
                    }
                    
                    using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                    {
                        ofd.Filter = "tiff|*.tiff|tif|*.tif|ima|*.ima|bmp|*.bmp|jpg|*.jpg|png|*.png|gif|*.gif|jpeg|*.jpeg|pcx|*.pcx|pgm|*.pgm|ppm|*.ppm|pbm|*.pbm|xwd|*.xwd|pnm|*.pnm";
                        ofd.Multiselect = false;
                        ofd.InitialDirectory = GoldenModelParameter.TrainningImagesDirectory;
                        if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                        GoldenModelParameter.ImagePath = ofd.FileName;
                        GoldenModelParameter.TrainningImagesDirectory = Directory.GetParent(GoldenModelParameter.ImagePath).ToString();
                        //GenImageListIndexBegin();//
                    }
                    goldenModelObject.Image?.Dispose();
                    LoadImage();
                    if (isRegionExsited)
                    {
                        DieUserRegions.Clear();
                        UserRegionForCutOut = null;
                        goldenModelObject.DieImage = null;
                        OnResetDieRegion?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 选择训练图集
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteSelectedTrainningImageDirectoryCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        if (Directory.Exists(GoldenModelParameter.TrainningImagesDirectory))
                        {
                            fbd.SelectedPath = GoldenModelParameter.TrainningImagesDirectory;
                        }
                        if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            GoldenModelParameter.TrainningImagesDirectory = fbd.SelectedPath;
                        }
                        if (File.Exists(GoldenModelParameter.ImagePath))
                        {
                            if (GoldenModelParameter.TrainningImagesDirectory != Path.GetDirectoryName(GoldenModelParameter.ImagePath))
                            {
                                MessageBox.Show("请修改加载图像文件！");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 添加区域，仅限矩形框
        /// </summary>
        private void ExecuteAddUserRegionCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    if (htWindow.RegionType == RegionType.Null)
                    {
                        System.Windows.MessageBox.Show("没有有效区域，选择区域后请右键点击确认");
                        return;
                    }
                    if (htWindow.RegionType != RegionType.Rectangle1)
                    {
                        MessageBox.Show("请使用矩形框选区域");
                        return;
                    }
                    UserRegion userRegion = UserRegion.GetHWindowRegion(htWindow);
                    if (userRegion == null) return;
                    userRegion.Index = DieUserRegions.Count + 1;
                    DieUserRegions.Add(userRegion);
                    htWindow.DisplayMultiRegion(DieRegions);
                    SwitchToImage();
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
        }

        private void ExecuteRemoveUserRegionCommand(object parameter)
        {
            if (isRightClick)
            {
                if (System.Windows.MessageBox.Show("是否删除选中的区域", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    for (int i = 0; i < DieUserRegions.Count; i++)
                    {
                        if (DieUserRegions[i].IsSelected)
                        {
                            if (DieUserRegions[i] == UserRegionForCutOut)
                            {
                                if (MessageBox.Show("即将删除用于创建Die的区域，删除该区域将清空后续数据。" + System.Environment.NewLine + "点击确定删除该区域；" + System.Environment.NewLine + "点击取消跳过。",
                                                    "", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                                {
                                    continue;
                                }
                                else
                                {
                                    UserRegionForCutOut = null;
                                    goldenModelObject.DieImage = null;
                                    OnResetDieRegion?.Invoke();
                                }
                            }
                            DieUserRegions.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            DieUserRegions[i].Index = i + 1;
                        }
                    }
                    htWindow.DisplayMultiRegion(DieRegions, goldenModelObject.Image);
                    SwitchToImage();
                }
            }
        }

        //修改区域
        private void ExecuteModifyRegionCommand(object parameter)//
        {
            if (isRightClick)
            {
                isRightClick = false;
                try
                {
                    for (int i = 0; i < DieUserRegions.Count; i++)
                    {
                        if (DieUserRegions[i].IsSelected)
                        {
                            if (DieUserRegions[i] == UserRegionForCutOut)
                            {
                                MessageBox.Show("剪裁区域不可修改");
                                return;
                            }
                            switch (DieUserRegions[i].RegionType)
                            {
                                case RegionType.Point:
                                    break;

                                case RegionType.Rectangle1:
                                    htWindow.InitialHWindowUpdate(Math.Floor(DieUserRegions[i].RegionParameters[0]),
                                                  Math.Floor(DieUserRegions[i].RegionParameters[1]),
                                                  Math.Ceiling(DieUserRegions[i].RegionParameters[2]),
                                                  Math.Ceiling(DieUserRegions[i].RegionParameters[3]),
                                                  DieUserRegions[i].RegionType, 0, 0, "yellow");
                                    HOperatorSet.DrawRectangle1Mod(htWindow.hTWindow.HalconWindow, (DieUserRegions[i].RegionParameters[0]),
                                                                                            (DieUserRegions[i].RegionParameters[1]),
                                                                                            (DieUserRegions[i].RegionParameters[2]),
                                                                                            (DieUserRegions[i].RegionParameters[3]),
                                                        out HTuple row1_Rectangle,
                                                        out HTuple column1_Rectangle,
                                                        out HTuple row2_Rectangle,
                                                        out HTuple column2_Rectangle);

                                    DieUserRegions[i].RegionType = RegionType.Rectangle1;
                                    //Die图
                                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, DieUserRegions[i].RegionType, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle);
                                    if (userRegion == null) return;
                                    DieUserRegions[i] = userRegion;
                                    htWindow.DisplayMultiRegion(DieRegions);//ok
                                    DieUserRegions[i].Index = i + 1;
                                    break;

                                case RegionType.Rectangle2:
                                    break;

                                case RegionType.Circle:
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
                    System.Windows.MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    isRightClick = true;
                }
            }
        }

        private void ExecuteUserRegionEnableChangedCommand(object parameter)
        {
            if (isRightClick)
            {
                UserRegion userRegion = parameter as UserRegion;
                if (userRegion == null) return;
                if (userRegion == SelectedUserRegion && !userRegion.IsEnable)
                {
                    if (System.Windows.MessageBox.Show("该区域用于创建Die，禁用它将清空后续数据。" + System.Environment.NewLine + "点击确定禁用该区域并删除数据；" + System.Environment.NewLine + "点击取消放弃操作。",
                        "", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        userRegion.IsEnable = true;
                        return;
                    }
                    else
                    {
                        UserRegionForCutOut = null;
                        goldenModelObject.DieImage = null;
                        OnResetDieRegion?.Invoke();
                    }
                }
                htWindow.DisplayMultiRegion(DieRegions, goldenModelObject.Image);
                SwitchToImage();
            }
        }

        /// <summary>
        /// 裁剪Die区域，切换到Die区域显示，进行通道分离
        /// </summary>
        private void ExecuteCutOutCommand(object parameter)
        {
            if (isRightClick)
            {
                if (SelectedUserRegion == null || DieUserRegions.Count == 0
                                               || SelectedUserRegion.DisplayRegion == null
                                               || !SelectedUserRegion.DisplayRegion.IsInitialized())
                {
                    System.Windows.MessageBox.Show("Die区域不存在，请重新选择");
                    return;
                }
                if (!SelectedUserRegion.IsEnable)
                {
                    System.Windows.MessageBox.Show("所选区域被禁用，请先启用");
                    return;
                }
                UserRegionForCutOut = SelectedUserRegion;
                goldenModelObject.UserRegionForCutOut = UserRegionForCutOut;//
                LoadDieImage(true);
            }
        }

        public bool LoadDieImage(bool IsShowDieImage)
        {
            
            //HObject imageReduced = null;
            //HOperatorSet.GenEmptyObj(out imageReduced);
            //goldenModelObject.DieImage?.Dispose();
            try
            {
                //SelectedUserRegion = userRegionForCutOut;
                //HOperatorSet.ReduceDomain(goldenModelObject.Image, UserRegionForCutOut.DisplayRegion, out imageReduced);
                //HOperatorSet.CropDomain(imageReduced, out HObject dieImage);
                //goldenModelObject.DieImage = dieImage;
                //imageReduced.Dispose();
                //if (IsShowDieImage)
                //{
                    //htWindow.Display(goldenModelObject.DieImage, IsShowDieImage);
                    //SwitchToDieImage();
                //}

                goldenModelObject.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, GoldenModelParameter.ImageChannelIndex);
                //goldenModelObject.DieImageR = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, 1);
                //goldenModelObject.DieImageG = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, 2);
                //goldenModelObject.DieImageB = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.DieImage, 3);

                //存储偏离值
                //goldenModelObject.DieImageRowOffset = UserRegionForCutOut.RegionParameters[0];
                //goldenModelObject.DieImageColumnOffset = UserRegionForCutOut.RegionParameters[1];
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        //选择参考路径
        private void ExecuteSelectReferenceCommand(object parameter)//
        {
            if (isRightClick)
            {
                try
                {
                    using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            ReferenceDirectory = $"{fbd.SelectedPath}\\";
                            GoldenModelParameter.ReferencePath = ReferenceDirectory;
                        }
                        else
                        {
                            GoldenModelParameter.ReferencePath = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    goldenModelObject.Image?.Dispose();
                    GoldenModelParameter.ImagePath = null;
                    GoldenModelParameter.TrainningImagesDirectory = null;
                    GoldenModelParameter.UserRegionForCutOutIndex = 0;
                    goldenModelObject.UserRegionForCutOut = null;
                    GoldenModelParameter.DieImageRowOffset = 0;
                    GoldenModelParameter.DieImageColumnOffset = 0;
                    GoldenModelParameter.ReferencePath = null;
                }
            }
        }

        public bool CheckCompleted()
        {
            /*
            if (string.IsNullOrEmpty(GoldenModelParameter.ImagePath) || !File.Exists(GoldenModelParameter.ImagePath))
            {
                System.Windows.MessageBox.Show("图片不存在，请重新选择");
                return false;
            }
            if (string.IsNullOrEmpty(GoldenModelParameter.TrainningImagesDirectory) || !Directory.Exists(GoldenModelParameter.TrainningImagesDirectory))
            {
                System.Windows.MessageBox.Show("训练图像文件夹不存在，请重新选择");
                return false;
            }
            if (DieUserRegions.Count == 0)
            {
                System.Windows.MessageBox.Show("请选择一个Die区域");
                return false;
            }
            if (goldenModelObject.DieImage == null || !goldenModelObject.DieImage.IsInitialized())
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
            // 1122
            //HOperatorSet.AccessChannel(goldenModelObject.Image, out HObject ChannelImageDisply, GoldenModelParameter.ImageChannelIndex + 1);
            HObject ChannelImageDisply = Algorithm.Region.GetChannnelImageUpdate(goldenModelObject.Image, GoldenModelParameter.ImageChannelIndex);
            htWindow.DisplaySingleRegion(goldenModelObject.UserRegionForCutOut?.CalculateRegion, ChannelImageDisply);
            //SwitchToImage();
        }

        private void SwitchToImage()
        {
            SwitchImageComboBoxIndex = 0;
            OnPropertyChanged("SwitchImageComboBoxIndex");
        }

        private void SwitchToDieImage()
        {
            SwitchImageComboBoxIndex = 1;
            OnPropertyChanged("SwitchImageComboBoxIndex");
        }

        public void Dispose()
        {
            (Content as Page_CutOutDieFrame).DataContext = null;
            (Content as Page_CutOutDieFrame).Close();
            Content = null;
            htWindow = null;
            GoldenModelParameter = null;
            goldenModelObject = null;
            DieUserRegions = null;
            LoadImageCommand = null;
            SelectedTrainningImageDirectoryCommand = null;
            AddUserRegionCommand = null;
            RemoveUserRegionCommand = null;
            CutOutCommand = null;
            LoadXMLCommand = null;
            UserRegionEnableChangedCommand = null;          
        }
    }
}
