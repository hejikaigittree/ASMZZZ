using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace JFInterfaceDef
{
    /// <summary>
    /// 定义类别特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class JFCategoryLevelsAttribute:Attribute
    {
        public JFCategoryLevelsAttribute(string category)
        {
            if (string.IsNullOrEmpty(category))
                Levels = new string[] { };
            else
                Levels = new string[] { category };
        }

        public JFCategoryLevelsAttribute(string[] categoryLevels)
        {
            if (null == categoryLevels || 0 == categoryLevels.Length)
                Levels = new string[] { };
            else
            {
                List<string> lstCl = new List<string>();
                foreach (string cl in categoryLevels)
                    if (!string.IsNullOrWhiteSpace(cl))
                        lstCl.Add(cl);
                Levels = lstCl.ToArray();
            }
        }

        /// <summary>
        /// 获取类别描述信息：主类别 -〉1级子类别 -〉2级子类别 -> etc...
        /// </summary>
        public string[] Levels { get; private set; }
    }
}
