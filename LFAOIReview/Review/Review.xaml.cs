using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Forms;
using System.Threading;
using System.Windows.Threading;

namespace LFAOIReview
{
    /// <summary>
    /// Review.xaml 的交互逻辑
    /// </summary>
    internal partial class Review : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public Review()
        {
            InitializeComponent();

            LoadConfig();

            Mapping.OnSelectedDieChanged += MappingSelectedDieChanged;
            Report.OnShowMessage += Report_OnShowMessage;

            HideLoadDataGrid();
        }
        public Review( DataShow dataShow)
        {
            InitializeComponent();

            LoadConfig();

            Mapping.OnSelectedDieChanged += MappingSelectedDieChanged;
            Report.OnShowMessage += Report_OnShowMessage;

            HideLoadDataGrid();
            Show(dataShow);
        }


        #region NotifyPropertyChanged委托
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region 属性变更通知
        private string _DbDirectory;
        public string DbDirectory
        {
            get { return _DbDirectory; }
            set
            {
                if (_DbDirectory != value)
                {
                    _DbDirectory = value;
                    OnPropertyChanged();

                    if (!File.Exists(ConfigIniPath))
                    {
                        File.Create(ConfigIniPath);
                    }
                    IniHelper.WriteValue(ConfigIniPath, "Config", "DbDirectory", DbDirectory);
                }
            }
        }

        private string _ImageDirectory;
        public string ImageDirectory
        {
            get { return _ImageDirectory; }
            set
            {
                if (_ImageDirectory != value)
                {
                    _ImageDirectory = value;
                    OnPropertyChanged();

                    if (!File.Exists(ConfigIniPath))
                    {
                        File.Create(ConfigIniPath);
                    }
                    IniHelper.WriteValue(ConfigIniPath, "Config", "ImageDirectory", ImageDirectory);
                }
            }
        }

        private string _ExcelDirectory;
        public string ExcelDirectory
        {
            get { return _ExcelDirectory; }
            set
            {
                if (_ExcelDirectory != value)
                {
                    _ExcelDirectory = value;
                    OnPropertyChanged();
                }

                if (!File.Exists(ConfigIniPath))
                {
                    File.Create(ConfigIniPath);
                }
                IniHelper.WriteValue(ConfigIniPath, "Config", "ExcelDirectory", ExcelDirectory);
            }
        }

        private bool _IsShowLoadDataGrid = false;
        public bool IsShowLoadDataGrid
        {
            get { return _IsShowLoadDataGrid; }
            set
            {
                if (_IsShowLoadDataGrid != value)
                {
                    _IsShowLoadDataGrid = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsShowHTWindow = true;
        public bool IsShowHTWindow
        {
            get { return _IsShowHTWindow; }
            set
            {
                if (_IsShowHTWindow != value)
                {
                    _IsShowHTWindow = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsShowReportGrid = false;
        public bool IsShowReportGrid
        {
            get { return _IsShowReportGrid; }
            set
            {
                if (_IsShowReportGrid != value)
                {
                    _IsShowReportGrid = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsShowMappingConfigGrid = false;
        public bool IsShowMappingConfigGrid
        {
            get { return _IsShowMappingConfigGrid; }
            set
            {
                if (_IsShowMappingConfigGrid != value)
                {
                    _IsShowMappingConfigGrid = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MappingMinCellWidth
        {
            get { return Mapping.MinCellWidth; }
            set
            {
                if (Mapping.MinCellWidth != value)
                {
                    Mapping.MinCellWidth = value;
                    OnPropertyChanged();

                    if (!File.Exists(ConfigIniPath))
                    {
                        File.Create(ConfigIniPath);
                    }
                    IniHelper.WriteValue(ConfigIniPath, "Config", "MappingMinCellWidth", MappingMinCellWidth.ToString());
                }
            }
        }

        public double MappingMinCellHeight
        {
            get { return Mapping.MinCellHeight; }
            set
            {
                if (Mapping.MinCellHeight != value)
                {
                    Mapping.MinCellHeight = value;
                    OnPropertyChanged();

                    if (!File.Exists(ConfigIniPath))
                    {
                        File.Create(ConfigIniPath);
                    }
                    IniHelper.WriteValue(ConfigIniPath, "Config", "MappingMinCellHeight", MappingMinCellHeight.ToString());
                }
            }
        }

        public double MappingBorderThickness
        {
            get { return Mapping.DefaultBorderThickness; }
            set
            {
                if (Mapping.DefaultBorderThickness != value)
                {
                    Mapping.DefaultBorderThickness = value;
                    OnPropertyChanged();

                    if (!File.Exists(ConfigIniPath))
                    {
                        File.Create(ConfigIniPath);
                    }
                    IniHelper.WriteValue(ConfigIniPath, "Config", "MappingBorderThickness", MappingBorderThickness.ToString());
                }
            }
        }


        private string _ProductCode;
        public string ProductCode
        {
            get { return _ProductCode; }
            set
            {
                if (_ProductCode != value)
                {
                    _ProductCode = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _LotName;
        public string LotName
        {
            get { return _LotName; }
            set
            {
                if (_LotName != value)
                {
                    _LotName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _FrameName;
        public string FrameName
        {
            get { return _FrameName; }
            set
            {
                if (_FrameName != value)
                {
                    _FrameName = value;
                    OnPropertyChanged();
                }
            }
        }

        private DefectDataView _SelectedDefect;
        public DefectDataView SelectedDefect
        {
            get { return _SelectedDefect; }
            set
            {
                if (_SelectedDefect != value)
                {
                    _SelectedDefect = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SelectedDefectImageChannel;
        public string SelectedDefectImageChannel
        {
            get { return _SelectedDefectImageChannel; }
            set
            {
                if (_SelectedDefectImageChannel != value)
                {
                    _SelectedDefectImageChannel = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _NGDefectCount;
        public int NGDefectCount
        {
            get { return _NGDefectCount; }
            set
            {
                if (_NGDefectCount != value)
                {
                    _NGDefectCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _N2KDefectCount;
        public int N2KDefectCount
        {
            get { return _N2KDefectCount; }
            set
            {
                if (_N2KDefectCount != value)
                {
                    _N2KDefectCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsExcelChinese = true;
        public bool IsExcelChinese
        {
            get { return _IsExcelChinese; }
            set
            {
                if (_IsExcelChinese != value)
                {
                    _IsExcelChinese = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        private int _curFrame;

        public int CurFrame
        {
            get { return _curFrame; }
            set
            {
                if(_curFrame != value)
                {
                    _curFrame = value;
                    OnPropertyChanged();
                }
            }

        }

        /// <summary>
        /// 0 普通模式  
        /// 1 武汉二维码
        /// </summary>
        public int ReportModel;


      

        public ObservableCollection<ComboBoxData> List_Product { get; set; } = new ObservableCollection<ComboBoxData>();
        public ObservableCollection<string> List_Lot { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> List_Frame { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<DefectDataView> List_DefectDataView { get; set; } = new ObservableCollection<DefectDataView>();
        public ObservableCollection<ComboBoxData> List_DieImages { get; set; } = new ObservableCollection<ComboBoxData>();


        private List<InspectionDataView> List_InspectionDataView;
        private Dictionary<int, string> Dict_DefectTyoe;
        private string DbFilePath;
        private string SelectedProductCode;
        private string SelectedLotName;
        private int LotIndex;
     
        private int RowCount { get; set; }
        private int ColumnCount { get; set; }

        #region 设置信息保存
        private string ConfigIniPath = System.Windows.Forms.Application.StartupPath + "\\LFAOIReviewConfig.ini";

        private void LoadConfig()
        {
            if (!File.Exists(ConfigIniPath))
            {
                DbDirectory = System.Windows.Forms.Application.StartupPath;
                ImageDirectory = System.Windows.Forms.Application.StartupPath;
                ExcelDirectory = System.Windows.Forms.Application.StartupPath;
                MappingMinCellWidth = 20;
                MappingMinCellHeight = 20;
                return;
            }

            DbDirectory = IniHelper.ReadValue(ConfigIniPath, "Config", "DbDirectory");
            if (string.IsNullOrEmpty(DbDirectory))
            {
                DbDirectory = System.Windows.Forms.Application.StartupPath;
            }

            ImageDirectory = IniHelper.ReadValue(ConfigIniPath, "Config", "ImageDirectory");
            if (string.IsNullOrEmpty(ImageDirectory))
            {
                ImageDirectory = System.Windows.Forms.Application.StartupPath;
            }

            ExcelDirectory = IniHelper.ReadValue(ConfigIniPath, "Config", "ExcelDirectory");
            if (string.IsNullOrEmpty(ExcelDirectory))
            {
                ExcelDirectory = System.Windows.Forms.Application.StartupPath;
            }

            try
            {
                MappingMinCellWidth = double.Parse(IniHelper.ReadValue(ConfigIniPath, "Config", "MappingMinCellWidth"));
                MappingMinCellHeight = double.Parse(IniHelper.ReadValue(ConfigIniPath, "Config", "MappingMinCellHeight"));
                MappingBorderThickness = double.Parse(IniHelper.ReadValue(ConfigIniPath, "Config", "MappingBorderThickness"));
            }
            catch
            {
                MappingMinCellWidth = 20;
                MappingMinCellHeight = 20;
                MappingBorderThickness = 0.5;
            }
        }
        #endregion

        #region 界面切换

        /// <summary>
        /// 隐藏加载数据界面
        /// </summary>
        private void HideLoadDataGrid()
        {
            IsShowLoadDataGrid = false;
            IsShowHTWindow = true;
        }

        private void HideReportGrid()
        {
            IsShowReportGrid = false;
            IsShowHTWindow = true;
        }

        private void HideMappingConfigGrid()
        {
            IsShowMappingConfigGrid = false;
            IsShowHTWindow = true;
        }

        /// <summary>
        /// 显示加载数据界面
        /// </summary>
        private void mi_ShowLoadDataGrid_Click(object sender, RoutedEventArgs e)
        {
            IsShowLoadDataGrid = true;
            IsShowHTWindow = false;
            GetProductDb(DbDirectory);
        }

        //快速切换前一盘
        private void mi_PreviousFrame_click(object sender , RoutedEventArgs e)
        {
            if (CurFrame != 0)
            {
                cb_Frame.SelectedIndex = CurFrame - 1;
                System.Windows.Controls.ComboBox cb = cb_Frame;
                if (cb.SelectedItem == null) return;
                string frameName = cb.SelectedItem.ToString();
                List<DefectDataView> list_DefectDataView;
                if (!SQLiteOperation.GetData(DbFilePath, LotIndex, frameName, out List_InspectionDataView, out list_DefectDataView, out Dict_DefectTyoe))
                {
                    return;
                }
                List_DefectDataView.Clear();
                NGDefectCount = 0;
                N2KDefectCount = 0;
                int i = 1;
                foreach (DefectDataView defectView in list_DefectDataView)
                {
                    defectView.DisplayIndex = i;
                    defectView.DieBelongTo = string.Format("{0} - {1}", defectView.RowIndex + 1, defectView.ColumnIndex + 1);
                    if (Dict_DefectTyoe.ContainsKey(defectView.DefectTypeIndex))
                    {
                        defectView.DefectType = Dict_DefectTyoe[defectView.DefectTypeIndex];
                    }
                    else
                    {
                        defectView.DefectType = "未知缺陷";
                    }
                    defectView.DisplayErrorDetail = defectView.ErrorDetail;
                    List_DefectDataView.Add(defectView);
                    InspectionDataView dataView = List_InspectionDataView.Where(d => d.DbIndex == defectView.InspectionDataDbIndex).FirstOrDefault();
                    if (dataView == null)
                    {
                        throw new Exception("未找到缺陷所属芯片");
                    }
                    defectView.InspectionDataListIndex = List_InspectionDataView.IndexOf(dataView);
                    dataView.List_DefectDataListIndex.Add(List_DefectDataView.IndexOf(defectView));
                    dataView.List_DefectData.Add(defectView.DefectTypeIndex);
                    if (defectView.Result == 0)
                    {
                        NGDefectCount++;
                    }
                    else
                    {
                        N2KDefectCount++;
                    }

                    i++;
                }
                List<ReviewEditData> list_editdata = new List<ReviewEditData>();
                SQLiteOperation.ReadUpdateErrCode(DbFilePath, LotIndex, frameName, out list_editdata);
                //重载mapping初始化函数 传入错误码 数据库相关信息
                Mapping.Initial(RowCount, ColumnCount, List_InspectionDataView, Dict_DefectTyoe, DbFilePath, LotIndex, frameName, list_editdata);
                HideLoadDataGrid();
                ProductCode = SelectedProductCode;
                LotName = SelectedLotName;
                FrameName = frameName;
                cb_ShowDefectTypeIndex.SelectedIndex = 0;
            }
        }

        //快速切换下一盘
        private void mi_NextFrame_click(object sender, RoutedEventArgs e)
        {
            if (CurFrame != cb_Frame.Items.Count)
            {
                cb_Frame.SelectedIndex = CurFrame + 1;
                System.Windows.Controls.ComboBox cb = cb_Frame;
                if (cb.SelectedItem == null) return;
                string frameName = cb.SelectedItem.ToString();
                List<DefectDataView> list_DefectDataView;
                if (!SQLiteOperation.GetData(DbFilePath, LotIndex, frameName, out List_InspectionDataView, out list_DefectDataView, out Dict_DefectTyoe))
                {
                    return;
                }
                List_DefectDataView.Clear();
                NGDefectCount = 0;
                N2KDefectCount = 0;
                int i = 1;
                foreach (DefectDataView defectView in list_DefectDataView)
                {
                    defectView.DisplayIndex = i;
                    defectView.DieBelongTo = string.Format("{0} - {1}", defectView.RowIndex + 1, defectView.ColumnIndex + 1);
                    if (Dict_DefectTyoe.ContainsKey(defectView.DefectTypeIndex))
                    {
                        defectView.DefectType = Dict_DefectTyoe[defectView.DefectTypeIndex];
                    }
                    else
                    {
                        defectView.DefectType = "未知缺陷";
                    }
                    defectView.DisplayErrorDetail = defectView.ErrorDetail;
                    List_DefectDataView.Add(defectView);
                    InspectionDataView dataView = List_InspectionDataView.Where(d => d.DbIndex == defectView.InspectionDataDbIndex).FirstOrDefault();
                    if (dataView == null)
                    {
                        throw new Exception("未找到缺陷所属芯片");
                    }
                    defectView.InspectionDataListIndex = List_InspectionDataView.IndexOf(dataView);
                    dataView.List_DefectDataListIndex.Add(List_DefectDataView.IndexOf(defectView));
                    dataView.List_DefectData.Add(defectView.DefectTypeIndex);
                    if (defectView.Result == 0)
                    {
                        NGDefectCount++;
                    }
                    else
                    {
                        N2KDefectCount++;
                    }

                    i++;
                }
                List<ReviewEditData> list_editdata = new List<ReviewEditData>();
                SQLiteOperation.ReadUpdateErrCode(DbFilePath, LotIndex, frameName, out list_editdata);
                //重载mapping初始化函数 传入错误码 数据库相关信息
                Mapping.Initial(RowCount, ColumnCount, List_InspectionDataView, Dict_DefectTyoe, DbFilePath, LotIndex, frameName, list_editdata);
                HideLoadDataGrid();
                ProductCode = SelectedProductCode;
                LotName = SelectedLotName;
                FrameName = frameName;
                cb_ShowDefectTypeIndex.SelectedIndex = 0;
            }
        }


        private void mi_ShowReportGrid_Click(object sender, RoutedEventArgs e)
        {
            IsShowReportGrid = true;
            IsShowHTWindow = false;
            GetProductDb(DbDirectory);
        }

        private void btn_ShowMappingConfigGrid_Click(object sender, RoutedEventArgs e)
        {
            IsShowMappingConfigGrid = true;
            IsShowHTWindow = false;
        }

        private void btn_HideLoadDataGrid_Click(object sender, RoutedEventArgs e)
        {
            HideLoadDataGrid();
        }

        private void btn_HideReportGrid_Click(object sender, RoutedEventArgs e)
        {
            HideReportGrid();
        }

        private void btn_HideMappingConfigGrid_Click(object sender, RoutedEventArgs e)
        {
            HideMappingConfigGrid();
        }

        #endregion

        #region  选择文件夹
        /// <summary>
        /// 选择图像文件夹
        /// </summary>
        private void btn_SelectImageDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (Directory.Exists(ImageDirectory))
                {
                    fbd.SelectedPath = ImageDirectory;
                }
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ImageDirectory = fbd.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 选择数据库文件夹
        /// </summary>
        private void btn_SelectDbDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (Directory.Exists(DbDirectory))
                {
                    fbd.SelectedPath = DbDirectory;
                }
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    DbDirectory = fbd.SelectedPath;
                    GetProductDb(DbDirectory);
                }
            }
        }

        /// <summary>
        /// 选择Excel文件夹
        /// </summary>
        private void btn_SelectExcelDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ExcelDirectory = fbd.SelectedPath;
                }
            }
        }
        #endregion

        /// <summary>
        /// 根据文件夹获取产品数据库
        /// </summary>
        /// <param name="dbDirectory"></param>
        public void GetProductDb(string dbDirectory)
        {
            try
            {
                List_Product.Clear();
                List_Lot.Clear();
                List_Frame.Clear();
                DirectoryInfo dbDirectoryInfo = new DirectoryInfo(dbDirectory);
                FileInfo[] fis = dbDirectoryInfo.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    if (fi.Extension.ToLower().Equals(".db"))
                    {
                        string productCode;
                        if (SQLiteOperation.GetProductDb(fi.FullName, out productCode))
                        {
                            List_Product.Add(new ComboBoxData { DisplayName = productCode, Tag = fi.FullName });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 选择产品，获取批次列表
        /// </summary>
        private void cb_Product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                System.Windows.Controls.ComboBox cb = sender as System.Windows.Controls.ComboBox;
                if (cb.SelectedItem == null) return;
                ComboBoxData data = cb.SelectedItem as ComboBoxData;
                SelectedProductCode = data.DisplayName;
                DbFilePath = data.Tag;
                List<string> list_LotName;
                if (SQLiteOperation.GetLot(DbFilePath, out list_LotName))
                {
                    List_Lot.Clear();
                    List_Frame.Clear();
                    foreach (string lotName in list_LotName)
                    {
                        List_Lot.Add(lotName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 选择批次，获取盘号列表
        /// </summary>
        private void cb_Lot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                System.Windows.Controls.ComboBox cb = sender as System.Windows.Controls.ComboBox;
                if (cb.SelectedItem == null) return;
                string lotName = cb.SelectedItem.ToString();
                SelectedLotName = lotName;
                List<string> list_FrameName;
                int rowCount = 0;
                int columnCount = 0;
                if (SQLiteOperation.GetFrame(DbFilePath, lotName, out list_FrameName, ref LotIndex, ref rowCount, ref columnCount))
                {
                    RowCount = rowCount;
                    ColumnCount = columnCount;
                    List_Frame.Clear();
                    foreach (string frameName in list_FrameName)
                    {
                        List_Frame.Add(frameName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 选择盘号，加载数据
        /// </summary>
        private void cb_Frame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                System.Windows.Controls.ComboBox cb = sender as System.Windows.Controls.ComboBox;
                if (cb.SelectedItem == null) return;
                string frameName = cb.SelectedItem.ToString();
                List<DefectDataView> list_DefectDataView;
                if (!SQLiteOperation.GetData(DbFilePath, LotIndex, frameName, out List_InspectionDataView, out list_DefectDataView, out Dict_DefectTyoe))
                {
                    return;
                }
                List_DefectDataView.Clear();
                NGDefectCount = 0;
                N2KDefectCount = 0;
                int i = 1;
                foreach (DefectDataView defectView in list_DefectDataView)
                {
                    defectView.DisplayIndex = i;
                    defectView.DieBelongTo = string.Format("{0} - {1}", defectView.RowIndex + 1, defectView.ColumnIndex + 1);
                    if (Dict_DefectTyoe.ContainsKey(defectView.DefectTypeIndex))
                    {
                        defectView.DefectType = Dict_DefectTyoe[defectView.DefectTypeIndex];
                    }
                    else
                    {
                        defectView.DefectType = "未知缺陷";
                    }
                    defectView.DisplayErrorDetail = defectView.ErrorDetail;
                    List_DefectDataView.Add(defectView);
                    InspectionDataView dataView = List_InspectionDataView.Where(d => d.DbIndex == defectView.InspectionDataDbIndex).FirstOrDefault();
                    if (dataView == null)
                    {
                        throw new Exception("未找到缺陷所属芯片");
                    }
                    defectView.InspectionDataListIndex = List_InspectionDataView.IndexOf(dataView);
                    dataView.List_DefectDataListIndex.Add(List_DefectDataView.IndexOf(defectView));
                    dataView.List_DefectData.Add(defectView.DefectTypeIndex);
                    if (defectView.Result == 0)
                    {
                        NGDefectCount++;
                    }
                    else
                    {
                        N2KDefectCount++;
                    }

                    i++;
                }
                List<ReviewEditData> list_editdata ;
                SQLiteOperation.ReadUpdateErrCode(DbFilePath, LotIndex, frameName, out list_editdata);
                //重载mapping初始化函数 传入错误码 数据库相关信息
                Mapping.Initial(RowCount, ColumnCount, List_InspectionDataView, Dict_DefectTyoe,DbFilePath, LotIndex, frameName, list_editdata);

                HideLoadDataGrid();
                ProductCode = SelectedProductCode;
                LotName = SelectedLotName;
                FrameName = frameName;
                cb_ShowDefectTypeIndex.SelectedIndex = 0;
                CurFrame = cb_Frame.SelectedIndex;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }


        private void lv_Defect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lv_Defect.SelectedItem != null)
                {
                    lv_Defect.ScrollIntoView(lv_Defect.SelectedItem);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void lv_Defect_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lv_Defect.SelectedItem != null)
                {
                    DefectDataView defectView = lv_Defect.SelectedItem as DefectDataView;
                    Mapping.ChangeSelectedDieExternal(defectView.RowIndex, defectView.ColumnIndex);
                    SelectDefectChanged(defectView);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        public void SelectDefectChanged(DefectDataView defectView)
        {
            try
            {
                if (defectView == null) return;
                SelectedDefect = defectView;
                InspectionDataView dataView = List_InspectionDataView[defectView.InspectionDataListIndex];
                ShowDieAndDefectImage(dataView, SelectedDefect);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void MappingSelectedDieChanged(int rowIndex, int columnIndex, InspectionResult inpectionResult)
        {
            try
            {
                InspectionDataView dataView = List_InspectionDataView.Where(d => (d.RowIndex == rowIndex && d.ColumnIndex == columnIndex))
                                                           .FirstOrDefault();
                if (dataView == null) return;
                List_DieImages.Clear();
                int i = 1;
                foreach (string concatImagePath in dataView.ConcatImagePath)
                {
                    ComboBoxData cbdata = new ComboBoxData { DisplayName = "图" + i.ToString(), Tag = ImageDirectory + concatImagePath };
                    List_DieImages.Add(cbdata);
                    i++;
                }
                if (dataView.List_DefectDataListIndex.Count == 0)
                {
                    ShowDieAndDefectImage(dataView, null);
                    return;
                }
                DefectDataView defectView = List_DefectDataView[dataView.List_DefectDataListIndex[0]];
                SelectDefectChanged(defectView);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void ShowDieAndDefectImage(InspectionDataView dataView, DefectDataView defectView)
        {
            try
            {
                List_DieImages.Clear();
                int concatImageCount = dataView.ConcatImagePath.Count;
                if (concatImageCount == 0)
                {
                    System.Windows.MessageBox.Show(string.Format("{0} - {1} 芯片图像未存储", dataView.RowIndex + 1, dataView.ColumnIndex + 1)); 
                    return;
                }
                for (int i = 0; i < concatImageCount; i++)
                {
                    string concatImagePath = ImageDirectory + dataView.ConcatImagePath[i];
                    string wirePath = ImageDirectory + dataView.WirePath[i];
                    ComboBoxData cbdata = new ComboBoxData();
                    cbdata.DisplayName = "图" + (i + 1).ToString();
                    cbdata.Tag = concatImagePath + ";" + wirePath;
                    List_DieImages.Add(cbdata);

                    if (defectView != null && defectView.ConcatImageIndex == i)
                    {
                        cb_DieImages.SelectedItem = cbdata;
                        switch (defectView.ImageIndex)
                        {
                            case -1:
                                SelectedDefectImageChannel = "";
                                break;

                            //2020.12.05改为多图层方式保存图片
                            //case 0:
                            //    SelectedDefectImageChannel = "R通道";
                            //    break;
                            //case 1:
                            //    SelectedDefectImageChannel = "G通道";
                            //    break;
                            //case 2:
                            //    SelectedDefectImageChannel = "B通道";
                            //    break;
                            default:
                                SelectedDefectImageChannel = "";
                                break;
                        }

                        HObject concatImage = null;
                        HObject channelImage = null;
                        HObject concatRegion = null;
                        HObject wire = null;
                        HObject channelwire = null;
                        HOperatorSet.GenEmptyObj(out concatImage);
                        HOperatorSet.GenEmptyObj(out channelImage);
                        HOperatorSet.GenEmptyObj(out concatRegion);
                        HOperatorSet.GenEmptyObj(out wire);
                        HOperatorSet.GenEmptyObj(out channelwire);
                        if (!File.Exists(concatImagePath))
                        {
                            System.Windows.MessageBox.Show(string.Format("{0} - {1} 图像文件不存在 {2}", dataView.RowIndex + 1, dataView.ColumnIndex + 1, concatImagePath));
                            break;
                        }
                        HOperatorSet.ReadImage(out concatImage, concatImagePath);

                        HTuple width, height;
                        HOperatorSet.GetImageSize(concatImage, out width, out height);
                        if (hWindow.Image != null)
                        {
                            hWindow.Image.Dispose();
                        }
                        
                        //2020.12.05提取对应图层的图片进行显示
                        if (defectView.ImageIndex >= 1)
                        {
                            HOperatorSet.SelectObj(concatImage, out channelImage, defectView.ImageIndex);
                            hWindow.SetImage(channelImage);
                            HOperatorSet.SetPart(hWindow.HTWindow.HalconWindow, 0, 0, height.TupleSelect(0).I - 1, width.TupleSelect(0).I - 1);
                            HOperatorSet.DispObj(channelImage, hWindow.HTWindow.HalconWindow);
                            channelImage.Dispose();
                        }
                        concatImage.Dispose();
                       

                        string concatRegionPath = ImageDirectory + dataView.ConcatRegionPath[defectView.ConcatRegionIndex];
                        if (!File.Exists(concatRegionPath))
                        {
                            System.Windows.MessageBox.Show(string.Format("{0} - {1} 区域文件不存在 {2}", dataView.RowIndex + 1, dataView.ColumnIndex + 1, concatRegionPath));
                            break;
                        }
                        HOperatorSet.ReadRegion(out concatRegion, concatRegionPath);

                        //2020.12.05提取对应图层的defect region进行显示
                        if (defectView.RegionIndex >= 1)
                        {
                            HObject region = concatRegion.SelectObj(defectView.RegionIndex);

                            if (hWindow.Region != null)
                            {
                                hWindow.Region.Dispose();
                            }
                            if (region != null)
                            {
                                hWindow.Region = region.CopyObj(1, -1);
                            }
                            HOperatorSet.SetDraw(hWindow.HTWindow.HalconWindow, "margin");
                            HOperatorSet.SetColor(hWindow.HTWindow.HalconWindow, "yellow");
                            HOperatorSet.DispRegion(region, hWindow.HTWindow.HalconWindow);
                            region.Dispose();
                        }
                        concatRegion.Dispose();
                        

                        if (!File.Exists(wirePath))
                        {
                            System.Windows.MessageBox.Show(string.Format("{0} - {1} 金线文件不存在 {2}", dataView.RowIndex + 1, dataView.ColumnIndex + 1, wirePath));
                            break;
                        }
                        //HTuple tempTuple = null;
                        //HOperatorSet.ReadContourXldDxf(out wire, wirePath, new HTuple(), new HTuple(), out tempTuple);
                        HOperatorSet.ReadRegion(out wire, wirePath);

                        //2020.12.05提取对应图层的wire region进行显示
                        if (defectView.ImageIndex >= 1)
                        {
                            int iwire = wire.CountObj();
                            HOperatorSet.SelectObj(wire, out channelwire, defectView.ImageIndex);
                            //HOperatorSet.SetColor(hWindow.HTWindow.HalconWindow, "green");
                            //HOperatorSet.DispObj(channelwire, hWindow.HTWindow.HalconWindow);
                            if (channelwire != null)
                            {
                                hWindow.Region = channelwire.CopyObj(1, -1);
                            }

                            HOperatorSet.SetDraw(hWindow.HTWindow.HalconWindow, "margin");
                            HOperatorSet.SetColor(hWindow.HTWindow.HalconWindow, "green");
                            HOperatorSet.DispRegion(channelwire, hWindow.HTWindow.HalconWindow);

                            channelwire.Dispose();
                        }
                        wire.Dispose();
                        
                        string color = defectView.Result == 0 ? "red" : "orange";

                        HalconOperation.DisplayMessage(hWindow.HTWindow.HalconWindow, string.Format("{0} - {1} 缺陷 {2}", dataView.RowIndex + 1, dataView.ColumnIndex + 1, defectView.DefectTypeIndex),
                                                       "window", 32, 12, color, "true");
                  
                    }             
                }
                if (defectView == null)
                {
                    cb_DieImages.SelectedItem = List_DieImages[0];
                    HObject concatImage = null;
                    HObject wire = null;
                    HOperatorSet.GenEmptyObj(out concatImage);
                    HOperatorSet.GenEmptyObj(out wire);
                    string concatImagePath = ImageDirectory + dataView.ConcatImagePath[0];
                    if (!File.Exists(concatImagePath))
                    {
                        System.Windows.MessageBox.Show(string.Format("{0} - {1} 图像文件不存在 {2}", dataView.RowIndex + 1, dataView.ColumnIndex + 1, concatImagePath));
                        return;
                    }
                    HOperatorSet.ReadImage(out concatImage, concatImagePath);
                    HTuple width, height;
                    HOperatorSet.GetImageSize(concatImage, out width, out height);
                    if (hWindow.Image != null)
                    {
                        hWindow.Image.Dispose();
                    }
                   

                    //2020.12.05提取对应图层的图片进行显示
                    if (concatImage.CountObj() >= 1)
                    {
                        HObject channelImage = concatImage.SelectObj(1);
                        hWindow.SetImage(channelImage);
                        HOperatorSet.SetPart(hWindow.HTWindow.HalconWindow, 0, 0, height.TupleSelect(0).D - 1, width.TupleSelect(0).D - 1);
                        HOperatorSet.DispObj(channelImage, hWindow.HTWindow.HalconWindow);
                        channelImage.Dispose();
                    }
                    concatImage.Dispose();

                    if (hWindow.Region != null)
                    {
                        hWindow.Region.Dispose();
                    }
                    string wirePath = ImageDirectory + dataView.WirePath[0];
                    if (!File.Exists(wirePath))
                    {
                        System.Windows.MessageBox.Show(string.Format("{0} - {1} 金线文件不存在 {2}", dataView.RowIndex + 1, dataView.ColumnIndex + 1, wirePath));
                        return;
                    }
                    //HTuple tempTuple = null;
                    HOperatorSet.ReadRegion(out wire, wirePath);
                    //HOperatorSet.ReadContourXldDxf(out wire, wirePath, new HTuple(), new HTuple(), out tempTuple);
                    HOperatorSet.SetColor(hWindow.HTWindow.HalconWindow, "green");

                    //2020.12.05提取对应图层的wire region进行显示
                    if (wire.CountObj() >= 1)
                    {
                        HObject channelwire = wire.SelectObj(1);
                        //HOperatorSet.DispObj(channelwire, hWindow.HTWindow.HalconWindow);
                        HOperatorSet.SetDraw(hWindow.HTWindow.HalconWindow, "margin");
                        HOperatorSet.SetColor(hWindow.HTWindow.HalconWindow, "green");
                        HOperatorSet.DispRegion(channelwire, hWindow.HTWindow.HalconWindow);

                        channelwire.Dispose();
                    }
                    wire.Dispose();

                    HalconOperation.DisplayMessage(hWindow.HTWindow.HalconWindow, string.Format("{0} - {1} {2}", dataView.RowIndex + 1, dataView.ColumnIndex + 1, InspectionResultsConverter.ToString(dataView.InspectionResult)),
                                                   "window", 12, 12, InspectionResultsConverter.ToColor(dataView.InspectionResult), "true");
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void MappingDockPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Mapping.Height = MappingDockPanel.ActualHeight;
            Mapping.Width = MappingDockPanel.ActualWidth;
            Mapping.Refresh();
        }

        private void btn_ToNextDefect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedDefect == null)
                {
                    if (List_DefectDataView.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        Mapping.ChangeSelectedDieExternal(List_DefectDataView[0].RowIndex, List_DefectDataView[0].ColumnIndex);
                        SelectDefectChanged(List_DefectDataView[0]);
                    }
                }
                else
                {
                    int index = List_DefectDataView.IndexOf(SelectedDefect);
                    if (index + 1 == List_DefectDataView.Count)
                    {
                        return;
                    }
                    else
                    {
                        Mapping.ChangeSelectedDieExternal(List_DefectDataView[index + 1].RowIndex, List_DefectDataView[index + 1].ColumnIndex);
                        SelectDefectChanged(List_DefectDataView[index + 1]);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_ToLastDefect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedDefect == null)
                {
                    return;
                }
                else
                {
                    int index = List_DefectDataView.IndexOf(SelectedDefect);
                    if (index - 1 < 0)
                    {
                        return;
                    }
                    else
                    {
                        Mapping.ChangeSelectedDieExternal(List_DefectDataView[index - 1].RowIndex, List_DefectDataView[index - 1].ColumnIndex);
                        SelectDefectChanged(List_DefectDataView[index - 1]);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 复看合格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetN2K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedDefect == null) return;
                if (SelectedDefect.Result == 1)
                {
                    System.Windows.MessageBox.Show("该缺陷已经复看合格");
                    return;
                }
                if (System.Windows.MessageBox.Show(string.Format("确认将芯片 {0} 的缺陷 {1} 设置为复看合格？", SelectedDefect.DieBelongTo, SelectedDefect.DefectType),
                                                   "将缺陷设置为复看合格",
                                                   System.Windows.MessageBoxButton.OKCancel)
                               != MessageBoxResult.OK)
                {
                    return;
                }
                SelectedDefect.Result = 1;
                NGDefectCount--;
                N2KDefectCount++;
                InspectionDataView dataView = List_InspectionDataView[SelectedDefect.InspectionDataListIndex];
                bool IsAllDefectN2K = true;
                foreach (int defectListIndex in dataView.List_DefectDataListIndex)
                {
                    if (List_DefectDataView[defectListIndex].Result == 0)
                    {
                        IsAllDefectN2K = false;
                    }
                }
                if (IsAllDefectN2K)
                {
                    dataView.InspectionResult = InspectionResult.N2K;
                    Mapping.SetDieState(dataView.RowIndex, dataView.ColumnIndex, dataView.InspectionResult);
                    SQLiteOperation.UpdateDefect(DbFilePath, LotIndex, SelectedDefect, dataView);
                    System.Windows.MessageBox.Show(string.Format("芯片 {0} 的缺陷 {1} 已被设置为复看合格", SelectedDefect.DieBelongTo, SelectedDefect.DefectType)
                                                   + System.Environment.NewLine
                                                   + string.Format("芯片 {0} 已被设置为复看合格", SelectedDefect.DieBelongTo));
                }
                else
                {
                    System.Windows.MessageBox.Show(string.Format("芯片 {0} 的缺陷 {1} 已被设置为复看合格", SelectedDefect.DieBelongTo, SelectedDefect.DefectType));
                    SQLiteOperation.UpdateDefect(DbFilePath, LotIndex, SelectedDefect);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 一键复看合格
        /// </summary>
        private void btn_SetAllN2K_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (SelectedDefect == null) return;

                List<DefectDataView> List_TDataView = new List<DefectDataView>();
                foreach(DefectDataView TDataView in List_DefectDataView)
                {
                    if(TDataView.ColumnIndex== SelectedDefect.ColumnIndex && TDataView.RowIndex == SelectedDefect.RowIndex)
                    {
                        List_TDataView.Add(TDataView);
                    }
                }
                int oknum = 0;
                
                foreach (var TDataView in List_TDataView)
                {
                   
                    if (TDataView.Result == 1)
                    {
                        oknum++;
                        if(oknum== List_TDataView.Count)
                        {
                            System.Windows.MessageBox.Show("该芯片已经复看合格");
                            return;
                        }
                    }
                    else
                    {
                        TDataView.Result = 1;
                        NGDefectCount--;
                        N2KDefectCount++;
                    }

                }
                InspectionDataView dataView = List_InspectionDataView[SelectedDefect.InspectionDataListIndex];
                dataView.InspectionResult = InspectionResult.N2K;
                Mapping.SetDieState(dataView.RowIndex, dataView.ColumnIndex, dataView.InspectionResult);
                SQLiteOperation.UpdateDefect(DbFilePath, LotIndex, SelectedDefect, dataView);
                System.Windows.MessageBox.Show(string.Format("芯片 {0} 的缺陷 {1} 已被设置为复看合格", SelectedDefect.DieBelongTo, SelectedDefect.DefectType)
                                               + System.Environment.NewLine
                                               + string.Format("芯片 {0} 已被设置为复看合格", SelectedDefect.DieBelongTo));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }


        }
        /// <summary>
        /// 复看不合格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetNG_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedDefect == null) return;
                if (SelectedDefect.Result == 0)
                {
                    System.Windows.MessageBox.Show("该缺陷已经不合格");
                    return;
                }
                if (System.Windows.MessageBox.Show(string.Format("确认将芯片 {0} 的缺陷 {1} 设置为不合格？", SelectedDefect.DieBelongTo, SelectedDefect.DefectType),
                                                                 "将缺陷设置为不合格",
                                                                 MessageBoxButton.OKCancel)
                               != MessageBoxResult.OK)
                {
                    return;
                }
                SelectedDefect.Result = 0;
                NGDefectCount++;
                N2KDefectCount--;
                InspectionDataView dataView = List_InspectionDataView[SelectedDefect.InspectionDataListIndex];
                if (dataView.InspectionResult == InspectionResult.N2K)
                {
                    dataView.InspectionResult = InspectionResult.NG;
                    Mapping.SetDieState(dataView.RowIndex, dataView.ColumnIndex, dataView.InspectionResult);
                    SQLiteOperation.UpdateDefect(DbFilePath, LotIndex, SelectedDefect, dataView);
                    System.Windows.MessageBox.Show(string.Format("芯片 {0} 的缺陷 {1} 已被设置为不合格", SelectedDefect.DieBelongTo, SelectedDefect.DefectType)
                                                   + System.Environment.NewLine
                                                   + string.Format("芯片 {0} 已被设置为不合格", SelectedDefect.DieBelongTo));
                }
                else
                {
                    System.Windows.MessageBox.Show(string.Format("芯片 {0} 的缺陷 {1} 已被设置为不合格", SelectedDefect.DieBelongTo, SelectedDefect.DefectType));
                    SQLiteOperation.UpdateDefect(DbFilePath, LotIndex, SelectedDefect);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void cb_DieImages_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cb_DieImages.IsDropDownOpen == true)
                {
                    if (cb_DieImages.SelectedItem == null) return;
                    SelectedDefect = null;
                    ComboBoxData cbdata = cb_DieImages.SelectedItem as ComboBoxData;
                    string[] tagString = cbdata.Tag.Split(';');
                    string concatImagePath = tagString[0];
                    string wirePath = tagString[1];
                    if (!File.Exists(concatImagePath))
                    {
                        System.Windows.MessageBox.Show(string.Format("图像文件不存在 {0}", concatImagePath));
                        return;
                    }
                    HObject concatImage = null;
                    HOperatorSet.GenEmptyObj(out concatImage);
                    HOperatorSet.ReadImage(out concatImage, concatImagePath);
                    HTuple width, height;
                    HOperatorSet.GetImageSize(concatImage, out width, out height);
                    HOperatorSet.SetPart(hWindow.HTWindow.HalconWindow, 0, 0, height - 1, width - 1);
                    HOperatorSet.DispObj(concatImage, hWindow.HTWindow.HalconWindow);
                    concatImage.Dispose();

                    if (!File.Exists(wirePath))
                    {
                        System.Windows.MessageBox.Show(string.Format("金线文件不存在 {0}", wirePath));
                        return;
                    }
                    HObject wire = null;
                    HOperatorSet.GenEmptyObj(out wire);
                    HTuple tempTuple = null;
                    HOperatorSet.ReadContourXldDxf(out wire, wirePath, new HTuple(), new HTuple(), out tempTuple);
                    HOperatorSet.SetColor(hWindow.HTWindow.HalconWindow, "green");
                    HOperatorSet.DispObj(wire, hWindow.HTWindow.HalconWindow);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Reload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(DbDirectory))
                {
                    System.Windows.MessageBox.Show("文件夹不存在");
                    return;
                }
                GetProductDb(DbDirectory);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
       
        
        /// <summary>
        /// 生成报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cb_Product_Report.SelectedItem == null || cb_Lot_Report.SelectedItem == null)
                {
                    return;
                }

                btn_CaccelReport.Content = "取  消";
                rtb_ReprotProgress.Document.Blocks.Clear();
                panel_GenerateReport.Visibility = Visibility.Collapsed;
                panel_ReprotProgress.Visibility = Visibility.Visible;
                ReportThread = new Thread(() =>
                {
                    try
                    {
                        Report.GenerateExcel(DbFilePath_Report, LotIndex_Report, ImageDirectory, ExcelDirectory, ReportModel, IsExcelChinese);
                        btn_CaccelReport.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate ()
                        {
                            btn_CaccelReport.Content = "返  回";
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate ()
                        {
                            System.Windows.MessageBox.Show(ex.ToString());
                        });
                    }
                });
                ReportThread.Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_CaccelReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportThread != null)
                ReportThread.Abort();
            panel_GenerateReport.Visibility = Visibility.Visible;
            panel_ReprotProgress.Visibility = Visibility.Collapsed;

        }

        Thread ReportThread;

        private void Report_OnShowMessage(string message)
        {
            rtb_ReprotProgress.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate ()
            {
                Paragraph paragraph = new Paragraph(new Run(message));
                rtb_ReprotProgress.Document.Blocks.Add(paragraph);
                rtb_ReprotProgress.ScrollToEnd();
            });
        }

        private string DbFilePath_Report;
        private string SelectedProductCode_Report;
        private string SelectedLotName_Report;
        private int LotIndex_Report;

        private void cb_Product_Report_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                System.Windows.Controls.ComboBox cb = sender as System.Windows.Controls.ComboBox;
                if (cb.SelectedItem == null) return;
                ComboBoxData data = cb.SelectedItem as ComboBoxData;
                this.SelectedProductCode_Report = data.DisplayName;
                this.DbFilePath_Report = data.Tag;
                List<string> list_LotName;
                if (SQLiteOperation.GetLot(DbFilePath_Report, out list_LotName))
                {
                    List_Lot.Clear();
                    List_Frame.Clear();
                    foreach (string lotName in list_LotName)
                    {
                        List_Lot.Add(lotName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 2019.8.9 武汉新增二维码模式 默认状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_Report_Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cb_Report_Model.SelectedItem == null) return;
                if (cb_Report_Model.SelectedIndex == 0)
                {
                    ReportModel = 0;
                }
                if (cb_Report_Model.SelectedIndex == 1)
                {
                    ReportModel = 1;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }

        }


        private void cb_Lot_Report_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                System.Windows.Controls.ComboBox cb = sender as System.Windows.Controls.ComboBox;
                if (cb.SelectedItem == null) return;
                string lotName = cb.SelectedItem.ToString();
                SelectedLotName_Report = lotName;
                List<string> list_FrameName;
                int rowCount = 0;
                int columnCount = 0;
                if (SQLiteOperation.GetFrame(DbFilePath_Report, lotName, out list_FrameName, ref LotIndex_Report, ref rowCount, ref columnCount))
                {
                    List_Frame.Clear();
                    foreach (string frameName in list_FrameName)
                    {
                        List_Frame.Add(frameName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void cb_ShowDefectTypeIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cb_ShowDefectTypeIndex.SelectedItem == null) return;
                Mapping.ShowDefectTypeIndex(cb_ShowDefectTypeIndex.SelectedIndex == 1);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_RefreshMapping_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mapping.Refresh();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void ShowSingleChannel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedDefect == null) return;
                if (SelectedDefect.ImageIndex == -1)
                {
                    System.Windows.MessageBox.Show("该图片为黑白图");
                }
                InspectionDataView dataView = List_InspectionDataView[SelectedDefect.InspectionDataListIndex];
                string concatImagePath = ImageDirectory + dataView.ConcatImagePath[SelectedDefect.ConcatImageIndex];
                HObject image = null;
                HOperatorSet.GenEmptyObj(out image);
                HOperatorSet.ReadImage(out image, concatImagePath);
                HObject imageR = null;
                HObject imageG = null;
                HObject imageB = null;
                HOperatorSet.GenEmptyObj(out imageR);
                HOperatorSet.GenEmptyObj(out imageG);
                HOperatorSet.GenEmptyObj(out imageB);
                HOperatorSet.Decompose3(image, out imageR, out imageG, out imageB);
                switch (SelectedDefect.ImageIndex)
                {
                    case 0:
                        image = imageR;
                        break;
                    case 1:
                        image = imageG;
                        break;
                    case 2:
                        image = imageB;
                        break;
                    default:return;
                }
                HTuple width, height;
                HOperatorSet.GetImageSize(image, out width, out height);
                HOperatorSet.SetPart(hWindow.HTWindow.HalconWindow, 0, 0, height - 1, width - 1);
                HOperatorSet.DispObj(image, hWindow.HTWindow.HalconWindow);
                image.Dispose();
                imageR.Dispose();
                imageG.Dispose();
                imageB.Dispose();

                string concatRegionPath = ImageDirectory + dataView.ConcatRegionPath[SelectedDefect.ConcatRegionIndex];
                if (!File.Exists(concatRegionPath))
                {
                    System.Windows.MessageBox.Show(string.Format("{0} - {1} 区域文件不存在 {2}", dataView.RowIndex + 1, dataView.ColumnIndex + 1, concatRegionPath));
                    return;
                }
                HObject concatRegion;
                HOperatorSet.GenEmptyObj(out concatRegion);
                HOperatorSet.ReadRegion(out concatRegion, concatRegionPath);
                HObject region = concatRegion.SelectObj(SelectedDefect.RegionIndex);
                HOperatorSet.SetDraw(hWindow.HTWindow.HalconWindow, "margin");
                HOperatorSet.SetColor(hWindow.HTWindow.HalconWindow, "yellow");
                HOperatorSet.DispRegion(region, hWindow.HTWindow.HalconWindow);
                region.Dispose();
                concatRegion.Dispose();
                HalconOperation.DisplayMessage(hWindow.HTWindow.HalconWindow, string.Format("{0} - {1} {2} 缺陷{3}", dataView.RowIndex + 1, dataView.ColumnIndex + 1, SelectedDefectImageChannel, SelectedDefect.DefectTypeIndex),
                                          "window", 12, 12, InspectionResultsConverter.ToColor(dataView.InspectionResult), "true");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }


        public  void Show( DataShow dataShow)
        {
            try
            {
                DbFilePath = dataShow.DbFilePath;
                LotIndex = dataShow.LotIndex;
                
                _ImageDirectory = dataShow.imageDirectory;
                List<DefectDataView> list_DefectDataView;
                if (!SQLiteOperation.GetData(dataShow.DbFilePath, dataShow.LotIndex, dataShow.frameName, out List_InspectionDataView, out list_DefectDataView, out Dict_DefectTyoe))
                {
                    return;
                }
                List_DefectDataView.Clear();
                NGDefectCount = 0;
                N2KDefectCount = 0;
                int i = 1;
                foreach (DefectDataView defectView in list_DefectDataView)
                {
                    defectView.DisplayIndex = i;
                    defectView.DieBelongTo = string.Format("{0} - {1}", defectView.RowIndex + 1, defectView.ColumnIndex + 1);
                    defectView.DefectType = Dict_DefectTyoe[defectView.DefectTypeIndex];
                    defectView.DisplayErrorDetail = defectView.ErrorDetail;
                    List_DefectDataView.Add(defectView);
                    InspectionDataView dataView = List_InspectionDataView.Where(d => d.DbIndex == defectView.InspectionDataDbIndex).FirstOrDefault();
                    if (dataView == null)
                    {
                        throw new Exception("未找到缺陷所属芯片");
                    }
                    defectView.InspectionDataListIndex = List_InspectionDataView.IndexOf(dataView);
                    dataView.List_DefectDataListIndex.Add(List_DefectDataView.IndexOf(defectView));
                    dataView.List_DefectData.Add(defectView.DefectTypeIndex);
                    if (defectView.Result == 0)
                    {
                        NGDefectCount++;
                    }
                    else
                    {
                        N2KDefectCount++;
                    }

                    i++;
                }
                List<ReviewEditData> list_editdata = new List<ReviewEditData>();
                SQLiteOperation.ReadUpdateErrCode(DbFilePath, LotIndex, dataShow.frameName, out list_editdata);
                //重载mapping初始化函数 传入错误码 数据库相关信息
                Mapping.Initial(dataShow.rowCount, dataShow.columnCount, List_InspectionDataView, Dict_DefectTyoe, DbFilePath, LotIndex, dataShow.frameName, list_editdata);
                HideLoadDataGrid();
                ProductCode = dataShow.product;
                LotName = dataShow.lotName;
                FrameName = dataShow.frameName;
                cb_ShowDefectTypeIndex.SelectedIndex = 0;
                CurFrame = cb_Frame.SelectedIndex;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }


      
    }

    class ComboBoxData
    {
        public string DisplayName { get; set; }
        public string Tag { get; set; }
    }
}
