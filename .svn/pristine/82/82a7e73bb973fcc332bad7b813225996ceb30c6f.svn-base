using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;
using JFToolKits;

namespace JFMethodCommonLib
{
    [JFCategoryLevels(new string[] { "JF 系统", "数据池操作" })]
    [JFDisplayName("将外部数据池写入内部数据池_S")]
    [JFVersion("1.0.0.0")]
    class JFCM_GetSOutPoolToInnerDataPool_S : JFMethodInitParamBase, IJFMethodFlowAcq
    {
        static string S_StaCfgName = "外部数据项名称";
        static string G_CfgToPoolName = "外部数据";
        public JFCM_GetSOutPoolToInnerDataPool_S()
        {
            //string[] valueNameList = _jf.DataPool.Keys.ToArray();
            //DeclearInitParam(JFParamDescribe.Create(S_StaCfgName, typeof(string), JFValueLimit.Options, valueNameList), "");
           DeclearInitParam(S_StaCfgName, typeof(string), "");
            DeclearOutput(G_CfgToPoolName, typeof(object), null);
        }
        protected override bool ActionGenuine(out string errorInfo)
        {
            string valueName = GetInitParamValue(S_StaCfgName) as string;

            if(string.IsNullOrEmpty(valueName))
            {
                errorInfo = "未设定初始化参数名称";
                return false;
            }
            object ob = _jf.OutterDataPool[valueName];
            SetOutputParamValue(G_CfgToPoolName, ob);
                   
            errorInfo = "Success";
            return true;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
               bool isOk = true;
          
                errorInfo = "Success";

            return isOk;
        }
        JFMethodFlow _jf = null;
        public void SetFlow(JFMethodFlow mf)
        {
            _jf = mf;
        }
       
    }
}
