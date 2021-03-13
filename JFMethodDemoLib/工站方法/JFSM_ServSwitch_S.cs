using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using JFHub;

namespace JFMethodCommonLib
{
    /// <summary>
    /// 工站方法，用于操作工站声明轴的伺服的上电/断电
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 工站方法", "Axis" })]
    [JFDisplayName("轴(工站内定)伺服开关_S")]
    [JFVersion("1.0.0.0")]
    public class JFSM_ServSwitch_S : JFMethodInitParamBase, IJFStationBaseAcq
    {
        public JFSM_ServSwitch_S()
        {
            //DeclearInitParam(JFParamDescribe.Create("站内轴名称",typeof(string),))
            DeclearInitParam(PN_AxisName, typeof(string), "");
            DeclearInitParam(PN_ServOn, typeof(bool), true);
        }
        static string PN_AxisName = "轴(站内)名称";
        static string PN_ServOn = "伺服使能";
        JFStationBase _station = null;

        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == PN_AxisName)
            {
                string[] valueRange = null;
                if (null == _station)
                    valueRange = new string[] { };
                else
                {
                    valueRange = _station.AllDecDevChnNames(NamedChnType.Axis);
                    if (null == valueRange)
                        valueRange = new string[] { };
                }
                return JFParamDescribe.Create(PN_AxisName, typeof(string), JFValueLimit.Options, valueRange);
            }
            return base.GetInitParamDescribe(name);
        }

     


        public void SetStation(JFStationBase station)
        {
            _station = station;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {

            string locAxisName = GetInitParamValue(PN_AxisName) as string;
            if (string.IsNullOrEmpty(locAxisName))
            {
                errorInfo = "初始化参数项:\"" + PN_AxisName + "\"未设置/空值";
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            if (_station == null)
            {
                errorInfo = "工站未设置";
                return false;
            }

            string locAxisName = GetInitParamValue(PN_AxisName) as string;
            if (!_station.IsDevChnDecleared(NamedChnType.Axis, locAxisName))
            {
                errorInfo = PN_AxisName + ":\"" + locAxisName + "\"非工站内定义轴";
                return false;
            }

            bool isServoOn = Convert.ToBoolean(GetInitParamValue(PN_ServOn));
            string innerErrorInfo;
            if (!_station.AxisServoByAlias(locAxisName, isServoOn, out innerErrorInfo))
            {
                errorInfo = innerErrorInfo;
                return false;
            }
            errorInfo = "Success";
            return true;
        }
    }
}
