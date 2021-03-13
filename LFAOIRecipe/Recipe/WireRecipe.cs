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
    class WireRecipe : ProcedureControl, IRecipe
    {
        //1121  using System.Collections.ObjectModel;
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;

        public const string XmlName = "WireRecipe.xml";

        public const string IdentifyString = "WIRERECIPEXML";

        private string RecipeDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\Wire{RecipeIndex.ToString()}\\");

        private string ModelsDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\Wire{RecipeIndex.ToString()}\\");

        private string ModelsFile => $"{ProductDirctory}\\Models\\";

        private string RecipeFile => $"{ProductDirctory}\\Recipe\\";

        public string XmlPath => $"{RecipeDirectory}{XmlName}";

        public int RecipeIndex { get; }

        public string RecipeName { get; } = "WireRecipe";

        private string ReferenceDirectory => $"{ProductDirctory}\\Recipe\\Reference\\";

        public static bool isRightClickBond = true;

        private readonly HTHalControlWPF htWindow;

        private WireObject wireObject = new WireObject();

        private WireParameter wireParameter = new WireParameter();

        public ObservableCollection<OnRecipe> startBondOnRecipes { get; set; } = new ObservableCollection<OnRecipe>();//起始焊点
        public ObservableCollection<OnRecipe> endBondOnRecipes { get; set; } = new ObservableCollection<OnRecipe>();//结束焊点

        public ObservableCollection<UserRegion> startBallAutoUserRegion { get; set; } = new ObservableCollection<UserRegion>();
        private ObservableCollection<UserRegion> stopBallAutoUserRegion { get; set; } = new ObservableCollection<UserRegion>();

        private ObservableCollection<WireAutoRegionGroup> wireRegionsModelGroup = new ObservableCollection<WireAutoRegionGroup>();

        private ObservableCollection<WireRegionsGroup> wireRegionsGroup = new ObservableCollection<WireRegionsGroup>();

        public static bool IsLoadXML = false;//

        //private readonly int wireIndex;

        #region 流程模块
        private IProcedure wireAddAutoRegion;
        private IProcedure wireAddRegion;
        private IProcedure wireInspectVerify;
        #endregion

        public WireRecipe(int wireIndex)
        {
            this.RecipeIndex = wireIndex;
            Content = new Page_CreateRecipe();
            (Content as Page_CreateRecipe).DataContext = this;
            htWindow = (Content as Page_CreateRecipe).htWindow;
            DisplayName = $"金线{this.RecipeIndex.ToString()}模板";

            wireAddAutoRegion = new WireAddAutoRegion(htWindow,
                                  ModelsFile,
                                  RecipeFile,
                                  ReferenceDirectory,
                                  wireObject,
                                  wireParameter,
                                  startBondOnRecipes,
                                  endBondOnRecipes,
                                  startBallAutoUserRegion,
                                  stopBallAutoUserRegion,
                                  wireRegionsModelGroup);

            wireAddRegion = new WireAddRegion(htWindow,
                                              ReferenceDirectory,
                                              wireObject,
                                              wireParameter,
                                              wireRegionsGroup,
                                              startBallAutoUserRegion,
                                              stopBallAutoUserRegion,
                                              wireRegionsModelGroup);

            wireInspectVerify = new WireInspectVerify(htWindow,
                                  ModelsFile,
                                  RecipeFile,
                                  ReferenceDirectory,
                                  wireObject,
                                  wireParameter,
                                  startBallAutoUserRegion,
                                  stopBallAutoUserRegion,
                                  wireRegionsGroup,
                                  ModelsDirectory);

            (wireInspectVerify as WireInspectVerify).OnSaveXML += SaveXML;

            IsLoadXML = false;

            htWindow.useAutoBondAndWire(wireAddAutoRegion as WireAddAutoRegion);

            Procedures = new IProcedure[]
            {
                wireAddAutoRegion,
                wireAddRegion,
                wireInspectVerify,
            };
            ProcedureChanged(0);
        }


        public void SaveXML()
        {
            try
            {
                Directory.GetFiles(ModelsDirectory).ToList().ForEach(File.Delete);
                XElement root = new XElement("Recipe");
                XMLHelper.AddIdentifier(root, IdentifyString);
                //
                XElement wireParameterNode = new XElement("WireParameter");
                XMLHelper.AddParameters(wireParameterNode, wireParameter, IdentifyString);

                //保存起始焊点归属
                XMLHelper.AddOnRecipes(root, startBondOnRecipes, "StartBondOnRecipes");
                XMLHelper.AddRegion(root, startBallAutoUserRegion, "StartBallAutoUserRegion", false,false,false,false);

                //保存结束焊点归属
                XMLHelper.AddOnRecipes(root, endBondOnRecipes, "EndBondOnRecipes");
                XMLHelper.AddRegion(root, stopBallAutoUserRegion, "StopBallAutoUserRegion", false,false,false,false);

                //金线模板组合
                XElement wireRegionsModelGroupNode = new XElement("WireRegionsModelGroup");
                foreach (var modelGroup in wireRegionsModelGroup)
                {
                    XElement modelGroupNode = new XElement("ModelGroup");
                    //组号索引
                    modelGroupNode.Add(new XAttribute("Index", modelGroup.Index.ToString()));
                    //金线模板序号选择
                    modelGroupNode.Add(new XAttribute("SelectModelNumber", modelGroup.SelectModelNumber.ToString()));
                    //模板起始焊点，结束焊点，焊点点金线，金线检测区域带参数
                    modelGroupNode.Add(modelGroup.ModelStartUserRegions?.ToXElement("ModelStartUserRegions"));
                    modelGroupNode.Add(modelGroup.ModelStopUserRegions?.ToXElement("ModelStopUserRegions"));
                    modelGroupNode.Add(modelGroup.RefLineModelRegions?.ToXElement("RefLineModelRegions"));
                    XMLHelper.AddRegion(modelGroupNode, modelGroup.LineModelRegions, "LineModelRegions", false,false,true,false);
                    wireRegionsModelGroupNode.Add(modelGroupNode);
                }

                //-----金线检测区域精修及保存区域
                XElement wireRegionsGroupNode = new XElement("WireRegionsGroup");
                foreach (var group in wireRegionsGroup)
                {
                    XElement groupNode = new XElement("Group");
                    groupNode.Add(new XAttribute("Index", group.Index.ToString()));
                    groupNode.Add(group.BondOnICUserRegions?.ToXElement("BondOnICUserRegions"));
                    groupNode.Add(group.BondOnFrameUserRegions?.ToXElement("BondOnFrameUserRegions"));
                    //groupNode.Add(group.RefLineUserRegions?.ToXElement("RefLineUserRegions"));
                    XMLHelper.AddRegion(groupNode, group.LineUserRegions, "LineUserRegions", false, true,false,false);
                    wireRegionsGroupNode.Add(groupNode);
                }


                root.Add(wireParameterNode);
                root.Add(wireRegionsModelGroupNode);
                root.Add(wireRegionsGroupNode);
                root.Save(XmlPath);

                HOperatorSet.GenEmptyObj(out HObject LineRegions);
                HTuple LineRegionsNumbers = new HTuple();
                HOperatorSet.WriteRegion(Algorithm.Region.ConcatRegion(wireRegionsGroup.Select(r => r.BondOnFrameUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)), ModelsDirectory + "Start_Regions.reg");
                HOperatorSet.WriteRegion(Algorithm.Region.ConcatRegion(wireRegionsGroup.Select(r => r.BondOnICUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)), ModelsDirectory + "Stop_Regions.reg");
                foreach (var group in wireRegionsGroup)
                {
                    LineRegions = LineRegions.ConcatObj(Algorithm.Region.ConcatRegion((group.LineUserRegions).Where(r => r.IsEnable).Select(r => r.CalculateRegion)));
                    LineRegionsNumbers = LineRegionsNumbers.TupleConcat(group.LineUserRegions.Count);
                }

                if (wireParameter.ImageCountChannels == 1)
                {
                    HOperatorSet.WriteTuple(new HTuple(1), ModelsDirectory + "Image_Index.tup");
                }
                else if (wireParameter.ImageCountChannels > 1)
                {
                    HOperatorSet.WriteTuple(wireParameter.ImageIndex + 1, ModelsDirectory + "Image_Index.tup");
                }

                HOperatorSet.WriteRegion(LineRegions, ModelsDirectory + "Inspect_Regions.reg");
                HOperatorSet.WriteTuple(LineRegionsNumbers, RecipeDirectory + "InspectRegNum.tup");
                HOperatorSet.WriteTuple(LineRegionsNumbers, ModelsDirectory + "InspectRegNum.tup");//Recipe下
                HOperatorSet.WriteTuple(wireParameter.IsTailInspect == false ? 0 : 1, ModelsDirectory + "IsInspectTail.tup");

                //保存起始焊点区域归属
                HTuple StartRegBelongToWhat = new HTuple();
                foreach (var StartonRecipe in startBondOnRecipes)
                {
                    if (StartonRecipe.IsSelected)
                    {
                        HOperatorSet.TupleConcat(StartRegBelongToWhat, new HTuple(StartonRecipe.Name), out StartRegBelongToWhat);
                    }
                }
                HOperatorSet.WriteTuple(StartRegBelongToWhat, ModelsDirectory + "StartRegBelongToWhat.tup");
                //1104 改成存带拾取顺序的组合焊点
                HOperatorSet.WriteTuple(new HTuple(wireParameter.StartBondonRecipesIndexs), ModelsDirectory + "StartRegBelongToWhat.tup");

                //保存结束焊点区域归属
                HTuple StopRegBelongToWhat = new HTuple();
                foreach (var StoponRecipe in endBondOnRecipes)
                {
                    if (StoponRecipe.IsSelected)
                    {
                        HOperatorSet.TupleConcat(StopRegBelongToWhat, new HTuple(StoponRecipe.Name), out StopRegBelongToWhat);
                    }

                }
                HOperatorSet.WriteTuple(StopRegBelongToWhat, ModelsDirectory + "StopRegBelongToWhat.tup");
                //1104 改成存带拾取顺序的组合焊点
                HOperatorSet.WriteTuple(new HTuple(wireParameter.StopBondonRecipesIndexs), ModelsDirectory + "StopRegBelongToWhat.tup");

                //1104 存储焊点组合后的重新排序信息
                HTuple Startreg_index_after_sort = new HTuple();
                HTuple ni;
                ni = 0;
                bool start_group_sort = false;
                foreach (var start_reg in startBallAutoUserRegion)
                {
                    //HOperatorSet.TupleConcat(Startreg_index_after_sort, new HTuple(start_reg.Index), out Startreg_index_after_sort);
                    Startreg_index_after_sort[ni] = start_reg.Index_ini;
                    ni++;
                    if (ni != start_reg.Index_ini)
                    {
                        start_group_sort = true;
                    }
                }
                HOperatorSet.WriteTuple(Startreg_index_after_sort, ModelsDirectory + "Startreg_index_after_sort.tup");
                HOperatorSet.WriteTuple(start_group_sort == false ? 0 : 1, ModelsDirectory + "Startreg_need_sort.tup");

                HTuple Stopreg_index_after_sort = new HTuple();
                ni = 0;
                bool stop_group_sort = false;
                foreach (var stop_reg in stopBallAutoUserRegion)
                {
                    //HOperatorSet.TupleConcat(Stopreg_index_after_sort, new HTuple(stop_reg.Index), out Stopreg_index_after_sort);
                    Stopreg_index_after_sort[ni] = stop_reg.Index_ini;
                    ni++;
                    if (ni != stop_reg.Index_ini)
                    {
                        stop_group_sort = true;
                    }
                }
                HOperatorSet.WriteTuple(Stopreg_index_after_sort, ModelsDirectory + "Stopreg_index_after_sort.tup");
                HOperatorSet.WriteTuple(stop_group_sort == false ? 0 : 1, ModelsDirectory + "Stopreg_need_sort.tup");

                HOperatorSet.WriteTuple(new HTuple(wireParameter.IsEnableStartVirtualBond).TupleConcat(new HTuple(wireParameter.IsEnableEndVirtualBond)), ModelsDirectory + "IsEnableVirtualBond.tup");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //加载
        public void LoadXML(string xmlPath)
        {
		    // 1216 lw 此处保存产品会出错
            /*await Task.Run(() =>
            {
                while (htWindow.HTWindow.HalconWindow.Handle == (IntPtr)0xffffffffffffffff)
                {
                    Thread.Sleep(500);
                }
            });*/

            IsLoadXML = true;

            try
            {
                XElement root = XElement.Load(xmlPath);
                XElement wireParameterNode = root.Element("WireParameter");
                if (wireParameterNode == null) return;
                XMLHelper.ReadParameters(wireParameterNode, wireParameter);

                //从金线自动生成区域加载参考信息
                (wireAddAutoRegion as WireAddAutoRegion).LoadReferenceData();

                //-----------------------------------------------------------------------------------------------------------------------------
                XElement wireRegionsGroupNode = root.Element("WireRegionsGroup");
                if (wireRegionsGroupNode == null) return;

                wireRegionsGroup.Clear();
                foreach (var groupNode in wireRegionsGroupNode.Elements("Group"))
                {
                    WireRegionsGroup group = new WireRegionsGroup();
                    group.BondOnICUserRegions = UserRegion.FromXElement(groupNode.Element("BondOnICUserRegions"), false, wireParameter.DieImageRowOffset, wireParameter.DieImageColumnOffset, false, false, false);
                    group.BondOnFrameUserRegions = UserRegion.FromXElement(groupNode.Element("BondOnFrameUserRegions"), false, wireParameter.DieImageRowOffset, wireParameter.DieImageColumnOffset, false, false, false);
                    //group.RefLineUserRegions = UserRegion.FromXElement(groupNode.Element("RefLineUserRegions"), false, wireParameter.DieImageRowOffset, wireParameter.DieImageColumnOffset);
                    XMLHelper.ReadRegion(groupNode, group.LineUserRegions, "LineUserRegions", wireParameter.DieImageRowOffset, wireParameter.DieImageColumnOffset, false, true, false, false);
                    
                    //1212 金线检测区域的通道值设置-生成线
                    for (int i = 0; i < group.LineUserRegions.Count(); i++)
                    {
                        if (group.LineUserRegions.ElementAt(i).ChannelNames.Count() == 0)
                        {
                            group.LineUserRegions.ElementAt(i).ChannelNames = wireParameter.ChannelNames; // currentChannelName
                            int tmp_ind;
                            tmp_ind = group.LineUserRegions.ElementAt(i).ImageIndex;
                        }
                    }
                    wireRegionsGroup.Add(group);
                    group.Index = wireRegionsGroup.Count;
                }
                (wireAddRegion as WireAddRegion).GroupsCount = wireRegionsGroup.Count;

                //-----加载自动生成金线界面
                //加载起始焊点区域归属
                startBondOnRecipes.Clear();
                XMLHelper.ReadOnRecipes(root, startBondOnRecipes, "StartBondOnRecipes");
                XMLHelper.ReadRegion(root, startBallAutoUserRegion, "StartBallAutoUserRegion", wireParameter.DieImageRowOffset,
                                     wireParameter.DieImageColumnOffset, false, false, false, false);

                // add lw
                HTuple hv__filePath = new HTuple(), hv_FileExists = new HTuple();
                HTuple Startreg_index_after_sort = new HTuple();
                hv__filePath = ModelsDirectory + "Startreg_index_after_sort.tup";
                HOperatorSet.FileExists(hv__filePath, out hv_FileExists);
                if ((int)(hv_FileExists) != 0)
                {
                    HOperatorSet.ReadTuple(hv__filePath, out Startreg_index_after_sort);
                }
                else
                {
                    Startreg_index_after_sort = HTuple.TupleGenSequence(1, startBallAutoUserRegion.Count, 1);
                }

                for (int i = 0; i < Startreg_index_after_sort.Length; i++)
                {
                    startBallAutoUserRegion[i].Index_ini = Startreg_index_after_sort[i];
                }

                if (startBondOnRecipes.Count == 0)
                {
                    IsLoadXML = false;
                    if (wireParameter.IsEnableStartVirtualBond == true)
                    {
                        wireParameter.IsEnableStartVirtualBond = false;
                        wireParameter.IsEnableStartVirtualBond = true;
                    }
                    if (wireParameter.IsEnableStartVirtualBond == false)
                    {
                        wireParameter.IsEnableStartVirtualBond = true;
                        wireParameter.IsEnableStartVirtualBond = false;
                    }

                }

                //加载结束焊点归属
                endBondOnRecipes.Clear();
                XMLHelper.ReadOnRecipes(root, endBondOnRecipes, "EndBondOnRecipes");
                XMLHelper.ReadRegion(root, stopBallAutoUserRegion, "StopBallAutoUserRegion", wireParameter.DieImageRowOffset,
                                     wireParameter.DieImageColumnOffset, false, false, false, false);

                // add lw
                HTuple Stopreg_index_after_sort = new HTuple();
                hv__filePath = ModelsDirectory + "Stopreg_index_after_sort.tup";
                HOperatorSet.FileExists(hv__filePath, out hv_FileExists);
                if ((int)(hv_FileExists) != 0)
                { 
                    HOperatorSet.ReadTuple(hv__filePath, out Stopreg_index_after_sort);
                }
                else
                {
                    Stopreg_index_after_sort = HTuple.TupleGenSequence(1, stopBallAutoUserRegion.Count, 1);
                }

                for (int i = 0; i < Stopreg_index_after_sort.Length; i++)
                {
                    stopBallAutoUserRegion[i].Index_ini = Stopreg_index_after_sort[i];
                }

                if (endBondOnRecipes.Count == 0)
                {
                    IsLoadXML = false;
                    if (wireParameter.IsEnableEndVirtualBond == true)
                    {
                        wireParameter.IsEnableEndVirtualBond = false;
                        wireParameter.IsEnableEndVirtualBond = true;
                    }
                    if (wireParameter.IsEnableEndVirtualBond == false)
                    {
                        wireParameter.IsEnableEndVirtualBond = true;
                        wireParameter.IsEnableEndVirtualBond = false;
                    }
                }

                //---------------生成界面中金线
                (wireAddAutoRegion as WireAddAutoRegion).UpdateStartandStopLineRegions(true);

                //金线模板Model
                XElement wireRegionsModelGroupNode = root.Element("WireRegionsModelGroup");
                if (wireRegionsModelGroupNode == null) return;
                wireRegionsModelGroup.Clear();

                foreach (var modelGroupNode in wireRegionsModelGroupNode.Elements("ModelGroup"))
                {
                    WireAutoRegionGroup modelGroup = new WireAutoRegionGroup();
                    modelGroup.ModelStartUserRegions = UserRegion.FromXElement(modelGroupNode.Element("ModelStartUserRegions"), 
                                                                               false, wireParameter.DieImageRowOffset, wireParameter.DieImageColumnOffset,false,false,false);
                    modelGroup.ModelStopUserRegions = UserRegion.FromXElement(modelGroupNode.Element("ModelStopUserRegions"), 
                                                                               false, wireParameter.DieImageRowOffset, wireParameter.DieImageColumnOffset,false,false,false);
                    modelGroup.RefLineModelRegions = UserRegion.FromXElement(modelGroupNode.Element("RefLineModelRegions"), false, 
                                                                               wireParameter.DieImageRowOffset, wireParameter.DieImageColumnOffset,false,false,false);
                    XMLHelper.ReadRegion(modelGroupNode, modelGroup.LineModelRegions, "LineModelRegions", wireParameter.DieImageRowOffset, 
                                         wireParameter.DieImageColumnOffset, false, false,true,false);
                    //1212 金线检测区域的通道值设置-模板线
                    for (int i = 0; i < modelGroup.LineModelRegions.Count(); i++)
                    {
                        if (modelGroup.LineModelRegions.ElementAt(i).ChannelNames.Count() == 0)
                        {
                            modelGroup.LineModelRegions.ElementAt(i).ChannelNames = wireParameter.ChannelNames;
                            int tmp_ind;
                            tmp_ind = modelGroup.LineModelRegions.ElementAt(i).ImageIndex;
                        }
                    }
                    wireRegionsModelGroup.Add(modelGroup);
                    modelGroup.Index = wireRegionsModelGroup.Count;

                    modelGroup.SelectModelNumber = (int)modelGroupNode.FirstAttribute.NextAttribute;
                }
                (wireAddAutoRegion as WireAddAutoRegion).ModelGroupsCount = wireRegionsModelGroup.Count;

                //恢复自动生成金线起始区域金线ModelType  add by wj
                for (int i = 0; i < wireParameter.WireRegModelType.Length; i++)
                {
                    startBallAutoUserRegion[i].ModelGroups = wireRegionsModelGroup;
                    startBallAutoUserRegion[i].CurrentModelGroup = wireRegionsModelGroup.ElementAt(wireParameter.WireRegModelType[i] - 1);

                }

                //1109 赋值初始排序
                if (wireParameter.WireAutoIndex_sorted_start.Length == startBallAutoUserRegion.Count)
                {
                    for (int i = 0; i < wireParameter.WireAutoIndex_sorted_start.Length; i++)
                    {
                        startBallAutoUserRegion[i].Index_ini = wireParameter.WireAutoIndex_sorted_start[i];
                    }
                }
                if (wireParameter.WireAutoIndex_sorted_stop.Length == stopBallAutoUserRegion.Count)
                {
                    for (int i = 0; i < wireParameter.WireAutoIndex_sorted_stop.Length; i++)
                    {
                        stopBallAutoUserRegion[i].Index_ini = wireParameter.WireAutoIndex_sorted_stop[i];
                    }
                }

                //1029 default model setting
                if (wireRegionsModelGroup.Count > 0)
                {
                    (wireAddAutoRegion as WireAddAutoRegion).CurrentModelGroup = (wireAddAutoRegion as WireAddAutoRegion).ModelGroups.ElementAt(0);
                }

                if ((wireAddAutoRegion as WireAddAutoRegion).StartBallAutoUserRegion.Count > 0)
                {
                    (wireAddAutoRegion as WireAddAutoRegion).WireParameter.IsStartPickUp = true;
                }
                if ((wireAddAutoRegion as WireAddAutoRegion).StopBallAutoUserRegion.Count > 0)
                {
                    (wireAddAutoRegion as WireAddAutoRegion).WireParameter.IsStopPickUp = true;
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
            Content = null;
            (wireInspectVerify as WireInspectVerify).OnSaveXML -= SaveXML;
            Procedures = null;

            wireAddAutoRegion.Dispose();
            wireAddRegion.Dispose();
            wireInspectVerify.Dispose();

            wireAddRegion = null;
            wireInspectVerify = null;

            wireObject = null;
            wireParameter = null;
            startBondOnRecipes = null;
            endBondOnRecipes = null;
            startBallAutoUserRegion = null;
            stopBallAutoUserRegion = null;
            wireRegionsModelGroup = null;
            wireRegionsGroup = null;

        }
    }
}
