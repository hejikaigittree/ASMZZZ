using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;

namespace JFMethodCommonLib.相机控制
{
    /// <summary>
    /// 将相机模式切换到主动抓图
    /// 调用此方法后，相机可以主动抓取图片
    /// </summary>
    [JFVersion("1.0.0.0")]
    [JFCategoryLevels(new string[] { "JF 设备控制", "Camera" })]
    [JFDisplayName("相机准备主动抓图")]
    public class JFCM_PrepareSnap:JFMethodInitParamBase
    {
        static string PN_CmrID = "相机ID";
        public JFCM_PrepareSnap()
        {
            DeclearInitParam(PN_CmrID, typeof(string), "");
        }


        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if(name == PN_CmrID)
            {
                string[] allCmrIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_Camera));
                return JFParamDescribe.Create(PN_CmrID, typeof(string), JFValueLimit.Options, allCmrIDs);
            }
            return base.GetInitParamDescribe(name);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string cmrID = GetInitParamValue(PN_CmrID) as string;
            if(string.IsNullOrEmpty(cmrID))
            {
                errorInfo = PN_CmrID + " 未设置";
                return false;
            }

            errorInfo = "Success";
            return true;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            string cmrID = GetInitParamValue(PN_CmrID) as string;
            string[] allCmrIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_Camera));
            if (null == allCmrIDs)
            {
                errorInfo = "设备列表中不存在相机";
                return false;
            }

            if(!allCmrIDs.Contains(cmrID))
            {
                errorInfo = "设备列表中不存在ID = \"" + cmrID + "\"的相机设备";
                return false;
            }

            int errCode = 0;
            IJFDevice_Camera cmr = JFHubCenter.Instance.InitorManager.GetInitor(cmrID) as IJFDevice_Camera;
            if(!cmr.IsDeviceOpen)
            {
                errCode = cmr.OpenDevice();
                if(errCode != 0)
                {
                    errorInfo = "相机ID:\"" + cmrID + "\" 打开设备失败，ErrorInfo:" + cmr.GetErrorInfo(errCode);
                    return false;
                }
            }

            JFCmrTrigMode tm;
            errCode = cmr.GetTrigMode(out tm);
            if(errCode != 0)
            {
                errorInfo = "获取相机触发模式失败，信息:" + cmr.GetErrorInfo(errCode);
                return false;
            }
            if (cmr.IsRegistAcqFrameCallback || tm != JFCmrTrigMode.disable)
            {
                if (cmr.IsGrabbing)
                {
                    errCode = cmr.StopGrab();
                    if(errCode != 0)
                    {
                        errorInfo = "相机切换模式时关闭采集失败：" + cmr.GetErrorInfo(errCode);
                        return false;
                    }
                }


            }
            if(tm!= JFCmrTrigMode.disable)
            {
                errCode = cmr.SetTrigMode(JFCmrTrigMode.disable);
                if(errCode != 0)
                {
                    errorInfo = "相机切换为软件取图模式失败";
                    return false;
                }
            }

            if (cmr.IsRegistAcqFrameCallback)
                cmr.ClearAcqFrameCallback();


            if(!cmr.IsGrabbing)
            {
                errCode = cmr.StartGrab();
                if(errCode != 0 )
                {
                    errorInfo = "相机ID:\"" + cmrID + "\"启动抓图失败，信息:" + cmr.GetErrorInfo(errCode);
                    return false;
                }

            }

            errorInfo = "Success";
            return true;
        }


    }
}
