using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{

    public class BondMeasureModelParameter : ViewModelBase, IParameter
    {
        private double measureContrast=5;
        /// <summary>
        /// 测量对比度
        /// </summary>
        public double MeasureContrast
        {
            get => measureContrast;
            set => OnPropertyChanged(ref measureContrast, value);
        }

        private double mearsureLength=2;
        /// <summary>
        /// 测量半长
        /// </summary>
        public double MearsureLength
        {
            get => mearsureLength;
            set => OnPropertyChanged(ref mearsureLength, value);
        }

        private double mearsureWideth=1;
        /// <summary>
        /// 测量间隔宽度
        /// </summary>
        public double MearsureWideth
        {
            get => mearsureWideth;
            set => OnPropertyChanged(ref mearsureWideth, value);
        }

        private string mearsureTransition = "positive";
        /// <summary>
        /// 测量变换  ‘positive’,‘negative’
        /// </summary>
        public string MearsureTransition
        {
            get => mearsureTransition;
            set => OnPropertyChanged(ref mearsureTransition, value);
        }

        private double distanceThreshold = 3.5;
        /// <summary>
        /// 测量间隔宽度
        /// </summary>
        public double DistanceThreshold
        {
            get => distanceThreshold;
            set => OnPropertyChanged(ref distanceThreshold, value);
        }

        private string mearsureSelect = "all";
        /// <summary>
        /// 边缘点选择  ‘all’,‘first’,‘last’
        /// </summary>
        public string MearsureSelect
        {
            get => mearsureSelect;
            set => OnPropertyChanged(ref mearsureSelect, value);
        }
    } 
}
