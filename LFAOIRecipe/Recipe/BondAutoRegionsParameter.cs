using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class BondAutoRegionsParameter : ViewModelBase, IParameter
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

        private double bondSize = 6;
        /// <summary>
        /// 焊点半径大小
        /// </summary>
        public double BondSize
        {
            get => bondSize;
            set => OnPropertyChanged(ref bondSize, value);
        }

        private double length1 = 50;
        /// <summary>
        /// 焊点半径大小
        /// </summary>
        public double Length1
        {
            get => length1;
            set => OnPropertyChanged(ref length1, value);
        }

        private double length2 = 15;
        /// <summary>
        /// 焊点半径大小
        /// </summary>
        public double Length2
        {
            get => length2;
            set => OnPropertyChanged(ref length2, value);
        }

        //顺时针排序
        private int sortMethod = 5;
        public int SortMethod
        {
            get => sortMethod;
            set => OnPropertyChanged(ref sortMethod, value);
        }

        private int firstSortNumber = 1;
        public int FirstSortNumber
        {
            get => firstSortNumber;
            set => OnPropertyChanged(ref firstSortNumber, value);
        }
    }
}
