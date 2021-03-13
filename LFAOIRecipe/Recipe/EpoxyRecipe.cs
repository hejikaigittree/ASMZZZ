using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using HalconDotNet;

namespace LFAOIRecipe
{
    class EpoxyRecipe : ProcedureControl, IRecipe
    {
        //1121  using System.Collections.ObjectModel;
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;

        public const string XmlName = "EpoxyRecipe.xml";

        public const string IdentifyString = "EPOXYRECIPEXML";

        private string RecipeDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\Epoxy{RecipeIndex.ToString()}\\");

        private string ModelsDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\Epoxy{RecipeIndex.ToString()}\\");//

        public string XmlPath => $"{RecipeDirectory}{XmlName}";

        public int RecipeIndex { get; }

        public string RecipeName { get; } = "EpoxyRecipe";

        private string ModelsFile => $"{ProductDirctory}\\Models\\";

        private string RecipeFile => $"{ProductDirctory}\\Recipe\\";
        private string ReferenceDirectory => $"{ProductDirctory}\\Recipe\\Reference\\";

        public static bool isRightClickBond = true;//

        private HTHalControlWPF htWindow;

        private EpoxyModelObject epoxyModelObject = new EpoxyModelObject();

        private EpoxyParameter epoxyParameter = new EpoxyParameter();

        private EpoxyModelVerifyParameter epoxyModelVerifyParameter = new EpoxyModelVerifyParameter();

        private EpoxyModelVerifyParameterSet epoxyModelVerifyParameterSet = new EpoxyModelVerifyParameterSet();

        private ObservableCollection<UserRegion> epoxyUserRegions = new ObservableCollection<UserRegion>();

        private ObservableCollection<UserRegion> epoxyReferceUserRegions = new ObservableCollection<UserRegion>();

        //private IEnumerable<HObject> DieRegions => InspectUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion);

        #region 流程模块
        private IProcedure addPoxyRegions;
        #endregion

        //private readonly int epoxyIndex;

        public EpoxyRecipe(int epoxyIndex)
        {
            this.RecipeIndex = epoxyIndex;
            Content = new Page_CreateRecipe();
            (Content as Page_CreateRecipe).DataContext = this;
            htWindow = (Content as Page_CreateRecipe).htWindow;
            DisplayName = $"银胶{this.RecipeIndex.ToString()}检测";

            addPoxyRegions = new AddPoxyRegions(htWindow,
                                                ModelsFile,
                                                RecipeFile,
                                                ReferenceDirectory,
                                                epoxyModelObject,
                                                epoxyParameter,
                                                epoxyModelVerifyParameter,
                                                epoxyModelVerifyParameterSet,
                                                epoxyUserRegions,
                                                ModelsDirectory,
                                                epoxyReferceUserRegions);

            (addPoxyRegions as AddPoxyRegions).OnSaveXML += SaveXML;

            Procedures = new IProcedure[]
            {
                addPoxyRegions,
            };
            ProcedureChanged(0);
        }

        //保存
        public void SaveXML()
        {
            Directory.GetFiles(ModelsDirectory).ToList().ForEach(File.Delete);
            XElement root = new XElement("Recipe");
            XMLHelper.AddIdentifier(root, IdentifyString);
            XElement epoxyParameterNode = new XElement("EpoxyParameter");
            XElement epoxyModelVerifyParameterNode = new XElement("EpoxyModelVerifyParameter");
            XMLHelper.AddParameters(epoxyParameterNode, epoxyParameter, IdentifyString);
            XMLHelper.AddParameters(epoxyModelVerifyParameterNode, epoxyModelVerifyParameter, IdentifyString);
            XMLHelper.AddRegion(root, epoxyUserRegions, "EpoxyUserRegions", true);
            XMLHelper.AddRegion(root, epoxyReferceUserRegions, "EpoxyReferceUserRegions", false);
            root.Add(epoxyParameterNode);
            root.Add(epoxyModelVerifyParameterNode);
            root.Save(XmlPath);
            HOperatorSet.WriteRegion(Algorithm.Region.ConcatRegion(epoxyUserRegions), $"{ModelsDirectory}Inspect_Regions.reg");
            HOperatorSet.WriteRegion((epoxyReferceUserRegions.Where(r=>r.IsEnable).Select(r=>r.CalculateRegion)).FirstOrDefault(), $"{ModelsDirectory}Reference_Region.reg");

            if (epoxyParameter.ImageCountChannels == 1)
            {
                HOperatorSet.WriteTuple(new HTuple(1), ModelsDirectory + "Image_Index.tup");
            }
            else if (epoxyParameter.ImageCountChannels > 1)
            {
                HOperatorSet.WriteTuple(epoxyModelVerifyParameter.ImageChannelIndex + 1,ModelsDirectory + "Image_Index.tup");
            }

            try
            {
                if (epoxyParameter.OnRecipesIndex >= 0)
                {
                    HOperatorSet.WriteTuple(epoxyParameter.OnRecipesIndexs[epoxyParameter.OnRecipesIndex], $"{ModelsDirectory}OnWhat.tup");
                }
            }
            catch (Exception)//改
            {
                MessageBox.Show("请选择\"银胶属于\"位置！");
            }
        }

        //加载
        public  void LoadXML(string xmlPath)
        {
            /*
            await Task.Run(() =>
            {
                while (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff)
                {
                    Thread.Sleep(500);
                }
            });
            */
            try
            {

                XElement root = XElement.Load(xmlPath);
                XElement epoxyParameterNode = root.Element("EpoxyParameter");
                if (epoxyParameterNode == null) return;
                XMLHelper.ReadParameters(epoxyParameterNode, epoxyParameter);
                (addPoxyRegions as AddPoxyRegions).LoadReferenceData();
                XElement epoxyModelVerifyParameterNode = root.Element("EpoxyModelVerifyParameter");
                XMLHelper.ReadParameters(epoxyModelVerifyParameterNode, epoxyModelVerifyParameter);
                XMLHelper.ReadRegion(root, epoxyUserRegions, "EpoxyUserRegions", epoxyParameter.DieImageRowOffset, epoxyParameter.DieImageColumnOffset, true);
                XMLHelper.ReadRegion(root, epoxyReferceUserRegions, "EpoxyReferceUserRegions", epoxyParameter.DieImageRowOffset, epoxyParameter.DieImageColumnOffset, false);
                (addPoxyRegions as AddPoxyRegions).Initial();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Dispose()
        {
            //htWindow.Dispose();
            (Content as Page_CreateRecipe).DataContext = null;
            (Content as Page_CreateRecipe).Close();
            (addPoxyRegions as AddPoxyRegions).OnSaveXML -= SaveXML;
            addPoxyRegions.Dispose();
            Content = null;
            epoxyModelObject = null;
            epoxyParameter = null;
            epoxyModelVerifyParameter =null;
            epoxyModelVerifyParameterSet = null;
            epoxyUserRegions = null;
            Procedures = null;
        }
    }
}
