using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    class BridgeModelInspectParameter : ViewModelBase, IParameter
    {
        private bool isBridgeSurfaceInspect = true;
        /// <summary>
        /// 是否桥接表面检测
        /// </summary>
        public bool IsBridgeSurfaceInspect
        {
            get => isBridgeSurfaceInspect;
            set => OnPropertyChanged(ref isBridgeSurfaceInspect, value);
        }
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

        private int imageBridgeVerifyChannelIndex = 0;
        public int ImageBridgeVerifyChannelIndex
        {
            get => imageBridgeVerifyChannelIndex;
            set => OnPropertyChanged(ref imageBridgeVerifyChannelIndex, value);
        }

        private string regSegMethod = "Adaptive";
        public string RegSegMethod
        {
            get => regSegMethod;
            set => OnPropertyChanged(ref regSegMethod, value);
        }

        private int regSegMethodIndex = 0;
        public int RegSegMethodIndex
        {
            get => regSegMethodIndex;
            set => OnPropertyChanged(ref regSegMethodIndex, value);
        }

        // Global 方法  
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

        // Adaptive 方法
        private string adaptiveMethod = "median";
        /// <summary>
        /// 局部阈值分割方法，取值为'mean'  'median'   'gauss'
        /// </summary>
        public string AdaptiveMethod
        {
            get => adaptiveMethod;
            set => OnPropertyChanged(ref adaptiveMethod, value);
        }

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
