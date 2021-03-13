using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LFAOIReview
{
    public class Report
    {
        public static string ImageTempSaveDirectory { get; set; } = System.Windows.Forms.Application.StartupPath + "\\ReportImageTempFile";


        public static event Action<string> OnShowMessage;
        private static void ShowMessage(string message)
        {
            OnShowMessage?.Invoke(message);
        }

        /// <summary>
        /// 根据错误码优先级对错误进行输出，当错误优先级设置为一样时可以输出多个错误码
        /// </summary>
        /// <param name="SToM"></param>单个芯片 多个不重复的错误码
        /// <param name="DefectPriority"></param>芯片错误码优先级
        /// <param name="SToS"></param> 单个芯片根据优先级输出错误码 
        public static void TranDefect(int[] SToM,Dictionary<int,int[]> DefectPriority, out HTuple SToS)
        {
            List<int []> tempPriority = new List<int []>();
            List<int> tempIndex = new List<int>();
            SToS  = new HTuple();
            for (int i =0;i<SToM.Length;i++)
            {
               if (DefectPriority.ContainsKey(SToM[i]))
                {
                    tempPriority.Add(DefectPriority[SToM[i]]);
                    tempIndex.Add(DefectPriority[SToM[i]][0]);

                }
                else
                {
                    SToS = -1;
                    return;
                }
            }
            //
            for(int ind = 0;ind< tempIndex.Count; ind++)
            {
                if (tempPriority[ind][0] == tempIndex.Min())
                {
                    SToS.Append(tempPriority[ind][1]);
                }
            }
            
        }

        public static void GenerateExcel(string dbFilePath, int lotIndex, string imageDirectory, string excelDirectory, int ReportModel,
                                         bool isChinese = true)
        {
            ShowMessage("正在读取数据...");
            LotInfoDb lotInfoDb;
            List<DefectTypeInfoReport> list_DefectTypeInfoReport;
            List<FrameIndexDb> list_FrameIndexDb;
            List<InspectionDataReport> list_InspectionDataReport;
            List<DefectDataReport> list_DefectDataReport;
            List<ReviewEditData> list_ReciewEditData;
            SQLiteOperation.ReadDataBase(dbFilePath,
                                         lotIndex,
                                         ReportModel,
                                         out lotInfoDb,
                                         out list_DefectTypeInfoReport,
                                         out list_FrameIndexDb,
                                         out list_InspectionDataReport,
                                         out list_DefectDataReport,
                                         out list_ReciewEditData);
            List<Matrix<InspectionDataReport>> list_InspectionDataReportMatrix = new List<Matrix<InspectionDataReport>>();
            if (list_FrameIndexDb.Count == 0)
            {
                ShowMessage("该批次没有数据，生成结束");
                return;
            }
            foreach (ReviewEditData item in list_ReciewEditData)
            {
                int dataIndex = (item.FrameLotIndex - 1) * lotInfoDb.RowCount * lotInfoDb.ColumnCount + item.Row * lotInfoDb.ColumnCount + item.Col;
                InspectionDataReport inspectionData = list_InspectionDataReport.Find(t => t.DbIndex == dataIndex);
                if (inspectionData != null)
                {
                    inspectionData.ReviewEditNG = "OK2NG";
                }
                
            }

            for (int i = 0; i < list_FrameIndexDb.Count; i++)
            {
                Matrix<InspectionDataReport> inpectionDataReportMatrix = new Matrix<InspectionDataReport>(lotInfoDb.RowCount, lotInfoDb.ColumnCount);
                inpectionDataReportMatrix.FrameName = list_FrameIndexDb[i].FrameName;
                list_InspectionDataReportMatrix.Add(inpectionDataReportMatrix);
            }

            int passCount = 0;
            int failCount = 0;
            int falsecallCount = 0;
            int skipCount = 0;
            int CodeNumber = 0;
            int NoCodeNumber = 0;
            int OK2NG = 0;

            //总图数量
            int generalImageCount = 0;

            foreach (InspectionDataReport dataReport in list_InspectionDataReport)
            {
                list_InspectionDataReportMatrix[dataReport.FrameIndex - 1][dataReport.RowIndex, dataReport.ColumnIndex] = dataReport;
                switch (dataReport.InspectionResult)
                {
                    case InspectionResult.OK:
                        passCount++;
                        break;
                    case InspectionResult.NG:
                        failCount++;
                        break;
                    case InspectionResult.N2K:
                        falsecallCount++;
                        break;
                    case InspectionResult.K2N:
                        OK2NG++;
                        break;
                    case InspectionResult.SKIP:
                        skipCount++;
                        break;
                }
                if (dataReport.Code2D == "null")
                {
                    NoCodeNumber++;
                }
                else
                {
                    CodeNumber++;
                }

                generalImageCount += dataReport.ConcatImagePath.Count();
            }
            if (Directory.Exists(ImageTempSaveDirectory))
            {
                Directory.Delete(ImageTempSaveDirectory, true);
            }
            Directory.CreateDirectory(ImageTempSaveDirectory);
            Thread.Sleep(200);

            ShowMessage("正在处理图像...");
            int defectCount = list_DefectDataReport.Count;
            HWindowControl hWindowControl = new HWindowControl();
            double imageOperationProgressPercent = 0;
            int iCount = 0;
            int ImageCount = 0;
            //int totalImageCount = defectCount + generalImageCount;
            //计算NG数、K2N数、N2K 的和 与 单个视野拍照次数的乘积 为总共图片数量
            int totalImageCount = (OK2NG + failCount+ falsecallCount) * list_InspectionDataReport[0].ConcatImagePath.Count;
            //为每一个芯片添加 错误列表 
            for (; iCount < defectCount; iCount++)
            {
                InspectionDataReport dataReport = list_InspectionDataReportMatrix[list_DefectDataReport[iCount].FrameIndex - 1][list_DefectDataReport[iCount].RowIndex, list_DefectDataReport[iCount].ColumnIndex];
                if (!dataReport.List_DefectData.Exists(d => d.DefectTypeIndex == list_DefectDataReport[iCount].DefectTypeIndex))
                {   if (list_DefectDataReport[iCount].DefectTypeIndex <= 0 || list_DefectDataReport[iCount].DefectTypeIndex > list_DefectTypeInfoReport.Count)
                    {
                        ShowMessage(String.Format("Error:存在错误码不再设定范围的错误:{0}\n芯片位置Row:{1},Column:{2}", list_DefectDataReport[iCount].DefectTypeIndex, list_DefectDataReport[iCount].RowIndex, list_DefectDataReport[iCount].ColumnIndex));
                    }
                    else
                    {
                        list_DefectTypeInfoReport.Where(d => d.Index == list_DefectDataReport[iCount].DefectTypeIndex).FirstOrDefault().Count++;
                    }
                }
                dataReport.List_DefectData.Add(list_DefectDataReport[iCount]);
            }


            #region 生成每一个错误图片 屏蔽
            //for (; iCount < defectCount; iCount++)
            //{
            //    InspectionDataReport dataReport = list_InspectionDataReportMatrix[list_DefectDataReport[iCount].FrameIndex - 1][list_DefectDataReport[iCount].RowIndex, list_DefectDataReport[iCount].ColumnIndex];
            //    if (!dataReport.List_DefectData.Exists(d=>d.DefectTypeIndex == list_DefectDataReport[iCount].DefectTypeIndex))
            //    {
            //        list_DefectTypeInfoReport.Where(d => d.Index == list_DefectDataReport[iCount].DefectTypeIndex).FirstOrDefault().Count++;
            //    }
            //    dataReport.List_DefectData.Add(list_DefectDataReport[iCount]);
            //    string imagePath = imageDirectory + dataReport.ConcatImagePath[list_DefectDataReport[iCount].ConcatImageIndex];
            //    if (!File.Exists(imagePath))
            //    {
            //        continue;
            //    }
            //    HObject concatImage;
            //    HOperatorSet.GenEmptyObj(out concatImage);
            //    HOperatorSet.ReadImage(out concatImage, imagePath);
            //    HTuple width, height;
            //    HOperatorSet.GetImageSize(concatImage, out width, out height);
            //    //hWindowControl.Height = height;
            //    //hWindowControl.Width = width;
            //    double scale = width / height;
            //    hWindowControl.Height = 300;
            //    hWindowControl.Width = (int)(hWindowControl.Height * scale);
            //    HOperatorSet.SetPart(hWindowControl.HalconWindow, 0, 0, height - 1, width - 1);
            //    HOperatorSet.DispObj(concatImage, hWindowControl.HalconWindow);
            //    concatImage.Dispose();

            //    string regionPath = imageDirectory + dataReport.ConcatRegionPath[list_DefectDataReport[iCount].ConcatRegionIndex];
            //    if (!File.Exists(regionPath))
            //    {
            //        continue;
            //    }
            //    HObject concatRegion;
            //    HOperatorSet.ReadRegion(out concatRegion, regionPath);
            //    HObject region = concatRegion.SelectObj(list_DefectDataReport[iCount].RegionIndex);
            //    HOperatorSet.SetColor(hWindowControl.HalconWindow, "yellow");
            //    HOperatorSet.SetDraw(hWindowControl.HalconWindow, "margin");
            //    HOperatorSet.DispRegion(region, hWindowControl.HalconWindow);
            //    concatRegion.Dispose();
            //    region.Dispose();

            //    string wirePath = imageDirectory + dataReport.WirePath[list_DefectDataReport[iCount].ConcatImageIndex];
            //    if (!File.Exists(wirePath))
            //    {
            //        continue;
            //    }
            //    HObject wire;
            //    HOperatorSet.GenEmptyObj(out wire);
            //    HTuple tempTuple;
            //    HOperatorSet.ReadContourXldDxf(out wire, wirePath, new HTuple(), new HTuple(), out tempTuple);
            //    HOperatorSet.SetColor(hWindowControl.HalconWindow, "green");

            //    HOperatorSet.DispObj(wire, hWindowControl.HalconWindow);
            //    wire.Dispose();


            //    string color = list_DefectDataReport[iCount].Result == 0 ? "red" : "orange";

            //    HalconOperation.DisplayMessage(hWindowControl.HalconWindow, string.Format("{0} - {1} 缺陷 {2}", list_DefectDataReport[iCount].RowIndex + 1, list_DefectDataReport[iCount].ColumnIndex + 1, list_DefectDataReport[iCount].DefectTypeIndex),
            //                                   "window", 12, 12, color, "true");

            //    HObject imageToSave;
            //    HOperatorSet.DumpWindowImage(out imageToSave, hWindowControl.HalconWindow);
            //    string imageTempPath = string.Format("{0}\\{1}.jpg", ImageTempSaveDirectory, iCount);
            //    HOperatorSet.WriteImage(imageToSave, "jpg", 0, imageTempPath);
            //    list_DefectDataReport[iCount].ImageTempPath = imageTempPath;

            //    if (iCount >= (int)(imageOperationProgressPercent * totalImageCount))
            //    {
            //        ShowMessage(string.Format("已经完成 {0} / {1}", iCount + 1, totalImageCount));
            //        while (iCount >= (int)(imageOperationProgressPercent * totalImageCount))
            //        {
            //            imageOperationProgressPercent += 0.01;
            //        }
            //    }
            //}
            #endregion
            // SingleDefect singleDefect = new SingleDefect();

            DefectStatistics defectStatistics = new DefectStatistics();
            
            //生成单个芯片的总图
            foreach (Matrix<InspectionDataReport> inspectionDataReportMatrix in list_InspectionDataReportMatrix)
            {
                if (inspectionDataReportMatrix == null) continue;
                foreach (InspectionDataReport dataReport in inspectionDataReportMatrix)
                {
                    if (dataReport == null) continue;

                    //对OK复看成NG的 K2N 单独生成图片 List_
                    if (dataReport.InspectionResult == InspectionResult.K2N)
                    {
                        for (int j = 0; j < dataReport.ConcatImagePath.Count; j++)
                        {
                            string imagePath = imageDirectory + dataReport.ConcatImagePath[j];
                            if (!File.Exists(imagePath))
                            {
                                continue;
                            }
                            HObject concatImage;
                            HOperatorSet.GenEmptyObj(out concatImage);
                            HOperatorSet.ReadImage(out concatImage, imagePath);
                            HTuple width, height;
                            HOperatorSet.GetImageSize(concatImage, out width, out height);
                            double scale = (double)width.TupleSelect(0).D / height.TupleSelect(0).D;
                            hWindowControl.Height = 600;
                            hWindowControl.Width = (int)(hWindowControl.Height * scale);
                            HOperatorSet.SetPart(hWindowControl.HalconWindow, 0, 0, height.TupleSelect(0).D - 1, width.TupleSelect(0).D - 1);

                            //2020.12.06 
                            List<int> imageIndexs = new List<int>();
                            foreach (var defectReport in dataReport.List_DefectData)
                            {
                                if (!imageIndexs.Contains(defectReport.ImageIndex))
                                {
                                    imageIndexs.Add(defectReport.ImageIndex);
                                }
                            }

                            for (int m = 0; m < imageIndexs.Count; m++)
                            {
                                List<DefectDataReport> defectReportList = dataReport.List_DefectData.FindAll(delegate (DefectDataReport s) { return s.ImageIndex == imageIndexs[m]; });
                                if (defectReportList.Count == 0) continue;
                                DefectDataReport defectReport = defectReportList[0];

                                HObject channelImage = concatImage.SelectObj(defectReport.ImageIndex);
                                HOperatorSet.DispObj(channelImage, hWindowControl.HalconWindow);
                                channelImage.Dispose();

                                HalconOperation.DisplayMessage(hWindowControl.HalconWindow, string.Format("{0} - {1} - {2}复看不合格", dataReport.RowIndex + 1, dataReport.ColumnIndex + 1, defectReport.ImageIndex),
                                                               "window", 12, 12, "red", "true");
                                HObject imageToSave;
                                HOperatorSet.DumpWindowImage(out imageToSave, hWindowControl.HalconWindow);
                                string imageTempPath = string.Format("{0}\\{1}_{2}.jpg", ImageTempSaveDirectory, ImageCount, defectReport.ImageIndex);
                                HOperatorSet.WriteImage(imageToSave, "jpg", 0, imageTempPath);
                                dataReport.List_GeneralImageTempPath.Add(imageTempPath);
                                imageToSave.Dispose();
                            }
                            concatImage.Dispose();


                            if (ImageCount >= (int)(imageOperationProgressPercent * totalImageCount))
                            {
                                ShowMessage(string.Format("已经完成 {0} / {1}", ImageCount + 1, totalImageCount));
                                while (ImageCount >= (int)(imageOperationProgressPercent * totalImageCount))
                                {
                                    imageOperationProgressPercent += 0.1;
                                }
                            }
                            ImageCount++;
                        }
                        continue;
                    }

                    if (dataReport.List_DefectData.Count == 0) continue;


                    //计算复看合格的错误码个数
                    if (dataReport.InspectionResult == InspectionResult.N2K)
                    {
                        if (defectStatistics.Flag)
                        {
                            int[] singleToMDefect = dataReport.List_DefectData.Select(d => d.DefectTypeIndex).Distinct().ToArray();
                            TranDefect(singleToMDefect, defectStatistics.DefectPriority, out HTuple SToS);
                            if (SToS == -1)
                            {
                                defectStatistics.Flag = false;
                            }
                            else
                            {
                                if (SToS.Length == 1)
                                {
                                    if (defectStatistics.CountN2KDefectResult.ContainsKey(SToS[0].I))
                                    {
                                        defectStatistics.CountN2KDefectResult[SToS[0].I]++;
                                    }
                                    else
                                    {
                                        defectStatistics.CountN2KDefectResult.Add(SToS[0].I, 1);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < SToS.Length; i++)
                                    {
                                        if (defectStatistics.CountN2KDefectResult.ContainsKey(SToS[i].I))
                                        {
                                            defectStatistics.CountN2KDefectResult[SToS[i].I]++;
                                        }
                                        else
                                        {
                                            defectStatistics.CountN2KDefectResult.Add(SToS[i].I, 1);
                                        }
                                    }
                                }

                            }
                        }
                    }
                     //单个芯片 多个不重复错误码
                    if (defectStatistics.Flag)
                    {
                        int[] singleToMDefect = dataReport.List_DefectData.Select(d => d.DefectTypeIndex).Distinct().ToArray();

                        //如果存在 没有设定过的错误码 直接continue 和defectStatistics.Flag功能重复了 未修改
                        bool flag = false;
                        foreach (int t in singleToMDefect)
                        {
                            if (t <= 0 || t > list_DefectTypeInfoReport.Count)
                            {
                                defectStatistics.List_RepeatPriority.Add(t);
                                flag = true;
                                break;
                            }
                        }
                        if(flag)
                        {

                        }
                        else
                        {
                            TranDefect(singleToMDefect, defectStatistics.DefectPriority, out HTuple SToS);

                            //singleDefect.row = dataReport.RowIndex;
                            //singleDefect.col = dataReport.ColumnIndex;
                            //singleDefect.SDefectType = SToS;
                            //defectStatistics.List_SingleDefectType.Add(singleDefect);

                            if (SToS == -1)
                            {
                                defectStatistics.Flag = false;
                                //continue;
                            }
                            else
                            {
                                // 根据优先级输出的错误码
                                for (int i = 0; i < SToS.Length; i++)
                                {
                                    dataReport.Priority_DetectType.Add(SToS[i].I);
                                }

                                if (SToS.Length == 1)
                                {
                                    if (defectStatistics.CountDefectResult.ContainsKey(SToS[0].I))
                                    {
                                        defectStatistics.CountDefectResult[SToS[0].I]++;
                                    }
                                    else
                                    {
                                        defectStatistics.CountDefectResult.Add(SToS[0].I, 1);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < SToS.Length; i++)
                                    {
                                        if (defectStatistics.CountDefectResult.ContainsKey(SToS[i].I))
                                        {
                                            defectStatistics.CountDefectResult[SToS[i].I]++;
                                        }
                                        else
                                        {
                                            defectStatistics.CountDefectResult.Add(SToS[i].I, 1);
                                        }
                                    }
                                    defectStatistics.List_RepeatPriority.Add(SToS);
                                }
                            }
                        }
                        


                    }

                    //对NG图片生成总图
                    for (int j = 0; j < dataReport.ConcatImagePath.Count; j++)
                    {
                        string imagePath = imageDirectory + dataReport.ConcatImagePath[j];
                        if (!File.Exists(imagePath))
                        {
                            continue;
                        }
                        HObject concatImage;
                        HOperatorSet.GenEmptyObj(out concatImage);
                        HOperatorSet.ReadImage(out concatImage, imagePath);
                        HTuple width, height;
                        HOperatorSet.GetImageSize(concatImage, out width, out height);
                        double scale = (double)width.TupleSelect(0).D / height.TupleSelect(0).D;
                        hWindowControl.Height = 600;
                        hWindowControl.Width = (int)(hWindowControl.Height * scale);
                        HOperatorSet.SetPart(hWindowControl.HalconWindow, 0, 0, height.TupleSelect(0).D - 1, width.TupleSelect(0).D - 1);

                        //2020.12.06 遍历List_DefectData，导出检出缺陷的图层序号
                        List<int> imageIndexs = new List<int>();
                        foreach (var defectReport in dataReport.List_DefectData)
                        {
                            if(!imageIndexs.Contains(defectReport.ImageIndex))
                            {
                                imageIndexs.Add(defectReport.ImageIndex);
                            }
                        }

                        //2020.12.06 加载导出的所有图层，并在该图层上display所有检出的Region
                        for (int m = 0; m < imageIndexs.Count; m++)
                        {
                            List<DefectDataReport> defectReportList = dataReport.List_DefectData.FindAll(delegate (DefectDataReport s) { return s.ImageIndex == imageIndexs[m]; });
                            if(defectReportList.Count==0) continue;
                            DefectDataReport defectReport = defectReportList[0];
                            if (defectReport.ConcatImageIndex != j) continue;

                            HObject channelImage = concatImage.SelectObj(defectReport.ImageIndex);
                            HOperatorSet.DispObj(channelImage, hWindowControl.HalconWindow);
                            channelImage.Dispose();

                            string regionPath = imageDirectory + dataReport.ConcatRegionPath[defectReport.ConcatRegionIndex];
                            if (!File.Exists(regionPath))
                            {
                                continue;
                            }
                            HObject concatRegion;
                            HOperatorSet.ReadRegion(out concatRegion, regionPath);
                            HObject showRegion=null;

                            //concat所有List_DefectData图片Index一致Region
                            for (int n = 0; n < defectReportList.Count; n++)
                            {
                                HObject region = concatRegion.SelectObj(defectReportList[n].RegionIndex);
                                if (showRegion == null)
                                    showRegion = region.Clone();
                                else
                                    HOperatorSet.ConcatObj(showRegion, region, out showRegion);
                                region.Dispose();
                            }
                            HOperatorSet.SetColor(hWindowControl.HalconWindow, "yellow");
                            HOperatorSet.SetDraw(hWindowControl.HalconWindow, "margin");
                            HOperatorSet.DispRegion(showRegion, hWindowControl.HalconWindow);
                            concatRegion.Dispose();
                            showRegion.Dispose();

                            string wirePath = imageDirectory + dataReport.WirePath[j];
                            if (!File.Exists(wirePath))
                            {
                                continue;
                            }
                            HObject wire;
                            HOperatorSet.GenEmptyObj(out wire);
                            HTuple tempTuple;
                            HOperatorSet.ReadRegion(out wire, wirePath);
                            //HOperatorSet.ReadContourXldDxf(out wire, wirePath, new HTuple(), new HTuple(), out tempTuple);
                            HOperatorSet.SetColor(hWindowControl.HalconWindow, "green");
                            HObject channelwire = wire.SelectObj(defectReport.ImageIndex);
                            HOperatorSet.DispObj(channelwire, hWindowControl.HalconWindow);
                            wire.Dispose();
                            channelwire.Dispose();


                            HalconOperation.DisplayMessage(hWindowControl.HalconWindow, string.Format("{0} - {1} - {2}", dataReport.RowIndex + 1, dataReport.ColumnIndex + 1, defectReport.ImageIndex),
                                                           "window", 12, 12, "red", "true");

                            HObject imageToSave;
                            HOperatorSet.DumpWindowImage(out imageToSave, hWindowControl.HalconWindow);
                            string imageTempPath = string.Format("{0}\\{1}_{2}.jpg", ImageTempSaveDirectory, ImageCount, defectReport.ImageIndex);
                            HOperatorSet.WriteImage(imageToSave, "jpg", 0, imageTempPath);
                            dataReport.List_GeneralImageTempPath.Add(imageTempPath);
                            imageToSave.Dispose();
                        }
                        concatImage.Dispose();


                        if (ImageCount >= (int)(imageOperationProgressPercent * totalImageCount))
                        {
                            ShowMessage(string.Format("已经完成 {0} / {1}", ImageCount + 1, totalImageCount));
                            while (ImageCount >= (int)(imageOperationProgressPercent * totalImageCount))
                            {
                                imageOperationProgressPercent += 0.1;
                            }
                        }
                        ImageCount++;
                    }
                }
            }

            
            ShowMessage("正在生成表格...");
            SummaryInfo summaryInfo = new SummaryInfo();
            //1、产品编号
            summaryInfo.ProductCode = lotInfoDb.ProductCode;
            //2、批次号
            summaryInfo.Lot = lotInfoDb.LotName;

            //3、设备号
            summaryInfo.Machine = lotInfoDb.Machine;
            //4、操作员
            summaryInfo.AIOperator = lotInfoDb.Operator;
            //5、开始日期
            summaryInfo.StartDate = lotInfoDb.StartDate;
            //6、开始时间
            summaryInfo.StartTime = lotInfoDb.StartTime;
            //7、结束日期
            summaryInfo.EndDate = lotInfoDb.EndDate;
            //8、结束时间
            summaryInfo.EndTime = lotInfoDb.EndTime;


            //9、批次理论盘数
            summaryInfo.TotalNumberOfStrips = lotInfoDb.TotalFrameCount;
            //10、批次检测过的盘数
            summaryInfo.NumberOfStripsInspected = list_FrameIndexDb.Count;
            //11、批次未检测的盘数
            if (summaryInfo.TotalNumberOfStrips != 0)
            {
                summaryInfo.NumberOfStripsNotInspected = summaryInfo.TotalNumberOfStrips - summaryInfo.NumberOfStripsInspected;
            }
            //12、单盘芯片数
            summaryInfo.QuantityOfDevicesPerStrip = lotInfoDb.RowCount * lotInfoDb.ColumnCount;
            //14、批次理论芯片数
            if (summaryInfo.TotalNumberOfStrips != 0)
            {
                summaryInfo.NumberOfStartQuantity = summaryInfo.TotalNumberOfStrips * summaryInfo.QuantityOfDevicesPerStrip;
            }
            summaryInfo.NumberOfDevicesInspected = list_InspectionDataReport.Count;
            //批次实际芯片数

            try
            {
                DateTime startTime = Convert.ToDateTime(lotInfoDb.StartDate + " " + lotInfoDb.StartTime);
                DateTime endTime = Convert.ToDateTime(lotInfoDb.EndDate + " " + lotInfoDb.EndTime);
                TimeSpan timDuring = endTime - startTime;
                double hours = timDuring.TotalHours;
                //15、每小时检测芯片数
                summaryInfo.DevicesPerHour = summaryInfo.NumberOfDevicesInspected / hours;
            }
            catch { }

            //16、误判率
            summaryInfo.FalseCallDevicePercent = falsecallCount * 1.0 / summaryInfo.NumberOfDevicesInspected * 100;
            //17、误判芯片数
            summaryInfo.NumberOfDevicesFalseCalled = falsecallCount;
            //18、合格芯片数
            summaryInfo.NumberOfDevicesPassed = passCount;
            //19、不合格芯片数
            summaryInfo.NumberOfDevicesRejected = failCount;
            //20、合格率
            summaryInfo.YieldByDevice = passCount / (double)summaryInfo.NumberOfDevicesInspected * 100;
            //21、跳过芯片数
            summaryInfo.NumberOfNoDies = skipCount;
            //22、二维码个数
            summaryInfo.CodeNumber = CodeNumber;
            //23、OK复看为NG数
            summaryInfo.NumberOfReviewNG = OK2NG;
            //24、 误检率算上复看OK到NG
            summaryInfo.DevicePercentOfK2N = (falsecallCount+ OK2NG)*1.0 / summaryInfo.NumberOfDevicesInspected * 100;

            if (isChinese)
            {
                ExcelText.Culture = new System.Globalization.CultureInfo("zh-CN");
            }
            else
            {
                ExcelText.Culture = new System.Globalization.CultureInfo("en-US");
            }

            //if (string.IsNullOrEmpty(excelFileName))
            //{
            //    excelFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + LegalFileName.Get(summaryInfo.ProductCode) + "_" + LegalFileName.Get(summaryInfo.Lot) + ".xls";
            //}
            //string excelPath = excelDirectory + "\\" + excelFileName;
            string excelPath = excelDirectory + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + LegalFileName.Get(summaryInfo.ProductCode) + "_" + LegalFileName.Get(summaryInfo.Lot) + ".xls";
            if (!Directory.Exists(excelDirectory))
            {
                Directory.CreateDirectory(excelDirectory);
            }

            ExcelOperation.ToExcel(excelPath,
                                   ReportModel,
                                   summaryInfo,
                                   list_InspectionDataReportMatrix,
                                   list_DefectTypeInfoReport,
                                   defectStatistics);

            if (Directory.Exists(ImageTempSaveDirectory))
            {
                Directory.Delete(ImageTempSaveDirectory, true);
            }

            ShowMessage("生成结束，保存到" + excelPath);
        }

            
    }
}
