using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using JFMethodCommonLib.相机控制;

namespace JFMethodCommonLib
{
    [JFVersion("1.0.0.0")]
    [JFCategoryLevels(new string[] { "JF 设备控制", "Camera" })]
    [JFDisplayName("拍照")]
    public class JFCM_CmrSnapOne_S : JFMethodInitParamBase,IJFRealtimeUIProvider
    {
        static string PN_CmrName = "相机ID";
        //static string PN_AutoOpenDevice = "自动打开设备";
        //static string PN_AutoSwitchMode = "自动切换为抓图模式";
        //static string PN_AutoGrab = "自动开启采集";
        public static string ON_Image = "图片";
        public JFCM_CmrSnapOne_S()
        {
            DeclearInitParam(PN_CmrName, typeof(string), "");
            //DeclearInitParam(PN_AutoOpenDevice, typeof(bool), true);
            //DeclearInitParam(PN_AutoSwitchMode, typeof(bool), true);
            //DeclearInitParam(PN_AutoGrab, typeof(bool), true);
            DeclearOutput(ON_Image, typeof(IJFImage), null);

        }

        public override JFParamDescribe GetInitParamDescribe(string name)
        {
            if (name == PN_CmrName)
            {
                string[] allCmrIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_Camera));
                return JFParamDescribe.Create(PN_CmrName, typeof(string), JFValueLimit.Options, allCmrIDs);
            }
            return base.GetInitParamDescribe(name);
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            string cmrID = GetInitParamValue(PN_CmrName) as string;
            if(string.IsNullOrEmpty(cmrID))
            {
                errorInfo = PN_CmrName + " 未设置";
                return false;
            }
            errorInfo = "Success";
            return true;
        }


        protected override bool ActionGenuine(out string errorInfo)
        {
            string cmrID = GetInitParamValue(PN_CmrName) as string;
            //bool isAutoOpenDev = (bool)GetInitParamValue(PN_AutoOpenDevice);
            //bool isAutoSwitchMode = (bool)GetInitParamValue(PN_AutoSwitchMode);
            //bool isAutoGrab = (bool)GetInitParamValue(PN_AutoGrab);
            string[] allCmrIDs = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_Camera));
            if(null == allCmrIDs)
            {
                errorInfo = "抓图失败，设备列表中不存在相机";
                SetOutputParamValue(ON_Image, null);
                return false;
            }
            if(!allCmrIDs.Contains(cmrID))
            {
                errorInfo = "抓图失败，设备列表中未包含名称为:\"" + cmrID + "\"的相机";
                SetOutputParamValue(ON_Image, null);
                return false;
            }
            int errCode = 0;
            IJFDevice_Camera cmr = JFHubCenter.Instance.InitorManager.GetInitor(cmrID) as IJFDevice_Camera;
            IJFImage img = null;
            errCode = cmr.GrabOne(out img, 1000);
            if(errCode != 0)
            {
                errorInfo = "抓图失败,错误信息:" + cmr.GetErrorInfo(errCode);
                SetOutputParamValue(ON_Image, null);
                return false;
            }

            errorInfo = "Success";
            SetOutputParamValue(ON_Image, img);
            return true;

        }

        UcRTSnapOne rtUI = new UcRTSnapOne();
        public JFRealtimeUI GetRealtimeUI()
        {

            rtUI.SetMethod(this);
            return rtUI;
        }
    }
}
