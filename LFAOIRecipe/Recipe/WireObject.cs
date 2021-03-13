using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class WireObject
    {
        /// <summary>
        /// 原图
        /// </summary>
        public HObject Image { get; set; }

        /// <summary>
        /// 通道图像
        /// </summary>
        public HObject ChannelImage { get; set; }

        public HObject DieChannelImage { get; set; }

        public UserRegion UserRegionForCutOut { get; set; }//

        /// <summary>
        /// Die区域
        /// </summary>
        public HObject DieImage { get; set; }

        /// <summary>
        /// Die区域R通道
        /// </summary>
        public HObject DieImageR { get; set; }

        /// <summary>
        /// Die区域G通道
        /// </summary>
        public HObject DieImageG { get; set; }

        /// <summary>
        /// Die区域B通道
        /// </summary>
        public HObject DieImageB { get; set; }

        /// <summary>
        /// 图像R通道
        /// </summary>
        public HObject ImageR { get; set; }//

        /// <summary>
        /// 图像G通道
        /// </summary>
        public HObject ImageG { get; set; }//

        /// <summary>
        /// 图像B通道
        /// </summary>
        public HObject ImageB { get; set; }//
    }
}
