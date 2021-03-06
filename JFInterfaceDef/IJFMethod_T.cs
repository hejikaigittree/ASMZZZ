using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 支持CmdWork线程内部操作（暂停/恢复/退出）的算子接口
    /// </summary>
    public interface IJFMethod_T:IJFMethod
    {
        /// <summary>
        /// 暂停执行
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复执行
        /// </summary>
        void Resume();

        /// <summary>
        /// 退出
        /// </summary>
        void Exit();

        
    }
}
