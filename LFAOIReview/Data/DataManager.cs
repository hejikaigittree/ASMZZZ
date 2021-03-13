using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LFAOIReview
{
    public class DataManager
    {
        /// <summary>
        /// 数据库存放文件夹
        /// </summary>
        public static string DataBaseDirectory { get; set; } = Application.StartupPath;

        /// <summary>
        /// 图像文件存放文件夹
        /// </summary>
        public static string FileSaveDirectory { get; set; } = Application.StartupPath;

        /// <summary>
        /// 当前批次
        /// </summary>
        public static string CurrentLot { get; private set; }

        /// <summary>
        /// 当前盘
        /// </summary>
        public static string CurrentFrame { get; private set; }

        /// <summary>
        /// 当前批次序号
        /// </summary>
        public static int CurrentLotIndex { get { return LotIndex; } }

        /// <summary>
        /// 当前盘序号
        /// </summary>
        public static int CurrentFrameIndex { get { return FrameIndex; } }

        private static string DbFilePath;
        private static int LotIndex = 0;
        private static int FrameIndex = 0;

        /// <summary>
        /// 初始化批次信息，同时输入缺陷列表
        /// </summary>
        /// <param name="lotInfo">批次信息</param>
        /// <param name="list_DefectTpe">缺陷列表</param>
        public static int InitialNewLot(LotInfo lotInfo, List<DefectTypeInfo> list_DefectTpe)
        {
            if (!Directory.Exists(DataBaseDirectory))
            {
                Directory.CreateDirectory(DataBaseDirectory);
            }
            CurrentLot = lotInfo.LotName;
            string dbFileName = LegalFileName.Get(lotInfo.ProductCode);

            DbFilePath = DataBaseDirectory + "\\" + dbFileName + ".db";

            SQLiteOperation.InitialDataBase(DbFilePath, lotInfo, list_DefectTpe, out LotIndex);
            return LotIndex;
        }

        /// <summary>
        /// 批次开始工作，记录当前时间
        /// </summary>
        public static void StartLot()
        {
            if (LotIndex == 0)
            {
                throw new Exception("请先初始化批次");
            }
            SQLiteOperation.UpdateValues(DbFilePath, LotIndex,
                              new List<string>
                              {
                                                  DbText.Column_StartDate,
                                                  DbText.Column_StartTime
                              },
                              new List<string>
                              {
                                                  DateTime.Now.ToString("yyyy/MM/dd"),
                                                  DateTime.Now.ToString("HH:mm:ss")
                              }
                            );
            IsWritingLot = true;
        }

        /// <summary>
        /// 批次结束工作，记录当前时间
        /// </summary>
        public static void EndLot()
        {
            if (LotIndex == 0)
            {
                throw new Exception("请先初始化批次");
            }
            SQLiteOperation.UpdateValues(DbFilePath, LotIndex,
                              new List<string>
                              {
                                                  DbText.Column_EndDate,
                                                  DbText.Column_EndTime
                              },
                              new List<string>
                              {
                                                  DateTime.Now.ToString("yyyy/MM/dd"),
                                                  DateTime.Now.ToString("HH:mm:ss")
                              }
                            );
            IsWritingLot = false;
        }

        /// <summary>
        /// 初始化盘信息
        /// </summary>
        /// <param name="frameName"></param>
        public static int InitialNewFrame(string frameName)
        {
            if (LotIndex == 0)
            {
                throw new Exception("请先初始化批次");
            }
            CurrentFrame = frameName;
            SQLiteOperation.InitialFrame(DbFilePath, LotIndex, frameName, out FrameIndex);
            return FrameIndex;
        }

        /// <summary>
        /// 添加检测数据
        /// </summary>
        /// <param name="list_InspectionData"></param>
        public static void AddInspectionData(List<InspectionData> list_InspectionData ,InspectionDataEx DataEx)
        {
            if (FrameIndex == 0)
            {
                throw new Exception("请先初始化盘");
            }

            foreach (InspectionData inspectionData in list_InspectionData)
            {
                if (inspectionData.InspectionResult == InspectionResult.NG && (inspectionData.List_DefectData.Count == 0 || inspectionData.List_DefectData==null))
                {
                    MessageBox.Show("测试结果信息异常---LotID:" + inspectionData.LotID.ToString() + " FrameID:" + inspectionData.FrameID.ToString() + " IC Row:" +
                        inspectionData.RowIndex.ToString() + " IC Col:" + inspectionData.ColumnIndex.ToString() + "Region Count:" + inspectionData.Region.CountObj().ToString());
                }
            }

            if (list_InspectionData.Count == 0) return;
            SQLiteOperation.AddInspectionData(DbFilePath, DataEx.LotIndex, DataEx.FrameIndex, FileSaveDirectory, list_InspectionData);
        }

        public static void AddInspectionData(List<InspectionData> list_InspectionData)
        {
            if (FrameIndex == 0)
            {
                throw new Exception("请先初始化盘");
            }
            if (list_InspectionData.Count == 0) return;
           
            SQLiteOperation.AddInspectionData(DbFilePath, LotIndex, FrameIndex, FileSaveDirectory, list_InspectionData);
        }

        /// <summary>
        /// 根据产品编号、批次名、盘名查询盘序号
        /// </summary>
        /// <param name="productCode">产品编号</param>
        /// <param name="lotName">批次名</param>
        /// <param name="frameName">盘名</param>
        /// <returns>盘序号，-1为未查到</returns>
        public static int GetFrameIndex(string productCode, string lotName, string frameName)
        {
            int frameIndex = -1;
            string dbFilePath = DataBaseDirectory + "\\" + LegalFileName.Get(productCode) + ".db";
            if (File.Exists(dbFilePath))
            {
                SQLiteOperation.GetFrameIndex(dbFilePath, lotName, frameName, ref frameIndex);
            }      
            return frameIndex;
        }   

        /// <summary>
        /// 解锁因异常断开导致的Sqlite数据库锁定
        /// </summary>
        //public static void UnlockDb()
        //{
        //    if (!File.Exists(DbFilePath))
        //    {
        //        return;
        //    }
        //    SQLiteOperation.UnlockDb(DbFilePath);
        //}

        /// <summary>
        /// 批次运行标志，当StartLot后为true，EndLot后为false
        /// </summary>
        public static bool IsWritingLot { get; private set; } = false;

        
    }
}
