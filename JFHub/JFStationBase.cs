using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using JFInterfaceDef;
using JFToolKits;
using JFUI;
namespace JFHub
{

    /// <summary>
    /// 产品加工完成消息
    /// </summary>
    /// <param name="station">消息发送者</param>
    /// <param name="PassCount">本次生产完成的成品数量</param>
    /// <param name="NGCount">本次生产的次品数量</param>
    /// <param name="NGInfo">次品信息</param>
    public delegate void DelegateStationProductFinished(object station, int passCount,string[] passIDs, int NGCount, string[] ngIDs,string[] NGInfo);

    /// <summary>
    /// 代理：向外部程序发送定制消息
    /// </summary>
    /// <param name="station"></param>
    /// <param name="msg"></param>
    public delegate void DelegateStationCustomizeMsg(object station, string msgCategory,object[] msgParam);


    public abstract class JFStationBase : JFCmdWorkBase, IJFStation
    {

        public static string WorkStatusName(JFWorkStatus ws)
        {
            string wsName = ws.ToString();
            switch (ws)
            {
                case JFWorkStatus.UnStart:// = 0,    //线程未开始运行
                    wsName = "未开始";
                    break;
                case JFWorkStatus.Running://,        //线程正在运行，未退出
                    wsName = "运行中";
                    break;
                case JFWorkStatus.Pausing://,        //线程暂停中
                    wsName = "已暂停";
                    break;
                case JFWorkStatus.Interactiving://,  //人机交互 ， 等待人工干预指令
                    wsName = "人工干预";
                    break;
                case JFWorkStatus.NormalEnd://     //线程正常完成后退出
                    wsName = "正常结束";
                    break;
                case JFWorkStatus.CommandExit://,    //收到退出指令
                    wsName = "指令结束";
                    break;
                case JFWorkStatus.ErrorExit://,      //发生错误退出，（重启或人工消除错误后可恢复）
                    wsName = "错误退出";
                    break;
                case JFWorkStatus.ExceptionExit://,  //发生异常退出 ,  (不可恢复的错误)
                    wsName = "异常退出";
                    break;
                case JFWorkStatus.AbortExit://,      //由调用者强制退出
                    wsName = "强制终止";
                    break;
                default:

                    break;
            }
            return wsName;
        }
        public JFStationBase()
        {
            //Name = "";
            DeclearCfgParam(JFParamDescribe.Create("工站归零模式", typeof(string), JFValueLimit.Options, _resetModes), "工站基础配置");
            SetCfgParamValue("工站归零模式", _resetModes[0]);//默认模式为：每次运行前都要归零1次
            _cfgFilePath = null;
            _cfg = new JFXCfg();
            IsInitOK = false;
            _ui = new UcStationRealtimeUI();
            _ui.SetStation(this);

        }
        string[] _resetModes = new string[] { "每次运行前必须归零", "程序启动后只运行一次", "运行前不检查是否归零" };

        public DelegateStationProductFinished EventProductFinished;
        public DelegateStationCustomizeMsg EventCustomizeMsg;

        int _standardAxisCount = 0; //标准轴数量
        public int StandardAxisCount { get { return _standardAxisCount; } }

        string _initErrorInfo = "No-Ops"; //初始化动作失败信息



        string _cfgFilePath; //工站配置文件名称
        JFXCfg _cfg = null;
        /// <summary>
        /// 用于保存工站私有配置数据
        /// </summary>
        public JFXCfg Config { get { return _cfg; } }

        Dictionary<string, JFMethodFlow> _dictMethodFlow = new Dictionary<string, JFMethodFlow>();//List<JFMethodFlow> _lstMethodFlowsInCfg = new List<JFMethodFlow>(); //可增删改的工作流(包括注册工作流)


        string _stationName = null;
        public override string Name //JFCmdWorkBase.Name
        {
            get
            {
                string nameInCfg = JFHubCenter.Instance.InitorManager.GetIDByInitor(this);
                return nameInCfg != null ? nameInCfg : _stationName;
            }
            set
            {
                if (null != JFHubCenter.Instance.InitorManager.GetIDByInitor(this)) //对已经存在于Initor管理器中的工站，不能修改名称
                    return;
                _stationName = value;
            }
        }

        string IJFWork.Name //IJFStation.Name
        {
            get { return Name; }
            set { Name = value; }
        }

        JFDataPool _workFlowDataPool = new JFDataPool();
        /// <summary>
        /// 工站内部工作流共用的数据池
        /// </summary>
        public JFDataPool DataPool { get { return _workFlowDataPool; } }

        #region IJFInitializable's API



        string[] _stationBaseInitParams = new string[] { //"工站名称", //工站名称 
                                                         "配置文件",//配置文件路径
                                                         "标准轴数"//标准轴为 X，Y，Z，R， 可选值0，1，2，3，4
                                                        };
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public virtual string[] InitParamNames { get { return _stationBaseInitParams; } }

        /// <summary>
        /// 检查初始化参数名称是否合法
        /// 如果InitParamNames未包含initParamName 会抛出异常
        /// </summary>
        /// <param name="initParamName"></param>
        protected void CheckInitParamName(string initParamName, string function = null)
        {
            if (string.IsNullOrEmpty(initParamName))
                throw new ArgumentNullException("initParamName is null or empty" + function == null ? "" : (" in StationName=" + Name + " 's function()"));
            if (!InitParamNames.Contains(initParamName))
                throw new ArgumentNullException("initParamName is not contained by ParamName's List, " + function == null ? "" : ("StationName=" + Name + " 's function()"));
        }
        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual JFParamDescribe GetInitParamDescribe(string name)
        {
            CheckInitParamName(name, "GetInitParamDescribe");
            //if (name == _stationBaseInitParams[0])
            //    return JFParamDescribe.Create(_stationBaseInitParams[0], typeof(string), JFValueLimit.NonLimit,null);
            /*else */
            if (name == _stationBaseInitParams[0])
                return JFParamDescribe.Create(_stationBaseInitParams[0], typeof(string), JFValueLimit.FilePath, null);
            else if (name == _stationBaseInitParams[1]) //工站中所包含的标准轴数（X，Y，Z，R）
                return JFParamDescribe.Create(_stationBaseInitParams[1], typeof(int), JFValueLimit.Options, new object[] { 0, 1, 2, 3, 4 });
            throw new Exception();//不可能运行到这一步
        }

        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public virtual object GetInitParamValue(string name)
        {
            CheckInitParamName(name, "GetInitParamValue");
            //if (name == _stationBaseInitParams[0])
            //    return Name;
            /*else */
            if (name == _stationBaseInitParams[0])
                return _cfgFilePath;
            else if (name == _stationBaseInitParams[1])
                return _standardAxisCount;
            throw new Exception();//不可能运行到这一步
        }

        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public virtual bool SetInitParamValue(string name, object value)
        {
            CheckInitParamName(name, "SetInitParamValue");
            if (name == _stationBaseInitParams[0])
            {
                if (null == value)
                {
                    _initErrorInfo = "SetInitParamValue(name = " + "\"" + name + "\",value) falied by: value = null";
                    return false;
                }
                if (!typeof(string).IsAssignableFrom(value.GetType()))
                {
                    _initErrorInfo = "SetInitParamValue(name = " + "\"" + name + "\",value) falied by: value's type = " + value.GetType().Name + " is not Assignable to string";
                    return false;
                }
                _cfgFilePath = value as string;
                _initErrorInfo = "Success";
                return true;
            }
            else if (name == _stationBaseInitParams[1])
            {
                if (null == value)
                {
                    _initErrorInfo = "SetInitParamValue(name = " + "\"" + name + "\",value) falied by: value = null";
                    return false;
                }

                int nValue = 0;
                if (typeof(int) == value.GetType())
                    nValue = (int)value;

                else if (JFTypeExt.IsExplicitFrom(typeof(int), value.GetType()))
                    nValue = (int)JFConvertExt.ChangeType(value, typeof(int));
                else
                {
                    _initErrorInfo = "SetInitParamValue(name = " + "\"" + name + "\",value) falied by: value's type = " + value.GetType().Name + " can not Changeto int";
                    return false;
                }

                if (nValue < 0 || nValue > 4)
                {
                    _initErrorInfo = "SetInitParamValue(name = " + "\"" + name + "\",value) falied by: value = " + nValue + " is outof range 0~4";
                    return false;
                }
                _standardAxisCount = nValue;
                return true;
            }
            throw new Exception();
        }


        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public virtual bool Initialize()
        {

            if (string.IsNullOrEmpty(_cfgFilePath))
            {
                _initErrorInfo = "Initialize Failed by: Station's CfgFilePath Is Null Or Empty";
                IsInitOK = false;
                return false;
            }

            try
            {
                //_cfg = new JFXCfg();
                _cfg.Load(_cfgFilePath, true);
                LoadCfg(); //加载工站配置
            }
            catch (Exception ex)
            {
                _cfg = null;
                _initErrorInfo = "Load Station's cfg failed!path = " + _cfgFilePath + ",Error:" + ex.Message;
                IsInitOK = false;
                return false;
            }

            _initErrorInfo = "Success";
            IsInitOK = true;
            return true;


        }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public virtual bool IsInitOK { get; private set; }

        /// <summary>获取初始化错误的描述信息</summary>
        public virtual string GetInitErrorInfo()
        {
            return _initErrorInfo;
        }
        #endregion


        #region 申明工站使用的不可删除对象（包括设备/配置/方法流） ， 建议（只）在继承类的构造函数中调用
        internal JFXmlDictionary<NamedChnType, List<List<string>>> DeclearedDevChnMapping
        {
            get
            {
                JFXmlDictionary<string, object> baseCfg = _cfg.GetItemValue("StationBasePrivateConfig") as JFXmlDictionary<string, object>;
                if (null == baseCfg)
                    return null;
                if (!baseCfg.ContainsKey("LocalDevChannelMap"))
                    return null;
                JFXmlDictionary<NamedChnType, List<List<string>>> dictDevChnMap = baseCfg["LocalDevChannelMap"] as JFXmlDictionary<NamedChnType, List<List<string>>>; //配置中已经保存的映射表
                return dictDevChnMap;
            }
        }

        /// <summary>
        /// 获取一个声明的（工站固有的）设备通道的真正（全局）名称
        /// </summary>
        /// <param name="chnType"></param>
        /// <param name="locName"></param>
        /// <returns></returns>
        protected string GetDecChnGlobName(NamedChnType devType, string locName)
        {
            if (!IsDevChnDecleared(devType, locName))
                return null;
            JFXmlDictionary<NamedChnType, List<List<string>>> namesMap = DeclearedDevChnMapping;
            if (null == namesMap)
                return null;
            if (!namesMap.ContainsKey(devType))
                return null;
            List<List<string>> lgMaps = namesMap[devType];
            foreach (List<string> lg in lgMaps)
                if (lg[0] == locName)
                {
                    if (lg.Count > 1)
                        return lg[1];
                    else
                        return null;
                }

            return null;
        }



        //获取轴的替身名 （如果轴未指定替身名，则返回 axisGlobName）
        /// <summary>
        /// 
        /// </summary>
        /// <param name="axisGlobName"></param>
        /// <returns></returns>
        public string GetDecChnAliasName(NamedChnType devType, string axisGlobName)
        {
            JFXmlDictionary<NamedChnType, List<List<string>>> namesMap = DeclearedDevChnMapping;
            if (null == namesMap)
                return axisGlobName;
            if (!namesMap.ContainsKey(devType))
                return null;
            List<List<string>> lgMaps = namesMap[devType];
            foreach (List<string> lg in lgMaps)
                if (lg.Count > 1 && lg[1] == axisGlobName)
                {
                        return lg[0];

                }

            return axisGlobName;
        }





        /// <summary>
        ///  获取工站声明的（固有的）所有设备通道的名称
        ///  主要用于在可选界面中作为参数列表 等...
        /// </summary>
        /// <param name="devType">设备类型</param>
        /// <returns></returns>
        public string[] AllDecDevChnNames(NamedChnType devType)
        {
            if (!_dictDeclearedDevChns.ContainsKey(devType))
                return null;
            return _dictDeclearedDevChns[devType].ToArray();

        }




        JFXmlDictionary<NamedChnType, List<string>> _dictDeclearedDevChns = new JFXmlDictionary<NamedChnType, List<string>>();
        //Dictionary<string, string> _dictDeclearedDevChnMap = new Dictionary<string, string>(); //保存声明的本地名称和全局名称的映射
        /// <summary>
        /// 声明工站使用的设备/通道（名称）
        /// </summary>
        /// <param name="devType">设备类型</param>
        /// <param name="locName">设备在工站中的名称</param>
        /// <param name="globalName">设备全局名称</param>
        protected void DeclearDevChn(NamedChnType devType, string locName/*, string globalName = null*/)
        {
            if (devType == NamedChnType.None)
                throw new ArgumentException("DeclearDevChn(devType,...) failed by devType == NamedChnType.None");
            if (string.IsNullOrEmpty(locName))
                throw new ArgumentException("DeclearDevChn(devType,locName,...) failed by locName is null or empty");
            if (!_dictDeclearedDevChns.ContainsKey(devType))
                _dictDeclearedDevChns.Add(devType, new List<string>());
            if (_dictDeclearedDevChns[devType].Contains(locName))
                throw new ArgumentException("DeclearDevChn failed: locName = " + locName + " has been decleared!");
            _dictDeclearedDevChns[devType].Add(locName);
            //if(!string.IsNullOrEmpty(globalName))//注册时已指定设备的全局名
            //    _dictDeclearedDevChnMap.Add(locName, globalName);

        }

        /// <summary>
        /// 设备通道是否为声明（固有）属性
        /// </summary>
        /// <param name="devType"></param>
        /// <param name="locName"></param>
        /// <returns></returns>
        public bool IsDevChnDecleared(NamedChnType devType, string locName)
        {
            if (string.IsNullOrEmpty(locName))
                return false;
            if (!_dictDeclearedDevChns.ContainsKey(devType))
                return false;
            return _dictDeclearedDevChns[devType].Contains(locName);
        }



        List<string> _lstWorkPosDecleared = new List<string>();
        /// <summary>
        /// 声明一个工作点位
        /// </summary>
        /// <param name="wpName"></param>
        /// <param name="pos"></param>
        protected void DeclearWorkPosition(string wpName)
        {
            if (string.IsNullOrEmpty(wpName))
                throw new ArgumentNullException("DeclearWorkPosition(wpName) failed by: wpName is null or empty!");
            if (IsWorkPosDecleared(wpName))
                throw new ArgumentException("DeclearWorkPosition(wpName) failed by: wpName = " + wpName + " is already decleared");
            _lstWorkPosDecleared.Add(wpName);

        }


        public bool IsWorkPosDecleared(string wpName)
        {
            return _lstWorkPosDecleared.Contains(wpName);
        }


        List<string> _lstMethodFlowDecleared = new List<string>();

        /// <summary>
        /// 申明一个动作流（不可删除的）
        /// </summary>
        /// <param name="mfName"></param>
        /// <param name="mf"></param>
        protected void DeclearMethodFlow(string mfName)
        {
            if (string.IsNullOrEmpty(mfName))
                throw new ArgumentNullException("DeclearMethodFlow(string mfName) failed by: mfName is null or empty");
            if (IsMethodFlowDecleared(mfName))
                throw new Exception("DeclearMethodFlow(string mfName) failed by: mfName =" + mfName + " is already decleared!");
            _lstMethodFlowDecleared.Add(mfName);
        }

        public bool IsMethodFlowDecleared(string mfName)
        {
            return _lstMethodFlowDecleared.Contains(mfName);
        }





        #endregion


        bool IsRecievedEndBatchCmd { get; set; }
        /// <summary>
        /// IJFStation's API
        /// </summary>
        /// <returns></returns>
        public JFWorkCmdResult EndBatch(int milliSeconds = -1) // 向工作线程下达结批指令,供外部调用者使用
        {
            if (CurrWorkStatus != JFWorkStatus.Running &&
                CurrWorkStatus != JFWorkStatus.Pausing &&
                CurrWorkStatus != JFWorkStatus.Interactiving)
                return JFWorkCmdResult.StatusError;
            IsRecievedEndBatchCmd = true;
            return JFWorkCmdResult.Success;
            //return _SendCmd(CommandEndBatch,)
        }


        /// <summary>
        /// 工作线程当前执行的任务模式
        /// </summary>
        protected enum StationThreadWorkMode
        {
            Normal = 0,//正常工作模式
            Resetting, //工站归零任务
        }

        StationThreadWorkMode _stationThreadWorkMode = StationThreadWorkMode.Normal;

        /// <summary>
        /// 工站线程的运行模式 
        /// </summary>
        protected StationThreadWorkMode STWorkMode{get{return _stationThreadWorkMode;} }
        public override JFWorkCmdResult Start()
        {
            lock (accessLocker)
            {
                if (_stationThreadWorkMode == StationThreadWorkMode.Resetting) //当前正在进行归零动作
                {
                    if (CurrWorkStatus == JFWorkStatus.Running ||
                           CurrWorkStatus == JFWorkStatus.Pausing ||
                           CurrWorkStatus == JFWorkStatus.Interactiving
                           )
                        return JFWorkCmdResult.StatusError;
            }
                _stationThreadWorkMode = StationThreadWorkMode.Normal;
                return base.Start();
            }
        }


        /// <summary>
        /// 向工站发送复位指令（各轴归零等动作）
        /// </summary>
        /// <returns></returns>
        public  JFWorkCmdResult Reset()
        {
            lock (accessLocker)
            {
                if (_stationThreadWorkMode == StationThreadWorkMode.Normal)
                {
                    if (CurrWorkStatus == JFWorkStatus.Pausing ||
                        CurrWorkStatus == JFWorkStatus.Interactiving ||
                        CurrWorkStatus == JFWorkStatus.Pausing) // 正在运行时不能执行归零动作
                        return JFWorkCmdResult.StatusError;
                }
                _stationThreadWorkMode = StationThreadWorkMode.Resetting; //将线程工作模式置为归零模式
                JFWorkCmdResult ret = base.Start();
                return ret;
            }
        }



        /// <summary>
        /// 执行归零动作，由继承类实现
        /// 函数如果执行失败 可通过ExitWork返回错误信息
        /// </summary>
        protected abstract void ExecuteReset();

        /// <summary>
        /// 打开程序后是否执行过一次归零动作
        /// </summary>
        bool _isExecuteResetOnce = false; 

        /// <summary>
        /// 执行结批动作，由继承类实现
        /// 函数如果执行失败 可通过ExitWork返回错误信息
        /// </summary>
        protected abstract void ExecuteEndBatch();


        /// <summary>
        /// 工站启动后是否需要归零
        /// </summary>
        /// <returns></returns>
        protected bool IsNeedResetWhenStart()
        {
            if (_stationThreadWorkMode == StationThreadWorkMode.Resetting)
                return true;

            string resetMode = GetCfgParamValue("工站归零模式") as string;
            if (resetMode == _resetModes[0]) //每次运行前都归零
                return true;
            else if (resetMode == _resetModes[1] && !_isExecuteResetOnce)//程序开启后只运行一次
                return true;
            return false;
        }


        /// <summary>
        /// 为了支持结批/归零动作，重写线程函数
        /// </summary>
        protected override void ThreadFunc()
        {
            long cmdWaited = CommandUnknown;
            WorkExitCode exitCode = WorkExitCode.Normal;
            try
            {
                cmdEvent.WaitOne();
                if (command != CommandStart)
                    ExitWork(WorkExitCode.Exception, "WorkThread receive first command is not CommandStart,command = " + command);
                RespCmd(JFWorkCmdResult.Success);
                ChangeWorkStatus(JFWorkStatus.Running);
                
                if(_stationThreadWorkMode == StationThreadWorkMode.Normal)
                    PrepareWhenWorkStart();
                else
                {
                    SendMsg2Outter("工站开始执行归零...");
                    ExecuteReset();
                    _isExecuteResetOnce = true;
                    ExitWork(WorkExitCode.Normal, "工站归零执行完成");
                }


                string resetMode = GetCfgParamValue("工站归零模式") as string;
                if(resetMode == _resetModes[0]) //每次运行前都归零
                {
                    SendMsg2Outter("任务运行前开始归零...");
                    ExecuteReset();
                    SendMsg2Outter("归零完成，开始运行任务");
                    _isExecuteResetOnce = true;
                }
                else if(resetMode == _resetModes[1])//程序开启后只运行一次
                {
                    if(!_isExecuteResetOnce)
                    {
                        SendMsg2Outter("任务运行前开始归零...");
                        ExecuteReset();
                        SendMsg2Outter("归零完成，开始运行任务");
                        _isExecuteResetOnce = true;
                    }
                }


                while (true)
                {
                    CheckCmd(CycleMilliseconds < 0 ? -1 : CycleMilliseconds);
                    RunLoopInWork();
                    if(IsRecievedEndBatchCmd)
                    {
                        ExecuteEndBatch();//DllNotFoundException() hehe ...
                        IsRecievedEndBatchCmd = false;
                        ExitWork(WorkExitCode.Normal, "结批完成");
                    }
                }
            }
            catch (JFWorkExitException wee) //工作线程退出流程
            {
                //Monitor.Exit(workStatusLocker);
                exitCode = wee.ExitCode;
                SendMsg2Outter("任务即将退出:" + exitCode + " 信息:" + wee.ExitInfo);

            }
            catch (Exception ex)
            {
                exitCode = WorkExitCode.Exception;
                SendMsg2Outter("任务发生未知的程序异常:" + ex.Message);
                ChangeWorkStatus(JFWorkStatus.ExceptionExit);
            }
            finally
            {
               
                try
                {
                    OnStop();
                }
                catch (JFWorkExitException eeStop)
                {
                    switch (eeStop.ExitCode)
                    {
                        case WorkExitCode.Command://此时不会再接受指令
                            ;// ChangeWorkStatus(JFWorkStatus.CommandExit);
                            break;
                        case WorkExitCode.Normal:
                            ;// SendMsg2Outter("任务清理完成，退出运行");
                            break;
                        case WorkExitCode.Error:
                            JFHubCenter.Instance.StationMgr.OnStationLog(this, "OnStop发生错误:" + eeStop.Message, JFLogLevel.ERROR, LogMode.ShowRecord);
                            break;
                        case WorkExitCode.Exception:
                            JFHubCenter.Instance.StationMgr.OnStationLog(this, "OnStop发生异常:" + eeStop.Message, JFLogLevel.FATAL, LogMode.ShowRecord);
                            break;

                    }

                }
                catch (Exception exOnStop)
                {
                    JFHubCenter.Instance.StationMgr.OnStationLog(this, "OnStop未定义程序异常:" + exOnStop.Message, JFLogLevel.FATAL, LogMode.ShowRecord);

                }
                SendMsg2Outter("任务开始退出前清理");
                try
                {
                    CleanupWhenWorkExit();
                    SendMsg2Outter("任务清理完成，退出运行");
                }
                catch (JFWorkExitException exCleanWEE) //CleanupWhenWorkExit()中发生可异常
                {
                    switch (exCleanWEE.ExitCode)
                    {
                        case WorkExitCode.Command://此时不会再接受指令
                            ;// ChangeWorkStatus(JFWorkStatus.CommandExit);
                            break;
                        case WorkExitCode.Normal:
                            SendMsg2Outter("任务清理完成，退出运行");
                            break;
                        case WorkExitCode.Error:
                            JFHubCenter.Instance.StationMgr.OnStationLog(this, "清理流程发生错误:" + exCleanWEE.Message, JFLogLevel.ERROR, LogMode.ShowRecord);
                            break;
                        case WorkExitCode.Exception:
                            JFHubCenter.Instance.StationMgr.OnStationLog(this, "清理流程发生异常:" + exCleanWEE.Message, JFLogLevel.FATAL, LogMode.ShowRecord);
                            break;
                        
                    }
                }
                catch (Exception exCleanup)
                {
                    JFHubCenter.Instance.StationMgr.OnStationLog(this, "清理流程发生未定义程序异常:" + exCleanup.Message, JFLogLevel.FATAL, LogMode.ShowRecord);
                }




                switch (exitCode)
                {
                    case WorkExitCode.Command:
                        ChangeWorkStatus(JFWorkStatus.CommandExit);
                        break;
                    case WorkExitCode.Error:
                        ChangeWorkStatus(JFWorkStatus.ErrorExit);
                        break;
                    case WorkExitCode.Exception:
                        ChangeWorkStatus(JFWorkStatus.ExceptionExit);
                        break;
                    case WorkExitCode.Normal:
                        ChangeWorkStatus(JFWorkStatus.NormalEnd);
                        break;

                }
                _stationThreadWorkMode = StationThreadWorkMode.Normal;
            }

        }


        #region IJFRealtimeUIProvider's API
        UcStationRealtimeUI _ui = null;//new UcStationBaseRealtimeUI();
        public virtual JFRealtimeUI GetRealtimeUI()
        {
            return _ui;
        }
        #endregion

        #region IJFConfigUIProvider's API
        public virtual void ShowCfgDialog()
        {
            FormStationBaseCfg fm = new FormStationBaseCfg();
            fm.SetStation(this);
            fm.Text = "工站参数配置-" + Name;
            fm.ShowDialog();
        }

        public void Dispose()
        {
            try
            {
                if (Stop(1000) != JFWorkCmdResult.Success)
                    Abort();
            }
            catch
            {
                Abort();
            }
        }
        #endregion

        static object DefaultValueFromType(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);
            
            ConstructorInfo[] ctors = t.GetConstructors(System.Reflection.BindingFlags.Instance
                                                          | System.Reflection.BindingFlags.NonPublic
                                                          | System.Reflection.BindingFlags.Public);
            if (null == ctors)
                throw new Exception("CreateInstance(Type t) failed By: Not found t-Instance's Constructor");
            foreach (ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] ps = ctor.GetParameters();
                if (ps == null || ps.Length == 0)
                    return ctor.Invoke(null) as IJFInitializable;
            }

            return null;
        }

        internal static bool IsNullableType(Type type)
        {
            return !type.IsValueType;//return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        bool _isFirstLoadCfg = true;
        public virtual void LoadCfg()
        {
            _cfg.Load();
            if (!_cfg.ContainsItem("StationBasePrivateConfig"))
                _cfg.AddItem("StationBasePrivateConfig", new JFXmlDictionary<string,object>());
            JFXmlDictionary<string, object> baseCfg = _cfg.GetItemValue("StationBasePrivateConfig") as JFXmlDictionary<string, object>; //StationBase 专用配置

            if (!baseCfg.ContainsKey("DiNames"))
                baseCfg.Add("DiNames", new List<string>());//_cfg.AddItem("DiNames", new List<string>(), "BaseConfig");
            if (!baseCfg.ContainsKey("DoNames"))
                baseCfg.Add("DoNames", new List<string>());

            if (!baseCfg.ContainsKey("AiNames"))
                baseCfg.Add("AiNames", new List<string>());
            if (!baseCfg.ContainsKey("AoNames"))
                baseCfg.Add("AoNames", new List<string>());

            if (!baseCfg.ContainsKey("AxisNames"))
                baseCfg.Add("AxisNames", new List<string>());

            if (!baseCfg.ContainsKey("CmpTrigNames")) //比较触发器
                baseCfg.Add("CmpTrigNames", new List<string>());

            if (!baseCfg.ContainsKey("CameraNames"))
                baseCfg.Add("CameraNames", new List<string>());

            if (!baseCfg.ContainsKey("WorkPositions"))//工站的工作点位
                baseCfg.Add("WorkPositions", new List<JFMultiAxisPosition>());


            if (!baseCfg.ContainsKey("LightChannelNames"))//工站的光源通道名称
                baseCfg.Add("LightChannelNames", new List<string>());


            if (!baseCfg.ContainsKey("TrigChannelNames"))//工站的触发通道名称
                baseCfg.Add("TrigChannelNames", new JFXmlDictionary<string,string>());


            if (!baseCfg.ContainsKey("SystemDataPoolAliasNameMapping")) //系统数据池变量名称映射表
                baseCfg.Add("SystemDataPoolAliasNameMapping", new JFXmlDictionary<string, string>());



            ///工站声明的设备通道名称与全局的设备通道名称的映射关系表
            if (!baseCfg.ContainsKey("LocalDevChannelMap"))
                baseCfg.Add("LocalDevChannelMap", new JFXmlDictionary<NamedChnType, List<List<string>>>());
            JFXmlDictionary<NamedChnType, List<List<string>>> dictDevChnMap = baseCfg["LocalDevChannelMap"] as JFXmlDictionary<NamedChnType, List<List<string>>>; //配置中已经保存的映射表
            AdjustDevChnMap(dictDevChnMap);
            _cfg.Save();

            _dictMethodFlow.Clear();
            if (!baseCfg.ContainsKey("MethodFlows"))//工站中包含的所有所有动作流的序列化文本
                baseCfg.Add("MethodFlows", new List<string>());
 
            List<string> methodFlowsTxts = baseCfg["MethodFlows"] as List<string>;
            foreach (string txt in methodFlowsTxts)
            {
                JFMethodFlow mf = new JFMethodFlow();
                mf.FromTxt(txt);
                _dictMethodFlow.Add(mf.Name,mf);
                mf.SetStation(this);
            }
            ///加载声明的方法流配置
            foreach(string mfDecleared in _lstMethodFlowDecleared)
            {
                bool isMFExisted = false;
                foreach(string mfName in _dictMethodFlow.Keys)
                    if(mfName == mfDecleared)
                    {
                        isMFExisted = true;
                        break;
                    }
                if(!isMFExisted)
                {
                    JFMethodFlow mf = new JFMethodFlow();
                    mf.Name = mfDecleared;
                    _dictMethodFlow.Add(mfDecleared,mf);
                    mf.SetStation(this);
                    _cfg.Save();
                }
            }


            ///加载继承类申明的配置项
            foreach (KeyValuePair<string,object[]> kv in dictCfgParamDecleared)
            {
                string cfgName = kv.Key;
                string cfgCategory = (kv.Value as object[])[0] as string;
                Type cfgType = (kv.Value[1] as JFParamDescribe).ParamType;
                object defaultValue = DefaultValueFromType(cfgType);
                if (!_cfg.ContainsItem(cfgName))
                {
                    _cfg.AddItem(cfgName, defaultValue, cfgCategory);
                    if (_isFirstLoadCfg && kv.Value.Length > 2) //用户在构造函数中改变了值
                        _cfg.SetItemValue(cfgName, kv.Value[2]);
                }
                else //cfg中已包含声明参数项 ，检查是否合法
                {
                    string currCategory = _cfg.GetItemTag(cfgName);
                    if(currCategory != cfgCategory) //检查类别名称是否合法
                    {
                        _cfg.RemoveItem(cfgName);
                        _cfg.AddItem(cfgName, defaultValue, cfgCategory);
                        if (_isFirstLoadCfg && kv.Value.Length > 2) //用户在构造函数中改变了值
                            _cfg.SetItemValue(cfgName, kv.Value[2]);
                    }
                    else //检查当前值类型是否合法
                    {
                        object currValue = _cfg.GetItemValue(cfgName);
                        if(null != currValue)
                        {
                            if(cfgType != currValue.GetType())
                            {
                                _cfg.RemoveItem(cfgName);
                                _cfg.AddItem(cfgName, defaultValue, cfgCategory);
                                if (_isFirstLoadCfg && kv.Value.Length > 2) //用户在构造函数中改变了值
                                    _cfg.SetItemValue(cfgName, kv.Value[2]);
                            }
                        }
                        else
                        {
                            if(!IsNullableType(cfgType))
                            {
                                _cfg.RemoveItem(cfgName);
                                _cfg.AddItem(cfgName, defaultValue, cfgCategory);
                                if (_isFirstLoadCfg && kv.Value.Length > 2) //用户在构造函数中改变了值
                                    _cfg.SetItemValue(cfgName, kv.Value[2]);
                            }
                        }
                    }
                }
            }

            ///加载声明的工作点位配置
            List<JFMultiAxisPosition> wokPosInCfg = baseCfg["WorkPositions"] as List<JFMultiAxisPosition>;
            foreach (string wpName in _lstWorkPosDecleared)
            {
                bool isExistedWP = false;
                foreach(JFMultiAxisPosition wp in wokPosInCfg)
                    if(wp.Name == wpName)
                    {
                        isExistedWP = true;
                        break;
                    }
                if (!isExistedWP)
                {
                    JFMultiAxisPosition ap = new JFMultiAxisPosition();
                    ap.Name = wpName;
                    wokPosInCfg.Add(ap);
                    _cfg.Save();
                }
            }

            ///加载系统数据映射表
            _dictSysPoolItemNameMapping = baseCfg["SystemDataPoolAliasNameMapping"] as JFXmlDictionary<string, string>;
            foreach(string aliasName in _lstSysPoolItemAliasNames)
            {
                if(!_dictSysPoolItemNameMapping.ContainsKey(aliasName))
                    _dictSysPoolItemNameMapping.Add(aliasName, Name + ":" + aliasName);//添加一个默认的全局名称

                string realName = _dictSysPoolItemNameMapping[aliasName];
                if (!JFHubCenter.Instance.DataPool.ContainItem(realName)) //如果系统数据池中没有值项，则添加初始值
                    JFHubCenter.Instance.DataPool.RegistItem(realName, _dictSysPoolItemDecleared[aliasName][0] as Type, _dictSysPoolItemDecleared[aliasName][1]);
            }


            _isFirstLoadCfg = false;
        }


        /// <summary>
        /// 整理注册的通道映射表 ，剔除多余的部分，增加（未加入的已声明通道）
        /// </summary>
        /// <param name="devChnMapInCfg">已保存在配置文件中的映射表</param>
        void AdjustDevChnMap(JFXmlDictionary<NamedChnType, List<List<string>>> devChnMapInCfg)
        {
            if (_dictDeclearedDevChns.Keys.Count == 0)
            {
                devChnMapInCfg.Clear();
                return;
            }

            //先删除配置中多余的Key
            int keyCount = devChnMapInCfg.Keys.Count;
            while(keyCount > 0)
            {
                int index = 0;
                foreach (NamedChnType k in devChnMapInCfg.Keys)
                {
                    if (!_dictDeclearedDevChns.Keys.Contains(k))
                    {
                        devChnMapInCfg.Remove(k);
                        break;
                    }
                    else
                        index++;
                }
                if (keyCount == index)
                    break;
                keyCount = devChnMapInCfg.Keys.Count;
            }


            foreach (NamedChnType k in _dictDeclearedDevChns.Keys)
            {
                if (!devChnMapInCfg.ContainsKey(k))
                {
                    devChnMapInCfg.Add(k, new List<List<string>>());
                    List<string> devChnDecleared = _dictDeclearedDevChns[k];
                    foreach (string locDevChnName in devChnDecleared)
                    {
                        List<string> locAndGlobName = new List<string>();
                        locAndGlobName.Add(locDevChnName);
                        locAndGlobName.Add("");
                        devChnMapInCfg[k].Add(locAndGlobName);
                    }
                }
                else
                {
                    List<string> devChnDecleared = _dictDeclearedDevChns[k];
                    List<List<string>> devChnMap = devChnMapInCfg[k];

                    ///先删除多余的部分
                    int dcCount = devChnMap.Count;
                    while (dcCount > 0)
                    {
                        int index = 0;
                        foreach (List<string> dcm in devChnMap)
                        {
                            if (!devChnDecleared.Contains(dcm[0]))
                            {
                                devChnMap.Remove(dcm);
                                break;
                            }
                            else
                                index++;
                        }
                        if (index == dcCount)
                            break;
                        dcCount = devChnMap.Count;
                    }


                    //再添加缺少的部分
                    foreach (string locDevChnName in devChnDecleared) 
                    {
                        bool isFound = false;
                        foreach(List<string> kv in devChnMap)
                            if(kv[0] == locDevChnName)
                            {
                                isFound = true;
                                break;
                            }
                        if (!isFound)
                        {
                            List<string> locAndGlobNames = new List<string>();
                            locAndGlobNames.Add(locDevChnName);
                            locAndGlobNames.Add("");
                            devChnMap.Add(locAndGlobNames);
                        }

                    }

                   



                }

            }

        }

        public virtual void SaveCfg()
        {
            List<string> methodFlowTxts = new List<string>();
            foreach (JFMethodFlow mf in _dictMethodFlow.Values)
                methodFlowTxts.Add(mf.ToTxt());
            (_cfg.GetItemValue("StationBasePrivateConfig") as JFXmlDictionary<string,object>)["MethodFlows"]= methodFlowTxts;
            (_cfg.GetItemValue("StationBasePrivateConfig") as JFXmlDictionary<string, object>)["SystemDataPoolAliasNameMapping"] = _dictSysPoolItemNameMapping;
            _cfg.Save();
        }

        List<string> _SBCfg(string sbCfgName)// => (_cfg.GetItemValue("StationBasePrivateConfig") as JFXmlDictionary<string, object>)[sbCfgName] as List<string>;
        {
            if (null == _cfg)
                return new List<string>();
            if(!_cfg.ContainsItem("StationBasePrivateConfig")) //if (null == _cfg.GetItemValue("StationBasePrivateConfig"))
                return new List<string>();
            JFXmlDictionary<string, object> cfgItm = _cfg.GetItemValue("StationBasePrivateConfig") as JFXmlDictionary<string, object>;
            if (!cfgItm.ContainsKey(sbCfgName))
                return new List<string>();
            return cfgItm[sbCfgName] as List<string>;
           // return (_cfg.GetItemValue("StationBasePrivateConfig") as JFXmlDictionary<string, object>)[sbCfgName] as List<string>;
        }

        #region 工站所使用的设备（通道）

        /// <summary>
        ///  获取工站所用的数字量输入（通道）
        /// </summary>
        public string[] DINames 
        {
            get
            {
                return _SBCfg("DiNames").ToArray();
            }
        }

        public void AddDI(string diName)
        {
            if (string.IsNullOrEmpty(diName))
                throw new ArgumentNullException("AddDi(diName) failed by:diName is null or empty");
            List<string> diNames = _SBCfg("DiNames");//_cfg.GetItemValue("DiNames") as List<string>;
            if (diNames.Contains(diName))
                return;
            diNames.Add(diName);
        }

        public void RemoveDI(string diName)
        {
            List<string> diNames = _SBCfg("DiNames");//_cfg.GetItemValue("DiNames") as List<string>;
            if (!diNames.Contains(diName))
                return;
            diNames.Remove(diName);
        }

        public void ClearDI()
        {
            _SBCfg("DiNames").Clear();//(_cfg.GetItemValue("DiNames") as List<string>).Clear();
        }

        public string[] DONames
        {
            get
            {
                return _SBCfg("DoNames").ToArray();//(_cfg.GetItemValue("DoNames") as List<string>).ToArray();
            }
        }

        public void AddDO(string doName)
        {
            if (string.IsNullOrEmpty(doName))
                throw new ArgumentNullException("AddDo(doName) failed by:doName is null or empty");
            List<string> doNames = _SBCfg("DoNames");//_cfg.GetItemValue("DoNames") as List<string>;
            if (doNames.Contains(doName))
                return;
            doNames.Add(doName);
        }

        public void RemoveDO(string doName)
        {
            List<string> doNames = _SBCfg("DoNames");//_cfg.GetItemValue("DoNames") as List<string>;
            if (!doNames.Contains(doName))
                return;
            doNames.Remove(doName);
        }

        public void ClearDO()
        {
            _SBCfg("DoNames").Clear();
        }


        public string[] AINames
        {
            get
            {
                return _SBCfg("AiNames").ToArray();//(_cfg.GetItemValue("AiNames") as List<string>).ToArray();
            }
        }

        public void AddAI(string aiName)
        {
            if (string.IsNullOrEmpty(aiName))
                throw new ArgumentNullException("AddAi(aiName) failed by:aiName is null or empty");
            List<string> aiNames = _SBCfg("AiNames");//_cfg.GetItemValue("AiNames") as List<string>;
            if (aiNames.Contains(aiName))
                return;
            aiNames.Add(aiName);
        }

        public void RemoveAI(string aiName)
        {
            List<string> aiNames = _SBCfg("AiNames");//_cfg.GetItemValue("AiNames") as List<string>;
            if (!aiNames.Contains(aiName))
                return;
            aiNames.Remove(aiName);
        }

        public void ClearAI()
        {
            _SBCfg("AiNames").Clear();
        }



        public string[] AONames
        {
            get
            {
                return _SBCfg("AoNames").ToArray();//(_cfg.GetItemValue("AoNames") as List<string>).ToArray();
            }
        }

        public void AddAO(string aoName)
        {
            if (string.IsNullOrEmpty(aoName))
                throw new ArgumentNullException("AddAo(aoName) failed by:aoName is null or empty");
            List<string> aoNames = _SBCfg("AoNames");//_cfg.GetItemValue("AoNames") as List<string>;
            if (aoNames.Contains(aoName))
                return;
            aoNames.Add(aoName);
        }

        public void RemoveAO(string aoName)
        {
            List<string> aoNames = _SBCfg("AoNames");//_cfg.GetItemValue("AoNames") as List<string>;
            if (!aoNames.Contains(aoName))
                return;
            aoNames.Remove(aoName);
        }

        public void ClearAO()
        {
            _SBCfg("AoNames").Clear();//(_cfg.GetItemValue("AoNames") as List<string>).Clear();
        }


        public string[] AxisNames
        {
            get
            {
                return _SBCfg("AxisNames").ToArray();//(_cfg.GetItemValue("AxisNames") as List<string>).ToArray();
            }
        }

        public void AddAxis(string axisName)
        {
            if (string.IsNullOrEmpty(axisName))
                throw new ArgumentNullException("AddAxis(axisName) failed by:axisName is null or empty");
            List<string> axisNames = _SBCfg("AxisNames");//_cfg.GetItemValue("AxisNames") as List<string>;
            if (axisNames.Contains(axisName))
                return;
            axisNames.Add(axisName);
        }

        public void RemoveAxis(string axisName)
        {
            List<string> axisNames = _SBCfg("AxisNames");//_cfg.GetItemValue("AxisNames") as List<string>;
            if (!axisNames.Contains(axisName))
                return;
            axisNames.Remove(axisName);
        }

        public void ClearAxis()
        {
            _SBCfg("AxisNames").Clear();//(_cfg.GetItemValue("AxisNames") as List<string>).Clear();
        }

        public bool ContainAxis(string axisName)
        {
            return _SBCfg("AxisNames").Contains(axisName);//return (_cfg.GetItemValue("AxisNames") as List<string>).Contains(axisName);
        }
             


        public string[] CmpTrigNames
        {
            get
            {
                return _SBCfg("CmpTrigNames").ToArray();//(_cfg.GetItemValue("CmpTrigNames") as List<string>).ToArray();
            }
        }

        public void AddCmpTrig(string cmpTrigName)
        {
            if (string.IsNullOrEmpty(cmpTrigName))
                throw new ArgumentNullException("AddCmpTrig(cmpTrigName) failed by:cmpTrigName is null or empty");
            List<string> cmpTrigNames = _SBCfg("CmpTrigNames");//_cfg.GetItemValue("CmpTrigNames") as List<string>;
            if (cmpTrigNames.Contains(cmpTrigName))
                return;
            cmpTrigNames.Add(cmpTrigName);
        }

        public void RemoveCmpTrig(string cmpTrigName)
        {
            List<string> cmpTrigNames = _SBCfg("CmpTrigNames");//_cfg.GetItemValue("CmpTrigNames") as List<string>;
            if (!cmpTrigNames.Contains(cmpTrigName))
                return;
            cmpTrigNames.Remove(cmpTrigName);
        }

        public void ClearCmpTrig()
        {
            _SBCfg("CmpTrigNames").Clear();//(_cfg.GetItemValue("CmpTrigNames") as List<string>).Clear();
        }


        public string[] CameraNames
        {
            get
            {
                return _SBCfg("CameraNames").ToArray();//(_cfg.GetItemValue("CameraNames") as List<string>).ToArray();
            }
        }

        public void AddCamera(string cmrName)
        {
            if (string.IsNullOrEmpty(cmrName))
                throw new ArgumentNullException("AddCamera(cmrName) failed by:cmrName is null or empty");
            List<string> cmrNames = _SBCfg("CameraNames");//_cfg.GetItemValue("CameraNames") as List<string>;
            if (cmrNames.Contains(cmrName))
                return;
            cmrNames.Add(cmrName);
        }

        public void RemoveCamera(string cmrName)
        {
            List<string> cmrNames = _SBCfg("CameraNames");//_cfg.GetItemValue("CameraNames") as List<string>;
            if (!cmrNames.Contains(cmrName))
                return;
            cmrNames.Remove(cmrName);
        }

        public void ClearCamera()
        {
            _SBCfg("CameraNames").Clear();//(_cfg.GetItemValue("CameraNames") as List<string>).Clear();
        }

        #endregion

        public string[] WorkPositionNames
        {
            get
            {
                List<string> ret = new List<string>();
                List<JFMultiAxisPosition> allPos = WorkPositions;
                foreach (JFMultiAxisPosition pos in allPos)
                    ret.Add(pos.Name);
                return ret.ToArray();
            }
        }

        public bool ContianPositionName(string posName)
        {
            foreach (JFMultiAxisPosition pos in WorkPositions)
                if (pos.Name == posName)
                    return true;
            return false;
        }

        public JFMultiAxisPosition GetWorkPosition(string name)
        {
            List<JFMultiAxisPosition> allPos = WorkPositions;
            foreach (JFMultiAxisPosition pos in allPos)
                if (pos.Name == name)
                    return pos;
            return null;
        }

        public void AddWorkPosition(JFMultiAxisPosition maPos)
        {
            if (null == maPos || string.IsNullOrEmpty(maPos.Name))
                throw new ArgumentNullException("SetWorkPosition(maPos) failed by:maPos == null or maPos.Name is null or empty");
            List<JFMultiAxisPosition> allPos = WorkPositions;
            for(int i = 0; i < allPos.Count;i++)
                if (allPos[i].Name == maPos.Name)
                {
                    allPos[i] = maPos;
                    return;
                }
            allPos.Add(maPos);
        }

        /// <summary>
        /// 移除工作点位
        /// </summary>
        /// <param name="name"></param>
        public void RemoveWorkPosition(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            List<JFMultiAxisPosition> allPos = WorkPositions;
            for (int i = 0; i < allPos.Count; i++)
                if (allPos[i].Name == name)
                {
                    allPos.RemoveAt(i);
                    return ;
                }
        }

        /// <summary>
        /// 移动到工作点位posName （非插补模式）
        /// </summary>
        /// <param name="posName"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool MoveToWorkPosition(string posName,out string errorInfo)
        {
            errorInfo = "Success";
            string[] workPosNames = WorkPositionNames;
            if (WorkPositionNames == null)
            {
                errorInfo = "无工作点位Name = " + posName;
                return false;
            }
            bool isExisted = false;
            foreach(string workPosName in workPosNames)
                if(workPosName == posName)
                {
                    isExisted = true;
                    break;
                }
            if(!isExisted)
            {
                errorInfo = " 工作点位Name = \"" + posName + "\"不存在";
                return false;
            }

            JFMultiAxisPosition pos = GetWorkPosition(posName);
            return MoveToPosition(pos, out errorInfo);

        }




        /// <summary>
        /// 检查轴通道(设备)是否存在(可用)
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        JFDevCellInfo CheckAxisDevInfo(string axisName,out string errorInfo)
        {
            if (!ContainAxis(axisName))
            {
                errorInfo = "工站不包含轴，Name = \"" + axisName + "\"";
                return null;
            }
            JFDevCellInfo ci = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(axisName); //在命名表中的通道信息
            if (null == ci)
            {
                errorInfo = "未找到轴:\"" + axisName + "\"设备信息";
                return null;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq;
            if (null == dev)
            {
                errorInfo = "未找到轴:\"" + axisName + "\"所属设备:\"" + ci.DeviceID + "\"";
                return null;
            }
            if (!dev.IsDeviceOpen)
            {
                errorInfo = "轴:\"" + axisName + "\"所属设备:\"" + ci.DeviceID + "\"未打开";
                return null;
            }
            if (ci.ModuleIndex >= dev.McCount)
            {
                errorInfo = "轴:\"" + axisName + "\"模块序号:\"" + ci.ModuleIndex + "\"超出限制!";
                return null;
            }
            IJFModule_Motion md = dev.GetMc(ci.ModuleIndex);
            if (ci.ChannelIndex >= md.AxisCount)
            {
                errorInfo = "轴:\"" + axisName + "\"通道序号:\"" + ci.ChannelIndex + "\"超出限制!";
                return null;
            }

            errorInfo = "";
            return ci;
        }

        bool CheckAxisCanMove(string axisName, out string errorInfo)
        {
            JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errorInfo);
            if (null == ci)
                return false;
            
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            return CheckAxisCanMove(md, ci.ChannelIndex, out errorInfo);
        }

        /// <summary>
        /// 清楚轴报警
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool AxisClearAlarm(string axisName,out string errorInfo)
        {
            JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errorInfo);
            if (null == ci)
            {
                errorInfo = "轴:\"" + axisName + "\" 清除报警失败,ErrorInfo:" + errorInfo;
                return false;
            }
            int errCode = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).ClearAlarm(ci.ChannelIndex);
            if (errCode != 0)
            {
                errorInfo = "轴:\"" + axisName + "\"清除报警失败,ErrorInfo:" + (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).GetErrorInfo(errCode);
                return false;
            }
            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 轴清除报警 （替身名）
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool AxisClearAlarmAlias(string aliasName, out string errorInfo)
        {
            string gAxisName = GetDecChnGlobName(NamedChnType.Axis, aliasName);
            if(string.IsNullOrEmpty(gAxisName))
            {
                errorInfo = "清除轴报警失败，替身名:\"" + aliasName + "\"  未指定轴通道名称";
                return false;
            }

            bool ret = AxisClearAlarm(gAxisName, out errorInfo);
            if (!ret)
                errorInfo = "轴替身:\"" +aliasName + "\" "+ errorInfo;
            return ret;
        }



        /// <summary>
        /// 打开所有轴设备,消除报警,伺服使能
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool EnableAllAxis(out string errorInfo)
        {
            string[] allAxisNames = AxisNames;//工站包含的所有电机
            if(null == allAxisNames || 0 == allAxisNames.Length)
            {
                errorInfo = "Success:工站不包含电机轴";
                return true;
            }
            foreach(string axisName in allAxisNames)
            {
                //打开电机所属设备
                if(!OpenChnDevice(NamedChnType.Axis, axisName,out errorInfo))
                {
                    errorInfo = "使能工站所有轴电机失败:" + errorInfo;
                    return false;
                }
                IJFDevice dev = null;
                JFDevCellInfo ci = null;
                if(!JFDevChannel.CheckChannel(JFDevCellType.Axis, axisName,out dev,out ci ,out errorInfo))
                {
                    errorInfo = "使能工站所有轴电机失败:" + errorInfo;
                    return false;
                }
                IJFModule_Motion md = (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
                //电机消除警报
                int nRet = md.ClearAlarm(ci.ChannelIndex);
                if (0!= nRet)
                {
                    errorInfo = "使能工站所有轴电机失败:未能消除电机报警:innerError = " + md.GetErrorInfo(nRet);
                    return false;
                }
                
                //电机伺服上电
                nRet = md.ServoOn(ci.ChannelIndex);
                if (0 != nRet)
                {
                    errorInfo = "使能工站所有轴电机失败:电机伺服使能失败:innerError = " + md.GetErrorInfo(nRet);
                    return false;
                }
                //上电之后再消除一次报警
                nRet = md.ClearAlarm(ci.ChannelIndex);
                if (0 != nRet)
                {
                    errorInfo = "使能工站所有轴电机失败:未能消除电机报警:innerError = " + md.GetErrorInfo(nRet);
                    return false;
                }
            }

            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 打开所有光源设备(不转换触发模式)
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool OpenAllLightDev(out string errorInfo)
        {
            string[] allLightNames = LightChannelNames;//工站包含的所有光源（同道）名称
            if (null == allLightNames || 0 == allLightNames.Length)
            {
                errorInfo = "Success:工站不包含光源（同道）";
                return true;
            }
            foreach (string lightName in allLightNames)
            {
                if (!OpenChnDevice(NamedChnType.Light, lightName, out errorInfo))
                {
                    errorInfo = "使能工站所有光源通道失败:" + errorInfo;
                    return false;
                }
            }

            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 打开工站中所有命名通道所属的设备
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool OpenAllDevices(out string errorInfo)
        {
            StringBuilder sbErrorInfo = new StringBuilder();
            bool ret = true;
            string optErrorInfo;


            //打开所有di设备
            //先检查所有设备通道的替身名都绑定
            if (_dictDeclearedDevChns.ContainsKey(NamedChnType.Di))
            {
                List<string> allAliasNames = _dictDeclearedDevChns[NamedChnType.Di];
                foreach (string s in allAliasNames)
                {
                    string gName = GetDecChnGlobName(NamedChnType.Di, s);
                    if (string.IsNullOrEmpty(gName))
                    {
                        ret = false;
                        sbErrorInfo.AppendLine(NamedChnType.Di.ToString() + " 替身名:\"" + s + "\"未绑定全局通道名称");
                    }
                    else
                    {
                        //检测gName 核发性
                    }
                }
            }

            string[] allChnNames = DINames;
            if (null != allChnNames)
                foreach (string s in allChnNames)
                    if (!OpenChnDevice(NamedChnType.Di, s, out optErrorInfo))
                    {
                        sbErrorInfo.AppendLine(optErrorInfo);
                        ret = false;
                    }

            //打开所有do设备
            allChnNames = DONames;
            if(null != allChnNames)
                foreach(string s in allChnNames)
                    if(!OpenChnDevice(NamedChnType.Do,s,out optErrorInfo))
                    {
                        sbErrorInfo.AppendLine(optErrorInfo);
                        ret = false;
                    }

            //打开所有轴设备
            allChnNames = AxisNames;
            if (null != allChnNames)
                foreach (string s in allChnNames)
                    if (!OpenChnDevice(NamedChnType.Axis, s, out optErrorInfo))
                    {
                        sbErrorInfo.AppendLine(optErrorInfo);
                        ret = false;
                    }

            //打开所有光源控制器设备
            allChnNames = LightChannelNames;
            if (null != allChnNames)
                foreach (string s in allChnNames)
                    if (!OpenChnDevice(NamedChnType.Light, s, out optErrorInfo))
                    {
                        sbErrorInfo.AppendLine(optErrorInfo);
                        ret = false;
                    }

            //打开所有触发器设备(光源触发强度控制)
            allChnNames = TrigChannelNames;
            if (null != allChnNames)
                foreach (string s in allChnNames)
                    if (!OpenChnDevice(NamedChnType.Trig, s, out optErrorInfo))
                    {
                        sbErrorInfo.AppendLine(optErrorInfo);
                        ret = false;
                    }

            //打开所有位置比较触发设备(伺服轴位置比较触发)
            allChnNames = CmpTrigNames;
            if (null != allChnNames)
                foreach (string s in allChnNames)
                    if (!OpenChnDevice(NamedChnType.CmpTrig, s, out optErrorInfo))
                    {
                        sbErrorInfo.AppendLine(optErrorInfo);
                        ret = false;
                    }

            //打开所有相机设备
            allChnNames = CameraNames;
            if (null != allChnNames)
                foreach (string s in allChnNames)
                    if (!OpenChnDevice(NamedChnType.Camera, s, out optErrorInfo))
                    {
                        sbErrorInfo.AppendLine(optErrorInfo);
                        ret = false;
                    }

            //打开所有AI设备
            allChnNames = AINames;
            if (null != allChnNames)
                foreach (string s in allChnNames)
                    if (!OpenChnDevice(NamedChnType.Ai, s, out optErrorInfo))
                    {
                        sbErrorInfo.AppendLine(optErrorInfo);
                        ret = false;
                    }

            //打开所有AO设备
            allChnNames = AONames;
            if (null != allChnNames)
                foreach (string s in allChnNames)
                    if (!OpenChnDevice(NamedChnType.Ao, s, out optErrorInfo))
                    {
                        sbErrorInfo.AppendLine(optErrorInfo);
                        ret = false;
                    }


            if (ret)
                errorInfo = "Success";
            else
                errorInfo = sbErrorInfo.ToString();
            return ret;
        }



        /// <summary>
        /// 转换光源工作模式（并使能光源通道）
        /// </summary>
        /// <param name="lightName"></param>
        /// <param name="mode"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetLightTrigMode(string lightName,JFLightWithTrigWorkMode mode,out string errorInfo)
        {
            IJFDevice dev = null;
            JFDevCellInfo ci = null;
            if (!JFDevChannel.CheckChannel(JFDevCellType.Light, lightName, out dev, out ci, out errorInfo))
            {
                errorInfo = "设置光源模式失败，Name:\"" + lightName +"\"" + " ErrorInfo:" + errorInfo;
                return false;
            }

            if(!(dev is IJFDevice_LightControllerWithTrig)) //不带触发功能的光源控制器
            {
                if(mode == JFLightWithTrigWorkMode.TurnOnOff)
                {
                    errorInfo = "Success";
                    return true;
                }
                else
                {
                    errorInfo = "设置光源模式失败，Name:\"" + lightName + "\"" + " 所属控制器不支持触发模式";
                    return false;
                }
            }

            IJFDevice_LightControllerWithTrig lightCtrl = dev as IJFDevice_LightControllerWithTrig;
            int ret = lightCtrl.SetWorkMode(mode);
            if(ret != 0)
            {
                errorInfo = "设置光源模式失败，Name:\"" + lightName + "\"" + " ErrorInfo:" + lightCtrl.GetErrorInfo(ret);
                return false;
            }

            //设置光源模式后，使能通道
            if (mode == JFLightWithTrigWorkMode.TurnOnOff)
            {
                ret = lightCtrl.SetLightChannelEnable(ci.ChannelIndex, true);
                if(ret != 0)
                {
                    errorInfo = "设置光源模式失败，Name:\"" + lightName + "\"" + " ErrorInfo:" + lightCtrl.GetErrorInfo(ret);
                    return false;
                }
            }
            else
            {
                ret = lightCtrl.SetTrigChannelEnable(ci.ChannelIndex,true);
                if (ret != 0)
                {
                    errorInfo = "设置光源模式失败，Name:\"" + lightName + "\"" + " ErrorInfo:" + lightCtrl.GetErrorInfo(ret);
                    return false;
                }
            }
            errorInfo = "Success";
            return true;
        }


        /// <summary>
        /// 转换光源工作模式(替身名) , （并使能光源通道）
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="mode"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetLightTrigModeAlias(string aliasName, JFLightWithTrigWorkMode mode, out string errorInfo)
        {
            if (!IsDevChnDecleared(NamedChnType.Light, aliasName))
            {
                errorInfo = "设置光源模式失败，Alias:\"" + aliasName + "\"不存在！";
                return false;
            }
            string globName = GetDecChnGlobName(NamedChnType.Light, aliasName);
            if(string.IsNullOrEmpty(globName))
            {
                errorInfo = "设置光源模式失败，Alias:\"" + aliasName + "\"" + " 未绑定全局通道名"; 
                return false;
            }

            if(!SetLightTrigMode(globName,mode,out errorInfo))
            {
                errorInfo = "设置光源模式失败，Alias:\"" + aliasName + "\"->" + errorInfo;
                return false;
            }

            return true;
        }

 


        /// <summary>
        /// 打开/关闭所有相机设备
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool EnableAllCmrDev(bool isOpen,out string errorInfo)
        {
            string[] allCmrNames = CameraNames;
            if(null == allCmrNames || 0 == allCmrNames.Length)
            {
                errorInfo = "Success:工站不包含相机设备";
                return true;
            }
            JFInitorManager mgr = JFHubCenter.Instance.InitorManager;
            foreach (string s in allCmrNames)
            {
                IJFInitializable initor = mgr.GetInitor(s);
                if(null == initor)
                {
                    errorInfo = "设备列表中未包含相机:" + s;
                    return false;
                }

                IJFDevice_Camera cmr = initor as IJFDevice_Camera;
                if(null == cmr)
                {
                    errorInfo = "设备列表中未包含相机:" + s + ",InitorName = \"" + s + "\" realType = " + initor.GetType().Name;
                    return false;
                }
                int ret = 0;
                if (isOpen)
                    ret = cmr.OpenDevice();
                else
                    ret = cmr.CloseDevice();
                if(ret != 0)
                {
                    errorInfo = "相机:\"" + s + "\"" + (isOpen?"打开":"关闭") + "失败，ErrorInfo:" + cmr.GetErrorInfo(ret);
                    return false;
                }
            }
            errorInfo = "Success";
            return true;

        }

        /// <summary>
        /// 设置相机触发模式
        /// </summary>
        /// <param name="cmrName"></param>
        /// <param name="mode"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetCmrTrigMode(string cmrName, JFCmrTrigMode mode, out string errorInfo)
        {
            JFInitorManager mgr = JFHubCenter.Instance.InitorManager;
            IJFInitializable initor = mgr.GetInitor(cmrName);
            if (null == initor)
            {
                errorInfo = "设置相机触发模式失败,设备列表中未包含相机:" + cmrName;
                return false;
            }

            IJFDevice_Camera cmr = initor as IJFDevice_Camera;
            if (null == cmr)
            {
                errorInfo = "设置相机触发模式失败,设备列表中未包含相机:" + cmrName + ",Initor's realType = " + initor.GetType().Name;
                return false;
            }

            int ret = cmr.SetTrigMode(mode);
            if (ret != 0)
            {
                errorInfo = "设置相机触发模式失败:Cmr = \"" + cmrName + "\",ErrorInfo = " + cmr.GetErrorInfo(ret);
                return false;
            }
            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 设置相机触发模式（替身名）
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="mode"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetCmrTrigModeAlias(string aliasName, JFCmrTrigMode mode, out string errorInfo)
        {
            if (!IsDevChnDecleared(NamedChnType.Camera, aliasName))
            {
                errorInfo = "设置相机触发模式失败，Alias:\"" + aliasName + "\"不存在！";
                return false;
            }
            string globName = GetDecChnGlobName(NamedChnType.Camera, aliasName);
            if (string.IsNullOrEmpty(globName))
            {
                errorInfo = "设置相机触发模式失败，Alias:\"" + aliasName + "\"" + " 未绑定全局设备名";
                return false;
            }

            if(!SetCmrTrigMode(globName,mode,out errorInfo))
            {
                errorInfo = "设置相机触发模式失败，Alias:\"" + aliasName + "\"->" + errorInfo;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 打开/关闭 相机图像采集
        /// </summary>
        /// <param name="cmrName"></param>
        /// <param name="isStart"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool EnableCmrGrab(string cmrName,bool isStart,out string errorInfo)
        {
            JFInitorManager mgr = JFHubCenter.Instance.InitorManager;
            IJFInitializable initor = mgr.GetInitor(cmrName);
            if (null == initor)
            {
                errorInfo = (isStart?"打开":"关闭") + "相机采集失败,设备列表中未包含相机:" + cmrName;
                return false;
            }

            IJFDevice_Camera cmr = initor as IJFDevice_Camera;
            if (null == cmr)
            {
                errorInfo = (isStart ? "打开" : "关闭") + "相机采集失败,设备列表中未包含相机:" + cmrName + ",Initor's realType = " + initor.GetType().Name;
                return false;
            }
            int ret = 0;
            if (isStart)
                ret = cmr.StartGrab();
            else
                ret = cmr.StopGrab();
            if (ret != 0)
            {
                errorInfo = (isStart ? "打开" : "关闭") + "相机采集失败:Cmr = \"" + cmrName + "\",ErrorInfo = " + cmr.GetErrorInfo(ret);
                return false;
            }
            errorInfo = "Success";
            return true;
        }

        public bool EnableCmrGrabAlias(string aliasName, bool isStart, out string errorInfo)
        {
            if (!IsDevChnDecleared(NamedChnType.Camera, aliasName))
            {
                errorInfo = (isStart ? "打开" : "关闭") + "相机采集失败，Alias:\"" + aliasName + "\"不存在！";
                return false;
            }
            string globName = GetDecChnGlobName(NamedChnType.Camera, aliasName);
            if (string.IsNullOrEmpty(globName))
            {
                errorInfo = (isStart ? "打开" : "关闭") + "相机采集失败，Alias:\"" + aliasName + "\"" + " 未绑定全局设备名";
                return false;
            }

            if (!EnableCmrGrab(globName, isStart, out errorInfo))
            {
                errorInfo = (isStart ? "打开" : "关闭") + "相机采集失败，Alias:\"" + aliasName + "\"->" + errorInfo;
                return false;
            }

            return true;
        }


        /// <summary>
        /// 设置相机曝光值
        /// </summary>
        /// <param name="cmrName"></param>
        /// <param name="microSeconds"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetCmrExposure(string cmrName, double microSeconds,out string errorInfo)
        {
            JFInitorManager mgr = JFHubCenter.Instance.InitorManager;
            IJFInitializable initor = mgr.GetInitor(cmrName);
            if (null == initor)
            {
                errorInfo = "设置相机曝光参数失败,设备列表中未包含相机:" + cmrName;
                return false;
            }

            IJFDevice_Camera cmr = initor as IJFDevice_Camera;
            if (null == cmr)
            {
                errorInfo = "设置相机曝光参数失败,设备列表中未包含相机:" + cmrName + ",Initor's realType = " + initor.GetType().Name;
                return false;
            }

            int ret = cmr.SetExposure(microSeconds);
            if (ret != 0)
            {
                errorInfo = "设置相机曝光参数失败:Cmr = \"" + cmrName + "\",ErrorInfo = " + cmr.GetErrorInfo(ret);
                return false;
            }
            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 设置相机曝光值（替身名）
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="microSeconds"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetCmrExposureAlias(string aliasName, double microSeconds, out string errorInfo)
        {
            if (!IsDevChnDecleared(NamedChnType.Camera, aliasName))
            {
                errorInfo = "设置相机曝光参数失败，Alias:\"" + aliasName + "\"不存在！";
                return false;
            }
            string globName = GetDecChnGlobName(NamedChnType.Camera, aliasName);
            if (string.IsNullOrEmpty(globName))
            {
                errorInfo = "设置相机曝光参数失败，Alias:\"" + aliasName + "\"" + " 未绑定全局设备名";
                return false;
            }

            if (!SetCmrExposure(globName, microSeconds, out errorInfo))
            {
                errorInfo = "设置相机曝光参数失败，Alias:\"" + aliasName + "\"->" + errorInfo;
                return false;
            }

            return true;
        }



        /// <summary>
        /// 设置相机增益参数
        /// </summary>
        /// <param name="cmrName"></param>
        /// <param name="gain"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetCmrGain(string cmrName, double gain, out string errorInfo)
        {
            JFInitorManager mgr = JFHubCenter.Instance.InitorManager;
            IJFInitializable initor = mgr.GetInitor(cmrName);
            if (null == initor)
            {
                errorInfo = "设置相机增益参数失败,设备列表中未包含相机:" + cmrName;
                return false;
            }

            IJFDevice_Camera cmr = initor as IJFDevice_Camera;
            if (null == cmr)
            {
                errorInfo = "设置相机增益参数失败,设备列表中未包含相机:" + cmrName + ",Initor's realType = " + initor.GetType().Name;
                return false;
            }

            int ret = cmr.SetGain(gain);
            if (ret != 0)
            {
                errorInfo = "设置相机增益参数失败:Cmr = \"" + cmrName + "\",ErrorInfo = " + cmr.GetErrorInfo(ret);
                return false;
            }
            errorInfo = "Success";
            return true;
        }


        /// <summary>
        /// 设置相机增益参数（替身名）
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="gan"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetCmrGainAlias(string aliasName, double gain, out string errorInfo)
        {
            if (!IsDevChnDecleared(NamedChnType.Camera, aliasName))
            {
                errorInfo = "设置相机增益参数失败，Alias:\"" + aliasName + "\"不存在！";
                return false;
            }
            string globName = GetDecChnGlobName(NamedChnType.Camera, aliasName);
            if (string.IsNullOrEmpty(globName))
            {
                errorInfo = "设置相机增益参数失败，Alias:\"" + aliasName + "\"" + " 未绑定全局设备名";
                return false;
            }

            if (!SetCmrGain(globName, gain, out errorInfo))
            {
                errorInfo = "设置相机增益参数失败，Alias:\"" + aliasName + "\"->" + errorInfo;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 软件采集一幅相机图片
        /// </summary>
        /// <returns></returns>
        public bool SnapCmrImage(string cmrName,out IJFImage img,out string errorInfo)
        {
            JFInitorManager mgr = JFHubCenter.Instance.InitorManager;
            IJFInitializable initor = mgr.GetInitor(cmrName);
            if (null == initor)
            {
                img = null;
                errorInfo = "采图失败,设备列表中未包含相机:" + cmrName;
                return false;
            }

            IJFDevice_Camera cmr = initor as IJFDevice_Camera;
            if (null == cmr)
            {
                img = null;
                errorInfo = "采图失败,设备列表中未包含相机:" + cmrName + ",Initor's realType = " + initor.GetType().Name;
                return false;
            }

            int ret = cmr.GrabOne(out img, 1000);
            if(ret != 0)
            {
                errorInfo = "采图失败,Cmr = \"" + cmrName + "\", ErrorInfo:" + cmr.GetErrorInfo(ret);
                return false;
            }

            errorInfo = "Success";
            return true;
        }


        public bool SnapCmrImageAlias(string aliasName, out IJFImage img, out string errorInfo)
        {
            if (!IsDevChnDecleared(NamedChnType.Camera, aliasName))
            {
                img = null;
                errorInfo = "相机采图失败，Alias:\"" + aliasName + "\"不存在！";
                return false;
            }
            string globName = GetDecChnGlobName(NamedChnType.Camera, aliasName);
            if (string.IsNullOrEmpty(globName))
            {
                img = null;
                errorInfo = "相机采图失败，Alias:\"" + aliasName + "\"" + " 未绑定全局设备名";
                return false;
            }

            if (!SnapCmrImage(globName,out img,out errorInfo))
            {
                errorInfo = "相机采图失败，Alias:\"" + aliasName + "\"->" + errorInfo;
                return false;
            }

            return true;
        }



        /// <summary>
        /// 将设备当前状态调整为指定的(单相机)视觉参数
        /// </summary>
        /// <param name="cmrName"></param>
        /// <param name="visionCfgName"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool AdjustSingleVisionCfg(string cmrName, string visionCfgName, out string errorInfo)
        {
            JFInitorManager mgr = JFHubCenter.Instance.InitorManager;
            IJFInitializable initor = mgr.GetInitor(cmrName);
            if (null == initor)
            {
                errorInfo = "调整视觉参数失败,设备列表中未包含相机:" + cmrName;
                return false;
            }

            IJFDevice_Camera cmr = initor as IJFDevice_Camera;
            if (null == cmr)
            {
                errorInfo = "调整视觉参数失败,设备列表中未包含相机:" + cmrName + ",Initor's realType = " + initor.GetType().Name;
                return false;
            }

            JFVisionManager vm = JFHubCenter.Instance.VisionMgr;
            if(!vm.ContainSingleVisionCfgByName(visionCfgName))
            {
                errorInfo = "调整视觉参数失败,视觉配置参数不存在，CfgName = " + visionCfgName;
                return false;
            }
            int ret = 0;
            JFSingleVisionCfgParam vCfg = vm.GetSingleVisionCfgByName(visionCfgName);
            bool currRevX;
            cmr.GetReverseX(out currRevX);
            if (currRevX != vCfg.CmrReverseX)
            {
                ret = cmr.SetReverseX(vCfg.CmrReverseX);
                if (ret != 0)
                {
                    errorInfo = "调整视觉参数失败,CfgName = " + visionCfgName + " SetReverseX failed,ErrorInfo = " + cmr.GetErrorInfo(ret);
                    return false;
                }
            }
            bool currRevY;
            cmr.GetReverseY(out currRevY);
            if (currRevY != vCfg.CmrReverseY)
            {

                ret = cmr.SetReverseY(vCfg.CmrReverseY);
                if (ret != 0)
                {
                    errorInfo = "调整视觉参数失败,CfgName = " + visionCfgName + " SetReverseY failed,ErrorInfo = " + cmr.GetErrorInfo(ret);
                    return false;
                }
            }

            ret = cmr.SetExposure(vCfg.CmrExposure);
            if (ret != 0)
            {
                errorInfo = "调整视觉参数失败,CfgName = " + visionCfgName + " SetExposure failed,ErrorInfo = " + cmr.GetErrorInfo(ret);
                return false;
            }


            ret = cmr.SetGain(vCfg.CmrGain);
            if (ret != 0)
            {
                errorInfo = "调整视觉参数失败,CfgName = " + visionCfgName + " SetGain failed,ErrorInfo = " + cmr.GetErrorInfo(ret);
                return false;
            }

            //设置光源参数
            if(vCfg.LightChnNames != null)
                for(int i = 0;i < vCfg.LightChnNames.Length;i++)
                {
                    string ln = vCfg.LightChnNames[i];
                    if (!SetLightIntensity(ln, vCfg.LightIntensities[i], out errorInfo))
                    {
                        errorInfo = "调整视觉参数失败,CfgName = " + visionCfgName + "->" + errorInfo;
                        return false;
                    }
                }

            //设置轴高度
            string[] axisNames = vCfg.AxisNames;
            if (null != axisNames && axisNames.Length > 0)
            {
                bool isOK = true;
                int j = 0;
                string innerError = "";
                for (j = 0; j < axisNames.Length; j++)
                {
                    if (!MoveAxis(axisNames[j],vCfg.AxisPositions[j],true,out innerError))
                    {
                        isOK = false;
                        break;
                    }
                }
                if(!isOK)
                {
                    for (int k = 0; k < j; k++) //
                        StopAxis(axisNames[k]);
                    errorInfo = "调整视觉参数失败:" + innerError;
                    return false;
                }
            }
            errorInfo = "Success";
            return true;
        }

        /// <summary>
        /// 将设备当前状态调整为指定的(单相机)视觉参数(相机名为替身名)
        /// </summary>
        /// <param name="cmrAliasName"></param>
        /// <param name="visionCfgName"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool AdjustSingleVisionCfgAlias(string cmrAliasName, string visionCfgName, out string errorInfo)
        {
            if (!IsDevChnDecleared(NamedChnType.Camera, cmrAliasName))
            {
                errorInfo = "调整视觉参数失败，AliasCmr:\"" + cmrAliasName + "\"不存在！";
                return false;
            }
            string globName = GetDecChnGlobName(NamedChnType.Camera, cmrAliasName);
            if (string.IsNullOrEmpty(globName))
            {
                errorInfo = "调整视觉参数失败，AliasCmr:\"" + cmrAliasName + "\"" + " 未绑定全局设备名";
                return false;
            }

            if (!AdjustSingleVisionCfg(globName, visionCfgName, out errorInfo))
            {
                errorInfo = "调整视觉参数失败，AliasCmr:\"" + cmrAliasName + "\"->" + errorInfo;
                return false;
            }

            return true;
        }




        /// <summary>
        /// 等待设置视觉参数完成，必须和AdjustSingleVisionCfg/Alias 配对使用
        /// 主要是等待配置轴到位
        /// </summary>
        /// <param name="visionCfgName"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public JFWorkCmdResult WaitSingleVisionCfgAdjustDone(string visionCfgName,out string errorInfo,int timeoutMilliseconds = -1)
        {
            JFVisionManager vm = JFHubCenter.Instance.VisionMgr;
            if (!vm.ContainSingleVisionCfgByName(visionCfgName))
            {
                errorInfo = "等待调整视觉参数失败,视觉配置参数不存在，CfgName = " + visionCfgName;
                return JFWorkCmdResult.UnknownError;
            }
            JFSingleVisionCfgParam vCfg = vm.GetSingleVisionCfgByName(visionCfgName);
            string[] axisNames = vCfg.AxisNames;
            if (null == axisNames || axisNames.Length == 0)
            {
                errorInfo = "Success";
                return JFWorkCmdResult.Success;
            }
            DateTime startTime = DateTime.Now;
            while(true)
            {
                bool isOK = true;
                for (int i = 0; i < axisNames.Length; i++)
                {
                    JFWorkCmdResult ret = WaitMotionDone(axisNames[i], 0);
                    if (ret == JFWorkCmdResult.Timeout)
                    {
                        isOK = false;
                        break;
                    }
                    else if (ret != JFWorkCmdResult.Success)
                    {
                        errorInfo = "等待轴:\"" + axisNames[i] + "\"出错，ErrorCode = " + ret;
                        return ret;
                    }
                }
                if (isOK)
                {
                    errorInfo = "Success";
                    return JFWorkCmdResult.Success;
                   
                }
                TimeSpan ts = DateTime.Now - startTime;
                if (timeoutMilliseconds >= 0 && ts.TotalMilliseconds >= timeoutMilliseconds)
                {
                    errorInfo = "等待调整视觉参数超时，CfgName = " + visionCfgName;
                    return JFWorkCmdResult.Timeout;
                }
            }


        }




        /// <summary>
        /// 打开所有DIO设备
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool OpenAllDioDev(out string errorInfo)
        {
            string[] allDoNames = DONames;//工站包含的所有电机
            if (null != allDoNames)
                foreach (string doName in allDoNames)
                {
                    //打开电机所属设备
                    if (!OpenChnDevice(NamedChnType.Do, doName, out errorInfo))
                    {
                        errorInfo = "使能工站所有DIO失败:" + errorInfo;
                        return false;
                    }
                }
            string[] allDINames = DINames;
            if(null != allDINames)
            foreach (string diName in allDINames)
            {
                //打开电机所属设备
                if (!OpenChnDevice(NamedChnType.Di, diName, out errorInfo))
                {
                    errorInfo = "使能工站所有DIO失败:" + errorInfo;
                    return false;
                }
            }

            errorInfo = "Success";
            return true;
        }



        public bool AxisServo(string axisName,bool isServOn,out string errorInfo)
        {
            JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errorInfo);
            if (null == ci)
            {
                errorInfo = "轴:\"" + axisName + "\"伺服" + (isServOn ? "使能" : "去使能") + "失败,ErrorInfo:" + errorInfo;
                return false;
            }
            int errCode = 0;
            if (isServOn)
                errCode = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).ServoOn(ci.ChannelIndex);
            else
                errCode = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).ServoOff(ci.ChannelIndex);
            if(errCode != 0)
            {
                errorInfo = "轴:\"" + axisName + "\"伺服" + (isServOn ? "使能" : "去使能") + "失败,ErrorInfo:" + (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).GetErrorInfo(errCode);
                return false;
            }
            errorInfo = "Success";
            return true;
        }


        /// <summary>
        /// 工站方法，操作（站内声明的）轴伺服上电/断电
        /// </summary>
        /// <param name="axisLocName">站内声明的轴ID</param>
        /// <param name="isServOn"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool AxisServoByAlias(string aliasName, bool isServOn, out string errorInfo)
        {
            errorInfo = "Unknown-Error";
            if (string.IsNullOrEmpty(aliasName))
            {
                errorInfo = "站内轴名称参数为空";
                return false;
            }
            if(!IsDevChnDecleared(NamedChnType.Axis, aliasName))
            {
                errorInfo = "站内轴名称 =\"" + aliasName + "\"不是工站固有轴";
                return false;
            }
            
            string globAxisID = GetDecChnGlobName(NamedChnType.Axis, aliasName);
            if(string.IsNullOrEmpty(globAxisID))
            {
                errorInfo = "站内轴名称 =\"" + aliasName + "\"未绑定全局轴ID";
                return false;
            }

            string innerErrorInfo;
            if(!AxisServo(globAxisID,isServOn,out innerErrorInfo))
            {
                errorInfo = "站内轴:\"" + aliasName + "\"->" + innerErrorInfo;
                return false;
            }

            errorInfo = "Success";
            return true;
        }


        /// <summary>
        /// 轴电机归零
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool AxisHome(string axisName,out string errorInfo)
        {
            errorInfo = "Success";
            JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errorInfo);
            if (null == ci)
            {
                errorInfo = "轴归零失败，Name = \"" + axisName + "\",ErrorInfo:" + errorInfo;
                return false;
            }

            if (!CheckAxisCanMove(axisName, out errorInfo))
            {
                errorInfo = "轴归零失败，Name = \"" + axisName + "\",ErrorInfo:" + errorInfo;
                return false;
            }
            int errCode = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).Home(ci.ChannelIndex);

            if (errCode != 0)
            {
                errorInfo = "轴归零失败，Name = \"" + axisName + "\",ErrorInfo:" + (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).GetErrorInfo(errCode);
                return false;
            }
            return true;
        }


        public bool AxisHomeByAlias(string aliasName, out string errorInfo)
        {
            string axisName = GetDecChnGlobName(NamedChnType.Axis, aliasName);
            if (string.IsNullOrEmpty(axisName))
            {
                errorInfo = "轴归零运动失败，AliasName = \"" + aliasName + "\",未指定轴ID";
            }
            bool ret = AxisHome(axisName, out errorInfo);
            if (!ret)
                errorInfo = "AliasName:\"" + aliasName + "\"" + errorInfo;
            return ret;
        }


        public JFWorkCmdResult WaitAxisHomeDone(string axisName,int timeoutMilliSeconds = -1)
        {
            string errorInfo = "";
            JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errorInfo);
            if (null == ci)
                return JFWorkCmdResult.UnknownError;
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            DateTime startTime = DateTime.Now;
            bool isDone = false;
            while(true)
            {

                if (0 != md.IsHomeDone(ci.ChannelIndex, out isDone))
                    return JFWorkCmdResult.UnknownError;
                if (isDone)
                    return JFWorkCmdResult.Success;
                if(timeoutMilliSeconds >= 0)
                {
                    TimeSpan ts = DateTime.Now - startTime;
                    if (ts.TotalMilliseconds >= timeoutMilliSeconds)
                        return JFWorkCmdResult.Timeout;
                }
                CheckCmd(CycleMilliseconds);//响应退出指令
            }

        }

        public JFWorkCmdResult WaitAxisHomeDoneByAlias(string aliasName, int timeoutMilliSeconds = -1)
        {
            string axisName = GetDecChnGlobName(NamedChnType.Axis, aliasName);
            if (string.IsNullOrEmpty(axisName))
            {
                return JFWorkCmdResult.UnknownError;
            }
            return WaitAxisHomeDone(axisName, timeoutMilliSeconds);
        }



        bool CheckAxisCanMove(IJFModule_Motion md ,int axisIndex,out string errorInfo)
        {
            bool[] axisStatus;
            int errRet = md.GetMotionStatus(axisIndex, out axisStatus);
            if (0 != errRet)
            {
                errorInfo = "获取轴状态失败";
                               return false;
            }
            bool isServOn = md.IsSVO(axisIndex); //测试查看
            if (!axisStatus[md.MSID_SVO])
            {
                errorInfo = "轴伺服未使能";
                                return false;
            }
            if (axisStatus[md.MSID_ALM])
            {
                errorInfo = "轴已报警";
                return false;
            }
            
            ////////////////////////////////////////////
            ///景焱控制卡在轴到达限位停止后，即使停了也不会反馈MDN或INP信号
            //if (!axisStatus[md.MSID_MDN])
            //{
            //    errorInfo = "轴运动未完成";
            //    return false;
            //}

            //if (!axisStatus[md.MSID_INP])
            //{
            //    errorInfo = "轴运动未到位";
            //    return false;
            //}

            errorInfo = "";
            return true;
        }

        /// <summary>
        /// 以速度模式移动指定轴（全局ID）
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="isPositive"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool MoveVelAxis(string axisName,bool isPositive,out string errorInfo )
        {

            JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errorInfo);
            if (null == ci)
            {
                errorInfo = "轴速度运动失败，Name = \"" + axisName + "\",ErrorInfo:" + errorInfo;
                return false;
            }

            if (!CheckAxisCanMove(axisName, out errorInfo))
            {
                errorInfo = "轴速度运动失败，Name = \"" + axisName + "\",ErrorInfo:" + errorInfo;
                return false;
            }
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            bool[] mcStatus = null;
            int nRet = md.GetMotionStatus(ci.ChannelIndex, out mcStatus);
            if(0 != nRet)
            {
                errorInfo = "轴速度运动失败，Name = \"" + axisName + "\",未能获取轴当前运动状态:";
                return false;
            }
            if(isPositive )
            {
                if(mcStatus[md.MSID_PL])
                {
                    errorInfo = "Success";
                    return true;
                }

                if(md.MSID_SPL >-1 && mcStatus[md.MSID_SPL]) //轴电机支持软正限位
                {
                    errorInfo = "Success:软正限位被触发";
                    return true;
                }


            }
            else
            {
                if (mcStatus[md.MSID_NL])
                {
                    errorInfo = "Success";
                    return true;
                }

                if (md.MSID_SNL > -1 && mcStatus[md.MSID_SNL]) //轴电机支持软负限位
                {
                    errorInfo = "Success:软负限位被触发";
                    return true;
                }
            }

            int errCode = md.Jog(ci.ChannelIndex,isPositive);

            if (errCode != 0)
            {
                errorInfo = "轴速移动度失败，Name = \"" + axisName + "\",ErrorInfo:" + md.GetErrorInfo(errCode);
                return false;
            }
            errorInfo = "Success";
            return true;
        }


        /// <summary>
        /// 以速度模式移动指定轴（替身ID）
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="isPositive"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool MoveVelAxisByAlias(string aliasName, bool isPositive, out string errorInfo)
        {
            string axisName = GetDecChnGlobName(NamedChnType.Axis, aliasName);
            if(string.IsNullOrEmpty(axisName))
            {
                errorInfo = "轴速度运动失败，AliasName = \"" + aliasName + "\",未指定轴ID" ;
            }
            bool ret = MoveVelAxis(axisName, isPositive, out errorInfo);
            if (!ret)
                errorInfo = "AliasName:\"" + aliasName + "\"" + errorInfo;
            return ret;
        }



        public bool MoveToPosition(JFMultiAxisPosition pos, out string errorInfo)
        {
            errorInfo = "Success";
            string[] axisNamesInPos = pos.AxisNames;
            if (null == axisNamesInPos || 0 == axisNamesInPos.Length)
            {
                errorInfo = "点位中不包含轴/电机";
                return false;
            }
            
            string[] AxisNamesInStation = AxisNames;
            if(AxisNamesInStation == null || AxisNamesInStation.Length == 0)
            {
                errorInfo = "工站配置中没有轴/电机";
                return false;
            }
            List<JFDevCellInfo> lstAxisCIs = new List<JFDevCellInfo>();
            foreach(string axisNameInPos in axisNamesInPos)
            {
                JFDevCellInfo ci = CheckAxisDevInfo(axisNameInPos, out errorInfo);
                if (null == ci)
                {
                    errorInfo = errorInfo = "轴移动失败 Name = \"" + axisNameInPos + "\",ErrorInfo :" + errorInfo;
                    return false;
                }
                lstAxisCIs.Add(ci);
            }

            bool isAllAxisOK = true;
            int i = 0;
            for(i = 0; i < lstAxisCIs.Count;i++)
            {
                IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(lstAxisCIs[i].DeviceID) as IJFDevice_MotionDaq).GetMc(lstAxisCIs[i].ModuleIndex);
                if(!CheckAxisCanMove(md, lstAxisCIs[i].ChannelIndex,out errorInfo))
                {
                    errorInfo = "轴移动失败 Name = \"" + axisNamesInPos[i] + "\",ErrorInfo :" + errorInfo;
                    isAllAxisOK = false;
                    break;
                }
              
                int errCode =md.AbsMove(lstAxisCIs[i].ChannelIndex, pos.GetAxisPos(axisNamesInPos[i]));
                if (0 != errCode)
                {
                    errorInfo = "轴移动失败 Name = \"" + axisNamesInPos[i] + "\",ErrorInfo :" + md.GetErrorInfo(errCode);
                    isAllAxisOK = false;
                    break;
                }
            }
            if(!isAllAxisOK)
            {
                for (int j = 0; j < i; j++)
                    (JFHubCenter.Instance.InitorManager.GetInitor(lstAxisCIs[j].DeviceID) as IJFDevice_MotionDaq).GetMc(lstAxisCIs[j].ModuleIndex).StopAxis(lstAxisCIs[j].ChannelIndex);
                return false;
            }
            return true;
            
        }


        public bool WaitToPosition(JFMultiAxisPosition pos, out string errorInfo,int timeoutMilliSeconds = -1)
        {
            //errorInfo = "Success";
            string[] axisNamesInPos = pos.AxisNames;
            if (null == axisNamesInPos || 0 == axisNamesInPos.Length)
            {
                errorInfo = "点位中不包含轴/电机";
                return false;
            }

            string[] AxisNamesInStation = AxisNames;
            if (AxisNamesInStation == null || AxisNamesInStation.Length == 0)
            {
                errorInfo = "工站配置中没有轴/电机";
                return false;
            }
            List<JFDevCellInfo> lstAxisCIs = new List<JFDevCellInfo>();
            List<IJFModule_Motion> lstMD = new List<IJFModule_Motion>();
            foreach (string axisNameInPos in axisNamesInPos)
            {
                JFDevCellInfo ci = CheckAxisDevInfo(axisNameInPos, out errorInfo);
                if (null == ci)
                {
                    errorInfo = errorInfo = "等待轴运动到点位失败,轴检测错误: Name = \"" + axisNameInPos + "\",ErrorInfo :" + errorInfo;
                    return false;
                }
                lstAxisCIs.Add(ci);
                lstMD.Add((JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex));
            }

            DateTime startTime = DateTime.Now;
            while(true)
            {
                bool isAllAxisDone = true;
                for (int i = 0; i < lstAxisCIs.Count; i++)
                {
                    bool[] mcStatus = null; //轴所有运动状态
                    int nOpt = lstMD[i].GetMotionStatus(lstAxisCIs[i].ChannelIndex, out mcStatus);
                    if(nOpt != 0)
                    {
                        errorInfo = "等待轴运动到点位失败,获取轴状态失败:" + lstMD[i].GetErrorInfo(nOpt);
                        return false;
                    }

                    if(lstMD[i].MSID_ALM >= 0 && mcStatus[lstMD[i].MSID_ALM]) //轴电机报警
                    {
                        errorInfo = "等待轴运动到点位失败,轴电机:\"" + axisNamesInPos[i] + "\"报警!";
                        return false;
                    }

                    if(lstMD[i].MSID_SVO >=0 && !mcStatus[lstMD[i].MSID_SVO]) //轴电机失电
                    {
                        errorInfo = "等待轴运动到点位失败,轴电机:\"" + axisNamesInPos[i] + "\"伺服失电!";
                        return false;
                    }

                    if(lstMD[i].MSID_EMG >=0 )//轴急停被触发
                    {
                        if (mcStatus[lstMD[i].MSID_EMG])
                        {
                            errorInfo = "等待轴运动到点位失败,轴电机:\"" + axisNamesInPos[i] + "\"急停被触发!";
                            return false;
                        }
                    }

                    if(!mcStatus[lstMD[i].MSID_MDN])
                    {
                        isAllAxisDone = false;
                        break;
                    }

                }

                if(isAllAxisDone)
                {
                    errorInfo = "Success";
                    return true;
                }

                if(timeoutMilliSeconds >= 0)
                {
                    TimeSpan ts = DateTime.Now - startTime;
                    if(ts.TotalMilliseconds >= timeoutMilliSeconds)
                    {
                        errorInfo = "等待轴运动到点位失败,超时未完成";
                        return false;
                    }
                }

                CheckCmd(CycleMilliseconds);
            }
        }



        public JFWorkCmdResult WaitMotionDone(string axisName,int milliSenconds = -1)
        {
            string errorInfo = "";
            JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errorInfo);
            if (null == ci)
                return JFWorkCmdResult.UnknownError;

            DateTime startTime = DateTime.Now;
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            while (true)
            {
                if(md.IsMDN(ci.ChannelIndex))
                {
                    errorInfo = "Success";
                    return JFWorkCmdResult.Success;
                }
                if(IsInWorkThread())
                    CheckCmd(CycleMilliseconds);
                if (milliSenconds >= 0)
                {
                    TimeSpan ts = DateTime.Now - startTime;
                    if (milliSenconds >= 0 && ts.TotalMilliseconds >= milliSenconds)
                        return JFWorkCmdResult.Timeout;
                }
                
            }


        }


        public JFWorkCmdResult WaitMotionDoneByAlias(string axisAliasName, int milliSenconds = -1)
        {
            string gName = GetDecChnGlobName(NamedChnType.Axis, axisAliasName);
            return WaitMotionDone(gName, milliSenconds);
        }

        /// <summary>
        /// 同时等待多个轴运动完成信号
        /// </summary>
        /// <param name="axisNames"></param>
        /// <param name="milliSeconds"></param>
        /// <returns></returns>
        public JFWorkCmdResult WaitMotionDones(string[] axisNames,int milliSeconds = -1)
        {
            if (null == axisNames || 0 == axisNames.Length)
                throw new ArgumentNullException();
            string errorInfo = "";
            JFDevCellInfo[] cis = new JFDevCellInfo[axisNames.Length];//
            IJFModule_Motion[] mds = new IJFModule_Motion[axisNames.Length];
            for (int i = 0; i < axisNames.Length; i++)
            {
                cis[i] = CheckAxisDevInfo(axisNames[i], out errorInfo);
                if (null == cis)
                    return JFWorkCmdResult.UnknownError;
                mds[i] = (JFHubCenter.Instance.InitorManager.GetInitor(cis[i].DeviceID) as IJFDevice_MotionDaq).GetMc(cis[i].ModuleIndex);
            }
            DateTime startTime = DateTime.Now;
            while (true)
            {
                if (IsInWorkThread())
                    CheckCmd(CycleMilliseconds);
                else
                    Thread.Sleep(CycleMilliseconds);

                bool isAllDone = true;
                for(int i = 0; i < cis.Length;i++)
                {
                    if (!mds[i].IsMDN(cis[i].ChannelIndex))
                    {
                        isAllDone = false;
                        break;
                    }
                }

                if (isAllDone)
                    return JFWorkCmdResult.Success;
                
                
                if (milliSeconds >= 0)
                {
                    TimeSpan ts = DateTime.Now - startTime;
                    if ( ts.TotalMilliseconds >= milliSeconds)
                        return JFWorkCmdResult.Timeout;
                }

            }
        }

        /// <summary>
        /// 同时等待多个轴运动完成信号（替身名）
        /// </summary>
        /// <returns></returns>
        public JFWorkCmdResult WaitMotionDonesByAlias(string[] aliasNames, int milliSeconds = -1)
        {
            if (null == aliasNames || 0 == aliasNames.Length)
                return JFWorkCmdResult.UnknownError;
            string[] gns = new string[aliasNames.Length];
            for (int i = 0; i < aliasNames.Length; i++)
            {
                gns[i] = GetDecChnGlobName(NamedChnType.Axis, aliasNames[i]);
                if (string.IsNullOrEmpty(gns[i]))
                    return JFWorkCmdResult.UnknownError;
            }
            return WaitMotionDones(gns, milliSeconds);
        }

        public bool WaitToWorkPosition(string posName, out string errorInfo, int timeoutMilliSeconds = -1)
        {
            string[] workPosNames = WorkPositionNames;
            if (WorkPositionNames == null || !WorkPositionNames.Contains(posName))
            {
                errorInfo = "等待到工作点位失败:无工作点位Name = " + posName;
                return false;
            }

            JFMultiAxisPosition pos = GetWorkPosition(posName);
            bool ret = WaitToPosition(pos, out errorInfo, timeoutMilliSeconds);
            if(!ret)
            {
                errorInfo = "等待到运动点位失败，点位名称:\"" + posName + "\",Error:" + errorInfo;
                return false;
            }

            errorInfo = "Success";
            return true;
        }


        /// <summary>
        /// 等待轴电机到达限位
        /// 由于景焱控制卡电机到达限位无MotionDone信号
        /// </summary>
        /// <param name="axisName">轴名称（全局ID）</param>
        /// <param name="isPLimit">正限位 （如果此值为false，表示等待负限位）</param>
        /// <param name="milliSenconds"></param>
        /// <returns></returns>
        public JFWorkCmdResult WaitAxis2Limit(string axisName, bool isPLimit, int milliSenconds = -1)
        {
            string errorInfo = "";
            JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errorInfo);
            if (null == ci)
                return JFWorkCmdResult.UnknownError;

            DateTime startTime = DateTime.Now;
            IJFModule_Motion md = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex);
            while (true)
            {
                if(isPLimit)
                {
                   if( md.IsPL(ci.ChannelIndex))
                    {
                        errorInfo = "Success";
                        return JFWorkCmdResult.Success;
                    }
                }
                else
                {
                    if (md.IsNL(ci.ChannelIndex))
                    {
                        errorInfo = "Success";
                        return JFWorkCmdResult.Success;
                    }
                }
                
                if (IsInWorkThread())
                    CheckCmd(CycleMilliseconds);
                if (milliSenconds >= 0)
                {
                    TimeSpan ts = DateTime.Now - startTime;
                    if (milliSenconds >= 0 && ts.TotalMilliseconds >= milliSenconds)
                        return JFWorkCmdResult.Timeout;
                }

            }


        }



        public JFWorkCmdResult WaitAxis2LimitByAlias(string aliasName, bool isPLimit, int milliSenconds = -1)
        {
            string axisName = GetDecChnGlobName(NamedChnType.Axis, aliasName) ;
            if(string.IsNullOrEmpty(axisName))
                return JFWorkCmdResult.UnknownError;

            return WaitAxis2Limit(axisName, isPLimit, milliSenconds); 


        }


        public bool LineToWorkPosition(string posName, out string errorInfo)
        {
            if (WorkPositionNames == null)
            {
                errorInfo = "无工作点位Name = " + posName;
                return false;
            }
            string[] workPosNames = WorkPositionNames;
            bool isExisted = false;
            foreach (string workPosName in workPosNames)
                if (workPosName == posName)
                {
                    isExisted = true;
                    break;
                }
            if (!isExisted)
            {
                errorInfo = " 工作点位Name = \"" + posName + "\"不存在";
                return false;
            }

            JFMultiAxisPosition pos = GetWorkPosition(posName);
            return LineToPosition(pos, out errorInfo);
        }

        /// <summary>
        /// 直线插补到某一个点位
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool LineToPosition(JFMultiAxisPosition pos, out string errorInfo)
        {
            errorInfo = "Success";
            string[] axisNamesInPos = pos.AxisNames;
            if (null == axisNamesInPos || 0 == axisNamesInPos.Length)
            {
                errorInfo = "点位中不包含轴/电机";
                return false;
            }
            string[] AxisNamesInStation = AxisNames;
            if (AxisNamesInStation == null || AxisNamesInStation.Length == 0)
            {
                errorInfo = "工站配置中没有轴/电机";
                return false;
            }
            List<JFDevCellInfo> lstAxisCIs = new List<JFDevCellInfo>();
            foreach (string axisNameInPos in axisNamesInPos)
            {
                JFDevCellInfo ci = CheckAxisDevInfo(axisNameInPos, out errorInfo);
                if (null == ci)
                {
                     errorInfo = "轴移动失败 Name = \"" + axisNameInPos + "\",ErrorInfo :" + errorInfo;
                    return false;
                }
                if(0 == lstAxisCIs.Count)
                    lstAxisCIs.Add(ci);
                else
                {
                    foreach(JFDevCellInfo ciExist in lstAxisCIs)
                        if(ciExist.DeviceID != ci.DeviceID || ciExist.ModuleIndex != ci.ModuleIndex)
                        {
                            errorInfo = "插补运动失败 Name = \"" + axisNameInPos + "\",与其他轴不在一个设备/模块中"; 
                            return false;
                        }
                    lstAxisCIs.Add(ci);
                }
                if(!CheckAxisCanMove(axisNameInPos,out errorInfo))
                {
                    errorInfo = "插补运动失败 轴名 = \"" + axisNameInPos + "\" ErrorInfo:" + errorInfo;
                    return false;
                }
            }
            List<int> axisIndexes = new List<int>();
            List<double> axisPoses = new List<double>();
            for(int i = 0; i < pos.Positions.Count;i++)
            {
                axisIndexes.Add(lstAxisCIs[i].ChannelIndex);
                axisPoses.Add(pos.Positions[i].Position);
            }
            int errCode = (JFHubCenter.Instance.InitorManager.GetInitor(lstAxisCIs[0].DeviceID) as IJFDevice_MotionDaq).GetMc(lstAxisCIs[0].ModuleIndex).AbsLine(axisIndexes.ToArray(), axisPoses.ToArray());
            if(errCode != 0)
            {
                errorInfo = "轴插补失败，ErrorInfo:" + (JFHubCenter.Instance.InitorManager.GetInitor(lstAxisCIs[0].DeviceID) as IJFDevice_MotionDaq).GetMc(lstAxisCIs[0].ModuleIndex).GetErrorInfo(errCode);
                return false;
            }
            return true;

        }


        /// <summary>
        /// 单轴运动(PTP)
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="pos"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool MoveAxis(string axisName,double pos,bool isAbs,out string errorInfo)
        {
            errorInfo = "Success";
            JFDevCellInfo ci = CheckAxisDevInfo(axisName, out errorInfo);
            if(null == ci)
            {
                errorInfo = "轴移动失败，Name = \"" + axisName + "\",ErrorInfo:" + errorInfo;
                return false;
            }

            if(!CheckAxisCanMove(axisName,out errorInfo))
            {
                errorInfo = "轴移动失败，Name = \"" + axisName + "\",ErrorInfo:" + errorInfo;
                return false;
            }
            int errCode = 0;
            if (isAbs)
                errCode = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).AbsMove(ci.ChannelIndex, pos);
            else
                errCode = (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).RelMove(ci.ChannelIndex, pos);

            if (errCode != 0)
            {
                errorInfo = "轴移动失败，Name = \"" + axisName + "\",ErrorInfo:" + (JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).GetErrorInfo(errCode);
                return false;
            }
            return true;
        }


        public bool MoveAxisByAlias(string axisAliasName, double pos, bool isAbs, out string errorInfo)
        {
            if(string.IsNullOrEmpty(axisAliasName))
            {
                errorInfo = "MoveAxisByAlias(axisAliasName...) failed by axisAliasName is null or empty";
                return false;
            }

            string gName = GetDecChnGlobName(NamedChnType.Axis, axisAliasName);
            if(string.IsNullOrEmpty(gName))
            {
                errorInfo = "MoveAxisByAlias(axisAliasName = \"" + axisAliasName + "\" failed by UnBind Global Channel";
                return false; 
            }
            return MoveAxis(gName, pos, isAbs, out errorInfo);
        }


        public List<JFMultiAxisPosition> WorkPositions { get { return (_cfg.GetItemValue("StationBasePrivateConfig") as JFXmlDictionary<string,object>)["WorkPositions"] as List<JFMultiAxisPosition>; } }
        
        public string[] WorkFlowNames
        {
            get
            {
                return _dictMethodFlow.Keys.ToArray();
            }
        }

        
        public bool ContainWorkFlow(string wfName)
        {
            return _dictMethodFlow.ContainsKey(wfName);
        }

        public JFMethodFlow GetWorkFlow(string mfName)
        {
            if (!_dictMethodFlow.ContainsKey(mfName))
                return null;
            return _dictMethodFlow[mfName];
        }

        public bool AddWorkFlow(JFMethodFlow mf)
        {
            if (null == mf)
                return false;
            if (_dictMethodFlow.ContainsKey(mf.Name))
                return false;
            if (_dictMethodFlow.ContainsValue(mf))
                return false;

            _dictMethodFlow.Add(mf.Name, mf);
            mf.SetStation(this);
            return true;
        }

        public void RemoveWorkFlow(string mfName)
        {
            if (!_dictMethodFlow.ContainsKey(mfName))
                return;
            _dictMethodFlow.Remove(mfName);
            return;
        }

        /// <summary>
        /// 获取轴的运动参数:速度
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="spd"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool GetAxisMPSpeed(string axisName,out double spd,out string errorInfo)
        {
            spd = 0;
            errorInfo = "Unknown-Error";
            JFDevCellInfo axisCellInfo = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(axisName);
            if (null == axisCellInfo)
            {
                errorInfo = string.Format("轴名称:\"{0}\"在配置表中不存在！", axisName);
                return false;
            }

            if (!JFHubCenter.Instance.InitorManager.ContainID(axisCellInfo.DeviceID))
            {
                errorInfo = string.Format("轴:\"{0}\"所属设备\"{1}\"在设备管理器中不存在!", axisName, axisCellInfo.DeviceID);
                return false;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(axisCellInfo.DeviceID) as IJFDevice_MotionDaq;
            if (!dev.IsDeviceOpen)
            {
                errorInfo = string.Format("轴:\"{0}\"所属设备\"{1}\"未打开!", axisName, axisCellInfo.DeviceID);
                return false;
            }
            if (axisCellInfo.ModuleIndex >= dev.McCount)
            {
                errorInfo = string.Format("轴:\"{0}\"所属模块序号\"{1}超出限制:0~{2}\"", axisName, axisCellInfo.ModuleIndex, dev.McCount - 1);
                return false;
            }

            IJFModule_Motion motion = dev.GetMc(axisCellInfo.ModuleIndex);
            if (axisCellInfo.ChannelIndex >= motion.AxisCount)
            {
                errorInfo = string.Format("轴:\"{0}\"通道序号\"{1}超出限制:0~{2}\"", axisName, axisCellInfo.ChannelIndex, motion.AxisCount - 1);
                return false;
            }


            JFMotionParam mp;

            int errorCode = motion.GetMotionParam(axisCellInfo.ChannelIndex,out mp);
            if (0 == errorCode)
            {
                errorInfo = "Success";
                spd = mp.vm;
                return true;
            }

            errorInfo = string.Format("获取轴:\"{0}\"速度失败：ErrorInfo:{1}", axisName, motion.GetErrorInfo(errorCode));
            return false;
        }

        /// <summary>
        /// 获取轴的运动参数:速度（使用替身名）
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="spd"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool GetAxisMPSpeedByAlias(string aliasName, out double spd, out string errorInfo)
        {
            errorInfo = "Unknown error";
            spd = 0;
            string gName = GetDecChnGlobName(NamedChnType.Axis, aliasName);
            if(string.IsNullOrEmpty(gName))
            {
                errorInfo = "替身名：" + aliasName + "未绑定通道设备";
                return false;
            }
            return GetAxisMPSpeed(gName, out spd, out errorInfo);

        }

        /// <summary>
        /// 设置轴运行速度
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="spd"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetAxisMPSpeed(string axisName,  double spd, out string errorInfo)
        {
            //spd = 0;
            errorInfo = "Unknown-Error";
            JFDevCellInfo axisCellInfo = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(axisName);
            if (null == axisCellInfo)
            {
                errorInfo = string.Format("轴名称:\"{0}\"在配置表中不存在！", axisName);
                return false;
            }

            if (!JFHubCenter.Instance.InitorManager.ContainID(axisCellInfo.DeviceID))
            {
                errorInfo = string.Format("轴:\"{0}\"所属设备\"{1}\"在设备管理器中不存在!", axisName, axisCellInfo.DeviceID);
                return false;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(axisCellInfo.DeviceID) as IJFDevice_MotionDaq;
            if (!dev.IsDeviceOpen)
            {
                errorInfo = string.Format("轴:\"{0}\"所属设备\"{1}\"未打开!", axisName, axisCellInfo.DeviceID);
                return false;
            }
            if (axisCellInfo.ModuleIndex >= dev.McCount)
            {
                errorInfo = string.Format("轴:\"{0}\"所属模块序号\"{1}超出限制:0~{2}\"", axisName, axisCellInfo.ModuleIndex, dev.McCount - 1);
                return false;
            }

            IJFModule_Motion motion = dev.GetMc(axisCellInfo.ModuleIndex);
            if (axisCellInfo.ChannelIndex >= motion.AxisCount)
            {
                errorInfo = string.Format("轴:\"{0}\"通道序号\"{1}超出限制:0~{2}\"", axisName, axisCellInfo.ChannelIndex, motion.AxisCount - 1);
                return false;
            }


            JFMotionParam mp;

            int errorCode = motion.GetMotionParam(axisCellInfo.ChannelIndex, out mp);
            if (0 != errorCode)
            {
                errorInfo = string.Format("获取轴:\"{0}\"运动参数失败：ErrorInfo:{1}", axisName, motion.GetErrorInfo(errorCode));
                return false;
            }
            mp.vm = spd;
            errorCode = motion.SetMotionParam(axisCellInfo.ChannelIndex, mp);
            if(0 != errorCode)
            {
                errorInfo = string.Format("设置轴:\"{0}\"运动参数失败：ErrorInfo:{1}", axisName, motion.GetErrorInfo(errorCode));
                return false;
            }
            errorInfo = "Success";
            return true;

        }

        /// <summary>
        /// 设置轴运行速度（替身名）
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="spd"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetAxisMPSpeedByAlias(string aliasName, double spd, out string errorInfo)
        {
            string gName = GetDecChnGlobName(NamedChnType.Axis, aliasName);
            if(string.IsNullOrEmpty(gName))
            {
                errorInfo = "替身名：" + aliasName + "未绑定通道设备";
                return false;
            }
            return SetAxisMPSpeed(gName, spd, out errorInfo);
        }



        
        

        /// <summary>
        /// 获取轴(电机)当前的实际位置
        /// </summary>
        public bool GetAxisPosition(string axisName,out double dPos,out string errorInfo)
        {
            dPos = 0;
            errorInfo = "Unknown-Error";
            JFDevCellInfo axisCellInfo = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(axisName);
            if(null == axisCellInfo)
            {
                errorInfo = string.Format("轴名称:\"{0}\"在配置表中不存在！", axisName);
                return false;
            }

            if(!JFHubCenter.Instance.InitorManager.ContainID(axisCellInfo.DeviceID))
            {
                errorInfo = string.Format("轴:\"{0}\"所属设备\"{1}\"在设备管理器中不存在!", axisName, axisCellInfo.DeviceID);
                return false;
            }
            IJFDevice_MotionDaq dev = JFHubCenter.Instance.InitorManager.GetInitor(axisCellInfo.DeviceID) as IJFDevice_MotionDaq;
            if(!dev.IsDeviceOpen)
            {
                errorInfo = string.Format("轴:\"{0}\"所属设备\"{1}\"未打开!", axisName, axisCellInfo.DeviceID);
                return false;
            }
            if(axisCellInfo.ModuleIndex >= dev.McCount)
            {
                errorInfo = string.Format("轴:\"{0}\"所属模块序号\"{1}超出限制:0~{2}\"", axisName, axisCellInfo.ModuleIndex, dev.McCount-1);
                return false;
            }

            IJFModule_Motion motion = dev.GetMc(axisCellInfo.ModuleIndex);
            if(axisCellInfo.ChannelIndex >= motion.AxisCount)
            {
                errorInfo = string.Format("轴:\"{0}\"通道序号\"{1}超出限制:0~{2}\"", axisName, axisCellInfo.ChannelIndex, motion.AxisCount - 1);
                return false;
            }

            int errorCode = motion.GetFbkPos(axisCellInfo.ChannelIndex, out dPos);
            if (0 == errorCode)
            {
                errorInfo = "Success";
                return true;
            }

            errorInfo = string.Format("获取轴:\"{0}\"位置失败：ErrorInfo:{1}", axisName, motion.GetErrorInfo(errorCode));
            return false;
        }


        //public abstract string[] InternalMethodFlowNames();

        //FormStationBase _stationForm = new FormStationBase();

        /// <summary>
        /// 用于单独调试工站的窗口界面(不建议以Dialog模式显示)
        /// </summary>
        /// <returns></returns>
        public virtual Form GenForm()
        {
            //get 
            {
                //_stationForm.SetStation(this); 
                //return _stationForm; 
                FormStationBaseDebug fm = new FormStationBaseDebug();
                fm.SetStation(this);
                return fm;
            }
        }

        /// <summary>
        /// 工站包含的所有光源通道名称
        /// </summary>
        public string[] LightChannelNames 
        { 
            get
            {
                return _SBCfg("LightChannelNames").ToArray();//return (_cfg.GetItemValue("LightChannelNames") as List<string>).ToArray();
            }
        }

        public void AddLightChannel(string chnName)
        {
            if (string.IsNullOrEmpty(chnName))
                throw new ArgumentNullException("AddLightChannel(chnName) failed by:chnName is null or empty");
            List<string> chnNames = _SBCfg("LightChannelNames");//_cfg.GetItemValue("LightChannelNames") as List<string>;
            if (chnNames.Contains(chnName))
                return;
            chnNames.Add(chnName);
        }

        public void RemoveLightChannel(string chnName)
        {
            List<string> chnNames = _SBCfg("LightChannelNames");//_cfg.GetItemValue("LightChannelNames") as List<string>;
            if (!chnNames.Contains(chnName))
                return;
            chnNames.Remove(chnName);
        }

        public void ClearLightChannel()
        {
            _SBCfg("LightChannelNames").Clear();//(_cfg.GetItemValue("LightChannelNames") as List<string>).Clear();
        }



        public string[] TrigChannelNames
        {
            get
            {
                if (null == _SBCfg("TrigChannelNames"))
                    return null;
                return _SBCfg("TrigChannelNames").ToArray();//return (_cfg.GetItemValue("TrigChannelNames") as List<string>).ToArray();
            }
        }

        public void AddTrigChannel(string chnName)
        {
            if (string.IsNullOrEmpty(chnName))
                throw new ArgumentNullException("AddTrigChannel(chnName) failed by:chnName is null or empty");
            List<string> chnNames = _SBCfg("TrigChannelNames");//_cfg.GetItemValue("TrigChannelNames") as List<string>;
            if (chnNames.Contains(chnName))
                return;
            chnNames.Add(chnName);
        }

        public void RemoveTrigChannel(string chnName)
        {
            List<string> chnNames = _SBCfg("TrigChannelNames");//_cfg.GetItemValue("TrigChannelNames") as List<string>;
            if (!chnNames.Contains(chnName))
                return;
            chnNames.Remove(chnName);
        }

        public void ClearTrigChannel()
        {
            _SBCfg("TrigChannelNames").Clear();//(_cfg.GetItemValue("TrigChannelNames") as List<string>).Clear();
        }

        


        /// <summary>
        /// 存放继承类中声明的参数配置 ， 
        /// Key = ItemName
        /// Value = <"参数类别",参数类型> 
        /// </summary>
        Dictionary<string, object[]> dictCfgParamDecleared = new Dictionary<string, object[]>(); 
        /// <summary>
        /// 供继承类申明需要的序列化参数
        /// 只在继承类的构造函数中使用
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramType"></param>
        public void DeclearCfgParam(string cfgName,Type cfgType,string category = "StationConfig")
        {
            if (string.IsNullOrEmpty(cfgName))
                throw new ArgumentNullException("声明的配置参数名称为空 in DeclearCfgParam()");
            if (cfgName == "StationBasePrivateConfig")
                throw new ArgumentException("声明的配置参数名称不能为:\"StationBasePrivateConfig\" in DeclearCfgParam()");
            if (string.IsNullOrEmpty(category))
                throw new ArgumentException("声明的配置参数类别名不能为空 in DeclearCfgParam()");
            if (dictCfgParamDecleared.ContainsKey(cfgName))
                throw new ArgumentException("重复申明配置参数,Name = " + cfgName);
            dictCfgParamDecleared.Add(cfgName, new object[] { category, JFParamDescribe.Create(cfgName,cfgType,JFValueLimit.NonLimit,null) });
           
        }

        /// <summary>
        /// 获取所有已声明配置参数的类别名称
        /// </summary>
        /// <returns></returns>
        public List<string> AllCfgParamCategories()
        {
            List<string> ret = new List<string>();
            foreach (KeyValuePair<string, object[]> kv in dictCfgParamDecleared)
                if (!ret.Contains(kv.Value[0] as string))
                    ret.Add(kv.Value[0] as string);
            return ret;
        }

        /// <summary>
        /// 获取某一个类别下的所有配置参数名
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<string> AllCfgParamNamesInCategory(string category)
        {
            List<string> ret = new List<string>();
            foreach (KeyValuePair<string, object[]> kv in dictCfgParamDecleared)
                if (kv.Value[0] as string == category)
                    ret.Add(kv.Key);
            return ret;
        }



        public void DeclearCfgParam(JFParamDescribe paramDescribe,string category)
        {
            if (string.IsNullOrEmpty(paramDescribe.DisplayName))
                throw new ArgumentNullException("声明的配置参数名称为空 in DeclearCfgParam()");
            if (paramDescribe.DisplayName == "StationBasePrivateConfig")
                throw new ArgumentException("声明的配置参数名称不能为:\"StationBasePrivateConfig\" in DeclearCfgParam()");
            if (string.IsNullOrEmpty(category))
                throw new ArgumentException("声明的配置参数类别名不能为空 in DeclearCfgParam()");
            if (dictCfgParamDecleared.ContainsKey(paramDescribe.DisplayName))
                throw new ArgumentException("重复申明配置参数,Name = " + paramDescribe.DisplayName);
            dictCfgParamDecleared.Add(paramDescribe.DisplayName, new object[] { category, paramDescribe });
        }


        /// <summary>
        /// 配置项是否为工站内注册（不可删除）
        /// </summary>
        /// <param name="cfgName"></param>
        /// <returns></returns>
        public bool IsCfgDecleared(string cfgName)
        {
            return dictCfgParamDecleared.ContainsKey(cfgName);
        }

        /// <summary>
        /// 获取配置项的类型描述信息
        /// </summary>
        /// <param name="cfgName"></param>
        /// <returns></returns>
        public JFParamDescribe GetCfgParamDescribe(string cfgName)
        {
            if(dictCfgParamDecleared.ContainsKey(cfgName)) //工站(声明的)固有配置属性
                return dictCfgParamDecleared[cfgName][1] as JFParamDescribe;
            else // 从配置界面上动态添加的配置
            {
                if(!_cfg.ContainsItem(cfgName))
                    throw new ArgumentException(string.Format("GetCfgParamDescribe(cfgName,...) failed by cfgName = \"{0}\" is not contained in StationName = \"{1}\"", cfgName, Name));
                return JFParamDescribe.Create(cfgName, _cfg.GetItemValue(cfgName).GetType(), JFValueLimit.NonLimit, null);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cfgName"></param>
        /// <param name="cfgValue"></param>
        public void SetCfgParamValue(string cfgName,object cfgValue)
        {
            
            //if (!dictCfgParamDecleared.ContainsKey(cfgName))
            //    throw new ArgumentException(string.Format("SetCfgParam(cfgName,...) failed by cfgName = \"{0}\" is not contained by DictCfgParamDecleared,StationName = \"{1}\"", cfgName, Name));
            //if(null == cfgValue)
            //{
            //    if(!IsNullableType((dictCfgParamDecleared[cfgName][1] as JFParamDescribe).ParamType))
            //        throw new ArgumentException(string.Format("SetCfgParam(cfgName = \"{0}\",cfgValue = null) failed by cfgType ={1}  is not nullable type,StationName = \"{2}\"", cfgName, (dictCfgParamDecleared[cfgName][1] as Type).Name, Name));
            //}
            //else
            //{
            //    if(cfgValue.GetType() != (dictCfgParamDecleared[cfgName][1] as JFParamDescribe).ParamType)
            //        throw new ArgumentException(string.Format("SetCfgParam(cfgName = \"{0}\",cfgValue ) failed by cfgType ={1}  isnot equal {2},StationName = \"{3}\"", cfgName, cfgValue.GetType().Name,(dictCfgParamDecleared[cfgName][1] as Type).Name, Name));

            //}
            if (IsInitOK) //已经初始化完成
                _cfg.SetItemValue(cfgName, cfgValue);
            else //调用时未初始化（构造函数中）
            {
                if (dictCfgParamDecleared.ContainsKey(cfgName))
                {
                    object[] oa = dictCfgParamDecleared[cfgName];
                    dictCfgParamDecleared[cfgName] = new object[] { oa[0], oa[1], cfgValue };
                }
                else
                    _cfg.SetItemValue(cfgName, cfgValue);
            }
        }


        public object GetCfgParamValue(string cfgName)
        {
            //if (!dictCfgParamDecleared.ContainsKey(cfgName))
            //    throw new ArgumentException(string.Format("GetCfgParam(cfgName) failed by cfgName = \"{0}\" is not contained by DictCfgParamDecleared,StationName = \"{1}\"", cfgName, Name));
            if(!IsInitOK)
            {
                if(_cfg.ContainsItem(cfgName)) //如果配置中已包含
                    return _cfg.GetItemValue(cfgName);

                if (dictCfgParamDecleared.ContainsKey(cfgName)) //如果是声明的（固有）参数项
                {
                    object[] oa = dictCfgParamDecleared[cfgName];
                    if (oa.Length > 2)
                        return oa[2];
                    else
                        return DefaultValueFromType((oa[1] as JFParamDescribe).ParamType);
                }

                return null;
            }
            return _cfg.GetItemValue(cfgName);
        }


        //Dictionary<string,JFMethodFlowBox> _dictMethodFlowBoxes = new Dictionary<string, JFMethodFlowBox>(); //所有正在异步运行的方法流容器



        #region  工站内方法流操作

        /// <summary>
        /// 同步运行一个（工站内定义）的工作流
        /// </summary>
        /// <param name="wfName">站内工作流名称</param>
        /// <returns></returns>
        public bool SynchRunWorkFlow(string wfName , out string errorInfo)
        {


            JFMethodFlow mf = GetWorkFlow(wfName);
            if(null == mf)
            {
                errorInfo = "运行动作流:\"" + wfName + "\"失败,动作流不存在！";
                return false;
            }
            if(mf.IsWorking())
            {
                errorInfo = "运行动作流:\"" + wfName + "\"失败,动作流当前状态:" + mf.CurrWorkStatus;
                return false;
            }
            if(!mf.Action())
            {
                errorInfo = "Faield info:" + mf.ActionErrorInfo() ;
                return false;
            }
            
            errorInfo = "Success";
            return true;
        }
        

        /// <summary>
        /// 异步运行一个(工站内定义的)工作流
        /// </summary>
        /// <param name="wfName">工作流名称</param>
        /// <param name="rounds">运行次数</param>
        /// <param name="errorInfo">启动错误信息</param>
        /// <returns></returns>
        public bool AsynchRunWorkFlow(string wfName,out string errorInfo)
        {
            errorInfo = "Unknown-Error";
            if (!ContainWorkFlow(wfName))
            {
                errorInfo = "工站内部未包含工作流:名称=" + wfName;
                return false;
            }
            JFMethodFlow mf = GetWorkFlow(wfName);
           

            bool ret = false;
            do
            {
                if (mf.IsWorking())
                {
                    errorInfo = mf.Name + "当前状态:" + mf.CurrWorkStatus;
                    break;
                }


                JFWorkCmdResult cmdRet = mf.Start();
                if (cmdRet != JFWorkCmdResult.Success)
                {
                    errorInfo = "启动工作流失败,错误代码:" + cmdRet;
                    break;
                }
                ret = true;
                errorInfo = "Success";
            } while (false);
            return ret;
        }

        /// <summary>
        /// 暂停一个(异步)执行中的工作流
        /// </summary>
        /// <param name="wfName"></param>
        /// <returns></returns>
        public bool PauseWorkFlow(string wfName,out string errorInfo)
        {
            errorInfo = "Unknown-Error";
            if (!ContainWorkFlow(wfName))
            {
                errorInfo = "工站内部未包含工作流:名称=" + wfName;
                return false;
            }
            JFMethodFlow mf = GetWorkFlow(wfName);
            if(mf.CurrWorkStatus == JFWorkStatus.Pausing) //
            {
                errorInfo = "Success";
                return true;
            }
            JFWorkCmdResult wcr = mf.Pause(CycleMilliseconds);
            if(wcr != JFWorkCmdResult.Success)
            {
                errorInfo = "执行暂停失败，ErrorCode:" + wcr;
                return true;
            }

            errorInfo = "Success";
            return true;

        }


        /// <summary>
        /// 恢复一个异步执行中的工作流
        /// </summary>
        /// <param name="wfName"></param>
        /// <returns></returns>
        public bool ResumeWorkFlow(string wfName,out string errorInfo)
        {
            errorInfo = "Unknown-Error";
            if (!ContainWorkFlow(wfName))
            {
                errorInfo = "工站内部未包含工作流:名称=" + wfName;
                return false;
            }
            JFMethodFlow mf = GetWorkFlow(wfName);
            if (mf.CurrWorkStatus == JFWorkStatus.Running) //
            {
                errorInfo = "Success";
                return true;
            }

            JFWorkCmdResult wcr = mf.Resume(CycleMilliseconds);
            if (wcr != JFWorkCmdResult.Success)
            {
                errorInfo = "恢复运行失败，ErrorCode:" + wcr;
                return true;
            }

            errorInfo = "Success";
            return true;
        }

        public void StopWorkFlow(string wfName)
        {
            if (!ContainWorkFlow(wfName))
                return;
            JFMethodFlow mf = GetWorkFlow(wfName);
            JFWorkCmdResult wcr = mf.Stop(CycleMilliseconds);
            if (wcr != JFWorkCmdResult.Success)
                mf.Abort();

            return;

        }

        


        /// <summary>
        /// 等待一个异步执行的工作流完成
        /// </summary>
        /// <param name="milliSecondsTimeout"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public JFWorkCmdResult WaitAsyncWorkFlowDone(string wfName,out string errorInfo, int milliSecondsTimeout)
        {
            errorInfo = "Unknown-Error";
            if (!ContainWorkFlow(wfName))
            {
                errorInfo = "工站内部未包含工作流:名称=" + wfName;
                return JFWorkCmdResult.UnknownError;
            }
            JFMethodFlow mf = GetWorkFlow(wfName);
            DateTime startTime = DateTime.Now;
            while (true)
            {
                JFWorkStatus currStatus = mf.CurrWorkStatus;
                if (currStatus == JFWorkStatus.NormalEnd )
                {
                    errorInfo = "Success";
                    return JFWorkCmdResult.Success;
                }
                if(!IsWorkingStatus(currStatus))
                {
                    errorInfo = "工作流运行错误:" + mf.CurrWorkStatus;
                    return JFWorkCmdResult.ActionError;
                }
                DateTime currTime = DateTime.Now;
                if (IsInWorkThread())//如果在主线程中运行，需要检查是否有指令到达
                    CheckCmd(CycleMilliseconds);
                TimeSpan ts = DateTime.Now - startTime;
                if(milliSecondsTimeout >=0 && ts.TotalMilliseconds >= milliSecondsTimeout)
                {
                    errorInfo = "等待超时";
                    return JFWorkCmdResult.Timeout;
                }
            }
        }


        /// <summary>
        /// 暂停所有异步执行中的工作流
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        protected bool PauseAllAsyncWorkFlow(out string errorInfo)
        {
            errorInfo = "Success";

            foreach (JFMethodFlow mf in _dictMethodFlow.Values)
            {

                JFWorkStatus ws = mf.CurrWorkStatus;
                if (ws == JFWorkStatus.Pausing)
                    continue;
                else //只对当前正在执行的MFB执行暂停
                {
                    if (ws == JFWorkStatus.Running || ws == JFWorkStatus.Interactiving)
                    {
                        JFWorkCmdResult ret = mf.Pause(CycleMilliseconds);
                        if (JFWorkCmdResult.Success != ret)
                        {
                            errorInfo = "暂停工作流:\"" + mf.Name + "\"失败，ErrorCode = " + ret;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 恢复所有暂停的工作流
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        protected bool ResumeAllAsyncWorkFlow(out string errorInfo)
        {
            errorInfo = "Success";
            foreach (JFMethodFlow mf in _dictMethodFlow.Values)
            {
                JFWorkStatus ws = mf.CurrWorkStatus;
                if (ws == JFWorkStatus.Running || ws == JFWorkStatus.Interactiving)
                    continue;
                if (ws == JFWorkStatus.Pausing)
                {
                    JFWorkCmdResult ret = mf.Resume(CycleMilliseconds);
                    if (JFWorkCmdResult.Success != ret)
                    {
                        errorInfo = "恢复工作流:\"" + mf.Name + "\"失败，ErrorCode = " + ret;
                        return false;
                    }
                }
            }
            return true;
        }

        protected void StopAllAsyncWorkFlow()
        {
            foreach (JFMethodFlow mf in _dictMethodFlow.Values)
            {
                int timeoutMilliSeconds = mf.CycleMilliseconds * 5;
                if (timeoutMilliSeconds < 0 && timeoutMilliSeconds > 200)
                    timeoutMilliSeconds = 200;
                JFWorkCmdResult ret = mf.Stop(timeoutMilliSeconds);
                if (JFWorkCmdResult.Success != ret)
                    mf.Abort();
                
            }
        }





        #endregion




        protected virtual void NotifyProductFinished(int passCount, string[] passIDs, int NGCount, string[] ngIDs, string[] NGInfo)
        {
            EventProductFinished?.Invoke(this, passCount, passIDs, NGCount, ngIDs, NGInfo);
        }

        protected virtual void NotifyCustomizeMsg(string msgCategory, object[] msgParams)
        {
            EventCustomizeMsg?.Invoke(this, msgCategory, msgParams);
        }



        #region 通用的工站方法
        /// <summary>
        /// 打开通道单元所属的设备
        /// </summary>
        /// <param name="chnType"></param>
        /// <param name="chnName"></param>
        /// <returns></returns>
        public bool OpenChnDevice(NamedChnType devChnType, string chnName, out string errorInfo)
        {
            if (NamedChnType.None == devChnType)
            {
                errorInfo = "未指定设备类型";
                return false;
            }
            if (string.IsNullOrEmpty(chnName))
            {
                errorInfo = "未指定设备通道名称";
                return false;
            }

            int errCode = 0;

            if (devChnType == NamedChnType.Camera) //相机设备未使用多级通道
            {
                string[] allCmrNames = JFHubCenter.Instance.InitorManager.GetIDs(typeof(IJFDevice_Camera));
                if (null == allCmrNames)
                {
                    errorInfo = "设备表中不存在相机设备！";
                    return false;
                }
                if (!allCmrNames.Contains(chnName))
                {
                    errorInfo = "设备表中未包含相机设备:\"" + chnName + "\"";
                    return false;
                }

                IJFDevice_Camera cmr = JFHubCenter.Instance.InitorManager.GetInitor(chnName) as IJFDevice_Camera;
                if (cmr.IsDeviceOpen)
                {
                    errorInfo = "Success";
                    return true;
                }

                errCode = cmr.OpenDevice();
                if (errCode != 0)
                {
                    errorInfo = cmr.GetErrorInfo(errCode);
                    return false;
                }
                errorInfo = "Success";
                return true;

            }
            else
            {
                JFDevCellInfo ci = null;
                
                switch (devChnType)
                {
                    case NamedChnType.Ai:
                        ci = JFHubCenter.Instance.MDCellNameMgr.GetAiCellInfo(chnName);
                        break;
                    case NamedChnType.Ao:
                        ci = JFHubCenter.Instance.MDCellNameMgr.GetAoCellInfo(chnName);
                        break;
                    case NamedChnType.Axis:
                        ci = JFHubCenter.Instance.MDCellNameMgr.GetAxisCellInfo(chnName);
                        break;
                    case NamedChnType.CmpTrig:
                        ci = JFHubCenter.Instance.MDCellNameMgr.GetCmpTrigCellInfo(chnName);
                        break;
                    case NamedChnType.Di:
                        ci = JFHubCenter.Instance.MDCellNameMgr.GetDiCellInfo(chnName);
                        break;
                    case NamedChnType.Do:
                        ci = JFHubCenter.Instance.MDCellNameMgr.GetDoCellInfo(chnName);
                        break;
                    case NamedChnType.Light:
                        ci = JFHubCenter.Instance.MDCellNameMgr.GetLightCtrlChannelInfo(chnName);
                        break;
                    case NamedChnType.Trig:
                        ci = JFHubCenter.Instance.MDCellNameMgr.GetTrigCtrlChannelInfo(chnName);
                        break;

                }
                //if(!JFDevChannel.CheckChannel(ct,chnName,out dev,) //打开设备时不能使用CheckChannel ， 因为未打开的设备可能未创建各子模块
                if (ci == null)
                {
                    errorInfo = "设备通道ID:\"" + chnName + "\" 在设备命名表中不存在";
                    return false;
                }
                IJFDevice dev = JFHubCenter.Instance.InitorManager.GetInitor(ci.DeviceID) as IJFDevice;
                if(null == dev)
                {
                    errorInfo = "设备通道ID:\"" + chnName + "\"所属设备:\""+ci.DeviceID + "\"不存在";
                    return false;
                }
                if(dev.IsDeviceOpen)
                {
                    errorInfo = "Success";
                    return true;
                }
                else
                {
                    errCode = dev.OpenDevice();
                    if(errCode != 0 )
                    {
                        errorInfo = "设备通道ID:\"" + chnName + "\"所属设备:\"" + ci.DeviceID + "\"打开失败:" + dev.GetErrorInfo(errCode);
                        return false;
                    }

                    errorInfo = "Success";
                    return true;
                }

            }





        }


        /// <summary>
        /// 打开通道单元所属的设备
        /// </summary>
        /// <param name="chnType"></param>
        /// <param name="chnAliasName">通道的替身名</param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool OpenChnDeviceAlias(NamedChnType devChnType, string chnAliasName, out string errorInfo)
        {
            if (NamedChnType.None == devChnType)
            {
                errorInfo = "未指定设备类型";
                return false;
            }
            if (string.IsNullOrEmpty(chnAliasName))
            {
                errorInfo = "未指定设备通道名称";
                return false;
            }
            string gName = GetDecChnGlobName(devChnType, chnAliasName);
            if(string.IsNullOrEmpty(gName))
            {
                errorInfo = "OpenChnDeviceAlias(chnAliasName ...) failed by:  chnAliasName = \"" + chnAliasName + "\" is not bingding to global";
                return false;
            }
            string innerError;
            bool isOK = OpenChnDevice(devChnType, gName, out innerError);
            if (!isOK)
                errorInfo = "OpenChnDeviceAlias(chnAliasName ...) failed by:  chnAliasName = \"" + chnAliasName + "\"," + innerError;
            else
                errorInfo = "Success";
            return isOK;

        }




        /// <summary>
        ///  设置DO状态
        /// </summary>
        /// <param name="doName">工站内配置的DO</param>
        /// <param name="isTurnOn"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetDO(string doName, bool isTurnOn, out string errorInfo)
        {
            errorInfo = "Unknown error";
            if(string.IsNullOrEmpty(doName))
            {
                errorInfo = "SetDO(string doName...)falied by doName is null or empty ";
                return false;
            }
            JFDevChannel chn = new JFDevChannel(JFDevCellType.DO, doName);
            string err;
            IJFDevice dev;
            JFDevCellInfo ci;
            if (!JFDevChannel.CheckChannel(JFDevCellType.DO,doName,out dev,out ci,out err))
            {
                errorInfo = "SetDO(...) fialed by:" + err;
                return false;
            }

            int errCode = (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex).SetDO(ci.ChannelIndex, isTurnOn);
            if(errCode != 0 )
            {
                errorInfo = "SetDO(...) fialed by :" + (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex).GetErrorInfo(errCode);
                return false;
            }

            errorInfo = "Success";
            return true;

        }


        public JFWorkCmdResult WaitDO(string doName,bool isTurnOn,out string errorInfo,int timeoutMilSeconds = -1)
        {
            errorInfo = "Unknown error";
            if (string.IsNullOrEmpty(doName))
            {
                errorInfo = "WaitDO(string doName...)falied by doName is null or empty ";
                return JFWorkCmdResult.UnknownError;
            }
            JFDevChannel chn = new JFDevChannel(JFDevCellType.DO, doName);
            string err;
            IJFDevice dev;
            JFDevCellInfo ci;
            if (!JFDevChannel.CheckChannel(JFDevCellType.DO, doName, out dev, out ci, out err))
            {
                errorInfo = "WaitDO(...) fialed by:" + err;
                return JFWorkCmdResult.UnknownError;
            }

            IJFModule_DIO md = (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex);

            DateTime startTime = DateTime.Now;
            bool isON = false;
            int errCode;
            while (true)
            {
                errCode = md.GetDO(ci.ChannelIndex, out isON);
                if(errCode != 0 )
                {
                    errorInfo = "WaitDO(string doName...) faled by:" + md.GetErrorInfo(errCode);
                    return JFWorkCmdResult.ActionError;
                }
                if(isON == isTurnOn)
                {
                    errorInfo = "Success";
                    return JFWorkCmdResult.Success;
                }
                if (IsInWorkThread())
                    CheckCmd(CycleMilliseconds);

                if(timeoutMilSeconds >=0)
                {
                    TimeSpan ts = (DateTime.Now - startTime);
                    if(ts.TotalMilliseconds >= timeoutMilSeconds)
                    {
                        errorInfo = "Timeout";
                        return JFWorkCmdResult.Timeout;
                    }
                }

            }


        }


        public bool GetDI(string diName,out bool isON, out string errorInfo)
        {
            isON = false;
            errorInfo = "Unknown error";
            if (string.IsNullOrEmpty(diName))
            {
                errorInfo = "GetDI(string diName...)falied by diName is null or empty ";
                return false;
            }
            //JFDevChannel chn = new JFDevChannel(JFDevCellType.DI, diName);
            string err;
            IJFDevice dev;
            JFDevCellInfo ci;
            if (!JFDevChannel.CheckChannel(JFDevCellType.DI, diName, out dev, out ci, out err))
            {
                errorInfo = "GetDI() fialed by:" + err;
                return false;
            }

            IJFModule_DIO md = (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex);
            int errCode = md.GetDI(ci.ChannelIndex, out isON);
            if (errCode != 0)
            {
                errorInfo = "WaitDI(string diName...) faled by:" + md.GetErrorInfo(errCode);
                return false;
            }

            return true;
        }

        public bool GetDIByAlias(string diAlias, out bool isON, out string errorInfo)
        {
            isON = false;
            if (string.IsNullOrEmpty(diAlias))
            {
                errorInfo = "GetDIByAlias failed by diAliasName is null or empty";
                return false;
            }
            string gName = GetDecChnGlobName(NamedChnType.Di, diAlias);
            if (string.IsNullOrEmpty(gName))
            {
                errorInfo = "GetDIByAlias(diAlias,..) faied by: diAlias = \"" + diAlias + "\" is not binding to global name";
                return false;
            }

            return GetDI(gName, out isON, out errorInfo);
        }

        public JFWorkCmdResult WaitDI(string diName, bool isTurnOn, out string errorInfo, int timeoutMilSeconds = -1)
        {
            errorInfo = "Unknown error";
            if (string.IsNullOrEmpty(diName))
            {
                errorInfo = "WaitDI(string diName...)falied by diName is null or empty ";
                return JFWorkCmdResult.UnknownError;
            }
            //JFDevChannel chn = new JFDevChannel(JFDevCellType.DI, diName);
            string err;
            IJFDevice dev;
            JFDevCellInfo ci;
            if (!JFDevChannel.CheckChannel(JFDevCellType.DI, diName, out dev, out ci, out err))
            {
                errorInfo = "WaitDI(...) fialed by:" + err;
                return JFWorkCmdResult.UnknownError;
            }

            IJFModule_DIO md = (dev as IJFDevice_MotionDaq).GetDio(ci.ModuleIndex);

            DateTime startTime = DateTime.Now;
            bool isON = false;
            int errCode;
            while (true)
            {
                errCode = md.GetDI(ci.ChannelIndex, out isON);
                if (errCode != 0)
                {
                    errorInfo = "WaitDI(string diName...) faled by:" + md.GetErrorInfo(errCode);
                    return JFWorkCmdResult.ActionError;
                }
                if (isON == isTurnOn)
                {
                    errorInfo = "Success";
                    return JFWorkCmdResult.Success;
                }
                if (IsInWorkThread())
                    CheckCmd(CycleMilliseconds);

                if (timeoutMilSeconds >= 0)
                {
                    TimeSpan ts = (DateTime.Now - startTime);
                    if (ts.TotalMilliseconds >= timeoutMilSeconds)
                    {
                        errorInfo = "Timeout";
                        return JFWorkCmdResult.Timeout;
                    }
                }

            }
        }


        /// <summary>
        /// 等待多个DI为指定的状态（相同的）
        /// </summary>
        /// <param name="diNames"></param>
        /// <param name="isTurnOn"></param>
        /// <param name="errorInfo"></param>
        /// <param name="timeoutMilSeconds"></param>
        /// <returns></returns>
        public JFWorkCmdResult WaitDIs(string[] diNames, bool isTurnOn, out string errorInfo, int timeoutMilSeconds = -1)
        {
            errorInfo = "Unknown error";
            if (null== diNames || 0 == diNames.Length)
            {
                errorInfo = "WaitDIs(string[] diNames...)falied by diNames is null or empty ";
                return JFWorkCmdResult.UnknownError;
            }
            string err;
            IJFDevice[] devs = new IJFDevice[diNames.Length];
            JFDevCellInfo[] cis = new JFDevCellInfo[diNames.Length];
            IJFModule_DIO[] mds = new IJFModule_DIO[diNames.Length];
            for (int i = 0; i < diNames.Length; i++)
            {
                if (!JFDevChannel.CheckChannel(JFDevCellType.DI, diNames[i], out devs[i], out cis[i], out err))
                {
                    errorInfo = "WaitDIs(...) fialed by:" + err;
                    return JFWorkCmdResult.UnknownError;
                }
                mds[i] = (devs[i] as IJFDevice_MotionDaq).GetDio(cis[i].ModuleIndex);
            }

            

            DateTime startTime = DateTime.Now;
            bool isON = false;
            bool isWaitedAll = false;
            int errCode;
            while (true)
            {
                isWaitedAll = true;
                for (int i = 0; i < diNames.Length; i++)
                {
                    errCode = mds[i].GetDI(cis[i].ChannelIndex, out isON);
                    if (errCode != 0)
                    {
                        errorInfo = "WaitDIs(string[] diNames...) faled by:" + mds[i].GetErrorInfo(errCode);
                        return JFWorkCmdResult.ActionError;
                    }
                    if (isON != isTurnOn)
                    {
                        isWaitedAll = false;
                        break;
                    }
                    
                }
                if(isWaitedAll)
                    return JFWorkCmdResult.Success;
                
                if (IsInWorkThread())
                    CheckCmd(CycleMilliseconds);

                if (timeoutMilSeconds >= 0)
                {
                    TimeSpan ts = (DateTime.Now - startTime);
                    if (ts.TotalMilliseconds >= timeoutMilSeconds)
                    {
                        errorInfo = "Timeout";
                        return JFWorkCmdResult.Timeout;
                    }
                }

            }
        }


        /// <summary>
        /// 等待多个DI为指定的状态（使用替身名）
        /// </summary>
        /// <param name="diNames"></param>
        /// <param name="isTurnOn"></param>
        /// <param name="errorInfo"></param>
        /// <param name="timeoutMilSeconds"></param>
        /// <returns></returns>
        public JFWorkCmdResult WaitDIsByAlias(string[] diAliasNames, bool isTurnOn, out string errorInfo, int timeoutMilSeconds = -1)
        {
            if(null == diAliasNames || 0 == diAliasNames.Length)
            {
                errorInfo = "WaitDIsByAlias(string[] diAliasNames,...) failed by diAliasNames is null or empty";
                return JFWorkCmdResult.UnknownError;
            }

            string[] diNames = new string[diAliasNames.Length];
            for (int i = 0; i < diAliasNames.Length; i++)
            {
                diNames[i] = GetDecChnGlobName(NamedChnType.Di, diAliasNames[i]);
                if(string.IsNullOrEmpty(diNames[i]))
                {
                    errorInfo = "WaitDIsByAlias(string[] diAliasNames,...) failed by AliasName = \"" + diAliasNames[i] + "\"未绑定全局通道名 ";
                return JFWorkCmdResult.UnknownError;
                }
            }

            return WaitDIs(diNames, isTurnOn, out errorInfo, timeoutMilSeconds);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="doName"></param>
        /// <param name="isTurnOn"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetDOAlias(string doAliasName, bool isTurnOn, out string errorInfo)
        {
            errorInfo = "Unknown error";
            if(string.IsNullOrEmpty(doAliasName))
            {
                errorInfo = "SetDOAlias(... faled by doAliasName is null or empty";
                return false;
            }
            string gName = GetDecChnGlobName(NamedChnType.Do, doAliasName);
            if(string.IsNullOrEmpty(gName))
            {
                errorInfo = "SetDOAlias(doAliasName,..) faied by: doAliasName = \"" + doAliasName + "\" is not binding to global name";
                return false;
            }
            string innerError;
            bool isOK = SetDO(gName, isTurnOn, out innerError);

            return isOK;
        }


        public JFWorkCmdResult WaitDOAlias(string doAliasName, bool isTurnOn, out string errorInfo,int timeoutMiliSeconds = -1)
        {
            errorInfo = "Unknown error";
            if (string.IsNullOrEmpty(doAliasName))
            {
                errorInfo = "WaitDOAlias(... )faled by doAliasName is null or empty";
                return JFWorkCmdResult.UnknownError;
            }
            string gName = GetDecChnGlobName(NamedChnType.Do, doAliasName);
            if (string.IsNullOrEmpty(gName))
            {
                errorInfo = "doAliasName = \"" + doAliasName + "\" is not binding to global name";
                return JFWorkCmdResult.UnknownError;
            }
            string innerError;
            JFWorkCmdResult ret = WaitDO(gName, isTurnOn, out innerError, timeoutMiliSeconds);
            if (ret != JFWorkCmdResult.Success)
                errorInfo = "WetDOAlias(doAliasName,..) faied by: doAliasName = \"" + doAliasName + "\" innerError = " + innerError;

            return ret;
        }

        public JFWorkCmdResult WaitDIAlias(string diAliasName, bool isTurnOn, out string errorInfo,int timeoutMiliSeconds = -1)
        {
            errorInfo = "Unknown error";
            if (string.IsNullOrEmpty(diAliasName))
            {
                errorInfo = "WaitDIAlias(... )faled by diAliasName is null or empty";
                return JFWorkCmdResult.UnknownError;
            }
            string gName = GetDecChnGlobName(NamedChnType.Di, diAliasName);
            if (string.IsNullOrEmpty(gName))
            {
                errorInfo = "diAliasName = \"" + diAliasName + "\" is not binding to global name";
                return JFWorkCmdResult.UnknownError;
            }
            string innerError;
            JFWorkCmdResult ret = WaitDI(gName, isTurnOn, out innerError, timeoutMiliSeconds);
            if (ret != JFWorkCmdResult.Success)
                errorInfo = "WaitDIAlias(diAliasName,..) faied by: diAliasName = \"" + diAliasName + "\" innerError = " + innerError;

            return ret;
        }



        public void StopAxis(string axisName)
        {
            if (string.IsNullOrEmpty(axisName))
                return;
            string err;
            IJFDevice dev;
            JFDevCellInfo ci;
            if (!JFDevChannel.CheckChannel(JFDevCellType.Axis, axisName, out dev, out ci, out err))
                return;

            (dev as IJFDevice_MotionDaq).GetMc(ci.ModuleIndex).StopAxis(ci.ChannelIndex);
        }


        public void StopAxisAlias(string axisAliasName)
        {
            if (string.IsNullOrEmpty(axisAliasName))
                return;
            string gAxisName = GetDecChnGlobName(NamedChnType.Axis, axisAliasName);
            StopAxis(gAxisName);
        }



        /// <summary>
        /// 设置光源(开关)亮度
        /// </summary>
        /// <param name="lightChnName"></param>
        /// <param name="intensity"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetLightIntensity(string lightName,int intensity,out string errorInfo)
        {
            errorInfo = "Unknown error";
            IJFDevice dev;
            JFDevCellInfo ci;
            if (!JFDevChannel.CheckChannel(JFDevCellType.Light, lightName, out dev, out ci, out errorInfo))
                return false;
            int errCode = (dev as IJFDevice_LightController).SetLightIntensity(ci.ChannelIndex, intensity);
            if(errCode != 0)
            {
                errorInfo = dev.GetErrorInfo(errCode);
                return false;
            }
            errorInfo = "Success";
            return true;
        }


        /// <summary>
        /// 设置光源开关亮度（替身名）
        /// </summary>
        /// <param name="lightAliasName"></param>
        /// <param name="intensity"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public bool SetLightIntensityAlias(string lightAliasName, int intensity, out string errorInfo)
        {
            if(!string.IsNullOrEmpty(lightAliasName))
            {
                errorInfo = "SetLightIntensityAlias(lightAliasName,...) failed by lightAliasName is null or empty";
                return false;
            }
            string gLightName = GetDecChnGlobName(NamedChnType.Axis, lightAliasName);
            if(string.IsNullOrEmpty(gLightName))
            {
                errorInfo = "SetLightIntensityAlias(lightAliasName,...) failed by lightAliasName is not bingding to global name";
                return false;
            }

            bool isOK = SetLightIntensity(gLightName, intensity, out errorInfo);
            if (!isOK)
                errorInfo = "SetLightIntensityAlias(lightAliasName,...) failed by lightAliasName:\"" + lightAliasName + "\" inner Error:" + errorInfo;
            else
                errorInfo = "Success";
            return isOK;
            
           
        }





        #endregion





        #region  数据池相关操作
        /////// <summary>
        /////// 注册一个变量到系统数据池当中
        /////// </summary>
        /////// <param name="itemName">数据项名称</param>
        /////// <param name="itemType">数据项类型</param>
        /////// <param name="initValue">初始值,如果数据项存在,并且已赋值,此项会被忽略</param>
        //public void RegistSystemPoolItem(string itemName, Type itemType, object initValue)
        //{
        //    if (string.IsNullOrEmpty(itemName))
        //        throw new ArgumentNullException("RegistSystemPoolItem(string itemName...) failed by itemName is null or empty");
        //    IJFDataPool dp = JFHubCenter.Instance.DataPool;
        //    if (!dp.RegistItem(itemName, itemType, initValue)) //已存在同名itemName,并且类型不匹配
        //        throw new Exception("RegistSystemPoolItem() failed ,itemName:" + itemName + " is Registed and regist Type = " + dp.GetItemType(itemName).Name);
        //    object existedVal;
        //    dp.GetItemValue(itemName, out existedVal);
        //    if (null != existedVal) //已存在值
        //        return;
        //    if (initValue == null)
        //        return;
        //    if (!dp.SetItemValue(itemName, initValue))
        //        throw new Exception(string.Format("RegistSystemPoolItem(itemName = {0},itemType = {1},initValue) = {2} failed by DataPool.SetValue(itemName,initValue)", itemName, itemType, initValue));

        //}

        /// <summary>
        /// 保存所有声明的系统变量假名(只是为了以申明顺序访问)
        /// </summary>
        List<string> _lstSysPoolItemAliasNames = new List<string>();

        /// <summary>
        /// 保存所有声明的系统变量假名/类型/初始值
        /// </summary>
        Dictionary<string, object[]> _dictSysPoolItemDecleared = new Dictionary<string, object[]>();

        JFXmlDictionary<string, string> _dictSysPoolItemNameMapping = new JFXmlDictionary<string, string>();



       


        /// <summary>
        /// 声明一个变量到系统数据池中
        /// 本函数在继承类的构造函数中使用
        /// 真正的注册动作在Station的Init函数中
        /// 声明的变量名称为站内替身名（非实际的系统变量名称）
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="itemType"></param>
        /// <param name="itemInitValue"></param>
        protected void DeclearSPItemAlias(string aliasName, Type itemType,object itemInitValue) //SP = system pool ,系统数据池 
        {
            if (string.IsNullOrEmpty(aliasName))
                throw new ArgumentNullException("DeclearSystemPoolItem failed by :aliasName is null or empty ");
            if (_dictSysPoolItemDecleared.ContainsKey(aliasName))
                throw new ArgumentException("DeclearSystemPoolItem failed by :aliasName = \"" + aliasName + "\" is decleared!");
            ///检查初始值和类型是否匹配
            if (itemType.IsValueType && itemInitValue == null)
                throw new ArgumentException("DeclearSystemPoolItem failed by:itemType = " + itemType.Name + " InitValue = null");
            object itemVal = itemInitValue;
            if(itemInitValue != null)
            {
                do
                {
                    if (itemType.IsAssignableFrom(itemInitValue.GetType()))//value的类型是ItemType的子类
                        break;
                    
                    if (JFTypeExt.IsExplicitFrom(itemType, itemInitValue.GetType())) //可以进行类型转换
                    {
                        itemVal = JFConvertExt.ChangeType(itemInitValue, itemType);
                        break;
                    }

                    ///不可以进行类型转化
                    throw new Exception("DeclearSystemPoolItem failed by:Can't Convert ItemValue = \"" + itemInitValue.ToString() + "\" to Type:" + itemType.Name);
                } while (false);
            }
            _lstSysPoolItemAliasNames.Add(aliasName);
            _dictSysPoolItemDecleared.Add(aliasName, new object[] { itemType, itemVal });

        }



        /// <summary>
        /// 开放给工作流调用，不建议在编程中使用
        /// </summary>
        /// <returns></returns>
        public bool DeclearSPItemAliasInMethodFlow(string aliasName, Type itemType, object itemInitValue,out string errorInfo)
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                //throw new ArgumentNullException("DeclearSystemPoolItem failed by :aliasName is null or empty ");
                errorInfo = "DeclearSystemPoolItem failed by :aliasName is null or empty ";
                return false;
            }
            if (_dictSysPoolItemDecleared.ContainsKey(aliasName))
            {
                Type existItemType = _dictSysPoolItemDecleared[aliasName][0] as Type;
                if (existItemType == itemType)
                {
                    errorInfo = "Success";
                    return true;
                }

                errorInfo = "DeclearSystemPoolItem failed by :aliasName is already exist ,and type = " + (_dictSysPoolItemDecleared[aliasName][0] as Type).Name;
            }
            ///检查初始值和类型是否匹配
            if (itemType.IsValueType && itemInitValue == null)
                throw new ArgumentException("DeclearSystemPoolItem failed by:itemType = " + itemType.Name + " InitValue = null");
            object itemVal = itemInitValue;
            if (itemInitValue != null)
            {
                do
                {
                    if (itemType.IsAssignableFrom(itemInitValue.GetType()))//value的类型是ItemType的子类
                        break;

                    if (JFTypeExt.IsExplicitFrom(itemType, itemInitValue.GetType())) //可以进行类型转换
                    {
                        itemVal = JFConvertExt.ChangeType(itemInitValue, itemType);
                        break;
                    }

                    ///不可以进行类型转化
                    throw new Exception("DeclearSystemPoolItem failed by:Can't Convert ItemValue = \"" + itemInitValue.ToString() + "\" to Type:" + itemType.Name);
                } while (false);
            }
            _lstSysPoolItemAliasNames.Add(aliasName);
            _dictSysPoolItemDecleared.Add(aliasName, new object[] { itemType, itemVal });
            errorInfo = "Success";
            return true;
        }


        /// <summary>
        /// 在工作流中等待一个bool替身的值 ， 建议只在工作流当中使用
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="targetVal"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public JFWorkCmdResult WaitSPBoolAliasInMethodFlow(string aliasName, bool targetVal, out string errorInfo,int timeoutMiliSeconds = -1)
        {
            if(string.IsNullOrEmpty(aliasName))
            {
                errorInfo = "aliasName is null or empty";
                return JFWorkCmdResult.UnknownError;
            }
            if(!AllSPAliasNames.Contains(aliasName))
            {
                errorInfo = " 工站：" + Name + " 未包含系统变量替身：" + aliasName;
                return JFWorkCmdResult.UnknownError;
            }
            string spName = GetSPAliasRealName(aliasName);//系统变量真实名称
            if(string.IsNullOrEmpty(spName))
            {
                errorInfo = "WaitSPBoolAlias failed by aliasName = \"" + aliasName + "\" is not bingding to glob";
                return JFWorkCmdResult.UnknownError;
            }

            IJFDataPool sysPool = JFHubCenter.Instance.DataPool;
            if(!sysPool.ContainItem(spName))
            {
                errorInfo = "系统数据池未包含Bool AliasName = \"" + aliasName + "\"绑定项:" + spName;
                return JFWorkCmdResult.UnknownError;
            }

            if(sysPool.GetItemType(spName) != typeof(bool))
            {
                errorInfo = "系统数据池 Bool AliasName = \"" + aliasName + "\"绑定项:" + spName + "类型:" + sysPool.GetItemType(spName).Name;
                return JFWorkCmdResult.UnknownError;
            }
            object ov = null;
            DateTime startTime = DateTime.Now;
            while(true)
            {
                if(!sysPool.GetItemValue(spName,out ov))
                {
                    errorInfo = "从系统数据池获取数据失败！";
                    return JFWorkCmdResult.UnknownError;
                }
                if(Convert.ToBoolean(ov) == targetVal)
                {
                    errorInfo = "Success";
                    return JFWorkCmdResult.Success;
                }
                if (IsInWorkThread())
                    CheckCmd(CycleMilliseconds);
                if(timeoutMiliSeconds >= 0)
                {
                    TimeSpan ts = DateTime.Now - startTime;
                    if(ts.TotalSeconds >= timeoutMiliSeconds)
                    {
                        errorInfo = "超时未等到目标值";
                        return JFWorkCmdResult.Timeout;
                    }
                }

            }

            //errorInfo = "";
            //return JFWorkCmdResult.UnknownError;
        }

        /// <summary>
        /// 获取工站内所有系统变量假名（不带StationName + ":"前缀）
        /// </summary>
        public string[] AllSPAliasNames
        {
            get { return _lstSysPoolItemAliasNames.ToArray(); }
        }



        /// <summary>
        /// 获取系统变量的真正名称
        /// </summary>
        /// <param name="locName"></param>
        /// <returns></returns>
        public string GetSPAliasRealName(string aliasName)
        {
            if (!_dictSysPoolItemNameMapping.ContainsKey(aliasName))
                return null;
            return _dictSysPoolItemNameMapping[aliasName];
        }

        /// <summary>
        /// 获取系统变量(替身)的类型
        /// </summary>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public Type GetSPAliasType(string aliasName)
        {
            if (!_dictSysPoolItemDecleared.ContainsKey(aliasName))
                return null;
            return _dictSysPoolItemDecleared[aliasName][0] as Type;
        }


        public bool SetSPAliasRealName(string aliasName, string realName)
        {
            if (string.IsNullOrEmpty(realName))
                throw new ArgumentNullException("SetSPAliasRealName(aliasName,realName) failed by realName is null or empty");
            if (!_dictSysPoolItemDecleared.ContainsKey(aliasName))
                return false;
            string oldRealName = null;
            if (_dictSysPoolItemNameMapping.ContainsKey(aliasName))
            {
                oldRealName = _dictSysPoolItemNameMapping[aliasName];
                if (oldRealName == realName)
                    return true;
                else
                    JFHubCenter.Instance.DataPool.RemoveItem(oldRealName);
                _dictSysPoolItemNameMapping.Remove(aliasName);
            }
            _dictSysPoolItemNameMapping.Add(aliasName, realName);
            JFHubCenter.Instance.DataPool.RegistItem(realName, _dictSysPoolItemDecleared[aliasName][0] as Type, _dictSysPoolItemDecleared[aliasName][1]);

            return true;
        }


        /// <summary>
        /// 根据站内名获取一个系统变量的值
        /// 
        /// </summary>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public bool GetSPAliasValue(string aliasName,out object value)
        {
            value = null;
            if (!_dictSysPoolItemDecleared.ContainsKey(aliasName))
                throw new ArgumentException("GetSPAliasValue(aliasName) failed by: aliasName = " + aliasName + "is not decleared");
            string spRealName = GetSPAliasRealName(aliasName);
            if (string.IsNullOrEmpty(spRealName))
                return false;//throw new Exception("获取系统变量值失败，替身名称:\"" + aliasName + "\"未绑定系统变量");
            return JFHubCenter.Instance.DataPool.GetItemValue(spRealName, out value);
        }

        /// <summary>
        /// 根据站内名设置一个系统变量值
        /// </summary>
        /// <param name="locName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetSPAliasValue(string aliasName, object value)
        {

            if (!_dictSysPoolItemDecleared.ContainsKey(aliasName))
                throw new ArgumentException("SetSPAliasValue(aliasName) failed by: aliasName = " + aliasName + "is not decleared");
            string spRealName = GetSPAliasRealName(aliasName);
            if (string.IsNullOrEmpty(spRealName))
                return false;
            return JFHubCenter.Instance.DataPool.SetItemValue(spRealName, value);
        }


        /// <summary>
        /// 等待一个系统bool变量的值
        /// </summary>
        /// <param name="sysPoolItemName">系统变量名称</param>
        /// <param name="targetValue">系统变量目标值</param>
        /// <param name="timeoutMilliSeconds">超时毫秒数</param>
        /// <returns></returns>
        protected bool WaitSPBool(string sysPoolItemName, bool targetValue, out string errorInfo,int timeoutMilliSeconds = -1)
        {
            errorInfo = "Success";
            if (!JFHubCenter.Instance.DataPool.ContainItem(sysPoolItemName))
            {
                errorInfo = "等待的系统Bool变量名称:\"" + sysPoolItemName + "\"不存在";
                return false;
            }

            if(JFHubCenter.Instance.DataPool.GetItemType(sysPoolItemName) != typeof(bool))
            {
                errorInfo = "等待的系统Bool变量名称:\"" + sysPoolItemName + "\"真实类型为:" + JFHubCenter.Instance.DataPool.GetItemType(sysPoolItemName).Name;
                return false;
            }
            DateTime startTime = DateTime.Now;
            object currVal;
            while(true)
            {
                CheckCmd(CycleMilliseconds);
                bool isOK = JFHubCenter.Instance.DataPool.GetItemValue(sysPoolItemName,out currVal);
                if(!isOK)
                {
                    errorInfo = "未能获取系统变量:\"" + sysPoolItemName + "\"的值";
                    return false;
                }

                if(Convert.ToBoolean(currVal) == targetValue)
                    return true;
                
                if(timeoutMilliSeconds >=0)
                {
                    TimeSpan ts = DateTime.Now - startTime;
                    if(ts.TotalMilliseconds >= timeoutMilliSeconds)
                    {
                        errorInfo = "等待超时！系统Bool变量名称:\"" + sysPoolItemName + "\" 目标值:" + targetValue;
                        return false;
                    }
                }


            }
            
        }

        protected bool WaitSPBoolByAliasName(string aliasName, bool targetValue, out string errorInfo, int timeoutMilliSeconds = -1)
        {
            string realName = GetSPAliasRealName(aliasName);
            if(string.IsNullOrEmpty(realName))
            {
                errorInfo = "等待系统Bool变量失败，AliasName = " + aliasName + "未绑定系统名称";
                return false;
            }

            bool ret = WaitSPBool(realName, targetValue, out errorInfo, timeoutMilliSeconds);
            if (!ret)
                errorInfo = "AliasName:\"" + aliasName + "\"->" + errorInfo;
            return ret;
        }


        JFStationRunMode _runMode = JFStationRunMode.Auto;
        public virtual JFStationRunMode RunMode { get { return _runMode; } }
        public virtual bool SetRunMode(JFStationRunMode runMode)
        {
            if (IsWorking())
                return false;
            _runMode = runMode;
            return true;
        }



        #endregion



        #region  工作流线程池相关



        /// <summary>
        /// 清空所有工作流线程池
        /// </summary>
        protected void ClearWFThreadPool()
        {

        }

        protected string[] AllWFThreadPoolName()
        {
            return null;
        }


        /// <summary>
        /// 创建一个线程池对象
        /// 池中每个线程都循环执行methodFlow ， 直到接收到停止指令
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="methodFlow">工作流模板</param>
        /// <returns></returns>
        protected bool CreateWFThreadPool(string poolName,JFMethodFlow methodFlow,int threadCount,out string errorInfo)
        {
            errorInfo = "Unsupported";
            return false;
        }

        protected bool StartWFThreadPool(string poolName,out string errorInfo)
        {

            errorInfo = "Unsupported";
            return false;
        }



        protected void StopWFThreadPool(string poolName)
        {

        }

        protected bool IsWFThreadPoolRunning(string poolName)
        {
            return false;
        }


        #endregion


        /// <summary>
        /// 将多个工作点位组合成一个点位,如果各工作点位中包含相同的轴，后面的轴位置会覆盖前面的点
        /// </summary>
        /// <param name="workPosNames"></param>
        /// <returns></returns>
        protected JFMultiAxisPosition UnionWorkPos(string[] workPosNames)
        {
            if (null == workPosNames || 0 == workPosNames.Length)
                return null;
            JFMultiAxisPosition ret = new JFMultiAxisPosition();
            foreach(string pn in workPosNames)
            {
                JFMultiAxisPosition pos = GetWorkPosition(pn);
                if (null == pos)
                    return null;
                foreach (JFAxisPos ap in pos.Positions)
                {
                    if (!ret.ContainAxis(ap.AxisName))
                        ret.Positions.Add(JFAxisPos.Create(ap.AxisName, ap.Position));
                    else
                        ret.SetAxisPos(ap.AxisName, ap.Position);
                }


            }

            return ret;
        }

    }
}
