using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;


namespace JFMethodCommonLib
{
    [JFCategoryLevels(new string[] { "JF 系统", "数据池操作" })]
    [JFDisplayName("注册bool型系统变量")]
    [JFVersion("1.0.0.0")]
    public class JFCM_RegistSysPoolBool:JFMethodInitParamBase
    {
        static string PN_ItemName = "数据项名称";
        static string PN_InitValue = "初始值";
        public JFCM_RegistSysPoolBool()
        {
            DeclearInitParam(PN_ItemName, typeof(string), "");
            DeclearInitParam(PN_InitValue, typeof(bool), false);
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            string itemName = GetInitParamValue(PN_ItemName) as string;
            bool initVal = Convert.ToBoolean(GetInitParamValue(PN_InitValue));
            IJFDataPool sysPool = JFHubCenter.Instance.DataPool;
            if(sysPool.ContainItem(PN_ItemName) )
            {
                if(sysPool.GetItemType(itemName) == typeof(bool))
                {
                    errorInfo = "Sucess";
                    return true;
                }
                else
                {
                    errorInfo = "系统数据池已存在数据项:\"" + itemName + "\",类型:" + sysPool.GetItemType(itemName).Name;
                    return false;
                }
            }
            if(!sysPool.RegistItem(PN_ItemName,typeof(bool), initVal))
            {
                errorInfo = "注册bool数据项失败！";
                return false;
            }

            errorInfo = "Success";
            return true;


        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string itemName = GetInitParamValue(PN_ItemName) as string;
            if(string.IsNullOrEmpty(itemName))
            {
                errorInfo = PN_ItemName + "未设置";
                return false;
            }
            errorInfo = "Success";
            return true;
        }
    }
}
