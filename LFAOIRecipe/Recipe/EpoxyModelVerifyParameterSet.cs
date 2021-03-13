using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class EpoxyModelVerifyParameterSet : ViewModelBase, IParameter
    {
        private double[] threshGray = { 0,180} ;
        /// <summary>
        /// 阈值分割的灰度阈值，可以为1个参数（灰度上限或下限）或2个参数（灰度上下限）
        /// </summary>
        public double[] ThreshGray
        {
            get => threshGray;
            set => OnPropertyChanged(ref threshGray, value);
        }

        private string lightOrDark = "dark";
        /// <summary>
        /// 亮暗因数
        /// </summary>
        public string LightOrDark
        {
            get => lightOrDark;
            set => OnPropertyChanged(ref lightOrDark, value);
        }

        private double openingSize = 7.5;
        /// <summary>
        /// 为去除金线干扰进行开运算的尺寸
        /// </summary>
        public double OpeningSize
        {
            get => openingSize;
            set => OnPropertyChanged(ref openingSize, value);
        }

        private double[] epoxySizeTh = { 2, 9999 };
        /// <summary>
        /// 银胶目标提取的面积阈值，可以为1个参数（面积下限）或2个参数（面积上下限） 
        /// </summary>
        public double[] EpoxySizeTh
        {
            get => epoxySizeTh;
            set => OnPropertyChanged(ref epoxySizeTh, value);
        }

        private double epoxyLenTh = 0.8;
        /// <summary>
        /// 银胶长度阈值：0-1之间，银胶长度与芯片长度的比值：上下左右 
        /// </summary>
        public double EpoxyLenTh
        {
            get => epoxyLenTh;
            set => OnPropertyChanged(ref epoxyLenTh, value);
        }

        private double epoxyHeiTh = 30;
        /// <summary>
        /// 银胶最高度距离芯片边缘的高度 ：上下左右的高度  单位像素 
        /// </summary>
        public double EpoxyHeiTh
        {
            get => epoxyHeiTh;
            set => OnPropertyChanged(ref epoxyHeiTh, value);
        }

        //private double dilationSize = 40;
        ///// <summary>
        ///// 膨胀参数
        ///// </summary>
        //public double DilationSize
        //{
        //    get => dilationSize;
        //    set => OnPropertyChanged(ref dilationSize, value);
        //}
    }
}
