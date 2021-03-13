 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HalconDotNet;

namespace LFAOIRecipe
{
    public class Bond2Model : ViewModelBase
    {
        private int intdex;
        public int Index
        {
            get => intdex;
            set => OnPropertyChanged(ref intdex, value);
        }

        //转正图保存路径
        public string RotatedImagePath { get; set; }

        /// <summary>
        /// 旋转角度 保存xml中 为了传输 以及后续重构方便
        /// </summary>
        public double RotatedImageAngel { get; set; }

        //模板ID保存路径
        public string ModelIdPath { get; set; } = string.Empty;

        private UserRegion rotateLineUserRegion;//
        public UserRegion RotateLineUserRegion//
        {
            get => rotateLineUserRegion;
            set => OnPropertyChanged(ref rotateLineUserRegion, value);
        }

        private UserRegion bond2UserRegion;
        public UserRegion Bond2UserRegion
        {
            get => bond2UserRegion;
            set => OnPropertyChanged(ref bond2UserRegion, value);
        }

        private UserRegion bond2UserRegionDiff;
        public UserRegion Bond2UserRegionDiff
        {
            get => bond2UserRegionDiff;
            set => OnPropertyChanged(ref bond2UserRegionDiff, value);
        }

        public ObservableCollection<UserRegion> RefineUserRegions { get; set; } = new ObservableCollection<UserRegion>();

        /*

        /// <summary>
        /// 图像最小旋转 角度
        /// </summary>
        private double angleStart = -180;
        public double AngleStart
        {
            get => angleStart;
            set => OnPropertyChanged(ref angleStart, value);
        }

        /// <summary>
        /// 图像旋转角度范围 大于等于0 角度   //改
        /// </summary>
        private double angleExt = 360;
        public double AngleExt
        {
            get => angleExt;
            set
            {
                if (value < 0)
                {
                    OnPropertyChanged(ref angleExt, 45.3);//原来有
                }
                else
                {
                    OnPropertyChanged(ref angleExt, value);
                }

            }
        }
        */

        private int imageIndex = 0;
        public int ImageIndex
        {
            get => imageIndex;
            set => OnPropertyChanged(ref imageIndex, value);
        }
    }
}
