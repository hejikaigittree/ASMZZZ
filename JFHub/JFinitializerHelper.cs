using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using JFInterfaceDef;
using JFToolKits;

namespace JFHub
{
    /// <summary>
    /// 对使用IJFInializable功能的程序提供帮助
    /// </summary>
    public  class JFinitializerHelper
    {
        internal JFinitializerHelper()
        {
            appendDlls = new List<string>();
            InitializableTypes = new List<Type>();
            //先将本程序集的类型提取出来
            //Module[]  mds = Assembly.getLo
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach(Assembly ass in assemblies)
            {
                Type[] ts = ass.GetTypes();
                foreach (Type t in ts)
                    if (typeof(IJFInitializable).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                        if (!InitializableTypes.Contains(t))
                            InitializableTypes.Add(t);
            }
        }


        //private static readonly Lazy<JFinitializerHelper> lazy = new Lazy<JFinitializerHelper>(() => new JFinitializerHelper());
        //public static JFinitializerHelper Instance { get { return lazy.Value; } }
         List<string> appendDlls; //保存程序使用的拓展dll路径
         List<Type> InitializableTypes;

        public  string[] AllApendDllPaths()
        {
            return appendDlls.ToArray();
        }


        /// <summary>添加一个支持IJFInializable接口的附加库</summary>
        /// <param name="dllPath">dll文件路径</param>
        public  void AppendDll(string dllPath)
        {
            if (string.IsNullOrWhiteSpace(dllPath))
                throw new ArgumentNullException("JFinitializerHelper.AppendDll(dllPath) fialed By: dllPath is null or whitespace");

            string dllName = Path.GetFileNameWithoutExtension(dllPath);
            if (string.IsNullOrWhiteSpace(dllName))
                throw new ArgumentException("JFinitializerHelper.AppendDll(dllPath) fialed By:dllPath's filename is empty!");
           
            string dllExtention = Path.GetExtension(dllPath);
            if (dllExtention == string.Empty)
                dllPath += ".dll";
            else
            {
                if(dllExtention != ".dll")
                    throw new ArgumentException("JFinitializerHelper.AppendDll(dllPath) fialed By:dllPath = " + dllPath + " isnot a dll file");
            }
            string fullPath = Path.GetFullPath(dllPath);
            if(!File.Exists(fullPath))
                throw new ArgumentNullException("JFinitializerHelper.AppendDll(dllPath) fialed By: dllPath = " + dllPath + " is not Existed");

            if (appendDlls.Contains(dllPath))
                return;
            
            Type[] ts = InstantiatedClassesInDll(fullPath);
            if (null != ts)
                foreach (Type t in ts)
                    if (!InitializableTypes.Contains(t))
                        InitializableTypes.Add(t);
            appendDlls.Add(fullPath);
        }

        /// <summary>移除指定的dll库和相应的类型</summary>
        public  void RemoveDll(string dllPath)
        {
            if (string.IsNullOrWhiteSpace(dllPath))
                return;

            string dllName = Path.GetFileNameWithoutExtension(dllPath);
            if (string.IsNullOrWhiteSpace(dllName))
                return;

            string dllExtention = Path.GetExtension(dllPath);
            if (dllExtention == string.Empty)
                dllPath += ".dll";
            else
            {
                if (dllExtention != ".dll")
                    return;
            }
            string fullPath = Path.GetFullPath(dllPath);
            if (appendDlls.Contains(dllPath))
                appendDlls.Remove(dllPath);
            if (!File.Exists(fullPath))
                return;
            
            Type[] ts = InstantiatedClassesInDll(dllPath);
            if (null == ts)
                return;
            foreach (Type t in ts)
                if (InitializableTypes.Contains(t))
                    InitializableTypes.Remove(t);
        }

        public void ClearDll()
        {
            appendDlls.Clear();
            InitializableTypes.Clear();
        }



        //获取一个Dll中所有支持JFInitializable接口的实体类型
        public Type[] InstantiatedClassesInDll(string dllPath)
        {
            if (string.IsNullOrWhiteSpace(dllPath))
                throw new ArgumentNullException("InstantiatedClassesInDll.AppendDll(dllPath) fialed By: dllPath is null or whitespace");

            string dllName = Path.GetFileNameWithoutExtension(dllPath);
            if (string.IsNullOrWhiteSpace(dllName))
                throw new ArgumentException("InstantiatedClassesInDll.AppendDll(dllPath) fialed By:dllPath's filename is empty!");

            string dllExtention = Path.GetExtension(dllPath);
            if (dllExtention == string.Empty)
                dllPath += ".dll";
            else
            {
                if (dllExtention != ".dll")
                    throw new ArgumentException("InstantiatedClassesInDll.AppendDll(dllPath) fialed By:dllPath = " + dllPath + " isnot a dll file");
            }
            string fullPath = Path.GetFullPath(dllPath);
            if (!File.Exists(fullPath))
                throw new ArgumentNullException("InstantiatedClassesInDll.AppendDll(dllPath) fialed By: dllPath = " + dllPath + " is not Existed");

            List<Type> ret = new List<Type>();
            Assembly ass = Assembly.LoadFrom(dllPath);
            Type[] ts = ass.GetTypes();
            foreach (Type t in ts)
                if (typeof(IJFInitializable).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    ret.Add(t);
            return ret.ToArray();
        }


        /// <summary>获取整个程序（包括附加的dll库）中支持JFInitializable接口的实体类 </summary>
        /// <returns></returns>
        public  Type[] InstantiatedClasses()
        {
            return InitializableTypes.ToArray();
        }

        /// <summary>获取整个程序中支持JFInitializable接口，并且继承于superClass的实体类
        public Type[] InstantiatedClasses(Type superClass)
        {
            List<Type> ret = new List<Type>();
            foreach(Type t in InitializableTypes)
                if(superClass.IsAssignableFrom(t))
                    ret.Add(t);
            return ret.ToArray();
        }

        /// <summary>创建一个继承于IJFInitializable 的类对象</summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IJFInitializable CreateInstance(Type t)
        {
            if (t == null)
                throw new ArgumentNullException("CreateInstance(Type t) failed By: t = null");
            if (!typeof(IJFInitializable).IsAssignableFrom(t) || !t.IsClass || t.IsAbstract)
                throw new ArgumentException("CreateInstance(Type t) failed By: t is not an  entity class inherited from IJFInitializable");
            ConstructorInfo[] ctors = t.GetConstructors(System.Reflection.BindingFlags.Instance
                                                          | System.Reflection.BindingFlags.NonPublic
                                                          | System.Reflection.BindingFlags.Public);
            if (null == ctors)
                throw new Exception("CreateInstance(Type t) failed By: Not found t-Instance's Constructor");
            foreach(ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] ps = ctor.GetParameters();
                if (ps == null || ps.Length == 0)
                    return ctor.Invoke(null) as IJFInitializable;
            }

            return null;
        }

        public IJFInitializable CreateInstance(string typeName)
        {
            return CreateInstance(Type.GetType(typeName));
        }

        /// <summary>
        /// 获取JFInitialize类型的显示名
        /// </summary>
        /// <param name="it"></param>
        /// <returns></returns>
        public static string DispalyTypeName(Type it)
        {
            JFDisplayNameAttribute[] vn = it.GetCustomAttributes(typeof(JFDisplayNameAttribute), false) as JFDisplayNameAttribute[];
            if (null != vn && vn.Length > 0)
                return vn[0].Name;
            else
                return it.Name;
        }

    }
}
