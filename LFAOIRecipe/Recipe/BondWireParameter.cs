using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LFAOIRecipe
{
    public class BondWireParameter : ViewModelBase, IParameter
    {
        private string verifyImagesDirectory = string.Empty;
        /// <summary>
        /// 检测图集
        /// </summary>
        public string VerifyImagesDirectory
        {
            get => verifyImagesDirectory;
            set => OnPropertyChanged(ref verifyImagesDirectory, value);
        }

        private int currentVerifySet;
        /// <summary>
        /// 当前检测图集
        /// </summary>
        public int CurrentVerifySet
        {
            get => currentVerifySet;
            set => OnPropertyChanged(ref currentVerifySet, value);
        }

        private int imageChannelIndex = 0;
        /// <summary>
        /// 图像三通道索引值
        /// </summary>
        public int ImageChannelIndex
        {
            get => imageChannelIndex;
            set => OnPropertyChanged(ref imageChannelIndex, value);
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

        //拾取功能
        //自动生成焊点检测区域 拾取功能 2021-01-11
        private bool isVerifyRegionPickUp = false;
        public bool IsVerifyRegionPickUp
        {
            get => isVerifyRegionPickUp;
            set => OnPropertyChanged(ref isVerifyRegionPickUp, value);
        }

        //焊盘区域或检测区域方法选择索引

        private int algoParameterIndex = 2;
        public int AlgoParameterIndex
        {
            get => algoParameterIndex;
            set => OnPropertyChanged(ref algoParameterIndex, value);
        }
        
    }
}
