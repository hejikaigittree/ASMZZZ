using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    class InspectionDataReport : InspectionDataDb
    {
        public new List<string> ConcatImagePath { get; set; } = new List<string>(1);
        public new List<string> ConcatRegionPath { get; set; } = new List<string>(0);
        public new List<string> WirePath { get; set; } = new List<string>(1);
        public new List<DefectDataReport> List_DefectData { get; set; } = new List<DefectDataReport>(0);

        public List<string> List_GeneralImageTempPath { get; set; } = new List<string>(1);

        public string ExcelDefectImageLink { get; set; }

        /// <summary>
        /// 将OK复看成NG "OK2NG"
        /// </summary>
        public string ReviewEditNG { get; set; }

        public List<int> Priority_DetectType { get; set; } = new List<int>();
    }
}
