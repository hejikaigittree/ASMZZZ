using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LFAOIRecipe
{
    class WireInspectVerify : ViewModelBase, IProcedure
    {
        //1123
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public event Action OnSaveXML;

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
                        if (value == -1)
                        {
                            HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, WireParameter.ImageIndex + 1);
                            htWindow.Display(ChannelImageDisplay, true);
                        }
                        else if (value >= 0)
                        {
                            HOperatorSet.AccessChannel(WireObject.DieImage, out HObject ChannelImageDisplay, value + 1);
                            WireParameter.ImageIndex = value;
                            htWindow.Display(ChannelImageDisplay, true);
                        }
                        imageIndex = value;
                        OnPropertyChanged();
                    }
                    else
                    {
                        WireParameter.ImageIndex = value;
                        ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, WireParameter.ImageIndex);
                        if (ChannelImageVerify != null) htWindow.Display(ChannelImageVerify, true);
                        imageIndex = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        private int isFovTaskFlag = 0;

        private int imgIndex = 0;
        private int pImageIndex = -1;

        private string pImageIndexPath;
        public string PImageIndexPath
        {
            get => pImageIndexPath;
            set => OnPropertyChanged(ref pImageIndexPath, value);
        }

        public CommandBase VerifyCommand { get; private set; }
        public CommandBase SaveCommand { get; private set; }
        public CommandBase DisplayLightDarkImageCommand { get; private set; }
        public CommandBase ImagesSetVerifyCommand { get; private set; }
        public CommandBase PreviousCommand { get; private set; }
        public CommandBase NextCommand { get; private set; }

        private HTHalControlWPF htWindow;

        private readonly string ModelsFile;

        private readonly string RecipeFile;

        private readonly string ReferenceDirectory;

        private readonly string ModelsRecipeDirectory;

        private HObject ImageVerify, ChannelImageVerify, DieRegions = null;

        private WireObject WireObject;

        public WireParameter WireParameter { get; set; }

        public CommandBase RefreshImagesSet { get; private set; }

        public ObservableCollection<UserRegion> StartBallAutoUserRegion { get; private set; }
        public ObservableCollection<UserRegion> StopBallAutoUserRegion { get; private set; }
        private ObservableCollection<WireRegionsGroup> WireRegionsGroup { get; set; }

        public WireInspectVerify(HTHalControlWPF htWindow,
                                        string modelsFile,
                                        string recipeFile,
                                        string referenceDirectory,
                                        WireObject WireObject,
                                        WireParameter WireParameter,
                                        ObservableCollection<UserRegion> startBallAutoUserRegion,
                                        ObservableCollection<UserRegion> stopBallAutoUserRegion,
                                        ObservableCollection<WireRegionsGroup> WireRegionsGroup,
                                        string modelsRecipeDirectory)
        {
            DisplayName = "检测验证";
            Content = new Page_WireInspectVerify { DataContext = this };
            this.htWindow = htWindow;
            this.ModelsFile = modelsFile;
            this.RecipeFile = recipeFile;
            this.ReferenceDirectory = referenceDirectory;
            this.WireObject = WireObject;
            this.WireParameter = WireParameter;
            this.StartBallAutoUserRegion = startBallAutoUserRegion;
            this.StopBallAutoUserRegion = stopBallAutoUserRegion;
            this.WireRegionsGroup = WireRegionsGroup;
            this.ModelsRecipeDirectory = modelsRecipeDirectory;
            VerifyCommand = new CommandBase(ExecuteVerifyCommand);
            SaveCommand = new CommandBase(ExecuteSaveCommand);
            //DisplayLightDarkImageCommand = new CommandBase(ExecuteDisplayLightDarkImageCommand);
            ImagesSetVerifyCommand = new CommandBase(ExecuteImagesSetVerifyCommand);
            PreviousCommand = new CommandBase(ExecutePreviousCommand);
            NextCommand = new CommandBase(ExecuteNextCommand);
            ImageVerify = WireObject?.Image;//改
            RefreshImagesSet = new CommandBase(ExecuteRefreshImagesSet);
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
                        WireParameter.VerifyImagesDirectory = fbd.SelectedPath;
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
                                                                                WireParameter.VerifyImagesDirectory,
                                                                                WireParameter.CurFovName,
                                                                                0, out HTuple hv_o_ImageVerifyNum, out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        WireParameter.CurrentVerifySet = hv_o_ImageVerifyNum;
                        PImageIndexPath = imageFiles[WireParameter.ImageIndex];
                        ImageVerify = ho_MutiImage;
                    }
                    else
                    {
                        isFovTaskFlag = 0;

                        Algorithm.File.list_image_files(WireParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        string[] folderList = imageFiles;
                        WireParameter.CurrentVerifySet = folderList.Count();
                        PImageIndexPath = imageFiles[0];
                        HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                        ImageVerify = image;
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, WireParameter.ImageIndex);
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

        // 加载默认图集
        private void ExecuteRefreshImagesSet(object parameter)
        {
            if (Directory.Exists(WireParameter.VerifyImagesDirectory))
            {
                Algorithm.File.list_image_files(WireParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                string[] folderList = imageFiles;
                WireParameter.CurrentVerifySet = folderList.Count();
                PImageIndexPath = imageFiles[0];
                HOperatorSet.ReadImage(out HObject image, imageFiles[0]);
                ImageVerify = image;
                ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, WireParameter.ImageIndex);
                htWindow.Display(ChannelImageVerify, true);
                pImageIndex = 0;
                imgIndex = 0;
                HOperatorSet.GenEmptyObj(out DieRegions);
                HOperatorSet.ReadRegion(out DieRegions, $"{ReferenceDirectory}CoarseReference.reg");
            }
        }

        //检测验证
        private void ExecuteVerifyCommand(object parameter)
        {
            //Window_Loading window_Loading = new Window_Loading("正在检验");
            if (ChannelImageVerify == null || !ChannelImageVerify.IsInitialized())
            {
                MessageBox.Show("请先选择验证图像");
                return;
            }
            if (WireParameter.StartBondonRecipesIndexs == null || WireParameter.StopBondonRecipesIndexs == null)
            {
                MessageBox.Show("请先选择焊点所在位置！");
                return;
            }
            if (WireParameter.ImageCountChannels > 0 && WireParameter.ImageIndex < 0) // 1123
            {
                MessageBox.Show("请先选择通道图像！");
                return;
            }
            if (DieRegions == null || !DieRegions.IsInitialized())
            {
                MessageBox.Show("请先加载搜索区！");
                return;
            }

            /////////////////////add 2021-01-19
            ///
            ///
            HObject ho_realStartBall = null, ho__bondInspectReg = null;
            HObject ho__startBondContours = null, ho__BondInspectReg = null, ho__startBondContour = null;
            HObject ho__ModelImg = null, ho_realStopBall = null, ho__stopBondContour = null,ho__stopBondContours = null;
            HObject ho_realStartBall_sort = null, ho__FailBondReg = null;
            HObject ho_realStopBall_sort = null, ho_tmp_reg = null, ho_virtualStartBall1 = null;
            HObject ho_virtualStopBall1 = null, ho_virtualStartBall2 = null;
            HObject ho_virtualStopBall2 = null, ho_virtualStartBall3 = null;
            HObject ho_virtualStopBall3 = null, ho__LineStartRegs = null;
            HObject ho__LineStopRegs = null, ho__tmp_reg_aff = null, ho_tmpvirtualStartreg = null;
            HObject ho_tmprealStartReg = null, ho_tmpvirtualStopreg = null;
            HObject ho_tmprealStopReg = null, ho__BondInspectRegs = null;

            HObjectVector ho__FailBondRegs = new HObjectVector(1);
            HObjectVector hvec__BondObjs = new HObjectVector(2), hvec__bondInspectObj = new HObjectVector(1);

            HTuple hv__modelErrCode = null, hv__modelErrStr = null;
            HTuple hv__recipeErrCode = null, hv__recipeErrStr = null;
            HTuple hv_regNum = new HTuple();
            HTuple hv_Number_start_group = new HTuple(), hv_Number_stop_group = new HTuple();
            HTuple hv_IdxReg = new HTuple(), hv__wireImageIdx = new HTuple();
            HTuple hv__IsUseStartReg = new HTuple(), hv_StartRegBelongToWhat = new HTuple();
            HTuple hv_o_ErrCode = new HTuple(), hv_o_ErrStr = new HTuple();
            HTuple hv__StartHomMat2D = new HTuple(), hv__ErrCode = new HTuple();
            HTuple hv__ErrStr = new HTuple(), hv_ii = new HTuple();
            HTuple hv__BondItem = new HTuple(), hv__BondModelPath = new HTuple();
            HTuple hv__BondRecipePath = new HTuple(), hv_regidx = new HTuple(), hv__InspectMethod = new HTuple();
            HTuple hv__bondImageIdx = new HTuple(), hv_BondOnWhat = new HTuple();
            HTuple hv_BondInspectMethod = new HTuple(), hv__measureType = new HTuple();
            HTuple hv__metrologyHandle = new HTuple(), hv__BondOffsetFactor = new HTuple();
            HTuple hv__BondOverSizeFactor = new HTuple(), hv__BondUnderSizeFactor = new HTuple();
            HTuple hv__isPreJudge = new HTuple(), hv__ThreshGray = new HTuple();
            HTuple hv__SegRegAreaFactor = new HTuple(), hv__ClosingSize = new HTuple();
            HTuple hv__DefectTypeReg = new HTuple(), hv__DefectImgIdxReg = new HTuple();
            HTuple hv__ErrRegCode = new HTuple(), hv__ErrRegStr = new HTuple();
            HTuple hv__ErrBondCode = new HTuple(), hv__ErrBondStr = new HTuple();
            HTuple hv__modelType = new HTuple(), hv__modelID = new HTuple(), hv_inspectRegNum = new HTuple();
            HTuple hv__ballNum_onReg = new HTuple(), hv__matchAngleStart = new HTuple();
            HTuple hv__matchAngleExt = new HTuple(), hv__matchMinScore = new HTuple();
            HTuple hv__BondSize = new HTuple(),hv__bondSize = new HTuple(),hv__IsBondRegRefine = new HTuple();
            HTuple hv__AddNum = new HTuple(), hv__OverLap = new HTuple();
            HTuple hv__MinHistScore = new HTuple(), hv__BondParas = new HTuple();
            HTuple hv_tmp_num = new HTuple(), hv__IsUseStopReg = new HTuple();
            HTuple hv_StopRegBelongToWhat = new HTuple(), hv__StopHomMat2D = new HTuple();
            HTuple hv_Startreg_index_after_sort = new HTuple(), hv_Stopreg_index_after_sort = new HTuple();
            HTuple hv_Startreg_need_sort = new HTuple(), hv_Stopreg_need_sort = new HTuple();
            HTuple hv_start_regnum = new HTuple(), hv_sort_num = new HTuple();
            HTuple hv_ind = new HTuple(), hv_tmp_ind = new HTuple();
            HTuple hv_stop_regnum = new HTuple(), hv_start_num = new HTuple();
            HTuple hv_i = new HTuple(), hv_i_index = new HTuple();
            HTuple hv_first_ind = new HTuple(), hv_j = new HTuple();
            HTuple hv_end_ind = new HTuple(), hv__StartHomMat2D_tmp = new HTuple();
            HTuple hv_new_j = new HTuple(), hv_StartRegArea = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_stop_num = new HTuple(), hv__StopHomMat2D_tmp = new HTuple();
            HTuple hv_StopRegArea = new HTuple(), hv__InspectRegNum = new HTuple();
            HTuple hv__wireErrCode = new HTuple();
            HTuple hv__wireErrStr = new HTuple(), hv_failNum = new HTuple();
            HTuple hv_WireNum = new HTuple();
            HTuple LineRegionsNumbers = new HTuple();
            HTuple hv_idx = new HTuple(), hv_HomMatSum = new HTuple();
            HTuple hv_HomMatStartFlag = new HTuple(), hv_HomMatStopFlag = new HTuple();

            HTupleVector hv__DefectWireType = new HTupleVector(1);
            HTupleVector hv__DefectBondType = new HTupleVector(1);
            HTupleVector hvec_HomMatMod2_start_group = new HTupleVector(1);
            HTupleVector hvec_HomMatMod2_stop_group = new HTupleVector(1);
            HTupleVector hvec__BondModels = new HTupleVector(2);
            HTupleVector hvec__BondParameters = new HTupleVector(3);
            HTupleVector hvec__bondInspectModel = new HTupleVector(1);
            HTupleVector LineRegVerifyPara = new HTupleVector(1);
            HTupleVector LineRegVerifyParas = new HTupleVector(2);
            HTupleVector hvec__BondParas = new HTupleVector(2);
            HTupleVector hvec__RegInspectParas = new HTupleVector(1);
            HTupleVector hvec__DefectValueReg = new HTupleVector(1), hvec__RefValueReg = new HTupleVector(1);

            HOperatorSet.GenEmptyObj(out HObject LineRegions);
            HOperatorSet.GenEmptyObj(out ho_realStartBall);
            HOperatorSet.GenEmptyObj(out ho__bondInspectReg);
            HOperatorSet.GenEmptyObj(out ho__startBondContours);
            HOperatorSet.GenEmptyObj(out ho__ModelImg);
            HOperatorSet.GenEmptyObj(out ho__BondInspectRegs);
            HOperatorSet.GenEmptyObj(out ho__BondInspectReg);
            HOperatorSet.GenEmptyObj(out ho__startBondContour);
            HOperatorSet.GenEmptyObj(out ho__FailBondReg);
            HOperatorSet.GenEmptyObj(out ho_realStopBall);
            HOperatorSet.GenEmptyObj(out ho__stopBondContour);
            HOperatorSet.GenEmptyObj(out ho__stopBondContours);
            HOperatorSet.GenEmptyObj(out ho_realStartBall_sort);
            HOperatorSet.GenEmptyObj(out ho_realStopBall_sort);
            HOperatorSet.GenEmptyObj(out ho_tmp_reg);
            HOperatorSet.GenEmptyObj(out ho_virtualStartBall1);
            HOperatorSet.GenEmptyObj(out ho_virtualStopBall1);
            HOperatorSet.GenEmptyObj(out ho_virtualStartBall2);
            HOperatorSet.GenEmptyObj(out ho_virtualStopBall2);
            HOperatorSet.GenEmptyObj(out ho_virtualStartBall3);
            HOperatorSet.GenEmptyObj(out ho_virtualStopBall3);
            HOperatorSet.GenEmptyObj(out ho__LineStartRegs);
            HOperatorSet.GenEmptyObj(out ho__LineStopRegs);
            HOperatorSet.GenEmptyObj(out ho__tmp_reg_aff);
            HOperatorSet.GenEmptyObj(out ho_tmpvirtualStartreg);
            HOperatorSet.GenEmptyObj(out ho_tmprealStartReg);
            HOperatorSet.GenEmptyObj(out ho_tmpvirtualStopreg);
            HOperatorSet.GenEmptyObj(out ho_tmprealStopReg);

            try
            {
                //***********************************************************************************
                //获取金线起始焊点是否启用：1-启用，使用虚拟焊点；0-不启用，使用真实焊点
                hv__IsUseStartReg = WireParameter.IsEnableStartVirtualBond ? 1 : 0;
                //
                if ((int)(hv__IsUseStartReg) != 0)
                {
                    //------------------使用虚拟焊点
                    //确定检测区域检测项隶属于哪个大区域,
                    hv_StartRegBelongToWhat = WireParameter.StartBondonRecipesIndexs;
                    //
                    //1104
                    if ((int)(new HTuple((new HTuple(hv_StartRegBelongToWhat.TupleLength()
                        )).TupleNotEqual(1))) != 0)
                    {
                        hv_o_ErrCode = -1;
                        hv_o_ErrStr = "start region with err info";
                        ho_realStartBall.Dispose();
                        ho__bondInspectReg.Dispose();
                        ho__startBondContours.Dispose();
                        ho__FailBondRegs.Dispose();
                        ho__ModelImg.Dispose();
                        ho__BondInspectRegs.Dispose();
                        ho__BondInspectReg.Dispose();
                        ho__startBondContour.Dispose();
                        ho__FailBondReg.Dispose();
                        ho_realStopBall.Dispose();
                        ho__stopBondContour.Dispose();
                        ho__stopBondContours.Dispose();
                        ho_realStartBall_sort.Dispose();
                        ho_realStopBall_sort.Dispose();
                        ho_tmp_reg.Dispose();
                        ho_virtualStartBall1.Dispose();
                        ho_virtualStopBall1.Dispose();
                        ho_virtualStartBall2.Dispose();
                        ho_virtualStopBall2.Dispose();
                        ho_virtualStartBall3.Dispose();
                        ho_virtualStopBall3.Dispose();
                        ho__LineStartRegs.Dispose();
                        ho__LineStopRegs.Dispose();
                        ho__tmp_reg_aff.Dispose();
                        ho_tmpvirtualStartreg.Dispose();
                        ho_tmprealStartReg.Dispose();
                        ho_tmpvirtualStopreg.Dispose();
                        ho_tmprealStopReg.Dispose();
                        hvec__BondObjs.Dispose();
                        hvec__bondInspectObj.Dispose();

                        return;
                    }
                    //获取bond检测需要的映射矩阵
                    Algorithm.Model_RegionAlg.HTV_confirm_mapping_matrix(Algorithm.Region.GetChannnelImageConcact(ImageVerify),//黑白图传1张；彩色图RGB三通道图concact在一起
                                                                DieRegions.SelectObj(imgIndex + 1),
                                                                ModelsFile,
                                                                RecipeFile,
                                                                hv_StartRegBelongToWhat,
                                                                out hv__StartHomMat2D,
                                                                out HTuple _frameLocPara,
                                                                out hv__ErrCode,
                                                                out hv__ErrStr);

                    hvec_HomMatMod2_start_group[0] = new HTupleVector(hv__StartHomMat2D).Clone();

                    //真实焊点置空
                    HOperatorSet.GenEmptyObj(out ho_realStartBall);

                }
                else
                {
                    //
                    HOperatorSet.GenEmptyObj(out ho_realStartBall);
                    //-------------------使用真实焊点
                    //获取起始焊点归属
                    hv_StartRegBelongToWhat = WireParameter.StartBondonRecipesIndexs;
                    //
                    for (hv_ii = 0; (int)hv_ii <= (int)((new HTuple(hv_StartRegBelongToWhat.TupleLength()
                        )) - 1); hv_ii = (int)hv_ii + 1)
                    {
                        //读取真实焊点检测model及参数
                        hv__BondItem = (hv_StartRegBelongToWhat.TupleSelect(hv_ii)) + "/";

                        //model下参数
                        hv__BondModelPath = ModelsFile + hv__BondItem;
                        hvec__BondObjs.Dispose();
                        Algorithm.Model_RegionAlg.HTV_read_bond_model(out hvec__BondObjs, hv__BondModelPath, out hvec__BondModels,
                            out hv__modelErrCode, out hv__modelErrStr);
                        //recipe下参数
                        hv__BondRecipePath = RecipeFile + hv__BondItem;
                        Algorithm.Model_RegionAlg.HTV_read_bond_recipe(hv__BondRecipePath, out hvec__BondParameters,
                            out hv__recipeErrCode, out hv__recipeErrStr);

                        //------------------开始检测真实起始焊点
                        //-------获取bond检测model参数
                        hvec__bondInspectModel = hvec__BondModels[0];
                        //选择bond所属区域
                        hv_BondOnWhat = hvec__bondInspectModel[0].T.Clone();

                        //获取bond检测需要的映射矩阵
                        Algorithm.Model_RegionAlg.HTV_confirm_mapping_matrix(Algorithm.Region.GetChannnelImageConcact(ImageVerify),//黑白图传1张；彩色图RGB三通道图concact在一起,
                                                                    DieRegions.SelectObj(imgIndex + 1),
                                                                    ModelsFile,
                                                                    RecipeFile,
                                                                    hv_BondOnWhat,
                                                                    out hv__StartHomMat2D,
                                                                    out HTuple _frameLocPara,
                                                                    out hv__ErrCode, out hv__ErrStr);

                        hvec_HomMatMod2_start_group[hv_ii] = new HTupleVector(hv__StartHomMat2D).Clone();
                        //
                        //
                        //获取bond模板类型：0-match;1-measure
                        hv_BondInspectMethod = hvec__bondInspectModel[1].T.Clone();
                        //输出类型带入Wire检测
                        if ((int)(new HTuple(hv_BondInspectMethod.TupleEqual(1))) != 0)
                        {
                            //
                            //获取检测测量模板参数
                            hv__measureType = hvec__bondInspectModel[2].T.Clone();
                            hv__metrologyHandle = hvec__bondInspectModel[3].T.Clone();
                        }
                        else
                        {
                            //
                            //模板旋正图像
                            ho__ModelImg.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho__ModelImg = hvec__bondInspectObj[1].O.CopyObj(1, -1);
                            }
                            //
                            hv__modelType = hvec__bondInspectModel[2].T.Clone();
                            hv__modelID = hvec__bondInspectModel[3].T.Clone();

                        }
                        //-----获取Bond检测参数
                        hvec__BondParas = hvec__BondParameters[0];
                        //
                        //***************************************************************************
                        //--------获取bond检测对象参数
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hvec__bondInspectObj = dh.Take(hvec__BondObjs[0]);
                        }
                        //检测区域
                        //ho__bondInspectReg.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho__bondInspectReg = hvec__bondInspectObj[0].O.CopyObj(1, -1);
                        }
                        //*****************************************************************************
                        //
                        if ((int)(new HTuple(hv__ErrCode.TupleEqual(0))) != 0)
                        {
                            //
                            //*******************************************检测区域内选择合适方法进行检测*************************************
                            //获取焊点映射后的检测区域
                            HOperatorSet.AffineTransRegion(ho__bondInspectReg, out ho__BondInspectRegs,
                                hv__StartHomMat2D, "nearest_neighbor");
                            HOperatorSet.CountObj(ho__BondInspectRegs, out hv_inspectRegNum);
                            //初始化焊点区域
                            HOperatorSet.GenEmptyObj(out ho__startBondContours);
                            //
                            HTuple end_val175 = hv_inspectRegNum - 1;
                            HTuple step_val175 = 1;
                            for (hv_regidx = 0; hv_regidx.Continue(end_val175, step_val175); hv_regidx = hv_regidx.TupleAdd(step_val175))
                            {
                                HOperatorSet.SelectObj(ho__BondInspectRegs, out ho__BondInspectReg,
                                    hv_regidx + 1);
                                //获取区域内检测参数
                                hvec__RegInspectParas = hvec__BondParas[hv_regidx];
                                //获取bond检测区域内检测方法:0-threshold,1-measure,2-match
                                hv__InspectMethod = hvec__RegInspectParas[0].T.Clone();
                                //
                                switch (hv__InspectMethod.I)
                                {
                                    case 0:
                                        //----阈值分割参数获取
                                        hv__bondImageIdx = hvec__RegInspectParas[1].T.Clone();
                                        hv__BondSize = hvec__RegInspectParas[2].T.Clone();
                                        hv__ThreshGray = hvec__RegInspectParas[3].T.Clone();
                                        hv__ClosingSize = hvec__RegInspectParas[4].T.Clone();
                                        hv__BondOverSizeFactor = hvec__RegInspectParas[5].T.Clone();
                                        hv__BondUnderSizeFactor = hvec__RegInspectParas[6].T.Clone();
                                        //
                                        //ho__startBondContour.Dispose(); ho__FailBondReg.Dispose();
                                        Algorithm.Model_RegionAlg.HTV_Bond_Inspect_threshold(Algorithm.Region.GetChannnelImageConcact(ImageVerify), ho__BondInspectReg, out ho__startBondContour,
                                            out ho__FailBondReg, hv__bondImageIdx, hv__BondSize, hv__ThreshGray,
                                            hv__ClosingSize, hv__BondOverSizeFactor, hv__BondUnderSizeFactor,
                                            out hv__DefectTypeReg, out hv__DefectImgIdxReg, out hvec__DefectValueReg,
                                            out hvec__RefValueReg, out hv__ErrRegCode, out hv__ErrRegStr);
                                        //
                                        break;
                                    case 1:
                                        //---测量模板参数获取
                                        hv__bondImageIdx = hvec__RegInspectParas[1].T.Clone();
                                        hv__BondOverSizeFactor = hvec__RegInspectParas[2].T.Clone();
                                        hv__BondUnderSizeFactor = hvec__RegInspectParas[3].T.Clone();
                                        hv__isPreJudge = hvec__RegInspectParas[4].T.Clone();
                                        hv__ThreshGray = hvec__RegInspectParas[5].T.Clone();
                                        hv__SegRegAreaFactor = hvec__RegInspectParas[6].T.Clone();
                                        //
                                        //开始检测
                                        //ho__startBondContour.Dispose(); ho__FailBondReg.Dispose();
                                        Algorithm.Model_RegionAlg.HTV_Bond_Inspect_measure(Algorithm.Region.GetChannnelImageConcact(ImageVerify), ho__BondInspectReg, out ho__startBondContour,
                                            out ho__FailBondReg, hv__StartHomMat2D, hv_regidx, hv__bondImageIdx,
                                            hv__measureType, hv__metrologyHandle, hv__BondOverSizeFactor,
                                            hv__BondUnderSizeFactor, hv__isPreJudge, hv__ThreshGray,
                                            hv__SegRegAreaFactor, out hv__DefectTypeReg, out hv__DefectImgIdxReg,
                                            out hvec__DefectValueReg, out hvec__RefValueReg, out hv__ErrRegCode,
                                            out hv__ErrRegStr);
                                        //

                                        break;
                                    case 2:
                                        //---匹配模板参数获取
                                        hv__bondImageIdx = hvec__RegInspectParas[1].T.Clone();
                                        hv__ballNum_onReg = hvec__RegInspectParas[2].T.Clone();
                                        hv__matchMinScore = hvec__RegInspectParas[3].T.Clone();
                                        hv__matchAngleStart = hvec__RegInspectParas[4].T.Clone();
                                        hv__matchAngleExt = hvec__RegInspectParas[5].T.Clone();
                                        hv__bondSize = hvec__RegInspectParas[6].T.Clone();
                                        hv__IsBondRegRefine = hvec__RegInspectParas[7].T.Clone();
                                        hv__AddNum = hvec__RegInspectParas[8].T.Clone();
                                        hv__OverLap = hvec__RegInspectParas[9].T.Clone();
                                        hv__MinHistScore = hvec__RegInspectParas[10].T.Clone();
                                        //
                                        //开始检测
                                        //ho__startBondContour.Dispose(); ho__FailBondReg.Dispose();
                                        Algorithm.Model_RegionAlg.HTV_Bond_Inspect_model(Algorithm.Region.GetChannnelImageConcact(ImageVerify), ho__ModelImg, ho__BondInspectReg,
                                            out ho__startBondContour, out ho__FailBondReg, hv__bondImageIdx,
                                            hv__ballNum_onReg, hv__modelType, hv__modelID, hv__matchMinScore,
                                            hv__matchAngleStart, hv__matchAngleExt, hv__bondSize, hv__IsBondRegRefine,
                                            hv__AddNum, hv__OverLap, hv__MinHistScore, out hv__DefectTypeReg,
                                            out hv__DefectImgIdxReg, out hvec__DefectValueReg, out hvec__RefValueReg,
                                            out hv__ErrRegCode, out hv__ErrRegStr);
                                        break;
                                }
                                //整合区域检测结果
                                //
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho__startBondContours, ho__startBondContour,
                                        out ExpTmpOutVar_0);
                                    ho__startBondContours.Dispose();
                                    ho__startBondContours = ExpTmpOutVar_0;
                                }
                            }
                            HOperatorSet.CountObj(ho__startBondContours, out hv_tmp_num);
                            if (hv_Number_start_group == null)
                                hv_Number_start_group = new HTuple();
                            hv_Number_start_group[hv_ii] = hv_tmp_num;
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_realStartBall, ho__startBondContours, out ExpTmpOutVar_0);
                                ho_realStartBall.Dispose();
                                ho_realStartBall = ExpTmpOutVar_0;
                            }
                        }
                        else
                        {
                            MessageBox.Show("金线起始端点定位不到！");
                            imgIndex++;
                            if (imgIndex + 1 > DieRegions.CountObj())
                            {
                                imgIndex = 0;
                            }
                            return;
                        }

                        //
                        //-------------------------清除模板
                        if ((int)(new HTuple(hv_BondInspectMethod.TupleEqual(1))) != 0)
                        {
                            //清除测量模板
                            HOperatorSet.ClearMetrologyModel(hv__metrologyHandle);
                        }
                        else
                        {
                            //清除ncc模板
                            Algorithm.Model_RegionAlg.HTV_clear_model(hv__modelID, hv__modelType);
                        }
                        //--------------------------------
                    }
                }
                //
                //***************************************************************************************
                //获取金线结束焊点是否启用：1-启用，使用虚拟焊点；0-不启用，使用真实焊点
                hv__IsUseStopReg = WireParameter.IsEnableEndVirtualBond ? 1 : 0;
                if ((int)(hv__IsUseStopReg) != 0)
                {
                    //----------------使用虚拟焊点
                    //获取结束点归属区域
                    hv_StopRegBelongToWhat = WireParameter.StopBondonRecipesIndexs;
                    //
                    //1104
                    if ((int)(new HTuple((new HTuple(hv_StopRegBelongToWhat.TupleLength()
                        )).TupleNotEqual(1))) != 0)
                    {
                        hv_o_ErrCode = -1;
                        hv_o_ErrStr = "stop region with err info";
                        ho_realStartBall.Dispose();
                        ho__bondInspectReg.Dispose();
                        ho__BondInspectReg.Dispose();
                        ho__BondInspectRegs.Dispose();
                        ho__startBondContours.Dispose();
                        ho__startBondContour.Dispose();
                        ho__FailBondRegs.Dispose();
                        ho__FailBondReg.Dispose();
                        ho__ModelImg.Dispose();
                        ho_realStopBall.Dispose();
                        ho__stopBondContour.Dispose();
                        ho__stopBondContours.Dispose();
                        ho_realStartBall_sort.Dispose();
                        ho_realStopBall_sort.Dispose();
                        ho_tmp_reg.Dispose();
                        ho_virtualStartBall1.Dispose();
                        ho_virtualStopBall1.Dispose();
                        ho_virtualStartBall2.Dispose();
                        ho_virtualStopBall2.Dispose();
                        ho_virtualStartBall3.Dispose();
                        ho_virtualStopBall3.Dispose();
                        ho__LineStartRegs.Dispose();
                        ho__LineStopRegs.Dispose();
                        ho__tmp_reg_aff.Dispose();
                        ho_tmpvirtualStartreg.Dispose();
                        ho_tmprealStartReg.Dispose();
                        ho_tmpvirtualStopreg.Dispose();
                        ho_tmprealStopReg.Dispose();
                        hvec__BondObjs.Dispose();
                        hvec__bondInspectObj.Dispose();

                        return;
                    }
                    //获取bond检测需要的映射矩阵
                    Algorithm.Model_RegionAlg.HTV_confirm_mapping_matrix(Algorithm.Region.GetChannnelImageConcact(ImageVerify),//黑白图传1张；彩色图RGB三通道图concact在一起
                                                                DieRegions.SelectObj(imgIndex + 1),
                                                                ModelsFile,
                                                                RecipeFile,
                                                                hv_StopRegBelongToWhat,
                                                                out hv__StopHomMat2D,
                                                                out HTuple _frameLocPara,
                                                                out hv__ErrCode, out hv__ErrStr);

                    hvec_HomMatMod2_stop_group[0] = new HTupleVector(hv__StopHomMat2D).Clone();

                    //置空结束焊点区域
                    HOperatorSet.GenEmptyObj(out ho_realStopBall);

                }
                else
                {
                    //
                    HOperatorSet.GenEmptyObj(out ho_realStopBall);
                    //----------------使用真实检测到的焊点
                    //获取结束点归属区域
                    hv_StopRegBelongToWhat = WireParameter.StopBondonRecipesIndexs;
                    //
                    for (hv_ii = 0; (int)hv_ii <= (int)((new HTuple(hv_StopRegBelongToWhat.TupleLength()
                        )) - 1); hv_ii = (int)hv_ii + 1)
                    {
                        //读取真实焊点检测model及参数
                        hv__BondItem = (hv_StopRegBelongToWhat.TupleSelect(hv_ii)) + "/";
                        //model下参数
                        hv__BondModelPath = ModelsFile + hv__BondItem;
                        hvec__BondObjs.Dispose();
                        Algorithm.Model_RegionAlg.HTV_read_bond_model(out hvec__BondObjs, hv__BondModelPath, out hvec__BondModels,
                            out hv__modelErrCode, out hv__modelErrStr);
                        //recipe下参数
                        hv__BondRecipePath = RecipeFile + hv__BondItem;
                        Algorithm.Model_RegionAlg.HTV_read_bond_recipe(hv__BondRecipePath, out hvec__BondParameters,
                            out hv__recipeErrCode, out hv__recipeErrStr);
                        //------------------开始检测真实结束焊点
                        hvec__bondInspectModel = hvec__BondModels[0];
                        //选择bond所属区域
                        hv_BondOnWhat = hvec__bondInspectModel[0].T.Clone();
                        //获取bond检测需要的映射矩阵
                        Algorithm.Model_RegionAlg.HTV_confirm_mapping_matrix(Algorithm.Region.GetChannnelImageConcact(ImageVerify),//黑白图传1张；彩色图RGB三通道图concact在一起,
                                                                    DieRegions.SelectObj(imgIndex + 1),
                                                                    ModelsFile,
                                                                    RecipeFile,
                                                                    hv_BondOnWhat,
                                                                    out hv__StopHomMat2D,
                                                                    out HTuple _frameLocPara,
                                                                    out hv__ErrCode, out hv__ErrStr);

                        hvec_HomMatMod2_stop_group[hv_ii] = new HTupleVector(hv__StopHomMat2D).Clone();
                        //
                        //获取bond模板类型：0-match;1-measure
                        hv_BondInspectMethod = hvec__bondInspectModel[1].T.Clone();
                        //输出类型带入Wire检测
                        if ((int)(new HTuple(hv_BondInspectMethod.TupleEqual(1))) != 0)
                        {
                            //
                            //获取检测测量模板参数
                            hv__measureType = hvec__bondInspectModel[2].T.Clone();
                            hv__metrologyHandle = hvec__bondInspectModel[3].T.Clone();
                        }
                        else
                        {
                            //
                            //模板旋正图像
                            ho__ModelImg.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho__ModelImg = hvec__bondInspectObj[1].O.CopyObj(1, -1);
                            }
                            //
                            hv__modelType = hvec__bondInspectModel[2].T.Clone();
                            hv__modelID = hvec__bondInspectModel[3].T.Clone();

                        }
                        //-----获取Bond检测参数
                        hvec__BondParas = hvec__BondParameters[0];
                        //
                        //***************************************************************************************

                        //--------获取bond检测对象参数
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hvec__bondInspectObj = dh.Take(hvec__BondObjs[0]);
                        }
                        //检测区域
                        ho__bondInspectReg.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho__bondInspectReg = hvec__bondInspectObj[0].O.CopyObj(1, -1);
                        }
                        //***************************************************************************************

                        if ((int)(new HTuple(hv__ErrCode.TupleEqual(0))) != 0)
                        {
                            //*******************************************检测区域内选择合适方法进行检测*************************************
                            //获取焊点映射后的检测区域
                            HOperatorSet.AffineTransRegion(ho__bondInspectReg, out ho__BondInspectRegs,
                                hv__StopHomMat2D, "nearest_neighbor");
                            HOperatorSet.CountObj(ho__BondInspectRegs, out hv_inspectRegNum);
                            //初始化焊点区域
                            HOperatorSet.GenEmptyObj(out ho__stopBondContours);
                            //
                            HTuple end_val334 = hv_inspectRegNum - 1;
                            HTuple step_val334 = 1;
                            for (hv_regidx = 0; hv_regidx.Continue(end_val334, step_val334); hv_regidx = hv_regidx.TupleAdd(step_val334))
                            {
                                ho__BondInspectReg.Dispose();
                                HOperatorSet.SelectObj(ho__BondInspectRegs, out ho__BondInspectReg,
                                    hv_regidx + 1);
                                //获取区域内检测参数
                                hvec__RegInspectParas = hvec__BondParas[hv_regidx];
                                //获取bond检测区域内检测方法:0-threshold,1-measure,2-match
                                hv__InspectMethod = hvec__RegInspectParas[0].T.Clone();
                                //
                                switch (hv__InspectMethod.I)
                                {
                                    case 0:
                                        //----阈值分割参数获取
                                        hv__bondImageIdx = hvec__RegInspectParas[1].T.Clone();
                                        hv__BondSize = hvec__RegInspectParas[2].T.Clone();
                                        hv__ThreshGray = hvec__RegInspectParas[3].T.Clone();
                                        hv__ClosingSize = hvec__RegInspectParas[4].T.Clone();
                                        hv__BondOverSizeFactor = hvec__RegInspectParas[5].T.Clone();
                                        hv__BondUnderSizeFactor = hvec__RegInspectParas[6].T.Clone();
                                        //
                                        //ho__stopBondContour.Dispose(); ho__FailBondReg.Dispose();
                                        Algorithm.Model_RegionAlg.HTV_Bond_Inspect_threshold(Algorithm.Region.GetChannnelImageConcact(ImageVerify), ho__BondInspectReg, out ho__stopBondContour,
                                            out ho__FailBondReg, hv__bondImageIdx, hv__BondSize, hv__ThreshGray,
                                            hv__ClosingSize, hv__BondOverSizeFactor, hv__BondUnderSizeFactor,
                                            out hv__DefectTypeReg, out hv__DefectImgIdxReg, out hvec__DefectValueReg,
                                            out hvec__RefValueReg, out hv__ErrRegCode, out hv__ErrRegStr);
                                        //
                                        break;
                                    case 1:
                                        //---测量模板参数获取
                                        hv__bondImageIdx = hvec__RegInspectParas[1].T.Clone();
                                        hv__BondOverSizeFactor = hvec__RegInspectParas[2].T.Clone();
                                        hv__BondUnderSizeFactor = hvec__RegInspectParas[3].T.Clone();
                                        hv__isPreJudge = hvec__RegInspectParas[4].T.Clone();
                                        hv__ThreshGray = hvec__RegInspectParas[5].T.Clone();
                                        hv__SegRegAreaFactor = hvec__RegInspectParas[6].T.Clone();
                                        //
                                        //开始检测
                                        //ho__stopBondContour.Dispose(); ho__FailBondReg.Dispose();
                                        Algorithm.Model_RegionAlg.HTV_Bond_Inspect_measure(Algorithm.Region.GetChannnelImageConcact(ImageVerify), ho__BondInspectReg, out ho__stopBondContour,
                                            out ho__FailBondReg, hv__StopHomMat2D, hv_regidx, hv__bondImageIdx,
                                            hv__measureType, hv__metrologyHandle, hv__BondOverSizeFactor,
                                            hv__BondUnderSizeFactor, hv__isPreJudge, hv__ThreshGray,
                                            hv__SegRegAreaFactor, out hv__DefectTypeReg, out hv__DefectImgIdxReg,
                                            out hvec__DefectValueReg, out hvec__RefValueReg, out hv__ErrRegCode,
                                            out hv__ErrRegStr);
                                        //

                                        break;
                                    case 2:
                                        //---匹配模板参数获取
                                        hv__bondImageIdx = hvec__RegInspectParas[1].T.Clone();
                                        hv__ballNum_onReg = hvec__RegInspectParas[2].T.Clone();
                                        hv__matchMinScore = hvec__RegInspectParas[3].T.Clone();
                                        hv__matchAngleStart = hvec__RegInspectParas[4].T.Clone();
                                        hv__matchAngleExt = hvec__RegInspectParas[5].T.Clone();
                                        hv__bondSize = hvec__RegInspectParas[6].T.Clone();
                                        hv__IsBondRegRefine = hvec__RegInspectParas[7].T.Clone();
                                        hv__AddNum = hvec__RegInspectParas[8].T.Clone();
                                        hv__OverLap = hvec__RegInspectParas[9].T.Clone();
                                        hv__MinHistScore = hvec__RegInspectParas[10].T.Clone();
                                        //
                                        //开始检测
                                        //ho__stopBondContour.Dispose(); ho__FailBondReg.Dispose();
                                        Algorithm.Model_RegionAlg.HTV_Bond_Inspect_model(Algorithm.Region.GetChannnelImageConcact(ImageVerify), ho__ModelImg, ho__BondInspectReg,
                                            out ho__stopBondContour, out ho__FailBondReg, hv__bondImageIdx,
                                            hv__ballNum_onReg, hv__modelType, hv__modelID, hv__matchMinScore,
                                            hv__matchAngleStart, hv__matchAngleExt, hv__bondSize, hv__IsBondRegRefine,
                                            hv__AddNum, hv__OverLap, hv__MinHistScore, out hv__DefectTypeReg,
                                            out hv__DefectImgIdxReg, out hvec__DefectValueReg, out hvec__RefValueReg,
                                            out hv__ErrRegCode, out hv__ErrRegStr);
                                        break;
                                }
                                //整合区域检测结果
                                //
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho__stopBondContours, ho__stopBondContour,
                                        out ExpTmpOutVar_0);
                                    ho__stopBondContours.Dispose();
                                    ho__stopBondContours = ExpTmpOutVar_0;
                                }
                            }
                            //整合区域检测结果
                            HOperatorSet.CountObj(ho__stopBondContours, out hv_tmp_num);
                                if (hv_Number_stop_group == null)
                                    hv_Number_stop_group = new HTuple();
                                hv_Number_stop_group[hv_ii] = hv_tmp_num;
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho_realStopBall, ho__stopBondContours, out ExpTmpOutVar_0
                                        );
                                    ho_realStopBall.Dispose();
                                    ho_realStopBall = ExpTmpOutVar_0;
                                }
                            
                        }
                        else
                        {
                            MessageBox.Show("金线结束端点定位不到！");
                            imgIndex++;
                            if (imgIndex + 1 > DieRegions.CountObj())
                            {
                                imgIndex = 0;
                            }
                            return;
                        }
                        //
                        //-------------------------清除模板
                        if ((int)(new HTuple(hv_BondInspectMethod.TupleEqual(1))) != 0)
                        {
                            //清除测量模板
                            HOperatorSet.ClearMetrologyModel(hv__metrologyHandle);
                        }
                        else
                        {
                            //清除ncc模板
                            Algorithm.Model_RegionAlg.HTV_clear_model(hv__modelID, hv__modelType);
                        }
                        //--------------------------------
                    }
                }
                //********************************区域内金线检测*******************************************************************
                //
                //2020-1104 根据组合后排序信息重新计算虚拟焊点和真实焊点
                HOperatorSet.GenEmptyObj(out ho_realStartBall_sort);
                HOperatorSet.GenEmptyObj(out ho_realStopBall_sort);

                HTuple ni;
                ni = 0;
                bool start_group_sort = false;
                foreach (var start_reg in StartBallAutoUserRegion)
                {
                    hv_Startreg_index_after_sort[ni] = start_reg.Index_ini;
                    ni++;
                    if (ni != start_reg.Index_ini)
                    {
                        start_group_sort = true;
                    }
                }

                hv_Startreg_need_sort = start_group_sort == false ? 0 : 1;

                ni = 0;
                bool stop_group_sort = false;
                foreach (var stop_reg in StopBallAutoUserRegion)
                {
                    hv_Stopreg_index_after_sort[ni] = stop_reg.Index_ini;
                    ni++;
                    if (ni != stop_reg.Index_ini)
                    {
                        stop_group_sort = true;
                    }
                }

                hv_Stopreg_need_sort = stop_group_sort == false ? 0 : 1;

                if ((int)(hv__IsUseStartReg) != 0)
                {
                    hv_Startreg_need_sort = 0;
                }
                if ((int)(hv__IsUseStopReg) != 0)
                {
                    hv_Stopreg_need_sort = 0;
                }
                //
                //1- 根据组合排序信息编辑真实焊点
                //1-1 真实起始焊点区域排序编辑
                if ((int)(hv_Startreg_need_sort) != 0)
                {
                    HOperatorSet.CountObj(ho_realStartBall, out hv_start_regnum);
                    hv_sort_num = new HTuple(hv_Startreg_index_after_sort.TupleLength());
                    if ((int)(new HTuple(hv_start_regnum.TupleNotEqual(hv_sort_num))) != 0)
                    {
                        hv_o_ErrCode = -1;
                        hv_o_ErrStr = "number mismatch of start region and sort_vec";
                        ho_realStartBall.Dispose();
                        ho__bondInspectReg.Dispose();
                        ho__startBondContours.Dispose();
                        ho__FailBondRegs.Dispose();
                        ho__ModelImg.Dispose();
                        ho_realStopBall.Dispose();
                        ho__stopBondContours.Dispose();
                        ho_realStartBall_sort.Dispose();
                        ho_realStopBall_sort.Dispose();
                        ho_tmp_reg.Dispose();
                        ho_virtualStartBall1.Dispose();
                        ho_virtualStopBall1.Dispose();
                        ho_virtualStartBall2.Dispose();
                        ho_virtualStopBall2.Dispose();
                        ho_virtualStartBall3.Dispose();
                        ho_virtualStopBall3.Dispose();
                        ho__LineStartRegs.Dispose();
                        ho__LineStopRegs.Dispose();
                        ho__tmp_reg_aff.Dispose();
                        ho_tmpvirtualStartreg.Dispose();
                        ho_tmprealStartReg.Dispose();
                        ho_tmpvirtualStopreg.Dispose();
                        ho_tmprealStopReg.Dispose();
                        hvec__BondObjs.Dispose();
                        hvec__bondInspectObj.Dispose();

                        return;
                    }

                    HTuple end_val363 = hv_start_regnum - 1;
                    HTuple step_val363 = 1;
                    for (hv_ind = 0; hv_ind.Continue(end_val363, step_val363); hv_ind = hv_ind.TupleAdd(step_val363))
                    {
                        hv_tmp_ind = hv_Startreg_index_after_sort.TupleSelect(hv_ind);
                        ho_tmp_reg.Dispose();
                        HOperatorSet.SelectObj(ho_realStartBall, out ho_tmp_reg, hv_tmp_ind);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_realStartBall_sort, ho_tmp_reg, out ExpTmpOutVar_0
                                );
                            ho_realStartBall_sort.Dispose();
                            ho_realStartBall_sort = ExpTmpOutVar_0;
                        }
                    }
                }
                else
                {
                    ho_realStartBall_sort.Dispose();
                    ho_realStartBall_sort = ho_realStartBall.CopyObj(1, -1);
                }
                //1-2 真实结束焊点区域排序编辑
                if ((int)(hv_Stopreg_need_sort) != 0)
                {
                    HOperatorSet.CountObj(ho_realStopBall, out hv_stop_regnum);
                    hv_sort_num = new HTuple(hv_Stopreg_index_after_sort.TupleLength());
                    if ((int)(new HTuple(hv_stop_regnum.TupleNotEqual(hv_sort_num))) != 0)
                    {
                        hv_o_ErrCode = -1;
                        hv_o_ErrStr = "number mismatch of stop region and sort_vec";
                        ho_realStartBall.Dispose();
                        ho__bondInspectReg.Dispose();
                        ho__startBondContours.Dispose();
                        ho__FailBondRegs.Dispose();
                        ho__ModelImg.Dispose();
                        ho_realStopBall.Dispose();
                        ho__stopBondContours.Dispose();
                        ho_realStartBall_sort.Dispose();
                        ho_realStopBall_sort.Dispose();
                        ho_tmp_reg.Dispose();
                        ho_virtualStartBall1.Dispose();
                        ho_virtualStopBall1.Dispose();
                        ho_virtualStartBall2.Dispose();
                        ho_virtualStopBall2.Dispose();
                        ho_virtualStartBall3.Dispose();
                        ho_virtualStopBall3.Dispose();
                        ho__LineStartRegs.Dispose();
                        ho__LineStopRegs.Dispose();
                        ho__tmp_reg_aff.Dispose();
                        ho_tmpvirtualStartreg.Dispose();
                        ho_tmprealStartReg.Dispose();
                        ho_tmpvirtualStopreg.Dispose();
                        ho_tmprealStopReg.Dispose();
                        hvec__BondObjs.Dispose();
                        hvec__bondInspectObj.Dispose();

                        return;
                    }

                    HTuple end_val381 = hv_stop_regnum - 1;
                    HTuple step_val381 = 1;
                    for (hv_ind = 0; hv_ind.Continue(end_val381, step_val381); hv_ind = hv_ind.TupleAdd(step_val381))
                    {
                        hv_tmp_ind = hv_Stopreg_index_after_sort.TupleSelect(hv_ind);
                        ho_tmp_reg.Dispose();
                        HOperatorSet.SelectObj(ho_realStopBall, out ho_tmp_reg, hv_tmp_ind);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_realStopBall_sort, ho_tmp_reg, out ExpTmpOutVar_0
                                );
                            ho_realStopBall_sort.Dispose();
                            ho_realStopBall_sort = ExpTmpOutVar_0;
                        }
                    }
                }
                else
                {
                    ho_realStopBall_sort.Dispose();
                    ho_realStopBall_sort = ho_realStopBall.CopyObj(1, -1);
                }
                //2- 根据组合排序信息编辑虚拟焊点
                ho_virtualStartBall1.Dispose();
                HOperatorSet.GenEmptyObj(out ho_virtualStartBall1);
                ho_virtualStopBall1.Dispose();
                HOperatorSet.GenEmptyObj(out ho_virtualStopBall1);
                ho_virtualStartBall2.Dispose();
                HOperatorSet.GenEmptyObj(out ho_virtualStartBall2);
                ho_virtualStopBall2.Dispose();
                HOperatorSet.GenEmptyObj(out ho_virtualStopBall2);
                ho_virtualStartBall3.Dispose();
                HOperatorSet.GenEmptyObj(out ho_virtualStartBall3);
                ho_virtualStopBall3.Dispose();
                HOperatorSet.GenEmptyObj(out ho_virtualStopBall3);
                ho__LineStartRegs.Dispose();
                HOperatorSet.GenEmptyObj(out ho__LineStartRegs);
                ho__LineStopRegs.Dispose();
                HOperatorSet.GenEmptyObj(out ho__LineStopRegs);

                //判断映射矩阵是否有效  add_lw 1119
                hv_HomMatStartFlag = 1;
                HTuple end_val242 = new HTuple(hvec_HomMatMod2_start_group.Length);
                HTuple step_val242 = 1;
                for (hv_idx = 0; hv_idx.Continue(end_val242, step_val242); hv_idx = hv_idx.TupleAdd(step_val242))
                {
                    HOperatorSet.TupleSum(hvec_HomMatMod2_start_group[0].T, out hv_HomMatSum);
                    if ((int)(new HTuple(hv_HomMatSum.TupleEqual(-12))) != 0)
                    {
                        hv_HomMatStartFlag = 0;
                        break;
                    }
                }

                hv_HomMatStopFlag = 1;
                HTuple end_val251 = new HTuple(hvec_HomMatMod2_stop_group.Length);
                HTuple step_val251 = 1;
                for (hv_idx = 0; hv_idx.Continue(end_val251, step_val251); hv_idx = hv_idx.TupleAdd(step_val251))
                {
                    HOperatorSet.TupleSum(hvec_HomMatMod2_stop_group[0].T, out hv_HomMatSum);
                    if ((int)(new HTuple(hv_HomMatSum.TupleEqual(-12))) != 0)
                    {
                        hv_HomMatStopFlag = 0;
                        break;
                    }
                }

                if ((int)(hv_HomMatStartFlag.TupleAnd(hv_HomMatStopFlag)) != 0)
                {
                    //2-1 虚拟起始焊点区域排序编辑
                    if ((int)(hv__IsUseStartReg) != 0)
                    {
                        hv__StartHomMat2D = hvec_HomMatMod2_start_group[0].T.Clone();
                        ho__LineStartRegs.Dispose();
                        HOperatorSet.AffineTransRegion(Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnFrameUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                            out ho__LineStartRegs, hv__StartHomMat2D, "nearest_neighbor");
                    }
                    else
                    {
                        //用真实焊点时，计算真实焊点对应的虚拟焊点（真实焊点的搜索中心或者测量中心）
                        //step-1: 模板区域按照焊点组合重新排列（组合排序之前的原始顺序）
                        if ((int)(hv_Startreg_need_sort) != 0)
                        {
                            HOperatorSet.CountObj(Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnFrameUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                out hv_start_num);
                            HTuple end_val408 = hv_start_num;
                            HTuple step_val408 = 1;
                            for (hv_i = 1; hv_i.Continue(end_val408, step_val408); hv_i = hv_i.TupleAdd(step_val408))
                            {
                                HOperatorSet.TupleFind(hv_Startreg_index_after_sort, hv_i, out hv_i_index);
                                ho_tmp_reg.Dispose();
                                HOperatorSet.SelectObj(Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnFrameUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                    out ho_tmp_reg, hv_i_index + 1);
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho_virtualStartBall1, ho_tmp_reg, out ExpTmpOutVar_0
                                        );
                                    ho_virtualStartBall1.Dispose();
                                    ho_virtualStartBall1 = ExpTmpOutVar_0;
                                }
                            }
                        }
                        else
                        {
                            ho_virtualStartBall1.Dispose();
                            ho_virtualStartBall1 = Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnFrameUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)).CopyObj(1, -1);
                        }
                        //step-2:根据对应矩阵，转换对应的模板区域，生成位置变换后的虚拟焊点区域
                        hv_first_ind = 1;
                        for (hv_j = 0; (int)hv_j <= (int)((new HTuple(hv_Number_start_group.TupleLength()
                            )) - 1); hv_j = (int)hv_j + 1)
                        {

                            hv_end_ind = (hv_first_ind + (hv_Number_start_group.TupleSelect(hv_j))) - 1;
                            hv__StartHomMat2D_tmp = hvec_HomMatMod2_start_group[hv_j].T.Clone();

                            HTuple end_val423 = hv_end_ind;
                            HTuple step_val423 = 1;
                            for (hv_new_j = hv_first_ind; hv_new_j.Continue(end_val423, step_val423); hv_new_j = hv_new_j.TupleAdd(step_val423))
                            {
                                ho_tmp_reg.Dispose();
                                HOperatorSet.SelectObj(ho_virtualStartBall1, out ho_tmp_reg, hv_new_j);
                                ho__tmp_reg_aff.Dispose();
                                HOperatorSet.AffineTransRegion(ho_tmp_reg, out ho__tmp_reg_aff, hv__StartHomMat2D_tmp,
                                    "nearest_neighbor");
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho_virtualStartBall2, ho__tmp_reg_aff, out ExpTmpOutVar_0
                                        );
                                    ho_virtualStartBall2.Dispose();
                                    ho_virtualStartBall2 = ExpTmpOutVar_0;
                                }
                            }

                            hv_first_ind = hv_end_ind + 1;
                        }
                        //step-3:根据组合排序向量，对位置变换后的虚拟焊点区域进行重新排序，得到与真实焊点对应的虚拟焊点
                        HOperatorSet.CountObj(Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnFrameUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                            out hv_start_num);
                        HTuple end_val433 = hv_start_num;
                        HTuple step_val433 = 1;
                        for (hv_i = 1; hv_i.Continue(end_val433, step_val433); hv_i = hv_i.TupleAdd(step_val433))
                        {
                            hv_tmp_ind = hv_Startreg_index_after_sort.TupleSelect(hv_i - 1);
                            ho_tmpvirtualStartreg.Dispose();
                            HOperatorSet.SelectObj(ho_virtualStartBall2, out ho_tmpvirtualStartreg,
                                hv_tmp_ind);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_virtualStartBall3, ho_tmpvirtualStartreg,
                                    out ExpTmpOutVar_0);
                                ho_virtualStartBall3.Dispose();
                                ho_virtualStartBall3 = ExpTmpOutVar_0;
                            }

                            //根据真实焊点是否有计算结果，来选取合适的采用区域，有计算结果则采用真实焊点，无计算结果则采用变换后的虚拟焊点位置
                            ho_tmprealStartReg.Dispose();
                            HOperatorSet.SelectObj(ho_realStartBall_sort, out ho_tmprealStartReg,
                                hv_i);
                            //********判断真实焊点是否存在：是-启用真实焊点，否-启用虚拟焊点
                            HOperatorSet.AreaCenter(ho_tmprealStartReg, out hv_StartRegArea, out hv_Row,
                                out hv_Column);
                            if ((int)(new HTuple(hv_StartRegArea.TupleEqual(0))) != 0)
                            {
                                //启用虚拟焊点
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho__LineStartRegs, ho_tmpvirtualStartreg,
                                        out ExpTmpOutVar_0);
                                    ho__LineStartRegs.Dispose();
                                    ho__LineStartRegs = ExpTmpOutVar_0;
                                }
                            }
                            else
                            {
                                //启用真实焊点
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho__LineStartRegs, ho_tmprealStartReg, out ExpTmpOutVar_0
                                        );
                                    ho__LineStartRegs.Dispose();
                                    ho__LineStartRegs = ExpTmpOutVar_0;
                                }
                            }
                        }

                    }

                    //2-2 虚拟起始焊点区域排序编辑
                    if ((int)(hv__IsUseStopReg) != 0)
                    {
                        hv__StopHomMat2D = hvec_HomMatMod2_stop_group[0].T.Clone();
                        ho__LineStopRegs.Dispose();
                        HOperatorSet.AffineTransRegion(Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnICUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                            out ho__LineStopRegs, hv__StopHomMat2D, "nearest_neighbor");
                    }
                    else
                    {
                        //用真实焊点时，计算真实焊点对应的虚拟焊点（真实焊点的搜索中心或者测量中心）
                        //step-1: 模板区域按照焊点组合重新排列（组合排序之前的原始顺序）
                        if ((int)(hv_Stopreg_need_sort) != 0)
                        {
                            HOperatorSet.CountObj(Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnICUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                out hv_stop_num);
                            HTuple end_val462 = hv_stop_num;
                            HTuple step_val462 = 1;
                            for (hv_i = 1; hv_i.Continue(end_val462, step_val462); hv_i = hv_i.TupleAdd(step_val462))
                            {
                                HOperatorSet.TupleFind(hv_Stopreg_index_after_sort, hv_i, out hv_i_index);
                                ho_tmp_reg.Dispose();
                                HOperatorSet.SelectObj(Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnICUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                    out ho_tmp_reg, hv_i_index + 1);
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho_virtualStopBall1, ho_tmp_reg, out ExpTmpOutVar_0
                                        );
                                    ho_virtualStopBall1.Dispose();
                                    ho_virtualStopBall1 = ExpTmpOutVar_0;
                                }
                            }
                        }
                        else
                        {
                            ho_virtualStopBall1.Dispose();
                            ho_virtualStopBall1 = Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnICUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)).CopyObj(1, -1);
                        }
                        //step-2:根据对应矩阵，转换对应的模板区域，生成位置变换后的虚拟焊点区域
                        hv_first_ind = 1;
                        for (hv_j = 0; (int)hv_j <= (int)((new HTuple(hv_Number_stop_group.TupleLength()
                            )) - 1); hv_j = (int)hv_j + 1)
                        {

                            hv_end_ind = (hv_first_ind + (hv_Number_stop_group.TupleSelect(hv_j))) - 1;
                            hv__StopHomMat2D_tmp = hvec_HomMatMod2_stop_group[hv_j].T.Clone();

                            HTuple end_val477 = hv_end_ind;
                            HTuple step_val477 = 1;
                            for (hv_new_j = hv_first_ind; hv_new_j.Continue(end_val477, step_val477); hv_new_j = hv_new_j.TupleAdd(step_val477))
                            {
                                ho_tmp_reg.Dispose();
                                HOperatorSet.SelectObj(ho_virtualStopBall1, out ho_tmp_reg, hv_new_j);
                                ho__tmp_reg_aff.Dispose();
                                HOperatorSet.AffineTransRegion(ho_tmp_reg, out ho__tmp_reg_aff, hv__StopHomMat2D_tmp,
                                    "nearest_neighbor");
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho_virtualStopBall2, ho__tmp_reg_aff, out ExpTmpOutVar_0
                                        );
                                    ho_virtualStopBall2.Dispose();
                                    ho_virtualStopBall2 = ExpTmpOutVar_0;
                                }
                            }

                            hv_first_ind = hv_end_ind + 1;
                        }
                        //step-3:根据组合排序向量，对位置变换后的虚拟焊点区域进行重新排序，得到与真实焊点对应的虚拟焊点
                        HOperatorSet.CountObj(Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnICUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                            out hv_stop_num);
                        HTuple end_val487 = hv_stop_num;
                        HTuple step_val487 = 1;
                        for (hv_i = 1; hv_i.Continue(end_val487, step_val487); hv_i = hv_i.TupleAdd(step_val487))
                        {
                            hv_tmp_ind = hv_Stopreg_index_after_sort.TupleSelect(hv_i - 1);
                            ho_tmpvirtualStopreg.Dispose();
                            HOperatorSet.SelectObj(ho_virtualStopBall2, out ho_tmpvirtualStopreg,
                                hv_tmp_ind);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_virtualStopBall3, ho_tmpvirtualStopreg, out ExpTmpOutVar_0
                                    );
                                ho_virtualStopBall3.Dispose();
                                ho_virtualStopBall3 = ExpTmpOutVar_0;
                            }

                            //根据真实焊点是否有计算结果，来选取合适的采用区域，有计算结果则采用真实焊点，无计算结果则采用变换后的虚拟焊点位置
                            ho_tmprealStopReg.Dispose();
                            HOperatorSet.SelectObj(ho_realStopBall_sort, out ho_tmprealStopReg,
                                hv_i);
                            //********判断真实焊点是否存在：是-启用真实焊点，否-启用虚拟焊点
                            HOperatorSet.AreaCenter(ho_tmprealStopReg, out hv_StopRegArea, out hv_Row,
                                out hv_Column);
                            if ((int)(new HTuple(hv_StopRegArea.TupleEqual(0))) != 0)
                            {
                                //启用虚拟焊点
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho__LineStopRegs, ho_tmpvirtualStopreg, out ExpTmpOutVar_0
                                        );
                                    ho__LineStopRegs.Dispose();
                                    ho__LineStopRegs = ExpTmpOutVar_0;
                                }
                            }
                            else
                            {
                                //启用真实焊点
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ConcatObj(ho__LineStopRegs, ho_tmprealStopReg, out ExpTmpOutVar_0
                                        );
                                    ho__LineStopRegs.Dispose();
                                    ho__LineStopRegs = ExpTmpOutVar_0;
                                }
                            }
                        }
                    }
                }
                else
                { 
                    MessageBox.Show("框架或芯片定位失败！");
                }
                //
                //-------------获取金线检测区域内检测参数
                HTupleVector _VerifyPara = new HTupleVector(1);
                int idx = 0;
                foreach (var group in WireRegionsGroup)
                {
                    for (int i = 0; i < group.LineUserRegions.Count(); i++)
                    {
                        if (group.LineUserRegions[i].AlgoParameterIndex == 0)
                        {
                            // Reg2_InspectPara:= {  _InspectMethod,_ImgIdx，_ThreshGray,_LightOrDark,_ClosingSize,_WireWideth,_WireLenth,\ _WireArea}
                            //检测区域分方法分区域选择检测图层
                            HTuple h_ImgIdx = group.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ImageIndex + 1;
                            _VerifyPara = (((((((((new HTupleVector(1).Insert(0, new HTupleVector(0)))
                                .Insert(1, new HTupleVector(h_ImgIdx)))
                                .Insert(2, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ThreshGray)))
                                .Insert(3, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.LightOrDark)))
                                .Insert(4, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ClosingSize)))
                                .Insert(5, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireWidth)))
                                .Insert(6, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireLength)))
                                .Insert(7, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.WireArea)))
                                .Insert(8, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.DistTh)));
                        }
                        else if (group.LineUserRegions[i].AlgoParameterIndex == 1)
                        {
                            //Reg1_InspectPara := {_InspectMethod,_ImgIdx，_WireWidth,_WireContrast,_Transition,_SelMetric,_SelMin,\
                            //_SelMax,_LinePhiDiff,_MaxWireGap}

                            //检测区域分方法分区域选择检测图层
                            HTuple h_ImgIdx = group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.ImageIndex + 1;
                            _VerifyPara = (((((((((((new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(1))))
                                .Insert(1, new HTupleVector(h_ImgIdx)))
                                .Insert(2, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.WireWidth)))
                                .Insert(3, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.WireContrast)))
                                .Insert(4, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.LightOrDark)))
                                .Insert(5, new HTupleVector(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMetric[0])
                                                            .TupleConcat(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMetric[1])))))
                                .Insert(6, new HTupleVector(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMin[0])
                                                            .TupleConcat(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMin[1])))))
                                .Insert(7, new HTupleVector(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMax[0])
                                                            .TupleConcat(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.SelMax[1])))))
                                .Insert(8, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.LinePhiDiff)))
                                .Insert(9, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.MaxWireGap)))
                                .Insert(10, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoPara.DistTh)));
                        }
                        else if (group.LineUserRegions[i].AlgoParameterIndex == 2)
                        {
                            //Reg1_InspectPara := {_InspectMethod,_ImgIdx，_WireWidth,_WireContrast,_Transition,_SelMetric,_SelMin,\
                            //_SelMax,_LinePhiDiff,_MaxWireGap}

                            HTuple hv__IsDoubleLines = 0;
                            HTuple hv__DoubleLinesType = "light_dark_light";
                            HTuple hv__MiddleLineWidth = 3;

                            _VerifyPara =   ((((((((((((((new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(2))))
                                .Insert(1, new HTupleVector(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireThresAlgoPara.ImageIndex + 1))))
                                .Insert(2, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.WireWidth)))
                                .Insert(3, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.WireContrast)))
                                .Insert(4, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.LightOrDark)))
                                .Insert(5, new HTupleVector(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMetric[0])
                                                            .TupleConcat(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMetric[1])))))
                                .Insert(6, new HTupleVector(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMin[0])
                                                            .TupleConcat(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMin[1])))))
                                .Insert(7, new HTupleVector(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMax[0])
                                                            .TupleConcat(new HTuple(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.SelMax[1])))))
                                .Insert(8, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.LinePhiDiff)))
                                .Insert(9, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.MaxWireGap)))
                                .Insert(10, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.IsDoubleLines == false ? 0 : 1)))
                                .Insert(11, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.DoubleLinesType)))
                                .Insert(12, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.MiddleLineWidth)))
                                .Insert(13, new HTupleVector(group.LineUserRegions[i].WireRegionWithPara.WireLineGauseAlgoParaAll.DistTh)));
                        }
                        LineRegVerifyParas[idx] = _VerifyPara;
                        idx++;
                    }
                    LineRegions = LineRegions.ConcatObj(Algorithm.Region.ConcatRegion((group.LineUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)));
                    LineRegionsNumbers = LineRegionsNumbers.TupleConcat(group.LineUserRegions.Count);
                }

                HTupleVector hvec_i_DefectValueWire = new HTupleVector(3);

                //初始化单类金线检测结果
                hvec_i_DefectValueWire = (new HTupleVector(3).Insert(0, (new HTupleVector(2).Insert(0, (
                    new HTupleVector(1).Insert(0, new HTupleVector(new HTuple())))))));
                HOperatorSet.CountObj(ho__LineStartRegs, out HTuple hv_wireNumber);
                HTuple end_val179 = hv_wireNumber - 1;
                HTuple step_val179 = 1;
                HTuple hv_wireidx = null;
                for (hv_wireidx = 0; hv_wireidx.Continue(end_val179, step_val179); hv_wireidx = hv_wireidx.TupleAdd(step_val179))
                {
                    hvec_i_DefectValueWire[hv_wireidx][0] = (new HTupleVector(1).Insert(0, new HTupleVector(new HTuple(-2))));
                }
              
                //-------------焊点间金线检测
                Algorithm.Model_RegionAlg.HTV_Wire_Inspect_MVP(Algorithm.Region.GetChannnelImageConcact(ImageVerify),
                                                        ho__LineStartRegs,
                                                        ho__LineStopRegs,
                                                        Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnFrameUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                                        Algorithm.Region.ConcatRegion(WireRegionsGroup.Select(r => r.BondOnICUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)),
                                                        LineRegions,
                                                        out HObject Wires,
                                                        out HObjectVector WireFailRegs,
                                                        LineRegionsNumbers,
                                                        LineRegVerifyParas,
                                                        hvec_i_DefectValueWire,
                                                        out hv__DefectWireType,
                                                        out HTupleVector hv__DefectWireImgIdx,
                                                        out HTupleVector hvec__DefectValueWire,
                                                        out HTupleVector hvec__RefValueWire,
                                                        out hv__wireErrCode, out hv__wireErrStr);

                Algorithm.Model_RegionAlg.HTV_Vector_to_ObjectConcat_1d(WireFailRegs, out HObject WireFailRegsConcat, out HTuple VerErrCode, out HTuple VerErrStr);
                if (Wires.CountObj() > 0)
                {
                    htWindow.DisplaySingleRegion(Wires.ConcatObj(WireFailRegsConcat).ConcatObj(DieRegions.SelectObj(imgIndex + 1)), ChannelImageVerify);
                }
                if (hv__wireErrCode > 0)
                {
                    htWindow.DisplaySingleRegion(Wires.ConcatObj(WireFailRegsConcat).ConcatObj(DieRegions.SelectObj(imgIndex + 1)), ChannelImageVerify, "orange");
                }

                WireFailRegsConcat?.Dispose();

                imgIndex++;
                if (imgIndex + 1 > DieRegions.CountObj())
                {
                    imgIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ho__startBondContours.Dispose();
                ho__bondInspectReg.Dispose();
                ho__FailBondRegs.Dispose();
                ho__stopBondContours.Dispose();
                hvec__BondObjs.Dispose();
                hvec__bondInspectObj.Dispose();
                ho_realStartBall.Dispose();
                ho_realStopBall.Dispose();
                //
                MessageBox.Show(ex.ToString());
                imgIndex++;
                if (imgIndex + 1 > DieRegions.CountObj())
                {
                    imgIndex = 0;
                }
                return;
            }
        }

        //前一页
        private void ExecutePreviousCommand(object parameter)
        {
            try
            {
                imgIndex = 0;
                if (WireParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == 0 ? WireParameter.CurrentVerifySet - 1 : --pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                WireParameter.VerifyImagesDirectory,
                                                                                WireParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[WireParameter.ImageIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(WireParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, WireParameter.ImageIndex);
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
                if (WireParameter.CurrentVerifySet > 0 && pImageIndex >= 0)
                {
                    pImageIndex = pImageIndex == WireParameter.CurrentVerifySet - 1 ? 0 : ++pImageIndex;
                    if (isFovTaskFlag == 1)
                    {
                        Algorithm.Model_RegionAlg.HTV_Create_Multichannel_Image(out HObject ho_MutiImage,
                                                                                WireParameter.VerifyImagesDirectory,
                                                                                WireParameter.CurFovName,
                                                                                pImageIndex, out HTuple hv_o_ImageVerifyNum,
                                                                                out HTuple imageFiles,
                                                                                out HTuple hv_o_ImgErrorCode, out HTuple hv_o_ImgErrorStr);

                        if ((int)(hv_o_ImgErrorCode) < 0)
                        {
                            MessageBox.Show(hv_o_ImgErrorStr.ToString());

                            return;
                        }

                        ImageVerify = ho_MutiImage;
                        PImageIndexPath = imageFiles[WireParameter.ImageIndex];
                    }
                    else
                    {
                        Algorithm.File.list_image_files(WireParameter.VerifyImagesDirectory, "default", "recursive", out HTuple imageFiles);
                        HOperatorSet.ReadImage(out HObject image, imageFiles[pImageIndex]);
                        ImageVerify = image;
                        PImageIndexPath = imageFiles[pImageIndex];
                    }
                    ChannelImageVerify = Algorithm.Region.GetChannnelImageUpdate(ImageVerify, WireParameter.ImageIndex);
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
                if (WireParameter.ImageCountChannels > 0 && WireParameter.ImageIndex < 0)
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
            //htWindow.DisplaySingleRegion(WireObject.Image);
            htWindow.Display(ChannelImageVerify, true);

            //1123
            ChannelNames = new ObservableCollection<ChannelName>(WireParameter.ChannelNames);
            OnPropertyChanged("ChannelNames");

            ImageIndex = WireParameter.ImageIndex;
            OnPropertyChanged("ImageIndex");
        }
        public void Dispose()
        {
            (Content as Page_WireInspectVerify).DataContext = null;
            (Content as Page_WireInspectVerify).Close();
            Content = null;
            htWindow = null;
            ChannelImageVerify = null;
            ImageVerify = null;
            WireObject = null;
            WireParameter = null;
            WireRegionsGroup = null;
            VerifyCommand = null;
            SaveCommand = null;
            DisplayLightDarkImageCommand = null;
            ImagesSetVerifyCommand = null;
            PreviousCommand = null;
            NextCommand = null;

            // add by wj

        }
    }
}
