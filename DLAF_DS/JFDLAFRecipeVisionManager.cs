using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using LFAOIRecipe;
using DLAF;
using JFHub;

namespace DLAF_DS
{
    public class JFDLAFRecipeVisionManager
    {
        JFDLAFRecipeVisionManager()
        {

        }

        private static readonly Lazy<JFDLAFRecipeVisionManager> lazy = new Lazy<JFDLAFRecipeVisionManager>(() => new JFDLAFRecipeVisionManager());
        public static JFDLAFRecipeVisionManager Instance { get { return lazy.Value; } }

        public bool InitInspectNode(string recipeID,out string errorInfo)
        {
            if(string.IsNullOrEmpty(recipeID))
            {
                errorInfo = "参数recipeID 为空字串";
                return false;
            }
            Inspect_Node node;
            if (!_dctInspect.ContainsKey(recipeID))
            {
                string recipePath = "";
                string modelPath = "";
                JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
                if(rm == null)
                {
                    errorInfo = "配方管理器为空!";
                    return false;
                }

                if(!rm.IsInitOK)
                {
                    errorInfo = "配方管理器未初始化:" + rm.GetInitErrorInfo();
                    return false;
                }

                string rmPath = rm.GetInitParamValue("配方保存路径") as string;

                recipePath = rmPath + "\\" + recipeID + "\\Recipe\\";
                modelPath = rmPath + "\\" + recipeID + "\\Models\\";


                node = new Inspect_Node(null, modelPath, recipePath);
                _dctInspect.Add(recipeID, node);
            }

            node = _dctInspect[recipeID];

            if (!node.InitInspectParam(out errorInfo))
                return false;
            errorInfo = "Success";
            return true;

        }

        public void Clear()
        {
            _dctInspect.Clear();
        }

        public Inspect_Node GetInspectNode(string recipeID)
        {
            if (string.IsNullOrEmpty(recipeID))
                return null;
            if (!_dctInspect.ContainsKey(recipeID))
                return null;
            return _dctInspect[recipeID];
        }

        Dictionary<string, Inspect_Node> _dctInspect = new Dictionary<string, Inspect_Node>();

    }
}
