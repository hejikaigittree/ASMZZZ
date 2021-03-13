using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using NPOI.XSSF.UserModel;
using HalconDotNet;


namespace LFAOIReview
{
    class ExcelOperation
    {
        /// <summary>
        /// 20190614新增 DefectStatistics 类 为了统计根据错误优先级输出的错误码
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="summaryInfo"></param>
        /// <param name="list_InspectionDataReportMatrix"></param>
        /// <param name="list_DefectTypeInfoReport"></param>
        /// <param name="defectStatistics"></param>
        public static void ToExcel(string fileName,
                                   int ReportModel,
                                   SummaryInfo summaryInfo,
                                   List<Matrix<InspectionDataReport>> list_InspectionDataReportMatrix,
                                   List<DefectTypeInfoReport> list_DefectTypeInfoReport,
                                   DefectStatistics defectStatistics)
        {
            using (MemoryStream ms = ToMemoryStram(summaryInfo, list_InspectionDataReportMatrix, list_DefectTypeInfoReport, defectStatistics,ReportModel))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        public static MemoryStream ToMemoryStram(SummaryInfo summaryInfo,
                                                 List<Matrix<InspectionDataReport>> list_InspectionDataReportMatrix,
                                                 List<DefectTypeInfoReport> list_DefectTypeInfoReport,
                                                 DefectStatistics defectStatistics,
                                                 int ReportModel)
        {
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(ExcelText.SheetName_SummaryOperator);
            #region SummaryOperator
            int rowIndex = 0;
            int code2drowIndex = 0;

            //1、产品编号
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.ProductCode, summaryInfo.ProductCode);
            //2、批次号
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.Lot, summaryInfo.Lot);
            //3、设备号
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.Machine, summaryInfo.Machine);
            //4、操作员
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.AIOperator, summaryInfo.AIOperator);
            //5、开始日期
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.StartDate, summaryInfo.StartDate);
            //6、开始时间
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.StartTime, summaryInfo.StartTime);
            //7、结束日期
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.EndDate, summaryInfo.EndDate);
            //8、结束时间
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.EndTime, summaryInfo.EndTime);

            rowIndex++;

            //10、批次理论盘数 没有输入默认10 已屏蔽
            //CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.TotalNumberOfStrips, summaryInfo.TotalNumberOfStrips.ToString());
            //11、已检测的盘数
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.NumberOfStripsInspected, summaryInfo.NumberOfStripsInspected.ToString());
            //12、未检测的盘数 已屏蔽
            //CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.NumberOfStripsNotInspected, summaryInfo.NumberOfStripsNotInspected.ToString());
            //13、单盘芯片数
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.QuantityOfDevicesPerStrip, summaryInfo.QuantityOfDevicesPerStrip.ToString());
            //14、批次理论芯片数
            //CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.NumberOfStartQuantity, summaryInfo.NumberOfStartQuantity.ToString());
            //15、批次已检测芯片数
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.NumberOfDevicesInspected, summaryInfo.NumberOfDevicesInspected.ToString());
            //16、每小时检测芯片数
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.DevicesPerHour, string.Format("{0:0.##}", summaryInfo.DevicesPerHour));

            rowIndex++;

            //17、合格芯片数
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.NumberOfDevicesPassed, summaryInfo.NumberOfDevicesPassed.ToString());
            //18、不合格的芯片数
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.NumberOfDevicesRejected, summaryInfo.NumberOfDevicesRejected.ToString());
            //19、误检芯片数
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.NumberOfDevicesFalseCalled, summaryInfo.NumberOfDevicesFalseCalled.ToString());
            //新增 OK复看为NG数
            CreateOneRowTwoColumnCells(sheet, ref rowIndex,"OK复看NG数", summaryInfo.NumberOfReviewNG.ToString());

            //20、跳过的芯片数
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.NumberOfNoDies, summaryInfo.NumberOfNoDies.ToString());

            //21、合格率  存在二维码时根据二维码总数
            if (ReportModel == 1)
            {
                //二维码总数 只在武汉二维码报表模式生成
                CreateOneRowTwoColumnCells(sheet, ref rowIndex, string.Format("{0}", "二维码总数"), summaryInfo.CodeNumber.ToString());
                double temp = summaryInfo.NumberOfDevicesPassed / (double)summaryInfo.CodeNumber;
                CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.YieldByDevice, string.Format("{0:0.##}", temp.ToString()));
            }
            else
            {
                CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.YieldByDevice, string.Format("{0:0.##}", summaryInfo.YieldByDevice));
            }            
            //22、误检率
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.FalseCallDevicePercent, string.Format("{0:0.##}", summaryInfo.FalseCallDevicePercent));
            //新增  当存在 K2N时显示算上复看OK到KG的误检率
            if(summaryInfo.NumberOfReviewNG!=0)
            {
               // CreateOneRowTwoColumnCells(sheet, ref rowIndex, "误检率（%包含复看不合格）", string.Format("{0:0.##}", summaryInfo.DevicePercentOfK2N));
            }


            rowIndex++;

            //23、错误种类出现数量
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.DefectType, ExcelText.Count);
            foreach (DefectTypeInfoReport defectTypeInfoReport in list_DefectTypeInfoReport)
            {
                if (defectTypeInfoReport.Count > 0)
                {
                    CreateOneRowTwoColumnCells(sheet,
                                               ref rowIndex,
                                               string.Format("{0}:{1}", defectTypeInfoReport.Index, defectTypeInfoReport.DefectType),
                                               defectTypeInfoReport.Count.ToString());
                }
            }

            rowIndex++;
            //24、错误种类根据错误优先级统计数量
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.DefectType+"(根据错误优先级表)", ExcelText.Count);
            if (defectStatistics.Flag ==false)
            {
                CreateOneRowTwoColumnCells(sheet, ref rowIndex, "警告，存在未规定的错误优先级", "");
            }
            else
            {
                foreach (KeyValuePair<int, int> ds in defectStatistics.CountDefectResult)
                {
                    if (defectStatistics.CountN2KDefectResult.ContainsKey(ds.Key))
                    {
                        CreateOneRowTwoColumnCells(sheet,
                                                       ref rowIndex,
                                                       string.Format("{0}:{1}", ds.Key, list_DefectTypeInfoReport.Find(ls => ls.Index == ds.Key).DefectType),
                                                       string.Format("{0}-{1}={2}", ds.Value, defectStatistics.CountN2KDefectResult[ds.Key], ds.Value - defectStatistics.CountN2KDefectResult[ds.Key]));
                    }
                    else
                    {
                        CreateOneRowTwoColumnCells(sheet,
                                                       ref rowIndex,
                                                       string.Format("{0}:{1}", ds.Key, list_DefectTypeInfoReport.Find(ls => ls.Index == ds.Key).DefectType),
                                                       ds.Value.ToString());
                    }
                }

                //25、是否存在优先级相同并且一同输出的
                if (defectStatistics.List_RepeatPriority.Count != 0)
                {
                    CreateOneRowTwoColumnCells(sheet, ref rowIndex, "警告 存在优先级相同情况", "");
                    for (int i = 0; i < defectStatistics.List_RepeatPriority.Count; i++)
                    {
                        CreateOneRowTwoColumnCells(sheet, ref rowIndex, defectStatistics.List_RepeatPriority[i].ToString(), "");
                    }
                }
            }

            sheet.SetColumnWidth(0, 80 * 256);
            sheet.SetColumnWidth(0, 30 * 256);
            #endregion

            workbook.CreateSheet(ExcelText.SheetName_MapOperator);
            workbook.CreateSheet(ExcelText.SheetName_UDD);
            int T = 0;
            #region 创建错误图片Sheet
            sheet = workbook.CreateSheet("错误图片");
            int sheetIndex = workbook.GetSheetIndex(sheet);
            sheet.SetColumnWidth(1, 30 * 256);
            sheet.SetColumnWidth(2, 30 * 256);

            int ImageSheetNum = 0;
            T = T + rowIndex;
            rowIndex = 0;
            foreach (Matrix<InspectionDataReport> inspectionDataReportMatrix in list_InspectionDataReportMatrix)
            {  

                if(rowIndex>=65000)
                {
                    ImageSheetNum++;
                    sheet = workbook.CreateSheet("错误图片续_"+ ImageSheetNum);
                    sheetIndex = workbook.GetSheetIndex(sheet);
                    rowIndex = 0;
                }
                HSSFPatriarch sheetPatriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
                HSSFCell cell = null;

                int defectCountInFrame = 0;
                //int sheetContinuedIndexEachStrip = 0;
                int ImageCol = 0;
                foreach (InspectionDataReport dataReport in inspectionDataReportMatrix)
                {
                    defectCountInFrame++;
                    int TempImage_row = 0;
                    ImageCol = 5;
                    int startRow = 0;
                    if (dataReport == null) continue;
                    if (dataReport.List_DefectData.Count == 0)
                    {
                        if (dataReport.InspectionResult != InspectionResult.K2N) continue;
                        
                    }
                    startRow = rowIndex;

                    sheet.CreateRow(rowIndex);
                    cell = (HSSFCell)sheet.GetRow(rowIndex).CreateCell(0);
                    cell.SetCellValue(string.Format("{0}-{1}", dataReport.RowIndex + 1, dataReport.ColumnIndex + 1));
                    cell = (HSSFCell)sheet.GetRow(rowIndex).CreateCell(1);
                    cell.SetCellValue(string.Format("第{0}条", dataReport.FrameIndex));
                    if (dataReport.Code2D != "null")
                    {
                        cell = (HSSFCell)sheet.GetRow(rowIndex).CreateCell(2);
                        cell.SetCellValue(string.Format("二维码：{0}", dataReport.Code2D));
                    }

                    

                    // 创建图片 所有缺陷在一张图中 如果存在拍多次的情况 进行横排  第一张图 col为5 
                    System.Drawing.Image[] images = new System.Drawing.Image[dataReport.List_GeneralImageTempPath.Count];
                    for (int i = 0; i < images.Count(); i++)
                    {
                        //创建图片单元格
                        cell = (HSSFCell)sheet.GetRow(rowIndex).CreateCell(ImageCol);
                        if (File.Exists(dataReport.List_GeneralImageTempPath[i]))
                        {
                            byte[] bytes = File.ReadAllBytes(dataReport.List_GeneralImageTempPath[i]);
                            int index = workbook.AddPicture(bytes, PictureType.JPEG);
                            System.Drawing.Image image;
                            using (MemoryStream ms = new MemoryStream(bytes))
                            {
                                image = System.Drawing.Image.FromStream(ms);
                            }
                            double imageWidth = image.Width;
                            double imageHeight = image.Height;
                            image.Dispose();
                            double cellWidth = (double)sheet.GetColumnWidthInPixels(cell.ColumnIndex);
                            double cellHeight = sheet.DefaultRowHeightInPoints / 72 * 96;
                            int imageInCellColumns = (int)(imageWidth / cellWidth);
                            int imageInCellRows = (int)(imageHeight / cellHeight);
                            double offsetX = (imageWidth - cellWidth * imageInCellColumns) / cellWidth * 1024;
                            double offsetY = (imageHeight - cellHeight * imageInCellRows) / cellHeight * 256;
                            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, (int)offsetX, (int)offsetY, ImageCol, rowIndex, imageInCellColumns + ImageCol, rowIndex + imageInCellRows);
                            sheetPatriarch.CreatePicture(anchor, index);
                            //计算图片高占多少个单元格
                            TempImage_row = (int)Math.Ceiling(imageHeight / cellHeight) + 1;

                            NPOI.SS.Util.CellReference cellReference = new NPOI.SS.Util.CellReference(rowIndex+ TempImage_row, 1);
                            dataReport.ExcelDefectImageLink = string.Format("'{0}'!{1}", sheet.SheetName, cellReference.FormatAsString());

                            
                        }
                        else
                        {
                            cell.SetCellValue("图片不存在");
                        }

                        ImageCol = ImageCol + 10;
                    }

                    rowIndex = rowIndex + 1;
                    //记录错误信息
                    foreach (var defectReort in dataReport.List_DefectData)
                     {
                        rowIndex = rowIndex + 1;
                        sheet.CreateRow(rowIndex);
                        cell = (HSSFCell)sheet.GetRow(rowIndex).CreateCell(1);
                        if (defectReort.DefectTypeIndex <= 0 || defectReort.DefectTypeIndex > list_DefectTypeInfoReport.Count)
                        {
                            cell.SetCellValue(string.Format("{0}:{1}", defectReort.DefectTypeIndex, defectReort.ErrorDetail));
                        }
                        else
                        {
                            cell.SetCellValue(string.Format("{0}:{1}", list_DefectTypeInfoReport[defectReort.DefectTypeIndex - 1].DefectType, defectReort.ErrorDetail));
                        }
                    }
                    rowIndex = rowIndex + 1;
                    string[] show_defecttype = new string[dataReport.List_DefectData.Count];
                    for (int i = 0; i < dataReport.List_DefectData.Count; i++)
                    {
                        show_defecttype[i] = dataReport.List_DefectData[i].DefectTypeIndex.ToString();
                    }
                    sheet.CreateRow(rowIndex);
                    cell = (HSSFCell)sheet.GetRow(rowIndex).CreateCell(1);
                    cell.SetCellValue(string.Format("详细错误码：{0}", string.Join(";", show_defecttype)));



                    //判断 图片所占行数多 还是写入的错误信息行数多
                    if (rowIndex-startRow<= TempImage_row)
                    {
                        rowIndex = startRow + TempImage_row + 2;
                    }
                    

                    #region 每个缺陷对应一张图 已屏蔽
                    //foreach (var defectReort in dataReport.List_DefectData)
                    //{
                    //    if (rowIndex>= 32757)
                    //    {
                    //        sheetContinuedIndexEachStrip++;
                    //        sheet = workbook.CreateSheet(inspectionDataReportMatrix.FrameName + " " + ExcelText.Continued + sheetContinuedIndexEachStrip.ToString());
                    //        sheetIndex = workbook.GetSheetIndex(sheet);
                    //        sheetPatriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
                    //        rowIndex = 0;
                    //    }

                    //    defectCountInFrame++;
                    //    sheet.CreateRow(rowIndex);
                    //    sheet.CreateRow(rowIndex + 1);
                    //    HSSFCell cell = (HSSFCell)sheet.GetRow(rowIndex).CreateCell(0);
                    //    cell.SetCellValue(string.Format("{0}-{1}", dataReport.RowIndex + 1, dataReport.ColumnIndex + 1));
                    //    cell = (HSSFCell)sheet.GetRow(rowIndex).CreateCell(1);
                    //    cell.SetCellValue(list_DefectTypeInfoReport[defectReort.DefectTypeIndex - 1].DefectType);
                    //    cell = (HSSFCell)sheet.GetRow(rowIndex + 1).CreateCell(1);
                    //    cell.SetCellValue(defectReort.ErrorDetail);
                    //    rowIndex += 2;
                    //    sheet.CreateRow(rowIndex);
                    //    cell = (HSSFCell)sheet.GetRow(rowIndex).CreateCell(1);
                    //    if (File.Exists(defectReort.ImageTempPath))
                    //    {
                    //        byte[] bytes = File.ReadAllBytes(defectReort.ImageTempPath);
                    //        int index = workbook.AddPicture(bytes, PictureType.JPEG);
                    //        System.Drawing.Image image;
                    //        using (MemoryStream ms = new MemoryStream(bytes))
                    //        {
                    //            image = System.Drawing.Image.FromStream(ms);
                    //        }
                    //        double imageWidth = image.Width;
                    //        double imageHeight = image.Height;
                    //        image.Dispose();
                    //        double cellWidth = (double)sheet.GetColumnWidthInPixels(cell.ColumnIndex);
                    //        double cellHeight = sheet.DefaultRowHeightInPoints / 72 * 96;
                    //        int imageInCellColumns = (int)(imageWidth / cellWidth);
                    //        int imageInCellRows = (int)(imageHeight / cellHeight);
                    //        double offsetX = (imageWidth - cellWidth * imageInCellColumns) / cellWidth * 1024;
                    //        double offsetY = (imageHeight - cellHeight * imageInCellRows) / cellHeight * 256;

                    //        
                    //        //HSSFClientAnchor commentAnchor = new HSSFClientAnchor(0, 0, (int)offsetX, (int)offsetY, 0, 0, imageInCellColumns, imageInCellRows);
                    //        //commentAnchor.AnchorType = AnchorType.MoveDontResize;
                    //        //HSSFComment comment = (HSSFComment)sheetPatriarch.CreateCellComment(commentAnchor);
                    //        //comment.SetBackgroundImage(index);
                    //        //cell.CellComment = (comment);
                    //        HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, (int)offsetX, (int)offsetY, 1, rowIndex, imageInCellColumns + 1, rowIndex + imageInCellRows);
                    //        sheetPatriarch.CreatePicture(anchor, index);
                    //        if (!hasRecordLink)
                    //        {
                    //            NPOI.SS.Util.CellReference cellReference = new NPOI.SS.Util.CellReference(rowIndex, 1);
                    //            dataReport.ExcelDefectImageLink = string.Format("'{0}'!{1}", sheet.SheetName, cellReference.FormatAsString());
                    //            hasRecordLink = true;
                    //        }
                    //        rowIndex += (int)Math.Ceiling(imageHeight / cellHeight) + 1;
                    //    }
                    //    else
                    //    {
                    //        cell.SetCellValue("图片不存在");
                    //    }
                    //}

                    #endregion
                }
                if (defectCountInFrame == 0)
                {
                    workbook.RemoveSheetAt(sheetIndex);
                }
            }
            #endregion

            #region MapOperator
            sheet = workbook.GetSheet(ExcelText.SheetName_MapOperator);

            if (ReportModel != 1)
            {
                sheet.DefaultColumnWidth = 1;
            }

            //sheet = workbook.GetSheetAt(1);
            rowIndex = 0;
            //1、产品编号
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.ProductCode, summaryInfo.ProductCode);
            //2、批次号
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.Lot, summaryInfo.Lot);
            //3、设备号
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.Machine, summaryInfo.Machine);
            //4、操作员
            CreateOneRowTwoColumnCells(sheet, ref rowIndex, ExcelText.AIOperator, summaryInfo.AIOperator);

            rowIndex++;

            //5、检测结果样例
            Dictionary<InspectionResult, ICellStyle> dict_result_style = new Dictionary<InspectionResult, ICellStyle>();
            ICellStyle exampleStyle = workbook.CreateCellStyle();
            exampleStyle.BorderBottom = BorderStyle.Thin;
            exampleStyle.BorderTop = BorderStyle.Thin;
            exampleStyle.BorderLeft = BorderStyle.Thin;
            exampleStyle.BorderRight = BorderStyle.Thin;
            exampleStyle.FillPattern = FillPattern.SolidForeground;
            exampleStyle.Alignment = HorizontalAlignment.Center;
            exampleStyle.FillForegroundColor = (short)ExcelColors.Green;
            dict_result_style.Add(InspectionResult.OK, exampleStyle);
            CreateOneRowTwoColumnCellsWithColumn1Style(sheet, ref rowIndex, InspectionResultToString(InspectionResult.OK), exampleStyle);

            exampleStyle = workbook.CreateCellStyle();
            exampleStyle.BorderBottom = BorderStyle.Thin;
            exampleStyle.BorderTop = BorderStyle.Thin;
            exampleStyle.BorderLeft = BorderStyle.Thin;
            exampleStyle.BorderRight = BorderStyle.Thin;
            exampleStyle.FillPattern = FillPattern.SolidForeground;
            exampleStyle.Alignment = HorizontalAlignment.Center;
            exampleStyle.FillForegroundColor = (short)ExcelColors.Red;
            dict_result_style.Add(InspectionResult.NG, exampleStyle);
            CreateOneRowTwoColumnCellsWithColumn1Style(sheet, ref rowIndex, InspectionResultToString(InspectionResult.NG), exampleStyle);

            exampleStyle = workbook.CreateCellStyle();
            exampleStyle.BorderBottom = BorderStyle.Thin;
            exampleStyle.BorderTop = BorderStyle.Thin;
            exampleStyle.BorderLeft = BorderStyle.Thin;
            exampleStyle.BorderRight = BorderStyle.Thin;
            exampleStyle.FillPattern = FillPattern.SolidForeground;
            exampleStyle.Alignment = HorizontalAlignment.Center;
            exampleStyle.FillForegroundColor = (short)ExcelColors.Yellow;
            dict_result_style.Add(InspectionResult.N2K, exampleStyle);
            CreateOneRowTwoColumnCellsWithColumn1Style(sheet, ref rowIndex, InspectionResultToString(InspectionResult.N2K), exampleStyle);

            exampleStyle = workbook.CreateCellStyle();
            exampleStyle.BorderBottom = BorderStyle.Thin;
            exampleStyle.BorderTop = BorderStyle.Thin;
            exampleStyle.BorderLeft = BorderStyle.Thin;
            exampleStyle.BorderRight = BorderStyle.Thin;
            exampleStyle.FillPattern = FillPattern.SolidForeground;
            exampleStyle.Alignment = HorizontalAlignment.Center;
            exampleStyle.FillForegroundColor = (short)ExcelColors.Orange;
            dict_result_style.Add(InspectionResult.K2N, exampleStyle);
            CreateOneRowTwoColumnCellsWithColumn1Style(sheet, ref rowIndex, InspectionResultToString(InspectionResult.K2N), exampleStyle);

            exampleStyle = workbook.CreateCellStyle();
            exampleStyle.BorderBottom = BorderStyle.Thin;
            exampleStyle.BorderTop = BorderStyle.Thin;
            exampleStyle.BorderLeft = BorderStyle.Thin;
            exampleStyle.BorderRight = BorderStyle.Thin;
            exampleStyle.FillPattern = FillPattern.SolidForeground;
            exampleStyle.Alignment = HorizontalAlignment.Center;
            exampleStyle.FillForegroundColor = (short)ExcelColors.SkyBlue;
            dict_result_style.Add(InspectionResult.SKIP, exampleStyle);
            CreateOneRowTwoColumnCellsWithColumn1Style(sheet, ref rowIndex, InspectionResultToString(InspectionResult.SKIP), exampleStyle);


            rowIndex++;

            //6、分盘绘制图谱
            HSSFPatriarch patriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
           
            ICellStyle centerAlignmentStyle = workbook.CreateCellStyle();
            centerAlignmentStyle.Alignment = HorizontalAlignment.Center;

            int sheetContinuedIndex = 0;

            foreach (Matrix<InspectionDataReport> inspectionDataReportMatrix in list_InspectionDataReportMatrix)
            {
                if (inspectionDataReportMatrix.ColumnCount < 254)
                {
                    //创建单元格
                    for (int i = 0; i < inspectionDataReportMatrix.RowCount + 1; i++)
                    {
                        IRow createRow = sheet.CreateRow(rowIndex + i);
                        for (int j = 0; j < inspectionDataReportMatrix.ColumnCount + 2; j++)
                        {
                            createRow.CreateCell(j);
                        }
                    }
                    //赋值条号
                    sheet.GetRow(rowIndex).GetCell(0).SetCellValue(string.Format("{0}:{1}", ExcelText.Strip, inspectionDataReportMatrix.FrameName));
                    for (int i = 0; i < inspectionDataReportMatrix.RowCount; i++)
                    {
                        sheet.GetRow(rowIndex + i).GetCell(1).SetCellValue((i + 1).ToString());
                    }
                    for (int i = 0; i < inspectionDataReportMatrix.ColumnCount; i++)
                    {
                        ICell cell = sheet.GetRow(rowIndex + inspectionDataReportMatrix.RowCount).GetCell(i + 2);
                        cell.SetCellValue((i + 1).ToString());
                        cell.CellStyle = centerAlignmentStyle;
                    }

                    //创建二维码列表
                    if(ReportModel==1)
                    {
                       code2drowIndex = rowIndex + inspectionDataReportMatrix.RowCount + 2;

                        //创建单元格
                        for (int i = 0; i < inspectionDataReportMatrix.RowCount + 1; i++)
                        {
                            IRow createRow = sheet.CreateRow(code2drowIndex + i);
                            for (int j = 0; j < inspectionDataReportMatrix.ColumnCount + 2; j++)
                            {
                                createRow.CreateCell(j);
                            }
                        }

                        //赋值二维码
                        sheet.GetRow(code2drowIndex).GetCell(0).SetCellValue(string.Format("{0}:{1}", "二维码", inspectionDataReportMatrix.FrameName));
                        for (int i = 0; i < inspectionDataReportMatrix.RowCount; i++)
                        {
                            sheet.GetRow(code2drowIndex + i).GetCell(1).SetCellValue((i + 1).ToString());
                        }
                        //for (int i = 0; i < inspectionDataReportMatrix.ColumnCount; i++)
                        //{
                        //    ICell cell = sheet.GetRow(code2drowIndex + inspectionDataReportMatrix.RowCount).GetCell(i + 2);
                        //    //cell.SetCellValue((i + 1).ToString());
                        //    cell.CellStyle = centerAlignmentStyle;
                        //}
                       
                        foreach (InspectionDataReport data1 in inspectionDataReportMatrix)
                        {
                            if (data1 == null)
                            {
                                continue;
                            }
                            ICell cell;
                            if (inspectionDataReportMatrix.ColumnCount < 254)
                            {
                                cell = sheet.GetRow(code2drowIndex + data1.RowIndex).GetCell(data1.ColumnIndex + 2);
                            }
                            else
                            {
                                cell = sheet.GetRow(code2drowIndex + data1.ColumnIndex).GetCell(data1.RowIndex + 2);
                            }

                            cell.SetCellValue(data1.Code2D);
                        }
                    }
                }
                else
                {
                    if (rowIndex + inspectionDataReportMatrix.ColumnCount + 1 >= 32757)
                    {
                        sheetContinuedIndex++;
                        sheet = workbook.CreateSheet(ExcelText.SheetName_MapOperator + " " + ExcelText.Continued + sheetContinuedIndex.ToString());
                        rowIndex = 0;
                    }

                    for (int i = 0; i < inspectionDataReportMatrix.ColumnCount + 1; i++)
                    {
                        IRow createRow = sheet.CreateRow(rowIndex + i);
                        for (int j = 0; j < inspectionDataReportMatrix.RowCount + 2; j++)
                        {
                            createRow.CreateCell(j);
                        }
                    }
                    sheet.GetRow(rowIndex).GetCell(0).SetCellValue(string.Format("{0}:{1}", ExcelText.Strip, inspectionDataReportMatrix.FrameName));
                    for (int i = 0; i < inspectionDataReportMatrix.ColumnCount; i++)
                    {
                        sheet.GetRow(rowIndex + i).GetCell(1).SetCellValue((i + 1).ToString());
                    }
                    for (int i = 0; i < inspectionDataReportMatrix.RowCount; i++)
                    {
                        ICell cell = sheet.GetRow(rowIndex + inspectionDataReportMatrix.ColumnCount).GetCell(i + 2);
                        cell.SetCellValue((i + 1).ToString());
                        cell.CellStyle = centerAlignmentStyle;
                    }
                }


                foreach (InspectionDataReport data in inspectionDataReportMatrix)
                {
                    if (data == null)
                    {
                        continue;
                    }
                    ICell cell;
                    if (inspectionDataReportMatrix.ColumnCount < 254)
                    {
                        cell = sheet.GetRow(rowIndex + data.RowIndex).GetCell(data.ColumnIndex + 2);
                    }
                    else
                    {
                        cell = sheet.GetRow(rowIndex + data.ColumnIndex).GetCell(data.RowIndex + 2);
                    }                  
                    cell.CellStyle = dict_result_style[data.InspectionResult];
                    if (data.InspectionResult == InspectionResult.NG)
                    {
                        //string[] defectTypes = new string[data.List_DefectData.Count];
                        //for (int i = 0; i < data.List_DefectData.Count; i++)
                        //{
                        //    defectTypes[i] = data.List_DefectData[i].DefectTypeIndex.ToString();
                        //}

                        string[] noRepeatDefectTyoes = new string[data.Priority_DetectType.Count];
                        for (int i = 0; i < data.Priority_DetectType.Count; i++)
                        {
                            noRepeatDefectTyoes[i] = data.Priority_DetectType[i].ToString();
                        }

                        //string[] noRepeatDefectTyoes = data.List_DefectData.Select(d => d.DefectTypeIndex.ToString()).Distinct().ToArray();
                        cell.SetCellValue(string.Join(";", noRepeatDefectTyoes));
                    }

                    if (data.InspectionResult == InspectionResult.NG || data.InspectionResult == InspectionResult.N2K|| data.InspectionResult == InspectionResult.K2N)
                    {
                        if (data.ExcelDefectImageLink == null) continue;
                        HSSFHyperlink link = new HSSFHyperlink(HyperlinkType.Document);
                        link.Address = data.ExcelDefectImageLink;
                        cell.Hyperlink = link;
                    }

                    #region 对NG图 以及 N2K图添加批注 鼠标悬停可以显示图片 已屏蔽 
                    //if (data.InspectionResult == InspectionResults.NG || data.InspectionResult == InspectionResults.N2K)
                    //{
                    //    //int defectCount = data.List_DefectData.Count;
                    //    //System.Drawing.Image[] images = new System.Drawing.Image[defectCount];
                    //    System.Drawing.Image[] images = new System.Drawing.Image[data.List_GeneralImageTempPath.Count];
                    //    double imageMaxWidth = 0;
                    //    double imageTotalHeight = 0;
                    //    //for (int i = 0; i < defectCount; i++)
                    //    //{
                    //    //    if (!File.Exists(data.List_DefectData[i].ImageTempPath)) break;
                    //    //    byte[] bytesOfImage = File.ReadAllBytes(data.List_DefectData[i].ImageTempPath);
                    //    //    using (MemoryStream ms = new MemoryStream(bytesOfImage))
                    //    //    {
                    //    //        images[i] = System.Drawing.Image.FromStream(ms);
                    //    //    }
                    //    //    if (images[i].Width > imageMaxWidth)
                    //    //    {
                    //    //        imageMaxWidth = images[i].Width;
                    //    //    }
                    //    //    imageTotalHeight += images[i].Height;
                    //    //}
                    //    for (int i = 0; i < images.Count(); i++)
                    //    {
                    //        if (!File.Exists(data.List_GeneralImageTempPath[i])) break;
                    //        byte[] bytesOfImage = File.ReadAllBytes(data.List_GeneralImageTempPath[i]);
                    //        using (MemoryStream ms = new MemoryStream(bytesOfImage))
                    //        {
                    //            images[i] = System.Drawing.Image.FromStream(ms);
                    //        }
                    //        if (images[i].Width > imageMaxWidth)
                    //        {
                    //            imageMaxWidth = images[i].Width;
                    //        }
                    //        imageTotalHeight += images[i].Height;
                    //    }

                    //    if (imageMaxWidth == 0 || imageTotalHeight == 0) continue;
                    //    Bitmap concatImage = new Bitmap((int)imageMaxWidth, (int)imageTotalHeight);
                    //    concatImage.SetResolution(300, 300);
                    //    Graphics g = Graphics.FromImage(concatImage);
                    //    float drawStartHeight = 0;
                    //    for (int i = 0; i < images.Count(); i++)
                    //    {
                    //        g.DrawImage(images[i], 0, drawStartHeight);
                    //        drawStartHeight += images[i].Height;
                    //        images[i].Dispose();
                    //    }

                    //    byte[] bytesOfConcatImage;
                    //    using (MemoryStream ms = new MemoryStream())
                    //    {
                    //        concatImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //        bytesOfConcatImage = ms.ToArray();
                    //    }
                    //    int index = workbook.AddPicture(bytesOfConcatImage, PictureType.JPEG);
                    //    double cellWidth = (double)sheet.GetColumnWidthInPixels(cell.ColumnIndex);
                    //    //EXCEL列高度的单位是磅,Apache POI的行高度单位是缇(twip)
                    //    //DPI = 1英寸内可显示的像素点个数。通常电脑屏幕是96DPI, IPhone4s的屏幕是326DPI, 普通激光黑白打印机是400DPI
                    //    //要计算Excel的行高，就先把它行转换到英寸，再乘小DPI就可以得到像素
                    //    //像素 = (Excel的行高度 / 72) * DPI
                    //    double cellHeight = sheet.DefaultRowHeightInPoints / 72 * 96;
                    //    int imageInCellColumns = (int)(imageMaxWidth / cellWidth);
                    //    int imageInCellRows = (int)(imageTotalHeight / cellHeight);
                    //    double offsetX = (imageMaxWidth - cellWidth * imageInCellColumns) / cellWidth * 1024;
                    //    double offsetY = (imageTotalHeight - cellHeight * imageInCellRows) / cellHeight * 256;
                    //    IClientAnchor commentAnchor = new HSSFClientAnchor(0, 0, (int)offsetX, (int)offsetY, 0, 0, imageInCellColumns, imageInCellRows);
                    //    commentAnchor.AnchorType = AnchorType.MoveDontResize;

                    //    HSSFComment comment = (HSSFComment)patriarch.CreateCellComment(commentAnchor);        
                    //    comment.SetBackgroundImage(index);
                    //    cell.CellComment = comment;

                    //    HSSFHyperlink link = new HSSFHyperlink(HyperlinkType.Document);
                    //    link.Address = data.ExcelDefectImageLink;
                    //    cell.Hyperlink = link;
                    //    //NPOI.SS.Util.CellReference cr = new NPOI.SS.Util.CellReference("A1");
                    //    //ICellStyle hlink_style = hssfworkbook.CreateCellStyle();
                    //    //IFont hlink_font = hssfworkbook.CreateFont();
                    //    //hlink_font.Underline = FontUnderlineType.Single;
                    //    //hlink_font.Color = HSSFColor.Blue.Index;
                    //    //hlink_style.SetFont(hlink_font);
                    //    //cell.CellStyle = (hlink_style);

                    //    //HSSFCell picIndexCell = (HSSFCell)sheet.GetRow(rowIndex).CreateCell(picColumnIndex);
                    //    //picIndexCell.SetCellValue(string.Format("{0}-{1}", data.Row + 1, data.Column + 1));
                    //    //picIndexCell.CellStyle = centerAlignmentStyle;
                    //    //if (!data.PicturePath.Equals(string.Empty))
                    //    //{
                    //    //    if (File.Exists(data.PicturePath))
                    //    //    {
                    //    //        byte[] bytes = File.ReadAllBytes(data.PicturePath);
                    //    //        int index = workbook.AddPicture(bytes, PictureType.JPEG);
                    //    //        System.Drawing.Image image;
                    //    //        using (MemoryStream ms = new MemoryStream(bytes))
                    //    //        {
                    //    //            image = System.Drawing.Image.FromStream(ms);
                    //    //        }
                    //    //        double imageWidth = image.Width;
                    //    //        double imageHeight = image.Height;
                    //    //        image.Dispose();
                    //    //        double cellWidth = (double)sheet.GetColumnWidthInPixels(cell.ColumnIndex);
                    //    //        //EXCEL列高度的单位是磅,Apache POI的行高度单位是缇(twip)
                    //    //        //DPI = 1英寸内可显示的像素点个数。通常电脑屏幕是96DPI, IPhone4s的屏幕是326DPI, 普通激光黑白打印机是400DPI
                    //    //        //要计算Excel的行高，就先把它行转换到英寸，再乘小DPI就可以得到像素
                    //    //        //像素 = (Excel的行高度 / 72) * DPI
                    //    //        double cellHeight = sheet.DefaultRowHeightInPoints / 72 * 96;
                    //    //        int imageInCellColumns = (int)(imageWidth / cellWidth);
                    //    //        int imageInCellRows = (int)(imageHeight / cellHeight);
                    //    //        double offsetX = (imageWidth - cellWidth * imageInCellColumns) / cellWidth * 1024;
                    //    //        double offsetY = (imageHeight - cellHeight * imageInCellRows) / cellHeight * 256;
                    //    //        HSSFClientAnchor commentAnchor = new HSSFClientAnchor(0, 0, (int)offsetX, (int)offsetY, 0, 0, imageInCellColumns, imageInCellRows);
                    //    //        commentAnchor.AnchorType = AnchorType.MoveDontResize;
                    //    //        HSSFComment comment = (HSSFComment)patriarch.CreateCellComment(commentAnchor);
                    //    //        comment.SetBackgroundImage(index);
                    //    //        cell.CellComment = (comment);
                    //    //        //HSSFClientAnchor anchor = new HSSFClientAnchor(100, 0, 923, 0, picColumnIndex, rowIndex + 1, picColumnIndex, rowIndex + dieDataSQLMatrix.RowCount);
                    //    //        //patriarch.CreatePicture(anchor, index);       
                    //    //    }
                    //    //}
                    //    //else
                    //    //{
                    //    //    cell = (HSSFCell)sheet.GetRow(rowIndex + 1).CreateCell(picColumnIndex);
                    //    //    cell.SetCellValue("图片不存在");
                    //    //    cell.CellStyle = centerAlignmentStyle;
                    //    //}
                    //    //picColumnIndex++;

                    //}
                    #endregion
                }

                if (inspectionDataReportMatrix.ColumnCount < 254)
                {
                    if(ReportModel==1)
                    {
                        rowIndex = code2drowIndex;
                    }
                    
                    rowIndex += inspectionDataReportMatrix.RowCount + 2;
                }
                else
                {
                    rowIndex += inspectionDataReportMatrix.ColumnCount + 2;
                }
            }
            sheet.SetColumnWidth(0, 20 * 256);
            #endregion


            #region UDD

            sheet = workbook.GetSheet(ExcelText.SheetName_UDD);
            //sheet = workbook.GetSheetAt(2);
            rowIndex = 0;
            foreach (DefectTypeInfoReport defectTypeInfoReport in list_DefectTypeInfoReport)
            {
                CreateOneRowTwoColumnCells(sheet, ref rowIndex, defectTypeInfoReport.Index.ToString(), defectTypeInfoReport.DefectType);
            }

            #endregion
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                //sheet.Dispose();
                return ms;
            }
        }

        private static void CreateOneRowTwoColumnCells(ISheet sheet, ref int rowIndex, string column0_text, string column1_text)
        {
            IRow row = sheet.CreateRow(rowIndex);
            ICell cell = row.CreateCell(0);
            cell.SetCellValue(column0_text);
            cell = row.CreateCell(1);
            cell.SetCellValue(column1_text);
            rowIndex++;
        }

        private static void CreateOneRowTwoColumnCellsWithColumn0FontColor(ISheet sheet, ref int rowIndex, string column0_text, string column1_text, IFont font)
        {
            IRow row = sheet.CreateRow(rowIndex);
            ICell cell = row.CreateCell(0);
            IRichTextString richString = new HSSFRichTextString(column0_text);
            richString.ApplyFont(font);
            cell.SetCellValue(richString);
            cell = row.CreateCell(1);
            cell.SetCellValue(column1_text);
            rowIndex++;
        }

        private static void CreateOneRowTwoColumnCellsWithColumn1Style(ISheet sheet, ref int rowIndex, string column0_text, ICellStyle style)
        {
            IRow row = sheet.CreateRow(rowIndex);
            ICell cell = (ICell)row.CreateCell(0);
            cell.SetCellValue(column0_text);
            cell = (ICell)row.CreateCell(1);
            cell.CellStyle = style;
            rowIndex++;
        }

        private static string InspectionResultToString(InspectionResult inspectionResult)
        {
            switch (inspectionResult)
            {
                case InspectionResult.OK:
                    return ExcelText.Pass;
                case InspectionResult.NG:
                    return ExcelText.Fail;
                case InspectionResult.N2K:
                    return ExcelText.FalseCall;
                case InspectionResult.K2N:
                    return "复看不合格";
                case InspectionResult.SKIP:
                    return ExcelText.Skip;
                default: return string.Empty;
            }
        }


    }
}
