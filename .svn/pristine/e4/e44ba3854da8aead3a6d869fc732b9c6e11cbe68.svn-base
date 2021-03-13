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
    /// 打开通道所属设备
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 工站方法", "设备控制" })]
    [JFDisplayName("打开通道所属设备_S")]
    [JFVersion("1.0.0.0")]
    public class JFSM_OpenChnDev_S:JFMethodInitParamBase,IJFStationBaseAcq
    {
        static string PN_DevChnName = "设备通道名称";
        static string PN_DevChnType = "设备通道类型";
        public JFSM_OpenChnDev_S()
        {
            DeclearInitParam(PN_DevChnType, typeof(NamedChnType), NamedChnType.None);
            DeclearInitParam(PN_DevChnName, typeof(string), "");
        }


        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if(name == PN_DevChnName)
            {
                //日后添加可选的设备名称
                //if(_station != null)
                //    string[] allDevNames = 

            }
            return base.GetInitParamDescribe(name);
        }


        protected override bool InitializeGenuine(out string errorInfo)
        {
            NamedChnType chnType = (NamedChnType)GetInitParamValue(PN_DevChnType);
            if(chnType == NamedChnType.None)
            {
                errorInfo = PN_DevChnType + " 未设置";
                return false;
            }
            string devChnName = GetInitParamValue(PN_DevChnName) as string;
            if(string.IsNullOrEmpty(devChnName))
            {
                errorInfo = PN_DevChnName + " 未设置";
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
            NamedChnType chnType = (NamedChnType)GetInitParamValue(PN_DevChnType);
            string devChnName = GetInitParamValue(PN_DevChnName) as string;
            return _station.OpenChnDevice(chnType, devChnName, out errorInfo);

        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }
    }
}
