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
    /// JFCM_SetLightIntensity:设置开关式光源的光强
    /// </summary>
    [JFCategoryLevels(new string[] { "JF 设备控制" ,"LightCtrl"})]
    [JFDisplayName("设置光强_S")]
    public class JFCM_SetLightIntensity_S:JFMethodInitParamBase
    {
        public JFCM_SetLightIntensity_S()
        {
            string[] allLightChannelIDs = JFHubCenter.Instance.MDCellNameMgr.AllLightChannelNames();
            DeclearInitParam(JFParamDescribe.Create("光源通道ID", typeof(string), JFValueLimit.Options, allLightChannelIDs), "");
            DeclearInitParam(JFParamDescribe.Create("自动切换到开关模式", typeof(bool), JFValueLimit.NonLimit, null), true);
            DeclearInitParam(JFParamDescribe.Create("自动使能", typeof(bool), JFValueLimit.NonLimit, null), true);
            DeclearInitParam(JFParamDescribe.Create("光照强度", typeof(int), JFValueLimit.MinLimit|JFValueLimit.MaxLimit, new object[] { 0,255}), -1);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string chnID = GetInitParamValue("光源通道ID") as string;
            if (string.IsNullOrEmpty(chnID))
            {
                errorInfo = "参数项:\"光源通道ID\"未设置/空值";
                return false;
            }
            if(!JFHubCenter.Instance.MDCellNameMgr.ContainLightChannelName(chnID))
            {
                errorInfo = "参数项:\"光源通道ID\" = " + chnID + " 在设备名称表中不存在";
                return false;
            }

            int lightIintensity = (int)GetInitParamValue("光照强度");
            if(lightIintensity < 0)
            {
                errorInfo = "参数项:\"光照强度\"未设置/-1";
                return false;
            }

            errorInfo = "Success";
            return true;
        }


        protected override bool ActionGenuine(out string errorInfo)
        {

            string chnID = GetInitParamValue("光源通道ID") as string;
            int lightIntensity = (int)GetInitParamValue("光照强度");
            bool isAutoSwitchMode = (bool)GetInitParamValue("自动切换到开关模式");
            bool isAutoEnable = (bool)GetInitParamValue("自动使能");
            IJFInitializable dev = null;
            JFDevCellInfo ci = null;
            if(!JFCMFunction.CheckDevCellName(JFCMFunction.LightCtrl,chnID,out dev,out ci,out errorInfo))
            {
                return false;
            }

            int errCode = 0;
            IJFDevice_LightController devLight = dev as IJFDevice_LightController;
            if(typeof(IJFDevice_LightControllerWithTrig).IsAssignableFrom(devLight.GetType())) //如果当前设备带有触发功能
            {
                IJFDevice_LightControllerWithTrig devLT = devLight as IJFDevice_LightControllerWithTrig;
                JFLightWithTrigWorkMode wm;
                errCode = devLT.GetWorkMode(out wm);
                if(errCode != 0)
                {
                    errorInfo = "获取光源控制器工作模式失败:" + devLT.GetErrorInfo(errCode);
                    return false;
                }

                if(wm == JFLightWithTrigWorkMode.Trigger) //当前处于触发模式
                {
                    if(!isAutoSwitchMode)
                    {
                        errorInfo = "控制器当前为触发模式";
                        return false;
                    }

                    errCode = devLT.SetWorkMode(JFLightWithTrigWorkMode.TurnOnOff);
                    if(errCode != 0)
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

            errCode = devLight.SetLightIntensity(ci.ChannelIndex, lightIntensity);
            if (errCode != 0 )
            {
                errorInfo = "设置光照强度失败：" + devLight.GetErrorInfo(errCode);
                return false;
            }

            errorInfo = "Success";
            return true;
        }


    }
}
