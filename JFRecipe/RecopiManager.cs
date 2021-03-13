using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFRecipe
{
    /// <summary>
    /// 配方管理类
    /// </summary>
    public class RecopiManager
    {
        RecopiManager()
        {

        }

        private static readonly Lazy<RecopiManager> lazy = new Lazy<RecopiManager>(() => new RecopiManager());
        public static RecopiManager Instance { get { return lazy.Value; } }

    }
}
