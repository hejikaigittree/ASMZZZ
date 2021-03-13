using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class AroundBondLineGaussAlgoPara : ViewModelBase, IParameter
    {
        private string name = "line_gauss";
        public string Name
        {
            get => name;
            set => OnPropertyChanged(ref name, value);
        }

        public AroundBondLineGaussAlgoPara()
        {

        }
        //焊点周围区域膨胀尺寸
        private double tailDilationSize = 10;
        public double TailDilationSize
        {
            get => tailDilationSize;
            set => OnPropertyChanged(ref tailDilationSize, value);
        }
        //检测焊点周围区域图层选择
        private int tailImageIndex = 0;
        public int TailImageIndex
        {
            get => tailImageIndex;
            set => OnPropertyChanged(ref tailImageIndex, value);
        }

        //Line_Gauss方法
        private double wireWidth = 10.0;
        public double WireWidth
        {
            get => wireWidth;
            set => OnPropertyChanged(ref wireWidth, value);
        }

        private double wireContrast = 15.0;
        public double WireContrast
        {
            get => wireContrast;
            set => OnPropertyChanged(ref wireContrast, value);
        }
        /// <summary>
        /// 筛选目标时的特征量
        /// </summary>
        private string[] selMetric = { "contour_length", "direction" };
        public string[] SelMetric
        {
            get => selMetric;
            set => OnPropertyChanged(ref selMetric, value);
        }

        private string lightOrDark = "dark";
        /// <summary>
        /// 亮暗因数 dark light
        /// </summary>
        public string LightOrDark
        {
            get => lightOrDark;
            set => OnPropertyChanged(ref lightOrDark, value);
        }
        /// <summary>
        /// [3,-rad(25)]  3, -25/180*Math.PI
        /// </summary>
        private double[] selMin = { 1, -0.436 };
        public double[] SelMin
        {
            get => selMin;
            set => OnPropertyChanged(ref selMin, value);
        }

        //25 / 180 * Math.PI
        private double[] selMax = { 999, 0.436 };
        public double[] SelMax
        {
            get => selMax;
            set => OnPropertyChanged(ref selMax, value);
        }
        /// <summary>
        /// 
        /// </summary>
        private double maxWireGap = 3;
        public double MaxWireGap
        {
            get => maxWireGap;
            set => OnPropertyChanged(ref maxWireGap, value);
        }

        //尾丝区域灰度阈值
        private double[] tailGrayThresh = { 0, 80 };
        public double[] TailGrayThresh
        {
            get => tailGrayThresh;
            set => OnPropertyChanged(ref tailGrayThresh, value);
        }

        //尾丝长度阈值
        private double tailLenTh = 10;
        public double TailLenTh
        {
            get => tailLenTh;
            set => OnPropertyChanged(ref tailLenTh, value);
        }

        //是否开启断线判断 "是" "否"
        private bool isWireJudgeAgain = false;
        public bool IsWireJudgeAgain
        {
            get => isWireJudgeAgain;
            set => OnPropertyChanged(ref isWireJudgeAgain, value);
        }

    }
}
