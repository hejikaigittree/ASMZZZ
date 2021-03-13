using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class BondMeasureVerifyParameterSet : ViewModelBase, IParameter
    {
        private double dilationSize=40;
        /// <summary>
        /// 匹配区域膨胀参数
        /// </summary>
        public double DilationSize
        {
            get => dilationSize;
            set => OnPropertyChanged(ref dilationSize, value);
        }

        //private double bondOffsetFactor=0.7;
        ///// <summary>
        ///// 焊点偏出参考点比例阈值
        ///// </summary>
        //public double BondOffsetFactor
        //{
        //    get => bondOffsetFactor;
        //    set => OnPropertyChanged(ref bondOffsetFactor, value);
        //}

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
    }
}
