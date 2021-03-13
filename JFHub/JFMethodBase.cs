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
    /// 方法基类
    /// </summary>
    public abstract class JFMethodBase : IJFMethod
    {
        public class ParamTypeValue
        {
            //ParamTypeValue() : this(typeof(object), null)
            //{

            //}
            public ParamTypeValue(Type type, object value)
            {
                if (null == type)
                    throw new ArgumentException("type = null in ParamTypeValue(Type type, object value)");
                ParamType = type;
                ParamValue = value;
            }


            public Type ParamType { get; private set; }

            object paramValue = null;
            public object ParamValue
            {
                get { return paramValue; }
                set
                {
                    if (null == value)
                    {
                        if(ParamType.IsValueType)//if (!JFTypeExt.IsNullableType(ParamType))
                            throw new ArgumentException("类型：" + ParamType.Name + " 的对象值不能为空 in ParamTypeValue->SetParamValue");
                        paramValue = value;
                        return;
                    }
                    //值非空
                    Type srcType = value.GetType();
                    Type dstType = ParamType;
                    if (dstType == srcType || JFTypeExt.IsImplicitFrom(dstType, srcType))//if (dstType == srcType ||dstType.IsAssignableFrom(srcType))//类型完全匹配
                        paramValue = value;
                    else if (JFTypeExt.IsExplicitFrom(dstType, srcType))//可以进行强制转换
                        paramValue = JFConvertExt.ChangeType(value, dstType);
                    else if (!JFTypeExt.IsExplicitFrom(dstType, srcType)) //不可以进行强制转化的类型
                        throw new ArgumentException("无法将类型:\"" + srcType.Name + "\"的变量转化为类型:\"" + dstType + "\"");
                    }
                }

            }

        
        protected JFMethodBase()
        {
            ActionErrorInfo = "None-Opt";
        }

        List<string> _inputParamNames = new List<string>();
        Dictionary<string, ParamTypeValue> _inputParams = new Dictionary<string, ParamTypeValue>();

        List<string> _outputParamNames = new List<string>();
        Dictionary<string, ParamTypeValue> _outputParams = new Dictionary<string, ParamTypeValue>();


        /// <summary>
        /// 声明一个输入参数项 ，建议只在继承类构造函数中使用
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramValue"></param>
        protected void DeclearInput(string paramName,Type paramType,object paramValue)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException("paramName is null or empty in JFMethodBase->DelearInput()");
            if(_inputParamNames.Contains(paramName))
                throw new ArgumentException("paramName is existed in JFMethodBase->DelearInput()");
            _inputParamNames.Add(paramName);
            _inputParams.Add(paramName, new ParamTypeValue(paramType, paramValue));
        }

        /// <summary>
        /// 声明一个输出参数项 ，建议只在继承类构造函数中使用
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramValue"></param>
        protected void DeclearOutput(string paramName, Type paramType, object paramValue)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException("paramName is null or empty in JFMethodBase->DelearOutput()");
            if (_outputParamNames.Contains(paramName))
                throw new ArgumentException("paramName is existed in JFMethodBase->DelearOutput()");
            _outputParamNames.Add(paramName);
            _outputParams.Add(paramName, new ParamTypeValue(paramType, paramValue));
        }

        /// <summary>
        /// 供继承类使用，Action执行完成后对Output赋值
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        protected virtual void SetOutputParamValue(string paramName,object paramValue)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("SetOutputValue(string paramName,...) param is null or empty in JFMethodBase");

            if(!_outputParamNames.Contains(paramName))
                throw new ArgumentNullException("SetOutputValue(string paramName = \"" + paramName + "\" 非法的参数名称");

            _outputParams[paramName].ParamValue = paramValue;

        }


        #region 输入项
        public  string[] MethodInputNames { get { return _inputParamNames.ToArray(); } }
        public Type GetMethodInputType(string name)
        {
            return _inputParams[name].ParamType;
        }
        public object GetMethodInputValue(string name)
        {
            return _inputParams[name].ParamValue;
        }
        /// <summary>
        /// 对于需要过滤/处理的输入值，可以复写次函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public virtual void SetMethodInputValue(string name, object value)
        {
            _inputParams[name].ParamValue = value;
        }
        #endregion

        #region 输出项
        public string[] MethodOutputNames { get { return _outputParamNames.ToArray(); } }
        public Type GetMethodOutputType(string name)
        {
            return _outputParams[name].ParamType;
        }
        public object GetMethodOutputValue(string name)
        {
            return _outputParams[name].ParamValue;
        }
        #endregion

        /// <summary>
        /// 返回给用户的错误信息
        /// 在Action中记录错误
        /// </summary>
        protected string ActionErrorInfo { get; set; } 

        #region 执行
        public virtual bool Action()
        {
            StartActionTimer();
            string error;
            bool ret = ActionGenuine(out error);
            ActionErrorInfo = error;
            StopActionTimer();
            return ret;
        }

        protected abstract bool ActionGenuine(out string errorInfo);
        public string GetActionErrorInfo() { return ActionErrorInfo; }

        double _startPerformanceCounter = 0;
        double _stopPerformanceCounter = 0;
        double _performanceFrq = JFFunctions.PerformanceFrequency();

        protected void StartActionTimer()
        {
            _startPerformanceCounter = JFFunctions.PerformanceCounter();
        }

        protected void StopActionTimer()
        {
            _stopPerformanceCounter = JFFunctions.PerformanceCounter();
        }



        public double GetActionSeconds()
        {
            return (_stopPerformanceCounter - _startPerformanceCounter) / _performanceFrq;
        }
        #endregion
    }
}
