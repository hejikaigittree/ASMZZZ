using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Xml.Serialization;
using ToolKits.FunctionModule;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
namespace DLAF
{
    /// <summary>
    /// 模板标准类，用于同时检测多颗Dies（dieModel/markModel）
    /// </summary>
    public class Model
    {
        /// <summary>
        /// 模板句柄
        /// </summary>
        public HTuple modelID;
        /// <summary>
        /// 模板类型
        /// </summary>
        public HTuple modelType;
        /// <summary>
        /// 映射点行坐标
        /// </summary>
        public HTuple defRows;
        /// <summary>
        /// 映射点列坐标
        /// </summary>
        public HTuple defCols;
        /// <summary>
        /// 匹配分数阈值
        /// </summary>
        public HTuple scoreThresh;
        /// <summary>
        /// 匹配起始角度
        /// </summary>
        public HTuple angleStart;
        /// <summary>
        /// 匹配角度范围
        /// </summary>
        public HTuple angleExtent;
        /// <summary>
        /// 用于显示的轮廓
        /// </summary>
        public HObject showContour;
        /// <summary>
        /// 用于记录做模板时设置的点位信息
        /// </summary>
        public HObject showImage;
        /// <summary>
        /// 匹配区域
        /// </summary>
        public HObject matchRegion;

        /// <summary>
        /// 临时使用 矫正点采用测量直线的方法 新增参数
        /// </summary>
        public HTuple hLineRows = new HTuple();
        public HTuple hLineCols = new HTuple();
        public HTuple vLineRows = new HTuple();
        public HTuple vLineCols = new HTuple();
        public HTuple refPointsRow = new HTuple();
        public HTuple refPointsCol = new HTuple();

        public Model()
        {
            modelID = new HTuple();
            modelType = new HTuple();
            defRows = new HTuple();
            defCols = new HTuple();
            scoreThresh = new HTuple();
            angleStart = new HTuple();
            angleExtent = new HTuple();
            showContour = new HObject();
            HOperatorSet.GenEmptyObj(out showContour);
            showImage = new HObject();
            HOperatorSet.GenEmptyObj(out showImage);
            matchRegion = new HObject();
            HOperatorSet.GenEmptyObj(out matchRegion);
        }
        /// <summary>
        /// 读取模板类的所有信息
        /// </summary>
        /// <param name="modelDirPath"></param>
        /// <returns></returns>
        public bool ReadModel(string modelDirPath)
        {
            try
            {
                HTuple iFlag = null;
                showContour.Dispose();
                ToolKits.FunctionModule.Vision.read_model(out showContour, modelDirPath, out modelType, out modelID, out defRows, out defCols, out iFlag);
                if (iFlag.I != 0)
                {
                    Dispose();
                    return false;
                }
                //???????
                //HOperatorSet.ReadTuple(modelDirPath + "\\angleStart.tup", out angleStart);
                //HOperatorSet.ReadTuple(modelDirPath + "\\angleExtent.tup", out angleExtent);
                //HOperatorSet.ReadTuple(modelDirPath + "\\scoreThresh.tup", out scoreThresh);
                showImage.Dispose();
                if (File.Exists(modelDirPath + "\\showImage.tiff"))
                {
                    HOperatorSet.ReadImage(out showImage, modelDirPath + "\\showImage.tiff");
                }
                matchRegion.Dispose();
                if (File.Exists(modelDirPath + "\\matchRegion.reg"))
                {
                    HOperatorSet.ReadRegion(out matchRegion, modelDirPath + "\\matchRegion.reg");
                }
                //加载其它modelID0-n
                string[] files = Directory.GetFiles(modelDirPath, "modelID*.dat");
                for (int i = 0; i < files.Length; i++)
                {
                    HTuple _modelID = new HTuple();
                    if (modelType.I == 0)
                    {
                        HOperatorSet.ReadNccModel(files[i], out _modelID);
                    }
                    else
                    {
                        HOperatorSet.ReadShapeModel(files[i], out _modelID);
                    }
                    HTuple _defRow, _defCol;
                    //HOperatorSet.ReadTuple(modelDirPath + "\\defRow*" + i.ToString() + ".tup", out _defRow);
                    //HOperatorSet.ReadTuple(modelDirPath + "\\defCol*" + i.ToString() + ".tup", out _defCol);
                    HOperatorSet.ReadTuple(modelDirPath + "\\defRow"  + ".tup", out _defRow);
                    HOperatorSet.ReadTuple(modelDirPath + "\\defCol"  + ".tup", out _defCol);
                    //modelID.Append(_modelID);
                    //defRows.Append(_defRow);
                    //defCols.Append(_defCol);
                }
                return true;
            }
            catch (Exception)
            {
                Dispose();
                return false;
            }
        }

        /// <summary>
        /// 2019.5.21 临时使用 测试新的矫正点 算法
        /// </summary>
        /// <param name="modelDirPath"></param>
        /// <returns></returns>
        public bool ReadModelCorrect(string modelDirPath)
        {
            try
            {
                HTuple iFlag = null;
                showContour.Dispose();
                if (modelID != null)
                    Vision.ClearModel(modelType, modelID);
                ToolKits.FunctionModule.Vision.read_model(out showContour, modelDirPath, out modelType, out modelID, out defRows, out defCols, out iFlag);
                if (iFlag.I != 0)
                {
                    Dispose();
                    return false;
                }
                //???????
                //HOperatorSet.ReadTuple(modelDirPath + "\\angleStart.tup", out angleStart);
                //HOperatorSet.ReadTuple(modelDirPath + "\\angleExtent.tup", out angleExtent);
                //HOperatorSet.ReadTuple(modelDirPath + "\\scoreThresh.tup", out scoreThresh);
                showImage.Dispose();
                if (File.Exists(modelDirPath + "\\showImage.tiff"))
                {
                    HOperatorSet.ReadImage(out showImage, modelDirPath + "\\showImage.tiff");
                }
                matchRegion.Dispose();
                if (File.Exists(modelDirPath + "\\matchRegion.reg"))
                {
                    HOperatorSet.ReadRegion(out matchRegion, modelDirPath + "\\matchRegion.reg");
                }

                HOperatorSet.ReadTuple(modelDirPath + "\\cornorRows.dat", out refPointsRow);
                HOperatorSet.ReadTuple(modelDirPath + "\\cornorCols.dat", out refPointsCol);
                HOperatorSet.ReadTuple(modelDirPath + "\\hLinesCol.dat", out hLineCols);
                HOperatorSet.ReadTuple(modelDirPath + "\\hLinesRow.dat", out hLineRows);
                HOperatorSet.ReadTuple(modelDirPath + "\\vLinesCol.dat", out vLineCols);
                HOperatorSet.ReadTuple(modelDirPath + "\\vLinesRow.dat", out vLineRows);



                //加载其它modelID0-n
                //string[] files = Directory.GetFiles(modelDirPath, "modelID*.dat");
                //for (int i = 0; i < files.Length; i++)
                //{
                //    HTuple _modelID = new HTuple();
                //    if (modelType.I == 0)
                //    {
                //        HOperatorSet.ReadNccModel(files[i], out _modelID);
                //    }
                //    else
                //    {
                //        HOperatorSet.ReadShapeModel(files[i], out _modelID);
                //    }
                //    HTuple _defRow, _defCol;
                //    //HOperatorSet.ReadTuple(modelDirPath + "\\defRow*" + i.ToString() + ".tup", out _defRow);
                //    //HOperatorSet.ReadTuple(modelDirPath + "\\defCol*" + i.ToString() + ".tup", out _defCol);
                //    HOperatorSet.ReadTuple(modelDirPath + "\\defRow" + ".tup", out _defRow);
                //    HOperatorSet.ReadTuple(modelDirPath + "\\defCol" + ".tup", out _defCol);
                //    //modelID.Append(_modelID);
                //    //defRows.Append(_defRow);
                //    //defCols.Append(_defCol);
                //}
                return true;
            }
            catch (Exception)
            {
                Dispose();
                return false;
            }
        }

        /// <summary>
        /// 保存模板类的所有信息
        /// </summary>
        /// <param name="modelDirPath"></param>
        /// <returns></returns>
        public bool WriteModel(string modelDirPath)
        {
            try
            {
                if (!Directory.Exists(modelDirPath)) Directory.CreateDirectory(modelDirPath);
                HTuple iFlag = null;
                ToolKits.FunctionModule.Vision.write_model(showContour, modelDirPath, modelType, modelID[0], defRows[0], defCols[0], out iFlag);
                if (iFlag.I != 0) return false;
                for (int i = 1; i < modelID.Length; i++)
                {
                    if (modelType.I == 0)
                        HOperatorSet.WriteNccModel(modelID[i], modelDirPath + "\\modelID_" + (i - 1).ToString() + ".dat");
                    else
                        HOperatorSet.WriteShapeModel(modelID[i], modelDirPath + "\\modelID_" + (i - 1).ToString() + ".dat");
                    HOperatorSet.WriteTuple(defRows[i], modelDirPath + "\\defRow_" + (i - 1).ToString() + ".tup");
                    HOperatorSet.WriteTuple(defCols[i], modelDirPath + "\\defCol_" + (i - 1).ToString() + ".tup");
                }
                HOperatorSet.WriteTuple(scoreThresh, modelDirPath + "\\scoreThresh.tup");
                HOperatorSet.WriteTuple(angleStart, modelDirPath + "\\angleStart.tup");
                HOperatorSet.WriteTuple(angleExtent, modelDirPath + "\\angleExtent.tup");
                if (showImage.IsInitialized())
                    HOperatorSet.WriteImage(showImage, "tiff", 0, modelDirPath + "\\showImage.tiff");
                if (matchRegion.IsInitialized())
                    HOperatorSet.WriteRegion(matchRegion, modelDirPath + "\\matchRegion.reg");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 对应之前的读模板ReadModelCorrect 函数
        /// </summary>
        /// <param name="modelDirPath"></param>
        /// <returns></returns>
        public bool WriteModelCorrect(string modelDirPath)
        {
            try
            {
                if (!Directory.Exists(modelDirPath)) Directory.CreateDirectory(modelDirPath);
                HTuple iFlag = null;
                ToolKits.FunctionModule.Vision.write_model(showContour, modelDirPath, modelType, modelID[0], defRows[0], defCols[0], out iFlag);
                if (iFlag.I != 0) return false;
                for (int i = 1; i < modelID.Length; i++)
                {
                    if (modelType.I == 0)
                        HOperatorSet.WriteNccModel(modelID[i], modelDirPath + "\\modelID_" + (i - 1).ToString() + ".dat");
                    else
                        HOperatorSet.WriteShapeModel(modelID[i], modelDirPath + "\\modelID_" + (i - 1).ToString() + ".dat");
                    HOperatorSet.WriteTuple(defRows[i], modelDirPath + "\\defRow_" + (i - 1).ToString() + ".tup");
                    HOperatorSet.WriteTuple(defCols[i], modelDirPath + "\\defCol_" + (i - 1).ToString() + ".tup");
                }
                HOperatorSet.WriteTuple(scoreThresh, modelDirPath + "\\scoreThresh.tup");
                HOperatorSet.WriteTuple(angleStart, modelDirPath + "\\angleStart.tup");
                HOperatorSet.WriteTuple(angleExtent, modelDirPath + "\\angleExtent.tup");
                HOperatorSet.WriteTuple(hLineRows, modelDirPath + "\\hLinesRow.dat");
                HOperatorSet.WriteTuple(hLineCols, modelDirPath + "\\hLinesCol.dat");
                HOperatorSet.WriteTuple(vLineRows, modelDirPath + "\\vLinesRow.dat");
                HOperatorSet.WriteTuple(vLineCols, modelDirPath + "\\vLinesCol.dat");
                HOperatorSet.WriteTuple(refPointsRow, modelDirPath + "\\cornorRows.dat");
                HOperatorSet.WriteTuple(refPointsCol, modelDirPath + "\\cornorCols.dat");

                if (showImage.IsInitialized())
                    HOperatorSet.WriteImage(showImage, "tiff", 0, modelDirPath + "\\showImage.tiff");
                if (matchRegion.IsInitialized())
                    HOperatorSet.WriteRegion(matchRegion, modelDirPath + "\\matchRegion.reg");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 模板匹配通用函数，仅匹配
        /// </summary>
        /// <param name="ho_image"></param>
        /// <param name="ho_roi"></param>
        /// <param name="ho_show_contour"></param>
        /// <param name="ho_update_show_contour"></param>
        /// <param name="hv_model_type"></param>
        /// <param name="hv_model_id"></param>
        /// <param name="hv_angle_start"></param>
        /// <param name="hv_angle_extent"></param>
        /// <param name="hv_score_thresh"></param>
        /// <param name="hv_match_num"></param>
        /// <param name="hv_def_row"></param>
        /// <param name="hv_def_col"></param>
        /// <param name="hv_found_row"></param>
        /// <param name="hv_found_col"></param>
        /// <param name="hv_found_angle"></param>
        /// <param name="hv_found_score"></param>
        /// <param name="hv_update_def_row"></param>
        /// <param name="hv_update_def_col"></param>
        /// <param name="hv_model_H_new"></param>
        /// <param name="hv_iFlag"></param>
        public static void find_model(HObject ho_image, HObject ho_roi, HObject ho_show_contour,
                                    out HObject ho_update_show_contour, HTuple hv_model_type, HTuple hv_model_id,
                                    HTuple hv_angle_start, HTuple hv_angle_extent, HTuple hv_score_thresh, HTuple hv_match_num,
                                    HTuple hv_def_row, HTuple hv_def_col, out HTuple hv_found_row, out HTuple hv_found_col,
                                    out HTuple hv_found_angle, out HTuple hv_found_score, out HTuple hv_update_def_row,
                                    out HTuple hv_update_def_col, out HTuple hv_model_H_new, out HTuple hv_iFlag)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_complement_region, ho_paint_image;
            HObject ho_reduced_image, ho_affine_show_contour = null;


            // Local control variables 

            HTuple hv_inv_model_row = new HTuple(), hv_inv_model_col = new HTuple();
            HTuple hv_ncc_numlevs = new HTuple(), hv_angle_step = new HTuple();
            HTuple hv_ncc_metric = new HTuple(), hv_local_row = new HTuple();
            HTuple hv_local_col = new HTuple(), hv_local_angle = new HTuple();
            HTuple hv_local_score = new HTuple(), hv_shape_numlevs = new HTuple();
            HTuple hv_shape_scale_min = new HTuple(), hv_shape_scale_max = new HTuple();
            HTuple hv_shape_scale_step = new HTuple(), hv_shape_metric = new HTuple();
            HTuple hv_shape_min_contrast = new HTuple(), hv_Greatereq = null;
            HTuple hv_GreaterInd = null, hv_model_row = new HTuple();
            HTuple hv_model_col = new HTuple(), hv_def_row_ind = new HTuple();
            HTuple hv_def_col_ind = new HTuple(), hv_i = new HTuple();
            HTuple hv_model_H_any = new HTuple(), hv__update_def_row = new HTuple();
            HTuple hv__update_def_col = new HTuple(), hv_update_model_row = new HTuple();
            HTuple hv_update_model_col = new HTuple();

            HTuple hv_angle_extent_COPY_INP_TMP = hv_angle_extent.Clone();
            HTuple hv_angle_start_COPY_INP_TMP = hv_angle_start.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_update_show_contour);
            HOperatorSet.GenEmptyObj(out ho_complement_region);
            HOperatorSet.GenEmptyObj(out ho_paint_image);
            HOperatorSet.GenEmptyObj(out ho_reduced_image);
            HOperatorSet.GenEmptyObj(out ho_affine_show_contour);

            hv_update_def_row = new HTuple();
            hv_update_def_col = new HTuple();
            hv_found_row = new HTuple();
            hv_found_col = new HTuple();
            hv_found_angle = new HTuple();
            hv_found_score = new HTuple();
            hv_model_H_new = new HTuple();
            hv_iFlag = 0;
            ho_update_show_contour.Dispose();
            HOperatorSet.GenEmptyObj(out ho_update_show_contour);

            HTuple channel;
            HOperatorSet.CountChannels(ho_image, out channel);
            if (channel.I != 1)
            {
                HOperatorSet.Rgb1ToGray(ho_image, out ho_image);
            }

            ho_complement_region.Dispose();
            HOperatorSet.Complement(ho_roi, out ho_complement_region);
            ho_paint_image.Dispose();
            HOperatorSet.PaintRegion(ho_complement_region, ho_image, out ho_paint_image,
                255, "fill");
            ho_reduced_image.Dispose();
            HOperatorSet.ReduceDomain(ho_paint_image, ho_roi, out ho_reduced_image);
            //******NCC/bin Ncc
            if ((int)(new HTuple(hv_model_type.TupleEqual(0))) != 0)
            {
                HOperatorSet.GetNccModelOrigin(hv_model_id, out hv_inv_model_row, out hv_inv_model_col);
                if ((int)((new HTuple(hv_angle_start_COPY_INP_TMP.TupleEqual(-1))).TupleOr(
                    new HTuple(hv_angle_extent_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    HOperatorSet.GetNccModelParams(hv_model_id, out hv_ncc_numlevs, out hv_angle_start_COPY_INP_TMP,
                        out hv_angle_extent_COPY_INP_TMP, out hv_angle_step, out hv_ncc_metric);
                }
                else
                {
                    hv_angle_start_COPY_INP_TMP = hv_angle_start_COPY_INP_TMP.TupleRad();
                    hv_angle_extent_COPY_INP_TMP = hv_angle_extent_COPY_INP_TMP.TupleRad();
                }
                HOperatorSet.FindNccModel(ho_reduced_image, hv_model_id, hv_angle_start_COPY_INP_TMP,
                    hv_angle_extent_COPY_INP_TMP, 0.5, hv_match_num, 0.5, "true", 0, out hv_local_row,
                    out hv_local_col, out hv_local_angle, out hv_local_score);
                //******Shape/Shape Xld
            }
            else
            {
                HOperatorSet.GetShapeModelOrigin(hv_model_id, out hv_inv_model_row, out hv_inv_model_col);
                if ((int)((new HTuple(hv_angle_start_COPY_INP_TMP.TupleEqual(-1))).TupleOr(
                    new HTuple(hv_angle_extent_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    HOperatorSet.GetShapeModelParams(hv_model_id, out hv_shape_numlevs, out hv_angle_start_COPY_INP_TMP,
                        out hv_angle_extent_COPY_INP_TMP, out hv_angle_step, out hv_shape_scale_min,
                        out hv_shape_scale_max, out hv_shape_scale_step, out hv_shape_metric,
                        out hv_shape_min_contrast);
                }
                else
                {
                    hv_angle_start_COPY_INP_TMP = hv_angle_start_COPY_INP_TMP.TupleRad();
                    hv_angle_extent_COPY_INP_TMP = hv_angle_extent_COPY_INP_TMP.TupleRad();
                }
                HOperatorSet.FindShapeModel(ho_reduced_image, hv_model_id, hv_angle_start_COPY_INP_TMP,
                    hv_angle_extent_COPY_INP_TMP, 0.5, hv_match_num, 0.5, "least_squares",
                    0, 0.9, out hv_local_row, out hv_local_col, out hv_local_angle, out hv_local_score);
            }

            if ((int)(new HTuple((new HTuple(hv_local_score.TupleLength())).TupleLess(1))) != 0)
            {
                hv_iFlag = -2;
                ho_complement_region.Dispose();
                ho_paint_image.Dispose();
                ho_reduced_image.Dispose();
                ho_affine_show_contour.Dispose();

                return;
            }
            HOperatorSet.TupleGreaterEqualElem(hv_local_score, hv_score_thresh, out hv_Greatereq);
            HOperatorSet.TupleFind(hv_Greatereq, 1, out hv_GreaterInd);
            if ((int)(new HTuple(hv_GreaterInd.TupleEqual(-1))) != 0)
            {
                hv_iFlag = -2;
                ho_complement_region.Dispose();
                ho_paint_image.Dispose();
                ho_reduced_image.Dispose();
                ho_affine_show_contour.Dispose();

                return;
            }
            else
            {
                //***获取模板中心坐标
                hv_model_row = -hv_inv_model_row;
                hv_model_col = -hv_inv_model_col;
                HOperatorSet.TupleFind(hv_def_row, -1, out hv_def_row_ind);
                HOperatorSet.TupleFind(hv_def_col, -1, out hv_def_col_ind);
                for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_GreaterInd.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                {
                    HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_local_row.TupleSelect(hv_GreaterInd.TupleSelect(
                        hv_i)), hv_local_col.TupleSelect(hv_GreaterInd.TupleSelect(hv_i)), hv_local_angle.TupleSelect(
                        hv_GreaterInd.TupleSelect(hv_i)), out hv_model_H_any);
                    ho_affine_show_contour.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_show_contour, out ho_affine_show_contour,
                        hv_model_H_any);

                    if ((int)((new HTuple(hv_def_row_ind.TupleEqual(-1))).TupleAnd(new HTuple(hv_def_col_ind.TupleEqual(
                        -1)))) != 0)
                    {
                        HOperatorSet.AffineTransPoint2d(hv_model_H_any, hv_def_row, hv_def_col,
                            out hv__update_def_row, out hv__update_def_col);
                    }
                    else
                    {
                        hv__update_def_row = new HTuple();
                        hv__update_def_col = new HTuple();
                    }
                    HOperatorSet.ConcatObj(ho_update_show_contour, ho_affine_show_contour, out OTemp[0]
                        );
                    ho_update_show_contour.Dispose();
                    ho_update_show_contour = OTemp[0];
                    //*****更新实际的模板坐标
                    HOperatorSet.AffineTransPoint2d(hv_model_H_any, hv_model_row, hv_model_col,
                        out hv_update_model_row, out hv_update_model_col);
                    hv_found_row = hv_found_row.TupleConcat(hv_update_model_row);
                    hv_found_col = hv_found_col.TupleConcat(hv_update_model_col);
                    hv_found_angle = hv_found_angle.TupleConcat(((hv_local_angle.TupleSelect(
                        hv_GreaterInd.TupleSelect(hv_i)))).TupleDeg());
                    hv_found_score = hv_found_score.TupleConcat(hv_local_score.TupleSelect(hv_GreaterInd.TupleSelect(
                        hv_i)));
                    hv_update_def_row = hv_update_def_row.TupleConcat(hv__update_def_row);
                    hv_update_def_col = hv_update_def_col.TupleConcat(hv__update_def_col);
                    hv_model_H_new = hv_model_H_new.TupleConcat(hv_model_H_any);
                }
            }

            ho_complement_region.Dispose();
            ho_paint_image.Dispose();
            ho_reduced_image.Dispose();
            ho_affine_show_contour.Dispose();

            return;
        }
        /// <summary>
        /// 拷贝整个model文件夹信息
        /// </summary>
        /// <param name="oriModel"></param>
        /// <returns></returns>
        public bool CopyModel(Model oriModel)
        {
            try
            {
                for (int i = 0; i < oriModel.modelID.Length; i++)
                {
                    modelID.Append(Vision.CopyModel(oriModel.modelID[i], oriModel.modelType));
                }
                modelType = oriModel.modelType;
                defRows = oriModel.defRows;
                defCols = oriModel.defCols;
                scoreThresh = oriModel.scoreThresh;
                angleStart = oriModel.angleStart;
                angleExtent = oriModel.angleExtent;
                if (oriModel.showContour.IsInitialized())
                    showContour = oriModel.showContour.CopyObj(1, -1);
                if (oriModel.showImage.IsInitialized())
                    showImage = oriModel.showImage.CopyObj(1, -1);
                if (oriModel.matchRegion.IsInitialized())
                    matchRegion = oriModel.matchRegion.CopyObj(1, -1);

                return true;
            }
            catch (Exception)
            {
                this.Dispose();
                return false;
            }
        }
        /// <summary>
        /// 释放模板信息
        /// </summary>
        /// <returns></returns>
        public bool DisposeModel()
        {
            for (int i = 0; i < modelID.Length; i++)
            {
                if (!Vision.ClearModel(modelType, modelID[i]))
                {
                    modelType = new HTuple();
                    modelID = new HTuple();
                    return false;
                }
            }
            modelType = new HTuple();
            modelID = new HTuple();
            showContour.Dispose();
            defRows = new HTuple();
            defCols = new HTuple();
            matchRegion.Dispose();

            return true;
        }
        /// <summary>
        /// 释放模板类的所有信息(包括参数)
        /// </summary>
        /// <returns></returns>
        public bool Dispose()
        {
            try
            {
                if (!DisposeModel()) return false;
                scoreThresh = new HTuple();
                angleStart = new HTuple();
                angleExtent = new HTuple();
                showImage.Dispose();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
