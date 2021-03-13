using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using System.CodeDom;

namespace JFMethodCommonLib
{
    /// <summary>
    /// 将工作流（数据池）中的项值写入到工站数据池中
    /// </summary>
    /// /// <summary>
    /// 从工站数据池中读取数据项，放入工作流数据池中
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 系统", "数据池操作" })]
    [JFDisplayName("将工作流数据项写入工站数据池_S")]
    [JFVersion("1.0.0.0")]
    public class JFCM_SetStationPoolValue_S : JFMethodInitParamBase, IJFStationBaseAcq, IJFMethodFlowAcq
    {
        static string PN_LocItemName = "读取工作流数据项名称";
        static string PN_StationItemName = "写入工站数据项名称";
        

        public JFCM_SetStationPoolValue_S()
        {
            //DeclearInitParam(PN_LocItemName, typeof(string), "");
            DeclearInitParam(PN_StationItemName, typeof(string), "");
        }
        protected override bool InitializeGenuine(out string errorInfo)
        {
            bool isOK = true;
            StringBuilder sbErrorInfo = new StringBuilder();
            if(string.IsNullOrEmpty(PN_LocItemName))
            {
                isOK = false;
                sbErrorInfo.AppendLine(PN_LocItemName + "  未设置");
            }

            if (string.IsNullOrEmpty(PN_StationItemName))
            {
                isOK = false;
                sbErrorInfo.AppendLine(PN_StationItemName + "  未设置");
            }

            if (!isOK)
                errorInfo = sbErrorInfo.ToString();
            else
                errorInfo = "Success";
            return isOK;
        }
        protected override bool ActionGenuine(out string errorInfo)
        {
            if (null == _owner)
            {
                errorInfo = "IJFMethodFlowAcq.ActionGenuine() failed by MethodFlow-Owner is unset/null";
                return false;
            }

            if(_station == null)
            {
                errorInfo = "工站未设置!";
                return false;
            }
            string locItemName = GetInitParamValue(PN_LocItemName) as string;
            if(!_owner.DataPool.ContainsKey(locItemName))
            {
                errorInfo = "当前工作流中未包含数据项：" + locItemName;
                return false;
            }

            object itemVal = _owner.DataPool[locItemName];
            Type itemType = _owner.TypePool[locItemName];
            if (itemType == typeof(object) && itemVal != null) //
                itemType = itemVal.GetType();
            string stationItemName = GetInitParamValue(PN_StationItemName) as string;
            if(!_station.DataPool.ContainItem(stationItemName))
            { 
                if(!_station.DataPool.RegistItem(stationItemName, itemType, itemVal))
                {
                    errorInfo = "向工站数据池写入/注册 数据项失败 stationItemName = \"" + stationItemName +"\", Type=" + itemType + ",Value="+ itemVal.ToString();
                    return false;
                }
            }
            else
            {
                if(!_station.DataPool.SetItemValue(stationItemName, itemVal))
                {
                    errorInfo = "向工站数据池写入数据项失败 stationItemName = \"" + stationItemName + "\", Type=" + itemType + ",Value=" + itemVal.ToString();
                    return false;
                }
            }

            errorInfo = "Success";
            return true;
        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }

        JFMethodFlow _owner = null;
        public void SetFlow(JFMethodFlow mf)
        {
            _owner = mf;
        }
    }
}
