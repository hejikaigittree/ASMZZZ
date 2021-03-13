using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using LFAOIRecipe;
using HalconDotNet;

namespace DLAF_DS
{
    [JFDisplayName("LAF单图检测算法")]
    [JFVersion("1.0.0.0")]
    public class JFDLAFSingleImageInspection:JFMethodBase, IJFMethod_Vision
    {
        JFDLAFSingleImageInspection()
        {
            DeclearInput("RecipeID", typeof(string), "");
            DeclearInput("FovName", typeof(string), "");
            DeclearInput("Image", typeof(IJFImage), null);
            
            DeclearOutput("ErrorInfo", typeof(string), "None-Option");
            DeclearOutput("ErrorCode", typeof(int), -1);
            DeclearOutput("焊点区域", typeof(HObject), null);
            DeclearOutput("金线区域", typeof(HObject), null);
            DeclearOutput("失败区域", typeof(HObject), null);
            //_rtUi.SetInspection(this);
        }

        //JFRtUiSingleImageInspection _rtUi = new JFRtUiSingleImageInspection();

        public JFRealtimeUI GetRealtimeUI()
        {
            JFRtUiSingleImageInspection rtUi = new JFRtUiSingleImageInspection();
            rtUi.SetInspection(this);
            return rtUi;
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            try
            {
                IJFImage img = GetMethodInputValue("Image") as IJFImage;
                if(null == img)
                {
                    errorInfo = "输入图像为空";
                    return false;
                }
                string recipeID = GetMethodInputValue("RecipeID") as string;
                if(string.IsNullOrEmpty(recipeID))
                {
                    errorInfo = "RecipeID未设置";
                    return false;
                }

                string fovName = GetMethodInputValue("FovName") as string;
                if(string.IsNullOrEmpty(fovName))
                {
                    errorInfo = "FovName未设置";
                    return false;
                }

                Inspect_Node node = JFDLAFInspectionManager.Instance.GetInspectNode(recipeID,fovName);
                if(null == node)
                {
                    errorInfo = "RecipeID对应的InspectNode未初始化";
                    return false;
                }


                object hoImg = null;
                int ret = img.GenHalcon(out hoImg);
                if(ret!= 0)
                {
                    errorInfo = "JFImage to halcon failed:" + img.GetErrorInfo(ret);
                    return false;
                }
                
                
                int errorCode = -1;
                //string errorInfo = "Unknown-Error";
                HObject BondContours;
                HObject Wires;
                HObject FailRegs;
                string optErrorInfo = "";
                if (!node.InspectImage(hoImg as HObject, out errorCode, out optErrorInfo, out BondContours, out Wires, out FailRegs))
                {
                    errorInfo = optErrorInfo;
                    SetOutputParamValue("ErrorCode", errorCode);
                    SetOutputParamValue("ErrorInfo", optErrorInfo);
                    SetOutputParamValue("焊点区域", null);
                    SetOutputParamValue("金线区域", null);
                    SetOutputParamValue("失败区域", null);
                    errorInfo = "Success";
                    return false;
                }
                SetOutputParamValue("ErrorCode", errorCode);
                SetOutputParamValue("ErrorInfo", optErrorInfo);
                SetOutputParamValue("焊点区域", BondContours);
                SetOutputParamValue("金线区域", Wires);
                SetOutputParamValue("失败区域", FailRegs);
                errorInfo = "Success";
                return true;

            }
            catch(Exception ex)
            {
                errorInfo = ex.Message;
                return false;
            }
        }
    }
}
