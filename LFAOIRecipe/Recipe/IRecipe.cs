using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    interface IRecipe : IDisposable
    {
        /// <summary>
        /// 模块名字
        /// </summary>
        string DisplayName { get;}

        /// <summary>
        /// 界面
        /// </summary>
        object Content { get; }

        /// <summary>
        /// 产品目录
        /// </summary>
        string ProductDirctory { get; set; }

        /// <summary>
        /// 加载XML
        /// </summary>
        /// <param name="xmlPath"></param>
        void LoadXML(string xmlPath);
        void SaveXML();

        /// <summary>
        /// xml路径
        /// </summary>
        string XmlPath { get; }

        /// <summary>
        /// Recipe索引号
        /// </summary>
        int RecipeIndex { get; }

        /// <summary>
        /// Recipe名字
        /// </summary>
        string RecipeName { get; }
    }
}
