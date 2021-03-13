using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
namespace JFVision
{
    public class JFCameraHik:IJFDevice_Camera,IJFRealtimeUIProvider
    {
        internal JFCameraHik()
        {

        }

        ~JFCameraHik()
        {
            Dispose(false);
        }

        #region Initor接口
        /// <summary>获取初始化需要的所有参数的名称 </summary>
        public string[] InitParamNames { get { throw new NotImplementedException(); } }


        /// <summary>
        /// 获取指定名称的初始化参数的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JFParamDescribe GetInitParamDescribe(string name) { throw new NotImplementedException(); }

        /// <summary>
        /// 获取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <returns>参数值</returns>
        public object GetInitParamValue(string name) { throw new NotImplementedException(); }

        /// <summary>
        ///设置取指定名称的初始化参数的当前值
        /// </summary>
        /// <param name="name">参数名称，如果参数名称不在InitParamNames中，将会抛出一个ArgumentException异常</param>
        /// <param name="value">参数值</param>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool SetInitParamValue(string name, object value) { throw new NotImplementedException(); }

        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <returns>操作成功返回True，失败返回false，可通过GetInitErrorInfo()获取错误信息</returns>
        public bool Initialize() { throw new NotImplementedException(); }


        /// <summary>获取初始化状态，如果对象已初始化成功，返回True</summary>
        public bool IsInitOK { get { throw new NotImplementedException(); } }

        /// <summary>获取初始化错误的描述信息</summary>
        public string GetInitErrorInfo() { throw new NotImplementedException(); }
        #endregion//Initor接口



        #region IJFDevice接口
        public string DeviceModel { get { throw new NotImplementedException(); } }
        /// <summary>
        /// 打开设备
        /// </summary>
        public int OpenDevice() { throw new NotImplementedException(); }

        /// <summary>
        /// 关闭设备
        /// </summary>
        public int CloseDevice() { throw new NotImplementedException(); }

        /// <summary>
        /// 设备是否已经打开
        /// </summary>
        public bool IsDeviceOpen { get { throw new NotImplementedException(); } }
        #endregion//IJFDevice接口


        #region ErrorInfo接口
        public string GetErrorInfo(int errorCode) { throw new NotImplementedException(); }
        #endregion//ErrorInfo接口

        #region 相机接口
        /// <summary>相机是否处于触发模式</summary>
        //JFCmrTrigMode TrigMode { get; set; }
        public int GetTrigMode(out JFCmrTrigMode tm) { throw new NotImplementedException(); }
        public int SetTrigMode(JFCmrTrigMode tm) { throw new NotImplementedException(); }


        /// <summary>X方向镜像</summary>
        //bool MirrorX { get; set; }
        public int GetMirrorX(out bool enabled) { throw new NotImplementedException(); }
        public int SetMirrorX(bool enabled) { throw new NotImplementedException(); }
        /// <summary>Y方向镜像</summary>
        //bool MirrorY { get; set; }
        public int GetMirrorY(out bool enabled) { throw new NotImplementedException(); }
        public int SetMirrorY(bool enabled) { throw new NotImplementedException(); }

        /// <summary>设置相机增益参数</summary>
        public int SetGain(double value) { throw new NotImplementedException(); }
        /// <summary>获取相机增益参数</summary>
        public int GetGain(out double value) { throw new NotImplementedException(); }

        /// <summary>设置相机曝光时间 </summary>
        public int SetExposure(double milliSeconds) { throw new NotImplementedException(); }
        /// <summary>获取相机曝光时间</summary>
        public int GetExposure(out double milliSeconds) { throw new NotImplementedException(); }

        /// <summary>内部图片缓存的最大数量</summary>
        public int GetFrameBuffMax(out int maxNum) { throw new NotImplementedException(); }
        public int SetFrameBuffMax(int maxNum) { throw new NotImplementedException(); }

        /// <summary>图像采集回调函数</summary>
        public int RegistAcqFrameCallback(JFCmrAcqFrameDelegate callback) { throw new NotImplementedException(); }
        public void RemoveAcqFrameCallback(JFCmrAcqFrameDelegate callback) { throw new NotImplementedException(); }
        public void ClearAcqFrameCallback() { throw new NotImplementedException(); }

        /// <summary>开始图像采集</summary>
        /// <returns></returns>
        public int StartGather() { throw new NotImplementedException(); }
        /// <summary>停止图像采集</summary>
        public int StopGather() { throw new NotImplementedException(); }


        /// <summary>实时抓拍一张图片</summary>
        public int Snap(out IJFImage img) { throw new NotImplementedException(); }

        /// <summary>当前已缓存的帧数</summary>
        public int CurrFrameCount() { throw new NotImplementedException(); }
        public int ClearFrames() { throw new NotImplementedException(); }

        /// <summary>
        /// 从队列中取出指定数量的图片
        /// </summary>
        /// <param name="images"></param>
        /// <param name="framecount"></param>
        /// <returns></returns>
        public int DeqFrames(out IJFImage[] images, int framecount, int timeoutMilSec) { throw new NotImplementedException(); }

        /// <summary>软触发一次使相机拍照</summary>
        public int SoftwareTrig() { throw new NotImplementedException(); }
        #endregion//相机接口

        /// <summary>实时调试界面接口</summary>
        public JFRealtimeUI GetRealtimeUI() { throw new NotImplementedException(); }


        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            ////////////释放非托管资源
            if (disposing)//////////////释放其他托管资源
            {

            }
        }
    }
}
