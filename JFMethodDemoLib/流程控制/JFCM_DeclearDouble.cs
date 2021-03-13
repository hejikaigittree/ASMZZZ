using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;

namespace JFMethodCommonLib
{
    [JFVersion("1.0.0")]
    [JFCategoryLevels(new string[] { "流程控制", "变量声明" })]
    [JFDisplayName("Double")]
    public class JFCM_DeclearDouble : IJFMethod, IJFInitializable
    {
        JFCM_DeclearDouble()
        {
            IsInitOK = false;
        }
        string[] _initParams = new string[] { "Double" };
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public string[] InitParamNames { get { return _initParams; } }


        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == InitParamNames[0])
                return JFParamDescribe.Create(name, typeof(double), JFValueLimit.NonLimit, null);
            throw new ArgumentException("初始化参数项名称不存在：" + name);
        }


        double _initVal = 0;
        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public object GetInitParamValue(string name)
        {
            if (name == InitParamNames[0])
                return _initVal;
            throw new ArgumentException("初始化参数项名称不存在：" + name);
        }

        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool SetInitParamValue(string name, object value)
        {
            if (name == InitParamNames[0])
            {
                _initVal = (double)value;
                return true;
            }
            initErrorInfo = "不存在的初始化参数项名称:" + name;
            return false;
        }

        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool Initialize()
        {
            IsInitOK = true;
            initErrorInfo = "Success";
            return true;
        }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get; private set; }


        string initErrorInfo = "未初始化";
        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo()
        {
            return initErrorInfo;
        }





        #region 输入项
        public string[] MethodInputNames { get { return new string[] { }; } }
        public Type GetMethodInputType(string name) { throw new NotImplementedException(); }
        public object GetMethodInputValue(string name) { throw new NotImplementedException(); }
        public void SetMethodInputValue(string name, object value) { throw new NotImplementedException(); }
        #endregion

        #region 输出项
        readonly string[] _outputNames = new string[] { "value" };
        public string[] MethodOutputNames { get { return _outputNames; } }
        public Type GetMethodOutputType(string name)
        {
            if (name == MethodOutputNames[0])
                return typeof(double);
            throw new ArgumentException("不存在的输出项名称:" + name);
        }
        public object GetMethodOutputValue(string name)
        {
            if (name == MethodOutputNames[0])
                return _initVal;

            throw new ArgumentException("不存在的输出项名称:" + name);
        }
        #endregion

        #region 执行
        public bool Action()
        {
            return true;
        }
        public string GetActionErrorInfo()
        {
            return "Success";
        }

        public double GetActionSeconds() { return 0; }

        #endregion


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~JFCM_DeclearString()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
