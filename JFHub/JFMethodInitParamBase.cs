using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
namespace JFHub
{
    /// <summary>
    /// 使用了初始化参数的方法基类
    /// </summary>
    public abstract class JFMethodInitParamBase:JFMethodBase,IJFInitializable
    {
        /// <summary>
        /// 初始化参数项的描述/值
        /// </summary>
        public class InitParamDV
        {
            public InitParamDV(JFParamDescribe paramDescribe, object paramValue)
            {
                if (null == paramDescribe)
                    throw new ArgumentException("paramDescribe = null in InitParamDV->Ctor()");
                ParamDescribe = paramDescribe;
                ParamValue = paramValue;
            }

            public JFParamDescribe ParamDescribe{get;private set;}

            object paramValue = null;
            public object ParamValue 
            {
                get { return paramValue; }
                set
                {
                    if (null == value)
                    {
                        if(ParamDescribe.ParamType.IsValueType)//if (!JFTypeExt.IsNullableType(ParamDescribe.ParamType))
                            throw new ArgumentException("类型：" + ParamDescribe.ParamType.Name + " 的对象值不能为空 in ParamTypeValue->SetParamValue");
                        paramValue = value;
                        return;
                    }
                    //值非空
                    Type srcType = value.GetType();
                    Type dstType = ParamDescribe.ParamType;
                    if (dstType == srcType || JFTypeExt.IsImplicitFrom(dstType, srcType))//if (dstType == srcType ||dstType.IsAssignableFrom(srcType))//类型完全匹配
                        paramValue = value;
                    else if (JFTypeExt.IsExplicitFrom(dstType, srcType))//可以进行强制转换
                        paramValue = JFConvertExt.ChangeType(value, dstType);
                    else if (!JFTypeExt.IsExplicitFrom(dstType, srcType)) //不可以进行强制转化的类型
                        throw new ArgumentException("无法将类型:\"" + srcType.Name + "\"的变量转化为类型:\"" + dstType + "\"");
                }
            }
            

        }

        protected JFMethodInitParamBase()
        {
            IsInitOK = false;
            InitErrorInfo = "None-Opt";
        }
        List<string> _initParamNames = new List<string>();
        Dictionary<string, InitParamDV> _initParams = new Dictionary<string, InitParamDV>();
        

        /// <summary>
        /// 申明一个初始化参数项,建议只在继承类的构造函数中使用
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="initValue"></param>
        protected void DeclearInitParam(string name,Type type,object initValue)
        {
            DeclearInitParam(JFParamDescribe.Create(name, type, JFValueLimit.NonLimit, null), initValue);
        }

        protected void DeclearInitParam(JFParamDescribe describ, object initValue)
        {
            if (string.IsNullOrEmpty(describ.DisplayName))
                throw new ArgumentException("name is null or empty in JFMethodInitParamBase->DeclearInitParam()");
            if (_initParamNames.Contains(describ.DisplayName))
                throw new ArgumentException("name is existed in JFMethodInitParamBase->DeclearInitParam()");
            
            _initParamNames.Add(describ.DisplayName);
            _initParams.Add(describ.DisplayName, new InitParamDV(describ, initValue));
        }

        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public string[] InitParamNames { get { return _initParamNames.ToArray(); } }


        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual JFParamDescribe GetInitParamDescribe(string name)
        {
            return _initParams[name].ParamDescribe;
        }

        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public virtual object GetInitParamValue(string name)
        {
            return _initParams[name].ParamValue;
        }

        /// <summary>
        /// 需要在设置/过滤初始化参数的值时，需要复写此函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool SetInitParamValue(string name, object value)
        {
            _initParams[name].ParamValue = value;
            return true;
        }

        /// <summary>
        /// 对象初始化，函数默认调用继承类的InitializeGenuine
        /// 继承类不需要复写此函数
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public virtual bool Initialize()
        {
            string errInfo;
            IsInitOK = InitializeGenuine(out errInfo);
            InitErrorInfo = errInfo;
            return IsInitOK;
        }

        /// <summary>
        /// 继承类中实现此函数
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        protected abstract bool InitializeGenuine(out string errorInfo);


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get; protected set; }

        /// <summary>
        /// 继承类中在初始化发生错误时，需设置此属性的值
        /// </summary>
        protected string InitErrorInfo { get; set; }
        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo() { return InitErrorInfo; }


        public override bool Action()
        {
            if(!IsInitOK)
            {
                ActionErrorInfo = "未初始化:" + InitErrorInfo;
                return false;
            }
            return  base.Action();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~JFMethodInitParamBase()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
