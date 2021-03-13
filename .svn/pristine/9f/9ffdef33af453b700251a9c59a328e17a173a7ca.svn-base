using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 数字量采集模块
    /// </summary>;
    public interface IJFModule_DIO:IJFErrorCode2Txt
    {
        /// <summary>模块是否处于打开（可用）状态</summary>
        bool IsOpen { get; }
        /// <summary>
        ///输入点数量 
        /// </summary>
        /// <returns></returns>
        int DICount { get; }


        /// <summary>
        /// 输出点数量 
        /// </summary>
        /// <returns></returns>
        int DOCount { get; }

        /// <summary>
        /// 获取单个输入点状态
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="index">输入点序号，从0开始</param>
        /// <returns></returns>
        int GetDI(int index,out bool isON);

        /// <summary>
        /// 获取所有的输入点状态
        /// </summary>
        /// <returns>byte[0]的最低位表示第0个输入点的当前状态</returns>
        int GetAllDIs(out bool[] isONs);

        /// <summary>
        /// 获取单个输出点状态
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="index">输出点序号，从0开始</param>
        /// <returns></returns>
        int GetDO(int index,out bool isON);

        /// <summary>
        /// 获取所有的输出点状态
        /// </summary>
        /// <returns>byte[0]的最低位表示第0个输出点的当前状态</returns>
        int GetAllDOs(out bool[] isONs);


        /// <summary>
        /// 设置单个输出点状态
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="index">输出点序号，从0开始</param>
        /// <param name="bit"></param>
        /// <returns></returns>
        int SetDO(int index, bool isON);
        /// <summary>
        /// 按顺序一次设置多个输出点状态
        /// ArgumentNull
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="bits">状态值</param>
        /// <param name="beginDOIndex">待设置输出点的起始序号（从0开始）</param>
        /// <returns></returns>
        int SetDOs(bool[] isONs, int beginIndex,int count);


        /// <summary>
        /// 一次设置多个输出点状态
        /// ArgumentNull
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="bits">待设置的状态</param>
        /// <param name="doIndexs">待设置的点位序号</param>
        /// <returns></returns>
        int SetDOs(bool[] isONs, int[] indexs);

    }
}
