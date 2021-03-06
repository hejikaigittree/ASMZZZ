using JFHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;

namespace JFMethodCommonLib
{
    [JFCategoryLevels(new string[] { "JF 系统", "数据池操作" })]
    [JFDisplayName("设置系统Bool值_S")]
    [JFVersion("1.0.0.0")]
    public class JFCM_SetSysPoolBool_S : JFMethodInitParamBase
    {
        static string PN_SysPoolItemName = "系统Bool变量名称";
        static string PN_TargetVal = "写入值";
        public JFCM_SetSysPoolBool_S()
        {
            DeclearInitParam(PN_SysPoolItemName, typeof(string), "");
            DeclearInitParam(PN_TargetVal, typeof(bool), false);
        }

        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == PN_SysPoolItemName)
            {
                IJFDataPool sysDataPool = JFHubCenter.Instance.DataPool;
                List<string> boolItemNames = new List<string>();
                string[] allItemNames = sysDataPool.AllItemKeys;
                if (null != allItemNames)
                    foreach (string itName in allItemNames)
                        if (sysDataPool.GetItemType(itName) == typeof(bool))
                            boolItemNames.Add(itName);
                return JFParamDescribe.Create(PN_SysPoolItemName, typeof(string), JFValueLimit.Options, boolItemNames.ToArray());
            }
            return base.GetInitParamDescribe(name);
        }
        protected override bool ActionGenuine(out string errorInfo)
        {
            string itemName = GetInitParamValue(PN_SysPoolItemName) as string;
            if (!JFHubCenter.Instance.DataPool.ContainItem(itemName))
            {
                errorInfo = "系统数据池中未包含数据项:" + itemName;
                return false;
            }

            if(JFHubCenter.Instance.DataPool.GetItemType(itemName) != typeof(bool))
            {
                errorInfo = "系统数据池中数据项:" + itemName + "值类型非Bool ,RealType = "+ JFHubCenter.Instance.DataPool.GetItemType(itemName).Name;
                return false;
            }

            bool targetVal = Convert.ToBoolean(GetInitParamValue(PN_TargetVal));
            bool isOK= JFHubCenter.Instance.DataPool.SetItemValue(itemName, targetVal);
            if (!isOK)
            {
                errorInfo = "设置系统数据项：\"" + itemName + "\"值：" + targetVal + " 失败！";
                return false;
            }
            errorInfo = "Success";
            return true;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string itemName = GetInitParamValue(PN_SysPoolItemName) as string;
            if(string.IsNullOrEmpty(itemName))
            {
                errorInfo = "初始化参数:\"" + PN_SysPoolItemName + "\"未设置值";
                return false;
            }

            errorInfo = "Success";
            return true;
        }
    }
}
