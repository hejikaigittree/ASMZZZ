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
    /// JFCM_ServoSwitch_S 轴伺服上电/断电方法
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 设备控制", "Axis" })]
    [JFDisplayName("轴伺服开关_S")]
    public class JFCM_ServoSwitch_S: JFMethodInitParamBase
    {
        public JFCM_ServoSwitch_S()
        {
            string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            DeclearInitParam(JFParamDescribe.Create("轴ID", typeof(string), JFValueLimit.Options, allAxisIDs), "");
            DeclearInitParam(JFParamDescribe.Create("轴是否励磁", typeof(bool), JFValueLimit.NonLimit, null), true);
            DeclearOutput("执行结果", typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string axisID = GetInitParamValue("轴ID") as string;
            if (string.IsNullOrEmpty(axisID))
            {
                errorInfo = "参数:\"轴ID\"未设置/空值";
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            string axisID = GetInitParamValue("轴ID") as string;
            if (!JFHubCenter.Instance.MDCellNameMgr.ContainAxisName(axisID))
            {
                errorInfo = "参数:\"轴ID\" = " + axisID + " 在设备名称表中不存在";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (!JFCMFunction.CheckDevCellName(JFCMFunction.Axis, axisID, out dev, out ci, out errorInfo))
            {
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int err = 0;
            bool IsServoOn = (bool)GetInitParamValue("轴是否励磁");
            if (IsServoOn)
                err = md.ServoOn(ci.ChannelIndex);
            else
                err = md.ServoOff(ci.ChannelIndex);
            if (err != 0)
            {
                errorInfo = (IsServoOn ? "励磁使能ON" : "励磁使能OFF") + "失败:" + md.GetErrorInfo(err);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            errorInfo = "Success";
            SetOutputParamValue("执行结果", JFWorkCmdResult.Success);
            return true;
        }
    }
}
