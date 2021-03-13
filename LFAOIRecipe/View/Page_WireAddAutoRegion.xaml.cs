using HalconDotNet;
using System.IO;
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
    /// CreateBond2Model.xaml 的交互逻辑
    /// </summary>
    internal partial class Page_WireAddAutoRegion : UserControl
    {
        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;

        private string ReferenceDirectory => $"{ProductDirctory}\\Recipe\\Reference\\";

        public WireParameter WireParameter { get; set; }
        public WireAddAutoRegion wireAddAutoRegion { get; private set; }

        public Page_WireAddAutoRegion()
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
        //是否启用起始虚拟焊点联动
        private void ComboBox_SelectionChanged_Start(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext != null)
            {
                (DataContext as WireAddAutoRegion).listboxstart = listboxstart;
            }
            else
            {
                return;
            }

            //判断是否已经加载了XML
            if (WireRecipe.IsLoadXML) return;

            if (IsUseStartVirtualBond.SelectedIndex == 0)
            {
                //不使用虚拟焊点
                HTuple filesBondMatch = new HTuple();
                HTuple filesBondMeasure = new HTuple();
                //
                filesBondMatch = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                            "BondMatch*.*", SearchOption.TopDirectoryOnly);
                filesBondMeasure = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                           "BondMeasure*.*", SearchOption.TopDirectoryOnly);

                string[] filesTo = filesBondMatch.TupleConcat(filesBondMeasure);
                string[] StartBondonRecipes = new string[filesTo.Length];

                ///////////////////////////////////添加OnRecipes////////////////////////////////////
                //还需要对EndBondOnRecipes赋值

                (DataContext as WireAddAutoRegion).StartBondOnRecipes.Clear();

                for (int i = 0; i < filesTo.Length; i++)
                {
                    StartBondonRecipes[i] = System.IO.Path.GetFileName(filesTo[i]);
                    (DataContext as WireAddAutoRegion).StartBondOnRecipes.Add(new OnRecipe() { Name = StartBondonRecipes[i], IsSelected = false });
                }

                //to be added
                //(DataContext as WireAddAutoRegion).WireParameter.StartBondonRecipesIndexs

                //使用自动生成的真实起始焊点
                WireAddAutoRegion.IsAutoLoadStartBallReg = true;


            }
            else if (IsUseStartVirtualBond.SelectedIndex == 1)
            {
                //使用虚拟焊点
                HTuple filesFrame = new HTuple();
                HTuple filesIC = new HTuple();
                filesFrame = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                            "Frame*.*", SearchOption.TopDirectoryOnly);
                filesIC = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                           "IC*.*", SearchOption.TopDirectoryOnly);

                //
                string[] filesOn = filesFrame.TupleConcat(filesIC);
                string[] StartBondonRecipes = new string[filesOn.Length];

                ///////////////////////////////////添加OnRecipes////////////////////////////////////
                //还需要对EndBondOnRecipes赋值

                (DataContext as WireAddAutoRegion).StartBondOnRecipes.Clear();

                for (int i = 0; i < filesOn.Length; i++)
                {
                    StartBondonRecipes[i] = System.IO.Path.GetFileName(filesOn[i]);
                    (DataContext as WireAddAutoRegion).StartBondOnRecipes.Add(new OnRecipe() { Name = StartBondonRecipes[i], IsSelected = false, IsSelected_pre = false, Selected_ind = 0 });
                }
                //(DataContext as WireAddAutoRegion).WireParameter.StartBondonRecipes = StartBondonRecipes;

                WireAddAutoRegion.IsAutoLoadStartBallReg = false;
            }

                
        }
        //是否启用结束虚拟焊点联动
        private void ComboBox_SelectionChanged_Stop(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext != null)
            {
                (DataContext as WireAddAutoRegion).listboxstart = listboxstart;
            }
            else
            {
                return;
            }

            if (WireRecipe.IsLoadXML) return;

            if (IsUseEndVirtualBond.SelectedIndex == 0)
            {
                //不使用虚拟焊点
                HTuple filesBondMatch = new HTuple();
                HTuple filesBondMeasure = new HTuple();
                //
                filesBondMatch = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                            "BondMatch*.*", SearchOption.TopDirectoryOnly);
                filesBondMeasure = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                           "BondMeasure*.*", SearchOption.TopDirectoryOnly);

                string[] filesTo = filesBondMatch.TupleConcat(filesBondMeasure);
                string[] StopBondonRecipes = new string[filesTo.Length];

                ///////////////////////////////////添加OnRecipes////////////////////////////////////
                //还需要对EndBondOnRecipes赋值

                (DataContext as WireAddAutoRegion).EndBondOnRecipes.Clear();

                for (int i = 0; i < filesTo.Length; i++)
                {
                    StopBondonRecipes[i] = System.IO.Path.GetFileName(filesTo[i]);
                    (DataContext as WireAddAutoRegion).EndBondOnRecipes.Add(new OnRecipe() { Name = StopBondonRecipes[i], IsSelected = false, IsSelected_pre = false, Selected_ind = 0 });
                }
               // (DataContext as WireAddAutoRegion).WireParameter.StopBondonRecipes = StopBondonRecipes;

                WireAddAutoRegion.IsAutoLoadStopBallReg = true;
            }
            else if (IsUseEndVirtualBond.SelectedIndex == 1)
            {
                //使用虚拟焊点
                HTuple filesFrame = new HTuple();
                HTuple filesIC = new HTuple();
                filesFrame = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                            "Frame*.*", SearchOption.TopDirectoryOnly);
                filesIC = Directory.GetDirectories((Directory.GetParent(Directory.GetParent(ReferenceDirectory).ToString())).ToString(),
                           "IC*.*", SearchOption.TopDirectoryOnly);

                //
                string[] filesOn = filesFrame.TupleConcat(filesIC);
                string[] StopBondonRecipes = new string[filesOn.Length];

                ///////////////////////////////////添加OnRecipes////////////////////////////////////
                //还需要对EndBondOnRecipes赋值

                (DataContext as WireAddAutoRegion).EndBondOnRecipes.Clear();

                for (int i = 0; i < filesOn.Length; i++)
                {
                    StopBondonRecipes[i] = System.IO.Path.GetFileName(filesOn[i]);
                    (DataContext as WireAddAutoRegion).EndBondOnRecipes.Add(new OnRecipe() { Name = StopBondonRecipes[i], IsSelected = false, IsSelected_pre = false, Selected_ind = 0 });
                }
                //(DataContext as WireAddAutoRegion).WireParameter.StopBondonRecipes = StopBondonRecipes;

                //不自动生成结束焊点区域，需要手动画
                WireAddAutoRegion.IsAutoLoadStopBallReg = false;
            }



        }
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            int a, b;
            a = (DataContext as WireAddAutoRegion).SelectedStartRegion.Index;

            if ((DataContext as WireAddAutoRegion).SelectedStartRegion == null)
            {
                MessageBox.Show("请确认区域被选中");
                return;
            }

            if ((sender as ComboBox).SelectedIndex < 0)
            {
                MessageBox.Show("请确认有构建好的模板线");
                return;
            }
            b = (sender as ComboBox).SelectedIndex + 1;
            (DataContext as WireAddAutoRegion).WireParameter.WireRegModelType[a - 1] = b;

            (DataContext as WireAddAutoRegion).StartBallAutoUserRegion.ElementAt(a - 1).CurrentModelGroup = ((sender as ComboBox).SelectedItem as WireAutoRegionGroup);


        }

    }
}
