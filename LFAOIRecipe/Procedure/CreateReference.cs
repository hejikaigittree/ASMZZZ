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
    class CreateReference : ViewModelBase, IProcedure
    {
        public string DisplayName { get; }

        public object Content { get; private set; }

        public static bool isRightClick = true;

        public IniParameters iniParameters { get; set; }

        private IniObjects iniObjects;

        string regionName = string.Empty;

        private string IniDirectory { get;  set; }

        private string productPath=FilePath.ProductDirectory;
        public string ProductPath
        {
            get => productPath;
            set => OnPropertyChanged(ref productPath, value);
        }

        public HObject SelectedImagechannel { get; set; }
        
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

        /*
        private UserRegion userRegionForCutOut;
        public UserRegion UserRegionForCutOut
        {
            get => userRegionForCutOut;
            set => OnPropertyChanged(ref userRegionForCutOut, value);
        }
        */

        //1120
        private int switchImageChannelIndex = -1;
        /// <summary>
        /// 切换图像通道数
        /// </summary>
        public int SwitchImageChannelIndex
        {
            get => switchImageChannelIndex;
            set
            {
                if (switchImageChannelIndex != value)
                {
                    if (value == 0)
                    {
                        HOperatorSet.AccessChannel(iniObjects.Image, out HObject ChannelImage1, 1);

                        SelectedImagechannel = ChannelImage1;
                        iniObjects.ChannelImage = ChannelImage1;
                        iniParameters.ImageIndex = 0;
                        htWindow.DisplayMultiRegion(DieRegions, ChannelImage1);
                    }
                    else if (value ==1)
                    {
                        HOperatorSet.AccessChannel(iniObjects.Image, out HObject ChannelImage2, 2);
                        SelectedImagechannel = ChannelImage2;
                        iniObjects.ChannelImage = ChannelImage2;
                        iniParameters.ImageIndex = 1;
                        htWindow.DisplayMultiRegion(DieRegions, ChannelImage2);
                    }
                    else if (value == 2)
                    {
                        HOperatorSet.AccessChannel(iniObjects.Image, out HObject ChannelImage3, 3);
                        SelectedImagechannel = ChannelImage3;
                        iniObjects.ChannelImage = ChannelImage3;
                        iniParameters.ImageIndex = 2;
                        htWindow.DisplayMultiRegion(DieRegions, ChannelImage3);
                    }
                    else if (value == 3)
                    {
                        HOperatorSet.AccessChannel(iniObjects.Image, out HObject ChannelImage4, 4);
                        SelectedImagechannel = ChannelImage4;
                        iniObjects.ChannelImage = ChannelImage4;
                        iniParameters.ImageIndex = 3;
                        htWindow.DisplayMultiRegion(DieRegions, ChannelImage4);
                    }
                    else if (value > 4)
                    {
                        if (value > iniParameters.ImageCountChannels)
                        {
                            System.Windows.MessageBox.Show("通道数异常");
                            return;
                        }
                        HOperatorSet.AccessChannel(iniObjects.Image, out HObject ChannelImagen, value+1);
                        SelectedImagechannel = ChannelImagen;
                        iniObjects.ChannelImage = ChannelImagen;
                        iniParameters.ImageIndex = value;
                        htWindow.DisplayMultiRegion(DieRegions, ChannelImagen);

                    }
                    switchImageChannelIndex = value;
                    OnPropertyChanged();
                }
            }
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
                        htWindow.DisplayMultiRegion(DieRegions, iniObjects.Image);
                    }
                    else if (value == 1)
                    {
                        if (iniObjects.DieImage==null)
                        {
                            System.Windows.MessageBox.Show("Die区域不存在，请重新剪裁");
                            return;
                        }
                        htWindow.Display(iniObjects.DieImage, true);
                    }
                    switchImageComboBoxIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public event Action OnSaveXML;

        public event Action OnResetDieRegion;

        private HTHalControlWPF htWindow;

        public ObservableCollection<UserRegion> DieUserRegions { get; private set; }

        public IEnumerable<HObject> DieRegions => DieUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion);

        public CommandBase LoadImageCommand { get; private set; }
        public CommandBase SelectedTrainningImageDirectoryCommand { get; private set; }
        public CommandBase AddUserRegionCommand { get; private set; }
        public CommandBase RemoveUserRegionCommand { get; private set; }
        public CommandBase CutOutCommand { get; private set; }
        public CommandBase LoadXMLCommand { get; private set; }
        public CommandBase UserRegionEnableChangedCommand { get; private set; }
        public CommandBase ModifyRegionCommand { get; private set; }   
        public CommandBase SaveAllCommand { get; private set; }
        public CommandBase LoadDieRegionCommand { get; private set; }

        public CreateReference(HTHalControlWPF htWindow,
                               IniObjects iniObjects,
                               IniParameters iniParameters,
                               string IniDirectory,
                               ObservableCollection<UserRegion> dieUserRegions)
        {
            DisplayName = "创建全局数据";
            Content = new Page_IniRecipe { DataContext = this };

            this.htWindow = htWindow;
            this.iniParameters = iniParameters;
            this.iniObjects = iniObjects;
            this.IniDirectory = IniDirectory;
            this.DieUserRegions = dieUserRegions;

            LoadImageCommand = new CommandBase(ExecuteLoadImageCommand);
            SelectedTrainningImageDirectoryCommand = new CommandBase(ExecuteSelectedTrainningImageDirectoryCommand);
            AddUserRegionCommand = new CommandBase(ExecuteAddUserRegionCommand);
            UserRegionEnableChangedCommand = new CommandBase(ExecuteUserRegionEnableChangedCommand);
            RemoveUserRegionCommand = new CommandBase(ExecuteRemoveUserRegionCommand);
            CutOutCommand = new CommandBase(ExecuteCutOutCommand);
            ModifyRegionCommand = new CommandBase(ExecuteModifyRegionCommand);//
            SaveAllCommand = new CommandBase(ExecuteSaveAllCommand);//
            LoadDieRegionCommand = new CommandBase(ExecuteLoadDieRegionCommand);//
        }

        //加载图像
        private void ExecuteLoadImageCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    /*
                    bool isRegionExsited = DieUserRegions.Count > 0;
                    if (isRegionExsited)
                    {
                        if (System.Windows.MessageBox.Show("更换图像将清空现有区域，是否继续？", "", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        {
                            return;
                        }
                    }

                    if (!Directory.Exists(iniParameters.TrainningImagesDirectory))
                    {
                        MessageBox.Show("请先选择训练图像文件的路径！");
                        return;
                    }
                     */
                    using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                    {
                        ofd.Filter = "tiff|*.tiff|tif|*.tif|ima|*.ima|bmp|*.bmp|jpg|*.jpg|png|*.png|gif|*.gif|jpeg|*.jpeg|pcx|*.pcx|pgm|*.pgm|ppm|*.ppm|pbm|*.pbm|xwd|*.xwd|pnm|*.pnm";
                        ofd.Multiselect = false;
                        ofd.InitialDirectory = iniParameters.TrainningImagesDirectory;
                        if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                        iniParameters.ImagePath = ofd.FileName;
                        iniParameters.TrainningImagesDirectory = Directory.GetParent(Directory.GetParent(ofd.FileName).ToString()).ToString();
                    }
                    iniObjects.Image?.Dispose();
                    HOperatorSet.GenEmptyObj(out HObject image);
                    HOperatorSet.ReadImage(out image, iniParameters.ImagePath);
                    iniObjects.Image = image;
                    HOperatorSet.WriteImage(iniObjects.Image, "tiff", 0, IniDirectory + "ReferenceImage.tiff");
                    HOperatorSet.WriteTuple(iniParameters.ImagePath, IniDirectory + "ImagePath.tup");
                    htWindow.DisplayMultiRegion(DieUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion), Algorithm.Region.GetChannnelImageUpdate(iniObjects.Image, 0));
                    SwitchToImage();
                    HOperatorSet.CountChannels(iniObjects.Image, out HTuple channels);
                    iniParameters.ImageCountChannels = channels;
                    iniParameters.ImageIndex = 1;
                    switchImageChannelIndex = 0;

                    //lht 1120
                    iniParameters.ChannelNames.Clear();
                    for (int i = 0; i < iniParameters.ImageCountChannels; i++)
                    {
                        ChannelName tmp_name = new ChannelName();
                        tmp_name.Name = (i + 1).ToString();
                        iniParameters.ChannelNames.Add(tmp_name);
                    }
                    switchImageChannelIndex = 0;
                    OnPropertyChanged("switchImageChannelIndex");

                    //1201 lw
                    HOperatorSet.TupleSplit(IniDirectory, "\\", out HTuple hv_subStr);
                    HTuple FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 3];
                    iniParameters.CurFovName = FOV_Name;
                    iniParameters.IniDirectory = IniDirectory;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void LoadImage()
        {
            try
            {
                iniObjects.Image?.Dispose();
                HOperatorSet.GenEmptyObj(out HObject image);

                if (File.Exists($"{IniDirectory}ReferenceImage.tiff"))
                {
                    HOperatorSet.ReadImage(out image, $"{IniDirectory}ReferenceImage.tiff");
                }
                else
                {
                    MessageBox.Show("全局数据没有参考图像！");
                }
                iniObjects.Image = image;
                htWindow.DisplayMultiRegion(DieUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion), image);
                SwitchToImage();
                HOperatorSet.CountChannels(iniObjects.Image, out HTuple channels);
                iniParameters.ImageCountChannels = channels;

                //1120-lht
                
                HOperatorSet.AccessChannel(iniObjects.Image, out HObject ChannelImage_sel, iniParameters.ImageIndex);
                iniObjects.ChannelImage = ChannelImage_sel;
                //HOperatorSet.CountChannels(iniObjects.ChannelImage, out HTuple channels1);

                /* 功能拓展
                iniObjects.ChannelImage = Algorithm.Region.GetChannnelImageUpdate(iniObjects.Image, iniParameters.ImageChannelIndex);
                iniObjects.ImageR = Algorithm.Region.GetChannnelImageUpdate(iniObjects.Image, 1);
                iniObjects.ImageG = Algorithm.Region.GetChannnelImageUpdate(iniObjects.Image, 2);
                iniObjects.ImageB = Algorithm.Region.GetChannnelImageUpdate(iniObjects.Image, 3);
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void CreateMultiChannelImages(string ImagesPath, out string GodenImgsPath)
        {
            string godenImgsFilePath = "";
            HObject ho_MutiImage = null, ho_taskImg = null;

            // Local control variables 

            HTuple hv__ImagePath = null, hv_Files = null;
            HTuple hv_MatcheFiles = null, hv_FovNameList = null, hv_SelectFiles = null;
            HTuple hv_Idx = null, hv_FovName = new HTuple(), hv_fovIdx = null;
            HTuple hv_FovIndex = new HTuple(), hv_FovFiles = new HTuple();
            HTuple hv_ImageFilesPath = new HTuple(), hv_idx = new HTuple();
            HTuple hv_ImageFiles = new HTuple(), hv_ImgIdx = new HTuple();
            HTuple hv_ObjNum = new HTuple(), hv_IC_FOV_Name = new HTuple();
            HTuple hv_GoldenImgsFilePath = new HTuple(), hv_FileExists = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_MutiImage);
            HOperatorSet.GenEmptyObj(out ho_taskImg);

            hv__ImagePath = ImagesPath;

            HOperatorSet.ListFiles(hv__ImagePath, "directories", out hv_Files);
            HOperatorSet.TupleRegexpSelect(hv_Files, "-Fov_*", out hv_Files);
            HOperatorSet.TupleRegexpMatch(hv_Files, "Fov_(.*)\\\\", out hv_MatcheFiles);

            if ((int)(new HTuple((new HTuple(hv_MatcheFiles.TupleLength())).TupleEqual(0))) != 0)
            {
                //图像存储规则不正确
                ho_MutiImage.Dispose();
                ho_taskImg.Dispose();
                GodenImgsPath = "";
                MessageBox.Show("图像名称存储规则不正确，请确认 Fov_ 标识！");
                return;
            }

            //Fov数目
            hv_FovNameList = new HTuple();
            hv_SelectFiles = hv_MatcheFiles.Clone();
            for (hv_Idx = 0; (int)hv_Idx <= (int)((new HTuple(hv_MatcheFiles.TupleLength())) - 1); hv_Idx = (int)hv_Idx + 1)
            {

                hv_FovName = hv_SelectFiles[0];
                hv_FovNameList = hv_FovNameList.TupleConcat(hv_FovName);
                HOperatorSet.TupleDifference(hv_SelectFiles, hv_FovName, out hv_SelectFiles);
                if ((int)(new HTuple(hv_SelectFiles.TupleEqual(new HTuple()))) != 0)
                {
                    break;
                }
            }

            //遍历Fov
            for (hv_fovIdx = 0; (int)hv_fovIdx <= (int)((new HTuple(hv_FovNameList.TupleLength()
                )) - 1); hv_fovIdx = (int)hv_fovIdx + 1)
            {
                hv_FovName = hv_FovNameList.TupleSelect(hv_fovIdx);
                HOperatorSet.TupleFind(hv_MatcheFiles, hv_FovName, out hv_FovIndex);
                HOperatorSet.TupleSelect(hv_Files, hv_FovIndex, out hv_FovFiles);

                hv_ImageFilesPath = hv_FovFiles.Clone();
                for (hv_idx = 0; (int)hv_idx <= (int)((new HTuple(hv_ImageFilesPath.TupleLength()
                    )) - 1); hv_idx = (int)hv_idx + 1)
                {
                    // 遍历task图像
                    Algorithm.Model_RegionAlg.list_image_files(hv_ImageFilesPath.TupleSelect(hv_idx), (((((new HTuple(".bmp")).TupleConcat(
                                ".tiff")).TupleConcat(".tif")).TupleConcat(".jpg")).TupleConcat(".jpeg")).TupleConcat(".png"), "recursive", out hv_ImageFiles);

                    if ((int)(new HTuple((new HTuple(hv_ImageFiles.TupleLength())).TupleEqual(0))) != 0)
                    {
                        MessageBox.Show("请选择格式为bmp/tiff/jpg/png的图像！");
                        break;
                    }

                    ho_MutiImage.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_MutiImage);
                    for (hv_ImgIdx = 0; (int)hv_ImgIdx <= (int)((new HTuple(hv_ImageFiles.TupleLength()
                        )) - 1); hv_ImgIdx = (int)hv_ImgIdx + 1)
                    {
                        ho_taskImg.Dispose();
                        HOperatorSet.ReadImage(out ho_taskImg, hv_ImageFiles.TupleSelect(hv_ImgIdx));
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.AppendChannel(ho_MutiImage, ho_taskImg, out ExpTmpOutVar_0
                                );
                            ho_MutiImage.Dispose();
                            ho_MutiImage = ExpTmpOutVar_0;
                        }
                    }

                    HOperatorSet.CountObj(ho_MutiImage, out hv_ObjNum);
                    if ((int)(new HTuple(hv_ObjNum.TupleGreater(0))) != 0)
                    {
                        //string Expression = hv__ImagePath + "\\(.*)";
                        //HOperatorSet.TupleRegexpMatch(hv_ImageFilesPath.TupleSelect(hv_idx), Expression, out hv_IC_FOV_Name);

                        HOperatorSet.TupleSplit(hv_ImageFilesPath.TupleSelect(hv_idx), "\\", out HTuple hv_subStr);
                        hv_IC_FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 1];

                        hv_GoldenImgsFilePath = hv__ImagePath + "\\GoldenImgs";
                        HOperatorSet.FileExists(hv_GoldenImgsFilePath, out hv_FileExists);
                        if ((int)(new HTuple(hv_FileExists.TupleEqual(0))) != 0)
                        {
                            HOperatorSet.MakeDir(hv_GoldenImgsFilePath);
                        }
                        godenImgsFilePath = hv_GoldenImgsFilePath; // 输出训练图集路径

                        hv_GoldenImgsFilePath = (hv_GoldenImgsFilePath + "\\" + hv_FovName) + "\\";
                        HOperatorSet.FileExists(hv_GoldenImgsFilePath, out hv_FileExists);
                        if ((int)(new HTuple(hv_FileExists.TupleEqual(0))) != 0)
                        {
                            HOperatorSet.MakeDir(hv_GoldenImgsFilePath);
                        }
                        hv_GoldenImgsFilePath = (hv_GoldenImgsFilePath + hv_IC_FOV_Name) + "\\";
                        HOperatorSet.FileExists(hv_GoldenImgsFilePath, out hv_FileExists);
                        if ((int)(new HTuple(hv_FileExists.TupleEqual(0))) != 0)
                        {
                            HOperatorSet.MakeDir(hv_GoldenImgsFilePath);
                        }

                        HOperatorSet.WriteImage(ho_MutiImage, "tiff", 0, hv_GoldenImgsFilePath + "TaskImg.tiff");
                    }
                }
            }

            // stop(); only in hdevelop
            ho_MutiImage.Dispose();
            ho_taskImg.Dispose();

            GodenImgsPath = godenImgsFilePath;

            return;
        }
    
        //选择训练图集
        private void ExecuteSelectedTrainningImageDirectoryCommand(object parameter)
        {
            if (isRightClick)
            {
                try
                {
                    // 1126 lw
                    if (MessageBox.Show("是否生成多通道训练图集？", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
                        {
                            if (Directory.Exists(iniParameters.TrainningImagesDirectory))
                            {
                                fbd.SelectedPath = iniParameters.TrainningImagesDirectory;
                            }
                            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                Window_Loading window_Loading = new Window_Loading("正在生成多通道训练图集");

                                window_Loading.Show();
                                CreateMultiChannelImages(fbd.SelectedPath, out string GodenImgsPath);
                                window_Loading.Close();

                                if (GodenImgsPath != "")
                                { 
                                    MessageBox.Show("训练图集已生成，请选择当前Fov文件夹！");
                                }
                                else
                                {                                       
                                    return;
                                }

                                fbd.SelectedPath = GodenImgsPath + "\\";

                                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    iniParameters.TrainningImagesDirectory = fbd.SelectedPath;
                                    HOperatorSet.WriteTuple(iniParameters.TrainningImagesDirectory, IniDirectory + "TrainningImagesDirectory.tup");
                                }
                            }
                        }
                    }
                    else
                    {
                        using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
                        {
                            if (Directory.Exists(iniParameters.TrainningImagesDirectory))
                            {
                                fbd.SelectedPath = iniParameters.TrainningImagesDirectory;
                            }
                            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                iniParameters.TrainningImagesDirectory = fbd.SelectedPath;
                                HOperatorSet.WriteTuple(iniParameters.TrainningImagesDirectory, IniDirectory + "TrainningImagesDirectory.tup");
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

        public void LoadTrainningImageDirectory()
        {

        }

        //加载Die区域 Region
        private void ExecuteLoadDieRegionCommand(object parameter)
        {
            try
            {
                //switchImageChannelIndex = 0; // 1205 mod
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = "reg|*.reg";
                    ofd.Multiselect = false;
                    //ofd.InitialDirectory = GoldenModelParameter.;
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return ;
                    regionName = ofd.FileName;
                }
                HOperatorSet.GenEmptyObj(out HObject GivenDieRegions);
                HOperatorSet.ReadRegion(out GivenDieRegions, new HTuple(regionName));
                LoadCoarseReference(GivenDieRegions);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void LoadCoarseReference(HObject GivenDieRegions)
        {
            try
            {
                DieUserRegions.Clear();
                HOperatorSet.CountObj(GivenDieRegions, out HTuple DieNumbers);
                for (int i = 0; i < DieNumbers; i++)
                {
                    HOperatorSet.SmallestRectangle1(GivenDieRegions.SelectObj(i + 1), out HTuple row1_Rectangle, out HTuple column1_Rectangle, out HTuple row2_Rectangle, out HTuple column2_Rectangle);
                    UserRegion userRegion = UserRegion.GetHWindowRegionUpdate(htWindow, RegionType.Rectangle1, row1_Rectangle, column1_Rectangle, row2_Rectangle, column2_Rectangle);
                    if (userRegion == null) return;
                    userRegion.RegionType = RegionType.Rectangle1;
                    DieUserRegions.Add(userRegion);
                    DieUserRegions[i].Index = i + 1;
                }
                htWindow.DisplayMultiRegion(DieRegions);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                GivenDieRegions = null;
            }

        }

        //添加区域
        private void ExecuteAddUserRegionCommand(object parameter)
        {
            if (isRightClick)
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
                isRightClick = false;
                try
                {
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
                    htWindow.RegionType = RegionType.Null;
                    isRightClick = true;
                }
            }
        }

        //删除区域
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
                            if (DieUserRegions[i] == selectedUserRegion)
                            {
                                if (MessageBox.Show("即将删除用于创建参考Die的区域，删除该区域将清空后续数据。" + System.Environment.NewLine + "点击确定删除该区域；" + System.Environment.NewLine + "点击取消跳过。",
                                                    "", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                                {
                                    continue;
                                }
                                else
                                {
                                    selectedUserRegion = null;
                                    iniObjects.DieImage = null;
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
                    htWindow.DisplayMultiRegion(DieRegions, iniObjects.Image);
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
                            if (DieUserRegions[i] == selectedUserRegion)
                            {
                                if (MessageBox.Show($"剪裁区域为制作模板的参考Die区域，是否确定修改?", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
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
                                    DieUserRegions[i].Index = i + 1;
                                    htWindow.DisplayMultiRegion(DieRegions);//ok
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
                        selectedUserRegion = null;
                        iniObjects.DieImage = null;
                        OnResetDieRegion?.Invoke();
                    }
                }
                //htWindow.DisplayMultiRegion(DieRegions, iniObjects.Image); //

                htWindow.DisplayMultiRegion(DieUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion), iniObjects.Image); //DieRegions
                SwitchToImage();
            }
        }

        //裁剪Die区域，切换到Die区域显示，进行通道分离
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

                // 1130
                HOperatorSet.SetSystem("clip_region", "false");

                //userRegionForCutOut = SelectedUserRegion;
                //iniObjects.UserRegionForCutOut = SelectedUserRegion;
                iniParameters.UserRegionForCutOutIndex = SelectedUserRegion.Index;
                DieCutOut(false);
            }
        }

        public bool DieCutOut(bool IsShowDieImage)
        {
            HObject imageReduced = null;
            HOperatorSet.GenEmptyObj(out imageReduced);
            iniObjects.DieImage?.Dispose();
            try
            {
                // 1124
                if (iniObjects.ChannelImage == null)
                {
                    HOperatorSet.AccessChannel(iniObjects.Image, out HObject ChannelImage, switchImageChannelIndex + 1);
                    iniObjects.ChannelImage = ChannelImage;
                }

                HOperatorSet.CountChannels(iniObjects.ChannelImage, out HTuple channels);
                //HOperatorSet.ReduceDomain(iniObjects.Image, SelectedUserRegion.CalculateRegion, out imageReduced);//修改

                // 1125
                HOperatorSet.GenEmptyRegion(out HObject EmptyRegion);
                HOperatorSet.TestEqualRegion(SelectedUserRegion.CalculateRegion, EmptyRegion, out HTuple isEqual);
                if (isEqual)
                {
                    HOperatorSet.GenRectangle1(out HObject Die_rect, SelectedUserRegion.RegionParameters[0], SelectedUserRegion.RegionParameters[1], SelectedUserRegion.RegionParameters[2], SelectedUserRegion.RegionParameters[3]);
                    SelectedUserRegion.CalculateRegion = Die_rect;
                }

                HOperatorSet.ReduceDomain(iniObjects.ChannelImage, SelectedUserRegion.CalculateRegion, out imageReduced);//120修改
                HOperatorSet.CropDomain(imageReduced, out HObject dieImage);
                iniObjects.DieImage = dieImage;
                imageReduced.Dispose();
                if (IsShowDieImage)
                {
                    htWindow.Display(dieImage, true);
                    SwitchToDieImage();
                }

                //通道分离
                /* 功能拓展
                iniObjects.DieChannelImage = Algorithm.Region.GetChannnelImageUpdate(iniObjects.dieImage, iniParameters.ImageChannelIndex);
                iniObjects.DieImageR = Algorithm.Region.GetChannnelImageUpdate(iniObjects.dieImage, 1);
                iniObjects.DieImageG = Algorithm.Region.GetChannnelImageUpdate(iniObjects.dieImage, 2);
                iniObjects.DieImageB = Algorithm.Region.GetChannnelImageUpdate(iniObjects.dieImage, 3);
                */

                //Die的偏差
                iniParameters.DieImageRowOffset = selectedUserRegion.RegionParameters[0];
                iniParameters.DieImageColumnOffset = selectedUserRegion.RegionParameters[1];
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            finally
            {
                //imageReduced?.Dispose();
            }
        }

        //保存参数
        private void ExecuteSaveAllCommand(object parameter)
        {
            try
            {
                OnSaveXML?.Invoke();  
                MessageBox.Show("参数保存完成!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "参数保存失败");
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
            htWindow.DisplayMultiRegion(DieRegions, Algorithm.Region.GetChannnelImageUpdate(iniObjects.Image, iniParameters.ImageIndex));
            SwitchToImage();

            //1201 lw
            if (iniParameters.CurFovName == "")
            { 
                HOperatorSet.TupleSplit(IniDirectory, "\\", out HTuple hv_subStr);
                HTuple FOV_Name = hv_subStr[(new HTuple(hv_subStr.TupleLength())) - 3];
                iniParameters.CurFovName = FOV_Name;
            }

            // 避免其他页滚轮显示检测区域
            iniParameters.IsInspectNodeVerify = false;
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
            (Content as Page_IniRecipe).DataContext = null;
            (Content as Page_IniRecipe).Close();
            Content = null;
            htWindow = null;
            iniParameters = null;
            iniObjects = null;
            DieUserRegions = null;
            SelectedUserRegion = null;
            LoadImageCommand = null;
            SelectedTrainningImageDirectoryCommand = null;
            AddUserRegionCommand = null;
            RemoveUserRegionCommand = null;
            CutOutCommand = null;
            LoadXMLCommand = null;
            UserRegionEnableChangedCommand = null;
            ModifyRegionCommand = null;
            SaveAllCommand = null;
            LoadDieRegionCommand = null;
        }
    }

}

