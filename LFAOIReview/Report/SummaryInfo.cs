using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    class SummaryInfo
    {
        public string Database { get; set; }

        public string AIOperator { get; set; }

        public string Machine { get; set; }

        public string StartDate { get; set; }

        public string StartTime { get; set; }

        public string EndDate { get; set; }

        public string EndTime { get; set; }

        public string Lot { get; set; }

        public string ProductCode { get; set; }

        public int TotalNumberOfStrips { get; set; }

        public int NumberOfStripsInspected { get; set; }

        public int NumberOfStripsNotInspected { get; set; }

        public double FalseCallDevicePercent { get; set; }

        public int NumberOfDevicesFalseCalled { get; set; }

        public int QuantityOfDevicesPerStrip { get; set; }
        public int NumberOfDevicesInspected { get; set; }

        public double DevicesPerHour { get; set; }

        public int NumberOfStartQuantity { get; set; }

        public int NumberOfDevicesPassed { get; set; }

        public int NumberOfDevicesRejected { get; set; }

        public double YieldByDevice { get; set; }

        public int NumberOfNoDies { get; set; }

        /// <summary>
        /// 2019.9.2 新增 武汉二维码个数
        /// </summary>
        public int CodeNumber { get; set; }

        /// <summary>
        /// 2019.11.26新增 复看不合格数量 OK复看为NG
        /// </summary>
        public int NumberOfReviewNG { get; set; }

        /// <summary>
        /// 误检率算上复看OK到NG
        /// </summary>
        public double DevicePercentOfK2N { get; set; }
    }
}
