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
    [JFDisplayName("直线插补_D")]
    class JFCM_LineMove_D : JFMethodInitParamBase
    {
        public JFCM_LineMove_D()
        {
            string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            DeclearInitParam(JFParamDescribe.Create("轴ID列表", typeof(string[]), JFValueLimit.NonLimit, null), new string[] { });
            DeclearInput("目标位置列表",typeof(double), new double[] { });
            DeclearInput("绝对位置模式", typeof(bool), true);
            DeclearOutput("执行结果", typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string[] axisIDList = GetInitParamValue("轴ID列表") as string[];

            if (axisIDList == null)
            {
                errorInfo = "参数项:\"轴ID\"未设置/空值";
                return false;
            }

            if (axisIDList.Length == 0)
            {
                errorInfo = "参数项:\"轴ID\"未设置/空值";
                return false;
            }

            foreach (string axisID in axisIDList)
            {
                if (string.IsNullOrEmpty(axisID))
                {
                    errorInfo = "参数项:\"轴ID\"未设置/空值";
                    return false;
                }
            }

            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            string[] axisIDList = GetInitParamValue("轴ID列表") as string[];
            double[] dPosList = GetMethodInputValue("目标位置列表") as double[];
            bool IsAbsMove = (bool)GetMethodInputValue("绝对位置模式");

            if (axisIDList == null || dPosList == null)
            {
                errorInfo = "轴ID列表长度或者目标位置列表长度为空";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            int[] iaxisIDList = new int[axisIDList.Length];
            if (axisIDList.Length <= 0)
            {
                errorInfo = "轴ID列表长度<=0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (dPosList.Length <= 0)
            {
                errorInfo = "目标位置列表<=0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (axisIDList.Length != dPosList.Length)
            {
                errorInfo = "轴ID列表长度为" + axisIDList.ToString() + "与目标列表长度" + dPosList.Length.ToString() + "不一致。";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            IJFInitializable dev = null;
            IJFInitializable devbuff = null;
            JFDevCellInfo ci = null;
            List<JFDevCellInfo> cibuffList = new List<JFDevCellInfo>();

            for (int m = 0; m < axisIDList.Length; m++)
            {
                if (!JFHubCenter.Instance.MDCellNameMgr.ContainAxisName(axisIDList[m]))
                {
                    errorInfo = "参数项:\"轴ID\" = " + axisIDList[m] + " 在设备名称表中不存在";
                    SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                    return false;
                }
                if (!JFCMFunction.CheckDevCellName(JFCMFunction.Axis, axisIDList[m], out dev, out ci, out errorInfo))
                {
                    SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                    return false;
                }
                iaxisIDList[m] = ci.ChannelIndex;
                if (devbuff != null)
                {
                    if (devbuff != dev)
                    {
                        errorInfo = "轴ID列表中的所有轴并不来源于同一个设备";
                        SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                        return false;
                    }
                }
                devbuff = dev;

                if(cibuffList.Contains(ci))
                {
                    errorInfo = "轴ID列表中存在重复的轴名称";
                    SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                    return false;
                }
                cibuffList.Add(ci);
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

            if (!AxisStatus[md.MSID_MDN])
            {
                errorInfo = "轴当前运动未完成";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (IsAbsMove)
                err = md.AbsLine(iaxisIDList, dPosList);
            else
                err = md.RelLine(iaxisIDList, dPosList);
            if (err != 0)
            {
                errorInfo = (IsAbsMove ? "绝对" : "相对") + "直线插补运动失败:" + md.GetErrorInfo(err);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            errorInfo = "Success";
            SetOutputParamValue("执行结果", JFWorkCmdResult.Success);
            return true;
        }
    }
}
