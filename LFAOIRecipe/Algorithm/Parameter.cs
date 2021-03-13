using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace LFAOIRecipe
{
    public class Parameter
    {
        public HObjectVector hvec__CoarseMatchObj = new HObjectVector(1);
        public HObjectVector hvec__MainIcObjs = new HObjectVector(1);
        public HObjectVector hvec__BondWireObjs = new HObjectVector(1);

        public HTupleVector hvec__CoarseArgs = new HTupleVector(1);
        public HTupleVector hvec__MainIcArgs = new HTupleVector(1);

        public void ParameterConfig()
        {
            //HTuple hv__CoarseModel = null;

            ////Coarse参数
            //hvec__CoarseArgs = ((new HTupleVector(1).Insert(0, new HTupleVector(hv__CoarseModel))).Insert(
            //1, new HTupleVector(new HTuple(((((((new HTuple(P.coarseDilationSize).TupleConcat(new HTuple(P.coarseAngleStart).TupleRad()
            //))).TupleConcat(new HTuple(P.coarseAngleExt).TupleRad()))).TupleConcat(new HTuple(P.coarseMinScore)))).TupleConcat(
            //new HTuple(P.coarseNumMatching))))));


        }
    }
}
