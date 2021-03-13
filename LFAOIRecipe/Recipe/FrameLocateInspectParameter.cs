using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    class FrameLocateInspectParameter : ViewModelBase, IParameter
    {
        private double dilationSize = 120;
        /// <summary>
        /// 设置区域膨胀系数
        /// </summary>
        public double DilationSize
        {
            get => dilationSize;
            set => OnPropertyChanged(ref dilationSize, value);
        }

        private double minMatchScore = 0.8;
        /// <summary>
        /// 设置最小匹配分数
        /// </summary>
        public double MinMatchScore
        {
            get => minMatchScore;
            set => OnPropertyChanged(ref minMatchScore, value);
        }

        private double angleStart = -0.39;
        /// <summary>
        /// 设置匹配的开始角
        /// </summary>
        public double AngleStart
        {
            get => angleStart;
            set => OnPropertyChanged(ref angleStart, value);
        }

        private double angleExt = 0.78;
        /// <summary>
        /// 设置匹配的结束角
        /// </summary>
        public double AngleExt
        {
            get => angleExt;
            set => OnPropertyChanged(ref angleExt, value);
        }

        private int matchNum =1;
        /// <summary>
        /// 设置匹配的结束角
        /// </summary>
        public int MatchNum
        {
            get => matchNum;
            set => OnPropertyChanged(ref matchNum, value);
        }
    }
}
