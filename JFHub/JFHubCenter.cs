using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JFInterfaceDef;
using JFToolKits;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32; //注册表
using JFLog;
using System.Threading;

namespace JFHub
{

    public class JFHubCenter : IDisposable
    {
        #region  ConfigKey
        public static string CK_ExpandDllFiles = "拓展dll文件";//拓展dll文件，Key = string,ValueType = List<string>
        public static string CK_InitDevParams = "子设备初始化参数";//用于保存系统初始化时创建设备(运动控制器/相机/机械手)对象的参数，Key = string, ValueType = SortedDictionary ,SortedDictionary[Key = DeviceID,value = List<object>]
        #endregion


        #region ConfigTag
        static string CT_DLL = "Dll文件管理";
        static string CT_DEV = "设备管理";

        #endregion

        /// <summary>
        /// </summary>
        JFHubCenter()
        {
            Initialize();
        }

        ~JFHubCenter()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            ////////////释放非托管资源
            if (disposing)//////////////释放其他托管资源
            {

            }
        }


        private static readonly Lazy<JFHubCenter> lazy = new Lazy<JFHubCenter>(() => new JFHubCenter());
        public static JFHubCenter Instance { get { return lazy.Value; } }




        /// <summary>系统配置对象</summary>
        public JFXCfg SystemCfg { get; private set; }


        /// <summary>JFInatializerable对象管理器</summary>
        public JFinitializerHelper InitorHelp { get; private set; }

        JFInitorManager _initMgr = null;
        object initMgrLock = new object();
        public JFInitorManager InitorManager
        {
            get
            {
                if (_initMgr != null)
                    return _initMgr;
                else
                {
                    Monitor.Enter(initMgrLock);
                    if (_initMgr != null)
                    {
                        Monitor.Exit(initMgrLock);
                        return _initMgr;
                    }
                    ////根据设备参数表，创建各种设备（运动控制卡/相机/光源）
                    _initMgr = new JFInitorManager();
                    _initMgr.Init();
                }
                Monitor.Exit(initMgrLock);
                return _initMgr;
            }

        }
    
        


        


        /// <summary>初始化HubCenter</summary>
        void Initialize()
        {
            dataPool = new JFDataPool();

            string chkError = "";
            string sysCfgFile = SystemCfgFilePath;
            while (string.IsNullOrEmpty(sysCfgFile) //系统配置文件未设置
                || !File.Exists(sysCfgFile)      //系统配置文件已设置，但是文件不存在
                || !_CheckSysCfg(sysCfgFile, false, out chkError)) //系统文件已存在，但是格式不正确
            {

                FormSelCfg fm = new FormSelCfg();
                if (string.IsNullOrEmpty(sysCfgFile))
                {
                    fm.Tips = "系统配置文件未设置！";
                }
                else if (!File.Exists(sysCfgFile))
                {
                    fm.Tips = "系统配置文件:" + "\"" + sysCfgFile + "\"不存在！\n请检查路径或选择/创建新的配置文件";
                }
                else//(!_CheckSysCfg(SystemCfgFilePath,false)) //文件已存在，但格式不正确（缺少必要的数据项）
                {
                    fm.Tips = "系统配置文件:" + "\"" + sysCfgFile + "\"格式错误！\nError:" + chkError + "\n请检查文件或选择/创建新的配置文件";

                }

                fm.ShowDialog();//////////FormInitHub.ShowDialog()中有退出程序的出口，如果运行到下一步，肯定是已经选择了配置文件
                sysCfgFile = fm.SysCfgFilePath;
                if (_CheckSysCfg(sysCfgFile, true, out chkError))
                {
                    SetSystemCfgFilePath(sysCfgFile);//退出程序，重新启动
                    break;
                }
            }//end while , 系统配置文件检查OK
            SystemCfg = new JFXCfg();
            SystemCfg.Load(sysCfgFile,false);
            InitorHelp = new JFinitializerHelper();
            //加载JFDll库文件
            List<string> dllFiles = SystemCfg.GetItemValue(CK_ExpandDllFiles) as List<string>;
            foreach(string dllFile in dllFiles)
            {
                try
                {
                    InitorHelp.AppendDll(dllFile);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Hub-Center 加载dll = \"" + dllFile + "\"异常：\n" + ex);
                }
            }

            _mdCellNameMgr = new JFDevCellNameManeger(sysCfgFile);//运动控制卡单元名称管理



            return;
            
        }


        /// <summary>
        /// 关闭工站/设备，释放各种资源
        /// </summary>
        public void Close()
        {
            string errInfo = "";
            ///关闭工站
            IJFMainStation mainStation = StationMgr.MainStation;
            if (null != mainStation)
                mainStation.Stop(out errInfo);
            string[] stationNames = StationMgr.AllStationNames();
            if(null != stationNames && stationNames.Length > 0)
                foreach(string stationName in stationNames)
                {
                    IJFStation station = StationMgr.GetStation(stationName);
                    station.Stop();
                }
            ///关闭所有设备
            string[] deviceIDs = InitorManager.GetIDs(typeof(IJFDevice));
            if(null != deviceIDs && deviceIDs.Length > 0)
                foreach(string devID in deviceIDs)
                {
                    IJFDevice dev = InitorManager.GetInitor(devID) as IJFDevice;
                    dev.CloseDevice();
                }

            ///释放其他对象
            ///添加代码 ... 
        }

        /// <summary>
        /// 检查配置文件是否合规（如果缺少必要的数据项，则返回false）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="isOpenOrCreate">如果文件不存在，是否创建</param>
        bool _CheckSysCfg(string filePath,bool isOpenOrCreate,out string errorInfo)
        {
            errorInfo = "";
            if (string.IsNullOrEmpty(filePath))
            {
                errorInfo = "文件名为空值（或空格）";
                return false;
            }



            if (!File.Exists(filePath)) //文件不存在
            {
                if(!isOpenOrCreate)
                {
                    errorInfo = "文件不存在";
                    return false;
                }
                try
                {
                    JFXCfg cfg = new JFXCfg();
                    cfg.Load(filePath, true);
                    cfg.AddItem(CK_ExpandDllFiles, new List<string>(), CT_DLL);
                    cfg.AddItem(CK_InitDevParams, new JFXmlSortedDictionary<string,List<object>>(),CT_DEV);      
                    ////////////////////////////////////////////////
                    ///添加其他配置项初始化动作
                    ///
                    ///

                    cfg.Save();
                    return true;
                }
                catch(Exception ex)
                {
                    errorInfo = ex.Message;
                    return false;
                }
               
                //break;
            }
            else //文件已存在，检查格式（只检查必须项是否存在）
            {
                try
                {
                    bool isCheckOK = true;
                    StringBuilder sbError = new StringBuilder();
                    JFXCfg cfg = new JFXCfg();
                    cfg.Load(filePath, false);
                    if (!cfg.ContainsItem(CK_ExpandDllFiles))
                    {
                        sbError.Append( "文件中不存在配置项:" + CK_ExpandDllFiles +"\n");
                        isCheckOK =  false;
                    }

                    if (!cfg.ContainsItem(CK_InitDevParams))
                    {
                        sbError.Append("文件中不存在配置项:" + CK_InitDevParams + "\n");
                        isCheckOK = false;
                    }

                    ////////////////////////////////////////////////
                    ///添加其他配置项检查动作
                    ///
                    ///
                    if (!isCheckOK)
                        errorInfo = sbError.ToString();
                    return isCheckOK;
                }
                catch (Exception ex)
                {
                    errorInfo = ex.Message;
                    return false;
                }

            }

        }





        /// <summary>获取配置文件路径（配置文件名存放在注册表中 ，key = AppPath）</summary>
        public string SystemCfgFilePath 
        { 
            get
            {
                RegistryKey rkLoc = Registry.LocalMachine;
                RegistryKey rkJF = rkLoc.CreateSubKey("SOFTWARE\\JoinFrame");
//#if DEBUG    //Debug版使用程序文件路径作为Key
                string appKey = Application.ExecutablePath.Replace('\\', '/');
//#else       //Release版使用文件MD5值作为Key
                //string appKey = JFFunctions.GetFileMD5(Application.ExecutablePath);
//#endif
                RegistryKey rkApp = rkJF.CreateSubKey(appKey);//
                return rkApp.GetValue("ConfigFile") as string;
            }
            
        }

        public void SetSystemCfgFilePath(string filePath)
        {
            if(string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("设置系统配置文件失败：文件名为空!");
            }

            if (filePath == SystemCfgFilePath)
                return;

            RegistryKey rkLoc = Registry.LocalMachine;
            RegistryKey rkJF = rkLoc.CreateSubKey("SOFTWARE\\JoinFrame");
            string appPath = Application.ExecutablePath;
            RegistryKey rkApp = rkJF.CreateSubKey(appPath.Replace('\\', '/'));//


            rkApp.SetValue("ConfigFile", filePath);

            MessageBox.Show("系统配置文件路径设置成功！程序将重新启动");
            ///更改配置文件后需要重新启动
            Application.Exit();
            System.Diagnostics.Process.Start(appPath);
        }

        JFDevCellNameManeger _mdCellNameMgr = null; //控制卡名称管理
        public JFDevCellNameManeger MDCellNameMgr { get { return _mdCellNameMgr; } }


        JFDataPool dataPool = null;
        public IJFDataPool DataPool { get { return dataPool; } }


        JFStationManager _stationMgr = null;// new JFStationManager();
        public JFStationManager StationMgr 
        { 
            get 
            {
                if (_stationMgr == null)
                {
                    lock (this)
                    {
                        if (null == _stationMgr)
                        {
                            string stationMgrCfg = Path.GetDirectoryName(SystemCfgFilePath);
                            stationMgrCfg += "\\StationMgr.xml";
                            _stationMgr = new JFStationManager(stationMgrCfg);//工站管理器
                        }
                        
                    }
                }
                return _stationMgr;
            }
        }

        JFVisionManager _visionMgr = null;

        public JFVisionManager VisionMgr
        {
            get
            {
                if(null == _visionMgr)
                {
                    string visionMgrCfg = Path.GetDirectoryName(SystemCfgFilePath);
                    visionMgrCfg += "\\VisionMgr.xml";
                    _visionMgr = new JFVisionManager(visionMgrCfg);
                }
                return _visionMgr;
            }
        }

        IJFLogger _systemLogger = JFLoggerManager.Instance.GetLogger("System");
        public IJFLogger SystemLog { get { return _systemLogger; } }


        /// <summary>
        ///  产品配方管理
        /// </summary>
        public IJFRecipeManager RecipeManager
        {
            get
            {
                JFInitorManager initMgr = InitorManager;
                string[] recipeMgrNames= initMgr.GetIDs(typeof(IJFRecipeManager));
                //initMgr.Remove("JF产品配方");
                if (null == recipeMgrNames || 0 == recipeMgrNames.Length)
                    return null;
                if (recipeMgrNames.Length > 1)
                    throw new Exception("RecipeManager count = " + recipeMgrNames.Length + " in SystemConfig");
                return initMgr.GetInitor(recipeMgrNames[0]) as IJFRecipeManager;
                
            }
        }

    }
}
