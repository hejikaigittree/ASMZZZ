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
    class BondMeasureRecipe : ProcedureControl, IRecipe
    {
        //1121  using System.Collections.ObjectModel;
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;

        public const string XmlName = "BondMeasureRecipe.xml";

        public const string IdentifyString = "BONDMEASURERECIPEXML";

        private string BondMeasureDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\BondMeasure{RecipeIndex.ToString()}\\");

        private string ModelsBondDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\BondMeasure{RecipeIndex.ToString()}\\");//

        public string XmlPath => $"{BondMeasureDirectory}{XmlName}";

        public int RecipeIndex { get; }

        public string RecipeName { get; } = "BondMeasureRecipe";

        private string ReferenceDirectory => $"{ProductDirctory}\\Recipe\\Reference\\";

        private string ModelsFile => $"{ProductDirctory}\\Models\\";

        private string RecipeFile => $"{ProductDirctory}\\Recipe\\";

        //public static bool isRightClickBond = true;

        private HTHalControlWPF htWindow;

        private BondMeasureModelObject bondMeasureModelObject = new BondMeasureModelObject();

        private BondMeasureParameter bondMeasureParameter = new BondMeasureParameter();

        public Bond1AutoRegionsParameter bond1AutoRegionsParameter { get; private set; } = new Bond1AutoRegionsParameter();//自动生成部分

        private BondMeasureVerifyParameter bondMeasureVerifyParameter = new BondMeasureVerifyParameter();

        private BondMeasureVerifyParameterSet bondMeasureVerifyParameterSet = new BondMeasureVerifyParameterSet();
        //add by wj 2021-01-07
        public BondVerifyModelPara bondVerifyModelPara = new BondVerifyModelPara();

        private ObservableCollection<UserRegion> bondModelUserRegions = new ObservableCollection<UserRegion>();

        private ObservableCollection<UserRegion> bond1AutoUserRegion = new ObservableCollection<UserRegion>();

        private ObservableCollection<UserRegion> PadUserRegions { get; set; } = new ObservableCollection<UserRegion>();


        private ObservableCollection<BondWireRegionGroup> bondWireRegionGroups = new ObservableCollection<BondWireRegionGroup>();

        public ObservableCollection<UserRegion> Bond2UserRegion { get; private set; } = new ObservableCollection<UserRegion>();

        public ObservableCollection<UserRegion> Bond2UserRegionDiff { get; private set; } = new ObservableCollection<UserRegion>();
        
        public ObservableCollection<UserRegion> RotateLineUserRegion { get; private set; } = new ObservableCollection<UserRegion>(); 

        #region 流程模块
        private IProcedure createAutoBondMeasureModel;
        private IProcedure createBondMeasureModel;
        private IProcedure bondMeasureVerify;
        #endregion

        public BondMeasureRecipe(int bondMeasureIndex)
        {
            this.RecipeIndex = bondMeasureIndex;
            Content = new Page_CreateRecipe();
            (Content as Page_CreateRecipe).DataContext = this;
            htWindow = (Content as Page_CreateRecipe).htWindow;
            DisplayName = $"焊点Measure{this.RecipeIndex.ToString()}模板";

            createAutoBondMeasureModel = new CreateAutoBondMeasureModel(htWindow,
                                                    ModelsFile,
                                                    RecipeFile,
                                                    ReferenceDirectory,
                                                    bondWireRegionGroups,
                                                    bondMeasureModelObject,
                                                    bondMeasureParameter,
                                                    bond1AutoRegionsParameter,
                                                    bondModelUserRegions,
                                                    bond1AutoUserRegion,
                                                    RotateLineUserRegion,
                                                    BondMeasureDirectory,
                                                    ModelsBondDirectory,
                                                    Bond2UserRegion,
                                                    Bond2UserRegionDiff);

            createBondMeasureModel = new CreateBondMeasureModel(htWindow,
                                        ReferenceDirectory,
                                        bondMeasureModelObject,
                                        bondMeasureParameter,
                                        bondModelUserRegions,
                                        bond1AutoUserRegion,
                                        BondMeasureDirectory,
                                        ModelsBondDirectory);

            bondMeasureVerify = new BondMeasureVerify(htWindow,
                                                        ModelsFile,
                                                        RecipeFile,
                                                        ReferenceDirectory,
                                                        bondMeasureModelObject,
                                                        bondMeasureParameter,
                                                        bondMeasureVerifyParameter,
                                                        bondMeasureVerifyParameterSet,
                                                        bondVerifyModelPara,
                                                        PadUserRegions,
                                                        bondModelUserRegions,
                                                        BondMeasureDirectory,
                                                        ModelsBondDirectory);

            (bondMeasureVerify as BondMeasureVerify).OnSaveXML += SaveXML;

            htWindow.useBondVerifyAndPad(bondMeasureVerify as BondMeasureVerify);

            htWindow.useAutoBond1(createAutoBondMeasureModel as CreateAutoBondMeasureModel);

            

            Procedures = new IProcedure[]
            {
                createAutoBondMeasureModel,
                createBondMeasureModel,
                bondMeasureVerify
            };
            ProcedureChanged(0);
        }

        public void SaveXML()
        {
            try
            {
                Directory.GetFiles(ModelsBondDirectory).ToList().ForEach(File.Delete);
                XElement root = new XElement("Recipe");
                XMLHelper.AddIdentifier(root, IdentifyString);
                XElement bondMeasureParameterNode = new XElement("BondMeasureParameter");
                XElement bond1AutoRegionsParameterNode = new XElement("Bond1AutoRegionsParameter");//自动生成
                XMLHelper.AddParameters(bondMeasureParameterNode, bondMeasureParameter, IdentifyString);
                if(bond1AutoRegionsParameter.RotatedImagePath == null)
                {
                    // 1122
                    bond1AutoRegionsParameter.RotatedImagePath = "";
                }
                XMLHelper.AddParameters(bond1AutoRegionsParameterNode, bond1AutoRegionsParameter, IdentifyString);//自动生成
                XMLHelper.AddRegion(root, bondModelUserRegions, "BondModelUserRegions", true);

                //XMLHelper.AddRegion(root, BondVerifyUserRegions, "BondVerifyUserRegions");//自动生成焊点检测区域
                //
                XMLHelper.AddRegion(root, PadUserRegions, "PadUserRegions",false,false,false,false,true);//xml中加载生成的焊盘区域 2021-01-05

                XElement BondVerifyModelParaNode = new XElement("BondVerifyModelPara");
                XMLHelper.AddParameters(BondVerifyModelParaNode, bondVerifyModelPara, IdentifyString);
                root.Add(BondVerifyModelParaNode);//保存批量设置的模板检测参数
                //models下保存焊盘区域                                                                                                        
                HOperatorSet.WriteRegion(Algorithm.Region.ConcatRegion(PadUserRegions), ModelsBondDirectory + "Pad_Regions.reg");//add by wj 2021-0105


                XMLHelper.AddRegion(root, Bond2UserRegion, "Bond2UserRegion");
                XMLHelper.AddRegion(root, Bond2UserRegionDiff, "Bond2UserRegionDiff");
                XMLHelper.AddRegion(root, RotateLineUserRegion, "RotateLineUserRegion");

                XElement BondMeasureVerifyParameterNode = new XElement("BondMeasureVerifyParameter");
                XElement bondWireGroupNode = new XElement("BondWireGroups");
                XMLHelper.AddParameters(BondMeasureVerifyParameterNode, bondMeasureVerifyParameter, IdentifyString);
                root.Add(bondMeasureParameterNode);
                root.Add(bond1AutoRegionsParameterNode);//自动生成
                root.Add(BondMeasureVerifyParameterNode);
                //自动生成焊点区域部分
                foreach (var group in bondWireRegionGroups)
                {
                    XElement groupNode = new XElement("BondWireGroup");
                    groupNode.Add(new XAttribute("Index", group.Index.ToString()));
                    groupNode.Add(new XAttribute("Bond2Numbers", group.Bond2_BallNums.ToString()));
                    groupNode.Add(group.Bond2UserRegion?.ToXElement("Bond2Region"));
                    groupNode.Add(group.WireUserRegion?.ToXElement("WireRegion"));
                    bondWireGroupNode.Add(groupNode);
                }
                root.Add(bondWireGroupNode);
                root.Save(XmlPath);

                //if (bondMeasureParameter.ImageCountChannels == 1)
                //{
                //    HOperatorSet.WriteTuple(new HTuple(1), ModelsBondDirectory + "Image_Index.tup");
                //}
                //else if (bondMeasureParameter.ImageCountChannels > 1)
                //{
                //    HOperatorSet.WriteTuple(bondMeasureVerifyParameter.ImageChannelIndex + 1, ModelsBondDirectory + "Image_Index.tup");
                //}

                //HOperatorSet.WriteRegion(Algorithm.Region.ConcatRegion(BondVerifyUserRegions), ModelsBondDirectory + "Inspect_Regions.reg");

                //HTuple bondOffsetFactor = new HTuple();
                //HTuple bondOverSizeFactor = new HTuple();
                //HTuple bondUnderSizeFactor = new HTuple();
                //HTuple PreJudgeEnable = new HTuple();
                //HTuple SegThreshGray = new HTuple();
                //HTuple SegRegAreaFactor = new HTuple();
                HTuple DistanceThreshold = new HTuple();

                foreach (var item in bondModelUserRegions)
                {
                    //    //HOperatorSet.TupleConcat(bondOffsetFactor,item.BondMeasureVerifyParameterSet.BondOffsetFactor, out bondOffsetFactor);
                    //    HOperatorSet.TupleConcat(bondOverSizeFactor, item.BondMeasureVerifyParameterSet.BondOverSizeFactor, out bondOverSizeFactor);
                    //    HOperatorSet.TupleConcat(bondUnderSizeFactor, item.BondMeasureVerifyParameterSet.BondUnderSizeFactor, out bondUnderSizeFactor);
                    //    HOperatorSet.TupleConcat(PreJudgeEnable, item.BondMeasureVerifyParameterSet.PreJudgeEnable, out PreJudgeEnable);
                    //    HOperatorSet.TupleConcat(SegThreshGray, item.BondMeasureVerifyParameterSet.SegThreshGray, out SegThreshGray);
                    //    HOperatorSet.TupleConcat(SegRegAreaFactor, item.BondMeasureVerifyParameterSet.SegRegAreaFactor, out SegRegAreaFactor);
                    HOperatorSet.TupleConcat(DistanceThreshold, item.BondMeasureModelParameter.DistanceThreshold, out DistanceThreshold);
                }
                //HOperatorSet.WriteTuple(bondOffsetFactor, $"{ModelsBondDirectory}BondOffsetFactor.tup");
                //HOperatorSet.WriteTuple(bondOverSizeFactor, $"{ModelsBondDirectory}BondOverSizeFactor.tup");
                //HOperatorSet.WriteTuple(bondUnderSizeFactor, $"{ModelsBondDirectory}BondUnderSizeFactor.tup");
                //HOperatorSet.WriteTuple(PreJudgeEnable, $"{ModelsBondDirectory}PreJudgeEnable.tup");
                //HOperatorSet.WriteTuple(SegThreshGray, $"{ModelsBondDirectory}SegThreshGray.tup");
                //HOperatorSet.WriteTuple(SegRegAreaFactor, $"{ModelsBondDirectory}SegRegAreaFactor.tup");
                HOperatorSet.WriteTuple(DistanceThreshold, $"{ModelsBondDirectory}MetrologyDistanceThreshold.tup");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            try
            {
                if (bondMeasureParameter.OnRecipesIndexs.Length > 0)
                {
                    HOperatorSet.WriteTuple(bondMeasureParameter.OnRecipesIndexs[bondMeasureParameter.OnRecipesIndex], $"{ModelsBondDirectory}OnWhat.tup");
                }
            }
            catch (Exception)//改
            {
                MessageBox.Show("请选择\"焊点在\"位置！");
            }

            try
            {
                if (bondMeasureModelObject.MetrologyHandle != null)
                {
                    HOperatorSet.WriteMetrologyModel(bondMeasureModelObject.MetrologyHandle, $"{ModelsBondDirectory}" + "MetrologyHandle.mtr");
                }
                else if (File.Exists($"{BondMeasureDirectory}MetrologyHandle.mtr"))
                {
                    HOperatorSet.ReadMetrologyModel($"{BondMeasureDirectory}MetrologyHandle.mtr", out HTuple metrologyHandle);
                    HOperatorSet.WriteMetrologyModel(metrologyHandle, $"{ModelsBondDirectory}" + "MetrologyHandle.mtr");
                }
                HOperatorSet.WriteTuple(Algorithm.Region.MetrologyType(bondModelUserRegions), $"{ModelsBondDirectory}MetrologyType.tup");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "模板句柄保存异常！");
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
                XElement bondMeasureParameterNode = root.Element("BondMeasureParameter");
                XElement bond1AutoRegionsParameterNode = root.Element("Bond1AutoRegionsParameter");//自动
                if (bondMeasureParameterNode == null) return;
                XMLHelper.ReadParameters(bondMeasureParameterNode, bondMeasureParameter);
                XMLHelper.ReadParameters(bond1AutoRegionsParameterNode, bond1AutoRegionsParameter);//自动
                (createBondMeasureModel as CreateBondMeasureModel).LoadReferenceData();
                XMLHelper.ReadRegion(root, bondModelUserRegions, "BondModelUserRegions", 0, 0, true);

                //XMLHelper.ReadRegion(root, BondVerifyUserRegions, "BondVerifyUserRegions", bondMeasureParameter.DieImageRowOffset, bondMeasureParameter.DieImageColumnOffset);

                XMLHelper.ReadRegion(root, PadUserRegions, "PadUserRegions", bondMeasureParameter.DieImageRowOffset, bondMeasureParameter.DieImageColumnOffset,false,false,false,false,true);
                //
                XElement BondVerifyModelParaNode = root.Element("BondVerifyModelPara");
                XMLHelper.ReadParameters(BondVerifyModelParaNode, bondVerifyModelPara);
                // 兼容老产品打开参数不保留 2021-0105 by wj
                if (PadUserRegions.Count() != 0)
                {
                    //add by wj 2021-01-05
                    string regionPathName;
                    string regionName = "Pad_Regions";
                    regionPathName = $"\\{regionName}.reg";

                    HOperatorSet.ReadRegion(out HObject freeRegion, $"{ModelsBondDirectory}\\{ regionPathName}");

                    //更新PadUserRegions
                    for (int i = 0; i < PadUserRegions.Count; i++)
                    {
                        //检测区域内通道值设置
                        if(PadUserRegions.ElementAt(i).ChannelNames.Count() == 0)
                        {
                            PadUserRegions.ElementAt(i).ChannelNames = bondMeasureParameter.ChannelNames;

                        }
                        if (PadUserRegions[i].RegionType == RegionType.Region)
                        {
                            HOperatorSet.SelectObj(freeRegion, out HObject padFreeRegion, i+1);
                            PadUserRegions[i].CalculateRegion = padFreeRegion;
                            HOperatorSet.MoveRegion(padFreeRegion, out HObject _padFreeRegion, -bondMeasureParameter.DieImageRowOffset, -bondMeasureParameter.DieImageColumnOffset);
                            PadUserRegions[i].DisplayRegion = _padFreeRegion;
                        }
                    }
                }

                XMLHelper.ReadRegion(root, Bond2UserRegion, "Bond2UserRegion", 0, 0, true);
                XMLHelper.ReadRegion(root, Bond2UserRegionDiff, "Bond2UserRegionDiff", 0, 0, true);
                XMLHelper.ReadRegion(root, RotateLineUserRegion, "RotateLineUserRegion", 0, 0, true);

                XElement BondMeasureVerifyParameterNode = root.Element("BondMeasureVerifyParameter");
                XMLHelper.ReadParameters(BondMeasureVerifyParameterNode, bondMeasureVerifyParameter);
                (createBondMeasureModel as CreateBondMeasureModel).Initial();

                bondWireRegionGroups.Clear();
                XElement bondWireGroupNode = root.Element("BondWireGroups");
                if (bondWireGroupNode == null) return;
                //自动生成焊点区域部分
                foreach (var groupNode in bondWireGroupNode.Elements("BondWireGroup"))
                {
                    BondWireRegionGroup group = new BondWireRegionGroup();
                    group.Bond2UserRegion = UserRegion.FromXElement(groupNode.Element("Bond2Region"), false);
                    group.WireUserRegion = UserRegion.FromXElement(groupNode.Element("WireRegion"), false);                                                                                                                                                                                          //XMLHelper.ReadParameters(groupNode, group.Parameter);
                    bondWireRegionGroups.Add(group);
                    group.Index = bondWireRegionGroups.Count;
                    group.Bond2_BallNums = int.Parse(groupNode.Attribute("Bond2Numbers")?.Value);
                }
                (createAutoBondMeasureModel as CreateAutoBondMeasureModel).GroupsCount = bondWireRegionGroups.Count;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        public void Dispose()
        {
            //htWindow.Dispose();
            (Content as Page_CreateRecipe).DataContext = null;
            (Content as Page_CreateRecipe).Close();
            (bondMeasureVerify as BondMeasureVerify).OnSaveXML -= SaveXML;
            Content = null;
            Procedures = null;
            //bondMeasureModelObject = null;
            bondMeasureParameter = null;
            bondMeasureVerifyParameter = null;
            bondMeasureVerifyParameterSet = null;
            bondModelUserRegions = null;
            bond1AutoUserRegion = null;
            createAutoBondMeasureModel.Dispose();
            createBondMeasureModel.Dispose();
            bondMeasureVerify.Dispose();
        }
    }
}
