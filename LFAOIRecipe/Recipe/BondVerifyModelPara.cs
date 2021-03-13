using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class BondVerifyModelPara : ViewModelBase, IParameter
    {

        //检测焊点检测图层选择 add by wj 2021-01-06

        private int imageIndex = 0;
        /// <summary>
        /// 公共选择 参数model-检测图层
        /// </summary>
        public int ImageIndex
        {
            get => imageIndex;
            set => OnPropertyChanged(ref imageIndex, value);
        }

        /// --------使用阈值分割方法参数模板
        //
        private string ballType = "circle";
        /// <summary>
        /// 焊点区域类别选择  ‘circle’,‘ellipse’
        /// </summary>
        public string BallType
        {
            get => ballType;
            set => OnPropertyChanged(ref ballType, value);
        }
        /// <summary>
        /// 焊盘区域内分割焊点阈值
        /// </summary>
        private double[] threshGray = { 0, 120 };
        public double[] ThreshGray
        {
            get => threshGray;
            set => OnPropertyChanged(ref threshGray, value);
        }
        /// <summary>
        /// 闭操作核大小
        /// </summary>
        private double closingSize = 2.5;
        public double ClosingSize
        {
            get => closingSize;
            set => OnPropertyChanged(ref closingSize, value);
        }
        //---------------------------------------

       //阈值分割焊点与测量焊点共有参数
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
        ///-------------------------------------------
        ///
        ///---------------测量模板参数
        ///
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
        private double distanceThreshold = 3.5;
        /// <summary>
        /// 测量间隔宽度
        /// </summary>
        public double DistanceThreshold
        {
            get => distanceThreshold;
            set => OnPropertyChanged(ref distanceThreshold, value);
        }
        ///-----------------------------------------------------------------------
        ///
        ///---------匹配模板参数
        ///
        private double minMatchScore = 0.65;
        /// <summary>
        /// 模板匹配最小分数的设置
        /// </summary>
        public double MinMatchScore
        {
            get => minMatchScore;
            set => OnPropertyChanged(ref minMatchScore, value);
        }

        private double angleExt = 0.5236;
        /// <summary>
        /// 图像旋转角度范围  角度
        /// </summary>
        public double AngleExt
        {
            get => angleExt;
            set => OnPropertyChanged(ref angleExt, value);
        }

        private bool isCircleBond = true;
        public bool IsCircleBond
        {
            get => isCircleBond;
            set => OnPropertyChanged(ref isCircleBond, value);
        }

        private double bondSize = 6;
        /// <summary>
        /// 焊点半径大小（圆）
        /// </summary>
        public double BondSize
        {
            get => bondSize;
            set => OnPropertyChanged(ref bondSize, value);
        }

        private double[] ellipsBondSize = { 8, 4 };
        /// <summary>
        /// 焊点半径大小(椭圆）
        /// </summary>
        public double[] EllipsBondSize
        {
            get => ellipsBondSize;
            set => OnPropertyChanged(ref ellipsBondSize, value);
        }

        private bool isBondRegRefine = false;
        /// <summary>
        /// 是否开启焊点匹配优化
        /// </summary>
        public bool IsBondRegRefine
        {
            get => isBondRegRefine;
            set => OnPropertyChanged(ref isBondRegRefine, value);
        }

        private double addBallNum = 3;
        /// <summary>
        /// 焊点检测增加数目
        /// </summary>
        public double AddBallNum
        {
            get => addBallNum;
            set => OnPropertyChanged(ref addBallNum, value);
        }

        private double maxOverlap = 0.9;
        /// <summary>
        /// 焊点匹配结果最大重合度
        /// </summary>
        public double MaxOverlap
        {
            get => maxOverlap;
            set => OnPropertyChanged(ref maxOverlap, value);
        }

        private double minHistScore = 0.5;
        /// <summary>
        /// 焊点匹配结果最小直方图相似性分数
        /// </summary>
        public double MinHistScore
        {
            get => minHistScore;
            set => OnPropertyChanged(ref minHistScore, value);
        }



    }
}
