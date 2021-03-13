using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using LFAOIRecipe;

namespace DLAF_DS
{
    [JFDisplayName("DLAF视觉算子初始化")]
    [JFVersion("1.0,0,0")]
    public class DLAFMathod_InitInspect : JFMethodBase
    {
        DLAFMathod_InitInspect()
        {
            DeclearInput("RecipeID", typeof(string), "");
        }
        protected override bool ActionGenuine(out string errorInfo)
        {
            string recipeID = GetMethodInputValue("RecipeID") as string;
            if(string.IsNullOrEmpty(recipeID))
            {
                errorInfo = "RecipeID未设置";
                return false;
            }
            //string innerError;
            if (!JFDLAFInspectionManager.Instance.InitInspectNode(recipeID,out errorInfo))
                return false;
            return true;
        }
    }
}
