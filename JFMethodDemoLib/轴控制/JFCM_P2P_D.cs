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
    /// 轴的点位运动方法：带输入参数：目标位置/绝对位置方式
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 设备控制", "Axis" })]
    [JFDisplayName("轴位置移动_D")]
    public class JFCM_P2P_D : JFMethodInitParamBase
    {
        public JFCM_P2P_D()
        {
            //string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            //DeclearInput("轴ID", typeof(string), "");
            string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            DeclearInitParam(JFParamDescribe.Create("轴ID", typeof(string), JFValueLimit.Options, allAxisIDs), "");
            DeclearInput("目标位置", typeof(double), 0);
            DeclearInput("绝对位置模式", typeof(bool), true);
            DeclearOutput("执行结果", typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            //string axisID = GetMethodInputValue("轴ID") as string;
            //if (string.IsNullOrEmpty(axisID))
            //{
            //    errorInfo = "参数:\"轴ID\"未设置/空值";
            //    return false;
            //}
            
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

            //if (!AxisStatus[md.MSID_MDN])
            //{
            //    errorInfo = "轴当前运动未完成";
            //    SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
            //    return false;
            //}

            double dPos = Convert.ToDouble(GetMethodInputValue("目标位置"));
            bool IsAbsMove = Convert.ToBoolean(GetMethodInputValue("绝对位置模式"));
            if (IsAbsMove)
                err = md.AbsMove(ci.ChannelIndex, dPos);
            else
                err = md.RelMove(ci.ChannelIndex, dPos);
            if (err != 0)
            {
                errorInfo = (IsAbsMove ? "绝对" : "相对") + "位置运动失败:" + md.GetErrorInfo(err);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            errorInfo = "Success";
            SetOutputParamValue("执行结果", JFWorkCmdResult.Success);
            return true;
        }
    }


    
}
