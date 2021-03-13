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
    class FrameLocateInspectVerify : ViewModelBase, IProcedure
    {
        public string DisplayName { get; }

        public object Content { get; private set; }

        public event Action OnSaveXML;

        private HTHalControlWPF htWindow;

        private GoldenModelObject goldenModelObject;

        public GoldenModelParameter goldenModelParameter { get; set; }

        public FrameLocateInspectParameter FrameLocateInspectParameter { get; set; }

        private string pImageIndexPath;
        public string PImageIndexPath
        {
            get => pImageIndexPath;
            set => OnPropertyChanged(ref pImageIndexPath, value);
        }

        public ObservableCollection<UserRegion> DieUserRegions { get; private set; }

        private ObservableCollection<MatchRegionsGroup> MatchGroups { get; set; }
        public ObservableCollection<UserRegion> FrameUserRegions { get; private set; }

        private IEnumerable<HObject> DieRegions => DieUserRegions.Where(r => r.IsEnable).Select(r => r.CalculateRegion);

        public CommandBase ImagesSetVerifyCommand { get; private set; }
        public CommandBase FrameLocCommand { get; private set; }
        public CommandBase FrameVerifyCommand { get; private set; }
        public CommandBase FrameBridgeVerifyCommand { get; private set; }
        public CommandBase SaveCommand { get; private set; }
        public CommandBase PreviousCommand { get; private set; }
        public CommandBase NextCommand { get; private set; }
        public CommandBase FramePreviousCommand { get; private set; }
        public CommandBase FrameNextCommand { get; private set; }
        public CommandBase PegRackPreviousCommand { get; private set; }
        public CommandBase PegRackNextCommand { get; private set; }
        public CommandBase RefreshModels { get; private set; }
        public CommandBase RefreshImagesSetModels { get; private set; }        

        private double[] frameHomMat2D;
        public double[] FrameHomMat2D
        {
            get => frameHomMat2D;
            set => OnPropertyChanged(ref frameHomMat2D, value);
        }

        private int isFovTaskFlag = 0;

        private HObject ImageVerify = null;
        private HObject ChannelImageVerify0 = null;

        private int imgIndex0 = 0;

        private int pImageIndex = -1;
        HTuple  modelID = new HTuple();

        private readonly string RecipeDirectory;

        public FrameLocateInspectVerify(HTHalControlWPF htWindow,
                                        GoldenModelObject goldenModelObject,
                                        GoldenModelParameter goldenModelParameter,
                                        FrameLocateInspectParameter frameLocateInspectParameter,
                                        ObservableCollection<UserRegion> dieUserRegions,
                                        ObservableCollection<MatchRegionsGroup> matchGroups,
                                        ObservableCollection<UserRegion> frameUserRegions,
                                        string recipeDirectory)
        {
            DisplayName = "检测验证";
            Content = new Page_FrameModelInspectVerify { DataContext = this };
            this.FrameLocateInspectParameter = frameLocateInspectParameter;
            this.htWindow = htWindow;
            this.goldenModelObject = goldenModelObject;
            this.goldenModelParameter = goldenModelParameter;
            this.DieUserRegions = dieUserRegions;
            this.MatchGroups = matchGroups;
            this.FrameUserRegions = frameUserRegions;
            this.RecipeDirectory = recipeDirectory;

            ImagesSetVerifyCommand = new CommandBase(ExecuteImagesSetVerifyCommand);
            FrameLocCommand = new CommandBase(ExecuteFrameLocCommand);
            SaveCommand = new CommandBase(ExecuteSaveCommand);
            PreviousCommand = new CommandBase(ExecutePreviousCommand);
            NextCommand = new CommandBase(ExecuteNextCommand);
            RefreshModels = new CommandBase(ExecuteRefreshModels); 
            RefreshImagesSetModels = new CommandBase(ExecuteRefreshImagesSetModels);
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
                    PImageIndexPath = imageFiles[goldenModelParameter.ImageChannelIndex];
                    ImageVerify = ho_MutiImage;
                }
                else
                {
                    isFovTaskFlag = 0;

                    Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                    string[] folderList = imageFiles;
                    goldenModelParameter.CurrentVerifySet = folderList.Count();
                    PImageIndexPath = imageFiles[0];
                    HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                    ImageVerify = image;

                }
                ChannelImageVerify0 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, goldenModelParameter.ImageChannelIndex);
                htWindow.Display(ChannelImageVerify0, true);
                pImageIndex = 0;
                imgIndex0 = 0;
                LoadModels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
            }        
        }

        private void ExecuteRefreshImagesSetModels(object parameter)
        {
            if (Directory.Exists(goldenModelParameter.VerifyImagesDirectory))
            {
                Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                string[] folderList = imageFiles;
                goldenModelParameter.CurrentVerifySet = folderList.Count();
                PImageIndexPath = imageFiles[0];
                HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                ImageVerify = image;
                ChannelImageVerify0 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, goldenModelParameter.ImageChannelIndex);
                htWindow.Display(ChannelImageVerify0, true);
                pImageIndex = 0;
                imgIndex0 = 0;
                LoadModels();
            }
        }
        //修改
        private void LoadModels()
        {
            if (goldenModelObject.PosModelID == null)
            {

                //---------------20201205------------------
                //初始化
                HTuple hv_PosModel = new HTuple();
                //确定posmodel的个数
                HOperatorSet.ListFiles(RecipeDirectory, "files", out HTuple hv_ItemFiles);
                HOperatorSet.TupleRegexpSelect(hv_ItemFiles, "PosModel.*", out HTuple hv__PosModelItem);

                // 清除已有模板 0115 -lw
                if (modelID != null && modelID.TupleLength() > 0)
                {
                    HTuple ModelTypeStr = null;
                    ModelTypeStr = goldenModelParameter.ModelType == 0 ? "ncc" : "shape";
                    Algorithm.Model_RegionAlg.HTV_clear_model(modelID, ModelTypeStr);
                    modelID = null;
                }

                //
                for (int hv_idx = 0; (int)hv_idx <= (int)((new HTuple(hv__PosModelItem.TupleLength())) - 1); hv_idx = (int)hv_idx + 1)
                {
                    //
                    HTuple hv__filePath = hv__PosModelItem.TupleSelect(hv_idx);
                    //读取模板类别
                    HTuple hv_Model_Type = goldenModelParameter.ModelType;

                    if ((int)(new HTuple(hv_Model_Type.TupleEqual(0))) != 0)
                    {
                        HOperatorSet.ReadNccModel(hv__filePath, out HTuple hv__PosModel);
                        hv_PosModel = hv_PosModel.TupleConcat(hv__PosModel);
                    }
                    else if ((int)(new HTuple(hv_Model_Type.TupleEqual(1))) != 0)
                    {
                        HOperatorSet.ReadShapeModel(hv__filePath, out HTuple hv__PosModel);
                        hv_PosModel = hv_PosModel.TupleConcat(hv__PosModel);
                    }
                }
                modelID = hv_PosModel;
                /*
                if (File.Exists($"{RecipeDirectory}\\PosModel.dat"))
                {
                    modelID = Algorithm.File.ReadModel($"{RecipeDirectory}\\PosModel.dat", goldenModelParameter.ModelType);
                }
                else
                {
                    MessageBox.Show("请先创建定位模板！");
                    return;
                }*/

            }
            else
            {
                modelID = goldenModelObject.PosModelID;
            }
        }
        //private void LoadModels()
        //{
        //    if (goldenModelObject.PosModelID == null)
        //    {
        //        if (($"{RecipeDirectory}\\PosModel.dat").Contains("PosModel"))
        //        {
        //            HTuple modleid = new HTuple();
        //            for (int i = 0; i < goldenModelParameter.ModelIdPath.Split(',').Length; i++)
        //            {
        //                HOperatorSet.TupleConcat(Algorithm.File.ReadModel($"{RecipeDirectory}\\PosModel_{i}.dat", goldenModelParameter.ModelType), modleid,out modleid);
        //            }
        //            modelID = modleid;
        //        }
        //        else
        //        {
        //            MessageBox.Show("请先创建定位模板！");
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        modelID = goldenModelObject.PosModelID;
        //    }
        //}

        //框架定位
        //


        private void ExecuteFrameLocCommand(object parameter)
        {
            //Window_Loading window_Loading = new Window_Loading("正在检验");  

            if (ImageVerify == null || !ImageVerify.IsInitialized())
            {
                MessageBox.Show("请先加载图集 或刷新！");
                return;
            }
            if (DieUserRegions.Count()<1)
            {
                MessageBox.Show("没有搜索区域！");
                return;
            }
            if (modelID==null || modelID.TupleLength()<1)
            {
                MessageBox.Show("请先创建定位模板或刷新模板！");
                return;
            }

            HOperatorSet.GenEmptyObj(out HObject matchRegions);

            foreach (var item in MatchGroups)
            {
                // 0115 lw
                HOperatorSet.ConcatObj(Algorithm.Region.Union1Region(item.MatchUserRegions.Where(r => r.IsEnable)), matchRegions, out matchRegions);
            }

            try
            {
                //window_Loading.Show();
                // 1121
                //HOperatorSet.AccessChannel(ImageVerify, out HObject ChannelImage, goldenModelParameter.ImageChannelIndex + 1);
                HObject ChannelImage = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, goldenModelParameter.ImageChannelIndex);
                htWindow.Display(ChannelImage, true);

                //htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, goldenModelParameter.ImageChannelIndex), true);

                //框架定位
                Algorithm.Model_RegionAlg.HTV_Frame_Location(ChannelImage,
                                                             DieRegions.ElementAt(imgIndex0),
                                                             matchRegions,
                                                             Algorithm.Region.Union1Region(FrameUserRegions),
                                                             out HObject PosFailRegs,
                                                            (goldenModelParameter.ModelType == 0) ? "ncc" : "shape",//ncc
                                                             modelID,
                                                             FrameLocateInspectParameter.DilationSize,
                                                             FrameLocateInspectParameter.MinMatchScore,
                                                             FrameLocateInspectParameter.AngleStart,
                                                             FrameLocateInspectParameter.AngleExt,
                                                             FrameLocateInspectParameter.MatchNum,
                                                             goldenModelParameter.ImageChannelIndex + 1,
                                                             out HTuple PosLocPara,
                                                             out HTuple frameHomMat2D,
                                                             out HTuple FrameLocDefectType,
                                                             out HTuple DefectFrameLocImgIdx,
                                                             out HTupleVector DefectFrameLocValue,
                                                             out HTuple locErrCode, out HTuple ErrStr);

                if (locErrCode != 0)
                {
                    imgIndex0++;
                    if (imgIndex0 + 1 > DieUserRegions.Count)
                    {
                        imgIndex0 = 0;
                    }
                    FrameHomMat2D = null;
                    MessageBox.Show("框架没定位到！");
                    return;
                }

                FrameHomMat2D = frameHomMat2D;

                //Algorithm.Model_RegionAlg.HTV_Arrange_Pos(modelID, (goldenModelParameter.ModelType == 0) ? "ncc" : "shape",
                //                PosLocPara[0],
                //                PosLocPara[1],
                //                PosLocPara[2],
                //                out HTuple PosRow, out HTuple PosCol);

                HOperatorSet.AffineTransRegion(Algorithm.Region.Union1Region(FrameUserRegions), out HObject ho__FrameRegion, frameHomMat2D, "nearest_neighbor");

                if ((int)((new HTuple((new HTuple(((PosLocPara.TupleSelect(
                    0))).TupleEqual(0))).TupleAnd(new HTuple(((PosLocPara.TupleSelect(
                    1))).TupleEqual(0))))).TupleAnd(new HTuple(((PosLocPara.TupleSelect(
                    2))).TupleEqual(0)))) != 0)
                {

                    HOperatorSet.SmallestRectangle2(ho__FrameRegion, out HTuple hv_Row, out HTuple hv_Col, out HTuple hv_Phi, out HTuple hv_Len1, out HTuple hv_Len2);

                    PosLocPara[0] = hv_Row;
                    PosLocPara[1] = hv_Col;
                    PosLocPara[2] = hv_Phi;
                }

                HOperatorSet.GenCrossContourXld(out HObject cross, PosLocPara[0], PosLocPara[1], 40, PosLocPara[2]);

                //htWindow.DisplaySingleRegion(cross.ConcatObj(DieRegions.ElementAt(imgIndex0)).ConcatObj(PosFailRegs), Algorithm.Region.GetChannnelImageUpdate(ImageVerify, goldenModelParameter.ImageChannelIndex));
                //htWindow.DisplaySingleRegion(cross.ConcatObj(DieRegions.ElementAt(imgIndex0)).ConcatObj(PosFailRegs), ChannelImage);
                htWindow.DisplaySingleRegion(cross.ConcatObj(ho__FrameRegion).ConcatObj(PosFailRegs), ChannelImage);

                imgIndex0++;
                if (imgIndex0 + 1 > DieUserRegions.Count)
                {
                    imgIndex0 = 0;
                }
                PosFailRegs?.Dispose();
            }

            catch (Exception ex)
            {
                //window_Loading.Close();
                MessageBox.Show(ex.ToString(), "检测验证异常！");
                imgIndex0++;
                if (imgIndex0 + 1 > DieUserRegions.Count)
                {
                    imgIndex0 = 0;
                }
            }
        }

        //保存
        private void ExecuteSaveCommand(object parameter)
        {
            try
            {
                if (goldenModelParameter.ImageCountChannels > 0  &&  goldenModelParameter.ImageChannelIndex < 0)
                {
                    MessageBox.Show("请选择图像通道！");
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

        private void ExecutePreviousCommand(object parameter)
        {
            try
            {
                imgIndex0 = 0;
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
                        PImageIndexPath = imageFiles[goldenModelParameter.ImageChannelIndex];
                    }
                    else
                    { 
                        Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }

                    ChannelImageVerify0 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, goldenModelParameter.ImageChannelIndex);
                    htWindow.Display(ChannelImageVerify0, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ExecuteNextCommand(object parameter)
        {
            try
            {
                imgIndex0 = 0;
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
                        PImageIndexPath = imageFiles[goldenModelParameter.ImageChannelIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(goldenModelParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }

                    ChannelImageVerify0 = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, goldenModelParameter.ImageChannelIndex);                 
                    htWindow.Display(ChannelImageVerify0, true);
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
            htWindow.Display(Algorithm.Region.GetChannnelImageUpdate(ImageVerify, goldenModelParameter.ImageChannelIndex), true);
        }

        public void Dispose()
        {
            (Content as Page_FrameModelInspectVerify).DataContext = null;
            (Content as Page_FrameModelInspectVerify).Close();
            Content = null;
            htWindow = null;
            ImageVerify = null;

            goldenModelObject = null;
            FrameLocateInspectParameter = null;
             DieUserRegions = null;
             FrameVerifyCommand = null;
            FrameBridgeVerifyCommand = null;
            SaveCommand = null;
            FramePreviousCommand = null;
            FrameNextCommand = null;
            PegRackPreviousCommand = null;
            PegRackNextCommand = null;
            FrameLocCommand = null;
            FrameHomMat2D = null;
            modelID = null;
        }
    }
}
