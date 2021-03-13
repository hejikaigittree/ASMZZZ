using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class GoldenModelInspectParameter : ViewModelBase, IParameter
    {      
        private bool isICExist = true;
        /// <summary>
        /// IC有无检测
        /// </summary>
        public bool IsICExist
        {
            get => isICExist;
            set => OnPropertyChanged(ref isICExist, value);
        }

        private bool isICLocate = true;
        /// <summary>
        /// IC定位检测
        /// </summary>
        public bool IsICLocate
        {
            get => isICLocate;
            set => OnPropertyChanged(ref isICLocate, value);
        }

        private bool isICOffSet = true;
        /// <summary>
        /// IC偏移检测
        /// </summary>
        public bool IsICOffSet
        {
            get => isICOffSet;
            set => OnPropertyChanged(ref isICOffSet, value);
        }

        private bool isICSurfaceInspect = true;
        /// <summary>
        /// IC表面检测
        /// </summary>
        public bool IsICSurfaceInspect
        {
            get => isICSurfaceInspect;
            set => OnPropertyChanged(ref isICSurfaceInspect, value);
        }

        private int imageChannelIndex_IcExist = 0;
        /// <summary>
        /// 图像三通道索引值
        /// </summary>
        public int ImageChannelIndex_IcExist
        {
            get => imageChannelIndex_IcExist;
            set => OnPropertyChanged(ref imageChannelIndex_IcExist, value);
        }

        private int threshGray = 180;
        /// <summary>
        /// 阈值分割的灰度阈值，可以为1个参数（灰度上限或下限）或2个参数（灰度上下限）
        /// </summary>
        public int ThreshGray
        {
            get => threshGray;
            set => OnPropertyChanged(ref threshGray, value);
        }

        private string lightOrDark = "dark";
        /// <summary>
        /// IC为亮目标或暗目标
        /// </summary>
        public string LightOrDark
        {
            get => lightOrDark;
            set => OnPropertyChanged(ref lightOrDark, value);
        }

        private double closeSize = 2;
        /// <summary>
        /// 为去除孔洞干扰进行闭运算的尺寸
        /// </summary>
        public double CloseSize
        {
            get => closeSize;
            set => OnPropertyChanged(ref closeSize, value);
        }

        private double closeSizeSurfaceInspect = 2;
        /// <summary>
        /// 为去除孔洞干扰进行闭运算的尺寸
        /// </summary>
        public double CloseSizeSurfaceInspect
        {
            get => closeSizeSurfaceInspect;
            set => OnPropertyChanged(ref closeSizeSurfaceInspect, value);
        }

        private double icSizeTh = 800;
        /// <summary>
        /// Ic目标的面积阈值，可以为1个参数（面积下限）或2个参数（面积上下限）
        /// </summary>
        public double IcSizeTh
        {
            get => icSizeTh;
            set => OnPropertyChanged(ref icSizeTh, value);
        }

        private double dilationSize = 40;
        /// <summary>
        /// 膨胀匹配区域获取搜索区域所需要的膨胀尺寸
        /// </summary>
        public double DilationSize
        {
            get => dilationSize;
            set => OnPropertyChanged(ref dilationSize, value);
        }

        private double rowDiffTh = 150;
        /// <summary>
        /// 行方向偏移阈值
        /// </summary>
        public double RowDiffTh
        {
            get => rowDiffTh;
            set => OnPropertyChanged(ref rowDiffTh, value);
        }

        private double colDiffTh = 150;
        /// <summary>
        /// 列方向偏移阈值
        /// </summary>
        public double ColDiffTh
        {
            get => colDiffTh;
            set => OnPropertyChanged(ref colDiffTh, value);
        }

        private double angleDiffTh = 0.78539815;
        /// <summary>
        /// 角度偏移阈值
        /// </summary>
        public double AngleDiffTh
        {
            get => angleDiffTh;
            set => OnPropertyChanged(ref angleDiffTh, value);
        }

        private double[] sobelScaleFactors = { 0.2, 0.2 };
        /// <summary>
        /// 边缘提取系数
        /// </summary>
        public double[] SobelScaleFactors
        {
            get => sobelScaleFactors;
            set => OnPropertyChanged(ref sobelScaleFactors, value);
        }

        private double[] darkScaleFactors = { 3, 3 };
        /// <summary>
        /// 生成暗图系数
        /// </summary>
        public double[] DarkScaleFactors
        {
            get => darkScaleFactors;
            set => OnPropertyChanged(ref darkScaleFactors, value);
        }

        private double[] lightScaleFactors = { 3, 3 };
        /// <summary>
        /// 生成亮图系数
        /// </summary>
        public double[] LightScaleFactors
        {
            get => lightScaleFactors;
            set => OnPropertyChanged(ref lightScaleFactors, value);
        }

        private double[] grayContrast = { 5 ,5}; 
        /// <summary>
        /// 灰度对比度，即超过阈值多大的范围视为感兴趣的目标
        /// </summary>
        public double[] GrayContrast
        {
            get => grayContrast;
            set => OnPropertyChanged(ref grayContrast, value);
        }

        private double[] minLength = { 3, 3 };
        /// <summary>
        /// 缺陷最小外接矩形半长,为一个向量，针对各重点检测区和非重点检测区分别设置不同参数
        /// </summary>
        public double[] MinLength
        {
            get => minLength;
            set => OnPropertyChanged(ref minLength, value);
        }

        private double[] minWidth = { 2, 2 };
        /// <summary>
        /// 缺陷最小外接矩形半宽,为一个向量，针对各重点检测区和非重点检测区分别设置不同参数
        /// </summary>
        public double[] MinWidth
        {
            get => minWidth;
            set => OnPropertyChanged(ref minWidth, value);
        }

        private double[] minArea = { 6, 6 };
        /// <summary>
        /// 缺陷最小面积,为一个向量，针对各重点检测区和非重点检测区分别设置不同参数
        /// </summary>
        public double[] MinArea
        {
            get => minArea;
            set => OnPropertyChanged(ref minArea, value);
        }

        private string selOperation ="and";
        /// <summary>
        /// 形状选择算子and或or
        /// </summary>
        public string SelOperation
        {
            get => selOperation;
            set => OnPropertyChanged(ref selOperation, value);
        }

        private double minMatchScore = 0.6;
        /// <summary>
        /// 设置最小匹配分数
        /// </summary>
        public double MinMatchScore
        {
            get => minMatchScore;
            set => OnPropertyChanged(ref minMatchScore, value);
        }

        private double angleStart =-0.13; 
        /// <summary>
        /// 设置匹配的开始角(弧度）
        /// </summary>
        public double AngleStart
        {
            get => angleStart;
            set => OnPropertyChanged(ref angleStart, value);
        }

        private double angleExt = 0.26;
        /// <summary>
        /// 设置匹配的角度范围（弧度）
        /// </summary>
        public double AngleExt
        {
            get => angleExt;
            set => OnPropertyChanged(ref angleExt, value);
        }

        private int matchNum = 1;
        /// <summary>
        /// 设置匹配个数
        /// </summary>
        public int MatchNum
        {
            get => matchNum;
            set => OnPropertyChanged(ref matchNum, value);
        }

        private bool isChromatismProcess = false;
        /// <summary>
        /// 色差图优化算子"是" "否"
        /// </summary>
        public bool IsChromatismProcess
        {
            get => isChromatismProcess;
            set => OnPropertyChanged(ref isChromatismProcess, value);
        }

        private bool isGlobalChromatism = false;
        /// <summary>
        /// 色差图类型 "整体色差" "局部色差"
        /// </summary>
        public bool IsGlobalChromatism
        {
            get => isGlobalChromatism;
            set => OnPropertyChanged(ref isGlobalChromatism, value);
        }
    }
}
