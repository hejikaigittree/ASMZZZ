using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 接收工站消息的接口定义
    /// </summary>
    public interface IJFStationMsgReceiver
    {   
        void OnWorkStatusChanged(JFWorkStatus currWorkStatus); //工作状态变化

        void OnCustomStatusChanged(int currCustomStatus); //业务逻辑变化

        void OnTxtMsg(string txt); //接受一条文本消息

        void OnProductFinished(int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo); //产品加工完成

        void OnCustomizeMsg(string msgCategory, object[] msgParams); //其他自定义消息
    }
}
