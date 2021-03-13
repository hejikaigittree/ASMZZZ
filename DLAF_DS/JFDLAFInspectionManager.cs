using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

using DLAF;
using JFHub;
using LFAOIRecipe;

namespace DLAF_DS
{
    public class JFDLAFInspectionManager
    {
        static string CategoyProduct = "Product";
        JFDLAFInspectionManager()
        {

        }

        private static readonly Lazy<JFDLAFInspectionManager> lazy = new Lazy<JFDLAFInspectionManager>(() => new JFDLAFInspectionManager());
        public static JFDLAFInspectionManager Instance { get { return lazy.Value; } }

        public bool InitInspectNode(string recipeID,out string errorInfo)
        {
            if(string.IsNullOrEmpty(recipeID))
            {
                errorInfo = "参数recipeID 为空字串";
                return false;
            }

            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            if (rm == null)
            {
                errorInfo = "配方管理器为空!";
                return false;
            }

            if (!rm.IsInitOK)
            {
                errorInfo = "配方管理器未初始化:" + rm.GetInitErrorInfo();
                return false;
            }

            string[] allRecipeIds = rm.AllRecipeIDsInCategoty(CategoyProduct);
            if(null == allRecipeIds || 0 == allRecipeIds.Length)
            {
                errorInfo = "配方管理器中不存在任何RecipeID";
                return false;
            }

            if(!allRecipeIds.Contains(recipeID))
            {
                errorInfo = "配方管理器中不存在RecipeID = " + recipeID;
                return false;
            }

            Dictionary<string, InspectNode> dctFovInspections = null;
            if (!_dctRecipeInspections.ContainsKey(recipeID)) //当前
            {
                dctFovInspections = new Dictionary<string, InspectNode>();
                _dctRecipeInspections.Add(recipeID, dctFovInspections);


                string rmPath = rm.GetInitParamValue("配方保存路径") as string;
               

                JFDLAFProductRecipe recipe = rm.GetRecipe(CategoyProduct, recipeID) as JFDLAFProductRecipe;
                string[] fovNames = recipe.FovNames();
                foreach(string fovName in fovNames)
                {
                    string recipePath = rmPath + "\\" + recipeID + "\\" + fovName+"\\Recipe\\";
                    string modelPath = rmPath + "\\" + recipeID + "\\" + fovName + "\\Models\\";

                    InspectNode inspectNode = new InspectNode(modelPath, recipePath);
                    dctFovInspections.Add(fovName, inspectNode);
                }

            }
            string err;
            dctFovInspections = _dctRecipeInspections[recipeID];
            foreach(KeyValuePair<string, InspectNode> kv in dctFovInspections)
            {
                if(!kv.Value.InitInspectParam(out err))
                {
                    errorInfo = "Fov = " + kv.Key + " 初始化失败:" + err;
                    return false;
                }
            }
            errorInfo = "Success";
            return true;

        }

        public void Clear()
        {
            _dctRecipeInspections.Clear();
        }

        public InspectNode GetInspectNode(string recipeID,string fovName)
        {
            if (string.IsNullOrEmpty(recipeID))
                return null;
            if (string.IsNullOrEmpty(fovName))
                return null;
            if (!_dctRecipeInspections.ContainsKey(recipeID))
                return null;
            if (!_dctRecipeInspections[recipeID].ContainsKey(fovName))
                return null;

            return _dctRecipeInspections[recipeID][fovName];
        }

        Dictionary<string, Dictionary<string, InspectNode>> _dctRecipeInspections = new Dictionary<string, Dictionary<string, InspectNode>>();

    }
}
