using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using JFHub;

namespace JFMethodCommonLib
{
    [JFCategoryLevels(new string[] { "JF 设备控制" })]
    [JFDisplayName("打开/关闭设备_S")]
    public class JFCM_OpenCloseDev_S : IJFMethod, IJFInitializable
    {
        public JFCM_OpenCloseDev_S()
        {
            IsInitOK = false;
        }
        readonly string[] _initParamNames = new string[] { "设备ID","开关选项" };
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public string[] InitParamNames { get { return _initParamNames; } }


        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JFParamDescribe GetInitParamDescribe(string name)
        {
            if(name == InitParamNames[0])
            {
                JFInitorManager devMgr = JFHubCenter.Instance.InitorManager;
                string[] allDevIDs = devMgr.GetIDs(typeof(IJFDevice));
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.Options, allDevIDs);
            }
            else if(name == InitParamNames[1])
            {
                return JFParamDescribe.Create(name, typeof(bool), JFValueLimit.NonLimit,null);
            }

            throw new ArgumentException("非法的输入参数项，名称:" + name);
        }

        string _devID = null;
        bool _isOpen = true;
        string _initErrorInfo = "None-Opt";

        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public object GetInitParamValue(string name)
        {
            if (name == InitParamNames[0])
                return _devID;
            else if (name == InitParamNames[1])
                return _isOpen;
            throw new ArgumentException("非法的输入参数项，名称:" + name);
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
                string devID = value as string;
                if(string.IsNullOrEmpty(devID))
                {
                    _initErrorInfo = "参数\"" + name + "\"的值为空字串";
                    return false;
                }

                JFInitorManager devMgr = JFHubCenter.Instance.InitorManager;
                if(!devMgr.ContainID(devID))
                {
                    _initErrorInfo = "DevID = " + devID + " 在设备列表中不存在";
                    return false;
                }
                _devID = devID;
                return true;
            }
            else if(name == InitParamNames[1])
            {
                _isOpen = (bool)value;
                return true;
            }
            throw new ArgumentException("非法的输入参数项，名称:" + name);
        }

        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool Initialize()
        {
            if(string.IsNullOrEmpty(_devID))
            {
                _initErrorInfo = "参数项:\"" + InitParamNames[0] + "\"未设置/空值";
                IsInitOK = false;
                return false;
            }
            JFInitorManager devMgr = JFHubCenter.Instance.InitorManager;
            if(!devMgr.ContainID(_devID))
            {
                IsInitOK = false;
                _initErrorInfo = "DevID = " + _devID + " 在设备列表中不存在";
                return false;
            }
            IsInitOK = true;
            _initErrorInfo = "Success";
            return true;
        }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get; private set; }

        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo()
        {
            return _initErrorInfo;
        }

        #region 输入项
        public string[] MethodInputNames { get { return new string[] { }; } }
        public Type GetMethodInputType(string name) { throw new NotImplementedException(); }
        public object GetMethodInputValue(string name){throw new NotImplementedException();}
        public void SetMethodInputValue(string name, object value) { throw new NotImplementedException(); }
        #endregion

        #region 输出项
        public string[] MethodOutputNames { get { return new string[] { }; } }
        public Type GetMethodOutputType(string name) { throw new NotImplementedException(); }
        public object GetMethodOutputValue(string name) { throw new NotImplementedException(); }
        #endregion

        #region 执行
        string _actionErrorInfo = "未执行";
        double _startPfCnt = 0; //动作开始时CPU计数
        double _endPfCnt = 0;//动作结束时CPU计数
        double _PfFrequency = JFFunctions.PerformanceFrequency();
        public bool Action()
        {
            _startPfCnt = JFFunctions.PerformanceCounter();
            if(!IsInitOK)
            {
                _endPfCnt = JFFunctions.PerformanceCounter();
                _actionErrorInfo = "初始化未完成";
                return false;
            }
            IJFDevice dev = JFHubCenter.Instance.InitorManager.GetInitor(_devID) as IJFDevice;
            if(null == dev)
            {
                _endPfCnt = JFFunctions.PerformanceCounter();
                _actionErrorInfo = "设备不存在，DevID = " + _devID;
                return false;
            }

            int errCode = 0;
            if (_isOpen)
                errCode = dev.OpenDevice();
            else
                errCode = dev.CloseDevice();
            _endPfCnt = JFFunctions.PerformanceCounter();
            if (0 != errCode)
            {
                _actionErrorInfo = (_isOpen ? "打开" : "关闭") + "设备失败:" + dev.GetErrorInfo(errCode);
                return false;
            }
            _actionErrorInfo = "Success";
            return true;
        }
        public string GetActionErrorInfo() { return _actionErrorInfo; }

        public double GetActionSeconds()
        {
            return (_endPfCnt - _startPfCnt) / _PfFrequency;
        }

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
        // ~JFCM_OpenCloseDev_S()
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
