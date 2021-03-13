using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class BondMatchAlgoPara : ViewModelBase, IParameter
    {
        private int index;
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value);
        }

        private string name = "match";
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
        public BondMatchAlgoPara(int index)
        {
            this.index = index;
        }
        // add by lht
        public BondMatchAlgoPara()
        {
            Index = 0;
        }

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

        private double angleStart = 0.0;
        /// <summary>
        /// 图像旋转角度范围  角度
        /// </summary>
        public double AngleStart
        {
            get => angleStart;
            set => OnPropertyChanged(ref angleStart, value);
        }

        private int ballNum_OnRegion = 1;
        /// <summary>
        /// 区域中焊点的数量
        /// </summary>
        public int BallNum_OnRegion
        {
            get => ballNum_OnRegion;
            set => OnPropertyChanged(ref ballNum_OnRegion, value);
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
