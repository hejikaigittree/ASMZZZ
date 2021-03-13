using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class BondMeasureModelObject : ViewModelBase
    {
        /// <summary>
        /// 整图
        /// </summary>
        public HObject Image { get; set; }
        public HObject ChannelImage { get; set; }
        public HObject ImageR { get; set; }
        public HObject ImageG { get; set; }
        public HObject ImageB { get; set; }

        /// <summary>
        /// Die图
        /// </summary>
        public HObject DieImage { get; set; }
        public HObject DieChannelImage { get; set; }
        public HObject DieImageR { get; set; }
        public HObject DieImageG { get; set; }
        public HObject DieImageB { get; set; }

        public HTuple MetrologyHandle { get; set; }

        public HTuple ModelID { get; set; }

        //多通道concat一起
        public HObject Images { get; set; }
        public HObject DieImages { get; set; }     

        /// <summary>
        /// 传入原图 传出分离通道后concat一起的图
        /// </summary>
        /// <param name="ho_i_Image"></param>
        /// <param name="ho_o_Image"></param>
        /// <returns></returns>
        public HObject Separat_Image(HObject ho_i_Image, out HObject ho_o_Image)
        {

            HObject ho_ObjectSelected = null, ho_Image1 = null;
            HObject ho_Image2 = null, ho_Image3 = null, ho_ObjectsConcat = null;

            // Local control variables 

            HTuple hv_Number = null, hv_Index = null, hv_Channels = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_o_Image);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            HOperatorSet.GenEmptyObj(out ho_ObjectsConcat);

            HOperatorSet.CountObj(ho_i_Image, out hv_Number);
            ho_o_Image.Dispose();
            HOperatorSet.GenEmptyObj(out ho_o_Image);
            HTuple end_val3 = hv_Number;
            HTuple step_val3 = 1;
            for (hv_Index = 1; hv_Index.Continue(end_val3, step_val3); hv_Index = hv_Index.TupleAdd(step_val3))
            {
                ho_ObjectSelected.Dispose();
                HOperatorSet.SelectObj(ho_i_Image, out ho_ObjectSelected, hv_Index);
                HOperatorSet.CountChannels(ho_ObjectSelected, out hv_Channels);
                if ((int)(new HTuple(hv_Channels.TupleEqual(3))) != 0)
                {
                    ho_Image1.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
                    HOperatorSet.Decompose3(ho_ObjectSelected, out ho_Image1, out ho_Image2,
                        out ho_Image3);
                    ho_ObjectsConcat.Dispose();
                    HOperatorSet.ConcatObj(ho_Image1, ho_Image2, out ho_ObjectsConcat);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_ObjectsConcat, ho_Image3, out ExpTmpOutVar_0);
                        ho_ObjectsConcat.Dispose();
                        ho_ObjectsConcat = ExpTmpOutVar_0;
                    }
                }
                else
                {
                    ho_ObjectsConcat.Dispose();
                    ho_ObjectsConcat = ho_ObjectSelected.CopyObj(1, -1);
                }

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_o_Image, ho_ObjectsConcat, out ExpTmpOutVar_0);
                    ho_o_Image.Dispose();
                    ho_o_Image = ExpTmpOutVar_0;
                }
            }

            ho_ObjectSelected.Dispose();
            ho_Image1.Dispose();
            ho_Image2.Dispose();
            ho_Image3.Dispose();
            ho_ObjectsConcat.Dispose();

            return ho_o_Image;

        }
    }
}
