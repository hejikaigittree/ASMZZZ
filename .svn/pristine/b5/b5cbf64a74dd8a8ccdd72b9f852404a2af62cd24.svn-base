using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace JFInterfaceDef
{
    /// <summary>
    /// 为指定项提供一个简单易读的名称
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class JFDisplayNameAttribute : Attribute
    {
        public JFDisplayNameAttribute(string name)
        {
            Name = name;
        }
        public string Name{get;set;}
    }
}
