using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 触发板控制器
    /// 功能:多路输出通道 ， 可调节输出信号的强度和时长
    /// </summary>
    public interface IJFDevice_TrigController:IJFDevice
    {
        int TrigChannelCount { get; }

        /// <summary>
        /// 触发源输入通道数量
        /// </summary>
        int TrigSrcChannelCount { get; } //新添加

        /// <summary>
        /// 设置（触发）输出通道的输入源（通道）
        /// </summary>
        /// <param name="trigChannel">输出通道号</param>
        /// <param name="srcMask">输入通道位掩码</param>
        /// <returns>如果设备不支持此功能，则返回Unsupport</returns>
        int GetTrigChannelSrc(int trigChannel, out int srcMask); //新添加

        int SetTrigChannelSrc(int trigChannel, int srcMask);//




        /// <summary>获取通道使能状态</summary>
        int GetTrigChannelEnable(int channel, out bool isEnabled);
        /// <summary>设置通道使能状态</summary>
        int SetTrigChannelEnable(int channel, bool isEnable);

        int GetTrigChannelEnables(out bool[] isEnables);

        int SetTrigChannelEnables(int[]channels, bool[] isEnables);

        /// <summary>获取通道触发强度</summary>
        int GetTrigChannelIntensity(int channel, out int intensity);
        /// <summary>设置通道触发强度</summary>
        int SetTrigChannelIntensity(int channel, int intensity);

        /// <summary>获取通道触发时长</summary>
        int GetTrigChannelDuration(int channel, out int duration);
        /// <summary>设置通道触发时长</summary>
        int SetTrigChannelDuration(int channel, int duration);


        /// <summary>设置所有通道触发强度</summary>
        int SetTrigIntensity(int intensity);
        /// <summary>设置所有通道触发时长</summary>
        int SetTrigDuration(int duration);

        int SoftwareTrigAll();

        int SoftwareTrigChannel(int channel);

    }
}
