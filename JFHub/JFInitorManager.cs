using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFToolKits;
using JFInterfaceDef;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace JFHub
{
    /// <summary>
    /// Initor管理器类
    /// </summary>
    public class JFInitorManager
    {
        internal JFInitorManager()
        {
            dictInitors = new SortedDictionary<string, IJFInitializable>();

        }

        internal void Init()
        {

            JFXmlSortedDictionary<string, List<object>> devInitParams = JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_InitDevParams) as JFXmlSortedDictionary<string, List<object>>;
            foreach (KeyValuePair<string, List<object>> kv in devInitParams)
            {
                IJFInitializable dev = JFHubCenter.Instance.InitorHelp.CreateInstance(kv.Value[0] as string);// as IJFDevice;
                dictInitors.Add(kv.Key, dev);
                try //尝试初始化
                {
                    string[] paramNames = dev.InitParamNames;
                    if (null != paramNames && paramNames.Length > 0)
                        for (int i = 0; i < paramNames.Length; i++)
                        {
                            object pr = i < kv.Value.Count - 1 ? kv.Value[i + 1] : null;
                            object relParamVal = null;
                            Type paramType = dev.GetInitParamDescribe(paramNames[i]).ParamType;
                            SerializableAttribute[] sas = paramType.GetCustomAttributes(typeof(SerializableAttribute), false) as SerializableAttribute[];
                            if (sas != null && sas.Length > 0) //如果是可序列化的类型，直接保存序列化后的文本
                            {
                                relParamVal = JFFunctions.FromXTString(pr as string, paramType);

                            }
                            else
                            {
                                if (paramType.IsValueType || paramType == typeof(string)) //单值对象和字符串对象直接添加
                                    relParamVal = pr;
                                else //目前支持Array 和 List
                                {
                                    if (paramType.IsArray) //参数类型是数组
                                    {
                                        if (null == pr || string.Empty == pr as string)
                                            relParamVal = null;
                                        else
                                        {
                                            string[] elmts = (pr as string).Split(new char[] { '$'});
                                            relParamVal = GenArrayObject(paramType, elmts.Length);
                                            if (elmts.Length != 0)
                                            {
                                                for (int j = 0; j < elmts.Length; j++)
                                                    (relParamVal as Array).SetValue(JFConvertExt.ChangeType(elmts[j], paramType.GetElementType()), j);
                                            }
                                        }

                                    }
                                    else if (typeof(IList).IsAssignableFrom(paramType)) //队列类型
                                    {
                                        if (null == pr || string.Empty == pr as string)
                                            relParamVal = null;
                                        else
                                        {
                                            string[] elmts = (pr as string).Split(new char[] { '$' });
                                            relParamVal = GenListObject(paramType);
                                            if (elmts.Length != 0)
                                            {
                                                for (int j = 0; j < elmts.Length; j++)
                                                    (relParamVal as IList).Add(JFConvertExt.ChangeType(elmts[j], paramType.GetElementType()));
                                            }
                                        }
                                    }
                                }

                            }
                            dev.SetInitParamValue(paramNames[i], relParamVal);
                        }
                    dev.Initialize();
                    if (dev is IJFDevice)
                    {
                        (dev as IJFDevice).OpenDevice();
                        Thread.Sleep(100);
                    }

                }
                catch
                {
                    //初始化发生异常
                }
                //dictInitors.Add(kv.Key, dev);
                //if (dev is IJFStation)
                //{
                //    (dev as IJFStation).WorkStatusChanged += JFHubCenter.Instance.StationMgr.StationWorkStatusChanged;
                //    (dev as IJFStation).CustomStatusChanged += JFHubCenter.Instance.StationMgr.StationCustomStatusChanged;
                //    if (dev is JFCmdWorkBase)
                //        (dev as JFCmdWorkBase).WorkMsg2Outter += JFHubCenter.Instance.StationMgr.StationWorkMsg;
                //}

            }

           
        }
        
        /// <summary>
        /// 根据指定的数组类型创建一个数组对象
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        object GenArrayObject(Type t,int len)
        {
            ConstructorInfo[] ctors = t.GetConstructors(System.Reflection.BindingFlags.Instance
                                                        | System.Reflection.BindingFlags.NonPublic
                                                        | System.Reflection.BindingFlags.Public);

            foreach (ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] ps = ctor.GetParameters();
                if (ps != null && ps.Length == 1 && ps[0].ParameterType == typeof(int))
                    return ctor.Invoke(new object[] { len });
            }
            return null;
        }


        object GenListObject(Type t)
        {
            ConstructorInfo[] ctors = t.GetConstructors(System.Reflection.BindingFlags.Instance
                                                        | System.Reflection.BindingFlags.NonPublic
                                                        | System.Reflection.BindingFlags.Public);

            foreach (ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] ps = ctor.GetParameters();
                if (ps == null || ps.Length == 0 )
                    return ctor.Invoke(null);
            }
            return null;
        }



        /// <summary>
        /// 获取当前所有已创建Initor对象名称
        /// </summary>
        public string[] InitorIDs 
        {
            get
            {
                return dictInitors.Keys.ToArray();
            }
        }

        /// <summary>根据指定的接口类型筛选ID</summary>
        public string[] GetIDs(Type matchType)
        {
            List<string> ret =new List<string>();
            foreach(KeyValuePair<string,IJFInitializable> kv in dictInitors)
            {
                if (matchType.IsAssignableFrom(kv.Value.GetType()))
                    ret.Add(kv.Key);
            }
            return ret.ToArray();
        }

        public IJFInitializable GetInitor(string ID)
        {
            if (string.IsNullOrEmpty(ID))
                return null;
            if (!dictInitors.ContainsKey(ID))
                return null;
            return dictInitors[ID];
        }

        public bool ContainID(string ID)
        {

            return dictInitors.ContainsKey(ID);
        }

        public IJFInitializable this[string ID]
        {
            get
            {
                return GetInitor(ID);
            }
        }

        /// <summary>
        /// 获取Initor对象对应的ID
        /// </summary>
        /// <param name="initor"></param>
        /// <returns></returns>
        public string GetIDByInitor(IJFInitializable initor)
        {
            if (!dictInitors.ContainsValue(initor))
                return null;
            return  dictInitors.FirstOrDefault(q => q.Value == initor).Key;
        }

        //internal bool SaveCfgWhenAdd { get; set; }

        internal void Add(string id,IJFInitializable dev)
        {
            dictInitors.Add(id, dev);
                JFXmlSortedDictionary<string, List<object>> dictInitorParam = JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_InitDevParams) as JFXmlSortedDictionary<string, List<object>>;
                List<object> paramsInCfg = new List<object>();
                paramsInCfg.Add(dev.GetType().AssemblyQualifiedName);
                for (int i = 0; i < dev.InitParamNames.Length; i++)
                {
                    //paramsInCfg.Add(dev.GetInitParamValue(dev.InitParamNames[i])); 本行代码用以下代码代替，以支持有限的 简单类型 + 数组（列表）类型的组合 
                    //暂时的解决方法：将数组类型转化为字符串存储
                    object paramVal = dev.GetInitParamValue(dev.InitParamNames[i]);
                    Type paramType = dev.GetInitParamDescribe(dev.InitParamNames[i]).ParamType;
                    SerializableAttribute[] sas = paramType.GetCustomAttributes(typeof(SerializableAttribute), false) as SerializableAttribute[];
                    if (sas != null && sas.Length > 0) //如果是可序列化的类型，直接保存序列化后的文本
                    {
                        StringBuilder buffer = new StringBuilder();
                        XmlSerializer serializer = new XmlSerializer(paramType);
                        using (TextWriter writer = new StringWriter(buffer))
                        {
                            serializer.Serialize(writer, paramVal);
                        }
                        string xmlTxt = buffer.ToString();
                        paramsInCfg.Add(xmlTxt);
                    }
                    else
                    {
                        if (paramType.IsValueType || paramType == typeof(string)) //单值对象和字符串对象直接添加
                            paramsInCfg.Add(paramVal);
                        else //目前支持Array 和 List
                        {
                            if (paramType.IsArray) //参数类型是数组
                            {
                                if (null == paramVal)
                                {
                                    paramsInCfg.Add("");
                                }
                                else
                                {
                                StringBuilder sb = new StringBuilder();
                                string splitString = "$";
                                for (int j = 0; j < (paramVal as Array).Length; j++)
                                {
                                    sb.Append((paramVal as Array).GetValue(j).ToString());
                                    if (j < (paramVal as Array).Length - 1)
                                        sb.Append(splitString);
                                }
                                paramsInCfg.Add(sb.ToString());//paramsInCfg.Add(string.Join(splitString, paramVal)); //使用ASC 响铃符作为间隔
                                }
                            }
                            else if (typeof(IList).IsAssignableFrom(paramType)) //除了数组之外的队列类型
                            {
                                if (null == paramVal)
                                {
                                    paramsInCfg.Add("");
                                }
                                else
                                {
                                StringBuilder sb = new StringBuilder();
                                string splitString = "$";
                                for (int j = 0; j < (paramVal as IList).Count; j++)
                                {
                                    sb.Append((paramVal as IList)[j].ToString());
                                    if (j < (paramVal as IList).Count - 1)
                                        sb.Append(splitString);
                                }
                                paramsInCfg.Add(sb.ToString());
                                //paramsInCfg.Add(string.Join(System.Text.Encoding.ASCII.GetString(new byte[] { 0x07 }), paramVal)); //使用ASC 响铃符作为间隔
                            }
                            }
                        }

                    }

                }
                dictInitorParam.Add(id, paramsInCfg);
                JFHubCenter.Instance.SystemCfg.NotifyItemChanged(JFHubCenter.CK_InitDevParams);
                JFHubCenter.Instance.SystemCfg.Save();
            
        }

        internal void Remove(string id)
        {
            dictInitors.Remove(id);
            JFXmlSortedDictionary<string, List<object>> dictInitorParam = JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_InitDevParams) as JFXmlSortedDictionary<string, List<object>>;
            dictInitorParam.Remove(id);
            JFHubCenter.Instance.SystemCfg.Save();
        }

        internal void Clear()
        {
            dictInitors.Clear();
            JFXmlSortedDictionary<string, List<object>> dictInitorParam = JFHubCenter.Instance.SystemCfg.GetItemValue(JFHubCenter.CK_InitDevParams) as JFXmlSortedDictionary<string, List<object>>;
            dictInitorParam.Clear();
            JFHubCenter.Instance.SystemCfg.Save();
        }


        SortedDictionary<string, IJFInitializable> dictInitors;
        

        
    }
}
