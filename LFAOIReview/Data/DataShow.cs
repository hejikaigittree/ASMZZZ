using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    public  class DataShow
    {
        public  int LotIndex;
        public  int rowCount;
        public  int columnCount;
        public  string frameName;
        public string imageDirectory;
        public string DbFilePath;
        public string product;
        public string lotName;

        public class ModifyDbData
        {
            public int RowIndex;
            public int ColumnIndex;
            public string InspectionResult;
        }

        /// <summary>
        /// 修改之后的所有检测结果
        /// </summary>
        public  List<ModifyDbData> List_ModifyDbData = new List<ModifyDbData>();

        /// <summary>
        /// 从NG改成OK的芯片
        /// </summary>
        public List<ModifyDbData> List_IncrementModify = new List<ModifyDbData>();

        /// <summary>
        /// 修改后NG的芯片
        /// </summary>
        public List<ModifyDbData> List_NG = new List<ModifyDbData>();

        public  void ShowInspectData(string DbFilePath, string _frameName, string lotName,string _imageDirectory,string _product)
        {
            List<string> list_FrameName;
            SQLiteOperation.GetFrame(DbFilePath, lotName, out list_FrameName, ref LotIndex, ref rowCount, ref columnCount);

            this.frameName = _frameName;
            this.lotName = lotName;
            this.imageDirectory = _imageDirectory;
            this.DbFilePath = DbFilePath;
            this.product = _product;
        }
        public bool ReadModifyDb()
        {
            try
            {
                using (SQLiteConnection sqlCon = new SQLiteConnection(@"DATA SOURCE=" + DbFilePath + @"; VERSION=3; UseUTF8Encoding = True"))
                {
                    if (sqlCon.State != System.Data.ConnectionState.Open)
                    {
                        sqlCon.Open();
                    }
                    SQLiteCommand cmd = new SQLiteCommand(sqlCon);
                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"",
                                                       DbText.TablePrefix_FrameIndex + LotIndex.ToString(),
                                                       DbText.Column_FrameName,
                                                       frameName);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        cmd.Dispose();
                        sqlCon.Close();
                        return false;
                    }
                    int frameIndex = Convert.ToInt32(reader[DbText.Column_Index]);
                    reader.Close();

                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"",
                                                    DbText.TablePrefix_Lot + LotIndex.ToString(),
                                                    DbText.Column_FrameIndex,
                                                    frameIndex);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ModifyDbData dataView = new ModifyDbData();
                       
                        dataView.RowIndex = Convert.ToInt32(reader[DbText.Column_Row]);
                        dataView.ColumnIndex = Convert.ToInt32(reader[DbText.Column_Column]);
                        dataView.InspectionResult =  Convert.ToString(reader[DbText.Column_InspectionResult]);
                        List_ModifyDbData.Add(dataView);
                        if (dataView.InspectionResult=="N2K")
                        {
                            List_IncrementModify.Add(dataView);
                        }
                        if(dataView.InspectionResult == "NG")
                        {
                            List_NG.Add(dataView);
                        }

                    }
                }
            }
            catch
            {
                return false;
            }


            return true;
        }

    }

    
}
