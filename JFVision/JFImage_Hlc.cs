using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using JFInterfaceDef;

namespace JFVision
{
    /// <summary>
    /// 从文件中加载的图像 , 目前使用的是Halcon提供的图片加载功能
    /// </summary>
    public class JFImage_Hlc : IJFImage
    {
        public JFImage_Hlc(HObject img,int seqIndex)
        {
            HTuple width, height;
            HOperatorSet.GetImageSize(img, out width, out height);
            PicWidth = width.I;
            PicHeight = height.I;
            StrideWidth = PicHeight;
            SequenceIndex = seqIndex;
            //HTuple imgType;
            //HOperatorSet.GetImageType(img, out imgType);
            //if ("byte" == imgType)
            //    PixerFormat = JFImagePixerFormat.Mono8;
            //else
            //    PixerFormat = JFImagePixerFormat.RGB24_P;
            HTuple chnCount;
            HOperatorSet.CountChannels(img, out chnCount);
            if (1 == chnCount.I)
                PixerFormat = JFImagePixerFormat.Mono8;
            else if (3 == chnCount.I)
                PixerFormat = JFImagePixerFormat.RGB24_P;
            else
                PixerFormat = JFImagePixerFormat.Unknown;
            hoImage = img;
        }
        ~JFImage_Hlc()
        {
            Dispose(false);
        }

        HObject hoImage = null;
        public int SequenceIndex { get; private set; }

        public int PicWidth { get; private set; }

        public int PicHeight { get; private set; }

        public int StrideWidth { get; private set; }//此处的行数据未考虑4字节对齐 ， 日后添加

        public JFImagePixerFormat PixerFormat { get; private set; }
        

        public int DisplayTo(IntPtr pWndHandle)
        {
            if (null == pWndHandle)
                return (int)ErrorDef.ParamError;
            HOperatorSet.DispImage(hoImage, pWndHandle);
            return (int)ErrorDef.Success;
        }

        [DllImport("Kernel32.dll")]
        internal static extern void CopyMemory(int dest, int source, int size);


        [DllImport("Kernel32.dll")]
        internal static extern void CopyMemory(IntPtr dest, IntPtr source, IntPtr size);


        public int GenBmp(out Bitmap bmp)
        {
            bmp = null;
            HTuple type, width, height, pointer, chnCount;
            HOperatorSet.CountChannels(hoImage, out chnCount);
            if (1 == chnCount.I) //灰度图像
            {
                HOperatorSet.GetImagePointer1(hoImage, out pointer, out type, out width, out height);
                bmp = new Bitmap(PicWidth, PicHeight, PixelFormat.Format8bppIndexed);
                ColorPalette pal = bmp.Palette;
                for (int i = 0; i <= 255; i++)
                {
                    pal.Entries[i] = Color.FromArgb(255, i, i, i);
                }
                bmp.Palette = pal;
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, PicWidth, PicHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                int PixelSize = Bitmap.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
                int stride = bitmapData.Stride;
                IntPtr ptr = bitmapData.Scan0;
                for (int i = 0; i < PicHeight; i++)
                {
                    CopyMemory(ptr, pointer, width * PixelSize);
                    pointer += width;
                    ptr += bitmapData.Stride;
                }

                bmp.UnlockBits(bitmapData);
                return(int)ErrorDef.Success;
            }
            else if(3 == chnCount.I)//RGB图像
            {
                HTuple hred, hgreen, hblue;// type, width, height;

                HOperatorSet.GetImagePointer3(hoImage, out hred, out hgreen, out hblue, out type, out width, out height);

                bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                
                Rectangle rect = new Rectangle(0, 0, width, height);
                BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int strideBytes = ((width * 3 + 3) >> 2) << 2;//Bitmap行宽度是4的倍数
                unsafe
                {
                    byte* pDst = (byte*)bitmapData.Scan0;
                    byte* pR = ((byte*)hred.I);
                    byte* pG = ((byte*)hgreen.I);
                    byte* pB = ((byte*)hblue.I);
                    for (int i = 0; i < height.I; i++)
                        for (int j = 0; j < width.I; j++)
                        {
                            pDst[(i * strideBytes) + j * 3] = pB[0];
                            pDst[(i * strideBytes) + j * 3+1] = pG[0];
                            pDst[(i * strideBytes) + j * 3 + 2] = pR[0];
                        }
                    //int lengh = width * height;
                    //for (int i = 0; i < lengh; i++)
                    //{
                    //    bptr[i * 4] = (b)[i];
                    //    bptr[i * 4 + 1] = (g)[i];
                    //    bptr[i * 4 + 2] = (r)[i];
                    //    bptr[i * 4 + 3] = 255;
                    //}
                }
                bmp.UnlockBits(bitmapData);
                return (int)ErrorDef.Success;
            }

            return (int)ErrorDef.PixelFormatError;

        }

        public int GenHalcon(out object hoImg)
        {
            HObject newImg;
            HOperatorSet.CopyImage(hoImage, out newImg);
            hoImg = newImg;
            return (int)ErrorDef.Success;
        }

        /// <summary>
        /// 生成指定类型的的图像对象
        /// </summary>
        /// <param name="imgObj"></param>
        /// <param name="imgType">"Halcon","Bitmap"或其他可能的类型</param>
        /// <returns></returns>
        public int GenImgObject(out object imgObj, string imgType)
        {
            imgObj = null;
            if ("Halcon" == imgType)
                return GenHalcon(out imgObj);
            else if ("Bitmap" == imgType)
            {
                Bitmap bmp;
                int ret = GenBmp(out bmp);
                imgObj = bmp;
                return ret;
            }
            return (int)ErrorDef.ParamError;
        }

        enum ErrorDef
        {
            Success = 0,
            InvokeFailed = -1,
            MemoryExcp = -2,//内存操作异常
            FileExc = -3, //文件操作异常
            Unsupported = -4,//不支持的功能
            PixelFormatError = -5, //暂不支持的像素格式
            ParamError = -6,
        }

        public string GetErrorInfo(int errorCode)
        {
            string ret = "Undefined ErrorCode = " + errorCode;
            switch (errorCode)
            {
                case (int)ErrorDef.Success:
                    ret = "Success";
                    break;
                case (int)ErrorDef.MemoryExcp:
                    ret = "Memory Exception";
                    break;
                case (int)ErrorDef.InvokeFailed:
                    ret = "Invoke failed";
                    break;
                case (int)ErrorDef.FileExc:
                    ret = "File Exception";
                    break;
                case (int)ErrorDef.Unsupported://= -4,//不支持的功能
                    ret = "Unsupported";
                    break;
                case (int)ErrorDef.PixelFormatError: //= -5, //暂不支持的像素格式
                    ret = "Pixel's format unspupported";
                    break;
                case (int)ErrorDef.ParamError:
                    ret = "Param Error";
                    break;
                default:
                    break;

            }
            return ret;
        }

        public int GetRowData(out byte[] rowData)
        {
            rowData = null;
            if(PixerFormat == JFImagePixerFormat.Mono8)
            {
                rowData = new byte[PicWidth * PicHeight];
                HTuple type, width, height, pointer;
                HOperatorSet.GetImagePointer1(hoImage, out pointer, out type, out width, out height);
                IntPtr pDst = Marshal.UnsafeAddrOfPinnedArrayElement(rowData, 0);
                CopyMemory(pDst.ToInt32(), pointer, PicWidth * PicHeight);
                return (int)ErrorDef.Success;
            }
            else if(PixerFormat == JFImagePixerFormat.RGB24_P)
            {
                rowData = new byte[PicWidth * PicHeight*3]; //此处的行数据未考虑4字节对齐 ， 日后添加
                HTuple hred, hgreen, hblue, type, width, height;

                HOperatorSet.GetImagePointer3(hoImage, out hred, out hgreen, out hblue, out type, out width, out height);
                IntPtr pDst = Marshal.UnsafeAddrOfPinnedArrayElement(rowData, 0);
                CopyMemory(pDst.ToInt32(), hred, PicWidth * PicHeight);
                CopyMemory(pDst.ToInt32() + PicWidth * PicHeight, hgreen, PicWidth * PicHeight);
                CopyMemory(pDst.ToInt32() + PicWidth * PicHeight*2, hblue, PicWidth * PicHeight);
                return (int)ErrorDef.Success;
            }
            return (int)ErrorDef.PixelFormatError;
        }

        public int Save(string filePath, JFImageSaveFileType fileType = JFImageSaveFileType.Bmp)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return (int)ErrorDef.ParamError;
            string extension = "bmp";
            switch(fileType)
            {
                case JFImageSaveFileType.Jpg:
                    extension = "jpeg";
                    break;
                case JFImageSaveFileType.Png:
                    extension = "png";
                    break;
                case JFImageSaveFileType.Tif:
                    extension = "tiff";
                    break;
                default:
                    break;
            }
            try
            {
                HOperatorSet.WriteImage(hoImage, extension, 0, filePath);
            }
            catch
            {
                return (int)ErrorDef.FileExc;
            }
            return (int)ErrorDef.Success;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                if (null != hoImage)
                {
                    hoImage.Dispose();
                    hoImage = null;
                }
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~JFImage_Hlc()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
