using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LFAOIReview
{
    /// <summary>
    /// Mapping.xaml 的交互逻辑
    /// </summary>
    internal partial class Mapping : UserControl
    {
        public Mapping()
        {
            InitializeComponent();
            MappingData.Dict_DieState_Color = new Dictionary<InspectionResult, Color>();
            MappingData.Dict_DieState_Color[InspectionResult.OK] = Color.FromRgb(76, 175, 80);
            MappingData.Dict_DieState_Color[InspectionResult.NG] = Color.FromRgb(244, 67, 54);
            MappingData.Dict_DieState_Color[InspectionResult.N2K] = Color.FromRgb(255, 239, 59);
            MappingData.Dict_DieState_Color[InspectionResult.K2N] = Color.FromRgb(255, 165, 0);
            MappingData.Dict_DieState_Color[InspectionResult.SKIP] = Color.FromRgb(0, 188, 212);
            this.DataContext = mappingViewModel;
        }

        MappingViewModel mappingViewModel = new MappingViewModel();

        /// <summary>
        /// 读取错误码用于将OK结果复看成NG
        /// </summary>
        public Dictionary<int, string> Dict_DefectType;
        /// <summary>
        /// 当前选中的行列坐标
        /// </summary>
        public int GlobalRow;
        public int GlobalColumn;
        /// <summary>
        /// 数据库相关信息
        /// </summary>
        public string DbFilePath;
        public int LotIndex;
        public string FrameName;
        public List<ReviewEditData> List_editData;

        public ObservableCollection<MappingData> DataCollection { set; get; } = new ObservableCollection<MappingData>();
        public ObservableCollection<ExampleData> ExampleCollection { get; set; } = new ObservableCollection<ExampleData>();

        private int RowCount;

        private int ColumnCount;

        /// <summary>
        /// 单元格最小宽度
        /// </summary>
        public static double MinCellWidth { get; set; } = 20;

        /// <summary>
        /// 单元格最小高度
        /// </summary>
        public static double MinCellHeight { get; set; } = 20;

        /// <summary>
        /// 默认边框厚度
        /// </summary>
        public static double DefaultBorderThickness { get; set; } = 1;

        /// <summary>
        /// 默认边框填充颜色
        /// </summary>
        public static Color DefaultBorderColor { get; set; } = Color.FromRgb(182, 182, 182);

        #region 选中单元格变更事件
        public Action<int, int, InspectionResult> OnSelectedDieChanged;
        private void SelectedDieChanged(int rowIndex, int columnIndex, InspectionResult inspectionResult)
        {
            OnSelectedDieChanged?.Invoke(rowIndex, columnIndex, inspectionResult);
            mappingViewModel.ChangeSelectedText(rowIndex, columnIndex, inspectionResult);
        }
        #endregion

        /// <summary>
        /// 当前选中个体
        /// </summary>
        private MappingData SelectedItem;

        /// <summary>
        /// 当前鼠标移动个体
        /// </summary>
        private MappingData MouseMoveItem;

        /// <summary>
        /// 图谱初始化
        /// </summary>
        /// <param name="rowCount">行数</param>
        /// <param name="columnCount">列数</param>
        /// <param name="dic_DieState_Color">芯片状态-颜色 字典</param>
        /// <param name="initialDieState">初始状态</param>
        public void Initial(int rowCount, int columnCount, List<InspectionDataView> list_InspectionDataView)
        {
            DataCollection.Clear();
            ExampleCollection.Clear();
            double cellWidth, cellHeight;
            RowCount = rowCount;
            ColumnCount = columnCount;
            SetCellSize(RowCount, ColumnCount, out cellWidth, out cellHeight);
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    MappingData data = new MappingData();
                    data.BorderThickness = DefaultBorderThickness;
                    data.Width = cellWidth;
                    data.Height = cellHeight;
                    data.VariableX = j * data.Width;
                    data.VariableY = i * data.Height;
                    data.RowIndex = i;
                    data.ColumnIndex = j;
                    data.DieState = InspectionResult.SKIP;
                    data.NormalBorderColor = DefaultBorderColor;
                    DataCollection.Add(data);
                }
            }
            foreach (InspectionDataView dataView in list_InspectionDataView)
            {
                DataCollection[dataView.RowIndex * ColumnCount + dataView.ColumnIndex].DieState = dataView.InspectionResult;
                if (dataView.List_DefectData.Count != 0)
                {
                    string[] defectTypeStrings = new string[dataView.List_DefectData.Count];
                    for (int i = 0; i < dataView.List_DefectData.Count; i++)
                    {
                        defectTypeStrings[i] = dataView.List_DefectData[i].ToString();
                    }
                    DataCollection[dataView.RowIndex * ColumnCount + dataView.ColumnIndex].DefectTypeIndexString = string.Join(";", defectTypeStrings);
                }
     
            }
            double fontSize = SetFontSize(cellWidth, cellHeight);
            foreach (MappingData data in DataCollection)
            {
                data.FontSize = fontSize;
            }

            foreach (KeyValuePair<InspectionResult, Color> kv in MappingData.Dict_DieState_Color)
            {
                ExampleData data = new ExampleData();
                data.DieState = InspectionResultsConverter.ToString(kv.Key);
                data.FillColor = kv.Value.ToString();
                data.BorderColor = DefaultBorderColor.ToString();
                data.Count = DataCollection.Where(d => d.DieState == kv.Key).Count();
                ExampleCollection.Add(data);
            }
        }

        /// <summary>
        /// 图谱初始化 重载
        /// </summary>
        /// <param name="rowCount">行数</param>
        /// <param name="columnCount">列数</param>
        /// <param name="dic_DieState_Color">芯片状态-颜色 字典</param>
        /// <param name="initialDieState">初始状态</param>
        /// <param name="Dict_DefectType">错误码</param>
        /// <param name="dbFilePath">数据库路径</param>
        /// <param name="lotIndex">批次号</param>
        /// <param name="frameName">盘名</param>
        public void Initial(int rowCount, int columnCount, List<InspectionDataView> list_InspectionDataView ,Dictionary<int, string> Dict_DefectType,string dbFilePath,int lotIndex,string frameName,List<ReviewEditData> list_editData)
        {
            this.DbFilePath = dbFilePath;
            this.LotIndex = lotIndex;
            this.FrameName = frameName;
            this.List_editData = list_editData;
            DataCollection.Clear();
            ExampleCollection.Clear();
            double cellWidth, cellHeight;
            RowCount = rowCount;
            ColumnCount = columnCount;
            SetCellSize(RowCount, ColumnCount, out cellWidth, out cellHeight);
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    MappingData data = new MappingData();
                    data.BorderThickness = DefaultBorderThickness;
                    data.Width = cellWidth;
                    data.Height = cellHeight;
                    data.VariableX = j * data.Width;
                    data.VariableY = i * data.Height;
                    data.RowIndex = i;
                    data.ColumnIndex = j;
                    data.DieState = InspectionResult.SKIP;
                    data.NormalBorderColor = DefaultBorderColor;
                    DataCollection.Add(data);
                }
            }
            foreach (InspectionDataView dataView in list_InspectionDataView)
            {
                DataCollection[dataView.RowIndex * ColumnCount + dataView.ColumnIndex].DieState = dataView.InspectionResult;
                if (dataView.List_DefectData.Count != 0)
                {
                    string[] defectTypeStrings = new string[dataView.List_DefectData.Count];
                    for (int i = 0; i < dataView.List_DefectData.Count; i++)
                    {
                        defectTypeStrings[i] = dataView.List_DefectData[i].ToString();
                    }
                    DataCollection[dataView.RowIndex * ColumnCount + dataView.ColumnIndex].DefectTypeIndexString = string.Join(";", defectTypeStrings);
                }

            }
            double fontSize = SetFontSize(cellWidth, cellHeight);
            foreach (MappingData data in DataCollection)
            {
                data.FontSize = fontSize;
            }

            foreach (KeyValuePair<InspectionResult, Color> kv in MappingData.Dict_DieState_Color)
            {
                ExampleData data = new ExampleData();
                data.DieState = InspectionResultsConverter.ToString(kv.Key);
                data.FillColor = kv.Value.ToString();
                data.BorderColor = DefaultBorderColor.ToString();
                data.Count = DataCollection.Where(d => d.DieState == kv.Key).Count();
                ExampleCollection.Add(data);
            }
            this.Dict_DefectType = Dict_DefectType;

            //foreach (ReviewEditData item in List_editData)
            //{
            //    SetDieState(item.Row, item.Col, InspectionResults.K2N);
            //}
            
        }

        /// <summary>
        /// 刷新图谱，不改变芯片状态
        /// </summary>
        public void Refresh()
        {
            if (DataCollection.Count == 0) return;
            double cellWidth, cellHeight;
            SetCellSize(RowCount, ColumnCount, out cellWidth, out cellHeight);         
            double fontSize = SetFontSize( cellWidth, cellHeight);
            foreach (MappingData data in DataCollection)
            {
                data.BorderThickness = DefaultBorderThickness;
                data.Width = cellWidth;
                data.Height = cellHeight;
                data.FontSize = (int)fontSize;
                data.VariableX = data.ColumnIndex * data.Width;
                data.VariableY = data.RowIndex * data.Height;
                data.NormalBorderColor = DefaultBorderColor;
            }
        }

        /// <summary>
        /// 根据控件大小计算单元格大小，如果单元格宽高度小于预设值，则维持预设值，显示滚动条
        /// </summary>
        /// <param name="columnCount">列数</param>
        /// <param name="rowCount">行数</param>
        /// <param name="cellWidth">单元格宽度</param>
        /// <param name="cellHeight">单元格高度</param>
        private void SetCellSize(int rowCount, int columnCount, out double cellWidth, out double cellHeight)
        {
            grid.Width = this.Width;
            grid.Height = this.Height - mappingViewModel.BottomBarHeight;
            cellWidth = grid.Width / columnCount;
            cellHeight = grid.Height / rowCount;

            bool isHorizontalScroll = false;
            bool isVerticalScroll = false;

            if (cellWidth < MinCellWidth)
            {
                isHorizontalScroll = true;
            }

            if (cellHeight < MinCellHeight)
            {
                isVerticalScroll = true;
            }

            if (isHorizontalScroll && isVerticalScroll)
            {
                cellWidth = MinCellWidth;
                grid.Width = cellWidth * columnCount;
                cellHeight = MinCellHeight;
                grid.Height = cellHeight * rowCount;
            }
            else if (isHorizontalScroll && !isVerticalScroll)
            {
                cellWidth = MinCellWidth;
                grid.Width = cellWidth * columnCount;
                if ((grid.Height - SystemParameters.HorizontalScrollBarHeight) / rowCount < MinCellHeight)
                {
                    cellHeight = MinCellHeight;
                    grid.Height = cellHeight * rowCount;
                }
                else
                {
                    grid.Height -= SystemParameters.HorizontalScrollBarHeight;
                    cellHeight = grid.Height / rowCount;
                }
            }
            else if (!isHorizontalScroll && isVerticalScroll)
            {
                cellHeight = MinCellHeight;
                grid.Height = cellHeight * rowCount;
                if ((grid.Width - SystemParameters.VerticalScrollBarWidth) / columnCount < MinCellWidth)
                {
                    cellWidth = MinCellWidth;
                    grid.Width = cellWidth * columnCount;
                }
                else
                {
                    grid.Width -= SystemParameters.VerticalScrollBarWidth;
                    cellWidth = grid.Width / columnCount;
                }
            }
        }

        private double SetFontSize(double cellWidth, double cellHeight)
        {
            double maxFontWidth = 0;
            double maxFontHeight = 0;
            double maxFontSize = 100;
            foreach (MappingData data in DataCollection)
            {
                if (IsShowDefectTypeIndex)
                {
                    data.Text = data.DefectTypeIndexString;
                    if (string.IsNullOrEmpty(data.Text))
                    {
                        continue;
                    }
                }
                else
                {
                    data.Text = string.Format("{0}-{1}", data.RowIndex + 1, data.ColumnIndex + 1);
                }
                var formattedText = new FormattedText(data.Text,
                           CultureInfo.CurrentUICulture,
                           FlowDirection.LeftToRight,
                           new Typeface("MicroSoft YaHei"),
                           maxFontSize,
                           Brushes.Black);
                Size desiredSize = new Size(formattedText.Width, formattedText.Height);
                if (maxFontWidth < desiredSize.Width)
                {
                    maxFontWidth = desiredSize.Width;
                }
                if (maxFontHeight < desiredSize.Height)
                {
                    maxFontHeight = desiredSize.Height;
                }
            }
            if (maxFontHeight == 0 || maxFontWidth == 0)
            {
                return 0;
            }
            double fontSize = maxFontSize;
            double margin = 4;
            double desiredHeight = maxFontHeight + margin;
            double desiredWidth = maxFontWidth + margin;
            if (cellHeight < desiredHeight)
            {
                double factor = (desiredHeight - margin) / (cellHeight - margin);
                fontSize = Math.Min(fontSize, maxFontSize / factor);
            }
            if (cellWidth < desiredWidth)
            {
                double factor = (desiredWidth - margin) / (cellWidth - margin);
                fontSize = Math.Min(fontSize, maxFontSize / factor);
            }
            return fontSize;
        }

        /// <summary>
        /// 鼠标移动事件，更改移动单元格颜色
        /// </summary>
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            //Rectangle rectangle = sender as Rectangle;
            //MappingData data = rectangle.DataContext as MappingData;
            Grid grid = sender as Grid;
            MappingData data = grid.DataContext as MappingData;
            data.IsMouseMove = true;
            if (MouseMoveItem == null)
            {
                MouseMoveItem = data;
                return;
            }
            if (MouseMoveItem != null && data != MouseMoveItem)
            {
                MouseMoveItem.IsMouseMove = false;
                MouseMoveItem = data;
            }
            
        }

        /// <summary>
        /// 鼠标移出控件，清除鼠标移动个体
        /// </summary>
        private void userControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (MouseMoveItem != null)
            {
                MouseMoveItem.IsMouseMove = false;
                MouseMoveItem = null;
            }
        }

        /// <summary>
        /// 鼠标选中事件，更改选中单元格颜色，调用变化委托
        /// </summary>
        private void Rectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            MappingData data = grid.DataContext as MappingData;
            data.IsSelected = true;
            if (SelectedItem == null)
            {
                SelectedItem = data;
                SelectedDieChanged(SelectedItem.RowIndex, SelectedItem.ColumnIndex, SelectedItem.DieState);
                return;
            }
            if (SelectedItem != null && data != SelectedItem)
            {
                SelectedItem.IsSelected = false;
                SelectedItem = data;
                SelectedDieChanged(SelectedItem.RowIndex, SelectedItem.ColumnIndex, SelectedItem.DieState);
            }
        }

        /// <summary>
        /// 鼠标右键菜单选择 复看错误代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {


            Grid grid = sender as Grid;
            MappingData data = grid.DataContext as MappingData;
            data.IsSelected = true;

            GlobalRow = data.RowIndex;
            GlobalColumn = data.ColumnIndex;


            //与数据库的复看数据进行比较 当前芯片是否被复看NG过 然后显示 取消
            bool IsEditNG =false;
            foreach (ReviewEditData reviewEditData in List_editData)
            {
                if (reviewEditData.Row == data.RowIndex && reviewEditData.Col == data.ColumnIndex) IsEditNG = true;
            }

            // 判断是OK 才执行右键菜单事件 或者是之前复看成NG的
            if (data.DieState == InspectionResult.OK || IsEditNG)
            {
                ContextMenu menu = new ContextMenu();
                //Menu menu = new Menu();
                MenuItem EditErrCode = new MenuItem();
                EditErrCode.Header = "编辑错误码";
                foreach (KeyValuePair<int, string> kvp in Dict_DefectType)
                {
                    MenuItem DefectType = new MenuItem();
                    DefectType.Header = kvp.Key + "_" + kvp.Value;
                    DefectType.Click += DefectType_Click;
                    EditErrCode.Items.Add(DefectType);
                }

                menu.Items.Add(EditErrCode);

                MenuItem CancelErrCode = new MenuItem();
                CancelErrCode.Header = "取消错误复判";
                CancelErrCode.Click += CancelErrCode_Click;
                CancelErrCode.IsEnabled = IsEditNG;

                menu.Items.Add(CancelErrCode);

                menu.IsOpen = true;
             }
        }

        /// <summary>
        /// 点击事件 去里面判断第几个错误码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefectType_Click(object sender, RoutedEventArgs e)
        {
            MenuItem DefectType = sender as MenuItem;


            //写入数据库 使用Review类中的函数
            SQLiteOperation.UpdateErrCode(DbFilePath,LotIndex,FrameName,GlobalRow,GlobalColumn,DefectType.Header.ToString());

            //修改状态
            SetDieState(GlobalRow, GlobalColumn, InspectionResult.K2N);
            //更新当前List_editdata
            SQLiteOperation.ReadUpdateErrCode(DbFilePath, LotIndex, FrameName, out List<ReviewEditData> temp);
            this.List_editData = temp;
        }

        /// <summary>
        /// 取消复看 将添加到 Review_Lot_XX 表中的数据删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelErrCode_Click(object sender, RoutedEventArgs e)
        {
            //删除数据库中的数据
            SQLiteOperation.DeleteUpdateErrCode(DbFilePath, LotIndex, FrameName, GlobalRow, GlobalColumn);
            //修改状态
            SetDieState(GlobalRow, GlobalColumn, InspectionResult.OK);
            //更新当前List_editdata
            SQLiteOperation.ReadUpdateErrCode(DbFilePath, LotIndex, FrameName, out List<ReviewEditData> temp);
            this.List_editData = temp;

        }



        /// <summary>
        /// 修改芯片状态
        /// </summary>
        /// <param name="rowIndex">行序号，从0开始</param>
        /// <param name="columnIndex">列序号，从0开始</param>
        /// <param name="state">芯片状态</param>
        public void SetDieState(int rowIndex, int columnIndex, InspectionResult inspectionResult)
        {
            if (!IsInitialized) return;
            MappingData data = DataCollection[rowIndex * ColumnCount + columnIndex];

            ExampleData example = ExampleCollection.Where(e => e.DieState == InspectionResultsConverter.ToString(data.DieState)).FirstOrDefault();
            example.Count--;

            data.DieState = inspectionResult;

            example = ExampleCollection.Where(e => e.DieState == InspectionResultsConverter.ToString(data.DieState)).FirstOrDefault();
            example.Count++;
            mappingViewModel.ChangeSelectedText(rowIndex, columnIndex, inspectionResult);

            ScrollToView(data);
        }

        /// <summary>
        /// 将单元格移动到视野内
        /// </summary>
        private void ScrollToView(MappingData data)
        {
            if (scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
            {
                double viewWidth = scrollViewer.ActualWidth;
                if (scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                {
                    viewWidth = scrollViewer.ActualWidth - SystemParameters.VerticalScrollBarWidth;
                }
                double cellLeft = data.VariableX;
                double cellRight = data.VariableX + data.Width;
                if (cellLeft < scrollViewer.HorizontalOffset || cellRight > scrollViewer.HorizontalOffset + viewWidth)
                {
                    double cellCenter = data.VariableX + data.Width / 2;
                    double horizontalOffset = cellCenter - viewWidth / 2;
                    if (horizontalOffset < 0)
                    {
                        horizontalOffset = 0;
                    }
                    else if (horizontalOffset > scrollViewer.ScrollableWidth)
                    {
                        horizontalOffset = scrollViewer.ScrollableWidth;
                    }
                    scrollViewer.ScrollToHorizontalOffset(horizontalOffset);
                }
            }
            if (scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                double viewHeight = scrollViewer.ActualHeight;
                if (scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
                {
                    viewHeight = scrollViewer.ActualHeight - SystemParameters.HorizontalScrollBarHeight;
                }
                double cellTop = data.VariableY;
                double cellButton = data.VariableY + data.Height;
                if (cellTop < scrollViewer.VerticalOffset || cellButton > scrollViewer.VerticalOffset + viewHeight)
                {
                    double cellCenter = data.VariableY + data.Height / 2;
                    double verticalOffset = cellCenter - viewHeight / 2;
                    if (verticalOffset < 0)
                    {
                        verticalOffset = 0;
                    }
                    else if (verticalOffset > scrollViewer.ScrollableHeight)
                    {
                        verticalOffset = scrollViewer.ScrollableHeight;
                    }
                    scrollViewer.ScrollToVerticalOffset(verticalOffset);
                }
            }
        }

        public void ChangeSelectedDieExternal(int rowIndex, int columnIndex)
        {
            MappingData data = DataCollection[rowIndex * ColumnCount + columnIndex];
            data.IsSelected = true;
            if (SelectedItem == null)
            {
                SelectedItem = data;
                mappingViewModel.ChangeSelectedText(data.RowIndex, data.ColumnIndex, data.DieState);
                return;
            }
            if (SelectedItem != null && data != SelectedItem)
            {
                SelectedItem.IsSelected = false;
                SelectedItem = data;
                mappingViewModel.ChangeSelectedText(data.RowIndex, data.ColumnIndex, data.DieState);
            }
            ScrollToView(data);
        }

        private bool IsShowDefectTypeIndex = false;

        public void ShowDefectTypeIndex(bool isShowDefectTypeIndex)
        {
            IsShowDefectTypeIndex = isShowDefectTypeIndex;
            Refresh();
        }
    }
}
