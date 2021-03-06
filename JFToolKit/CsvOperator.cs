using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace JFToolKits
{
    public class CsvFileOperator : List<List<string>>
    {
        /// <summary>
        /// 从CSV文件中加载内容到数据表lst中，要求文件格式为Csv
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="filePath">文件路径</param>
        static void UpdataLLByFile(List<List<string>> lst, string filePath)
        {
            if (null == lst)
                throw new ArgumentNullException("Param:lst is null in CsvFileOperator.MaptoList( lst,filePath)");
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("Param:filePath is null in CsvFileOperator.MaptoList( lst,filePath)");
            lst.Clear();

            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.Default))
                {
                    while (!reader.EndOfStream)
                        lst.Add(_RowCells(reader.ReadLine()));
                }
            }
        }

        public List<List<string>> LLFromFile(string filePath)
        {
            List<List<string>> ret = new List<List<string>>();
            UpdataLLByFile(ret, filePath);
            return ret;
        }


        /// <summary>
        /// 将二维表数据存入csv文件
        /// </summary>
        /// <param name="lst">二维数据表对象</param>
        /// <param name="filePath">需要保存的文件路径</param>
        /// <returns></returns>
        public static bool SaveLLToFile(List<List<string>> lst, string filePath)
        {
            if (null == lst)
                throw new ArgumentNullException("Param:lst is null in CsvFileOperator.SaveLLToFile");
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("Param:filePath is null in CsvFileOperator.SaveLLToFile");

            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(dirPath);
                if (di == null || !di.Exists)
                    return false;
            }
            
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.Default))
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        List<string> rowCells = lst[i];
                        for (int j = 0; j < rowCells.Count; j++)
                        {
                            streamWriter.Write(_CsvCell(rowCells[j]));
                            if (j < rowCells.Count - 1)
                                streamWriter.Write(",");
                        }
                        if (i < lst.Count - 1)
                            streamWriter.WriteLine();
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// 将字符串转化成一行数据（csv格式）
        /// </summary>
        /// <param name="rowText"></param>
        /// <returns></returns>
        static List<string> _RowCells(string rowText) //测试OK 可以按“，”分割，
        {
            //Regex regex = new Regex("(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
            string ptSlipRow = "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)";
            MatchCollection mc = Regex.Matches(rowText, ptSlipRow);//regex.Matches(rowText);
            List<string> ret = new List<string>();
            foreach (Match m in mc)
            {
                string cellTxt = Regex.Replace(m.Value, "^\"|\"$", string.Empty); //去掉首位的 "
                cellTxt = Regex.Replace(cellTxt, "\"\"", "\"");
                ret.Add(cellTxt);
            }

            return ret;
        }

        static string _CsvCell(string orgCellTxt)
        {
            string ret = orgCellTxt;
            if (ret == null)
                ret = "";
            do
            {
                if (0 == ret.Length)
                    break;
                if (!orgCellTxt.Contains("\""))
                {
                    if (!orgCellTxt.Contains(","))
                        break;
                    //只包含逗号的字符串
                    ret = "\"" + orgCellTxt + "\"";
                    break;

                }

                ret = "\"" + orgCellTxt.Replace("\"", "\"\"") + "\"";

            }
            while (false);
            return ret;
        }


        public CsvFileOperator() : base()
        {
            FilePath = null;
        }

        public CsvFileOperator(string filePath) : base()
        {
            _CheckFilePath(filePath);
            FileInfo fi = new FileInfo(filePath);
            FilePath = fi.FullName;
            Load(FilePath);
        }

        public CsvFileOperator(int row,int col) : base()
        {
            FitRows(row);
            FitCols(col);
        }


        public CsvFileOperator(string filePath,int row, int col) : this(filePath)
        {
            FitRows(row);
            FitCols(col);
        }


        
        public string FilePath { get; private set; }

        public void Load(string filePath = null)
        {
            if (filePath == null)
            {
                filePath = FilePath;
                if (null == filePath)
                    throw new ArgumentNullException("Param:filePath is null in CsvFileOperator.Load(filePath)");
            }
            if (!File.Exists(filePath))
                throw new ArgumentNullException("Param:filePath= " + filePath + " is Unexists in CsvFileOperator.Load(filePath)");

            UpdataLLByFile(this, filePath);
            FileInfo fi = new FileInfo(filePath);
            FilePath = fi.FullName;

        }

        public bool Save(string filePath = null)
        {
            if (filePath == null)
            {
                filePath = FilePath;
                if (null == filePath)
                    throw new ArgumentNullException("Param:filePath is null in CsvFileOperator.Save(filePath)");
            }

            return SaveLLToFile(this, filePath);
        }

        public int Columns
        {
            get
            {
                int ret = 0;
                for (int i = 0; i < Count; i++)
                    if (this[i].Count > ret)
                        ret = this[i].Count;
                return ret;
            }
        }


        /// <summary>
        /// 重置数据表行数
        /// </summary>
        /// <param name="row"></param>
        public void FitRows(int row)
        {
            if (Count == row)
                return;
            else if(Count > row)
                RemoveRange(row, Count - row);
            else
            {
                int willAdd = row - Count;
                for (int i = 0; i < willAdd; i++)
                    Add(new List<string>(Columns));
                
            }

        }

        /// <summary>
        /// 重置数据表列数
        /// </summary>
        /// <param name="col"></param>
        public void FitCols(int col)
        {
            for(int i = 0; i < Count;i++)
            {
                if (this[i].Count > col)
                    this[i].RemoveRange(col, this[i].Count - col);
                else if(this[i].Count < col)
                {
                    int addCnt = col - this[i].Count;
                    for(int j = 0; j < addCnt;j++)
                    this[i].Add(null);
                }
            }
        }

        public string GetItem(int row,int col)
        {
            if (row >= Count)
                return null;
            if (col >= this[row].Count)
                return null;
            return this[row][col];
        }

        public void SetItem(int row, int col,string item)
        {
            if (row >= Count)
                FitRows(row);
            if (col >= this[row].Count)
                FitCols(col);
            this[row][col] = item;
            //XmlSerializerFile.WriteXmlFile<CsvFileOperator>(this, "hehe.xml");

        }



        //检查文件是否存在，如果不存在会创建一个
        void _CheckFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.Constructor");
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
        }





    }






}
