using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{

    public enum JFCompareTrigMode
    {
        disable,//禁用触发
        liner,  //线性触发模式
        table,   //点表（非线性）触发
        //timer
    }

    /// <summary>
    /// 
    /// </summary>
    public struct JFCompareTrigLinerParam
    {
        public double start; //触发起始点
        public double interval; //触发间隔
        public int repeats;//重复次数
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is JFCompareTrigLinerParam)
            {
                var b = (JFCompareTrigLinerParam)obj;

                return this.start == b.start && this.repeats == b.repeats && this.interval == b.interval;
            }

            return base.Equals(obj);
        }
        public static bool operator ==(JFCompareTrigLinerParam left, JFCompareTrigLinerParam right)
        {
            if(left == null)
            {
                if (right == null)
                    return true;
                return false;
            }
            return left.Equals(right);
        }

        public static bool operator !=(JFCompareTrigLinerParam left, JFCompareTrigLinerParam right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + start.GetHashCode();
                hash = hash * 23 + interval.GetHashCode();
                hash = hash * 23 + repeats.GetHashCode();
                return hash;
            }
        }

    }


    /// <summary>
    /// 位置比较触发控制器
    /// </summary>
    public interface IJFModule_CmprTrigger : IJFErrorCode2Txt
    {
        /// <summary>
        /// 编码器通道数量（脉冲输入）
        /// </summary>
        int EncoderChannels { get; }

        /// <summary>
        /// 触发输出通道数量
        /// </summary>
        int TrigChannels { get; }

        /// <summary>
        /// 设置编码器脉冲当量
        /// </summary>
        /// <returns></returns>
        int SetEncoderFactor(int encChn, double factor);
        int GetEncoderFactor(int encChn, out double factor);


        /// <summary>
        /// 绑定编码器和触发通道
        /// </summary>
        /// <param name="encChn">编码器（输入）通道</param>
        /// <param name="trigChns">触发信号（输出）通道 ，如果此值为null，表示编码器不绑定任何输出通道</param>
        /// <returns></returns>
        int SetEncoderTrigBind(int encChn, int[] trigChns);
        int GetEncoderTrigBind(int encChn, out int[] trigChns);


        /// <summary>
        /// 设置编码器的脉冲触发方式
        /// </summary>
        /// <param name="EncChn">编码器通道</param>
        /// <param name="mode">触发方式 0：取消触发功能 1：线性位置触发 2：点表位置触发  3：定时器触发</param>
        /// <returns></returns>
        int SetTrigMode(int encChn, JFCompareTrigMode mode);
        int GetTrigMode(int encChn, out JFCompareTrigMode mode);

        /// <summary>
        ///  设置线性触发参数
        /// </summary>
        /// <param name="encChn">编码器通道号</param>
        /// <param name="linerParam">线性位置参数</param>
        /// <returns></returns>
        int SetTrigLiner(int encChn,JFCompareTrigLinerParam linerParam);
        int GetTrigLiner(int encChn, out JFCompareTrigLinerParam linerParam);


        /// <summary>
        /// 设置点表触发参数
        /// </summary>
        /// <param name="encChn"></param>
        /// <param name="posTable"></param>
        /// <returns></returns>
        int SetTrigTable(int encChn, double[] posTable);
        int GetTrigTable(int encChn, out double[] posTable);



        /// <summary>获取触发（输出）通道的使能状态</summary>
        int GetTrigEnable(int trigChn, out bool isEnabled);
        /// <summary>设置触发（输出）通道的使能状态</summary>
        int SetTrigEnable(int trigChn, bool isEnable);

        /// <summary>
        /// 获取触发（输出）通道已经触发的次数（从上一次置0开始）
        /// </summary>
        /// <param name="trigChn"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        int GetTriggedCount(int trigChn, out int count);
        /// <summary>
        /// 重置触发通道的触发次数为0
        /// </summary>
        /// <param name="trigChn"></param>
        /// <returns></returns>
        int ResetTriggedCount(int trigChn);

        /// <summary>
        /// 获取编码器当前位置
        /// </summary>
        /// <param name="encChn"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        int GetEncoderCurrPos(int encChn, out double pos);
        /// <summary>
        /// 设置编码器当前位置
        /// </summary>
        /// <param name="encChn"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        int SetEncoderCurrPos(int encChn, double pos); 


        /// <summary>
        /// 同步触发通道的位置与外部触发源位置一致 （HTM板卡专用）
        /// </summary>
        /// <param name="encChn"></param>
        /// <returns></returns>
        int SyncEncoderCurrPos(int encChn);

        /// <summary>
        /// 软件控制触发通道（输出一个触发信号）
        /// </summary>
        /// <param name="trigChns"></param>
        /// <returns></returns>
        int SoftTrigge(int[] trigChns);









    }
}
