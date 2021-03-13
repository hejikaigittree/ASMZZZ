using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    class ICRecipe : GoldenModelRecipe
    {
        //1121  using System.Collections.ObjectModel;
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public override string DisplayName { get; }

        public const string XmlName = "ICRecipe.xml";

        public const string IdentifyString = "ICRECIPEXML";

        protected override string RecipeDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\IC{RecipeIndex.ToString()}\\");

        protected override string ModelsRecipeDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\IC{RecipeIndex.ToString()}\\");//

        public override string XmlPath => $"{RecipeDirectory}{XmlName}";

        public override int RecipeIndex { get; }

        public override string RecipeName { get; } = "ICRecipe";

        public ICRecipe(int icIndex)
        {
            this.RecipeIndex = icIndex;
            DisplayName = $"芯片{this.RecipeIndex.ToString()}模板";
            (Content as Page_CreateRecipe).DataContext = this;

            InitialProcedures();
            Procedures = new IProcedure[]
            {
                cutOutDie,
                addMatchRegion,
                addInspectRegion,
                addRejectRegion,
                addSubRegion,
                createGoldenModel,
                goldenModelInspectVerify
            };
            (goldenModelInspectVerify as GoldenModelInspectVerify).OnSaveXML += SaveXML;

            ProcedureChanged(0);
        }

        public override void SaveXML()
        {
            try
            {
		        Directory.GetFiles(ModelsRecipeDirectory).ToList().ForEach(File.Delete);
                UserRegion userRegion = (cutOutDie as CutOutDie).UserRegionForCutOut;
                if (userRegion != null) goldenModelParameter.UserRegionForCutOutIndex = userRegion.Index;

                XElement root = new XElement("Recipe");
                XMLHelper.AddIdentifier(root, IdentifyString);
                XElement goldenModelParameterNode = new XElement("GoldenModelParameter");
                XElement goldenModelInspectParameterNode = new XElement("GoldenModelInspectParameter");

                XMLHelper.AddRegion(root, dieUserRegions, "Die_Regions");
                XMLHelper.AddRegion(root, matchUserRegions, "Match_Region");
                XMLHelper.AddRegion(root, inspectUserRegions, "Inspect_Region");
                XMLHelper.AddRegion(root, subUserRegions, "Sub_Regions");
                XMLHelper.AddRegion(root, rejectUserRegions, "Reject_Region");
                if(goldenModelObject.RejectRegion != null)
                { 
                    //当拒绝区是不规则区域时，拒绝区域保存在Models下
                    HOperatorSet.WriteRegion(goldenModelObject.RejectRegion, $"{ModelsRecipeDirectory}" + "Reject_FreeRegion.reg");
                }

                XMLHelper.AddParameters(goldenModelParameterNode, goldenModelParameter, IdentifyString);
                XMLHelper.AddParameters(goldenModelInspectParameterNode, goldenModelInspectParameter, IdentifyString);

                root.Add(goldenModelParameterNode);
                root.Add(goldenModelInspectParameterNode);
                root.Save(XmlPath);

                if (goldenModelParameter.OnRecipesIndexs.Length > 0)
                {
                    HOperatorSet.WriteTuple(goldenModelParameter.OnRecipesIndexs?[goldenModelParameter.OnRecipesIndex], ModelsRecipeDirectory + "OnWhat.tup");
                }
                HOperatorSet.WriteTuple(goldenModelInspectParameter.AngleStart, ModelsRecipeDirectory + "AngleStart.tup");
                HOperatorSet.WriteTuple(goldenModelInspectParameter.AngleExt, ModelsRecipeDirectory + "AngleExt.tup");
                HOperatorSet.WriteTuple(goldenModelInspectParameter.MinMatchScore, ModelsRecipeDirectory + "MinMatchScore.tup");
                HOperatorSet.WriteTuple(goldenModelInspectParameter.MatchNum, ModelsRecipeDirectory + "MatchNum.tup");
                HOperatorSet.WriteTuple(goldenModelInspectParameter.DilationSize, ModelsRecipeDirectory + "DilationSize.tup");
                HOperatorSet.WriteRegion(Algorithm.Region.Union1Region(matchUserRegions), ModelsRecipeDirectory + "Match_Region.reg");
                HOperatorSet.WriteRegion(Algorithm.Region.Union1Region(inspectUserRegions), ModelsRecipeDirectory + "Inspect_Region.reg");
                HOperatorSet.WriteRegion(Algorithm.Region.Union1Region(rejectUserRegions), ModelsRecipeDirectory + "Reject_Region.reg");
                HOperatorSet.WriteRegion(Algorithm.Region.Union1Region(subUserRegions), ModelsRecipeDirectory + "Sub_Region.reg");
 
                if (goldenModelParameter.ImageCountChannels == 1)
                {
                    HOperatorSet.WriteTuple((new HTuple(1)).TupleConcat(new HTuple(1)).TupleConcat(new HTuple(1)), ModelsRecipeDirectory + "Image_Index.tup");
                }
                else if (goldenModelParameter.ImageCountChannels > 1)
                {
                    // 1122-lw
                    HOperatorSet.WriteTuple((new HTuple(goldenModelParameter.ImageChannelIndex + 1))
                                           .TupleConcat(new HTuple( goldenModelParameter.ImageGoldChannelIndex + 1))
                                           .TupleConcat(new HTuple( goldenModelInspectParameter.ImageChannelIndex_IcExist + 1)),
                                           ModelsRecipeDirectory + "Image_Index.tup");
                }

                HOperatorSet.WriteTuple(new HTuple(goldenModelInspectParameter.IsICExist == false ? 0 : 1)
                                        .TupleConcat(new HTuple(goldenModelInspectParameter.IsICLocate == false ? 0 : 1))
                                        .TupleConcat(new HTuple(goldenModelInspectParameter.IsICOffSet == false ? 0 : 1))
                                        .TupleConcat(new HTuple(goldenModelInspectParameter.IsICSurfaceInspect == false ? 0 : 1)),
                                        ModelsRecipeDirectory + "TaskEnable.tup");

                if (goldenModelParameter.ModelType == 0)
                {
                    HOperatorSet.WriteTuple(new HTuple("ncc"), $"{ModelsRecipeDirectory}Model_Type.tup");
                }
                else if (goldenModelParameter.ModelType == 1)
                {
                    HOperatorSet.WriteTuple(new HTuple("shape"), $"{ModelsRecipeDirectory}Model_Type.tup");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
 
            try
            {
                if (File.Exists($"{RecipeDirectory}PosModel.dat") || goldenModelObject.PosModelID != null)
                {
                    if (goldenModelParameter.ModelIdPath.Split(',').Length == 1)
                    {
                        if (File.Exists($"{RecipeDirectory}PosModel.dat") || goldenModelObject.PosModelID != null)
                        {
                            if (goldenModelObject.PosModelID != null)
                            {
                                Algorithm.File.SaveModel(ModelsRecipeDirectory + "PosModel.dat", goldenModelParameter.ModelType, goldenModelObject.PosModelID);
                            }
                            else if (File.Exists($"{RecipeDirectory}PosModel.dat"))
                            {
                                HTuple PosModelId = Algorithm.File.ReadModel($"{RecipeDirectory}PosModel.dat", goldenModelParameter.ModelType);
                                Algorithm.File.SaveModel(ModelsRecipeDirectory + "PosModel.dat", goldenModelParameter.ModelType, PosModelId);
                                // 清除模板 lw 0121
                                Algorithm.Model_RegionAlg.HTV_clear_model_recipe(PosModelId, goldenModelParameter.ModelType);
                            }
                        }
                        else
                        {
                            MessageBox.Show("请创建定位模板！");
                        }
                    }
                    else
                    {
                        if (goldenModelObject.PosModelID != null)
                        {
                            String[] ModelIdPathArry = new string[goldenModelObject.PosModelID.TupleLength()];
                            for (int i = 0; i < goldenModelObject.PosModelID.TupleLength(); i++)
                            {
                                ModelIdPathArry[i] = $"{ModelsRecipeDirectory}PosModel_" + i + ".dat";
                            }
                            goldenModelParameter.ModelIdPath = String.Join(",", ModelIdPathArry);
                            Algorithm.File.SaveModel(ModelIdPathArry, goldenModelParameter.ModelType, goldenModelObject.PosModelID);
                        }
                        else if (File.Exists($"{RecipeDirectory}PosModel.dat"))
                        {
                            HTuple ModelId = Algorithm.File.ReadModel($"{RecipeDirectory}PosModel.dat".Split(','), goldenModelParameter.ModelType);
                            String[] ModelIdPathArry = new string[ModelId.TupleLength()];
                            for (int i = 0; i < ModelId.TupleLength(); i++)
                            {
                                ModelIdPathArry[i] = $"{ModelsRecipeDirectory}PosModel_" + i + ".dat";
                            }
                            Algorithm.File.SaveModel(ModelIdPathArry, goldenModelParameter.ModelType, ModelId);
                            // 清除模板 lw 0121
                            Algorithm.Model_RegionAlg.HTV_clear_model_recipe(ModelId, goldenModelParameter.ModelType);
                        }
                    }
                }

                if (goldenModelObject.MeadImage != null && goldenModelObject.StdImage != null)
                {
                    HOperatorSet.WriteImage(goldenModelObject.MeadImage, "tiff", 0, ModelsRecipeDirectory + "Mean_Image.tiff");
                    //HOperatorSet.WriteImage(goldenModelObject.StdImage, "tiff", 0, ModelsRecipeDirectory + "Std_Image.tiff");
                }
                else if (File.Exists($"{RecipeDirectory}Mean_Image.tiff") /*&& File.Exists($"{RecipeDirectory}Std_Image.tiff")*/)
                {
                    HOperatorSet.ReadImage(out HObject Mean_Image, $"{RecipeDirectory}Mean_Image.tiff");
                    //HOperatorSet.ReadImage(out HObject Std_Image, $"{RecipeDirectory}Std_Image.tiff"); // 1206
                    HOperatorSet.WriteImage(Mean_Image, "tiff", 0, ModelsRecipeDirectory + "Mean_Image.tiff");
                    //HOperatorSet.WriteImage(Std_Image, "tiff", 0, ModelsRecipeDirectory + "Std_Image.tiff");
                }
                else
                {
                    MessageBox.Show("请生成均值方差图！");
                }


                if (goldenModelObject.LightImage != null && goldenModelObject.DarkImage != null)
                {
                    HOperatorSet.WriteImage(goldenModelObject.LightImage, "tiff", 0, ModelsRecipeDirectory + "Light_Image.tiff");
                    HOperatorSet.WriteImage(goldenModelObject.DarkImage, "tiff", 0, ModelsRecipeDirectory + "Dark_Image.tiff");
                }
                else if (File.Exists($"{RecipeDirectory}Light_Image.tiff") && File.Exists($"{RecipeDirectory}Dark_Image.tiff"))
                {
                    HOperatorSet.ReadImage(out HObject Light_Image, $"{RecipeDirectory}Light_Image.tiff");
                    HOperatorSet.ReadImage(out HObject Dark_Image, $"{RecipeDirectory}Dark_Image.tiff");
                    HOperatorSet.WriteImage(Light_Image, "tiff", 0, ModelsRecipeDirectory + "Light_Image.tiff");
                    HOperatorSet.WriteImage(Dark_Image, "tiff", 0, ModelsRecipeDirectory + "Dark_Image.tiff");
                }
                else
                {
                    MessageBox.Show("请生成亮图像、暗图像！");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public   override void LoadXML(string xmlPath)
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
                XElement goldenModelParameterrNode = root.Element("GoldenModelParameter");
                XElement goldenModelInspectParameterNode = root.Element("GoldenModelInspectParameter");
                if (goldenModelParameterrNode == null) return;
                XMLHelper.ReadParameters(goldenModelInspectParameterNode, goldenModelInspectParameter);
                XMLHelper.ReadParameters(goldenModelParameterrNode, goldenModelParameter);

                (cutOutDie as CutOutDie).LoadReferenceData();

                XMLHelper.ReadRegion(root, matchUserRegions, "Match_Region", goldenModelParameter.DieImageRowOffset, goldenModelParameter.DieImageColumnOffset);
                XMLHelper.ReadRegion(root, inspectUserRegions, "Inspect_Region", goldenModelParameter.DieImageRowOffset, goldenModelParameter.DieImageColumnOffset);
                XMLHelper.ReadRegion(root, rejectUserRegions, "Reject_Region", goldenModelParameter.DieImageRowOffset, goldenModelParameter.DieImageColumnOffset);
                XMLHelper.ReadRegion(root, subUserRegions, "Sub_Regions", goldenModelParameter.DieImageRowOffset, goldenModelParameter.DieImageColumnOffset, true);

                //1211
                foreach (var item in rejectUserRegions)
                {
                    if (item.RegionType == RegionType.Region)
                    {
                        HOperatorSet.ReadRegion(out HObject rejectRegion,$"{ ProductDirctory}\\{ item.RegionPath}");
                        item.CalculateRegion = rejectRegion;
                        HOperatorSet.MoveRegion(rejectRegion, out HObject _rejectRegion, -goldenModelParameter.DieImageRowOffset, -goldenModelParameter.DieImageColumnOffset);
                        item.DisplayRegion = _rejectRegion;
                    }
                }

                (goldenModelInspectVerify as GoldenModelInspectVerify).AddBondOnItem();
                (createGoldenModel as CreateGoldenModel).AddBondOnItem();
                (cutOutDie as CutOutDie).Initial();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "模板保存报错!");
            }
        }

        public override void Dispose()//new original
        {
            base.Dispose();
            Procedures = null;
        }
    }
}
