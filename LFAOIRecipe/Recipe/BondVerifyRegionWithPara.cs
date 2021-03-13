using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class BondVerifyRegionWithPara : ViewModelBase, IParameter
    {

        public BondThresAlgoPara BondThresAlgoPara { get; set; }
        public BondMeasureAlgoPara BondMeasureAlgoPara { get; set; }
        public BondMatchAlgoPara BondMatchAlgoPara { get; set; }


        private string inspectMethod = "Threshold";
        public string InspectMethod
        {
            get => inspectMethod;
            set => OnPropertyChanged(ref inspectMethod, value);
        }

        private int inspectMethodIndex = 0;
        public int InspectMethodIndex
        {
            get => inspectMethodIndex;
            set => OnPropertyChanged(ref inspectMethodIndex, value);
        }
        /// <summary>
        /// 各种方法引用关系
        /// </summary>
        public BondVerifyRegionWithPara()
        {
            BondThresAlgoPara = new BondThresAlgoPara();
            BondMeasureAlgoPara = new BondMeasureAlgoPara();
            BondMatchAlgoPara = new BondMatchAlgoPara();
        }



    }
}
