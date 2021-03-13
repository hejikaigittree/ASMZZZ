using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using LFAOIRecipe;
using HalconDotNet;
using DLAF;

namespace DLAF_DS
{
    [JFDisplayName("LAF单图检测算法")]
    [JFVersion("1.0.0.0")]
    public class DLAFMethod_Inspection:JFMethodBase, IJFMethod_Vision
    {
        DLAFMethod_Inspection()
        {
            DeclearInput("RecipeID", typeof(string), "");
            DeclearInput("FovName", typeof(string), "");
            DeclearInput("TaskImages", typeof(IJFImage[]), null);


            DeclearOutput("DetectResult", typeof(bool), false);
            DeclearOutput("DetectErrorInfo", typeof(string), "");
            DeclearOutput("DieErrorInfos", typeof(string[]), null);
            DeclearOutput("DiesErrorCodes", typeof(List<int[]>), null);
            DeclearOutput("BondRegions", typeof(HObject), null);
            DeclearOutput("WiresRegions", typeof(HObject), null);
            DeclearOutput("FailRegions", typeof(HObject), null);
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
            errorInfo = "已作废，未实现功能！";
            return false;
#if false
            try
            {
                IJFImage[] imgs = GetMethodInputValue("TaskImages") as IJFImage[];
                if(null == imgs || 0 == imgs.Length)
                {
                    errorInfo = "输入参数项:TaskImages为空";
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

                string[] taskNames = ((JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager).GetRecipe("Product", recipeID) as JFDLAFProductRecipe).TaskNames(fovName); 

                Inspect_Node node = JFDLAFInspectionManager.Instance.GetInspectNode(recipeID,fovName);
                if(null == node)
                {
                    errorInfo = "Recipe_Fov对应的InspectNode未初始化";
                    return false;
                }


                List<HObject> lsthimgs = new List<HObject>();
                for (int i = 0; i < imgs.Length; i++)
                {
                    object hoImg = null;
                    int ret = imgs[i].GenHalcon(out hoImg);
                    if (ret != 0)
                    {
                        errorInfo = "JFImage to halcon failed:" + imgs[i].GetErrorInfo(ret);
                        return false;
                    }
                    lsthimgs.Add(hoImg as HObject);
                }
                
                
                List<int[]> diesErrorCodes = null;
                string[] dieErrorInfos = null;

                //string errorInfo = "Unknown-Error";
                HObject BondContours;
                HObject Wires;
                HObject FailRegs;
                string optErrorInfo = "";
                
                if (!node.InspectImage(lsthimgs.ToArray(), taskNames,out optErrorInfo, out diesErrorCodes, out dieErrorInfos,out BondContours, out Wires, out FailRegs))
                {
                    SetOutputParamValue("DetectResult", false);
                    SetOutputParamValue("DetectErrorInfo", optErrorInfo);
                    SetOutputParamValue("DieErrorCodes", null);
                    SetOutputParamValue("DieErrorInfos", null);
                    SetOutputParamValue("BondRegions", null);
                    SetOutputParamValue("WiresRegions", null);
                    SetOutputParamValue("FailRegions", null);
                    errorInfo = "Inspect_Node.InspectImage failed errorInfo:" + optErrorInfo;
                    return true; //此处只表示Action执行成功，但是检测出错
                }
                SetOutputParamValue("DetectResult", true);
                SetOutputParamValue("DetectErrorInfo", optErrorInfo);
                SetOutputParamValue("DiesErrorCodes", diesErrorCodes);
                SetOutputParamValue("DieErrorInfos", dieErrorInfos);
                SetOutputParamValue("BondRegions", BondContours);
                SetOutputParamValue("WiresRegions", Wires);
                SetOutputParamValue("FailRegions", FailRegs);
                errorInfo = "Success";
                return true;

            }
            catch(Exception ex)
            {
                errorInfo = ex.Message;
                return false;
            }
#endif //false

        }
    }
}
