using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;

namespace JFMethodDemoLib
{
    /// <summary>
    /// 加法算子示例,输入为两个double数值，输出为一个double和
    /// </summary>
    [JFVersion("1.0.0")]
    [JFCategoryLevels(new string[] { "数学运算" })]
    [JFDisplayName("除法算子")]
    public class JFMethod_Div : IJFMethod
    {
        JFMethod_Div()
        {

        }
        string[] IJFMethod.MethodInputNames { get { return new string[] { "num1", "num2" }; } }

        string[] IJFMethod.MethodOutputNames { get { return new string[] { "result" }; } }

        double num1 = 0, num2 = 0;
        double result = 0;
        bool IJFMethod.Action()
        {
            if(num2 == 0)
            {
                return false;
            }
            result = num1 / num2;
            return true;
        }

        string IJFMethod.GetActionErrorInfo()
        {
            ///如果Action失败，这里需要返回一条错误描述字符串 ， 此处省略 ... 
            if(num2 == 0)
            {
                return "Num2 小于零不合法";
            }
            return "Sucess";
        }

        double IJFMethod.GetActionSeconds()
        {
            ///返回方法执行的耗时，单位: 秒 ， 此处省略 ... 
            return 0;
        }

        Type IJFMethod.GetMethodInputType(string name)
        {
            ///需要检查参数名称的合法性，这里省略...
            return typeof(double);
        }

        object IJFMethod.GetMethodInputValue(string name)
        {
            if ("num1" == name)
                return num1;
            else if ("num2" == name)
                return num2;
            throw new Exception("name isnot legal input-name");
        }

        Type IJFMethod.GetMethodOutputType(string name)
        {
            ///需要检查参数名称的合法性，这里省略...
            return typeof(double);
        }

        object IJFMethod.GetMethodOutputValue(string name)
        {
            ///需要检查参数名称的合法性，这里省略...
            return result;
        }

        void IJFMethod.SetMethodInputValue(string name, object value)
        {
            ///需要检查参数名称的合法性，这里省略...
            if ("num1" == name)
                num1 = (double)value;
            else
                num2 = (double)value;
        }
    }



}
