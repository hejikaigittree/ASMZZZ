using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public enum RegionOperatType
    {
        Null,
        Union,
        Difference,
        Connection,
        Erosion,
        Dilation,
        Fillup,
        OpeningCircle,
        ClosingCircle,
        Threshold,
        SelectShape,
        RegionTrans,
        RegionArray,
    }
}
