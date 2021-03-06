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
    [JFDisplayName("设置轴运动参数")]
    public class JFCM_SetMotionParam:JFMethodInitParamBase
    {
        public JFCM_SetMotionParam()
        {
            string[] allAxisIDs = JFHubCenter.Instance.MDCellNameMgr.AllAxisNames();
            DeclearInitParam(JFParamDescribe.Create("轴ID", typeof(string), JFValueLimit.Options, allAxisIDs), "");
            DeclearInitParam(JFParamDescribe.Create("运动参数", typeof(JFMotionParam), JFValueLimit.Options, allAxisIDs), new JFMotionParam());
            DeclearOutput("执行结果", typeof(JFWorkCmdResult), JFWorkCmdResult.UnknownError);
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
            JFMotionParam mp = (JFMotionParam)GetInitParamValue("运动参数");
            /*
            public double vs { get; set; }
            /// <summary>最大速度</summary>
            public double vm { get; set; }
            /// <summary>结束速度</summary>
            public double ve { get; set; }
            /// <summary>加速度</summary>
            public double acc { get; set; }
            /// <summary>减速度</summary>
            public double dec { get; set; }
            /// <summary>s曲线因子(0~1.0)</summary>
            public double curve { get; set; }
            /// <summary>加加速</summary>
            public double jerk { get; set; }
            */
            if(mp.vs < 0)
            {
                errorInfo = "起始速度参数vs < 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (mp.vm <= 0)
            {
                errorInfo = "运行速度参数vm <= 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (mp.ve < 0)
            {
                errorInfo = "终点速度参数ve < 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (mp.acc <= 0)
            {
                errorInfo = "加速度参数acc <= 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if (mp.dec <= 0)
            {
                errorInfo = "减速度参数dec <= 0";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            if(mp.curve <0 || mp.curve > 1)
            {
                errorInfo = "加速度曲线段系数不在允许范围0~1";
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            int errCode = md.SetMotionParam(ci.ChannelIndex, mp);
            if(errCode < 0)
            {
                errorInfo = "SDK调用出错:" + md.GetErrorInfo(errCode);
                SetOutputParamValue("执行结果", JFWorkCmdResult.ActionError);
                return false;
            }

            errorInfo = "Success";
            SetOutputParamValue("执行结果", JFWorkCmdResult.Success);
            return true;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string axisID = GetInitParamValue("轴ID") as string;
            if(string.IsNullOrEmpty(axisID))
            {
                errorInfo = "参数项:\"轴ID\"未设置/为空值";
                return false;
            }

            errorInfo = "Success";
            return true;
        }
    }
}
