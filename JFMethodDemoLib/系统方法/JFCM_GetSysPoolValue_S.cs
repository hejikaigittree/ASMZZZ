using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;

namespace JFMethodCommonLib
{
    /// <summary>
    /// 从系统数据池中获取指定的单项值（并放入工作流数据池中）
    /// 
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 系统", "数据池操作" })]
    [JFDisplayName("读取系统数据项到工作流_S")]
    [JFVersion("1.0.0.0")]
    public class JFCM_GetSysPoolValue_S : JFMethodInitParamBase
    {
        static string PN_SysPoolItemName = "系统变量名称";
        /// Output 's name
        static string ON_CurrVal = "当前值"; //需要向工作流中输出的值
        public JFCM_GetSysPoolValue_S()
        {
            DeclearInitParam(PN_SysPoolItemName, typeof(string), "");
            DeclearOutput(ON_CurrVal, typeof(object), null);
        }

        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == PN_SysPoolItemName)
                return JFParamDescribe.Create(PN_SysPoolItemName, typeof(string), JFValueLimit.Options, JFHubCenter.Instance.DataPool.AllItemKeys);
            
            return base.GetInitParamDescribe(name);
        }


        protected override bool ActionGenuine(out string errorInfo)
        {
            string itemName = GetInitParamValue(PN_SysPoolItemName) as string;
            IJFDataPool sysDataPool = JFHubCenter.Instance.DataPool;
            if(!sysDataPool.ContainItem(itemName))
            {
                errorInfo = "系统数据池不包含数据项名：" + itemName;
                return false;
            }

            object val;
            if(!sysDataPool.GetItemValue(itemName,out val))
            {
                errorInfo = "获取系统数据项失败：名称：\"" + itemName + "\"";
                return false;
            }

            SetOutputParamValue(ON_CurrVal, val);
            errorInfo = "Success";
            return true;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string itemName = GetInitParamValue(PN_SysPoolItemName) as string;
            if(string.IsNullOrEmpty(itemName))
            {
                errorInfo = "初始化参数项:\"" + PN_SysPoolItemName + "\"未设置";
                return false;
            }

            errorInfo = "Success";
            return true;
        }
    }
}
