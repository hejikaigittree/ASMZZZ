using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace LFAOIRecipe
{
    public class GoldenModelObject
    {
        public HObject Image { get; set; }
        public HObject ChannelImage { get; set; }
        public HObject ImageR { get; set; }
        public HObject ImageG { get; set; }
        public HObject ImageB { get; set; }

 
        public HObject DieImage { get; set; }
        public HObject DieChannelImage { get; set; }
        public HObject DieImageR { get; set; }
        public HObject DieImageG { get; set; }
        public HObject DieImageB { get; set; }

        public HObject MeadImage { get; set; }

        public HObject StdImage { get; set; }

        public HObject LightImage { get; set; }

        public HObject DarkImage { get; set; }

        //public HTuple ModelID { get; set; }

        /// <summary>
        /// 定位模板
        /// </summary>
        public HTuple PosModelID { get; set; }

        public UserRegion UserRegionForCutOut { get; set; }

        public HObject RejectRegion { get; set; }

    }
}
