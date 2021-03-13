using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;

namespace JFHub
{

    /// <summary>
    /// 退出工作流程异常
    /// </summary>
    public class JFBreakMethodFlowException : Exception
    {
        public JFBreakMethodFlowException() : base() { }
        
    }

    [JFVersion("1.0.0")]
    [JFCategoryLevels(new string[] { "流程控制" })]
    [JFDisplayName("BREAK")]
    public class JFMethodBreak:IJFMethod
    {
        #region 输入项
        public string[] MethodInputNames { get { return new string[] { }; } }
        public Type GetMethodInputType(string name) { throw new Exception("GetMethodInputType failed by: Nonexist input param"); }
        public object GetMethodInputValue(string name) { throw new Exception("GetMethodInputValue failed by: Nonexist input param"); }
        public void SetMethodInputValue(string name, object value) { throw new Exception("SetMethodInputValue failed by: Nonexist input param"); }
        #endregion

        #region 输出项
        public string[] MethodOutputNames { get { return new string[] { }; } }
        public Type GetMethodOutputType(string name) { throw new Exception("GetMethodOutputType failed by: Nonexist output param"); }
        public object GetMethodOutputValue(string name) { throw new Exception("GetMethodOutputValue failed by: Nonexist output param"); }
        #endregion

        #region 执行
        public bool Action() { throw new JFBreakMethodFlowException(); }
        public string GetActionErrorInfo() { return "Success"; }

        public double GetActionSeconds() { return 0; }
        #endregion
    }
}
