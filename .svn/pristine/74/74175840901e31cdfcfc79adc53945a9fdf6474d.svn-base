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
    [JFDisplayName("设置光源触发通道参数_D")]
    public class JFCM_SetLightTrigParam_D : JFMethodInitParamBase
    {
        public JFCM_SetLightTrigParam_D()
        {
            //string[] allLightChannelIDs = JFHubCenter.Instance.MDCellNameMgr.AllTrigChannelNames(); //所有光源触发通道名称（不是开关式光源）
            //DeclearInitParam(JFParamDescribe.Create("触发通道ID", typeof(string), JFValueLimit.Options, allLightChannelIDs), "");
            //DeclearInitParam(JFParamDescribe.Create("自动切换到触发模式", typeof(bool), JFValueLimit.NonLimit, null), true);
            //DeclearInitParam(JFParamDescribe.Create("自动使能", typeof(bool), JFValueLimit.NonLimit, null), true);
            //DeclearInitParam(JFParamDescribe.Create("触发强度", typeof(int), JFValueLimit.MinLimit | JFValueLimit.MaxLimit, new object[] { 0, 255 }), -1);
            DeclearInitParam(JFParamDescribe.Create("忽略不支持强度设置", typeof(bool), JFValueLimit.NonLimit, null), false);
            //DeclearInitParam(JFParamDescribe.Create("触发时长", typeof(int), JFValueLimit.MinLimit, new object[] { 0 }), -1);
            DeclearInitParam(JFParamDescribe.Create("忽略不支持时长设置", typeof(bool), JFValueLimit.NonLimit, null), false);
            //DeclearInitParam(JFParamDescribe.Create("输入源通道", typeof(int[]), JFValueLimit.NonLimit, null), new int[] { });
            //DeclearInitParam(JFParamDescribe.Create("忽略不支持源通道设置", typeof(bool), JFValueLimit.NonLimit, null), false);
            DeclearInput("触发通道ID", typeof(string), "");
            DeclearInput("触发强度", typeof(int), 0);
            DeclearInput("触发时长", typeof(int), 0);
        }
        protected override bool InitializeGenuine(out string errorInfo)
        {
            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            bool ignoreIntensity = (bool)GetInitParamValue("忽略不支持强度设置");
            bool ignoreDuration = (bool)GetInitParamValue("忽略不支持强度设置");
            string chnID = GetMethodInputValue("触发通道ID") as string;
            int intensity = (int)GetMethodInputValue("触发强度");
            int duration = (int)GetMethodInputValue("触发时长");
            if(string.IsNullOrEmpty(chnID))
            {
                errorInfo = "输入项:\"触发通道ID\"为空字串";
                return false;
            }

            if(!JFHubCenter.Instance.MDCellNameMgr.ContainLightChannelName(chnID))
            {
                errorInfo = "输入项:\"触发通道ID\" = " + chnID + " 在设备名称表中不存在";
                return false;
            }


            errorInfo = "功能暂未实现";
            return false;
        }
    }
}
