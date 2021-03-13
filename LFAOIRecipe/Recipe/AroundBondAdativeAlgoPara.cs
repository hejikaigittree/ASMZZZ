using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class AroundBondAdativeAlgoPara : ViewModelBase, IParameter
    {


        private string name = "adative";
        public string Name
        {
            get => name;
            set => OnPropertyChanged(ref name, value);
        }

        public AroundBondAdativeAlgoPara()
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

        //自适应阈值分割方法
        private string adaptiveMethod = "median";
        /// <summary>
        /// 局部阈值分割方法，取值为'mean'  'median'   'gauss'
        /// </summary>
        public string AdaptiveMethod
        {
            get => adaptiveMethod;
            set => OnPropertyChanged(ref adaptiveMethod, value);
        }
        //
        private double blockSize = 7;
        /// <summary>
        /// 邻域大小，即局部阈值分割的尺寸
        /// </summary>
        public double BlockSize
        {
            get => blockSize;
            set => OnPropertyChanged(ref blockSize, value);
        }
        private double contrast = 15;
        /// <summary>
        /// 灰度对比度
        /// </summary>
        public double Contrast
        {
            get => contrast;
            set => OnPropertyChanged(ref contrast, value);
        }

        private string lightOrDark = "dark";
        /// <summary>
        /// 亮暗因数  light dark all
        /// </summary>
        public string LightOrDark
        {
            get => lightOrDark;
            set => OnPropertyChanged(ref lightOrDark, value);
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
