using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;

namespace DLAFMethodLib
{
    /// <summary>
    /// 加法算子示例,输入为两个double数值，输出为一个double和
    /// </summary>
    [JFVersion("1.0.0")]
    [JFCategoryLevels(new string[] { "JF 自定义", "比较操作" })]
    [JFDisplayName("比较大小")]
    public class JFM_Compare : JFMethodInitParamBase
    {
        JFM_Compare()
        {
            DeclearInput("参数1：",typeof(double),0);
            DeclearInput("参数2：", typeof(double), 0);

            DeclearOutput("result.", typeof(bool), false);
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            double data1 = (double)GetMethodInputValue("参数1：");
            double data2 = (double)GetMethodInputValue("参数2：");
            bool result;
            if (data1 <= data2)
                result = true;
            else
                result = false;

                SetOutputParamValue("result.", result);
            errorInfo = "success";
            return true;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            bool isOk = true;           
            errorInfo = "success";
            return isOk;
        }
    }
}
