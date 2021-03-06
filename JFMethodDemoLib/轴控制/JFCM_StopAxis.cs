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
    [JFDisplayName("轴停止")]
    public class JFCM_StopAxis:JFMethodInitParamBase
    {
        public JFCM_StopAxis()
        {
            string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            DeclearInitParam(JFParamDescribe.Create("轴ID", typeof(string), JFValueLimit.Options, allAxisIDs), "");
            DeclearOutput("执行结果", typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
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

        protected override bool ActionGenuine(out string errorInfo)
        {
            string axisID = GetInitParamValue("轴ID") as string;
            if (!JFHubCenter.Instance.MDCellNameMgr.ContainAxisName(axisID))
            {
                errorInfo = "参数项:\"轴ID\" = " + axisID + " 在设备名称表中不存在";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            if(!JFCMFunction.CheckDevCellName(JFCMFunction.Axis, axisID,out dev,out ci,out errorInfo))
            {
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }
            IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            //if(!md.IsSVO(ci.ChannelIndex)) //HTM控制卡在未上电时，调用Stop会异常退出
            //{
            //    errorInfo = "\"轴ID\" = " + axisID + " 未励磁";
            //    SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
            //    return true;
            //}
            int errCode = md.StopAxis(ci.ChannelIndex);
            if(errCode != 0)
            {
                errorInfo = md.GetErrorInfo(errCode);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            errorInfo = "Success";
            SetOutputParamValue("执行结果", JFWorkCmdResult.Success);
            return true;
        }


    }
}
