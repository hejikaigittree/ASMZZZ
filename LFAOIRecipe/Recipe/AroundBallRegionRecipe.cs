using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace LFAOIRecipe
{
    class AroundBallRegionRecipe : ProcedureControl, IRecipe
    {
        public string DisplayName { get; }

        public object Content { get; private set; }

        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;

        public const string XmlName = "AroundBallRegionRecipe.xml";

        public const string IdentifyString = "AROUNDBALLREGIONRECIPEXML";

        private string RecipeDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\AroundBallRegion\\");

        private string ModelsDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\AroundBallRegion\\");

        private string ModelsFile => $"{ProductDirctory}\\Models\\";

        private string RecipeFile => $"{ProductDirctory}\\Recipe\\";

        public string XmlPath => $"{RecipeDirectory}{XmlName}";

        public int RecipeIndex { get; }

        public string RecipeName { get; } = "AroundBallRegionRecipe";

        private string ReferenceDirectory => $"{ProductDirctory}\\Recipe\\Reference\\";

        public static bool isRightClickBond = true;//

        private readonly HTHalControlWPF htWindow;

        //HTuple aroundBondRegionParameters, AroundBondRegionRecipeNames = null;

        //后续优化
        private EpoxyModelObject AroundBondDetectionObject = new EpoxyModelObject();

        private ObservableCollection<UserRegion> AroundBondRegUserRegions = new ObservableCollection<UserRegion>();

        private AroundBondRegionModelInspectParameter AroundBondRegInspectParameter = new AroundBondRegionModelInspectParameter();


        #region 流程模块
        private IProcedure createAroundBondRegionModel;
        #endregion


        public AroundBallRegionRecipe()
        {

            Content = new Page_CreateRecipe();
            (Content as Page_CreateRecipe).DataContext = this;
            htWindow = (Content as Page_CreateRecipe).htWindow;
            DisplayName = $"AroundBond模板";

            createAroundBondRegionModel = new CreateAroundBondRegionModel(htWindow,
                                                                         ReferenceDirectory,
                                                                         ModelsFile,
                                                                         RecipeFile,
                                                                         AroundBondDetectionObject,
                                                                         AroundBondRegInspectParameter,
                                                                         AroundBondRegUserRegions,
                                                                         ModelsDirectory
                                                                         );
            (createAroundBondRegionModel as CreateAroundBondRegionModel).OnSaveXML += SaveXML;

            // 0127
            htWindow.useCreateAroundBondRegionModel(createAroundBondRegionModel as CreateAroundBondRegionModel);

            Procedures = new IProcedure[]
            {
                createAroundBondRegionModel,
            };
            ProcedureChanged(0);
        }

        //加载
        //--------保存XML
        public void SaveXML()
        {
            try
            {
                Directory.GetFiles(ModelsDirectory).ToList().ForEach(File.Delete);
                //Directory.GetFiles(RecipeDirectory).ToList().ForEach(File.Delete);
                XElement root = new XElement("Recipe");
                XMLHelper.AddIdentifier(root, IdentifyString);

                XElement AroundBondRegInspectParameterNode = new XElement("AroundBondRegInspectParameter");

                XMLHelper.AddParameters(AroundBondRegInspectParameterNode, AroundBondRegInspectParameter, IdentifyString);

                XElement AroundBondRegNode = new XElement("AroundBondRegions");

                XMLHelper.AddRegion(AroundBondRegNode, AroundBondRegUserRegions, "AroundBondReg", false, false, false,true);

                //if (AroundBondDetectionObject.RejectRegion != null)
                //{
                //    //当拒绝区是不规则区域时，拒绝区域保存在Models下
                //    HOperatorSet.WriteRegion(goldenModelObject.RejectRegion, $"{ModelsRecipeDirectory}" + "Reject_FreeRegion.reg");
                //}

                foreach (var item in AroundBondRegUserRegions)
                {
                    if (item.RegionType == RegionType.Region)
                    {
                        //HOperatorSet.ReadRegion(out HObject freeRegion, $"{ ProductDirctory}\\{ item.RegionPath}");
                        HOperatorSet.WriteRegion(item.CalculateRegion, $"{ProductDirctory}\\{item.RegionPath}");
                    }
                }


                root.Add(AroundBondRegInspectParameterNode);
                root.Add(AroundBondRegNode);
                root.Save(XmlPath);

            }
              catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //加载
        public void LoadXML(string xmlPath)
        {
            try
            {
                XElement root = XElement.Load(xmlPath);
                XElement AroundBondRegInspectParameterNode = new XElement("AroundBondRegInspectParameter");
                if (AroundBondRegInspectParameterNode == null) return;
                XMLHelper.ReadParameters(AroundBondRegInspectParameterNode, AroundBondRegInspectParameter);

                (createAroundBondRegionModel as CreateAroundBondRegionModel).LoadReferenceData();
                //(createAroundBondRegionModel as CreateAroundBondRegionModel).isLoadXML = isLoadXML;
                //(createAroundBondRegionModel as CreateAroundBondRegionModel).LoadInspectBondRegions();


                //加载区域内检测参数
                XElement AroundBondRegNode = root.Element("AroundBondRegions");
                if (AroundBondRegNode == null) return;
                XMLHelper.ReadRegion(AroundBondRegNode, AroundBondRegUserRegions, "AroundBondReg", 
                                    AroundBondRegInspectParameter.DieImageRowOffset, AroundBondRegInspectParameter.DieImageColumnOffset, false, false, false,true);
                //XMLHelper.ReadRegion(AroundBondRegNode, AroundBondRegUserRegions, "AroundBondReg", AroundBondRegInspectParameter.DieImageRowOffset, AroundBondRegInspectParameter.DieImageColumnOffset, false, true);

                //1211
                foreach (var item in AroundBondRegUserRegions)
                {
                    // 兼容老产品打开参数不保留 1223 lw
                    if (item == null)
                    {
                        AroundBondRegUserRegions.Clear();
                        break;
                    }

                    if (item.RegionType == RegionType.Region)
                    {
                        //HOperatorSet.ReadRegion(out HObject freeRegion, $"{ ProductDirctory}\\{ item.RegionPath}");

                        // mod by lht 12-23
                        //HOperatorSet.ReadRegion(out HObject freeRegion, $"{ item.RegionPath}");
                        HOperatorSet.ReadRegion(out HObject freeRegion, $"{ProductDirctory}\\{ item.RegionPath}");

                        item.CalculateRegion = freeRegion;
                        HOperatorSet.MoveRegion(freeRegion, out HObject _freeRegion, -AroundBondRegInspectParameter.DieImageRowOffset, -AroundBondRegInspectParameter.DieImageColumnOffset);
                        item.DisplayRegion = _freeRegion;
                    }
                }
                //if (isLoadXML)
                //{
                //    htWindow.DisplayMultiRegion(AroundBondRegUserRegions2.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
                //}
                //else
                //{
                    htWindow.DisplayMultiRegion(AroundBondRegUserRegions.Where(r => r.IsEnable).Select(r => r.DisplayRegion));
                //}
                (createAroundBondRegionModel as CreateAroundBondRegionModel).Initial();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }




    }
}
