using System.Collections.Generic;
using System.Windows.Media;

namespace LFAOIReview
{
    class MappingData : ViewModelBase
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

        #region 显示文字大小 变更通知
        double _FontSize;
        /// <summary>
        /// 显示文字大小
        /// </summary>
        public double FontSize
        {
            get { return _FontSize; }
            set
            {
                if (_FontSize != value)
                {
                    _FontSize = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 显示文字 变更通知
        string _Text;
        /// <summary>
        /// 显示文字
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 边框厚度 变更通知
        double _BorderThickness;
        /// <summary>
        /// 边框厚度
        /// </summary>
        public double BorderThickness
        {
            get { return _BorderThickness; }
            set
            {
                if (_BorderThickness != value)
                {
                    _BorderThickness = value;
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

        /// <summary>
        /// 行序号，从0开始
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 列序号，从0开始
        /// </summary>
        public int ColumnIndex { get; set; }

        #region 正常状态的单元格填充颜色，变化时计算其他状态的颜色，刷新颜色
        private Color _NormalFillColor;
        /// <summary>
        /// 正常状态的单元格填充颜色
        /// </summary>
        public Color NormalFillColor
        {
            get { return _NormalFillColor; }
            set
            {
                if (_NormalFillColor != value)
                {
                    _NormalFillColor = value;
                    ///变化时计算其他状态的颜色，刷新颜色
                    MouseMoveFillColor = ToDarkColor(_NormalFillColor);
                    SelectedFillColor = ToLightColor(_NormalFillColor);
                    ChangeColor();
                }
            }
        }
        #endregion

        /// <summary>
        /// 鼠标移动时单元格填充颜色
        /// </summary>
        public Color MouseMoveFillColor { get; set; }

        /// <summary>
        /// 选中时单元格填充颜色
        /// </summary>
        public Color SelectedFillColor { get; set; }

        #region 正常状态的边框填充颜色，变化时计算其他状态的颜色，刷新颜色
        private Color _NormalBorderColor;
        /// <summary>
        /// 正常状态的边框填充颜色
        /// </summary>
        public Color NormalBorderColor
        {
            get { return _NormalBorderColor; }
            set
            {
                if (_NormalBorderColor != value)
                {
                    _NormalBorderColor = value;
                    ///变化时计算其他状态的颜色，刷新颜色
                    MouseMoveBorderColor = ToDarkColor(_NormalBorderColor);
                    SelectedBorderColor = ToLightColor(_NormalBorderColor);
                    ChangeColor();
                }
            }
        }
        #endregion

        /// <summary>
        /// 鼠标移动时边框填充颜色
        /// </summary>
        public Color MouseMoveBorderColor { get; set; }
        /// <summary>
        /// 选中时边框颜色
        /// </summary>
        public Color SelectedBorderColor { get; set; }

        #region 是否为鼠标移动状态 变更刷新颜色
        private bool _IsMouseMove = false;
        public bool IsMouseMove
        {
            get { return _IsMouseMove; }
            set
            {
                if (_IsMouseMove != value)
                {
                    _IsMouseMove = value;
                    ChangeColor();
                }
            }
        }
        #endregion

        #region 是否选中状态 变更刷新颜色
        private bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    ChangeColor();
                }
            }
        }
        #endregion

        #region 芯片状态 变更刷新颜色
        private InspectionResult _DieState;
        /// <summary>
        /// 芯片装填
        /// </summary>
        public InspectionResult DieState
        {
            get { return _DieState; }
            set
            {
                if (_DieState != value)
                {
                    _DieState = value;
                    NormalFillColor = Dict_DieState_Color[_DieState];
                    ChangeColor();
                }
            }
        }
        #endregion

        public static Dictionary<InspectionResult, Color> Dict_DieState_Color;

        /// <summary>
        /// 根据鼠标状态变更颜色，选中状态优先级最高，其次是鼠标移动状态
        /// </summary>
        private void ChangeColor()
        {
            if (IsSelected)
            {
                FillColor = SelectedFillColor.ToString();
                BorderColor = SelectedBorderColor.ToString();
                return;
            }
            if (IsMouseMove)
            {
                FillColor = MouseMoveFillColor.ToString();
                BorderColor = SelectedBorderColor.ToString();
                return;
            }
            FillColor = NormalFillColor.ToString();
            BorderColor = NormalBorderColor.ToString();
            return;
        }

        /// <summary>
        /// 计算加深的颜色
        /// </summary>
        private Color ToDarkColor(Color color)
        {
            return color * 1.5f;
        }

        /// <summary>
        /// 计算变浅的颜色
        /// </summary>
        private Color ToLightColor(Color color)
        {
            return color * 0.5f;
        }

        public string DefectTypeIndexString { get; set; }
    }


    /// <summary>
    /// 复看修改从OK改成NG的数据
    /// </summary>
    class ReviewEditData
    {
        public string FrameName;
        public int Row;
        public int Col;
        public int ErrCode;
        public int FrameLotIndex;

    }

}
