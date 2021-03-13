using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LFAOIRecipe
{
    /// <summary>
    /// Window_AddNewComponent.xaml 的交互逻辑
    /// </summary>
    internal partial class Window_AddNewRecipe : Window
    {
        public Type ComponentType { get; private set; }

        public bool IsLoadXML { get; private set; }

        public string XmlPath { get; private set; }

        public int FrameIndex { get; private set; }

        //public int ICIndex { get; private set; }

        private int iCIndex = 1;
        public int ICIndex
        {
            get { return iCIndex; }
            set
            {
                iCIndex = value;
                this.tbNumber.Text = value.ToString();
            }
        }

        public int EpoxyIndex { get; private set; }

        public int BondMeasureIndex { get; private set; }

        public int BondMatchIndex { get; private set; }

        public int WireIndex { get; private set; }

        public int FreeRegionIndex { get; private set; }

        public int SurfaceDetectionIndex { get; private set; }


        public bool IsChangeProductDirectory => (bool)ckb_ChangeProductDirectory.IsChecked;

        public string NewProductDirectory => tb_ProductDirectory.Text;

        public int[] FrameIndexs { get; }

        //public int[] ICIndexs { get; }

        public int[] EpoxyIndexs { get; }

        public int[] BondMeasureIndexs { get; }

        public int[] BondMatchIndexs { get; }

        public int[] WireIndexs { get; }

        public int[] FreeRegionIndexs { get; }

        public int[] SurfaceDetectionIndexs { get; }

        private const int maxFrameComponentCount = 18;

        private const int maxICComponentCount = 18;

        private const int maxEpoxyComponentCount = 18;

        private const int maxBondMeasureComponentCount = 18;

        private const int maxBondMatchComponentCount = 18;

        private const int maxWireComponentCount = 18;

        private const int maxFreeRegionComponentCount = 18;

        private const int maxSurfaceDetectionComponentCount = 18;

        private List<IRecipe> currentComponents;

        public Window_AddNewRecipe(List<IRecipe> currentComponents)
        {
            FrameIndexs = new int[maxFrameComponentCount];
            for (int i = 0; i < maxFrameComponentCount; i++)
            {
                FrameIndexs[i] = i + 1;
            }

            //ICIndexs = new int[maxICComponentCount];
            //for (int i = 0; i < maxICComponentCount; i++)
            //{
            //    ICIndexs[i] = i + 1;
            //}

            EpoxyIndexs = new int[maxEpoxyComponentCount];
            for (int i = 0; i < maxEpoxyComponentCount; i++)
            {
                EpoxyIndexs[i] = i + 1;
            }

            BondMatchIndexs = new int[maxBondMatchComponentCount];
            for (int i = 0; i < maxBondMatchComponentCount; i++)
            {
                BondMatchIndexs[i] = i + 1;
            }

            BondMeasureIndexs = new int[maxBondMeasureComponentCount];
            for (int i = 0; i < maxBondMeasureComponentCount; i++)
            {
                BondMeasureIndexs[i] = i + 1;
            }

            WireIndexs = new int[maxWireComponentCount];
            for (int i = 0; i < maxWireComponentCount; i++)
            {
                WireIndexs[i] = i + 1;
            }

            FreeRegionIndexs = new int[maxFreeRegionComponentCount];
            for (int i = 0; i < maxFreeRegionComponentCount; i++)
            {
                FreeRegionIndexs[i] = i + 1;
            }

            SurfaceDetectionIndexs = new int[maxSurfaceDetectionComponentCount];
            for (int i = 0; i < maxSurfaceDetectionComponentCount; i++)
            {
                SurfaceDetectionIndexs[i] = i + 1;
            }

            InitializeComponent();
            this.currentComponents = currentComponents;

            //FilePath.ProductDirectory = $"{System.Windows.Forms.Application.StartupPath}\\TestProduct";//
            tb_ProductDirectory.Text = FilePath.ProductDirectory;//
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Btn_AddIniRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddIniRecipeCheck()) return;
            ComponentType = typeof(IniRecipe);
            this.DialogResult = true;
        }

        private void Btn_AddNewFrameRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddFrameRecipeCheck()) return;
            ComponentType = typeof(FrameRecipe);
            this.DialogResult = true;
        }

        private void Btn_AddNewICRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddICRecipeCheck()) return;
            ComponentType = typeof(ICRecipe);
            this.DialogResult = true;
        }

        private void Btn_AddBondRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddBondRecipeCheck()) return;
            ComponentType = typeof(BondRecipe);
            this.DialogResult = true;
        }

        private void Btn_AddBondMeasureRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddBondMeasureRecipeCheck()) return;
            ComponentType = typeof(BondMeasureRecipe);
            this.DialogResult = true;
        }

        private void Btn_AddEpoxyRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddEpoxyRecipeCheck()) return;
            ComponentType = typeof(EpoxyRecipe);
            this.DialogResult = true;
        }

        private void Btn_AddWireRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddWireRecipeCheck()) return;
            IsLoadXML = false; //1029
            ComponentType = typeof(WireRecipe);
            this.DialogResult = true;
        }

        private void Btn_AddRegionRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddRegionRecipeCheck()) return;
            ComponentType = typeof(FreeRegionRecipe);
            this.DialogResult = true;
        }

        private void Btn_AddCutRegionRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddCutRegionRecipeCheck()) return;
            ComponentType = typeof(CutRegionRecipe);
            this.DialogResult = true;
        }

        private void Btn_AddSurfaceDetectionRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddSurfaceDetectionRecipeCheck()) return;
            ComponentType = typeof(SurfaceDetectionRecipe);
            this.DialogResult = true;
        }
        //add by wj 2020-10-22
        private void Btn_AddAroundBondRegionDetectionRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (!AddAroundBondRegionDetectionRecipeCheck()) return;
            ComponentType = typeof(AroundBallRegionRecipe);
            this.DialogResult = true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        private void Btn_SelectIniXML_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                //if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                //tb_IniXmlPath.Text = IniRecipe.XmlName;

                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_IniXmlPath.Text = ofd.FileName;
            }
        }

        private void Btn_SelectFrameXML_Click(object sender, RoutedEventArgs e)
        {
            //tb_FrameXmlPath.Text = ShowOpenFileDialog(FrameRecipe.XmlName);
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_FrameXmlPath.Text = ofd.FileName;
            }
        }

        private void Btn_SelectICXML_Click(object sender, RoutedEventArgs e)
        {
            //tb_ICXmlPath.Text = ShowOpenFileDialog(ICRecipe.XmlName);
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_ICXmlPath.Text = ofd.FileName;
            }
        }

        private void Btn_SelectBondXML_Click(object sender, RoutedEventArgs e)
        {
            //tb_BondXmlPath.Text = ShowOpenFileDialog(BondRecipe.XmlName);
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_BondXmlPath.Text = ofd.FileName;
            }
        }

        private void Btn_SelectBondMeasureXML_Click(object sender, RoutedEventArgs e)
        {
            //tb_BondMeasureXmlPath.Text = ShowOpenFileDialog(BondMeasureRecipe.XmlName);
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_BondMeasureXmlPath.Text = ofd.FileName;
            }
        }

        private void Btn_SelectEpoxyXML_Click(object sender, RoutedEventArgs e)
        {
            //tb_EpoxyXmlPath.Text = ShowOpenFileDialog(EpoxyRecipe.XmlName);
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_EpoxyXmlPath.Text = ofd.FileName;
            }
        }

        private void Btn_SelectWireXML_Click(object sender, RoutedEventArgs e)
        {
            //tb_WireXmlPath.Text = ShowOpenFileDialog(WireRecipe.XmlName);
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_WireXmlPath.Text = ofd.FileName;
            }
        }

        private void Btn_SelectCutRegionXML_Click(object sender, RoutedEventArgs e)
        {
            //tb_WireXmlPath.Text = ShowOpenFileDialog(WireRecipe.XmlName);
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_CutRegionXmlPath.Text = ofd.FileName;
            }
        }

        private void Btn_SelectSurfaceDetectionXML_Click(object sender, RoutedEventArgs e)
        {
            //tb_WireXmlPath.Text = ShowOpenFileDialog(WireRecipe.XmlName);
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_SurfaceDetectionXmlPath.Text = ofd.FileName;
            }
        }

        // add by wj 2020-10-2
        private void Btn_SelectAroundBondRegionDetectionXML_Click(object sender, RoutedEventArgs e)
        {
            //tb_WireXmlPath.Text = ShowOpenFileDialog(WireRecipe.XmlName);
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_AroundBondRegionDetectionXmlPath.Text = ofd.FileName;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        private void Btn_LoadIniXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddIniRecipeCheck()) return;
            if (!File.Exists(tb_IniXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_IniXmlPath.Text, IniRecipe.IdentifyString)) return;
            ComponentType = typeof(IniRecipe);
            IsLoadXML = true;
            XmlPath = tb_IniXmlPath.Text;
            this.DialogResult = true;
        }

        private void Btn_LoadFrameXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddFrameRecipeCheck()) return;
            if (!File.Exists(tb_FrameXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_FrameXmlPath.Text, FrameRecipe.IdentifyString)) return;
            ComponentType = typeof(FrameRecipe);
            IsLoadXML = true;
            XmlPath = tb_FrameXmlPath.Text;
            this.DialogResult = true;
        }

        private void Btn_LoadICXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddICRecipeCheck()) return;
            ComponentType = typeof(ICRecipe);
            if (!File.Exists(tb_ICXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_ICXmlPath.Text, ICRecipe.IdentifyString)) return;
            IsLoadXML = true;
            XmlPath = tb_ICXmlPath.Text;
            FilePath.RecipeLoadProductDirectory = tb_ICXmlPath.Text;//
            this.DialogResult = true;
        }

        private void Btn_LoadBondXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddBondRecipeCheck()) return;
            ComponentType = typeof(BondRecipe);
            if (!File.Exists(tb_BondXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_BondXmlPath.Text, BondRecipe.IdentifyString)) return;
            IsLoadXML = true;
            XmlPath = tb_BondXmlPath.Text;
            FilePath.RecipeLoadProductDirectory = tb_BondXmlPath.Text;//
            this.DialogResult = true;
        }

        private void Btn_LoadBondMeasureXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddBondMeasureRecipeCheck()) return;
            ComponentType = typeof(BondMeasureRecipe);
            if (!File.Exists(tb_BondMeasureXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_BondMeasureXmlPath.Text, BondMeasureRecipe.IdentifyString)) return;
            IsLoadXML = true;
            XmlPath = tb_BondMeasureXmlPath.Text;
            FilePath.RecipeLoadProductDirectory = tb_BondMeasureXmlPath.Text;//
            this.DialogResult = true;
        }

        private void Btn_LoadEpoxyXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddEpoxyRecipeCheck()) return;
            ComponentType = typeof(EpoxyRecipe);
            if (!File.Exists(tb_EpoxyXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_EpoxyXmlPath.Text, EpoxyRecipe.IdentifyString)) return;
            IsLoadXML = true;
            XmlPath = tb_EpoxyXmlPath.Text;
            FilePath.RecipeLoadProductDirectory = tb_EpoxyXmlPath.Text;//
            this.DialogResult = true;
        }

        private void Btn_LoadWireXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddWireRecipeCheck()) return;
            ComponentType = typeof(WireRecipe);
            if (!File.Exists(tb_WireXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_WireXmlPath.Text, WireRecipe.IdentifyString)) return;
            IsLoadXML = true;
            XmlPath = tb_WireXmlPath.Text;
            FilePath.RecipeLoadProductDirectory = tb_WireXmlPath.Text;//
            this.DialogResult = true;
        }

        private void Btn_LoadCutRegionXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddCutRegionRecipeCheck()) return;
            ComponentType = typeof(CutRegionRecipe);
            if (!File.Exists(tb_CutRegionXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_CutRegionXmlPath.Text, CutRegionRecipe.IdentifyString)) return;
            IsLoadXML = true;
            XmlPath = tb_CutRegionXmlPath.Text;
            FilePath.RecipeLoadProductDirectory = tb_CutRegionXmlPath.Text;//
            this.DialogResult = true;
        }

        private void Btn_LoadSurfaceDetectionXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddSurfaceDetectionRecipeCheck()) return;
            ComponentType = typeof(SurfaceDetectionRecipe);
            if (!File.Exists(tb_SurfaceDetectionXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_SurfaceDetectionXmlPath.Text, SurfaceDetectionRecipe.IdentifyString)) return;
            IsLoadXML = true;
            XmlPath = tb_SurfaceDetectionXmlPath.Text;
            FilePath.RecipeLoadProductDirectory = tb_SurfaceDetectionXmlPath.Text;//
            this.DialogResult = true;
        }
        //add by wj 2020-10-22
        private void Btn_LoadAroundBondRegionDetectionXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddAroundBondRegionDetectionRecipeCheck()) return;
            ComponentType = typeof(AroundBallRegionRecipe);
            if (!File.Exists(tb_AroundBondRegionDetectionXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_AroundBondRegionDetectionXmlPath.Text, AroundBallRegionRecipe.IdentifyString)) return;
            IsLoadXML = true;
            XmlPath = tb_AroundBondRegionDetectionXmlPath.Text;
            FilePath.RecipeLoadProductDirectory = tb_AroundBondRegionDetectionXmlPath.Text;//
            this.DialogResult = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private bool AddIniRecipeCheck()
        {
            if (currentComponents.Exists(c => c is IniRecipe))
            {
                MessageBox.Show("创建全局数据模块已经存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }

        private bool AddFrameRecipeCheck()
        {
            if (currentComponents.Count(c => c is FrameRecipe) == maxFrameComponentCount)
            {
                MessageBox.Show($"最多支持{maxFrameComponentCount.ToString()}个Frame模块");
                return false;
            }
            FrameIndex = cb_FrameIndex.SelectedIndex + 1;
            if (currentComponents.Exists(c => c.DisplayName.Equals($"框架{FrameIndex.ToString()}模板")))
            {
                MessageBox.Show($"Frame{FrameIndex.ToString()}模板已存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }

        private bool AddICRecipeCheck()//改
        {
            if (currentComponents.Count(c => c is ICRecipe) == maxICComponentCount)
            {
                MessageBox.Show($"最多支持{maxICComponentCount.ToString()}个IC模块");
                return false;
            }
            //ICIndex = cb_ICIndex.SelectedIndex + 1;
            ICIndex = int.Parse(tbNumber.Text);
            if (currentComponents.Exists(c => c.DisplayName.Equals($"芯片{ICIndex.ToString()}模板")))
            {
                MessageBox.Show($"IC{ICIndex.ToString()}模板已存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }

        private bool AddBondRecipeCheck()
        {
            if (currentComponents.Count(c => c is BondRecipe) == maxBondMatchComponentCount)
            {
                MessageBox.Show($"最多支持{maxBondMatchComponentCount.ToString()}个基于模板匹配方式的焊点模块");
                return false;
            }
            BondMatchIndex = cb_BondMatchIndex.SelectedIndex + 1;
            if (currentComponents.Exists(c => c.DisplayName.Equals($"焊点Match{BondMatchIndex.ToString()}模板")))
            {
                MessageBox.Show($"焊点Match{BondMatchIndex.ToString()}模板已存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }

        private bool AddBondMeasureRecipeCheck()
        {
            if (currentComponents.Count(c => c is BondMeasureRecipe) == maxBondMeasureComponentCount)
            {
                MessageBox.Show($"最多支持{maxBondMeasureComponentCount.ToString()}个基于测量方式的焊点模块");
                return false;
            }
            BondMeasureIndex = cb_BondMeasureIndex.SelectedIndex + 1;
            if (currentComponents.Exists(c => c.DisplayName.Equals($"焊点Measure{BondMeasureIndex.ToString()}模板")))
            {
                MessageBox.Show($"焊点Measure{BondMeasureIndex.ToString()}模板已存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }

        private bool AddEpoxyRecipeCheck()
        {
            if (currentComponents.Count(c => c is EpoxyRecipe) == maxEpoxyComponentCount)
            {
                MessageBox.Show($"最多支持{maxEpoxyComponentCount.ToString()}个银胶检测");
                return false;
            }
            EpoxyIndex = cb_EpoxyIndex.SelectedIndex + 1;
            if (currentComponents.Exists(c => c.DisplayName.Equals($"银胶{EpoxyIndex.ToString()}检测")))
            {
                MessageBox.Show($"银胶{EpoxyIndex.ToString()}检测已存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }

        private bool AddWireRecipeCheck()
        {
            if (currentComponents.Count(c => c is WireRecipe) == maxWireComponentCount)
            {
                MessageBox.Show($"最多支持{maxWireComponentCount.ToString()}个Wire模块");
                return false;
            }
            WireIndex = cb_WireIndex.SelectedIndex + 1;
            if (currentComponents.Exists(c => c.DisplayName.Equals($"金线{WireIndex.ToString()}模板")))
            {
                MessageBox.Show($"Wire{WireIndex.ToString()}模板已存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }

        private bool AddRegionRecipeCheck()
        {
            if (currentComponents.Count(c => c is FreeRegionRecipe) == maxFreeRegionComponentCount)
            {
                MessageBox.Show($"最多支持{maxFreeRegionComponentCount.ToString()}个FreeRegion模块");
                return false;
            }
            FreeRegionIndex = cb_FreeRegionIndex.SelectedIndex + 1;
            if (currentComponents.Exists(c => c.DisplayName.Equals($"FreeRegion{FreeRegionIndex.ToString()}模板")))
            {
                MessageBox.Show($"FreeRegion{FreeRegionIndex.ToString()}模板已存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }

        private bool AddCutRegionRecipeCheck()
        {
            if (currentComponents.Exists(c => c.DisplayName.Equals($"CutRegion模板")))
            {
                MessageBox.Show($"CutRegion模板已存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }

        private bool AddSurfaceDetectionRecipeCheck()
        {
            if (currentComponents.Count(c => c is SurfaceDetectionRecipe) == maxSurfaceDetectionComponentCount)
            {
                MessageBox.Show($"最多支持{maxSurfaceDetectionComponentCount.ToString()}个表面检测模块");
                return false;
            }
            SurfaceDetectionIndex = cb_SurfaceDetectionIndex.SelectedIndex + 1;
            if (currentComponents.Exists(c => c.DisplayName.Equals($"SurfaceDetection{SurfaceDetectionIndex.ToString()}模板")))
            {
                MessageBox.Show($"SurfaceDetection{SurfaceDetectionIndex.ToString()}模板已存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }
        //add by wj 2020-10-22
        private bool AddAroundBondRegionDetectionRecipeCheck()
        {
            if (currentComponents.Exists(c => c.DisplayName.Equals($"AroundBondRegion模板")))
            {
                MessageBox.Show($"AroundBondRegion检测模板已存在");
                return false;
            }
            if ((bool)ckb_ChangeProductDirectory.IsChecked && !Directory.Exists(tb_ProductDirectory.Text))
            {
                MessageBox.Show("临时产品目录不存在，请重新选择");
                return false;
            }
            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string ShowOpenFileDialog(string filterFileName)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Filter = $"xml|{filterFileName}";
                ofd.Multiselect = false;
                ofd.InitialDirectory = FilePath.ProductDirectory + "\\Recipe";
                try
                {
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return "";
                    }
                }
                catch
                {
                    throw;
                }
                return ofd.FileName;
            }
        }

        private void Btn_SelectProductDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (Directory.Exists(tb_ProductDirectory.Text))
                {
                    fbd.SelectedPath = tb_ProductDirectory.Text;
                }
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    tb_ProductDirectory.Text = fbd.SelectedPath;
                }
            }
        }

        // lw 1216
        private void Btn_SelectRegionXML_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                if ((bool)ckb_ChangeProductDirectory.IsChecked && Directory.Exists(tb_ProductDirectory.Text))
                {
                    ofd.InitialDirectory = tb_ProductDirectory.Text;
                }
                else
                {
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                }
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                tb_RegionXmlPath.Text = ofd.FileName;
            }
        }

        private void Btn_LoadRegionXML_Click(object sender, RoutedEventArgs e)
        {
            if (!AddRegionRecipeCheck()) return;
            ComponentType = typeof(FreeRegionRecipe);
            if (!File.Exists(tb_RegionXmlPath.Text))
            {
                MessageBox.Show("参数文件不存在");
                return;
            }
            if (!XMLHelper.CheckIdentifier(tb_RegionXmlPath.Text, FreeRegionRecipe.IdentifyString)) return;
            IsLoadXML = true;
            XmlPath = tb_RegionXmlPath.Text;
            FilePath.RecipeLoadProductDirectory = tb_RegionXmlPath.Text;//
            this.DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Form_Lv_Inf a = new Form_Lv_Inf();
            a.ShowDialog();
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            ICIndex++;
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            if (ICIndex < 2) return;
            ICIndex--;
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbNumber == null || FrameIndex < 1) return;
            if (!int.TryParse(tbNumber.Text, out iCIndex))
                tbNumber.Text = iCIndex.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbNumber.Text = iCIndex.ToString();
        }
    }
}
