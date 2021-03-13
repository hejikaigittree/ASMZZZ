using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class BondThresAlgoPara : ViewModelBase, IParameter
    {
        private int index;
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value);
        }

        private string name = "threshold";
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
        public BondThresAlgoPara(int index)
        {
            this.index = index;
        }
        // add by lht
        public BondThresAlgoPara()
        {
            Index = 0;
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




    }
}
