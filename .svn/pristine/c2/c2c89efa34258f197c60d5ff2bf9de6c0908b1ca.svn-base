using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFInterfaceDef
{
    //public interface IJFUIProvider:IJFErrorCode2Txt
    //{
    //    /// <summary>获取一个（新创建的）UI控件</summary>
    //    int GetRealtimeUI(out Control panel);

    //    /// <summary>显示一个对话框窗口</summary>
    //    int ShowCfgDialog();

    //}

    public class JFRealtimeUI: UserControl
    {
        public JFRealtimeUI():base()
        {

        }
        /// <summary>更新数据源状态到界面上</summary>
        public virtual void UpdateSrc2UI() { } 
    }

    /// <summary>
    /// 实时界面接口
    /// 提供 实时信息显示/功能调试 等界面
    /// </summary>
    public interface IJFRealtimeUIProvider
    {
        JFRealtimeUI GetRealtimeUI();
    }

    /// <summary>
    /// 参数配置界面接口
    /// </summary>
    public interface IJFConfigUIProvider
    {
        void ShowCfgDialog();
    }
}
