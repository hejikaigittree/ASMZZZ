using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using System.Threading;
namespace JFMethodCommonLib
{
    /// <summary>
    /// 延迟固定的毫秒,继承JFMethodInitParamBase的一个Demo
    /// </summary>
    [JFVersion("1.0.0")]
    [JFCategoryLevels(new string[] { "流程控制" })]
    [JFDisplayName("延时毫秒_S")]
    public class JFCM_Delay_S : JFMethodInitParamBase
    {
        public JFCM_Delay_S()
        {
            DeclearInitParam("延时毫秒", typeof(int), 0);
        }
        protected override bool ActionGenuine(out string errorInfo)
        {
            if(!IsInitOK)
            {
                errorInfo = "初始化未完成:" + InitErrorInfo;
                return false;
            }
            Thread.Sleep((int)GetInitParamValue("延时毫秒"));
            errorInfo = "Success";
            return true;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            errorInfo = "Success";
            return true;
        }
    }
}
