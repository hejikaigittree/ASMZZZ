using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JFInterfaceDef;
using JFToolKits;
using MvCamCtrl.NET; //海康相机SDK
using HalconDotNet;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace JFVision
{
    public class JFImage_Hik : IJFImage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataPtr">指向图像数据</param>
        /// <param name="frameInfo"></param>
        internal JFImage_Hik(byte[] dataBytes, MyCamera.MV_FRAME_OUT_INFO_EX frameInfo, MyCamera cmr)
        {
            _dataBytes = dataBytes;
            _frameInfo = frameInfo;
            _cmr = cmr;
        }
        ~JFImage_Hik()
        {
            Dispose(false);
        }

        

        //IntPtr _dataPtr = IntPtr.Zero;
        byte[] _dataBytes = null;
        MyCamera.MV_FRAME_OUT_INFO_EX _frameInfo;
        MyCamera _cmr = null;


        /// <summary>图像帧序号</summary>
        public int SequenceIndex{ get { return (int)_frameInfo.nFrameNum; } }
        /// <summary>图像宽度（X方向）</summary>
        public int PicWidth { get { return _frameInfo.nWidth; } }
        /// <summary>图像高度（Y方向）</summary>
        public int PicHeight { get { return _frameInfo.nHeight;  } }
        /// <summary>图像数据行的宽度</summary>
        public int StrideWidth { get { return _frameInfo.nHeight; } }

        /// <summary>图像格式</summary>
        public JFImagePixerFormat PixerFormat
        {
            get 
            {
                switch(_frameInfo.enPixelType)
                {
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                        return JFImagePixerFormat.Mono8;
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                        return JFImagePixerFormat.RGB24;
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Planar:
                        return JFImagePixerFormat.RGB24_P;
                    default:
                        return JFImagePixerFormat.Unknown;
                }

            } 
        }

        /// <summary>获取图像字节数据</summary>
        public int GetRowData(out byte[] rowData) 
        {
            rowData = _dataBytes;
            return (int)ErrorDef.Success;

        }

        /// <summary>生成一个对应的Halcon图像对象</summary>
        public int GenHalcon(out object hoImg) 
        {
            hoImg = null;
            HObject hImg = null;

            MyCamera.MvGvspPixelType dstPixType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;

            if (IsMono)
            {
                if(_frameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                {
                    try
                    {
                        HOperatorSet.GenImage1(out hImg, "byte", _frameInfo.nWidth, _frameInfo.nHeight, Marshal.UnsafeAddrOfPinnedArrayElement(_dataBytes, 0));
                        hoImg = hImg;
                        return (int)ErrorDef.Success;
                    }
                    catch
                    {
                        return (int)ErrorDef.MemoryExcp;
                    }
                }
            }
            else if (IsColor)
            {
                if(_frameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Planar)
                {
                    try
                    {
                        IntPtr pDataR = Marshal.UnsafeAddrOfPinnedArrayElement(_dataBytes, 0);
                        IntPtr pDataG = pDataR + _frameInfo.nWidth * _frameInfo.nHeight;
                        IntPtr pDataB = pDataG + _frameInfo.nWidth * _frameInfo.nHeight;
                        HOperatorSet.GenImage3(out hImg, "byte", _frameInfo.nWidth, _frameInfo.nHeight,pDataR, pDataG, pDataB);
                        hoImg = hImg;
                        return (int)ErrorDef.Success;
                    }
                    catch
                    {
                        return (int)ErrorDef.MemoryExcp;
                    }

                }

                dstPixType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Planar; //将图像转化为RGB平面格式
            }
            else
                return (int)ErrorDef.PixelFormatError;

            int tmpBuffLen = (int)(_frameInfo.nWidth * _frameInfo.nHeight * ((((uint)dstPixType) >> 16) & 0x00ff) >> 3);
            IntPtr pTmp = Marshal.AllocHGlobal(tmpBuffLen);

            MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();
            stConverPixelParam.nWidth = _frameInfo.nWidth;
            stConverPixelParam.nHeight = _frameInfo.nHeight;
            stConverPixelParam.pSrcData = Marshal.UnsafeAddrOfPinnedArrayElement(_dataBytes, 0);
            stConverPixelParam.nSrcDataLen = (uint)(_frameInfo.nWidth * _frameInfo.nHeight * ((((uint)_frameInfo.enPixelType) >> 16) & 0x00ff) >> 3);
            stConverPixelParam.enSrcPixelType = _frameInfo.enPixelType;
            stConverPixelParam.enDstPixelType = dstPixType;
            stConverPixelParam.pDstBuffer = pTmp;
            stConverPixelParam.nDstBufferSize = (uint)tmpBuffLen;
            int err = _cmr.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
            if (MyCamera.MV_OK != err)
            {
                Marshal.FreeHGlobal(pTmp);
                return (int)ErrorDef.InvokeFailed;
            }
            try
            {
                if (dstPixType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                    HOperatorSet.GenImage1(out hImg, "byte", _frameInfo.nWidth, _frameInfo.nHeight, pTmp);
                else
                {
                    IntPtr pDataR =pTmp;
                    IntPtr pDataG = pDataR + _frameInfo.nWidth * _frameInfo.nHeight;
                    IntPtr pDataB = pDataG + _frameInfo.nWidth * _frameInfo.nHeight;
                    HOperatorSet.GenImage3(out hImg, "byte", _frameInfo.nWidth, _frameInfo.nHeight,
                                            pDataR, pDataG, pDataB);
                }
                Marshal.FreeHGlobal(pTmp);
                hoImg = hImg;
                return (int)ErrorDef.Success;
            }
            catch
            {
                Marshal.FreeHGlobal(pTmp);
                return (int)ErrorDef.MemoryExcp;
            }
        }

        public bool IsMono
        {
            get
            {
                switch (_frameInfo.enPixelType)
                {
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool IsColor
        {
            get
            {
                switch (_frameInfo.enPixelType)
                {
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                    case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                        return true;

                    default:
                        return false;
                }
            }
        }



        /// <summary>生成一个对应的bmp图像对象</summary>
        public int GenBmp(out Bitmap bmp) 
        {
            bmp = null;
            IntPtr pBmpData = IntPtr.Zero;
            //IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(_dataBytes, 0);
            MyCamera.MvGvspPixelType dstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8; //将要生成的BMP对象的图像格式
            int nBmpLen = (int)(_frameInfo.nWidth * _frameInfo.nHeight * ((((uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8) >> 16) & 0x00ff) >> 3);
            if (IsMono)
                pBmpData = Marshal.AllocHGlobal(nBmpLen);
            else if (IsColor)
            {
                dstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed; //RGB24
                nBmpLen = (int)(_frameInfo.nWidth * _frameInfo.nHeight * ((((uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed) >> 16) & 0x00ff) >> 3);
                pBmpData = Marshal.AllocHGlobal(nBmpLen);
            }
            else
                return (int)ErrorDef.PixelFormatError;

            MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();
            stConverPixelParam.nWidth = _frameInfo.nWidth;
            stConverPixelParam.nHeight = _frameInfo.nHeight;
            stConverPixelParam.pSrcData  = Marshal.UnsafeAddrOfPinnedArrayElement(_dataBytes, 0) ;
            stConverPixelParam.nSrcDataLen = (uint)(_frameInfo.nWidth * _frameInfo.nHeight * ((((uint)_frameInfo.enPixelType) >> 16) & 0x00ff) >> 3);
            stConverPixelParam.enSrcPixelType = _frameInfo.enPixelType;
            stConverPixelParam.enDstPixelType = dstPixelType;
            stConverPixelParam.pDstBuffer = pBmpData;
            stConverPixelParam.nDstBufferSize = (uint)nBmpLen;
            int err = _cmr.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
            if (MyCamera.MV_OK != err)
            {
                Marshal.FreeHGlobal(pBmpData);
                return (int)ErrorDef.InvokeFailed;
            }

            if (dstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
            {
                Bitmap bmpTmp = new Bitmap(_frameInfo.nWidth, _frameInfo.nHeight, _frameInfo.nWidth , PixelFormat.Format8bppIndexed, pBmpData);

                ColorPalette cp = bmpTmp.Palette;
                // init palette
                for (int i = 0; i < 256; i++)
                {
                    cp.Entries[i] = Color.FromArgb(i, i, i);
                }
                // set palette back
                bmpTmp.Palette = cp;
                bmp = new Bitmap(bmpTmp);
                bmpTmp.Dispose();
                Marshal.FreeHGlobal(pBmpData);
                return (int)ErrorDef.Success;
            }
            ///将图像转化为RGB24
            try
            {
                bmp = new Bitmap(_frameInfo.nWidth, _frameInfo.nHeight, _frameInfo.nWidth * 3, PixelFormat.Format24bppRgb, pBmpData);
            }
            catch
            {
                Marshal.FreeHGlobal(pBmpData);
                return (int)ErrorDef.MemoryExcp;
            }
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
            else if("Bitmap" == imgType)
            {
                Bitmap bmp;
                int ret = GenBmp(out bmp);
                imgObj = bmp;
                return ret;
            }
            return (int)ErrorDef.ParamError;
        }

        /// <summary>保存图像到文件中 </summary>
        public int Save(string filePath, JFImageSaveFileType fileType = JFImageSaveFileType.Bmp) 
        {
            string dir = Path.GetDirectoryName(filePath);
            if(!Directory.Exists(dir))
            {
                DirectoryInfo di = Directory.CreateDirectory(dir);
                if (di == null || !di.Exists)
                    return (int)ErrorDef.FolderUnExist;
            }




            try
            {
                if (fileType == JFImageSaveFileType.Bmp || fileType == JFImageSaveFileType.Jpg) //调用Hik提供的功能
                {
                    int imgFileBuffLen = _frameInfo.nHeight * _frameInfo.nWidth * 3 + 2048;
                    byte[] imgFileBytes = new byte[imgFileBuffLen];
                    IntPtr pBufImage = Marshal.UnsafeAddrOfPinnedArrayElement(imgFileBytes, 0);
                    MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
                    stSaveParam.enImageType = (MyCamera.MV_SAVE_IAMGE_TYPE)(fileType + 1);
                    stSaveParam.enPixelType = _frameInfo.enPixelType;
                    stSaveParam.pData = Marshal.UnsafeAddrOfPinnedArrayElement(_dataBytes, 0); ;
                    stSaveParam.nDataLen = _frameInfo.nFrameLen;
                    stSaveParam.nHeight = _frameInfo.nHeight;
                    stSaveParam.nWidth = _frameInfo.nWidth;
                    stSaveParam.pImageBuffer = pBufImage;
                    stSaveParam.nBufferSize = (uint)(_frameInfo.nHeight * _frameInfo.nWidth * 3 + 2048);
                    if(fileType == JFImageSaveFileType.Jpg) //if(stSaveParam.enImageType == MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Jpeg)
                        stSaveParam.nJpgQuality = 99;

                    int err = _cmr.MV_CC_SaveImageEx_NET(ref stSaveParam);
                    if (MyCamera.MV_OK != err)
                    {
                        return (int)ErrorDef.InvokeFailed;
                    }


                    FileStream pFile = new FileStream(filePath, FileMode.Create);
                    pFile.Write(imgFileBytes, 0, (int)stSaveParam.nImageLen);
                    pFile.Close();
                }
                else if(fileType == JFImageSaveFileType.Tif)
                {
                    string fn = Path.GetFileNameWithoutExtension(filePath);
                    if (string.IsNullOrEmpty(fn))
                        return (int)ErrorDef.FolderUnExist;
                    object oimg;
                    int ret = GenHalcon(out oimg);
                    if (0 != ret)
                        return ret;
                    string extension = Path.GetExtension(filePath);
                    //if (string.IsNullOrEmpty(extension) ||
                    //    0 != string.Compare(".tiff", extension, true) ||
                    //    0 != string.Compare(".tif", extension, true))
                    //    filePath += ".tiff";
                    //if (!string.IsNullOrEmpty(extension)) // 去掉后缀
                    //    filePath = Path.GetFileNameWithoutExtension(filePath);
                    HOperatorSet.WriteImage(oimg as HObject, "tiff", 0, filePath);
                    (oimg as HObject).Dispose();
                    return (int)ErrorDef.Success;
                }
                else if(fileType == JFImageSaveFileType.Png)
                {
                    string fn = Path.GetFileNameWithoutExtension(filePath);
                    if (string.IsNullOrEmpty(fn))
                        return (int)ErrorDef.FolderUnExist;
                    object oimg;
                    int ret = GenHalcon(out oimg);
                    if (0 != ret)
                        return ret;
                    //string extension = Path.GetExtension(filePath);
                    //if (string.IsNullOrEmpty(extension) ||
                    //    0 != string.Compare(".png", extension, true) )
                    //    filePath += ".png";
                    HOperatorSet.WriteImage(oimg as HObject, "png", 0, filePath);
                    (oimg as HObject).Dispose();
                    return (int)ErrorDef.Success;
                }
                else
                {
                    return (int)ErrorDef.Unsupported;
                    //throw new ArgumentException("不支持的图像存储格式:" + fileType.ToString());
                }

                return (int)ErrorDef.Success;
            }
            catch
            {
                return (int)ErrorDef.FileExc;
            }
        }

        public int DisplayTo(IntPtr pWnd)
        {
            if (IntPtr.Zero == pWnd)
                return (int)ErrorDef.ParamError;
            MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();
            stDisplayInfo.hWnd = pWnd;
            stDisplayInfo.pData = Marshal.UnsafeAddrOfPinnedArrayElement(_dataBytes, 0);
            stDisplayInfo.nDataLen = _frameInfo.nFrameLen;
            stDisplayInfo.nWidth = _frameInfo.nWidth;
            stDisplayInfo.nHeight = _frameInfo.nHeight;
            stDisplayInfo.enPixelType = _frameInfo.enPixelType;
            int err = _cmr.MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);
            if (err != MyCamera.MV_OK)
                return (int)ErrorDef.InvokeFailed;
            return (int)ErrorDef.Success;
        }


        enum ErrorDef
        {
            Success = 0,
            InvokeFailed = -1,
            MemoryExcp=-2,//内存操作异常
            FileExc = -3, //文件操作异常
            Unsupported = -4,//不支持的功能
            PixelFormatError = -5, //暂不支持的像素格式
            ParamError = -6,
            FolderUnExist = -7,
        }

        public string GetErrorInfo(int errorCode) 
        {
            string ret = "Undefined ErrorCode = " + errorCode;
            switch(errorCode)
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
                case (int)ErrorDef.FolderUnExist:
                    ret = "Dirct Path UnExist/Create failed";
                    break;
                default:
                    break;

            }
            return ret;
        }

        void Dispose(bool disposing)
        {
            ////////////释放非托管资源
            //if(_dataBuff != IntPtr.Zero)
            //{
            //    Marshal.FreeHGlobal(_dataBuff);
            //    _dataBuff = IntPtr.Zero;
            //}
            if (disposing)//////////////释放其他托管资源
            {
                _dataBytes = null;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);

        }
    }
}
