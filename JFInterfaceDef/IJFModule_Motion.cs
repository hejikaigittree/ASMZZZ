using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{

    /// <summary>
    /// 运动参数
    /// </summary>
    //[TypeConverter(typeof(JFMotionParamTypeConvert))]
    public struct JFMotionParam
    {
        /// <summary>起始速度</summary>
        public double vs { get; set; }
        /// <summary>最大速度</summary>
        public double vm { get; set; }
        /// <summary>结束速度</summary>
        public double ve { get; set; }
        /// <summary>加速度</summary>
        public double acc { get; set; }
        /// <summary>减速度</summary>
        public double dec { get; set; }
        /// <summary>s曲线因子(0~1.0)</summary>
        public double curve { get; set; }
        /// <summary>加加速</summary>
        public double jerk { get; set; }
    }

    public struct JFHomeParam
    {
        /// <summary>归零模式  0:使用Org（原点）作为归零参考   1：使用限位信号作为归零参考   2：仅使用EZ信号作为归零参考</summary>
        public int mode { get; set; }    
        /// <summary>归零运动的方向  True:正方向</summary>
        public bool dir { get; set; }
        /// <summary>是否使用EZ信号作为归零定位 True：使用EZ辅助</summary>
        public bool eza { get; set; }       //
        /// <summary>加速度/减速度</summary>
        public double acc { get; set; }
        /// <summary>最大速度</summary>
        public double vm { get; set; }
        /// <summary>寻找原点速度</summary>
        public double vo { get; set; }
        /// <summary>接近速度</summary>
        public double va { get; set; }
        /// <summary>回零偏移量(回零后显示的位置)</summary>
        public double shift { get; set; }
        public double offset { get; set; }
    }



    /// <summary>
    /// IJFMotionCtrl 运动控制模块，
    /// </summary>
    public interface IJFModule_Motion:IJFErrorCode2Txt
    {
        /// <summary>模块是否处于打开（可用）状态</summary>
        bool IsOpen { get; }
        /// <summary> 模块包含的轴数量 /// </summary>
        int AxisCount { get; }

        #region 获取（指定轴的）单个运动状态(IO)
        /// <summary>获取报警状态</summary>
        bool IsALM(int axis);
        /// <summary>获取伺服上电状态</summary>
        bool IsSVO(int axis);
        /// <summary>获取运动完成（停止）状态</summary>
        bool IsMDN(int axis);
        /// <summary>获取运动到位状态</summary>
        bool IsINP(int axis);
        /// <summary>获取急停信号状态</summary>
        bool IsEMG(int axis);
        /// <summary>获取正限位信号状态</summary>
        bool IsPL(int axis);
        /// <summary>获取负限位信号状态</summary>
        bool IsNL(int axis);
        /// <summary>获取原点信号状态</summary>
        bool IsORG(int axis);
        /// <summary>获取软正限位状态</summary>
        bool IsSPL(int axis);
        /// <summary>获取软负限位信号状态</summary>
        bool IsSNL(int axis);

        /// <summary>
        /// 在复位动作开始后，获取复位动作是否完成（返回负值表示不支持此功能），
        /// 目前只有HTM控制卡用到此接口，其他控制卡可用MotionDone信号（IsMDN）代替
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        int IsHomeDone(int axis,out bool isDone);

        #endregion

        #region 获取（指定轴的）多个状态(IO)
        /// <summary>
        /// 轴报警信号位置，通过GetMotionIOs（axisID）函数获取的数组中的序号
        /// 如果轴不支持此信号，应该返回一个负数
        ///  MSID=MotionStatus Index
        /// </summary>
        int MSID_ALM { get; }
        int MSID_SVO { get; }
        int MSID_MDN { get; }
        int MSID_INP { get; }
        int MSID_EMG { get; }
        int MSID_PL { get; }
        int MSID_NL { get; }
        int MSID_ORG { get; }
        int MSID_SPL { get; }
        int MSID_SNL { get; }



        /// <summary>
        /// 一次获取轴的多个运动IO状态
        /// </summary>
        /// <param name="axisIndex">从0开始</param>
        /// <returns></returns>
        int GetMotionStatus(int axis,out bool[] status);
        #endregion

        #region 轴运动参数


        /// <summary>
        /// 获取轴脉冲当量
        /// 如果调用失败，会抛出一个异常
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        int GetPulseFactor(int axis,out double fact);

        /// <summary>
        /// 设置轴脉冲当量
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="plsFactor"></param>
        /// <returns>调用成功时返回0，失败则返回负数</returns>
        int SetPulseFactor(int axis, double plsFactor);


        int SetSPLimit(int axis, bool enable, double pos);
        int GetSPLimit(int axis, out bool enable, out double pos);

        int SetSNLimit(int axis, bool enable, double pos);
        int GetSNLimit(int axis, out bool enable, out double pos);

        /// <summary>
        /// 获取单轴运动参数
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        int GetMotionParam(int axis,out JFMotionParam mp);
        int SetMotionParam(int axis, JFMotionParam mp);

        /// <summary>
        /// 单轴回零参数
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        int GetHomeParam(int axis,out JFHomeParam hp);

        int SetHomeParam(int axis, JFHomeParam hp);


        #endregion

        #region 设置/获取 轴状态数据
        int GetCmdPos(int axis,out double cmdPos);
        int SetCmdPos(int axis, double cmdPos);

        int GetFbkPos(int axis,out double fbkPos);
        int SetFbkPos(int axis, double fbkPos);
        #endregion


        #region 启动/停止 清除报警 归零
        /// <summary>清除轴报警信号</summary>
        int ClearAlarm(int axis);


        int ServoOn(int axis);
        int ServoOff(int axis);



        int StopAxis(int axis);
        int StopAxisEmg(int axis);

        /// <summary>停止所有轴</summary>
        void Stop();
        /// <summary>急停所有轴</summary>
        void StopEmg();


        int Home(int axis);
        #endregion

        #region 单轴运动

        /// <summary>
        /// 单轴PTP绝对运动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        int AbsMove(int axis, double position);

        /// <summary>
        /// 单轴PTP相对运动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        int RelMove(int axis, double distance);


        #endregion

        #region 单轴速度模式运动
        int Jog(int axis,bool isPositive);


        /// <summary>
        /// 以指定的速度参数作速度运动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="speed">速度 单位/秒</param>
        /// <param name="isPositive">true:正方向   false:负方向</param>
        /// <returns></returns>
        int VelMove(int axis, double velocity,bool isPositive);

        /// <summary>
        /// 以指定的运动参数作速度运动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        int VelMove_P(int axis, JFMotionParam mp, bool isPositive);



        #endregion

        #region 插补运动
        /// <summary>
        /// 多轴直线插补（绝对方式），使用第一个轴的运动参数
        /// </summary>
        /// <param name="axisList"></param>
        /// <param name="posList"></param>
        /// <returns></returns>
        int AbsLine(int[] axisList, double[] posList);

        int AbsLine_P(int[] axisList, double[] posList, JFMotionParam mp);

        /// <summary>
        /// 以相对方式做直线插补运动
        /// </summary>
        /// <param name="axisList"></param>
        /// <param name="posList"></param>
        /// <returns></returns>
        int RelLine(int[] axisList, double[] posList);

        int RelLine_P(int[] axisList, double[] posList, JFMotionParam mp);

        /// <summary>
        /// 圆弧插补运动，以轴1运动参数
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        int AbsArc2CA(int axis1, int axis2, double center1, double center2, double angle);
        int AbsArc2CA_P(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp);

        int RelArc2CA(int axis1, int axis2, double center1, double center2, double angle);

        int RelArc2CA_P(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp);


        int AbsArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive);
        int AbsArc2CE_P(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp);

        int RelArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive);

        int RelArc2CE_P(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp);

        #endregion

        #region 缓存时运动,
        int BuffAbsLine(int[] axisList, double[] posList, JFMotionParam mp);
        int BuffRelLine(int[] axisList, double[] posList, JFMotionParam mp);
        int BuffAbsArc2CA(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp);
        int BuffRelArc2CA(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp);
        int BuffAbsArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp);
        int BuffRelArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp);
        #endregion



        #region 轴位置锁存
        /// <summary>
        /// 获取轴锁存使能状态
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        int GetLtcEnabled(int axis, out bool enabled);
        int SetLtcEnabled(int axis, bool enabled);

        /// <summary>
        /// 获取轴锁存触发电平
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="isHighLevel"></param>
        /// <returns></returns>
        int GetLtcLogic(int axis, out bool isHighLevel);
        int SetLtcLogic(int axis, bool isHighLevel);

        /// <summary>
        /// 清空锁存缓存数据
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        int ClearLtcBuff(int axis);

        /// <summary>
        /// 获取锁存位置数据
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="buff"></param>
        /// <returns></returns>
        int GetLtcBuff(int axis, out double[] buff);

        #endregion


        //string GetMCErrorInfo(int errorCode);
    }
}
