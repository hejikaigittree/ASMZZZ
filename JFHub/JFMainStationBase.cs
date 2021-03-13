using JFInterfaceDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JFHub
{
    public class JFMainStationBase:IJFMainStation
    {
        public JFMainStationBase()
        {
            ///主工站UI面板，这里只是一个简单的演示
            UcMainStationPanelBase mainStationPanel = new UcMainStationPanelBase();
            mainStationPanel.SetMainStation(this);
            UIPanel = mainStationPanel;

            /////简介面板，只是一个简单的演示
            Panel briefPanel = new Panel();
            Label lb = new Label();
            lb.Text = "工站简介面板示例";
            briefPanel.Controls.Add(lb);
            BriefPanel = briefPanel;
            


            WorkStatus = JFWorkStatus.UnStart;
            IsAlarming = false;


        }

        /// <summary>
        /// 人机交互面板，用于在主窗口中显示
        /// </summary>
        /// <returns></returns>
        public Control UIPanel { get; protected set; }



        /// <summary>
        /// 设备简介面板,用于显示设备简介，包含图片和文字信息等
        /// </summary>
        public Control BriefPanel { get; protected set; }



        /// <summary>
        /// 获取当前工作状态
        /// </summary>
        public virtual JFWorkStatus WorkStatus { get; protected set; }

        public virtual bool IsAlarming { get; protected set; }


        public virtual string GetAlarmInfo()
        {
            return "No-Alarm";
        }

        /// <summary>
        /// 重置（消除）报警信号
        /// </summary>
        public virtual bool ClearAlarming(out string errorInfo)
        {
            errorInfo = "Success";
            IsAlarming = false;
            WorkStatus = JFWorkStatus.UnStart;
            return true;
            
        }

        protected bool IsStationRunning(JFWorkStatus ws)
        {
            return ws == JFWorkStatus.Running || ws == JFWorkStatus.Pausing || ws == JFWorkStatus.Interactiving;
        }

        public virtual bool Start(out string errorInfo)//开始运行
        {
            errorInfo = "Unknown Error";
            if(IsAlarming)
            {
                errorInfo = "当前处于报警状态";
                return false;
            }
            if (IsStationRunning(WorkStatus))
            {
                errorInfo = "Success";
                return true;
            }
            JFStationManager stationMgr = JFHubCenter.Instance.StationMgr;


            string[] allEnableStationNames = stationMgr.AllEnabledStationNames();
            if(null == allEnableStationNames || 0 == allEnableStationNames.Length)
            {
                errorInfo = "不存在使能的工站";
                return false;
            }

            foreach(string stationName in allEnableStationNames) // 先检查有没有正在运行的工站
            {
                IJFStation station = stationMgr.GetStation(stationName);
                if(IsStationRunning(station.CurrWorkStatus))
                {
                    errorInfo = "启动失败，工站:" + station.Name + " 当前状态:" + station.CurrWorkStatus.ToString();
                    return false;
                }
            }

            int failedIndex = -1; //启动失败的工站号
            foreach(string stationName in allEnableStationNames)
            {
                IJFStation station = stationMgr.GetStation(stationName);
                JFWorkCmdResult ret = station.Start();
                if(ret != JFWorkCmdResult.Success)
                {
                    errorInfo = "工站:" + station.Name + " 启动失败,Error:" + ret.ToString();
                    break;
                }
            }

            if (failedIndex > -1)
            {
                for (int i = 0; i < failedIndex + 1; i++)
                {
                    IJFStation station = stationMgr.GetStation(allEnableStationNames[i]);
                    if (JFWorkCmdResult.Success != station.Stop(100))
                        station.Abort();
                }
                return false;
            }
            WorkStatus = JFWorkStatus.Running;
            errorInfo = "Success";
            return true;
        }

        /// <summary>停止运行</summary>
        public virtual bool Stop(out string errorInfo)
        {
            errorInfo = "Unknown Error";
            //if (!IsStationRunning(WorkStatus))
            //{
            //    errorInfo = "Success";
            //    return true;
            //}
            JFStationManager stationMgr = JFHubCenter.Instance.StationMgr;


            string[] allEnableStationNames = stationMgr.AllEnabledStationNames();
            if (null == allEnableStationNames || 0 == allEnableStationNames.Length)
            {
                errorInfo = "Success";
                return true;
            }

            foreach (string stationName in allEnableStationNames) // 先检查有没有正在运行的工站
            {
                IJFStation station = stationMgr.GetStation(stationName);
                if (IsStationRunning(station.CurrWorkStatus))
                {
                    JFWorkCmdResult ret = station.Stop(1000);
                    if(ret != JFWorkCmdResult.Success)
                        station.Abort();
                }
            }
          
            WorkStatus = JFWorkStatus.CommandExit;
            errorInfo = "Success";
            return true;
        }

        /// <summary>暂停</summary>
        public virtual bool Pause(out string errorInfo)
        {
            errorInfo = "Unknown Error";
            if(WorkStatus != JFWorkStatus.Running)
            {
                errorInfo = "设备当前状态:" + WorkStatus.ToString();
                return false;
            }
            if(WorkStatus == JFWorkStatus.Pausing)
            {
                errorInfo = "Success";
                return true;
            }
            JFStationManager stationMgr = JFHubCenter.Instance.StationMgr;
            string[] allEnableStationNames = stationMgr.AllEnabledStationNames();
            if (null == allEnableStationNames || 0 == allEnableStationNames.Length)
            {
                errorInfo = "无使能工站";
                return false;
            }

            foreach(string sn in allEnableStationNames)
            {
                IJFStation station = stationMgr.GetStation(sn);
                JFWorkCmdResult ret =  station.Pause(-1);
                if(ret != JFWorkCmdResult.Success)
                {
                    errorInfo = "工站:" + station.Name + " 暂停失败:" + ret.ToString();
                    return false;
                }
            }

            WorkStatus = JFWorkStatus.Pausing;
            errorInfo = "Success";
            return true;
        }

        /// <summary>从暂停中恢复运行</summary>
        public virtual bool Resume(out string errorInfo)
        {
            errorInfo = "Unknown Error";
            if (WorkStatus == JFWorkStatus.Running)
            {
                errorInfo = "当前正在运行！恢复运行指令将被忽略";
                return true;
            }
            if(WorkStatus != JFWorkStatus.Pausing)
            {
                errorInfo = "当前状态 = " + WorkStatus + ",不能响应恢复运行指令";
                return false;
            }

            JFStationManager stationMgr = JFHubCenter.Instance.StationMgr;

            string[] allEnableStationNames = stationMgr.AllEnabledStationNames();
            if (null == allEnableStationNames || 0 == allEnableStationNames.Length)
            {
                errorInfo = "无使能工站";
                return false;
            }

            foreach (string sn in allEnableStationNames)
            {
                IJFStation station = stationMgr.GetStation(sn);
                JFWorkCmdResult ret = station.Resume(1000);
                if (ret != JFWorkCmdResult.Success)
                {
                    errorInfo = "工站:" + station.Name + "恢复运行失败:" + ret.ToString();
                    return false;
                }
            }

            errorInfo = "Success";
            WorkStatus = JFWorkStatus.Running;
            return true;
        }

        /// <summary>
        /// 结批，完成当前工作后退出任务 
        /// </summary>
        public virtual  bool EndBatch(out string errorInfo)
        {
            errorInfo = "Unknown Error";
            return false;
        }

        public virtual bool Reset(out string errorInfo)
        {
            errorInfo = "Unknown Error";
            //先检查所有子工站是否满足归零条件

            return false;
        }


        /// <summary>
        /// 处理工站状态改变
        /// </summary>
        /// <param name="station"></param>
        /// <param name="currWorkStatus"></param>
        public virtual void OnStationWorkStatusChanged(IJFStation station, JFWorkStatus currWorkStatus)
        {

        }
        //void HandleStationMsg(IJFStation station,object msg )

        /// <summary>
        ///  处理工站的业务状态发生改变
        /// </summary>
        /// <param name="station"></param>
        /// <param name="currCustomStatus"></param>
        public virtual void OnStationCustomStatusChanged(IJFStation station, int currCustomStatus)
        {

        }

        /// <summary>
        /// 产品加工完成消息
        /// </summary>
        /// <param name="station">消息发送者</param>
        /// <param name="PassCount">本次生产完成的成品数量</param>
        /// <param name="NGCount">本次生产的次品数量</param>
        /// <param name="NGInfo">次品信息</param>
        public virtual void OnStationProductFinished(IJFStation station, int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo)
        {

        }

        /// <summary>
        /// 处理工站发来的其他定制化的消息
        /// </summary>
        /// <param name="station"></param>
        /// <param name="msg"></param>
        public virtual void OnStationCustomizeMsg(IJFStation station, string msgCategory, object[] msgParams)
        {

        }

        public virtual void OnStationTxtMsg(IJFStation station, string msgInfo)
        {
            
        }
    }
}
