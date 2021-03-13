using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;

namespace JFMethodCommonLib
{
    /// <summary>
    /// 工站对象注册一个系统数据池bool变量替身
    /// 主要供JFRuleStation配置动作流时使用
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 工站方法", "系统数据池" })]
    [JFDisplayName("等待系统Bool值(替身)")]
    [JFVersion("1.0.0.0")]
    public class JFSM_WaitSysPoolBoolAlias:JFMethodInitParamBase,IJFStationBaseAcq
    {
        static string PN_AliasName = "Bool项替身名称";
        static string PN_TargetVal = "目标值";
        static string PN_TimeoutMilSec = "超时时间(毫秒)";
        static string ON_Result = "Result";
        public JFSM_WaitSysPoolBoolAlias()
        {
            DeclearInitParam(PN_AliasName, typeof(string), "");
            DeclearInitParam(PN_TargetVal, typeof(bool), true);
            DeclearInitParam(PN_TimeoutMilSec, typeof(int), -1);
            DeclearOutput(ON_Result, typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }

        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if(name == PN_AliasName && _station != null)
            {
                List<string> optionItems = new List<string>();
                string[] allAliasNames = _station.AllSPAliasNames;
                foreach (string an in allAliasNames)
                    if (_station.GetSPAliasType(an) == typeof(bool))
                        optionItems.Add(an);

                return JFParamDescribe.Create(PN_AliasName, typeof(string), JFValueLimit.Options, optionItems.ToArray());

            }
            return base.GetInitParamDescribe(name);
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            if(_station == null)
            {
                errorInfo = "工站未设置";
                return false;
            }

            string aliasName = GetInitParamValue(PN_AliasName) as string;
            bool tgtVal = Convert.ToBoolean(GetInitParamValue(PN_TargetVal));
            int timeoutMilSec = Convert.ToInt32(GetInitParamValue(PN_TimeoutMilSec));

            JFWorkCmdResult ret = _station.WaitSPBoolAliasInMethodFlow(aliasName, tgtVal, out errorInfo, timeoutMilSec);
            SetOutputParamValue(ON_Result, ret);
            if (ret == JFWorkCmdResult.Success || ret == JFWorkCmdResult.Timeout)
                return true;
            return false;

        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string aliasName = GetInitParamValue(PN_AliasName) as string;
            if(string.IsNullOrEmpty(aliasName))
            {
                errorInfo = PN_AliasName + " 未设置";
                return false;
            }

            errorInfo = "Success";
            return true;
        }
    }
}
