using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    public interface IJFRecipeManager:IJFInitializable
    {

    

        /// <summary>
        /// 载入(/重新载入)所有产品,配方
        /// </summary>
        bool Load();

        void Save();

        /// <summary>
        /// 获取所有产品/配方 类别
        /// </summary>
        /// <returns></returns>
        string[] AllCategoties();

        ///// <summary>
        ///// 添加一个产品类别
        ///// </summary>
        ///// <param name="recipeCategoty"></param>
        //void AddRecipeCategoty(string categoty);

        /// <summary>
        /// 移除一个类别
        /// </summary>
        /// <param name="recipeCategoty"></param>
        void RemoveCategoty(string categoty);

        /// <summary>
        /// 获取指定类别下的所有产品/配方 ID
        /// </summary>
        /// <returns></returns>
        string[] AllRecipeIDsInCategoty(string categoty);

        /// <summary>
        /// 获取一个产品/配方
        /// </summary>
        /// <param name="categoty"></param>
        /// <param name="recipeID"></param>
        /// <returns></returns>
        IJFRecipe GetRecipe(string categoty, string recipeID);

        /// <summary>
        /// 添加一个产品/配方
        /// </summary>
        /// <param name="categoty"></param>
        /// <param name="recipeID"></param>
        /// <param name="recipe"></param>
        bool AddRecipe(string categoty, string recipeID,IJFRecipe recipe = null);

        /// <summary>
        /// 移出一个产品配方
        /// </summary>
        /// <param name="categoty"></param>
        /// <param name="recipeID"></param>
        /// <returns></returns>
        IJFRecipe RemoveRecipe(string categoty, string recipeID);




    }
}
