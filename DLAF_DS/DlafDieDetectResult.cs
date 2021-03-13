using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLAF_DS
{
    /// <summary>
    /// 一颗Die的检测结果
    /// </summary>
    public class DlafDieDetectResult:IDisposable
    {
        public DlafDieDetectResult()
        {
            ErrorRegions = null;
            DetectItems = null;
        }

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~DlafDieDetectResult()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器

                if(null != ErrorRegions)
                {
                    for(int i = 0; i < ErrorRegions.Length;i++)
                    {
                        if (ErrorRegions[i] != null)
                            foreach (HObject ho in ErrorRegions[i])
                            {
                                ho.Dispose();
                            }
                       
                    }
                    ErrorRegions = null;
                }

        
                //if(null != BondsContour)
                //{
                //    foreach (HObject ho in BondsContour)
                //        if (ho != null)
                //            ho.Dispose();
                //    BondsContour = null;
                //}

                //if(null != WiresRegion)
                //{
                //    foreach (HObject ho in WiresRegion)
                //        if (null != ho)
                //            ho.Dispose();
                //    WiresRegion = null;
                //}

                if(null != DetectItems)
                {
                    foreach (Dictionary<string, HObject> detectItem in DetectItems)
                        foreach (HObject hoVal in detectItem.Values)
                            if (null != hoVal)
                                hoVal.Dispose();
                    DetectItems = null;
                }


                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }



        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }



        private bool disposedValue;

        /// <summary>
        /// Die的行序号 ， 以在料片中的Die为单位排列
        /// </summary>
        public int Row { get; set; }
        
        /// <summary>
        /// Die的列序号
        /// </summary>
        public int Col { get; set; }

        //Die中所有Fov的名称
        public string[] FovNames { get; set; }

        /// <summary>
        /// Fov下的TaskName（图层）列表 ， 一维长度对应的Fov序号
        /// </summary>
        public string[][] TaskNames { get; set; }




        /// <summary>
        /// 错误码 , 一维长度对应的Fov序号
        /// </summary>
        public int[][] ErrorCodes { get; set; }


        /// <summary>
        /// 每个错误码对应的图层（TaskName） ，一维长度为Fov长度
        /// </summary>
        public string[][] ErrorTaskNames { get; set; }




        /// <summary>
        /// 错误码对应的区域
        /// </summary>
        public HObject[][] ErrorRegions { get; set; }


        public Dictionary<string, HObject>[] DetectItems { get; set; }




    }
}
