using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFInterfaceDef
{
    /// <summary>
    /// 扫码枪工作模式
    /// </summary>
    public enum JFBarcodeSanWorkMode 
    {
        Unknown,
        Initiative, //主动模式
        Passive //被动模式，在回调函数中获取扫描条码
    }

    public delegate void JFBarcodeDelegate(IJFDevice_BarcodeScan scanner, string barcode);



    public interface IJFDevice_BarcodeScan:IJFDevice
    {
        /// <summary>
        /// 获取扫码枪当前工作模式
        /// </summary>
        /// <returns></returns>
        JFBarcodeSanWorkMode GetWorkMode();

        /// <summary>
        /// 设置工作模式
        /// 当设置为主动模式时，应屏蔽Callback
        /// 当设置为被动模式时，主动Scan函数无效
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        int SetWorkMode(JFBarcodeSanWorkMode mode);

        /// <summary>
        /// 被动工作模式下，扫码枪扫描完成后，会通过此回调函数返回条码字串
        /// </summary>
        JFBarcodeDelegate BarcodeCallBack { get; }


        /// <summary>
        /// 主动工作模式下，向扫码枪发送扫码指令，并返回扫描结果
        /// </summary>
        /// <returns></returns>
        int Scan(out string barcode);

        /// <summary>
        /// 清空数据缓冲区
        /// </summary>
        /// <returns></returns>
        int ClearBuff();

    }
}
