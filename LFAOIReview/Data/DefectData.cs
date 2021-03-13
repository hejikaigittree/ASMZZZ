using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace LFAOIReview
{
    public class DefectData
    {
        /// <summary>
        /// 缺陷类型编号
        /// </summary>
        public int DefectTypeIndex { get; set; }
        
        /// <summary>
        /// 缺陷所属图像
        /// -1 原图为黑白图
        /// 0 R通道
        /// 1 G通道
        /// 2 B通道
        /// </summary>
        public int ImageIndex { get; set; }

        /// <summary>
        /// 缺陷信息，单个字符串
        /// </summary>
        public HTuple ErrorDetail { get; set; }
    }
}
