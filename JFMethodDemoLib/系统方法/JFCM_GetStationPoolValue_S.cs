using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;

namespace JFMethodCommonLib
{
    /// <summary>
    /// 从工站数据池中读取数据项，放入工作流数据池中
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 系统", "数据池操作" })]
    [JFDisplayName("读取工站数据项到工作流_S")]
    [JFVersion("1.0.0.0")]
    public class JFCM_GetStationPoolValue_S : JFMethodInitParamBase, IJFStationBaseAcq
    {
        static string PN_StationItemName = "工站数据项名称";
        static string ON_CurrVal = "当前值";
        public JFCM_GetStationPoolValue_S()
        {
            DeclearInitParam(PN_StationItemName, typeof(string), "");
            DeclearOutput(ON_CurrVal, typeof(object), null);
        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }



        protected override bool InitializeGenuine(out string errorInfo)
        {
            string stationItemName = GetInitParamValue(PN_StationItemName) as string;
            if(string.IsNullOrEmpty(stationItemName))
            {
                errorInfo = PN_StationItemName + " 未设置";
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            if(_station == null)
            {
                errorInfo = "工站未设置";
                return false;
            }
            string stationItemName = GetInitParamValue(PN_StationItemName) as string;
            if(!_station.DataPool.ContainItem(stationItemName))
            {
                errorInfo = "工站:\"" + _station.Name + "\"数据池未包含数据项名称:\"" + stationItemName + "\"";
                return false;
            }
            object stationItemVal = null;
            bool isOK = _station.DataPool.GetItemValue(stationItemName,out stationItemVal);
            if(!isOK)
            {
                errorInfo = "未能获取 工站:\"" + _station.Name + "\"数据项名称:\"" + stationItemName + "\"";
                return false;
            }
            Type stationItemType = _station.DataPool.GetItemType(stationItemName);

            SetOutputParamValue(ON_CurrVal, stationItemVal);
            errorInfo = "Success";
            return true;

        }


    }
}
