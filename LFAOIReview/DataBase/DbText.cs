using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    /// <summary>
    /// 数据库文本
    /// </summary>
    class DbText
    {
        public static string Table_ProductInfo { get; } = "ProductInfo";
        public static string Table_LotInfo { get; } = "LotInfo";
        public static string Table_DefectTypeInfo { get; } = "DefectTypeInfo";
        public static string TablePrefix_Defect { get; } = "Defect_Lot_";
        public static string TablePrefix_Lot { get; } = "Lot_";
        public static string TablePrefix_FrameIndex { get; } = "FrameIndex_Lot_";
        public static string Column_ProductCode { get; } = "ProductCode";
        public static string Column_Index { get; } = "Index";
        public static string Column_LotName { get; } = "LotName";
        public static string Column_TotalFrameCount { get; } = "TotalFrameCount";
        public static string Column_RowCount { get; } = "RowCount";
        public static string Column_ColumnCount { get; } = "ColumnCount";
        public static string Column_Machine { get; } = "Machine";
        public static string Column_Operator { get; } = "Operator";
        public static string Column_StartDate { get; } = "StartDate";
        public static string Column_StartTime { get; } = "StartTime";
        public static string Column_EndDate { get; } = "EndDate";
        public static string Column_EndTime { get; } = "EndTime";
        public static string Column_FrameName { get; } = "FrameName";
        public static string Column_FrameIndex { get; } = "FrameIndex";
        public static string Column_Row { get; } = "Row";
        public static string Column_Column { get; set; } = "Column";
        public static string Column_InspectionResult { get; } = "InspectionResult";
        public static string Column_ConcatImagePath { get; } = "ImagePath";
        public static string Column_ConcatRegionPath { get; } = "RegionPath";
        public static string Column_WirePath { get; } = "WirePath";
        public static string Column_InspectionDataIndex { get; } = "InspectionDataIndex";
        public static string Column_DefectTypeIndex { get; } = "DefectTypeIndex";
        public static string Column_DefectType { get; } = "DefectType";
        public static string Column_ImageIndex { get; set; } = "ImageIndex";
        public static string Column_ConcatImageIndex { get; set; } = "ConcatImageIndex";
        public static string Column_RegionIndex { get; set; } = "RegionIndex";
        public static string Column_ConcatRegionIndex { get; set; } = "ConcatRegionIndex";
        public static string Column_ErrorDetail { get; set; } = "ErrorDetail";
        public static string Column_Result { get; } = "Result";
        public static string Column_DbIndex { get; } = "DbIndex";
        public static string Column_Code2D { get; } = "Code2D";
        public static string ReviewEdit { get; } = "ReviewEdit_Lot_";
    }
}
