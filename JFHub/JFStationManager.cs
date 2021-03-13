using JFInterfaceDef;
using JFToolKits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JFLog;
using System.Threading;

namespace JFHub
{
    /// <summary>
    /// Station继承于Initor接口，可配置的Station的管理功能放在InitorManager中
    /// </summary>
    public class JFStationManager
    {
        /// <summary>
        /// 工站运行状态改变事件
        /// Run/Stop/Pause
        /// </summary>
        public event WorkStatusChange EventStationWorkStatusChanged;
        /// <summary>
        /// 工站自定义状态改变
        /// 正在上料/正在加工 等等...
        /// </summary>
        public event CustomStatusChange EventStationCustomStatusChanged;
        /// <summary>
        /// 工站产品加工完成事件
        /// </summary>
        public event DelegateStationProductFinished EventStationProductFinished;

        /// <summary>
        /// 工站自定义消息
        /// </summary>
        public event DelegateStationCustomizeMsg EventStationCustomizeMsg;

        public event WorkMsgInfo EventStationTxtMsg;

        



        internal JFStationManager(string cfgPath)
        {
            _cfg.Load(cfgPath, true);
            if (!_cfg.ContainsItem("StationEnabled"))
            {
                _dictStationEnabled = new JFXmlDictionary<string, bool>();
                _cfg.AddItem("StationEnabled", _dictStationEnabled);
            }
            else
                _dictStationEnabled = _cfg.GetItemValue("StationEnabled") as JFXmlDictionary<string, bool>;

            List<string> existedStationNames = _initorStationNames();
            List<string> stationNamesInCfg = _dictStationEnabled.Keys.ToList();
            if (null == existedStationNames)
                _dictStationEnabled.Clear();
            else
            {
                foreach (string cfgName in stationNamesInCfg) //去除多余的项
                    if (!existedStationNames.Contains(cfgName))
                        _dictStationEnabled.Remove(cfgName);
                foreach (string exsitedName in existedStationNames) //添加缺少的项
                    if (!_dictStationEnabled.ContainsKey(exsitedName))
                    {
                        _dictStationEnabled.Add(exsitedName, true);
                        //SetStationEnabled(exsitedName, true);
                       

                    }
            }

            ///添加默认的消息回调
            foreach(string stationName in _dictStationEnabled.Keys)
            {
                IJFStation station = GetStation(stationName);
                station.WorkStatusChanged += StationWorkStatusChanged;
                station.CustomStatusChanged += StationCustomStatusChanged;
                if (station is JFCmdWorkBase)
                    (station as JFCmdWorkBase).WorkMsg2Outter += StationTxtMsg;
                if (station is JFStationBase)
                {
                    (station as JFStationBase).EventCustomizeMsg += StationCustomizeMsg;
                    (station as JFStationBase).EventProductFinished += StationProductFinished;
                }
            }
            
            _cfg.Save();
            DeclearedStationNames = new List<string>();
            StartShowStationLog();
        }
        JFXCfg _cfg = new JFXCfg();
        JFXmlDictionary<string, bool> _dictStationEnabled = null;

        //List<IJFStation> _lstStations = new List<IJFStation>();

        IJFMainStation _mainStation = null; //主工站
        /// <summary>
        /// 为应用程序注册一个主工站，在Application.Run()运行之前调用
        /// </summary>
        /// <param name=""></param>
        public void DeclearMainStation(IJFMainStation mainStation)
        {
            if (_mainStation != null)
                throw new Exception("Main Station is already Registed!");
            if (null == mainStation)
                throw new ArgumentNullException("Main Station is null object");
            _mainStation = mainStation;
        }

        public IJFMainStation MainStation{get{ return _mainStation; } }

        internal List<string> DeclearedStationNames { get; private set; }

        /// <summary>
        /// 注册一个工站（不可删除）,
        /// 在Application.Run()运行之前调用
        /// </summary>
        /// <param name="station"></param>
        public void DeclearStation(IJFStation station)
        {
            if (station == null)
                throw new ArgumentNullException("StationManager.DeclearStation(IJFStation station) failed by station = null");
            //if (_lstDeclearStations.Contains(station))
            //    throw new ArgumentException("StationManager.DeclearStation(IJFStation station) failed by station is already decleared!");
            string name = station.Name;
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("StationManager.DeclearStation(IJFStation station) failed by station's Name null or empty");
            if (_initorStationNames().Contains(name))
            {
                IJFInitializable existedStation = JFHubCenter.Instance.InitorManager.GetInitor(name);
                if(existedStation.GetType() != station.GetType())
                {
                    throw new Exception("StationManager.DeclearStation(IJFStation station) failed by:Exist a station with same name and type unmatched: Decleared Type = " + station.GetType() + " ; Existed Type = " + existedStation.GetType());
                }
                (existedStation as IJFStation).Name = name;
                DeclearedStationNames.Add(station.Name);
                //station.WorkStatusChanged += StationWorkStatusChanged;
                //station.CustomStatusChanged += StationCustomStatusChanged;
                //if(station is JFCmdWorkBase)
                //    (station as JFCmdWorkBase).WorkMsg2Outter += StationTxtMsg;
                return;
                //throw new ArgumentException("StationManager.DeclearStation(IJFStation station) failed by station's Name = \"" + name + "\" is exsited in InitorStation-List!");
            }

            JFHubCenter.Instance.InitorManager.Add(station.Name, station);
            DeclearedStationNames.Add(station.Name);
            //station.WorkStatusChanged += StationWorkStatusChanged;
            //station.CustomStatusChanged += StationCustomStatusChanged;
            //if (station is JFCmdWorkBase)
            //    (station as JFCmdWorkBase).WorkMsg2Outter += StationTxtMsg;
        }

        public void StationTxtMsg(object sender,string msgInfo)
        {
            IJFStation station = sender as IJFStation;
                if (_StationMsgReciever.ContainsKey(station))
                {
                    List<IJFStationMsgReceiver> uis = _StationMsgReciever[station];
                    foreach (IJFStationMsgReceiver ui in uis)
                    ui.OnTxtMsg(msgInfo);
                }

            //Task t = new Task(() => /*日后可能改成固定线程，性能是否可优化存疑*/
            //{

                ///将消息发送到MainStaion处理
                MainStation.OnStationTxtMsg(sender as IJFStation, msgInfo);
                EventStationTxtMsg?.Invoke(sender, msgInfo);
            //});

            


        }

        void StationWorkStatusChanged(object sender, JFWorkStatus currWorkStatus)
        {
            _StationWorkStatusChanged(sender as IJFStation, currWorkStatus, true); 
        }

        void StationCustomStatusChanged(object sender, int currCustomStatus)
        {
            _StationCustomStatusChanged(sender as IJFStation, currCustomStatus, true);
        }

        void StationCustomizeMsg(object station, string msgCategory, object[] msgParams)
        {
            _StationCustomizeMsg(station as IJFStation, msgCategory, msgParams, true);
        }

        void StationProductFinished(object station, int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo)
        {
            _StationProductFinished(station as IJFStation, passCount, passIDs, ngCount, ngIDs, ngInfo,true);
        }




        /// <summary>
        ///  通过配置界面添加的所有工站名称
        /// </summary>
        List<string> _initorStationNames()
        {
            List<string> ret = new List<string>();
            string[] initStationNames = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFStation));
            ret.AddRange(initStationNames);
            return ret ; 
        }


        


        
        /// <summary>
        /// 获取程序中所有可用的工站
        /// </summary>
        /// <returns></returns>
        public string[] AllStationNames()
        {
            List<string> ret = new List<string>();
            foreach (string declearedStationName in DeclearedStationNames)
                ret.Add(declearedStationName);
            List<string> allStationNames = _initorStationNames();
            foreach (string sn in allStationNames)
                if (!DeclearedStationNames.Contains(sn))
                    ret.Add(sn);

            //ret.AddRange(_initorStationNames());
            return ret.ToArray();
        }

        public IJFStation GetStation(string stationName)
        {
            //foreach (IJFStation station in _lstDeclearStations) //查询申明的工站列表
            //    if (station.Name == stationName)
            //        return station;
            JFInitorManager initorMgr = JFHubCenter.Instance.InitorManager;
            string[] allInitorStation = initorMgr.GetIDs(typeof(IJFStation)); //通过架构UI创建的工站对象
            if (null == allInitorStation)
                return null;
            foreach (string stName in allInitorStation)
                if (stationName == stName)
                    return initorMgr.GetInitor(stationName) as IJFStation;
            return null;
        }


        /// <summary>
        /// 所有工站的消息接收者
        /// </summary>
        Dictionary<IJFStation, List<IJFStationMsgReceiver>> _StationMsgReciever = new Dictionary<IJFStation, List<IJFStationMsgReceiver>>();

        /// <summary>
        /// 为工站附加一个ui ， 一般情况下由App中的架构功能调用
        /// 如：不提供RealtimeUI的工站，系统会自动指派一个ui 
        /// </summary>
        /// <param name="station"></param>
        /// <param name="ui"></param>
        public void AppendStationMsgReceiver(IJFStation station, IJFStationMsgReceiver rcver)
        {
            if (!_StationMsgReciever.ContainsKey(station))
                _StationMsgReciever.Add(station, new List<IJFStationMsgReceiver>());

            List<IJFStationMsgReceiver> uis = _StationMsgReciever[station];
            if (!uis.Contains(rcver))
                uis.Add(rcver);
        }

        /// <summary>
        /// 将一个ui移除
        /// </summary>
        /// <param name="ui"></param>
        public void RemoveStationMsgReciever(IJFStationMsgReceiver ui)
        {
            foreach (List<IJFStationMsgReceiver> stationUIS in _StationMsgReciever.Values)
                if (stationUIS.Contains(ui))
                    stationUIS.Remove(ui);
        }


        Dictionary<string, IJFLogger> _stationLoggers = new Dictionary<string, IJFLogger>(); //各工站的日志记录对象

        /// <summary>
        /// 接收一条工站日志记录,保存
        /// 如果工站自身未提供UI，则会在架构附加的StationUI上显示
        /// 如果工站自身有UI，则忽略显示功能
        /// </summary>
        /// <param name="info"></param>
        public void OnStationLog(IJFStation station, string info, JFLogLevel level,LogMode mode)
        {
            ///添加界面显示
            ///向日志线程中添加一条记录

            if (null == station)
                throw new ArgumentNullException("Station is null in JFStationManeger.StationLog(station, ...");
            string stationName = station.Name;
            if (string.IsNullOrEmpty(stationName))
                throw new ArgumentNullException("Station's Name is null or empty in JFStationManeger.StationLog(station, ...");

            if ((mode & LogMode.Record) == LogMode.Record)
            {
                if (!_stationLoggers.ContainsKey(stationName))
                    _stationLoggers.Add(stationName, JFLoggerManager.Instance.GetLogger(stationName));
                _stationLoggers[stationName].Log(level, info);
            }
            if (_isShowLogThreadRunning)
            {
                if((mode & LogMode.Show) == LogMode.Show)
                    if (_StationMsgReciever.ContainsKey(station))
                {
                    _log2Shows.Enqueue(new KeyValuePair<IJFStation, string>(station, level.ToString() + ":" + info));
                    _semaphoreShowLog.Release(1);
                }
            }

        }

        void StartShowStationLog()
        {
            if (_isShowLogThreadRunning)
                return;
            _isShowLogThreadRunning = true;
            _threadShowStationLog = new Thread(_ThreadFuncShowStationLog);
            _threadShowStationLog.Start();
        }

        void StopShowStationLog()
        {
            if (!_isShowLogThreadRunning)
                return;
            _isShowLogThreadRunning = false;
            if (!_threadShowStationLog.Join(500))
                _threadShowStationLog.Abort();
            _threadShowStationLog = null;

        }

        Thread _threadShowStationLog = null;
        object logLocker = new object();
        Semaphore _semaphoreShowLog = new Semaphore(0, int.MaxValue);
        bool _isShowLogThreadRunning = false;
        /// <summary>
        /// 需要在界面上显示的log
        /// </summary>
        Queue<KeyValuePair<IJFStation, string>> _log2Shows = new Queue<KeyValuePair<IJFStation, string>>();
        void _ThreadFuncShowStationLog()
        {
            while(_isShowLogThreadRunning)
            {
                if(_semaphoreShowLog.WaitOne(200))
                {
                    if (0 == _log2Shows.Count)
                        continue;

                    KeyValuePair<IJFStation, string> kv = _log2Shows.Dequeue();
                    if (kv.Key == null)
                        continue;
                    List<IJFStationMsgReceiver> uis = _StationMsgReciever[kv.Key];
                    if (null != uis)
                    foreach (IJFStationMsgReceiver ui in uis)
                        ui.OnTxtMsg(kv.Value);
                    
                }
            }
        }

        /// <summary>
        /// 处理工站状态改变
        /// 未提供JustRealTimeUI的工站通过此函数刷新界面
        /// 以提供JustRealTimeUI的工站 只是通过此函数向MainStation发送消息，相关界面更新功能由工站自身维护
        /// </summary>
        /// <param name="station"></param>
        /// <param name="currWorkStatus"></param>
        void _StationWorkStatusChanged(IJFStation station, JFWorkStatus currWorkStatus,bool isSynchMode = true) //异步模式有问题(异步线程未运行'),待日后改进
        {
            if (isSynchMode)
            {
                if (_StationMsgReciever.ContainsKey(station))
                {
                    List<IJFStationMsgReceiver> uis = _StationMsgReciever[station];
                    foreach (IJFStationMsgReceiver ui in uis)
                        ui.OnWorkStatusChanged(currWorkStatus);
                }
                ///将消息发送到MainStaion处理
                MainStation.OnStationWorkStatusChanged(station, currWorkStatus);
                EventStationWorkStatusChanged?.Invoke(station, currWorkStatus);
            }
            else
            {
                Task t = new Task(() => /*日后可能改成固定线程，性能是否可优化存疑*/
                {
                    if (_StationMsgReciever.ContainsKey(station))
                    {
                        List<IJFStationMsgReceiver> uis = _StationMsgReciever[station];
                        foreach (IJFStationMsgReceiver ui in uis)
                            ui.OnWorkStatusChanged(currWorkStatus);
                    }
                ///将消息发送到MainStaion处理
                    MainStation.OnStationWorkStatusChanged(station, currWorkStatus);
                    EventStationWorkStatusChanged?.Invoke(station, currWorkStatus);
                });
            }
        }


        /// <summary>
        ///  处理工站的业务状态发生改变
        /// 未提供JustRealTimeUI的工站通过此函数刷新界面
        /// 以提供JustRealTimeUI的工站 只是通过此函数向MainStation发送消息，相关界面更新功能由工站自身维护
        /// </summary>
        /// <param name="station"></param>
        /// <param name="currCustomStatus"></param>
        void _StationCustomStatusChanged(IJFStation station, int currCustomStatus, bool isSynchMode = true)//异步模式有问题(异步线程未运行'),待日后改进
        {
            if (isSynchMode)
            {
                if (_StationMsgReciever.ContainsKey(station))
                {
                    List<IJFStationMsgReceiver> uis = _StationMsgReciever[station];
                    foreach (IJFStationMsgReceiver ui in uis)
                        ui.OnCustomStatusChanged(currCustomStatus);
                }
                MainStation.OnStationCustomStatusChanged(station, currCustomStatus);
                EventStationCustomStatusChanged?.Invoke(station, currCustomStatus);
            }
            else
            {
                

                Task t = new Task(() =>
                {
                    ///添加代码，界面显示
                    if (_StationMsgReciever.ContainsKey(station))
                    {
                        List<IJFStationMsgReceiver> uis = _StationMsgReciever[station];
                        foreach (IJFStationMsgReceiver ui in uis)
                            ui.OnCustomStatusChanged(currCustomStatus);
                    }
                    MainStation.OnStationCustomStatusChanged(station, currCustomStatus);
                    EventStationCustomStatusChanged?.Invoke(station, currCustomStatus);
                });
            }
        }

        /// <summary>
        /// 产品加工完成消息
        /// 未提供JustRealTimeUI的工站通过此函数刷新界面
        /// 以提供JustRealTimeUI的工站 只是通过此函数向MainStation发送消息，相关界面更新功能由工站自身维护
        /// </summary>
        /// <param name="station">消息发送者</param>
        /// <param name="PassCount">本次生产完成的成品数量</param>
        /// <param name="NGCount">本次生产的次品数量</param>
        /// <param name="NGInfo">次品信息</param>
        void _StationProductFinished(IJFStation station, int passCount, string[] passIDs, int ngCount, string[] ngIDs, string[] ngInfo, bool isSynchMode = true)//异步模式有问题(异步线程未运行'),待日后改进
        {
            if (isSynchMode)
            {
                if (_StationMsgReciever.ContainsKey(station))
                {
                    List<IJFStationMsgReceiver> uis = _StationMsgReciever[station];
                    foreach (IJFStationMsgReceiver ui in uis)
                        ui.OnProductFinished(passCount, passIDs, ngCount, ngIDs, ngInfo);
                }
                MainStation.OnStationProductFinished(station, passCount, passIDs, ngCount, ngIDs, ngInfo);
                EventStationProductFinished?.Invoke(station, passCount, passIDs, ngCount, ngIDs, ngInfo);

            }
            else
            {
                Task t = new Task(() =>
               {
                   if (_StationMsgReciever.ContainsKey(station))
                   {
                       List<IJFStationMsgReceiver> uis = _StationMsgReciever[station];
                       foreach (IJFStationMsgReceiver ui in uis)
                           ui.OnProductFinished(passCount, passIDs, ngCount, ngIDs, ngInfo);
                   }
                   MainStation.OnStationProductFinished(station, passCount, passIDs, ngCount, ngIDs, ngInfo);
                   EventStationProductFinished?.Invoke(station, passCount, passIDs, ngCount, ngIDs, ngInfo);
               });
            }

        }

        /// <summary>
        /// 处理工站发来的其他定制化的消息
        /// 只是向MainStation发送消息，定制化的界面显示由工站通过自身提供的JustRealTimeUI完成
        /// </summary>
        /// <param name="station"></param>
        /// <param name="msg"></param>
        void _StationCustomizeMsg(IJFStation station, string msgCategory, object[] msgParams, bool isSynchMode = true)//异步模式有问题(异步线程未运行'),待日后改进
        {
            if (isSynchMode)
            {
                if (_StationMsgReciever.ContainsKey(station))
                {
                    List<IJFStationMsgReceiver> uis = _StationMsgReciever[station];
                    foreach (IJFStationMsgReceiver ui in uis)
                        ui.OnCustomizeMsg(msgCategory, msgParams);
                }


                MainStation.OnStationCustomizeMsg(station, msgCategory, msgParams);
                EventStationCustomizeMsg?.Invoke(station, msgCategory, msgParams);
            }
            else
            {
                Task t = new Task(() =>
                {
                    if (_StationMsgReciever.ContainsKey(station))
                    {
                        List<IJFStationMsgReceiver> uis = _StationMsgReciever[station];
                        foreach (IJFStationMsgReceiver ui in uis)
                            ui.OnCustomizeMsg(msgCategory, msgParams);
                    }


                    MainStation.OnStationCustomizeMsg(station, msgCategory, msgParams);
                    EventStationCustomizeMsg?.Invoke(station, msgCategory, msgParams);
                });
            }
        }

        /// <summary>
        /// 设置工站使能
        /// </summary>
        /// <param name="stationName"></param>
        /// <param name="enable"></param>
        public void SetStationEnabled(string stationName,bool enable)
        {
            if (!_initorStationNames().Contains(stationName))
                throw new ArgumentException("stationName = \"" + stationName + "\" is not included by Station-Name List");
            if (!_dictStationEnabled.ContainsKey(stationName))
            {
                _dictStationEnabled.Add(stationName, enable);
                return;
            }
            else
                _dictStationEnabled[stationName] = enable;
            IJFStation station = GetStation(stationName);
            if (enable)
            {
                station.WorkStatusChanged += StationWorkStatusChanged;
                station.CustomStatusChanged += StationCustomStatusChanged;
                if (station is JFCmdWorkBase)
                    (station as JFCmdWorkBase).WorkMsg2Outter += StationTxtMsg;
                if(station is JFStationBase)
                {
                    (station as JFStationBase).EventCustomizeMsg += StationCustomizeMsg;
                    (station as JFStationBase).EventProductFinished += StationProductFinished;
                }
                
            }
            else
            {
                station.WorkStatusChanged -= StationWorkStatusChanged;
                station.CustomStatusChanged -= StationCustomStatusChanged;
                if (station is JFCmdWorkBase)
                    (station as JFCmdWorkBase).WorkMsg2Outter -= StationTxtMsg;

                if (station is JFStationBase)
                {
                    (station as JFStationBase).EventCustomizeMsg -= StationCustomizeMsg;
                    (station as JFStationBase).EventProductFinished -= StationProductFinished;
                }

            }
            _cfg.Save();
        }

        /// <summary>
        /// 获取工站使能
        /// </summary>
        /// <param name="stationName"></param>
        /// <returns></returns>
        public bool GetStationEnabled(string stationName)
        {
            if (!_initorStationNames().Contains(stationName))
                throw new ArgumentException("stationName = \"" + stationName + "\" is not included by Station-Name List");

            if (!_dictStationEnabled.ContainsKey(stationName))
            {
                _dictStationEnabled.Add(stationName, true);
                _cfg.Save();
            }
            return _dictStationEnabled[stationName];
        }

        /// <summary>
        /// 获取所有已使能的工站名称
        /// </summary>
        /// <returns></returns>
        public string[] AllEnabledStationNames()
        {
            List<string> ret = new List<string>();
            List<string> allStationNames = _initorStationNames();
            foreach (string stationName in allStationNames)
                if (GetStationEnabled(stationName))
                    ret.Add(stationName);
            return ret.ToArray();
        }


        bool IsStationRunning(IJFStation station)
        {
            JFWorkStatus ws = station.CurrWorkStatus;
            return ws == JFWorkStatus.Running || ws == JFWorkStatus.Pausing || ws == JFWorkStatus.Interactiving;
        }

        /// <summary>
        /// 停止工站日志记录/显示
        /// 在程序退出前调用
        /// </summary>
        public void Stop()
        {
            string errorInfo;
            MainStation.Stop(out errorInfo);
            string[] stationNames = AllStationNames();
            if(null != stationNames)
                foreach(string stationName in stationNames)
                {
                    IJFStation station = GetStation(stationName);
                    if (IsStationRunning(station))
                    {
                        JFWorkCmdResult ret = station.Stop(1000);
                        if (ret != JFWorkCmdResult.Success)
                        {
                            //日后可能添加强制关闭的系统日志...
                            station.Abort();
                        }
                    }
                }
            JFLoggerManager.Instance.Stop();
            StopShowStationLog();
            if (null != stationNames)
                foreach (string stationName in stationNames)
                {
                    IJFStation station = GetStation(stationName);
                    station.WorkStatusChanged -= StationWorkStatusChanged;
                    station.CustomStatusChanged -= StationCustomStatusChanged;
                    if (station is JFCmdWorkBase)
                        (station as JFCmdWorkBase).WorkMsg2Outter -= StationTxtMsg;

                    if (station is JFStationBase)
                    {
                        (station as JFStationBase).EventCustomizeMsg -= StationCustomizeMsg;
                        (station as JFStationBase).EventProductFinished -= StationProductFinished;
                    }
                }

            Thread.Sleep(2000);
        }
    }
}
