using JFHub;
using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JFMethodCommonLib
{
    /// <summary>
    /// 等待一个系统数据池中的bool项值
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 系统", "数据池操作" })]
    [JFDisplayName("等待系统Bool值_S")]
    [JFVersion("1.0.0.0")]
    public class JFSM_WaitSysPoolBool_S : JFMethodInitParamBase,IJFMethod_T
    {
        //Init Param's Name 
        static string PN_SysPoolItemName = "系统Bool变量名称"; 
        static string PN_TargetVal = "目标值";
        static string PN_TimeoutMilliSeconds = "超时毫秒";
        static string PN_CycleMilliSeconds = "轮循间隔毫秒";
        //Output Param's Name 
        static string ON_Result = "执行结果";
        //static string ON_ErrorInfo = "错误信息";
        public JFSM_WaitSysPoolBool_S()
        {
            DeclearInitParam(PN_SysPoolItemName, typeof(string), "");
            DeclearInitParam(PN_TargetVal, typeof(bool), true);
            DeclearInitParam(JFParamDescribe.Create(PN_CycleMilliSeconds, typeof(int), JFValueLimit.MinLimit | JFValueLimit.MaxLimit, new object[] { 10, 1000 }),50);
            DeclearInitParam(PN_TimeoutMilliSeconds, typeof(int), -1);
            DeclearOutput(ON_Result, typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
            //DeclearOutput(ON_ErrorInfo, typeof(string), "No-Option");
        }

        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if(name == PN_SysPoolItemName)
                return JFParamDescribe.Create(name, typeof(string), JFValueLimit.Options, JFHubCenter.Instance.DataPool.AllItemKeys);
            
            return base.GetInitParamDescribe(name);
        }



        protected override bool InitializeGenuine(out string errorInfo)
        {
            string itemName = GetInitParamValue(PN_SysPoolItemName) as string;
            if(string.IsNullOrEmpty(itemName))
            {
                errorInfo = "系统Bool变量名称 未设置";
                return false;
            }
            errorInfo = "Success";
            return true;
        }


        int _workCmd = 0; //-1：指令退出 0:正常运行，1:暂停  
        bool _isRunning = false;


        protected override bool ActionGenuine(out string errorInfo)
        {
            _isRunning = true;
            string itemName;
            bool targetVal = Convert.ToBoolean(GetInitParamValue(PN_TargetVal));
            int cycleMilliSeconds = Convert.ToInt32(GetInitParamValue(PN_CycleMilliSeconds));
            int timeoutMilliSeconds = Convert.ToInt32(GetInitParamValue(PN_TimeoutMilliSeconds));

            itemName = GetInitParamValue(PN_SysPoolItemName) as string;
            if (!JFHubCenter.Instance.DataPool.ContainItem(itemName))
            {
                errorInfo = "系统数据池不包含项名:" + itemName;
                SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                //SetOutputParamValue(ON_ErrorInfo, errorInfo)
                _workCmd = 0;
                _isRunning = false;
                return false;
            }

            if (JFHubCenter.Instance.DataPool.GetItemType(itemName) != typeof(bool))
            {
                errorInfo = "系统数据项类型不是Bool：" + JFHubCenter.Instance.DataPool.GetItemType(itemName).Name;
                SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                _isRunning = false;
                _workCmd = 0;
                return false;
            }

            
            DateTime startTime = DateTime.Now;
            while(true)
            {
                
                if(0 == _workCmd) //正常工作
                {
                    object currVal;
                    JFHubCenter.Instance.DataPool.GetItemValue(itemName, out currVal);
                    if(Convert.ToBoolean(currVal) == targetVal)
                    {
                        errorInfo = "Success";
                        _workCmd = 0;
                        _isRunning = false;
                        SetOutputParamValue(ON_Result, JFWorkCmdResult.Success);
                        return true;
                    }

                    if(timeoutMilliSeconds >=0)
                    {
                        TimeSpan ts = DateTime.Now - startTime;
                        if(ts.TotalMilliseconds >= timeoutMilliSeconds)
                        {
                            errorInfo = "超时未等到数据项:\" " + itemName + "\" 目标值:" + targetVal;
                            _workCmd = 0;
                            _isRunning = false;
                            SetOutputParamValue(ON_Result, JFWorkCmdResult.Timeout);
                            return true;
                        }
                    }

                    Thread.Sleep(cycleMilliSeconds);
                }
                else if(1 == _workCmd)//当前为暂停状态
                {
                    Thread.Sleep(cycleMilliSeconds);
                    continue;
                }
                else //指令退出
                {
                    errorInfo = "收到退出指令";
                    _workCmd = 0;
                    _isRunning = false;
                    SetOutputParamValue(ON_Result, JFWorkCmdResult.ActionError);
                    return false;
                    
                }
            }

        }

        public void Pause()
        {
            if (!_isRunning)
                return;
            _workCmd = 1;
        }

        public void Resume()
        {
            if (!_isRunning)
                return;
            _workCmd = 0;
        }

        public void Exit()
        {
            if (!_isRunning)
                return;
            _workCmd = -1;
        }
    }



}
