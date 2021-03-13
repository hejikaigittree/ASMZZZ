using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIReview
{
    class ExampleData : ViewModelBase
    {
        #region 坐标X 变更通知
        double _VariableX;
        /// <summary>
        /// 坐标X
        /// </summary>
        public double VariableX
        {
            get { return _VariableX; }
            set
            {
                if (_VariableX != value)
                {
                    _VariableX = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 坐标Y 变更通知
        double _VariableY;
        /// <summary>
        /// 坐标Y
        /// </summary>
        public double VariableY
        {
            get { return _VariableY; }
            set
            {
                if (_VariableY != value)
                {
                    _VariableY = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 宽度 变更通知
        double _Width;
        /// <summary>
        /// 单元格带边框的宽度
        /// </summary>
        public double Width
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    _Width = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 高度 变更通知
        double _Height;
        /// <summary>
        /// 单元格带边框的高度
        /// </summary>
        public double Height
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    _Height = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 单元格填充颜色 变更通知
        string _FillColor;
        /// <summary>
        /// 单元格填充颜色
        /// </summary>
        public string FillColor
        {
            get { return _FillColor; }
            set
            {
                if (_FillColor != value)
                {
                    _FillColor = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 边框颜色 变更通知
        string _BorderColor;
        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor
        {
            get { return _BorderColor; }
            set
            {
                if (_BorderColor != value)
                {
                    _BorderColor = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 芯片状态 变更通知
        private string _DieState;
        /// <summary>
        /// 芯片装填
        /// </summary>
        public string DieState
        {
            get { return _DieState; }
            set
            {
                if (_DieState != value)
                {
                    _DieState = value;
                    OnPropertyChanged("DieState");
                }
            }
        }
        #endregion

        #region 数量 变更通知
        double _Count;
        /// <summary>
        /// 数量
        /// </summary>
        public double Count
        {
            get { return _Count; }
            set
            {
                if (_Count != value)
                {
                    _Count = value;
                    OnPropertyChanged("Count");
                }
            }
        }
        #endregion
    }
}
