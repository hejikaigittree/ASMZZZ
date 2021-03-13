using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFHub;
using System.Windows.Forms;
using HalconDotNet;
using JFToolKits;

namespace DLAF
{
    public class MainStation : IJFMainStation
    {
        // SDN = System Data Name
        public static string SDN_CurrRecipeID = "DLAFCurrentRecipeID"; //当前加工的产品配方ID （系统数据项名称)
        public static string SDN_CurrLotID = "DLAFCurrentLotID"; //当前加工产品的批次号

        //SCN = System Config Name
        public static string SCN_CategotyProd = "Product"; //配方管理器中的 配方类别名称 （Product/Box）
        public static string SCN_CurrentRecipeID = "CurrentID";//当前加工的产品配方ID 在系统配置中的项名称 ,
        public static string SCN_CurrentLotID = "DLAFCurrentLotID"; // 将当前Lot保存到配置

        public static string SCN_OnLineReview = "在线复判"; 



        UcDLAFMainStation _uiPanel = new UcDLAFMainStation();


        /// <summary>
        /// 检查配方管理器是否存在
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        bool CheckRecipeMgr(out string errorInfo)
        {
            IJFRecipeManager rm = JFHubCenter.Instance.RecipeManager;
            if(null == rm)
            {
                errorInfo = "配方管理器未创建";
                return false;
            }

            if(rm.GetType()!= typeof(JFDLAFRecipeManager) &&
               !typeof(JFDLAFRecipeManager).IsAssignableFrom(rm.GetType()))
            {
                errorInfo = "当前配方管理器类型错误:" + rm.GetType().Name;
                return false;
            }

            if(!rm.IsInitOK)
            {
                errorInfo = "配方管理器初始化未完成，ErrorInfo:" + rm.GetInitErrorInfo();
                return false;
            }
            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 当前正在加工的产品配方名称
        /// </summary>
        public string CurrRecipeID
        {
            get
            {
                object ob;
                if (!JFHubCenter.Instance.DataPool.GetItemValue(SDN_CurrRecipeID, out ob))
                    return null;
                return ob as string;
            }
        }



        public bool SetCurrRecipeID(string recipeID,out string errorInfo)
        {
            if(string.IsNullOrEmpty(recipeID))
            {
                errorInfo = "产品ID为null/空字串";
                return false;
            }
            if (!CheckRecipeMgr(out errorInfo))
                return false;

            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            string[] allRecipeIDs = rm.AllRecipeIDsInCategoty(SCN_CategotyProd);
            if(null == allRecipeIDs || !allRecipeIDs.Contains(recipeID))
            {
                errorInfo = "产品ID:\"" + recipeID + "\"在配方管理器中不存在";
                return false;
            }
            if( WorkStatus == JFWorkStatus.Running || WorkStatus == JFWorkStatus.Interactiving || WorkStatus == JFWorkStatus.Pausing)
            {
                errorInfo = "正在运行，不能修改产品ID";
                return false;
            }

            IJFDataPool dp = JFHubCenter.Instance.DataPool;
            bool isOK = dp.SetItemValue(SDN_CurrRecipeID, recipeID);
            if (!isOK)
                errorInfo = "写入数据池失败";
            else
            {
                //将当前产品信息写入SystemCfg
                JFXCfg sysCfg = JFHubCenter.Instance.SystemCfg;
                if (sysCfg.ContainsItem(SCN_CurrentRecipeID))
                {
                    sysCfg.SetItemValue(SCN_CurrentRecipeID, recipeID); //将当前产品ID 加到配置项中
                    sysCfg.Save();
                }
                errorInfo = "Success";
            }
            return isOK;
        }



        public string CurrLotID
        {
            get
            {
                object ob;
                if (!JFHubCenter.Instance.DataPool.GetItemValue(SDN_CurrLotID, out ob))
                    return null;
                return ob as string;
            }
        }

        public bool SetCurrLotID(string lotID, out string errorInfo)
        {
            if (string.IsNullOrEmpty(lotID))
            {
                errorInfo = "LotID为null/空字串";
                return false;
            }
 

            if (WorkStatus == JFWorkStatus.Running || WorkStatus == JFWorkStatus.Interactiving || WorkStatus == JFWorkStatus.Pausing)
            {
                errorInfo = "正在运行，不能修改LotID";
                return false;
            }

            IJFDataPool dp = JFHubCenter.Instance.DataPool;
            bool isOK = dp.SetItemValue(SDN_CurrLotID, lotID);
            if (!isOK)
                errorInfo = "LotID写入数据池失败";
            else
            {
                //将当前产品信息写入SystemCfg
                JFXCfg sysCfg = JFHubCenter.Instance.SystemCfg;
                if (sysCfg.ContainsItem(SCN_CurrentLotID))
                {
                    sysCfg.SetItemValue(SCN_CurrentLotID, lotID); //将当前LotID 保存到配置项中
                    sysCfg.Save();
                }

                


                errorInfo = "Success";
            }
            return isOK;
        }


        /// <summary>
        /// 人机交互面板，用于在主窗口中显示
        /// </summary>
        /// <returns></returns>
        public Control UIPanel { get { return _uiPanel; } }

        /// <summary>
        /// 设备简介面板,用于显示设备简介，包含图片和文字信息等
        /// </summary>
        public Control BriefPanel { get; protected set; }

        /// <summary>
        /// 获取当前工作状态
        /// </summary>
        public virtual JFWorkStatus WorkStatus { get; protected set; }

        public virtual bool IsAlarming { get; protected set; }


        public MainStation()
        {

            _uiPanel.SetMainStation(this);

            ///////简介面板，只是一个简单的演示
            //Panel briefPanel = new Panel();
            //Label lb = new Label();
            //lb.Text = "工站简介面板示例";
            //briefPanel.Controls.Add(lb);
            //BriefPanel = briefPanel;

            WorkStatus = JFWorkStatus.UnStart;
            IsAlarming = false;

            InitStationParams();
        }

        private void InitStationParams()
        {
            //JFHubCenter.Instance.
            JFHubCenter.Instance.DataPool.RegistItem(SDN_CurrRecipeID, typeof(string), "");
            JFHubCenter.Instance.DataPool.RegistItem(SDN_CurrLotID, typeof(string), "");
            //JFHubCenter.Instance.DataPool.RegistItem(SDN_CurrLotID, typeof(string), "");
            if(!JFHubCenter.Instance.SystemCfg.ContainsItem(SCN_OnLineReview))
            {
                JFHubCenter.Instance.SystemCfg.AddItem(SCN_OnLineReview, false, "运行参数");
                JFHubCenter.Instance.SystemCfg.Save();
            }
            
            
        }


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
            if (IsAlarming)
            {
                errorInfo = "当前处于报警状态";
                return false;
            }
            if (IsStationRunning(WorkStatus))
            {
                errorInfo = "Success";
                return true;
            }

            JFDLAFRecipeManager rm = JFHubCenter.Instance.RecipeManager as JFDLAFRecipeManager;
            if(null == rm)
            {
                errorInfo = "配方管理器未创建！";
                return false;
            }
            if(!rm.IsInitOK)
            {
                errorInfo = "配方管理器初始化未完成,ErrorInfo:" + rm.GetInitErrorInfo();
                return false;
            }



            JFStationManager stationMgr = JFHubCenter.Instance.StationMgr;


            string[] allEnableStationNames = stationMgr.AllEnabledStationNames();
            if (null == allEnableStationNames || 0 == allEnableStationNames.Length)
            {
                errorInfo = "不存在使能的工站";
                return false;
            }

            foreach (string stationName in allEnableStationNames) // 先检查有没有正在运行的工站
            {
                IJFStation station = stationMgr.GetStation(stationName);
                if (IsStationRunning(station.CurrWorkStatus))
                {
                    errorInfo = "启动失败，工站:" + station.Name + " 当前状态:" + station.CurrWorkStatus.ToString();
                    return false;
                }
            }

            ///检查当前RecipeID 和 LotID 
            if(string.IsNullOrEmpty(CurrRecipeID))
            {
                errorInfo = "启动失败:当前产品ID未设置";
                return false;
            }

            string[] allRecipeIDs = rm.AllRecipeIDsInCategoty(SCN_CategotyProd);
            if(null == allRecipeIDs || !allRecipeIDs.Contains(CurrRecipeID))
            {
                errorInfo = "启动失败，当前产品ID:" + CurrRecipeID + " 在配方管理器中不存在";
                return false;
            }


            if(string.IsNullOrEmpty(CurrLotID))
            {
                errorInfo = "启动失败:当前批次号未设置！";
                return false;
            }







            int failedIndex = -1; //启动失败的工站号
            foreach (string stationName in allEnableStationNames)
            {
                IJFStation station = stationMgr.GetStation(stationName);
                JFWorkCmdResult ret = station.Start();
                if (ret != JFWorkCmdResult.Success)
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
                    if (ret != JFWorkCmdResult.Success)
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
            if (WorkStatus != JFWorkStatus.Running)
            {
                errorInfo = "设备当前状态:" + WorkStatus.ToString();
                return false;
            }
            if (WorkStatus == JFWorkStatus.Pausing)
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

            foreach (string sn in allEnableStationNames)
            {
                IJFStation station = stationMgr.GetStation(sn);
                JFWorkCmdResult ret = station.Pause(-1);
                if (ret != JFWorkCmdResult.Success)
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
            if (WorkStatus != JFWorkStatus.Pausing)
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
        public virtual bool EndBatch(out string errorInfo)
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
            if (station.GetType().Name.Contains("DLAFDetectStation"))
                _uiPanel.OnWorkStatusChanged(currWorkStatus);

            ///继续添加其他工站发过来的消息
                
        }
        //void HandleStationMsg(IJFStation station,object msg )

        /// <summary>
        ///  处理工站的业务状态发生改变
        /// </summary>
        /// <param name="station"></param>
        /// <param name="currCustomStatus"></param>
        public virtual void OnStationCustomStatusChanged(IJFStation station, int currCustomStatus)
        {
            if (station.GetType().Name.Contains("DLAFDetectStation"))
            {
                string custonStatusTxt = station.GetCustomStatusName(currCustomStatus);
                _uiPanel.OnCustomStatusChanged(custonStatusTxt);
            }
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
            if (station.GetType().Name.Contains("DLAFDetectStation"))
            {
                _uiPanel.OnProductFinished(passCount, passIDs, ngCount, ngIDs, ngInfo);
            }
        }

        /// <summary>
        /// 处理工站发来的其他定制化的消息
        /// </summary>
        /// <param name="station"></param>
        /// <param name="msg"></param>
        public virtual void OnStationCustomizeMsg(IJFStation station, string msgCategory, object[] msgParams)
        {
            if (station.GetType().Name.Contains("DLAFDetectStation"))
            {
                _uiPanel.OnCustomizeMsg(msgCategory, msgParams);
            }
        }

        public void OnStationTxtMsg(IJFStation station, string msgInfo)
        {
            if (station.GetType().Name.Contains("DLAFDetectStation"))
            {
                _uiPanel.OnTxtMsg(msgInfo);
            }
        }
    }
}
