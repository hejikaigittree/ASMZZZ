using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary> 线程（工作）状态 </summary>
    public enum JFWorkStatus
    {
        UnStart = 0,    //线程未开始运行
        Running,        //线程正在运行，未退出
        Pausing,        //线程暂停中
        Interactiving,  //人机交互 ， 等待人工干预指令
        NormalEnd,     //线程正常完成后退出
        CommandExit,    //收到退出指令
        ErrorExit,      //发生错误退出，（重启或人工消除错误后可恢复）
        ExceptionExit,  //发生异常退出 ,  (不可恢复的错误)
        AbortExit,      //由调用者强制退出
    }

    /// <summary>向线程发送用户指令的执行结果</summary>
    public enum JFWorkCmdResult
    {
        UnknownError = -1, //发生未定义的错误
        Success = 0, //指令执行成功
        IllegalCmd,//不支持的非法指令
        StatusError, //工作状态（包括用户自定义状态）不支持当前指令 ，（向未运行的线程发送Resume指令）
        ActionError, //指令执行失败
        Timeout,//线程超时未响应
    }

    /// <summary>线程工作状态发生改变代理（回调）</summary>
    public delegate void WorkStatusChange(object sender, JFWorkStatus currWorkStatus);

   

    public interface IJFWork
    {
        string Name { get; set; }
        /// <summary>线程工作状态发生变化事件</summary>
        event WorkStatusChange WorkStatusChanged;


        /// <summary>当前线程工作状态</summary>
        JFWorkStatus CurrWorkStatus { get; } //当前线程工作状态


        /// <summary>开始运行</summary>
        JFWorkCmdResult Start();//开始运行

        /// <summary>停止运行</summary>
        JFWorkCmdResult Stop(int timeoutMilliseconds = -1);

        /// <summary>暂停</summary>
        JFWorkCmdResult Pause(int timeoutMilliseconds = -1);

        /// <summary>从暂停中恢复运行</summary>
        JFWorkCmdResult Resume(int timeoutMilliseconds = -1);

        /// <summary>强制停止线程运行</summary>
        void Abort();

    }



    /// <summary> 用户自定义状态发生改变代理（回调） </summary>
    public delegate void CustomStatusChange(object sender, int currCustomStatus);




    /// <summary>
    /// 可接收用户指令的工作（线程）接口类
    /// </summary>
    public interface IJFCmdWork: IJFWork
    {

        /// <summary>用户自定义状态发生变化事件</summary>
        event CustomStatusChange CustomStatusChanged;


        /// <summary>当前线程自定义状态（与工作逻辑相关）</summary>
        int CurrCustomStatus { get; } 

        /// <summary>所有自定义(工作)状态</summary>
        int[] AllCustomStatus { get; }

        /// <summary>获取自定义状态名称</summary>
        string GetCustomStatusName(int status);

        /// <summary>获取本对象（线程）支持的所有用户自定义指令</summary>
        int[] AllCmds { get; } 

        /// <summary>用户自定义指令名称</summary>
        string GetCmdName(int cmd);

        /// <summary>向线程发送一条自定义指令</summary>
        JFWorkCmdResult SendCmd(int cmd, int timeoutMilliseconds = -1);

        /// <summary>
        /// 线程内部轮询周期
        /// </summary>
        int CycleMilliseconds { get; set; }
    }


    //public enum JFWorkCmdChkCode
    //{
    //    None = -1,
    //    Stop,
    //    Run,
    //    Pause,
    //    Resume,
    //}

    ///// <summary>
    ///// 可供其他对象检查是否有WorkCmd到来的接口类
    ///// 主要用在：线程函数中需要在内循环中等待的场合
    ///// </summary>
    //public interface IJFWorkCmdChecker
    //{
    //    /// <summary>
    //    /// 检查是否收到了一条工作指令
    //    /// </summary>
    //    /// <param name="cmdCode">收到的信息</param>
    //    /// <param name="timeoutMiliSeconds">超时毫秒</param>
    //    /// <returns></returns>
    //    JFWorkCmdChkCode CheckedWorkCmd(int timeoutMiliSeconds = -1);
    //}


    //public enum JFWCAcqStatus
    //{
    //    Stoppped, //停止状态
    //    Running,  //运行状态
    //    Pausing,  //暂停状态
    //}

    ///// <summary>
    ///// 等待方法需继承此接口
    ///// </summary>
    //public interface IJFWorkCmdCheckerAcq
    //{
    //    void SetWorkCmdChecker(IJFWorkCmdChecker cmdChecked);
    //    JFWCAcqStatus CurrWorkStatus { get; }
    //}
}
