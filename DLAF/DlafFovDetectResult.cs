using HalconDotNet;
using LFAOIRecipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLAF
{
    /// <summary>
    /// 一个Fov的检测结果
    /// </summary>
    public class DlafFovDetectResult : IDisposable
    {
        /// <summary>
        /// 用于判断一组(单项)检测结果是否OK
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool IsInspectItemsOK(InspectResultItem[] items)
        {
            if (null == items || 0 == items.Length) //没有检测项
                return true;
            foreach (InspectResultItem item in items)
                if (!item.IsDetectOK())
                    return false;
            return true;
        }
        public DlafFovDetectResult()
        {
            ICRow = -1;
            ICCol = -1;
            FovName = null;
            DetectDiesRows = null;
            DetectDiesCols = null;
            DiesErrorCodes = null;
            DiesErrorTaskNames = null;
            DiesErrorRegions = null;
            //BondsContours = null;
            //WiresRegions = null;
            DetectIterms = null;
        }

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~DlafFovDetectResult()
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
                //if (null != BondsContours)
                //{
                //    BondsContours.Dispose();
                //    BondsContours = null;
                //}

                //if (null != WiresRegions)
                //{
                //    WiresRegions.Dispose();
                //    WiresRegions = null;
                //}

                if (null != DetectIterms)
                {
                    foreach (string key in DetectIterms.Keys)
                        if (DetectIterms[key] != null)
                            DetectIterms[key].Dispose();
                    DetectIterms = null;
                }

                if (null != DiesErrorRegions)
                {
                    foreach (HObject[] ha in DiesErrorRegions)
                        if (null != ha)
                            foreach (HObject ho in ha)
                                ho.Dispose();
                    DiesErrorRegions = null;
                }

                if (null != DetectDiesImages)
                {
                    foreach (HObject ho in DetectDiesImages)
                        ho.Dispose();
                    DetectDiesImages = null;
                }

                if (null != WireRegion)
                {
                    WireRegion.Dispose();
                    WireRegion = null;
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

        public DlafFovDetectResult Clone()
        {
            DlafFovDetectResult ret = new DlafFovDetectResult();
            ret.ICRow = ICRow;
            ret.ICCol = ICCol;
            ret.FovName = FovName;
            ret.TaskNames = TaskNames;
            ret.DetectDiesRows = DetectDiesRows;
            ret.DetectDiesCols = DetectDiesCols;
            ret.DiesErrorCodes = DiesErrorCodes;
            ret.IsDetectSuccess = IsDetectSuccess;
            ret.DetectErrorInfo = DetectErrorInfo;
            ret.CurrColCount = CurrColCount;
            ret.CurrRowCount = CurrRowCount;
            ret.DieInspectResults = DieInspectResults;

            ret.DiesErrorTaskNames = DiesErrorTaskNames;

            //非托管对象
            if (null != DiesErrorRegions)
            {
                ret.DiesErrorRegions = new HObject[DiesErrorRegions.Length][];
                for (int i = 0; i < DiesErrorRegions.Length; i++)
                {
                    if (DiesErrorRegions[i] == null)
                        ret.DiesErrorRegions[i] = null;
                    else
                    {
                        ret.DiesErrorRegions[i] = new HObject[DiesErrorRegions[i].Length];
                        for (int j = 0; j < DiesErrorRegions[i].Length; j++)
                            ret.DiesErrorRegions[i][j] = DiesErrorRegions[i][j].CopyObj(1, -1);//Clone();
                    }
                }
            }
            else
                ret.DiesErrorRegions = DiesErrorRegions;



            if (null != DetectIterms)
            {
                ret.DetectIterms = new Dictionary<string, HObject>();
                foreach (string key in DetectIterms.Keys)
                {
                    HObject val = DetectIterms[key];
                    if (null == val)
                        ret.DetectIterms.Add(key, null);
                    else
                        ret.DetectIterms.Add(key, val.Clone());
                }
            }
            else
                ret.DetectIterms = null;


            if (null != DetectDiesImages)
            {
                ret.DetectDiesImages = new HObject[DetectDiesImages.Length];
                for (int i = 0; i < DetectDiesImages.Length; i++)
                    ret.DetectDiesImages[i] = DetectDiesImages[i].Clone();
            }
            else
                ret.DetectDiesImages = null;

            if (null != WireRegion)
                ret.WireRegion = WireRegion.Clone();
            else
                ret.WireRegion = null;

            ret.DiesErrorDetails = DiesErrorDetails;




            return ret;
        }




        private bool disposedValue;

        public int CurrRowCount { get; set; }
        public int CurrColCount { get; set; }


        /// <summary>
        /// 扫描点位行序号
        /// </summary>
        public int ICRow { get; set; }

        /// <summary>
        /// 扫描点位列
        /// </summary>
        public int ICCol { get; set; }

        /// <summary>
        /// fov名称
        /// </summary>
        public string FovName { get; set; }

        /// <summary>
        /// FOV中所有TaskNames
        /// </summary>
        public string[] TaskNames { get; set; }


        /// <summary>
        /// 在Fov中需要检测的Die的行序号
        /// 每个Fov中可能有多颗Die，也可能只是某颗Die的一部分
        /// 当Fov属于Die的一部分时，DetectDiesRows的长度为1
        /// </summary>
        public int[] DetectDiesRows { get; set; }
        /// <summary>
        /// 在Fov中需要检测的Die的列序号
        /// </summary>
        public int[] DetectDiesCols { get; set; }

        /// <summary>
        /// 需要检测的Die区域的图象 ，数量同dies数量 ， 由各Die的DetectRegion从原图裁剪，每幅图像都是多通道
        /// </summary>
        public HObject[] DetectDiesImages {
            get;
            set; }


        public bool IsDetectSuccess { get; set; }
        public string DetectErrorInfo { get; set; }

        /// <summary>
        /// 各Die中被检测出的错误码
        /// 每个Die可能有多个错误码，当每颗Die的错误码只有一个且为0时表示检测OK
        /// 一维序号表示Die的序号（按检测Die的行列顺序排列）
        /// </summary>
        public int[][] DiesErrorCodes { get; set;}

        /// <summary>
        /// 每颗Die的错误码所对应的图层（TaskName）
        /// </summary>
        public string[][] DiesErrorTaskNames { get; set; }


        /// <summary>
        /// 每颗Die的错误码所对应区域  ， 一维数量同Die ， 二维数量同每颗Die中实际发生的ErrorCode的数量
        /// </summary>
        public HObject[][] DiesErrorRegions { get; set; }

        /// <summary>
        /// 每个错误的详细信息
        /// </summary>
        public string[][] DiesErrorDetails { get; set; }

        /// <summary>
        /// 金线区域（可能为null，检测项中无金线）
        /// </summary>
        public HObject WireRegion { get; set; }

        /// <summary>
        /// 金线区域所在图层
        /// </summary>
        public string WireRegionTaskName { get; set; }

        /// <summary>
        /// 除了金线之外的其他检测项
        /// </summary>
        public Dictionary<string, HObject> DetectIterms { get; set; }

        /// <summary>
        /// 其他检测项的图层信息
        /// </summary>
        public Dictionary<string, string> DetectItemTaskNames { get; set; }

        /// <summary>
        /// 所有Die的检测项
        /// </summary>
        public List<InspectResultItem[]>   DieInspectResults { get; set; }


        /// <summary>
        /// 检测后为FOV内没有错误
        /// </summary>
        public bool IsFovOK
        {
            get
            {
                if (!IsDetectSuccess)
                    return false;

                if (null == DieInspectResults || 0 == DieInspectResults.Count)
                    return true;
                foreach (InspectResultItem[] dir in DieInspectResults)
                    if (!IsInspectItemsOK(dir))
                        return false;
                return true;
            }
        }

        /// <summary>
        /// Fov中的某颗Die是否良品
        /// </summary>
        /// <param name="dieIndex"></param>
        /// <returns></returns>
        public bool IsDieOK(int dieIndex)
        {

            if (dieIndex < 0 || dieIndex >= DieInspectResults.Count)
                throw new ArgumentException();

            return IsInspectItemsOK(DieInspectResults[dieIndex]);

        }





    }
}
