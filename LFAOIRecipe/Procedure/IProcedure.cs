using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    interface IProcedure : IDisposable
    {
        /// <summary>
        /// 流程模块名字
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// 界面
        /// </summary>
        object Content { get;}

        /// <summary>
        /// 判断是否完成
        /// </summary>
        bool CheckCompleted();

        /// <summary>
        /// 初始化
        /// </summary>
        void Initial();
    }
}
