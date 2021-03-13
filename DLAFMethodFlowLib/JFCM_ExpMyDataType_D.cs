using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;

namespace DLAFMethodLib
{
    [JFCategoryLevels(new string[] { "JF 自定义", "数据解析操作" })]
    [JFDisplayName("解析自定义DataType")]
    [JFVersion("1.0.0.0")]
    class JFCM_ExpMyDataType_D : JFMethodInitParamBase, IJFMethodFlowAcq
    {
        static string InPutCfg = "解析数据对象";

        public JFCM_ExpMyDataType_D()
        {
            DeclearInput(InPutCfg,typeof(object),"");
           
        }
        JFMethodFlow _jf = null;
        public void SetFlow(JFMethodFlow mf)
        {
            _jf = mf;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            if(_jf == null)
            {
                errorInfo = "动作流对象为空";
                return false;
            }
            object ob = GetMethodInputValue(InPutCfg);
            if(ob == null)
            {
                errorInfo = "未绑定输入对象或对象为空";
                return false;
            }

           // LoadParam LoadSta = (LoadParam)ob;
            if(!ob.GetType().IsAssignableFrom(typeof(LoadParam)) && !ob.GetType().IsAssignableFrom(typeof(TrackParm)) 
                && !ob.GetType().IsAssignableFrom(typeof(UnloadParam)))
            {
                errorInfo = "绑定的数据项与已知的数据对象不匹配";
                return false;
            }
            foreach(System.Reflection.PropertyInfo info in ob.GetType().GetProperties())
            {
               AttributeCollection attributes = TypeDescriptor.GetProperties(ob)[info.Name].Attributes;
               DescriptionAttribute myAttribute = (DescriptionAttribute)attributes[typeof(DescriptionAttribute)];
                string name = myAttribute.Description;
                if (_jf.DataPool.ContainsKey(name))
                _jf.DataPool.Remove(name);
                if (_jf.TypePool.ContainsKey(name))
                    _jf.TypePool.Remove(name);              
                {
                    object oob = info.GetValue(ob);
                   
                   _jf.DataPool.Add(name, oob);
                   _jf.TypePool.Add(name, oob.GetType());
                }                           
            }
            errorInfo = "Success";
            return true;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            bool isOk = true;
            StringBuilder sbErrorInfo = new StringBuilder();
            object ob = GetMethodInputValue(InPutCfg);

            if (ob == null)
            {
                isOk = false;
                sbErrorInfo.AppendLine(InPutCfg + "  未设置");
            }
            if (!isOk)
                errorInfo = sbErrorInfo.ToString();
            else
                errorInfo = "Success";

            return isOk;
        }
    }
}
