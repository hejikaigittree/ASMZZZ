using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Reflection;
using System.Data;
using HalconDotNet;
using System.IO;


namespace LFAOIReview
{
    /// <summary>
    /// 与 InspectionDataView 作用一致
    /// </summary>
    public class TestDataView
    {
        public int DbIndex { get; set; }
        
        /// <summary>
        /// 行序号
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 列序号
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 检测结果
        /// </summary>
        public InspectionResult InspectionResult { get; set; }

        public List<string> ConcatImagePath { get; set; } = new List<string>(1);
        public List<string> ConcatRegionPath { get; set; } = new List<string>(0);
        public List<string> WirePath { get; set; } = new List<string>(1);
    }

    /// <summary>
    /// 与 DefectDataView 作用一致
    /// </summary>
    public class ResultDataView
    {
        public int DbIndex { get; set; }
        public int InspectionDataDbIndex { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public int FrameIndex { get; set; }
        public int ConcatImageIndex { get; set; }
        public int RegionIndex { get; set; }
        public int ConcatRegionIndex { get; set; }
        public int Result { get; set; }
        public  string ErrorDetail { get; set; }

        /// <summary>
        /// 缺陷类型编号
        /// </summary>
        public int DefectTypeIndex { get; set; }

        /// <summary>
        /// 缺陷所属图像
        /// -1 原图为黑白图
        /// 0 R通道
        /// 1 G通道
        /// 2 B通道
        /// </summary>
        public int ImageIndex { get; set; }
    }

    public class DataGeting
    { 
        public bool ReadData(string dbFilePath, int lotIndex, string frameName,
                                   out List<TestDataView> list_InspectionDataView,
                                   out List<ResultDataView> list_DefectDataView,
                                   out Dictionary<int, string> dict_DefectTyoe)
        {

            bool result = false;
            list_InspectionDataView = new List<TestDataView>();
            list_DefectDataView = new List<ResultDataView>();
            dict_DefectTyoe = new Dictionary<int, string>();
            try
            {
                using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
                {
                    if (sqlCon.State != System.Data.ConnectionState.Open)
                    {
                        sqlCon.Open();
                    }
                    SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"",
                                                      DbText.TablePrefix_FrameIndex + lotIndex.ToString(),
                                                      DbText.Column_FrameName,
                                                      frameName);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        cmd.Dispose();
                        sqlCon.Close();
                        return result;
                    }
                    int frameIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                    reader.Close();

                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE \"{1}\" = '{2}'",
                                                    DbText.Table_LotInfo,
                                                    DbText.Column_Index,
                                                    lotIndex);
                    reader = cmd.ExecuteReader();
                    string productCode_LegalFileName;
                    string lotName_LegalFileName;

                    if (!reader.Read())
                    {
                        throw new Exception("数据库未找到该批次信息");
                    }
                    else
                    {
                        productCode_LegalFileName = LegalFileName.Get(Convert.ToString(reader[DbText.Column_ProductCode]));
                        lotName_LegalFileName = LegalFileName.Get(Convert.ToString(reader[DbText.Column_LotName]));
                    }
                    reader.Close();


                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"",
                                                    DbText.TablePrefix_Lot + lotIndex.ToString(),
                                                    DbText.Column_FrameIndex,
                                                    frameIndex);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TestDataView dataView = new TestDataView();
                        dataView.DbIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                        dataView.RowIndex = Convert.ToInt32(reader[DbText.Column_Row]);
                        dataView.ColumnIndex = Convert.ToInt32(reader[DbText.Column_Column]);
                        dataView.InspectionResult = (InspectionResult)Enum.Parse(typeof(InspectionResult), Convert.ToString(reader[DbText.Column_InspectionResult]));


                        string[] concatImagePaths = Convert.ToString(reader[DbText.Column_ConcatImagePath]).Split(';');
                        if (!string.IsNullOrEmpty(concatImagePaths[0]))
                        {
                            foreach (string concatImagePath in concatImagePaths)
                            {
                                dataView.ConcatImagePath.Add("\\" + productCode_LegalFileName +
                                                             "\\" + lotName_LegalFileName
                                                                  + concatImagePath);
                            }
                        }

                        string[] concatRegionPaths = Convert.ToString(reader[DbText.Column_ConcatRegionPath]).Split(';');
                        if (!string.IsNullOrEmpty(concatRegionPaths[0]))
                        {
                            foreach (string concatRegionPath in concatRegionPaths)
                            {
                                dataView.ConcatRegionPath.Add("\\" + productCode_LegalFileName +
                                                              "\\" + lotName_LegalFileName
                                                                   + concatRegionPath);
                            }
                        }

                        string[] WirePaths = Convert.ToString(reader[DbText.Column_WirePath]).Split(';');
                        if (!string.IsNullOrEmpty(WirePaths[0]))
                        {
                            foreach (string wirePath in WirePaths)
                            {
                                dataView.WirePath.Add("\\" + productCode_LegalFileName +
                                                      "\\" + lotName_LegalFileName
                                                           + wirePath);
                            }
                        }
                        list_InspectionDataView.Add(dataView);
                    }
                    reader.Close();
                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\" ORDER BY {3}, {4} ASC",
                                                    DbText.TablePrefix_Defect + lotIndex.ToString(),
                                                    DbText.Column_FrameIndex,
                                                    frameIndex,
                                                    DbText.Column_InspectionDataIndex,
                                                    DbText.Column_DefectTypeIndex);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ResultDataView defectView = new ResultDataView();
                        defectView.DbIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                        defectView.InspectionDataDbIndex = Convert.ToInt32(reader[DbText.Column_InspectionDataIndex]);
                        defectView.DefectTypeIndex = Convert.ToInt32(reader[DbText.Column_DefectTypeIndex]);
                        defectView.RowIndex = Convert.ToInt32(reader[DbText.Column_Row]);
                        defectView.ColumnIndex = Convert.ToInt32(reader[DbText.Column_Column]);
                        defectView.Result = Convert.ToInt32(reader[DbText.Column_Result]);
                        defectView.ConcatImageIndex = Convert.ToInt32(reader[DbText.Column_ConcatImageIndex]);
                        defectView.ImageIndex = Convert.ToInt32(reader[DbText.Column_ImageIndex]);
                        defectView.ConcatRegionIndex = Convert.ToInt32(reader[DbText.Column_ConcatRegionIndex]);
                        defectView.RegionIndex = Convert.ToInt32(reader[DbText.Column_RegionIndex]);
                        defectView.ErrorDetail = Convert.ToString(reader[DbText.Column_ErrorDetail]);
                        list_DefectDataView.Add(defectView);
                    }
                    reader.Close();
                    cmd.CommandText = string.Format("SELECT * FROM {0}",
                                                    DbText.Table_DefectTypeInfo);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dict_DefectTyoe[Convert.ToInt32(reader[DbText.Column_Index])]
                            = Convert.ToString(reader[DbText.Column_DefectType]);
                    }
                    result = true;
                    reader.Close();
                    cmd.Dispose();
                    sqlCon.Close();
                }
            }
            catch { }
            return result;
        }

    
    }
}
