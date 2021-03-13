using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub; 

namespace JFMethodCommonLib
{
    /// <summary>
    /// 将工站配置项数据载入到工作流（的数据池）中
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 工站方法", "配置数据" })]
    [JFDisplayName("导入工站配置项到数据流")]
    [JFVersion("1.0.0.0")]
    public class JFSM_StationCfg2Flow : JFMethodInitParamBase, IJFStationBaseAcq
    {
        static string PN_StationCfgItemName = "工站配置项名称";
        static string ON_StationCfgItemValue = "当前值";
        public JFSM_StationCfg2Flow()
        {
            DeclearInitParam(PN_StationCfgItemName, typeof(string), "");
            DeclearOutput(ON_StationCfgItemValue, typeof(object), null);
        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }

        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if(name == PN_StationCfgItemName)
            {
                if (null == _station)
                    return JFParamDescribe.Create(PN_StationCfgItemName, typeof(string), JFValueLimit.Options, new string[] { });
                return JFParamDescribe.Create(PN_StationCfgItemName, typeof(string), JFValueLimit.Options, _station.Config.AllItemNames());
            }
            return base.GetInitParamDescribe(name);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string cfgItemName = GetInitParamValue(PN_StationCfgItemName) as string;
            if(string.IsNullOrEmpty(cfgItemName))
            {
                errorInfo = PN_StationCfgItemName + " 未设置";
                return false;
            }
            errorInfo = "Success";
            return true;
        }



        protected override bool ActionGenuine(out string errorInfo)
        {
            if(_station == null)
            {
                errorInfo = "工站未设置/空值";
                return false;
            }

            string cfgItemName = GetInitParamValue(PN_StationCfgItemName) as string;
            if(!_station.Config.ContainsItem(cfgItemName))
            {
                errorInfo = "工站配置未包含项名称:" + cfgItemName;
                return false;
            }
            object itemVal = _station.Config.GetItemValue(cfgItemName);

            SetOutputParamValue(ON_StationCfgItemValue, itemVal);
            errorInfo = "Success";
            return true;
        }

  
    }
}
