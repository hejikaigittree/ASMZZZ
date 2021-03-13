using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class BondMeasureAlgoPara : ViewModelBase, IParameter
    {
        private int index;
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value);
        }

        private string name = "measure";
        public string Name
        {
            get => name;
            set => OnPropertyChanged(ref name, value);
        }
        //检测焊点图层选择 add by wj 2021-01-05
        private int imageIndex = 0;
        public int ImageIndex
        {
            get => imageIndex;
            set => OnPropertyChanged(ref imageIndex, value);
        }
        public BondMeasureAlgoPara(int index)
        {
            this.index = index;
        }
        // add by lht
        public BondMeasureAlgoPara()
        {
            Index = 0;
        }

        private int preJudgeEnable = 0;
        /// <summary>
        /// 是否进行预判断  0-否； 1-是
        /// </summary>
        public int PreJudgeEnable
        {
            get => preJudgeEnable;
            set => OnPropertyChanged(ref preJudgeEnable, value);
        }

        private double[] segThreshGray = { 180, 255 };
        /// <summary>
        /// 焊点内部区域阈值分割阈值
        /// </summary>
        public double[] SegThreshGray
        {
            get => segThreshGray;
            set => OnPropertyChanged(ref segThreshGray, value);
        }

        private double segRegAreaFactor = 0.4;
        /// <summary>
        /// 分割区域面积占比系数
        /// </summary>
        public double SegRegAreaFactor
        {
            get => segRegAreaFactor;
            set => OnPropertyChanged(ref segRegAreaFactor, value);
        }
        private double bondOverSizeFactor = 1.5;
        /// <summary>
        /// 焊点过大比例系数
        /// </summary>
        public double BondOverSizeFactor
        {
            get => bondOverSizeFactor;
            set => OnPropertyChanged(ref bondOverSizeFactor, value);
        }

        private double bondUnderSizeFactor = 0.5;
        /// <summary>
        /// 焊点过小比例系数
        /// </summary>
        public double BondUnderSizeFactor
        {
            get => bondUnderSizeFactor;
            set => OnPropertyChanged(ref bondUnderSizeFactor, value);
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
    }
}
