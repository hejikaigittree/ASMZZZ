using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFHub;
using JFInterfaceDef;
using DLAF;
namespace DLAFMethodLib
{
    [JFCategoryLevels(new string[] { "JF 系统", "Reccipe操作" })]
    [JFDisplayName("获取料盒Reccipe参数")]
    [JFVersion("1.0.0.0")]
    class JFCM_GetBoxRecipeData: JFMethodInitParamBase
    {
        private string mMagezineBox = "①对应料盒";
        private string mMgzIdx = "②料盒索引";
        private string mHeightLastSlot = "③上料最后槽上料位Z坐标";
        private string mHeightLastSlot_y = "③上料最后槽上料位Y坐标";
        private string mHeightFirstSlot = "④上料第一槽上料位Z坐标";
        private string mHeightFirstSlot_y = "④上料第一槽上料位Y坐标";
        private string mHeightLastSlot_Unload = "⑤下料最后槽上料位Z坐标";
        private string mHeightLastSlot_Unload_y = "⑤下料最后槽上料位Y坐标";
        private string mHeightFirstSlot_Unload = "⑥下料第一槽上料位Z坐标";
        private string mHeightFirstSlot_Unload_y = "⑥下料第一槽上料位Y坐标";
        private string mSlotNumber = "⑦料盒内槽数目";
        private string mBlankNumber_Unload = "⑧顺序下料间隔槽数目";
        private string mFrameWidth = "⑨导轨Y轴位";
        private string mLoad_y_LoadUnLoadFramePos = "⑩上料Y轴上片位";
        private string mUnLoad_y_LoadUnLoadFramePos = "⑫下料Y轴下片位";
        private string mLoad_x_PushRodWaitPos = "⑭上料推杆等待位";
        private string mLoad_x_PushRodOverPos = "⑮上料推杆结束位";
        private string mLoad_x_ChuckLoadFramePos = "①产品检测位";

        public JFCM_GetBoxRecipeData()
        {
            DeclearOutput(mHeightLastSlot,typeof(double),0);
            DeclearOutput(mHeightLastSlot_y, typeof(double), 0);
            DeclearOutput(mHeightFirstSlot, typeof(double), 0);
            DeclearOutput(mHeightFirstSlot_y, typeof(double), 0);

            DeclearOutput(mHeightLastSlot_Unload, typeof(double),0);
            DeclearOutput(mHeightLastSlot_Unload_y, typeof(double), 0);
            DeclearOutput(mHeightFirstSlot_Unload, typeof(double),0);
            DeclearOutput(mHeightFirstSlot_Unload_y, typeof(double),0);

            DeclearOutput(mSlotNumber, typeof(int), 0);
            DeclearOutput(mBlankNumber_Unload, typeof(int), 0);
            DeclearOutput(mFrameWidth, typeof(double), 0);
            DeclearOutput(mLoad_y_LoadUnLoadFramePos, typeof(double),0);

            DeclearOutput(mUnLoad_y_LoadUnLoadFramePos, typeof(double), 0);
            DeclearOutput(mLoad_x_PushRodWaitPos, typeof(double), 0);
            DeclearOutput(mLoad_x_PushRodOverPos, typeof(double), 0);
            DeclearOutput(mLoad_x_ChuckLoadFramePos, typeof(double),0);
        }

        protected override bool ActionGenuine(out string errorInfo)
        {
            JFDLAFBoxRecipe boxRecipe = ((JFDLAFBoxRecipe)JFHubCenter.Instance.RecipeManager.GetRecipe("Box",
                                          (string)JFHubCenter.Instance.SystemCfg.GetItemValue("CurrentID")));
            
            
            if(boxRecipe == null)
            {
                errorInfo = "料盒Reccipe不存在";
                return false;
            }
           
           SetOutputParamValue(mHeightLastSlot,(double)boxRecipe.HeightLastSlot);
           SetOutputParamValue(mHeightLastSlot_y, (double)boxRecipe.HeightLastSlot_y);
            SetOutputParamValue(mHeightFirstSlot, (double)boxRecipe.HeightFirstSlot);
            SetOutputParamValue(mHeightFirstSlot_y, (double)boxRecipe.HeightFirstSlot_y);

            SetOutputParamValue(mHeightLastSlot_Unload, (double)boxRecipe.HeightLastSlot_Unload);
            SetOutputParamValue(mHeightLastSlot_Unload_y, (double)boxRecipe.HeightLastSlot_Unload_y);
            SetOutputParamValue(mHeightFirstSlot_Unload, (double)boxRecipe.HeightFirstSlot_Unload);
            SetOutputParamValue(mHeightFirstSlot_Unload_y, (double)boxRecipe.HeightFirstSlot_Unload_y);

            SetOutputParamValue(mSlotNumber, (int)boxRecipe.SlotNumber);
            SetOutputParamValue(mBlankNumber_Unload, (double)boxRecipe.BlankNumber_Unload);
            SetOutputParamValue(mFrameWidth, (double)boxRecipe.FrameWidth);
            SetOutputParamValue(mLoad_y_LoadUnLoadFramePos, (double)boxRecipe.Load_y_LoadUnLoadFramePos);

            SetOutputParamValue(mUnLoad_y_LoadUnLoadFramePos, (int)boxRecipe.UnLoad_y_LoadUnLoadFramePos);
            SetOutputParamValue(mLoad_x_PushRodWaitPos, (double)boxRecipe.Load_x_PushRodWaitPos);
            SetOutputParamValue(mLoad_x_PushRodOverPos, (double)boxRecipe.Load_x_PushRodOverPos);
            SetOutputParamValue(mLoad_x_ChuckLoadFramePos, (double)boxRecipe.Load_x_ChuckLoadFramePos);
            errorInfo = "Success";
            return true;
        }

        protected override bool InitializeGenuine(out string errorInfo)
        {
            errorInfo = "Success";
            return true;
        }      
    }
}
