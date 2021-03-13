using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class AroundBondGlobalAlgoPara : ViewModelBase, IParameter
    {
        private string name = "global";
        public string Name
        {
            get => name;
            set => OnPropertyChanged(ref name, value);
        }

        public AroundBondGlobalAlgoPara()
        {

        }
        //焊点膨胀为免检区域尺寸
        private double surfDilationSize = 1.5;
        public double SurfDilationSize
        {
            get => surfDilationSize;
            set => OnPropertyChanged(ref surfDilationSize, value);
        }
        //检测焊点周围区域图层选择
        private int surfImageIndex = 0;
        public int SurfImageIndex
        {
            get => surfImageIndex;
            set => OnPropertyChanged(ref surfImageIndex, value);
        }

        // Global 方法  
        //灰度阈值
        private double[] threshGray = { 60, 100 };
        public double[] ThreshGray
        {
            get => threshGray;
            set => OnPropertyChanged(ref threshGray, value);
        }

        //灰度阈值 内-0  外-1
        private int threshGrayInOrOut = 0;
        public int ThreshGrayInOrOut
        {
            get => threshGrayInOrOut;
            set => OnPropertyChanged(ref threshGrayInOrOut, value);
        }

        //开运算尺寸
        private double morphSize = 4.5;
        public double MorphSize
        {
            get => morphSize;
            set => OnPropertyChanged(ref morphSize, value);
        }

        //////////////////////////////////////////////////////////

        private double closeSize = 2;
        /// <summary>
        /// 闭运算尺寸
        /// </summary>
        public double CloseSize
        {
            get => closeSize;
            set => OnPropertyChanged(ref closeSize, value);
        }

        private double minLength = 4;
        /// <summary>
        /// 缺陷最小外接矩形半长,为一个向量，针对各重点检测区和非重点检测区分别设置不同参数
        /// </summary>
        public double MinLength
        {
            get => minLength;
            set => OnPropertyChanged(ref minLength, value);
        }

        private double minWidth = 3;
        /// <summary>
        /// 缺陷最小外接矩形半宽,为一个向量，针对各重点检测区和非重点检测区分别设置不同参数
        /// </summary>
        public double MinWidth
        {
            get => minWidth;
            set => OnPropertyChanged(ref minWidth, value);
        }

        private double minArea = 10;
        /// <summary>
        /// 缺陷最小面积,为一个向量，针对各重点检测区和非重点检测区分别设置不同参数
        /// </summary>
        public double MinArea
        {
            get => minArea;
            set => OnPropertyChanged(ref minArea, value);
        }

        private string selOperation = "and";
        /// <summary>
        /// 形状选择算子and或or等
        /// </summary>
        public string SelOperation
        {
            get => selOperation;
            set => OnPropertyChanged(ref selOperation, value);
        }


    }
}
