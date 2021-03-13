using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class AroundBallMeasureAlgoPara : ViewModelBase, IParameter
    {
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名
        private string name = "match_measure";
        public string Name
        {
            get => name;
            set => OnPropertyChanged(ref name, value);
        }

        public AroundBallMeasureAlgoPara()
        {

        }
        //检测焊点周围区域图层选择
        private int shiftImageIndex = 0;
        public int ShiftImageIndex
        {
            get => shiftImageIndex;
            set => OnPropertyChanged(ref shiftImageIndex, value);
        }

        /// <summary>
        /// //////////////////////焊盘测量参数设置////////////////////////////////////////////
        /// </summary>

        private int isMeasurePad = 0;
        /// <summary>
        /// 是否进行焊盘测量检测  0-否； 1-是
        /// </summary>
        public int IsMeasurePad
        {
            get => isMeasurePad;
            set => OnPropertyChanged(ref isMeasurePad, value);
        }
        /// <summary>
        /// 焊盘测量模板类型:0-rectangle2，1-circle,2-ellipse
        /// </summary>
        private string padMeasureType = "rectangle2";
        public string PadMeasureType
        {
            get => padMeasureType;
            set => OnPropertyChanged(ref padMeasureType, value);
        }

        /// <summary>
        /// 测量类别的长和宽选择项目索引
        /// </summary>
        private int padItemSelectIndex = 0;
        public int PadItemSelectIndex
        {
            get => padItemSelectIndex;
            set => OnPropertyChanged(ref padItemSelectIndex, value);
        }
        /// <summary>
        /// 判断焊盘是否为测量圆类型
        /// </summary>
        private bool padIsCircle = false;
        public bool PadIsCircle
        {
            get => padIsCircle;
            set => OnPropertyChanged(ref padIsCircle, value);
        }
        /// <summary>
        /// 测量非圆的长与宽设置：如矩形的半长、半宽；椭圆的半长、半宽
        /// </summary>
        private double[] padMeasureLenAndWidth = { 10, 5 };
        public double[] PadMeasureLenAndWidth
        {
            get => padMeasureLenAndWidth;
            set => OnPropertyChanged(ref padMeasureLenAndWidth, value);
        }

        private double padMeasureRadius = 10;
        public double PadMeasureRadius
        {
            get => padMeasureRadius;
            set => OnPropertyChanged(ref padMeasureRadius, value);
        }

        //焊盘测量灰度阈值
        private double padMeasureGrayThr = 10;
        public double PadMeasureGrayThr
        {
            get => padMeasureGrayThr;
            set => OnPropertyChanged(ref padMeasureGrayThr, value);
        }
        //焊盘测量灰度变换
        private string padMeasureTrans = "negative";
        public string PadMeasureTrans
        {
            get => padMeasureTrans;
            set => OnPropertyChanged(ref padMeasureTrans, value);
        }

        //阈值分割焊盘灰度阈值
        private double[] padGrayThresh = { 100, 255 };
        public double[] PadGrayThresh
        {
            get => padGrayThresh;
            set => OnPropertyChanged(ref padGrayThresh, value);
        }
        /// <summary>
        /// //////////////////////焊点测量参数设置////////////////////////////////////////////
        /// </summary>

        private int isMeasureBall = 0;
        /// <summary>
        /// 是否进行焊点测量检测  0-否； 1-是
        /// </summary>
        public int IsMeasureBall
        {
            get => isMeasureBall;
            set => OnPropertyChanged(ref isMeasureBall, value);
        }

        //阈值分割焊盘内焊点灰度阈值
        private double[] ballGrayThresh = { 50, 150 };
        public double[] BallGrayThresh
        {
            get => ballGrayThresh;
            set => OnPropertyChanged(ref ballGrayThresh, value);
        }

        /// <summary>
        /// 焊盘测量模板类型:1-circle,2-ellipse
        /// </summary>
        private string ballMeasureType = "circle";
        public string BallMeasureType
        {
            get => ballMeasureType;
            set => OnPropertyChanged(ref ballMeasureType, value);
        }
        /// <summary>
        /// 测量类别的长和宽选择项目索引
        /// </summary>
        private int ballItemSelectIndex = 0;
        public int BallItemSelectIndex
        {
            get => ballItemSelectIndex;
            set => OnPropertyChanged(ref ballItemSelectIndex, value);
        }
        /// <summary>
        /// 判断焊盘是否为测量圆类型
        /// </summary>
        private bool ballIsCircle = true;
        public bool BallIsCircle
        {
            get => ballIsCircle;
            set => OnPropertyChanged(ref ballIsCircle, value);
        }
        /// <summary>
        /// 测量非圆的长与宽设置：，椭圆的半长、半宽
        /// </summary>
        private double[] ellipsBondSize = { 8, 4 };
        /// <summary>
        /// 焊点半径大小(椭圆）
        /// </summary>
        public double[] EllipsBondSize
        {
            get => ellipsBondSize;
            set => OnPropertyChanged(ref ellipsBondSize, value);
        }

        //焊盘内焊点大小比
        private double circleBallSize = 15;
        public double CircleBallSize
        {
            get => circleBallSize;
            set => OnPropertyChanged(ref circleBallSize, value);
        }
        //焊点测量灰度阈值
        private double ballMeasureGrayThr = 10;
        public double BallMeasureGrayThr
        {
            get => ballMeasureGrayThr;
            set => OnPropertyChanged(ref ballMeasureGrayThr, value);
        }
        //焊点测量灰度变换
        private string ballMeasureTrans = "positive";
        public string BallMeasureTrans
        {
            get => ballMeasureTrans;
            set => OnPropertyChanged(ref ballMeasureTrans, value);
        }

        /// <summary>
        /// ///////////////////////焊点偏出焊盘比例阈值///////////////////////////////////////////
        /// </summary>

        private double ballShiftRatioThr = 0.5;
        public double BallShiftRatioThr
        {
            get => ballShiftRatioThr;
            set => OnPropertyChanged(ref ballShiftRatioThr, value);
        }



    }
}
