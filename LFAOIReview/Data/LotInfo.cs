using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    public class LotInfo
    {
        /// <summary>
        /// 产品号
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string LotName { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 批次总盘数
        /// </summary>
        public int TotalFrameCount { get; set; }

        /// <summary>
        /// 每盘行总数
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// 每盘列总数
        /// </summary>
        public int ColumnCount { get; set; }
    }
}
