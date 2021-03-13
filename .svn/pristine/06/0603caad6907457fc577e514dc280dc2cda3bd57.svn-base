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
    /// 等待(工站的)轴到达限位位置
    /// HTM运动控制卡到达限位后没有MotionDone信号，所以添加此方法
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 工站方法", "运动控制" })]
    [JFDisplayName("等待工站轴到达限位_S")]
    [JFVersion("1.0.0.0")]
    public class JFSM_WaitAxis2Limit_S:JFMethodInitParamBase,IJFStationBaseAcq
    {
        public static string PN_AxisName = "轴ID(全局)";
        public static string PN_IsPositiveLimit = "正限位";
        public static string PN_TimeoutMS = "超时时间(毫秒)";
        public static string ON_Result = "返回值";
        public JFSM_WaitAxis2Limit_S()
        {
            DeclearInitParam(PN_AxisName, typeof(string), "");
            DeclearInitParam(PN_IsPositiveLimit, typeof(bool), true);
            DeclearInitParam(PN_TimeoutMS, typeof(int), -1);
            DeclearOutput(ON_Result, typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }


        JFStationBase _station = null;
        public void SetStation(JFStationBase station)
        {
            _station = station;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            if(null == _station)
            {
                SetOutputParamValue(ON_Result, JFWorkCmdResult.UnknownError);
                errorInfo = "工站未设置";
                return false;
            }
            string axisID = GetInitParamValue(PN_AxisName) as string;
            bool isPositiveLimit = Convert.ToBoolean(GetInitParamValue(PN_IsPositiveLimit));
            int timeoutMS = Convert.ToInt32(GetInitParamValue(PN_TimeoutMS));
            if(!_station.ContainAxis(axisID))
            {
                SetOutputParamValue(ON_Result, JFWorkCmdResult.UnknownError);
                errorInfo = "工站未包含轴:" + axisID;
                return false;
            }

            JFWorkCmdResult ret = _station.WaitAxis2Limit(axisID, isPositiveLimit, timeoutMS);
            if(ret == JFWorkCmdResult.ActionError ||
                ret == JFWorkCmdResult.StatusError ||
                ret == JFWorkCmdResult.IllegalCmd ||
                ret == JFWorkCmdResult.UnknownError)
            {
                SetOutputParamValue(ON_Result, ret);
                errorInfo = "_station.WaitAxis2Limit return :" + ret;
                return false;
            }
            errorInfo = "Success";
            return true;

        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string axisID = GetInitParamValue(PN_AxisName) as string;
            if(string.IsNullOrEmpty(axisID))
            {
                errorInfo = PN_AxisName + " 未设置";
                return false;
            }
            errorInfo = "Success";
            return true;
        }
    }
}
