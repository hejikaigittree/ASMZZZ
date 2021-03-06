using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;

namespace JFMethodCommonLib
{
    [JFCategoryLevels(new string[] { "JF 设备控制", "Axis" })]
    [JFDisplayName("轴归零_S")]
    public class JFCM_Home_S : JFMethodInitParamBase
    {
        public JFCM_Home_S()
        {
            string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            DeclearInitParam(JFParamDescribe.Create("轴ID", typeof(string), JFValueLimit.Options, allAxisIDs), "");
            DeclearOutput("执行结果", typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }
        protected override bool ActionGenuine(out string errorInfo)
        { 
            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            string axisID = GetInitParamValue("轴ID") as string;
            if (!JFHubCenter.Instance.MDCellNameMgr.ContainAxisName(axisID))
            {
                errorInfo = "参数项:\"轴ID\"的值:\"" + axisID + "\"在设备名称表中不存在";
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
            if(err != 0)
            {
                errorInfo = "归0前获取轴状态失败:" + md.GetErrorInfo(err);
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


            err = md.Home(ci.ChannelIndex);
            if(err != 0)
            {
                errorInfo = "调用SDK失败:" + md.GetErrorInfo(err);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            errorInfo = "Success";
            SetOutputParamValue("执行结果", JFWorkCmdResult.Success);
            return true;
        }

        public override bool SetInitParamValue(string name, object value)
        {
            if(name == "轴ID")
            {
                string axisID = value as string;
                if(string.IsNullOrEmpty(axisID))
                {
                    InitErrorInfo = "参数项:\"轴ID\"为空字串";
                    return false;
                }

                if(!JFHubCenter.Instance.MDCellNameMgr.ContainAxisName(axisID))
                {
                    InitErrorInfo = "参数项:\"轴ID\"的值:\"" + axisID +"\"在设备名称表中不存在";
                    return false;
                }
            }
            return base.SetInitParamValue(name, value);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string axisID = GetInitParamValue("轴ID") as string;
            if(string.IsNullOrEmpty(axisID))
            {
                errorInfo = "参数项:\"轴ID\"未设置/空值"; 
                return false;
            }

            errorInfo = "Success";
            return true;
        }
    }
}
