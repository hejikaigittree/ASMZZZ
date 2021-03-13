using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace JFToolKits
{
    /// <summary>
    /// Dictionary(支持 XML 序列化)
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    [XmlRoot("JFXmlDictionary")]
    [System.Serializable]
    public class JFXmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region 构造函数
        public JFXmlDictionary() : base()
        { }

        public JFXmlDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        { }

        public JFXmlDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        { }

        public JFXmlDictionary(int capacity) : base(capacity)
        { }

        public JFXmlDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
        { }

        protected JFXmlDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
        #endregion 构造函数

        #region IXmlSerializable Members
        public XmlSchema GetSchema() => null;

        /// <summary>
        ///     从对象的 XML 表示形式生成该对象(反序列化)
        /// </summary>
        /// <param name="xr"></param>
        public void ReadXml(XmlReader xr)
        {
            if (xr.IsEmptyElement)
            {
                xr.Read();//xr.ReadEndElement();
                return;
            }
            var ks = new XmlSerializer(typeof(TKey));
            var vs = new XmlSerializer(typeof(TValue));
            var ts = new XmlSerializer(typeof(string));
            xr.Read();
            while (xr.NodeType != XmlNodeType.EndElement)
            {
                xr.ReadStartElement("Item");
                xr.ReadStartElement("Type");
                string assemblyQualifiedName = (string)ts.Deserialize(xr);
                xr.ReadEndElement();
                xr.ReadStartElement("Key");
                var key = (TKey)ks.Deserialize(xr);
                xr.ReadEndElement();
                xr.ReadStartElement("Value");
                vs = new XmlSerializer(Type.GetType(assemblyQualifiedName));
                var value = (TValue)vs.Deserialize(xr);
                Add(key, value);
                xr.ReadEndElement();
                xr.ReadEndElement();
                xr.MoveToContent();
            }
            xr.ReadEndElement();
        }

        /// <summary>
        ///     将对象转换为其 XML 表示形式(序列化)
        /// </summary>
        /// <param name="xw"></param>
        public void WriteXml(XmlWriter xw)
        {
            var ks = new XmlSerializer(typeof(TKey));
            var vs = new XmlSerializer(typeof(TValue));
            var ss = new XmlSerializer(typeof(string));
            //foreach (var key in Keys)
            //{
            //    if(null != this[key] )
            //        vs = new XmlSerializer(this[key].GetType());
            //    else
            //        vs = new XmlSerializer(typeof(TValue));
            //    xw.WriteStartElement("Item");
            //    xw.WriteStartElement("Key");
            //    ks.Serialize(xw, key);
            //    xw.WriteEndElement();
            //    xw.WriteStartElement("Value");
            //    vs.Serialize(xw, this[key]);
            //    xw.WriteEndElement();
            //    xw.WriteEndElement();
            //}
            Type realType = typeof(TValue); //项值的真实类型
            foreach (var key in Keys)
            {
                if (null != this[key])
                    realType = this[key].GetType();//vs = new XmlSerializer(this[key].GetType());
                else
                    realType = typeof(TValue);//vs = new XmlSerializer(typeof(TValue));
                vs = new XmlSerializer(realType);
                xw.WriteStartElement("Item");
                xw.WriteStartElement("Type");
                ss.Serialize(xw, realType.AssemblyQualifiedName);
                xw.WriteEndElement();
                xw.WriteStartElement("Key");
                ks.Serialize(xw, key);
                xw.WriteEndElement();
                xw.WriteStartElement("Value");
                vs.Serialize(xw, this[key]);
                xw.WriteEndElement();
                xw.WriteEndElement();
            }

        }
        #endregion IXmlSerializable Members
    }

    [XmlRoot("JFXmlSortedDictionary")]
    [Serializable]
    public class JFXmlSortedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region 构造函数
        public JFXmlSortedDictionary() : base()
        { }

        public JFXmlSortedDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        { }

        public JFXmlSortedDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        { }

        public JFXmlSortedDictionary(int capacity) : base(capacity)
        { }

        public JFXmlSortedDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
        { }

        protected JFXmlSortedDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
        #endregion 构造函数

        #region IXmlSerializable Members
        public XmlSchema GetSchema() => null;

        /// <summary>
        ///     从对象的 XML 表示形式生成该对象(反序列化)
        /// </summary>
        /// <param name="xr"></param>
        public void ReadXml(XmlReader xr)
        {
            if (xr.IsEmptyElement)
            {
                xr.Read();//xr.ReadEndElement();
                return;
            }
            var ks = new XmlSerializer(typeof(TKey));
            var vs = new XmlSerializer(typeof(TValue));
            var ts = new XmlSerializer(typeof(string));
            xr.Read();
            while (xr.NodeType != XmlNodeType.EndElement)
            {
                xr.ReadStartElement("Item");
                xr.ReadStartElement("Type");
                string assemblyQualifiedName = (string)ts.Deserialize(xr);
                xr.ReadEndElement();
                xr.ReadStartElement("Key");
                var key = (TKey)ks.Deserialize(xr);
                xr.ReadEndElement();
                xr.ReadStartElement("Value");
                vs = new XmlSerializer(Type.GetType(assemblyQualifiedName));
                var value = (TValue)vs.Deserialize(xr);
                xr.ReadEndElement();
                Add(key, value);
                xr.ReadEndElement();
                xr.MoveToContent();
            }
            xr.ReadEndElement();
        }

        /// <summary>
        ///     将对象转换为其 XML 表示形式(序列化)
        /// </summary>
        /// <param name="xw"></param>
        public void WriteXml(XmlWriter xw)
        {
            var ks = new XmlSerializer(typeof(TKey));
            var vs = new XmlSerializer(typeof(TValue));
            var ss = new XmlSerializer(typeof(string));
            Type realType = typeof(TValue); //项值的真实类型
            foreach (var key in Keys)
            {
                if (null != this[key])
                    realType = this[key].GetType();//vs = new XmlSerializer(this[key].GetType());
                else
                    realType = typeof(TValue);//vs = new XmlSerializer(typeof(TValue));
                vs = new XmlSerializer(realType);
                xw.WriteStartElement("Item");
                xw.WriteStartElement("Type");
                ss.Serialize(xw, realType.AssemblyQualifiedName);
                xw.WriteEndElement();
                xw.WriteStartElement("Key");
                ks.Serialize(xw, key);
                xw.WriteEndElement();
                xw.WriteStartElement("Value");
                vs.Serialize(xw, this[key]);
                xw.WriteEndElement();
                xw.WriteEndElement();
            }

        }
        #endregion IXmlSerializable Members
    }


}
