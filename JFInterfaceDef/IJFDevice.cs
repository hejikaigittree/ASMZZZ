using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 设备类接口定义
    /// </summary>
    public interface IJFDevice:IJFErrorCode2Txt,IJFInitializable
    {

        string DeviceModel { get; }
        /// <summary>
        /// 打开设备
        /// </summary>
        int OpenDevice();

        /// <summary>
        /// 关闭设备
        /// </summary>
        int CloseDevice();

        /// <summary>
        /// 设备是否已经打开
        /// </summary>
        bool IsDeviceOpen { get; }
    }
}
