using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace JFInterfaceDef
{
    /// <summary>
    /// (高) FATAL > ERROR > WARN > INFO > DEBUG (低);
    /// </summary>
    public enum JFLogLevel
    {
        /// <summary>
        /// 致命错误
        /// </summary>
        FATAL = 1,
        /// <summary>
        /// 一般错误
        /// </summary>
        ERROR = 2,
        /// <summary>
        /// 警告
        /// </summary>
        WARN = 3,
        /// <summary>
        /// 一般信息
        /// </summary>
        INFO = 4,
        /// <summary>
        /// 调试信息
        /// </summary>
        DEBUG = 5,
    }

    public interface IJFLogger
    {
        /// <summary>
        /// 日志对象的名称，全局唯一标识
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 最大存储数量（信息条数）
        /// </summary>
        int Size { get; set; }

        /// <summary>
        /// 日志中已存储的信息数量（单位：条）
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 最早一条记录的时间
        /// </summary>
        DateTime BeginTime { get; }

        /// <summary>
        /// 最晚一条记录的时间
        /// </summary>
        DateTime EndTime { get; }

        /// <summary>
        /// 需要记录的日志等级，只有高于此等级的日志才会被记录
        /// </summary>
        JFLogLevel Level { get; set; }

        /// <summary>
        /// 向日志中写入一条信息
        /// </summary>
        /// <param name="level">信息等级</param>
        /// <param name="info">信息文本</param>
        /// <param name="">format</param>
        void Log(JFLogLevel level, string format, params Object[] paramsInfo);

        /// <summary>
        /// 从已有的信息中查找符合条件的记录
        /// </summary>
        /// <param name="indexBegin">开始查询的序号（base：0）</param>
        /// <param name="count">查询信息的最大数量</param>
        /// <param name="timeBegin">开始时间（不早于次时间）</param>
        /// <param name="timeEnd">结束时间（不晚于此时间）</param>
        /// <param name="levelMin">查询的最低等级（高于或等于）</param>
        /// <param name="levelMax">查询的最高等级（低于或等于）</param>
        /// <param name="indexVisited">查询截至的记录序号（base 0，包含）</param>
        /// <returns></returns>
        DataTable PastLogs(DateTime timeBegin, DateTime timeEnd, JFLogLevel levelMin, JFLogLevel levelMax);

        /// <summary>
        /// 为Log对象添加一个显示界面
        /// </summary>
        /// <param name="tabelname"></param>
        /// <param name="ui">实时显示log信息的控件</param>
        //void AddUI(UCLogger ui);

        /// <summary>
        /// 从Log对象中删除一个显示界面
        /// </summary>
        /// <param name="tabelname"></param>
        /// <param name="ui">实时显示log信息的控件</param>
        //void RemoveUI(UCLogger ui);

    }
}
