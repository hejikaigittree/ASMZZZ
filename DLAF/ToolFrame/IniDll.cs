using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace DLAF
{
    /// <summary>
    /// IniFiles的类
    /// </summary>
    public class IniFiles
    {
        private string FileName; //INI文件名
        //声明读写INI文件的API函数

        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);


        //类的构造函数，传递INI文件名
        public IniFiles(string AFileName)
        {
            // 判断文件是否存在
            FileInfo fileInfo = new FileInfo(AFileName);
            //Todo:搞清枚举的用法
            if ((!fileInfo.Exists))
            { //|| (FileAttributes.Directory in fileInfo.Attributes))
                //文件不存在，建立文件
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(AFileName, false, System.Text.Encoding.Default);
                try
                {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                    WritePrivateProfileString("ThisFileName", "ThisFileName", fileInfo.Name, AFileName);
                    //sw.Write("#" + fileInfo.Name);
                    //sw.Close();
                }
                catch
                {
                }
            }
            //必须是完全路径，不能是相对路径
            FileName = fileInfo.FullName;
        }
        //写入字符串
        public bool WriteString(string Section, string Ident, string Value)
        {
            return WritePrivateProfileString(Section, Ident, Value, FileName);
        }
        //读取字符串
        public bool ReadString(string Section, string Ident, out string Value)
        {
            try
            {
                string Default = "";
                Byte[] Buffer = new Byte[65535];
                int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), FileName);
                //必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
                string s = Encoding.GetEncoding(0).GetString(Buffer);
                Value = (s.Substring(0, bufLen)).Trim();
                return true;
            }
            catch (Exception)
            {
                Value = "";
                return false;
            }
        }

        //读取字符串
        public bool ReadString(string Section, string Ident, string FileName, out string Value)
        {
            try
            {
                string Default = "";
                Byte[] Buffer = new Byte[65535];
                int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), FileName);
                //必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
                string s = Encoding.GetEncoding(0).GetString(Buffer);
                Value = (s.Substring(0, bufLen)).Trim();
                return true;
            }
            catch (Exception)
            {
                Value = "";
                return false;
            }
        }

        //读整数
        public bool ReadInteger(string Section, string Ident, out int Value)
        {
            string intStr = "";
            try
            {
                if (ReadString(Section, Ident, out intStr))
                {
                    Value = Convert.ToInt32(intStr);
                    return true;
                }
                else
                {
                    Value = 0;
                    return false;
                }
            }
            catch (Exception)
            {
                Value = 0;
                return false;
            }
        }



        //读整数有文件路径参数
        public bool ReadInteger(string Section, string Ident, string file, out int Value)
        {
            string intStr = "";
            try
            {
                if (ReadString(Section, Ident, file, out intStr))
                {
                    Value = Convert.ToInt32(intStr);
                    return true;
                }
                else
                {
                    Value = 0;
                    return false;
                }
            }
            catch (Exception)
            {
                Value = 0;
                return false;
            }
        }



        //写整数有文件路径参数
        public bool WriteInteger(string Section, string Ident, string file, int Value)
        {
            return WritePrivateProfileString(Section, Ident, Value.ToString(), file); ;
        }

        //写整数
        public bool WriteInteger(string Section, string Ident, int Value)
        {
            return WriteString(Section, Ident, Value.ToString());
        }

        //读浮点数
        public bool ReadDouble(string Section, string Ident, out double Value)
        {
            string doubleStr = "";
            try
            {
                if (ReadString(Section, Ident, out doubleStr))
                {
                    Value = Convert.ToDouble(doubleStr);
                    return true;
                }
                else
                {
                    Value = 0;
                    return false;
                }
            }
            catch (Exception)
            {
                Value = 0;
                return false;
            }
        }

        //写浮点数
        public bool Writedouble(string Section, string Ident, double Value)
        {
            return WriteString(Section, Ident, Value.ToString());
        }

        //读布尔
        public bool ReadBool(string Section, string Ident, out bool Value)
        {
            try
            {
                string intStr = "";
                if (ReadString(Section, Ident, out intStr))
                {
                    Value = Convert.ToBoolean(intStr);
                    return true;
                }
                else
                {
                    Value = false;
                    return false;
                }
            }
            catch (Exception)
            {
                Value = false;
                return false;
            }
        }

        //写布尔
        public bool WriteBool(string Section, string Ident, bool Value)
        {
            return WriteString(Section, Ident, Convert.ToString(Value));
        }

        /// <summary>
        /// 保存类内所有int,double,bool,string公共变量
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SaveObj(object obj)
        {
            System.Reflection.FieldInfo[] infos = obj.GetType().GetFields();

            foreach (System.Reflection.FieldInfo fi in infos)
            {
                switch (fi.FieldType.Name)
                {
                    case "Int32":
                        WriteInteger(obj.GetType().ToString(), fi.Name, (int)fi.GetValue(obj));
                        break;
                    case "double":
                        Writedouble(obj.GetType().ToString(), fi.Name, (double)fi.GetValue(obj));
                        break;
                    case "Boolean":
                        WriteBool(obj.GetType().ToString(), fi.Name, (bool)fi.GetValue(obj));
                        break;
                    case "String":
                        WriteString(obj.GetType().ToString(), fi.Name, (string)fi.GetValue(obj));
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        /// <summary>
        /// 加载窗体内所有int,double,bool,string公共变量
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool LoadObj(object obj)
        {
            System.Reflection.FieldInfo[] infos = obj.GetType().GetFields();

            foreach (System.Reflection.FieldInfo fi in infos)
            {
                switch (fi.FieldType.Name)
                {
                    case "Int32":
                        int tempInt;
                        ReadInteger(obj.GetType().ToString(), fi.Name, out tempInt);
                        fi.SetValue(obj, tempInt);
                        break;
                    case "double":
                        double tempdouble;
                        ReadDouble(obj.GetType().ToString(), fi.Name, out tempdouble);
                        fi.SetValue(obj, tempdouble);
                        break;
                    case "Boolean":
                        bool tempBool;
                        ReadBool(obj.GetType().ToString(), fi.Name, out tempBool);
                        fi.SetValue(obj, tempBool);
                        break;
                    case "String":
                        string tempString;
                        ReadString(obj.GetType().ToString(), fi.Name, out tempString);
                        fi.SetValue(obj, tempString);
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// 保存类内所有int,double,bool,string公共变量
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="Section"></param>
        /// <returns></returns>
        public bool SaveObj(object obj,string Section)
        {
            System.Reflection.FieldInfo[] infos = obj.GetType().GetFields();

            foreach (System.Reflection.FieldInfo fi in infos)
            {
                switch (fi.FieldType.Name)
                {
                    case "Int32":
                        WriteInteger(Section, fi.Name, (int)fi.GetValue(obj));
                        break;
                    case "double":
                        Writedouble(Section, fi.Name, (double)fi.GetValue(obj));
                        break;
                    case "Boolean":
                        WriteBool(Section, fi.Name, (bool)fi.GetValue(obj));
                        break;
                    case "String":
                        WriteString(Section, fi.Name, (string)fi.GetValue(obj));
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        /// <summary>
        /// 加载窗体内所有int,double,bool,string公共变量
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="Section"></param>
        /// <returns></returns>
        public bool LoadObj(object obj, string Section)
        {
            System.Reflection.FieldInfo[] infos = obj.GetType().GetFields();

            foreach (System.Reflection.FieldInfo fi in infos)
            {
                switch (fi.FieldType.Name)
                {
                    case "Int32":
                        int tempInt;
                        ReadInteger(Section, fi.Name, out tempInt);
                        fi.SetValue(obj, tempInt);
                        break;
                    case "double":
                        double tempdouble;
                        ReadDouble(Section, fi.Name, out tempdouble);
                        fi.SetValue(obj, tempdouble);
                        break;
                    case "Boolean":
                        bool tempBool;
                        ReadBool(Section, fi.Name, out tempBool);
                        fi.SetValue(obj, tempBool);
                        break;
                    case "String":
                        string tempString;
                        ReadString(Section, fi.Name, out tempString);
                        fi.SetValue(obj, tempString);
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        //从Ini文件中，将指定的Section名称中的所有Ident添加到列表中
        public void ReadSection(string Section, StringCollection Idents)
        {
            Byte[] Buffer = new Byte[16384];
            //Idents.Clear();

            int bufLen = GetPrivateProfileString(Section, null, null, Buffer, Buffer.GetUpperBound(0),
                  FileName);
            //对Section进行解析
            GetStringsFromBuffer(Buffer, bufLen, Idents);
        }

        private void GetStringsFromBuffer(Byte[] Buffer, int bufLen, StringCollection Strings)
        {
            Strings.Clear();
            if (bufLen != 0)
            {
                int start = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    if ((Buffer[i] == 0) && ((i - start) > 0))
                    {
                        String s = Encoding.GetEncoding(0).GetString(Buffer, start, i - start);
                        Strings.Add(s);
                        start = i + 1;
                    }
                }
            }
        }
        //从Ini文件中，读取所有的Sections的名称
        public void ReadSections(StringCollection SectionList)
        {
            //Note:必须得用Bytes来实现，StringBuilder只能取到第一个Section
            byte[] Buffer = new byte[65535];
            int bufLen = 0;
            bufLen = GetPrivateProfileString(null, null, null, Buffer,
             Buffer.GetUpperBound(0), FileName);
            GetStringsFromBuffer(Buffer, bufLen, SectionList);
        }
        //读取指定的Section的所有Value到列表中
        public void ReadSectionValues(string Section, NameValueCollection Values)
        {
            StringCollection KeyList = new StringCollection();
            ReadSection(Section, KeyList);
            Values.Clear();
            string val = "";
            foreach (string key in KeyList)
            {
                ReadString(Section, key, out val);
                Values.Add(key, val);
            }
        }
        ////读取指定的Section的所有Value到列表中，
        //public void ReadSectionValues(string Section, NameValueCollection Values,char splitString)
        //{　 string sectionValue;
        //　　string[] sectionValueSplit;
        //　　StringCollection KeyList = new StringCollection();
        //　　ReadSection(Section, KeyList);
        //　　Values.Clear();
        //　　foreach (string key in KeyList)
        //　　{
        //　　　　sectionValue=ReadString(Section, key, "");
        //　　　　sectionValueSplit=sectionValue.Split(splitString);
        //　　　　Values.Add(key, sectionValueSplit[0].ToString(),sectionValueSplit[1].ToString());

        //　　}
        //}
        //清除某个Section
        public void EraseSection(string Section)
        {
            if (!WritePrivateProfileString(Section, null, null, FileName))
            {
                throw (new ApplicationException("无法清除Ini文件中的Section"));
            }
        }
        //删除某个Section下的键
        public void DeleteKey(string Section, string Ident)
        {
            WritePrivateProfileString(Section, Ident, null, FileName);
        }
        //Note:对于Win9X，来说需要实现UpdateFile方法将缓冲中的数据写入文件
        //在Win NT, 2000和XP上，都是直接写文件，没有缓冲，所以，无须实现UpdateFile
        //执行完对Ini文件的修改之后，应该调用本方法更新缓冲区。
        public void UpdateFile()
        {
            WritePrivateProfileString(null, null, null, FileName);
        }

        //检查某个Section下的某个键值是否存在
        public bool ValueExists(string Section, string Ident)
        {
            StringCollection Idents = new StringCollection();
            ReadSection(Section, Idents);
            return Idents.IndexOf(Ident) > -1;
        }

        //确保资源的释放
        ~IniFiles()
        {
            UpdateFile();
        }
    }
}