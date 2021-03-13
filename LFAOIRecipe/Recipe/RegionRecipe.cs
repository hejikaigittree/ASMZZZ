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

    class FreeRegionParameter : ViewModelBase, IParameter
    {
        private int index;
        public int Index
        {
            get => index;
            set => OnPropertyChanged(ref index, value);
        }

        private int lastIndex;
        public int LastIndex
        {
            get => lastIndex;
            set => OnPropertyChanged(ref lastIndex, value);
        }

        private RegionType regionType;
        public RegionType RegionType
        {
            get => regionType;
            set => OnPropertyChanged(ref regionType, value);
        }

        // 自定义区域操作类别
        private RegionOperatType regionOperatType;
        public RegionOperatType RegionOperatType
        {
            get => regionOperatType;
            set => OnPropertyChanged(ref regionOperatType, value);
        }

        private double[] regionParameters;
        public double[] RegionParameters
        {
            get => regionParameters;
            set => OnPropertyChanged(ref regionParameters, value);
        }

        private bool isAccept = true;
        public bool IsAccept
        {
            get => isAccept;
            set => OnPropertyChanged(ref isAccept, value);
        }

    }

    class FreeRegionRecipe : ProcedureControl, IRecipe
    {
        //1121  using System.Collections.ObjectModel;
        public ObservableCollection<ChannelName> ChannelNames { get; set; } = new ObservableCollection<ChannelName>();//通道名

        public string DisplayName { get; }

        public object Content { get; private set; }

        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;

        public const string XmlName = "FreeRegionRecipe.xml";

        public const string IdentifyString = "FREEREGIONRECIPEXML";

        private string RecipeDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\FreeRegion{RecipeIndex.ToString()}\\");

        private string ModelsDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\FreeRegion{RecipeIndex.ToString()}\\");

        private string ModelsFile => $"{ProductDirctory}\\Models\\";

        private string RecipeFile => $"{ProductDirctory}\\Recipe\\";

        public string XmlPath => $"{RecipeDirectory}{XmlName}";

        public int RecipeIndex { get; } = 0;

        public string RecipeName { get; } = "FreeRegionRecipe";

        private string ReferenceDirectory => $"{ProductDirctory}\\Recipe\\Reference\\";

        public static bool isRightClickBond = true;//

        private readonly HTHalControlWPF htWindow;

        private EpoxyModelObject epoxyModelObject = new EpoxyModelObject();

        private EpoxyParameter epoxyParameter = new EpoxyParameter();

        private ObservableCollection<UserRegion> OperRegionUserRegions = new ObservableCollection<UserRegion>();

        #region 流程模块
        private IProcedure createRegionModel;
        #endregion

        public FreeRegionRecipe(int freeRegionIndex)
        {
            this.RecipeIndex = freeRegionIndex;
            Content = new Page_CreateRecipe();
            (Content as Page_CreateRecipe).DataContext = this;
            htWindow = (Content as Page_CreateRecipe).htWindow;
            DisplayName = $"FreeRegion{this.RecipeIndex.ToString()}模板";

            createRegionModel = new CreateRegionModel(htWindow,
                                                ModelsFile,
                                                RecipeFile,
                                                ReferenceDirectory,
                                                epoxyModelObject,
                                                epoxyParameter,
                                                OperRegionUserRegions,
                                                ModelsDirectory);

            (createRegionModel as CreateRegionModel).OnSaveXML += SaveXML;

            Procedures = new IProcedure[]
            {
                createRegionModel,
            };
            ProcedureChanged(0);
        }

        //保存 
        public void SaveXML()
        {
            try
            {
                //Directory.GetFiles(ModelsDirectory).ToList().ForEach(File.Delete);
                XElement root = new XElement("Recipe");
                XMLHelper.AddIdentifier(root, IdentifyString);
                XElement freeRegionParameterNode = new XElement("FreeRegionParameterNode");
                XMLHelper.AddParameters(freeRegionParameterNode, epoxyParameter, IdentifyString);

                XElement operRegionUserRegionsNode = new XElement("OperRegionUserRegions");
                XElement operListNumNode = new XElement("OperRegionCount");

                operListNumNode.Value = OperRegionUserRegions.Count.ToString();

                operRegionUserRegionsNode.Add(operListNumNode);

                FreeRegionParameter SingleRegionParameters = new FreeRegionParameter();

                for (int i = 0; i < OperRegionUserRegions.Count; i++)
                {
                    XElement operListNode = new XElement("Operation");
                    //operListNode.Add(new XAttribute("Index", OperRegionUserRegions[i].Index.ToString()));

                    SingleRegionParameters.Index = OperRegionUserRegions[i].Index;
                    SingleRegionParameters.RegionType = OperRegionUserRegions[i].RegionType;
                    SingleRegionParameters.RegionOperatType = OperRegionUserRegions[i].RegionOperatType;
                    SingleRegionParameters.RegionParameters = OperRegionUserRegions[i].RegionParameters;
                    SingleRegionParameters.IsAccept = OperRegionUserRegions[i].IsAccept;
                    SingleRegionParameters.LastIndex = OperRegionUserRegions[i].LastIndex;

                    XMLHelper.AddParameters(operListNode, SingleRegionParameters, IdentifyString);
                    operRegionUserRegionsNode.Add(operListNode);
                }

                root.Add(freeRegionParameterNode);
                root.Add(operRegionUserRegionsNode);
                root.Save(XmlPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //加载
        public /*async*/ void LoadXML(string xmlPath)
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
                XElement operRegionUserRegionsNode = root.Element("OperRegionUserRegions");
                if (operRegionUserRegionsNode == null) return;
                XElement freeRegionParameterNode = root.Element("FreeRegionParameterNode");
                if (freeRegionParameterNode == null) return;
                XMLHelper.ReadParameters(freeRegionParameterNode, epoxyParameter);
                (createRegionModel as CreateRegionModel).LoadReferenceData();

                OperRegionUserRegions.Clear();

                foreach (var operationNode in operRegionUserRegionsNode.Elements("Operation"))
                {
                    FreeRegionParameter SingleRegionParameters = new FreeRegionParameter();
                    XMLHelper.ReadParameters(operationNode, SingleRegionParameters);

                    switch (SingleRegionParameters.RegionType)
                    {
                        case RegionType.Point:
                            break;
                        case RegionType.Rectangle1:
                            UserRegion userRegionRectangle1 = UserRegion.GetHWindowRegionUpdate(htWindow, SingleRegionParameters.RegionType,
                                                            SingleRegionParameters.RegionParameters[0] - epoxyParameter.DieImageRowOffset,
                                                            SingleRegionParameters.RegionParameters[1] - epoxyParameter.DieImageColumnOffset,
                                                            SingleRegionParameters.RegionParameters[2] - epoxyParameter.DieImageRowOffset,
                                                            SingleRegionParameters.RegionParameters[3] - epoxyParameter.DieImageColumnOffset,
                                                            epoxyParameter.DieImageRowOffset, epoxyParameter.DieImageColumnOffset);

                            if (userRegionRectangle1 == null) return;
                            userRegionRectangle1.Index = OperRegionUserRegions.Count + 1;
                            userRegionRectangle1.LastIndex = SingleRegionParameters.LastIndex;
                            userRegionRectangle1.IsAccept = SingleRegionParameters.IsAccept;
                            OperRegionUserRegions.Add(userRegionRectangle1);
                            break;
                        case RegionType.Rectangle2:
                            UserRegion userRegionRectangle2 = UserRegion.GetHWindowRegionUpdate(htWindow, SingleRegionParameters.RegionType,
                                                            SingleRegionParameters.RegionParameters[0] - epoxyParameter.DieImageRowOffset,
                                                            SingleRegionParameters.RegionParameters[1] - epoxyParameter.DieImageColumnOffset,
                                                            SingleRegionParameters.RegionParameters[3],
                                                            SingleRegionParameters.RegionParameters[4],
                                                            epoxyParameter.DieImageRowOffset,
                                                            epoxyParameter.DieImageColumnOffset,
                                                            SingleRegionParameters.RegionParameters[2]);
                            if (userRegionRectangle2 == null) return;
                            userRegionRectangle2.Index = OperRegionUserRegions.Count + 1;
                            userRegionRectangle2.LastIndex = SingleRegionParameters.LastIndex;
                            userRegionRectangle2.IsAccept = SingleRegionParameters.IsAccept;
                            OperRegionUserRegions.Add(userRegionRectangle2);
                            break;
                        case RegionType.Circle:
                            UserRegion userRegionCircle = UserRegion.GetHWindowRegionUpdate(htWindow, SingleRegionParameters.RegionType,
                                                            SingleRegionParameters.RegionParameters[0] - epoxyParameter.DieImageRowOffset,
                                                            SingleRegionParameters.RegionParameters[1] - epoxyParameter.DieImageColumnOffset,
                                                            SingleRegionParameters.RegionParameters[2], 
                                                            0, epoxyParameter.DieImageRowOffset, 
                                                            epoxyParameter.DieImageColumnOffset, 0);
                            if (userRegionCircle == null) return;
                            userRegionCircle.Index = OperRegionUserRegions.Count + 1;
                            userRegionCircle.LastIndex = SingleRegionParameters.LastIndex;
                            userRegionCircle.IsAccept = SingleRegionParameters.IsAccept;
                            OperRegionUserRegions.Add(userRegionCircle);
                            break;
                        case RegionType.Ellipse:
                            UserRegion userRegionEllipse = UserRegion.GetHWindowRegionUpdate(htWindow, SingleRegionParameters.RegionType,
                                                            SingleRegionParameters.RegionParameters[0] - epoxyParameter.DieImageRowOffset,
                                                            SingleRegionParameters.RegionParameters[1] - epoxyParameter.DieImageColumnOffset,
                                                            SingleRegionParameters.RegionParameters[3],
                                                            SingleRegionParameters.RegionParameters[4], 
                                                            epoxyParameter.DieImageRowOffset, 
                                                            epoxyParameter.DieImageColumnOffset, 
                                                            SingleRegionParameters.RegionParameters[2]);
                            if (userRegionEllipse == null) return;
                            userRegionEllipse.Index = OperRegionUserRegions.Count + 1;
                            userRegionEllipse.LastIndex = SingleRegionParameters.LastIndex;
                            userRegionEllipse.IsAccept = SingleRegionParameters.IsAccept;
                            OperRegionUserRegions.Add(userRegionEllipse);
                            break;

                        case RegionType.Operation:
                            UserRegion userRegion0 = new UserRegion
                            {
                                //DisplayRegion在加载全局数据中生成
                                //DisplayRegion = (createRegionModel as CreateRegionModel).RunSteps(OperRegionUserRegions.Count + 1),
                                RegionType = SingleRegionParameters.RegionType,
                                RegionOperatType = SingleRegionParameters.RegionOperatType,
                                RegionParameters = SingleRegionParameters.RegionParameters,
                                IsAccept = SingleRegionParameters.IsAccept,
                                LastIndex = SingleRegionParameters.LastIndex,
                            };

                            if (userRegion0 == null) return;
                            userRegion0.Index = OperRegionUserRegions.Count + 1;
                            OperRegionUserRegions.Add(userRegion0);
                            break;

                        default:
                            UserRegion userRegion = new UserRegion
                            {
                                //DisplayRegion在加载全局数据中生成
                                //DisplayRegion = (createRegionModel as CreateRegionModel).RunSteps(OperRegionUserRegions.Count + 1),
                                RegionType = SingleRegionParameters.RegionType,
                                RegionOperatType = SingleRegionParameters.RegionOperatType,
                                RegionParameters = SingleRegionParameters.RegionParameters,
                                IsAccept = SingleRegionParameters.IsAccept,
                                LastIndex = SingleRegionParameters.LastIndex,
                            };

                            if (userRegion == null) return;
                            userRegion.Index = OperRegionUserRegions.Count + 1;
                            OperRegionUserRegions.Add(userRegion);
                            break;
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
            htWindow.Dispose();
            (Content as Page_CreateRecipe).DataContext = null;
            (Content as Page_CreateRecipe).Close();
            (createRegionModel as CreateRegionModel).OnSaveXML -= SaveXML;
            createRegionModel.Dispose();
            Content = null;
            epoxyModelObject = null;
            epoxyParameter = null;
            OperRegionUserRegions = null;
            Procedures = null;
        }
    }
}
