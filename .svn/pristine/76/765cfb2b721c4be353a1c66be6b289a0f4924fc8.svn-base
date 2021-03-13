using JFHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFMethodCommonLib.数学运算
{
    public class JFAddExpend_Demo : JFMethodBase
    {
        public JFAddExpend_Demo()
        {
            DeclearInput("Num1", typeof(double), 0f);
            DeclearInput("Num2", typeof(double), 0f);
            DeclearOutput("Result", typeof(double), 0f);
        }
        protected override bool ActionGenuine(out string errorInfo)
        {
            double num1 = Convert.ToDouble(GetMethodInputValue("Num1"));
            double num2 = Convert.ToDouble(GetMethodInputValue("Num2"));


            SetOutputParamValue("Result", num1 + num2);
            errorInfo = "Success";
            return true;
        }
    }
}
