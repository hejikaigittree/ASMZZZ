using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 光源控制器（不包含触发功能）
    /// </summary>
    public interface IJFDevice_LightController: IJFDevice
    {
        /// <summary>光源通道数量</summary>
        int LightChannelCount { get; }

        /// <summary>获取通道使能状态（打开或关闭）</summary>
        int GetLightChannelEnable(int channel, out bool isTurnOn);
        /// <summary>设置通道开关</summary>
        int SetLightChannelEnable(int channel, bool isTurnOn);
        /// <summary>获取所有通道的开关状态</summary>
        int GetLightChannelEnables(out bool[] isTurnOns);
        /// <summary>一次设置多个通道的开关状态</summary>
        int SetLightChannelEnables(int[] channels, bool[] isTurnOns);

        /// <summary>获取通道的(常亮时的)亮度值</summary>
        int GetLightIntensity(int channel,out int intensity);
        /// <summary>设置通道的亮度值</summary>
        int SetLightIntensity(int channel,  int intensity);

    }
}
