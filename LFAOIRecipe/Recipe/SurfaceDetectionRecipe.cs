using HalconDotNet;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LFAOIRecipe
{
    class SurfaceDetectionRecipe : ProcedureControl, IRecipe
    {
        public string DisplayName { get; }

        public object Content { get; private set; }

        public string ProductDirctory { get; set; } = FilePath.ProductDirectory;

        public const string XmlName = "SurfaceDetectionRecipe.xml";

        public const string IdentifyString = "SURFACEDETECTIONRECIPEXML";

        private string RecipeDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Recipe\\SurfaceDetection{RecipeIndex.ToString()}\\");

        private string ModelsDirectory => FilePath.EnsureDirectoryExisted($"{ProductDirctory}\\Models\\SurfaceDetection{RecipeIndex.ToString()}\\");

        private string ModelsFile => $"{ProductDirctory}\\Models\\";

        private string RecipeFile => $"{ProductDirctory}\\Recipe\\";

        public string XmlPath => $"{RecipeDirectory}{XmlName}";

        public int RecipeIndex { get; } = 0;

        public string RecipeName { get; } = "SurfaceDetectionRecipe";

        private string ReferenceDirectory => $"{ProductDirctory}\\Recipe\\Reference\\";

        public static bool isRightClickBond = true;

        private readonly HTHalControlWPF htWindow;

        private EpoxyModelObject SurfaceDetectionObject = new EpoxyModelObject();

        private GoldenModelParameter SurfaceDetectionParameter = new GoldenModelParameter();

        private ObservableCollection<UserRegion> LoadRegionUserRegions = new ObservableCollection<UserRegion>();

        public FrameModelInspectParameter FrameModelInspectParameter = new FrameModelInspectParameter();

        public PegRackModelInspectParameter PegRackModelInspectParameter = new PegRackModelInspectParameter();

        public BridgeModelInspectParameter BridgeModelInspectParameter = new BridgeModelInspectParameter();


        #region 流程模块
        private IProcedure surfaceDetection;
        #endregion

        public SurfaceDetectionRecipe(int surfaceDetectionIndex)
        {
            this.RecipeIndex = surfaceDetectionIndex;
            Content = new Page_CreateRecipe();
            (Content as Page_CreateRecipe).DataContext = this;
            htWindow = (Content as Page_CreateRecipe).htWindow;
            DisplayName = $"表面检测{this.RecipeIndex.ToString()}模板";

            surfaceDetection = new SurfaceDetection(htWindow,
                                               SurfaceDetectionObject,
                                                SurfaceDetectionParameter,
                                                ReferenceDirectory,
                                                ModelsFile,
                                                RecipeFile,
                                                FrameModelInspectParameter,
                                                PegRackModelInspectParameter,
                                                BridgeModelInspectParameter,
                                                LoadRegionUserRegions,
                                                ModelsDirectory);

            (surfaceDetection as SurfaceDetection).OnSaveXML += SaveXML;

            Procedures = new IProcedure[]
            {
                surfaceDetection,
            };
            ProcedureChanged(0);
        }

        //保存 后续添加
        public void SaveXML()
        {
            try
            {
                XElement root = new XElement("Recipe");
                XMLHelper.AddIdentifier(root, IdentifyString);
                XElement surfaceDetectionParameterNode = new XElement("SurfaceDetectionParameter");
                XElement frameInspectParameterNode = new XElement("FrameInspectParameter");
                XElement pegRackInspectParameterNode = new XElement("PegRackInspectParameter");
                XElement bridgeModelInspectParameterNode = new XElement("BridgeInspectParameter");

                XMLHelper.AddIdentifier(root, IdentifyString);

                XMLHelper.AddParameters(surfaceDetectionParameterNode, SurfaceDetectionParameter, IdentifyString);
                XMLHelper.AddParameters(frameInspectParameterNode, FrameModelInspectParameter, IdentifyString);
                XMLHelper.AddParameters(pegRackInspectParameterNode, PegRackModelInspectParameter, IdentifyString);
                XMLHelper.AddParameters(bridgeModelInspectParameterNode, BridgeModelInspectParameter, IdentifyString);

                root.Add(surfaceDetectionParameterNode);
                root.Add(frameInspectParameterNode);
                root.Add(pegRackInspectParameterNode);
                root.Add(bridgeModelInspectParameterNode);

                root.Save(XmlPath);
                root.Save($"{RecipeFile}{SurfaceDetectionParameter.OnRecipesIndexs[SurfaceDetectionParameter.OnRecipesIndex]}\\{XmlName}");

                if (SurfaceDetectionParameter.OnRecipesIndex != -1)
                {
                    if (SurfaceDetectionParameter.ImageCountChannels == 1)
                    {
                        HOperatorSet.WriteTuple((new HTuple(1)).TupleConcat(new HTuple(1)).TupleConcat(new HTuple(1)),
                           $"{ModelsFile}{SurfaceDetectionParameter.OnRecipesIndexs[SurfaceDetectionParameter.OnRecipesIndex]}\\Inspect_ImageIndex.tup");
                    }
                    else if (SurfaceDetectionParameter.ImageCountChannels >1)
                    {
                        if (FrameModelInspectParameter.ImageFrameVerifyChannelIndex < 0 || PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex < 0 || BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex < 0)
                        {
                            MessageBox.Show("没有选择检测验证通道图像！");
                        }
                        else
                        {
                            //保存在所选框架上  暂时保存在同一个框架上  1211 lw 图层+1
                            HOperatorSet.WriteTuple((new HTuple(FrameModelInspectParameter.ImageFrameVerifyChannelIndex+1))
                                                    .TupleConcat(new HTuple(PegRackModelInspectParameter.ImagePegRackVerifyChannelIndex+1))
                                                    .TupleConcat(new HTuple(BridgeModelInspectParameter.ImageBridgeVerifyChannelIndex+1)),
                                                    $"{ModelsFile}{SurfaceDetectionParameter.OnRecipesIndexs[SurfaceDetectionParameter.OnRecipesIndex]}\\Inspect_ImageIndex.tup");
                        }
                    }

                    HOperatorSet.WriteTuple(new HTuple(FrameModelInspectParameter.IsFrameSurfaceInspect == false ? 0 : 1)
                                .TupleConcat(new HTuple(PegRackModelInspectParameter.IsPegRackSurfaceInspect == false ? 0 : 1))
                                .TupleConcat(new HTuple(BridgeModelInspectParameter.IsBridgeSurfaceInspect == false ? 0 : 1)), $"{ModelsFile}{SurfaceDetectionParameter.OnRecipesIndexs[SurfaceDetectionParameter.OnRecipesIndex]}\\TaskEnable.tup");

                }
                    HOperatorSet.WriteTuple(new HTuple(FrameModelInspectParameter.IsFrameSurfaceInspect == false ? 0 : 1)
                            .TupleConcat(new HTuple(PegRackModelInspectParameter.IsPegRackSurfaceInspect == false ? 0 : 1))
                            .TupleConcat(new HTuple(BridgeModelInspectParameter.IsBridgeSurfaceInspect == false ? 0 : 1)), ModelsDirectory + "TaskEnable.tup");
                }

            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //加载 后续添加
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
                XElement surfaceDetectionParameterNode = root.Element("SurfaceDetectionParameter");
                if (surfaceDetectionParameterNode == null) return;
                XMLHelper.ReadParameters(surfaceDetectionParameterNode, SurfaceDetectionParameter);
                (surfaceDetection as SurfaceDetection).LoadReferenceData();
                
                XElement frameInspectParameterNode = root.Element("FrameInspectParameter");
                XElement pegRackInspectParameterNode = root.Element("PegRackInspectParameter");
                XElement bridgeModelInspectParameterNode = root.Element("BridgeInspectParameter");

                //if (epoxyParameterNode == null) return;
                XMLHelper.ReadParameters(frameInspectParameterNode, FrameModelInspectParameter);
                XMLHelper.ReadParameters(pegRackInspectParameterNode, PegRackModelInspectParameter);
                XMLHelper.ReadParameters(bridgeModelInspectParameterNode, BridgeModelInspectParameter);

                (surfaceDetection as SurfaceDetection).Initial();
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
            (surfaceDetection as SurfaceDetection).OnSaveXML -= SaveXML;
            surfaceDetection.Dispose();
            Content = null;
            SurfaceDetectionObject = null;
            SurfaceDetectionParameter = null;
            LoadRegionUserRegions = null;
            Procedures = null;
        }
    }
}
