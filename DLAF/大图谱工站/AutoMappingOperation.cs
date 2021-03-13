using JFHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using HalconDotNet;
using HT_Lib;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;
using JFMethodCommonLib;
using HTHalControl;
using System.ComponentModel;

namespace DLAF
{
    public struct RC
    {
        public int r;
        public int c;
    }

    /// <summary>
    /// 单个拍照位图片集合
    /// </summary>
    public class ImageCache
    {
        /// <summary>
        /// 2d图片集合
        /// </summary>
        public HObject _2dImage;
        /// <summary>
        /// 2d图片名
        /// </summary>
        public List<string> _2dImgKeys;
        /// <summary>
        /// 3d图片集合
        /// </summary>
        public HObject _3dImage;
        /// <summary>
        /// 3d图片名
        /// </summary>
        public List<string> _3dImgKeys;
        /// <summary>
        /// 当前拍照位包含的所有die的rc
        /// </summary>
        public List<RC> rc;
        /// <summary>
        /// 所在block
        /// </summary>
        public int b { get; set; }
        /// <summary>
        /// 所在拍照位的行
        /// </summary>
        public int r { get; set; }
        /// <summary>
        /// 所在拍照位的列
        /// </summary>
        public int c { get; set; }
        /// <summary>
        /// 当前拍照位X
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// 当前拍照位Y
        /// </summary>
        public double Y { get; set; }
        public List<HObject> List_3dImage;
        public List<HTuple> List_Z_TrigPos;
        /// <summary>
        /// 当前芯片对应的批次号index
        /// </summary>
        public int LotIndex;
        /// <summary>
        /// 当前芯片对应的Frame号index
        /// </summary>
        public int FrameIndex;
        /// <summary>
        /// 拍摄图存储路径
        /// </summary>
        public string savePath;

        public ImageCache()
        {
            _2dImage = new HObject();
            HOperatorSet.GenEmptyObj(out _2dImage);
            _2dImgKeys = new List<string>();

            _3dImage = new HObject();
            HOperatorSet.GenEmptyObj(out _3dImage);
            _3dImgKeys = new List<string>();
            rc = new List<RC>();
            b = 0;
            r = 0;
            c = 0;
            List_3dImage = new List<HObject>();
            List_Z_TrigPos = new List<HTuple>();
        }
        public ImageCache(int _lot, int _frame, string _path = "")
        {
            _2dImage = new HObject();
            HOperatorSet.GenEmptyObj(out _2dImage);
            _2dImgKeys = new List<string>();

            _3dImage = new HObject();
            HOperatorSet.GenEmptyObj(out _3dImage);
            _3dImgKeys = new List<string>();
            rc = new List<RC>();
            b = 0;
            r = 0;
            c = 0;
            List_3dImage = new List<HObject>();
            List_Z_TrigPos = new List<HTuple>();
            LotIndex = _lot;
            FrameIndex = _frame;
            savePath = _path;
        }

        public void Dispose()
        {
            if (_2dImage != null) _2dImage.Dispose();
            if (_2dImgKeys != null) _2dImgKeys.Clear();
            if (_3dImage != null) _3dImage.Dispose();
            if (_3dImgKeys != null) _3dImgKeys.Clear();
            if (rc != null) rc.Clear();
            b = 0;
            r = 0;
            c = 0;
            if (List_3dImage != null) List_3dImage.Clear(); ;
            if (List_Z_TrigPos != null) List_Z_TrigPos.Clear(); ;


        }
        public void Copy(ImageCache cache)
        {
            if (cache._2dImage != null) HOperatorSet.CopyObj(cache._2dImage, out _2dImage, 1, -1);
            if (cache._2dImgKeys != null) _2dImgKeys = cache._2dImgKeys;
            if (cache._3dImage != null) HOperatorSet.CopyObj(cache._3dImage, out _3dImage, 1, -1);
            if (cache._3dImgKeys != null) _3dImgKeys = cache._3dImgKeys;
            if (cache.rc != null) rc = cache.rc;
            b = cache.b;
            r = cache.r;
            c = cache.c;
            X = cache.X;
            Y = cache.Y;
            if (cache.List_3dImage != null)
            {
                for (int i = 0; i < cache.List_3dImage.Count; i++)
                {
                    HObject obj = null;
                    HOperatorSet.CopyObj(cache.List_3dImage[i], out obj, 1, -1);
                    List_3dImage.Add(obj);
                }
            }
            if (cache.List_Z_TrigPos != null) List_Z_TrigPos = cache.List_Z_TrigPos;
            LotIndex = cache.LotIndex;
            FrameIndex = cache.FrameIndex;
            savePath = cache.savePath;
        }
    }

    public class AutoMappingOperation 
    {
        AutoMappingStation _station;
       
        public AutoMappingOperation()
        {

        }

        public void SetStation(AutoMappingStation station)
        {
            _station = station;
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
            //IniFiles config = new IniFiles(camIniPath);
            //Obj_Camera.Num_Camera = _station.Num_Camera;
            //config.WriteInteger("Camera_Number", "Num_Camera", Obj_Camera.Num_Camera);
            //config.WriteInteger("Camera_Selected", "SelectedIndex", Obj_Camera.SelectedIndex);
            //for (int i = 0; i < Obj_Camera.Num_Camera; i++)
            //{
            //    Obj_Camera obj_cam = obj_camera[i];
            //    int cameraType_No;
            //    cameraType_No = Convert.ToInt32(obj_cam.cameraType);
            //    config.WriteBool("Camera_" + i.ToString(), "isEnable", obj_cam.isEnable);
            //    config.WriteString("Camera_" + i.ToString(), "cameraName", obj_cam.cameraName);
            //    config.WriteString("Camera_" + i.ToString(), "camFile", obj_cam.camFile);
            //    config.WriteInteger("Camera_" + i.ToString(), "cameraType", cameraType_No);
            //    config.Writedouble("Camera_" + i.ToString(), "exposure", obj_cam.exposure);
            //    config.Writedouble("Camera_" + i.ToString(), "gain", obj_cam.gain);
            //    config.WriteBool("Camera_" + i.ToString(), "isCameraTrigger", obj_cam.isCameraTrigger);
            //    config.WriteBool("Camera_" + i.ToString(), "isSoftwareTrigger", obj_cam.isSoftwareTrigger);
            //    config.WriteBool("Camera_" + i.ToString(), "isMirrorX", obj_cam.isMirrorX);
            //    config.WriteBool("Camera_" + i.ToString(), "isMirrorY", obj_cam.isMirrorY);
            //}
        }

        public bool Save()
        {
            Boolean ret = true;
            //try
            //{
            //    sqlCon = new SQLiteConnection(@"DATA SOURCE=" + paraFile + @"; VERSION=3");//改动
            //    if (sqlCon.State == System.Data.ConnectionState.Closed)
            //    {
            //        sqlCon.Open();
            //    }
            //    String sql = "CREATE TABLE IF NOT EXISTS " + paraTable + "(Para TEXT PRIMARY KEY NOT NULL, Value TEXT NOT NULL)";
            //    SQLiteCommand cmd = new SQLiteCommand(sql, sqlCon);
            //    cmd.ExecuteNonQuery();
            //    PropertyInfo[] infos = this.GetType().GetProperties();//type.GetField
            //    foreach (PropertyInfo fi in infos)
            //    {
            //        switch (fi.PropertyType.Name)
            //        {
            //            case "String":
            //            case "Int32":
            //            case "Boolean":
            //            case "double":
            //                cmd.CommandText = String.Format("REPLACE INTO {0}(Para, Value) VALUES(@_Para, @_Value)", paraTable);//1234之类的？
            //                cmd.Parameters.Add("_Para", System.Data.DbType.String).Value = fi.Name;
            //                cmd.Parameters.Add("_Value", System.Data.DbType.Object).Value = fi.GetValue(this);
            //                cmd.ExecuteNonQuery();
            //                break;
            //        }
            //    }
            //    sqlCon.Close();
            //}
            //catch (Exception exp)
            //{
            //    errCode = -1;
            //    errString = exp.ToString();
            //    sqlCon.Close();
            //    ret = false;
            //}
            return ret;
        }

        #region Print
        /// <summary>
        /// 在某位置开始喷码
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="IsX"></param>
        /// <param name="rXorY"></param>
        /// <returns></returns>
        public bool EC_Printer(double x, double y, double z, bool IsX, double rXorY)
        {
            Boolean ret = true;
            //Boolean ret = false;
            //    if (runMode == Convert.ToInt16(SystemRunMode.MODE_OFFLINE))
            //    {
            //        return true;
            //    }

            //    //确认模块ready
            //    if (ready == false)
            //    {
            //        errCode = -1;
            //        errString = "Chuck模块未初始化！";
            //        goto _end;
            //    }
            //    if (App.obj_SystemConfig.Marking && App.obj_Pdt.UseMarker)
            //    {
            //        if (IsHaveInk() == 0)
            //        {
            //            errCode = -1;
            //            errString = "喷印器无墨水！";
            //            goto _end;
            //        }
            //        if (App.obj_Chuck.IsJetRun() == 0)
            //        {
            //            errCode = -1;
            //            errString = "喷印器未启动！";
            //            goto _end;
            //        }
            //    }
            //    else
            //    {
            //        errCode = -1;
            //        errString = "未启用喷印器！";
            //        goto _end;
            //    }
            //    //发送运动指令
            //    if (!XYZ_Move(x, y, z))
            //    {
            //        errString = "Chuck模块三轴运动失败！";
            //        goto _end;
            //    }
            //    //调试
            //    //string msg = "";
            //    //msg += String.Format("\r\n");
            //    //msg += String.Format("{0},{1},{2},{3},{4},\r\n", x, y, z, rXorY, IsX);
            //    //File.AppendAllText("D:\\T2.csv", msg);
            //    App.ec_Jet.TriggerPrint();
            //    if (IsX)
            //    {
            //        if (!X_Move(x + rXorY))
            //        {
            //            errString = "Chuck模块三轴运动失败！";
            //            goto _end;
            //        }
            //        if (!X_Move(x))
            //        {
            //            errString = "Chuck模块三轴运动失败！";
            //            goto _end;
            //        }
            //        //if (!X_Move(x - rXorY))
            //        //{
            //        //    errString = "Chuck模块三轴运动失败！";
            //        //    goto _end;
            //        //}
            //    }
            //    else
            //    {
            //        if (!Y_Move(y + rXorY))
            //        {
            //            errString = "Chuck模块三轴运动失败！";
            //            goto _end;
            //        }
            //        if (!Y_Move(y))
            //        {
            //            errString = "Chuck模块三轴运动失败！";
            //            goto _end;
            //        }
            //        //if (!Y_Move(y - rXorY))
            //        //{
            //        //    errString = "Chuck模块三轴运动失败！";
            //        //    goto _end;
            //        //}
            //    }
            //    ret = true;
            //_end:
            return ret;
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
                        htWindow.RefreshWindow(image, region, "zoom");
                    else
                        htWindow.RefreshWindow(image, region, "zoom");
                    htWindow.SetInteractive(true);
                    htWindow.ColorName = "green";
                    htWindow.Refresh();
                }
            }
        }

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="htWindow">图像视窗</param>
        /// <param name="image">图像数据</param>
        /// <param name="region">区域数据</param>
        public void ShowImageEx(HTWindowControl htWindow, HObject image, HObject region)
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
                        htWindow.RefreshWindow(image, region, "NULL");
                    else
                        htWindow.RefreshWindow(image, region, "NULL");
                    htWindow.SetInteractive(true);
                    htWindow.ColorName = "green";
                    htWindow.Refresh();
                }
            }
        }

        /// <summary>
        /// 到指定位置拍照
        /// </summary>
        /// <param name="x">相机X坐标</param>
        /// <param name="y">相机Y坐标</param>
        /// <param name="z">相机Z坐标</param>
        /// <param name="htWindow">图像视窗</param>
        /// <param name="img">拍到的图像</param>
        public void SnapPos(double x, double y, double z, HTHalControl.HTWindowControl htWindow, out HObject img)
        {
            string errMsg = "";
            img = null;
            if (MultiAxisMove(new string[] { "X","Y","Z"},new double[] { x,y,z},true,out errMsg)!=0)
            {
                HTUi.PopError(errMsg);
                return;
            }
            if (_station._RunMode == 1) return;

            //3. 触发
            //if(SWPosTrig(new string[] { "X"},out errMsg)!=0)
            //{
            //    HTUi.PopError(errMsg);
            //    return;
            //}
            //4. 取图
            if (_station.operation.CaputreOneImage(_station.CamereDev[_station.SelectedIndex], "Halcon", out img, out errMsg) != 0)
            {
                HTUi.PopError("采集图像失败：" + errMsg);
                return;
            }
           
            ShowImage(htWindow, img, null);
        }

        public static HTuple CopyModel(HTuple modelId, HTuple modelType)
        {
            HTuple modelID = null;
            switch (modelType.I)
            {
                case 0:
                    modelID = CopyNccModel(modelId);
                    break;
                case 1:
                    modelID = CopyShapeModel(modelId);
                    break;
            }
            return modelID;
        }

        public static HTuple CopyShapeModel(HTuple shapeModelId)
        {
            HTuple copyShapeModeId = new HTuple();
            HTuple serializedItemHandle = new HTuple();
            HOperatorSet.SerializeShapeModel(shapeModelId, out serializedItemHandle);
            HOperatorSet.DeserializeShapeModel(serializedItemHandle, out copyShapeModeId);
            return copyShapeModeId;
        }
        //model类Copy,需要序列化和反序列化的过程
        public static HTuple CopyNccModel(HTuple nccModelId)
        {
            HTuple copyNccModeId = new HTuple();
            HTuple serializedItemHandle = new HTuple();
            HOperatorSet.SerializeNccModel(nccModelId, out serializedItemHandle);
            HOperatorSet.DeserializeNccModel(serializedItemHandle, out copyNccModeId);
            return copyNccModeId;
        }

        /// <summary>
        /// 获取大芯片中心的扫描点位（IC的中心为相机视野中心）
        /// </summary>
        /// <param name="hv_width"></param>
        /// <param name="hv_height"></param>
        /// <param name="hv_uvHxy"></param>
        /// <param name="hv_widthFactor"></param>
        /// <param name="hv_heightFactor"></param>
        /// <param name="hv_zoomFactor"></param>
        /// <param name="hv_mapRowCnt"></param>
        /// <param name="hv_mapColCnt"></param>
        /// <param name="hv_clipmapX"></param>
        /// <param name="hv_clipmapY"></param>
        /// <param name="hv_clipmapRow"></param>
        /// <param name="hv_clipmapCol"></param>
        /// <param name="hv_snapX"></param>
        /// <param name="hv_snapY"></param>
        /// <param name="hv_snapRow"></param>
        /// <param name="hv_snapCol"></param>
        /// <param name="hv_iFlag"></param>
        public void get_scan_points(HTuple hv_width, HTuple hv_height, HTuple hv_uvHxy, HTuple hv_zoomFactor, HTuple hv_mapRowCnt, HTuple hv_mapColCnt, HTuple hv_clipmapX,
            HTuple hv_clipmapY, HTuple hv_clipmapRow, HTuple hv_clipmapCol, out HTuple hv_snapX, out HTuple hv_snapY, out HTuple hv_snapRow, out HTuple hv_snapCol, out HTuple hv_iFlag)
        {
            
            hv_iFlag = "";
            hv_snapX = new HTuple();
            hv_snapY = new HTuple();
            hv_snapRow = new HTuple();
            hv_snapCol = new HTuple();

            HTuple hv_uvHxyScaled = new HTuple();

            try
            {
                //HOperatorSet.HomMat2dScale(hv_uvHxy, 1 /*/ hv_zoomFactor*/, 1 /*/ hv_zoomFactor*/, 0, 0, out hv_uvHxyScaled);

                //hv_snapX = hv_clipmapX + ((hv_uvHxyScaled.TupleSelect(0) * (-1) * (hv_height - 1)) / 2.0) 
                //    + ((hv_uvHxyScaled.TupleSelect(1) * (-1) * (hv_width - 1)) / 2.0);

                //hv_snapY = hv_clipmapY + ((hv_uvHxyScaled.TupleSelect(3) * (-1) * (hv_height - 1)) / 2.0)
                //    + ((hv_uvHxyScaled.TupleSelect(4) * (-1) * (hv_width - 1)) / 2.0);

                //*****将扫描点位按弓字形排序
                HTuple hv_seqInd = new HTuple();
                HTuple hv_rowIndConst = new HTuple();
                HTuple hv_colIndSeq = new HTuple();
                HTuple hv__x = new HTuple();
                HTuple hv__y = new HTuple();
                HTuple hv_i = 0;
                for (hv_i = 0; hv_i<hv_mapRowCnt; hv_i++)
                {
                    HOperatorSet.TupleGenSequence(hv_i * hv_mapColCnt, ((hv_i + 1) * hv_mapColCnt) - 1,
                        1, out hv_seqInd);
                    HOperatorSet.TupleGenConst(hv_mapColCnt, hv_i, out hv_rowIndConst);
                    HOperatorSet.TupleGenSequence(0, hv_mapColCnt - 1, 1, out hv_colIndSeq);
                    if ((int)(new HTuple(((hv_i % 2)).TupleEqual(0))) != 0)
                    {
                        hv__x = hv_clipmapX.TupleSelect(hv_seqInd);
                        hv__y = hv_clipmapY.TupleSelect(hv_seqInd);
                    }
                    else
                    {
                        hv__x = ((hv_clipmapX.TupleSelect(hv_seqInd))).TupleInverse();
                        hv__y = ((hv_clipmapY.TupleSelect(hv_seqInd))).TupleInverse();
                        hv_colIndSeq = hv_colIndSeq.TupleInverse();
                    }
                    hv_snapX = hv_snapX.TupleConcat(hv__x);
                    hv_snapY = hv_snapY.TupleConcat(hv__y);
                    hv_snapRow = hv_snapRow.TupleConcat(hv_rowIndConst);
                    hv_snapCol = hv_snapCol.TupleConcat(hv_colIndSeq);
                }

            }
            catch (HalconException HDevExpDefaultException)
            {
                HTuple hv_Exception = new HTuple();
                HDevExpDefaultException.ToHTuple(out hv_Exception);
                VisionMethonDll.VisionMethon.GetErrInfo(hv_Exception, out hv_iFlag);
            }
        }

        public void Get_ICRegion_points(HTuple hv_width, HTuple hv_height,HTuple hv_Row,HTuple hv_Col, HTuple hv_uvHxy, HTuple hv_zoomFactor,
            out HTuple hv_icXScan,out HTuple hv_icYScan,out HTuple hv_iFlag)
        {
            hv_iFlag = "";
            hv_icXScan = new HTuple();
            hv_icYScan = new HTuple();

            HTuple hv_uvHxyScaled = new HTuple();
            try
            {
                HOperatorSet.HomMat2dScale(hv_uvHxy, 1 / hv_zoomFactor, 1 / hv_zoomFactor, 0, 0, out hv_uvHxyScaled);

                hv_icXScan = (hv_uvHxyScaled.TupleSelect(0) * ((hv_height - 1) / 2.0-hv_Row))
                    + (hv_uvHxyScaled.TupleSelect(1) * ((hv_width - 1) / 2.0-hv_Col));

                hv_icYScan = (hv_uvHxyScaled.TupleSelect(3) * ((hv_height - 1) / 2.0 - hv_Row))
                    + (hv_uvHxyScaled.TupleSelect(4) * ((hv_width - 1) / 2.0 - hv_Col));
            }
            catch (HalconException HDevExpDefaultException)
            {
                HTuple hv_Exception = new HTuple();
                HDevExpDefaultException.ToHTuple(out hv_Exception);
                VisionMethonDll.VisionMethon.GetErrInfo(hv_Exception, out hv_iFlag);
            }
        }
        #endregion

        #region Motion
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

        /// <summary>
        /// 多轴运动(非插补运动)
        /// </summary>
        /// <param name="AxisName"></param>
        /// <param name="distance"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int MultiAxisMove(string[] AxisName, double[] distance, bool isAbs, out string errMsg)
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
                if (!_station.MoveAxis(AxisName[i], distance[i], isAbs, out errMsg))
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
        #endregion

        #region Camera
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
    }
}
