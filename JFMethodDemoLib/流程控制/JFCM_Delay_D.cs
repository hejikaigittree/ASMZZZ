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
    /// 延迟固定的毫秒
    /// </summary>
    [JFVersion("1.0.0")]
    [JFCategoryLevels(new string[] { "流程控制" })]
    [JFDisplayName("延时毫秒_D")]
    public class JFCM_Delay_D  : JFMethodBase
    {
        public JFCM_Delay_D()
        {
            DeclearInput("延时毫秒", typeof(int), 0);
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            Thread.Sleep((int)GetMethodInputValue("延时毫秒"));
            errorInfo = "Success";
            return true;
        }
    }
}
