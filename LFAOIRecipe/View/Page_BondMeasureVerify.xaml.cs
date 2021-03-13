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
    /// Page_BondMeasureVerify.xaml 的交互逻辑
    /// </summary>
    internal partial class Page_BondMeasureVerify : UserControl
    {
        public Page_BondMeasureVerify()
        {
            InitializeComponent();
        }

        public void Close()
        {
            dockPanel.Children.Clear();
        }


        private void ComboBox_SelectionChanged_SelectMethod(object sender, SelectionChangedEventArgs e)
        {

            //判断该类焊点区域需不需要检测，若不需要直接将检测项目置为“false”
            if (DataContext  == null) return;

            if ((DataContext as BondMeasureVerify).PadUserRegions.Count == 0) return;

            //UserRegion userRegion = (DataContext as BondMeasureVerify).SelectedPadRegion;

            //if (userRegion == null) return;
            foreach (var item in (DataContext as BondMeasureVerify).PadUserRegions)
            {
                if (item == null) break;
                //add by wj 2021-0107
                item.AlgoParameterIndex = SelectBondMethod.SelectedIndex;
            }

        }





    }
}
