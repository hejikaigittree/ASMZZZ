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
    /// JFCM_GetDI_S 获取一个DI通道的状态
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 设备控制", "DIO" })]
    [JFDisplayName("获取DI状态_S")]
    public class JFCM_GetDI_S : JFMethodInitParamBase
    {
        public JFCM_GetDI_S()
        {
            string[] allDiNames = JFHubCenter.Instance.MDCellNameMgr.AllDiNames();
            DeclearInitParam(JFParamDescribe.Create("DI通道名称", typeof(string), JFValueLimit.Options, allDiNames), "");
            DeclearOutput("DI状态", typeof(bool), false);
            DeclearOutput("执行结果", typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string diName = GetInitParamValue("DI通道名称") as string;
            if (string.IsNullOrEmpty(diName))
            {
                errorInfo = "参数项\"DI通道名称\"未设置/空字串";
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            string diName = GetInitParamValue("DI通道名称") as string;
            if (!JFHubCenter.Instance.MDCellNameMgr.ContainDiName(diName))
            {
                errorInfo = "DI通道名称:" + diName + " 在设备命名表中不存在";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            if (!JFCMFunction.CheckDevCellName(JFCMFunction.DI, diName, out dev, out ci, out errorInfo))
            {
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            bool isTurnOn = false;
            int errCode = (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex).GetDI(ci.ChannelIndex, out isTurnOn);
            if (errCode != 0)
            {
                errorInfo = (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex).GetErrorInfo(errCode);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            errorInfo = "Success";
            SetOutputParamValue("执行结果", JFWorkCmdResult.Success);
            SetOutputParamValue("DI状态", isTurnOn);
            return true;
        }
    }
}

