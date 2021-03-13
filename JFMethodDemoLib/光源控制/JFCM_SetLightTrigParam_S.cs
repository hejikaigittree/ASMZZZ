using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
namespace JFMethodCommonLib
{
    [JFCategoryLevels(new string[] { "JF 设备控制", "LightTrigger" })]
    [JFDisplayName("设置光源触发通道参数_S")]
    public class JFCM_SetLightTrigParam_S:JFMethodInitParamBase
    {
        public JFCM_SetLightTrigParam_S()
        {
            string[] allLightChannelIDs = JFHubCenter.Instance.MDCellNameMgr.AllTrigChannelNames(); //所有光源触发通道名称（不是开关式光源）
            DeclearInitParam(JFParamDescribe.Create("触发通道ID", typeof(string), JFValueLimit.Options, allLightChannelIDs), "");
            DeclearInitParam(JFParamDescribe.Create("自动切换到触发模式", typeof(bool), JFValueLimit.NonLimit, null), true);
            DeclearInitParam(JFParamDescribe.Create("自动使能", typeof(bool), JFValueLimit.NonLimit, null), true);
            DeclearInitParam(JFParamDescribe.Create("触发强度", typeof(int), JFValueLimit.MinLimit | JFValueLimit.MaxLimit, new object[] { 0, 255 }), -1);
            DeclearInitParam(JFParamDescribe.Create("忽略不支持强度设置", typeof(bool), JFValueLimit.NonLimit, null), false);
            DeclearInitParam(JFParamDescribe.Create("触发时长", typeof(int), JFValueLimit.MinLimit , new object[] { 0}), -1);
            DeclearInitParam(JFParamDescribe.Create("忽略不支持时长设置", typeof(bool), JFValueLimit.NonLimit, null), false);
            DeclearInitParam(JFParamDescribe.Create("输入源通道", typeof(int[]), JFValueLimit.NonLimit, null), new int[] { });
            DeclearInitParam(JFParamDescribe.Create("忽略不支持源通道设置", typeof(bool), JFValueLimit.NonLimit, null), false);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string chnID = GetInitParamValue("触发通道ID") as string;
            if(string.IsNullOrEmpty(chnID))
            {
                errorInfo = "参数项:\"触发通道ID\"未设置/空值";
                return false;
            }

            if(!JFHubCenter.Instance.MDCellNameMgr.ContainTrigChannelName(chnID))
            {
                errorInfo = "参数项:\"触发通道ID\" = " + chnID + " 在设备名称表中不存在";
                return false;
            }

            int intensity = (int)GetInitParamValue("触发强度");
            if(intensity < 0)
            {
                errorInfo = "参数项:\"触发强度\" = " + intensity + " 未设置/不合法";
                return false;
            }

            int duration = (int)GetInitParamValue("触发时长");
            if(duration < 0)
            {
                errorInfo = "参数项:\"触发时长\" = " + intensity + " 未设置/不合法";
                return false;
            }
            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {

            string chnID = GetInitParamValue("触发通道ID") as string;
            bool isAutoSwitchWorkMode = (bool)GetInitParamValue("自动切换到触发模式");
            bool isAutoEnable = (bool)GetInitParamValue("自动使能"); 
            int intensity = (int)GetInitParamValue("触发强度");
            bool ignoreIntensity = (bool)GetInitParamValue("忽略不支持强度设置");
            int duration = (int)GetInitParamValue("触发时长"); 
            bool ignoreDuration = (bool)GetInitParamValue("忽略不支持时长设置");
            int[] srcChannels = GetInitParamValue("输入源通道") as int[];
            bool ignoreSrcChannels = (bool)GetInitParamValue("忽略不支持源通道设置");

            IJFInitializable initor = null;
            JFDevCellInfo ci = null;
            if(!JFCMFunction.CheckDevCellName(JFCMFunction.LightTrig,chnID,out initor, out ci,out errorInfo))
            {
                return false;
            }
            int errCode = 0;
            IJFDevice_TrigController dev = initor as IJFDevice_TrigController;
            if(typeof(IJFDevice_LightControllerWithTrig).IsAssignableFrom(dev.GetType())) //当前设备同时也是开关式光源控制器
            {
                IJFDevice_LightControllerWithTrig lightCtrlWithT = dev as IJFDevice_LightControllerWithTrig;
                JFLightWithTrigWorkMode wm;
                errCode = lightCtrlWithT.GetWorkMode(out wm);
                if(errCode != 0)
                {
                    errorInfo = "获取工作模式失败:" + lightCtrlWithT.GetErrorInfo(errCode);
                    return false;
                }

                if(wm == JFLightWithTrigWorkMode.TurnOnOff) //当前处于开关模式
                {
                    if(!isAutoSwitchWorkMode)
                    {
                        errorInfo = "当前工作模式为开关光源模式";
                        return false;
                    }

                    errCode = lightCtrlWithT.SetWorkMode(JFLightWithTrigWorkMode.Trigger);
                    if(errCode !=0)
                    {
                        errorInfo = "未能将光源控制器切换为触发模式:" + lightCtrlWithT.GetErrorInfo(errCode);
                        return false;
                    }
                }

            }

            bool isChnEnabled = false;
            errCode = dev.GetTrigChannelEnable(ci.ChannelIndex, out isChnEnabled);
            if(0!= errCode)
            {
                errorInfo = "获取通道使能状态失败:" + dev.GetErrorInfo(errCode);
                return false;
            }

            if(!isChnEnabled)
            {
                if(!isAutoEnable)
                {
                    errorInfo = "触发通道当前未使能";
                    return false;
                }
                errCode = dev.SetTrigChannelEnable(ci.ChannelIndex, true);
                if(errCode != 0)
                {
                    errorInfo = "使能触发通道失败:" + dev.GetErrorInfo(errCode);
                    return false;
                }
            }

            if(null != srcChannels && srcChannels.Length != 0) //需要绑定输入源通道
            {
                int mask = 0; //输入通道掩码
                foreach (int srcChn in srcChannels)
                    mask += 1 << mask;
                errCode = dev.SetTrigChannelSrc(ci.ChannelIndex, mask);
                if(errCode != 0  && !ignoreSrcChannels)
                {
                    errorInfo = "绑定触发源通道失败:" + dev.GetErrorInfo(errCode);
                    return false;
                }
            }

            errCode = dev.SetTrigChannelIntensity(ci.ChannelIndex, intensity);
            if(errCode != 0 && !ignoreIntensity)
            {
                errorInfo = "设置触发强度:" + intensity + " 失败：" + dev.GetErrorInfo(errCode);
                return false;
            }

            errCode = dev.SetTrigChannelDuration(ci.ChannelIndex, duration);
            if (errCode != 0 && !ignoreDuration)
            {
                errorInfo = "设置触发时长:" + duration + " 失败：" + dev.GetErrorInfo(errCode);
                return false;
            }
            errorInfo = "Success";
            return true;
        }


    }
}
