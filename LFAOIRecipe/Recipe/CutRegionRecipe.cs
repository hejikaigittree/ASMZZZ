using HalconDotNet;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Windows.Interactivity;
using System.Linq;

namespace LFAOIRecipe
{
    class CutRegionRecipe : ProcedureControl, IRecipe
    {
        //1121  using System.Collections.ObjectModel;
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;

        public const string XmlName = "CutRegionRecipe.xml";

        public const string IdentifyString = "CUTREGIONRECIPEXML";

        private string RecipeDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\CutRegion\\");

        private string ModelsDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\CutRegion\\");

        private string ModelsFile => $"{ProductDirctory}\\Models\\";

        private string RecipeFile => $"{ProductDirctory}\\Recipe\\";

        public string XmlPath => $"{RecipeDirectory}{XmlName}";

        public int RecipeIndex { get; }

        public string RecipeName { get; } = "CutRegionRecipe";

        private string ReferenceDirectory => $"{ProductDirctory}\\Recipe\\Reference\\";

        public static bool isRightClickBond = true;//

        private readonly HTHalControlWPF htWindow;

        HTuple cutRegionParameters ,CutRegionRecipeNames = null;

        //后续优化
        private EpoxyModelObject CutRegionObject = new EpoxyModelObject();

        private CutRegionParameter CutRegionParameter = new CutRegionParameter();

        private ObservableCollection<UserRegion> CutRegionUserRegions = new ObservableCollection<UserRegion>();
        private ObservableCollection<UserRegion> OriRegionUserRegions = new ObservableCollection<UserRegion>();

        #region 流程模块
        private IProcedure createCutRegionModel;
        #endregion

        public CutRegionRecipe()
        {
            Content = new Page_CreateRecipe();
            (Content as Page_CreateRecipe).DataContext = this;
            htWindow = (Content as Page_CreateRecipe).htWindow;
            DisplayName = $"CutRegion模板";

            createCutRegionModel = new CreateCutRegionModel(htWindow,
                                                ReferenceDirectory,
                                                ModelsFile,
                                                RecipeFile,
                                                CutRegionObject,
                                                CutRegionParameter,
                                                CutRegionUserRegions,
                                                OriRegionUserRegions,
                                                ModelsDirectory);

            (createCutRegionModel as CreateCutRegionModel).OnSaveXML += SaveXML;

            Procedures = new IProcedure[]
            {
                createCutRegionModel,
            };
            ProcedureChanged(0);
        }

        //保存 
        public void SaveXML()
        {
            try
            {
                Directory.GetFiles(ModelsDirectory).ToList().ForEach(File.Delete);
                XElement root = new XElement("Recipe");
                XMLHelper.AddIdentifier(root, IdentifyString);
                XElement cutRegionParameterNode = new XElement("CutRegionParameter");

                // 保存初始cutReg, 即膨胀前参数 0125 lw
                XElement CutRegNode = new XElement("CutRegions");
                XMLHelper.AddRegion(CutRegNode, OriRegionUserRegions, "CutReg", false, false, false, false);

                //Models下保存FreeRegion  add by 2020-12-24
                foreach (var item in OriRegionUserRegions)
                {
                    if (item.RegionType == RegionType.Region)
                    {
                        //HOperatorSet.ReadRegion(out HObject freeRegion, $"{ ProductDirctory}\\{ item.RegionPath}");
                        HOperatorSet.WriteRegion(item.CalculateRegion, $"{ProductDirctory}\\{item.RegionPath}");
                    }
                }

                //HOperatorSet.MoveRegion(Algorithm.Region.ConcatRegion(CutRegionUserRegions), out HObject CutRegion, CutRegionParameter.DieImageRowOffset, CutRegionParameter.DieImageColumnOffset);//

                //HTuple file = new HTuple();
                //file = $"{ModelsFile}\\{ CutRegionParameter.OnRecipesIndexs[CutRegionParameter.OnRecipesIndex]}\\";
                //HOperatorSet.WriteRegion(CutRegion, file + "CutRegions.reg");


                cutRegionParameters = new HTuple();
                CutRegionRecipeNames = new HTuple();
                foreach (var item in CutRegionUserRegions)
                {
                    HOperatorSet.TupleConcat(cutRegionParameters, item.RegionParameters[0], out cutRegionParameters); 
                    HOperatorSet.TupleConcat(CutRegionRecipeNames, item.RecipeNames, out CutRegionRecipeNames); 
                }
                HOperatorSet.WriteTuple(cutRegionParameters, $"{ModelsDirectory}CutRegionParameters.tup");
                HOperatorSet.WriteTuple(CutRegionRecipeNames, $"{ModelsDirectory}CutRegionRecipeNames.tup");
                CutRegionParameter.CutRegionParameters = cutRegionParameters;
                XMLHelper.AddParameters(cutRegionParameterNode, CutRegionParameter, IdentifyString);

                root.Add(cutRegionParameterNode);
                root.Add(CutRegNode);
                root.Save(XmlPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //加载 
        public void LoadXML(string xmlPath)//async
        {
            
            //await Task.Run(() =>
            //{
            //    while (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff)
            //    {
            //        Thread.Sleep(500);
            //    }
            //});
            

            try
            {
                XElement root = XElement.Load(xmlPath);
                XElement cutRegionParameterNode = root.Element("CutRegionParameter");
                if (cutRegionParameterNode == null) return;
                XMLHelper.ReadParameters(cutRegionParameterNode, CutRegionParameter);
                (createCutRegionModel as CreateCutRegionModel).LoadReferenceData();
                //(createCutRegionModel as CreateCutRegionModel).LoadInspectCutRegions();
                
                // add by wj
                //加载区域内检测参数
                XElement AroundBondRegNode = root.Element("CutRegions");
                if (AroundBondRegNode == null) return;
                XMLHelper.ReadRegion(AroundBondRegNode, OriRegionUserRegions, "CutReg",
                                    CutRegionParameter.DieImageRowOffset, CutRegionParameter.DieImageColumnOffset, false, false, false, false);

                foreach (var item in OriRegionUserRegions)
                {
                    // 兼容老产品打开参数不保留 1223 lw
                    if (item == null)
                    {
                        OriRegionUserRegions.Clear();
                        break;
                    }

                    if (item.RegionType == RegionType.Region)
                    {
                        HOperatorSet.ReadRegion(out HObject freeRegion, $"{ProductDirctory}\\{ item.RegionPath}");

                        item.DisplayRegion = freeRegion;
                        item.CalculateRegion = freeRegion;
                    }
                }

                (createCutRegionModel as CreateCutRegionModel).LoadDilationParameters();
                /*
                HOperatorSet.ReadTuple($"{ModelsDirectory}CutRegionParameters.tup",out cutRegionParameters);
                HOperatorSet.ReadTuple($"{ModelsDirectory}CutRegionRecipeNames.tup", out CutRegionRecipeNames);
                HOperatorSet.ReadRegion(out HObject CutDisplayRegions, $"{ModelsDirectory}CutDisplayRegions.reg");
                HOperatorSet.ReadRegion(out HObject CutRegions, $"{ModelsDirectory}CutRegions.reg");
                HOperatorSet.MoveRegion(CutDisplayRegions, out HObject DieCutDisplayRegion, -CutRegionParameter.DieImageRowOffset, -CutRegionParameter.DieImageColumnOffset);//
                HOperatorSet.MoveRegion(CutRegions, out HObject DieCutRegion, -CutRegionParameter.DieImageRowOffset, -CutRegionParameter.DieImageColumnOffset);//calculation
                CutDisplayRegions.CountObj();

                for (int i = 1; i < CutRegionRecipeNames.TupleLength()+1; i++)
                {
                    UserRegion userRegion = new UserRegion
                    {                        
                        DisplayRegion = DieCutDisplayRegion.SelectObj(i),
                        CalculateRegion = DieCutRegion.SelectObj(i),
                        RegionParameters = new double[1] { cutRegionParameters.TupleSelect(i-1)},
                        RecipeNames = CutRegionRecipeNames.TupleSelect(i-1),
                        RegionType=RegionType.Dilation,
                    };
                    if (userRegion == null) return;
                    userRegion.Index = CutRegionUserRegions.Count + 1;
                    CutRegionUserRegions.Add(userRegion);
                }
                */

                (createCutRegionModel as CreateCutRegionModel).Initial();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Dispose()
        {
            htWindow.Dispose();
            (Content as Page_CreateRecipe).DataContext = null;
            (Content as Page_CreateRecipe).Close();
            (createCutRegionModel as CreateCutRegionModel).OnSaveXML -= SaveXML;
            createCutRegionModel.Dispose();
            Content = null;
            CutRegionObject = null;
            CutRegionParameter = null;
            CutRegionUserRegions = null;
            OriRegionUserRegions = null;
            Procedures = null;
            CutRegionRecipeNames = null;
        }
    }
}
