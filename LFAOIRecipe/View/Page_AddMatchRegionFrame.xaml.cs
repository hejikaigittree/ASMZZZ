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
    /// Page_MatchRegion.xaml 的交互逻辑
    /// </summary>
    internal partial class Page_AddMatchRegionFrame : UserControl
    {
        public Page_AddMatchRegionFrame()
        {
            InitializeComponent();
        }

        public void Close()
        {
            dockPanel.Children.Clear();
        }
    }
}
