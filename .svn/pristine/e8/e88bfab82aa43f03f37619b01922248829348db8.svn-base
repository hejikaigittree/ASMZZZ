using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace JFInterfaceDef
{
    /// <summary>图像的像素格式</summary>
    public enum JFImagePixerFormat
    {
        Unknown = -1, //未知类型
        Mono8 = 0,
        RGB24,
        RGB24_P, //RGB24 平面
    }

    /// <summary></summary>
    public enum JFImageSaveFileType
    {
        Bmp = 0,
        Jpg = 1,
        Png = 2,
        Tif = 3
    }

    /// <summary>图像接口定义</summary>
    public interface IJFImage:IDisposable,IJFErrorCode2Txt
    {
        /// <summary>图像帧序号</summary>
        int SequenceIndex { get; }
        /// <summary>图像宽度（X方向）</summary>
        int PicWidth { get; }
        /// <summary>图像高度（Y方向）</summary>
        int PicHeight { get; }
        /// <summary>图像数据行的宽度</summary>
        int StrideWidth { get; }

        /// <summary>图像格式</summary>
        JFImagePixerFormat PixerFormat { get; }

        /// <summary>获取图像字节数据</summary>
        int GetRowData(out byte[] rowData);

        /// <summary>生成一个对应的Halcon图像对象</summary>
        int GenHalcon(out object hoImg);

        /// <summary>生成一个对应的bmp图像对象</summary>
        int GenBmp(out Bitmap bmp);

        /// <summary>
        /// 生成指定类型的的图像对象
        /// </summary>
        /// <param name="imgObj"></param>
        /// <param name="imgType">"Halcon","Bitmap"或其他可能的类型</param>
        /// <returns></returns>
        int GenImgObject(out object imgObj, string imgType);

        /// <summary>将图片显示到控件上</summary>
        int DisplayTo(IntPtr pWndHandle);

        /// <summary>保存图像到文件中 </summary>
        int Save(string filePath, JFImageSaveFileType fileType = JFImageSaveFileType.Bmp);
    }
}
