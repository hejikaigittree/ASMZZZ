using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;

namespace JFMethodCommonLib
{
    [JFCategoryLevels(new string[] { "JF 工站方法", "运动控制" })]
    [JFDisplayName("等待工站轴运动到位_S")]
    [JFVersion("1.0.0.0")]
    public class JFSM_WaitMotionDone_S:JFMethodInitParamBase,IJFStationBaseAcq
    {
        public static string PN_AxisName = "轴ID(全局)";
        public static string PN_TimeoutMS = "超时时间(毫秒)";
        public static string ON_Result = "返回值";
        public JFSM_WaitMotionDone_S()
        {
            DeclearInitParam(PN_AxisName, typeof(string), "");
            DeclearInitParam(PN_TimeoutMS, typeof(int), -1);
            DeclearOutput(ON_Result, typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string axisName = GetInitParamValue(PN_AxisName) as string;
            if(string.IsNullOrEmpty(axisName))
            {
                errorInfo = PN_AxisName + "未设置";
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            if(_station == null)
            {
                errorInfo = "工站未设置/空值";
                SetOutputParamValue(ON_Result, JFWorkCmdResult.UnknownError);
                return false;
            }
            string axisID = GetInitParamValue(PN_AxisName) as string;
            int timeoutMS = Convert.ToInt32(GetInitParamValue(PN_TimeoutMS));
            if(!_station.ContainAxis(axisID))
            {
                errorInfo = "工站未包含轴ID:\"" + axisID + "\"";
                SetOutputParamValue(ON_Result,JFWorkCmdResult.UnknownError);
                return false;
            }

            JFWorkCmdResult ret = _station.WaitMotionDone(axisID, timeoutMS);
            SetOutputParamValue(ON_Result, ret);
            errorInfo = "Success";
            return true;

            
        }


    }
}
