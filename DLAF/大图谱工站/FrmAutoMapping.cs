using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using HalconDotNet;
using HTHalControl;
using System.Threading;
using HT_Lib;
using ToolKits.TemplateEdit;
using System.IO;
using IniDll;
using System.Reflection;
using ToolKits.RegionModify;
using VisionMethonDll;
using JFInterfaceDef;
using JFHub;
using JFToolKits;

namespace DLAF
{
    public struct ImagePosition
    {
        /// <summary>
        /// 所在block
        /// </summary>
        public int b;
        /// <summary>
        /// 行索引
        /// </summary>
        public int r;
        /// <summary>
        /// 列索引
        /// </summary>
        public int c;
        /// <summary>
        /// 位置X
        /// </summary>
        public double x;
        /// <summary>
        /// Y
        /// </summary>
        public double y;
        /// <summary>
        /// Z
        /// </summary>
        public double z;
        /// <summary>
        /// OkOrNg 为false则打标
        /// </summary>
        public bool OkOrNg;
        /// <summary>
        /// 实际单颗芯片row
        /// </summary>
        public int realRow;
        /// <summary>
        /// 实际单颗芯片column
        /// </summary>
        public int realCol;


        public void Dispose()
        {
            b = -1;
            r = -1;
            c = -1;
            x = 0;
            y = 0;
            z = 0;
            realRow = -1;
            realCol = -1;
        }
    }

    public partial class FrmAutoMapping : Form
    {
        #region 参数
        public static FrmAutoMapping Instance = null;
        /// <summary>
        /// 拍照位位置信息
        /// </summary>
        public string mappingImgDir = "MappingImgDir";
        public HTuple snapM;
        public HTuple snapN;

        ToolKits.FunctionModule.Vision tool_vision;
        
        HObject Image, showRegion;
        MainTemplateForm mTmpLctMdlFrm = null;
        MainTemplateForm mTmpCheckPosMdlFrm = null;
        Form_FixMapPos mTmpFixClipPosFrm = null;
        Form toolForm = null;
        Model LoctionModels;
        private List<ImagePosition> GenMapPostions;
        HTuple X_motion = null;
        HTuple Y_motion = null;
        HTuple row ,col;
        HTuple hv_iFlag;
        HObject icImage;
        #endregion

        AutoMappingStation _station;
        HObject allShowRegion;
        HObject selectRegion;
        double fovHeight = 50;
        double fovWidth = 50;

        public FrmAutoMapping()
        {
            InitializeComponent();
            Instance = this;
            row = new HTuple();
            col = new HTuple();
            HOperatorSet.GenEmptyObj(out allShowRegion);
            HOperatorSet.GenEmptyObj(out selectRegion);
        }

        public void SetStation(AutoMappingStation station)
        {
            _station = station;
            //if(Created)
            //    SetupUI();
        }

        public void SetupUI()
        {
            try
            {
                _station.InitStationParams();

                if (_station.jFDLAFProductRecipe == null)
                    this.FindForm().Enabled = false;
                else
                    this.FindForm().Enabled = true;

                GenMapPostions = new List<ImagePosition>();
                numStartX.Value = (decimal)_station.genMapStartX;
                numStartY.Value = (decimal)_station.genMapStartY;
                numEndX.Value = (decimal)_station.genMapEndX;
                numEndY.Value = (decimal)_station.genMapEndY;
                numSameSpace.Value = (decimal)_station.sameSpace;

                if (_station.jFDLAFProductRecipe != null)
                {
                    numWidthFactor.Value = (decimal)_station.jFDLAFProductRecipe.WidthFactor;
                    numHeightFactor.Value = (decimal)_station.jFDLAFProductRecipe.HeightFactor;
                    numScaleFactor.Value = (decimal)_station.jFDLAFProductRecipe.ScaleFactor;
                    numCheckPosX.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosX;
                    numCheckPosY.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosY;
                    numCheckPosScoreThresh.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosScoreThresh;
                }
                numLctScoreThresh.Value = (decimal)_station.lctScoreThresh;
                cbxScanModelMethod.SelectedIndex = _station.checkMdlMethod;

                ckbdoubleCheck.Checked = Convert.ToBoolean(_station.usedoubleCheck);
                cbxCheckPosSelect.Items.Clear();
                if (_station.usedoubleCheck ==1)//双矫正点
                {
                    cbxCheckPosSelect.Items.AddRange(new object[] { "左矫正点", "右矫正点" });
                    btn_UpdateMapTest.Visible = true;
                }
                else
                {
                    cbxCheckPosSelect.Items.AddRange(new object[] { "左矫正点"});
                    btn_UpdateMapTest.Visible = false;
                }
                cbxCheckPosSelect.SelectedIndex = 0;

                //光源配置列表
                cbLightChoose.Items.Clear();
                cbLightChoose.Items.AddRange((string[])JFHubCenter.Instance.VisionMgr.AllSingleVisionCfgNames());
                cbLightChoose.SelectedIndex = 0;

                //若未标定，则无法进行生成大图谱编辑
                if (!InitUV2XYParam())
                {
                    this.FindForm().Enabled = false;
                }
                else
                {
                    //测试阶段先屏蔽
                    fovWidth = Math.Abs(_station.List_UV2XYResult[_station.SelectedIndex][1].D * _station.width.D);
                    fovHeight = Math.Abs(_station.List_UV2XYResult[_station.SelectedIndex][3].D * _station.height.D);
                }


                if (Created)
                {
                    EditColumnHeadText();
                    dgvFovInf.Rows.Clear();
                    //更新Fov信息
                    if (_station.jFDLAFProductRecipe != null)
                    {
                        int fovcount = _station.jFDLAFProductRecipe.visionCfgParams.Keys.Count;
                        for(int i=0;i<fovcount;i++)
                        {
                            JFXmlDictionary<string, JFXmlDictionary<string, string>> fovVisionCfgName = _station.jFDLAFProductRecipe.visionCfgParams[i.ToString()];
                            foreach (string fovname in fovVisionCfgName.Keys)
                            {
                                JFXmlDictionary<string, string> keyValuePairs = fovVisionCfgName[fovname];
                                AddDataGridViewFunction(dgvFovInf, Convert.ToInt32(i.ToString()), true, fovname, keyValuePairs.Keys.Count);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                HTUi.PopError(this.GetType().Name + "UI加载失败！\n" + ex.ToString());
            }
        }



        private bool InitUV2XYParam()
        {
            try
            {
                _station.List_UV2XYResult = new List<HTuple>();
                tool_vision = new ToolKits.FunctionModule.Vision();
                if (_station.Num_Camera <= 0)
                    return false;
                for (int i = 0; i < _station.Num_Camera; i++)
                {
                    HTuple Item_UV2XYResult = null;
                    if (File.Exists(_station.SystemUV2XYDir + "\\Camera_" + i + "\\" + "UV2XY" + ".dat"))
                    {
                        tool_vision.read_hom2d(_station.SystemUV2XYDir + "\\Camera_" + i + "\\" + "UV2XY" + ".dat", out Item_UV2XYResult);
                        _station.List_UV2XYResult.Add(Item_UV2XYResult);
                    }
                    else
                    {
                        Item_UV2XYResult = new HTuple(-1);
                        _station.List_UV2XYResult.Add(Item_UV2XYResult);
                        return false;
                    }
                }
            }
            catch
            {
                //HTUi.PopError("无法读取UV-XY结果文件.");
                return false;
            }
            return true;
        }

        #region 方法
        /// <summary>
        /// 获取所有拍照位的拍摄位置信息   by TWL
        /// </summary>
        /// <returns></returns>
        public string GetImagePositions()
        {
            mappingImgDir = "MappingImgDir";
            //清除之前的点位
            if (GenMapPostions != null)
            {
                GenMapPostions.Clear();
            }
            else
            {
                GenMapPostions = new List<ImagePosition>();
            }


            //1.根据die与die之间的关系计算得出在一个block里行方向需要几个拍照位、列方向需要几个拍照位
            //2.跟距标定后的结果，准确获取第一个block里所有die的位置信息
            //3.计算第一个block内的拍照位
            //4.根据block标定结果计算出所有的block拍照位信息
            //5.将所有拍照位按照以下顺序排列
            /*
                --> --> -->
                            |
                            |
                <-- <-- <--
              */
            if(_station.List_UV2XYResult[_station.SelectedIndex].Type==HTupleType.INTEGER)
            {
                throw new Exception("请先标定该相机！");
            }
            //6.将所有拍照位所有的位置添加到list里 B=16 R=8 C=2 M=1 N=2
            double[] start = new double[2] { _station.genMapStartX, _station.genMapStartY };//new double[2] { -182.184, -6.053 };//左上角视野中心
            double viewWidth = Math.Abs(_station.List_UV2XYResult[_station.SelectedIndex][1].D * _station.width.D);
            double viewHeight = Math.Abs(_station.List_UV2XYResult[_station.SelectedIndex][3].D * _station.height.D);

            double distanceX = viewWidth - _station.sameSpace;//distanceBlockX / _station.ColumnNumber;
            double distanceY = viewHeight - _station.sameSpace;//(end[1] - start[1]) / (NumM - 1);
            int times_X = (int)(Math.Abs(_station.genMapEndX - _station.genMapStartX) / distanceX) + 2;
            int times_Y = (int)(Math.Abs(_station.genMapEndY - _station.genMapStartY) / distanceY) + 2;
            ImagePosition imagePosition = new ImagePosition();
            imagePosition.z = _station.ZFocus;

            string sPath = _station.imageFolder + "\\" + mappingImgDir;
            Directory.CreateDirectory(sPath);
            IniDll.IniFiles config = new IniDll.IniFiles(sPath + "\\point.ini");
            imagePosition.b = 0;
            int step = 0;
            for (int i = 0; i < times_Y; i++)
            {
                imagePosition.r = i;
                imagePosition.y = start[1] - i * distanceY;
                //if (imagePosition.y < _station.genMapEndY) imagePosition.y = _station.genMapEndY;
                for (int j = 0; j < times_X; j++)
                {
                    imagePosition.c = j;
                    imagePosition.x = start[0] + j * distanceX;
                    //if (imagePosition.x > _station.genMapEndX) imagePosition.x = _station.genMapEndX;
                    GenMapPostions.Add(imagePosition);
                    //X_motion.Append(imagePosition.x);
                    //Y_motion.Append(imagePosition.y);
                    config.WriteString("ScanPoint", "step" + step.ToString(), i + "-" + j + "(" + imagePosition.x.ToString() + "," + imagePosition.y + ")");
                    step++;
                }
            }
            return "";
        }
        /// <summary>
        /// 同一个拍照位拍摄多张图 包含2d和3d采集到的图  by M.Bing
        /// </summary>
        /// <param name="b">所在block</param>
        /// <param name="r">所在行</param>
        /// <param name="c">所在列</param>
        /// <returns></returns>
        public string CaputreMultipleImages(ref ImageCache imageCache)
        {
            string errMsg = "";
            if (_station._RunMode == Convert.ToInt16(SystemRunMode.MODE_OFFLINE)) return errMsg;
            //4. 取图 
            //if (_station.operation.SWPosTrig(new string[] { "X" }, out errMsg) != 0)
            //{
            //    HTUi.PopError(errMsg);
            //    return errMsg;
            //}

            Thread.Sleep(10);

            HOperatorSet.GenEmptyObj(out Image);
            //3.拍图
            if (_station.operation.CaputreOneImage(_station.CamereDev[_station.SelectedIndex], "Halcon", out imageCache._2dImage, out errMsg) != 0)
            {
                HTUi.PopError(errMsg);
                btnSnap.Enabled = true;
                return errMsg;
            }

            double nowX = 0, nowY = 0;
            if(!_station.GetAxisPosition(_station.AxisXYZ[0], out nowX,out errMsg))
            {
                HTUi.PopError(errMsg);
                btnSnap.Enabled = true;
                return errMsg;
            }
            if (!_station.GetAxisPosition(_station.AxisXYZ[1], out nowY, out errMsg))
            {
                HTUi.PopError(errMsg);
                btnSnap.Enabled = true;
                return errMsg;
            }
            X_motion.Append(nowX);
            Y_motion.Append(nowY);
            imageCache.b = GenMapPostions[scan_index].b;
            imageCache.r = GenMapPostions[scan_index].r;
            imageCache.c = GenMapPostions[scan_index].c;
            HObject ImageSelect = new HObject();
            _station.jFDLAFProductRecipe.ScaleFactor = Convert.ToDouble(numScaleFactor.Value);
            HOperatorSet.SelectObj(imageCache._2dImage, out ImageSelect, 1);
            HOperatorSet.GetImageSize(ImageSelect, out _station.width, out _station.height);
            //string sPath = _station.ProductDir + "\\" + _station.ActivePdt + "\\" + mappingImgDir;
            string sPath = _station.ProductDir + "\\" + _station.ActivePdt + "\\" + mappingImgDir;

            int imageR = imageCache.r, imageC = imageCache.c;
            Task.Run(() =>
            {
                HOperatorSet.ZoomImageFactor(ImageSelect, out ImageSelect, _station.jFDLAFProductRecipe.ScaleFactor, _station.jFDLAFProductRecipe.ScaleFactor, "constant");
                HOperatorSet.WriteImage(ImageSelect, "tiff", 0, sPath + "\\" + imageR + "-" + imageC + ".tiff");
            });
            return errMsg;
        }

        #endregion
        private void btnSnapMapStart_Click(object sender, EventArgs e)
        {
            string errMsg = "";
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (_station.operation.MultiAxisMove(_station.AxisXYZ, new double[] { _station.genMapStartX, _station.genMapStartY, _station.ZFocus }, true, out errMsg) != 0)
                {
                    HTUi.PopError(String.Format("移动的拍照位{0}_{1}_{2}失败!详细信息:{3}",
                                                            _station.genMapStartX,
                                                            _station.genMapStartY,
                                                            _station.ZFocus,
                                                            errMsg));//报警 停止动作
                    return;
                }
                Thread.Sleep(100);

                if (_station.WaitMotionDone(_station.AxisXYZ[1], 10000) != JFWorkCmdResult.Success)
                {
                    HTUi.PopError("等待轴" + _station.AxisXYZ[1] + "运动完成超时");
                    return;
                }
                if (_station.WaitMotionDone(_station.AxisXYZ[2], 10000) != JFWorkCmdResult.Success)
                {
                    HTUi.PopError("等待轴" + _station.AxisXYZ[2] + "运动完成超时");
                    return;
                }
                if (_station.WaitMotionDone(_station.AxisXYZ[0], 10000) != JFWorkCmdResult.Success)
                {
                    HTUi.PopError("等待轴" + _station.AxisXYZ[0] + "运动完成超时");
                    return;
                }

                if (_station._RunMode == 1) return;

                //if (_station.operation.SWPosTrig(_station.AxisX, out errMsg) != 0)
                //{
                //    HTUi.PopError(errMsg);
                //    return;
                //}
                //4. 取图
                HOperatorSet.GenEmptyObj(out Image);
                if (_station.operation.CaputreOneImage(_station.CamereDev[_station.SelectedIndex], "Halcon", out Image, out errMsg) != 0)
                {
                    HTUi.PopError(errMsg);
                    btnSnap.Enabled = true;
                    return;
                }
                _station.operation.ShowImage(htWindow, Image, null);
            }
            catch(Exception ex)
            {
                HTUi.PopError("采集Map起点失败："+ex.ToString());
                return;
            }
        }

        private void btnTool_Click(object sender, EventArgs e)
        {
            if (HTM.LoadUI() < 0)
            {
                HTUi.PopError("打开轴调试助手界面失败");
            }
        }


        bool ScanWork = false;
        int scan_index = 0;
        private async void btnScanMapImg_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                GetImagePositions();
                HTUi.TipHint("采集Map用点生成完成.");
                HTLog.Info("采集Map用点生成完成.");

                if (ScanWork)
                {
                    ScanWork = false;
                    btnScanMapImg.Text = "采集Mapping用图";
                }
                else
                {
                    ScanWork = true;
                    btnScanMapImg.Text = "停止采集";
                    await Task.Run(() =>
                    {
                        scan_index = 0;
                        if (GenMapPostions == null) GenMapPostions = new List<ImagePosition>();
                        X_motion = new HTuple();
                        Y_motion = new HTuple();
                        string errStr = "";
                        scan_index = 0;
                        string sPath = _station.ProductDir + "\\" + _station.ActivePdt + "\\" + mappingImgDir;
                        if (Directory.Exists(sPath)) Directory.Delete(sPath, true);
                        Directory.CreateDirectory(sPath);

                        string errMsg = "";
                        while (ScanWork)
                        {
                            double[] distance = new double[] { GenMapPostions[scan_index].x, GenMapPostions[scan_index].y, GenMapPostions[scan_index].z };


                            if (_station.operation.MultiAxisMove(_station.AxisXYZ, distance, true, out errMsg) != 0)
                            {
                                HTUi.PopError(String.Format("移动的拍照位{0}_{1}_{2}失败!详细信息:{3}",
                                                           GenMapPostions[scan_index].b,
                                                           GenMapPostions[scan_index].r,
                                                           GenMapPostions[scan_index].c,
                                                           errMsg));//报警 停止动作
                                ScanWork = false;
                                btnScanMapImg.BeginInvoke(new System.Windows.Forms.MethodInvoker(() => { btnScanMapImg.Text = "采集Mapping用图"; }));
                                return;
                            }

                            ImageCache imageCache = new ImageCache();
                            errStr = CaputreMultipleImages(ref imageCache);
                            if (errStr == "" || errStr == "Success")//拍照完成
                            {
                                _station.operation.ShowImage(htWindow, imageCache._2dImage.SelectObj(1), null);
                            }
                            scan_index++;
                            if (scan_index >= GenMapPostions.Count)
                            {
                                break;
                            }
                        }
                        HOperatorSet.WriteTuple(X_motion, _station.ProductDir + "\\" + _station.ActivePdt + "\\" + mappingImgDir + "\\Xpoint.dat");
                        HOperatorSet.WriteTuple(Y_motion, _station.ProductDir + "\\" + _station.ActivePdt + "\\" + mappingImgDir + "\\Ypoint.dat");

                        if (_station.operation.MultiAxisMove(_station.AxisXYZ, new double[] { _station.ref_x, _station.ref_y, _station.Z_safe }, true, out errMsg) != 0)
                        {
                            HTUi.PopError("【检测台模块】" + errMsg);
                        }
                        HTUi.TipHint("采集Map采集结束.");
                        HTLog.Info("采集Map采集结束.");
                        scan_index = 0;
                        ScanWork = false;
                        btnScanMapImg.BeginInvoke(new System.Windows.Forms.MethodInvoker(() => { btnScanMapImg.Text = "采集Mapping用图"; }));
                    });
                }
            }
            catch (Exception ex)
            {
                HTUi.PopError("采集Map用点生成失败" + ex.ToString());
                return;
            }
}

        private void btnAutoGenMap_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                VisionMethon.gen_map_images(out _station.frameMapImg, _station.ProductDir + "\\" + _station.ActivePdt + "\\" + mappingImgDir, _station.List_UV2XYResult[_station.SelectedIndex], _station.jFDLAFProductRecipe.ScaleFactor, out _station.hv_xSnapPosLT, out _station.hv_ySnapPosLT, out hv_iFlag);
                if (hv_iFlag != "")
                {
                    HTUi.PopError("生成图谱失败.");
                    return;
                }
                HOperatorSet.WriteImage(_station.frameMapImg, "tiff", 0, _station.ProductDir + "\\" + _station.ActivePdt + "\\frameMapImg.tiff");
                HOperatorSet.WriteTuple(_station.hv_xSnapPosLT, _station.ProductDir + "\\" + _station.ActivePdt + "\\hv_xSnapPosLT.tup");
                HOperatorSet.WriteTuple(_station.hv_ySnapPosLT, _station.ProductDir + "\\" + _station.ActivePdt + "\\hv_ySnapPosLT.tup");
                _station.operation.ShowImage(htWindow, _station.frameMapImg, null);

                _station.SaveStationParams();
                _station.SaveCfg();
                _station.jFDLAFProductRecipe.SaveParamsToCfg();
                JFHubCenter.Instance.RecipeManager.Save();
                HTUi.TipHint("生成图谱完成.");
                HTLog.Info("生成图谱完成.");
            }
            catch (Exception ex)
            {
                HTUi.PopError("生成图谱失败.\n" + ex.ToString());
            }
        }

        private void btnCrtLctMdls_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (toolForm == null || toolForm.IsDisposed)
                {
                    toolForm = new Form();
                    if (this.mTmpLctMdlFrm != null)
                        if (!this.mTmpLctMdlFrm.IsDisposed) this.mTmpLctMdlFrm.Dispose();
                    this.mTmpLctMdlFrm = new MainTemplateForm(ToolKits.TemplateEdit.MainTemplateForm.TemplateScence.Match,
                                     this.htWindow, new MainTemplateForm.TemplateParam());
                    toolForm.Controls.Clear();
                    this.toolForm.Controls.Add(this.mTmpLctMdlFrm);
                    this.mTmpLctMdlFrm.Dock = DockStyle.Fill;
                    toolForm.Size = new Size(300, 600);
                    toolForm.TopMost = true;
                    toolForm.Text = "模板制作";
                    toolForm.Show();
                    int SH = Screen.PrimaryScreen.Bounds.Height;
                    int SW = Screen.PrimaryScreen.Bounds.Width;
                    toolForm.Location = new Point(SW - toolForm.Size.Width, SH / 8);
                    Task.Run(() =>
                    {
                        while (!mTmpLctMdlFrm.WorkOver)
                        {
                            Thread.Sleep(200);
                        }
                        HTUi.TipHint("创建芯片定位模板完成!");
                        HTLog.Info("创建芯片定位模板完成!");
                        toolForm.Close();
                    });
                }
                else
                {
                    toolForm.Activate();
                }
            }
            catch (Exception ex)
            {
                HTUi.PopError("创建定位模板失败" + ex.ToString());
                return;
            }
}

        private async void btnSaveLctMdls_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (!this.mTmpLctMdlFrm.tmpResult.createTmpOK)
                {
                    HTUi.PopError("创建定位模板失败！");
                    return;
                }
                this.LoctionModels = new Model();
                this.LoctionModels.Dispose();
                this.LoctionModels.showContour = this.mTmpLctMdlFrm.tmpResult.showContour.CopyObj(1, -1);
                this.LoctionModels.defRows = this.mTmpLctMdlFrm.tmpResult.defRows;
                this.LoctionModels.defCols = this.mTmpLctMdlFrm.tmpResult.defCols;
                this.LoctionModels.modelType = this.mTmpLctMdlFrm.tmpResult.modelType;
                this.LoctionModels.modelID = AutoMappingOperation.CopyModel(this.mTmpLctMdlFrm.tmpResult.modelID, this.mTmpLctMdlFrm.tmpResult.modelType);
                this.LoctionModels.scoreThresh = this.mTmpLctMdlFrm.mte_TmpPrmValues.Score;
                this.LoctionModels.angleStart = this.mTmpLctMdlFrm.mte_TmpPrmValues.AngleStart;
                this.LoctionModels.angleExtent = this.mTmpLctMdlFrm.mte_TmpPrmValues.AngleExtent;
                //保存至硬盘
                string modelPath = _station.ProductDir + "\\" + _station.ActivePdt + "\\LoctionModels"; ;
                _station.CalibrUV2XYModelPath = modelPath;

                Form_Wait.ShowForm();
                await Task.Run(new Action(() =>
                {
                    if (!this.LoctionModels.WriteModel(modelPath))
                    {
                        HTUi.PopError("保存标定模板失败！");
                        HTLog.Error("保存标定模板失败！");
                        return;
                    }
                    HTUi.TipHint("保存定位模板成功！");
                    HTLog.Info("保存定位模板成功！");
                }));

                HObject _clipRegion = null, clipImage;
                HTuple _row1, _col1, _row2, _col2;
                HTuple hom2D;
                HOperatorSet.GenRegionContourXld(this.LoctionModels.showContour, out _clipRegion, "filled");
                HOperatorSet.SmallestRectangle1(_clipRegion, out _row1, out _col1, out _row2, out _col2);
                HOperatorSet.VectorAngleToRigid(_row1, _col1, 0, 0, 0, 0, out hom2D);
                HOperatorSet.CropRectangle1(_station.frameMapImg, out clipImage, _row1, _col1, _row2, _col2);
                Directory.CreateDirectory(_station.ProductDir + "\\" + _station.ActivePdt);
                HOperatorSet.WriteImage(clipImage, "tiff", 0, _station.ProductDir + "\\" + _station.ActivePdt + "\\" + "ClipImage.tiff");

                Form_Wait.CloseForm();
            }
            catch (Exception ex)
            {
                HTUi.PopError("保存定位模板失败！\n" + ex.ToString());
            }
        }

        private void btnGenMapPos_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (this.LoctionModels != null)
                {
                    this.LoctionModels.Dispose();
                }

                this.LoctionModels = new Model();
                if (!this.LoctionModels.ReadModel(_station.ProductDir + "\\" + _station.ActivePdt + "\\LoctionModels"))
                {
                    HTUi.PopError("未创建定位模板！");
                    return;
                }
                
                if (_station.frameMapImg == null)
                {
                    HTUi.PopError("未生成或加载料片图！");
                    return;
                }

                HTuple dieHeight = new HTuple();
                HTuple dieWidth = new HTuple();

                VisionMethon.get_mapping_coords(_station.frameMapImg, this.LoctionModels.showContour, this.LoctionModels.modelType, this.LoctionModels.modelID,
                    this.LoctionModels.defRows, this.LoctionModels.defCols, _station.lctScoreThresh, _station.hv_xSnapPosLT, _station.hv_ySnapPosLT, _station.List_UV2XYResult[_station.SelectedIndex],
                    _station.jFDLAFProductRecipe.ScaleFactor,_station.RowNumber, _station.ColumnNumber * _station.BlockNumber,
                    out _station.clipMapX, out _station.clipMapY, out _station.clipMapRow, out _station.clipMapCol,
                    out _station.clipMapU, out _station.clipMapV, out dieWidth, out dieHeight, out hv_iFlag);
                if (hv_iFlag.S != "")
                {
                    HTUi.PopError("生成芯片点位失败." + hv_iFlag.S);
                    return;
                }
                HOperatorSet.WriteTuple(_station.clipMapX, _station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapX.dat");
                HOperatorSet.WriteTuple(_station.clipMapY, _station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapY.dat");
                HOperatorSet.WriteTuple(_station.clipMapRow, _station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapRow.dat");
                HOperatorSet.WriteTuple(_station.clipMapCol, _station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapCol.dat");

                //清除之前的点位
                if (_station.ClipMapPostions != null)
                {
                    _station.ClipMapPostions.Clear();
                }
                else
                {
                    _station.ClipMapPostions = new List<ImagePosition>();
                }
                ImagePosition imagePosition = new ImagePosition();
                imagePosition.z = _station.ZFocus;
                imagePosition.b = 0;
                _station.clipPosNum = _station.clipMapX.Length;
                for (int i = 0; i < _station.clipPosNum; i++)
                {
                    imagePosition.x = _station.clipMapX.TupleSelect(i);
                    imagePosition.y = _station.clipMapY.TupleSelect(i);
                    imagePosition.r = _station.clipMapRow.TupleSelect(i);
                    imagePosition.c = _station.clipMapCol.TupleSelect(i);
                    _station.ClipMapPostions.Add(imagePosition);
                }
                if (showRegion == null) showRegion = new HObject();
                showRegion.Dispose();
                HOperatorSet.GenCrossContourXld(out showRegion, _station.clipMapU, _station.clipMapV, 26, 0);
                _station.operation.ShowImage(htWindow, _station.frameMapImg, showRegion);
                //HOperatorSet.SetColor(htWindow.HTWindow.HalconWindow, "green");
                //HOperatorSet.DispObj(_station.frameMapImg,htWindow.HTWindow.HalconWindow);
                //HOperatorSet.DispObj(Region, htWindow.HTWindow.HalconWindow);
                //Region.Dispose();

                _station.jFDLAFProductRecipe.DieWidth = dieWidth.D;
                _station.jFDLAFProductRecipe.DieHeight = dieHeight.D;
                _station.jFDLAFProductRecipe.SaveParamsToCfg();
                JFHubCenter.Instance.RecipeManager.Save();


                HTUi.TipHint("生成芯片点位完成.");
                HTLog.Info("生成芯片点位完成.");
            }
            catch (Exception ex)
            {
                HTUi.PopError("生成芯片点位失败。\n" + ex.ToString());
                HTLog.Error("生成芯片点位失败。\n" + ex.ToString());
            }
        }
        private void btnFixMapPos_Click(object sender, EventArgs e)
        {
            if (_station.jFDLAFProductRecipe == null)
            {
                HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                return;
            }

            if (this.mTmpFixClipPosFrm == null || this.mTmpFixClipPosFrm.IsDisposed)
            {
                if (this.LoctionModels == null)
                {
                    this.LoctionModels = new Model();
                    if (!this.LoctionModels.ReadModel(_station.ProductDir + "\\" + _station.ActivePdt + "\\LoctionModels"))
                    {
                        HTUi.PopError("未创建定位模板！");
                        return;
                    }
                }
                if(_station.frameMapImg==null)
                {
                    HTUi.PopError("未生成或加载料片图！");
                    return;
                }
                try
                {
                    HObject _clipRegion = null, clipImage;
                    HTuple _row1, _col1, _row2, _col2;
                    HTuple hom2D;
                    HOperatorSet.GenRegionContourXld(this.LoctionModels.showContour, out _clipRegion, "filled");
                    HOperatorSet.SmallestRectangle1(_clipRegion, out _row1, out _col1, out _row2, out _col2);
                    HOperatorSet.VectorAngleToRigid(_row1, _col1, 0, 0, 0, 0, out hom2D);
                    HOperatorSet.CropRectangle1(_station.frameMapImg, out clipImage, _row1, _col1, _row2, _col2);
                    Directory.CreateDirectory(_station.ProductDir + "\\" + _station.ActivePdt);
                    HOperatorSet.WriteImage(clipImage, "tiff", 0, _station.ProductDir + "\\" + _station.ActivePdt + "\\" + "ClipImage.tiff");

                    this.mTmpFixClipPosFrm = new Form_FixMapPos(htWindow, this.LoctionModels, RegionModifyForm.RegionMode.contour);
                    this.mTmpFixClipPosFrm.SetStation(_station);
                    this.mTmpFixClipPosFrm.TopMost = false;
                    this.mTmpFixClipPosFrm.Text = "手动生成芯片点位";
                    this.mTmpFixClipPosFrm.Show();
                    int SH = Screen.PrimaryScreen.Bounds.Height;
                    int SW = Screen.PrimaryScreen.Bounds.Width;
                    this.mTmpFixClipPosFrm.Location = new Point((SW - this.mTmpFixClipPosFrm.Size.Width) / 2, (SH - this.mTmpFixClipPosFrm.Size.Height) / 2);
                }
                catch(Exception ex)
                {
                    HTUi.PopError("截取芯片图出错！\n"+ex.ToString());
                }
            }
            else
            {
                this.mTmpFixClipPosFrm.Activate();
            }
        }
        
        private void btnGenScanPos_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (htWindow.Image == null)
                {
                    HTUi.PopError("生成失败.\n未在窗口显示图片.");
                    return;
                }


                if (fovWidth>=_station.jFDLAFProductRecipe.DieWidth && fovHeight>=_station.jFDLAFProductRecipe.DieHeight)//大视野vs小芯片
                {
                    //HOperatorSet.GetImageSize(htWindow.Image, out width, out height);
                    if (showRegion == null) showRegion = new HObject();
                    showRegion.Dispose();
                    VisionMethon.get_scan_points(out showRegion, _station.width, _station.height, _station.jFDLAFProductRecipe.WidthFactor, _station.jFDLAFProductRecipe.HeightFactor, _station.jFDLAFProductRecipe.ScaleFactor,
                        _station.jFDLAFProductRecipe.DieWidth, _station.jFDLAFProductRecipe.DieHeight, _station.clipMapX, _station.clipMapY, _station.List_UV2XYResult[_station.SelectedIndex],
                        _station.hv_xSnapPosLT, _station.hv_ySnapPosLT, _station.RowNumber, _station.ColumnNumber * _station.BlockNumber,
                        out snapM, out snapN, out _station.snapMapX, out _station.snapMapY,
                        out _station.snapMapRow, out _station.snapMapCol, out hv_iFlag);
                    //_station.snapMapX = _station.snapMapX - 0.1;
                    //_station.snapMapY = _station.snapMapY + 0.1;
                    if (hv_iFlag != "")
                    {
                        HTUi.PopError("生成扫描点位失败.");
                        return;
                    }

                    _station.jFDLAFProductRecipe.DieRowInFov = snapM.TupleMax().I;
                    _station.jFDLAFProductRecipe.DieColInFov = snapN.TupleMax().I;
                    _station.jFDLAFProductRecipe.SaveParamsToCfg();
                    JFHubCenter.Instance.RecipeManager.Save();

                }
                else//大芯片vs小视野
                {
                    if(!File.Exists(_station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapX.dat") || !File.Exists(_station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapY.dat") ||
                        !File.Exists(_station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapRow.dat") || !File.Exists(_station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapCol.dat"))
                    {
                        HTUi.PopError(_station.ProductDir + "\\" + _station.ActivePdt + "clipMapX.dat/clipMapY.dat/clipMapRow.dat/clipMapCol.dat,"+"其中的文件不存在");
                        return;
                    }
                    HOperatorSet.ReadTuple(_station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapX.dat", out _station.clipMapX);
                    HOperatorSet.ReadTuple(_station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapY.dat", out _station.clipMapY);
                    HOperatorSet.ReadTuple(_station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapRow.dat", out _station.clipMapRow);
                    HOperatorSet.ReadTuple(_station.ProductDir + "\\" + _station.ActivePdt + "\\clipMapCol.dat", out _station.clipMapCol);
                    _station.operation.get_scan_points(_station.width, _station.height, _station.List_UV2XYResult[_station.SelectedIndex],_station.jFDLAFProductRecipe.ScaleFactor,
                        _station.RowNumber,_station.BlockNumber*_station.ColumnNumber,_station.clipMapX, _station.clipMapY, _station.clipMapRow, _station.clipMapCol, out _station.snapMapX, out _station.snapMapY,
                        out _station.snapMapRow, out _station.snapMapCol, out hv_iFlag);
                    if (hv_iFlag != "")
                    {
                        HTUi.PopError("生成大芯片中心扫描点位失败.");
                        return;
                    }
                    _station.jFDLAFProductRecipe.DieRowInFov = 0;
                    _station.jFDLAFProductRecipe.DieColInFov = 0;
                    _station.jFDLAFProductRecipe.SaveParamsToCfg();
                    JFHubCenter.Instance.RecipeManager.Save();
                }
                _station.scanRowNum = _station.snapMapRow.TupleMax() + 1;
                _station.scanColNum = _station.snapMapCol.TupleMax() + 1;
                _station.operation.ShowImage(htWindow, _station.frameMapImg, showRegion);
                HOperatorSet.WriteTuple(_station.snapMapX, _station.ProductDir + "\\" + _station.ActivePdt + "\\" + "snapMapX.dat");
                HOperatorSet.WriteTuple(_station.snapMapY, _station.ProductDir + "\\" + _station.ActivePdt + "\\" + "snapMapY.dat");
                HOperatorSet.WriteTuple(_station.snapMapRow, _station.ProductDir + "\\" + _station.ActivePdt + "\\" + "snapMapRow.dat");
                HOperatorSet.WriteTuple(_station.snapMapCol, _station.ProductDir + "\\" + _station.ActivePdt + "\\" + "snapMapCol.dat");

                //清除之前的点位
                if (_station.ScanMapPostions != null)
                {
                    _station.ScanMapPostions.Clear();
                }
                else
                {
                    _station.ScanMapPostions = new List<ImagePosition>();
                }
                ImagePosition imagePosition = new ImagePosition();
                imagePosition.z = _station.ZFocus;
                _station.scanPosNum = _station.snapMapX.Length;
                for (int i = 0; i < _station.scanPosNum; i++)
                {
                    imagePosition.x = _station.snapMapX.TupleSelect(i);
                    imagePosition.y = _station.snapMapY.TupleSelect(i);
                    imagePosition.r = _station.snapMapRow.TupleSelect(i);
                    imagePosition.c = _station.snapMapCol.TupleSelect(i);
                    _station.ScanMapPostions.Add(imagePosition);
                }

                HTUi.TipHint("生成扫描点位完成.");
                HTLog.Info("生成扫描点位完成.");

            }
            catch (Exception ex)
            {
                HTUi.PopError("生成扫描点位失败。\n" + ex.ToString());
                HTLog.Error("生成扫描点位失败。\n" + ex.ToString());
            }
        }

        
        private void btnLoadICImage_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe != null)
                {
                    if (!File.Exists(_station.ProductDir + "\\" + _station.ActivePdt + "\\" + "ClipImage.tiff"))
                    {
                        HTUi.PopError("未生成过该图片");
                    }
                    HOperatorSet.ReadImage(out icImage, _station.ProductDir + "\\" + _station.ActivePdt + "\\" + "ClipImage.tiff");
                    //_station.operation.ShowImage(htWindow, icImage, null);

                    HOperatorSet.GenEmptyObj(out allShowRegion);       
                    int fovCount = _station.ICFovCol.Count;
                    row = new HTuple();
                    col = new HTuple();
                    for (int i=0;i<fovCount;i++)
                    {
                        row = row.TupleConcat((HTuple)((_station.ICFovRow[i])));
                        col = col.TupleConcat((HTuple)((_station.ICFovCol[i])));
                    }
                    ShowAllRegion();
                    _station.operation.ShowImage(htWindow, icImage, allShowRegion);

                    btnGenICScanPos.Enabled = true;
                    HTUi.TipHint("加载产品图片成功！");
                    HTLog.Info("加载产品图片成功！");
                }
                else
                {
                    MessageBox.Show("请先加载产品配方"+ (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }
            }
            catch (Exception ex)
            {
                HTUi.PopError("加载产品图片失败" + ex.ToString());
                return;
            }

        }

        private void btnGenICScanPos_Click(object sender, EventArgs e)
        {
            HTuple icwidth = new HTuple();
            HTuple icheight = new HTuple();
            HTuple hv_iFlag = new HTuple();
            HTuple hv_icYScan = new HTuple();
            HTuple hv_icXScan = new HTuple();
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (icImage==null)
                {
                    HTUi.PopError("未成功加载IC图像");
                    return;
                }
                if(row.Length<=0 || col.Length<=0)
                {
                    HTUi.PopError("至少添加一个Fov");
                    return;
                }

                if (fovWidth >= _station.jFDLAFProductRecipe.DieWidth && fovHeight >= _station.jFDLAFProductRecipe.DieHeight)//大视野vs小芯片
                {
                    if(row.Length>1)
                    {
                        HTUi.PopError("大视野小芯片只能有一个视野，请删除多余视野");
                        return;
                    }
                }

                if (_station.ICFovRow.Count!=_station.jFDLAFProductRecipe.visionCfgParams.Keys.Count)
                {
                    HTUi.PopError(string.Format("请删除所有视野并重新框选视野"));
                    return;
                }

                if (!UpdateAllFovAndTaskName())
                {
                    return;
                }


                HOperatorSet.GetImageSize(icImage, out icwidth, out icheight);
                _station.operation.Get_ICRegion_points(icwidth, icheight, row, col, _station.List_UV2XYResult[_station.SelectedIndex], _station.jFDLAFProductRecipe.ScaleFactor,
                    out hv_icXScan, out hv_icYScan, out hv_iFlag);
                if (hv_iFlag != "")
                {
                    HTUi.PopError("生成单颗芯片扫描点位失败.");
                    return;
                }

                //判断FOV补偿值是否合理
  
                if(Math.Abs(hv_icXScan.TupleMax().D)<= _station.jFDLAFProductRecipe.DieWidth/2 && Math.Abs(hv_icXScan.TupleMin().D) <= _station.jFDLAFProductRecipe.DieWidth / 2
                    && Math.Abs(hv_icYScan.TupleMax().D) <= _station.jFDLAFProductRecipe.DieHeight / 2 && Math.Abs(hv_icYScan.TupleMin().D) <= _station.jFDLAFProductRecipe.DieHeight / 2)
                {

                }
                else
                {
                    HTUi.PopError("请先生成扫描点位。\r\n或者单颗芯片扫描点的补偿值超过芯片尺寸，请确认所有视野是否超出图片区域");
                    return;
                }

                int fovCount = hv_icXScan.Length;
                _station.jFDLAFProductRecipe.ICFovOffsetX.Clear();
                _station.jFDLAFProductRecipe.ICFovOffsetY.Clear();
                for (int i=0;i<fovCount;i++)
                {
                    _station.jFDLAFProductRecipe.ICFovOffsetX.Add(hv_icXScan.TupleSelect(i).D);
                    _station.jFDLAFProductRecipe.ICFovOffsetY.Add(hv_icYScan.TupleSelect(i).D);
                }

                _station.SaveStationParams();
                _station.SaveCfg();
                _station.jFDLAFProductRecipe.SaveParamsToCfg();
                JFHubCenter.Instance.RecipeManager.Save();

                HTUi.TipHint("生成芯片视野扫描点位成功！");
                HTLog.Info("生成芯片视野扫描点位成功！");
            }
            catch (Exception ex)
            {
                HTUi.PopError("生成单颗扫描点位失败。\n" + ex.ToString());
                HTLog.Error("生成单颗扫描点位失败。\n" + ex.ToString());
            }
        }

        /// <summary>
        /// 判断FovName/TaskName是否为空,以及Taskcount是否为大于0的正整数
        /// </summary>
        /// <returns></returns>
        private bool IsFovNameAndTaskNameValid()
        {
            int fovCount = row.Length;
            for (int i = 0; i < fovCount; i++)
            {
                if (dgvFovInf.Rows[i].Cells[2].Value==null)
                {
                    HTUi.PopError(string.Format("索引名为{0}的Fov名不可为空", i));
                    return false;
                }
                if(dgvFovInf.Rows[i].Cells[3].Value==null)
                {
                    HTUi.PopError(string.Format("光源配置数量必须为大于0的正整数"));
                    return false;
                }
                string mTaskcount = dgvFovInf.Rows[i].Cells[3].Value.ToString();
                int iTaskCount = 0;
                if (!int.TryParse(mTaskcount, out iTaskCount))
                {
                    HTUi.PopError(string.Format("光源配置数量必须为大于0的正整数"));
                    return false;
                }
                if (iTaskCount <= 0)
                {
                    HTUi.PopError("光源配置数量必须为大于0的正整数");
                    return false;
                }
                if (!_station.jFDLAFProductRecipe.visionCfgParams.ContainsKey(string.Format("{0}", i)))
                {
                    HTUi.PopError(string.Format("请删除所有视野并重新框选视野"));
                    return false;
                }
                if (_station.jFDLAFProductRecipe.visionCfgParams[string.Format("{0}", i)].Keys.Count == 0)
                {
                    HTUi.PopError(string.Format("请删除所有视野并重新框选视野"));
                    return false;
                }
                JFXmlDictionary<string, JFXmlDictionary<string, string>> fovVisionCfg = _station.jFDLAFProductRecipe.visionCfgParams[string.Format("{0}", i)];
                foreach (string fovname in fovVisionCfg.Keys)
                {
                    JFXmlDictionary<string, string> keyValuePairs = fovVisionCfg[fovname];
                    foreach (string taskname in keyValuePairs.Keys)
                    {
                        if (taskname == "")
                        {
                            HTUi.PopError(string.Format("索引名为{0}的Task名不可为空", i));
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void btnSnapCheckPos_Click(object sender, EventArgs e)
        {
            string errMsg = "";
            try
            {
                if (cbxCheckPosSelect.SelectedIndex == 0)//左矫正点
                {
                    if (_station.operation.MultiAxisMove(_station.AxisXYZ, new double[] { _station.jFDLAFProductRecipe.CheckPosX, _station.jFDLAFProductRecipe.CheckPosY, _station.ZFocus }, true, out errMsg) != 0)
                    {
                        HTUi.PopError(String.Format("移动的拍照位{0}_{1}_{2}失败!详细信息:{3}",
                                                    _station.jFDLAFProductRecipe.CheckPosX,
                                                    _station.jFDLAFProductRecipe.CheckPosY,
                                                    _station.ZFocus,
                                                    errMsg));//报警 停止动作
                        return;
                    }
                    if (_station._RunMode == 1) return;

                    //if (_station.operation.SWPosTrig(new string[] { "X" }, out errMsg) != 0)
                    //{
                    //    HTUi.PopError(errMsg);
                    //    return;
                    //}
                    //4. 取图

                    HOperatorSet.GenEmptyObj(out Image);

                    if (_station.operation.CaputreOneImage(_station.CamereDev[_station.SelectedIndex], "Halcon", out Image, out errMsg) != 0)
                    {
                        HTUi.PopError("采集图像失败：" + errMsg);
                        return;
                    }

                    _station.operation.ShowImage(htWindow, Image, null);
                    if (_station.CheckPosModels == null) return;
                    HTuple hv_updateSnapMapX; HTuple hv_updateSnapMapY; HTuple hv_iFlag;
                    if (_station.CheckPosModels.matchRegion == null || !_station.CheckPosModels.matchRegion.IsInitialized())
                        HOperatorSet.GetDomain(Image, out _station.CheckPosModels.matchRegion);




                    if (_station.checkMdlMethod == (int)CheckModelMethod.MODEL_ORD)
                    {
                        if (_station.usedoubleCheck == 0)
                        {
                            VisionMethon.update_map_points(Image, _station.CheckPosModels.matchRegion, _station.CheckPosModels.modelType, _station.CheckPosModels.modelID,
                                _station.jFDLAFProductRecipe.CheckPosScoreThresh, _station.clipMapX, _station.clipMapY, _station.snapMapX, _station.snapMapY, _station.List_UV2XYResult[_station.SelectedIndex],
                                out _station.hv_updateMapX, out _station.hv_updateMapY, out hv_updateSnapMapX, out hv_updateSnapMapY, out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                        }
                        else
                        {
                            VisionMethon.coor_uvToxy_point(Image, _station.CheckPosModels.defRows, _station.CheckPosModels.defCols, _station.jFDLAFProductRecipe.CheckPosX, _station.jFDLAFProductRecipe.CheckPosY,
                                _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_defCol2X, out _station.hv_defRow2Y, out hv_iFlag);
                            if (hv_iFlag.S != "")
                            {
                                HTLog.Error("模板中心像素转换成实际坐标失败！" + hv_iFlag.S);
                                return;
                            }
                            VisionMethon.Map_Points_Match(Image, _station.CheckPosModels.matchRegion, _station.CheckPosModels.modelType, _station.CheckPosModels.modelID,
                                _station.jFDLAFProductRecipe.CheckPosScoreThresh, _station.CheckPosModels.defRows, _station.CheckPosModels.defCols, _station.hv_defCol2X, _station.hv_defRow2Y,
                                _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_modelHmap, out _station.hv_updateCoorX, out _station.hv_updateCoorY,
                                out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                        }
                    }
                    else
                    {
                        VisionMethon.update_map_points_Test(Image, _station.CheckPosModels.matchRegion, _station.CheckPosModels.modelType, _station.CheckPosModels.modelID,
                            _station.CheckPosModels.defRows, _station.CheckPosModels.defCols, _station.CheckPosModels.hLineRows, _station.CheckPosModels.hLineCols, _station.CheckPosModels.vLineRows,
                            _station.CheckPosModels.vLineCols, _station.CheckPosModels.refPointsRow, _station.CheckPosModels.refPointsCol, _station.clipMapX, _station.clipMapY, _station.snapMapX,
                            _station.snapMapY, _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_updateMapX, out _station.hv_updateMapY, out hv_updateSnapMapX, out hv_updateSnapMapY, out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                    }
                    if (hv_iFlag.S != "")
                    {
                        HTLog.Error("矫正点识别失败！" + hv_iFlag.S);
                        return;
                    }

                }
                else
                {
                    if (_station.operation.MultiAxisMove(_station.AxisXYZ, new double[] { _station.jFDLAFProductRecipe.CheckPosRX, _station.jFDLAFProductRecipe.CheckPosRY, _station.ZFocus }, true, out errMsg) != 0)
                    {
                        HTUi.PopError(String.Format("移动的拍照位{0}_{1}_{2}失败!详细信息:{3}",
                                                    _station.jFDLAFProductRecipe.CheckPosRX,
                                                    _station.jFDLAFProductRecipe.CheckPosRY,
                                                    _station.ZFocus,
                                                    errMsg));//报警 停止动作
                        return;
                    }

                    if (_station._RunMode == 1) return;

                    //if (_station.operation.SWPosTrig(_station.AxisX, out errMsg) != 0)
                    //{
                    //    HTUi.PopError(errMsg);
                    //    return;
                    //}
                    //4. 取图

                    HOperatorSet.GenEmptyObj(out Image);

                    if (_station.operation.CaputreOneImage(_station.CamereDev[_station.SelectedIndex], "Halcon", out Image, out errMsg) != 0)
                    {
                        HTUi.PopError("采集图像失败：" + errMsg);
                        return;
                    }

                    _station.operation.ShowImage(htWindow, Image, null);
                    if (_station.CheckPosRModels == null) return;
                    HTuple hv_updateSnapMapX; HTuple hv_updateSnapMapY; HTuple hv_iFlag;
                    if (_station.CheckPosRModels.matchRegion == null || !_station.CheckPosRModels.matchRegion.IsInitialized())
                        HOperatorSet.GetDomain(Image, out _station.CheckPosRModels.matchRegion);

                    if (_station.checkMdlMethod == (int)CheckModelMethod.MODEL_ORD)
                    {
                        //VisionMethon.update_map_points(Image, _station.CheckPosRModels.matchRegion, _station.CheckPosRModels.modelType, _station.CheckPosRModels.modelID,
                        //    _station.jFDLAFProductRecipe.CheckPosRScoreThresh, _station.clipMapX, _station.clipMapY, _station.snapMapX, _station.snapMapY, _station.List_UV2XYResult[_station.SelectedIndex],
                        //    out _station.hv_updateMapX, out _station.hv_updateMapY, out hv_updateSnapMapX, out hv_updateSnapMapY, out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                        VisionMethon.coor_uvToxy_point(Image, _station.CheckPosRModels.defRows, _station.CheckPosRModels.defCols, _station.jFDLAFProductRecipe.CheckPosRX, _station.jFDLAFProductRecipe.CheckPosRY,
                            _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_defCol2XR, out _station.hv_defRow2YR, out hv_iFlag);
                        if (hv_iFlag.S != "")
                        {
                            HTLog.Error("模板中心像素转换成实际坐标失败！" + hv_iFlag.S);
                            return;
                        }
                        VisionMethon.Map_Points_Match(Image, _station.CheckPosRModels.matchRegion, _station.CheckPosRModels.modelType, _station.CheckPosRModels.modelID,
                            _station.jFDLAFProductRecipe.CheckPosRScoreThresh, _station.CheckPosRModels.defRows, _station.CheckPosRModels.defCols, _station.hv_defCol2XR, _station.hv_defRow2YR,
                            _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_modelHmapR, out _station.hv_updateCoorXR, out _station.hv_updateCoorYR,
                            out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                    }
                    else
                    {
                        VisionMethon.update_map_points_Test(Image, _station.CheckPosRModels.matchRegion, _station.CheckPosRModels.modelType, _station.CheckPosRModels.modelID,
                            _station.CheckPosRModels.defRows, _station.CheckPosRModels.defCols, _station.CheckPosRModels.hLineRows, _station.CheckPosRModels.hLineCols, _station.CheckPosRModels.vLineRows,
                            _station.CheckPosRModels.vLineCols, _station.CheckPosRModels.refPointsRow, _station.CheckPosRModels.refPointsCol, _station.clipMapX, _station.clipMapY, _station.snapMapX,
                            _station.snapMapY, _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_updateMapX, out _station.hv_updateMapY, out hv_updateSnapMapX, out hv_updateSnapMapY, out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                    }
                    if (hv_iFlag.S != "")
                    {
                        HTLog.Error("矫正点识别失败！" + hv_iFlag.S);
                        return;
                    }

                }
                if (_station.showRegion != null) _station.showRegion.Dispose();
                HOperatorSet.GenCrossContourXld(out _station.showRegion, _station.hv_foundU, _station.hv_foundV, 512, 0);
                _station.operation.ShowImage(htWindow, Image, _station.showRegion);
            }
            catch(Exception ex)
            {
                HTLog.Error("矫正点识别失败:"+ex.ToString());
                return;
            }
        }
        private void btnCrtCheckPosMdl_Click(object sender, EventArgs e)
        {
            try
            {
                if (toolForm == null || toolForm.IsDisposed)
                {
                    toolForm = new Form();
                    if (this.mTmpCheckPosMdlFrm != null)
                        if (!this.mTmpCheckPosMdlFrm.IsDisposed) this.mTmpCheckPosMdlFrm.Dispose();
                    if (_station.checkMdlMethod == (int)CheckModelMethod.MODEL_ORD)
                    {
                        this.mTmpCheckPosMdlFrm = new MainTemplateForm(ToolKits.TemplateEdit.MainTemplateForm.TemplateScence.Match,
                                        this.htWindow, new MainTemplateForm.TemplateParam());
                    }
                    else if (_station.checkMdlMethod == (int)CheckModelMethod.MODEL_EX)
                    {
                        this.mTmpCheckPosMdlFrm = new MainTemplateForm(ToolKits.TemplateEdit.MainTemplateForm.TemplateScence.CheckPosMatch,
                                         this.htWindow, new MainTemplateForm.TemplateParam());
                    }
                    toolForm.Controls.Clear();
                    this.toolForm.Controls.Add(this.mTmpCheckPosMdlFrm);
                    this.mTmpCheckPosMdlFrm.Dock = DockStyle.Fill;
                    toolForm.Size = new Size(300, 600);
                    toolForm.TopMost = true;
                    toolForm.Show();
                    int SH = Screen.PrimaryScreen.Bounds.Height;
                    int SW = Screen.PrimaryScreen.Bounds.Width;
                    toolForm.Location = new Point(SW - toolForm.Size.Width, SH / 8);

                    Task.Run(() =>
                    {
                        while (!mTmpCheckPosMdlFrm.WorkOver)
                        {
                            Thread.Sleep(200);
                        }
                        HTUi.TipHint("创建矫正点模板完成!");
                        HTLog.Info("创建矫正点模板完成!");
                        toolForm.Close();
                    });
                }
                else
                {
                    toolForm.Activate();
                }
            }
            catch(Exception ex)
            {
                HTLog.Error("创建矫正点模板失败:" + ex.ToString());
                return;
            }
        }

        private async void btnSaveCheckPosMdl_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.mTmpCheckPosMdlFrm.tmpResult.createTmpOK)
                {
                    HTUi.PopError("创建矫正点模板失败！");
                    return;
                }
                if (cbxCheckPosSelect.SelectedIndex == 0)//left
                {
                    _station.CheckPosModels = new Model();
                    _station.CheckPosModels.Dispose();
                    _station.CheckPosModels.showContour = this.mTmpCheckPosMdlFrm.tmpResult.showContour.CopyObj(1, -1);
                    _station.CheckPosModels.defRows = this.mTmpCheckPosMdlFrm.tmpResult.defRows;
                    _station.CheckPosModels.defCols = this.mTmpCheckPosMdlFrm.tmpResult.defCols;
                    _station.CheckPosModels.modelType = this.mTmpCheckPosMdlFrm.tmpResult.modelType;
                    _station.CheckPosModels.modelID = AutoMappingOperation.CopyModel(this.mTmpCheckPosMdlFrm.tmpResult.modelID, this.mTmpCheckPosMdlFrm.tmpResult.modelType);
                    _station.CheckPosModels.scoreThresh = this.mTmpCheckPosMdlFrm.mte_TmpPrmValues.Score;
                    _station.CheckPosModels.angleStart = this.mTmpCheckPosMdlFrm.mte_TmpPrmValues.AngleStart;
                    _station.CheckPosModels.angleExtent = this.mTmpCheckPosMdlFrm.mte_TmpPrmValues.AngleExtent;

                    _station.CheckPosModels.hLineRows = this.mTmpCheckPosMdlFrm.tmpResult.hRow;
                    _station.CheckPosModels.hLineCols = this.mTmpCheckPosMdlFrm.tmpResult.hCol;
                    _station.CheckPosModels.vLineRows = this.mTmpCheckPosMdlFrm.tmpResult.vRow;
                    _station.CheckPosModels.vLineCols = this.mTmpCheckPosMdlFrm.tmpResult.vCol;
                    _station.CheckPosModels.refPointsRow = this.mTmpCheckPosMdlFrm.tmpResult.cornorRow;
                    _station.CheckPosModels.refPointsCol = this.mTmpCheckPosMdlFrm.tmpResult.cornorCol;

                    //保存至硬盘
                    string modelPath = _station.ProductDir + "\\" + _station.ActivePdt + "\\CheckPosModels";
                    _station.CalibrUV2XYModelPath = modelPath;

                    Form_Wait.ShowForm();
                    await Task.Run(new Action(() =>
                    {
                        try
                        {
                            if (!_station.CheckPosModels.WriteModelCorrect(modelPath))
                            {
                                HTUi.PopError("保存矫正点模板失败！");
                                return;
                            }
                            //_station.scanIniConfig.Writedouble("ScanPoints", "checkPosX", _station.jFDLAFProductRecipe.CheckPosX);
                            //_station.scanIniConfig.Writedouble("ScanPoints", "checkPosY", _station.jFDLAFProductRecipe.CheckPosY);
                            HOperatorSet.WriteImage(Image, "tiff", 0, modelPath + "\\checkPos.tiff");

                            HTuple hv_iFlag, hv_defCol2X, hv_defCol2Y;
                            if (_station.CheckPosModels.matchRegion == null || !_station.CheckPosModels.matchRegion.IsInitialized())
                                HOperatorSet.GetDomain(Image, out _station.CheckPosModels.matchRegion);
                            if (_station.checkMdlMethod == (int)CheckModelMethod.MODEL_ORD)
                            {
                                if (_station.usedoubleCheck == 0)
                                {

                                }
                                else
                                {
                                    VisionMethon.coor_uvToxy_point(Image, _station.CheckPosModels.defRows, _station.CheckPosModels.defCols, _station.jFDLAFProductRecipe.CheckPosX, _station.jFDLAFProductRecipe.CheckPosY,
                                       _station.List_UV2XYResult[_station.SelectedIndex], out hv_defCol2X, out hv_defCol2Y, out hv_iFlag);
                                    if (hv_iFlag.S != "")
                                    {
                                        HTLog.Error("模板中心像素转换成实际坐标失败！" + hv_iFlag.S);
                                        return;
                                    }
                                    if(_station.jFDLAFProductRecipe==null)
                                    {
                                        HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                                        return;
                                    }
                                    else
                                    {
                                        _station.jFDLAFProductRecipe.SetMarkRealWorldPos1(hv_defCol2X.D, hv_defCol2Y.D);
                                        _station.jFDLAFProductRecipe.SaveParamsToCfg();
                                        JFHubCenter.Instance.RecipeManager.Save();
                                    }
                                    
                                }
                            }
                            else
                            {

                            }
                            HTUi.TipHint("保存矫正点模板成功！");
                            HTLog.Info("保存矫正点模板成功！");
                        }
                        catch (Exception ex)
                        {
                            HTUi.PopError("保存矫正点模板出错！\n" + ex.ToString());
                        }
                    }));
                    Form_Wait.CloseForm();
                }
                else
                {
                    _station.CheckPosRModels = new Model();
                    _station.CheckPosRModels.Dispose();
                    _station.CheckPosRModels.showContour = this.mTmpCheckPosMdlFrm.tmpResult.showContour.CopyObj(1, -1);
                    _station.CheckPosRModels.defRows = this.mTmpCheckPosMdlFrm.tmpResult.defRows;
                    _station.CheckPosRModels.defCols = this.mTmpCheckPosMdlFrm.tmpResult.defCols;
                    _station.CheckPosRModels.modelType = this.mTmpCheckPosMdlFrm.tmpResult.modelType;
                    _station.CheckPosRModels.modelID = AutoMappingOperation.CopyModel(this.mTmpCheckPosMdlFrm.tmpResult.modelID, this.mTmpCheckPosMdlFrm.tmpResult.modelType);
                    _station.CheckPosRModels.scoreThresh = this.mTmpCheckPosMdlFrm.mte_TmpPrmValues.Score;
                    _station.CheckPosRModels.angleStart = this.mTmpCheckPosMdlFrm.mte_TmpPrmValues.AngleStart;
                    _station.CheckPosRModels.angleExtent = this.mTmpCheckPosMdlFrm.mte_TmpPrmValues.AngleExtent;

                    _station.CheckPosRModels.hLineRows = this.mTmpCheckPosMdlFrm.tmpResult.hRow;
                    _station.CheckPosRModels.hLineCols = this.mTmpCheckPosMdlFrm.tmpResult.hCol;
                    _station.CheckPosRModels.vLineRows = this.mTmpCheckPosMdlFrm.tmpResult.vRow;
                    _station.CheckPosRModels.vLineCols = this.mTmpCheckPosMdlFrm.tmpResult.vCol;
                    _station.CheckPosRModels.refPointsRow = this.mTmpCheckPosMdlFrm.tmpResult.cornorRow;
                    _station.CheckPosRModels.refPointsCol = this.mTmpCheckPosMdlFrm.tmpResult.cornorCol;

                    //保存至硬盘
                    string modelPath = _station.ProductDir + "\\" + _station.ActivePdt + "\\CheckPosModelsR";
                    //App.obj_Operations.CalibrUV2XYModelPath = modelPath;

                    Form_Wait.ShowForm();
                    await Task.Run(new Action(() =>
                    {
                        try
                        {
                            if (!_station.CheckPosRModels.WriteModelCorrect(modelPath))
                            {
                                HTUi.PopError("保存矫正点模板失败！");
                                return;
                            }
                            //_station.scanIniConfig.Writedouble("ScanPoints", "checkPosRX", _station.jFDLAFProductRecipe.CheckPosRX);
                            //_station.scanIniConfig.Writedouble("ScanPoints", "checkPosRY", _station.jFDLAFProductRecipe.CheckPosRY);
                            HOperatorSet.WriteImage(Image, "tiff", 0, modelPath + "\\checkPos.tiff");

                            HTuple hv_iFlag, hv_defCol2X, hv_defCol2Y;
                            if (_station.CheckPosRModels.matchRegion == null || !_station.CheckPosRModels.matchRegion.IsInitialized())
                                HOperatorSet.GetDomain(Image, out _station.CheckPosRModels.matchRegion);
                            if (_station.checkMdlMethod == (int)CheckModelMethod.MODEL_ORD)
                            {
                                if (_station.usedoubleCheck == 0)
                                {

                                }
                                else
                                {
                                    VisionMethon.coor_uvToxy_point(Image, _station.CheckPosRModels.defRows, _station.CheckPosRModels.defCols, _station.jFDLAFProductRecipe.CheckPosRX, _station.jFDLAFProductRecipe.CheckPosRY,
                                       _station.List_UV2XYResult[_station.SelectedIndex], out hv_defCol2X, out hv_defCol2Y, out hv_iFlag);
                                    if (hv_iFlag.S != "")
                                    {
                                        HTLog.Error("模板中心像素转换成实际坐标失败！" + hv_iFlag.S);
                                        return;
                                    }
                                    if (_station.jFDLAFProductRecipe == null)
                                    {
                                        HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                                        return;
                                    }
                                    else
                                    {
                                        _station.jFDLAFProductRecipe.SetMarkRealWorldPos2(hv_defCol2X.D, hv_defCol2Y.D);
                                        _station.jFDLAFProductRecipe.SaveParamsToCfg();
                                        JFHubCenter.Instance.RecipeManager.Save();
                                    }
                                }
                            }
                            else
                            {

                            }
                            HTUi.TipHint("保存矫正点模板成功！");
                            HTLog.Info("保存矫正点模板成功！");
                        }
                        catch (Exception ex)
                        {
                            HTUi.PopError("保存矫正点模板出错！\n" + ex.ToString());
                        }
                    }));
                    Form_Wait.CloseForm();
                }
            }
            catch (Exception ex)
            {
                HTUi.PopError("保存矫正点模板失败！\n" + ex.ToString());
            }
        }

        private void btnSetXY2StartPos_Click(object sender, EventArgs e)
        {
            try
            {
                double nowX=0,nowY = 0;
                string errMsg = "";
                _station.GetAxisPosition(_station.AxisXYZ[0], out nowX, out errMsg);
                _station.GetAxisPosition(_station.AxisXYZ[1], out nowY, out errMsg);
                numStartX.Value = (decimal)nowX;
                numStartY.Value = (decimal)nowY;
            }
            catch (Exception ex)
            {
                HTUi.PopError("获取当前点位失败！\n" + ex.ToString());
            }
        }

        private void btnSetXY2CheckPos_Click(object sender, EventArgs e)
        {
            try
            {
                double nowX = 0, nowY = 0;
                string errMsg = "";
                _station.GetAxisPosition(_station.AxisXYZ[0], out nowX, out errMsg);
                _station.GetAxisPosition(_station.AxisXYZ[1], out nowY, out errMsg);
                numCheckPosX.Value = (decimal)nowX;
                numCheckPosY.Value = (decimal)nowY;
            }
            catch (Exception ex)
            {
                HTUi.PopError("加载矫正点图像失败！\n" + ex.ToString());
            }
        }

        private void btnSetXYEndPos_Click(object sender, EventArgs e)
        {
            try
            {
                string errMsg = "";
                double nowX = 0, nowY = 0;
                _station.GetAxisPosition(_station.AxisXYZ[0], out nowX, out errMsg);
                _station.GetAxisPosition(_station.AxisXYZ[1], out nowY, out errMsg);
                numEndX.Value = (decimal)nowX;
                numEndY.Value = (decimal)nowY;
            }
            catch (Exception ex)
            {
                HTUi.PopError("获取当前点位失败！\n" + ex.ToString());
            }
        }

        private void btnSnap_Click(object sender, EventArgs e)
        {
            try
            {
                string errMsg = "";
                //if (_station.operation.SWPosTrig(_station.AxisX, out errMsg) != 0)
                //{
                //    HTUi.PopError(errMsg);
                //    return;
                //}
                //4. 取图
                HOperatorSet.GenEmptyObj(out Image);
                if (_station.operation.CaputreOneImage(_station.CamereDev[_station.SelectedIndex], "Halcon", out Image, out errMsg) != 0)
                {
                    HTUi.PopError("采集图像失败：" + errMsg);
                    return;
                }
                _station.operation.ShowImage(htWindow, Image, null);
            }
            catch(Exception ex)
            {
                HTUi.PopError("采集图像失败：" + ex.ToString());
            }
        }

        private void btnCmrAxisTool_Click(object sender, EventArgs e)
        {
            FrmCamAxisMotion frm = FrmCamAxisMotion.Instance;
            if (frm == null || frm.IsDisposed)
            {
                frm = new FrmCamAxisMotion();
                frm.SetStation(_station as IJFStation);
                frm.TopMost = false;
                int SH = Screen.PrimaryScreen.Bounds.Height;
                int SW = Screen.PrimaryScreen.Bounds.Width;
                frm.Show();
                frm.Location = new Point((SW - frm.Size.Width) / 2, (SH - frm.Size.Height) / 2);
            }
            else
            {
                frm.Activate();
            }

        }

        private void btnSavePara_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (fovWidth >= _station.jFDLAFProductRecipe.DieWidth && fovHeight >= _station.jFDLAFProductRecipe.DieHeight)//大视野vs小芯片
                {
                    if (_station.ICFovRow.Count > 1)
                    {
                        HTUi.PopError("大视野小芯片只能有一个视野，请删除多余视野");
                        return;
                    }
                }

                if (!UpdateAllFovAndTaskName())
                {
                    return;
                }

                _station.genMapStartX = (double)numStartX.Value;
                _station.genMapStartY = (double)numStartY.Value;
                _station.genMapEndX = (double)numEndX.Value;
                _station.genMapEndY = (double)numEndY.Value;
                _station.sameSpace = (double)numSameSpace.Value;
                _station.jFDLAFProductRecipe.ScaleFactor = (double)numScaleFactor.Value;
                _station.lctScoreThresh = (double)numLctScoreThresh.Value;
                _station.usedoubleCheck = Convert.ToInt32(ckbdoubleCheck.Checked);
                if (cbxCheckPosSelect.SelectedIndex == 0)
                {
                    _station.jFDLAFProductRecipe.CheckPosX = (double)numCheckPosX.Value;
                    _station.jFDLAFProductRecipe.CheckPosY = (double)numCheckPosY.Value;
                    _station.jFDLAFProductRecipe.CheckPosScoreThresh = (double)numCheckPosScoreThresh.Value;

                    if (_station.jFDLAFProductRecipe == null)
                    {
                        HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                        return;
                    }
                    else
                    {
                        _station.jFDLAFProductRecipe.SetMarkSnapPos1(_station.jFDLAFProductRecipe.CheckPosX, _station.jFDLAFProductRecipe.CheckPosY);
                        _station.jFDLAFProductRecipe.SaveParamsToCfg();
                        JFHubCenter.Instance.RecipeManager.Save();
                    }
                }
                else
                {
                    _station.jFDLAFProductRecipe.CheckPosRX = (double)numCheckPosX.Value;
                    _station.jFDLAFProductRecipe.CheckPosRY = (double)numCheckPosY.Value;
                    _station.jFDLAFProductRecipe.CheckPosRScoreThresh = (double)numCheckPosScoreThresh.Value;

                    if (_station.jFDLAFProductRecipe == null)
                    {
                        HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                        return;
                    }
                    else
                    {
                        _station.jFDLAFProductRecipe.SetMarkSnapPos2(_station.jFDLAFProductRecipe.CheckPosRX, _station.jFDLAFProductRecipe.CheckPosRY);
                        _station.jFDLAFProductRecipe.SaveParamsToCfg();
                        JFHubCenter.Instance.RecipeManager.Save();
                    }
                }
                _station.jFDLAFProductRecipe.WidthFactor = (double)numWidthFactor.Value;
                _station.jFDLAFProductRecipe.HeightFactor = (double)numHeightFactor.Value;
                _station.checkMdlMethod = (int)cbxScanModelMethod.SelectedIndex;


                _station.SaveStationParams();
                _station.SaveCfg();
                
                HTUi.TipHint(_station.ActivePdt+"产品图谱参数保存成功！");
                HTLog.Info(_station.ActivePdt + "产品图谱参数保存成功！");
            }
            catch (Exception ex)
            {
                HTUi.PopError(_station.ActivePdt + "产品图谱参数保存失败。\n" + ex.ToString());
                HTLog.Error(_station.ActivePdt + "产品图谱参数保存失败。\n" + ex.ToString());
            }
        }

        private void btnLoadMapImg_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                string modelPath = _station.ProductDir + "\\" + _station.ActivePdt;
                if (!File.Exists(modelPath + "\\frameMapImg.tiff"))
                {
                    HTUi.PopError("未生成过该产品图谱！\n无法找到该产品图谱文件！");
                }
                HOperatorSet.ReadImage(out Image, modelPath + "\\frameMapImg.tiff");
                _station.operation.ShowImage(htWindow, Image, null);
                _station.frameMapImg = Image.CopyObj(1, -1);
                _station.hv_xSnapPosLT = new HTuple();
                _station.hv_ySnapPosLT = new HTuple();
                HOperatorSet.GenEmptyObj(out icImage);//清除IC Image
                if (File.Exists(_station.ProductDir + "\\" + _station.ActivePdt + "\\hv_xSnapPosLT.tup"))
                    HOperatorSet.ReadTuple(_station.ProductDir + "\\" + _station.ActivePdt + "\\hv_xSnapPosLT.tup", out _station.hv_xSnapPosLT);
                if (File.Exists(_station.ProductDir + "\\" + _station.ActivePdt + "\\hv_ySnapPosLT.tup"))
                    HOperatorSet.ReadTuple(_station.ProductDir + "\\" + _station.ActivePdt + "\\hv_ySnapPosLT.tup", out _station.hv_ySnapPosLT);
                HTUi.TipHint("加载产品图谱成功！");
                HTLog.Info("加载产品图谱成功！");

                btnGenICScanPos.Enabled = false;
            }
            catch (Exception ex)
            {
                HTUi.PopError("加载产品图谱失败！\n" + ex.ToString());
            }
        }

        private void btnCcCenterDis_Click(object sender, EventArgs e)
        {
            try
            {
                if (htWindow.RegionType != "Point")
                {
                    MessageBox.Show("请先画个点作为绘制中心！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _station.showRegion = new HObject();
                ToolKits.FunctionModule.Vision.GenROI(htWindow, "contour", ref _station.showRegion);


                HTuple r, c, area, width, height;
                HObject centerPoint = null;

                HOperatorSet.AreaCenterPointsXld(_station.showRegion, out area, out r, out c);
                HOperatorSet.GenCrossContourXld(out _station.showRegion, r, c, 100, 0);
                _station.operation.ShowImage(htWindow, htWindow.Image, _station.showRegion);

                HOperatorSet.GetImageSize(htWindow.Image, out width, out height);

                HOperatorSet.GenCrossContourXld(out centerPoint, height / 2, width / 2, 100, 0);
                HOperatorSet.SetColor(htWindow.HTWindow.HalconWindow, "pink");
                HOperatorSet.DispXld(centerPoint, htWindow.HTWindow.HalconWindow);
                HOperatorSet.SetColor(htWindow.HTWindow.HalconWindow, "yellow");


                double vdx = (c * 2 - width) / 2;
                double vdy = -(r * 2 - height) / 2;
                tbdx.Text = (vdx * Math.Abs(_station.List_UV2XYResult[_station.SelectedIndex][1].D)).ToString();
                tbdy.Text = (vdy * Math.Abs(_station.List_UV2XYResult[_station.SelectedIndex][3].D)).ToString();
            }
            catch (Exception ex)
            {
                HTLog.Error(ex.ToString());
                return;
            }
        }

        private void btnFirstDie_Click(object sender, EventArgs e)
        {
            try
            {
                _station.operation.SnapPos(_station.ClipMapPostions[0].x, _station.ClipMapPostions[0].y, _station.ClipMapPostions[0].z, htWindow, out _station.Image);
            }
            catch (Exception ex)
            {
                HTLog.Error(ex.ToString());
                return;
            }
        }

        private void button_WriteDxDY_Click(object sender, EventArgs e)
        {
            try
            {
                _station.RelativeMark_X = double.Parse(tbdx.Text);
                _station.RelativeMark_Y = double.Parse(tbdy.Text);
                _station.SymmetryMark = cbx_Symmetry.Checked;
                _station.operation.Save();
            }
            catch (Exception ex)
            {
                HTLog.Error(ex.ToString());
                return;
            }
        }

        private void btn_MarkFirstDie_Click(object sender, EventArgs e)
        {
            try
            {
                _station.operation.EC_Printer(_station.ClipMapPostions[0].x + double.Parse(tbdx.Text) + _station.Ref_Mark_x,
                    _station.ClipMapPostions[0].y + double.Parse(tbdy.Text) + _station.Ref_Mark_y,
                    _station.RelativeMark_Z,
                    _station.IsX, _station.RXorY);
                _station.operation.SnapPos(_station.ClipMapPostions[0].x, _station.ClipMapPostions[0].y, _station.ClipMapPostions[0].z, htWindow, out _station.Image);
            }
            catch (Exception ex)
            {
                HTLog.Error(ex.ToString());
                return;
            }
        }

        private void btn_MarkReview_Click(object sender, EventArgs e)
        {
            try
            {
                _station.operation.EC_Printer(_station.ClipMapPostions[1].x + double.Parse(tbdx.Text) - _station.Ref_Mark_x,
                    _station.ClipMapPostions[1].y + double.Parse(tbdy.Text) + _station.Ref_Mark_y,
                   _station.RelativeMark_Z,
                    _station.IsX, _station.RXorY);
                _station.operation.SnapPos(_station.ClipMapPostions[1].x, _station.ClipMapPostions[1].y, _station.ClipMapPostions[1].z, htWindow, out _station.Image);
            }
            catch (Exception ex)
            {
                HTLog.Error(ex.ToString());
                return;
            }
        }

        private void cbxScanModelMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbxCheckPosSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxCheckPosSelect.SelectedIndex == 0)//坐矫正点
            {
                if (_station.jFDLAFProductRecipe != null)
                {
                    numCheckPosX.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosX;
                    numCheckPosY.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosY;
                    numCheckPosScoreThresh.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosScoreThresh;
                }
            }
            else//右矫正点
            {
                if (_station.jFDLAFProductRecipe != null)
                {
                    numCheckPosX.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosRX;
                    numCheckPosY.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosRY;
                    numCheckPosScoreThresh.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosRScoreThresh;
                }
            }
        }

        private void btn_UpdateMapTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.CheckPosModels == null || _station.CheckPosRModels == null)
                {
                    HTUi.PopHint("请先创建左右矫正点模板！");
                    return;
                }
                HTuple hv_iFlag = null;
                HTuple hv_du = null;
                HTuple hv_dv = null;
                //VisionMethon.update_map_correction(_station.clipMapX, _station.clipMapY, _station.snapMapX, _station.snapMapY,
                //    _station.hv_updateCoorX, _station.hv_updateCoorY, _station.hv_modelHmap, _station.hv_defCol2XR, _station.hv_defRow2YR,
                //    _station.hv_updateCoorXR, _station.hv_updateCoorYR, out _station.hv_updateMapX, out _station.hv_updateMapY,
                //    out _station.hv_updateSnapMapX, out _station.hv_updateSnapMapY, out hv_iFlag);
                VisionMethon.update_map_correction(_station.clipMapX, _station.clipMapY, _station.snapMapX, _station.snapMapY,
                    _station.hv_defCol2X, _station.hv_defRow2Y, _station.hv_defCol2XR, _station.hv_defRow2YR, _station.hv_updateCoorX,
                    _station.hv_updateCoorY, _station.hv_updateCoorXR, _station.hv_updateCoorYR, _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_updateMapX,
                    out _station.hv_updateMapY, out _station.hv_updateSnapMapX, out _station.hv_updateSnapMapY, out hv_iFlag, out hv_du, out hv_dv);
                if (hv_iFlag.S != "")
                {
                    HTLog.Error("矫正后点位信息更新失败！" + hv_iFlag.S);
                    return;
                }
                string msg = "";
                msg += String.Format("\r\n");
                msg += String.Format("{0},{1},{2},{3},{4},{5},{6},{7}\r\n", _station.hv_defCol2X.D, _station.hv_defRow2Y.D,
                    _station.hv_defCol2XR.D, _station.hv_defRow2YR.D, _station.hv_updateCoorX.D, _station.hv_updateCoorY.D,
                    _station.hv_updateCoorXR.D, _station.hv_updateCoorYR.D);
                File.AppendAllText("D:\\CheckMethodRecord.csv", msg);
            }
            catch (Exception ex)
            {
                HTLog.Error("矫正测试失败:" + ex.ToString());
                return;
            }
        }

        private void ckbdoubleCheck_CheckedChanged(object sender, EventArgs e)
        {
            cbxCheckPosSelect.Items.Clear();
            if (ckbdoubleCheck.Checked)
            {
                cbxCheckPosSelect.Items.AddRange(new object[] { "左矫正点", "右矫正点" });
                btn_UpdateMapTest.Visible = true;
            }
            else
            {
                cbxCheckPosSelect.Items.AddRange(new object[] { "左矫正点"});
                btn_UpdateMapTest.Visible = false;
                numCheckPosX.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosX;
                numCheckPosY.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosY;
                numCheckPosScoreThresh.Value = (decimal)_station.jFDLAFProductRecipe.CheckPosScoreThresh;
            }
            _station.usedoubleCheck = Convert.ToInt32(ckbdoubleCheck.Checked);
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    App.obj_Process._ngLocation = new List<NGLocation>();
        //    App.obj_Process.ThisMarkPostions = new List<ImagePosition>();
        //    for (int row = 0; row < 3; row++)
        //    {
        //        for (int col = 0; col < 6; col++)
        //        {
        //            ImagePosition p = new ImagePosition();
        //            p.b = 0;
        //            p.r = row;
        //            p.c = col;
        //            p.x = 0;
        //            p.y = 0;
        //            p.z = 0;
        //            App.obj_Process.ThisMarkPostions.Add(p);
        //        }
        //    }
        //    NGLocation ng1 = new NGLocation();
        //    ng1.NGr = 0;
        //    ng1.NGc = 0;
        //    NGLocation ng2 = new NGLocation();
        //    ng2.NGr = 1;
        //    ng2.NGc = 2;
        //    NGLocation ng3 = new NGLocation();
        //    ng3.NGr = 2;
        //    ng3.NGc = 5;
        //    App.obj_Process._ngLocation.Add(ng1);
        //    App.obj_Process._ngLocation.Add(ng2);
        //    App.obj_Process._ngLocation.Add(ng3);
        //    string msg = "";
        //    for (int j = 0; j < App.obj_Process._ngLocation.Count; j++)
        //    {
        //        if (j == 0) msg += String.Format("\r\n");
        //        msg += String.Format("{0},{1}\r\n", App.obj_Process._ngLocation[j].NGr, App.obj_Process._ngLocation[j].NGc);
        //    }
        //    File.AppendAllText("D:\\MarkPositionPre.csv", msg);
        //    msg = "";
        //    for (int i = 0; i < App.obj_Process.ThisMarkPostions.Count; i++)
        //    {
        //        msg += String.Format("{0},{1},{2},{3}\r\n", App.obj_Process.ThisMarkPostions[i].r, App.obj_Process.ThisMarkPostions[i].c,
        //            App.obj_Process.ThisMarkPostions[i].x, App.obj_Process.ThisMarkPostions[i].y);
        //    }
        //    File.AppendAllText("D:\\MarkPositionPre.csv", msg);
        //    _station.UpdataMarkPosition(App.obj_Process._ngLocation, 0, 0, ref App.obj_Process.ThisMarkPostions);
        //    _station.UpdataMarkPosition(App.obj_Process._ngLocation, 0, 1, ref App.obj_Process.ThisMarkPostions);
        //    msg = "";
        //    for (int i = 0; i < App.obj_Process.ThisMarkPostions.Count; i++)
        //    {
        //        if (i == 0) msg += String.Format("\r\n");
        //        msg += String.Format("{0},{1},{2},{3}\r\n", App.obj_Process.ThisMarkPostions[i].r, App.obj_Process.ThisMarkPostions[i].c,
        //            App.obj_Process.ThisMarkPostions[i].x, App.obj_Process.ThisMarkPostions[i].y);
        //    }
        //    File.AppendAllText("D:\\MarkPositionAft.csv", msg);
        //}

        private void btnLoadCheckPosImg_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (cbxCheckPosSelect.SelectedIndex == 0)//left
                {
                    string modelPath = _station.ProductDir + "\\" + _station.ActivePdt + "\\CheckPosModels";
                    if(!Directory.Exists(modelPath))
                    {
                        HTUi.PopError("产品信息："+ modelPath+", 该文件夹不存在");
                        return;
                    }
                    if (!_station.CheckPosModels.ReadModelCorrect(modelPath))
                    {
                        HTUi.PopError("无法读取该产品左矫正点模板信息.\n");
                        return;
                    }
                  
                    if (!File.Exists(modelPath + "\\checkPos.tiff"))
                    {
                        HTUi.PopError("未生成过该产品矫正点！\n无法找到该产品矫正点文件！");
                        return;
                    }
                    HOperatorSet.ReadImage(out Image, modelPath + "\\checkPos.tiff");
                    HTuple hv_updateSnapMapX; HTuple hv_updateSnapMapY; HTuple hv_iFlag;
                    if (_station.CheckPosModels.matchRegion == null || !_station.CheckPosModels.matchRegion.IsInitialized())
                        HOperatorSet.GetDomain(Image, out _station.CheckPosModels.matchRegion);
                    if (_station.checkMdlMethod == (int)CheckModelMethod.MODEL_ORD)
                    {
                        if (_station.usedoubleCheck == 0)
                        {
                            VisionMethon.update_map_points(Image, _station.CheckPosModels.matchRegion, _station.CheckPosModels.modelType, _station.CheckPosModels.modelID,
                                _station.jFDLAFProductRecipe.CheckPosScoreThresh, _station.clipMapX, _station.clipMapY, _station.snapMapX, _station.snapMapY, _station.List_UV2XYResult[_station.SelectedIndex],
                                out _station.hv_updateMapX, out _station.hv_updateMapY, out hv_updateSnapMapX, out hv_updateSnapMapY, out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                        }
                        else
                        {
                            VisionMethon.coor_uvToxy_point(Image, _station.CheckPosModels.defRows, _station.CheckPosModels.defCols, _station.jFDLAFProductRecipe.CheckPosX, _station.jFDLAFProductRecipe.CheckPosY,
                               _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_defCol2X, out _station.hv_defRow2Y, out hv_iFlag);
                            if (hv_iFlag.S != "")
                            {
                                HTLog.Error("模板中心像素转换成实际坐标失败！" + hv_iFlag.S);
                                return;
                            }
                            VisionMethon.Map_Points_Match(Image, _station.CheckPosModels.matchRegion, _station.CheckPosModels.modelType, _station.CheckPosModels.modelID,
                                _station.jFDLAFProductRecipe.CheckPosScoreThresh, _station.CheckPosModels.defRows, _station.CheckPosModels.defCols, _station.hv_defCol2X, _station.hv_defRow2Y,
                                _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_modelHmap, out _station.hv_updateCoorX, out _station.hv_updateCoorY,
                                out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                        }
                    }
                    else
                    {
                        VisionMethon.update_map_points_Test(Image, _station.CheckPosModels.matchRegion, _station.CheckPosModels.modelType, _station.CheckPosModels.modelID,
                           _station.CheckPosModels.defRows, _station.CheckPosModels.defCols, _station.CheckPosModels.hLineRows, _station.CheckPosModels.hLineCols, _station.CheckPosModels.vLineRows,
                           _station.CheckPosModels.vLineCols, _station.CheckPosModels.refPointsRow, _station.CheckPosModels.refPointsCol, _station.clipMapX, _station.clipMapY, _station.snapMapX, _station.snapMapY, _station.List_UV2XYResult[_station.SelectedIndex],
                           out _station.hv_updateMapX, out _station.hv_updateMapY, out hv_updateSnapMapX, out hv_updateSnapMapY, out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                    }

                    if (hv_iFlag.S != "")
                    {
                        HTLog.Error("矫正点识别失败！" + hv_iFlag.S);
                        return;
                    }
                }
                else
                {
                    string modelPath = _station.ProductDir + "\\" + _station.ActivePdt + "\\CheckPosModelsR";
                    if (!Directory.Exists(modelPath))
                    {
                        HTUi.PopError("产品信息：" + modelPath + ", 该文件夹不存在");
                        return;
                    }
                    if (!_station.CheckPosRModels.ReadModelCorrect(modelPath))
                    {
                        HTUi.PopError("无法读取该产品右矫正点模板信息.\n");
                        return;
                    }

                    if (!File.Exists(modelPath + "\\checkPos.tiff"))
                    {
                        HTUi.PopError("未生成过该产品矫正点！\n无法找到该产品矫正点文件！");
                    }
                    HOperatorSet.ReadImage(out Image, modelPath + "\\checkPos.tiff");
                    HTuple hv_updateSnapMapX; HTuple hv_updateSnapMapY; HTuple hv_iFlag;
                    if (_station.CheckPosRModels.matchRegion == null || !_station.CheckPosRModels.matchRegion.IsInitialized())
                        HOperatorSet.GetDomain(Image, out _station.CheckPosRModels.matchRegion);
                    if (_station.checkMdlMethod == (int)CheckModelMethod.MODEL_ORD)
                    {
                        //VisionMethon.update_map_points(Image, _station.CheckPosRModels.matchRegion, _station.CheckPosRModels.modelType, _station.CheckPosRModels.modelID,
                        //    _station.jFDLAFProductRecipe.CheckPosRScoreThresh, _station.clipMapX, _station.clipMapY, _station.snapMapX, _station.snapMapY, _station.List_UV2XYResult[_station.SelectedIndex],
                        //    out _station.hv_updateMapX, out _station.hv_updateMapY, out hv_updateSnapMapX, out hv_updateSnapMapY, out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                        VisionMethon.coor_uvToxy_point(Image, _station.CheckPosRModels.defRows, _station.CheckPosRModels.defCols, _station.jFDLAFProductRecipe.CheckPosRX, _station.jFDLAFProductRecipe.CheckPosRY,
                          _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_defCol2XR, out _station.hv_defRow2YR, out hv_iFlag);
                        if (hv_iFlag.S != "")
                        {
                            HTLog.Error("模板中心像素转换成实际坐标失败！" + hv_iFlag.S);
                            return;
                        }
                        VisionMethon.Map_Points_Match(Image, _station.CheckPosRModels.matchRegion, _station.CheckPosRModels.modelType, _station.CheckPosRModels.modelID,
                           _station.jFDLAFProductRecipe.CheckPosRScoreThresh, _station.CheckPosRModels.defRows, _station.CheckPosRModels.defCols, _station.hv_defCol2XR, _station.hv_defRow2YR,
                           _station.List_UV2XYResult[_station.SelectedIndex], out _station.hv_modelHmapR, out _station.hv_updateCoorXR, out _station.hv_updateCoorYR,
                           out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                    }
                    else
                    {
                        VisionMethon.update_map_points_Test(Image, _station.CheckPosRModels.matchRegion, _station.CheckPosRModels.modelType, _station.CheckPosRModels.modelID,
                           _station.CheckPosRModels.defRows, _station.CheckPosRModels.defCols, _station.CheckPosRModels.hLineRows, _station.CheckPosRModels.hLineCols, _station.CheckPosRModels.vLineRows,
                           _station.CheckPosRModels.vLineCols, _station.CheckPosRModels.refPointsRow, _station.CheckPosRModels.refPointsCol, _station.clipMapX, _station.clipMapY, _station.snapMapX, _station.snapMapY, _station.List_UV2XYResult[_station.SelectedIndex],
                           out _station.hv_updateMapX, out _station.hv_updateMapY, out hv_updateSnapMapX, out hv_updateSnapMapY, out _station.hv_foundU, out _station.hv_foundV, out hv_iFlag);
                    }

                    if (hv_iFlag.S != "")
                    {
                        HTLog.Error("矫正点识别失败！" + hv_iFlag.S);
                        return;
                    }
                }
                if (_station.showRegion != null) _station.showRegion.Dispose();
                HOperatorSet.GenCrossContourXld(out _station.showRegion, _station.hv_foundU, _station.hv_foundV, 512, 0);
                _station.operation.ShowImage(htWindow, Image, _station.showRegion);
                HTUi.TipHint("加载矫正点图像成功！");
                HTLog.Info("加载矫正点图像成功！");
            }
            catch (Exception ex)
            {
                HTUi.PopError("加载矫正点图像失败！\n" + ex.ToString());
            }
        }


        /// <summary>
        /// 增加dgv控件列表
        /// </summary>
        private void EditColumnHeadText()
        {
            dgvFovInf.AllowUserToAddRows = false;    //禁止添加行
            dgvFovInf.AllowUserToDeleteRows = false;   //禁止删除行

            dgvFovInf.Columns.Clear();

            DataGridViewLinkColumn indexColumn = new DataGridViewLinkColumn();
            indexColumn.HeaderText = "索引名";
            indexColumn.SortMode= DataGridViewColumnSortMode.NotSortable;
            dgvFovInf.Columns.Add(indexColumn);

            DataGridViewCheckBoxColumn showColumn = new DataGridViewCheckBoxColumn();
            showColumn.HeaderText = "显示";
            showColumn.SortMode= DataGridViewColumnSortMode.NotSortable;
            dgvFovInf.Columns.Add(showColumn);

            DataGridViewTextBoxColumn fovNameColumn = new DataGridViewTextBoxColumn();
            fovNameColumn.HeaderText = "视野名称";
            fovNameColumn.SortMode= DataGridViewColumnSortMode.NotSortable;
            dgvFovInf.Columns.Add(fovNameColumn);

            DataGridViewTextBoxColumn taskNumColumn = new DataGridViewTextBoxColumn();
            taskNumColumn.HeaderText = "光源配置数";
            taskNumColumn.SortMode= DataGridViewColumnSortMode.NotSortable;
            dgvFovInf.Columns.Add(taskNumColumn);
        }

        private delegate void AddDisplayDateGridView(System.Windows.Forms.DataGridView dataGridView, int fovindex, bool checkStatus, string fovname, int taskcount);
        /// <summary>
        /// 新增DataGridView控件数据
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="dateTime"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        private void AddDataGridViewFunction(System.Windows.Forms.DataGridView dataGridView, int fovindex, bool checkStatus, string fovname, int taskcount)
        {
            if (dataGridView.InvokeRequired)
            {
                AddDisplayDateGridView ss = new AddDisplayDateGridView(AddDataGridViewFunction);
                Invoke(ss, new object[] { dataGridView, fovindex, checkStatus, fovname, taskcount });
            }
            else
            {
                int index = dataGridView.Rows.Add();
                for(int i=0;i<index;i++)
                    dataGridView.Rows[i].Selected = false;
                dataGridView.Rows[index].Cells[0].Value = fovindex;
                dataGridView.Rows[index].Cells[1].Value = checkStatus;
                dataGridView.Rows[index].Cells[2].Value = fovname;
                dataGridView.Rows[index].Cells[3].Value = taskcount;
                dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.Rows.Count - 1;
                dataGridView.Rows[dataGridView.Rows.Count - 1].Selected = true;
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private delegate void DeleteDisplayDateGridView(System.Windows.Forms.DataGridView dataGridView, int selectRowIndex);
        /// <summary>
        /// 删除DataGridView控件数据
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="dateTime"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        private void DeleteDataGridViewFunction(System.Windows.Forms.DataGridView dataGridView, int selectRowIndex)
        {
            if (dataGridView.InvokeRequired)
            {
                DeleteDisplayDateGridView ss = new DeleteDisplayDateGridView(DeleteDataGridViewFunction);
                Invoke(ss, new object[] { dataGridView, selectRowIndex });
            }
            else
            {
                dataGridView.Rows.RemoveAt(selectRowIndex);
                if (dataGridView.Rows.Count > 0)
                {
                    dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.Rows.Count - 1;
                    dataGridView.Rows[dataGridView.Rows.Count - 1].Selected = true;
                }
                System.Windows.Forms.Application.DoEvents();
            }
        }


        bool flag = true;
        /// <summary>
        /// 增加一个FOV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAddFov_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flag)
                {
                    HTUi.PopError("请先右击完成当前视野新增再进行新的视野新增");
                    return;
                }

                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (icImage == null)
                {
                    HTUi.PopError("未成功加载IC图像");
                    return;
                }

                if (fovWidth >= _station.jFDLAFProductRecipe.DieWidth && fovHeight >= _station.jFDLAFProductRecipe.DieHeight)//大视野vs小芯片
                {
                    if (_station.ICFovRow.Count >= 1)
                    {
                        HTUi.PopError("大视野小芯片只能有一个视野，请勿添加多个视野；若已存在多个视野，请仅留下一个视野");
                        return;
                    }
                }

                flag = false;
                HTuple rowPosition = new HTuple();
                HTuple colPosition = new HTuple();
                //自动生成drag region
                if (!GenDragRegion(out rowPosition, out colPosition))
                {
                    HTUi.PopError("添加视野边界请勿超出图片所在区域");
                    flag = true;
                    return;
                }


                //data更新
                HOperatorSet.TupleConcat(row, rowPosition, out row);
                HOperatorSet.TupleConcat(col, colPosition, out col);

                _station.ICFovRow.Add(rowPosition.D);
                _station.ICFovCol.Add(colPosition.D);

                int fovIndex = row.Length - 1;

                int newFovNumber = 0;

                bool flagRepert = true;
                while (flagRepert)
                {
                    flagRepert = false;
                    foreach (string index in _station.jFDLAFProductRecipe.visionCfgParams.Keys)
                    {
                        if (_station.jFDLAFProductRecipe.visionCfgParams[index].ContainsKey(string.Format("Fov{0}", newFovNumber)))
                        {
                            newFovNumber++;
                            flagRepert = true;
                            break;
                        }
                    }
                }

                JFXmlDictionary<string, JFXmlDictionary<string, string>> fovVisionCfg = new JFXmlDictionary<string, JFXmlDictionary<string, string>>();
                JFXmlDictionary<string, string> keyValuePairs = new JFXmlDictionary<string, string>();
                keyValuePairs.Add("Task0", "");
                fovVisionCfg.Add(string.Format("Fov{0}", newFovNumber), keyValuePairs);
                _station.jFDLAFProductRecipe.visionCfgParams.Add(fovIndex.ToString(), fovVisionCfg);

                AddDataGridViewFunction(dgvFovInf, fovIndex, true, string.Format("Fov{0}", newFovNumber), 1);


                ChangeShowSelectRegion(-1);
                _station.operation.ShowImageEx(htWindow, icImage, allShowRegion);

                _station.SaveStationParams();
                _station.SaveCfg();

                _station.jFDLAFProductRecipe.SaveParamsToCfg();
                JFHubCenter.Instance.RecipeManager.Save();
                flag = true;
            }
            catch(Exception ex)
            {
                HTUi.PopError(ex.ToString());
                return;
            }
        }

        /// <summary>
        /// 删除一个FOV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDeleteFov_Click(object sender, EventArgs e)
        {
            try
            {
                int index = 0;
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (icImage == null)
                {
                    HTUi.PopError("未成功加载IC图像");
                    return;
                }

                if (dgvFovInf.SelectedRows.Count >= 1)
                {
                    index = dgvFovInf.SelectedRows[0].Index;

                    HOperatorSet.TupleRemove(row, index, out row);
                    HOperatorSet.TupleRemove(col, index, out col);

                    _station.ICFovRow.RemoveAt(index);
                    _station.ICFovCol.RemoveAt(index);

                    if (_station.jFDLAFProductRecipe.visionCfgParams.ContainsKey(string.Format("{0}", index)))
                        _station.jFDLAFProductRecipe.visionCfgParams.Remove(string.Format("{0}", index));

                    //删除控件中选中行
                    DeleteDataGridViewFunction(dgvFovInf, index);

                    //重新排序
                    int endIndex = _station.jFDLAFProductRecipe.visionCfgParams.Keys.Count;
                    int fovCount = 0;
                    fovCount = _station.jFDLAFProductRecipe.visionCfgParams.Keys.Count;

                    JFDLAFProductRecipe cloneDiv = _station.jFDLAFProductRecipe.Clone();
                    for (int i = 0; i < fovCount; i++)
                    {
                        if (_station.jFDLAFProductRecipe.visionCfgParams.ContainsKey(i.ToString()))
                            continue;
                        JFXmlDictionary<string, JFXmlDictionary<string, string>> fovVisionCfg = _station.jFDLAFProductRecipe.visionCfgParams[(i + 1).ToString()];
                        _station.jFDLAFProductRecipe.visionCfgParams.Remove((i + 1).ToString());
                        _station.jFDLAFProductRecipe.visionCfgParams.Add(i.ToString(), fovVisionCfg);

                        dgvFovInf.Rows[i].Cells[0].Value = i.ToString();
                    }

                    //更新allShowRegion
                    ChangeShowSelectRegion(-1);
                    _station.operation.ShowImageEx(htWindow, icImage, allShowRegion);

                    _station.SaveStationParams();
                    _station.SaveCfg();

                    _station.jFDLAFProductRecipe.SaveParamsToCfg();
                    JFHubCenter.Instance.RecipeManager.Save();

                }
                else
                {
                    HTUi.PopError("请选中需要删除的Fov所在行");
                    return;
                }
            }
            catch(Exception ex)
            {
                HTUi.PopError("请删除所有视野。\r\n"+ ex.ToString());
                return;
            }
        }

        /// <summary>
        /// 编辑任务名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btEditTaskName_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (dgvFovInf.Rows.Count != _station.jFDLAFProductRecipe.visionCfgParams.Keys.Count)
                {
                    HTUi.PopError(string.Format("请删除所有视野并重新框选视野"));
                    return;
                }

                if(!UpdateAllFovAndTaskName())
                {
                    return;
                }

                Form_VisionCfgManager visionCfg = new Form_VisionCfgManager(_station.jFDLAFProductRecipe.visionCfgParams);
                if (visionCfg.ShowDialog() == DialogResult.OK)
                {
                    _station.jFDLAFProductRecipe.visionCfgParams = visionCfg._visionCfgParams;
                    _station.jFDLAFProductRecipe.SaveParamsToCfg();
                    JFHubCenter.Instance.RecipeManager.Save();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool UpdateAllFovAndTaskName()
        {
            //更新FovName或者TaskCount
            List<string> fovnameList = new List<string>();
            int fovcount = dgvFovInf.Rows.Count;
            for (int i = 0; i < fovcount; i++)
            {
                //判断FovName/TaskName是否为空,以及Taskcount是否为大于0的正整数
                if (!IsFovNameAndTaskNameValid())
                    return false;

                JFXmlDictionary<string, JFXmlDictionary<string, string>> fovVisionCfg = _station.jFDLAFProductRecipe.visionCfgParams[i.ToString()];
                if (fovVisionCfg.Keys.Count != 1)
                {
                    HTUi.PopError(string.Format("请删除所有视野并重新框选视野"));
                    return false;
                }
                //名称变更
                foreach (string fovname in fovVisionCfg.Keys)
                {
                    if (fovnameList.Contains(fovname))
                    {
                        HTUi.PopError(string.Format("视野名称不可重复"));
                        return false;
                    }

                    if (fovname != dgvFovInf.Rows[i].Cells[2].Value.ToString())
                    {
                        JFXmlDictionary<string, string> keyValuePairs = fovVisionCfg[fovname];
                        fovVisionCfg.Remove(fovname);
                        fovVisionCfg.Add(dgvFovInf.Rows[i].Cells[2].Value.ToString(), keyValuePairs);

                        fovnameList.Add(dgvFovInf.Rows[i].Cells[2].Value.ToString());
                    }
                    else
                        fovnameList.Add(fovname);

                    //TaskCount变更
                    if (fovVisionCfg[dgvFovInf.Rows[i].Cells[2].Value.ToString()].Values.Count.ToString() != dgvFovInf.Rows[i].Cells[3].Value.ToString())
                    {
                        JFXmlDictionary<string, string> keyValuePairs = fovVisionCfg[dgvFovInf.Rows[i].Cells[2].Value.ToString()];
                        keyValuePairs.Clear();
                        for (int m = 0; m < Convert.ToInt32(dgvFovInf.Rows[i].Cells[3].Value.ToString()); m++)
                        {
                            keyValuePairs.Add(string.Format("Task{0}", m), "");
                        }
                        fovVisionCfg[dgvFovInf.Rows[i].Cells[2].Value.ToString()] = keyValuePairs;
                    }
                    break;
                }
            }
            return true;
        }

        /// <summary>
        /// 更新AllShowRegion
        /// </summary>
        private void ShowAllRegion()
        {
            int fovcount = _station.ICFovCol.Count;
            HOperatorSet.GenEmptyObj(out selectRegion);
            HOperatorSet.GenEmptyObj(out allShowRegion);


            for (int i = 0; i < fovcount; i++)
            {
                HObject con = new HObject();
                HOperatorSet.GenEmptyObj(out con);
                GenFovRegion(row.TupleSelect(i).D, col.TupleSelect(i).D, out con);
                HOperatorSet.ConcatObj(allShowRegion, con, out allShowRegion);
                con.Dispose();

            }
        }

        /// <summary>
        /// 显示所有选中的Region
        /// </summary>
        private void ShowAllSelectRegion()
        {
            int fovcount = dgvFovInf.Rows.Count;
            HOperatorSet.GenEmptyObj(out selectRegion);
            HOperatorSet.GenEmptyObj(out allShowRegion);

            for (int i = 0;i < fovcount;i++)
            {
                DataGridViewCheckBoxCell cellEnable = dgvFovInf.Rows[i].Cells[1] as DataGridViewCheckBoxCell;
                if (((bool)(cellEnable.EditingCellFormattedValue)))
                {
                    HObject con = new HObject();
                    HOperatorSet.GenEmptyObj(out con);
                    GenFovRegion(row.TupleSelect(i).D, col.TupleSelect(i).D, out con);
                    HOperatorSet.ConcatObj(allShowRegion, con, out allShowRegion);
                    con.Dispose();
                }
            }
        }

        /// <summary>
        /// Change选中的Region
        /// </summary>
        private void ChangeShowSelectRegion(int index)
        {
            int fovcount = dgvFovInf.Rows.Count;
            HOperatorSet.GenEmptyObj(out selectRegion);
            HOperatorSet.GenEmptyObj(out allShowRegion);


            for (int i = 0; i < fovcount; i++)
            {
                bool checkStatus = false;
                if (index == i)
                {
                    DataGridViewCheckBoxCell cellEnable = dgvFovInf.Rows[i].Cells[1] as DataGridViewCheckBoxCell;
                    checkStatus = ((bool)(cellEnable.EditingCellFormattedValue));
                }
                else
                {
                    checkStatus = ((bool)(dgvFovInf.Rows[i].Cells[1].Value));
                }
                if (checkStatus)
                {
                    HObject con = new HObject();
                    HOperatorSet.GenEmptyObj(out con);
                    GenFovRegion(row.TupleSelect(i).D, col.TupleSelect(i).D, out con);
                    HOperatorSet.ConcatObj(allShowRegion, con, out allShowRegion);
                    con.Dispose();
                }
            }
        }


        /// <summary>
        /// 生成Drag Region
        /// </summary>
        /// <returns></returns>
        private bool GenDragRegion(out HTuple row,out HTuple col)
        {
            //生成Fov Region
            HObject ho_show_region = new HObject(), ho_dragRect = null, ho_update_show_contour = null;
            HTuple area = new HTuple();
            HObject cont = new HObject();
            HOperatorSet.GenEmptyObj(out cont);

            HTuple _row1=new HTuple(), _col1 = new HTuple();
            if (fovWidth >= _station.jFDLAFProductRecipe.DieWidth && fovHeight >= _station.jFDLAFProductRecipe.DieHeight)//大视野vs小芯片
            {
                HOperatorSet.GetImageSize(icImage, out _col1, out _row1);

                HOperatorSet.GenRectangle2ContourXld(out cont, Math.Abs(_row1) / 2, Math.Abs(_col1) / 2, 0,
                    -10+Math.Abs(_col1) / 2, -10 + Math.Abs(_row1) / 2);
            }
            else
                HOperatorSet.GenRectangle2ContourXld(out cont, Math.Abs(_station.height.D * (double)_station.jFDLAFProductRecipe.ScaleFactor) / 2, 
                                                    Math.Abs(_station.width.D * (double)_station.jFDLAFProductRecipe.ScaleFactor)/ 2, 0, Math.Abs(_station.width.D * (double)_station.jFDLAFProductRecipe.ScaleFactor)/2 , Math.Abs(_station.height.D * (double)_station.jFDLAFProductRecipe.ScaleFactor) / 2);
           
            HOperatorSet.GenRegionContourXld(cont, out ho_show_region,
                "filled");
            HOperatorSet.DragRegion1(ho_show_region, out ho_dragRect, htWindow.HTWindow.HalconWindow);
            HOperatorSet.GenContourRegionXld(ho_dragRect, out ho_update_show_contour,
                "border");

            cont.Dispose();
            HOperatorSet.AreaCenter(ho_dragRect, out area, out row, out col);

            HTuple width = new HTuple();
            HTuple height = new HTuple();
            HOperatorSet.GetImageSize(icImage, out width, out height);

            //判断加的视野边界是否有超出图片region范围
            if (fovWidth >= _station.jFDLAFProductRecipe.DieWidth && fovHeight >= _station.jFDLAFProductRecipe.DieHeight)//大视野vs小芯片
            {
                if ((row.D - Math.Abs(_row1.D) / 2)>=-10 && (col.D - Math.Abs(_col1) / 2) >= -10 
                    && (row.D + Math.Abs(_row1) / 2) <=(height.D+10) && (col.D + Math.Abs(_col1) / 2) <= (width.D+10))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if ((row.D - Math.Abs(_station.height.D * (double)_station.jFDLAFProductRecipe.ScaleFactor) / 2) >= 0 && (col.D - Math.Abs(_station.width.D * (double)_station.jFDLAFProductRecipe.ScaleFactor) / 2) >= 0
                     && (row.D + Math.Abs(_station.height.D * (double)_station.jFDLAFProductRecipe.ScaleFactor) / 2) <= height.D && (col.D + Math.Abs(_station.width.D * (double)_station.jFDLAFProductRecipe.ScaleFactor) / 2) <= width.D)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 生成Fov Region
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="genObj"></param>
        /// <returns></returns>
        private bool GenFovRegion(double row, double col,out HObject genObj)
        {
            HObject ho_show_region = new HObject();
            HObject cont = new HObject();
            HOperatorSet.GenEmptyObj(out cont);

            if (fovWidth >= _station.jFDLAFProductRecipe.DieWidth && fovHeight >= _station.jFDLAFProductRecipe.DieHeight)//大视野vs小芯片
            {
                HTuple _row1=new HTuple(), _col1=new HTuple();
                HOperatorSet.GetImageSize(icImage, out _col1, out _row1);
                HOperatorSet.GenRectangle2ContourXld(out cont, row, col, 0,
                                                    -10+Math.Abs(_col1) / 2, -10 + Math.Abs(_row1) / 2);
            }
            else
                HOperatorSet.GenRectangle2ContourXld(out cont, row, col, 0, Math.Abs(_station.width.D * (double)_station.jFDLAFProductRecipe.ScaleFactor/2),Math.Abs(_station.height.D * (double)_station.jFDLAFProductRecipe.ScaleFactor) / 2);
            
            HOperatorSet.GenRegionContourXld(cont, out ho_show_region,
                "filled");
            HOperatorSet.GenContourRegionXld(ho_show_region, out genObj,
                "border");
            cont.Dispose();
            return true;
        }

        /// <summary>
        /// 操作一个Cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvFovInf_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    if (e.RowIndex >= 0)
                    {
                        if (icImage == null)
                        {
                            HTUi.PopError("未成功加载IC图像");
                            return;
                        }

                        //CheckBox状态发生改变
                        ChangeShowSelectRegion(e.RowIndex);
                        _station.operation.ShowImageEx(htWindow, icImage, allShowRegion);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 保存视野参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveFovParams_Click(object sender, EventArgs e)
        {
            int FovCount = dgvFovInf.Rows.Count;
            if (FovCount >= 1)
            {
                for (int i = 0; i < FovCount; i++)
                {
                    //if (_station.visionCfgParams[])
                }
            }
        }

        /// <summary>
        /// 删除所有Fov
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDeleteAllFov_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }
                HOperatorSet.GenEmptyObj(out allShowRegion);
                HOperatorSet.GenEmptyObj(out selectRegion);

                _station.jFDLAFProductRecipe.visionCfgParams = new JFXmlDictionary<string, JFXmlDictionary<string, JFXmlDictionary<string, string>>>();
                _station.ICFovCol = new List<double>();
                _station.ICFovRow = new List<double>();
                row = new HTuple();
                col = new HTuple();
                dgvFovInf.Rows.Clear();

                ShowAllSelectRegion();
                _station.operation.ShowImageEx(htWindow, icImage, null);

                _station.SaveStationParams();
                _station.SaveCfg();

                _station.jFDLAFProductRecipe.SaveParamsToCfg();
                JFHubCenter.Instance.RecipeManager.Save();
            }
            catch(Exception ex)
            {
                HTUi.PopError(ex.ToString()); 
            }
        }

        bool bChanged = true;
        /// <summary>
        /// 选择光源参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSelectLight_Click(object sender, EventArgs e)
        {
            string errInf = "";
            if (!bChanged) return;
            bChanged = false;
            //切换光源配置以及等待切换完成
            _station.AdjustSingleVisionCfg(_station.CamereDev[0], cbLightChoose.SelectedItem.ToString(), out errInf);
            _station.WaitSingleVisionCfgAdjustDone(cbLightChoose.SelectedItem.ToString(), out errInf,10000);
            bChanged = true;
            if(errInf!="" && errInf!= "Success")
            {
                HTUi.TipHint("光源参数切换失败！");
                HTLog.Info("光源参数切换失败！");
                return;
            }
            HTUi.TipHint("光源参数切换成功！");
            HTLog.Info("光源参数切换成功！");
        }

        private void FrmAutoMapping_Load(object sender, EventArgs e)
        {
            EditColumnHeadText();
            SetupUI();
        }

        private void btnSnapMapEnd_Click(object sender, EventArgs e)
        {
            string errMsg = "";
            try
            {
                if (_station.jFDLAFProductRecipe == null)
                {
                    HTUi.PopError("请先加载产品配方" + (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID"));
                    return;
                }

                if (_station.operation.MultiAxisMove(_station.AxisXYZ, new double[] { _station.genMapEndX, _station.genMapEndY, _station.ZFocus }, true, out errMsg) != 0)
                {
                    HTUi.PopError(String.Format("移动的拍照位{0}_{1}_{2}失败!详细信息:{3}",
                                                            _station.genMapEndX,
                                                            _station.genMapEndY,
                                                            _station.ZFocus,
                                                            errMsg));//报警 停止动作
                    return;
                }

                if (_station.WaitMotionDone(_station.AxisXYZ[0], 10000) != JFWorkCmdResult.Success)
                {
                    HTUi.PopError("等待轴" + _station.AxisXYZ[0] + "运动完成超时");
                    return;
                }
                if (_station.WaitMotionDone(_station.AxisXYZ[1], 10000) != JFWorkCmdResult.Success)
                {
                    HTUi.PopError("等待轴" + _station.AxisXYZ[1] + "运动完成超时");
                    return;
                }
                if (_station.WaitMotionDone(_station.AxisXYZ[2], 10000) != JFWorkCmdResult.Success)
                {
                    HTUi.PopError("等待轴" + _station.AxisXYZ[2] + "运动完成超时");
                    return;
                }

                if (_station._RunMode == 1) return;

                //if (_station.operation.SWPosTrig(new string[] { "X" }, out errMsg) != 0)
                //{
                //    HTUi.PopError(errMsg);
                //    return;
                //}
                //4. 取图
                HOperatorSet.GenEmptyObj(out Image);
                if (_station.operation.CaputreOneImage(_station.CamereDev[_station.SelectedIndex], "Halcon", out Image, out errMsg) != 0)
                {
                    HTUi.PopError(errMsg);
                    btnSnap.Enabled = true;
                    return;
                }
                _station.operation.ShowImage(htWindow, Image, null);
            }
            catch (Exception ex)
            {
                HTUi.PopError("采集Map终点失败：" + ex.ToString());
                return;
            }
        }

    }
}