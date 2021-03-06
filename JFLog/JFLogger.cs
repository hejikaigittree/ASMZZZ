using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JFInterfaceDef;

namespace JFLog
{
    public class JFLogger : IJFLogger
    {
        public class LogRecord
        {
            public LogRecord(DateTime time,JFLogLevel level,string info)
            {
                Time = time;
                Level = level;
                Info = info;
            }
            public DateTime Time { get; set; }
            public JFLogLevel Level { get; set; }
            public string Info { get; set; }
        }

        private List<UCLogger> lstUIs = new List<UCLogger>();

        /// <summary>
        /// Log集合
        /// </summary>
        public  System.Collections.Queue mLogQueue = new System.Collections.Queue();

        /// <summary>
        /// 表名称
        /// </summary>
        //private readonly string _name = string.Empty;

        /// <summary>
        /// 数据库建立连接信息
        /// </summary>
        private static string connectstring = "Data Source=" + System.Environment.CurrentDirectory + "HTTech.db";

        internal JFLogger(string name)
        {
            Name = name;
        }

        #region IJFLogger的属性
        public string Name { get ; private set ; }

        public int Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public DateTime BeginTime => throw new NotImplementedException();

        public DateTime EndTime => throw new NotImplementedException();

        public JFLogLevel Level { get; set; }
        #endregion

        #region IJFLogger的API
        /// <summary>
        /// 日志写入数据库
        /// </summary>
        /// <param name="level"></param>
        /// <param name="info"></param>
        public void Log(JFLogLevel level, string info)
        {
            lock (mLogQueue)
            {
                mLogQueue.Enqueue(new LogRecord(DateTime.Now, level, info));
            }
        }

        /// <summary>
        /// 向日志中写入一条信息
        /// </summary>
        /// <param name="level">信息等级</param>
        /// <param name="info">信息文本</param>
        /// <param name="format">信息格式</param>
        /// <param name="paramsInfo">可变数组</param>
        public void Log(JFLogLevel level, string format, params Object[] paramsInfo)
        {
            Log(level, string.Format(format, paramsInfo));
        }

        /// <summary>
        /// 从已有的信息中查找符合条件的记录
        /// </summary>
        /// <param name="timeBegin"></param>
        /// <param name="timeEnd"></param>
        /// <param name="levelMin"></param>
        /// <param name="levelMax"></param>
        /// <returns></returns>
        public DataTable PastLogs(DateTime timeBegin, DateTime timeEnd, JFLogLevel levelMin, JFLogLevel levelMax)
        {
            string minLevel = TransLogLevelToString(levelMin);
            string maxLevel = TransLogLevelToString(levelMax);

            //获取符合条件的等级范围
            string Level = "DEBUG,INFO,WARN,ERROR,FATAL".Substring("DEBUG,INFO,WARN,ERROR,FATAL".IndexOf(minLevel));
            string[] _Level = (Level.IndexOf(maxLevel) >= 0 ? Level.Substring(0, Level.IndexOf(maxLevel) + maxLevel.Length) : "").Split(',');

            string LastLevel = string.Empty;
            for (int i = 0; i < _Level.Length; i++)
            {
                if (i == (_Level.Length - 1))
                    LastLevel = " And (" + LastLevel + string.Format("LogLevel='{0}'", _Level[i]) + ")";
                else
                    LastLevel += string.Format("LogLevel='{0}' Or ", _Level[i]);
            }
            string commandText = string.Empty;
            commandText += string.Format("select * from {0} where {1} Between '{2}' a1nd '{3}'", Name, "Time", timeBegin.ToString("yyyy-MM-dd HH:mm:ss"), timeEnd.ToString("yyyy-MM-dd HH:mm:ss"));
            commandText += string.Format("{0}", LastLevel);

            DataTable ds = JFLoggerManager.jFSQLiteDB.ExecuteDataset(connectstring, CommandType.Text, commandText).Tables[0];

            return ds;
        }
        #endregion

        /// <summary>
        /// 为Log对象添加一个显示界面
        /// </summary>
        /// <param name="ui">实时显示log信息的控件</param>
        public void AddUI(UCLogger ui)
        {
            lstUIs.Add(ui);
        }

        /// <summary>
        /// 从Log对象中删除一个显示界面
        /// </summary>
        /// <param name="ui"></param>
        public void RemoveUI(UCLogger ui)
        {
            if(lstUIs.Contains(ui))
                lstUIs.Remove(ui);
        }

        /// <summary>
        /// Log输出到UCLoggers
        /// </summary>
        /// <param name="rcd"></param>
        internal void ShowInfo2UI(LogRecord rcd)
        {
            foreach (UCLogger ui in lstUIs)
                ui.ShowMsg(rcd);
        }

        /// <summary>
        /// 数据类型转换： JFLogLevel->string
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public string TransLogLevelToString(JFLogLevel level)
        {
            string _level = string.Empty;
            switch (level)
            {
                case JFLogLevel.FATAL:
                    _level = "FATAL";
                    break;
                case JFLogLevel.ERROR:
                    _level = "ERROR";
                    break;
                case JFLogLevel.WARN:
                    _level = "WARN";
                    break;
                case JFLogLevel.DEBUG:
                    _level = "DEBUG";
                    break;
                case JFLogLevel.INFO:
                    _level = "INFO";
                    break;
            }
            return _level;
        }
    }
}
