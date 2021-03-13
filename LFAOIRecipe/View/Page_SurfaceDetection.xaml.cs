using System.Windows;
using System.Windows.Controls;

namespace LFAOIRecipe
{
    internal partial class Page_SurfaceDetection : UserControl
    {
        public Page_SurfaceDetection()
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
            DockPanel.Children.Clear();
        }
    }
}
