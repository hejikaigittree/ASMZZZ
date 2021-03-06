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
    /// JFCM_GetDO_S 获取一个DO通道的状态
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 设备控制", "DIO" })]
    [JFDisplayName("获取DO状态_S")]
    public class JFCM_GetDO_S : JFMethodInitParamBase
    {
        public JFCM_GetDO_S()
        {
            string[] allDoNames = JFHubCenter.Instance.MDCellNameMgr.AllDoNames();
            DeclearInitParam(JFParamDescribe.Create("DO通道名称", typeof(string), JFValueLimit.Options, allDoNames), "");
            DeclearOutput("DO状态", typeof(bool), false);
            DeclearOutput("执行结果", typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string doName = GetInitParamValue("DO通道名称") as string;
            if (string.IsNullOrEmpty(doName))
            {
                errorInfo = "参数项\"DO通道名称\"未设置/空字串";
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            string doName = GetInitParamValue("DO通道名称") as string;
            if (!JFHubCenter.Instance.MDCellNameMgr.ContainDoName(doName))
            {
                errorInfo = "DO通道名称:" + doName + " 在设备命名表中不存在";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            if (!JFCMFunction.CheckDevCellName(JFCMFunction.DO, doName, out dev, out ci, out errorInfo))
            {
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            bool isTurnOn = false;
            int errCode = (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex).GetDO(ci.ChannelIndex, out isTurnOn);
            if (errCode != 0)
            {
                errorInfo = (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex).GetErrorInfo(errCode);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            errorInfo = "Success";
            SetOutputParamValue("执行结果", JFWorkCmdResult.Success);
            SetOutputParamValue("DO状态", isTurnOn);
            return true;

        }
    }
}
