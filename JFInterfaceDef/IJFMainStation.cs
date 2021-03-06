using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFInterfaceDef;

namespace JFHub
{
    public enum LogMode
    {
        None,  //忽略
        Show = 1,//只是显示
        Record =2,//只是记录
        ShowRecord = Show| Record,//显示并且记录
    }

    /// <summary>
    /// 主控工站接口，每个设备程序中只能有一个实例
    /// </summary>
    public interface IJFMainStation
    {
        /// <summary>
        /// 人机交互面板，用于在主窗口中显示
        /// </summary>
        /// <returns></returns>
        Control UIPanel { get; }


        /// <summary>
        /// 设备简介面板
        /// </summary>
        Control BriefPanel { get; }

        /// <summary>
        /// 获取当前工作状态
        /// </summary>
        JFWorkStatus WorkStatus { get; }

        bool IsAlarming { get; }

        /// <summary>
        /// 重置（消除）报警信号
        /// </summary>
        bool ClearAlarming( out string errorInfo);

        /// <summary>
        /// 获取报警信息
        /// </summary>
        /// <returns></returns>
        string GetAlarmInfo();

        bool Start(out string errorInfo);//开始运行

        /// <summary>停止运行</summary>
        bool Stop(out string errorInfo);

        /// <summary>暂停</summary>
        bool Pause(out string errorInfo);

        /// <summary>从暂停中恢复运行</summary>
        bool Resume(out string errorInfo);

        /// <summary>
        /// 结批，完成当前工作后退出任务
        /// </summary>
        bool EndBatch(out string errorInfo);

        /// <summary>
        /// 所有工站复位
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        bool Reset(out string errorInfo);


        /// <summary>
        /// 处理工站状态改变
        /// </summary>
        /// <param name="station"></param>
        /// <param name="currWorkStatus"></param>
        void OnStationWorkStatusChanged(IJFStation station, JFWorkStatus currWorkStatus);
        //void HandleStationMsg(IJFStation station,object msg )

        /// <summary>
        ///  处理工站的业务状态发生改变
        /// </summary>
        /// <param name="station"></param>
        /// <param name="currCustomStatus"></param>
        void OnStationCustomStatusChanged(IJFStation station, int currCustomStatus);

        /// <summary>
        /// 产品加工完成消息
        /// </summary>
        /// <param name="station">消息发送者</param>
        /// <param name="PassCount">本次生产完成的成品数量</param>
        /// <param name="NGCount">本次生产的次品数量</param>
        /// <param name="NGInfo">次品信息</param>
        void OnStationProductFinished(IJFStation station, int passCount, string[] passIDs,int ngCount,string[] ngIDs,string[] ngInfo);

        /// <summary>
        /// 处理工站发来的其他定制化的消息
        /// </summary>
        /// <param name="station"></param>
        /// <param name="msg"></param>
        void OnStationCustomizeMsg(IJFStation station, string msgCategory,object[] msgParam);

        /// <summary>
        /// 处理工站文本消息
        /// </summary>
        /// <param name="station"></param>
        /// <param name="msgInfo"></param>
        void OnStationTxtMsg(IJFStation station, string msgInfo);

    }
}
