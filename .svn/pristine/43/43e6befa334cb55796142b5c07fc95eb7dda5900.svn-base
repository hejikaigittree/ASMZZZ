using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;

namespace JFToolKits
{
    public static class XmlSerializerFile
    {
        public static byte[] Keys = { 0x79, 0x63, 0x70, 0x66, 0x72, 0x61, 0x6D, 0x6C }; // ycpframe
        public const string Iv = "01234567";
        public static void WriteXmlFile<T>(this T obj, string filename) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            WriteXmlFileInternal<T>(obj, filename, serializer, false);
        }
        public static void WriteXmlFile<T>(this T obj, string filename, IEnumerable<Type> knownTypes) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T), knownTypes);
            WriteXmlFileInternal<T>(obj, filename, serializer, false);
        }
        public static void WriteEnCryptFile<T>(this T obj, string filename) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            WriteXmlFileInternal<T>(obj, filename, serializer, true);
        }
        public static void WriteEnCryptFile<T>(this T obj, string filename, IEnumerable<Type> knownTypes) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T), knownTypes);
            WriteXmlFileInternal<T>(obj, filename, serializer, true);
        }
        private static void WriteXmlFileInternal<T>(T obj, string filename, DataContractSerializer serializer, bool bEncrypt) where T : class
        {
            Stream stream = null;
            XmlWriter writer = null;
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineHandling = NewLineHandling.Entitize;
                xmlWriterSettings.CheckCharacters = false;

                stream = new FileStream(filename, FileMode.Create, FileAccess.Write);

                if (bEncrypt)
                {
                    byte[] rgbKey = Encoding.UTF8.GetBytes(Iv.Substring(0, 8));
                    byte[] rgbIV = Keys;

                    DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                    //用指定的密钥 (Key) 和初始化向量 (IV) 创建对称数据加密标准 (DES) 加密器对象
                    CryptoStream cStream = new CryptoStream(stream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                    stream = cStream;
                    dCSP.Clear();
                }
                writer = XmlWriter.Create(stream, xmlWriterSettings);
                serializer.WriteObject(writer, obj);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        public static T ReadXmlFile<T>(this T obj, string filename) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            var newObj = ReadXmlFile(filename, serializer, false) as T;
            return newObj == null ? obj : newObj;
        }
        public static T ReadXmlFile<T>(string filename) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            return ReadXmlFile(filename, serializer, false) as T;
        }
        public static T ReadXmlFile<T>(this T obj, string filename, IEnumerable<Type> knownTypes) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T), knownTypes);
            var newObj = ReadXmlFile(filename, serializer, false) as T;
            return newObj == null ? obj : newObj;
        }
        public static T ReadXmlFile<T>(string filename, IEnumerable<Type> knownTypes) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T), knownTypes);
            return ReadXmlFile(filename, serializer, false) as T;
        }
        public static T ReadEncryptFile<T>(this T obj, string filename) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            var newObj = ReadXmlFile(filename, serializer, true) as T;
            return newObj == null ? obj : newObj;
        }
        public static T ReadEncryptFile<T>(string filename) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            return ReadXmlFile(filename, serializer, true) as T;
        }
        public static T ReadEncryptFile<T>(this T obj, string filename, IEnumerable<Type> knownTypes) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T), knownTypes);
            var newObj = ReadXmlFile(filename, serializer, true) as T;
            return newObj == null ? obj : newObj;
        }
        public static T ReadEncryptFile<T>(string filename, IEnumerable<Type> knownTypes) where T : class
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T), knownTypes);
            return ReadXmlFile(filename, serializer, true) as T;
        }

        private static object ReadXmlFile(string filename, DataContractSerializer serializer, bool Ecrypt)
        {
            object obj = null;

            if (File.Exists(filename))
            {
                //throw new FileNotFoundException(string.Format("Failed to load file: {0}", filename));
                Stream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                try
                {
                    if (Ecrypt)
                    {
                        byte[] rgbKey = Encoding.UTF8.GetBytes(Iv.Substring(0, 8));
                        byte[] rgbIV = Keys;

                        //long  len = fs.Length;
                        //byte[] EncryptData = new byte[len];
                        //fs.Read(EncryptData, 0, EncryptData.Length);


                        DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                        //用指定的密钥 (Key) 和初始化向量 (IV) 创建对称数据加密标准 (DES) 加密器对象
                        //MemoryStream mstream = new MemoryStream();
                        CryptoStream cStream = new CryptoStream(fs, dCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Read);

                        //StreamReader sr = new StreamReader(cStream);
                        //string test = sr.ReadLine();
                        //cStream.Write(EncryptData, 0, EncryptData.Length);
                        //cStream.FlushFinalBlock();
                        fs = cStream;
                        //string outdata = Convert.ToBase64String(mstream.ToArray());
                        //cStream.Clear();
                        //cStream.Close(); // 显示不正确的数据

                    }
                    var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                    obj = serializer.ReadObject(reader, true);

                }
                finally
                {
                    fs.Close();
                }
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Warning: Failed to load file: " + filename);
            }
            return obj;
        }
    }
}
