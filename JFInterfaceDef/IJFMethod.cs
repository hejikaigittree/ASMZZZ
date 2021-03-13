using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFInterfaceDef
{
    [JFDisplayName("JF方法")]
    public interface IJFMethod
    {
        #region 输入项
        string[] MethodInputNames { get; }
        Type GetMethodInputType(string name);
        object GetMethodInputValue(string name);
        void SetMethodInputValue(string name, object value);
        #endregion

        #region 输出项
        string[] MethodOutputNames { get; }
        Type GetMethodOutputType(string name);
        object GetMethodOutputValue(string name);
        #endregion

        #region 执行
        bool Action();
        string GetActionErrorInfo();

        double GetActionSeconds();
        #endregion

    }
}
