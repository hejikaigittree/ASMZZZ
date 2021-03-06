using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;

namespace JFMethodCommonLib.轴控制
{
    [JFCategoryLevels(new string[] { "JF 设备控制", "Axis" })]
    [JFDisplayName("速度模式移动")]

    public class JFCM_VelMove_S : JFMethodInitParamBase
    {
        public JFCM_VelMove_S()
        {
            string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            DeclearInitParam(JFParamDescribe.Create("轴ID", typeof(string), JFValueLimit.Options, allAxisIDs), "");
            DeclearInitParam(JFParamDescribe.Create("移动速度", typeof(double), JFValueLimit.NonLimit, null), 0.0);
            DeclearInitParam(JFParamDescribe.Create("正向移动", typeof(bool), JFValueLimit.NonLimit, null), true);
            DeclearOutput("执行结果", typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string axisID = GetInitParamValue("轴ID") as string;
            if (string.IsNullOrEmpty(axisID))
            {
                errorInfo = "参数项:\"轴ID\"未设置/为空值";
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
                errorInfo = "参数项:\"轴ID\" = " + axisID + " 在设备名称表中不存在";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (!JFCMFunction.CheckDevCellName(JFCMFunction.Axis, axisID, out dev, out ci, out errorInfo))
            {
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            bool isPositive = (bool)GetInitParamValue("正向移动");
            double velocity = Convert.ToDouble(GetInitParamValue("移动速度"));

            IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            bool[] AxisStatus = null;
            int err = md.GetMotionStatus(ci.ChannelIndex, out AxisStatus);
            if (err != 0)
            {
                errorInfo = "开始运动前检测轴状态失败:" + md.GetErrorInfo(err);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            //if(md.IsSVO)
            if (!AxisStatus[md.MSID_SVO])
            {
                errorInfo = "轴伺服未上电";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (AxisStatus[md.MSID_ALM])
            {
                errorInfo = "轴伺服已报警";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (!AxisStatus[md.MSID_MDN])
            {
                errorInfo = "轴当前运动未完成";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            int errCode = md.VelMove(ci.ChannelIndex, velocity, isPositive);
            if (errCode < 0)
            {
                errorInfo = "SDK调用出错:" + md.GetErrorInfo(errCode);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            errorInfo = "Success";
            SetOutputParamValue("执行结果", JFWorkCmdResult.Success);
            return true;
        }
    }
}
