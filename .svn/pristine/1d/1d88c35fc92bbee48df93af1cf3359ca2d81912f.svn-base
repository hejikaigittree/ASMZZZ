using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace JFToolKits
{
    public class IniFileOperator 
    {
        #region Win32 API export
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, [In] [Out] char[] lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, string lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileSection(string lpAppName, string lpString, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        #endregion

        #region Static-API

        /// <summary>
        /// 获取ini文件中所有的Section名称
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static string[] SectionNames(string filePath) 
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.SectionNames()");
            filePath = new FileInfo(filePath).FullName;
            string[] ret = null;
            uint buffChars = 1024;
            IntPtr buffPtr = Marshal.AllocCoTaskMem((int)(buffChars * 2));
            uint readChars = 0;
            do
            {
                readChars = GetPrivateProfileSectionNames(buffPtr, buffChars, filePath);
                if (readChars == 0)
                    break;
                if (readChars < buffChars - 2)
                    break;
                buffChars *= 2;
                buffPtr = Marshal.ReAllocCoTaskMem(buffPtr, (int)buffChars * 2);
            } while (true);
            if (readChars > 0)
            {
                string text = Marshal.PtrToStringAuto(buffPtr, (int)readChars).ToString();
                ret = text.Split(new char[1] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            Marshal.FreeCoTaskMem(buffPtr);
            return ret;
        }

        static bool IsSectionExist(string sectionName, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.IsSectionExist()");

            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("Param:section is null or empty in IniFileOperator.IsSectionExist(section)");

            string[] sections = SectionNames(filePath);
            if (null != sections)
                foreach (string sec in sections)
                    if (sec.CompareTo(sectionName) == 0)
                        return true;
            return false;

        }

        static string[] ItemKeysInSection(string sectionName, string filePath)//已测试
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.ItemKeysInSection()");

            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("Param:section is null or empty in InitFileOperator.KeyNamesInSection(string section)");
            filePath = new FileInfo(filePath).FullName;
            string[] ret = null;
            uint buffChars = 1024;
            char[] buffArray = new char[buffChars];
            uint readChars = 0;
            do
            {
                readChars = GetPrivateProfileString(sectionName, null, null, buffArray, buffChars, filePath);
                if (readChars == 0)
                    break;
                if (readChars < buffChars - 2)
                    break;
                buffChars *= 2;
                buffArray = new char[buffChars];
            } while (true);
            if (readChars != 0 && readChars < buffChars - 2)
                ret = new string(buffArray).Split(new char[1] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            return ret;
        }

        static string ItemValue(string sectionName, string itemKey, string filePath, string valIfUnfound = null)//已测试
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.ItemValue()");

            string ret = valIfUnfound;
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("Section is null or empty in IniFileOperator.Value()");

            if (string.IsNullOrEmpty(itemKey))
                throw new ArgumentNullException("key is null or empty in IniFileOperator.Value()");
            filePath = new FileInfo(filePath).FullName;
            uint buffChars = 1024;
            char[] buffArray = new char[buffChars];
            uint readChars = 0;
            do
            {
                readChars = GetPrivateProfileString(sectionName, itemKey, valIfUnfound, buffArray, buffChars, filePath);
                if (readChars == 0)
                    break;
                if (readChars < buffChars - 2)
                    break;
                buffChars *= 2;
                buffArray = new char[buffChars];
            } while (true);
            if (readChars != 0 && readChars < buffChars - 2)
                ret = new string(buffArray).Split(new char[1] { '\0' }, StringSplitOptions.RemoveEmptyEntries)[0];
            return ret;
        }

        static bool SetItemValue(string sectionName, string itemKey, string itemValue, string filePath)//已测试
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.SetItemValue()");
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("Section is null or empty in IniFileOperator.SetItemValue()");
            if (string.IsNullOrEmpty(itemKey))
                throw new ArgumentNullException("key is null or empty in IniFileOperator.SetItemValue()");
            if (string.IsNullOrEmpty(itemValue))
                throw new ArgumentNullException("value is null or empty in IniFileOperator.SetItemValue()");
            filePath = new FileInfo(filePath).FullName;
            return WritePrivateProfileString(sectionName, itemKey, itemValue, filePath);
        }

        static void MaptoDictionary(SortedList<string, SortedList<string, string>> dictMap, string filePath) //已测试
        {

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("Param:filePath is null in IniFileOperator.MaptoDictionary(dictMap,filePath)");

            if (null == dictMap)
                throw new ArgumentNullException("Param:dictMap is null in IniFileOperator.MaptoDictionary(dictMap,filePath)");


            dictMap.Clear();
            if (!File.Exists(filePath))
                return;
            filePath = new FileInfo(filePath).FullName;
            do
            {
                string[] sectionNames = SectionNames(filePath);
                if (null == sectionNames)
                    break;

                foreach (string sm in sectionNames)
                {
                    SortedList<string, string> dictItems = new SortedList<string, string>();
                    dictMap.Add(sm, dictItems);
                    string[] itemKeys = ItemKeysInSection(sm, filePath);
                    if (null != itemKeys)
                        foreach (string itemKey in itemKeys)
                        {
                            string itemVal = ItemValue(sm, itemKey, filePath, null);
                            dictItems.Add(itemKey, itemVal);
                        }

                }
            } while (false);
            return;
        }

        public static SortedList<string, SortedList<string, string>> SortedListFromFile(string filePath) //已测试
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("FilePath is NullOrEmpty in IniFileOperator.ToSortedList");

            SortedList<string, SortedList<string, string>> ret = new SortedList<string, SortedList<string, string>>();
            MaptoDictionary(ret, filePath);
            return ret;
        }

        public static bool SaveSortedList(SortedList<string, SortedList<string, string>> slist, string filePath)//已测试
        {
            if (null == slist)
                throw new ArgumentNullException("Param:dict is null in IniFileOperator.SaveDictionary(dict)");
            //先删除ini文件中所有的Section
            filePath = new FileInfo(filePath).FullName;
            string[] sections = SectionNames(filePath);
            if (null != sections)
                foreach (string section in sections)
                {
                    if (!WritePrivateProfileString(section, null, null, filePath))
                        return false;
                }

            for (int i = 0; i < slist.Count; i++)
            {
                
                string sec = slist.Keys[i];
                SortedList<string, string> items = slist[sec];
                for (int j = 0; j < items.Count; j++)
                {
                    if (!SetItemValue(sec, items.Keys[j], items.Values[j], filePath))
                        return false;
                }
            }

            return true;
        }
        #endregion

        public IniFileOperator(string filePath)
        {
            _CheckFilePath(filePath);
            FileInfo fi = new FileInfo(filePath);
            FilePath = fi.FullName;
        }

        public string FilePath { get; private set; }

        public void _CheckFilePath(string filePath)
        {
            if (!File.Exists(filePath))
                File.Create(filePath);
        }




        public string[] SectionNames() //测试OK
        {
            return SectionNames(FilePath);
        }

        public bool IsSectionExist(string sectionName)
        {
            return IsSectionExist(sectionName, FilePath);
        }

        public bool DeleteSection(string sectionName) //已测试OK
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.DeleteSection()");
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("Param:section is null or empty in IniFileOperator.DeleteSection(string section)");
            if (!IsSectionExist(sectionName))
                return true;
            return WritePrivateProfileString(sectionName, null, null, FilePath);//删除Section时，filePath必须使用全路径
        }

        public bool ClearSection(string sectionName)//已测试OK
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.ClearSection()");
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("Param:section is null or empty in IniFileOperator.ClearSection(string section)");
            if (!IsSectionExist(sectionName))
                return true;
            return WritePrivateProfileSection(sectionName, string.Empty, FilePath);
        }

        public bool AddSection(string sectionName) //Test OK
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.AddSection()");

            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("Param:section is null or empty in IniFileOperator.AddSection(string section)");
            if (IsSectionExist(sectionName))
                return true;
            return WritePrivateProfileSection(sectionName, string.Empty, FilePath);

        }

        public string[] ItemKeysInSection(string sectionName)//已测试
        {
            return ItemKeysInSection(sectionName, FilePath);
        }

        public bool IsItemExist(string sectionName, string itemKey)
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.IsItemExist()");

            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("Section is null or empty in IniFileOperator.IsItemExist()");

            if (string.IsNullOrEmpty(itemKey))
                throw new ArgumentNullException("key is null or empty in IniFileOperator.IsItemExist()");
            string[] itemKeys = ItemKeysInSection(sectionName);
            if (null != itemKeys)
                foreach (string ik in itemKeys)
                    if (ik.CompareTo(itemKey) == 0)
                        return true;
            return false;
        }


        public string ItemValue(string sectionName, string itemKey, string valIfUnfound = null)//已测试
        {
            return ItemValue(sectionName, itemKey, FilePath, valIfUnfound);
        }

        public bool SetItemValue(string sectionName, string itemKey, string itemValue)//已测试
        {
            return SetItemValue(sectionName, itemKey, itemValue, FilePath);
        }

        public bool DeleteItem(string sectionName, string itemKey) //已测试
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new ArgumentNullException("FilePath is Null or Empty in IniFileOperator.DeleteItem()");

            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("Section is null or empty in IniFileOperator.DeleteItem()");
            if (string.IsNullOrEmpty(itemKey))
                throw new ArgumentNullException("key is null or empty in IniFileOperator.DeleteItem()");

            return WritePrivateProfileString(sectionName, itemKey, null, FilePath);
        }

    }
}
