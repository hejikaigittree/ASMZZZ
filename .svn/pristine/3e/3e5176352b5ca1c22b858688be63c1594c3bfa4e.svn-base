using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
namespace JFHub
{
    public class JFDataPool : IJFDataPool
    {
        public JFDataPool()
        {
            
            
        }
        Dictionary<string, Type> dictItemTypes = new Dictionary<string, Type>();
        Dictionary<string, Type> dictListElementTypes = new Dictionary<string, Type>();
        ConcurrentDictionary<string, object> dictItems = new ConcurrentDictionary<string, object>();
        ConcurrentDictionary<string, /*List<object>*/object> dictLists = new ConcurrentDictionary<string, /*List<object>*/object>();

        public bool RegistItem(string key, Type itemType, object initValue)
        {
            lock(dictItemTypes)
            {
                if (dictItemTypes.ContainsKey(key))
                {
                    if (dictItemTypes[key] == itemType)
                        return true;
                    return false;
                }
                dictItemTypes.Add(key, itemType);
                if (!SetItemValue(key, initValue))
                    throw new ArgumentException(string.Format("RegistItem(key = {0},type = {1},value = {2})",key,itemType.Name,initValue.ToString()));

                //dictItems.TryAdd(key, null);
            }
            return true;
        }

        public bool ContainItem(string key)
        {
            return dictItemTypes.ContainsKey(key);
        }


        public object RemoveItem(string key)
        {
            lock (dictItemTypes)
            {
                if (!dictItemTypes.ContainsKey(key))
                    return null;

                dictItemTypes.Remove(key);
                object val = null;
                dictItems.TryRemove(key, out val);
                return val;
            }
        }
        public string[] AllItemKeys { get { return dictItems.Keys.ToArray(); } }

        public Type GetItemType(string key)
        {
            return dictItemTypes[key];
        }
        /// <summary>
        /// 设置一个项的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SetItemValue(string key, object value)
        {
            if (!dictItemTypes.ContainsKey(key))
                return false;
            lock (dictItemTypes[key])
            {
                if (null == value)
                {
                    //if (dictItemTypes[key].IsValueType)
                    //    return false;
                    dictItems[key] = value;
                    return true;
                }


                if (dictItemTypes[key].IsAssignableFrom(value.GetType()))//value的类型是ItemType的子类
                {
                    dictItems[key] = value;
                    return true;
                }

                if (JFTypeExt.IsExplicitFrom(dictItemTypes[key], value.GetType()))
                {
                    dictItems[key] =  JFConvertExt.ChangeType(value, dictItemTypes[key]);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 获取一个项值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetItemValue(string key, out object value)
        {
            return dictItems.TryGetValue(key, out value);
        }



        public bool RegistList(string key, Type itemType)
        {
            lock(dictListElementTypes)
            {
                if (dictListElementTypes.ContainsKey(key) && dictListElementTypes[key] == itemType)
                    return true;
                dictListElementTypes.Add(key, itemType);

                object item = null;
                Type lstType = typeof(List<>).MakeGenericType(itemType); //动态获取数组类型
                ConstructorInfo[] ctors = lstType.GetConstructors(System.Reflection.BindingFlags.Instance
                                                        | System.Reflection.BindingFlags.NonPublic
                                                        | System.Reflection.BindingFlags.Public);
                foreach (ConstructorInfo ctor in ctors)
                {
                    ParameterInfo[] ps = ctor.GetParameters();
                    if (ps == null || ps.Length == 0)
                        item =  ctor.Invoke(null);
                    else if (ps.Length == 1 && ps[0].ParameterType == typeof(int)) //用于初始化数组类/
                        item = ctor.Invoke(new object[] { 0 });

                }



                return dictLists.TryAdd(key, item/*new List<object>()*/);
            }
            
        }
        public string[] AllListKeys { get { return dictLists.Keys.ToArray(); } }

        public Type GetListElementType(string key)
        {
            if (!dictListElementTypes.ContainsKey(key))
                return null;
            return dictListElementTypes[key];
        }

        public /*List<object>*/object LockList(string key)
        {
            if (!dictListElementTypes.ContainsKey(key))
                return null;
            Monitor.Enter(dictLists[key]);
            return dictLists[key];
            //}
        }

        public void UnlockList(string key)
        {
            if (!dictListElementTypes.ContainsKey(key))
                return;
            Monitor.Exit(dictLists[key]);
        }

        public void SetList(string key,object lstValue)
        {
            if (!dictListElementTypes.ContainsKey(key))
                return;
            Monitor.Enter(dictLists[key]);
            IList lstCurr = dictLists[key] as IList;
            lstCurr.Clear();
            if (null != lstValue)
            {
                //日后添加lstValue元素类型的判断
                IList newValue = lstValue as IList;
                for (int i = 0; i < newValue.Count; i++)
                    lstCurr.Add(newValue[i]);

            }
            Monitor.Exit(dictLists[key]);
        }

        public int GetListCount(string key)
        {
            if (!dictListElementTypes.ContainsKey(key))
                return -1;

            return (dictLists[key] as IList).Count;
        }
        public void ClearList(string key)
        {
            if (!dictListElementTypes.ContainsKey(key))
                return;
            (dictLists[key] as IList).Clear();
        }
        public object PeekList(string key)
        {
            if (!dictListElementTypes.ContainsKey(key))
                return null;
            lock (dictLists[key])
            {
                if (0 == (dictLists[key] as IList).Count)
                    return null;
            
                return (dictLists[key] as IList)[0];
            }
        }
        public bool EnqueList(string key, object element)
        {
            if (!dictListElementTypes.ContainsKey(key))
                return false;
            lock(dictLists[key])
            {
                if(null == element)
                {
                    (dictLists[key] as IList).Add(element);
                        return true;
                }
                if(dictListElementTypes[key].IsAssignableFrom(element.GetType()))
                {
                    (dictLists[key] as IList).Add(element);
                    return true;
                }

                if(JFTypeExt.IsExplicitFrom(dictItemTypes[key], element.GetType()))
                {
                    (dictLists[key] as IList).Add(JFConvertExt.ChangeType(element, dictListElementTypes[key]));
                    return true;
                }
                return false;
            }

        }
        public object DequeList(string key)
        {
            if (!dictListElementTypes.ContainsKey(key))
                return null;
            lock (dictLists[key])
            {
                if ((dictLists[key] as IList).Count == 0)
                    return null;
                object ret = (dictLists[key] as IList)[0];
                (dictLists[key] as IList).RemoveAt(0);
                return ret;
            }
        }

        public object PopList(string key)
        {
            if (!dictListElementTypes.ContainsKey(key))
                return null;
            lock (dictLists[key])
            {
                IList ll = dictLists[key] as IList;
                if (ll.Count == 0)
                    return null;
                object ret = ll[ll.Count -1];
                ll.RemoveAt(ll.Count - 1);
                return ret;
            }
        }
    }
}
