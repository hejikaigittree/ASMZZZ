using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;
namespace JFMethodCommonLib
{
    /// <summary>
    /// JFCM_SetLightIntensity_D 设置光源通道亮度，通道名称和亮度值由输入参数确定
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 设备控制", "LightCtrl" })]
    [JFDisplayName("设置光强_D")]
    public class JFCM_SetLightIntensity_D:JFMethodInitParamBase
    {
        public JFCM_SetLightIntensity_D()
        {
            string[] allLightChannelIDs = JFHubCenter.Instance.MDCellNameMgr.AllLightChannelNames();
            DeclearInitParam(JFParamDescribe.Create("自动切换到开关模式", typeof(bool), JFValueLimit.NonLimit, null), true);
            DeclearInitParam(JFParamDescribe.Create("自动使能", typeof(bool), JFValueLimit.NonLimit, null), true);
            DeclearInput("光源通道ID", typeof(string), "");
            DeclearInput("光照强度", typeof(int), -1);

        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            string chnID = GetMethodInputValue("光源通道ID") as string;
            int intensity = (int)GetMethodInputValue("光照强度");
            if (string.IsNullOrEmpty(chnID))
            {
                errorInfo = "输入参数项:\"光源通道ID\" 为空字串";
                return false;
            }
            if(!JFHubCenter.Instance.MDCellNameMgr.ContainLightChannelName(chnID))
            {
                errorInfo = "输入参数项:\"光源通道ID\"  = " + chnID + " 在设备名称表中不存在";
                return false;
            }

            if(intensity < 0)
            {
                errorInfo = "输入参数项:\"光照强度\"  = " + intensity + " 为无效值（参数值必须>=0）";
                return false;
            }

            bool isAutoSwitchMode = (bool)GetInitParamValue("自动切换到开关模式");
            bool isAutoEnable = (bool)GetInitParamValue("自动使能");


            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            string errInfo = null;
            if (!JFCMFunction.CheckDevCellName(JFCMFunction.LightCtrl, chnID, out dev, out ci, out errInfo))
            {
                errorInfo = errInfo;
                return false;
            }

            int errCode = 0;
            IJFDevice_LightController devLight = dev as IJFDevice_LightController;
            if (typeof(IJFDevice_LightControllerWithTrig).IsAssignableFrom(devLight.GetType())) //如果当前设备带有触发功能
            {
                IJFDevice_LightControllerWithTrig devLT = devLight as IJFDevice_LightControllerWithTrig;
                JFLightWithTrigWorkMode wm;
                errCode = devLT.GetWorkMode(out wm);
                if (errCode != 0)
                {
                    errorInfo = "获取光源控制器工作模式失败:" + devLT.GetErrorInfo(errCode);
                    return false;
                }

                if (wm == JFLightWithTrigWorkMode.Trigger) //当前处于触发模式
                {
                    if (!isAutoSwitchMode)
                    {
                        errorInfo = "控制器当前为触发模式";
                        return false;
                    }

                    errCode = devLT.SetWorkMode(JFLightWithTrigWorkMode.TurnOnOff);
                    if (errCode != 0)
                    {
                        errorInfo = "控制器切换工作模式失败:" + devLT.GetErrorInfo(errCode);
                        return false;
                    }
                }



            }

            bool isLightChnEnabled = false;
            errCode = devLight.GetLightChannelEnable(ci.ChannelIndex, out isLightChnEnabled);
            if (0 != errCode)
            {
                errorInfo = "获取通道使能状态失败:" + devLight.GetErrorInfo(errCode);
                return false;
            }
            if (!isLightChnEnabled)
            {
                if (!isAutoEnable)
                {
                    errorInfo = "光源通道未使能";
                    return false;
                }
                errCode = devLight.SetLightChannelEnable(ci.ChannelIndex, true);
                if (errCode != 0)
                {
                    errorInfo = "光源通道使能失败：" + devLight.GetErrorInfo(errCode);
                    return false;
                }
            }

            errCode = devLight.SetLightIntensity(ci.ChannelIndex, intensity);
            if (errCode != 0)
            {
                errorInfo = "设置光照强度失败：" + devLight.GetErrorInfo(errCode);
                return false;
            }
            errorInfo = "Success";
            return true;
        }


    }
}
