using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    class DefectDataDb : DefectData
    {
        public int DbIndex { get; set; }
        public int InspectionDataDbIndex { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public int FrameIndex { get; set; }
        public int ConcatImageIndex { get; set; }
        public int RegionIndex { get; set; }
        public int ConcatRegionIndex { get; set; }
        public int Result { get; set; }
        public new string ErrorDetail { get; set; }
    }
}
