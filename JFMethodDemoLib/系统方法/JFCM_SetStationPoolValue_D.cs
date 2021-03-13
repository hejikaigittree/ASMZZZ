using JFHub;
using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFMethodCommonLib.系统方法
{
    /// <summary>
    /// 将工作流（数据池）中的项值写入到工站数据池中
    /// </summary>
    /// /// <summary>
    /// 从工站数据池中读取数据项，放入工作流数据池中
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 系统", "数据池操作" })]
    [JFDisplayName("将工作流数据项写入工站数据池_D")]
    [JFVersion("1.0.0.0")]
    public class JFCM_SetStationPoolValue_D : JFMethodInitParamBase, IJFStationBaseAcq, IJFMethodFlowAcq
    {
        static string PN_StationItemName = "写入工站数据项名称";
        static string IN_LocItem = "工作流内数据项";

        public JFCM_SetStationPoolValue_D()
        {
            DeclearInitParam(PN_StationItemName, typeof(string), "");
            DeclearInput(IN_LocItem, typeof(object), null);

        }

        JFMethodFlow _owner = null;
        public void SetFlow(JFMethodFlow mf)
        {
            _owner = mf;
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
                errorInfo = PN_StationItemName + "未设置";
                return false;
            }
            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            if(null == _owner)
            {
                errorInfo = "Owner MethodFlow is unset/null";
                return false;
            }

            if(_station == null)
            {
                errorInfo = "工站未设置";
                return false;
            }
            IJFDataPool stationDataPool = _station.DataPool;
            object srcItem = GetMethodInputValue(IN_LocItem);
            string stationItemName = GetInitParamValue(PN_StationItemName) as string;
            Type srcItemType = typeof(object);
            if (srcItem != null)
                srcItemType = srcItem.GetType();
            if (!stationDataPool.ContainItem(stationItemName))
            {
                if(!stationDataPool.RegistItem(stationItemName, srcItemType, srcItem))
                {
                    errorInfo = "向工站数据池写入/注册 数据项失败 stationItemName = \"" + stationItemName + "\", Type=" + srcItemType + ",Value=" + srcItem.ToString();
                    return false;
                }
            }
            else
            {
                if (!stationDataPool.SetItemValue(stationItemName, srcItem))
                {
                    errorInfo = "向工站数据池写入 数据项失败 stationItemName = \"" + stationItemName + "\", Type=" + srcItemType + ",Value=" + srcItem.ToString();
                    return false;
                }
            }
            errorInfo = "Success";
            return true;
        }


    }
}
