using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Reflection;

namespace JFToolKits
{
    /// <summary>
    /// 各种方法调用
    /// </summary>
    public static class JFFunctions
    {
        /// <summary>
        /// 将一个对象转化为xml字符串，和类型信息字符串（AssemblyQualifiedName）
        /// </summary>
        /// <param name="o"></param>
        /// <param name="xml"></param>
        /// <param name="typeString"></param>
        public static void ToXTString(object o, out string xml, out string typesAssemblyQualifiedName)
        {
            if(null == o)
            {
                xml = null;
                typesAssemblyQualifiedName = null;
                return;
            }
            StringBuilder buffer = new StringBuilder();
            XmlSerializer serializer = new XmlSerializer(o.GetType());
            using (TextWriter writer = new StringWriter(buffer))
            {
                serializer.Serialize(writer, o);
            }

            xml =  buffer.ToString();
            typesAssemblyQualifiedName = o.GetType().AssemblyQualifiedName;
        }

        public static object FromXTString(string xml, string typesAssemblyQualifiedName)
        {
            //Object obj = null;
            //StringBuilder buffer = new StringBuilder();
            //buffer.Append(xml);

            //XmlSerializer serializer = new XmlSerializer(Type.GetType(typesAssemblyQualifiedName));

            //using (TextReader reader = new StringReader(buffer.ToString()))
            //{
            //    obj = serializer.Deserialize(reader);
            //}

            //return obj;
            return FromXTString(xml, Type.GetType(typesAssemblyQualifiedName));
        }


        public static object FromXTString(string xml, Type t)
        {
            Object obj = null;
            StringBuilder buffer = new StringBuilder();
            buffer.Append(xml);

            XmlSerializer serializer = new XmlSerializer(t);

            using (TextReader reader = new StringReader(buffer.ToString()))
            {
                obj = serializer.Deserialize(reader);
            }

            return obj;
        }

        /// <summary>判断字符串是不是合法的串口名</summary>
        public static bool IsSerialPortName(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return false;
            if (txt.IndexOf("COM") != 0)
                return false;
            string pattern = "^COM[1-9]\\d*|0$";
            Match mc = Regex.Match(txt, pattern);
            if(mc != null && mc.Success )
                return true;
            return false;
        }

        /// <summary>
        /// 判断string是不是一个合法的IP地址
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static bool IsIPAddress(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return false;

            Regex rx = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");
            if (!rx.IsMatch(txt))
                return false;
            return true;
        }




        public static string ToFullPath(string localShortPath)
        {
            if (string.IsNullOrWhiteSpace(localShortPath))
                throw new ArgumentNullException("JFFunctions.ToFullPath(localShortPath) failed By: localShortPath Is Null Or WhiteSpace ");
            return Path.GetFullPath(localShortPath);
            
        }

        public static string ToLocalShortPath(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentNullException("JFFunctions.ToLocalShortPath(fullPath) failed By: fullPath Is Null Or WhiteSpace ");

            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            if (fullPath.IndexOf(appDir) == 0)
                fullPath = fullPath.Replace(appDir, ".\\");
            return fullPath;
        }

        public static string GetFileMD5(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetFileMD5() fail,error:" + ex.Message);
            }
        }

        [DllImport("Kernel32.dll")]
        static extern bool QueryPerformanceCounter(out long performanceCount);

        [DllImport("Kernel32.dll")]
        static extern bool QueryPerformanceFrequency(out long frequency);

        /// <summary>获取CPU当前计数</summary>
        public static long PerformanceCounter()
        {
            long ret = 0;
            QueryPerformanceCounter(out ret);
            return ret;
        }

        static long _performanceFrequency = 0;
        /// <summary>获取本机CPU频率</summary>
        public static long PerformanceFrequency()
        {
            if (_performanceFrequency > 0)
                return _performanceFrequency;
            QueryPerformanceFrequency(out _performanceFrequency);
            return _performanceFrequency;
        }

        /// <summary>
        /// 获取当前程序加载的所有程序集中，继承于baseType的所有实体类型
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Type[] GetClassTypesInherited(Type baseType)
        {
            List<Type> ret = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly ass in assemblies)
            {
                Type[] ts = ass.GetTypes();
                foreach (Type t in ts)
                    if (baseType.IsAssignableFrom(t) && t.IsClass)
                        if (!ret.Contains(t))
                            ret.Add(t);
            }

            return ret.ToArray();
        }

        /// <summary>
        /// 显示一段信息，延时指定的时间后关闭
        /// </summary>
        /// <param name="info"></param>
        /// <param name="delayMillisecondsClose"></param>
        public static void DelayCloseTips(string info,int delaySecondsClose = 2)
        {

        }

    }

   
}
