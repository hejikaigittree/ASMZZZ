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
    /// 设置一个DO的使能状态 ， DO名称和操作（打开/关闭）都是参数化
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 设备控制", "DIO" })]
    [JFDisplayName("设置DO状态_D")]
    public class JFCM_SetDO_D:JFMethodInitParamBase
    {
        public JFCM_SetDO_D()
        {
            string[] allDoNames = JFHubCenter.Instance.MDCellNameMgr.AllDoNames();
            DeclearInput("DO通道名称", typeof(string), "");
            DeclearInput("DO状态", typeof(bool), true);
            DeclearOutput("执行结果", typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            //string doName = GetMethodInputValue("DO通道名称") as string;
            //if(string.IsNullOrEmpty(doName))
            //{
            //    errorInfo = "参数项:\"DO通道名称\"未设置/为空字串";
            //    return false;
            //}

            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            string doName = GetMethodInputValue("DO通道名称") as string;
            bool enabled = (bool)GetMethodInputValue("DO状态");
            if (!JFHubCenter.Instance.MDCellNameMgr.ContainDoName(doName))
            {
                errorInfo = "参数项:\"DO通道名称\"的值:\"" + doName + "\"在设备名称表中不存在";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            if(!JFCMFunction.CheckDevCellName(JFCMFunction.DO,doName,out dev,out ci,out errorInfo))
            {
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            int errCode = (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex).SetDO(ci.ChannelIndex, enabled);
            if (errCode != 0)
            {
                errorInfo = (enabled ? "打开" : "关闭") + "DO = " + doName + " 失败，" + (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex).GetErrorInfo(errCode);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }
            errorInfo = "Success";
            SetOutputParamValue("执行结果", JFWorkCmdResult.Success);
            return true;
        }


    }
}
