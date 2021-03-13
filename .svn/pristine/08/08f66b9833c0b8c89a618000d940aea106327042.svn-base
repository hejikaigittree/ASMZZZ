using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;
namespace JFMethodCommonLib
{
    [JFCategoryLevels(new string[] { "JF 系统", "数据池操作" })]
    [JFDisplayName("将工站配置参数写入数据池_D")]
    [JFVersion("1.0.0.0")]
    class JFCM_GetStationCfgToDataPool_D : JFMethodInitParamBase, IJFStationBaseAcq, IJFMethodFlowAcq
    {
        static string S_StaCfgName = "读取工站配置数据项名称";
        static string G_CfgToPoolName = "工站配置参数";
        public JFCM_GetStationCfgToDataPool_D()
        {        
            DeclearOutput(G_CfgToPoolName, typeof(object), null);
            DeclearInput(S_StaCfgName, typeof(object), "");
        }
        protected override bool ActionGenuine(out string errorInfo)
        {
            if (_station == null)
            {
                errorInfo = "工站未设置!";
                return false;
            }
            if (_jf == null)
            {
                errorInfo = "动作流未设置!";
                return false;
            }
            string cfgName = GetMethodInputValue(S_StaCfgName) as string;
            if (cfgName == null)
            {
                errorInfo = "要获取参数名称未设置!";
                return false;
            }
            object ob = _station.GetCfgParamValue(cfgName);
            if (ob == null)
            {
                errorInfo = "要获取参数值为空!";
                return false;
            }

            SetOutputParamValue(G_CfgToPoolName, ob);

            errorInfo = "Success";
            return true;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            bool isOk = true;
            StringBuilder sbErrorInfo = new StringBuilder();
            string Cfg_Name = GetMethodInputValue(S_StaCfgName) as string;
            //if (string.IsNullOrEmpty(Cfg_Name))
            //{
            //    isOk = false;
            //    sbErrorInfo.AppendLine(S_StaCfgName + "  未设置");
            //}
            if (!isOk)
                errorInfo = sbErrorInfo.ToString();
            else
                errorInfo = "Success";

            return isOk;
        }
        JFMethodFlow _jf = null;
        public void SetFlow(JFMethodFlow mf)
        {
            _jf = mf;
        }
        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }
    }
}
