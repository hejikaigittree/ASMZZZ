using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class Bond1AutoRegionsParameter : ViewModelBase, IParameter
    {
        //private string verifyImagesDirectory = string.Empty;
        ///// <summary>
        ///// 检测图集
        ///// </summary>
        //public string VerifyImagesDirectory
        //{
        //    get => verifyImagesDirectory;
        //    set => OnPropertyChanged(ref verifyImagesDirectory, value);
        //}

        //private int currentVerifySet;
        ///// <summary>
        ///// 当前检测图集
        ///// </summary>
        //public int CurrentVerifySet
        //{
        //    get => currentVerifySet;
        //    set => OnPropertyChanged(ref currentVerifySet, value);
        //}

        //private int imageChannelIndex = 0;
        ///// <summary>
        ///// 图像三通道索引值
        ///// </summary>
        //public int ImageChannelIndex
        //{
        //    get => imageChannelIndex;
        //    set => OnPropertyChanged(ref imageChannelIndex, value);
        //}

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

        private double[] ellipsBondSize = new double[] { 6, 8 };
        /// <summary>
        /// 椭圆焊点半径大小
        /// </summary>
        public double[] EllipsBondSize
        {
            get => ellipsBondSize;
            set => OnPropertyChanged(ref ellipsBondSize, value);
        }

        private double ellipsAngle = 0;
        /// <summary>
        /// 椭圆的角度
        /// </summary>
        public double EllipsAngle
        {
            get => ellipsAngle;
            set => OnPropertyChanged(ref ellipsAngle, value);
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

        private bool isEnableAutoGenMeasureBond;
        public bool IsEnableAutoGenMeasureBond
        {
            get => isEnableAutoGenMeasureBond;
            set => OnPropertyChanged(ref isEnableAutoGenMeasureBond, value);
        }

        private bool isCircleShape = true;
        public bool IsCircleShape
        {
            get => isCircleShape;
            set => OnPropertyChanged(ref isCircleShape, value);
        }

        private int modelType;
        public int ModelType
        {
            get => modelType;
            set => OnPropertyChanged(ref modelType, value);
        }

        //转正图保存路径
        public string RotatedImagePath { get; set; }

        /// <summary>
        /// 旋转角度 保存xml中 为了传输 以及后续重构方便
        /// </summary>
        public double RotatedImageAngel { get; set; }

        //private UserRegion rotateLineUserRegion;//
        //public UserRegion RotateLineUserRegion//
        //{
        //    get => rotateLineUserRegion;
        //    set => OnPropertyChanged(ref rotateLineUserRegion, value);
        //}

        //private UserRegion bond2UserRegion;
        //public UserRegion Bond2UserRegion
        //{
        //    get => bond2UserRegion;
        //    set => OnPropertyChanged(ref bond2UserRegion, value);
        //}

        //private UserRegion bond2UserRegionDiff;
        //public UserRegion Bond2UserRegionDiff
        //{
        //    get => bond2UserRegionDiff;
        //    set => OnPropertyChanged(ref bond2UserRegionDiff, value);
        //}
    }
}
