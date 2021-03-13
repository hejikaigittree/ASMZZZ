using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using MaterialDesignThemes.Wpf;
using System.Xml.Linq;
using System.Windows.Controls;
using System.IO;

namespace LFAOIRecipe
{
    class MainUIViewModel : ViewModelBase
    {
        string xmlDirectory=string.Empty;

        public ObservableCollection<IRecipe> Recipes { get; set; } = new ObservableCollection<IRecipe>();

        private IRecipe selectedRecipe;
        public IRecipe SelectedRecipe
        {
            get { return selectedRecipe; }
            set
            {
                //当所有Expander都被关闭后，ListBox选择值为null，界面会被清空
                //if (value == null) return;
                OnPropertyChanged(ref selectedRecipe, value);
            }
        }

        //1216
        Window_Loading window_Loading;

        public CommandBase AddRecipeCommand { get; private set; }
        public CommandBase DeleteRecipeCommand { get; private set; }
        public CommandBase LoadRecipesCommand { get; private set; }
        public CommandBase SaveRecipesCommand { get; private set; }
        public CommandBase DeleteRecipesCommand { get; private set; }
        public CommandBase SaveAsRecipesCommand { get; private set; }

        public MainUIViewModel()
        {
            AddRecipeCommand = new CommandBase(ExecuteAddRecipeCommand);
            DeleteRecipeCommand = new CommandBase(ExecuteDeleteRecipeCommand);
            LoadRecipesCommand = new CommandBase(ExecuteLoadRecipesCommand);
            SaveRecipesCommand = new CommandBase(ExecuteSaveRecipesCommand);
            DeleteRecipesCommand = new CommandBase(ExecuteDeleteRecipesCommand);
            SaveAsRecipesCommand = new CommandBase(ExecuteSaveAsRecipesCommand);
        }

        private void ExecuteAddRecipeCommand(object parameter)
         {
            Window_AddNewRecipe window_AddNewRecipe = new Window_AddNewRecipe(Recipes.ToList());

            FilePath.RecipeLoadProductDirectory = string.Empty;//

            if ((bool)window_AddNewRecipe.ShowDialog())
            {
                if (window_AddNewRecipe.IsChangeProductDirectory)
                {
                    FilePath.ProductDirectory = window_AddNewRecipe.NewProductDirectory;
                }
                if (window_AddNewRecipe.IsLoadXML)
                {
                    string recipePath = $"{Directory.GetParent(Directory.GetParent(window_AddNewRecipe.XmlPath).ToString())}\\";
                    string modelsPath = $"{Directory.GetParent(Directory.GetParent(Directory.GetParent(window_AddNewRecipe.XmlPath).ToString()).ToString())}\\Models\\";
                    FilePath.ProductDirectory = Directory.GetParent(Directory.GetParent(modelsPath).ToString()).ToString();
                }

                IRecipe recipe = null;

                Window_Loading window_Loading = new Window_Loading("正在创建模板");
                window_Loading.Show();

                if (window_AddNewRecipe.ComponentType == typeof(IniRecipe))
                {
                    recipe = new IniRecipe();
                }
                else if(window_AddNewRecipe.ComponentType == typeof(FrameRecipe))
                {
                    recipe = new FrameRecipe(window_AddNewRecipe.FrameIndex);
                }
                else if (window_AddNewRecipe.ComponentType == typeof(ICRecipe))
                {
                    recipe = new ICRecipe(window_AddNewRecipe.ICIndex);
                }
                else if (window_AddNewRecipe.ComponentType == typeof(EpoxyRecipe))
                {
                    recipe = new EpoxyRecipe(window_AddNewRecipe.EpoxyIndex);
                }
                else if (window_AddNewRecipe.ComponentType == typeof(BondRecipe))
                {
                    recipe = new BondRecipe(window_AddNewRecipe.BondMatchIndex);
                }
                else if (window_AddNewRecipe.ComponentType == typeof(BondMeasureRecipe))
                {
                    recipe = new BondMeasureRecipe(window_AddNewRecipe.BondMeasureIndex);
                }
                else if (window_AddNewRecipe.ComponentType == typeof(WireRecipe))
                {
                    recipe = new WireRecipe(window_AddNewRecipe.WireIndex);
                }
                else if (window_AddNewRecipe.ComponentType == typeof(FreeRegionRecipe))
                {
                    recipe = new FreeRegionRecipe(window_AddNewRecipe.FreeRegionIndex);
                }
                else if (window_AddNewRecipe.ComponentType == typeof(CutRegionRecipe))
                {
                    recipe = new CutRegionRecipe();
                }
                else if (window_AddNewRecipe.ComponentType == typeof(SurfaceDetectionRecipe))
                {
                    recipe = new SurfaceDetectionRecipe(window_AddNewRecipe.SurfaceDetectionIndex);
                }
                else if (window_AddNewRecipe.ComponentType == typeof(AroundBallRegionRecipe))
                {
                    recipe = new AroundBallRegionRecipe();
                }
                else return;

                Recipes.Add(recipe);
                
                SelectedRecipe = Recipes[Recipes.Count - 1];

                if (window_AddNewRecipe.IsLoadXML)
                {
                    recipe.LoadXML(window_AddNewRecipe.XmlPath);
                }
                window_Loading.Close();
            }
        }

        private void ExecuteDeleteRecipeCommand(object parameter)
        {
            if (SelectedRecipe == null) return;
            if (MessageBox.Show($"是否删除【{SelectedRecipe.DisplayName}】", "", MessageBoxButton.YesNo)== MessageBoxResult.Yes)
            {
                SelectedRecipe.Dispose();
                Recipes.Remove(SelectedRecipe);
                SelectedRecipe = null;
                GC.Collect();
            }
        }

        //加载产品
        private void ExecuteLoadRecipesCommand(object parameter)
        {
            try
            {
                if (Recipes.Count() != 0) return;
                string xmlPath;
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Multiselect = false;
                    ofd.InitialDirectory = FilePath.ProductDirectory;
                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    xmlPath = ofd.FileName;
                }

                string recipePath = $"{Directory.GetParent(xmlPath).ToString()}\\Recipe\\";
                string modelsPath = $"{Directory.GetParent(xmlPath).ToString()}\\Models\\";
                FilePath.ProductDirectory = Directory.GetParent(Directory.GetParent(modelsPath).ToString()).ToString();

                XElement root = XElement.Load(xmlPath);
                if (root == null)  return;

                //1216
                window_Loading = new Window_Loading("正在加载产品......");
                window_Loading.Show();

                foreach (var element in root.Elements())
                {
                    IRecipe recipe = null;
                    switch (element.Name.ToString())
                    {
                        case "IniRecipe":
                            recipe = new IniRecipe();
                            xmlDirectory = $"{recipePath}Reference\\";
                            break;

                        case "FrameRecipe":
                            recipe = new FrameRecipe(int.Parse(element.Attribute("Index").Value));
                            xmlDirectory = $"{recipePath}Frame{recipe.RecipeIndex}\\";
                            break;

                        case "ICRecipe":
                            recipe = new ICRecipe(int.Parse(element.Attribute("Index").Value));
                            xmlDirectory = $"{recipePath}IC{recipe.RecipeIndex}\\";
                            break;

                        case "EpoxyRecipe":
                            recipe = new EpoxyRecipe(int.Parse(element.Attribute("Index").Value));
                            xmlDirectory = $"{recipePath}Epoxy{recipe.RecipeIndex}\\";
                            break;

                        case "BondRecipe":
                            recipe = new BondRecipe(int.Parse(element.Attribute("Index").Value));
                            xmlDirectory = $"{recipePath}BondMatch{recipe.RecipeIndex}\\";
                            break;

                        case "BondMeasureRecipe":
                            recipe = new BondMeasureRecipe(int.Parse(element.Attribute("Index").Value));
                            xmlDirectory = $"{recipePath}BondMeasure{recipe.RecipeIndex}\\";
                            break;

                        case "WireRecipe":
                            recipe = new WireRecipe(int.Parse(element.Attribute("Index").Value));
                            xmlDirectory = $"{recipePath}Wire{recipe.RecipeIndex}\\";
                            break;

                        case "CutRegionRecipe":
                            recipe = new CutRegionRecipe();
                            xmlDirectory = $"{recipePath}CutRegion\\";
                            break;

                        case "SurfaceDetectionRecipe":
                            recipe = new SurfaceDetectionRecipe(int.Parse(element.Attribute("Index").Value));
                            xmlDirectory = $"{recipePath}SurfaceDetection{recipe.RecipeIndex}\\";
                            break;

                        case "FreeRegionRecipe":
                            recipe = new FreeRegionRecipe(int.Parse(element.Attribute("Index").Value));
                            xmlDirectory = $"{recipePath}FreeRegion{recipe.RecipeIndex}\\";
                            break;
                        case "AroundBallRegionRecipe":
                            recipe = new AroundBallRegionRecipe();
                            xmlDirectory = $"{recipePath}AroundBallRegion\\";
                            break;

                        default:
                            break;
                    }
                    if (recipe != null)
                    {
                        Recipes.Add(recipe);
                        if(!File.Exists($"{xmlDirectory}{element.Name.ToString()}.xml"))
                        {
                            MessageBox.Show($"{element.Name.ToString()}.xml 文件不存在！");
                        }
                        else
                        {
                            recipe.LoadXML($"{xmlDirectory}{element.Name.ToString()}.xml");
                        }
                    }
                }
                SelectedRecipe = Recipes[0];

                //1216
                window_Loading.Close();
            }
            catch (Exception ex)
            {
                //1216
                window_Loading.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        //保存产品
        private void ExecuteSaveRecipesCommand(object parameter)
        {
            try 
            {
                /* 后续添加另存为功能
                string selectedFolder;
                //MessageBox.Show("是否保存产品目录下所有recipe？");
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    dialog.SelectedPath = FilePath.ProductDirectory;
                    //System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        selectedFolder = dialog.SelectedPath;
                    }
                    else
                    {
                        selectedFolder = FilePath.ProductDirectory;
                    }
                }
                */

                XElement root = new XElement("Recipe");
                //XMLHelper.AddIdentifier(root, "ProductRecipes");
                foreach (var recipe in Recipes)
                {
                    XElement recipeNode = new XElement($"{ recipe.RecipeName }");
                    recipeNode.Add(new XAttribute("Index", $"{ recipe.RecipeIndex.ToString() }"));
                    recipeNode.Add(new XAttribute("XmlPath", $"{ recipe.XmlPath }"));
                    root.Add(recipeNode);
                    recipe.SaveXML(); 
                }
                root.Save($"{FilePath.ProductDirectory}\\{"ProductRecipes.xml"}");

                MessageBox.Show("保存完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //删除产品
        private void ExecuteDeleteRecipesCommand(object parameter)
        {
            try
            {
                if (Recipes.Count() == 0) return;
                if (MessageBox.Show($"是否删除产品目录", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int index = Recipes.Count();
                    foreach (var _recipe in Recipes)
                    {
                        _recipe.Dispose();
                    }
                    Recipes.Clear();
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //另存为
        private void ExecuteSaveAsRecipesCommand(object parameter)
        {

        }
    }
}
