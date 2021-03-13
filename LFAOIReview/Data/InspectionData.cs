using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace LFAOIReview
{
    /// <summary>
    /// Die的所有检测结果
    /// </summary>
    public class InspectionData
    {
        /// <summary>
        /// 行序号 Die在整个料片中的行号
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 列序号 Die在整个料片
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 检测结果 OK/NG/...表示这个Die是否OK
        /// </summary>
        public InspectionResult InspectionResult { get; set; }

        /// <summary>
        /// 检测图像，合并 将Fov中的所有Task排列成一维数组（Concact），每幅图片表示一个DetectRegion
        /// </summary>
        public HObject Image { get; set; }

        /// <summary>
        /// 检测区域，合并，顺序与缺陷列表顺序对应  （存储所有错误对应的Region）
        /// </summary>
        public HObject Region { get; set; }

        /// <summary>
        /// 金线
        /// </summary>
        public HObject Wire { get; set; }


        /// <summary>
        /// 缺陷列表，顺序与Region顺序对应
        /// </summary>
        public List<DefectData> List_DefectData { get; set; } = new List<DefectData>(0);


        /// <summary>
        /// 2019.8.9 新增二维码 如果没有二维码则为 null
        /// </summary>
        public string Code2D { get; set; } = "null";



        #region 新添加的数据项 By 袁楚平 2020-12-03
        public string RecipeID { get; set; }


        public string LotID { get; set; }


        /// <summary>
        /// Die所属料片ID
        /// </summary>
        public string FrameID { get; set; }

        /// <summary>
        /// 所有Fov名称 （大芯片的每颗Die可能包含多个Fov ， 小芯片只有一个Fov）
        /// </summary>
        public string[] FovNames { get; set; }

        /// <summary>
        /// 每个Fov包含的TaskNames（图层名称）
        /// </summary>
        public string[][] TaskNamesInFovs { get; set; }


        public HObject SuccessRegion { get; set; }

        /// <summary>
        /// 所有检测正确区域的类型 ，数量和序号同 Wire
        /// </summary>
        public string[] SuccessRegionTypes { get; set; }

        /// <summary>
        /// 所有正确区域对应的图象序号
        /// </summary>
        public int[] SuccessRegionImageIndex { get; set; }

        #endregion
    }


    /// <summary>
    /// InspectionData类的补充
    /// </summary>
    public class InspectionDataEx
    {
        public int LotIndex { get; set; }

        public int FrameIndex { get; set; }
    }
}
