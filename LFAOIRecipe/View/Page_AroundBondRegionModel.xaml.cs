using HalconDotNet;
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
    /// Page_AroundBondRegionModel.xaml 的交互逻辑
    /// </summary>
    public partial class Page_AroundBondRegionModel : UserControl
    {
        private HTHalControlWPF htWindow;

        public Page_AroundBondRegionModel()
        {
            InitializeComponent();
        }
        public void Close()
        {
            DockPanel.Children.Clear();
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserRegion userRegion = (DataContext as CreateAroundBondRegionModel).SelectedUserRegion;

            if (userRegion == null) return;

            int image3index;
            image3index = (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.ShiftImageIndex;
            int image2index;
            image2index = (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.AroundBondLineGaussAlgoPara.TailImageIndex;
            int image0index;
            image0index = (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.AroundBondAdativeAlgoPara.SurfImageIndex;
            int image1index;
            image1index = (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.AroundBondGlobalAlgoPara.SurfImageIndex;

            (DataContext as CreateAroundBondRegionModel).ImageIndex0 = image0index;
            (DataContext as CreateAroundBondRegionModel).ImageIndex1 = image1index;
            (DataContext as CreateAroundBondRegionModel).ImageIndex2 = image2index;
            (DataContext as CreateAroundBondRegionModel).ImageIndex3 = image3index;
        }

        private void ComboBox_SelectionChanged_IsInspect(object sender, SelectionChangedEventArgs e)
        {
            //判断该类焊点区域需不需要检测，若不需要直接将检测项目置为“false”
            UserRegion userRegion = (DataContext as CreateAroundBondRegionModel).SelectedUserRegion;

            if ( userRegion == null) return;

            int IsInspect = (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.IsAroundBondRegInspect;
            if(IsInspect == 0)
            {
                (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.IsBallShiftInspect = false;
                (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.IsTailInspect = false;
                (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.IsSurfaceInspect = false;
            }else
            {
                (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.IsBallShiftInspect = true;
                (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.IsTailInspect = true;
                (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.IsSurfaceInspect = true;
            }
        }

        /// <summary>
        /// 联动选择焊盘测量类型，相应的改变测量类型的测量大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged_Pad(object sender, SelectionChangedEventArgs e)
        {
            UserRegion userRegion = (DataContext as CreateAroundBondRegionModel).SelectedUserRegion;

            if (userRegion == null) return;

            (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.PadItemSelectIndex = SelectMeasureType_Pad.SelectedIndex;
            //判断选择的项目索引号后需要保存的测量
            if (SelectMeasureType_Pad.SelectedIndex == 1)
            {
                (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.PadIsCircle = true;
            }
            else
            {
                (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.PadIsCircle = false;
            }
        }
        /// <summary>
        /// 联动选择焊点测量类型，相应的改变测量类型的测量大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged_Ball(object sender, SelectionChangedEventArgs e)
        {
            UserRegion userRegion = (DataContext as CreateAroundBondRegionModel).SelectedUserRegion;

            if (userRegion == null) return;

            (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.BallItemSelectIndex = SelectMeasureType_Ball.SelectedIndex;
            //判断选择的项目索引号后需要保存的测量
            if (SelectMeasureType_Ball.SelectedIndex == 0)
            {
                (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.BallIsCircle = true;
            }
            else
            {
                (DataContext as CreateAroundBondRegionModel).SelectedUserRegion.AroundBondRegionWithPara.AroundBallMeasureAlgoPara.BallIsCircle = false;
            }

        }

    }
}
