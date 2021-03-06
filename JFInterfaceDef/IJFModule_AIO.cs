using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    public interface IJFModule_AIO : IJFErrorCode2Txt
    {
        /// <summary>模块是否处于打开（可用）状态</summary>
        bool IsOpen { get; }



        int GetAIFactor(int index, out double factor);

        int SetAIFactor(int index,double factor);

        int GetAOFactor(int index, out double factor);

        int SetAOFactor(int index, double factor);


        /// <summary>
        ///输入点数量 
        /// </summary>
        /// <returns></returns>
        int AICount { get; }


        /// <summary>
        /// 输出点数量 
        /// </summary>
        /// <returns></returns>
        int AOCount { get; }

        /// <summary>
        /// 获取单个输入点模拟量
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="index">输入点序号，从0开始</param>
        /// <returns></returns>
        int GetAI(int index, out double volt);

        /// <summary>
        /// 获取所有的输入点状态
        /// </summary>
        int GetAllAIs(out double[] volts);


        /// <summary>
        /// 复位AI，将当前AI通道采集的数值置为0
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int ResetAI(int index);

        /// <summary>
        /// 获取单个输出点状态
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="index">输出点序号，从0开始</param>
        /// <returns></returns>
        int GetAO(int index, out double volt);

        /// <summary>
        /// 获取所有的输出点状态
        /// </summary>
        int GetAllAOs(out double[] volt);


        /// <summary>
        /// 设置单个输出点状态
        /// ArgumentOutofRange
        /// </summary>
        /// <param name="index">输出点序号，从0开始</param>
        int SetAO(int index, double volt);
        /// <summary>
        /// 按顺序一次设置多个输出点状态
        /// ArgumentNull
        /// ArgumentOutofRange
        /// </summary>
        int SetAOs(double[] volts, int beginIndex, int count);


        /// <summary>
        /// 一次设置多个输出点状态
        /// ArgumentNull
        /// ArgumentOutofRange
        /// </summary>
        /// <returns></returns>
        int SetAOs(double[] volts, int[] indexs);



    }
}
