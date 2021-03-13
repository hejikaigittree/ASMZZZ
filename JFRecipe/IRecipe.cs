using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFRecipe
{
    /// <summary>
    ///  产品/配方接口类
    /// </summary>
    public interface IRecipe
    {
        /// <summary>
        /// 配方类别 比如：托盘 产品 等
        /// </summary>
        string Categoty { get; }

        /// <summary>
        /// 配方型号
        /// </summary>
        string Model { get; }

        void Load(string filePath);

        void Save(string filePath);
    }
}
