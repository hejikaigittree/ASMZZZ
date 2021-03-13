using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace JFToolKits
{
    /// <summary>
    /// JFVersionAttribute：为实体类添加版本标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class JFVersionAttribute : Attribute
    {
        public JFVersionAttribute(string info)
        {
            Info = info;
        }
        public string Info{ get; set; }
             
    }
}
