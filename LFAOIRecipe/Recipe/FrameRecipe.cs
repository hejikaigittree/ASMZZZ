using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HalconDotNet;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Xml.Linq;

namespace LFAOIRecipe
{
    sealed class FrameRecipe : GoldenModelRecipe
    {

        public const string XmlName = "FrameRecipe.xml";

        public const string IdentifyString = "FRAMERECIPEXML";

        public override string DisplayName { get; }

        protected override string RecipeDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\Frame{RecipeIndex.ToString()}\\");

        protected override string ModelsRecipeDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\Frame{RecipeIndex.ToString()}\\");

        public override string XmlPath => $"{RecipeDirectory}{XmlName}";

        public override int RecipeIndex { get; }

        public override string RecipeName => "FrameRecipe";

        public FrameRecipe(int RecipeIndex)
        {
            this.RecipeIndex = RecipeIndex;
            DisplayName = $"框架{this.RecipeIndex.ToString()}模板";
            (Content as Page_CreateRecipe).DataContext = this;

            InitialProcedures();
            Procedures = new IProcedure[]
            {
                cutOutDieFrame,
                addMatchRegionFrame,
                createPositionModel,
                frameLocateInspectVerify
            };
            (frameLocateInspectVerify as FrameLocateInspectVerify).OnSaveXML += SaveXML;
            ProcedureChanged(0);
        }

        public override void SaveXML()
        {
            try
            {
                // 保留表面检测保存区域 0113 lw
                HObject ho_Frame_Region = null;
                if (File.Exists($"{ModelsRecipeDirectory}Frame_Region.reg"))
                {                    
                    HOperatorSet.ReadRegion(out ho_Frame_Region, ModelsRecipeDirectory + "Frame_Region.reg");
                }

                HObject ho_PegRack_Region = null;
                if (File.Exists($"{ModelsRecipeDirectory}PegRack_Region.reg"))
                {                   
                    HOperatorSet.ReadRegion(out ho_PegRack_Region, ModelsRecipeDirectory + "PegRack_Region.reg");
                }

                HObject ho_Bridge_Region = null;
                if (File.Exists($"{ModelsRecipeDirectory}Bridge_Region.reg"))
                {                    
                    HOperatorSet.ReadRegion(out ho_Bridge_Region, ModelsRecipeDirectory + "Bridge_Region.reg");
                }

                //Models删除文件
                Directory.GetFiles(ModelsRecipeDirectory).ToList().ForEach(File.Delete);

                if(ho_Frame_Region != null)
                {
                    HOperatorSet.WriteRegion(ho_Frame_Region, ModelsRecipeDirectory + "Frame_Region.reg");
                    ho_Frame_Region.Dispose();
                }
                if (ho_PegRack_Region != null)
                {
                    HOperatorSet.WriteRegion(ho_PegRack_Region, ModelsRecipeDirectory + "PegRack_Region.reg");
                    ho_PegRack_Region.Dispose();
                }
                if (ho_Bridge_Region != null)
                {
                    HOperatorSet.WriteRegion(ho_Bridge_Region, ModelsRecipeDirectory + "Bridge_Region.reg");
                    ho_Bridge_Region.Dispose();
                }

                XElement root = new XElement("Recipe");
                XElement goldenModelParameterNode = new XElement("GoldenModelParameter");
                XElement frameLocateInspectParameterNode = new XElement("FrameLocateParameter");
                XMLHelper.AddIdentifier(root, IdentifyString);
                XMLHelper.AddRegion(root, dieUserRegions, "Die_Regions");

                //多框架区域保存
                XElement matchRegionGroupsNode = new XElement("MatchRegionGroup");
                foreach (var group in MatchRegionsGroups)
                {
                    XElement matchRegionNode = new XElement("MatchRegion");
                    matchRegionNode.Add(new XAttribute("Index", group.Index.ToString()));
                    XMLHelper.AddRegion(matchRegionNode, group.MatchUserRegions, "Region");
                    matchRegionGroupsNode.Add(matchRegionNode);
                }
                //框架区域保存
                XMLHelper.AddRegion(root, FrameUserRegions, "Frame_Region");

                XMLHelper.AddParameters(goldenModelParameterNode, goldenModelParameter, IdentifyString);
                XMLHelper.AddParameters(frameLocateInspectParameterNode, frameLocateInspectParameter, IdentifyString);
                root.Add(goldenModelParameterNode);
                root.Add(frameLocateInspectParameterNode);
                root.Add(matchRegionGroupsNode);
                root.Save(XmlPath);

                if (goldenModelParameter.ImageCountChannels == 1)
                {
                    HOperatorSet.WriteTuple(new HTuple(1), ModelsRecipeDirectory + "Image_Index.tup");
                }
                else if (goldenModelParameter.ImageCountChannels > 1)
                {
                    //1121
                    HOperatorSet.WriteTuple(new HTuple(goldenModelParameter.ImageChannelIndex + 1), ModelsRecipeDirectory + "Image_Index.tup");
                }

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

            //try
            //{
            //    if (File.Exists($"{RecipeDirectory}PosModel.dat")|| goldenModelObject.PosModelID != null)
            //    {
            //        if (goldenModelObject.PosModelID != null)
            //        {
            //            Algorithm.File.SaveModel(ModelsRecipeDirectory + "PosModel.dat", goldenModelParameter.ModelType, goldenModelObject.PosModelID);
            //        }
            //        else if (File.Exists($"{RecipeDirectory}PosModel.dat"))
            //        {
            //            HTuple PosModelId = Algorithm.File.ReadModel($"{RecipeDirectory}PosModel.dat", goldenModelParameter.ModelType);
            //            Algorithm.File.SaveModel(ModelsRecipeDirectory + "PosModel.dat", goldenModelParameter.ModelType, PosModelId);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("请创建定位模板！");
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.ToString(),"保存模板错误！");
            //}

            try
            {
                HOperatorSet.WriteTuple(frameLocateInspectParameter.AngleStart, FilePath.EnsureDirectoryExisted($"{ModelsRecipeDirectory}") + "AngleStart.tup");
                HOperatorSet.WriteTuple(frameLocateInspectParameter.AngleExt, FilePath.EnsureDirectoryExisted($"{ModelsRecipeDirectory}") + "AngleExt.tup");
                HOperatorSet.WriteTuple(frameLocateInspectParameter.MinMatchScore, FilePath.EnsureDirectoryExisted($"{ModelsRecipeDirectory}") + "MinMatchScore.tup");
                HOperatorSet.WriteTuple(frameLocateInspectParameter.MatchNum, FilePath.EnsureDirectoryExisted($"{ModelsRecipeDirectory}") + "MatchNum.tup");

                //HOperatorSet.WriteRegion(Algorithm.Region.Union1Region(matchUserRegions), ModelsRecipeDirectory + "Match_Region.reg");
                HOperatorSet.GenEmptyObj(out HObject matchRegions);
                foreach (var item in MatchRegionsGroups)
                {
                    // lw - 0115  每组区域应Union
                    HOperatorSet.ConcatObj(Algorithm.Region.Union1Region(item.MatchUserRegions.Where(r => r.IsEnable)), matchRegions, out matchRegions);
                }
                HOperatorSet.WriteRegion(matchRegions, ModelsRecipeDirectory + "Match_Region.reg");
                matchRegions.Dispose();

                if (FrameUserRegions.Count > 0)
                {
                    HOperatorSet.WriteRegion(FrameUserRegions[0].CalculateRegion, ModelsRecipeDirectory + "FrameRegion.reg");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //保存模板
            string[] ModelPathArr = goldenModelParameter.PosModelIdPath.Split(',');

            if (goldenModelObject.PosModelID == null && File.Exists(ModelPathArr[0]))
            {
                goldenModelObject.PosModelID = Algorithm.File.ReadModel(ModelPathArr, goldenModelParameter.ModelType);
            }
            else if(goldenModelObject.PosModelID == null)
            {
                MessageBox.Show("请先创建定位模板！");
                return;
            }
            if (goldenModelObject.PosModelID.Length == 1)
            {
                string ModelsIDPath = $"{ModelsRecipeDirectory}PosModel.dat";
                Algorithm.File.SaveModel(ModelsIDPath, goldenModelParameter.ModelType, goldenModelObject.PosModelID);
            }
            else if (goldenModelObject.PosModelID.Length > 1)
            {
                String[] ModelIdPathArry = new string[goldenModelObject.PosModelID.Length];
                for (int i = 0; i < goldenModelParameter.PosModelIdPath.Split(',').Length; i++)
                {
                    ModelIdPathArry[i] = $"{ModelsRecipeDirectory}PosModel_" + i + ".dat";
                }
                Algorithm.File.SaveModel(ModelIdPathArry, goldenModelParameter.ModelType, goldenModelObject.PosModelID);//
            }
            else
            {
                MessageBox.Show("请先创建模板！");
            }

            //try
            //{
            //    if (($"{RecipeDirectory}PosModel.dat").Contains("PosModel"))
            //    {
            //        if (goldenModelParameter.ModelIdPath.Split(',').Length == 1 && goldenModelParameter.ModelIdPath != "")
            //        {
            //            if (File.Exists($"{RecipeDirectory}PosModel.dat") || goldenModelObject.PosModelID != null)
            //            {
            //                if (goldenModelObject.PosModelID != null)
            //                {
            //                    Algorithm.File.SaveModel(ModelsRecipeDirectory + "PosModel.dat", goldenModelParameter.ModelType, goldenModelObject.PosModelID);
            //                }
            //                else if (File.Exists($"{RecipeDirectory}PosModel.dat"))
            //                {
            //                    HTuple PosModelId = Algorithm.File.ReadModel($"{RecipeDirectory}PosModel.dat", goldenModelParameter.ModelType);
            //                    Algorithm.File.SaveModel(ModelsRecipeDirectory + "PosModel.dat", goldenModelParameter.ModelType, PosModelId);
            //                }
            //            }
            //            else
            //            {
            //                MessageBox.Show("请创建定位模板！");
            //            }
            //        }
            //        else
            //        {
            //            if (goldenModelObject.PosModelID != null)
            //            {
            //                String[] ModelIdPathArry = new string[goldenModelObject.PosModelID.TupleLength()];
            //                for (int i = 0; i < goldenModelObject.PosModelID.TupleLength(); i++)
            //                {
            //                    ModelIdPathArry[i] = $"{ModelsRecipeDirectory}PosModel_" + i + ".dat";
            //                }
            //                goldenModelParameter.ModelIdPath = String.Join(",", ModelIdPathArry);
            //                Algorithm.File.SaveModel(ModelIdPathArry, goldenModelParameter.ModelType, goldenModelObject.PosModelID);
            //            }
            //            else if (File.Exists($"{RecipeDirectory}PosModel.dat"))
            //            {
            //                HTuple ModelId = Algorithm.File.ReadModel($"{RecipeDirectory}PosModel.dat".Split(','), goldenModelParameter.ModelType);
            //                String[] ModelIdPathArry = new string[ModelId.TupleLength()];
            //                for (int i = 0; i < ModelId.TupleLength(); i++)
            //                {
            //                    ModelIdPathArry[i] = $"{ModelsRecipeDirectory}PosModel_" + i + ".dat";
            //                }
            //                Algorithm.File.SaveModel(ModelIdPathArry, goldenModelParameter.ModelType, ModelId);
            //            }
            //        }
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        }

        public override void LoadXML(string xmlPath)//async
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
                XElement frameLocateParameterNode = root.Element("FrameLocateParameter");
                if (goldenModelParameterrNode == null) return;
                XMLHelper.ReadParameters(goldenModelParameterrNode, goldenModelParameter);
                (cutOutDieFrame as CutOutDieFrame).LoadReferenceData();
                XMLHelper.ReadParameters(frameLocateParameterNode, frameLocateInspectParameter);

                //XMLHelper.ReadRegion(root, matchUserRegions, "Match_Region", goldenModelParameter.DieImageRowOffset, goldenModelParameter.DieImageColumnOffset);
                //12-05
                XElement matchRegionGroupsNode = root.Element("MatchRegionGroup");
                if (matchRegionGroupsNode == null) return;
                MatchRegionsGroups.Clear();
                foreach (var groupNode in matchRegionGroupsNode.Elements("MatchRegion"))
                {
                    MatchRegionsGroup group = new MatchRegionsGroup();
                    XMLHelper.ReadRegion(groupNode, group.MatchUserRegions, "Region", goldenModelParameter.DieImageRowOffset, goldenModelParameter.DieImageColumnOffset);
                    MatchRegionsGroups.Add(group);
                    group.Index = MatchRegionsGroups.Count;
                }
                (addMatchRegionFrame as AddMatchRegionFrame).GroupsCount = MatchRegionsGroups.Count;

                //框架区域
                XMLHelper.ReadRegion(root, FrameUserRegions, "Frame_Region", goldenModelParameter.DieImageRowOffset, goldenModelParameter.DieImageColumnOffset);

                (cutOutDieFrame as CutOutDieFrame).Initial();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Procedures = null;
        }
    }
}
