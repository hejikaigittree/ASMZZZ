using System;
using System.Collections.Generic;
using System.Linq;
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

namespace LFAOIRecipe
{
    /// <summary>
    /// CreateBondMeasureModel.xaml 的交互逻辑
    /// </summary>
    internal partial class Page_CreateAutoBondMeasureModel : UserControl
    {
        public Page_CreateAutoBondMeasureModel()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.SelectAll();
        }

        public void Close()
        {
            dockPanel.Children.Clear();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext != null)
            {
                if ((DataContext as CreateAutoBondMeasureModel).Bond1AutoRegionsParameter.IsCircleShape == true)
                {
                    this.DataGrid.Columns[5].Visibility = Visibility.Hidden;
                }
                if ((DataContext as CreateAutoBondMeasureModel).Bond1AutoRegionsParameter.IsCircleShape == false)
                {
                    this.DataGrid.Columns[5].Visibility = Visibility.Visible;
                }
            }

        }
    }
}
