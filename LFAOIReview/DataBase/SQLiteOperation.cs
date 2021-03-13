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
    class SQLiteOperation
    {
        #region Data
        public static void InitialDataBase(string dbFilePath, LotInfo lotInfo, List<DefectTypeInfo> list_DefectTypeInfo, out int lotIndex)
        {
            using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
            {
                if (sqlCon.State != System.Data.ConnectionState.Open)
                {
                    sqlCon.Open();
                }
                SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                StringBuilder sb = new StringBuilder();

                cmd.CommandText = "PRAGMA synchronous = OFF;";
                cmd.ExecuteNonQuery();
                cmd = new SQLiteCommand("begin", sqlCon);
                cmd.ExecuteNonQuery();

                //创建LotInfo表
                sb.Append(string.Format("CREATE TABLE IF NOT EXISTS {0} (" +
                                        "'{1}' integer PRIMARY KEY NOT NULL, " +
                                        "'{2}' TEXT NOT NULL, " +
                                        "'{3}' TEXT NOT NULL, " +
                                        "'{4}' integer NOT NULL, " +
                                        "'{5}' integer NOT NULL, " +
                                        "'{6}' integer NOT NULL, " +
                                        "'{7}' TEXT, " +
                                        "'{8}' TEXT, " +
                                        "'{9}' TEXT, " +
                                        "'{10}' TEXT, " +
                                        "'{11}' TEXT, " +
                                        "'{12}' TEXT ); ",
                                        DbText.Table_LotInfo,
                                        DbText.Column_Index,
                                        DbText.Column_LotName,
                                        DbText.Column_ProductCode,
                                        DbText.Column_TotalFrameCount,
                                        DbText.Column_RowCount,
                                        DbText.Column_ColumnCount,
                                        DbText.Column_Machine,
                                        DbText.Column_Operator,
                                        DbText.Column_StartDate,
                                        DbText.Column_StartTime,
                                        DbText.Column_EndDate,
                                        DbText.Column_EndTime));

                sb.Append(string.Format("CREATE TABLE IF NOT EXISTS {0} (" +
                                         "'{1}' integer PRIMARY KEY NOT NULL, " +
                                         "'{2}' TEXT NOT NULL ); ",
                                         DbText.Table_DefectTypeInfo,
                                         DbText.Column_Index,
                                         DbText.Column_DefectType));

                //插入检测错误列表
                sb.Append(string.Format("DELETE FROM {0}; ", DbText.Table_DefectTypeInfo));
                sb.Append(InsertValues(list_DefectTypeInfo, DbText.Table_DefectTypeInfo));

                cmd.CommandText = sb.ToString();
                cmd.ExecuteNonQuery();

                //插入LotName，获取LotIndex
                cmd.CommandText = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"",
                                                DbText.Table_LotInfo,
                                                DbText.Column_LotName,
                                                lotInfo.LotName);
                SQLiteDataReader reader = cmd.ExecuteReader();
                sb = new StringBuilder();
                if (!reader.Read())
                {
                    //不存在则插入新行
                    reader.Close();
                    cmd.CommandText = string.Format("SELECT COUNT (*) FROM {0}", DbText.Table_LotInfo);
                    //序号从1开始
                    lotIndex = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                    sb.Append(InsertValues(lotInfo, DbText.Table_LotInfo));
                }
                else
                {
                    //存在则更新
                    lotIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                    reader.Close();
                    sb.Append(string.Format("UPDATE {0} SET ", DbText.Table_LotInfo));
                    List<string> list_Parameter = new List<string>(5);
                    list_Parameter.Add(DbText.Column_TotalFrameCount);
                    list_Parameter.Add(DbText.Column_RowCount);
                    list_Parameter.Add(DbText.Column_ColumnCount);
                    list_Parameter.Add(DbText.Column_Machine);
                    list_Parameter.Add(DbText.Column_Operator);
                    List<string> list_Value = new List<string>(5);
                    list_Value.Add(lotInfo.TotalFrameCount.ToString());
                    list_Value.Add(lotInfo.RowCount.ToString());
                    list_Value.Add(lotInfo.ColumnCount.ToString());
                    list_Value.Add(lotInfo.Machine);
                    list_Value.Add(lotInfo.Operator);
                    string[] setStrings = new string[list_Parameter.Count];
                    for (int i = 0; i < list_Parameter.Count; i++)
                    {
                        setStrings[i] = string.Format("'{0}' = '{1}'", list_Parameter[i], list_Value[i]);
                    }
                    string setString = string.Join(",", setStrings);
                    sb.Append(setString);
                    sb.Append(string.Format(" WHERE \"{0}\" = '{1}'; ", DbText.Column_Index, lotIndex));
                }


                //创建Lot数据表
                sb.Append(string.Format("CREATE TABLE IF NOT EXISTS {0} (" +
                                                "'{1}' integer PRIMARY KEY NOT NULL, " +
                                                "'{2}' integer NOT NULL, " +
                                                "'{3}' integer NOT NULL, " +
                                                "'{4}' integer NOT NULL, " +
                                                "'{5}' TEXT NOT NULL, " +
                                                "'{6}' TEXT, " +
                                                "'{7}' TEXT, " +
                                                "'{8}' TEXT , "+
                                                "'{9}' TEXT ); ",
                                                DbText.TablePrefix_Lot + lotIndex,
                                                DbText.Column_Index,
                                                DbText.Column_FrameIndex,
                                                DbText.Column_Row,
                                                DbText.Column_Column,
                                                DbText.Column_InspectionResult,
                                                DbText.Column_ConcatImagePath,
                                                DbText.Column_ConcatRegionPath,
                                                DbText.Column_WirePath,
                                                DbText.Column_Code2D
                                                ));

                //创建Defect数据表
                sb.Append(string.Format("CREATE TABLE IF NOT EXISTS {0} (" +
                                                "'{1}' integer PRIMARY KEY NOT NULL, " +
                                                "'{2}' integer NOT NULL, " +
                                                "'{3}' integer NOT NULL, " +
                                                "'{4}' integer NOT NULL, " +
                                                "'{5}' integer NOT NULL, " +
                                                "'{6}' integer NOT NULL, " +
                                                "'{7}' integer NOT NULL, " +
                                                "'{8}' integer NOT NULL, " +
                                                "'{9}' integer NOT NULL, " +
                                                "'{10}' integer NOT NULL, " +
                                                "'{11}' integer NOT NULL, " +
                                                "'{12}' TEXT ); ",
                                                DbText.TablePrefix_Defect + lotIndex,
                                                DbText.Column_Index,
                                                DbText.Column_FrameIndex,
                                                DbText.Column_InspectionDataIndex,
                                                DbText.Column_DefectTypeIndex,
                                                DbText.Column_Row,
                                                DbText.Column_Column,
                                                DbText.Column_Result,
                                                DbText.Column_ConcatImageIndex,
                                                DbText.Column_ImageIndex,
                                                DbText.Column_ConcatRegionIndex,
                                                DbText.Column_RegionIndex,
                                                DbText.Column_ErrorDetail));


                //创建Frame索引表
                sb.Append(string.Format("CREATE TABLE IF NOT EXISTS {0} (" +
                                                "'{1}' integer PRIMARY KEY NOT NULL, " +
                                                "'{2}' TEXT NOT NULL ); ",
                                                DbText.TablePrefix_FrameIndex + lotIndex,
                                                DbText.Column_Index,
                                                DbText.Column_FrameName));
                cmd.CommandText = sb.ToString();
                cmd.ExecuteNonQuery();

                cmd = new SQLiteCommand("end", sqlCon);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "PRAGMA synchronous = ON;";
                cmd.ExecuteNonQuery();

                cmd.Dispose();
                sqlCon.Close();
            }
        }

        public static void UpdateValues(string dbFilePath, int lotIndex, List<string> list_Parameter, List<string> list_Value)
        {
            try
            {
                using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
                {
                    sqlCon.Open();
                    SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Format("UPDATE {0} SET ", DbText.Table_LotInfo));
                    string[] setStrings = new string[list_Parameter.Count];
                    for (int i = 0; i < list_Parameter.Count; i++)
                    {
                        setStrings[i] = string.Format("'{0}' = '{1}'", list_Parameter[i], list_Value[i]);
                    }
                    string setString = string.Join(",", setStrings);
                    sb.Append(setString);
                    sb.Append(string.Format(" WHERE \"{0}\" = '{1}' ", DbText.Column_Index, lotIndex));
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    sqlCon.Close();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InitialFrame(string dbFilePath, int lotIndex, string frameName, out int frameIndex)
        {
            using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
            {
                if (sqlCon.State != System.Data.ConnectionState.Open)
                {
                    sqlCon.Open();
                }
                SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                cmd.CommandText = string.Format("SELECT * FROM {0} WHERE \"{1}\" = \"{2}\"",
                                               DbText.TablePrefix_FrameIndex + lotIndex.ToString(),
                                               DbText.Column_FrameName,
                                               frameName);
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    //不存在则插入新行
                    reader.Close();
                    cmd.CommandText = string.Format("SELECT COUNT (*) FROM {0}", DbText.TablePrefix_FrameIndex + lotIndex.ToString());
                    //序号从1开始
                    frameIndex = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                    cmd.CommandText = string.Format("INSERT INTO {0} (\"{1}\") VALUES ('{2}')",
                                                    DbText.TablePrefix_FrameIndex + lotIndex.ToString(),
                                                    DbText.Column_FrameName,
                                                    frameName);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    frameIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                    reader.Close();
                }
                cmd.Dispose();
                sqlCon.Close();
            }
        }

        public static void AddInspectionData(string dbFilePath, int lotIndex, int frameIndex, string fileSaveDirectory, List<InspectionData> list_InspectionData)
        {
            
            using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
            {
                try
                {
                    if (sqlCon.State != System.Data.ConnectionState.Open)
                    {
                        sqlCon.Open();
                    }
                    SQLiteCommand cmd = new SQLiteCommand(sqlCon);

                    cmd.CommandText = "PRAGMA synchronous = OFF;";
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand("begin", sqlCon);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE \"{1}\" = '{2}'",
                                                    DbText.Table_LotInfo,
                                                    DbText.Column_Index,
                                                    lotIndex);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    int rowCount;
                    int columnCount;
                    string productCode_LegalFileName;
                    string lotName_LegalFileName;

                    if (!reader.Read())
                    {
                        throw new Exception("数据库未找到该批次信息");
                    }
                    else
                    {
                        rowCount = Convert.ToInt32(reader[DbText.Column_RowCount]);
                        columnCount = Convert.ToInt32(reader[DbText.Column_ColumnCount]);
                        productCode_LegalFileName = LegalFileName.Get(Convert.ToString(reader[DbText.Column_ProductCode]));
                        lotName_LegalFileName = LegalFileName.Get(Convert.ToString(reader[DbText.Column_LotName]));
                    }
                    reader.Close();
                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE \"{1}\" = '{2}'",
                                                    DbText.TablePrefix_Lot + lotIndex.ToString(),
                                                    DbText.Column_FrameIndex,
                                                    frameIndex);
                    reader = cmd.ExecuteReader();
                    List<InspectionDataDb> list_InspectionDataDb = new List<InspectionDataDb>();
                    List<DefectDataDb> list_DefectDataDb = new List<DefectDataDb>();
                    while (reader.Read())
                    {
                        InspectionDataDb dataDb = new InspectionDataDb();
                        dataDb.DbIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                        dataDb.FrameIndex = Convert.ToInt32(reader[DbText.Column_FrameIndex]);
                        dataDb.RowIndex = Convert.ToInt32(reader[DbText.Column_Row]);
                        dataDb.ColumnIndex = Convert.ToInt32(reader[DbText.Column_Column]);
                        dataDb.InspectionResult = (InspectionResult)Enum.Parse(typeof(InspectionResult), Convert.ToString(reader[DbText.Column_InspectionResult]));
                        dataDb.ConcatImagePath = Convert.ToString(reader[DbText.Column_ConcatImagePath]);
                        dataDb.ConcatRegionPath = Convert.ToString(reader[DbText.Column_ConcatRegionPath]);
                        dataDb.WirePath = Convert.ToString(reader[DbText.Column_WirePath]);
                        list_InspectionDataDb.Add(dataDb);
                    }
                    reader.Close();

                    if (!Directory.Exists(fileSaveDirectory + "\\" + productCode_LegalFileName + "\\" + lotName_LegalFileName))
                    {
                        Directory.CreateDirectory(fileSaveDirectory + "\\" + productCode_LegalFileName + "\\" + lotName_LegalFileName);
                    }
                    List<InspectionDataDb> list_InspectionDataDbNew = new List<InspectionDataDb>();
                    foreach (InspectionData data in list_InspectionData)
                    {
                        int dataIndex = (frameIndex - 1) * rowCount * columnCount + data.RowIndex * columnCount + data.ColumnIndex;
                        //如果在同一次写入存在多个同一位置的芯片，需要按芯片已存在处理
                        InspectionDataDb existDataDb = list_InspectionDataDbNew.Where(d => d.DbIndex == dataIndex).FirstOrDefault();
                        if (existDataDb == null)
                        {
                            existDataDb = list_InspectionDataDb.Where(d => d.DbIndex == dataIndex).FirstOrDefault();
                        }
                        int existImageCount = 0;
                        int existRegionCount = 0;
                        int existWireCount = 0;
                        if (existDataDb == null)
                        {
                            InspectionDataDb dataDb = new InspectionDataDb();
                            dataDb.DbIndex = dataIndex;
                            dataDb.FrameIndex = frameIndex;
                            dataDb.RowIndex = data.RowIndex;
                            dataDb.ColumnIndex = data.ColumnIndex;
                            dataDb.InspectionResult = data.InspectionResult;
                            dataDb.Code2D = data.Code2D;
                            
                            //保存图像
                            if (data.Image != null && data.Image.IsInitialized())
                            {
                                string imagePath = string.Format("\\{0}_{1}_{2}_0.tiff",
                                                                 frameIndex,
                                                                 dataDb.RowIndex,
                                                                 dataDb.ColumnIndex);
                                Task.Run(() => {
                                    HObject zoomImage;
                                    int zoomFactor = 4;
                                    //HOperatorSet.ZoomImageFactor(data.Image, out zoomImage, 1 / zoomFactor, 1 / zoomFactor, "constant");
                                    HOperatorSet.WriteImage(data.Image, "tiff", 0, fileSaveDirectory + "\\"
                                                                 + productCode_LegalFileName + "\\"
                                                                 + lotName_LegalFileName
                                                                 + imagePath);
                                    //zoomImage.Dispose();

                                    data.Image.Dispose();
                                });
                                
                                dataDb.ConcatImagePath = imagePath;
                      
                            }

                            
                            //保存区域
                            if(data.Region != null && data.Region.IsInitialized())
                            {
                                string regionPath = string.Format("\\{0}_{1}_{2}_0.reg",
                                                                  frameIndex,
                                                                  dataDb.RowIndex,
                                                                  dataDb.ColumnIndex);
                                Task.Run(() =>
                                {
                                    //2020.12.06
                                    HObject zoomRegion;
                                    int zoomFactor = 4;
                                    //HOperatorSet.ZoomRegion(data.Region, out zoomRegion, 1 / zoomFactor, 1 / zoomFactor);
                                    HOperatorSet.WriteRegion(data.Region, fileSaveDirectory + "\\"
                                                                        + productCode_LegalFileName + "\\"
                                                                        + lotName_LegalFileName
                                                                        + regionPath);
                                    //zoomRegion.Dispose();
                                    data.Region.Dispose();
                                });
                                dataDb.ConcatRegionPath = regionPath;
                            }
                            
                            //保存金线区
                            if (data.Wire != null && data.Wire.IsInitialized())
                            {
                                string wirePath = string.Format("\\{0}_{1}_{2}_0_wire.reg",
                                                                frameIndex,
                                                                dataDb.RowIndex,
                                                                dataDb.ColumnIndex);
                                Task.Run(() =>
                                {
                                    //2020.12.06
                                    HObject zoomRegion;
                                    int zoomFactor = 4;
                                    HOperatorSet.WriteRegion(data.Wire, fileSaveDirectory + "\\"
                                                                        + productCode_LegalFileName + "\\"
                                                                        + lotName_LegalFileName
                                                                        + wirePath);

                                    data.Wire.Dispose();
                                    //HOperatorSet.ZoomRegion(data.Wire, out zoomRegion, 1 / zoomFactor, 1 / zoomFactor);
                                    //HOperatorSet.WriteContourXldDxf(data.Wire, fileSaveDirectory + "\\"
                                    //                                     + productCode_LegalFileName + "\\"
                                    //                                     + lotName_LegalFileName
                                    //                                     + wirePath);
                                    ////zoomRegion.Dispose();
                                    //data.Wire.Dispose();
                                });
                                dataDb.WirePath = wirePath;
                            }
                            
                            list_InspectionDataDbNew.Add(dataDb);

                        }
                        else
                        {
                            if (data.InspectionResult != InspectionResult.SKIP || 
                                data.InspectionResult != InspectionResult.OK)
                            {
                                if (existDataDb.InspectionResult == InspectionResult.OK)
                                {
                                    existDataDb.InspectionResult = data.InspectionResult;
                                }
                            }

                            if (data.Image != null && data.Image.IsInitialized())
                            {
                                string[] existImagePath = existDataDb.ConcatImagePath.Split(';');
                           
                                if (!existImagePath[0].Equals(string.Empty))
                                {
                                    existImageCount = existImagePath.Count();
                                    string imagePath = string.Format("\\{0}_{1}_{2}_{3}.tiff",
                                                                      frameIndex,
                                                                      existDataDb.RowIndex,
                                                                      existDataDb.ColumnIndex,
                                                                      existImageCount);
                                    Task.Run(() =>
                                    {
                                        HOperatorSet.WriteImage(data.Image, "tiff", 0, fileSaveDirectory + "\\"
                                                                                 + productCode_LegalFileName + "\\"
                                                                                 + lotName_LegalFileName
                                                                                 + imagePath);
                                        data.Image.Dispose();
                                    });
                                    existDataDb.ConcatImagePath += ";" + imagePath;
                       
                                }
                                else
                                {
                                    existImageCount = existImagePath.Count();
                                    string imagePath = string.Format("\\{0}_{1}_{2}_0.tiff",
                                                                     frameIndex,
                                                                     existDataDb.RowIndex,
                                                                     existDataDb.ColumnIndex);
                                    Task.Run(() =>
                                    {
                                        HOperatorSet.WriteImage(data.Image, "tiff", 0, fileSaveDirectory + "\\"
                                                                                     + productCode_LegalFileName + "\\"
                                                                                     + lotName_LegalFileName
                                                                                     + imagePath);
                                        data.Image.Dispose();
                                    });
                                    existDataDb.ConcatImagePath = imagePath;
          
                                }
                                
                            }

                            string[] existRegionPath = existDataDb.ConcatRegionPath.Split(';');
                            if (!existRegionPath[0].Equals(string.Empty))
                            {
                                existRegionCount = existRegionPath.Count();
                            }
                            if (data.Region != null && data.Region.IsInitialized())
                            {
                                string[] regionPath = new string[existRegionCount + 1];
                                for (int i = 0; i < existRegionCount; i++)
                                {
                                    regionPath[i] = existRegionPath[i];
                                }
                                regionPath[existRegionCount] = string.Format("\\{0}_{1}_{2}_{3}.reg",
                                                                             frameIndex,
                                                                             existDataDb.RowIndex,
                                                                             existDataDb.ColumnIndex,
                                                                             existRegionCount);
                                Task.Run(() =>
                                {
                                    HOperatorSet.WriteRegion(data.Region, fileSaveDirectory + "\\"
                                                                    + productCode_LegalFileName + "\\"
                                                                    + lotName_LegalFileName
                                                                    + regionPath[existRegionCount]);
                                    data.Region.Dispose();
                                });
                                existDataDb.ConcatRegionPath = string.Join(";", regionPath);
                            }

                            string[] existWirePath = existDataDb.WirePath.Split(';');
                            if (!existWirePath[0].Equals(string.Empty))
                            {
                                existWireCount = existWirePath.Count();
                            }
                            if (data.Wire != null && data.Wire.IsInitialized())
                            {
                                string[] wirePath = new string[existWireCount + 1];
                                for (int i = 0; i < existWireCount; i++)
                                {
                                    wirePath[i] = existWirePath[i];
                                }
                                wirePath[existWireCount] = string.Format("\\{0}_{1}_{2}_{3}_wire.reg",
                                                                         frameIndex,
                                                                         existDataDb.RowIndex,
                                                                         existDataDb.ColumnIndex,
                                                                         existWireCount);
                                Task.Run(() =>
                                {
                                    HOperatorSet.WriteRegion(data.Wire, fileSaveDirectory + "\\"
                                                                         + productCode_LegalFileName + "\\"
                                                                         + lotName_LegalFileName
                                                                         + wirePath[existRegionCount]);
                                    //HOperatorSet.WriteContourXldDxf(data.Wire, fileSaveDirectory + "\\"
                                     //                                    + productCode_LegalFileName + "\\"
                                     //                                    + lotName_LegalFileName
                                     //                                    + wirePath[existRegionCount]);
                                    data.Wire.Dispose();
                                });
                                existDataDb.WirePath = string.Join(";", wirePath);
                            }
                            list_InspectionDataDbNew.Add(existDataDb);
                        }
                        foreach (DefectData defect in data.List_DefectData)
                        {
                            DefectDataDb defectDb = new DefectDataDb();
                            defectDb.InspectionDataDbIndex = dataIndex;
                            defectDb.DefectTypeIndex = defect.DefectTypeIndex;
                            defectDb.ConcatImageIndex = existImageCount;
                            defectDb.ImageIndex = defect.ImageIndex;
                            defectDb.ConcatRegionIndex = existRegionCount;
                            defectDb.RegionIndex = data.List_DefectData.IndexOf(defect) + 1;
                            defectDb.ErrorDetail = defect.ErrorDetail.S;
                            defectDb.RowIndex = data.RowIndex;
                            defectDb.ColumnIndex = data.ColumnIndex;
                            defectDb.Result = 0;
                            list_DefectDataDb.Add(defectDb);
                        }
                    }
                    list_InspectionDataDb = list_InspectionDataDb.OrderBy(d => d.DbIndex).ToList();

                    StringBuilder sb = new StringBuilder();

                    foreach (InspectionDataDb dataDb in list_InspectionDataDbNew)
                    {
                        sb.Append(string.Format("INSERT OR REPLACE INTO {0} (\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\")" +
                                                         "VALUES (\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\"); ",
                                                 DbText.TablePrefix_Lot + lotIndex.ToString(),
                                                 DbText.Column_Index,
                                                 DbText.Column_FrameIndex,
                                                 DbText.Column_Row,
                                                 DbText.Column_Column,
                                                 DbText.Column_InspectionResult,
                                                 DbText.Column_ConcatImagePath,
                                                 DbText.Column_ConcatRegionPath,
                                                 DbText.Column_WirePath,
                                                 DbText.Column_Code2D,
                                                 dataDb.DbIndex,
                                                 dataDb.FrameIndex,
                                                 dataDb.RowIndex,
                                                 dataDb.ColumnIndex,
                                                 dataDb.InspectionResult,
                                                 dataDb.ConcatImagePath,
                                                 dataDb.ConcatRegionPath,
                                                 dataDb.WirePath,
                                                 dataDb.Code2D));
                    }
                    foreach (DefectDataDb defectDb in list_DefectDataDb)
                    {
                        sb.Append(string.Format("INSERT INTO {0} (\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\")" +
                                                         "VALUES (\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\"); ",
                                                 DbText.TablePrefix_Defect + lotIndex.ToString(),
                                                 DbText.Column_FrameIndex,
                                                 DbText.Column_InspectionDataIndex,
                                                 DbText.Column_DefectTypeIndex,
                                                 DbText.Column_Row,
                                                 DbText.Column_Column,
                                                 DbText.Column_Result,
                                                 DbText.Column_ConcatImageIndex,
                                                 DbText.Column_ImageIndex,
                                                 DbText.Column_ConcatRegionIndex,
                                                 DbText.Column_RegionIndex,
                                                 DbText.Column_ErrorDetail,
                                                 frameIndex,
                                                 defectDb.InspectionDataDbIndex,
                                                 defectDb.DefectTypeIndex,
                                                 defectDb.RowIndex,
                                                 defectDb.ColumnIndex,
                                                 defectDb.Result,
                                                 defectDb.ConcatImageIndex,
                                                 defectDb.ImageIndex,
                                                 defectDb.ConcatRegionIndex,
                                                 defectDb.RegionIndex,
                                                 defectDb.ErrorDetail));
                    }
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand("end", sqlCon);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "PRAGMA synchronous = OFF;";
                    cmd.ExecuteNonQuery();

                    cmd.Dispose();
                    sqlCon.Close();
                    
                }
                catch (Exception ex)
                {
                    sqlCon.Close();
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }

        }

        public static void GetFrameIndex(string dbFilePath, string lotName, string frameName, ref int frameIndex)
        {
            using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
            {
                if (sqlCon.State != System.Data.ConnectionState.Open)
                {
                    sqlCon.Open();
                }
                SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                cmd.CommandText = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"",
                                              DbText.Table_LotInfo,
                                              DbText.Column_LotName,
                                              lotName);
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    reader.Close();
                    cmd.Dispose();
                    sqlCon.Close();
                    return;
                }              
                int lotIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                reader.Close();
                cmd.CommandText = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"",
                                       DbText.TablePrefix_FrameIndex + lotIndex.ToString(),
                                       DbText.Column_FrameName,
                                       frameName);
                reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    reader.Close();
                    cmd.Dispose();
                    sqlCon.Close();
                    return;
                }
                frameIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                reader.Close();
                cmd.Dispose();
                sqlCon.Close();
                return;      
            }
        }

        private static string InsertValues<T>(List<T> list, string tableName)
        {
            if (list == null || list.Count == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] pis = list[0].GetType().GetProperties();
            string[] parameters = new string[pis.Count()];
            for (int i = 0; i < pis.Count(); i++)
            {
                parameters[i] = string.Format("'{0}'", pis[i].Name);
            }
            string parameterString = string.Join(",", parameters);
            foreach (T item in list)
            {
                string[] values = new string[pis.Count()];
                for (int i = 0; i < pis.Count(); i++)
                {
                    values[i] = string.Format("'{0}'", pis[i].GetValue(item));
                }
                string valueString = string.Join(",", values);
                sb.Append(string.Format("INSERT INTO {0} ({1}) VALUES ({2}); ", tableName, parameterString, valueString));
            }
            return sb.ToString();
        }

        private static string InsertValues<T>(T item, string tableName)
        {
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] pis = item.GetType().GetProperties();
            string[] parameters = new string[pis.Count()];
            for (int i = 0; i < pis.Count(); i++)
            {
                parameters[i] = string.Format("'{0}'", pis[i].Name);
            }
            string parameterString = string.Join(",", parameters);
            string[] values = new string[pis.Count()];
            for (int i = 0; i < pis.Count(); i++)
            {
                values[i] = string.Format("'{0}'", pis[i].GetValue(item));
            }
            string valueString = string.Join(",", values);
            sb.Append(string.Format("INSERT INTO {0} ({1}) VALUES ({2}); ", tableName, parameterString, valueString));
            return sb.ToString();
        }

        public static string ReplaceValues<T>(T item, string tableName)
        {
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] pis = item.GetType().GetProperties();
            string[] parameters = new string[pis.Count()];
            for (int i = 0; i < pis.Count(); i++)
            {
                parameters[i] = string.Format("'{0}'", pis[i].Name);
            }
            string parameterString = string.Join(",", parameters);
            string[] values = new string[pis.Count()];
            for (int i = 0; i < pis.Count(); i++)
            {
                values[i] = string.Format("'{0}'", pis[i].GetValue(item));
            }
            string valueString = string.Join(",", values);
            sb.Append(string.Format("Replace INTO {0} ({1}) VALUES ({2})", tableName, parameterString, valueString));
            return sb.ToString();
        }

        /// <summary>
        /// 复看编辑错误码 同一颗重复编辑会覆盖之前的
        /// 2019.11.25修改增加对InspectResult的修改 从OK改成K2N
        /// </summary>
        /// <param name="dbFilePath"></param>
        /// <param name="lotIndex"></param>
        /// <param name="frameIndex"></param>
        /// <param name="Row"></param>
        /// <param name="Col"></param>
        /// <param name="Err"></param>错误码_错误说明
        public static void UpdateErrCode(string dbFilePath, int lotIndex, string frameName, int Row, int Col, string Err)
        {

            using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
            {
                try
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
                        return;
                    }
                    int frameIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                    reader.Close();

                    //如果不存在 就创建表 ReviewEdit_XX 
                    cmd.CommandText = string.Format("CREATE TABLE IF NOT EXISTS {0} (" + "'{1}'integer NOT NULL ," +
                                                     "'{2}' Text," + "'{3}'integer NOT NULL ," + "'{4}'integer NOT NULL ," +
                                                     "'{5}'integer NOT NULL ," + "'{6}'Text );",
                                                     DbText.ReviewEdit + lotIndex,
                                                     DbText.Column_FrameIndex,
                                                     DbText.Column_FrameName,
                                                     DbText.Column_Row,
                                                     DbText.Column_Column,
                                                     "ErrCode",
                                                     "ErrString"
                                                     );

                    cmd.ExecuteNonQuery();

                    //判断是否存在已经复看过OK-NG的
                    cmd.CommandText = string.Format("  SELECT COUNT(*) FROM \"{0}\" WHERE \"{1}\" =  \"{2}\" AND \"{3}\" =  \"{4}\" AND \"{5}\" =  \"{6}\" ",
                                                    DbText.ReviewEdit + lotIndex,
                                                    DbText.Column_FrameIndex,
                                                    frameIndex,
                                                    DbText.Column_Row,
                                                    Row,
                                                    DbText.Column_Column,
                                                    Col);

                    string userCount = cmd.ExecuteScalar().ToString();

                    String[] s = Err.Split(new char[] { '_' });

                    if (userCount == "0")
                    {
                        cmd.CommandText = string.Format("INSERT INTO {0} (\"{1}\", \"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\")" +
                                                                "VALUES(\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\" );",
                                                         DbText.ReviewEdit + lotIndex,
                                                         DbText.Column_FrameIndex,
                                                         DbText.Column_FrameName,
                                                         DbText.Column_Row,
                                                         DbText.Column_Column,
                                                         "ErrCode",
                                                         "ErrString",
                                                         frameIndex,
                                                         frameName,
                                                         Row,
                                                         Col,
                                                         Convert.ToInt32(s[0]),
                                                         s[1]);

                        cmd.ExecuteNonQuery();

                        //将Lot_XX中的InspectResult 修改为 K2N
                        cmd.CommandText = string.Format("UPDATE \"{0}\" SET \"{1}\"=\"{2}\"  " +
                                                        " WHERE \"{3}\" =  \"{4}\" AND \"{5}\" =  \"{6}\" AND \"{7}\" =  \"{8}\" ",
                                                        DbText.TablePrefix_Lot + lotIndex,
                                                        DbText.Column_InspectionResult,
                                                        "K2N",
                                                        DbText.Column_FrameIndex,
                                                        frameIndex,
                                                        DbText.Column_Row,
                                                        Row,
                                                        DbText.Column_Column,
                                                        Col);
                        cmd.ExecuteNonQuery();

                        System.Windows.MessageBox.Show("复看数据成功\n当前错误为："+ Err);
                    }
                    else
                    {
                        cmd.CommandText = string.Format("  UPDATE \"{0}\"  SET \"{1}\"=\"{2}\",\"{3}\"=\"{4}\" " +
                                                        " WHERE \"{5}\" =  \"{6}\" AND \"{7}\" =  \"{8}\" AND \"{9}\" =  \"{10}\" ",
                                                     DbText.ReviewEdit + lotIndex,
                                                     "ErrCode",
                                                     Convert.ToInt32(s[0]),
                                                     "ErrString",
                                                     s[1].ToString(),
                                                     DbText.Column_FrameIndex,
                                                     frameIndex,
                                                     DbText.Column_Row,
                                                     Row,
                                                     DbText.Column_Column,
                                                     Col);
                        cmd.ExecuteNonQuery();
                        System.Windows.MessageBox.Show("覆盖之前复看数据成功\n当前错误为：" + Err);
                    }

                    cmd.Dispose();
                    sqlCon.Close();
                    return;
                }
            
            catch (Exception ex)
            {
                sqlCon.Close();
                System.Windows.MessageBox.Show(ex.ToString());
            }
            }
        }
        

        /// <summary>
        /// 在加载数据时读取 Review_lot_XX中的数据，判断哪些是OK复看成NG的，进行单个刷新
        /// </summary>
        /// <returns></returns>
        public static void ReadUpdateErrCode(string dbFilePath, int lotIndex, string frameName ,out List<ReviewEditData> list_editData)
        {
            list_editData = new List<ReviewEditData>();
            using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
            {
                try
                {
                    if (sqlCon.State != System.Data.ConnectionState.Open)
                    {
                        sqlCon.Open();
                    }

                    SQLiteCommand cmd = new SQLiteCommand(sqlCon);

                    //判断是否存在该表
                    cmd.CommandText = "SELECT COUNT(*)  FROM sqlite_master WHERE TYPE='table' AND NAME='" + DbText.ReviewEdit + lotIndex + "'";
                    int Count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (Count == 0) return;
                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE {1} =\"{2}\" ",
                                                    DbText.ReviewEdit + lotIndex,
                                                    DbText.Column_FrameName,
                                                    frameName
                                                    );

                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ReviewEditData editData = new ReviewEditData();
                        editData.FrameName = frameName;
                        editData.Row = Convert.ToInt32(reader[DbText.Column_Row]);
                        editData.Col = Convert.ToInt32(reader[DbText.Column_Column]);
                        editData.ErrCode = Convert.ToInt32(reader["ErrCode"]);
                        list_editData.Add(editData);

                    }
                      
                    reader.Close();

                }
                catch (Exception ex)
                {
                    sqlCon.Close();
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }


                return ;
        }

        public static void DeleteUpdateErrCode(string dbFilePath, int lotIndex, string frameName, int Row, int Col)
        {
            using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
            {
                try
                {
                    if (sqlCon.State != System.Data.ConnectionState.Open)
                    {
                        sqlCon.Open();
                    }

                    SQLiteCommand cmd = new SQLiteCommand(sqlCon);

                    cmd.CommandText = string.Format("Delete  FROM {0} WHERE \"{1}\" =\"{2}\" AND \"{3}\" =\"{4}\" AND \"{5}\" =\"{6}\" ",
                                                    DbText.ReviewEdit + lotIndex,
                                                    DbText.Column_FrameName,
                                                    frameName,
                                                    DbText.Column_Row,
                                                    Row,
                                                    DbText.Column_Column,
                                                    Col
                                                    );
                    cmd.ExecuteNonQuery();

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
                        return;
                    }
                    int frameIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                    reader.Close();

                    //将Lot_XX中的InspectResult 修改为 OK
                    cmd.CommandText = string.Format("UPDATE \"{0}\" SET \"{1}\"=\"{2}\"  " +
                                                    " WHERE \"{3}\" =  \"{4}\" AND \"{5}\" =  \"{6}\" AND \"{7}\" =  \"{8}\" ",
                                                    DbText.TablePrefix_Lot + lotIndex,
                                                    DbText.Column_InspectionResult,
                                                    "OK",
                                                    DbText.Column_FrameIndex,
                                                    frameIndex,
                                                    DbText.Column_Row,
                                                    Row,
                                                    DbText.Column_Column,
                                                    Col);
                    cmd.ExecuteNonQuery();

                    System.Windows.MessageBox.Show("取消复看操作成功");

                }
                catch (Exception ex)
                {
                    sqlCon.Close();
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
        }

        //public static void UnlockDb(string dbFilePath)
        //{
        //    using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
        //    {
        //        if (sqlCon.State != System.Data.ConnectionState.Open)
        //        {
        //            sqlCon.Open();
        //        }
        //        SQLiteCommand cmd = new SQLiteCommand(sqlCon);
        //        cmd.CommandText = "PRAGMA synchronous = On;";
        //        cmd.ExecuteNonQuery();
        //        cmd = new SQLiteCommand("end", sqlCon);
        //        cmd.ExecuteNonQuery();
        //        cmd.Dispose();
        //        sqlCon.Close();
        //    }

        //}

        #endregion

        #region Review
        public static bool GetProductDb(string dbFilePath, out string productCode)
        {
            bool result = false;
            productCode = string.Empty;
            try
            {
                using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3"))
                {
                    if (sqlCon.State != System.Data.ConnectionState.Open)
                    {
                        sqlCon.Open();
                    }
                    SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                    cmd.CommandText = string.Format("SELECT * FROM {0} LIMIT 1",
                                                      DbText.Table_LotInfo,
                                                      DbText.Table_LotInfo);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        productCode = Convert.ToString(reader[DbText.Column_ProductCode]);
                        result = true;
                    }
                    reader.Close();
                    cmd.Dispose();
                    sqlCon.Close();

                }
            }
            catch { }
            return result;
        }

        public static bool GetLot(string dbFilePath, out List<string> list_LotName)
        {
            bool result = false;
            list_LotName = new List<string>();
            try
            {
                using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
                {
                    if (sqlCon.State != System.Data.ConnectionState.Open)
                    {
                        sqlCon.Open();
                    }
                    SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                    cmd.CommandText = string.Format("SELECT * FROM {0}", DbText.Table_LotInfo);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list_LotName.Add(Convert.ToString(reader[DbText.Column_LotName]));                      
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

        public static bool GetFrame(string dbFilePath, string lotName, out List<string> list_FrameName, ref int lotIndex, ref int rowCount, ref int columnCount)
        {
            bool result = false;
            list_FrameName = new List<string>();
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
                                                      DbText.Table_LotInfo,
                                                      DbText.Column_LotName,
                                                      lotName);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        cmd.Dispose();
                        sqlCon.Close();
                        return result;
                    }
                    lotIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                    rowCount = Convert.ToInt32(reader[DbText.Column_RowCount]);
                    columnCount = Convert.ToInt32(reader[DbText.Column_ColumnCount]);
                    reader.Close();
                    cmd.CommandText = string.Format("SELECT * FROM {0}", DbText.TablePrefix_FrameIndex + lotIndex.ToString());
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list_FrameName.Add(Convert.ToString(reader[DbText.Column_FrameName]));
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

        public static bool GetData(string dbFilePath, int lotIndex, string frameName, 
                                   out List<InspectionDataView> list_InspectionDataView, 
                                   out List<DefectDataView> list_DefectDataView,
                                   out Dictionary<int, string> dict_DefectTyoe)
        {
            bool result = false;
            list_InspectionDataView = new List<InspectionDataView>();
            list_DefectDataView = new List<DefectDataView>();
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
                        InspectionDataView dataView = new InspectionDataView();
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
                        DefectDataView defectView = new DefectDataView();
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

        public static void UpdateDefect(string dbFilePath, int lotIndex,
                                        DefectDataView defectDataView,
                                        InspectionDataView inspectionDataView = null)
        {
            try
            {
                using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
                {
                    sqlCon.Open();
                    SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Format("UPDATE {0} SET \"{1}\" = '{2}' WHERE \"{3}\" = '{4}'; ", 
                                            DbText.TablePrefix_Defect + lotIndex.ToString(),
                                            DbText.Column_Result,
                                            defectDataView.Result.ToString(),
                                            DbText.Column_Index,
                                            defectDataView.DbIndex));
                    if (inspectionDataView != null)
                    {
                        sb.Append(string.Format("UPDATE {0} SET \"{1}\" = '{2}' WHERE \"{3}\" = '{4}'; ",
                                                DbText.TablePrefix_Lot + lotIndex.ToString(),
                                                DbText.Column_InspectionResult,
                                                inspectionDataView.InspectionResult.ToString(),
                                                DbText.Column_Index,
                                                inspectionDataView.DbIndex));
                    }
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    sqlCon.Close();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region Report
        public static void ReadDataBase(string dbFilePath, 
                                        int lotIndex, 
                                        int ReportModel,
                                        out LotInfoDb lotInfoDb, 
                                        out List<DefectTypeInfoReport> list_DefectTypeInfoReport,
                                        out List<FrameIndexDb> list_FrameIndexDb,
                                        out List<InspectionDataReport> list_InspectionDataReport,
                                        out List<DefectDataReport> list_DefectDataReport,
                                        out List<ReviewEditData> list_ReviewEditData)
        {
            try
            {
                using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + dbFilePath + @"; VERSION=3"))
                {
                    sqlCon.Open();
                    SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                    //读取Lot信息
                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE \"{1}\" = \"{2}\"", DbText.Table_LotInfo, DbText.Column_Index, lotIndex);
                    ReadValue(cmd, out lotInfoDb);

                    //读取DefectTypeInfo信息
                    cmd.CommandText = string.Format("SELECT * FROM {0}", DbText.Table_DefectTypeInfo);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    list_DefectTypeInfoReport = new List<DefectTypeInfoReport>();
                    while (reader.Read())
                    {
                        DefectTypeInfoReport defectTypeReport = new DefectTypeInfoReport();
                        defectTypeReport.Index = Convert.ToInt32(reader[DbText.Column_Index]);
                        defectTypeReport.DefectType = Convert.ToString(reader[DbText.Column_DefectType]);
                        list_DefectTypeInfoReport.Add(defectTypeReport);
                    }
                    reader.Close();

                    //读取FrameIndex信息
                    cmd.CommandText = string.Format("SELECT * FROM {0}", DbText.TablePrefix_FrameIndex + lotIndex.ToString());
                    ReadValue(cmd, out list_FrameIndexDb);

                    string productCode_LegalFileName = LegalFileName.Get(lotInfoDb.ProductCode);
                    string lotName_LegalFileName = LegalFileName.Get(lotInfoDb.LotName);

                    //读取批次数据
                    cmd.CommandText = string.Format("SELECT * FROM {0}", DbText.TablePrefix_Lot + lotIndex.ToString());
                    reader = cmd.ExecuteReader();
                    list_InspectionDataReport = new List<InspectionDataReport>();
                    while (reader.Read())
                    {
                        InspectionDataReport dataReport = new InspectionDataReport();
                        dataReport.DbIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                        dataReport.RowIndex = Convert.ToInt32(reader[DbText.Column_Row]);
                        dataReport.ColumnIndex = Convert.ToInt32(reader[DbText.Column_Column]);
                        dataReport.FrameIndex = Convert.ToInt32(reader[DbText.Column_FrameIndex]);
                        dataReport.InspectionResult = (InspectionResult)Enum.Parse(typeof(InspectionResult), Convert.ToString(reader[DbText.Column_InspectionResult]));

                        string[] concatImagePaths = Convert.ToString(reader[DbText.Column_ConcatImagePath]).Split(';');
                        if (!string.IsNullOrEmpty(concatImagePaths[0]))
                        {
                            foreach (string concatImagePath in concatImagePaths)
                            {
                                dataReport.ConcatImagePath.Add("\\" + productCode_LegalFileName + 
                                                               "\\" + lotName_LegalFileName
                                                                    + concatImagePath);
                            }
                        }

                        string[] concatRegionPaths = Convert.ToString(reader[DbText.Column_ConcatRegionPath]).Split(';');
                        if (!string.IsNullOrEmpty(concatRegionPaths[0]))
                        {
                            foreach (string concatRegionPath in concatRegionPaths)
                            {
                                dataReport.ConcatRegionPath.Add("\\" + productCode_LegalFileName +
                                                                "\\" + lotName_LegalFileName
                                                                     + concatRegionPath);
                            }
                        }

                        string[] WirePaths = Convert.ToString(reader[DbText.Column_WirePath]).Split(';');
                        if (!string.IsNullOrEmpty(WirePaths[0]))
                        {
                            foreach (string wirePath in WirePaths)
                            {
                                dataReport.WirePath.Add("\\" + productCode_LegalFileName +
                                                        "\\" + lotName_LegalFileName
                                                             + wirePath);
                            }
                        }
                        if (ReportModel == 1)
                        {
                            string Code2D = Convert.ToString(reader[DbText.Column_Code2D]);
                            dataReport.Code2D = Code2D;
                        }
                        list_InspectionDataReport.Add(dataReport);
                    }
                    reader.Close();


                    //读取缺陷数据
                    cmd.CommandText = string.Format("SELECT * FROM {0}", DbText.TablePrefix_Defect + lotIndex.ToString());
                    reader = cmd.ExecuteReader();
                    list_DefectDataReport = new List<DefectDataReport>();
                    while (reader.Read())
                    {
                        DefectDataReport defectReport = new DefectDataReport();
                        defectReport.DbIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                        defectReport.InspectionDataDbIndex = Convert.ToInt32(reader[DbText.Column_InspectionDataIndex]);
                        defectReport.DefectTypeIndex = Convert.ToInt32(reader[DbText.Column_DefectTypeIndex]);
                        defectReport.RowIndex = Convert.ToInt32(reader[DbText.Column_Row]);
                        defectReport.ColumnIndex = Convert.ToInt32(reader[DbText.Column_Column]);
                        defectReport.FrameIndex = Convert.ToInt32(reader[DbText.Column_FrameIndex]);
                        defectReport.Result = Convert.ToInt32(reader[DbText.Column_Result]);
                        defectReport.ConcatImageIndex = Convert.ToInt32(reader[DbText.Column_ConcatImageIndex]);
                        defectReport.ImageIndex = Convert.ToInt32(reader[DbText.Column_ImageIndex]);
                        defectReport.ConcatRegionIndex = Convert.ToInt32(reader[DbText.Column_ConcatRegionIndex]);
                        defectReport.RegionIndex = Convert.ToInt32(reader[DbText.Column_RegionIndex]);
                        defectReport.ErrorDetail = Convert.ToString(reader[DbText.Column_ErrorDetail]);
                        list_DefectDataReport.Add(defectReport);
                    }
                    reader.Close();

                    //读取ReviewEdit数据
                    cmd.CommandText = "SELECT COUNT(*)  FROM sqlite_master WHERE TYPE='table' AND NAME='" + DbText.ReviewEdit + lotIndex + "'";
                    int Count = Convert.ToInt32(cmd.ExecuteScalar());
                    list_ReviewEditData = new List<ReviewEditData>();
                    if (Count == 0)
                    {

                    }
                    else
                    {


                        cmd.CommandText = string.Format("SELECT * FROM {0}  ",
                                                        DbText.ReviewEdit + lotIndex
                                                        );

                        reader = cmd.ExecuteReader();
                        
                        while (reader.Read())
                        {
                            ReviewEditData editData = new ReviewEditData();
                            editData.FrameLotIndex = Convert.ToInt32(reader[DbText.Column_FrameIndex]);
                            editData.FrameName = Convert.ToString(reader[DbText.Column_FrameName]);
                            editData.Row = Convert.ToInt32(reader[DbText.Column_Row]);
                            editData.Col = Convert.ToInt32(reader[DbText.Column_Column]);
                            editData.ErrCode = Convert.ToInt32(reader["ErrCode"]);
                            list_ReviewEditData.Add(editData);

                        }

                        reader.Close();
                    }
                    cmd.Dispose();
                    sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


   
        private static void ReadValue<T>(SQLiteCommand cmd, out List<T> list)
        {
            SQLiteDataReader reader = cmd.ExecuteReader();
            list = new List<T>();
            while (reader.Read())
            {
                T item = Activator.CreateInstance<T>();
                PropertyInfo[] pis = item.GetType().GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    switch (pi.PropertyType.Name)
                    {
                        case "String":
                            pi.SetValue(item, Convert.ToString(reader[pi.Name]));
                            break;
                        case "Int32":
                            pi.SetValue(item, Convert.ToInt32(reader[pi.Name]));
                            break;
                        case "Double":
                            pi.SetValue(item, Convert.ToDouble(reader[pi.Name]));
                            break;
                        case "Boolean":
                            pi.SetValue(item, Convert.ToBoolean(Convert.ToInt32(reader[pi.Name])));
                            break;
                        default:
                            if (pi.PropertyType.IsEnum)
                            {
                                pi.SetValue(item, Enum.Parse(pi.PropertyType, Convert.ToString(reader[pi.Name])));
                            }
                            break;
                    }
                }
                list.Add(item);
            }
            reader.Close();
        }

        private static void ReadValue<T>(SQLiteCommand cmd, out T item)
        {
            item = Activator.CreateInstance<T>();
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                return;
            }
            PropertyInfo[] pis = item.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                switch (pi.PropertyType.Name)
                {
                    case "String":
                        pi.SetValue(item, Convert.ToString(reader[pi.Name]));
                        break;
                    case "Int32":
                        pi.SetValue(item, Convert.ToInt32(reader[pi.Name]));
                        break;
                    case "Double":
                        pi.SetValue(item, Convert.ToDouble(reader[pi.Name]));
                        break;
                    case "Boolean":
                        pi.SetValue(item, Convert.ToBoolean(Convert.ToInt32(reader[pi.Name])));
                        break;
                    default:
                        if (pi.PropertyType.IsEnum)
                        {
                            Type enumType = Type.GetType(pi.PropertyType.Name);
                            pi.SetValue(item, Enum.Parse(enumType, Convert.ToString(reader[pi.Name])));
                        }
                        break;
                }
            }
            reader.Close();
        }
        #endregion
    }
}
