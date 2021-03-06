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
    /// 需要访问外部动作流数据池的接口
    /// </summary>
    interface IJFMethodOutterDataPoolAcq
    {
        void SetOutterDataPool(Dictionary<string, object> outterDataPool, Dictionary<string, Type> outterTypePool, string[] outterAvailedIDs);
    }
    /// <summary>
    /// 可以包含多个子方法
    /// </summary>
    public abstract class JFMethodCollectionBase : IJFMethod_T,IJFStationBaseAcq, IJFRealtimeUIProvider,IJFInitializable,IJFConfigUIProvider,IJFMethodOutterDataPoolAcq
    {
        public JFMethodCollectionBase()
        {
            //innerFlow = new JFMethodFlow();
        }
        #region IJFMethodSB's API
        public abstract string[] MethodInputNames { get; }
        public abstract Type GetMethodInputType(string name);
        public abstract object GetMethodInputValue(string name);
        public abstract void SetMethodInputValue(string name, object value);

        public abstract string[] MethodOutputNames { get; }
        public abstract Type GetMethodOutputType(string name);

        public abstract object GetMethodOutputValue(string name);


        long _actionStartCpuCount = 0;
        long _actionStopCpuCount = 0;
        public virtual bool Action()
        {
            _actionStartCpuCount = JFFunctions.PerformanceCounter();
            bool isOK = innerFlow.Action();
            _actionStopCpuCount = JFFunctions.PerformanceCounter();
            return isOK;
        }
        public virtual string GetActionErrorInfo()
        {
            return innerFlow.ActionErrorInfo();
        }
        public virtual double GetActionSeconds()
        {
            return ((double)(_actionStopCpuCount - _actionStartCpuCount)) / JFFunctions.PerformanceFrequency();
        }

        JFStationBase _station;
        public void SetStation(JFStationBase station)
        {
            _station = station;
            innerFlow.SetStation(station);
        }


        #endregion //IJFMethodSB's API



        UcRTMethodCollection _rtUI = new UcRTMethodCollection();
        public JFRealtimeUI GetRealtimeUI()
        {
            _rtUI.SetInnerFlow(innerFlow);
            return _rtUI;
            //return null;
        }



        string[] _initParamNames = new string[] { "InnerMethodFlowText" }; //内部数据流转化成的文本
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public virtual string[] InitParamNames { get { return _initParamNames; } }


        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns>如果name不在InitParamNames中，则返回一个空值</returns>
        public virtual JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == "InnerMethodFlowText")
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.NonLimit, null);
            return null;
        }

        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，返回null</param>
        /// <returns>参数值</returns>
        public virtual object GetInitParamValue(string name)
        {
            if (name == "InnerMethodFlowText")
                return innerFlow.ToTxt();
            return null;
        }

        string _initErrorInfo = "No-Ops";
        bool _isInitOK = true;
        string _flowTxt = null; //方法流转化文本
        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public virtual bool SetInitParamValue(string name, object value)
        {
            if(name == "InnerMethodFlowText")
            {
                if(null != value && value.GetType() != typeof(string))
                {
                    _initErrorInfo = "错误的参数类型，Name = " + name + ",Type = " + value.GetType().ToString();
                    _isInitOK = false;
                    return false;
                }
                _flowTxt = value as string;
                return true;
            }
            _initErrorInfo = "不支持的初始化参数，Name = " + name;
            return false;
        }

        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public virtual bool Initialize()
        {
            innerFlow.Clear();
            try
            {
                bool isOK = innerFlow.FromTxt(_flowTxt);
                if(!isOK)
                {
                    _isInitOK = false;
                    _initErrorInfo = "未能从参数Name =\"InnerMethodFlowText\"中加载内部数据流";
                    return false;
                }
                _isInitOK = true;
                _initErrorInfo = "Success";
                return true;
            }
            catch(Exception ex)
            {
                _isInitOK = false;
                _initErrorInfo = "从参数Name =\"InnerMethodFlowText\"中加载内部数据流发生异常:" + ex.ToString();
                return false;
            }
        }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public virtual bool IsInitOK { get { return _isInitOK; } }

        /// <summary>获取初始化错误的描述信息</summary>
        public virtual string GetInitErrorInfo()
        {
            return _initErrorInfo;
        }


        public virtual void Dispose()
        {
            return;
        }



        ///// <summary>
        ///// 外部（上层）数据池对象
        ///// </summary>
        //Dictionary<string, object> _outterDataPool = null;
        ///// <summary>
        ///// 外部类型池
        ///// </summary>
        //Dictionary<string, Type> _outterTypePool = null;

        ///// <summary>
        ///// 可用的外部数据ID
        ///// </summary>
        //public string[] _outterDataIDs = null;

        public void SetOutterDataPool(Dictionary<string,object> outterDataPool,Dictionary<string,Type> outterTypePool,string[] outterAvailedIDs)
        {
            //_outterDataPool = outterDataPool;
            //_outterTypePool = outterTypePool;
            //_outterDataIDs = outterAvailedIDs;
            innerFlow.SetOutterDataPool(outterDataPool, outterTypePool, outterAvailedIDs);
        }

        public void ShowCfgDialog()
        {
            FormMethodCollectionConfigUI cfgUI = new FormMethodCollectionConfigUI();
            cfgUI.SetMethodFlow(innerFlow);
            cfgUI.ShowDialog();
        }

        public virtual void Pause()
        {
            innerFlow.Pause();
        }

        public virtual void Resume()
        {
            //throw new NotImplementedException();
            innerFlow.Resume();
        }

        public virtual void Exit()
        {
            if (innerFlow.Stop(innerFlow.CycleMilliseconds*5) != JFWorkCmdResult.Success)
                innerFlow.Abort();
        }

        JFMethodFlow innerFlow = new JFMethodFlow();

        protected JFMethodFlow InnerFlow { get { return innerFlow; } }

        



    }
}
