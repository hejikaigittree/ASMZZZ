using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 需要序列化和动态创建的类 可以继承此接口
    /// 实例类必须提供一个无参构造函数
    /// </summary>
    public interface IJFInitializable:IDisposable
    {
        
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        string[] InitParamNames { get; }


        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        JFParamDescribe GetInitParamDescribe(string name);

        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        object GetInitParamValue(string name);

        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        bool SetInitParamValue(string name, object value);

        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        bool Initialize();


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        bool IsInitOK { get; }

        /// <summary>获取初始化错误的描述信息</summary>
        string GetInitErrorInfo();
    }
}
