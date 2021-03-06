using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;

namespace JFHub
{
    [JFVersion("1.0.0")]
    [JFCategoryLevels(new string[] { "流程控制"})]
    [JFDisplayName("FOR")]
    public class JFMethodFor : JFMethodCollectionBase
    {
        public JFMethodFor()
        {

        }


        #region 输入项
        string[] _inputNames = new string[] { "初始值", "结束值", "增量值", "包含结束" };
        double _startValue = 0; //循环变量初始置
        double _endValue = 0;//循环结束条件值
        double _interval = 0; //循环步距
        double _forValue; //循环变量的当前值
        bool _isIncludeEnd = false; //是否包含结束值
        public override string[] MethodInputNames { get { return _inputNames; } }
        public override Type GetMethodInputType(string name)
        {
            if (name == MethodInputNames[0] || name == MethodInputNames[1] || name == MethodInputNames[2])
                return typeof(double);
            else if (name == MethodInputNames[3])
                return typeof(bool);
            else
                throw new ArgumentException(string.Format("GetMethodInputType(string name = {0})failed by name isnot included by  MethodInputNames", name));
        }
        public override object GetMethodInputValue(string name)
        {
            if (name == _inputNames[0])
                return _startValue;
            else if (name == _inputNames[1])
                return _endValue;
            else if (name == _inputNames[2])
                return _interval;
            else if (name == _inputNames[3])
                return _isIncludeEnd;
            else
                throw new ArgumentException(string.Format("GetMethodInputValue(string name = {0})failed by name isnot included by  MethodInputNames", name));

        }
        public override void SetMethodInputValue(string name, object value)
        {
            if (name == _inputNames[0])
                _startValue = (double)value;
            else if (name == _inputNames[1])
                _endValue = (double)value;
            else if (name == _inputNames[2])
                _interval = (double)value;
            else if (name == _inputNames[3])
                _isIncludeEnd = (bool)value;
            else
                throw new ArgumentException(string.Format("SetMethodInputValue(string name = {0})failed by name isnot included by  MethodInputNames", name));

        }
        #endregion

        #region 输出项

        public override string[] MethodOutputNames { get { return new string[] { }; } }
        public override Type GetMethodOutputType(string name)
        {
            throw new Exception("GetMethodOutputType failed by: None Output Param");//不提供输出参数
        }
        public override object GetMethodOutputValue(string name)
        {
            throw new Exception("GetMethodOutputValue failed by: None Output Param");//不提供输出参数
        }
        #endregion

        #region 执行

        string _actionErrorInfo = "No-Ops";
        long _actionStartPerformance = 0;//JFFunctions.PerformanceCounter();
        long _actionStopPerformance = 0;
        public override bool Action()
        {
            _actionStartPerformance = JFFunctions.PerformanceCounter();

            _status = 1;
            _eventCmd.Set();
            if (!IsInitOK)
            {
                _actionStopPerformance = JFFunctions.PerformanceCounter();
                _actionErrorInfo = "Action failed by:is not init OK,initError:" + GetInitErrorInfo();
                _status = 0;
                return false;
            }

            try
            {
                if (_isIncludeEnd) //包含结束值
                {
                    if (_interval >= 0) //正向增加
                        for (_forValue = _startValue; _forValue <= _endValue; _forValue += _interval)
                        {
                            InnerFlow.DataPool["For_Value"] = _forValue;
                            if (!InnerFlow.Action())
                            {
                                _status = 0;
                                _actionStopPerformance = JFFunctions.PerformanceCounter();
                                return false;
                            }
                            if (2 == _status)
                            {
                                _eventCmd.WaitOne();
                                if (3 == _status)
                                {
                                    _status = 0;
                                    _actionStopPerformance = JFFunctions.PerformanceCounter();
                                    if (InnerFlow.Stop(10) != JFWorkCmdResult.Success)
                                        InnerFlow.Abort();
                                    throw new JFBreakMethodFlowException();
                                }
                            }
                        }

                    else //负向递减
                    {
                        for (_forValue = _startValue; _forValue >= _endValue; _forValue += _interval)
                        {
                            InnerFlow.DataPool["For_Value"] = _forValue;
                            if (!InnerFlow.Action())
                            {
                                _status = 0;
                                _actionStopPerformance = JFFunctions.PerformanceCounter();
                                return false;
                            }
                            if (2 == _status)
                            {
                                _eventCmd.WaitOne();
                                if (3 == _status)
                                {
                                    _status = 0;
                                    if (InnerFlow.Stop(10) != JFWorkCmdResult.Success)
                                        InnerFlow.Abort();
                                    _actionStopPerformance = JFFunctions.PerformanceCounter();
                                    throw new JFBreakMethodFlowException();
                                }
                            }
                        }
                    }
                }
                else//不包含结束值
                {
                    if (_interval >= 0) //正向增加
                        for (_forValue = _startValue; _forValue < _endValue; _forValue += _interval)
                        {
                            InnerFlow.DataPool["For_Value"] = _forValue;
                            if (!InnerFlow.Action())
                            {
                                _status = 0;
                                _actionStopPerformance = JFFunctions.PerformanceCounter();
                                return false;
                            }
                            if (2 == _status)
                            {
                                _eventCmd.WaitOne();
                                if (3 == _status)
                                {
                                    _status = 0;
                                    if (InnerFlow.Stop(10) != JFWorkCmdResult.Success)
                                        InnerFlow.Abort();
                                    _actionStopPerformance = JFFunctions.PerformanceCounter();
                                    throw new JFBreakMethodFlowException();
                                }
                            }
                        }

                    else //负向递减
                    {
                        for (_forValue = _startValue; _forValue > _endValue; _forValue += _interval)
                        {
                            InnerFlow.DataPool["For_Value"] = _forValue;
                            if (!InnerFlow.Action())
                            {
                                _status = 0;
                                _actionStopPerformance = JFFunctions.PerformanceCounter();
                                return false;
                            }
                            if (2 == _status)
                            {
                                _eventCmd.WaitOne();
                                if (3 == _status)
                                {
                                    _status = 0;
                                    if (InnerFlow.Stop(10) != JFWorkCmdResult.Success)
                                        InnerFlow.Abort();
                                    _actionStopPerformance = JFFunctions.PerformanceCounter();
                                    throw new JFBreakMethodFlowException();
                                }
                            }
                        }
                    }
                }
            }
            catch (JFBreakMethodFlowException) //Break 正常退出循环
            {
                _status = 0;
                _actionStopPerformance = JFFunctions.PerformanceCounter();
                _actionErrorInfo = "Success - break while";
                return true;
            }


            _actionStopPerformance = JFFunctions.PerformanceCounter();
            return true;
        }
        public override string GetActionErrorInfo()
        {
            return _actionErrorInfo;
        }

        public override double GetActionSeconds()
        {
            return (double)(_actionStopPerformance - _actionStartPerformance) / JFFunctions.PerformanceFrequency();
        }
        #endregion


        int _status = 0; //当前状态 0表示未运行 1表示正在运行，2表示暂停,3表示指令退出
        //int _cmd = 0; //外部发来的指令 ， 1表示退出，2表示暂停，3表示恢复
        ManualResetEvent _eventCmd = new ManualResetEvent(false);
        /// <summary>
        /// 异步操作:暂停执行
        /// </summary>
        public override void Pause()
        {
            if (_status != 1)
                return;
            _eventCmd.Reset();
            _status = 2;
            InnerFlow.Pause();
        }

        /// <summary>
        /// 异步操作：恢复执行
        /// </summary>
        public  override void Resume()
        {
            if (_status != 2)
                return;
            _status = 1;
            _eventCmd.Set();
            InnerFlow.Resume();
        }

        /// <summary>
        /// 异步操作:退出
        /// </summary>
        public override void Exit()
        {
            if (0 == _status || 3 == _status)
                return;
            _status = 3;
            if (2 == _status)
                _eventCmd.Set();
            if (InnerFlow.Stop(10) != JFWorkCmdResult.Success)
                InnerFlow.Abort();
        }

        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;
            if (!InnerFlow.DataPool.ContainsKey("For_Value"))
            {
                InnerFlow.DataPool.Add("For_Value", _forValue);
                InnerFlow.TypePool.Add("For_Value", typeof(double));
            }
            else
                InnerFlow.DataPool["For_Value"] = _forValue;
            return true;
        }
    }
}
