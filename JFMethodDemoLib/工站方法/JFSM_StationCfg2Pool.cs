using JFHub;
using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFMethodCommonLib
{
    /// <summary>
    /// 从工站中的配置数据中加载一个数据项到工站数据池中
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 工站方法", "系统数据池" })]
    [JFDisplayName("导入工站配置项到工站数据池")]
    [JFVersion("1.0.0.0")]
    public class JFSM_StationCfg2Pool : JFMethodInitParamBase,IJFStationBaseAcq
    {
        static string PN_CfgItemName = "工站配置项名称";
        static string PN_PoolItemName = "写入工站数据池名称";
        public JFSM_StationCfg2Pool()
        {
            DeclearInitParam(PN_CfgItemName, typeof(string), "");
            DeclearInitParam(PN_PoolItemName, typeof(string), "");
        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }

        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if(name == PN_CfgItemName)
            {
                if (null == _station)
                    return JFParamDescribe.Create(PN_CfgItemName, typeof(string), JFValueLimit.Options, new string[] { });
                else
                    return JFParamDescribe.Create(PN_CfgItemName, typeof(string), JFValueLimit.Options, _station.Config.AllItemNames());
            }
            return base.GetInitParamDescribe(name);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string cfgItemName = GetInitParamValue(PN_CfgItemName) as string;
            string poolItemName = GetInitParamValue(PN_PoolItemName) as string;
            StringBuilder sbError = new StringBuilder();
            bool isOK = true;
            if(string.IsNullOrEmpty(cfgItemName))
            {
                sbError.AppendLine(PN_CfgItemName + " 未设置");
                isOK = false;
            }

            if(string.IsNullOrEmpty(poolItemName))
            {
                sbError.AppendLine(PN_PoolItemName + " 未设置");
                isOK = false;
            }

            if (isOK)
                errorInfo = "Success";
            else
                errorInfo = sbError.ToString();
            return isOK;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            if(_station == null)
            {
                errorInfo = "工站未设置/空值";
                return false;
            }
            string cfgItemName = GetInitParamValue(PN_CfgItemName) as string;
            string poolItemName = GetInitParamValue(PN_PoolItemName) as string;
            if(!_station.Config.ContainsItem(cfgItemName))
            {
                errorInfo = "工站配置中为包含配置项:" + cfgItemName;
                return false;
            }

            object itemVal = _station.Config.GetItemValue(cfgItemName);
            Type itemType = typeof(object);
            if (null != itemVal)
                itemType = itemVal.GetType();

            if(!_station.DataPool.ContainItem(poolItemName))
            {
                if(!_station.DataPool.RegistItem(poolItemName, itemType, itemVal))
                {
                    errorInfo = "向工站数据池中写入/注册 变量失败，PoolItemName = \"" + poolItemName + "\", Type = " + itemType.ToString() + ", Value = " + itemVal.ToString();
                    return false;
                }
            }
            else
            {
                if(!_station.DataPool.SetItemValue(poolItemName, itemVal))
                {
                    errorInfo = "向工站数据池中写入 变量失败，PoolItemName = \"" + poolItemName + "\", Type = " + itemType.ToString() + ", Value = " + itemVal.ToString();
                    return false;
                }
            }

            errorInfo = "Success";
            return true;
        }


    }
}
