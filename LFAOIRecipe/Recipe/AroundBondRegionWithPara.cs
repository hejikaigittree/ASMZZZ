using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    public class AroundBondRegionWithPara : ViewModelBase, IParameter
    {

        public AroundBondAdativeAlgoPara AroundBondAdativeAlgoPara { get; set; }
        public AroundBondGlobalAlgoPara AroundBondGlobalAlgoPara { get; set; }
        public AroundBondLineGaussAlgoPara AroundBondLineGaussAlgoPara { get; set; }
        public AroundBallMeasureAlgoPara AroundBallMeasureAlgoPara { get; set; }

        //------useRegion区域是否其它项目检测标签

        //------------------------------------------------------
        private bool isBallShiftInspect = true;
        /// <summary>
        /// AroundBond周围--是否焊点偏移检测
        /// </summary>
        public bool IsBallShiftInspect
        {
            get => isBallShiftInspect;
            set => OnPropertyChanged(ref isBallShiftInspect, value);
        }

        private string shiftInspectMethod = "Match_Measure";
        public string ShiftInspectMethod
        {
            get => shiftInspectMethod;
            set => OnPropertyChanged(ref shiftInspectMethod, value);
        }

        private int shiftInspectMethodIndex = 0;
        public int ShiftInspectMethodIndex
        {
            get => shiftInspectMethodIndex;
            set => OnPropertyChanged(ref shiftInspectMethodIndex, value);
        }
        //------------------------------------------------------

        //------------------------------------------------------
        private bool isTailInspect = true;
        /// <summary>
        /// AroundBond周围--是否焊点反弧检测
        /// </summary>
        public bool IsTailInspect
        {
            get => isTailInspect;
            set => OnPropertyChanged(ref isTailInspect, value);
        }

        private string tailInspectMethod = "Line_Gauss";
        public string TailInspectMethod
        {
            get => tailInspectMethod;
            set => OnPropertyChanged(ref tailInspectMethod, value);
        }

        private int tailInspectMethodIndex = 0;
        public int TailInspectMethodIndex
        {
            get => tailInspectMethodIndex;
            set => OnPropertyChanged(ref tailInspectMethodIndex, value);
        }
        //-------------------------------------------------------------------


        //-------------------------------------------------------------------
        private bool isSurfaceInspect = true;
        /// <summary>
        /// AroundBond周围--是否表面异物检测
        /// </summary>
        public bool IsSurfaceInspect
        {
            get => isSurfaceInspect;
            set => OnPropertyChanged(ref isSurfaceInspect, value);
        }

        private string surfaceInspectMethod = "Adaptive";
        public string SurfaceInspectMethod
        {
            get => surfaceInspectMethod;
            set => OnPropertyChanged(ref surfaceInspectMethod, value);
        }

        private int surfaceInspectMethodIndex = 0;
        public int SurfaceInspectMethodIndex
        {
            get => surfaceInspectMethodIndex;
            set => OnPropertyChanged(ref surfaceInspectMethodIndex, value);
        }

        /// <summary>
        /// 各种方法引用关系
        /// </summary>
        public AroundBondRegionWithPara()
        {
            AroundBondAdativeAlgoPara = new AroundBondAdativeAlgoPara();
            AroundBondGlobalAlgoPara = new AroundBondGlobalAlgoPara();
            AroundBondLineGaussAlgoPara = new AroundBondLineGaussAlgoPara();
            AroundBallMeasureAlgoPara = new AroundBallMeasureAlgoPara();
        }
    }
}
