using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;

namespace JFMethodDemoLib
{
    [JFVersion("1.0.0")]
    [JFCategoryLevels(new string[] { "流程控制", "变量声明" })]
    [JFDisplayName("Int")]
    public class JFCM_DeclearInt : IJFMethod, IJFInitializable
    {
        JFCM_DeclearInt()
        {
            IsInitOK = false;
        }
        public string[] MethodInputNames { get { return null; } }

        public string[] MethodOutputNames { get { return new string[] { "value" }; } }

        public string[] InitParamNames { get { return new string[] { "Int" }; } }
        int _srcValue = 0;

        public bool IsInitOK { get; private set; }

        public bool Action()
        {
            return true;
        }

        public void Dispose()
        {
            return;
        }

        public string GetActionErrorInfo()
        {
            return "Success";
        }

        public double GetActionSeconds()
        {
            return 0;
        }

        public string GetInitErrorInfo()
        {
            return "";
        }

        public JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == "Int")
                return JFParamDescribe.Create(name, typeof(int), JFValueLimit.NonLimit, null);
            throw new Exception();
        }

        public object GetInitParamValue(string name)
        {
            if (name == "Int")
                return _srcValue;
            throw new Exception();
        }

        public Type GetMethodInputType(string name)
        {
            return null;
        }

        public object GetMethodInputValue(string name)
        {
            return null;
        }

        public Type GetMethodOutputType(string name)
        {
            return typeof(int);
        }

        public object GetMethodOutputValue(string name)
        {
            return _srcValue;
        }

        public bool Initialize()
        {
            IsInitOK = true;
            return true;
        }

        public bool SetInitParamValue(string name, object value)
        {
            if (name == "Int")
            {
                _srcValue = (int)value;
                return true;
            }
            throw new Exception();
        }

        public void SetMethodInputValue(string name, object value)
        {
            return;
        }
    }

}
