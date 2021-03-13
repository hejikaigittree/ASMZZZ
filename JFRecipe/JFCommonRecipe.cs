using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;

namespace JFRecipe
{
    public class JFCommonRecipe:IJFRecipe
    {
        public JFCommonRecipe()
        {

            Dict = new JFXmlDictionary<string, object>();
        }
        /// <summary>
        /// 产品/配方 类别 比如：托盘 产品 等
        /// </summary>
        public string Categoty { get; internal set; }

        /// <summary>
        /// 产品/配方 名称 
        /// </summary>
        public string ID { get; internal set; }


        /// <summary>
        /// 获取所有配置项名称
        /// </summary>
        /// <returns></returns>
        public  string[] AllItemNames()
        {
            return Dict.Keys.ToArray(); 
        }

        /// <summary>
        /// 配置项类型
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public Type ItemType(string itemName)
        {
            if (!Dict.ContainsKey(itemName))
                throw new ArgumentOutOfRangeException();
            return Dict[itemName].GetType();
            
        }

        public object GetItemValue(string itemName)
        {
            return Dict[itemName];
        }

        public void SetItemValue(string itemName, object itemValue)
        {
            Dict[itemName] = itemValue;
        }

        public void AddItem(string itemName, object itemValue)
        {
            Dict.Add(itemName, itemValue);
        }

        public void RemoveItem(string itemName)
        {
            Dict.Remove(itemName);
        }

        public JFXmlDictionary<string, object> Dict { get; internal set; }


        
    }
}
