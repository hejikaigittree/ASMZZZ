using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace JFToolKits
{
    /// <summary>
    /// 使用xml文件存储参数配置的类
    /// </summary>
    public class JFXCfg
    {

        /// <summary>
        /// 代理:配置项值改变
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="newValue"></param>
        public delegate void ItemChange(string itemName, object newValue);
        public ItemChange ItemChangedEvent;


        public JFXCfg()
        {
            FilePath = null;
            dictNamesInTag = new JFXmlSortedDictionary<string, List<string>>();
            dictNameValue = new JFXmlDictionary<string, object>();
            //innerData = new InnerData();
            
        }
        //InnerData innerData;
        JFXmlSortedDictionary<string, List<string>> dictNamesInTag;//按标签名存储的数据项名称
        JFXmlDictionary<string, object> dictNameValue; //数据项键值对

        /// <summary>
        /// 配置文件的名称
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// 所有的标签名称,标签的作用：给配置项分类，便于用户使用分页显示
        /// </summary>
        public string[] AllTags
        {
            get
            {
                if (0 == dictNamesInTag.Count)
                    return new string[] { };
                return dictNamesInTag.Keys.ToArray<string>();
            }
        }


        /// <summary>
        /// 获取标签下的所有配置项名称
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public string[] ItemNamesInTag(string tag)
        {
            lock (this)
            {
                if (!/*innerData.*/dictNamesInTag.ContainsKey(tag))
                    return new string[] { };
                return /*innerData.*/dictNamesInTag[tag].ToArray();
            }
            
        }


        /// <summary>
        /// 获取所有数据项名称
        /// </summary>
        /// <returns></returns>
        public string[] AllItemNames() 
        {  return dictNameValue.Keys.ToArray();  }

        /// <summary>
        /// 从文件中（重新）加载配置
        /// </summary>
        public void Load()
        {
            Load(FilePath, false);
        }

        /// <summary>
        /// 从文件中加载参数配置
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="isOpenOrCreate">文件不存在时是否创建新文件，=True:创建新文件 ;  =False：不创建新文件，会抛出一个异常</param>
        public void Load(string filePath, bool isOpenOrCreate)
        {

            if (!File.Exists(filePath)) //文件不存在
            {
                if (!isOpenOrCreate)
                    throw new FileNotFoundException(string.Format("JFXCfg.Load(filePath={0},isOpenOrCreate={1}) failed by: FilePath is Nonexists!", filePath, isOpenOrCreate));
                CreateXmlFile(filePath);
                dictNamesInTag.Clear();
                dictNameValue.Clear();
                FilePath = filePath;
                return;
            }

            XmlDocument xd = new XmlDocument();
            xd.Load(filePath);
            XmlNode xnRoot = xd.SelectSingleNode("root");
            XmlElement xeTagNames = xnRoot.SelectSingleNode("Tag-Names") as XmlElement;
            if (null != xeTagNames && !string.IsNullOrEmpty(xeTagNames.InnerText))
                dictNamesInTag = JFFunctions.FromXTString(xeTagNames.InnerText, typeof(JFXmlSortedDictionary<string, List<string>>).AssemblyQualifiedName) as JFXmlSortedDictionary<string, List<string>>;
            else
                dictNamesInTag.Clear();


            XmlElement xeItems = xnRoot.SelectSingleNode("Items") as XmlElement;
            if (null != xeItems && !string.IsNullOrEmpty(xeItems.InnerText))
                dictNameValue = JFFunctions.FromXTString(xeItems.InnerText, typeof(JFXmlDictionary<string, object>).AssemblyQualifiedName) as JFXmlDictionary<string, object>;
            else
                dictNameValue.Clear();

            //JFXCfg cfg = this.ReadXmlFile(filePath);
            //dictNamesInTag = cfg.dictNamesInTag;
            //dictNameValue = cfg.dictNameValue;
            //cfg = null;
            FilePath = filePath;
        }

        public void Save()
        {
            Save(FilePath);
            //this.WriteXmlFile(FilePath);
        }

        public void Save(string filePath)
        {
            if (!File.Exists(filePath)) //文件不存在
                CreateXmlFile(filePath);
            XmlDocument xd = new XmlDocument();
            XmlElement xeRoot = xd.CreateElement("root");
            xd.AppendChild(xeRoot);

            string xmlTxt, typeTxt;
            JFFunctions.ToXTString(dictNamesInTag, out xmlTxt, out typeTxt);
            XmlElement xeNamesInTag = xd.CreateElement("Tag-Names");
            xeRoot.AppendChild(xeNamesInTag);
            xeNamesInTag.InnerText = xmlTxt;

            JFFunctions.ToXTString(dictNameValue, out xmlTxt, out typeTxt);
            XmlElement xeItems = xd.CreateElement("Items");
            xeRoot.AppendChild(xeItems);
            xeItems.InnerText = xmlTxt;

            xd.Save(filePath);
        }


        void CreateXmlFile(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dir) && !string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            XmlTextWriter xml = new XmlTextWriter(filePath, Encoding.UTF8);
            xml.WriteStartDocument();

            xml.WriteStartElement("root");
            xml.WriteEndElement();
            xml.WriteEndDocument();
            xml.Flush();
            xml.Close();
        }

        public bool ContainsItem(string itemName)
        {
            return /*innerData.*/dictNameValue.ContainsKey(itemName);
            
        }


        public void AddItem(string name, object value, string tag = null/*string.Empty*/)
        {
            lock (this)
            {
                //if (dictNameValue.ContainsKey(name))
                //    dictNameValue[name] = value;
                //else
                /*innerData.*/dictNameValue.Add(name, value);
                if (null == tag)
                    tag = "";
                if (!/*innerData.*/dictNamesInTag.ContainsKey(tag))
                    /*innerData.*/dictNamesInTag.Add(tag, new List<string>());
                /*innerData.*/dictNamesInTag[tag].Add(name);

            }
        }

        public void RemoveItem(string itemName)
        {
            lock (this)
            {
                if (dictNameValue.ContainsKey(itemName))
                    dictNameValue.Remove(itemName);
                string tag = GetItemTag(itemName);
                if (tag != null)
                {
                    dictNamesInTag[tag].Remove(itemName);
                    if (dictNamesInTag[tag].Count == 0)
                        dictNamesInTag.Remove(tag);
                }
            }

        }


        public string GetItemTag(string itemName)
        {
            //lock (this)
            {
                foreach (KeyValuePair<string, List<string>> kv in dictNamesInTag)
                    if (kv.Value.Contains(itemName))
                        return kv.Key;
            }
            return null;
        }

        public object GetItemValue(string itemName)
        {
            return dictNameValue[itemName];
        }

        public void SetItemValue(string itemName, object itemValue)
        {

            lock (this)
            {
                //if (GetItemValue(itemName).Equals(itemValue))
                //    return;
                dictNameValue[itemName] = itemValue;
            }
            if (null != ItemChangedEvent)
                ItemChangedEvent(itemName, itemValue);
        }

        public void NotifyItemChanged(string itemName)
        {
            if (null != ItemChangedEvent)
                ItemChangedEvent(itemName, GetItemValue(itemName));
        }


        


    }
}
