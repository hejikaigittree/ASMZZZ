using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace LFAOIRecipe
{
    class ObjectCloneHelper
    {
        /// <summary>
        /// 序列化复制，需要对象支持序列化[Serializable]
        /// </summary>
        public static object SerializeClone(object obj)
        {
            if (obj == null) return null;
            byte[] arrByte = ToBytes(obj);
            return ToObject(arrByte);
        }

        // 将对象序列化成字节数组
        private static byte[] ToBytes(object obj)
        {
            if (obj == null) return null;
            using (MemoryStream s = new MemoryStream())
            {
                IFormatter f = new BinaryFormatter();
                f.Serialize(s, obj);
                return s.GetBuffer();
            }
        }

        // 将字节数组反序列化成对象
        private static object ToObject(byte[] Bytes)
        {
            using (MemoryStream s = new MemoryStream(Bytes))
            {
                IFormatter f = new BinaryFormatter();
                return f.Deserialize(s);
            }
        }
    }

}
