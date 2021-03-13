using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 相机触发模式
    /// </summary>
    public enum JFCmrTrigMode
    {
        disable , //禁用触发功能 
        software,//软触发模式
        hardware_line0, //硬触发模式,触发线
        hardware_line1,
        hardware_line2,
        hardware_line3

    }

    public delegate void JFCmrAcqFrameDelegate(IJFDevice_Camera cmr,IJFImage frame);

    /// <summary>相机接口定义</summary>
    public interface IJFDevice_Camera:IJFDevice
    {
        /// <summary>相机是否处于触发模式</summary>
        //JFCmrTrigMode TrigMode { get; set; }
        int GetTrigMode(out JFCmrTrigMode tm);
        int SetTrigMode(JFCmrTrigMode tm);


        /// <summary>X方向镜像</summary>
        //bool ReverseX { get; set; }
        int GetReverseX(out bool enabled);
        int SetReverseX(bool enabled);
        /// <summary>Y方向镜像</summary>
        //bool ReverseY { get; set; }
        int GetReverseY(out bool enabled);
        int SetReverseY(bool enabled);

        /// <summary>设置相机增益参数</summary>
        int SetGain(double value);
        /// <summary>获取相机增益参数</summary>
        int GetGain(out double value);

        /// <summary>设置相机曝光时间 </summary>
        int SetExposure(double microSeconds);
        /// <summary>获取相机曝光时间</summary>
        int GetExposure(out double microSeconds);

        /// <summary>内部图片缓存的最大数量</summary>
        int GetBuffSize(out int maxNum);
        int SetBuffSize(int maxNum);

        /// <summary>图像采集回调函数</summary>
        bool IsRegistAcqFrameCallback { get; } // 是否已经注册了回调函数
        int RegistAcqFrameCallback(JFCmrAcqFrameDelegate callback);
        void RemoveAcqFrameCallback(JFCmrAcqFrameDelegate callback);
        void ClearAcqFrameCallback();


        bool IsGrabbing { get; }
        /// <summary>开始图像采集</summary>
        /// <returns></returns>
        int StartGrab();
        /// <summary>停止图像采集</summary>
        int StopGrab();


        /// <summary>实时抓拍一张图片</summary>
        int GrabOne(out IJFImage img,int timeoutMilSeconds = -1);

        /// <summary>当前已缓存的帧数</summary>
        int CurrBuffCount();
        int ClearBuff();

        /// <summary>
        /// 从队列中取出指定数量的图片
        /// </summary>
        /// <param name="images"></param>
        /// <param name="framecount"></param>
        /// <returns></returns>
        int DeqFrames(out IJFImage[] images, int framecount,int timeoutMilSec);

        /// <summary>软触发一次使相机拍照</summary>
        int SoftwareTrig();
        
    }
}
