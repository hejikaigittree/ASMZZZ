using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using JFInterfaceDef;

namespace JFLog
{
    public class JFLoggerManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        JFLoggerManager()
        {

        }

        /// <summary>
        /// 唯一实例化对象
        /// </summary>
        private static readonly Lazy<JFLoggerManager> lazy = new Lazy<JFLoggerManager>(() => new JFLoggerManager());
        public static JFLoggerManager Instance { get { return lazy.Value; } }

        /// <summary>
        /// Dictionary锁
        /// </summary>
        public object locker = new object();
        /// <summary>
        /// Key:Table Name  / Value:IJFLogger
        /// </summary>
        private Dictionary<string, IJFLogger> existtLoggers = new Dictionary<string, IJFLogger>();
        /// <summary>
        /// Key:Table Name  / Value:UCLogger
        /// </summary>
        public static List<KeyValuePair<string, UCLogger>> waitBindUIs = new List<KeyValuePair<string, UCLogger>>();
        /// <summary>
        /// SQLite对象实例化
        /// </summary>
        public static JFSQLiteDB jFSQLiteDB = new JFSQLiteDB();
        private Thread threadRunTask = null;

        /// <summary>
        /// 写日志退出条件
        /// </summary>
        public bool mGetLogQueueFlg=true;
        /// <summary>
        /// 数据库建立连接信息
        /// </summary>
        private static string connectstring = "Data Source=" + System.Environment.CurrentDirectory + "\\HTTech.db";

        /// <summary>
        /// 获取一个日志对象
        /// </summary>
        /// <param name="name">日志对象（名称）标识</param>
        /// <returns></returns>
        public IJFLogger GetLogger(string name)
        {
            string connectionCreatTableString = string.Format("CREATE TABLE IF NOT EXISTS {0}(Time string,LogLevel string,Message string)", name);
            if (threadRunTask == null)
            {
                threadRunTask = new Thread(RunTask);
                threadRunTask.Start();
            }
            if (string.IsNullOrEmpty(name))
                return null;
            lock (locker)
            {
                if (existtLoggers.ContainsKey(name))
                    return existtLoggers[name] as IJFLogger;

                JFLogger ret = new JFLogger(name);
                if (jFSQLiteDB.CreateDB(connectstring))
                    jFSQLiteDB.CreateOrDeleteTable(connectionCreatTableString);

                existtLoggers.Add(name, ret);
                for(int i = 0; i < waitBindUIs.Count;i++)
                {
                    KeyValuePair<string, UCLogger> item = waitBindUIs[i];
                    if (item.Key == name)
                    {
                        ret.AddUI(item.Value);
                        waitBindUIs.Remove(item);
                        i--;
                    }
                }
                return ret as IJFLogger; 
            }
        }

        /// <summary>
        /// Log对象与日志界面绑定
        /// </summary>
        /// <param name="logName"></param>
        /// <param name="ui"></param>
        public void BindUI(string logName,UCLogger ui)
        {
            if (string.IsNullOrEmpty(logName))
                return;
            lock (locker)  
            {
                if (!existtLoggers.ContainsKey(logName))
                {
                    KeyValuePair<string, UCLogger> keyValuePair = new KeyValuePair<string, UCLogger>(logName, ui);
                    waitBindUIs.Add(keyValuePair);
                    return;
                }
                if(typeof(JFLogger) == existtLoggers[logName].GetType() || typeof(JFLogger).IsAssignableFrom(existtLoggers[logName].GetType()))
                    (existtLoggers[logName] as JFLogger).AddUI(ui);
            }
        }

        /// <summary>
        /// 日志写入以及日志界面更新Task
        /// </summary>
        private void RunTask()
        {
            while (mGetLogQueueFlg)
            {
                lock (locker)
                {
                    foreach (KeyValuePair<string, IJFLogger> logger in existtLoggers)
                    {
                        JFLogger log = logger.Value as JFLogger;
                        if (log.mLogQueue.Count > 0)
                        {
                            JFLogger.LogRecord rcd;
                            lock (log.mLogQueue)
                            {
                                rcd = log.mLogQueue.Dequeue() as JFLogger.LogRecord;
                            }
                            //存文件
                            log.ShowInfo2UI(rcd);
                            //写日志
                            string commandtext = string.Format("insert into {0} values('{1}','{2}','{3}')", logger.Key, rcd.Time.ToString("yyyy-MM-dd HH:mm:ss"), log.TransLogLevelToString(rcd.Level), rcd.Info);
                            jFSQLiteDB.ExecuteNonQuery(connectstring, CommandType.Text, commandtext);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 终止线程
        /// </summary>
        public void Stop()
        {
            if (threadRunTask != null)
            {
                mGetLogQueueFlg = false;
                if(!threadRunTask.Join(1000))
                    threadRunTask.Abort();
                threadRunTask = null;
            }
        }
    }
}
