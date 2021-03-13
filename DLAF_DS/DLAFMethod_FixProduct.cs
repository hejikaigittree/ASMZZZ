using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFVision;
using JFHub;
using DLAF;

namespace DLAF_DS
{
    /// <summary>
    /// DLAF产品定位算子
    /// </summary>
    [JFDisplayName("DLAF产品定位")]
    [JFVersion("1.0.0.0")]
    public class DLAFMethod_FixProduct : JFMethodBase
    {
        public DLAFMethod_FixProduct()
        {
            DeclearInput("RecipeID", typeof(string), "");
            DeclearInput("MarkImage1", typeof(IJFImage), null);
            DeclearInput("MarkSanpX1", typeof(double), (double)0);
            DeclearInput("MarkSanpY1", typeof(double), (double)0);
            DeclearInput("MarkImage2", typeof(IJFImage), null);
            DeclearInput("MarkSanpX2", typeof(double), (double)0);
            DeclearInput("MarkSanpY2", typeof(double), (double)0);
            DeclearInput("定位结果接收者", typeof(IDLAFProductFixReceiver), null);
        }
        protected override bool ActionGenuine(out string errorInfo)
        {
            string recipeID = GetMethodInputValue("RecipeID") as string;
            if(string.IsNullOrEmpty(recipeID))
            {
                errorInfo = "输入参数RecipeID 为空字串";
                return false;
            }

            IJFImage markImg1 = GetMethodInputValue("MarkImage1") as IJFImage;
            if(null == markImg1)
            {
                errorInfo = "MarkImage1 is null";
                return false;
            }

            IJFImage markImg2 = GetMethodInputValue("MarkImage2") as IJFImage;
            if (null == markImg1)
            {
                errorInfo = "MarkImage1 is null";
                return false;
            }

            IDLAFProductFixReceiver rcver = GetMethodInputValue("定位结果接收者") as IDLAFProductFixReceiver;
            if(null == rcver)
            {
                errorInfo = "定位结果接收者 未设置";
                return false;
            }
            double markSnapX1 = Convert.ToDouble(GetMethodInputValue("MarkSanpX1"));
            double markSnapY1 = Convert.ToDouble(GetMethodInputValue("MarkSanpY1"));
            double markSnapX2 = Convert.ToDouble(GetMethodInputValue("MarkSanpX2"));
            double markSnapY2 = Convert.ToDouble(GetMethodInputValue("MarkSanpY2"));

            double[] icCenterX = null;
            double[] icCenterY = null;
            double[] fovOffsetX = null;
            double[] fovOffsetY = null;
            int fixErrorCode = -1;
            string fixErrorInfo = "软件功能未实现";
            //////////////////////////////////////////////添加定位算法流程
            
            IJFRecipeManager irm = JFHubCenter.Instance.RecipeManager;
            if(null == irm)
            {
                errorInfo = "配方管理器未设置";
                rcver.PFErrorInfo = errorInfo;
                rcver.PFErrorCode = -1;

                return false;
            } 

            if(!irm.IsInitOK)
            {
                errorInfo = "配方管理器未初始化 :" + irm.GetInitErrorInfo();
                rcver.PFErrorInfo = errorInfo;
                rcver.PFErrorCode = -1;
                return false;
            }
            JFDLAFRecipeManager rm = irm as JFDLAFRecipeManager;
            if(null == rm)
            {
                errorInfo = "配方管理器类型错误 :" + irm.GetType().Name;
                rcver.PFErrorInfo = errorInfo;
                rcver.PFErrorCode = -1;
                return false;
            }

            JFDLAFProductRecipe recipe = rm.GetRecipe("Product", recipeID) as JFDLAFProductRecipe;
            if(null == recipe)
            {
                errorInfo = "RecipeID =\""  + recipeID + "\" 在配方管理器中不存在";
                rcver.PFErrorInfo = errorInfo;
                rcver.PFErrorCode = -1;
                return false;
            }

            ///使用原始拍照点位
            icCenterX = new double[recipe.ICCount];
            icCenterY = new double[recipe.ICCount];
            for (int i = 0; i < recipe.RowCount; i++)
                for (int j = 0; j < recipe.ColCount; j++)
                    recipe.GetICSnapCenter(i, j, out icCenterX[i * recipe.ColCount + j], out icCenterY[i * recipe.ColCount + j]);

            ///使用原始Fov偏移量
            fovOffsetX = new double[recipe.FovCount];
            fovOffsetY = new double[recipe.FovCount];
            for (int i = 0; i < recipe.FovCount; i++)
                recipe.GetFovOffset(recipe.FovNames()[i], out fovOffsetX[i], out fovOffsetY[i]);
            fixErrorCode = 0;


            //当前为演示代码，返回模板图像拍照位置
            rcver.PFRecipeID = recipeID;
            rcver.PFErrorInfo = fixErrorInfo;
            rcver.PFICCenterX = icCenterX;
            rcver.PFICCenterY = icCenterY;
            rcver.PFFovOffsetX = fovOffsetX;
            rcver.PFFovOffsetY = fovOffsetY;
            rcver.PFErrorCode = fixErrorCode;


            errorInfo = "Success";
            return true;

        }

    }
}
