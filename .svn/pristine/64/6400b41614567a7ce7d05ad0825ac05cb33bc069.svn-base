using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using HalconDotNet;
using System.Windows.Forms;
using System.IO;

namespace JFVision
{
    [JFVersion("1.0.0.0")]
    [JFCategoryLevels(new string[] { "图像方法","加载图片"})]
    [JFDisplayName("加载图片文件")]
    public class JFVM_LoadImage:IJFMethod_Vision, IJFInitializable, IJFConfigUIProvider
    {
        public JFVM_LoadImage()
        {

        }

        ~JFVM_LoadImage()
        {
            Dispose(false);
        }
        #region  IJFInitializable's API
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public string[] InitParamNames { get {return  new string[] { "FilePath" }; } }

        enum ParamCategory
        {
            init,
            methodIn,
            methodOut
        }

        /// <summary>
        /// 检查参数名是否合法
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="name"></param>
        /// <param name="func"></param>
        void _CheckParamName(ParamCategory pc,string name, string func)
        {
            if (null == name)
                throw new ArgumentNullException(string.Format("{0} failed By: name = null! ", func));
            switch(pc)
            {
                case ParamCategory.init:
                    if (!InitParamNames.Contains(name))
                        throw new ArgumentException(string.Format("{0} failed By: name = {1} is not included by InitParamNames:{2}", func, name, string.Join("|", InitParamNames)));
                    break;
                case ParamCategory.methodIn:
                    if (!MethodInputNames.Contains(name))
                        throw new ArgumentException(string.Format("{0} failed By: name = {1} is not included by MethodInputNames:{2}", func, name, string.Join("|", MethodInputNames)));
                    break;
                case ParamCategory.methodOut:
                    if (!MethodOutputNames.Contains(name))
                        throw new ArgumentException(string.Format("{0} failed By: name = {1} is not included by MethodOutputNames:{2}", func, name, string.Join("|", MethodOutputNames)));
                    break;
            }
        }

        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JFParamDescribe GetInitParamDescribe(string name) 
        {
            _CheckParamName( ParamCategory.init,name, "GetInitParamDescribe(name)");
            return JFParamDescribe.Create(name, typeof(string), JFValueLimit.FilePath,null);
        }
        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public object GetInitParamValue(string name) 
        {
            _CheckParamName(ParamCategory.init, name, "GetInitParamValue(name)");
            return _filePath;
        }


        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool SetInitParamValue(string name, object value)
        {
            _CheckParamName(ParamCategory.init,name, "SetInitParamValue(name,value)");
            if (null == value)
            {
                _filePath = string.Empty;
                return true;
            }
            if (!typeof(string).IsAssignableFrom(value.GetType()))
                throw new Exception("SetInitParamValue(name = \"FilePath\",value) failed By: value's type is not String");
            _filePath = value as string;
            return true; 
        }

        /// <summary>
        /// 对象初始化 , 无论FilePath是否设置，文件是否存在都返回true
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool Initialize() { return true; }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get { return true; } }

        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo() { return "Success"; }
        #endregion //IJFInitializable's API

        #region IJFMethod's API
        string _filePath = string.Empty;
        public string[] MethodInputNames { get { return new string[] { /*"FilePath"*/ }; } }
        public Type GetMethodInputType(string name) 
        {
            _CheckParamName(ParamCategory.methodIn, name, "GetMethodInputType(name)");
            //if(name == "FilePath")
                return typeof(string);
            //throw new Exception("GetMethodInputType(name = \"" + name + "\") failed By name is not legal inputParam's Name");
        }
        public object GetMethodInputValue(string name) 
        {
            _CheckParamName(ParamCategory.methodIn, name, "GetMethodInputValue(name)");
            //if (name == "FilePath")
            return _filePath;
            //throw new Exception("GetMethodInputValue(name = \"" + name + "\") failed By name is not legal inputParam's Name");
        }
        public void SetMethodInputValue(string name, object value)
        {
            _CheckParamName(ParamCategory.methodIn, name, "SetMethodInputValue(name,value)");
            if(null == value)
            {
                _filePath = string.Empty;
                return;
            }
            //if(!typeof(string).IsAssignableFrom(value.GetType()))
            //    throw new Exception("SetMethodInputValue(name = \"FilePath\",value) failed By: value's type is not String");
            _filePath = (string)value;//value as string;

        }

        public string[] MethodOutputNames { get { return new string[] { "Image" }; } }
        public Type GetMethodOutputType(string name)
        {
            _CheckParamName(ParamCategory.methodOut, name, "GetMethodOutputType(name,value)");
            //if (name == "Iamge")
                return typeof(IJFImage);
        }
        IJFImage _image = null;
        public object GetMethodOutputValue(string name)
        {
            _CheckParamName(ParamCategory.methodOut, name, "GetMethodOutputValue(name)");
            return _image;
        }

        long _startPfCnt = 0; //动作开始时CPU计数
        long _endPfCnt = 0;//动作结束时CPU计数
        long _PfFrequency = JFFunctions.PerformanceFrequency();
        public bool Action()
        {
            _startPfCnt = JFFunctions.PerformanceCounter();
            _endPfCnt = _startPfCnt;
            if (string.IsNullOrEmpty(_filePath))
            {
                _actionErrorInfo = "图片文件路径未设置";
                _endPfCnt = JFFunctions.PerformanceCounter();
                return false;
            }
            if(!File.Exists(_filePath))
            {
                _actionErrorInfo = "Failed By: _filePath = \"" + _filePath + "\" is not existed";
                _endPfCnt = JFFunctions.PerformanceCounter();
                return false;
            }
            //if(_image != null)
            //{
            //    _image.Dispose();
            //    _image = null;
            //}
            try
            {
                HObject ho;
                HOperatorSet.ReadImage(out ho, _filePath);
                _image = new JFImage_Hlc(ho, 0);
            }
            catch(Exception ex)
            {
                _actionErrorInfo = "Failed By:Exception =" + ex.ToString();
                _endPfCnt = JFFunctions.PerformanceCounter();
                return false;
            }
            _endPfCnt = JFFunctions.PerformanceCounter();
            _actionErrorInfo = "Success";
            return true;
        }
        string _actionErrorInfo = "No_Option";
        public string GetActionErrorInfo()
        {
            return _actionErrorInfo;
        }

        public double GetActionSeconds()
        {
            if (_PfFrequency == 0)
                return 0;
            return (_endPfCnt -_startPfCnt) / _PfFrequency;
        }

        #endregion //IJFMethod's API

        ///IJFRealtimeUIProvider's API
        public JFRealtimeUI GetRealtimeUI()
        {
            UcRTLoadImage1 ui = new UcRTLoadImage1();//UcRTLoadImg ui = new UcRTLoadImg();
            ui.SetMethodObj(this);
            return ui;
        }

        ///IJFConfigUIProvider's API
        public void ShowCfgDialog()
        {
            //MessageBox.Show("\"加载图片文件方法\"没有配置项");
            Form cfgDemo = new Form();
            cfgDemo.Text = "参数/配置窗口-演示";
            cfgDemo.ShowDialog();
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
