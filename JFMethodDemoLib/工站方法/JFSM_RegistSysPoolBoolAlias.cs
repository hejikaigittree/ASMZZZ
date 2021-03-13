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
    [JFDisplayName("注册系统Bool替身")]
    [JFVersion("1.0.0.0")]
    public class JFSM_RegistSysPoolBoolAlias:JFMethodInitParamBase,IJFStationBaseAcq
    {
        static string PN_AliasName = "替身名称";
        static string PN_InitValue = "初始值";
        public JFSM_RegistSysPoolBoolAlias()
        {
            DeclearInitParam(PN_AliasName, typeof(string), "");
            DeclearInitParam(PN_InitValue, typeof(bool), false);
        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            if(_station == null)
            {
                errorInfo = "工站未设置";
                return false;
            }
            string aliasName = GetInitParamValue(PN_AliasName) as string;
            bool initVal = Convert.ToBoolean(GetInitParamValue(PN_InitValue));
            return _station.DeclearSPItemAliasInMethodFlow(aliasName, typeof(bool), initVal, out errorInfo);

        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string aliasName = GetInitParamValue(PN_AliasName) as string;
            if(string.IsNullOrEmpty(aliasName))
            {
                errorInfo = PN_AliasName +  " 未设置";
                return false;
            }
            errorInfo = "Success";
            return true;
        }
    }
}
