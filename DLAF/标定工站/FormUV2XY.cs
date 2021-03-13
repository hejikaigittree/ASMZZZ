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
using System.Threading;
using HT_Lib;
using ToolKits.TemplateEdit;
using System.IO;
using ToolKits.FunctionModule;
using HTHalControl;
using JFMethodCommonLib;
using JFInterfaceDef;
using JFHub;

namespace DLAF
{
    /// <summary>
    /// 多颗DIE检测标定、光源亮度标定
    /// </summary>
    public partial class FormUV2XY : Form
    {
        public HObject Image = new HObject();
        MainTemplateForm mTmpFrm = null;
        Form modelForm = null;
        Model aoiModels;
        HTuple aoiHoms;
        HTuple Pos_Uv2Xy = null;
        CalibStation _station = null;
        ToolKits.FunctionModule.Vision tool_vision;



        public FormUV2XY()
        {
            InitializeComponent();
            tool_vision = new ToolKits.FunctionModule.Vision();
            aoiHoms = new HTuple();
        }

        public void SetStation(CalibStation station)
        {
            _station = station;
        }

        public void SetupUI()
        {
            try
            {
                if (Created)
                {
                    _station.InitStationParams();
                    dgvUVXY2D.Rows.Clear();
                    InitUV2XYParam();
                    dgvUVXY2D.Rows.Add();
                    if (_station.Num_Camera > _station.SelectedIndex)
                    {
                        if (_station.List_UV2XYResult[_station.SelectedIndex].Type != HTupleType.INTEGER)
                        {
                            SetDGVValue(ref dgvUVXY2D, _station.List_UV2XYResult[_station.SelectedIndex]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HTUi.PopError("" + ex.ToString());
            }
        }

        private void InitUV2XYParam()
        {
            try
            {
                _station.List_UV2XYResult = new List<HTuple>();
                tool_vision = new ToolKits.FunctionModule.Vision();
                for (int i = 0; i < _station.Num_Camera; i++)
                {
                    HTuple Item_UV2XYResult = null;
                    //if (!obj_camera[i].isEnable)
                    //{
                    //    Item_UV2XYResult = new HTuple(-1);
                    //    _station.List_UV2XYResult.Add(Item_UV2XYResult);
                    //    continue;
                    //}
                    if (File.Exists(_station.SystemUV2XYDir + "\\Camera_" + i + "\\" + "UV2XY" + ".dat"))
                    {
                        tool_vision.read_hom2d(_station.SystemUV2XYDir + "\\Camera_" + i + "\\" + "UV2XY" + ".dat", out Item_UV2XYResult);
                        _station.List_UV2XYResult.Add(Item_UV2XYResult);
                    }
                    else
                    {
                        Item_UV2XYResult = new HTuple(-1);
                        _station.List_UV2XYResult.Add(Item_UV2XYResult);
                    }
                }
            }
            catch(Exception ex)
            {
                HTUi.PopError("无法读取UV-XY结果文件:"+ex.ToString());
            }
        }

        /// <summary>
        /// UV2XY标定时加载光源、点位和视觉参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadPara_Click(object sender, EventArgs e)
        {
            try
            {
                //App.obj_light.FlashMultiLight(LightUseFor.ScanPoint1st);
                HTUi.TipHint("参数加载成功");
            }
            catch (Exception exp)
            {
                HTUi.PopError(exp.ToString());
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSavePara_Click(object sender, EventArgs e)
        {
            try
            {
                _station.SaveStationParams();
                _station.SaveCfg();
            }
            catch (Exception ex)
            {
                HTUi.PopError("保存数据失败！\n" + ex.ToString());
            }
        }
        private delegate void ShowImageDelegate(HTWindowControl htWindow, HObject image, HObject region);
        public void ShowImage(HTWindowControl htWindow, HObject image, HObject region)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowImageDelegate(ShowImage), new object[] { htWindow, image, region });
            }
            else
            {
                htWindow.ColorName = "green";
                htWindow.SetInteractive(false);
                if (htWindow.Image == null || !htWindow.Image.IsInitialized())
                    htWindow.RefreshWindow(image, region, "fit");//适应窗口
                else
                    htWindow.RefreshWindow(image, region, "");//可以不显示区域
                htWindow.SetInteractive(true);
            }
        }

        private void btnCreateCalibModel2D_Click(object sender, EventArgs e)
        {
            try
            {
                if (modelForm == null || modelForm.IsDisposed)
                {
                    modelForm = new Form();
                    if (this.mTmpFrm != null && !this.mTmpFrm.IsDisposed) this.mTmpFrm.Dispose();
                    this.mTmpFrm = new MainTemplateForm(ToolKits.TemplateEdit.MainTemplateForm.TemplateScence.Match,
                                     this.htWindowCalibration, new MainTemplateForm.TemplateParam());
                    modelForm.Controls.Clear();
                    this.modelForm.Controls.Add(this.mTmpFrm);
                    this.mTmpFrm.Dock = DockStyle.Fill;
                    modelForm.Size = new Size(300, 450);
                    modelForm.TopMost = true;
                    modelForm.Show();
                    int SH = Screen.PrimaryScreen.Bounds.Height;
                    int SW = Screen.PrimaryScreen.Bounds.Width;
                    modelForm.Location = new Point(SW - modelForm.Size.Width, SH / 8);

                    Task.Run(() =>
                    {
                        while (!mTmpFrm.WorkOver)
                        {
                            Thread.Sleep(200);
                        }
                        HTUi.TipHint("创建标定模板完成!");
                        HTLog.Info("创建标定模板完成!");
                        modelForm.Close();
                    });
                }
                else
                {
                    modelForm.Activate();
                }
            }
            catch (Exception ex)
            {
                HTUi.PopError("创建标定模板失败：" + ex.Message);
                btnSnap.Enabled = true;
            }
        }

        private async void btnSaveCalibModel2D_Click(object sender, EventArgs e)
        {
            if (!this.mTmpFrm.tmpResult.createTmpOK)
            {
                HTUi.PopError("创建标定模板失败！");
                return;
            }
            Form_Wait.ShowForm();
            bool result = await Task.Run(() =>
            {
                try
                {
                    this.aoiModels = new Model();
                    this.aoiModels.Dispose();
                    this.aoiModels.showContour = this.mTmpFrm.tmpResult.showContour.CopyObj(1, -1);
                    this.aoiModels.defRows = this.mTmpFrm.tmpResult.defRows;
                    this.aoiModels.defCols = this.mTmpFrm.tmpResult.defCols;
                    this.aoiModels.modelType = this.mTmpFrm.tmpResult.modelType;
                    this.aoiModels.modelID = Vision.CopyModel(this.mTmpFrm.tmpResult.modelID, this.mTmpFrm.tmpResult.modelType);
                    this.aoiModels.scoreThresh = this.mTmpFrm.mte_TmpPrmValues.Score;
                    this.aoiModels.angleStart = this.mTmpFrm.mte_TmpPrmValues.AngleStart;
                    this.aoiModels.angleExtent = this.mTmpFrm.mte_TmpPrmValues.AngleExtent;
                        //保存至硬盘
                    string folderPath = _station.SystemUV2XYDir + "\\Camera_" + _station.SelectedIndex;
                    _station.CalibrUV2XYModelPath = folderPath;
                    if (!this.aoiModels.WriteModel(folderPath))
                    {
                        HTLog.Error("保存标定模板失败！");
                        HTUi.PopError("保存标定模板失败！");
                        return false;
                    }

                    string errMsg = "";
                    Pos_Uv2Xy = new HTuple();
                    double xUV2xy = 0, yUV2xy = 0, zUV2xy = 0;

                    if (!_station.GetAxisPosition(_station.AxisXYZ[0],out xUV2xy,out errMsg))
                    {
                        HTLog.Error(errMsg+"：保存获取X轴反馈坐标失败！");
                        HTUi.PopError(errMsg + "：保存获取X轴反馈坐标失败！");
                        return false;
                    }

                    if (!_station.GetAxisPosition(_station.AxisXYZ[1], out yUV2xy, out errMsg))
                    {
                        HTLog.Error(errMsg + "：保存获取Y轴反馈坐标失败！");
                        HTUi.PopError(errMsg + "：保存获取Y轴反馈坐标失败！");
                        return false;
                    }

                    if (!_station.GetAxisPosition(_station.AxisXYZ[2], out zUV2xy, out errMsg))
                    {
                        HTLog.Error(errMsg + "：保存获取Z轴反馈坐标失败！");
                        HTUi.PopError(errMsg + "：保存获取Z轴反馈坐标失败！");
                        return false;
                    }

                    _station.operation.YUv2xy = yUV2xy;
                    _station.operation.XUv2xy = xUV2xy;
                    _station.operation.ZUv2xy = zUV2xy;

                    Pos_Uv2Xy.Append(_station.operation.XUv2xy);
                    Pos_Uv2Xy.Append(_station.operation.YUv2xy);
                    Pos_Uv2Xy.Append(_station.operation.ZUv2xy);
                    HOperatorSet.WriteTuple(Pos_Uv2Xy, folderPath + "\\Pos_Uv2Xy.tup");
                    HTUi.TipHint("保存标定模板成功！");
                    HTLog.Info("保存标定模板成功！");
                    return true;
                }
                catch (Exception ex)
                {
                    HTLog.Error(ex.ToString());
                    HTUi.PopError(ex.ToString());

                    return false;
                }
            });
            Form_Wait.CloseForm();
            if (!result)
            {
                HTLog.Error("保存标定模板失败！");
                HTUi.PopError("保存标定模板失败！");
                return;
            }
            HTUi.TipHint("保存标定模板成功！");
            HTLog.Info("保存标定模板成功！");
        }

        private async void btnUVXYCalib2D_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.aoiModels == null)
                {
                    this.aoiModels = new Model();
                    string folderPath = _station.SystemUV2XYDir + "\\Camera_" + _station.SelectedIndex;
                    if (!this.aoiModels.ReadModel(folderPath))
                    {
                        HTUi.PopError("未创建2D标定模板！");
                        return;
                    }
                    HOperatorSet.ReadTuple(folderPath + "\\Pos_Uv2Xy.tup", out Pos_Uv2Xy);
                }
                //设定走位范围
                CalibSearchRange csRangeFrm = new CalibSearchRange();
                if (csRangeFrm.ShowDialog() != DialogResult.OK) return;
                double xyRange = csRangeFrm.Range;
                //条件：标定板中心在2D相机视野中心位置时的chuck x y的粗略位置已知，即                              
                //料片视野为左下芯片为中心作为初始点位
                _station.operation.XUv2xy = Pos_Uv2Xy[0];//App.obj_Chuck.ref_x + ((App.obj_Pdt.BlockNumber - 1) * App.obj_Pdt.BlockSpace + (App.obj_Pdt.ColumnNumber - 1) * App.obj_Pdt.ColomnSpace)/2 + App.obj_Pdt.RightEdge;
                _station.operation.YUv2xy = Pos_Uv2Xy[1];//App.obj_Chuck.ref_y + App.obj_Pdt.TopEdge + (App.obj_Pdt.RowNumber - 1) * App.obj_Pdt.RowSpace/2;
                _station.operation.ZUv2xy = Pos_Uv2Xy[2];//App.obj_Pdt.ZFocus;
                HTuple uvHxy = new HTuple();
                bool status = await Task.Run(() =>
                {
                    return _station.operation.calibUV2XY
                                  (
                                      _station.SelectedIndex,
                                      ref uvHxy,
                                      this.aoiModels,
                                      xyRange, 20
                                  );
                });
                if (!status)
                {
                    HTUi.PopError("2D相机uvHxy标定失败！");
                    return;
                }
                for (int i = 0; i < dgvUVXY2D.RowCount; i++)
                {
                    dgvUVXY2D.Rows[i].Cells[0].Value = uvHxy[i * 3].D;
                    dgvUVXY2D.Rows[i].Cells[1].Value = uvHxy[i * 3 + 1].D;
                    dgvUVXY2D.Rows[i].Cells[2].Value = uvHxy[i * 3 + 2].D;
                }
                HTUi.TipHint("2D相机uvHxy标定成功！");
                HTLog.Info("2D相机uvHxy标定成功！");
            }
            catch (Exception ex)
            {
                HTUi.PopError("2D相机uvHxy标定失败：" + ex.Message);
                btnSnap.Enabled = true;
            }

        }
        /// <summary>
        /// 设置DGV某个行列范围内的数据，赋值给tuple
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="rowInd">起始行与终止行数组</param>
        /// <param name="colInd">起始列与终止列数组</param>
        /// <param name="tuple"></param>
        private void SetDGVValue(ref DataGridView dgv, HTuple tuple)
        {
            for (int i = 0; i < dgv.RowCount; i++)
            {
                dgv.Rows[i].Cells[0].Value = tuple[i * 3].D;
                dgv.Rows[i].Cells[1].Value = tuple[i * 3 + 1].D;
                dgv.Rows[i].Cells[2].Value = tuple[i * 3 + 2].D;
            }
        }
        /// <summary>
        /// 获取DGV某个行列范围内的数据，赋值给tuple
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="rowInd">起始行与终止行数组</param>
        /// <param name="colInd">起始列与终止列数组</param>
        /// <param name="tuple"></param>
        private void GetDGVValue(DataGridView dgv, int[] rowInd, int[] colInd, ref HTuple tuple)
        {
            tuple = new HTuple();
            for (int i = rowInd[0]; i < rowInd[1] - rowInd[0] + 1; i++)
            {
                for (int j = colInd[0]; j < colInd[1] - colInd[0] + 1; j++)
                {
                    tuple.Append(Convert.ToDouble(dgv.Rows[i].Cells[j].Value));
                }
            }
        }

        private void btnSaveUVXY2D_Click(object sender, EventArgs e)
        {
            this.btnSaveUVXY2D.Focus();
            HTuple uvHxy = null;
            try
            {
                GetDGVValue(dgvUVXY2D, new int[] { 0, 1 }, new int[] { 0, 2 }, ref uvHxy);
            }
            catch (Exception)
            {
                MessageBox.Show("获取2D相机UV-XY矩阵数据失败！");
                return;
            }
            //保存
            string filePath = _station.SystemUV2XYDir + "\\Camera_" + _station.SelectedIndex+ "\\" + "UV2XY" + ".dat";
            try
            {
                string dir = Directory.GetParent(filePath).FullName;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                tool_vision.save_hom2d(filePath, uvHxy);
                _station.List_UV2XYResult[_station.SelectedIndex] = uvHxy;
            }
            catch (Exception ex)
            {
                HTUi.PopError("2D相机UV-XY矩阵保存失败！\n"+ex.ToString());
                return;
            }
            HTUi.TipHint("2D相机UV-XY矩阵保存成功！");
            HTLog.Info("2D相机UV-XY矩阵保存成功！");
        }

        private void btnSnap_Click(object sender, EventArgs e)
        {
            try
            {
                if (_station._RunMode == 1) return;
                string errStr = "";
                string[] TrigChnName = new string[1];
                TrigChnName[0] = "TrigX";

                //if(_station.operation.SWPosTrig(TrigChnName,out errStr) !=0)
                //{
                //    HTUi.PopError(errStr);
                //    btnSnap.Enabled = true;
                //    return;
                //}
                //4. 取图
                HOperatorSet.GenEmptyObj(out Image);

                if(_station.operation.CaputreOneImage(_station.CamereDev[_station.SelectedIndex], "Halcon", out Image, out errStr)!=0)
                {
                    HTUi.PopError(errStr);
                    btnSnap.Enabled = true;
                    return;
                }
               
                ShowImage(htWindowCalibration, Image, null);
                if (errStr != "")
                {
                    HTUi.PopError(errStr);
                    btnSnap.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                HTUi.PopError("单张采集失败："+ex.Message);
                btnSnap.Enabled = true;
            }
        }

        private void btnTool_Click(object sender, EventArgs e)
        {
            if (HTM.LoadUI() < 0)
            {
                HTUi.PopError("打开轴调试助手界面失败");
            }
        }

        private void btnCmrAxisTool_Click(object sender, EventArgs e)
        {
            FrmCamAxisMotion frm = FrmCamAxisMotion.Instance;
            if (frm == null || frm.IsDisposed)
            {
                frm = new FrmCamAxisMotion();
                frm.SetStation(_station as IJFStation);
                frm.TopMost = true;
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
    }
}
