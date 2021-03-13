using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HT_Lib;
using JFInterfaceDef;

namespace JFHTMDevice
{

    /// <summary>
    /// MC_Htm ,HTM(景焱)运动控制器模块
    /// </summary>
    public class HtmMC : IJFModule_Motion
    {
        internal HtmMC()
        {
            AxisCount = 0;
            IsOpen = false;
        }
        enum ErrorDef
        {
            Success = 0,//操作成功，无错误
            Unsupported = -1,//设备不支持此功能
            ParamError = -2,//参数错误（不支持的参数）
            InvokeFailed = -3,//库函数调用出错
            Allowed = 1,//调用成功，但不是所有的参数都支持
            InitFailedWhenOpenCard = -4,//
        }

        public string GetErrorInfo(int errorCode)
        {

            switch (errorCode)
            {
                case (int)ErrorDef.Success://操作成功，无错误
                    return "Success";
                case (int)ErrorDef.Unsupported://设备不支持此功能
                    return "Unsupported";
                case (int)ErrorDef.ParamError://参数错误（不支持的参数）
                    return "Param Error";
                case (int)ErrorDef.InvokeFailed://库函数调用出错
                    return "Inner API invoke failed";
                case (int)ErrorDef.Allowed://调用成功，但不是所有的参数都支持
                    return "Allowed,Not all param are supported";
                case (int)ErrorDef.InitFailedWhenOpenCard:
                    return "Not initialized when open ";
                default://未定义的错误类型
                    return "Unknown-Error";
            }
        }

        public bool IsOpen { get; private set; }



        /// <summary>运动控制卡初始化 </summary>

        internal void Open()
        {
            AxisCount = HTM.GetAxisNum();
            if (AxisCount > 0)
            {
                axisHomeDones = new bool[AxisCount];
                detectHomeDones = new Thread[AxisCount];
                _motionParams = new JFMotionParam[AxisCount];
                for (int i = 0; i < AxisCount; i++)
                {
                    HTM.AXIS_INFO ai;
                    HTM.GetAxisInfo(i, out ai); 
                    HTM.MOTION_PARA mp = HTM.GetMotionPara(i);//HTM.GetMovePara(i, out mp);
                    //if (ret != 0)
                    //{
                    //    AxisCount = 0;
                    //    _motionParams = null;
                    //    throw new Exception(string.Format("MC_Htm.Init Failed :HTM.GetMovePara(axis = {0}) return errorCode = {1}", i, ret));
                    //}
                    _motionParams[i] = new JFMotionParam();
                    _motionParams[i].vs = mp.vStart;
                    _motionParams[i].vm = mp.vMax;
                    _motionParams[i].ve = mp.vEnd;
                    _motionParams[i].acc = mp.acc;
                    _motionParams[i].dec = mp.dec;
                    _motionParams[i].curve = mp.sFactor;
                    _motionParams[i].jerk = 0;
                    axisHomeDones[i] = false;
                    detectHomeDones[i] = new Thread(new ParameterizedThreadStart(FuncDetectHomeDone));
                    
                }
                
            }
            IsOpen = true;
        }

        void FuncDetectHomeDone(object oAxisIndex/*int axis*/)
        {
            int axis = (int)oAxisIndex;
            if (0 == HTM.HomeDone(axis))
                axisHomeDones[axis] = true;
            else
                axisHomeDones[axis] = false;

        }

        internal void Close()
        {
            AxisCount = 0;
            _motionParams = null;
            IsOpen = false;
        }

        /// <summary>检查轴序号是否合法，内部函数使用</summary>
        void _CheckAxisEnable(int axis, string funcName)
        {
            if (!IsOpen)
                throw new Exception("MC_Htm." + funcName + "() failed: MC is not Open!");
            if (axis < 0 || axis >= AxisCount)
                throw new ArgumentOutOfRangeException(string.Format("MC_Htm.{0}() failed:axis={1} is out of range(Axis Count = {2})", funcName, axis, AxisCount));
        }

        /// <summary>检查轴序号是否合法，内部函数使用</summary>
        void _CheckAxisEnable(int[] axes, string funcName)
        {
            if (!IsOpen)
                throw new Exception("MC_Htm." + funcName + "() failed: MC is not open!");
            if (null == axes || 0 == axes.Length)
                throw new ArgumentOutOfRangeException(string.Format("MC_Htm.{0}() failed: param axes is null or empty)", funcName));
            foreach (int axis in axes)
                if (axis < 0 || axis >= AxisCount)
                    throw new ArgumentOutOfRangeException(string.Format("MC_Htm.{0}() failed:axis={1} is out of range(Axis Count = {2})", funcName, axis, AxisCount));
        }

        /// <summary> 模块包含的轴数量</summary>
        public int AxisCount { get; private set; }


        #region 获取（指定轴的）单个运动状态(IO)
        public bool IsALM(int axis)
        {
            _CheckAxisEnable(axis, "IsAlarm");
            return HTM.GetAlarm(axis) == 1;//.GetMotionIO(axis, MotionIO.ALM) == 1;
        }

        public bool IsSVO(int axis)
        {
            _CheckAxisEnable(axis, "IsSVO");
            return HTM.GetSVON(axis) == 1;//GetMotionIO(axis, MotionIO.SVON) == 1;
        }
        public bool IsMDN(int axis)
        {
            _CheckAxisEnable(axis, "IsMDN");
            return HTM.OnceDone(axis) == 1;
        }
        public bool IsINP(int axis)
        {
            _CheckAxisEnable(axis, "IsINP");
            return HTM.GetINP(axis) == 1;
        }
        public bool IsEMG(int axis)
        {
            //_CheckAxisEnable(axis, "IsEMG");
            //return HTM.GetAlarm(axis) == 1; //未找到急停信号接口，暂时以报警信号代替
            return false;
        }
        public bool IsPL(int axis)
        {
            _CheckAxisEnable(axis, "IsPL");
            return HTM.GetPEL(axis) == 1;
        }
        public bool IsNL(int axis)
        {
            _CheckAxisEnable(axis, "IsNL");
            return HTM.GetMEL(axis) == 1;
        }
        public bool IsSPL(int axis)
        {
            _CheckAxisEnable(axis, "IsSPL");
            return HTM.GetMotionIO(axis, HTM.MotionIO.SPEL) == 1;
        }

        public bool IsSNL(int axis)
        {
            _CheckAxisEnable(axis, "IsSNL");
            return HTM.GetMotionIO(axis, HTM.MotionIO.SMEL) == 1;
        }
        public bool IsORG(int axis)
        {
            _CheckAxisEnable(axis, "IsORG");
            return HTM.GetMotionIO(axis, HTM.MotionIO.ORG) == 1;
        }

        bool[] axisHomeDones = null; //景焱控制卡专用 ,用于标识轴是否复位完成，在Home执行时开启一个detect线程
        Thread[] detectHomeDones = null;
        public int IsHomeDone(int axis, out bool isDone)
        {
            isDone = axisHomeDones[axis];
            
            return 0;
        }
        #endregion

        #region 获取（指定轴的）多个运动状态(IO)

        /// <summary>
        /// 轴报警信号位置，GetAxisStatus（axisID）函数获取的数组中的序号
        ///  MSID=MotionStatus Index
        /// </summary>
        public int MSID_ALM { get { return 0; } }
        public int MSID_SVO { get { return 1; } }
        public int MSID_MDN { get { return 2; } }
        public int MSID_INP { get { return 3; } }
        public int MSID_EMG { get { return -1; } }//HTM不支持获取EMG信号功能
        public int MSID_PL { get { return 4; } }
        public int MSID_NL { get { return 5; } }

        public int MSID_SPL { get { return 6; } }
        public int MSID_SNL { get { return 7; } }

        public int MSID_ORG { get { return 8; } }


        /// <summary>
        /// 一次获取轴的多个运动IO状态
        /// </summary>
        /// <param name="axisIndex">从0开始</param>
        /// <returns></returns>
        public int GetMotionStatus(int axis,out bool[] ret)
        {
            _CheckAxisEnable(axis, "GetMotionStatus");
            ret = new bool[9];
            HTM.AXIS_STATUS axs;
            int opt = HTM.GetAxisStatus(axis, out axs);
            if (opt != 0)
                throw new Exception("MC_Htm.GetMotionStatus failed:HTM.GetAxisStatus return error code = " + opt.ToString());
            //通过背景线程获取
            ret[MSID_ALM] = axs.alm == 1;
            ret[MSID_SVO] = axs.svon == 1;
            ret[MSID_INP] = axs.inp == 1;
            //ret[MSID_EMG] = axs.alm == 1;
            ret[MSID_PL] = axs.pel == 1;
            ret[MSID_NL] = axs.mel == 1;
            ret[MSID_ORG] = axs.org == 1;
            //实时获取
            ret[MSID_SPL] = HTM.GetMotionIO(axis, HTM.MotionIO.SPEL) == 1;
            ret[MSID_SNL] = HTM.GetMotionIO(axis, HTM.MotionIO.SMEL) == 1;
            ret[MSID_MDN] = HTM.OnceDone(axis) == 1;

            return (int)ErrorDef.Success;

        }
        #endregion

        #region 轴参数 
        public int GetPulseFactor(int axis,out double fact)
        {
            _CheckAxisEnable(axis, "GetPulseFactor");
            fact =  HTM.GetPulseFactor(axis);
            return (int)ErrorDef.Success;
        }
        public int SetPulseFactor(int axis, double plsFactor)
        {
            _CheckAxisEnable(axis, "SetPulseFactor");
            HTM.AXIS_INFO ai;
            int opt = HTM.GetAxisInfo(axis, out ai);
            if (0 != opt)
                throw new Exception("MDD_Htm.SetPulseFactor failed: HTM.GetAxisInfo return errorCode = " + opt);
            ai.pulseFactor = plsFactor;
            opt = HTM.SetAxisInfo(axis, ref ai);
            if (0 != opt)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        public int GetSPLimit(int axis, out bool enable, out double pos)
        {
            _CheckAxisEnable(axis, "GetSPLimit");
            HTM.AXIS_INFO axi;
            enable = false;
            pos = 0;
            int nRet = HTM.GetAxisInfo(axis, out axi);
            if (nRet != 0)
                return (int)ErrorDef.Success;
            if ((axi.enableSEL & 0x02) != 0)
                enable = true;
            else
                enable = false;
            pos = axi.sPELPos;
            return (int)ErrorDef.Success;
        }


        public int SetSPLimit(int axis, bool enable, double pos)
        {
            _CheckAxisEnable(axis, "SetSPLimit");
            HTM.AXIS_INFO axi;
            int nRet = HTM.GetAxisInfo(axis, out axi);
            if (nRet != 0)
                throw new Exception("MDD_Htm.SetSPLimit failed: HTM.GetAxisInfo return errorCode = " + nRet);
            if (enable)
            {
                axi.enableSEL = (byte)(axi.enableSEL | 0x02);
                axi.sPELPos = pos;
            }
            else
            {
                axi.enableSEL = (byte)(axi.enableSEL & 0x01);
            }
            int opt = HTM.SetAxisInfo(axis, ref axi);
            if (0 != opt)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;

        }

        public int GetSNLimit(int axis, out bool enable, out double pos)
        {
            _CheckAxisEnable(axis, "GetSNLimit");
            enable = false;
            pos = 0;
            HTM.AXIS_INFO axi;
            int nRet = HTM.GetAxisInfo(axis, out axi);
            if (nRet != 0)
                return (int)ErrorDef.InvokeFailed;   
            if ((axi.enableSEL & 0x01) != 0)
                enable = true;
            else
                enable = false;
            pos = axi.sMELPos;
            return (int)ErrorDef.Success;
        }

        public int SetSNLimit(int axis, bool enable, double pos)
        {
            _CheckAxisEnable(axis, "SetSNLimit");
            HTM.AXIS_INFO axi;
            int nRet = HTM.GetAxisInfo(axis, out axi);
            if (nRet != 0)
                throw new Exception("MDD_Htm.SetSNLimit failed: HTM.GetAxisInfo return errorCode = " + nRet);

            if (enable)
            {
                axi.enableSEL = (byte)(axi.enableSEL | 0x01);
                axi.sMELPos = pos;
            }
            else
            {
                axi.enableSEL = (byte)(axi.enableSEL & 0x02);
            }
            int opt = HTM.SetAxisInfo(axis, ref axi);
            if (0 != opt)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }


        /// <summary>
        /// 获取单轴运动参数
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public int GetMotionParam(int axis, out JFMotionParam mp)
        {
            _CheckAxisEnable(axis, "GetMotionParam");
            mp = _motionParams[axis];
            return (int)ErrorDef.Success;
        }
        public int SetMotionParam(int axis, JFMotionParam mp)
        {
            _CheckAxisEnable(axis, "GetMotionParam");
            _motionParams[axis] = mp;
            return 0;
        }

        /// <summary>单轴回零参数</summary>
        public int GetHomeParam(int axis,out JFHomeParam pm)
        {
            _CheckAxisEnable(axis, "GetHomeParam");
            HTM.AXIS_INFO axInfo;
            int errCode = HTM.GetAxisInfo(axis, out axInfo);
            if (errCode != 0)
            {
                pm = new JFHomeParam();
                return (int)ErrorDef.InvokeFailed;
            }
            pm = new JFHomeParam()
            {
                mode = axInfo.homeMode,
                dir = axInfo.homeDir != 0,
                eza = axInfo.homeEZA != 0,
                acc = axInfo.homeAcc,
                vm = axInfo.homeVm,
                vo = axInfo.homeVo,
                va = axInfo.homeVo,
                shift = axInfo.homeShift,
                offset = 0
            };
            return (int)ErrorDef.Success;
        }

        public int SetHomeParam(int axis, JFHomeParam hp)
        {
            _CheckAxisEnable(axis, "SetHomeParam");
            HTM.AXIS_INFO axInfo;
            int opt = HTM.GetAxisInfo(axis, out axInfo);
            if (opt != 0)
                throw new Exception("SetHomeParam() Failed By: HTM.GetAxisInfo() return errorCode = " + opt);

            axInfo.homeMode = (sbyte)hp.mode;
            axInfo.homeDir = (byte)(hp.dir ? 1 : 0);
            axInfo.homeEZA = (byte)(hp.eza ? 1 : 0);
            axInfo.homeAcc = hp.acc;
            axInfo.homeVm = hp.vm;
            axInfo.homeVo = hp.vo;
            axInfo.homeShift = hp.shift;
            opt = HTM.SetAxisInfo(axis, ref axInfo);
            if (0 != opt)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }


        #endregion

        #region 设置/获取 轴位置数据
        public int  GetCmdPos(int axis,out double cmdPos)
        {
            _CheckAxisEnable(axis, "GetCmdPos");
            cmdPos = HTM.GetCmdPos(axis);
            return (int)ErrorDef.Success;
        }
        public int SetCmdPos(int axis, double cmdPos)
        {
            _CheckAxisEnable(axis, "SetCmdPos");
            if (0 != HTM.SetCmdPos(axis, cmdPos))
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;

        }

        public int GetFbkPos(int axis,out double fbkPos)
        {
            _CheckAxisEnable(axis, "GetFbkPos");
            fbkPos = HTM.GetFbkPos(axis);
            return (int)ErrorDef.Success;
        }
        public int SetFbkPos(int axis, double fbkPos)
        {
            _CheckAxisEnable(axis, "SetFbkPos");
            if (0 != HTM.SetFbkPos(axis, fbkPos))
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }
        #endregion

        #region 启动/停止 清除报警 归零
        /// <summary>清除轴报警信号</summary>
        /// <summary>
        /// 清除轴报警信号
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public int ClearAlarm(int axis)
        {
            _CheckAxisEnable(axis, "ClearAlarm");
            if (0 != HTM.ClearAlarm(axis))
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }


        public int ServoOn(int axis)
        {
            _CheckAxisEnable(axis, "ServoOn");
            if (0 != HTM.SetSVON(axis, 1))
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;

        }
        public int ServoOff(int axis)
        {
            _CheckAxisEnable(axis, "ServoOff");
            if (0 != HTM.SetSVON(axis, 0))
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }


        public int StopAxis(int axis) //如果伺服未上电，调用HTM.Stop会闪退
        {
            _CheckAxisEnable(axis, "StopAxis");
            if (0 != HTM.Stop(axis, HTM.StopMode.DEC))
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }
        public int StopAxisEmg(int axis)
        {
            _CheckAxisEnable(axis, "StopAxisEmg");
            if (0 != HTM.Stop(axis, HTM.StopMode.EMG))
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        /// <summary>停止所有轴</summary>
        public void Stop()
        {
            for (int i = 0; i < AxisCount; i++)
                HTM.Stop(i, HTM.StopMode.DEC);
        }
        /// <summary>急停所有轴</summary>
        public void StopEmg()
        {
            for (int i = 0; i < AxisCount; i++)
                HTM.Stop(i, HTM.StopMode.EMG);
        }


        public int Home(int axis)
        {
            _CheckAxisEnable(axis, "Home");
            axisHomeDones[axis] = false;
            if (0 != HTM.Home(axis))
                return (int)ErrorDef.InvokeFailed;
            if (detectHomeDones[axis].IsAlive)
                detectHomeDones[axis].Abort();
            detectHomeDones[axis] = new Thread(new ParameterizedThreadStart(FuncDetectHomeDone));
            Thread.Sleep(200);
            detectHomeDones[axis].Start(axis);
            return (int)ErrorDef.Success;
        }
        #endregion

        #region 单轴运动


        /// <summary>
        /// 以预设的运动参数做单轴PTP运动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public int AbsMove(int axis, double position)
        {
            _CheckAxisEnable(axis, "AbsMove");
            lock (ml)
            {
                HTM.MOTION_PARA mp = new HTM.MOTION_PARA()
                {
                    vStart = _motionParams[axis].vs,
                    vMax = _motionParams[axis].vm,
                    vEnd = _motionParams[axis].ve,
                    acc = _motionParams[axis].acc,
                    dec = _motionParams[axis].dec,
                    sFactor = _motionParams[axis].curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.Move(axis, position, 1.0, HTM.MotionMode.AS, ref mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }

        }



        public int RelMove(int axis, double distance)
        {
            _CheckAxisEnable(axis, "RelMove");
            lock (ml)
            {
                HTM.MOTION_PARA mp = new HTM.MOTION_PARA()
                {
                    vStart = _motionParams[axis].vs,
                    vMax = _motionParams[axis].vm,
                    vEnd = _motionParams[axis].ve,
                    acc = _motionParams[axis].acc,
                    dec = _motionParams[axis].dec,
                    sFactor = _motionParams[axis].curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.Move(axis, distance, 1.0, HTM.MotionMode.RS, ref mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }



        public int Jog(int axis,bool isPositive)
        {
            _CheckAxisEnable(axis, "Jog");
            if (0 != HTM.Speed(axis, isPositive?1.0:-1.0))//以速度运动代替
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 以指定速度作速度运动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public int VelMove(int axis, double velocity, bool isPositive)
        {
            _CheckAxisEnable(axis, "VelMove");
            if (velocity <= 0)
                return (int)ErrorDef.ParamError;
            lock (ml)
            {
                HTM.MOTION_PARA mp = new HTM.MOTION_PARA()
                {
                    vStart = _motionParams[axis].vs,
                    vMax = velocity,
                    vEnd = _motionParams[axis].ve,
                    acc = _motionParams[axis].acc,
                    dec = _motionParams[axis].dec,
                    sFactor = _motionParams[axis].curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.Speed(axis, isPositive?1.0:-1.0, ref mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 以指定运动参数做速度运动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public int VelMove_P(int axis, JFMotionParam mp,bool isPositive)
        {
            _CheckAxisEnable(axis, "VelMove_P");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.Speed(axis, isPositive?1.0:-1.0, ref _mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        #endregion

        #region 插补运动
        /// <summary>
        /// 多轴直线插补（绝对方式），使用第一个轴的运动参数
        /// </summary>
        /// <param name="axisList"></param>
        /// <param name="posList"></param>
        /// <returns></returns>
        public int AbsLine(int[] axisList, double[] posList)
        {
            _CheckAxisEnable(axisList, "AbsLine");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("AbsLine(int[] axisList, double[] posList) failed By: posList.Count() != axisList.Count()");
            lock (ml)
            {
                int opt = HTM.Line(axisList, axisList.Count(), posList, 1.0, HTM.MotionMode.AS);
                if (opt != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }



        public int AbsLine_P(int[] axisList, double[] posList, JFMotionParam mp)
        {
            _CheckAxisEnable(axisList, "AbsLine_P");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("AbsLine_P(int[] axisList, double[] posList) failed By: posList.Count() != axisList.Count()");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.Line(axisList, axisList.Count(), posList, 1.0, HTM.MotionMode.AS, ref _mp);
                if (opt != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 已相对方式做直线插补运动
        /// </summary>
        /// <param name="axisList"></param>
        /// <param name="posList"></param>
        /// <returns></returns>
        public int RelLine(int[] axisList, double[] posList)
        {
            _CheckAxisEnable(axisList, "RelLine");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("RelLine(int[] axisList, double[] posList) failed By: posList.Count() != axisList.Count()");
            lock (ml)
            {
                int opt = HTM.Line(axisList, axisList.Count(), posList, 1.0, HTM.MotionMode.RS);
                if (opt != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        public int RelLine_P(int[] axisList, double[] posList, JFMotionParam mp)
        {
            _CheckAxisEnable(axisList, "RelLine_P");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("RelLine_P(int[] axisList, double[] posList) failed By: posList.Count() != axisList.Count()");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.Line(axisList, axisList.Count(), posList, 1.0, HTM.MotionMode.RS, ref _mp);
                if (opt != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        /// <summary>
        /// 圆弧插补运动，以轴1运动参数
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <param name="center1"></param>
        /// <param name="center2"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public int AbsArc2CA(int axis1, int axis2, double center1, double center2, double angle)
        {
            _CheckAxisEnable(axis1, "AbsArc2CA");
            _CheckAxisEnable(axis2, "AbsArc2CA");
            lock (ml)
            {
                int opt = HTM.ArcCa(axis1, axis2, center1, center2, angle, 1.0, HTM.MotionMode.AS);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        public int AbsArc2CA_P(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "AbsArc2CA_P");
            _CheckAxisEnable(axis2, "AbsArc2CA_P");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.ArcCa(axis1, axis2, center1, center2, angle, 1.0, HTM.MotionMode.AS, ref _mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        public int RelArc2CA(int axis1, int axis2, double center1, double center2, double angle)
        {
            _CheckAxisEnable(axis1, "RelArc2CA");
            _CheckAxisEnable(axis2, "RelArc2CA");
            lock (ml)
            {
                int opt = HTM.ArcCa(axis1, axis2, center1, center2, angle, 1.0, HTM.MotionMode.RS);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        public int RelArc2CA_P(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "RelArc2CA_P");
            _CheckAxisEnable(axis2, "RelArc2CA_P");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.ArcCa(axis1, axis2, center1, center2, angle, 1.0, HTM.MotionMode.RS, ref _mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }


        public int AbsArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive)
        {
            _CheckAxisEnable(axis1, "AbsArc2CE");
            _CheckAxisEnable(axis2, "AbsArc2CE");
            lock (ml)
            {
                int opt = HTM.ArcCe(axis1, axis2, center1, center2, pos1, pos2, isPositive ? 1 : 0, 1.0, HTM.MotionMode.AS);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        public int AbsArc2CE_P(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "AbsArc2CE_P");
            _CheckAxisEnable(axis2, "AbsArc2CE_P");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.ArcCe(axis1, axis2, center1, center2, pos1, pos2, isPositive ? 1 : 0, 1.0, HTM.MotionMode.AS, ref _mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        public int RelArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive)
        {
            _CheckAxisEnable(axis1, "RelArc2CE");
            _CheckAxisEnable(axis2, "RelArc2CE");
            lock (ml)
            {
                int opt = HTM.ArcCe(axis1, axis2, center1, center2, pos1, pos2, isPositive ? 1 : 0, 1.0, HTM.MotionMode.RS);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        public int RelArc2CE_P(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "AbsArc2CE_P");
            _CheckAxisEnable(axis2, "AbsArc2CE_P");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.ArcCe(axis1, axis2, center1, center2, pos1, pos2, isPositive ? 1 : 0, 1.0, HTM.MotionMode.RS, ref _mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }

        #endregion

        #region 缓存式运动,
        public int BuffAbsLine(int[] axisList, double[] posList, JFMotionParam mp)
        {
            _CheckAxisEnable(axisList, "BuffAbsLine");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("BuffAbsLine(int[] axisList, double[] posList) failed By: posList.Count() != axisList.Count()");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.Line(axisList, axisList.Count(), posList, 1.0, HTM.MotionMode.BUF | HTM.MotionMode.AS, ref _mp);

                if (opt != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        public int BuffRelLine(int[] axisList, double[] posList, JFMotionParam mp)
        {
            _CheckAxisEnable(axisList, "BuffRelLine");
            if (posList == null || posList.Count() != axisList.Count())
                throw new ArgumentException("BuffRelLine(int[] axisList, double[] posList) failed By: posList.Count() != axisList.Count()");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.Line(axisList, axisList.Count(), posList, 1.0, HTM.MotionMode.BUF | HTM.MotionMode.RS, ref _mp);

                if (opt != 0)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        public int BuffAbsArc2CA(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "BuffAbsArc2CA");
            _CheckAxisEnable(axis2, "BuffAbsArc2CA");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.ArcCa(axis1, axis2, center1, center2, angle, 1.0, HTM.MotionMode.AS | HTM.MotionMode.BUF, ref _mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        public int BuffRelArc2CA(int axis1, int axis2, double center1, double center2, double angle, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "BuffRelArc2CA");
            _CheckAxisEnable(axis2, "BuffRelArc2CA");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.ArcCa(axis1, axis2, center1, center2, angle, 1.0, HTM.MotionMode.RS | HTM.MotionMode.BUF, ref _mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        public int BuffAbsArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "BuffAbsArc2CE");
            _CheckAxisEnable(axis2, "BuffAbsArc2CE");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.ArcCe(axis1, axis2, center1, center2, pos1, pos2, isPositive ? 1 : 0, 1.0, (HTM.MotionMode.AS | HTM.MotionMode.BUF), ref _mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        public int BuffRelArc2CE(int axis1, int axis2, double center1, double center2, double pos1, double pos2, bool isPositive, JFMotionParam mp)
        {
            _CheckAxisEnable(axis1, "BuffAbsArc2CE");
            _CheckAxisEnable(axis2, "BuffAbsArc2CE");
            lock (ml)
            {
                HTM.MOTION_PARA _mp = new HTM.MOTION_PARA()
                {
                    vStart = mp.vs,
                    vMax = mp.vm,
                    vEnd = mp.ve,
                    acc = mp.acc,
                    dec = mp.dec,
                    sFactor = mp.curve,
                    timeout = double.MaxValue
                };
                int opt = HTM.ArcCe(axis1, axis2, center1, center2, pos1, pos2, isPositive ? 1 : 0, 1.0, HTM.MotionMode.RS | HTM.MotionMode.BUF, ref _mp);
                if (0 != opt)
                    return (int)ErrorDef.InvokeFailed;
                return (int)ErrorDef.Success;
            }
        }
        #endregion


        #region 轴位置锁存
        /// <summary>
        /// 获取轴锁存使能状态
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public int GetLtcEnabled(int axis, out bool enabled) 
        {
            enabled = false;
            return (int)ErrorDef.Unsupported;
        }
        public int SetLtcEnabled(int axis, bool enabled)
        {
            return (int)ErrorDef.Unsupported;
        }

        /// <summary>
        /// 获取轴锁存触发电平
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="isHighLevel"></param>
        /// <returns></returns>
        public int GetLtcLogic(int axis, out bool isHighLevel)
        {
            isHighLevel = false;
            return (int)ErrorDef.Unsupported;
        }
        public int SetLtcLogic(int axis, bool isHighLevel)
        {
            return (int)ErrorDef.Unsupported;
        }

        /// <summary>
        /// 清空锁存缓存数据
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public int ClearLtcBuff(int axis)
        {
            return (int)ErrorDef.Unsupported;
        }

        /// <summary>
        /// 获取锁存位置数据
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="buff"></param>
        /// <returns></returns>
        public int GetLtcBuff(int axis, out double[] buff)
        {
            buff = null;
            return (int)ErrorDef.Unsupported;
        }
        #endregion

        //bool isInitialized;
        JFMotionParam[] _motionParams;//各轴的运动参数
        readonly object ml = new object();//Motion Lock (线程锁)
    }

}
