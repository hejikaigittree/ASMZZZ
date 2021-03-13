using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    ///  产品/配方接口类
    /// </summary>
    public interface IJFRecipe
    {
        /// <summary>
        /// 产品/配方 类别 比如：托盘 产品 等
        /// </summary>
        string Categoty { get; }

        /// <summary>
        /// 产品/配方 名称 
        /// </summary>
        string ID { get; }


        /// <summary>
        /// 获取所有配置项名称
        /// </summary>
        /// <returns></returns>
        string[] AllItemNames();

        /// <summary>
        /// 配置项类型
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        Type ItemType(string itemName);

        object GetItemValue(string itemName);

        void SetItemValue(string itemName, object itemValue);

    }
}
