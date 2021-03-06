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
    [JFDisplayName("设置轴归零参数")]
    class JFCM_SetHomeParam_S : JFMethodInitParamBase
    {
        public JFCM_SetHomeParam_S()
        {
            string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            DeclearInitParam(JFParamDescribe.Create("轴ID", typeof(string), JFValueLimit.Options, allAxisIDs), "");
            DeclearInitParam(JFParamDescribe.Create("轴归零参数", typeof(JFHomeParam), JFValueLimit.Options, allAxisIDs), new JFHomeParam());
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
            JFHomeParam hp = (JFHomeParam)GetInitParamValue("轴归零参数");
            /*
            归零模式  0:使用Org（原点）作为归零参考   1：使用限位信号作为归零参考   2：仅使用EZ信号作为归零参考
            */
            if (hp.mode!=0 && hp.mode != 1 && hp.mode != 2 && hp.mode != -1 && hp.mode != -2)
            {
                errorInfo = "归零模式设置错误,mode="+hp.mode;
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (hp.acc <= 0)
            {
                errorInfo = "加速度/减速度acc <= 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (hp.vm <= 0)
            {
                errorInfo = "最大速度参数vm <= 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (hp.vo <= 0)
            {
                errorInfo = "寻找原点速度vo <= 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (hp.va <= 0)
            {
                errorInfo = "接近速度va <= 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (hp.shift < 0)
            {
                errorInfo = "回零偏移量sift < 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (hp.offset < 0)
            {
                errorInfo = "回零补偿值offset < 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.SetHomeParam(ci.ChannelIndex, hp);
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
