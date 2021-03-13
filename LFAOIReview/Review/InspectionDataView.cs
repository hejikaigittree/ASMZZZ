using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    class InspectionDataView : InspectionDataDb
    {
        public new List<string> ConcatImagePath { get; set; } = new List<string>(1);
        public new List<string> ConcatRegionPath { get; set; } = new List<string>(0);
        public new List<string> WirePath { get; set; } = new List<string>(1);
        public List<int> List_DefectDataListIndex { get; set; } = new List<int>(0);
        public new List<int> List_DefectData { get; set; } = new List<int>(0);
    }
}
