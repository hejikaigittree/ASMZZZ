using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using HalconDotNet;
using DLAF;
using System.IO;
using VisionMethonDll;

namespace DLAF_DS
{

    /// <summary>
    /// DLAF视觉定位算子类
    /// </summary>
    public class DLAFVisionFixer
    {
        /// <summary>
        /// 生成Die的检测区域
        /// </summary>
        /// <param name="ho_dieMatchRegion"></param>
        /// <param name="hv_imgCenterX"></param>
        /// <param name="hv_imgCenterY"></param>
        /// <param name="hv_dieWidth"></param>
        /// <param name="hv_dieHeight"></param>
        /// <param name="hv_dieX"></param>
        /// <param name="hv_dieY"></param>
        /// <param name="hv_uvHxy"></param>
        /// <param name="hv_imgWidth"></param>
        /// <param name="hv_imgHeight"></param>
        /// <param name="hv_imgWidthFactor"></param>
        /// <param name="hv_imgHeightFactor"></param>
        /// <param name="hv_zoomFactor"></param>
        /// <param name="hv_dilationSize"></param>
        /// <param name="hv_mapRowCnt"></param>
        /// <param name="hv_mapColCnt"></param>
        /// <param name="hv_dieRow"></param>
        /// <param name="hv_dieCol"></param>
        /// <param name="hv_iFlag"></param>
        public void gen_die_match_region(out HObject ho_dieMatchRegion, HTuple hv_imgCenterX,HTuple hv_imgCenterY, HTuple hv_dieWidth, HTuple hv_dieHeight, 
                                        HTuple hv_dieX,HTuple hv_dieY, HTuple hv_uvHxy, HTuple hv_imgWidth, HTuple hv_imgHeight, HTuple hv_imgWidthFactor,
                                        HTuple hv_imgHeightFactor, HTuple hv_zoomFactor, HTuple hv_dilationSize, HTuple hv_mapRowCnt,HTuple hv_mapColCnt, 
                                        out HTuple hv_dieRow, out HTuple hv_dieCol, out HTuple hv_iFlag)
        {
            // Local iconic variables 
            // Local control variables 
            HTuple hv_uvHxyScaled = new HTuple(), hv_xyHuvScale = new HTuple();
            HTuple hv_viewWidth = new HTuple(), hv_viewHeight = new HTuple();
            HTuple hv_viewXLT = new HTuple(), hv_viewYLT = new HTuple();
            HTuple hv_viewXRB = new HTuple(), hv_viewYRB = new HTuple();
            HTuple hv_dieXLT = new HTuple(), hv_dieYLT = new HTuple();
            HTuple hv_dieXRB = new HTuple(), hv_dieYRB = new HTuple();
            HTuple hv_xLTGreater = new HTuple(), hv_yLTLess = new HTuple();
            HTuple hv_xRBLess = new HTuple(), hv_yRBGreater = new HTuple();
            HTuple hv_greater = new HTuple(), hv_greaterInd = new HTuple();
            HTuple hv_viewDieX = new HTuple(), hv_viewDieY = new HTuple();
            HTuple hv_deltaX = new HTuple(), hv_deltaY = new HTuple();
            HTuple hv_xyHuv = new HTuple(), hv_deltaRow = new HTuple();
            HTuple hv_deltaCol = new HTuple(), hv_dieImgRow = new HTuple();
            HTuple hv_dieImgCol = new HTuple(), hv_dieImgHeight = new HTuple();
            HTuple hv_dieImgWidth = new HTuple(), hv_row1 = new HTuple();
            HTuple hv_col1 = new HTuple(), hv_row2 = new HTuple();
            HTuple hv_col2 = new HTuple(), hv_Exception = null;
            HTuple hv_Less = new HTuple(), hv_Indices1 = new HTuple(), hv_Greater = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_dieMatchRegion);
            hv_iFlag = "Success";
            ho_dieMatchRegion.Dispose();
            HOperatorSet.GenEmptyObj(out ho_dieMatchRegion);
            hv_dieRow = new HTuple();
            hv_dieCol = new HTuple();
            try
            {
                HOperatorSet.HomMat2dScale(hv_uvHxy, 1 / hv_zoomFactor, 1 / hv_zoomFactor, 0, 0,
                    out hv_uvHxyScaled);
                HOperatorSet.HomMat2dInvert(hv_uvHxyScaled, out hv_xyHuvScale);
                hv_viewWidth = (((((hv_uvHxy.TupleSelect(0)) * hv_imgHeight) * hv_imgHeightFactor) + (((hv_uvHxy.TupleSelect(
                    1)) * hv_imgWidth) * hv_imgWidthFactor))).TupleAbs();
                hv_viewHeight = (((((hv_uvHxy.TupleSelect(3)) * hv_imgHeight) * hv_imgHeightFactor) + (((hv_uvHxy.TupleSelect(
                    4)) * hv_imgWidth) * hv_imgWidthFactor))).TupleAbs();
                hv_viewXLT = hv_imgCenterX - (hv_viewWidth / 2.0);
                hv_viewYLT = hv_imgCenterY + (hv_viewHeight / 2.0);
                hv_viewXRB = hv_imgCenterX + (hv_viewWidth / 2.0);
                hv_viewYRB = hv_imgCenterY - (hv_viewHeight / 2.0);
                hv_dieXLT = hv_dieX - (hv_dieWidth / 2.0);
                hv_dieYLT = hv_dieY + (hv_dieHeight / 2.0);
                hv_dieXRB = hv_dieX + (hv_dieWidth / 2.0);
                hv_dieYRB = hv_dieY - (hv_dieHeight / 2.0);

                HOperatorSet.TupleGreaterEqualElem(hv_dieXLT, hv_viewXLT, out hv_xLTGreater);
                HOperatorSet.TupleGreaterEqualElem(hv_viewYLT, hv_dieYLT, out hv_yLTLess);
                HOperatorSet.TupleLessEqualElem(hv_dieXRB, hv_viewXRB, out hv_xRBLess);
                HOperatorSet.TupleLessEqualElem(hv_viewYRB, hv_dieYRB, out hv_yRBGreater);
                hv_greater = ((hv_xLTGreater * hv_yLTLess) * hv_xRBLess) * hv_yRBGreater;
                HOperatorSet.TupleFind(hv_greater, 1, out hv_greaterInd);
                if ((int)(new HTuple(hv_greaterInd.TupleEqual(-1))) != 0)
                {
                    return;
                }
                hv_viewDieX = hv_dieX.TupleSelect(hv_greaterInd);
                hv_viewDieY = hv_dieY.TupleSelect(hv_greaterInd);
                hv_deltaX = hv_imgCenterX - hv_viewDieX;
                hv_deltaY = hv_imgCenterY - hv_viewDieY;
                HOperatorSet.HomMat2dInvert(hv_uvHxy, out hv_xyHuv);
                hv_deltaRow = ((hv_xyHuv.TupleSelect(0)) * hv_deltaX) + ((hv_xyHuv.TupleSelect(
                    1)) * hv_deltaY);
                hv_deltaCol = ((hv_xyHuv.TupleSelect(3)) * hv_deltaX) + ((hv_xyHuv.TupleSelect(
                    4)) * hv_deltaY);
                hv_dieImgRow = ((hv_imgHeight - 1) / 2.0) + hv_deltaRow;
                hv_dieImgCol = ((hv_imgWidth - 1) / 2.0) + hv_deltaCol;
                hv_dieImgHeight = ((((hv_xyHuv.TupleSelect(0)) * hv_dieWidth) + ((hv_xyHuv.TupleSelect(
                    1)) * (-hv_dieHeight)))).TupleAbs();
                hv_dieImgWidth = ((((hv_xyHuv.TupleSelect(3)) * hv_dieWidth) + ((hv_xyHuv.TupleSelect(
                    4)) * (-hv_dieHeight)))).TupleAbs();
                hv_row1 = (hv_dieImgRow - (hv_dieImgHeight / 2.0)) - hv_dilationSize;
                hv_col1 = (hv_dieImgCol - (hv_dieImgWidth / 2.0)) - hv_dilationSize;
                hv_row2 = (hv_dieImgRow + (hv_dieImgHeight / 2.0)) + hv_dilationSize;
                hv_col2 = (hv_dieImgCol + (hv_dieImgWidth / 2.0)) + hv_dilationSize;
                //if ((int)((new HTuple((new HTuple((new HTuple(hv_row1.TupleLess(0))).TupleOr(
                //    new HTuple(hv_col1.TupleLess(0))))).TupleOr(
                //    new HTuple(hv_row2.TupleGreater(hv_imgHeight))))).TupleOr(
                //    new HTuple(hv_col2.TupleGreater(hv_imgWidth)))) != 0)
                //{

                //    return;
                //}
                //*2019/11/4 修改 解决膨胀区域超过图片像素时直接return的问题 上面注释的代码
                HOperatorSet.TupleLessElem(hv_row1, 0, out hv_Less);
                HOperatorSet.TupleFind(hv_Less, 1, out hv_Indices1);
                if ((int)(new HTuple(hv_Indices1.TupleNotEqual(-1))) != 0)
                {
                    if (hv_row1 == null)
                        hv_row1 = new HTuple();
                    hv_row1[hv_Indices1] = 0;
                }

                HOperatorSet.TupleLessElem(hv_col1, 0, out hv_Less);
                HOperatorSet.TupleFind(hv_Less, 1, out hv_Indices1);
                if ((int)(new HTuple(hv_Indices1.TupleNotEqual(-1))) != 0)
                {
                    if (hv_col1 == null)
                        hv_col1 = new HTuple();
                    hv_col1[hv_Indices1] = 0;
                }

                HOperatorSet.TupleGreaterElem(hv_row2, hv_imgHeight, out hv_Greater);
                HOperatorSet.TupleFind(hv_Greater, 1, out hv_Indices1);
                if ((int)(new HTuple(hv_Indices1.TupleNotEqual(-1))) != 0)
                {
                    if (hv_row2 == null)
                        hv_row2 = new HTuple();
                    hv_row2[hv_Indices1] = hv_imgHeight;
                }

                HOperatorSet.TupleGreaterElem(hv_col2, hv_imgWidth, out hv_Greater);
                HOperatorSet.TupleFind(hv_Less, 1, out hv_Indices1);
                if ((int)(new HTuple(hv_Indices1.TupleNotEqual(-1))) != 0)
                {
                    if (hv_col2 == null)
                        hv_col2 = new HTuple();
                    hv_col2[hv_Indices1] = hv_imgWidth;
                }
                ho_dieMatchRegion.Dispose();
                HOperatorSet.GenRectangle1(out ho_dieMatchRegion, hv_row1, hv_col1, hv_row2,
                    hv_col2);
                hv_dieRow = hv_greaterInd / hv_mapColCnt;
                hv_dieCol = hv_greaterInd % hv_mapColCnt;
            }
            catch (HalconException HDevExpDefaultException1)
            {
                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                GetErrInfo(hv_Exception, out hv_iFlag);
            }
            return;
        }

        public void GetErrInfo(HTuple hv_Exception, out HTuple hv_ErrMessage)
        {
            // Local iconic variables 
            // Initialize local and output iconic variables 
            hv_ErrMessage = (((("method [" + (hv_Exception.TupleSelect(7))) + "] use [") + (hv_Exception.TupleSelect(
                5))) + "] err! ") + (hv_Exception.TupleSelect(2));

            return;
        }


        public DLAFVisionFixer()
        {

        }

        bool _isInitOK = false;
        string _calibFilePath = null;//标定文件路径
        int _imgWidth = 0;//像素图片宽度
        int _imgHegiht = 0;//像素图片高度

        ToolKits.FunctionModule.Vision tool_vision;
        HTuple _cmrCalibData = null;
        public DLAF.Model _leftMarkModel;  //左矫正点模板
        public DLAF.Model _rightMarkModel; //右矫正点模板


        JFDLAFProductRecipe _recipe = null;

        public bool Init(string cmrCalibFilePath, JFDLAFProductRecipe recipe, int imgWidth, int imgHeight, out string errorInfo)
        {
            _isInitOK = false;
            errorInfo = "None-Options";
            _calibFilePath = cmrCalibFilePath;
            _imgWidth = imgWidth;
            _imgHegiht = imgHeight;

            if (!File.Exists(_calibFilePath))
            {
                errorInfo = "相机标定文件:\"" + _calibFilePath + "\" 不存在";
                return false;
            }

            if(!InitUV2XYParam(out errorInfo))
            {
                return false;
            }

            _recipe = recipe;
            if (null == _recipe) //日后添加其他合法性检测
            {
                errorInfo = "Recipe对象无效/空值";
                return false;
            }
            if (_leftMarkModel != null)
                _leftMarkModel.Dispose();
            if (_rightMarkModel != null)
                _rightMarkModel.Dispose();

            _leftMarkModel = new Model();
            _rightMarkModel = new Model();
            if (!_leftMarkModel.ReadModelCorrect(JFHubCenter.Instance.RecipeManager.GetInitParamValue((string)JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + _recipe.ID + "\\CheckPosModels"))
            {
                errorInfo = "无法读取该产品左矫正点模板信息.\n";
                return false;
            }
            if (!_rightMarkModel.ReadModelCorrect(JFHubCenter.Instance.RecipeManager.GetInitParamValue((string)JFHubCenter.Instance.RecipeManager.InitParamNames[2]) + "\\" + _recipe.ID + "\\CheckPosModelsR"))
            {
                errorInfo = "无法读取该产品右矫正点模板信息.\n";
                return false;
            }

            _isInitOK = true;
            errorInfo = "Success";
            return true;

        }

        /// <summary>
        /// 加载标定文件
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        private bool InitUV2XYParam(out string errorInfo)
        {
            try
            {
                _cmrCalibData = new HTuple();
                tool_vision = new ToolKits.FunctionModule.Vision();
                tool_vision.read_hom2d(_calibFilePath, out _cmrCalibData);         
            }
            catch
            {
                errorInfo="无法读取UV-XY结果文件.";
                return false;
            }
            errorInfo = "Success";
            return true;
        }

       
        /// <summary>
        /// 生成芯片实际扫描点位
        /// </summary>
        /// <param name="imgs"></param>
        /// <param name="snapX"></param>
        /// <param name="snapY"></param>
        /// <param name="icsCenterX"></param>
        /// <param name="icsCenterY"></param>
        /// <param name="fovsOffsetX"></param>
        /// <param name="fovsOffsetY"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool CalibProduct(IJFImage[] imgs, double[] snapX, double[] snapY, out double[] icsCenterX, out double[] icsCenterY, 
                                out double[] fovsOffsetX, out double[] fovsOffsetY,out string errorInfo)
        {

            if (!_isInitOK)
            {
                icsCenterX = null;
                icsCenterY = null;
                fovsOffsetX = null;
                fovsOffsetY = null;
                errorInfo = "初始化未完成";
                return false;
            }

            HObject[] himgs = new HObject[] { null, null };
            object oi;
            if (0 != imgs[0].GenHalcon(out oi))
            {
                icsCenterX = null;
                icsCenterY = null;
                fovsOffsetX = null;
                fovsOffsetY = null;
                errorInfo = "左Mark图像转Halcon失败！";
                return false;
            }
            himgs[0] = oi as HObject;

            if (0 != imgs[1].GenHalcon(out oi))
            {
                icsCenterX = null;
                icsCenterY = null;
                fovsOffsetX = null;
                fovsOffsetY = null;
                errorInfo = "右Mark图像转Halcon失败！";
                return false;
            }
            himgs[1] = oi as HObject;

            //左矫正点
            HTuple hv_iFlag = new HTuple();
            HTuple hvLeftModeHMap = new HTuple();
            HTuple hvDefLeft_coorX = new HTuple();
            HTuple hvDefLeft_coorY = new HTuple();
            HTuple hvDefLeft_updateX = new HTuple();
            HTuple hvDefLeft_updateY = new HTuple();
            HTuple hvDefLeft_updateRow = new HTuple();
            HTuple hvDefLeft_updateCol = new HTuple();

            if (_leftMarkModel.matchRegion == null || !_leftMarkModel.matchRegion.IsInitialized())
                HOperatorSet.GetDomain(himgs[0], out _leftMarkModel.matchRegion);

            VisionMethon.coor_uvToxy_point(himgs[0], _leftMarkModel.defRows, _leftMarkModel.defCols, _recipe.CheckPosX, _recipe.CheckPosY,
                    _cmrCalibData, out hvDefLeft_coorX, out hvDefLeft_coorY, out hv_iFlag);
            if (hv_iFlag.S != "")
            {
                icsCenterX = null;
                icsCenterY = null;
                fovsOffsetX = null;
                fovsOffsetY = null;
                hvDefLeft_updateX = null;
                hvDefLeft_updateY = null;
                errorInfo = "左模板中心像素转换成实际坐标失败！" + hv_iFlag.S;
                return false;
            }
            VisionMethon.Map_Points_Match(himgs[0], _leftMarkModel.matchRegion, _leftMarkModel.modelType, _leftMarkModel.modelID,
                _recipe.CheckPosScoreThresh, _leftMarkModel.defRows, _leftMarkModel.defCols, hvDefLeft_coorX, hvDefLeft_coorY,
                _cmrCalibData, out hvLeftModeHMap, out hvDefLeft_updateX, out hvDefLeft_updateY, out hvDefLeft_updateRow, out hvDefLeft_updateCol, out hv_iFlag);
            if (hv_iFlag.S != "")
            {
                icsCenterX = null;
                icsCenterY = null;
                fovsOffsetX = null;
                fovsOffsetY = null;
                hvDefLeft_updateX = null;
                hvDefLeft_updateY = null;
                errorInfo = "左矫正点匹配失败！" + hv_iFlag.S;
                return false;
            }

            //if (showRegion != null) showRegion.Dispose();
            //HOperatorSet.GenCrossContourXld(out showRegion, hvDefLeft_updateRow, hvDefLeft_updateCol, 512, 0);
            //ShowImage(hTWindow, himgs[0], showRegion);

            //左矫正点
            HTuple hvRightModeHMap = new HTuple();
            HTuple hvDefRight_coorX = new HTuple();
            HTuple hvDefRight_coorY = new HTuple();
            HTuple hvDefRight_updateX = new HTuple();
            HTuple hvDefRight_updateY = new HTuple();
            HTuple hvDefRight_updateRow = new HTuple();
            HTuple hvDefRight_updateCol = new HTuple();

            if (_rightMarkModel.matchRegion == null || !_rightMarkModel.matchRegion.IsInitialized())
                HOperatorSet.GetDomain(himgs[1], out _rightMarkModel.matchRegion);

            VisionMethon.coor_uvToxy_point(himgs[1], _rightMarkModel.defRows, _rightMarkModel.defCols, _recipe.CheckPosRX, _recipe.CheckPosRY,
                    _cmrCalibData, out hvDefRight_coorX, out hvDefRight_coorY, out hv_iFlag);
            if (hv_iFlag.S != "")
            {
                icsCenterX = null;
                icsCenterY = null;
                fovsOffsetX = null;
                fovsOffsetY = null;
                errorInfo = "右模板中心像素转换成实际坐标失败！" + hv_iFlag.S;
                return false;
            }
            VisionMethon.Map_Points_Match(himgs[1], _rightMarkModel.matchRegion, _rightMarkModel.modelType, _rightMarkModel.modelID,
                _recipe.CheckPosRScoreThresh, _rightMarkModel.defRows, _rightMarkModel.defCols, hvDefRight_coorX, hvDefRight_coorY,
                _cmrCalibData, out hvRightModeHMap, out hvDefRight_updateX, out hvDefRight_updateY, out hvDefRight_updateRow, out hvDefRight_updateCol, out hv_iFlag);
            if (hv_iFlag.S != "")
            {
                icsCenterX = null;
                icsCenterY = null;
                fovsOffsetX = null;
                fovsOffsetY = null;
                errorInfo = "右矫正点匹配失败！" + hv_iFlag.S;
                return false;
            }

            //if (showRegion != null) showRegion.Dispose();
            //HOperatorSet.GenCrossContourXld(out showRegion, hvDefLeft_updateRow, hvDefLeft_updateCol, 512, 0);
            //ShowImage(hTWindow, himgs[1], showRegion);

            //生成矫正后芯片扫描点位
            HTuple hvAfterFix_SnapX = new HTuple();
            HTuple hvAfterFix_SnapY = new HTuple();
            HTuple hvAfterFix_ICX = null;
            HTuple hvAfterFix_ICY = null;
            HTuple hv_du = null;
            HTuple hv_dv = null;

            VisionMethon.update_map_correction(_recipe.icMapX, _recipe.icMapY, _recipe.snapMapX, _recipe.snapMapY,
                hvDefLeft_coorX, hvDefLeft_coorY, hvDefRight_coorX, hvDefRight_coorY, hvDefLeft_updateX,
                hvDefLeft_updateY, hvDefRight_updateX, hvDefRight_updateY, _cmrCalibData, out hvAfterFix_ICX,
                out hvAfterFix_ICY, out hvAfterFix_SnapX, out hvAfterFix_SnapY, out hv_iFlag, out hv_du, out hv_dv);
            if (hv_iFlag.S != "")
            {
                icsCenterX = null;
                icsCenterY = null;
                fovsOffsetX = null;
                fovsOffsetY = null;
                errorInfo = "生成矫正点后扫描点位失败！" + hv_iFlag.S;
                return false;
            }

            //Fov矫正日后添加
            int fovCount = _recipe.FovCount;
            fovsOffsetX = new double[fovCount];
            fovsOffsetY = new double[fovCount];
            for (int i = 0; i < fovCount; i++)
                _recipe.GetFovOffset(_recipe.FovNames()[i], out fovsOffsetX[i], out fovsOffsetY[i]);



            double[] _icsCenterX = new double[hvAfterFix_SnapX.Length];
            double[] _icsCenterY = new double[hvAfterFix_SnapX.Length];
            icsCenterX = new double[hvAfterFix_SnapX.Length];
            icsCenterY = new double[hvAfterFix_SnapX.Length];

            for (int i = 0; i < hvAfterFix_SnapX.Length; i++)
            {
                _icsCenterX[i] = hvAfterFix_SnapX.TupleSelect(i).D;
                _icsCenterY[i] = hvAfterFix_SnapY.TupleSelect(i).D;
            }

            //重新排序每行扫描按照从左到右进行扫描
            int currentIndex = 0;
            for (int m = 0; m < _recipe.RowCount; m++)
            {
                for (int n = 0; n < _recipe.ColCount; n++)
                {
                    int index = 0;
                    if (m % 2 == 0)
                    {
                        index = (int)(m * _recipe.ColCount + n);
                    }
                    else
                    {
                        index = (int)(m * _recipe.ColCount + (_recipe.ColCount - 1 - n));
                    }
                    icsCenterX[currentIndex] = _icsCenterX[index];
                    icsCenterY[currentIndex] = _icsCenterY[index];

                    currentIndex++;
                }
            }
            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 按照行列信息获取单颗IC的扫描点位
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="diesRows"></param>
        /// <param name="diesCols"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool GetSignalICSnapPosition(int row, int col, double[] icsCenterX, double[] icsCenterY, out double x, out double y, out string errorInfo)
        {
            x = 0;
            y = 0;
            int index = 0;
            if (row % 2 == 0)
            {
                index = (int)(row * _recipe.ColCount + col);
            }
            else
            {
                index = (int)(row * _recipe.ColCount + (_recipe.ColCount - 1 - col));
            }

            if (index >= icsCenterX.Length)
            {
                x = 0;
                y = 0;
                errorInfo = "矫正后扫描点位数组溢出";
                return false;
            }

            x = icsCenterX[index];
            y = icsCenterY[index];
            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 获取检测区域
        /// </summary>
        /// <param name="icRow"></param>
        /// <param name="icCol"></param>
        /// <param name="regions"></param>
        /// <param name="diesRows"></param>
        /// <param name="diesCols"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool GetDetectRegionInFov(int icRow, int icCol, out HObject regions, out int[] diesRows, out int[] diesCols, out int diesRow, out int diesCol,out string errorInfo)
        {
            regions = null;
            diesRows = null;
            diesCols = null;
            diesRow = 0;
            diesCol = 0;

            // Initialize local and output iconic variables 
            double snapPositionX = 0, snapPositionY = 0;//Recipe拍照点XY
            _recipe.GetICSnapCenter(icRow, icCol, out snapPositionX, out snapPositionY);

            HTuple hv_dilationSize = new HTuple();
            HTuple hv_dieRows = new HTuple();
            HTuple hv_dieCols = new HTuple();
            HTuple hv_iFlag = new HTuple();
            HTuple hv_uvHXY = new HTuple();
            hv_dilationSize = 0;//该参数值不可变

            if (_recipe.DieRowInFov == 0 && _recipe.DieColInFov == 0)//大芯片
            {
                HOperatorSet.GenRectangle1(out regions, 0, 0, _imgHegiht, _imgWidth);
                errorInfo = "Success";
                diesRows = new int[1];
                diesCols = new int[1];
                diesRows[0] = icRow;
                diesCols[0] = icCol;
                diesRow = 0;
                diesCol = 0;
            }
            else//小芯片
            {
                gen_die_match_region(out regions, snapPositionX, snapPositionY, _recipe.DieWidth, _recipe.DieHeight, _recipe.icMapX, _recipe.icMapY,
                                _cmrCalibData, _imgWidth, _imgHegiht, _recipe.WidthFactor, _recipe.HeightFactor, _recipe.ScaleFactor,
                                hv_dilationSize, _recipe.RowNumber, _recipe.ColumnNumber * _recipe.BlockNumber, out hv_dieRows, out hv_dieCols, out hv_iFlag);

                int iregion = regions.CountObj();
                if (hv_iFlag.S != "Success")
                {
                    errorInfo = hv_iFlag.S;
                    return false;
                }

                diesRow = hv_dieRows.TupleMax().I - hv_dieRows.TupleMin().I+1;
                diesCol = hv_dieCols.TupleMax().I - hv_dieCols.TupleMin().I+1;

                diesRows = new int[hv_dieRows.Length];
                diesCols = new int[hv_dieRows.Length];
                for (int i = 0; i < hv_dieRows.Length; i++)
                {
                    diesRows[i] = hv_dieRows.TupleSelect(i).I;
                    diesCols[i] = hv_dieCols.TupleSelect(i).I;
                }
                errorInfo = "Success";
            }
            return true;
        }

    }
}
