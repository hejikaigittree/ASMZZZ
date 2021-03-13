using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;
using HalconDotNet;
using HT_Lib;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;
using JFMethodCommonLib;
using HTHalControl;
using System.ComponentModel;
using IniDll;

namespace DLAF
{
    public class Show
    {
        public HObject Image;
        public int CamInd;
        /// <summary>
        /// 0表示仅显示图像，1表示自动聚焦,2表示匹配
        /// </summary>
        public int Mode;
        /// <summary>
        /// 映射点中心行坐标
        /// </summary>
        public double Row;
        /// <summary>
        /// 映射点中心列坐标
        /// </summary>
        public double Column;

        public Show()
        {
            Row = 0;
            Column = 0;
        }
        public void Dispose()
        {
            if (Image != null) Image.Dispose();
        }
    }

    public class CalibOperation
    {
        private CalibStation _station;
        
        public double _xUv2xy;
        public double _yUv2xy;
        public double _zUv2xy;
        public string errString = "";

        public double XUv2xy
        {
            get { return _xUv2xy; }
            set { _xUv2xy = value; }
        }
        public double YUv2xy
        {
            get { return _yUv2xy; }
            set { _yUv2xy = value; }
        }
        public double ZUv2xy
        {
            get { return _zUv2xy; }
            set { _zUv2xy = value; }
        }

        public event EventHandler<Show> GrabImageDoneHandler;
        public CalibOperation()
        {

        }

        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            Unsupported = -1,//设备不支持此功能
            ParamError = -2,//参数错误（不支持的参数）
            InvokeFailed = -3,//库函数调用出错
            Allowed = 1,//调用成功，但不是所有的参数都支持
            InitFailedWhenOpenCard = -4,//
            Timeout = -5,//
        }

        public void SaveCameraData()
        {
        }

        #region Motion
        public void SetStation(CalibStation station)
        {
            _station = station;
        }

        /// <summary>
        /// 软触发
        /// </summary>
        /// <param name="TrigChnName"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int SWPosTrig(string[] TrigChnName, out string errMsg)
        {
            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            errMsg = "";

            if (TrigChnName.Length == 0)
            {
                errMsg = "触发通道名称为空";
                return (int)ErrorDef.InvokeFailed;
            }

            int[] trigChns = new int[TrigChnName.Length];
            for (int i = 0; i < TrigChnName.Length; i++)
            {
                if (!JFCMFunction.CheckDevCellName(JFCMFunction.CmpTrig, TrigChnName[i], out dev, out ci, out errMsg))
                    return (int)ErrorDef.InvokeFailed;
                trigChns[i] = ci.ChannelIndex;
            }

            IJFModule_CmprTrigger md = (dev as IJFDevice_MotionDaq).GetCompareTrigger(ci.ModuleIndex);
            int errCode = md.SoftTrigge(trigChns);
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = md.GetErrorInfo(errCode);
                return errCode;
            }
            return (int)ErrorDef.Success;
        }

        HObject image;
        /// <summary>
        /// 已测试
        /// 软触发拍照下，计算相机的uv->xy坐标系变换的操作，如large相机
        /// 要求之前设置好相机/光源/触发至“合理”状态
        /// [x]   =   [ H1  H2  H3  ]    [u]
        /// [y]   =   [ H4  H5  H6  ] *  [v]
        ///                              [1]
        /// </summary>
        /// <param name="matUV2XY">uv->xy变换矩阵，将2*3的矩阵按照行堆叠的方式存储为1*6的数组, ref传递方式，需要数组有一个初始值</param>
        /// <param name="cam">相机控制类，注意曝光时间需要设置合适。软触发拍照建议曝光时间在100ms左右</param>
        /// <param name="modelID">匹配的ncc模板</param>
        /// <param name="score_thresh">匹配分数，需要大于0.5</param>
        /// <param name="axisPara">运动轴信息（xy插补），具体包括轴速，加速度</param>
        /// <param name="initPoint">初始点，中间仅适用了xy信息，需要保证走到初始点时室内内包含完整的，可匹配的model</param>
        /// <param name="lightInd">软触发拍照对应的光源id，需要事先设置好对应光源合适的时间</param>
        /// <param name="axisInd">x，y轴号</param>
        /// <param name="xyRange">计算变换时，以初始点为中心，随机走位的范围，单位mm</param>
        /// <param name="nPoints">随机走位多少个点</param>
        /// <param name="lightDelay">软触发拍照光源的延迟，单位us，典型值10us</param>
        /// <param name="timeOut">单次运动的延迟，单位ms</param>
        /// <param name="winID">(调试用）显示窗口ID</param>
        /// <returns>操作是否成功</returns>
        public bool calibUV2XY
                (
                int camInd,
                ref HTuple matUV2XY,
                Model model,
                double xyRange,
                int nPoints = 20
                )

        {
            //if (model.ReadModel(CalibrUV2XYModelPath) == false)
            //{
            //    HTUi.PopError("加载标定模版失败！");
            //    return false;
            //}
            string errMsg = "";
            if (_station.operation.MultiAxisMove(_station.AxisXYZ,new double[] { _xUv2xy, _yUv2xy, _zUv2xy },true,out errMsg) !=0)
            {
                HTUi.PopError("无法移动至标定点位置！");
                return false;
            }
            Thread.Sleep(200);
            //  App.obj_light.FlashMultiLight(LightUseFor.ScanPoint1st);
            
            //if(_station.operation.SWPosTrig(_station.AxisX,out errMsg)!=0)
            //{
            //    HTUi.PopError(errMsg);
            //    return false;
            //}

            Thread.Sleep(10);
            if (image != null)
                image.Dispose();
            HOperatorSet.GenEmptyObj(out image);

            if(_station.operation.CaputreOneImage(_station.CamereDev[camInd], "Halcon",out image,out errMsg)!=0)
            {
                HTUi.PopError("采集图像失败：" + errMsg);
                return false;
            }

            //3. match
            HTuple u, v, angle;
            bool status = matchModel(camInd, ref image, model, out u, out v, out angle);
            HObject showRegion = new HObject();
            showRegion.Dispose();
            HOperatorSet.GenCrossContourXld(out showRegion, u, v, 512, 0);
            _station._rtUi._fm.ShowImage(_station._rtUi._fm.htWindowCalibration, image, showRegion);
            //image.Dispose();
            if (!status)
            {
                HTUi.PopError("获取匹配初始位置图像失败");
                return false;

            }
            HTuple xArr = new HTuple(), yArr = new HTuple(), uArr = new HTuple(), vArr = new HTuple();
            Random rand = new Random();
            //4. for ... snap , match, add <u,v,x,y>
            for (int i = 0; i < nPoints; i++)
            {
                DateTime t1 = DateTime.Now;
                //大于或等于 0.0 且小于 1.0 的双精度浮点数
                double x = (rand.NextDouble() - 0.5) * xyRange + _xUv2xy;
                double y = (rand.NextDouble() - 0.5) * xyRange + _yUv2xy;

                if (_station.operation.MultiAxisMove(new string[] { _station.AxisXYZ[0], _station.AxisXYZ[1], }, new double[] { x, y }, true, out errMsg) != 0)
                {
                    HTUi.PopError("无法移动至标定点位置！");
                    return false;
                }

                Thread.Sleep(200); ;

                //if (_station.operation.SWPosTrig(_station.AxisX, out errMsg) != 0)
                //{
                //    HTUi.PopError(errMsg);
                //    return false;
                //}

                Thread.Sleep(10);
                if (image != null)
                    image.Dispose();
                HOperatorSet.GenEmptyObj(out image);

                if (_station.operation.CaputreOneImage(_station.CamereDev[camInd], "Halcon", out image, out errMsg) != 0)
                {
                    HTUi.PopError("采集图像失败：" + errMsg);
                    return false;
                }

                if (matchModel(camInd, ref image, model, out u, out v, out angle)) //found something
                {
                    xArr.Append(x); yArr.Append(y); uArr.Append(u); vArr.Append(v);
                }
                showRegion.Dispose();
                HOperatorSet.GenCrossContourXld(out showRegion, u, v, 512, 0);
                _station._rtUi._fm.ShowImage(_station._rtUi._fm.htWindowCalibration, image, showRegion);
                //image.Dispose();
            }
            if (xArr.Length < 10)
            {
                HTUi.PopError("有效点数不够");
                return false;
            }
            //5. least square estimation
            Matrix<double> In = Matrix<double>.Build.Dense(3, xArr.Length, 1.0); //by default In[2,:] = 1.0
            Matrix<double> Out = Matrix<double>.Build.Dense(2, xArr.Length);
            Out.SetRow(0, xArr.ToDArr());
            Out.SetRow(1, yArr.ToDArr());
            In.SetRow(0, uArr.ToDArr());
            In.SetRow(1, vArr.ToDArr());
            Matrix<double> A = vec2Mat(In, Out);
            //6. move to center of uv space
            double[] aArr = A.ToRowWiseArray(); //need to be tested
            _station._calibrationUV2XYParameter = string.Join(",", aArr.ToArray());
            //parse
            if (matUV2XY == null)
            {
                matUV2XY = new HTuple();
            }
            for (int i = 0; i < 6; i++)
            {
                matUV2XY.Append(aArr[i]);
            }
            return true;
        }

        /// <summary>
        /// 多轴运动(非插补运动)
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="distance"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int MultiAxisMove(string[] AxisName, double[] distance, bool isAbs,out string errMsg)
        {
            errMsg = "";

            if (AxisName.Length == 0)
            {
                errMsg = "轴通道名称为空";
                return (int)ErrorDef.InvokeFailed;
            }

            if (AxisName.Length != distance.Length)
            {
                errMsg = "轴通道名称数量与位置坐标数量不匹配";
                return (int)ErrorDef.InvokeFailed;
            }

            for (int i = 0; i < AxisName.Length; i++)
            {
                if (!_station.MoveAxis(AxisName[i], distance[i], isAbs,out errMsg))
                {
                    return (int)ErrorDef.InvokeFailed;
                }
            }

            int errCode = 0;
            errCode = MotionDone(AxisName, out errMsg,10000);
            if (errCode != 0)
            {
                return errCode;
            }
            return (int)ErrorDef.Success;
        }


        /// <summary>
        /// 轴动作完成
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="errMsg"></param>
        /// <param name="timeoutSelSecond"></param>
        /// <returns></returns>
        private int MotionDone(string[] AxisName, out string errMsg, int timeoutSelSecond = -1)
        {
            errMsg = "";
            for (int i = 0; i < AxisName.Length; i++)
            {
                if (_station.WaitMotionDone(AxisName[i], timeoutSelSecond) !=JFWorkCmdResult.Success)
                    return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }

        HObject ImageSrc;
        /// <summary>
        /// 图像匹配函数，测试使用，可重新编写
        /// 只实现了简单ncc匹配，不支持roi内匹配，只能匹配一个目标
        /// </summary>
        /// <param name="image">输入图像</param>
        /// <param name="model">模型</param>
        /// <param name="u">返回匹配行坐标，pixel，如果失败为null</param>
        /// <param name="v">返回匹配列坐标，pixel，如果失败为null</param>
        /// <param name="angle">返回匹配角度，弧度，如果失败为null</param>
        /// <returns>是否匹配成功</returns>
        public bool matchModel(int camInd, ref HObject image, Model model, out HTuple u, out HTuple v, out HTuple angle)
        {
            //HTuple _u = new HTuple(), _v = new HTuple(), _angle = new HTuple();
            //bool matched =  matchModel(image, model.modelID, model.scoreThresh.D, out _u, out _v, out _angle);
            //u = _u; v = _v; angle = _angle;
            //return matched;
            u = null;
            v = null;
            angle = null;
            try
            {
                HTuple scores = new HTuple();
                // HTuple a = new HTuple(); HTuple b = new HTuple();

                HTuple us = new HTuple(), vs = new HTuple();
                HTuple score;
                HObject cross = null;
                HOperatorSet.GenEmptyObj(out cross);

                HObject updateShowCont = null;
                HOperatorSet.GenEmptyObj(out updateShowCont);
                HTuple updateDefRow = new HTuple(), updateDefCol = new HTuple(), hom2d, iFlag = new HTuple();
                HObject matchRegion = (model.matchRegion != null && model.matchRegion.IsInitialized()) ? model.matchRegion : image;
                //HOperatorSet.FindNccModel(image, modelID, -0.39, 0.78, 0.5, 1, 0.5, "true", 0, out u, out v, out angle, out score);
                //模版句柄
                for (int i = 0; i < model.modelID.Length; i++)
                {
                    updateShowCont.Dispose();

                    HOperatorSet.GenEmptyObj(out model.showContour);
                    //匹配起始角度为-1rad，范围为2rad,阈值为0.5
                    ToolKits.FunctionModule.Vision.find_model(image, matchRegion, model.showContour, out updateShowCont, model.modelType, model.modelID[i], -0.1, 0.2,
                       0.5, 1, model.defRows[i], model.defCols[i], out u, out v, out angle, out score, out updateDefRow, out updateDefCol,
                                        out hom2d, out iFlag);

                    if (iFlag.I == 0)
                    {
                        scores.Append(score);
                        us.Append(updateDefRow);
                        vs.Append(updateDefCol);
                    }
                    else
                    {
                        us.Append(0);
                        vs.Append(0);
                        scores.Append(0);
                        updateShowCont.Dispose();
                        cross.Dispose();
                        return false;
                    }
                }
                HTuple sortInd;
                //排数 从小到大
                HOperatorSet.TupleSortIndex(scores, out sortInd);
                //sortInd ？？？
                HTuple maxInd = sortInd[sortInd.Length - 1];
                score = scores[maxInd];
                bool status = false;
                //double
                if (score.D < 0.01)
                {
                    u = null;
                    v = null;
                    angle = null;
                    return false;
                }
                else
                {
                    u = us[maxInd];
                    v = vs[maxInd];
                    angle = 0;
                    status = true;
                }
                if (this.GrabImageDoneHandler != null && camInd >= 0)
                {
                    //Show _show = new Show();
                    //_show.CamInd = camInd;
                    //_show.Image = image.CopyObj(1, -1);
                    //_show.Mode = 2;
                    //_show.Row = status ? u.D : 0;
                    //_show.Column = status ? v.D : 0;
                    //;
                    //this.GrabImageDoneHandler(null, _show);
                }
                HOperatorSet.GenCrossContourXld(out cross, updateDefRow, updateDefCol, 300, 0);
                //App.mainWindow.ShowImage(frmCalibration.Instance.htWindowCalibration, image, cross);
                if (ImageSrc != null)
                    ImageSrc.Dispose();
                HOperatorSet.GenEmptyObj(out ImageSrc);
                //for (int i = 0; i < imageNum; i++)
                //{
                //    //从acq里取图并保存图片，行号-列号.tiff
                HOperatorSet.SelectObj(image, out ImageSrc, 1);
                ////frmCalibration.Instance.ShowImage(frmCalibration.Instance.htWindowCalibration, image, cross);
                _station._rtUi._fm.ShowImage(_station._rtUi._fm.htWindowCalibration, ImageSrc, cross);
                //HOperatorSet.WriteContourXldDxf(updateShowCont, "D://CONT");
                //htWindow.RefreshWindow(image, cross, "");
                cross.Dispose();
                updateShowCont.Dispose();
                return status;
            }
            catch (HalconException EXP)
            {
                string errMsg = EXP.Message;
                return false;
            }
        }


        /// <summary>
        /// 3个点及其以上经过测试，2个点情况尚未测试
        /// 通过最小二乘方法，计算输出点和输出点之间的映射关系
        /// least square estimate
        /// Out = A* In, solve A from (In,Out), where \hat A = Out*In.T*(In*In.T)^-1
        /// </summary>
        /// <param name="In">输入点，每一列都是一个独立点</param>
        /// <param name="Out">输出点，每一列都是一个独立点</param>
        /// <returns>转换矩阵</returns>
        public Matrix<double> vec2Mat(Matrix<double> In, Matrix<double> Out)
        {
            if (In.ColumnCount == 2)
            //[A] = a[cos(b), sin(b), tx; -sin(b), cos(b), ty]
            //convert from python:
            //dx = x[1]-x[0];dy = y[1]-y[0];dX = X[1]-X[0];dY = Y[1]-Y[0]
            //a = np.linalg.norm([dX, dY], 2)/(np.linalg.norm([dx,dy],2)+0.00000001)
            //b = -np.math.atan2(dY, dX) + np.math.atan2(dy,dx)
            //A11 = a*np.math.cos(b);A12 = a*np.math.sin(b);A21 = - A12;A22 = A11
            //tx = np.mean(X) - A11*np.mean(x) - A12*np.mean(y)
            //ty = np.mean(Y) - A21*np.mean(x) - A22*np.mean(y)
            //return np.array([[A11, A12, tx], [A21, A22, ty]])
            {
                double dx = In[0, 1] - In[0, 0];
                double dy = In[1, 1] - In[1, 0];
                double dX = Out[0, 1] - Out[0, 0];
                double dY = Out[1, 1] - Out[1, 0];
                double a = Math.Sqrt((dY * dY + dX * dX) / (dx * dx + dy * dy + 1e-10));
                double b = -Math.Atan2(dY, dX) + Math.Atan2(dy, dx);
                double A11 = a * Math.Cos(b); double A12 = a * Math.Sin(b); double A21 = -A12; double A22 = A11;
                double tx = (Out[0, 0] + Out[0, 1]) * 0.5 - A11 * (In[0, 0] + In[0, 1]) * 0.5 - A12 * (In[1, 0] + In[1, 1]) * 0.5;
                double ty = (Out[1, 0] + Out[1, 1]) * 0.5 - A21 * (In[0, 0] + In[0, 1]) * 0.5 - A22 * (In[1, 0] + In[1, 1]) * 0.5;
                return Matrix<double>.Build.DenseOfArray(new[,] { { A11, A12, tx }, { A21, A22, ty } });
            }
            else
            //[A] = [A, B, tx; C, D, ty]
            {
                return (Out * In.Transpose()) * (In * In.Transpose()).Inverse();
            }
        }
        #endregion

        #region Camera
        public int ClearImageQueue(string cmrName, out string errMsg)
        {
            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            errMsg = "";

            if (!JFCMFunction.CheckDevCellName(JFCMFunction.Cmr, cmrName, out dev, out ci, out errMsg))
                return (int)ErrorDef.InvokeFailed;

            IJFDevice_Camera cmr = (dev as IJFDevice_Camera);
            int errCode = cmr.ClearBuff();
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = cmr.GetErrorInfo(errCode);
                return errCode;
            }
            return (int)ErrorDef.Success;
        }

        public int CaputreOneImage(string cmrName, string imageType, out HObject hObject, out string errMsg, int timeoutMilSwconds = -1)
        {
            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            IJFImage image = null;
            hObject = null;
            errMsg = "";

            if (!JFCMFunction.CheckDevCellName(JFCMFunction.Cmr, cmrName, out dev, out ci, out errMsg))
                return (int)ErrorDef.InvokeFailed;

            IJFDevice_Camera cmr = (dev as IJFDevice_Camera);
            cmr.ClearBuff();
            int errCode = cmr.GrabOne(out image, timeoutMilSwconds);
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = cmr.GetErrorInfo(errCode);
                return errCode;
            }

            if (GenImgObject(imageType, image, out hObject, out errMsg) != 0)
            {
                return (int)ErrorDef.InvokeFailed;
            }
            return (int)ErrorDef.Success;
        }

        private int GenImgObject(string imageType, IJFImage image, out HObject hObject, out string errMsg)
        {
            hObject = null;
            errMsg = "";
            object ImgObject;
            int errCode = image.GenImgObject(out ImgObject, imageType);
            if (errCode != (int)ErrorDef.Success)
            {
                errMsg = image.GetErrorInfo(errCode);
                return errCode;
            }
            hObject = (HObject)ImgObject;

            return (int)ErrorDef.Success;
        }
        #endregion

        #region Vision
        private delegate void ShowImageDelegate(HTWindowControl htWindow, HObject image, HObject region);
        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="htWindow">图像视窗</param>
        /// <param name="image">图像数据</param>
        /// <param name="region">区域数据</param>
        public void ShowImage(HTWindowControl htWindow, HObject image, HObject region)
        {
            if (htWindow.InvokeRequired)
            {
                htWindow.Invoke(new ShowImageDelegate(ShowImage), new object[] { htWindow, image, region });
            }
            else
            {
                lock (htWindow)
                {
                    htWindow.ColorName = "yellow";
                    htWindow.SetInteractive(false);
                    if (htWindow.Image == null)
                        htWindow.RefreshWindow(image, region, "fit");
                    else
                        htWindow.RefreshWindow(image, region, "fit");
                    htWindow.SetInteractive(true);
                    htWindow.ColorName = "green";
                }
            }
        }
        #endregion

    }
}
