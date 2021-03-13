using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    class InspectionDataDb : InspectionData
    {
        public int DbIndex { get; set; }
        public int FrameIndex { get; set; }
        public string ConcatImagePath { get; set; }
        public string ConcatRegionPath { get; set; }
        public string WirePath { get; set; }
        public string ReportImagePath { get; set; }
    }
}
