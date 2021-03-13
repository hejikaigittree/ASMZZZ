using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    public enum InspectionResult
    {
        OK = 0,
        NG = 1,
        N2K = 2,
        SKIP = 3,
        K2N = 4
    }

    internal class InspectionResultsConverter
    {
        public static string ToString(InspectionResult inspectionResult)
        {
            switch(inspectionResult)
            {
                case InspectionResult.OK:
                    return "合格";
                case InspectionResult.NG:
                    return "不合格";
                case InspectionResult.N2K:
                    return "复看合格";
                case InspectionResult.SKIP:
                    return "跳过";
                case InspectionResult.K2N:
                    return "复看不合格";
                default:return string.Empty;
            }
        }

        public static string ToColor(InspectionResult inspectionResult)
        {
            switch (inspectionResult)
            {
                case InspectionResult.OK:
                    return "dark green";
                case InspectionResult.NG:
                    return "red";
                case InspectionResult.N2K:
                    return "yellow";
                case InspectionResult.SKIP:
                    return "blue";
                case InspectionResult.K2N:
                    return "orange";
                default: return "red";
            }
        }
    }
}
