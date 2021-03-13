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
    class BondRecipe : ProcedureControl, IRecipe
    {
        //1121  using System.Collections.ObjectModel;
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;

        public const string XmlName = "BondRecipe.xml";

        public const string IdentifyString = "BONRECIPEXML";

        private string BondDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\BondMatch{RecipeIndex.ToString()}\\");

        private string ModelsBondDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\BondMatch{RecipeIndex.ToString()}\\");

        private string ModelsFile => $"{ProductDirctory}\\Models\\";

        private string RecipeFile => $"{ProductDirctory}\\Recipe\\";

        public string XmlPath => $"{BondDirectory}{XmlName}";

        public int RecipeIndex { get; } = 0;

        public string RecipeName { get; } = "BondRecipe";

        private string ReferenceDirectory => $"{ProductDirctory}\\Recipe\\Reference\\";

        //public static bool isRightClickBond = true;//

        private HTHalControlWPF htWindow;

        private Bond2ModelObject bond2ModelObject = new Bond2ModelObject();

        private Bond2ModelParameter bond2ModelParameter = new Bond2ModelParameter();

        private BondWireParameter bondWireParameter = new BondWireParameter();

        public BondVerifyModelPara BondVerifyModelPara = new BondVerifyModelPara();

        private ObservableCollection<Bond2Model> bond2Models = new ObservableCollection<Bond2Model>();

        private ObservableCollection<BondMatchAutoRegionGroup> Groups = new ObservableCollection<BondMatchAutoRegionGroup>();

        private ObservableCollection<BondWireRegionGroup> bondWireRegionGroups = new ObservableCollection<BondWireRegionGroup>();

        private ObservableCollection<BondMatchAutoRegionGroup> bondMatchAutoRegionGroups = new ObservableCollection<BondMatchAutoRegionGroup>();

        private ObservableCollection<UserRegion> BondVerifyUserRegions { get; set; } = new ObservableCollection<UserRegion>();//2021-01-11
        private ObservableCollection<UserRegion> bond2AutoUserRegion { get; set; } = new ObservableCollection<UserRegion>();

        public BondAutoRegionsParameter bondAutoRegionsParameter { get; private set; } = new BondAutoRegionsParameter();//自动生成部分

        #region 流程模块
        private IProcedure createBond2Model;
        private IProcedure addBondMatchRegions;
        private IProcedure bondMatchAutoRegions;
        private IProcedure bondMatchVerify;
        #endregion

        public BondRecipe(int bondMatchIndex)
        {
            this.RecipeIndex = bondMatchIndex;
            Content = new Page_CreateRecipe();
            (Content as Page_CreateRecipe).DataContext = this;
            htWindow = (Content as Page_CreateRecipe).htWindow;
            DisplayName = $"焊点Match{this.RecipeIndex.ToString()}模板";

            createBond2Model = new CreateBond2Model(htWindow,
                                                    ModelsBondDirectory,
                                                    ReferenceDirectory,
                                                    bond2ModelObject,
                                                    bond2Models,
                                                    bond2ModelParameter,
                                                    BondDirectory);

            addBondMatchRegions = new AddBondMatchRegions(htWindow,
                                                      bondWireRegionGroups,
                                                      bond2ModelObject,
                                                      bond2ModelParameter,
                                                      bond2AutoUserRegion,
                                                      ReferenceDirectory);

            bondMatchAutoRegions = new BondMatchAutoRegions(htWindow,
                              ModelsFile,
                              RecipeFile,
                              Groups,
                              bond2ModelObject,
                              bond2ModelParameter,
                              bondWireParameter,
                              bondAutoRegionsParameter,
                              bond2AutoUserRegion,
                              BondDirectory,
                              ReferenceDirectory);

            bondMatchVerify = new BondMatchVerify(htWindow,
                                                      ModelsFile,
                                                      RecipeFile,
                                                      bond2Models,
                                                      BondVerifyUserRegions,
                                                      bondWireRegionGroups,
                                                      bond2ModelObject,
                                                      bond2ModelParameter,
                                                      bondWireParameter,
                                                      BondVerifyModelPara,
                                                      BondDirectory,
                                                      ReferenceDirectory);

            (bondMatchVerify as BondMatchVerify).OnSaveXML += SaveXML;

            htWindow.useAutoBond(bondMatchAutoRegions as BondMatchAutoRegions);
            htWindow.useBond2Verify(bondMatchVerify as BondMatchVerify);

            Procedures = new IProcedure[]
            {
                createBond2Model,
                bondMatchAutoRegions,
                addBondMatchRegions,
                bondMatchVerify,
            };
            ProcedureChanged(0);
        }

        //保存
        public void SaveXML()
        {
            try
            {
                Directory.GetFiles(ModelsBondDirectory).ToList().ForEach(File.Delete);
                XElement root = new XElement("Recipe");
                XMLHelper.AddIdentifier(root, IdentifyString);
                XElement bond2ModelNode = new XElement("Bond2Models");
                HTuple Bond2_ModelImgIdxTemp = new HTuple();

                foreach (var model in bond2Models)
                {
                    XElement modelNode = new XElement("Model");
                    modelNode.Add(new XAttribute("Index", model.Index.ToString()));
                    //modelNode.Add(new XAttribute("ModelType", model.ModelType.ToString()));
                    modelNode.Add(new XAttribute("ModelIDPath", model.ModelIdPath));
                    modelNode.Add(new XElement("RotatedImagePath", model.RotatedImagePath));
                    modelNode.Add(new XElement("RotatedImageAngel", model.RotatedImageAngel));
                    modelNode.Add(new XAttribute("ImageIndex", model.ImageIndex.ToString()));

                    modelNode.Add(model.Bond2UserRegion?.ToXElement("Bond2ModelRegion"));
                    modelNode.Add(model.Bond2UserRegionDiff?.ToXElement("Bond2ModelRegionDiff"));
                    modelNode.Add(model.RotateLineUserRegion?.ToXElement("RotateLineRegion"));
                    XMLHelper.AddRegion(modelNode, model.RefineUserRegions, "RefineRegions");

                    bond2ModelNode.Add(modelNode);

                    if (bond2ModelParameter.ModelType == 0)
                    {
                        HOperatorSet.WriteTuple(new HTuple("ncc"), $"{ModelsBondDirectory}Model_Type.tup");
                    }
                    else if (bond2ModelParameter.ModelType == 1)
                    {
                        HOperatorSet.WriteTuple(new HTuple("shape"), $"{ModelsBondDirectory}Model_Type.tup");
                    }
                    model.RotatedImagePath = $"{BondDirectory}\\RotatedImage{model.Index.ToString()}.tiff";
                    HOperatorSet.ReadImage(out HObject rotatedImage, model.RotatedImagePath);
                    HOperatorSet.WriteImage(rotatedImage, "tiff", 0, $"{ModelsBondDirectory}RotatedImage{model.Index.ToString()}.tiff");
                }
                XElement bond2ModelParameterNode = new XElement("Bond2ModelParameter");
                XElement bondAutoRegionsParameterNode = new XElement("BondAutoRegionsParameter");//自动生成
                XElement bondWireParameterNode = new XElement("BondWireParameter");
                XElement BondVerifyModelParaNode = new XElement("BondVerifyModelPara");
                
                XMLHelper.AddParameters(bond2ModelParameterNode, bond2ModelParameter, IdentifyString);
                XMLHelper.AddParameters(bondWireParameterNode, bondWireParameter, IdentifyString);
                XMLHelper.AddParameters(BondVerifyModelParaNode, BondVerifyModelPara, IdentifyString);
                XMLHelper.AddParameters(bondAutoRegionsParameterNode, bondAutoRegionsParameter, IdentifyString);//自动生成

                root.Add(bond2ModelNode);
                root.Add(bond2ModelParameterNode);
                root.Add(bondWireParameterNode);
                root.Add(BondVerifyModelParaNode);//保存批量设置的模板检测参数
                root.Add(bondAutoRegionsParameterNode);//自动生成

                //XML保存检测区域带参数 add by wj
                XMLHelper.AddRegion(root, BondVerifyUserRegions, "BondVerifyUserRegions", false, false, false, false, true);//xml中加载生成的焊盘区域 2021-01-11

                try
                {
                    if (bond2ModelParameter.ModelIdPath.Split(',').Length >= 1 || bond2ModelObject.ModelID != null)
                    {
                        HTuple ModelId = new HTuple();

                        if (File.Exists($"{BondDirectory}PosModel.dat"))
                        {
                            if (bond2ModelObject.ModelID != null)
                            {
                                ModelId = bond2ModelObject.ModelID;
                            }
                            else if (File.Exists($"{BondDirectory}PosModel.dat"))
                            {
                                ModelId = Algorithm.File.ReadModel($"{BondDirectory}PosModel.dat", bond2ModelParameter.ModelType);
                            }
                            Algorithm.File.SaveModel($"{ModelsBondDirectory}PosModel.dat", bond2ModelParameter.ModelType, ModelId);
                        }
                    }
                    else
                    {
                        MessageBox.Show("请先创建焊点模板");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("焊点不存在或保存失败");
                }

                XElement GroupNode = new XElement("GenGroups");
                XElement bondWireGroupNode = new XElement("BondWireGroups");
                HTuple Bond2_BallNumsTemp = new HTuple();
                HTuple LineStartAngle = new HTuple();
                HOperatorSet.GenEmptyObj(out HObject Bond2_Regions);

                foreach (var group in Groups)
                {
                    XElement _groupNode = new XElement("GenGroup");
                    _groupNode.Add(new XAttribute("Index", group.Index.ToString()));
                    _groupNode.Add(new XAttribute("Bond2Numbers", group.Bond2_BallNums.ToString()));
                    _groupNode.Add(group.Bond2UserRegion?.ToXElement("Bond2Region"));
                    _groupNode.Add(group.WireUserRegion?.ToXElement("WireRegion"));
                    GroupNode.Add(_groupNode);
                }
                root.Add(GroupNode);

                foreach (var group in bondWireRegionGroups)
                {
                    XElement groupNode = new XElement("BondWireGroup");
                    groupNode.Add(new XAttribute("Index", group.Index.ToString()));
                    groupNode.Add(new XAttribute("Bond2Numbers", group.Bond2_BallNums.ToString()));
                    groupNode.Add(group.Bond2UserRegion?.ToXElement("Bond2Region"));
                    groupNode.Add(group.WireUserRegion?.ToXElement("WireRegion"));
                    bondWireGroupNode.Add(groupNode);

                    HOperatorSet.ConcatObj(Bond2_Regions, Algorithm.Region.ConcatRegion(group.Bond2UserRegion), out Bond2_Regions);
                    //Bond2_BallNumsTemp = Bond2_BallNumsTemp.TupleConcat(group.Bond2_BallNums);
                    //HOperatorSet.AngleLx(group.WireUserRegion.RegionParameters[0], group.WireUserRegion.RegionParameters[1], group.WireUserRegion.RegionParameters[2], group.WireUserRegion.RegionParameters[3],
                    //                    out HTuple lineAngle);
                    //HOperatorSet.TupleConcat(LineStartAngle, lineAngle, out LineStartAngle);
                }

                HOperatorSet.WriteRegion(Bond2_Regions, $"{ModelsBondDirectory}Inspect_Regions.reg");

                root.Add(bondWireGroupNode);
                root.Save(XmlPath);
                //
                try
                {
                    if (bond2ModelParameter.OnRecipesIndexs.Length > 0)
                    {
                        HOperatorSet.WriteTuple(bond2ModelParameter.OnRecipesIndexs[bond2ModelParameter.OnRecipesIndex], $"{ModelsBondDirectory}OnWhat.tup");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("请选择\"焊点在\"位置！");
                }

                //if (bond2ModelParameter.ImageCountChannels == 1)
                //{
                //    HOperatorSet.WriteTuple(new HTuple(1), ModelsBondDirectory + "Image_Index.tup");
                //}
                //else if (bond2ModelParameter.ImageCountChannels > 1)
                //{
                //    // 1122_lw
                //    HOperatorSet.WriteTuple(bondWireParameter.ImageChannelIndex + 1, ModelsBondDirectory + "Image_Index.tup");
                //}
                
                //HOperatorSet.WriteTuple(Bond2_BallNumsTemp, $"{ModelsBondDirectory}BallNum_OnRegion.tup");
                //HOperatorSet.WriteTuple(LineStartAngle, $"{ModelsBondDirectory}Match_StartAngle.tup");
                //HOperatorSet.WriteTuple(bondWireParameter.MinMatchScore, $"{ModelsBondDirectory}Match_MinScore.tup");// MinMatchScore
                //HOperatorSet.WriteTuple(bondWireParameter.AngleExt, $"{ModelsBondDirectory}Match_AngleExt.tup");//  AngleExt
                //if (bondWireParameter.IsCircleBond == true)
                //{
                //    HOperatorSet.WriteTuple(bondWireParameter.BondSize, $"{ModelsBondDirectory}BallRadius.tup");
                //}
                //else
                //{
                //    HOperatorSet.WriteTuple(new HTuple(bondWireParameter.EllipsBondSize[0]).TupleConcat(new HTuple(bondWireParameter.EllipsBondSize[1])), $"{ModelsBondDirectory}BallRadius.tup");
                //}
                //HOperatorSet.WriteTuple(bondWireParameter.IsBondRegRefine == false ? 0 : 1, $"{ModelsBondDirectory}IsBondRegRefine.tup");
                //HOperatorSet.WriteTuple(bondWireParameter.AddBallNum, $"{ModelsBondDirectory}AddBallNum.tup");
                //HOperatorSet.WriteTuple(bondWireParameter.MaxOverlap, $"{ModelsBondDirectory}MaxOverlap.tup");
                //HOperatorSet.WriteTuple(bondWireParameter.MinHistScore, $"{ModelsBondDirectory}MinHistScore.tup");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //加载
        public void LoadXML(string xmlPath)
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
                XElement bond2ModelParameterNode = root.Element("Bond2ModelParameter");
                if (bond2ModelParameterNode == null) return;
                XElement bond2ModelNode = root.Element("Bond2Models");
                if (bond2ModelNode == null) return;
                XMLHelper.ReadParameters(bond2ModelParameterNode, bond2ModelParameter);
                (createBond2Model as CreateBond2Model).LoadReferenceData();

                bond2Models.Clear();
                bondWireRegionGroups.Clear();
                Groups.Clear();
                XElement bondWireParameterNode = root.Element("BondWireParameter");
                XMLHelper.ReadParameters(bondWireParameterNode, bondWireParameter);

                XElement bondAutoRegionsParameterNode = root.Element("BondAutoRegionsParameter");//自动
                XMLHelper.ReadParameters(bondAutoRegionsParameterNode, bondAutoRegionsParameter);//自动

                //bond2Models.Clear();放前面
                foreach (var modelNode in bond2ModelNode.Elements("Model"))
                {
                    Bond2Model model = new Bond2Model();
                    model.ModelIdPath = modelNode.Attribute("ModelIDPath")?.Value;
                    model.Index = (int.Parse)(modelNode.Attribute("Index")?.Value);
                    model.RotatedImagePath = modelNode.Element("RotatedImagePath")?.Value;
                    model.Bond2UserRegion = UserRegion.FromXElement(modelNode.Element("Bond2ModelRegion"));
                    model.RotateLineUserRegion = UserRegion.FromXElement(modelNode.Element("RotateLineRegion"));
                    model.Bond2UserRegionDiff = UserRegion.FromXElement(modelNode.Element("Bond2ModelRegionDiff"));//
                    XMLHelper.ReadRegion(modelNode, model.RefineUserRegions, "RefineRegions");
                    bond2Models.Add(model);
                    model.Index = bond2Models.Count;
                }
               (createBond2Model as CreateBond2Model).ModelsCount = bond2Models.Count;

                // 自动焊点区域部分
                XElement GenGroupsNode = root.Element("GenGroups");
                if (GenGroupsNode != null)
                { 
                    foreach (var groupNode in GenGroupsNode.Elements("GenGroup"))
                    {
                        BondMatchAutoRegionGroup group = new BondMatchAutoRegionGroup();
                        group.Bond2UserRegion = UserRegion.FromXElement(groupNode.Element("Bond2Region"), false, bond2ModelParameter.DieImageRowOffset, bond2ModelParameter.DieImageColumnOffset);
                        group.WireUserRegion = UserRegion.FromXElement(groupNode.Element("WireRegion"), false, bond2ModelParameter.DieImageRowOffset, bond2ModelParameter.DieImageColumnOffset);
                        //XMLHelper.ReadParameters(groupNode, group.Parameter);
                        Groups.Add(group);
                        group.Index = Groups.Count;
                        group.Bond2_BallNums = int.Parse(groupNode.Attribute("Bond2Numbers")?.Value);
                    }
                }

                XElement bondWireGroupNode = root.Element("BondWireGroups");
                if (bondWireGroupNode == null) return;

                foreach (var groupNode in bondWireGroupNode.Elements("BondWireGroup"))
                {
                    BondWireRegionGroup group = new BondWireRegionGroup();
                    group.Bond2UserRegion = UserRegion.FromXElement(groupNode.Element("Bond2Region"), false, bond2ModelParameter.DieImageRowOffset, bond2ModelParameter.DieImageColumnOffset);
                    group.WireUserRegion = UserRegion.FromXElement(groupNode.Element("WireRegion"), false, bond2ModelParameter.DieImageRowOffset, bond2ModelParameter.DieImageColumnOffset);                                                                                                                                                                                          //XMLHelper.ReadParameters(groupNode, group.Parameter);
                    bondWireRegionGroups.Add(group);
                    group.Index = bondWireRegionGroups.Count;
                    group.Bond2_BallNums = int.Parse(groupNode.Attribute("Bond2Numbers")?.Value);
                }

                (bondMatchAutoRegions as BondMatchAutoRegions).GroupsCount = Groups.Count;
                (addBondMatchRegions as AddBondMatchRegions).GroupsCount = bondWireRegionGroups.Count;

                (createBond2Model as CreateBond2Model).Initial();

                //加载焊点检测区域及其参数
                XMLHelper.ReadRegion(root, BondVerifyUserRegions, "BondVerifyUserRegions", bond2ModelParameter.DieImageRowOffset, bond2ModelParameter.DieImageColumnOffset, false, false, false, false, true);
                //
                XElement BondVerifyModelParaNode = root.Element("BondVerifyModelPara");
                XMLHelper.ReadParameters(BondVerifyModelParaNode, BondVerifyModelPara);
                // 兼容老产品打开参数不保留 2021-0105 by wj
                if (BondVerifyUserRegions.Count() != 0)
                {
                    //更新PadUserRegions
                    for (int i = 0; i < BondVerifyUserRegions.Count; i++)
                    {
                        //检测区域内通道值设置
                        if (BondVerifyUserRegions.ElementAt(i).ChannelNames.Count() == 0)
                        {
                            BondVerifyUserRegions.ElementAt(i).ChannelNames = bond2ModelParameter.ChannelNames;

                        }
                    }
                }

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
            (bondMatchVerify as BondMatchVerify).OnSaveXML -= SaveXML;
            Content = null;
            bond2Models = null;
            BondVerifyUserRegions = null;
            bond2ModelParameter = null;
            BondVerifyModelPara = null;
            bondWireParameter = null;
            bondWireRegionGroups = null;
            createBond2Model.Dispose();
            addBondMatchRegions.Dispose();
            bondMatchAutoRegions.Dispose();
            bondMatchVerify.Dispose();
            Procedures = null;
        }
    }
}
